using System;
using System.Collections.Generic;
using System.Threading;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class WorldManager : IDisposable
    {
        public long Seed { get; private set; }
        public string WorldName { get; private set; }
        public WorldGenerator Generator { get; private set; }
        public NoiseGenerator Noise { get; private set; }

        private readonly Dictionary<Vector2i, Chunk> chunks = new Dictionary<Vector2i, Chunk>();
        private readonly Queue<Vector2i> chunksToGenerate = new Queue<Vector2i>();
        private readonly Queue<Vector2i> chunksToUnload = new Queue<Vector2i>();
        private readonly object chunkLock = new object();

        public int LoadedChunkCount => chunks.Count;
        public int PendingGenerations => chunksToGenerate.Count;

        // 世界时间和天气
        public long Time { get; set; } = 0;
        public WeatherType CurrentWeather { get; set; } = WeatherType.Clear;
        public float WeatherIntensity { get; set; } = 0f;
        public float RainLevel { get; set; } = 0f;
        public float ThunderLevel { get; set; } = 0f;

        // 世界边界
        public int WorldBorder { get; set; } = 29999984;
        public Vector2 WorldBorderCenter { get; set; } = Vector2.Zero;

        // 游戏规则
        public GameRules Rules { get; private set; } = new GameRules();

        // 实体
        public List<Entities.Entity> Entities { get; private set; } = new List<Entities.Entity>();
        public List<Entities.ItemEntity> ItemEntities { get; private set; } = new List<Entities.ItemEntity>();
        public List<Entities.ExperienceOrb> ExperienceOrbs { get; private set; } = new List<Entities.ExperienceOrb>();

        // 方块事件
        public event Action<Vector3i, ushort, ushort> OnBlockChanged;
        public event Action<Vector3i> OnBlockExploded;
        public event Action<Vector3i, int> OnBlockBroken;
        public event Action<Vector3i, ushort> OnBlockPlaced;

        public WorldManager(long seed)
        {
            Seed = seed;
            Noise = new NoiseGenerator(seed);
            Generator = new WorldGenerator(seed, this);
            Console.WriteLine($"[WorldManager] 世界管理器初始化，种子: {seed}");
        }

        public void Initialize()
        {
            Generator.Initialize();
            Console.WriteLine("[WorldManager] 世界管理器初始化完成");
        }

        public void Regenerate(long newSeed)
        {
            Seed = newSeed;
            Noise = new NoiseGenerator(newSeed);
            Generator = new WorldGenerator(newSeed, this);
            Generator.Initialize();

            lock (chunkLock)
            {
                chunks.Clear();
                chunksToGenerate.Clear();
                chunksToUnload.Clear();
            }

            Console.WriteLine($"[WorldManager] 世界重新生成，新种子: {newSeed}");
        }

        // ========================================
        // 区块管理
        // ========================================
        public Chunk GetChunk(int chunkX, int chunkZ)
        {
            Vector2i key = new Vector2i(chunkX, chunkZ);
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

        public void AddChunk(Chunk chunk)
        {
            lock (chunkLock)
            {
                Vector2i key = new Vector2i(chunk.X, chunk.Z);
                chunks[key] = chunk;
            }
        }

        public void RemoveChunk(int chunkX, int chunkZ)
        {
            Vector2i key = new Vector2i(chunkX, chunkZ);
            lock (chunkLock)
            {
                if (chunks.TryGetValue(key, out var chunk))
                {
                    chunk.Dispose();
                    chunks.Remove(key);
                }
            }
        }

        public bool IsChunkLoaded(int chunkX, int chunkZ)
        {
            Vector2i key = new Vector2i(chunkX, chunkZ);
            lock (chunkLock)
            {
                return chunks.ContainsKey(key);
            }
        }

        public void RequestChunkGeneration(int chunkX, int chunkZ)
        {
            Vector2i key = new Vector2i(chunkX, chunkZ);
            lock (chunkLock)
            {
                if (!chunks.ContainsKey(key) && !chunksToGenerate.Contains(key))
                {
                    chunksToGenerate.Enqueue(key);
                }
            }
        }

        public void RequestChunkUnload(int chunkX, int chunkZ)
        {
            Vector2i key = new Vector2i(chunkX, chunkZ);
            lock (chunkLock)
            {
                if (!chunksToUnload.Contains(key))
                {
                    chunksToUnload.Enqueue(key);
                }
            }
        }

        public void ProcessPendingGenerations()
        {
            int maxPerFrame = 4;
            int processed = 0;

            while (chunksToGenerate.Count > 0 && processed < maxPerFrame)
            {
                Vector2i key = chunksToGenerate.Dequeue();
                GenerateChunk(key.X, key.Y);
                processed++;
            }

            while (chunksToUnload.Count > 0)
            {
                Vector2i key = chunksToUnload.Dequeue();
                RemoveChunk(key.X, key.Y);
            }
        }

        private void GenerateChunk(int chunkX, int chunkZ)
        {
            if (IsChunkLoaded(chunkX, chunkZ)) return;

            Chunk chunk = new Chunk(chunkX, chunkZ);
            Generator.GenerateChunk(chunk);
            AddChunk(chunk);
        }

        // ========================================
        // 方块操作
        // ========================================
        public ushort GetBlock(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT)
            {
                return GameConstants.BLOCK_AIR;
            }

            int chunkX = MathF.FloorDiv(x, GameConstants.CHUNK_SIZE);
            int chunkZ = MathF.FloorDiv(z, GameConstants.CHUNK_SIZE);
            Chunk chunk = GetChunk(chunkX, chunkZ);

            if (chunk == null)
            {
                // 未加载的区块返回石头（防止透视）
                return GameConstants.BLOCK_STONE;
            }

            int localX = x - chunkX * GameConstants.CHUNK_SIZE;
            int localZ = z - chunkZ * GameConstants.CHUNK_SIZE;
            return chunk.GetBlock(localX, y, localZ);
        }

        public ushort GetBlock(Vector3i pos)
        {
            return GetBlock(pos.X, pos.Y, pos.Z);
        }

        public void SetBlock(int x, int y, int z, ushort blockId, bool update = true)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return;

            int chunkX = MathF.FloorDiv(x, GameConstants.CHUNK_SIZE);
            int chunkZ = MathF.FloorDiv(z, GameConstants.CHUNK_SIZE);
            Chunk chunk = GetChunk(chunkX, chunkZ);

            if (chunk == null) return;

            int localX = x - chunkX * GameConstants.CHUNK_SIZE;
            int localZ = z - chunkZ * GameConstants.CHUNK_SIZE;
            ushort oldBlock = chunk.GetBlock(localX, y, localZ);

            if (oldBlock == blockId) return;

            chunk.SetBlock(localX, y, localZ, blockId);
            chunk.IsDirty = true;

            OnBlockChanged?.Invoke(new Vector3i(x, y, z), oldBlock, blockId);

            if (update)
            {
                UpdateNeighbors(x, y, z);
            }
        }

        public void SetBlock(Vector3i pos, ushort blockId, bool update = true)
        {
            SetBlock(pos.X, pos.Y, pos.Z, blockId, update);
        }

        public bool BreakBlock(int x, int y, int z)
        {
            ushort block = GetBlock(x, y, z);
            if (block == GameConstants.BLOCK_AIR || block == GameConstants.BLOCK_BEDROCK)
            {
                return false;
            }

            SetBlock(x, y, z, GameConstants.BLOCK_AIR);
            OnBlockBroken?.Invoke(new Vector3i(x, y, z), block);

            // 掉落物品
            BlockInfo info = BlockRegistry.GetBlockInfo(block);
            if (info != null && info.DropItem != GameConstants.ITEM_AIR)
            {
                SpawnItemEntity(x + 0.5f, y + 0.5f, z + 0.5f, info.DropItem, 1);
            }

            return true;
        }

        public bool PlaceBlock(int x, int y, int z, ushort blockId)
        {
            if (GetBlock(x, y, z) != GameConstants.BLOCK_AIR)
            {
                return false;
            }

            SetBlock(x, y, z, blockId);
            OnBlockPlaced?.Invoke(new Vector3i(x, y, z), blockId);
            return true;
        }

        private void UpdateNeighbors(int x, int y, int z)
        {
            // 更新相邻区块的mesh
            int chunkX = MathF.FloorDiv(x, GameConstants.CHUNK_SIZE);
            int chunkZ = MathF.FloorDiv(z, GameConstants.CHUNK_SIZE);
            int localX = x - chunkX * GameConstants.CHUNK_SIZE;
            int localZ = z - chunkZ * GameConstants.CHUNK_SIZE;

            if (localX == 0) MarkChunkDirty(chunkX - 1, chunkZ);
            if (localX == GameConstants.CHUNK_SIZE - 1) MarkChunkDirty(chunkX + 1, chunkZ);
            if (localZ == 0) MarkChunkDirty(chunkX, chunkZ - 1);
            if (localZ == GameConstants.CHUNK_SIZE - 1) MarkChunkDirty(chunkX, chunkZ + 1);
        }

        public void MarkChunkDirty(int chunkX, int chunkZ)
        {
            Chunk chunk = GetChunk(chunkX, chunkZ);
            if (chunk != null)
            {
                chunk.IsDirty = true;
            }
        }

        // ========================================
        // 光照
        // ========================================
        public byte GetSkyLight(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return 0;

            Chunk chunk = GetChunkAtBlock(x, z);
            if (chunk == null) return 15;

            int localX = MathF.Mod(x, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(z, GameConstants.CHUNK_SIZE);
            return chunk.GetSkyLight(localX, y, localZ);
        }

        public byte GetBlockLight(int x, int y, int z)
        {
            if (y < 0 || y >= GameConstants.WORLD_HEIGHT) return 0;

            Chunk chunk = GetChunkAtBlock(x, z);
            if (chunk == null) return 0;

            int localX = MathF.Mod(x, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(z, GameConstants.CHUNK_SIZE);
            return chunk.GetBlockLight(localX, y, localZ);
        }

        public void SetSkyLight(int x, int y, int z, byte value)
        {
            Chunk chunk = GetChunkAtBlock(x, z);
            if (chunk == null) return;

            int localX = MathF.Mod(x, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(z, GameConstants.CHUNK_SIZE);
            chunk.SetSkyLight(localX, y, localZ, value);
        }

        public void SetBlockLight(int x, int y, int z, byte value)
        {
            Chunk chunk = GetChunkAtBlock(x, z);
            if (chunk == null) return;

            int localX = MathF.Mod(x, GameConstants.CHUNK_SIZE);
            int localZ = MathF.Mod(z, GameConstants.CHUNK_SIZE);
            chunk.SetBlockLight(localX, y, localZ, value);
        }

        // ========================================
        // 实体管理
        // ========================================
        public void SpawnEntity(Entities.Entity entity)
        {
            Entities.Add(entity);
        }

        public void SpawnItemEntity(float x, float y, float z, int itemId, int count)
        {
            Entities.ItemEntity item = new Entities.ItemEntity(itemId, count, new Vector3(x, y, z));
            ItemEntities.Add(item);
        }

        public void SpawnExperienceOrb(float x, float y, float z, int amount)
        {
            Entities.ExperienceOrb orb = new Entities.ExperienceOrb(amount, new Vector3(x, y, z));
            ExperienceOrbs.Add(orb);
        }

        public void RemoveEntity(Entities.Entity entity)
        {
            Entities.Remove(entity);
        }

        public List<Entities.Entity> GetEntitiesNear(Vector3 position, float radius)
        {
            List<Entities.Entity> result = new List<Entities.Entity>();
            foreach (var entity in Entities)
            {
                if (Vector3.Distance(entity.Position, position) < radius)
                {
                    result.Add(entity);
                }
            }
            return result;
        }

        // ========================================
        // 爆炸
        // ========================================
        public void CreateExplosion(Vector3 center, float radius, bool causeFire = false)
        {
            int minX = (int)Math.Floor(center.X - radius);
            int maxX = (int)Math.Ceiling(center.X + radius);
            int minY = (int)Math.Floor(center.Y - radius);
            int maxY = (int)Math.Ceiling(center.Y + radius);
            int minZ = (int)Math.Floor(center.Z - radius);
            int maxZ = (int)Math.Ceiling(center.Z + radius);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        float dist = Vector3.Distance(center, new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                        if (dist < radius)
                        {
                            ushort block = GetBlock(x, y, z);
                            if (block != GameConstants.BLOCK_AIR && block != GameConstants.BLOCK_BEDROCK)
                            {
                                BlockInfo info = BlockRegistry.GetBlockInfo(block);
                                if (info != null && info.Resistance < radius * radius)
                                {
                                    SetBlock(x, y, z, GameConstants.BLOCK_AIR);
                                    OnBlockExploded?.Invoke(new Vector3i(x, y, z));
                                }
                            }
                        }
                    }
                }
            }

            // 对实体造成伤害
            foreach (var entity in GetEntitiesNear(center, radius * 1.5f))
            {
                float dist = Vector3.Distance(entity.Position, center);
                float damage = (1.0f - dist / (radius * 1.5f)) * radius * 8;
                entity.TakeDamage(damage, DamageSource.Explosion);
            }
        }

        // ========================================
        // 天气
        // ========================================
        public void SetWeather(WeatherType type, float intensity = 1.0f)
        {
            CurrentWeather = type;
            WeatherIntensity = intensity;

            switch (type)
            {
                case WeatherType.Clear:
                    RainLevel = 0;
                    ThunderLevel = 0;
                    break;
                case WeatherType.Rain:
                    RainLevel = intensity;
                    ThunderLevel = 0;
                    break;
                case WeatherType.Thunder:
                    RainLevel = intensity;
                    ThunderLevel = intensity;
                    break;
                case WeatherType.Snow:
                    RainLevel = intensity;
                    ThunderLevel = 0;
                    break;
            }
        }

        public void UpdateWeather(float deltaTime)
        {
            // 天气变化逻辑
            if (CurrentWeather != WeatherType.Clear)
            {
                WeatherIntensity -= deltaTime * 0.001f;
                if (WeatherIntensity <= 0)
                {
                    SetWeather(WeatherType.Clear);
                }
            }
        }

        // ========================================
        // 高度图
        // ========================================
        public int GetHeight(int x, int z)
        {
            for (int y = GameConstants.WORLD_HEIGHT - 1; y >= 0; y--)
            {
                if (GetBlock(x, y, z) != GameConstants.BLOCK_AIR)
                {
                    return y;
                }
            }
            return 0;
        }

        public int GetSeaLevel()
        {
            return GameConstants.SEA_LEVEL;
        }

        // ========================================
        // 生物群系
        // ========================================
        public BiomeType GetBiome(int x, int z)
        {
            return Generator.GetBiome(x, z);
        }

        public float GetTemperature(int x, int z)
        {
            return Generator.GetTemperature(x, z);
        }

        public float GetHumidity(int x, int z)
        {
            return Generator.GetHumidity(x, z);
        }

        // ========================================
        // 清理
        // ========================================
        public void ClearAllChunks()
        {
            lock (chunkLock)
            {
                foreach (var chunk in chunks.Values)
                {
                    chunk.Dispose();
                }
                chunks.Clear();
                chunksToGenerate.Clear();
                chunksToUnload.Clear();
            }
        }

        public void Dispose()
        {
            ClearAllChunks();
            Entities.Clear();
            ItemEntities.Clear();
            ExperienceOrbs.Clear();
            Console.WriteLine("[WorldManager] 世界管理器已释放");
        }
    }

    // ========================================
    // 天气类型
    // ========================================
    public enum WeatherType
    {
        Clear,
        Rain,
        Thunder,
        Snow
    }

    // ========================================
    // 伤害来源
    // ========================================
    public enum DamageSource
    {
        Generic,
        Player,
        Mob,
        Fall,
        Drowning,
        Suffocation,
        Fire,
        Lava,
        Explosion,
        Void,
        Starvation,
        Poison,
        Wither,
        Magic,
        Cactus,
        SweetBerryBush,
        FallingBlock,
        Anvil,
        Lightning,
        Cramming
    }

    // ========================================
    // 游戏规则
    // ========================================
    public class GameRules
    {
        public bool DoDaylightCycle = true;
        public bool DoWeatherCycle = true;
        public bool DoMobSpawning = true;
        public bool DoMobLoot = true;
        public bool DoTileDrops = true;
        public bool DoEntityDrops = true;
        public bool KeepInventory = false;
        public bool MobGriefing = true;
        public bool NaturalRegeneration = true;
        public bool PvP = true;
        public bool ShowDeathMessages = true;
        public bool DoFireTick = true;
        public bool DoInsomnia = true;
        public bool DrowningDamage = true;
        public bool FallDamage = true;
        public bool FireDamage = true;
        public bool FreezeDamage = true;
        public bool UniversalAnger = false;
        public int RandomTickSpeed = 3;
        public int MaxEntityCramming = 24;
        public int SpawnRadius = 10;
        public string GameMode = "survival";
        public string Difficulty = "normal";
    }

    // ========================================
    // 数学工具扩展
    // ========================================
    public static class MathF
    {
        public static int FloorDiv(int a, int b)
        {
            return (int)Math.Floor((double)a / b);
        }

        public static int Mod(int a, int b)
        {
            return ((a % b) + b) % b;
        }
    }
}
