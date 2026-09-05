using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Render;

namespace VoxelCraft.World
{
    public class ChunkManager : IDisposable
    {
        private readonly WorldManager world;
        private readonly Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();
        private readonly Queue<Vector2i> chunksToLoad = new Queue<Vector2i>();
        private readonly Queue<Vector2i> chunksToUnload = new Queue<Vector2i>();
        private readonly Queue<Chunk> chunksToMesh = new Queue<Chunk>();
        private readonly object chunkLock = new object();

        public int LoadedChunkCount => chunks.Count;
        public int RenderedChunkCount { get; private set; }
        public int ChunksToLoad => chunksToLoad.Count;
        public int ChunksToMesh => chunksToMesh.Count;

        // 扩展属性
        public Dictionary<Vector2i, Chunk> LoadedChunks => chunks;
        public Queue<Chunk> PendingMeshBuilds => chunksToMesh;
        public int RenderedChunks => RenderedChunkCount;

        // 玩家位置
        private int playerChunkX = 0;
        private int playerChunkZ = 0;
        private int renderDistance = GameConstants.RENDER_DISTANCE;

        // 网格构建线程
        private System.Threading.Thread meshThread;
        private volatile bool isRunning = true;

        // 事件
        public event Action<Chunk> OnChunkLoaded;
        public event Action<Chunk> OnChunkUnloaded;
        public event Action<Chunk> OnChunkMeshed;

        public ChunkManager(WorldManager world)
        {
            this.world = world;
        }

        public void Initialize()
        {
            StartMeshThread();
            Console.WriteLine("[ChunkManager] 区块管理器初始化完成");
        }

        private void StartMeshThread()
        {
            isRunning = true;
            meshThread = new System.Threading.Thread(MeshBuildLoop)
            {
                Name = "MeshBuildThread",
                IsBackground = true,
                Priority = System.Threading.ThreadPriority.Normal
            };
            meshThread.Start();
        }

