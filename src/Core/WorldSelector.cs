using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Save;

namespace VoxelCraft.Core
{
    public class WorldSelector
    {
        private bool isOpen;
        private int selectedWorld;
        private float scrollOffset;
        private readonly List<WorldInfo> worlds;

        // 事件
        public event Action<string> OnWorldSelected;
        public event Action<string> OnWorldDeleted;
        public event Action OnCreateWorld;
        public event Action OnClosed;

        public bool IsOpen => isOpen;
        public int WorldCount => worlds.Count;

        public WorldSelector()
        {
            worlds = new List<WorldInfo>();
            selectedWorld = -1;
        }

        public void Initialize()
        {
            Console.WriteLine("[WorldSelector] 世界选择器初始化完成");
            LoadWorldList();
        }

        public void Open()
        {
            isOpen = true;
            LoadWorldList();
            if (worlds.Count > 0)
            {
                selectedWorld = 0;
            }
        }

        public void Close()
        {
            isOpen = false;
            OnClosed?.Invoke();
        }

        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        private void LoadWorldList()
        {
            worlds.Clear();

            string savesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "saves");
            if (!Directory.Exists(savesDirectory))
            {
                Directory.CreateDirectory(savesDirectory);
                return;
            }

            string[] worldDirs = Directory.GetDirectories(savesDirectory);
            foreach (string worldDir in worldDirs)
            {
                try
                {
                    WorldInfo info = LoadWorldInfo(worldDir);
                    if (info != null)
                    {
                        worlds.Add(info);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WorldSelector] 加载世界信息失败: {worldDir} - {ex.Message}");
                }
            }

            // 按最后游玩时间排序
            worlds.Sort((a, b) => b.LastPlayed.CompareTo(a.LastPlayed));
        }

        private WorldInfo LoadWorldInfo(string worldDir)
        {
            string levelDatPath = Path.Combine(worldDir, "level.dat");
            if (!File.Exists(levelDatPath))
            {
                // 没有level.dat，创建基本信息
                return new WorldInfo
                {
                    Name = Path.GetFileName(worldDir),
                    Directory = worldDir,
                    LastPlayed = Directory.GetLastWriteTime(worldDir),
                    GameMode = "生存",
                    Difficulty = "普通",
                    Seed = 0,
                    TimePlayed = 0
                };
            }

            // 简化：从目录名和修改时间创建信息
            return new WorldInfo
            {
                Name = Path.GetFileName(worldDir),
                Directory = worldDir,
                LastPlayed = File.GetLastWriteTime(levelDatPath),
                GameMode = "生存",
                Difficulty = "普通",
                Seed = 0,
                TimePlayed = 0
            };
        }

        public void Update()
        {
            if (!isOpen) return;
        }

