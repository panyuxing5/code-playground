using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Player;
using VoxelCraft.Entities;

namespace VoxelCraft.Core
{
    public class DebugScreen
    {
        private bool isVisible;
        private int fps;
        private int frameCount;
        private float fpsTimer;
        private readonly Stopwatch stopwatch;

        // 调试信息
        public string AdditionalInfo { get; set; } = "";

        public bool IsVisible => isVisible;

        public DebugScreen()
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void Initialize()
        {
            Console.WriteLine("[DebugScreen] 调试界面初始化完成");
        }

        public void Toggle()
        {
            isVisible = !isVisible;
        }

        public void Show()
        {
            isVisible = true;
        }

        public void Hide()
        {
            isVisible = false;
        }

        public void Update(float deltaTime)
        {
            frameCount++;
            fpsTimer += deltaTime;

            if (fpsTimer >= 1.0f)
            {
                fps = (int)(frameCount / fpsTimer);
                frameCount = 0;
                fpsTimer = 0;
            }
        }

        public void Render(UIManager uiManager, GameEngine gameEngine, WorldManager world,
            PlayerController player, ChunkManager chunkManager, EntityManager entityManager,
            ParticleSystem particleSystem)
        {
            if (!isVisible) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 左侧信息
            List<string> leftLines = new List<string>();

            // 游戏版本和FPS
            leftLines.Add("VoxelCraft 1.0.0 (C# + OpenTK)");
            leftLines.Add($"FPS: {fps} (最低: {gameEngine.Profiler.MinFps}, 最高: {gameEngine.Profiler.MaxFps})");
            leftLines.Add($"帧时间: {gameEngine.Profiler.LastFrameTime:F2}ms");
            leftLines.Add($"");

            // 玩家位置
            leftLines.Add($"XYZ: {player.Position.X:F2} / {player.Position.Y:F2} / {player.Position.Z:F2}");
            leftLines.Add($"区块: {player.ChunkX}, {player.ChunkZ}");
            leftLines.Add($"朝向: {player.Yaw:F1} / {player.Pitch:F1}");
            leftLines.Add($"速度: {player.Velocity.Length:F2}");
            leftLines.Add($"");

            // 朝向
            string facing = GetFacingDirection(player.Yaw);
            leftLines.Add($"朝向: {facing}");
            leftLines.Add($"");

            // 世界信息
            leftLines.Add($"世界时间: {world.WorldTime} ({GetTimeOfDay((int)world.WorldTime)})");
            leftLines.Add($"天气: {world.Weather}");
            leftLines.Add($"难度: {world.Difficulty}");
            leftLines.Add($"");

            // 区块信息
            leftLines.Add($"已加载区块: {chunkManager.LoadedChunks}");
            leftLines.Add($"渲染中区块: {chunkManager.RenderedChunks}");
            leftLines.Add($"待构建网格: {chunkManager.PendingMeshBuilds}");
            leftLines.Add($"");

            // 实体信息
            leftLines.Add($"实体数量: {entityManager.TotalEntities}");
            leftLines.Add($"粒子数量: {particleSystem.ActiveParticles}");
            leftLines.Add($"");

            // 内存信息
            long memoryUsed = GC.GetTotalMemory(false) / (1024 * 1024);
            leftLines.Add($"内存使用: {memoryUsed} MB");
            leftLines.Add($"GC 代数: {GC.MaxGeneration}");
            leftLines.Add($"");

            // 系统信息
            leftLines.Add($"操作系统: {Environment.OSVersion}");
            leftLines.Add($"处理器: {Environment.ProcessorCount} 核");
            leftLines.Add($"");

            // 渲染左侧
            int y = 10;
            foreach (string line in leftLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    // 半透明背景
                    int textWidth = uiManager.MeasureText(line, 12);
                    uiManager.DrawPanel(2, y - 1, textWidth + 4, 12,
                        new Color4(0.0f, 0.0f, 0.0f, 0.5f));
                    uiManager.DrawText(line, 4, y, 12, Color4.White);
                }
                y += 12;
            }

            // 右侧信息
            List<string> rightLines = new List<string>();

