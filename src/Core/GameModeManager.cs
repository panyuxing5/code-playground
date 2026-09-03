using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Player;
using VoxelCraft.Entities;

namespace VoxelCraft.Core
{
    public class GameModeManager
    {
        private GameMode currentMode;
        private GameMode previousMode;
        private readonly Dictionary<GameMode, GameModeSettings> modeSettings;

        // 事件
        public event Action<GameMode> OnGameModeChanged;
        public event Action<GameMode, GameMode> OnGameModeChangedWithPrevious;

        public GameMode CurrentMode => currentMode;
        public GameMode PreviousMode => previousMode;

        public GameModeManager()
        {
            currentMode = GameMode.Survival;
            previousMode = GameMode.Survival;
            modeSettings = new Dictionary<GameMode, GameModeSettings>();
        }

        public void Initialize()
        {
            Console.WriteLine("[GameModeManager] 游戏模式管理器初始化完成");
            LoadModeSettings();
        }

        private void LoadModeSettings()
        {
            // 生存模式设置
            modeSettings[GameMode.Survival] = new GameModeSettings
            {
                ModeName = "生存模式",
                Description = "收集资源、建造庇护所、对抗怪物",
                CanFly = false,
                CanBreakBlocks = true,
                CanPlaceBlocks = true,
                HasHealth = true,
                HasHunger = true,
                HasArmor = true,
                HasInventory = true,
                HasHotbar = true,
                CanUseItems = true,
                CanAttackMobs = true,
                MobsAttackPlayer = true,
                SpawnMobs = true,
                DayNightCycle = true,
                WeatherEnabled = true,
                KeepInventory = false,
                PvPEnabled = true,
                InstantBreak = false,
                InfiniteItems = false,
                ShowHealthBar = true,
                ShowHungerBar = true,
                ShowArmorBar = true,
                ShowExperienceBar = true,
                AllowCommands = false,
                Difficulty = Difficulty.Normal,
                RespawnAtBed = true,
                DropItemsOnDeath = true,
                XpDroppedOnDeath = true,
                CanSleep = true,
                CanSetSpawn = true,
                HungerDepletionRate = 1.0f,
                HealthRegenRate = 1.0f,
                MobSpawnRate = 1.0f,
                MobDamageMultiplier = 1.0f,
                PlayerDamageMultiplier = 1.0f,
                FallDamageEnabled = true,
                DrowningDamageEnabled = true,
                FireDamageEnabled = true,
                LavaDamageEnabled = true,
                VoidDamageEnabled = true,
                ExplosionDamageEnabled = true,
                PoisonDamageEnabled = true,
                WitherDamageEnabled = true,
                FreezeDamageEnabled = true,
                LightningDamageEnabled = true,
                CanUseElytra = true,
                CanUseTrident = true,
                CanUseCrossbow = true,
                CanUseShield = true,
                CanUseTotem = true,
                CanUseEnderPearl = true,
                CanUseChorusFruit = true,
                CanUseNetherPortal = true,
                CanUseEndPortal = true,
                CanUseRespawnAnchor = true,
                CanUseLodestone = true,
                CanUseCompass = true,
                CanUseClock = true,
                CanUseMap = true,
                CanUseSpyglass = true,
                CanUseNameTag = true,
                CanUseLead = true,
                CanUseSaddle = true,
                CanUseHorseArmor = true,
                CanUseCarpet = true,
                CanUseBanner = true,
                CanUseShieldPattern = true,
                CanUseFireworkRocket = true,
                CanUseFireworkStar = true,
                CanUseFireCharge = true,
                CanUseFlintAndSteel = true,
                CanUseBucket = true,
                CanUseLavaBucket = true,
                CanUseWaterBucket = true,
                CanUseMilkBucket = true,
                CanUsePowderSnowBucket = true,
                CanUsePufferfishBucket = true,
                CanUseTropicalFishBucket = true,
                CanUseCodBucket = true,
                CanUseSalmonBucket = true,
                CanUseAxolotlBucket = true,
                CanUseTadpoleBucket = true
            };

            // 创造模式设置
            modeSettings[GameMode.Creative] = new GameModeSettings
            {
                ModeName = "创造模式",
                Description = "无限资源、自由建造、瞬间破坏",
                CanFly = true,
                CanBreakBlocks = true,
                CanPlaceBlocks = true,
                HasHealth = false,
                HasHunger = false,
                HasArmor = false,
                HasInventory = true,
                HasHotbar = true,
                CanUseItems = true,
                CanAttackMobs = true,
                MobsAttackPlayer = false,
                SpawnMobs = true,
                DayNightCycle = true,
                WeatherEnabled = true,
                KeepInventory = true,
                PvPEnabled = false,
                InstantBreak = true,
                InfiniteItems = true,
                ShowHealthBar = false,
                ShowHungerBar = false,
                ShowArmorBar = false,
                ShowExperienceBar = false,
                AllowCommands = true,
                Difficulty = Difficulty.Peaceful,
                RespawnAtBed = true,
                DropItemsOnDeath = false,
                XpDroppedOnDeath = false,
                CanSleep = true,
                CanSetSpawn = true,
                HungerDepletionRate = 0.0f,
                HealthRegenRate = 10.0f,
                MobSpawnRate = 1.0f,
                MobDamageMultiplier = 0.0f,
                PlayerDamageMultiplier = 1.0f,
                FallDamageEnabled = false,
                DrowningDamageEnabled = false,
                FireDamageEnabled = false,
                LavaDamageEnabled = false,
                VoidDamageEnabled = true,
                ExplosionDamageEnabled = false,
                PoisonDamageEnabled = false,
                WitherDamageEnabled = false,
                FreezeDamageEnabled = false,
                LightningDamageEnabled = false,
                CanUseElytra = true,
                CanUseTrident = true,
                CanUseCrossbow = true,
                CanUseShield = true,
                CanUseTotem = true,
                CanUseEnderPearl = true,
                CanUseChorusFruit = true,
                CanUseNetherPortal = true,
                CanUseEndPortal = true,
                CanUseRespawnAnchor = true,
                CanUseLodestone = true,
                CanUseCompass = true,
                CanUseClock = true,
                CanUseMap = true,
                CanUseSpyglass = true,
                CanUseNameTag = true,
                CanUseLead = true,
                CanUseSaddle = true,
                CanUseHorseArmor = true,
                CanUseCarpet = true,
                CanUseBanner = true,
                CanUseShieldPattern = true,
                CanUseFireworkRocket = true,
                CanUseFireworkStar = true,
                CanUseFireCharge = true,
                CanUseFlintAndSteel = true,
                CanUseBucket = true,
                CanUseLavaBucket = true,
                CanUseWaterBucket = true,
                CanUseMilkBucket = true,
                CanUsePowderSnowBucket = true,
                CanUsePufferfishBucket = true,
                CanUseTropicalFishBucket = true,
                CanUseCodBucket = true,
                CanUseSalmonBucket = true,
                CanUseAxolotlBucket = true,
                CanUseTadpoleBucket = true
            };

            // 冒险模式设置
            modeSettings[GameMode.Adventure] = new GameModeSettings
            {
                ModeName = "冒险模式",
                Description = "只能使用对应工具破坏方块，适合自定义地图",
                CanFly = false,
                CanBreakBlocks = false,
                CanPlaceBlocks = false,
                HasHealth = true,
                HasHunger = true,
                HasArmor = true,
                HasInventory = true,
                HasHotbar = true,
                CanUseItems = true,
                CanAttackMobs = true,
                MobsAttackPlayer = true,
                SpawnMobs = true,
                DayNightCycle = true,
                WeatherEnabled = true,
                KeepInventory = false,
                PvPEnabled = true,
                InstantBreak = false,
                InfiniteItems = false,
                ShowHealthBar = true,
                ShowHungerBar = true,
                ShowArmorBar = true,
                ShowExperienceBar = true,
                AllowCommands = false,
                Difficulty = Difficulty.Normal,
                RespawnAtBed = true,
                DropItemsOnDeath = true,
                XpDroppedOnDeath = true,
                CanSleep = true,
                CanSetSpawn = true,
                HungerDepletionRate = 1.0f,
                HealthRegenRate = 1.0f,
                MobSpawnRate = 1.0f,
                MobDamageMultiplier = 1.0f,
                PlayerDamageMultiplier = 1.0f,
                FallDamageEnabled = true,
                DrowningDamageEnabled = true,
                FireDamageEnabled = true,
                LavaDamageEnabled = true,
                VoidDamageEnabled = true,
                ExplosionDamageEnabled = true,
                PoisonDamageEnabled = true,
                WitherDamageEnabled = true,
                FreezeDamageEnabled = true,
                LightningDamageEnabled = true,
                CanUseElytra = true,
                CanUseTrident = true,
                CanUseCrossbow = true,
                CanUseShield = true,
                CanUseTotem = true,
                CanUseEnderPearl = true,
                CanUseChorusFruit = true,
                CanUseNetherPortal = true,
                CanUseEndPortal = true,
                CanUseRespawnAnchor = true,
                CanUseLodestone = true,
                CanUseCompass = true,
                CanUseClock = true,
                CanUseMap = true,
                CanUseSpyglass = true,
                CanUseNameTag = true,
                CanUseLead = true,
                CanUseSaddle = true,
                CanUseHorseArmor = true,
                CanUseCarpet = true,
                CanUseBanner = true,
                CanUseShieldPattern = true,
                CanUseFireworkRocket = true,
                CanUseFireworkStar = true,
                CanUseFireCharge = true,
                CanUseFlintAndSteel = true,
                CanUseBucket = true,
                CanUseLavaBucket = true,
                CanUseWaterBucket = true,
                CanUseMilkBucket = true,
                CanUsePowderSnowBucket = true,
                CanUsePufferfishBucket = true,
                CanUseTropicalFishBucket = true,
                CanUseCodBucket = true,
                CanUseSalmonBucket = true,
                CanUseAxolotlBucket = true,
                CanUseTadpoleBucket = true
            };

            // 旁观者模式设置
            modeSettings[GameMode.Spectator] = new GameModeSettings
            {
                ModeName = "旁观者模式",
                Description = "自由观察、穿墙、不可交互",
                CanFly = true,
                CanBreakBlocks = false,
                CanPlaceBlocks = false,
                HasHealth = false,
                HasHunger = false,
                HasArmor = false,
                HasInventory = false,
                HasHotbar = false,
                CanUseItems = false,
                CanAttackMobs = false,
                MobsAttackPlayer = false,
                SpawnMobs = true,
                DayNightCycle = true,
                WeatherEnabled = true,
                KeepInventory = true,
                PvPEnabled = false,
                InstantBreak = false,
                InfiniteItems = false,
                ShowHealthBar = false,
                ShowHungerBar = false,
                ShowArmorBar = false,
                ShowExperienceBar = false,
                AllowCommands = true,
                Difficulty = Difficulty.Peaceful,
                RespawnAtBed = false,
                DropItemsOnDeath = false,
                XpDroppedOnDeath = false,
                CanSleep = false,
                CanSetSpawn = false,
                HungerDepletionRate = 0.0f,
                HealthRegenRate = 0.0f,
                MobSpawnRate = 1.0f,
                MobDamageMultiplier = 0.0f,
                PlayerDamageMultiplier = 0.0f,
                FallDamageEnabled = false,
                DrowningDamageEnabled = false,
                FireDamageEnabled = false,
                LavaDamageEnabled = false,
                VoidDamageEnabled = false,
                ExplosionDamageEnabled = false,
                PoisonDamageEnabled = false,
                WitherDamageEnabled = false,
                FreezeDamageEnabled = false,
                LightningDamageEnabled = false,
                CanUseElytra = false,
                CanUseTrident = false,
                CanUseCrossbow = false,
                CanUseShield = false,
                CanUseTotem = false,
                CanUseEnderPearl = false,
                CanUseChorusFruit = false,
                CanUseNetherPortal = true,
                CanUseEndPortal = true,
                CanUseRespawnAnchor = false,
                CanUseLodestone = false,
                CanUseCompass = true,
                CanUseClock = true,
                CanUseMap = true,
                CanUseSpyglass = true,
                CanUseNameTag = false,
                CanUseLead = false,
                CanUseSaddle = false,
                CanUseHorseArmor = false,
                CanUseCarpet = false,
                CanUseBanner = false,
                CanUseShieldPattern = false,
                CanUseFireworkRocket = false,
                CanUseFireworkStar = false,
                CanUseFireCharge = false,
                CanUseFlintAndSteel = false,
                CanUseBucket = false,
                CanUseLavaBucket = false,
                CanUseWaterBucket = false,
                CanUseMilkBucket = false,
                CanUsePowderSnowBucket = false,
                CanUsePufferfishBucket = false,
                CanUseTropicalFishBucket = false,
                CanUseCodBucket = false,
                CanUseSalmonBucket = false,
                CanUseAxolotlBucket = false,
                CanUseTadpoleBucket = false
            };

            // 极限模式设置
            modeSettings[GameMode.Hardcore] = new GameModeSettings
            {
                ModeName = "极限模式",
                Description = "最高难度，死亡后世界删除",
                CanFly = false,
                CanBreakBlocks = true,
                CanPlaceBlocks = true,
                HasHealth = true,
                HasHunger = true,
                HasArmor = true,
                HasInventory = true,
                HasHotbar = true,
                CanUseItems = true,
                CanAttackMobs = true,
                MobsAttackPlayer = true,
                SpawnMobs = true,
                DayNightCycle = true,
                WeatherEnabled = true,
                KeepInventory = false,
                PvPEnabled = true,
                InstantBreak = false,
                InfiniteItems = false,
                ShowHealthBar = true,
                ShowHungerBar = true,
                ShowArmorBar = true,
                ShowExperienceBar = true,
                AllowCommands = false,
                Difficulty = Difficulty.Hard,
                RespawnAtBed = false,
                DropItemsOnDeath = true,
                XpDroppedOnDeath = true,
                CanSleep = true,
                CanSetSpawn = true,
                HungerDepletionRate = 1.5f,
                HealthRegenRate = 0.5f,
                MobSpawnRate = 2.0f,
                MobDamageMultiplier = 1.5f,
                PlayerDamageMultiplier = 1.0f,
                FallDamageEnabled = true,
                DrowningDamageEnabled = true,
                FireDamageEnabled = true,
                LavaDamageEnabled = true,
                VoidDamageEnabled = true,
                ExplosionDamageEnabled = true,
                PoisonDamageEnabled = true,
                WitherDamageEnabled = true,
                FreezeDamageEnabled = true,
                LightningDamageEnabled = true,
                CanUseElytra = true,
                CanUseTrident = true,
                CanUseCrossbow = true,
                CanUseShield = true,
                CanUseTotem = true,
                CanUseEnderPearl = true,
                CanUseChorusFruit = true,
                CanUseNetherPortal = true,
                CanUseEndPortal = true,
                CanUseRespawnAnchor = true,
                CanUseLodestone = true,
                CanUseCompass = true,
                CanUseClock = true,
                CanUseMap = true,
                CanUseSpyglass = true,
                CanUseNameTag = true,
                CanUseLead = true,
                CanUseSaddle = true,
                CanUseHorseArmor = true,
                CanUseCarpet = true,
                CanUseBanner = true,
                CanUseShieldPattern = true,
                CanUseFireworkRocket = true,
                CanUseFireworkStar = true,
                CanUseFireCharge = true,
                CanUseFlintAndSteel = true,
                CanUseBucket = true,
                CanUseLavaBucket = true,
                CanUseWaterBucket = true,
                CanUseMilkBucket = true,
                CanUsePowderSnowBucket = true,
                CanUsePufferfishBucket = true,
                CanUseTropicalFishBucket = true,
                CanUseCodBucket = true,
                CanUseSalmonBucket = true,
                CanUseAxolotlBucket = true,
                CanUseTadpoleBucket = true
            };

            Console.WriteLine($"[GameModeManager] 已加载 {modeSettings.Count} 种游戏模式设置");
        }

