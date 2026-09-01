using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Player;

namespace VoxelCraft.Entities
{
    public class EntityManager
    {
        private readonly WorldManager world;
        private readonly List<Entity> entities;
        private readonly Dictionary<int, Entity> entityById;
        private readonly Queue<Entity> entitiesToAdd;
        private readonly Queue<int> entitiesToRemove;

        // 实体限制
        public int MaxEntities { get; set; } = 1000;
        public int MaxMobSpawnRange { get; set; } = 128;
        public int MobSpawnInterval { get; set; } = 200; // ticks

        // 刷怪
        private int mobSpawnTimer;
        private readonly Random random;

        // 统计
        public int EntityCount => entities.Count;
        public int MobCount { get; private set; }
        public int ItemEntityCount { get; private set; }
        public int ProjectileCount { get; private set; }

        public EntityManager(WorldManager world)
        {
            this.world = world;
            entities = new List<Entity>();
            entityById = new Dictionary<int, Entity>();
            entitiesToAdd = new Queue<Entity>();
            entitiesToRemove = new Queue<int>();
            random = new Random();
        }

        public void Initialize()
        {
            Console.WriteLine("[EntityManager] 实体管理器初始化完成");
        }

        public void Update(float deltaTime, PlayerController player)
        {
            // 添加待添加的实体
            while (entitiesToAdd.Count > 0)
            {
                Entity entity = entitiesToAdd.Dequeue();
                entities.Add(entity);
                entityById[entity.Id] = entity;
            }

            // 移除待移除的实体
            while (entitiesToRemove.Count > 0)
            {
                int id = entitiesToRemove.Dequeue();
                if (entityById.TryGetValue(id, out Entity entity))
                {
                    entities.Remove(entity);
                    entityById.Remove(id);
                }
            }

            // 更新所有实体
            foreach (Entity entity in entities)
            {
                if (entity.IsDead) continue;

                // 只更新玩家附近的实体
                if (player != null)
                {
                    float distance = entity.GetDistanceTo(player.Position);
                    if (distance > 256) continue; // 太远的实体不更新
                }

                entity.Update(deltaTime);
            }

            // 清理死亡实体
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                if (entities[i].IsDead)
                {
                    entityById.Remove(entities[i].Id);
                    entities.RemoveAt(i);
                }
            }

            // 刷怪
            mobSpawnTimer++;
            if (mobSpawnTimer >= MobSpawnInterval)
            {
                mobSpawnTimer = 0;
                SpawnMobs(player);
            }

            // 更新统计
            UpdateStats();
        }

        private void UpdateStats()
        {
            MobCount = 0;
            ItemEntityCount = 0;
            ProjectileCount = 0;

            foreach (Entity entity in entities)
            {
                if (entity is Mob) MobCount++;
                else if (entity is ItemEntity) ItemEntityCount++;
                else if (entity is Projectile) ProjectileCount++;
            }
        }

        public void AddEntity(Entity entity)
        {
            if (entities.Count >= MaxEntities)
            {
                Console.WriteLine("[EntityManager] 实体数量已达上限");
                return;
            }
            entitiesToAdd.Enqueue(entity);
        }

        public void RemoveEntity(int entityId)
        {
            entitiesToRemove.Enqueue(entityId);
        }

        public Entity GetEntity(int id)
        {
            entityById.TryGetValue(id, out Entity entity);
            return entity;
        }

