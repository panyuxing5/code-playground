using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class DimensionSystem
    {
        private readonly Dictionary<DimensionType, Dimension> dimensions;
        private DimensionType currentDimension;

        // 事件
        public event Action<DimensionType, DimensionType> OnDimensionChanged;

        public DimensionType CurrentDimension => currentDimension;
        public int DimensionCount => dimensions.Count;

        public DimensionSystem()
        {
            dimensions = new Dictionary<DimensionType, Dimension>();
            currentDimension = DimensionType.Overworld;
        }

        public void Initialize()
        {
            Console.WriteLine("[DimensionSystem] 维度系统初始化完成");
            RegisterDefaultDimensions();
        }

        private void RegisterDefaultDimensions()
        {
            // 主世界
            dimensions[DimensionType.Overworld] = new Dimension
            {
                Type = DimensionType.Overworld,
                Name = "主世界",
                WorldHeight = 384,
                MinY = -64,
                MaxY = 320,
                SeaLevel = 63,
                HasSky = true,
                HasCeiling = false,
                HasWeather = true,
                HasDayNightCycle = true,
                Gravity = 0.08f,
                CloudHeight = 192,
                SkyColor = new Vector3(0.5f, 0.7f, 0.95f),
                FogColor = new Vector3(0.6f, 0.8f, 0.95f),
                WaterColor = new Vector3(0.2f, 0.4f, 0.8f),
                AmbientLight = 0.0f,
                TimeScale = 1.0f,
                IsNether = false,
                IsEnd = false,
                IsOverworld = true,
                SupportedBiomes = new List<Biome>
                {
                    Biome.Ocean, Biome.Plains, Biome.Desert, Biome.Mountains,
                    Biome.Forest, Biome.Taiga, Biome.Swamp, Biome.River,
                    Biome.FrozenOcean, Biome.SnowyTundra, Biome.MushroomFields,
                    Biome.Beach, Biome.Jungle, Biome.DeepOcean, Biome.BirchForest,
                    Biome.DarkForest, Biome.Savanna, Biome.Badlands,
                    Biome.DripstoneCaves, Biome.LushCaves, Biome.DeepDark,
                    Biome.Meadow, Biome.Grove, Biome.SnowySlopes,
                    Biome.JaggedPeaks, Biome.FrozenPeaks, Biome.StonyPeaks,
                    Biome.MangroveSwamp
                }
            };

            // 下界
            dimensions[DimensionType.Nether] = new Dimension
            {
                Type = DimensionType.Nether,
                Name = "下界",
                WorldHeight = 256,
                MinY = 0,
                MaxY = 256,
                SeaLevel = 32,
                HasSky = false,
                HasCeiling = true,
                HasWeather = false,
                HasDayNightCycle = false,
                Gravity = 0.08f,
                CloudHeight = 0,
                SkyColor = new Vector3(0.3f, 0.1f, 0.1f),
                FogColor = new Vector3(0.4f, 0.15f, 0.15f),
                WaterColor = new Vector3(0.3f, 0.2f, 0.2f),
                AmbientLight = 0.1f,
                TimeScale = 1.0f,
                IsNether = true,
                IsEnd = false,
                IsOverworld = false,
                SupportedBiomes = new List<Biome>
                {
                    Biome.NetherWastes, Biome.SoulSandValley,
                    Biome.CrimsonForest, Biome.WarpedForest,
                    Biome.BasaltDeltas
                }
            };

            // 末地
            dimensions[DimensionType.TheEnd] = new Dimension
            {
                Type = DimensionType.TheEnd,
                Name = "末地",
                WorldHeight = 256,
                MinY = 0,
                MaxY = 256,
                SeaLevel = 0,
                HasSky = true,
                HasCeiling = false,
                HasWeather = false,
                HasDayNightCycle = false,
                Gravity = 0.08f,
                CloudHeight = 0,
                SkyColor = new Vector3(0.1f, 0.05f, 0.15f),
                FogColor = new Vector3(0.2f, 0.1f, 0.25f),
                WaterColor = new Vector3(0.4f, 0.35f, 0.4f),
                AmbientLight = 0.2f,
                TimeScale = 1.0f,
                IsNether = false,
                IsEnd = true,
                IsOverworld = false,
                SupportedBiomes = new List<Biome>
                {
                    Biome.TheEnd, Biome.SmallEndIslands,
                    Biome.EndMidlands, Biome.EndHighlands,
                    Biome.EndBarrens
                }
            };

            Console.WriteLine($"[DimensionSystem] 已注册 {dimensions.Count} 个维度");
        }

        public void RegisterDimension(Dimension dimension)
        {
            if (!dimensions.ContainsKey(dimension.Type))
            {
                dimensions[dimension.Type] = dimension;
                Console.WriteLine($"[DimensionSystem] 已注册维度: {dimension.Name}");
            }
        }

        public void UnregisterDimension(DimensionType type)
        {
            if (type != DimensionType.Overworld && type != DimensionType.Nether && type != DimensionType.TheEnd)
            {
                dimensions.Remove(type);
            }
        }

        public Dimension GetDimension(DimensionType type)
        {
            if (dimensions.TryGetValue(type, out Dimension dimension))
            {
                return dimension;
            }
            return null;
        }

        public Dimension GetCurrentDimension()
        {
            return dimensions[currentDimension];
        }

        public void ChangeDimension(DimensionType newDimension)
        {
            if (!dimensions.ContainsKey(newDimension))
            {
                Console.WriteLine($"[DimensionSystem] 未知维度: {newDimension}");
                return;
            }

            DimensionType oldDimension = currentDimension;
            currentDimension = newDimension;

            Console.WriteLine($"[DimensionSystem] 维度变化: {oldDimension} -> {newDimension}");
            OnDimensionChanged?.Invoke(oldDimension, newDimension);
        }

        public Vector3 GetPortalTargetPosition(DimensionType from, DimensionType to, Vector3 fromPosition)
        {
            // 计算传送门目标位置
            // 主世界 <-> 下界：坐标比例 8:1
            if (from == DimensionType.Overworld && to == DimensionType.Nether)
            {
                return new Vector3(
                    fromPosition.X / 8.0f,
                    Math.Clamp(fromPosition.Y, 0, 128),
                    fromPosition.Z / 8.0f
                );
            }
            else if (from == DimensionType.Nether && to == DimensionType.Overworld)
            {
                return new Vector3(
                    fromPosition.X * 8.0f,
                    Math.Clamp(fromPosition.Y, 0, 256),
                    fromPosition.Z * 8.0f
                );
            }
            // 主世界 <-> 末地：固定位置
            else if (to == DimensionType.TheEnd)
            {
                return new Vector3(0, 64, 0);
            }
            else if (from == DimensionType.TheEnd)
            {
                return new Vector3(0, 64, 0);
            }

            return fromPosition;
        }

        public bool CanCreatePortal(DimensionType dimension)
        {
            switch (dimension)
            {
                case DimensionType.Overworld:
                case DimensionType.Nether:
                    return true;

                case DimensionType.TheEnd:
                    return false; // 末地需要末地传送门框架

                default:
                    return false;
            }
        }

        public bool IsValidPortalBlock(ushort blockId, DimensionType dimension)
        {
            switch (dimension)
            {
                case DimensionType.Overworld:
                case DimensionType.Nether:
                    return blockId == GameConstants.BLOCK_OBSIDIAN;

                case DimensionType.TheEnd:
                    return blockId == GameConstants.BLOCK_END_PORTAL_FRAME;

                default:
                    return false;
            }
        }

        public int GetMinY(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.MinY;
            }
            return 0;
        }

        public int GetMaxY(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.MaxY;
            }
            return 256;
        }

        public int GetWorldHeight(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.WorldHeight;
            }
            return 256;
        }

        public int GetSeaLevel(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.SeaLevel;
            }
            return 64;
        }

        public bool HasSky(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.HasSky;
            }
            return true;
        }

        public bool HasCeiling(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.HasCeiling;
            }
            return false;
        }

        public bool HasWeather(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.HasWeather;
            }
            return false;
        }

        public bool HasDayNightCycle(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.HasDayNightCycle;
            }
            return true;
        }

        public float GetGravity(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.Gravity;
            }
            return 0.08f;
        }

        public Vector3 GetSkyColor(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.SkyColor;
            }
            return new Vector3(0.5f, 0.7f, 0.95f);
        }

        public Vector3 GetFogColor(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.FogColor;
            }
            return new Vector3(0.6f, 0.8f, 0.95f);
        }

        public Vector3 GetWaterColor(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.WaterColor;
            }
            return new Vector3(0.2f, 0.4f, 0.8f);
        }

        public float GetAmbientLight(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.AmbientLight;
            }
            return 0.0f;
        }

        public bool IsBiomeSupported(DimensionType dimension, Biome biome)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.SupportedBiomes.Contains(biome);
            }
            return false;
        }

        public List<Biome> GetSupportedBiomes(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return new List<Biome>(dim.SupportedBiomes);
            }
            return new List<Biome>();
        }

        public List<DimensionType> GetAllDimensionTypes()
        {
            return new List<DimensionType>(dimensions.Keys);
        }

        public string GetDimensionName(DimensionType dimension)
        {
            if (dimensions.TryGetValue(dimension, out Dimension dim))
            {
                return dim.Name;
            }
            return dimension.ToString();
        }
    }

    public class Dimension
    {
        public DimensionType Type;
        public string Name;
        public int WorldHeight;
        public int MinY;
        public int MaxY;
        public int SeaLevel;
        public bool HasSky;
        public bool HasCeiling;
        public bool HasWeather;
        public bool HasDayNightCycle;
        public float Gravity;
        public int CloudHeight;
        public Vector3 SkyColor;
        public Vector3 FogColor;
        public Vector3 WaterColor;
        public float AmbientLight;
        public float TimeScale;
        public bool IsNether;
        public bool IsEnd;
        public bool IsOverworld;
        public List<Biome> SupportedBiomes;

        public Dimension()
        {
            SupportedBiomes = new List<Biome>();
        }
    }

    public enum DimensionType
    {
        Overworld,
        Nether,
        TheEnd,
        Custom1,
        Custom2,
        Custom3
    }
}