        public void SetGameMode(GameMode mode)
        {
            if (currentMode == mode) return;

            previousMode = currentMode;
            currentMode = mode;

            OnGameModeChanged?.Invoke(mode);
            OnGameModeChangedWithPrevious?.Invoke(previousMode, mode);

            Console.WriteLine($"[GameModeManager] 游戏模式已切换: {previousMode} -> {mode}");
        }

        public void ToggleGameMode()
        {
            if (currentMode == GameMode.Survival)
            {
                SetGameMode(GameMode.Creative);
            }
            else
            {
                SetGameMode(GameMode.Survival);
            }
        }

        public GameModeSettings GetCurrentSettings()
        {
            return GetSettings(currentMode);
        }

        public GameModeSettings GetSettings(GameMode mode)
        {
            if (modeSettings.TryGetValue(mode, out GameModeSettings settings))
            {
                return settings;
            }
            return modeSettings[GameMode.Survival];
        }

        public bool IsSurvival()
        {
            return currentMode == GameMode.Survival || currentMode == GameMode.Hardcore;
        }

        public bool IsCreative()
        {
            return currentMode == GameMode.Creative;
        }

        public bool IsAdventure()
        {
            return currentMode == GameMode.Adventure;
        }

        public bool IsSpectator()
        {
            return currentMode == GameMode.Spectator;
        }

