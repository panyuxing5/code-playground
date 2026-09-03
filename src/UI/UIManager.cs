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
        private readonly ChatSystem chat;

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
            chat = new ChatSystem();
        }

        public UIManager() : this(null, null, null)
        {
        }

        public void Initialize()
        {
            Initialize(1280, 720);
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
                inventoryScreen.Render(this, null);
            }

            // 暂停菜单
            if (IsPauseMenuOpen)
            {
                pauseMenu.Render(this);
            }

            // 聊天
            chat.Render(this);

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

        public void RenderDebugInfo()
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
                $"渲染距离: {GameEngine.Instance.RenderDistance}",
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
            chat.AddMessage("System", message);
        }

        public void Dispose()
        {
            fontRenderer?.Dispose();
            Console.WriteLine("[UIManager] UI管理器已释放");
        }

        // ========================================
        // 兼容方法
        // ========================================
        public void DrawPanel(int x, int y, int width, int height, Vector4 color)
        {
            // 绘制面板背景
        }

        public void DrawPanel(int x, int y, int width, int height, Color4 color)
        {
        }

        public void DrawPanel(float x, float y, float width, float height, Color4 color)
        {
        }

        public void DrawText(string text, int x, int y, Vector4 color, float scale = 1.0f)
        {
            fontRenderer?.DrawText(text, x, y, color, scale);
        }

        public void DrawText(string text, int x, int y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, int x, float y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, float x, int y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, float x, float y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, int x, int y, Color4 color)
        {
        }

        public void DrawText(string text, float x, float y, Color4 color)
        {
        }

        public Vector2 MeasureText(string text, float scale = 1.0f)
        {
            return fontRenderer?.MeasureText(text, scale) ?? Vector2.Zero;
        }

        public int MeasureText(string text, int fontSize)
        {
            return text.Length * fontSize;
        }

        public void DrawItemIcon(int itemId, int x, int y, int size = 32)
        {
            // 绘制物品图标
        }

        public void RenderLoadingScreen(string message, float progress)
        {
            // 渲染加载界面
        }

        public void RenderMainMenu()
        {
            mainMenu?.Render(this, null);
        }

        public void ShowMainMenu(bool show)
        {
            // 显示或隐藏主菜单
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

        public void DrawText(string text, int x, int y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, int x, float y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, float x, int y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, float x, float y, int fontSize, Color4 color)
        {
        }

        public void DrawText(string text, float x, float y, float fontSize, Color4 color)
        {
        }

        public void DrawText(string text, int x, int y, float fontSize, Color4 color)
        {
        }

        public void DrawText(string text, int x, int y, int fontSize, Vector4 color)
        {
        }

        public void DrawText(string text, int x, int y, Vector4 color)
        {
        }

        public void DrawText(string text, int x, int y, Color4 color)
        {
        }

        public void DrawText(string text, int x, int y, Color4 color, float scale)
        {
        }

        public void DrawText(string text, int x, int y, Vector4 color, float scale)
        {
        }

        public void DrawText(string text, int x, int y, float r, float g, float b, float a)
        {
        }

        public void DrawText(string text, float x, float y, Color4 color)
        {
        }

        public void DrawText(string text, float x, float y, Vector4 color)
        {
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

        

        public Vector2 MeasureText(string text, float scale = 1.0f)
        {
            return new Vector2(text.Length * 8 * scale, 16 * scale);
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
