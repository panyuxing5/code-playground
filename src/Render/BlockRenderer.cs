using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Render
{
    public class BlockRenderer
    {
        private readonly ShaderManager shaderManager;
        private readonly TextureManager textureManager;
        private readonly Camera camera;
        private readonly WorldManager world;

        // 渲染统计
        public int RenderedChunks { get; private set; }
        public int RenderedFaces { get; private set; }
        public int CulledChunks { get; private set; }

        public BlockRenderer(ShaderManager shaderManager, TextureManager textureManager, Camera camera, WorldManager world)
        {
            this.shaderManager = shaderManager;
            this.textureManager = textureManager;
            this.camera = camera;
            this.world = world;
        }

        public void Initialize()
        {
            Console.WriteLine("[BlockRenderer] 方块渲染器初始化完成");
        }

        public void Render(ChunkManager chunkManager, float deltaTime)
        {
            RenderedChunks = 0;
            RenderedFaces = 0;
            CulledChunks = 0;

            Shader blockShader = shaderManager.GetShader("block");
            if (blockShader == null) return;

            blockShader.Use();

            // 设置 uniforms
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            blockShader.SetMatrix4("view", view);
            blockShader.SetMatrix4("projection", projection);
            blockShader.SetVector3("cameraPos", camera.Position);
            blockShader.SetFloat("time", (float)GameEngine.Instance.Time);

            // 光照
            blockShader.SetVector3("lightDir", new Vector3(0.5f, 1.0f, 0.3f).Normalized());
            blockShader.SetVector3("lightColor", new Vector3(1.0f, 0.95f, 0.85f));
            blockShader.SetVector3("ambientColor", new Vector3(0.3f, 0.3f, 0.35f));

            // 雾效
            blockShader.SetFloat("fogDensity", 0.008f);
            blockShader.SetVector3("fogColor", new Vector3(0.7f, 0.8f, 0.9f));

            // 绑定纹理图集
            textureManager.BindBlockTextureAtlas();

            // 获取可见区块
            List<Chunk> visibleChunks = GetVisibleChunks(chunkManager);

            foreach (Chunk chunk in visibleChunks)
            {
                if (chunk == null || !chunk.IsGenerated || chunk.Mesh == null || !chunk.Mesh.IsBuilt || chunk.Mesh.IsEmpty)
                {
                    CulledChunks++;
                    continue;
                }

                // 视锥体剔除
                if (!IsChunkVisible(chunk))
                {
                    CulledChunks++;
                    continue;
                }

                // 区块偏移
                Matrix4 model = Matrix4.CreateTranslation(
                    chunk.X * GameConstants.CHUNK_SIZE,
                    0,
                    chunk.Z * GameConstants.CHUNK_SIZE
                );
                blockShader.SetMatrix4("model", model);

                chunk.Mesh.Render();
                RenderedChunks++;
                RenderedFaces += chunk.Mesh.IndexCount / 3;
            }

            blockShader.Unbind();
        }

        private List<Chunk> GetVisibleChunks(ChunkManager chunkManager)
        {
            List<Chunk> chunks = new List<Chunk>();
            int renderDistance = GameEngine.Instance.RenderDistance;
            int playerChunkX = (int)camera.Position.X >> 4;
            int playerChunkZ = (int)camera.Position.Z >> 4;

            for (int dx = -renderDistance; dx <= renderDistance; dx++)
            {
                for (int dz = -renderDistance; dz <= renderDistance; dz++)
                {
                    Chunk chunk = chunkManager.GetChunk(playerChunkX + dx, playerChunkZ + dz);
                    if (chunk != null)
                    {
                        chunks.Add(chunk);
                    }
                }
            }

            return chunks;
        }

        private bool IsChunkVisible(Chunk chunk)
        {
            // 简单的视锥体剔除
            Vector3 chunkCenter = new Vector3(
                chunk.X * GameConstants.CHUNK_SIZE + GameConstants.CHUNK_SIZE / 2f,
                GameConstants.CHUNK_HEIGHT / 2f,
                chunk.Z * GameConstants.CHUNK_SIZE + GameConstants.CHUNK_SIZE / 2f
            );

            float radius = GameConstants.CHUNK_SIZE * 0.7f;

            return camera.FrustumContains(chunkCenter, radius);
        }

        public void RenderBlockHighlight(Vector3i position)
        {
            Shader highlightShader = shaderManager.GetShader("highlight");
            if (highlightShader == null) return;

            highlightShader.Use();

            Matrix4 model = Matrix4.CreateTranslation(position.X, position.Y, position.Z);
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

            highlightShader.SetMatrix4("model", model);
            highlightShader.SetMatrix4("view", view);
            highlightShader.SetMatrix4("projection", projection);
            highlightShader.SetVector4("color", new Vector4(0f, 0f, 0f, 0.4f));

            // 渲染线框立方体
            RenderWireframeCube();

            highlightShader.Unbind();
        }

        private void RenderWireframeCube()
        {
            float[] vertices = {
                0, 0, 0,  1, 0, 0,
                1, 0, 0,  1, 0, 1,
                1, 0, 1,  0, 0, 1,
                0, 0, 1,  0, 0, 0,
                0, 1, 0,  1, 1, 0,
                1, 1, 0,  1, 1, 1,
                1, 1, 1,  0, 1, 1,
                0, 1, 1,  0, 1, 0,
                0, 0, 0,  0, 1, 0,
                1, 0, 0,  1, 1, 0,
                1, 0, 1,  1, 1, 1,
                0, 0, 1,  0, 1, 1
            };

            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length / 3);

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
        }

        public void Dispose()
        {
            Console.WriteLine("[BlockRenderer] 方块渲染器已释放");
        }
    }
}