        private void MeshBuildLoop()
        {
            while (isRunning)
            {
                try
                {
                    if (chunksToMesh.Count > 0)
                    {
                        Chunk chunk = chunksToMesh.Dequeue();
                        if (chunk != null && chunk.IsLoaded)
                        {
                            chunk.BuildMesh(world);
                            OnChunkMeshed?.Invoke(chunk);
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MeshBuildThread] 错误: {ex.Message}");
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        // ========================================
        // 更新
        // ========================================
        public void Update(float deltaTime)
        {
            // 处理待加载的区块
            int maxLoadPerFrame = 2;
            int loaded = 0;
            while (chunksToLoad.Count > 0 && loaded < maxLoadPerFrame)
            {
                Vector2i key = chunksToLoad.Dequeue();
                LoadChunk(key.X, key.Y);
                loaded++;
            }

            // 处理待卸载的区块
            int maxUnloadPerFrame = 4;
            int unloaded = 0;
            while (chunksToUnload.Count > 0 && unloaded < maxUnloadPerFrame)
            {
                Vector2i key = chunksToUnload.Dequeue();
                UnloadChunk(key.X, key.Y);
                unloaded++;
            }

            // 标记需要重建网格的区块
            foreach (var chunk in chunks.Values)
            {
                if (chunk.IsDirty && chunk.IsGenerated && !chunksToMesh.Contains(chunk))
                {
                    chunksToMesh.Enqueue(chunk);
                }
            }
        }

        public void UpdateChunksAroundPlayer(int chunkX, int chunkZ, int distance)
        {
            playerChunkX = chunkX;
            playerChunkZ = chunkZ;
            renderDistance = distance;

            // 加载需要的区块
            for (int dx = -distance; dx <= distance; dx++)
            {
                for (int dz = -distance; dz <= distance; dz++)
                {
                    int x = chunkX + dx;
                    int z = chunkZ + dz;

                    // 圆形加载
                    if (dx * dx + dz * dz <= distance * distance)
                    {
                        if (!IsChunkLoaded(x, z))
                        {
                            RequestChunkLoad(x, z);
                        }
                    }
                }
            }

            // 卸载远处的区块
            List<Vector2i> chunksToRemove = new List<Vector2i>();
            foreach (var key in chunks.Keys)
            {
                int dx = key.X - chunkX;
                int dz = key.Y - chunkZ;
                if (dx * dx + dz * dz > (distance + 2) * (distance + 2))
                {
                    chunksToRemove.Add(key);
                }
            }

            foreach (var key in chunksToRemove)
            {
                RequestChunkUnload(key.X, key.Y);
            }
        }

        // ========================================
        // 区块加载/卸载
        // ========================================
        public void RequestChunkLoad(int x, int z)
        {
            Vector2i key = new Vector2i(x, z);
            lock (chunkLock)
            {
                if (!chunks.ContainsKey(key) && !chunksToLoad.Contains(key))
                {
                    chunksToLoad.Enqueue(key);
                }
            }
        }

        public void RequestChunkUnload(int x, int z)
        {
            Vector2i key = new Vector2i(x, z);
            lock (chunkLock)
            {
                if (!chunksToUnload.Contains(key))
                {
                    chunksToUnload.Enqueue(key);
                }
            }
        }

        private void LoadChunk(int x, int z)
        {
            if (IsChunkLoaded(x, z)) return;

            Chunk chunk = new Chunk(x, z);
            world.Generator.GenerateChunk(chunk);
            chunk.CalculateInitialLighting();

            lock (chunkLock)
            {
                chunks[new Vector2i(x, z)] = chunk;
            }

            // 设置邻居引用
            UpdateNeighborReferences(chunk);

            // 请求构建网格
            chunksToMesh.Enqueue(chunk);

            OnChunkLoaded?.Invoke(chunk);
        }

        private void UnloadChunk(int x, int z)
        {
            Vector2i key = new Vector2i(x, z);
            lock (chunkLock)
            {
                if (chunks.TryGetValue(key, out var chunk))
                {
                    // 清除邻居引用
                    ClearNeighborReferences(chunk);
                    chunk.Dispose();
                    chunks.Remove(key);
                    OnChunkUnloaded?.Invoke(chunk);
                }
            }
        }

        private void UpdateNeighborReferences(Chunk chunk)
        {
            Chunk north = GetChunk(chunk.X, chunk.Z - 1);
            Chunk south = GetChunk(chunk.X, chunk.Z + 1);
            Chunk east = GetChunk(chunk.X + 1, chunk.Z);
            Chunk west = GetChunk(chunk.X - 1, chunk.Z);

            chunk.NorthNeighbor = north;
            chunk.SouthNeighbor = south;
            chunk.EastNeighbor = east;
            chunk.WestNeighbor = west;

            if (north != null) north.SouthNeighbor = chunk;
            if (south != null) south.NorthNeighbor = chunk;
            if (east != null) east.WestNeighbor = chunk;
            if (west != null) west.EastNeighbor = chunk;
        }

        private void ClearNeighborReferences(Chunk chunk)
        {
            if (chunk.NorthNeighbor != null) chunk.NorthNeighbor.SouthNeighbor = null;
            if (chunk.SouthNeighbor != null) chunk.SouthNeighbor.NorthNeighbor = null;
            if (chunk.EastNeighbor != null) chunk.EastNeighbor.WestNeighbor = null;
            if (chunk.WestNeighbor != null) chunk.WestNeighbor.EastNeighbor = null;
        }

        // ========================================
        // 区块查询
        // ========================================
        public Chunk GetChunk(int x, int z)
        {
            Vector2i key = new Vector2i(x, z);
            lock (chunkLock)
            {
                chunks.TryGetValue(key, out var chunk);
                return chunk;
            }
        }

        public Chunk GetChunkAtBlock(int blockX, int blockZ)
        {
            int chunkX = MathF.FloorDiv(blockX, GameConstants.CHUNK_SIZE);
            int chunkZ = MathF.FloorDiv(blockZ, GameConstants.CHUNK_SIZE);
            return GetChunk(chunkX, chunkZ);
        }

        public bool IsChunkLoaded(int x, int z)
        {
            Vector2i key = new Vector2i(x, z);
            lock (chunkLock)
            {
                return chunks.ContainsKey(key);
            }
        }

        public bool IsChunkGenerated(int x, int z)
        {
            Chunk chunk = GetChunk(x, z);
            return chunk != null && chunk.IsGenerated;
        }

        public bool IsChunkMeshed(int x, int z)
        {
            Chunk chunk = GetChunk(x, z);
            return chunk != null && chunk.IsMeshBuilt;
        }

        // ========================================
        // 渲染
        // ========================================
        public void Render(Camera camera)
        {
            RenderedChunkCount = 0;

            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    if (!chunk.IsMeshBuilt || chunk.Mesh == null) continue;

                    // 视锥体剔除
                    Vector3 min = chunk.WorldPosition;
                    Vector3 max = min + new Vector3(
                        GameConstants.CHUNK_SIZE,
                        GameConstants.CHUNK_HEIGHT,
                        GameConstants.CHUNK_SIZE
                    );

                    if (!camera.IsBoxInFrustum(min, max)) continue;

                    // 渲染实体方块
                    if (chunk.Mesh.HasVertices)
                    {
                        chunk.Mesh.Render();
                        RenderedChunkCount++;
                    }

                    // 渲染水
                    if (chunk.WaterMesh != null && chunk.WaterMesh.HasVertices)
                    {
                        chunk.WaterMesh.Render();
                    }

                    // 渲染透明方块
                    if (chunk.TransparentMesh != null && chunk.TransparentMesh.HasVertices)
                    {
                        chunk.TransparentMesh.Render();
                    }
                }
            }
        }

        // ========================================
        // 强制重建所有网格
        // ========================================
        public void RebuildAllMeshes()
        {
            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    chunk.IsDirty = true;
                    if (!chunksToMesh.Contains(chunk))
                    {
                        chunksToMesh.Enqueue(chunk);
                    }
                }
            }
        }

