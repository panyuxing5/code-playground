using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Entities
{
    public enum EntityType
    {
        Player,
        Zombie,
        Skeleton,
        Creeper,
        Spider,
        Enderman,
        Cow,
        Pig,
        Sheep,
        Chicken,
        Rabbit,
        Wolf,
        Cat,
        Villager,
        Item,
        ExperienceOrb,
        Arrow,
        Snowball,
        EnderPearl,
        PrimedTNT,
        FallingBlock,
        Boat,
        Minecart,
        Allay,
        Bee,
        Camel,
        Donkey,
        Fox,
        Frog,
        Goat,
        Hoglin,
        Horse,
        Llama,
        Mule,
        Ocelot,
        Parrot,
        Sniffer,
        Strider,
        Tadpole
    }

    public enum StatusEffect
    {
        None,
        Speed,
        Slowness,
        Haste,
        MiningFatigue,
        Strength,
        InstantHealth,
        InstantDamage,
        JumpBoost,
        Nausea,
        Regeneration,
        Resistance,
        FireResistance,
        WaterBreathing,
        Invisibility,
        Blindness,
        NightVision,
        Hunger,
        Weakness,
        Poison,
        Wither,
        HealthBoost,
        Absorption,
        Saturation,
        Glowing,
        Levitation,
        Luck,
        BadLuck,
        SlowFalling,
        ConduitPower,
        DolphinsGrace,
        BadOmen,
        HeroOfTheVillage
    }

    public abstract class Entity
    {
        public int Id { get; set; }
        public EntityType Type { get; protected set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Rotation { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }

        public Vector3 Forward => new Vector3(
            (float)(Math.Sin(Yaw) * Math.Cos(Pitch)),
            (float)Math.Sin(Pitch),
            (float)(Math.Cos(Yaw) * Math.Cos(Pitch))
        );

        public float Width { get; protected set; } = 0.6f;
        public float Height { get; protected set; } = 1.8f;
        public float EyeHeight { get; protected set; } = 1.62f;

        public float Health { get; set; }
        public float MaxHealth { get; protected set; }
        public bool IsDead { get; set; }
        public bool IsOnGround { get; set; }
        public bool IsInWater { get; set; }
        public bool IsInLava { get; set; }
        public bool IsOnFire { get; set; }
        public int FireTicks { get; set; }

        public int Age { get; set; }
        public int Lifetime { get; protected set; } = -1; // -1 = 无限

        public bool NoGravity { get; set; }
        public bool Invulnerable { get; set; }
        public bool CustomNameVisible { get; set; }
        public string CustomName { get; set; }

        public Vector3 LastPosition { get; protected set; }
        public Vector3 Motion { get; set; }

        protected WorldManager world;
        protected static int nextEntityId = 1;

        public Entity(WorldManager world)
        {
            this.world = world;
            Id = nextEntityId++;
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            Rotation = Vector3.Zero;
            Health = 20;
            MaxHealth = 20;
        }

        public void AddEffect(object effect)
        {
            // 添加状态效果
        }

        public void AddEffect(StatusEffect effect, int duration, float amplifier)
        {
            // 添加状态效果
        }

        public virtual void Update(float deltaTime)
        {
            Age++;

            if (Lifetime > 0 && Age > Lifetime)
            {
                Die();
                return;
            }

            LastPosition = Position;

            // 重力
            if (!NoGravity && !IsOnGround)
            {
                Velocity = new Vector3(Velocity.X, Velocity.Y - 25.0f * deltaTime, Velocity.Z);
                Velocity = new Vector3(Velocity.X, Math.Max(Velocity.Y, -50.0f), Velocity.Z);
            }

            // 移动
            Position += Velocity * deltaTime;

            // 碰撞检测
            ResolveCollisions();

            // 火焰
            if (IsOnFire)
            {
                FireTicks--;
                if (FireTicks <= 0)
                {
                    IsOnFire = false;
                }
                if (FireTicks % 20 == 0)
                {
                    TakeDamage(1.0f, "fire");
                }
            }

            // 水/岩浆检测
            CheckLiquidCollisions();

            // 虚空检测
            if (Position.Y < -10)
            {
                TakeDamage(4.0f, "void");
            }
        }

        protected virtual void ResolveCollisions()
        {
            float halfWidth = Width / 2f;

            // 简单的AABB碰撞
            int minX = (int)Math.Floor(Position.X - halfWidth);
            int maxX = (int)Math.Floor(Position.X + halfWidth);
            int minY = (int)Math.Floor(Position.Y);
            int maxY = (int)Math.Floor(Position.Y + Height);
            int minZ = (int)Math.Floor(Position.Z - halfWidth);
            int maxZ = (int)Math.Floor(Position.Z + halfWidth);

            IsOnGround = false;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        ushort block = world.GetBlock(x, y, z);
                        if (IsSolidBlock(block))
                        {
                            // 简单处理：如果脚的位置有方块，站在上面
                            if (Position.Y < y + 1 && Position.Y > y - 0.5f && Velocity.Y <= 0)
                            {
                                Position = new Vector3(Position.X, y + 1, Position.Z);
                                Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
                                IsOnGround = true;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void CheckLiquidCollisions()
        {
            int x = (int)Math.Floor(Position.X);
            int y = (int)Math.Floor(Position.Y + 0.1f);
            int z = (int)Math.Floor(Position.Z);

            ushort block = world.GetBlock(x, y, z);
            IsInWater = block == GameConstants.BLOCK_WATER_STILL || block == GameConstants.BLOCK_WATER_FLOWING;
            IsInLava = block == GameConstants.BLOCK_LAVA_STILL || block == GameConstants.BLOCK_LAVA_FLOWING;

            if (IsInLava && !IsOnFire)
            {
                SetOnFire(300);
            }
        }

        protected bool IsSolidBlock(ushort blockId)
        {
            if (blockId == GameConstants.BLOCK_AIR) return false;
            if (blockId == GameConstants.BLOCK_WATER_STILL) return false;
            if (blockId == GameConstants.BLOCK_WATER_FLOWING) return false;
            if (blockId == GameConstants.BLOCK_LAVA_STILL) return false;
            if (blockId == GameConstants.BLOCK_LAVA_FLOWING) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(blockId);
            return info != null && info.IsSolid;
        }

        public virtual void TakeDamage(float amount, string cause)
        {
            if (Invulnerable || IsDead) return;

            Health -= amount;
            OnDamage(amount, cause);

            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
        }

        public void TakeDamage(float amount)
        {
            TakeDamage(amount, "generic");
        }

        protected virtual void OnDamage(float amount, string cause)
        {
            // 子类可以重写
        }

        public virtual void Heal(float amount)
        {
            if (IsDead) return;
            Health = Math.Min(Health + amount, MaxHealth);
        }

        public virtual void Die()
        {
            IsDead = true;
            OnDeath();
        }

        protected virtual void OnDeath()
        {
            // 子类可以重写，掉落物等
        }

        public virtual void SetOnFire(int ticks)
        {
            IsOnFire = true;
            FireTicks = Math.Max(FireTicks, ticks);
        }

        public virtual void Extinguish()
        {
            IsOnFire = false;
            FireTicks = 0;
        }

        public virtual Vector3 GetEyePosition()
        {
            return Position + new Vector3(0, EyeHeight, 0);
        }

        public virtual float GetDistanceTo(Entity other)
        {
            return Vector3.Distance(Position, other.Position);
        }

        public virtual float GetDistanceTo(Vector3 point)
        {
            return Vector3.Distance(Position, point);
        }

        public virtual bool IsAlive()
        {
            return !IsDead && Health > 0;
        }

        public virtual void Teleport(Vector3 position)
        {
            Position = position;
            Velocity = Vector3.Zero;
        }

        public virtual void Knockback(Vector3 source, float strength)
        {
            Vector3 direction = Position - source;
            direction.Y = 0;
            direction.Normalize();
            Velocity += direction * strength;
            Velocity = new Vector3(Velocity.X, strength * 0.5f, Velocity.Z);
        }

        public abstract void Render();
    }

    // ========================================
    // 生物基类
    // ========================================
    public abstract class Mob : Entity
    {
        public AIState CurrentState { get; set; }
        public Entity Target { get; set; }
        public float MovementSpeed { get; set; } = 1.0f;
        public float AttackDamage { get; set; } = 2.0f;
        public float AttackRange { get; set; } = 1.5f;
        public float DetectionRange { get; set; } = 16.0f;
        public float FollowRange { get; set; } = 32.0f;
        public int AttackCooldown { get; set; }
        public int MaxAttackCooldown { get; set; } = 20;

        public bool IsHostile { get; set; }
        public bool IsPassive { get; set; }
        public bool IsNeutral { get; set; }

        public Vector3 WanderTarget { get; set; }
        public int WanderCooldown { get; set; }

        protected AIBase ai;

        public Mob(WorldManager world) : base(world)
        {
            CurrentState = AIState.Idle;
            ai = new AIBase(world, this);
        }

        public Mob() : this(null)
        {
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (IsDead) return;

            // AI更新
            ai.Update(deltaTime);

            // 攻击冷却
            if (AttackCooldown > 0)
            {
                AttackCooldown--;
            }

            // 状态更新
            UpdateAIState();
        }

        protected virtual void UpdateAIState()
        {
            switch (CurrentState)
            {
                case AIState.Idle:
                    UpdateIdleState();
                    break;
                case AIState.Wander:
                    UpdateWanderState();
                    break;
                case AIState.Chase:
                    UpdateChaseState();
                    break;
                case AIState.Attack:
                    UpdateAttackState();
                    break;
                case AIState.Flee:
                    UpdateFleeState();
                    break;
                case AIState.Eat:
                    UpdateEatState();
                    break;
            }
        }

        protected virtual void UpdateIdleState()
        {
            // 随机游荡
            WanderCooldown--;
            if (WanderCooldown <= 0)
            {
                WanderCooldown = 100 + new Random().Next(200);
                if (new Random().NextDouble() < 0.3f)
                {
                    CurrentState = AIState.Wander;
                    GenerateWanderTarget();
                }
            }
        }

        protected virtual void UpdateWanderState()
        {
            if (WanderTarget == Vector3.Zero)
            {
                CurrentState = AIState.Idle;
                return;
            }

            MoveTowards(WanderTarget, MovementSpeed * 0.5f);

            if (Vector3.Distance(Position, WanderTarget) < 1.0f)
            {
                CurrentState = AIState.Idle;
                WanderCooldown = 100 + new Random().Next(200);
            }
        }

        protected virtual void UpdateChaseState()
        {
            if (Target == null || !Target.IsAlive())
            {
                CurrentState = AIState.Idle;
                Target = null;
                return;
            }

            float distance = GetDistanceTo(Target);

            if (distance > FollowRange)
            {
                CurrentState = AIState.Idle;
                Target = null;
                return;
            }

            if (distance <= AttackRange)
            {
                CurrentState = AIState.Attack;
                return;
            }

            MoveTowards(Target.Position, MovementSpeed);
        }

        protected virtual void UpdateAttackState()
        {
            if (Target == null || !Target.IsAlive())
            {
                CurrentState = AIState.Idle;
                Target = null;
                return;
            }

            float distance = GetDistanceTo(Target);

            if (distance > AttackRange * 1.5f)
            {
                CurrentState = AIState.Chase;
                return;
            }

            // 面向目标
            LookAt(Target.Position);

            // 攻击
            if (AttackCooldown <= 0)
            {
                Attack(Target);
                AttackCooldown = MaxAttackCooldown;
            }
        }

        protected virtual void UpdateFleeState()
        {
            if (Target == null)
            {
                CurrentState = AIState.Idle;
                return;
            }

            Vector3 fleeDirection = Position - Target.Position;
            fleeDirection.Y = 0;
            fleeDirection.Normalize();

            Vector3 fleeTarget = Position + fleeDirection * 10;
            MoveTowards(fleeTarget, MovementSpeed * 1.5f);

            if (GetDistanceTo(Target) > 20)
            {
                CurrentState = AIState.Idle;
                Target = null;
            }
        }

        protected virtual void UpdateEatState()
        {
            // 子类实现
        }

        protected virtual void Attack(Entity target)
        {
            target.TakeDamage(AttackDamage, "mob");
            target.Knockback(Position, 0.5f);
        }

        protected virtual void MoveTowards(Vector3 target, float speed)
        {
            Vector3 direction = target - Position;
            direction.Y = 0;

            if (direction.Length > 0.1f)
            {
                direction.Normalize();
                Velocity = new Vector3(direction.X * speed, Velocity.Y, direction.Z * speed);
                LookAt(target);
            }
        }

        protected virtual void LookAt(Vector3 target)
        {
            Vector3 direction = target - Position;
            Yaw = (float)Math.Atan2(-direction.X, -direction.Z);
            Pitch = (float)Math.Atan2(direction.Y, new Vector2(direction.X, direction.Z).Length);
        }

        protected virtual void GenerateWanderTarget()
        {
            Random random = new Random();
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float distance = 5 + (float)random.NextDouble() * 10;

            WanderTarget = new Vector3(
                Position.X + (float)Math.Cos(angle) * distance,
                Position.Y,
                Position.Z + (float)Math.Sin(angle) * distance
            );
        }

        public virtual void SetTarget(Entity target)
        {
            Target = target;
            if (target != null)
            {
                CurrentState = IsHostile ? AIState.Chase : AIState.Flee;
            }
        }

        protected override void OnDeath()
        {
            // 掉落经验和物品
            DropLoot();
        }

        protected virtual void DropLoot()
        {
            // 子类实现
        }
    }

    public enum AIState
    {
        Idle,
        Wander,
        Chase,
        Attack,
        Flee,
        Eat,
        Sleep,
        Play,
        Follow,
        Mate
    }
}
