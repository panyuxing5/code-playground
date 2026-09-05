// ==================== 永恒地牢 - 数值平衡配置 ====================
// 全局数值调优，确保游戏体验平衡

const BalanceConfig = {
  // ==================== 玩家成长曲线 ====================
  player: {
    // 每级属性成长
    hpPerLevel: 15,
    mpPerLevel: 8,
    damagePerLevel: 2,
    armorPerLevel: 0.5,
    magicResistPerLevel: 0.3,
    // 经验值需求（指数增长）
    expBase: 100,
    expMultiplier: 1.15,
    // 计算升级所需经验
    getExpForLevel: function(level) {
      return Math.floor(this.expBase * Math.pow(this.expMultiplier, level - 1));
    },
    // 属性缩放（根据楼层调整）
    floorScaling: 1.0
  },

  // ==================== 怪物属性缩放 ====================
  monster: {
    // 每层属性增长
    hpPerFloor: 1.08,
    damagePerFloor: 1.06,
    armorPerFloor: 1.04,
    expPerFloor: 1.1,
    goldPerFloor: 1.08,
    // 计算缩放后的怪物属性
    scaleStats: function(baseStats, floor) {
      const multiplier = Math.pow(this.hpPerFloor, floor - 1);
      return {
        hp: Math.floor(baseStats.hp * multiplier),
        damage: Math.floor(baseStats.damage * Math.pow(this.damagePerFloor, floor - 1)),
        armor: Math.floor(baseStats.armor * Math.pow(this.armorPerFloor, floor - 1)),
        magicResist: Math.floor(baseStats.magicResist * Math.pow(this.armorPerFloor, floor - 1))
      };
    }
  },

  // ==================== 装备属性 ====================
  equipment: {
    // 品质倍率
    qualityMultiplier: {
      common: 1.0,
      uncommon: 1.2,
      rare: 1.5,
      epic: 1.8,
      legendary: 2.2,
      mythic: 2.8
    },
    // 强化加成
    enchantBonusPerLevel: 0.1,
    // 计算装备实际属性
    getStats: function(baseStats, quality, enchantLevel = 0) {
      const qualityMult = this.qualityMultiplier[quality] || 1.0;
      const enchantMult = 1 + enchantLevel * this.enchantBonusPerLevel;
      const result = {};
      for (const key in baseStats) {
        result[key] = Math.floor(baseStats[key] * qualityMult * enchantMult);
      }
      return result;
    }
  },

  // ==================== 技能伤害 ====================
  skills: {
    // 技能伤害系数
    damageCoefficient: {
      warrior: 1.2,  // 战士技能伤害较高
      mage: 1.5,     // 法师魔法伤害高但耗蓝
      ranger: 1.1,   // 游侠攻速快
      rogue: 1.3,    // 盗贼爆发高
      paladin: 1.0,  // 圣骑士均衡
      necromancer: 1.2 // 死灵法师持续伤害
    },
    // 魔法消耗
    manaCostBase: 10,
    manaCostPerLevel: 2,
    // 冷却时间（毫秒）
    cooldownBase: 3000,
    cooldownReductionPerLevel: 100
  },

  // ==================== 经济系统 ====================
  economy: {
    // 金币掉落范围
    goldDropMultiplier: 1.0,
    // 商店价格倍率
    shopPriceMultiplier: 1.0,
    // 修理费用
    repairCostPerDurability: 5,
    // 强化费用
    enchantCostBase: 100,
    enchantCostMultiplier: 1.5,
    // 计算强化费用
    getEnchantCost: function(currentLevel) {
      return Math.floor(this.enchantCostBase * Math.pow(this.enchantCostMultiplier, currentLevel));
    }
  },

  // ==================== 掉落率 ====================
  drops: {
    // 普通怪物掉落率
    normalDropRate: 0.15,
    // 精英怪物掉落率
    eliteDropRate: 0.5,
    // BOSS掉落率
    bossDropRate: 1.0,
    // 品质掉落权重
    qualityWeights: {
      common: 60,
      uncommon: 25,
      rare: 10,
      epic: 4,
      legendary: 1,
      mythic: 0.1
    },
    // 随机获取品质
    getRandomQuality: function() {
      const total = Object.values(this.qualityWeights).reduce((a, b) => a + b, 0);
      let random = Math.random() * total;
      for (const quality in this.qualityWeights) {
        random -= this.qualityWeights[quality];
        if (random <= 0) return quality;
      }
      return 'common';
    }
  },

  // ==================== 难度曲线 ====================
  difficulty: {
    // 每层难度增长
    floorDifficulty: 1.05,
    // BOSS难度倍率
    bossDifficultyMultiplier: 3.0,
    // 精英怪难度倍率
    eliteDifficultyMultiplier: 2.0,
    // 计算楼层难度
    getFloorDifficulty: function(floor) {
      return Math.pow(this.floorDifficulty, floor - 1);
    }
  },

  // ==================== 回复系统 ====================
  recovery: {
    // 生命回复（每秒）
    hpRegenBase: 1,
    hpRegenPerLevel: 0.2,
    // 魔法回复（每秒）
    mpRegenBase: 2,
    mpRegenPerLevel: 0.3,
    // 药水回复量
    healthPotionHeal: 50,
    manaPotionHeal: 40,
    // 战斗中回复倍率
    combatRegenMultiplier: 0.3
  },

  // ==================== 暴击系统 ====================
  critical: {
    // 基础暴击率
    baseCritChance: 0.05,
    // 暴击伤害倍率
    critDamageMultiplier: 2.0,
    // 每点敏捷增加暴击率
    critChancePerAgility: 0.002
  },

  // ==================== 闪避系统 ====================
  dodge: {
    // 基础闪避率
    baseDodgeChance: 0.03,
    // 每点敏捷增加闪避率
    dodgeChancePerAgility: 0.003,
    // 最大闪避率
    maxDodgeChance: 0.5
  },

  // ==================== 宠物系统 ====================
  pets: {
    // 宠物伤害倍率
    petDamageMultiplier: 0.5,
    // 宠物生命倍率
    petHpMultiplier: 0.8,
    // 宠物经验获取倍率
    petExpMultiplier: 0.7,
    // 抽卡概率
    gachaRates: {
      common: 0.40,
      uncommon: 0.25,
      rare: 0.18,
      epic: 0.10,
      legendary: 0.05,
      mythic: 0.02
    },
    // 保底抽数
    pityThreshold: 90
  },

  // ==================== 验证和调优函数 ====================

  // 验证玩家在某楼层的战斗力
  validatePlayerPower: function(level, floor) {
    const expectedHp = 100 + (level - 1) * this.player.hpPerLevel;
    const expectedDamage = 10 + (level - 1) * this.player.damagePerLevel;
    const floorDifficulty = this.difficulty.getFloorDifficulty(floor);

    return {
      playerPower: expectedHp * 0.5 + expectedDamage * 10,
      floorPower: 50 * floorDifficulty,
      ratio: (expectedHp * 0.5 + expectedDamage * 10) / (50 * floorDifficulty),
      recommendation: this.getDifficultyRecommendation(level, floor)
    };
  },

  // 获取难度建议
  getDifficultyRecommendation: function(level, floor) {
    const validation = this.validatePlayerPower(level, floor);
    if (validation.ratio > 2.0) return '轻松';
    if (validation.ratio > 1.2) return '适中';
    if (validation.ratio > 0.8) return '困难';
    if (validation.ratio > 0.5) return '极难';
    return '不建议';
  },

  // 平衡调整日志（用于调试）
  balanceLog: [],
  logBalance: function(category, message) {
    this.balanceLog.push({
      time: Date.now(),
      category,
      message
    });
    if (this.balanceLog.length > 100) {
      this.balanceLog.shift();
    }
  },

  // 导出平衡报告
  exportBalanceReport: function() {
    return {
      playerGrowth: this.player,
      monsterScaling: this.monster,
      equipment: this.equipment,
      skills: this.skills,
      economy: this.economy,
      drops: this.drops,
      difficulty: this.difficulty,
      recentLogs: this.balanceLog.slice(-20)
    };
  }
};
