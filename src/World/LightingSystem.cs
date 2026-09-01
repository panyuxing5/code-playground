using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class LightingSystem
    {
        private readonly WorldManager world;
        private readonly Queue<LightUpdate> lightUpdates;
        private readonly Queue<LightUpdate> skyLightUpdates;

        // 光照参数
        public int MaxLightLevel { get; set; } = 15;
        public int SkyLightAttenuation { get; set; } = 1;
        public int BlockLightAttenuation { get; set; } = 1;
        public bool EnableSmoothLighting { get; set; } = true;

        // 统计
        public int PendingLightUpdates => lightUpdates.Count;
        public int PendingSkyLightUpdates => skyLightUpdates.Count;
        public int TotalLightUpdatesProcessed { get; private set; }

        public LightingSystem(WorldManager world)
        {
            this.world = world;
            lightUpdates = new Queue<LightUpdate>();
            skyLightUpdates = new Queue<LightUpdate>();
        }

        public void Initialize()
        {
            Console.WriteLine("[LightingSystem] 光照系统初始化完成");
        }

        public void Update()
        {
            // 处理方块光照更新
            int maxUpdatesPerFrame = 100;
            int updatesProcessed = 0;

            while (lightUpdates.Count > 0 && updatesProcessed < maxUpdatesPerFrame)
            {
                LightUpdate update = lightUpdates.Dequeue();
                ProcessBlockLightUpdate(update);
                updatesProcessed++;
                TotalLightUpdatesProcessed++;
            }

            // 处理天空光照更新
            updatesProcessed = 0;
            while (skyLightUpdates.Count > 0 && updatesProcessed < maxUpdatesPerFrame)
            {
                LightUpdate update = skyLightUpdates.Dequeue();
                ProcessSkyLightUpdate(update);
                updatesProcessed++;
                TotalLightUpdatesProcessed++;
            }
        }

        public void UpdateBlockLight(int x, int y, int z)
        {
            // 获取方块发出的光照
            ushort block = world.GetBlock(x, y, z);
            int emittedLight = GetBlockEmittedLight(block);

            if (emittedLight > 0)
            {
                // 从光源开始传播
                lightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = emittedLight,
                    IsRemoval = false
                });
            }
            else
            {
                // 移除光照
                lightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = 0,
                    IsRemoval = true
                });
            }
        }

        public void UpdateSkyLight(int x, int y, int z)
        {
            // 检查是否能看到天空
            if (CanSeeSky(x, y, z))
            {
                skyLightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = MaxLightLevel,
                    IsRemoval = false
                });
            }
            else
            {
                skyLightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = 0,
                    IsRemoval = true
                });
            }
        }

        private void ProcessBlockLightUpdate(LightUpdate update)
        {
            if (update.Y < 0 || update.Y >= GameConstants.WORLD_HEIGHT) return;

            int currentLight = world.GetBlockLight(update.X, update.Y, update.Z);

            if (update.IsRemoval)
            {
                // 移除光照：需要重新计算周围的光照
                if (currentLight > 0)
                {
                    world.SetBlockLight(update.X, update.Y, update.Z, 0);

                    // 传播移除到邻居
                    PropagateLightRemoval(update.X, update.Y, update.Z, currentLight);
                }
            }
            else
            {
                // 添加光照
                if (update.LightLevel > currentLight)
                {
                    world.SetBlockLight(update.X, update.Y, update.Z, update.LightLevel);

                    // 传播到邻居
                    int newLightLevel = update.LightLevel - GetLightAttenuation(update.X, update.Y, update.Z);
                    if (newLightLevel > 0)
                    {
                        PropagateLight(update.X + 1, update.Y, update.Z, newLightLevel);
                        PropagateLight(update.X - 1, update.Y, update.Z, newLightLevel);
                        PropagateLight(update.X, update.Y + 1, update.Z, newLightLevel);
                        PropagateLight(update.X, update.Y - 1, update.Z, newLightLevel);
                        PropagateLight(update.X, update.Y, update.Z + 1, newLightLevel);
                        PropagateLight(update.X, update.Y, update.Z - 1, newLightLevel);
                    }
                }
            }
        }

        private void ProcessSkyLightUpdate(LightUpdate update)
        {
            if (update.Y < 0 || update.Y >= GameConstants.WORLD_HEIGHT) return;

            int currentLight = world.GetSkyLight(update.X, update.Y, update.Z);

            if (update.IsRemoval)
            {
                if (currentLight > 0)
                {
                    world.SetSkyLight(update.X, update.Y, update.Z, 0);
                    PropagateSkyLightRemoval(update.X, update.Y, update.Z, currentLight);
                }
            }
            else
            {
                if (update.LightLevel > currentLight)
                {
                    world.SetSkyLight(update.X, update.Y, update.Z, update.LightLevel);

                    // 天空光向下传播不衰减
                    if (update.Y > 0)
                    {
                        PropagateSkyLight(update.X, update.Y - 1, update.Z, update.LightLevel);
                    }

                    // 水平传播衰减
                    int newLightLevel = update.LightLevel - SkyLightAttenuation;
                    if (newLightLevel > 0)
                    {
                        PropagateSkyLight(update.X + 1, update.Y, update.Z, newLightLevel);
                        PropagateSkyLight(update.X - 1, update.Y, update.Z, newLightLevel);
                        PropagateSkyLight(update.X, update.Y, update.Z + 1, newLightLevel);
                        PropagateSkyLight(update.X, update.Y, update.Z - 1, newLightLevel);
                    }
                }
            }
        }

        private void PropagateLight(int x, int y, int z, int lightLevel)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;
            if (lightLevel <= 0) return;

            int currentLight = world.GetBlockLight(x, y, z);
            if (lightLevel > currentLight)
            {
                lightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = lightLevel,
                    IsRemoval = false
                });
            }
        }

        private void PropagateLightRemoval(int x, int y, int z, int removedLight)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            // 检查6个方向的邻居
            CheckNeighborForLightRemoval(x + 1, y, z, removedLight);
            CheckNeighborForLightRemoval(x - 1, y, z, removedLight);
            CheckNeighborForLightRemoval(x, y + 1, z, removedLight);
            CheckNeighborForLightRemoval(x, y - 1, z, removedLight);
            CheckNeighborForLightRemoval(x, y, z + 1, removedLight);
            CheckNeighborForLightRemoval(x, y, z - 1, removedLight);
        }

        private void CheckNeighborForLightRemoval(int x, int y, int z, int removedLight)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            int neighborLight = world.GetBlockLight(x, y, z);
            if (neighborLight > 0 && neighborLight < removedLight)
            {
                // 这个邻居的光照来自被移除的光源，需要移除
                world.SetBlockLight(x, y, z, 0);
                PropagateLightRemoval(x, y, z, neighborLight);
            }
            else if (neighborLight >= removedLight)
            {
                // 这个邻居有自己的光源或更强的光源，重新传播
                lightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = neighborLight,
                    IsRemoval = false
                });
            }
        }

        private void PropagateSkyLight(int x, int y, int z, int lightLevel)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;
            if (lightLevel <= 0) return;

            int currentLight = world.GetSkyLight(x, y, z);
            if (lightLevel > currentLight)
            {
                skyLightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = lightLevel,
                    IsRemoval = false
                });
            }
        }

        private void PropagateSkyLightRemoval(int x, int y, int z, int removedLight)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            CheckNeighborForSkyLightRemoval(x + 1, y, z, removedLight);
            CheckNeighborForSkyLightRemoval(x - 1, y, z, removedLight);
            CheckNeighborForSkyLightRemoval(x, y + 1, z, removedLight);
            CheckNeighborForSkyLightRemoval(x, y - 1, z, removedLight);
            CheckNeighborForSkyLightRemoval(x, y, z + 1, removedLight);
            CheckNeighborForSkyLightRemoval(x, y, z - 1, removedLight);
        }

        private void CheckNeighborForSkyLightRemoval(int x, int y, int z, int removedLight)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            int neighborLight = world.GetSkyLight(x, y, z);
            if (neighborLight > 0 && neighborLight < removedLight)
            {
                world.SetSkyLight(x, y, z, 0);
                PropagateSkyLightRemoval(x, y, z, neighborLight);
            }
            else if (neighborLight >= removedLight)
            {
                skyLightUpdates.Enqueue(new LightUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    LightLevel = neighborLight,
                    IsRemoval = false
                });
            }
        }

        private int GetLightAttenuation(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);

            // 透明方块衰减少
            if (IsTransparent(block))
            {
                return BlockLightAttenuation;
            }

            // 不透明方块衰减大
            return 15;
        }

        private bool IsTransparent(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_AIR ||
                   blockId == GameConstants.BLOCK_GLASS ||
                   blockId == GameConstants.BLOCK_WATER_STILL ||
                   blockId == GameConstants.BLOCK_WATER_FLOWING ||
                   blockId == GameConstants.BLOCK_LEAVES ||
                   blockId == GameConstants.BLOCK_ICE ||
                   blockId == GameConstants.BLOCK_GLOWSTONE;
        }

        private int GetBlockEmittedLight(ushort blockId)
        {
            switch (blockId)
            {
                case GameConstants.BLOCK_TORCH:
                case GameConstants.BLOCK_WALL_TORCH:
                    return 14;

                case GameConstants.BLOCK_GLOWSTONE:
                    return 15;

                case GameConstants.BLOCK_JACK_O_LANTERN:
                case GameConstants.BLOCK_LANTERN:
                    return 15;

                case GameConstants.BLOCK_LAVA_STILL:
                case GameConstants.BLOCK_LAVA_FLOWING:
                    return 15;

                case GameConstants.BLOCK_FIRE:
                    return 15;

                case GameConstants.BLOCK_SEA_LANTERN:
                    return 15;

                case GameConstants.BLOCK_SHROOMLIGHT:
                    return 15;

                case GameConstants.BLOCK_CAMPFIRE:
                    return 15;

                case GameConstants.BLOCK_SOUL_CAMPFIRE:
                    return 10;

                case GameConstants.BLOCK_SOUL_LANTERN:
                case GameConstants.BLOCK_SOUL_TORCH:
                    return 10;

                case GameConstants.BLOCK_END_ROD:
                    return 14;

                case GameConstants.BLOCK_REDSTONE_LAMP_ON:
                    return 15;

                case GameConstants.BLOCK_CONDUIT:
                    return 15;

                case GameConstants.BLOCK_BEACON:
                    return 15;

                case GameConstants.BLOCK_AMETHYST_CLUSTER:
                    return 5;

                case GameConstants.BLOCK_SCULK_CATALYST:
                    return 6;

                case GameConstants.BLOCK_SCULK_SHRIEKER:
                    return 6;

                case GameConstants.BLOCK_ENCHANTING_TABLE:
                    return 7;

                case GameConstants.BLOCK_BOOKSHELF:
                    return 3;

                case GameConstants.BLOCK_END_PORTAL_FRAME:
                    return 1;

                case GameConstants.BLOCK_NETHER_PORTAL:
                    return 11;

                case GameConstants.BLOCK_MAGMA_BLOCK:
                    return 3;

                case GameConstants.BLOCK_BREWING_STAND:
                    return 1;

                case GameConstants.BLOCK_DRAGON_EGG:
                    return 1;

                case GameConstants.BLOCK_KELP:
                    return 0;

                default:
                    return 0;
            }
        }

        private bool CanSeeSky(int x, int y, int z)
        {
            // 从当前位置向上检查是否有不透明方块
            for (int checkY = y + 1; checkY < GameConstants.WORLD_HEIGHT; checkY++)
            {
                ushort block = world.GetBlock(x, checkY, z);
                if (block != GameConstants.BLOCK_AIR && !IsTransparent(block))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetCombinedLight(int x, int y, int z)
        {
            int blockLight = world.GetBlockLight(x, y, z);
            int skyLight = world.GetSkyLight(x, y, z);

            // 取最大值
            return Math.Max(blockLight, skyLight);
        }

        public float GetLightLevel(int x, int y, int z)
        {
            int combinedLight = GetCombinedLight(x, y, z);
            return combinedLight / (float)MaxLightLevel;
        }

        public Vector3 GetLightColor(int x, int y, int z, int worldTime)
        {
            int blockLight = world.GetBlockLight(x, y, z);
            int skyLight = world.GetSkyLight(x, y, z);

            // 方块光是暖色调
            Vector3 blockLightColor = new Vector3(1.0f, 0.9f, 0.7f) * (blockLight / 15.0f);

            // 天空光根据时间变化
            float dayFactor = GetDaylightFactor(worldTime);
            Vector3 skyLightColor = new Vector3(
                0.8f + 0.2f * dayFactor,
                0.85f + 0.15f * dayFactor,
                1.0f
            ) * (skyLight / 15.0f) * (0.3f + 0.7f * dayFactor);

            // 合并
            Vector3 combined = blockLightColor + skyLightColor;
            return Vector3.ComponentMin(combined, Vector3.One);
        }

        private float GetDaylightFactor(int worldTime)
        {
            int timeOfDay = worldTime % 24000;

            if (timeOfDay >= 1000 && timeOfDay <= 11000)
            {
                // 白天
                return 1.0f;
            }
            else if (timeOfDay > 11000 && timeOfDay < 13000)
            {
                // 日落
                return 1.0f - (timeOfDay - 11000) / 2000.0f;
            }
            else if (timeOfDay >= 13000 && timeOfDay <= 23000)
            {
                // 夜晚
                return 0.0f;
            }
            else
            {
                // 日出
                if (timeOfDay > 23000)
                {
                    return (timeOfDay - 23000) / 1000.0f;
                }
                else
                {
                    return timeOfDay / 1000.0f;
                }
            }
        }

        public void RecalculateChunkLighting(int chunkX, int chunkZ)
        {
            // 重新计算整个区块的光照
            int startX = chunkX * GameConstants.CHUNK_SIZE;
            int startZ = chunkZ * GameConstants.CHUNK_SIZE;

            // 先清除所有光照
            for (int x = startX; x < startX + GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = startZ; z < startZ + GameConstants.CHUNK_SIZE; z++)
                {
                    for (int y = 0; y < GameConstants.WORLD_HEIGHT; y++)
                    {
                        world.SetBlockLight(x, y, z, 0);
                        world.SetSkyLight(x, y, z, 0);
                    }
                }
            }

            // 重新计算天空光
            for (int x = startX; x < startX + GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = startZ; z < startZ + GameConstants.CHUNK_SIZE; z++)
                {
                    for (int y = GameConstants.WORLD_HEIGHT - 1; y >= 0; y--)
                    {
                        if (CanSeeSky(x, y, z))
                        {
                            world.SetSkyLight(x, y, z, MaxLightLevel);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // 重新计算方块光
            for (int x = startX; x < startX + GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = startZ; z < startZ + GameConstants.CHUNK_SIZE; z++)
                {
                    for (int y = 0; y < GameConstants.WORLD_HEIGHT; y++)
                    {
                        ushort block = world.GetBlock(x, y, z);
                        int emittedLight = GetBlockEmittedLight(block);
                        if (emittedLight > 0)
                        {
                            UpdateBlockLight(x, y, z);
                        }
                    }
                }
            }
        }

        public void ClearPendingUpdates()
        {
            lightUpdates.Clear();
            skyLightUpdates.Clear();
        }

        public void ResetStats()
        {
            TotalLightUpdatesProcessed = 0;
        }
    }

    public struct LightUpdate
    {
        public int X;
        public int Y;
        public int Z;
        public int LightLevel;
        public bool IsRemoval;
    }
}
