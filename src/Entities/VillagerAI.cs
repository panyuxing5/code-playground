using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Items;

namespace VoxelCraft.Entities
{
    public class VillagerAI
    {
        private readonly WorldManager world;
        private readonly EntityManager entityManager;

        // 村民行为参数
        public float WalkSpeed { get; set; } = 0.15f;
        public float RunSpeed { get; set; } = 0.3f;
        public float DetectionRange { get; set; } = 16.0f;
        public float InteractionRange { get; set; } = 3.0f;
        public int WorkStartTime { get; set; } = 2000; // 游戏刻
        public int WorkEndTime { get; set; } = 9000;
        public int SocialStartTime { get; set; } = 9000;
        public int SocialEndTime { get; set; } = 11000;
        public int SleepStartTime { get; set; } = 12000;
        public int SleepEndTime { get; set; } = 23000;

        // 统计
        public int ActiveVillagers { get; private set; }
        public int TradesCompleted { get; private set; }
        public int ItemsGathered { get; private set; }

        public VillagerAI(WorldManager world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
        }

        public void Initialize()
        {
            Console.WriteLine("[VillagerAI] 村民AI系统初始化完成");
        }

        public void Update(Villager villager, int gameTime)
        {
            if (villager.IsDead) return;

            // 更新日程
            UpdateSchedule(villager, gameTime);

            // 根据当前状态执行行为
            switch (villager.CurrentActivity)
            {
                case VillagerActivity.Idle:
                    UpdateIdle(villager);
                    break;

                case VillagerActivity.Working:
                    UpdateWorking(villager);
                    break;

                case VillagerActivity.Socializing:
                    UpdateSocializing(villager);
                    break;

                case VillagerActivity.Sleeping:
                    UpdateSleeping(villager);
                    break;

                case VillagerActivity.Wandering:
                    UpdateWandering(villager);
                    break;

                case VillagerActivity.Fleeing:
                    UpdateFleeing(villager);
                    break;

                case VillagerActivity.Trading:
                    UpdateTrading(villager);
                    break;

                case VillagerActivity.Harvesting:
                    UpdateHarvesting(villager);
                    break;
            }

            // 检查危险
            CheckForDanger(villager);

            ActiveVillagers = entityManager.GetEntities().FindAll(e => e is Villager && !e.IsDead).Count;
        }

        private void UpdateSchedule(Villager villager, int gameTime)
        {
            // 根据时间切换活动
            if (gameTime >= WorkStartTime && gameTime < WorkEndTime)
            {
                if (villager.CurrentActivity != VillagerActivity.Working &&
                    villager.CurrentActivity != VillagerActivity.Fleeing)
                {
                    villager.CurrentActivity = VillagerActivity.Working;
                }
            }
            else if (gameTime >= SocialStartTime && gameTime < SocialEndTime)
            {
                if (villager.CurrentActivity != VillagerActivity.Socializing &&
                    villager.CurrentActivity != VillagerActivity.Fleeing)
                {
                    villager.CurrentActivity = VillagerActivity.Socializing;
                }
            }
            else if (gameTime >= SleepStartTime || gameTime < SleepEndTime)
            {
                if (villager.CurrentActivity != VillagerActivity.Sleeping &&
                    villager.CurrentActivity != VillagerActivity.Fleeing)
                {
                    villager.CurrentActivity = VillagerActivity.Sleeping;
                }
            }
            else
            {
                if (villager.CurrentActivity != VillagerActivity.Wandering &&
                    villager.CurrentActivity != VillagerActivity.Fleeing)
                {
                    villager.CurrentActivity = VillagerActivity.Wandering;
                }
            }
        }

        private void UpdateIdle(Villager villager)
        {
            // 随机环顾四周
            if (Random.Shared.NextDouble() < 0.02)
            {
                villager.Yaw = Random.Shared.Next(360);
            }

            // 偶尔开始游荡
            if (Random.Shared.NextDouble() < 0.01)
            {
                villager.CurrentActivity = VillagerActivity.Wandering;
            }
        }

        private void UpdateWorking(Villager villager)
        {
            // 前往工作站点
            if (villager.WorkstationPosition.HasValue)
            {
                float distance = Vector3.Distance(villager.Position, villager.WorkstationPosition.Value);

                if (distance > 2.0f)
                {
                    // 走向工作站
                    MoveTowards(villager, villager.WorkstationPosition.Value);
                }
                else
                {
                    // 在工作站工作
                    villager.WorkTicks++;

                    // 工作时偶尔环顾
                    if (Random.Shared.NextDouble() < 0.05)
                    {
                        villager.Yaw = Random.Shared.Next(360);
                    }

                    // 农民收获作物
                    if (villager.Profession == VillagerProfession.Farmer)
                    {
                        TryHarvestCrops(villager);
                    }
                }
            }
            else
            {
                // 寻找工作站
                FindWorkstation(villager);
            }
        }

