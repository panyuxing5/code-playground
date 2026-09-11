using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Render;

namespace VoxelCraft.World
{
    public class WeatherSystem
    {
        private readonly WorldManager world;
        private readonly ParticleRenderer particleRenderer;

        // 天气状态
        public WeatherType CurrentWeather { get; private set; }
        public float WeatherIntensity { get; private set; }
        public int WeatherDuration { get; private set; }
        public int WeatherTimer { get; private set; }

        // 雷暴
        public bool IsThundering { get; private set; }
        public int ThunderTimer { get; private set; }
        public int NextThunder { get; private set; }

        // 粒子
        private int rainParticleCount;
        private int snowParticleCount;

        // 随机
        private readonly Random random;

        // 生物群系降水
        private Dictionary<BiomeType, bool> biomeRain;
        private Dictionary<BiomeType, bool> biomeSnow;

        public WeatherSystem(WorldManager world, ParticleRenderer particleRenderer)
        {
            this.world = world;
            this.particleRenderer = particleRenderer;
            random = new Random();
            CurrentWeather = WeatherType.Clear;
            WeatherIntensity = 0;
            InitializeBiomePrecipitation();
        }

        public void Initialize()
        {
            Console.WriteLine("[WeatherSystem] 天气系统初始化完成");
        }

        private void InitializeBiomePrecipitation()
        {
            biomeRain = new Dictionary<BiomeType, bool>();
            biomeSnow = new Dictionary<BiomeType, bool>();

            // 默认所有生物群系都有降水
            foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
            {
                biomeRain[biome] = true;
                biomeSnow[biome] = false;
            }

            // 沙漠不下雨
            biomeRain[BiomeType.Desert] = false;
            biomeRain[BiomeType.DesertHills] = false;
            biomeRain[BiomeType.DesertLakes] = false;
            biomeRain[BiomeType.Badlands] = false;
            biomeRain[BiomeType.BadlandsPlateau] = false;
            biomeRain[BiomeType.ErodedBadlands] = false;

            // 雪地生物群系下雪
            biomeSnow[BiomeType.SnowyTundra] = true;
            biomeSnow[BiomeType.SnowyMountains] = true;
            biomeSnow[BiomeType.IceSpikes] = true;
            biomeSnow[BiomeType.SnowyTaiga] = true;
            biomeSnow[BiomeType.SnowyTaigaHills] = true;
            biomeSnow[BiomeType.SnowyTaigaMountains] = true;
            biomeSnow[BiomeType.SnowyBeach] = true;
            biomeSnow[BiomeType.FrozenOcean] = true;
            biomeSnow[BiomeType.DeepFrozenOcean] = true;
            biomeSnow[BiomeType.FrozenRiver] = true;
        }

        public void Update(float deltaTime, Vector3 playerPosition)
        {
            // 天气计时器
            WeatherTimer++;
            if (WeatherTimer >= WeatherDuration)
            {
                ChangeWeather();
            }

            // 雷暴
            if (IsThundering)
            {
                ThunderTimer++;
                if (ThunderTimer >= NextThunder)
                {
                    StrikeLightning(playerPosition);
                    ThunderTimer = 0;
                    NextThunder = random.Next(100, 500);
                }
            }

            // 更新天气强度
            UpdateWeatherIntensity(deltaTime);

            // 生成降水粒子
            if (CurrentWeather != WeatherType.Clear && WeatherIntensity > 0.1f)
            {
                SpawnPrecipitation(playerPosition);
            }
        }

        private void ChangeWeather()
        {
            WeatherTimer = 0;

            int r = random.Next(100);
            if (CurrentWeather == WeatherType.Clear)
            {
                if (r < 70)
                {
                    CurrentWeather = WeatherType.Rain;
                    WeatherDuration = random.Next(6000, 12000); // 5-10分钟
                }
                else
                {
                    CurrentWeather = WeatherType.Clear;
                    WeatherDuration = random.Next(12000, 24000); // 10-20分钟
                }
            }
            else
            {
                if (r < 80)
                {
                    CurrentWeather = WeatherType.Clear;
                    WeatherDuration = random.Next(12000, 24000);
                    IsThundering = false;
                }
                else
                {
                    // 继续下雨，可能变成雷暴
                    if (r < 90)
                    {
                        IsThundering = true;
                        NextThunder = random.Next(100, 500);
                    }
                    WeatherDuration = random.Next(6000, 12000);
                }
            }

            Console.WriteLine($"[WeatherSystem] 天气变化: {CurrentWeather}, 持续: {WeatherDuration} ticks");
        }

