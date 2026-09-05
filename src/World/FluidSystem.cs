using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class FluidSystem
    {
        private readonly WorldManager world;
        private readonly Queue<FluidUpdate> fluidUpdates;
        private readonly HashSet<Vector3i> updatedBlocks;

        // 流体参数
        public int WaterFlowSpeed { get; set; } = 5; // 每N tick更新一次
        public int LavaFlowSpeed { get; set; } = 30;
        public int WaterFlowDistance { get; set; } = 7;
        public int LavaFlowDistance { get; set; } = 3;
        public int WaterLevelDecrease { get; set; } = 1;
        public int LavaLevelDecrease { get; set; } = 2;

        // 统计
        public int PendingUpdates => fluidUpdates.Count;
        public int TotalFluidUpdates { get; private set; }
        public int FlowingWaterBlocks { get; private set; }
        public int FlowingLavaBlocks { get; private set; }

        public FluidSystem(WorldManager world)
        {
            this.world = world;
            fluidUpdates = new Queue<FluidUpdate>();
            updatedBlocks = new HashSet<Vector3i>();
        }

        public void Initialize()
        {
            Console.WriteLine("[FluidSystem] 流体系统初始化完成");
        }

        public void Update(int gameTick)
        {
            // 水更新较快
            if (gameTick % WaterFlowSpeed == 0)
            {
                ProcessFluidUpdates(FluidType.Water);
            }

            // 熔岩更新较慢
            if (gameTick % LavaFlowSpeed == 0)
            {
                ProcessFluidUpdates(FluidType.Lava);
            }

            // 统计流动的流体
            FlowingWaterBlocks = 0;
            FlowingLavaBlocks = 0;
        }

        private void ProcessFluidUpdates(FluidType type)
        {
            int maxUpdates = 50;
            int updatesProcessed = 0;

            while (fluidUpdates.Count > 0 && updatesProcessed < maxUpdates)
            {
                FluidUpdate update = fluidUpdates.Dequeue();
                if (update.Type != type)
                {
                    fluidUpdates.Enqueue(update);
                    continue;
                }

                ProcessFluidUpdate(update);
                updatesProcessed++;
                TotalFluidUpdates++;
            }
        }

        private void ProcessFluidUpdate(FluidUpdate update)
        {
            int x = update.X;
            int y = update.Y;
            int z = update.Z;

            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            ushort block = world.GetBlock(x, y, z);

            // 检查是否是源方块
            if (IsFluidSource(block))
            {
                // 源方块向周围流动
                FlowFromSource(x, y, z, GetFluidType(block));
            }
            else if (IsFlowing(block))
            {
                // 流动方块继续流动
                FlowFromFlowing(x, y, z, block);
            }
            else if (block == GameConstants.BLOCK_AIR)
            {
                // 空气方块，检查是否有流体流入
                CheckFluidFlowInto(x, y, z);
            }
        }

        public void OnBlockChanged(int x, int y, int z)
        {
            // 方块变化时，检查周围的流体
            ScheduleFluidUpdate(x + 1, y, z);
            ScheduleFluidUpdate(x - 1, y, z);
            ScheduleFluidUpdate(x, y + 1, z);
            ScheduleFluidUpdate(x, y - 1, z);
            ScheduleFluidUpdate(x, y, z + 1);
            ScheduleFluidUpdate(x, y, z - 1);
        }

        public void ScheduleFluidUpdate(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            Vector3i pos = new Vector3i(x, y, z);
            if (updatedBlocks.Contains(pos)) return;

            ushort block = world.GetBlock(x, y, z);
            FluidType type = GetFluidType(block);

            if (type != FluidType.None || block == GameConstants.BLOCK_AIR)
            {
                fluidUpdates.Enqueue(new FluidUpdate
                {
                    X = x,
                    Y = y,
                    Z = z,
                    Type = type
                });
                updatedBlocks.Add(pos);
            }
        }

        private void FlowFromSource(int x, int y, int z, FluidType type)
        {
            int flowDistance = type == FluidType.Water ? WaterFlowDistance : LavaFlowDistance;
            ushort flowingBlock = type == FluidType.Water ? GameConstants.BLOCK_WATER_FLOWING : GameConstants.BLOCK_LAVA_FLOWING;

            // 向下流动
            if (CanFlowInto(x, y - 1, z, type))
            {
                world.SetBlock(x, y - 1, z, flowingBlock);
                SetFluidLevel(x, y - 1, z, flowDistance);
                ScheduleFluidUpdate(x, y - 1, z);
            }

            // 水平流动
            FlowHorizontal(x, y, z, type, flowDistance);
        }

        private void FlowFromFlowing(int x, int y, int z, ushort block)
        {
            FluidType type = GetFluidType(block);
            int currentLevel = GetFluidLevel(x, y, z);
            int flowDistance = type == FluidType.Water ? WaterFlowDistance : LavaFlowDistance;
            int levelDecrease = type == FluidType.Water ? WaterLevelDecrease : LavaLevelDecrease;

            if (currentLevel <= 0)
            {
                // 流体消失
                world.SetBlock(x, y, z, GameConstants.BLOCK_AIR);
                return;
            }

            ushort flowingBlock = type == FluidType.Water ? GameConstants.BLOCK_WATER_FLOWING : GameConstants.BLOCK_LAVA_FLOWING;

            // 向下流动
            if (CanFlowInto(x, y - 1, z, type))
            {
                world.SetBlock(x, y - 1, z, flowingBlock);
                SetFluidLevel(x, y - 1, z, flowDistance);
                ScheduleFluidUpdate(x, y - 1, z);
            }

            // 水平流动（降低等级）
            int newLevel = currentLevel - levelDecrease;
            if (newLevel > 0)
            {
                FlowHorizontalWithLevel(x, y, z, type, newLevel);
            }
        }

        private void FlowHorizontal(int x, int y, int z, FluidType type, int level)
        {
            FlowHorizontalWithLevel(x, y, z, type, level);
        }

        private void FlowHorizontalWithLevel(int x, int y, int z, FluidType type, int level)
        {
            ushort flowingBlock = type == FluidType.Water ? GameConstants.BLOCK_WATER_FLOWING : GameConstants.BLOCK_LAVA_FLOWING;

            // 四个方向
            TryFlowTo(x + 1, y, z, type, level, flowingBlock);
            TryFlowTo(x - 1, y, z, type, level, flowingBlock);
            TryFlowTo(x, y, z + 1, type, level, flowingBlock);
            TryFlowTo(x, y, z - 1, type, level, flowingBlock);
        }

        private void TryFlowTo(int x, int y, int z, FluidType type, int level, ushort flowingBlock)
        {
            if (CanFlowInto(x, y, z, type))
            {
                // 检查是否是更低等级的同类流体
                ushort existingBlock = world.GetBlock(x, y, z);
                if (IsFlowing(existingBlock) && GetFluidType(existingBlock) == type)
                {
                    int existingLevel = GetFluidLevel(x, y, z);
                    if (existingLevel < level)
                    {
                        SetFluidLevel(x, y, z, level);
                        ScheduleFluidUpdate(x, y, z);
                    }
                }
                else
                {
                    world.SetBlock(x, y, z, flowingBlock);
                    SetFluidLevel(x, y, z, level);
                    ScheduleFluidUpdate(x, y, z);
                }

                // 检查与其他流体的交互
                CheckFluidInteraction(x, y, z, type);
            }
        }

        private bool CanFlowInto(int x, int y, int z, FluidType type)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return false;

            ushort block = world.GetBlock(x, y, z);

            // 空气可以流入
            if (block == GameConstants.BLOCK_AIR) return true;

            // 同类流体可以流入（更新等级）
            if (IsFlowing(block) && GetFluidType(block) == type) return true;

            // 可被流体替换的方块
            if (IsReplaceable(block)) return true;

            return false;
        }

        private bool IsReplaceable(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_AIR ||
                   blockId == GameConstants.BLOCK_GRASS ||
                   blockId == GameConstants.BLOCK_TALL_GRASS ||
                   blockId == GameConstants.BLOCK_FERN ||
                   blockId == GameConstants.BLOCK_DEAD_BUSH ||
                   blockId == GameConstants.BLOCK_FIRE ||
                   blockId == GameConstants.BLOCK_SNOW ||
                   blockId == GameConstants.BLOCK_VINE;
        }

        private void CheckFluidFlowInto(int x, int y, int z)
        {
            // 检查上方是否有流体
            ushort aboveBlock = world.GetBlock(x, y + 1, z);
            if (IsFluid(aboveBlock))
            {
                ScheduleFluidUpdate(x, y + 1, z);
                return;
            }

            // 检查四周是否有更高等级的流体
            CheckNeighborFluid(x + 1, y, z, x, y, z);
            CheckNeighborFluid(x - 1, y, z, x, y, z);
            CheckNeighborFluid(x, y, z + 1, x, y, z);
            CheckNeighborFluid(x, y, z - 1, x, y, z);
        }

        private void CheckNeighborFluid(int fromX, int fromY, int fromZ, int toX, int toY, int toZ)
        {
            ushort block = world.GetBlock(fromX, fromY, fromZ);
            if (IsFlowing(block))
            {
                int level = GetFluidLevel(fromX, fromY, fromZ);
                if (level > 1)
                {
                    ScheduleFluidUpdate(fromX, fromY, fromZ);
                }
            }
            else if (IsFluidSource(block))
            {
                ScheduleFluidUpdate(fromX, fromY, fromZ);
            }
        }

        private void CheckFluidInteraction(int x, int y, int z, FluidType type)
        {
            // 水和熔岩接触产生石头/圆石/黑曜石
            if (type == FluidType.Water)
            {
                // 检查周围是否有熔岩
                CheckLavaInteraction(x + 1, y, z);
                CheckLavaInteraction(x - 1, y, z);
                CheckLavaInteraction(x, y + 1, z);
                CheckLavaInteraction(x, y - 1, z);
                CheckLavaInteraction(x, y, z + 1);
                CheckLavaInteraction(x, y, z - 1);
            }
            else if (type == FluidType.Lava)
            {
                // 检查周围是否有水
                CheckWaterInteraction(x + 1, y, z);
                CheckWaterInteraction(x - 1, y, z);
                CheckWaterInteraction(x, y + 1, z);
                CheckWaterInteraction(x, y - 1, z);
                CheckWaterInteraction(x, y, z + 1);
                CheckWaterInteraction(x, y, z - 1);
            }
        }

        private void CheckLavaInteraction(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);
            if (IsLava(block))
            {
                // 水接触熔岩
                if (IsLavaSource(block))
                {
                    // 熔岩源 + 水 = 黑曜石
                    world.SetBlock(x, y, z, GameConstants.BLOCK_OBSIDIAN);
                }
                else
                {
                    // 流动熔岩 + 水 = 圆石
                    world.SetBlock(x, y, z, GameConstants.BLOCK_COBBLESTONE);
                }
                OnBlockChanged(x, y, z);
            }
        }

        private void CheckWaterInteraction(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);
            if (IsWater(block))
            {
                // 熔岩接触水
                if (IsLavaSource(world.GetBlock(x, y, z)))
                {
                    world.SetBlock(x, y, z, GameConstants.BLOCK_OBSIDIAN);
                }
                else
                {
                    world.SetBlock(x, y, z, GameConstants.BLOCK_STONE);
                }
                OnBlockChanged(x, y, z);
            }
        }

        public bool IsFluid(ushort blockId)
        {
            return IsWater(blockId) || IsLava(blockId);
        }

        public bool IsWater(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WATER_STILL ||
                   blockId == GameConstants.BLOCK_WATER_FLOWING;
        }

        public bool IsLava(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_LAVA_STILL ||
                   blockId == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public bool IsFluidSource(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WATER_STILL ||
                   blockId == GameConstants.BLOCK_LAVA_STILL;
        }

        public bool IsFlowing(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WATER_FLOWING ||
                   blockId == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public bool IsWaterSource(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WATER_STILL;
        }

        public bool IsLavaSource(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_LAVA_STILL;
        }

        public FluidType GetFluidType(ushort blockId)
        {
            if (IsWater(blockId)) return FluidType.Water;
            if (IsLava(blockId)) return FluidType.Lava;
            return FluidType.None;
        }

        private int GetFluidLevel(int x, int y, int z)
        {
            // 简化实现：使用方块数据存储等级
            // 实际实现中需要额外的数据存储
            return WaterFlowDistance; // 默认最大等级
        }

        private void SetFluidLevel(int x, int y, int z, int level)
        {
            // 简化实现：实际需要存储到方块数据中
        }

        public void PlaceFluid(int x, int y, int z, FluidType type)
        {
            ushort block = type == FluidType.Water ? GameConstants.BLOCK_WATER_STILL : GameConstants.BLOCK_LAVA_STILL;
            world.SetBlock(x, y, z, block);
            ScheduleFluidUpdate(x, y, z);
            OnBlockChanged(x, y, z);
        }

        public void RemoveFluid(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);
            if (IsFluid(block))
            {
                world.SetBlock(x, y, z, GameConstants.BLOCK_AIR);
                OnBlockChanged(x, y, z);
            }
        }

        public bool IsFluidSolidifying(ushort blockId)
        {
            // 检查流体是否正在凝固（与其他流体接触）
            return false;
        }

        public float GetFluidHeight(ushort blockId)
        {
            if (IsWater(blockId)) return 0.875f; // 7/8
            if (IsLava(blockId)) return 0.875f;
            return 0f;
        }

        public float GetFluidViscosity(FluidType type)
        {
            return type == FluidType.Water ? 0.8f : 0.5f;
        }

        public float GetFluidDrag(FluidType type)
        {
            return type == FluidType.Water ? 0.8f : 0.6f;
        }

        public float GetFluidBuoyancy(FluidType type)
        {
            return type == FluidType.Water ? 0.5f : 0.3f;
        }

        public void ClearPendingUpdates()
        {
            fluidUpdates.Clear();
            updatedBlocks.Clear();
        }

        public void ResetStats()
        {
            TotalFluidUpdates = 0;
            FlowingWaterBlocks = 0;
            FlowingLavaBlocks = 0;
        }
    }

    public struct FluidUpdate
    {
        public int X;
        public int Y;
        public int Z;
        public FluidType Type;
    }

    public enum FluidType
    {
        None,
        Water,
        Lava
    }
}