            // 目标方块信息
            if (player.TargetedBlock.HasValue)
            {
                Vector3i target = player.TargetedBlock.Value;
                ushort blockId = world.GetBlock(target.X, target.Y, target.Z);
                string blockName = BlockRegistry.GetBlockName(blockId);
                Biome biome = world.GetBiome(target.X, target.Z);

                rightLines.Add($"目标方块: {blockName} ({blockId})");
                rightLines.Add($"位置: {target.X}, {target.Y}, {target.Z}");
                rightLines.Add($"生物群系: {biome}");
                rightLines.Add($"光照: {world.GetBlockLight(target.X, target.Y, target.Z)}");
                rightLines.Add($"天空光照: {world.GetSkyLight(target.X, target.Y, target.Z)}");
                rightLines.Add($"");
            }

            // 性能统计
            rightLines.Add("=== 性能统计 ===");
            rightLines.Add($"渲染时间: {gameEngine.Profiler.GetCounter("render"):F2}ms");
            rightLines.Add($"更新时间: {gameEngine.Profiler.GetCounter("update"):F2}ms");
            rightLines.Add($"物理时间: {gameEngine.Profiler.GetCounter("physics"):F2}ms");
            rightLines.Add($"AI时间: {gameEngine.Profiler.GetCounter("ai"):F2}ms");
            rightLines.Add($"");

            // 游戏规则
            rightLines.Add("=== 游戏规则 ===");
            rightLines.Add($"昼夜循环: {(world.Rules.DoDaylightCycle ? "开" : "关")}");
            rightLines.Add($"天气循环: {(world.Rules.DoWeatherCycle ? "开" : "关")}");
            rightLines.Add($"生物生成: {(world.Rules.DoMobSpawning ? "开" : "关")}");
            rightLines.Add($"");

            // 玩家状态
            rightLines.Add("=== 玩家状态 ===");
            rightLines.Add($"生命值: {player.Health}/{player.MaxHealth}");
            rightLines.Add($"饥饿值: {player.Hunger}/{player.MaxHunger}");
            rightLines.Add($"经验值: {player.Experience} (等级 {player.ExperienceLevel})");
            rightLines.Add($"游戏模式: {player.GameMode}");
            rightLines.Add($"");

            // 额外信息
            if (!string.IsNullOrEmpty(AdditionalInfo))
            {
                rightLines.Add(AdditionalInfo);
            }

            // 渲染右侧
            y = 10;
            foreach (string line in rightLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    int textWidth = uiManager.MeasureText(line, 12);
                    int x = screenWidth - textWidth - 4;
                    uiManager.DrawPanel(x - 2, y - 1, textWidth + 4, 12,
                        new Color4(0.0f, 0.0f, 0.0f, 0.5f));
                    uiManager.DrawText(line, x, y, 12, Color4.White);
                }
                y += 12;
            }

            // 底部信息
            string bottomInfo = $"按 F3 切换调试界面 | 按 F3+Q 显示帮助";
            int bottomWidth = uiManager.MeasureText(bottomInfo, 12);
            uiManager.DrawText(bottomInfo, (screenWidth - bottomWidth) / 2, screenHeight - 20, 12,
                new Color4(0.7f, 0.7f, 0.7f, 1f));
        }

        private string GetFacingDirection(float yaw)
        {
            yaw = ((yaw % 360) + 360) % 360;

            if (yaw >= 315 || yaw < 45)
                return "南 (正Z)";
            else if (yaw >= 45 && yaw < 135)
                return "西 (负X)";
            else if (yaw >= 135 && yaw < 225)
                return "北 (负Z)";
            else
                return "东 (正X)";
        }

        private string GetTimeOfDay(int gameTime)
        {
            int timeOfDay = gameTime % 24000;

            if (timeOfDay >= 0 && timeOfDay < 1000)
                return "日出";
            else if (timeOfDay >= 1000 && timeOfDay < 11000)
                return "白天";
            else if (timeOfDay >= 11000 && timeOfDay < 13000)
                return "日落";
            else if (timeOfDay >= 13000 && timeOfDay < 23000)
                return "夜晚";
            else
                return "黎明";
        }

        public int GetFps()
        {
            return fps;
        }
    }
}
