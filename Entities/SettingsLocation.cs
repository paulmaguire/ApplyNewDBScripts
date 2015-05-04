using System.IO;
using System.Collections.Generic;

namespace ApplyNewDBScriptsV2.Entities
{
    public class SettingsLocation
    {
        #region  Public Properties

        public bool isRecursive { get; set; }
        public bool isForceReApply { get; set; }
        public string BasePath { get; set; }
        public string RelativePath {get; set;}
        public bool isSingleRun { get; set; }
        public bool IsFile { get; set; }

        public List<ScriptVariables> ScriptVariablesList { get; set; }
        
        public string AbsolutePath
        {
            get
            {
                if (!string.IsNullOrEmpty(BasePath))
                {
                    return Path.GetFullPath(Path.Combine(BasePath, RelativePath));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        
        #endregion

    }
}
