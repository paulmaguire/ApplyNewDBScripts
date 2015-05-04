using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApplyNewDBScriptsV2
{
    public partial class frmReplaceDlg : Form
    {
        public frmReplaceDlg()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public string GetFindValue
        {
            get { return txtBoxFind.Text; }
        }

        public string GetReplaceValue
        {
            get { return txtBoxReplace.Text; }
        }

        public void SetFindValue(string strValue)
        {
            txtBoxFind.Text = strValue;
            //return true; chkCaseSensitive
        }

        public bool GetIsCaseSensitive
        {
            get { return chkCaseSensitive.Checked; }
        }
    }
}
