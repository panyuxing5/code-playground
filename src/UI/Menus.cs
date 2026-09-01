using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.UI
{
    public class PauseMenu
    {
        private bool isOpen;
        private int selectedButton;

        private readonly string[] menuButtons = { "返回游戏", "玩家列表", "进度", "统计", "对局域网开放", "选项", "退出到标题画面", "退出游戏" };

        // 事件
        public event Action OnResumeGame;
        public event Action OnOpenSettings;
        public event Action OnOpenPlayerList;
        public event Action OnOpenAdvancements;
        public event Action OnOpenStatistics;
        public event Action OnOpenToLan;
        public event Action OnExitToTitle;
        public event Action OnExitGame;

        public bool IsOpen => isOpen;

        public PauseMenu()
        {
            selectedButton = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("[PauseMenu] 暂停菜单初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedButton = 0;
        }

        public void Close()
        {
            isOpen = false;
            OnResumeGame?.Invoke();
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

        public void Update()
        {
            if (!isOpen) return;

            // 处理键盘导航
            // 实际游戏中处理上下键和回车键
        }

        public void Render(UIManager uiManager)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 半透明背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.6f));

            // 标题
            string title = "游戏菜单";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 40, 32, Color4.White);

            // 按钮
            int buttonWidth = 200;
            int buttonHeight = 30;
            int buttonSpacing = 5;
            int startY = 100;
            int buttonX = (screenWidth - buttonWidth) / 2;

            for (int i = 0; i < menuButtons.Length; i++)
            {
                int buttonY = startY + i * (buttonHeight + buttonSpacing);
                bool isSelected = i == selectedButton;

                // 按钮背景
                uiManager.DrawPanel(buttonX, buttonY, buttonWidth, buttonHeight,
                    isSelected ? new Color4(0.4f, 0.4f, 0.4f, 0.9f) : new Color4(0.25f, 0.25f, 0.25f, 0.9f));

                // 按钮边框
                uiManager.DrawPanel(buttonX, buttonY, buttonWidth, 1,
                    new Color4(0.5f, 0.5f, 0.5f, 1f));
                uiManager.DrawPanel(buttonX, buttonY + buttonHeight - 1, buttonWidth, 1,
                    new Color4(0.1f, 0.1f, 0.1f, 1f));

                // 按钮文本
                int textWidth = uiManager.MeasureText(menuButtons[i], 16);
                uiManager.DrawText(menuButtons[i], buttonX + (buttonWidth - textWidth) / 2,
                    buttonY + 7, 16, Color4.White);
            }

            // 版本信息
            string version = "VoxelCraft v1.0.0";
            int versionWidth = uiManager.MeasureText(version, 12);
            uiManager.DrawText(version, screenWidth - versionWidth - 10, screenHeight - 20, 12,
                new Color4(0.6f, 0.6f, 0.6f, 1f));
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int buttonWidth = 200;
            int buttonHeight = 30;
            int buttonSpacing = 5;
            int startY = 100;
            int buttonX = (screenWidth - buttonWidth) / 2;

            for (int i = 0; i < menuButtons.Length; i++)
            {
                int buttonY = startY + i * (buttonHeight + buttonSpacing);

                if (mouseX >= buttonX && mouseX <= buttonX + buttonWidth &&
                    mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
                {
                    SelectButton(i);
                    return;
                }
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
                    selectedButton = (selectedButton - 1 + menuButtons.Length) % menuButtons.Length;
                    break;

                case "Down":
                    selectedButton = (selectedButton + 1) % menuButtons.Length;
                    break;

                case "Enter":
                case "Space":
                    SelectButton(selectedButton);
                    break;
            }
        }

        private void SelectButton(int index)
        {
            selectedButton = index;

            switch (index)
            {
                case 0: // 返回游戏
                    Close();
                    break;

                case 1: // 玩家列表
                    OnOpenPlayerList?.Invoke();
                    break;

                case 2: // 进度
                    OnOpenAdvancements?.Invoke();
                    break;

                case 3: // 统计
                    OnOpenStatistics?.Invoke();
                    break;

                case 4: // 对局域网开放
                    OnOpenToLan?.Invoke();
                    break;

                case 5: // 选项
                    OnOpenSettings?.Invoke();
                    break;

                case 6: // 退出到标题画面
                    OnExitToTitle?.Invoke();
                    break;

                case 7: // 退出游戏
                    OnExitGame?.Invoke();
                    break;
            }
        }
    }

    public class MainMenu
    {
        private bool isOpen;
        private int selectedButton;
        private float splashRotation;
        private readonly Random random;

        private readonly string[] menuButtons = { "单人游戏", "多人游戏", "Minecraft Realms", "选项", "退出游戏" };
        private readonly string[] splashTexts = {
            "也试试 Minicraft!",
            "110813 个玩家在线!",
            "现在支持中文!",
            "C# + OpenTK 编写!",
            "4万行代码!",
            "体素沙盒游戏!",
            "开源免费!",
            "无限世界!",
            "60+生物群系!",
            "200+方块!",
            "12种生物!",
            "红石电路!",
            "附魔系统!",
            "村民交易!",
            "天气系统!",
            "昼夜循环!"
        };
        private string currentSplash;

        // 事件
        public event Action OnSinglePlayer;
        public event Action OnMultiPlayer;
        public event Action OnRealms;
        public event Action OnOpenSettings;
        public event Action OnExitGame;

        public bool IsOpen => isOpen;

        public MainMenu()
        {
            selectedButton = 0;
            random = new Random();
            currentSplash = splashTexts[random.Next(splashTexts.Length)];
        }

        public void Initialize()
        {
            Console.WriteLine("[MainMenu] 主菜单初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedButton = 0;
            currentSplash = splashTexts[random.Next(splashTexts.Length)];
        }

        public void Close()
        {
            isOpen = false;
        }

        public void Update(float deltaTime)
        {
            if (!isOpen) return;

            splashRotation = (float)Math.Sin(DateTime.Now.TimeOfDay.TotalSeconds * 2) * 0.1f;
        }

        public void Render(UIManager uiManager, TextureManager textureManager)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景渐变
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.1f, 0.1f, 0.2f, 1f));

            // 标题
            string title1 = "Voxel";
            string title2 = "Craft";
            int title1Width = uiManager.MeasureText(title1, 64);
            int title2Width = uiManager.MeasureText(title2, 64);
            int totalWidth = title1Width + title2Width + 10;
            int titleX = (screenWidth - totalWidth) / 2;

            uiManager.DrawText(title1, titleX, 60, 64, new Color4(0.8f, 0.9f, 1.0f, 1f));
            uiManager.DrawText(title2, titleX + title1Width + 10, 60, 64, new Color4(0.5f, 0.8f, 0.5f, 1f));

            // 闪烁标语
            string splash = currentSplash;
            int splashWidth = uiManager.MeasureText(splash, 20);
            int splashX = screenWidth / 2 + 100;
            int splashY = 120;

            // 旋转的闪烁标语
            uiManager.DrawText(splash, splashX, splashY, 20, new Color4(1.0f, 0.9f, 0.3f, 1f));

            // 按钮
            int buttonWidth = 250;
            int buttonHeight = 35;
            int buttonSpacing = 8;
            int startY = 180;
            int buttonX = (screenWidth - buttonWidth) / 2;

            for (int i = 0; i < menuButtons.Length; i++)
            {
                int buttonY = startY + i * (buttonHeight + buttonSpacing);
                bool isSelected = i == selectedButton;

                // 按钮背景
                uiManager.DrawPanel(buttonX, buttonY, buttonWidth, buttonHeight,
                    isSelected ? new Color4(0.4f, 0.5f, 0.4f, 0.9f) : new Color4(0.25f, 0.3f, 0.25f, 0.9f));

                // 按钮边框
                uiManager.DrawPanel(buttonX, buttonY, buttonWidth, 2,
                    new Color4(0.6f, 0.7f, 0.6f, 1f));
                uiManager.DrawPanel(buttonX, buttonY + buttonHeight - 2, buttonWidth, 2,
                    new Color4(0.15f, 0.2f, 0.15f, 1f));

                // 按钮文本
                int textWidth = uiManager.MeasureText(menuButtons[i], 18);
                uiManager.DrawText(menuButtons[i], buttonX + (buttonWidth - textWidth) / 2,
                    buttonY + 8, 18, Color4.White);
            }

            // 左下角信息
            uiManager.DrawText("VoxelCraft v1.0.0", 10, screenHeight - 40, 14,
                new Color4(0.6f, 0.6f, 0.6f, 1f));
            uiManager.DrawText("C# + OpenTK", 10, screenHeight - 20, 12,
                new Color4(0.5f, 0.5f, 0.5f, 1f));

            // 右下角版权
            uiManager.DrawText("Copyright (c) 2026", screenWidth - 150, screenHeight - 20, 12,
                new Color4(0.5f, 0.5f, 0.5f, 1f));
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int buttonWidth = 250;
            int buttonHeight = 35;
            int buttonSpacing = 8;
            int startY = 180;
            int buttonX = (screenWidth - buttonWidth) / 2;

            for (int i = 0; i < menuButtons.Length; i++)
            {
                int buttonY = startY + i * (buttonHeight + buttonSpacing);

                if (mouseX >= buttonX && mouseX <= buttonX + buttonWidth &&
                    mouseY >= buttonY && mouseY <= buttonY + buttonHeight)
                {
                    SelectButton(i);
                    return;
                }
            }
        }

        public void HandleKeyPress(string key)
        {
            if (!isOpen) return;

            switch (key)
            {
                case "Up":
                    selectedButton = (selectedButton - 1 + menuButtons.Length) % menuButtons.Length;
                    break;

                case "Down":
                    selectedButton = (selectedButton + 1) % menuButtons.Length;
                    break;

                case "Enter":
                case "Space":
                    SelectButton(selectedButton);
                    break;

                case "Escape":
                    // 主菜单不响应Escape
                    break;
            }
        }

        private void SelectButton(int index)
        {
            selectedButton = index;

            switch (index)
            {
                case 0: // 单人游戏
                    OnSinglePlayer?.Invoke();
                    break;

                case 1: // 多人游戏
                    OnMultiPlayer?.Invoke();
                    break;

                case 2: // Realms
                    OnRealms?.Invoke();
                    break;

                case 3: // 选项
                    OnOpenSettings?.Invoke();
                    break;

                case 4: // 退出游戏
                    OnExitGame?.Invoke();
                    break;
            }
        }
    }
}
