using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class NoiseGenerator
    {
        private readonly long seed;
        private readonly Random random;
        private readonly int[] permutationTable;

        // 噪声参数
        public float Frequency { get; set; } = 0.01f;
        public float Amplitude { get; set; } = 1.0f;
        public int Octaves { get; set; } = 4;
        public float Persistence { get; set; } = 0.5f;
        public float Lacunarity { get; set; } = 2.0f;

        public NoiseGenerator(long seed)
        {
            this.seed = seed;
            random = new Random((int)seed);
            permutationTable = new int[512];
            GeneratePermutationTable();
        }

        private void GeneratePermutationTable()
        {
            int[] p = new int[256];
            for (int i = 0; i < 256; i++)
            {
                p[i] = i;
            }

            // Fisher-Yates shuffle
            for (int i = 255; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (p[i], p[j]) = (p[j], p[i]);
            }

            for (int i = 0; i < 512; i++)
            {
                permutationTable[i] = p[i & 255];
            }
        }

        // ========================================
        // 2D Perlin Noise
        // ========================================
        public float Perlin2D(float x, float y)
        {
            x *= Frequency;
            y *= Frequency;

            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;

            float xf = x - (float)Math.Floor(x);
            float yf = y - (float)Math.Floor(y);

            float u = Fade(xf);
            float v = Fade(yf);

            int aa = permutationTable[permutationTable[xi] + yi];
            int ab = permutationTable[permutationTable[xi] + yi + 1];
            int ba = permutationTable[permutationTable[xi + 1] + yi];
            int bb = permutationTable[permutationTable[xi + 1] + yi + 1];

            float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
            float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

            return Lerp(x1, x2, v);
        }

        // ========================================
        // 3D Perlin Noise
        // ========================================
        public float Perlin3D(float x, float y, float z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;
            int zi = (int)Math.Floor(z) & 255;

            float xf = x - (float)Math.Floor(x);
            float yf = y - (float)Math.Floor(y);
            float zf = z - (float)Math.Floor(z);

            float u = Fade(xf);
            float v = Fade(yf);
            float w = Fade(zf);

            int aaa = permutationTable[permutationTable[permutationTable[xi] + yi] + zi];
            int aba = permutationTable[permutationTable[permutationTable[xi] + yi + 1] + zi];
            int aab = permutationTable[permutationTable[permutationTable[xi] + yi] + zi + 1];
            int abb = permutationTable[permutationTable[permutationTable[xi] + yi + 1] + zi + 1];
            int baa = permutationTable[permutationTable[permutationTable[xi + 1] + yi] + zi];
            int bba = permutationTable[permutationTable[permutationTable[xi + 1] + yi + 1] + zi];
            int bab = permutationTable[permutationTable[permutationTable[xi + 1] + yi] + zi + 1];
            int bbb = permutationTable[permutationTable[permutationTable[xi + 1] + yi + 1] + zi + 1];

            float x1 = Lerp(Grad3(aaa, xf, yf, zf), Grad3(baa, xf - 1, yf, zf), u);
            float x2 = Lerp(Grad3(aba, xf, yf - 1, zf), Grad3(bba, xf - 1, yf - 1, zf), u);
            float y1 = Lerp(x1, x2, v);

            x1 = Lerp(Grad3(aab, xf, yf, zf - 1), Grad3(bab, xf - 1, yf, zf - 1), u);
            x2 = Lerp(Grad3(abb, xf, yf - 1, zf - 1), Grad3(bbb, xf - 1, yf - 1, zf - 1), u);
            float y2 = Lerp(x1, x2, v);

            return Lerp(y1, y2, w);
        }

        // ========================================
        // Fractal Brownian Motion (分形布朗运动)
        // ========================================
        public float FBM2D(float x, float y)
        {
            float value = 0f;
            float amplitude = Amplitude;
            float frequency = Frequency;
            float maxValue = 0f;

            for (int i = 0; i < Octaves; i++)
            {
                value += Perlin2D(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= Persistence;
                frequency *= Lacunarity;
            }

            return value / maxValue;
        }

        public float FBM3D(float x, float y, float z)
        {
            float value = 0f;
            float amplitude = Amplitude;
            float frequency = Frequency;
            float maxValue = 0f;

            for (int i = 0; i < Octaves; i++)
            {
                value += Perlin3D(x * frequency, y * frequency, z * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= Persistence;
                frequency *= Lacunarity;
            }

            return value / maxValue;
        }

        // ========================================
        // Simplex Noise (更平滑的噪声)
        // ========================================
        public float Simplex2D(float x, float y)
        {
            const float F2 = 0.366025403f;
            const float G2 = 0.211324865f;

            float s = (x + y) * F2;
            int i = (int)Math.Floor(x + s);
            int j = (int)Math.Floor(y + s);

            float t = (i + j) * G2;
            float X0 = i - t;
            float Y0 = j - t;
            float x0 = x - X0;
            float y0 = y - Y0;

            int i1, j1;
            if (x0 > y0) { i1 = 1; j1 = 0; }
            else { i1 = 0; j1 = 1; }

            float x1 = x0 - i1 + G2;
            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2;
            float y2 = y0 - 1.0f + 2.0f * G2;

            int ii = i & 255;
            int jj = j & 255;

            float n0 = 0f, n1 = 0f, n2 = 0f;

            float t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 >= 0)
            {
                t0 *= t0;
                int gi0 = permutationTable[ii + permutationTable[jj]] % 12;
                n0 = t0 * t0 * Dot(gi0, x0, y0);
            }

            float t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 >= 0)
            {
                t1 *= t1;
                int gi1 = permutationTable[ii + i1 + permutationTable[jj + j1]] % 12;
                n1 = t1 * t1 * Dot(gi1, x1, y1);
            }

            float t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 >= 0)
            {
                t2 *= t2;
                int gi2 = permutationTable[ii + 1 + permutationTable[jj + 1]] % 12;
                n2 = t2 * t2 * Dot(gi2, x2, y2);
            }

            return 70.0f * (n0 + n1 + n2);
        }

        // ========================================
        // Ridged Multi-fractal (用于山脉)
        // ========================================
        public float RidgedMulti2D(float x, float y)
        {
            float value = 0f;
            float amplitude = 1f;
            float frequency = Frequency;
            float weight = 1f;
            float maxValue = 0f;

            for (int i = 0; i < Octaves; i++)
            {
                float noise = 1.0f - Math.Abs(Perlin2D(x * frequency, y * frequency));
                noise *= noise;
                noise *= weight;
                weight = noise * 0.5f;

                value += noise * amplitude;
                maxValue += amplitude;

                amplitude *= 0.5f;
                frequency *= 2.0f;
            }

            return value / maxValue;
        }

        // ========================================
        // Billow Noise (用于云朵等)
        // ========================================
        public float Billow2D(float x, float y)
        {
            return Math.Abs(FBM2D(x, y)) * 2.0f - 1.0f;
        }

        // ========================================
        // Voronoi Noise (用于细胞结构)
        // ========================================
        public float Voronoi2D(float x, float y, out float minDist)
        {
            minDist = float.MaxValue;
            int xi = (int)Math.Floor(x);
            int yi = (int)Math.Floor(y);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int cellX = xi + i;
                    int cellY = yi + j;

                    Random cellRandom = new Random(cellX * 73856093 ^ cellY * 19349663 ^ (int)seed);
                    float pointX = cellX + (float)cellRandom.NextDouble();
                    float pointY = cellY + (float)cellRandom.NextDouble();

                    float dist = (float)Math.Sqrt((x - pointX) * (x - pointX) + (y - pointY) * (y - pointY));
                    minDist = Math.Min(minDist, dist);
                }
            }

            return minDist;
        }

        // ========================================
        // Domain Warping (域扭曲，用于更自然的地形)
        // ========================================
        public float DomainWarp2D(float x, float y)
        {
            float warpX = FBM2D(x + 5.2f, y + 1.3f) * 4.0f;
            float warpY = FBM2D(x + 8.5f, y + 2.8f) * 4.0f;
            return FBM2D(x + warpX, y + warpY);
        }

        // ========================================
        // 工具函数
        // ========================================
        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private static float Grad3(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private static float Dot(int gi, float x, float y)
        {
            switch (gi % 12)
            {
                case 0: return x + y;
                case 1: return -x + y;
                case 2: return x - y;
                case 3: return -x - y;
                case 4: return x;
                case 5: return -x;
                case 6: return y;
                case 7: return -y;
                case 8: return x + y;
                case 9: return -x + y;
                case 10: return x - y;
                case 11: return -x - y;
                default: return 0;
            }
        }

        // ========================================
        // 便捷方法
        // ========================================
        public float GetHeight(float x, float z, float baseHeight, float variation)
        {
            float noise = DomainWarp2D(x, z);
            return baseHeight + noise * variation;
        }

        public float GetMountainHeight(float x, float z, float baseHeight, float mountainHeight)
        {
            float ridged = RidgedMulti2D(x, z);
            float fbm = FBM2D(x * 0.5f, z * 0.5f);
            float mountainMask = Math.Max(0, fbm * 2.0f - 0.5f);
            return baseHeight + ridged * mountainHeight * mountainMask;
        }

        public float GetTemperature(float x, float z)
        {
            float temp = FBM2D(x * 0.001f, z * 0.001f);
            float latitude = Math.Abs(z) * 0.0001f;
            return Math.Clamp(0.5f + temp * 0.5f - latitude, 0f, 1f);
        }

        public float GetHumidity(float x, float z)
        {
            float humidity = FBM2D(x * 0.002f + 1000, z * 0.002f + 1000);
            return Math.Clamp(0.5f + humidity * 0.5f, 0f, 1f);
        }

        public float GetContinentalness(float x, float z)
        {
            return FBM2D(x * 0.0005f, z * 0.0005f);
        }

        public float GetErosion(float x, float z)
        {
            return FBM2D(x * 0.003f + 500, z * 0.003f + 500);
        }

        public float GetWeirdness(float x, float z)
        {
            return FBM2D(x * 0.004f + 2000, z * 0.004f + 2000);
        }

        public bool IsCave(float x, float y, float z)
        {
            float noise1 = Perlin3D(x * 0.05f, y * 0.05f, z * 0.05f);
            float noise2 = Perlin3D(x * 0.1f + 100, y * 0.1f + 100, z * 0.1f + 100);
            return noise1 > 0.3f && noise2 > 0.3f;
        }

        public bool IsLargeCave(float x, float y, float z)
        {
            float noise = FBM3D(x * 0.03f, y * 0.03f, z * 0.03f);
            return noise > 0.5f && y < GameConstants.SEA_LEVEL - 10;
        }

        public bool IsLavaCave(float x, float y, float z)
        {
            float noise = Perlin3D(x * 0.04f, y * 0.04f, z * 0.04f);
            return noise > 0.4f && y < 10;
        }

        public int GetOreHeight(float x, float z, int minY, int maxY)
        {
            float noise = FBM2D(x * 0.1f, z * 0.1f);
            return (int)(minY + (maxY - minY) * (noise * 0.5f + 0.5f));
        }

        public bool ShouldSpawnOre(float x, float y, float z, float threshold)
        {
            float noise = Perlin3D(x * 0.2f, y * 0.2f, z * 0.2f);
            return noise > threshold;
        }

        public Vector2 GetTreePosition(int chunkX, int chunkZ, int index)
        {
            Random r = new Random((int)(seed + chunkX * 341873128713 + chunkZ * 132897987541 + index * 7919));
            float x = chunkX * GameConstants.CHUNK_SIZE + r.Next(2, GameConstants.CHUNK_SIZE - 2);
            float z = chunkZ * GameConstants.CHUNK_SIZE + r.Next(2, GameConstants.CHUNK_SIZE - 2);
            return new Vector2(x, z);
        }

        public int GetTreeCount(int chunkX, int chunkZ, BiomeType biome)
        {
            float noise = FBM2D(chunkX * 0.5f + 100, chunkZ * 0.5f + 100);
            int baseCount = biome switch
            {
                BiomeType.Forest => 8,
                BiomeType.BirchForest => 7,
                BiomeType.DarkForest => 12,
                BiomeType.Jungle => 15,
                BiomeType.Taiga => 6,
                BiomeType.Savanna => 2,
                BiomeType.Plains => 1,
                BiomeType.Desert => 0,
                BiomeType.SnowyTundra => 1,
                BiomeType.Mountains => 3,
                BiomeType.Swamp => 4,
                _ => 3
            };
            return Math.Max(0, (int)(baseCount * (noise * 0.5f + 0.7f)));
        }
    }
}
