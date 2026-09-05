// ==================== 永恒地牢 - 职业数据配置 ====================
// 定义6个职业的基础属性、成长率、技能树

const ClassData = {
  // ==================== 战士 ====================
  warrior: {
    name: '战士',
    icon: '🗡️',
    description: '高血量高防御的近战勇士，擅长冲锋陷阵，是团队的坚实后盾。',
    difficulty: '简单',
    baseStats: {
      hp: 120,
      mp: 30,
      stamina: 100,
      str: 18,
      dex: 10,
      int: 6,
      vit: 16,
      luck: 8
    },
    growthStats: {
      hp: 12,
      mp: 3,
      stamina: 5,
      str: 3,
      dex: 1,
      int: 0.5,
      vit: 2.5,
      luck: 0.5
    },
    derivedStats: {
      damage: 15,
      attackSpeed: 1.0,
      attackRange: 50,
      critChance: 0.1,
      critDamage: 1.5,
      dodge: 0.05,
      armor: 8,
      magicResist: 5,
      moveSpeed: 3.5,
      hpRegen: 2,
      mpRegen: 1
    },
    skills: [
      {
        id: 'warrior_charge',
        name: '冲锋',
        icon: '⚡',
        key: 'Q',
        cooldown: 5,
        manaCost: 10,
        description: '向鼠标方向冲锋150距离，对路径上的敌人造成150%伤害并击退。',
        type: 'active',
        effects: {
          damage: 1.5,
          range: 150,
          knockback: 50
        }
      },
      {
        id: 'warrior_shield_bash',
        name: '盾击',
        icon: '🛡️',
        key: 'W',
        cooldown: 8,
        manaCost: 15,
        description: '用盾牌猛击周围敌人，造成80%伤害并眩晕2秒。',
        type: 'active',
        effects: {
          damage: 0.8,
          radius: 100,
          stun: 2
        }
      },
      {
        id: 'warrior_battle_cry',
        name: '战吼',
        icon: '📢',
        key: 'E',
        cooldown: 15,
        manaCost: 20,
        description: '发出战吼，提升50%攻击力和20%移动速度，持续10秒。',
        type: 'buff',
        effects: {
          damageBoost: 1.5,
          speedBoost: 1.2,
          duration: 10
        }
      },
      {
        id: 'warrior_whirlwind',
        name: '旋风斩',
        icon: '🌀',
        key: 'R',
        cooldown: 12,
        manaCost: 25,
        description: '旋转武器攻击周围所有敌人，造成200%伤害。',
        type: 'active',
        effects: {
          damage: 2.0,
          radius: 80
        }
      },
      {
        id: 'warrior_berserker',
        name: '狂暴',
        icon: '😤',
        key: 'F',
        cooldown: 60,
        manaCost: 40,
        description: '进入狂暴状态，攻击力+100%，攻击速度+50%，但受到伤害+30%，持续15秒。',
        type: 'ultimate',
        effects: {
          damageBoost: 2.0,
          attackSpeedBoost: 1.5,
          damageTaken: 1.3,
          duration: 15
        }
      }
    ],
    talentTree: {
      name: '战士天赋',
      branches: [
        {
          name: '武器大师',
          talents: [
            { id: 'w_t1', name: '强力挥砍', desc: '普通攻击伤害+10%', maxLevel: 5, effect: 'damage', value: 0.1 },
            { id: 'w_t2', name: '致命一击', desc: '暴击率+5%', maxLevel: 5, effect: 'crit', value: 0.05 },
            { id: 'w_t3', name: '破甲', desc: '攻击无视10%护甲', maxLevel: 3, effect: 'armorPen', value: 0.1 },
            { id: 'w_t4', name: '武器精通', desc: '所有技能伤害+15%', maxLevel: 3, effect: 'skillDamage', value: 0.15 }
          ]
        },
        {
          name: '坚韧',
          talents: [
            { id: 'w_t5', name: '强壮体魄', desc: '最大生命+50', maxLevel: 5, effect: 'maxHp', value: 50 },
            { id: 'w_t6', name: '铁皮', desc: '护甲+5', maxLevel: 5, effect: 'armor', value: 5 },
            { id: 'w_t7', name: '生命恢复', desc: '生命恢复+2/秒', maxLevel: 3, effect: 'hpRegen', value: 2 },
            { id: 'w_t8', name: '不屈意志', desc: '生命低于30%时减伤30%', maxLevel: 1, effect: 'lowHpDamageReduction', value: 0.3 }
          ]
        },
        {
          name: '战术',
          talents: [
            { id: 'w_t9', name: '迅捷冲锋', desc: '冲锋冷却-1秒', maxLevel: 3, effect: 'cooldown', value: 1, skill: 'warrior_charge' },
            { id: 'w_t10', name: '震慑', desc: '盾击眩晕时间+0.5秒', maxLevel: 3, effect: 'stunDuration', value: 0.5 },
            { id: 'w_t11', name: '战斗本能', desc: '闪避率+3%', maxLevel: 5, effect: 'dodge', value: 0.03 },
            { id: 'w_t12', name: '战神', desc: '狂暴持续时间+5秒', maxLevel: 2, effect: 'duration', value: 5, skill: 'warrior_berserker' }
          ]
        }
      ]
    }
  },

  // ==================== 法师 ====================
  mage: {
    name: '法师',
    icon: '🔮',
    description: '掌控元素之力的施法者，拥有强大的远程魔法输出，但身板脆弱。',
    difficulty: '中等',
    baseStats: {
      hp: 70,
      mp: 100,
      stamina: 80,
      str: 6,
      dex: 10,
      int: 20,
      vit: 8,
      luck: 12
    },
    growthStats: {
      hp: 6,
      mp: 10,
      stamina: 3,
      str: 0.5,
      dex: 1,
      int: 4,
      vit: 1,
      luck: 1
    },
    derivedStats: {
      damage: 12,
      attackSpeed: 0.8,
      attackRange: 300,
      critChance: 0.15,
      critDamage: 2.0,
      dodge: 0.08,
      armor: 3,
      magicResist: 15,
      moveSpeed: 3.2,
      hpRegen: 1,
      mpRegen: 4
    },
    skills: [
      {
        id: 'mage_fireball',
        name: '火球术',
        icon: '🔥',
        key: 'Q',
        cooldown: 2,
        manaCost: 8,
        description: '发射一颗火球，命中后爆炸造成范围伤害并点燃敌人。',
        type: 'active',
        effects: {
          damage: 2.0,
          radius: 60,
          burn: 3
        }
      },
      {
        id: 'mage_ice_shield',
        name: '冰盾',
        icon: '🧊',
        key: 'W',
        cooldown: 10,
        manaCost: 20,
        description: '召唤冰盾吸收相当于最大生命50%的伤害，持续8秒。',
        type: 'buff',
        effects: {
          shieldPercent: 0.5,
          duration: 8
        }
      },
      {
        id: 'mage_blink',
        name: '闪现',
        icon: '✨',
        key: 'E',
        cooldown: 6,
        manaCost: 15,
        description: '瞬间传送到鼠标位置。',
        type: 'active',
        effects: {
          range: 200
        }
      },
      {
        id: 'mage_meteor',
        name: '陨石术',
        icon: '☄️',
        key: 'R',
        cooldown: 20,
        manaCost: 40,
        description: '召唤陨石轰炸目标区域，造成300%范围伤害。',
        type: 'active',
        effects: {
          damage: 3.0,
          radius: 120,
          delay: 0.5
        }
      },
      {
        id: 'mage_arcane_storm',
        name: '奥术风暴',
        icon: '🌪️',
        key: 'F',
        cooldown: 60,
        manaCost: 60,
        description: '在周围召唤奥术风暴，持续5秒，每秒对周围敌人造成100%伤害。',
        type: 'ultimate',
        effects: {
          damagePerSec: 1.0,
          radius: 150,
          duration: 5
        }
      }
    ],
    talentTree: {
      name: '法师天赋',
      branches: [
        {
          name: '火焰',
          talents: [
            { id: 'm_t1', name: '烈焰', desc: '火球伤害+15%', maxLevel: 5, effect: 'skillDamage', value: 0.15, skill: 'mage_fireball' },
            { id: 'm_t2', name: '焚烧', desc: '点燃持续时间+1秒', maxLevel: 3, effect: 'burnDuration', value: 1 },
            { id: 'm_t3', name: '火焰精通', desc: '所有火焰伤害+20%', maxLevel: 3, effect: 'fireDamage', value: 0.2 },
            { id: 'm_t4', name: '陨石强化', desc: '陨石伤害+30%', maxLevel: 3, effect: 'skillDamage', value: 0.3, skill: 'mage_meteor' }
          ]
        },
        {
          name: '冰霜',
          talents: [
            { id: 'm_t5', name: '寒冰护盾', desc: '冰盾吸收量+10%', maxLevel: 5, effect: 'shield', value: 0.1 },
            { id: 'm_t6', name: '冰霜新星', desc: '冰盾破碎时冻结周围敌人1秒', maxLevel: 3, effect: 'freeze', value: 1 },
            { id: 'm_t7', name: '冰冷', desc: '攻击有20%几率减速敌人', maxLevel: 3, effect: 'slowChance', value: 0.2 },
            { id: 'm_t8', name: '绝对零度', desc: '减速效果+50%', maxLevel: 2, effect: 'slowPower', value: 0.5 }
          ]
        },
        {
          name: '奥术',
          talents: [
            { id: 'm_t9', name: '魔力涌动', desc: '最大魔法+30', maxLevel: 5, effect: 'maxMp', value: 30 },
            { id: 'm_t10', name: '冥想', desc: '魔法恢复+1/秒', maxLevel: 5, effect: 'mpRegen', value: 1 },
            { id: 'm_t11', name: '法术暴击', desc: '暴击伤害+20%', maxLevel: 5, effect: 'critDamage', value: 0.2 },
            { id: 'm_t12', name: '时空领主', desc: '闪现冷却-2秒', maxLevel: 2, effect: 'cooldown', value: 2, skill: 'mage_blink' }
          ]
        }
      ]
    }
  },

  // ==================== 游侠 ====================
  ranger: {
    name: '游侠',
    icon: '🏹',
    description: '敏捷致命的远程射手，擅长多重射击和陷阱，机动性极强。',
    difficulty: '中等',
    baseStats: {
      hp: 90,
      mp: 60,
      stamina: 120,
      str: 12,
      dex: 20,
      int: 8,
      vit: 12,
      luck: 15
    },
    growthStats: {
      hp: 8,
      mp: 5,
      stamina: 8,
      str: 1.5,
      dex: 3.5,
      int: 1,
      vit: 1.5,
      luck: 1.5
    },
    derivedStats: {
      damage: 14,
      attackSpeed: 1.2,
      attackRange: 350,
      critChance: 0.25,
      critDamage: 1.8,
      dodge: 0.15,
      armor: 5,
      magicResist: 8,
      moveSpeed: 4.0,
      hpRegen: 1.5,
      mpRegen: 2
    },
    skills: [
      {
        id: 'ranger_multishot',
        name: '多重射击',
        icon: '🏹',
        key: 'Q',
        cooldown: 4,
        manaCost: 12,
        description: '同时射出5支箭，每支造成80%伤害。',
        type: 'active',
        effects: {
          damage: 0.8,
          arrows: 5,
          spread: 0.6
        }
      },
      {
        id: 'ranger_dodge',
        name: '闪避',
        icon: '💨',
        key: 'W',
        cooldown: 8,
        manaCost: 10,
        description: '进入闪避状态，下次攻击必定闪避，持续3秒。',
        type: 'buff',
        effects: {
          dodgeBoost: 1,
          duration: 3
        }
      },
      {
        id: 'ranger_poison_arrow',
        name: '毒箭',
        icon: '☠️',
        key: 'E',
        cooldown: 6,
        manaCost: 15,
        description: '射出一支毒箭，造成120%伤害并使敌人中毒5秒。',
        type: 'active',
        effects: {
          damage: 1.2,
          poison: 5,
          poisonDamage: 5
        }
      },
      {
        id: 'ranger_arrow_rain',
        name: '箭雨',
        icon: '🌧️',
        key: 'R',
        cooldown: 18,
        manaCost: 30,
        description: '向目标区域降下箭雨，持续3秒，每秒造成80%伤害。',
        type: 'active',
        effects: {
          damagePerSec: 0.8,
          radius: 100,
          duration: 3
        }
      },
      {
        id: 'ranger_wolf_form',
        name: '狼形态',
        icon: '🐺',
        key: 'F',
        cooldown: 60,
        manaCost: 40,
        description: '变身为巨狼，移动速度+50%，攻击速度+100%，持续20秒。',
        type: 'ultimate',
        effects: {
          speedBoost: 1.5,
          attackSpeedBoost: 2.0,
          duration: 20
        }
      }
    ],
    talentTree: {
      name: '游侠天赋',
      branches: [
        {
          name: '射击',
          talents: [
            { id: 'r_t1', name: '精准射击', desc: '暴击率+5%', maxLevel: 5, effect: 'crit', value: 0.05 },
            { id: 'r_t2', name: '穿透', desc: '箭矢可穿透1个敌人', maxLevel: 3, effect: 'pierce', value: 1 },
            { id: 'r_t3', name: '远程大师', desc: '攻击距离+30', maxLevel: 3, effect: 'range', value: 30 },
            { id: 'r_t4', name: '箭雨强化', desc: '箭雨持续时间+1秒', maxLevel: 3, effect: 'duration', value: 1, skill: 'ranger_arrow_rain' }
          ]
        },
        {
          name: '生存',
          talents: [
            { id: 'r_t5', name: '敏捷', desc: '闪避率+3%', maxLevel: 5, effect: 'dodge', value: 0.03 },
            { id: 'r_t6', name: '疾风步', desc: '移动速度+0.3', maxLevel: 5, effect: 'moveSpeed', value: 0.3 },
            { id: 'r_t7', name: '自然恢复', desc: '体力恢复+2/秒', maxLevel: 3, effect: 'staminaRegen', value: 2 },
            { id: 'r_t8', name: '野外生存', desc: '生命低于50%时闪避+20%', maxLevel: 1, effect: 'lowHpDodge', value: 0.2 }
          ]
        },
        {
          name: '陷阱',
          talents: [
            { id: 'r_t9', name: '剧毒', desc: '中毒伤害+50%', maxLevel: 5, effect: 'poisonDamage', value: 0.5 },
            { id: 'r_t10', name: '减速陷阱', desc: '攻击有15%几率减速敌人', maxLevel: 3, effect: 'slowChance', value: 0.15 },
            { id: 'r_t11', name: '多重箭', desc: '多重射击多1支箭', maxLevel: 2, effect: 'extraArrows', value: 1, skill: 'ranger_multishot' },
            { id: 'r_t12', name: '兽王', desc: '狼形态持续时间+10秒', maxLevel: 2, effect: 'duration', value: 10, skill: 'ranger_wolf_form' }
          ]
        }
      ]
    }
  },

  // ==================== 盗贼 ====================
  rogue: {
    name: '盗贼',
    icon: '🗡️',
    description: '暗影中的致命暗杀者，拥有极高的暴击和速度，擅长背刺和隐身。',
    difficulty: '困难',
    baseStats: {
      hp: 80,
      mp: 70,
      stamina: 110,
      str: 10,
      dex: 22,
      int: 10,
      vit: 10,
      luck: 18
    },
    growthStats: {
      hp: 7,
      mp: 6,
      stamina: 7,
      str: 1.5,
      dex: 4,
      int: 1,
      vit: 1.2,
      luck: 2
    },
    derivedStats: {
      damage: 16,
      attackSpeed: 1.5,
      attackRange: 45,
      critChance: 0.35,
      critDamage: 2.5,
      dodge: 0.20,
      armor: 4,
      magicResist: 6,
      moveSpeed: 4.2,
      hpRegen: 1.5,
      mpRegen: 2.5
    },
    skills: [
      {
        id: 'rogue_backstab',
        name: '背刺',
        icon: '🔪',
        key: 'Q',
        cooldown: 4,
        manaCost: 10,
        description: '瞬移到最近敌人背后，造成250%伤害且必定暴击。',
        type: 'active',
        effects: {
          damage: 2.5,
          range: 200,
          guaranteedCrit: true
        }
      },
      {
        id: 'rogue_stealth',
        name: '隐身',
        icon: '👻',
        key: 'W',
        cooldown: 12,
        manaCost: 20,
        description: '进入隐身状态5秒，攻击或使用技能后显形。',
        type: 'buff',
        effects: {
          duration: 5
        }
      },
      {
        id: 'rogue_smoke_bomb',
        name: '烟雾弹',
        icon: '💨',
        key: 'E',
        cooldown: 10,
        manaCost: 15,
        description: '释放烟雾，使周围敌人混乱3秒。',
        type: 'active',
        effects: {
          radius: 120,
          confuse: 3
        }
      },
      {
        id: 'rogue_deadly_strike',
        name: '致命一击',
        icon: '💀',
        key: 'R',
        cooldown: 15,
        manaCost: 30,
        description: '下一次攻击造成300%伤害。',
        type: 'buff',
        effects: {
          nextAttackDamage: 3.0
        }
      },
      {
        id: 'rogue_shadow_dance',
        name: '暗影之舞',
        icon: '🌑',
        key: 'F',
        cooldown: 60,
        manaCost: 50,
        description: '进入暗影之舞状态，持续10秒，期间可以无限使用背刺且不消耗魔法。',
        type: 'ultimate',
        effects: {
          duration: 10,
          freeSkills: ['rogue_backstab']
        }
      }
    ],
    talentTree: {
      name: '盗贼天赋',
      branches: [
        {
          name: '暗杀',
          talents: [
            { id: 'ro_t1', name: '致命', desc: '暴击伤害+15%', maxLevel: 5, effect: 'critDamage', value: 0.15 },
            { id: 'ro_t2', name: '弱点攻击', desc: '暴击率+5%', maxLevel: 5, effect: 'crit', value: 0.05 },
            { id: 'ro_t3', name: '背刺精通', desc: '背刺伤害+20%', maxLevel: 3, effect: 'skillDamage', value: 0.2, skill: 'rogue_backstab' },
            { id: 'ro_t4', name: '处决', desc: '对生命低于30%的敌人伤害+50%', maxLevel: 3, effect: 'executeDamage', value: 0.5 }
          ]
        },
        {
          name: '暗影',
          talents: [
            { id: 'ro_t5', name: '潜行', desc: '隐身持续时间+1秒', maxLevel: 3, effect: 'duration', value: 1, skill: 'rogue_stealth' },
            { id: 'ro_t6', name: '暗影步', desc: '移动速度+0.4', maxLevel: 5, effect: 'moveSpeed', value: 0.4 },
            { id: 'ro_t7', name: '烟雾大师', desc: '烟雾弹混乱时间+1秒', maxLevel: 3, effect: 'confuseDuration', value: 1 },
            { id: 'ro_t8', name: '暗影亲和', desc: '隐身时攻击伤害+50%', maxLevel: 2, effect: 'stealthDamage', value: 0.5 }
          ]
        },
        {
          name: '幸运',
          talents: [
            { id: 'ro_t9', name: '幸运', desc: '运气+5', maxLevel: 5, effect: 'luck', value: 5 },
            { id: 'ro_t10', name: '金币猎手', desc: '金币获取+20%', maxLevel: 3, effect: 'goldBonus', value: 0.2 },
            { id: 'ro_t11', name: '寻宝', desc: '装备掉落率+15%', maxLevel: 3, effect: 'dropRate', value: 0.15 },
            { id: 'ro_t12', name: '命运', desc: '致命一击伤害+100%', maxLevel: 2, effect: 'skillDamage', value: 1.0, skill: 'rogue_deadly_strike' }
          ]
        }
      ]
    }
  },

  // ==================== 圣骑士 ====================
  paladin: {
    name: '圣骑士',
    icon: '⚜️',
    description: '圣光庇护的神圣战士，攻守兼备，拥有治疗和光环能力。',
    difficulty: '中等',
    baseStats: {
      hp: 100,
      mp: 60,
      stamina: 100,
      str: 15,
      dex: 8,
      int: 12,
      vit: 14,
      luck: 10
    },
    growthStats: {
      hp: 10,
      mp: 6,
      stamina: 6,
      str: 2.5,
      dex: 0.8,
      int: 2,
      vit: 2,
      luck: 0.8
    },
    derivedStats: {
      damage: 15,
      attackSpeed: 0.9,
      attackRange: 55,
      critChance: 0.12,
      critDamage: 1.6,
      dodge: 0.08,
      armor: 10,
      magicResist: 12,
      moveSpeed: 3.3,
      hpRegen: 2.5,
      mpRegen: 2
    },
    skills: [
      {
        id: 'paladin_holy_strike',
        name: '圣光打击',
        icon: '✨',
        key: 'Q',
        cooldown: 4,
        manaCost: 12,
        description: '用圣光强化武器攻击，造成150%神圣伤害并治疗自身。',
        type: 'active',
        effects: {
          damage: 1.5,
          healPercent: 0.1
        }
      },
      {
        id: 'paladin_holy_shield',
        name: '神圣护盾',
        icon: '🛡️',
        key: 'W',
        cooldown: 12,
        manaCost: 20,
        description: '召唤神圣护盾，免疫所有伤害3秒。',
        type: 'buff',
        effects: {
          invincibility: 3
        }
      },
      {
        id: 'paladin_heal',
        name: '治疗术',
        icon: '💚',
        key: 'E',
        cooldown: 6,
        manaCost: 18,
        description: '治疗自身30%最大生命。',
        type: 'active',
        effects: {
          healPercent: 0.3
        }
      },
      {
        id: 'paladin_consecration',
        name: '奉献',
        icon: '🌟',
        key: 'R',
        cooldown: 15,
        manaCost: 25,
        description: '在地面奉献圣光，持续5秒，每秒对周围敌人造成80%神圣伤害。',
        type: 'active',
        effects: {
          damagePerSec: 0.8,
          radius: 100,
          duration: 5
        }
      },
      {
        id: 'paladin_divine_judgment',
        name: '神圣审判',
        icon: '⚡',
        key: 'F',
        cooldown: 60,
        manaCost: 50,
        description: '召唤神圣光柱审判敌人，造成500%伤害并眩晕3秒。',
        type: 'ultimate',
        effects: {
          damage: 5.0,
          radius: 150,
          stun: 3
        }
      }
    ],
    talentTree: {
      name: '圣骑士天赋',
      branches: [
        {
          name: '神圣',
          talents: [
            { id: 'p_t1', name: '圣光', desc: '神圣伤害+15%', maxLevel: 5, effect: 'holyDamage', value: 0.15 },
            { id: 'p_t2', name: '治疗精通', desc: '治疗量+20%', maxLevel: 5, effect: 'healBonus', value: 0.2 },
            { id: 'p_t3', name: '圣光打击强化', desc: '圣光打击伤害+25%', maxLevel: 3, effect: 'skillDamage', value: 0.25, skill: 'paladin_holy_strike' },
            { id: 'p_t4', name: '神圣审判', desc: '审判伤害+30%', maxLevel: 3, effect: 'skillDamage', value: 0.3, skill: 'paladin_divine_judgment' }
          ]
        },
        {
          name: '防护',
          talents: [
            { id: 'p_t5', name: '坚韧', desc: '护甲+8', maxLevel: 5, effect: 'armor', value: 8 },
            { id: 'p_t6', name: '魔法抗性', desc: '魔法抗性+10', maxLevel: 5, effect: 'magicResist', value: 10 },
            { id: 'p_t7', name: '神圣护盾', desc: '护盾持续时间+1秒', maxLevel: 3, effect: 'duration', value: 1, skill: 'paladin_holy_shield' },
            { id: 'p_t8', name: '圣盾', desc: '生命低于20%时自动获得护盾', maxLevel: 1, effect: 'autoShield', value: 1 }
          ]
        },
        {
          name: '惩戒',
          talents: [
            { id: 'p_t9', name: '力量', desc: '力量+5', maxLevel: 5, effect: 'str', value: 5 },
            { id: 'p_t10', name: '暴击', desc: '暴击率+4%', maxLevel: 5, effect: 'crit', value: 0.04 },
            { id: 'p_t11', name: '奉献强化', desc: '奉献范围+20%', maxLevel: 3, effect: 'radius', value: 0.2, skill: 'paladin_consecration' },
            { id: 'p_t12', name: '复仇', desc: '受到伤害时反弹10%', maxLevel: 3, effect: 'thorns', value: 0.1 }
          ]
        }
      ]
    }
  },

  // ==================== 死灵法师 ====================
  necromancer: {
    name: '死灵法师',
    icon: '💀',
    description: '操控亡灵的黑暗法师，可以召唤骷髅军团为自己作战。',
    difficulty: '困难',
    baseStats: {
      hp: 75,
      mp: 90,
      stamina: 85,
      str: 8,
      dex: 8,
      int: 22,
      vit: 9,
      luck: 10
    },
    growthStats: {
      hp: 7,
      mp: 9,
      stamina: 4,
      str: 0.8,
      dex: 0.8,
      int: 4.5,
      vit: 1.2,
      luck: 0.8
    },
    derivedStats: {
      damage: 13,
      attackSpeed: 0.85,
      attackRange: 280,
      critChance: 0.12,
      critDamage: 1.7,
      dodge: 0.07,
      armor: 4,
      magicResist: 18,
      moveSpeed: 3.1,
      hpRegen: 1.2,
      mpRegen: 3.5
    },
    skills: [
      {
        id: 'necro_shadow_bolt',
        name: '暗影箭',
        icon: '🌑',
        key: 'Q',
        cooldown: 2,
        manaCost: 6,
        description: '发射暗影箭，造成120%暗影伤害。',
        type: 'active',
        effects: {
          damage: 1.2
        }
      },
      {
        id: 'necro_summon_skeleton',
        name: '召唤骷髅',
        icon: '💀',
        key: 'W',
        cooldown: 10,
        manaCost: 25,
        description: '召唤一个骷髅战士为你作战，持续30秒。最多同时存在3个。',
        type: 'active',
        effects: {
          summon: 'skeleton',
          duration: 30,
          maxSummons: 3
        }
      },
      {
        id: 'necro_life_drain',
        name: '生命汲取',
        icon: '🩸',
        key: 'E',
        cooldown: 8,
        manaCost: 15,
        description: '吸取目标生命，造成100%伤害并治疗自身等量生命。',
        type: 'active',
        effects: {
          damage: 1.0,
          lifesteal: 1.0,
          range: 200
        }
      },
      {
        id: 'necro_corpse_explosion',
        name: '尸体爆炸',
        icon: '💥',
        key: 'R',
        cooldown: 12,
        manaCost: 20,
        description: '引爆周围尸体，每个尸体造成150%范围伤害。',
        type: 'active',
        effects: {
          damage: 1.5,
          radius: 80,
          useCorpses: true
        }
      },
      {
        id: 'necro_death_knight',
        name: '死亡骑士',
        icon: '⚔️',
        key: 'F',
        cooldown: 90,
        manaCost: 60,
        description: '召唤强大的死亡骑士为你作战，持续60秒。死亡骑士拥有你80%的属性。',
        type: 'ultimate',
        effects: {
          summon: 'death_knight',
          duration: 60,
          statMultiplier: 0.8
        }
      }
    ],
    talentTree: {
      name: '死灵法师天赋',
      branches: [
        {
          name: '召唤',
          talents: [
            { id: 'n_t1', name: '亡灵大师', desc: '骷髅伤害+20%', maxLevel: 5, effect: 'summonDamage', value: 0.2 },
            { id: 'n_t2', name: '亡灵坚韧', desc: '骷髅生命+30%', maxLevel: 5, effect: 'summonHp', value: 0.3 },
            { id: 'n_t3', name: '亡灵军团', desc: '最大骷髅数量+1', maxLevel: 2, effect: 'maxSummons', value: 1 },
            { id: 'n_t4', name: '死亡领主', desc: '死亡骑士持续时间+30秒', maxLevel: 2, effect: 'duration', value: 30, skill: 'necro_death_knight' }
          ]
        },
        {
          name: '暗影',
          talents: [
            { id: 'n_t5', name: '暗影精通', desc: '暗影伤害+15%', maxLevel: 5, effect: 'shadowDamage', value: 0.15 },
            { id: 'n_t6', name: '暗影箭强化', desc: '暗影箭伤害+25%', maxLevel: 3, effect: 'skillDamage', value: 0.25, skill: 'necro_shadow_bolt' },
            { id: 'n_t7', name: '腐蚀', desc: '攻击有20%几率使敌人中毒', maxLevel: 3, effect: 'poisonChance', value: 0.2 },
            { id: 'n_t8', name: '黑暗吞噬', desc: '生命汲取治疗量+50%', maxLevel: 3, effect: 'lifesteal', value: 0.5, skill: 'necro_life_drain' }
          ]
        },
        {
          name: '死亡',
          talents: [
            { id: 'n_t9', name: '死亡之力', desc: '智力+5', maxLevel: 5, effect: 'int', value: 5 },
            { id: 'n_t10', name: '尸体爆炸强化', desc: '爆炸伤害+30%', maxLevel: 3, effect: 'skillDamage', value: 0.3, skill: 'necro_corpse_explosion' },
            { id: 'n_t11', name: '死亡光环', desc: '周围敌人每秒受到2%最大生命伤害', maxLevel: 3, effect: 'auraDamage', value: 0.02 },
            { id: 'n_t12', name: '不朽', desc: '致命伤害保留1点生命，冷却60秒', maxLevel: 1, effect: 'immortal', value: 1 }
          ]
        }
      ]
    }
  }
};

window.ClassData = ClassData;
