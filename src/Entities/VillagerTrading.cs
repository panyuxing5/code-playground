using System;
using System.Collections.Generic;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.Entities
{
    public class VillagerTrade
    {
        public ItemStack Input1 { get; set; }
        public ItemStack Input2 { get; set; }
        public ItemStack Output { get; set; }
        public int MaxUses { get; set; }
        public int Uses { get; set; }
        public bool IsDisabled => Uses >= MaxUses;
        public int PriceMultiplier { get; set; }
        public int Demand { get; set; }
        public int SpecialPrice { get; set; }

        public VillagerTrade()
        {
            Input1 = new ItemStack(0, 0);
            Input2 = new ItemStack(0, 0);
            Output = new ItemStack(0, 0);
            MaxUses = 12;
            Uses = 0;
            PriceMultiplier = 0;
            Demand = 0;
            SpecialPrice = 0;
        }

        public bool CanTrade()
        {
            return !IsDisabled;
        }

        public void Use()
        {
            Uses++;
        }

        public void ResetUses()
        {
            Uses = 0;
        }
    }

    public class VillagerProfession
    {
        public string Name { get; set; }
        public string TranslationKey { get; set; }
        public int WorkstationBlock { get; set; }
        public List<VillagerTrade[]> Trades { get; set; }

        // 静态职业实例
        public static readonly VillagerProfession None = new VillagerProfession { Name = "None" };
        public static readonly VillagerProfession Armorer = new VillagerProfession { Name = "Armorer" };
        public static readonly VillagerProfession Butcher = new VillagerProfession { Name = "Butcher" };
        public static readonly VillagerProfession Cartographer = new VillagerProfession { Name = "Cartographer" };
        public static readonly VillagerProfession Cleric = new VillagerProfession { Name = "Cleric" };
        public static readonly VillagerProfession Farmer = new VillagerProfession { Name = "Farmer" };
        public static readonly VillagerProfession Fisherman = new VillagerProfession { Name = "Fisherman" };
        public static readonly VillagerProfession Fletcher = new VillagerProfession { Name = "Fletcher" };
        public static readonly VillagerProfession Leatherworker = new VillagerProfession { Name = "Leatherworker" };
        public static readonly VillagerProfession Librarian = new VillagerProfession { Name = "Librarian" };
        public static readonly VillagerProfession Mason = new VillagerProfession { Name = "Mason" };
        public static readonly VillagerProfession Shepherd = new VillagerProfession { Name = "Shepherd" };
        public static readonly VillagerProfession ToolSmith = new VillagerProfession { Name = "ToolSmith" };
        public static readonly VillagerProfession WeaponSmith = new VillagerProfession { Name = "WeaponSmith" };

        public VillagerProfession()
        {
            Trades = new List<VillagerTrade[]>();
            for (int i = 0; i < 5; i++)
            {
                Trades.Add(new VillagerTrade[0]);
            }
        }
    }

    public class VillagerTrading
    {
        private static readonly Dictionary<string, VillagerProfession> professions = new Dictionary<string, VillagerProfession>();
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllProfessions();
            Console.WriteLine("[VillagerTrading] 村民交易系统初始化完成，共注册 " + professions.Count + " 个职业");
        }

        private static void RegisterAllProfessions()
        {
            RegisterFarmer();
            RegisterFletcher();
            RegisterFisherman();
            RegisterShepherd();
            RegisterFletcher();
            RegisterLibrarian();
            RegisterCartographer();
            RegisterCleric();
            RegisterArmorer();
            RegisterWeaponSmith();
            RegisterToolSmith();
            RegisterButcher();
            RegisterLeatherworker();
            RegisterMason();
            RegisterNitwit();
            RegisterUnemployed();
        }

        private static void RegisterFarmer()
        {
            VillagerProfession farmer = new VillagerProfession
            {
                Name = "农民",
                TranslationKey = "entity.minecraft.villager.farmer",
                WorkstationBlock = GameConstants.BLOCK_COMPOSTER
            };

            // 等级1交易
            farmer.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 20), Output = new ItemStack(GameConstants.BLOCK_WHEAT, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 22), Output = new ItemStack(GameConstants.BLOCK_POTATO, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 22), Output = new ItemStack(GameConstants.BLOCK_CARROT, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(406, 1), Output = new ItemStack(500, 1), MaxUses = 16 }
            };

            // 等级2交易
            farmer.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 15), Output = new ItemStack(407, 6), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(GameConstants.BLOCK_PUMPKIN, 6), MaxUses = 16 }
            };

            // 等级3交易
            farmer.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(GameConstants.BLOCK_MELON, 4), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(409, 1), MaxUses = 12 }
            };

            // 等级4交易
            farmer.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 4), Output = new ItemStack(410, 1), MaxUses = 12 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(GameConstants.BLOCK_CAKE, 1), MaxUses = 12 }
            };

            // 等级5交易
            farmer.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(GameConstants.BLOCK_GLOW_BERRIES, 3), MaxUses = 12 }
            };

            professions["farmer"] = farmer;
        }

        private static void RegisterFletcher()
        {
            VillagerProfession fletcher = new VillagerProfession
            {
                Name = "制箭师",
                TranslationKey = "entity.minecraft.villager.fletcher",
                WorkstationBlock = GameConstants.BLOCK_FLETCHING_TABLE
            };

            fletcher.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(510, 32), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(520, 16), MaxUses = 16 }
            };

            fletcher.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 2), Output = new ItemStack(521, 1), MaxUses = 12 }
            };

            fletcher.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(522, 1), MaxUses = 12 }
            };

            fletcher.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 8), Output = new ItemStack(523, 1), MaxUses = 8 }
            };

            fletcher.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 14), Output = new ItemStack(524, 1), MaxUses = 4 }
            };

            professions["fletcher"] = fletcher;
        }

        private static void RegisterFisherman()
        {
            VillagerProfession fisherman = new VillagerProfession
            {
                Name = "渔夫",
                TranslationKey = "entity.minecraft.villager.fisherman",
                WorkstationBlock = GameConstants.BLOCK_BARREL
            };

            fisherman.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(598, 10), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(620, 1), MaxUses = 16 }
            };

            fisherman.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(599, 10), Output = new ItemStack(500, 1), MaxUses = 16 }
            };

            fisherman.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 8), Output = new ItemStack(621, 1), MaxUses = 12 }
            };

            fisherman.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 10), Output = new ItemStack(622, 1), MaxUses = 12 }
            };

            fisherman.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 15), Output = new ItemStack(623, 1), MaxUses = 8 }
            };

            professions["fisherman"] = fisherman;
        }

        private static void RegisterShepherd()
        {
            VillagerProfession shepherd = new VillagerProfession
            {
                Name = "牧羊人",
                TranslationKey = "entity.minecraft.villager.shepherd",
                WorkstationBlock = GameConstants.BLOCK_LOOM
            };

            shepherd.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 18), Output = new ItemStack(GameConstants.BLOCK_WOOL, 1), MaxUses = 16 }
            };

            shepherd.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(630, 1), MaxUses = 16 }
            };

            shepherd.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 4), Output = new ItemStack(631, 1), MaxUses = 12 }
            };

            shepherd.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(632, 1), MaxUses = 12 }
            };

            shepherd.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 6), Output = new ItemStack(633, 1), MaxUses = 8 }
            };

            professions["shepherd"] = shepherd;
        }

        private static void RegisterLibrarian()
        {
            VillagerProfession librarian = new VillagerProfession
            {
                Name = "图书管理员",
                TranslationKey = "entity.minecraft.villager.librarian",
                WorkstationBlock = GameConstants.BLOCK_LECTERN
            };

            librarian.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(640, 24), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(GameConstants.BLOCK_BOOKSHELF, 1), MaxUses = 16 }
            };

            librarian.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 20), Output = new ItemStack(641, 1), MaxUses = 12 }
            };

            librarian.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 10), Output = new ItemStack(GameConstants.BLOCK_COMPASS, 1), MaxUses = 12 },
                new VillagerTrade { Input1 = new ItemStack(500, 10), Output = new ItemStack(GameConstants.BLOCK_CLOCK, 1), MaxUses = 12 }
            };

            librarian.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 20), Output = new ItemStack(642, 1), MaxUses = 8 }
            };

            librarian.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 30), Output = new ItemStack(GameConstants.BLOCK_NAME_TAG, 1), MaxUses = 4 }
            };

            professions["librarian"] = librarian;
        }

        private static void RegisterCartographer()
        {
            VillagerProfession cartographer = new VillagerProfession
            {
                Name = "制图师",
                TranslationKey = "entity.minecraft.villager.cartographer",
                WorkstationBlock = GameConstants.BLOCK_CARTOGRAPHY_TABLE
            };

            cartographer.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(640, 24), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 7), Output = new ItemStack(GameConstants.BLOCK_MAP, 1), MaxUses = 16 }
            };

            cartographer.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 13), Input2 = new ItemStack(GameConstants.BLOCK_MAP, 1), Output = new ItemStack(650, 1), MaxUses = 12 }
            };

            cartographer.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 14), Output = new ItemStack(GameConstants.BLOCK_ITEM_FRAME, 1), MaxUses = 12 }
            };

            cartographer.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 30), Output = new ItemStack(651, 1), MaxUses = 8 }
            };

            cartographer.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 40), Output = new ItemStack(652, 1), MaxUses = 4 }
            };

            professions["cartographer"] = cartographer;
        }

        private static void RegisterCleric()
        {
            VillagerProfession cleric = new VillagerProfession
            {
                Name = "牧师",
                TranslationKey = "entity.minecraft.villager.cleric",
                WorkstationBlock = GameConstants.BLOCK_BREWING_STAND
            };

            cleric.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(507, 32), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(660, 3), MaxUses = 16 }
            };

            cleric.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(509, 32), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(661, 1), MaxUses = 12 }
            };

            cleric.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(506, 32), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 4), Output = new ItemStack(662, 1), MaxUses = 12 }
            };

            cleric.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(663, 1), MaxUses = 8 }
            };

            cleric.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 25), Output = new ItemStack(GameConstants.BLOCK_END_EYE, 1), MaxUses = 4 }
            };

            professions["cleric"] = cleric;
        }

        private static void RegisterArmorer()
        {
            VillagerProfession armorer = new VillagerProfession
            {
                Name = "盔甲商",
                TranslationKey = "entity.minecraft.villager.armorer",
                WorkstationBlock = GameConstants.BLOCK_BLAST_FURNACE
            };

            armorer.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(502, 15), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(670, 1), MaxUses = 16 }
            };

            armorer.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 7), Output = new ItemStack(671, 1), MaxUses = 12 }
            };

            armorer.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 9), Output = new ItemStack(672, 1), MaxUses = 12 }
            };

            armorer.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 12), Output = new ItemStack(673, 1), MaxUses = 8 }
            };

            armorer.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 18), Output = new ItemStack(674, 1), MaxUses = 4 }
            };

            professions["armorer"] = armorer;
        }

        private static void RegisterWeaponSmith()
        {
            VillagerProfession weaponSmith = new VillagerProfession
            {
                Name = "武器商",
                TranslationKey = "entity.minecraft.villager.weaponsmith",
                WorkstationBlock = GameConstants.BLOCK_GRINDSTONE
            };

            weaponSmith.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(502, 15), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(680, 1), MaxUses = 16 }
            };

            weaponSmith.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 7), Output = new ItemStack(681, 1), MaxUses = 12 }
            };

            weaponSmith.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 10), Output = new ItemStack(682, 1), MaxUses = 12 }
            };

            weaponSmith.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 15), Output = new ItemStack(683, 1), MaxUses = 8 }
            };

            weaponSmith.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 20), Output = new ItemStack(684, 1), MaxUses = 4 }
            };

            professions["weaponsmith"] = weaponSmith;
        }

        private static void RegisterToolSmith()
        {
            VillagerProfession toolSmith = new VillagerProfession
            {
                Name = "工具商",
                TranslationKey = "entity.minecraft.villager.toolsmith",
                WorkstationBlock = GameConstants.BLOCK_SMITHING_TABLE
            };

            toolSmith.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(510, 32), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(690, 1), MaxUses = 16 }
            };

            toolSmith.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 7), Output = new ItemStack(691, 1), MaxUses = 12 }
            };

            toolSmith.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 9), Output = new ItemStack(692, 1), MaxUses = 12 }
            };

            toolSmith.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 12), Output = new ItemStack(693, 1), MaxUses = 8 }
            };

            toolSmith.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 18), Output = new ItemStack(694, 1), MaxUses = 4 }
            };

            professions["toolsmith"] = toolSmith;
        }

        private static void RegisterButcher()
        {
            VillagerProfession butcher = new VillagerProfession
            {
                Name = "屠夫",
                TranslationKey = "entity.minecraft.villager.butcher",
                WorkstationBlock = GameConstants.BLOCK_SMOKER
            };

            butcher.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(590, 14), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(402, 5), MaxUses = 16 }
            };

            butcher.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(591, 10), Output = new ItemStack(500, 1), MaxUses = 16 }
            };

            butcher.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(700, 1), MaxUses = 12 }
            };

            butcher.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 8), Output = new ItemStack(701, 1), MaxUses = 8 }
            };

            butcher.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 12), Output = new ItemStack(702, 1), MaxUses = 4 }
            };

            professions["butcher"] = butcher;
        }

        private static void RegisterLeatherworker()
        {
            VillagerProfession leatherworker = new VillagerProfession
            {
                Name = "皮匠",
                TranslationKey = "entity.minecraft.villager.leatherworker",
                WorkstationBlock = GameConstants.BLOCK_CAULDRON
            };

            leatherworker.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(710, 6), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 3), Output = new ItemStack(711, 1), MaxUses = 16 }
            };

            leatherworker.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 7), Output = new ItemStack(712, 1), MaxUses = 12 }
            };

            leatherworker.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 10), Output = new ItemStack(713, 1), MaxUses = 12 }
            };

            leatherworker.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 15), Output = new ItemStack(714, 1), MaxUses = 8 }
            };

            leatherworker.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 20), Output = new ItemStack(715, 1), MaxUses = 4 }
            };

            professions["leatherworker"] = leatherworker;
        }

        private static void RegisterMason()
        {
            VillagerProfession mason = new VillagerProfession
            {
                Name = "石匠",
                TranslationKey = "entity.minecraft.villager.mason",
                WorkstationBlock = GameConstants.BLOCK_STONECUTTER
            };

            mason.Trades[0] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(GameConstants.BLOCK_STONE, 20), Output = new ItemStack(500, 1), MaxUses = 16 },
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(GameConstants.BLOCK_STONE_BRICKS, 4), MaxUses = 16 }
            };

            mason.Trades[1] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 1), Output = new ItemStack(GameConstants.BLOCK_POLISHED_ANDESITE, 4), MaxUses = 16 }
            };

            mason.Trades[2] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 4), Output = new ItemStack(GameConstants.BLOCK_TERRACOTTA, 1), MaxUses = 12 }
            };

            mason.Trades[3] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 5), Output = new ItemStack(GameConstants.BLOCK_QUARTZ_BLOCK, 1), MaxUses = 8 }
            };

            mason.Trades[4] = new[]
            {
                new VillagerTrade { Input1 = new ItemStack(500, 8), Output = new ItemStack(GameConstants.BLOCK_GILDED_BLACKSTONE, 1), MaxUses = 4 }
            };

            professions["mason"] = mason;
        }

        private static void RegisterNitwit()
        {
            VillagerProfession nitwit = new VillagerProfession
            {
                Name = "傻子",
                TranslationKey = "entity.minecraft.villager.nitwit",
                WorkstationBlock = 0
            };

            professions["nitwit"] = nitwit;
        }

        private static void RegisterUnemployed()
        {
            VillagerProfession unemployed = new VillagerProfession
            {
                Name = "失业",
                TranslationKey = "entity.minecraft.villager.unemployed",
                WorkstationBlock = 0
            };

            professions["unemployed"] = unemployed;
        }

        public static VillagerProfession GetProfession(string name)
        {
            professions.TryGetValue(name, out VillagerProfession profession);
            return profession;
        }

        public static Dictionary<string, VillagerProfession> GetAllProfessions()
        {
            return new Dictionary<string, VillagerProfession>(professions);
        }

        public static VillagerTrade[] GetTradesForLevel(string profession, int level)
        {
            if (professions.TryGetValue(profession, out VillagerProfession prof))
            {
                if (level >= 0 && level < prof.Trades.Count)
                {
                    return prof.Trades[level];
                }
            }
            return new VillagerTrade[0];
        }

        public static string GetProfessionName(string profession)
        {
            if (professions.TryGetValue(profession, out VillagerProfession prof))
            {
                return prof.Name;
            }
            return "未知";
        }
    }
}