        public bool IsHardcore()
        {
            return currentMode == GameMode.Hardcore;
        }

        public bool CanFly()
        {
            return GetCurrentSettings().CanFly;
        }

        public bool CanBreakBlocks()
        {
            return GetCurrentSettings().CanBreakBlocks;
        }

        public bool CanPlaceBlocks()
        {
            return GetCurrentSettings().CanPlaceBlocks;
        }

        public bool HasHealth()
        {
            return GetCurrentSettings().HasHealth;
        }

        public bool HasHunger()
        {
            return GetCurrentSettings().HasHunger;
        }

        public bool HasArmor()
        {
            return GetCurrentSettings().HasArmor;
        }

        public bool HasInventory()
        {
            return GetCurrentSettings().HasInventory;
        }

        public bool HasHotbar()
        {
            return GetCurrentSettings().HasHotbar;
        }

        public bool CanUseItems()
        {
            return GetCurrentSettings().CanUseItems;
        }

        public bool CanAttackMobs()
        {
            return GetCurrentSettings().CanAttackMobs;
        }

        public bool MobsAttackPlayer()
        {
            return GetCurrentSettings().MobsAttackPlayer;
        }

        public bool SpawnMobs()
        {
            return GetCurrentSettings().SpawnMobs;
        }

        public bool DayNightCycle()
        {
            return GetCurrentSettings().DayNightCycle;
        }

