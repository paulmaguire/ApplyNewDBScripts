using System;

namespace ApplyNewDBScriptsV2.Entities
{
    public class AppliedScript
    {
        public int ID { get; set; }
        public string Hash { get; set; }
        public DateTime DateScriptApplied { get; set; }
        public string PreviousAuthor { get; set; }
        public string ScriptPath { get; set; }
        public string Comment { get; set; }
        public bool IsRunOnce { get; set; }
        public bool IsForceApplied { get; set; }
        public string Revision { get; set; }

    }
}