        public void Render(UIManager uiManager)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.1f, 0.1f, 0.15f, 1f));

            // 标题
            string title = "选择世界";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 20, 32, Color4.White);

            // 世界列表区域
            int listX = 50;
            int listY = 70;
            int listWidth = screenWidth - 100;
            int listHeight = screenHeight - 180;

            uiManager.DrawPanel(listX, listY, listWidth, listHeight,
                new Color4(0.15f, 0.15f, 0.2f, 0.9f));

            // 渲染世界列表
            int worldItemHeight = 60;
            int worldSpacing = 5;

            for (int i = 0; i < worlds.Count; i++)
            {
                int itemY = listY + 10 + i * (worldItemHeight + worldSpacing) - (int)scrollOffset;

                if (itemY < listY || itemY > listY + listHeight - worldItemHeight)
                {
                    continue;
                }

                WorldInfo world = worlds[i];
                bool isSelected = i == selectedWorld;

                // 世界项背景
                uiManager.DrawPanel(listX + 10, itemY, listWidth - 20, worldItemHeight,
                    isSelected ? new Color4(0.3f, 0.3f, 0.4f, 0.9f) : new Color4(0.2f, 0.2f, 0.25f, 0.9f));

                // 世界名称
                uiManager.DrawText(world.Name, listX + 25, itemY + 8, 18, Color4.White);

                // 世界信息
                string info = $"游戏模式: {world.GameMode} | 难度: {world.Difficulty} | 种子: {world.Seed}";
                uiManager.DrawText(info, listX + 25, itemY + 30, 12,
                    new Color4(0.7f, 0.7f, 0.7f, 1f));

                // 最后游玩时间
                string timeInfo = $"最后游玩: {world.LastPlayed:yyyy-MM-dd HH:mm} | 游戏时长: {FormatTimePlayed(world.TimePlayed)}";
                uiManager.DrawText(timeInfo, listX + 25, itemY + 45, 10,
                    new Color4(0.5f, 0.5f, 0.5f, 1f));

                // 世界版本
                uiManager.DrawText("VoxelCraft v1.0", listX + listWidth - 120, itemY + 8, 10,
                    new Color4(0.5f, 0.5f, 0.5f, 1f));
            }

            // 空列表提示
            if (worlds.Count == 0)
            {
                string emptyText = "没有保存的世界，点击\"创建新世界\"开始游戏";
                int emptyWidth = uiManager.MeasureText(emptyText, 16);
                uiManager.DrawText(emptyText, (screenWidth - emptyWidth) / 2, screenHeight / 2 - 20, 16,
                    new Color4(0.6f, 0.6f, 0.6f, 1f));
            }

            // 滚动条
            int scrollbarX = listX + listWidth - 10;
            int scrollbarY = listY + 5;
            int scrollbarHeight = listHeight - 10;
            float scrollRatio = worlds.Count > 0 ? (float)listHeight / (worlds.Count * (worldItemHeight + worldSpacing)) : 1;
            int thumbHeight = (int)(scrollbarHeight * Math.Min(1, scrollRatio));
            int thumbY = scrollbarY + (int)(scrollOffset / (worlds.Count * (worldItemHeight + worldSpacing)) * (scrollbarHeight - thumbHeight));

            uiManager.DrawPanel(scrollbarX, scrollbarY, 5, scrollbarHeight,
                new Color4(0.2f, 0.2f, 0.2f, 0.8f));
            uiManager.DrawPanel(scrollbarX, thumbY, 5, thumbHeight,
                new Color4(0.5f, 0.5f, 0.5f, 0.9f));

            // 按钮
            int buttonY = screenHeight - 80;
            int buttonWidth = 150;
            int buttonHeight = 35;
            int buttonSpacing = 10;
            int totalWidth = buttonWidth * 4 + buttonSpacing * 3;
            int startX = (screenWidth - totalWidth) / 2;

            // 进入世界按钮
            uiManager.DrawPanel(startX, buttonY, buttonWidth, buttonHeight,
                selectedWorld >= 0 ? new Color4(0.3f, 0.5f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("进入世界", startX + (buttonWidth - uiManager.MeasureText("进入世界", 14)) / 2,
                buttonY + 10, 14, Color4.White);

            // 创建新世界按钮
            uiManager.DrawPanel(startX + buttonWidth + buttonSpacing, buttonY, buttonWidth, buttonHeight,
                new Color4(0.3f, 0.4f, 0.5f, 0.9f));
            uiManager.DrawText("创建新世界", startX + buttonWidth + buttonSpacing + (buttonWidth - uiManager.MeasureText("创建新世界", 14)) / 2,
                buttonY + 10, 14, Color4.White);

            // 编辑世界按钮
            uiManager.DrawPanel(startX + (buttonWidth + buttonSpacing) * 2, buttonY, buttonWidth, buttonHeight,
                selectedWorld >= 0 ? new Color4(0.4f, 0.4f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("编辑世界", startX + (buttonWidth + buttonSpacing) * 2 + (buttonWidth - uiManager.MeasureText("编辑世界", 14)) / 2,
                buttonY + 10, 14, Color4.White);

            // 删除世界按钮
            uiManager.DrawPanel(startX + (buttonWidth + buttonSpacing) * 3, buttonY, buttonWidth, buttonHeight,
                selectedWorld >= 0 ? new Color4(0.5f, 0.3f, 0.3f, 0.9f) : new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("删除世界", startX + (buttonWidth + buttonSpacing) * 3 + (buttonWidth - uiManager.MeasureText("删除世界", 14)) / 2,
                buttonY + 10, 14, Color4.White);

            // 返回按钮
            int backBtnWidth = 100;
            uiManager.DrawPanel(20, 20, backBtnWidth, buttonHeight,
                new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("返回", 20 + (backBtnWidth - uiManager.MeasureText("返回", 14)) / 2,
                30, 14, Color4.White);
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 返回按钮
            if (mouseX >= 20 && mouseX <= 120 && mouseY >= 20 && mouseY <= 55)
            {
                Close();
                return;
            }

            // 世界列表点击
            int listX = 50;
            int listY = 70;
            int listWidth = screenWidth - 100;
            int listHeight = screenHeight - 180;
            int worldItemHeight = 60;
            int worldSpacing = 5;

            for (int i = 0; i < worlds.Count; i++)
            {
                int itemY = listY + 10 + i * (worldItemHeight + worldSpacing) - (int)scrollOffset;

                if (mouseX >= listX + 10 && mouseX <= listX + listWidth - 10 &&
                    mouseY >= itemY && mouseY <= itemY + worldItemHeight)
                {
                    selectedWorld = i;
                    return;
                }
            }

            // 按钮点击
            int buttonY = screenHeight - 80;
            int buttonWidth = 150;
            int buttonHeight = 35;
            int buttonSpacing = 10;
            int totalWidth = buttonWidth * 4 + buttonSpacing * 3;
            int startX = (screenWidth - totalWidth) / 2;

            // 进入世界
            if (mouseX >= startX && mouseX <= startX + buttonWidth &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                if (selectedWorld >= 0)
                {
                    OnWorldSelected?.Invoke(worlds[selectedWorld].Directory);
                }
                return;
            }

            // 创建新世界
            if (mouseX >= startX + buttonWidth + buttonSpacing &&
                mouseX <= startX + (buttonWidth + buttonSpacing) * 2 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                OnCreateWorld?.Invoke();
                return;
            }

            // 编辑世界
            if (mouseX >= startX + (buttonWidth + buttonSpacing) * 2 &&
                mouseX <= startX + (buttonWidth + buttonSpacing) * 3 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                // 编辑世界（简化：重新加载列表）
                LoadWorldList();
                return;
            }

            // 删除世界
            if (mouseX >= startX + (buttonWidth + buttonSpacing) * 3 &&
                mouseX <= startX + (buttonWidth + buttonSpacing) * 4 - buttonSpacing &&
                mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
            {
                if (selectedWorld >= 0)
                {
                    string worldDir = worlds[selectedWorld].Directory;
                    OnWorldDeleted?.Invoke(worldDir);
                    worlds.RemoveAt(selectedWorld);
                    if (selectedWorld >= worlds.Count)
                    {
                        selectedWorld = worlds.Count - 1;
                    }
                }
                return;
            }
        }

        public void HandleKeyPress(string key)
        {
            if (!isOpen) return;

            switch (key)
            {
                case "Escape":
                    Close();
                    break;

                case "Up":
                    if (selectedWorld > 0)
                    {
                        selectedWorld--;
                    }
                    break;

                case "Down":
                    if (selectedWorld < worlds.Count - 1)
                    {
                        selectedWorld++;
                    }
                    break;

                case "Enter":
                    if (selectedWorld >= 0)
                    {
                        OnWorldSelected?.Invoke(worlds[selectedWorld].Directory);
                    }
                    break;

                case "Delete":
                    if (selectedWorld >= 0)
                    {
                        string worldDir = worlds[selectedWorld].Directory;
                        OnWorldDeleted?.Invoke(worldDir);
                        worlds.RemoveAt(selectedWorld);
                        if (selectedWorld >= worlds.Count)
                        {
                            selectedWorld = worlds.Count - 1;
                        }
                    }
                    break;
            }
        }

        public void HandleScroll(float delta)
        {
            if (!isOpen) return;
            scrollOffset += delta * 50;
            scrollOffset = Math.Max(0, scrollOffset);
        }

        public void DeleteWorld(string worldDirectory)
        {
            try
            {
                if (Directory.Exists(worldDirectory))
                {
                    Directory.Delete(worldDirectory, true);
                    Console.WriteLine($"[WorldSelector] 已删除世界: {worldDirectory}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorldSelector] 删除世界失败: {ex.Message}");
            }
        }

        public List<WorldInfo> GetWorlds()
        {
            return new List<WorldInfo>(worlds);
        }

        public WorldInfo GetSelectedWorld()
        {
            if (selectedWorld >= 0 && selectedWorld < worlds.Count)
            {
                return worlds[selectedWorld];
            }
            return null;
        }

        private string FormatTimePlayed(long ticks)
        {
            long seconds = ticks / 20;
            long minutes = seconds / 60;
            long hours = minutes / 60;

            if (hours > 0)
            {
                return $"{hours}小时{minutes % 60}分钟";
            }
            else if (minutes > 0)
            {
                return $"{minutes}分钟";
            }
            else
            {
                return $"{seconds}秒";
            }
        }
    }

    public class WorldInfo
    {
        public string Name;
        public string Directory;
        public DateTime LastPlayed;
        public string GameMode;
        public string Difficulty;
        public long Seed;
        public long TimePlayed;
        public string Version;
        public bool HasCheats;
        public bool IsHardcore;
        public int PlayerCount;
        public long SizeOnDisk;

        public override string ToString()
        {
            return Name;
        }
    }
}