        private void UpdateSocializing(Villager villager)
        {
            // 寻找其他村民社交
            Villager otherVillager = FindNearbyVillager(villager, 8.0f);

            if (otherVillager != null)
            {
                float distance = Vector3.Distance(villager.Position, otherVillager.Position);

                if (distance > 2.0f)
                {
                    MoveTowards(villager, otherVillager.Position);
                }
                else
                {
                    // 面对面社交
                    villager.Yaw = CalculateYaw(villager.Position, otherVillager.Position);
                    villager.SocialTicks++;

                    // 有几率繁殖意愿
                    if (villager.CanBreed && otherVillager.CanBreed &&
                        villager.SocialTicks > 100 && Random.Shared.NextDouble() < 0.001)
                    {
                        TryBreed(villager, otherVillager);
                    }
                }
            }
            else
            {
                // 没有其他村民，游荡
                villager.CurrentActivity = VillagerActivity.Wandering;
            }
        }

        private void UpdateSleeping(Villager villager)
        {
            // 寻找床铺
            if (villager.BedPosition.HasValue)
            {
                float distance = Vector3.Distance(villager.Position, villager.BedPosition.Value);

                if (distance > 1.5f)
                {
                    MoveTowards(villager, villager.BedPosition.Value);
                }
                else
                {
                    // 睡觉
                    villager.IsSleeping = true;
                    villager.Velocity = Vector3.Zero;
                }
            }
            else
            {
                // 寻找床铺
                FindBed(villager);
            }
        }

        private void UpdateWandering(Villager villager)
        {
            // 随机游荡
            if (!villager.HasTarget || Random.Shared.NextDouble() < 0.02)
            {
                villager.TargetPosition = villager.Position + new Vector3(
                    (float)(Random.Shared.NextDouble() - 0.5) * 20,
                    0,
                    (float)(Random.Shared.NextDouble() - 0.5) * 20
                );
                villager.HasTarget = true;
            }

            if (villager.HasTarget)
            {
                float distance = Vector3.Distance(villager.Position, villager.TargetPosition);

                if (distance > 1.0f)
                {
                    MoveTowards(villager, villager.TargetPosition);
                }
                else
                {
                    villager.HasTarget = false;
                }
            }
        }

        private void UpdateFleeing(Villager villager)
        {
            // 逃离威胁
            if (villager.ThreatPosition.HasValue)
            {
                Vector3 fleeDirection = (villager.Position - villager.ThreatPosition.Value).Normalized();
                Vector3 fleeTarget = villager.Position + fleeDirection * 10.0f;

                MoveTowards(villager, fleeTarget, RunSpeed);

                // 检查是否已经逃离
                float distance = Vector3.Distance(villager.Position, villager.ThreatPosition.Value);
                if (distance > 20.0f)
                {
                    villager.ThreatPosition = null;
                    villager.CurrentActivity = VillagerActivity.Wandering;
                }
            }
            else
            {
                villager.CurrentActivity = VillagerActivity.Wandering;
            }
        }

        private void UpdateTrading(Villager villager)
        {
            // 交易状态（玩家打开交易界面时）
            villager.Yaw = CalculateYaw(villager.Position, villager.Position + villager.GetLookVector());
        }

        private void UpdateHarvesting(Villager villager)
        {
            // 农民收获作物
            TryHarvestCrops(villager);
        }

        private void CheckForDanger(Villager villager)
        {
            // 检查附近的敌对生物
            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity.IsDead) continue;
                if (entity is not Zombie && entity is not Skeleton &&
                    entity is not Creeper && entity is not Spider) continue;

                float distance = Vector3.Distance(villager.Position, entity.Position);
                if (distance < 8.0f)
                {
                    villager.ThreatPosition = entity.Position;
                    villager.CurrentActivity = VillagerActivity.Fleeing;
                    villager.IsSleeping = false;
                    return;
                }
            }

