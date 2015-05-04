using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApplyNewDBScriptsV2.Entities
{
    public class ScriptVariables
    {
        public string ScriptName { get; set; }
        public Dictionary<string, string> ScriptParameters { get; set; }
    }
}
