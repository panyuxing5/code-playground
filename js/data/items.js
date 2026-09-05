// ==================== 永恒地牢 - 物品数据配置 ====================
// 定义所有物品：武器、护甲、饰品、消耗品、材料

const ItemData = {
  // ==================== 消耗品 ====================
  healthPotion: {
    name: '生命药水',
    icon: '❤️',
    type: 'consumable',
    rarity: 'common',
    price: 20,
    stackable: true,
    maxStack: 99,
    effect: { type: 'heal', value: 50 },
    description: '恢复50点生命值。'
  },
  manaPotion: {
    name: '魔法药水',
    icon: '💙',
    type: 'consumable',
    rarity: 'common',
    price: 25,
    stackable: true,
    maxStack: 99,
    effect: { type: 'mana', value: 30 },
    description: '恢复30点魔法值。'
  },
  staminaPotion: {
    name: '体力药水',
    icon: '💚',
    type: 'consumable',
    rarity: 'common',
    price: 15,
    stackable: true,
    maxStack: 99,
    effect: { type: 'stamina', value: 50 },
    description: '恢复50点体力。'
  },
  superHealthPotion: {
    name: '高级生命药水',
    icon: '💖',
    type: 'consumable',
    rarity: 'uncommon',
    price: 60,
    stackable: true,
    maxStack: 99,
    effect: { type: 'heal', value: 150 },
    description: '恢复150点生命值。'
  },
  superManaPotion: {
    name: '高级魔法药水',
    icon: '💜',
    type: 'consumable',
    rarity: 'uncommon',
    price: 70,
    stackable: true,
    maxStack: 99,
    effect: { type: 'mana', value: 80 },
    description: '恢复80点魔法值。'
  },
  fullPotion: {
    name: '全满药水',
    icon: '🌈',
    type: 'consumable',
    rarity: 'rare',
    price: 200,
    stackable: true,
    maxStack: 20,
    effect: { type: 'full' },
    description: '完全恢复生命和魔法。'
  },
  strengthPotion: {
    name: '力量药水',
    icon: '💪',
    type: 'consumable',
    rarity: 'rare',
    price: 50,
    stackable: true,
    maxStack: 20,
    effect: { type: 'buff', stat: 'str', value: 10, duration: 60 },
    description: '力量+10，持续60秒。'
  },
  agilityPotion: {
    name: '迅捷药水',
    icon: '⚡',
    type: 'consumable',
    rarity: 'rare',
    price: 50,
    stackable: true,
    maxStack: 20,
    effect: { type: 'buff', stat: 'dex', value: 10, duration: 60 },
    description: '敏捷+10，持续60秒。'
  },
  defensePotion: {
    name: '铁皮药水',
    icon: '🛡️',
    type: 'consumable',
    rarity: 'rare',
    price: 50,
    stackable: true,
    maxStack: 20,
    effect: { type: 'buff', stat: 'armor', value: 15, duration: 60 },
    description: '护甲+15，持续60秒。'
  },
  invisibilityPotion: {
    name: '隐身药水',
    icon: '👻',
    type: 'consumable',
    rarity: 'epic',
    price: 150,
    stackable: true,
    maxStack: 10,
    effect: { type: 'stealth', duration: 30 },
    description: '隐身30秒，攻击后显形。'
  },
  bomb: {
    name: '炸弹',
    icon: '💣',
    type: 'consumable',
    rarity: 'uncommon',
    price: 40,
    stackable: true,
    maxStack: 20,
    effect: { type: 'damage', value: 80, radius: 100 },
    description: '对周围敌人造成80点伤害。'
  },
  fireBomb: {
    name: '火焰炸弹',
    icon: '🔥',
    type: 'consumable',
    rarity: 'rare',
    price: 80,
    stackable: true,
    maxStack: 10,
    effect: { type: 'damage', value: 120, radius: 120, burn: 5 },
    description: '造成120点火焰伤害并点燃敌人。'
  },
  key: {
    name: '钥匙',
    icon: '🔑',
    type: 'key',
    rarity: 'uncommon',
    price: 30,
    stackable: true,
    maxStack: 10,
    description: '可以打开上锁的宝箱。'
  },
  antidote: {
    name: '解毒剂',
    icon: '🧪',
    type: 'consumable',
    rarity: 'common',
    price: 15,
    stackable: true,
    maxStack: 50,
    effect: { type: 'cure', status: 'poison' },
    description: '解除中毒状态。'
  },
  cleanse: {
    name: '净化药水',
    icon: '✨',
    type: 'consumable',
    rarity: 'uncommon',
    price: 40,
    stackable: true,
    maxStack: 20,
    effect: { type: 'cleanse' },
    description: '解除所有负面状态。'
  },
  scrollOfTeleport: {
    name: '传送卷轴',
    icon: '📜',
    type: 'consumable',
    rarity: 'rare',
    price: 100,
    stackable: true,
    maxStack: 10,
    effect: { type: 'teleport', target: 'town' },
    description: '传送回城镇。'
  },
  scrollOfIdentify: {
    name: '鉴定卷轴',
    icon: '🔍',
    type: 'consumable',
    rarity: 'uncommon',
    price: 30,
    stackable: true,
    maxStack: 20,
    effect: { type: 'identify' },
    description: '鉴定一件未鉴定的物品。'
  },

  // ==================== 武器 ====================
  rustySword: {
    name: '生锈的剑',
    icon: '🗡️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 0,
    stats: { damage: 5, attackSpeed: 0 },
    requiredLevel: 1,
    description: '一把生锈的旧剑，聊胜于无。'
  },
  dagger: {
    name: '匕首',
    icon: '🔪',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 50,
    stats: { damage: 8, attackSpeed: 0.3, crit: 0.05 },
    requiredLevel: 1,
    description: '轻便的匕首，攻击速度快。'
  },
  shortSword: {
    name: '短剑',
    icon: '🗡️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 60,
    stats: { damage: 10 },
    requiredLevel: 2,
    description: '标准的短剑。'
  },
  ironSword: {
    name: '铁剑',
    icon: '⚔️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 80,
    stats: { damage: 12 },
    requiredLevel: 3,
    description: '铁制的长剑。'
  },
  steelSword: {
    name: '钢剑',
    icon: '⚔️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'uncommon',
    price: 200,
    stats: { damage: 20, crit: 0.05 },
    requiredLevel: 5,
    description: '精钢打造的长剑。'
  },
  silverSword: {
    name: '银剑',
    icon: '⚔️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'uncommon',
    price: 250,
    stats: { damage: 18, magicDamage: 10 },
    requiredLevel: 6,
    description: '镀银的剑，对亡灵有额外伤害。'
  },
  flameBlade: {
    name: '烈焰之刃',
    icon: '🔥',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 500,
    stats: { damage: 30, burnChance: 0.3 },
    requiredLevel: 8,
    effect: { type: 'burn', chance: 0.3, damage: 5, duration: 3 },
    description: '燃烧着永恒火焰的魔剑。'
  },
  shadowDagger: {
    name: '暗影匕首',
    icon: '🌑',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 600,
    stats: { damage: 25, attackSpeed: 0.4, crit: 0.15 },
    requiredLevel: 8,
    description: '蕴含暗影之力的匕首。'
  },
  runedBlade: {
    name: '符文之刃',
    icon: '✨',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 550,
    stats: { damage: 28, magicDamage: 15 },
    requiredLevel: 9,
    description: '刻满魔法符文的长剑。'
  },
  greatAxe: {
    name: '巨斧',
    icon: '🪓',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 480,
    stats: { damage: 40, attackSpeed: -0.3, crit: 0.1 },
    requiredLevel: 10,
    description: '沉重的巨斧，伤害极高但速度慢。'
  },
  dragonSlayer: {
    name: '屠龙剑',
    icon: '🐲',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'epic',
    price: 1500,
    stats: { damage: 50, crit: 0.1, dragonBonus: 0.5 },
    requiredLevel: 15,
    description: '传说中用于屠龙的圣剑。'
  },
  hellfireBlade: {
    name: '地狱火之刃',
    icon: '😈',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'epic',
    price: 1800,
    stats: { damage: 55, burnChance: 0.5 },
    requiredLevel: 18,
    effect: { type: 'burn', chance: 0.5, damage: 10, duration: 4 },
    description: '用地狱之火锻造的魔剑。'
  },
  excalibur: {
    name: '王者之剑',
    icon: '✨',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'legendary',
    price: 3000,
    stats: { damage: 80, crit: 0.2, critDamage: 0.5 },
    requiredLevel: 25,
    description: '传说中的圣剑，只有真正的王者才能挥舞。'
  },
  frostmourne: {
    name: '霜之哀伤',
    icon: '❄️',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'legendary',
    price: 3500,
    stats: { damage: 75, slowChance: 0.4, lifesteal: 0.1 },
    requiredLevel: 25,
    description: '诅咒之剑，吸取敌人的生命。'
  },
  abyssBlade: {
    name: '深渊之刃',
    icon: '🌑',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'mythic',
    price: 8000,
    stats: { damage: 120, crit: 0.25, critDamage: 1.0, lifesteal: 0.15 },
    requiredLevel: 30,
    description: '来自深渊最深处的神器，拥有毁灭一切的力量。'
  },

  // 远程武器
  shortBow: {
    name: '短弓',
    icon: '🏹',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 70,
    stats: { damage: 10, range: 50 },
    requiredLevel: 2,
    weaponType: 'ranged',
    description: '基础的短弓。'
  },
  longBow: {
    name: '长弓',
    icon: '🏹',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'uncommon',
    price: 220,
    stats: { damage: 18, range: 80, crit: 0.05 },
    requiredLevel: 5,
    weaponType: 'ranged',
    description: '射程更远的长弓。'
  },
  compositeBow: {
    name: '复合弓',
    icon: '🏹',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 550,
    stats: { damage: 28, range: 100, attackSpeed: 0.2 },
    requiredLevel: 10,
    weaponType: 'ranged',
    description: '精心制作的复合弓。'
  },
  windRunner: {
    name: '风行者',
    icon: '💨',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'epic',
    price: 1600,
    stats: { damage: 45, range: 120, attackSpeed: 0.4, crit: 0.15 },
    requiredLevel: 18,
    weaponType: 'ranged',
    description: '风之精灵祝福的神弓。'
  },

  // 法杖
  apprenticeStaff: {
    name: '学徒法杖',
    icon: '🪄',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'common',
    price: 60,
    stats: { damage: 6, magicDamage: 12, mpRegen: 1 },
    requiredLevel: 1,
    weaponType: 'staff',
    description: '法师学徒的基础法杖。'
  },
  wizardStaff: {
    name: '巫师法杖',
    icon: '🪄',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'uncommon',
    price: 230,
    stats: { damage: 10, magicDamage: 25, mpRegen: 2 },
    requiredLevel: 5,
    weaponType: 'staff',
    description: '正式巫师使用的法杖。'
  },
  archmageStaff: {
    name: '大法师法杖',
    icon: '🔮',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'rare',
    price: 600,
    stats: { damage: 15, magicDamage: 45, mpRegen: 3, crit: 0.1 },
    requiredLevel: 10,
    weaponType: 'staff',
    description: '大法师才能使用的强力法杖。'
  },
  staffOfEternity: {
    name: '永恒法杖',
    icon: '✨',
    type: 'weapon',
    slot: 'weapon',
    rarity: 'legendary',
    price: 3200,
    stats: { damage: 25, magicDamage: 80, mpRegen: 5, crit: 0.2 },
    requiredLevel: 25,
    weaponType: 'staff',
    description: '蕴含永恒魔力的神器。'
  },

  // ==================== 护甲 ====================
  clothArmor: {
    name: '布甲',
    icon: '👕',
    type: 'armor',
    slot: 'armor',
    rarity: 'common',
    price: 0,
    stats: { armor: 3, magicResist: 5 },
    requiredLevel: 1,
    description: '普通的布制护甲。'
  },
  leatherArmor: {
    name: '皮甲',
    icon: '🦺',
    type: 'armor',
    slot: 'armor',
    rarity: 'common',
    price: 100,
    stats: { armor: 8, dodge: 0.03 },
    requiredLevel: 2,
    description: '轻便的皮革护甲。'
  },
  chainmail: {
    name: '锁子甲',
    icon: '🛡️',
    type: 'armor',
    slot: 'armor',
    rarity: 'uncommon',
    price: 250,
    stats: { armor: 15 },
    requiredLevel: 5,
    description: '铁环编织的护甲。'
  },
  scaleArmor: {
    name: '鳞甲',
    icon: '🐟',
    type: 'armor',
    slot: 'armor',
    rarity: 'uncommon',
    price: 280,
    stats: { armor: 18, magicResist: 5 },
    requiredLevel: 6,
    description: '鳞片制成的护甲。'
  },
  plateArmor: {
    name: '板甲',
    icon: '🛡️',
    type: 'armor',
    slot: 'armor',
    rarity: 'rare',
    price: 600,
    stats: { armor: 25, moveSpeed: -0.3 },
    requiredLevel: 10,
    description: '厚重的全身板甲。'
  },
  darkRobe: {
    name: '暗黑法袍',
    icon: '🥋',
    type: 'armor',
    slot: 'armor',
    rarity: 'rare',
    price: 550,
    stats: { armor: 8, magicResist: 25, mpRegen: 2 },
    requiredLevel: 10,
    description: '暗黑法师穿的法袍。'
  },
  dragonScale: {
    name: '龙鳞甲',
    icon: '🐉',
    type: 'armor',
    slot: 'armor',
    rarity: 'epic',
    price: 1500,
    stats: { armor: 40, magicResist: 20, fireResist: 0.3 },
    requiredLevel: 15,
    description: '用龙鳞打造的护甲。'
  },
  boneArmor: {
    name: '骨甲',
    icon: '🦴',
    type: 'armor',
    slot: 'armor',
    rarity: 'epic',
    price: 1400,
    stats: { armor: 35, magicResist: 15, thorns: 0.1 },
    requiredLevel: 12,
    description: '用骨头拼接的护甲。'
  },
  demonArmor: {
    name: '恶魔护甲',
    icon: '😈',
    type: 'armor',
    slot: 'armor',
    rarity: 'epic',
    price: 1800,
    stats: { armor: 45, fireResist: 0.3, damage: 10 },
    requiredLevel: 18,
    description: '用恶魔皮制作的护甲。'
  },
  divineArmor: {
    name: '神圣战甲',
    icon: '👼',
    type: 'armor',
    slot: 'armor',
    rarity: 'legendary',
    price: 3000,
    stats: { armor: 60, magicResist: 30, hpRegen: 5 },
    requiredLevel: 25,
    description: '圣光加持的神圣战甲。'
  },
  lichKingArmor: {
    name: '巫妖王铠甲',
    icon: '💀',
    type: 'armor',
    slot: 'armor',
    rarity: 'legendary',
    price: 3500,
    stats: { armor: 55, magicResist: 40, mpRegen: 5, summonDamage: 0.2 },
    requiredLevel: 25,
    description: '巫妖王穿过的铠甲。'
  },
  abyssArmor: {
    name: '深渊战甲',
    icon: '🌑',
    type: 'armor',
    slot: 'armor',
    rarity: 'mythic',
    price: 8000,
    stats: { armor: 80, magicResist: 50, hpRegen: 10, damageReduction: 0.15 },
    requiredLevel: 30,
    description: '深渊之主的铠甲。'
  },

  // 头盔
  leatherHelm: {
    name: '皮帽',
    icon: '🎩',
    type: 'armor',
    slot: 'helmet',
    rarity: 'common',
    price: 50,
    stats: { armor: 2 },
    requiredLevel: 1,
    description: '简单的皮帽。'
  },
  ironHelm: {
    name: '铁盔',
    icon: '⛑️',
    type: 'armor',
    slot: 'helmet',
    rarity: 'uncommon',
    price: 150,
    stats: { armor: 5 },
    requiredLevel: 5,
    description: '铁制头盔。'
  },
  knightHelm: {
    name: '骑士头盔',
    icon: '⛑️',
    type: 'armor',
    slot: 'helmet',
    rarity: 'rare',
    price: 400,
    stats: { armor: 10, magicResist: 5 },
    requiredLevel: 10,
    description: '骑士佩戴的头盔。'
  },
  deathKnightHelm: {
    name: '死亡骑士头盔',
    icon: '💀',
    type: 'armor',
    slot: 'helmet',
    rarity: 'epic',
    price: 1000,
    stats: { armor: 15, magicResist: 10, crit: 0.05 },
    requiredLevel: 15,
    description: '死亡骑士的头盔。'
  },
  skeletonCrown: {
    name: '骷髅王冠',
    icon: '👑',
    type: 'armor',
    slot: 'helmet',
    rarity: 'epic',
    price: 1200,
    stats: { armor: 12, magicDamage: 20, mpRegen: 3 },
    requiredLevel: 12,
    description: '骷髅王的王冠。'
  },

  // 靴子
  leatherBoots: {
    name: '皮靴',
    icon: '👢',
    type: 'armor',
    slot: 'boots',
    rarity: 'common',
    price: 50,
    stats: { armor: 2, moveSpeed: 0.1 },
    requiredLevel: 1,
    description: '普通的皮靴。'
  },
  ironBoots: {
    name: '铁靴',
    icon: '🥾',
    type: 'armor',
    slot: 'boots',
    rarity: 'uncommon',
    price: 150,
    stats: { armor: 5 },
    requiredLevel: 5,
    description: '沉重的铁靴。'
  },
  swiftBoots: {
    name: '疾风之靴',
    icon: '💨',
    type: 'armor',
    slot: 'boots',
    rarity: 'rare',
    price: 450,
    stats: { armor: 5, moveSpeed: 0.5, dodge: 0.05 },
    requiredLevel: 10,
    description: '风之精灵祝福的靴子。'
  },
  dragonBoots: {
    name: '龙鳞靴',
    icon: '🐲',
    type: 'armor',
    slot: 'boots',
    rarity: 'epic',
    price: 900,
    stats: { armor: 12, moveSpeed: 0.3, fireResist: 0.2 },
    requiredLevel: 15,
    description: '龙鳞制作的靴子。'
  },

  // ==================== 饰品 ====================
  ringOfPower: {
    name: '力量之戒',
    icon: '💍',
    type: 'accessory',
    slot: 'ring',
    rarity: 'rare',
    price: 300,
    stats: { str: 8, damage: 5 },
    requiredLevel: 5,
    description: '蕴含力量的戒指。'
  },
  ringOfAgility: {
    name: '敏捷之戒',
    icon: '💍',
    type: 'accessory',
    slot: 'ring',
    rarity: 'rare',
    price: 300,
    stats: { dex: 8, crit: 0.05 },
    requiredLevel: 5,
    description: '提升敏捷的戒指。'
  },
  ringOfWisdom: {
    name: '智慧之戒',
    icon: '💍',
    type: 'accessory',
    slot: 'ring',
    rarity: 'rare',
    price: 300,
    stats: { int: 10, mpRegen: 2 },
    requiredLevel: 5,
    description: '提升智力的戒指。'
  },
  ringOfVitality: {
    name: '生命之戒',
    icon: '💍',
    type: 'accessory',
    slot: 'ring',
    rarity: 'rare',
    price: 350,
    stats: { vit: 8, maxHp: 50, hpRegen: 2 },
    requiredLevel: 5,
    description: '提升生命的戒指。'
  },
  amuletOfWisdom: {
    name: '智慧护符',
    icon: '📿',
    type: 'accessory',
    slot: 'amulet',
    rarity: 'rare',
    price: 400,
    stats: { int: 10, magicDamage: 15 },
    requiredLevel: 8,
    description: '蕴含智慧的护符。'
  },
  amuletOfPower: {
    name: '力量护符',
    icon: '📿',
    type: 'accessory',
    slot: 'amulet',
    rarity: 'rare',
    price: 400,
    stats: { str: 10, damage: 10 },
    requiredLevel: 8,
    description: '蕴含力量的护符。'
  },
  luckyCharm: {
    name: '幸运符',
    icon: '🍀',
    type: 'accessory',
    slot: 'amulet',
    rarity: 'epic',
    price: 500,
    stats: { luck: 15, crit: 0.1, goldBonus: 0.2, dropRate: 0.15 },
    requiredLevel: 10,
    description: '带来好运的护符。'
  },
  heartOfDracula: {
    name: '德古拉之心',
    icon: '🩸',
    type: 'accessory',
    slot: 'amulet',
    rarity: 'legendary',
    price: 2000,
    stats: { lifesteal: 0.15, maxHp: 100, hpRegen: 5 },
    requiredLevel: 20,
    description: '吸血鬼始祖的心脏。'
  },
  abyssRing: {
    name: '深渊之戒',
    icon: '🌑',
    type: 'accessory',
    slot: 'ring',
    rarity: 'mythic',
    price: 5000,
    stats: { allStats: 15, damage: 30, crit: 0.15, critDamage: 0.5 },
    requiredLevel: 30,
    description: '深渊之主的戒指。'
  },
  phylactery: {
    name: '护符匣',
    icon: '💎',
    type: 'accessory',
    slot: 'amulet',
    rarity: 'legendary',
    price: 2500,
    stats: { int: 20, magicDamage: 30, mpRegen: 5, summonDamage: 0.3 },
    requiredLevel: 22,
    description: '巫妖的灵魂容器。'
  },

  // ==================== 材料 ====================
  slimeGel: {
    name: '史莱姆凝胶',
    icon: '🟢',
    type: 'material',
    rarity: 'common',
    price: 5,
    stackable: true,
    maxStack: 99,
    description: '史莱姆身上的凝胶，可以用于炼金。'
  },
  batWing: {
    name: '蝙蝠翅膀',
    icon: '🦇',
    type: 'material',
    rarity: 'common',
    price: 8,
    stackable: true,
    maxStack: 99,
    description: '蝙蝠的翅膀。'
  },
  ratTail: {
    name: '老鼠尾巴',
    icon: '🐀',
    type: 'material',
    rarity: 'common',
    price: 3,
    stackable: true,
    maxStack: 99,
    description: '巨鼠的尾巴。'
  },
  bone: {
    name: '骨头',
    icon: '🦴',
    type: 'material',
    rarity: 'common',
    price: 10,
    stackable: true,
    maxStack: 99,
    description: '骷髅的骨头。'
  },
  rottenFlesh: {
    name: '腐肉',
    icon: '🥩',
    type: 'material',
    rarity: 'common',
    price: 5,
    stackable: true,
    maxStack: 99,
    description: '僵尸身上的腐肉。'
  },
  goblinEar: {
    name: '哥布林耳朵',
    icon: '👂',
    type: 'material',
    rarity: 'common',
    price: 12,
    stackable: true,
    maxStack: 99,
    description: '哥布林的耳朵。'
  },
  spiderSilk: {
    name: '蛛丝',
    icon: '🕸️',
    type: 'material',
    rarity: 'uncommon',
    price: 15,
    stackable: true,
    maxStack: 99,
    description: '巨蛛的丝，可以制作装备。'
  },
  poisonVial: {
    name: '毒液',
    icon: '🧪',
    type: 'material',
    rarity: 'uncommon',
    price: 20,
    stackable: true,
    maxStack: 99,
    description: '蜘蛛的毒液。'
  },
  ectoplasm: {
    name: '灵质',
    icon: '👻',
    type: 'material',
    rarity: 'uncommon',
    price: 25,
    stackable: true,
    maxStack: 99,
    description: '幽灵留下的灵能物质。'
  },
  orcTusk: {
    name: '兽人獠牙',
    icon: '🦷',
    type: 'material',
    rarity: 'uncommon',
    price: 30,
    stackable: true,
    maxStack: 99,
    description: '兽人的獠牙。'
  },
  wolfFang: {
    name: '狼牙',
    icon: '🦷',
    type: 'material',
    rarity: 'uncommon',
    price: 28,
    stackable: true,
    maxStack: 99,
    description: '狼人的尖牙。'
  },
  wolfPelt: {
    name: '狼皮',
    icon: '🐺',
    type: 'material',
    rarity: 'uncommon',
    price: 35,
    stackable: true,
    maxStack: 99,
    description: '狼人的皮毛。'
  },
  stoneFragment: {
    name: '石头碎片',
    icon: '🪨',
    type: 'material',
    rarity: 'uncommon',
    price: 20,
    stackable: true,
    maxStack: 99,
    description: '石魔像的碎片。'
  },
  vampireFang: {
    name: '吸血鬼尖牙',
    icon: '🦷',
    type: 'material',
    rarity: 'rare',
    price: 50,
    stackable: true,
    maxStack: 99,
    description: '吸血鬼的尖牙。'
  },
  bloodVial: {
    name: '血瓶',
    icon: '🩸',
    type: 'material',
    rarity: 'rare',
    price: 45,
    stackable: true,
    maxStack: 99,
    description: '吸血鬼的血液。'
  },
  soulShard: {
    name: '灵魂碎片',
    icon: '💎',
    type: 'material',
    rarity: 'rare',
    price: 60,
    stackable: true,
    maxStack: 99,
    description: '怨灵的灵魂碎片。'
  },
  minotaurHorn: {
    name: '牛角',
    icon: '🐂',
    type: 'material',
    rarity: 'rare',
    price: 70,
    stackable: true,
    maxStack: 99,
    description: '牛头人的角。'
  },
  demonHorn: {
    name: '恶魔角',
    icon: '😈',
    type: 'material',
    rarity: 'epic',
    price: 100,
    stackable: true,
    maxStack: 99,
    description: '恶魔的角。'
  },
  fireGem: {
    name: '火焰宝石',
    icon: '🔥',
    type: 'material',
    rarity: 'epic',
    price: 120,
    stackable: true,
    maxStack: 99,
    description: '蕴含火焰之力的宝石。'
  },
  dragonScaleMat: {
    name: '龙鳞',
    icon: '🐲',
    type: 'material',
    rarity: 'epic',
    price: 150,
    stackable: true,
    maxStack: 99,
    description: '龙的鳞片。'
  },
  dragonHeart: {
    name: '龙心',
    icon: '❤️',
    type: 'material',
    rarity: 'legendary',
    price: 500,
    stackable: true,
    maxStack: 10,
    description: '龙的心脏，蕴含强大力量。'
  },
  demonHeart: {
    name: '恶魔之心',
    icon: '🖤',
    type: 'material',
    rarity: 'legendary',
    price: 600,
    stackable: true,
    maxStack: 10,
    description: '恶魔的心脏。'
  },
  magicScroll: {
    name: '魔法卷轴',
    icon: '📜',
    type: 'material',
    rarity: 'rare',
    price: 80,
    stackable: true,
    maxStack: 20,
    description: '记载着魔法知识的卷轴。'
  },
  ironIngot: {
    name: '铁锭',
    icon: '🔩',
    type: 'material',
    rarity: 'common',
    price: 15,
    stackable: true,
    maxStack: 99,
    description: '基础锻造材料。'
  },
  magicDust: {
    name: '魔法粉尘',
    icon: '✨',
    type: 'material',
    rarity: 'uncommon',
    price: 30,
    stackable: true,
    maxStack: 99,
    description: '附魔用的魔法材料。'
  },
  herb: {
    name: '草药',
    icon: '🌿',
    type: 'material',
    rarity: 'common',
    price: 5,
    stackable: true,
    maxStack: 99,
    description: '常见的炼金材料。'
  },
  gold: {
    name: '金币',
    icon: '💰',
    type: 'currency',
    rarity: 'common',
    price: 1,
    stackable: true,
    maxStack: 99999,
    description: '通用货币。'
  },

  // ==================== 工具方法 ====================

  // 获取物品
  getItem(id) {
    return this[id] || null;
  },

  // 创建物品实例
  createItem(id, quantity = 1) {
    const data = this[id];
    if (!data) return null;
    return {
      id,
      ...data,
      quantity: data.stackable ? quantity : 1
    };
  },

  // 根据稀有度和等级获取随机装备
  getRandomEquipment(level, rarity = null) {
    const rarities = rarity ? [rarity] : ['common', 'uncommon', 'rare', 'epic', 'legendary'];
    const weights = rarity ? [1] : [50, 30, 15, 4, 1];
    const selectedRarity = Utils.weightedChoice(rarities, weights);

    const equipment = [];
    for (const key in this) {
      const item = this[key];
      if (item.type === 'weapon' || item.type === 'armor' || item.type === 'accessory') {
        if (item.rarity === selectedRarity && item.requiredLevel <= level + 3) {
          equipment.push(key);
        }
      }
    }
    if (equipment.length === 0) return 'rustySword';
    return Utils.randomChoice(equipment);
  },

  // 获取随机消耗品
  getRandomConsumable(level) {
    const consumables = [];
    for (const key in this) {
      const item = this[key];
      if (item.type === 'consumable' && item.price <= level * 30) {
        consumables.push(key);
      }
    }
    if (consumables.length === 0) return 'healthPotion';
    return Utils.randomChoice(consumables);
  },

  // 计算物品总属性
  getItemStats(item) {
    if (!item || !item.stats) return {};
    return { ...item.stats };
  }
};

window.ItemData = ItemData;
