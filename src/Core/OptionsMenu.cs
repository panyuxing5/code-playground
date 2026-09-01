using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class OptionsMenu
    {
        private bool isOpen;
        private int selectedCategory;
        private int selectedOption;

        private readonly string[] categories = { "视频设置", "音频设置", "控制设置", "语言设置", "聊天设置", "皮肤设置", "资源包", "游戏设置" };

        // 事件
        public event Action OnClosed;
        public event Action<string, object> OnOptionChanged;

        public bool IsOpen => isOpen;

        public OptionsMenu()
        {
            selectedCategory = 0;
            selectedOption = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("[OptionsMenu] 选项菜单初始化完成");
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
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.8f));

            // 标题
            string title = "选项";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 20, 32, Color4.White);

            // 分类标签
            int categoryY = 70;
            int categoryWidth = 100;
            int categoryHeight = 25;
            int categorySpacing = 3;

            for (int i = 0; i < categories.Length; i++)
            {
                int catX = 20 + i * (categoryWidth + categorySpacing);
                bool isSelected = i == selectedCategory;

                uiManager.DrawPanel(catX, categoryY, categoryWidth, categoryHeight,
                    isSelected ? new Color4(0.4f, 0.4f, 0.4f, 0.9f) : new Color4(0.2f, 0.2f, 0.2f, 0.8f));

                int textWidth = uiManager.MeasureText(categories[i], 12);
                uiManager.DrawText(categories[i], catX + (categoryWidth - textWidth) / 2, categoryY + 6, 12,
                    isSelected ? Color4.White : new Color4(0.8f, 0.8f, 0.8f, 1f));
            }

            // 设置内容区域
            int contentX = 20;
            int contentY = 110;
            int contentWidth = screenWidth - 40;
            int contentHeight = screenHeight - 180;

            uiManager.DrawPanel(contentX, contentY, contentWidth, contentHeight,
                new Color4(0.1f, 0.1f, 0.15f, 0.9f));

            // 渲染当前分类的设置
            RenderCategoryOptions(uiManager, contentX + 10, contentY + 10, contentWidth - 20);

            // 完成按钮
            int doneBtnWidth = 150;
            int doneBtnHeight = 35;
            int doneBtnX = (screenWidth - doneBtnWidth) / 2;
            int doneBtnY = screenHeight - 55;

            uiManager.DrawPanel(doneBtnX, doneBtnY, doneBtnWidth, doneBtnHeight,
                new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("完成", doneBtnX + (doneBtnWidth - uiManager.MeasureText("完成", 16)) / 2,
                doneBtnY + 9, 16, Color4.White);
        }

        private void RenderCategoryOptions(UIManager uiManager, int x, int y, int width)
        {
            GameSettings settings = GameSettings.Instance;
            int optionHeight = 30;
            int optionSpacing = 5;
            int currentY = y;

            switch (selectedCategory)
            {
                case 0: // 视频设置
                    RenderSlider(uiManager, x, currentY, width, "渲染距离", settings.RenderDistance, 2, 32, "区块");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "视野", settings.FOV, 30, 110, "度");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "最大帧率", settings.MaxFps, 30, 240, "FPS");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "垂直同步", settings.VSync);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "全屏", settings.Fullscreen);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "平滑光照", settings.SmoothLighting);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "云", settings.Clouds, 0, 2, "");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "视角摇晃", settings.ViewBobbing);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示帧率", settings.ShowFps);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "界面大小", (int)(settings.GuiScale * 10), 10, 40, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "高级工具提示", settings.AdvancedTooltips);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "暂停时失去焦点", settings.PauseOnLostFocus);
                    break;

                case 1: // 音频设置
                    RenderSlider(uiManager, x, currentY, width, "主音量", (int)(settings.MasterVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "音乐音量", (int)(settings.MusicVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "音效音量", (int)(settings.SoundEffectsVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "环境音量", (int)(settings.AmbientVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "方块音量", (int)(settings.BlocksVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "敌对生物音量", (int)(settings.HostileVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "友好生物音量", (int)(settings.FriendlyVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "玩家音量", (int)(settings.PlayersVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "天气音量", (int)(settings.WeatherVolume * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示字幕", settings.Subtitles);
                    break;

                case 2: // 控制设置
                    RenderSlider(uiManager, x, currentY, width, "鼠标灵敏度", (int)(settings.MouseSensitivity * 100), 0, 200, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "反转鼠标", settings.InvertMouse);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "自动跳跃", settings.AutoJump);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "触屏模式", settings.TouchscreenMode);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "分离控制", settings.SplitControls);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "自动暂停", settings.AutoPause);
                    currentY += optionHeight + optionSpacing;
                    uiManager.DrawText("键位绑定:", x, currentY, 14, Color4.White);
                    currentY += 25;
                    RenderKeyBinding(uiManager, x, currentY, width, "前进", "W");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "后退", "S");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "左移", "A");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "右移", "D");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "跳跃", "空格");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "潜行", "Shift");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "疾跑", "Ctrl");
                    currentY += optionHeight + optionSpacing;
                    RenderKeyBinding(uiManager, x, currentY, width, "物品栏", "E");
                    break;

                case 3: // 语言设置
                    uiManager.DrawText("当前语言: " + settings.Language, x, currentY, 16, Color4.White);
                    currentY += 30;
                    string[] languages = { "简体中文 (zh_cn)", "繁體中文 (zh_tw)", "English (en_us)", "日本語 (ja_jp)", "한국어 (ko_kr)", "Deutsch (de_de)", "Français (fr_fr)", "Español (es_es)", "Русский (ru_ru)", "Português (pt_br)" };
                    for (int i = 0; i < languages.Length; i++)
                    {
                        bool isSelected = settings.Language == languages[i].Split(' ')[^1].Trim('(', ')');
                        uiManager.DrawPanel(x, currentY, width - 20, 25,
                            isSelected ? new Color4(0.3f, 0.3f, 0.3f, 0.9f) : new Color4(0.2f, 0.2f, 0.2f, 0.8f));
                        uiManager.DrawText(languages[i], x + 10, currentY + 5, 12,
                            isSelected ? Color4.White : new Color4(0.8f, 0.8f, 0.8f, 1f));
                        currentY += 30;
                    }
                    break;

                case 4: // 聊天设置
                    RenderToggle(uiManager, x, currentY, width, "聊天颜色", settings.ChatColors);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "聊天链接", settings.ChatLinks);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "聊天提示", settings.ChatPrompts);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "聊天不透明度", (int)(settings.ChatOpacity * 100), 0, 100, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "聊天大小", (int)(settings.ChatScale * 100), 50, 150, "%");
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "聊天行间距", settings.ChatLineSpacing, 0, 20, "px");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "减少聊天背景", settings.ReducedChatBackground);
                    break;

                case 5: // 皮肤设置
                    uiManager.DrawText("玩家名称: " + settings.PlayerName, x, currentY, 14, Color4.White);
                    currentY += 25;
                    uiManager.DrawText("皮肤模型: " + settings.SkinModel, x, currentY, 14, Color4.White);
                    currentY += 25;
                    uiManager.DrawText("披风: " + settings.Cape, x, currentY, 14, Color4.White);
                    currentY += 25;
                    RenderToggle(uiManager, x, currentY, width, "显示披风", settings.ShowCape);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示皮肤", settings.ShowSkin);
                    break;

                case 6: // 资源包
                    uiManager.DrawText("已加载资源包:", x, currentY, 14, Color4.White);
                    currentY += 25;
                    uiManager.DrawText("默认资源包 (已启用)", x + 20, currentY, 12, new Color4(0.7f, 0.7f, 0.7f, 1f));
                    currentY += 20;
                    uiManager.DrawText("点击\"打开资源包文件夹\"添加资源包", x, currentY + 20, 12,
                        new Color4(0.6f, 0.6f, 0.6f, 1f));
                    break;

                case 7: // 游戏设置
                    RenderToggle(uiManager, x, currentY, width, "自动保存", settings.AutoSave);
                    currentY += optionHeight + optionSpacing;
                    RenderSlider(uiManager, x, currentY, width, "自动保存间隔", settings.AutoSaveInterval, 1, 30, "分钟");
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示教程提示", settings.ShowTutorialHints);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示FPS", settings.ShowFps);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示坐标", settings.ShowCoordinates);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示生物群系", settings.ShowBiome);
                    currentY += optionHeight + optionSpacing;
                    RenderToggle(uiManager, x, currentY, width, "显示时间", settings.ShowTime);
                    break;
            }
        }

        private void RenderSlider(UIManager uiManager, int x, int y, int width, string label, int value, int min, int max, string unit)
        {
            uiManager.DrawText(label, x, y + 5, 14, Color4.White);

            int sliderX = x + 200;
            int sliderWidth = width - 300;
            int sliderHeight = 20;
            int sliderY = y + 5;

            uiManager.DrawPanel(sliderX, sliderY, sliderWidth, sliderHeight,
                new Color4(0.2f, 0.2f, 0.2f, 0.9f));

            float ratio = (float)(value - min) / (max - min);
            int fillWidth = (int)(sliderWidth * ratio);
            uiManager.DrawPanel(sliderX, sliderY, fillWidth, sliderHeight,
                new Color4(0.3f, 0.6f, 0.3f, 0.9f));

            int handleX = sliderX + fillWidth - 5;
            uiManager.DrawPanel(handleX, sliderY - 2, 10, sliderHeight + 4,
                new Color4(0.8f, 0.8f, 0.8f, 1f));

            string valueText = value + unit;
            uiManager.DrawText(valueText, sliderX + sliderWidth + 10, y + 5, 14, Color4.White);
        }

        private void RenderToggle(UIManager uiManager, int x, int y, int width, string label, bool value)
        {
            uiManager.DrawText(label, x, y + 5, 14, Color4.White);

            int toggleX = x + width - 80;
            int toggleWidth = 60;
            int toggleHeight = 20;
            int toggleY = y + 5;

            uiManager.DrawPanel(toggleX, toggleY, toggleWidth, toggleHeight,
                value ? new Color4(0.3f, 0.7f, 0.3f, 0.9f) : new Color4(0.4f, 0.2f, 0.2f, 0.9f));

            int handleX = value ? toggleX + toggleWidth - 18 : toggleX + 2;
            uiManager.DrawPanel(handleX, toggleY + 2, 16, toggleHeight - 4,
                new Color4(0.9f, 0.9f, 0.9f, 1f));

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

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 检查分类点击
            int categoryY = 70;
            int categoryWidth = 100;
            int categoryHeight = 25;
            int categorySpacing = 3;

            for (int i = 0; i < categories.Length; i++)
            {
                int catX = 20 + i * (categoryWidth + categorySpacing);

                if (mouseX >= catX && mouseX <= catX + categoryWidth &&
                    mouseY >= categoryY && mouseY <= categoryY + categoryHeight)
                {
                    selectedCategory = i;
                    selectedOption = 0;
                    return;
                }
            }

            // 检查完成按钮
            int doneBtnWidth = 150;
            int doneBtnHeight = 35;
            int doneBtnX = (screenWidth - doneBtnWidth) / 2;
            int doneBtnY = screenHeight - 55;

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

            switch (key)
            {
                case "Escape":
                    Close();
                    break;

                case "Up":
                    if (selectedOption > 0)
                    {
                        selectedOption--;
                    }
                    break;

                case "Down":
                    selectedOption++;
                    break;

                case "Left":
                    if (selectedCategory > 0)
                    {
                        selectedCategory--;
                        selectedOption = 0;
                    }
                    break;

                case "Right":
                    if (selectedCategory < categories.Length - 1)
                    {
                        selectedCategory++;
                        selectedOption = 0;
                    }
                    break;
            }
        }
    }
}