        public bool WeatherEnabled()
        {
            return GetCurrentSettings().WeatherEnabled;
        }

        public bool KeepInventory()
        {
            return GetCurrentSettings().KeepInventory;
        }

        public bool PvPEnabled()
        {
            return GetCurrentSettings().PvPEnabled;
        }

        public bool InstantBreak()
        {
            return GetCurrentSettings().InstantBreak;
        }

        public bool InfiniteItems()
        {
            return GetCurrentSettings().InfiniteItems;
        }

        public bool ShowHealthBar()
        {
            return GetCurrentSettings().ShowHealthBar;
        }

        public bool ShowHungerBar()
        {
            return GetCurrentSettings().ShowHungerBar;
        }

        public bool ShowArmorBar()
        {
            return GetCurrentSettings().ShowArmorBar;
        }

        public bool ShowExperienceBar()
        {
            return GetCurrentSettings().ShowExperienceBar;
        }

        public bool AllowCommands()
        {
            return GetCurrentSettings().AllowCommands;
        }

        public Difficulty GetDifficulty()
        {
            return GetCurrentSettings().Difficulty;
        }

        public bool RespawnAtBed()
        {
            return GetCurrentSettings().RespawnAtBed;
        }

        public bool DropItemsOnDeath()
        {
            return GetCurrentSettings().DropItemsOnDeath;
        }

