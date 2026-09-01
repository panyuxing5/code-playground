using System;
using System.Collections.Generic;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.Items
{
    public class RecipeBook
    {
        private readonly List<Recipe> recipes;
        private readonly HashSet<int> unlockedRecipes;
        private readonly Dictionary<string, List<Recipe>> recipesByCategory;

        // 统计
        public int TotalRecipes => recipes.Count;
        public int UnlockedRecipes => unlockedRecipes.Count;

        public RecipeBook()
        {
            recipes = new List<Recipe>();
            unlockedRecipes = new HashSet<int>();
            recipesByCategory = new Dictionary<string, List<Recipe>>();
        }

        public void Initialize()
        {
            Console.WriteLine("[RecipeBook] 配方书初始化完成");
            RegisterDefaultRecipes();
        }

        private void RegisterDefaultRecipes()
        {
            // 注册各种配方
            RegisterRecipe(new Recipe
            {
                Id = 1,
                Name = "木板",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_WOOD, 1 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_PLANKS, Count = 4 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 2,
                Name = "木棍",
                Category = "材料",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_STICK, Count = 4 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 3,
                Name = "工作台",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 4 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_CRAFTING_TABLE, Count = 1 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 4,
                Name = "熔炉",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_COBBLESTONE, 8 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_FURNACE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 5,
                Name = "箱子",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 8 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_CHEST, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 6,
                Name = "木剑",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 2 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_WOODEN_SWORD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 7,
                Name = "石剑",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_COBBLESTONE, 2 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_STONE_SWORD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 8,
                Name = "铁剑",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 2 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_SWORD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 9,
                Name = "金剑",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_GOLD_INGOT, 2 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_GOLDEN_SWORD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 10,
                Name = "钻石剑",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 2 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_SWORD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 11,
                Name = "木镐",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_WOODEN_PICKAXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 12,
                Name = "石镐",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_COBBLESTONE, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_STONE_PICKAXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 13,
                Name = "铁镐",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_PICKAXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 14,
                Name = "钻石镐",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_PICKAXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 15,
                Name = "木斧",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_WOODEN_AXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 16,
                Name = "铁斧",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_AXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 17,
                Name = "钻石斧",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 3 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_AXE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 18,
                Name = "木铲",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 1 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_WOODEN_SHOVEL, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 19,
                Name = "铁铲",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 1 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_SHOVEL, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 20,
                Name = "钻石铲",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 1 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_SHOVEL, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 21,
                Name = "铁头盔",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 5 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_HELMET, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 22,
                Name = "铁胸甲",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 8 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_CHESTPLATE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 23,
                Name = "铁护腿",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 7 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_LEGGINGS, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 24,
                Name = "铁靴子",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 4 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_BOOTS, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 25,
                Name = "钻石头盔",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 5 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_HELMET, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 26,
                Name = "钻石胸甲",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 8 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_CHESTPLATE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 27,
                Name = "钻石护腿",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 7 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_LEGGINGS, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 28,
                Name = "钻石靴子",
                Category = "护甲",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_DIAMOND, 4 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_DIAMOND_BOOTS, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 29,
                Name = "火把",
                Category = "照明",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_COAL, 1 }, { GameConstants.ITEM_STICK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_TORCH, Count = 4 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 30,
                Name = "梯子",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_STICK, 7 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_LADDER, Count = 3 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 31,
                Name = "门",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 6 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_WOODEN_DOOR, Count = 3 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 32,
                Name = "栅栏",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 4 }, { GameConstants.ITEM_STICK, 2 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_FENCE, Count = 3 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 33,
                Name = "玻璃",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_SAND, 1 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_GLASS, Count = 1 },
                IsShapeless = true,
                RequiresFurnace = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 34,
                Name = "铁锭",
                Category = "材料",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_IRON_ORE, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_IRON_INGOT, Count = 1 },
                IsShapeless = true,
                RequiresFurnace = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 35,
                Name = "金锭",
                Category = "材料",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_GOLD_ORE, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_GOLD_INGOT, Count = 1 },
                IsShapeless = true,
                RequiresFurnace = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 36,
                Name = "面包",
                Category = "食物",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_WHEAT, 3 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_BREAD, Count = 1 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 37,
                Name = "蛋糕",
                Category = "食物",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_WHEAT, 3 }, { GameConstants.ITEM_EGG, 1 }, { GameConstants.ITEM_MILK_BUCKET, 3 }, { GameConstants.ITEM_SUGAR, 2 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_CAKE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 38,
                Name = "曲奇",
                Category = "食物",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_WHEAT, 2 }, { GameConstants.ITEM_COCOA_BEANS, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_COOKIE, Count = 8 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 39,
                Name = "弓",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_STICK, 3 }, { GameConstants.ITEM_STRING, 3 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_BOW, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 40,
                Name = "箭",
                Category = "武器",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_FLINT, 1 }, { GameConstants.ITEM_STICK, 1 }, { GameConstants.ITEM_FEATHER, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_ARROW, Count = 4 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 41,
                Name = "钓鱼竿",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_STICK, 3 }, { GameConstants.ITEM_STRING, 2 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_FISHING_ROD, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 42,
                Name = "指南针",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_INGOT, 4 }, { GameConstants.ITEM_REDSTONE, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_COMPASS, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 43,
                Name = "时钟",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_GOLD_INGOT, 4 }, { GameConstants.ITEM_REDSTONE, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_CLOCK, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 44,
                Name = "地图",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_PAPER, 8 }, { GameConstants.ITEM_COMPASS, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_MAP, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 45,
                Name = "书",
                Category = "材料",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_PAPER, 3 }, { GameConstants.ITEM_LEATHER, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_BOOK, Count = 1 },
                IsShapeless = true
            });

            RegisterRecipe(new Recipe
            {
                Id = 46,
                Name = "书架",
                Category = "建筑方块",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_PLANKS, 6 }, { GameConstants.ITEM_BOOK, 3 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_BOOKSHELF, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 47,
                Name = "附魔台",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_OBSIDIAN, 4 }, { GameConstants.ITEM_DIAMOND, 2 }, { GameConstants.ITEM_BOOK, 1 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_ENCHANTING_TABLE, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 48,
                Name = "铁砧",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_IRON_BLOCK, 3 }, { GameConstants.ITEM_IRON_INGOT, 4 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_ANVIL, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 49,
                Name = "TNT",
                Category = "工具",
                Input = new Dictionary<int, int> { { GameConstants.BLOCK_SAND, 4 }, { GameConstants.ITEM_GUNPOWDER, 5 } },
                Output = new ItemStack { ItemId = GameConstants.BLOCK_TNT, Count = 1 },
                IsShapeless = false
            });

            RegisterRecipe(new Recipe
            {
                Id = 50,
                Name = "末影之眼",
                Category = "材料",
                Input = new Dictionary<int, int> { { GameConstants.ITEM_ENDER_PEARL, 1 }, { GameConstants.ITEM_BLAZE_POWDER, 1 } },
                Output = new ItemStack { ItemId = GameConstants.ITEM_ENDER_EYE, Count = 1 },
                IsShapeless = true
            });

            Console.WriteLine($"[RecipeBook] 已注册 {recipes.Count} 个配方");
        }

        public void RegisterRecipe(Recipe recipe)
        {
            if (!recipes.Exists(r => r.Id == recipe.Id))
            {
                recipes.Add(recipe);

                // 按分类索引
                if (!recipesByCategory.ContainsKey(recipe.Category))
                {
                    recipesByCategory[recipe.Category] = new List<Recipe>();
                }
                recipesByCategory[recipe.Category].Add(recipe);
            }
        }

        public void UnregisterRecipe(int recipeId)
        {
            Recipe recipe = recipes.Find(r => r.Id == recipeId);
            if (recipe != null)
            {
                recipes.Remove(recipe);
                if (recipesByCategory.ContainsKey(recipe.Category))
                {
                    recipesByCategory[recipe.Category].Remove(recipe);
                }
            }
        }

        public Recipe GetRecipe(int recipeId)
        {
            return recipes.Find(r => r.Id == recipeId);
        }

        public Recipe FindRecipe(Dictionary<int, int> input)
        {
            foreach (Recipe recipe in recipes)
            {
                if (recipe.IsShapeless)
                {
                    if (MatchesShapeless(recipe, input))
                    {
                        return recipe;
                    }
                }
                else
                {
                    if (MatchesShaped(recipe, input))
                    {
                        return recipe;
                    }
                }
            }
            return null;
        }

        private bool MatchesShapeless(Recipe recipe, Dictionary<int, int> input)
        {
            if (recipe.Input.Count != input.Count) return false;

            foreach (KeyValuePair<int, int> kvp in recipe.Input)
            {
                if (!input.TryGetValue(kvp.Key, out int count) || count != kvp.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool MatchesShaped(Recipe recipe, Dictionary<int, int> input)
        {
            // 简化的有序配方匹配
            return MatchesShapeless(recipe, input);
        }

        public void UnlockRecipe(int recipeId)
        {
            if (!unlockedRecipes.Contains(recipeId))
            {
                unlockedRecipes.Add(recipeId);
            }
        }

        public void UnlockRecipes(IEnumerable<int> recipeIds)
        {
            foreach (int id in recipeIds)
            {
                UnlockRecipe(id);
            }
        }

        public bool IsRecipeUnlocked(int recipeId)
        {
            return unlockedRecipes.Contains(recipeId);
        }

        public List<Recipe> GetAllRecipes()
        {
            return new List<Recipe>(recipes);
        }

        public List<Recipe> GetUnlockedRecipes()
        {
            return recipes.FindAll(r => unlockedRecipes.Contains(r.Id));
        }

        public List<Recipe> GetLockedRecipes()
        {
            return recipes.FindAll(r => !unlockedRecipes.Contains(r.Id));
        }

        public List<Recipe> GetRecipesByCategory(string category)
        {
            if (recipesByCategory.TryGetValue(category, out List<Recipe> categoryRecipes))
            {
                return new List<Recipe>(categoryRecipes);
            }
            return new List<Recipe>();
        }

        public List<string> GetAllCategories()
        {
            return new List<string>(recipesByCategory.Keys);
        }

        public List<Recipe> GetRecipesByOutput(int outputItemId)
        {
            return recipes.FindAll(r => r.Output.ItemId == outputItemId);
        }

        public List<Recipe> GetRecipesByInput(int inputItemId)
        {
            return recipes.FindAll(r => r.Input.ContainsKey(inputItemId));
        }

        public void Reset()
        {
            unlockedRecipes.Clear();
        }
    }

    public class Recipe
    {
        public int Id;
        public string Name;
        public string Category;
        public Dictionary<int, int> Input;
        public ItemStack Output;
        public bool IsShapeless;
        public bool RequiresFurnace;
        public int ExperienceReward;

        public Recipe()
        {
            Input = new Dictionary<int, int>();
        }
    }
}
