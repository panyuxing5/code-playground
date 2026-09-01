using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;

namespace VoxelCraft.Entities
{
    public class BossAI
    {
        private readonly WorldManager world;
        private readonly EntityManager entityManager;

        // Boss参数
        public float DetectionRange { get; set; } = 64.0f;
        public float AttackRange { get; set; } = 10.0f;
        public float MovementSpeed { get; set; } = 0.2f;
        public float AttackCooldown { get; set; } = 60; // ticks
        public float SpecialAttackCooldown { get; set; } = 300; // ticks

        // 统计
        public int ActiveBosses { get; private set; }
        public int BossesDefeated { get; private set; }
        public int TotalDamageDealt { get; private set; }

        public BossAI(WorldManager world, EntityManager entityManager)
        {
            this.world = world;
            this.entityManager = entityManager;
        }

        public void Initialize()
        {
            Console.WriteLine("[BossAI] Boss AI系统初始化完成");
        }

        public void Update(Boss boss, Vector3 playerPosition)
        {
            if (boss.IsDead) return;

            // 更新Boss状态
            UpdateBossState(boss, playerPosition);

            // 根据状态执行行为
            switch (boss.CurrentState)
            {
                case BossState.Idle:
                    UpdateIdle(boss);
                    break;

                case BossState.Chasing:
                    UpdateChasing(boss, playerPosition);
                    break;

                case BossState.Attacking:
                    UpdateAttacking(boss, playerPosition);
                    break;

                case BossState.SpecialAttacking:
                    UpdateSpecialAttacking(boss, playerPosition);
                    break;

                case BossState.Enraged:
                    UpdateEnraged(boss, playerPosition);
                    break;

                case BossState.Stunned:
                    UpdateStunned(boss);
                    break;

                case BossState.Dying:
                    UpdateDying(boss);
                    break;
            }

            // 更新冷却
            if (boss.AttackCooldownTimer > 0)
            {
                boss.AttackCooldownTimer--;
            }
            if (boss.SpecialAttackCooldownTimer > 0)
            {
                boss.SpecialAttackCooldownTimer--;
            }
            if (boss.StunTimer > 0)
            {
                boss.StunTimer--;
                if (boss.StunTimer == 0)
                {
                    boss.CurrentState = BossState.Chasing;
                }
            }

            ActiveBosses = entityManager.GetEntities().FindAll(e => e is Boss && !e.IsDead).Count;
        }

        private void UpdateBossState(Boss boss, Vector3 playerPosition)
        {
            float distance = Vector3.Distance(boss.Position, playerPosition);

            // 检查是否死亡
            if (boss.Health <= 0)
            {
                boss.CurrentState = BossState.Dying;
                return;
            }

            // 检查是否眩晕
            if (boss.StunTimer > 0)
            {
                boss.CurrentState = BossState.Stunned;
                return;
            }

            // 检查是否狂暴（血量低于30%）
            if (boss.Health < boss.MaxHealth * 0.3f && !boss.IsEnraged)
            {
                boss.IsEnraged = true;
                boss.CurrentState = BossState.Enraged;
                world.CreateParticleEffect(boss.Position, ParticleType.AngryVillager, 20);
                return;
            }

            // 状态转换
            switch (boss.CurrentState)
            {
                case BossState.Idle:
                    if (distance < DetectionRange)
                    {
                        boss.CurrentState = BossState.Chasing;
                        boss.HasTarget = true;
                    }
                    break;

                case BossState.Chasing:
                    if (distance < AttackRange && boss.AttackCooldownTimer <= 0)
                    {
                        boss.CurrentState = BossState.Attacking;
                    }
                    else if (boss.SpecialAttackCooldownTimer <= 0 && distance < DetectionRange * 0.7f)
                    {
                        boss.CurrentState = BossState.SpecialAttacking;
                    }
                    break;

                case BossState.Attacking:
                    if (boss.AttackCooldownTimer <= 0)
                    {
                        if (distance < AttackRange)
                        {
                            PerformAttack(boss, playerPosition);
                        }
                        else
                        {
                            boss.CurrentState = BossState.Chasing;
                        }
                    }
                    break;

                case BossState.SpecialAttacking:
                    if (boss.SpecialAttackCooldownTimer <= 0)
                    {
                        PerformSpecialAttack(boss, playerPosition);
                    }
                    else
                    {
                        boss.CurrentState = BossState.Chasing;
                    }
                    break;

                case BossState.Enraged:
                    // 狂暴状态下持续追击和攻击
                    if (distance < AttackRange && boss.AttackCooldownTimer <= 0)
                    {
                        PerformAttack(boss, playerPosition);
                    }
                    else if (boss.SpecialAttackCooldownTimer <= 0)
                    {
                        PerformSpecialAttack(boss, playerPosition);
                    }
                    else
                    {
                        MoveTowards(boss, playerPosition, MovementSpeed * 1.5f);
                    }
                    break;
            }
        }

