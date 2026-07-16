using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace Caro_client
{
    public partial class Form1 : Form
    {
        //TCP
        TcpClient tcpClient;
        NetworkStream tcpStream;
        Thread tcpReceiveThread;
        readonly StringBuilder tcpBuffer = new StringBuilder(); 

        //UDP (Chat)
        UdpClient udpClient;
        Thread udpReceiveThread;
        string serverIP = "";
        int udpServerPort = 0;
        int roomId = -1;

        readonly Button[,] board = new Button[15, 15];
        readonly int[,] gameBoard = new int[15, 15];

        bool myTurn = false;
        string myName = "";
        string opponentName = "";
        string mySymbol = ""; 

        System.Windows.Forms.Timer turnTimer;
        int timeLeft = 30;

        public Form1()
        {
            InitializeComponent();
            CreateBoard();
            SetupStyles();
            turnTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            turnTimer.Tick += TurnTimer_Tick;

            // Hiển thị IP máy cục bộ
            string hostName = Dns.GetHostName();
            foreach (var ip in Dns.GetHostAddresses(hostName))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    lblMyIP.Text = "IP may: " + ip;
                    break;
                }
            }
        }

        void SetupStyles()
        {
            this.BackColor = Color.FromArgb(240, 240, 245);
            this.Font = new Font("Segoe UI", 10);

            panelRight.BackColor = Color.FromArgb(248, 248, 252);
            panelRight.Padding = new Padding(10);

            btnConnect.BackColor = Color.MediumSeaGreen;
            btnConnect.ForeColor = Color.White;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Cursor = Cursors.Hand;

            btnExit.BackColor = Color.IndianRed;
            btnExit.ForeColor = Color.White;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Cursor = Cursors.Hand;

            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Cursor = Cursors.Hand;

            btnSend.BackColor = Color.SteelBlue;
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Cursor = Cursors.Hand;

            lblStatus.ForeColor = Color.OrangeRed;
            lblTurn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        }
        void CreateBoard()
        {
            const int CellSize = 36;

            panelBoard.Width = 15 * CellSize;
            panelBoard.Height = 15 * CellSize;
            panelBoard.BorderStyle = BorderStyle.FixedSingle;
            panelBoard.BackColor = Color.FromArgb(245, 230, 195); // Nền gỗ nhạt

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    var btn = new Button
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Location = new Point(i * CellSize, j * CellSize),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.FromArgb(245, 230, 195),
                        Font = new Font("Arial", 11, FontStyle.Bold),
                        Tag = $"{i},{j}",
                        Cursor = Cursors.Hand
                    };
                    btn.FlatAppearance.BorderColor = Color.FromArgb(139, 90, 43);
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += Btn_Click;

                    panelBoard.Controls.Add(btn);
                    board[i, j] = btn;
                }
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (tcpClient == null || !tcpClient.Connected)
            {
                MessageBox.Show("Chua ket noi server!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (btn.Text != "") return;
            if (!myTurn)
            {
                MessageBox.Show("Chua den luot cua ban!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string[] pos = btn.Tag.ToString().Split(',');
            int x = int.Parse(pos[0]);
            int y = int.Parse(pos[1]);
            PlacePiece(btn, mySymbol, x, y, isMyPiece: true);

            TcpSend($"MOVE|{x}|{y}");
            turnTimer.Stop();
            myTurn = false;
            UpdateTurnUI();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                MessageBox.Show("Da ket noi roi!"); return;
            }

            myName = txtName.Text.Trim();
            if (myName == "")
            {
                MessageBox.Show("Vui long nhap ten truoc!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            serverIP = txtIP.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out int port))
            {
                MessageBox.Show("Port khong hop le!"); return;
            }

            try
            {
                tcpClient = new TcpClient(serverIP, port);
                tcpStream = tcpClient.GetStream();

                TcpSend($"NAME|{myName}");

                lblStatus.Text = "Status: Da ket noi";
                lblStatus.ForeColor = Color.MediumSeaGreen;
                lblPlayer.Text = "Ban: " + myName;
                btnConnect.Enabled = false;

                AppendChat("[He thong]", "Da ket noi server. Dang cho doi thu...");

                tcpReceiveThread = new Thread(ReceiveTcpLoop) { IsBackground = true };
                tcpReceiveThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Loi ket noi: " + ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void ReceiveTcpLoop()
        {
            byte[] buf = new byte[4096];

            while (true)
            {
                try
                {
                    int n = tcpStream.Read(buf, 0, buf.Length);
                    if (n == 0) break;

                    lock (tcpBuffer)
                    {
                        tcpBuffer.Append(Encoding.UTF8.GetString(buf, 0, n));
                    }

                    while (true)
                    {
                        string data;
                        lock (tcpBuffer) { data = tcpBuffer.ToString(); }

                        int idx = data.IndexOf('\n');
                        if (idx < 0) break;

                        string msg = data.Substring(0, idx).Trim();
                        lock (tcpBuffer) { tcpBuffer.Remove(0, idx + 1); }

                        if (msg.Length > 0)
                        {
                            this.Invoke((MethodInvoker)(() => HandleTcpMessage(msg)));
                        }
                    }
                }
                catch
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        lblStatus.Text = "Status: Mat ket noi";
                        lblStatus.ForeColor = Color.OrangeRed;
                        btnConnect.Enabled = true;
                        turnTimer.Stop();
                        AppendChat("[He thong]", "Mat ket noi den server.");
                    });
                    break;
                }
            }
        }
        void HandleTcpMessage(string msg)
        {
            string[] parts = msg.Split('|');
            if (parts.Length == 0) return;

            switch (parts[0])
            {
                case "NAME":
                    opponentName = parts.Length > 1 ? parts[1] : "An danh";
                    lblOpponent.Text = "Doi thu: " + opponentName;
                    AppendChat("[He thong]", $"{opponentName} da vao phong. Tro choi sap bat dau!");
                    break;

                case "ROLE":
                    mySymbol = parts.Length > 1 ? parts[1] : "X";
                    lblRole.Text = $"Ban la: {mySymbol}";

                    if (parts.Length >= 4)
                    {
                        roomId = int.Parse(parts[2]);
                        udpServerPort = int.Parse(parts[3]);
                        SetupUdp(); 
                    }

                    myTurn = (mySymbol == "X");
                    UpdateTurnUI();

                    if (myTurn) StartTurnTimer();
                    break;

                case "MOVE":
                    if (parts.Length < 3) break;
                    int rx = int.Parse(parts[1]), ry = int.Parse(parts[2]);
                    string enemy = (mySymbol == "X") ? "O" : "X";
                    PlacePiece(board[rx, ry], enemy, rx, ry, isMyPiece: false);
                    lstHistory.Items.Add($"[{opponentName}] {enemy}: ({rx},{ry})");
                    lstHistory.TopIndex = lstHistory.Items.Count - 1;

                    myTurn = true;
                    UpdateTurnUI();
                    StartTurnTimer();
                    break;

                case "WIN":
                    turnTimer.Stop();
                    MessageBox.Show($" Chuc mung {myName}!\nban da thang!",
                        "Ket qua", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetGame();
                    break;

                case "LOSE":
                    turnTimer.Stop();
                    MessageBox.Show($" Ban da thua!\n{opponentName} gianh chien thang.",
                        "Ket qua", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ResetGame();
                    break;

                case "OPPONENT_LEFT":
                    turnTimer.Stop();
                    MessageBox.Show("Doi thu da roi khoi phong.", "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    AppendChat("[He thong]", $"{opponentName} da ngat ket noi.");
                    lblOpponent.Text = "Doi thu: (offline)";
                    ResetGame();
                    break;

                case "INVALID_MOVE":
                    string reason = parts.Length > 1 ? parts[1] : "Khong xac dinh";
                    MessageBox.Show("Nuoc di khong hop le: " + reason, "Canh bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    myTurn = true;
                    UpdateTurnUI();
                    StartTurnTimer();
                    break;
            }
        }

        void SetupUdp()
        {
            try
            {
                udpClient = new UdpClient(0);
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), udpServerPort);

                byte[] reg = Encoding.UTF8.GetBytes($"UDPREG|{roomId}");
                udpClient.Send(reg, reg.Length, serverEP);

                udpReceiveThread = new Thread(ReceiveUdpLoop) { IsBackground = true };
                udpReceiveThread.Start();

                AppendChat("[He thong]", $"Chat UDP san sang (port {udpServerPort}).");
            }
            catch (Exception ex)
            {
                AppendChat("[He thong]", "Khong the khoi dong UDP chat: " + ex.Message);
            }
        }

        void ReceiveUdpLoop()
        {
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref remoteEP);
                    string msg = Encoding.UTF8.GetString(data);
                    string[] parts = msg.Split('|');

                    if (parts[0] == "CHAT" && parts.Length >= 3)
                    {
                        this.Invoke((MethodInvoker)(() =>
                            AppendChat(parts[1], parts[2])
                        ));
                    }
                }
                catch { break; }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            SendChat();
        }
        private void txtChat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendChat();
                e.Handled = true;
            }
        }

        void SendChat()
        {
            string text = txtChat.Text.Trim();
            if (text == "") return;

            if (udpClient == null || roomId < 0)
            {
                AppendChat("[He thong]", "Chua ket noi, khong the gui chat.");
                return;
            }

            try
            {
                string msg = $"CHAT|{roomId}|{myName}|{text}";
                byte[] data = Encoding.UTF8.GetBytes(msg);
                IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(serverIP), udpServerPort);
                udpClient.Send(data, data.Length, serverEP);

                AppendChat(myName, text);
                txtChat.Clear();
                txtChat.Focus();
            }
            catch (Exception ex)
            {
                AppendChat("[He thong]", "Loi gui chat: " + ex.Message);
            }
        }

        void AppendChat(string sender, string message)
        {
            string time = DateTime.Now.ToString("HH:mm");
            lstChat.Items.Add($"[{time}] {sender}: {message}");
            lstChat.TopIndex = lstChat.Items.Count - 1;
        }

        void PlacePiece(Button btn, string symbol, int x, int y, bool isMyPiece)
        {
            btn.Text = symbol;
            btn.ForeColor = (symbol == "X") ? Color.Crimson : Color.RoyalBlue;
            btn.BackColor = isMyPiece ? Color.FromArgb(255, 255, 200) : Color.FromArgb(200, 230, 255);
            gameBoard[x, y] = (symbol == "X") ? 1 : 2;

            lstHistory.Items.Add($"{(isMyPiece ? "[Ban]" : $"[{opponentName}]")} {symbol}: ({x},{y})");
            lstHistory.TopIndex = lstHistory.Items.Count - 1;
        }

        void UpdateTurnUI()
        {
            if (myTurn)
            {
                lblTurn.Text = "Turn: ► Luot ban!";
                lblTurn.ForeColor = Color.MediumSeaGreen;
            }
            else
            {
                lblTurn.Text = "Turn: Cho doi thu...";
                lblTurn.ForeColor = Color.Gray;
            }
        }

        void StartTurnTimer()
        {
            timeLeft = 30;
            lblTime.Text = "Thoi gian: 30s";
            lblTime.ForeColor = Color.Black;
            turnTimer.Start();
        }

        void ResetGame()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    board[i, j].Text = "";
                    board[i, j].BackColor = Color.FromArgb(245, 230, 195);
                    gameBoard[i, j] = 0;
                }
            }

            lstHistory.Items.Clear();

            myTurn = (mySymbol == "X");
            UpdateTurnUI();

            turnTimer.Stop();
            if (myTurn) StartTurnTimer();
        }


        void TurnTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            lblTime.Text = $"Thoi gian: {timeLeft}s";
            lblTime.ForeColor = (timeLeft <= 5) ? Color.Red : Color.Black;

            if (timeLeft <= 0)
            {
                turnTimer.Stop();
                TcpSend("RESIGN");
                MessageBox.Show("Het thoi gian! Ban thua van nay.", "Ket qua",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetGame();
            }
        }


        void TcpSend(string msg)
        {
            if (tcpClient == null || !tcpClient.Connected) return;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                tcpStream.Write(data, 0, data.Length);
            }
            catch { }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                try { TcpSend("RESIGN"); } catch { }
            }
            Application.Exit();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == MessageBox.Show(
                "Ban muon dau hang va Reset game?", "Xac nhan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                TcpSend("RESIGN");
            }
        }

        private void lstHistory_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }

        private void lblOpponent_Click(object sender, EventArgs e)
        {

        }

        private void lblPlayer_Click(object sender, EventArgs e)
        {

        }
    }
}