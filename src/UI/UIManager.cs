using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelCraft.Core;
using VoxelCraft.Items;
using VoxelCraft.Player;

namespace VoxelCraft.UI
{
    public class UIManager
    {
        private readonly GameEngine engine;
        private readonly PlayerController player;
        private readonly InputManager input;

        // UI状态
        public bool IsInventoryOpen { get; private set; }
        public bool IsPauseMenuOpen { get; private set; }
        public bool IsChatOpen { get; private set; }
        public bool IsDebugInfoVisible { get; private set; } = true;

        // UI元素
        private readonly HUD hud;
        private readonly InventoryScreen inventoryScreen;
        private readonly PauseMenu pauseMenu;
        private readonly MainMenu mainMenu;
        private readonly SettingsMenu settingsMenu;
        private readonly ChatUI chat;

        // 字体渲染
        private FontRenderer fontRenderer;

        // 屏幕尺寸
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }

        // 选中的快捷栏槽位
        private int selectedHotbarSlot = 0;

        public UIManager(GameEngine engine, PlayerController player, InputManager input)
        {
            this.engine = engine;
            this.player = player;
            this.input = input;

            hud = new HUD(this, player);
            inventoryScreen = new InventoryScreen(this, player);
            pauseMenu = new PauseMenu(this, engine);
            mainMenu = new MainMenu(this, engine);
            settingsMenu = new SettingsMenu(this, engine);
            chat = new ChatUI(this);
        }

        public void Initialize(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
            fontRenderer = new FontRenderer();
            fontRenderer.Initialize();

            hud.Initialize();
            inventoryScreen.Initialize();
            pauseMenu.Initialize();
            mainMenu.Initialize();
            settingsMenu.Initialize();
            chat.Initialize();

            Console.WriteLine("[UIManager] UI管理器初始化完成");
        }

        public void Update(float deltaTime)
        {
            // 切换物品栏
            if (input.IsKeyPressed(Keys.E) && !IsPauseMenuOpen)
            {
                IsInventoryOpen = !IsInventoryOpen;
                engine.IsCursorVisible = IsInventoryOpen;
            }

            // 暂停菜单
            if (input.IsKeyPressed(Keys.Escape))
            {
                if (IsInventoryOpen)
                {
                    IsInventoryOpen = false;
                    engine.IsCursorVisible = false;
                }
                else if (IsChatOpen)
                {
                    IsChatOpen = false;
                }
                else
                {
                    IsPauseMenuOpen = !IsPauseMenuOpen;
                    engine.IsCursorVisible = IsPauseMenuOpen;
                }
            }

            // 调试信息
            if (input.IsKeyPressed(Keys.F3))
            {
                IsDebugInfoVisible = !IsDebugInfoVisible;
            }

            // 聊天
            if (input.IsKeyPressed(Keys.T) && !IsInventoryOpen && !IsPauseMenuOpen)
            {
                IsChatOpen = true;
                engine.IsCursorVisible = true;
            }

            // 快捷栏切换
            float scroll = input.GetMouseScroll();
            if (scroll != 0)
            {
                selectedHotbarSlot = (selectedHotbarSlot - (int)Math.Sign(scroll) + 9) % 9;
            }

            // 数字键切换快捷栏
            for (int i = 0; i < 9; i++)
            {
                if (input.IsKeyPressed(Keys.D0 + i) || input.IsKeyPressed(Keys.KeyPad0 + i))
                {
                    selectedHotbarSlot = i == 0 ? 9 : i - 1;
                }
            }

            // 更新各个UI
            hud.Update(deltaTime);

            if (IsInventoryOpen)
            {
                inventoryScreen.Update(deltaTime);
            }

            if (IsPauseMenuOpen)
            {
                pauseMenu.Update(deltaTime);
            }

            chat.Update(deltaTime);
        }

