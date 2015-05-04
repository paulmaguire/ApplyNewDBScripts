using System;
using System.Windows.Forms;

namespace ApplyNewDBScriptsV2
{
    public partial class GetVariableValue : Form
    {
        public string VariableValue
        {
            get { return txtVariableValue.Text; }
        }

        public bool CheckBoxValue
        {
            get { return chkDontShowThisAgain.Checked; }
        }

        public bool CheckBoxVisible
        {
            get { return chkDontShowThisAgain.Visible; }
            set { chkDontShowThisAgain.Visible = value; }
        }

        public GetVariableValue()
        {
            InitializeComponent();
        }

        public void Show(string fileDisplayName, string variableName, string defaultVariableValue)
        {
            lblFileName.Text = fileDisplayName;
            lblVariableName.Text = variableName;
            txtVariableValue.Text = defaultVariableValue;

            ShowDialog();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void chkDontShowThisAgain_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
