using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static readonly List<Room> Rooms = new List<Room>();
    static readonly object RoomsLock = new object();

    static UdpClient udpServer;
    static int tcpPort;

    static void Main()
    {
        Console.Write("Nhap port TCP: ");
        tcpPort = int.Parse(Console.ReadLine());
        int udpPort = tcpPort + 1;

        udpServer = new UdpClient(udpPort);
        Thread udpThread = new Thread(UdpChatLoop) { IsBackground = true };
        udpThread.Start();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[TCP] Game server lang nghe tren port {tcpPort}");
        Console.WriteLine($"[UDP] Chat server lang nghe tren port {udpPort}");
        Console.ResetColor();
        Console.WriteLine("Dang cho nguoi choi ket noi...\n");

        TcpListener listener = new TcpListener(IPAddress.Any, tcpPort);
        listener.Start();

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Room room;
            lock (RoomsLock)
            {
                room = FindOrCreateRoom();
            }
            AssignToRoom(room, client);
            Thread t = new Thread(() => ClientLoop(client, room)) { IsBackground = true };
            t.Start();
        }
    }


    static Room FindOrCreateRoom()
    {
        foreach (var r in Rooms)
            if (!r.IsFull) return r;

        var newRoom = new Room(Rooms.Count);
        Rooms.Add(newRoom);
        return newRoom;
    }

    static void AssignToRoom(Room room, TcpClient client)
    {
        lock (room)
        {
            if (room.Player1 == null)
            {
                room.Player1 = client;
                // Gửi: ROLE|symbol|roomId|udpPort
                TcpSend(client, $"ROLE|X|{room.Id}|{tcpPort + 1}");
                Log(room.Id, ConsoleColor.Yellow, "Player X (nguoi choi 1) da ket noi.");
            }
            else
            {
                room.Player2 = client;
                TcpSend(client, $"ROLE|O|{room.Id}|{tcpPort + 1}");
                Log(room.Id, ConsoleColor.Green, "Player O (nguoi choi 2) da ket noi. Tro choi bat dau!");
            }
        }
    }


    static void ClientLoop(TcpClient client, Room room)
    {
        NetworkStream stream = client.GetStream();
        byte[] buf = new byte[2048];
        bool isPlayer1;

        lock (room) { isPlayer1 = (client == room.Player1); }

        try
        {
            while (true)
            {
                int n = stream.Read(buf, 0, buf.Length);
                if (n == 0) break;

                string raw = Encoding.UTF8.GetString(buf, 0, n);
                foreach (string line in raw.Split('\n'))
                {
                    string msg = line.Trim();
                    if (msg.Length == 0) continue;
                    ProcessClientMessage(room, client, isPlayer1, msg);
                }
            }
        }
        catch { /* Client ngắt kết nối đột ngột */ }
        finally
        {
            HandleDisconnect(room, client, isPlayer1);
        }
    }

    static void ProcessClientMessage(Room room, TcpClient client, bool isPlayer1, string msg)
    {
        string[] parts = msg.Split('|');
        Log(room.Id, ConsoleColor.White, $"{(isPlayer1 ? "X" : "O")} → {msg}");

        switch (parts[0])
        {
            case "NAME":
                if (parts.Length < 2) return;
                lock (room)
                {
                    if (isPlayer1) room.P1Name = parts[1];
                    else room.P2Name = parts[1];
                }
                Relay(room, client, msg);
                break;

            case "MOVE":
                if (parts.Length < 3) return;
                HandleMove(room, client, isPlayer1, int.Parse(parts[1]), int.Parse(parts[2]));
                break;

            case "RESIGN":
                HandleResign(room, client, isPlayer1);
                break;
        }
    }


    static void HandleMove(Room room, TcpClient client, bool isPlayer1, int x, int y)
    {
        lock (room)
        {
            if (room.IsXTurn != isPlayer1)
            {
                TcpSend(client, "INVALID_MOVE|Chua den luot ban");
                return;
            }

            if (x < 0 || x >= 15 || y < 0 || y >= 15 || room.Board[x, y] != 0)
            {
                TcpSend(client, "INVALID_MOVE|O da duoc danh hoac khong hop le");
                return;
            }

            int val = isPlayer1 ? 1 : 2;
            room.Board[x, y] = val;
            room.IsXTurn = !room.IsXTurn;

            Relay(room, client, $"MOVE|{x}|{y}");

            if (CheckWin(room.Board, x, y, val))
            {
                string winner = isPlayer1 ? room.P1Name : room.P2Name;
                Log(room.Id, ConsoleColor.Magenta, $"{winner} THANG!");

                TcpSend(client, "WIN");
                TcpSend(Opponent(room, client), "LOSE");
                room.Reset();
            }
        }
    }

    static void HandleResign(Room room, TcpClient client, bool isPlayer1)
    {
        Log(room.Id, ConsoleColor.DarkYellow, $"{(isPlayer1 ? "X" : "O")} dau hang."); 
        TcpSend(client, "LOSE");
        TcpSend(Opponent(room, client), "WIN");
        lock (room) { room.Reset(); }
    }

    static void HandleDisconnect(Room room, TcpClient client, bool isPlayer1)
    {
        Log(room.Id, ConsoleColor.Red, $"{(isPlayer1 ? "X" : "O")} ngat ket noi.");
        TcpSend(Opponent(room, client), "OPPONENT_LEFT");
        lock (room)
        {
            if (isPlayer1) { room.Player1 = null; room.P1UdpEP = null; room.Reset(); }
            else { room.Player2 = null; room.P2UdpEP = null; room.Reset(); }
        }
    }


    static bool CheckWin(int[,] b, int x, int y, int p)
        => Dir(b, x, y, p, 1, 0)
        || Dir(b, x, y, p, 0, 1)
        || Dir(b, x, y, p, 1, 1)
        || Dir(b, x, y, p, 1, -1);

    static bool Dir(int[,] b, int x, int y, int p, int dx, int dy)
    {
        int count = 1;

        for (int d = 1; d < 5; d++)
        {
            int nx = x + dx * d, ny = y + dy * d;
            if (nx < 0 || nx >= 15 || ny < 0 || ny >= 15 || b[nx, ny] != p) break;
            count++;
        }

        for (int d = 1; d < 5; d++)
        {
            int nx = x - dx * d, ny = y - dy * d;
            if (nx < 0 || nx >= 15 || ny < 0 || ny >= 15 || b[nx, ny] != p) break;
            count++;
        }

        return count >= 5;
    }


    static void UdpChatLoop()
    {
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = udpServer.Receive(ref ep);
                string msg = Encoding.UTF8.GetString(data);
                string[] parts = msg.Split('|');

                if (parts[0] == "UDPREG" && parts.Length >= 2)
                {
                    int roomId = int.Parse(parts[1]);
                    if (roomId >= 0 && roomId < Rooms.Count)
                        RegisterUdpEndpoint(Rooms[roomId], new IPEndPoint(ep.Address, ep.Port));
                }
                else if (parts[0] == "CHAT" && parts.Length >= 4)
                {
                    int roomId = int.Parse(parts[1]);
                    if (roomId >= 0 && roomId < Rooms.Count)
                        RelayUdpChat(Rooms[roomId], ep, parts[2], parts[3]);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[UDP Loi] " + ex.Message);
                Console.ResetColor();
            }
        }
    }

    static void RegisterUdpEndpoint(Room room, IPEndPoint ep)
    {
        lock (room)
        {
            if (room.P1UdpEP == null)
            {
                room.P1UdpEP = ep;
                Log(room.Id, ConsoleColor.Cyan, $"[UDP] P1 dang ky: {ep}");
            }
            else if (room.P2UdpEP == null && !room.P1UdpEP.Equals(ep))
            {
                room.P2UdpEP = ep;
                Log(room.Id, ConsoleColor.Cyan, $"[UDP] P2 dang ky: {ep}");
            }
        }
    }

    static void RelayUdpChat(Room room, IPEndPoint sender, string name, string message)
    {
        string chatMsg = $"CHAT|{name}|{message}";
        byte[] data = Encoding.UTF8.GetBytes(chatMsg);

        IPEndPoint target = null;
        lock (room)
        {
            if (room.P1UdpEP != null && !room.P1UdpEP.Equals(sender)) target = room.P1UdpEP;
            else if (room.P2UdpEP != null && !room.P2UdpEP.Equals(sender)) target = room.P2UdpEP;
        }

        if (target != null)
            udpServer.Send(data, data.Length, target);
    }


    static TcpClient Opponent(Room room, TcpClient me)
        => (me == room.Player1) ? room.Player2 : room.Player1;

    static void Relay(Room room, TcpClient sender, string msg)
        => TcpSend(Opponent(room, sender), msg);

    static void TcpSend(TcpClient client, string msg)
    {
        if (client == null) return;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(msg + "\n");
            lock (client)
            {
                client.GetStream().Write(data, 0, data.Length);
            }
        }
        catch { }
    }

    static void Log(int roomId, ConsoleColor color, string msg)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[Room {roomId}] {msg}");
        Console.ResetColor();
    }
}

class Room
{
    public int Id;
    public TcpClient Player1, Player2;
    public string P1Name = "Nguoi choi 1";
    public string P2Name = "Nguoi choi 2";
    public IPEndPoint P1UdpEP, P2UdpEP;
    public int[,] Board = new int[15, 15];
    public bool IsXTurn = true; 

    public Room(int id) { Id = id; }

    public bool IsFull => Player1 != null && Player2 != null;

    public void Reset()
    {
        Board = new int[15, 15];
        IsXTurn = true;
    }
}