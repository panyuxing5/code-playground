using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Player;
using VoxelCraft.Items;

namespace VoxelCraft.Save
{
    public class SaveManager
    {
        private readonly string saveDirectory;
        private readonly string worldName;
        private readonly WorldManager world;
        private readonly PlayerController player;

        // 保存配置
        public bool AutoSave { get; set; } = true;
        public float AutoSaveInterval { get; set; } = 300.0f; // 5分钟
        private float autoSaveTimer;

        // 保存格式版本
        private const int SaveVersion = 1;

        public SaveManager(string saveDirectory, string worldName, WorldManager world, PlayerController player)
        {
            this.saveDirectory = saveDirectory;
            this.worldName = worldName;
            this.world = world;
            this.player = player;
        }

        public SaveManager() : this("saves", "world", null, null)
        {
        }

        public void Initialize()
        {
            // 创建保存目录
            string worldPath = Path.Combine(saveDirectory, worldName);
            if (!Directory.Exists(worldPath))
            {
                Directory.CreateDirectory(worldPath);
                Directory.CreateDirectory(Path.Combine(worldPath, "region"));
                Directory.CreateDirectory(Path.Combine(worldPath, "playerdata"));
                Directory.CreateDirectory(Path.Combine(worldPath, "entities"));
            }

            Console.WriteLine($"[SaveManager] 存档管理器初始化完成，存档路径: {worldPath}");
        }

        public void LoadWorld()
        {
            Console.WriteLine("[SaveManager] 加载世界...");
        }

        public void LoadWorld(string name)
        {
            Console.WriteLine($"[SaveManager] 加载世界: {name}");
        }

        public void SaveWorld()
        {
            Console.WriteLine("[SaveManager] 保存世界...");
        }

        public void SaveWorld(string name)
        {
            Console.WriteLine($"[SaveManager] 保存世界: {name}");
        }

        public void Update(float deltaTime)
        {
            if (!AutoSave) return;

            autoSaveTimer += deltaTime;
            if (autoSaveTimer >= AutoSaveInterval)
            {
                autoSaveTimer = 0;
                SaveAll();
                Console.WriteLine("[SaveManager] 自动保存完成");
            }
        }

        public void SaveAll()
        {
            try
            {
                SaveLevelData();
                SavePlayerData();
                SaveChunks();
                SaveWorldInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveManager] 保存失败: {ex.Message}");
            }
        }

