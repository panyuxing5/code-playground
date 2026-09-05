using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class CreditsScreen
    {
        private bool isOpen;
        private float scrollPosition;
        private readonly List<CreditSection> sections;

        // 事件
        public event Action OnClosed;

        public bool IsOpen => isOpen;

        public CreditsScreen()
        {
            sections = new List<CreditSection>();
            scrollPosition = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("[CreditsScreen] 制作人员名单初始化完成");
            LoadCredits();
        }

        private void LoadCredits()
        {
            // 开发团队
            sections.Add(new CreditSection
            {
                Title = "开发团队",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "项目负责人" },
                    new CreditItem { Name = "OpenTK", Role = "图形库" },
                    new CreditItem { Name = "C#", Role = "编程语言" },
                    new CreditItem { Name = ".NET 9", Role = "运行时" }
                }
            });

            // 游戏设计
            sections.Add(new CreditSection
            {
                Title = "游戏设计",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "游戏设计" },
                    new CreditItem { Name = "Minecraft", Role = "灵感来源" },
                    new CreditItem { Name = "社区", Role = "反馈与建议" }
                }
            });

            // 程序开发
            sections.Add(new CreditSection
            {
                Title = "程序开发",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "主程序" },
                    new CreditItem { Name = "OpenTK Contributors", Role = "OpenGL绑定" },
                    new CreditItem { Name = "OpenAL", Role = "音频库" }
                }
            });

            // 美术资源
            sections.Add(new CreditSection
            {
                Title = "美术资源",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "像素艺术" },
                    new CreditItem { Name = "Mojang", Role = "原版材质参考" },
                    new CreditItem { Name = "社区", Role = "资源包创作" }
                }
            });

            // 音频资源
            sections.Add(new CreditSection
            {
                Title = "音频资源",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "音效设计" },
                    new CreditItem { Name = "OpenAL Soft", Role = "音频引擎" },
                    new CreditItem { Name = "社区", Role = "音乐创作" }
                }
            });

            // 测试人员
            sections.Add(new CreditSection
            {
                Title = "测试人员",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "Alpha Testers", Role = "内部测试" },
                    new CreditItem { Name = "Beta Testers", Role = "公开测试" },
                    new CreditItem { Name = "社区", Role = "Bug报告" }
                }
            });

            // 翻译团队
            sections.Add(new CreditSection
            {
                Title = "翻译团队",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft Team", Role = "简体中文" },
                    new CreditItem { Name = "社区贡献者", Role = "繁體中文" },
                    new CreditItem { Name = "社区贡献者", Role = "English" },
                    new CreditItem { Name = "社区贡献者", Role = "日本語" },
                    new CreditItem { Name = "社区贡献者", Role = "한국어" },
                    new CreditItem { Name = "社区贡献者", Role = "Deutsch" },
                    new CreditItem { Name = "社区贡献者", Role = "Français" },
                    new CreditItem { Name = "社区贡献者", Role = "Español" },
                    new CreditItem { Name = "社区贡献者", Role = "Русский" },
                    new CreditItem { Name = "社区贡献者", Role = "Português" }
                }
            });

            // 特别感谢
            sections.Add(new CreditSection
            {
                Title = "特别感谢",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "Notch", Role = "Minecraft创作者" },
                    new CreditItem { Name = "Mojang Studios", Role = "Minecraft开发" },
                    new CreditItem { Name = "Microsoft", Role = "Minecraft所有者" },
                    new CreditItem { Name = "所有玩家", Role = "支持与鼓励" }
                }
            });

            // 开源项目
            sections.Add(new CreditSection
            {
                Title = "开源项目",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "OpenTK", Role = "MIT License" },
                    new CreditItem { Name = "OpenAL Soft", Role = "LGPL License" },
                    new CreditItem { Name = ".NET Runtime", Role = "MIT License" },
                    new CreditItem { Name = "C# Compiler", Role = "MIT License" }
                }
            });

            // 社区
            sections.Add(new CreditSection
            {
                Title = "社区",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "Discord社区", Role = "讨论与反馈" },
                    new CreditItem { Name = "Reddit", Role = "社区讨论" },
                    new CreditItem { Name = "GitHub", Role = "代码贡献" },
                    new CreditItem { Name = "Wiki", Role = "文档编写" }
                }
            });

            // 结尾
            sections.Add(new CreditSection
            {
                Title = "感谢游玩！",
                Items = new List<CreditItem>
                {
                    new CreditItem { Name = "VoxelCraft v1.0.0", Role = "" },
                    new CreditItem { Name = "© 2026 VoxelCraft Team", Role = "" },
                    new CreditItem { Name = "All rights reserved", Role = "" }
                }
            });
        }

        public void Open()
        {
            isOpen = true;
            scrollPosition = 0;
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

        public void Update(float deltaTime)
        {
            if (!isOpen) return;

            // 自动滚动
            scrollPosition += deltaTime * 30;
        }

        public void Render(UIManager uiManager)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.05f, 0.05f, 0.1f, 1f));

            // 计算总高度
            int totalHeight = 0;
            foreach (CreditSection section in sections)
            {
                totalHeight += 50; // 标题高度
                totalHeight += section.Items.Count * 25; // 项目高度
                totalHeight += 30; // 间距
            }

            // 渲染滚动内容
            int currentY = screenHeight - (int)scrollPosition;

            foreach (CreditSection section in sections)
            {
                // 标题
                if (currentY > -50 && currentY < screenHeight + 50)
                {
                    int sectionTitleWidth = uiManager.MeasureText(section.Title, 24);
                    uiManager.DrawText(section.Title, (screenWidth - sectionTitleWidth) / 2, currentY, 24,
                        new Color4(1.0f, 0.9f, 0.5f, 1f));
                }
                currentY += 50;

                // 项目
                foreach (CreditItem item in section.Items)
                {
                    if (currentY > -25 && currentY < screenHeight + 25)
                    {
                        string nameText = item.Name;
                        string roleText = item.Role;

                        int nameWidth = uiManager.MeasureText(nameText, 16);
                        uiManager.DrawText(nameText, (screenWidth - nameWidth) / 2 - 100, currentY, 16, Color4.White);

                        if (!string.IsNullOrEmpty(roleText))
                        {
                            uiManager.DrawText(roleText, (screenWidth - nameWidth) / 2 + 80, currentY, 14,
                                new Color4(0.7f, 0.7f, 0.7f, 1f));
                        }
                    }
                    currentY += 25;
                }

                currentY += 30;
            }

            // 顶部渐变遮罩
            uiManager.DrawPanel(0, 0, screenWidth, 80,
                new Color4(0.05f, 0.05f, 0.1f, 0.9f));

            // 底部渐变遮罩
            uiManager.DrawPanel(0, screenHeight - 80, screenWidth, 80,
                new Color4(0.05f, 0.05f, 0.1f, 0.9f));

            // 标题
            string title = "制作人员名单";
            int titleWidth = uiManager.MeasureText(title, 32);
            uiManager.DrawText(title, (screenWidth - titleWidth) / 2, 20, 32, Color4.White);

            // 关闭提示
            string closeText = "按 ESC 或点击任意位置关闭";
            int closeWidth = uiManager.MeasureText(closeText, 14);
            uiManager.DrawText(closeText, (screenWidth - closeWidth) / 2, screenHeight - 30, 14,
                new Color4(0.6f, 0.6f, 0.6f, 1f));

            // 滚动到末尾后重置
            if (scrollPosition > totalHeight + screenHeight)
            {
                scrollPosition = 0;
            }
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isOpen) return;
            Close();
        }

        public void HandleKeyPress(string key)
        {
            if (!isOpen) return;

            if (key == "Escape")
            {
                Close();
            }
        }

        public void HandleScroll(float delta)
        {
            if (!isOpen) return;
            scrollPosition += delta * 50;
            scrollPosition = Math.Max(0, scrollPosition);
        }
    }

    public class CreditSection
    {
        public string Title;
        public List<CreditItem> Items;

        public CreditSection()
        {
            Items = new List<CreditItem>();
        }
    }

    public class CreditItem
    {
        public string Name;
        public string Role;
    }

    public class StatisticsScreen
    {
        private bool isOpen;
        private int selectedCategory;
        private float scrollOffset;

        private readonly string[] categories = { "通用", "方块", "物品", "生物", "冒险", "玩家" };

        // 事件
        public event Action OnClosed;

        public bool IsOpen => isOpen;

        public StatisticsScreen()
        {
            selectedCategory = 0;
            scrollOffset = 0;
        }

        public void Initialize()
        {
            Console.WriteLine("[StatisticsScreen] 统计界面初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedCategory = 0;
            scrollOffset = 0;
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

        public void Render(UIManager uiManager, StatisticsSystem statistics)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.8f));

            // 标题
            string title = "统计";
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

            // 统计内容区域
            int listX = 20;
            int listY = 110;
            int listWidth = screenWidth - 40;
            int listHeight = screenHeight - 180;

            uiManager.DrawPanel(listX, listY, listWidth, listHeight,
                new Color4(0.1f, 0.1f, 0.15f, 0.9f));

            // 获取统计数据
            Dictionary<string, long> stats = GetStatisticsByCategory(statistics, selectedCategory);

            // 渲染统计项
            int itemY = listY + 10;
            int itemHeight = 25;
            int itemSpacing = 2;

            foreach (KeyValuePair<string, long> stat in stats)
            {
                int currentY = itemY - (int)scrollOffset;
                if (currentY < listY || currentY > listY + listHeight - itemHeight)
                {
                    itemY += itemHeight + itemSpacing;
                    continue;
                }

                // 统计项背景
                uiManager.DrawPanel(listX + 10, currentY, listWidth - 20, itemHeight,
                    new Color4(0.2f, 0.2f, 0.25f, 0.9f));

                // 统计名称
                uiManager.DrawText(stat.Key, listX + 20, currentY + 5, 14, Color4.White);

                // 统计值
                string valueText = FormatStatValue(stat.Key, stat.Value);
                int valueWidth = uiManager.MeasureText(valueText, 14);
                uiManager.DrawText(valueText, listX + listWidth - valueWidth - 30, currentY + 5, 14,
                    new Color4(0.8f, 0.8f, 0.5f, 1f));

                itemY += itemHeight + itemSpacing;
            }

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

        private Dictionary<string, long> GetStatisticsByCategory(StatisticsSystem statistics, int category)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();

            switch (category)
            {
                case 0: // 通用
                    result["游戏时间"] = statistics.PlayTimeTicks;
                    result["游戏天数"] = statistics.DaysPlayed;
                    result["死亡次数"] = statistics.Deaths;
                    result["跳跃次数"] = statistics.Jumps;
                    result["行走距离"] = statistics.DistanceWalked;
                    result["游泳距离"] = statistics.DistanceSwum;
                    result["飞行距离"] = statistics.DistanceFlown;
                    result["下落距离"] = statistics.DistanceFallen;
                    result["潜水时间"] = statistics.TimeUnderwater;
                    break;

                case 1: // 方块
                    result["挖掘方块数"] = statistics.BlocksMined;
                    result["放置方块数"] = statistics.BlocksPlaced;
                    result["工作台使用次数"] = statistics.CraftingTableUsed;
                    result["熔炉使用次数"] = statistics.FurnaceUsed;
                    break;

                case 2: // 物品
                    result["获得物品数"] = statistics.ItemsPickedUp;
                    result["丢弃物品数"] = statistics.ItemsDropped;
                    result["使用物品数"] = statistics.ItemsUsed;
                    result["损坏物品数"] = statistics.ItemsBroken;
                    break;

                case 3: // 生物
                    result["击杀生物数"] = statistics.MobsKilled;
                    result["被生物杀死次数"] = statistics.DeathsByMobs;
                    result["繁殖动物数"] = statistics.AnimalsBred;
                    break;

                case 4: // 冒险
                    result["探索生物群系数"] = statistics.BiomesExplored;
                    result["到达最高高度"] = statistics.MaxHeightReached;
                    result["交易次数"] = statistics.TradesCompleted;
                    break;

                case 5: // 玩家
                    result["伤害 dealt"] = (long)statistics.DamageDealt;
                    result["伤害 taken"] = (long)statistics.DamageTaken;
                    result["恢复生命值"] = statistics.HealthRestored;
                    result["吃食物次数"] = statistics.FoodEaten;
                    break;
            }

            return result;
        }

        private string FormatStatValue(string key, long value)
        {
            switch (key)
            {
                case "游戏时间":
                    long hours = value / 20 / 3600;
                    long minutes = (value / 20 / 60) % 60;
                    return $"{hours}小时{minutes}分钟";

                case "行走距离":
                case "游泳距离":
                case "飞行距离":
                case "下落距离":
                    return $"{value / 100.0:F1} 米";

                case "潜水时间":
                    return $"{value / 20} 秒";

                default:
                    return value.ToString();
            }
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

            if (key == "Escape")
            {
                Close();
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
