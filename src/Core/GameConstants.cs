using System;
using OpenTK.Mathematics;

namespace VoxelCraft.Core
{
    public static class GameConstants
    {
        // ========================================
        // 游戏基本信息
        // ========================================
        public const string GAME_NAME = "VoxelCraft";
        public const string GAME_VERSION = "1.0.0";
        public const string GAME_AUTHOR = "panyuxing5";
        public const int TARGET_FPS = 60;
        public const int TARGET_UPS = 60;

        // ========================================
        // 窗口设置
        // ========================================
        public const int WINDOW_WIDTH = 1280;
        public const int WINDOW_HEIGHT = 720;
        public const string WINDOW_TITLE = "VoxelCraft - 3D体素沙盒游戏";
        public const bool VSYNC_ENABLED = true;
        public const int MAX_FPS = 240;

        // ========================================
        // 世界设置
        // ========================================
        public const int CHUNK_SIZE = 16;
        public const int CHUNK_HEIGHT = 256;
        public const int WORLD_HEIGHT = 256;
        public const int SEA_LEVEL = 64;
        public const int BEDROCK_LEVEL = 0;
        public const int MAX_BUILD_HEIGHT = 255;
        public const int RENDER_DISTANCE = 8;
        public const int SERVER_RENDER_DISTANCE = 10;
        public const int WORLD_SIZE_LIMIT = 30000000;
        public const long WORLD_SEED_DEFAULT = 1234567890;

        // ========================================
        // 方块设置
        // ========================================
        public const float BLOCK_SIZE = 1.0f;
        public const int MAX_BLOCK_ID = 256;
        public const int MAX_METADATA = 16;
        public const float BLOCK_HARDNESS_DEFAULT = 1.0f;
        public const float BLOCK_HARDNESS_UNBREAKABLE = -1.0f;
        public const float BLOCK_RESISTANCE_DEFAULT = 1.0f;
        public const float BLOCK_LIGHT_VALUE_MAX = 15;
        public const float BLOCK_LIGHT_SUN = 15;
        public const float BLOCK_LIGHT_TORCH = 14;
        public const float BLOCK_LIGHT_LAVA = 15;
        public const float BLOCK_LIGHT_GLOWSTONE = 15;

        // ========================================
        // 玩家设置
        // ========================================
        public const float PLAYER_WIDTH = 0.6f;
        public const float PLAYER_HEIGHT = 1.8f;
        public const float PLAYER_EYE_HEIGHT = 1.62f;
        public const float PLAYER_HEAD_HEIGHT = 0.5f;
        public const float PLAYER_MASS = 80.0f;
        public const float PLAYER_WALK_SPEED = 4.317f;
        public const float PLAYER_SPRINT_SPEED = 5.612f;
        public const float PLAYER_SNEAK_SPEED = 1.295f;
        public const float PLAYER_FLY_SPEED = 10.0f;
        public const float PLAYER_SPRINT_FLY_SPEED = 20.0f;
        public const float PLAYER_JUMP_FORCE = 8.0f;
        public const float PLAYER_GRAVITY = 25.0f;
        public const float PLAYER_TERMINAL_VELOCITY = 78.4f;
        public const float PLAYER_REACH_DISTANCE = 5.0f;
        public const float PLAYER_CREATIVE_REACH = 6.0f;
        public const float PLAYER_HEALTH_MAX = 20.0f;
        public const float PLAYER_HUNGER_MAX = 20.0f;
        public const float PLAYER_SATURATION_MAX = 20.0f;
        public const float PLAYER_EXHAUSTION_MAX = 4.0f;
        public const float PLAYER_HEALTH_REGEN_RATE = 0.5f;
        public const float PLAYER_HUNGER_DECAY_RATE = 0.005f;
        public const int PLAYER_HOTBAR_SIZE = 9;
        public const int PLAYER_INVENTORY_SIZE = 36;
        public const int PLAYER_ARMOR_SIZE = 4;
        public const int PLAYER_OFFHAND_SIZE = 1;
        public const float PLAYER_STEP_HEIGHT = 0.6f;
        public const float PLAYER_SNEAK_HEIGHT = 1.5f;
        public const float PLAYER_SNEAK_EYE_HEIGHT = 1.32f;
        public const float PLAYER_SWIM_SPEED = 2.0f;
        public const float PLAYER_CLIMB_SPEED = 2.35f;
        public const float PLAYER_FALL_DAMAGE_THRESHOLD = 3.0f;
        public const float PLAYER_FALL_DAMAGE_PER_BLOCK = 1.0f;

        // ========================================
        // 相机设置
        // ========================================
        public const float FOV_DEFAULT = 70.0f;
        public const float FOV_MIN = 30.0f;
        public const float FOV_MAX = 110.0f;
        public const float NEAR_PLANE = 0.1f;
        public const float FAR_PLANE = 1000.0f;
        public const float MOUSE_SENSITIVITY_DEFAULT = 0.002f;
        public const float MOUSE_SENSITIVITY_MIN = 0.0001f;
        public const float MOUSE_SENSITIVITY_MAX = 0.01f;
        public const float PITCH_MIN = -89.9f;
        public const float PITCH_MAX = 89.9f;

        // ========================================
        // 物理设置
        // ========================================
        public const float GRAVITY = 25.0f;
        public const float JUMP_VELOCITY = 8.0f;
        public const float TERMINAL_VELOCITY = 78.4f;
        public const float AIR_RESISTANCE = 0.98f;
        public const float WATER_RESISTANCE = 0.8f;
        public const float LAVA_RESISTANCE = 0.5f;
        public const float FRICTION_GROUND = 0.6f;
        public const float FRICTION_AIR = 0.99f;
        public const float FRICTION_ICE = 0.98f;
        public const float FRICTION_SLIME = 0.0f;
        public const float COLLISION_EPSILON = 0.001f;
        public const float MAX_COLLISION_ITERATIONS = 4;

        // ========================================
        // 光照设置
        // ========================================
        public const int LIGHT_LEVEL_MAX = 15;
        public const int LIGHT_LEVEL_MIN = 0;
        public const float LIGHT_ATTENUATION = 0.8f;
        public const float SKY_LIGHT_DAY = 1.0f;
        public const float SKY_LIGHT_NIGHT = 0.1f;
        public const float SKY_LIGHT_SUNSET = 0.5f;
        public const Vector3 SKY_COLOR_DAY = default;
        public const Vector3 SKY_COLOR_NIGHT = default;
        public const Vector3 SKY_COLOR_SUNSET = default;
        public const Vector3 FOG_COLOR_DAY = default;
        public const Vector3 FOG_COLOR_NIGHT = default;
        public const float FOG_DENSITY = 0.005f;
        public const float FOG_START = 50.0f;
        public const float FOG_END = 200.0f;

