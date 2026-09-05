using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.World;
using VoxelCraft.Render;
using VoxelCraft.Player;
using VoxelCraft.Entities;
using VoxelCraft.UI;
using VoxelCraft.Save;
using VoxelCraft.Audio;
using VoxelCraft.Items;

namespace VoxelCraft.Core
{
    public class GameEngine : GameWindow
    {
        // ========================================
        // 单例实例
        // ========================================
        public static GameEngine Instance { get; private set; }

        // ========================================
        // 游戏状态
        // ========================================
        public enum GameState
        {
            MainMenu,
            Loading,
            Playing,
            Paused,
            Inventory,
            Crafting,
            Chest,
            Settings,
            GameOver,
            Debug
        }

        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public GameState PreviousState { get; private set; } = GameState.MainMenu;

        // ========================================
        // 时间管理
        // ========================================
        public float DeltaTime { get; private set; }
        public float UnscaledDeltaTime { get; private set; }
        public float TimeScale { get; set; } = 1.0f;
        public float TotalTime { get; private set; }
        public int FrameCount { get; private set; }
        public int FPS { get; private set; }
        private float fpsTimer = 0f;
        private int fpsCounter = 0;
        public float UPS { get; private set; }
        private float upsTimer = 0f;
        private int upsCounter = 0;

        // ========================================
        // 游戏世界时间
        // ========================================
        public long WorldTime { get; private set; } = 0;
        public float WorldTimeFloat { get; private set; } = 0f;
        public int DayCount { get; private set; } = 0;
        public bool IsDaytime => WorldTime % GameConstants.DAY_LENGTH_TICKS < GameConstants.SUNSET_TICK;
        public bool IsNight => !IsDaytime;
        public float DayProgress => (float)(WorldTime % GameConstants.DAY_LENGTH_TICKS) / GameConstants.DAY_LENGTH_TICKS;
        public long DayTime
        {
            get => WorldTime % GameConstants.DAY_LENGTH_TICKS;
            set => WorldTime = value;
        }
        public new float Time
        {
            get => TotalTime;
            set { }
        }

        // ========================================
        // 核心系统
        // ========================================
        public InputManager Input { get; private set; }
        public Camera MainCamera { get; private set; }
        public ShaderManager Shaders { get; private set; }
        public TextureManager Textures { get; private set; }
        public WorldManager World { get; private set; }
        public ChunkManager Chunks { get; private set; }
        public ItemRegistry Items { get; private set; }
        public CraftingSystem Crafting { get; private set; }
        public PlayerController Player { get; private set; }
        public EntityManager Entities { get; private set; }
        public UIManager UI { get; private set; }
        public SaveManager SaveSystem { get; private set; }
        public SoundManager Sounds { get; private set; }
        public ParticleSystem Particles { get; private set; }
        public Profiler Profiler { get; private set; }

        // ========================================
        // 世界设置
        // ========================================
        public long WorldSeed { get; private set; } = GameConstants.WORLD_SEED_DEFAULT;
        public string WorldName { get; private set; } = "New World";
        public GameMode CurrentGameMode { get; private set; } = GameMode.Creative;
        public Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;
        public bool IsPaused { get; private set; } = false;
        public bool ShowDebugInfo { get; set; } = GameConstants.DEBUG_MODE;
        public bool IsFirstPerson { get; private set; } = true;

        // ========================================
        // 渲染设置
        // ========================================
        public int RenderDistance { get; set; } = GameConstants.RENDER_DISTANCE;
        public bool VsyncEnabled { get; set; } = GameConstants.VSYNC_ENABLED;
        public float FOV { get; set; } = GameConstants.FOV_DEFAULT;
        public float MouseSensitivity { get; set; } = GameConstants.MOUSE_SENSITIVITY_DEFAULT;
        public bool UseFancyGraphics { get; set; } = true;
        public bool UseSmoothLighting { get; set; } = true;
        public bool UseClouds { get; set; } = true;
        public bool UseParticles { get; set; } = true;
        public bool UseAmbientOcclusion { get; set; } = true;

        // ========================================
        // 线程管理
        // ========================================
        private Thread chunkLoadThread;
        private Thread worldGenThread;
        private volatile bool isRunning = true;
        private readonly object worldLock = new object();