        private void UpdateIdle(Boss boss)
        {
            // 随机环顾
            if (Random.Shared.NextDouble() < 0.02)
            {
                boss.Yaw = Random.Shared.Next(360);
            }

            // 缓慢恢复生命值
            if (boss.Health < boss.MaxHealth)
            {
                boss.Health += 0.01f;
            }
        }

        private void UpdateChasing(Boss boss, Vector3 playerPosition)
        {
            MoveTowards(boss, playerPosition, MovementSpeed);

            // 面向玩家
            boss.Yaw = CalculateYaw(boss.Position, playerPosition);
        }

        private void UpdateAttacking(Boss boss, Vector3 playerPosition)
        {
            // 面向玩家
            boss.Yaw = CalculateYaw(boss.Position, playerPosition);

            // 攻击动画
            boss.AttackAnimationTimer++;
        }

        private void UpdateSpecialAttacking(Boss boss, Vector3 playerPosition)
        {
            // 特殊攻击蓄力
            boss.SpecialAttackChargeTimer++;

            if (boss.SpecialAttackChargeTimer >= 40)
            {
                // 释放特殊攻击
                ReleaseSpecialAttack(boss, playerPosition);
                boss.SpecialAttackChargeTimer = 0;
            }
        }

        private void UpdateEnraged(Boss boss, Vector3 playerPosition)
        {
            // 狂暴状态下的行为在UpdateBossState中处理
            boss.Yaw = CalculateYaw(boss.Position, playerPosition);
        }

        private void UpdateStunned(Boss boss)
        {
            // 眩晕状态，无法行动
            boss.Velocity = Vector3.Zero;
        }

        private void UpdateDying(Boss boss)
        {
            boss.DeathTimer++;

            // 死亡动画
            if (boss.DeathTimer < 60)
            {
                // 闪烁效果
                if (boss.DeathTimer % 10 < 5)
                {
                    boss.IsVisible = false;
                }
                else
                {
                    boss.IsVisible = true;
                }
            }
            else
            {
                // 死亡完成
                boss.IsDead = true;
                boss.IsVisible = false;
                BossesDefeated++;

                // 掉落物品
                DropBossLoot(boss);

                // 生成经验球
                world.CreateExperienceOrbs(boss.Position, 500);

                // 爆炸效果
                world.CreateExplosion(boss.Position, 2);
            }
        }

        private void PerformAttack(Boss boss, Vector3 playerPosition)
        {
            boss.AttackCooldownTimer = AttackCooldown;
            boss.CurrentState = BossState.Chasing;

            // 造成伤害
            float damage = boss.AttackDamage;
            if (boss.IsEnraged)
            {
                damage *= 1.5f;
            }

            // 检查玩家是否在攻击范围内
            float distance = Vector3.Distance(boss.Position, playerPosition);
            if (distance < AttackRange)
            {
                // 玩家受伤（实际游戏中通过事件传递）
                TotalDamageDealt += (int)damage;
            }

            // 攻击粒子效果
            world.CreateParticleEffect(boss.Position + boss.GetLookVector() * 2, ParticleType.Crit, 5);
        }

        private void PerformSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            boss.SpecialAttackCooldownTimer = SpecialAttackCooldown;
            boss.CurrentState = BossState.Chasing;

            // 根据Boss类型执行不同的特殊攻击
            switch (boss.BossType)
            {
                case BossType.EnderDragon:
                    EnderDragonSpecialAttack(boss, playerPosition);
                    break;

                case BossType.Wither:
                    WitherSpecialAttack(boss, playerPosition);
                    break;

                case BossType.Warden:
                    WardenSpecialAttack(boss, playerPosition);
                    break;

                case BossType.ElderGuardian:
                    ElderGuardianSpecialAttack(boss, playerPosition);
                    break;

                default:
                    GenericSpecialAttack(boss, playerPosition);
                    break;
            }
        }

