using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;

namespace VoxelCraft.Entities
{
    public class AnimalAI
    {
        private readonly WorldManager world;
        private readonly EntityManager entityManager;

        // 动物行为参数
        public float WalkSpeed { get; set; } = 0.1f;
        public float RunSpeed { get; set; } = 0.25f;
        public float DetectionRange { get; set; } = 10.0f;
        public float BreedRange { get; set; } = 2.0f;
        public float FollowRange { get; set; } = 6.0f;

        // 统计
        public int ActiveAnimals { get; private set; }
        public int AnimalsBorn { get; private set; }
        public int AnimalsBred { get; private set; }

        public AnimalAI(WorldManager world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
        }

        public void Initialize()
        {
            Console.WriteLine("[AnimalAI] 动物AI系统初始化完成");
        }

        public void Update(Animal animal, Vector3 playerPosition, int gameTime)
        {
            if (animal.IsDead) return;

            // 更新动物状态
            UpdateAnimalState(animal, playerPosition);

            // 根据状态执行行为
            switch (animal.CurrentState)
            {
                case AnimalState.Idle:
                    UpdateIdle(animal);
                    break;

                case AnimalState.Wandering:
                    UpdateWandering(animal);
                    break;

                case AnimalState.Following:
                    UpdateFollowing(animal, playerPosition);
                    break;

                case AnimalState.Breeding:
                    UpdateBreeding(animal);
                    break;

                case AnimalState.Fleeing:
                    UpdateFleeing(animal);
                    break;

                case AnimalState.Eating:
                    UpdateEating(animal);
                    break;

                case AnimalState.Sleeping:
                    UpdateSleeping(animal, gameTime);
                    break;

                case AnimalState.Attacking:
                    UpdateAttacking(animal, playerPosition);
                    break;
            }

            // 更新繁殖冷却
            if (animal.BreedCooldown > 0)
            {
                animal.BreedCooldown--;
            }

            // 更新生长
            if (animal.IsBaby && animal.Age < animal.AdultAge)
            {
                animal.Age++;
                if (animal.Age >= animal.AdultAge)
                {
                    animal.IsBaby = false;
                }
            }

            ActiveAnimals = entityManager.GetEntities().FindAll(e => e is Animal && !e.IsDead).Count;
        }

        private void UpdateAnimalState(Animal animal, Vector3 playerPosition)
        {
            float distanceToPlayer = Vector3.Distance(animal.Position, playerPosition);

            // 检查是否被攻击
            if (animal.IsPanicked)
            {
                animal.CurrentState = AnimalState.Fleeing;
                return;
            }

            // 检查是否有繁殖意愿
            if (animal.CanBreed && animal.BreedCooldown <= 0 && !animal.IsBaby)
            {
                Animal partner = FindBreedingPartner(animal);
                if (partner != null)
                {
                    animal.CurrentState = AnimalState.Breeding;
                    animal.BreedTarget = partner;
                    return;
                }
            }

            // 检查是否跟随玩家（被食物吸引）
            if (animal.IsFoodItemHeld && distanceToPlayer < FollowRange)
            {
                animal.CurrentState = AnimalState.Following;
                return;
            }

            // 检查是否攻击玩家（敌对动物）
            if (animal.IsHostile && distanceToPlayer < DetectionRange)
            {
                animal.CurrentState = AnimalState.Attacking;
                return;
            }

            // 随机状态转换
            switch (animal.CurrentState)
            {
                case AnimalState.Idle:
                    if (Random.Shared.NextDouble() < 0.02)
                    {
                        animal.CurrentState = AnimalState.Wandering;
                        animal.HasTarget = false;
                    }
                    else if (Random.Shared.NextDouble() < 0.01)
                    {
                        animal.CurrentState = AnimalState.Eating;
                    }
                    break;

                case AnimalState.Wandering:
                    if (!animal.HasTarget || Random.Shared.NextDouble() < 0.01)
                    {
                        animal.CurrentState = AnimalState.Idle;
                    }
                    break;

                case AnimalState.Following:
                    if (!animal.IsFoodItemHeld || distanceToPlayer > FollowRange * 1.5f)
                    {
                        animal.CurrentState = AnimalState.Idle;
                    }
                    break;

                case AnimalState.Fleeing:
                    if (animal.PanicTimer <= 0)
                    {
                        animal.IsPanicked = false;
                        animal.CurrentState = AnimalState.Idle;
                    }
                    break;

                case AnimalState.Eating:
                    if (animal.EatTimer <= 0)
                    {
                        animal.CurrentState = AnimalState.Idle;
                    }
                    break;
            }
        }

