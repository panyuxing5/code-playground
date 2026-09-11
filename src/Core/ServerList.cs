using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class ServerList
    {
        private readonly List<ServerInfo> servers;
        private int selectedServer;
        private readonly string serversFilePath;

        // 事件
        public event Action<ServerInfo> OnServerSelected;
        public event Action OnServerAdded;
        public event Action OnServerRemoved;
        public event Action OnClosed;

        public int ServerCount => servers.Count;
        public ServerInfo SelectedServer => selectedServer >= 0 && selectedServer < servers.Count ? servers[selectedServer] : null;

        public ServerList(string serversFilePath)
        {
            this.serversFilePath = serversFilePath;
            servers = new List<ServerInfo>();
            selectedServer = -1;
        }

        public void Initialize()
        {
            Console.WriteLine("[ServerList] 服务器列表初始化完成");
            LoadServers();
        }

        private void LoadServers()
        {
            servers.Clear();

            try
            {
                if (File.Exists(serversFilePath))
                {
                    string[] lines = File.ReadAllLines(serversFilePath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] parts = line.Split('|');
                        if (parts.Length >= 2)
                        {
                            ServerInfo server = new ServerInfo
                            {
                                Name = parts[0],
                                Address = parts[1],
                                Port = parts.Length > 2 ? int.Parse(parts[2]) : 25565,
                                Description = parts.Length > 3 ? parts[3] : "",
                                LastPlayed = parts.Length > 4 ? DateTime.Parse(parts[4]) : DateTime.MinValue
                            };
                            servers.Add(server);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServerList] 加载服务器列表失败: {ex.Message}");
            }

            // 添加一些默认服务器
            if (servers.Count == 0)
            {
                servers.Add(new ServerInfo
                {
                    Name = "本地服务器",
                    Address = "127.0.0.1",
                    Port = 25565,
                    Description = "本地测试服务器"
                });
            }
        }

        public void SaveServers()
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (ServerInfo server in servers)
                {
                    lines.Add($"{server.Name}|{server.Address}|{server.Port}|{server.Description}|{server.LastPlayed:yyyy-MM-dd HH:mm:ss}");
                }
                File.WriteAllLines(serversFilePath, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ServerList] 保存服务器列表失败: {ex.Message}");
            }
        }

        public void AddServer(string name, string address, int port, string description = "")
        {
            ServerInfo server = new ServerInfo
            {
                Name = name,
                Address = address,
                Port = port,
                Description = description,
                LastPlayed = DateTime.Now
            };

            servers.Add(server);
            SaveServers();
            OnServerAdded?.Invoke();
        }

        public void RemoveServer(int index)
        {
            if (index >= 0 && index < servers.Count)
            {
                servers.RemoveAt(index);
                SaveServers();

                if (selectedServer >= servers.Count)
                {
                    selectedServer = servers.Count - 1;
                }

                OnServerRemoved?.Invoke();
            }
        }

        public void RemoveServer(string name)
        {
            int index = servers.FindIndex(s => s.Name == name);
            if (index >= 0)
            {
                RemoveServer(index);
            }
        }

        public void SelectServer(int index)
        {
            if (index >= 0 && index < servers.Count)
            {
                selectedServer = index;
                servers[index].LastPlayed = DateTime.Now;
                SaveServers();
            }
        }

        public void ConnectToSelectedServer()
        {
            if (selectedServer >= 0 && selectedServer < servers.Count)
            {
                OnServerSelected?.Invoke(servers[selectedServer]);
            }
        }

        public void ConnectToServer(int index)
        {
            if (index >= 0 && index < servers.Count)
            {
                SelectServer(index);
                OnServerSelected?.Invoke(servers[index]);
            }
        }

        public List<ServerInfo> GetServers()
        {
            return new List<ServerInfo>(servers);
        }

        public ServerInfo GetServer(int index)
        {
            if (index >= 0 && index < servers.Count)
            {
                return servers[index];
            }
            return null;
        }

        public void Render(UIManager uiManager, int x, int y, int width, int height)
        {
            // 背景
            uiManager.DrawPanel(x, y, width, height,
                new Color4(0.15f, 0.15f, 0.2f, 0.95f));

            // 标题
            string title = "多人游戏";
            int titleWidth = uiManager.MeasureText(title, 24);
            uiManager.DrawText(title, x + (width - titleWidth) / 2, y + 10, 24, Color4.White);

            // 服务器列表
            int listY = y + 50;
            int listHeight = height - 120;
            int serverItemHeight = 50;
            int serverSpacing = 5;

            for (int i = 0; i < servers.Count; i++)
            {
                int itemY = listY + i * (serverItemHeight + serverSpacing);
                if (itemY + serverItemHeight > y + height - 60) break;

                ServerInfo server = servers[i];
                bool isSelected = i == selectedServer;

                // 服务器项背景
                uiManager.DrawPanel(x + 10, itemY, width - 20, serverItemHeight,
                    isSelected ? new Color4(0.3f, 0.3f, 0.4f, 0.9f) : new Color4(0.2f, 0.2f, 0.25f, 0.9f));

                // 服务器名称
                uiManager.DrawText(server.Name, x + 20, itemY + 5, 16, Color4.White);

                // 服务器地址
                uiManager.DrawText($"{server.Address}:{server.Port}", x + 20, itemY + 25, 12,
                    new Color4(0.7f, 0.7f, 0.7f, 1f));

                // 服务器描述
                if (!string.IsNullOrEmpty(server.Description))
                {
                    uiManager.DrawText(server.Description, x + 200, itemY + 5, 12,
                        new Color4(0.6f, 0.6f, 0.6f, 1f));
                }

                // 最后游玩时间
                if (server.LastPlayed != DateTime.MinValue)
                {
                    uiManager.DrawText($"最后游玩: {server.LastPlayed:yyyy-MM-dd}", x + width - 150, itemY + 25, 10,
                        new Color4(0.5f, 0.5f, 0.5f, 1f));
                }

                // 连接状态指示器
                uiManager.DrawPanel(x + width - 30, itemY + 15, 10, 10,
                    server.IsOnline ? new Color4(0.3f, 0.8f, 0.3f, 1f) : new Color4(0.5f, 0.3f, 0.3f, 1f));
            }

            // 空列表提示
            if (servers.Count == 0)
            {
                string emptyText = "没有保存的服务器，点击\"添加服务器\"添加";
                int emptyWidth = uiManager.MeasureText(emptyText, 14);
                uiManager.DrawText(emptyText, x + (width - emptyWidth) / 2, y + height / 2 - 20, 14,
                    new Color4(0.6f, 0.6f, 0.6f, 1f));
            }

            // 按钮
            int buttonY = y + height - 50;
            int buttonWidth = 120;
            int buttonHeight = 30;
            int buttonSpacing = 10;

            // 加入服务器
            uiManager.DrawPanel(x + 10, buttonY, buttonWidth, buttonHeight,
                selectedServer >= 0 ? new Color4(0.3f, 0.5f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("加入服务器", x + 10 + (buttonWidth - uiManager.MeasureText("加入服务器", 12)) / 2,
                buttonY + 8, 12, Color4.White);

            // 添加服务器
            uiManager.DrawPanel(x + 10 + buttonWidth + buttonSpacing, buttonY, buttonWidth, buttonHeight,
                new Color4(0.3f, 0.4f, 0.5f, 0.9f));
            uiManager.DrawText("添加服务器", x + 10 + buttonWidth + buttonSpacing + (buttonWidth - uiManager.MeasureText("添加服务器", 12)) / 2,
                buttonY + 8, 12, Color4.White);

            // 编辑服务器
            uiManager.DrawPanel(x + 10 + (buttonWidth + buttonSpacing) * 2, buttonY, buttonWidth, buttonHeight,
                selectedServer >= 0 ? new Color4(0.4f, 0.4f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("编辑服务器", x + 10 + (buttonWidth + buttonSpacing) * 2 + (buttonWidth - uiManager.MeasureText("编辑服务器", 12)) / 2,
                buttonY + 8, 12, Color4.White);

            // 删除服务器
            uiManager.DrawPanel(x + 10 + (buttonWidth + buttonSpacing) * 3, buttonY, buttonWidth, buttonHeight,
                selectedServer >= 0 ? new Color4(0.5f, 0.3f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("删除服务器", x + 10 + (buttonWidth + buttonSpacing) * 3 + (buttonWidth - uiManager.MeasureText("删除服务器", 12)) / 2,
                buttonY + 8, 12, Color4.White);

            // 直接连接
            uiManager.DrawPanel(x + 10 + (buttonWidth + buttonSpacing) * 4, buttonY, buttonWidth, buttonHeight,
                new Color4(0.3f, 0.3f, 0.4f, 0.9f));
            uiManager.DrawText("直接连接", x + 10 + (buttonWidth + buttonSpacing) * 4 + (buttonWidth - uiManager.MeasureText("直接连接", 12)) / 2,
                buttonY + 8, 12, Color4.White);
        }

        public void HandleClick(int mouseX, int mouseY, int x, int y, int width, int height)
        {
            // 服务器列表点击
            int listY = y + 50;
            int serverItemHeight = 50;
            int serverSpacing = 5;

            for (int i = 0; i < servers.Count; i++)
            {
                int itemY = listY + i * (serverItemHeight + serverSpacing);

                if (mouseX >= x + 10 && mouseX <= x + width - 10 &&
                    mouseY >= itemY && mouseY <= itemY + serverItemHeight)
                {
                    selectedServer = i;
                    return;
                }
            }

            // 按钮点击
            int buttonY = y + height - 50;
            int buttonWidth = 120;
            int buttonHeight = 30;
            int buttonSpacing = 10;

            // 加入服务器
            if (mouseX >= x + 10 && mouseX <= x + 10 + buttonWidth &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                ConnectToSelectedServer();
                return;
            }

            // 添加服务器
            if (mouseX >= x + 10 + buttonWidth + buttonSpacing &&
                mouseX <= x + 10 + (buttonWidth + buttonSpacing) * 2 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                // 添加服务器（简化：添加一个测试服务器）
                AddServer($"服务器 {servers.Count + 1}", "192.168.1.1", 25565, "新添加的服务器");
                return;
            }

            // 编辑服务器
            if (mouseX >= x + 10 + (buttonWidth + buttonSpacing) * 2 &&
                mouseX <= x + 10 + (buttonWidth + buttonSpacing) * 3 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                // 编辑服务器（简化：重新加载列表）
                LoadServers();
                return;
            }

            // 删除服务器
            if (mouseX >= x + 10 + (buttonWidth + buttonSpacing) * 3 &&
                mouseX <= x + 10 + (buttonWidth + buttonSpacing) * 4 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                if (selectedServer >= 0)
                {
                    RemoveServer(selectedServer);
                }
                return;
            }

            // 直接连接
            if (mouseX >= x + 10 + (buttonWidth + buttonSpacing) * 4 &&
                mouseX <= x + 10 + (buttonWidth + buttonSpacing) * 5 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                // 直接连接（简化：连接到选中的服务器）
                ConnectToSelectedServer();
                return;
            }
        }

        public void HandleKeyPress(string key)
        {
            switch (key)
            {
                case "Up":
                    if (selectedServer > 0)
                    {
                        selectedServer--;
                    }
                    break;

                case "Down":
                    if (selectedServer < servers.Count - 1)
                    {
                        selectedServer++;
                    }
                    break;

                case "Enter":
                    ConnectToSelectedServer();
                    break;

                case "Delete":
                    if (selectedServer >= 0)
                    {
                        RemoveServer(selectedServer);
                    }
                    break;
            }
        }
    }

    public class ServerInfo
    {
        public string Name;
        public string Address;
        public int Port;
        public string Description;
        public DateTime LastPlayed;
        public bool IsOnline;
        public int PlayerCount;
        public int MaxPlayers;
        public string Version;
        public long Ping;

        public override string ToString()
        {
            return $"{Name} ({Address}:{Port})";
        }
    }
}
