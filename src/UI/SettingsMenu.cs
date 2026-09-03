using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.UI
{
    public class SettingsMenu
    {
        private bool isOpen;
        private int selectedCategory;
        private int selectedOption;
        private float scrollOffset;

        private readonly string[] categories = { "视频", "音频", "控制", "语言", "皮肤", "聊天", "多人游戏", "辅助功能" };

        // 事件
        public event Action OnSettingsClosed;
        public event Action<string, object> OnSettingChanged;

        public bool IsOpen => isOpen;

        public SettingsMenu()
        {
            selectedCategory = 0;
            selectedOption = 0;
            scrollOffset = 0;
        }

        public SettingsMenu(UIManager ui, GameEngine engine) : this()
        {
        }

        public void Initialize()
        {
            Console.WriteLine("[SettingsMenu] 设置菜单初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedCategory = 0;
            selectedOption = 0;
        }

        public void Close()
        {
            isOpen = false;
            OnSettingsClosed?.Invoke();
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

            // 处理输入
            // 实际游戏中处理键盘和鼠标输入
        }

        public void Render(UIManager uiManager)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.7f));

            // 标题
            string title = "选项";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 20, 32, Color4.White);

            // 分类标签
            int categoryY = 70;
            int categoryWidth = 120;
            int categoryHeight = 30;
            int categorySpacing = 5;

            for (int i = 0; i < categories.Length; i++)
            {
                int catX = 20;
                int catY = categoryY + i * (categoryHeight + categorySpacing);
                bool isSelected = i == selectedCategory;

                uiManager.DrawPanel(catX, catY, categoryWidth, categoryHeight,
                    isSelected ? new Color4(0.3f, 0.3f, 0.3f, 0.9f) : new Color4(0.2f, 0.2f, 0.2f, 0.8f));

                int textWidth = uiManager.MeasureText(categories[i], 14);
                uiManager.DrawText(categories[i], catX + (categoryWidth - textWidth) / 2, catY + 8, 14,
                    isSelected ? Color4.White : new Color4(0.8f, 0.8f, 0.8f, 1f));
            }

            // 设置内容区域
            int contentX = 160;
            int contentY = 70;
            int contentWidth = screenWidth - 180;
            int contentHeight = screenHeight - 140;

            uiManager.DrawPanel(contentX, contentY, contentWidth, contentHeight,
                new Color4(0.15f, 0.15f, 0.15f, 0.9f));

            // 渲染当前分类的设置项
            RenderCategorySettings(uiManager, contentX + 10, contentY + 10, contentWidth - 20);

            // 完成按钮
            int doneBtnWidth = 200;
            int doneBtnHeight = 40;
            int doneBtnX = (screenWidth - doneBtnWidth) / 2;
            int doneBtnY = screenHeight - 50;

            uiManager.DrawPanel(doneBtnX, doneBtnY, doneBtnWidth, doneBtnHeight,
                new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("完成", doneBtnX + (doneBtnWidth - uiManager.MeasureText("完成", 18)) / 2,
                doneBtnY + 10, 18, Color4.White);
        }

        private void RenderCategorySettings(UIManager uiManager, int x, int y, int width)
        {
            GameSettings settings = GameSettings.Instance;
            int optionHeight = 30;
            int optionSpacing = 5;
            int currentY = y;

            switch (selectedCategory)
            {
                case 0: // 视频
                    RenderSlider(uiManager, x, currentY, width, "渲染距离", settings.RenderDistance, 2, 32, (v) => settings.RenderDistance = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "视野", settings.FOV, 30, 110, (v) => settings.FOV = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "最大帧率", settings.MaxFps, 30, 240, (v) => settings.MaxFps = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "垂直同步", settings.VSync, (v) => settings.VSync = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "全屏", settings.Fullscreen, (v) => settings.Fullscreen = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "平滑光照", settings.SmoothLighting, (v) => settings.SmoothLighting = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "云", settings.Clouds, 0, 2, (v) => settings.Clouds = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "视角摇晃", settings.ViewBobbing, (v) => settings.ViewBobbing = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示帧率", settings.ShowFps, (v) => settings.ShowFps = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "界面大小", (int)(settings.GuiScale * 10), 10, 40, (v) => settings.GuiScale = v / 10.0f);
                    break;

                case 1: // 音频
                    RenderSlider(uiManager, x, currentY, width, "主音量", (int)(settings.MasterVolume * 100), 0, 100, (v) => settings.MasterVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "音乐音量", (int)(settings.MusicVolume * 100), 0, 100, (v) => settings.MusicVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "音效音量", (int)(settings.SoundEffectsVolume * 100), 0, 100, (v) => settings.SoundEffectsVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "环境音量", (int)(settings.AmbientVolume * 100), 0, 100, (v) => settings.AmbientVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "方块音量", (int)(settings.BlocksVolume * 100), 0, 100, (v) => settings.BlocksVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "敌对生物音量", (int)(settings.HostileVolume * 100), 0, 100, (v) => settings.HostileVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "友好生物音量", (int)(settings.FriendlyVolume * 100), 0, 100, (v) => settings.FriendlyVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "玩家音量", (int)(settings.PlayersVolume * 100), 0, 100, (v) => settings.PlayersVolume = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "天气音量", (int)(settings.WeatherVolume * 100), 0, 100, (v) => settings.WeatherVolume = v / 100.0f);
                    break;

                case 2: // 控制
                    RenderSlider(uiManager, x, currentY, width, "鼠标灵敏度", (int)(settings.MouseSensitivity * 100), 0, 200, (v) => settings.MouseSensitivity = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "反转鼠标", settings.InvertMouse, (v) => settings.InvertMouse = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "自动跳跃", settings.AutoJump, (v) => settings.AutoJump = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "触屏模式", settings.TouchscreenMode, (v) => settings.TouchscreenMode = v);
                    currentY += optionHeight + optionSpacing;
                    // 键位设置
                    uiManager.DrawText("键位设置:", x, currentY, 14, Color4.White);
                    currentY += 25;
                    RenderKeyBinding(uiManager, x, currentY, width, "前进", settings.GetKeyBinding("forward"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "后退", settings.GetKeyBinding("back"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "左移", settings.GetKeyBinding("left"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "右移", settings.GetKeyBinding("right"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "跳跃", settings.GetKeyBinding("jump"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "潜行", settings.GetKeyBinding("sneak"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "疾跑", settings.GetKeyBinding("sprint"));
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "物品栏", settings.GetKeyBinding("inventory"));
                    break;

                case 3: // 语言
                    uiManager.DrawText("当前语言: " + settings.Language, x, currentY, 16, Color4.White);
                    currentY += 30;
                    string[] languages = { "zh_cn (简体中文)", "zh_tw (繁體中文)", "en_us (English)", "ja_jp (日本語)" };
                    for (int i = 0; i < languages.Length; i++)
                    {
                        bool isSelected = settings.Language == languages[i].Split(' ')[0];
                        uiManager.DrawPanel(x, currentY, width - 20, 25,
                            isSelected ? new Color4(0.3f, 0.3f, 0.3f, 0.9f) : new Color4(0.2f, 0.2f, 0.2f, 0.8f));
                        uiManager.DrawText(languages[i], x + 10, currentY + 5, 14,
                            isSelected ? Color4.White : new Color4(0.8f, 0.8f, 0.8f, 1f));
                        currentY += 30;
                    }
                    break;

                case 4: // 皮肤
                    uiManager.DrawText("玩家名称: " + settings.PlayerName, x, currentY, 14, Color4.White);
                    currentY += 25;
                    uiManager.DrawText("皮肤模型: " + settings.SkinModel, x, currentY, 14, Color4.White);
                    currentY += 25;
                    uiManager.DrawText("披风: " + settings.Cape, x, currentY, 14, Color4.White);
                    break;

                case 5: // 聊天
                    RenderToggle(uiManager, x, currentY, width, "聊天颜色", settings.ChatColors, (v) => settings.ChatColors = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "聊天链接", settings.ChatLinks, (v) => settings.ChatLinks = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "聊天提示", settings.ChatPrompts, (v) => settings.ChatPrompts = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "聊天不透明度", (int)(settings.ChatOpacity * 100), 0, 100, (v) => settings.ChatOpacity = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "聊天大小", (int)(settings.ChatScale * 100), 50, 150, (v) => settings.ChatScale = v / 100.0f);
                    break;

                case 6: // 多人游戏
                    RenderToggle(uiManager, x, currentY, width, "多人游戏警告", settings.MultiplayerWarnings, (v) => settings.MultiplayerWarnings = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "允许服务器列表", settings.AllowServerListing, (v) => settings.AllowServerListing = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "启用玩家举报", settings.EnablePlayerReporting, (v) => settings.EnablePlayerReporting = v);
                    break;

                case 7: // 辅助功能
                    RenderToggle(uiManager, x, currentY, width, "字幕", settings.Subtitles, (v) => settings.Subtitles = v);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "文本背景", settings.TextBackground, (v) => settings.TextBackground = v);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "文本背景不透明度", (int)(settings.TextBackgroundOpacity * 100), 0, 100, (v) => settings.TextBackgroundOpacity = v / 100.0f);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "隐藏闪烁文本", settings.HideSplashTexts, (v) => settings.HideSplashTexts = v);
                    break;
            }
        }

        private void RenderSlider(UIManager uiManager, int x, int y, int width, string label, int value, int min, int max, Action<int> onChange)
        {
            // 标签
            uiManager.DrawText(label, x, y + 5, 14, Color4.White);

            // 滑块背景
            int sliderX = x + 150;
            int sliderWidth = width - 200;
            int sliderHeight = 20;
            int sliderY = y + 5;

            uiManager.DrawPanel(sliderX, sliderY, sliderWidth, sliderHeight,
                new Color4(0.2f, 0.2f, 0.2f, 0.9f));

            // 滑块填充
            float ratio = (float)(value - min) / (max - min);
            int fillWidth = (int)(sliderWidth * ratio);
            uiManager.DrawPanel(sliderX, sliderY, fillWidth, sliderHeight,
                new Color4(0.3f, 0.6f, 0.3f, 0.9f));

            // 滑块手柄
            int handleX = sliderX + fillWidth - 5;
            uiManager.DrawPanel(handleX, sliderY - 2, 10, sliderHeight + 4,
                new Color4(0.8f, 0.8f, 0.8f, 1f));

            // 值
            string valueText = value.ToString();
            uiManager.DrawText(valueText, sliderX + sliderWidth + 10, y + 5, 14, Color4.White);
        }

        private void RenderToggle(UIManager uiManager, int x, int y, int width, string label, bool value, Action<bool> onChange)
        {
            // 标签
            uiManager.DrawText(label, x, y + 5, 14, Color4.White);

            // 开关
            int toggleX = x + width - 80;
            int toggleWidth = 60;
            int toggleHeight = 20;
            int toggleY = y + 5;

            uiManager.DrawPanel(toggleX, toggleY, toggleWidth, toggleHeight,
                value ? new Color4(0.3f, 0.7f, 0.3f, 0.9f) : new Color4(0.4f, 0.2f, 0.2f, 0.9f));

            // 开关手柄
            int handleX = value ? toggleX + toggleWidth - 18 : toggleX + 2;
            uiManager.DrawPanel(handleX, toggleY + 2, 16, toggleHeight - 4,
                new Color4(0.9f, 0.9f, 0.9f, 1f));

            // 状态文本
            string statusText = value ? "开" : "关";
            uiManager.DrawText(statusText, toggleX + (toggleWidth - uiManager.MeasureText(statusText, 12)) / 2,
                toggleY + 4, 12, Color4.White);
        }

        private void RenderKeyBinding(UIManager uiManager, int x, int y, int width, string action, string key)
        {
            uiManager.DrawText(action, x, y + 5, 14, Color4.White);

            int keyX = x + 150;
            int keyWidth = 100;
            int keyHeight = 20;

            uiManager.DrawPanel(keyX, y + 5, keyWidth, keyHeight,
                new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText(key, keyX + (keyWidth - uiManager.MeasureText(key, 12)) / 2,
                y + 9, 12, Color4.White);
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;

            // 检查分类点击
            int categoryY = 70;
            int categoryWidth = 120;
            int categoryHeight = 30;
            int categorySpacing = 5;

            for (int i = 0; i < categories.Length; i++)
            {
                int catX = 20;
                int catY = categoryY + i * (categoryHeight + categorySpacing);

                if (mouseX >= catX && mouseX <= catX + categoryWidth &&
                    mouseY >= catY && mouseY <= catY + categoryHeight)
                {
                    selectedCategory = i;
                    selectedOption = 0;
                    return;
                }
            }

            // 检查完成按钮
            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;
            int doneBtnWidth = 200;
            int doneBtnHeight = 40;
            int doneBtnX = (screenWidth - doneBtnWidth) / 2;
            int doneBtnY = screenHeight - 50;

            if (mouseX >= doneBtnX && mouseX <= doneBtnX + doneBtnWidth &&
                mouseY >= doneBtnY && mouseY <= doneBtnY + doneBtnHeight)
            {
                Close();
                return;
            }
        }

        public void HandleKeyPress(string key)
        {
            if (!isOpen) return;

            if (key == "Escape")
            {
                Close();
            }
        }
    }
}
