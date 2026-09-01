using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class ParticleSystem
    {
        private readonly List<Particle> particles;
        private readonly Queue<Particle> particlePool;

        // 粒子限制
        public int MaxParticles { get; set; } = 10000;
        public int ParticleBudget { get; set; } = 1000;

        // 统计
        public int ActiveParticles => particles.Count;
        public int ParticlesSpawned { get; private set; }
        public int ParticlesExpired { get; private set; }

        public ParticleSystem()
        {
            particles = new List<Particle>();
            particlePool = new Queue<Particle>();

            // 预分配粒子池
            for (int i = 0; i < MaxParticles; i++)
            {
                particlePool.Enqueue(new Particle());
            }
        }

        public void Initialize()
        {
            Console.WriteLine("[ParticleSystem] 粒子系统初始化完成，粒子池大小: " + MaxParticles);
        }

        public void Update(float deltaTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];
                UpdateParticle(particle, deltaTime);

                if (particle.IsDead)
                {
                    particles.RemoveAt(i);
                    particlePool.Enqueue(particle);
                    ParticlesExpired++;
                }
            }
        }

        private void UpdateParticle(Particle particle, float deltaTime)
        {
            particle.Age += deltaTime;

            if (particle.Age >= particle.Lifetime)
            {
                particle.IsDead = true;
                return;
            }

            // 应用重力
            particle.Velocity.Y -= particle.Gravity * deltaTime;

            // 应用阻力
            particle.Velocity *= (1.0f - particle.Drag * deltaTime);

            // 移动
            particle.Position += particle.Velocity * deltaTime;

            // 更新大小
            float lifeRatio = particle.Age / particle.Lifetime;
            particle.CurrentSize = particle.StartSize + (particle.EndSize - particle.StartSize) * lifeRatio;

            // 更新颜色
            particle.CurrentColor = Vector3.Lerp(particle.StartColor, particle.EndColor, lifeRatio);

            // 更新透明度
            particle.CurrentAlpha = particle.StartAlpha + (particle.EndAlpha - particle.StartAlpha) * lifeRatio;

            // 旋转
            particle.Rotation += particle.RotationSpeed * deltaTime;
        }

        public void SpawnParticle(Vector3 position, Vector3 velocity, ParticleType type)
        {
            if (particles.Count >= ParticleBudget) return;
            if (particlePool.Count == 0) return;

            Particle particle = particlePool.Dequeue();
            particle.Reset();

            particle.Position = position;
            particle.Velocity = velocity;
            particle.Type = type;

            // 根据类型设置属性
            ApplyParticleTypeProperties(particle);

            particles.Add(particle);
            ParticlesSpawned++;
        }

        public void SpawnParticle(Vector3 position, Vector3 velocity, Vector3 color, float size, float lifetime)
        {
            if (particles.Count >= ParticleBudget) return;
            if (particlePool.Count == 0) return;

            Particle particle = particlePool.Dequeue();
            particle.Reset();

            particle.Position = position;
            particle.Velocity = velocity;
            particle.StartColor = color;
            particle.EndColor = color;
            particle.StartSize = size;
            particle.EndSize = size * 0.5f;
            particle.Lifetime = lifetime;
            particle.Gravity = 0.5f;
            particle.Drag = 0.5f;
            particle.Type = ParticleType.Custom;

            particles.Add(particle);
            ParticlesSpawned++;
        }

        public void SpawnParticles(Vector3 position, ParticleType type, int count, float spread = 1.0f)
        {
            Random random = Random.Shared;

            for (int i = 0; i < count; i++)
            {
                Vector3 offset = new Vector3(
                    (float)(random.NextDouble() - 0.5) * spread,
                    (float)(random.NextDouble() - 0.5) * spread,
                    (float)(random.NextDouble() - 0.5) * spread
                );

                Vector3 velocity = new Vector3(
                    (float)(random.NextDouble() - 0.5) * 0.5f,
                    (float)(random.NextDouble() - 0.5) * 0.5f,
                    (float)(random.NextDouble() - 0.5) * 0.5f
                );

                SpawnParticle(position + offset, velocity, type);
            }
        }

        private void ApplyParticleTypeProperties(Particle particle)
        {
            switch (particle.Type)
            {
                case ParticleType.Smoke:
                    particle.StartColor = new Vector3(0.3f, 0.3f, 0.3f);
                    particle.EndColor = new Vector3(0.1f, 0.1f, 0.1f);
                    particle.StartSize = 0.3f;
                    particle.EndSize = 0.6f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = -0.1f;
                    particle.Drag = 0.3f;
                    particle.StartAlpha = 0.8f;
                    particle.EndAlpha = 0.0f;
                    break;

                case ParticleType.Flame:
                    particle.StartColor = new Vector3(1.0f, 0.8f, 0.2f);
                    particle.EndColor = new Vector3(1.0f, 0.2f, 0.0f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = -0.3f;
                    particle.Drag = 0.2f;
                    particle.StartAlpha = 1.0f;
                    particle.EndAlpha = 0.0f;
                    break;

                case ParticleType.Spark:
                    particle.StartColor = new Vector3(1.0f, 0.9f, 0.5f);
                    particle.EndColor = new Vector3(1.0f, 0.5f, 0.1f);
                    particle.StartSize = 0.05f;
                    particle.EndSize = 0.02f;
                    particle.Lifetime = 0.3f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.Water:
                    particle.StartColor = new Vector3(0.2f, 0.5f, 1.0f);
                    particle.EndColor = new Vector3(0.1f, 0.3f, 0.8f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.5f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.Rain:
                    particle.StartColor = new Vector3(0.3f, 0.5f, 0.8f);
                    particle.EndColor = new Vector3(0.3f, 0.5f, 0.8f);
                    particle.StartSize = 0.05f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = 3.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.Snow:
                    particle.StartColor = new Vector3(0.95f, 0.95f, 1.0f);
                    particle.EndColor = new Vector3(0.9f, 0.9f, 0.95f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.08f;
                    particle.Lifetime = 3.0f;
                    particle.Gravity = 0.3f;
                    particle.Drag = 0.5f;
                    break;

                case ParticleType.Heart:
                    particle.StartColor = new Vector3(1.0f, 0.2f, 0.4f);
                    particle.EndColor = new Vector3(1.0f, 0.4f, 0.6f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.15f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Note:
                    particle.StartColor = new Vector3(0.5f, 1.0f, 0.5f);
                    particle.EndColor = new Vector3(0.3f, 0.8f, 0.3f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.15f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.1f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Enchant:
                    particle.StartColor = new Vector3(0.7f, 0.5f, 1.0f);
                    particle.EndColor = new Vector3(0.5f, 0.3f, 0.8f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Crit:
                    particle.StartColor = new Vector3(0.8f, 0.8f, 1.0f);
                    particle.EndColor = new Vector3(0.6f, 0.6f, 0.8f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.5f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Damage:
                    particle.StartColor = new Vector3(1.0f, 0.2f, 0.2f);
                    particle.EndColor = new Vector3(0.8f, 0.1f, 0.1f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Experience:
                    particle.StartColor = new Vector3(0.5f, 1.0f, 0.2f);
                    particle.EndColor = new Vector3(0.3f, 0.8f, 0.1f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Portal:
                    particle.StartColor = new Vector3(0.8f, 0.3f, 1.0f);
                    particle.EndColor = new Vector3(0.6f, 0.1f, 0.8f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.1f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.BlockBreak:
                    particle.StartColor = new Vector3(0.6f, 0.6f, 0.6f);
                    particle.EndColor = new Vector3(0.4f, 0.4f, 0.4f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.8f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.BlockPlace:
                    particle.StartColor = new Vector3(0.7f, 0.7f, 0.7f);
                    particle.EndColor = new Vector3(0.5f, 0.5f, 0.5f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.5f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Snowball:
                    particle.StartColor = new Vector3(0.9f, 0.95f, 1.0f);
                    particle.EndColor = new Vector3(0.8f, 0.85f, 0.9f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.5f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Egg:
                    particle.StartColor = new Vector3(0.9f, 0.85f, 0.7f);
                    particle.EndColor = new Vector3(0.8f, 0.75f, 0.6f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.5f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Potion:
                    particle.StartColor = new Vector3(0.5f, 0.3f, 0.8f);
                    particle.EndColor = new Vector3(0.3f, 0.1f, 0.6f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.DragonBreath:
                    particle.StartColor = new Vector3(0.8f, 0.3f, 1.0f);
                    particle.EndColor = new Vector3(0.6f, 0.1f, 0.8f);
                    particle.StartSize = 0.3f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.ShulkerBullet:
                    particle.StartColor = new Vector3(0.7f, 0.6f, 1.0f);
                    particle.EndColor = new Vector3(0.5f, 0.4f, 0.8f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.Totem:
                    particle.StartColor = new Vector3(1.0f, 0.9f, 0.3f);
                    particle.EndColor = new Vector3(0.8f, 0.7f, 0.1f);
                    particle.StartSize = 0.3f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Soul:
                    particle.StartColor = new Vector3(0.4f, 0.7f, 0.8f);
                    particle.EndColor = new Vector3(0.2f, 0.5f, 0.6f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.SculkSoul:
                    particle.StartColor = new Vector3(0.2f, 0.1f, 0.4f);
                    particle.EndColor = new Vector3(0.1f, 0.05f, 0.2f);
                    particle.StartSize = 0.25f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.SculkCharge:
                    particle.StartColor = new Vector3(0.1f, 0.8f, 0.9f);
                    particle.EndColor = new Vector3(0.05f, 0.6f, 0.7f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.1f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Shriek:
                    particle.StartColor = new Vector3(0.8f, 0.2f, 0.8f);
                    particle.EndColor = new Vector3(0.6f, 0.1f, 0.6f);
                    particle.StartSize = 0.3f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Vibration:
                    particle.StartColor = new Vector3(0.5f, 0.8f, 1.0f);
                    particle.EndColor = new Vector3(0.3f, 0.6f, 0.8f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.Glow:
                    particle.StartColor = new Vector3(1.0f, 1.0f, 0.8f);
                    particle.EndColor = new Vector3(0.8f, 0.8f, 0.6f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.1f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.GlowSquidInk:
                    particle.StartColor = new Vector3(0.2f, 0.8f, 1.0f);
                    particle.EndColor = new Vector3(0.1f, 0.6f, 0.8f);
                    particle.StartSize = 0.25f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = 0.1f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.SquidInk:
                    particle.StartColor = new Vector3(0.1f, 0.1f, 0.2f);
                    particle.EndColor = new Vector3(0.05f, 0.05f, 0.1f);
                    particle.StartSize = 0.25f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = 0.1f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.SporeBlossom:
                    particle.StartColor = new Vector3(0.7f, 0.6f, 0.9f);
                    particle.EndColor = new Vector3(0.5f, 0.4f, 0.7f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = 0.05f;
                    particle.Drag = 0.4f;
                    break;

                case ParticleType.CrimsonSpore:
                    particle.StartColor = new Vector3(0.9f, 0.3f, 0.4f);
                    particle.EndColor = new Vector3(0.7f, 0.2f, 0.3f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.05f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.WarpedSpore:
                    particle.StartColor = new Vector3(0.3f, 0.8f, 0.8f);
                    particle.EndColor = new Vector3(0.2f, 0.6f, 0.6f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.05f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Ash:
                    particle.StartColor = new Vector3(0.4f, 0.4f, 0.4f);
                    particle.EndColor = new Vector3(0.2f, 0.2f, 0.2f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = 0.1f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.WhiteAsh:
                    particle.StartColor = new Vector3(0.8f, 0.8f, 0.8f);
                    particle.EndColor = new Vector3(0.6f, 0.6f, 0.6f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = 0.1f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Lava:
                    particle.StartColor = new Vector3(1.0f, 0.5f, 0.1f);
                    particle.EndColor = new Vector3(0.8f, 0.3f, 0.0f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.8f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.DrippingLava:
                    particle.StartColor = new Vector3(1.0f, 0.6f, 0.2f);
                    particle.EndColor = new Vector3(0.9f, 0.4f, 0.1f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.5f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.DrippingWater:
                    particle.StartColor = new Vector3(0.3f, 0.6f, 1.0f);
                    particle.EndColor = new Vector3(0.2f, 0.5f, 0.9f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.5f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.DrippingObsidianTear:
                    particle.StartColor = new Vector3(0.5f, 0.3f, 0.8f);
                    particle.EndColor = new Vector3(0.4f, 0.2f, 0.7f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.5f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.DrippingNectar:
                    particle.StartColor = new Vector3(1.0f, 0.8f, 0.3f);
                    particle.EndColor = new Vector3(0.9f, 0.7f, 0.2f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.5f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.FallingDust:
                    particle.StartColor = new Vector3(0.7f, 0.6f, 0.4f);
                    particle.EndColor = new Vector3(0.5f, 0.4f, 0.3f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.FallingLava:
                    particle.StartColor = new Vector3(1.0f, 0.5f, 0.1f);
                    particle.EndColor = new Vector3(0.8f, 0.3f, 0.0f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.FallingWater:
                    particle.StartColor = new Vector3(0.3f, 0.6f, 1.0f);
                    particle.EndColor = new Vector3(0.2f, 0.5f, 0.9f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.FallingNectar:
                    particle.StartColor = new Vector3(1.0f, 0.8f, 0.3f);
                    particle.EndColor = new Vector3(0.9f, 0.7f, 0.2f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.FallingObsidianTear:
                    particle.StartColor = new Vector3(0.5f, 0.3f, 0.8f);
                    particle.EndColor = new Vector3(0.4f, 0.2f, 0.7f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.LandingLava:
                    particle.StartColor = new Vector3(1.0f, 0.6f, 0.2f);
                    particle.EndColor = new Vector3(0.9f, 0.4f, 0.1f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.LandingWater:
                    particle.StartColor = new Vector3(0.3f, 0.6f, 1.0f);
                    particle.EndColor = new Vector3(0.2f, 0.5f, 0.9f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.LandingObsidianTear:
                    particle.StartColor = new Vector3(0.5f, 0.3f, 0.8f);
                    particle.EndColor = new Vector3(0.4f, 0.2f, 0.7f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.LandingNectar:
                    particle.StartColor = new Vector3(1.0f, 0.8f, 0.3f);
                    particle.EndColor = new Vector3(0.9f, 0.7f, 0.2f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Mycelium:
                    particle.StartColor = new Vector3(0.6f, 0.4f, 0.6f);
                    particle.EndColor = new Vector3(0.5f, 0.3f, 0.5f);
                    particle.StartSize = 0.1f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.05f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Sneeze:
                    particle.StartColor = new Vector3(0.8f, 0.8f, 0.8f);
                    particle.EndColor = new Vector3(0.6f, 0.6f, 0.6f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.3f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.AngryVillager:
                    particle.StartColor = new Vector3(0.8f, 0.2f, 0.2f);
                    particle.EndColor = new Vector3(0.6f, 0.1f, 0.1f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.HappyVillager:
                    particle.StartColor = new Vector3(0.3f, 0.8f, 0.3f);
                    particle.EndColor = new Vector3(0.2f, 0.6f, 0.2f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Witch:
                    particle.StartColor = new Vector3(0.5f, 0.2f, 0.8f);
                    particle.EndColor = new Vector3(0.3f, 0.1f, 0.6f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Dolphin:
                    particle.StartColor = new Vector3(0.5f, 0.8f, 0.9f);
                    particle.EndColor = new Vector3(0.3f, 0.6f, 0.7f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.2f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Bubble:
                    particle.StartColor = new Vector3(0.7f, 0.9f, 1.0f);
                    particle.EndColor = new Vector3(0.5f, 0.7f, 0.9f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -0.5f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.BubbleColumnUp:
                    particle.StartColor = new Vector3(0.7f, 0.9f, 1.0f);
                    particle.EndColor = new Vector3(0.5f, 0.7f, 0.9f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = -1.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.BubblePop:
                    particle.StartColor = new Vector3(0.8f, 0.95f, 1.0f);
                    particle.EndColor = new Vector3(0.6f, 0.75f, 0.9f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.3f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.CurrentDown:
                    particle.StartColor = new Vector3(0.3f, 0.5f, 0.8f);
                    particle.EndColor = new Vector3(0.2f, 0.4f, 0.7f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 1.0f;
                    particle.Drag = 0.1f;
                    break;

                case ParticleType.SquidInk:
                    particle.StartColor = new Vector3(0.1f, 0.1f, 0.2f);
                    particle.EndColor = new Vector3(0.05f, 0.05f, 0.1f);
                    particle.StartSize = 0.25f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = 0.1f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.Fishing:
                    particle.StartColor = new Vector3(0.5f, 0.8f, 1.0f);
                    particle.EndColor = new Vector3(0.3f, 0.6f, 0.9f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.5f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Explosion:
                    particle.StartColor = new Vector3(1.0f, 0.8f, 0.5f);
                    particle.EndColor = new Vector3(0.8f, 0.4f, 0.2f);
                    particle.StartSize = 0.5f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.3f;
                    break;

                case ParticleType.ExplosionEmitter:
                    particle.StartColor = new Vector3(1.0f, 0.9f, 0.7f);
                    particle.EndColor = new Vector3(0.9f, 0.7f, 0.5f);
                    particle.StartSize = 0.8f;
                    particle.EndSize = 0.2f;
                    particle.Lifetime = 0.3f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Cloud:
                    particle.StartColor = new Vector3(0.9f, 0.9f, 0.95f);
                    particle.EndColor = new Vector3(0.7f, 0.7f, 0.8f);
                    particle.StartSize = 0.5f;
                    particle.EndSize = 0.2f;
                    particle.Lifetime = 2.0f;
                    particle.Gravity = -0.05f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.ElderGuardian:
                    particle.StartColor = new Vector3(0.5f, 0.7f, 0.8f);
                    particle.EndColor = new Vector3(0.3f, 0.5f, 0.6f);
                    particle.StartSize = 0.5f;
                    particle.EndSize = 0.2f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.Effect:
                    particle.StartColor = new Vector3(0.5f, 0.5f, 0.5f);
                    particle.EndColor = new Vector3(0.3f, 0.3f, 0.3f);
                    particle.StartSize = 0.2f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 1.0f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.InstantEffect:
                    particle.StartColor = new Vector3(0.7f, 0.7f, 0.7f);
                    particle.EndColor = new Vector3(0.5f, 0.5f, 0.5f);
                    particle.StartSize = 0.3f;
                    particle.EndSize = 0.1f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.AmbientEntityEffect:
                    particle.StartColor = new Vector3(0.4f, 0.4f, 0.4f);
                    particle.EndColor = new Vector3(0.2f, 0.2f, 0.2f);
                    particle.StartSize = 0.15f;
                    particle.EndSize = 0.05f;
                    particle.Lifetime = 1.5f;
                    particle.Gravity = -0.05f;
                    particle.Drag = 0.2f;
                    break;

                case ParticleType.BlockMarker:
                    particle.StartColor = new Vector3(1.0f, 0.0f, 0.0f);
                    particle.EndColor = new Vector3(1.0f, 0.0f, 0.0f);
                    particle.StartSize = 0.5f;
                    particle.EndSize = 0.5f;
                    particle.Lifetime = 0.5f;
                    particle.Gravity = 0.0f;
                    particle.Drag = 0.0f;
                    break;

                case ParticleType.Custom:
                    // 使用默认值
                    break;
            }
        }

        public List<Particle> GetParticles()
        {
            return new List<Particle>(particles);
        }

        public void Clear()
        {
            foreach (Particle particle in particles)
            {
                particlePool.Enqueue(particle);
            }
            particles.Clear();
        }

        public void ClearArea(Vector3 center, float radius)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(particles[i].Position, center) < radius)
                {
                    Particle particle = particles[i];
                    particles.RemoveAt(i);
                    particlePool.Enqueue(particle);
                }
            }
        }
    }

    public class Particle
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 StartColor;
        public Vector3 EndColor;
        public Vector3 CurrentColor;
        public float StartSize;
        public float EndSize;
        public float CurrentSize;
        public float StartAlpha;
        public float EndAlpha;
        public float CurrentAlpha;
        public float Lifetime;
        public float Age;
        public float Gravity;
        public float Drag;
        public float Rotation;
        public float RotationSpeed;
        public ParticleType Type;
        public bool IsDead;

        public Particle()
        {
            Reset();
        }

        public void Reset()
        {
            Position = Vector3.Zero;
            Velocity = Vector3.Zero;
            StartColor = Vector3.One;
            EndColor = Vector3.One;
            CurrentColor = Vector3.One;
            StartSize = 0.1f;
            EndSize = 0.05f;
            CurrentSize = 0.1f;
            StartAlpha = 1.0f;
            EndAlpha = 0.0f;
            CurrentAlpha = 1.0f;
            Lifetime = 1.0f;
            Age = 0.0f;
            Gravity = 0.5f;
            Drag = 0.2f;
            Rotation = 0.0f;
            RotationSpeed = 0.0f;
            Type = ParticleType.Custom;
            IsDead = false;
        }
    }

    public enum ParticleType
    {
        Custom,
        Smoke,
        Flame,
        Spark,
        Water,
        Rain,
        Snow,
        Heart,
        Note,
        Enchant,
        Crit,
        Damage,
        Experience,
        Portal,
        BlockBreak,
        BlockPlace,
        Snowball,
        Egg,
        Potion,
        DragonBreath,
        ShulkerBullet,
        Totem,
        Soul,
        SculkSoul,
        SculkCharge,
        Shriek,
        Vibration,
        Glow,
        GlowSquidInk,
        SquidInk,
        SporeBlossom,
        CrimsonSpore,
        WarpedSpore,
        Ash,
        WhiteAsh,
        Lava,
        DrippingLava,
        DrippingWater,
        DrippingObsidianTear,
        DrippingNectar,
        FallingDust,
        FallingLava,
        FallingWater,
        FallingNectar,
        FallingObsidianTear,
        LandingLava,
        LandingWater,
        LandingObsidianTear,
        LandingNectar,
        Mycelium,
        Sneeze,
        AngryVillager,
        HappyVillager,
        Witch,
        Dolphin,
        Bubble,
        BubbleColumnUp,
        BubblePop,
        CurrentDown,
        Fishing,
        Explosion,
        ExplosionEmitter,
        Cloud,
        ElderGuardian,
        Effect,
        InstantEffect,
        AmbientEntityEffect,
        BlockMarker
    }
}