        // ========================================
        // 性能统计
        // ========================================
        public int ChunksLoaded { get; private set; }
        public int ChunksRendered { get; private set; }
        public int EntitiesRendered { get; private set; }
        public int ParticlesActive { get; private set; }
        public long MemoryUsed { get; private set; }
        public float FrameTimeMs { get; private set; }
        public float UpdateTimeMs { get; private set; }
        public float RenderTimeMs { get; private set; }

        // ========================================
        // 事件
        // ========================================
        public event Action OnGameStart;
        public event Action OnGamePause;
        public event Action OnGameResume;
        public event Action OnGameQuit;
        public event Action<GameState, GameState> OnStateChanged;
        public event Action<long> OnWorldTimeChange;
        public event Action OnDayStart;
        public event Action OnNightStart;

        // ========================================
        // 构造函数
        // ========================================
        public GameEngine() : base(CreateGameWindowSettings(), CreateNativeWindowSettings())
        {
            Instance = this;
            Console.WriteLine("[GameEngine] 初始化游戏引擎...");
        }

        private static GameWindowSettings CreateGameWindowSettings()
        {
            return new GameWindowSettings
            {
                UpdateFrequency = GameConstants.TARGET_UPS
            };
        }

        private static NativeWindowSettings CreateNativeWindowSettings()
        {
            return new NativeWindowSettings
            {
                Size = new Vector2i(GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT),
                Title = GameConstants.WINDOW_TITLE,
                WindowBorder = WindowBorder.Resizable,
                WindowState = WindowState.Normal,
                Vsync = GameConstants.VSYNC_ENABLED ? VSyncMode.On : VSyncMode.Off,
                API = ContextAPI.OpenGL,
                Profile = ContextProfile.Core,
                APIVersion = new Version(4, 5),
                NumberOfSamples = GameConstants.MSAA_SAMPLES
            };
        }