        // ========================================
        // 时间设置
        // ========================================
        public const int DAY_LENGTH_TICKS = 24000;
        public const int DAY_START_TICK = 0;
        public const int NOON_TICK = 6000;
        public const int SUNSET_TICK = 12000;
        public const int NIGHT_START_TICK = 13000;
        public const int MIDNIGHT_TICK = 18000;
        public const int SUNRISE_TICK = 23000;
        public const float TICK_RATE = 20.0f;
        public const float TICK_TIME = 1.0f / TICK_RATE;

        // ========================================
        // 生物设置
        // ========================================
        public const int MAX_ENTITIES_PER_CHUNK = 32;
        public const int MAX_ENTITIES_PER_WORLD = 1000;
        public const float ENTITY_RENDER_DISTANCE = 64.0f;
        public const float MOB_SPAWN_DISTANCE_MIN = 24.0f;
        public const float MOB_SPAWN_DISTANCE_MAX = 128.0f;
        public const float MOB_DESPAWN_DISTANCE = 128.0f;
        public const int MOB_SPAWN_ATTEMPTS_PER_TICK = 4;
        public const float ANIMAL_HEALTH_DEFAULT = 10.0f;
        public const float MONSTER_HEALTH_DEFAULT = 20.0f;
        public const float BOSS_HEALTH_DEFAULT = 200.0f;
        public const float MOB_DAMAGE_DEFAULT = 2.0f;
        public const float MOB_SPEED_DEFAULT = 1.0f;
        public const float MOB_FOLLOW_RANGE = 16.0f;
        public const float MOB_ATTACK_RANGE = 2.0f;
        public const float MOB_ATTACK_COOLDOWN = 1.0f;
        public const float MOB_KNOCKBACK_FORCE = 2.0f;

        // ========================================
        // 物品设置
        // ========================================
        public const int MAX_ITEM_ID = 1024;
        public const int MAX_STACK_SIZE_DEFAULT = 64;
        public const int MAX_STACK_SIZE_UNSTACKABLE = 1;
        public const int MAX_DAMAGE_DEFAULT = 0;
        public const int MAX_DAMAGE_TOOL = 256;
        public const int MAX_DAMAGE_WEAPON = 1561;
        public const int MAX_DAMAGE_ARMOR = 100;
        public const float ITEM_ENTITY_SIZE = 0.25f;
        public const float ITEM_ENTITY_LIFETIME = 6000.0f;
        public const float ITEM_ENTITY_PICKUP_DELAY = 0.5f;
        public const float ITEM_ENTITY_MERGE_DISTANCE = 0.5f;
        public const float ITEM_ENTITY_BOUNCE_FORCE = 0.3f;

        // ========================================
        // 合成设置
        // ========================================
        public const int CRAFTING_GRID_SIZE = 3;
        public const int FURNACE_SLOTS = 3;
        public const int BREWING_SLOTS = 5;
        public const int ENCHANTING_SLOTS = 2;
        public const float FURNACE_SMELT_TIME = 10.0f;
        public const float FURNACE_FUEL_TIME_COAL = 80.0f;
        public const float FURNACE_FUEL_TIME_WOOD = 15.0f;
        public const float FURNACE_FUEL_TIME_LAVA = 1000.0f;

        // ========================================
        // 存档设置
        // ========================================
        public const string SAVE_FOLDER_NAME = "saves";
        public const string WORLD_DATA_FILE = "level.dat";
        public const string PLAYER_DATA_FILE = "player.dat";
        public const string CHUNK_DATA_EXTENSION = ".mca";
        public const int CHUNK_REGION_SIZE = 32;
        public const int AUTOSAVE_INTERVAL_TICKS = 6000;
        public const bool COMPRESS_SAVE_DATA = true;
        public const int SAVE_COMPRESSION_LEVEL = 6;

        // ========================================
        // 网络设置
        // ========================================
        public const int DEFAULT_PORT = 25565;
        public const int MAX_PLAYERS = 20;
        public const int NETWORK_BUFFER_SIZE = 8192;
        public const float NETWORK_TICK_RATE = 20.0f;
        public const float NETWORK_TIMEOUT = 30.0f;
        public const int MAX_PACKET_SIZE = 1048576;

        // ========================================
        // 渲染设置
        // ========================================
        public const int MAX_VERTICES_PER_CHUNK = CHUNK_SIZE * CHUNK_SIZE * CHUNK_HEIGHT * 6 * 4;
        public const int MAX_INDICES_PER_CHUNK = CHUNK_SIZE * CHUNK_SIZE * CHUNK_HEIGHT * 6 * 6;
        public const bool USE_FACE_CULLING = true;
        public const bool USE_GREEDY_MESHING = true;
        public const bool USE_MIPMAP = true;
        public const int ANISOTROPIC_FILTERING = 16;
        public const int MSAA_SAMPLES = 4;
        public const bool USE_SHADOWS = true;
        public const int SHADOW_MAP_SIZE = 2048;
        public const float SHADOW_DISTANCE = 64.0f;
        public const bool USE_WATER_REFLECTION = false;
        public const bool USE_WATER_REFRACTION = false;
        public const bool USE_NORMAL_MAPPING = false;
        public const bool USE_PARALLAX_OCCLUSION = false;
        public const bool USE_SSAO = false;
        public const float SSAO_RADIUS = 0.5f;
        public const int SSAO_SAMPLES = 16;
        public const bool USE_BLOOM = false;
        public const float BLOOM_THRESHOLD = 1.0f;
        public const bool USE_TONE_MAPPING = true;
        public const float EXPOSURE = 1.0f;
        public const bool USE_VIGNETTE = false;
        public const float VIGNETTE_STRENGTH = 0.5f;
        public const bool USE_CHROMATIC_ABERRATION = false;
        public const float CHROMATIC_ABERRATION_STRENGTH = 0.001f;
        public const bool USE_MOTION_BLUR = false;
        public const float MOTION_BLUR_STRENGTH = 0.5f;
        public const bool USE_DEPTH_OF_FIELD = false;
        public const float DOF_FOCUS_DISTANCE = 10.0f;
        public const float DOF_APERTURE = 0.02f;

