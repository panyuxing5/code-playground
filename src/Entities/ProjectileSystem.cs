using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;

namespace VoxelCraft.Entities
{
    public class ProjectileSystem
    {
        private readonly WorldManager world;
        private readonly EntityManager entityManager;
        private readonly List<ProjectileInstance> projectiles;

        // 鐗╃悊鍙傛暟
        public float Gravity { get; set; } = 0.05f;
        public float AirResistance { get; set; } = 0.99f;
        public float WaterResistance { get; set; } = 0.8f;
        public float MaxProjectileAge { get; set; } = 600; // ticks

        // 缁熻
        public int ActiveProjectiles => projectiles.Count;
        public int ProjectilesFired { get; private set; }
        public int ProjectilesHit { get; private set; }

        public ProjectileSystem(WorldManager world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
            projectiles = new List<ProjectileInstance>();
        }

        public void Initialize()
        {
            Console.WriteLine("[ProjectileSystem] 鎶涘皠鐗╃郴缁熷垵濮嬪寲瀹屾垚");
        }

        public void Update()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                ProjectileInstance projectile = projectiles[i];
                UpdateProjectile(projectile);

                if (projectile.IsDead || projectile.Age > MaxProjectileAge)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        private void UpdateProjectile(ProjectileInstance projectile)
        {
            projectile.Age++;

            // 搴旂敤閲嶅姏
            projectile.Velocity.Y -= Gravity;

            // 搴旂敤绌烘皵闃诲姏
            projectile.Velocity *= AirResistance;

            // 妫€鏌ユ槸鍚﹀湪姘翠腑
            ushort block = world.GetBlock(
                (int)projectile.Position.X,
                (int)projectile.Position.Y,
                (int)projectile.Position.Z);

            if (block == GameConstants.BLOCK_WATER_STILL || block == GameConstants.BLOCK_WATER_FLOWING)
            {
                projectile.Velocity *= WaterResistance;
                projectile.InWater = true;
            }
            else
            {
                projectile.InWater = false;
            }

            // 绉诲姩
            Vector3 oldPosition = projectile.Position;
            projectile.Position += projectile.Velocity;

            // 纰版挒妫€娴?            if (CheckBlockCollision(projectile))
            {
                OnProjectileHitBlock(projectile, oldPosition);
                return;
            }

            if (CheckEntityCollision(projectile))
            {
                OnProjectileHitEntity(projectile);
                return;
            }

            // 鏇存柊鏃嬭浆
            if (projectile.Velocity.Length > 0.01f)
            {
                projectile.Yaw = (float)Math.Atan2(projectile.Velocity.X, projectile.Velocity.Z) * 180 / MathF.PI;
                projectile.Pitch = (float)Math.Atan2(projectile.Velocity.Y, projectile.Velocity.Xz.Length) * 180 / MathF.PI;
            }
        }

        private bool CheckBlockCollision(ProjectileInstance projectile)
        {
            int x = (int)Math.Floor(projectile.Position.X);
            int y = (int)Math.Floor(projectile.Position.Y);
            int z = (int)Math.Floor(projectile.Position.Z);

            ushort block = world.GetBlock(x, y, z);
            return block != GameConstants.BLOCK_AIR &&
                   block != GameConstants.BLOCK_WATER_STILL &&
                   block != GameConstants.BLOCK_WATER_FLOWING &&
                   block != GameConstants.BLOCK_LAVA_STILL &&
                   block != GameConstants.BLOCK_LAVA_FLOWING;
        }

        private bool CheckEntityCollision(ProjectileInstance projectile)
        {
            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity.Id == projectile.OwnerId) continue;
                if (entity.IsDead) continue;

