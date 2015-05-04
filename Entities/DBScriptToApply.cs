using System.Collections.Generic;
using System.IO;

namespace ApplyNewDBScriptsV2.Entities
{
    public class DBScriptToApply
    {
        #region Public Properties

        public string Hash { get; set; }
        public string FullyQualifiedPath { get; set; }
        
        public string RelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(_relativePath))
                {
                    var scriptToApplyLoweredRelativePath = Path.GetFullPath(FullyQualifiedPath).ToLower();

                    // turns "..\Views" into "\Views"
                    string cleanPath = AssociatedSettingLocation.RelativePath.Replace(@"..\", "");
                    // turns "Schema" into "\Schema"
                    if (cleanPath[0] != '\\')
                        cleanPath = '\\' + cleanPath;
                    // get rid of accidental extra spaces
                    cleanPath = cleanPath.Trim();

                    _relativePath = scriptToApplyLoweredRelativePath.Substring(scriptToApplyLoweredRelativePath.LastIndexOf(cleanPath, System.StringComparison.OrdinalIgnoreCase));
                }

                return _relativePath;
            } 
        }

        public string ScriptName { get; set; }
        public string ParentFolder { get; set; }
        public string Database { get; set; }
                
        public bool IsAppliedPreviously { get; set; }        

        public AppliedScript  AppliedScriptInfo { get; set; }
        public Dictionary<string, string> ScriptParameters { get; set; } 
        public SVNInfo SVNInformation { get; set; }
        public SettingsLocation AssociatedSettingLocation { get; set; }

        #endregion Public Properties

        #region Private properties

        private string _relativePath;

        #endregion

        #region Constructors

        #endregion

        #region Public Methods



        #endregion
    }
}
