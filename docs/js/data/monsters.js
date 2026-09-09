// ==================== 永恒地牢 - 怪物数据配置 ====================
// 定义所有怪物、BOSS的属性、AI、掉落

const MonsterData = {
  // ==================== 普通怪物 ====================

  // 第1-3层
  slime: {
    name: '史莱姆',
    icon: '🟢',
    type: 'melee',
    baseStats: { hp: 14, damage: 5, armor: 0, magicResist: 0 },
    speed: 1.0,
    attackRange: 30,
    attackSpeed: 1.0,
    exp: 10,
    gold: [3, 8],
    dropChance: 0.1,
    drops: ['healthPotion', 'slimeGel'],
    size: 24,
    ai: 'chase',
    description: '最基础的地牢生物，软绵绵的但数量众多。'
  },
  bat: {
    name: '蝙蝠',
    icon: '🦇',
    type: 'melee',
    baseStats: { hp: 10, damage: 4, armor: 0, magicResist: 0 },
    speed: 2.0,
    attackRange: 30,
    attackSpeed: 1.5,
    exp: 8,
    gold: [2, 6],
    dropChance: 0.08,
    drops: ['batWing', 'healthPotion'],
    size: 20,
    ai: 'erratic',
    description: '飞行生物，移动速度快但很脆弱。'
  },
  rat: {
    name: '巨鼠',
    icon: '🐀',
    type: 'melee',
    baseStats: { hp: 12, damage: 6, armor: 0, magicResist: 0 },
    speed: 1.8,
    attackRange: 25,
    attackSpeed: 1.3,
    exp: 9,
    gold: [2, 5],
    dropChance: 0.1,
    drops: ['ratTail', 'healthPotion'],
    size: 22,
    ai: 'chase',
    description: '地牢里常见的害虫，携带疾病。'
  },

  // 第3-5层
  skeleton: {
    name: '骷髅兵',
    icon: '💀',
    type: 'melee',
    baseStats: { hp: 24, damage: 8, armor: 3, magicResist: 0 },
    speed: 1.2,
    attackRange: 35,
    attackSpeed: 1.0,
    exp: 15,
    gold: [8, 15],
    dropChance: 0.15,
    drops: ['bone', 'ironSword', 'leatherArmor'],
    size: 26,
    ai: 'chase',
    description: '被黑暗魔法复活的亡灵战士。'
  },
  zombie: {
    name: '僵尸',
    icon: '🧟',
    type: 'melee',
    baseStats: { hp: 35, damage: 10, armor: 2, magicResist: 5 },
    speed: 0.7,
    attackRange: 35,
    attackSpeed: 0.8,
    exp: 20,
    gold: [10, 18],
    dropChance: 0.15,
    drops: ['rottenFlesh', 'chainmail'],
    size: 28,
    ai: 'chase',
    description: '行动迟缓但生命力顽强的亡灵。'
  },
  goblin: {
    name: '哥布林',
    icon: '👺',
    type: 'melee',
    baseStats: { hp: 21, damage: 7, armor: 1, magicResist: 0 },
    speed: 1.5,
    attackRange: 35,
    attackSpeed: 1.2,
    exp: 12,
    gold: [15, 25],
    dropChance: 0.2,
    drops: ['goblinEar', 'gold', 'dagger'],
    size: 24,
    ai: 'chase',
    description: '贪婪的小生物，喜欢收集闪亮的东西。'
  },
  spider: {
    name: '巨蛛',
    icon: '🕷️',
    type: 'melee',
    baseStats: { hp: 28, damage: 9, armor: 2, magicResist: 0 },
    speed: 1.8,
    attackRange: 35,
    attackSpeed: 1.4,
    exp: 18,
    gold: [12, 20],
    dropChance: 0.18,
    drops: ['spiderSilk', 'poisonVial'],
    size: 26,
    ai: 'chase',
    poison: { chance: 0.3, damage: 3, duration: 4 },
    description: '剧毒的大蜘蛛，攻击可能使你中毒。'
  },

  // 第5-8层
  ghost: {
    name: '幽灵',
    icon: '👻',
    type: 'ranged',
    baseStats: { hp: 17, damage: 12, armor: 0, magicResist: 20 },
    speed: 1.3,
    attackRange: 200,
    attackSpeed: 1.5,
    exp: 25,
    gold: [18, 28],
    dropChance: 0.2,
    drops: ['ectoplasm', 'manaPotion'],
    size: 26,
    ai: 'ranged',
    description: '徘徊在地牢中的亡魂，发射灵能弹。'
  },
  orc: {
    name: '兽人',
    icon: '👹',
    type: 'melee',
    baseStats: { hp: 56, damage: 15, armor: 5, magicResist: 0 },
    speed: 1.0,
    attackRange: 40,
    attackSpeed: 0.9,
    exp: 35,
    gold: [25, 40],
    dropChance: 0.25,
    drops: ['orcTusk', 'steelSword', 'chainmail'],
    size: 32,
    ai: 'chase',
    enrage: { hpPercent: 0.3, damageBoost: 1.5, speedBoost: 1.3 },
    description: '强壮的绿皮战士，低血量时会狂暴。'
  },
  darkMage: {
    name: '暗黑法师',
    icon: '🧙',
    type: 'ranged',
    baseStats: { hp: 31, damage: 18, armor: 2, magicResist: 15 },
    speed: 0.9,
    attackRange: 250,
    attackSpeed: 1.2,
    exp: 40,
    gold: [30, 50],
    dropChance: 0.3,
    drops: ['darkRobe', 'manaPotion', 'magicScroll'],
    size: 28,
    ai: 'ranged',
    description: '堕落的法师，使用黑暗魔法攻击。'
  },
  werewolf: {
    name: '狼人',
    icon: '🐺',
    type: 'melee',
    baseStats: { hp: 42, damage: 14, armor: 4, magicResist: 5 },
    speed: 2.2,
    attackRange: 40,
    attackSpeed: 1.6,
    exp: 38,
    gold: [28, 45],
    dropChance: 0.25,
    drops: ['wolfFang', 'wolfPelt'],
    size: 30,
    ai: 'chase',
    description: '速度极快的野兽，攻击迅猛。'
  },

  // 第8-12层
  golem: {
    name: '石魔像',
    icon: '🗿',
    type: 'melee',
    baseStats: { hp: 105, damage: 20, armor: 15, magicResist: 10 },
    speed: 0.5,
    attackRange: 45,
    attackSpeed: 0.6,
    exp: 60,
    gold: [50, 80],
    dropChance: 0.35,
    drops: ['stoneFragment', 'plateArmor'],
    size: 38,
    ai: 'chase',
    description: '由魔法驱动的石头巨人，防御极高。'
  },
  vampire: {
    name: '吸血鬼',
    icon: '🧛',
    type: 'melee',
    baseStats: { hp: 49, damage: 16, armor: 5, magicResist: 15 },
    speed: 1.8,
    attackRange: 40,
    attackSpeed: 1.3,
    exp: 55,
    gold: [45, 70],
    dropChance: 0.3,
    drops: ['vampireFang', 'bloodVial'],
    size: 28,
    ai: 'chase',
    lifesteal: 0.3,
    description: '不死的血族，攻击会吸取生命。'
  },
  wraith: {
    name: '怨灵',
    icon: '👤',
    type: 'ranged',
    baseStats: { hp: 38, damage: 22, armor: 0, magicResist: 30 },
    speed: 1.5,
    attackRange: 280,
    attackSpeed: 1.0,
    exp: 50,
    gold: [40, 65],
    dropChance: 0.28,
    drops: ['soulShard', 'manaPotion'],
    size: 28,
    ai: 'ranged',
    description: '充满怨恨的灵体，魔法抗性很高。'
  },
  minotaur: {
    name: '牛头人',
    icon: '🐂',
    type: 'melee',
    baseStats: { hp: 84, damage: 25, armor: 8, magicResist: 5 },
    speed: 1.4,
    attackRange: 50,
    attackSpeed: 0.8,
    exp: 70,
    gold: [55, 85],
    dropChance: 0.35,
    drops: ['minotaurHorn', 'greatAxe'],
    size: 36,
    ai: 'chase',
    charge: { cooldown: 8, damage: 2.0, speed: 3 },
    description: '迷宫中的守护者，会发动强力冲锋。'
  },

  // 第12层以上
  demon: {
    name: '小恶魔',
    icon: '👿',
    type: 'ranged',
    baseStats: { hp: 56, damage: 28, armor: 8, magicResist: 20 },
    speed: 1.6,
    attackRange: 260,
    attackSpeed: 1.1,
    exp: 80,
    gold: [60, 100],
    dropChance: 0.35,
    drops: ['demonHorn', 'fireGem'],
    size: 30,
    ai: 'ranged',
    description: '来自深渊的低级恶魔，使用火焰攻击。'
  },
  lich: {
    name: '巫妖',
    icon: '☠️',
    type: 'ranged',
    baseStats: { hp: 70, damage: 32, armor: 5, magicResist: 40 },
    speed: 1.0,
    attackRange: 300,
    attackSpeed: 0.9,
    exp: 100,
    gold: [80, 130],
    dropChance: 0.4,
    drops: ['lichPhylactery', 'darkRobe', 'magicScroll'],
    size: 32,
    ai: 'ranged',
    summon: { cooldown: 15, type: 'skeleton', count: 2 },
    description: '强大的不死法师，能召唤骷髅仆从。'
  },
  dragon: {
    name: '幼龙',
    icon: '🐲',
    type: 'ranged',
    baseStats: { hp: 125, damage: 30, armor: 12, magicResist: 25 },
    speed: 1.2,
    attackRange: 280,
    attackSpeed: 0.8,
    exp: 120,
    gold: [100, 160],
    dropChance: 0.5,
    drops: ['dragonScale', 'dragonScale', 'flameBlade'],
    size: 40,
    ai: 'ranged',
    description: '年幼的龙，但依然十分危险。'
  },
  deathKnight: {
    name: '死亡骑士',
    icon: '⚔️',
    type: 'melee',
    baseStats: { hp: 140, damage: 35, armor: 18, magicResist: 15 },
    speed: 1.3,
    attackRange: 55,
    attackSpeed: 1.0,
    exp: 150,
    gold: [120, 200],
    dropChance: 0.5,
    drops: ['deathKnightHelm', 'runedBlade', 'plateArmor'],
    size: 36,
    ai: 'chase',
    description: '堕落的圣骑士，现在为黑暗效力。'
  },

  // ==================== 精英怪物 ====================
  elite_skeleton_king: {
    name: '骷髅王',
    icon: '👑',
    type: 'elite',
    baseStats: { hp: 105, damage: 18, armor: 8, magicResist: 10 },
    speed: 1.1,
    attackRange: 45,
    attackSpeed: 1.0,
    exp: 80,
    gold: [60, 100],
    dropChance: 0.8,
    drops: ['skeletonCrown', 'steelSword', 'gold'],
    size: 34,
    ai: 'chase',
    summon: { cooldown: 10, type: 'skeleton', count: 2 },
    description: '骷髅大军的统领，能召唤手下。'
  },
  elite_orc_chief: {
    name: '兽人酋长',
    icon: '🪓',
    type: 'elite',
    baseStats: { hp: 175, damage: 25, armor: 12, magicResist: 5 },
    speed: 1.2,
    attackRange: 50,
    attackSpeed: 0.9,
    exp: 100,
    gold: [80, 140],
    dropChance: 0.8,
    drops: ['orcChiefAxe', 'plateArmor', 'gold'],
    size: 40,
    ai: 'chase',
    warCry: { cooldown: 15, buff: { damage: 1.5, speed: 1.3, duration: 8 } },
    description: '兽人部落的首领，战吼能强化自身。'
  },

  // ==================== 新增怪物（元素系） ====================
  fire_elemental: {
    name: '火元素',
    icon: '🔥',
    type: 'elemental',
    baseStats: { hp: 42, damage: 15, armor: 0, magicResist: 20 },
    speed: 1.3,
    attackRange: 120,
    attackSpeed: 1.2,
    exp: 25,
    gold: [15, 30],
    dropChance: 0.2,
    drops: ['fireEssence', 'healthPotion'],
    size: 28,
    ai: 'ranged',
    rangedAttack: { damage: 15, speed: 5, color: '#FF4500', size: 10 },
    tags: ['elemental', 'fire'],
    description: '由纯粹火焰构成的元素生物，攻击附带燃烧效果。'
  },
  ice_elemental: {
    name: '冰元素',
    icon: '❄️',
    type: 'elemental',
    baseStats: { hp: 38, damage: 12, armor: 5, magicResist: 25 },
    speed: 1.0,
    attackRange: 130,
    attackSpeed: 1.0,
    exp: 25,
    gold: [15, 30],
    dropChance: 0.2,
    drops: ['iceEssence', 'manaPotion'],
    size: 28,
    ai: 'ranged',
    rangedAttack: { damage: 12, speed: 4, color: '#00CED1', size: 10, slow: 0.5 },
    tags: ['elemental', 'ice'],
    description: '寒冰凝聚的元素生物，攻击会减速目标。'
  },
  earth_elemental: {
    name: '土元素',
    icon: '🪨',
    type: 'elemental',
    baseStats: { hp: 84, damage: 18, armor: 15, magicResist: 5 },
    speed: 0.6,
    attackRange: 40,
    attackSpeed: 0.8,
    exp: 30,
    gold: [20, 40],
    dropChance: 0.25,
    drops: ['earthEssence', 'ironOre'],
    size: 36,
    ai: 'chase',
    tags: ['elemental', 'earth'],
    description: '岩石构成的巨型元素，防御极高但移动缓慢。'
  },
  lightning_elemental: {
    name: '雷元素',
    icon: '⚡',
    type: 'elemental',
    baseStats: { hp: 31, damage: 20, armor: 0, magicResist: 30 },
    speed: 2.0,
    attackRange: 100,
    attackSpeed: 1.5,
    exp: 28,
    gold: [18, 35],
    dropChance: 0.2,
    drops: ['lightningEssence', 'manaPotion'],
    size: 26,
    ai: 'erratic',
    rangedAttack: { damage: 20, speed: 8, color: '#FFD700', size: 8 },
    tags: ['elemental', 'lightning'],
    description: '闪电化身的元素生物，速度极快，攻击迅猛。'
  },

  // ==================== 新增怪物（亡灵系） ====================
  ghost: {
    name: '幽灵',
    icon: '👻',
    type: 'undead',
    baseStats: { hp: 28, damage: 14, armor: 0, magicResist: 40 },
    speed: 1.5,
    attackRange: 35,
    attackSpeed: 1.2,
    exp: 22,
    gold: [10, 25],
    dropChance: 0.15,
    drops: ['ectoplasm', 'manaPotion'],
    size: 26,
    ai: 'chase',
    tags: ['undead', 'spirit'],
    description: '飘荡在地牢中的亡魂，对物理攻击有抗性。'
  },
  wraith: {
    name: '怨灵',
    icon: '💀',
    type: 'undead',
    baseStats: { hp: 49, damage: 22, armor: 0, magicResist: 50 },
    speed: 1.8,
    attackRange: 40,
    attackSpeed: 1.3,
    exp: 35,
    gold: [25, 50],
    dropChance: 0.2,
    drops: ['soulShard', 'darkEssence'],
    size: 30,
    ai: 'erratic',
    tags: ['undead', 'spirit'],
    description: '充满怨念的强大亡灵，能吸取生命。'
  },
  bone_knight: {
    name: '骷髅骑士',
    icon: '🛡️',
    type: 'undead',
    baseStats: { hp: 62, damage: 20, armor: 12, magicResist: 5 },
    speed: 1.0,
    attackRange: 45,
    attackSpeed: 1.0,
    exp: 32,
    gold: [25, 45],
    dropChance: 0.25,
    drops: ['boneShield', 'ironSword'],
    size: 32,
    ai: 'chase',
    tags: ['undead'],
    description: '骑着骷髅马的亡灵骑士，攻防兼备。'
  },
  necromancer: {
    name: '死灵法师',
    icon: '🧙',
    type: 'undead',
    baseStats: { hp: 35, damage: 25, armor: 2, magicResist: 30 },
    speed: 0.9,
    attackRange: 150,
    attackSpeed: 0.8,
    exp: 40,
    gold: [30, 60],
    dropChance: 0.3,
    drops: ['necroTome', 'darkEssence', 'manaPotion'],
    size: 28,
    ai: 'kite',
    rangedAttack: { damage: 25, speed: 4, color: '#00FF00', size: 12 },
    tags: ['undead', 'caster'],
    description: '操纵亡灵的邪恶法师，能召唤骷髅。'
  },
  vampire: {
    name: '吸血鬼',
    icon: '🧛',
    type: 'undead',
    baseStats: { hp: 56, damage: 18, armor: 5, magicResist: 20 },
    speed: 1.6,
    attackRange: 35,
    attackSpeed: 1.4,
    exp: 38,
    gold: [30, 55],
    dropChance: 0.25,
    drops: ['vampireFang', 'bloodVial'],
    size: 28,
    ai: 'chase',
    lifesteal: 0.3,
    tags: ['undead'],
    description: '永生的血族，攻击能吸取生命。'
  },
  mummy: {
    name: '木乃伊',
    icon: '🧟',
    type: 'undead',
    baseStats: { hp: 70, damage: 16, armor: 8, magicResist: 10 },
    speed: 0.7,
    attackRange: 35,
    attackSpeed: 0.9,
    exp: 28,
    gold: [20, 40],
    dropChance: 0.2,
    drops: ['ancientCloth', 'goldCoin'],
    size: 32,
    ai: 'chase',
    tags: ['undead'],
    description: '被诅咒的古代尸体，缠绕着神圣的绷带。'
  },

  // ==================== 新增怪物（恶魔系） ====================
  imp: {
    name: '小恶魔',
    icon: '👿',
    type: 'demon',
    baseStats: { hp: 24, damage: 12, armor: 2, magicResist: 15 },
    speed: 1.8,
    attackRange: 90,
    attackSpeed: 1.3,
    exp: 20,
    gold: [12, 25],
    dropChance: 0.15,
    drops: ['demonHorn', 'fireEssence'],
    size: 22,
    ai: 'erratic',
    rangedAttack: { damage: 12, speed: 5, color: '#FF0000', size: 8 },
    tags: ['demon'],
    description: '地狱的低级恶魔，狡猾且喜欢恶作剧。'
  },
  succubus: {
    name: '魅魔',
    icon: '😈',
    type: 'demon',
    baseStats: { hp: 42, damage: 16, armor: 3, magicResist: 25 },
    speed: 1.5,
    attackRange: 100,
    attackSpeed: 1.1,
    exp: 35,
    gold: [30, 60],
    dropChance: 0.25,
    drops: ['succubusWing', 'charmScroll'],
    size: 26,
    ai: 'kite',
    rangedAttack: { damage: 16, speed: 4, color: '#FF69B4', size: 10 },
    tags: ['demon'],
    description: '诱惑凡人的恶魔，能魅惑目标。'
  },
  hellhound: {
    name: '地狱犬',
    icon: '🐕',
    type: 'demon',
    baseStats: { hp: 49, damage: 20, armor: 4, magicResist: 10 },
    speed: 2.2,
    attackRange: 35,
    attackSpeed: 1.5,
    exp: 30,
    gold: [20, 45],
    dropChance: 0.2,
    drops: ['hellfang', 'fireEssence'],
    size: 30,
    ai: 'chase',
    tags: ['demon', 'beast'],
    description: '来自地狱的猎犬，速度极快，口中喷火。'
  },
  demon_brute: {
    name: '恶魔蛮兵',
    icon: '👹',
    type: 'demon',
    baseStats: { hp: 77, damage: 25, armor: 8, magicResist: 5 },
    speed: 0.9,
    attackRange: 45,
    attackSpeed: 0.9,
    exp: 40,
    gold: [35, 70],
    dropChance: 0.25,
    drops: ['demonHide', 'heavyMace'],
    size: 38,
    ai: 'chase',
    tags: ['demon'],
    description: '高大强壮的恶魔战士，力大无穷。'
  },

  // ==================== 新增怪物（野兽系） ====================
  giant_spider: {
    name: '巨型蜘蛛',
    icon: '🕷️',
    type: 'beast',
    baseStats: { hp: 31, damage: 14, armor: 3, magicResist: 5 },
    speed: 1.7,
    attackRange: 30,
    attackSpeed: 1.4,
    exp: 20,
    gold: [10, 22],
    dropChance: 0.2,
    drops: ['spiderSilk', 'venomSac'],
    size: 26,
    ai: 'erratic',
    tags: ['beast', 'poison'],
    description: '在地牢中结网的巨型蜘蛛，攻击带毒。'
  },
  scorpion: {
    name: '巨蝎',
    icon: '🦂',
    type: 'beast',
    baseStats: { hp: 45, damage: 18, armor: 10, magicResist: 0 },
    speed: 1.2,
    attackRange: 40,
    attackSpeed: 1.0,
    exp: 25,
    gold: [15, 30],
    dropChance: 0.2,
    drops: ['scorpionTail', 'venomSac'],
    size: 30,
    ai: 'chase',
    tags: ['beast', 'poison'],
    description: '剧毒的沙漠巨蝎，尾刺能致命。'
  },
  basilisk: {
    name: '蛇怪',
    icon: '🐍',
    type: 'beast',
    baseStats: { hp: 59, damage: 22, armor: 6, magicResist: 15 },
    speed: 1.1,
    attackRange: 50,
    attackSpeed: 1.1,
    exp: 38,
    gold: [30, 55],
    dropChance: 0.25,
    drops: ['basiliskScale', 'petrificationEye'],
    size: 34,
    ai: 'chase',
    tags: ['beast', 'poison'],
    description: '传说中的蛇怪，目光能石化生物。'
  },
  griffin: {
    name: '狮鹫',
    icon: '🦅',
    type: 'beast',
    baseStats: { hp: 52, damage: 20, armor: 7, magicResist: 10 },
    speed: 2.0,
    attackRange: 40,
    attackSpeed: 1.3,
    exp: 35,
    gold: [28, 50],
    dropChance: 0.25,
    drops: ['griffinFeather', 'talon'],
    size: 32,
    ai: 'erratic',
    tags: ['beast', 'flying'],
    description: '狮身鹰首的神兽，飞行速度极快。'
  },
  wyvern: {
    name: '双足飞龙',
    icon: '🐉',
    type: 'beast',
    baseStats: { hp: 66, damage: 24, armor: 8, magicResist: 15 },
    speed: 1.5,
    attackRange: 110,
    attackSpeed: 0.9,
    exp: 42,
    gold: [35, 65],
    dropChance: 0.3,
    drops: ['wyvernScale', 'dragonTooth'],
    size: 36,
    ai: 'kite',
    rangedAttack: { damage: 20, speed: 5, color: '#FF6347', size: 12 },
    tags: ['beast', 'flying', 'dragon'],
    description: '龙族的远亲，能喷吐酸液。'
  },

  // ==================== 新增怪物（构造体系） ====================
  golem: {
    name: '石像鬼',
    icon: '🗿',
    type: 'construct',
    baseStats: { hp: 91, damage: 22, armor: 20, magicResist: 10 },
    speed: 0.5,
    attackRange: 45,
    attackSpeed: 0.7,
    exp: 35,
    gold: [25, 50],
    dropChance: 0.25,
    drops: ['stoneFragment', 'golemHeart'],
    size: 40,
    ai: 'chase',
    tags: ['construct'],
    description: '被魔法赋予生命的石像，坚不可摧。'
  },
  automaton: {
    name: '机械傀儡',
    icon: '🤖',
    type: 'construct',
    baseStats: { hp: 62, damage: 18, armor: 12, magicResist: 20 },
    speed: 1.0,
    attackRange: 80,
    attackSpeed: 1.0,
    exp: 32,
    gold: [30, 55],
    dropChance: 0.25,
    drops: ['gear', 'arcaneCore'],
    size: 30,
    ai: 'ranged',
    rangedAttack: { damage: 18, speed: 6, color: '#87CEEB', size: 8 },
    tags: ['construct'],
    description: '古代文明留下的机械造物，仍在执行古老的命令。'
  },

  // ==================== 新增怪物（精英怪） ====================
  elite_skeleton: {
    name: '骷髅将军',
    icon: '⚔️',
    type: 'elite',
    baseStats: { hp: 105, damage: 28, armor: 10, magicResist: 10 },
    speed: 1.2,
    attackRange: 50,
    attackSpeed: 1.1,
    exp: 60,
    gold: [50, 100],
    dropChance: 0.5,
    drops: ['generalSword', 'eliteBone', 'healthPotion'],
    size: 34,
    ai: 'chase',
    isElite: true,
    tags: ['undead', 'elite'],
    description: '骷髅大军的统帅，带领着亡灵军团。'
  },
  elite_orc: {
    name: '兽人督军',
    icon: '🪓',
    type: 'elite',
    baseStats: { hp: 125, damage: 32, armor: 8, magicResist: 5 },
    speed: 1.0,
    attackRange: 55,
    attackSpeed: 0.9,
    exp: 65,
    gold: [55, 110],
    dropChance: 0.5,
    drops: ['warAxe', 'orcTooth', 'healthPotion'],
    size: 42,
    ai: 'chase',
    isElite: true,
    warCry: { cooldown: 12, buff: { damage: 1.6, speed: 1.4, duration: 6 } },
    tags: ['humanoid', 'elite'],
    description: '兽人部落的督军，战吼能激励周围的兽人。'
  },
  elite_mage: {
    name: '大法师',
    icon: '🔮',
    type: 'elite',
    baseStats: { hp: 70, damage: 35, armor: 3, magicResist: 40 },
    speed: 1.0,
    attackRange: 160,
    attackSpeed: 0.8,
    exp: 70,
    gold: [60, 120],
    dropChance: 0.5,
    drops: ['archmageRobe', 'manaGem', 'manaPotion'],
    size: 30,
    ai: 'kite',
    isElite: true,
    rangedAttack: { damage: 30, speed: 6, color: '#9370DB', size: 14 },
    tags: ['humanoid', 'elite', 'caster'],
    description: '精通奥术的大法师，能释放强大的魔法。'
  },

  // ==================== BOSS ====================
  boss_slime_king: {
    name: '史莱姆王',
    icon: '👑',
    type: 'boss',
    floor: 5,
    baseStats: { hp: 210, damage: 18, armor: 5, magicResist: 10 },
    speed: 0.8,
    attackRange: 50,
    attackSpeed: 0.8,
    exp: 200,
    gold: [150, 250],
    dropChance: 1.0,
    drops: ['slimeKingCrown', 'superHealthPotion', 'gold', 'gold'],
    size: 50,
    ai: 'boss',
    skills: [
      { name: '分裂', cooldown: 15, effect: 'spawn', type: 'slime', count: 3 },
      { name: '弹跳', cooldown: 8, effect: 'jump', damage: 2.0, radius: 80 }
    ],
    description: '巨大的史莱姆，地牢第一层的守护者。'
  },
  boss_skeleton_lord: {
    name: '骷髅领主',
    icon: '☠️',
    type: 'boss',
    floor: 10,
    baseStats: { hp: 350, damage: 28, armor: 12, magicResist: 15 },
    speed: 1.0,
    attackRange: 55,
    attackSpeed: 1.0,
    exp: 400,
    gold: [300, 500],
    dropChance: 1.0,
    drops: ['skeletonLordBlade', 'boneArmor', 'gold', 'magicScroll'],
    size: 55,
    ai: 'boss',
    skills: [
      { name: '召唤骷髅', cooldown: 12, effect: 'spawn', type: 'skeleton', count: 4 },
      { name: '死亡之握', cooldown: 10, effect: 'rangedAttack', damage: 2.5 },
      { name: '骨矛', cooldown: 6, effect: 'projectile', damage: 1.8, count: 3 }
    ],
    description: '统治地牢深处的亡灵君主。'
  },
  boss_dragon: {
    name: '深渊巨龙',
    icon: '🐉',
    type: 'boss',
    floor: 15,
    baseStats: { hp: 560, damage: 40, armor: 20, magicResist: 30 },
    speed: 1.2,
    attackRange: 300,
    attackSpeed: 0.7,
    exp: 800,
    gold: [500, 800],
    dropChance: 1.0,
    drops: ['dragonHeart', 'dragonScaleArmor', 'dragonSlayer', 'gold'],
    size: 65,
    ai: 'boss',
    skills: [
      { name: '龙息', cooldown: 8, effect: 'cone', damage: 2.5, range: 200, angle: 60 },
      { name: '尾扫', cooldown: 6, effect: 'aoe', damage: 1.5, radius: 100 },
      { name: '飞行', cooldown: 15, effect: 'fly', duration: 5 },
      { name: '火球雨', cooldown: 20, effect: 'meteor', damage: 2.0, count: 8 }
    ],
    description: '来自深渊的古老巨龙，喷吐毁灭之焰。'
  },
  boss_demon_lord: {
    name: '地狱恶魔',
    icon: '😈',
    type: 'boss',
    floor: 20,
    baseStats: { hp: 840, damage: 55, armor: 25, magicResist: 35 },
    speed: 1.5,
    attackRange: 60,
    attackSpeed: 1.1,
    exp: 1500,
    gold: [800, 1200],
    dropChance: 1.0,
    drops: ['demonHeart', 'demonArmor', 'hellfireBlade', 'gold'],
    size: 60,
    ai: 'boss',
    skills: [
      { name: '地狱火', cooldown: 5, effect: 'projectile', damage: 2.0, count: 5 },
      { name: '召唤恶魔', cooldown: 12, effect: 'spawn', type: 'demon', count: 2 },
      { name: '毁灭打击', cooldown: 10, effect: 'melee', damage: 3.0, stun: 2 },
      { name: '黑暗领域', cooldown: 25, effect: 'aura', damage: 0.1, radius: 200, duration: 8 }
    ],
    description: '地狱的统治者，拥有毁灭一切的力量。'
  },
  boss_lich_king: {
    name: '巫妖王',
    icon: '💀',
    type: 'boss',
    floor: 25,
    baseStats: { hp: 1050, damage: 50, armor: 15, magicResist: 50 },
    speed: 1.0,
    attackRange: 350,
    attackSpeed: 0.9,
    exp: 2500,
    gold: [1200, 2000],
    dropChance: 1.0,
    drops: ['frostmourne', 'lichKingArmor', ' phylactery', 'gold'],
    size: 60,
    ai: 'boss',
    skills: [
      { name: '死亡凋零', cooldown: 8, effect: 'aoe', damage: 1.5, radius: 150 },
      { name: '召唤亡灵', cooldown: 10, effect: 'spawn', type: 'skeleton', count: 5 },
      { name: '冰霜新星', cooldown: 12, effect: 'aoe', damage: 2.0, radius: 120, slow: 0.5 },
      { name: '亡灵大军', cooldown: 30, effect: 'spawn', type: 'zombie', count: 8 },
      { name: '生命汲取', cooldown: 15, effect: 'drain', damage: 2.0, lifesteal: 1.0 }
    ],
    description: '不死军团的最高统帅，掌控生死的力量。'
  },
  boss_final: {
    name: '深渊之主',
    icon: '🌑',
    type: 'boss',
    floor: 30,
    baseStats: { hp: 2100, damage: 80, armor: 35, magicResist: 50 },
    speed: 1.3,
    attackRange: 100,
    attackSpeed: 1.0,
    exp: 5000,
    gold: [3000, 5000],
    dropChance: 1.0,
    drops: ['abyssBlade', 'abyssArmor', 'abyssRing', 'gold', 'gold', 'gold'],
    size: 80,
    ai: 'boss',
    phases: 3,
    skills: [
      { name: '深渊冲击', cooldown: 6, effect: 'aoe', damage: 2.5, radius: 150 },
      { name: '黑暗触手', cooldown: 8, effect: 'summon', count: 4 },
      { name: '虚空射线', cooldown: 10, effect: 'beam', damage: 3.0 },
      { name: '现实扭曲', cooldown: 20, effect: 'teleport', count: 3 },
      { name: '深渊吞噬', cooldown: 25, effect: 'blackhole', damage: 4.0, radius: 200 }
    ],
    description: '地牢最深处的终极存在，传说中的深渊之主。'
  },

  // ==================== 召唤物 ====================
  summon_skeleton: {
    name: '骷髅战士',
    icon: '💀',
    type: 'summon',
    baseStats: { hp: 28, damage: 10, armor: 3, magicResist: 0 },
    speed: 1.2,
    attackRange: 35,
    attackSpeed: 1.0,
    exp: 0,
    gold: [0, 0],
    dropChance: 0,
    drops: [],
    size: 24,
    ai: 'friendly',
    lifetime: 30,
    description: '死灵法师召唤的骷髅仆从。'
  },
  summon_death_knight: {
    name: '死亡骑士',
    icon: '⚔️',
    type: 'summon',
    baseStats: { hp: 140, damage: 25, armor: 12, magicResist: 10 },
    speed: 1.3,
    attackRange: 50,
    attackSpeed: 1.0,
    exp: 0,
    gold: [0, 0],
    dropChance: 0,
    drops: [],
    size: 34,
    ai: 'friendly',
    lifetime: 60,
    description: '死灵法师的终极召唤物。'
  },

  // ==================== 工具方法 ====================

  // 根据层数获取怪物列表
  getMonstersForFloor(floor) {
    const monsters = [];
    if (floor <= 2) {
      monsters.push('slime', 'bat', 'rat');
    } else if (floor <= 4) {
      monsters.push('slime', 'bat', 'skeleton', 'goblin', 'rat');
    } else if (floor <= 6) {
      monsters.push('skeleton', 'zombie', 'goblin', 'spider', 'bat');
    } else if (floor <= 8) {
      monsters.push('zombie', 'spider', 'ghost', 'orc', 'goblin');
    } else if (floor <= 10) {
      monsters.push('ghost', 'orc', 'darkMage', 'werewolf', 'skeleton');
    } else if (floor <= 15) {
      monsters.push('orc', 'darkMage', 'werewolf', 'golem', 'vampire', 'wraith');
    } else if (floor <= 20) {
      monsters.push('golem', 'vampire', 'wraith', 'minotaur', 'demon', 'lich');
    } else {
      monsters.push('demon', 'lich', 'dragon', 'deathKnight', 'minotaur', 'golem');
    }
    // 精英怪
    if (floor >= 5 && floor % 3 === 0) {
      if (floor < 10) monsters.push('elite_skeleton_king');
      else monsters.push('elite_orc_chief');
    }
    return monsters;
  },

  // 随机获取该楼层的一个怪物
  getRandomMonsterForFloor(floor) {
    const monsters = this.getMonstersForFloor(floor);
    if (monsters.length === 0) return 'slime';
    return monsters[Math.floor(Math.random() * monsters.length)];
  },

  // 获取怪物基础数据
  getMonster(type) {
    return this[type] || null;
  },

  // 获取BOSS
  getBossForFloor(floor) {
    if (floor === 5) return 'boss_slime_king';
    if (floor === 10) return 'boss_skeleton_lord';
    if (floor === 15) return 'boss_dragon';
    if (floor === 20) return 'boss_demon_lord';
    if (floor === 25) return 'boss_lich_king';
    if (floor >= 30) return 'boss_final';
    return null;
  },

  // 创建怪物实例
  createMonster(type, x, y, floor = 1) {
    const data = this[type];
    if (!data) return null;
    const scale = 1 + (floor - 1) * 0.12;
    return {
      type,
      name: data.name,
      icon: data.icon,
      x, y,
      maxHp: Math.floor(data.baseStats.hp * scale),
      hp: Math.floor(data.baseStats.hp * scale),
      damage: Math.floor(data.baseStats.damage * scale),
      armor: Math.floor(data.baseStats.armor * scale),
      magicResist: data.baseStats.magicResist,
      speed: data.speed,
      attackRange: data.attackRange,
      attackSpeed: data.attackSpeed,
      exp: Math.floor(data.exp * scale),
      gold: [Math.floor(data.gold[0] * scale), Math.floor(data.gold[1] * scale)],
      dropChance: data.dropChance,
      drops: data.drops,
      size: data.size,
      ai: data.ai,
      isBoss: data.type === 'boss',
      isElite: data.type === 'elite',
      isSummon: data.type === 'summon',
      skills: data.skills || [],
      skillCooldowns: {},
      attackCooldown: 0,
      stunned: 0,
      confused: 0,
      poisoned: 0,
      burning: 0,
      slowed: 0,
      frozen: 0,
      lifetime: data.lifetime || 0,
      description: data.description,
      enraged: false
    };
  }
};

window.MonsterData = MonsterData;
