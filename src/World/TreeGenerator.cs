using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class TreeGenerator
    {
        private readonly long seed;
        private readonly Random random;

        // 树木参数
        public int MinTreeHeight { get; set; } = 4;
        public int MaxTreeHeight { get; set; } = 8;
        public int CanopyRadius { get; set; } = 2;
        public float LeafDensity { get; set; } = 0.7f;

        public TreeGenerator(long seed)
        {
            this.seed = seed;
            random = new Random((int)seed);
        }

        public TreeGenerator(WorldManager world, long seed) : this(seed)
        {
        }

        public void GenerateTree(Chunk chunk, int x, int y, int z, TreeType type)
        {
            switch (type)
            {
                case TreeType.Oak:
                    GenerateOakTree(chunk, x, y, z);
                    break;
                case TreeType.Birch:
                    GenerateBirchTree(chunk, x, y, z);
                    break;
                case TreeType.Spruce:
                    GenerateSpruceTree(chunk, x, y, z);
                    break;
                case TreeType.Jungle:
                    GenerateJungleTree(chunk, x, y, z);
                    break;
                case TreeType.Acacia:
                    GenerateAcaciaTree(chunk, x, y, z);
                    break;
                case TreeType.DarkOak:
                    GenerateDarkOakTree(chunk, x, y, z);
                    break;
                case TreeType.SwampOak:
                    GenerateSwampOakTree(chunk, x, y, z);
                    break;
                case TreeType.HugeMushroom:
                    GenerateHugeMushroom(chunk, x, y, z);
                    break;
                case TreeType.Cherry:
                    GenerateCherryTree(chunk, x, y, z);
                    break;
            }
        }

        // ========================================
        // 橡树
        // ========================================
        private void GenerateOakTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(4, 7);

            // 树干
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_LOG);
            }

            // 树冠
            int canopyY = y + height - 2;
            for (int dy = -1; dy <= 2; dy++)
            {
                int radius = dy == 2 ? 1 : CanopyRadius;
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        if (dx == 0 && dz == 0 && dy < 2) continue;
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > LeafDensity) continue;

                        int lx = x + dx;
                        int ly = canopyY + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_LEAVES);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 白桦树
        // ========================================
        private void GenerateBirchTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(5, 8);

            // 树干（白桦木）
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_BIRCH_LOG);
            }

            // 树冠（更细长）
            int canopyY = y + height - 2;
            for (int dy = -1; dy <= 2; dy++)
            {
                int radius = dy == 2 ? 1 : 2;
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        if (dx == 0 && dz == 0 && dy < 2) continue;
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > 0.6f) continue;

                        int lx = x + dx;
                        int ly = canopyY + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_BIRCH_LEAVES);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 云杉树
        // ========================================
        private void GenerateSpruceTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(6, 10);

            // 树干
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_SPRUCE_LOG);
            }

            // 锥形树冠
            for (int dy = 0; dy < height; dy++)
            {
                int currentY = y + height - dy;
                int radius = (int)(dy * 0.5f) + 1;

                if (dy < 2) continue;

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        if (dx == 0 && dz == 0) continue;
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > 0.5f) continue;

                        int lx = x + dx;
                        int ly = currentY;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_SPRUCE_LEAVES);
                            }
                        }
                    }
                }
            }

            // 树顶
            chunk.SetBlock(x, y + height, z, GameConstants.BLOCK_SPRUCE_LEAVES);
        }

        // ========================================
        // 丛林木
        // ========================================
        private void GenerateJungleTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(8, 14);

            // 树干（更粗）
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_JUNGLE_LOG);
                if (random.NextDouble() > 0.5f)
                {
                    chunk.SetBlock(x + 1, y + dy, z, GameConstants.BLOCK_JUNGLE_LOG);
                }
            }

            // 大树冠
            int canopyY = y + height - 3;
            for (int dy = -2; dy <= 2; dy++)
            {
                int radius = dy == 2 ? 2 : 3;
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > 0.4f) continue;

                        int lx = x + dx;
                        int ly = canopyY + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_JUNGLE_LEAVES);
                            }
                        }
                    }
                }
            }

            // 藤蔓
            for (int dy = 0; dy < height - 2; dy++)
            {
                if (random.NextDouble() > 0.7f)
                {
                    int vineX = x + random.Next(-1, 2);
                    int vineZ = z + random.Next(-1, 2);
                    if (vineX >= 0 && vineX < GameConstants.CHUNK_SIZE &&
                        vineZ >= 0 && vineZ < GameConstants.CHUNK_SIZE)
                    {
                        if (chunk.GetBlock(vineX, y + dy, vineZ) == GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(vineX, y + dy, vineZ, GameConstants.BLOCK_VINES);
                        }
                    }
                }
            }
        }

        // ========================================
        // 金合欢树
        // ========================================
        private void GenerateAcaciaTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(4, 7);

            // 树干（倾斜）
            int offsetX = 0;
            int offsetZ = 0;
            for (int dy = 0; dy < height; dy++)
            {
                if (dy > height / 2)
                {
                    offsetX = random.Next(-1, 2);
                    offsetZ = random.Next(-1, 2);
                }
                chunk.SetBlock(x + offsetX, y + dy, z + offsetZ, GameConstants.BLOCK_ACACIA_LOG);
            }

            // 扁平树冠
            int canopyY = y + height;
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    if (Math.Abs(dx) == 3 && Math.Abs(dz) == 3) continue;
                    if (random.NextDouble() > 0.7f) continue;

                    int lx = x + offsetX + dx;
                    int ly = canopyY;
                    int lz = z + offsetZ + dz;

                    if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                        ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                        lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                    {
                        if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_ACACIA_LEAVES);
                        }
                    }
                }
            }
        }

        // ========================================
        // 深色橡树
        // ========================================
        private void GenerateDarkOakTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(6, 9);

            // 粗树干（2x2）
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_DARK_OAK_LOG);
                chunk.SetBlock(x + 1, y + dy, z, GameConstants.BLOCK_DARK_OAK_LOG);
                chunk.SetBlock(x, y + dy, z + 1, GameConstants.BLOCK_DARK_OAK_LOG);
                chunk.SetBlock(x + 1, y + dy, z + 1, GameConstants.BLOCK_DARK_OAK_LOG);
            }

            // 茂密树冠
            int canopyY = y + height - 2;
            for (int dy = -2; dy <= 2; dy++)
            {
                int radius = dy == 2 ? 2 : 3;
                for (int dx = -radius; dx <= radius + 1; dx++)
                {
                    for (int dz = -radius; dz <= radius + 1; dz++)
                    {
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > 0.3f) continue;

                        int lx = x + dx;
                        int ly = canopyY + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_DARK_OAK_LEAVES);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 沼泽橡树
        // ========================================
        private void GenerateSwampOakTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(4, 6);

            // 树干
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_LOG);
            }

            // 扁平树冠
            int canopyY = y + height;
            for (int dx = -3; dx <= 3; dx++)
            {
                for (int dz = -3; dz <= 3; dz++)
                {
                    if (Math.Abs(dx) == 3 && Math.Abs(dz) == 3) continue;

                    int lx = x + dx;
                    int ly = canopyY;
                    int lz = z + dz;

                    if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                        ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                        lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                    {
                        if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_LEAVES);
                        }
                    }
                }
            }

            // 藤蔓
            for (int dy = 1; dy < height; dy++)
            {
                if (random.NextDouble() > 0.5f)
                {
                    chunk.SetBlock(x - 1, y + dy, z, GameConstants.BLOCK_VINES);
                }
                if (random.NextDouble() > 0.5f)
                {
                    chunk.SetBlock(x + 1, y + dy, z, GameConstants.BLOCK_VINES);
                }
            }
        }

        // ========================================
        // 巨型蘑菇
        // ========================================
        private void GenerateHugeMushroom(Chunk chunk, int x, int y, int z)
        {
            bool isRed = random.NextDouble() > 0.5f;
            int height = random.Next(4, 7);

            // 蘑菇柄
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_MUSHROOM_STEM);
            }

            // 蘑菇帽
            ushort capBlock = isRed ? GameConstants.BLOCK_MUSHROOM_BLOCK_RED : GameConstants.BLOCK_MUSHROOM_BLOCK_BROWN;
            int capY = y + height;

            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dz = -2; dz <= 2; dz++)
                {
                    if (Math.Abs(dx) == 2 && Math.Abs(dz) == 2) continue;

                    int lx = x + dx;
                    int ly = capY;
                    int lz = z + dz;

                    if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                        ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                        lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                    {
                        if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                        {
                            chunk.SetBlock(lx, ly, lz, capBlock);
                        }
                    }
                }
            }

            // 帽顶
            chunk.SetBlock(x, capY + 1, z, capBlock);
        }

        // ========================================
        // 樱花树
        // ========================================
        private void GenerateCherryTree(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(5, 8);

            // 树干
            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_CHERRY_LOG);
            }

            // 粉色树冠
            int canopyY = y + height - 2;
            for (int dy = -1; dy <= 2; dy++)
            {
                int radius = dy == 2 ? 2 : 3;
                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        if (dx == 0 && dz == 0 && dy < 2) continue;
                        if (Math.Abs(dx) == radius && Math.Abs(dz) == radius && random.NextDouble() > 0.5f) continue;

                        int lx = x + dx;
                        int ly = canopyY + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_CHERRY_LEAVES);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 灌木
        // ========================================
        public void GenerateBush(Chunk chunk, int x, int y, int z)
        {
            // 小灌木
            chunk.SetBlock(x, y, z, GameConstants.BLOCK_LOG);

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    for (int dy = 0; dy <= 1; dy++)
                    {
                        if (dx == 0 && dz == 0 && dy == 0) continue;

                        int lx = x + dx;
                        int ly = y + dy;
                        int lz = z + dz;

                        if (lx >= 0 && lx < GameConstants.CHUNK_SIZE &&
                            ly >= 0 && ly < GameConstants.CHUNK_HEIGHT &&
                            lz >= 0 && lz < GameConstants.CHUNK_SIZE)
                        {
                            if (chunk.GetBlock(lx, ly, lz) == GameConstants.BLOCK_AIR)
                            {
                                chunk.SetBlock(lx, ly, lz, GameConstants.BLOCK_LEAVES);
                            }
                        }
                    }
                }
            }
        }

        // ========================================
        // 仙人掌
        // ========================================
        public void GenerateCactus(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(1, 4);

            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_CACTUS);
            }

            // 分支
            if (height > 2 && random.NextDouble() > 0.5f)
            {
                int branchY = y + random.Next(1, height - 1);
                int branchDir = random.Next(4);
                int bx = x, bz = z;

                switch (branchDir)
                {
                    case 0: bx++; break;
                    case 1: bx--; break;
                    case 2: bz++; break;
                    case 3: bz--; break;
                }

                if (bx >= 0 && bx < GameConstants.CHUNK_SIZE &&
                    bz >= 0 && bz < GameConstants.CHUNK_SIZE)
                {
                    chunk.SetBlock(bx, branchY, bz, GameConstants.BLOCK_CACTUS);
                    chunk.SetBlock(bx, branchY + 1, bz, GameConstants.BLOCK_CACTUS);
                }
            }
        }

        // ========================================
        // 甘蔗
        // ========================================
        public void GenerateSugarCane(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(2, 5);

            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_SUGAR_CANE);
            }
        }

        // ========================================
        // 竹子
        // ========================================
        public void GenerateBamboo(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(5, 12);

            for (int dy = 0; dy < height; dy++)
            {
                chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_BAMBOO_PLANKS); // 用木板代替
            }
        }
    }
}
