using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class GameSettings
    {
        private static GameSettings instance;
        public static GameSettings Instance => instance ??= new GameSettings();

        // 视频设置
        public int RenderDistance { get; set; } = 8;
        public int FOV { get; set; } = 70;
        public int MaxFps { get; set; } = 60;
        public bool VSync { get; set; } = true;
        public bool Fullscreen { get; set; } = false;
        public int WindowWidth { get; set; } = 1280;
        public int WindowHeight { get; set; } = 720;
        public string GraphicsQuality { get; set; } = "Fancy"; // Fast, Fancy
        public bool SmoothLighting { get; set; } = true;
        public int Clouds { get; set; } = 2; // 0=off, 1=fast, 2=fancy
        public bool ViewBobbing { get; set; } = true;
        public bool ShowFps { get; set; } = false;
        public float GuiScale { get; set; } = 2.0f;
        public int BiomeBlendRadius { get; set; } = 2;
        public bool EntityShadows { get; set; } = true;

        // 音频设置
        public float MasterVolume { get; set; } = 1.0f;
        public float MusicVolume { get; set; } = 1.0f;
        public float SoundEffectsVolume { get; set; } = 1.0f;
        public float AmbientVolume { get; set; } = 1.0f;
        public float BlocksVolume { get; set; } = 1.0f;
        public float HostileVolume { get; set; } = 1.0f;
        public float FriendlyVolume { get; set; } = 1.0f;
        public float PlayersVolume { get; set; } = 1.0f;
        public float WeatherVolume { get; set; } = 1.0f;
        public float VoiceVolume { get; set; } = 1.0f;

        // 控制设置
        public float MouseSensitivity { get; set; } = 0.5f;
        public bool InvertMouse { get; set; } = false;
        public bool TouchscreenMode { get; set; } = false;
        public string SneakMode { get; set; } = "Hold"; // Hold, Toggle
        public string SprintMode { get; set; } = "Hold"; // Hold, Toggle
        public bool AutoJump { get; set; } = true;

        // 游戏设置
        public string Difficulty { get; set; } = "Normal"; // Peaceful, Easy, Normal, Hard
        public string GameMode { get; set; } = "Survival"; // Survival, Creative, Adventure, Spectator
        public bool AllowCheats { get; set; } = false;
        public bool Hardcore { get; set; } = false;
        public bool ReducedDebugInfo { get; set; } = false;
        public bool ShowInventoryTooltip { get; set; } = true;
        public bool AdvancedItemTooltips { get; set; } = false;
        public bool HideLightningFlashes { get; set; } = false;
        public bool FovEffectScale { get; set; } = true;
        public float ScreenEffectScale { get; set; } = 1.0f;
        public bool DamageTilt { get; set; } = true;

        // 皮肤设置
        public string PlayerName { get; set; } = "Player";
        public string SkinModel { get; set; } = "Steve"; // Steve, Alex
        public string Cape { get; set; } = "None";

        // 语言
        public string Language { get; set; } = "zh_cn";
        public bool ForceUnicodeFont { get; set; } = false;

        // 聊天设置
        public string ChatVisibility { get; set; } = "Shown"; // Shown, CommandsOnly, Hidden
        public float ChatOpacity { get; set; } = 1.0f;
        public float ChatLineSpacing { get; set; } = 0.0f;
        public bool ChatColors { get; set; } = true;
        public bool ChatLinks { get; set; } = true;
        public bool ChatPrompts { get; set; } = true;
        public float ChatScale { get; set; } = 1.0f;
        public int ChatWidth { get; set; } = 0;
        public int ChatHeightFocused { get; set; } = 0;
        public int ChatHeightUnfocused { get; set; } = 0;

        // 多人游戏设置
        public bool MultiplayerWarnings { get; set; } = true;
        public bool AllowServerListing { get; set; } = true;
        public bool EnablePlayerReporting { get; set; } = true;
        public bool AllowCustomServers { get; set; } = true;

        // 辅助功能
        public bool NarratorEnabled { get; set; } = false;
        public float NarratorVolume { get; set; } = 1.0f;
        public bool Subtitles { get; set; } = false;
        public bool TextBackground { get; set; } = false;
        public float TextBackgroundOpacity { get; set; } = 0.5f;
        public bool HideSplashTexts { get; set; } = false;
        public bool DarkMojangStudiosBackground { get; set; } = false;
        public bool HideBundleTutorial { get; set; } = false;

        // 性能设置
        public int MaxChunks { get; set; } = 256;
        public int ChunkUpdateThreads { get; set; } = 2;
        public bool UseFastMath { get; set; } = false;
        public bool UseVertexBufferObjects { get; set; } = true;
        public int MaxEntityRenderDistance { get; set; } = 64;
        public int EntityCullingRange { get; set; } = 128;
        public bool UseCulling { get; set; } = true;
        public bool UseFog { get; set; } = true;
        public bool UseMipmaps { get; set; } = true;
        public int AnisotropicFiltering { get; set; } = 4;

        // 控制键位
        public Dictionary<string, string> KeyBindings { get; set; }

        // 配置文件路径
        private string configPath;

        private GameSettings()
        {
            KeyBindings = new Dictionary<string, string>();
            InitializeDefaultKeyBindings();
        }

        private void InitializeDefaultKeyBindings()
        {
            KeyBindings["forward"] = "W";
            KeyBindings["back"] = "S";
            KeyBindings["left"] = "A";
            KeyBindings["right"] = "D";
            KeyBindings["jump"] = "Space";
            KeyBindings["sneak"] = "LeftShift";
            KeyBindings["sprint"] = "LeftControl";
            KeyBindings["inventory"] = "E";
            KeyBindings["drop"] = "Q";
            KeyBindings["chat"] = "T";
            KeyBindings["command"] = "Slash";
            KeyBindings["togglePerspective"] = "F5";
            KeyBindings["smoothCamera"] = "F6";
            KeyBindings["fullscreen"] = "F11";
            KeyBindings["screenshot"] = "F2";
            KeyBindings["debug"] = "F3";
            KeyBindings["toggleFog"] = "F";
            KeyBindings["attack"] = "MouseLeft";
            KeyBindings["use"] = "MouseRight";
            KeyBindings["pickBlock"] = "MouseMiddle";
            KeyBindings["hotbar1"] = "1";
            KeyBindings["hotbar2"] = "2";
            KeyBindings["hotbar3"] = "3";
            KeyBindings["hotbar4"] = "4";
            KeyBindings["hotbar5"] = "5";
            KeyBindings["hotbar6"] = "6";
            KeyBindings["hotbar7"] = "7";
            KeyBindings["hotbar8"] = "8";
            KeyBindings["hotbar9"] = "9";
            KeyBindings["hotbar10"] = "0";
            KeyBindings["playerList"] = "Tab";
            KeyBindings["advancements"] = "L";
            KeyBindings["recipeBook"] = "H";
        }

        public void Load(string path)
        {
            configPath = path;

            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    GameSettings loaded = JsonSerializer.Deserialize<GameSettings>(json, options);

                    if (loaded != null)
                    {
                        // 复制所有属性
                        CopyFrom(loaded);
                        Console.WriteLine("[GameSettings] 配置已加载");
                    }
                }
                else
                {
                    Console.WriteLine("[GameSettings] 配置文件不存在，使用默认设置");
                    Save(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameSettings] 加载配置失败: {ex.Message}");
            }
        }

        public void Save(string path = null)
        {
            if (path == null) path = configPath;
            if (path == null) return;

            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(path, json);
                Console.WriteLine("[GameSettings] 配置已保存");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameSettings] 保存配置失败: {ex.Message}");
            }
        }

        private void CopyFrom(GameSettings other)
        {
            // 视频设置
            RenderDistance = other.RenderDistance;
            FOV = other.FOV;
            MaxFps = other.MaxFps;
            VSync = other.VSync;
            Fullscreen = other.Fullscreen;
            WindowWidth = other.WindowWidth;
            WindowHeight = other.WindowHeight;
            GraphicsQuality = other.GraphicsQuality;
            SmoothLighting = other.SmoothLighting;
            Clouds = other.Clouds;
            ViewBobbing = other.ViewBobbing;
            ShowFps = other.ShowFps;
            GuiScale = other.GuiScale;
            BiomeBlendRadius = other.BiomeBlendRadius;
            EntityShadows = other.EntityShadows;

            // 音频设置
            MasterVolume = other.MasterVolume;
            MusicVolume = other.MusicVolume;
            SoundEffectsVolume = other.SoundEffectsVolume;
            AmbientVolume = other.AmbientVolume;
            BlocksVolume = other.BlocksVolume;
            HostileVolume = other.HostileVolume;
            FriendlyVolume = other.FriendlyVolume;
            PlayersVolume = other.PlayersVolume;
            WeatherVolume = other.WeatherVolume;
            VoiceVolume = other.VoiceVolume;

            // 控制设置
            MouseSensitivity = other.MouseSensitivity;
            InvertMouse = other.InvertMouse;
            TouchscreenMode = other.TouchscreenMode;
            SneakMode = other.SneakMode;
            SprintMode = other.SprintMode;
            AutoJump = other.AutoJump;

            // 游戏设置
            Difficulty = other.Difficulty;
            GameMode = other.GameMode;
            AllowCheats = other.AllowCheats;
            Hardcore = other.Hardcore;
            ReducedDebugInfo = other.ReducedDebugInfo;
            ShowInventoryTooltip = other.ShowInventoryTooltip;
            AdvancedItemTooltips = other.AdvancedItemTooltips;
            HideLightningFlashes = other.HideLightningFlashes;
            FovEffectScale = other.FovEffectScale;
            ScreenEffectScale = other.ScreenEffectScale;
            DamageTilt = other.DamageTilt;

            // 皮肤设置
            PlayerName = other.PlayerName;
            SkinModel = other.SkinModel;
            Cape = other.Cape;

            // 语言
            Language = other.Language;
            ForceUnicodeFont = other.ForceUnicodeFont;

            // 聊天设置
            ChatVisibility = other.ChatVisibility;
            ChatOpacity = other.ChatOpacity;
            ChatLineSpacing = other.ChatLineSpacing;
            ChatColors = other.ChatColors;
            ChatLinks = other.ChatLinks;
            ChatPrompts = other.ChatPrompts;
            ChatScale = other.ChatScale;
            ChatWidth = other.ChatWidth;
            ChatHeightFocused = other.ChatHeightFocused;
            ChatHeightUnfocused = other.ChatHeightUnfocused;

            // 多人游戏设置
            MultiplayerWarnings = other.MultiplayerWarnings;
            AllowServerListing = other.AllowServerListing;
            EnablePlayerReporting = other.EnablePlayerReporting;
            AllowCustomServers = other.AllowCustomServers;

            // 辅助功能
            NarratorEnabled = other.NarratorEnabled;
            NarratorVolume = other.NarratorVolume;
            Subtitles = other.Subtitles;
            TextBackground = other.TextBackground;
            TextBackgroundOpacity = other.TextBackgroundOpacity;
            HideSplashTexts = other.HideSplashTexts;
            DarkMojangStudiosBackground = other.DarkMojangStudiosBackground;
            HideBundleTutorial = other.HideBundleTutorial;

            // 性能设置
            MaxChunks = other.MaxChunks;
            ChunkUpdateThreads = other.ChunkUpdateThreads;
            UseFastMath = other.UseFastMath;
            UseVertexBufferObjects = other.UseVertexBufferObjects;
            MaxEntityRenderDistance = other.MaxEntityRenderDistance;
            EntityCullingRange = other.EntityCullingRange;
            UseCulling = other.UseCulling;
            UseFog = other.UseFog;
            UseMipmaps = other.UseMipmaps;
            AnisotropicFiltering = other.AnisotropicFiltering;

            // 键位
            if (other.KeyBindings != null)
            {
                KeyBindings = new Dictionary<string, string>(other.KeyBindings);
            }
        }

        public void ResetToDefaults()
        {
            GameSettings defaults = new GameSettings();
            CopyFrom(defaults);
            Console.WriteLine("[GameSettings] 已重置为默认设置");
        }

        public string GetKeyBinding(string action)
        {
            return KeyBindings.TryGetValue(action, out string key) ? key : "Unknown";
        }

        public void SetKeyBinding(string action, string key)
        {
            KeyBindings[action] = key;
        }

        public int GetDifficultyLevel()
        {
            return Difficulty switch
            {
                "Peaceful" => 0,
                "Easy" => 1,
                "Normal" => 2,
                "Hard" => 3,
                _ => 2
            };
        }

        public int GetGameModeId()
        {
            return GameMode switch
            {
                "Survival" => 0,
                "Creative" => 1,
                "Adventure" => 2,
                "Spectator" => 3,
                _ => 0
            };
        }

        public bool IsSurvivalMode()
        {
            return GameMode == "Survival" || GameMode == "Adventure";
        }

        public bool IsCreativeMode()
        {
            return GameMode == "Creative";
        }

        public bool IsSpectatorMode()
        {
            return GameMode == "Spectator";
        }
    }
}