        public void RebuildChunkMesh(int x, int z)
        {
            Chunk chunk = GetChunk(x, z);
            if (chunk != null)
            {
                chunk.IsDirty = true;
                if (!chunksToMesh.Contains(chunk))
                {
                    chunksToMesh.Enqueue(chunk);
                }
            }
        }

        // ========================================
        // 方块操作（带邻居更新）
        // ========================================
        public void SetBlock(int worldX, int worldY, int worldZ, ushort blockId)
        {
            int chunkX = MathF.FloorDiv(worldX, GameConstants.CHUNK_SIZE);
            int chunkZ = MathF.FloorDiv(worldZ, GameConstants.CHUNK_SIZE);
            int localX = MathF.Mod(worldX, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(worldZ, GameConstants.CHUNK_SIZE);

            Chunk chunk = GetChunk(chunkX, chunkZ);
            if (chunk == null) return;

            chunk.SetBlock(localX, worldY, localZ, blockId);
            chunk.IsDirty = true;

            // 如果在边界，标记邻居区块也需要重建
            if (localX == 0) RebuildChunkMesh(chunkX - 1, chunkZ);
            if (localX == GameConstants.CHUNK_SIZE - 1) RebuildChunkMesh(chunkX + 1, chunkZ);
            if (localZ == 0) RebuildChunkMesh(chunkX, chunkZ - 1);
            if (localZ == GameConstants.CHUNK_SIZE - 1) RebuildChunkMesh(chunkX, chunkZ + 1);
        }

        public ushort GetBlock(int worldX, int worldY, int worldZ)
        {
            if (worldY < 0 || worldY >= GameConstants.CHUNK_HEIGHT)
            {
                return GameConstants.BLOCK_AIR;
            }

            Chunk chunk = GetChunkAtBlock(worldX, worldZ);
            if (chunk == null) return GameConstants.BLOCK_AIR;

            int localX = MathF.Mod(worldX, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(worldZ, GameConstants.CHUNK_SIZE);
            return chunk.GetBlock(localX, worldY, localZ);
        }

        // ========================================
        // 统计
        // ========================================
        public int GetTotalBlocks()
        {
            int total = 0;
            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    total += chunk.SolidBlockCount;
                }
            }
            return total;
        }

        public float GetAverageFillPercentage()
        {
            if (chunks.Count == 0) return 0;

            float total = 0;
            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    total += chunk.GetFillPercentage();
                }
            }
            return total / chunks.Count;
        }

        public List<Chunk> GetLoadedChunks()
        {
            lock (chunkLock)
            {
                return new List<Chunk>(chunks.Values);
            }
        }

        // ========================================
        // 清理
        // ========================================
        public void ClearAllChunks()
        {
            isRunning = false;
            meshThread?.Join(1000);

            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    chunk.Dispose();
                }
                chunks.Clear();
                chunksToLoad.Clear();
                chunksToUnload.Clear();
                chunksToMesh.Clear();
            }
        }

        public void Dispose()
        {
            ClearAllChunks();
            Console.WriteLine("[ChunkManager] 区块管理器已释放");
        }
    }
}
