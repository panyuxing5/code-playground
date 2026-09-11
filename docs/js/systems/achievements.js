// ==================== 永恒地牢 - 成就系统 ====================
// 管理所有成就的解锁、进度追踪、奖励发放

const AchievementSystem = {
  achievements: {},
  unlocked: new Set(),
  stats: {
    totalKills: 0,
    totalDamageDealt: 0,
    totalDamageTaken: 0,
    totalGoldEarned: 0,
    totalGoldSpent: 0,
    totalItemsCollected: 0,
    totalChestsOpened: 0,
    totalQuestsCompleted: 0,
    totalDeaths: 0,
    totalPlayTime: 0,
    highestFloor: 0,
    highestLevel: 0,
    bossesKilled: {},
    skillsUsed: 0,
    criticalHits: 0,
    potionsUsed: 0,
    distanceTraveled: 0
  },

  // 初始化
  init() {
    this.achievements = {};
    this.unlocked = new Set();
    this.loadAchievements();
    console.log('[AchievementSystem] 成就系统初始化完成');
  },

  // 加载所有成就定义
  loadAchievements() {
    const achievementList = QuestData.getAchievements();
    for (const ach of achievementList) {
      this.achievements[ach.id] = {
        ...ach,
        progress: 0,
        unlocked: false,
        unlockedAt: null
      };
    }

    // 添加额外成就
    const extraAchievements = [
      {
        id: 'first_death',
        name: '初次死亡',
        description: '第一次死亡。',
        objectives: [{ type: 'deaths', count: 1 }],
        rewards: { exp: 10, gold: 5 }
      },
      {
        id: 'survivor',
        name: '幸存者',
        description: '连续存活30分钟。',
        objectives: [{ type: 'playtime', count: 1800 }],
        rewards: { exp: 200, gold: 100 }
      },
      {
        id: 'wealthy',
        name: '富甲一方',
        description: '累计获得50000金币。',
        objectives: [{ type: 'gold_earned', count: 50000 }],
        rewards: { exp: 500, gold: 0 }
      },
      {
        id: 'collector_master',
        name: '收藏大师',
        description: '收集100种不同物品。',
        objectives: [{ type: 'unique_items', count: 100 }],
        rewards: { exp: 800, gold: 500 }
      },
      {
        id: 'boss_slayer_all',
        name: 'BOSS杀手',
        description: '击败所有BOSS。',
        objectives: [{ type: 'all_bosses', count: 6 }],
        rewards: { exp: 2000, gold: 2000 }
      },
      {
        id: 'speed_runner',
        name: '速通达人',
        description: '在1小时内到达第20层。',
        objectives: [{ type: 'speed_floor', count: 20 }],
        rewards: { exp: 1000, gold: 800 }
      },
      {
        id: 'pacifist',
        name: '和平主义者',
        description: '不杀任何怪物到达第5层。',
        objectives: [{ type: 'no_kill_floor', count: 5 }],
        rewards: { exp: 500, gold: 300 }
      },
      {
        id: 'alchemist',
        name: '炼金术士',
        description: '使用50瓶药水。',
        objectives: [{ type: 'potions', count: 50 }],
        rewards: { exp: 300, gold: 200 }
      },
      {
        id: 'crit_master',
        name: '暴击大师',
        description: '造成1000次暴击。',
        objectives: [{ type: 'crits', count: 1000 }],
        rewards: { exp: 600, gold: 400 }
      },
      {
        id: 'skill_master',
        name: '技能大师',
        description: '使用技能1000次。',
        objectives: [{ type: 'skills', count: 1000 }],
        rewards: { exp: 500, gold: 300 }
      },
      {
        id: 'explorer_master',
        name: '探索大师',
        description: '探索所有30层。',
        objectives: [{ type: 'all_floors', count: 30 }],
        rewards: { exp: 3000, gold: 2000 }
      },
      {
        id: 'perfect_run',
        name: '完美通关',
        description: '不死亡通关游戏。',
        objectives: [{ type: 'no_death_win', count: 1 }],
        rewards: { exp: 10000, gold: 10000 }
      }
    ];

    for (const ach of extraAchievements) {
      this.achievements[ach.id] = {
        ...ach,
        progress: 0,
        unlocked: false,
        unlockedAt: null
      };
    }
  },

  // 更新统计数据
  updateStat(stat, value = 1) {
    if (this.stats[stat] !== undefined) {
      this.stats[stat] += value;
      this.checkAchievements();
    }
  },

  // 设置统计数据
  setStat(stat, value) {
    if (this.stats[stat] !== undefined) {
      this.stats[stat] = value;
      this.checkAchievements();
    }
  },

  // 记录击杀
  recordKill(monsterType, isBoss = false) {
    this.stats.totalKills++;
    if (isBoss) {
      if (!this.stats.bossesKilled[monsterType]) {
        this.stats.bossesKilled[monsterType] = 0;
      }
      this.stats.bossesKilled[monsterType]++;
    }
    this.checkAchievements();
  },

  // 记录伤害
  recordDamageDealt(damage, isCrit = false) {
    this.stats.totalDamageDealt += damage;
    if (isCrit) this.stats.criticalHits++;
    this.checkAchievements();
  },

  // 记录金币
  recordGoldEarned(amount) {
    this.stats.totalGoldEarned += amount;
    this.checkAchievements();
  },

  // 记录楼层
  recordFloor(floor) {
    if (floor > this.stats.highestFloor) {
      this.stats.highestFloor = floor;
    }
    this.checkAchievements();
  },

  // 记录等级
  recordLevel(level) {
    if (level > this.stats.highestLevel) {
      this.stats.highestLevel = level;
    }
    this.checkAchievements();
  },

  // 检查所有成就
  checkAchievements() {
    for (const id in this.achievements) {
      const ach = this.achievements[id];
      if (ach.unlocked) continue;

      const progress = this.calculateProgress(ach);
      ach.progress = progress;

      if (progress >= 1) {
        this.unlockAchievement(id);
      }
    }
  },

  // 计算成就进度
  calculateProgress(achievement) {
    if (!achievement.objectives || achievement.objectives.length === 0) return 0;

    let totalProgress = 0;
    for (const obj of achievement.objectives) {
      const current = this.getObjectiveProgress(obj);
      totalProgress += Math.min(1, current / obj.count);
    }
    return totalProgress / achievement.objectives.length;
  },

  // 获取目标进度
  getObjectiveProgress(objective) {
    switch (objective.type) {
      case 'kill_any_total':
        return this.stats.totalKills;
      case 'reach_level':
        return this.stats.highestLevel;
      case 'reach_floor':
        return this.stats.highestFloor;
      case 'have_gold':
        return this.stats.totalGoldEarned;
      case 'gold_earned':
        return this.stats.totalGoldEarned;
      case 'collect_unique':
        return this.stats.totalItemsCollected;
      case 'unique_items':
        return this.stats.totalItemsCollected;
      case 'kill_boss':
        return this.stats.bossesKilled[objective.target] || 0;
      case 'all_bosses':
        return Object.keys(this.stats.bossesKilled).length;
      case 'deaths':
        return this.stats.totalDeaths;
      case 'playtime':
        return this.stats.totalPlayTime;
      case 'potions':
        return this.stats.potionsUsed;
      case 'crits':
        return this.stats.criticalHits;
      case 'skills':
        return this.stats.skillsUsed;
      case 'all_floors':
        return this.stats.highestFloor;
      default:
        return 0;
    }
  },

  // 解锁成就
  unlockAchievement(id) {
    const ach = this.achievements[id];
    if (!ach || ach.unlocked) return;

    ach.unlocked = true;
    ach.unlockedAt = Date.now();
    this.unlocked.add(id);

    // 发放奖励
    if (ach.rewards) {
      // 这里可以调用游戏主逻辑发放奖励
      console.log(`[Achievement] 解锁成就: ${ach.name}`);
      if (window.Game && window.Game.player) {
        if (ach.rewards.exp) window.Game.player.gainExp(ach.rewards.exp);
        if (ach.rewards.gold) window.Game.player.gold += ach.rewards.gold;
      }
    }

    // 显示成就通知
    if (window.UIManager) {
      window.UIManager.showAchievementNotification(ach);
    }
  },

  // 获取已解锁成就
  getUnlockedAchievements() {
    return Array.from(this.unlocked).map(id => this.achievements[id]);
  },

  // 获取所有成就（带进度）
  getAllAchievements() {
    return Object.values(this.achievements);
  },

  // 获取成就进度
  getAchievementProgress(id) {
    const ach = this.achievements[id];
    if (!ach) return 0;
    return ach.progress;
  },

  // 保存成就数据
  save() {
    return {
      unlocked: Array.from(this.unlocked),
      stats: this.stats,
      achievements: Object.fromEntries(
        Object.entries(this.achievements).map(([id, ach]) => [id, {
          progress: ach.progress,
          unlocked: ach.unlocked,
          unlockedAt: ach.unlockedAt
        }])
      )
    };
  },

  // 加载成就数据
  load(data) {
    if (!data) return;
    if (data.unlocked) {
      this.unlocked = new Set(data.unlocked);
    }
    if (data.stats) {
      Object.assign(this.stats, data.stats);
    }
    if (data.achievements) {
      for (const id in data.achievements) {
        if (this.achievements[id]) {
          Object.assign(this.achievements[id], data.achievements[id]);
        }
      }
    }
  },

  // 重置成就
  reset() {
    this.unlocked = new Set();
    for (const id in this.achievements) {
      this.achievements[id].progress = 0;
      this.achievements[id].unlocked = false;
      this.achievements[id].unlockedAt = null;
    }
    for (const key in this.stats) {
      if (typeof this.stats[key] === 'number') {
        this.stats[key] = 0;
      } else if (typeof this.stats[key] === 'object') {
        this.stats[key] = {};
      }
    }
  }
};

window.AchievementSystem = AchievementSystem;
