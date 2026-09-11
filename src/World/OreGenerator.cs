using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class OreGenerator
    {
        private readonly long seed;
        private readonly NoiseGenerator noise;
        private readonly Random random;

        // 矿石配置
        private readonly List<OreConfig> oreConfigs;

        public OreGenerator(long seed)
        {
            this.seed = seed;
            noise = new NoiseGenerator(seed);
            random = new Random((int)seed);
            oreConfigs = InitializeOreConfigs();
        }

        private List<OreConfig> InitializeOreConfigs()
        {
            return new List<OreConfig>
            {
                // 煤矿石 - 最常见，全高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_COAL_ORE,
                    MinY = 5,
                    MaxY = 128,
                    VeinSize = 17,
                    VeinsPerChunk = 20,
                    SpawnChance = 1.0f,
                    NoiseThreshold = 0.3f
                },
                // 铁矿石 - 中低高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_IRON_ORE,
                    MinY = 5,
                    MaxY = 64,
                    VeinSize = 9,
                    VeinsPerChunk = 12,
                    SpawnChance = 0.9f,
                    NoiseThreshold = 0.35f
                },
                // 金矿石 - 低高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_GOLD_ORE,
                    MinY = 5,
                    MaxY = 32,
                    VeinSize = 9,
                    VeinsPerChunk = 4,
                    SpawnChance = 0.6f,
                    NoiseThreshold = 0.4f
                },
                // 钻石矿石 - 极低高度，稀有
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DIAMOND_ORE,
                    MinY = 5,
                    MaxY = 16,
                    VeinSize = 8,
                    VeinsPerChunk = 2,
                    SpawnChance = 0.4f,
                    NoiseThreshold = 0.45f
                },
                // 红石矿石 - 低高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_REDSTONE_ORE,
                    MinY = 5,
                    MaxY = 16,
                    VeinSize = 8,
                    VeinsPerChunk = 8,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.35f
                },
                // 青金石矿石 - 中低高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_LAPIS_ORE,
                    MinY = 5,
                    MaxY = 32,
                    VeinSize = 7,
                    VeinsPerChunk = 4,
                    SpawnChance = 0.5f,
                    NoiseThreshold = 0.4f
                },
                // 绿宝石矿石 - 山脉生物群系，极稀有
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_EMERALD_ORE,
                    MinY = 5,
                    MaxY = 32,
                    VeinSize = 1,
                    VeinsPerChunk = 1,
                    SpawnChance = 0.1f,
                    NoiseThreshold = 0.5f
                },
                // 铜矿石 - 中高度
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_COPPER_ORE,
                    MinY = 5,
                    MaxY = 96,
                    VeinSize = 10,
                    VeinsPerChunk = 10,
                    SpawnChance = 0.8f,
                    NoiseThreshold = 0.35f
                },
                // 深层煤矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_COAL_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 17,
                    VeinsPerChunk = 15,
                    SpawnChance = 0.9f,
                    NoiseThreshold = 0.3f
                },
                // 深层铁矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_IRON_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 9,
                    VeinsPerChunk = 8,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.35f
                },
                // 深层金矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_GOLD_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 9,
                    VeinsPerChunk = 3,
                    SpawnChance = 0.5f,
                    NoiseThreshold = 0.4f
                },
                // 深层钻石矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_DIAMOND_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 8,
                    VeinsPerChunk = 2,
                    SpawnChance = 0.35f,
                    NoiseThreshold = 0.45f
                },
                // 深层红石矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_REDSTONE_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 8,
                    VeinsPerChunk = 6,
                    SpawnChance = 0.6f,
                    NoiseThreshold = 0.35f
                },
                // 深层青金石矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_LAPIS_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 7,
                    VeinsPerChunk = 3,
                    SpawnChance = 0.4f,
                    NoiseThreshold = 0.4f
                },
                // 深层铜矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DEEPSLATE_COPPER_ORE,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 10,
                    VeinsPerChunk = 6,
                    SpawnChance = 0.6f,
                    NoiseThreshold = 0.35f
                },
                // 下界石英矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_NETHER_QUARTZ_ORE,
                    MinY = 10,
                    MaxY = 120,
                    VeinSize = 14,
                    VeinsPerChunk = 16,
                    SpawnChance = 0.9f,
                    NoiseThreshold = 0.3f,
                    IsNetherOre = true
                },
                // 下界金矿石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_NETHER_GOLD_ORE,
                    MinY = 10,
                    MaxY = 120,
                    VeinSize = 10,
                    VeinsPerChunk = 10,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.35f,
                    IsNetherOre = true
                },
                // 远古残骸
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_ANCIENT_DEBRIS,
                    MinY = 8,
                    MaxY = 22,
                    VeinSize = 3,
                    VeinsPerChunk = 2,
                    SpawnChance = 0.15f,
                    NoiseThreshold = 0.55f,
                    IsNetherOre = true
                },
                // 安山岩
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_ANDESITE,
                    MinY = 5,
                    MaxY = 128,
                    VeinSize = 33,
                    VeinsPerChunk = 10,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.3f,
                    IsStoneVariant = true
                },
                // 花岗岩
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_GRANITE,
                    MinY = 5,
                    MaxY = 128,
                    VeinSize = 33,
                    VeinsPerChunk = 10,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.3f,
                    IsStoneVariant = true
                },
                // 闪长岩
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DIORITE,
                    MinY = 5,
                    MaxY = 128,
                    VeinSize = 33,
                    VeinsPerChunk = 10,
                    SpawnChance = 0.7f,
                    NoiseThreshold = 0.3f,
                    IsStoneVariant = true
                },
                // 凝灰岩
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_TUFF,
                    MinY = 0,
                    MaxY = 16,
                    VeinSize = 33,
                    VeinsPerChunk = 8,
                    SpawnChance = 0.6f,
                    NoiseThreshold = 0.3f,
                    IsStoneVariant = true
                },
                // 滴水石
                new OreConfig
                {
                    BlockId = GameConstants.BLOCK_DRIPSTONE_BLOCK,
                    MinY = 5,
                    MaxY = 64,
                    VeinSize = 10,
                    VeinsPerChunk = 4,
                    SpawnChance = 0.3f,
                    NoiseThreshold = 0.4f
                }
            };
        }

        public void GenerateOres(Chunk chunk)
        {
            foreach (OreConfig config in oreConfigs)
            {
                if (random.NextDouble() > config.SpawnChance) continue;

                for (int vein = 0; vein < config.VeinsPerChunk; vein++)
                {
                    int x = random.Next(0, GameConstants.CHUNK_SIZE);
                    int y = random.Next(config.MinY, config.MaxY + 1);
                    int z = random.Next(0, GameConstants.CHUNK_SIZE);

                    GenerateOreVein(chunk, x, y, z, config);
                }
            }
        }

        private void GenerateOreVein(Chunk chunk, int startX, int startY, int startZ, OreConfig config)
        {
            // 使用噪声生成矿脉形状
            for (int dx = -config.VeinSize / 2; dx <= config.VeinSize / 2; dx++)
            {
                for (int dy = -config.VeinSize / 2; dy <= config.VeinSize / 2; dy++)
                {
                    for (int dz = -config.VeinSize / 2; dz <= config.VeinSize / 2; dz++)
                    {
                        int x = startX + dx;
                        int y = startY + dy;
                        int z = startZ + dz;

                        if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                            y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                            z < 0 || z >= GameConstants.CHUNK_SIZE)
                            continue;

                        // 球形矿脉
                        float dist = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        if (dist > config.VeinSize / 2.0f) continue;

                        // 噪声扰动
                        float noiseVal = noise.Perlin3D(
                            (x + chunk.X * GameConstants.CHUNK_SIZE) * 0.3f,
                            y * 0.3f,
                            (z + chunk.Z * GameConstants.CHUNK_SIZE) * 0.3f
                        );

                        if (noiseVal < config.NoiseThreshold) continue;

                        ushort currentBlock = chunk.GetBlock(x, y, z);

                        // 只替换石头或深板岩
                        if (config.IsNetherOre)
                        {
                            if (currentBlock == GameConstants.BLOCK_NETHERRACK)
                            {
                                chunk.SetBlock(x, y, z, config.BlockId);
                            }
                        }
                        else if (config.IsStoneVariant)
                        {
                            if (currentBlock == GameConstants.BLOCK_STONE)
                            {
                                chunk.SetBlock(x, y, z, config.BlockId);
                            }
                        }
                        else
                        {
                            if (currentBlock == GameConstants.BLOCK_STONE ||
                                currentBlock == GameConstants.BLOCK_DEEPSLATE)
                            {
                                chunk.SetBlock(x, y, z, config.BlockId);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 矿石分布查询（用于游戏内信息）
        // ========================================
        public OreDistribution GetOreDistribution(ushort blockId)
        {
            foreach (OreConfig config in oreConfigs)
            {
                if (config.BlockId == blockId)
                {
                    return new OreDistribution
                    {
                        MinY = config.MinY,
                        MaxY = config.MaxY,
                        VeinSize = config.VeinSize,
                        VeinsPerChunk = config.VeinsPerChunk,
                        Rarity = 1.0f / config.SpawnChance
                    };
                }
            }
            return default;
        }

        public List<ushort> GetOresAtHeight(int y)
        {
            List<ushort> ores = new List<ushort>();
            foreach (OreConfig config in oreConfigs)
            {
                if (y >= config.MinY && y <= config.MaxY)
                {
                    ores.Add(config.BlockId);
                }
            }
            return ores;
        }

        public float GetOreRarity(ushort blockId)
        {
            foreach (OreConfig config in oreConfigs)
            {
                if (config.BlockId == blockId)
                {
                    return config.SpawnChance * config.VeinsPerChunk / 20.0f;
                }
            }
            return 0f;
        }
    }

    // ========================================
    // 矿石配置
    // ========================================
    public struct OreConfig
    {
        public ushort BlockId;
        public int MinY;
        public int MaxY;
        public int VeinSize;
        public int VeinsPerChunk;
        public float SpawnChance;
        public float NoiseThreshold;
        public bool IsNetherOre;
        public bool IsStoneVariant;
    }

    public struct OreDistribution
    {
        public int MinY;
        public int MaxY;
        public int VeinSize;
        public int VeinsPerChunk;
        public float Rarity;
    }
}
