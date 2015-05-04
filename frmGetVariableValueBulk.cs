using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ApplyNewDBScriptsV2
{
    public partial class frmGetVariableValueBulk : Form
    {
        int ControlPosTop = 10;
        int ControlStep = 20;
        int ControlLeft = 5;
        int ControlLeftShift = 10;
        int ControlStepNext = 20;
        int ScriptNumPrev = -1;
        int DelimiterNum = 0;
        string txtBoxPrefix = "_vv_";
        string lblVariablePrefix = "_v_";
        //string lblFileNamePrfix = "_";

        public bool CheckBoxValue
        {
            get { return chkDontShowThisAgain.Checked; }
        }

        public bool CheckBoxVisible
        {
            get { return chkDontShowThisAgain.Visible; }
            set { chkDontShowThisAgain.Visible = value; }
        }

        public frmGetVariableValueBulk()
        {
            InitializeComponent();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            //if (this.CanClose()) 
            this.DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Hide();
        }

        public int GetAssignedControlCount(int StopOnFirst = -1)
        {   // tis function will count just TextBox from pnlVariableControls
            // if return more like 0 then was a Popup
            int ControlsCount = 0;
            foreach (var MyControl in pnlVariableControls.Controls)
            {
                if (MyControl is TextBox)
                {
                    ControlsCount++;
                }
                if ((StopOnFirst == 1) &&
                     (ControlsCount > 0))
                {
                    break;
                }
            }

            return ControlsCount;
        }

        public int SetFocusToFirstTextBox()
        {   // tis function will count just TextBox from pnlVariableControls
            // if return more like 0 then was a Popup
            try
            {
                foreach (var MyControl in pnlVariableControls.Controls)
                {
                    if (MyControl is TextBox)
                    {
                        ((TextBox)MyControl).Select();
                        break;
                    }
                }
            }
            catch (Exception Ex)
            {
                Ex.ToString();
            }
            return 1;
        }

        public void AddVariable(int scriptNum, string fileScriptName, string variableName, string defaultVariableValue)
        {
            // Actually this function is building controls
            // to do:
            // create an multidimensional class and build control against this class

            string sScriptNumber = "";

            // Label - addd delimiter - Group
            if (ScriptNumPrev != scriptNum)
            {
                DelimiterNum++;
                ScriptNumPrev = scriptNum;
                Label lblDelimiter = new Label();
                lblDelimiter.Name = "lbl_dl_" + scriptNum.ToString() + "_" + fileScriptName;
                lblDelimiter.AutoSize = true;
                pnlVariableControls.Controls.Add(lblDelimiter);
                lblDelimiter.Left = ControlLeft;
                lblDelimiter.Top = ControlPosTop;
                lblDelimiter.Text = string.Format("({0})  {1}"
                                                 , DelimiterNum.ToString("000")
                                                 , fileScriptName
                                                 );
                ControlPosTop = ControlPosTop + ControlStep;
            }
            // Label Script (File) Name
            //Label lblFileName = new Label();
            //lblFileName.Name = "lbl_sn_" + scriptNum.ToString() + lblFileNamePrfix + fileScriptName;
            //if (fileScriptName != variableName)
            //{
            //    lblFileName.Text = fileScriptName;
            //}
            //lblFileName.AutoSize = true;
            //pnlVariableControls.Controls.Add(lblFileName);

            //lblFileName.Left = ControlLeft + ControlLeftShift;
            //lblFileName.Top = ControlPosTop;
            //ControlPosTop = ControlPosTop + ControlStep;

            // Label Variable Name
            #if DEBUG
              sScriptNumber = scriptNum.ToString("000") + " ";
            #endif

            Label lblVariable = new Label();
            lblVariable.Name = "lbl_" + scriptNum.ToString() + lblVariablePrefix + variableName;
            lblVariable.Text = sScriptNumber +
                               variableName;
            lblVariable.AutoSize = true;
            pnlVariableControls.Controls.Add(lblVariable);
            lblVariable.Left = ControlLeft + ControlLeftShift;
            lblVariable.Top = ControlPosTop; // lblFileName.Top + ControlStep;
            ControlPosTop = ControlPosTop + ControlStep;


            // Text Box Value
            TextBox txtBoxVariableValue = new TextBox();
            txtBoxVariableValue.Tag = scriptNum;
            txtBoxVariableValue.Name = "txt_" + scriptNum.ToString() + txtBoxPrefix + variableName;
            txtBoxVariableValue.Text = defaultVariableValue;
            pnlVariableControls.Controls.Add(txtBoxVariableValue);

            txtBoxVariableValue.Left = ControlLeft + ControlLeftShift;
            txtBoxVariableValue.Top = ControlPosTop;// lblVariable.Top + ControlStep;
            txtBoxVariableValue.Size = new Size(pnlVariableControls.Size.Width - (ControlLeft * 6), 22);
            txtBoxVariableValue.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            ControlPosTop = ControlPosTop + ControlStep;

            // Update settings
            ControlPosTop = txtBoxVariableValue.Top + ControlStep + ControlStepNext;
        }

        public void AddVariable(int scriptNum, string ScriptName, Dictionary<string, string> ParamsList )
        {
            if (ParamsList.Count > 0) 
            {
                foreach (var Param in ParamsList)
                {
                    AddVariable(scriptNum, ScriptName, Param.Key, Param.Value);
                }
            }
        }

        public Dictionary<int, Dictionary<string,string>> GetVariableValues()
        {
            Dictionary<int, Dictionary<string, string>> dictVariableValues = new Dictionary<int, Dictionary<string, string>>();

            string variableName = String.Empty;
            string variableValue = String.Empty;
            int StrPos;
            int LenPrefix = txtBoxPrefix.Length;
            int scriptNum;

            foreach (var MyControl in pnlVariableControls.Controls )
            {
                if (MyControl is TextBox)
                {
                    TextBox tb = (TextBox)MyControl;
                    //
                    variableName = tb.Name;
                    StrPos = variableName.IndexOf(txtBoxPrefix) + LenPrefix;
                    variableName = variableName.Remove(0, StrPos);
                    //
                    variableValue = tb.Text;
                    //
                    scriptNum = Int32.Parse(tb.Tag.ToString());
                    //
                    if (!dictVariableValues.ContainsKey(scriptNum))
                    {
                        dictVariableValues.Add(scriptNum, new Dictionary<string, string>() );
                    }
                    if (!dictVariableValues[scriptNum].ContainsKey(variableName))
                    {
                        dictVariableValues[scriptNum].Add(variableName, variableValue);
                    }
                }                
            }
            //scriptNum = 0;
            return dictVariableValues;
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            frmReplaceDlg frmGetVariableValueBulk = new frmReplaceDlg();
            string FindValue;
            string ReplaceValue;
            string SelectedText = "";
            string ResulValue = "";
            bool IsCaseSensitive;
            // get selection
            foreach (var MyControl in pnlVariableControls.Controls)
            {
                if (MyControl is TextBox) 
                {
                    TextBox tb = (TextBox)MyControl;
                    if (tb.SelectionLength > 0)
                    {
                        SelectedText = tb.SelectedText;
                        // clear selection
                        tb.DeselectAll();
                        tb.SelectionLength = 0;
                        break;
                    }
                }
            }

            frmGetVariableValueBulk.SetFindValue(SelectedText);

            // replace
            if (frmGetVariableValueBulk.ShowDialog() == DialogResult.OK)
            {
                FindValue = frmGetVariableValueBulk.GetFindValue;
                ReplaceValue = frmGetVariableValueBulk.GetReplaceValue;
                IsCaseSensitive = frmGetVariableValueBulk.GetIsCaseSensitive;

                if (FindValue != ReplaceValue)
                {
                    foreach (var MyControl in pnlVariableControls.Controls)
                    {
                        if (MyControl is TextBox)
                        {
                            TextBox tb = (TextBox)MyControl;
                            if (IsCaseSensitive == true)
                            {
                                StringBuilder builder = new StringBuilder(tb.Text);
                                builder.Replace(FindValue, ReplaceValue);
                                ResulValue = builder.ToString();
                            }
                            else
                            {
                                //Regex regex = new Regex(tb.Text, RegexOptions.IgnoreCase);
                                //ResulValue = regex.Replace(FindValue, ReplaceValue);
                                ResulValue = Regex.Replace(tb.Text, FindValue, ReplaceValue, RegexOptions.IgnoreCase);
                            }                            
                            tb.Text = ResulValue;
                        }
                    }
                }
                this.Invalidate(true);

            }
            frmGetVariableValueBulk.Dispose();
        }
    }
}