        private void UpdateIdle(Animal animal)
        {
            // 随机环顾
            if (Random.Shared.NextDouble() < 0.03)
            {
                animal.Yaw = Random.Shared.Next(360);
            }

            // 偶尔发出声音
            if (Random.Shared.NextDouble() < 0.005)
            {
                PlayAnimalSound(animal);
            }
        }

        private void UpdateWandering(Animal animal)
        {
            // 随机游荡
            if (!animal.HasTarget || Random.Shared.NextDouble() < 0.02)
            {
                animal.TargetPosition = animal.Position + new Vector3(
                    (float)(Random.Shared.NextDouble() - 0.5) * 20,
                    0,
                    (float)(Random.Shared.NextDouble() - 0.5) * 20
                );
                animal.HasTarget = true;
            }

            if (animal.HasTarget)
            {
                float distance = Vector3.Distance(animal.Position, animal.TargetPosition);

                if (distance > 1.0f)
                {
                    MoveTowards(animal, animal.TargetPosition, WalkSpeed);
                }
                else
                {
                    animal.HasTarget = false;
                    animal.CurrentState = AnimalState.Idle;
                }
            }
        }

        private void UpdateFollowing(Animal animal, Vector3 playerPosition)
        {
            float distance = Vector3.Distance(animal.Position, playerPosition);

            if (distance > 2.0f)
            {
                MoveTowards(animal, playerPosition, WalkSpeed * 1.2f);
            }
            else
            {
                // 停在玩家附近
                animal.Velocity *= 0.5f;
                animal.Yaw = CalculateYaw(animal.Position, playerPosition);
            }
        }

        private void UpdateBreeding(Animal animal)
        {
            if (animal.BreedTarget == null || animal.BreedTarget.IsDead)
            {
                animal.CurrentState = AnimalState.Idle;
                return;
            }

            float distance = Vector3.Distance(animal.Position, animal.BreedTarget.Position);

            if (distance > BreedRange)
            {
                MoveTowards(animal, animal.BreedTarget.Position, WalkSpeed);
            }
            else
            {
                // 繁殖
                animal.BreedTimer++;
                if (animal.BreedTimer >= 60)
                {
                    TryBreed(animal, animal.BreedTarget);
                    animal.BreedTimer = 0;
                }
            }
        }

        private void UpdateFleeing(Animal animal)
        {
            animal.PanicTimer--;

            // 逃离威胁
            if (animal.ThreatPosition.HasValue)
            {
                Vector3 fleeDirection = (animal.Position - animal.ThreatPosition.Value).Normalized();
                Vector3 fleeTarget = animal.Position + fleeDirection * 15.0f;

                MoveTowards(animal, fleeTarget, RunSpeed);

                float distance = Vector3.Distance(animal.Position, animal.ThreatPosition.Value);
                if (distance > 20.0f)
                {
                    animal.ThreatPosition = null;
                }
            }
            else
            {
                // 随机逃跑
                if (!animal.HasTarget)
                {
                    animal.TargetPosition = animal.Position + new Vector3(
                        (float)(Random.Shared.NextDouble() - 0.5) * 30,
                        0,
                        (float)(Random.Shared.NextDouble() - 0.5) * 30
                    );
                    animal.HasTarget = true;
                }

                if (animal.HasTarget)
                {
                    MoveTowards(animal, animal.TargetPosition, RunSpeed);
                }
            }
        }