        public bool XpDroppedOnDeath()
        {
            return GetCurrentSettings().XpDroppedOnDeath;
        }

        public bool CanSleep()
        {
            return GetCurrentSettings().CanSleep;
        }

        public bool CanSetSpawn()
        {
            return GetCurrentSettings().CanSetSpawn;
        }

        public float GetHungerDepletionRate()
        {
            return GetCurrentSettings().HungerDepletionRate;
        }

        public float GetHealthRegenRate()
        {
            return GetCurrentSettings().HealthRegenRate;
        }

        public float GetMobSpawnRate()
        {
            return GetCurrentSettings().MobSpawnRate;
        }

        public float GetMobDamageMultiplier()
        {
            return GetCurrentSettings().MobDamageMultiplier;
        }

        public float GetPlayerDamageMultiplier()
        {
            return GetCurrentSettings().PlayerDamageMultiplier;
        }

        public bool IsDamageEnabled(DamageType type)
        {
            GameModeSettings settings = GetCurrentSettings();
            switch (type)
            {
                case DamageType.Fall: return settings.FallDamageEnabled;
                case DamageType.Drowning: return settings.DrowningDamageEnabled;
                case DamageType.Fire: return settings.FireDamageEnabled;
                case DamageType.Lava: return settings.LavaDamageEnabled;
                case DamageType.Void: return settings.VoidDamageEnabled;
                case DamageType.Explosion: return settings.ExplosionDamageEnabled;
                case DamageType.Poison: return settings.PoisonDamageEnabled;
                case DamageType.Wither: return settings.WitherDamageEnabled;
                case DamageType.Freeze: return settings.FreezeDamageEnabled;
                case DamageType.Lightning: return settings.LightningDamageEnabled;
                default: return true;
            }
        }

