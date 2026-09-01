using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;

namespace VoxelCraft.World
{
    public class ExplosionSystem
    {
        private readonly WorldManager world;
        private readonly EntityManager entityManager;
        private readonly ParticleSystem particleSystem;

        // 爆炸参数
        public float ExplosionPower { get; set; } = 4.0f;
        public int ExplosionResolution { get; set; } = 16;
        public float DamageMultiplier { get; set; } = 7.0f;
        public float KnockbackMultiplier { get; set; } = 1.0f;
        public bool CreateFire { get; set; } = false;
        public bool BreakBlocks { get; set; } = true;
        public bool DamageEntities { get; set; } = true;
        public bool DropItems { get; set; } = true;

        // 统计
        public int ExplosionsTriggered { get; private set; }
        public int BlocksDestroyed { get; private set; }
        public int EntitiesDamaged { get; private set; }

        public ExplosionSystem(WorldManager world, EntityManager entityManager, ParticleSystem particleSystem)
        {
            this.world = world;
            this.entityManager = entityManager;
            this.particleSystem = particleSystem;
        }

        public void Initialize()
        {
            Console.WriteLine("[ExplosionSystem] 爆炸系统初始化完成");
        }

        public void CreateExplosion(Vector3 position, float power, bool createFire = false, bool breakBlocks = true)
        {
            ExplosionsTriggered++;

            // 1. 破坏方块
            if (breakBlocks)
            {
                DestroyBlocksInRadius(position, power);
            }

            // 2. 伤害实体
            if (DamageEntities)
            {
                DamageEntitiesInRadius(position, power);
            }

            // 3. 生成粒子效果
            CreateExplosionParticles(position, power);

            // 4. 生成火焰
            if (createFire)
            {
                CreateFireInRadius(position, power);
            }

            // 5. 播放音效
            // soundManager.PlaySound("explosion", position);
        }

        private void DestroyBlocksInRadius(Vector3 center, float power)
        {
            int radius = (int)Math.Ceiling(power);
            int minX = (int)Math.Floor(center.X - radius);
            int maxX = (int)Math.Ceiling(center.X + radius);
            int minY = (int)Math.Floor(center.Y - radius);
            int maxY = (int)Math.Ceiling(center.Y + radius);
            int minZ = (int)Math.Floor(center.Z - radius);
            int maxZ = (int)Math.Ceiling(center.Z + radius);

            List<Vector3i> blocksToDestroy = new List<Vector3i>();

            // 使用射线采样确定哪些方块被破坏
            for (int i = 0; i < ExplosionResolution; i++)
            {
                for (int j = 0; j < ExplosionResolution; j++)
                {
                    for (int k = 0; k < ExplosionResolution; k++)
                    {
                        if (i == 0 || i == ExplosionResolution - 1 ||
                            j == 0 || j == ExplosionResolution - 1 ||
                            k == 0 || k == ExplosionResolution - 1)
                        {
                            // 从中心向各个方向发射射线
                            Vector3 direction = new Vector3(
                                (float)i / (ExplosionResolution - 1) * 2 - 1,
                                (float)j / (ExplosionResolution - 1) * 2 - 1,
                                (float)k / (ExplosionResolution - 1) * 2 - 1
                            ).Normalized();

                            float currentPower = power * (0.7f + (float)Random.Shared.NextDouble() * 0.6f);

                            for (float distance = 0.3f; distance < power && currentPower > 0; distance += 0.3f)
                            {
                                Vector3 pos = center + direction * distance;
                                int bx = (int)Math.Floor(pos.X);
                                int by = (int)Math.Floor(pos.Y);
                                int bz = (int)Math.Floor(pos.Z);

                                if (by < 0 || by >= GameConstants.WORLD_HEIGHT) continue;

                                ushort block = world.GetBlock(bx, by, bz);
                                if (block != GameConstants.BLOCK_AIR)
                                {
                                    // 检查方块爆炸抗性
                                    float resistance = GetBlockExplosionResistance(block);
                                    currentPower -= (resistance + 0.3f) * 0.3f;

                                    if (currentPower > 0)
                                    {
                                        Vector3i blockPos = new Vector3i(bx, by, bz);
                                        if (!blocksToDestroy.Contains(blockPos))
                                        {
                                            blocksToDestroy.Add(blockPos);
                                        }
                                    }
                                }

                                // 水和熔岩会减弱爆炸
                                if (block == GameConstants.BLOCK_WATER_STILL ||
                                    block == GameConstants.BLOCK_WATER_FLOWING ||
                                    block == GameConstants.BLOCK_LAVA_STILL ||
                                    block == GameConstants.BLOCK_LAVA_FLOWING)
                                {
                                    currentPower -= 0.5f;
                                }
                            }
                        }
                    }
                }
            }

            // 实际破坏方块
            foreach (Vector3i blockPos in blocksToDestroy)
            {
                ushort block = world.GetBlock(blockPos.X, blockPos.Y, blockPos.Z);
                if (block != GameConstants.BLOCK_AIR)
                {
                    // 掉落物品
                    if (DropItems && ShouldDropBlock(block))
                    {
                        world.CreateItemEntity(
                            new Vector3(blockPos.X + 0.5f, blockPos.Y + 0.5f, blockPos.Z + 0.5f),
                            block, 1);
                    }

                    world.SetBlock(blockPos.X, blockPos.Y, blockPos.Z, GameConstants.BLOCK_AIR);
                    BlocksDestroyed++;
                }
            }
        }

