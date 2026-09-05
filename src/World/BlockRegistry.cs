using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public static class BlockRegistry
    {
        private static readonly Dictionary<ushort, BlockInfo> blocks = new Dictionary<ushort, BlockInfo>();
        private static readonly Dictionary<string, ushort> nameToId = new Dictionary<string, ushort>();
        private static bool initialized = false;

        public static void RegisterAllBlocks()
        {
            if (initialized) return;
            initialized = true;

            Console.WriteLine("[BlockRegistry] 注册方块...");

            // ========================================
            // 空气
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_AIR, "air")
                .SetType(BlockType.Air)
                .SetMaterial(BlockMaterial.Air)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetReplaceable(true)
                .SetCollidable(false)
                .SetSelectable(false)
                .SetHardness(-1)
                .SetResistance(0));

            // ========================================
            // 石头类
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_STONE, "stone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetDrop(GameConstants.ITEM_STONE_PICKAXE) // 实际应该是圆石
                .SetTexture("stone")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_GRANITE, "granite")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("granite")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_DIORITE, "diorite")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("diorite")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_ANDESITE, "andesite")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("andesite")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_COBBLESTONE, "cobblestone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(2.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("cobblestone")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_MOSSY_COBBLESTONE, "mossy_cobblestone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(2.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("mossy_cobblestone")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_STONE_BRICKS, "stone_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("stone_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_MOSSY_STONE_BRICKS, "mossy_stone_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("mossy_stone_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_CRACKED_STONE_BRICKS, "cracked_stone_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("cracked_stone_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_CHISELED_STONE_BRICKS, "chiseled_stone_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("chiseled_stone_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 泥土/草方块
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_GRASS, "grass_block")
                .SetType(BlockType.Grass)
                .SetMaterial(BlockMaterial.Grass)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTool(ToolType.Shovel)
                .SetTexture("grass_top", "dirt", "grass_side")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));

            Register(new BlockInfo(GameConstants.BLOCK_DIRT, "dirt")
                .SetType(BlockType.Dirt)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("dirt")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            Register(new BlockInfo(GameConstants.BLOCK_COARSE_DIRT, "coarse_dirt")
                .SetType(BlockType.Dirt)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("coarse_dirt")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            Register(new BlockInfo(GameConstants.BLOCK_PODZOL, "podzol")
                .SetType(BlockType.Dirt)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("podzol_top", "dirt", "podzol_side")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            Register(new BlockInfo(GameConstants.BLOCK_MYCELIUM, "mycelium")
                .SetType(BlockType.Dirt)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("mycelium_top", "dirt", "mycelium_side")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            Register(new BlockInfo(GameConstants.BLOCK_GRASS_PATH, "grass_path")
                .SetType(BlockType.Dirt)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTool(ToolType.Shovel)
                .SetFullBlock(false)
                .SetBoundingBox(0, 0, 0, 1, 0.9375f, 1)
                .SetTexture("grass_path_top", "dirt", "grass_path_side")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            Register(new BlockInfo(GameConstants.BLOCK_FARMLAND, "farmland")
                .SetType(BlockType.Crop)
                .SetMaterial(BlockMaterial.Dirt)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTool(ToolType.Shovel)
                .SetFullBlock(false)
                .SetBoundingBox(0, 0, 0, 1, 0.9375f, 1)
                .SetTexture("farmland", "dirt", "farmland")
                .SetSounds("block.dirt.break", "block.dirt.place", "block.dirt.step"));

            // ========================================
            // 基岩
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_BEDROCK, "bedrock")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(-1)
                .SetResistance(3600000.0f)
                .SetTexture("bedrock")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 矿石
            // ========================================
            RegisterOre(GameConstants.BLOCK_COAL_ORE, "coal_ore", 3.0f, 3.0f, GameConstants.ITEM_AIR, 0, 2);
            RegisterOre(GameConstants.BLOCK_IRON_ORE, "iron_ore", 3.0f, 3.0f, GameConstants.ITEM_RAW_IRON, 0, 0);
            RegisterOre(GameConstants.BLOCK_GOLD_ORE, "gold_ore", 3.0f, 3.0f, GameConstants.ITEM_RAW_GOLD, 0, 0);
            RegisterOre(GameConstants.BLOCK_DIAMOND_ORE, "diamond_ore", 3.0f, 3.0f, GameConstants.ITEM_DIAMOND, 3, 7);
            RegisterOre(GameConstants.BLOCK_EMERALD_ORE, "emerald_ore", 3.0f, 3.0f, GameConstants.ITEM_EMERALD, 3, 7);
            RegisterOre(GameConstants.BLOCK_LAPIS_ORE, "lapis_ore", 3.0f, 3.0f, GameConstants.ITEM_LAPIS_LAZULI, 2, 5);
            RegisterOre(GameConstants.BLOCK_REDSTONE_ORE, "redstone_ore", 3.0f, 3.0f, GameConstants.ITEM_REDSTONE, 4, 5);
            RegisterOre(GameConstants.BLOCK_COPPER_ORE, "copper_ore", 3.0f, 3.0f, GameConstants.ITEM_RAW_COPPER, 0, 0);

            // ========================================
            // 矿物块
            // ========================================
            RegisterBlock(GameConstants.BLOCK_COAL_BLOCK, "coal_block", BlockType.Stone, BlockMaterial.Coal, 5.0f, 6.0f, "coal_block", ToolType.Pickaxe, ToolTier.Wood);
            RegisterBlock(GameConstants.BLOCK_IRON_BLOCK, "iron_block", BlockType.Stone, BlockMaterial.Iron, 5.0f, 6.0f, "iron_block", ToolType.Pickaxe, ToolTier.Stone);
            RegisterBlock(GameConstants.BLOCK_GOLD_BLOCK, "gold_block", BlockType.Stone, BlockMaterial.Gold, 3.0f, 6.0f, "gold_block", ToolType.Pickaxe, ToolTier.Iron);
            RegisterBlock(GameConstants.BLOCK_DIAMOND_BLOCK, "diamond_block", BlockType.Stone, BlockMaterial.Diamond, 5.0f, 6.0f, "diamond_block", ToolType.Pickaxe, ToolTier.Iron);
            RegisterBlock(GameConstants.BLOCK_EMERALD_BLOCK, "emerald_block", BlockType.Stone, BlockMaterial.Emerald, 5.0f, 6.0f, "emerald_block", ToolType.Pickaxe, ToolTier.Iron);
            RegisterBlock(GameConstants.BLOCK_LAPIS_BLOCK, "lapis_block", BlockType.Stone, BlockMaterial.Lapis, 3.0f, 6.0f, "lapis_block", ToolType.Pickaxe, ToolTier.Stone);
            RegisterBlock(GameConstants.BLOCK_REDSTONE_BLOCK, "redstone_block", BlockType.Redstone, BlockMaterial.Redstone, 5.0f, 6.0f, "redstone_block", ToolType.Pickaxe, ToolTier.Wood);
            RegisterBlock(GameConstants.BLOCK_COPPER_BLOCK, "copper_block", BlockType.Stone, BlockMaterial.Copper, 3.0f, 6.0f, "copper_block", ToolType.Pickaxe, ToolTier.Stone);

            // ========================================
            // 木头/木板
            // ========================================
            RegisterWoodBlocks(GameConstants.BLOCK_LOG, "oak", GameConstants.BLOCK_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_SPRUCE_LOG, "spruce", GameConstants.BLOCK_SPRUCE_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_BIRCH_LOG, "birch", GameConstants.BLOCK_BIRCH_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_JUNGLE_LOG, "jungle", GameConstants.BLOCK_JUNGLE_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_ACACIA_LOG, "acacia", GameConstants.BLOCK_ACACIA_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_DARK_OAK_LOG, "dark_oak", GameConstants.BLOCK_DARK_OAK_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_MANGROVE_LOG, "mangrove", GameConstants.BLOCK_MANGROVE_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_CHERRY_LOG, "cherry", GameConstants.BLOCK_CHERRY_PLANKS);
            RegisterWoodBlocks(GameConstants.BLOCK_BAMBOO_PLANKS, "bamboo", GameConstants.BLOCK_BAMBOO_PLANKS);

            // ========================================
            // 树叶
            // ========================================
            RegisterLeaves(GameConstants.BLOCK_LEAVES, "oak");
            RegisterLeaves(GameConstants.BLOCK_SPRUCE_LEAVES, "spruce");
            RegisterLeaves(GameConstants.BLOCK_BIRCH_LEAVES, "birch");
            RegisterLeaves(GameConstants.BLOCK_JUNGLE_LEAVES, "jungle");
            RegisterLeaves(GameConstants.BLOCK_ACACIA_LEAVES, "acacia");
            RegisterLeaves(GameConstants.BLOCK_DARK_OAK_LEAVES, "dark_oak");
            RegisterLeaves(GameConstants.BLOCK_MANGROVE_LEAVES, "mangrove");
            RegisterLeaves(GameConstants.BLOCK_CHERRY_LEAVES, "cherry");
            RegisterLeaves(GameConstants.BLOCK_AZALEA_LEAVES, "azalea");
            RegisterLeaves(GameConstants.BLOCK_FLOWERING_AZALEA_LEAVES, "flowering_azalea");

            // ========================================
            // 沙子/沙砾
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_SAND, "sand")
                .SetType(BlockType.Sand)
                .SetMaterial(BlockMaterial.Sand)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetGravity(true)
                .SetTexture("sand")
                .SetSounds("block.sand.break", "block.sand.place", "block.sand.step"));

            Register(new BlockInfo(GameConstants.BLOCK_RED_SAND, "red_sand")
                .SetType(BlockType.Sand)
                .SetMaterial(BlockMaterial.Sand)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetGravity(true)
                .SetTexture("red_sand")
                .SetSounds("block.sand.break", "block.sand.place", "block.sand.step"));

            Register(new BlockInfo(GameConstants.BLOCK_GRAVEL, "gravel")
                .SetType(BlockType.Gravel)
                .SetMaterial(BlockMaterial.Gravel)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTool(ToolType.Shovel)
                .SetGravity(true)
                .SetTexture("gravel")
                .SetSounds("block.gravel.break", "block.gravel.place", "block.gravel.step"));

            // ========================================
            // 玻璃
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_GLASS, "glass")
                .SetType(BlockType.Glass)
                .SetMaterial(BlockMaterial.Glass)
                .SetHardness(0.3f)
                .SetResistance(0.3f)
                .SetTool(ToolType.Pickaxe)
                .SetTransparent(true)
                .SetLightOpacity(0)
                .SetTexture("glass")
                .SetSounds("block.glass.break", "block.glass.place", "block.glass.step"));

            // ========================================
            // 液体
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_WATER_STILL, "water")
                .SetType(BlockType.Liquid)
                .SetMaterial(BlockMaterial.Water)
                .SetLiquid(true)
                .SetHardness(100)
                .SetResistance(100)
                .SetLightOpacity(3)
                .SetTexture("water_still")
                .SetSlipperiness(0.8f));

            Register(new BlockInfo(GameConstants.BLOCK_WATER_FLOWING, "flowing_water")
                .SetType(BlockType.Liquid)
                .SetMaterial(BlockMaterial.Water)
                .SetLiquid(true)
                .SetHardness(100)
                .SetResistance(100)
                .SetLightOpacity(3)
                .SetTexture("water_flow")
                .SetSlipperiness(0.8f));

            Register(new BlockInfo(GameConstants.BLOCK_LAVA_STILL, "lava")
                .SetType(BlockType.Liquid)
                .SetMaterial(BlockMaterial.Lava)
                .SetLiquid(true)
                .SetHardness(100)
                .SetResistance(100)
                .SetLightEmission(15)
                .SetLightOpacity(0)
                .SetTexture("lava_still")
                .SetSlipperiness(0.5f));

            Register(new BlockInfo(GameConstants.BLOCK_LAVA_FLOWING, "flowing_lava")
                .SetType(BlockType.Liquid)
                .SetMaterial(BlockMaterial.Lava)
                .SetLiquid(true)
                .SetHardness(100)
                .SetResistance(100)
                .SetLightEmission(15)
                .SetLightOpacity(0)
                .SetTexture("lava_flow")
                .SetSlipperiness(0.5f));

            // ========================================
            // 冰/雪
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_ICE, "ice")
                .SetType(BlockType.Ice)
                .SetMaterial(BlockMaterial.Ice)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Pickaxe)
                .SetTransparent(true)
                .SetLightOpacity(3)
                .SetSlipperiness(0.98f)
                .SetTexture("ice")
                .SetSounds("block.glass.break", "block.glass.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_PACKED_ICE, "packed_ice")
                .SetType(BlockType.Ice)
                .SetMaterial(BlockMaterial.Ice)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Pickaxe)
                .SetSlipperiness(0.98f)
                .SetTexture("packed_ice")
                .SetSounds("block.glass.break", "block.glass.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_BLUE_ICE, "blue_ice")
                .SetType(BlockType.Ice)
                .SetMaterial(BlockMaterial.Ice)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Pickaxe)
                .SetSlipperiness(0.989f)
                .SetTexture("blue_ice")
                .SetSounds("block.glass.break", "block.glass.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SNOW, "snow")
                .SetType(BlockType.Snow)
                .SetMaterial(BlockMaterial.Snow)
                .SetHardness(0.2f)
                .SetResistance(0.2f)
                .SetTool(ToolType.Shovel)
                .SetReplaceable(true)
                .SetFullBlock(false)
                .SetBoundingBox(0, 0, 0, 1, 0.125f, 1)
                .SetTexture("snow")
                .SetSounds("block.snow.break", "block.snow.place", "block.snow.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SNOW_BLOCK, "snow_block")
                .SetType(BlockType.Snow)
                .SetMaterial(BlockMaterial.Snow)
                .SetHardness(0.2f)
                .SetResistance(0.2f)
                .SetTool(ToolType.Shovel)
                .SetTexture("snow_block")
                .SetSounds("block.snow.break", "block.snow.place", "block.snow.step"));

            // ========================================
            // 植物
            // ========================================
            RegisterPlant(GameConstants.BLOCK_DANDELION, "dandelion", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_POPPY, "poppy", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_BLUE_ORCHID, "blue_orchid", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_ALLIUM, "allium", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_AZURE_BLUET, "azure_bluet", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_RED_TULIP, "red_tulip", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_ORANGE_TULIP, "orange_tulip", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_WHITE_TULIP, "white_tulip", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_PINK_TULIP, "pink_tulip", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_OXEYE_DAISY, "oxeye_daisy", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_CORNFLOWER, "cornflower", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_LILY_OF_THE_VALLEY, "lily_of_the_valley", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_WITHER_ROSE, "wither_rose", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_SUNFLOWER, "sunflower", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_LILAC, "lilac", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_ROSE_BUSH, "rose_bush", BlockType.Flower);
            RegisterPlant(GameConstants.BLOCK_PEONY, "peony", BlockType.Flower);

            RegisterPlant(GameConstants.BLOCK_TALL_GRASS, "tall_grass", BlockType.TallGrass);
            RegisterPlant(GameConstants.BLOCK_LARGE_FERN, "large_fern", BlockType.TallGrass);
            RegisterPlant(GameConstants.BLOCK_FERN, "fern", BlockType.TallGrass);
            RegisterPlant(GameConstants.BLOCK_DEAD_BUSH, "dead_bush", BlockType.DeadBush);
            RegisterPlant(GameConstants.BLOCK_BROWN_MUSHROOM, "brown_mushroom", BlockType.Mushroom);
            RegisterPlant(GameConstants.BLOCK_RED_MUSHROOM, "red_mushroom", BlockType.Mushroom);

            // ========================================
            // 火把
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_TORCH, "torch")
                .SetType(BlockType.Torch)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetFullBlock(false)
                .SetLightEmission(14)
                .SetLightOpacity(0)
                .SetBoundingBox(0.35f, 0, 0.35f, 0.65f, 0.6f, 0.65f)
                .SetTexture("torch")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SOUL_TORCH, "soul_torch")
                .SetType(BlockType.Torch)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetFullBlock(false)
                .SetLightEmission(10)
                .SetLightOpacity(0)
                .SetBoundingBox(0.35f, 0, 0.35f, 0.65f, 0.6f, 0.65f)
                .SetTexture("soul_torch")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            // ========================================
            // 梯子
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_LADDER, "ladder")
                .SetType(BlockType.Ladder)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(0.4f)
                .SetResistance(0.4f)
                .SetTool(ToolType.Axe)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetFullBlock(false)
                .SetFlammable(true, 30, 60)
                .SetTexture("ladder")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            // ========================================
            // 工作台/熔炉/箱子
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_CRAFTING_TABLE, "crafting_table")
                .SetType(BlockType.Crafting)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(2.5f)
                .SetResistance(2.5f)
                .SetTool(ToolType.Axe)
                .SetFlammable(true, 30, 60)
                .SetTexture("crafting_table_top", "crafting_table_bottom", "crafting_table_side")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            Register(new BlockInfo(GameConstants.BLOCK_FURNACE, "furnace")
                .SetType(BlockType.Furnace)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(3.5f)
                .SetResistance(3.5f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("furnace_top", "furnace_top", "furnace_side")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_CHEST, "chest")
                .SetType(BlockType.Chest)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(2.5f)
                .SetResistance(2.5f)
                .SetTool(ToolType.Axe)
                .SetFlammable(true, 30, 60)
                .SetTexture("chest")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            // ========================================
            // 黑曜石/发光方块
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_OBSIDIAN, "obsidian")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Obsidian)
                .SetHardness(50)
                .SetResistance(1200)
                .SetTool(ToolType.Pickaxe, ToolTier.Diamond)
                .SetTexture("obsidian")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_CRYING_OBSIDIAN, "crying_obsidian")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Obsidian)
                .SetHardness(50)
                .SetResistance(1200)
                .SetTool(ToolType.Pickaxe, ToolTier.Diamond)
                .SetLightEmission(10)
                .SetTexture("crying_obsidian")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_GLOWSTONE, "glowstone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Glowstone)
                .SetHardness(0.3f)
                .SetResistance(0.3f)
                .SetTool(ToolType.Pickaxe)
                .SetLightEmission(15)
                .SetTransparent(true)
                .SetTexture("glowstone")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SEA_LANTERN, "sea_lantern")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Glass)
                .SetHardness(0.3f)
                .SetResistance(0.3f)
                .SetTool(ToolType.Pickaxe)
                .SetLightEmission(15)
                .SetTransparent(true)
                .SetTexture("sea_lantern")
                .SetSounds("block.glass.break", "block.glass.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SHROOMLIGHT, "shroomlight")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Glowstone)
                .SetHardness(0.3f)
                .SetResistance(0.3f)
                .SetTool(ToolType.Hoe)
                .SetLightEmission(15)
                .SetTransparent(true)
                .SetTexture("shroomlight")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 下界方块
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_NETHERRACK, "netherrack")
                .SetType(BlockType.Netherrack)
                .SetMaterial(BlockMaterial.Netherrack)
                .SetHardness(0.4f)
                .SetResistance(0.4f)
                .SetTool(ToolType.Pickaxe)
                .SetFlammable(true, 30, 60)
                .SetTexture("netherrack")
                .SetSounds("block.netherrack.break", "block.netherrack.place", "block.netherrack.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SOUL_SAND, "soul_sand")
                .SetType(BlockType.SoulSand)
                .SetMaterial(BlockMaterial.SoulSand)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetSpeedFactor(0.4f)
                .SetTexture("soul_sand")
                .SetSounds("block.soul_sand.break", "block.soul_sand.place", "block.soul_sand.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SOUL_SOIL, "soul_soil")
                .SetType(BlockType.SoulSand)
                .SetMaterial(BlockMaterial.SoulSand)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("soul_soil")
                .SetSounds("block.soul_sand.break", "block.soul_sand.place", "block.soul_sand.step"));

            Register(new BlockInfo(GameConstants.BLOCK_NETHER_BRICKS, "nether_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Netherrack)
                .SetHardness(2.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("nether_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_RED_NETHER_BRICKS, "red_nether_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Netherrack)
                .SetHardness(2.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("red_nether_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_NETHER_WART_BLOCK, "nether_wart_block")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Netherrack)
                .SetHardness(1.0f)
                .SetResistance(1.0f)
                .SetTool(ToolType.Hoe)
                .SetTexture("nether_wart_block")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_QUARTZ_BLOCK, "quartz_block")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Quartz)
                .SetHardness(0.8f)
                .SetResistance(0.8f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("quartz_block")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 末地方块
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_END_STONE, "end_stone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.EndStone)
                .SetHardness(3.0f)
                .SetResistance(9.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("end_stone")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_END_BRICKS, "end_bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.EndStone)
                .SetHardness(3.0f)
                .SetResistance(9.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("end_bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_PURPUR_BLOCK, "purpur_block")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.EndStone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("purpur_block")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 深板岩/凝灰岩/方解石
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_DEEPSLATE, "deepslate")
                .SetType(BlockType.Deepslate)
                .SetMaterial(BlockMaterial.Deepslate)
                .SetHardness(3.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("deepslate")
                .SetSounds("block.deepslate.break", "block.deepslate.place", "block.deepslate.step"));

            Register(new BlockInfo(GameConstants.BLOCK_COBBLED_DEEPSLATE, "cobbled_deepslate")
                .SetType(BlockType.Deepslate)
                .SetMaterial(BlockMaterial.Deepslate)
                .SetHardness(3.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("cobbled_deepslate")
                .SetSounds("block.deepslate.break", "block.deepslate.place", "block.deepslate.step"));

            Register(new BlockInfo(GameConstants.BLOCK_POLISHED_DEEPSLATE, "polished_deepslate")
                .SetType(BlockType.Deepslate)
                .SetMaterial(BlockMaterial.Deepslate)
                .SetHardness(3.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("polished_deepslate")
                .SetSounds("block.deepslate.break", "block.deepslate.place", "block.deepslate.step"));

            Register(new BlockInfo(GameConstants.BLOCK_DEEPSLATE_BRICKS, "deepslate_bricks")
                .SetType(BlockType.Deepslate)
                .SetMaterial(BlockMaterial.Deepslate)
                .SetHardness(3.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("deepslate_bricks")
                .SetSounds("block.deepslate.break", "block.deepslate.place", "block.deepslate.step"));

            Register(new BlockInfo(GameConstants.BLOCK_TUFF, "tuff")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("tuff")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_CALCITE, "calcite")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(0.75f)
                .SetResistance(0.75f)
                .SetTool(ToolType.Pickaxe)
                .SetTexture("calcite")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 铜块
            // ========================================
            RegisterCopperBlocks();

            // ========================================
            // 紫水晶
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_AMETHYST_BLOCK, "amethyst_block")
                .SetType(BlockType.Amethyst)
                .SetMaterial(BlockMaterial.Amethyst)
                .SetHardness(1.5f)
                .SetResistance(1.5f)
                .SetTool(ToolType.Pickaxe, ToolTier.Iron)
                .SetTransparent(true)
                .SetTexture("amethyst_block")
                .SetSounds("block.amethyst.break", "block.amethyst.place", "block.amethyst.step"));

            Register(new BlockInfo(GameConstants.BLOCK_BUDDING_AMETHYST, "budding_amethyst")
                .SetType(BlockType.Amethyst)
                .SetMaterial(BlockMaterial.Amethyst)
                .SetHardness(1.5f)
                .SetResistance(1.5f)
                .SetTool(ToolType.Pickaxe, ToolTier.Iron)
                .SetTransparent(true)
                .SetTexture("budding_amethyst")
                .SetSounds("block.amethyst.break", "block.amethyst.place", "block.amethyst.step"));

            // ========================================
            // 泥巴/红树
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_MUD, "mud")
                .SetType(BlockType.Mud)
                .SetMaterial(BlockMaterial.Mud)
                .SetHardness(0.5f)
                .SetResistance(0.5f)
                .SetTool(ToolType.Shovel)
                .SetTexture("mud")
                .SetSounds("block.mud.break", "block.mud.place", "block.mud.step"));

            Register(new BlockInfo(GameConstants.BLOCK_MUD_BRICKS, "mud_bricks")
                .SetType(BlockType.Mud)
                .SetMaterial(BlockMaterial.Mud)
                .SetHardness(1.5f)
                .SetResistance(3.0f)
                .SetTool(ToolType.Pickaxe)
                .SetTexture("mud_bricks")
                .SetSounds("block.mud.break", "block.mud.place", "block.mud.step"));

            Register(new BlockInfo(GameConstants.BLOCK_PACKED_MUD, "packed_mud")
                .SetType(BlockType.Mud)
                .SetMaterial(BlockMaterial.Mud)
                .SetHardness(1.0f)
                .SetResistance(1.0f)
                .SetTool(ToolType.Pickaxe)
                .SetTexture("packed_mud")
                .SetSounds("block.mud.break", "block.mud.place", "block.mud.step"));

            // ========================================
            // 苔藓/杜鹃
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_MOSS_BLOCK, "moss_block")
                .SetType(BlockType.Plant)
                .SetMaterial(BlockMaterial.Plants)
                .SetHardness(0.1f)
                .SetResistance(0.1f)
                .SetTool(ToolType.Hoe)
                .SetTexture("moss_block")
                .SetSounds("block.moss.break", "block.moss.place", "block.moss.step"));

            Register(new BlockInfo(GameConstants.BLOCK_MOSS_CARPET, "moss_carpet")
                .SetType(BlockType.Plant)
                .SetMaterial(BlockMaterial.Plants)
                .SetHardness(0.1f)
                .SetResistance(0.1f)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetFullBlock(false)
                .SetBoundingBox(0, 0, 0, 1, 0.0625f, 1)
                .SetTexture("moss_carpet")
                .SetSounds("block.moss.break", "block.moss.place", "block.moss.step"));

            Register(new BlockInfo(GameConstants.BLOCK_AZALEA, "azalea")
                .SetType(BlockType.Plant)
                .SetMaterial(BlockMaterial.Plants)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetFullBlock(false)
                .SetTexture("azalea")
                .SetSounds("block.wood.break", "block.wood.place", "block.grass.step"));

            Register(new BlockInfo(GameConstants.BLOCK_FLOWERING_AZALEA, "flowering_azalea")
                .SetType(BlockType.Plant)
                .SetMaterial(BlockMaterial.Plants)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetFullBlock(false)
                .SetTexture("flowering_azalea")
                .SetSounds("block.wood.break", "block.wood.place", "block.grass.step"));

            // ========================================
            // 幽匿系列
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_SCULK, "sculk")
                .SetType(BlockType.Sculk)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(0.2f)
                .SetResistance(0.2f)
                .SetTool(ToolType.Hoe)
                .SetLightEmission(0)
                .SetTexture("sculk")
                .SetSounds("block.sculk.break", "block.sculk.place", "block.sculk.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SCULK_CATALYST, "sculk_catalyst")
                .SetType(BlockType.Sculk)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(3.0f)
                .SetResistance(3.0f)
                .SetTool(ToolType.Hoe)
                .SetLightEmission(6)
                .SetTexture("sculk_catalyst")
                .SetSounds("block.sculk.break", "block.sculk.place", "block.sculk.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SCULK_SENSOR, "sculk_sensor")
                .SetType(BlockType.Sculk)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(1.5f)
                .SetResistance(1.5f)
                .SetTool(ToolType.Hoe)
                .SetLightEmission(1)
                .SetTexture("sculk_sensor")
                .SetSounds("block.sculk.break", "block.sculk.place", "block.sculk.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SCULK_SHRIEKER, "sculk_shrieker")
                .SetType(BlockType.Sculk)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(3.0f)
                .SetResistance(3.0f)
                .SetTool(ToolType.Hoe)
                .SetLightEmission(6)
                .SetTexture("sculk_shrieker")
                .SetSounds("block.sculk.break", "block.sculk.place", "block.sculk.step"));

            // ========================================
            // TNT/火
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_TNT, "tnt")
                .SetType(BlockType.TNT)
                .SetMaterial(BlockMaterial.TNT)
                .SetHardness(0)
                .SetResistance(0)
                .SetFlammable(true, 100, 100)
                .SetTexture("tnt_top", "tnt_bottom", "tnt_side")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));

            Register(new BlockInfo(GameConstants.BLOCK_FIRE, "fire")
                .SetType(BlockType.Fire)
                .SetMaterial(BlockMaterial.Fire)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetReplaceable(true)
                .SetLightEmission(15)
                .SetFullBlock(false)
                .SetTexture("fire")
                .SetSounds("block.fire.break", "block.fire.place", "block.fire.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SOUL_FIRE, "soul_fire")
                .SetType(BlockType.Fire)
                .SetMaterial(BlockMaterial.Fire)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetReplaceable(true)
                .SetLightEmission(10)
                .SetFullBlock(false)
                .SetTexture("soul_fire")
                .SetSounds("block.fire.break", "block.fire.place", "block.fire.step"));

            // ========================================
            // 仙人掌/藤蔓
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_CACTUS, "cactus")
                .SetType(BlockType.Cactus)
                .SetMaterial(BlockMaterial.Cactus)
                .SetHardness(0.4f)
                .SetResistance(0.4f)
                .SetSolid(false)
                .SetFullBlock(false)
                .SetBoundingBox(0.0625f, 0, 0.0625f, 0.9375f, 1, 0.9375f)
                .SetTexture("cactus_top", "cactus_top", "cactus_side")
                .SetSounds("block.cloth.break", "block.cloth.place", "block.cloth.step"));

            Register(new BlockInfo(GameConstants.BLOCK_VINES, "vines")
                .SetType(BlockType.Vine)
                .SetMaterial(BlockMaterial.Vine)
                .SetHardness(0.2f)
                .SetResistance(0.2f)
                .SetTool(ToolType.Shears)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetFullBlock(false)
                .SetFlammable(true, 100, 100)
                .SetTexture("vines")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));

            // ========================================
            // 蜘蛛网
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_COBWEB, "cobweb")
                .SetType(BlockType.Cobweb)
                .SetMaterial(BlockMaterial.Web)
                .SetHardness(4.0f)
                .SetResistance(4.0f)
                .SetTool(ToolType.Sword)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetSpeedFactor(0.25f)
                .SetTexture("cobweb")
                .SetSounds("block.cloth.break", "block.cloth.place", "block.cloth.step"));

            // ========================================
            // 海绵
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_SPONGE, "sponge")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Sponge)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTexture("sponge")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));

            Register(new BlockInfo(GameConstants.BLOCK_WET_SPONGE, "wet_sponge")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Sponge)
                .SetHardness(0.6f)
                .SetResistance(0.6f)
                .SetTexture("wet_sponge")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));

            // ========================================
            // 砖块/陶瓦
            // ========================================
            Register(new BlockInfo(GameConstants.BLOCK_BRICKS, "bricks")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(2.0f)
                .SetResistance(6.0f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("bricks")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_SANDSTONE, "sandstone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(0.8f)
                .SetResistance(0.8f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("sandstone_top", "sandstone_bottom", "sandstone_side")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            Register(new BlockInfo(GameConstants.BLOCK_RED_SANDSTONE, "red_sandstone")
                .SetType(BlockType.Stone)
                .SetMaterial(BlockMaterial.Stone)
                .SetHardness(0.8f)
                .SetResistance(0.8f)
                .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                .SetTexture("red_sandstone_top", "red_sandstone_bottom", "red_sandstone_side")
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));

            // ========================================
            // 羊毛（16色）
            // ========================================
            string[] woolColors = { "white", "orange", "magenta", "light_blue", "yellow", "lime", "pink", "gray", "light_gray", "cyan", "purple", "blue", "brown", "green", "red", "black" };
            for (int i = 0; i < woolColors.Length && GameConstants.BLOCK_WHITE_WOOL + i < 256; i++)
            {
                Register(new BlockInfo((ushort)(GameConstants.BLOCK_WHITE_WOOL + i), $"{woolColors[i]}_wool")
                    .SetType(BlockType.Stone)
                    .SetMaterial(BlockMaterial.Wool)
                    .SetHardness(0.8f)
                    .SetResistance(0.8f)
                    .SetTool(ToolType.Shears)
                    .SetFlammable(true, 30, 60)
                    .SetTexture($"{woolColors[i]}_wool")
                    .SetSounds("block.cloth.break", "block.cloth.place", "block.cloth.step"));
            }

            // ========================================
            // 混凝土（16色）
            // ========================================
            for (int i = 0; i < woolColors.Length && GameConstants.BLOCK_WHITE_CONCRETE + i < 256; i++)
            {
                Register(new BlockInfo((ushort)(GameConstants.BLOCK_WHITE_CONCRETE + i), $"{woolColors[i]}_concrete")
                    .SetType(BlockType.Stone)
                    .SetMaterial(BlockMaterial.Concrete)
                    .SetHardness(1.8f)
                    .SetResistance(1.8f)
                    .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                    .SetTexture($"{woolColors[i]}_concrete")
                    .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));
            }

            // ========================================
            // 陶瓦（16色）
            // ========================================
            for (int i = 0; i < woolColors.Length && GameConstants.BLOCK_WHITE_TERRACOTTA + i < 256; i++)
            {
                Register(new BlockInfo((ushort)(GameConstants.BLOCK_WHITE_TERRACOTTA + i), $"{woolColors[i]}_terracotta")
                    .SetType(BlockType.Stone)
                    .SetMaterial(BlockMaterial.Terracotta)
                    .SetHardness(1.25f)
                    .SetResistance(4.2f)
                    .SetTool(ToolType.Pickaxe, ToolTier.Wood)
                    .SetTexture($"{woolColors[i]}_terracotta")
                    .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));
            }

            Console.WriteLine($"[BlockRegistry] 注册了 {blocks.Count} 个方块");
        }

        private static void Register(BlockInfo block)
        {
            blocks[block.Id] = block;
            nameToId[block.Name] = block.Id;
        }

        private static void RegisterOre(ushort id, string name, float hardness, float resistance, int dropItem, int expMin, int expMax)
        {
            Register(new BlockInfo(id, name)
                .SetType(BlockType.Ore)
                .SetMaterial(BlockMaterial.Ore)
                .SetHardness(hardness)
                .SetResistance(resistance)
                .SetTool(ToolType.Pickaxe, ToolTier.Stone)
                .SetDrop(dropItem)
                .SetExperience(expMin, expMax)
                .SetTexture(name)
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));
        }

        private static void RegisterBlock(ushort id, string name, BlockType type, BlockMaterial material, float hardness, float resistance, string texture, ToolType tool, ToolTier tier)
        {
            Register(new BlockInfo(id, name)
                .SetType(type)
                .SetMaterial(material)
                .SetHardness(hardness)
                .SetResistance(resistance)
                .SetTool(tool, tier)
                .SetTexture(texture)
                .SetSounds("block.stone.break", "block.stone.place", "block.stone.step"));
        }

        private static void RegisterWoodBlocks(ushort logId, string woodType, ushort planksId)
        {
            Register(new BlockInfo(logId, $"{woodType}_log")
                .SetType(BlockType.Wood)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(2.0f)
                .SetResistance(2.0f)
                .SetTool(ToolType.Axe)
                .SetFlammable(true, 30, 60)
                .SetTexture($"{woodType}_log_top", $"{woodType}_log_top", $"{woodType}_log")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));

            Register(new BlockInfo(planksId, $"{woodType}_planks")
                .SetType(BlockType.Wood)
                .SetMaterial(BlockMaterial.Wood)
                .SetHardness(2.0f)
                .SetResistance(2.0f)
                .SetTool(ToolType.Axe)
                .SetFlammable(true, 30, 60)
                .SetTexture($"{woodType}_planks")
                .SetSounds("block.wood.break", "block.wood.place", "block.wood.step"));
        }

        private static void RegisterLeaves(ushort id, string woodType)
        {
            Register(new BlockInfo(id, $"{woodType}_leaves")
                .SetType(BlockType.Leaves)
                .SetMaterial(BlockMaterial.Leaves)
                .SetHardness(0.2f)
                .SetResistance(0.2f)
                .SetTool(ToolType.Hoe)
                .SetTransparent(true)
                .SetLightOpacity(1)
                .SetFlammable(true, 30, 60)
                .SetTexture($"{woodType}_leaves")
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));
        }

        private static void RegisterPlant(ushort id, string name, BlockType type)
        {
            Register(new BlockInfo(id, name)
                .SetType(type)
                .SetMaterial(BlockMaterial.Plants)
                .SetHardness(0)
                .SetResistance(0)
                .SetSolid(false)
                .SetOpaque(false)
                .SetTransparent(true)
                .SetCollidable(false)
                .SetReplaceable(true)
                .SetFullBlock(false)
                .SetBoundingBox(0.1f, 0, 0.1f, 0.9f, 0.8f, 0.9f)
                .SetTexture(name)
                .SetSounds("block.grass.break", "block.grass.place", "block.grass.step"));
        }

        private static void RegisterCopperBlocks()
        {
            string[] copperStages = { "copper", "exposed_copper", "weathered_copper", "oxidized_copper" };
            string[] waxedStages = { "waxed_copper", "waxed_exposed_copper", "waxed_weathered_copper", "waxed_oxidized_copper" };

            for (int i = 0; i < 4; i++)
            {
                ushort id = (ushort)(GameConstants.BLOCK_COPPER_BLOCK + i);
                if (id < 256)
                {
                    Register(new BlockInfo(id, copperStages[i])
                        .SetType(BlockType.Copper)
                        .SetMaterial(BlockMaterial.Copper)
                        .SetHardness(3.0f)
                        .SetResistance(6.0f)
                        .SetTool(ToolType.Pickaxe, ToolTier.Stone)
                        .SetTexture(copperStages[i])
                        .SetSounds("block.copper.break", "block.copper.place", "block.copper.step"));
                }
            }
        }

        public static BlockInfo GetBlockInfo(ushort id)
        {
            if (blocks.TryGetValue(id, out var block))
            {
                return block;
            }
            return blocks[GameConstants.BLOCK_AIR];
        }

        public static BlockInfo GetBlockInfo(string name)
        {
            if (nameToId.TryGetValue(name, out var id))
            {
                return GetBlockInfo(id);
            }
            return blocks[GameConstants.BLOCK_AIR];
        }

        public static ushort GetBlockId(string name)
        {
            if (nameToId.TryGetValue(name, out var id))
            {
                return id;
            }
            return GameConstants.BLOCK_AIR;
        }

        public static string GetBlockName(ushort id)
        {
            if (blocks.TryGetValue(id, out var block))
            {
                return block.Name;
            }
            return "air";
        }

        public static bool IsBlockRegistered(ushort id)
        {
            return blocks.ContainsKey(id);
        }

        public static bool IsBlockRegistered(string name)
        {
            return nameToId.ContainsKey(name);
        }

        public static Dictionary<ushort, BlockInfo>.ValueCollection GetAllBlocks()
        {
            return blocks.Values;
        }

        public static int BlockCount => blocks.Count;
    }
}
