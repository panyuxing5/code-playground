using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class AchievementSystem
    {
        private static readonly Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();
        private static bool isInitialized;

        private readonly HashSet<string> unlockedAchievements;
        private readonly Dictionary<string, int> progress;

        public event Action<string> OnAchievementUnlocked;
        public event Action<string, int> OnAchievementProgress;

        public AchievementSystem()
        {
            unlockedAchievements = new HashSet<string>();
            progress = new Dictionary<string, int>();
        }

        public int TotalAchievements => achievements.Count;
        public int UnlockedAchievements => unlockedAchievements.Count;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllAchievements();
            Console.WriteLine("[AchievementSystem] 成就系统初始化完成，共注册 " + achievements.Count + " 个成就");
        }

        private static void RegisterAllAchievements()
        {
            // 故事成就
            RegisterStoryAchievements();
            // 下界成就
            RegisterNetherAchievements();
            // 末地成就
            RegisterEndAchievements();
            // 冒险成就
            RegisterAdventureAchievements();
            // 农牧业成就
            RegisterHusbandryAchievements();
        }

        private static void RegisterStoryAchievements()
        {
            achievements["story.root"] = new Achievement
            {
                Id = "story.root",
                Name = "我的世界",
                Description = "在物品栏中获得工作台",
                Icon = "crafting_table",
                Parent = null,
                Target = 1,
                Points = 0
            };

            achievements["story.mine_stone"] = new Achievement
            {
                Id = "story.mine_stone",
                Name = "石器时代",
                Description = "用你的镐开采石头",
                Icon = "stone",
                Parent = "story.root",
                Target = 1,
                Points = 20
            };

            achievements["story.upgrade_tools"] = new Achievement
            {
                Id = "story.upgrade_tools",
                Name = "获得升级",
                Description = "在工作台中用圆石制作石质工具",
                Icon = "stone_pickaxe",
                Parent = "story.mine_stone",
                Target = 1,
                Points = 40
            };

            achievements["story.smelt_iron"] = new Achievement
            {
                Id = "story.smelt_iron",
                Name = "来硬的",
                Description = "在熔炉中冶炼铁矿石",
                Icon = "iron_ingot",
                Parent = "story.upgrade_tools",
                Target = 1,
                Points = 60
            };

            achievements["story.build_pickaxe"] = new Achievement
            {
                Id = "story.build_pickaxe",
                Name = "这不是铁镐么",
                Description = "在工作台中用铁锭制作铁镐",
                Icon = "iron_pickaxe",
                Parent = "story.smelt_iron",
                Target = 1,
                Points = 80
            };

            achievements["story.obtain_armor"] = new Achievement
            {
                Id = "story.obtain_armor",
                Name = "整装上阵",
                Description = "在工作台中用铁锭制作铁制护甲",
                Icon = "iron_chestplate",
                Parent = "story.smelt_iron",
                Target = 1,
                Points = 80
            };

            achievements["story.lava_bucket"] = new Achievement
            {
                Id = "story.lava_bucket",
                Name = "热腾腾的",
                Description = "用桶收集岩浆",
                Icon = "lava_bucket",
                Parent = "story.build_pickaxe",
                Target = 1,
                Points = 100
            };

            achievements["story.iron_to_diamond"] = new Achievement
            {
                Id = "story.iron_to_diamond",
                Name = "钻石！",
                Description = "在物品栏中获得钻石",
                Icon = "diamond",
                Parent = "story.build_pickaxe",
                Target = 1,
                Points = 100
            };

            achievements["story.enter_nether"] = new Achievement
            {
                Id = "story.enter_nether",
                Name = "我们需要再深入些",
                Description = "进入下界维度",
                Icon = "nether_portal",
                Parent = "story.lava_bucket",
                Target = 1,
                Points = 120
            };

            achievements["story.shiny_gear"] = new Achievement
            {
                Id = "story.shiny_gear",
                Name = "闪闪发光",
                Description = "在工作台中用钻石制作钻石工具或护甲",
                Icon = "diamond_sword",
                Parent = "story.iron_to_diamond",
                Target = 1,
                Points = 120
            };
        }

        private static void RegisterNetherAchievements()
        {
            achievements["nether.root"] = new Achievement
            {
                Id = "nether.root",
                Name = "下界",
                Description = "进入下界",
                Icon = "netherrack",
                Parent = null,
                Target = 1,
                Points = 0
            };

            achievements["nether.return_to_sender"] = new Achievement
            {
                Id = "nether.return_to_sender",
                Name = "谁在切洋葱？",
                Description = "用火球击杀恶魂",
                Icon = "ghast_tear",
                Parent = "nether.root",
                Target = 1,
                Points = 50
            };

            achievements["nether.find_bastion"] = new Achievement
            {
                Id = "nether.find_bastion",
                Name = "那些是目标吗？",
                Description = "进入猪灵堡垒",
                Icon = "piglin_brute_spawn_egg",
                Parent = "nether.root",
                Target = 1,
                Points = 30
            };

            achievements["nether.loot_bastion"] = new Achievement
            {
                Id = "nether.loot_bastion",
                Name = "战利品猎人",
                Description = "打开猪灵堡垒中的箱子",
                Icon = "chest",
                Parent = "nether.find_bastion",
                Target = 1,
                Points = 50
            };

            achievements["nether.obtain_ancient_debris"] = new Achievement
            {
                Id = "nether.obtain_ancient_debris",
                Name = "深藏不露",
                Description = "在物品栏中获得远古残骸",
                Icon = "ancient_debris",
                Parent = "nether.root",
                Target = 1,
                Points = 50
            };

            achievements["nether.netherite"] = new Achievement
            {
                Id = "nether.netherite",
                Name = "深藏不露",
                Description = "在锻造台中用下界合金锭升级钻石装备",
                Icon = "netherite_pickaxe",
                Parent = "nether.obtain_ancient_debris",
                Target = 1,
                Points = 100
            };

            achievements["nether.all_effects"] = new Achievement
            {
                Id = "nether.all_effects",
                Name = "为什么会变成这样呢？",
                Description = "同时拥有所有状态效果",
                Icon = "beacon",
                Parent = "nether.root",
                Target = 1,
                Points = 100
            };

            achievements["nether.fast_travel"] = new Achievement
            {
                Id = "nether.fast_travel",
                Name = "狂奔",
                Description = "在下界中使用鞘翅飞行7公里",
                Icon = "elytra",
                Parent = "nether.root",
                Target = 7000,
                Points = 100
            };

            achievements["nether.uneasy_alliance"] = new Achievement
            {
                Id = "nether.uneasy_alliance",
                Name = "不稳定的同盟",
                Description = "在下界中击杀一只恶魂",
                Icon = "ghast_spawn_egg",
                Parent = "nether.root",
                Target = 1,
                Points = 50
            };

            achievements["nether.ride_strider"] = new Achievement
            {
                Id = "nether.ride_strider",
                Name = "脚下留情",
                Description = "用诡异菌钓竿骑乘炽足兽在岩浆上前进50格",
                Icon = "strider_spawn_egg",
                Parent = "nether.root",
                Target = 50,
                Points = 50
            };

            achievements["nether.ghast"] = new Achievement
            {
                Id = "nether.ghast",
                Name = "见鬼去吧",
                Description = "用弹射物击杀一只恶魂",
                Icon = "ghast_tear",
                Parent = "nether.root",
                Target = 1,
                Points = 50
            };

            achievements["nether.wither"] = new Achievement
            {
                Id = "nether.wither",
                Name = "带点恶魂之泪回家",
                Description = "在物品栏中获得恶魂之泪",
                Icon = "ghast_tear",
                Parent = "nether.root",
                Target = 1,
                Points = 30
            };
        }

        private static void RegisterEndAchievements()
        {
            achievements["end.root"] = new Achievement
            {
                Id = "end.root",
                Name = "末地",
                Description = "进入末地",
                Icon = "end_stone",
                Parent = null,
                Target = 1,
                Points = 0
            };

            achievements["end.kill_dragon"] = new Achievement
            {
                Id = "end.kill_dragon",
                Name = "解放末地",
                Description = "击杀末影龙",
                Icon = "dragon_breath",
                Parent = "end.root",
                Target = 1,
                Points = 100
            };

            achievements["end.dragon_egg"] = new Achievement
            {
                Id = "end.dragon_egg",
                Name = "下一世代",
                Description = "在物品栏中获得龙蛋",
                Icon = "dragon_egg",
                Parent = "end.kill_dragon",
                Target = 1,
                Points = 100
            };

            achievements["end.respawn_dragon"] = new Achievement
            {
                Id = "end.respawn_dragon",
                Name = "那是飞机吗？",
                Description = "用末影水晶复活末影龙",
                Icon = "end_crystal",
                Parent = "end.kill_dragon",
                Target = 1,
                Points = 100
            };

            achievements["end.elytra"] = new Achievement
            {
                Id = "end.elytra",
                Name = "在宇宙的尽头",
                Description = "在物品栏中获得鞘翅",
                Icon = "elytra",
                Parent = "end.kill_dragon",
                Target = 1,
                Points = 100
            };

            achievements["end.dragon_breath"] = new Achievement
            {
                Id = "end.dragon_breath",
                Name = "你需要来点薄荷吗？",
                Description = "在物品栏中获得龙息",
                Icon = "dragon_breath",
                Parent = "end.kill_dragon",
                Target = 1,
                Points = 50
            };
        }

        private static void RegisterAdventureAchievements()
        {
            achievements["adventure.root"] = new Achievement
            {
                Id = "adventure.root",
                Name = "冒险",
                Description = "杀死任意怪物或被任意怪物杀死",
                Icon = "iron_sword",
                Parent = null,
                Target = 1,
                Points = 0
            };

            achievements["adventure.voluntary_exile"] = new Achievement
            {
                Id = "adventure.voluntary_exile",
                Name = "自愿的流亡",
                Description = "击杀一名灾厄村民",
                Icon = "pillager_spawn_egg",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.spyglass_at_parrot"] = new Achievement
            {
                Id = "adventure.spyglass_at_parrot",
                Name = "望远镜里的鹦鹉",
                Description = "用望远镜观察鹦鹉",
                Icon = "spyglass",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.avoid_vibration"] = new Achievement
            {
                Id = "adventure.avoid_vibration",
                Name = "非常静谧",
                Description = "在潜行时躲避幽匿感测体的振动检测",
                Icon = "sculk_sensor",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.walk_on_powder_snow"] = new Achievement
            {
                Id = "adventure.walk_on_powder_snow",
                Name = "穿着皮革靴子走在细雪上",
                Description = "穿着皮革靴子在细雪上行走",
                Icon = "leather_boots",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.lightning_rod"] = new Achievement
            {
                Id = "adventure.lightning_rod",
                Name = "用三叉戟击中避雷针",
                Description = "在雷暴天气用引雷三叉戟击中避雷针",
                Icon = "lightning_rod",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.kill_mob_near_sculk"] = new Achievement
            {
                Id = "adventure.kill_mob_near_sculk",
                Name = "在幽匿催发体附近杀死生物",
                Description = "在幽匿催发体附近杀死生物",
                Icon = "sculk_catalyst",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.bullseye"] = new Achievement
            {
                Id = "adventure.bullseye",
                Name = "正中靶心",
                Description = "从至少30格外用弹射物击中标靶的靶心",
                Icon = "target",
                Parent = "adventure.root",
                Target = 1,
                Points = 50
            };

            achievements["adventure.summon_iron_golem"] = new Achievement
            {
                Id = "adventure.summon_iron_golem",
                Name = "英雄住在这里",
                Description = "用铁块和雕刻过的南瓜生成铁傀儡",
                Icon = "iron_golem_spawn_egg",
                Parent = "adventure.root",
                Target = 1,
                Points = 50
            };

            achievements["adventure.summon_wither"] = new Achievement
            {
                Id = "adventure.summon_wither",
                Name = "开始了",
                Description = "召唤凋灵",
                Icon = "wither_skeleton_skull",
                Parent = "adventure.root",
                Target = 1,
                Points = 50
            };

            achievements["adventure.kill_wither"] = new Achievement
            {
                Id = "adventure.kill_wither",
                Name = "下界之星",
                Description = "击杀凋灵",
                Icon = "nether_star",
                Parent = "adventure.summon_wither",
                Target = 1,
                Points = 100
            };

            achievements["adventure.trade"] = new Achievement
            {
                Id = "adventure.trade",
                Name = "成交！",
                Description = "与村民交易",
                Icon = "emerald",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.throw_trident"] = new Achievement
            {
                Id = "adventure.throw_trident",
                Name = "一去不返",
                Description = "投掷三叉戟",
                Icon = "trident",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.shoot_arrow"] = new Achievement
            {
                Id = "adventure.shoot_arrow",
                Name = "瞄准目标",
                Description = "用弓射出箭",
                Icon = "arrow",
                Parent = "adventure.root",
                Target = 1,
                Points = 30
            };

            achievements["adventure.sniper_duel"] = new Achievement
            {
                Id = "adventure.sniper_duel",
                Name = "狙击手的对决",
                Description = "从至少50格外用箭击杀骷髅",
                Icon = "skeleton_skull",
                Parent = "adventure.shoot_arrow",
                Target = 1,
                Points = 50
            };

            achievements["adventure.totem_of_undying"] = new Achievement
            {
                Id = "adventure.totem_of_undying",
                Name = "不死的图腾",
                Description = "用不死图腾复活",
                Icon = "totem_of_undying",
                Parent = "adventure.root",
                Target = 1,
                Points = 100
            };

            achievements["adventure.hero_of_the_village"] = new Achievement
            {
                Id = "adventure.hero_of_the_village",
                Name = "村庄英雄",
                Description = "赢得一场袭击",
                Icon = "hero_of_the_village",
                Parent = "adventure.root",
                Target = 1,
                Points = 100
            };
        }

        private static void RegisterHusbandryAchievements()
        {
            achievements["husbandry.root"] = new Achievement
            {
                Id = "husbandry.root",
                Name = "农牧业",
                Description = "在物品栏中获得树叶",
                Icon = "oak_sapling",
                Parent = null,
                Target = 1,
                Points = 0
            };

            achievements["husbandry.safely_harvest_honey"] = new Achievement
            {
                Id = "husbandry.safely_harvest_honey",
                Name = "安全获取蜂蜜",
                Description = "在不激怒蜜蜂的情况下从蜂巢或蜂箱中收集蜂蜜瓶",
                Icon = "honey_bottle",
                Parent = "husbandry.root",
                Target = 1,
                Points = 50
            };

            achievements["husbandry.axolotl_in_a_bucket"] = new Achievement
            {
                Id = "husbandry.axolotl_in_a_bucket",
                Name = "最可爱的捕食者",
                Description = "用桶装起美西螈",
                Icon = "axolotl_bucket",
                Parent = "husbandry.root",
                Target = 1,
                Points = 30
            };

            achievements["husbandry.kill_axolotl_target"] = new Achievement
            {
                Id = "husbandry.kill_axolotl_target",
                Name = "两栖动物的胜利",
                Description = "在美西螈的帮助下杀死一只发光鱿鱼",
                Icon = "glow_squid_spawn_egg",
                Parent = "husbandry.axolotl_in_a_bucket",
                Target = 1,
                Points = 50
            };

            achievements["husbandry.breed_all_animals"] = new Achievement
            {
                Id = "husbandry.breed_all_animals",
                Name = "成双成对",
                Description = "繁殖所有可繁殖的动物",
                Icon = "heart",
                Parent = "husbandry.root",
                Target = 1,
                Points = 100
            };

            achievements["husbandry.plant_seed"] = new Achievement
            {
                Id = "husbandry.plant_seed",
                Name = "播种",
                Description = "在耕地上种植种子",
                Icon = "wheat_seeds",
                Parent = "husbandry.root",
                Target = 1,
                Points = 20
            };

            achievements["husbandry.bread"] = new Achievement
            {
                Id = "husbandry.bread",
                Name = "填饱肚子",
                Description = "在工作台中用小麦制作面包",
                Icon = "bread",
                Parent = "husbandry.plant_seed",
                Target = 1,
                Points = 40
            };

            achievements["husbandry.bake_cake"] = new Achievement
            {
                Id = "husbandry.bake_cake",
                Name = "蛋糕是谎言",
                Description = "在工作台中用小麦、鸡蛋、牛奶和糖制作蛋糕",
                Icon = "cake",
                Parent = "husbandry.bread",
                Target = 1,
                Points = 60
            };

            achievements["husbandry.ride_boat"] = new Achievement
            {
                Id = "husbandry.ride_boat",
                Name = "水上漂",
                Description = "乘船航行",
                Icon = "oak_boat",
                Parent = "husbandry.root",
                Target = 1,
                Points = 20
            };

            achievements["husbandry.tame_animal"] = new Achievement
            {
                Id = "husbandry.tame_animal",
                Name = "永恒的伙伴",
                Description = "驯服一只动物",
                Icon = "wolf",
                Parent = "husbandry.root",
                Target = 1,
                Points = 30
            };

            achievements["husbandry.breed_animal"] = new Achievement
            {
                Id = "husbandry.breed_animal",
                Name = "我还能养点什么？",
                Description = "繁殖两只动物",
                Icon = "cow",
                Parent = "husbandry.tame_animal",
                Target = 1,
                Points = 40
            };

            achievements["husbandry.netherite_hoes"] = new Achievement
            {
                Id = "husbandry.netherite_hoes",
                Name = "最珍贵的物品",
                Description = "在锻造台中用下界合金锭升级钻石锄",
                Icon = "netherite_hoe",
                Parent = "husbandry.root",
                Target = 1,
                Points = 100
            };

            achievements["husbandry.balanced_diet"] = new Achievement
            {
                Id = "husbandry.balanced_diet",
                Name = "均衡饮食",
                Description = "吃下所有能吃的食物",
                Icon = "golden_apple",
                Parent = "husbandry.root",
                Target = 1,
                Points = 100
            };

            achievements["husbandry.allay_deliver_item"] = new Achievement
            {
                Id = "husbandry.allay_deliver_item",
                Name = "递送物品",
                Description = "让悦灵为你递送物品",
                Icon = "allay_spawn_egg",
                Parent = "husbandry.root",
                Target = 1,
                Points = 50
            };

            achievements["husbandry.leash_all_frog_variants"] = new Achievement
            {
                Id = "husbandry.leash_all_frog_variants",
                Name = "拴住所有青蛙变种",
                Description = "用拴绳拴住所有3种青蛙变种",
                Icon = "lead",
                Parent = "husbandry.root",
                Target = 3,
                Points = 50
            };
        }

        public void UnlockAchievement(string id)
        {
            if (!achievements.ContainsKey(id)) return;
            if (unlockedAchievements.Contains(id)) return;

            unlockedAchievements.Add(id);
            OnAchievementUnlocked?.Invoke(id);

            Achievement achievement = achievements[id];
            Console.WriteLine($"[AchievementSystem] 解锁成就: {achievement.Name}");
        }

        public void UpdateProgress(string id, int value)
        {
            if (!achievements.ContainsKey(id)) return;

            progress[id] = value;
            OnAchievementProgress?.Invoke(id, value);

            Achievement achievement = achievements[id];
            if (value >= achievement.Target)
            {
                UnlockAchievement(id);
            }
        }

        public void IncrementProgress(string id, int amount = 1)
        {
            if (!achievements.ContainsKey(id)) return;

            if (!progress.ContainsKey(id))
            {
                progress[id] = 0;
            }

            progress[id] += amount;
            UpdateProgress(id, progress[id]);
        }

        public bool IsUnlocked(string id)
        {
            return unlockedAchievements.Contains(id);
        }

        public int GetProgress(string id)
        {
            return progress.TryGetValue(id, out int value) ? value : 0;
        }

        public Achievement GetAchievement(string id)
        {
            return achievements.TryGetValue(id, out Achievement achievement) ? achievement : null;
        }

        public List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>(achievements.Values);
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> result = new List<Achievement>();
            foreach (string id in unlockedAchievements)
            {
                if (achievements.TryGetValue(id, out Achievement achievement))
                {
                    result.Add(achievement);
                }
            }
            return result;
        }

        public List<Achievement> GetLockedAchievements()
        {
            List<Achievement> result = new List<Achievement>();
            foreach (Achievement achievement in achievements.Values)
            {
                if (!unlockedAchievements.Contains(achievement.Id))
                {
                    result.Add(achievement);
                }
            }
            return result;
        }

        public int GetTotalPoints()
        {
            int total = 0;
            foreach (string id in unlockedAchievements)
            {
                if (achievements.TryGetValue(id, out Achievement achievement))
                {
                    total += achievement.Points;
                }
            }
            return total;
        }

        public int GetUnlockedCount()
        {
            return unlockedAchievements.Count;
        }

        public int GetTotalCount()
        {
            return achievements.Count;
        }

        public void Reset()
        {
            unlockedAchievements.Clear();
            progress.Clear();
        }

        public void Save(string path)
        {
            // 保存成就进度
        }

        public void Load(string path)
        {
            // 加载成就进度
        }
    }

    public class Achievement
    {
        public string Id;
        public string Name;
        public string Description;
        public string Icon;
        public string Parent;
        public int Target;
        public int Points;
        public bool IsSecret;

        // 扩展属性和方法
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockTime { get; set; }
    }

    public static class AchievementExtensions
    {
        public static int TotalAchievements { get; set; }
        public static int UnlockedAchievements { get; set; }

        public static List<Achievement> GetAchievementsByCategory(this AchievementSystem system, string category)
        {
            return new List<Achievement>();
        }
    }
}