        public void Render()
        {
            // 始终渲染HUD
            hud.Render();

            // 调试信息
            if (IsDebugInfoVisible)
            {
                RenderDebugInfo();
            }

            // 物品栏
            if (IsInventoryOpen)
            {
                inventoryScreen.Render();
            }

            // 暂停菜单
            if (IsPauseMenuOpen)
            {
                pauseMenu.Render();
            }

            // 聊天
            chat.Render();

            // 十字准星
            if (!IsInventoryOpen && !IsPauseMenuOpen)
            {
                RenderCrosshair();
            }
        }

        private void RenderCrosshair()
        {
            int centerX = ScreenWidth / 2;
            int centerY = ScreenHeight / 2;
            int size = 10;

            // 简单的十字准星
            // 实际应该用OpenGL渲染
        }

        private void RenderDebugInfo()
        {
            int x = 5;
            int y = 5;
            int lineHeight = 12;

            List<string> debugLines = new List<string>
            {
                $"VoxelCraft 0.1.0",
                $"FPS: {engine.FPS:F0}",
                $"位置: {player.Position.X:F1}, {player.Position.Y:F1}, {player.Position.Z:F1}",
                $"朝向: {player.Yaw * 180 / Math.PI:F0}, {player.Pitch * 180 / Math.PI:F0}",
                $"速度: {player.Velocity.Length:F2}",
                $"在地上: {player.IsGrounded}",
                $"飞行: {player.IsFlying}",
                $"生命: {player.Health:F0}/{player.MaxHealth}",
                $"饥饿: {player.Hunger:F0}/{player.MaxHunger}",
                $"区块: {(int)player.Position.X >> 4}, {(int)player.Position.Z >> 4}",
                $"渲染距离: {GameEngine.RenderDistance}",
                $"已加载区块: {engine.World?.LoadedChunkCount ?? 0}",
                $"内存: {GC.GetTotalMemory(false) / 1024 / 1024:F0} MB"
            };

            foreach (string line in debugLines)
            {
                fontRenderer.DrawText(line, x, y, Vector3.One);
                y += lineHeight;
            }
        }

        public void Resize(int width, int height)
        {
            ScreenWidth = width;
            ScreenHeight = height;
        }

        public int GetSelectedHotbarSlot()
        {
            return selectedHotbarSlot;
        }

        public void SetSelectedHotbarSlot(int slot)
        {
            selectedHotbarSlot = Math.Clamp(slot, 0, 8);
        }

        public FontRenderer GetFontRenderer()
        {
            return fontRenderer;
        }

        public void AddChatMessage(string message)
        {
            chat.AddMessage(message);
        }

