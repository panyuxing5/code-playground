using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Entities
{
    public class Zombie : Mob
    {
        public Zombie(WorldManager world) : base(world)
        {
            Type = EntityType.Zombie;
            MaxHealth = 20;
            Health = 20;
            Width = 0.6f;
            Height = 1.95f;
            EyeHeight = 1.74f;
            MovementSpeed = 0.23f;
            AttackDamage = 3.0f;
            AttackRange = 1.5f;
            DetectionRange = 35.0f;
            FollowRange = 40.0f;
            IsHostile = true;
            MaxAttackCooldown = 20;
            CustomName = "僵尸";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 僵尸在阳光下燃烧
            if (IsInDaylight() && !IsOnFire && !IsInWater)
            {
                SetOnFire(160);
            }
        }

        private bool IsInDaylight()
        {
            // 简化：假设白天
            return true;
        }

        protected override void DropLoot()
        {
            // 掉落腐肉
            Random random = new Random();
            int rottenFlesh = random.Next(0, 3);

            // 稀有掉落
            if (random.NextDouble() < 0.025)
            {
                // 铁锭/胡萝卜/土豆
            }
        }

        public override void Render()
        {
            // 渲染僵尸模型
        }
    }

    public class Skeleton : Mob
    {
        public int ArrowCount { get; set; } = 15;
        public int ShootCooldown { get; set; }

        public Skeleton(WorldManager world) : base(world)
        {
            Type = EntityType.Skeleton;
            MaxHealth = 20;
            Health = 20;
            Width = 0.6f;
            Height = 1.99f;
            EyeHeight = 1.74f;
            MovementSpeed = 0.25f;
            AttackDamage = 2.0f;
            AttackRange = 15.0f;
            DetectionRange = 16.0f;
            FollowRange = 20.0f;
            IsHostile = true;
            MaxAttackCooldown = 40;
            CustomName = "骷髅";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 骷髅在阳光下燃烧
            if (IsInDaylight() && !IsOnFire && !IsInWater)
            {
                SetOnFire(160);
            }

            // 远程攻击
            if (CurrentState == AIState.Attack && Target != null)
            {
                if (ShootCooldown <= 0)
                {
                    ShootArrow(Target);
                    ShootCooldown = 60;
                }
                else
                {
                    ShootCooldown--;
                }
            }
        }

        private void ShootArrow(Entity target)
        {
            if (ArrowCount <= 0) return;
            ArrowCount--;

            // 创建箭实体
            Vector3 direction = target.GetEyePosition() - GetEyePosition();
            direction.Normalize();

            // Arrow arrow = new Arrow(world, GetEyePosition(), direction * 3.0f);
            // EntityManager.AddEntity(arrow);
        }

        private bool IsInDaylight()
        {
            return true;
        }

        protected override void DropLoot()
        {
            Random random = new Random();
            int bones = random.Next(0, 3);
            int arrows = random.Next(0, 3);
        }

        public override void Render()
        {
            // 渲染骷髅模型
        }
    }

    public class Creeper : Mob
    {
        public int FuseTime { get; set; }
        public bool IsPrimed { get; set; }
        public float ExplosionRadius { get; set; } = 3.0f;

        public Creeper(WorldManager world) : base(world)
        {
            Type = EntityType.Creeper;
            MaxHealth = 20;
            Health = 20;
            Width = 0.6f;
            Height = 1.7f;
            EyeHeight = 1.5f;
            MovementSpeed = 0.25f;
            AttackDamage = 0; // 爆炸伤害
            AttackRange = 3.0f;
            DetectionRange = 16.0f;
            FollowRange = 20.0f;
            IsHostile = true;
            CustomName = "爬行者";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Target != null && CurrentState == AIState.Chase)
            {
                float distance = GetDistanceTo(Target);
                if (distance < 3.0f)
                {
                    // 开始爆炸倒计时
                    if (!IsPrimed)
                    {
                        IsPrimed = true;
                        FuseTime = 30; // 1.5秒
                    }
                }
                else if (distance > 6.0f)
                {
                    // 取消爆炸
                    IsPrimed = false;
                    FuseTime = 0;
                }
            }

            if (IsPrimed)
            {
                FuseTime--;
                if (FuseTime <= 0)
                {
                    Explode();
                }
            }
        }

        private void Explode()
        {
            // 创建爆炸
            world.CreateExplosion(Position, (int)ExplosionRadius);
            Die();
        }

        protected override void DropLoot()
        {
            // 掉落火药
        }

        public override void Render()
        {
            // 渲染爬行者模型
        }
    }

    public class Spider : Mob
    {
        public Spider(WorldManager world) : base(world)
        {
            Type = EntityType.Spider;
            MaxHealth = 16;
            Health = 16;
            Width = 1.4f;
            Height = 0.9f;
            EyeHeight = 0.65f;
            MovementSpeed = 0.3f;
            AttackDamage = 2.0f;
            AttackRange = 1.5f;
            DetectionRange = 16.0f;
            FollowRange = 20.0f;
            IsHostile = true;
            CustomName = "蜘蛛";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 蜘蛛可以爬墙
            // 简化处理
        }

        protected override void DropLoot()
        {
            // 掉落线和蜘蛛眼
        }

        public override void Render()
        {
            // 渲染蜘蛛模型
        }
    }

    public class Enderman : Mob
    {
        public ushort CarriedBlock { get; set; }
        public int TeleportCooldown { get; set; }

        public Enderman(WorldManager world) : base(world)
        {
            Type = EntityType.Enderman;
            MaxHealth = 40;
            Health = 40;
            Width = 0.6f;
            Height = 2.9f;
            EyeHeight = 2.55f;
            MovementSpeed = 0.3f;
            AttackDamage = 7.0f;
            AttackRange = 3.0f;
            DetectionRange = 64.0f;
            FollowRange = 64.0f;
            IsNeutral = true;
            CustomName = "末影人";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 随机传送
            if (TeleportCooldown <= 0 && new Random().NextDouble() < 0.01)
            {
                TeleportRandomly();
                TeleportCooldown = 100;
            }
            else
            {
                TeleportCooldown--;
            }

            // 搬方块
            if (CarriedBlock == 0 && new Random().NextDouble() < 0.005)
            {
                PickUpBlock();
            }
        }

        private void TeleportRandomly()
        {
            Random random = new Random();
            Vector3 newPos = Position + new Vector3(
                (float)(random.NextDouble() - 0.5) * 32,
                (float)(random.NextDouble() - 0.5) * 16,
                (float)(random.NextDouble() - 0.5) * 32
            );
            Teleport(newPos);
        }

        private void PickUpBlock()
        {
            int x = (int)Position.X;
            int y = (int)Position.Y;
            int z = (int)Position.Z;

            ushort block = world.GetBlock(x, y - 1, z);
            if (block != GameConstants.BLOCK_AIR)
            {
                CarriedBlock = block;
                world.SetBlock(x, y - 1, z, GameConstants.BLOCK_AIR);
            }
        }

        protected override void DropLoot()
        {
            // 掉落末影珍珠
        }

        public override void Render()
        {
            // 渲染末影人模型
        }
    }

    // ========================================
    // 被动生物
    // ========================================
    public class Cow : Mob
    {
        public Cow(WorldManager world) : base(world)
        {
            Type = EntityType.Cow;
            MaxHealth = 10;
            Health = 10;
            Width = 0.9f;
            Height = 1.4f;
            EyeHeight = 1.2f;
            MovementSpeed = 0.2f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "牛";
        }

        protected override void DropLoot()
        {
            Random random = new Random();
            int beef = random.Next(1, 4);
            int leather = random.Next(0, 2);
        }

        public override void Render()
        {
            // 渲染牛模型
        }
    }

    public class Pig : Mob
    {
        public Pig(WorldManager world) : base(world)
        {
            Type = EntityType.Pig;
            MaxHealth = 10;
            Health = 10;
            Width = 0.9f;
            Height = 0.9f;
            EyeHeight = 0.7f;
            MovementSpeed = 0.25f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "猪";
        }

        protected override void DropLoot()
        {
            Random random = new Random();
            int porkchop = random.Next(1, 4);
        }

        public override void Render()
        {
            // 渲染猪模型
        }
    }

    public class Sheep : Mob
    {
        public int WoolColor { get; set; } = 0; // 0 = 白色
        public bool IsSheared { get; set; }

        public Sheep(WorldManager world) : base(world)
        {
            Type = EntityType.Sheep;
            MaxHealth = 8;
            Health = 8;
            Width = 0.9f;
            Height = 1.3f;
            EyeHeight = 1.1f;
            MovementSpeed = 0.23f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "羊";
        }

        public void Shear()
        {
            if (!IsSheared)
            {
                IsSheared = true;
                // 掉落羊毛
            }
        }

        protected override void DropLoot()
        {
            Random random = new Random();
            int mutton = random.Next(1, 3);
            if (!IsSheared)
            {
                // 掉落1个羊毛
            }
        }

        public override void Render()
        {
            // 渲染羊模型
        }
    }

    public class Chicken : Mob
    {
        public Chicken(WorldManager world) : base(world)
        {
            Type = EntityType.Chicken;
            MaxHealth = 4;
            Health = 4;
            Width = 0.4f;
            Height = 0.7f;
            EyeHeight = 0.55f;
            MovementSpeed = 0.25f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "鸡";
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // 鸡可以缓降
            if (Velocity.Y < -2.0f)
            {
                Velocity.Y = -2.0f;
            }
        }

        protected override void DropLoot()
        {
            Random random = new Random();
            int feather = random.Next(0, 3);
            int chicken = random.Next(1, 2);
        }

        public override void Render()
        {
            // 渲染鸡模型
        }
    }

    public class Rabbit : Mob
    {
        public Rabbit(WorldManager world) : base(world)
        {
            Type = EntityType.Rabbit;
            MaxHealth = 3;
            Health = 3;
            Width = 0.4f;
            Height = 0.5f;
            EyeHeight = 0.4f;
            MovementSpeed = 0.3f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "兔子";
        }

        protected override void DropLoot()
        {
            // 掉落兔子皮和兔肉
        }

        public override void Render()
        {
            // 渲染兔子模型
        }
    }

    public class Wolf : Mob
    {
        public bool IsTamed { get; set; }
        public int CollarColor { get; set; } = 14; // 红色

        public Wolf(WorldManager world) : base(world)
        {
            Type = EntityType.Wolf;
            MaxHealth = 8;
            Health = 8;
            Width = 0.6f;
            Height = 0.85f;
            EyeHeight = 0.7f;
            MovementSpeed = 0.3f;
            AttackDamage = 4.0f;
            AttackRange = 1.5f;
            DetectionRange = 16.0f;
            IsNeutral = true;
            CustomName = "狼";
        }

        protected override void DropLoot()
        {
            // 掉落0-2个骨头
        }

        public override void Render()
        {
            // 渲染狼模型
        }
    }

    public class Villager : Mob
    {
        public string Profession { get; set; } = "farmer";
        public int Level { get; set; } = 1;

        public Villager(WorldManager world) : base(world)
        {
            Type = EntityType.Villager;
            MaxHealth = 20;
            Health = 20;
            Width = 0.6f;
            Height = 1.95f;
            EyeHeight = 1.74f;
            MovementSpeed = 0.2f;
            AttackDamage = 0;
            DetectionRange = 16.0f;
            IsPassive = true;
            CustomName = "村民";
        }

        protected override void DropLoot()
        {
            // 村民不掉落物品
        }

        public override void Render()
        {
            // 渲染村民模型
        }
    }
}