        private void ReleaseSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 特殊攻击释放
            PerformSpecialAttack(boss, playerPosition);
        }

        private void EnderDragonSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 末影龙：龙息攻击
            world.CreateParticleEffect(boss.Position + boss.GetLookVector() * 5, ParticleType.DragonBreath, 30);

            // 对玩家造成持续伤害
            float distance = Vector3.Distance(boss.Position, playerPosition);
            if (distance < 15)
            {
                TotalDamageDealt += 10;
            }
        }

        private void WitherSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 凋灵：发射凋灵骷髅头颅
            Vector3 direction = (playerPosition - boss.Position).Normalized();

            // 发射3个头颅
            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = new Vector3(
                    (float)(Random.Shared.NextDouble() - 0.5) * 2,
                    (float)(Random.Shared.NextDouble() - 0.5) * 2,
                    (float)(Random.Shared.NextDouble() - 0.5) * 2
                );

                world.SpawnProjectile(ProjectileType.WitherSkull,
                    boss.Position + Vector3.UnitY * 3,
                    direction + offset * 0.1f,
                    boss.Id,
                    8);
            }
        }

        private void WardenSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 监守者：音波攻击
            world.CreateParticleEffect(boss.Position, ParticleType.Shriek, 20);

            // 造成穿透伤害
            float distance = Vector3.Distance(boss.Position, playerPosition);
            if (distance < 20)
            {
                TotalDamageDealt += 15;
            }

            // 击退
            Vector3 knockback = (playerPosition - boss.Position).Normalized() * 5;
        }

        private void ElderGuardianSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 远古守卫者：挖掘疲劳诅咒
            world.CreateParticleEffect(playerPosition, ParticleType.ElderGuardian, 10);

            // 施加挖掘疲劳效果
        }

        private void GenericSpecialAttack(Boss boss, Vector3 playerPosition)
        {
            // 通用特殊攻击：范围攻击
            world.CreateExplosion(boss.Position, 3);

            float distance = Vector3.Distance(boss.Position, playerPosition);
            if (distance < 10)
            {
                TotalDamageDealt += (int)(20 * (1 - distance / 10));
            }
        }

        private void MoveTowards(Boss boss, Vector3 target, float speed)
        {
            Vector3 direction = (target - boss.Position).Normalized();
            direction.Y = 0;

            boss.Velocity.X = direction.X * speed;
            boss.Velocity.Z = direction.Z * speed;

            // 检查是否需要跳
            if (IsBlockAhead(boss))
            {
                if (boss.OnGround)
                {
                    boss.Velocity.Y = 0.5f;
                }
            }
        }

        private bool IsBlockAhead(Boss boss)
        {
            Vector3 ahead = boss.Position + boss.GetLookVector() * 1.0f;
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

        private void DropBossLoot(Boss boss)
        {
            // 根据Boss类型掉落不同物品
            switch (boss.BossType)
            {
                case BossType.EnderDragon:
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_DRAGON_EGG, 1);
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_ENDER_EYE, 12);
                    break;

                case BossType.Wither:
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_NETHER_STAR, 1);
                    break;

                case BossType.Warden:
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_ECHO_SHARD, 5);
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_SCULK_CATALYST, 1);
                    break;

                case BossType.ElderGuardian:
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_SPONGE, 3);
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_PRISMARINE_SHARD, 10);
                    break;

                default:
                    world.CreateItemEntity(boss.Position, GameConstants.ITEM_EXPERIENCE_BOTTLE, 5);
                    break;
            }
        }

        public void StunBoss(Boss boss, int duration)
        {
            boss.StunTimer = duration;
            boss.CurrentState = BossState.Stunned;
        }

        public void EnrageBoss(Boss boss)
        {
            boss.IsEnraged = true;
            boss.CurrentState = BossState.Enraged;
        }

        public void ResetStats()
        {
            BossesDefeated = 0;
            TotalDamageDealt = 0;
        }
    }

    public class Boss : Mob
    {
        public BossType BossType;
        public BossState CurrentState;
        public float AttackDamage;
        public int AttackCooldownTimer;
        public int SpecialAttackCooldownTimer;
        public int SpecialAttackChargeTimer;
        public int AttackAnimationTimer;
        public int StunTimer;
        public int DeathTimer;
        public bool IsEnraged;
        public bool HasTarget;
        public bool IsVisible = true;
        public Vector3 TargetPosition;

        public Boss()
        {
            CurrentState = BossState.Idle;
            IsBoss = true;
        }
    }

    public enum BossType
    {
        EnderDragon,
        Wither,
        Warden,
        ElderGuardian,
        Ravager,
        Custom
    }

    public enum BossState
    {
        Idle,
        Chasing,
        Attacking,
        SpecialAttacking,
        Enraged,
        Stunned,
        Dying
    }
}
