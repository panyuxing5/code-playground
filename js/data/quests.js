// ==================== 永恒地牢 - 任务数据配置 ====================
// 定义所有任务、目标、奖励

const QuestData = {
  // ==================== 主线任务 ====================
  main_enter_dungeon: {
    id: 'main_enter_dungeon',
    name: '进入地牢',
    type: 'main',
    description: '探索永恒地牢的第一层，了解基本操作。',
    objectives: [
      { type: 'enter_floor', target: 1, description: '进入地牢第1层' },
      { type: 'kill', target: 'slime', count: 3, description: '击杀3只史莱姆' }
    ],
    rewards: {
      exp: 50,
      gold: 30,
      items: ['healthPotion', 'healthPotion']
    },
    nextQuest: 'main_first_boss',
    giver: 'town_elder',
    dialogueStart: '年轻人，你终于来了。永恒地牢已经吞噬了无数冒险者，你确定要进去吗？',
    dialogueComplete: '做得好！你已经证明了自己的实力。继续深入吧。'
  },
  main_first_boss: {
    id: 'main_first_boss',
    name: '史莱姆王',
    type: 'main',
    description: '击败地牢第5层的BOSS史莱姆王。',
    objectives: [
      { type: 'reach_floor', target: 5, description: '到达第5层' },
      { type: 'kill_boss', target: 'boss_slime_king', count: 1, description: '击败史莱姆王' }
    ],
    rewards: {
      exp: 200,
      gold: 150,
      items: ['steelSword', 'superHealthPotion']
    },
    nextQuest: 'main_skeleton_lord',
    giver: 'town_elder',
    dialogueStart: '史莱姆王是地牢第一个守护者，击败它你才能继续深入。',
    dialogueComplete: '不可思议！你居然击败了史莱姆王。接下来的敌人会更加强大。'
  },
  main_skeleton_lord: {
    id: 'main_skeleton_lord',
    name: '骷髅领主',
    type: 'main',
    description: '击败第10层的骷髅领主。',
    objectives: [
      { type: 'reach_floor', target: 10, description: '到达第10层' },
      { type: 'kill_boss', target: 'boss_skeleton_lord', count: 1, description: '击败骷髅领主' }
    ],
    rewards: {
      exp: 500,
      gold: 400,
      items: ['boneArmor', 'fullPotion', 'scrollOfTeleport']
    },
    nextQuest: 'main_dragon',
    giver: 'town_elder',
    dialogueStart: '骷髅领主统治着地牢的亡灵大军，小心它的召唤术。',
    dialogueComplete: '你已经成为了真正的勇士。龙穴就在前方。'
  },
  main_dragon: {
    id: 'main_dragon',
    name: '深渊巨龙',
    type: 'main',
    description: '击败第15层的深渊巨龙。',
    objectives: [
      { type: 'reach_floor', target: 15, description: '到达第15层' },
      { type: 'kill_boss', target: 'boss_dragon', count: 1, description: '击败深渊巨龙' }
    ],
    rewards: {
      exp: 1000,
      gold: 800,
      items: ['dragonScale', 'dragonSlayer']
    },
    nextQuest: 'main_demon_lord',
    giver: 'town_elder',
    dialogueStart: '深渊巨龙是远古生物，它的龙息可以融化一切。',
    dialogueComplete: '屠龙者！你的名字将被永远铭记。'
  },
  main_demon_lord: {
    id: 'main_demon_lord',
    name: '地狱恶魔',
    type: 'main',
    description: '击败第20层的地狱恶魔。',
    objectives: [
      { type: 'reach_floor', target: 20, description: '到达第20层' },
      { type: 'kill_boss', target: 'boss_demon_lord', count: 1, description: '击败地狱恶魔' }
    ],
    rewards: {
      exp: 2000,
      gold: 1500,
      items: ['demonHeart', 'hellfireBlade']
    },
    nextQuest: 'main_lich_king',
    giver: 'town_elder',
    dialogueStart: '地狱恶魔来自深渊，它的力量超乎想象。',
    dialogueComplete: '你已经接近地牢的最深处了。'
  },
  main_lich_king: {
    id: 'main_lich_king',
    name: '巫妖王',
    type: 'main',
    description: '击败第25层的巫妖王。',
    objectives: [
      { type: 'reach_floor', target: 25, description: '到达第25层' },
      { type: 'kill_boss', target: 'boss_lich_king', count: 1, description: '击败巫妖王' }
    ],
    rewards: {
      exp: 4000,
      gold: 3000,
      items: ['frostmourne', 'lichKingArmor']
    },
    nextQuest: 'main_abyss_lord',
    giver: 'town_elder',
    dialogueStart: '巫妖王是不死军团的统帅，它的亡灵大军无穷无尽。',
    dialogueComplete: '传说中的英雄！最后一个敌人在等着你。'
  },
  main_abyss_lord: {
    id: 'main_abyss_lord',
    name: '深渊之主',
    type: 'main',
    description: '击败地牢最深处的终极BOSS深渊之主。',
    objectives: [
      { type: 'reach_floor', target: 30, description: '到达第30层' },
      { type: 'kill_boss', target: 'boss_final', count: 1, description: '击败深渊之主' }
    ],
    rewards: {
      exp: 10000,
      gold: 10000,
      items: ['abyssBlade', 'abyssArmor', 'abyssRing']
    },
    nextQuest: null,
    giver: 'town_elder',
    dialogueStart: '深渊之主是地牢的创造者，击败它你将成为永恒的传说。',
    dialogueComplete: '恭喜你！你已经征服了永恒地牢，成为了真正的传奇！'
  },

  // ==================== 支线任务 ====================
  side_slime_hunt: {
    id: 'side_slime_hunt',
    name: '史莱姆狩猎',
    type: 'side',
    description: '为铁匠收集10个史莱姆凝胶。',
    objectives: [
      { type: 'collect', target: 'slimeGel', count: 10, description: '收集10个史莱姆凝胶' }
    ],
    rewards: {
      exp: 100,
      gold: 80,
      items: ['ironSword']
    },
    giver: 'blacksmith',
    dialogueStart: '嘿冒险者，帮我收集一些史莱姆凝胶吧，我需要它们来锻造武器。',
    dialogueComplete: '太好了！这些凝胶足够我打造一把好剑了。'
  },
  side_bat_extermination: {
    id: 'side_bat_extermination',
    name: '蝙蝠灭绝',
    type: 'side',
    description: '消灭20只蝙蝠。',
    objectives: [
      { type: 'kill', target: 'bat', count: 20, description: '击杀20只蝙蝠' }
    ],
    rewards: {
      exp: 120,
      gold: 60,
      items: ['agilityPotion']
    },
    giver: 'town_guard',
    dialogueStart: '地牢里的蝙蝠越来越多了，帮我们清理一下吧。',
    dialogueComplete: '干得好！现在地牢里安静多了。'
  },
  side_herb_collection: {
    id: 'side_herb_collection',
    name: '草药采集',
    type: 'side',
    description: '为药剂师收集5个蜘蛛丝和3个毒液。',
    objectives: [
      { type: 'collect', target: 'spiderSilk', count: 5, description: '收集5个蜘蛛丝' },
      { type: 'collect', target: 'poisonVial', count: 3, description: '收集3个毒液' }
    ],
    rewards: {
      exp: 150,
      gold: 100,
      items: ['antidote', 'antidote', 'cleanse']
    },
    giver: 'alchemist',
    dialogueStart: '我需要一些材料来制作解毒剂，能帮我收集吗？',
    dialogueComplete: '完美！这些材料足够我制作一批解毒剂了。'
  },
  side_undead_slayer: {
    id: 'side_undead_slayer',
    name: '亡灵杀手',
    type: 'side',
    description: '击杀30个亡灵生物（骷髅、僵尸、幽灵）。',
    objectives: [
      { type: 'kill_type', target: 'undead', count: 30, description: '击杀30个亡灵生物' }
    ],
    rewards: {
      exp: 300,
      gold: 200,
      items: ['silverSword']
    },
    giver: 'priest',
    dialogueStart: '亡灵的数量在不断增加，用这把银剑去净化它们吧。',
    dialogueComplete: '愿圣光保佑你，勇敢的冒险者。'
  },
  side_treasure_hunt: {
    id: 'side_treasure_hunt',
    name: '寻宝猎人',
    type: 'side',
    description: '打开10个宝箱。',
    objectives: [
      { type: 'open_chest', count: 10, description: '打开10个宝箱' }
    ],
    rewards: {
      exp: 200,
      gold: 150,
      items: ['key', 'key', 'scrollOfIdentify']
    },
    giver: 'treasure_hunter',
    dialogueStart: '我听说地牢里藏着很多宝藏，帮我找一些吧！',
    dialogueComplete: '哇，你找到了这么多！真是个天生的寻宝者。'
  },
  side_merchant_escort: {
    id: 'side_merchant_escort',
    name: '商人护送',
    type: 'side',
    description: '保护商人到达第8层。',
    objectives: [
      { type: 'escort', target: 'merchant', floor: 8, description: '护送商人到达第8层' }
    ],
    rewards: {
      exp: 250,
      gold: 300,
      items: ['tradeDiscount']
    },
    giver: 'merchant',
    dialogueStart: '我要去第8层收购材料，你能保护我吗？',
    dialogueComplete: '安全到达！这是你的报酬，以后买东西给你打折！'
  },
  side_arena_champion: {
    id: 'side_arena_champion',
    name: '竞技场冠军',
    type: 'side',
    description: '在竞技场连续获胜10场。',
    objectives: [
      { type: 'arena_wins', count: 10, description: '竞技场连胜10场' }
    ],
    rewards: {
      exp: 500,
      gold: 500,
      items: ['championBelt']
    },
    giver: 'arena_master',
    dialogueStart: '欢迎来到竞技场！你有信心成为冠军吗？',
    dialogueComplete: '冠军！你是真正的战士！'
  },

  // ==================== 每日任务 ====================
  daily_kill_50: {
    id: 'daily_kill_50',
    name: '每日猎杀',
    type: 'daily',
    description: '击杀50个怪物。',
    objectives: [
      { type: 'kill_any', count: 50, description: '击杀50个怪物' }
    ],
    rewards: {
      exp: 200,
      gold: 150,
      items: ['superHealthPotion']
    },
    resetDaily: true
  },
  daily_explore: {
    id: 'daily_explore',
    name: '每日探索',
    type: 'daily',
    description: '探索3层新地图。',
    objectives: [
      { type: 'explore_floors', count: 3, description: '探索3层新地图' }
    ],
    rewards: {
      exp: 150,
      gold: 100,
      items: ['scrollOfTeleport']
    },
    resetDaily: true
  },
  daily_loot: {
    id: 'daily_loot',
    name: '每日掠夺',
    type: 'daily',
    description: '获得1000金币。',
    objectives: [
      { type: 'earn_gold', count: 1000, description: '获得1000金币' }
    ],
    rewards: {
      exp: 180,
      gold: 200,
      items: ['luckyCharm']
    },
    resetDaily: true
  },

  // ==================== 成就任务 ====================
  achievement_first_blood: {
    id: 'achievement_first_blood',
    name: '初次杀戮',
    type: 'achievement',
    description: '击杀第一个怪物。',
    objectives: [
      { type: 'kill_any', count: 1, description: '击杀1个怪物' }
    ],
    rewards: { exp: 20, gold: 10 }
  },
  achievement_100_kills: {
    id: 'achievement_100_kills',
    name: '百人斩',
    type: 'achievement',
    description: '累计击杀100个怪物。',
    objectives: [
      { type: 'kill_any_total', count: 100, description: '累计击杀100个怪物' }
    ],
    rewards: { exp: 200, gold: 100 }
  },
  achievement_1000_kills: {
    id: 'achievement_1000_kills',
    name: '千人斩',
    type: 'achievement',
    description: '累计击杀1000个怪物。',
    objectives: [
      { type: 'kill_any_total', count: 1000, description: '累计击杀1000个怪物' }
    ],
    rewards: { exp: 1000, gold: 500 }
  },
  achievement_level_10: {
    id: 'achievement_level_10',
    name: '小有所成',
    type: 'achievement',
    description: '达到10级。',
    objectives: [
      { type: 'reach_level', count: 10, description: '达到10级' }
    ],
    rewards: { exp: 100, gold: 100 }
  },
  achievement_level_30: {
    id: 'achievement_level_30',
    name: '登峰造极',
    type: 'achievement',
    description: '达到30级。',
    objectives: [
      { type: 'reach_level', count: 30, description: '达到30级' }
    ],
    rewards: { exp: 500, gold: 500 }
  },
  achievement_rich: {
    id: 'achievement_rich',
    name: '富翁',
    type: 'achievement',
    description: '拥有10000金币。',
    objectives: [
      { type: 'have_gold', count: 10000, description: '拥有10000金币' }
    ],
    rewards: { exp: 300, gold: 0 }
  },
  achievement_collector: {
    id: 'achievement_collector',
    name: '收藏家',
    type: 'achievement',
    description: '收集50种不同的物品。',
    objectives: [
      { type: 'collect_unique', count: 50, description: '收集50种不同物品' }
    ],
    rewards: { exp: 400, gold: 300 }
  },
  achievement_explorer: {
    id: 'achievement_explorer',
    name: '探索者',
    type: 'achievement',
    description: '到达第20层。',
    objectives: [
      { type: 'reach_floor', count: 20, description: '到达第20层' }
    ],
    rewards: { exp: 500, gold: 400 }
  },
  achievement_boss_slayer: {
    id: 'achievement_boss_slayer',
    name: '屠龙勇士',
    type: 'achievement',
    description: '击败深渊巨龙。',
    objectives: [
      { type: 'kill_boss', target: 'boss_dragon', count: 1, description: '击败深渊巨龙' }
    ],
    rewards: { exp: 800, gold: 600 }
  },
  achievement_legend: {
    id: 'achievement_legend',
    name: '传奇',
    type: 'achievement',
    description: '击败深渊之主。',
    objectives: [
      { type: 'kill_boss', target: 'boss_final', count: 1, description: '击败深渊之主' }
    ],
    rewards: { exp: 5000, gold: 5000 }
  },

  // ==================== 工具方法 ====================

  // 获取任务
  getQuest(questId) {
    return this[questId] || null;
  },

  // 获取所有主线任务
  getMainQuests() {
    return Object.values(this).filter(q => q.type === 'main');
  },

  // 获取所有支线任务
  getSideQuests() {
    return Object.values(this).filter(q => q.type === 'side');
  },

  // 获取所有每日任务
  getDailyQuests() {
    return Object.values(this).filter(q => q.type === 'daily');
  },

  // 获取所有成就
  getAchievements() {
    return Object.values(this).filter(q => q.type === 'achievement');
  },

  // 创建任务实例
  createQuestInstance(questId) {
    const quest = this.getQuest(questId);
    if (!quest) return null;
    return {
      id: questId,
      ...Utils.deepClone(quest),
      progress: quest.objectives.map(o => ({ ...o, current: 0, completed: false })),
      completed: false,
      claimed: false,
      startedAt: Date.now()
    };
  },

  // 检查任务是否完成
  checkQuestComplete(questInstance) {
    return questInstance.progress.every(o => o.current >= o.count);
  }
};

window.QuestData = QuestData;