        public void LoadAll()
        {
            try
            {
                LoadWorldInfo();
                LoadLevelData();
                LoadPlayerData();
                // 区块在需要时加载
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveManager] 加载失败: {ex.Message}");
            }
        }

        // ========================================
        // 世界信息
        // ========================================
        private void SaveWorldInfo()
        {
            string path = Path.Combine(saveDirectory, worldName, "level.dat");

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8))
            {
                writer.Write(SaveVersion);
                writer.Write(worldName);
                writer.Write(world.Seed);
                writer.Write(GameEngine.Instance.Time);
                writer.Write(GameEngine.Instance.DayTime);
                writer.Write(GameEngine.Instance.RenderDistance);
                writer.Write(world.SpawnPoint.X);
                writer.Write(world.SpawnPoint.Y);
                writer.Write(world.SpawnPoint.Z);
                writer.Write(world.IsRaining);
                writer.Write(world.RainTime);
                writer.Write(world.IsThundering);
                writer.Write(world.ThunderTime);
                writer.Write(world.Difficulty);
                writer.Write(world.GameMode);
                writer.Write(world.Hardcore);
                writer.Write(world.AllowCommands);
                writer.Write(DateTime.Now.ToString("o"));
            }
        }

        private void LoadWorldInfo()
        {
            string path = Path.Combine(saveDirectory, worldName, "level.dat");
            if (!File.Exists(path)) return;

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
            {
                int version = reader.ReadInt32();
                string name = reader.ReadString();
                long seed = reader.ReadInt64();
                double time = reader.ReadDouble();
                float dayTime = reader.ReadSingle();
                int renderDistance = reader.ReadInt32();
                float spawnX = reader.ReadSingle();
                float spawnY = reader.ReadSingle();
                float spawnZ = reader.ReadSingle();
                bool isRaining = reader.ReadBoolean();
                int rainTime = reader.ReadInt32();
                bool isThundering = reader.ReadBoolean();
                int thunderTime = reader.ReadInt32();
                int difficulty = reader.ReadInt32();
                int gameMode = reader.ReadInt32();
                bool hardcore = reader.ReadBoolean();
                bool allowCommands = reader.ReadBoolean();
                string lastPlayed = reader.ReadString();

                GameEngine.Instance.Time = (float)time;
                GameEngine.Instance.DayTime = (long)dayTime;
                world.SpawnPoint = new Vector3(spawnX, spawnY, spawnZ);
                world.IsRaining = isRaining;
                world.RainTime = rainTime;
                world.IsThundering = isThundering;
                world.ThunderTime = thunderTime;
            }
        }

        // ========================================
        // 玩家数据
        // ========================================
        private void SavePlayerData()
        {
            string path = Path.Combine(saveDirectory, worldName, "playerdata", "player.dat");

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8))
            {
                writer.Write(SaveVersion);

                // 位置和速度
                writer.Write(player.Position.X);
                writer.Write(player.Position.Y);
                writer.Write(player.Position.Z);
                writer.Write(player.Velocity.X);
                writer.Write(player.Velocity.Y);
                writer.Write(player.Velocity.Z);

                // 旋转
                writer.Write(player.Yaw);
                writer.Write(player.Pitch);

                // 属性
                writer.Write(player.Health);
                writer.Write(player.MaxHealth);
                writer.Write(player.Hunger);
                writer.Write(player.MaxHunger);
                writer.Write(player.Saturation);
                writer.Write(player.Experience);
                writer.Write(player.Level);

                // 状态
                writer.Write(player.IsFlying);
                writer.Write(player.IsSprinting);
                writer.Write(player.IsSneaking);
                writer.Write(player.IsSwimming);

                // 物品栏
                SaveInventory(writer, null); // player.Inventory

                // 护甲
                // for (int i = 0; i < 4; i++)
                // {
                //     SaveItemStack(writer, player.Inventory.ArmorSlots[i]);
                // }

                // 副手
                // SaveItemStack(writer, player.Inventory.OffhandSlot);
            }
        }

        private void LoadPlayerData()
        {
            string path = Path.Combine(saveDirectory, worldName, "playerdata", "player.dat");
            if (!File.Exists(path)) return;

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
            {
                int version = reader.ReadInt32();

                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                float vx = reader.ReadSingle();
                float vy = reader.ReadSingle();
                float vz = reader.ReadSingle();

                float yaw = reader.ReadSingle();
                float pitch = reader.ReadSingle();

                float health = reader.ReadSingle();
                float maxHealth = reader.ReadSingle();
                float hunger = reader.ReadSingle();
                float maxHunger = reader.ReadSingle();
                float saturation = reader.ReadSingle();
                int experience = reader.ReadInt32();
                int level = reader.ReadInt32();

                bool isFlying = reader.ReadBoolean();
                bool isSprinting = reader.ReadBoolean();
                bool isSneaking = reader.ReadBoolean();
                bool isSwimming = reader.ReadBoolean();

                player.Position = new Vector3(x, y, z);
                player.Velocity = new Vector3(vx, vy, vz);
                player.Yaw = yaw;
                player.Pitch = pitch;
                player.Health = health;
                player.MaxHealth = maxHealth;
                player.Hunger = hunger;
                player.MaxHunger = maxHunger;
                player.Saturation = saturation;
                player.Experience = experience;
                player.Level = level;
                player.IsFlying = isFlying;

                // 加载物品栏
                LoadInventory(reader, null);
            }
        }

        private void SaveInventory(BinaryWriter writer, Inventory inventory)
        {
            if (inventory == null)
            {
                writer.Write(0);
                return;
            }

            writer.Write(inventory.Size);
            for (int i = 0; i < inventory.Size; i++)
            {
                SaveItemStack(writer, inventory.GetSlot(i));
            }
        }

        private void LoadInventory(BinaryReader reader, Inventory inventory)
        {
            int size = reader.ReadInt32();
            if (inventory == null)
            {
                // 跳过物品栏数据
                for (int i = 0; i < size; i++)
                {
                    LoadItemStack(reader);
                }
                return;
            }

            for (int i = 0; i < size && i < inventory.Size; i++)
            {
                inventory.SetSlot(i, LoadItemStack(reader));
            }
        }

        private void SaveItemStack(BinaryWriter writer, ItemStack stack)
        {
            if (stack == null || stack.IsEmpty())
            {
                writer.Write(false);
                return;
            }

            writer.Write(true);
            writer.Write(stack.ItemId);
            writer.Write(stack.Count);
            writer.Write(stack.Durability);
            writer.Write(stack.NBT.Count);
            foreach (var kvp in stack.NBT)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.ToString());
            }
        }

        private ItemStack LoadItemStack(BinaryReader reader)
        {
            bool hasItem = reader.ReadBoolean();
            if (!hasItem) return new ItemStack(0, 0);

            int itemId = reader.ReadInt32();
            int count = reader.ReadInt32();
            int durability = reader.ReadInt32();
            int nbtCount = reader.ReadInt32();

            ItemStack stack = new ItemStack(itemId, count);
            stack.Durability = durability;

            for (int i = 0; i < nbtCount; i++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                stack.NBT[key] = value;
            }

            return stack;
        }

        // ========================================
        // 区块数据
        // ========================================
        private void SaveChunks()
        {
            // 获取所有已加载区块
            // Dictionary<Vector2i, Chunk> chunks = world.GetLoadedChunks();

            // 按区域文件分组（每个区域文件32x32个区块）
            // Dictionary<Vector2i, List<Chunk>> regions = new Dictionary<Vector2i, List<Chunk>>();

            // foreach (var kvp in chunks)
            // {
            //     Vector2i regionPos = new Vector2i(kvp.Key.X >> 5, kvp.Key.Y >> 5);
            //     if (!regions.ContainsKey(regionPos))
            //     {
            //         regions[regionPos] = new List<Chunk>();
            //     }
            //     regions[regionPos].Add(kvp.Value);
            // }

            // foreach (var region in regions)
            // {
            //     SaveRegion(region.Key, region.Value);
            // }
        }

        private void SaveRegion(Vector2i regionPos, List<Chunk> chunks)
        {
            string regionPath = Path.Combine(saveDirectory, worldName, "region", $"r.{regionPos.X}.{regionPos.Y}.mca");

            using (FileStream fs = new FileStream(regionPath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8))
            {
                // 区域文件头：4096字节的位置表 + 4096字节的时间戳表
                byte[] header = new byte[8192];
                writer.Write(header);

                // 写入每个区块
                foreach (Chunk chunk in chunks)
                {
                    SaveChunk(writer, chunk);
                }
            }
        }

        private void SaveChunk(BinaryWriter writer, Chunk chunk)
        {
            // 序列化区块数据
            byte[] chunkData = SerializeChunk(chunk);

            // 压缩
            byte[] compressedData = CompressData(chunkData);

            // 写入长度和压缩后的数据
            writer.Write(compressedData.Length);
            writer.Write((byte)2); // 压缩类型：zlib
            writer.Write(compressedData);
        }

        private byte[] SerializeChunk(Chunk chunk)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(chunk.X);
                writer.Write(chunk.Z);
                writer.Write(chunk.IsGenerated);
                writer.Write(chunk.IsPopulated);

                // 方块数据
                for (int x = 0; x < GameConstants.CHUNK_SIZE; x++)
                {
                    for (int z = 0; z < GameConstants.CHUNK_SIZE; z++)
                    {
                        for (int y = 0; y < GameConstants.CHUNK_HEIGHT; y++)
                        {
                            writer.Write(chunk.GetBlock(x, y, z));
                        }
                    }
                }

                // 光照数据
                // ...

                return ms.ToArray();
            }
        }

        public Chunk LoadChunk(int chunkX, int chunkZ)
        {
            string regionPath = Path.Combine(saveDirectory, worldName, "region",
                $"r.{chunkX >> 5}.{chunkZ >> 5}.mca");

            if (!File.Exists(regionPath)) return null;

            try
            {
                using (FileStream fs = new FileStream(regionPath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    // 读取位置表
                    int chunkIndex = (chunkX & 31) + (chunkZ & 31) * 32;
                    fs.Position = chunkIndex * 4;

                    int offset = reader.ReadInt32();
                    if (offset == 0) return null; // 区块不存在

                    fs.Position = offset * 4096;

                    int length = reader.ReadInt32();
                    byte compressionType = reader.ReadByte();

                    byte[] compressedData = reader.ReadBytes(length - 1);
                    byte[] chunkData = DecompressData(compressedData, compressionType);

                    return DeserializeChunk(chunkData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveManager] 加载区块失败 ({chunkX}, {chunkZ}): {ex.Message}");
                return null;
            }
        }

        private Chunk DeserializeChunk(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                int x = reader.ReadInt32();
                int z = reader.ReadInt32();
                bool isGenerated = reader.ReadBoolean();
                bool isPopulated = reader.ReadBoolean();

                Chunk chunk = new Chunk(x, z);
                chunk.IsGenerated = isGenerated;
                chunk.IsPopulated = isPopulated;

                // 读取方块数据
                for (int bx = 0; bx < GameConstants.CHUNK_SIZE; bx++)
                {
                    for (int bz = 0; bz < GameConstants.CHUNK_SIZE; bz++)
                    {
                        for (int by = 0; by < GameConstants.CHUNK_HEIGHT; by++)
                        {
                            ushort block = reader.ReadUInt16();
                            chunk.SetBlock(bx, by, bz, block);
                        }
                    }
                }

                return chunk;
            }
        }

        // ========================================
        // 关卡数据（游戏规则等）
        // ========================================
        private void SaveLevelData()
        {
            // 可以保存游戏规则、天气、时间等
        }

        private void LoadLevelData()
        {
        }

        // ========================================
        // 压缩工具
        // ========================================
        private byte[] CompressData(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        private byte[] DecompressData(byte[] compressedData, byte compressionType)
        {
            using (MemoryStream ms = new MemoryStream(compressedData))
            using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
            using (MemoryStream output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }

        // ========================================
        // 存档管理
        // ========================================
        public List<SaveInfo> GetAvailableSaves()
        {
            List<SaveInfo> saves = new List<SaveInfo>();

            if (!Directory.Exists(saveDirectory)) return saves;

            string[] directories = Directory.GetDirectories(saveDirectory);
            foreach (string dir in directories)
            {
                string levelDat = Path.Combine(dir, "level.dat");
                if (File.Exists(levelDat))
                {
                    SaveInfo info = ReadSaveInfo(dir);
                    if (info != null)
                    {
                        saves.Add(info);
                    }
                }
            }

            return saves;
        }

        private SaveInfo ReadSaveInfo(string directory)
        {
            try
            {
                string levelDat = Path.Combine(directory, "level.dat");
                using (FileStream fs = new FileStream(levelDat, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs, Encoding.UTF8))
                {
                    int version = reader.ReadInt32();
                    string name = reader.ReadString();
                    long seed = reader.ReadInt64();
                    double time = reader.ReadDouble();

                    return new SaveInfo
                    {
                        Name = name,
                        Path = directory,
                        Seed = seed,
                        PlayTime = time,
                        LastModified = File.GetLastWriteTime(levelDat)
                    };
                }
            }
            catch
            {
                return null;
            }
        }

        public void DeleteSave(string saveName)
        {
            string path = Path.Combine(saveDirectory, saveName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Console.WriteLine($"[SaveManager] 已删除存档: {saveName}");
            }
        }

        public void RenameSave(string oldName, string newName)
        {
            string oldPath = Path.Combine(saveDirectory, oldName);
            string newPath = Path.Combine(saveDirectory, newName);

            if (Directory.Exists(oldPath))
            {
                Directory.Move(oldPath, newPath);
                Console.WriteLine($"[SaveManager] 存档已重命名: {oldName} -> {newName}");
            }
        }

        public bool SaveExists(string saveName)
        {
            return Directory.Exists(Path.Combine(saveDirectory, saveName));
        }

        public long GetSaveSize(string saveName)
        {
            string path = Path.Combine(saveDirectory, saveName);
            if (!Directory.Exists(path)) return 0;

            long size = 0;
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                size += new FileInfo(file).Length;
            }
            return size;
        }
    }

    public class SaveInfo
    {
        public string Name;
        public string Path;
        public long Seed;
        public double PlayTime;
        public DateTime LastModified;
    }
}
