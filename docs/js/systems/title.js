// ==================== 永恒地牢 - 称号系统 ====================
// 称号获取、装备、属性加成

const TitleData = {
  // ==================== 称号定义 ====================
  titles: {
    beginner: {
      id: 'beginner',
      name: '初心者',
      icon: '🌱',
      rarity: 'common',
      description: '刚刚踏上冒险之路的新手',
      requirement: { type: 'level', value: 1 },
      stats: {},
      color: '#2ecc71'
    },
    adventurer: {
      id: 'adventurer',
      name: '冒险者',
      icon: '🗺️',
      rarity: 'common',
      description: '经验丰富的冒险者',
      requirement: { type: 'level', value: 10 },
      stats: { str: 2, dex: 2, int: 2, vit: 2 },
      color: '#3498db'
    },
    veteran: {
      id: 'veteran',
      name: '老兵',
      icon: '🎖️',
      rarity: 'uncommon',
      description: '身经百战的老兵',
      requirement: { type: 'level', value: 20 },
      stats: { str: 5, vit: 5 },
      color: '#9b59b6'
    },
    legend: {
      id: 'legend',
      name: '传奇',
      icon: '🏆',
      rarity: 'legendary',
      description: '传说中的英雄',
      requirement: { type: 'level', value: 30 },
      stats: { str: 10, dex: 10, int: 10, vit: 10 },
      color: '#f1c40f'
    },
    slime_slayer: {
      id: 'slime_slayer',
      name: '史莱姆杀手',
      icon: '🟢',
      rarity: 'common',
      description: '击杀100只史莱姆',
      requirement: { type: 'kill', target: 'slime', value: 100 },
      stats: { damage: 3 },
      color: '#2ecc71'
    },
    dragon_slayer: {
      id: 'dragon_slayer',
      name: '屠龙者',
      icon: '🐲',
      rarity: 'epic',
      description: '击败深渊巨龙',
      requirement: { type: 'kill_boss', target: 'boss_dragon', value: 1 },
      stats: { damage: 15, crit: 0.05 },
      color: '#e67e22'
    },
    demon_slayer: {
      id: 'demon_slayer',
      name: '驱魔师',
      icon: '😈',
      rarity: 'epic',
      description: '击败地狱恶魔',
      requirement: { type: 'kill_boss', target: 'boss_demon_lord', value: 1 },
      stats: { damage: 12, fireResist: 20 },
      color: '#e74c3c'
    },
    dungeon_conqueror: {
      id: 'dungeon_conqueror',
      name: '地牢征服者',
      icon: '👑',
      rarity: 'mythic',
      description: '击败深渊之主，征服永恒地牢',
      requirement: { type: 'kill_boss', target: 'boss_final', value: 1 },
      stats: { str: 15, dex: 15, int: 15, vit: 15, damage: 20 },
      color: '#9b59b6'
    },
    rich: {
      id: 'rich',
      name: '富翁',
      icon: '💰',
      rarity: 'uncommon',
      description: '累计拥有10000金币',
      requirement: { type: 'gold', value: 10000 },
      stats: { goldBonus: 0.1 },
      color: '#f1c40f'
    },
    wealthy: {
      id: 'wealthy',
      name: '富豪',
      icon: '💎',
      rarity: 'rare',
      description: '累计拥有50000金币',
      requirement: { type: 'gold', value: 50000 },
      stats: { goldBonus: 0.2 },
      color: '#3498db'
    },
    collector: {
      id: 'collector',
      name: '收藏家',
      icon: '📦',
      rarity: 'uncommon',
      description: '收集50种不同物品',
      requirement: { type: 'unique_items', value: 50 },
      stats: { luck: 5 },
      color: '#9b59b6'
    },
    explorer: {
      id: 'explorer',
      name: '探索者',
      icon: '🧭',
      rarity: 'uncommon',
      description: '到达第20层',
      requirement: { type: 'floor', value: 20 },
      stats: { moveSpeed: 0.2 },
      color: '#2ecc71'
    },
    arena_champion: {
      id: 'arena_champion',
      name: '竞技场冠军',
      icon: '🏅',
      rarity: 'epic',
      description: '竞技场噩梦难度通关',
      requirement: { type: 'arena', difficulty: 'nightmare', value: 1 },
      stats: { damage: 10, crit: 0.05, critDamage: 0.2 },
      color: '#e74c3c'
    },
    arena_legend: {
      id: 'arena_legend',
      name: '竞技场传奇',
      icon: '🌟',
      rarity: 'legendary',
      description: '竞技场地狱难度通关',
      requirement: { type: 'arena', difficulty: 'hell', value: 1 },
      stats: { damage: 20, crit: 0.1, critDamage: 0.3, armor: 10 },
      color: '#f1c40f'
    },
    potion_master: {
      id: 'potion_master',
      name: '药水大师',
      icon: '🧪',
      rarity: 'uncommon',
      description: '使用100瓶药水',
      requirement: { type: 'potions', value: 100 },
      stats: { hpRegen: 3, mpRegen: 2 },
      color: '#2ecc71'
    },
    crit_master: {
      id: 'crit_master',
      name: '暴击大师',
      icon: '💥',
      rarity: 'rare',
      description: '造成500次暴击',
      requirement: { type: 'crits', value: 500 },
      stats: { crit: 0.08, critDamage: 0.2 },
      color: '#e67e22'
    },
    survivor: {
      id: 'survivor',
      name: '幸存者',
      icon: '🛡️',
      rarity: 'rare',
      description: '连续存活1小时',
      requirement: { type: 'playtime', value: 3600 },
      stats: { armor: 8, maxHp: 50 },
      color: '#3498db'
    },
    pacifist: {
      id: 'pacifist',
      name: '和平主义者',
      icon: '☮️',
      rarity: 'rare',
      description: '不杀任何怪物到达第10层',
      requirement: { type: 'no_kill_floor', value: 10 },
      stats: { dodge: 0.1, moveSpeed: 0.3 },
      color: '#2ecc71'
    },
    berserker: {
      id: 'berserker',
      name: '狂战士',
      icon: '🔥',
      rarity: 'epic',
      description: '战士职业达到30级',
      requirement: { type: 'class_level', class: 'warrior', value: 30 },
      stats: { damage: 25, lifesteal: 0.1 },
      color: '#e74c3c'
    },
    archmage: {
      id: 'archmage',
      name: '大法师',
      icon: '🔮',
      rarity: 'epic',
      description: '法师职业达到30级',
      requirement: { type: 'class_level', class: 'mage', value: 30 },
      stats: { magicDamage: 30, mpRegen: 5 },
      color: '#9b59b6'
    },
    shadow_assassin: {
      id: 'shadow_assassin',
      name: '暗影刺客',
      icon: '🗡️',
      rarity: 'epic',
      description: '盗贼职业达到30级',
      requirement: { type: 'class_level', class: 'rogue', value: 30 },
      stats: { crit: 0.15, dodge: 0.1, damage: 15 },
      color: '#2c3e50'
    },
    undead_lord: {
      id: 'undead_lord',
      name: '亡灵之主',
      icon: '💀',
      rarity: 'epic',
      description: '死灵法师职业达到30级',
      requirement: { type: 'class_level', class: 'necromancer', value: 30 },
      stats: { summonDamage: 0.3, maxMp: 100 },
      color: '#2c3e50'
    },
    speed_runner: {
      id: 'speed_runner',
      name: '速通达人',
      icon: '⚡',
      rarity: 'legendary',
      description: '1小时内通关游戏',
      requirement: { type: 'speed_run', value: 3600 },
      stats: { moveSpeed: 0.5, attackSpeed: 0.3 },
      color: '#f1c40f'
    },
    perfect: {
      id: 'perfect',
      name: '完美无瑕',
      icon: '✨',
      rarity: 'mythic',
      description: '不死亡通关游戏',
      requirement: { type: 'no_death_win', value: 1 },
      stats: { allStats: 20, damage: 30, crit: 0.15 },
      color: '#f1c40f'
    }
  },

  // 获取称号
  getTitle(titleId) {
    return this.titles[titleId] || null;
  },

  // 获取所有称号
  getAllTitles() {
    return Object.values(this.titles);
  },

  // 检查称号是否解锁
  isTitleUnlocked(titleId, stats) {
    const title = this.getTitle(titleId);
    if (!title) return false;

    const req = title.requirement;
    switch (req.type) {
      case 'level':
        return stats.level >= req.value;
      case 'kill':
        return (stats.killCounts?.[req.target] || 0) >= req.value;
      case 'kill_boss':
        return (stats.bossKills?.[req.target] || 0) >= req.value;
      case 'gold':
        return stats.totalGold >= req.value;
      case 'unique_items':
        return stats.uniqueItems >= req.value;
      case 'floor':
        return stats.highestFloor >= req.value;
      case 'arena':
        return (stats.arenaWins?.[req.difficulty] || 0) >= req.value;
      case 'potions':
        return stats.potionsUsed >= req.value;
      case 'crits':
        return stats.criticalHits >= req.value;
      case 'playtime':
        return stats.totalPlayTime >= req.value;
      case 'no_kill_floor':
        return stats.noKillFloor >= req.value;
      case 'class_level':
        return stats.classType === req.class && stats.level >= req.value;
      case 'speed_run':
        return stats.bestClearTime <= req.value;
      case 'no_death_win':
        return stats.noDeathWin >= req.value;
      default:
        return false;
    }
  }
};