        // ========================================
        // 粒子设置
        // ========================================
        public const int MAX_PARTICLES = 10000;
        public const float PARTICLE_SIZE_DEFAULT = 0.1f;
        public const float PARTICLE_LIFETIME_DEFAULT = 1.0f;
        public const int BLOCK_BREAK_PARTICLES = 16;
        public const int BLOCK_PLACE_PARTICLES = 4;
        public const int RAIN_PARTICLES_PER_CHUNK = 100;
        public const int SNOW_PARTICLES_PER_CHUNK = 50;

        // ========================================
        // 音效设置
        // ========================================
        public const float MASTER_VOLUME_DEFAULT = 1.0f;
        public const float MUSIC_VOLUME_DEFAULT = 0.5f;
        public const float SOUND_VOLUME_DEFAULT = 1.0f;
        public const float AMBIENT_VOLUME_DEFAULT = 0.5f;
        public const float VOICE_VOLUME_DEFAULT = 1.0f;
        public const int MAX_SOUND_CHANNELS = 32;
        public const float SOUND_MIN_DISTANCE = 1.0f;
        public const float SOUND_MAX_DISTANCE = 64.0f;
        public const float SOUND_ROLLOFF_FACTOR = 1.0f;

        // ========================================
        // 调试设置
        // ========================================
        public const bool DEBUG_MODE = false;
        public const bool SHOW_FPS = true;
        public const bool SHOW_COORDINATES = true;
        public const bool SHOW_CHUNK_INFO = false;
        public const bool SHOW_MEMORY_USAGE = true;
        public const bool SHOW_RENDER_STATS = false;
        public const bool SHOW_COLLISION_BOXES = false;
        public const bool SHOW_ENTITY_PATHS = false;
        public const bool SHOW_LIGHT_LEVELS = false;
        public const bool ENABLE_PROFILER = false;
        public const int PROFILER_HISTORY_SIZE = 300;

