using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace VoxelCraft.Core
{
    public class TextureManager : IDisposable
    {
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private readonly Dictionary<string, TextureAtlas> atlases = new Dictionary<string, TextureAtlas>();
        private int currentTextureUnit = 0;
        private int[] boundTextures = new int[32];

        public void LoadAllTextures()
        {
            Console.WriteLine("[TextureManager] 加载纹理...");

            // 创建方块纹理图集
            CreateBlockTextureAtlas();

            // 加载物品纹理
            LoadItemTextures();

            // 加载UI纹理
            LoadUITextures();

            // 加载实体纹理
            LoadEntityTextures();

            // 加载环境纹理
            LoadEnvironmentTextures();

            Console.WriteLine($"[TextureManager] 加载了 {textures.Count} 个纹理");
        }

        public void BindBlockTextureAtlas()
        {
            // 绑定方块纹理图集
        }

        private void CreateBlockTextureAtlas()
        {
            // 创建方块纹理图集（16x16的方块纹理，拼成256x256的图集）
            int atlasSize = 256;
            int tileSize = 16;
            int tilesPerRow = atlasSize / tileSize;

            TextureAtlas atlas = new TextureAtlas("blocks", atlasSize, tileSize);

            // 注册所有方块纹理
            string[] blockTextures = {
                "stone", "grass_top", "grass_side", "dirt", "cobblestone",
                "planks_oak", "planks_spruce", "planks_birch", "planks_jungle",
                "log_oak", "log_oak_top", "leaves_oak", "leaves_spruce",
                "sand", "gravel", "gold_ore", "iron_ore", "coal_ore",
                "diamond_ore", "redstone_ore", "lapis_ore", "emerald_ore",
                "obsidian", "bedrock", "glass", "tnt_side", "tnt_top", "tnt_bottom",
                "bookshelf", "mossy_cobblestone", "obsidian", "sponge",
                "brick", "clay", "snow", "ice", "cactus_side", "cactus_top",
                "pumpkin_side", "pumpkin_top", "pumpkin_face", "melon_side", "melon_top",
                "mycelium", "netherrack", "soul_sand", "glowstone", "end_stone",
                "quartz_block", "quartz_ore", "prismarine", "sea_lantern",
                "hay_block", "hardened_clay", "coal_block", "packed_ice",
                "red_sand", "red_sandstone", "purpur_block", "end_bricks",
                "magma", "nether_wart_block", "red_nether_bricks", "bone_block",
                "white_wool", "orange_wool", "magenta_wool", "light_blue_wool",
                "yellow_wool", "lime_wool", "pink_wool", "gray_wool",
                "light_gray_wool", "cyan_wool", "purple_wool", "blue_wool",
                "brown_wool", "green_wool", "red_wool", "black_wool",
                "water_still", "lava_still", "fire", "soul_fire",
                "torch", "soul_torch", "redstone_torch", "ladder",
                "wheat_stage_0", "wheat_stage_1", "wheat_stage_2", "wheat_stage_3",
                "wheat_stage_4", "wheat_stage_5", "wheat_stage_6", "wheat_stage_7",
                "farmland", "farmland_moist", "sugar_cane", "vine",
                "dandelion", "poppy", "blue_orchid", "allium",
                "azure_bluet", "red_tulip", "orange_tulip", "white_tulip",
                "pink_tulip", "oxeye_daisy", "brown_mushroom", "red_mushroom",
                "tall_grass", "fern", "dead_bush", "lily_pad",
                "cobweb", "sweet_berry_bush", "cactus", "bamboo",
                "oak_sapling", "spruce_sapling", "birch_sapling", "jungle_sapling",
                "acacia_sapling", "dark_oak_sapling",
                "oak_door", "iron_door", "spruce_door", "birch_door",
                "jungle_door", "acacia_door", "dark_oak_door",
                "oak_trapdoor", "iron_trapdoor", "spruce_trapdoor",
                "oak_fence", "spruce_fence", "birch_fence", "jungle_fence",
                "oak_stairs", "cobblestone_stairs", "brick_stairs",
                "stone_bricks", "mossy_stone_bricks", "cracked_stone_bricks",
                "chiseled_stone_bricks", "sandstone", "red_sandstone",
                "quartz_stairs", "brick_stairs", "stone_brick_stairs",
                "nether_bricks", "nether_brick_fence", "nether_brick_stairs",
                "ender_chest", "enchanting_table", "brewing_stand",
                "cauldron", "anvil", "beacon", "conduit",
                "white_concrete", "orange_concrete", "magenta_concrete",
                "light_blue_concrete", "yellow_concrete", "lime_concrete",
                "pink_concrete", "gray_concrete", "light_gray_concrete",
                "cyan_concrete", "purple_concrete", "blue_concrete",
                "brown_concrete", "green_concrete", "red_concrete", "black_concrete",
                "white_terracotta", "orange_terracotta", "yellow_terracotta",
                "red_terracotta", "brown_terracotta", "green_terracotta",
                "cyan_terracotta", "blue_terracotta", "purple_terracotta",
                "magenta_terracotta", "pink_terracotta", "gray_terracotta",
                "light_gray_terracotta", "black_terracotta", "white_glazed_terracotta",
                "orange_glazed_terracotta", "magenta_glazed_terracotta",
                "light_blue_glazed_terracotta", "yellow_glazed_terracotta",
                "lime_glazed_terracotta", "pink_glazed_terracotta",
                "gray_glazed_terracotta", "light_gray_glazed_terracotta",
                "cyan_glazed_terracotta", "purple_glazed_terracotta",
                "blue_glazed_terracotta", "brown_glazed_terracotta",
                "green_glazed_terracotta", "red_glazed_terracotta",
                "black_glazed_terracotta", "slime_block", "honey_block",
                "honeycomb_block", "beehive", "bee_nest",
                "scaffolding", "loom", "cartography_table", "fletching_table",
                "smithing_table", "grindstone", "stonecutter", "bell",
                "lantern", "soul_lantern", "campfire", "soul_campfire",
                "barrel", "smoker", "blast_furnace", "lectern",
                "composter", "target", "respawn_anchor", "crying_obsidian",
                "ancient_debris", "nether_gold_ore", "blackstone",
                "polished_blackstone", "polished_blackstone_bricks",
                "chiseled_polished_blackstone", "cracked_polished_blackstone_bricks",
                "gilded_blackstone", "gilded_polished_blackstone",
                "quartz_bricks", "soul_soil", "basalt", "polished_basalt",
                "tuff", "calcite", "dripstone_block", "pointed_dripstone",
                "moss_block", "moss_carpet", "azalea", "flowering_azalea",
                "glow_lichen", "sculk", "sculk_catalyst", "sculk_sensor",
                "sculk_shrieker", "deepslate", "cobbled_deepslate",
                "polished_deepslate", "deepslate_bricks", "deepslate_tiles",
                "cracked_deepslate_bricks", "cracked_deepslate_tiles",
                "chiseled_deepslate", "infested_deepslate",
                "copper_ore", "iron_ore_deepslate", "gold_ore_deepslate",
                "diamond_ore_deepslate", "emerald_ore_deepslate",
                "lapis_ore_deepslate", "redstone_ore_deepslate",
                "coal_ore_deepslate", "copper_block", "exposed_copper",
                "weathered_copper", "oxidized_copper", "waxed_copper",
                "waxed_exposed_copper", "waxed_weathered_copper",
                "waxed_oxidized_copper", "cut_copper", "exposed_cut_copper",
                "weathered_cut_copper", "oxidized_cut_copper",
                "lightning_rod", "raw_copper_block", "raw_iron_block",
                "raw_gold_block", "amethyst_block", "budding_amethyst",
                "small_amethyst_bud", "medium_amethyst_bud",
                "large_amethyst_bud", "amethyst_cluster",
                "mangrove_log", "mangrove_planks", "mangrove_leaves",
                "mangrove_roots", "muddy_mangrove_roots", "mud",
                "mud_bricks", "packed_mud", "frogspawn",
                "mangrove_propagule", "mangrove_fence", "mangrove_door",
                "mangrove_trapdoor", "mangrove_stairs", "mangrove_slab",
                "mangrove_fence_gate", "mangrove_button", "mangrove_pressure_plate",
                "mangrove_sign", "mangrove_boat", "mangrove_chest_boat",
                "cherry_log", "cherry_planks", "cherry_leaves",
                "cherry_sapling", "cherry_fence", "cherry_door",
                "cherry_trapdoor", "cherry_stairs", "cherry_slab",
                "cherry_fence_gate", "cherry_button", "cherry_pressure_plate",
                "cherry_sign", "cherry_boat", "cherry_chest_boat",
                "bamboo_planks", "bamboo_fence", "bamboo_door",
                "bamboo_trapdoor", "bamboo_stairs", "bamboo_slab",
                "bamboo_fence_gate", "bamboo_button", "bamboo_pressure_plate",
                "bamboo_sign", "bamboo_raft", "bamboo_chest_raft",
                "chiseled_bookshelf", "decorated_pot", "piglin_head",
                "piglin_wall_head", "zombie_head", "zombie_wall_head",
                "skeleton_skull", "skeleton_wall_skull",
                "wither_skeleton_skull", "wither_skeleton_wall_skull",
                "player_head", "player_wall_head",
                "creeper_head", "creeper_wall_head",
                "dragon_head", "dragon_wall_head",
                "suspicious_sand", "suspicious_gravel",
                "sniffer_egg", "torchflower", "torchflower_crop",
                "pitcher_plant", "pitcher_crop", "pitcher_pod",
                "calibrated_sculk_sensor", "trial_spawner",
                "vault", "omega_chest", "bogged_spawn_egg"
            };

            for (int i = 0; i < blockTextures.Length && i < tilesPerRow * tilesPerRow; i++)
            {
                int row = i / tilesPerRow;
                int col = i % tilesPerRow;
                atlas.AddTile(blockTextures[i], col, row);
            }

            atlas.Build();
            atlases["blocks"] = atlas;
            textures["blocks_atlas"] = atlas.Texture;

            Console.WriteLine($"[TextureManager] 方块纹理图集创建完成，包含 {blockTextures.Length} 个纹理");
        }

        private void LoadItemTextures()
        {
            // 加载物品纹理
            string[] itemTextures = {
                "wooden_sword", "stone_sword", "iron_sword", "golden_sword", "diamond_sword", "netherite_sword",
                "wooden_pickaxe", "stone_pickaxe", "iron_pickaxe", "golden_pickaxe", "diamond_pickaxe", "netherite_pickaxe",
                "wooden_axe", "stone_axe", "iron_axe", "golden_axe", "diamond_axe", "netherite_axe",
                "wooden_shovel", "stone_shovel", "iron_shovel", "golden_shovel", "diamond_shovel", "netherite_shovel",
                "wooden_hoe", "stone_hoe", "iron_hoe", "golden_hoe", "diamond_hoe", "netherite_hoe",
                "bow", "crossbow", "trident", "shield", "fishing_rod", "flint_and_steel",
                "apple", "golden_apple", "enchanted_golden_apple", "carrot", "golden_carrot",
                "potato", "baked_potato", "poisonous_potato", "beetroot", "beetroot_soup",
                "bread", "cookie", "cake", "pumpkin_pie",
                "raw_beef", "steak", "raw_porkchop", "cooked_porkchop",
                "raw_chicken", "cooked_chicken", "raw_mutton", "cooked_mutton",
                "raw_rabbit", "cooked_rabbit", "rabbit_stew", "rabbit_foot", "rabbit_hide",
                "raw_cod", "cooked_cod", "raw_salmon", "cooked_salmon",
                "clownfish", "pufferfish", "tropical_fish",
                "rotten_flesh", "spider_eye", "fermented_spider_eye",
                "ender_pearl", "ender_eye", "blaze_rod", "blaze_powder",
                "ghast_tear", "magma_cream", "glowstone_dust",
                "nether_wart", "nether_star", "netherite_ingot", "netherite_scrap",
                "iron_ingot", "gold_ingot", "diamond", "emerald", "lapis_lazuli",
                "redstone", "coal", "charcoal", "copper_ingot",
                "raw_iron", "raw_gold", "raw_copper", "iron_nugget", "gold_nugget",
                "stick", "string", "leather", "feather", "bone", "bone_meal",
                "sugar", "paper", "book", "book_and_quill", "written_book",
                "slime_ball", "clay_ball", "brick", "flint",
                "snowball", "egg", "compass", "clock", "map", "empty_map",
                "name_tag", "lead", "saddle", "armor_stand",
                "potion", "glass_bottle", "dragon_breath", "experience_bottle",
                "fire_charge", "firework_rocket", "firework_star",
                "item_frame", "glow_item_frame", "flower_pot",
                "leather_helmet", "leather_chestplate", "leather_leggings", "leather_boots",
                "chainmail_helmet", "chainmail_chestplate", "chainmail_leggings", "chainmail_boots",
                "iron_helmet", "iron_chestplate", "iron_leggings", "iron_boots",
                "golden_helmet", "golden_chestplate", "golden_leggings", "golden_boots",
                "diamond_helmet", "diamond_chestplate", "diamond_leggings", "diamond_boots",
                "netherite_helmet", "netherite_chestplate", "netherite_leggings", "netherite_boots",
                "turtle_helmet", "elytra",
                "iron_horse_armor", "golden_horse_armor", "diamond_horse_armor", "leather_horse_armor",
                "music_disc_13", "music_disc_cat", "music_disc_blocks", "music_disc_chirp",
                "music_disc_far", "music_disc_mall", "music_disc_mellohi", "music_disc_stal",
                "music_disc_strad", "music_disc_ward", "music_disc_11", "music_disc_wait",
                "music_disc_otherside", "music_disc_pigstep", "music_disc_5", "music_disc_relic",
                "totem_of_undying", "shield_banner", "nautilus_shell", "heart_of_the_sea",
                "scute", "phantom_membrane", "sweet_berries", "glow_berries",
                "honeycomb", "honey_bottle", "spyglass", "bundle",
                "goat_horn", "echo_shard", "recovery_compass",
                "disc_fragment_5", "armadillo_scute", "wolf_armor",
                "mace", "trial_key", "ominous_trial_key", "wind_charge",
                "breeze_rod", "heavy_core", "creaking_heart"
            };

            foreach (string item in itemTextures)
            {
                LoadTexture($"item/{item}", $"items/{item}.png");
            }
        }

        private void LoadUITextures()
        {
            string[] uiTextures = {
                "widgets", "icons", "hotbar", "inventory", "creative_inventory",
                "heart", "heart_full", "heart_half", "heart_empty",
                "hunger_full", "hunger_half", "hunger_empty",
                "armor_full", "armor_half", "armor_empty",
                "bubble_full", "bubble_half", "bubble_empty",
                "crosshair", "selection", "highlight",
                "button", "button_hover", "button_pressed",
                "slider", "slider_knob", "checkbox", "checkbox_checked",
                "scrollbar", "scrollbar_knob",
                "title", "mojang", "options_background",
                "particle", "break_0", "break_1", "break_2", "break_3",
                "break_4", "break_5", "break_6", "break_7", "break_8", "break_9",
                "sun", "moon", "cloud", "rain", "snow",
                "vignette", "spyglass_overlay", "helmet_overlay",
                "powder_snow_outline", "underwater_overlay",
                "fire_0", "fire_1", "soul_fire_0", "soul_fire_1"
            };

            foreach (string ui in uiTextures)
            {
                LoadTexture($"ui/{ui}", $"ui/{ui}.png");
            }
        }

        private void LoadEntityTextures()
        {
            string[] entityTextures = {
                "player", "player_slim", "zombie", "skeleton", "creeper",
                "spider", "cave_spider", "enderman", "witch", "slime",
                "magma_cube", "ghast", "blaze", "wither_skeleton",
                "husk", "stray", "drowned", "phantom",
                "cow", "pig", "sheep", "chicken", "rabbit",
                "horse", "donkey", "mule", "llama", "wolf",
                "ocelot", "cat", "parrot", "bat", "squid",
                "dolphin", "turtle", "panda", "fox", "bee",
                "polar_bear", "mooshroom", "snow_golem", "iron_golem",
                "villager", "wandering_trader", "evoker", "vindicator",
                "pillager", "ravager", "vex", "warden",
                "frog", "tadpole", "allay", "warden",
                "camel", "sniffer", "armadillo", "bogged",
                "breeze", "creaking", "zombie_horse", "skeleton_horse",
                "strider", "hoglin", "zoglin", "piglin", "piglin_brute",
                "endermite", "silverfish", "shulker", "elder_guardian",
                "guardian", "wither", "ender_dragon",
                "arrow", "spectral_arrow", "tipped_arrow",
                "item", "experience_orb", "eye_of_ender",
                "ender_crystal", "firework_rocket_entity", "firework_star_entity",
                "painting", "item_frame_entity", "leash_knot",
                "armor_stand_entity", "evoker_fangs", "llama_spit",
                "shulker_bullet", "dragon_fireball", "fireball",
                "small_fireball", "wither_skull", "spectral_arrow_entity",
                "trident_entity", "tnt", "minecart", "chest_minecart",
                "furnace_minecart", "tnt_minecart", "hopper_minecart",
                "spawner_minecart", "command_block_minecart",
                "boat", "chest_boat", "raft", "chest_raft",
                "fishing_hook", "lightning_bolt", "area_effect_cloud"
            };

            foreach (string entity in entityTextures)
            {
                LoadTexture($"entity/{entity}", $"entity/{entity}.png");
            }
        }

        private void LoadEnvironmentTextures()
        {
            string[] envTextures = {
                "sun_gradient", "moon_phases", "clouds", "stars",
                "rain", "snow", "underwater", "nether_background",
                "end_background", "end_portal", "nether_portal",
                "ender_gateway", "beacon_beam", "conduit_beam",
                "particle_atlas", "break_particles", "water_still_anim",
                "lava_still_anim", "fire_anim", "soul_fire_anim",
                "portal_anim", "ender_portal_anim", "enchant_table_anim",
                "sculk_sensor_anim", "respawn_anchor_anim"
            };

            foreach (string env in envTextures)
            {
                LoadTexture($"env/{env}", $"environment/{env}.png");
            }
        }

        public void LoadTexture(string name, string filename)
        {
            string path = Path.Combine("assets", "textures", filename);
            if (File.Exists(path))
            {
                try
                {
                    Texture texture = new Texture(name, path);
                    if (texture.IsValid)
                    {
                        textures[name] = texture;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TextureManager] 加载纹理 {name} 失败: {ex.Message}");
                }
            }
            else
            {
                // 创建默认纹理
                textures[name] = CreateDefaultTexture(name);
            }
        }

        private Texture CreateDefaultTexture(string name)
        {
            // 创建16x16的紫色/黑色棋盘格默认纹理
            int width = 16;
            int height = 16;
            byte[] data = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = (y * width + x) * 4;
                    bool isPurple = (x / 4 + y / 4) % 2 == 0;
                    data[idx] = isPurple ? (byte)255 : (byte)0;     // R
                    data[idx + 1] = isPurple ? (byte)0 : (byte)0;    // G
                    data[idx + 2] = isPurple ? (byte)255 : (byte)0;  // B
                    data[idx + 3] = 255;                               // A
                }
            }

            return new Texture(name, data, width, height);
        }

        public Texture GetTexture(string name)
        {
            if (textures.TryGetValue(name, out var texture))
            {
                return texture;
            }
            return null;
        }

        public TextureAtlas GetAtlas(string name)
        {
            if (atlases.TryGetValue(name, out var atlas))
            {
                return atlas;
            }
            return null;
        }

        public Vector4 GetTextureCoords(string atlasName, string tileName)
        {
            if (atlases.TryGetValue(atlasName, out var atlas))
            {
                return atlas.GetTileCoords(tileName);
            }
            return new Vector4(0, 0, 1, 1);
        }

        public void BindTexture(string name, int textureUnit = 0)
        {
            if (textures.TryGetValue(name, out var texture))
            {
                texture.Bind(textureUnit);
                boundTextures[textureUnit] = texture.TextureId;
            }
        }

        public void UnbindTexture(int textureUnit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            boundTextures[textureUnit] = 0;
        }

        public void Dispose()
        {
            foreach (var texture in textures.Values)
            {
                texture.Dispose();
            }
            foreach (var atlas in atlases.Values)
            {
                atlas.Dispose();
            }
            textures.Clear();
            atlases.Clear();
        }
    }

    // ========================================
    // 纹理类
    // ========================================
    public class Texture : IDisposable
    {
        public int TextureId { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsValid { get; private set; }

        public Texture(string name, string filePath)
        {
            Name = name;
            LoadFromFile(filePath);
        }

        public Texture(string name, byte[] data, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
            CreateTexture(data);
        }

        private void LoadFromFile(string filePath)
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(filePath))
                {
                    Width = bitmap.Width;
                    Height = bitmap.Height;

                    BitmapData bmpData = bitmap.LockBits(
                        new Rectangle(0, 0, Width, Height),
                        ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb
                    );

                    byte[] data = new byte[Width * Height * 4];
                    System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

                    // 转换ARGB到RGBA
                    for (int i = 0; i < data.Length; i += 4)
                    {
                        byte b = data[i];
                        byte g = data[i + 1];
                        byte r = data[i + 2];
                        byte a = data[i + 3];
                        data[i] = r;
                        data[i + 1] = g;
                        data[i + 2] = b;
                        data[i + 3] = a;
                    }

                    bitmap.UnlockBits(bmpData);
                    CreateTexture(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Texture] 加载纹理 {Name} 失败: {ex.Message}");
                IsValid = false;
            }
        }

        private void CreateTexture(byte[] data)
        {
            TextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            if (GameConstants.USE_MIPMAP)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            IsValid = true;
        }

        public void Bind(int textureUnit = 0)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetFilter(TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
        }

        public void SetWrap(TextureWrapMode wrapS, TextureWrapMode wrapT)
        {
            Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapS);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapT);
        }

        public void Dispose()
        {
            if (TextureId != 0)
            {
                GL.DeleteTexture(TextureId);
                TextureId = 0;
            }
        }
    }

    // ========================================
    // 纹理图集类
    // ========================================
    public class TextureAtlas : IDisposable
    {
        public string Name { get; private set; }
        public Texture Texture { get; private set; }
        public int AtlasSize { get; private set; }
        public int TileSize { get; private set; }
        public int TilesPerRow { get; private set; }

        private readonly Dictionary<string, Vector2> tilePositions = new Dictionary<string, Vector2>();
        private byte[] atlasData;

        public TextureAtlas(string name, int atlasSize, int tileSize)
        {
            Name = name;
            AtlasSize = atlasSize;
            TileSize = tileSize;
            TilesPerRow = atlasSize / tileSize;
            atlasData = new byte[atlasSize * atlasSize * 4];
        }

        public void AddTile(string name, int col, int row)
        {
            tilePositions[name] = new Vector2(col, row);

            // 填充默认纹理数据（棋盘格）
            for (int y = 0; y < TileSize; y++)
            {
                for (int x = 0; x < TileSize; x++)
                {
                    int atlasX = col * TileSize + x;
                    int atlasY = row * TileSize + y;
                    int idx = (atlasY * AtlasSize + atlasX) * 4;

                    bool isLight = (x / 4 + y / 4) % 2 == 0;
                    byte shade = isLight ? (byte)200 : (byte)150;

                    atlasData[idx] = shade;
                    atlasData[idx + 1] = shade;
                    atlasData[idx + 2] = shade;
                    atlasData[idx + 3] = 255;
                }
            }
        }

        public void Build()
        {
            Texture = new Texture(Name, atlasData, AtlasSize, AtlasSize);
        }

        public Vector4 GetTileCoords(string tileName)
        {
            if (tilePositions.TryGetValue(tileName, out var pos))
            {
                float u0 = pos.X / TilesPerRow;
                float v0 = 1.0f - (pos.Y + 1) / TilesPerRow;
                float u1 = (pos.X + 1) / TilesPerRow;
                float v1 = 1.0f - pos.Y / TilesPerRow;
                return new Vector4(u0, v0, u1, v1);
            }
            return new Vector4(0, 0, 1.0f / TilesPerRow, 1.0f / TilesPerRow);
        }

        public bool HasTile(string tileName)
        {
            return tilePositions.ContainsKey(tileName);
        }

        public void Dispose()
        {
            Texture?.Dispose();
            tilePositions.Clear();
        }
    }
}