// ==================== 称号管理器 ====================
const TitleManager = {
  unlockedTitles: new Set(),
  equippedTitle: null,
  titleStats: {},

  init() {
    this.unlockedTitles = new Set();
    this.equippedTitle = null;
    this.titleStats = {};
    console.log('[TitleManager] 称号系统初始化完成');
  },

  // 检查并解锁称号
  checkTitles(game) {
    const stats = {
      level: game.player?.level || 0,
      classType: game.player?.classType,
      totalKills: AchievementSystem.stats.totalKills || 0,
      killCounts: AchievementSystem.stats.killCounts || {},
      bossKills: AchievementSystem.stats.bossesKilled || {},
      totalGold: AchievementSystem.stats.totalGoldEarned || 0,
      uniqueItems: AchievementSystem.stats.totalItemsCollected || 0,
      highestFloor: AchievementSystem.stats.highestFloor || 0,
      potionsUsed: AchievementSystem.stats.potionsUsed || 0,
      criticalHits: AchievementSystem.stats.criticalHits || 0,
      totalPlayTime: AchievementSystem.stats.totalPlayTime || 0
    };

    for (const titleId in TitleData.titles) {
      if (!this.unlockedTitles.has(titleId)) {
        if (TitleData.isTitleUnlocked(titleId, stats)) {
          this.unlockTitle(titleId, game);
        }
      }
    }
  },

  // 解锁称号
  unlockTitle(titleId, game) {
    const title = TitleData.getTitle(titleId);
    if (!title) return;

    this.unlockedTitles.add(titleId);
    game.showMessage(`🎉 获得称号: ${title.icon} ${title.name}`);
    ParticleSystem.levelUp(game.player.x, game.player.y);
    AudioSystem.playSound('levelup');
  },

  // 装备称号
  equipTitle(titleId, game) {
    if (!this.unlockedTitles.has(titleId)) {
      return { success: false, message: '称号未解锁' };
    }

    const title = TitleData.getTitle(titleId);
    if (!title) return { success: false, message: '称号不存在' };

    // 卸下旧称号
    if (this.equippedTitle) {
      this.unequipTitle(game);
    }

    this.equippedTitle = titleId;
    this.titleStats = { ...title.stats };

    // 应用称号属性
    if (game.player) {
      game.player.calculateStats();
    }

    return { success: true, message: `装备称号: ${title.name}` };
  },

  // 卸下称号
  unequipTitle(game) {
    this.equippedTitle = null;
    this.titleStats = {};
    if (game.player) {
      game.player.calculateStats();
    }
  },

  // 获取称号属性加成
  getTitleStats() {
    return this.titleStats;
  },

  // 获取已解锁称号列表
  getUnlockedTitles() {
    return Array.from(this.unlockedTitles).map(id => TitleData.getTitle(id)).filter(t => t);
  },

  // 渲染称号界面
  renderUI(ctx, game) {
    const x = 100;
    const y = 60;
    const width = game.canvas.width - 200;
    const height = game.canvas.height - 120;

    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#f1c40f';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🏅 称号收藏', x + width / 2, y + 40);

    // 当前装备
    if (this.equippedTitle) {
      const title = TitleData.getTitle(this.equippedTitle);
      ctx.fillStyle = title.color;
      ctx.font = 'bold 18px Arial';
      ctx.fillText(`当前称号: ${title.icon} ${title.name}`, x + width / 2, y + 70);
    } else {
      ctx.fillStyle = '#888';
      ctx.font = '16px Arial';
      ctx.fillText('未装备称号', x + width / 2, y + 70);
    }

    // 称号列表
    const titles = TitleData.getAllTitles();
    const listX = x + 30;
    const listY = y + 100;
    const itemHeight = 45;
    const columns = 2;
    const itemWidth = (width - 80) / columns;

    titles.forEach((title, i) => {
      const col = i % columns;
      const row = Math.floor(i / columns);
      const tx = listX + col * itemWidth;
      const ty = listY + row * itemHeight;

      if (ty > y + height - 50) return;

      const unlocked = this.unlockedTitles.has(title.id);
      const equipped = this.equippedTitle === title.id;

      ctx.fillStyle = equipped ? 'rgba(241, 196, 15, 0.2)' : unlocked ? 'rgba(46, 204, 113, 0.1)' : 'rgba(0,0,0,0.3)';
      ctx.fillRect(tx, ty, itemWidth - 20, itemHeight - 5);

      ctx.globalAlpha = unlocked ? 1 : 0.4;
      ctx.fillStyle = title.color;
      ctx.font = '20px Arial';
      ctx.textAlign = 'left';
      ctx.fillText(title.icon, tx + 10, ty + 28);

      ctx.fillStyle = unlocked ? '#fff' : '#666';
      ctx.font = 'bold 14px Arial';
      ctx.fillText(title.name, tx + 40, ty + 18);

      ctx.fillStyle = '#aaa';
      ctx.font = '11px Arial';
      ctx.fillText(title.description.substring(0, 25), tx + 40, ty + 35);

      if (equipped) {
        ctx.fillStyle = '#f1c40f';
        ctx.font = 'bold 12px Arial';
        ctx.textAlign = 'right';
        ctx.fillText('已装备', tx + itemWidth - 35, ty + 25);
      }
      ctx.globalAlpha = 1;
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(`已解锁: ${this.unlockedTitles.size}/${titles.length} | 点击称号装备`, x + width / 2, y + height - 25);
  },

  // 处理点击
  handleClick(mouseX, mouseY, game) {
    const x = 100;
    const y = 60;
    const width = game.canvas.width - 200;
    const height = game.canvas.height - 120;

    const titles = TitleData.getAllTitles();
    const listX = x + 30;
    const listY = y + 100;
    const itemHeight = 45;
    const columns = 2;
    const itemWidth = (width - 80) / columns;

    for (let i = 0; i < titles.length; i++) {
      const col = i % columns;
      const row = Math.floor(i / columns);
      const tx = listX + col * itemWidth;
      const ty = listY + row * itemHeight;

      if (mouseX > tx && mouseX < tx + itemWidth - 20 &&
          mouseY > ty && mouseY < ty + itemHeight - 5) {
        if (this.unlockedTitles.has(titles[i].id)) {
          if (this.equippedTitle === titles[i].id) {
            this.unequipTitle(game);
          } else {
            this.equipTitle(titles[i].id, game);
          }
        }
        return;
      }
    }
  },

  // 保存
  save() {
    return {
      unlockedTitles: Array.from(this.unlockedTitles),
      equippedTitle: this.equippedTitle
    };
  },

  // 加载
  load(data) {
    if (!data) return;
    this.unlockedTitles = new Set(data.unlockedTitles || []);
    this.equippedTitle = data.equippedTitle || null;
    if (this.equippedTitle) {
      const title = TitleData.getTitle(this.equippedTitle);
      if (title) {
        this.titleStats = { ...title.stats };
      }
    }
  }
};

window.TitleData = TitleData;
window.TitleManager = TitleManager;