        // ========================================
        // 颜色定义
        // ========================================
        public static readonly Vector4 COLOR_WHITE = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Vector4 COLOR_BLACK = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_RED = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_GREEN = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_BLUE = new Vector4(0.0f, 0.0f, 1.0f, 1.0f);
        public static readonly Vector4 COLOR_YELLOW = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_CYAN = new Vector4(0.0f, 1.0f, 1.0f, 1.0f);
        public static readonly Vector4 COLOR_MAGENTA = new Vector4(1.0f, 0.0f, 1.0f, 1.0f);
        public static readonly Vector4 COLOR_GRAY = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
        public static readonly Vector4 COLOR_DARK_GRAY = new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
        public static readonly Vector4 COLOR_LIGHT_GRAY = new Vector4(0.75f, 0.75f, 0.75f, 1.0f);
        public static readonly Vector4 COLOR_ORANGE = new Vector4(1.0f, 0.5f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_PURPLE = new Vector4(0.5f, 0.0f, 0.5f, 1.0f);
        public static readonly Vector4 COLOR_PINK = new Vector4(1.0f, 0.5f, 0.5f, 1.0f);
        public static readonly Vector4 COLOR_BROWN = new Vector4(0.5f, 0.25f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_LIME = new Vector4(0.5f, 1.0f, 0.0f, 1.0f);
        public static readonly Vector4 COLOR_LIGHT_BLUE = new Vector4(0.0f, 0.5f, 1.0f, 1.0f);
        public static readonly Vector4 COLOR_MAGENTA_DARK = new Vector4(0.5f, 0.0f, 0.5f, 1.0f);

        // ========================================
        // 方块ID定义
        // ========================================
        public const ushort BLOCK_AIR = 0;
        public const ushort BLOCK_STONE = 1;
        public const ushort BLOCK_GRASS = 2;
        public const ushort BLOCK_DIRT = 3;
        public const ushort BLOCK_COBBLESTONE = 4;
        public const ushort BLOCK_PLANKS = 5;
        public const ushort BLOCK_SAPLING = 6;
        public const ushort BLOCK_BEDROCK = 7;
        public const ushort BLOCK_WATER_FLOWING = 8;
        public const ushort BLOCK_WATER_STILL = 9;
        public const ushort BLOCK_LAVA_FLOWING = 10;
        public const ushort BLOCK_LAVA_STILL = 11;
        public const ushort BLOCK_SAND = 12;
        public const ushort BLOCK_GRAVEL = 13;
        public const ushort BLOCK_GOLD_ORE = 14;
        public const ushort BLOCK_IRON_ORE = 15;
        public const ushort BLOCK_COAL_ORE = 16;
        public const ushort BLOCK_LOG = 17;
        public const ushort BLOCK_LEAVES = 18;
        public const ushort BLOCK_SPONGE = 19;
        public const ushort BLOCK_GLASS = 20;
        public const ushort BLOCK_LAPIS_ORE = 21;
        public const ushort BLOCK_LAPIS_BLOCK = 22;
        public const ushort BLOCK_DISPENSER = 23;
        public const ushort BLOCK_SANDSTONE = 24;
        public const ushort BLOCK_NOTE_BLOCK = 25;
        public const ushort BLOCK_BED = 26;
        public const ushort BLOCK_POWERED_RAIL = 27;
        public const ushort BLOCK_DETECTOR_RAIL = 28;
        public const ushort BLOCK_STICKY_PISTON = 29;
        public const ushort BLOCK_COBWEB = 30;
        public const ushort BLOCK_TALL_GRASS = 31;
        public const ushort BLOCK_DEAD_BUSH = 32;
        public const ushort BLOCK_PISTON = 33;
        public const ushort BLOCK_PISTON_HEAD = 34;
        public const ushort BLOCK_WOOL = 35;
        public const ushort BLOCK_PISTON_EXTENSION = 36;
        public const ushort BLOCK_DANDELION = 37;
        public const ushort BLOCK_POPPY = 38;
        public const ushort BLOCK_BROWN_MUSHROOM = 39;
        public const ushort BLOCK_RED_MUSHROOM = 40;
        public const ushort BLOCK_GOLD_BLOCK = 41;
        public const ushort BLOCK_IRON_BLOCK = 42;
        public const ushort BLOCK_DOUBLE_SLAB = 43;
        public const ushort BLOCK_SLAB = 44;
        public const ushort BLOCK_BRICKS = 45;
        public const ushort BLOCK_TNT = 46;
        public const ushort BLOCK_BOOKSHELF = 47;
        public const ushort BLOCK_MOSSY_COBBLESTONE = 48;
        public const ushort BLOCK_OBSIDIAN = 49;
        public const ushort BLOCK_TORCH = 50;
        public const ushort BLOCK_FIRE = 51;
        public const ushort BLOCK_MOB_SPAWNER = 52;
        public const ushort BLOCK_OAK_STAIRS = 53;
        public const ushort BLOCK_CHEST = 54;
        public const ushort BLOCK_REDSTONE_WIRE = 55;
        public const ushort BLOCK_DIAMOND_ORE = 56;
        public const ushort BLOCK_DIAMOND_BLOCK = 57;
        public const ushort BLOCK_CRAFTING_TABLE = 58;
        public const ushort BLOCK_WHEAT = 59;
        public const ushort BLOCK_FARMLAND = 60;
        public const ushort BLOCK_FURNACE = 61;
        public const ushort BLOCK_FURNACE_LIT = 62;
        public const ushort BLOCK_STANDING_SIGN = 63;
        public const ushort BLOCK_WOODEN_DOOR = 64;
        public const ushort BLOCK_LADDER = 65;
        public const ushort BLOCK_RAIL = 66;
        public const ushort BLOCK_COBBLESTONE_STAIRS = 67;
        public const ushort BLOCK_WALL_SIGN = 68;
        public const ushort BLOCK_LEVER = 69;
        public const ushort BLOCK_STONE_PRESSURE_PLATE = 70;
        public const ushort BLOCK_IRON_DOOR = 71;
        public const ushort BLOCK_WOODEN_PRESSURE_PLATE = 72;
        public const ushort BLOCK_REDSTONE_ORE = 73;
        public const ushort BLOCK_REDSTONE_ORE_LIT = 74;
        public const ushort BLOCK_UNLIT_REDSTONE_TORCH = 75;
        public const ushort BLOCK_REDSTONE_TORCH = 76;
        public const ushort BLOCK_STONE_BUTTON = 77;
        public const ushort BLOCK_SNOW = 78;
        public const ushort BLOCK_ICE = 79;
        public const ushort BLOCK_SNOW_BLOCK = 80;
        public const ushort BLOCK_CACTUS = 81;
        public const ushort BLOCK_CLAY = 82;
        public const ushort BLOCK_SUGAR_CANE = 83;
        public const ushort BLOCK_JUKEBOX = 84;
        public const ushort BLOCK_FENCE = 85;
        public const ushort BLOCK_PUMPKIN = 86;
        public const ushort BLOCK_NETHERRACK = 87;
        public const ushort BLOCK_SOUL_SAND = 88;
        public const ushort BLOCK_GLOWSTONE = 89;
        public const ushort BLOCK_NETHER_PORTAL = 90;
        public const ushort BLOCK_JACK_O_LANTERN = 91;
        public const ushort BLOCK_CAKE = 92;
        public const ushort BLOCK_REPEATER = 93;
        public const ushort BLOCK_REPEATER_LIT = 94;
        public const ushort BLOCK_STAINED_GLASS = 95;
        public const ushort BLOCK_TRAPDOOR = 96;
        public const ushort BLOCK_STONE_BRICKS = 97;
        public const ushort BLOCK_MOSSY_STONE_BRICKS = 98;
        public const ushort BLOCK_CRACKED_STONE_BRICKS = 99;
        public const ushort BLOCK_CHISELED_STONE_BRICKS = 100;
        public const ushort BLOCK_MUSHROOM_STEM = 101;
        public const ushort BLOCK_MUSHROOM_BLOCK_RED = 102;
        public const ushort BLOCK_MUSHROOM_BLOCK_BROWN = 103;
        public const ushort BLOCK_IRON_BARS = 104;
        public const ushort BLOCK_GLASS_PANE = 105;
        public const ushort BLOCK_MELON = 106;
        public const ushort BLOCK_PUMPKIN_STEM = 107;
        public const ushort BLOCK_MELON_STEM = 108;
        public const ushort BLOCK_VINES = 109;
        public const ushort BLOCK_FENCE_GATE = 110;
        public const ushort BLOCK_BRICK_STAIRS = 111;
        public const ushort BLOCK_STONE_BRICK_STAIRS = 112;
        public const ushort BLOCK_MYCELIUM = 113;
        public const ushort BLOCK_LILY_PAD = 114;
        public const ushort BLOCK_NETHER_BRICKS = 115;
        public const ushort BLOCK_NETHER_BRICK_FENCE = 116;
        public const ushort BLOCK_NETHER_BRICK_STAIRS = 117;
        public const ushort BLOCK_NETHER_WART = 118;
        public const ushort BLOCK_ENCHANTING_TABLE = 119;
        public const ushort BLOCK_BREWING_STAND = 120;
        public const ushort BLOCK_CAULDRON = 121;
        public const ushort BLOCK_END_PORTAL = 122;
        public const ushort BLOCK_END_PORTAL_FRAME = 123;
        public const ushort BLOCK_END_STONE = 124;
        public const ushort BLOCK_ENDER_DRAGON_EGG = 125;
        public const ushort BLOCK_REDSTONE_LAMP = 126;
        public const ushort BLOCK_REDSTONE_LAMP_LIT = 127;
        public const ushort BLOCK_WOODEN_SLAB = 128;
        public const ushort BLOCK_SANDSTONE_STAIRS = 129;
        public const ushort BLOCK_EMERALD_ORE = 130;
        public const ushort BLOCK_ENDER_CHEST = 131;
        public const ushort BLOCK_TRIPWIRE_HOOK = 132;
        public const ushort BLOCK_TRIPWIRE = 133;
        public const ushort BLOCK_EMERALD_BLOCK = 134;
        public const ushort BLOCK_SPRUCE_STAIRS = 135;
        public const ushort BLOCK_BIRCH_STAIRS = 136;
        public const ushort BLOCK_JUNGLE_STAIRS = 137;
        public const ushort BLOCK_COMMAND_BLOCK = 138;
        public const ushort BLOCK_BEACON = 139;
        public const ushort BLOCK_COBBLESTONE_WALL = 140;
        public const ushort BLOCK_FLOWER_POT = 141;
        public const ushort BLOCK_CARROTS = 142;
        public const ushort BLOCK_POTATOES = 143;
        public const ushort BLOCK_WOODEN_BUTTON = 144;
        public const ushort BLOCK_MOB_HEAD = 145;
        public const ushort BLOCK_ANVIL = 146;
        public const ushort BLOCK_TRAPPED_CHEST = 147;
        public const ushort BLOCK_WEIGHTED_PRESSURE_PLATE_LIGHT = 148;
        public const ushort BLOCK_WEIGHTED_PRESSURE_PLATE_HEAVY = 149;
        public const ushort BLOCK_REDSTONE_COMPARATOR = 150;
        public const ushort BLOCK_REDSTONE_COMPARATOR_LIT = 151;
        public const ushort BLOCK_DAYLIGHT_DETECTOR = 152;
        public const ushort BLOCK_REDSTONE_BLOCK = 153;
        public const ushort BLOCK_QUARTZ_ORE = 154;
        public const ushort BLOCK_HOPPER = 155;
        public const ushort BLOCK_QUARTZ_BLOCK = 156;
        public const ushort BLOCK_QUARTZ_STAIRS = 157;
        public const ushort BLOCK_ACTIVATOR_RAIL = 158;
        public const ushort BLOCK_DROPPER = 159;
        public const ushort BLOCK_STAINED_CLAY = 160;
        public const ushort BLOCK_STAINED_GLASS_PANE = 161;
        public const ushort BLOCK_LEAVES2 = 162;
        public const ushort BLOCK_LOG2 = 163;
        public const ushort BLOCK_ACACIA_STAIRS = 164;
        public const ushort BLOCK_DARK_OAK_STAIRS = 165;
        public const ushort BLOCK_SLIME_BLOCK = 166;
        public const ushort BLOCK_BARRIER = 167;
        public const ushort BLOCK_IRON_TRAPDOOR = 168;
        public const ushort BLOCK_PRISMARINE = 169;
        public const ushort BLOCK_SEA_LANTERN = 170;
        public const ushort BLOCK_HAY_BLOCK = 171;
        public const ushort BLOCK_CARPET = 172;
        public const ushort BLOCK_HARDENED_CLAY = 173;
        public const ushort BLOCK_BLOCK_OF_COAL = 174;
        public const ushort BLOCK_PACKED_ICE = 175;
        public const ushort BLOCK_DOUBLE_PLANT = 176;
        public const ushort BLOCK_STANDING_BANNER = 177;
        public const ushort BLOCK_WALL_BANNER = 178;
        public const ushort BLOCK_SANDSTONE2 = 179;
        public const ushort BLOCK_RED_SANDSTONE = 180;
        public const ushort BLOCK_RED_SANDSTONE_STAIRS = 181;
        public const ushort BLOCK_DOUBLE_RED_SANDSTONE_SLAB = 182;
        public const ushort BLOCK_RED_SANDSTONE_SLAB = 183;
        public const ushort BLOCK_SPRUCE_FENCE_GATE = 184;
        public const ushort BLOCK_BIRCH_FENCE_GATE = 185;
        public const ushort BLOCK_JUNGLE_FENCE_GATE = 186;
        public const ushort BLOCK_DARK_OAK_FENCE_GATE = 187;
        public const ushort BLOCK_ACACIA_FENCE_GATE = 188;
        public const ushort BLOCK_SPRUCE_FENCE = 189;
        public const ushort BLOCK_BIRCH_FENCE = 190;
        public const ushort BLOCK_JUNGLE_FENCE = 191;
        public const ushort BLOCK_DARK_OAK_FENCE = 192;
        public const ushort BLOCK_ACACIA_FENCE = 193;
        public const ushort BLOCK_SPRUCE_DOOR = 194;
        public const ushort BLOCK_BIRCH_DOOR = 195;
        public const ushort BLOCK_JUNGLE_DOOR = 196;
        public const ushort BLOCK_ACACIA_DOOR = 197;
        public const ushort BLOCK_DARK_OAK_DOOR = 198;
        public const ushort BLOCK_END_ROD = 199;
        public const ushort BLOCK_CHORUS_PLANT = 200;
        public const ushort BLOCK_CHORUS_FLOWER = 201;
        public const ushort BLOCK_PURPUR_BLOCK = 202;
        public const ushort BLOCK_PURPUR_PILLAR = 203;
        public const ushort BLOCK_PURPUR_STAIRS = 204;
        public const ushort BLOCK_PURPUR_DOUBLE_SLAB = 205;
        public const ushort BLOCK_PURPUR_SLAB = 206;
        public const ushort BLOCK_END_BRICKS = 207;
        public const ushort BLOCK_BEETROOTS = 208;
        public const ushort BLOCK_GRASS_PATH = 209;
        public const ushort BLOCK_END_GATEWAY = 210;
        public const ushort BLOCK_REPEATING_COMMAND_BLOCK = 211;
        public const ushort BLOCK_CHAIN_COMMAND_BLOCK = 212;
        public const ushort BLOCK_FROSTED_ICE = 213;
        public const ushort BLOCK_MAGMA = 214;
        public const ushort BLOCK_NETHER_WART_BLOCK = 215;
        public const ushort BLOCK_RED_NETHER_BRICKS = 216;
        public const ushort BLOCK_BONE_BLOCK = 217;
        public const ushort BLOCK_OBSERVER = 218;
        public const ushort BLOCK_WHITE_SHULKER_BOX = 219;
        public const ushort BLOCK_ORANGE_SHULKER_BOX = 220;
        public const ushort BLOCK_MAGENTA_SHULKER_BOX = 221;
        public const ushort BLOCK_LIGHT_BLUE_SHULKER_BOX = 222;
        public const ushort BLOCK_YELLOW_SHULKER_BOX = 223;
        public const ushort BLOCK_LIME_SHULKER_BOX = 224;
        public const ushort BLOCK_PINK_SHULKER_BOX = 225;
        public const ushort BLOCK_GRAY_SHULKER_BOX = 226;
        public const ushort BLOCK_LIGHT_GRAY_SHULKER_BOX = 227;
        public const ushort BLOCK_CYAN_SHULKER_BOX = 228;
        public const ushort BLOCK_PURPLE_SHULKER_BOX = 229;
        public const ushort BLOCK_BLUE_SHULKER_BOX = 230;
        public const ushort BLOCK_BROWN_SHULKER_BOX = 231;
        public const ushort BLOCK_GREEN_SHULKER_BOX = 232;
        public const ushort BLOCK_RED_SHULKER_BOX = 233;
        public const ushort BLOCK_BLACK_SHULKER_BOX = 234;
        public const ushort BLOCK_WHITE_GLAZED_TERRACOTTA = 235;
        public const ushort BLOCK_ORANGE_GLAZED_TERRACOTTA = 236;
        public const ushort BLOCK_MAGENTA_GLAZED_TERRACOTTA = 237;
        public const ushort BLOCK_LIGHT_BLUE_GLAZED_TERRACOTTA = 238;
        public const ushort BLOCK_YELLOW_GLAZED_TERRACOTTA = 239;
        public const ushort BLOCK_LIME_GLAZED_TERRACOTTA = 240;
        public const ushort BLOCK_PINK_GLAZED_TERRACOTTA = 241;
        public const ushort BLOCK_GRAY_GLAZED_TERRACOTTA = 242;
        public const ushort BLOCK_LIGHT_GRAY_GLAZED_TERRACOTTA = 243;
        public const ushort BLOCK_CYAN_GLAZED_TERRACOTTA = 244;
        public const ushort BLOCK_PURPLE_GLAZED_TERRACOTTA = 245;
        public const ushort BLOCK_BLUE_GLAZED_TERRACOTTA = 246;
        public const ushort BLOCK_BROWN_GLAZED_TERRACOTTA = 247;
        public const ushort BLOCK_GREEN_GLAZED_TERRACOTTA = 248;
        public const ushort BLOCK_RED_GLAZED_TERRACOTTA = 249;
        public const ushort BLOCK_BLACK_GLAZED_TERRACOTTA = 250;
        public const ushort BLOCK_CONCRETE = 251;
        public const ushort BLOCK_CONCRETE_POWDER = 252;
        public const ushort BLOCK_CHORUS_FRUIT = 253;
        public const ushort BLOCK_PURPUR_DOOR = 254;
        public const ushort BLOCK_UNKNOWN = 255;

        // ========================================
        // 物品ID定义
        // ========================================
        public const int ITEM_AIR = 0;
        public const int ITEM_STONE_SWORD = 1;
        public const int ITEM_STONE_PICKAXE = 2;
        public const int ITEM_STONE_AXE = 3;
        public const int ITEM_STONE_SHOVEL = 4;
        public const int ITEM_STONE_HOE = 5;
        public const int ITEM_WOODEN_SWORD = 6;
        public const int ITEM_WOODEN_PICKAXE = 7;
        public const int ITEM_WOODEN_AXE = 8;
        public const int ITEM_WOODEN_SHOVEL = 9;
        public const int ITEM_WOODEN_HOE = 10;
        public const int ITEM_GOLDEN_SWORD = 11;
        public const int ITEM_GOLDEN_PICKAXE = 12;
        public const int ITEM_GOLDEN_AXE = 13;
        public const int ITEM_GOLDEN_SHOVEL = 14;
        public const int ITEM_GOLDEN_HOE = 15;
        public const int ITEM_IRON_SWORD = 16;
        public const int ITEM_IRON_PICKAXE = 17;
        public const int ITEM_IRON_AXE = 18;
        public const int ITEM_IRON_SHOVEL = 19;
        public const int ITEM_IRON_HOE = 20;
        public const int ITEM_DIAMOND_SWORD = 21;
        public const int ITEM_DIAMOND_PICKAXE = 22;
        public const int ITEM_DIAMOND_AXE = 23;
        public const int ITEM_DIAMOND_SHOVEL = 24;
        public const int ITEM_DIAMOND_HOE = 25;
        public const int ITEM_NETHERITE_SWORD = 26;
        public const int ITEM_NETHERITE_PICKAXE = 27;
        public const int ITEM_NETHERITE_AXE = 28;
        public const int ITEM_NETHERITE_SHOVEL = 29;
        public const int ITEM_NETHERITE_HOE = 30;
        public const int ITEM_BOW = 31;
        public const int ITEM_ARROW = 32;
        public const int ITEM_CROSSBOW = 33;
        public const int ITEM_TRIDENT = 34;
        public const int ITEM_SHIELD = 35;
        public const int ITEM_LEATHER_HELMET = 36;
        public const int ITEM_LEATHER_CHESTPLATE = 37;
        public const int ITEM_LEATHER_LEGGINGS = 38;
        public const int ITEM_LEATHER_BOOTS = 39;
        public const int ITEM_CHAINMAIL_HELMET = 40;
        public const int ITEM_CHAINMAIL_CHESTPLATE = 41;
        public const int ITEM_CHAINMAIL_LEGGINGS = 42;
        public const int ITEM_CHAINMAIL_BOOTS = 43;
        public const int ITEM_IRON_HELMET = 44;
        public const int ITEM_IRON_CHESTPLATE = 45;
        public const int ITEM_IRON_LEGGINGS = 46;
        public const int ITEM_IRON_BOOTS = 47;
        public const int ITEM_GOLDEN_HELMET = 48;
        public const int ITEM_GOLDEN_CHESTPLATE = 49;
        public const int ITEM_GOLDEN_LEGGINGS = 50;
        public const int ITEM_GOLDEN_BOOTS = 51;
        public const int ITEM_DIAMOND_HELMET = 52;
        public const int ITEM_DIAMOND_CHESTPLATE = 53;
        public const int ITEM_DIAMOND_LEGGINGS = 54;
        public const int ITEM_DIAMOND_BOOTS = 55;
        public const int ITEM_NETHERITE_HELMET = 56;
        public const int ITEM_NETHERITE_CHESTPLATE = 57;
        public const int ITEM_NETHERITE_LEGGINGS = 58;
        public const int ITEM_NETHERITE_BOOTS = 59;
        public const int ITEM_FLINT_AND_STEEL = 60;
        public const int ITEM_APPLE = 61;
        public const int ITEM_BOWL = 62;
        public const int ITEM_MUSHROOM_STEW = 63;
        public const int ITEM_GOLDEN_APPLE = 64;
        public const int ITEM_ENCHANTED_GOLDEN_APPLE = 65;
        public const int ITEM_BUCKET = 66;
        public const int ITEM_WATER_BUCKET = 67;
        public const int ITEM_LAVA_BUCKET = 68;
        public const int ITEM_MINECART = 69;
        public const int ITEM_SADDLE = 70;
        public const int ITEM_IRON_DOOR_ITEM = 71;
        public const int ITEM_REDSTONE = 72;
        public const int ITEM_SNOWBALL = 73;
        public const int ITEM_BOAT = 74;
        public const int ITEM_LEATHER = 75;
        public const int ITEM_KELP = 76;
        public const int ITEM_BRICK = 77;
        public const int ITEM_CLAY_BALL = 78;
        public const int ITEM_REEDS = 79;
        public const int ITEM_PAPER = 80;
        public const int ITEM_BOOK = 81;
        public const int ITEM_SLIME_BALL = 82;
        public const int ITEM_CHEST_MINECART = 83;
        public const int ITEM_FURNACE_MINECART = 84;
        public const int ITEM_EGG = 85;
        public const int ITEM_COMPASS = 86;
        public const int ITEM_FISHING_ROD = 87;
        public const int ITEM_CLOCK = 88;
        public const int ITEM_GLOWSTONE_DUST = 89;
        public const int ITEM_RAW_FISH = 90;
        public const int ITEM_COOKED_FISH = 91;
        public const int ITEM_DYE = 92;
        public const int ITEM_BONE = 93;
        public const int ITEM_SUGAR = 94;
        public const int ITEM_CAKE_ITEM = 95;
        public const int ITEM_BED_ITEM = 96;
        public const int ITEM_REPEATER_ITEM = 97;
        public const int ITEM_COOKIE = 98;
        public const int ITEM_MAP = 99;
        public const int ITEM_SHEARS = 100;
        public const int ITEM_MELON = 101;
        public const int ITEM_PUMPKIN_SEEDS = 102;
        public const int ITEM_MELON_SEEDS = 103;
        public const int ITEM_RAW_BEEF = 104;
        public const int ITEM_STEAK = 105;
        public const int ITEM_RAW_CHICKEN = 106;
        public const int ITEM_COOKED_CHICKEN = 107;
        public const int ITEM_ROTTEN_FLESH = 108;
        public const int ITEM_ENDER_PEARL = 109;
        public const int ITEM_BLAZE_ROD = 110;
        public const int ITEM_GHAST_TEAR = 111;
        public const int ITEM_GOLD_NUGGET = 112;
        public const int ITEM_NETHER_WART_ITEM = 113;
        public const int ITEM_POTION = 114;
        public const int ITEM_GLASS_BOTTLE = 115;
        public const int ITEM_SPIDER_EYE = 116;
        public const int ITEM_FERMENTED_SPIDER_EYE = 117;
        public const int ITEM_BLAZE_POWDER = 118;
        public const int ITEM_MAGMA_CREAM = 119;
        public const int ITEM_BREWING_STAND_ITEM = 120;
        public const int ITEM_CAULDRON_ITEM = 121;
        public const int ITEM_ENDER_EYE = 122;
        public const int ITEM_GLISTERING_MELON = 123;
        public const int ITEM_SPAWN_EGG = 124;
        public const int ITEM_EXPERIENCE_BOTTLE = 125;
        public const int ITEM_FIRE_CHARGE = 126;
        public const int ITEM_BOOK_AND_QUILL = 127;
        public const int ITEM_WRITTEN_BOOK = 128;
        public const int ITEM_EMERALD = 129;
        public const int ITEM_ITEM_FRAME = 130;
        public const int ITEM_FLOWER_POT_ITEM = 131;
        public const int ITEM_CARROT = 132;
        public const int ITEM_POTATO = 133;
        public const int ITEM_BAKED_POTATO = 134;
        public const int ITEM_POISONOUS_POTATO = 135;
        public const int ITEM_EMPTY_MAP = 136;
        public const int ITEM_GOLDEN_CARROT = 137;
        public const int ITEM_SKULL = 138;
        public const int ITEM_CARROT_ON_A_STICK = 139;
        public const int ITEM_NETHER_STAR = 140;
        public const int ITEM_PUMPKIN_PIE = 141;
        public const int ITEM_FIREWORK_ROCKET = 142;
        public const int ITEM_FIREWORK_STAR = 143;
        public const int ITEM_ENCHANTED_BOOK = 144;
        public const int ITEM_COMPARATOR = 145;
        public const int ITEM_NETHER_BRICK = 146;
        public const int ITEM_QUARTZ = 147;
        public const int ITEM_TNT_MINECART = 148;
        public const int ITEM_HOPPER_MINECART = 149;
        public const int ITEM_PRISMARINE_SHARD = 150;
        public const int ITEM_PRISMARINE_CRYSTALS = 151;
        public const int ITEM_RAW_MUTTON = 152;
        public const int ITEM_COOKED_MUTTON = 153;
        public const int ITEM_RAW_PORKCHOP = 154;
        public const int ITEM_COOKED_PORKCHOP = 155;
        public const int ITEM_ARMOR_STAND = 156;
        public const int ITEM_IRON_HORSE_ARMOR = 157;
        public const int ITEM_GOLDEN_HORSE_ARMOR = 158;
        public const int ITEM_DIAMOND_HORSE_ARMOR = 159;
        public const int ITEM_LEAD = 160;
        public const int ITEM_NAME_TAG = 161;
        public const int ITEM_RAW_RABBIT = 162;
        public const int ITEM_COOKED_RABBIT = 163;
        public const int ITEM_RABBIT_STEW = 164;
        public const int ITEM_RABBIT_FOOT = 165;
        public const int ITEM_RABBIT_HIDE = 166;
        public const int ITEM_LEATHER_HORSE_ARMOR = 167;
        public const int ITEM_LILY_PAD_ITEM = 168;
        public const int ITEM_END_CRYSTAL = 169;
        public const int ITEM_SPRUCE_DOOR_ITEM = 170;
        public const int ITEM_BIRCH_DOOR_ITEM = 171;
        public const int ITEM_JUNGLE_DOOR_ITEM = 172;
        public const int ITEM_ACACIA_DOOR_ITEM = 173;
        public const int ITEM_DARK_OAK_DOOR_ITEM = 174;
        public const int ITEM_CHORUS_FRUIT_ITEM = 175;
        public const int ITEM_POPPED_CHORUS_FRUIT = 176;
        public const int ITEM_BEETROOT = 177;
        public const int ITEM_BEETROOT_SOUP = 178;
        public const int ITEM_BEETROOT_SEEDS = 179;
        public const int ITEM_RAW_SALMON = 180;
        public const int ITEM_COOKED_SALMON = 181;
        public const int ITEM_CLOWNFISH = 182;
        public const int ITEM_PUFFERFISH = 183;
        public const int ITEM_BANNER_ITEM = 184;
        public const int ITEM_TOTEM_OF_UNDYING = 185;
        public const int ITEM_SHULKER_SHELL = 186;
        public const int ITEM_IRON_NUGGET = 187;
        public const int ITEM_KNOWLEDGE_BOOK = 188;
        public const int ITEM_DEBUG_STICK = 189;
        public const int ITEM_MUSIC_DISC_13 = 190;
        public const int ITEM_MUSIC_DISC_CAT = 191;
        public const int ITEM_MUSIC_DISC_BLOCKS = 192;
        public const int ITEM_MUSIC_DISC_CHIRP = 193;
        public const int ITEM_MUSIC_DISC_FAR = 194;
        public const int ITEM_MUSIC_DISC_MALL = 195;
        public const int ITEM_MUSIC_DISC_MELLOHI = 196;
        public const int ITEM_MUSIC_DISC_STAL = 197;
        public const int ITEM_MUSIC_DISC_STRAD = 198;
        public const int ITEM_MUSIC_DISC_WARD = 199;
        public const int ITEM_MUSIC_DISC_11 = 200;
        public const int ITEM_MUSIC_DISC_WAIT = 201;
        public const int ITEM_TRIDENT_ITEM = 202;
        public const int ITEM_PHANTOM_MEMBRANE = 203;
        public const int ITEM_NAUTILUS_SHELL = 204;
        public const int ITEM_HEART_OF_THE_SEA = 205;
        public const int ITEM_SCUTE = 206;
        public const int ITEM_SWEET_BERRIES = 207;
        public const int ITEM_HONEYCOMB = 208;
        public const int ITEM_HONEY_BOTTLE = 209;
        public const int ITEM_NETHERITE_INGOT = 210;
        public const int ITEM_NETHERITE_SCRAP = 211;
        public const int ITEM_LODESTONE = 212;
        public const int ITEM_CRIMSON_FUNGUS = 213;
        public const int ITEM_WARPED_FUNGUS = 214;
        public const int ITEM_SHROOMLIGHT = 215;
        public const int ITEM_WEEPING_VINES = 216;
        public const int ITEM_TWISTING_VINES = 217;
        public const int ITEM_NETHER_SPROUTS = 218;
        public const int ITEM_TARGET = 219;
        public const int ITEM_RESPAWN_ANCHOR = 220;
        public const int ITEM_ANCIENT_DEBRIS = 221;
        public const int ITEM_NETHER_GOLD_ORE = 222;
        public const int ITEM_GILDED_BLACKSTONE = 223;
        public const int ITEM_BLACKSTONE = 224;
        public const int ITEM_POLISHED_BLACKSTONE = 225;
        public const int ITEM_POLISHED_BLACKSTONE_BRICKS = 226;
        public const int ITEM_CHISELED_POLISHED_BLACKSTONE = 227;
        public const int ITEM_CRACKED_POLISHED_BLACKSTONE_BRICKS = 228;
        public const int ITEM_GILDED_POLISHED_BLACKSTONE = 229;
        public const int ITEM_QUARTZ_BRICKS = 230;
        public const int ITEM_SOUL_TORCH = 231;
        public const int ITEM_SOUL_LANTERN = 232;
        public const int ITEM_CRYING_OBSIDIAN = 233;
        public const int ITEM_NETHERITE_SCRAP_ITEM = 234;
        public const int ITEM_BUNDLE = 235;
        public const int ITEM_SPYGLASS = 236;
        public const int ITEM_AMETHYST_SHARD = 237;
        public const int ITEM_TUFF = 238;
        public const int ITEM_CALCITE = 239;
        public const int ITEM_DEEPSLATE = 240;
        public const int ITEM_COPPER_INGOT = 241;
        public const int ITEM_RAW_COPPER = 242;
        public const int ITEM_RAW_IRON = 243;
        public const int ITEM_RAW_GOLD = 244;
        public const int ITEM_COPPER_BLOCK = 245;
        public const int ITEM_EXPOSED_COPPER = 246;
        public const int ITEM_WEATHERED_COPPER = 247;
        public const int ITEM_OXIDIZED_COPPER = 248;
        public const int ITEM_WAXED_COPPER = 249;
        public const int ITEM_LIGHTNING_ROD = 250;
        public const int ITEM_POINTED_DRIPSTONE = 251;
        public const int ITEM_DRIPSTONE_BLOCK = 252;
        public const int ITEM_MOSS_CARPET = 253;
        public const int ITEM_MOSS_BLOCK = 254;
        public const int ITEM_AZALEA = 255;
        public const int ITEM_FLOWERING_AZALEA = 256;
        public const int ITEM_GLOW_BERRIES = 257;
        public const int ITEM_GLOW_INK_SAC = 258;
        public const int ITEM_GLOW_ITEM_FRAME = 259;
        public const int ITEM_GLOW_LICHEN = 260;
        public const int ITEM_SCULK_SENSOR = 261;
        public const int ITEM_GOAT_HORN = 262;
        public const int ITEM_ECHO_SHARD = 263;
        public const int ITEM_RECOVERY_COMPASS = 264;
        public const int ITEM_MANGROVE_LOG = 265;
        public const int ITEM_MANGROVE_PLANKS = 266;
        public const int ITEM_MANGROVE_LEAVES = 267;
        public const int ITEM_MANGROVE_ROOTS = 268;
        public const int ITEM_MUDDY_MANGROVE_ROOTS = 269;
        public const int ITEM_MUD = 270;
        public const int ITEM_MUD_BRICKS = 271;
        public const int ITEM_PACKED_MUD = 272;
        public const int ITEM_FROGSPAWN = 273;
        public const int ITEM_TADPOLE_BUCKET = 274;
        public const int ITEM_FROG_BUCKET = 275;
        public const int ITEM_ALEXANDRITE = 276;
        public const int ITEM_GARNET = 277;
        public const int ITEM_AMETHYST = 278;
        public const int ITEM_JADE = 279;
        public const int ITEM_JASPER = 280;
        public const int ITEM_RUBY = 281;
        public const int ITEM_SAPPHIRE = 282;
        public const int ITEM_TOPAZ = 283;
        public const int ITEM_TOURMALINE = 284;
        public const int ITEM_TURQUOISE = 285;
        public const int ITEM_AGATE = 286;
        public const int ITEM_AMBER = 287;
        public const int ITEM_CITRINE = 288;
        public const int ITEM_FLUORITE = 289;
        public const int ITEM_HEMATITE = 290;
        public const int ITEM_JET = 291;
        public const int ITEM_MALACHITE = 292;
        public const int ITEM_OBSIDIAN_ITEM = 293;
        public const int ITEM_ONYX = 294;
        public const int ITEM_PEARL = 295;
        public const int ITEM_PERIDOT = 296;
        public const int ITEM_ROSE_QUARTZ = 297;
        public const int ITEM_SMOKY_QUARTZ = 298;
        public const int ITEM_TIGERS_EYE = 299;
        public const int ITEM_ZIRCON = 300;
    }
}
