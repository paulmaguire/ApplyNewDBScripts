namespace ApplyNewDBScriptsV2
{
    partial class MemoryBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryBox));
            this.buttonYes = new System.Windows.Forms.Button();
            this.buttonYestoAll = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.buttonNotoAll = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelBody = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonYes
            // 
            this.buttonYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonYes.Location = new System.Drawing.Point(21, 104);
            this.buttonYes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(100, 28);
            this.buttonYes.TabIndex = 1;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // buttonYestoAll
            // 
            this.buttonYestoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonYestoAll.Location = new System.Drawing.Point(129, 104);
            this.buttonYestoAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonYestoAll.Name = "buttonYestoAll";
            this.buttonYestoAll.Size = new System.Drawing.Size(100, 28);
            this.buttonYestoAll.TabIndex = 2;
            this.buttonYestoAll.Text = "Yes to All";
            this.buttonYestoAll.UseVisualStyleBackColor = true;
            this.buttonYestoAll.Click += new System.EventHandler(this.buttonYestoAll_Click);
            // 
            // buttonNo
            // 
            this.buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNo.Location = new System.Drawing.Point(237, 104);
            this.buttonNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(100, 28);
            this.buttonNo.TabIndex = 3;
            this.buttonNo.Text = "No";
            this.buttonNo.UseVisualStyleBackColor = true;
            this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // buttonNotoAll
            // 
            this.buttonNotoAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNotoAll.Location = new System.Drawing.Point(345, 104);
            this.buttonNotoAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonNotoAll.Name = "buttonNoToAll";
            this.buttonNotoAll.Size = new System.Drawing.Size(100, 28);
            this.buttonNotoAll.TabIndex = 4;
            this.buttonNotoAll.Text = "No to All";
            this.buttonNotoAll.UseVisualStyleBackColor = true;
            this.buttonNotoAll.Click += new System.EventHandler(this.buttonNotoAll_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(453, 104);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(100, 28);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // labelBody
            // 
            this.labelBody.AutoSize = true;
            this.labelBody.Location = new System.Drawing.Point(16, 11);
            this.labelBody.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBody.Name = "labelBody";
            this.labelBody.Size = new System.Drawing.Size(0, 17);
            this.labelBody.TabIndex = 6;
            // 
            // MemoryBox
            // 
            this.AcceptButton = this.buttonYes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(579, 147);
            this.Controls.Add(this.labelBody);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonNotoAll);
            this.Controls.Add(this.buttonNo);
            this.Controls.Add(this.buttonYestoAll);
            this.Controls.Add(this.buttonYes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemoryBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Confirm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonYestoAll;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.Button buttonNotoAll;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelBody;
    }
}