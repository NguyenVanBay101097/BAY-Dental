namespace Demo
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.lblLogout = new System.Windows.Forms.Label();
            this.lblAccountName = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.content = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.lblLogout);
            this.panel1.Controls.Add(this.lblAccountName);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(694, 43);
            this.panel1.TabIndex = 0;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button4.FlatAppearance.BorderSize = 0;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button4.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button4.Location = new System.Drawing.Point(394, 13);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(64, 30);
            this.button4.TabIndex = 0;
            this.button4.Text = "Lịch sử";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblLogout
            // 
            this.lblLogout.BackColor = System.Drawing.Color.Transparent;
            this.lblLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLogout.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogout.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblLogout.Location = new System.Drawing.Point(532, 17);
            this.lblLogout.Name = "lblLogout";
            this.lblLogout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblLogout.Size = new System.Drawing.Size(145, 17);
            this.lblLogout.TabIndex = 1;
            this.lblLogout.Text = "Đăng xuất";
            this.lblLogout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblLogout.Click += new System.EventHandler(this.lblLogout_Click);
            // 
            // lblAccountName
            // 
            this.lblAccountName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAccountName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblAccountName.Location = new System.Drawing.Point(532, 2);
            this.lblAccountName.Name = "lblAccountName";
            this.lblAccountName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAccountName.Size = new System.Drawing.Size(145, 20);
            this.lblAccountName.TabIndex = 1;
            this.lblAccountName.Text = "lblAccountName";
            this.lblAccountName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button3.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button3.Location = new System.Drawing.Point(266, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(129, 30);
            this.button3.TabIndex = 0;
            this.button3.Text = "Dữ liệu chấm công";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button2.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.button2.Location = new System.Drawing.Point(120, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(146, 30);
            this.button2.TabIndex = 0;
            this.button2.Text = "Tài khoản chấm công";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.button1.Location = new System.Drawing.Point(0, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Máy chấm công";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // content
            // 
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.Location = new System.Drawing.Point(0, 43);
            this.content.Name = "content";
            this.content.Size = new System.Drawing.Size(694, 339);
            this.content.TabIndex = 1;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(694, 382);
            this.Controls.Add(this.content);
            this.Controls.Add(this.panel1);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TMTDentalFPS";
            this.Load += new System.EventHandler(this.Main_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel content;
        private System.Windows.Forms.Label lblAccountName;
        private System.Windows.Forms.Label lblLogout;
        private System.Windows.Forms.Button button4;
    }
}