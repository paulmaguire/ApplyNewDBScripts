using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using ApplyNewDBScriptsV2.Entities;
using Microsoft.Win32;

namespace ApplyNewDBScriptsV2
{
    static class Program
    {
        #region Private Properties

        private static string strConnectionString { get; set; }
        private static string strConfigPath { get; set; }
        private static string strConfigConnectionName { get; set; }
        private static string strTail { get; set; }
        private static string strXMLConfigFile { get; set; }
        private static string strLogFile { get; set; }
        private static bool bLookupNoIdentity { get; set; }
        private static bool bStopOnError { get; set; }
        private static bool HideScriptVariablePopup { get; set; }
        private static bool bQuietMode { get; set; }

        private static StreamWriter WriterLog { get; set; }
        private static DateTime dtStart { get; set; }
        private static List<string> strSvnFiles { get; set; }
        private static List<SVNInfo> SVNInfoForScriptsToApply { get; set; }

        private static bool bLogDated { get; set; }
        private static string _logFileName = string.Empty;
        private static string strServer { get; set; }
        private static string strDatabase { get; set; }
        private static string strUser { get; set; }
        private static string strPassword { get; set; }
        private static string strCurrentEnv { get; set; }
        private static string strClientScriptFolder { get; set; }

        private static string GetLogFileName()
        {
            if (_logFileName == string.Empty)
            {
                _logFileName = Path.GetTempFileName();
            }

            return _logFileName;
        }

