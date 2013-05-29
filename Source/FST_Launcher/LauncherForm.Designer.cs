namespace FST_Launcher
{
    sealed partial class LauncherForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LauncherForm));
            this.lblApplicationName = new System.Windows.Forms.Label();
            this.listBoxOutput = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblApplicationName
            // 
            this.lblApplicationName.AutoSize = true;
            this.lblApplicationName.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblApplicationName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblApplicationName.Location = new System.Drawing.Point(64, 35);
            this.lblApplicationName.Name = "lblApplicationName";
            this.lblApplicationName.Size = new System.Drawing.Size(222, 30);
            this.lblApplicationName.TabIndex = 0;
            this.lblApplicationName.Text = "Forex Strategy Trader";
            // 
            // listBoxOutput
            // 
            this.listBoxOutput.BackColor = System.Drawing.SystemColors.Control;
            this.listBoxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxOutput.ForeColor = System.Drawing.SystemColors.ControlText;
            this.listBoxOutput.FormattingEnabled = true;
            this.listBoxOutput.ItemHeight = 15;
            this.listBoxOutput.Location = new System.Drawing.Point(96, 120);
            this.listBoxOutput.Name = "listBoxOutput";
            this.listBoxOutput.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxOutput.Size = new System.Drawing.Size(230, 135);
            this.listBoxOutput.TabIndex = 3;
            this.listBoxOutput.TabStop = false;
            // 
            // LauncherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(420, 280);
            this.Controls.Add(this.listBoxOutput);
            this.Controls.Add(this.lblApplicationName);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LauncherForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FST Launcher";
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FormLauncher_MouseDoubleClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblApplicationName;
        private System.Windows.Forms.ListBox listBoxOutput;
    }
}

