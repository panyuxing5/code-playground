using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.World;

namespace VoxelCraft.World
{
    public class PlantGrowthSystem
    {
        private readonly WorldManager world;
        private readonly Random random;
        private readonly Queue<Vector3i> updateQueue;
        private readonly HashSet<Vector3i> queuedBlocks;
        private readonly object queueLock = new object();

        // 生长参数
        public int GrowthTickRate { get; set; } = 50;
        public float GrowthChance { get; set; } = 0.5f;
        public bool BonemealEnabled { get; set; } = true;

        // 统计
        public int PlantsGrown { get; private set; }

        public PlantGrowthSystem(WorldManager world, int seed = 0)
        {
            this.world = world;
            random = new Random(seed);
            updateQueue = new Queue<Vector3i>();
            queuedBlocks = new HashSet<Vector3i>();
        }

        public void Initialize()
        {
            Console.WriteLine("[PlantGrowthSystem] 植物生长系统初始化完成");
        }

        public void Update()
        {
            PlantsGrown = 0;

            int maxUpdates = 100;
            for (int i = 0; i < maxUpdates && updateQueue.Count > 0; i++)
            {
                Vector3i pos;
                lock (queueLock)
                {
                    pos = updateQueue.Dequeue();
                    queuedBlocks.Remove(pos);
                }

                UpdatePlant(pos.X, pos.Y, pos.Z);
            }
        }

        public void ScheduleUpdate(int x, int y, int z)
        {
            Vector3i pos = new Vector3i(x, y, z);
            lock (queueLock)
            {
                if (!queuedBlocks.Contains(pos))
                {
                    updateQueue.Enqueue(pos);
                    queuedBlocks.Add(pos);
                }
            }
        }

        private void UpdatePlant(int x, int y, int z)
        {
            ushort block = world.GetBlock(x, y, z);

            switch (block)
            {
                case GameConstants.BLOCK_WHEAT:
                    GrowCrop(x, y, z, GameConstants.BLOCK_WHEAT, 7);
                    break;
                case GameConstants.BLOCK_CARROT:
                    GrowCrop(x, y, z, GameConstants.BLOCK_CARROT, 7);
                    break;
                case GameConstants.BLOCK_POTATO:
                    GrowCrop(x, y, z, GameConstants.BLOCK_POTATO, 7);
                    break;
                case GameConstants.BLOCK_BEETROOT:
                    GrowCrop(x, y, z, GameConstants.BLOCK_BEETROOT, 3);
                    break;
                case GameConstants.BLOCK_SUGAR_CANE:
                    GrowSugarCane(x, y, z);
                    break;
                case GameConstants.BLOCK_CACTUS:
                    GrowCactus(x, y, z);
                    break;
                case GameConstants.BLOCK_BAMBOO:
                    GrowBamboo(x, y, z);
                    break;
                case GameConstants.BLOCK_SAPLING:
                    GrowSapling(x, y, z);
                    break;
                case GameConstants.BLOCK_MELON_STEM:
                    GrowStem(x, y, z, GameConstants.BLOCK_MELON_STEM, GameConstants.BLOCK_MELON);
                    break;
                case GameConstants.BLOCK_PUMPKIN_STEM:
                    GrowStem(x, y, z, GameConstants.BLOCK_PUMPKIN_STEM, GameConstants.BLOCK_PUMPKIN);
                    break;
                case GameConstants.BLOCK_SWEET_BERRY_BUSH:
                    GrowSweetBerryBush(x, y, z);
                    break;
                case GameConstants.BLOCK_KELP:
                    GrowKelp(x, y, z);
                    break;
                case GameConstants.BLOCK_SEAGRASS:
                    // 海草不生长
                    break;
                case GameConstants.BLOCK_VINE:
                    GrowVine(x, y, z);
                    break;
                case GameConstants.BLOCK_COCOA:
                    GrowCocoa(x, y, z);
                    break;
                case GameConstants.BLOCK_NETHER_WART:
                    GrowNetherWart(x, y, z);
                    break;
                case GameConstants.BLOCK_CHORUS_FLOWER:
                    GrowChorusFlower(x, y, z);
                    break;
                case GameConstants.BLOCK_SWEET_BERRY_BUSH:
                    GrowSweetBerryBush(x, y, z);
                    break;
            }
        }

        private void GrowCrop(int x, int y, int z, ushort cropType, int maxAge)
        {
            // 检查是否有足够的光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 检查是否在耕地上
            ushort below = world.GetBlock(x, y - 1, z);
            if (below != GameConstants.BLOCK_FARMLAND) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance) return;

            // 获取当前生长阶段
            int currentAge = GetCropAge(x, y, z);
            if (currentAge >= maxAge) return;

            // 生长
            SetCropAge(x, y, z, currentAge + 1);
            PlantsGrown++;

            // 检查是否完全成熟
            if (currentAge + 1 >= maxAge)
            {
                // 作物成熟，可以收获
            }
        }

