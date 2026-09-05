// ==================== 永恒地牢 - Buff/Debuff系统 ====================
// 管理所有状态效果：增益、减益、持续伤害、控制效果

const BuffSystem = {
  // ==================== Buff定义 ====================
  buffs: {
    // 增益效果
    strength: {
      name: '力量提升',
      icon: '💪',
      type: 'buff',
      stat: 'str',
      value: 10,
      duration: 60,
      color: '#e67e22',
      description: '力量+10'
    },
    agility: {
      name: '迅捷',
      icon: '⚡',
      type: 'buff',
      stat: 'dex',
      value: 10,
      duration: 60,
      color: '#2ecc71',
      description: '敏捷+10'
    },
    defense: {
      name: '铁壁',
      icon: '🛡️',
      type: 'buff',
      stat: 'armor',
      value: 15,
      duration: 60,
      color: '#95a5a6',
      description: '护甲+15'
    },
    damageBoost: {
      name: '伤害提升',
      icon: '⚔️',
      type: 'buff',
      stat: 'damageMultiplier',
      value: 1.5,
      duration: 10,
      color: '#e74c3c',
      description: '伤害+50%'
    },
    speedBoost: {
      name: '加速',
      icon: '💨',
      type: 'buff',
      stat: 'moveSpeed',
      value: 1.5,
      duration: 10,
      color: '#3498db',
      description: '移动速度+50%'
    },
    attackSpeedBoost: {
      name: '急速',
      icon: '⏱️',
      type: 'buff',
      stat: 'attackSpeed',
      value: 1.5,
      duration: 10,
      color: '#f1c40f',
      description: '攻击速度+50%'
    },
    shield: {
      name: '护盾',
      icon: '🔰',
      type: 'buff',
      stat: 'shield',
      value: 100,
      duration: 8,
      color: '#3498db',
      description: '吸收100点伤害'
    },
    invincibility: {
      name: '无敌',
      icon: '✨',
      type: 'buff',
      stat: 'invincible',
      value: true,
      duration: 3,
      color: '#f1c40f',
      description: '免疫所有伤害'
    },
    stealth: {
      name: '隐身',
      icon: '👻',
      type: 'buff',
      stat: 'stealth',
      value: true,
      duration: 5,
      color: '#2c3e50',
      description: '敌人无法发现你'
    },
    lifesteal: {
      name: '吸血',
      icon: '🩸',
      type: 'buff',
      stat: 'lifesteal',
      value: 0.2,
      duration: 15,
      color: '#c0392b',
      description: '攻击吸血20%'
    },
    critBoost: {
      name: '暴击提升',
      icon: '💥',
      type: 'buff',
      stat: 'crit',
      value: 0.3,
      duration: 15,
      color: '#f1c40f',
      description: '暴击率+30%'
    },
    hpRegen: {
      name: '生命恢复',
      icon: '❤️',
      type: 'buff',
      stat: 'hpRegen',
      value: 10,
      duration: 20,
      color: '#2ecc71',
      description: '每秒恢复10生命'
    },
    mpRegen: {
      name: '魔力恢复',
      icon: '💙',
      type: 'buff',
      stat: 'mpRegen',
      value: 5,
      duration: 20,
      color: '#3498db',
      description: '每秒恢复5魔力'
    },
    bless: {
      name: '祝福',
      icon: '🌟',
      type: 'buff',
      stat: 'allStats',
      value: 5,
      duration: 300,
      color: '#f1c40f',
      description: '全属性+5'
    },
    rage: {
      name: '狂暴',
      icon: '😤',
      type: 'buff',
      stat: 'damageMultiplier',
      value: 2.0,
      duration: 15,
      color: '#e74c3c',
      description: '伤害+100%，受到伤害+30%',
      sideEffect: { stat: 'damageTaken', value: 1.3 }
    },

    // 减益效果
    poison: {
      name: '中毒',
      icon: '☠️',
      type: 'debuff',
      damagePerSec: 5,
      duration: 5,
      color: '#27ae60',
      description: '每秒受到5点伤害',
      stackable: true,
      maxStacks: 5
    },
    burn: {
      name: '燃烧',
      icon: '🔥',
      type: 'debuff',
      damagePerSec: 8,
      duration: 3,
      color: '#e67e22',
      description: '每秒受到8点火焰伤害',
      stackable: true,
      maxStacks: 3
    },
    freeze: {
      name: '冰冻',
      icon: '🧊',
      type: 'debuff',
      stat: 'moveSpeed',
      value: 0.3,
      duration: 3,
      color: '#3498db',
      description: '移动速度降低70%'
    },
    slow: {
      name: '减速',
      icon: '🐌',
      type: 'debuff',
      stat: 'moveSpeed',
      value: 0.5,
      duration: 4,
      color: '#95a5a6',
      description: '移动速度降低50%'
    },
    stun: {
      name: '眩晕',
      icon: '💫',
      type: 'debuff',
      stat: 'stunned',
      value: true,
      duration: 2,
      color: '#f1c40f',
      description: '无法移动和攻击'
    },
    silence: {
      name: '沉默',
      icon: '🤫',
      type: 'debuff',
      stat: 'silenced',
      value: true,
      duration: 3,
      color: '#9b59b6',
      description: '无法使用技能'
    },
    confuse: {
      name: '混乱',
      icon: '😵',
      type: 'debuff',
      stat: 'confused',
      value: true,
      duration: 3,
      color: '#e67e22',
      description: '移动方向随机'
    },
    weakness: {
      name: '虚弱',
      icon: '💔',
      type: 'debuff',
      stat: 'damageMultiplier',
      value: 0.5,
      duration: 10,
      color: '#7f8c8d',
      description: '伤害降低50%'
    },
    armorBreak: {
      name: '破甲',
      icon: '🔨',
      type: 'debuff',
      stat: 'armor',
      value: -20,
      duration: 8,
      color: '#8b4513',
      description: '护甲降低20'
    },
    curse: {
      name: '诅咒',
      icon: '🌑',
      type: 'debuff',
      stat: 'allStats',
      value: -10,
      duration: 30,
      color: '#2c3e50',
      description: '全属性-10'
    },
    bleed: {
      name: '流血',
      icon: '🩸',
      type: 'debuff',
      damagePerSec: 3,
      duration: 6,
      color: '#c0392b',
      description: '每秒受到3点伤害',
      stackable: true,
      maxStacks: 10
    },
    root: {
      name: '定身',
      icon: '🌿',
      type: 'debuff',
      stat: 'moveSpeed',
      value: 0,
      duration: 2,
      color: '#27ae60',
      description: '无法移动'
    },
    fear: {
      name: '恐惧',
      icon: '😱',
      type: 'debuff',
      stat: 'feared',
      value: true,
      duration: 3,
      color: '#9b59b6',
      description: '不受控制地逃跑'
    },
    taunt: {
      name: '嘲讽',
      icon: '😠',
      type: 'debuff',
      stat: 'taunted',
      value: true,
      duration: 3,
      color: '#e74c3c',
      description: '强制攻击施法者'
    },

    // 特殊效果
    berserker: {
      name: '狂暴状态',
      icon: '🔥',
      type: 'transform',
      stats: {
        damageMultiplier: 2.0,
        attackSpeed: 1.5,
        damageTaken: 1.3
      },
      duration: 15,
      color: '#e74c3c',
      description: '战士终极技能状态'
    },
    wolfForm: {
      name: '狼形态',
      icon: '🐺',
      type: 'transform',
      stats: {
        moveSpeed: 1.5,
        attackSpeed: 2.0,
        damageMultiplier: 1.2
      },
      duration: 20,
      color: '#95a5a6',
      description: '游侠终极技能状态'
    },
    shadowDance: {
      name: '暗影之舞',
      icon: '🌑',
      type: 'transform',
      stats: {
        stealth: true,
        crit: 0.5,
        cooldownReduction: 0.5
      },
      duration: 10,
      color: '#2c3e50',
      description: '盗贼终极技能状态'
    }
  },

  // ==================== 系统方法 ====================

  // 创建Buff实例
  createBuff(buffId, source = null, duration = null) {
    const buffDef = this.buffs[buffId];
    if (!buffDef) return null;
    return {
      id: buffId,
      ...Utils.deepClone(buffDef),
      remaining: duration || buffDef.duration,
      source,
      stacks: 1,
      startTime: Date.now()
    };
  },

  // 应用Buff到实体
  applyBuff(entity, buffId, source = null, duration = null) {
    if (!entity.buffs) entity.buffs = [];

    const buffDef = this.buffs[buffId];
    if (!buffDef) return;

    // 检查是否已有相同Buff
    const existing = entity.buffs.find(b => b.id === buffId);
    if (existing) {
      if (buffDef.stackable && existing.stacks < (buffDef.maxStacks || 1)) {
        existing.stacks++;
        existing.remaining = Math.max(existing.remaining, duration || buffDef.duration);
      } else {
        existing.remaining = Math.max(existing.remaining, duration || buffDef.duration);
      }
      return;
    }

    const buff = this.createBuff(buffId, source, duration);
    entity.buffs.push(buff);

    // 立即应用属性修改
    this.applyBuffStats(entity, buff);
  },

  // 应用Buff属性
  applyBuffStats(entity, buff) {
    if (!buff.stats) {
      // 单属性Buff
      if (buff.stat && buff.value !== undefined) {
        if (!entity.buffModifiers) entity.buffModifiers = {};
        if (!entity.buffModifiers[buff.stat]) entity.buffModifiers[buff.stat] = 0;
        entity.buffModifiers[buff.stat] += buff.value * (buff.stacks || 1);
      }
    } else {
      // 多属性Buff（变形状态）
      for (const stat in buff.stats) {
        if (!entity.buffModifiers) entity.buffModifiers = {};
        if (!entity.buffModifiers[stat]) entity.buffModifiers[stat] = 0;
        entity.buffModifiers[stat] += buff.stats[stat];
      }
    }
  },

  // 移除Buff
  removeBuff(entity, buffId) {
    if (!entity.buffs) return;
    const index = entity.buffs.findIndex(b => b.id === buffId);
    if (index === -1) return;

    const buff = entity.buffs[index];
    this.removeBuffStats(entity, buff);
    entity.buffs.splice(index, 1);
  },

  // 移除Buff属性
  removeBuffStats(entity, buff) {
    if (!entity.buffModifiers) return;

    if (!buff.stats) {
      if (buff.stat && entity.buffModifiers[buff.stat] !== undefined) {
        entity.buffModifiers[buff.stat] -= buff.value * (buff.stacks || 1);
      }
    } else {
      for (const stat in buff.stats) {
        if (entity.buffModifiers[stat] !== undefined) {
          entity.buffModifiers[stat] -= buff.stats[stat];
        }
      }
    }
  },

  // 更新所有Buff
  updateBuffs(entity, dt) {
    if (!entity.buffs || entity.buffs.length === 0) return;

    for (let i = entity.buffs.length - 1; i >= 0; i--) {
      const buff = entity.buffs[i];
      buff.remaining -= dt;

      // 持续伤害
      if (buff.damagePerSec) {
        const damage = buff.damagePerSec * buff.stacks * dt;
        entity.hp -= damage;
        if (Math.random() < 0.1) {
          ParticleSystem.damageNumber(entity.x, entity.y, Math.floor(damage), false, false, false);
        }
      }

      // 持续治疗
      if (buff.healPerSec) {
        const heal = buff.healPerSec * buff.stacks * dt;
        entity.hp = Math.min(entity.maxHp, entity.hp + heal);
      }

      // Buff过期
      if (buff.remaining <= 0) {
        this.removeBuffStats(entity, buff);
        entity.buffs.splice(i, 1);
      }
    }
  },

  // 检查实体是否有某个Buff
  hasBuff(entity, buffId) {
    if (!entity.buffs) return false;
    return entity.buffs.some(b => b.id === buffId);
  },

  // 获取Buff层数
  getBuffStacks(entity, buffId) {
    if (!entity.buffs) return 0;
    const buff = entity.buffs.find(b => b.id === buffId);
    return buff ? buff.stacks : 0;
  },

  // 清除所有Debuff
  cleanseDebuffs(entity) {
    if (!entity.buffs) return;
    for (let i = entity.buffs.length - 1; i >= 0; i--) {
      if (entity.buffs[i].type === 'debuff') {
        this.removeBuffStats(entity, entity.buffs[i]);
        entity.buffs.splice(i, 1);
      }
    }
  },

  // 清除所有Buff
  clearAllBuffs(entity) {
    if (!entity.buffs) return;
    for (const buff of entity.buffs) {
      this.removeBuffStats(entity, buff);
    }
    entity.buffs = [];
  },

  // 获取实体的有效属性
  getEffectiveStat(entity, stat, baseValue) {
    if (!entity.buffModifiers || entity.buffModifiers[stat] === undefined) {
      return baseValue;
    }
    const modifier = entity.buffModifiers[stat];
    // 百分比修饰符（值在0-2之间）
    if (modifier > 0 && modifier < 3 && stat.includes('Multiplier') || stat === 'moveSpeed' || stat === 'attackSpeed') {
      return baseValue * modifier;
    }
    return baseValue + modifier;
  },

  // 检查实体是否被控制
  isControlled(entity) {
    if (!entity.buffs) return false;
    return entity.buffs.some(b =>
      b.stat === 'stunned' ||
      b.stat === 'feared' ||
      b.stat === 'confused' ||
      b.stat === 'moveSpeed' && b.value === 0
    );
  },

  // 检查实体是否可以施法
  canCast(entity) {
    if (!entity.buffs) return true;
    return !entity.buffs.some(b => b.stat === 'silenced' || b.stat === 'stunned');
  },

  // 获取Buff图标列表（用于UI显示）
  getBuffIcons(entity) {
    if (!entity.buffs) return [];
    return entity.buffs.map(b => ({
      id: b.id,
      icon: b.icon,
      name: b.name,
      remaining: b.remaining,
      duration: b.duration,
      stacks: b.stacks,
      type: b.type,
      color: b.color
    }));
  }
};

window.BuffSystem = BuffSystem;
