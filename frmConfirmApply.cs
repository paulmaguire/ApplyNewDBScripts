using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ApplyNewDBScriptsV2.Entities;
using Microsoft.Win32;

namespace ApplyNewDBScriptsV2
{
    public partial class frmConfirmApply : Form
    {
        private double numSecondsDisplayed = 0;
        public delegate void ButtonClick(string selectedSettingsFilePath);
        public event ButtonClick ButtonClick_Yes;
        public event ButtonClick ButtonClick_No;
        public const int iTimerDuration = 10000;

        public frmConfirmApply()
        {
            InitializeComponent();
        }

        private void StoreSettings(string database, bool bConfirmed)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
            {
                if (key != null)
                    key.SetValue(database + "HideScriptVariablePopup", chkHideScriptVariablePopup.Checked);
            }

            // Store always apply setting
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
            {
                if (key != null)
                    key.SetValue(database, (bConfirmed && chkAlways.Checked));
            }

            if (chkNotForNMinutes.Checked)
            {
                int mins = 60;
                if (int.TryParse(txtMins.Text, out mins))
                    mins = int.Parse(txtMins.Text);

                // Store ignore scripts for next hour setting
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
                {
                    if (key != null)
                        key.SetValue(database + "AskAgainAfter", DateTime.Now.AddMinutes(mins));
                }
            }

            if (chkNever.Checked)
            {
                // Store Never apply to this environment setting
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
                {
                    if (key != null)
                        key.SetValue(database + "Never", chkNever.Checked);
                }
            }
            // Store setting in for display individual filenames
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
            {
                if (key != null)
                    key.SetValue(database + "ShowFileNames", chkShowFilenames.Checked);
            }
            // Store setting in for include unversioned files
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\RBT\\ApplyNewDBScripts"))
            {
                if (key != null)
                    key.SetValue(database + "ApplyUnversionedScripts", chkIncludeUnversionedFiles.Checked);
            }
        }

        public void Show(string database, string strSettingsFileName)
        {
            // Check if user selected to always apply
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    bool bRetVal = bool.Parse(key.GetValue(database, false).ToString());
                    if (bRetVal)
                    {
                        ConfirmYesClick();
                        return;
                    }
                }
            }

            // Check if user has asked to ignore scripts for next hour and if hour has passed
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    DateTime dtNextAsk = DateTime.Parse(key.GetValue(database + "AskAgainAfter", DateTime.MinValue).ToString());
                    if (dtNextAsk >= DateTime.Now)
                    {
                        ConfirmNoClick();
                        return;
                    }
                }
            }

            // Check if user has asked to Never apply scripts to this environment
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    bool bRetVal = bool.Parse(key.GetValue(database + "Never", false).ToString());
                    if (bRetVal)
                    {
                        ConfirmNoClick();
                        return;
                    }
                }
            }

            // Set previous setting
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    chkShowFilenames.Checked = bool.Parse(key.GetValue(database + "ShowFileNames", false).ToString());
                }
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    chkShowFilenames.Checked = bool.Parse(key.GetValue(database + "ShowFileNames", false).ToString());
                }
            }
            
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\RBT\\ApplyNewDBScripts", false))
            {
                if (key != null)
                {
                    chkHideScriptVariablePopup.Checked = bool.Parse(key.GetValue(database + "HideScriptVariablePopup", false).ToString());
                }
            }

            lblDatabase.Text = database;
            var fileInfo = new FileInfo(strSettingsFileName);
            var directoryPath = Directory.GetParent(strSettingsFileName).FullName + @"\";
            if (fileInfo.Directory != null)
            {
                ddlSettingFiles.DataSource = fileInfo.Directory.EnumerateFiles("*.xml").Where(a => !a.FullName.ToLower().Contains("template")).Select(d => d.FullName.Replace(directoryPath, string.Empty).Trim()).ToArray();
                ddlSettingFiles.SelectedItem = fileInfo.Name;
            }

            ShowDialog();
        }
        
        private void ConfirmYesClick()
        {
            Hide();

            StoreSettings(lblDatabase.Text, true);

            if (ButtonClick_Yes != null)
                ButtonClick_Yes(ddlSettingFiles.SelectedValue.ToString());
        }

        private void ConfirmNoClick()
        {
            this.Hide();
            StoreSettings(lblDatabase.Text, false);

            if (ButtonClick_No != null)
                ButtonClick_No(string.Empty);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            ConfirmYesClick();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            ConfirmNoClick();
        }

        private void txtMins_TextChanged(object sender, EventArgs e)
        {
            chkNotForNMinutes.Checked = true;
        }

    }
}