using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.World
{
    public class BlockInfo
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public string TranslationKey { get; set; }
        public BlockType Type { get; set; }
        public BlockMaterial Material { get; set; }

        // 娓叉煋灞炴€?        public bool IsSolid { get; set; } = true;
        public bool IsOpaque { get; set; } = true;
        public bool IsTransparent { get; set; } = false;
        public bool IsLiquid { get; set; } = false;
        public bool IsFlammable { get; set; } = false;
        public bool IsReplaceable { get; set; } = false;
        public bool HasGravity { get; set; } = false;
        public bool IsFullBlock { get; set; } = true;
        public bool EmitsLight { get; set; } = false;
        public byte LightEmission { get; set; } = 0;
        public byte LightOpacity { get; set; } = 15;

        // 物理属性
        public float Hardness { get; set; } = 1.0f;
        public float Resistance { get; set; } = 1.0f;
        public float Slipperiness { get; set; } = 0.6f;
        public float SpeedFactor { get; set; } = 1.0f;
        public float JumpFactor { get; set; } = 1.0f;

        // 掉落物
        public int DropItem { get; set; } = GameConstants.ITEM_AIR;
        public int DropCountMin { get; set; } = 1;
        public int DropCountMax { get; set; } = 1;
        public int ExperienceMin { get; set; } = 0;
        public int ExperienceMax { get; set; } = 0;

        // 宸ュ叿绫诲瀷
        public ToolType RequiredTool { get; set; } = ToolType.None;
        public ToolTier MinimumTier { get; set; } = ToolTier.Wood;

        // 绾圭悊
        public string TextureTop { get; set; }
        public string TextureBottom { get; set; }
        public string TextureSide { get; set; }
        public string TextureAll { get; set; }

        // 颜色（用于没有纹理时）
        public Vector4 ColorTop { get; set; } = Vector4.One;
        public Vector4 ColorBottom { get; set; } = Vector4.One;
        public Vector4 ColorSide { get; set; } = Vector4.One;

        // 方块状态
        public int MaxMetadata { get; set; } = 0;
        public bool HasBlockEntity { get; set; } = false;
        public bool IsTickable { get; set; } = false;
        public int TickRate { get; set; } = 10;

        // 澹伴煶
        public string BreakSound { get; set; } = "block.stone.break";
        public string PlaceSound { get; set; } = "block.stone.place";
        public string StepSound { get; set; } = "block.stone.step";
        public string HitSound { get; set; } = "block.stone.hit";
        public string FallSound { get; set; } = "block.stone.fall";

        // 绮掑瓙
        public int BreakParticleCount { get; set; } = 8;
        public bool SpawnBreakParticles { get; set; } = true;

        // 鐕冪儳
        public int BurnChance { get; set; } = 0;
        public int Encouragement { get; set; } = 0;

        // 鍏朵粬
        public bool IsSolid { get; set; } = true;
        public bool IsCollidable { get; set; } = true;
        public bool IsSelectable { get; set; } = true;
        public float BoundingBoxMinY { get; set; } = 0f;
        public float BoundingBoxMaxY { get; set; } = 1f;
        public float BoundingBoxMinX { get; set; } = 0f;
        public float BoundingBoxMaxX { get; set; } = 1f;
        public float BoundingBoxMinZ { get; set; } = 0f;
        public float BoundingBoxMaxZ { get; set; } = 1f;

        public BlockInfo(ushort id, string name)
        {
            Id = id;
            Name = name;
            TranslationKey = "block." + name;
            TextureAll = name;
        }

        public BlockInfo SetSelectable(bool selectable)
        {
            IsSelectable = selectable;
            return this;
        }

        public BlockInfo SetType(BlockType type)
        {
            Type = type;
            return this;
        }

        public BlockInfo SetMaterial(BlockMaterial material)
        {
            Material = material;
            return this;
        }

        public BlockInfo SetSolid(bool solid)
        {
            IsSolid = solid;
            return this;
        }

        public BlockInfo SetOpaque(bool opaque)
        {
            IsOpaque = opaque;
            IsTransparent = !opaque;
            return this;
        }

        public BlockInfo SetTransparent(bool transparent)
        {
            IsTransparent = transparent;
            IsOpaque = !transparent;
            return this;
        }

        public BlockInfo SetLiquid(bool liquid)
        {
            IsLiquid = liquid;
            IsSolid = !liquid;
            IsOpaque = false;
            IsTransparent = true;
            IsCollidable = false;
            return this;
        }

        public BlockInfo SetHardness(float hardness)
        {
            Hardness = hardness;
            return this;
        }

        public BlockInfo SetResistance(float resistance)
        {
            Resistance = resistance;
            return this;
        }

        public BlockInfo SetLightEmission(byte light)
        {
            LightEmission = light;
            EmitsLight = light > 0;
            return this;
        }

        public BlockInfo SetLightOpacity(byte opacity)
        {
            LightOpacity = opacity;
            return this;
        }

        public BlockInfo SetDrop(int itemId, int min = 1, int max = 1)
        {
            DropItem = itemId;
            DropCountMin = min;
            DropCountMax = max;
            return this;
        }

        public BlockInfo SetTexture(string all)
        {
            TextureAll = all;
            TextureTop = all;
            TextureBottom = all;
            TextureSide = all;
            return this;
        }

        public BlockInfo SetTexture(string top, string bottom, string side)
        {
            TextureTop = top;
            TextureBottom = bottom;
            TextureSide = side;
            return this;
        }

        public BlockInfo SetTool(ToolType tool, ToolTier tier = ToolTier.Wood)
        {
            RequiredTool = tool;
            MinimumTier = tier;
            return this;
        }

        public BlockInfo SetFlammable(bool flammable, int burnChance = 30, int encouragement = 60)
        {
            IsFlammable = flammable;
            BurnChance = flammable ? burnChance : 0;
            Encouragement = flammable ? encouragement : 0;
            return this;
        }

        public BlockInfo SetGravity(bool gravity)
        {
            HasGravity = gravity;
            return this;
        }

        public BlockInfo SetReplaceable(bool replaceable)
        {
            IsReplaceable = replaceable;
            return this;
        }

        public BlockInfo SetFullBlock(bool full)
        {
            IsFullBlock = full;
            return this;
        }

        public BlockInfo SetBoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            BoundingBoxMinX = minX;
            BoundingBoxMinY = minY;
            BoundingBoxMinZ = minZ;
            BoundingBoxMaxX = maxX;
            BoundingBoxMaxY = maxY;
            BoundingBoxMaxZ = maxZ;
            IsFullBlock = (minX == 0 && minY == 0 && minZ == 0 && maxX == 1 && maxY == 1 && maxZ == 1);
            return this;
        }

        public BlockInfo SetSounds(string breakSound, string placeSound, string stepSound)
        {
            BreakSound = breakSound;
            PlaceSound = placeSound;
            StepSound = stepSound;
            HitSound = breakSound;
            FallSound = stepSound;
            return this;
        }

        public string GetTexture(BlockFace face)
        {
            switch (face)
            {
                case BlockFace.Top:
                    return TextureTop ?? TextureAll;
                case BlockFace.Bottom:
                    return TextureBottom ?? TextureAll;
                case BlockFace.North:
                case BlockFace.South:
                case BlockFace.East:
                case BlockFace.West:
                    return TextureSide ?? TextureAll;
                default:
                    return TextureAll;
            }
        }

        public Vector4 GetColor(BlockFace face)
        {
            switch (face)
            {
                case BlockFace.Top:
                    return ColorTop;
                case BlockFace.Bottom:
                    return ColorBottom;
                default:
                    return ColorSide;
            }
        }

        public bool CanBeBrokenWith(ToolType tool, ToolTier tier)
        {
            if (RequiredTool == ToolType.None) return true;
            if (tool != RequiredTool) return false;
            return tier >= MinimumTier;
        }

        public float GetBreakTime(ToolType tool, ToolTier tier, float miningSpeed)
        {
            if (Hardness < 0) return float.MaxValue; // 涓嶅彲鐮村潖

            float baseTime = Hardness * 1.5f;

            if (CanBeBrokenWith(tool, tier))
            {
                baseTime /= miningSpeed;
            }
            else
            {
                baseTime *= 5f; // 娌℃湁鍚堥€傚伐鍏凤紝鎸栨帢閫熷害鍙樻參
            }

            return baseTime;
        }

        public int GetDropCount(Random random)
        {
            if (DropCountMin == DropCountMax) return DropCountMin;
            return random.Next(DropCountMin, DropCountMax + 1);
        }

        public int GetExperience(Random random)
        {
            if (ExperienceMin == ExperienceMax) return ExperienceMin;
            return random.Next(ExperienceMin, ExperienceMax + 1);
        }

        public override string ToString()
        {
            return $"Block[{Id}: {Name}]";
        }
    

        // 扩展属性和方法
        public string TextureFront { get; set; }
        public string TextureBack { get; set; }
        public string TextureLeft { get; set; }
        public string TextureRight { get; set; }
        public int Experience { get; set; }

        public BlockInfo SetCollidable(bool collidable)
        {
            IsCollidable = collidable;
            return this;
        }

        public BlockInfo SetExperience(int min, int max)
        {
            ExperienceMin = min;
            ExperienceMax = max;
            return this;
        }

        public BlockInfo SetSlipperiness(float value)
        {
            Slipperiness = value;
            return this;
        }

        public BlockInfo SetSpeedFactor(float value)
        {
            SpeedFactor = value;
            return this;
        }

    }

    // ========================================
    // 鏂瑰潡绫诲瀷
    // ========================================
    public enum BlockType
    {
        Air,
        Stone,
        Dirt,
        Grass,
        Wood,
        Leaves,
        Sand,
        Gravel,
        Ore,
        Glass,
        Liquid,
        Plant,
        Crop,
        Door,
        Trapdoor,
        Fence,
        Stairs,
        Slab,
        Button,
        PressurePlate,
        Torch,
        Ladder,
        Rail,
        Redstone,
        Crafting,
        Furnace,
        Chest,
        Sign,
        Bed,
        Anvil,
        Enchanting,
        Brewing,
        Beacon,
        EndPortal,
        NetherPortal,
        Spawner,
        TNT,
        Fire,
        Snow,
        Ice,
        Cactus,
        Vine,
        LilyPad,
        Reed,
        Mushroom,
        Flower,
        TallGrass,
        DeadBush,
        Cobweb,
        Cake,
        DragonEgg,
        Cocoa,
        Tripwire,
        TripwireHook,
        Piston,
        PistonHead,
        PistonExtension,
        Note,
        Jukebox,
        Comparator,
        Repeater,
        DaylightDetector,
        Hopper,
        Dropper,
        Dispenser,
        Observer,
        ShulkerBox,
        Banner,
        BedBlock,
        StructureBlock,
        CommandBlock,
        Barrier,
        StructureVoid,
        Jigsaw,
        Target,
        RespawnAnchor,
        CryingObsidian,
        AncientDebris,
        Basalt,
        Blackstone,
        Netherrack,
        SoulSand,
        SoulSoil,
        Glowstone,
        NetherWart,
        NetherBricks,
        Quartz,
        Prismarine,
        SeaLantern,
        EndStone,
        EndBricks,
        Purpur,
        Magma,
        BoneBlock,
        Shroomlight,
        SporeBlossom,
        Moss,
        MossCarpet,
        Azalea,
        FloweringAzalea,
        GlowLichen,
        Sculk,
        SculkCatalyst,
        SculkSensor,
        SculkShrieker,
        Deepslate,
        Copper,
        Amethyst,
        Tuff,
        Calcite,
        Dripstone,
        Mud,
        MudBricks,
        PackedMud,
        Mangrove,
        Cherry,
        Bamboo,
        ChiseledBookshelf,
        DecoratedPot,
        PiglinHead,
        SuspiciousSand,
        SuspiciousGravel,
        SnifferEgg,
        Torchflower,
        PitcherPlant,
        CalibratedSculkSensor,
        TrialSpawner,
        Vault,
        OmegaChest,
        BoggedSpawnEgg,
        Custom
    }

    // ========================================
    // 鏂瑰潡鏉愯川
    // ========================================
    public enum BlockMaterial
    {
        Air,
        Stone,
        Dirt,
        Grass,
        Wood,
        Leaves,
        Plants,
        Sand,
        Gravel,
        Ore,
        Glass,
        Water,
        Lava,
        Ice,
        Snow,
        Clay,
        Netherrack,
        SoulSand,
        Glowstone,
        EndStone,
        Anvil,
        Iron,
        Gold,
        Diamond,
        Netherite,
        Emerald,
        Lapis,
        Redstone,
        Coal,
        Copper,
        Quartz,
        Prismarine,
        Sponge,
        Wool,
        TNT,
        Fire,
        Cactus,
        Vine,
        Cake,
        Web,
        Piston,
        Shulker,
        Terracotta,
        Concrete,
        ConcretePowder,
        Amethyst,
        Deepslate,
        Mud,
        Mangrove,
        Cherry,
        Bamboo,
        DecoratedPot,
        Cloth,
        Ground,
        Metal,
        Obsidian,
        Rock,
        Custom
    }

    // ========================================
    // 宸ュ叿绫诲瀷
    // ========================================
    public enum ToolType
    {
        None,
        Pickaxe,
        Axe,
        Shovel,
        Hoe,
        Sword,
        Shears
    }

    // ========================================
    // 宸ュ叿绛夌骇
    // ========================================
    public enum ToolTier
    {
        Wood = 0,
        Gold = 1,
        Stone = 2,
        Iron = 3,
        Diamond = 4,
        Netherite = 5
    }


}

    // ========================================
    // 方块面
    // ========================================
    public enum BlockFace
    {
        Top = 0,
        Bottom = 1,
        North = 2,
        South = 3,
        East = 4,
        West = 5
    }
