using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class RedstoneSystem
    {
        private readonly WorldManager world;
        private readonly Queue<RedstoneUpdate> updates;
        private readonly HashSet<Vector3i> updatedBlocks;

        // 红石参数
        public int MaxSignalStrength { get; set; } = 15;
        public int SignalDecay { get; set; } = 1;
        public int TickRate { get; set; } = 2; // 每N tick更新一次

        // 统计
        public int PendingUpdates => updates.Count;
        public int TotalRedstoneUpdates { get; private set; }
        public int ActiveRedstoneComponents { get; private set; }

        public RedstoneSystem(WorldManager world)
        {
            this.world = world;
            updates = new Queue<RedstoneUpdate>();
            updatedBlocks = new HashSet<Vector3i>();
        }

        public void Initialize()
        {
            Console.WriteLine("[RedstoneSystem] 红石系统初始化完成");
        }

        public void Update(int gameTick)
        {
            if (gameTick % TickRate != 0) return;

            int maxUpdates = 100;
            int updatesProcessed = 0;

            while (updates.Count > 0 && updatesProcessed < maxUpdates)
            {
                RedstoneUpdate update = updates.Dequeue();
                ProcessRedstoneUpdate(update);
                updatesProcessed++;
                TotalRedstoneUpdates++;
            }

            // 统计活跃的红石组件
            ActiveRedstoneComponents = 0;
        }

        private void ProcessRedstoneUpdate(RedstoneUpdate update)
        {
            int x = update.X;
            int y = update.Y;
            int z = update.Z;

            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            ushort block = world.GetBlock(x, y, z);

            switch (block)
            {
                case GameConstants.BLOCK_REDSTONE_WIRE:
                    UpdateRedstoneWire(x, y, z);
                    break;

                case GameConstants.BLOCK_REDSTONE_TORCH:
                case GameConstants.BLOCK_REDSTONE_TORCH_WALL:
                    UpdateRedstoneTorch(x, y, z);
                    break;

                case GameConstants.BLOCK_REDSTONE_REPEATER:
                    UpdateRepeater(x, y, z);
                    break;

                case GameConstants.BLOCK_REDSTONE_COMPARATOR:
                    UpdateComparator(x, y, z);
                    break;

                case GameConstants.BLOCK_LEVER:
                    UpdateLever(x, y, z);
                    break;

                case GameConstants.BLOCK_STONE_BUTTON:
                case GameConstants.BLOCK_WOODEN_BUTTON:
                    UpdateButton(x, y, z);
                    break;

                case GameConstants.BLOCK_STONE_PRESSURE_PLATE:
                case GameConstants.BLOCK_WOODEN_PRESSURE_PLATE:
                case GameConstants.BLOCK_HEAVY_WEIGHTED_PRESSURE_PLATE:
                case GameConstants.BLOCK_LIGHT_WEIGHTED_PRESSURE_PLATE:
                    UpdatePressurePlate(x, y, z);
                    break;

                case GameConstants.BLOCK_PISTON:
                case GameConstants.BLOCK_STICKY_PISTON:
                    UpdatePiston(x, y, z);
                    break;

                case GameConstants.BLOCK_DISPENSER:
                case GameConstants.BLOCK_DROPPER:
                    UpdateDispenser(x, y, z);
                    break;

                case GameConstants.BLOCK_HOPPER:
                    UpdateHopper(x, y, z);
                    break;

                case GameConstants.BLOCK_NOTE_BLOCK:
                    UpdateNoteBlock(x, y, z);
                    break;

                case GameConstants.BLOCK_TRIPWIRE_HOOK:
                case GameConstants.BLOCK_TRIPWIRE:
                    UpdateTripwire(x, y, z);
                    break;

                case GameConstants.BLOCK_DAYLIGHT_DETECTOR:
                    UpdateDaylightDetector(x, y, z);
                    break;

                case GameConstants.BLOCK_TARGET:
                    UpdateTarget(x, y, z);
                    break;
            }
        }

        public void OnBlockChanged(int x, int y, int z)
        {
            // 方块变化时，更新周围的红石
            ScheduleRedstoneUpdate(x, y, z);
            ScheduleNeighborUpdates(x, y, z);
        }

        public void ScheduleRedstoneUpdate(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            Vector3i pos = new Vector3i(x, y, z);
            if (updatedBlocks.Contains(pos)) return;

            ushort block = world.GetBlock(x, y, z);
            if (IsRedstoneComponent(block))
            {
                updates.Enqueue(new RedstoneUpdate { X = x, Y = y, Z = z });
                updatedBlocks.Add(pos);
            }
        }

        private void ScheduleNeighborUpdates(int x, int y, int z)
        {
            ScheduleRedstoneUpdate(x + 1, y, z);
            ScheduleRedstoneUpdate(x - 1, y, z);
            ScheduleRedstoneUpdate(x, y + 1, z);
            ScheduleRedstoneUpdate(x, y - 1, z);
            ScheduleRedstoneUpdate(x, y, z + 1);
            ScheduleRedstoneUpdate(x, y, z - 1);
        }

        private void UpdateRedstoneWire(int x, int y, int z)
        {
            // 获取周围最强信号
            int maxSignal = 0;

            maxSignal = Math.Max(maxSignal, GetInputSignal(x + 1, y, z));
            maxSignal = Math.Max(maxSignal, GetInputSignal(x - 1, y, z));
            maxSignal = Math.Max(maxSignal, GetInputSignal(x, y + 1, z));
            maxSignal = Math.Max(maxSignal, GetInputSignal(x, y - 1, z));
            maxSignal = Math.Max(maxSignal, GetInputSignal(x, y, z + 1));
            maxSignal = Math.Max(maxSignal, GetInputSignal(x, y, z - 1));

            // 衰减
            int newSignal = Math.Max(0, maxSignal - SignalDecay);

            // 设置信号强度（简化：使用方块数据存储）
            SetWireSignal(x, y, z, newSignal);

            // 传播到邻居
            if (newSignal > 0)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateRedstoneTorch(int x, int y, int z)
        {
            // 红石火把：下方方块被充能时熄灭
            bool isPowered = IsBlockPowered(x, y - 1, z);

            if (isPowered)
            {
                // 熄灭
                SetTorchPowered(x, y, z, false);
            }
            else
            {
                // 点亮，输出信号
                SetTorchPowered(x, y, z, true);
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateRepeater(int x, int y, int z)
        {
            // 中继器：延迟后输出输入信号
            int inputSignal = GetInputSignal(x, y, z);
            int delay = GetRepeaterDelay(x, y, z);

            // 简化：立即输出
            if (inputSignal > 0)
            {
                SetRepeaterPowered(x, y, z, true);
                ScheduleNeighborUpdates(x, y, z);
            }
            else
            {
                SetRepeaterPowered(x, y, z, false);
            }
        }

        private void UpdateComparator(int x, int y, int z)
        {
            // 比较器：比较或减法模式
            int inputSignal = GetInputSignal(x, y, z);
            int sideSignal = Math.Max(
                GetInputSignal(x + 1, y, z),
                Math.Max(GetInputSignal(x - 1, y, z),
                Math.Max(GetInputSignal(x, y, z + 1),
                GetInputSignal(x, y, z - 1)))
            );

            bool isSubtractionMode = IsComparatorSubtractionMode(x, y, z);

            int outputSignal;
            if (isSubtractionMode)
            {
                outputSignal = Math.Max(0, inputSignal - sideSignal);
            }
            else
            {
                outputSignal = sideSignal > inputSignal ? 0 : inputSignal;
            }

            SetComparatorOutput(x, y, z, outputSignal);

            if (outputSignal > 0)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateLever(int x, int y, int z)
        {
            // 拉杆：开关状态
            bool isOn = IsLeverOn(x, y, z);

            if (isOn)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateButton(int x, int y, int z)
        {
            // 按钮：临时信号
            if (IsButtonPressed(x, y, z))
            {
                ScheduleNeighborUpdates(x, y, z);

                // 自动弹起
                updates.Enqueue(new RedstoneUpdate { X = x, Y = y, Z = z });
            }
        }

        private void UpdatePressurePlate(int x, int y, int z)
        {
            // 压力板：检测实体
            int signal = CalculatePressurePlateSignal(x, y, z);
            SetPressurePlateSignal(x, y, z, signal);

            if (signal > 0)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdatePiston(int x, int y, int z)
        {
            // 活塞：被充能时推出
            bool isPowered = IsBlockPowered(x, y, z);
            bool isExtended = IsPistonExtended(x, y, z);

            if (isPowered && !isExtended)
            {
                // 推出
                ExtendPiston(x, y, z);
            }
            else if (!isPowered && isExtended)
            {
                // 收回
                RetractPiston(x, y, z);
            }
        }

        private void UpdateDispenser(int x, int y, int z)
        {
            // 发射器：被充能时发射物品
            if (IsBlockPowered(x, y, z))
            {
                DispenseItem(x, y, z);
            }
        }

        private void UpdateHopper(int x, int y, int z)
        {
            // 漏斗：转移物品
            TransferItems(x, y, z);
        }

        private void UpdateNoteBlock(int x, int y, int z)
        {
            // 音符盒：被充能时播放音符
            if (IsBlockPowered(x, y, z))
            {
                PlayNote(x, y, z);
            }
        }

        private void UpdateTripwire(int x, int y, int z)
        {
            // 绊线：检测实体
            bool isTriggered = IsTripwireTriggered(x, y, z);
            SetTripwirePowered(x, y, z, isTriggered);

            if (isTriggered)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateDaylightDetector(int x, int y, int z)
        {
            // 阳光探测器：根据光照输出信号
            int lightLevel = world.GetSkyLight(x, y, z);
            int signal = (int)((lightLevel / 15.0f) * 15);
            SetDaylightDetectorSignal(x, y, z, signal);

            if (signal > 0)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private void UpdateTarget(int x, int y, int z)
        {
            // 标靶：被弹射物击中时输出信号
            int signal = GetTargetSignal(x, y, z);
            if (signal > 0)
            {
                ScheduleNeighborUpdates(x, y, z);
            }
        }

        private int GetInputSignal(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return 0;

            ushort block = world.GetBlock(x, y, z);

            switch (block)
            {
                case GameConstants.BLOCK_REDSTONE_WIRE:
                    return GetWireSignal(x, y, z);

                case GameConstants.BLOCK_REDSTONE_TORCH:
                case GameConstants.BLOCK_REDSTONE_TORCH_WALL:
                    return IsTorchPowered(x, y, z) ? MaxSignalStrength : 0;

                case GameConstants.BLOCK_REDSTONE_REPEATER:
                    return IsRepeaterPowered(x, y, z) ? MaxSignalStrength : 0;

                case GameConstants.BLOCK_REDSTONE_COMPARATOR:
                    return GetComparatorOutput(x, y, z);

                case GameConstants.BLOCK_LEVER:
                    return IsLeverOn(x, y, z) ? MaxSignalStrength : 0;

                case GameConstants.BLOCK_STONE_BUTTON:
                case GameConstants.BLOCK_WOODEN_BUTTON:
                    return IsButtonPressed(x, y, z) ? MaxSignalStrength : 0;

                case GameConstants.BLOCK_STONE_PRESSURE_PLATE:
                case GameConstants.BLOCK_WOODEN_PRESSURE_PLATE:
                case GameConstants.BLOCK_HEAVY_WEIGHTED_PRESSURE_PLATE:
                case GameConstants.BLOCK_LIGHT_WEIGHTED_PRESSURE_PLATE:
                    return GetPressurePlateSignal(x, y, z);

                case GameConstants.BLOCK_DAYLIGHT_DETECTOR:
                    return GetDaylightDetectorSignal(x, y, z);

                case GameConstants.BLOCK_TARGET:
                    return GetTargetSignal(x, y, z);

                case GameConstants.BLOCK_TRIPWIRE_HOOK:
                    return IsTripwireHookPowered(x, y, z) ? MaxSignalStrength : 0;

                default:
                    // 检查方块是否被强充能
                    if (IsBlockStronglyPowered(x, y, z))
                    {
                        return MaxSignalStrength;
                    }
                    return 0;
            }
        }

        private bool IsBlockPowered(int x, int y, int z)
        {
            return GetInputSignal(x, y, z) > 0;
        }

        private bool IsBlockStronglyPowered(int x, int y, int z)
        {
            // 检查方块是否被强充能（红石火把、中继器等直接指向）
            return GetInputSignal(x + 1, y, z) > 0 ||
                   GetInputSignal(x - 1, y, z) > 0 ||
                   GetInputSignal(x, y + 1, z) > 0 ||
                   GetInputSignal(x, y - 1, z) > 0 ||
                   GetInputSignal(x, y, z + 1) > 0 ||
                   GetInputSignal(x, y, z - 1) > 0;
        }

        public bool IsRedstoneComponent(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_REDSTONE_WIRE ||
                   blockId == GameConstants.BLOCK_REDSTONE_TORCH ||
                   blockId == GameConstants.BLOCK_REDSTONE_TORCH_WALL ||
                   blockId == GameConstants.BLOCK_REDSTONE_REPEATER ||
                   blockId == GameConstants.BLOCK_REDSTONE_COMPARATOR ||
                   blockId == GameConstants.BLOCK_LEVER ||
                   blockId == GameConstants.BLOCK_STONE_BUTTON ||
                   blockId == GameConstants.BLOCK_WOODEN_BUTTON ||
                   blockId == GameConstants.BLOCK_STONE_PRESSURE_PLATE ||
                   blockId == GameConstants.BLOCK_WOODEN_PRESSURE_PLATE ||
                   blockId == GameConstants.BLOCK_HEAVY_WEIGHTED_PRESSURE_PLATE ||
                   blockId == GameConstants.BLOCK_LIGHT_WEIGHTED_PRESSURE_PLATE ||
                   blockId == GameConstants.BLOCK_PISTON ||
                   blockId == GameConstants.BLOCK_STICKY_PISTON ||
                   blockId == GameConstants.BLOCK_DISPENSER ||
                   blockId == GameConstants.BLOCK_DROPPER ||
                   blockId == GameConstants.BLOCK_HOPPER ||
                   blockId == GameConstants.BLOCK_NOTE_BLOCK ||
                   blockId == GameConstants.BLOCK_TRIPWIRE_HOOK ||
                   blockId == GameConstants.BLOCK_TRIPWIRE ||
                   blockId == GameConstants.BLOCK_DAYLIGHT_DETECTOR ||
                   blockId == GameConstants.BLOCK_TARGET ||
                   blockId == GameConstants.BLOCK_REDSTONE_BLOCK ||
                   blockId == GameConstants.BLOCK_OBSERVER;
        }

        // 以下是简化的状态存储方法（实际实现需要额外的数据存储）

        private int GetWireSignal(int x, int y, int z) { return 0; }
        private void SetWireSignal(int x, int y, int z, int signal) { }
        private bool IsTorchPowered(int x, int y, int z) { return false; }
        private void SetTorchPowered(int x, int y, int z, bool powered) { }
        private bool IsRepeaterPowered(int x, int y, int z) { return false; }
        private void SetRepeaterPowered(int x, int y, int z, bool powered) { }
        private int GetRepeaterDelay(int x, int y, int z) { return 1; }
        private bool IsComparatorSubtractionMode(int x, int y, int z) { return false; }
        private int GetComparatorOutput(int x, int y, int z) { return 0; }
        private void SetComparatorOutput(int x, int y, int z, int signal) { }
        private bool IsLeverOn(int x, int y, int z) { return false; }
        private bool IsButtonPressed(int x, int y, int z) { return false; }
        private int GetPressurePlateSignal(int x, int y, int z) { return 0; }
        private void SetPressurePlateSignal(int x, int y, int z, int signal) { }
        private int CalculatePressurePlateSignal(int x, int y, int z) { return 0; }
        private bool IsPistonExtended(int x, int y, int z) { return false; }
        private void ExtendPiston(int x, int y, int z) { }
        private void RetractPiston(int x, int y, int z) { }
        private void DispenseItem(int x, int y, int z) { }
        private void TransferItems(int x, int y, int z) { }
        private void PlayNote(int x, int y, int z) { }
        private bool IsTripwireTriggered(int x, int y, int z) { return false; }
        private void SetTripwirePowered(int x, int y, int z, bool powered) { }
        private bool IsTripwireHookPowered(int x, int y, int z) { return false; }
        private int GetDaylightDetectorSignal(int x, int y, int z) { return 0; }
        private void SetDaylightDetectorSignal(int x, int y, int z, int signal) { }
        private int GetTargetSignal(int x, int y, int z) { return 0; }

        public void ToggleLever(int x, int y, int z)
        {
            // 切换拉杆状态
            OnBlockChanged(x, y, z);
        }

        public void PressButton(int x, int y, int z)
        {
            // 按下按钮
            OnBlockChanged(x, y, z);
        }

        public void TriggerTarget(int x, int y, int z, int signalStrength)
        {
            // 触发标靶
            OnBlockChanged(x, y, z);
        }

        public void ClearPendingUpdates()
        {
            updates.Clear();
            updatedBlocks.Clear();
        }

        public void ResetStats()
        {
            TotalRedstoneUpdates = 0;
            ActiveRedstoneComponents = 0;
        }
    }

    public struct RedstoneUpdate
    {
        public int X;
        public int Y;
        public int Z;
    }
}
