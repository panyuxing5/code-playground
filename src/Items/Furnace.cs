using System;
using System.Collections.Generic;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.Items
{
    public class FurnaceRecipe
    {
        public int InputId;
        public int OutputId;
        public int OutputCount;
        public float Experience;
        public int CookTime;
    }

    public class Furnace
    {
        private static readonly Dictionary<int, FurnaceRecipe> recipes = new Dictionary<int, FurnaceRecipe>();
        private static bool isInitialized;

        public ItemStack InputSlot { get; set; }
        public ItemStack FuelSlot { get; set; }
        public ItemStack OutputSlot { get; set; }

        public int BurnTime { get; private set; }
        public int CurrentBurnTime { get; private set; }
        public int CookTime { get; private set; }
        public int TotalCookTime { get; private set; }

        public bool IsBurning => CurrentBurnTime > 0;
        public bool IsCooking => InputSlot != null && !InputSlot.IsEmpty() && CanCook();

        public Furnace()
        {
            InputSlot = new ItemStack(0, 0);
            FuelSlot = new ItemStack(0, 0);
            OutputSlot = new ItemStack(0, 0);
        }

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllRecipes();
            Console.WriteLine("[Furnace] 熔炉系统初始化完成，共注册 " + recipes.Count + " 个配方");
        }

        private static void RegisterAllRecipes()
        {
            // 矿石冶炼
            recipes[GameConstants.BLOCK_IRON_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_IRON_ORE,
                OutputId = 502, // 铁锭
                OutputCount = 1,
                Experience = 0.7f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_GOLD_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_GOLD_ORE,
                OutputId = 503, // 金锭
                OutputCount = 1,
                Experience = 1.0f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_COAL_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_COAL_ORE,
                OutputId = 500, // 煤炭
                OutputCount = 1,
                Experience = 0.1f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_DIAMOND_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_DIAMOND_ORE,
                OutputId = 504, // 钻石
                OutputCount = 1,
                Experience = 1.0f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_REDSTONE_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_REDSTONE_ORE,
                OutputId = 506, // 红石
                OutputCount = 1,
                Experience = 0.7f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_LAPIS_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_LAPIS_ORE,
                OutputId = 507, // 青金石
                OutputCount = 1,
                Experience = 0.2f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_EMERALD_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_EMERALD_ORE,
                OutputId = 508, // 绿宝石
                OutputCount = 1,
                Experience = 1.0f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_COPPER_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_COPPER_ORE,
                OutputId = 580, // 铜锭
                OutputCount = 1,
                Experience = 0.7f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_NETHER_QUARTZ_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_NETHER_QUARTZ_ORE,
                OutputId = 509, // 石英
                OutputCount = 1,
                Experience = 0.2f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_NETHER_GOLD_ORE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_NETHER_GOLD_ORE,
                OutputId = 503, // 金粒（简化为金锭）
                OutputCount = 1,
                Experience = 0.1f,
                CookTime = 200
            };

            // 食物烹饪
            recipes[590] = new FurnaceRecipe // 生牛肉
            {
                InputId = 590,
                OutputId = 402, // 牛排
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[591] = new FurnaceRecipe // 生猪排
            {
                InputId = 591,
                OutputId = 403, // 烤猪排
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[592] = new FurnaceRecipe // 生鸡肉
            {
                InputId = 592,
                OutputId = 593, // 熟鸡肉
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[594] = new FurnaceRecipe // 生羊肉
            {
                InputId = 594,
                OutputId = 595, // 熟羊肉
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[596] = new FurnaceRecipe // 生兔肉
            {
                InputId = 596,
                OutputId = 597, // 熟兔肉
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[598] = new FurnaceRecipe // 生鳕鱼
            {
                InputId = 598,
                OutputId = 599, // 熟鳕鱼
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            recipes[407] = new FurnaceRecipe // 马铃薯
            {
                InputId = 407,
                OutputId = 408, // 烤马铃薯
                OutputCount = 1,
                Experience = 0.35f,
                CookTime = 200
            };

            // 方块烧制
            recipes[GameConstants.BLOCK_STONE] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_STONE,
                OutputId = GameConstants.BLOCK_STONE_BRICKS, // 石砖（简化）
                OutputCount = 1,
                Experience = 0.1f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_CLAY] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_CLAY,
                OutputId = GameConstants.BLOCK_BRICKS, // 砖块
                OutputCount = 1,
                Experience = 0.3f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_SAND] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_SAND,
                OutputId = GameConstants.BLOCK_GLASS, // 玻璃
                OutputCount = 1,
                Experience = 0.1f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_RED_SAND] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_RED_SAND,
                OutputId = GameConstants.BLOCK_GLASS, // 玻璃（简化）
                OutputCount = 1,
                Experience = 0.1f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_LOG] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_LOG,
                OutputId = 501, // 木炭
                OutputCount = 1,
                Experience = 0.15f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_CACTUS] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_CACTUS,
                OutputId = 600, // 仙人掌绿
                OutputCount = 1,
                Experience = 1.0f,
                CookTime = 200
            };

            recipes[GameConstants.BLOCK_OBSIDIAN] = new FurnaceRecipe
            {
                InputId = GameConstants.BLOCK_OBSIDIAN,
                OutputId = 601, // 黑曜石（简化）
                OutputCount = 1,
                Experience = 0.0f,
                CookTime = 200
            };

            // 其他
            recipes[610] = new FurnaceRecipe // 湿海绵
            {
                InputId = 610,
                OutputId = 611, // 干海绵
                OutputCount = 1,
                Experience = 0.15f,
                CookTime = 200
            };

            recipes[612] = new FurnaceRecipe // 青金石矿石
            {
                InputId = 612,
                OutputId = 507,
                OutputCount = 1,
                Experience = 0.2f,
                CookTime = 200
            };
        }

        public void Update()
        {
            bool wasBurning = IsBurning;

            // 燃烧燃料
            if (CurrentBurnTime > 0)
            {
                CurrentBurnTime--;
            }

            // 如果没有燃烧且有可烹饪物品，尝试消耗燃料
            if (!IsBurning && CanCook() && HasFuel())
            {
                BurnFuel();
            }

            // 烹饪
            if (IsBurning && CanCook())
            {
                CookTime++;
                TotalCookTime = GetCookTime();

                if (CookTime >= TotalCookTime)
                {
                    CookItem();
                    CookTime = 0;
                }
            }
            else
            {
                CookTime = 0;
            }
        }

        private bool CanCook()
        {
            if (InputSlot == null || InputSlot.IsEmpty()) return false;
            if (!recipes.TryGetValue(InputSlot.ItemId, out FurnaceRecipe recipe)) return false;

            // 检查输出槽
            if (OutputSlot == null || OutputSlot.IsEmpty()) return true;
            if (OutputSlot.ItemId != recipe.OutputId) return false;
            return OutputSlot.Count + recipe.OutputCount <= OutputSlot.GetMaxStackSize();
        }

        private bool HasFuel()
        {
            if (FuelSlot == null || FuelSlot.IsEmpty()) return false;
            return GetFuelTime(FuelSlot.ItemId) > 0;
        }

        private void BurnFuel()
        {
            if (FuelSlot == null || FuelSlot.IsEmpty()) return;

            int fuelTime = GetFuelTime(FuelSlot.ItemId);
            if (fuelTime > 0)
            {
                CurrentBurnTime = fuelTime;
                BurnTime = fuelTime;
                FuelSlot.Count--;
                if (FuelSlot.Count <= 0)
                {
                    FuelSlot = new ItemStack(0, 0);
                }
            }
        }

        private void CookItem()
        {
            if (InputSlot == null || InputSlot.IsEmpty()) return;
            if (!recipes.TryGetValue(InputSlot.ItemId, out FurnaceRecipe recipe)) return;

            // 消耗输入
            InputSlot.Count--;
            if (InputSlot.Count <= 0)
            {
                InputSlot = new ItemStack(0, 0);
            }

            // 添加输出
            if (OutputSlot == null || OutputSlot.IsEmpty())
            {
                OutputSlot = new ItemStack(recipe.OutputId, recipe.OutputCount);
            }
            else
            {
                OutputSlot.Count += recipe.OutputCount;
            }
        }

        public static int GetFuelTime(int itemId)
        {
            return itemId switch
            {
                500 => 1600, // 煤炭
                501 => 1600, // 木炭
                GameConstants.BLOCK_LOG => 300, // 原木
                GameConstants.BLOCK_WOOD_PLANKS => 300, // 木板
                510 => 100, // 木棍
                702 => 20000, // 岩浆桶
                701 => 20000, // 岩浆桶
                GameConstants.BLOCK_COAL_BLOCK => 16000, // 煤炭块
                GameConstants.BLOCK_PLANKS => 300, // 木板
                GameConstants.BLOCK_CRAFTING_TABLE => 300, // 工作台
                GameConstants.BLOCK_CHEST => 300, // 箱子
                GameConstants.BLOCK_BOOKSHELF => 300, // 书架
                411 => 1500, // 蛋糕
                _ => 0
            };
        }

        public int GetCookTime()
        {
            if (InputSlot == null || InputSlot.IsEmpty()) return 200;
            if (recipes.TryGetValue(InputSlot.ItemId, out FurnaceRecipe recipe))
            {
                return recipe.CookTime;
            }
            return 200;
        }

        public static FurnaceRecipe GetRecipe(int inputId)
        {
            recipes.TryGetValue(inputId, out FurnaceRecipe recipe);
            return recipe;
        }

        public static Dictionary<int, FurnaceRecipe> GetAllRecipes()
        {
            return new Dictionary<int, FurnaceRecipe>(recipes);
        }

        public float GetBurnProgress()
        {
            if (BurnTime == 0) return 0;
            return (float)CurrentBurnTime / BurnTime;
        }

        public float GetCookProgress()
        {
            if (TotalCookTime == 0) return 0;
            return (float)CookTime / TotalCookTime;
        }
    }
}