        private void GrowSugarCane(int x, int y, int z)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 检查下方是否是甘蔗或沙子/泥土
            ushort below = world.GetBlock(x, y - 1, z);
            if (below != GameConstants.BLOCK_SUGAR_CANE &&
                below != GameConstants.BLOCK_SAND &&
                below != GameConstants.BLOCK_DIRT &&
                below != GameConstants.BLOCK_GRASS)
            {
                return;
            }

            // 计算当前高度
            int height = 1;
            while (world.GetBlock(x, y - height, z) == GameConstants.BLOCK_SUGAR_CANE)
            {
                height++;
            }

            if (height >= 3) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.5) return;

            // 在上方生长
            if (world.GetBlock(x, y + 1, z) == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y + 1, z, GameConstants.BLOCK_SUGAR_CANE);
                PlantsGrown++;
            }
        }

        private void GrowCactus(int x, int y, int z)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 检查下方
            ushort below = world.GetBlock(x, y - 1, z);
            if (below != GameConstants.BLOCK_CACTUS && below != GameConstants.BLOCK_SAND)
            {
                return;
            }

            // 计算高度
            int height = 1;
            while (world.GetBlock(x, y - height, z) == GameConstants.BLOCK_CACTUS)
            {
                height++;
            }

            if (height >= 3) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.3) return;

            // 检查周围是否有方块（仙人掌不能与其他方块相邻）
            if (world.GetBlock(x + 1, y, z) != GameConstants.BLOCK_AIR ||
                world.GetBlock(x - 1, y, z) != GameConstants.BLOCK_AIR ||
                world.GetBlock(x, y, z + 1) != GameConstants.BLOCK_AIR ||
                world.GetBlock(x, y, z - 1) != GameConstants.BLOCK_AIR)
            {
                return;
            }

            // 生长
            if (world.GetBlock(x, y + 1, z) == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y + 1, z, GameConstants.BLOCK_CACTUS);
                PlantsGrown++;
            }
        }

        private void GrowBamboo(int x, int y, int z)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 计算高度
            int height = 1;
            while (world.GetBlock(x, y - height, z) == GameConstants.BLOCK_BAMBOO)
            {
                height++;
            }

            if (height >= 16) return;

            // 生长概率（竹子生长较快）
            if (random.NextDouble() > GrowthChance * 1.5) return;

            // 生长
            if (world.GetBlock(x, y + 1, z) == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y + 1, z, GameConstants.BLOCK_BAMBOO);
                PlantsGrown++;
            }
        }

        private void GrowSapling(int x, int y, int z)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.1) return;

            // 长成树
            TreeGenerator treeGenerator = new TreeGenerator(world, random.Next());
            treeGenerator.GenerateTree(x, y, z);

            // 移除树苗
            world.SetBlock(x, y, z, GameConstants.BLOCK_AIR);
            PlantsGrown++;
        }

        private void GrowStem(int x, int y, int z, ushort stemType, ushort fruitType)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 检查是否在耕地上
            ushort below = world.GetBlock(x, y - 1, z);
            if (below != GameConstants.BLOCK_FARMLAND) return;

            // 获取生长阶段
            int currentAge = GetCropAge(x, y, z);

            if (currentAge < 7)
            {
                // 茎生长
                if (random.NextDouble() > GrowthChance) return;
                SetCropAge(x, y, z, currentAge + 1);
                PlantsGrown++;
            }
            else
            {
                // 茎成熟，尝试结果
                if (random.NextDouble() > GrowthChance * 0.2) return;

                // 在相邻的空地上结果
                int[] dx = { 1, -1, 0, 0 };
                int[] dz = { 0, 0, 1, -1 };
                int dir = random.Next(4);

                int fruitX = x + dx[dir];
                int fruitZ = z + dz[dir];

                if (world.GetBlock(fruitX, y, fruitZ) == GameConstants.BLOCK_AIR)
                {
                    ushort fruitBelow = world.GetBlock(fruitX, y - 1, fruitZ);
                    if (fruitBelow == GameConstants.BLOCK_FARMLAND ||
                        fruitBelow == GameConstants.BLOCK_DIRT ||
                        fruitBelow == GameConstants.BLOCK_GRASS)
                    {
                        world.SetBlock(fruitX, y, fruitZ, fruitType);
                        PlantsGrown++;
                    }
                }
            }
        }

        private void GrowSweetBerryBush(int x, int y, int z)
        {
            // 检查光照
            int light = world.GetLightLevel(x, y, z);
            if (light < 9) return;

            // 生长阶段：0=幼苗，1=中，2=成熟，3=结果
            int currentAge = GetCropAge(x, y, z);
            if (currentAge >= 3) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.3) return;

            SetCropAge(x, y, z, currentAge + 1);
            PlantsGrown++;
        }

        private void GrowKelp(int x, int y, int z)
        {
            // 海带只能在水中生长
            ushort above = world.GetBlock(x, y + 1, z);
            if (above != GameConstants.BLOCK_WATER_STILL &&
                above != GameConstants.BLOCK_WATER_FLOWING)
            {
                return;
            }

            // 计算高度
            int height = 1;
            while (world.GetBlock(x, y - height, z) == GameConstants.BLOCK_KELP)
            {
                height++;
            }

            if (height >= 26) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.7) return;

            // 生长
            world.SetBlock(x, y + 1, z, GameConstants.BLOCK_KELP);
            PlantsGrown++;
        }

        private void GrowVine(int x, int y, int z)
        {
            // 藤蔓生长概率较低
            if (random.NextDouble() > GrowthChance * 0.1) return;

            // 向下生长
            if (world.GetBlock(x, y - 1, z) == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y - 1, z, GameConstants.BLOCK_VINE);
                PlantsGrown++;
                return;
            }

            // 水平生长
            int[] dx = { 1, -1, 0, 0 };
            int[] dz = { 0, 0, 1, -1 };
            int dir = random.Next(4);

            int vineX = x + dx[dir];
            int vineZ = z + dz[dir];

            if (world.GetBlock(vineX, y, vineZ) == GameConstants.BLOCK_AIR)
            {
                // 检查是否有支撑
                ushort support = world.GetBlock(vineX + dx[dir], y, vineZ + dz[dir]);
                if (support != GameConstants.BLOCK_AIR)
                {
                    world.SetBlock(vineX, y, vineZ, GameConstants.BLOCK_VINE);
                    PlantsGrown++;
                }
            }
        }

        private void GrowCocoa(int x, int y, int z)
        {
            // 可可豆生长
            int currentAge = GetCropAge(x, y, z);
            if (currentAge >= 2) return;

            // 生长概率
            if (random.NextDouble() > GrowthChance * 0.2) return;

            SetCropAge(x, y, z, currentAge + 1);
            PlantsGrown++;
        }

        private void GrowNetherWart(int x, int y, int z)
        {
            // 下界疣只能在灵魂沙上生长
            ushort below = world.GetBlock(x, y - 1, z);
            if (below != GameConstants.BLOCK_SOUL_SAND) return;

            // 生长阶段：0-3
            int currentAge = GetCropAge(x, y, z);
            if (currentAge >= 3) return;

            // 生长概率（下界疣生长较慢）
            if (random.NextDouble() > GrowthChance * 0.1) return;

            SetCropAge(x, y, z, currentAge + 1);
            PlantsGrown++;
        }

        private void GrowChorusFlower(int x, int y, int z)
        {
            // 紫颂花生长
            if (random.NextDouble() > GrowthChance * 0.05) return;

            // 向上生长
            if (world.GetBlock(x, y + 1, z) == GameConstants.BLOCK_AIR)
            {
                world.SetBlock(x, y, z, GameConstants.BLOCK_CHORUS_PLANT);
                world.SetBlock(x, y + 1, z, GameConstants.BLOCK_CHORUS_FLOWER);
                PlantsGrown++;
            }
        }

        public void ApplyBonemeal(int x, int y, int z)
        {
            if (!BonemealEnabled) return;

            ushort block = world.GetBlock(x, y, z);

            switch (block)
            {
                case GameConstants.BLOCK_WHEAT:
                case GameConstants.BLOCK_CARROT:
                case GameConstants.BLOCK_POTATO:
                case GameConstants.BLOCK_BEETROOT:
                    // 骨粉加速作物生长2-5阶段
                    int currentAge = GetCropAge(x, y, z);
                    int growth = random.Next(2, 6);
                    SetCropAge(x, y, z, Math.Min(currentAge + growth, 7));
                    PlantsGrown++;
                    break;

                case GameConstants.BLOCK_SAPLING:
                    // 骨粉让树苗立刻长成树
                    GrowSapling(x, y, z);
                    break;

                case GameConstants.BLOCK_SUGAR_CANE:
                case GameConstants.BLOCK_CACTUS:
                case GameConstants.BLOCK_BAMBOO:
                    // 骨粉让这些植物立即生长1格
                    if (world.GetBlock(x, y + 1, z) == GameConstants.BLOCK_AIR)
                    {
                        world.SetBlock(x, y + 1, z, block);
                        PlantsGrown++;
                    }
                    break;

                case GameConstants.BLOCK_SWEET_BERRY_BUSH:
                    // 骨粉让甜浆果丛生长
                    int berryAge = GetCropAge(x, y, z);
                    if (berryAge < 3)
                    {
                        SetCropAge(x, y, z, berryAge + 1);
                        PlantsGrown++;
                    }
                    break;

                case GameConstants.BLOCK_MELON_STEM:
                case GameConstants.BLOCK_PUMPKIN_STEM:
                    // 骨粉让茎成熟
                    int stemAge = GetCropAge(x, y, z);
                    SetCropAge(x, y, z, Math.Min(stemAge + 3, 7));
                    PlantsGrown++;
                    break;
            }
        }

        private int GetCropAge(int x, int y, int z)
        {
            // 简化：从方块元数据获取生长阶段
            // 实际应该从block state获取
            return 0;
        }

        private void SetCropAge(int x, int y, int z, int age)
        {
            // 简化：设置方块元数据
            // 实际应该设置block state
        }

        public void ClearQueue()
        {
            lock (queueLock)
            {
                updateQueue.Clear();
                queuedBlocks.Clear();
            }
        }
    }
}
