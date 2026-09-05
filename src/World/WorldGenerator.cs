using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class WorldGenerator
    {
        private readonly long seed;
        private readonly WorldManager world;
        private readonly NoiseGenerator noise;
        private readonly CaveGenerator caveGenerator;
        private readonly TreeGenerator treeGenerator;
        private readonly BiomeGenerator biomeGenerator;
        private readonly OreGenerator oreGenerator;

        // 地形参数
        private const float BASE_HEIGHT = 64f;
        private const float TERRAIN_VARIATION = 24f;
        private const float MOUNTAIN_HEIGHT = 60f;
        private const float OCEAN_LEVEL = 58f;
        private const float BEACH_LEVEL = 62f;

        public WorldGenerator(long seed, WorldManager world)
        {
            this.seed = seed;
            this.world = world;
            noise = new NoiseGenerator(seed);
            caveGenerator = new CaveGenerator(seed);
            treeGenerator = new TreeGenerator(seed);
            biomeGenerator = new BiomeGenerator(seed);
            oreGenerator = new OreGenerator(seed);
        }

        public void Initialize()
        {
            Console.WriteLine("[WorldGenerator] 世界生成器初始化完成");
        }

        public void GenerateChunk(Chunk chunk)
        {
            GenerateTerrain(chunk);
            GenerateCaves(chunk);
            GenerateOres(chunk);
            GenerateTrees(chunk);
            GenerateStructures(chunk);
            GenerateLiquids(chunk);

            chunk.IsGenerated = true;
            chunk.IsPopulated = true;
        }

        // ========================================
        // 地形生成
        // ========================================
        private void GenerateTerrain(Chunk chunk)
        {
            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                {
                    int worldX = chunk.X * GameConstants.CHUNK_SIZE + x;
                    int worldZ = chunk.Z * GameConstants.CHUNK_SIZE + z;

                    BiomeType biome = biomeGenerator.GetBiome(worldX, worldZ);
                    float temperature = biomeGenerator.GetTemperature(worldX, worldZ);
                    float humidity = biomeGenerator.GetHumidity(worldX, worldZ);
                    float continentalness = noise.GetContinentalness(worldX, worldZ);
                    float erosion = noise.GetErosion(worldX, worldZ);
                    float weirdness = noise.GetWeirdness(worldX, worldZ);

                    int height = CalculateHeight(worldX, worldZ, biome, continentalness, erosion, weirdness);

                    for (int y = 0; y < GameConstants.CHUNK_HEIGHT; y++)
                    {
                        ushort block = GetBlockAtHeight(y, height, biome, temperature, humidity);

                        if (block != GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(x, y, z, block);
                        }
                    }

                    // 基岩层
                    for (int y = 0; y < 5; y++)
                    {
                        if (noise.Perlin2D(worldX + y * 100, worldZ + y * 100) > -0.5f)
                        {
                            chunk.SetBlock(x, y, z, GameConstants.BLOCK_BEDROCK);
                        }
                    }
                }
            }
        }

        private int CalculateHeight(int x, int z, BiomeType biome, float continentalness, float erosion, float weirdness)
        {
            float baseHeight = BASE_HEIGHT;

            // 大陆性决定基本地形
            if (continentalness < -0.3f)
            {
                // 海洋
                baseHeight = OCEAN_LEVEL - 10 + continentalness * 20;
            }
            else if (continentalness < -0.1f)
            {
                // 近海
                baseHeight = OCEAN_LEVEL - 5 + (continentalness + 0.3f) * 25;
            }
            else if (continentalness < 0.1f)
            {
                // 海岸/低地
                baseHeight = BEACH_LEVEL + continentalness * 20;
            }
            else
            {
                // 陆地
                baseHeight = BASE_HEIGHT + continentalness * 15;
            }

            // 侵蚀度影响地形起伏
            float terrainNoise = noise.DomainWarp2D(x, z);
            float variation = TERRAIN_VARIATION * (1.0f - Math.Abs(erosion) * 0.5f);
            baseHeight += terrainNoise * variation;

            // 怪异度影响特殊地形
            if (weirdness > 0.5f)
            {
                // 山脉
                float mountainNoise = noise.RidgedMulti2D(x * 0.5f, z * 0.5f);
                float mountainMask = Math.Max(0, weirdness - 0.3f) * 2.0f;
                baseHeight += mountainNoise * MOUNTAIN_HEIGHT * mountainMask;
            }
            else if (weirdness < -0.5f)
            {
                // 盆地/峡谷
                baseHeight -= Math.Abs(weirdness + 0.5f) * 30;
            }

            // 生物群系修正
            switch (biome)
            {
                case BiomeType.Mountains:
                case BiomeType.WoodedMountains:
                case BiomeType.GravellyMountains:
                    baseHeight += 20;
                    break;
                case BiomeType.Plains:
                case BiomeType.SunflowerPlains:
                    baseHeight -= 5;
                    break;
                case BiomeType.Desert:
                    baseHeight -= 3;
                    break;
                case BiomeType.Swamp:
                case BiomeType.SwampHills:
                    baseHeight = BEACH_LEVEL + 2;
                    break;
                case BiomeType.Ocean:
                case BiomeType.DeepOcean:
                    baseHeight = OCEAN_LEVEL - 15;
                    break;
            }

            return (int)Math.Clamp(baseHeight, 5, GameConstants.CHUNK_HEIGHT - 32);
        }

        private ushort GetBlockAtHeight(int y, int height, BiomeType biome, float temperature, float humidity)
        {
            if (y > height)
            {
                // 海平面以下填充水
                if (y <= GameConstants.SEA_LEVEL)
                {
                    return GameConstants.BLOCK_WATER_STILL;
                }
                return GameConstants.BLOCK_AIR;
            }

            int depth = height - y;

            if (depth == 0)
            {
                // 表层方块
                return GetSurfaceBlock(biome, temperature, humidity, y);
            }
            else if (depth <= 3)
            {
                // 次表层
                return GetSubSurfaceBlock(biome);
            }
            else if (depth <= 5)
            {
                // 过渡层
                return GameConstants.BLOCK_DIRT;
            }
            else
            {
                // 深层石头
                return GetDeepStoneBlock(y);
            }
        }

        private ushort GetSurfaceBlock(BiomeType biome, float temperature, float humidity, int y)
        {
            if (y <= GameConstants.SEA_LEVEL + 1)
            {
                // 海滩
                if (temperature > 0.3f)
                {
                    return GameConstants.BLOCK_SAND;
                }
                else
                {
                    return GameConstants.BLOCK_SNOW_BLOCK;
                }
            }

            switch (biome)
            {
                case BiomeType.Desert:
                case BiomeType.DesertHills:
                case BiomeType.DesertLakes:
                    return GameConstants.BLOCK_SAND;
                case BiomeType.Badlands:
                case BiomeType.BadlandsPlateau:
                case BiomeType.ErodedBadlands:
                    return GameConstants.BLOCK_RED_SAND;
                case BiomeType.SnowyTundra:
                case BiomeType.SnowyMountains:
                case BiomeType.IceSpikes:
                case BiomeType.SnowyTaiga:
                    return GameConstants.BLOCK_SNOW;
                case BiomeType.Swamp:
                case BiomeType.SwampHills:
                    return GameConstants.BLOCK_GRASS;
                case BiomeType.MushroomFields:
                case BiomeType.MushroomFieldShore:
                    return GameConstants.BLOCK_MYCELIUM;
                default:
                    if (temperature < 0.2f)
                    {
                        return GameConstants.BLOCK_SNOW;
                    }
                    return GameConstants.BLOCK_GRASS;
            }
        }

        private ushort GetSubSurfaceBlock(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Desert:
                case BiomeType.DesertHills:
                case BiomeType.DesertLakes:
                    return GameConstants.BLOCK_SAND;
                case BiomeType.Badlands:
                case BiomeType.BadlandsPlateau:
                case BiomeType.ErodedBadlands:
                    return GameConstants.BLOCK_RED_SAND;
                case BiomeType.MushroomFields:
                case BiomeType.MushroomFieldShore:
                    return GameConstants.BLOCK_MYCELIUM;
                default:
                    return GameConstants.BLOCK_DIRT;
            }
        }

        private ushort GetDeepStoneBlock(int y)
        {
            if (y < 0) return GameConstants.BLOCK_BEDROCK;
            if (y < 8)
            {
                // 深层区域可能有深板岩
                float noiseVal = noise.Perlin2D(y * 10, y * 10);
                if (noiseVal > 0.3f)
                {
                    return GameConstants.BLOCK_DEEPSLATE;
                }
            }
            return GameConstants.BLOCK_STONE;
        }

        // ========================================
        // 洞穴生成
        // ========================================
        private void GenerateCaves(Chunk chunk)
        {
            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                {
                    int worldX = chunk.X * GameConstants.CHUNK_SIZE + x;
                    int worldZ = chunk.Z * GameConstants.CHUNK_SIZE + z;

                    for (int y = 5; y < GameConstants.SEA_LEVEL - 5; y++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);
                        if (block == GameConstants.BLOCK_AIR || block == GameConstants.BLOCK_WATER_STILL)
                            continue;

                        if (caveGenerator.IsCave(worldX, y, worldZ))
                        {
                            chunk.SetBlock(x, y, z, GameConstants.BLOCK_AIR);

                            // 熔岩湖
                            if (y < 10 && caveGenerator.IsLavaCave(worldX, y, worldZ))
                            {
                                chunk.SetBlock(x, y, z, GameConstants.BLOCK_LAVA_STILL);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 矿石生成
        // ========================================
        private void GenerateOres(Chunk chunk)
        {
            oreGenerator.GenerateOres(chunk);
        }

        // ========================================
        // 树木生成
        // ========================================
        private void GenerateTrees(Chunk chunk)
        {
            BiomeType biome = biomeGenerator.GetBiome(
                chunk.X * GameConstants.CHUNK_SIZE + 8,
                chunk.Z * GameConstants.CHUNK_SIZE + 8
            );

            int treeCount = noise.GetTreeCount(chunk.X, chunk.Z, biome);

            for (int i = 0; i < treeCount; i++)
            {
                Vector2 treePos = noise.GetTreePosition(chunk.X, chunk.Z, i);
                int treeX = (int)treePos.X - chunk.X * GameConstants.CHUNK_SIZE;
                int treeZ = (int)treePos.Y - chunk.Z * GameConstants.CHUNK_SIZE;

                if (treeX < 2 || treeX >= GameConstants.CHUNK_SIZE - 2 ||
                    treeZ < 2 || treeZ >= GameConstants.CHUNK_SIZE - 2)
                    continue;

                // 找到地面高度
                int groundY = -1;
                for (int y = GameConstants.CHUNK_HEIGHT - 1; y >= 0; y--)
                {
                    ushort block = chunk.GetBlock(treeX, y, treeZ);
                    if (block == GameConstants.BLOCK_GRASS ||
                        block == GameConstants.BLOCK_DIRT ||
                        block == GameConstants.BLOCK_PODZOL)
                    {
                        groundY = y + 1;
                        break;
                    }
                }

                if (groundY <= 0 || groundY >= GameConstants.CHUNK_HEIGHT - 20)
                    continue;

                // 生成树木
                TreeType treeType = GetTreeTypeForBiome(biome);
                treeGenerator.GenerateTree(chunk, treeX, groundY, treeZ, treeType);
            }
        }

        private TreeType GetTreeTypeForBiome(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Forest:
                case BiomeType.FlowerForest:
                    return TreeType.Oak;
                case BiomeType.BirchForest:
                case BiomeType.TallBirchForest:
                    return TreeType.Birch;
                case BiomeType.DarkForest:
                    return TreeType.DarkOak;
                case BiomeType.Jungle:
                case BiomeType.BambooJungle:
                    return TreeType.Jungle;
                case BiomeType.Taiga:
                case BiomeType.GiantTreeTaiga:
                case BiomeType.GiantSpruceTaiga:
                case BiomeType.SnowyTaiga:
                    return TreeType.Spruce;
                case BiomeType.Savanna:
                case BiomeType.SavannaPlateau:
                    return TreeType.Acacia;
                case BiomeType.Swamp:
                    return TreeType.SwampOak;
                default:
                    return TreeType.Oak;
            }
        }

        // ========================================
        // 结构生成
        // ========================================
        private void GenerateStructures(Chunk chunk)
        {
            // 村庄
            if (ShouldGenerateVillage(chunk))
            {
                GenerateVillage(chunk);
            }

            // 神庙
            if (ShouldGenerateTemple(chunk))
            {
                GenerateTemple(chunk);
            }

            // 矿井
            if (ShouldGenerateMineShaft(chunk))
            {
                GenerateMineShaft(chunk);
            }

            // 地牢
            if (ShouldGenerateDungeon(chunk))
            {
                GenerateDungeon(chunk);
            }
        }

        private bool ShouldGenerateVillage(Chunk chunk)
        {
            BiomeType biome = biomeGenerator.GetBiome(
                chunk.X * GameConstants.CHUNK_SIZE + 8,
                chunk.Z * GameConstants.CHUNK_SIZE + 8
            );

            bool validBiome = biome == BiomeType.Plains ||
                             biome == BiomeType.Savanna ||
                             biome == BiomeType.Desert ||
                             biome == BiomeType.Taiga ||
                             biome == BiomeType.SnowyTundra;

            if (!validBiome) return false;

            float noiseVal = noise.Perlin2D(chunk.X * 0.1f + 5000, chunk.Z * 0.1f + 5000);
            return noiseVal > 0.85f;
        }

        private void GenerateVillage(Chunk chunk)
        {
            // 简单村庄生成
            int centerX = GameConstants.CHUNK_SIZE / 2;
            int centerZ = GameConstants.CHUNK_SIZE / 2;
            int groundY = FindGroundHeight(chunk, centerX, centerZ);

            if (groundY <= 0) return;

            // 生成几个房子
            Random r = new Random((int)(seed + chunk.X * 1000 + chunk.Z));
            int houseCount = r.Next(3, 8);

            for (int i = 0; i < houseCount; i++)
            {
                int hx = centerX + r.Next(-6, 6);
                int hz = centerZ + r.Next(-6, 6);
                int hy = FindGroundHeight(chunk, hx, hz);

                if (hy > 0)
                {
                    GenerateSimpleHouse(chunk, hx, hy, hz, r);
                }
            }

            // 水井
            GenerateWell(chunk, centerX, groundY, centerZ);
        }

        private void GenerateSimpleHouse(Chunk chunk, int x, int y, int z, Random r)
        {
            int width = r.Next(4, 7);
            int depth = r.Next(4, 7);
            int height = r.Next(3, 5);

            // 地板
            for (int dx = 0; dx < width; dx++)
            {
                for (int dz = 0; dz < depth; dz++)
                {
                    chunk.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 墙壁
            for (int dy = 1; dy <= height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    chunk.SetBlock(x + dx, y + dy, z, GameConstants.BLOCK_PLANKS);
                    chunk.SetBlock(x + dx, y + dy, z + depth - 1, GameConstants.BLOCK_PLANKS);
                }
                for (int dz = 0; dz < depth; dz++)
                {
                    chunk.SetBlock(x, y + dy, z + dz, GameConstants.BLOCK_PLANKS);
                    chunk.SetBlock(x + width - 1, y + dy, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 屋顶
            for (int dx = -1; dx <= width; dx++)
            {
                for (int dz = -1; dz <= depth; dz++)
                {
                    chunk.SetBlock(x + dx, y + height + 1, z + dz, GameConstants.BLOCK_PLANKS);
                }
            }

            // 门
            chunk.SetBlock(x + width / 2, y + 1, z, GameConstants.BLOCK_AIR);
            chunk.SetBlock(x + width / 2, y + 2, z, GameConstants.BLOCK_AIR);

            // 窗户
            chunk.SetBlock(x + 1, y + 2, z + depth / 2, GameConstants.BLOCK_GLASS);
            chunk.SetBlock(x + width - 2, y + 2, z + depth / 2, GameConstants.BLOCK_GLASS);
        }

        private void GenerateWell(Chunk chunk, int x, int y, int z)
        {
            // 简单水井
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    chunk.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }

            for (int dy = 1; dy <= 3; dy++)
            {
                chunk.SetBlock(x - 2, y + dy, z - 2, GameConstants.BLOCK_COBBLESTONE);
                chunk.SetBlock(x + 2, y + dy, z - 2, GameConstants.BLOCK_COBBLESTONE);
                chunk.SetBlock(x - 2, y + dy, z + 2, GameConstants.BLOCK_COBBLESTONE);
                chunk.SetBlock(x + 2, y + dy, z + 2, GameConstants.BLOCK_COBBLESTONE);
            }

            chunk.SetBlock(x, y + 1, z, GameConstants.BLOCK_WATER_STILL);
        }

        private bool ShouldGenerateTemple(Chunk chunk)
        {
            BiomeType biome = biomeGenerator.GetBiome(
                chunk.X * GameConstants.CHUNK_SIZE + 8,
                chunk.Z * GameConstants.CHUNK_SIZE + 8
            );

            bool validBiome = biome == BiomeType.Desert ||
                             biome == BiomeType.Jungle ||
                             biome == BiomeType.Swamp;

            if (!validBiome) return false;

            float noiseVal = noise.Perlin2D(chunk.X * 0.15f + 8000, chunk.Z * 0.15f + 8000);
            return noiseVal > 0.9f;
        }

        private void GenerateTemple(Chunk chunk)
        {
            // 简单神庙
            int centerX = GameConstants.CHUNK_SIZE / 2;
            int centerZ = GameConstants.CHUNK_SIZE / 2;
            int groundY = FindGroundHeight(chunk, centerX, centerZ);

            if (groundY <= 0) return;

            // 基座
            for (int dx = -5; dx <= 5; dx++)
            {
                for (int dz = -5; dz <= 5; dz++)
                {
                    chunk.SetBlock(centerX + dx, groundY, centerZ + dz, GameConstants.BLOCK_SANDSTONE);
                }
            }

            // 柱子
            for (int dy = 1; dy <= 4; dy++)
            {
                chunk.SetBlock(centerX - 4, groundY + dy, centerZ - 4, GameConstants.BLOCK_SANDSTONE);
                chunk.SetBlock(centerX + 4, groundY + dy, centerZ - 4, GameConstants.BLOCK_SANDSTONE);
                chunk.SetBlock(centerX - 4, groundY + dy, centerZ + 4, GameConstants.BLOCK_SANDSTONE);
                chunk.SetBlock(centerX + 4, groundY + dy, centerZ + 4, GameConstants.BLOCK_SANDSTONE);
            }

            // 屋顶
            for (int dx = -5; dx <= 5; dx++)
            {
                for (int dz = -5; dz <= 5; dz++)
                {
                    chunk.SetBlock(centerX + dx, groundY + 5, centerZ + dz, GameConstants.BLOCK_SANDSTONE);
                }
            }
        }

        private bool ShouldGenerateMineShaft(Chunk chunk)
        {
            float noiseVal = noise.Perlin2D(chunk.X * 0.2f + 12000, chunk.Z * 0.2f + 12000);
            return noiseVal > 0.88f;
        }

        private void GenerateMineShaft(Chunk chunk)
        {
            // 简单矿井
            Random r = new Random((int)(seed + chunk.X * 2000 + chunk.Z));
            int tunnelY = r.Next(15, 40);
            int tunnelX = r.Next(2, GameConstants.CHUNK_SIZE - 2);
            int tunnelZ = r.Next(2, GameConstants.CHUNK_SIZE - 2);

            // 水平隧道
            for (int dx = -4; dx <= 4; dx++)
            {
                for (int dy = 0; dy <= 2; dy++)
                {
                    chunk.SetBlock(tunnelX + dx, tunnelY + dy, tunnelZ, GameConstants.BLOCK_AIR);
                    chunk.SetBlock(tunnelX + dx, tunnelY + dy, tunnelZ + 1, GameConstants.BLOCK_AIR);
                }
            }

            // 支撑柱
            for (int dx = -3; dx <= 3; dx += 2)
            {
                chunk.SetBlock(tunnelX + dx, tunnelY, tunnelZ, GameConstants.BLOCK_PLANKS);
                chunk.SetBlock(tunnelX + dx, tunnelY + 1, tunnelZ, GameConstants.BLOCK_PLANKS);
                chunk.SetBlock(tunnelX + dx, tunnelY + 2, tunnelZ, GameConstants.BLOCK_PLANKS);
            }

            // 火把
            chunk.SetBlock(tunnelX, tunnelY + 2, tunnelZ + 1, GameConstants.BLOCK_TORCH);
        }

        private bool ShouldGenerateDungeon(Chunk chunk)
        {
            float noiseVal = noise.Perlin3D(chunk.X * 0.3f + 15000, chunk.Z * 0.3f + 15000, chunk.X + chunk.Z);
            return noiseVal > 0.92f;
        }

        private void GenerateDungeon(Chunk chunk)
        {
            Random r = new Random((int)(seed + chunk.X * 3000 + chunk.Z));
            int dungeonY = r.Next(10, 35);
            int centerX = r.Next(3, GameConstants.CHUNK_SIZE - 3);
            int centerZ = r.Next(3, GameConstants.CHUNK_SIZE - 3);

            int width = r.Next(4, 7);
            int depth = r.Next(4, 7);

            // 挖空房间
            for (int dx = -width / 2; dx <= width / 2; dx++)
            {
                for (int dz = -depth / 2; dz <= depth / 2; dz++)
                {
                    for (int dy = 0; dy <= 3; dy++)
                    {
                        chunk.SetBlock(centerX + dx, dungeonY + dy, centerZ + dz, GameConstants.BLOCK_AIR);
                    }
                }
            }

            // 圆石墙壁
            for (int dx = -width / 2 - 1; dx <= width / 2 + 1; dx++)
            {
                for (int dz = -depth / 2 - 1; dz <= depth / 2 + 1; dz++)
                {
                    chunk.SetBlock(centerX + dx, dungeonY - 1, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                    chunk.SetBlock(centerX + dx, dungeonY + 4, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }

            // 刷怪笼
            chunk.SetBlock(centerX, dungeonY, centerZ, GameConstants.BLOCK_MOB_SPAWNER);

            // 箱子
            chunk.SetBlock(centerX - width / 2 + 1, dungeonY, centerZ - depth / 2 + 1, GameConstants.BLOCK_CHEST);
            chunk.SetBlock(centerX + width / 2 - 1, dungeonY, centerZ + depth / 2 - 1, GameConstants.BLOCK_CHEST);
        }

        // ========================================
        // 液体生成
        // ========================================
        private void GenerateLiquids(Chunk chunk)
        {
            // 水已经在地形生成时处理了
            // 这里处理额外的水源和熔岩源

            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                {
                    int worldX = chunk.X * GameConstants.CHUNK_SIZE + x;
                    int worldZ = chunk.Z * GameConstants.CHUNK_SIZE + z;

                    // 地下湖泊
                    if (noise.Perlin2D(worldX * 0.05f + 20000, worldZ * 0.05f + 20000) > 0.7f)
                    {
                        int lakeY = noise.GetOreHeight(worldX, worldZ, 20, 50);
                        if (chunk.GetBlock(x, lakeY, z) == GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(x, lakeY, z, GameConstants.BLOCK_WATER_STILL);
                        }
                    }
                }
            }
        }

        // ========================================
        // 工具方法
        // ========================================
        private int FindGroundHeight(Chunk chunk, int x, int z)
        {
            for (int y = GameConstants.CHUNK_HEIGHT - 1; y >= 0; y--)
            {
                ushort block = chunk.GetBlock(x, y, z);
                if (block == GameConstants.BLOCK_GRASS ||
                    block == GameConstants.BLOCK_DIRT ||
                    block == GameConstants.BLOCK_STONE ||
                    block == GameConstants.BLOCK_SAND)
                {
                    return y + 1;
                }
            }
            return -1;
        }

        public BiomeType GetBiome(int x, int z)
        {
            return biomeGenerator.GetBiome(x, z);
        }

        public float GetTemperature(int x, int z)
        {
            return biomeGenerator.GetTemperature(x, z);
        }

        public float GetHumidity(int x, int z)
        {
            return biomeGenerator.GetHumidity(x, z);
        }
    }

    // ========================================
    // 树木类型
    // ========================================
    public enum TreeType
    {
        Oak,
        Birch,
        Spruce,
        Jungle,
        Acacia,
        DarkOak,
        SwampOak,
        HugeMushroom,
        Cherry
    }
}
