// ==================== 永恒地牢 - 宠物系统 ====================
// 宠物的召唤、属性、技能、升级、进化

const PetData = {
  // ==================== 宠物定义 ====================
  slime_pet: {
    id: 'slime_pet',
    name: '史莱姆宝宝',
    icon: '🟢',
    type: 'normal',
    rarity: 'common',
    baseStats: {
      hp: 50,
      damage: 5,
      armor: 2,
      speed: 2.0,
      attackSpeed: 1.0
    },
    growth: {
      hp: 8,
      damage: 1.5,
      armor: 0.5,
      speed: 0.05
    },
    skills: ['slime_bounce'],
    ability: '分裂攻击',
    description: '可爱的史莱姆宝宝，会弹跳攻击敌人。',
    evolveTo: 'king_slime_pet',
    evolveLevel: 20
  },
  wolf_pet: {
    id: 'wolf_pet',
    name: '幼狼',
    icon: '🐺',
    type: 'beast',
    rarity: 'uncommon',
    baseStats: {
      hp: 80,
      damage: 12,
      armor: 5,
      speed: 3.5,
      attackSpeed: 1.5
    },
    growth: {
      hp: 12,
      damage: 3,
      armor: 1,
      speed: 0.08
    },
    skills: ['wolf_bite', 'wolf_howl'],
    ability: '嗜血：击杀后提升攻击力',
    description: '忠诚的幼狼，速度快，攻击高。',
    evolveTo: 'dire_wolf_pet',
    evolveLevel: 25
  },
  fairy_pet: {
    id: 'fairy_pet',
    name: '小精灵',
    icon: '🧚',
    type: 'magic',
    rarity: 'rare',
    baseStats: {
      hp: 40,
      damage: 15,
      armor: 1,
      speed: 4.0,
      attackSpeed: 1.2
    },
    growth: {
      hp: 6,
      damage: 4,
      armor: 0.3,
      speed: 0.1
    },
    skills: ['fairy_heal', 'fairy_bolt'],
    ability: '治疗光环：每秒恢复主人生命',
    description: '会治疗主人的小精灵，魔法伤害高。',
    evolveTo: 'spirit_pet',
    evolveLevel: 30
  },
  skeleton_pet: {
    id: 'skeleton_pet',
    name: '小骷髅',
    icon: '💀',
    type: 'undead',
    rarity: 'uncommon',
    baseStats: {
      hp: 70,
      damage: 10,
      armor: 8,
      speed: 2.5,
      attackSpeed: 1.0
    },
    growth: {
      hp: 10,
      damage: 2.5,
      armor: 1.5,
      speed: 0.05
    },
    skills: ['bone_throw', 'bone_shield'],
    ability: '骨盾：为主人分担伤害',
    description: '死灵法师的好帮手，防御高。',
    evolveTo: 'skeleton_knight_pet',
    evolveLevel: 25
  },
  dragon_pet: {
    id: 'dragon_pet',
    name: '幼龙',
    icon: '🐲',
    type: 'dragon',
    rarity: 'legendary',
    baseStats: {
      hp: 150,
      damage: 25,
      armor: 15,
      speed: 3.0,
      attackSpeed: 1.0
    },
    growth: {
      hp: 20,
      damage: 5,
      armor: 2,
      speed: 0.1
    },
    skills: ['dragon_breath', 'dragon_claw', 'dragon_roar'],
    ability: '龙息：范围火焰伤害',
    description: '传说中的幼龙，全属性成长极高。',
    evolveTo: 'adult_dragon_pet',
    evolveLevel: 40
  },
  ghost_pet: {
    id: 'ghost_pet',
    name: '小幽灵',
    icon: '👻',
    type: 'spirit',
    rarity: 'rare',
    baseStats: {
      hp: 60,
      damage: 18,
      armor: 0,
      speed: 3.5,
      attackSpeed: 1.3
    },
    growth: {
      hp: 8,
      damage: 4,
      armor: 0,
      speed: 0.08
    },
    skills: ['ghost_possess', 'ghost_curse'],
    ability: '穿墙：无视碰撞',
    description: '飘忽不定的小幽灵，魔法攻击强。',
    evolveTo: 'wraith_pet',
    evolveLevel: 30
  },
  golem_pet: {
    id: 'golem_pet',
    name: '石头人',
    icon: '🗿',
    type: 'construct',
    rarity: 'epic',
    baseStats: {
      hp: 200,
      damage: 15,
      armor: 25,
      speed: 1.5,
      attackSpeed: 0.7
    },
    growth: {
      hp: 25,
      damage: 3,
      armor: 3,
      speed: 0.02
    },
    skills: ['golem_slam', 'golem_taunt'],
    ability: '嘲讽：吸引敌人攻击',
    description: '坚不可摧的石头人，坦克型宠物。',
    evolveTo: 'titan_pet',
    evolveLevel: 35
  },
  phoenix_pet: {
    id: 'phoenix_pet',
    name: '凤凰雏鸟',
    icon: '🦅',
    type: 'divine',
    rarity: 'mythic',
    baseStats: {
      hp: 120,
      damage: 30,
      armor: 10,
      speed: 4.5,
      attackSpeed: 1.5
    },
    growth: {
      hp: 18,
      damage: 6,
      armor: 1.5,
      speed: 0.12
    },
    skills: ['phoenix_fire', 'phoenix_rebirth', 'phoenix_dive'],
    ability: '涅槃：死亡后复活一次',
    description: '不死鸟的雏鸟，拥有复活能力。',
    evolveTo: null,
    evolveLevel: null
  },

  // ==================== 神话生物 - 龙族 ====================
  fire_dragon_pet: {
    id: 'fire_dragon_pet',
    name: '火龙幼崽',
    icon: '🐉',
    type: 'dragon',
    rarity: 'legendary',
    baseStats: {
      hp: 180,
      damage: 45,
      armor: 15,
      speed: 3.0,
      attackSpeed: 1.0
    },
    growth: {
      hp: 25,
      damage: 8,
      armor: 2,
      speed: 0.08
    },
    skills: ['fire_dragon_breath', 'fire_dragon_claw', 'fire_dragon_roar', 'inferno_burst'],
    ability: '烈焰吐息：攻击附带燃烧效果',
    description: '传说中的火龙幼崽，掌控烈焰之力，输出天花板。',
    evolveTo: 'ancient_fire_dragon_pet',
    evolveLevel: 40,
    isMythic: true,
    obtainMethod: '抽卡/BOSS掉落'
  },
  ice_dragon_pet: {
    id: 'ice_dragon_pet',
    name: '冰龙幼崽',
    icon: '🐲',
    type: 'dragon',
    rarity: 'legendary',
    baseStats: {
      hp: 200,
      damage: 40,
      armor: 20,
      speed: 2.8,
      attackSpeed: 0.9
    },
    growth: {
      hp: 28,
      damage: 7,
      armor: 2.5,
      speed: 0.07
    },
    skills: ['ice_dragon_breath', 'frost_nova', 'ice_dragon_claw', 'absolute_zero'],
    ability: '冰霜吐息：攻击附带减速效果',
    description: '来自极寒之地的冰龙，掌控冰霜之力。',
    evolveTo: 'ancient_ice_dragon_pet',
    evolveLevel: 40,
    isMythic: true,
    obtainMethod: '抽卡/BOSS掉落'
  },
  thunder_dragon_pet: {
    id: 'thunder_dragon_pet',
    name: '雷龙幼崽',
    icon: '⚡',
    type: 'dragon',
    rarity: 'legendary',
    baseStats: {
      hp: 160,
      damage: 50,
      armor: 12,
      speed: 4.0,
      attackSpeed: 1.3
    },
    growth: {
      hp: 22,
      damage: 9,
      armor: 1.8,
      speed: 0.1
    },
    skills: ['thunder_dragon_breath', 'lightning_strike', 'thunder_roar', 'thunder_storm'],
    ability: '雷电吐息：攻击有几率眩晕',
    description: '掌控雷霆的雷龙，攻击速度快，爆发极高。',
    evolveTo: 'ancient_thunder_dragon_pet',
    evolveLevel: 40,
    isMythic: true,
    obtainMethod: '抽卡/BOSS掉落'
  },

  // ==================== 神话生物 - 祥瑞 ====================
  qilin_pet: {
    id: 'qilin_pet',
    name: '麒麟幼兽',
    icon: '🦌',
    type: 'divine',
    rarity: 'mythic',
    baseStats: {
      hp: 250,
      damage: 25,
      armor: 20,
      speed: 3.5,
      attackSpeed: 1.0
    },
    growth: {
      hp: 30,
      damage: 4,
      armor: 3,
      speed: 0.08
    },
    skills: ['qilin_heal', 'qilin_blessing', 'qilin_revive', 'sacred_light'],
    ability: '祥瑞之光：持续治愈主人',
    description: '祥瑞之兽麒麟，拥有强大的治愈和复活能力。',
    evolveTo: null,
    evolveLevel: null,
    isMythic: true,
    obtainMethod: '抽卡/BOSS掉落'
  },

  // ==================== 神话生物 - 坐骑 ====================
  unicorn_pet: {
    id: 'unicorn_pet',
    name: '独角兽',
    icon: '🦄',
    type: 'mount',
    rarity: 'legendary',
    baseStats: {
      hp: 150,
      damage: 20,
      armor: 15,
      speed: 5.0,
      attackSpeed: 1.0
    },
    growth: {
      hp: 20,
      damage: 3,
      armor: 2,
      speed: 0.15
    },
    skills: ['unicorn_purify', 'unicorn_heal', 'holy_charge'],
    ability: '坐骑：骑乘后移动速度+50%，可净化debuff',
    description: '纯洁的独角兽，可以作为坐骑，附带净化能力。',
    evolveTo: null,
    evolveLevel: null,
    isMythic: true,
    isMount: true,
    mountSpeedBonus: 0.5,
    obtainMethod: '抽卡/BOSS掉落'
  },
  pegasus_pet: {
    id: 'pegasus_pet',
    name: '飞马',
    icon: '🐴',
    type: 'mount',
    rarity: 'legendary',
    baseStats: {
      hp: 140,
      damage: 22,
      armor: 12,
      speed: 5.5,
      attackSpeed: 1.1
    },
    growth: {
      hp: 18,
      damage: 4,
      armor: 1.8,
      speed: 0.18
    },
    skills: ['pegasus_purify', 'wind_blade', 'sky_dive'],
    ability: '坐骑：骑乘后移动速度+60%，可飞行无视障碍，净化debuff',
    description: '展翅高飞的飞马，可以作为坐骑，速度极快。',
    evolveTo: null,
    evolveLevel: null,
    isMythic: true,
    isMount: true,
    mountSpeedBonus: 0.6,
    canFly: true,
    obtainMethod: '抽卡/BOSS掉落'
  },

  // ==================== 远古龙进化形态 ====================
  ancient_fire_dragon_pet: {
    id: 'ancient_fire_dragon_pet',
    name: '远古火龙',
    icon: '🔥',
    type: 'dragon',
    rarity: 'mythic',
    baseStats: { hp: 400, damage: 80, armor: 30, speed: 3.5, attackSpeed: 1.2 },
    growth: { hp: 35, damage: 12, armor: 3, speed: 0.1 },
    skills: ['fire_dragon_breath', 'inferno_burst', 'fire_dragon_roar', 'meteor_shower'],
    ability: '远古烈焰：大范围火焰爆发，焚烧一切',
    description: '进化后的远古火龙，真正的火焰霸主。'
  },
  ancient_ice_dragon_pet: {
    id: 'ancient_ice_dragon_pet',
    name: '远古冰龙',
    icon: '❄️',
    type: 'dragon',
    rarity: 'mythic',
    baseStats: { hp: 450, damage: 70, armor: 40, speed: 3.2, attackSpeed: 1.0 },
    growth: { hp: 40, damage: 10, armor: 4, speed: 0.09 },
    skills: ['ice_dragon_breath', 'absolute_zero', 'frost_nova', 'blizzard'],
    ability: '绝对零度：冻结周围所有敌人',
    description: '进化后的远古冰龙，冰封万里。'
  },
  ancient_thunder_dragon_pet: {
    id: 'ancient_thunder_dragon_pet',
    name: '远古雷龙',
    icon: '🌩️',
    type: 'dragon',
    rarity: 'mythic',
    baseStats: { hp: 350, damage: 90, armor: 25, speed: 4.5, attackSpeed: 1.5 },
    growth: { hp: 30, damage: 14, armor: 2.5, speed: 0.12 },
    skills: ['thunder_dragon_breath', 'thunder_storm', 'lightning_strike', 'divine_thunder'],
    ability: '天雷：召唤天雷劈打所有敌人',
    description: '进化后的远古雷龙，雷霆之主。'
  },

  // 进化形态
  king_slime_pet: {
    id: 'king_slime_pet',
    name: '史莱姆王',
    icon: '👑',
    type: 'normal',
    rarity: 'rare',
    baseStats: { hp: 150, damage: 20, armor: 10, speed: 2.5, attackSpeed: 1.2 },
    growth: { hp: 15, damage: 3, armor: 1, speed: 0.06 },
    skills: ['slime_bounce', 'slime_split', 'slime_wave'],
    ability: '分裂：被攻击时分裂小史莱姆',
    description: '史莱姆的进化形态，戴着王冠。'
  },
  dire_wolf_pet: {
    id: 'dire_wolf_pet',
    name: '恐狼',
    icon: '🐺',
    type: 'beast',
    rarity: 'epic',
    baseStats: { hp: 200, damage: 35, armor: 12, speed: 4.5, attackSpeed: 1.8 },
    growth: { hp: 18, damage: 5, armor: 1.5, speed: 0.1 },
    skills: ['wolf_bite', 'wolf_howl', 'wolf_pack'],
    ability: '狼群：召唤两只小狼助战',
    description: '远古巨兽恐狼，凶猛无比。'
  },

  // ==================== 宠物技能 ====================
  petSkills: {
    slime_bounce: {
      name: '弹跳攻击',
      description: '弹跳攻击敌人，造成120%伤害',
      cooldown: 2,
      damageMultiplier: 1.2
    },
    slime_split: {
      name: '分裂',
      description: '分裂出小史莱姆攻击',
      cooldown: 10,
      summon: 'mini_slime'
    },
    slime_wave: {
      name: '史莱姆波',
      description: '范围攻击，造成150%伤害',
      cooldown: 8,
      damageMultiplier: 1.5,
      radius: 80
    },
    wolf_bite: {
      name: '撕咬',
      description: '凶猛撕咬，造成150%伤害',
      cooldown: 3,
      damageMultiplier: 1.5
    },
    wolf_howl: {
      name: '狼嚎',
      description: '提升主人和自身攻击力20%',
      cooldown: 15,
      buff: { stat: 'damage', value: 1.2, duration: 10 }
    },
    wolf_pack: {
      name: '狼群召唤',
      description: '召唤两只小狼助战',
      cooldown: 20,
      summon: 'mini_wolf',
      count: 2
    },
    fairy_heal: {
      name: '治愈之光',
      description: '治疗主人10%最大生命',
      cooldown: 8,
      healPercent: 0.1
    },
    fairy_bolt: {
      name: '魔法箭',
      description: '发射魔法箭，造成180%魔法伤害',
      cooldown: 2,
      damageMultiplier: 1.8,
      projectile: true
    },
    bone_throw: {
      name: '投骨',
      description: '投掷骨头，造成130%伤害',
      cooldown: 2.5,
      damageMultiplier: 1.3,
      projectile: true
    },
    bone_shield: {
      name: '骨盾',
      description: '为主人增加护盾，吸收50点伤害',
      cooldown: 12,
      shield: 50
    },
    dragon_breath: {
      name: '龙息',
      description: '喷射龙息，造成200%范围火焰伤害',
      cooldown: 5,
      damageMultiplier: 2.0,
      radius: 100,
      burn: true
    },
    dragon_claw: {
      name: '龙爪',
      description: '利爪攻击，造成250%伤害',
      cooldown: 3,
      damageMultiplier: 2.5
    },
    dragon_roar: {
      name: '龙吼',
      description: '震慑周围敌人，眩晕2秒',
      cooldown: 15,
      stun: 2,
      radius: 120
    },
    ghost_possess: {
      name: '附身',
      description: '附身在敌人身上，持续造成伤害',
      cooldown: 10,
      dot: { damage: 5, duration: 5 }
    },
    ghost_curse: {
      name: '诅咒',
      description: '诅咒敌人，降低其30%攻击力',
      cooldown: 12,
      debuff: { stat: 'damage', value: 0.7, duration: 8 }
    },
    golem_slam: {
      name: '大地震击',
      description: '猛击地面，造成180%范围伤害',
      cooldown: 4,
      damageMultiplier: 1.8,
      radius: 90
    },
    golem_taunt: {
      name: '嘲讽',
      description: '嘲讽周围敌人，强制攻击自己',
      cooldown: 8,
      taunt: true,
      radius: 100
    },
    phoenix_fire: {
      name: '凤凰之火',
      description: '释放神圣火焰，造成300%伤害',
      cooldown: 4,
      damageMultiplier: 3.0,
      burn: true
    },
    phoenix_rebirth: {
      name: '涅槃重生',
      description: '死亡后复活，恢复50%生命',
      cooldown: 60,
      passive: true
    },
    phoenix_dive: {
      name: '俯冲攻击',
      description: '高速俯冲，造成250%范围伤害',
      cooldown: 6,
      damageMultiplier: 2.5,
      radius: 80
    },

    // ==================== 火龙技能 ====================
    fire_dragon_breath: {
      name: '烈焰吐息',
      description: '喷射烈焰，造成200%范围火焰伤害并燃烧',
      cooldown: 4,
      damageMultiplier: 2.0,
      radius: 120,
      burn: true,
      element: 'fire'
    },
    fire_dragon_claw: {
      name: '龙爪撕裂',
      description: '利爪撕裂，造成280%伤害',
      cooldown: 3,
      damageMultiplier: 2.8
    },
    fire_dragon_roar: {
      name: '火龙咆哮',
      description: '震慑周围敌人，眩晕3秒',
      cooldown: 15,
      stun: 3,
      radius: 150
    },
    inferno_burst: {
      name: '地狱烈焰',
      description: '大范围火焰爆发，造成350%伤害',
      cooldown: 12,
      damageMultiplier: 3.5,
      radius: 150,
      burn: true,
      element: 'fire'
    },
    meteor_shower: {
      name: '流星火雨',
      description: '召唤流星火雨，持续造成伤害',
      cooldown: 20,
      damageMultiplier: 4.0,
      radius: 200,
      burn: true,
      duration: 5
    },

    // ==================== 冰龙技能 ====================
    ice_dragon_breath: {
      name: '冰霜吐息',
      description: '喷射寒冰，造成180%范围冰霜伤害并减速',
      cooldown: 4,
      damageMultiplier: 1.8,
      radius: 120,
      slow: 0.5,
      element: 'ice'
    },
    frost_nova: {
      name: '霜冻新星',
      description: '释放冰霜新星，冻结周围敌人2秒',
      cooldown: 10,
      damageMultiplier: 1.5,
      radius: 130,
      freeze: 2
    },
    ice_dragon_claw: {
      name: '冰龙爪击',
      description: '寒冰利爪，造成250%伤害',
      cooldown: 3,
      damageMultiplier: 2.5
    },
    absolute_zero: {
      name: '绝对零度',
      description: '极寒爆发，造成300%伤害并冻结3秒',
      cooldown: 15,
      damageMultiplier: 3.0,
      radius: 160,
      freeze: 3,
      element: 'ice'
    },
    blizzard: {
      name: '暴风雪',
      description: '召唤暴风雪，持续减速并造成伤害',
      cooldown: 20,
      damageMultiplier: 3.5,
      radius: 200,
      slow: 0.7,
      duration: 6
    },

    // ==================== 雷龙技能 ====================
    thunder_dragon_breath: {
      name: '雷电吐息',
      description: '喷射雷电，造成220%范围雷电伤害，有几率眩晕',
      cooldown: 3,
      damageMultiplier: 2.2,
      radius: 110,
      stunChance: 0.2,
      element: 'thunder'
    },
    lightning_strike: {
      name: '闪电打击',
      description: '召唤闪电劈打敌人，造成300%伤害',
      cooldown: 4,
      damageMultiplier: 3.0,
      stunChance: 0.3
    },
    thunder_roar: {
      name: '雷龙怒吼',
      description: '雷电咆哮，麻痹周围敌人2秒',
      cooldown: 12,
      stun: 2,
      radius: 140
    },
    thunder_storm: {
      name: '雷暴',
      description: '召唤雷暴，随机闪电劈打多个敌人',
      cooldown: 15,
      damageMultiplier: 3.5,
      radius: 180,
      stunChance: 0.4,
      count: 5
    },
    divine_thunder: {
      name: '神雷',
      description: '召唤神圣雷霆，造成450%伤害',
      cooldown: 25,
      damageMultiplier: 4.5,
      radius: 200,
      stun: 2
    },

    // ==================== 麒麟技能 ====================
    qilin_heal: {
      name: '祥瑞治愈',
      description: '治愈主人30%最大生命',
      cooldown: 6,
      healPercent: 0.3
    },
    qilin_blessing: {
      name: '麒麟祝福',
      description: '提升主人全属性20%，持续15秒',
      cooldown: 20,
      buff: { stat: 'all', value: 1.2, duration: 15 }
    },
    qilin_revive: {
      name: '重生',
      description: '主人死亡时自动复活，恢复50%生命（每场战斗一次）',
      cooldown: 120,
      revive: true,
      revivePercent: 0.5
    },
    sacred_light: {
      name: '圣光普照',
      description: '释放圣光，治愈主人并伤害周围敌人',
      cooldown: 10,
      healPercent: 0.2,
      damageMultiplier: 2.0,
      radius: 120
    },

    // ==================== 独角兽技能 ====================
    unicorn_purify: {
      name: '净化',
      description: '净化主人身上所有debuff',
      cooldown: 8,
      purify: true
    },
    unicorn_heal: {
      name: '治愈光环',
      description: '持续治愈主人，每秒恢复5%生命，持续5秒',
      cooldown: 15,
      healOverTime: { percent: 0.05, duration: 5 }
    },
    holy_charge: {
      name: '神圣冲锋',
      description: '冲锋撞击敌人，造成200%伤害',
      cooldown: 5,
      damageMultiplier: 2.0,
      charge: true
    },

    // ==================== 飞马技能 ====================
    pegasus_purify: {
      name: '清风净化',
      description: '净化主人所有debuff并提升速度',
      cooldown: 7,
      purify: true,
      buff: { stat: 'speed', value: 1.3, duration: 5 }
    },
    wind_blade: {
      name: '风刃',
      description: '发射风刃，造成180%伤害',
      cooldown: 2,
      damageMultiplier: 1.8,
      projectile: true
    },
    sky_dive: {
      name: '俯冲轰炸',
      description: '从高空俯冲，造成280%范围伤害',
      cooldown: 8,
      damageMultiplier: 2.8,
      radius: 100
    }
  },

  // ==================== 工具方法 ====================

  getPet(petId) {
    return this[petId] || null;
  },

  getPetSkill(skillId) {
    return this.petSkills[skillId] || null;
  },

  createPet(petId, level = 1) {
    const petDef = this.getPet(petId);
    if (!petDef) return null;

    const pet = {
      id: petId,
      name: petDef.name,
      icon: petDef.icon,
      type: petDef.type,
      rarity: petDef.rarity,
      level,
      exp: 0,
      expToNext: level * 50,
      maxHp: Math.floor(petDef.baseStats.hp + petDef.growth.hp * (level - 1)),
      hp: 0,
      damage: Math.floor(petDef.baseStats.damage + petDef.growth.damage * (level - 1)),
      armor: Math.floor(petDef.baseStats.armor + petDef.growth.armor * (level - 1)),
      speed: petDef.baseStats.speed + petDef.growth.speed * (level - 1),
      attackSpeed: petDef.baseStats.attackSpeed,
      skills: [...petDef.skills],
      ability: petDef.ability,
      skillCooldowns: {},
      x: 0,
      y: 0,
      targetX: 0,
      targetY: 0,
      attackCooldown: 0,
      alive: true,
      rebirthAvailable: petId === 'phoenix_pet',
      owner: null
    };
    pet.hp = pet.maxHp;
    return pet;
  },

  // 获取所有可获得的宠物
  getAllPets() {
    return Object.keys(this).filter(k => this[k] && this[k].id && this[k].baseStats);
  },

  // 根据稀有度获取随机宠物
  getRandomPet(minRarity = 'common') {
    const rarityOrder = ['common', 'uncommon', 'rare', 'epic', 'legendary', 'mythic'];
    const minIndex = rarityOrder.indexOf(minRarity);
    const pets = this.getAllPets().filter(id => {
      const pet = this.getPet(id);
      return rarityOrder.indexOf(pet.rarity) >= minIndex;
    });
    return Utils.randomChoice(pets);
  }
};

