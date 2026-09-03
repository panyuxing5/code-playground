using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;

namespace VoxelCraft.Network
{
    public class NetworkManager
    {
        // 网络状态
        public bool IsServer { get; private set; }
        public bool IsClient { get; private set; }
        public bool IsConnected { get; private set; }
        public string ServerAddress { get; private set; }
        public int ServerPort { get; private set; }

        // 服务器
        private TcpListener serverListener;
        private readonly List<ClientConnection> clients;
        private Thread serverThread;
        private bool serverRunning;

        // 客户端
        private TcpClient client;
        private NetworkStream clientStream;
        private Thread clientThread;
        private bool clientRunning;

        // 数据包处理
        private readonly Queue<NetworkPacket> incomingPackets;
        private readonly Queue<NetworkPacket> outgoingPackets;
        private readonly object packetLock = new object();

        // 事件
        public event Action<ClientConnection> OnClientConnected;
        public event Action<ClientConnection> OnClientDisconnected;
        public event Action<NetworkPacket> OnPacketReceived;
        public event Action<string> OnConnectionFailed;
        public event Action OnConnected;
        public event Action OnDisconnected;

        // 统计
        public int BytesSent { get; private set; }
        public int BytesReceived { get; private set; }
        public int PacketsSent { get; private set; }
        public int PacketsReceived { get; private set; }
        public float Ping { get; private set; }

        // 配置
        public int MaxPlayers { get; set; } = 20;
        public int ProtocolVersion { get; } = 1;
        public string ServerName { get; set; } = "VoxelCraft Server";

        public NetworkManager()
        {
            clients = new List<ClientConnection>();
            incomingPackets = new Queue<NetworkPacket>();
            outgoingPackets = new Queue<NetworkPacket>();
        }

        // ========================================
        // 服务器
        // ========================================
        public void StartServer(int port = 25565)
        {
            if (IsServer || IsClient)
            {
                Console.WriteLine("[NetworkManager] 已经在运行中");
                return;
            }

            try
            {
                serverListener = new TcpListener(IPAddress.Any, port);
                serverListener.Start();
                IsServer = true;
                serverRunning = true;
                ServerPort = port;

                serverThread = new Thread(ServerLoop)
                {
                    IsBackground = true,
                    Name = "ServerThread"
                };
                serverThread.Start();

                Console.WriteLine($"[NetworkManager] 服务器已启动，端口: {port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NetworkManager] 服务器启动失败: {ex.Message}");
            }
        }

        private void ServerLoop()
        {
            while (serverRunning)
            {
                try
                {
                    if (serverListener.Pending())
                    {
                        TcpClient tcpClient = serverListener.AcceptTcpClient();

                        if (clients.Count >= MaxPlayers)
                        {
                            tcpClient.Close();
                            continue;
                        }

                        ClientConnection connection = new ClientConnection(tcpClient, this);
                        clients.Add(connection);
                        connection.Start();

                        OnClientConnected?.Invoke(connection);
                        Console.WriteLine($"[NetworkManager] 客户端连接: {connection.EndPoint}");
                    }

                    // 处理传出数据包
                    SendOutgoingPackets();

                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NetworkManager] 服务器错误: {ex.Message}");
                }
            }
        }

        public void StopServer()
        {
            serverRunning = false;
            serverListener?.Stop();

            foreach (ClientConnection client in clients)
            {
                client.Disconnect();
            }
            clients.Clear();

            IsServer = false;
            Console.WriteLine("[NetworkManager] 服务器已停止");
        }

        // ========================================
        // 客户端
        // ========================================
        public void ConnectToServer(string address, int port = 25565)
        {
            if (IsServer || IsClient)
            {
                Console.WriteLine("[NetworkManager] 已经在运行中");
                return;
            }

            try
            {
                client = new TcpClient();
                client.Connect(address, port);
                clientStream = client.GetStream();

                IsClient = true;
                IsConnected = true;
                clientRunning = true;
                ServerAddress = address;
                ServerPort = port;

                clientThread = new Thread(ClientLoop)
                {
                    IsBackground = true,
                    Name = "ClientThread"
                };
                clientThread.Start();

                OnConnected?.Invoke();
                Console.WriteLine($"[NetworkManager] 已连接到服务器: {address}:{port}");
            }
            catch (Exception ex)
            {
                OnConnectionFailed?.Invoke(ex.Message);
                Console.WriteLine($"[NetworkManager] 连接失败: {ex.Message}");
            }
        }