        private void DamageEntitiesInRadius(Vector3 center, float power)
        {
            float damageRadius = power * 2.0f;

            foreach (Entity entity in entityManager.GetEntities())
            {
                if (entity.IsDead) continue;

                float distance = Vector3.Distance(center, entity.Position);
                if (distance < damageRadius)
                {
                    // 计算伤害（距离越近伤害越高）
                    float exposure = CalculateEntityExposure(center, entity);
                    float impact = (1.0f - distance / damageRadius) * exposure;
                    float damage = (impact * impact + impact) * 0.5f * 7.0f * power + 1.0f;

                    entity.TakeDamage((int)damage);
                    EntitiesDamaged++;

                    // 击退效果
                    Vector3 knockbackDirection = (entity.Position - center).Normalized();
                    entity.Velocity += knockbackDirection * impact * KnockbackMultiplier * 2.0f;

                    // 点燃实体
                    if (CreateFire && Random.Shared.NextDouble() < 0.3)
                    {
                        entity.SetOnFire(100);
                    }
                }
            }
        }

        private float CalculateEntityExposure(Vector3 explosionPos, Entity entity)
        {
            // 简化的暴露度计算
            // 实际Minecraft中会检查射线是否被方块阻挡
            int samples = 10;
            int exposed = 0;

            for (int i = 0; i < samples; i++)
            {
                Vector3 samplePoint = entity.Position + new Vector3(
                    (float)(Random.Shared.NextDouble() - 0.5) * entity.Width,
                    (float)(Random.Shared.NextDouble() * entity.Height),
                    (float)(Random.Shared.NextDouble() - 0.5) * entity.Width
                );

                Vector3 direction = (samplePoint - explosionPos).Normalized();
                float distance = Vector3.Distance(explosionPos, samplePoint);
                bool blocked = false;

                for (float d = 0; d < distance; d += 0.5f)
                {
                    Vector3 pos = explosionPos + direction * d;
                    ushort block = world.GetBlock((int)pos.X, (int)pos.Y, (int)pos.Z);
                    if (block != GameConstants.BLOCK_AIR &&
                        block != GameConstants.BLOCK_WATER_STILL &&
                        block != GameConstants.BLOCK_WATER_FLOWING)
                    {
                        blocked = true;
                        break;
                    }
                }

                if (!blocked)
                {
                    exposed++;
                }
            }

            return (float)exposed / samples;
        }

        private void CreateExplosionParticles(Vector3 position, float power)
        {
            // 爆炸粒子
            particleSystem.SpawnParticles(position, ParticleType.Explosion, (int)(power * 10), power);

            // 烟雾粒子
            particleSystem.SpawnParticles(position, ParticleType.Smoke, (int)(power * 5), power * 0.5f);

            // 火花粒子
            particleSystem.SpawnParticles(position, ParticleType.Spark, (int)(power * 8), power * 0.3f);
        }