        // ========================================
        // 初始化
        // ========================================
        protected override void OnLoad()
        {
            base.OnLoad();
            Console.WriteLine("[GameEngine] OnLoad - 加载游戏资源...");

            try
            {
                InitializeOpenGL();
                InitializeCoreSystems();
                InitializeWorld();
                InitializePlayer();
                InitializeUI();
                InitializeAudio();

                CurrentState = GameState.MainMenu;
                Console.WriteLine("[GameEngine] 游戏引擎初始化完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] 初始化失败: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        private void InitializeOpenGL()
        {
            Console.WriteLine("[GameEngine] 初始化 OpenGL...");

            GL.ClearColor(0.5f, 0.7f, 1.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            if (GameConstants.USE_MIPMAP)
            {
                GL.Enable(EnableCap.Texture2D);
            }

            int maxTextureSize;
            GL.GetInteger(GetPName.MaxTextureSize, out maxTextureSize);
            Console.WriteLine($"[GameEngine] 最大纹理尺寸: {maxTextureSize}");

            int maxTextureUnits;
            GL.GetInteger(GetPName.MaxTextureImageUnits, out maxTextureUnits);
            Console.WriteLine($"[GameEngine] 最大纹理单元: {maxTextureUnits}");

            string version = GL.GetString(StringName.Version);
            Console.WriteLine($"[GameEngine] OpenGL 版本: {version}");

            string renderer = GL.GetString(StringName.Renderer);
            Console.WriteLine($"[GameEngine] 渲染器: {renderer}");

            string vendor = GL.GetString(StringName.Vendor);
            Console.WriteLine($"[GameEngine] 厂商: {vendor}");

            string glslVersion = GL.GetString(StringName.ShadingLanguageVersion);
            Console.WriteLine($"[GameEngine] GLSL 版本: {glslVersion}");
        }

        private void InitializeCoreSystems()
        {
            Console.WriteLine("[GameEngine] 初始化核心系统...");

            Input = new InputManager(this);
            MainCamera = new Camera();
            Shaders = new ShaderManager("assets/shaders");
            Textures = new TextureManager();
            Particles = new ParticleSystem();
            Profiler = Profiler.Instance;

            BlockRegistry.RegisterAllBlocks();
            ItemRegistry.Initialize();
            CraftingSystem.Initialize();
            Shaders.Initialize();
            Textures.LoadAllTextures();

            Console.WriteLine("[GameEngine] 核心系统初始化完成");
        }

        private void InitializeWorld()
        {
            Console.WriteLine("[GameEngine] 初始化世界...");

            World = new WorldManager(WorldSeed);
            Chunks = new ChunkManager(World);
            Entities = new EntityManager();
            SaveSystem = new SaveManager();

            World.Initialize();
            Chunks.Initialize();
            Entities.Initialize();

            StartChunkLoadingThreads();

            Console.WriteLine("[GameEngine] 世界初始化完成");
        }

        private void InitializePlayer()
        {
            Console.WriteLine("[GameEngine] 初始化玩家...");

            Player = new PlayerController();
            Player.Initialize();

            // 设置玩家出生点
            float spawnX = 0.5f;
            float spawnY = GameConstants.SEA_LEVEL + 10;
            float spawnZ = 0.5f;
            Player.SetPosition(spawnX, spawnY, spawnZ);

            MainCamera.Position = new Vector3(spawnX, spawnY + GameConstants.PLAYER_EYE_HEIGHT, spawnZ);

            Console.WriteLine($"[GameEngine] 玩家出生点: ({spawnX}, {spawnY}, {spawnZ})");
        }

        private void InitializeUI()
        {
            Console.WriteLine("[GameEngine] 初始化 UI...");

            UI = new UIManager();
            UI.Initialize();
            UI.ShowMainMenu(true);

            Console.WriteLine("[GameEngine] UI 初始化完成");
        }

        private void InitializeAudio()
        {
            Console.WriteLine("[GameEngine] 初始化音频系统...");

            try
            {
                Sounds = new SoundManager();
                Sounds.Initialize();
                Console.WriteLine("[GameEngine] 音频系统初始化完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] 音频系统初始化失败（将继续无音频运行）: {ex.Message}");
            }
        }

        private void StartChunkLoadingThreads()
        {
            isRunning = true;

            chunkLoadThread = new Thread(ChunkLoadLoop)
            {
                Name = "ChunkLoadThread",
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal
            };
            chunkLoadThread.Start();

            worldGenThread = new Thread(WorldGenLoop)
            {
                Name = "WorldGenThread",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            worldGenThread.Start();

            Console.WriteLine("[GameEngine] 区块加载线程已启动");
        }

        // ========================================
        // 游戏状态管理
        // ========================================
        public void ChangeState(GameState newState)
        {
            if (newState == CurrentState) return;

            PreviousState = CurrentState;
            GameState oldState = CurrentState;
            CurrentState = newState;

            Console.WriteLine($"[GameEngine] 状态改变: {oldState} -> {newState}");

            switch (newState)
            {
                case GameState.Playing:
                    CursorGrabbed = true;
                    CursorVisible = false;
                    IsPaused = false;
                    break;
                case GameState.Paused:
                case GameState.Inventory:
                case GameState.Crafting:
                case GameState.Chest:
                case GameState.Settings:
                    CursorGrabbed = false;
                    CursorVisible = true;
                    IsPaused = true;
                    break;
                case GameState.MainMenu:
                    CursorGrabbed = false;
                    CursorVisible = true;
                    IsPaused = true;
                    break;
                case GameState.Loading:
                    CursorGrabbed = false;
                    CursorVisible = true;
                    IsPaused = true;
                    break;
            }

            OnStateChanged?.Invoke(oldState, newState);
        }

        public void StartNewWorld(string name, long seed, GameMode mode, Difficulty difficulty)
        {
            Console.WriteLine($"[GameEngine] 创建新世界: {name}, 种子: {seed}, 模式: {mode}");

            WorldName = name;
            WorldSeed = seed;
            CurrentGameMode = mode;
            CurrentDifficulty = difficulty;
            WorldTime = 0;
            DayCount = 0;

            lock (worldLock)
            {
                World.Regenerate(seed);
                Chunks.ClearAllChunks();
                Entities.ClearAllEntities();
            }

            Player.ResetForNewWorld();
            float spawnX = 0.5f;
            float spawnY = GameConstants.SEA_LEVEL + 10;
            float spawnZ = 0.5f;
            Player.SetPosition(spawnX, spawnY, spawnZ);

            ChangeState(GameState.Playing);
            OnGameStart?.Invoke();

            Console.WriteLine("[GameEngine] 新世界创建完成");
        }

        public void LoadWorld(string name)
        {
            Console.WriteLine($"[GameEngine] 加载世界: {name}");

            WorldName = name;
            ChangeState(GameState.Loading);

            try
            {
                SaveSystem.LoadWorld(name);
                ChangeState(GameState.Playing);
                OnGameStart?.Invoke();
                Console.WriteLine("[GameEngine] 世界加载完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] 世界加载失败: {ex.Message}");
                ChangeState(GameState.MainMenu);
            }
        }

        public void SaveCurrentWorld()
        {
            if (CurrentState != GameState.Playing) return;

            Console.WriteLine("[GameEngine] 保存世界...");
            try
            {
                SaveSystem.SaveWorld(WorldName);
                Console.WriteLine("[GameEngine] 世界保存完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameEngine] 世界保存失败: {ex.Message}");
            }
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;
            ChangeState(GameState.Paused);
            OnGamePause?.Invoke();
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;
            ChangeState(GameState.Playing);
            OnGameResume?.Invoke();
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (CurrentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        public void QuitToMainMenu()
        {
            SaveCurrentWorld();
            ChangeState(GameState.MainMenu);
        }

        public void QuitGame()
        {
            Console.WriteLine("[GameEngine] 退出游戏...");
            SaveCurrentWorld();
            OnGameQuit?.Invoke();
            isRunning = false;
            Close();
        }

        // ========================================
        // 更新循环
        // ========================================
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            UnscaledDeltaTime = (float)e.Time;
            DeltaTime = UnscaledDeltaTime * TimeScale;
            TotalTime += DeltaTime;

            // FPS 计算
            fpsCounter++;
            fpsTimer += UnscaledDeltaTime;
            if (fpsTimer >= 1.0f)
            {
                FPS = fpsCounter;
                fpsCounter = 0;
                fpsTimer = 0f;
            }

            // UPS 计算
            upsCounter++;
            upsTimer += UnscaledDeltaTime;
            if (upsTimer >= 1.0f)
            {
                UPS = upsCounter;
                upsCounter = 0;
                upsTimer = 0f;
            }

            var stopwatch = Stopwatch.StartNew();

            if (!IsPaused || CurrentState == GameState.Playing)
            {
                UpdateGame();
            }

            UpdateUI();

            stopwatch.Stop();
            UpdateTimeMs = (float)stopwatch.Elapsed.TotalMilliseconds;

            // 内存使用统计
            MemoryUsed = GC.GetTotalMemory(false);
        }

        private void UpdateGame()
        {
            if (CurrentState != GameState.Playing) return;

            // 更新世界时间
            WorldTimeFloat += DeltaTime * GameConstants.TICK_RATE;
            long newWorldTime = (long)WorldTimeFloat;
            if (newWorldTime != WorldTime)
            {
                WorldTime = newWorldTime;
                OnWorldTimeChange?.Invoke(WorldTime);

                // 检测昼夜变化
                long dayTick = WorldTime % GameConstants.DAY_LENGTH_TICKS;
                if (dayTick == GameConstants.DAY_START_TICK)
                {
                    DayCount++;
                    OnDayStart?.Invoke();
                }
                else if (dayTick == GameConstants.NIGHT_START_TICK)
                {
                    OnNightStart?.Invoke();
                }
            }

            // 更新输入
            Input.Update(DeltaTime);

            // 更新玩家
            Player.Update(DeltaTime);

            // 更新相机
            UpdateCamera();

            // 更新区块
            lock (worldLock)
            {
                Chunks.Update(DeltaTime);
                ChunksLoaded = Chunks.LoadedChunkCount;
            }

            // 更新实体
            Entities.Update(DeltaTime, Player);
            EntitiesRendered = Entities.EntityCount;

            // 更新粒子
            if (UseParticles)
            {
                Particles.Update(DeltaTime);
                ParticlesActive = Particles.ActiveParticleCount;
            }

            // 更新音频
            Sounds?.Update(DeltaTime);

            // 自动保存
            if (WorldTime % GameConstants.AUTOSAVE_INTERVAL_TICKS == 0 && WorldTime > 0)
            {
                SaveCurrentWorld();
            }

            // 性能分析
            if (GameConstants.ENABLE_PROFILER)
            {
                Profiler.Update(DeltaTime);
            }
        }

        private void UpdateCamera()
        {
            Vector3 playerPos = Player.Position;
            float eyeHeight = Player.IsSneaking ? GameConstants.PLAYER_SNEAK_EYE_HEIGHT : GameConstants.PLAYER_EYE_HEIGHT;

            if (IsFirstPerson)
            {
                MainCamera.Position = new Vector3(
                    playerPos.X,
                    playerPos.Y + eyeHeight,
                    playerPos.Z
                );
            }
            else
            {
                // 第三人称视角
                Vector3 offset = -MainCamera.Forward * 5.0f + Vector3.UnitY * 2.0f;
                MainCamera.Position = playerPos + offset + Vector3.UnitY * eyeHeight;
            }

            MainCamera.Yaw = Player.Yaw;
            MainCamera.Pitch = Player.Pitch;
            MainCamera.UpdateViewMatrix();
            MainCamera.UpdateProjectionMatrix(FOV, Size.X / (float)Size.Y, GameConstants.NEAR_PLANE, GameConstants.FAR_PLANE);
        }

        private void UpdateUI()
        {
            UI.Update(DeltaTime);

            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F3))
            {
                ShowDebugInfo = !ShowDebugInfo;
            }

            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F5))
            {
                IsFirstPerson = !IsFirstPerson;
            }

            if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                if (CurrentState == GameState.Playing)
                {
                    TogglePause();
                }
                else if (CurrentState == GameState.Paused)
                {
                    ResumeGame();
                }
                else if (CurrentState == GameState.Inventory || CurrentState == GameState.Crafting)
                {
                    ChangeState(GameState.Playing);
                }
            }

            if (CurrentState == GameState.Playing)
            {
                if (Input.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.E))
                {
                    ChangeState(GameState.Inventory);
                }
            }
        }