        public string GetModeName()
        {
            return GetCurrentSettings().ModeName;
        }

        public string GetModeDescription()
        {
            return GetCurrentSettings().Description;
        }

        public List<GameMode> GetAllModes()
        {
            return new List<GameMode>((GameMode[])Enum.GetValues(typeof(GameMode)));
        }

        public void RenderModeSelector(UIManager uiManager, int x, int y, int width, int height)
        {
            GameMode[] modes = (GameMode[])Enum.GetValues(typeof(GameMode));
            int modeWidth = 120;
            int modeHeight = 80;
            int spacing = 10;
            int modesPerRow = width / (modeWidth + spacing);

            for (int i = 0; i < modes.Length; i++)
            {
                int row = i / modesPerRow;
                int col = i % modesPerRow;
                int modeX = x + col * (modeWidth + spacing);
                int modeY = y + row * (modeHeight + spacing);

                if (modeY + modeHeight > y + height) break;

                GameMode mode = modes[i];
                GameModeSettings settings = GetSettings(mode);
                bool isSelected = mode == currentMode;

                // 模式卡片背景
                uiManager.DrawPanel(modeX, modeY, modeWidth, modeHeight,
                    isSelected ? new Color4(0.3f, 0.4f, 0.3f, 0.9f) : new Color4(0.2f, 0.2f, 0.25f, 0.9f));

                // 模式名称
                uiManager.DrawText(settings.ModeName, modeX + 5, modeY + 5, 14, Color4.White);

                // 模式描述
                uiManager.DrawText(settings.Description, modeX + 5, modeY + 25, 10,
                    new Color4(0.7f, 0.7f, 0.7f, 1f));

                // 选中标记
                if (isSelected)
                {
                    uiManager.DrawPanel(modeX, modeY + modeHeight - 20, modeWidth, 20,
                        new Color4(0.3f, 0.6f, 0.3f, 0.8f));
                    int textWidth = uiManager.MeasureText("当前模式", 10);
                    uiManager.DrawText("当前模式", modeX + (modeWidth - textWidth) / 2, modeY + modeHeight - 15, 10, Color4.White);
                }
            }
        }
    }

