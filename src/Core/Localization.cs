using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class Localization
    {
        private static Localization instance;
        public static Localization Instance => instance ??= new Localization();

        private readonly Dictionary<string, string> translations;
        private readonly Dictionary<string, Dictionary<string, string>> allLanguages;
        private string currentLanguage;

        public string CurrentLanguage => currentLanguage;
        public int TranslationCount => translations.Count;

        private Localization()
        {
            translations = new Dictionary<string, string>();
            allLanguages = new Dictionary<string, Dictionary<string, string>>();
            currentLanguage = "zh_cn";
        }

        public void Initialize()
        {
            // 加载内置语言
            LoadBuiltinLanguages();

            // 加载当前语言
            LoadLanguage(currentLanguage);

            Console.WriteLine($"[Localization] 本地化系统初始化完成，当前语言: {currentLanguage}，共 {translations.Count} 条翻译");
        }

        private void LoadBuiltinLanguages()
        {
            // 简体中文
            allLanguages["zh_cn"] = GenerateChineseTranslations();

            // 英文
            allLanguages["en_us"] = GenerateEnglishTranslations();

            // 繁体中文
            allLanguages["zh_tw"] = GenerateTraditionalChineseTranslations();

            // 日文
            allLanguages["ja_jp"] = GenerateJapaneseTranslations();
        }

        private Dictionary<string, string> GenerateChineseTranslations()
        {
            Dictionary<string, string> lang = new Dictionary<string, string>();

            // 游戏
            lang["game.title"] = "VoxelCraft";
            lang["game.menu.singleplayer"] = "单人游戏";
            lang["game.menu.multiplayer"] = "多人游戏";
            lang["game.menu.options"] = "选项";
            lang["game.menu.quit"] = "退出游戏";
            lang["game.menu.language"] = "语言";

            // 菜单
            lang["menu.play"] = "开始游戏";
            lang["menu.createWorld"] = "创建新世界";
            lang["menu.selectWorld"] = "选择世界";
            lang["menu.delete"] = "删除";
            lang["menu.rename"] = "重命名";
            lang["menu.copy"] = "复制";
            lang["menu.cancel"] = "取消";
            lang["menu.ok"] = "确定";
            lang["menu.yes"] = "是";
            lang["menu.no"] = "否";
            lang["menu.back"] = "返回";
            lang["menu.done"] = "完成";
            lang["menu.save"] = "保存";
            lang["menu.load"] = "加载";

            // 世界创建
            lang["createWorld.title"] = "创建新世界";
            lang["createWorld.name"] = "世界名称";
            lang["createWorld.seed"] = "种子";
            lang["createWorld.gamemode"] = "游戏模式";
            lang["createWorld.difficulty"] = "难度";
            lang["createWorld.worldType"] = "世界类型";
            lang["createWorld.generateStructures"] = "生成结构";
            lang["createWorld.allowCheats"] = "允许作弊";
            lang["createWorld.hardcore"] = "极限模式";
            lang["createWorld.survival"] = "生存模式";
            lang["createWorld.creative"] = "创造模式";
            lang["createWorld.adventure"] = "冒险模式";
            lang["createWorld.peaceful"] = "和平";
            lang["createWorld.easy"] = "简单";
            lang["createWorld.normal"] = "普通";
            lang["createWorld.hard"] = "困难";
            lang["createWorld.default"] = "默认";
            lang["createWorld.flat"] = "超平坦";
            lang["createWorld.largeBiomes"] = "大型生物群系";
            lang["createWorld.amplified"] = "放大化";

            // 设置
            lang["options.title"] = "选项";
            lang["options.video"] = "视频设置";
            lang["options.audio"] = "音频设置";
            lang["options.controls"] = "控制设置";
            lang["options.language"] = "语言设置";
            lang["options.skin"] = "皮肤设置";
            lang["options.chat"] = "聊天设置";
            lang["options.multiplayer"] = "多人游戏设置";
            lang["options.accessibility"] = "辅助功能";
            lang["options.renderDistance"] = "渲染距离";
            lang["options.fov"] = "视野";
            lang["options.maxFps"] = "最大帧率";
            lang["options.vsync"] = "垂直同步";
            lang["options.fullscreen"] = "全屏";
            lang["options.graphics"] = "图像品质";
            lang["options.smoothLighting"] = "平滑光照";
            lang["options.clouds"] = "云";
            lang["options.viewBobbing"] = "视角摇晃";
            lang["options.showFps"] = "显示帧率";
            lang["options.guiScale"] = "界面大小";
            lang["options.masterVolume"] = "主音量";
            lang["options.musicVolume"] = "音乐音量";
            lang["options.soundVolume"] = "音效音量";
            lang["options.mouseSensitivity"] = "鼠标灵敏度";
            lang["options.invertMouse"] = "反转鼠标";
            lang["options.autoJump"] = "自动跳跃";

            // 游戏内
            lang["game.paused"] = "游戏暂停";
            lang["game.resume"] = "继续游戏";
            lang["game.options"] = "选项";
            lang["game.advancements"] = "进度";
            lang["game.statistics"] = "统计";
            lang["game.openToLan"] = "对局域网开放";
            lang["game.saveAndQuit"] = "保存并退出";
            lang["game.death"] = "你死了！";
            lang["game.respawn"] = "重生";
            lang["game.titleScreen"] = "返回标题画面";
            lang["game.spectator"] = "旁观者模式";

            // 物品栏
            lang["inventory.title"] = "物品栏";
            lang["inventory.creative"] = "创造模式物品栏";
            lang["inventory.survival"] = "生存模式物品栏";
            lang["inventory.crafting"] = "合成";
            lang["inventory.smelting"] = "熔炼";
            lang["inventory.trading"] = "交易";
            lang["inventory.enchanting"] = "附魔";
            lang["inventory.brewing"] = "酿造";
            lang["inventory.search"] = "搜索物品...";
            lang["inventory.all"] = "全部";
            lang["inventory.buildingBlocks"] = "建筑方块";
            lang["inventory.decorations"] = "装饰方块";
            lang["inventory.redstone"] = "红石";
            lang["inventory.transportation"] = "交通";
            lang["inventory.misc"] = "杂项";
            lang["inventory.food"] = "食物";
            lang["inventory.tools"] = "工具";
            lang["inventory.combat"] = "战斗";
            lang["inventory.brewing"] = "酿造";

            // 方块
            lang["block.stone"] = "石头";
            lang["block.grass"] = "草方块";
            lang["block.dirt"] = "泥土";
            lang["block.cobblestone"] = "圆石";
            lang["block.planks"] = "木板";
            lang["block.log"] = "原木";
            lang["block.leaves"] = "树叶";
            lang["block.sand"] = "沙子";
            lang["block.gravel"] = "沙砾";
            lang["block.glass"] = "玻璃";
            lang["block.bricks"] = "砖块";
            lang["block.stoneBricks"] = "石砖";
            lang["block.obsidian"] = "黑曜石";
            lang["block.water"] = "水";
            lang["block.lava"] = "岩浆";
            lang["block.bedrock"] = "基岩";
            lang["block.coalOre"] = "煤矿石";
            lang["block.ironOre"] = "铁矿石";
            lang["block.goldOre"] = "金矿石";
            lang["block.diamondOre"] = "钻石矿石";
            lang["block.redstoneOre"] = "红石矿石";
            lang["block.lapisOre"] = "青金石矿石";
            lang["block.emeraldOre"] = "绿宝石矿石";
            lang["block.copperOre"] = "铜矿石";
            lang["block.tnt"] = "TNT";
            lang["block.craftingTable"] = "工作台";
            lang["block.furnace"] = "熔炉";
            lang["block.chest"] = "箱子";
            lang["block.door"] = "木门";
            lang["block.ironDoor"] = "铁门";
            lang["block.ladder"] = "梯子";
            lang["block.torch"] = "火把";
            lang["block.glowstone"] = "萤石";
            lang["block.snow"] = "雪";
            lang["block.ice"] = "冰";
            lang["block.pumpkin"] = "南瓜";
            lang["block.melon"] = "西瓜";
            lang["block.cactus"] = "仙人掌";
            lang["block.sugarCane"] = "甘蔗";
            lang["block.bamboo"] = "竹子";
            lang["block.vine"] = "藤蔓";
            lang["block.wheat"] = "小麦";
            lang["block.carrot"] = "胡萝卜";
            lang["block.potato"] = "马铃薯";
            lang["block.farmland"] = "耕地";
            lang["block.redstoneWire"] = "红石线";
            lang["block.redstoneTorch"] = "红石火把";
            lang["block.repeater"] = "中继器";
            lang["block.comparator"] = "比较器";
            lang["block.lever"] = "拉杆";
            lang["block.button"] = "按钮";
            lang["block.piston"] = "活塞";
            lang["block.stickyPiston"] = "粘性活塞";
            lang["block.dispenser"] = "发射器";
            lang["block.dropper"] = "投掷器";
            lang["block.noteBlock"] = "音符盒";
            lang["block.jukebox"] = "唱片机";
            lang["block.enchantingTable"] = "附魔台";
            lang["block.anvil"] = "铁砧";
            lang["block.brewingStand"] = "酿造台";
            lang["block.cauldron"] = "炼药锅";
            lang["block.endPortal"] = "末地传送门";
            lang["block.netherPortal"] = "下界传送门";
            lang["block.endPortalFrame"] = "末地传送门框架";
            lang["block.netherrack"] = "下界岩";
            lang["block.soulSand"] = "灵魂沙";
            lang["block.netherQuartzOre"] = "下界石英矿石";
            lang["block.netherGoldOre"] = "下界金矿石";
            lang["block.blackstone"] = "黑石";
            lang["block.basalt"] = "玄武岩";
            lang["block.crimsonNylium"] = "绯红菌岩";
            lang["block.warpedNylium"] = "诡异菌岩";
            lang["block.shroomlight"] = "菌光体";
            lang["block.respawnAnchor"] = "重生锚";
            lang["block.cryingObsidian"] = "哭泣的黑曜石";
            lang["block.ancientDebris"] = "远古残骸";
            lang["block.netheriteBlock"] = "下界合金块";
            lang["block.smithingTable"] = "锻造台";
            lang["block.stonecutter"] = "切石机";
            lang["block.grindstone"] = "砂轮";
            lang["block.blastFurnace"] = "高炉";
            lang["block.smoker"] = "烟熏炉";
            lang["block.cartographyTable"] = "制图台";
            lang["block.fletchingTable"] = "制箭台";
            lang["block.lectern"] = "讲台";
            lang["block.smithingTable"] = "锻造台";
            lang["block.barrel"] = "木桶";
            lang["block.loom"] = "织布机";
            lang["block.composter"] = "堆肥桶";
            lang["block.beehive"] = "蜂巢";
            lang["block.beeNest"] = "蜂箱";
            lang["block.honeyBlock"] = "蜂蜜块";
            lang["block.honeycombBlock"] = "蜜脾块";
            lang["block.target"] = "标靶";
            lang["block.netheriteScrap"] = "下界合金碎片";
            lang["block.soulSoil"] = "灵魂土";
            lang["block.soulFire"] = "灵魂火";
            lang["block.soulTorch"] = "灵魂火把";
            lang["block.soulLantern"] = "灵魂灯笼";
            lang["block.crimsonStem"] = "绯红菌柄";
            lang["block.warpedStem"] = "诡异菌柄";
            lang["block.crimsonPlanks"] = "绯红木板";
            lang["block.warpedPlanks"] = "诡异木板";
            lang["block.crimsonSlab"] = "绯红台阶";
            lang["block.warpedSlab"] = "诡异台阶";
            lang["block.crimsonStairs"] = "绯红楼梯";
            lang["block.warpedStairs"] = "诡异楼梯";
            lang["block.crimsonFence"] = "绯红栅栏";
            lang["block.warpedFence"] = "诡异栅栏";
            lang["block.crimsonDoor"] = "绯红木门";
            lang["block.warpedDoor"] = "诡异木门";
            lang["block.crimsonTrapdoor"] = "绯红活板门";
            lang["block.warpedTrapdoor"] = "诡异活板门";
            lang["block.crimsonButton"] = "绯红按钮";
            lang["block.warpedButton"] = "诡异按钮";
            lang["block.crimsonPressurePlate"] = "绯红压力板";
            lang["block.warpedPressurePlate"] = "诡异压力板";
            lang["block.crimsonSign"] = "绯红告示牌";
            lang["block.warpedSign"] = "诡异告示牌";
            lang["block.chain"] = "锁链";
            lang["block.lantern"] = "灯笼";
            lang["block.campfire"] = "营火";
            lang["block.soulCampfire"] = "灵魂营火";
            lang["block.sweetBerryBush"] = "甜浆果丛";
            lang["block.witherRose"] = "凋零玫瑰";
            lang["block.lilyOfTheValley"] = "铃兰";
            lang["block.cornflower"] = "矢车菊";
            lang["block.blueOrchid"] = "兰花";
            lang["block.allium"] = "绒球葱";
            lang["block.azureBluet"] = "滨菊";
            lang["block.redTulip"] = "红色郁金香";
            lang["block.orangeTulip"] = "橙色郁金香";
            lang["block.whiteTulip"] = "白色郁金香";
            lang["block.pinkTulip"] = "粉红色郁金香";
            lang["block.dandelion"] = "蒲公英";
            lang["block.poppy"] = "虞美人";
            lang["block.blueOrchid"] = "兰花";
            lang["block.brownMushroom"] = "棕色蘑菇";
            lang["block.redMushroom"] = "红色蘑菇";
            lang["block.mushroomStem"] = "蘑菇柄";
            lang["block.mushroomBlock"] = "蘑菇方块";
            lang["block.deadBush"] = "枯萎的灌木";
            lang["block.fern"] = "蕨";
            lang["block.largeFern"] = "大型蕨";
            lang["block.grassPlant"] = "草";
            lang["block.tallGrass"] = "高草丛";
            lang["block.seagrass"] = "海草";
            lang["block.tallSeagrass"] = "高海草";
            lang["block.kelp"] = "海带";
            lang["block.driedKelpBlock"] = "干海带块";
            lang["block.bamboo"] = "竹子";
            lang["block.bambooSapling"] = "竹笋";
            lang["block.scaffolding"] = "脚手架";
            lang["block.strippedOakLog"] = "去皮橡木原木";
            lang["block.strippedBirchLog"] = "去皮白桦木原木";
            lang["block.strippedSpruceLog"] = "去皮云杉木原木";
            lang["block.strippedJungleLog"] = "去皮丛林木原木";
            lang["block.strippedAcaciaLog"] = "去皮金合欢木原木";
            lang["block.strippedDarkOakLog"] = "去皮深色橡木原木";
            lang["block.strippedCrimsonStem"] = "去皮绯红菌柄";
            lang["block.strippedWarpedStem"] = "去皮诡异菌柄";
            lang["block.oakWood"] = "橡木";
            lang["block.birchWood"] = "白桦木";
            lang["block.spruceWood"] = "云杉木";
            lang["block.jungleWood"] = "丛林木";
            lang["block.acaciaWood"] = "金合欢木";
            lang["block.darkOakWood"] = "深色橡木";
            lang["block.strippedOakWood"] = "去皮橡木";
            lang["block.strippedBirchWood"] = "去皮白桦木";
            lang["block.strippedSpruceWood"] = "去皮云杉木";
            lang["block.strippedJungleWood"] = "去皮丛林木";
            lang["block.strippedAcaciaWood"] = "去皮金合欢木";
            lang["block.strippedDarkOakWood"] = "去皮深色橡木";

            // 物品
            lang["item.apple"] = "苹果";
            lang["item.bread"] = "面包";
            lang["item.cookedPorkchop"] = "熟猪排";
            lang["item.rawPorkchop"] = "生猪排";
            lang["item.cookedBeef"] = "牛排";
            lang["item.rawBeef"] = "生牛肉";
            lang["item.cookedChicken"] = "熟鸡肉";
            lang["item.rawChicken"] = "生鸡肉";
            lang["item.cookedMutton"] = "熟羊肉";
            lang["item.rawMutton"] = "生羊肉";
            lang["item.cookedRabbit"] = "熟兔肉";
            lang["item.rawRabbit"] = "生兔肉";
            lang["item.cookedCod"] = "熟鳕鱼";
            lang["item.rawCod"] = "生鳕鱼";
            lang["item.cookedSalmon"] = "熟鲑鱼";
            lang["item.rawSalmon"] = "生鲑鱼";
            lang["item.tropicalFish"] = "热带鱼";
            lang["item.pufferfish"] = "河豚";
            lang["item.cookie"] = "曲奇";
            lang["item.cake"] = "蛋糕";
            lang["item.melonSlice"] = "西瓜片";
            lang["item.pumpkinPie"] = "南瓜派";
            lang["item.mushroomStew"] = "蘑菇煲";
            lang["item.rabbitStew"] = "兔肉煲";
            lang["item.beetrootSoup"] = "甜菜汤";
            lang["item.beetroot"] = "甜菜根";
            lang["item.potato"] = "马铃薯";
            lang["item.bakedPotato"] = "烤马铃薯";
            lang["item.poisonousPotato"] = "毒马铃薯";
            lang["item.carrot"] = "胡萝卜";
            lang["item.goldenCarrot"] = "金胡萝卜";
            lang["item.goldenApple"] = "金苹果";
            lang["item.enchantedGoldenApple"] = "附魔金苹果";
            lang["item.milkBucket"] = "牛奶桶";
            lang["item.waterBucket"] = "水桶";
            lang["item.lavaBucket"] = "岩浆桶";
            lang["item.bucket"] = "桶";
            lang["item.coal"] = "煤炭";
            lang["item.charcoal"] = "木炭";
            lang["item.ironIngot"] = "铁锭";
            lang["item.goldIngot"] = "金锭";
            lang["item.diamond"] = "钻石";
            lang["item.emerald"] = "绿宝石";
            lang["item.lapisLazuli"] = "青金石";
            lang["item.redstone"] = "红石";
            lang["item.quartz"] = "石英";
            lang["item.copperIngot"] = "铜锭";
            lang["item.netheriteIngot"] = "下界合金锭";
            lang["item.netheriteScrap"] = "下界合金碎片";
            lang["item.stick"] = "木棍";
            lang["item.string"] = "线";
            lang["item.leather"] = "皮革";
            lang["item.feather"] = "羽毛";
            lang["item.bone"] = "骨头";
            lang["item.boneMeal"] = "骨粉";
            lang["item.arrow"] = "箭";
            lang["item.bow"] = "弓";
            lang["item.crossbow"] = "弩";
            lang["item.fishingRod"] = "钓鱼竿";
            lang["item.shears"] = "剪刀";
            lang["item.flintAndSteel"] = "打火石";
            lang["item.flint"] = "燧石";
            lang["item.clayBall"] = "黏土球";
            lang["item.brick"] = "红砖";
            lang["item.netherBrick"] = "下界砖";
            lang["item.glowstoneDust"] = "萤石粉";
            lang["item.blazeRod"] = "烈焰棒";
            lang["item.blazePowder"] = "烈焰粉";
            lang["item.ghastTear"] = "恶魂之泪";
            lang["item.goldNugget"] = "金粒";
            lang["item.ironNugget"] = "铁粒";
            lang["item.enderPearl"] = "末影珍珠";
            lang["item.enderEye"] = "末影之眼";
            lang["item.fireCharge"] = "火焰弹";
            lang["item.fireworkRocket"] = "烟花火箭";
            lang["item.fireworkStar"] = "烟花之星";
            lang["item.netherStar"] = "下界之星";
            lang["item.dragonEgg"] = "龙蛋";
            lang["item.elytra"] = "鞘翅";
            lang["item.shield"] = "盾牌";
            lang["item.totemOfUndying"] = "不死图腾";
            lang["item.shulkerShell"] = "潜影壳";
            lang["item.phantomMembrane"] = "幻翼膜";
            lang["item.nautilusShell"] = "鹦鹉螺壳";
            lang["item.heartOfTheSea"] = "海洋之心";
            lang["item.trident"] = "三叉戟";
            lang["item.saddle"] = "鞍";
            lang["item.nameTag"] = "命名牌";
            lang["item.lead"] = "拴绳";
            lang["item.horseArmorLeather"] = "皮革马铠";
            lang["item.horseArmorIron"] = "铁马铠";
            lang["item.horseArmorGold"] = "金马铠";
            lang["item.horseArmorDiamond"] = "钻石马铠";
            lang["item.map"] = "地图";
            lang["item.filledMap"] = "已定位的地图";
            lang["item.compass"] = "指南针";
            lang["item.clock"] = "钟";
            lang["item.book"] = "书";
            lang["item.writableBook"] = "书与笔";
            lang["item.writtenBook"] = "成书";
            lang["item.enchantedBook"] = "附魔书";
            lang["item.paper"] = "纸";
            lang["item.itemFrame"] = "物品展示框";
            lang["item.painting"] = "画";
            lang["item.sign"] = "告示牌";
            lang["item.snowball"] = "雪球";
            lang["item.egg"] = "鸡蛋";
            lang["item.spawnEgg"] = "刷怪蛋";
            lang["item.experienceBottle"] = "附魔之瓶";
            lang["item.potion"] = "药水";
            lang["item.splashPotion"] = "喷溅药水";
            lang["item.lingeringPotion"] = "滞留药水";
            lang["item.glassBottle"] = "玻璃瓶";
            lang["item.fermentedSpiderEye"] = "发酵蛛眼";
            lang["item.spiderEye"] = "蜘蛛眼";
            lang["item.rottenFlesh"] = "腐肉";
            lang["item.gunpowder"] = "火药";
            lang["item.string"] = "线";
            lang["item.slimeball"] = "史莱姆球";
            lang["item.magmaCream"] = "岩浆膏";
            lang["item.ghastTear"] = "恶魂之泪";
            lang["item.rabbitFoot"] = "兔子脚";
            lang["item.rabbitHide"] = "兔子皮";
            lang["item.batSpawnEgg"] = "蝙蝠刷怪蛋";
            lang["item.blazeSpawnEgg"] = "烈焰人刷怪蛋";
            lang["item.caveSpiderSpawnEgg"] = "洞穴蜘蛛刷怪蛋";
            lang["item.chickenSpawnEgg"] = "鸡刷怪蛋";
            lang["item.codSpawnEgg"] = "鳕鱼刷怪蛋";
            lang["item.cowSpawnEgg"] = "牛刷怪蛋";
            lang["item.creeperSpawnEgg"] = "爬行者刷怪蛋";
            lang["item.dolphinSpawnEgg"] = "海豚刷怪蛋";
            lang["item.donkeySpawnEgg"] = "驴刷怪蛋";
            lang["item.drownedSpawnEgg"] = "溺尸刷怪蛋";
            lang["item.elderGuardianSpawnEgg"] = "远古守卫者刷怪蛋";
            lang["item.endermanSpawnEgg"] = "末影人刷怪蛋";
            lang["item.endermiteSpawnEgg"] = "末影螨刷怪蛋";
            lang["item.evokerSpawnEgg"] = "唤魔者刷怪蛋";
            lang["item.foxSpawnEgg"] = "狐狸刷怪蛋";
            lang["item.ghastSpawnEgg"] = "恶魂刷怪蛋";
            lang["item.guardianSpawnEgg"] = "守卫者刷怪蛋";
            lang["item.horseSpawnEgg"] = "马刷怪蛋";
            lang["item.huskSpawnEgg"] = "尸壳刷怪蛋";
            lang["item.llamaSpawnEgg"] = "羊驼刷怪蛋";
            lang["item.magmaCubeSpawnEgg"] = "岩浆怪刷怪蛋";
            lang["item.mooshroomSpawnEgg"] = "哞菇刷怪蛋";
            lang["item.muleSpawnEgg"] = "骡子刷怪蛋";
            lang["item.ocelotSpawnEgg"] = "豹猫刷怪蛋";
            lang["item.parrotSpawnEgg"] = "鹦鹉刷怪蛋";
            lang["item.phantomSpawnEgg"] = "幻翼刷怪蛋";
            lang["item.pigSpawnEgg"] = "猪刷怪蛋";
            lang["item.pillagerSpawnEgg"] = "掠夺者刷怪蛋";
            lang["item.polarBearSpawnEgg"] = "北极熊刷怪蛋";
            lang["item.pufferfishSpawnEgg"] = "河豚刷怪蛋";
            lang["item.rabbitSpawnEgg"] = "兔子刷怪蛋";
            lang["item.ravagerSpawnEgg"] = "劫掠兽刷怪蛋";
            lang["item.salmonSpawnEgg"] = "鲑鱼刷怪蛋";
            lang["item.sheepSpawnEgg"] = "羊刷怪蛋";
            lang["item.shulkerSpawnEgg"] = "潜影贝刷怪蛋";
            lang["item.silverfishSpawnEgg"] = "蠹虫刷怪蛋";
            lang["item.skeletonSpawnEgg"] = "骷髅刷怪蛋";
            lang["item.skeletonHorseSpawnEgg"] = "骷髅马刷怪蛋";
            lang["item.slimeSpawnEgg"] = "史莱姆刷怪蛋";
            lang["item.spiderSpawnEgg"] = "蜘蛛刷怪蛋";
            lang["item.squidSpawnEgg"] = "鱿鱼刷怪蛋";
            lang["item.straySpawnEgg"] = "流浪者刷怪蛋";
            lang["item.tropicalFishSpawnEgg"] = "热带鱼刷怪蛋";
            lang["item.turtleSpawnEgg"] = "海龟刷怪蛋";
            lang["item.vexSpawnEgg"] = "恼鬼刷怪蛋";
            lang["item.villagerSpawnEgg"] = "村民刷怪蛋";
            lang["item.vindicatorSpawnEgg"] = "卫道士刷怪蛋";
            lang["item.wanderingTraderSpawnEgg"] = "流浪商人刷怪蛋";
            lang["item.witchSpawnEgg"] = "女巫刷怪蛋";
            lang["item.witherSkeletonSpawnEgg"] = "凋灵骷髅刷怪蛋";
            lang["item.wolfSpawnEgg"] = "狼刷怪蛋";
            lang["item.zombieSpawnEgg"] = "僵尸刷怪蛋";
            lang["item.zombieHorseSpawnEgg"] = "僵尸马刷怪蛋";
            lang["item.zombieVillagerSpawnEgg"] = "僵尸村民刷怪蛋";
            lang["item.zombifiedPiglinSpawnEgg"] = "僵尸猪灵刷怪蛋";
            lang["item.piglinSpawnEgg"] = "猪灵刷怪蛋";
            lang["item.hoglinSpawnEgg"] = "疣猪兽刷怪蛋";
            lang["item.piglinBruteSpawnEgg"] = "猪灵蛮兵刷怪蛋";
            lang["item.zoglinSpawnEgg"] = "僵尸疣猪兽刷怪蛋";
            lang["item.striderSpawnEgg"] = "炽足兽刷怪蛋";
            lang["item.beeSpawnEgg"] = "蜜蜂刷怪蛋";
            lang["item.axolotlSpawnEgg"] = "美西螈刷怪蛋";
            lang["item.goatSpawnEgg"] = "山羊刷怪蛋";
            lang["item.glowSquidSpawnEgg"] = "发光鱿鱼刷怪蛋";
            lang["item.marker"] = "标记";
            lang["item.musicDisc"] = "音乐唱片";
            lang["item.musicDisc13"] = "音乐唱片 - 13";
            lang["item.musicDiscCat"] = "音乐唱片 - cat";
            lang["item.musicDiscBlocks"] = "音乐唱片 - blocks";
            lang["item.musicDiscChirp"] = "音乐唱片 - chirp";
            lang["item.musicDiscFar"] = "音乐唱片 - far";
            lang["item.musicDiscMall"] = "音乐唱片 - mall";
            lang["item.musicDiscMellohi"] = "音乐唱片 - mellohi";
            lang["item.musicDiscStal"] = "音乐唱片 - stal";
            lang["item.musicDiscStrad"] = "音乐唱片 - strad";
            lang["item.musicDiscWard"] = "音乐唱片 - ward";
            lang["item.musicDisc11"] = "音乐唱片 - 11";
            lang["item.musicDiscWait"] = "音乐唱片 - wait";
            lang["item.musicDiscOtherside"] = "音乐唱片 - otherside";
            lang["item.musicDiscPigstep"] = "音乐唱片 - Pigstep";

            // 工具
            lang["item.woodenSword"] = "木剑";
            lang["item.stoneSword"] = "石剑";
            lang["item.ironSword"] = "铁剑";
            lang["item.goldenSword"] = "金剑";
            lang["item.diamondSword"] = "钻石剑";
            lang["item.netheriteSword"] = "下界合金剑";
            lang["item.woodenPickaxe"] = "木镐";
            lang["item.stonePickaxe"] = "石镐";
            lang["item.ironPickaxe"] = "铁镐";
            lang["item.goldenPickaxe"] = "金镐";
            lang["item.diamondPickaxe"] = "钻石镐";
            lang["item.netheritePickaxe"] = "下界合金镐";
            lang["item.woodenAxe"] = "木斧";
            lang["item.stoneAxe"] = "石斧";
            lang["item.ironAxe"] = "铁斧";
            lang["item.goldenAxe"] = "金斧";
            lang["item.diamondAxe"] = "钻石斧";
            lang["item.netheriteAxe"] = "下界合金斧";
            lang["item.woodenShovel"] = "木锹";
            lang["item.stoneShovel"] = "石锹";
            lang["item.ironShovel"] = "铁锹";
            lang["item.goldenShovel"] = "金锹";
            lang["item.diamondShovel"] = "钻石锹";
            lang["item.netheriteShovel"] = "下界合金锹";
            lang["item.woodenHoe"] = "木锄";
            lang["item.stoneHoe"] = "石锄";
            lang["item.ironHoe"] = "铁锄";
            lang["item.goldenHoe"] = "金锄";
            lang["item.diamondHoe"] = "钻石锄";
            lang["item.netheriteHoe"] = "下界合金锄";

            // 护甲
            lang["item.leatherHelmet"] = "皮革帽子";
            lang["item.leatherChestplate"] = "皮革外套";
            lang["item.leatherLeggings"] = "皮革裤子";
            lang["item.leatherBoots"] = "皮革靴子";
            lang["item.ironHelmet"] = "铁头盔";
            lang["item.ironChestplate"] = "铁胸甲";
            lang["item.ironLeggings"] = "铁护腿";
            lang["item.ironBoots"] = "铁靴子";
            lang["item.goldenHelmet"] = "金头盔";
            lang["item.goldenChestplate"] = "金胸甲";
            lang["item.goldenLeggings"] = "金护腿";
            lang["item.goldenBoots"] = "金靴子";
            lang["item.diamondHelmet"] = "钻石头盔";
            lang["item.diamondChestplate"] = "钻石胸甲";
            lang["item.diamondLeggings"] = "钻石护腿";
            lang["item.diamondBoots"] = "钻石靴子";
            lang["item.netheriteHelmet"] = "下界合金头盔";
            lang["item.netheriteChestplate"] = "下界合金胸甲";
            lang["item.netheriteLeggings"] = "下界合金护腿";
            lang["item.netheriteBoots"] = "下界合金靴子";
            lang["item.turtleShell"] = "海龟壳";

            // 实体
            lang["entity.zombie"] = "僵尸";
            lang["entity.skeleton"] = "骷髅";
            lang["entity.creeper"] = "爬行者";
            lang["entity.spider"] = "蜘蛛";
            lang["entity.enderman"] = "末影人";
            lang["entity.cow"] = "牛";
            lang["entity.pig"] = "猪";
            lang["entity.sheep"] = "羊";
            lang["entity.chicken"] = "鸡";
            lang["entity.rabbit"] = "兔子";
            lang["entity.wolf"] = "狼";
            lang["entity.villager"] = "村民";
            lang["entity.player"] = "玩家";
            lang["entity.item"] = "物品";
            lang["entity.xpOrb"] = "经验球";
            lang["entity.tnt"] = "TNT";
            lang["entity.fallingBlock"] = "下落的方块";
            lang["entity.arrow"] = "箭";
            lang["entity.snowball"] = "雪球";
            lang["entity.egg"] = "鸡蛋";
            lang["entity.enderPearl"] = "末影珍珠";
            lang["entity.experienceBottle"] = "附魔之瓶";
            lang["entity.fireworkRocket"] = "烟花火箭";
            lang["entity.fireball"] = "火球";
            lang["entity.smallFireball"] = "小火球";
            lang["entity.witherSkull"] = "凋灵骷髅头颅";
            lang["entity.shulkerBullet"] = "潜影贝导弹";
            lang["entity.dragonFireball"] = "龙息";
            lang["entity.trident"] = "三叉戟";
            lang["entity.boat"] = "船";
            lang["entity.minecart"] = "矿车";
            lang["entity.chestMinecart"] = "运输矿车";
            lang["entity.furnaceMinecart"] = "动力矿车";
            lang["entity.tntMinecart"] = "TNT矿车";
            lang["entity.hopperMinecart"] = "漏斗矿车";
            lang["entity.commandBlockMinecart"] = "命令方块矿车";
            lang["entity.spawnerMinecart"] = "刷怪笼矿车";

            // 生物群系
            lang["biome.ocean"] = "海洋";
            lang["biome.plains"] = "平原";
            lang["biome.desert"] = "沙漠";
            lang["biome.mountains"] = "山地";
            lang["biome.forest"] = "森林";
            lang["biome.taiga"] = "针叶林";
            lang["biome.swamp"] = "沼泽";
            lang["biome.river"] = "河流";
            lang["biome.netherWastes"] = "下界荒地";
            lang["biome.theEnd"] = "末地";
            lang["biome.frozenOcean"] = "冻洋";
            lang["biome.frozenRiver"] = "冻河";
            lang["biome.snowyTundra"] = "雪原";
            lang["biome.snowyMountains"] = "雪山";
            lang["biome.mushroomFields"] = "蘑菇岛";
            lang["biome.mushroomFieldShore"] = "蘑菇岛岸";
            lang["biome.beach"] = "海滩";
            lang["biome.desertHills"] = "沙漠丘陵";
            lang["biome.woodedHills"] = "繁茂的丘陵";
            lang["biome.taigaHills"] = "针叶林丘陵";
            lang["biome.mountainEdge"] = "山地边缘";
            lang["biome.jungle"] = "丛林";
            lang["biome.jungleHills"] = "丛林丘陵";
            lang["biome.jungleEdge"] = "丛林边缘";
            lang["biome.deepOcean"] = "深海";
            lang["biome.stoneShore"] = "石岸";
            lang["biome.snowyBeach"] = "积雪的沙滩";
            lang["biome.birchForest"] = "白桦林";
            lang["biome.birchForestHills"] = "白桦林丘陵";
            lang["biome.darkForest"] = "黑森林";
            lang["biome.snowyTaiga"] = "积雪的针叶林";
            lang["biome.snowyTaigaHills"] = "积雪的针叶林丘陵";
            lang["biome.giantTreeTaiga"] = "巨型针叶林";
            lang["biome.giantTreeTaigaHills"] = "巨型针叶林丘陵";
            lang["biome.woodedMountains"] = "繁茂的山地";
            lang["biome.savanna"] = "热带草原";
            lang["biome.savannaPlateau"] = "热带高原";
            lang["biome.badlands"] = "恶地";
            lang["biome.woodedBadlandsPlateau"] = "繁茂的恶地高原";
            lang["biome.badlandsPlateau"] = "恶地高原";
            lang["biome.warmOcean"] = "暖水海洋";
            lang["biome.lukewarmOcean"] = "温水海洋";
            lang["biome.coldOcean"] = "冷水海洋";
            lang["biome.deepWarmOcean"] = "暖水深海";
            lang["biome.deepLukewarmOcean"] = "温水深海";
            lang["biome.deepColdOcean"] = "冷水深海";
            lang["biome.deepFrozenOcean"] = "冻洋深海";
            lang["biome.theVoid"] = "虚空";
            lang["biome.sunflowerPlains"] = "向日葵平原";
            lang["biome.desertLakes"] = "沙漠湖泊";
            lang["biome.gravellyMountains"] = "沙砾山地";
            lang["biome.flowerForest"] = "繁花森林";
            lang["biome.taigaMountains"] = "针叶林山地";
            lang["biome.swampHills"] = "沼泽丘陵";
            lang["biome.iceSpikes"] = "冰刺平原";
            lang["biome.modifiedJungle"] = "变种丛林";
            lang["biome.modifiedJungleEdge"] = "变种丛林边缘";
            lang["biome.tallBirchForest"] = "高大白桦林";
            lang["biome.tallBirchHills"] = "高大白桦林丘陵";
            lang["biome.darkForestHills"] = "黑森林丘陵";
            lang["biome.snowyTaigaMountains"] = "积雪的针叶林山地";
            lang["biome.giantSpruceTaiga"] = "巨型云杉针叶林";
            lang["biome.giantSpruceTaigaHills"] = "巨型云杉针叶林丘陵";
            lang["biome.gravellyMountainsPlus"] = "沙砾山地+";
            lang["biome.shatteredSavanna"] = "破碎的热带草原";
            lang["biome.shatteredSavannaPlateau"] = "破碎的热带高原";
            lang["biome.erodedBadlands"] = "被风蚀的恶地";
            lang["biome.modifiedWoodedBadlandsPlateau"] = "变种繁茂的恶地高原";
            lang["biome.modifiedBadlandsPlateau"] = "变种恶地高原";
            lang["biome.bambooJungle"] = "竹林";
            lang["biome.bambooJungleHills"] = "竹林丘陵";
            lang["biome.soulSandValley"] = "灵魂沙峡谷";
            lang["biome.crimsonForest"] = "绯红森林";
            lang["biome.warpedForest"] = "诡异森林";
            lang["biome.basaltDeltas"] = "玄武岩三角洲";
            lang["biome.dripstoneCaves"] = "溶洞";
            lang["biome.lushCaves"] = "繁茂洞穴";
            lang["biome.deepDark"] = "深暗之域";
            lang["biome.meadow"] = "草甸";
            lang["biome.grove"] = "雪林";
            lang["biome.snowySlopes"] = "积雪山坡";
            lang["biome.jaggedPeaks"] = "尖峭山峰";
            lang["biome.frozenPeaks"] = "冰封山峰";
            lang["biome.stonyPeaks"] = "裸岩山峰";
            lang["biome.oldGrowthBirchForest"] = "原始白桦林";
            lang["biome.oldGrowthPineTaiga"] = "原始松木针叶林";
            lang["biome.oldGrowthSpruceTaiga"] = "原始云杉针叶林";
            lang["biome.windsweptHills"] = "风蚀丘陵";
            lang["biome.windsweptForest"] = "风蚀森林";
            lang["biome.windsweptGravellyHills"] = "风蚀沙砾丘陵";
            lang["biome.windsweptSavanna"] = "风蚀热带草原";
            lang["biome.mangroveSwamp"] = "红树林沼泽";

            // 附魔
            lang["enchantment.protection"] = "保护";
            lang["enchantment.fireProtection"] = "火焰保护";
            lang["enchantment.featherFalling"] = "摔落保护";
            lang["enchantment.blastProtection"] = "爆炸保护";
            lang["enchantment.projectileProtection"] = "弹射物保护";
            lang["enchantment.thorns"] = "荆棘";
            lang["enchantment.respiration"] = "水下呼吸";
            lang["enchantment.aquaAffinity"] = "水下速掘";
            lang["enchantment.depthStrider"] = "深海探索者";
            lang["enchantment.frostWalker"] = "冰霜行者";
            lang["enchantment.bindingCurse"] = "绑定诅咒";
            lang["enchantment.sharpness"] = "锋利";
            lang["enchantment.smite"] = "亡灵杀手";
            lang["enchantment.baneOfArthropods"] = "节肢杀手";
            lang["enchantment.knockback"] = "击退";
            lang["enchantment.fireAspect"] = "火焰附加";
            lang["enchantment.looting"] = "抢夺";
            lang["enchantment.sweepingEdge"] = "横扫之刃";
            lang["enchantment.efficiency"] = "效率";
            lang["enchantment.silkTouch"] = "精准采集";
            lang["enchantment.unbreaking"] = "耐久";
            lang["enchantment.fortune"] = "时运";
            lang["enchantment.power"] = "力量";
            lang["enchantment.punch"] = "冲击";
            lang["enchantment.flame"] = "火矢";
            lang["enchantment.infinity"] = "无限";
            lang["enchantment.luckOfTheSea"] = "海之眷顾";
            lang["enchantment.lure"] = "饵钓";
            lang["enchantment.loyalty"] = "忠诚";
            lang["enchantment.impaling"] = "穿刺";
            lang["enchantment.riptide"] = "激流";
            lang["enchantment.channeling"] = "引雷";
            lang["enchantment.multishot"] = "多重射击";
            lang["enchantment.quickCharge"] = "快速装填";
            lang["enchantment.piercing"] = "穿透";
            lang["enchantment.mending"] = "经验修补";
            lang["enchantment.vanishingCurse"] = "消失诅咒";
            lang["enchantment.soulSpeed"] = "灵魂疾行";

            // 进度
            lang["advancements.story.root"] = "我的世界";
            lang["advancements.story.mineStone"] = "石器时代";
            lang["advancements.story.upgradeTools"] = "获得升级";
            lang["advancements.story.smeltIron"] = "来硬的";
            lang["advancements.story.buildPickaxe"] = "这不是铁镐么";
            lang["advancements.story.obtainArmor"] = "整装上阵";
            lang["advancements.story.lavaBucket"] = "热腾腾的";
            lang["advancements.story.ironToDiamond"] = "钻石！";
            lang["advancements.story.enterNether"] = "我们需要再深入些";
            lang["advancements.story.shinyGear"] = "闪闪发光";
            lang["advancements.nether.root"] = "下界";
            lang["advancements.nether.returnToSender"] = "谁在切洋葱？";
            lang["advancements.nether.findBastion"] = "那些是目标吗？";
            lang["advancements.nether.lootBastion"] = "战利品猎人";
            lang["advancements.nether.obtainAncientDebris"] = "深藏不露";
            lang["advancements.nether.netherite"] = "深藏不露";
            lang["advancements.nether.allEffects"] = "为什么会变成这样呢？";
            lang["advancements.nether.fastTravel"] = "狂奔";
            lang["advancements.nether.uneasyAlliance"] = "不稳定的同盟";
            lang["advancements.nether.rideStrider"] = "脚下留情";
            lang["advancements.nether.ghast"] = "见鬼去吧";
            lang["advancements.nether.wither"] = "带点恶魂之泪回家";
            lang["advancements.end.root"] = "末地";
            lang["advancements.end.killDragon"] = "解放末地";
            lang["advancements.end.dragonEgg"] = "下一世代";
            lang["advancements.end.respawnDragon"] = "那是飞机吗？";
            lang["advancements.end.elytra"] = "在宇宙的尽头";
            lang["advancements.end.dragonBreath"] = "你需要来点薄荷吗？";
            lang["advancements.adventure.root"] = "冒险";
            lang["advancements.adventure.voluntaryExile"] = "自愿的流亡";
            lang["advancements.adventure.spyglassAtParrot"] = "望远镜里的鹦鹉";
            lang["advancements.adventure.avoidVibration"] = "非常静谧";
            lang["advancements.adventure.walkOnPowderSnowWithLeatherBoots"] = "穿着皮革靴子走在细雪上";
            lang["advancements.adventure.lightningRodWithTrident"] = "用三叉戟击中避雷针";
            lang["advancements.adventure.killMobNearSculkCatalyst"] = "在幽匿催发体附近杀死生物";
            lang["advancements.adventure.bullseye"] = "正中靶心";
            lang["advancements.adventure.summonIronGolem"] = "英雄住在这里";
            lang["advancements.adventure.summonWither"] = "开始了";
            lang["advancements.adventure.killWither"] = "下界之星";
            lang["advancements.adventure.trade"] = "成交！";
            lang["advancements.adventure.throwTrident"] = "一去不返";
            lang["advancements.adventure.shootArrow"] = "瞄准目标";
            lang["advancements.adventure.sniperDuel"] = "狙击手的对决";
            lang["advancements.adventure.totemOfUndying"] = "不死的图腾";
            lang["advancements.adventure.heroOfTheVillage"] = "村庄英雄";
            lang["advancements.husbandry.root"] = "农牧业";
            lang["advancements.husbandry.safelyHarvestHoney"] = "安全获取蜂蜜";
            lang["advancements.husbandry.axolotlInABucket"] = "最可爱的捕食者";
            lang["advancements.husbandry.killAxolotlTarget"] = "两栖动物的胜利";
            lang["advancements.husbandry.breedAllAnimals"] = "成双成对";
            lang["advancements.husbandry.plantSeed"] = "播种";
            lang["advancements.husbandry.bread"] = "填饱肚子";
            lang["advancements.husbandry.bakeCake"] = "蛋糕是谎言";
            lang["advancements.husbandry.rideBoat"] = "水上漂";
            lang["advancements.husbandry.tameAnimal"] = "永恒的伙伴";
            lang["advancements.husbandry.breedAnimal"] = "我还能养点什么？";
            lang["advancements.husbandry.netheriteHoes"] = "最珍贵的物品";
            lang["advancements.husbandry.balancedDiet"] = "均衡饮食";
            lang["advancements.husbandry.netheriteAllay"] = "和悦灵共舞";
            lang["advancements.husbandry.allayDeliverItem"] = "递送物品";
            lang["advancements.husbandry.leashAllFrogVariants"] = "拴住所有青蛙变种";

            // 状态效果
            lang["effect.absorption"] = "伤害吸收";
            lang["effect.badOmen"] = "不祥之兆";
            lang["effect.blindness"] = "失明";
            lang["effect.conduitPower"] = "潮涌能量";
            lang["effect.darkness"] = "黑暗";
            lang["effect.dolphinsGrace"] = "海豚的恩惠";
            lang["effect.fireResistance"] = "抗火";
            lang["effect.glowing"] = "发光";
            lang["effect.haste"] = "急迫";
            lang["effect.healthBoost"] = "生命提升";
            lang["effect.heroOfTheVillage"] = "村庄英雄";
            lang["effect.hunger"] = "饥饿";
            lang["effect.instantDamage"] = "瞬间伤害";
            lang["effect.instantHealth"] = "瞬间治疗";
            lang["effect.invisibility"] = "隐身";
            lang["effect.jumpBoost"] = "跳跃提升";
            lang["effect.levitation"] = "飘浮";
            lang["effect.luck"] = "幸运";
            lang["effect.miningFatigue"] = "挖掘疲劳";
            lang["effect.nausea"] = "恶心";
            lang["effect.nightVision"] = "夜视";
            lang["effect.poison"] = "中毒";
            lang["effect.regeneration"] = "生命恢复";
            lang["effect.resistance"] = "抗性提升";
            lang["effect.saturation"] = "饱和";
            lang["effect.slowFalling"] = "缓降";
            lang["effect.slowness"] = "缓慢";
            lang["effect.speed"] = "速度";
            lang["effect.strength"] = "力量";
            lang["effect.unluck"] = "霉运";
            lang["effect.waterBreathing"] = "水下呼吸";
            lang["effect.weakness"] = "虚弱";
            lang["effect.wither"] = "凋零";

            // 死亡消息
            lang["death.attack.generic"] = "%1$s 死了";
            lang["death.attack.player"] = "%1$s 被 %2$s 杀死了";
            lang["death.attack.mob"] = "%1$s 被 %2$s 杀死了";
            lang["death.fell.accident.generic"] = "%1$s 从高处摔了下来";
            lang["death.fell.accident.ladder"] = "%1$s 从梯子上摔了下来";
            lang["death.fell.accident.vines"] = "%1$s 从藤蔓上摔了下来";
            lang["death.fell.accident.water"] = "%1$s 从水面上摔了下来";
            lang["death.fell.killer"] = "%1$s 被 %2$s 从高处推了下来";
            lang["death.fell.finish"] = "%1$s 摔死了";
            lang["death.drown"] = "%1$s 淹死了";
            lang["death.drown.player"] = "%1$s 被 %2$s 按在水里淹死了";
            lang["death.lava"] = "%1$s 试图在岩浆里游泳";
            lang["death.lava.player"] = "%1$s 被 %2$s 推进了岩浆里";
            lang["death.onFire"] = "%1$s 被烧死了";
            lang["death.onFire.player"] = "%1$s 被 %2$s 烧死了";
            lang["death.inFire"] = "%1$s 被火烧死了";
            lang["death.inFire.player"] = "%1$s 被 %2$s 推进了火里";
            lang["death.starve"] = "%1$s 饿死了";
            lang["death.wither"] = "%1$s 枯萎了";
            lang["death.poison"] = "%1$s 被毒死了";
            lang["death.magic"] = "%1$s 被魔法杀死了";
            lang["death.magic.player"] = "%1$s 被 %2$s 用魔法杀死了";
            lang["death.explosion"] = "%1$s 被炸死了";
            lang["death.explosion.player"] = "%1$s 被 %2$s 炸死了";
            lang["death.void"] = "%1$s 掉入了虚空";
            lang["death.void.player"] = "%1$s 被 %2$s 推入了虚空";
            lang["death.cactus"] = "%1$s 被仙人掌扎死了";
            lang["death.cactus.player"] = "%1$s 被 %2$s 推到了仙人掌上";
            lang["death.cramming"] = "%1$s 被挤死了";
            lang["death.dragonBreath"] = "%1$s 被龙息杀死了";
            lang["death.dryout"] = "%1$s 在水中待太久了";
            lang["death.electrocution"] = "%1$s 被电死了";
            lang["death.fallingBlock"] = "%1$s 被下落的方块砸死了";
            lang["death.fireworks"] = "%1$s 被烟花炸死了";
            lang["death.flyIntoWall"] = "%1$s 撞上了墙";
            lang["death.freeze"] = "%1$s 冻死了";
            lang["death.hotFloor"] = "%1$s 站在了太热的方块上";
            lang["death.lightning"] = "%1$s 被闪电击中了";
            lang["death.magma"] = "%1$s 发现了岩浆块的秘密";
            lang["death.noWater"] = "%1$s 离开了水";
            lang["death.outOfWorld"] = "%1$s 发现了世界的边界";
            lang["death.outOfRespawn"] = "%1$s 没有可用的重生点";
            lang["death.stalactite"] = "%1$s 被钟乳石刺死了";
            lang["death.stalagmite"] = "%1$s 被石笋刺死了";
            lang["death.sting"] = "%1$s 被蜇死了";
            lang["death.thorns"] = "%1$s 被荆棘反伤致死";
            lang["death.trident"] = "%1$s 被三叉戟刺死了";
            lang["death.witherRose"] = "%1$s 触碰了凋零玫瑰";
            lang["death.sweetBerryBush"] = "%1$s 被甜浆果丛刺死了";
            lang["death.anvil"] = "%1$s 被铁砧砸死了";

            // 聊天
            lang["chat.type.advancement.task"] = "%1$s 达成了进度 [%2$s]";
            lang["chat.type.advancement.goal"] = "%1$s 达成了目标 [%2$s]";
            lang["chat.type.advancement.challenge"] = "%1$s 完成了挑战 [%2$s]";
            lang["chat.type.announcement"] = "[公告] %1$s";
            lang["chat.type.emote"] = "* %1$s %2$s";
            lang["chat.type.text"] = "<%1$s> %2$s";
            lang["chat.type.admin"] = "[%1$s: %2$s]";
            lang["chat.stream.text"] = "<%1$s> %2$s";
            lang["chat.type.incoming"] = "%1$s  whispers to you: %2$s";
            lang["chat.type.outgoing"] = "You whisper to %1$s: %2$s";
            lang["chat.type.team.text"] = "<%1$s> %2$s";
            lang["chat.type.team.sent"] = "You whisper to %1$s: %2$s";
            lang["chat.type.team.incoming"] = "%1$s whispers to you: %2$s";

            // 多人游戏
            lang["multiplayer.title"] = "多人游戏";
            lang["multiplayer.addServer"] = "添加服务器";
            lang["multiplayer.directConnect"] = "直接连接";
            lang["multiplayer.serverName"] = "服务器名称";
            lang["multiplayer.serverAddress"] = "服务器地址";
            lang["multiplayer.serverResourcePacks"] = "服务器资源包";
            lang["multiplayer.serverResourcePacks.prompt"] = "此服务器需要资源包";
            lang["multiplayer.serverResourcePacks.enabled"] = "已启用";
            lang["multiplayer.serverResourcePacks.disabled"] = "已禁用";
            lang["multiplayer.joinedGame"] = "%1$s 加入了游戏";
            lang["multiplayer.leftGame"] = "%1$s 离开了游戏";
            lang["multiplayer.playerList"] = "玩家列表";

            // 统计
            lang["stat.generic"] = "通用";
            lang["stat.items"] = "物品";
            lang["stat.mobs"] = "生物";
            lang["stat.blocks"] = "方块";
            lang["stat.playOneMinute"] = "游戏时间";
            lang["stat.timeSinceDeath"] = "上次死亡以来的时间";
            lang["stat.timeSinceRest"] = "上次睡觉以来的时间";
            lang["stat.sneakTime"] = "潜行时间";
            lang["stat.walkOneCm"] = "步行距离";
            lang["stat.crouchOneCm"] = "潜行距离";
            lang["stat.sprintOneCm"] = "疾跑距离";
            lang["stat.swimOneCm"] = "游泳距离";
            lang["stat.fallOneCm"] = "下落距离";
            lang["stat.climbOneCm"] = "攀爬距离";
            lang["stat.flyOneCm"] = "飞行距离";
            lang["stat.diveOneCm"] = "潜水距离";
            lang["stat.minecartOneCm"] = "矿车距离";
            lang["stat.boatOneCm"] = "船距离";
            lang["stat.pigOneCm"] = "骑猪距离";
            lang["stat.horseOneCm"] = "骑马距离";
            lang["stat.aviateOneCm"] = "鞘翅飞行距离";
            lang["stat.jump"] = "跳跃次数";
            lang["stat.drop"] = "掉落次数";
            lang["stat.damageDealt"] = "造成的伤害";
            lang["stat.damageTaken"] = "受到的伤害";
            lang["stat.deaths"] = "死亡次数";
            lang["stat.mobKills"] = "击杀生物数";
            lang["stat.playerKills"] = "击杀玩家数";
            lang["stat.fishCaught"] = "钓到的鱼数";
            lang["stat.talkedToVillager"] = "与村民交谈次数";
            lang["stat.tradedWithVillager"] = "与村民交易次数";
            lang["stat.cakeSlicesEaten"] = "吃掉的蛋糕片数";
            lang["stat.itemsEnchanted"] = "附魔的物品数";
            lang["stat.recordsPlayed"] = "播放的唱片数";
            lang["stat.noteblocksPlayed"] = "弹奏的音符盒数";
            lang["stat.noteblocksTuned"] = "调音的音符盒数";
            lang["stat.flowerPotted"] = "栽种的花盆数";
            lang["stat.bellRung"] = "敲响的钟数";
            lang["stat.raidTrigger"] = "触发的袭击数";
            lang["stat.raidWin"] = "赢得的袭击数";
            lang["stat.sleepInBed"] = "睡觉次数";
            lang["stat.fullness"] = "进食次数";
            lang["stat.openContainer"] = "打开的容器数";
            lang["stat.openEnderchest"] = "打开的末影箱数";
            lang["stat.playNoteblock"] = "弹奏的音符盒数";
            lang["stat.craftingTableInteraction"] = "使用工作台次数";
            lang["stat.furnaceInteraction"] = "使用熔炉次数";
            lang["stat.brewingstandInteraction"] = "使用酿造台次数";
            lang["stat.chestOpened"] = "打开的箱子数";
            lang["stat.trappedChestTriggered"] = "触发的陷阱箱数";
            lang["stat.enderchestOpened"] = "打开的末影箱数";
            lang["stat.enchantingItem"] = "附魔的物品数";
            lang["stat.furnaceTaken"] = "从熔炉取出的物品数";
            lang["stat.brewingstandTaken"] = "从酿造台取出的药水数";
            lang["stat.dispenserInspected"] = "检查的发射器数";
            lang["stat.dropperInspected"] = "检查的投掷器数";
            lang["stat.hopperInspected"] = "检查的漏斗数";
            lang["stat.dropperInserted"] = "放入投掷器的物品数";
            lang["stat.hopperInserted"] = "放入漏斗的物品数";
            lang["stat.dispenserInserted"] = "放入发射器的物品数";
            lang["stat.itemFrameRemoved"] = "从物品展示框取下的物品数";
            lang["stat.itemFramePlaced"] = "放入物品展示框的物品数";
            lang["stat.itemFrameRotated"] = "旋转物品展示框的次数";
            lang["stat.jukeboxPlayed"] = "播放的唱片数";
            lang["stat.jukeboxInserted"] = "放入唱片机的唱片数";
            lang["stat.campfireCookedFood"] = "在营火上烤的食物数";
            lang["stat.beehiveSheared"] = "剪过的蜂巢数";
            lang["stat.beehiveScraped"] = "刮过的蜂巢数";
            lang["stat.turtleEggsBroken"] = "破坏的海龟蛋数";
            lang["stat.turtleEggsHatched"] = "孵化的海龟蛋数";
            lang["stat.animalBred"] = "繁殖的动物数";
            lang["stat.animalsBred"] = "繁殖的动物数";
            lang["stat.horseCleaned"] = "清理的马数";
            lang["stat.cauldronFilled"] = "装满的炼药锅数";
            lang["stat.cauldronUsed"] = "使用的炼药锅数";
            lang["stat.cauldronWaterTaken"] = "从炼药锅取出的水量";
            lang["stat.cleanArmor"] = "清洗的护甲数";
            lang["stat.cleanBanner"] = "清洗的旗帜数";
            lang["stat.cleanShulkerBox"] = "清洗的潜影盒数";
            lang["stat.interactWithAnvil"] = "使用铁砧次数";
            lang["stat.interactWithBeacon"] = "使用信标次数";
            lang["stat.interactWithBlastFurnace"] = "使用高炉次数";
            lang["stat.interactWithBrewingstand"] = "使用酿造台次数";
            lang["stat.interactWithCartographyTable"] = "使用制图台次数";
            lang["stat.interactWithCraftingTable"] = "使用工作台次数";
            lang["stat.interactWithFurnace"] = "使用熔炉次数";
            lang["stat.interactWithGrindstone"] = "使用砂轮次数";
            lang["stat.interactWithLectern"] = "使用讲台次数";
            lang["stat.interactWithLoom"] = "使用织布机次数";
            lang["stat.interactWithSmithingTable"] = "使用锻造台次数";
            lang["stat.interactWithSmoker"] = "使用烟熏炉次数";
            lang["stat.interactWithStonecutter"] = "使用切石机次数";
            lang["stat.leaveBed"] = "离开床的次数";
            lang["stat.sleepInBed"] = "睡觉次数";
            lang["stat.openBarrel"] = "打开的木桶数";
            lang["stat.openChest"] = "打开的箱子数";
            lang["stat.openEnderChest"] = "打开的末影箱数";
            lang["stat.openShulkerBox"] = "打开的潜影盒数";
            lang["stat.playRecord"] = "播放的唱片数";
            lang["stat.playNoteblock"] = "弹奏的音符盒数";
            lang["stat.tuneNoteblock"] = "调音的音符盒数";
            lang["stat.triggerTrappedChest"] = "触发的陷阱箱数";
            lang["stat.useCauldron"] = "使用炼药锅次数";

            // 粒子
            lang["particle.ambientEntityEffect"] = "环境实体效果";
            lang["particle.angryVillager"] = "生气的村民";
            lang["particle.ash"] = "灰烬";
            lang["particle.block"] = "方块";
            lang["particle.blockMarker"] = "方块标记";
            lang["particle.bubble"] = "气泡";
            lang["particle.bubbleColumnUp"] = "气泡柱上升";
            lang["particle.bubblePop"] = "气泡破裂";
            lang["particle.cloud"] = "云";
            lang["particle.composter"] = "堆肥桶";
            lang["particle.crimsonSpore"] = "绯红孢子";
            lang["particle.crit"] = "暴击";
            lang["particle.currentDown"] = "水流向下";
            lang["particle.damageIndicator"] = "伤害指示器";
            lang["particle.dolphin"] = "海豚";
            lang["particle.dragonBreath"] = "龙息";
            lang["particle.drippingLava"] = "滴落岩浆";
            lang["particle.drippingObsidianTear"] = "滴落哭泣的黑曜石";
            lang["particle.drippingWater"] = "滴水";
            lang["particle.dust"] = "灰尘";
            lang["particle.dustColorTransition"] = "灰尘颜色过渡";
            lang["particle.effect"] = "效果";
            lang["particle.eggCrack"] = "鸡蛋破裂";
            lang["particle.elderGuardian"] = "远古守卫者";
            lang["particle.enchant"] = "附魔";
            lang["particle.enchantedHit"] = "附魔命中";
            lang["particle.endRod"] = "末地烛";
            lang["particle.entityEffect"] = "实体效果";
            lang["particle.explosion"] = "爆炸";
            lang["particle.explosionEmitter"] = "爆炸发射器";
            lang["particle.fallingDust"] = "下落的灰尘";
            lang["particle.fallingLava"] = "下落的岩浆";
            lang["particle.fallingNectar"] = "下落的花蜜";
            lang["particle.fallingObsidianTear"] = "下落的哭泣的黑曜石";
            lang["particle.fallingWater"] = "下落的水";
            lang["particle.firework"] = "烟花";
            lang["particle.fishing"] = "钓鱼";
            lang["particle.flame"] = "火焰";
            lang["particle.flash"] = "闪光";
            lang["particle.glow"] = "发光";
            lang["particle.glowSquidInk"] = "发光鱿鱼墨汁";
            lang["particle.happyVillager"] = "开心的村民";
            lang["particle.heart"] = "爱心";
            lang["particle.instantEffect"] = "瞬间效果";
            lang["particle.item"] = "物品";
            lang["particle.itemSlime"] = "史莱姆球";
            lang["particle.itemSnowball"] = "雪球";
            lang["particle.largeSmoke"] = "大烟雾";
            lang["particle.lava"] = "岩浆";
            lang["particle.mycelium"] = "菌丝";
            lang["particle.nautilus"] = "鹦鹉螺";
            lang["particle.note"] = "音符";
            lang["particle.poof"] = "噗";
            lang["particle.portal"] = "传送门";
            lang["particle.rain"] = "雨";
            lang["particle.reversePortal"] = "反向传送门";
            lang["particle.scrape"] = "刮擦";
            lang["particle.sculkCharge"] = "幽匿电荷";
            lang["particle.sculkSoul"] = "幽匿灵魂";
            lang["particle.shriek"] = "尖啸";
            lang["particle.smallFlame"] = "小火焰";
            lang["particle.smoke"] = "烟雾";
            lang["particle.sneeze"] = "喷嚏";
            lang["particle.snowflake"] = "雪花";
            lang["particle.soul"] = "灵魂";
            lang["particle.soulFireFlame"] = "灵魂火焰";
            lang["particle.spit"] = "吐";
            lang["particle.splash"] = "飞溅";
            lang["particle.sporeBlossomAir"] = "孢子花空气";
            lang["particle.squidInk"] = "鱿鱼墨汁";
            lang["particle.sweepAttack"] = "横扫攻击";
            lang["particle.totemOfUndying"] = "不死图腾";
            lang["particle.underwater"] = "水下";
            lang["particle.vibration"] = "振动";
            lang["particle.warpedSpore"] = "诡异孢子";
            lang["particle.waxOff"] = "除蜡";
            lang["particle.waxOn"] = "上蜡";
            lang["particle.whiteAsh"] = "白灰";
            lang["particle.witch"] = "女巫";

            // 其他
            lang["misc.respawn"] = "重生";
            lang["misc.titleScreen"] = "标题画面";
            lang["misc.spectator"] = "旁观者模式";
            lang["misc.debug"] = "调试";
            lang["misc.profiler"] = "性能分析器";
            lang["misc.tps"] = "每秒刻数";
            lang["misc.fps"] = "每秒帧数";
            lang["misc.ping"] = "延迟";
            lang["misc.bytes"] = "字节";
            lang["misc.kilobytes"] = "千字节";
            lang["misc.megabytes"] = "兆字节";
            lang["misc.gigabytes"] = "吉字节";
            lang["misc.seconds"] = "秒";
            lang["misc.minutes"] = "分钟";
            lang["misc.hours"] = "小时";
            lang["misc.days"] = "天";
            lang["misc.ticks"] = "刻";
            lang["misc.chunks"] = "区块";
            lang["misc.entities"] = "实体";
            lang["misc.particles"] = "粒子";
            lang["misc.memory"] = "内存";
            lang["misc.allocated"] = "已分配";
            lang["misc.used"] = "已使用";
            lang["misc.free"] = "空闲";
            lang["misc.max"] = "最大";
            lang["misc.cpu"] = "CPU";
            lang["misc.gpu"] = "GPU";
            lang["misc.opengl"] = "OpenGL";
            lang["misc.version"] = "版本";
            lang["misc.seed"] = "种子";
            lang["misc.difficulty"] = "难度";
            lang["misc.gamemode"] = "游戏模式";
            lang["misc.time"] = "时间";
            lang["misc.weather"] = "天气";
            lang["misc.biome"] = "生物群系";
            lang["misc.light"] = "光照";
            lang["misc.day"] = "白天";
            lang["misc.night"] = "夜晚";
            lang["misc.sunrise"] = "日出";
            lang["misc.sunset"] = "日落";
            lang["misc.rain"] = "雨";
            lang["misc.snow"] = "雪";
            lang["misc.thunder"] = "雷暴";
            lang["misc.clear"] = "晴天";
            lang["misc.loading"] = "加载中...";
            lang["misc.generatingWorld"] = "正在生成世界...";
            lang["misc.savingWorld"] = "正在保存世界...";
            lang["misc.loadingChunks"] = "正在加载区块...";
            lang["misc.buildingTerrain"] = "正在构建地形...";
            lang["misc.preparingSpawn"] = "正在准备出生点...";
            lang["misc.convertingWorld"] = "正在转换世界...";
            lang["misc.loadingData"] = "正在加载数据...";
            lang["misc.loadingResourcePack"] = "正在加载资源包...";
            lang["misc.initializing"] = "正在初始化...";
            lang["misc.connectingToServer"] = "正在连接服务器...";
            lang["misc.authenticating"] = "正在验证身份...";
            lang["misc.loggingIn"] = "正在登录...";
            lang["misc.downloadingTerrain"] = "正在下载地形...";
            lang["misc.joiningGame"] = "正在加入游戏...";
            lang["misc.respawning"] = "正在重生...";
            lang["misc.generatingStructures"] = "正在生成结构...";
            lang["misc.loadingEntities"] = "正在加载实体...";
            lang["misc.loadingPlayer"] = "正在加载玩家...";
            lang["misc.loadingWorld"] = "正在加载世界...";
            lang["misc.savingChunks"] = "正在保存区块...";
            lang["misc.savingPlayer"] = "正在保存玩家...";
            lang["misc.savingEntities"] = "正在保存实体...";
            lang["misc.savingWorld"] = "正在保存世界...";
            lang["misc.worldSaved"] = "世界已保存";
            lang["misc.worldLoaded"] = "世界已加载";
            lang["misc.worldCreated"] = "世界已创建";
            lang["misc.worldDeleted"] = "世界已删除";
            lang["misc.worldCopied"] = "世界已复制";
            lang["misc.worldRenamed"] = "世界已重命名";
            lang["misc.worldImported"] = "世界已导入";
            lang["misc.worldExported"] = "世界已导出";
            lang["misc.worldBackup"] = "世界备份";
            lang["misc.worldBackupCreated"] = "世界备份已创建";
            lang["misc.worldBackupRestored"] = "世界备份已恢复";
            lang["misc.worldBackupDeleted"] = "世界备份已删除";
            lang["misc.worldBackupFailed"] = "世界备份失败";
            lang["misc.worldBackupNotFound"] = "未找到世界备份";
            lang["misc.worldBackupList"] = "世界备份列表";
            lang["misc.worldBackupInfo"] = "世界备份信息";
            lang["misc.worldBackupSize"] = "世界备份大小";
            lang["misc.worldBackupDate"] = "世界备份日期";
            lang["misc.worldBackupName"] = "世界备份名称";
            lang["misc.worldBackupDescription"] = "世界备份描述";
            lang["misc.worldBackupProgress"] = "世界备份进度";
            lang["misc.worldBackupComplete"] = "世界备份完成";
            lang["misc.worldBackupCancelled"] = "世界备份已取消";
            lang["misc.worldBackupError"] = "世界备份错误";
            lang["misc.worldBackupWarning"] = "世界备份警告";
            lang["misc.worldBackupSuccess"] = "世界备份成功";
            lang["misc.worldBackupFailed"] = "世界备份失败";
            lang["misc.worldBackupTimeout"] = "世界备份超时";
            lang["misc.worldBackupInterrupted"] = "世界备份被中断";
            lang["misc.worldBackupCorrupted"] = "世界备份已损坏";
            lang["misc.worldBackupIncompatible"] = "世界备份不兼容";
            lang["misc.worldBackupOutdated"] = "世界备份已过时";
            lang["misc.worldBackupUnsupported"] = "世界备份不受支持";
            lang["misc.worldBackupUnavailable"] = "世界备份不可用";
            lang["misc.worldBackupReadOnly"] = "世界备份只读";
            lang["misc.worldBackupLocked"] = "世界备份已锁定";
            lang["misc.worldBackupProtected"] = "世界备份已保护";
            lang["misc.worldBackupEncrypted"] = "世界备份已加密";
            lang["misc.worldBackupCompressed"] = "世界备份已压缩";
            lang["misc.worldBackupSplitted"] = "世界备份已分割";
            lang["misc.worldBackupVersion"] = "世界备份版本";
            lang["misc.worldBackupFormat"] = "世界备份格式";
            lang["misc.worldBackupType"] = "世界备份类型";
            lang["misc.worldBackupMode"] = "世界备份模式";
            lang["misc.worldBackupState"] = "世界备份状态";
            lang["misc.worldBackupStatus"] = "世界备份状态";
            lang["misc.worldBackupResult"] = "世界备份结果";
            lang["misc.worldBackupCode"] = "世界备份代码";
            lang["misc.worldBackupMessage"] = "世界备份消息";
            lang["misc.worldBackupDetails"] = "世界备份详情";
            lang["misc.worldBackupSummary"] = "世界备份摘要";
            lang["misc.worldBackupReport"] = "世界备份报告";
            lang["misc.worldBackupLog"] = "世界备份日志";
            lang["misc.worldBackupHistory"] = "世界备份历史";
            lang["misc.worldBackupSchedule"] = "世界备份计划";
            lang["misc.worldBackupPolicy"] = "世界备份策略";
            lang["misc.worldBackupRetention"] = "世界备份保留";
            lang["misc.worldBackupRotation"] = "世界备份轮换";
            lang["misc.worldBackupVerification"] = "世界备份验证";
            lang["misc.worldBackupIntegrity"] = "世界备份完整性";
            lang["misc.worldBackupConsistency"] = "世界备份一致性";
            lang["misc.worldBackupReliability"] = "世界备份可靠性";
            lang["misc.worldBackupAvailability"] = "世界备份可用性";
            lang["misc.worldBackupDurability"] = "世界备份持久性";
            lang["misc.worldBackupSecurity"] = "世界备份安全性";
            lang["misc.worldBackupPrivacy"] = "世界备份隐私";
            lang["misc.worldBackupConfidentiality"] = "世界备份机密性";
            lang["misc.worldBackupIntegrity"] = "世界备份完整性";
            lang["misc.worldBackupAuthenticity"] = "世界备份真实性";
            lang["misc.worldBackupNonRepudiation"] = "世界备份不可否认性";
            lang["misc.worldBackupAuthorization"] = "世界备份授权";
            lang["misc.worldBackupAuthentication"] = "世界备份认证";
            lang["misc.worldBackupAccessControl"] = "世界备份访问控制";
            lang["misc.worldBackupAudit"] = "世界备份审计";
            lang["misc.worldBackupCompliance"] = "世界备份合规性";
            lang["misc.worldBackupGovernance"] = "世界备份治理";
            lang["misc.worldBackupManagement"] = "世界备份管理";
            lang["misc.worldBackupAdministration"] = "世界备份管理";
            lang["misc.worldBackupOperation"] = "世界备份操作";
            lang["misc.worldBackupMaintenance"] = "世界备份维护";
            lang["misc.worldBackupSupport"] = "世界备份支持";
            lang["misc.worldBackupService"] = "世界备份服务";
            lang["misc.worldBackupSystem"] = "世界备份系统";
            lang["misc.worldBackupInfrastructure"] = "世界备份基础设施";
            lang["misc.worldBackupPlatform"] = "世界备份平台";
            lang["misc.worldBackupFramework"] = "世界备份框架";
            lang["misc.worldBackupArchitecture"] = "世界备份架构";
            lang["misc.worldBackupDesign"] = "世界备份设计";
            lang["misc.worldBackupImplementation"] = "世界备份实现";
            lang["misc.worldBackupDeployment"] = "世界备份部署";
            lang["misc.worldBackupConfiguration"] = "世界备份配置";
            lang["misc.worldBackupCustomization"] = "世界备份定制";
            lang["misc.worldBackupOptimization"] = "世界备份优化";
            lang["misc.worldBackupPerformance"] = "世界备份性能";
            lang["misc.worldBackupScalability"] = "世界备份可扩展性";
            lang["misc.worldBackupFlexibility"] = "世界备份灵活性";
            lang["misc.worldBackupPortability"] = "世界备份可移植性";
            lang["misc.worldBackupInteroperability"] = "世界备份互操作性";
            lang["misc.worldBackupCompatibility"] = "世界备份兼容性";
            lang["misc.worldBackupExtensibility"] = "世界备份可扩展性";
            lang["misc.worldBackupModularity"] = "世界备份模块化";
            lang["misc.worldBackupReusability"] = "世界备份可重用性";
            lang["misc.worldBackupMaintainability"] = "世界备份可维护性";
            lang["misc.worldBackupTestability"] = "世界备份可测试性";
            lang["misc.worldBackupDebuggability"] = "世界备份可调试性";
            lang["misc.worldBackupObservability"] = "世界备份可观测性";
            lang["misc.worldBackupMonitorability"] = "世界备份可监控性";
            lang["misc.worldBackupManageability"] = "世界备份可管理性";
            lang["misc.worldBackupUsability"] = "世界备份可用性";
            lang["misc.worldBackupAccessibility"] = "世界备份可访问性";
            lang["misc.worldBackupLearnability"] = "世界备份易学性";
            lang["misc.worldBackupUnderstandability"] = "世界备份易懂性";
            lang["misc.worldBackupReadability"] = "世界备份可读性";
            lang["misc.worldBackupWritability"] = "世界备份可写性";
            lang["misc.worldBackupEditability"] = "世界备份可编辑性";
            lang["misc.worldBackupModifiability"] = "世界备份可修改性";
            lang["misc.worldBackupChangeability"] = "世界备份可变更性";
            lang["misc.worldBackupAdaptability"] = "世界备份适应性";
            lang["misc.worldBackupEvolvability"] = "世界备份可演进性";
            lang["misc.worldBackupMaturity"] = "世界备份成熟度";
            lang["misc.worldBackupStability"] = "世界备份稳定性";
            lang["misc.worldBackupRobustness"] = "世界备份健壮性";
            lang["misc.worldBackupResilience"] = "世界备份弹性";
            lang["misc.worldBackupFaultTolerance"] = "世界备份容错性";
            lang["misc.worldBackupErrorHandling"] = "世界备份错误处理";
            lang["misc.worldBackupExceptionHandling"] = "世界备份异常处理";
            lang["misc.worldBackupRecovery"] = "世界备份恢复";
            lang["misc.worldBackupFailover"] = "世界备份故障转移";
            lang["misc.worldBackupRedundancy"] = "世界备份冗余";
            lang["misc.worldBackupReplication"] = "世界备份复制";
            lang["misc.worldBackupSynchronization"] = "世界备份同步";
            lang["misc.worldBackupConsistency"] = "世界备份一致性";
            lang["misc.worldBackupCoherence"] = "世界备份连贯性";
            lang["misc.worldBackupCoordination"] = "世界备份协调";
            lang["misc.worldBackupCollaboration"] = "世界备份协作";
            lang["misc.worldBackupCommunication"] = "世界备份通信";
            lang["misc.worldBackupIntegration"] = "世界备份集成";
            lang["misc.worldBackupInterconnection"] = "世界备份互连";
            lang["misc.worldBackupInterfacing"] = "世界备份接口";
            lang["misc.worldBackupInteroperation"] = "世界备份互操作";
            lang["misc.worldBackupInteraction"] = "世界备份交互";
            lang["misc.worldBackupInterchange"] = "世界备份交换";
            lang["misc.worldBackupInterconversion"] = "世界备份转换";
            lang["misc.worldBackupInterrelation"] = "世界备份关联";
            lang["misc.worldBackupInterdependence"] = "世界备份依赖";
            lang["misc.worldBackupInterference"] = "世界备份干扰";
            lang["misc.worldBackupInterception"] = "世界备份拦截";
            lang["misc.worldBackupInterruption"] = "世界备份中断";
            lang["misc.worldBackupIntervention"] = "世界备份干预";
            lang["misc.worldBackupInterpretation"] = "世界备份解释";
            lang["misc.worldBackupInterrogation"] = "世界备份询问";
            lang["misc.worldBackupInterruption"] = "世界备份中断";
            lang["misc.worldBackupIntersection"] = "世界备份交集";
            lang["misc.worldBackupInterpolation"] = "世界备份插值";
            lang["misc.worldBackupInterposition"] = "世界备份插入";
            lang["misc.worldBackupInterpretation"] = "世界备份解释";
            lang["misc.worldBackupInterrogation"] = "世界备份询问";
            lang["misc.worldBackupInterruption"] = "世界备份中断";
            lang["misc.worldBackupIntersection"] = "世界备份交集";
            lang["misc.worldBackupInterpolation"] = "世界备份插值";
            lang["misc.worldBackupInterposition"] = "世界备份插入";

            return lang;
        }

        private Dictionary<string, string> GenerateEnglishTranslations()
        {
            Dictionary<string, string> lang = new Dictionary<string, string>();

            lang["game.title"] = "VoxelCraft";
            lang["game.menu.singleplayer"] = "Singleplayer";
            lang["game.menu.multiplayer"] = "Multiplayer";
            lang["game.menu.options"] = "Options";
            lang["game.menu.quit"] = "Quit Game";
            lang["game.menu.language"] = "Language";

            lang["menu.play"] = "Play";
            lang["menu.createWorld"] = "Create New World";
            lang["menu.selectWorld"] = "Select World";
            lang["menu.delete"] = "Delete";
            lang["menu.rename"] = "Rename";
            lang["menu.copy"] = "Copy";
            lang["menu.cancel"] = "Cancel";
            lang["menu.ok"] = "OK";
            lang["menu.yes"] = "Yes";
            lang["menu.no"] = "No";
            lang["menu.back"] = "Back";
            lang["menu.done"] = "Done";
            lang["menu.save"] = "Save";
            lang["menu.load"] = "Load";

            lang["options.title"] = "Options";
            lang["options.video"] = "Video Settings";
            lang["options.audio"] = "Audio Settings";
            lang["options.controls"] = "Controls";
            lang["options.language"] = "Language";
            lang["options.renderDistance"] = "Render Distance";
            lang["options.fov"] = "FOV";
            lang["options.maxFps"] = "Max FPS";
            lang["options.vsync"] = "VSync";
            lang["options.fullscreen"] = "Fullscreen";
            lang["options.graphics"] = "Graphics";
            lang["options.masterVolume"] = "Master Volume";
            lang["options.musicVolume"] = "Music Volume";
            lang["options.soundVolume"] = "Sound Volume";
            lang["options.mouseSensitivity"] = "Mouse Sensitivity";

            lang["game.paused"] = "Game Paused";
            lang["game.resume"] = "Resume Game";
            lang["game.options"] = "Options";
            lang["game.advancements"] = "Advancements";
            lang["game.statistics"] = "Statistics";
            lang["game.saveAndQuit"] = "Save and Quit to Title";
            lang["game.death"] = "You died!";
            lang["game.respawn"] = "Respawn";
            lang["game.titleScreen"] = "Title Screen";
            lang["game.spectator"] = "Spectator Mode";

            lang["inventory.title"] = "Inventory";
            lang["inventory.creative"] = "Creative Inventory";
            lang["inventory.search"] = "Search items...";
            lang["inventory.all"] = "All";
            lang["inventory.buildingBlocks"] = "Building Blocks";
            lang["inventory.decorations"] = "Decorations";
            lang["inventory.redstone"] = "Redstone";
            lang["inventory.transportation"] = "Transportation";
            lang["inventory.misc"] = "Misc";
            lang["inventory.food"] = "Food";
            lang["inventory.tools"] = "Tools";
            lang["inventory.combat"] = "Combat";
            lang["inventory.brewing"] = "Brewing";

            lang["block.stone"] = "Stone";
            lang["block.grass"] = "Grass Block";
            lang["block.dirt"] = "Dirt";
            lang["block.cobblestone"] = "Cobblestone";
            lang["block.planks"] = "Planks";
            lang["block.log"] = "Log";
            lang["block.leaves"] = "Leaves";
            lang["block.sand"] = "Sand";
            lang["block.gravel"] = "Gravel";
            lang["block.glass"] = "Glass";
            lang["block.bricks"] = "Bricks";
            lang["block.stoneBricks"] = "Stone Bricks";
            lang["block.obsidian"] = "Obsidian";
            lang["block.water"] = "Water";
            lang["block.lava"] = "Lava";
            lang["block.bedrock"] = "Bedrock";
            lang["block.coalOre"] = "Coal Ore";
            lang["block.ironOre"] = "Iron Ore";
            lang["block.goldOre"] = "Gold Ore";
            lang["block.diamondOre"] = "Diamond Ore";
            lang["block.redstoneOre"] = "Redstone Ore";
            lang["block.lapisOre"] = "Lapis Lazuli Ore";
            lang["block.emeraldOre"] = "Emerald Ore";
            lang["block.copperOre"] = "Copper Ore";
            lang["block.tnt"] = "TNT";
            lang["block.craftingTable"] = "Crafting Table";
            lang["block.furnace"] = "Furnace";
            lang["block.chest"] = "Chest";
            lang["block.door"] = "Wooden Door";
            lang["block.ironDoor"] = "Iron Door";
            lang["block.ladder"] = "Ladder";
            lang["block.torch"] = "Torch";
            lang["block.glowstone"] = "Glowstone";

            lang["item.apple"] = "Apple";
            lang["item.bread"] = "Bread";
            lang["item.cookedPorkchop"] = "Cooked Porkchop";
            lang["item.rawPorkchop"] = "Raw Porkchop";
            lang["item.cookedBeef"] = "Steak";
            lang["item.rawBeef"] = "Raw Beef";
            lang["item.coal"] = "Coal";
            lang["item.charcoal"] = "Charcoal";
            lang["item.ironIngot"] = "Iron Ingot";
            lang["item.goldIngot"] = "Gold Ingot";
            lang["item.diamond"] = "Diamond";
            lang["item.emerald"] = "Emerald";
            lang["item.lapisLazuli"] = "Lapis Lazuli";
            lang["item.redstone"] = "Redstone";
            lang["item.quartz"] = "Quartz";
            lang["item.copperIngot"] = "Copper Ingot";
            lang["item.netheriteIngot"] = "Netherite Ingot";
            lang["item.stick"] = "Stick";
            lang["item.string"] = "String";
            lang["item.leather"] = "Leather";
            lang["item.feather"] = "Feather";
            lang["item.bone"] = "Bone";
            lang["item.boneMeal"] = "Bone Meal";
            lang["item.arrow"] = "Arrow";
            lang["item.bow"] = "Bow";
            lang["item.crossbow"] = "Crossbow";
            lang["item.fishingRod"] = "Fishing Rod";
            lang["item.shears"] = "Shears";
            lang["item.flintAndSteel"] = "Flint and Steel";
            lang["item.flint"] = "Flint";

            lang["entity.zombie"] = "Zombie";
            lang["entity.skeleton"] = "Skeleton";
            lang["entity.creeper"] = "Creeper";
            lang["entity.spider"] = "Spider";
            lang["entity.enderman"] = "Enderman";
            lang["entity.cow"] = "Cow";
            lang["entity.pig"] = "Pig";
            lang["entity.sheep"] = "Sheep";
            lang["entity.chicken"] = "Chicken";
            lang["entity.rabbit"] = "Rabbit";
            lang["entity.wolf"] = "Wolf";
            lang["entity.villager"] = "Villager";
            lang["entity.player"] = "Player";

            lang["biome.ocean"] = "Ocean";
            lang["biome.plains"] = "Plains";
            lang["biome.desert"] = "Desert";
            lang["biome.mountains"] = "Mountains";
            lang["biome.forest"] = "Forest";
            lang["biome.taiga"] = "Taiga";
            lang["biome.swamp"] = "Swamp";
            lang["biome.river"] = "River";
            lang["biome.netherWastes"] = "Nether Wastes";
            lang["biome.theEnd"] = "The End";

            lang["enchantment.protection"] = "Protection";
            lang["enchantment.fireProtection"] = "Fire Protection";
            lang["enchantment.featherFalling"] = "Feather Falling";
            lang["enchantment.blastProtection"] = "Blast Protection";
            lang["enchantment.projectileProtection"] = "Projectile Protection";
            lang["enchantment.thorns"] = "Thorns";
            lang["enchantment.respiration"] = "Respiration";
            lang["enchantment.aquaAffinity"] = "Aqua Affinity";
            lang["enchantment.depthStrider"] = "Depth Strider";
            lang["enchantment.frostWalker"] = "Frost Walker";
            lang["enchantment.sharpness"] = "Sharpness";
            lang["enchantment.smite"] = "Smite";
            lang["enchantment.baneOfArthropods"] = "Bane of Arthropods";
            lang["enchantment.knockback"] = "Knockback";
            lang["enchantment.fireAspect"] = "Fire Aspect";
            lang["enchantment.looting"] = "Looting";
            lang["enchantment.efficiency"] = "Efficiency";
            lang["enchantment.silkTouch"] = "Silk Touch";
            lang["enchantment.unbreaking"] = "Unbreaking";
            lang["enchantment.fortune"] = "Fortune";
            lang["enchantment.power"] = "Power";
            lang["enchantment.punch"] = "Punch";
            lang["enchantment.flame"] = "Flame";
            lang["enchantment.infinity"] = "Infinity";
            lang["enchantment.mending"] = "Mending";

            lang["death.fell.accident.generic"] = "%1$s fell from a high place";
            lang["death.drown"] = "%1$s drowned";
            lang["death.lava"] = "%1$s tried to swim in lava";
            lang["death.onFire"] = "%1$s burned to death";
            lang["death.starve"] = "%1$s starved to death";
            lang["death.void"] = "%1$s fell out of the world";

            lang["misc.loading"] = "Loading...";
            lang["misc.generatingWorld"] = "Generating world...";
            lang["misc.savingWorld"] = "Saving world...";
            lang["misc.worldSaved"] = "World saved";
            lang["misc.worldLoaded"] = "World loaded";

            return lang;
        }

        private Dictionary<string, string> GenerateTraditionalChineseTranslations()
        {
            Dictionary<string, string> lang = new Dictionary<string, string>();

            lang["game.title"] = "VoxelCraft";
            lang["game.menu.singleplayer"] = "單人遊戲";
            lang["game.menu.multiplayer"] = "多人遊戲";
            lang["game.menu.options"] = "選項";
            lang["game.menu.quit"] = "離開遊戲";

            lang["menu.play"] = "開始遊戲";
            lang["menu.createWorld"] = "建立新世界";
            lang["menu.selectWorld"] = "選擇世界";
            lang["menu.delete"] = "刪除";
            lang["menu.rename"] = "重新命名";
            lang["menu.copy"] = "複製";
            lang["menu.cancel"] = "取消";
            lang["menu.ok"] = "確定";
            lang["menu.back"] = "返回";
            lang["menu.done"] = "完成";
            lang["menu.save"] = "儲存";

            lang["options.title"] = "選項";
            lang["options.video"] = "視訊設定";
            lang["options.audio"] = "音訊設定";
            lang["options.controls"] = "控制設定";
            lang["options.language"] = "語言設定";
            lang["options.renderDistance"] = "渲染距離";
            lang["options.fov"] = "視野";
            lang["options.maxFps"] = "最大幀率";
            lang["options.vsync"] = "垂直同步";
            lang["options.fullscreen"] = "全螢幕";

            lang["game.paused"] = "遊戲已暫停";
            lang["game.resume"] = "繼續遊戲";
            lang["game.options"] = "選項";
            lang["game.saveAndQuit"] = "儲存並離開";
            lang["game.death"] = "你死了！";
            lang["game.respawn"] = "重生";
            lang["game.titleScreen"] = "返回標題畫面";

            lang["inventory.title"] = "物品欄";
            lang["inventory.creative"] = "創造模式物品欄";
            lang["inventory.search"] = "搜尋物品...";

            lang["block.stone"] = "石頭";
            lang["block.grass"] = "草方塊";
            lang["block.dirt"] = "泥土";
            lang["block.cobblestone"] = "鵝卵石";
            lang["block.planks"] = "木板";
            lang["block.log"] = "原木";
            lang["block.leaves"] = "樹葉";
            lang["block.sand"] = "沙子";
            lang["block.gravel"] = "礫石";
            lang["block.glass"] = "玻璃";
            lang["block.obsidian"] = "黑曜石";
            lang["block.water"] = "水";
            lang["block.lava"] = "岩漿";
            lang["block.bedrock"] = "基岩";

            lang["item.apple"] = "蘋果";
            lang["item.bread"] = "麵包";
            lang["item.coal"] = "煤炭";
            lang["item.ironIngot"] = "鐵錠";
            lang["item.goldIngot"] = "金錠";
            lang["item.diamond"] = "鑽石";
            lang["item.emerald"] = "綠寶石";
            lang["item.redstone"] = "紅石";

            lang["entity.zombie"] = "殭屍";
            lang["entity.skeleton"] = "骷髏";
            lang["entity.creeper"] = "苦力怕";
            lang["entity.spider"] = "蜘蛛";
            lang["entity.enderman"] = "終界使者";
            lang["entity.cow"] = "牛";
            lang["entity.pig"] = "豬";
            lang["entity.sheep"] = "羊";
            lang["entity.chicken"] = "雞";
            lang["entity.villager"] = "村民";

            lang["biome.ocean"] = "海洋";
            lang["biome.plains"] = "平原";
            lang["biome.desert"] = "沙漠";
            lang["biome.mountains"] = "山地";
            lang["biome.forest"] = "森林";

            lang["misc.loading"] = "載入中...";
            lang["misc.generatingWorld"] = "正在生成世界...";
            lang["misc.savingWorld"] = "正在儲存世界...";

            return lang;
        }

        private Dictionary<string, string> GenerateJapaneseTranslations()
        {
            Dictionary<string, string> lang = new Dictionary<string, string>();

            lang["game.title"] = "VoxelCraft";
            lang["game.menu.singleplayer"] = "シングルプレイ";
            lang["game.menu.multiplayer"] = "マルチプレイ";
            lang["game.menu.options"] = "オプション";
            lang["game.menu.quit"] = "ゲームを終了";

            lang["menu.play"] = "プレイ";
            lang["menu.createWorld"] = "新しい世界を作成";
            lang["menu.selectWorld"] = "世界を選択";
            lang["menu.delete"] = "削除";
            lang["menu.rename"] = "名前変更";
            lang["menu.copy"] = "コピー";
            lang["menu.cancel"] = "キャンセル";
            lang["menu.ok"] = "OK";
            lang["menu.back"] = "戻る";
            lang["menu.done"] = "完了";
            lang["menu.save"] = "保存";

            lang["options.title"] = "オプション";
            lang["options.video"] = "ビデオ設定";
            lang["options.audio"] = "オーディオ設定";
            lang["options.controls"] = "コントロール設定";
            lang["options.language"] = "言語設定";
            lang["options.renderDistance"] = "描画距離";
            lang["options.fov"] = "視野";
            lang["options.maxFps"] = "最大FPS";
            lang["options.vsync"] = "垂直同期";
            lang["options.fullscreen"] = "フルスクリーン";

            lang["game.paused"] = "ゲーム停止中";
            lang["game.resume"] = "ゲームを再開";
            lang["game.options"] = "オプション";
            lang["game.saveAndQuit"] = "保存してタイトルへ";
            lang["game.death"] = "あなたは死んでしまった！";
            lang["game.respawn"] = "リスポーン";
            lang["game.titleScreen"] = "タイトル画面";

            lang["inventory.title"] = "インベントリ";
            lang["inventory.creative"] = "クリエイティブインベントリ";
            lang["inventory.search"] = "アイテムを検索...";

            lang["block.stone"] = "石";
            lang["block.grass"] = "草ブロック";
            lang["block.dirt"] = "土";
            lang["block.cobblestone"] = "丸石";
            lang["block.planks"] = "板材";
            lang["block.log"] = "原木";
            lang["block.leaves"] = "葉";
            lang["block.sand"] = "砂";
            lang["block.gravel"] = "砂利";
            lang["block.glass"] = "ガラス";
            lang["block.obsidian"] = "黒曜石";
            lang["block.water"] = "水";
            lang["block.lava"] = "溶岩";
            lang["block.bedrock"] = "岩盤";

            lang["item.apple"] = "りんご";
            lang["item.bread"] = "パン";
            lang["item.coal"] = "石炭";
            lang["item.ironIngot"] = "鉄インゴット";
            lang["item.goldIngot"] = "金インゴット";
            lang["item.diamond"] = "ダイヤモンド";
            lang["item.emerald"] = "エメラルド";
            lang["item.redstone"] = "レッドストーン";

            lang["entity.zombie"] = "ゾンビ";
            lang["entity.skeleton"] = "スケルトン";
            lang["entity.creeper"] = "クリーパー";
            lang["entity.spider"] = "クモ";
            lang["entity.enderman"] = "エンダーマン";
            lang["entity.cow"] = "牛";
            lang["entity.pig"] = "豚";
            lang["entity.sheep"] = "羊";
            lang["entity.chicken"] = "鶏";
            lang["entity.villager"] = "村人";

            lang["biome.ocean"] = "海";
            lang["biome.plains"] = "平原";
            lang["biome.desert"] = "砂漠";
            lang["biome.mountains"] = "山岳";
            lang["biome.forest"] = "森林";

            lang["misc.loading"] = "読み込み中...";
            lang["misc.generatingWorld"] = "世界を生成中...";
            lang["misc.savingWorld"] = "世界を保存中...";

            return lang;
        }

        public void LoadLanguage(string languageCode)
        {
            currentLanguage = languageCode;
            translations.Clear();

            if (allLanguages.TryGetValue(languageCode, out Dictionary<string, string> lang))
            {
                foreach (KeyValuePair<string, string> kvp in lang)
                {
                    translations[kvp.Key] = kvp.Value;
                }
            }
            else
            {
                // 回退到英文
                if (allLanguages.TryGetValue("en_us", out Dictionary<string, string> en))
                {
                    foreach (KeyValuePair<string, string> kvp in en)
                    {
                        translations[kvp.Key] = kvp.Value;
                    }
                }
            }

            Console.WriteLine($"[Localization] 已加载语言: {languageCode}，共 {translations.Count} 条翻译");
        }

        public string Translate(string key)
        {
            if (translations.TryGetValue(key, out string value))
            {
                return value;
            }
            return key;
        }

        public string Translate(string key, params object[] args)
        {
            string format = Translate(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }

        public bool HasTranslation(string key)
        {
            return translations.ContainsKey(key);
        }

        public List<string> GetAvailableLanguages()
        {
            return new List<string>(allLanguages.Keys);
        }

        public Dictionary<string, string> GetLanguageNames()
        {
            return new Dictionary<string, string>
            {
                { "zh_cn", "简体中文" },
                { "zh_tw", "繁體中文" },
                { "en_us", "English (US)" },
                { "ja_jp", "日本語" }
            };
        }

        public void LoadLanguageFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    Dictionary<string, string> lang = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (lang != null)
                    {
                        string langCode = Path.GetFileNameWithoutExtension(path);
                        allLanguages[langCode] = lang;
                        Console.WriteLine($"[Localization] 已加载语言文件: {path}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Localization] 加载语言文件失败: {ex.Message}");
            }
        }

        public void Reload()
        {
            LoadLanguage(currentLanguage);
        }
    }
}