        private void UpdateEating(Animal animal)
        {
            animal.EatTimer--;

            // 吃东西动画
            if (animal.EatTimer % 10 == 0)
            {
                // 播放吃东西的粒子效果
            }

            if (animal.EatTimer <= 0)
            {
                // 恢复生命值
                if (animal.Health < animal.MaxHealth)
                {
                    animal.Health += 2;
                }
            }
        }

        private void UpdateSleeping(Animal animal, int gameTime)
        {
            // 检查是否是白天
            int timeOfDay = gameTime % 24000;
            bool isDay = timeOfDay >= 1000 && timeOfDay <= 11000;

            if (isDay && animal.SleepsDuringNight)
            {
                animal.CurrentState = AnimalState.Idle;
                animal.IsSleeping = false;
            }
            else if (!isDay && !animal.SleepsDuringNight)
            {
                animal.CurrentState = AnimalState.Idle;
                animal.IsSleeping = false;
            }
        }

        private void UpdateAttacking(Animal animal, Vector3 playerPosition)
        {
            float distance = Vector3.Distance(animal.Position, playerPosition);

            if (distance > 1.5f)
            {
                MoveTowards(animal, playerPosition, RunSpeed);
            }
            else
            {
                // 攻击
                if (animal.AttackCooldown <= 0)
                {
                    // 造成伤害
                    animal.AttackCooldown = 20;
                }
            }

            animal.Yaw = CalculateYaw(animal.Position, playerPosition);

            if (animal.AttackCooldown > 0)
            {
                animal.AttackCooldown--;
            }
        }

        private void MoveTowards(Animal animal, Vector3 target, float speed)
        {
            Vector3 direction = (target - animal.Position).Normalized();
            direction.Y = 0;

            animal.Velocity = new Vector3(direction.X * speed, animal.Velocity.Y, animal.Velocity.Z);
            animal.Velocity = new Vector3(animal.Velocity.X, animal.Velocity.Y, direction.Z * speed);

            animal.Yaw = CalculateYaw(animal.Position, target);

            // 检查是否需要跳
            if (IsBlockAhead(animal))
            {
                if (animal.OnGround)
                {
                    animal.Velocity = new Vector3(animal.Velocity.X, 0.4f, animal.Velocity.Z);
                }
            }
        }

        private bool IsBlockAhead(Animal animal)
        {
            Vector3 ahead = animal.Position + animal.GetLookVector() * 0.5f;
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

        private Animal FindBreedingPartner(Animal animal)
        {
            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity is Animal other && !other.IsDead && other.Id != animal.Id)
                {
                    if (other.GetType() == animal.GetType() &&
                        other.CanBreed && !other.IsBaby && other.BreedCooldown <= 0)
                    {
                        float distance = Vector3.Distance(animal.Position, other.Position);
                        if (distance < 8.0f)
                        {
                            return other;
                        }
                    }
                }
            }
            return null;
        }

        private void TryBreed(Animal animal1, Animal animal2)
        {
            if (!animal1.CanBreed || !animal2.CanBreed) return;
            if (animal1.BreedCooldown > 0 || animal2.BreedCooldown > 0) return;

            // 生成后代
            Vector3 spawnPos = (animal1.Position + animal2.Position) / 2;
            Animal baby = entityManager.SpawnEntity(animal1.EntityType, spawnPos) as Animal;

            if (baby != null)
            {
                baby.IsBaby = true;
                baby.Age = 0;
                baby.AdultAge = 20 * 60 * 20; // 20分钟

                // 设置繁殖冷却
                animal1.BreedCooldown = 20 * 60 * 5; // 5分钟
                animal2.BreedCooldown = 20 * 60 * 5;
                animal1.CanBreed = false;
                animal2.CanBreed = false;

                AnimalsBorn++;
                AnimalsBred++;

                // 繁殖粒子效果
                world.CreateParticleEffect(spawnPos, ParticleType.Heart, 10);
            }

            animal1.CurrentState = AnimalState.Idle;
            animal2.CurrentState = AnimalState.Idle;
        }

