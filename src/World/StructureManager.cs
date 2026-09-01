using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class StructureManager
    {
        private readonly WorldManager world;
        private readonly Dictionary<string, StructureTemplate> structureTemplates;
        private readonly Dictionary<string, StructureInstance> activeStructures;

        // 统计
        public int LoadedStructures => structureTemplates.Count;
        public int ActiveStructures => activeStructures.Count;
        public int StructuresGenerated { get; private set; }

        public StructureManager(WorldManager world)
        {
            this.world = world;
            structureTemplates = new Dictionary<string, StructureTemplate>();
            activeStructures = new Dictionary<string, StructureInstance>();
        }

        public void Initialize()
        {
            Console.WriteLine("[StructureManager] 结构管理器初始化完成");
            RegisterDefaultStructures();
        }

        private void RegisterDefaultStructures()
        {
            // 注册各种结构模板
            RegisterStructure(new StructureTemplate
            {
                Name = "village",
                Width = 32,
                Height = 16,
                Depth = 32,
                SpawnChance = 0.02f,
                MinY = 60,
                MaxY = 80,
                Biomes = new List<Biome> { Biome.Plains, Biome.Savanna, Biome.Desert, Biome.Taiga }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "desert_temple",
                Width = 21,
                Height = 10,
                Depth = 21,
                SpawnChance = 0.01f,
                MinY = 60,
                MaxY = 70,
                Biomes = new List<Biome> { Biome.Desert }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "jungle_temple",
                Width = 15,
                Height = 10,
                Depth = 15,
                SpawnChance = 0.01f,
                MinY = 60,
                MaxY = 80,
                Biomes = new List<Biome> { Biome.Jungle }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "witch_hut",
                Width = 9,
                Height = 7,
                Depth = 9,
                SpawnChance = 0.005f,
                MinY = 60,
                MaxY = 70,
                Biomes = new List<Biome> { Biome.Swamp }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "igloo",
                Width = 9,
                Height = 6,
                Depth = 9,
                SpawnChance = 0.005f,
                MinY = 60,
                MaxY = 70,
                Biomes = new List<Biome> { Biome.SnowyTundra, Biome.FrozenOcean }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "pillager_outpost",
                Width = 15,
                Height = 20,
                Depth = 15,
                SpawnChance = 0.008f,
                MinY = 60,
                MaxY = 80,
                Biomes = new List<Biome> { Biome.Plains, Biome.Desert, Biome.Savanna, Biome.Taiga }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "ruined_portal",
                Width = 10,
                Height = 10,
                Depth = 10,
                SpawnChance = 0.015f,
                MinY = 50,
                MaxY = 90,
                Biomes = new List<Biome>()
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "shipwreck",
                Width = 16,
                Height = 10,
                Depth = 32,
                SpawnChance = 0.01f,
                MinY = 30,
                MaxY = 60,
                Biomes = new List<Biome> { Biome.Ocean, Biome.DeepOcean, Biome.FrozenOcean }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "ocean_ruin",
                Width = 12,
                Height = 8,
                Depth = 12,
                SpawnChance = 0.012f,
                MinY = 30,
                MaxY = 55,
                Biomes = new List<Biome> { Biome.Ocean, Biome.DeepOcean }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "mineshaft",
                Width = 30,
                Height = 10,
                Depth = 30,
                SpawnChance = 0.02f,
                MinY = 10,
                MaxY = 50,
                Biomes = new List<Biome>()
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "stronghold",
                Width = 50,
                Height = 20,
                Depth = 50,
                SpawnChance = 0.001f,
                MinY = 10,
                MaxY = 40,
                Biomes = new List<Biome>()
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "dungeon",
                Width = 9,
                Height = 5,
                Depth = 9,
                SpawnChance = 0.03f,
                MinY = 10,
                MaxY = 60,
                Biomes = new List<Biome>()
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "nether_fortress",
                Width = 50,
                Height = 30,
                Depth = 50,
                SpawnChance = 0.005f,
                MinY = 30,
                MaxY = 80,
                Biomes = new List<Biome> { Biome.NetherWastes }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "bastion_remnant",
                Width = 30,
                Height = 20,
                Depth = 30,
                SpawnChance = 0.004f,
                MinY = 30,
                MaxY = 80,
                Biomes = new List<Biome> { Biome.NetherWastes, Biome.CrimsonForest, Biome.WarpedForest, Biome.SoulSandValley }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "end_city",
                Width = 20,
                Height = 40,
                Depth = 20,
                SpawnChance = 0.003f,
                MinY = 50,
                MaxY = 100,
                Biomes = new List<Biome> { Biome.TheEnd }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "ancient_city",
                Width = 60,
                Height = 20,
                Depth = 60,
                SpawnChance = 0.002f,
                MinY = -50,
                MaxY = 0,
                Biomes = new List<Biome> { Biome.DeepDark }
            });

            RegisterStructure(new StructureTemplate
            {
                Name = "trial_chambers",
                Width = 40,
                Height = 20,
                Depth = 40,
                SpawnChance = 0.003f,
                MinY = -20,
                MaxY = 20,
                Biomes = new List<Biome>()
            });

            Console.WriteLine($"[StructureManager] 已注册 {structureTemplates.Count} 个结构模板");
        }

        public void RegisterStructure(StructureTemplate template)
        {
            if (!structureTemplates.ContainsKey(template.Name))
            {
                structureTemplates[template.Name] = template;
            }
        }

        public void UnregisterStructure(string name)
        {
            structureTemplates.Remove(name);
        }

        public StructureTemplate GetStructureTemplate(string name)
        {
            if (structureTemplates.TryGetValue(name, out StructureTemplate template))
            {
                return template;
            }
            return null;
        }

        public List<string> GetAllStructureNames()
        {
            return new List<string>(structureTemplates.Keys);
        }

        public bool CanStructureSpawn(string structureName, int x, int y, int z, Biome biome)
        {
            if (!structureTemplates.TryGetValue(structureName, out StructureTemplate template))
            {
                return false;
            }

            // 检查Y坐标范围
            if (y < template.MinY || y > template.MaxY)
            {
                return false;
            }

            // 检查生物群系
            if (template.Biomes.Count > 0 && !template.Biomes.Contains(biome))
            {
                return false;
            }

            // 检查生成概率
            if (Random.Shared.NextDouble() > template.SpawnChance)
            {
                return false;
            }

            // 检查是否与其他结构重叠
            foreach (StructureInstance instance in activeStructures.Values)
            {
                if (Math.Abs(instance.X - x) < 50 && Math.Abs(instance.Z - z) < 50)
                {
                    return false;
                }
            }

            return true;
        }

        public void GenerateStructure(string structureName, int x, int y, int z)
        {
            if (!structureTemplates.TryGetValue(structureName, out StructureTemplate template))
            {
                Console.WriteLine($"[StructureManager] 未找到结构模板: {structureName}");
                return;
            }

            StructureInstance instance = new StructureInstance
            {
                Name = structureName,
                X = x,
                Y = y,
                Z = z,
                Width = template.Width,
                Height = template.Height,
                Depth = template.Depth,
                Generated = false
            };

            // 根据结构类型生成
            switch (structureName)
            {
                case "village":
                    GenerateVillage(instance);
                    break;

                case "desert_temple":
                    GenerateDesertTemple(instance);
                    break;

                case "jungle_temple":
                    GenerateJungleTemple(instance);
                    break;

                case "witch_hut":
                    GenerateWitchHut(instance);
                    break;

                case "igloo":
                    GenerateIgloo(instance);
                    break;

                case "pillager_outpost":
                    GeneratePillagerOutpost(instance);
                    break;

                case "ruined_portal":
                    GenerateRuinedPortal(instance);
                    break;

                case "shipwreck":
                    GenerateShipwreck(instance);
                    break;

                case "ocean_ruin":
                    GenerateOceanRuin(instance);
                    break;

                case "mineshaft":
                    GenerateMineshaft(instance);
                    break;

                case "stronghold":
                    GenerateStronghold(instance);
                    break;

                case "dungeon":
                    GenerateDungeon(instance);
                    break;

                case "nether_fortress":
                    GenerateNetherFortress(instance);
                    break;

                case "bastion_remnant":
                    GenerateBastionRemnant(instance);
                    break;

                case "end_city":
                    GenerateEndCity(instance);
                    break;

                case "ancient_city":
                    GenerateAncientCity(instance);
                    break;

                case "trial_chambers":
                    GenerateTrialChambers(instance);
                    break;

                default:
                    Console.WriteLine($"[StructureManager] 未知结构类型: {structureName}");
                    break;
            }

            instance.Generated = true;
            activeStructures[$"{x}_{y}_{z}_{structureName}"] = instance;
            StructuresGenerated++;
        }

        private void GenerateVillage(StructureInstance instance)
        {
            // 村庄生成
            int centerX = instance.X;
            int centerY = instance.Y;
            int centerZ = instance.Z;

            // 生成水井（中心）
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    world.SetBlock(centerX + dx, centerY, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }
            world.SetBlock(centerX, centerY + 1, centerZ, GameConstants.BLOCK_WATER_STILL);

            // 生成几栋房子
            int houseCount = Random.Shared.Next(3, 8);
            for (int i = 0; i < houseCount; i++)
            {
                int houseX = centerX + Random.Shared.Next(-15, 15);
                int houseZ = centerZ + Random.Shared.Next(-15, 15);
                GenerateSimpleHouse(houseX, centerY, houseZ);
            }

            // 生成道路
            for (int dx = -15; dx <= 15; dx++)
            {
                world.SetBlock(centerX + dx, centerY, centerZ, GameConstants.BLOCK_GRAVEL);
            }
            for (int dz = -15; dz <= 15; dz++)
            {
                world.SetBlock(centerX, centerY, centerZ + dz, GameConstants.BLOCK_GRAVEL);
            }
        }

        private void GenerateSimpleHouse(int x, int y, int z)
        {
            int width = 5;
            int depth = 5;
            int height = 4;

            // 地板
            for (int dx = 0; dx < width; dx++)
            {
                for (int dz = 0; dz < depth; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 墙壁
            for (int dy = 1; dy <= height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    world.SetBlock(x + dx, y + dy, z, GameConstants.BLOCK_WOOD);
                    world.SetBlock(x + dx, y + dy, z + depth - 1, GameConstants.BLOCK_WOOD);
                }
                for (int dz = 0; dz < depth; dz++)
                {
                    world.SetBlock(x, y + dy, z + dz, GameConstants.BLOCK_WOOD);
                    world.SetBlock(x + width - 1, y + dy, z + dz, GameConstants.BLOCK_WOOD);
                }
            }

            // 门
            world.SetBlock(x + width / 2, y + 1, z, GameConstants.BLOCK_AIR);
            world.SetBlock(x + width / 2, y + 2, z, GameConstants.BLOCK_AIR);

            // 窗户
            world.SetBlock(x + 1, y + 2, z, GameConstants.BLOCK_GLASS);
            world.SetBlock(x + width - 2, y + 2, z, GameConstants.BLOCK_GLASS);

            // 屋顶
            for (int dx = -1; dx <= width; dx++)
            {
                for (int dz = -1; dz <= depth; dz++)
                {
                    world.SetBlock(x + dx, y + height + 1, z + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }
        }

        private void GenerateDesertTemple(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 沙漠神殿主体
            for (int dx = -10; dx <= 10; dx++)
            {
                for (int dz = -10; dz <= 10; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_SANDSTONE);
                }
            }

            // 塔
            for (int dx = -10; dx <= -7; dx++)
            {
                for (int dz = -10; dz <= -7; dz++)
                {
                    for (int dy = 1; dy <= 8; dy++)
                    {
                        world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_SANDSTONE);
                    }
                }
            }
        }

        private void GenerateJungleTemple(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 丛林神殿
            for (int dx = -7; dx <= 7; dx++)
            {
                for (int dz = -7; dz <= 7; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_COBBLESTONE);
                    world.SetBlock(x + dx, y + 1, z + dz, GameConstants.BLOCK_MOSSY_COBBLESTONE);
                }
            }
        }

        private void GenerateWitchHut(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 女巫小屋
            for (int dx = 0; dx < 7; dx++)
            {
                for (int dz = 0; dz < 7; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 支柱
            for (int dx = 0; dx < 7; dx += 6)
            {
                for (int dz = 0; dz < 7; dz += 6)
                {
                    for (int dy = -3; dy < 0; dy++)
                    {
                        world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_WOOD);
                    }
                }
            }
        }

        private void GenerateIgloo(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 冰屋
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    for (int dy = 0; dy <= 3; dy++)
                    {
                        if (dx * dx + dz * dz + dy * dy <= 9)
                        {
                            world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_SNOW_BLOCK);
                        }
                    }
                }
            }
        }

        private void GeneratePillagerOutpost(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 掠夺者前哨站
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 塔
            for (int dy = 1; dy <= 15; dy++)
            {
                world.SetBlock(x, y + dy, z, GameConstants.BLOCK_WOOD);
            }
        }

        private void GenerateRuinedPortal(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 废弃传送门
            for (int dy = 0; dy < 5; dy++)
            {
                world.SetBlock(x - 2, y + dy, z, GameConstants.BLOCK_OBSIDIAN);
                world.SetBlock(x + 2, y + dy, z, GameConstants.BLOCK_OBSIDIAN);
            }
            for (int dx = -2; dx <= 2; dx++)
            {
                world.SetBlock(x + dx, y + 5, z, GameConstants.BLOCK_OBSIDIAN);
            }
        }

        private void GenerateShipwreck(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 沉船
            for (int dx = 0; dx < 20; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_WOOD);
                }
            }
        }

        private void GenerateOceanRuin(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 海底遗迹
            for (int dx = -5; dx <= 5; dx++)
            {
                for (int dz = -5; dz <= 5; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_STONE_BRICKS);
                }
            }
        }

        private void GenerateMineshaft(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 废弃矿井
            for (int dx = -15; dx <= 15; dx++)
            {
                for (int dy = 0; dy < 3; dy++)
                {
                    world.SetBlock(x + dx, y + dy, z, GameConstants.BLOCK_AIR);
                    world.SetBlock(x + dx, y + dy, z + 3, GameConstants.BLOCK_AIR);
                }
            }
        }

        private void GenerateStronghold(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 要塞
            for (int dx = -20; dx <= 20; dx++)
            {
                for (int dz = -20; dz <= 20; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_STONE_BRICKS);
                }
            }
        }

        private void GenerateDungeon(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 地牢
            for (int dx = -4; dx <= 4; dx++)
            {
                for (int dz = -4; dz <= 4; dz++)
                {
                    for (int dy = 0; dy <= 4; dy++)
                    {
                        if (dx == -4 || dx == 4 || dz == -4 || dz == 4 || dy == 0 || dy == 4)
                        {
                            world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_COBBLESTONE);
                        }
                        else
                        {
                            world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_AIR);
                        }
                    }
                }
            }

            // 刷怪笼
            world.SetBlock(x, y + 1, z, GameConstants.BLOCK_SPAWNER);
        }

        private void GenerateNetherFortress(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 下界要塞
            for (int dx = -20; dx <= 20; dx++)
            {
                for (int dz = -5; dz <= 5; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_NETHER_BRICKS);
                }
            }
        }

        private void GenerateBastionRemnant(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 猪灵堡垒
            for (int dx = -15; dx <= 15; dx++)
            {
                for (int dz = -15; dz <= 15; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_BLACKSTONE);
                }
            }
        }

        private void GenerateEndCity(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 末地城
            for (int dy = 0; dy < 30; dy++)
            {
                world.SetBlock(x, y + dy, z, GameConstants.BLOCK_END_STONE_BRICKS);
            }
        }

        private void GenerateAncientCity(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 远古城市
            for (int dx = -30; dx <= 30; dx++)
            {
                for (int dz = -30; dz <= 30; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_DEEPSLATE);
                }
            }
        }

        private void GenerateTrialChambers(StructureInstance instance)
        {
            int x = instance.X;
            int y = instance.Y;
            int z = instance.Z;

            // 试炼密室
            for (int dx = -20; dx <= 20; dx++)
            {
                for (int dz = -20; dz <= 20; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_TUFF);
                }
            }
        }

        public StructureInstance GetStructureAt(int x, int y, int z)
        {
            foreach (StructureInstance instance in activeStructures.Values)
            {
                if (x >= instance.X && x < instance.X + instance.Width &&
                    y >= instance.Y && y < instance.Y + instance.Height &&
                    z >= instance.Z && z < instance.Z + instance.Depth)
                {
                    return instance;
                }
            }
            return null;
        }

        public List<StructureInstance> GetStructuresInArea(int minX, int minZ, int maxX, int maxZ)
        {
            List<StructureInstance> result = new List<StructureInstance>();

            foreach (StructureInstance instance in activeStructures.Values)
            {
                if (instance.X >= minX && instance.X <= maxX &&
                    instance.Z >= minZ && instance.Z <= maxZ)
                {
                    result.Add(instance);
                }
            }

            return result;
        }

        public void ClearStructures()
        {
            activeStructures.Clear();
        }

        public void ResetStats()
        {
            StructuresGenerated = 0;
        }
    }

    public class StructureTemplate
    {
        public string Name;
        public int Width;
        public int Height;
        public int Depth;
        public float SpawnChance;
        public int MinY;
        public int MaxY;
        public List<Biome> Biomes;
        public Dictionary<Vector3i, ushort> Blocks;

        public StructureTemplate()
        {
            Biomes = new List<Biome>();
            Blocks = new Dictionary<Vector3i, ushort>();
        }
    }

    public class StructureInstance
    {
        public string Name;
        public int X;
        public int Y;
        public int Z;
        public int Width;
        public int Height;
        public int Depth;
        public bool Generated;
    }
}
