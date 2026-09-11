// ==================== 永恒地牢 - 对话数据配置 ====================
// 定义所有NPC的对话树

const DialogueData = {
  // ==================== 城镇NPC ====================
  town_elder: {
    name: '村长',
    icon: '👴',
    dialogues: {
      default: [
        {
          text: '欢迎来到永恒村，年轻人。你是来挑战永恒地牢的吗？',
          options: [
            { text: '是的，我想进入地牢。', next: 'enter_dungeon' },
            { text: '这里是什么地方？', next: 'about_town' },
            { text: '有什么任务给我吗？', next: 'quests' },
            { text: '再见。', next: null }
          ]
        }
      ],
      enter_dungeon: [
        {
          text: '地牢里充满了危险，但也藏着无尽的宝藏。你准备好了吗？',
          options: [
            { text: '我准备好了！', next: 'ready' },
            { text: '我需要更多准备。', next: null }
          ]
        }
      ],
      ready: [
        {
          text: '很好！地牢入口就在村子的北边。记住，每5层就会有一个强大的BOSS。祝你好运，勇敢的冒险者！',
          options: [{ text: '谢谢！', next: null }]
        }
      ],
      about_town: [
        {
          text: '永恒村是地牢入口前的最后一个村庄。这里有铁匠、药剂师、商人，你可以在这里补给。地牢里的怪物越来越强，记得经常回来升级装备。',
          options: [
            { text: '地牢有多深？', next: 'dungeon_depth' },
            { text: '我知道了。', next: null }
          ]
        }
      ],
      dungeon_depth: [
        {
          text: '没有人知道地牢到底有多深。传说最深处沉睡着深渊之主，一个可以毁灭世界的存在。已经有无数冒险者进入地牢，但没有人到达过最深处...',
          options: [{ text: '我会成为第一个！', next: null }]
        }
      ],
      quests: [
        {
          text: '我确实有一些任务需要有人帮忙。你愿意接受吗？',
          options: [
            { text: '我愿意！', next: 'accept_quest' },
            { text: '让我考虑一下。', next: null }
          ]
        }
      ],
      accept_quest: [
        {
          text: '太好了！首先，去地牢里击杀3只史莱姆，证明你的实力。完成后回来找我。',
          options: [{ text: '遵命！', next: null, action: 'start_quest:main_enter_dungeon' }]
        }
      ]
    }
  },

  blacksmith: {
    name: '铁匠',
    icon: '🔨',
    dialogues: {
      default: [
        {
          text: '嘿，冒险者！需要打造或修理装备吗？我这里有最好的武器和护甲。',
          options: [
            { text: '看看你的商品。', next: 'shop' },
            { text: '有什么任务吗？', next: 'quest' },
            { text: '升级我的装备。', next: 'upgrade' },
            { text: '再见。', next: null }
          ]
        }
      ],
      shop: [
        {
          text: '这些都是我的得意之作，看看有没有你需要的。',
          options: [{ text: '打开商店', next: null, action: 'open_shop:weapon' }]
        }
      ],
      quest: [
        {
          text: '我正在研究一种新型合金，需要10个史莱姆凝胶作为材料。你能帮我收集吗？',
          options: [
            { text: '没问题！', next: null, action: 'start_quest:side_slime_hunt' },
            { text: '我还有事。', next: null }
          ]
        }
      ],
      upgrade: [
        {
          text: '升级装备需要对应的材料和金币。把材料给我，我可以帮你强化装备。',
          options: [{ text: '以后再说。', next: null }]
        }
      ]
    }
  },

  alchemist: {
    name: '药剂师',
    icon: '🧪',
    dialogues: {
      default: [
        {
          text: '欢迎来到我的药剂店！我这里有各种药水和毒药。',
          options: [
            { text: '买些药水。', next: 'shop' },
            { text: '有任务吗？', next: 'quest' },
            { text: '教我炼金。', next: 'craft' },
            { text: '再见。', next: null }
          ]
        }
      ],
      shop: [
        {
          text: '这些药水都是我亲手调制的，效果绝对有保证！',
          options: [{ text: '打开商店', next: null, action: 'open_shop:potion' }]
        }
      ],
      quest: [
        {
          text: '我需要一些蜘蛛丝和毒液来制作解毒剂。能帮我收集吗？',
          options: [
            { text: '包在我身上！', next: null, action: 'start_quest:side_herb_collection' },
            { text: '我考虑一下。', next: null }
          ]
        }
      ],
      craft: [
        {
          text: '炼金需要配方和材料。你可以在地牢里找到各种配方书。等你收集到材料，我可以教你制作。',
          options: [{ text: '我明白了。', next: null }]
        }
      ]
    }
  },

  merchant: {
    name: '商人',
    icon: '💰',
    dialogues: {
      default: [
        {
          text: '欢迎光临！我这里有各种稀有物品，绝对物超所值！',
          options: [
            { text: '看看有什么。', next: 'shop' },
            { text: '出售物品。', next: 'sell' },
            { text: '有任务吗？', next: 'quest' },
            { text: '再见。', next: null }
          ]
        }
      ],
      shop: [
        {
          text: '这些都是我从各地收集来的珍品，随便看！',
          options: [{ text: '打开商店', next: null, action: 'open_shop:general' }]
        }
      ],
      sell: [
        {
          text: '把你不需要的东西卖给我吧，我给你一个好价钱！',
          options: [{ text: '打开出售界面', next: null, action: 'open_sell' }]
        }
      ],
      quest: [
        {
          text: '我要去第8层收购一些稀有材料，你能保护我吗？报酬很丰厚哦！',
          options: [
            { text: '我来保护你！', next: null, action: 'start_quest:side_merchant_escort' },
            { text: '太危险了。', next: null }
          ]
        }
      ]
    }
  },

  priest: {
    name: '神父',
    icon: '⛪',
    dialogues: {
      default: [
        {
          text: '愿圣光保佑你，我的孩子。地牢里的亡灵需要被净化。',
          options: [
            { text: '祝福我吧。', next: 'bless' },
            { text: '有任务吗？', next: 'quest' },
            { text: '治疗我。', next: 'heal' },
            { text: '再见。', next: null }
          ]
        }
      ],
      bless: [
        {
          text: '愿圣光与你同在！你的攻击力暂时提升了。',
          options: [{ text: '感谢神父。', next: null, action: 'buff:damage:1.2:300' }]
        }
      ],
      quest: [
        {
          text: '地牢里的亡灵越来越多了。用这把银剑去净化30个亡灵生物吧。',
          options: [
            { text: '我会净化它们！', next: null, action: 'start_quest:side_undead_slayer' },
            { text: '我需要考虑。', next: null }
          ]
        }
      ],
      heal: [
        {
          text: '圣光治愈你的伤痛。你已经完全恢复了。',
          options: [{ text: '感谢！', next: null, action: 'full_heal' }]
        }
      ]
    }
  },

  arena_master: {
    name: '竞技场主人',
    icon: '⚔️',
    dialogues: {
      default: [
        {
          text: '欢迎来到永恒竞技场！你有信心成为冠军吗？',
          options: [
            { text: '我要挑战！', next: 'challenge' },
            { text: '查看排名。', next: 'ranking' },
            { text: '有任务吗？', next: 'quest' },
            { text: '再见。', next: null }
          ]
        }
      ],
      challenge: [
        {
          text: '选择你的对手：初级战士、中级骑士、高级剑圣？',
          options: [
            { text: '初级战士', next: null, action: 'arena:easy' },
            { text: '中级骑士', next: null, action: 'arena:normal' },
            { text: '高级剑圣', next: null, action: 'arena:hard' },
            { text: '算了', next: null }
          ]
        }
      ],
      ranking: [
        {
          text: '当前排名：1.屠龙者 2.暗影刺客 3.圣光骑士 ... 你还没有排名，去挑战吧！',
          options: [{ text: '我会努力的！', next: null }]
        }
      ],
      quest: [
        {
          text: '如果你能在竞技场连胜10场，我就承认你是真正的冠军！',
          options: [
            { text: '接受挑战！', next: null, action: 'start_quest:side_arena_champion' },
            { text: '太难了。', next: null }
          ]
        }
      ]
    }
  },

  treasure_hunter: {
    name: '寻宝猎人',
    icon: '🗺️',
    dialogues: {
      default: [
        {
          text: '嘿，伙计！我听说地牢里藏着无数宝藏，你想一起去找吗？',
          options: [
            { text: '有什么任务？', next: 'quest' },
            { text: '卖地图吗？', next: 'map' },
            { text: '再见。', next: null }
          ]
        }
      ],
      quest: [
        {
          text: '帮我打开10个宝箱，找到的宝物都归你，我只要其中的研究资料。怎么样？',
          options: [
            { text: '成交！', next: null, action: 'start_quest:side_treasure_hunt' },
            { text: '我考虑一下。', next: null }
          ]
        }
      ],
      map: [
        {
          text: '我这里有一些地牢的藏宝图，不过价格不菲。你要买吗？',
          options: [
            { text: '购买藏宝图（100金币）', next: null, action: 'buy_map:100' },
            { text: '太贵了。', next: null }
          ]
        }
      ]
    }
  },

  town_guard: {
    name: '守卫',
    icon: '💂',
    dialogues: {
      default: [
        {
          text: '站住！地牢入口就在前面，小心里面的怪物。',
          options: [
            { text: '有什么需要注意的？', next: 'warning' },
            { text: '有任务吗？', next: 'quest' },
            { text: '我知道了。', next: null }
          ]
        }
      ],
      warning: [
        {
          text: '地牢里的蝙蝠特别烦人，数量又多。如果你能帮忙清理一些，我们会很感激的。',
          options: [{ text: '我会帮忙的。', next: null }]
        }
      ],
      quest: [
        {
          text: '最近地牢里的蝙蝠越来越多，都快飞到村子里了。帮我们消灭20只蝙蝠吧！',
          options: [
            { text: '交给我！', next: null, action: 'start_quest:side_bat_extermination' },
            { text: '我还有事。', next: null }
          ]
        }
      ]
    }
  },

  // ==================== 地牢NPC ====================
  mysterious_merchant: {
    name: '神秘商人',
    icon: '🎭',
    dialogues: {
      default: [
        {
          text: '嘿嘿嘿...年轻人，我这里有一些特别的商品，有兴趣看看吗？',
          options: [
            { text: '看看有什么。', next: 'shop' },
            { text: '你是谁？', next: 'identity' },
            { text: '不了，谢谢。', next: null }
          ]
        }
      ],
      shop: [
        {
          text: '这些可都是稀有货，价格嘛...自然也不便宜。',
          options: [{ text: '打开神秘商店', next: null, action: 'open_shop:mystery' }]
        }
      ],
      identity: [
        {
          text: '我是谁不重要，重要的是我有你需要的东西。嘿嘿嘿...',
          options: [{ text: '好吧。', next: null }]
        }
      ]
    }
  },

  ghost_adventurer: {
    name: '冒险者亡灵',
    icon: '👻',
    dialogues: {
      default: [
        {
          text: '救救我...我被困在这里太久了...',
          options: [
            { text: '你怎么了？', next: 'story' },
            { text: '有什么可以帮你的？', next: 'help' },
            { text: '离开。', next: null }
          ]
        }
      ],
      story: [
        {
          text: '我曾经也是一个冒险者，在第10层被骷髅领主杀死了。我的灵魂被困在这里，无法安息...',
          options: [{ text: '我会为你报仇的。', next: null }]
        }
      ],
      help: [
        {
          text: '如果你能击败骷髅领主，我的灵魂就能得到解脱。这是我生前的装备，送给你吧...',
          options: [{ text: '谢谢你。', next: null, action: 'give_item:ghost_armor' }]
        }
      ]
    }
  },

  // ==================== BOSS对话 ====================
  boss_slime_king: {
    name: '史莱姆王',
    icon: '👑',
    dialogues: {
      start: [
        { text: '咕噜咕噜！人类，你竟敢闯入我的领地！准备受死吧！', options: [] }
      ],
      enrage: [
        { text: '不可能！我是无敌的史莱姆王！', options: [] }
      ],
      death: [
        { text: '咕噜...我不甘心...', options: [] }
      ]
    }
  },

  boss_skeleton_lord: {
    name: '骷髅领主',
    icon: '☠️',
    dialogues: {
      start: [
        { text: '又一个送死的人类。我的亡灵大军会把你撕碎！', options: [] }
      ],
      enrage: [
        { text: '你惹怒了我！感受死亡的恐惧吧！', options: [] }
      ],
      death: [
        { text: '不...我还不想死...', options: [] }
      ]
    }
  },

  boss_dragon: {
    name: '深渊巨龙',
    icon: '🐉',
    dialogues: {
      start: [
        { text: '渺小的人类，你敢挑战我？我会用龙息把你烧成灰烬！', options: [] }
      ],
      enrage: [
        { text: '你让我愤怒了！准备承受巨龙的怒火吧！', options: [] }
      ],
      death: [
        { text: '不可能...我是无敌的巨龙...', options: [] }
      ]
    }
  },

  boss_demon_lord: {
    name: '地狱恶魔',
    icon: '😈',
    dialogues: {
      start: [
        { text: '哈哈哈！又一个灵魂送到我面前了！', options: [] }
      ],
      enrage: [
        { text: '你会为此付出代价！地狱之火将吞噬你！', options: [] }
      ],
      death: [
        { text: '不...我还会回来的...', options: [] }
      ]
    }
  },

  boss_final: {
    name: '深渊之主',
    icon: '🌑',
    dialogues: {
      start: [
        { text: '终于...有人来到了这里。你准备好面对真相了吗？', options: [] }
      ],
      phase2: [
        { text: '不错的力量...但这还不够！', options: [] }
      ],
      phase3: [
        { text: '你让我认真起来了！感受深渊的力量吧！', options: [] }
      ],
      death: [
        { text: '原来...这就是死亡的感觉...谢谢你，让我得到了解脱...', options: [] }
      ]
    }
  },

  // ==================== 工具方法 ====================

  getDialogue(npcId, dialogueId = 'default') {
    const npc = this[npcId];
    if (!npc || !npc.dialogues) return null;
    return npc.dialogues[dialogueId] || npc.dialogues.default;
  },

  getNPCInfo(npcId) {
    const npc = this[npcId];
    if (!npc) return null;
    return { name: npc.name, icon: npc.icon };
  },

  getAllTownNPCs() {
    return ['town_elder', 'blacksmith', 'alchemist', 'merchant', 'priest', 'arena_master', 'treasure_hunter', 'town_guard'];
  }
};

window.DialogueData = DialogueData;