            // 检查僵尸围城
            int zombieCount = 0;
            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity is Zombie && !entity.IsDead)
                {
                    float distance = Vector3.Distance(villager.Position, entity.Position);
                    if (distance < 16.0f)
                    {
                        zombieCount++;
                    }
                }
            }

            if (zombieCount >= 5)
            {
                villager.IsPanicked = true;
            }
        }

        private void MoveTowards(Villager villager, Vector3 target, float speed = -1)
        {
            if (speed < 0) speed = WalkSpeed;

            Vector3 direction = (target - villager.Position).Normalized();
            direction.Y = 0;

            villager.Velocity.X = direction.X * speed;
            villager.Velocity.Z = direction.Z * speed;

            villager.Yaw = CalculateYaw(villager.Position, target);

            // 检查是否需要跳
            if (IsBlockAhead(villager))
            {
                if (villager.OnGround)
                {
                    villager.Velocity.Y = 0.4f;
                }
            }
        }

        private bool IsBlockAhead(Villager villager)
        {
            Vector3 ahead = villager.Position + villager.GetLookVector() * 0.5f;
            ushort block = world.GetBlock((int)ahead.X, (int)ahead.Y, (int)ahead.Z);
            return block != GameConstants.BLOCK_AIR &&
                   block != GameConstants.BLOCK_WATER_STILL &&
                   block != GameConstants.BLOCK_WATER_FLOWING;
        }

        private float CalculateYaw(Vector3 from, Vector3 to)
        {
            float dx = to.X - from.X;
            float dz = to.Z - from.Z;
            return (float)(Math.Atan2(dx, dz) * 180 / Math.PI);
        }

        private Villager FindNearbyVillager(Villager current, float range)
        {
            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity is Villager other && !other.IsDead && other.Id != current.Id)
                {
                    float distance = Vector3.Distance(current.Position, other.Position);
                    if (distance < range)
                    {
                        return other;
                    }
                }
            }
            return null;
        }

        private void FindWorkstation(Villager villager)
        {
            // 搜索附近的工作站方块
            int searchRadius = 16;
            Vector3i pos = new Vector3i((int)villager.Position.X, (int)villager.Position.Y, (int)villager.Position.Z);

            for (int dx = -searchRadius; dx <= searchRadius; dx++)
            {
                for (int dy = -searchRadius; dy <= searchRadius; dy++)
                {
                    for (int dz = -searchRadius; dz <= searchRadius; dz++)
                    {
                        ushort block = world.GetBlock(pos.X + dx, pos.Y + dy, pos.Z + dz);
                        if (IsWorkstation(block, villager.Profession))
                        {
                            villager.WorkstationPosition = new Vector3(pos.X + dx + 0.5f, pos.Y + dy, pos.Z + dz + 0.5f);
                            return;
                        }
                    }
                }
            }
        }

        private bool IsWorkstation(ushort blockId, VillagerProfession profession)
        {
            switch (profession)
            {
                case VillagerProfession.Farmer:
                    return blockId == GameConstants.BLOCK_COMPOSTER;

                case VillagerProfession.Fisherman:
                    return blockId == GameConstants.BLOCK_BARREL;

                case VillagerProfession.Fletcher:
                    return blockId == GameConstants.BLOCK_FLETCHING_TABLE;

                case VillagerProfession.Shepherd:
                    return blockId == GameConstants.BLOCK_LOOM;

                case VillagerProfession.Librarian:
                    return blockId == GameConstants.BLOCK_LECTERN;

                case VillagerProfession.Cartographer:
                    return blockId == GameConstants.BLOCK_CARTOGRAPHY_TABLE;

                case VillagerProfession.Cleric:
                    return blockId == GameConstants.BLOCK_BREWING_STAND;

                case VillagerProfession.Armorer:
                    return blockId == GameConstants.BLOCK_BLAST_FURNACE;

                case VillagerProfession.WeaponSmith:
                    return blockId == GameConstants.BLOCK_GRINDSTONE;

                case VillagerProfession.ToolSmith:
                    return blockId == GameConstants.BLOCK_SMITHING_TABLE;

                case VillagerProfession.Butcher:
                    return blockId == GameConstants.BLOCK_SMOKER;

                case VillagerProfession.Leatherworker:
                    return blockId == GameConstants.BLOCK_CAULDRON;

                case VillagerProfession.Mason:
                    return blockId == GameConstants.BLOCK_STONECUTTER;

                default:
                    return false;
            }
        }

        private void FindBed(Villager villager)
        {
            int searchRadius = 16;
            Vector3i pos = new Vector3i((int)villager.Position.X, (int)villager.Position.Y, (int)villager.Position.Z);

            for (int dx = -searchRadius; dx <= searchRadius; dx++)
            {
                for (int dy = -searchRadius; dy <= searchRadius; dy++)
                {
                    for (int dz = -searchRadius; dz <= searchRadius; dz++)
                    {
                        ushort block = world.GetBlock(pos.X + dx, pos.Y + dy, pos.Z + dz);
                        if (block == GameConstants.BLOCK_RED_BED ||
                            block == GameConstants.BLOCK_BLUE_BED ||
                            block == GameConstants.BLOCK_GREEN_BED ||
                            block == GameConstants.BLOCK_YELLOW_BED ||
                            block == GameConstants.BLOCK_WHITE_BED)
                        {
                            villager.BedPosition = new Vector3(pos.X + dx + 0.5f, pos.Y + dy, pos.Z + dz + 0.5f);
                            return;
                        }
                    }
                }
            }
        }

        private void TryHarvestCrops(Villager villager)
        {
            // 搜索附近的成熟作物
            int searchRadius = 4;
            Vector3i pos = new Vector3i((int)villager.Position.X, (int)villager.Position.Y, (int)villager.Position.Z);

            for (int dx = -searchRadius; dx <= searchRadius; dx++)
            {
                for (int dz = -searchRadius; dz <= searchRadius; dz++)
                {
                    int bx = pos.X + dx;
                    int bz = pos.Z + dz;
                    int by = pos.Y;

                    ushort block = world.GetBlock(bx, by, bz);

                    // 检查作物是否成熟
                    if (IsMatureCrop(block))
                    {
                        float distance = Vector3.Distance(villager.Position, new Vector3(bx + 0.5f, by, bz + 0.5f));
                        if (distance < 2.0f)
                        {
                            // 收获
                            world.SetBlock(bx, by, bz, GameConstants.BLOCK_AIR);
                            world.CreateItemEntity(new Vector3(bx + 0.5f, by + 0.5f, bz + 0.5f), GetCropDrop(block), 1);
                            ItemsGathered++;

                            // 重新种植
                            world.SetBlock(bx, by, bz, GetCropSeed(block));
                        }
                    }
                }
            }
        }

        private bool IsMatureCrop(ushort blockId)
        {
            return blockId == GameConstants.BLOCK_WHEAT ||
                   blockId == GameConstants.BLOCK_CARROTS ||
                   blockId == GameConstants.BLOCK_POTATOES ||
                   blockId == GameConstants.BLOCK_BEETROOTS;
        }

        private ushort GetCropDrop(ushort cropId)
        {
            switch (cropId)
            {
                case GameConstants.BLOCK_WHEAT:
                    return GameConstants.ITEM_WHEAT;
                case GameConstants.BLOCK_CARROTS:
                    return GameConstants.ITEM_CARROT;
                case GameConstants.BLOCK_POTATOES:
                    return GameConstants.ITEM_POTATO;
                case GameConstants.BLOCK_BEETROOTS:
                    return GameConstants.ITEM_BEETROOT;
                default:
                    return GameConstants.ITEM_WHEAT;
            }
        }

        private ushort GetCropSeed(ushort cropId)
        {
            switch (cropId)
            {
                case GameConstants.BLOCK_WHEAT:
                    return GameConstants.BLOCK_WHEAT_SEEDS;
                case GameConstants.BLOCK_CARROTS:
                    return GameConstants.BLOCK_CARROTS;
                case GameConstants.BLOCK_POTATOES:
                    return GameConstants.BLOCK_POTATOES;
                case GameConstants.BLOCK_BEETROOTS:
                    return GameConstants.BLOCK_BEETROOTS;
                default:
                    return GameConstants.BLOCK_WHEAT_SEEDS;
            }
        }

        private void TryBreed(Villager villager1, Villager villager2)
        {
            // 检查繁殖条件
            if (!villager1.CanBreed || !villager2.CanBreed) return;
            if (villager1.BreedCooldown > 0 || villager2.BreedCooldown > 0) return;

            // 生成小村民
            Vector3 spawnPos = (villager1.Position + villager2.Position) / 2;
            Villager baby = entityManager.SpawnEntity(EntityType.Villager, spawnPos) as Villager;

            if (baby != null)
            {
                baby.IsBaby = true;
                baby.Age = 0;
                baby.Profession = VillagerProfession.None;

                // 重置繁殖冷却
                villager1.BreedCooldown = 20 * 60 * 5; // 5分钟
                villager2.BreedCooldown = 20 * 60 * 5;
                villager1.CanBreed = false;
                villager2.CanBreed = false;
            }
        }

        public void CompleteTrade(Villager villager)
        {
            TradesCompleted++;
            villager.TradeUses++;

            // 升级村民等级
            int[] levelThresholds = { 0, 10, 50, 150, 400 };
            for (int i = levelThresholds.Length - 1; i >= 0; i--)
            {
                if (villager.TradeUses >= levelThresholds[i])
                {
                    villager.Level = i + 1;
                    break;
                }
            }
        }

        public void ResetStats()
        {
            TradesCompleted = 0;
            ItemsGathered = 0;
        }
    }

    public enum VillagerActivity
    {
        Idle,
        Working,
        Socializing,
        Sleeping,
        Wandering,
        Fleeing,
        Trading,
        Harvesting
    }
}