        private void ClientLoop()
        {
            byte[] buffer = new byte[8192];

            while (clientRunning && client.Connected)
            {
                try
                {
                    if (clientStream.DataAvailable)
                    {
                        int bytesRead = clientStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            BytesReceived += bytesRead;
                            PacketsReceived++;

                            NetworkPacket packet = NetworkPacket.Deserialize(buffer, bytesRead);
                            if (packet != null)
                            {
                                lock (packetLock)
                                {
                                    incomingPackets.Enqueue(packet);
                                }
                                OnPacketReceived?.Invoke(packet);
                            }
                        }
                    }

                    // 发送传出数据包
                    SendClientPackets();

                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NetworkManager] 客户端错误: {ex.Message}");
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            clientRunning = false;
            client?.Close();
            clientStream?.Close();

            IsClient = false;
            IsConnected = false;

            OnDisconnected?.Invoke();
            Console.WriteLine("[NetworkManager] 已断开连接");
        }

        // ========================================
        // 数据包处理
        // ========================================
        public void SendPacket(NetworkPacket packet)
        {
            lock (packetLock)
            {
                outgoingPackets.Enqueue(packet);
            }
        }

        public NetworkPacket ReceivePacket()
        {
            lock (packetLock)
            {
                if (incomingPackets.Count > 0)
                {
                    return incomingPackets.Dequeue();
                }
            }
            return null;
        }

        public List<NetworkPacket> ReceiveAllPackets()
        {
            List<NetworkPacket> packets = new List<NetworkPacket>();
            lock (packetLock)
            {
                while (incomingPackets.Count > 0)
                {
                    packets.Add(incomingPackets.Dequeue());
                }
            }
            return packets;
        }

        private void SendOutgoingPackets()
        {
            lock (packetLock)
            {
                while (outgoingPackets.Count > 0)
                {
                    NetworkPacket packet = outgoingPackets.Dequeue();
                    byte[] data = packet.Serialize();

                    foreach (ClientConnection client in clients)
                    {
                        client.Send(data);
                        BytesSent += data.Length;
                        PacketsSent++;
                    }
                }
            }
        }

        private void SendClientPackets()
        {
            lock (packetLock)
            {
                while (outgoingPackets.Count > 0)
                {
                    NetworkPacket packet = outgoingPackets.Dequeue();
                    byte[] data = packet.Serialize();

                    clientStream?.Write(data, 0, data.Length);
                    BytesSent += data.Length;
                    PacketsSent++;
                }
            }
        }

        public void SendToClient(ClientConnection client, NetworkPacket packet)
        {
            byte[] data = packet.Serialize();
            client.Send(data);
            BytesSent += data.Length;
            PacketsSent++;
        }

        public void BroadcastPacket(NetworkPacket packet)
        {
            byte[] data = packet.Serialize();
            foreach (ClientConnection client in clients)
            {
                client.Send(data);
                BytesSent += data.Length;
                PacketsSent++;
            }
        }

