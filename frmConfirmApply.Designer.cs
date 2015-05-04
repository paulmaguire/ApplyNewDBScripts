namespace ApplyNewDBScriptsV2
{
    partial class frmConfirmApply
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfirmApply));
            this.label1 = new System.Windows.Forms.Label();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.chkAlways = new System.Windows.Forms.CheckBox();
            this.chkNotForNMinutes = new System.Windows.Forms.CheckBox();
            this.chkNever = new System.Windows.Forms.CheckBox();
            this.txtMins = new System.Windows.Forms.TextBox();
            this.chkShowFilenames = new System.Windows.Forms.CheckBox();
            this.grpBxAdvanced = new System.Windows.Forms.GroupBox();
            this.chkHideScriptVariablePopup = new System.Windows.Forms.CheckBox();
            this.chkIncludeUnversionedFiles = new System.Windows.Forms.CheckBox();
            this.ddlSettingFiles = new System.Windows.Forms.ComboBox();
            this.grpBxAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(52, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Do you want to apply new scripts to ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblDatabase
            // 
            this.lblDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDatabase.Location = new System.Drawing.Point(3, 38);
            this.lblDatabase.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(401, 28);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "Server\\Database";
            this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnYes
            // 
            this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(97, 87);
            this.btnYes.Margin = new System.Windows.Forms.Padding(4);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(100, 28);
            this.btnYes.TabIndex = 3;
            this.btnYes.Text = "Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnNo
            // 
            this.btnNo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(205, 87);
            this.btnNo.Margin = new System.Windows.Forms.Padding(4);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(100, 28);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // chkAlways
            // 
            this.chkAlways.AutoSize = true;
            this.chkAlways.Location = new System.Drawing.Point(11, 78);
            this.chkAlways.Margin = new System.Windows.Forms.Padding(4);
            this.chkAlways.Name = "chkAlways";
            this.chkAlways.Size = new System.Drawing.Size(373, 21);
            this.chkAlways.TabIndex = 2;
            this.chkAlways.Text = "Always apply scripts to this environment without asking";
            this.chkAlways.UseVisualStyleBackColor = true;
            // 
            // chkNotForNMinutes
            // 
            this.chkNotForNMinutes.AutoSize = true;
            this.chkNotForNMinutes.Location = new System.Drawing.Point(11, 23);
            this.chkNotForNMinutes.Margin = new System.Windows.Forms.Padding(4);
            this.chkNotForNMinutes.Name = "chkNotForNMinutes";
            this.chkNotForNMinutes.Size = new System.Drawing.Size(391, 21);
            this.chkNotForNMinutes.TabIndex = 4;
            this.chkNotForNMinutes.Text = "Skip this step for this environment for next            minutes";
            this.chkNotForNMinutes.UseVisualStyleBackColor = true;
            // 
            // chkNever
            // 
            this.chkNever.AutoSize = true;
            this.chkNever.Location = new System.Drawing.Point(11, 49);
            this.chkNever.Margin = new System.Windows.Forms.Padding(4);
            this.chkNever.Name = "chkNever";
            this.chkNever.Size = new System.Drawing.Size(275, 21);
            this.chkNever.TabIndex = 6;
            this.chkNever.Text = "Never apply scripts to this environment";
            this.chkNever.UseVisualStyleBackColor = true;
            // 
            // txtMins
            // 
            this.txtMins.Location = new System.Drawing.Point(303, 20);
            this.txtMins.Margin = new System.Windows.Forms.Padding(4);
            this.txtMins.Name = "txtMins";
            this.txtMins.Size = new System.Drawing.Size(33, 22);
            this.txtMins.TabIndex = 7;
            this.txtMins.Text = "60";
            this.txtMins.TextChanged += new System.EventHandler(this.txtMins_TextChanged);
            // 
            // chkShowFilenames
            // 
            this.chkShowFilenames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowFilenames.AutoSize = true;
            this.chkShowFilenames.Location = new System.Drawing.Point(11, 134);
            this.chkShowFilenames.Margin = new System.Windows.Forms.Padding(4);
            this.chkShowFilenames.Name = "chkShowFilenames";
            this.chkShowFilenames.Size = new System.Drawing.Size(281, 21);
            this.chkShowFilenames.TabIndex = 8;
            this.chkShowFilenames.Text = "Show individual scripts before execution";
            this.chkShowFilenames.UseVisualStyleBackColor = true;
            // 
            // grpBxAdvanced
            // 
            this.grpBxAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBxAdvanced.Controls.Add(this.chkHideScriptVariablePopup);
            this.grpBxAdvanced.Controls.Add(this.chkIncludeUnversionedFiles);
            this.grpBxAdvanced.Controls.Add(this.chkNever);
            this.grpBxAdvanced.Controls.Add(this.chkShowFilenames);
            this.grpBxAdvanced.Controls.Add(this.chkAlways);
            this.grpBxAdvanced.Controls.Add(this.txtMins);
            this.grpBxAdvanced.Controls.Add(this.chkNotForNMinutes);
            this.grpBxAdvanced.Location = new System.Drawing.Point(5, 118);
            this.grpBxAdvanced.Margin = new System.Windows.Forms.Padding(4);
            this.grpBxAdvanced.Name = "grpBxAdvanced";
            this.grpBxAdvanced.Padding = new System.Windows.Forms.Padding(4);
            this.grpBxAdvanced.Size = new System.Drawing.Size(403, 190);
            this.grpBxAdvanced.TabIndex = 9;
            this.grpBxAdvanced.TabStop = false;
            this.grpBxAdvanced.Text = "Options";
            // 
            // chkHideScriptVariablePopup
            // 
            this.chkHideScriptVariablePopup.AutoSize = true;
            this.chkHideScriptVariablePopup.Location = new System.Drawing.Point(11, 161);
            this.chkHideScriptVariablePopup.Margin = new System.Windows.Forms.Padding(4);
            this.chkHideScriptVariablePopup.Name = "chkHideScriptVariablePopup";
            this.chkHideScriptVariablePopup.Size = new System.Drawing.Size(242, 21);
            this.chkHideScriptVariablePopup.TabIndex = 11;
            this.chkHideScriptVariablePopup.Text = "Don\'t Show Script Variable Popup";
            this.chkHideScriptVariablePopup.UseVisualStyleBackColor = true;
            // 
            // chkIncludeUnversionedFiles
            // 
            this.chkIncludeUnversionedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkIncludeUnversionedFiles.AutoSize = true;
            this.chkIncludeUnversionedFiles.Location = new System.Drawing.Point(11, 106);
            this.chkIncludeUnversionedFiles.Margin = new System.Windows.Forms.Padding(4);
            this.chkIncludeUnversionedFiles.Name = "chkIncludeUnversionedFiles";
            this.chkIncludeUnversionedFiles.Size = new System.Drawing.Size(227, 21);
            this.chkIncludeUnversionedFiles.TabIndex = 10;
            this.chkIncludeUnversionedFiles.Text = "Ask me about unversioned files";
            this.chkIncludeUnversionedFiles.UseVisualStyleBackColor = true;
            // 
            // ddlSettingFiles
            // 
            this.ddlSettingFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlSettingFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ddlSettingFiles.FormattingEnabled = true;
            this.ddlSettingFiles.Location = new System.Drawing.Point(5, 57);
            this.ddlSettingFiles.Name = "ddlSettingFiles";
            this.ddlSettingFiles.Size = new System.Drawing.Size(403, 24);
            this.ddlSettingFiles.TabIndex = 11;
            // 
            // frmConfirmApply
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(419, 317);
            this.ControlBox = false;
            this.Controls.Add(this.ddlSettingFiles);
            this.Controls.Add(this.grpBxAdvanced);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDatabase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmConfirmApply";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Apply Database Scripts - Confirm";
            this.TopMost = true;
            this.grpBxAdvanced.ResumeLayout(false);
            this.grpBxAdvanced.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.CheckBox chkAlways;
        private System.Windows.Forms.CheckBox chkNotForNMinutes;
        private System.Windows.Forms.CheckBox chkNever;
        private System.Windows.Forms.TextBox txtMins;
        private System.Windows.Forms.CheckBox chkShowFilenames;
        private System.Windows.Forms.GroupBox grpBxAdvanced;
        private System.Windows.Forms.CheckBox chkIncludeUnversionedFiles;
        private System.Windows.Forms.CheckBox chkHideScriptVariablePopup;
        private System.Windows.Forms.ComboBox ddlSettingFiles;
    }
}