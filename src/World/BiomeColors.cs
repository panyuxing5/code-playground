using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class BiomeColors
    {
        private static readonly Dictionary<Biome, BiomeColorData> biomeColors = new Dictionary<Biome, BiomeColorData>();
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;
            isInitialized = true;

            RegisterAllBiomeColors();
            Console.WriteLine("[BiomeColors] 生物群系颜色系统初始化完成");
        }

        private static void RegisterAllBiomeColors()
        {
            // 海洋
            biomeColors[Biome.Ocean] = new BiomeColorData
            {
                GrassColor = new Vector3(0.4f, 0.7f, 0.4f),
                FoliageColor = new Vector3(0.35f, 0.65f, 0.35f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.5f, 0.7f, 0.95f),
                FogColor = new Vector3(0.6f, 0.8f, 0.95f),
                Temperature = 0.5f,
                Humidity = 0.5f
            };

            // 平原
            biomeColors[Biome.Plains] = new BiomeColorData
            {
                GrassColor = new Vector3(0.55f, 0.75f, 0.35f),
                FoliageColor = new Vector3(0.5f, 0.7f, 0.3f),
                WaterColor = new Vector3(0.25f, 0.45f, 0.85f),
                WaterFogColor = new Vector3(0.2f, 0.35f, 0.65f),
                SkyColor = new Vector3(0.55f, 0.75f, 0.95f),
                FogColor = new Vector3(0.65f, 0.85f, 0.95f),
                Temperature = 0.8f,
                Humidity = 0.4f
            };

            // 沙漠
            biomeColors[Biome.Desert] = new BiomeColorData
            {
                GrassColor = new Vector3(0.85f, 0.8f, 0.5f),
                FoliageColor = new Vector3(0.8f, 0.75f, 0.45f),
                WaterColor = new Vector3(0.3f, 0.5f, 0.85f),
                WaterFogColor = new Vector3(0.25f, 0.4f, 0.65f),
                SkyColor = new Vector3(0.6f, 0.8f, 0.95f),
                FogColor = new Vector3(0.7f, 0.85f, 0.9f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 山地
            biomeColors[Biome.Mountains] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.65f, 0.45f),
                FoliageColor = new Vector3(0.45f, 0.6f, 0.4f),
                WaterColor = new Vector3(0.2f, 0.35f, 0.75f),
                WaterFogColor = new Vector3(0.15f, 0.25f, 0.55f),
                SkyColor = new Vector3(0.5f, 0.65f, 0.9f),
                FogColor = new Vector3(0.6f, 0.75f, 0.9f),
                Temperature = 0.2f,
                Humidity = 0.3f
            };

            // 森林
            biomeColors[Biome.Forest] = new BiomeColorData
            {
                GrassColor = new Vector3(0.4f, 0.7f, 0.35f),
                FoliageColor = new Vector3(0.35f, 0.65f, 0.3f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.5f, 0.7f, 0.95f),
                FogColor = new Vector3(0.6f, 0.8f, 0.95f),
                Temperature = 0.7f,
                Humidity = 0.8f
            };

            // 针叶林
            biomeColors[Biome.Taiga] = new BiomeColorData
            {
                GrassColor = new Vector3(0.45f, 0.65f, 0.45f),
                FoliageColor = new Vector3(0.4f, 0.6f, 0.4f),
                WaterColor = new Vector3(0.2f, 0.35f, 0.75f),
                WaterFogColor = new Vector3(0.15f, 0.25f, 0.55f),
                SkyColor = new Vector3(0.5f, 0.65f, 0.9f),
                FogColor = new Vector3(0.6f, 0.75f, 0.9f),
                Temperature = 0.25f,
                Humidity = 0.8f
            };

            // 沼泽
            biomeColors[Biome.Swamp] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.65f, 0.4f),
                FoliageColor = new Vector3(0.45f, 0.6f, 0.35f),
                WaterColor = new Vector3(0.35f, 0.45f, 0.35f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.25f),
                SkyColor = new Vector3(0.55f, 0.7f, 0.85f),
                FogColor = new Vector3(0.65f, 0.75f, 0.8f),
                Temperature = 0.8f,
                Humidity = 0.9f
            };

            // 河流
            biomeColors[Biome.River] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.7f, 0.4f),
                FoliageColor = new Vector3(0.45f, 0.65f, 0.35f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.5f, 0.7f, 0.95f),
                FogColor = new Vector3(0.6f, 0.8f, 0.95f),
                Temperature = 0.5f,
                Humidity = 0.5f
            };

            // 下界荒地
            biomeColors[Biome.NetherWastes] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.3f, 0.3f),
                FoliageColor = new Vector3(0.45f, 0.25f, 0.25f),
                WaterColor = new Vector3(0.3f, 0.2f, 0.2f),
                WaterFogColor = new Vector3(0.2f, 0.1f, 0.1f),
                SkyColor = new Vector3(0.3f, 0.1f, 0.1f),
                FogColor = new Vector3(0.4f, 0.15f, 0.15f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 末地
            biomeColors[Biome.TheEnd] = new BiomeColorData
            {
                GrassColor = new Vector3(0.6f, 0.55f, 0.6f),
                FoliageColor = new Vector3(0.55f, 0.5f, 0.55f),
                WaterColor = new Vector3(0.4f, 0.35f, 0.4f),
                WaterFogColor = new Vector3(0.3f, 0.25f, 0.3f),
                SkyColor = new Vector3(0.1f, 0.05f, 0.15f),
                FogColor = new Vector3(0.2f, 0.1f, 0.25f),
                Temperature = 0.5f,
                Humidity = 0.5f
            };

            // 冻洋
            biomeColors[Biome.FrozenOcean] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.6f, 0.65f),
                FoliageColor = new Vector3(0.45f, 0.55f, 0.6f),
                WaterColor = new Vector3(0.3f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.6f, 0.75f, 0.9f),
                FogColor = new Vector3(0.7f, 0.85f, 0.95f),
                Temperature = 0.0f,
                Humidity = 0.5f
            };

            // 雪原
            biomeColors[Biome.SnowyTundra] = new BiomeColorData
            {
                GrassColor = new Vector3(0.6f, 0.7f, 0.75f),
                FoliageColor = new Vector3(0.55f, 0.65f, 0.7f),
                WaterColor = new Vector3(0.3f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.65f, 0.8f, 0.95f),
                FogColor = new Vector3(0.75f, 0.9f, 0.98f),
                Temperature = 0.0f,
                Humidity = 0.5f
            };

            // 蘑菇岛
            biomeColors[Biome.MushroomFields] = new BiomeColorData
            {
                GrassColor = new Vector3(0.7f, 0.5f, 0.7f),
                FoliageColor = new Vector3(0.65f, 0.45f, 0.65f),
                WaterColor = new Vector3(0.25f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.2f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.55f, 0.7f, 0.95f),
                FogColor = new Vector3(0.65f, 0.8f, 0.95f),
                Temperature = 0.9f,
                Humidity = 1.0f
            };

            // 海滩
            biomeColors[Biome.Beach] = new BiomeColorData
            {
                GrassColor = new Vector3(0.8f, 0.75f, 0.5f),
                FoliageColor = new Vector3(0.75f, 0.7f, 0.45f),
                WaterColor = new Vector3(0.25f, 0.45f, 0.85f),
                WaterFogColor = new Vector3(0.2f, 0.35f, 0.65f),
                SkyColor = new Vector3(0.55f, 0.75f, 0.95f),
                FogColor = new Vector3(0.65f, 0.85f, 0.95f),
                Temperature = 0.8f,
                Humidity = 0.4f
            };

            // 丛林
            biomeColors[Biome.Jungle] = new BiomeColorData
            {
                GrassColor = new Vector3(0.35f, 0.7f, 0.3f),
                FoliageColor = new Vector3(0.3f, 0.65f, 0.25f),
                WaterColor = new Vector3(0.2f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.15f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.5f, 0.75f, 0.9f),
                FogColor = new Vector3(0.6f, 0.85f, 0.9f),
                Temperature = 0.95f,
                Humidity = 0.9f
            };

            // 深海
            biomeColors[Biome.DeepOcean] = new BiomeColorData
            {
                GrassColor = new Vector3(0.35f, 0.6f, 0.35f),
                FoliageColor = new Vector3(0.3f, 0.55f, 0.3f),
                WaterColor = new Vector3(0.15f, 0.3f, 0.7f),
                WaterFogColor = new Vector3(0.1f, 0.2f, 0.5f),
                SkyColor = new Vector3(0.45f, 0.65f, 0.9f),
                FogColor = new Vector3(0.55f, 0.75f, 0.9f),
                Temperature = 0.5f,
                Humidity = 0.5f
            };

            // 白桦林
            biomeColors[Biome.BirchForest] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.75f, 0.4f),
                FoliageColor = new Vector3(0.6f, 0.8f, 0.4f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.55f, 0.75f, 0.95f),
                FogColor = new Vector3(0.65f, 0.85f, 0.95f),
                Temperature = 0.6f,
                Humidity = 0.6f
            };

            // 黑森林
            biomeColors[Biome.DarkForest] = new BiomeColorData
            {
                GrassColor = new Vector3(0.35f, 0.6f, 0.3f),
                FoliageColor = new Vector3(0.3f, 0.55f, 0.25f),
                WaterColor = new Vector3(0.2f, 0.35f, 0.75f),
                WaterFogColor = new Vector3(0.15f, 0.25f, 0.55f),
                SkyColor = new Vector3(0.45f, 0.65f, 0.85f),
                FogColor = new Vector3(0.55f, 0.75f, 0.85f),
                Temperature = 0.7f,
                Humidity = 0.8f
            };

            // 热带草原
            biomeColors[Biome.Savanna] = new BiomeColorData
            {
                GrassColor = new Vector3(0.7f, 0.7f, 0.4f),
                FoliageColor = new Vector3(0.65f, 0.65f, 0.35f),
                WaterColor = new Vector3(0.3f, 0.5f, 0.8f),
                WaterFogColor = new Vector3(0.25f, 0.4f, 0.6f),
                SkyColor = new Vector3(0.6f, 0.8f, 0.95f),
                FogColor = new Vector3(0.7f, 0.85f, 0.95f),
                Temperature = 1.2f,
                Humidity = 0.0f
            };

            // 恶地
            biomeColors[Biome.Badlands] = new BiomeColorData
            {
                GrassColor = new Vector3(0.8f, 0.6f, 0.4f),
                FoliageColor = new Vector3(0.75f, 0.55f, 0.35f),
                WaterColor = new Vector3(0.3f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.6f, 0.75f, 0.9f),
                FogColor = new Vector3(0.7f, 0.8f, 0.9f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 灵魂沙峡谷
            biomeColors[Biome.SoulSandValley] = new BiomeColorData
            {
                GrassColor = new Vector3(0.4f, 0.3f, 0.4f),
                FoliageColor = new Vector3(0.35f, 0.25f, 0.35f),
                WaterColor = new Vector3(0.25f, 0.2f, 0.3f),
                WaterFogColor = new Vector3(0.15f, 0.1f, 0.2f),
                SkyColor = new Vector3(0.2f, 0.15f, 0.25f),
                FogColor = new Vector3(0.3f, 0.2f, 0.35f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 绯红森林
            biomeColors[Biome.CrimsonForest] = new BiomeColorData
            {
                GrassColor = new Vector3(0.7f, 0.2f, 0.2f),
                FoliageColor = new Vector3(0.65f, 0.15f, 0.15f),
                WaterColor = new Vector3(0.4f, 0.15f, 0.15f),
                WaterFogColor = new Vector3(0.3f, 0.1f, 0.1f),
                SkyColor = new Vector3(0.5f, 0.1f, 0.1f),
                FogColor = new Vector3(0.6f, 0.15f, 0.15f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 诡异森林
            biomeColors[Biome.WarpedForest] = new BiomeColorData
            {
                GrassColor = new Vector3(0.2f, 0.6f, 0.6f),
                FoliageColor = new Vector3(0.15f, 0.55f, 0.55f),
                WaterColor = new Vector3(0.15f, 0.4f, 0.4f),
                WaterFogColor = new Vector3(0.1f, 0.3f, 0.3f),
                SkyColor = new Vector3(0.1f, 0.4f, 0.4f),
                FogColor = new Vector3(0.15f, 0.5f, 0.5f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 玄武岩三角洲
            biomeColors[Biome.BasaltDeltas] = new BiomeColorData
            {
                GrassColor = new Vector3(0.4f, 0.35f, 0.35f),
                FoliageColor = new Vector3(0.35f, 0.3f, 0.3f),
                WaterColor = new Vector3(0.25f, 0.2f, 0.2f),
                WaterFogColor = new Vector3(0.15f, 0.1f, 0.1f),
                SkyColor = new Vector3(0.3f, 0.2f, 0.2f),
                FogColor = new Vector3(0.4f, 0.25f, 0.25f),
                Temperature = 2.0f,
                Humidity = 0.0f
            };

            // 溶洞
            biomeColors[Biome.DripstoneCaves] = new BiomeColorData
            {
                GrassColor = new Vector3(0.45f, 0.55f, 0.45f),
                FoliageColor = new Vector3(0.4f, 0.5f, 0.4f),
                WaterColor = new Vector3(0.2f, 0.35f, 0.7f),
                WaterFogColor = new Vector3(0.15f, 0.25f, 0.5f),
                SkyColor = new Vector3(0.4f, 0.55f, 0.8f),
                FogColor = new Vector3(0.5f, 0.65f, 0.8f),
                Temperature = 0.6f,
                Humidity = 0.7f
            };

            // 繁茂洞穴
            biomeColors[Biome.LushCaves] = new BiomeColorData
            {
                GrassColor = new Vector3(0.4f, 0.7f, 0.4f),
                FoliageColor = new Vector3(0.35f, 0.65f, 0.35f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.75f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.55f),
                SkyColor = new Vector3(0.45f, 0.65f, 0.85f),
                FogColor = new Vector3(0.55f, 0.75f, 0.85f),
                Temperature = 0.6f,
                Humidity = 0.9f
            };

            // 深暗之域
            biomeColors[Biome.DeepDark] = new BiomeColorData
            {
                GrassColor = new Vector3(0.15f, 0.15f, 0.2f),
                FoliageColor = new Vector3(0.1f, 0.1f, 0.15f),
                WaterColor = new Vector3(0.1f, 0.1f, 0.2f),
                WaterFogColor = new Vector3(0.05f, 0.05f, 0.1f),
                SkyColor = new Vector3(0.05f, 0.05f, 0.1f),
                FogColor = new Vector3(0.1f, 0.1f, 0.15f),
                Temperature = 0.3f,
                Humidity = 0.4f
            };

            // 草甸
            biomeColors[Biome.Meadow] = new BiomeColorData
            {
                GrassColor = new Vector3(0.55f, 0.75f, 0.45f),
                FoliageColor = new Vector3(0.5f, 0.7f, 0.4f),
                WaterColor = new Vector3(0.25f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.2f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.55f, 0.75f, 0.95f),
                FogColor = new Vector3(0.65f, 0.85f, 0.95f),
                Temperature = 0.5f,
                Humidity = 0.6f
            };

            // 雪林
            biomeColors[Biome.Grove] = new BiomeColorData
            {
                GrassColor = new Vector3(0.55f, 0.65f, 0.7f),
                FoliageColor = new Vector3(0.5f, 0.6f, 0.65f),
                WaterColor = new Vector3(0.3f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.6f, 0.75f, 0.9f),
                FogColor = new Vector3(0.7f, 0.85f, 0.95f),
                Temperature = -0.2f,
                Humidity = 0.8f
            };

            // 积雪山坡
            biomeColors[Biome.SnowySlopes] = new BiomeColorData
            {
                GrassColor = new Vector3(0.6f, 0.7f, 0.75f),
                FoliageColor = new Vector3(0.55f, 0.65f, 0.7f),
                WaterColor = new Vector3(0.3f, 0.45f, 0.75f),
                WaterFogColor = new Vector3(0.25f, 0.35f, 0.55f),
                SkyColor = new Vector3(0.65f, 0.8f, 0.95f),
                FogColor = new Vector3(0.75f, 0.9f, 0.98f),
                Temperature = -0.3f,
                Humidity = 0.7f
            };

            // 尖峭山峰
            biomeColors[Biome.JaggedPeaks] = new BiomeColorData
            {
                GrassColor = new Vector3(0.65f, 0.7f, 0.75f),
                FoliageColor = new Vector3(0.6f, 0.65f, 0.7f),
                WaterColor = new Vector3(0.35f, 0.5f, 0.75f),
                WaterFogColor = new Vector3(0.3f, 0.4f, 0.55f),
                SkyColor = new Vector3(0.7f, 0.85f, 0.95f),
                FogColor = new Vector3(0.8f, 0.92f, 0.98f),
                Temperature = -0.7f,
                Humidity = 0.6f
            };

            // 冰封山峰
            biomeColors[Biome.FrozenPeaks] = new BiomeColorData
            {
                GrassColor = new Vector3(0.7f, 0.75f, 0.8f),
                FoliageColor = new Vector3(0.65f, 0.7f, 0.75f),
                WaterColor = new Vector3(0.4f, 0.55f, 0.75f),
                WaterFogColor = new Vector3(0.35f, 0.45f, 0.55f),
                SkyColor = new Vector3(0.75f, 0.88f, 0.95f),
                FogColor = new Vector3(0.85f, 0.95f, 0.98f),
                Temperature = -1.0f,
                Humidity = 0.5f
            };

            // 裸岩山峰
            biomeColors[Biome.StonyPeaks] = new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.55f, 0.55f),
                FoliageColor = new Vector3(0.45f, 0.5f, 0.5f),
                WaterColor = new Vector3(0.25f, 0.4f, 0.7f),
                WaterFogColor = new Vector3(0.2f, 0.3f, 0.5f),
                SkyColor = new Vector3(0.55f, 0.7f, 0.9f),
                FogColor = new Vector3(0.65f, 0.8f, 0.9f),
                Temperature = 0.3f,
                Humidity = 0.4f
            };

            // 红树林沼泽
            biomeColors[Biome.MangroveSwamp] = new BiomeColorData
            {
                GrassColor = new Vector3(0.45f, 0.65f, 0.35f),
                FoliageColor = new Vector3(0.4f, 0.6f, 0.3f),
                WaterColor = new Vector3(0.3f, 0.4f, 0.3f),
                WaterFogColor = new Vector3(0.2f, 0.3f, 0.2f),
                SkyColor = new Vector3(0.5f, 0.7f, 0.85f),
                FogColor = new Vector3(0.6f, 0.8f, 0.85f),
                Temperature = 0.8f,
                Humidity = 0.9f
            };
        }

        public static BiomeColorData GetBiomeColors(Biome biome)
        {
            if (biomeColors.TryGetValue(biome, out BiomeColorData colors))
            {
                return colors;
            }

            // 默认颜色
            return new BiomeColorData
            {
                GrassColor = new Vector3(0.5f, 0.7f, 0.4f),
                FoliageColor = new Vector3(0.45f, 0.65f, 0.35f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                WaterFogColor = new Vector3(0.15f, 0.3f, 0.6f),
                SkyColor = new Vector3(0.5f, 0.7f, 0.95f),
                FogColor = new Vector3(0.6f, 0.8f, 0.95f),
                Temperature = 0.5f,
                Humidity = 0.5f
            };
        }

        public static Vector3 GetGrassColor(Biome biome)
        {
            return GetBiomeColors(biome).GrassColor;
        }

        public static Vector3 GetFoliageColor(Biome biome)
        {
            return GetBiomeColors(biome).FoliageColor;
        }

        public static Vector3 GetWaterColor(Biome biome)
        {
            return GetBiomeColors(biome).WaterColor;
        }

        public static Vector3 GetWaterFogColor(Biome biome)
        {
            return GetBiomeColors(biome).WaterFogColor;
        }

        public static Vector3 GetSkyColor(Biome biome)
        {
            return GetBiomeColors(biome).SkyColor;
        }

        public static Vector3 GetFogColor(Biome biome)
        {
            return GetBiomeColors(biome).FogColor;
        }

        public static float GetTemperature(Biome biome)
        {
            return GetBiomeColors(biome).Temperature;
        }

        public static float GetHumidity(Biome biome)
        {
            return GetBiomeColors(biome).Humidity;
        }

        public static Vector3 GetInterpolatedGrassColor(Biome biome, int x, int z)
        {
            // 基于位置的颜色插值（模拟Minecraft的颜色图）
            Vector3 baseColor = GetGrassColor(biome);

            // 添加一些基于位置的变化
            float noise = (float)(Math.Sin(x * 0.01) * Math.Cos(z * 0.01) * 0.05f);
            return new Vector3(
                Math.Clamp(baseColor.X + noise, 0, 1),
                Math.Clamp(baseColor.Y + noise, 0, 1),
                Math.Clamp(baseColor.Z + noise, 0, 1)
            );
        }

        public static Vector3 GetInterpolatedFoliageColor(Biome biome, int x, int z)
        {
            Vector3 baseColor = GetFoliageColor(biome);

            float noise = (float)(Math.Sin(x * 0.015) * Math.Cos(z * 0.015) * 0.05f);
            return new Vector3(
                Math.Clamp(baseColor.X + noise, 0, 1),
                Math.Clamp(baseColor.Y + noise, 0, 1),
                Math.Clamp(baseColor.Z + noise, 0, 1)
            );
        }

        public static Vector3 GetInterpolatedWaterColor(Biome biome, int x, int z, int y)
        {
            Vector3 baseColor = GetWaterColor(biome);

            // 深度影响颜色
            float depthFactor = Math.Clamp((64 - y) / 64.0f, 0, 0.3f);
            return new Vector3(
                Math.Clamp(baseColor.X - depthFactor * 0.5f, 0, 1),
                Math.Clamp(baseColor.Y - depthFactor * 0.5f, 0, 1),
                Math.Clamp(baseColor.Z - depthFactor * 0.3f, 0, 1)
            );
        }

        public static bool IsSnowy(Biome biome)
        {
            float temp = GetTemperature(biome);
            return temp < 0.15f;
        }

        public static bool IsCold(Biome biome)
        {
            float temp = GetTemperature(biome);
            return temp < 0.5f;
        }

        public static bool IsWarm(Biome biome)
        {
            float temp = GetTemperature(biome);
            return temp > 1.0f;
        }

        public static bool IsDry(Biome biome)
        {
            float humidity = GetHumidity(biome);
            return humidity < 0.3f;
        }

        public static bool IsWet(Biome biome)
        {
            float humidity = GetHumidity(biome);
            return humidity > 0.7f;
        }
    }

    public struct BiomeColorData
    {
        public Vector3 GrassColor;
        public Vector3 FoliageColor;
        public Vector3 WaterColor;
        public Vector3 WaterFogColor;
        public Vector3 SkyColor;
        public Vector3 FogColor;
        public float Temperature;
        public float Humidity;
    }
}
