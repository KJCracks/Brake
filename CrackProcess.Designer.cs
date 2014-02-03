namespace Brake
{
    partial class CrackProcess
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
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.appProgressBar = new System.Windows.Forms.ProgressBar();
            this.appStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // logBox
            // 
            this.logBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.logBox.Location = new System.Drawing.Point(12, 12);
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.Size = new System.Drawing.Size(759, 245);
            this.logBox.TabIndex = 0;
            this.logBox.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 308);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(759, 23);
            this.progressBar1.TabIndex = 1;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 276);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Status:";
            
            //
            // appProgressBar
            // 
            this.appProgressBar.Location = new System.Drawing.Point(12, 388);
            this.appProgressBar.Name = "appProgressBar";
            this.appProgressBar.Size = new System.Drawing.Size(759, 23);
            this.appProgressBar.TabIndex = 3;
            // 
            // appStatusLabel
            // 
            this.appStatusLabel.AutoSize = true;
            this.appStatusLabel.Location = new System.Drawing.Point(12, 362);
            this.appStatusLabel.Name = "appStatusLabel";
            this.appStatusLabel.Size = new System.Drawing.Size(73, 13);
            this.appStatusLabel.TabIndex = 4;
            this.appStatusLabel.Text = "Cracking app:";
            
            // 
            // CrackProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 422);
            this.Controls.Add(this.appStatusLabel);
            this.Controls.Add(this.appProgressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.logBox);
            this.Name = "CrackProcess";
            this.Text = "CrackProcess";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar appProgressBar;
        private System.Windows.Forms.Label appStatusLabel;

    }
}