// ==================== 宠物管理器 ====================
const PetManager = {
  activePet: null,
  ownedPets: [],
  petInventory: [],

  // 抽卡系统
  gachaPoints: 0,
  gachaHistory: [],
  pityCounter: 0, // 保底计数
  pityThreshold: 90, // 90抽保底传说

  // 坐骑系统
  activeMount: null,
  isMounted: false,
  mountSpeedBonus: 0,

  init() {
    this.activePet = null;
    this.ownedPets = [];
    this.petInventory = [];
    this.gachaPoints = 0;
    this.gachaHistory = [];
    this.pityCounter = 0;
    this.activeMount = null;
    this.isMounted = false;
    this.mountSpeedBonus = 0;
    console.log('[PetManager] 宠物系统初始化完成');
  },

  // ==================== 抽卡系统 ====================

  // 单抽
  singlePull(game) {
    const cost = 100; // 金币
    if (game.player.gold < cost) {
      return { success: false, message: '金币不足，需要100金币' };
    }

    game.player.gold -= cost;
    this.pityCounter++;
    this.gachaPoints += 10;

    // 稀有度概率
    const roll = Math.random();
    let rarity;
    if (this.pityCounter >= this.pityThreshold || roll < 0.02) {
      rarity = 'mythic';
      this.pityCounter = 0;
    } else if (roll < 0.08) {
      rarity = 'legendary';
    } else if (roll < 0.20) {
      rarity = 'epic';
    } else if (roll < 0.45) {
      rarity = 'rare';
    } else if (roll < 0.75) {
      rarity = 'uncommon';
    } else {
      rarity = 'common';
    }

    // 根据稀有度获取宠物
    const pets = PetData.getAllPets().filter(id => {
      const pet = PetData.getPet(id);
      return pet && pet.rarity === rarity && !pet.isEvolved;
    });

    if (pets.length === 0) {
      // 降级查找
      const allPets = PetData.getAllPets().filter(id => {
        const pet = PetData.getPet(id);
        return pet && !pet.isEvolved;
      });
      const petId = Utils.randomChoice(allPets);
      return this._grantPet(petId, game);
    }

    const petId = Utils.randomChoice(pets);
    return this._grantPet(petId, game);
  },

  // 十连抽
  tenPull(game) {
    const cost = 900; // 九折
    if (game.player.gold < cost) {
      return { success: false, message: '金币不足，需要900金币' };
    }

    game.player.gold -= cost;
    const results = [];
    let hasRareOrAbove = false;

    for (let i = 0; i < 10; i++) {
      this.pityCounter++;
      this.gachaPoints += 10;

      const roll = Math.random();
      let rarity;

      // 十连保底至少一个稀有
      if (i === 9 && !hasRareOrAbove) {
        rarity = 'rare';
      } else if (this.pityCounter >= this.pityThreshold || roll < 0.02) {
        rarity = 'mythic';
        this.pityCounter = 0;
      } else if (roll < 0.08) {
        rarity = 'legendary';
      } else if (roll < 0.20) {
        rarity = 'epic';
      } else if (roll < 0.45) {
        rarity = 'rare';
      } else if (roll < 0.75) {
        rarity = 'uncommon';
      } else {
        rarity = 'common';
      }

      if (['rare', 'epic', 'legendary', 'mythic'].includes(rarity)) {
        hasRareOrAbove = true;
      }

      const pets = PetData.getAllPets().filter(id => {
        const pet = PetData.getPet(id);
        return pet && pet.rarity === rarity && !pet.isEvolved;
      });

      const petId = pets.length > 0 ? Utils.randomChoice(pets) : 'slime_pet';
      const result = this._grantPet(petId, game, false);
      results.push(result);
    }

    game.showMessage(`十连抽完成！获得 ${results.filter(r => r.isNew).length} 只新宠物`);
    return { success: true, results, isMulti: true };
  },

  // 发放宠物
  _grantPet(petId, game, showMessage = true) {
    const petDef = PetData.getPet(petId);
    const isNew = !this.ownedPets.includes(petId);

    if (isNew) {
      this.ownedPets.push(petId);
    }

    // 记录历史
    this.gachaHistory.unshift({
      petId,
      name: petDef?.name || petId,
      rarity: petDef?.rarity || 'common',
      time: Date.now(),
      isNew
    });
    if (this.gachaHistory.length > 50) {
      this.gachaHistory.pop();
    }

    if (showMessage) {
      const rarityColors = {
        common: '#fff', uncommon: '#2ecc71', rare: '#3498db',
        epic: '#9b59b6', legendary: '#f1c40f', mythic: '#e74c3c'
      };
      game.showMessage(`获得宠物: ${petDef?.icon || ''} ${petDef?.name || petId} (${petDef?.rarity || ''})${isNew ? ' 【新】' : ''}`);
      AudioSystem.playSound('levelup');
      ParticleSystem.magic(game.player.x, game.player.y, rarityColors[petDef?.rarity] || '#fff', 20);
    }

    return { success: true, petId, isNew, pet: petDef };
  },

  // BOSS掉落宠物
  bossDropPet(bossType, game) {
    // BOSS有几率掉落神话宠物
    const dropChance = 0.1; // 10%几率
    if (Math.random() > dropChance) return null;

    const bossPetMap = {
      boss_dragon: ['fire_dragon_pet', 'ice_dragon_pet', 'thunder_dragon_pet'],
      boss_demon_lord: ['fire_dragon_pet', 'qilin_pet'],
      boss_lich: ['ice_dragon_pet', 'unicorn_pet'],
      boss_final: ['fire_dragon_pet', 'ice_dragon_pet', 'thunder_dragon_pet', 'qilin_pet', 'phoenix_pet', 'unicorn_pet', 'pegasus_pet']
    };

    const possiblePets = bossPetMap[bossType] || ['fire_dragon_pet'];
    const petId = Utils.randomChoice(possiblePets);
    return this._grantPet(petId, game);
  },

  // ==================== 坐骑系统 ====================

  // 骑乘坐骑
  mount(petId) {
    const petDef = PetData.getPet(petId);
    if (!petDef || !petDef.isMount) {
      return { success: false, message: '该宠物不能作为坐骑' };
    }

    if (!this.ownedPets.includes(petId)) {
      return { success: false, message: '你还没有这个宠物' };
    }

    this.activeMount = petId;
    this.isMounted = true;
    this.mountSpeedBonus = petDef.mountSpeedBonus || 0;

    // 应用速度加成
    if (window.Game?.player) {
      window.Game.player.calculateStats();
    }

    AudioSystem.playSound('success');
    return { success: true, message: `骑乘了 ${petDef.name}` };
  },

  // 下坐骑
  dismount() {
    if (!this.isMounted) return;

    this.isMounted = false;
    this.mountSpeedBonus = 0;

    if (window.Game?.player) {
      window.Game.player.calculateStats();
    }
  },

  // 切换骑乘状态
  toggleMount() {
    if (this.isMounted) {
      this.dismount();
    } else if (this.activeMount) {
      this.mount(this.activeMount);
    }
  },

  // 获取坐骑速度加成
  getMountSpeedBonus() {
    return this.isMounted ? this.mountSpeedBonus : 0;
  },

  // 净化debuff（坐骑能力）
  purifyDebuffs(player) {
    if (!this.isMounted || !this.activeMount) return false;

    const petDef = PetData.getPet(this.activeMount);
    if (!petDef || !petDef.skills?.some(s => s.includes('purify'))) return false;

    // 移除所有debuff
    if (player.buffs) {
      player.buffs = player.buffs.filter(b => !b.isDebuff);
    }

    ParticleSystem.magic(player.x, player.y, '#fff', 15);
    AudioSystem.playSound('heal');
    return true;
  },

  // 召唤宠物
  summonPet(petId, level = 1) {
    const pet = PetData.createPet(petId, level);
    if (!pet) return null;

    this.activePet = pet;
    pet.owner = window.Game?.player;

    if (window.Game?.player) {
      pet.x = window.Game.player.x + 30;
      pet.y = window.Game.player.y;
    }

    AudioSystem.playSound('summon');
    ParticleSystem.magic(pet.x, pet.y, '#9b59b6', 15);

    console.log(`[Pet] 召唤宠物: ${pet.name}`);
    return pet;
  },

  // 收回宠物
  dismissPet() {
    if (this.activePet) {
      ParticleSystem.magic(this.activePet.x, this.activePet.y, '#9b59b6', 10);
      console.log(`[Pet] 收回宠物: ${this.activePet.name}`);
    }
    this.activePet = null;
  },

  // 更新宠物
  update(dt, game) {
    if (!this.activePet || !this.activePet.alive) return;

    const pet = this.activePet;
    const player = game.player;
    if (!player) return;

    // 更新技能冷却
    for (const skillId in pet.skillCooldowns) {
      if (pet.skillCooldowns[skillId] > 0) {
        pet.skillCooldowns[skillId] -= dt;
      }
    }

    // 攻击冷却
    if (pet.attackCooldown > 0) {
      pet.attackCooldown -= dt;
    }

    // 查找最近敌人
    let nearestEnemy = null;
    let minDist = Infinity;
    for (const monster of game.monsters) {
      if (monster.dead) continue;
      const dist = Utils.distance(pet.x, pet.y, monster.x, monster.y);
      if (dist < minDist && dist < 300) {
        minDist = dist;
        nearestEnemy = monster;
      }
    }

    if (nearestEnemy) {
      // 战斗状态
      if (minDist > 40) {
        // 移动向敌人
        const dx = nearestEnemy.x - pet.x;
        const dy = nearestEnemy.y - pet.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        pet.x += (dx / dist) * pet.speed * dt * 60;
        pet.y += (dy / dist) * pet.speed * dt * 60;
      } else if (pet.attackCooldown <= 0) {
        // 攻击
        this.performPetAttack(pet, nearestEnemy, game);
        pet.attackCooldown = 1.0 / pet.attackSpeed;
      }

      // 使用技能
      this.tryPetSkill(pet, nearestEnemy, game);
    } else {
      // 跟随主人
      const distToOwner = Utils.distance(pet.x, pet.y, player.x, player.y);
      if (distToOwner > 80) {
        const dx = player.x - pet.x;
        const dy = player.y - pet.y;
        const dist = Math.sqrt(dx * dx + dy * dy);
        pet.x += (dx / dist) * pet.speed * dt * 60;
        pet.y += (dy / dist) * pet.speed * dt * 60;
      }

      // 治疗光环（小精灵）
      if (pet.id === 'fairy_pet' || pet.id === 'spirit_pet') {
        if (Math.random() < dt * 0.5) {
          player.hp = Math.min(player.maxHp, player.hp + 2);
        }
      }
    }

    // 边界限制
    pet.x = Utils.clamp(pet.x, 20, game.mapWidth - 20);
    pet.y = Utils.clamp(pet.y, 20, game.mapHeight - 20);
  },

  // 宠物攻击
  performPetAttack(pet, target, game) {
    const damage = pet.damage;
    target.takeDamage(damage, pet);
    ParticleSystem.damageNumber(target.x, target.y, damage);
    AudioSystem.playSound('attack');
  },

  // 尝试使用技能
  tryPetSkill(pet, target, game) {
    for (const skillId of pet.skills) {
      const skill = PetData.getPetSkill(skillId);
      if (!skill) continue;
      if (pet.skillCooldowns[skillId] > 0) continue;

      // 随机使用技能
      if (Math.random() < 0.02) {
        this.usePetSkill(pet, skillId, target, game);
        pet.skillCooldowns[skillId] = skill.cooldown || 5;
        break;
      }
    }
  },

  // 使用宠物技能
  usePetSkill(pet, skillId, target, game) {
    const skill = PetData.getPetSkill(skillId);
    if (!skill) return;

    AudioSystem.playSound('spell');

    if (skill.damageMultiplier) {
      const damage = Math.floor(pet.damage * skill.damageMultiplier);
      if (skill.radius) {
        // 范围伤害
        for (const monster of game.monsters) {
          if (monster.dead) continue;
          const dist = Utils.distance(target.x, target.y, monster.x, monster.y);
          if (dist < skill.radius) {
            monster.takeDamage(damage, pet);
            ParticleSystem.damageNumber(monster.x, monster.y, damage);
          }
        }
        ParticleSystem.explosion(target.x, target.y, '#e67e22', skill.radius, 15);
      } else if (skill.projectile) {
        // 投射物
        game.createProjectile({
          x: pet.x,
          y: pet.y,
          targetX: target.x,
          targetY: target.y,
          speed: 8,
          damage,
          owner: pet,
          color: '#9b59b6'
        });
      } else {
        // 单体伤害
        target.takeDamage(damage, pet);
        ParticleSystem.damageNumber(target.x, target.y, damage);
      }
    }

    if (skill.healPercent && game.player) {
      const heal = Math.floor(game.player.maxHp * skill.healPercent);
      game.player.hp = Math.min(game.player.maxHp, game.player.hp + heal);
      ParticleSystem.heal(game.player.x, game.player.y);
    }

    if (skill.shield && game.player) {
      game.player.shield = (game.player.shield || 0) + skill.shield;
      ParticleSystem.magic(game.player.x, game.player.y, '#3498db');
    }

    if (skill.stun) {
      for (const monster of game.monsters) {
        if (monster.dead) continue;
        const dist = Utils.distance(pet.x, pet.y, monster.x, monster.y);
        if (dist < (skill.radius || 100)) {
          BuffSystem.applyBuff(monster, 'stun', null, skill.stun);
        }
      }
      ParticleSystem.explosion(pet.x, pet.y, '#f1c40f', 100, 10);
    }

    if (skill.buff && game.player) {
      BuffSystem.applyBuff(game.player, 'damageBoost', null, skill.buff.duration);
    }
  },

  // 宠物受伤
  petTakeDamage(pet, damage, source) {
    if (!pet || !pet.alive) return;

    pet.hp -= damage;
    ParticleSystem.damageNumber(pet.x, pet.y, damage);

    if (pet.hp <= 0) {
      // 凤凰复活
      if (pet.rebirthAvailable) {
        pet.rebirthAvailable = false;
        pet.hp = Math.floor(pet.maxHp * 0.5);
        ParticleSystem.levelUp(pet.x, pet.y);
        game.showMessage(`${pet.name} 涅槃重生！`);
      } else {
        pet.alive = false;
        ParticleSystem.explosion(pet.x, pet.y, '#9b59b6', 40, 10);
        game.showMessage(`${pet.name} 倒下了...`);
        setTimeout(() => { this.activePet = null; }, 1000);
      }
    }
  },

  // 宠物获得经验
  gainPetExp(pet, amount) {
    if (!pet) return;
    pet.exp += amount;

    while (pet.exp >= pet.expToNext) {
      pet.exp -= pet.expToNext;
      pet.level++;
      pet.expToNext = pet.level * 50;

      // 属性提升
      const petDef = PetData.getPet(pet.id);
      pet.maxHp = Math.floor(petDef.baseStats.hp + petDef.growth.hp * (pet.level - 1));
      pet.damage = Math.floor(petDef.baseStats.damage + petDef.growth.damage * (pet.level - 1));
      pet.armor = Math.floor(petDef.baseStats.armor + petDef.growth.armor * (pet.level - 1));
      pet.hp = pet.maxHp;

      ParticleSystem.levelUp(pet.x, pet.y);
      game.showMessage(`${pet.name} 升级到 ${pet.level} 级！`);

      // 检查进化
      if (petDef.evolveTo && pet.level >= petDef.evolveLevel) {
        this.evolvePet(pet);
      }
    }
  },

  // 宠物进化
  evolvePet(pet) {
    const petDef = PetData.getPet(pet.id);
    if (!petDef.evolveTo) return;

    const evolvedDef = PetData.getPet(petDef.evolveTo);
    if (!evolvedDef) return;

    pet.id = evolvedDef.id;
    pet.name = evolvedDef.name;
    pet.icon = evolvedDef.icon;
    pet.rarity = evolvedDef.rarity;
    pet.skills = [...evolvedDef.skills];
    pet.ability = evolvedDef.ability;

    // 重新计算属性
    pet.maxHp = Math.floor(evolvedDef.baseStats.hp + evolvedDef.growth.hp * (pet.level - 1));
    pet.damage = Math.floor(evolvedDef.baseStats.damage + evolvedDef.growth.damage * (pet.level - 1));
    pet.armor = Math.floor(evolvedDef.baseStats.armor + evolvedDef.growth.armor * (pet.level - 1));
    pet.hp = pet.maxHp;

    ParticleSystem.explosion(pet.x, pet.y, '#f1c40f', 80, 25);
    AudioSystem.playSound('levelup');
    game.showMessage(`${pet.name} 进化了！`);
  },

  // 渲染抽卡界面
  renderGachaUI(ctx, game) {
    const x = 100;
    const y = 60;
    const width = game.canvas.width - 200;
    const height = game.canvas.height - 120;

    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#9b59b6';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    ctx.fillStyle = '#9b59b6';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🎰 宠物召唤', x + width / 2, y + 40);

    // 保底进度
    ctx.fillStyle = '#fff';
    ctx.font = '16px Arial';
    ctx.fillText(`保底进度: ${this.pityCounter}/${this.pityThreshold}`, x + width / 2, y + 70);
    ctx.fillText(`召唤点数: ${this.gachaPoints}`, x + width / 2, y + 95);

    // 概率说明
    ctx.fillStyle = '#aaa';
    ctx.font = '12px Arial';
    ctx.fillText('神话2% 传说6% 史诗12% 稀有25% 优秀30% 普通25%', x + width / 2, y + 120);

    // 单抽按钮
    ctx.fillStyle = '#3498db';
    ctx.fillRect(x + width / 2 - 200, y + 160, 180, 60);
    ctx.fillStyle = '#fff';
    ctx.font = 'bold 20px Arial';
    ctx.fillText('单抽 (100金币)', x + width / 2 - 110, y + 198);

    // 十连抽按钮
    ctx.fillStyle = '#9b59b6';
    ctx.fillRect(x + width / 2 + 20, y + 160, 180, 60);
    ctx.fillStyle = '#fff';
    ctx.fillText('十连抽 (900金币)', x + width / 2 + 110, y + 198);

    // 最近获得
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 16px Arial';
    ctx.textAlign = 'left';
    ctx.fillText('最近获得:', x + 30, y + 250);

    const rarityColors = {
      common: '#fff', uncommon: '#2ecc71', rare: '#3498db',
      epic: '#9b59b6', legendary: '#f1c40f', mythic: '#e74c3c'
    };

    this.gachaHistory.slice(0, 10).forEach((record, i) => {
      const ry = y + 280 + i * 25;
      ctx.fillStyle = rarityColors[record.rarity] || '#fff';
      ctx.font = '14px Arial';
      ctx.fillText(`${record.name} [${record.rarity}]${record.isNew ? ' 【新】' : ''}`, x + 40, ry);
    });

    // 神话生物展示
    ctx.fillStyle = '#e74c3c';
    ctx.font = 'bold 16px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('⭐ 神话生物 ⭐', x + width - 150, y + 250);

    const mythicPets = ['fire_dragon_pet', 'ice_dragon_pet', 'thunder_dragon_pet', 'qilin_pet', 'unicorn_pet', 'pegasus_pet', 'phoenix_pet'];
    mythicPets.forEach((petId, i) => {
      const pet = PetData.getPet(petId);
      const owned = this.ownedPets.includes(petId);
      const mx = x + width - 220 + (i % 2) * 100;
      const my = y + 280 + Math.floor(i / 2) * 50;

      ctx.globalAlpha = owned ? 1 : 0.3;
      ctx.font = '28px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(pet?.icon || '?', mx, my + 20);
      ctx.font = '11px Arial';
      ctx.fillStyle = owned ? '#fff' : '#666';
      ctx.fillText(pet?.name || '???', mx, my + 38);
      ctx.globalAlpha = 1;
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 ESC 关闭 | 点击按钮抽取', x + width / 2, y + height - 25);
  },

  // 渲染宠物图鉴
  renderPetCollection(ctx, game) {
    const x = 80;
    const y = 60;
    const width = game.canvas.width - 160;
    const height = game.canvas.height - 120;

    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#f1c40f';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('📖 宠物图鉴', x + width / 2, y + 40);

    const allPets = PetData.getAllPets().filter(id => {
      const pet = PetData.getPet(id);
      return pet && !pet.isEvolved;
    });
    const ownedCount = allPets.filter(id => this.ownedPets.includes(id)).length;

    ctx.fillStyle = '#fff';
    ctx.font = '16px Arial';
    ctx.fillText(`收集进度: ${ownedCount}/${allPets.length}`, x + width / 2, y + 70);

    // 宠物网格
    const cols = 6;
    const cellW = 120;
    const cellH = 100;
    const startX = x + 40;
    const startY = y + 100;

    allPets.forEach((petId, i) => {
      const pet = PetData.getPet(petId);
      const owned = this.ownedPets.includes(petId);
      const col = i % cols;
      const row = Math.floor(i / cols);
      const cx = startX + col * cellW;
      const cy = startY + row * cellH;

      if (cy > y + height - 60) return;

      ctx.fillStyle = owned ? 'rgba(241, 196, 15, 0.1)' : 'rgba(0,0,0,0.3)';
      ctx.fillRect(cx, cy, cellW - 10, cellH - 10);
      ctx.strokeStyle = owned ? '#f1c40f' : '#444';
      ctx.strokeRect(cx, cy, cellW - 10, cellH - 10);

      ctx.globalAlpha = owned ? 1 : 0.3;
      ctx.font = '32px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(owned ? pet.icon : '?', cx + (cellW - 10) / 2, cy + 35);

      ctx.fillStyle = owned ? '#fff' : '#666';
      ctx.font = '12px Arial';
      ctx.fillText(owned ? pet.name : '???', cx + (cellW - 10) / 2, cy + 58);

      if (owned) {
        const rarityColors = {
          common: '#fff', uncommon: '#2ecc71', rare: '#3498db',
          epic: '#9b59b6', legendary: '#f1c40f', mythic: '#e74c3c'
        };
        ctx.fillStyle = rarityColors[pet.rarity] || '#fff';
        ctx.font = '10px Arial';
        ctx.fillText(pet.rarity, cx + (cellW - 10) / 2, cy + 75);

        if (pet.isMount) {
          ctx.fillStyle = '#3498db';
          ctx.fillText('🐴坐骑', cx + (cellW - 10) / 2, cy + 88);
        }
      }
      ctx.globalAlpha = 1;
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 ESC 关闭', x + width / 2, y + height - 25);
  },

  // 处理抽卡界面点击
  handleGachaClick(mouseX, mouseY, game) {
    const x = 100;
    const y = 60;
    const width = game.canvas.width - 200;

    // 单抽按钮
    if (mouseX > x + width / 2 - 200 && mouseX < x + width / 2 - 20 &&
        mouseY > y + 160 && mouseY < y + 220) {
      return this.singlePull(game);
    }

    // 十连抽按钮
    if (mouseX > x + width / 2 + 20 && mouseX < x + width / 2 + 200 &&
        mouseY > y + 160 && mouseY < y + 220) {
      return this.tenPull(game);
    }

    return null;
  },

  // 渲染宠物
  render(ctx, camera) {
    if (!this.activePet || !this.activePet.alive) return;

    const pet = this.activePet;
    const screenX = pet.x - camera.x;
    const screenY = pet.y - camera.y;

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(screenX, screenY + 12, 10, 4, 0, 0, Math.PI * 2);
    ctx.fill();

    // 宠物图标
    ctx.font = '24px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(pet.icon, screenX, screenY);

    // 血条
    const barWidth = 30;
    const barHeight = 3;
    ctx.fillStyle = '#333';
    ctx.fillRect(screenX - barWidth / 2, screenY - 18, barWidth, barHeight);
    ctx.fillStyle = '#2ecc71';
    ctx.fillRect(screenX - barWidth / 2, screenY - 18, barWidth * (pet.hp / pet.maxHp), barHeight);

    // 名字和等级
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 10px Arial';
    ctx.fillText(`${pet.name} Lv.${pet.level}`, screenX, screenY - 24);
  },

  // 保存
  save() {
    return {
      activePet: this.activePet ? { id: this.activePet.id, level: this.activePet.level } : null,
      ownedPets: this.ownedPets,
      gachaPoints: this.gachaPoints,
      pityCounter: this.pityCounter,
      gachaHistory: this.gachaHistory.slice(0, 20),
      activeMount: this.activeMount,
      isMounted: this.isMounted
    };
  },

  // 加载
  load(data) {
    if (!data) return;
    if (data.activePet) {
      this.summonPet(data.activePet.id, data.activePet.level);
    }
    this.ownedPets = data.ownedPets || [];
    this.gachaPoints = data.gachaPoints || 0;
    this.pityCounter = data.pityCounter || 0;
    this.gachaHistory = data.gachaHistory || [];
    this.activeMount = data.activeMount || null;
    this.isMounted = data.isMounted || false;
    if (this.activeMount) {
      const petDef = PetData.getPet(this.activeMount);
      this.mountSpeedBonus = petDef?.mountSpeedBonus || 0;
    }
  }
};

window.PetData = PetData;
window.PetManager = PetManager;