        private static Dictionary<string, string> _dicSettings;
        //private static List<string> _listExclude = new List<string>();
        private static frmConfirmApply ConfirmationPopup { get; set; }
        private const string seperator = "==================================================";
        private const int constExceptionOccured = 2;
        private const int constNotAllScriptsSuccessfullyApplied = 3;
        public static bool IsInstallerRunningProgram
        {
            get
            {
                // If username is system or network service and domain name does not start with rainmaker then this is the installer
                return (((System.Environment.UserName.ToLower() == "system") || (System.Environment.UserName.ToLower() == "network service")) &&
                    !System.Environment.UserDomainName.ToLower().StartsWith("rainmaker"));

            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is been run by a developer or if a windows service is running the program.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is been run by a windows service; otherwise, <c>false</c>.
        /// </value>
        private static bool IsServiceRunningProgram
        {
            get
            {
                if (bQuietMode) return true;

                string serviceUser = ConfigurationManager.AppSettings["AutomationServerUser"];
                foreach (var appSettingKey in ConfigurationManager.AppSettings.AllKeys)
                {
                    if (appSettingKey.Contains("AutomationServer") && System.Environment.MachineName.ToLower() == ConfigurationManager.AppSettings[appSettingKey]
                        && (System.Environment.UserName.ToLower() == serviceUser || serviceUser == ""))
                    {
                        return true;
                    }
                }
                // return ((System.Environment.UserName.ToLower() == "system") || (System.Environment.UserName.ToLower() == "network service"));
                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            strLogFile = GetLogFileName();

            using (WriterLog = new StreamWriter(strLogFile, false))
            {
                WriterLog.AutoFlush = true;
                LogMessage("Started at " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), true);
                LogMessage(seperator);
                WriterLog.Flush();

                try
                {
                    dtStart = DateTime.Now;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    //Parse Parameters and validate them after to ensure enough information is supplied for a successful run
                    if (!ParseParameters(args)) return;
                }
                catch (Exception Ex)
                {
                    LogMessage(Ex.ToString());
                }

                WriterLog.Flush();
            }

            try
            {
                if (!RenameLogFile()) return;
            }
            catch (IOException Ex)
            {
                LogMessage(seperator, true);
                LogMessage(Ex.Message, true);
                LogMessage(Ex.StackTrace, true);
                LogMessage(seperator, true);

                throw new Exception("Error when renaming temp log file:", Ex);
            }

            using (WriterLog = new StreamWriter(strLogFile, true))
            {
                WriterLog.AutoFlush = true;

                try
                {
                    if (!string.IsNullOrEmpty(strLogFile) && !string.IsNullOrEmpty(strTail))
                    {
                        try
                        {
                            Process.Start(strTail, strLogFile);
                        }
                        catch (Exception Ex)
                        {
                            LogMessage(seperator, true);
                            LogMessage(Ex.Message, true);
                            LogMessage(Ex.StackTrace, true);
                            LogMessage(seperator, true);
                        }
                    }
                    
                    if (!ParseConnectionString()) 
                        return;

                    bGitUsed = CheckIfGitIsUsed(strXMLConfigFile);

                    // Check to ensure we are allowed run on this computer
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
                    {
                        if (key != null)
                        {
                            if (bool.Parse(key.GetValue(strCurrentEnv + "Never", false).ToString()))
                            {
                                return;
                            }
                        }
                    }

                    try
                    {
                        // if running as a service don't ask user 
                        if (IsServiceRunningProgram)
                        {
                            ApplyScriptsConfirmed();
                        }
                        else
                        {
                            // Ask for confirmation of scripts to apply
                            LogMessage("Asking for confirmation");

                            ConfirmationPopup = new frmConfirmApply();
                            ConfirmationPopup.ButtonClick_Yes += new frmConfirmApply.ButtonClick(ConfirmationPopup_ButtonClick_Yes);
                            ConfirmationPopup.ButtonClick_No += new frmConfirmApply.ButtonClick(ConfirmationPopup_ButtonClick_No);

                            ConfirmationPopup.Show(strServer + "\\" + strDatabase, strXMLConfigFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry("ApplyNewDBScripts",
                                            string.Format(
                                                @"Error in ApplyScriptsConfirmed(). Exception Message = {0} Stack Trace = {1}",
                                                ex.Message, ex.StackTrace), EventLogEntryType.Error);

                        if (!IsServiceRunningProgram)
                            MessageBox.Show(ex.ToString(), "Error Asking For Confirmation", MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);

                        LogMessage(seperator, true);
                        LogMessage(ex.ToString(), true);
                        LogMessage(seperator, true);
                        Environment.Exit(constExceptionOccured);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(seperator, true);
                    LogMessage(ex.ToString(), true);;
                    LogMessage(seperator, true);
                    Environment.Exit(constExceptionOccured);
                }

                WriterLog.Flush();
            }
        }

        private static bool CheckIfGitIsUsed(string strXmlConfigFile)
        {
            if (strSvnFiles.Count > 0)
            {
                // we are in apply mode
                using (StreamReader reader = new StreamReader(strSvnFiles[0]))
                {
                    string line1 = reader.ReadLine();
                    if (line1 == "<?xml version=\"1.0\" encoding=\"UTF-8\"?>")
                        return false;
                    return true;
                }
            }
            return GetGitInfo(Path.GetDirectoryName(strXmlConfigFile)) != null;
        }

        static void ConfirmationPopup_ButtonClick_No(string selectedSettingsFilePath)
        {
            LogMessage(DateTime.Now.ToString() + " : User has chosen not to apply scripts");
            Finished();
        }

        static void ConfirmationPopup_ButtonClick_Yes(string selectedSettingsFilePath)
        {
            var fileInfo = new FileInfo(strXMLConfigFile);
            strXMLConfigFile = string.Format(@"{0}\{1}", fileInfo.Directory, selectedSettingsFilePath);
            ApplyScriptsConfirmed();
            Finished();
        }

        static MemoryBoxResult? _mbrApplyScripts = null;

        static MemoryBoxResult? _mbrApplyUnversionedScripts = null;
        private static bool bGitUsed;

        public static string GetFileMD5(string filename)
        {
            StringBuilder sb = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                hash = md5.ComputeHash(fs);
            }
            foreach (byte hex in hash)
                sb.AppendFormat("{0:x2}", hex);
            return sb.ToString();
        }

        public static bool LookupTableHasIdentity(string sql)
        {
            // Quick check for keywords
            sql = sql.ToLower();
            if (sql.Contains("create") && sql.Contains("table") && sql.Contains("lookup") && sql.Contains("identity"))
            {
                // Remove single line comments
                sql = Regex.Replace(sql, "--.*", "", RegexOptions.Multiline);

                // Convert to single line with consistent spacing
                sql = sql.ToLower().Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');

                // Remove comments
                sql = Regex.Replace(sql, @"\/\*.*\*\/", "", RegexOptions.None);

                while (sql.Contains("  "))
                    sql = sql.Replace("  ", " ");

                int iPos;
                string strTemp;
                int iOpenCount;
                int iOpenPos = 0;
                int iClosedPos = 0;

                while (sql.Length > 0)
                {
                    iPos = sql.IndexOf("create table");
                    if (iPos < 0)
                        break;

                    // Get table name
                    sql = sql.Substring(iPos + 12);
                    iPos = sql.IndexOf('(');
                    strTemp = sql.Substring(0, iPos);

                    // Get table definition
                    sql = sql.Substring(iPos);
                    iOpenCount = 1;
                    iPos = 0;
                    while (iOpenCount > 0)
                    {
                        iClosedPos = sql.IndexOf(')', iPos + 1);
                        iOpenPos = sql.IndexOf('(', iPos + 1);

                        if ((iOpenPos > 0) && (iOpenPos < iClosedPos))
                            iOpenCount++;
                        else
                            iOpenCount--;

                        if (iOpenPos < 0) iOpenPos = iClosedPos;
                        if (iClosedPos < 0) iClosedPos = iOpenPos;
                        iPos = Math.Min(iOpenPos, iClosedPos);
                    }

                    // If lookup, check for identity
                    if (strTemp.Contains("lookup"))
                    {
                        strTemp = sql.Substring(0, iClosedPos);
                        if (strTemp.Contains("identity"))
                            return true;
                    }

                    // Move on to next table
                    sql = sql.Substring(iClosedPos + 1);
                }
            }

            return false;
        }

        public static DatabaseBackupReturnValue BackupDatabases()
        {
            DatabaseBackupReturnValue retVal = new DatabaseBackupReturnValue();
            if (strLogFile == null)
                strLogFile = GetLogFileName();
            using (StreamWriter writerLog = new StreamWriter(strLogFile, true))
            {
                writerLog.AutoFlush = true;
                string strAction = "Calling Backup Job";
                Console.WriteLine(strAction);
                writerLog.WriteLine(strAction);

                string strSQL = ConfigurationManager.AppSettings["DBBackupsSQL"];

                string strParams = string.Empty;
                if (string.IsNullOrEmpty(strUser) || string.IsNullOrEmpty(strPassword))
                    strParams = string.Format("-S {0} -d {1} -E -b -Q \"{2}\"", strServer, strDatabase, strSQL);
                else
                    strParams = string.Format("-S {0} -d {1} -U {2} -P {3} -b -Q \"{4}\"", strServer, strDatabase, strUser, strPassword, strSQL);

                Process ps = new Process();
                writerLog.WriteLine("SqlCmd.exe " + strParams);

                ProcessStartInfo psInfo = new ProcessStartInfo("SqlCmd.exe", strParams);
                psInfo.UseShellExecute = false;
                psInfo.CreateNoWindow = true;
                psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                psInfo.RedirectStandardOutput = true;
                psInfo.RedirectStandardError = true;
                ps.StartInfo = psInfo;
                if (ps.Start())
                {
                    string strProcessOutput = string.Empty;
                    while (true)
                    {
                        string strLine;
                        while ((strLine = ps.StandardOutput.ReadLine()) != null)
                        {
                            if (!retVal.Rtflg.HasValue)
                                retVal = new DatabaseBackupReturnValue(strLine);

                            strProcessOutput += string.Format("{0}{1}", Environment.NewLine, strLine);
                            writerLog.WriteLine(strLine);
                        }
                        while ((strLine = ps.StandardError.ReadLine()) != null)
                        {
                            if (!retVal.Rtflg.HasValue)
                                retVal = new DatabaseBackupReturnValue(strLine);

                            retVal.Message += string.Format("{0}{1}", Environment.NewLine, strLine);
                            writerLog.WriteLine(strLine);
                        }
                        if (ps.WaitForExit(0))
                            break;
                    }

                    // Mark as applied if successful
                    int iCode = ps.ExitCode;
                    if (retVal.Rtflg.GetValueOrDefault(-1) == 0)
                    {
                        writerLog.WriteLine("Successfully backed up database");
                    }
                    else
                    {
                        retVal.Message += string.Format("{0}{1}", Environment.NewLine, strProcessOutput);
                        writerLog.WriteLine("\r\nDatabase Backup Failed (Exit Code " + retVal.Rtflg.GetValueOrDefault(-1) + ") Message: " + retVal.Message);
                    }
                }

                writerLog.WriteLine(string.Empty);
                writerLog.WriteLine(seperator);
                writerLog.WriteLine(string.Empty);
                writerLog.Flush();
            }

            return retVal;
        }

        #endregion

        #region Private Methods

        private static List<ScriptVariables> ParseScriptProperties(IEnumerable<XElement> variables)
        {
            IEnumerable<ScriptVariables> result = new List<ScriptVariables>();

            if (variables != null)
            {
                result = from v in variables
                         let name = v.Attribute("ScriptName") != null ? v.Attribute("ScriptName").Value : string.Empty
                         let parameters = v.Elements("variables").Elements("variable")
                         select new ScriptVariables
                         {
                             ScriptName = name,
                             ScriptParameters = ParseScriptVariable(parameters, name)
                         };
            }

            return result.ToList();
        }

        private static Dictionary<string, string> ParseScriptVariable(IEnumerable<XElement> parameters, string scriptName)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (parameters != null && parameters.Any())
            {
                IEnumerable<KeyValuePair<string, string>> temp = (from p in parameters
                                                                  let name = p.Attribute("Name") != null ? p.Attribute("Name").Value : string.Empty
                                                                  let value =
                                                                      !string.IsNullOrWhiteSpace(p.Value) ? p.Value : string.Empty
                                                                  select new KeyValuePair<string, string>(name, value));

                foreach (var keyValuePair in temp)
                {
                    if (result.ContainsKey(keyValuePair.Key))
                        throw new InvalidDataException(string.Format("A script variable has already been added for {0}. See {1} with value {2}. ", scriptName, keyValuePair.Key, keyValuePair.Value));
                    result.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return result;
        }

        private static bool RenameLogFile()
        {
            bool result = false;

            try
            {
                // Rename the log file from the temp file
                // Get log name
                if (bLogDated)
                    strLogFile = string.Format("{0}_{1:yyyyMMdd_HHmmss}.log", strXMLConfigFile.Replace(".xml", ""), DateTime.Now);
                else
                    strLogFile = string.Format("{0}.log", strXMLConfigFile.Replace(".xml", ""));

                if (File.Exists(strLogFile))
                    File.Delete(strLogFile);

                File.Move(_logFileName, strLogFile);
                result = true;
            }
            catch (Exception Ex)
            {
                result = false;
                LogMessage(Ex.ToString());
            }

            return result;
        }

        private static bool ParseConnectionString()
        {
            LogMessage("Parsing Connection String");
            bool result = false;


            if (!string.IsNullOrEmpty(strConnectionString))
            {
                try
                {
                    Dictionary<string, string> dicConnection = new Dictionary<string, string>();
                    string[] arrstrPairs = strConnectionString.Split(';');
                    foreach (string strPair in arrstrPairs)
                    {
                        if (!string.IsNullOrWhiteSpace(strPair))
                        {
                            int assignPosition = strPair.IndexOf('=');
                            if (assignPosition > 0 && (assignPosition + 1) < strPair.Length)
                            {
                                string key = strPair.Substring(0, assignPosition).Trim().ToLower();
                                string value = strPair.Substring(assignPosition + 1).Trim();
                                if (key.Length > 0 && value.Length > 0)
                                {
                                    dicConnection[key] = value;
                                    continue;
                                }
                            }
                            throw new ArgumentException("Could not parse connection string\r\n\r\n(" + strPair + ")\r\n");
                        }
                    }

                    if (dicConnection.Count > 0)
                    {
                        // Machine Name
                        if (dicConnection.ContainsKey("data source"))
                        {
                            strServer = dicConnection["data source"];
                        }
                        else if (dicConnection.ContainsKey("server"))
                        {
                            strServer = dicConnection["server"];
                        }

                        // Database
                        if (dicConnection.ContainsKey("initial catalog"))
                        {
                            strDatabase = dicConnection["initial catalog"];
                        }
                        else if (dicConnection.ContainsKey("database"))
                        {
                            strDatabase = dicConnection["database"];
                        }

                        // UserID
                        if (dicConnection.ContainsKey("user id"))
                        {
                            strUser = dicConnection["user id"];
                        }
                        else if (dicConnection.ContainsKey("uid"))
                        {
                            strUser = dicConnection["uid"];
                        }

                        // Password
                        if (dicConnection.ContainsKey("password"))
                        {
                            strPassword = dicConnection["password"];
                        }
                        else if (dicConnection.ContainsKey("pwd"))
                        {
                            strPassword = dicConnection["pwd"];
                        }

                        string originalDB = strDatabase;

                        strDatabase = AskToChangeDatabaseName();

                        if (!originalDB.Equals(strDatabase))
                        {
                            LogMessage(string.Format("User changing initial database catalogue from {0} to {1}", originalDB, strDatabase));
                            strConnectionString = strConnectionString.Replace(originalDB, strDatabase);
                        }

                        strConnectionString = Regex.Replace(strConnectionString, "Initial catalog", "Database", RegexOptions.IgnoreCase);
                    }

                    result = !string.IsNullOrEmpty(strDatabase) && !string.IsNullOrEmpty(strServer);

                    // Must have server and database
                    if (!result)
                    {                        
                        LogMessage(
                            "Connection string does not contain Data Source or Initial Catalog\r\n\r\n(" +
                            strConnectionString + ")");
                    }
                    else
                    {
                        strCurrentEnv = string.Format("{0}\\{1}", strServer, strDatabase);


                        if (IsServiceRunningProgram)
                            LogMessage(string.Format("Connection string = {0}- Server = {1}", strConnectionString, strServer));
                    }
                }
                catch (Exception ex)
                {
                    LogMessage(ex.ToString());
                    result = false;
                }
            }

            return result;
        }

        private static bool ParseParameters(string[] args)
        {
            bool result = false;
            // Parse parameters

            StringBuilder msg = new StringBuilder();

            foreach (string strArg in args)
                msg.Append(strArg + " ");

            LogMessage(string.Format("{0} {1}", "ApplyNewDBScripts Args = ", msg), true);

            LogMessage("Parsing Parameters..");

            try
            {
                int iParamCount = args.Length;
                int iLastParam = iParamCount - 1;
                strSvnFiles = new List<string>();

                for (int i = 0; i < iParamCount; i++)
                {
                    if (args[i] == "-?" || args[i] == @"/?")
                        LogMessage(Resources.Program.HelpText);
                    else if ((args[i] == "-c") && (i < iLastParam))
                        strConnectionString = args[++i];
                    else if ((args[i] == "-cp") && (i < iLastParam))
                        strConfigPath = args[++i];
                    else if ((args[i] == "-cn") && (i < iLastParam))
                        strConfigConnectionName = args[++i];
                    else if ((args[i] == "-s") && (i < iLastParam))
                        strXMLConfigFile = args[++i].Trim();
                    else if ((args[i] == "-t") && (i < iLastParam))
                        strTail = args[++i];
                    else if (args[i] == "-l")
                        bLogDated = true;
                    else if (args[i] == "-i")
                        bLookupNoIdentity = true;
                    else if (args[i] == "-e")
                        bStopOnError = true;
                    else if (args[i] == "-svn")
                        strSvnFiles.AddRange(args[++i].Split(','));
                    else if (args[i] == "-q")
                        bQuietMode = true;
                }

                result = ValidateParameters();
                if (result == false)
                    Environment.Exit(1);

                //IF connection string is not supplied directly then retrieve from the supplied config file
                if (result && string.IsNullOrEmpty(strConnectionString))
                {
                    XDocument docConfig = XDocument.Load(strConfigPath);

                    LogMessage("Reading Config File for connection string as no connection string was supplied directly");

                    IEnumerable<object> conStrings = (IEnumerable<object>)docConfig.XPathEvaluate(string.Format("//configuration/connectionStrings/add[@name='{0}']/@connectionString",
                                          strConfigConnectionName));

                    if ((conStrings.Count() == 1) && (conStrings.First() is XAttribute))
                    {
                        LogMessage("Setting Connection String from Config File");
                        strConnectionString = ((XAttribute)conStrings.First()).Value;
                    }
                }

                if (string.IsNullOrEmpty(strConnectionString))
                {
                    if (!IsServiceRunningProgram)
                        MessageBox.Show(
                            "Usage:\r\n\r\nApplyNewDBScripts.exe -d <script dir> -c <connection string> [ -l <log file> ]\r\n\r\nOr\r\n\r\nApplyNewDBScripts.exe -d <script dir> -cp <path to app.config> -cn <connection name> [ -l <log file> ]",
                            "Usage", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        EventLog.WriteEntry("ApplyNewDBScripts",
                                            "Invalid Parameters - Must have directory and connection string",
                                            EventLogEntryType.Warning);
                    result = false;
                }
                else
                {
                    LogMessage("Parameters Parsed Successfully.");
                }
            }
            catch (Exception Ex)
            {
                LogMessage(Ex.ToString());

                LogMessage("Parameter Parsing failed");
            }

            return result;
        }

        private static void LogMessage(string strMessage)
        {
            LogMessage(strMessage, false);
        }

        private static void LogMessage(string strMessage, bool AppendNewline)
        {
            if (WriterLog.BaseStream == null)
            {
                if (string.IsNullOrEmpty(strLogFile))
                {
                    strLogFile = GetLogFileName();
                }

                WriterLog = new StreamWriter(strLogFile, true);
                WriterLog.AutoFlush = true;
                WriterLog.Flush();
            }

            if (AppendNewline)
                WriterLog.WriteLine(Environment.NewLine);

            WriterLog.WriteLine(strMessage);

            // But why?!? 
            if ((!IsServiceRunningProgram) || (bQuietMode))
            {
                if (AppendNewline)
                    Console.WriteLine(Environment.NewLine);

                Console.WriteLine(strMessage);
            }
        }

        private static bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(strXMLConfigFile))
            {
                LogMessage("ERROR: Path to the required XML settings file is not specified (use -s to specify).");
                return false;
            }
            else if (!string.IsNullOrEmpty(strXMLConfigFile) && !File.Exists(strXMLConfigFile))
            {
                LogMessage(String.Format("ERROR: Specified XML settings file '{0}' does not exist.", strXMLConfigFile));
                return false;
            }
            else if ((string.IsNullOrEmpty(strConnectionString)) && (string.IsNullOrEmpty(strConfigPath)))
            {
                LogMessage("ERROR: Either connection string (-c) or config file path (-cp) must be supplied.");
                return false;
            }
            else if (!string.IsNullOrEmpty(strConfigPath) && (!File.Exists(strConfigPath) || string.IsNullOrEmpty(strConfigConnectionName)))
            {
                LogMessage("ERROR: Config file specified does not exist or connection name is not specified (-cn)");
                return false;
            }

            return true;
        }

        private static string AskToChangeDatabaseName()
        {
            // Dont give this option if service is running program
            if (IsServiceRunningProgram)
                return strDatabase;

            bool blnNeverAskToChangeDatabaseName = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                    blnNeverAskToChangeDatabaseName = bool.Parse(key.GetValue(strServer + "\\" + strDatabase + "\\NeverAskToChangeDatabaseName", false).ToString());
            }

            if (blnNeverAskToChangeDatabaseName)
                return strDatabase;

            GetVariableValue frmDBName = new GetVariableValue();
            frmDBName.CheckBoxVisible = true;
            frmDBName.Show("Would you like to change the initial catalog(database)", "New Database Name", strDatabase);

            // Store always apply setting
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
            {
                if (key != null)
                    key.SetValue(strServer + "\\" + strDatabase + "\\NeverAskToChangeDatabaseName", frmDBName.CheckBoxValue);
            }

            return frmDBName.VariableValue;
        }

        private static void ApplyScriptsConfirmed()
        {
            string strParams;
            int iSuccessCount = 0;
            List<DBScriptToApply> listToApply = new List<DBScriptToApply>();

            // Find scripts to apply
            LogMessage("Finding Scripts Not Yet Applied");
            listToApply = GetListOfScriptsToApply();

            int numFiles = listToApply.Count;
            int currentScriptNumber = 0;
            LogMessage(string.Format("{0} file{1} to be applied", numFiles, numFiles == 1 ? string.Empty : "s"));
            LogMessage(seperator);

            foreach (var scriptToApply in listToApply)
            {
                currentScriptNumber++;

                if (ApplyScriptPopup(scriptToApply.ScriptName, currentScriptNumber, numFiles))
                {
                    LogMessage(string.Format("Applying script {0} - {1}", currentScriptNumber, scriptToApply.FullyQualifiedPath));

                    if (IsServiceRunningProgram)
                        LogMessage(string.Format("Connection string = {0}- Server = {1}", strConnectionString, strServer));

                    if (string.IsNullOrEmpty(strUser) || string.IsNullOrEmpty(strPassword))
                        strParams = string.Format("-S {0} -I -d {1} -E -b -i \"{2}\"", strServer, scriptToApply.Database, scriptToApply.FullyQualifiedPath);
                    else
                        strParams = string.Format("-S {0} -I -d {1} -U {2} -P {3} -b -i \"{4}\"", strServer, scriptToApply.Database, strUser, strPassword, scriptToApply.FullyQualifiedPath);

                    foreach (var scriptParameter in scriptToApply.ScriptParameters)
                    {
                        strParams = string.Format("{0} -v {1}=\"{2}\" ", strParams, scriptParameter.Key, scriptParameter.Value);
                    }

                reApply:
                    Process ps = new Process();
                    ProcessStartInfo psInfo = new ProcessStartInfo("SqlCmd.exe", strParams);
                    LogMessage("Calling SqlCmd.exe with the following parameters : " + strParams);

                    try
                    {
                        #region ApplyScript

                        psInfo.UseShellExecute = false;
                        psInfo.CreateNoWindow = true;
                        psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        psInfo.RedirectStandardOutput = true;
                        psInfo.RedirectStandardError = true;
                        ps.StartInfo = psInfo;
                        if (ps.Start())
                        {
                            while (true)
                            {
                                string strLine;
                                while ((strLine = ps.StandardOutput.ReadLine()) != null)
                                {
                                    if (!string.IsNullOrEmpty(strLine))
                                        LogMessage(strLine);
                                }
                                while ((strLine = ps.StandardError.ReadLine()) != null)
                                {
                                    if ((strLine == "Sqlcmd: Error: Microsoft SQL Server Native Client 10.0 : Unspecified error.")
                                        || (strLine == "Sqlcmd: Error: Internal error at ExecuteSqlCmd (Reason: Unspecified error)."))
                                    {
                                        Console.Error.WriteLine("This is a known error. Re-applying the script.");
                                        goto reApply;
                                    }
                                    if (!string.IsNullOrEmpty(strLine))
                                        LogMessage(strLine);
                                }
                                if (ps.WaitForExit(0))
                                    break;
                            }

                            // This is required if we are creating a database from scratch. If we are changing from Master db to normal db, 
                            // we need to clear the old connections in the connection pool
                            if (!scriptToApply.Database.Equals(strDatabase))
                                SqlConnection.ClearAllPools();

                            // Mark as applied if successful
                            int iCode = ps.ExitCode;
                            if (iCode == 0)
                            {
                                LogMessage("Successfully applied script to " + scriptToApply.Database);
                                SaveAppliedScript(strConnectionString, scriptToApply);
                                LogMessage("Marked script as applied in database");
                                iSuccessCount++;
                            }
                            else
                            {
                                LogMessage("\r\nSCRIPT FAILED (Exit Code " + iCode + ")");
                                LogMessage(seperator);
                                if (bStopOnError)
                                    break;
                            }
                        }

                        LogMessage(seperator);

                        #endregion
                    }
                    catch (IOException Ex)
                    {
                        LogMessage(seperator, true);
                        LogMessage(Ex.Message, true);
                        LogMessage(Ex.StackTrace, true);
                        LogMessage(seperator, true);

                        MessageBox.Show("IO Exception occurred in ApplyScriptsConfirmed. Application is terminating. See Log files for details");

                        Environment.Exit(constExceptionOccured);
                    }
                    catch (Exception ex)
                    {
                        LogMessage(String.Format("SqlCmd.exe {0}", strParams));
                        if (IsServiceRunningProgram)
                            EventLog.WriteEntry("ApplyNewDBScripts",
                                                string.Format("Error running SqlCmd.exe {0}", strParams),
                                                EventLogEntryType.Error);

                        throw new Exception(string.Format("Error running SqlCmd.exe {0}", strParams), ex);
                    }
                }
            }


            if (iSuccessCount != numFiles)
            {
                LogMessage("Not all database scripts were successfully applied.", true);

                // If scripts have failed, remove the AskAgainAfter key from registry
                using (RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
                {
                    if (reg.GetValueNames().Contains(strCurrentEnv + "AskAgainAfter"))
                        reg.DeleteValue(strCurrentEnv + "AskAgainAfter");
                }

                // Don't ask user if running as a service
                if (!IsServiceRunningProgram)
                {
                    if (MessageBox.Show("Not all database scripts were successfully applied.\r\nPress OK to view log",
                                        "Database Scripts Log File", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button1) == DialogResult.OK)
                    {
                        if (string.IsNullOrEmpty(strLogFile))
                        {
                            strLogFile = GetLogFileName();
                        }

                        Process.Start(strLogFile);
                    }
                }
                else
                    EventLog.WriteEntry("ApplyNewDBScripts", "Not all database scripts were successfully applied.",
                                        EventLogEntryType.Error);

                Environment.Exit(constNotAllScriptsSuccessfullyApplied);
            }
            else
            {
                LogMessage("All database scripts were successfully applied.", true);
            }

            LogMessage(seperator);
            LogMessage("Completed at " + DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), true);

        }

        private static void Finished()
        {
            DateTime dtFinish = DateTime.Now;
            TimeSpan ts = new TimeSpan(dtFinish.Ticks - dtStart.Ticks);
            string message = string.Format(@"Apply Database Scripts Completed. Duration {0} Hours {1} minutess {2} seconds Started @ {3} 	Finished @ {4}",
                /*0*/ts.Hours,
                /*1*/ts.Minutes,
                /*2*/ts.Seconds,
                /*3*/dtStart,
                /*4*/dtFinish);
            LogMessage(message, true);

            WriterLog.Flush();
        }

        private static List<AppliedScript> GetAppliedScripts()
        {
            List<AppliedScript> listRetVal = new List<AppliedScript>();
            string strMasterConnectionString = string.Empty;

            if(!string.IsNullOrEmpty(strConnectionString) && !string.IsNullOrEmpty(strDatabase))
            {
                string tempConnString = strConnectionString;
                strMasterConnectionString = tempConnString.Replace(strDatabase, "Master");
            }

            try
            {
                // then check to see if this database exists by quering the master database
                using(SqlConnection sqlConn = new SqlConnection(strMasterConnectionString))
                {
                    sqlConn.Open();

                    // Check Database Table exists
                    SqlCommand sqlCmd = new SqlCommand(string.Format(Resources.Program.CheckDatabaseExistsScript, strDatabase), sqlConn);
                    using(SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                    {
                        if(!sqlReader.HasRows)
                            return listRetVal;
                    }
                }

                using(SqlConnection sqlConn = new SqlConnection(strConnectionString))
                {
                    sqlConn.Open();

                    // Check Database Table exists
                    CreateAppliedScriptsTable(strConnectionString);

                    // Read table
                    string command = "SELECT AppliedScriptId, PreviousAuthor, HashValue, PathValue, IsRunOnce," 
                                   + "IsForceApplied, Revision, DateTimeScriptApplied, Comment "
                                   + "FROM dbo.AppliedScript";
                    SqlCommand sqlCmd = new SqlCommand(command, sqlConn);

                    using(SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                    {
                        while(sqlReader.Read())
                        {
                            listRetVal.Add(new AppliedScript() {
                                PreviousAuthor = sqlReader["PreviousAuthor"].ToString(),
                                ScriptPath = sqlReader["PathValue"].ToString(),
                                IsRunOnce = Convert.ToBoolean(sqlReader["IsRunOnce"]),
                                IsForceApplied = Convert.ToBoolean(sqlReader["IsForceApplied"]),
                                ID = Convert.ToInt32(sqlReader["AppliedScriptId"]),
                                Hash = sqlReader["HashValue"].ToString(),
                                DateScriptApplied = Convert.ToDateTime(sqlReader["DateTimeScriptApplied"]),
                                Comment = sqlReader["Comment"].ToString(),
                                Revision = sqlReader["Revision"].ToString()
                            });
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                LogMessage(ex.ToString());
                throw;
            }

            return listRetVal.OrderByDescending(x => x.DateScriptApplied).ToList();
        }

        private static void CreateAppliedScriptsTable(string connectionString)
        {
            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();

                string command =

@"IF NOT EXISTS (SELECT * FROM sys.tables WHERE object_id=object_id('dbo.AppliedScript')) 
BEGIN 
	CREATE TABLE dbo.AppliedScript
	(
		AppliedScriptId int IDENTITY(1,1) NOT NULL,
		HashValue nvarchar(1000) NOT NULL,
		PathValue nvarchar(1000) NULL,
		PreviousAuthor nvarchar(100) NULL,
		Revision nvarchar(50) NULL,
		IsForceApplied bit NOT NULL DEFAULT 0,
		IsRunOnce bit NOT NULL DEFAULT 1,
		DateTimeScriptApplied datetime NOT NULL DEFAULT GETDATE(),
		Comment nvarchar(500) NULL
		CONSTRAINT PK_AppliedScripts PRIMARY KEY CLUSTERED (AppliedScriptId ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	)
END";

                // Check Database Table exists if not create it
                SqlCommand sqlCmd = new SqlCommand(command, sqlConn);
                sqlCmd.ExecuteNonQuery();

                ConvertAppliedScriptsToAppliedScript(sqlConn);
                ConvertRevisionToNvarchar(sqlConn);
            }
        }

        private static void ConvertRevisionToNvarchar(SqlConnection sqlConn)
        {
            string command =
                @"-- DSM-197 Change the type of the Revision column from INT to NVARCHAR (50) (dbo.AppliedScript)
IF NOT EXISTS (SELECT DATA_TYPE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE 
                     TABLE_NAME = 'AppliedScript'
                     AND TABLE_SCHEMA = 'dbo'
                     AND COLUMN_NAME = 'Revision'
                     AND DATA_TYPE = 'nvarchar')
BEGIN
    ALTER TABLE dbo.AppliedScript
    ALTER COLUMN Revision NVARCHAR(50) NULL
END";
            new SqlCommand(command, sqlConn).ExecuteNonQuery();
        }

        private static void ConvertAppliedScriptsToAppliedScript(SqlConnection sqlConn)
        {
            string command;
            SqlCommand sqlCmd;
// Check if CrewPay's dbo.AppliedScripts table exists and convert data to dbo.AppliedScript format
            command = @"SELECT * FROM sys.tables WHERE object_id=object_id('dbo.AppliedScripts')";
            if (new SqlCommand(command, sqlConn).ExecuteScalar() == null)
                return;

            command = @"SELECT * FROM dbo.AppliedScripts";
            sqlCmd = new SqlCommand(command, sqlConn);
            var sqlFiles =
                Directory.EnumerateFiles(Path.GetFullPath(Path.GetDirectoryName(strXMLConfigFile)), "*.sql",
                    SearchOption.AllDirectories).ToList();

            var parsedHashList = new List<AppliedScript>();
            using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
            {
                while (sqlReader.Read())
                {
                    var parsedHash = ParseHash(sqlReader["hash"].ToString(), sqlFiles);

                    if (parsedHash != null)
                    {
                        parsedHashList.Add(parsedHash);
                        parsedHash.DateScriptApplied = Convert.ToDateTime(sqlReader["DateCreated"]);
                    }
                }
            }

            foreach (var parsedHash in parsedHashList)
            {
                string insertCommand = String.Format(
                    @"INSERT INTO AppliedScript (HashValue, PathValue, Revision, DateTimeScriptApplied)
VALUES ('{0}', '{1}', {2}, '{3}')", parsedHash.Hash, parsedHash.ScriptPath, parsedHash.Revision,
                    parsedHash.DateScriptApplied.ToString("yyyy-MM-dd HH:mm:ss"));

                new SqlCommand(insertCommand, sqlConn).ExecuteNonQuery();
            }

            command = @"EXEC sp_rename 'AppliedScripts', 'AppliedScripts_Archived'";
            new SqlCommand(command, sqlConn).ExecuteNonQuery();
        }

        private static AppliedScript ParseHash(string hashString, IEnumerable<string> directoryListing)
        {
            var revRegex = Regex.Match(hashString, @"\[(\d*)]");

            string revNum = "0";
            if (revRegex.Groups.Count > 1)
                revNum = Regex.Match(hashString, @"\[(\d*)]").Groups[1].Value;

            string scriptName = Regex.Match(hashString, @"(.*)_\[").Groups[1].Value;
            string hash = Regex.Match(hashString, @":(.*)").Groups[1].Value;

            var fullPath = directoryListing.FirstOrDefault(s => (s.IndexOf(scriptName, StringComparison.OrdinalIgnoreCase) >= 0));

            if (fullPath == null || fullPath.Contains(@"\Archive\"))
                return null;
            else
                fullPath = fullPath.Replace(Path.GetFullPath(Path.GetDirectoryName(strXMLConfigFile)), @"\airline\crewpay\airline.crewpay.database.scripts.asa");
            
            return new AppliedScript { Revision = revNum, ScriptPath = fullPath, Hash = hash };
        }

        private static void SaveAppliedScript(string connectionString, DBScriptToApply script)
        {
            CreateAppliedScriptsTable(connectionString);

            using (SqlConnection sqlConn = new SqlConnection(connectionString))
            {
                sqlConn.Open();

                string revision = "-1";

                if (script.SVNInformation != null)
                    revision = script.SVNInformation.LastChangedRev.ToString();

                if (bGitUsed)
                    revision = null;

                SqlCommand sqlCmd = new SqlCommand(
                    String.Format(@"INSERT INTO dbo.AppliedScript(HashValue, PathValue, PreviousAuthor, Revision, IsForceApplied, IsRunOnce, DateTimeScriptApplied, Comment) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')",
                        script.Hash.Replace("'", "''"),
                        script.RelativePath,
                        script.SVNInformation == null ? "Unversioned" : script.SVNInformation.LastChangedAuthor,
                        revision,
                        script.AssociatedSettingLocation.isForceReApply ? "1" : "0",
                        script.AssociatedSettingLocation.isSingleRun ? "1" : "0",
                        DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss"),
                        string.Empty),
                    sqlConn);

                sqlCmd.ExecuteNonQuery();
            }
        }

        private static bool ApplyScriptPopup(string strScript, int scriptNumber, int totalScriptCount)
        {
            bool showFileNamePopup = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                    showFileNamePopup = bool.Parse(key.GetValue(strServer + "\\" + strDatabase + "ShowFileNames", false).ToString());
            }

            MemoryBox memoryBox = new MemoryBox();

            if (IsServiceRunningProgram || !showFileNamePopup)
                _mbrApplyScripts = MemoryBoxResult.YesToAll;

            if (!_mbrApplyScripts.HasValue || (_mbrApplyScripts.Value != MemoryBoxResult.YesToAll &&
                _mbrApplyScripts.Value != MemoryBoxResult.NoToAll && _mbrApplyScripts.Value != MemoryBoxResult.Cancel))
            {
                _mbrApplyScripts =
                    memoryBox.ShowMemoryDialog(string.Format(@"Do you want to apply {3}{0}?{3}{3}Script {1} of {2}.",
                    /*0*/strScript,
                    /*1*/scriptNumber,
                    /*2*/totalScriptCount,
                    /*3*/Environment.NewLine)
                        , "Apply Script Confirmation");
            }

            switch (_mbrApplyScripts)
            {
                case MemoryBoxResult.Cancel:
                    LogMessage("User Cancelled Script Process - " + strScript);
                    LogMessage(seperator);
                    return false;
                case MemoryBoxResult.No:
                case MemoryBoxResult.NoToAll:
                    LogMessage("User Skipping " + strScript);
                    LogMessage(seperator);
                    return false;
                default:
                    return true;
            }
        }

        private static bool ApplyUnversionedScriptPopup(string strScript)
        {
            bool showFileNamePopup = false;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                    showFileNamePopup = bool.Parse(key.GetValue(strServer + "\\" + strDatabase + "ApplyUnversionedScripts", false).ToString());
            }

            MemoryBox memoryBox = new MemoryBox();

            if (IsServiceRunningProgram || !showFileNamePopup)
                _mbrApplyUnversionedScripts = MemoryBoxResult.NoToAll;

            if (!_mbrApplyUnversionedScripts.HasValue || (_mbrApplyUnversionedScripts.Value != MemoryBoxResult.YesToAll &&
                _mbrApplyUnversionedScripts.Value != MemoryBoxResult.NoToAll && _mbrApplyUnversionedScripts.Value != MemoryBoxResult.Cancel))
            {
                _mbrApplyUnversionedScripts =
                    memoryBox.ShowMemoryDialog(string.Format(@"Do you want to include unversioned script {0}?", strScript), "Include Unversioned Script Confirmation");
            }

            switch (_mbrApplyUnversionedScripts)
            {
                case MemoryBoxResult.Cancel:
                case MemoryBoxResult.No:
                case MemoryBoxResult.NoToAll:
                    LogMessage(".Skipping.");
                    return false;
                default:
                    return true;
            }
        }

        private static List<SVNInfo> GetGitDetailsOfSqlFiles(SettingsLocation location)
        {
            string path = location.AbsolutePath;

            if (strSvnFiles != null)
            {
                var svnInfoList = new List<SVNInfo>();
                for (int i = 0; i < strSvnFiles.Count(); i++)
                {
                    if (File.Exists(strSvnFiles[i]))
                    {
                        foreach (var gitInfo in File.ReadAllLines(strSvnFiles[i]))
                        {
                            var regex = new Regex(pattern: @"""(.*)"",(\w*),(.*),(.*)");
                            var matches = regex.Match(gitInfo).Groups;

                            svnInfoList.Add(new SVNInfo
                            {
                                Path = matches[1].Value,
                                GitHash = matches[2].Value,
                                LastChangedAuthor = matches[3].Value,
                                LastChangedDate = matches[4].Value,
                            });
                        }
                        
                    }
                    else
                    {
                        LogMessage(String.Format("{0} not found. Please review the settings file and program arguments.", strSvnFiles[i]));
                    }
                }
                return svnInfoList;
            }

            if (location.IsFile)
            {
                return new List<SVNInfo>
                {
                    GetGitInfo(path)
                };
            }
            else
            {
                return Directory.EnumerateFiles(path, "*",
                    location.isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                    .Select(GetGitInfo)
                    .ToList();
            }
        }

        private static SVNInfo GetGitInfo(string path)
        {
            string args = "log -1 --format=%h,%an,%ai \"" + path + '"';
            var sb = new StringBuilder();

            using (Process ps = new Process())
            {
                ProcessStartInfo psInfo = new ProcessStartInfo("git.exe", args);

                psInfo.UseShellExecute = false;
                psInfo.CreateNoWindow = true;
                psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                psInfo.RedirectStandardOutput = true;
                psInfo.RedirectStandardError = true;
                psInfo.WorkingDirectory = Path.GetDirectoryName(path);
                ps.StartInfo = psInfo;

                try
                {
                    if (ps.Start())
                    {
                        while (true)
                        {
                            string strLine;
                            while ((strLine = ps.StandardOutput.ReadLine()) != null)
                                sb.AppendLine(strLine);
                            while ((strLine = ps.StandardError.ReadLine()) != null)
                            {
                                WriterLog.WriteLine("GIT Error - {0}", strLine);
                                sb.AppendLine(strLine);
                            }
                            if (ps.WaitForExit(0))
                                break;
                        }

                        if (ps.ExitCode != 0)
                        {
                            LogMessage(
                                "Error occurred during the process of getting the GIT file information. Contact rainmaker about this error.");
                            return null;
                        }
                    }
                }
                catch (Exception)
                {
                    LogMessage("git.exe does not exist. Please install git if you're running from git repository.");
                }
            }

            string answer = sb.ToString();
            if (String.IsNullOrEmpty(answer))
            {
                LogMessage("No GIT information could be obtained for " + path);
                return null;
            }

            var answerArray = answer.Split(',');
            return
                new SVNInfo
                {
                    Kind = SVNInfoEntryKind.File,
                    Path = path,
                    GitHash = answerArray[0],
                    LastChangedAuthor = answerArray[1],
                    LastChangedDate = answerArray[2],
                    LastChangedRev = 1
                };
        }

        private static List<SVNInfo> GetSVNDetailsOfSQLFiles(SettingsLocation location, SearchOption options)
        {
            string strXMLLocation;
            string strArgs = string.Empty;
            XDocument docDefault = new XDocument();

            if (strSvnFiles == null || (strSvnFiles != null && strSvnFiles.Count < 1))
            {
                strXMLLocation = Path.GetTempFileName();
                string strSVN = "svn.exe";

                string strPath = location.AbsolutePath;

                if (strPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    strPath = strPath.Substring(0,
                                                strPath.Length -
                                                (strPath.Length - strPath.LastIndexOf(Path.DirectorySeparatorChar)));
                }

                if (!location.IsFile)
                {
                    if (options == SearchOption.AllDirectories)
                        strArgs = string.Format("info --depth=infinity \"{0}\" --xml", strPath);
                    else
                        strArgs = string.Format("info --depth=files \"{0}\" --xml", strPath);
                }
                else
                {
                    strPath = strPath.Substring(0, strPath.LastIndexOf(Path.DirectorySeparatorChar));
                    strArgs = string.Format("info --depth=files \"{0}\" --xml", strPath);
                }


                LogMessage(string.Format("{0}, {1}", strSVN, strArgs));

                using (StreamWriter xmlWriter = new StreamWriter(strXMLLocation, false))
                {
                    using (Process ps = new Process())
                    {
                        ProcessStartInfo psInfo = new ProcessStartInfo(strSVN, strArgs);

                        psInfo.UseShellExecute = false;
                        psInfo.CreateNoWindow = true;
                        psInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        psInfo.RedirectStandardOutput = true;
                        psInfo.RedirectStandardError = true;
                        psInfo.WorkingDirectory = Path.GetPathRoot(strPath);
                        ps.StartInfo = psInfo;

                        if (ps.Start())
                        {
                            while (true)
                            {
                                string strLine;
                                while ((strLine = ps.StandardOutput.ReadLine()) != null)
                                    xmlWriter.WriteLine(strLine);
                                while ((strLine = ps.StandardError.ReadLine()) != null)
                                {
                                    WriterLog.WriteLine(string.Format("SVN Error - {0}", strLine));
                                    xmlWriter.WriteLine(strLine);
                                }
                                if (ps.WaitForExit(0))
                                    break;
                            }

                            if (ps.ExitCode != 0)
                            {
                                LogMessage(
                                    "Error occurred during the process of getting the SVN file information. Contact rainmaker about this error.");
                            }
                        }
                    }
                }

                docDefault = XDocument.Load(strXMLLocation);


                // Delete temporary file
                if (File.Exists(strXMLLocation))
                    File.Delete(strXMLLocation);
            }
            else if (SVNInfoForScriptsToApply == null || SVNInfoForScriptsToApply.Count == 0)
            {
                for (int i = 0; i < strSvnFiles.Count(); i++)
                {
                    if (File.Exists(strSvnFiles[i]))
                    {
                        if (i == 0)
                        {
                            docDefault = XDocument.Load(strSvnFiles[i]);
                        }
                        else
                        {
                            XDocument tmp = XDocument.Load(strSvnFiles[i]);

                            var elements = from entry in tmp.Descendants("entry") select entry;

                            if (docDefault.Root != null) docDefault.Root.Add(elements);
                        }
                    }
                    else
                    {
                        LogMessage(String.Format("{0} not found. Please review the settings file and program arguments.", strSvnFiles[i]));
                    }
                }
            }

            // Find all "entry" objects
            int tempRevisionOutHolder;
            IEnumerable<SVNInfo> entries = from entry in docDefault.Descendants("entry")
                                           where entry.Attribute("kind").Value.Equals("file")
                                           select new SVNInfo
                                                      {
                                                          Kind = SVNInfoEntryKind.File,
                                                          Path = entry.Attribute("path").Value,
                                                          Revision =
                                                              int.TryParse(entry.Attribute("revision").Value,
                                                                           out tempRevisionOutHolder)
                                                                  ? tempRevisionOutHolder
                                                                  : 0,
                                                          LastChangedRev =
                                                              entry.Element("commit") != null
                                                                  ? Convert.ToInt32(
                                                                      entry.Element("commit").Attribute("revision").
                                                                          Value)
                                                                  : 0,
                                                          LastChangedAuthor =
                                                              entry.Element("commit") != null
                                                                  ? entry.Element("commit").Element("author").Value
                                                                  : string.Empty,
                                                          LastChangedDate =
                                                              entry.Element("commit") != null
                                                                  ? entry.Element("commit").Element("date").Value
                                                                  : string.Empty
                                                      };



            // Only return sql files))
            return entries.Where(x => x.Path.ToLower().Trim().EndsWith(".sql")).ToList();
        }

        private static List<DBScriptToApply> GetListOfScriptsToApply()
        {
            
            List<DBScriptToApply> ScriptsToApply = new List<DBScriptToApply>();
            frmGetVariableValueBulk frmGetVariableValueBulk = new frmGetVariableValueBulk();
            int IsCanceled = 0;
            try
            {
                LogMessage("Finding Applied Scripts");
                List<AppliedScript> listApplied = GetAppliedScripts();

                // Find scripts already applied to database
                LogMessage("Finding Scripts Already Applied");

                int iScriptNum = 0;
                int scriptDirectoryCount = 0;
                int iScriptsToApplyNum = 1;

                bool enableUnversionedScripts = (!IsInstallerRunningProgram && !IsServiceRunningProgram);

                HideScriptVariablePopup = false;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
                {
                    if (key != null)
                        HideScriptVariablePopup = bool.Parse(key.GetValue(strServer + "\\" + strDatabase + "HideScriptVariablePopup", false).ToString());
                }

                //Load paths from user configured Settings file
                var xd = XDocument.Load(strXMLConfigFile);
                var baseSettingLocationPath = strXMLConfigFile.Substring(0, strXMLConfigFile.LastIndexOf(Path.DirectorySeparatorChar));
                var pathsToApply = from sl in xd.Elements("entry").Elements("location")
                                   let recursive = sl.Attribute("Recursive") == null || Convert.ToBoolean(sl.Attribute("Recursive").Value)
                                   let runOnce = sl.Attribute("RunOnce") != null && Convert.ToBoolean(sl.Attribute("RunOnce").Value)
                                   let forceReApply = sl.Attribute("ForceReApply") != null && Convert.ToBoolean(sl.Attribute("ForceReApply").Value)
                                   let isFile = sl.Attribute("IsFile") != null && Convert.ToBoolean(sl.Attribute("IsFile").Value)
                                   let path = sl.Attribute("Path").Value
                                   let scriptVar = sl.Elements("scriptProperties")
                                   select new SettingsLocation()
                                   {
                                       IsFile = isFile,
                                       isForceReApply = forceReApply,
                                       isRecursive = recursive,
                                       isSingleRun = runOnce,
                                       RelativePath = path,
                                       BasePath = baseSettingLocationPath,
                                       ScriptVariablesList = ParseScriptProperties(scriptVar)
                                   };

                LogMessage("Reading Script Directories from user configured paths");
                foreach (SettingsLocation settingsLocation in pathsToApply)
                {
                    SearchOption options;

                    if (settingsLocation.isRecursive)
                    {
                        options = SearchOption.AllDirectories;
                    }
                    else
                    {
                        options = SearchOption.TopDirectoryOnly;
                    }

                    List<string> scriptDirectoryContents = new List<string>();

                    if (!settingsLocation.IsFile)
                    {
                        scriptDirectoryContents =
                            Directory.GetFiles(settingsLocation.AbsolutePath, "*.sql", options).Select(
                                x => GetAbsolutePath(x))
                                .Distinct()
                                .Where(y => y.EndsWith(".sql", true, new CultureInfo("EN")))
                                .ToList();

                        scriptDirectoryCount += scriptDirectoryContents.Count();
                    }
                    else
                    {
                        scriptDirectoryContents.Add(Path.GetFullPath(settingsLocation.AbsolutePath));
                        scriptDirectoryCount++;
                    }

                    if (scriptDirectoryCount > 0)
                    {
                        if (bGitUsed)
                        {
                            SVNInfoForScriptsToApply = new List<SVNInfo>();
                        }
                        else
                        {
                            if (SVNInfoForScriptsToApply == null)
                                SVNInfoForScriptsToApply = GetSVNDetailsOfSQLFiles(settingsLocation, options);
                            else
                                SVNInfoForScriptsToApply.AddRange(GetSVNDetailsOfSQLFiles(settingsLocation, options));
                        }

                        foreach (string script in scriptDirectoryContents)
                        {
                            iScriptNum++;

                            string scriptLower = script.ToLower();

                            LogMessage(String.Format("{0} : Getting SVN Info for {1}..",
                                /*0*/iScriptNum.ToString("000"),
                                /*1*/script));


                            string strParentFolderName = script.Substring(0,
                                                                          script.LastIndexOf(Path.DirectorySeparatorChar));
                            strParentFolderName =
                                strParentFolderName.Substring(
                                    strParentFolderName.LastIndexOf(Path.DirectorySeparatorChar));
                            strParentFolderName = strParentFolderName.Replace(Path.DirectorySeparatorChar.ToString(),
                                                                              string.Empty);

                            string scriptName = Path.GetFileName(scriptLower);

                            DBScriptToApply scriptToApply = new DBScriptToApply();
                            scriptToApply.AssociatedSettingLocation = settingsLocation;
                            scriptToApply.ParentFolder = strParentFolderName;
                            scriptToApply.ScriptName = scriptName;
                            scriptToApply.FullyQualifiedPath = script;
                            scriptToApply.AppliedScriptInfo = new AppliedScript();

                            scriptToApply.SVNInformation =
                                SVNInfoForScriptsToApply.FirstOrDefault(
                                    x => x.Path.ToLower().Contains(scriptToApply.RelativePath));

                            if (scriptToApply.SVNInformation == null)
                                scriptToApply.SVNInformation = new SVNInfo();

                            scriptToApply.Hash = (scriptToApply.SVNInformation.LastChangedRev > 0 || bGitUsed)
                                ? GetFileMD5(script)
                                : GetFileMD5(script) + DateTime.Now.Ticks;

                            if (scriptToApply.SVNInformation.LastChangedRev > 0 || bGitUsed ||
                                (enableUnversionedScripts &&
                                 ApplyUnversionedScriptPopup(strParentFolderName + "//" + scriptName)))
                            {
                                string scriptSpecificDataBase;
                                int AddVarToDlgFlag;

                                scriptToApply.IsAppliedPreviously =
                                    listApplied.Any(x => x.ScriptPath.ToLower().EndsWith(scriptToApply.RelativePath));

                                if (scriptToApply.IsAppliedPreviously)
                                {
                                    scriptToApply.AppliedScriptInfo =
                                        listApplied.FirstOrDefault(
                                            x => x.ScriptPath.ToLower().EndsWith(scriptToApply.RelativePath));
                                }

                                if (
                                    /* Scripts from that location can be applied multiple times and 
                                       this particular script has ForceReApply flag or wasn't applied previously */
                                    ((settingsLocation.isForceReApply || !scriptToApply.IsAppliedPreviously) &&
                                     !scriptToApply.AssociatedSettingLocation.isSingleRun)
                                    ||
                                    // The script should be run once and it wasn't applied previously
                                    (scriptToApply.AssociatedSettingLocation.isSingleRun &&
                                     !scriptToApply.IsAppliedPreviously)
                                    ||
                                    // The file is different from what's already applied
                                    (scriptToApply.IsAppliedPreviously && !scriptToApply.AssociatedSettingLocation.isSingleRun
                                    && scriptToApply.AppliedScriptInfo != null
                                    && scriptToApply.AppliedScriptInfo.Hash != scriptToApply.Hash)
                                    )
                                {
                                    AddVarToDlgFlag = 0;
                                    scriptToApply.ScriptParameters =
                                        ParseScriptForParametersOrDatabase(settingsLocation, scriptToApply.FullyQualifiedPath,
                                                                           scriptToApply.ScriptName,
                                                                           out scriptSpecificDataBase,
                                                                           out AddVarToDlgFlag);
                                    scriptToApply.Database = scriptSpecificDataBase;

                                    if (AddVarToDlgFlag == 1)
                                    {
                                        frmGetVariableValueBulk.AddVariable(iScriptsToApplyNum, scriptToApply.ScriptName, scriptToApply.ScriptParameters);
                                    }
                                    //was 
                                    ScriptsToApply.Add(scriptToApply);
                                    iScriptsToApplyNum++;
                                }
                            }
                            else
                            {
                                LogMessage(String.Format("{0} of {1} : Get SVN Info Step skipped for {2}..",
                                    /*0*/iScriptNum.ToString("000"),
                                    /*1*/iScriptNum.ToString("000"),
                                    /*2*/script));
                            }
                        }
                    }
                }

                // Replace default Values with Users Values
                // If was Assigned any Control then was populated variables
                // then show Dialogue
                DialogResult GetVariableDialogResult;
                if (frmGetVariableValueBulk.GetAssignedControlCount(1) > 0)
                {
                    // Show Dialogue with Variables
                    LogMessage(String.Format("==============     START Replace with user Values       ***********"
                                ));
                    //set focus to first control
                    frmGetVariableValueBulk.SetFocusToFirstTextBox();
                    GetVariableDialogResult = frmGetVariableValueBulk.ShowDialog();
                    if (GetVariableDialogResult == DialogResult.OK)
                    {
                        Dictionary<int, Dictionary<string, string>> dictVariableValues = new Dictionary<int, Dictionary<string, string>>();
                        int iTag;
                        string sValue;
                        string sKey;
                        dictVariableValues = frmGetVariableValueBulk.GetVariableValues();

                        // loop for each variable and locate changes
                        foreach (KeyValuePair<int, Dictionary<string, string>> kvp in dictVariableValues)
                        {
                            iTag = kvp.Key - 1;
                            LogMessage(String.Format("Script {0} Script Name: {1}",
                                /*0*/iTag.ToString("000"),
                                /*1*/ScriptsToApply[iTag].ScriptName
                                ));
                            foreach (KeyValuePair<string, string> kvp2 in kvp.Value)
                            {
                                if (ScriptsToApply[iTag].ScriptParameters.ContainsKey(kvp2.Key))
                                {
                                    if (ScriptsToApply[iTag].ScriptParameters[kvp2.Key] != kvp2.Value)
                                    {
                                        sKey = kvp2.Key.ToString();

                                        sValue = Convert.ToString(ScriptsToApply[iTag].ScriptParameters[kvp2.Key]);
                                        LogMessage(String.Format("* Script {0} Key {1} : Old Value: {2}",
                                                /*0*/iTag.ToString("000"),
                                                /*1*/sKey,
                                                /*2*/sValue));
                                            ;

                                        sKey   = kvp2.Key.ToString();
                                        sValue = Convert.ToString(kvp2.Value);
                                        LogMessage(String.Format("* Script {0} Key {1} : New Value: {2}",
                                                /*0*/iTag.ToString("000"),
                                                /*1*/sKey,
                                                /*2*/sValue));
                                        ScriptsToApply[iTag].ScriptParameters[kvp2.Key] = sValue;
                                    }
                                    else
                                    {
                                        sKey   = kvp2.Key.ToString();
                                        sValue = Convert.ToString(ScriptsToApply[iTag].ScriptParameters[kvp2.Key]); 
                                        LogMessage(String.Format("Script {0} Key {1} : Same Value: {2}",
                                                /*0*/iTag.ToString("000"),
                                                /*1*/sKey,
                                                /*2*/sValue));
                                    }
                                }
                                else
                                {
                                    sKey   = kvp2.Key.ToString();
                                    LogMessage(String.Format("Script {0} ERROR Key {1} : Can not find key",
                                                /*0*/iTag.ToString("000"),
                                                /*1*/sKey
                                                ));
                                }                                

                            }
                        }
  
                    }
                    else 
                    {
                        frmGetVariableValueBulk.Dispose();
                        if ((GetVariableDialogResult == DialogResult.Abort) |
                            (GetVariableDialogResult == DialogResult.Cancel))
                        {
                            //Environment.FailFast("");
                            IsCanceled = 1;
                            LogMessage("");
                            LogMessage("");
                            LogMessage(String.Format("============================================================="));
                            LogMessage(String.Format("==============     Application was Canceled       ==========="));
                            LogMessage(String.Format("============================================================="));
                            LogMessage("");
                            ScriptsToApply.Clear();
                            Application.Exit();
                        }
                        
                    }
                    if (IsCanceled == 0) {
                        LogMessage(String.Format("==============     END Replace with user Values       ***********"));
                    }
                    
                }

            }
            catch (Exception Ex)
            {
                LogMessage(seperator, true);
                LogMessage(Ex.ToString(), true);
                LogMessage(seperator, true);

                Environment.Exit(constExceptionOccured);
            }
            return ScriptsToApply;
        }

        private static Dictionary<string, string> ParseScriptForParametersOrDatabase(SettingsLocation settingLoc, string scriptPath, string script, out string database, out int AddVarToDlgFlag)
        {
            Dictionary<string, string> dictVariableValues = new Dictionary<string, string>();

            // Dont give this option if service is running program
            database = strDatabase.Trim();

            AddVarToDlgFlag = 0;

            //if (IsServiceRunningProgram) return dictVariableValues;

            using (StreamReader scriptText = new StreamReader(scriptPath))
            {
                StringBuilder strBldr = new StringBuilder();
                bool firstlineParsed = false;

                while (!scriptText.EndOfStream)
                {
                    string lineOfText = string.Empty;

                    lineOfText = scriptText.ReadLine().Trim();

                    if (!firstlineParsed && !string.IsNullOrEmpty(lineOfText))
                    {
                        firstlineParsed = true;

                        lineOfText = lineOfText.Replace('[', ' ').Replace(']', ' ').Replace(';', ' ');

                        if (lineOfText.ToLower().StartsWith("use "))
                        {
                            database = lineOfText.Substring(lineOfText.IndexOf(' ') + 1).Trim();

                            LogMessage(string.Format("Changing DB Script Context from {0} to {1}", strDatabase, database));
                        }
                    }

                    strBldr.AppendLine(lineOfText);
                }

                string fileText = strBldr.ToString().Trim();

                string sPattern = @"\$\(\w*\)";

                var matches = Regex.Matches(fileText, sPattern, RegexOptions.IgnoreCase);
                var uniqueMatches = matches
                    .OfType<Match>()
                    .Select(m => m.Value)
                    .Distinct();

                try
                {

                    foreach (var match in uniqueMatches)
                    {
                        string variableName = match.Replace("$", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty);
                        string value = null;

                        if (settingLoc.ScriptVariablesList != null && settingLoc.ScriptVariablesList.Any(x => x.ScriptName.ToLower() == script))
                        {
                            var values = settingLoc.ScriptVariablesList.Where(x => x.ScriptName.ToLower() == script && x.ScriptParameters.ContainsKey(variableName)).ToList();
                            if(values.Any())
                            {
                                value = values.First().ScriptParameters[variableName];
                            }
                        }

                        if (!IsServiceRunningProgram)
                        {
                            if (!HideScriptVariablePopup)
                            {
                                AddVarToDlgFlag = 1;
                                // OLD
                                //GetVariableValue frmVariableValue = new GetVariableValue();
                                //frmVariableValue.Show(script, variableName, value);
                                //value = frmVariableValue.VariableValue;                                                              
                            }
                        }

                        dictVariableValues.Add(variableName, value);
                    }
                }
                catch (Exception Ex)
                {
                    LogMessage(seperator, true);
                    LogMessage(Ex.ToString(), true);
                    LogMessage(seperator, true);
                }
            }            
            return dictVariableValues;
        }

        private static string GetAbsolutePath(string relativePath)
        {
            if (relativePath.IndexOf(@"..\") >= 0)
            {
                var endPath = relativePath.Substring(relativePath.LastIndexOf(@"..\") + 3);

                var start = relativePath.Replace(endPath, "");
                int levels = 0;
                while (start.EndsWith(@"..\"))
                {
                    levels++;
                    start = start.Substring(0, start.LastIndexOf(@"..\"));
                }

                if (start.EndsWith(@"\"))
                    start = start.Substring(0, start.Length - 1);

                while (levels > 0)
                {
                    start = start.Substring(0, start.LastIndexOf(@"\"));
                    levels--;
                }

                relativePath = string.Format(@"{0}\{1}", start, endPath);
            }

            return relativePath;
        }

        #endregion
    }
}