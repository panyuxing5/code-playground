using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class StructureGenerator
    {
        private readonly WorldManager world;
        private readonly Random random;

        // 结构配置
        public int VillageSpacing { get; set; } = 32;
        public int TempleSpacing { get; set; } = 48;
        public int DungeonSpacing { get; set; } = 16;
        public int MineshaftSpacing { get; set; } = 24;
        public int StrongholdSpacing { get; set; } = 64;
        public int PyramidSpacing { get; set; } = 40;
        public int IglooSpacing { get; set; } = 36;
        public int MansionSpacing { get; set; } = 80;
        public int OutpostSpacing { get; set; } = 56;
        public int RuinedPortalSpacing { get; set; } = 28;
        public int ShipwreckSpacing { get; set; } = 24;
        public int OceanRuinSpacing { get; set; } = 20;
        public int AncientCitySpacing { get; set; } = 96;

        public StructureGenerator(WorldManager world, int seed = 0)
        {
            this.world = world;
            random = new Random(seed);
        }

        public void Initialize()
        {
            Console.WriteLine("[StructureGenerator] 结构生成器初始化完成");
        }

        public void GenerateStructuresForChunk(int chunkX, int chunkZ)
        {
            // 基于区块坐标和种子决定是否生成结构
            int worldX = chunkX * GameConstants.CHUNK_SIZE;
            int worldZ = chunkZ * GameConstants.CHUNK_SIZE;

            // 村庄
            if (ShouldGenerateStructure(chunkX, chunkZ, VillageSpacing))
            {
                GenerateVillage(worldX + 8, worldZ + 8);
            }

            // 地牢
            if (ShouldGenerateStructure(chunkX, chunkZ, DungeonSpacing))
            {
                GenerateDungeon(worldX + 8, worldZ + 8);
            }

            // 废弃矿井
            if (ShouldGenerateStructure(chunkX, chunkZ, MineshaftSpacing))
            {
                GenerateMineshaft(worldX + 8, worldZ + 8);
            }

            // 沙漠神殿
            if (ShouldGenerateStructure(chunkX, chunkZ, TempleSpacing))
            {
                GenerateDesertTemple(worldX + 8, worldZ + 8);
            }

            // 丛林神庙
            if (ShouldGenerateStructure(chunkX, chunkZ, TempleSpacing + 10))
            {
                GenerateJungleTemple(worldX + 8, worldZ + 8);
            }

            // 海底遗迹
            if (ShouldGenerateStructure(chunkX, chunkZ, OceanRuinSpacing))
            {
                GenerateOceanRuin(worldX + 8, worldZ + 8);
            }

            // 沉船
            if (ShouldGenerateStructure(chunkX, chunkZ, ShipwreckSpacing))
            {
                GenerateShipwreck(worldX + 8, worldZ + 8);
            }

            // 废弃传送门
            if (ShouldGenerateStructure(chunkX, chunkZ, RuinedPortalSpacing))
            {
                GenerateRuinedPortal(worldX + 8, worldZ + 8);
            }
        }

        private bool ShouldGenerateStructure(int chunkX, int chunkZ, int spacing)
        {
            // 使用区块坐标和间距计算是否生成
            int regionX = Math.DivRem(chunkX, spacing, out int offsetX);
            int regionZ = Math.DivRem(chunkZ, spacing, out int offsetZ);

            if (offsetX != 0 || offsetZ != 0) return false;

            // 使用区域种子决定
            int regionSeed = regionX * 341873128712 + regionZ * 132897987541 + spacing;
            Random regionRandom = new Random(regionSeed);
            return regionRandom.NextDouble() < 0.5;
        }

        // ========================================
        // 村庄
        // ========================================
        public void GenerateVillage(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            // 检查生物群系
            Biome biome = world.GetBiome(centerX, centerZ);
            if (biome == Biome.Ocean || biome == Biome.DeepOcean ||
                biome == Biome.River || biome == Biome.Mountains)
            {
                return;
            }

            // 生成村庄中心（水井）
            GenerateWell(centerX, groundY, centerZ);

            // 生成道路
            GenerateVillageRoads(centerX, groundY, centerZ);

            // 生成房屋
            int houseCount = random.Next(5, 12);
            for (int i = 0; i < houseCount; i++)
            {
                int angle = random.Next(360);
                int distance = random.Next(10, 30);
                int houseX = centerX + (int)(Math.Cos(angle * Math.PI / 180) * distance);
                int houseZ = centerZ + (int)(Math.Sin(angle * Math.PI / 180) * distance);
                int houseY = world.GetHighestBlockY(houseX, houseZ);

                if (houseY > 0)
                {
                    GenerateHouse(houseX, houseY, houseZ, random.Next(4));
                }
            }

            // 生成农田
            int farmCount = random.Next(2, 5);
            for (int i = 0; i < farmCount; i++)
            {
                int angle = random.Next(360);
                int distance = random.Next(8, 20);
                int farmX = centerX + (int)(Math.Cos(angle * Math.PI / 180) * distance);
                int farmZ = centerZ + (int)(Math.Sin(angle * Math.PI / 180) * distance);
                int farmY = world.GetHighestBlockY(farmX, farmZ);

                if (farmY > 0)
                {
                    GenerateFarmland(farmX, farmY, farmZ);
                }
            }

            Console.WriteLine($"[StructureGenerator] 生成村庄 at ({centerX}, {centerZ})");
        }

        private void GenerateWell(int x, int y, int z)
        {
            // 水井结构
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_STONE_BRICKS);
                }
            }

            // 水
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    world.SetBlock(x + dx, y + 1, z + dz, GameConstants.BLOCK_WATER_STILL);
                }
            }

            // 柱子
            world.SetBlock(x - 2, y + 1, z - 2, GameConstants.BLOCK_LOG);
            world.SetBlock(x + 2, y + 1, z - 2, GameConstants.BLOCK_LOG);
            world.SetBlock(x - 2, y + 1, z + 2, GameConstants.BLOCK_LOG);
            world.SetBlock(x + 2, y + 1, z + 2, GameConstants.BLOCK_LOG);

            // 屋顶
            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    world.SetBlock(x + dx, y + 4, z + dz, GameConstants.BLOCK_WOOD_PLANKS);
                }
            }
        }

        private void GenerateVillageRoads(int centerX, int centerY, int centerZ)
        {
            // 简单的十字道路
            for (int i = -20; i <= 20; i++)
            {
                world.SetBlock(centerX + i, centerY, centerZ, GameConstants.BLOCK_GRAVEL);
                world.SetBlock(centerX, centerY, centerZ + i, GameConstants.BLOCK_GRAVEL);
            }
        }

        private void GenerateHouse(int x, int y, int z, int type)
        {
            int width = random.Next(5, 8);
            int depth = random.Next(5, 8);
            int height = random.Next(3, 5);

            // 地基
            for (int dx = 0; dx < width; dx++)
            {
                for (int dz = 0; dz < depth; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_WOOD_PLANKS);
                }
            }

            // 墙壁
            for (int dy = 1; dy <= height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    world.SetBlock(x + dx, y + dy, z, GameConstants.BLOCK_LOG);
                    world.SetBlock(x + dx, y + dy, z + depth - 1, GameConstants.BLOCK_LOG);
                }
                for (int dz = 0; dz < depth; dz++)
                {
                    world.SetBlock(x, y + dy, z + dz, GameConstants.BLOCK_LOG);
                    world.SetBlock(x + width - 1, y + dy, z + dz, GameConstants.BLOCK_LOG);
                }
            }

            // 填充墙壁
            for (int dy = 1; dy <= height; dy++)
            {
                for (int dx = 1; dx < width - 1; dx++)
                {
                    for (int dz = 1; dz < depth - 1; dz++)
                    {
                        world.SetBlock(x + dx, y + dy, z + dz, GameConstants.BLOCK_AIR);
                    }
                }
            }

            // 屋顶
            for (int dx = -1; dx <= width; dx++)
            {
                for (int dz = -1; dz <= depth; dz++)
                {
                    world.SetBlock(x + dx, y + height + 1, z + dz, GameConstants.BLOCK_WOOD_PLANKS);
                }
            }

            // 门
            world.SetBlock(x + width / 2, y + 1, z, GameConstants.BLOCK_DOOR);
            world.SetBlock(x + width / 2, y + 2, z, GameConstants.BLOCK_DOOR);

            // 窗户
            world.SetBlock(x + 1, y + 2, z + depth / 2, GameConstants.BLOCK_GLASS);
            world.SetBlock(x + width - 2, y + 2, z + depth / 2, GameConstants.BLOCK_GLASS);
        }

        private void GenerateFarmland(int x, int y, int z)
        {
            // 农田
            for (int dx = 0; dx < 6; dx++)
            {
                for (int dz = 0; dz < 6; dz++)
                {
                    world.SetBlock(x + dx, y, z + dz, GameConstants.BLOCK_FARMLAND);
                    if (random.NextDouble() < 0.7)
                    {
                        world.SetBlock(x + dx, y + 1, z + dz, GameConstants.BLOCK_WHEAT);
                    }
                }
            }

            // 水渠
            for (int dx = 0; dx < 6; dx++)
            {
                world.SetBlock(x + dx, y, z + 3, GameConstants.BLOCK_WATER_STILL);
            }
        }

        // ========================================
        // 地牢
        // ========================================
        public void GenerateDungeon(int centerX, int centerZ)
        {
            int y = random.Next(10, 40);

            // 地牢房间
            int width = random.Next(5, 8);
            int depth = random.Next(5, 8);

            // 清空房间
            for (int dx = 0; dx < width; dx++)
            {
                for (int dz = 0; dz < depth; dz++)
                {
                    for (int dy = 0; dy < 4; dy++)
                    {
                        world.SetBlock(centerX + dx, y + dy, centerZ + dz, GameConstants.BLOCK_AIR);
                    }
                }
            }

            // 墙壁（圆石）
            for (int dx = -1; dx <= width; dx++)
            {
                for (int dz = -1; dz <= depth; dz++)
                {
                    world.SetBlock(centerX + dx, y - 1, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                    world.SetBlock(centerX + dx, y + 4, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }

            for (int dy = 0; dy < 4; dy++)
            {
                for (int dx = -1; dx <= width; dx++)
                {
                    world.SetBlock(centerX + dx, y + dy, centerZ - 1, GameConstants.BLOCK_COBBLESTONE);
                    world.SetBlock(centerX + dx, y + dy, centerZ + depth, GameConstants.BLOCK_COBBLESTONE);
                }
                for (int dz = -1; dz <= depth; dz++)
                {
                    world.SetBlock(centerX - 1, y + dy, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                    world.SetBlock(centerX + width, y + dy, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                }
            }

            // 刷怪笼
            world.SetBlock(centerX + width / 2, y + 1, centerZ + depth / 2, GameConstants.BLOCK_SPAWNER);

            // 宝箱
            int chestCount = random.Next(1, 3);
            for (int i = 0; i < chestCount; i++)
            {
                int chestX = centerX + random.Next(1, width - 1);
                int chestZ = centerZ + random.Next(1, depth - 1);
                world.SetBlock(chestX, y + 1, chestZ, GameConstants.BLOCK_CHEST);
            }

            // 苔藓石
            for (int i = 0; i < 10; i++)
            {
                int mx = centerX + random.Next(-1, width + 1);
                int mz = centerZ + random.Next(-1, depth + 1);
                int my = y + random.Next(0, 4);
                if (world.GetBlock(mx, my, mz) == GameConstants.BLOCK_COBBLESTONE)
                {
                    world.SetBlock(mx, my, mz, GameConstants.BLOCK_MOSSY_COBBLESTONE);
                }
            }

            Console.WriteLine($"[StructureGenerator] 生成地牢 at ({centerX}, {y}, {centerZ})");
        }

        // ========================================
        // 废弃矿井
        // ========================================
        public void GenerateMineshaft(int centerX, int centerZ)
        {
            int y = random.Next(15, 45);

            // 主隧道
            int length = random.Next(30, 60);
            int direction = random.Next(4);

            for (int i = 0; i < length; i++)
            {
                int tunnelX = centerX;
                int tunnelZ = centerZ;

                switch (direction)
                {
                    case 0: tunnelX += i; break;
                    case 1: tunnelX -= i; break;
                    case 2: tunnelZ += i; break;
                    case 3: tunnelZ -= i; break;
                }

                // 挖掘隧道
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        for (int dy = 0; dy < 3; dy++)
                        {
                            world.SetBlock(tunnelX + dx, y + dy, tunnelZ + dz, GameConstants.BLOCK_AIR);
                        }
                    }
                }

                // 支撑柱
                if (i % 5 == 0)
                {
                    world.SetBlock(tunnelX - 1, y, tunnelZ, GameConstants.BLOCK_LOG);
                    world.SetBlock(tunnelX - 1, y + 1, tunnelZ, GameConstants.BLOCK_LOG);
                    world.SetBlock(tunnelX + 1, y, tunnelZ, GameConstants.BLOCK_LOG);
                    world.SetBlock(tunnelX + 1, y + 1, tunnelZ, GameConstants.BLOCK_LOG);
                }

                // 铁轨
                if (random.NextDouble() < 0.5)
                {
                    world.SetBlock(tunnelX, y, tunnelZ, GameConstants.BLOCK_RAIL);
                }

                // 分支隧道
                if (i % 15 == 0 && i > 0)
                {
                    GenerateMineshaftBranch(tunnelX, y, tunnelZ, (direction + 1) % 4);
                }
            }

            // 宝箱矿车
            if (random.NextDouble() < 0.5)
            {
                int chestX = centerX + random.Next(5, length - 5);
                int chestZ = centerZ;
                if (direction >= 2)
                {
                    chestX = centerX;
                    chestZ = centerZ + random.Next(5, length - 5);
                }
                world.SetBlock(chestX, y, chestZ, GameConstants.BLOCK_CHEST);
            }

            Console.WriteLine($"[StructureGenerator] 生成废弃矿井 at ({centerX}, {y}, {centerZ})");
        }

        private void GenerateMineshaftBranch(int x, int y, int z, int direction)
        {
            int length = random.Next(10, 25);

            for (int i = 0; i < length; i++)
            {
                int branchX = x;
                int branchZ = z;

                switch (direction)
                {
                    case 0: branchX += i; break;
                    case 1: branchX -= i; break;
                    case 2: branchZ += i; break;
                    case 3: branchZ -= i; break;
                }

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        for (int dy = 0; dy < 3; dy++)
                        {
                            world.SetBlock(branchX + dx, y + dy, branchZ + dz, GameConstants.BLOCK_AIR);
                        }
                    }
                }
            }
        }

        // ========================================
        // 沙漠神殿
        // ========================================
        public void GenerateDesertTemple(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            Biome biome = world.GetBiome(centerX, centerZ);
            if (biome != Biome.Desert && biome != Biome.DesertHills) return;

            // 主体结构
            for (int dx = -8; dx <= 8; dx++)
            {
                for (int dz = -8; dz <= 8; dz++)
                {
                    for (int dy = 0; dy < 6; dy++)
                    {
                        world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_SANDSTONE);
                    }
                }
            }

            // 金字塔顶部
            for (int layer = 0; layer < 5; layer++)
            {
                int size = 8 - layer;
                for (int dx = -size; dx <= size; dx++)
                {
                    for (int dz = -size; dz <= size; dz++)
                    {
                        world.SetBlock(centerX + dx, groundY + 6 + layer, centerZ + dz, GameConstants.BLOCK_SANDSTONE);
                    }
                }
            }

            // 塔楼
            GenerateDesertTower(centerX - 6, groundY, centerZ - 6);
            GenerateDesertTower(centerX + 6, groundY, centerZ - 6);
            GenerateDesertTower(centerX - 6, groundY, centerZ + 6);
            GenerateDesertTower(centerX + 6, groundY, centerZ + 6);

            // 入口
            for (int dy = 1; dy <= 3; dy++)
            {
                world.SetBlock(centerX, groundY + dy, centerZ - 8, GameConstants.BLOCK_AIR);
            }

            // 地下室（宝藏室）
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    for (int dy = -5; dy < 0; dy++)
                    {
                        world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_AIR);
                    }
                }
            }

            // 宝箱
            world.SetBlock(centerX - 2, groundY - 4, centerZ - 2, GameConstants.BLOCK_CHEST);
            world.SetBlock(centerX + 2, groundY - 4, centerZ - 2, GameConstants.BLOCK_CHEST);
            world.SetBlock(centerX - 2, groundY - 4, centerZ + 2, GameConstants.BLOCK_CHEST);
            world.SetBlock(centerX + 2, groundY - 4, centerZ + 2, GameConstants.BLOCK_CHEST);

            // 压力板（陷阱）
            world.SetBlock(centerX, groundY - 4, centerZ, GameConstants.BLOCK_STONE_PRESSURE_PLATE);

            // TNT陷阱
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    world.SetBlock(centerX + dx, groundY - 5, centerZ + dz, GameConstants.BLOCK_TNT);
                }
            }

            Console.WriteLine($"[StructureGenerator] 生成沙漠神殿 at ({centerX}, {centerZ})");
        }

        private void GenerateDesertTower(int x, int y, int z)
        {
            for (int dy = 0; dy < 10; dy++)
            {
                world.SetBlock(x, y + dy, z, GameConstants.BLOCK_SANDSTONE);
                world.SetBlock(x + 1, y + dy, z, GameConstants.BLOCK_SANDSTONE);
                world.SetBlock(x, y + dy, z + 1, GameConstants.BLOCK_SANDSTONE);
                world.SetBlock(x + 1, y + dy, z + 1, GameConstants.BLOCK_SANDSTONE);
            }
        }

        // ========================================
        // 丛林神庙
        // ========================================
        public void GenerateJungleTemple(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            Biome biome = world.GetBiome(centerX, centerZ);
            if (biome != Biome.Jungle && biome != Biome.JungleHills) return;

            // 主体（圆石）
            for (int dx = -5; dx <= 5; dx++)
            {
                for (int dz = -5; dz <= 5; dz++)
                {
                    for (int dy = 0; dy < 8; dy++)
                    {
                        world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_COBBLESTONE);
                    }
                }
            }

            // 内部空间
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    for (int dy = 1; dy < 6; dy++)
                    {
                        world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_AIR);
                    }
                }
            }

            // 藤蔓装饰
            for (int i = 0; i < 20; i++)
            {
                int vx = centerX + random.Next(-5, 6);
                int vz = centerZ + random.Next(-5, 6);
                int vy = groundY + random.Next(1, 8);
                if (world.GetBlock(vx, vy, vz) == GameConstants.BLOCK_COBBLESTONE)
                {
                    world.SetBlock(vx, vy, vz, GameConstants.BLOCK_VINE);
                }
            }

            // 宝箱
            world.SetBlock(centerX - 2, groundY + 1, centerZ + 2, GameConstants.BLOCK_CHEST);
            world.SetBlock(centerX + 2, groundY + 1, centerZ - 2, GameConstants.BLOCK_CHEST);

            Console.WriteLine($"[StructureGenerator] 生成丛林神庙 at ({centerX}, {centerZ})");
        }

        // ========================================
        // 海底遗迹
        // ========================================
        public void GenerateOceanRuin(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            Biome biome = world.GetBiome(centerX, centerZ);
            if (biome != Biome.Ocean && biome != Biome.DeepOcean) return;

            // 石砖结构
            for (int dx = -4; dx <= 4; dx++)
            {
                for (int dz = -4; dz <= 4; dz++)
                {
                    for (int dy = 0; dy < 4; dy++)
                    {
                        if (random.NextDouble() < 0.8)
                        {
                            world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_STONE_BRICKS);
                        }
                    }
                }
            }

            // 宝箱
            if (random.NextDouble() < 0.7)
            {
                world.SetBlock(centerX, groundY + 1, centerZ, GameConstants.BLOCK_CHEST);
            }

            Console.WriteLine($"[StructureGenerator] 生成海底遗迹 at ({centerX}, {centerZ})");
        }

        // ========================================
        // 沉船
        // ========================================
        public void GenerateShipwreck(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            Biome biome = world.GetBiome(centerX, centerZ);
            if (biome != Biome.Ocean && biome != Biome.DeepOcean &&
                biome != Biome.Beach) return;

            // 船体（木头）
            for (int dx = -10; dx <= 10; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    for (int dy = 0; dy < 3; dy++)
                    {
                        if (random.NextDouble() < 0.7)
                        {
                            world.SetBlock(centerX + dx, groundY + dy, centerZ + dz, GameConstants.BLOCK_LOG);
                        }
                    }
                }
            }

            // 宝箱
            int chestCount = random.Next(1, 3);
            for (int i = 0; i < chestCount; i++)
            {
                int chestX = centerX + random.Next(-8, 9);
                int chestZ = centerZ + random.Next(-2, 3);
                world.SetBlock(chestX, groundY + 1, chestZ, GameConstants.BLOCK_CHEST);
            }

            Console.WriteLine($"[StructureGenerator] 生成沉船 at ({centerX}, {centerZ})");
        }

        // ========================================
        // 废弃传送门
        // ========================================
        public void GenerateRuinedPortal(int centerX, int centerZ)
        {
            int groundY = world.GetHighestBlockY(centerX, centerZ);
            if (groundY < 0) return;

            // 传送门框架（黑曜石）
            for (int dy = 0; dy < 5; dy++)
            {
                world.SetBlock(centerX - 2, groundY + dy, centerZ, GameConstants.BLOCK_OBSIDIAN);
                world.SetBlock(centerX + 2, groundY + dy, centerZ, GameConstants.BLOCK_OBSIDIAN);
            }
            for (int dx = -2; dx <= 2; dx++)
            {
                world.SetBlock(centerX + dx, groundY, centerZ, GameConstants.BLOCK_OBSIDIAN);
                world.SetBlock(centerX + dx, groundY + 4, centerZ, GameConstants.BLOCK_OBSIDIAN);
            }

            // 传送门内部
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = 1; dy <= 3; dy++)
                {
                    if (random.NextDouble() < 0.5)
                    {
                        world.SetBlock(centerX + dx, groundY + dy, centerZ, GameConstants.BLOCK_NETHER_PORTAL);
                    }
                }
            }

            // 周围的岩浆和黑石
            for (int i = 0; i < 10; i++)
            {
                int fx = centerX + random.Next(-5, 6);
                int fz = centerZ + random.Next(-5, 6);
                if (world.GetBlock(fx, groundY, fz) != GameConstants.BLOCK_OBSIDIAN)
                {
                    world.SetBlock(fx, groundY, fz, GameConstants.BLOCK_BLACKSTONE);
                }
            }

            // 宝箱
            if (random.NextDouble() < 0.5)
            {
                world.SetBlock(centerX + 3, groundY + 1, centerZ + 1, GameConstants.BLOCK_CHEST);
            }

            Console.WriteLine($"[StructureGenerator] 生成废弃传送门 at ({centerX}, {centerZ})");
        }
    }
}
