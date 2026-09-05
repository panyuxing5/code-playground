// ==================== 永恒地牢 - 竞技场系统 ====================
// 竞技场战斗、波次、排名、奖励

const ArenaData = {
  // ==================== 竞技场难度 ====================
  difficulties: {
    easy: {
      name: '初级',
      icon: '🟢',
      waves: 5,
      enemyLevelMod: 0,
      rewardMultiplier: 1,
      description: '适合新手的挑战'
    },
    normal: {
      name: '中级',
      icon: '🟡',
      waves: 10,
      enemyLevelMod: 3,
      rewardMultiplier: 2,
      description: '有一定难度的挑战'
    },
    hard: {
      name: '高级',
      icon: '🟠',
      waves: 15,
      enemyLevelMod: 6,
      rewardMultiplier: 3,
      description: '高手的挑战'
    },
    nightmare: {
      name: '噩梦',
      icon: '🔴',
      waves: 20,
      enemyLevelMod: 10,
      rewardMultiplier: 5,
      description: '只有最强者才能生存'
    },
    hell: {
      name: '地狱',
      icon: '💀',
      waves: 30,
      enemyLevelMod: 15,
      rewardMultiplier: 10,
      description: '传说中的死亡挑战'
    }
  },

  // ==================== 竞技场敌人组合 ====================
  wavePresets: {
    1: ['slime', 'slime', 'rat'],
    2: ['slime', 'bat', 'bat'],
    3: ['skeleton', 'slime', 'slime'],
    4: ['zombie', 'skeleton', 'bat'],
    5: ['goblin', 'goblin', 'slime'],
    6: ['spider', 'skeleton', 'skeleton'],
    7: ['orc', 'goblin', 'goblin'],
    8: ['werewolf', 'spider', 'spider'],
    9: ['golem', 'skeleton', 'zombie'],
    10: ['vampire', 'ghost', 'ghost'],
    11: ['minotaur', 'orc', 'orc'],
    12: ['demon', 'vampire', 'ghost'],
    13: ['darkKnight', 'golem', 'skeleton'],
    14: ['lich', 'ghost', 'ghost'],
    15: ['dragon', 'demon', 'vampire']
  },

  // 获取波次敌人
  getWaveEnemies(wave, difficulty) {
    const diff = this.difficulties[difficulty];
    const presetIndex = Math.min(wave, 15);
    const preset = this.wavePresets[presetIndex] || this.wavePresets[15];

    return preset.map(monsterId => ({
      monsterId,
      levelBonus: diff.enemyLevelMod + wave
    }));
  },

  // 计算波次奖励
  getWaveReward(wave, difficulty) {
    const diff = this.difficulties[difficulty];
    return {
      exp: Math.floor((50 + wave * 20) * diff.rewardMultiplier),
      gold: Math.floor((30 + wave * 15) * diff.rewardMultiplier),
      score: wave * 100 * diff.rewardMultiplier
    };
  },

  // 计算通关奖励
  getCompletionReward(difficulty) {
    const diff = this.difficulties[difficulty];
    return {
      exp: Math.floor(500 * diff.rewardMultiplier),
      gold: Math.floor(300 * diff.rewardMultiplier),
      score: 1000 * diff.rewardMultiplier,
      title: diff.name + '征服者'
    };
  }
};

