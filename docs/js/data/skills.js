// ==================== 永恒地牢 - 技能数据配置 ====================
// 定义所有技能的详细效果、动画、音效

const SkillData = {
  // ==================== 战士技能 ====================
  warrior_charge: {
    name: '冲锋',
    icon: '⚡',
    type: 'active',
    cooldown: 5,
    manaCost: 10,
    castTime: 0,
    range: 150,
    description: '向鼠标方向冲锋，对路径上的敌人造成150%伤害并击退。',
    effects: {
      damageMultiplier: 1.5,
      knockback: 50,
      stun: 0.5
    },
    animation: {
      type: 'dash',
      color: '#3498db',
      trail: true
    },
    sound: 'charge'
  },
  warrior_shield_bash: {
    name: '盾击',
    icon: '🛡️',
    type: 'active',
    cooldown: 8,
    manaCost: 15,
    castTime: 0,
    range: 100,
    description: '用盾牌猛击周围敌人，造成80%伤害并眩晕2秒。',
    effects: {
      damageMultiplier: 0.8,
      radius: 100,
      stun: 2
    },
    animation: {
      type: 'aoe',
      color: '#95a5a6',
      radius: 100
    },
    sound: 'shield_bash'
  },
  warrior_battle_cry: {
    name: '战吼',
    icon: '📢',
    type: 'buff',
    cooldown: 15,
    manaCost: 20,
    duration: 10,
    description: '发出战吼，提升50%攻击力和20%移动速度。',
    effects: {
      damageBoost: 1.5,
      speedBoost: 1.2
    },
    animation: {
      type: 'buff',
      color: '#e67e22'
    },
    sound: 'battle_cry'
  },
  warrior_whirlwind: {
    name: '旋风斩',
    icon: '🌀',
    type: 'active',
    cooldown: 12,
    manaCost: 25,
    castTime: 0.5,
    range: 80,
    description: '旋转武器攻击周围所有敌人，造成200%伤害。',
    effects: {
      damageMultiplier: 2.0,
      radius: 80,
      channelTime: 0.5
    },
    animation: {
      type: 'spin',
      color: '#9b59b6',
      duration: 0.5
    },
    sound: 'whirlwind'
  },
  warrior_berserker: {
    name: '狂暴',
    icon: '😤',
    type: 'ultimate',
    cooldown: 60,
    manaCost: 40,
    duration: 15,
    description: '进入狂暴状态，攻击力+100%，攻击速度+50%，但受到伤害+30%。',
    effects: {
      damageBoost: 2.0,
      attackSpeedBoost: 1.5,
      damageTaken: 1.3
    },
    animation: {
      type: 'transform',
      color: '#e74c3c'
    },
    sound: 'berserker'
  },

  // ==================== 法师技能 ====================
  mage_fireball: {
    name: '火球术',
    icon: '🔥',
    type: 'active',
    cooldown: 2,
    manaCost: 8,
    castTime: 0,
    range: 400,
    projectileSpeed: 8,
    description: '发射一颗火球，命中后爆炸造成范围伤害并点燃敌人。',
    effects: {
      damageMultiplier: 2.0,
      explosionRadius: 60,
      burn: { damage: 5, duration: 3 }
    },
    animation: {
      type: 'projectile',
      color: '#e67e22',
      size: 12,
      trail: true
    },
    sound: 'fireball'
  },
  mage_ice_shield: {
    name: '冰盾',
    icon: '🧊',
    type: 'buff',
    cooldown: 10,
    manaCost: 20,
    duration: 8,
    description: '召唤冰盾吸收相当于最大生命50%的伤害。',
    effects: {
      shieldPercent: 0.5
    },
    animation: {
      type: 'shield',
      color: '#3498db'
    },
    sound: 'ice_shield'
  },
  mage_blink: {
    name: '闪现',
    icon: '✨',
    type: 'active',
    cooldown: 6,
    manaCost: 15,
    castTime: 0,
    range: 200,
    description: '瞬间传送到鼠标位置。',
    effects: {
      maxRange: 200
    },
    animation: {
      type: 'teleport',
      color: '#9b59b6'
    },
    sound: 'blink'
  },
  mage_meteor: {
    name: '陨石术',
    icon: '☄️',
    type: 'active',
    cooldown: 20,
    manaCost: 40,
    castTime: 0.5,
    range: 500,
    description: '召唤陨石轰炸目标区域，造成300%范围伤害。',
    effects: {
      damageMultiplier: 3.0,
      radius: 120,
      delay: 0.5,
      burn: { damage: 8, duration: 4 }
    },
    animation: {
      type: 'meteor',
      color: '#e67e22',
      radius: 120
    },
    sound: 'meteor'
  },
  mage_arcane_storm: {
    name: '奥术风暴',
    icon: '🌪️',
    type: 'ultimate',
    cooldown: 60,
    manaCost: 60,
    duration: 5,
    description: '在周围召唤奥术风暴，持续5秒，每秒对周围敌人造成100%伤害。',
    effects: {
      damagePerSec: 1.0,
      radius: 150
    },
    animation: {
      type: 'storm',
      color: '#9b59b6',
      radius: 150
    },
    sound: 'arcane_storm'
  },

  // ==================== 游侠技能 ====================
  ranger_multishot: {
    name: '多重射击',
    icon: '🏹',
    type: 'active',
    cooldown: 4,
    manaCost: 12,
    castTime: 0,
    range: 400,
    projectileSpeed: 12,
    description: '同时射出5支箭，每支造成80%伤害。',
    effects: {
      damageMultiplier: 0.8,
      arrows: 5,
      spread: 0.6
    },
    animation: {
      type: 'multishot',
      color: '#3498db'
    },
    sound: 'multishot'
  },
  ranger_dodge: {
    name: '闪避',
    icon: '💨',
    type: 'buff',
    cooldown: 8,
    manaCost: 10,
    duration: 3,
    description: '进入闪避状态，下次攻击必定闪避。',
    effects: {
      dodgeBoost: 1.0
    },
    animation: {
      type: 'buff',
      color: '#2ecc71'
    },
    sound: 'dodge'
  },
  ranger_poison_arrow: {
    name: '毒箭',
    icon: '☠️',
    type: 'active',
    cooldown: 6,
    manaCost: 15,
    castTime: 0,
    range: 400,
    projectileSpeed: 10,
    description: '射出一支毒箭，造成120%伤害并使敌人中毒5秒。',
    effects: {
      damageMultiplier: 1.2,
      poison: { damage: 5, duration: 5 }
    },
    animation: {
      type: 'projectile',
      color: '#27ae60',
      trail: true
    },
    sound: 'poison_arrow'
  },
  ranger_arrow_rain: {
    name: '箭雨',
    icon: '🌧️',
    type: 'active',
    cooldown: 18,
    manaCost: 30,
    castTime: 0.3,
    range: 500,
    duration: 3,
    description: '向目标区域降下箭雨，持续3秒，每秒造成80%伤害。',
    effects: {
      damagePerSec: 0.8,
      radius: 100
    },
    animation: {
      type: 'rain',
      color: '#95a5a6',
      radius: 100
    },
    sound: 'arrow_rain'
  },
  ranger_wolf_form: {
    name: '狼形态',
    icon: '🐺',
    type: 'ultimate',
    cooldown: 60,
    manaCost: 40,
    duration: 20,
    description: '变身为巨狼，移动速度+50%，攻击速度+100%。',
    effects: {
      speedBoost: 1.5,
      attackSpeedBoost: 2.0
    },
    animation: {
      type: 'transform',
      color: '#95a5a6'
    },
    sound: 'wolf_form'
  },

  // ==================== 盗贼技能 ====================
  rogue_backstab: {
    name: '背刺',
    icon: '🔪',
    type: 'active',
    cooldown: 4,
    manaCost: 10,
    castTime: 0,
    range: 200,
    description: '瞬移到最近敌人背后，造成250%伤害且必定暴击。',
    effects: {
      damageMultiplier: 2.5,
      guaranteedCrit: true
    },
    animation: {
      type: 'teleport_attack',
      color: '#9b59b6'
    },
    sound: 'backstab'
  },
  rogue_stealth: {
    name: '隐身',
    icon: '👻',
    type: 'buff',
    cooldown: 12,
    manaCost: 20,
    duration: 5,
    description: '进入隐身状态5秒，攻击或使用技能后显形。',
    effects: {
      stealth: true
    },
    animation: {
      type: 'stealth',
      color: '#2c3e50'
    },
    sound: 'stealth'
  },
  rogue_smoke_bomb: {
    name: '烟雾弹',
    icon: '💨',
    type: 'active',
    cooldown: 10,
    manaCost: 15,
    castTime: 0,
    range: 120,
    description: '释放烟雾，使周围敌人混乱3秒。',
    effects: {
      radius: 120,
      confuse: 3
    },
    animation: {
      type: 'smoke',
      color: '#7f8c8d',
      radius: 120
    },
    sound: 'smoke_bomb'
  },
  rogue_deadly_strike: {
    name: '致命一击',
    icon: '💀',
    type: 'buff',
    cooldown: 15,
    manaCost: 30,
    duration: 10,
    description: '下一次攻击造成300%伤害。',
    effects: {
      nextAttackDamage: 3.0
    },
    animation: {
      type: 'buff',
      color: '#e74c3c'
    },
    sound: 'deadly_strike'
  },
  rogue_shadow_dance: {
    name: '暗影之舞',
    icon: '🌑',
    type: 'ultimate',
    cooldown: 60,
    manaCost: 50,
    duration: 10,
    description: '进入暗影之舞状态，持续10秒，期间可以无限使用背刺。',
    effects: {
      freeSkills: ['rogue_backstab']
    },
    animation: {
      type: 'aura',
      color: '#2c3e50'
    },
    sound: 'shadow_dance'
  },

  // ==================== 圣骑士技能 ====================
  paladin_holy_strike: {
    name: '圣光打击',
    icon: '✨',
    type: 'active',
    cooldown: 4,
    manaCost: 12,
    castTime: 0,
    range: 60,
    description: '用圣光强化武器攻击，造成150%神圣伤害并治疗自身10%。',
    effects: {
      damageMultiplier: 1.5,
      healPercent: 0.1,
      damageType: 'holy'
    },
    animation: {
      type: 'melee',
      color: '#f1c40f'
    },
    sound: 'holy_strike'
  },
  paladin_holy_shield: {
    name: '神圣护盾',
    icon: '🛡️',
    type: 'buff',
    cooldown: 12,
    manaCost: 20,
    duration: 3,
    description: '召唤神圣护盾，免疫所有伤害3秒。',
    effects: {
      invincibility: 3
    },
    animation: {
      type: 'shield',
      color: '#f1c40f'
    },
    sound: 'holy_shield'
  },
  paladin_heal: {
    name: '治疗术',
    icon: '💚',
    type: 'active',
    cooldown: 6,
    manaCost: 18,
    castTime: 0,
    description: '治疗自身30%最大生命。',
    effects: {
      healPercent: 0.3
    },
    animation: {
      type: 'heal',
      color: '#2ecc71'
    },
    sound: 'heal'
  },
  paladin_consecration: {
    name: '奉献',
    icon: '🌟',
    type: 'active',
    cooldown: 15,
    manaCost: 25,
    duration: 5,
    description: '在地面奉献圣光，持续5秒，每秒对周围敌人造成80%神圣伤害。',
    effects: {
      damagePerSec: 0.8,
      radius: 100,
      damageType: 'holy'
    },
    animation: {
      type: 'ground_aoe',
      color: '#f1c40f',
      radius: 100
    },
    sound: 'consecration'
  },
  paladin_divine_judgment: {
    name: '神圣审判',
    icon: '⚡',
    type: 'ultimate',
    cooldown: 60,
    manaCost: 50,
    castTime: 1,
    range: 150,
    description: '召唤神圣光柱审判敌人，造成500%伤害并眩晕3秒。',
    effects: {
      damageMultiplier: 5.0,
      radius: 150,
      stun: 3,
      damageType: 'holy'
    },
    animation: {
      type: 'beam',
      color: '#f1c40f',
      radius: 150
    },
    sound: 'divine_judgment'
  },

  // ==================== 死灵法师技能 ====================
  necro_shadow_bolt: {
    name: '暗影箭',
    icon: '🌑',
    type: 'active',
    cooldown: 2,
    manaCost: 6,
    castTime: 0,
    range: 350,
    projectileSpeed: 9,
    description: '发射暗影箭，造成120%暗影伤害。',
    effects: {
      damageMultiplier: 1.2,
      damageType: 'shadow'
    },
    animation: {
      type: 'projectile',
      color: '#9b59b6',
      trail: true
    },
    sound: 'shadow_bolt'
  },
  necro_summon_skeleton: {
    name: '召唤骷髅',
    icon: '💀',
    type: 'active',
    cooldown: 10,
    manaCost: 25,
    castTime: 1,
    description: '召唤一个骷髅战士为你作战，持续30秒。最多同时存在3个。',
    effects: {
      summon: 'skeleton',
      duration: 30,
      maxSummons: 3
    },
    animation: {
      type: 'summon',
      color: '#bdc3c7'
    },
    sound: 'summon'
  },
  necro_life_drain: {
    name: '生命汲取',
    icon: '🩸',
    type: 'active',
    cooldown: 8,
    manaCost: 15,
    castTime: 0,
    range: 200,
    channelTime: 2,
    description: '吸取目标生命，造成100%伤害并治疗自身等量生命。',
    effects: {
      damageMultiplier: 1.0,
      lifesteal: 1.0
    },
    animation: {
      type: 'beam',
      color: '#e74c3c'
    },
    sound: 'life_drain'
  },
  necro_corpse_explosion: {
    name: '尸体爆炸',
    icon: '💥',
    type: 'active',
    cooldown: 12,
    manaCost: 20,
    castTime: 0,
    range: 80,
    description: '引爆周围尸体，每个尸体造成150%范围伤害。',
    effects: {
      damageMultiplier: 1.5,
      radius: 80,
      useCorpses: true
    },
    animation: {
      type: 'explosion',
      color: '#8b4513',
      radius: 80
    },
    sound: 'corpse_explosion'
  },
  necro_death_knight: {
    name: '死亡骑士',
    icon: '⚔️',
    type: 'ultimate',
    cooldown: 90,
    manaCost: 60,
    castTime: 2,
    duration: 60,
    description: '召唤强大的死亡骑士为你作战，持续60秒。',
    effects: {
      summon: 'death_knight',
      duration: 60,
      statMultiplier: 0.8
    },
    animation: {
      type: 'summon',
      color: '#2c3e50'
    },
    sound: 'death_knight'
  },

  // ==================== 通用技能效果 ====================
  // 伤害类型
  damageTypes: {
    physical: { color: '#fff', resistStat: 'armor' },
    fire: { color: '#e67e22', resistStat: 'fireResist' },
    ice: { color: '#3498db', resistStat: 'iceResist' },
    lightning: { color: '#f1c40f', resistStat: 'lightningResist' },
    holy: { color: '#fffacd', resistStat: 'holyResist' },
    shadow: { color: '#9b59b6', resistStat: 'shadowResist' },
    poison: { color: '#27ae60', resistStat: 'poisonResist' }
  },

  // 工具方法
  getSkill(skillId) {
    return this[skillId] || null;
  },

  // 获取职业技能列表
  getClassSkills(classType) {
    const classData = ClassData[classType];
    if (!classData) return [];
    return classData.skills.map(s => ({
      ...s,
      ...this[s.id]
    }));
  },

  // 计算技能伤害
  calculateSkillDamage(skill, player, target) {
    if (!skill || !skill.effects) return 0;
    let baseDamage = player.damage;
    let multiplier = skill.effects.damageMultiplier || 1;
    let damage = baseDamage * multiplier;

    // 伤害类型抗性
    if (skill.effects.damageType && target) {
      const resist = target[skill.effects.damageType + 'Resist'] || 0;
      damage *= (1 - resist / 100);
    }

    // 目标护甲
    if (target && target.armor) {
      damage = Math.max(1, damage - target.armor * 0.5);
    }

    return Math.floor(damage);
  }
};

window.SkillData = SkillData;
