using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public enum BiomeType
    {
        // 海洋
        Ocean,
        DeepOcean,
        FrozenOcean,
        DeepFrozenOcean,
        ColdOcean,
        DeepColdOcean,
        LukewarmOcean,
        DeepLukewarmOcean,
        WarmOcean,
        DeepWarmOcean,

        // 平原
        Plains,
        SunflowerPlains,

        // 沙漠
        Desert,
        DesertHills,
        DesertLakes,

        // 山脉
        Mountains,
        WoodedMountains,
        GravellyMountains,
        MountainEdge,
        Meadow,
        Grove,
        SnowySlopes,
        FrozenPeaks,
        JaggedPeaks,
        StonyPeaks,

        // 森林
        Forest,
        WoodedHills,
        FlowerForest,
        BirchForest,
        BirchForestHills,
        TallBirchForest,
        TallBirchHills,
        DarkForest,
        DarkForestHills,

        // 针叶林
        Taiga,
        TaigaHills,
        TaigaMountains,
        SnowyTaiga,
        SnowyTaigaHills,
        SnowyTaigaMountains,
        GiantTreeTaiga,
        GiantTreeTaigaHills,
        GiantSpruceTaiga,
        GiantSpruceTaigaHills,

        // 雪地
        SnowyTundra,
        SnowyMountains,
        IceSpikes,
        SnowyBeach,

        // 海滩
        Beach,
        StoneShore,

        // 河流
        River,
        FrozenRiver,

        // 沼泽
        Swamp,
        SwampHills,
        MangroveSwamp,

        // 丛林
        Jungle,
        JungleHills,
        ModifiedJungle,
        JungleEdge,
        ModifiedJungleEdge,
        BambooJungle,
        BambooJungleHills,

        // 热带草原
        Savanna,
        SavannaPlateau,
        ShatteredSavanna,
        ShatteredSavannaPlateau,

        // 恶地
        Badlands,
        BadlandsPlateau,
        ModifiedBadlandsPlateau,
        ErodedBadlands,
        WoodedBadlandsPlateau,
        ModifiedWoodedBadlandsPlateau,

        // 蘑菇岛
        MushroomFields,
        MushroomFieldShore,

        // 下界
        NetherWastes,
        SoulSandValley,
        CrimsonForest,
        WarpedForest,
        BasaltDeltas,

        // 末地
        TheEnd,
        EndHighlands,
        EndMidlands,
        SmallEndIslands,
        EndBarrens,

        // 洞穴
        DripstoneCaves,
        LushCaves,
        DeepDark,

        // 特殊
        TheVoid,
        Custom
    }

    public class BiomeGenerator
    {
        private readonly long seed;
        private readonly NoiseGenerator noise;

        // 生物群系参数
        public float TemperatureScale { get; set; } = 0.001f;
        public float HumidityScale { get; set; } = 0.0015f;
        public float ContinentalnessScale { get; set; } = 0.0005f;
        public float ErosionScale { get; set; } = 0.003f;
        public float WeirdnessScale { get; set; } = 0.004f;

        public BiomeGenerator(long seed)
        {
            this.seed = seed;
            noise = new NoiseGenerator(seed);
        }

        // ========================================
        // 主生物群系获取
        // ========================================
        public BiomeType GetBiome(int x, int z)
        {
            float temperature = GetTemperature(x, z);
            float humidity = GetHumidity(x, z);
            float continentalness = noise.GetContinentalness(x, z);
            float erosion = noise.GetErosion(x, z);
            float weirdness = noise.GetWeirdness(x, z);

            // 海洋判定
            if (continentalness < -0.3f)
            {
                return GetOceanBiome(temperature, continentalness);
            }

            // 河流判定
            if (IsRiver(x, z))
            {
                return temperature < 0.3f ? BiomeType.FrozenRiver : BiomeType.River;
            }

            // 海滩判定
            if (continentalness < -0.1f && continentalness > -0.3f)
            {
                return GetBeachBiome(temperature, erosion);
            }

            // 陆地生物群系
            return GetLandBiome(temperature, humidity, erosion, weirdness);
        }

        // ========================================
        // 温度
        // ========================================
        public float GetTemperature(int x, int z)
        {
            float baseTemp = noise.FBM2D(x * TemperatureScale + 1000, z * TemperatureScale + 1000);
            float latitude = Math.Abs(z) * 0.00005f;
            float temp = 0.5f + baseTemp * 0.5f - latitude;
            return Math.Clamp(temp, 0f, 1f);
        }

        // ========================================
        // 湿度
        // ========================================
        public float GetHumidity(int x, int z)
        {
            float humidity = noise.FBM2D(x * HumidityScale + 2000, z * HumidityScale + 2000);
            return Math.Clamp(0.5f + humidity * 0.5f, 0f, 1f);
        }

        // ========================================
        // 海洋生物群系
        // ========================================
        private BiomeType GetOceanBiome(float temperature, float continentalness)
        {
            bool isDeep = continentalness < -0.5f;

            if (temperature < 0.2f)
            {
                return isDeep ? BiomeType.DeepFrozenOcean : BiomeType.FrozenOcean;
            }
            else if (temperature < 0.4f)
            {
                return isDeep ? BiomeType.DeepColdOcean : BiomeType.ColdOcean;
            }
            else if (temperature < 0.7f)
            {
                return isDeep ? BiomeType.DeepLukewarmOcean : BiomeType.LukewarmOcean;
            }
            else
            {
                return isDeep ? BiomeType.DeepWarmOcean : BiomeType.WarmOcean;
            }
        }

        // ========================================
        // 海滩生物群系
        // ========================================
        private BiomeType GetBeachBiome(float temperature, float erosion)
        {
            if (temperature < 0.3f)
            {
                return BiomeType.SnowyBeach;
            }
            else if (erosion > 0.5f)
            {
                return BiomeType.StoneShore;
            }
            else
            {
                return BiomeType.Beach;
            }
        }

        // ========================================
        // 陆地生物群系
        // ========================================
        private BiomeType GetLandBiome(float temperature, float humidity, float erosion, float weirdness)
        {
            // 雪地
            if (temperature < 0.2f)
            {
                if (weirdness > 0.7f) return BiomeType.IceSpikes;
                if (humidity > 0.5f) return BiomeType.SnowyTaiga;
                return BiomeType.SnowyTundra;
            }

            // 寒冷
            if (temperature < 0.4f)
            {
                if (humidity > 0.6f)
                {
                    if (weirdness > 0.5f) return BiomeType.GiantSpruceTaiga;
                    return BiomeType.Taiga;
                }
                if (erosion > 0.5f) return BiomeType.Mountains;
                return BiomeType.Plains;
            }

            // 温带
            if (temperature < 0.6f)
            {
                if (humidity > 0.7f)
                {
                    if (weirdness > 0.6f) return BiomeType.DarkForest;
                    if (humidity > 0.85f) return BiomeType.BirchForest;
                    return BiomeType.Forest;
                }
                if (humidity < 0.3f)
                {
                    if (erosion > 0.4f) return BiomeType.Mountains;
                    return BiomeType.Plains;
                }
                if (weirdness > 0.5f && erosion > 0.3f) return BiomeType.Mountains;
                return BiomeType.Plains;
            }

            // 温暖
            if (temperature < 0.8f)
            {
                if (humidity > 0.8f)
                {
                    if (weirdness > 0.5f) return BiomeType.BambooJungle;
                    return BiomeType.Jungle;
                }
                if (humidity > 0.5f)
                {
                    if (weirdness > 0.6f) return BiomeType.Swamp;
                    return BiomeType.Forest;
                }
                if (humidity < 0.3f)
                {
                    return BiomeType.Savanna;
                }
                return BiomeType.Plains;
            }

            // 炎热
            if (humidity > 0.7f)
            {
                return BiomeType.Jungle;
            }
            if (humidity < 0.2f)
            {
                if (weirdness > 0.5f) return BiomeType.Badlands;
                return BiomeType.Desert;
            }
            if (humidity < 0.4f)
            {
                return BiomeType.Savanna;
            }
            return BiomeType.Plains;
        }

        // ========================================
        // 河流检测
        // ========================================
        private bool IsRiver(int x, int z)
        {
            float riverNoise = noise.Perlin2D(x * 0.005f + 3000, z * 0.005f + 3000);
            float riverNoise2 = noise.Perlin2D(x * 0.008f + 4000, z * 0.008f + 4000);
            return Math.Abs(riverNoise) < 0.05f && Math.Abs(riverNoise2) < 0.1f;
        }

        // ========================================
        // 生物群系信息
        // ========================================
        public BiomeInfo GetBiomeInfo(BiomeType biome)
        {
            return biome switch
            {
                BiomeType.Ocean => new BiomeInfo("海洋", 0.5f, 0.5f, 0x3f76e4),
                BiomeType.DeepOcean => new BiomeInfo("深海", 0.5f, 0.5f, 0x0000c0),
                BiomeType.Plains => new BiomeInfo("平原", 0.8f, 0.4f, 0x8db360),
                BiomeType.SunflowerPlains => new BiomeInfo("向日葵平原", 0.8f, 0.4f, 0x8db360),
                BiomeType.Desert => new BiomeInfo("沙漠", 2.0f, 0.0f, 0xf9e27b),
                BiomeType.DesertHills => new BiomeInfo("沙漠丘陵", 2.0f, 0.0f, 0xdac468),
                BiomeType.Mountains => new BiomeInfo("山脉", 0.2f, 0.3f, 0x6e6e6e),
                BiomeType.WoodedMountains => new BiomeInfo("林木繁茂的山脉", 0.2f, 0.3f, 0x5f703c),
                BiomeType.Forest => new BiomeInfo("森林", 0.7f, 0.8f, 0x567c35),
                BiomeType.FlowerForest => new BiomeInfo("繁花森林", 0.7f, 0.8f, 0x567c35),
                BiomeType.BirchForest => new BiomeInfo("白桦林", 0.6f, 0.8f, 0x6a8f3d),
                BiomeType.DarkForest => new BiomeInfo("黑森林", 0.7f, 0.9f, 0x3b511f),
                BiomeType.Taiga => new BiomeInfo("针叶林", 0.25f, 0.8f, 0x3b5d3b),
                BiomeType.SnowyTaiga => new BiomeInfo("积雪针叶林", -0.5f, 0.8f, 0x2e3f2e),
                BiomeType.SnowyTundra => new BiomeInfo("积雪冻原", -0.5f, 0.4f, 0x7f92a8),
                BiomeType.IceSpikes => new BiomeInfo("冰刺平原", -0.5f, 0.4f, 0xa5c6d6),
                BiomeType.Swamp => new BiomeInfo("沼泽", 0.8f, 0.9f, 0x4f7a3b),
                BiomeType.SwampHills => new BiomeInfo("沼泽丘陵", 0.8f, 0.9f, 0x476d35),
                BiomeType.Jungle => new BiomeInfo("丛林", 0.95f, 0.9f, 0x30bb0b),
                BiomeType.JungleHills => new BiomeInfo("丛林丘陵", 0.95f, 0.9f, 0x2a9a09),
                BiomeType.BambooJungle => new BiomeInfo("竹林", 0.95f, 0.9f, 0x30bb0b),
                BiomeType.Savanna => new BiomeInfo("热带草原", 1.2f, 0.0f, 0xbdb25f),
                BiomeType.SavannaPlateau => new BiomeInfo("热带高原", 1.2f, 0.0f, 0xa89d4e),
                BiomeType.Badlands => new BiomeInfo("恶地", 2.0f, 0.0f, 0xd94515),
                BiomeType.ErodedBadlands => new BiomeInfo("被风蚀的恶地", 2.0f, 0.0f, 0xc93515),
                BiomeType.MushroomFields => new BiomeInfo("蘑菇岛", 0.9f, 1.0f, 0x907070),
                BiomeType.Beach => new BiomeInfo("海滩", 0.8f, 0.4f, 0xf9e27b),
                BiomeType.River => new BiomeInfo("河流", 0.5f, 0.5f, 0x3f76e4),
                BiomeType.FrozenRiver => new BiomeInfo("冻河", -0.5f, 0.5f, 0x7fa8e4),
                BiomeType.NetherWastes => new BiomeInfo("下界荒地", 2.0f, 0.0f, 0x8b0000),
                BiomeType.SoulSandValley => new BiomeInfo("灵魂沙峡谷", 2.0f, 0.0f, 0x4a3b5c),
                BiomeType.CrimsonForest => new BiomeInfo("绯红森林", 2.0f, 0.0f, 0x8b0000),
                BiomeType.WarpedForest => new BiomeInfo("诡异森林", 2.0f, 0.0f, 0x006400),
                BiomeType.TheEnd => new BiomeInfo("末地", 0.5f, 0.5f, 0x808080),
                _ => new BiomeInfo("未知", 0.5f, 0.5f, 0x808080)
            };
        }

        // ========================================
        // 生物群系颜色
        // ========================================
        public Vector3 GetGrassColor(BiomeType biome)
        {
            BiomeInfo info = GetBiomeInfo(biome);
            float temp = info.Temperature;
            float humidity = info.Humidity;

            // 基于温度和湿度计算草颜色
            float r = 0.3f + temp * 0.2f;
            float g = 0.5f + humidity * 0.3f - temp * 0.1f;
            float b = 0.2f + humidity * 0.1f;

            // 特殊生物群系覆盖
            switch (biome)
            {
                case BiomeType.Swamp:
                case BiomeType.SwampHills:
                    r = 0.35f; g = 0.45f; b = 0.25f;
                    break;
                case BiomeType.DarkForest:
                    r = 0.25f; g = 0.35f; b = 0.15f;
                    break;
                case BiomeType.Desert:
                    r = 0.8f; g = 0.7f; b = 0.4f;
                    break;
                case BiomeType.SnowyTundra:
                case BiomeType.IceSpikes:
                    r = 0.6f; g = 0.7f; b = 0.7f;
                    break;
            }

            return new Vector3(
                Math.Clamp(r, 0f, 1f),
                Math.Clamp(g, 0f, 1f),
                Math.Clamp(b, 0f, 1f)
            );
        }

        public Vector3 GetWaterColor(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Swamp:
                case BiomeType.SwampHills:
                    return new Vector3(0.3f, 0.4f, 0.3f);
                case BiomeType.Ocean:
                case BiomeType.DeepOcean:
                    return new Vector3(0.2f, 0.4f, 0.7f);
                case BiomeType.WarmOcean:
                    return new Vector3(0.3f, 0.6f, 0.8f);
                case BiomeType.FrozenOcean:
                case BiomeType.FrozenRiver:
                    return new Vector3(0.5f, 0.7f, 0.9f);
                default:
                    return new Vector3(0.25f, 0.45f, 0.7f);
            }
        }

        public Vector3 GetFogColor(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.NetherWastes:
                case BiomeType.CrimsonForest:
                    return new Vector3(0.3f, 0.1f, 0.1f);
                case BiomeType.SoulSandValley:
                    return new Vector3(0.2f, 0.15f, 0.25f);
                case BiomeType.WarpedForest:
                    return new Vector3(0.1f, 0.2f, 0.15f);
                case BiomeType.TheEnd:
                    return new Vector3(0.5f, 0.5f, 0.6f);
                case BiomeType.SnowyTundra:
                case BiomeType.IceSpikes:
                    return new Vector3(0.8f, 0.85f, 0.9f);
                case BiomeType.Desert:
                    return new Vector3(0.9f, 0.8f, 0.6f);
                default:
                    return new Vector3(0.7f, 0.8f, 0.9f);
            }
        }

        public Vector3 GetSkyColor(BiomeType biome)
        {
            BiomeInfo info = GetBiomeInfo(biome);
            float temp = info.Temperature;

            float r = 0.5f + temp * 0.1f;
            float g = 0.7f + temp * 0.05f;
            float b = 0.9f;

            switch (biome)
            {
                case BiomeType.NetherWastes:
                case BiomeType.CrimsonForest:
                    r = 0.2f; g = 0.05f; b = 0.05f;
                    break;
                case BiomeType.TheEnd:
                    r = 0.3f; g = 0.3f; b = 0.4f;
                    break;
            }

            return new Vector3(
                Math.Clamp(r, 0f, 1f),
                Math.Clamp(g, 0f, 1f),
                Math.Clamp(b, 0f, 1f)
            );
        }

        // ========================================
        // 生物群系生成结构
        // ========================================
        public bool CanGenerateVillage(BiomeType biome)
        {
            return biome == BiomeType.Plains ||
                   biome == BiomeType.Savanna ||
                   biome == BiomeType.Desert ||
                   biome == BiomeType.Taiga ||
                   biome == BiomeType.SnowyTundra;
        }

        public bool CanGenerateTemple(BiomeType biome)
        {
            return biome == BiomeType.Desert ||
                   biome == BiomeType.Jungle ||
                   biome == BiomeType.Swamp;
        }

        public bool CanGenerateMineShaft(BiomeType biome)
        {
            return true; // 所有生物群系都可以
        }

        public bool CanGenerateStronghold(BiomeType biome)
        {
            return biome != BiomeType.Ocean &&
                   biome != BiomeType.DeepOcean;
        }

        // ========================================
        // 生物群系降水
        // ========================================
        public bool HasRain(BiomeType biome)
        {
            BiomeInfo info = GetBiomeInfo(biome);
            return info.Temperature > 0.15f && info.Humidity > 0.3f;
        }

        public bool HasSnow(BiomeType biome)
        {
            BiomeInfo info = GetBiomeInfo(biome);
            return info.Temperature <= 0.15f;
        }

        public float GetRainfall(BiomeType biome)
        {
            BiomeInfo info = GetBiomeInfo(biome);
            return info.Humidity;
        }
    }

    // ========================================
    // 生物群系信息结构
    // ========================================
    public struct BiomeInfo
    {
        public string Name;
        public float Temperature;
        public float Humidity;
        public int Color;

        public BiomeInfo(string name, float temperature, float humidity, int color)
        {
            Name = name;
            Temperature = temperature;
            Humidity = humidity;
            Color = color;
        }
    }
}