        public List<Entity> GetEntitiesInRange(Vector3 position, float range)
        {
            List<Entity> result = new List<Entity>();
            foreach (Entity entity in entities)
            {
                if (entity.GetDistanceTo(position) <= range)
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        public List<T> GetEntitiesOfType<T>(Vector3 position, float range) where T : Entity
        {
            List<T> result = new List<T>();
            foreach (Entity entity in entities)
            {
                if (entity is T typed && entity.GetDistanceTo(position) <= range)
                {
                    result.Add(typed);
                }
            }
            return result;
        }

        public List<Entity> GetAllEntities()
        {
            return new List<Entity>(entities);
        }

        // ========================================
        // 刷怪系统
        // ========================================
        private void SpawnMobs(PlayerController player)
        {
            if (player == null) return;
            if (MobCount > 200) return; // 怪物上限

            int spawnAttempts = random.Next(1, 5);
            for (int i = 0; i < spawnAttempts; i++)
            {
                Vector3 spawnPos = FindSpawnPosition(player.Position);
                if (spawnPos == Vector3.Zero) continue;

                // 根据位置和时间选择生物类型
                EntityType type = ChooseMobType(spawnPos);
                SpawnMob(type, spawnPos);
            }
        }

        private Vector3 FindSpawnPosition(Vector3 playerPos)
        {
            for (int attempt = 0; attempt < 10; attempt++)
            {
                float angle = (float)(random.NextDouble() * Math.PI * 2);
                float distance = 24 + random.Next(40);

                int x = (int)(playerPos.X + Math.Cos(angle) * distance);
                int z = (int)(playerPos.Z + Math.Sin(angle) * distance);

                // 找到地面
                for (int y = GameConstants.CHUNK_HEIGHT - 1; y > 0; y--)
                {
                    ushort block = world.GetBlock(x, y, z);
                    if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_WATER_STILL)
                    {
                        ushort above = world.GetBlock(x, y + 1, z);
                        ushort above2 = world.GetBlock(x, y + 2, z);

                        if (above == GameConstants.BLOCK_AIR && above2 == GameConstants.BLOCK_AIR)
                        {
                            return new Vector3(x + 0.5f, y + 1, z + 0.5f);
                        }
                        break;
                    }
                }
            }

            return Vector3.Zero;
        }

        private EntityType ChooseMobType(Vector3 position)
        {
            // 简化：根据高度和生物群系选择
            float dayTime = GameEngine.DayTime;
            bool isNight = dayTime < 0.25f || dayTime > 0.75f;

            if (isNight)
            {
                // 夜晚生成敌对生物
                int r = random.Next(100);
                if (r < 30) return EntityType.Zombie;
                if (r < 55) return EntityType.Skeleton;
                if (r < 75) return EntityType.Creeper;
                if (r < 90) return EntityType.Spider;
                return EntityType.Enderman;
            }
            else
            {
                // 白天生成被动生物
                int r = random.Next(100);
                if (r < 25) return EntityType.Cow;
                if (r < 45) return EntityType.Pig;
                if (r < 65) return EntityType.Sheep;
                if (r < 80) return EntityType.Chicken;
                if (r < 90) return EntityType.Rabbit;
                return EntityType.Wolf;
            }
        }

        private void SpawnMob(EntityType type, Vector3 position)
        {
            Mob mob = type switch
            {
                EntityType.Zombie => new Zombie(world),
                EntityType.Skeleton => new Skeleton(world),
                EntityType.Creeper => new Creeper(world),
                EntityType.Spider => new Spider(world),
                EntityType.Enderman => new Enderman(world),
                EntityType.Cow => new Cow(world),
                EntityType.Pig => new Pig(world),
                EntityType.Sheep => new Sheep(world),
                EntityType.Chicken => new Chicken(world),
                EntityType.Rabbit => new Rabbit(world),
                EntityType.Wolf => new Wolf(world),
                EntityType.Villager => new Villager(world),
                _ => null
            };

            if (mob != null)
            {
                mob.Position = position;
                AddEntity(mob);
            }
        }

        public void SpawnItemEntity(int itemId, int count, Vector3 position)
        {
            ItemEntity item = new ItemEntity(world, itemId, count)
            {
                Position = position,
                Velocity = new Vector3(
                    (float)(random.NextDouble() - 0.5) * 2,
                    2.0f,
                    (float)(random.NextDouble() - 0.5) * 2
                )
            };
            AddEntity(item);
        }

        public void SpawnExperienceOrb(int amount, Vector3 position)
        {
            // 简化：直接给玩家经验
        }

        public void DamageEntitiesInRange(Vector3 position, float range, float damage, string cause)
        {
            foreach (Entity entity in entities)
            {
                if (entity.IsDead) continue;
                if (entity.GetDistanceTo(position) <= range)
                {
                    entity.TakeDamage(damage, cause);
                }
            }
        }

        public void ClearAll()
        {
            entities.Clear();
            entityById.Clear();
            entitiesToAdd.Clear();
            entitiesToRemove.Clear();
            Console.WriteLine("[EntityManager] 所有实体已清除");
        }

        public void Dispose()
        {
            ClearAll();
            Console.WriteLine("[EntityManager] 实体管理器已释放");
        }
    }

    // ========================================
    // 物品实体
    // ========================================
    public class ItemEntity : Entity
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public int PickupDelay { get; set; } = 40;
        public int Age2 { get; set; }
        public int DespawnTime { get; set; } = 6000; // 5分钟

        public ItemEntity(WorldManager world, int itemId, int count) : base(world)
        {
            Type = EntityType.Item;
            ItemId = itemId;
            Count = count;
            Width = 0.25f;
            Height = 0.25f;
            MaxHealth = 5;
            Health = 5;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Age2++;
            if (PickupDelay > 0) PickupDelay--;

            // 物品旋转
            Yaw += deltaTime * 2;

            // 自动消失
            if (Age2 > DespawnTime)
            {
                Die();
            }
        }

