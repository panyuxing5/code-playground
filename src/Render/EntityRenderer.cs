using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Entities;
using VoxelCraft.World;

namespace VoxelCraft.Render
{
    public class EntityRenderer
    {
        private readonly ShaderManager shaderManager;
        private readonly Camera camera;
        private readonly TextureManager textureManager;

        // 实体模型缓存
        private readonly Dictionary<EntityType, EntityModel> modelCache;

        // 渲染统计
        public int RenderedEntities { get; private set; }
        public int CulledEntities { get; private set; }

        public EntityRenderer(ShaderManager shaderManager, Camera camera, TextureManager textureManager)
        {
            this.shaderManager = shaderManager;
            this.camera = camera;
            this.textureManager = textureManager;
            modelCache = new Dictionary<EntityType, EntityModel>();
        }

        public void Initialize()
        {
            // 预生成实体模型
            GenerateEntityModels();
            Console.WriteLine("[EntityRenderer] 实体渲染器初始化完成");
        }

        private void GenerateEntityModels()
        {
            // 玩家模型
            modelCache[EntityType.Player] = GeneratePlayerModel();
            // 僵尸模型
            modelCache[EntityType.Zombie] = GenerateZombieModel();
            // 骷髅模型
            modelCache[EntityType.Skeleton] = GenerateSkeletonModel();
            // 爬行者模型
            modelCache[EntityType.Creeper] = GenerateCreeperModel();
            // 蜘蛛模型
            modelCache[EntityType.Spider] = GenerateSpiderModel();
            // 牛模型
            modelCache[EntityType.Cow] = GenerateCowModel();
            // 猪模型
            modelCache[EntityType.Pig] = GeneratePigModel();
            // 羊模型
            modelCache[EntityType.Sheep] = GenerateSheepModel();
            // 鸡模型
            modelCache[EntityType.Chicken] = GenerateChickenModel();
            // 村民模型
            modelCache[EntityType.Villager] = GenerateVillagerModel();
        }

        private EntityModel GeneratePlayerModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-4, 0, -4, 8, 8, 8, new Vector3(1, 0.9f, 0.8f));
            // 身体
            model.AddBox(-4, -12, -2, 8, 12, 4, new Vector3(0.3f, 0.5f, 0.8f));
            // 左臂
            model.AddBox(-8, -12, -2, 4, 12, 4, new Vector3(1, 0.9f, 0.8f));
            // 右臂
            model.AddBox(4, -12, -2, 4, 12, 4, new Vector3(1, 0.9f, 0.8f));
            // 左腿
            model.AddBox(-4, -24, -2, 4, 12, 4, new Vector3(0.2f, 0.3f, 0.6f));
            // 右腿
            model.AddBox(0, -24, -2, 4, 12, 4, new Vector3(0.2f, 0.3f, 0.6f));

