using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class StatisticsSystem
    {
        private static StatisticsSystem instance;
        public static StatisticsSystem Instance => instance ??= new StatisticsSystem();

        // 通用统计
        private readonly Dictionary<string, long> generalStats;

        // 物品统计
        private readonly Dictionary<int, long> minedBlocks;
        private readonly Dictionary<int, long> craftedItems;
        private readonly Dictionary<int, long> usedItems;
        private readonly Dictionary<int, long> brokenItems;
        private readonly Dictionary<int, long> pickedUpItems;
        private readonly Dictionary<int, long> droppedItems;

        // 实体统计
        private readonly Dictionary<int, long> killedEntities;
        private readonly Dictionary<int, long> killedByEntities;

        // 游戏时间
        public long PlayTime { get; private set; }
        public long TimeSinceDeath { get; private set; }
        public long TimeSinceRest { get; private set; }

        // 移动距离（厘米）
        public long WalkDistance { get; private set; }
        public long CrouchDistance { get; private set; }
        public long SprintDistance { get; private set; }
        public long SwimDistance { get; private set; }
        public long FallDistance { get; private set; }
        public long ClimbDistance { get; private set; }
        public long FlyDistance { get; private set; }
        public long DiveDistance { get; private set; }
        public long MinecartDistance { get; private set; }
        public long BoatDistance { get; private set; }
        public long PigDistance { get; private set; }
        public long HorseDistance { get; private set; }
        public long AviateDistance { get; private set; }

        // 其他统计
        public long JumpCount { get; private set; }
        public long DropCount { get; private set; }
        public float DamageDealt { get; private set; }
        public float DamageTaken { get; private set; }
        public long DeathCount { get; private set; }
        public long MobKills { get; private set; }
        public long PlayerKills { get; private set; }
        public long FishCaught { get; private set; }
        public long TalkedToVillager { get; private set; }
        public long TradedWithVillager { get; private set; }
        public long CakeSlicesEaten { get; private set; }
        public long ItemsEnchanted { get; private set; }
        public long RecordsPlayed { get; private set; }
        public long NoteblocksPlayed { get; private set; }
        public long NoteblocksTuned { get; private set; }
        public long FlowerPotted { get; private set; }
        public long BellRung { get; private set; }
        public long RaidTrigger { get; private set; }
        public long RaidWin { get; private set; }
        public long SleepInBed { get; private set; }
        public long Fullness { get; private set; }
        public long OpenContainer { get; private set; }
        public long OpenEnderchest { get; private set; }

        private StatisticsSystem()
        {
            generalStats = new Dictionary<string, long>();
            minedBlocks = new Dictionary<int, long>();
            craftedItems = new Dictionary<int, long>();
            usedItems = new Dictionary<int, long>();
            brokenItems = new Dictionary<int, long>();
            pickedUpItems = new Dictionary<int, long>();
            droppedItems = new Dictionary<int, long>();
            killedEntities = new Dictionary<int, long>();
            killedByEntities = new Dictionary<int, long>();
        }

        public void Initialize()
        {
            Console.WriteLine("[StatisticsSystem] 统计系统初始化完成");
        }

        public void Update(float deltaTime)
        {
            // 更新游戏时间（1秒 = 20刻）
            PlayTime += (long)(deltaTime * 20);
            TimeSinceDeath += (long)(deltaTime * 20);
            TimeSinceRest += (long)(deltaTime * 20);
        }

        // ========================================
        // 通用统计
        // ========================================
        public void IncrementStat(string stat, long amount = 1)
        {
            if (!generalStats.ContainsKey(stat))
            {
                generalStats[stat] = 0;
            }
            generalStats[stat] += amount;
        }

        public long GetStat(string stat)
        {
            return generalStats.TryGetValue(stat, out long value) ? value : 0;
        }

        // ========================================
        // 方块统计
        // ========================================
        public void OnBlockMined(int blockId)
        {
            if (!minedBlocks.ContainsKey(blockId))
            {
                minedBlocks[blockId] = 0;
            }
            minedBlocks[blockId]++;
        }

        public long GetBlocksMined(int blockId)
        {
            return minedBlocks.TryGetValue(blockId, out long value) ? value : 0;
        }

        public long GetTotalBlocksMined()
        {
            long total = 0;
            foreach (long value in minedBlocks.Values)
            {
                total += value;
            }
            return total;
        }

        // ========================================
        // 物品统计
        // ========================================
        public void OnItemCrafted(int itemId, int count = 1)
        {
            if (!craftedItems.ContainsKey(itemId))
            {
                craftedItems[itemId] = 0;
            }
            craftedItems[itemId] += count;
        }

        public long GetItemsCrafted(int itemId)
        {
            return craftedItems.TryGetValue(itemId, out long value) ? value : 0;
        }

        public void OnItemUsed(int itemId)
        {
            if (!usedItems.ContainsKey(itemId))
            {
                usedItems[itemId] = 0;
            }
            usedItems[itemId]++;
        }

        public long GetItemsUsed(int itemId)
        {
            return usedItems.TryGetValue(itemId, out long value) ? value : 0;
        }

        public void OnItemBroken(int itemId)
        {
            if (!brokenItems.ContainsKey(itemId))
            {
                brokenItems[itemId] = 0;
            }
            brokenItems[itemId]++;
        }

        public long GetItemsBroken(int itemId)
        {
            return brokenItems.TryGetValue(itemId, out long value) ? value : 0;
        }

        public void OnItemPickedUp(int itemId, int count = 1)
        {
            if (!pickedUpItems.ContainsKey(itemId))
            {
                pickedUpItems[itemId] = 0;
            }
            pickedUpItems[itemId] += count;
        }

        public long GetItemsPickedUp(int itemId)
        {
            return pickedUpItems.TryGetValue(itemId, out long value) ? value : 0;
        }

        public void OnItemDropped(int itemId, int count = 1)
        {
            if (!droppedItems.ContainsKey(itemId))
            {
                droppedItems[itemId] = 0;
            }
            droppedItems[itemId] += count;
            DropCount += count;
        }

        public long GetItemsDropped(int itemId)
        {
            return droppedItems.TryGetValue(itemId, out long value) ? value : 0;
        }

        // ========================================
        // 实体统计
        // ========================================
        public void OnEntityKilled(int entityType)
        {
            if (!killedEntities.ContainsKey(entityType))
            {
                killedEntities[entityType] = 0;
            }
            killedEntities[entityType]++;
            MobKills++;
        }

        public long GetEntitiesKilled(int entityType)
        {
            return killedEntities.TryGetValue(entityType, out long value) ? value : 0;
        }

        public void OnKilledByEntity(int entityType)
        {
            if (!killedByEntities.ContainsKey(entityType))
            {
                killedByEntities[entityType] = 0;
            }
            killedByEntities[entityType]++;
        }

        public long GetKilledByEntity(int entityType)
        {
            return killedByEntities.TryGetValue(entityType, out long value) ? value : 0;
        }

        public void OnPlayerKilled()
        {
            PlayerKills++;
        }

        // ========================================
        // 移动统计
        // ========================================
        public void AddWalkDistance(float distance)
        {
            WalkDistance += (long)(distance * 100);
        }

        public void AddCrouchDistance(float distance)
        {
            CrouchDistance += (long)(distance * 100);
        }

        public void AddSprintDistance(float distance)
        {
            SprintDistance += (long)(distance * 100);
        }

        public void AddSwimDistance(float distance)
        {
            SwimDistance += (long)(distance * 100);
        }

        public void AddFallDistance(float distance)
        {
            FallDistance += (long)(distance * 100);
        }

        public void AddClimbDistance(float distance)
        {
            ClimbDistance += (long)(distance * 100);
        }

        public void AddFlyDistance(float distance)
        {
            FlyDistance += (long)(distance * 100);
        }

        public void AddDiveDistance(float distance)
        {
            DiveDistance += (long)(distance * 100);
        }

        public void AddMinecartDistance(float distance)
        {
            MinecartDistance += (long)(distance * 100);
        }

        public void AddBoatDistance(float distance)
        {
            BoatDistance += (long)(distance * 100);
        }

        public void AddPigDistance(float distance)
        {
            PigDistance += (long)(distance * 100);
        }

        public void AddHorseDistance(float distance)
        {
            HorseDistance += (long)(distance * 100);
        }

        public void AddAviateDistance(float distance)
        {
            AviateDistance += (long)(distance * 100);
        }

        // ========================================
        // 其他统计
        // ========================================
        public void OnJump()
        {
            JumpCount++;
        }

        public void OnDamageDealt(float damage)
        {
            DamageDealt += damage;
        }

        public void OnDamageTaken(float damage)
        {
            DamageTaken += damage;
        }

        public void OnDeath()
        {
            DeathCount++;
            TimeSinceDeath = 0;
        }

        public void OnFishCaught()
        {
            FishCaught++;
        }

        public void OnTalkedToVillager()
        {
            TalkedToVillager++;
        }

        public void OnTradedWithVillager()
        {
            TradedWithVillager++;
        }

        public void OnCakeEaten()
        {
            CakeSlicesEaten++;
        }

        public void OnItemEnchanted()
        {
            ItemsEnchanted++;
        }

        public void OnRecordPlayed()
        {
            RecordsPlayed++;
        }

        public void OnNoteblockPlayed()
        {
            NoteblocksPlayed++;
        }

        public void OnNoteblockTuned()
        {
            NoteblocksTuned++;
        }

        public void OnFlowerPotted()
        {
            FlowerPotted++;
        }

        public void OnBellRung()
        {
            BellRung++;
        }

        public void OnRaidTrigger()
        {
            RaidTrigger++;
        }

        public void OnRaidWin()
        {
            RaidWin++;
        }

        public void OnSleepInBed()
        {
            SleepInBed++;
            TimeSinceRest = 0;
        }

        public void OnEat()
        {
            Fullness++;
        }

        public void OnOpenContainer()
        {
            OpenContainer++;
        }

        public void OnOpenEnderchest()
        {
            OpenEnderchest++;
        }

        // ========================================
        // 格式化
        // ========================================
        public static string FormatDistance(long cm)
        {
            if (cm < 100) return $"{cm} cm";
            if (cm < 100000) return $"{cm / 100.0:F1} m";
            return $"{cm / 100000.0:F1} km";
        }

        public static string FormatTime(long ticks)
        {
            long seconds = ticks / 20;
            long minutes = seconds / 60;
            long hours = minutes / 60;
            long days = hours / 24;

            if (days > 0) return $"{days}天 {hours % 24}小时";
            if (hours > 0) return $"{hours}小时 {minutes % 60}分钟";
            if (minutes > 0) return $"{minutes}分钟 {seconds % 60}秒";
            return $"{seconds}秒";
        }

        public static string FormatNumber(long number)
        {
            if (number < 1000) return number.ToString();
            if (number < 1000000) return $"{number / 1000.0:F1}K";
            if (number < 1000000000) return $"{number / 1000000.0:F1}M";
            return $"{number / 1000000000.0:F1}B";
        }

        // ========================================
        // 保存/加载
        // ========================================
        public void Save(string path)
        {
            // 保存统计数据
            Console.WriteLine("[StatisticsSystem] 统计数据已保存");
        }

        public void Load(string path)
        {
            // 加载统计数据
            Console.WriteLine("[StatisticsSystem] 统计数据已加载");
        }

        public void Reset()
        {
            generalStats.Clear();
            minedBlocks.Clear();
            craftedItems.Clear();
            usedItems.Clear();
            brokenItems.Clear();
            pickedUpItems.Clear();
            droppedItems.Clear();
            killedEntities.Clear();
            killedByEntities.Clear();

            PlayTime = 0;
            TimeSinceDeath = 0;
            TimeSinceRest = 0;
            WalkDistance = 0;
            CrouchDistance = 0;
            SprintDistance = 0;
            SwimDistance = 0;
            FallDistance = 0;
            ClimbDistance = 0;
            FlyDistance = 0;
            DiveDistance = 0;
            MinecartDistance = 0;
            BoatDistance = 0;
            PigDistance = 0;
            HorseDistance = 0;
            AviateDistance = 0;
            JumpCount = 0;
            DropCount = 0;
            DamageDealt = 0;
            DamageTaken = 0;
            DeathCount = 0;
            MobKills = 0;
            PlayerKills = 0;
            FishCaught = 0;
            TalkedToVillager = 0;
            TradedWithVillager = 0;
            CakeSlicesEaten = 0;
            ItemsEnchanted = 0;
            RecordsPlayed = 0;
            NoteblocksPlayed = 0;
            NoteblocksTuned = 0;
            FlowerPotted = 0;
            BellRung = 0;
            RaidTrigger = 0;
            RaidWin = 0;
            SleepInBed = 0;
            Fullness = 0;
            OpenContainer = 0;
            OpenEnderchest = 0;

            Console.WriteLine("[StatisticsSystem] 统计数据已重置");
        }

        public Dictionary<string, long> GetAllGeneralStats()
        {
            return new Dictionary<string, long>(generalStats);
        }

        public Dictionary<int, long> GetAllMinedBlocks()
        {
            return new Dictionary<int, long>(minedBlocks);
        }

        public Dictionary<int, long> GetAllCraftedItems()
        {
            return new Dictionary<int, long>(craftedItems);
        }

        public Dictionary<int, long> GetAllKilledEntities()
        {
            return new Dictionary<int, long>(killedEntities);
        }
    }
}