        // ========================================
        // 游戏数据包
        // ========================================
        public void SendPlayerPosition(Vector3 position, float yaw, float pitch)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.PlayerPosition,
                Data = new Dictionary<string, object>
                {
                    { "x", position.X },
                    { "y", position.Y },
                    { "z", position.Z },
                    { "yaw", yaw },
                    { "pitch", pitch }
                }
            };
            SendPacket(packet);
        }

        public void SendBlockChange(int x, int y, int z, ushort blockId)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.BlockChange,
                Data = new Dictionary<string, object>
                {
                    { "x", x },
                    { "y", y },
                    { "z", z },
                    { "blockId", blockId }
                }
            };
            SendPacket(packet);
        }

        public void SendChatMessage(string message)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.ChatMessage,
                Data = new Dictionary<string, object>
                {
                    { "message", message }
                }
            };
            SendPacket(packet);
        }

        public void SendDisconnect(string reason)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.Disconnect,
                Data = new Dictionary<string, object>
                {
                    { "reason", reason }
                }
            };
            SendPacket(packet);
        }

        public void SendChunkData(Chunk chunk)
        {
            // 简化：发送区块数据
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.ChunkData,
                Data = new Dictionary<string, object>
                {
                    { "chunkX", chunk.X },
                    { "chunkZ", chunk.Z }
                }
            };
            SendPacket(packet);
        }

        public void SendEntitySpawn(Entity entity)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.EntitySpawn,
                Data = new Dictionary<string, object>
                {
                    { "entityId", entity.Id },
                    { "entityType", (int)entity.Type },
                    { "x", entity.Position.X },
                    { "y", entity.Position.Y },
                    { "z", entity.Position.Z }
                }
            };
            SendPacket(packet);
        }

        public void SendEntityPosition(Entity entity)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.EntityPosition,
                Data = new Dictionary<string, object>
                {
                    { "entityId", entity.Id },
                    { "x", entity.Position.X },
                    { "y", entity.Position.Y },
                    { "z", entity.Position.Z },
                    { "yaw", entity.Yaw },
                    { "pitch", entity.Pitch }
                }
            };
            SendPacket(packet);
        }

        public void SendEntityDestroy(int entityId)
        {
            NetworkPacket packet = new NetworkPacket
            {
                PacketId = (int)PacketType.EntityDestroy,
                Data = new Dictionary<string, object>
                {
                    { "entityId", entityId }
                }
            };
            SendPacket(packet);
        }

        // ========================================
        // 工具方法
        // ========================================
        public List<ClientConnection> GetClients()
        {
            return new List<ClientConnection>(clients);
        }

        public int GetPlayerCount()
        {
            return clients.Count;
        }

        public void Update()
        {
            // 处理传入数据包
            // 实际游戏逻辑中处理
        }

        public void Dispose()
        {
            if (IsServer) StopServer();
            if (IsClient) Disconnect();

            lock (packetLock)
            {
                incomingPackets.Clear();
                outgoingPackets.Clear();
            }

            Console.WriteLine("[NetworkManager] 网络管理器已释放");
        }
    }

    // ========================================
    // 客户端连接
    // ========================================
    public class ClientConnection
    {
        private readonly TcpClient tcpClient;
        private readonly NetworkManager networkManager;
        private NetworkStream stream;
        private Thread receiveThread;
        private bool isRunning;

        public string EndPoint => tcpClient.Client.RemoteEndPoint?.ToString();
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public Vector3 Position { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public int Ping { get; set; }
        public DateTime LastPacketTime { get; set; }

        public ClientConnection(TcpClient tcpClient, NetworkManager networkManager)
        {
            this.tcpClient = tcpClient;
            this.networkManager = networkManager;
            stream = tcpClient.GetStream();
            LastPacketTime = DateTime.Now;
        }

        public void Start()
        {
            isRunning = true;
            receiveThread = new Thread(ReceiveLoop)
            {
                IsBackground = true
            };
            receiveThread.Start();
        }

        private void ReceiveLoop()
        {
            byte[] buffer = new byte[8192];

            while (isRunning && tcpClient.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            LastPacketTime = DateTime.Now;
                            // 处理数据包
                        }
                    }

                    // 超时检测
                    if ((DateTime.Now - LastPacketTime).TotalSeconds > 30)
                    {
                        Disconnect();
                    }

                    Thread.Sleep(10);
                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                stream?.Write(data, 0, data.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            isRunning = false;
            tcpClient?.Close();
            stream?.Close();
        }
    }

    // ========================================
    // 网络数据包
    // ========================================
    public class NetworkPacket
    {
        public int PacketId;
        public Dictionary<string, object> Data;

        public NetworkPacket()
        {
            Data = new Dictionary<string, object>();
        }

        public byte[] Serialize()
        {
            // 简化序列化
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(PacketId);
                writer.Write(Data.Count);

                foreach (var kvp in Data)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.ToString());
                }

                return ms.ToArray();
            }
        }

        public static NetworkPacket Deserialize(byte[] data, int length)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data, 0, length))
                using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
                {
                    NetworkPacket packet = new NetworkPacket
                    {
                        PacketId = reader.ReadInt32()
                    };

                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        string value = reader.ReadString();
                        packet.Data[key] = value;
                    }

                    return packet;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public enum PacketType
    {
        // 握手
        Handshake = 0,
        Login = 1,
        Disconnect = 2,

        // 玩家
        PlayerPosition = 10,
        PlayerLook = 11,
        PlayerPositionAndLook = 12,
        PlayerAnimation = 13,
        PlayerAction = 14,

        // 世界
        ChunkData = 20,
        BlockChange = 21,
        BlockBreak = 22,
        BlockPlace = 23,

        // 实体
        EntitySpawn = 30,
        EntityPosition = 31,
        EntityDestroy = 32,
        EntityMetadata = 33,
        EntityVelocity = 34,

        // 聊天
        ChatMessage = 40,

        // 其他
        KeepAlive = 50,
        TimeUpdate = 51,
        Weather = 52,
        SoundEffect = 53,
        Particle = 54
    }
}
