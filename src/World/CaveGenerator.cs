using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class CaveGenerator
    {
        private readonly long seed;
        private readonly NoiseGenerator noise;
        private readonly Random random;

        // 洞穴参数
        public float CaveFrequency { get; set; } = 0.03f;
        public float CaveThreshold { get; set; } = 0.35f;
        public float LargeCaveFrequency { get; set; } = 0.02f;
        public float LargeCaveThreshold { get; set; } = 0.5f;
        public int MaxCaveHeight { get; set; } = 120;
        public int MinCaveHeight { get; set; } = 5;
        public int LavaLevel { get; set; } = 10;

        // 峡谷参数
        public float RavineFrequency { get; set; } = 0.01f;
        public float RavineWidth { get; set; } = 3.0f;
        public int RavineMinDepth { get; set; } = 20;
        public int RavineMaxDepth { get; set; } = 60;

        // 溶洞参数
        public float CavernFrequency { get; set; } = 0.005f;
        public float CavernRadius { get; set; } = 8.0f;

        public CaveGenerator(long seed)
        {
            this.seed = seed;
            noise = new NoiseGenerator(seed);
            random = new Random((int)seed);
        }

        // ========================================
        // 主洞穴检测
        // ========================================
        public bool IsCave(float x, float y, float z)
        {
            if (y < MinCaveHeight || y > MaxCaveHeight)
                return false;

            // 基础洞穴噪声
            float noise1 = noise.Perlin3D(x * CaveFrequency, y * CaveFrequency * 1.5f, z * CaveFrequency);
            float noise2 = noise.Perlin3D(x * CaveFrequency * 2 + 100, y * CaveFrequency * 3 + 100, z * CaveFrequency * 2 + 100);

            // 组合噪声
            float combined = noise1 * 0.6f + noise2 * 0.4f;

            // 大洞穴
            float largeCave = noise.Perlin3D(x * LargeCaveFrequency, y * LargeCaveFrequency, z * LargeCaveFrequency);
            if (largeCave > LargeCaveThreshold && y < 80)
            {
                return true;
            }

            return combined > CaveThreshold;
        }

        public bool IsLavaCave(float x, float y, float z)
        {
            if (y > LavaLevel) return false;

            float noiseVal = noise.Perlin3D(x * 0.05f, y * 0.05f, z * 0.05f);
            return noiseVal > 0.4f;
        }

        // ========================================
        // 峡谷生成
        // ========================================
        public List<Ravine> GenerateRavines(int chunkX, int chunkZ)
        {
            List<Ravine> ravines = new List<Ravine>();

            // 检查这个区块是否应该生成峡谷
            float noiseVal = noise.Perlin2D(chunkX * RavineFrequency + 30000, chunkZ * RavineFrequency + 30000);
            if (noiseVal < 0.85f) return ravines;

            Random r = new Random((int)(seed + chunkX * 10000 + chunkZ));
            int ravineCount = r.Next(1, 3);

            for (int i = 0; i < ravineCount; i++)
            {
                Ravine ravine = new Ravine
                {
                    StartX = chunkX * GameConstants.CHUNK_SIZE + r.Next(0, GameConstants.CHUNK_SIZE),
                    StartZ = chunkZ * GameConstants.CHUNK_SIZE + r.Next(0, GameConstants.CHUNK_SIZE),
                    StartY = r.Next(RavineMinDepth, RavineMaxDepth),
                    Length = r.Next(30, 80),
                    Width = RavineWidth * (float)(r.NextDouble() * 0.5 + 0.75),
                    Direction = (float)(r.NextDouble() * Math.PI * 2),
                    Curve = (float)(r.NextDouble() * 0.5 - 0.25)
                };
                ravines.Add(ravine);
            }

            return ravines;
        }

        public bool IsInRavine(float x, float y, float z, Ravine ravine)
        {
            // 简化的峡谷检测
            float dx = x - ravine.StartX;
            float dz = z - ravine.StartZ;

            float projectedLength = dx * (float)Math.Cos(ravine.Direction) + dz * (float)Math.Sin(ravine.Direction);
            if (projectedLength < 0 || projectedLength > ravine.Length) return false;

            float perpendicularDist = Math.Abs(dx * (float)-Math.Sin(ravine.Direction) + dz * (float)Math.Cos(ravine.Direction));
            float curveOffset = (float)Math.Sin(projectedLength * 0.1f) * ravine.Curve * projectedLength;

            float currentWidth = ravine.Width * (1.0f - projectedLength / ravine.Length * 0.5f);

            return perpendicularDist + curveOffset < currentWidth && y < ravine.StartY + 10 && y > ravine.StartY - 20;
        }

        // ========================================
        // 溶洞生成
        // ========================================
        public List<Cavern> GenerateCaverns(int chunkX, int chunkZ)
        {
            List<Cavern> caverns = new List<Cavern>();

            float noiseVal = noise.Perlin2D(chunkX * CavernFrequency + 40000, chunkZ * CavernFrequency + 40000);
            if (noiseVal < 0.9f) return caverns;

            Random r = new Random((int)(seed + chunkX * 20000 + chunkZ));
            int cavernCount = r.Next(1, 2);

            for (int i = 0; i < cavernCount; i++)
            {
                Cavern cavern = new Cavern
                {
                    CenterX = chunkX * GameConstants.CHUNK_SIZE + r.Next(4, GameConstants.CHUNK_SIZE - 4),
                    CenterY = r.Next(15, 50),
                    CenterZ = chunkZ * GameConstants.CHUNK_SIZE + r.Next(4, GameConstants.CHUNK_SIZE - 4),
                    RadiusX = CavernRadius * (float)(r.NextDouble() * 0.5 + 0.75),
                    RadiusY = CavernRadius * 0.6f * (float)(r.NextDouble() * 0.5 + 0.75),
                    RadiusZ = CavernRadius * (float)(r.NextDouble() * 0.5 + 0.75)
                };
                caverns.Add(cavern);
            }

            return caverns;
        }

        public bool IsInCavern(float x, float y, float z, Cavern cavern)
        {
            float dx = (x - cavern.CenterX) / cavern.RadiusX;
            float dy = (y - cavern.CenterY) / cavern.RadiusY;
            float dz = (z - cavern.CenterZ) / cavern.RadiusZ;

            return dx * dx + dy * dy + dz * dz < 1.0f;
        }

        // ========================================
        // 洞穴装饰
        // ========================================
        public void DecorateCave(Chunk chunk, int x, int y, int z)
        {
            // 石笋
            if (random.NextDouble() < 0.02f && y > 10)
            {
                GenerateStalagmite(chunk, x, y, z);
            }

            // 钟乳石
            if (random.NextDouble() < 0.02f && y < GameConstants.CHUNK_HEIGHT - 5)
            {
                GenerateStalactite(chunk, x, y, z);
            }

            // 矿石暴露
            if (random.NextDouble() < 0.05f)
            {
                GenerateExposedOre(chunk, x, y, z);
            }
        }

        private void GenerateStalagmite(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(1, 4);
            for (int dy = 0; dy < height; dy++)
            {
                if (chunk.GetBlock(x, y + dy, z) == GameConstants.BLOCK_AIR)
                {
                    chunk.SetBlock(x, y + dy, z, GameConstants.BLOCK_STONE);
                }
            }
        }

        private void GenerateStalactite(Chunk chunk, int x, int y, int z)
        {
            int height = random.Next(1, 3);
            for (int dy = 0; dy < height; dy++)
            {
                if (chunk.GetBlock(x, y - dy, z) == GameConstants.BLOCK_AIR)
                {
                    chunk.SetBlock(x, y - dy, z, GameConstants.BLOCK_STONE);
                }
            }
        }

        private void GenerateExposedOre(Chunk chunk, int x, int y, int z)
        {
            ushort[] ores = {
                GameConstants.BLOCK_COAL_ORE,
                GameConstants.BLOCK_IRON_ORE,
                GameConstants.BLOCK_GOLD_ORE,
                GameConstants.BLOCK_REDSTONE_ORE,
                GameConstants.BLOCK_LAPIS_ORE
            };

            ushort ore = ores[random.Next(ores.Length)];
            if (chunk.GetBlock(x, y, z) == GameConstants.BLOCK_STONE)
            {
                chunk.SetBlock(x, y, z, ore);
            }
        }

        // ========================================
        // 地下湖
        // ========================================
        public bool IsUndergroundLake(float x, float y, float z)
        {
            if (y > 50 || y < 15) return false;

            float noiseVal = noise.Perlin3D(x * 0.02f + 50000, y * 0.02f, z * 0.02f + 50000);
            return noiseVal > 0.6f;
        }

        public bool IsUndergroundLavaLake(float x, float y, float z)
        {
            if (y > 20) return false;

            float noiseVal = noise.Perlin3D(x * 0.03f + 60000, y * 0.03f, z * 0.03f + 60000);
            return noiseVal > 0.55f;
        }

        // ========================================
        // 废弃矿井
        // ========================================
        public List<MineTunnel> GenerateMineTunnels(int chunkX, int chunkZ)
        {
            List<MineTunnel> tunnels = new List<MineTunnel>();

            float noiseVal = noise.Perlin2D(chunkX * 0.05f + 70000, chunkZ * 0.05f + 70000);
            if (noiseVal < 0.88f) return tunnels;

            Random r = new Random((int)(seed + chunkX * 30000 + chunkZ));
            int tunnelCount = r.Next(1, 4);

            for (int i = 0; i < tunnelCount; i++)
            {
                MineTunnel tunnel = new MineTunnel
                {
                    StartX = chunkX * GameConstants.CHUNK_SIZE + r.Next(0, GameConstants.CHUNK_SIZE),
                    StartY = r.Next(15, 45),
                    StartZ = chunkZ * GameConstants.CHUNK_SIZE + r.Next(0, GameConstants.CHUNK_SIZE),
                    Length = r.Next(20, 60),
                    Direction = (float)(r.NextDouble() * Math.PI * 2),
                    HasRails = r.NextDouble() > 0.5f,
                    HasSupport = r.NextDouble() > 0.3f
                };
                tunnels.Add(tunnel);
            }

            return tunnels;
        }

        public bool IsInMineTunnel(float x, float y, float z, MineTunnel tunnel)
        {
            float dx = x - tunnel.StartX;
            float dz = z - tunnel.StartZ;

            float projectedLength = dx * (float)Math.Cos(tunnel.Direction) + dz * (float)Math.Sin(tunnel.Direction);
            if (projectedLength < 0 || projectedLength > tunnel.Length) return false;

            float perpendicularDist = Math.Abs(dx * (float)-Math.Sin(tunnel.Direction) + dz * (float)Math.Cos(tunnel.Direction));

            return perpendicularDist < 1.5f && Math.Abs(y - tunnel.StartY) < 1.5f;
        }
    }

    // ========================================
    // 数据结构
    // ========================================
    public struct Ravine
    {
        public float StartX;
        public float StartZ;
        public int StartY;
        public float Length;
        public float Width;
        public float Direction;
        public float Curve;
    }

    public struct Cavern
    {
        public float CenterX;
        public float CenterY;
        public float CenterZ;
        public float RadiusX;
        public float RadiusY;
        public float RadiusZ;
    }

    public struct MineTunnel
    {
        public float StartX;
        public int StartY;
        public float StartZ;
        public float Length;
        public float Direction;
        public bool HasRails;
        public bool HasSupport;
    }
}
