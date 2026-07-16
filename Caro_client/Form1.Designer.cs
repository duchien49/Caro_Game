namespace Caro_client
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelBoard = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblIP = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblRole = new System.Windows.Forms.Label();
            this.lblTurn = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lstHistory = new System.Windows.Forms.ListBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblChat = new System.Windows.Forms.Label();
            this.lstChat = new System.Windows.Forms.ListBox();
            this.txtChat = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblMyIP = new System.Windows.Forms.Label();
            this.lblPlayer = new System.Windows.Forms.Label();
            this.lblOpponent = new System.Windows.Forms.Label();
            this.panelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBoard
            // 
            this.panelBoard.Location = new System.Drawing.Point(60, 60);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(900, 900);
            this.panelBoard.TabIndex = 0;
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.lblStatus);
            this.panelRight.Controls.Add(this.lblIP);
            this.panelRight.Controls.Add(this.txtIP);
            this.panelRight.Controls.Add(this.lblPort);
            this.panelRight.Controls.Add(this.txtPort);
            this.panelRight.Controls.Add(this.lblName);
            this.panelRight.Controls.Add(this.txtName);
            this.panelRight.Controls.Add(this.btnConnect);
            this.panelRight.Controls.Add(this.lblRole);
            this.panelRight.Controls.Add(this.lblTurn);
            this.panelRight.Controls.Add(this.lblTime);
            this.panelRight.Controls.Add(this.lstHistory);
            this.panelRight.Controls.Add(this.btnReset);
            this.panelRight.Controls.Add(this.btnExit);
            this.panelRight.Controls.Add(this.lblChat);
            this.panelRight.Controls.Add(this.lstChat);
            this.panelRight.Controls.Add(this.txtChat);
            this.panelRight.Controls.Add(this.btnSend);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(1206, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(700, 1137);
            this.panelRight.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(40, 20);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(269, 32);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Status: Chưa kết nối";
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(40, 65);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(40, 32);
            this.lblIP.TabIndex = 1;
            this.lblIP.Text = "IP";
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(160, 62);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(200, 38);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "127.0.0.1";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(40, 115);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(66, 32);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(160, 112);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(200, 38);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "8888";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(40, 165);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(89, 32);
            this.lblName.TabIndex = 3;
            this.lblName.Text = "Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(160, 162);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 38);
            this.txtName.TabIndex = 3;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(390, 60);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(250, 150);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblRole.Location = new System.Drawing.Point(40, 225);
            this.lblRole.Name = "lblRole";
            this.lblRole.Size = new System.Drawing.Size(179, 50);
            this.lblRole.TabIndex = 5;
            this.lblRole.Text = "Bạn là: ...";
            // 
            // lblTurn
            // 
            this.lblTurn.AutoSize = true;
            this.lblTurn.Location = new System.Drawing.Point(40, 275);
            this.lblTurn.Name = "lblTurn";
            this.lblTurn.Size = new System.Drawing.Size(111, 32);
            this.lblTurn.TabIndex = 6;
            this.lblTurn.Text = "Turn: ...";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(40, 320);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(193, 32);
            this.lblTime.TabIndex = 7;
            this.lblTime.Text = "Thời gian: 30s";
            // 
            // lstHistory
            // 
            this.lstHistory.FormattingEnabled = true;
            this.lstHistory.ItemHeight = 31;
            this.lstHistory.Location = new System.Drawing.Point(40, 401);
            this.lstHistory.Name = "lstHistory";
            this.lstHistory.Size = new System.Drawing.Size(600, 221);
            this.lstHistory.TabIndex = 5;
            this.lstHistory.SelectedIndexChanged += new System.EventHandler(this.lstHistory_SelectedIndexChanged);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(390, 225);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(250, 80);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Đầu hàng";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(390, 315);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(250, 80);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblChat
            // 
            this.lblChat.AutoSize = true;
            this.lblChat.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblChat.Location = new System.Drawing.Point(41, 635);
            this.lblChat.Name = "lblChat";
            this.lblChat.Size = new System.Drawing.Size(92, 46);
            this.lblChat.TabIndex = 8;
            this.lblChat.Text = "Chat";
            // 
            // lstChat
            // 
            this.lstChat.FormattingEnabled = true;
            this.lstChat.ItemHeight = 31;
            this.lstChat.Location = new System.Drawing.Point(40, 696);
            this.lstChat.Name = "lstChat";
            this.lstChat.ScrollAlwaysVisible = true;
            this.lstChat.Size = new System.Drawing.Size(600, 252);
            this.lstChat.TabIndex = 8;
            // 
            // txtChat
            // 
            this.txtChat.Location = new System.Drawing.Point(46, 982);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(490, 38);
            this.txtChat.TabIndex = 9;
            this.txtChat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtChat_KeyPress);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(542, 974);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(100, 52);
            this.btnSend.TabIndex = 10;
            this.btnSend.Text = "Gửi";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblMyIP
            // 
            this.lblMyIP.AutoSize = true;
            this.lblMyIP.Location = new System.Drawing.Point(60, 20);
            this.lblMyIP.Name = "lblMyIP";
            this.lblMyIP.Size = new System.Drawing.Size(139, 32);
            this.lblMyIP.TabIndex = 0;
            this.lblMyIP.Text = "IP máy: ...";
            // 
            // lblPlayer
            // 
            this.lblPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPlayer.AutoSize = true;
            this.lblPlayer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPlayer.ForeColor = System.Drawing.Color.Crimson;
            this.lblPlayer.Location = new System.Drawing.Point(52, 1082);
            this.lblPlayer.Name = "lblPlayer";
            this.lblPlayer.Size = new System.Drawing.Size(126, 46);
            this.lblPlayer.TabIndex = 1;
            this.lblPlayer.Text = "Bạn: ...";
            this.lblPlayer.Click += new System.EventHandler(this.lblPlayer_Click);
            // 
            // lblOpponent
            // 
            this.lblOpponent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOpponent.AutoSize = true;
            this.lblOpponent.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOpponent.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblOpponent.Location = new System.Drawing.Point(293, 1082);
            this.lblOpponent.Name = "lblOpponent";
            this.lblOpponent.Size = new System.Drawing.Size(184, 46);
            this.lblOpponent.TabIndex = 2;
            this.lblOpponent.Text = "Đối thủ: ...";
            this.lblOpponent.Click += new System.EventHandler(this.lblOpponent_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1906, 1137);
            this.Controls.Add(this.lblMyIP);
            this.Controls.Add(this.lblPlayer);
            this.Controls.Add(this.lblOpponent);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelBoard);
            this.Name = "Form1";
            this.Text = "Caro_Client_UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        // ─── Field declarations ──────────────────────────────────────
        private System.Windows.Forms.Panel panelBoard;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.Label lblTurn;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.ListBox lstHistory;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblMyIP;
        private System.Windows.Forms.Label lblPlayer;
        private System.Windows.Forms.Label lblOpponent;

        // Chat UDP
        private System.Windows.Forms.Label lblChat;
        private System.Windows.Forms.ListBox lstChat;
        private System.Windows.Forms.TextBox txtChat;
        private System.Windows.Forms.Button btnSend;
    }
}