                float distance = Vector3.Distance(projectile.Position, entity.Position);
                if (distance < 1.0f)
                {
                    projectile.HitEntity = entity;
                    return true;
                }
            }
            return false;
        }

        private void OnProjectileHitBlock(ProjectileInstance projectile, Vector3 oldPosition)
        {
            projectile.IsDead = true;
            ProjectilesHit++;

            // 根据抛射物类型处理
            switch (projectile.Type)
            {
                case ProjectileType.Arrow:
                    // 绠彃鍦ㄦ柟鍧椾笂
                    world.CreateItemEntity(projectile.Position, 520, 1);
                    break;

                case ProjectileType.Snowball:
                    // 闆悆鐮寸
                    world.CreateParticleEffect(projectile.Position, ParticleType.Snowball, 8);
                    break;

                case ProjectileType.Egg:
                    // 楦¤泲鐮寸锛屾湁鍑犵巼鐢熸垚灏忛浮
                    world.CreateParticleEffect(projectile.Position, ParticleType.Egg, 8);
                    if (Random.Shared.NextDouble() < 0.125)
                    {
                        entityManager.SpawnEntity(EntityType.Chicken, projectile.Position);
                    }
                    break;

                case ProjectileType.EnderPearl:
                    // 鏈奖鐝嶇彔浼犻€?                    // 浼犻€佺帺瀹跺埌鍛戒腑浣嶇疆
                    break;

                case ProjectileType.Fireball:
                    // 鐏悆鐖嗙偢
                    world.CreateExplosion(projectile.Position, 3);
                    break;

                case ProjectileType.SmallFireball:
                    // 小火球点燃方块
                    world.SetBlock(
                        (int)projectile.Position.X,
                        (int)projectile.Position.Y,
                        (int)projectile.Position.Z,
                        GameConstants.BLOCK_FIRE);
                    break;

                case ProjectileType.WitherSkull:
                    // 鍑嬬伒楠烽珔澶撮鐖嗙偢
                    world.CreateExplosion(projectile.Position, 2);
                    break;

                case ProjectileType.ShulkerBullet:
                    // 娼滃奖璐濆寮?                    world.CreateParticleEffect(projectile.Position, ParticleType.ShulkerBullet, 6);
                    break;

                case ProjectileType.DragonFireball:
                    // 榫欐伅
                    world.CreateParticleEffect(projectile.Position, ParticleType.DragonBreath, 20);
                    break;

                case ProjectileType.Trident:
                    // 涓夊弶鎴熸帀钀?                    world.CreateItemEntity(projectile.Position, 524, 1);
                    break;

                case ProjectileType.ExperienceBottle:
                    // 闄勯瓟涔嬬摱鐢熸垚缁忛獙鐞?                    world.CreateExperienceOrbs(projectile.Position, Random.Shared.Next(3, 12));
                    break;

                case ProjectileType.Potion:
                    // 鑽按鏁堟灉
                    world.CreateParticleEffect(projectile.Position, ParticleType.Potion, 15);
                    break;

                case ProjectileType.FishingHook:
                    // 钓鱼钩
                    break;
        }
        }

        private void OnProjectileHitEntity(ProjectileInstance projectile)
        {
            projectile.IsDead = true;
            ProjectilesHit++;

            if (projectile.HitEntity == null) return;

            // 鏍规嵁鎶涘皠鐗╃被鍨嬮€犳垚浼ゅ
            switch (projectile.Type)
            {
                case ProjectileType.Arrow:
                    projectile.HitEntity.TakeDamage(projectile.Damage);
                    world.CreateItemEntity(projectile.Position, 520, 1);
                    break;

                case ProjectileType.Snowball:
                    projectile.HitEntity.TakeDamage(0);
                    world.CreateParticleEffect(projectile.Position, ParticleType.Snowball, 8);
                    break;

                case ProjectileType.Egg:
                    projectile.HitEntity.TakeDamage(0);
                    world.CreateParticleEffect(projectile.Position, ParticleType.Egg, 8);
                    break;

                case ProjectileType.EnderPearl:
                    projectile.HitEntity.TakeDamage(5);
                    break;

                case ProjectileType.Fireball:
                    projectile.HitEntity.TakeDamage(projectile.Damage);
                    world.CreateExplosion(projectile.Position, 3);
                    break;

                case ProjectileType.SmallFireball:
                    projectile.HitEntity.TakeDamage(projectile.Damage);
                    projectile.HitEntity.SetOnFire(100);
                    break;

                case ProjectileType.WitherSkull:
                    projectile.HitEntity.TakeDamage(projectile.Damage);
                    projectile.HitEntity.AddEffect(StatusEffect.Wither, 200, 1);
                    world.CreateExplosion(projectile.Position, 2);
                    break;

                case ProjectileType.ShulkerBullet:
                    projectile.HitEntity.TakeDamage(4);
                    projectile.HitEntity.AddEffect(StatusEffect.Levitation, 200, 1);
                    break;

                case ProjectileType.Trident:
                    projectile.HitEntity.TakeDamage(projectile.Damage);
                    world.CreateItemEntity(projectile.Position, 524, 1);
                    break;

                case ProjectileType.Potion:
                    // 搴旂敤鑽按鏁堟灉
                    world.CreateParticleEffect(projectile.Position, ParticleType.Potion, 15);
                    break;
            }
        }

        public void FireProjectile(ProjectileType type, Vector3 position, Vector3 velocity, int ownerId, float damage = 2)
        {
            ProjectileInstance projectile = new ProjectileInstance
            {
                Type = type,
                Position = position,
                Velocity = velocity,
                OwnerId = ownerId,
                Damage = damage,
                Age = 0,
                IsDead = false
            };

            projectiles.Add(projectile);
            ProjectilesFired++;
        }

        public void FireArrow(Vector3 position, Vector3 direction, float power, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * power * 3.0f;
            FireProjectile(ProjectileType.Arrow, position, velocity, ownerId, 2 + power * 2);
        }

        public void FireSnowball(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 1.5f;
            FireProjectile(ProjectileType.Snowball, position, velocity, ownerId, 0);
        }

        public void FireEgg(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 1.5f;
            FireProjectile(ProjectileType.Egg, position, velocity, ownerId, 0);
        }

        public void FireEnderPearl(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 1.5f;
            FireProjectile(ProjectileType.EnderPearl, position, velocity, ownerId, 5);
        }

        public void FireFireball(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 0.5f;
            FireProjectile(ProjectileType.Fireball, position, velocity, ownerId, 6);
        }

        public void FireSmallFireball(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 0.5f;
            FireProjectile(ProjectileType.SmallFireball, position, velocity, ownerId, 5);
        }

        public void FireWitherSkull(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 0.5f;
            FireProjectile(ProjectileType.WitherSkull, position, velocity, ownerId, 8);
        }

        public void FireShulkerBullet(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 0.5f;
            FireProjectile(ProjectileType.ShulkerBullet, position, velocity, ownerId, 4);
        }

        public void FireDragonFireball(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 0.5f;
            FireProjectile(ProjectileType.DragonFireball, position, velocity, ownerId, 6);
        }

        public void FireTrident(Vector3 position, Vector3 direction, float power, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * power * 2.5f;
            FireProjectile(ProjectileType.Trident, position, velocity, ownerId, 8 + power * 2);
        }

        public void FireExperienceBottle(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 1.0f;
            FireProjectile(ProjectileType.ExperienceBottle, position, velocity, ownerId, 0);
        }

        public void FirePotion(Vector3 position, Vector3 direction, int ownerId)
        {
            Vector3 velocity = direction.Normalized() * 1.0f;
            FireProjectile(ProjectileType.Potion, position, velocity, ownerId, 0);
        }

        public List<ProjectileInstance> GetProjectiles()
        {
            return new List<ProjectileInstance>(projectiles);
        }

        public void Clear()
        {
            projectiles.Clear();
        }
    }

    public class ProjectileInstance
    {
        public ProjectileType Type;
        public Vector3 Position;
        public Vector3 Velocity;
        public int OwnerId;
        public float Damage;
        public int Age;
        public bool IsDead;
        public bool InWater;
        public float Yaw;
        public float Pitch;
        public Entity HitEntity;
    }

    public enum ProjectileType
    {
        Arrow,
        Snowball,
        Egg,
        EnderPearl,
        Fireball,
        SmallFireball,
        WitherSkull,
        ShulkerBullet,
        DragonFireball,
        Trident,
        ExperienceBottle,
        Potion,
        FishingHook
    }
}
