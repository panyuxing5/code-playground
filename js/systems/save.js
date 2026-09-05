// ==================== 永恒地牢 - 存档系统 ====================
// 管理游戏存档的保存、加载、删除

const SaveSystem = {
  SAVE_KEY: 'eternal_dungeon_save',
  MAX_SAVES: 5,
  currentSaveSlot: 0,

  // 初始化
  init() {
    console.log('[SaveSystem] 存档系统初始化完成');
  },

  // 保存游戏
  saveGame(slot = 0) {
    if (!window.Game || !window.Game.player) {
      return { success: false, message: '游戏未初始化' };
    }

    const saveData = {
      version: '1.1.0',
      timestamp: Date.now(),
      player: this.serializePlayer(window.Game.player),
      gameState: this.serializeGameState(),
      inventory: this.serializeInventory(),
      equipment: this.serializeEquipment(),
      skills: this.serializeSkills(),
      talents: TalentSystem.save(),
      quests: QuestSystem.save(),
      achievements: AchievementSystem.save(),
      settings: this.serializeSettings(),
      stats: { ...AchievementSystem.stats },
      // 新增数据
      pets: typeof PetManager !== 'undefined' ? PetManager.save() : null,
      story: typeof StorySystem !== 'undefined' ? StorySystem.save() : null,
      defeatedBosses: window.Game.defeatedBosses || [],
      titles: typeof TitleManager !== 'undefined' ? TitleManager.save() : null,
      crafting: typeof CraftingManager !== 'undefined' ? CraftingManager.save() : null
    };

    try {
      const saves = this.getAllSaves();
      saves[slot] = saveData;
      localStorage.setItem(this.SAVE_KEY, JSON.stringify(saves));
      console.log(`[Save] 游戏已保存到槽位 ${slot}`);
      return { success: true, slot, timestamp: saveData.timestamp };
    } catch (e) {
      console.error('[Save] 保存失败:', e);
      return { success: false, message: e.message };
    }
  },

  // 加载游戏
  loadGame(slot = 0) {
    try {
      const saves = this.getAllSaves();
      const saveData = saves[slot];
      if (!saveData) {
        return { success: false, message: '存档不存在' };
      }

      // 验证版本
      if (saveData.version !== '1.0.0') {
        console.warn('[Save] 存档版本不兼容');
      }

      this.currentSaveSlot = slot;

      // 恢复玩家数据
      if (window.Game && window.Game.player) {
        this.deserializePlayer(window.Game.player, saveData.player);
      }

      // 恢复游戏状态
      this.deserializeGameState(saveData.gameState);

      // 恢复系统数据
      TalentSystem.load(saveData.talents);
      QuestSystem.load(saveData.quests);
      AchievementSystem.load(saveData.achievements);

      // 恢复新增数据
      if (saveData.pets && typeof PetManager !== 'undefined') {
        PetManager.load(saveData.pets);
      }
      if (saveData.story && typeof StorySystem !== 'undefined') {
        StorySystem.load(saveData.story);
      }
      if (saveData.defeatedBosses) {
        window.Game.defeatedBosses = saveData.defeatedBosses;
      }
      if (saveData.titles && typeof TitleManager !== 'undefined') {
        TitleManager.load(saveData.titles);
      }
      if (saveData.crafting && typeof CraftingManager !== 'undefined') {
        CraftingManager.load(saveData.crafting);
      }

      // 恢复设置
      if (saveData.settings) {
        this.deserializeSettings(saveData.settings);
      }

      console.log(`[Save] 游戏已从槽位 ${slot} 加载`);
      return { success: true, slot, saveData };
    } catch (e) {
      console.error('[Save] 加载失败:', e);
      return { success: false, message: e.message };
    }
  },

  // 删除存档
  deleteSave(slot) {
    try {
      const saves = this.getAllSaves();
      if (saves[slot]) {
        delete saves[slot];
        localStorage.setItem(this.SAVE_KEY, JSON.stringify(saves));
        console.log(`[Save] 存档 ${slot} 已删除`);
        return { success: true };
      }
      return { success: false, message: '存档不存在' };
    } catch (e) {
      return { success: false, message: e.message };
    }
  },

  // 获取所有存档
  getAllSaves() {
    try {
      const data = localStorage.getItem(this.SAVE_KEY);
      return data ? JSON.parse(data) : {};
    } catch (e) {
      return {};
    }
  },

  // 获取存档列表
  getSaveList() {
    const saves = this.getAllSaves();
    const list = [];
    for (let i = 0; i < this.MAX_SAVES; i++) {
      if (saves[i]) {
        list.push({
          slot: i,
          timestamp: saves[i].timestamp,
          playerName: saves[i].player?.name || '未知',
          playerClass: saves[i].player?.classType || '未知',
          level: saves[i].player?.level || 1,
          floor: saves[i].gameState?.currentFloor || 1,
          playTime: saves[i].stats?.totalPlayTime || 0
        });
      } else {
        list.push({ slot: i, empty: true });
      }
    }
    return list;
  },

  // 检查存档是否存在
  hasSave(slot) {
    const saves = this.getAllSaves();
    return !!saves[slot];
  },

  // 自动保存
  autoSave() {
    return this.saveGame(this.currentSaveSlot);
  },

  // 自动保存定时器
  autoSaveInterval: null,
  autoSaveIntervalMs: 60000, // 每分钟自动保存

  // 启动自动保存
  startAutoSave: function() {
    if (this.autoSaveInterval) return;
    this.autoSaveInterval = setInterval(() => {
      if (window.Game && window.Game.state === 'playing') {
        this.autoSave();
      }
    }, this.autoSaveIntervalMs);
    console.log('[Save] 自动保存已启动，每60秒保存一次');
  },

  // 停止自动保存
  stopAutoSave: function() {
    if (this.autoSaveInterval) {
      clearInterval(this.autoSaveInterval);
      this.autoSaveInterval = null;
    }
  },

  // 导出存档（备份）
  exportSave: function(slot = 0) {
    const saves = this.getAllSaves();
    if (!saves[slot]) return null;
    return JSON.stringify(saves[slot]);
  },

  // 导入存档（恢复）
  importSave: function(saveString, slot = 0) {
    try {
      const saveData = JSON.parse(saveString);
      if (!saveData.version || !saveData.player) {
        return { success: false, message: '无效的存档文件' };
      }
      const saves = this.getAllSaves();
      saves[slot] = saveData;
      localStorage.setItem(this.SAVE_KEY, JSON.stringify(saves));
      return { success: true };
    } catch (e) {
      return { success: false, message: e.message };
    }
  },

  // ==================== 序列化方法 ====================

  serializePlayer(player) {
    return {
      name: player.name,
      classType: player.classType,
      level: player.level,
      exp: player.exp,
      hp: player.hp,
      maxHp: player.maxHp,
      mp: player.mp,
      maxMp: player.maxMp,
      stamina: player.stamina,
      maxStamina: player.maxStamina,
      x: player.x,
      y: player.y,
      gold: player.gold,
      stats: { ...player.baseStats },
      skillPoints: player.skillPoints,
      attributePoints: player.attributePoints
    };
  },

  deserializePlayer(player, data) {
    if (!data) return;
    player.name = data.name;
    player.classType = data.classType;
    player.level = data.level;
    player.exp = data.exp;
    player.hp = data.hp;
    player.maxHp = data.maxHp;
    player.mp = data.mp;
    player.maxMp = data.maxMp;
    player.stamina = data.stamina;
    player.maxStamina = data.maxStamina;
    player.x = data.x;
    player.y = data.y;
    player.gold = data.gold;
    if (data.stats) {
      player.baseStats = { ...data.stats };
    }
    player.skillPoints = data.skillPoints || 0;
    player.attributePoints = data.attributePoints || 0;
  },

  serializeGameState() {
    return {
      currentFloor: window.Game.currentFloor || 1,
      gameTime: window.Game.gameTime || 0,
      difficulty: window.Game.difficulty || 'normal',
      paused: window.Game.paused || false
    };
  },

  deserializeGameState(data) {
    if (!data || !window.Game) return;
    window.Game.currentFloor = data.currentFloor || 1;
    window.Game.gameTime = data.gameTime || 0;
    window.Game.difficulty = data.difficulty || 'normal';
  },

  serializeInventory() {
    if (!window.Game.player || !window.Game.player.inventory) return [];
    return window.Game.player.inventory.map(item => ({
      id: item.id,
      quantity: item.quantity || 1
    }));
  },

  serializeEquipment() {
    if (!window.Game.player || !window.Game.player.equipment) return {};
    const eq = {};
    for (const slot in window.Game.player.equipment) {
      if (window.Game.player.equipment[slot]) {
        eq[slot] = window.Game.player.equipment[slot].id;
      }
    }
    return eq;
  },

  serializeSkills() {
    if (!window.Game.player || !window.Game.player.skills) return [];
    return window.Game.player.skills.map(s => ({
      id: s.id,
      level: s.level || 1
    }));
  },

  serializeSettings() {
    return {
      volume: window.Game?.settings?.volume || 0.5,
      musicVolume: window.Game?.settings?.musicVolume || 0.3,
      sfxVolume: window.Game?.settings?.sfxVolume || 0.7,
      difficulty: window.Game?.difficulty || 'normal',
      showDamageNumbers: window.Game?.settings?.showDamageNumbers !== false,
      screenShake: window.Game?.settings?.screenShake !== false
    };
  },

  deserializeSettings(data) {
    if (!data || !window.Game) return;
    window.Game.settings = { ...window.Game.settings, ...data };
  },

  // 导出存档
  exportSave(slot) {
    const saves = this.getAllSaves();
    if (!saves[slot]) return null;
    return btoa(unescape(encodeURIComponent(JSON.stringify(saves[slot]))));
  },

  // 导入存档
  importSave(slot, encodedData) {
    try {
      const saveData = JSON.parse(decodeURIComponent(escape(atob(encodedData))));
      const saves = this.getAllSaves();
      saves[slot] = saveData;
      localStorage.setItem(this.SAVE_KEY, JSON.stringify(saves));
      return { success: true };
    } catch (e) {
      return { success: false, message: '存档数据无效' };
    }
  },

  // 清除所有存档
  clearAllSaves() {
    localStorage.removeItem(this.SAVE_KEY);
    console.log('[Save] 所有存档已清除');
  }
};

window.SaveSystem = SaveSystem;
