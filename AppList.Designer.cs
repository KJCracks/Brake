namespace Brake
{
    partial class AppList
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.crackButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.crackAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 74);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(790, 304);
            this.listView1.TabIndex = 0;
            this.listView1.TileSize = new System.Drawing.Size(90, 90);
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Tile;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // crackButton
            // 
            this.crackButton.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crackButton.Location = new System.Drawing.Point(556, 21);
            this.crackButton.Name = "crackButton";
            this.crackButton.Size = new System.Drawing.Size(139, 36);
            this.crackButton.TabIndex = 1;
            this.crackButton.Text = "Crack Selected";
            this.crackButton.UseVisualStyleBackColor = true;
            this.crackButton.Click += new System.EventHandler(this.crackButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "Brake 0.0.6";
            // 
            // crackAllButton
            // 
            this.crackAllButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.crackAllButton.Location = new System.Drawing.Point(701, 21);
            this.crackAllButton.Name = "crackAllButton";
            this.crackAllButton.Size = new System.Drawing.Size(101, 36);
            this.crackAllButton.TabIndex = 3;
            this.crackAllButton.Text = "Crack All";
            this.crackAllButton.UseVisualStyleBackColor = true;
            this.crackAllButton.Click += new System.EventHandler(this.crackAllButton_Click);
            // 
            // AppList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 390);
            this.Controls.Add(this.crackAllButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.crackButton);
            this.Controls.Add(this.listView1);
            this.Name = "AppList";
            this.Text = "Brake";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.form_closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button crackButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button crackAllButton;
    }
}