// ==================== 永恒地牢 - 剧情系统 ====================
// 主线剧情、章节、过场动画、剧情触发

const StorySystem = {
  // 剧情章节定义
  chapters: {
    prologue: {
      id: 'prologue',
      title: '序章：永恒的召唤',
      unlockCondition: null, // 游戏开始即触发
      scenes: [
        {
          type: 'narration',
          text: '在这片大陆的中央，矗立着一座神秘的地牢。',
          duration: 3000
        },
        {
          type: 'narration',
          text: '传说它的最深处沉睡着混沌之主，一个足以毁灭世界的存在。',
          duration: 3000
        },
        {
          type: 'narration',
          text: '千百年来，无数冒险者踏入地牢，却无人归来。',
          duration: 3000
        },
        {
          type: 'narration',
          text: '而你，是最新的挑战者。你的故事，从这里开始...',
          duration: 3000
        }
      ]
    },

    chapter1: {
      id: 'chapter1',
      title: '第一章：地牢入口',
      unlockCondition: { type: 'floor', value: 1 },
      scenes: [
        {
          type: 'dialogue',
          speaker: '村长',
          text: '年轻人，你终于来了。地牢的入口就在前方，但我必须警告你——里面的危险远超你的想象。',
          options: [
            { text: '我不怕危险。', next: 'brave' },
            { text: '有什么建议吗？', next: 'advice' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '村长',
          id: 'brave',
          text: '勇气可嘉！但记住，活着回来比什么都重要。地牢第5层有一只史莱姆之王，那是你遇到的第一个真正挑战。',
          options: [{ text: '我记住了。', next: null }]
        },
        {
          type: 'dialogue',
          speaker: '村长',
          id: 'advice',
          text: '多收集药水，注意躲避远程攻击，遇到打不过的就跑。还有，每5层的BOSS都有特殊机制，仔细观察它们的攻击模式。',
          options: [{ text: '谢谢指点！', next: null }]
        }
      ]
    },

    chapter2: {
      id: 'chapter2',
      title: '第二章：亡灵之地',
      unlockCondition: { type: 'floor', value: 6 },
      scenes: [
        {
          type: 'narration',
          text: '你击败了史莱姆之王，继续深入地牢。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '空气变得阴冷，周围开始出现骷髅和僵尸的踪迹。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '神秘旅人',
          text: '等等，冒险者！你能看到我？看来你确实有特殊的体质。我是一千年前进入地牢的冒险者，如今只剩下灵魂徘徊在这里。',
          options: [
            { text: '一千年前？发生了什么？', next: 'history' },
            { text: '你能帮我吗？', next: 'help' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '神秘旅人',
          id: 'history',
          text: '当年我和我的队伍一路杀到第25层，却在巫妖王面前全军覆没。它的冰霜魔法太强大了...如果你要继续前进，一定要准备好火焰抗性的装备。',
          options: [{ text: '我会小心的。', next: null }]
        },
        {
          type: 'dialogue',
          speaker: '神秘旅人',
          id: 'help',
          text: '我可以告诉你每个BOSS的弱点。史莱姆之王怕火，骷髅领主会召唤小怪要优先清理，巨龙的龙息需要躲到它身后。记住这些，你会走得更远。',
          options: [{ text: '多谢！', next: null, action: 'give_item:fireResistPotion' }]
        }
      ]
    },

    chapter3: {
      id: 'chapter3',
      title: '第三章：龙之巢穴',
      unlockCondition: { type: 'floor', value: 11 },
      scenes: [
        {
          type: 'narration',
          text: '你穿过了亡灵之地，来到了一片炽热的区域。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '墙壁上的火焰在燃烧，远处传来巨龙的咆哮。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '远古巨龙',
          text: '又一个渺小的人类来到我的领地。你是来送死的吗？',
          options: [
            { text: '我是来击败你的！', next: 'challenge' },
            { text: '我只是路过。', next: 'pass' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '远古巨龙',
          id: 'challenge',
          text: '哈哈哈哈！好久没有人敢这么跟我说话了。很好，我会让你死得痛快一点。准备好面对我的烈焰吧！',
          options: [{ text: '来吧！', next: null, action: 'start_boss:boss_dragon' }]
        },
        {
          type: 'dialogue',
          speaker: '远古巨龙',
          id: 'pass',
          text: '路过？在我的巢穴里路过？你以为我会相信吗？既然来了，就别想走了！',
          options: [{ text: '那就战吧！', next: null, action: 'start_boss:boss_dragon' }]
        }
      ]
    },

    chapter4: {
      id: 'chapter4',
      title: '第四章：地狱深渊',
      unlockCondition: { type: 'floor', value: 16 },
      scenes: [
        {
          type: 'narration',
          text: '巨龙倒下了，但地牢还在继续。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '周围的环境变得扭曲，空气中弥漫着硫磺的味道。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '堕落牧师',
          text: '你...你还活着？看来恶魔领主的手下都是废物。不过没关系，你会在这里终结。',
          options: [
            { text: '你是谁？', next: 'who' },
            { text: '让开！', next: 'move' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '堕落牧师',
          id: 'who',
          text: '我曾经是光明教会的牧师，直到我发现了真相——所谓的神不过是更强大的恶魔。现在我侍奉真正的主人，恶魔领主。你也应该加入我们。',
          options: [
            { text: '我绝不会堕落！', next: 'refuse' },
            { text: '...让我想想。', next: 'think' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '堕落牧师',
          id: 'refuse',
          text: '冥顽不灵！那就让你看看地狱的力量！',
          options: [{ text: '来吧！', next: null, action: 'spawn_enemy:imp' }]
        },
        {
          type: 'dialogue',
          speaker: '堕落牧师',
          id: 'think',
          text: '犹豫了？很好，这就是堕落的开始。不过恶魔领主不需要犹豫的人，你还是去死吧！',
          options: [{ text: '等等！', next: null, action: 'spawn_enemy:imp' }]
        },
        {
          type: 'dialogue',
          speaker: '堕落牧师',
          id: 'move',
          text: '让开？这是恶魔领主的领地，你以为你是谁？受死吧！',
          options: [{ text: '那就别怪我不客气了！', next: null, action: 'spawn_enemy:imp' }]
        }
      ]
    },

    chapter5: {
      id: 'chapter5',
      title: '第五章：冰封王座',
      unlockCondition: { type: 'floor', value: 21 },
      scenes: [
        {
          type: 'narration',
          text: '你击败了恶魔领主，继续向地牢深处前进。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '温度骤降，一切都被冰雪覆盖。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '巫妖王',
          text: '欢迎来到我的宫殿，凡人。你比我想象的走得更远。不过，你的旅程到此为止了。',
          options: [
            { text: '巫妖王！我要终结你的统治！', next: 'fight' },
            { text: '为什么要做这些？', next: 'why' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '巫妖王',
          id: 'fight',
          text: '终结我？哈哈哈哈！多少英雄说过同样的话，如今他们都成了我的亡灵大军的一员。加入他们吧！',
          options: [{ text: '我不会输的！', next: null, action: 'start_boss:boss_lich_king' }]
        },
        {
          type: 'dialogue',
          speaker: '巫妖王',
          id: 'why',
          text: '为什么？因为这个世界充满了虚伪和不公。我曾经也是人类的国王，却被我最信任的人背叛。现在，我要用不死的力量重塑这个世界！',
          options: [
            { text: '你的方式是错的！', next: 'wrong' },
            { text: '我理解你的痛苦。', next: 'understand' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '巫妖王',
          id: 'wrong',
          text: '错的？这个世界从来就没有对过。既然你不理解，那就和这个腐朽的世界一起毁灭吧！',
          options: [{ text: '那就用实力说话！', next: null, action: 'start_boss:boss_lich_king' }]
        },
        {
          type: 'dialogue',
          speaker: '巫妖王',
          id: 'understand',
          text: '...你理解？已经很久没有人这样说了。但理解不能改变什么。我已经走得太远，无法回头了。让我们用战斗来结束这一切吧。',
          options: [{ text: '我会给你一个解脱。', next: null, action: 'start_boss:boss_lich_king' }]
        }
      ]
    },

    chapter6: {
      id: 'chapter6',
      title: '终章：混沌之主',
      unlockCondition: { type: 'floor', value: 26 },
      scenes: [
        {
          type: 'narration',
          text: '巫妖王倒下了，你终于来到了地牢的最深处。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '这里的空间扭曲，现实与虚空交织。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '在你面前的，是传说中的混沌之主。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '混沌之主',
          text: '有趣...你居然能走到这里。你身上有特殊的气息，是命运选中的人吗？',
          options: [
            { text: '我是来消灭你的！', next: 'destroy' },
            { text: '你到底是什么？', next: 'what' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '混沌之主',
          id: 'destroy',
          text: '消灭我？哈哈哈哈！我是混沌本身，是万物的起源和终结。你无法消灭我，就像你无法消灭黑暗一样。不过，我可以给你一个机会——成为我的使者，共同重塑这个世界。',
          options: [
            { text: '我拒绝！', next: 'refuse_final' },
            { text: '...如果我答应呢？', next: 'accept' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '混沌之主',
          id: 'what',
          text: '我是什么？我是混沌，是虚空，是一切可能性的集合。这个世界是从我的一部分中诞生的，而我，要把它重新收回混沌之中。',
          options: [
            { text: '我不会让你这么做！', next: 'refuse_final' },
            { text: '也许你是对的...', next: 'accept' }
          ]
        },
        {
          type: 'dialogue',
          speaker: '混沌之主',
          id: 'refuse_final',
          text: '拒绝？那就让我看看，命运选中的人有多少实力。用你的一切来挑战我吧！',
          options: [{ text: '决一死战！', next: null, action: 'start_boss:boss_final' }]
        },
        {
          type: 'dialogue',
          speaker: '混沌之主',
          id: 'accept',
          text: '哦？你愿意接受混沌的力量？很好...不过，我需要先测试你的诚意。击败我，证明你有资格成为我的使者！',
          options: [{ text: '我会证明的！', next: null, action: 'start_boss:boss_final' }]
        }
      ]
    },

    ending: {
      id: 'ending',
      title: '结局：永恒的传说',
      unlockCondition: { type: 'boss_defeated', value: 'boss_final' },
      scenes: [
        {
          type: 'narration',
          text: '混沌之主倒下了，它的身体化为无数光点消散在虚空中。',
          duration: 3000
        },
        {
          type: 'narration',
          text: '地牢开始崩塌，你拼命向外跑去。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '当你终于逃出地牢时，阳光洒在你的脸上。',
          duration: 2500
        },
        {
          type: 'narration',
          text: '你拯救了世界，成为了永恒的传说。',
          duration: 2500
        },
        {
          type: 'dialogue',
          speaker: '村长',
          text: '你...你回来了！你真的做到了！混沌之主被消灭了！整个世界都会记住你的名字！',
          options: [{ text: '这只是开始。', next: null, action: 'game_victory' }]
        }
      ]
    }
  },

  // 已触发的剧情
  triggeredChapters: [],

  // 当前播放的剧情
  currentScene: null,
  currentSceneIndex: 0,
  isPlaying: false,

  // 初始化
  init: function() {
    this.triggeredChapters = [];
    this.currentScene = null;
    this.currentSceneIndex = 0;
    this.isPlaying = false;
  },

  // 检查剧情触发
  checkTriggers: function(game) {
    for (const chapterId in this.chapters) {
      const chapter = this.chapters[chapterId];
      if (this.triggeredChapters.includes(chapterId)) continue;

      if (!chapter.unlockCondition) {
        // 序章，游戏开始时触发
        this.triggerChapter(chapterId, game);
        return;
      }

      const condition = chapter.unlockCondition;
      if (condition.type === 'floor' && game.currentFloor >= condition.value) {
        this.triggerChapter(chapterId, game);
        return;
      }
      if (condition.type === 'boss_defeated' && game.defeatedBosses && game.defeatedBosses.includes(condition.value)) {
        this.triggerChapter(chapterId, game);
        return;
      }
    }
  },

  // 触发章节
  triggerChapter: function(chapterId, game) {
    const chapter = this.chapters[chapterId];
    if (!chapter) return;

    this.triggeredChapters.push(chapterId);
    this.currentScene = chapter;
    this.currentSceneIndex = 0;
    this.isPlaying = true;
    game.state = 'story';
    game.showMessage(`【${chapter.title}】`);
  },

  // 推进剧情
  advanceScene: function(game, optionIndex = null) {
    if (!this.currentScene || !this.isPlaying) return;

    const scenes = this.currentScene.scenes;
    const currentScene = scenes[this.currentSceneIndex];

    if (currentScene.type === 'dialogue' && optionIndex !== null) {
      const option = currentScene.options[optionIndex];
      if (option) {
        // 执行动作
        if (option.action) {
          this.executeAction(option.action, game);
        }
        // 跳转到指定对话
        if (option.next) {
          const nextScene = scenes.find(s => s.id === option.next);
          if (nextScene) {
            this.currentSceneIndex = scenes.indexOf(nextScene);
            return;
          }
        }
        // 结束对话
        this.currentSceneIndex++;
      }
    } else {
      this.currentSceneIndex++;
    }

    // 检查是否结束
    if (this.currentSceneIndex >= scenes.length) {
      this.isPlaying = false;
      this.currentScene = null;
      game.state = 'playing';
    }
  },

  // 执行剧情动作
  executeAction: function(action, game) {
    const parts = action.split(':');
    const actionType = parts[0];
    const actionValue = parts[1];

    switch (actionType) {
      case 'start_quest':
        QuestSystem.acceptQuest(actionValue);
        break;
      case 'give_item':
        game.player.addItem(actionValue, 1);
        game.showMessage('获得物品！');
        break;
      case 'start_boss':
        // BOSS战在loadFloor中处理
        break;
      case 'spawn_enemy':
        for (let i = 0; i < 3; i++) {
          const angle = Math.random() * Math.PI * 2;
          const mx = game.player.x + Math.cos(angle) * 100;
          const my = game.player.y + Math.sin(angle) * 100;
          game.monsters.push(new Monster(mx, my, actionValue, game.currentFloor));
        }
        game.showMessage('敌人出现了！');
        break;
      case 'game_victory':
        game.state = 'victory';
        break;
    }
  },

  // 渲染剧情
  render: function(ctx, canvas, game) {
    if (!this.isPlaying || !this.currentScene) return;
    if (!game) game = window.Game;

    const scenes = this.currentScene.scenes;
    const currentScene = scenes[this.currentSceneIndex];
    if (!currentScene) return;

    // 背景
    ctx.fillStyle = 'rgba(0, 0, 0, 0.85)';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    if (currentScene.type === 'narration') {
      // 旁白
      ctx.fillStyle = '#f1c40f';
      ctx.font = 'bold 28px Arial';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';

      // 打字机效果（简化：直接显示）
      ctx.fillText(currentScene.text, canvas.width / 2, canvas.height / 2);

      ctx.fillStyle = '#7f8c8d';
      ctx.font = '18px Arial';
      ctx.fillText('点击继续', canvas.width / 2, canvas.height - 80);
    } else if (currentScene.type === 'dialogue') {
      // 对话框
      const boxX = 100;
      const boxY = canvas.height - 250;
      const boxWidth = canvas.width - 200;
      const boxHeight = 200;

      ctx.fillStyle = 'rgba(26, 26, 46, 0.95)';
      ctx.fillRect(boxX, boxY, boxWidth, boxHeight);
      ctx.strokeStyle = '#9b59b6';
      ctx.lineWidth = 3;
      ctx.strokeRect(boxX, boxY, boxWidth, boxHeight);

      // 说话者
      ctx.fillStyle = '#f1c40f';
      ctx.font = 'bold 24px Arial';
      ctx.textAlign = 'left';
      ctx.textBaseline = 'top';
      ctx.fillText(currentScene.speaker, boxX + 20, boxY + 15);

      // 对话内容
      ctx.fillStyle = '#fff';
      ctx.font = '20px Arial';
      this.wrapText(ctx, currentScene.text, boxX + 20, boxY + 55, boxWidth - 40, 30);

      // 选项
      if (currentScene.options) {
        currentScene.options.forEach((opt, i) => {
          const optY = boxY + 130 + i * 30;
          const isHovered = game.mouseY > optY - 15 && game.mouseY < optY + 15 &&
                           game.mouseX > boxX + 20 && game.mouseX < boxX + boxWidth - 20;

          ctx.fillStyle = isHovered ? '#f1c40f' : '#bdc3c7';
          ctx.font = '18px Arial';
          ctx.fillText(`${i + 1}. ${opt.text}`, boxX + 40, optY);
        });
      }
    }
  },

  // 文字换行
  wrapText: function(ctx, text, x, y, maxWidth, lineHeight) {
    const words = text.split('');
    let line = '';
    let currentY = y;

    for (let n = 0; n < words.length; n++) {
      const testLine = line + words[n];
      const metrics = ctx.measureText(testLine);
      if (metrics.width > maxWidth && n > 0) {
        ctx.fillText(line, x, currentY);
        line = words[n];
        currentY += lineHeight;
      } else {
        line = testLine;
      }
    }
    ctx.fillText(line, x, currentY);
  },

  // 处理点击
  handleClick: function(game) {
    if (!this.isPlaying || !this.currentScene) return;

    const scenes = this.currentScene.scenes;
    const currentScene = scenes[this.currentSceneIndex];

    if (currentScene.type === 'narration') {
      this.advanceScene(game);
    } else if (currentScene.type === 'dialogue' && currentScene.options) {
      const boxX = 100;
      const boxY = game.canvas.height - 250;
      const boxWidth = game.canvas.width - 200;

      currentScene.options.forEach((opt, i) => {
        const optY = boxY + 130 + i * 30;
        if (game.mouseY > optY - 15 && game.mouseY < optY + 15 &&
            game.mouseX > boxX + 20 && game.mouseX < boxX + boxWidth - 20) {
          this.advanceScene(game, i);
        }
      });
    }
  },

  // 保存/加载
  save: function() {
    return {
      triggeredChapters: [...this.triggeredChapters]
    };
  },

  load: function(data) {
    if (data && data.triggeredChapters) {
      this.triggeredChapters = [...data.triggeredChapters];
    }
  }
};
