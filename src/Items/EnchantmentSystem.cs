using System;
using System.Collections.Generic;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.Items
{
    public class Enchantment
    {
        public int Id;
        public string Name;
        public string TranslationKey;
        public int MaxLevel;
        public EnchantmentTarget Target;
        public bool IsTreasure;
        public bool IsCurse;
        public List<int> Conflicts;
        public int MinLevel;
        public int MaxLevelCost;
        public string Description;
    }

    public enum EnchantmentTarget
    {
        All,
        Armor,
        Helmet,
        Chestplate,
        Leggings,
        Boots,
        Weapon,
        Sword,
        Tool,
        Pickaxe,
        Axe,
        Shovel,
        Hoe,
        Bow,
        Crossbow,
        Trident,
        FishingRod,
        Book
    }

    public class EnchantmentInstance
    {
        public int EnchantmentId;
        public int Level;
    }

    public class EnchantmentSystem
    {
        private static readonly Dictionary<int, Enchantment> enchantments = new Dictionary<int, Enchantment>();
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllEnchantments();
            Console.WriteLine("[EnchantmentSystem] 附魔系统初始化完成，共注册 " + enchantments.Count + " 个附魔");
        }

        private static void RegisterAllEnchantments()
        {
            // 护甲附魔
            RegisterProtectionEnchantments();
            // 武器附魔
            RegisterWeaponEnchantments();
            // 工具附魔
            RegisterToolEnchantments();
            // 弓附魔
            RegisterBowEnchantments();
            // 其他附魔
            RegisterMiscEnchantments();
        }

        private static void RegisterProtectionEnchantments()
        {
            enchantments[0] = new Enchantment
            {
                Id = 0,
                Name = "保护",
                TranslationKey = "enchantment.protect.all",
                MaxLevel = 4,
                Target = EnchantmentTarget.Armor,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 1, 2, 3 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "减少所有伤害"
            };

            enchantments[1] = new Enchantment
            {
                Id = 1,
                Name = "火焰保护",
                TranslationKey = "enchantment.protect.fire",
                MaxLevel = 4,
                Target = EnchantmentTarget.Armor,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 0, 2, 3 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "减少火焰伤害"
            };

            enchantments[2] = new Enchantment
            {
                Id = 2,
                Name = "摔落保护",
                TranslationKey = "enchantment.protect.fall",
                MaxLevel = 4,
                Target = EnchantmentTarget.Boots,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 0, 1, 3 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "减少摔落伤害"
            };

            enchantments[3] = new Enchantment
            {
                Id = 3,
                Name = "爆炸保护",
                TranslationKey = "enchantment.protect.explosion",
                MaxLevel = 4,
                Target = EnchantmentTarget.Armor,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 0, 1, 2 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "减少爆炸伤害"
            };

            enchantments[4] = new Enchantment
            {
                Id = 4,
                Name = "弹射物保护",
                TranslationKey = "enchantment.protect.projectile",
                MaxLevel = 4,
                Target = EnchantmentTarget.Armor,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 0, 1, 3 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "减少弹射物伤害"
            };

            enchantments[5] = new Enchantment
            {
                Id = 5,
                Name = "荆棘",
                TranslationKey = "enchantment.thorns",
                MaxLevel = 3,
                Target = EnchantmentTarget.Armor,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "反弹伤害给攻击者"
            };

            enchantments[6] = new Enchantment
            {
                Id = 6,
                Name = "水下呼吸",
                TranslationKey = "enchantment.water.respiration",
                MaxLevel = 3,
                Target = EnchantmentTarget.Helmet,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "延长水下呼吸时间"
            };

            enchantments[7] = new Enchantment
            {
                Id = 7,
                Name = "水下速掘",
                TranslationKey = "enchantment.water.aqua_affinity",
                MaxLevel = 1,
                Target = EnchantmentTarget.Helmet,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "加快水下挖掘速度"
            };

            enchantments[8] = new Enchantment
            {
                Id = 8,
                Name = "深海探索者",
                TranslationKey = "enchantment.water.depth_strider",
                MaxLevel = 3,
                Target = EnchantmentTarget.Boots,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "加快水下移动速度"
            };

            enchantments[9] = new Enchantment
            {
                Id = 9,
                Name = "冰霜行者",
                TranslationKey = "enchantment.water.frost_walker",
                MaxLevel = 2,
                Target = EnchantmentTarget.Boots,
                IsTreasure = true,
                IsCurse = false,
                Conflicts = new List<int> { 8 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "在水上行走时生成冰"
            };

            enchantments[10] = new Enchantment
            {
                Id = 10,
                Name = "绑定诅咒",
                TranslationKey = "enchantment.curse.binding",
                MaxLevel = 1,
                Target = EnchantmentTarget.Armor,
                IsTreasure = true,
                IsCurse = true,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "无法卸下被绑定的护甲"
            };
        }

        private static void RegisterWeaponEnchantments()
        {
            enchantments[16] = new Enchantment
            {
                Id = 16,
                Name = "锋利",
                TranslationKey = "enchantment.damage.all",
                MaxLevel = 5,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 17, 18 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "增加所有伤害"
            };

            enchantments[17] = new Enchantment
            {
                Id = 17,
                Name = "亡灵杀手",
                TranslationKey = "enchantment.damage.undead",
                MaxLevel = 5,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 16, 18 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "对亡灵生物造成额外伤害"
            };

            enchantments[18] = new Enchantment
            {
                Id = 18,
                Name = "节肢杀手",
                TranslationKey = "enchantment.damage.arthropods",
                MaxLevel = 5,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 16, 17 },
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "对节肢生物造成额外伤害"
            };

            enchantments[19] = new Enchantment
            {
                Id = 19,
                Name = "击退",
                TranslationKey = "enchantment.knockback",
                MaxLevel = 2,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 12,
                Description = "增加击退距离"
            };

            enchantments[20] = new Enchantment
            {
                Id = 20,
                Name = "火焰附加",
                TranslationKey = "enchantment.fire",
                MaxLevel = 2,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "使目标着火"
            };

            enchantments[21] = new Enchantment
            {
                Id = 21,
                Name = "抢夺",
                TranslationKey = "enchantment.loot",
                MaxLevel = 3,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加掉落物数量"
            };

            enchantments[22] = new Enchantment
            {
                Id = 22,
                Name = "横扫之刃",
                TranslationKey = "enchantment.sweeping",
                MaxLevel = 3,
                Target = EnchantmentTarget.Sword,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加横扫攻击伤害"
            };
        }

        private static void RegisterToolEnchantments()
        {
            enchantments[32] = new Enchantment
            {
                Id = 32,
                Name = "效率",
                TranslationKey = "enchantment.digging",
                MaxLevel = 5,
                Target = EnchantmentTarget.Tool,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "加快挖掘速度"
            };

            enchantments[33] = new Enchantment
            {
                Id = 33,
                Name = "精准采集",
                TranslationKey = "enchantment.silk_touch",
                MaxLevel = 1,
                Target = EnchantmentTarget.Tool,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 34 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "采集方块本身而非掉落物"
            };

            enchantments[34] = new Enchantment
            {
                Id = 34,
                Name = "时运",
                TranslationKey = "enchantment.loot_bonus",
                MaxLevel = 3,
                Target = EnchantmentTarget.Tool,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 33 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加矿物掉落数量"
            };

            enchantments[35] = new Enchantment
            {
                Id = 35,
                Name = "耐久",
                TranslationKey = "enchantment.durability",
                MaxLevel = 3,
                Target = EnchantmentTarget.All,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "减少耐久消耗"
            };

            enchantments[36] = new Enchantment
            {
                Id = 36,
                Name = "经验修补",
                TranslationKey = "enchantment.mending",
                MaxLevel = 1,
                Target = EnchantmentTarget.All,
                IsTreasure = true,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "用经验球修复耐久"
            };

            enchantments[37] = new Enchantment
            {
                Id = 37,
                Name = "消失诅咒",
                TranslationKey = "enchantment.curse.vanishing",
                MaxLevel = 1,
                Target = EnchantmentTarget.All,
                IsTreasure = true,
                IsCurse = true,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "死亡时物品消失"
            };
        }

        private static void RegisterBowEnchantments()
        {
            enchantments[48] = new Enchantment
            {
                Id = 48,
                Name = "力量",
                TranslationKey = "enchantment.arrow.damage",
                MaxLevel = 5,
                Target = EnchantmentTarget.Bow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加箭的伤害"
            };

            enchantments[49] = new Enchantment
            {
                Id = 49,
                Name = "冲击",
                TranslationKey = "enchantment.arrow.knockback",
                MaxLevel = 2,
                Target = EnchantmentTarget.Bow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加箭的击退"
            };

            enchantments[50] = new Enchantment
            {
                Id = 50,
                Name = "火矢",
                TranslationKey = "enchantment.arrow.fire",
                MaxLevel = 1,
                Target = EnchantmentTarget.Bow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "箭会点燃目标"
            };

            enchantments[51] = new Enchantment
            {
                Id = 51,
                Name = "无限",
                TranslationKey = "enchantment.arrow.infinite",
                MaxLevel = 1,
                Target = EnchantmentTarget.Bow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 36 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "射箭不消耗箭"
            };
        }

        private static void RegisterMiscEnchantments()
        {
            enchantments[61] = new Enchantment
            {
                Id = 61,
                Name = "海之眷顾",
                TranslationKey = "enchantment.fishing.luck",
                MaxLevel = 3,
                Target = EnchantmentTarget.FishingRod,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "增加钓鱼时获得宝藏的几率"
            };

            enchantments[62] = new Enchantment
            {
                Id = 62,
                Name = "饵钓",
                TranslationKey = "enchantment.fishing.speed",
                MaxLevel = 3,
                Target = EnchantmentTarget.FishingRod,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "加快钓鱼速度"
            };

            enchantments[64] = new Enchantment
            {
                Id = 64,
                Name = "穿刺",
                TranslationKey = "enchantment.trident.impaling",
                MaxLevel = 5,
                Target = EnchantmentTarget.Trident,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "对水生生物造成额外伤害"
            };

            enchantments[65] = new Enchantment
            {
                Id = 65,
                Name = "激流",
                TranslationKey = "enchantment.trident.riptide",
                MaxLevel = 3,
                Target = EnchantmentTarget.Trident,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 66, 67 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "在水中或雨中投掷时将玩家发射出去"
            };

            enchantments[66] = new Enchantment
            {
                Id = 66,
                Name = "忠诚",
                TranslationKey = "enchantment.trident.loyalty",
                MaxLevel = 3,
                Target = EnchantmentTarget.Trident,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 65 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "三叉戟会返回玩家手中"
            };

            enchantments[67] = new Enchantment
            {
                Id = 67,
                Name = "引雷",
                TranslationKey = "enchantment.trident.channeling",
                MaxLevel = 1,
                Target = EnchantmentTarget.Trident,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 65 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "雷暴天气时召唤闪电"
            };

            enchantments[70] = new Enchantment
            {
                Id = 70,
                Name = "多重射击",
                TranslationKey = "enchantment.crossbow.multishot",
                MaxLevel = 1,
                Target = EnchantmentTarget.Crossbow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 71 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "一次射出三支箭"
            };

            enchantments[71] = new Enchantment
            {
                Id = 71,
                Name = "穿透",
                TranslationKey = "enchantment.crossbow.piercing",
                MaxLevel = 4,
                Target = EnchantmentTarget.Crossbow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int> { 70 },
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "箭可以穿透多个实体"
            };

            enchantments[72] = new Enchantment
            {
                Id = 72,
                Name = "快速装填",
                TranslationKey = "enchantment.crossbow.quick_charge",
                MaxLevel = 3,
                Target = EnchantmentTarget.Crossbow,
                IsTreasure = false,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "加快弩的装填速度"
            };

            enchantments[73] = new Enchantment
            {
                Id = 73,
                Name = "灵魂疾行",
                TranslationKey = "enchantment.boot.soul_speed",
                MaxLevel = 3,
                Target = EnchantmentTarget.Boots,
                IsTreasure = true,
                IsCurse = false,
                Conflicts = new List<int>(),
                MinLevel = 1,
                MaxLevelCost = 15,
                Description = "在灵魂沙和灵魂土上加速"
            };
        }

        public static Enchantment GetEnchantment(int id)
        {
            enchantments.TryGetValue(id, out Enchantment enchantment);
            return enchantment;
        }

        public static List<Enchantment> GetEnchantmentsForItem(int itemId)
        {
            List<Enchantment> result = new List<Enchantment>();
            ItemInfo item = ItemRegistry.GetItem(itemId);
            if (item == null) return result;

            EnchantmentTarget target = GetItemTarget(item);

            foreach (Enchantment enchantment in enchantments.Values)
            {
                if (enchantment.Target == EnchantmentTarget.All ||
                    enchantment.Target == target ||
                    IsCompatibleTarget(enchantment.Target, target))
                {
                    result.Add(enchantment);
                }
            }

            return result;
        }

        private static EnchantmentTarget GetItemTarget(ItemInfo item)
        {
            return item.Type switch
            {
                ItemType.Weapon => EnchantmentTarget.Sword,
                ItemType.Tool => EnchantmentTarget.Tool,
                ItemType.Armor => EnchantmentTarget.Armor,
                _ => EnchantmentTarget.All
            };
        }

        private static bool IsCompatibleTarget(EnchantmentTarget enchantmentTarget, EnchantmentTarget itemTarget)
        {
            if (enchantmentTarget == EnchantmentTarget.Armor &&
                (itemTarget == EnchantmentTarget.Helmet ||
                 itemTarget == EnchantmentTarget.Chestplate ||
                 itemTarget == EnchantmentTarget.Leggings ||
                 itemTarget == EnchantmentTarget.Boots))
            {
                return true;
            }

            if (enchantmentTarget == EnchantmentTarget.Tool &&
                (itemTarget == EnchantmentTarget.Pickaxe ||
                 itemTarget == EnchantmentTarget.Axe ||
                 itemTarget == EnchantmentTarget.Shovel ||
                 itemTarget == EnchantmentTarget.Hoe))
            {
                return true;
            }

            return false;
        }

        public static bool AreEnchantmentsCompatible(int id1, int id2)
        {
            Enchantment e1 = GetEnchantment(id1);
            Enchantment e2 = GetEnchantment(id2);

            if (e1 == null || e2 == null) return false;
            if (e1.Id == e2.Id) return false;

            return !e1.Conflicts.Contains(e2.Id) && !e2.Conflicts.Contains(e1.Id);
        }

        public static List<EnchantmentInstance> GenerateEnchantments(int itemId, int levels, int bookshelves, Random random)
        {
            List<EnchantmentInstance> result = new List<EnchantmentInstance>();
            List<Enchantment> available = GetEnchantmentsForItem(itemId);

            if (available.Count == 0) return result;

            // 基础附魔成本
            int baseCost = levels + random.Next(1, 8 + bookshelves / 2) + random.Next(1, 8 + bookshelves / 2);
            float costMultiplier = 1.0f + (random.NextSingle() - 0.5f) * 0.5f;
            int finalCost = (int)Math.Max(1, baseCost * costMultiplier);

            // 选择附魔
            float enchantmentChance = finalCost / 50.0f;
            if (random.NextDouble() < enchantmentChance)
            {
                Enchantment selected = available[random.Next(available.Count)];
                int level = GetRandomEnchantmentLevel(selected, finalCost, random);
                result.Add(new EnchantmentInstance { EnchantmentId = selected.Id, Level = level });

                // 额外附魔
                while (random.NextDouble() < (finalCost + 7) / 100.0f)
                {
                    List<Enchantment> compatible = new List<Enchantment>();
                    foreach (Enchantment e in available)
                    {
                        bool isCompatible = true;
                        foreach (EnchantmentInstance existing in result)
                        {
                            if (!AreEnchantmentsCompatible(e.Id, existing.EnchantmentId))
                            {
                                isCompatible = false;
                                break;
                            }
                        }
                        if (isCompatible) compatible.Add(e);
                    }

                    if (compatible.Count == 0) break;

                    Enchantment next = compatible[random.Next(compatible.Count)];
                    int nextLevel = GetRandomEnchantmentLevel(next, finalCost / 2, random);
                    result.Add(new EnchantmentInstance { EnchantmentId = next.Id, Level = nextLevel });
                    finalCost /= 2;
                }
            }

            return result;
        }

        private static int GetRandomEnchantmentLevel(Enchantment enchantment, int cost, Random random)
        {
            int minLevel = enchantment.MinLevel;
            int maxLevel = Math.Min(enchantment.MaxLevel, cost / enchantment.MaxLevelCost + 1);
            if (maxLevel < minLevel) maxLevel = minLevel;
            return random.Next(minLevel, maxLevel + 1);
        }

        public static Dictionary<int, Enchantment> GetAllEnchantments()
        {
            return new Dictionary<int, Enchantment>(enchantments);
        }
    }
}
