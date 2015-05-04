namespace ApplyNewDBScriptsV2
{
    partial class GetVariableValue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GetVariableValue));
            this.lblVariableName = new System.Windows.Forms.Label();
            this.txtVariableValue = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblFileName = new System.Windows.Forms.Label();
            this.chkDontShowThisAgain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblVariableName
            // 
            this.lblVariableName.Location = new System.Drawing.Point(16, 41);
            this.lblVariableName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVariableName.Name = "lblVariableName";
            this.lblVariableName.Size = new System.Drawing.Size(563, 25);
            this.lblVariableName.TabIndex = 0;
            this.lblVariableName.Text = "VariableName";
            // 
            // txtVariableValue
            // 
            this.txtVariableValue.Location = new System.Drawing.Point(21, 74);
            this.txtVariableValue.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtVariableValue.Name = "txtVariableValue";
            this.txtVariableValue.Size = new System.Drawing.Size(561, 22);
            this.txtVariableValue.TabIndex = 1;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(376, 107);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(100, 28);
            this.btnConfirm.TabIndex = 6;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(484, 107);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.Location = new System.Drawing.Point(17, 11);
            this.lblFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(563, 25);
            this.lblFileName.TabIndex = 8;
            this.lblFileName.Text = "fileName";
            // 
            // chkDontShowThisAgain
            // 
            this.chkDontShowThisAgain.AutoSize = true;
            this.chkDontShowThisAgain.Location = new System.Drawing.Point(21, 107);
            this.chkDontShowThisAgain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkDontShowThisAgain.Name = "chkDontShowThisAgain";
            this.chkDontShowThisAgain.Size = new System.Drawing.Size(169, 21);
            this.chkDontShowThisAgain.TabIndex = 9;
            this.chkDontShowThisAgain.Text = "Dont Show This Again";
            this.chkDontShowThisAgain.UseVisualStyleBackColor = true;
            this.chkDontShowThisAgain.Visible = false;
            this.chkDontShowThisAgain.CheckedChanged += new System.EventHandler(this.chkDontShowThisAgain_CheckedChanged);
            // 
            // GetVariableValue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 146);
            this.Controls.Add(this.chkDontShowThisAgain);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtVariableValue);
            this.Controls.Add(this.lblVariableName);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GetVariableValue";
            this.Text = "Enter Variable Value";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVariableName;
        private System.Windows.Forms.TextBox txtVariableValue;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.CheckBox chkDontShowThisAgain;
    }
}