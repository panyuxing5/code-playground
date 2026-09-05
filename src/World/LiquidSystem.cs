using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class LiquidSystem
    {
        private readonly WorldManager world;
        private readonly Queue<Vector3i> updateQueue;
        private readonly HashSet<Vector3i> queuedBlocks;
        private readonly object queueLock = new object();

        // 液体参数
        public int MaxFlowDepth { get; set; } = 7;
        public int FlowSpeed { get; set; } = 5; // ticks per flow
        public float WaterSpread { get; set; } = 1.0f;
        public float LavaSpread { get; set; } = 0.5f;

        // 更新统计
        public int UpdatedBlocks { get; private set; }
        public int QueueSize => updateQueue.Count;

        public LiquidSystem(WorldManager world)
        {
            this.world = world;
            updateQueue = new Queue<Vector3i>();
            queuedBlocks = new HashSet<Vector3i>();
        }

        public void Initialize()
        {
            Console.WriteLine("[LiquidSystem] 液体系统初始化完成");
        }

        public void Update()
        {
            UpdatedBlocks = 0;

            // 处理一定数量的液体更新
            int maxUpdates = 100;
            for (int i = 0; i < maxUpdates && updateQueue.Count > 0; i++)
            {
                Vector3i pos;
                lock (queueLock)
                {
                    pos = updateQueue.Dequeue();
                    queuedBlocks.Remove(pos);
                }

                UpdateLiquid(pos.X, pos.Y, pos.Z);
                UpdatedBlocks++;
            }
        }

        public void ScheduleUpdate(int x, int y, int z)
        {
            Vector3i pos = new Vector3i(x, y, z);
            lock (queueLock)
            {
                if (!queuedBlocks.Contains(pos))
                {
                    updateQueue.Enqueue(pos);
                    queuedBlocks.Add(pos);
                }
            }
        }

        public void ScheduleNeighborUpdates(int x, int y, int z)
        {
            ScheduleUpdate(x + 1, y, z);
            ScheduleUpdate(x - 1, y, z);
            ScheduleUpdate(x, y + 1, z);
            ScheduleUpdate(x, y - 1, z);
            ScheduleUpdate(x, y, z + 1);
            ScheduleUpdate(x, y, z - 1);
        }

        private void UpdateLiquid(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);

            if (block == GameConstants.BLOCK_WATER_STILL || block == GameConstants.BLOCK_WATER_FLOWING)
            {
                UpdateWater(x, y, z);
            }
            else if (block == GameConstants.BLOCK_LAVA_STILL || block == GameConstants.BLOCK_LAVA_FLOWING)
            {
                UpdateLava(x, y, z);
            }
        }

        private void UpdateWater(int x, int y, int z)
        {
            // 检查下方是否可以流动
            ushort below = world.GetBlock(x, y - 1, z);
            if (below == GameConstants.BLOCK_AIR)
            {
                // 向下流动
                world.SetBlock(x, y - 1, z, GameConstants.BLOCK_WATER_FLOWING);
                ScheduleUpdate(x, y - 1, z);
                return;
            }

            // 水平流动
            int currentLevel = GetLiquidLevel(x, y, z);
            if (currentLevel <= 0) return;

            int nextLevel = currentLevel - 1;
            if (nextLevel < 0) return;

            // 尝试向四个方向流动
            TryFlowTo(x + 1, y, z, nextLevel, GameConstants.BLOCK_WATER_FLOWING);
            TryFlowTo(x - 1, y, z, nextLevel, GameConstants.BLOCK_WATER_FLOWING);
            TryFlowTo(x, y, z + 1, nextLevel, GameConstants.BLOCK_WATER_FLOWING);
            TryFlowTo(x, y, z - 1, nextLevel, GameConstants.BLOCK_WATER_FLOWING);
        }

        private void UpdateLava(int x, int y, int z)
        {
            // 熔岩流动更慢
            // 检查下方
            ushort below = world.GetBlock(x, y - 1, z);
            if (below == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y - 1, z, GameConstants.BLOCK_LAVA_FLOWING);
                ScheduleUpdate(x, y - 1, z);
                return;
            }

            int currentLevel = GetLiquidLevel(x, y, z);
            if (currentLevel <= 0) return;

            int nextLevel = currentLevel - 1;
            if (nextLevel < 0) return;

            // 熔岩流动距离更短
            if (nextLevel > 3) return;

            TryFlowTo(x + 1, y, z, nextLevel, GameConstants.BLOCK_LAVA_FLOWING);
            TryFlowTo(x - 1, y, z, nextLevel, GameConstants.BLOCK_LAVA_FLOWING);
            TryFlowTo(x, y, z + 1, nextLevel, GameConstants.BLOCK_LAVA_FLOWING);
            TryFlowTo(x, y, z - 1, nextLevel, GameConstants.BLOCK_LAVA_FLOWING);

            // 熔岩遇水生成石头/圆石/黑曜石
            CheckLavaWaterInteraction(x, y, z);
        }

        private void TryFlowTo(int x, int y, int z, int level, ushort liquidType)
        {
            if (x < 0 || z < 0 || y < 0 || y >= GameConstants.CHUNK_HEIGHT) return;

            ushort target = world.GetBlock(x, y, z);

            if (target == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y, z, liquidType);
                SetLiquidLevel(x, y, z, level);
                ScheduleUpdate(x, y, z);
            }
            else if (IsLiquid(target) && GetLiquidLevel(x, y, z) < level)
            {
                SetLiquidLevel(x, y, z, level);
                ScheduleUpdate(x, y, z);
            }
        }

        private void CheckLavaWaterInteraction(int x, int y, int z)
        {
            // 检查周围是否有水
            ushort[] neighbors = {
                world.GetBlock(x + 1, y, z),
                world.GetBlock(x - 1, y, z),
                world.GetBlock(x, y, z + 1),
                world.GetBlock(x, y, z - 1),
                world.GetBlock(x, y + 1, z)
            };

            foreach (ushort neighbor in neighbors)
            {
                if (neighbor == GameConstants.BLOCK_WATER_STILL || neighbor == GameConstants.BLOCK_WATER_FLOWING)
                {
                    // 熔岩遇水变成圆石
                    world.SetBlock(x, y, z, GameConstants.BLOCK_COBBLESTONE);
                    ScheduleNeighborUpdates(x, y, z);
                    return;
                }
            }

            // 熔岩上方有水 -> 黑曜石
            ushort above = world.GetBlock(x, y + 1, z);
            if (above == GameConstants.BLOCK_WATER_STILL || above == GameConstants.BLOCK_WATER_FLOWING)
            {
                world.SetBlock(x, y, z, GameConstants.BLOCK_OBSIDIAN);
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        public int GetLiquidLevel(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);
            if (block == GameConstants.BLOCK_WATER_STILL || block == GameConstants.BLOCK_LAVA_STILL)
            {
                return MaxFlowDepth;
            }
            if (block == GameConstants.BLOCK_WATER_FLOWING || block == GameConstants.BLOCK_LAVA_FLOWING)
            {
                // 简化：使用元数据存储等级
                // 实际应该从block state获取
                return MaxFlowDepth / 2;
            }
            return -1;
        }

        public void SetLiquidLevel(int x, int y, int z, int level)
        {
            // 简化：不存储具体等级
        }

        public bool IsLiquid(ushort block)
        {
            return block == GameConstants.BLOCK_WATER_STILL ||
                   block == GameConstants.BLOCK_WATER_FLOWING ||
                   block == GameConstants.BLOCK_LAVA_STILL ||
                   block == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public bool IsWater(ushort block)
        {
            return block == GameConstants.BLOCK_WATER_STILL || block == GameConstants.BLOCK_WATER_FLOWING;
        }

        public bool IsLava(ushort block)
        {
            return block == GameConstants.BLOCK_LAVA_STILL || block == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public void ClearQueue()
        {
            lock (queueLock)
            {
                updateQueue.Clear();
                queuedBlocks.Clear();
            }
        }
    }
}