            model.Build();
            return model;
        }

        private EntityModel GenerateZombieModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-4, 0, -4, 8, 8, 8, new Vector3(0.4f, 0.6f, 0.3f));
            // 身体
            model.AddBox(-4, -12, -2, 8, 12, 4, new Vector3(0.2f, 0.4f, 0.2f));
            // 左臂
            model.AddBox(-8, -12, -2, 4, 12, 4, new Vector3(0.4f, 0.6f, 0.3f));
            // 右臂
            model.AddBox(4, -12, -2, 4, 12, 4, new Vector3(0.4f, 0.6f, 0.3f));
            // 左腿
            model.AddBox(-4, -24, -2, 4, 12, 4, new Vector3(0.2f, 0.3f, 0.15f));
            // 右腿
            model.AddBox(0, -24, -2, 4, 12, 4, new Vector3(0.2f, 0.3f, 0.15f));

            model.Build();
            return model;
        }

        private EntityModel GenerateSkeletonModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-4, 0, -4, 8, 8, 8, new Vector3(0.9f, 0.9f, 0.9f));
            // 身体
            model.AddBox(-3, -12, -1.5f, 6, 12, 3, new Vector3(0.8f, 0.8f, 0.8f));
            // 左臂
            model.AddBox(-7, -12, -1, 2, 12, 2, new Vector3(0.9f, 0.9f, 0.9f));
            // 右臂
            model.AddBox(5, -12, -1, 2, 12, 2, new Vector3(0.9f, 0.9f, 0.9f));
            // 左腿
            model.AddBox(-3, -24, -1, 2, 12, 2, new Vector3(0.9f, 0.9f, 0.9f));
            // 右腿
            model.AddBox(1, -24, -1, 2, 12, 2, new Vector3(0.9f, 0.9f, 0.9f));

            model.Build();
            return model;
        }

        private EntityModel GenerateCreeperModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-4, 0, -4, 8, 8, 8, new Vector3(0.2f, 0.7f, 0.2f));
            // 身体
            model.AddBox(-4, -12, -2, 8, 12, 4, new Vector3(0.2f, 0.6f, 0.2f));
            // 左腿
            model.AddBox(-4, -18, -2, 4, 6, 4, new Vector3(0.2f, 0.7f, 0.2f));
            // 右腿
            model.AddBox(0, -18, -2, 4, 6, 4, new Vector3(0.2f, 0.7f, 0.2f));

            model.Build();
            return model;
        }

        private EntityModel GenerateSpiderModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-3, 0, -3, 6, 6, 6, new Vector3(0.4f, 0.2f, 0.2f));
            // 身体
            model.AddBox(-5, -3, -4, 10, 5, 8, new Vector3(0.5f, 0.25f, 0.2f));
            // 腿
            for (int i = 0; i < 4; i++)
            {
                float angle = i * (float)Math.PI / 2;
                model.AddBox(
                    (float)Math.Cos(angle) * 6 - 1,
                    -2,
                    (float)Math.Sin(angle) * 6 - 1,
                    2, 2, 8,
                    new Vector3(0.4f, 0.2f, 0.2f)
                );
            }

            model.Build();
            return model;
        }

        private EntityModel GenerateCowModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-3, 2, -6, 6, 6, 6, new Vector3(0.6f, 0.4f, 0.3f));
            // 身体
            model.AddBox(-5, -4, -4, 10, 8, 12, new Vector3(0.5f, 0.35f, 0.25f));
            // 腿
            model.AddBox(-4, -12, -3, 3, 8, 3, new Vector3(0.5f, 0.35f, 0.25f));
            model.AddBox(1, -12, -3, 3, 8, 3, new Vector3(0.5f, 0.35f, 0.25f));
            model.AddBox(-4, -12, 2, 3, 8, 3, new Vector3(0.5f, 0.35f, 0.25f));
            model.AddBox(1, -12, 2, 3, 8, 3, new Vector3(0.5f, 0.35f, 0.25f));
            // 角
            model.AddBox(-4, 6, -5, 1, 3, 1, new Vector3(0.8f, 0.8f, 0.8f));
            model.AddBox(3, 6, -5, 1, 3, 1, new Vector3(0.8f, 0.8f, 0.8f));

            model.Build();
            return model;
        }

        private EntityModel GeneratePigModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-3, 1, -5, 6, 6, 6, new Vector3(0.9f, 0.6f, 0.6f));
            // 鼻子
            model.AddBox(-2, 2, -6, 4, 3, 1, new Vector3(0.8f, 0.5f, 0.5f));
            // 身体
            model.AddBox(-4, -3, -3, 8, 7, 10, new Vector3(0.9f, 0.6f, 0.6f));
            // 腿
            model.AddBox(-3, -9, -2, 2, 6, 2, new Vector3(0.9f, 0.6f, 0.6f));
            model.AddBox(1, -9, -2, 2, 6, 2, new Vector3(0.9f, 0.6f, 0.6f));
            model.AddBox(-3, -9, 2, 2, 6, 2, new Vector3(0.9f, 0.6f, 0.6f));
            model.AddBox(1, -9, 2, 2, 6, 2, new Vector3(0.9f, 0.6f, 0.6f));

            model.Build();
            return model;
        }

        private EntityModel GenerateSheepModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-2.5f, 1, -4, 5, 5, 5, new Vector3(0.6f, 0.4f, 0.3f));
            // 身体（羊毛）
            model.AddBox(-4, -2, -3, 8, 8, 10, new Vector3(0.95f, 0.95f, 0.95f));
            // 腿
            model.AddBox(-3, -8, -2, 2, 6, 2, new Vector3(0.6f, 0.4f, 0.3f));
            model.AddBox(1, -8, -2, 2, 6, 2, new Vector3(0.6f, 0.4f, 0.3f));
            model.AddBox(-3, -8, 2, 2, 6, 2, new Vector3(0.6f, 0.4f, 0.3f));
            model.AddBox(1, -8, 2, 2, 6, 2, new Vector3(0.6f, 0.4f, 0.3f));

            model.Build();
            return model;
        }

        private EntityModel GenerateChickenModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-1.5f, 3, -1.5f, 3, 3, 3, new Vector3(0.9f, 0.9f, 0.9f));
            // 喙
            model.AddBox(-1, 4, -2.5f, 2, 1, 1, new Vector3(1, 0.8f, 0));
            // 肉垂
            model.AddBox(-0.5f, 2, -2, 1, 1, 0.5f, new Vector3(1, 0, 0));
            // 身体
            model.AddBox(-2, -1, -2, 4, 5, 5, new Vector3(0.9f, 0.9f, 0.9f));
            // 翅膀
            model.AddBox(-3, 0, -1, 1, 3, 3, new Vector3(0.85f, 0.85f, 0.85f));
            model.AddBox(2, 0, -1, 1, 3, 3, new Vector3(0.85f, 0.85f, 0.85f));
            // 腿
            model.AddBox(-1, -5, -0.5f, 1, 4, 1, new Vector3(1, 0.8f, 0));
            model.AddBox(0, -5, -0.5f, 1, 4, 1, new Vector3(1, 0.8f, 0));

            model.Build();
            return model;
        }

        private EntityModel GenerateVillagerModel()
        {
            EntityModel model = new EntityModel();

            // 头部
            model.AddBox(-4, 0, -4, 8, 8, 8, new Vector3(0.8f, 0.6f, 0.4f));
            // 鼻子
            model.AddBox(-1, 3, -5, 2, 3, 1, new Vector3(0.7f, 0.5f, 0.3f));
            // 身体（长袍）
            model.AddBox(-4, -12, -3, 8, 12, 6, new Vector3(0.5f, 0.4f, 0.3f));
            // 左臂
            model.AddBox(-8, -10, -2, 3, 10, 4, new Vector3(0.5f, 0.4f, 0.3f));
            // 右臂
            model.AddBox(5, -10, -2, 3, 10, 4, new Vector3(0.5f, 0.4f, 0.3f));
            // 腿
            model.AddBox(-3, -20, -2, 3, 8, 4, new Vector3(0.4f, 0.3f, 0.2f));
            model.AddBox(0, -20, -2, 3, 8, 4, new Vector3(0.4f, 0.3f, 0.2f));

            model.Build();
            return model;
        }

        public void Render(List<Entity> entities, float deltaTime)
        {
            RenderedEntities = 0;
            CulledEntities = 0;

            Shader entityShader = shaderManager.GetShader("entity");
            if (entityShader == null) return;

            entityShader.Use();

            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            entityShader.SetMatrix4("view", view);
            entityShader.SetMatrix4("projection", projection);
            entityShader.SetVector3("lightDir", new Vector3(0.5f, 1.0f, 0.3f).Normalized());

            foreach (Entity entity in entities)
            {
                if (entity.IsDead) continue;

                // 视锥体剔除
                if (!camera.FrustumContains(entity.Position, 2.0f))
                {
                    CulledEntities++;
                    continue;
                }

                // 距离剔除
                if (Vector3.Distance(entity.Position, camera.Position) > 128)
                {
                    CulledEntities++;
                    continue;
                }

                if (modelCache.TryGetValue(entity.Type, out EntityModel model))
                {
                    RenderEntity(entity, model, entityShader);
                    RenderedEntities++;
                }
            }

            entityShader.Unbind();
        }

        private void RenderEntity(Entity entity, EntityModel model, Shader shader)
        {
            Matrix4 modelMatrix = Matrix4.CreateTranslation(entity.Position);
            modelMatrix *= Matrix4.CreateRotationY(entity.Yaw);

            shader.SetMatrix4("model", modelMatrix);

            model.Render();
        }

        public void Dispose()
        {
            foreach (EntityModel model in modelCache.Values)
            {
                model.Dispose();
            }
            modelCache.Clear();
            Console.WriteLine("[EntityRenderer] 实体渲染器已释放");
        }
    }

    // ========================================
    // 实体模型
    // ========================================
    public class EntityModel
    {
        private readonly List<ModelBox> boxes;
        private int vao;
        private int vbo;
        private int vertexCount;
        private bool isBuilt;

        public EntityModel()
        {
            boxes = new List<ModelBox>();
        }

        public void AddBox(float x, float y, float z, float width, float height, float depth, Vector3 color)
        {
            boxes.Add(new ModelBox
            {
                X = x,
                Y = y,
                Z = z,
                Width = width,
                Height = height,
                Depth = depth,
                Color = color
            });
        }

        public void Build()
        {
            List<float> vertices = new List<float>();

            foreach (ModelBox box in boxes)
            {
                AddBoxVertices(vertices, box);
            }

            vertexCount = vertices.Count / 6;

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
            isBuilt = true;
        }

        private void AddBoxVertices(List<float> vertices, ModelBox box)
        {
            float x0 = box.X;
            float x1 = box.X + box.Width;
            float y0 = box.Y;
            float y1 = box.Y + box.Height;
            float z0 = box.Z;
            float z1 = box.Z + box.Depth;

            float r = box.Color.X;
            float g = box.Color.Y;
            float b = box.Color.Z;

            // 前面
            vertices.AddRange(new float[] {
                x0, y0, z1, r, g, b,
                x1, y0, z1, r, g, b,
                x1, y1, z1, r, g, b,
                x0, y0, z1, r, g, b,
                x1, y1, z1, r, g, b,
                x0, y1, z1, r, g, b
            });

            // 后面
            vertices.AddRange(new float[] {
                x1, y0, z0, r, g, b,
                x0, y0, z0, r, g, b,
                x0, y1, z0, r, g, b,
                x1, y0, z0, r, g, b,
                x0, y1, z0, r, g, b,
                x1, y1, z0, r, g, b
            });

            // 左面
            vertices.AddRange(new float[] {
                x0, y0, z0, r, g, b,
                x0, y0, z1, r, g, b,
                x0, y1, z1, r, g, b,
                x0, y0, z0, r, g, b,
                x0, y1, z1, r, g, b,
                x0, y1, z0, r, g, b
            });

            // 右面
            vertices.AddRange(new float[] {
                x1, y0, z1, r, g, b,
                x1, y0, z0, r, g, b,
                x1, y1, z0, r, g, b,
                x1, y0, z1, r, g, b,
                x1, y1, z0, r, g, b,
                x1, y1, z1, r, g, b
            });

            // 顶面
            vertices.AddRange(new float[] {
                x0, y1, z1, r, g, b,
                x1, y1, z1, r, g, b,
                x1, y1, z0, r, g, b,
                x0, y1, z1, r, g, b,
                x1, y1, z0, r, g, b,
                x0, y1, z0, r, g, b
            });

            // 底面
            vertices.AddRange(new float[] {
                x0, y0, z0, r, g, b,
                x1, y0, z0, r, g, b,
                x1, y0, z1, r, g, b,
                x0, y0, z0, r, g, b,
                x1, y0, z1, r, g, b,
                x0, y0, z1, r, g, b
            });
        }

        public void Render()
        {
            if (!isBuilt) return;

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (vao != 0)
            {
                GL.DeleteVertexArray(vao);
                GL.DeleteBuffer(vbo);
                vao = 0;
                vbo = 0;
            }
        }
    }

    public struct ModelBox
    {
        public float X;
        public float Y;
        public float Z;
        public float Width;
        public float Height;
        public float Depth;
        public Vector3 Color;
    }
}
