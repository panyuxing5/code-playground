// ==================== 永恒地牢 - 天赋系统 ====================
// 管理天赋树、天赋点分配、天赋效果

const TalentSystem = {
  talentPoints: 0,
  unlockedTalents: new Set(),
  talentTree: {},

  // 初始化
  init(classType) {
    this.talentPoints = 0;
    this.unlockedTalents = new Set();
    this.loadTalentTree(classType);
    console.log('[TalentSystem] 天赋系统初始化完成');
  },

  // 加载职业天赋树
  loadTalentTree(classType) {
    const classData = ClassData[classType];
    if (!classData || !classData.talentTree) {
      this.talentTree = {};
      return;
    }
    this.talentTree = Utils.deepClone(classData.talentTree);
  },

  // 添加天赋点
  addTalentPoints(points) {
    this.talentPoints += points;
    console.log(`[Talent] 获得 ${points} 天赋点，当前: ${this.talentPoints}`);
  },

  // 解锁天赋
  unlockTalent(talentId) {
    if (this.talentPoints <= 0) {
      return { success: false, message: '天赋点不足' };
    }

    const talent = this.findTalent(talentId);
    if (!talent) {
      return { success: false, message: '天赋不存在' };
    }

    if (this.unlockedTalents.has(talentId)) {
      return { success: false, message: '天赋已解锁' };
    }

    // 检查前置天赋
    if (talent.requires && !this.unlockedTalents.has(talent.requires)) {
      return { success: false, message: '需要先解锁前置天赋' };
    }

    this.talentPoints--;
    this.unlockedTalents.add(talentId);

    // 应用天赋效果
    this.applyTalentEffect(talent);

    console.log(`[Talent] 解锁天赋: ${talent.name}`);
    return { success: true, talent };
  },

  // 查找天赋
  findTalent(talentId) {
    for (const branch in this.talentTree) {
      for (const tier in this.talentTree[branch]) {
        const talents = this.talentTree[branch][tier];
        if (Array.isArray(talents)) {
          for (const t of talents) {
            if (t.id === talentId) return t;
          }
        } else if (talents.id === talentId) {
          return talents;
        }
      }
    }
    return null;
  },

  // 应用天赋效果
  applyTalentEffect(talent) {
    if (!talent.effects || !window.Game || !window.Game.player) return;

    const player = window.Game.player;
    for (const effect of talent.effects) {
      switch (effect.type) {
        case 'stat':
          if (player[effect.stat] !== undefined) {
            player[effect.stat] += effect.value;
          }
          break;
        case 'percent':
          if (player.talentModifiers) {
            player.talentModifiers[effect.stat] = (player.talentModifiers[effect.stat] || 1) * effect.value;
          }
          break;
        case 'skill_modify':
          if (player.skillModifiers) {
            if (!player.skillModifiers[effect.skill]) {
              player.skillModifiers[effect.skill] = {};
            }
            player.skillModifiers[effect.skill][effect.modifier] = effect.value;
          }
          break;
        case 'passive':
          if (player.passiveAbilities) {
            player.passiveAbilities.push(effect.ability);
          }
          break;
      }
    }
  },

  // 检查天赋是否解锁
  isTalentUnlocked(talentId) {
    return this.unlockedTalents.has(talentId);
  },

  // 获取天赋点
  getTalentPoints() {
    return this.talentPoints;
  },

  // 获取天赋树
  getTalentTree() {
    return this.talentTree;
  },

  // 获取已解锁天赋
  getUnlockedTalents() {
    return Array.from(this.unlockedTalents);
  },

  // 重置天赋
  resetTalents() {
    this.unlockedTalents = new Set();
    // 重新计算天赋点（每级1点）
    if (window.Game && window.Game.player) {
      this.talentPoints = window.Game.player.level - 1;
    }
    console.log('[Talent] 天赋已重置');
  },

  // 保存天赋数据
  save() {
    return {
      talentPoints: this.talentPoints,
      unlockedTalents: Array.from(this.unlockedTalents)
    };
  },

  // 加载天赋数据
  load(data) {
    if (!data) return;
    this.talentPoints = data.talentPoints || 0;
    this.unlockedTalents = new Set(data.unlockedTalents || []);
  }
};

window.TalentSystem = TalentSystem;
