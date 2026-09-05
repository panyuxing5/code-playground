using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Items
{
    public class ItemInfo
    {
        public int Id;
        public string Name;
        public string TranslationKey;
        public int MaxStackSize;
        public ItemType Type;
        public ItemRarity Rarity;
        public int Durability;
        public int AttackDamage;
        public float AttackSpeed;
        public int Defense;
        public int Armor;
        public float Toughness;
        public int FoodLevel;
        public float Saturation;
        public bool IsEdible;
        public bool IsStackable;
        public bool IsDamageable;
        public int BurnTime;
        public int TextureIndex;
        public string Description;
        public List<string> Tooltips;
    }

    public enum ItemType
    {
        Block,
        Tool,
        Weapon,
        Armor,
        Food,
        Material,
        Potion,
        Misc,
        Record,
        HorseArmor
    }

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic
    }

    public enum ToolType
    {
        None,
        Pickaxe,
        Axe,
        Shovel,
        Hoe,
        Sword,
        Shears,
        FishingRod,
        FlintAndSteel,
        Bow,
        Crossbow,
        Trident,
        Shield
    }

    public enum ToolMaterial
    {
        None,
        Wood,
        Stone,
        Iron,
        Gold,
        Diamond,
        Netherite
    }

    public class ItemRegistry
    {
        private static readonly Dictionary<int, ItemInfo> items = new Dictionary<int, ItemInfo>();
        private static readonly Dictionary<string, int> nameToId = new Dictionary<string, int>();
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllItems();
            Console.WriteLine("[ItemRegistry] 物品注册表初始化完成，共注册 " + items.Count + " 个物品");
        }

        private static void RegisterAllItems()
        {
            // 方块物品（从方块ID映射）
            RegisterBlockItems();

            // 工具
            RegisterTools();

            // 武器
            RegisterWeapons();

            // 护甲
            RegisterArmor();

            // 食物
            RegisterFoods();

            // 材料
            RegisterMaterials();

            // 药水
            RegisterPotions();

            // 杂项
            RegisterMisc();
        }

        private static void RegisterBlockItems()
        {
            // 这里只注册一些主要的方块物品，其他方块物品从BlockRegistry自动映射
            RegisterItem(new ItemInfo
            {
                Id = 1,
                Name = "石头",
                TranslationKey = "item.stone",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 0,
                Description = "普通的石头"
            });

            RegisterItem(new ItemInfo
            {
                Id = 2,
                Name = "泥土",
                TranslationKey = "item.dirt",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 1,
                Description = "普通的泥土"
            });

            RegisterItem(new ItemInfo
            {
                Id = 3,
                Name = "草方块",
                TranslationKey = "item.grass_block",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 2,
                Description = "带有草的泥土"
            });

            RegisterItem(new ItemInfo
            {
                Id = 4,
                Name = "圆石",
                TranslationKey = "item.cobblestone",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 3,
                Description = "开采石头获得"
            });

            RegisterItem(new ItemInfo
            {
                Id = 5,
                Name = "木板",
                TranslationKey = "item.planks",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 4,
                Description = "由原木合成"
            });

            RegisterItem(new ItemInfo
            {
                Id = 6,
                Name = "原木",
                TranslationKey = "item.log",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 5,
                Description = "从树木获得"
            });

            RegisterItem(new ItemInfo
            {
                Id = 7,
                Name = "沙子",
                TranslationKey = "item.sand",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 6,
                Description = "受重力影响"
            });

            RegisterItem(new ItemInfo
            {
                Id = 8,
                Name = "玻璃",
                TranslationKey = "item.glass",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 7,
                Description = "透明的方块"
            });

            RegisterItem(new ItemInfo
            {
                Id = 9,
                Name = "铁矿石",
                TranslationKey = "item.iron_ore",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 8,
                Description = "需要石镐开采"
            });

            RegisterItem(new ItemInfo
            {
                Id = 10,
                Name = "金矿石",
                TranslationKey = "item.gold_ore",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 9,
                Description = "需要铁镐开采"
            });

            RegisterItem(new ItemInfo
            {
                Id = 11,
                Name = "钻石矿石",
                TranslationKey = "item.diamond_ore",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Rare,
                IsStackable = true,
                TextureIndex = 10,
                Description = "需要铁镐开采，非常稀有"
            });

            RegisterItem(new ItemInfo
            {
                Id = 12,
                Name = "煤矿石",
                TranslationKey = "item.coal_ore",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 11,
                Description = "最常见的矿石"
            });

            RegisterItem(new ItemInfo
            {
                Id = 13,
                Name = "工作台",
                TranslationKey = "item.crafting_table",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 12,
                Description = "用于合成物品"
            });

            RegisterItem(new ItemInfo
            {
                Id = 14,
                Name = "熔炉",
                TranslationKey = "item.furnace",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 13,
                Description = "用于冶炼物品"
            });

            RegisterItem(new ItemInfo
            {
                Id = 15,
                Name = "箱子",
                TranslationKey = "item.chest",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 14,
                Description = "用于存储物品"
            });

            RegisterItem(new ItemInfo
            {
                Id = 16,
                Name = "火把",
                TranslationKey = "item.torch",
                MaxStackSize = 64,
                Type = ItemType.Block,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 15,
                Description = "提供光照"
            });
        }

        private static void RegisterTools()
        {
            // 木镐
            RegisterItem(new ItemInfo
            {
                Id = 100,
                Name = "木镐",
                TranslationKey = "item.wooden_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 59,
                IsDamageable = true,
                AttackDamage = 2,
                AttackSpeed = 1.2f,
                TextureIndex = 100,
                Description = "基础的采矿工具"
            });

            // 石镐
            RegisterItem(new ItemInfo
            {
                Id = 101,
                Name = "石镐",
                TranslationKey = "item.stone_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 131,
                IsDamageable = true,
                AttackDamage = 3,
                AttackSpeed = 1.2f,
                TextureIndex = 101,
                Description = "可以开采铁矿石"
            });

            // 铁镐
            RegisterItem(new ItemInfo
            {
                Id = 102,
                Name = "铁镐",
                TranslationKey = "item.iron_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 250,
                IsDamageable = true,
                AttackDamage = 4,
                AttackSpeed = 1.2f,
                TextureIndex = 102,
                Description = "可以开采金矿石和钻石矿石"
            });

            // 金镐
            RegisterItem(new ItemInfo
            {
                Id = 103,
                Name = "金镐",
                TranslationKey = "item.golden_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Uncommon,
                Durability = 32,
                IsDamageable = true,
                AttackDamage = 2,
                AttackSpeed = 1.2f,
                TextureIndex = 103,
                Description = "挖掘速度快但不耐用"
            });

            // 钻石镐
            RegisterItem(new ItemInfo
            {
                Id = 104,
                Name = "钻石镐",
                TranslationKey = "item.diamond_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Rare,
                Durability = 1561,
                IsDamageable = true,
                AttackDamage = 5,
                AttackSpeed = 1.2f,
                TextureIndex = 104,
                Description = "可以开采黑曜石"
            });

            // 下界合金镐
            RegisterItem(new ItemInfo
            {
                Id = 105,
                Name = "下界合金镐",
                TranslationKey = "item.netherite_pickaxe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Epic,
                Durability = 2031,
                IsDamageable = true,
                AttackDamage = 6,
                AttackSpeed = 1.2f,
                TextureIndex = 105,
                Description = "最强的镐"
            });

            // 木斧
            RegisterItem(new ItemInfo
            {
                Id = 110,
                Name = "木斧",
                TranslationKey = "item.wooden_axe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 59,
                IsDamageable = true,
                AttackDamage = 7,
                AttackSpeed = 0.8f,
                TextureIndex = 110,
                Description = "用于砍树"
            });

            // 铁斧
            RegisterItem(new ItemInfo
            {
                Id = 111,
                Name = "铁斧",
                TranslationKey = "item.iron_axe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 250,
                IsDamageable = true,
                AttackDamage = 9,
                AttackSpeed = 0.9f,
                TextureIndex = 111,
                Description = "高效的伐木工具"
            });

            // 钻石斧
            RegisterItem(new ItemInfo
            {
                Id = 112,
                Name = "钻石斧",
                TranslationKey = "item.diamond_axe",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Rare,
                Durability = 1561,
                IsDamageable = true,
                AttackDamage = 9,
                AttackSpeed = 1.0f,
                TextureIndex = 112,
                Description = "最强的斧"
            });

            // 木铲
            RegisterItem(new ItemInfo
            {
                Id = 120,
                Name = "木铲",
                TranslationKey = "item.wooden_shovel",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 59,
                IsDamageable = true,
                AttackDamage = 2,
                AttackSpeed = 1.0f,
                TextureIndex = 120,
                Description = "用于挖掘泥土类方块"
            });

            // 铁铲
            RegisterItem(new ItemInfo
            {
                Id = 121,
                Name = "铁铲",
                TranslationKey = "item.iron_shovel",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 250,
                IsDamageable = true,
                AttackDamage = 4,
                AttackSpeed = 1.0f,
                TextureIndex = 121,
                Description = "高效的挖掘工具"
            });

            // 钻石铲
            RegisterItem(new ItemInfo
            {
                Id = 122,
                Name = "钻石铲",
                TranslationKey = "item.diamond_shovel",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Rare,
                Durability = 1561,
                IsDamageable = true,
                AttackDamage = 5,
                AttackSpeed = 1.0f,
                TextureIndex = 122,
                Description = "最强的铲"
            });
        }

        private static void RegisterWeapons()
        {
            // 木剑
            RegisterItem(new ItemInfo
            {
                Id = 200,
                Name = "木剑",
                TranslationKey = "item.wooden_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Durability = 59,
                IsDamageable = true,
                AttackDamage = 4,
                AttackSpeed = 1.6f,
                TextureIndex = 200,
                Description = "基础的武器"
            });

            // 石剑
            RegisterItem(new ItemInfo
            {
                Id = 201,
                Name = "石剑",
                TranslationKey = "item.stone_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Durability = 131,
                IsDamageable = true,
                AttackDamage = 5,
                AttackSpeed = 1.6f,
                TextureIndex = 201,
                Description = "比木剑更强"
            });

            // 铁剑
            RegisterItem(new ItemInfo
            {
                Id = 202,
                Name = "铁剑",
                TranslationKey = "item.iron_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Durability = 250,
                IsDamageable = true,
                AttackDamage = 6,
                AttackSpeed = 1.6f,
                TextureIndex = 202,
                Description = "可靠的武器"
            });

            // 金剑
            RegisterItem(new ItemInfo
            {
                Id = 203,
                Name = "金剑",
                TranslationKey = "item.golden_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Uncommon,
                Durability = 32,
                IsDamageable = true,
                AttackDamage = 4,
                AttackSpeed = 1.6f,
                TextureIndex = 203,
                Description = "附魔效果好但不耐用"
            });

            // 钻石剑
            RegisterItem(new ItemInfo
            {
                Id = 204,
                Name = "钻石剑",
                TranslationKey = "item.diamond_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                Durability = 1561,
                IsDamageable = true,
                AttackDamage = 7,
                AttackSpeed = 1.6f,
                TextureIndex = 204,
                Description = "最强的剑之一"
            });

            // 下界合金剑
            RegisterItem(new ItemInfo
            {
                Id = 205,
                Name = "下界合金剑",
                TranslationKey = "item.netherite_sword",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Epic,
                Durability = 2031,
                IsDamageable = true,
                AttackDamage = 8,
                AttackSpeed = 1.6f,
                TextureIndex = 205,
                Description = "最强的剑"
            });

            // 弓
            RegisterItem(new ItemInfo
            {
                Id = 210,
                Name = "弓",
                TranslationKey = "item.bow",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Common,
                Durability = 384,
                IsDamageable = true,
                AttackDamage = 9,
                AttackSpeed = 0.5f,
                TextureIndex = 210,
                Description = "远程武器，需要箭"
            });

            // 箭
            RegisterItem(new ItemInfo
            {
                Id = 211,
                Name = "箭",
                TranslationKey = "item.arrow",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 211,
                Description = "弓的弹药"
            });

            // 三叉戟
            RegisterItem(new ItemInfo
            {
                Id = 212,
                Name = "三叉戟",
                TranslationKey = "item.trident",
                MaxStackSize = 1,
                Type = ItemType.Weapon,
                Rarity = ItemRarity.Rare,
                Durability = 250,
                IsDamageable = true,
                AttackDamage = 9,
                AttackSpeed = 1.1f,
                TextureIndex = 212,
                Description = "可以投掷的武器"
            });
        }

        private static void RegisterArmor()
        {
            // 皮革套
            RegisterArmorSet(300, "皮革", ToolMaterial.Wood, 5, 1, ItemRarity.Common);
            // 锁链套
            RegisterArmorSet(310, "锁链", ToolMaterial.Stone, 12, 2, ItemRarity.Uncommon);
            // 铁套
            RegisterArmorSet(320, "铁", ToolMaterial.Iron, 15, 2, ItemRarity.Common);
            // 金套
            RegisterArmorSet(330, "金", ToolMaterial.Gold, 7, 2, ItemRarity.Uncommon);
            // 钻石套
            RegisterArmorSet(340, "钻石", ToolMaterial.Diamond, 20, 2, ItemRarity.Rare);
            // 下界合金套
            RegisterArmorSet(350, "下界合金", ToolMaterial.Netherite, 20, 3, ItemRarity.Epic);
        }

        private static void RegisterArmorSet(int baseId, string materialName, ToolMaterial material, int baseArmor, int toughness, ItemRarity rarity)
        {
            // 头盔
            RegisterItem(new ItemInfo
            {
                Id = baseId,
                Name = materialName + "头盔",
                TranslationKey = $"item.{materialName.ToLower()}_helmet",
                MaxStackSize = 1,
                Type = ItemType.Armor,
                Rarity = rarity,
                Durability = GetArmorDurability(material, 11),
                IsDamageable = true,
                Armor = baseArmor / 4,
                Toughness = toughness,
                TextureIndex = baseId,
                Description = "保护头部"
            });

            // 胸甲
            RegisterItem(new ItemInfo
            {
                Id = baseId + 1,
                Name = materialName + "胸甲",
                TranslationKey = $"item.{materialName.ToLower()}_chestplate",
                MaxStackSize = 1,
                Type = ItemType.Armor,
                Rarity = rarity,
                Durability = GetArmorDurability(material, 16),
                IsDamageable = true,
                Armor = baseArmor / 3,
                Toughness = toughness,
                TextureIndex = baseId + 1,
                Description = "保护胸部"
            });

            // 护腿
            RegisterItem(new ItemInfo
            {
                Id = baseId + 2,
                Name = materialName + "护腿",
                TranslationKey = $"item.{materialName.ToLower()}_leggings",
                MaxStackSize = 1,
                Type = ItemType.Armor,
                Rarity = rarity,
                Durability = GetArmorDurability(material, 15),
                IsDamageable = true,
                Armor = baseArmor / 4,
                Toughness = toughness,
                TextureIndex = baseId + 2,
                Description = "保护腿部"
            });

            // 靴子
            RegisterItem(new ItemInfo
            {
                Id = baseId + 3,
                Name = materialName + "靴子",
                TranslationKey = $"item.{materialName.ToLower()}_boots",
                MaxStackSize = 1,
                Type = ItemType.Armor,
                Rarity = rarity,
                Durability = GetArmorDurability(material, 13),
                IsDamageable = true,
                Armor = baseArmor / 5,
                Toughness = toughness,
                TextureIndex = baseId + 3,
                Description = "保护脚部"
            });
        }

        private static int GetArmorDurability(ToolMaterial material, int baseValue)
        {
            return material switch
            {
                ToolMaterial.Wood => baseValue * 5,
                ToolMaterial.Stone => baseValue * 5,
                ToolMaterial.Iron => baseValue * 15,
                ToolMaterial.Gold => baseValue * 7,
                ToolMaterial.Diamond => baseValue * 33,
                ToolMaterial.Netherite => baseValue * 37,
                _ => baseValue
            };
        }

        private static void RegisterFoods()
        {
            RegisterItem(new ItemInfo
            {
                Id = 400,
                Name = "苹果",
                TranslationKey = "item.apple",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 4,
                Saturation = 2.4f,
                TextureIndex = 400,
                Description = "基础的食物"
            });

            RegisterItem(new ItemInfo
            {
                Id = 401,
                Name = "面包",
                TranslationKey = "item.bread",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 5,
                Saturation = 6.0f,
                TextureIndex = 401,
                Description = "由小麦合成"
            });

            RegisterItem(new ItemInfo
            {
                Id = 402,
                Name = "牛排",
                TranslationKey = "item.cooked_beef",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 8,
                Saturation = 12.8f,
                TextureIndex = 402,
                Description = "高营养食物"
            });

            RegisterItem(new ItemInfo
            {
                Id = 403,
                Name = "猪排",
                TranslationKey = "item.cooked_porkchop",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 8,
                Saturation = 12.8f,
                TextureIndex = 403,
                Description = "高营养食物"
            });

            RegisterItem(new ItemInfo
            {
                Id = 404,
                Name = "金苹果",
                TranslationKey = "item.golden_apple",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Rare,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 4,
                Saturation = 9.6f,
                TextureIndex = 404,
                Description = "提供生命恢复和伤害吸收"
            });

            RegisterItem(new ItemInfo
            {
                Id = 405,
                Name = "附魔金苹果",
                TranslationKey = "item.enchanted_golden_apple",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Epic,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 4,
                Saturation = 9.6f,
                TextureIndex = 405,
                Description = "提供强力增益效果"
            });

            RegisterItem(new ItemInfo
            {
                Id = 406,
                Name = "胡萝卜",
                TranslationKey = "item.carrot",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 3,
                Saturation = 3.6f,
                TextureIndex = 406,
                Description = "可以种植"
            });

            RegisterItem(new ItemInfo
            {
                Id = 407,
                Name = "马铃薯",
                TranslationKey = "item.potato",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 1,
                Saturation = 0.6f,
                TextureIndex = 407,
                Description = "可以种植，烤后更好"
            });

            RegisterItem(new ItemInfo
            {
                Id = 408,
                Name = "烤马铃薯",
                TranslationKey = "item.baked_potato",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 5,
                Saturation = 6.0f,
                TextureIndex = 408,
                Description = "烤制后的马铃薯"
            });

            RegisterItem(new ItemInfo
            {
                Id = 409,
                Name = "西瓜片",
                TranslationKey = "item.melon_slice",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 2,
                Saturation = 1.2f,
                TextureIndex = 409,
                Description = "清凉的水果"
            });

            RegisterItem(new ItemInfo
            {
                Id = 410,
                Name = "曲奇",
                TranslationKey = "item.cookie",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 2,
                Saturation = 0.4f,
                TextureIndex = 410,
                Description = "美味的小点心"
            });

            RegisterItem(new ItemInfo
            {
                Id = 411,
                Name = "蛋糕",
                TranslationKey = "item.cake",
                MaxStackSize = 1,
                Type = ItemType.Food,
                Rarity = ItemRarity.Uncommon,
                IsEdible = true,
                IsStackable = false,
                FoodLevel = 14,
                Saturation = 2.8f,
                TextureIndex = 411,
                Description = "可以多次食用"
            });

            RegisterItem(new ItemInfo
            {
                Id = 412,
                Name = "南瓜派",
                TranslationKey = "item.pumpkin_pie",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 8,
                Saturation = 4.8f,
                TextureIndex = 412,
                Description = "节日美食"
            });
        }

        private static void RegisterMaterials()
        {
            RegisterItem(new ItemInfo
            {
                Id = 500,
                Name = "煤炭",
                TranslationKey = "item.coal",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                BurnTime = 80,
                TextureIndex = 500,
                Description = "常用燃料"
            });

            RegisterItem(new ItemInfo
            {
                Id = 501,
                Name = "木炭",
                TranslationKey = "item.charcoal",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                BurnTime = 80,
                TextureIndex = 501,
                Description = "由原木烧制"
            });

            RegisterItem(new ItemInfo
            {
                Id = 502,
                Name = "铁锭",
                TranslationKey = "item.iron_ingot",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 502,
                Description = "由铁矿石冶炼"
            });

            RegisterItem(new ItemInfo
            {
                Id = 503,
                Name = "金锭",
                TranslationKey = "item.gold_ingot",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 503,
                Description = "由金矿石冶炼"
            });

            RegisterItem(new ItemInfo
            {
                Id = 504,
                Name = "钻石",
                TranslationKey = "item.diamond",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Rare,
                IsStackable = true,
                TextureIndex = 504,
                Description = "稀有宝石"
            });

            RegisterItem(new ItemInfo
            {
                Id = 505,
                Name = "下界合金锭",
                TranslationKey = "item.netherite_ingot",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Epic,
                IsStackable = true,
                TextureIndex = 505,
                Description = "最强的材料"
            });

            RegisterItem(new ItemInfo
            {
                Id = 506,
                Name = "红石",
                TranslationKey = "item.redstone",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 506,
                Description = "用于红石电路"
            });

            RegisterItem(new ItemInfo
            {
                Id = 507,
                Name = "青金石",
                TranslationKey = "item.lapis_lazuli",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 507,
                Description = "用于附魔"
            });

            RegisterItem(new ItemInfo
            {
                Id = 508,
                Name = "绿宝石",
                TranslationKey = "item.emerald",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Rare,
                IsStackable = true,
                TextureIndex = 508,
                Description = "村民交易货币"
            });

            RegisterItem(new ItemInfo
            {
                Id = 509,
                Name = "石英",
                TranslationKey = "item.quartz",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 509,
                Description = "下界材料"
            });

            RegisterItem(new ItemInfo
            {
                Id = 510,
                Name = "木棍",
                TranslationKey = "item.stick",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 510,
                Description = "工具手柄"
            });

            RegisterItem(new ItemInfo
            {
                Id = 511,
                Name = "线",
                TranslationKey = "item.string",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 511,
                Description = "用于合成弓和钓鱼竿"
            });

            RegisterItem(new ItemInfo
            {
                Id = 512,
                Name = "皮革",
                TranslationKey = "item.leather",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 512,
                Description = "用于制作皮革护甲"
            });

            RegisterItem(new ItemInfo
            {
                Id = 513,
                Name = "羽毛",
                TranslationKey = "item.feather",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 513,
                Description = "用于制作箭和书"
            });

            RegisterItem(new ItemInfo
            {
                Id = 514,
                Name = "骨头",
                TranslationKey = "item.bone",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 514,
                Description = "可以制成骨粉"
            });

            RegisterItem(new ItemInfo
            {
                Id = 515,
                Name = "腐肉",
                TranslationKey = "item.rotten_flesh",
                MaxStackSize = 64,
                Type = ItemType.Food,
                Rarity = ItemRarity.Common,
                IsEdible = true,
                IsStackable = true,
                FoodLevel = 4,
                Saturation = 0.8f,
                TextureIndex = 515,
                Description = "可能引起饥饿效果"
            });

            RegisterItem(new ItemInfo
            {
                Id = 516,
                Name = "末影珍珠",
                TranslationKey = "item.ender_pearl",
                MaxStackSize = 16,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Rare,
                IsStackable = true,
                TextureIndex = 516,
                Description = "投掷后传送"
            });

            RegisterItem(new ItemInfo
            {
                Id = 517,
                Name = "烈焰棒",
                TranslationKey = "item.blaze_rod",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 517,
                Description = "用于酿造和合成末影眼"
            });

            RegisterItem(new ItemInfo
            {
                Id = 518,
                Name = "末影眼",
                TranslationKey = "item.ender_eye",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Rare,
                IsStackable = true,
                TextureIndex = 518,
                Description = "用于寻找末地要塞"
            });

            RegisterItem(new ItemInfo
            {
                Id = 519,
                Name = "龙息",
                TranslationKey = "item.dragon_breath",
                MaxStackSize = 64,
                Type = ItemType.Material,
                Rarity = ItemRarity.Epic,
                IsStackable = true,
                TextureIndex = 519,
                Description = "用于酿造滞留药水"
            });
        }

        private static void RegisterPotions()
        {
            RegisterItem(new ItemInfo
            {
                Id = 600,
                Name = "治疗药水",
                TranslationKey = "item.healing_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 600,
                Description = "立即恢复生命值"
            });

            RegisterItem(new ItemInfo
            {
                Id = 601,
                Name = "力量药水",
                TranslationKey = "item.strength_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 601,
                Description = "增加攻击伤害"
            });

            RegisterItem(new ItemInfo
            {
                Id = 602,
                Name = "迅捷药水",
                TranslationKey = "item.swiftness_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 602,
                Description = "增加移动速度"
            });

            RegisterItem(new ItemInfo
            {
                Id = 603,
                Name = "夜视药水",
                TranslationKey = "item.night_vision_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                IsStackable = false,
                TextureIndex = 603,
                Description = "在黑暗中看清"
            });

            RegisterItem(new ItemInfo
            {
                Id = 604,
                Name = "水肺药水",
                TranslationKey = "item.water_breathing_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                IsStackable = false,
                TextureIndex = 604,
                Description = "在水下呼吸"
            });

            RegisterItem(new ItemInfo
            {
                Id = 605,
                Name = "抗火药水",
                TranslationKey = "item.fire_resistance_potion",
                MaxStackSize = 1,
                Type = ItemType.Potion,
                Rarity = ItemRarity.Uncommon,
                IsStackable = false,
                TextureIndex = 605,
                Description = "免疫火焰伤害"
            });
        }

        private static void RegisterMisc()
        {
            RegisterItem(new ItemInfo
            {
                Id = 700,
                Name = "水桶",
                TranslationKey = "item.water_bucket",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 700,
                Description = "可以装水"
            });

            RegisterItem(new ItemInfo
            {
                Id = 701,
                Name = "岩浆桶",
                TranslationKey = "item.lava_bucket",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                BurnTime = 1000,
                TextureIndex = 701,
                Description = "可以装岩浆"
            });

            RegisterItem(new ItemInfo
            {
                Id = 702,
                Name = "空桶",
                TranslationKey = "item.bucket",
                MaxStackSize = 16,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 702,
                Description = "可以装液体"
            });

            RegisterItem(new ItemInfo
            {
                Id = 703,
                Name = "指南针",
                TranslationKey = "item.compass",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 703,
                Description = "指向出生点"
            });

            RegisterItem(new ItemInfo
            {
                Id = 704,
                Name = "时钟",
                TranslationKey = "item.clock",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 704,
                Description = "显示游戏时间"
            });

            RegisterItem(new ItemInfo
            {
                Id = 705,
                Name = "地图",
                TranslationKey = "item.map",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = false,
                TextureIndex = 705,
                Description = "显示周围地形"
            });

            RegisterItem(new ItemInfo
            {
                Id = 706,
                Name = "书",
                TranslationKey = "item.book",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 706,
                Description = "用于附魔台"
            });

            RegisterItem(new ItemInfo
            {
                Id = 707,
                Name = "纸",
                TranslationKey = "item.paper",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Common,
                IsStackable = true,
                TextureIndex = 707,
                Description = "由甘蔗合成"
            });

            RegisterItem(new ItemInfo
            {
                Id = 708,
                Name = "命名牌",
                TranslationKey = "item.name_tag",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 708,
                Description = "给生物命名"
            });

            RegisterItem(new ItemInfo
            {
                Id = 709,
                Name = "鞍",
                TranslationKey = "item.saddle",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Uncommon,
                IsStackable = false,
                TextureIndex = 709,
                Description = "骑乘生物"
            });

            RegisterItem(new ItemInfo
            {
                Id = 710,
                Name = "钓鱼竿",
                TranslationKey = "item.fishing_rod",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 64,
                IsDamageable = true,
                TextureIndex = 710,
                Description = "用于钓鱼"
            });

            RegisterItem(new ItemInfo
            {
                Id = 711,
                Name = "打火石",
                TranslationKey = "item.flint_and_steel",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 64,
                IsDamageable = true,
                TextureIndex = 711,
                Description = "点燃方块"
            });

            RegisterItem(new ItemInfo
            {
                Id = 712,
                Name = "剪刀",
                TranslationKey = "item.shears",
                MaxStackSize = 1,
                Type = ItemType.Tool,
                Rarity = ItemRarity.Common,
                Durability = 238,
                IsDamageable = true,
                TextureIndex = 712,
                Description = "剪羊毛和树叶"
            });

            RegisterItem(new ItemInfo
            {
                Id = 713,
                Name = "拴绳",
                TranslationKey = "item.lead",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 713,
                Description = "牵引动物"
            });

            RegisterItem(new ItemInfo
            {
                Id = 714,
                Name = "经验瓶",
                TranslationKey = "item.experience_bottle",
                MaxStackSize = 64,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Uncommon,
                IsStackable = true,
                TextureIndex = 714,
                Description = "投掷后掉落经验"
            });

            RegisterItem(new ItemInfo
            {
                Id = 715,
                Name = "不死图腾",
                TranslationKey = "item.totem_of_undying",
                MaxStackSize = 1,
                Type = ItemType.Misc,
                Rarity = ItemRarity.Epic,
                IsStackable = false,
                TextureIndex = 715,
                Description = "死亡时复活"
            });
        }

        private static void RegisterItem(ItemInfo item)
        {
            items[item.Id] = item;
            nameToId[item.Name] = item.Id;
            nameToId[item.TranslationKey] = item.Id;
        }

        public static ItemInfo GetItem(int id)
        {
            items.TryGetValue(id, out ItemInfo item);
            return item;
        }

        public static ItemInfo GetItem(string name)
        {
            if (nameToId.TryGetValue(name, out int id))
            {
                return GetItem(id);
            }
            return null;
        }

        public static bool ItemExists(int id)
        {
            return items.ContainsKey(id);
        }

        public static Dictionary<int, ItemInfo> GetAllItems()
        {
            return new Dictionary<int, ItemInfo>(items);
        }

        public static List<ItemInfo> GetItemsByType(ItemType type)
        {
            List<ItemInfo> result = new List<ItemInfo>();
            foreach (ItemInfo item in items.Values)
            {
                if (item.Type == type)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static List<ItemInfo> GetItemsByRarity(ItemRarity rarity)
        {
            List<ItemInfo> result = new List<ItemInfo>();
            foreach (ItemInfo item in items.Values)
            {
                if (item.Rarity == rarity)
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