        // ========================================
        // 渲染循环
        // ========================================
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            FrameCount++;

            var stopwatch = Stopwatch.StartNew();

            Render();

            stopwatch.Stop();
            RenderTimeMs = (float)stopwatch.Elapsed.TotalMilliseconds;
            FrameTimeMs = UpdateTimeMs + RenderTimeMs;

            SwapBuffers();
        }

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (CurrentState == GameState.MainMenu)
            {
                RenderMainMenu();
                return;
            }

            if (CurrentState == GameState.Loading)
            {
                RenderLoadingScreen();
                return;
            }

            RenderWorld();
            RenderEntities();
            RenderParticles();
            RenderUI();

            if (ShowDebugInfo)
            {
                RenderDebugInfo();
            }
        }

        private void RenderMainMenu()
        {
            GL.ClearColor(0.2f, 0.3f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            UI.RenderMainMenu();
        }

        private void RenderLoadingScreen()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            UI.RenderLoadingScreen("加载中...", 0.5f);
        }

        private void RenderWorld()
        {
            // 更新天空颜色
            UpdateSkyColor();

            lock (worldLock)
            {
                Chunks.Render(MainCamera);
                ChunksRendered = Chunks.RenderedChunkCount;
            }
        }

        private void UpdateSkyColor()
        {
            float dayProgress = DayProgress;
            Vector3 skyColor;

            if (dayProgress < 0.25f) // 早晨
            {
                float t = dayProgress / 0.25f;
                skyColor = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.2f), new Vector3(0.5f, 0.7f, 1.0f), t);
            }
            else if (dayProgress < 0.5f) // 白天
            {
                skyColor = new Vector3(0.5f, 0.7f, 1.0f);
            }
            else if (dayProgress < 0.6f) // 黄昏
            {
                float t = (dayProgress - 0.5f) / 0.1f;
                skyColor = Vector3.Lerp(new Vector3(0.5f, 0.7f, 1.0f), new Vector3(0.8f, 0.4f, 0.2f), t);
            }
            else if (dayProgress < 0.75f) // 日落
            {
                float t = (dayProgress - 0.6f) / 0.15f;
                skyColor = Vector3.Lerp(new Vector3(0.8f, 0.4f, 0.2f), new Vector3(0.1f, 0.1f, 0.2f), t);
            }
            else // 夜晚
            {
                skyColor = new Vector3(0.05f, 0.05f, 0.15f);
            }

            GL.ClearColor(skyColor.X, skyColor.Y, skyColor.Z, 1.0f);
        }

        private void RenderEntities()
        {
            Entities.Render(MainCamera);
        }

        private void RenderParticles()
        {
            if (UseParticles)
            {
                Particles.Render(MainCamera);
            }
        }

        private void RenderUI()
        {
            UI.Render();
        }

        private void RenderDebugInfo()
        {
            UI.RenderDebugInfo();
        }

        // ========================================
        // 区块加载线程
        // ========================================
        private void ChunkLoadLoop()
        {
            while (isRunning)
            {
                try
                {
                    if (CurrentState == GameState.Playing)
                    {
                        Vector3 playerPos = Player.Position;
                        int playerChunkX = (int)Math.Floor(playerPos.X / GameConstants.CHUNK_SIZE);
                        int playerChunkZ = (int)Math.Floor(playerPos.Z / GameConstants.CHUNK_SIZE);

                        lock (worldLock)
                        {
                            Chunks.UpdateChunksAroundPlayer(playerChunkX, playerChunkZ, RenderDistance);
                        }
                    }
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ChunkLoadThread] 错误: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        private void WorldGenLoop()
        {
            while (isRunning)
            {
                try
                {
                    if (CurrentState == GameState.Playing)
                    {
                        lock (worldLock)
                        {
                            World.ProcessPendingGenerations();
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WorldGenThread] 错误: {ex.Message}");
                    Thread.Sleep(100);
                }
            }
        }

        // ========================================
        // 窗口事件
        // ========================================
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            Console.WriteLine($"[GameEngine] 窗口大小改变: {e.Width}x{e.Height}");
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            Console.WriteLine("[GameEngine] 卸载游戏资源...");

            isRunning = false;
            chunkLoadThread?.Join(1000);
            worldGenThread?.Join(1000);

            SaveCurrentWorld();

            Sounds?.Dispose();
            Shaders?.Dispose();
            Textures?.Dispose();
            Chunks?.Dispose();
            World?.Dispose();

            Console.WriteLine("[GameEngine] 资源卸载完成");
        }

        // ========================================
        // 工具方法
        // ========================================
        public bool IsChunkVisible(Vector3 chunkPosition)
        {
            return MainCamera.IsBoxInFrustum(
                chunkPosition,
                chunkPosition + new Vector3(GameConstants.CHUNK_SIZE, GameConstants.CHUNK_HEIGHT, GameConstants.CHUNK_SIZE)
            );
        }

        public Vector3 GetLookAtBlock()
        {
            Vector3i? block = Player.GetLookAtBlock();
            return block.HasValue ? new Vector3(block.Value.X, block.Value.Y, block.Value.Z) : Vector3.Zero;
        }

        public Vector3? GetRaycastHit(float maxDistance)
        {
            return Player.Raycast(maxDistance);
        }

        public void PlayBlockBreakSound(Vector3 position)
        {
            Sounds?.PlaySound("block.break", position);
        }

        public void PlayBlockPlaceSound(Vector3 position)
        {
            Sounds?.PlaySound("block.place", position);
        }

        public void PlayStepSound(string blockType)
        {
            Sounds?.PlaySound($"step.{blockType}", Player.Position);
        }

        // 扩展属性和方法
        public bool IsCursorVisible { get; set; } = true;
        public bool CursorGrabbed { get; set; }
        public bool CursorVisible { get; set; } = true;

        public void Stop()
        {
            // 停止游戏
        }
    }

    // ========================================
    // 游戏模式枚举
    // ========================================
    public enum GameMode
    {
        Survival,
        Creative,
        Adventure,
        Spectator,
        Hardcore
    }

    // ========================================
    // 难度枚举
    // ========================================
    public enum Difficulty
    {
        Peaceful,
        Easy,
        Normal,
        Hard
    }
}
