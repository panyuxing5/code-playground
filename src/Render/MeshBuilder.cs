using System;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.Render
{
    public class MeshBuilder
    {
        private readonly Queue<Chunk> meshBuildQueue;
        private readonly object queueLock = new object();
        private Thread meshBuildThread;
        private bool isRunning;
        private readonly WorldManager world;
        private readonly int maxThreads;

        public int QueuedCount
        {
            get
            {
                lock (queueLock)
                {
                    return meshBuildQueue.Count;
                }
            }
        }

        public MeshBuilder(WorldManager world, int maxThreads = 2)
        {
            this.world = world;
            this.maxThreads = maxThreads;
            meshBuildQueue = new Queue<Chunk>();
        }

        public void Start()
        {
            isRunning = true;
            meshBuildThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "MeshBuilderThread"
            };
            meshBuildThread.Start();
            Console.WriteLine("[MeshBuilder] 网格构建线程已启动");
        }

        public void Stop()
        {
            isRunning = false;
            lock (queueLock)
            {
                Monitor.PulseAll(queueLock);
            }
            meshBuildThread?.Join(1000);
            Console.WriteLine("[MeshBuilder] 网格构建线程已停止");
        }

        public void QueueChunkMeshBuild(Chunk chunk)
        {
            if (chunk == null) return;
            if (chunk.Mesh != null && chunk.Mesh.IsBuilt) return;

            lock (queueLock)
            {
                if (!meshBuildQueue.Contains(chunk))
                {
                    meshBuildQueue.Enqueue(chunk);
                    Monitor.Pulse(queueLock);
                }
            }
        }

        public void QueueChunkMeshBuild(IEnumerable<Chunk> chunks)
        {
            lock (queueLock)
            {
                foreach (Chunk chunk in chunks)
                {
                    if (chunk != null && !meshBuildQueue.Contains(chunk))
                    {
                        meshBuildQueue.Enqueue(chunk);
                    }
                }
                Monitor.Pulse(queueLock);
            }
        }

        public void ClearQueue()
        {
            lock (queueLock)
            {
                meshBuildQueue.Clear();
            }
        }

        private void ProcessQueue()
        {
            while (isRunning)
            {
                Chunk chunk = null;

                lock (queueLock)
                {
                    while (meshBuildQueue.Count == 0 && isRunning)
                    {
                        Monitor.Wait(queueLock);
                    }

                    if (!isRunning) break;

                    if (meshBuildQueue.Count > 0)
                    {
                        chunk = meshBuildQueue.Dequeue();
                    }
                }

                if (chunk != null)
                {
                    try
                    {
                        BuildChunkMesh(chunk);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[MeshBuilder] 构建网格失败: {ex.Message}");
                    }
                }
            }
        }

        private void BuildChunkMesh(Chunk chunk)
        {
            if (chunk.Mesh == null)
            {
                chunk.Mesh = new ChunkMesh(chunk);
            }

            chunk.Mesh.BuildMesh(world);
            chunk.IsMeshDirty = false;
        }

        public void RebuildChunkMesh(Chunk chunk)
        {
            if (chunk == null) return;

            chunk.Mesh?.Dispose();
            chunk.Mesh = null;
            chunk.IsMeshDirty = true;

            QueueChunkMeshBuild(chunk);
        }

        public void RebuildNeighborMeshes(int worldX, int worldY, int worldZ)
        {
            int chunkX = worldX >> 4;
            int chunkZ = worldZ >> 4;
            int localX = worldX & 15;
            int localZ = worldZ & 15;

            // 重建当前区块
            Chunk currentChunk = world.GetChunk(chunkX, chunkZ);
            if (currentChunk != null)
            {
                RebuildChunkMesh(currentChunk);
            }

            // 如果方块在区块边缘，重建相邻区块
            if (localX == 0)
            {
                Chunk neighbor = world.GetChunk(chunkX - 1, chunkZ);
                if (neighbor != null) RebuildChunkMesh(neighbor);
            }
            if (localX == 15)
            {
                Chunk neighbor = world.GetChunk(chunkX + 1, chunkZ);
                if (neighbor != null) RebuildChunkMesh(neighbor);
            }
            if (localZ == 0)
            {
                Chunk neighbor = world.GetChunk(chunkX, chunkZ - 1);
                if (neighbor != null) RebuildChunkMesh(neighbor);
            }
            if (localZ == 15)
            {
                Chunk neighbor = world.GetChunk(chunkX, chunkZ + 1);
                if (neighbor != null) RebuildChunkMesh(neighbor);
            }
        }

        public void Update()
        {
            // 可以在这里处理主线程的网格上传
            // 目前所有工作都在后台线程完成
        }
    }
}
