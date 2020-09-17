using System;

namespace TMTTimeKeeper
{
    partial class Master
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Master));
            this.tbxDeviceIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxPort = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnPullData = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblDeviceInfo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dpDateFrom = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dpDateTo = new System.Windows.Forms.DateTimePicker();
            this.btnSyncData = new System.Windows.Forms.Button();
            this.dgvRecords = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lbEmployee = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxDeviceIP
            // 
            this.tbxDeviceIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxDeviceIP.Location = new System.Drawing.Point(55, 9);
            this.tbxDeviceIP.Name = "tbxDeviceIP";
            this.tbxDeviceIP.Size = new System.Drawing.Size(99, 22);
            this.tbxDeviceIP.TabIndex = 0;
            this.tbxDeviceIP.Text = "192.168.0.201";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Ip máy";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(160, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port máy";
            // 
            // tbxPort
            // 
            this.tbxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbxPort.Location = new System.Drawing.Point(217, 9);
            this.tbxPort.MaxLength = 6;
            this.tbxPort.Name = "tbxPort";
            this.tbxPort.Size = new System.Drawing.Size(56, 22);
            this.tbxPort.TabIndex = 2;
            this.tbxPort.Text = "4370";
            this.tbxPort.TextChanged += new System.EventHandler(this.tbxPort_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(279, 9);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(104, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Kết nối";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(0, 603);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.lblStatus.Size = new System.Drawing.Size(1284, 25);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "label3";
            // 
            // btnPullData
            // 
            this.btnPullData.Location = new System.Drawing.Point(550, 143);
            this.btnPullData.Name = "btnPullData";
            this.btnPullData.Size = new System.Drawing.Size(80, 23);
            this.btnPullData.TabIndex = 10;
            this.btnPullData.Text = "Lấy dữ liệu";
            this.btnPullData.UseVisualStyleBackColor = true;
            this.btnPullData.Click += new System.EventHandler(this.btnPullData_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.AllowDrop = true;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.pnlHeader.Controls.Add(this.label6);
            this.pnlHeader.Controls.Add(this.lblDeviceInfo);
            this.pnlHeader.Controls.Add(this.label1);
            this.pnlHeader.Controls.Add(this.tbxDeviceIP);
            this.pnlHeader.Controls.Add(this.tbxPort);
            this.pnlHeader.Controls.Add(this.label2);
            this.pnlHeader.Controls.Add(this.btnConnect);
            this.pnlHeader.Location = new System.Drawing.Point(2, 73);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1281, 63);
            this.pnlHeader.TabIndex = 712;
            this.pnlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeader_Paint);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 39);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Mã máy: ";
            // 
            // lblDeviceInfo
            // 
            this.lblDeviceInfo.Location = new System.Drawing.Point(64, 39);
            this.lblDeviceInfo.Name = "lblDeviceInfo";
            this.lblDeviceInfo.Size = new System.Drawing.Size(100, 18);
            this.lblDeviceInfo.TabIndex = 0;
            this.lblDeviceInfo.Text = "lblDeviceInfo";
            this.lblDeviceInfo.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Từ ngày";
            // 
            // dpDateFrom
            // 
            this.dpDateFrom.Checked = true;
            this.dpDateFrom.CustomFormat = "dd/MM/yyyy";
            this.dpDateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpDateFrom.Location = new System.Drawing.Point(67, 144);
            this.dpDateFrom.Name = "dpDateFrom";
            this.dpDateFrom.Size = new System.Drawing.Size(200, 22);
            this.dpDateFrom.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(274, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "đến ngày";
            // 
            // dpDateTo
            // 
            this.dpDateTo.Checked = true;
            this.dpDateTo.CustomFormat = "dd/MM/yyyy";
            this.dpDateTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dpDateTo.Location = new System.Drawing.Point(335, 144);
            this.dpDateTo.Name = "dpDateTo";
            this.dpDateTo.Size = new System.Drawing.Size(200, 22);
            this.dpDateTo.TabIndex = 14;          
            // 
            // btnSyncData
            // 
            this.btnSyncData.Location = new System.Drawing.Point(638, 143);
            this.btnSyncData.Name = "btnSyncData";
            this.btnSyncData.Size = new System.Drawing.Size(75, 23);
            this.btnSyncData.TabIndex = 15;
            this.btnSyncData.Text = "Đồng bộ";
            this.btnSyncData.UseVisualStyleBackColor = true;
            this.btnSyncData.Click += new System.EventHandler(this.btnSyncData_Click);
            // 
            // dgvRecords
            // 
            this.dgvRecords.AllowUserToAddRows = false;
            this.dgvRecords.AllowUserToDeleteRows = false;
            this.dgvRecords.AllowUserToOrderColumns = true;
            this.dgvRecords.AllowUserToResizeColumns = false;
            this.dgvRecords.AllowUserToResizeRows = false;
            this.dgvRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecords.Location = new System.Drawing.Point(0, 0);
            this.dgvRecords.Name = "dgvRecords";
            this.dgvRecords.Size = new System.Drawing.Size(1257, 423);
            this.dgvRecords.TabIndex = 883;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.dgvRecords);
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Location = new System.Drawing.Point(14, 172);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1257, 423);
            this.panel1.TabIndex = 891;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(565, 177);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 884;
            this.pictureBox1.TabStop = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1257, 0);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Algerian", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label12.ForeColor = System.Drawing.Color.Blue;
            this.label12.Location = new System.Drawing.Point(6, 7);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(428, 54);
            this.label12.TabIndex = 5;
            this.label12.Text = "TMT TIMEKEEPER";
            // 
            // panel2
            // 
            this.panel2.AllowDrop = true;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.panel2.Controls.Add(this.lbEmployee);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.panel2.Location = new System.Drawing.Point(0, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1283, 66);
            this.panel2.TabIndex = 712;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeader_Paint);
            // 
            // lbEmployee
            // 
            this.lbEmployee.AutoSize = true;
            this.lbEmployee.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lbEmployee.Location = new System.Drawing.Point(1151, 8);
            this.lbEmployee.Name = "lbEmployee";
            this.lbEmployee.Size = new System.Drawing.Size(80, 17);
            this.lbEmployee.TabIndex = 5;
            this.lbEmployee.Text = "lbEmployee";
            this.lbEmployee.Visible = false;
            // 
            // Master
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 628);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dpDateFrom);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnSyncData);
            this.Controls.Add(this.dpDateTo);
            this.Controls.Add(this.btnPullData);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(615, 425);
            this.Name = "Master";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TMT Timekeeper";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecords)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxDeviceIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxPort;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnPullData;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.DataGridView dgvRecords;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblDeviceInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Button btnSyncData;
        private System.Windows.Forms.DateTimePicker dpDateFrom;
        private System.Windows.Forms.DateTimePicker dpDateTo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lbEmployee;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