// ==================== 竞技场管理器 ====================
const ArenaManager = {
  isInArena: false,
  currentDifficulty: null,
  currentWave: 0,
  totalWaves: 0,
  enemiesRemaining: 0,
  waveCleared: false,
  totalScore: 0,
  bestScores: {},
  winStreak: 0,
  arenaTimer: 0,
  waveDelay: 0,

  init() {
    this.isInArena = false;
    this.currentDifficulty = null;
    this.currentWave = 0;
    this.totalScore = 0;
    console.log('[ArenaManager] 竞技场系统初始化完成');
  },

  // 开始竞技场
  startArena(difficulty, game) {
    const diff = ArenaData.difficulties[difficulty];
    if (!diff) return { success: false, message: '难度不存在' };

    this.isInArena = true;
    this.currentDifficulty = difficulty;
    this.currentWave = 0;
    this.totalWaves = diff.waves;
    this.totalScore = 0;
    this.arenaTimer = 0;
    this.waveDelay = 2;

    // 清空怪物
    game.monsters = [];

    // 恢复玩家状态
    game.player.hp = game.player.maxHp;
    game.player.mp = game.player.maxMp;
    game.player.stamina = game.player.maxStamina;

    game.showMessage(`竞技场开始！难度: ${diff.name}，共 ${diff.waves} 波`);
    AudioSystem.playSound('boss');

    return { success: true };
  },

  // 开始下一波
  startNextWave(game) {
    this.currentWave++;
    if (this.currentWave > this.totalWaves) {
      this.completeArena(game);
      return;
    }

    const enemies = ArenaData.getWaveEnemies(this.currentWave, this.currentDifficulty);
    this.enemiesRemaining = enemies.length;
    this.waveCleared = false;

    // 生成敌人
    for (let i = 0; i < enemies.length; i++) {
      const angle = (i / enemies.length) * Math.PI * 2;
      const dist = 150;
      const x = game.player.x + Math.cos(angle) * dist;
      const y = game.player.y + Math.sin(angle) * dist;

      const monster = new Monster(x, y, enemies[i].monsterId, 1);
      // 增强敌人属性
      const levelBonus = enemies[i].levelBonus;
      monster.maxHp = Math.floor(monster.maxHp * (1 + levelBonus * 0.1));
      monster.hp = monster.maxHp;
      monster.damage = Math.floor(monster.damage * (1 + levelBonus * 0.1));
      game.monsters.push(monster);
    }

    game.showMessage(`第 ${this.currentWave}/${this.totalWaves} 波！敌人数量: ${enemies.length}`);
    AudioSystem.playSound('boss');
    ParticleSystem.explosion(game.player.x, game.player.y, '#e74c3c', 100, 15);
  },

  // 更新竞技场
  update(dt, game) {
    if (!this.isInArena) return;

    this.arenaTimer += dt;

    // 波次间隔
    if (this.waveDelay > 0) {
      this.waveDelay -= dt;
      if (this.waveDelay <= 0) {
        this.startNextWave(game);
      }
      return;
    }

    // 检查波次是否清空
    const aliveMonsters = game.monsters.filter(m => !m.dead).length;
    if (aliveMonsters === 0 && !this.waveCleared) {
      this.waveCleared = true;
      this.onWaveClear(game);
    }
  },

  // 波次清空
  onWaveClear(game) {
    const reward = ArenaData.getWaveReward(this.currentWave, this.currentDifficulty);
    this.totalScore += reward.score;

    game.player.gainExp(reward.exp);
    game.player.gold += reward.gold;

    game.showMessage(`第 ${this.currentWave} 波完成！获得 ${reward.exp} 经验，${reward.gold} 金币`);
    ParticleSystem.levelUp(game.player.x, game.player.y);
    AudioSystem.playSound('levelup');

    // 下一波延迟
    if (this.currentWave < this.totalWaves) {
      this.waveDelay = 3;
    } else {
      this.completeArena(game);
    }
  },

  // 完成竞技场
  completeArena(game) {
    const reward = ArenaData.getCompletionReward(this.currentDifficulty);
    this.totalScore += reward.score;

    game.player.gainExp(reward.exp);
    game.player.gold += reward.gold;
    this.winStreak++;

    // 更新最高分
    const diff = this.currentDifficulty;
    if (!this.bestScores[diff] || this.totalScore > this.bestScores[diff]) {
      this.bestScores[diff] = this.totalScore;
    }

    game.showMessage(`🏆 竞技场通关！总分: ${this.totalScore}，连胜: ${this.winStreak}`);
    ParticleSystem.explosion(game.player.x, game.player.y, '#f1c40f', 150, 30);
    AudioSystem.playSound('victory');

    this.isInArena = false;

    // 任务进度
    QuestSystem.updateProgress('arena_wins');
  },

  // 玩家死亡
  onPlayerDeath(game) {
    if (!this.isInArena) return;

    this.winStreak = 0;
    game.showMessage(`竞技场失败！坚持到第 ${this.currentWave} 波，得分: ${this.totalScore}`);
    this.isInArena = false;
  },

  // 退出竞技场
  exitArena(game) {
    this.isInArena = false;
    this.currentDifficulty = null;
    game.monsters = [];
    game.showMessage('已退出竞技场');
  },

  // 渲染竞技场HUD
  renderHUD(ctx, game) {
    if (!this.isInArena) return;

    const diff = ArenaData.difficulties[this.currentDifficulty];

    // 波次信息
    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(game.canvas.width / 2 - 150, 70, 300, 50);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 20px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(`${diff.icon} ${diff.name} 第 ${this.currentWave}/${this.totalWaves} 波`, game.canvas.width / 2, 95);

    // 分数
    ctx.fillStyle = '#fff';
    ctx.font = '14px Arial';
    ctx.fillText(`得分: ${this.totalScore} | 时间: ${Math.floor(this.arenaTimer)}s`, game.canvas.width / 2, 115);

    // 波次间隔提示
    if (this.waveDelay > 0) {
      ctx.fillStyle = 'rgba(0,0,0,0.8)';
      ctx.fillRect(game.canvas.width / 2 - 100, game.canvas.height / 2 - 30, 200, 60);
      ctx.fillStyle = '#2ecc71';
      ctx.font = 'bold 24px Arial';
      ctx.fillText(`下一波: ${Math.ceil(this.waveDelay)}s`, game.canvas.width / 2, game.canvas.height / 2 + 8);
    }
  },

  // 渲染竞技场选择界面
  renderMenu(ctx, game) {
    const x = 150;
    const y = 80;
    const width = game.canvas.width - 300;
    const height = game.canvas.height - 160;

    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#e74c3c';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    ctx.fillStyle = '#e74c3c';
    ctx.font = 'bold 36px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('⚔️ 竞技场 ⚔️', x + width / 2, y + 50);

    ctx.fillStyle = '#aaa';
    ctx.font = '16px Arial';
    ctx.fillText('选择难度开始挑战，击败所有波次获得丰厚奖励！', x + width / 2, y + 85);

    // 难度选择
    const difficulties = Object.entries(ArenaData.difficulties);
    difficulties.forEach(([key, diff], i) => {
      const dx = x + 50 + (i % 3) * ((width - 100) / 3);
      const dy = y + 120 + Math.floor(i / 3) * 150;
      const dw = (width - 120) / 3;
      const dh = 130;

      const bestScore = this.bestScores[key] || 0;

      ctx.fillStyle = 'rgba(231, 76, 60, 0.15)';
      ctx.fillRect(dx, dy, dw, dh);
      ctx.strokeStyle = '#e74c3c';
      ctx.lineWidth = 2;
      ctx.strokeRect(dx, dy, dw, dh);

      ctx.fillStyle = '#fff';
      ctx.font = 'bold 22px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(`${diff.icon} ${diff.name}`, dx + dw / 2, dy + 30);

      ctx.fillStyle = '#aaa';
      ctx.font = '13px Arial';
      ctx.fillText(diff.description, dx + dw / 2, dy + 55);
      ctx.fillText(`波数: ${diff.waves} | 奖励: x${diff.rewardMultiplier}`, dx + dw / 2, dy + 75);
      ctx.fillText(`最高分: ${bestScore}`, dx + dw / 2, dy + 95);

      ctx.fillStyle = '#e74c3c';
      ctx.fillRect(dx + dw / 2 - 50, dy + 100, 100, 25);
      ctx.fillStyle = '#fff';
      ctx.font = 'bold 14px Arial';
      ctx.fillText('开始挑战', dx + dw / 2, dy + 118);
    });

    // 连胜记录
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 18px Arial';
    ctx.fillText(`当前连胜: ${this.winStreak}`, x + width / 2, y + height - 50);

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.fillText('按 ESC 关闭', x + width / 2, y + height - 20);
  },

  // 处理点击
  handleClick(mouseX, mouseY, game) {
    const x = 150;
    const y = 80;
    const width = game.canvas.width - 300;
    const height = game.canvas.height - 160;

    const difficulties = Object.keys(ArenaData.difficulties);
    for (let i = 0; i < difficulties.length; i++) {
      const dx = x + 50 + (i % 3) * ((width - 100) / 3);
      const dy = y + 120 + Math.floor(i / 3) * 150;
      const dw = (width - 120) / 3;

      if (mouseX > dx + dw / 2 - 50 && mouseX < dx + dw / 2 + 50 &&
          mouseY > dy + 100 && mouseY < dy + 125) {
        return this.startArena(difficulties[i], game);
      }
    }
    return null;
  },

  // 保存
  save() {
    return {
      bestScores: this.bestScores,
      winStreak: this.winStreak
    };
  },

  // 加载
  load(data) {
    if (!data) return;
    this.bestScores = data.bestScores || {};
    this.winStreak = data.winStreak || 0;
  }
};

window.ArenaData = ArenaData;
window.ArenaManager = ArenaManager;
