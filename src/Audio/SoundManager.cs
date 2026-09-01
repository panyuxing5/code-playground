using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Audio
{
    public class SoundManager
    {
        // 音量设置
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

        // 音效缓存
        private readonly Dictionary<string, Sound> soundCache;
        private readonly List<SoundInstance> activeSounds;

        // 音乐
        private Sound currentMusic;
        private float musicFadeTime;
        private float musicTimer;
        private int currentTrackIndex;
        private readonly List<string> musicTracks;

        // 环境音
        private float ambientTimer;
        private readonly List<string> ambientSounds;

        // 监听器位置
        public Vector3 ListenerPosition { get; set; }
        public Vector3 ListenerVelocity { get; set; }
        public float ListenerYaw { get; set; }
        public float ListenerPitch { get; set; }

        // 是否启用
        public bool Enabled { get; set; } = true;

        public SoundManager()
        {
            soundCache = new Dictionary<string, Sound>();
            activeSounds = new List<SoundInstance>();
            musicTracks = new List<string>();
            ambientSounds = new List<string>();
            InitializeMusicTracks();
            InitializeAmbientSounds();
        }

        public void Initialize()
        {
            // 初始化音频设备
            Console.WriteLine("[SoundManager] 音效管理器初始化完成");
        }

        private void InitializeMusicTracks()
        {
            // Minecraft风格的背景音乐
            musicTracks.AddRange(new[]
            {
                "music.game.calm1",
                "music.game.calm2",
                "music.game.calm3",
                "music.game.piano1",
                "music.game.piano2",
                "music.game.piano3",
                "music.nether.basalt_deltas",
                "music.nether.crimson_forest",
                "music.nether.nether_wastes",
                "music.nether.soul_sand_valley",
                "music.nether.warped_forest",
                "music.end.end",
                "music.end.end_boss",
                "music.end.end_fight"
            });
        }

        private void InitializeAmbientSounds()
        {
            ambientSounds.AddRange(new[]
            {
                "ambient.cave",
                "ambient.cave.cave1",
                "ambient.cave.cave2",
                "ambient.cave.cave3",
                "ambient.weather.rain",
                "ambient.weather.thunder",
                "ambient.basalt_deltas",
                "ambient.crimson_forest",
                "ambient.nether_wastes",
                "ambient.soul_sand_valley",
                "ambient.warped_forest"
            });
        }

        public void Update(float deltaTime)
        {
            if (!Enabled) return;

            // 更新活跃音效
            for (int i = activeSounds.Count - 1; i >= 0; i--)
            {
                SoundInstance instance = activeSounds[i];
                instance.Time += deltaTime;

                if (instance.Time >= instance.Duration)
                {
                    activeSounds.RemoveAt(i);
                    continue;
                }

                // 3D音效位置更新
                UpdateSoundPosition(instance);
            }

            // 音乐播放
            UpdateMusic(deltaTime);

            // 环境音
            UpdateAmbient(deltaTime);
        }

        private void UpdateMusic(float deltaTime)
        {
            musicTimer -= deltaTime;

            if (currentMusic == null && musicTimer <= 0)
            {
                // 随机选择下一首
                currentTrackIndex = new Random().Next(musicTracks.Count);
                PlayMusic(musicTracks[currentTrackIndex]);
                musicTimer = 300.0f + new Random().Next(600); // 5-15分钟间隔
            }

            // 音乐淡出
            if (musicFadeTime > 0)
            {
                musicFadeTime -= deltaTime;
                if (currentMusic != null)
                {
                    // currentMusic.Volume = Math.Max(0, musicFadeTime / 3.0f) * MusicVolume * MasterVolume;
                }
            }
        }

        private void UpdateAmbient(float deltaTime)
        {
            ambientTimer -= deltaTime;

            if (ambientTimer <= 0)
            {
                // 随机播放环境音
                if (new Random().NextDouble() < 0.3f)
                {
                    string sound = ambientSounds[new Random().Next(ambientSounds.Count)];
                    PlaySound(sound, ListenerPosition, AmbientVolume * MasterVolume, 1.0f);
                }
                ambientTimer = 10.0f + new Random().Next(20);
            }
        }

        private void UpdateSoundPosition(SoundInstance instance)
        {
            if (!instance.Is3D) return;

            // 计算距离和方向
            Vector3 direction = instance.Position - ListenerPosition;
            float distance = direction.Length;

            // 距离衰减
            float volume = 1.0f / (1.0f + distance * 0.1f);
            instance.CurrentVolume = instance.BaseVolume * volume;

            // 左右声道平衡
            if (distance > 0.1f)
            {
                direction.Normalize();
                float yawRad = ListenerYaw;
                Vector3 listenerForward = new Vector3(
                    (float)-Math.Sin(yawRad),
                    0,
                    (float)-Math.Cos(yawRad)
                );
                Vector3 listenerRight = new Vector3(
                    (float)Math.Cos(yawRad),
                    0,
                    (float)-Math.Sin(yawRad)
                );

                float pan = Vector3.Dot(direction, listenerRight);
                instance.Pan = Math.Clamp(pan, -1.0f, 1.0f);
            }
        }

        // ========================================
        // 播放音效
        // ========================================
        public void PlaySound(string soundName, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
        {
            if (!Enabled) return;

            Sound sound = GetSound(soundName);
            if (sound == null) return;

            SoundInstance instance = new SoundInstance
            {
                Sound = sound,
                Position = position,
                BaseVolume = volume * SoundEffectsVolume * MasterVolume,
                Pitch = pitch,
                Is3D = true,
                Time = 0,
                Duration = sound.Duration
            };

            activeSounds.Add(instance);
        }

        public void PlaySound(string soundName, float volume = 1.0f, float pitch = 1.0f)
        {
            if (!Enabled) return;

            Sound sound = GetSound(soundName);
            if (sound == null) return;

            SoundInstance instance = new SoundInstance
            {
                Sound = sound,
                Position = ListenerPosition,
                BaseVolume = volume * SoundEffectsVolume * MasterVolume,
                Pitch = pitch,
                Is3D = false,
                Time = 0,
                Duration = sound.Duration
            };

            activeSounds.Add(instance);
        }

        public void PlayBlockBreak(ushort blockId, Vector3 position)
        {
            string sound = GetBlockBreakSound(blockId);
            PlaySound(sound, position, BlocksVolume * MasterVolume, 0.8f + new Random().Next(4) * 0.1f);
        }

        public void PlayBlockPlace(ushort blockId, Vector3 position)
        {
            string sound = GetBlockPlaceSound(blockId);
            PlaySound(sound, position, BlocksVolume * MasterVolume, 0.8f + new Random().Next(4) * 0.1f);
        }

        public void PlayStepSound(ushort blockId, Vector3 position)
        {
            string sound = GetStepSound(blockId);
            PlaySound(sound, position, BlocksVolume * MasterVolume * 0.5f, 0.9f + new Random().Next(3) * 0.1f);
        }

        public void PlayHurtSound(string entityType, Vector3 position)
        {
            string sound = $"mob.{entityType}.hurt";
            PlaySound(sound, position, HostileVolume * MasterVolume, 1.0f);
        }

        public void PlayDeathSound(string entityType, Vector3 position)
        {
            string sound = $"mob.{entityType}.death";
            PlaySound(sound, position, HostileVolume * MasterVolume, 1.0f);
        }

        public void PlayAmbientSound(string entityType, Vector3 position)
        {
            string sound = $"mob.{entityType}.ambient";
            PlaySound(sound, position, HostileVolume * MasterVolume, 1.0f);
        }

        public void PlayMusic(string trackName)
        {
            if (!Enabled) return;

            StopMusic();

            currentMusic = GetSound(trackName);
            if (currentMusic != null)
            {
                // 播放音乐
                musicFadeTime = 3.0f;
            }
        }

        public void StopMusic()
        {
            if (currentMusic != null)
            {
                musicFadeTime = 3.0f;
            }
        }

        public void PlayRain()
        {
            PlaySound("ambient.weather.rain", ListenerPosition, WeatherVolume * MasterVolume * 0.5f, 1.0f);
        }

        public void PlayThunder()
        {
            PlaySound("ambient.weather.thunder", ListenerPosition, WeatherVolume * MasterVolume, 0.8f);
        }

        public void PlayExplosion(Vector3 position)
        {
            PlaySound("random.explode", position, SoundEffectsVolume * MasterVolume, 0.8f + new Random().Next(4) * 0.1f);
        }

        public void PlayLevelUp()
        {
            PlaySound("random.levelup", PlayersVolume * MasterVolume, 1.0f);
        }

        public void PlayEat()
        {
            PlaySound("random.eat", PlayersVolume * MasterVolume * 0.5f, 1.0f);
        }

        public void PlayDrink()
        {
            PlaySound("random.drink", PlayersVolume * MasterVolume * 0.5f, 1.0f);
        }

        public void PlayBowShoot()
        {
            PlaySound("random.bow", PlayersVolume * MasterVolume, 1.0f);
        }

        public void PlayArrowHit()
        {
            PlaySound("random.bowhit", PlayersVolume * MasterVolume, 1.0f);
        }

        public void PlayItemPickup()
        {
            PlaySound("random.pop", PlayersVolume * MasterVolume * 0.5f, 1.5f);
        }

        public void PlayItemBreak()
        {
            PlaySound("random.break", PlayersVolume * MasterVolume, 1.0f);
        }

        public void PlaySplash(Vector3 position)
        {
            PlaySound("random.splash", position, SoundEffectsVolume * MasterVolume, 1.0f);
        }

        public void PlaySwim()
        {
            PlaySound("liquid.swim", PlayersVolume * MasterVolume * 0.3f, 1.0f);
        }

        // ========================================
        // 音效映射
        // ========================================
        private string GetBlockBreakSound(ushort blockId)
        {
            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            if (info == null) return "dig.stone";

            return info.Material switch
            {
                BlockMaterial.Grass => "dig.grass",
                BlockMaterial.Ground => "dig.gravel",
                BlockMaterial.Wood => "dig.wood",
                BlockMaterial.Rock => "dig.stone",
                BlockMaterial.Metal => "dig.stone",
                BlockMaterial.Glass => "dig.glass",
                BlockMaterial.Sand => "dig.sand",
                BlockMaterial.Snow => "dig.snow",
                BlockMaterial.Cloth => "dig.cloth",
                _ => "dig.stone"
            };
        }

        private string GetBlockPlaceSound(ushort blockId)
        {
            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            if (info == null) return "dig.stone";

            return info.Material switch
            {
                BlockMaterial.Grass => "dig.grass",
                BlockMaterial.Ground => "dig.gravel",
                BlockMaterial.Wood => "dig.wood",
                BlockMaterial.Rock => "dig.stone",
                BlockMaterial.Metal => "dig.stone",
                BlockMaterial.Glass => "dig.glass",
                BlockMaterial.Sand => "dig.sand",
                BlockMaterial.Snow => "dig.snow",
                BlockMaterial.Cloth => "dig.cloth",
                _ => "dig.stone"
            };
        }

        private string GetStepSound(ushort blockId)
        {
            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            if (info == null) return "step.stone";

            return info.Material switch
            {
                BlockMaterial.Grass => "step.grass",
                BlockMaterial.Ground => "step.gravel",
                BlockMaterial.Wood => "step.wood",
                BlockMaterial.Rock => "step.stone",
                BlockMaterial.Metal => "step.metal",
                BlockMaterial.Glass => "step.glass",
                BlockMaterial.Sand => "step.sand",
                BlockMaterial.Snow => "step.snow",
                BlockMaterial.Cloth => "step.cloth",
                _ => "step.stone"
            };
        }

        // ========================================
        // 音效加载
        // ========================================
        private Sound GetSound(string soundName)
        {
            if (soundCache.TryGetValue(soundName, out Sound sound))
            {
                return sound;
            }

            // 尝试加载音效
            sound = LoadSound(soundName);
            if (sound != null)
            {
                soundCache[soundName] = sound;
            }

            return sound;
        }

        private Sound LoadSound(string soundName)
        {
            // 尝试从assets目录加载
            string path = Path.Combine("assets", "sounds", soundName.Replace('.', Path.DirectorySeparatorChar) + ".ogg");

            if (!File.Exists(path))
            {
                // 返回一个空的音效对象（实际项目中应该有默认音效）
                return new Sound
                {
                    Name = soundName,
                    Duration = 0.5f,
                    SampleRate = 44100,
                    Channels = 2
                };
            }

            try
            {
                // 实际项目中应该用音频库加载
                return new Sound
                {
                    Name = soundName,
                    Duration = 1.0f,
                    SampleRate = 44100,
                    Channels = 2
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SoundManager] 加载音效失败 {soundName}: {ex.Message}");
                return null;
            }
        }

        public void PreloadSounds(List<string> soundNames)
        {
            foreach (string name in soundNames)
            {
                GetSound(name);
            }
            Console.WriteLine($"[SoundManager] 预加载了 {soundNames.Count} 个音效");
        }

        public void StopAllSounds()
        {
            activeSounds.Clear();
            StopMusic();
        }

        public void SetListener(Vector3 position, Vector3 velocity, float yaw, float pitch)
        {
            ListenerPosition = position;
            ListenerVelocity = velocity;
            ListenerYaw = yaw;
            ListenerPitch = pitch;
        }

        public void Dispose()
        {
            StopAllSounds();
            soundCache.Clear();
            Console.WriteLine("[SoundManager] 音效管理器已释放");
        }
    }

    // ========================================
    // 音效数据结构
    // ========================================
    public class Sound
    {
        public string Name;
        public float Duration;
        public int SampleRate;
        public int Channels;
        public byte[] Data;
    }

    public class SoundInstance
    {
        public Sound Sound;
        public Vector3 Position;
        public float BaseVolume;
        public float CurrentVolume;
        public float Pitch;
        public float Pan;
        public bool Is3D;
        public float Time;
        public float Duration;
    }
}
