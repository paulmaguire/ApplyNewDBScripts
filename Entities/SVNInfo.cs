using System;
using System.IO;

namespace ApplyNewDBScriptsV2.Entities
{
    public enum SVNInfoEntryKind
    {
        File,
        Directory
    }

    public class SVNInfo
    {
        public string BasePath { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string RepositoryRoot { get; set; }
        public int Revision { get; set; }
        public string GitHash { get; set; }
        public string NodeKind { get; set; }
        public string Schedule { get; set; }
        public string LastChangedAuthor { get; set; }
        public int LastChangedRev { get; set; }
        public string LastChangedDate { get; set; }
        public string TextLastUpdated { get; set; }
        public string Checksum { get; set; }

        public SVNInfoEntryKind Kind { get; set; }
        
        public SVNInfo()
        {

        }

        public SVNInfo(StreamReader svnInfoOutput)
        {
            string strLine = svnInfoOutput.ReadLine();
            while (!string.IsNullOrEmpty(strLine))
            {
                if (strLine.ToLower().StartsWith("path: "))
                    this.Path = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("name: "))
                    this.Name = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("url: "))
                    this.URL = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("repository root: "))
                    this.RepositoryRoot = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("revision: "))
                    this.Revision = Convert.ToInt32(strLine.Substring(strLine.IndexOf(": ") + 2).Trim());

                if (strLine.ToLower().StartsWith("node kind: "))
                    this.NodeKind = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("schedule: "))
                    this.Schedule = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("last changed author: "))
                    this.LastChangedAuthor = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("last changed rev: "))
                    this.LastChangedRev = Convert.ToInt32(strLine.Substring(strLine.IndexOf(": ") + 2).Trim());

                if (strLine.ToLower().StartsWith("last changed date: "))
                    this.LastChangedDate = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("text last updated: "))
                    this.TextLastUpdated = strLine.Substring(strLine.IndexOf(": ") + 2);

                if (strLine.ToLower().StartsWith("checksum: "))
                    this.Checksum = strLine.Substring(strLine.IndexOf(": ") + 2);

                strLine = svnInfoOutput.ReadLine();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Kind, Path);
        }
    }
}
