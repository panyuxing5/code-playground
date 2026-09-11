using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Items
{
    public class CraftingRecipe
    {
        public string Name;
        public ItemStack Result;
        public int[,] Pattern; // 3x3 网格，0表示空，其他表示物品ID
        public bool Shapeless; // 是否无序合成
        public Dictionary<int, int> Ingredients; // 无序合成用：物品ID -> 数量
    }

    public class CraftingSystem
    {
        private static readonly List<CraftingRecipe> recipes = new List<CraftingRecipe>();
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllRecipes();
            Console.WriteLine("[CraftingSystem] 合成系统初始化完成，共注册 " + recipes.Count + " 个配方");
        }

        private static void RegisterAllRecipes()
        {
            // 基础合成
            RegisterBasicRecipes();
            // 工具合成
            RegisterToolRecipes();
            // 武器合成
            RegisterWeaponRecipes();
            // 护甲合成
            RegisterArmorRecipes();
            // 食物合成
            RegisterFoodRecipes();
            // 方块合成
            RegisterBlockRecipes();
            // 杂项合成
            RegisterMiscRecipes();
        }

        private static void RegisterBasicRecipes()
        {
            // 木板 (1原木 -> 4木板)
            recipes.Add(new CraftingRecipe
            {
                Name = "木板",
                Result = new ItemStack(5, 4),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 6, 1 } }
            });

            // 木棍 (2木板 -> 4木棍)
            recipes.Add(new CraftingRecipe
            {
                Name = "木棍",
                Result = new ItemStack(510, 4),
                Pattern = new int[,]
                {
                    { 5, 0, 0 },
                    { 5, 0, 0 },
                    { 0, 0, 0 }
                }
            });

            // 工作台 (4木板)
            recipes.Add(new CraftingRecipe
            {
                Name = "工作台",
                Result = new ItemStack(13, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 0 },
                    { 5, 5, 0 },
                    { 0, 0, 0 }
                }
            });

            // 熔炉 (8圆石)
            recipes.Add(new CraftingRecipe
            {
                Name = "熔炉",
                Result = new ItemStack(14, 1),
                Pattern = new int[,]
                {
                    { 4, 4, 4 },
                    { 4, 0, 4 },
                    { 4, 4, 4 }
                }
            });

            // 箱子 (8木板)
            recipes.Add(new CraftingRecipe
            {
                Name = "箱子",
                Result = new ItemStack(15, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 5 },
                    { 5, 0, 5 },
                    { 5, 5, 5 }
                }
            });

            // 火把 (1煤炭 + 1木棍 -> 4火把)
            recipes.Add(new CraftingRecipe
            {
                Name = "火把",
                Result = new ItemStack(16, 4),
                Pattern = new int[,]
                {
                    { 500, 0, 0 },
                    { 510, 0, 0 },
                    { 0, 0, 0 }
                }
            });

            // 梯子
            recipes.Add(new CraftingRecipe
            {
                Name = "梯子",
                Result = new ItemStack(2000, 3),
                Pattern = new int[,]
                {
                    { 510, 0, 510 },
                    { 510, 510, 510 },
                    { 510, 0, 510 }
                }
            });

            // 栅栏
            recipes.Add(new CraftingRecipe
            {
                Name = "栅栏",
                Result = new ItemStack(2001, 3),
                Pattern = new int[,]
                {
                    { 510, 5, 510 },
                    { 510, 5, 510 },
                    { 0, 0, 0 }
                }
            });

            // 栅栏门
            recipes.Add(new CraftingRecipe
            {
                Name = "栅栏门",
                Result = new ItemStack(2002, 1),
                Pattern = new int[,]
                {
                    { 510, 5, 510 },
                    { 510, 5, 510 },
                    { 0, 0, 0 }
                }
            });

            // 门
            recipes.Add(new CraftingRecipe
            {
                Name = "木门",
                Result = new ItemStack(2003, 3),
                Pattern = new int[,]
                {
                    { 5, 5, 0 },
                    { 5, 5, 0 },
                    { 5, 5, 0 }
                }
            });

            // 活板门
            recipes.Add(new CraftingRecipe
            {
                Name = "活板门",
                Result = new ItemStack(2004, 2),
                Pattern = new int[,]
                {
                    { 5, 5, 5 },
                    { 5, 5, 5 },
                    { 0, 0, 0 }
                }
            });

            // 按钮
            recipes.Add(new CraftingRecipe
            {
                Name = "木按钮",
                Result = new ItemStack(2005, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 5, 1 } }
            });

            // 压力板
            recipes.Add(new CraftingRecipe
            {
                Name = "木压力板",
                Result = new ItemStack(2006, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                }
            });
        }

        private static void RegisterToolRecipes()
        {
            // 木镐
            recipes.Add(new CraftingRecipe
            {
                Name = "木镐",
                Result = new ItemStack(100, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 5 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 石镐
            recipes.Add(new CraftingRecipe
            {
                Name = "石镐",
                Result = new ItemStack(101, 1),
                Pattern = new int[,]
                {
                    { 4, 4, 4 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 铁镐
            recipes.Add(new CraftingRecipe
            {
                Name = "铁镐",
                Result = new ItemStack(102, 1),
                Pattern = new int[,]
                {
                    { 502, 502, 502 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 金镐
            recipes.Add(new CraftingRecipe
            {
                Name = "金镐",
                Result = new ItemStack(103, 1),
                Pattern = new int[,]
                {
                    { 503, 503, 503 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 钻石镐
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石镐",
                Result = new ItemStack(104, 1),
                Pattern = new int[,]
                {
                    { 504, 504, 504 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 木斧
            recipes.Add(new CraftingRecipe
            {
                Name = "木斧",
                Result = new ItemStack(110, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 0 },
                    { 5, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 铁斧
            recipes.Add(new CraftingRecipe
            {
                Name = "铁斧",
                Result = new ItemStack(111, 1),
                Pattern = new int[,]
                {
                    { 502, 502, 0 },
                    { 502, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 钻石斧
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石斧",
                Result = new ItemStack(112, 1),
                Pattern = new int[,]
                {
                    { 504, 504, 0 },
                    { 504, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 木铲
            recipes.Add(new CraftingRecipe
            {
                Name = "木铲",
                Result = new ItemStack(120, 1),
                Pattern = new int[,]
                {
                    { 5, 0, 0 },
                    { 510, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 铁铲
            recipes.Add(new CraftingRecipe
            {
                Name = "铁铲",
                Result = new ItemStack(121, 1),
                Pattern = new int[,]
                {
                    { 502, 0, 0 },
                    { 510, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 钻石铲
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石铲",
                Result = new ItemStack(122, 1),
                Pattern = new int[,]
                {
                    { 504, 0, 0 },
                    { 510, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 木锄
            recipes.Add(new CraftingRecipe
            {
                Name = "木锄",
                Result = new ItemStack(130, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 0 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 铁锄
            recipes.Add(new CraftingRecipe
            {
                Name = "铁锄",
                Result = new ItemStack(131, 1),
                Pattern = new int[,]
                {
                    { 502, 502, 0 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });

            // 钻石锄
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石锄",
                Result = new ItemStack(132, 1),
                Pattern = new int[,]
                {
                    { 504, 504, 0 },
                    { 0, 510, 0 },
                    { 0, 510, 0 }
                }
            });
        }

        private static void RegisterWeaponRecipes()
        {
            // 木剑
            recipes.Add(new CraftingRecipe
            {
                Name = "木剑",
                Result = new ItemStack(200, 1),
                Pattern = new int[,]
                {
                    { 5, 0, 0 },
                    { 5, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 石剑
            recipes.Add(new CraftingRecipe
            {
                Name = "石剑",
                Result = new ItemStack(201, 1),
                Pattern = new int[,]
                {
                    { 4, 0, 0 },
                    { 4, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 铁剑
            recipes.Add(new CraftingRecipe
            {
                Name = "铁剑",
                Result = new ItemStack(202, 1),
                Pattern = new int[,]
                {
                    { 502, 0, 0 },
                    { 502, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 金剑
            recipes.Add(new CraftingRecipe
            {
                Name = "金剑",
                Result = new ItemStack(203, 1),
                Pattern = new int[,]
                {
                    { 503, 0, 0 },
                    { 503, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 钻石剑
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石剑",
                Result = new ItemStack(204, 1),
                Pattern = new int[,]
                {
                    { 504, 0, 0 },
                    { 504, 0, 0 },
                    { 510, 0, 0 }
                }
            });

            // 弓
            recipes.Add(new CraftingRecipe
            {
                Name = "弓",
                Result = new ItemStack(210, 1),
                Pattern = new int[,]
                {
                    { 511, 5, 0 },
                    { 511, 0, 5 },
                    { 511, 5, 0 }
                }
            });

            // 箭 (1燧石 + 1木棍 + 1羽毛 -> 4箭)
            recipes.Add(new CraftingRecipe
            {
                Name = "箭",
                Result = new ItemStack(211, 4),
                Pattern = new int[,]
                {
                    { 520, 0, 0 },
                    { 510, 0, 0 },
                    { 513, 0, 0 }
                }
            });

            // 盾牌
            recipes.Add(new CraftingRecipe
            {
                Name = "盾牌",
                Result = new ItemStack(220, 1),
                Pattern = new int[,]
                {
                    { 5, 502, 5 },
                    { 5, 5, 5 },
                    { 0, 5, 0 }
                }
            });
        }

        private static void RegisterArmorRecipes()
        {
            // 皮革头盔 (5皮革)
            recipes.Add(new CraftingRecipe
            {
                Name = "皮革头盔",
                Result = new ItemStack(300, 1),
                Pattern = new int[,]
                {
                    { 512, 512, 512 },
                    { 512, 0, 512 },
                    { 0, 0, 0 }
                }
            });

            // 皮革胸甲 (8皮革)
            recipes.Add(new CraftingRecipe
            {
                Name = "皮革胸甲",
                Result = new ItemStack(301, 1),
                Pattern = new int[,]
                {
                    { 512, 0, 512 },
                    { 512, 512, 512 },
                    { 512, 512, 512 }
                }
            });

            // 皮革护腿 (7皮革)
            recipes.Add(new CraftingRecipe
            {
                Name = "皮革护腿",
                Result = new ItemStack(302, 1),
                Pattern = new int[,]
                {
                    { 512, 512, 512 },
                    { 512, 0, 512 },
                    { 512, 0, 512 }
                }
            });

            // 皮革靴子 (4皮革)
            recipes.Add(new CraftingRecipe
            {
                Name = "皮革靴子",
                Result = new ItemStack(303, 1),
                Pattern = new int[,]
                {
                    { 512, 0, 512 },
                    { 512, 0, 512 },
                    { 0, 0, 0 }
                }
            });

            // 铁头盔
            recipes.Add(new CraftingRecipe
            {
                Name = "铁头盔",
                Result = new ItemStack(320, 1),
                Pattern = new int[,]
                {
                    { 502, 502, 502 },
                    { 502, 0, 502 },
                    { 0, 0, 0 }
                }
            });

            // 铁胸甲
            recipes.Add(new CraftingRecipe
            {
                Name = "铁胸甲",
                Result = new ItemStack(321, 1),
                Pattern = new int[,]
                {
                    { 502, 0, 502 },
                    { 502, 502, 502 },
                    { 502, 502, 502 }
                }
            });

            // 铁护腿
            recipes.Add(new CraftingRecipe
            {
                Name = "铁护腿",
                Result = new ItemStack(322, 1),
                Pattern = new int[,]
                {
                    { 502, 502, 502 },
                    { 502, 0, 502 },
                    { 502, 0, 502 }
                }
            });

            // 铁靴子
            recipes.Add(new CraftingRecipe
            {
                Name = "铁靴子",
                Result = new ItemStack(323, 1),
                Pattern = new int[,]
                {
                    { 502, 0, 502 },
                    { 502, 0, 502 },
                    { 0, 0, 0 }
                }
            });

            // 钻石头盔
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石头盔",
                Result = new ItemStack(340, 1),
                Pattern = new int[,]
                {
                    { 504, 504, 504 },
                    { 504, 0, 504 },
                    { 0, 0, 0 }
                }
            });

            // 钻石胸甲
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石胸甲",
                Result = new ItemStack(341, 1),
                Pattern = new int[,]
                {
                    { 504, 0, 504 },
                    { 504, 504, 504 },
                    { 504, 504, 504 }
                }
            });

            // 钻石护腿
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石护腿",
                Result = new ItemStack(342, 1),
                Pattern = new int[,]
                {
                    { 504, 504, 504 },
                    { 504, 0, 504 },
                    { 504, 0, 504 }
                }
            });

            // 钻石靴子
            recipes.Add(new CraftingRecipe
            {
                Name = "钻石靴子",
                Result = new ItemStack(343, 1),
                Pattern = new int[,]
                {
                    { 504, 0, 504 },
                    { 504, 0, 504 },
                    { 0, 0, 0 }
                }
            });
        }

        private static void RegisterFoodRecipes()
        {
            // 面包 (3小麦)
            recipes.Add(new CraftingRecipe
            {
                Name = "面包",
                Result = new ItemStack(401, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 530, 3 } }
            });

            // 曲奇 (2小麦 + 1可可豆)
            recipes.Add(new CraftingRecipe
            {
                Name = "曲奇",
                Result = new ItemStack(410, 8),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 530, 2 }, { 531, 1 } }
            });

            // 南瓜派 (1南瓜 + 1糖 + 1鸡蛋)
            recipes.Add(new CraftingRecipe
            {
                Name = "南瓜派",
                Result = new ItemStack(412, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 532, 1 }, { 533, 1 }, { 534, 1 } }
            });

            // 金苹果 (8金锭 + 1苹果)
            recipes.Add(new CraftingRecipe
            {
                Name = "金苹果",
                Result = new ItemStack(404, 1),
                Pattern = new int[,]
                {
                    { 503, 503, 503 },
                    { 503, 400, 503 },
                    { 503, 503, 503 }
                }
            });

            // 西瓜片 (1西瓜 -> 9片)
            recipes.Add(new CraftingRecipe
            {
                Name = "西瓜片",
                Result = new ItemStack(409, 9),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 535, 1 } }
            });

            // 糖 (1甘蔗)
            recipes.Add(new CraftingRecipe
            {
                Name = "糖",
                Result = new ItemStack(533, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 536, 1 } }
            });

            // 蛋糕
            recipes.Add(new CraftingRecipe
            {
                Name = "蛋糕",
                Result = new ItemStack(411, 1),
                Pattern = new int[,]
                {
                    { 534, 534, 534 },
                    { 533, 530, 533 },
                    { 5, 5, 5 }
                }
            });
        }

        private static void RegisterBlockRecipes()
        {
            // 石头 (4圆石 -> 1石头，需要熔炉，这里只是示例)
            // 实际应该用熔炉冶炼

            // 沙石 (4沙子)
            recipes.Add(new CraftingRecipe
            {
                Name = "沙石",
                Result = new ItemStack(2100, 1),
                Pattern = new int[,]
                {
                    { 7, 7, 0 },
                    { 7, 7, 0 },
                    { 0, 0, 0 }
                }
            });

            // 砖块 (4粘土)
            recipes.Add(new CraftingRecipe
            {
                Name = "砖块",
                Result = new ItemStack(2101, 1),
                Pattern = new int[,]
                {
                    { 540, 540, 0 },
                    { 540, 540, 0 },
                    { 0, 0, 0 }
                }
            });

            // 书架 (3书 + 6木板)
            recipes.Add(new CraftingRecipe
            {
                Name = "书架",
                Result = new ItemStack(2102, 1),
                Pattern = new int[,]
                {
                    { 5, 5, 5 },
                    { 706, 706, 706 },
                    { 5, 5, 5 }
                }
            });

            // 黑曜石无法合成，只能用钻石镐开采

            // 萤石 (4萤石粉)
            recipes.Add(new CraftingRecipe
            {
                Name = "萤石",
                Result = new ItemStack(2103, 1),
                Pattern = new int[,]
                {
                    { 550, 550, 0 },
                    { 550, 550, 0 },
                    { 0, 0, 0 }
                }
            });
        }

        private static void RegisterMiscRecipes()
        {
            // 纸 (3甘蔗)
            recipes.Add(new CraftingRecipe
            {
                Name = "纸",
                Result = new ItemStack(707, 3),
                Pattern = new int[,]
                {
                    { 536, 536, 536 },
                    { 0, 0, 0 },
                    { 0, 0, 0 }
                }
            });

            // 书 (3纸 + 1皮革)
            recipes.Add(new CraftingRecipe
            {
                Name = "书",
                Result = new ItemStack(706, 1),
                Pattern = new int[,]
                {
                    { 707, 707, 0 },
                    { 707, 512, 0 },
                    { 0, 0, 0 }
                }
            });

            // 地图 (8纸 + 1指南针)
            recipes.Add(new CraftingRecipe
            {
                Name = "地图",
                Result = new ItemStack(705, 1),
                Pattern = new int[,]
                {
                    { 707, 707, 707 },
                    { 707, 703, 707 },
                    { 707, 707, 707 }
                }
            });

            // 指南针 (4铁锭 + 1红石)
            recipes.Add(new CraftingRecipe
            {
                Name = "指南针",
                Result = new ItemStack(703, 1),
                Pattern = new int[,]
                {
                    { 0, 502, 0 },
                    { 502, 506, 502 },
                    { 0, 502, 0 }
                }
            });

            // 时钟 (4金锭 + 1红石)
            recipes.Add(new CraftingRecipe
            {
                Name = "时钟",
                Result = new ItemStack(704, 1),
                Pattern = new int[,]
                {
                    { 0, 503, 0 },
                    { 503, 506, 503 },
                    { 0, 503, 0 }
                }
            });

            // 钓鱼竿 (3木棍 + 2线)
            recipes.Add(new CraftingRecipe
            {
                Name = "钓鱼竿",
                Result = new ItemStack(710, 1),
                Pattern = new int[,]
                {
                    { 0, 0, 510 },
                    { 0, 510, 511 },
                    { 510, 0, 511 }
                }
            });

            // 打火石 (1铁锭 + 1燧石)
            recipes.Add(new CraftingRecipe
            {
                Name = "打火石",
                Result = new ItemStack(711, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 502, 1 }, { 520, 1 } }
            });

            // 剪刀 (2铁锭)
            recipes.Add(new CraftingRecipe
            {
                Name = "剪刀",
                Result = new ItemStack(712, 1),
                Pattern = new int[,]
                {
                    { 502, 0, 0 },
                    { 0, 502, 0 },
                    { 0, 0, 0 }
                }
            });

            // 拴绳 (4线 + 1史莱姆球)
            recipes.Add(new CraftingRecipe
            {
                Name = "拴绳",
                Result = new ItemStack(713, 2),
                Pattern = new int[,]
                {
                    { 511, 511, 0 },
                    { 511, 560, 0 },
                    { 0, 0, 511 }
                }
            });

            // 末影眼 (1末影珍珠 + 1烈焰粉)
            recipes.Add(new CraftingRecipe
            {
                Name = "末影眼",
                Result = new ItemStack(518, 1),
                Shapeless = true,
                Ingredients = new Dictionary<int, int> { { 516, 1 }, { 570, 1 } }
            });
        }

        public static ItemStack Craft(ItemStack[] craftingGrid)
        {
            if (craftingGrid == null || craftingGrid.Length != 9) return null;

            // 检查有序配方
            foreach (CraftingRecipe recipe in recipes)
            {
                if (recipe.Shapeless) continue;
                if (MatchesShapedRecipe(craftingGrid, recipe))
                {
                    return recipe.Result.Copy();
                }
            }

            // 检查无序配方
            foreach (CraftingRecipe recipe in recipes)
            {
                if (!recipe.Shapeless) continue;
                if (MatchesShapelessRecipe(craftingGrid, recipe))
                {
                    return recipe.Result.Copy();
                }
            }

            return null;
        }

        private static bool MatchesShapedRecipe(ItemStack[] grid, CraftingRecipe recipe)
        {
            // 尝试所有可能的偏移
            for (int offsetX = 0; offsetX <= 1; offsetX++)
            {
                for (int offsetY = 0; offsetY <= 1; offsetY++)
                {
                    bool match = true;

                    for (int x = 0; x < 3; x++)
                    {
                        for (int y = 0; y < 3; y++)
                        {
                            int gridIndex = y * 3 + x;
                            int recipeX = x - offsetX;
                            int recipeY = y - offsetY;

                            int requiredItem = 0;
                            if (recipeX >= 0 && recipeX < 3 && recipeY >= 0 && recipeY < 3)
                            {
                                requiredItem = recipe.Pattern[recipeY, recipeX];
                            }

                            int gridItem = grid[gridIndex] != null && !grid[gridIndex].IsEmpty() ? grid[gridIndex].ItemId : 0;

                            if (gridItem != requiredItem)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (!match) break;
                    }

                    if (match) return true;
                }
            }

            return false;
        }

        private static bool MatchesShapelessRecipe(ItemStack[] grid, CraftingRecipe recipe)
        {
            Dictionary<int, int> gridItems = new Dictionary<int, int>();

            foreach (ItemStack stack in grid)
            {
                if (stack != null && !stack.IsEmpty())
                {
                    if (!gridItems.ContainsKey(stack.ItemId))
                    {
                        gridItems[stack.ItemId] = 0;
                    }
                    gridItems[stack.ItemId] += stack.Count;
                }
            }

            if (gridItems.Count != recipe.Ingredients.Count) return false;

            foreach (var kvp in recipe.Ingredients)
            {
                if (!gridItems.ContainsKey(kvp.Key)) return false;
                if (gridItems[kvp.Key] < kvp.Value) return false;
            }

            return true;
        }

        public static List<CraftingRecipe> GetAllRecipes()
        {
            return new List<CraftingRecipe>(recipes);
        }

        public static List<CraftingRecipe> GetRecipesForItem(int itemId)
        {
            List<CraftingRecipe> result = new List<CraftingRecipe>();
            foreach (CraftingRecipe recipe in recipes)
            {
                if (recipe.Result.ItemId == itemId)
                {
                    result.Add(recipe);
                }
            }
            return result;
        }
    }
}