        protected override void OnDeath()
        {
            // 物品消失
        }

        public override void Render()
        {
            // 渲染物品模型
        }
    }

    // ========================================
    // 经验球
    // ========================================
    public class ExperienceOrb : Entity
    {
        public int Experience { get; set; }
        public int Age2 { get; set; }

        public ExperienceOrb(WorldManager world, int experience) : base(world)
        {
            Type = EntityType.ExperienceOrb;
            Experience = experience;
            Width = 0.25f;
            Height = 0.25f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Age2++;

            // 被玩家吸引
            // 简化处理
        }

        public override void Render()
        {
        }
    }

    // ========================================
    // 抛射物基类
    // ========================================
    public abstract class Projectile : Entity
    {
        public Entity Shooter { get; set; }
        public float Damage { get; set; }
        public int Life { get; set; } = 600;

        protected Projectile(WorldManager world) : base(world)
        {
            NoGravity = false;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Life--;

            if (Life <= 0)
            {
                Die();
                return;
            }

            // 碰撞检测
            CheckCollision();
        }

        protected virtual void CheckCollision()
        {
            // 检测与方块的碰撞
            int x = (int)Math.Floor(Position.X);
            int y = (int)Math.Floor(Position.Y);
            int z = (int)Math.Floor(Position.Z);

            ushort block = world.GetBlock(x, y, z);
            if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_WATER_STILL)
            {
                OnHitBlock(x, y, z);
            }
        }

        protected virtual void OnHitBlock(int x, int y, int z)
        {
            Die();
        }

        protected virtual void OnHitEntity(Entity entity)
        {
            entity.TakeDamage(Damage, "projectile");
            Die();
        }
    }

    public class Arrow : Projectile
    {
        public Arrow(WorldManager world) : base(world)
        {
            Type = EntityType.Arrow;
            Damage = 2.0f;
            Width = 0.1f;
            Height = 0.1f;
        }

        public override void Render()
        {
        }
    }

    public class Snowball : Projectile
    {
        public Snowball(WorldManager world) : base(world)
        {
            Type = EntityType.Snowball;
            Damage = 0;
            Width = 0.15f;
            Height = 0.15f;
        }

        protected override void OnHitBlock(int x, int y, int z)
        {
            // 生成雪球粒子
            base.OnHitBlock(x, y, z);
        }

        public override void Render()
        {
        }
    }

    public class EnderPearl : Projectile
    {
        public EnderPearl(WorldManager world) : base(world)
        {
            Type = EntityType.EnderPearl;
            Damage = 5.0f;
            Width = 0.15f;
            Height = 0.15f;
        }

        protected override void OnHitBlock(int x, int y, int z)
        {
            // 传送投掷者
            if (Shooter != null)
            {
                Shooter.Teleport(new Vector3(x + 0.5f, y + 1, z + 0.5f));
                Shooter.TakeDamage(5.0f, "ender_pearl");
            }
            base.OnHitBlock(x, y, z);
        }

        public override void Render()
        {
        }
    }

    // ========================================
    // 点燃的TNT
    // ========================================
    public class PrimedTNT : Entity
    {
        public int Fuse { get; set; } = 80;
        public float ExplosionRadius { get; set; } = 4.0f;

        public PrimedTNT(WorldManager world) : base(world)
        {
            Type = EntityType.PrimedTNT;
            Width = 0.98f;
            Height = 0.98f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Fuse--;

            if (Fuse <= 0)
            {
                Explode();
            }
        }

        private void Explode()
        {
            world.CreateExplosion(Position, (int)ExplosionRadius);
            Die();
        }

        public override void Render()
        {
        }
    }

    // ========================================
    // 掉落的方块
    // ========================================
    public class FallingBlock : Entity
    {
        public ushort BlockId { get; set; }

        public FallingBlock(WorldManager world, ushort blockId) : base(world)
        {
            Type = EntityType.FallingBlock;
            BlockId = blockId;
            Width = 0.98f;
            Height = 0.98f;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (IsOnGround)
            {
                // 放置方块
                int x = (int)Math.Floor(Position.X);
                int y = (int)Math.Floor(Position.Y);
                int z = (int)Math.Floor(Position.Z);

                if (world.GetBlock(x, y, z) == GameConstants.BLOCK_AIR)
                {
                    world.SetBlock(x, y, z, BlockId);
                }
                Die();
            }
        }

        public override void Render()
        {
        }
    }
}