        public void Dispose()
        {
            fontRenderer?.Dispose();
            Console.WriteLine("[UIManager] UI管理器已释放");
        }
    }

    // ========================================
    // HUD (抬头显示)
    // ========================================
    public class HUD
    {
        private readonly UIManager uiManager;
        private readonly PlayerController player;

        // 快捷栏
        private const int HotbarSlotSize = 40;
        private const int HotbarSlotCount = 9;

        // 心形和鸡腿图标
        private const int IconSize = 16;
        private const int HeartCount = 10;
        private const int HungerCount = 10;

        // 经验条
        private const int ExperienceBarWidth = 200;
        private const int ExperienceBarHeight = 5;

        public HUD(UIManager uiManager, PlayerController player)
        {
            this.uiManager = uiManager;
            this.player = player;
        }

        public void Initialize()
        {
        }

        public void Update(float deltaTime)
        {
        }

        public void Render()
        {
            RenderHotbar();
            RenderHealthAndHunger();
            RenderExperienceBar();
            RenderSelectedItemName();
        }

        private void RenderHotbar()
        {
            int totalWidth = HotbarSlotCount * HotbarSlotSize;
            int startX = (uiManager.ScreenWidth - totalWidth) / 2;
            int y = uiManager.ScreenHeight - HotbarSlotSize - 5;

            for (int i = 0; i < HotbarSlotCount; i++)
            {
                int x = startX + i * HotbarSlotSize;
                bool isSelected = i == uiManager.GetSelectedHotbarSlot();

                // 渲染槽位背景
                RenderSlot(x, y, HotbarSlotSize, isSelected);

                // 渲染物品
                // ItemStack item = player.Inventory.GetSlot(player.Inventory.HotbarStart + i);
                // if (item != null && !item.IsEmpty())
                // {
                //     RenderItemIcon(item, x + 4, y + 4);
                //     RenderItemCount(item, x + HotbarSlotSize - 16, y + HotbarSlotSize - 16);
                // }
            }
        }

        private void RenderSlot(int x, int y, int size, bool selected)
        {
            // 渲染槽位背景
            // 实际应该用OpenGL渲染
        }

        private void RenderHealthAndHunger()
        {
            int startX = (uiManager.ScreenWidth - (HeartCount * IconSize * 2)) / 2;
            int y = uiManager.ScreenHeight - HotbarSlotSize - IconSize - 15;

            // 生命值（心形）
            for (int i = 0; i < HeartCount; i++)
            {
                int x = startX + i * IconSize;
                float health = player.Health - i * 2;

                if (health >= 2)
                {
                    // 完整的心
                    RenderHeart(x, y, true, false);
                }
                else if (health >= 1)
                {
                    // 半颗心
                    RenderHeart(x, y, false, true);
                }
                else
                {
                    // 空的心
                    RenderHeart(x, y, false, false);
                }
            }

            // 饥饿值（鸡腿）
            startX = (uiManager.ScreenWidth + (HeartCount * IconSize * 2)) / 2 - HungerCount * IconSize;
            for (int i = 0; i < HungerCount; i++)
            {
                int x = startX + i * IconSize;
                float hunger = player.Hunger - i * 2;

                if (hunger >= 2)
                {
                    RenderDrumstick(x, y, true);
                }
                else if (hunger >= 1)
                {
                    RenderDrumstick(x, y, false);
                }
            }
        }

        private void RenderHeart(int x, int y, bool full, bool half)
        {
            // 渲染心形图标
        }

        private void RenderDrumstick(int x, int y, bool full)
        {
            // 渲染鸡腿图标
        }

        private void RenderExperienceBar()
        {
            int x = (uiManager.ScreenWidth - ExperienceBarWidth) / 2;
            int y = uiManager.ScreenHeight - HotbarSlotSize - 10;

            // 背景
            // 前景（经验值）
            float progress = player.Experience % 100 / 100f;

            // 等级文字
            if (player.Level > 0)
            {
                uiManager.GetFontRenderer().DrawText(
                    player.Level.ToString(),
                    uiManager.ScreenWidth / 2 - 5,
                    y - 15,
                    new Vector3(0.5f, 1.0f, 0.5f)
                );
            }
        }

        private void RenderSelectedItemName()
        {
            // ItemStack selected = player.Inventory.GetSelectedItem();
            // if (selected != null && !selected.IsEmpty())
            // {
            //     ItemInfo info = selected.GetItemInfo();
            //     if (info != null)
            //     {
            //         int x = uiManager.ScreenWidth / 2;
            //         int y = uiManager.ScreenHeight - HotbarSlotSize - 40;
            //         uiManager.GetFontRenderer().DrawTextCentered(info.Name, x, y, Vector3.One);
            //     }
            // }
        }
    }

    // ========================================
    // 物品栏界面
    // ========================================
    public class InventoryScreen
    {
        private readonly UIManager uiManager;
        private readonly PlayerController player;

        private const int SlotSize = 36;
        private const int SlotGap = 4;
        private const int InventoryWidth = 9;
        private const int InventoryHeight = 3;

        private int windowX;
        private int windowY;
        private int windowWidth;
        private int windowHeight;

        public InventoryScreen(UIManager uiManager, PlayerController player)
        {
            this.uiManager = uiManager;
            this.player = player;
        }

        public void Initialize()
        {
            windowWidth = InventoryWidth * (SlotSize + SlotGap) + SlotGap * 2;
            windowHeight = (InventoryHeight + 1) * (SlotSize + SlotGap) + SlotGap * 2 + 50;
        }

        public void Update(float deltaTime)
        {
            // 计算窗口位置
            windowX = (uiManager.ScreenWidth - windowWidth) / 2;
            windowY = (uiManager.ScreenHeight - windowHeight) / 2;

            // 鼠标点击处理
            // if (input.IsMouseButtonPressed(MouseButton.Left))
            // {
            //     HandleClick(input.MousePosition);
            // }
        }

        public void Render()
        {
            // 半透明背景
            RenderBackground();

            // 标题
            uiManager.GetFontRenderer().DrawText(
                "物品栏",
                windowX + 10,
                windowY + 10,
                Vector3.One
            );

            // 玩家物品栏
            RenderPlayerInventory();

            // 护甲槽
            RenderArmorSlots();

            // 副手槽
            RenderOffhandSlot();

            // 合成格子
            RenderCraftingGrid();
        }

        private void RenderBackground()
        {
            // 渲染半透明灰色背景
        }

        private void RenderPlayerInventory()
        {
            int startX = windowX + SlotGap;
            int startY = windowY + 50;

            for (int row = 0; row < InventoryHeight; row++)
            {
                for (int col = 0; col < InventoryWidth; col++)
                {
                    int x = startX + col * (SlotSize + SlotGap);
                    int y = startY + row * (SlotSize + SlotGap);
                    int slotIndex = row * InventoryWidth + col;

                    RenderSlot(x, y, SlotSize);
                    // 渲染物品
                }
            }

            // 快捷栏
            startY += (InventoryHeight + 1) * (SlotSize + SlotGap);
            for (int col = 0; col < InventoryWidth; col++)
            {
                int x = startX + col * (SlotSize + SlotGap);
                int slotIndex = InventoryHeight * InventoryWidth + col;

                RenderSlot(x, startY, SlotSize);
                // 渲染物品
            }
        }

        private void RenderArmorSlots()
        {
            // 渲染4个护甲槽
        }

        private void RenderOffhandSlot()
        {
            // 渲染副手槽
        }

        private void RenderCraftingGrid()
        {
            // 渲染2x2合成格子和结果槽
        }

        private void RenderSlot(int x, int y, int size)
        {
            // 渲染槽位
        }
    }

    // ========================================
    // 暂停菜单
    // ========================================
    public class PauseMenu
    {
        private readonly UIManager uiManager;
        private readonly GameEngine engine;

        private List<Button> buttons;
        private int selectedButtonIndex;

        public PauseMenu(UIManager uiManager, GameEngine engine)
        {
            this.uiManager = uiManager;
            this.engine = engine;
            buttons = new List<Button>();
        }

        public void Initialize()
        {
            buttons.Clear();

            buttons.Add(new Button
            {
                Text = "返回游戏",
                Action = () => { uiManager.IsPauseMenuOpen = false; engine.IsCursorVisible = false; }
            });

            buttons.Add(new Button
            {
                Text = "设置",
                Action = () => { /* 打开设置 */ }
            });

            buttons.Add(new Button
            {
                Text = "对局域网开放",
                Action = () => { /* 开放局域网 */ }
            });

            buttons.Add(new Button
            {
                Text = "保存并退出",
                Action = () => { engine.SaveAndExit(); }
            });
        }

        public void Update(float deltaTime)
        {
            // 键盘导航
            // if (input.IsKeyPressed(Keys.Up)) selectedButtonIndex = (selectedButtonIndex - 1 + buttons.Count) % buttons.Count;
            // if (input.IsKeyPressed(Keys.Down)) selectedButtonIndex = (selectedButtonIndex + 1) % buttons.Count;
            // if (input.IsKeyPressed(Keys.Enter)) buttons[selectedButtonIndex].Action.Invoke();
        }

        public void Render()
        {
            // 半透明背景
            // 标题
            uiManager.GetFontRenderer().DrawTextCentered(
                "游戏暂停",
                uiManager.ScreenWidth / 2,
                uiManager.ScreenHeight / 4,
                Vector3.One,
                2.0f
            );

            // 按钮
            int buttonWidth = 200;
            int buttonHeight = 40;
            int startY = uiManager.ScreenHeight / 3;

            for (int i = 0; i < buttons.Count; i++)
            {
                int x = (uiManager.ScreenWidth - buttonWidth) / 2;
                int y = startY + i * (buttonHeight + 10);

                RenderButton(x, y, buttonWidth, buttonHeight, buttons[i].Text, i == selectedButtonIndex);
            }
        }

        private void RenderButton(int x, int y, int width, int height, string text, bool selected)
        {
            // 渲染按钮背景和文字
        }
    }

    // ========================================
    // 主菜单
    // ========================================
    public class MainMenu
    {
        private readonly UIManager uiManager;
        private readonly GameEngine engine;

        private List<Button> buttons;

        public MainMenu(UIManager uiManager, GameEngine engine)
        {
            this.uiManager = uiManager;
            this.engine = engine;
            buttons = new List<Button>();
        }

        public void Initialize()
        {
            buttons.Clear();

            buttons.Add(new Button { Text = "单人游戏", Action = () => { } });
            buttons.Add(new Button { Text = "多人游戏", Action = () => { } });
            buttons.Add(new Button { Text = "设置", Action = () => { } });
            buttons.Add(new Button { Text = "退出游戏", Action = () => { engine.Exit(); } });
        }

        public void Update(float deltaTime)
        {
        }

        public void Render()
        {
            // 渲染主菜单背景
            // 标题
            uiManager.GetFontRenderer().DrawTextCentered(
                "VoxelCraft",
                uiManager.ScreenWidth / 2,
                uiManager.ScreenHeight / 4,
                new Vector3(0.3f, 0.8f, 0.3f),
                3.0f
            );

            // 按钮
            int buttonWidth = 250;
            int buttonHeight = 40;
            int startY = uiManager.ScreenHeight / 3;

            for (int i = 0; i < buttons.Count; i++)
            {
                int x = (uiManager.ScreenWidth - buttonWidth) / 2;
                int y = startY + i * (buttonHeight + 10);
                RenderButton(x, y, buttonWidth, buttonHeight, buttons[i].Text);
            }
        }

        private void RenderButton(int x, int y, int width, int height, string text)
        {
        }
    }

    // ========================================
    // 设置菜单
    // ========================================
    public class SettingsMenu
    {
        private readonly UIManager uiManager;
        private readonly GameEngine engine;

        private List<SettingOption> options;
        private int selectedIndex;

        public SettingsMenu(UIManager uiManager, GameEngine engine)
        {
            this.uiManager = uiManager;
            this.engine = engine;
            options = new List<SettingOption>();
        }

        public void Initialize()
        {
            options.Clear();

            options.Add(new SettingOption
            {
                Name = "渲染距离",
                Type = SettingType.Slider,
                MinValue = 2,
                MaxValue = 16,
                CurrentValue = GameEngine.RenderDistance,
                OnValueChanged = (v) => { GameEngine.RenderDistance = (int)v; }
            });

            options.Add(new SettingOption
            {
                Name = "视野",
                Type = SettingType.Slider,
                MinValue = 30,
                MaxValue = 110,
                CurrentValue = 70,
                OnValueChanged = (v) => { }
            });

            options.Add(new SettingOption
            {
                Name = "音乐音量",
                Type = SettingType.Slider,
                MinValue = 0,
                MaxValue = 100,
                CurrentValue = 100,
                OnValueChanged = (v) => { }
            });

            options.Add(new SettingOption
            {
                Name = "音效音量",
                Type = SettingType.Slider,
                MinValue = 0,
                MaxValue = 100,
                CurrentValue = 100,
                OnValueChanged = (v) => { }
            });

            options.Add(new SettingOption
            {
                Name = "垂直同步",
                Type = SettingType.Toggle,
                CurrentValue = 1,
                OnValueChanged = (v) => { }
            });

            options.Add(new SettingOption
            {
                Name = "全屏",
                Type = SettingType.Toggle,
                CurrentValue = 0,
                OnValueChanged = (v) => { }
            });
        }

        public void Update(float deltaTime)
        {
        }

        public void Render()
        {
        }
    }

    // ========================================
    // 聊天UI
    // ========================================
    public class ChatUI
    {
        private readonly UIManager uiManager;
        private List<ChatMessage> messages;
        private string inputText;
        private int maxMessages = 100;
        private int visibleMessages = 10;

        public ChatUI(UIManager uiManager)
        {
            this.uiManager = uiManager;
            messages = new List<ChatMessage>();
            inputText = "";
        }

        public void Initialize()
        {
        }

        public void Update(float deltaTime)
        {
            // 消息淡出
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                messages[i].Age += deltaTime;
                if (messages[i].Age > 10.0f)
                {
                    messages.RemoveAt(i);
                }
            }
        }

        public void Render()
        {
            int x = 5;
            int y = uiManager.ScreenHeight - 100;
            int lineHeight = 12;

            // 渲染历史消息
            int start = Math.Max(0, messages.Count - visibleMessages);
            for (int i = start; i < messages.Count; i++)
            {
                float alpha = Math.Clamp(1.0f - messages[i].Age / 10.0f, 0, 1);
                uiManager.GetFontRenderer().DrawText(
                    messages[i].Text,
                    x,
                    y - (messages.Count - 1 - i) * lineHeight,
                    new Vector3(1, 1, 1) * alpha
                );
            }

            // 输入框
            if (uiManager.IsChatOpen)
            {
                uiManager.GetFontRenderer().DrawText(
                    "> " + inputText + "_",
                    x,
                    y + lineHeight,
                    Vector3.One
                );
            }
        }

        public void AddMessage(string text)
        {
            messages.Add(new ChatMessage { Text = text, Age = 0 });
            if (messages.Count > maxMessages)
            {
                messages.RemoveAt(0);
            }
        }

        public void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(inputText))
            {
                AddMessage("<玩家> " + inputText);
                inputText = "";
            }
        }
    }

    // ========================================
    // 辅助类
    // ========================================
    public class Button
    {
        public string Text;
        public Action Action;
        public bool Enabled = true;
    }

    public class SettingOption
    {
        public string Name;
        public SettingType Type;
        public float MinValue;
        public float MaxValue;
        public float CurrentValue;
        public Action<float> OnValueChanged;
    }

    public enum SettingType
    {
        Slider,
        Toggle,
        Select
    }

    public class ChatMessage
    {
        public string Text;
        public float Age;
    }

    // ========================================
    // 字体渲染器（简化版）
    // ========================================
    public class FontRenderer : IDisposable
    {
        private int fontTexture;
        private Dictionary<char, Glyph> glyphs;

        public void Initialize()
        {
            glyphs = new Dictionary<char, Glyph>();
            // 初始化字体纹理
            Console.WriteLine("[FontRenderer] 字体渲染器初始化完成");
        }

        public void DrawText(string text, int x, int y, Vector3 color)
        {
            // 简化：实际应该用OpenGL渲染
        }

        public void DrawTextCentered(string text, int centerX, int y, Vector3 color, float scale = 1.0f)
        {
            int textWidth = GetTextWidth(text);
            DrawText(text, centerX - textWidth / 2, y, color);
        }

        public int GetTextWidth(string text)
        {
            return text.Length * 8; // 简化
        }

        public int GetTextHeight(string text)
        {
            return 12;
        }

        public void Dispose()
        {
        }
    }

    public struct Glyph
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int XOffset;
        public int YOffset;
        public int XAdvance;
    }
}
