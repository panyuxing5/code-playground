using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.UI
{
    public class AchievementScreen
    {
        private bool isOpen;
        private int selectedCategory;
        private int selectedAchievement;
        private float scrollOffset;

        private readonly string[] categories = { "Minecraft", "农业", "冒险", "末地", "下界", "装饰", "挖矿", "红石", "运输", "极寒" };

        // 事件
        public event Action OnClosed;

        public bool IsOpen => isOpen;

        public AchievementScreen()
        {
            selectedCategory = 0;
            selectedAchievement = 0;
            scrollOffset = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("[AchievementScreen] 成就界面初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedCategory = 0;
            selectedAchievement = 0;
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

        public void Render(UIManager uiManager, AchievementSystem achievementSystem)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.8f));

            // 标题
            string title = "进度";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 20, 32, Color4.White);

            // 进度统计
            int totalAchievements = achievementSystem.TotalAchievements;
            int unlockedAchievements = achievementSystem.UnlockedAchievements;
            string progress = $"已完成: {unlockedAchievements}/{totalAchievements} ({(int)((float)unlockedAchievements / totalAchievements * 100)}%)";
            int progressWidth = uiManager.MeasureText(progress, 16);
            uiManager.DrawText(progress, (screenWidth - progressWidth) / 2, 60, 16, new Color4(0.8f, 0.8f, 0.8f, 1f));

            // 分类标签
            int categoryY = 90;
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

            // 成就列表区域
            int listX = 20;
            int listY = 130;
            int listWidth = screenWidth - 40;
            int listHeight = screenHeight - 200;

            uiManager.DrawPanel(listX, listY, listWidth, listHeight,
                new Color4(0.1f, 0.1f, 0.1f, 0.9f));

            // 获取当前分类的成就
            List<Achievement> achievements = achievementSystem.GetAchievementsByCategory(categories[selectedCategory]);

            // 渲染成就
            int achievementY = listY + 10;
            int achievementHeight = 50;
            int achievementSpacing = 5;

            for (int i = 0; i < achievements.Count; i++)
            {
                Achievement achievement = achievements[i];
                int currentY = achievementY + i * (achievementHeight + achievementSpacing) - (int)scrollOffset;

                if (currentY < listY || currentY > listY + listHeight - achievementHeight)
                {
                    continue;
                }

                bool isSelected = i == selectedAchievement;
                bool isUnlocked = achievement.IsUnlocked;

                // 成就背景
                uiManager.DrawPanel(listX + 10, currentY, listWidth - 20, achievementHeight,
                    isSelected ? new Color4(0.3f, 0.3f, 0.3f, 0.9f) :
                    isUnlocked ? new Color4(0.2f, 0.25f, 0.2f, 0.9f) :
                    new Color4(0.15f, 0.15f, 0.15f, 0.9f));

                // 成就图标位置
                int iconX = listX + 20;
                int iconY = currentY + 5;

                // 图标背景
                uiManager.DrawPanel(iconX, iconY, 40, 40,
                    isUnlocked ? new Color4(0.3f, 0.5f, 0.3f, 1f) : new Color4(0.3f, 0.3f, 0.3f, 1f));

                // 成就名称
                uiManager.DrawText(achievement.Name, listX + 70, currentY + 8, 14,
                    isUnlocked ? Color4.White : new Color4(0.6f, 0.6f, 0.6f, 1f));

                // 成就描述
                uiManager.DrawText(achievement.Description, listX + 70, currentY + 26, 11,
                    new Color4(0.7f, 0.7f, 0.7f, 1f));

                // 完成状态
                string status = isUnlocked ? "已完成" : "未完成";
                int statusWidth = uiManager.MeasureText(status, 12);
                uiManager.DrawText(status, listX + listWidth - statusWidth - 30, currentY + 18, 12,
                    isUnlocked ? new Color4(0.3f, 1.0f, 0.3f, 1f) : new Color4(0.8f, 0.3f, 0.3f, 1f));

                // 完成时间
                if (isUnlocked && achievement.UnlockTime.HasValue)
                {
                    string time = achievement.UnlockTime.Value.ToString("yyyy-MM-dd HH:mm");
                    int timeWidth = uiManager.MeasureText(time, 10);
                    uiManager.DrawText(time, listX + listWidth - timeWidth - 30, currentY + 35, 10,
                        new Color4(0.5f, 0.5f, 0.5f, 1f));
                }
            }

            // 滚动条
            int scrollbarX = listX + listWidth - 10;
            int scrollbarY = listY + 5;
            int scrollbarHeight = listHeight - 10;
            float scrollRatio = achievements.Count > 0 ? (float)listHeight / (achievements.Count * (achievementHeight + achievementSpacing)) : 1;
            int thumbHeight = (int)(scrollbarHeight * Math.Min(1, scrollRatio));
            int thumbY = scrollbarY + (int)(scrollOffset / (achievements.Count * (achievementHeight + achievementSpacing)) * (scrollbarHeight - thumbHeight));

            uiManager.DrawPanel(scrollbarX, scrollbarY, 5, scrollbarHeight,
                new Color4(0.2f, 0.2f, 0.2f, 0.8f));
            uiManager.DrawPanel(scrollbarX, thumbY, 5, thumbHeight,
                new Color4(0.5f, 0.5f, 0.5f, 0.9f));

            // 关闭按钮
            int closeBtnWidth = 100;
            int closeBtnHeight = 30;
            int closeBtnX = (screenWidth - closeBtnWidth) / 2;
            int closeBtnY = screenHeight - 50;

            uiManager.DrawPanel(closeBtnX, closeBtnY, closeBtnWidth, closeBtnHeight,
                new Color4(0.3f, 0.3f, 0.3f, 0.9f));
            uiManager.DrawText("完成", closeBtnX + (closeBtnWidth - uiManager.MeasureText("完成", 14)) / 2,
                closeBtnY + 8, 14, Color4.White);
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 检查分类点击
            int categoryY = 90;
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
                    selectedAchievement = 0;
                    scrollOffset = 0;
                    return;
                }
            }

            // 检查关闭按钮
            int closeBtnWidth = 100;
            int closeBtnHeight = 30;
            int closeBtnX = (screenWidth - closeBtnWidth) / 2;
            int closeBtnY = screenHeight - 50;

            if (mouseX >= closeBtnX && mouseX <= closeBtnX + closeBtnWidth &&
                mouseY >= closeBtnY && mouseY <= closeBtnY + closeBtnHeight)
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
                    if (selectedAchievement > 0)
                    {
                        selectedAchievement--;
                    }
                    break;

                case "Down":
                    selectedAchievement++;
                    break;

                case "Left":
                    if (selectedCategory > 0)
                    {
                        selectedCategory--;
                        selectedAchievement = 0;
                    }
                    break;

                case "Right":
                    if (selectedCategory < categories.Length - 1)
                    {
                        selectedCategory++;
                        selectedAchievement = 0;
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
    }
}
