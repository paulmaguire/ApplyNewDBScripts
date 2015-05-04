namespace ApplyNewDBScriptsV2.Entities
{
    public class Install3rdPartyAssembliesReturnValue
    {
        public bool RetVal { get; set; }
        public string Message { get; set; }

        public Install3rdPartyAssembliesReturnValue(bool retVal, string message)
        {
            this.Message = message;
            this.RetVal = retVal;
        }

        public Install3rdPartyAssembliesReturnValue()
        {
        }
    }
}