/**
 * 符文系统 (Rune System)
 * 允许玩家给装备镶嵌符文，获得各种属性加成
 */

const RuneSystem = {
  // 符文类型定义
  runeTypes: {
    // 攻击类符文
    fire: {
      name: '炎之符文',
      icon: '🔥',
      color: '#ff6b6b',
      rarity: 'common',
      effect: { attackDamage: 5 },
      description: '镶嵌后增加 5 点攻击力',
      maxStack: 3
    },
    ice: {
      name: '冰之符文',
      icon: '❄️',
      color: '#74b9ff',
      rarity: 'common',
      effect: { attackDamage: 3, critChance: 0.02 },
      description: '镶嵌后增加 3 点攻击力和 2% 暴击率',
      maxStack: 3
    },
    thunder: {
      name: '雷之符文',
      icon: '⚡',
      color: '#fdcb6e',
      rarity: 'rare',
      effect: { attackDamage: 8, attackSpeed: 0.1 },
      description: '镶嵌后增加 8 点攻击力和 10% 攻击速度',
      maxStack: 2
    },
    wind: {
      name: '风之符文',
      icon: '🌪️',
      color: '#a8e6cf',
      rarity: 'rare',
      effect: { attackSpeed: 0.15, moveSpeed: 50 },
      description: '镶嵌后增加 15% 攻击速度和 50 移动速度',
      maxStack: 2
    },
    // 防御类符文
    earth: {
      name: '土之符文',
      icon: '🪨',
      color: '#b2bec3',
      rarity: 'common',
      effect: { defense: 5, maxHp: 20 },
      description: '镶嵌后增加 5 点防御力和 20 点最大生命',
      maxStack: 3
    },
    water: {
      name: '水之符文',
      icon: '💧',
      color: '#81ecec',
      rarity: 'common',
      effect: { maxMp: 30, hpRegen: 1 },
      description: '镶嵌后增加 30 点最大法力和 1 点生命恢复',
      maxStack: 3
    },
    light: {
      name: '光之符文',
      icon: '✨',
      color: '#ffeaa7',
      rarity: 'rare',
      effect: { defense: 8, maxHp: 50, hpRegen: 2 },
      description: '镶嵌后增加 8 点防御力、50 点最大生命和 2 点生命恢复',
      maxStack: 2
    },
    dark: {
      name: '暗之符文',
      icon: '🌑',
      color: '#636e72',
      rarity: 'epic',
      effect: { attackDamage: 10, defense: 5, lifesteal: 0.05 },
      description: '镶嵌后增加 10 点攻击力、5 点防御力和 5% 生命偷取',
      maxStack: 1
    },
    // 特殊符文
    vampire: {
      name: '吸血符文',
      icon: '🦇',
      color: '#d63031',
      rarity: 'epic',
      effect: { lifesteal: 0.1 },
      description: '镶嵌后增加 10% 生命偷取',
      maxStack: 1
    },
    critical: {
      name: '暴击符文',
      icon: '💥',
      color: '#e17055',
      rarity: 'rare',
      effect: { critChance: 0.05, critDamage: 0.2 },
      description: '镶嵌后增加 5% 暴击率和 20% 暴击伤害',
      maxStack: 2
    },
    fortune: {
      name: '幸运符文',
      icon: '🍀',
      color: '#00b894',
      rarity: 'epic',
      effect: { magicFind: 0.15, goldBonus: 0.2 },
      description: '镶嵌后增加 15% 物品掉落率和 20% 金币加成',
      maxStack: 1
    },
    ancient: {
      name: '远古符文',
      icon: '🏺',
      color: '#d4a574',
      rarity: 'legendary',
      effect: { attackDamage: 15, defense: 10, maxHp: 100, maxMp: 50, allStats: 5 },
      description: '镶嵌后全属性大幅提升（攻击+15、防御+10、生命+100、法力+50、全属性+5）',
      maxStack: 1
    },
    dragon: {
      name: '龙之符文',
      icon: '🐉',
      color: '#e84393',
      rarity: 'legendary',
      effect: { attackDamage: 20, critChance: 0.1, critDamage: 0.5, lifesteal: 0.05 },
      description: '传说中的龙之符文，攻击+20、暴击+10%、暴伤+50%、吸血+5%',
      maxStack: 1
    }
  },

  // 稀有度颜色
  rarityColors: {
    common: '#b2bec3',
    rare: '#74b9ff',
    epic: '#a29bfe',
    legendary: '#fdcb6e'
  },

  // 可镶嵌符文的装备槽位
  enchantableSlots: ['weapon', 'armor', 'helmet', 'boots', 'ring', 'amulet'],

  /**
   * 获取符文信息
   */
  getRune(runeType) {
    return this.runeTypes[runeType] || null;
  },

  /**
   * 获取所有符文类型
   */
  getAllRunes() {
    return Object.keys(this.runeTypes);
  },

  /**
   * 按稀有度获取符文
   */
  getRunesByRarity(rarity) {
    return Object.keys(this.runeTypes).filter(type => this.runeTypes[type].rarity === rarity);
  },

  /**
   * 随机获取一个符文（根据稀有度权重）
   */
  getRandomRune(floor = 1) {
    const weights = {
      common: 60,
      rare: 25,
      epic: 12,
      legendary: 3
    };
    
    // 随着楼层增加，稀有符文概率提升
    const bonus = Math.min(floor * 0.5, 20);
    weights.rare += bonus;
    weights.epic += bonus * 0.5;
    weights.legendary += bonus * 0.2;
    weights.common -= bonus * 1.7;
    weights.common = Math.max(weights.common, 20);

    const total = Object.values(weights).reduce((a, b) => a + b, 0);
    let random = Math.random() * total;
    
    for (const [rarity, weight] of Object.entries(weights)) {
      random -= weight;
      if (random <= 0) {
        const runes = this.getRunesByRarity(rarity);
        return runes[Math.floor(Math.random() * runes.length)];
      }
    }
    
    return 'fire'; // 默认返回普通符文
  },

  /**
   * 给装备镶嵌符文
   */
  enchantItem(item, runeType) {
    if (!item || !runeType) return false;
    
    const rune = this.getRune(runeType);
    if (!rune) return false;
    
    // 检查装备是否可以镶嵌
    if (!this.enchantableSlots.includes(item.slot)) {
      return { success: false, message: '该装备无法镶嵌符文' };
    }
    
    // 初始化符文槽
    if (!item.runes) item.runes = [];
    
    // 检查符文槽数量（默认2个，稀有装备3个，传说4个）
    const maxSlots = item.rarity === 'legendary' ? 4 : item.rarity === 'epic' ? 3 : 2;
    if (item.runes.length >= maxSlots) {
      return { success: false, message: '符文槽已满' };
    }
    
    // 检查同类型符文数量
    const sameTypeCount = item.runes.filter(r => r.type === runeType).length;
    if (sameTypeCount >= rune.maxStack) {
      return { success: false, message: `该符文最多镶嵌 ${rune.maxStack} 个` };
    }
    
    // 镶嵌符文
    item.runes.push({
      type: runeType,
      name: rune.name,
      icon: rune.icon,
      effect: { ...rune.effect }
    });
    
    // 重新计算装备属性
    this.recalculateItemStats(item);
    
    return { success: true, message: `成功镶嵌 ${rune.name}！` };
  },

  /**
   * 移除装备上的符文
   */
  removeRune(item, index) {
    if (!item || !item.runes || index < 0 || index >= item.runes.length) {
      return { success: false, message: '无效的符文位置' };
    }
    
    const removed = item.runes.splice(index, 1)[0];
    this.recalculateItemStats(item);
    
    return { success: true, message: `已移除 ${removed.name}`, rune: removed };
  },

  /**
   * 重新计算装备属性（包含符文加成）
   */
  recalculateItemStats(item) {
    if (!item) return;
    
    // 保存基础属性
    if (!item.baseStats) {
      item.baseStats = { ...item.stats };
    }
    
    // 重置为基础属性
    item.stats = { ...item.baseStats };
    
    // 叠加符文属性
    if (item.runes) {
      for (const rune of item.runes) {
        for (const [stat, value] of Object.entries(rune.effect)) {
          if (item.stats[stat] !== undefined) {
            item.stats[stat] += value;
          } else {
            item.stats[stat] = value;
          }
        }
      }
    }
  },

  /**
   * 计算玩家所有装备的符文总加成
   */
  getPlayerRuneBonus(player) {
    const bonus = {};
    
    if (!player || !player.inventory) return bonus;
    
    const equipped = player.inventory.equipped || {};
    
    for (const [slot, item] of Object.entries(equipped)) {
      if (item && item.runes) {
        for (const rune of item.runes) {
          for (const [stat, value] of Object.entries(rune.effect)) {
            bonus[stat] = (bonus[stat] || 0) + value;
          }
        }
      }
    }
    
    return bonus;
  },

  /**
   * 应用符文加成到玩家
   */
  applyRuneBonus(player) {
    if (!player) return;
    
    const bonus = this.getPlayerRuneBonus(player);
    
    // 保存基础属性
    if (!player.baseStats) {
      player.baseStats = {
        maxHp: player.maxHp,
        maxMp: player.maxMp,
        attackDamage: player.attackDamage,
        defense: player.defense,
        attackSpeed: player.attackSpeed,
        moveSpeed: player.moveSpeed,
        critChance: player.critChance,
        critDamage: player.critDamage,
        lifesteal: player.lifesteal || 0,
        hpRegen: player.hpRegen || 0,
        mpRegen: player.mpRegen || 0
      };
    }
    
    // 应用加成
    for (const [stat, value] of Object.entries(bonus)) {
      if (player[stat] !== undefined) {
        player[stat] = player.baseStats[stat] + value;
      }
    }
  },

  /**
   * 生成符文物品（用于掉落和商店）
   */
  createRuneItem(runeType, floor = 1) {
    const rune = this.getRune(runeType);
    if (!rune) return null;
    
    return {
      id: `rune_${runeType}_${Date.now()}`,
      name: rune.name,
      icon: rune.icon,
      type: 'rune',
      runeType: runeType,
      rarity: rune.rarity,
      color: rune.color,
      description: rune.description,
      effect: { ...rune.effect },
      stackable: false,
      maxStack: 1,
      value: Math.floor((rune.rarity === 'legendary' ? 500 : rune.rarity === 'epic' ? 200 : rune.rarity === 'rare' ? 80 : 30) * (1 + floor * 0.1)),
      sellValue: Math.floor((rune.rarity === 'legendary' ? 250 : rune.rarity === 'epic' ? 100 : rune.rarity === 'rare' ? 40 : 15) * (1 + floor * 0.1))
    };
  },

  /**
   * 获取符文掉落概率（根据怪物类型）
   */
  getRuneDropChance(monsterType = 'normal') {
    const chances = {
      normal: 0.05,    // 普通怪物 5%
      elite: 0.15,      // 精英怪物 15%
      boss: 0.5,        // Boss 50%
      worldBoss: 1.0    // 世界Boss 100%
    };
    return chances[monsterType] || 0.05;
  },

  /**
   * 怪物死亡时尝试掉落符文
   */
  tryDropRune(monster, floor) {
    const chance = this.getRuneDropChance(monster.type || 'normal');
    if (Math.random() > chance) return null;
    
    const runeType = this.getRandomRune(floor);
    return this.createRuneItem(runeType, floor);
  },

  /**
   * 格式化符文效果描述
   */
  formatRuneEffect(effect) {
    const statNames = {
      attackDamage: '攻击力',
      defense: '防御力',
      maxHp: '最大生命',
      maxMp: '最大法力',
      attackSpeed: '攻击速度',
      moveSpeed: '移动速度',
      critChance: '暴击率',
      critDamage: '暴击伤害',
      lifesteal: '生命偷取',
      hpRegen: '生命恢复',
      mpRegen: '法力恢复',
      magicFind: '物品掉落',
      goldBonus: '金币加成',
      allStats: '全属性'
    };
    
    const parts = [];
    for (const [stat, value] of Object.entries(effect)) {
      const name = statNames[stat] || stat;
      const isPercent = ['attackSpeed', 'critChance', 'critDamage', 'lifesteal', 'magicFind', 'goldBonus'].includes(stat);
      parts.push(`${name} +${isPercent ? (value * 100).toFixed(0) + '%' : value}`);
    }
    
    return parts.join('，');
  }
};

// 导出到全局
if (typeof window !== 'undefined') {
  window.RuneSystem = RuneSystem;
}