        private void UpdateWeatherIntensity(float deltaTime)
        {
            float targetIntensity = CurrentWeather == WeatherType.Clear ? 0f : 1f;
            WeatherIntensity += (targetIntensity - WeatherIntensity) * deltaTime * 0.5f;
        }

        private void SpawnPrecipitation(Vector3 playerPosition)
        {
            // 检查玩家所在生物群系是否有降水
            BiomeType biome = world.GetBiomeAt((int)playerPosition.X, (int)playerPosition.Z);

            bool hasRain = biomeRain.ContainsKey(biome) && biomeRain[biome];
            bool hasSnow = biomeSnow.ContainsKey(biome) && biomeSnow[biome];

            if (!hasRain && !hasSnow) return;

            int particleCount = (int)(50 * WeatherIntensity);

            if (hasSnow)
            {
                particleRenderer.AddSnowParticles(playerPosition, particleCount);
            }
            else if (hasRain)
            {
                particleRenderer.AddRainParticles(playerPosition, particleCount);
            }
        }

        private void StrikeLightning(Vector3 playerPosition)
        {
            // 在玩家附近随机位置生成闪电
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float distance = random.Next(20, 100);

            int x = (int)(playerPosition.X + Math.Cos(angle) * distance);
            int z = (int)(playerPosition.Z + Math.Sin(angle) * distance);

            // 找到地面
            for (int y = GameConstants.CHUNK_HEIGHT - 1; y > 0; y--)
            {
                ushort block = world.GetBlock(x, y, z);
                if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_WATER_STILL)
                {
                    // 闪电效果
                    world.CreateExplosion(new Vector3(x, y + 1, z), 0); // 0半径，只做效果

                    // 点燃方块
                    if (random.NextDouble() < 0.3f)
                    {
                        world.SetBlock(x, y + 1, z, GameConstants.BLOCK_FIRE);
                    }

                    // 伤害附近实体
                    // entityManager.DamageEntitiesInRange(new Vector3(x, y, z), 5, 5, "lightning");

                    Console.WriteLine($"[WeatherSystem] 闪电击中: ({x}, {y}, {z})");
                    break;
                }
            }
        }

        public void SetWeather(WeatherType weather, int duration = -1)
        {
            CurrentWeather = weather;
            WeatherTimer = 0;
            WeatherDuration = duration > 0 ? duration : random.Next(6000, 12000);

            if (weather == WeatherType.Clear)
            {
                IsThundering = false;
            }

            Console.WriteLine($"[WeatherSystem] 天气设置为: {weather}");
        }

        public void SetThundering(bool thundering)
        {
            IsThundering = thundering;
            if (thundering)
            {
                NextThunder = random.Next(100, 500);
            }
        }

        public float GetRainLevel()
        {
            return CurrentWeather == WeatherType.Rain ? WeatherIntensity : 0;
        }

        public float GetSnowLevel()
        {
            return 0; // 简化
        }

        public bool IsRaining()
        {
            return CurrentWeather == WeatherType.Rain && WeatherIntensity > 0.5f;
        }

        public bool IsSnowing()
        {
            return false; // 简化
        }

        public Vector3 GetSkyColor(float dayTime)
        {
            Vector3 baseColor = new Vector3(0.5f, 0.7f, 0.9f);

            if (CurrentWeather == WeatherType.Rain)
            {
                baseColor = Vector3.Lerp(baseColor, new Vector3(0.4f, 0.4f, 0.5f), WeatherIntensity);
            }

            if (IsThundering)
            {
                baseColor = Vector3.Lerp(baseColor, new Vector3(0.3f, 0.3f, 0.35f), 0.5f);
            }

            return baseColor;
        }
    }

    public enum WeatherType
    {
        Clear,
        Rain,
        Snow,
        Thunder
    }
}
