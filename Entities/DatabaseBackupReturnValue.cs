using System.Linq;

namespace ApplyNewDBScriptsV2.Entities
{
    public class DatabaseBackupReturnValue
    {
        public string Message { get; set; }
        public int? Rtflg { get; set; }

        public DatabaseBackupReturnValue(string strQueryOutput)
        {
            if (!string.IsNullOrEmpty(strQueryOutput))
            {
                strQueryOutput = strQueryOutput.Trim();
                while (strQueryOutput.IndexOf("  ") >= 0)
                {
                    strQueryOutput = strQueryOutput.Replace("  ", " ");
                }

                string[] arrOutput = strQueryOutput.Split(' ');
                if (arrOutput != null && arrOutput.Count() >= 2)
                {
                    int outInt = 0;
                    if (int.TryParse(arrOutput[0], out outInt))
                    {
                        this.Rtflg = outInt;
                        for (int i = 1; i < arrOutput.Count(); i++)
                            this.Message += arrOutput[i] + " ";
                        this.Message = this.Message.Trim();
                    }
                }
            }
        }

        public DatabaseBackupReturnValue()
        {

        }
    }
}