        private void CreateFireInRadius(Vector3 center, float power)
        {
            int radius = (int)Math.Ceiling(power);

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    for (int dz = -radius; dz <= radius; dz++)
                    {
                        float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
                        if (distance > power) continue;

                        int x = (int)center.X + dx;
                        int y = (int)center.Y + dy;
                        int z = (int)center.Z + dz;

                        if (y < 0 || y >= GameConstants.WORLD_HEIGHT) continue;

                        // 在空气方块下方有固体方块时生成火
                        if (world.GetBlock(x, y, z) == GameConstants.BLOCK_AIR &&
                            world.GetBlock(x, y - 1, z) != GameConstants.BLOCK_AIR &&
                            Random.Shared.NextDouble() < 0.1)
                        {
                            world.SetBlock(x, y, z, GameConstants.BLOCK_FIRE);
                        }
                    }
                }
            }
        }

        private float GetBlockExplosionResistance(ushort blockId)
        {
            switch (blockId)
            {
                // 高抗性方块
                case GameConstants.BLOCK_OBSIDIAN:
                case GameConstants.BLOCK_CRYING_OBSIDIAN:
                    return 1200.0f;

                case GameConstants.BLOCK_BEDROCK:
                case GameConstants.BLOCK_REINFORCED_DEEPSLATE:
                    return 3600000.0f;

                case GameConstants.BLOCK_END_STONE:
                    return 9.0f;

                case GameConstants.BLOCK_ANCIENT_DEBRY:
                    return 1200.0f;

                // 中等抗性
                case GameConstants.BLOCK_STONE:
                case GameConstants.BLOCK_COBBLESTONE:
                case GameConstants.BLOCK_STONE_BRICKS:
                    return 6.0f;

                case GameConstants.BLOCK_COAL_ORE:
                case GameConstants.BLOCK_IRON_ORE:
                case GameConstants.BLOCK_GOLD_ORE:
                case GameConstants.BLOCK_DIAMOND_ORE:
                case GameConstants.BLOCK_REDSTONE_ORE:
                case GameConstants.BLOCK_LAPIS_ORE:
                case GameConstants.BLOCK_EMERALD_ORE:
                    return 3.0f;

                case GameConstants.BLOCK_DIRT:
                case GameConstants.BLOCK_GRASS_BLOCK:
                    return 0.5f;

                case GameConstants.BLOCK_SAND:
                case GameConstants.BLOCK_GRAVEL:
                    return 0.5f;

                case GameConstants.BLOCK_WOOD:
                case GameConstants.BLOCK_PLANKS:
                    return 2.0f;

                case GameConstants.BLOCK_GLASS:
                    return 0.3f;

                case GameConstants.BLOCK_TNT:
                    return 0.0f;

                // 默认
                default:
                    return 1.0f;
            }
        }

        private bool ShouldDropBlock(ushort blockId)
        {
            // 某些方块不掉落
            switch (blockId)
            {
                case GameConstants.BLOCK_FIRE:
                case GameConstants.BLOCK_AIR:
                case GameConstants.BLOCK_WATER_STILL:
                case GameConstants.BLOCK_WATER_FLOWING:
                case GameConstants.BLOCK_LAVA_STILL:
                case GameConstants.BLOCK_LAVA_FLOWING:
                case GameConstants.BLOCK_BEDROCK:
                    return false;

                default:
                    return true;
            }
        }

        public void CreateTNTExplosion(Vector3 position)
        {
            CreateExplosion(position, 4.0f, false, true);
        }

        public void CreateCreeperExplosion(Vector3 position, bool powered = false)
        {
            float power = powered ? 6.0f : 3.0f;
            CreateExplosion(position, power, false, true);
        }

        public void CreateGhastFireballExplosion(Vector3 position)
        {
            CreateExplosion(position, 1.0f, true, true);
        }

        public void CreateWitherExplosion(Vector3 position)
        {
            CreateExplosion(position, 1.0f, false, true);
        }

        public void CreateEndCrystalExplosion(Vector3 position)
        {
            CreateExplosion(position, 6.0f, false, true);
        }

        public void CreateBedExplosion(Vector3 position)
        {
            CreateExplosion(position, 5.0f, true, true);
        }

        public void CreateRespawnAnchorExplosion(Vector3 position)
        {
            CreateExplosion(position, 5.0f, true, true);
        }

        public void ResetStats()
        {
            ExplosionsTriggered = 0;
            BlocksDestroyed = 0;
            EntitiesDamaged = 0;
        }
    }
}