        public void FeedAnimal(Animal animal, int foodItemId)
        {
            if (IsFavoriteFood(animal, foodItemId))
            {
                // 恢复生命值
                animal.Health = Math.Min(animal.MaxHealth, animal.Health + 4);

                // 进入繁殖模式
                if (!animal.IsBaby)
                {
                    animal.CanBreed = true;
                    world.CreateParticleEffect(animal.Position, ParticleType.Heart, 5);
                }
                else
                {
                    // 加速生长
                    animal.Age += 20 * 60; // 加速1分钟
                }
            }
        }

        public bool IsFavoriteFood(Animal animal, int itemId)
        {
            switch (animal.EntityType)
            {
                case EntityType.Cow:
                case EntityType.Sheep:
                    return itemId == GameConstants.ITEM_WHEAT;

                case EntityType.Pig:
                    return itemId == GameConstants.ITEM_CARROT ||
                           itemId == GameConstants.ITEM_POTATO ||
                           itemId == GameConstants.ITEM_BEETROOT;

                case EntityType.Chicken:
                    return itemId == GameConstants.ITEM_WHEAT_SEEDS ||
                           itemId == GameConstants.ITEM_BEETROOT_SEEDS ||
                           itemId == GameConstants.ITEM_MELON_SEEDS ||
                           itemId == GameConstants.ITEM_PUMPKIN_SEEDS;

                case EntityType.Rabbit:
                    return itemId == GameConstants.ITEM_CARROT ||
                           itemId == GameConstants.ITEM_GOLDEN_CARROT ||
                           itemId == GameConstants.ITEM_DANDELION;

                case EntityType.Wolf:
                    return itemId == GameConstants.ITEM_BONE ||
                           itemId == GameConstants.ITEM_BEEF ||
                           itemId == GameConstants.ITEM_PORKCHOP ||
                           itemId == GameConstants.ITEM_CHICKEN ||
                           itemId == GameConstants.ITEM_RABBIT_MEAT ||
                           itemId == GameConstants.ITEM_MUTTON;

                case EntityType.Horse:
                case EntityType.Donkey:
                case EntityType.Mule:
                    return itemId == GameConstants.ITEM_WHEAT ||
                           itemId == GameConstants.ITEM_SUGAR ||
                           itemId == GameConstants.ITEM_HAY_BLOCK ||
                           itemId == GameConstants.ITEM_GOLDEN_CARROT ||
                           itemId == GameConstants.ITEM_GOLDEN_APPLE;

                case EntityType.Llama:
                    return itemId == GameConstants.ITEM_HAY_BLOCK;

                case EntityType.Cat:
                case EntityType.Ocelot:
                    return itemId == GameConstants.ITEM_COD ||
                           itemId == GameConstants.ITEM_SALMON ||
                           itemId == GameConstants.ITEM_TROPICAL_FISH ||
                           itemId == GameConstants.ITEM_PUFFERFISH;

                case EntityType.Parrot:
                    return itemId == GameConstants.ITEM_WHEAT_SEEDS ||
                           itemId == GameConstants.ITEM_MELON_SEEDS ||
                           itemId == GameConstants.ITEM_PUMPKIN_SEEDS ||
                           itemId == GameConstants.ITEM_BEETROOT_SEEDS;

                case EntityType.Fox:
                    return itemId == GameConstants.ITEM_SWEET_BERRIES;

                case EntityType.Bee:
                    return itemId == GameConstants.ITEM_HONEY_BOTTLE ||
                           itemId == GameConstants.BLOCK_POPPY ||
                           itemId == GameConstants.BLOCK_DANDELION;

                case EntityType.Hoglin:
                    return itemId == GameConstants.BLOCK_CRIMSON_FUNGUS;

                case EntityType.Strider:
                    return itemId == GameConstants.ITEM_WARPED_FUNGUS;

                case EntityType.Goat:
                    return itemId == GameConstants.ITEM_WHEAT;

                case EntityType.Frog:
                    return itemId == GameConstants.ITEM_SLIMEBALL;

                case EntityType.Tadpole:
                    return false; // 蝌蚪不能喂食

                case EntityType.Allay:
                    return false; // 悦灵不能喂食繁殖

                case EntityType.Sniffer:
                    return itemId == GameConstants.ITEM_TORCHFLOWER_SEEDS;

                case EntityType.Camel:
                    return itemId == GameConstants.ITEM_CACTUS;

                default:
                    return false;
            }
        }

