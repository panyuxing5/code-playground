// ==================== 永恒地牢 - 任务系统 ====================
// 管理任务的接受、进度追踪、完成、奖励发放

const QuestSystem = {
  activeQuests: [],
  completedQuests: [],
  claimedQuests: [],
  dailyQuests: [],
  lastDailyReset: null,

  // 初始化
  init() {
    this.activeQuests = [];
    this.completedQuests = [];
    this.claimedQuests = [];
    this.dailyQuests = [];
    this.checkDailyReset();
    console.log('[QuestSystem] 任务系统初始化完成');
  },

  // 检查每日重置
  checkDailyReset() {
    const today = new Date().toDateString();
    if (this.lastDailyReset !== today) {
      this.lastDailyReset = today;
      this.dailyQuests = [];
      // 生成每日任务
      const dailyList = QuestData.getDailyQuests();
      for (const daily of dailyList) {
        this.dailyQuests.push(QuestData.createQuestInstance(daily.id));
      }
    }
  },

  // 接受任务
  acceptQuest(questId) {
    // 检查是否已经接受
    if (this.isQuestActive(questId)) {
      return { success: false, message: '任务已经在进行中' };
    }
    // 检查是否已经完成
    if (this.isQuestCompleted(questId)) {
      return { success: false, message: '任务已经完成' };
    }

    const quest = QuestData.createQuestInstance(questId);
    if (!quest) {
      return { success: false, message: '任务不存在' };
    }

    this.activeQuests.push(quest);
    console.log(`[Quest] 接受任务: ${quest.name}`);

    if (window.UIManager) {
      window.UIManager.showQuestNotification(quest, 'accepted');
    }

    return { success: true, quest };
  },

  // 更新任务进度
  updateProgress(type, target, count = 1) {
    const allQuests = [...this.activeQuests, ...this.dailyQuests];

    for (const quest of allQuests) {
      if (quest.completed) continue;

      for (const obj of quest.progress) {
        if (obj.completed) continue;

        let matches = false;
        switch (obj.type) {
          case 'kill':
            matches = obj.target === target;
            break;
          case 'kill_type':
            matches = this.checkMonsterType(target, obj.target);
            break;
          case 'kill_any':
          case 'kill_any_total':
            matches = true;
            break;
          case 'kill_boss':
            matches = obj.target === target;
            break;
          case 'collect':
            matches = obj.target === target;
            break;
          case 'reach_floor':
          case 'enter_floor':
            matches = true;
            obj.current = Math.max(obj.current, count);
            break;
          case 'explore_floors':
            matches = true;
            break;
          case 'open_chest':
            matches = true;
            break;
          case 'earn_gold':
            matches = true;
            break;
          case 'have_gold':
            matches = true;
            obj.current = count;
            break;
          case 'reach_level':
            matches = true;
            obj.current = Math.max(obj.current, count);
            break;
          case 'collect_unique':
            matches = true;
            break;
          case 'arena_wins':
            matches = true;
            break;
          case 'escort':
            matches = true;
            break;
          default:
            matches = false;
        }

        if (matches && obj.type !== 'reach_floor' && obj.type !== 'enter_floor' && obj.type !== 'reach_level' && obj.type !== 'have_gold') {
          obj.current += count;
        }

        if (obj.current >= obj.count && !obj.completed) {
          obj.completed = true;
          console.log(`[Quest] 目标完成: ${obj.description}`);
        }
      }

      // 检查任务是否完成
      if (this.isQuestComplete(quest) && !quest.completed) {
        quest.completed = true;
        this.onQuestComplete(quest);
      }
    }
  },

  // 检查怪物类型
  checkMonsterType(monsterId, type) {
    const monster = MonsterData.getMonster(monsterId);
    if (!monster) return false;
    return monster.type === type || monster.tags?.includes(type);
  },

  // 任务完成回调
  onQuestComplete(quest) {
    console.log(`[Quest] 任务完成: ${quest.name}`);
    if (window.UIManager) {
      window.UIManager.showQuestNotification(quest, 'completed');
    }
  },

  // 领取任务奖励
  claimReward(questId) {
    const quest = this.activeQuests.find(q => q.id === questId) ||
                  this.dailyQuests.find(q => q.id === questId);

    if (!quest || !quest.completed || quest.claimed) {
      return { success: false, message: '无法领取奖励' };
    }

    quest.claimed = true;

    // 发放奖励
    if (quest.rewards && window.Game && window.Game.player) {
      if (quest.rewards.exp) {
        window.Game.player.gainExp(quest.rewards.exp);
      }
      if (quest.rewards.gold) {
        window.Game.player.gold += quest.rewards.gold;
      }
      if (quest.rewards.items) {
        for (const itemId of quest.rewards.items) {
          window.Game.player.addToInventory(itemId);
        }
      }
    }

    // 移动到已完成列表
    const index = this.activeQuests.findIndex(q => q.id === questId);
    if (index !== -1) {
      this.activeQuests.splice(index, 1);
      this.completedQuests.push(quest);
    }

    console.log(`[Quest] 领取奖励: ${quest.name}`);
    return { success: true, rewards: quest.rewards };
  },

  // 检查任务是否完成
  isQuestComplete(quest) {
    return quest.progress.every(obj => obj.completed);
  },

  // 检查任务是否在进行中
  isQuestActive(questId) {
    return this.activeQuests.some(q => q.id === questId) ||
           this.dailyQuests.some(q => q.id === questId);
  },

  // 检查任务是否已完成
  isQuestCompleted(questId) {
    return this.completedQuests.some(q => q.id === questId) ||
           this.claimedQuests.some(q => q.id === questId);
  },

  // 获取进行中的任务
  getActiveQuests() {
    return [...this.activeQuests, ...this.dailyQuests.filter(q => !q.claimed)];
  },

  // 获取已完成任务
  getCompletedQuests() {
    return [...this.completedQuests, ...this.claimedQuests];
  },

  // 获取每日任务
  getDailyQuests() {
    return this.dailyQuests;
  },

  // 获取任务追踪（最多3个）
  getTrackedQuests() {
    return this.getActiveQuests().slice(0, 3);
  },

  // 放弃任务
  abandonQuest(questId) {
    const index = this.activeQuests.findIndex(q => q.id === questId);
    if (index !== -1) {
      const quest = this.activeQuests[index];
      if (quest.type === 'main') {
        return { success: false, message: '主线任务无法放弃' };
      }
      this.activeQuests.splice(index, 1);
      return { success: true };
    }
    return { success: false, message: '任务不存在' };
  },

  // 保存任务数据
  save() {
    return {
      activeQuests: this.activeQuests,
      completedQuests: this.completedQuests,
      claimedQuests: this.claimedQuests,
      dailyQuests: this.dailyQuests,
      lastDailyReset: this.lastDailyReset
    };
  },

  // 加载任务数据
  load(data) {
    if (!data) return;
    this.activeQuests = data.activeQuests || [];
    this.completedQuests = data.completedQuests || [];
    this.claimedQuests = data.claimedQuests || [];
    this.dailyQuests = data.dailyQuests || [];
    this.lastDailyReset = data.lastDailyReset;
    this.checkDailyReset();
  }
};

window.QuestSystem = QuestSystem;
