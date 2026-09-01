using System;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Render;

namespace VoxelCraft.World
{
    public class Chunk : IDisposable
    {
        public int X { get; private set; }
        public int Z { get; private set; }
        public bool IsGenerated { get; set; } = false;
        public bool IsDirty { get; set; } = true;
        public bool IsMeshBuilt { get; set; } = false;
        public bool HasEntities { get; set; } = false;

        // 方块数据
        private ushort[] blocks;
        private byte[] skyLight;
        private byte[] blockLight;
        private byte[] metadata;

        // 渲染数据
        public ChunkMesh Mesh { get; private set; }
        public ChunkMesh WaterMesh { get; private set; }
        public ChunkMesh TransparentMesh { get; private set; }

        // 统计
        public int SolidBlockCount { get; private set; }
        public int AirBlockCount { get; private set; }

        // 区块状态
        public bool IsLoaded { get; set; } = false;
        public bool IsPopulated { get; set; } = false;
        public bool IsLightCalculated { get; set; } = false;

        // 邻居引用
        public Chunk NorthNeighbor { get; set; }
        public Chunk SouthNeighbor { get; set; }
        public Chunk EastNeighbor { get; set; }
        public Chunk WestNeighbor { get; set; }

        // 实体列表
        public System.Collections.Generic.List<Entities.Entity> Entities { get; private set; }
            = new System.Collections.Generic.List<Entities.Entity>();

        // 区块修改时间
        public DateTime LastModified { get; set; } = DateTime.Now;
        public DateTime LastAccessed { get; set; } = DateTime.Now;

        public Chunk(int x, int z)
        {
            X = x;
            Z = z;
            int size = GameConstants.CHUNK_SIZE * GameConstants.CHUNK_HEIGHT * GameConstants.CHUNK_SIZE;
            blocks = new ushort[size];
            skyLight = new byte[size];
            blockLight = new byte[size];
            metadata = new byte[size];
            AirBlockCount = size;
        }

        // ========================================
        // 方块操作
        // ========================================
        public ushort GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return GameConstants.BLOCK_AIR;
            }

