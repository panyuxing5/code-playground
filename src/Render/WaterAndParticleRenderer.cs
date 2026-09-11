using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Render
{
    public class WaterRenderer
    {
        private readonly ShaderManager shaderManager;
        private readonly Camera camera;
        private readonly WorldManager world;
        private int waterVao;
        private int waterVbo;
        private int waterVertexCount;

        // 水的参数
        public float WaterLevel { get; set; } = 62.0f;
        public float WaveSpeed { get; set; } = 1.0f;
        public float WaveHeight { get; set; } = 0.1f;
        public Vector3 WaterColor { get; set; } = new Vector3(0.2f, 0.4f, 0.7f);
        public float Transparency { get; set; } = 0.6f;

        public WaterRenderer(ShaderManager shaderManager, Camera camera, WorldManager world)
        {
            this.shaderManager = shaderManager;
            this.camera = camera;
            this.world = world;
        }

        public void Initialize()
        {
            CreateWaterMesh();
            Console.WriteLine("[WaterRenderer] 水渲染器初始化完成");
        }

        private void CreateWaterMesh()
        {
            // 创建一个大的水面网格
            int gridSize = 100;
            float cellSize = 2.0f;
            List<float> vertices = new List<float>();

            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    float x0 = (x - gridSize / 2f) * cellSize;
                    float z0 = (z - gridSize / 2f) * cellSize;
                    float x1 = x0 + cellSize;
                    float z1 = z0 + cellSize;

                    // 两个三角形
                    vertices.AddRange(new float[] {
                        x0, 0, z0,  0, 0,
                        x1, 0, z0,  1, 0,
                        x1, 0, z1,  1, 1,
                        x0, 0, z0,  0, 0,
                        x1, 0, z1,  1, 1,
                        x0, 0, z1,  0, 1
                    });
                }
            }

            waterVertexCount = vertices.Count / 5;

            waterVao = GL.GenVertexArray();
            waterVbo = GL.GenBuffer();

            GL.BindVertexArray(waterVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, waterVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
        }

        public void Render(float deltaTime, float dayTime)
        {
            Shader waterShader = shaderManager.GetShader("water");
            if (waterShader == null) return;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);

            waterShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            Matrix4 model = Matrix4.CreateTranslation(camera.Position.X, WaterLevel, camera.Position.Z);

            waterShader.SetMatrix4("view", view);
            waterShader.SetMatrix4("projection", projection);
            waterShader.SetMatrix4("model", model);
            waterShader.SetFloat("time", (float)GameEngine.Instance.Time);
            waterShader.SetFloat("waveSpeed", WaveSpeed);
            waterShader.SetFloat("waveHeight", WaveHeight);
            waterShader.SetVector3("waterColor", WaterColor);
            waterShader.SetFloat("transparency", Transparency);
            waterShader.SetVector3("cameraPos", camera.Position);

            // 光照
            Vector3 lightDir = new Vector3(0.5f, 1.0f, 0.3f).Normalized();
            waterShader.SetVector3("lightDir", lightDir);
            waterShader.SetFloat("dayTime", dayTime);

            GL.BindVertexArray(waterVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, waterVertexCount);
            GL.BindVertexArray(0);

            waterShader.Unbind();

            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
        }

        public void Dispose()
        {
            if (waterVao != 0)
            {
                GL.DeleteVertexArray(waterVao);
                GL.DeleteBuffer(waterVbo);
            }
            Console.WriteLine("[WaterRenderer] 水渲染器已释放");
        }
    }

    // ========================================
    // 粒子渲染器
    // ========================================
    public class ParticleRenderer
    {
        private readonly ShaderManager shaderManager;
        private readonly Camera camera;
        private readonly List<Particle> particles;
        private int particleVao;
        private int particleVbo;

        public ParticleRenderer(ShaderManager shaderManager, Camera camera)
        {
            this.shaderManager = shaderManager;
            this.camera = camera;
            particles = new List<Particle>(1000);
        }

        public void Initialize()
        {
            CreateParticleMesh();
            Console.WriteLine("[ParticleRenderer] 粒子渲染器初始化完成");
        }

        private void CreateParticleMesh()
        {
            float size = 0.1f;
            float[] vertices = {
                -size, -size, 0,  0, 0,
                 size, -size, 0,  1, 0,
                 size,  size, 0,  1, 1,
                -size, -size, 0,  0, 0,
                 size,  size, 0,  1, 1,
                -size,  size, 0,  0, 1
            };

            particleVao = GL.GenVertexArray();
            particleVbo = GL.GenBuffer();

            GL.BindVertexArray(particleVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, particleVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.BindVertexArray(0);
        }

        public void AddParticle(Particle particle)
        {
            if (particles.Count < 10000)
            {
                particles.Add(particle);
            }
        }

        public void AddBlockBreakParticles(Vector3 position, ushort blockId)
        {
            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                Particle particle = new Particle
                {
                    Position = position + new Vector3(
                        (float)random.NextDouble(),
                        (float)random.NextDouble(),
                        (float)random.NextDouble()
                    ),
                    Velocity = new Vector3(
                        (float)(random.NextDouble() - 0.5) * 2,
                        (float)random.NextDouble() * 3,
                        (float)(random.NextDouble() - 0.5) * 2
                    ),
                    Color = GetBlockParticleColor(blockId),
                    Size = 0.1f,
                    Life = 1.0f,
                    MaxLife = 1.0f,
                    Gravity = -9.8f
                };
                particles.Add(particle);
            }
        }

        public void AddBlockPlaceParticles(Vector3 position, ushort blockId)
        {
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                Particle particle = new Particle
                {
                    Position = position + new Vector3(
                        (float)random.NextDouble(),
                        1.0f,
                        (float)random.NextDouble()
                    ),
                    Velocity = new Vector3(
                        (float)(random.NextDouble() - 0.5) * 1,
                        (float)random.NextDouble() * 2,
                        (float)(random.NextDouble() - 0.5) * 1
                    ),
                    Color = GetBlockParticleColor(blockId),
                    Size = 0.08f,
                    Life = 0.5f,
                    MaxLife = 0.5f,
                    Gravity = -5.0f
                };
                particles.Add(particle);
            }
        }

        public void AddRainParticles(Vector3 cameraPos, int count)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                Particle particle = new Particle
                {
                    Position = cameraPos + new Vector3(
                        (float)(random.NextDouble() - 0.5) * 40,
                        20,
                        (float)(random.NextDouble() - 0.5) * 40
                    ),
                    Velocity = new Vector3(0, -20, 0),
                    Color = new Vector3(0.5f, 0.6f, 0.8f),
                    Size = 0.05f,
                    Life = 2.0f,
                    MaxLife = 2.0f,
                    Gravity = 0
                };
                particles.Add(particle);
            }
        }

        public void AddSnowParticles(Vector3 cameraPos, int count)
        {
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                Particle particle = new Particle
                {
                    Position = cameraPos + new Vector3(
                        (float)(random.NextDouble() - 0.5) * 40,
                        20,
                        (float)(random.NextDouble() - 0.5) * 40
                    ),
                    Velocity = new Vector3(
                        (float)(random.NextDouble() - 0.5) * 0.5f,
                        -2,
                        (float)(random.NextDouble() - 0.5) * 0.5f
                    ),
                    Color = new Vector3(1.0f, 1.0f, 1.0f),
                    Size = 0.1f,
                    Life = 5.0f,
                    MaxLife = 5.0f,
                    Gravity = -1.0f
                };
                particles.Add(particle);
            }
        }

        public void Update(float deltaTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle particle = particles[i];
                particle.Life -= deltaTime;

                if (particle.Life <= 0)
                {
                    particles.RemoveAt(i);
                    continue;
                }

                particle.Velocity.Y += particle.Gravity * deltaTime;
                particle.Position += particle.Velocity * deltaTime;
                particles[i] = particle;
            }
        }

        public void Render()
        {
            if (particles.Count == 0) return;

            Shader particleShader = shaderManager.GetShader("particle");
            if (particleShader == null) return;

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.DepthMask(false);

            particleShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            particleShader.SetMatrix4("view", view);
            particleShader.SetMatrix4("projection", projection);

            foreach (Particle particle in particles)
            {
                float alpha = particle.Life / particle.MaxLife;
                Matrix4 model = Matrix4.CreateTranslation(particle.Position);
                model *= Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(camera.Rotation)); // 面向相机

                particleShader.SetMatrix4("model", model);
                particleShader.SetVector4("color", new Vector4(particle.Color, alpha));
                particleShader.SetFloat("size", particle.Size);

                GL.BindVertexArray(particleVao);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
                GL.BindVertexArray(0);
            }

            particleShader.Unbind();

            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
        }

        private Vector3 GetBlockParticleColor(ushort blockId)
        {
            return blockId switch
            {
                GameConstants.BLOCK_GRASS => new Vector3(0.4f, 0.7f, 0.2f),
                GameConstants.BLOCK_DIRT => new Vector3(0.5f, 0.35f, 0.2f),
                GameConstants.BLOCK_STONE => new Vector3(0.5f, 0.5f, 0.5f),
                GameConstants.BLOCK_SAND => new Vector3(0.9f, 0.85f, 0.6f),
                GameConstants.BLOCK_WOOD_PLANKS => new Vector3(0.6f, 0.45f, 0.25f),
                GameConstants.BLOCK_LOG => new Vector3(0.4f, 0.3f, 0.15f),
                GameConstants.BLOCK_LEAVES => new Vector3(0.2f, 0.5f, 0.1f),
                GameConstants.BLOCK_COBBLESTONE => new Vector3(0.45f, 0.45f, 0.45f),
                _ => new Vector3(0.6f, 0.6f, 0.6f)
            };
        }

        public void Clear()
        {
            particles.Clear();
        }

        public void Dispose()
        {
            if (particleVao != 0)
            {
                GL.DeleteVertexArray(particleVao);
                GL.DeleteBuffer(particleVbo);
            }
            Console.WriteLine("[ParticleRenderer] 粒子渲染器已释放");
        }
    }

    // ========================================
    // 粒子结构
    // ========================================
    public struct Particle
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Color;
        public float Size;
        public float Life;
        public float MaxLife;
        public float Gravity;
    }
}