        public void TameAnimal(Animal animal, int itemId)
        {
            if (animal.IsTamed) return;

            bool canTame = false;
            float tameChance = 0f;

            switch (animal.EntityType)
            {
                case EntityType.Wolf:
                    canTame = itemId == GameConstants.ITEM_BONE;
                    tameChance = 0.33f;
                    break;

                case EntityType.Cat:
                case EntityType.Ocelot:
                    canTame = itemId == GameConstants.ITEM_COD || itemId == GameConstants.ITEM_SALMON;
                    tameChance = 0.33f;
                    break;

                case EntityType.Horse:
                case EntityType.Donkey:
                case EntityType.Mule:
                    canTame = true; // 需要空手骑乘驯服
                    tameChance = 0.2f;
                    break;

                case EntityType.Llama:
                    canTame = true;
                    tameChance = 0.15f;
                    break;

                case EntityType.Parrot:
                    canTame = itemId == GameConstants.ITEM_WHEAT_SEEDS ||
                              itemId == GameConstants.ITEM_MELON_SEEDS;
                    tameChance = 0.33f;
                    break;
            }

            if (canTame && Random.Shared.NextDouble() < tameChance)
            {
                animal.IsTamed = true;
                world.CreateParticleEffect(animal.Position, ParticleType.Heart, 10);
            }
            else
            {
                world.CreateParticleEffect(animal.Position, ParticleType.Smoke, 5);
            }
        }

        private void PlayAnimalSound(Animal animal)
        {
            // 播放动物声音（简化）
        }

        public void PanicAnimal(Animal animal, Vector3 threatPosition)
        {
            animal.IsPanicked = true;
            animal.PanicTimer = 100;
            animal.ThreatPosition = threatPosition;
            animal.CurrentState = AnimalState.Fleeing;
        }

        public void ResetStats()
        {
            AnimalsBorn = 0;
            AnimalsBred = 0;
        }
    }

    public class Animal : Mob
    {
        public AnimalState CurrentState;
        public bool IsBaby;
        public int Age;
        public int AdultAge;
        public bool CanBreed;
        public int BreedCooldown;
        public Animal BreedTarget;
        public int BreedTimer;
        public bool IsTamed;
        public bool IsFoodItemHeld;
        public bool IsPanicked;
        public int PanicTimer;
        public Vector3? ThreatPosition;
        public int EatTimer;
        public bool IsSleeping;
        public bool SleepsDuringNight = true;
        public bool IsHostile;
        public int AttackCooldown;
        public bool HasTarget;
        public Vector3 TargetPosition;

        public Animal()
        {
            CurrentState = AnimalState.Idle;
            IsBaby = false;
            Age = 0;
            AdultAge = 20 * 60 * 20;
            CanBreed = false;
            IsTamed = false;
        }

        public override void Render()
        {
            // 动物渲染由EntityRenderer处理
        }

        // 扩展属性和方法
        public new EntityType EntityType { get; set; }
        public new bool OnGround { get; set; }

        public Vector3 GetLookVector()
        {
            return Forward;
        }
    }

    public enum AnimalState
    {
        Idle,
        Wandering,
        Following,
        Breeding,
        Fleeing,
        Eating,
        Sleeping,
        Attacking
    }
}