            int index = GetIndex(x, y, z);
            return blocks[index];
        }

        public void SetBlock(int x, int y, int z, ushort blockId)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return;
            }

            int index = GetIndex(x, y, z);
            ushort oldBlock = blocks[index];

            if (oldBlock == blockId) return;

            blocks[index] = blockId;
            IsDirty = true;
            LastModified = DateTime.Now;

            if (oldBlock == GameConstants.BLOCK_AIR && blockId != GameConstants.BLOCK_AIR)
            {
                SolidBlockCount++;
                AirBlockCount--;
            }
            else if (oldBlock != GameConstants.BLOCK_AIR && blockId == GameConstants.BLOCK_AIR)
            {
                SolidBlockCount--;
                AirBlockCount++;
            }
        }

        public byte GetMetadata(int x, int y, int z)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return 0;
            }

            return metadata[GetIndex(x, y, z)];
        }

        public void SetMetadata(int x, int y, int z, byte value)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return;
            }

            metadata[GetIndex(x, y, z)] = value;
            IsDirty = true;
        }

        // ========================================
        // 光照操作
        // ========================================
        public byte GetSkyLight(int x, int y, int z)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return 15;
            }

            return skyLight[GetIndex(x, y, z)];
        }

        public void SetSkyLight(int x, int y, int z, byte value)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return;
            }

            skyLight[GetIndex(x, y, z)] = value;
            IsDirty = true;
        }

        public byte GetBlockLight(int x, int y, int z)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return 0;
            }

            return blockLight[GetIndex(x, y, z)];
        }

        public void SetBlockLight(int x, int y, int z, byte value)
        {
            if (x < 0 || x >= GameConstants.CHUNK_SIZE ||
                y < 0 || y >= GameConstants.CHUNK_HEIGHT ||
                z < 0 || z >= GameConstants.CHUNK_SIZE)
            {
                return;
            }

            blockLight[GetIndex(x, y, z)] = value;
            IsDirty = true;
        }

        public byte GetLightLevel(int x, int y, int z)
        {
            byte sky = GetSkyLight(x, y, z);
            byte block = GetBlockLight(x, y, z);
            return Math.Max(sky, block);
        }

        // ========================================
        // 索引计算
        // ========================================
        private int GetIndex(int x, int y, int z)
        {
            return (y * GameConstants.CHUNK_SIZE + z) * GameConstants.CHUNK_SIZE + x;
        }

        public static int GetBlockIndex(int x, int y, int z)
        {
            return (y * GameConstants.CHUNK_SIZE + z) * GameConstants.CHUNK_SIZE + x;
        }

        // ========================================
        // 网格管理
        // ========================================
        public void BuildMesh(WorldManager world)
        {
            if (Mesh == null)
            {
                Mesh = new ChunkMesh();
                WaterMesh = new ChunkMesh();
                TransparentMesh = new ChunkMesh();
            }

            MeshBuilder builder = new MeshBuilder(world, this);
            builder.BuildMesh();

            IsMeshBuilt = true;
            IsDirty = false;
        }

        public void DisposeMesh()
        {
            Mesh?.Dispose();
            WaterMesh?.Dispose();
            TransparentMesh?.Dispose();
            Mesh = null;
            WaterMesh = null;
            TransparentMesh = null;
            IsMeshBuilt = false;
        }

        // ========================================
        // 光照计算
        // ========================================
        public void CalculateInitialLighting()
        {
            // 初始化天空光照
            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                {
                    bool hitBlock = false;
                    for (int y = GameConstants.CHUNK_HEIGHT - 1; y >= 0; y--)
                    {
                        ushort block = GetBlock(x, y, z);
                        BlockInfo info = BlockRegistry.GetBlockInfo(block);

                        if (block != GameConstants.BLOCK_AIR && info != null && info.IsOpaque)
                        {
                            hitBlock = true;
                        }

                        if (hitBlock)
                        {
                            SetSkyLight(x, y, z, 0);
                        }
                        else
                        {
                            SetSkyLight(x, y, z, 15);
                        }
                    }
                }
            }

            // 初始化方块光照
            for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < GameConstants.CHUNK_HEIGHT; y++)
                {
                    for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                    {
                        ushort block = GetBlock(x, y, z);
                        BlockInfo info = BlockRegistry.GetBlockInfo(block);

                        if (info != null && info.LightEmission > 0)
                        {
                            SetBlockLight(x, y, z, info.LightEmission);
                        }
                    }
                }
            }

            IsLightCalculated = true;
        }

        // ========================================
        // 方块查询
        // ========================================
        public bool IsSolid(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            if (block == GameConstants.BLOCK_AIR) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(block);
            return info != null && info.IsSolid;
        }

        public bool IsOpaque(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            if (block == GameConstants.BLOCK_AIR) return false;

            BlockInfo info = BlockRegistry.GetBlockInfo(block);
            return info != null && info.IsOpaque;
        }

        public bool IsTransparent(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            if (block == GameConstants.BLOCK_AIR) return true;

            BlockInfo info = BlockRegistry.GetBlockInfo(block);
            return info != null && info.IsTransparent;
        }

        public bool IsLiquid(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            return block == GameConstants.BLOCK_WATER_STILL ||
                   block == GameConstants.BLOCK_WATER_FLOWING ||
                   block == GameConstants.BLOCK_LAVA_STILL ||
                   block == GameConstants.BLOCK_LAVA_FLOWING;
        }

        public bool IsWater(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            return block == GameConstants.BLOCK_WATER_STILL ||
                   block == GameConstants.BLOCK_WATER_FLOWING;
        }

        public bool IsLava(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            return block == GameConstants.BLOCK_LAVA_STILL ||
                   block == GameConstants.BLOCK_LAVA_FLOWING;
        }

        // ========================================
        // 最高方块
        // ========================================
        public int GetHeight(int x, int z)
        {
            for (int y = GameConstants.CHUNK_HEIGHT - 1; y >= 0; y--)
            {
                if (GetBlock(x, y, z) != GameConstants.BLOCK_AIR)
                {
                    return y;
                }
            }
            return 0;
        }

        public int GetHeightIgnoreLeaves(int x, int z)
        {
            for (int y = GameConstants.CHUNK_HEIGHT - 1; y >= 0; y--)
            {
                ushort block = GetBlock(x, y, z);
                if (block != GameConstants.BLOCK_AIR &&
                    block != GameConstants.BLOCK_LEAVES &&
                    block != GameConstants.BLOCK_TALL_GRASS)
                {
                    return y;
                }
            }
            return 0;
        }

        // ========================================
        // 实体管理
        // ========================================
        public void AddEntity(Entities.Entity entity)
        {
            Entities.Add(entity);
            HasEntities = true;
        }

        public void RemoveEntity(Entities.Entity entity)
        {
            Entities.Remove(entity);
            if (Entities.Count == 0)
            {
                HasEntities = false;
            }
        }

        // ========================================
        // 数据序列化
        // ========================================
        public byte[] Serialize()
        {
            int blockDataSize = blocks.Length * 2;
            int lightDataSize = skyLight.Length * 2;
            int metadataSize = metadata.Length;
            int totalSize = 4 + 4 + blockDataSize + lightDataSize + metadataSize;

            byte[] data = new byte[totalSize];
            int offset = 0;

            // 写入坐标
            BitConverter.GetBytes(X).CopyTo(data, offset);
            offset += 4;
            BitConverter.GetBytes(Z).CopyTo(data, offset);
            offset += 4;

            // 写入方块数据
            for (int i = 0; i < blocks.Length; i++)
            {
                BitConverter.GetBytes(blocks[i]).CopyTo(data, offset);
                offset += 2;
            }

            // 写入光照数据
            for (int i = 0; i < skyLight.Length; i++)
            {
                data[offset++] = skyLight[i];
                data[offset++] = blockLight[i];
            }

            // 写入元数据
            Array.Copy(metadata, 0, data, offset, metadata.Length);

            return data;
        }

        public void Deserialize(byte[] data)
        {
            int offset = 0;

            X = BitConverter.ToInt32(data, offset);
            offset += 4;
            Z = BitConverter.ToInt32(data, offset);
            offset += 4;

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = BitConverter.ToUInt16(data, offset);
                offset += 2;
            }

            for (int i = 0; i < skyLight.Length; i++)
            {
                skyLight[i] = data[offset++];
                blockLight[i] = data[offset++];
            }

            Array.Copy(data, offset, metadata, 0, metadata.Length);

            IsGenerated = true;
            IsLoaded = true;
            IsLightCalculated = true;
        }

        // ========================================
        // 清空
        // ========================================
        public void Clear()
        {
            Array.Clear(blocks, 0, blocks.Length);
            Array.Clear(skyLight, 0, skyLight.Length);
            Array.Clear(blockLight, 0, blockLight.Length);
            Array.Clear(metadata, 0, metadata.Length);
            SolidBlockCount = 0;
            AirBlockCount = blocks.Length;
            IsDirty = true;
            IsGenerated = false;
            IsMeshBuilt = false;
            DisposeMesh();
        }

        // ========================================
        // 统计
        // ========================================
        public void RecalculateBlockCounts()
        {
            SolidBlockCount = 0;
            AirBlockCount = 0;

            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == GameConstants.BLOCK_AIR)
                {
                    AirBlockCount++;
                }
                else
                {
                    SolidBlockCount++;
                }
            }
        }

        public float GetFillPercentage()
        {
            return (float)SolidBlockCount / blocks.Length * 100f;
        }

        // ========================================
        // 位置转换
        // ========================================
        public Vector3i WorldToLocal(Vector3i worldPos)
        {
            return new Vector3i(
                worldPos.X - X * GameConstants.CHUNK_SIZE,
                worldPos.Y,
                worldPos.Z - Z * GameConstants.CHUNK_SIZE
            );
        }

        public Vector3i LocalToWorld(Vector3i localPos)
        {
            return new Vector3i(
                localPos.X + X * GameConstants.CHUNK_SIZE,
                localPos.Y,
                localPos.Z + Z * GameConstants.CHUNK_SIZE
            );
        }

        public Vector3 WorldPosition => new Vector3(
            X * GameConstants.CHUNK_SIZE,
            0,
            Z * GameConstants.CHUNK_SIZE
        );

        public Vector3 CenterPosition => new Vector3(
            X * GameConstants.CHUNK_SIZE + GameConstants.CHUNK_SIZE / 2f,
            GameConstants.CHUNK_HEIGHT / 2f,
            Z * GameConstants.CHUNK_SIZE + GameConstants.CHUNK_SIZE / 2f
        );

        // ========================================
        // 释放
        // ========================================
        public void Dispose()
        {
            DisposeMesh();
            blocks = null;
            skyLight = null;
            blockLight = null;
            metadata = null;
            Entities.Clear();
            IsLoaded = false;
        }

        public override string ToString()
        {
            return $"Chunk[{X}, {Z}] (Blocks: {SolidBlockCount}, Generated: {IsGenerated}, Mesh: {IsMeshBuilt})";
        }
    }
}