    public enum DamageType
    {
        Fall,
        Drowning,
        Fire,
        Lava,
        Void,
        Explosion,
        Poison,
        Wither,
        Freeze,
        Lightning,
        Mob,
        Player,
        Arrow,
        Thorns,
        Cactus,
        SweetBerryBush,
        MagmaBlock,
        HotFloor,
        Contact,
        Generic
    }

    public class GameModeSettings
    {
        public string ModeName;
        public string Description;
        public bool CanFly;
        public bool CanBreakBlocks;
        public bool CanPlaceBlocks;
        public bool HasHealth;
        public bool HasHunger;
        public bool HasArmor;
        public bool HasInventory;
        public bool HasHotbar;
        public bool CanUseItems;
        public bool CanAttackMobs;
        public bool MobsAttackPlayer;
        public bool SpawnMobs;
        public bool DayNightCycle;
        public bool WeatherEnabled;
        public bool KeepInventory;
        public bool PvPEnabled;
        public bool InstantBreak;
        public bool InfiniteItems;
        public bool ShowHealthBar;
        public bool ShowHungerBar;
        public bool ShowArmorBar;
        public bool ShowExperienceBar;
        public bool AllowCommands;
        public Difficulty Difficulty;
        public bool RespawnAtBed;
        public bool DropItemsOnDeath;
        public bool XpDroppedOnDeath;
        public bool CanSleep;
        public bool CanSetSpawn;
        public float HungerDepletionRate;
        public float HealthRegenRate;
        public float MobSpawnRate;
        public float MobDamageMultiplier;
        public float PlayerDamageMultiplier;
        public bool FallDamageEnabled;
        public bool DrowningDamageEnabled;
        public bool FireDamageEnabled;
        public bool LavaDamageEnabled;
        public bool VoidDamageEnabled;
        public bool ExplosionDamageEnabled;
        public bool PoisonDamageEnabled;
        public bool WitherDamageEnabled;
        public bool FreezeDamageEnabled;
        public bool LightningDamageEnabled;
        public bool CanUseElytra;
        public bool CanUseTrident;
        public bool CanUseCrossbow;
        public bool CanUseShield;
        public bool CanUseTotem;
        public bool CanUseEnderPearl;
        public bool CanUseChorusFruit;
        public bool CanUseNetherPortal;
        public bool CanUseEndPortal;
        public bool CanUseRespawnAnchor;
        public bool CanUseLodestone;
        public bool CanUseCompass;
        public bool CanUseClock;
        public bool CanUseMap;
        public bool CanUseSpyglass;
        public bool CanUseNameTag;
        public bool CanUseLead;
        public bool CanUseSaddle;
        public bool CanUseHorseArmor;
        public bool CanUseCarpet;
        public bool CanUseBanner;
        public bool CanUseShieldPattern;
        public bool CanUseFireworkRocket;
        public bool CanUseFireworkStar;
        public bool CanUseFireCharge;
        public bool CanUseFlintAndSteel;
        public bool CanUseBucket;
        public bool CanUseLavaBucket;
        public bool CanUseWaterBucket;
        public bool CanUseMilkBucket;
        public bool CanUsePowderSnowBucket;
        public bool CanUsePufferfishBucket;
        public bool CanUseTropicalFishBucket;
        public bool CanUseCodBucket;
        public bool CanUseSalmonBucket;
        public bool CanUseAxolotlBucket;
        public bool CanUseTadpoleBucket;
    }
}
