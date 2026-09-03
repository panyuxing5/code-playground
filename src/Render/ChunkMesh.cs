using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Render
{
    public class ChunkMesh
    {
        public int Vao { get; private set; }
        public int Vbo { get; private set; }
        public int Ebo { get; private set; }
        public int VertexCount { get; private set; }
        public int IndexCount { get; private set; }
        public bool IsBuilt { get; private set; }
        public bool IsEmpty { get; private set; }
        public bool HasVertices => VertexCount > 0;

        private readonly List<float> vertices;
        private readonly List<uint> indices;
        private readonly Chunk chunk;

        public ChunkMesh(Chunk chunk)
        {
            this.chunk = chunk;
            vertices = new List<float>(10000);
            indices = new List<uint>(15000);
            IsBuilt = false;
            IsEmpty = true;
        }

        public ChunkMesh() : this(null)
        {
        }

        public void BuildMesh(WorldManager world)
        {
            vertices.Clear();
            indices.Clear();

            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < GameConstants.CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);
                        if (block == GameConstants.BLOCK_AIR) continue;

                        BlockInfo info = BlockRegistry.GetBlockInfo(block);
                        if (info == null) continue;

                        AddBlockFaces(world, x, y, z, block, info);
                    }
                }
            }

            UploadToGPU();
            IsBuilt = true;
            IsEmpty = vertices.Count == 0;
        }

        private void AddBlockFaces(WorldManager world, int x, int y, int z, ushort block, BlockInfo info)
        {
            // 六个面
            for (int face = 0; face < 6; face++)
            {
                if (!ShouldRenderFace(world, x, y, z, face, info))
                    continue;

                AddFace(x, y, z, face, block, info);
            }
        }

        private bool ShouldRenderFace(WorldManager world, int x, int y, int z, int face, BlockInfo info)
        {
            int nx = x, ny = y, nz = z;

            switch (face)
            {
                case 0: nx++; break; // 右
                case 1: nx--; break; // 左
                case 2: ny++; break; // 上
                case 3: ny--; break; // 下
                case 4: nz++; break; // 前
                case 5: nz--; break; // 后
            }

            ushort neighbor = world.GetBlock(
                chunk.X * GameConstants.CHUNK_SIZE + nx,
                ny,
                chunk.Z * GameConstants.CHUNK_SIZE + nz
            );

            if (neighbor == GameConstants.BLOCK_AIR) return true;

            BlockInfo neighborInfo = BlockRegistry.GetBlockInfo(neighbor);
            if (neighborInfo == null) return true;

            // 透明方块处理
            if (info.IsTransparent && !neighborInfo.IsTransparent) return true;
            if (!info.IsTransparent && neighborInfo.IsTransparent) return true;
            if (info.IsTransparent && neighborInfo.IsTransparent) return true;

            // 液体处理
            if (info.IsLiquid && !neighborInfo.IsLiquid) return true;

            return false;
        }

        private void AddFace(int x, int y, int z, int face, ushort block, BlockInfo info)
        {
            uint baseIndex = (uint)(vertices.Count / 8);

            // 获取纹理坐标
            Vector2[] texCoords = GetTextureCoords(face, block, info);

            // 顶点位置
            Vector3[] positions = GetFacePositions(x, y, z, face);

            // 法线
            Vector3 normal = GetFaceNormal(face);

            // 光照
            float lightLevel = GetLightLevel(x, y, z, face);

            for (int i = 0; i < 4; i++)
            {
                vertices.Add(positions[i].X);
                vertices.Add(positions[i].Y);
                vertices.Add(positions[i].Z);
                vertices.Add(texCoords[i].X);
                vertices.Add(texCoords[i].Y);
                vertices.Add(normal.X);
                vertices.Add(normal.Y);
                vertices.Add(normal.Z);
                vertices.Add(lightLevel);
            }

            // 索引
            indices.Add(baseIndex);
            indices.Add(baseIndex + 1);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 2);
            indices.Add(baseIndex + 3);
            indices.Add(baseIndex);
        }

        private Vector3[] GetFacePositions(int x, int y, int z, int face)
        {
            float x0 = x, x1 = x + 1;
            float y0 = y, y1 = y + 1;
            float z0 = z, z1 = z + 1;

            return face switch
            {
                0 => new[] { new Vector3(x1, y0, z0), new Vector3(x1, y0, z1), new Vector3(x1, y1, z1), new Vector3(x1, y1, z0) },
                1 => new[] { new Vector3(x0, y0, z1), new Vector3(x0, y0, z0), new Vector3(x0, y1, z0), new Vector3(x0, y1, z1) },
                2 => new[] { new Vector3(x0, y1, z0), new Vector3(x0, y1, z1), new Vector3(x1, y1, z1), new Vector3(x1, y1, z0) },
                3 => new[] { new Vector3(x0, y0, z1), new Vector3(x1, y0, z1), new Vector3(x1, y0, z0), new Vector3(x0, y0, z0) },
                4 => new[] { new Vector3(x0, y0, z1), new Vector3(x1, y0, z1), new Vector3(x1, y1, z1), new Vector3(x0, y1, z1) },
                5 => new[] { new Vector3(x1, y0, z0), new Vector3(x0, y0, z0), new Vector3(x0, y1, z0), new Vector3(x1, y1, z0) },
                _ => Array.Empty<Vector3>()
            };
        }

        private Vector3 GetFaceNormal(int face)
        {
            return face switch
            {
                0 => new Vector3(1, 0, 0),
                1 => new Vector3(-1, 0, 0),
                2 => new Vector3(0, 1, 0),
                3 => new Vector3(0, -1, 0),
                4 => new Vector3(0, 0, 1),
                5 => new Vector3(0, 0, -1),
                _ => Vector3.Zero
            };
        }

        private Vector2[] GetTextureCoords(int face, ushort block, BlockInfo info)
        {
            string textureName = face switch
            {
                0 => info.TextureRight,
                1 => info.TextureLeft,
                2 => info.TextureTop,
                3 => info.TextureBottom,
                4 => info.TextureFront,
                5 => info.TextureBack,
                _ => info.TextureFront
            };

            // 计算在纹理图集中的UV
            int atlasSize = 16; // 16x16 纹理
            int textureIndex = 0; // 默认纹理索引
            int texX = textureIndex % atlasSize;
            int texY = textureIndex / atlasSize;

            float u0 = texX / (float)atlasSize;
            float u1 = (texX + 1) / (float)atlasSize;
            float v0 = 1.0f - (texY + 1) / (float)atlasSize;
            float v1 = 1.0f - texY / (float)atlasSize;

            // 稍微缩小UV避免纹理 bleeding
            float padding = 0.001f;
            u0 += padding;
            u1 -= padding;
            v0 += padding;
            v1 -= padding;

            return new[]
            {
                new Vector2(u0, v0),
                new Vector2(u1, v0),
                new Vector2(u1, v1),
                new Vector2(u0, v1)
            };
        }

        private float GetLightLevel(int x, int y, int z, int face)
        {
            // 简单的面光照
            return face switch
            {
                2 => 1.0f,   // 顶面最亮
                0 or 1 => 0.8f, // 侧面
                4 or 5 => 0.7f, // 前后
                3 => 0.5f,   // 底面最暗
                _ => 0.8f
            };
        }

        private void UploadToGPU()
        {
            if (Vao != 0)
            {
                GL.DeleteVertexArray(Vao);
                GL.DeleteBuffer(Vbo);
                GL.DeleteBuffer(Ebo);
            }

            Vao = GL.GenVertexArray();
            Vbo = GL.GenBuffer();
            Ebo = GL.GenBuffer();

            GL.BindVertexArray(Vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

            // 位置
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // 纹理坐标
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 9 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // 法线
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 9 * sizeof(float), 5 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            // 光照
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, 9 * sizeof(float), 8 * sizeof(float));
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0);

            VertexCount = vertices.Count / 9;
            IndexCount = indices.Count;
        }

        public void Render()
        {
            if (!IsBuilt || IsEmpty) return;

            GL.BindVertexArray(Vao);
            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (Vao != 0)
            {
                GL.DeleteVertexArray(Vao);
                GL.DeleteBuffer(Vbo);
                GL.DeleteBuffer(Ebo);
                Vao = 0;
                Vbo = 0;
                Ebo = 0;
            }
            IsBuilt = false;
        }
    }
}
