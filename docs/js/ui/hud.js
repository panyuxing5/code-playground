// ==================== 永恒地牢 - HUD系统 ====================
// 游戏内UI：血条、蓝条、技能栏、小地图、消息日志

const HUD = {
  // 渲染HUD
  render(ctx, game) {
    const player = game.player;
    if (!player) return;

    this.renderHealthBar(ctx, player);
    this.renderManaBar(ctx, player);
    this.renderStaminaBar(ctx, player);
    this.renderSkillBar(ctx, player, game);
    this.renderMinimap(ctx, game);
    this.renderMessageLog(ctx, game);
    this.renderQuestTracker(ctx, game);
    this.renderBuffBar(ctx, player);
    this.renderFloorInfo(ctx, game);
    this.renderGold(ctx, player, game);
  },

  // 血条
  renderHealthBar(ctx, player) {
    const x = 20;
    const y = 20;
    const width = 250;
    const height = 24;

    // 背景
    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x - 2, y - 2, width + 4, height + 4);

    // 血条
    ctx.fillStyle = '#333';
    ctx.fillRect(x, y, width, height);
    ctx.fillStyle = '#e74c3c';
    ctx.fillRect(x, y, width * (player.hp / player.maxHp), height);

    // 渐变
    const gradient = ctx.createLinearGradient(x, y, x, y + height);
    gradient.addColorStop(0, 'rgba(255,255,255,0.3)');
    gradient.addColorStop(1, 'rgba(0,0,0,0.2)');
    ctx.fillStyle = gradient;
    ctx.fillRect(x, y, width * (player.hp / player.maxHp), height);

    // 文字
    ctx.fillStyle = '#fff';
    ctx.font = 'bold 12px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(`HP ${Math.floor(player.hp)} / ${player.maxHp}`, x + width / 2, y + height / 2);

    // 护盾
    if (player.shield > 0) {
      ctx.fillStyle = '#3498db';
      ctx.fillRect(x, y - 5, width * (player.shield / player.maxHp), 3);
    }
  },

  // 蓝条
  renderManaBar(ctx, player) {
    const x = 20;
    const y = 50;
    const width = 250;
    const height = 18;

    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x - 2, y - 2, width + 4, height + 4);

    ctx.fillStyle = '#333';
    ctx.fillRect(x, y, width, height);
    ctx.fillStyle = '#3498db';
    ctx.fillRect(x, y, width * (player.mp / player.maxMp), height);

    ctx.fillStyle = '#fff';
    ctx.font = 'bold 11px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(`MP ${Math.floor(player.mp)} / ${player.maxMp}`, x + width / 2, y + height / 2);
  },

  // 体力条
  renderStaminaBar(ctx, player) {
    const x = 20;
    const y = 74;
    const width = 250;
    const height = 12;

    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x - 2, y - 2, width + 4, height + 4);

    ctx.fillStyle = '#333';
    ctx.fillRect(x, y, width, height);
    ctx.fillStyle = '#2ecc71';
    ctx.fillRect(x, y, width * (player.stamina / player.maxStamina), height);
  },

  // 技能栏
  renderSkillBar(ctx, player, game) {
    const startX = 20;
    const y = game.canvas.height - 70;
    const slotSize = 50;
    const gap = 5;

    for (let i = 0; i < 5; i++) {
      const x = startX + i * (slotSize + gap);
      const skillId = player.skillBar[i];
      const skill = skillId ? SkillData.getSkill(skillId) : null;
      const cooldown = player.skillCooldowns[skillId] || 0;

      // 槽位背景
      ctx.fillStyle = 'rgba(0,0,0,0.7)';
      ctx.fillRect(x, y, slotSize, slotSize);
      ctx.strokeStyle = '#555';
      ctx.lineWidth = 2;
      ctx.strokeRect(x, y, slotSize, slotSize);

      if (skill) {
        // 技能图标
        ctx.font = '24px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(skill.icon, x + slotSize / 2, y + slotSize / 2);

        // 冷却遮罩
        if (cooldown > 0) {
          ctx.fillStyle = 'rgba(0,0,0,0.6)';
          const cooldownHeight = slotSize * (cooldown / skill.cooldown);
          ctx.fillRect(x, y, slotSize, cooldownHeight);

          ctx.fillStyle = '#fff';
          ctx.font = 'bold 14px Arial';
          ctx.fillText(cooldown.toFixed(1), x + slotSize / 2, y + slotSize / 2);
        }

        // 快捷键
        ctx.fillStyle = '#f1c40f';
        ctx.font = 'bold 10px Arial';
        ctx.textAlign = 'left';
        ctx.fillText(i + 1, x + 3, y + 12);
      }
    }

    // 普通攻击提示
    ctx.fillStyle = 'rgba(255,255,255,0.7)';
    ctx.font = '11px Arial';
    ctx.textAlign = 'left';
    ctx.fillText('左键: 攻击 | 1-5: 技能 | 空格: 冲刺 | E: 交互 | I: 背包', 20, y - 10);
  },

  // 小地图
  renderMinimap(ctx, game) {
    const mapSize = 150;
    const x = game.canvas.width - mapSize - 20;
    const y = 20;

    // 背景
    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x - 2, y - 2, mapSize + 4, mapSize + 4);
    ctx.fillStyle = '#1a1a2e';
    ctx.fillRect(x, y, mapSize, mapSize);

    if (!game.dungeon) return;

    const scale = mapSize / Math.max(game.dungeon.width, game.dungeon.height);

    // 绘制地图
    for (let ty = 0; ty < game.dungeon.height; ty++) {
      for (let tx = 0; tx < game.dungeon.width; tx++) {
        const tile = game.dungeon.tiles[ty][tx];
        if (tile === DungeonGenerator.TILE_TYPES.FLOOR) {
          ctx.fillStyle = '#4a4a6a';
          ctx.fillRect(x + tx * scale, y + ty * scale, scale, scale);
        } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_DOWN) {
          ctx.fillStyle = '#e74c3c';
          ctx.fillRect(x + tx * scale, y + ty * scale, scale, scale);
        } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_UP) {
          ctx.fillStyle = '#3498db';
          ctx.fillRect(x + tx * scale, y + ty * scale, scale, scale);
        }
      }
    }

    // 绘制怪物
    for (const monster of game.monsters) {
      if (monster.dead) continue;
      const mx = x + (monster.x / DungeonGenerator.TILE_SIZE) * scale;
      const my = y + (monster.y / DungeonGenerator.TILE_SIZE) * scale;
      ctx.fillStyle = monster.isBoss ? '#e74c3c' : '#f39c12';
      ctx.beginPath();
      ctx.arc(mx, my, monster.isBoss ? 4 : 2, 0, Math.PI * 2);
      ctx.fill();
    }

    // 绘制玩家
    const px = x + (game.player.x / DungeonGenerator.TILE_SIZE) * scale;
    const py = y + (game.player.y / DungeonGenerator.TILE_SIZE) * scale;
    ctx.fillStyle = '#2ecc71';
    ctx.beginPath();
    ctx.arc(px, py, 3, 0, Math.PI * 2);
    ctx.fill();

    // 边框
    ctx.strokeStyle = '#555';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, mapSize, mapSize);
  },

  // 消息日志
  renderMessageLog(ctx, game) {
    const x = 20;
    const y = game.canvas.height - 180;
    const width = 400;
    const maxMessages = 6;

    ctx.fillStyle = 'rgba(0,0,0,0.5)';
    ctx.fillRect(x, y, width, maxMessages * 18);

    ctx.font = '12px Arial';
    ctx.textAlign = 'left';
    ctx.textBaseline = 'top';

    const messages = game.messages.slice(-maxMessages);
    messages.forEach((msg, i) => {
      const alpha = 1 - (messages.length - 1 - i) * 0.15;
      ctx.globalAlpha = alpha;
      ctx.fillStyle = msg.color || '#fff';
      ctx.fillText(msg.text, x + 5, y + 5 + i * 18);
    });
    ctx.globalAlpha = 1;
  },

  // 任务追踪
  renderQuestTracker(ctx, game) {
    const quests = QuestSystem.getTrackedQuests();
    if (quests.length === 0) return;

    const x = game.canvas.width - 220;
    const y = 190;
    const width = 200;

    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x, y, width, 30 + quests.length * 40);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('任务追踪', x + width / 2, y + 18);

    quests.forEach((quest, i) => {
      const qy = y + 30 + i * 40;
      ctx.fillStyle = quest.completed ? '#2ecc71' : '#fff';
      ctx.font = '11px Arial';
      ctx.textAlign = 'left';
      ctx.fillText(quest.name, x + 8, qy);

      quest.progress.forEach((obj, j) => {
        ctx.fillStyle = obj.completed ? '#2ecc71' : '#aaa';
        ctx.font = '10px Arial';
        ctx.fillText(`  ${obj.description} (${obj.current}/${obj.count})`, x + 8, qy + 14 + j * 12);
      });
    });
  },

  // Buff栏
  renderBuffBar(ctx, player) {
    const buffs = BuffSystem.getBuffIcons(player);
    if (buffs.length === 0) return;

    const x = 20;
    const y = 95;
    const size = 28;
    const gap = 3;

    buffs.forEach((buff, i) => {
      const bx = x + i * (size + gap);

      ctx.fillStyle = buff.type === 'buff' ? 'rgba(46, 204, 113, 0.3)' : 'rgba(231, 76, 60, 0.3)';
      ctx.fillRect(bx, y, size, size);
      ctx.strokeStyle = buff.type === 'buff' ? '#2ecc71' : '#e74c3c';
      ctx.lineWidth = 1;
      ctx.strokeRect(bx, y, size, size);

      ctx.font = '16px Arial';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(buff.icon, bx + size / 2, y + size / 2);

      // 剩余时间
      if (buff.remaining < 10) {
        ctx.fillStyle = '#fff';
        ctx.font = 'bold 9px Arial';
        ctx.fillText(buff.remaining.toFixed(0), bx + size / 2, y + size + 8);
      }

      // 层数
      if (buff.stacks > 1) {
        ctx.fillStyle = '#f1c40f';
        ctx.font = 'bold 10px Arial';
        ctx.textAlign = 'right';
        ctx.fillText(buff.stacks, bx + size - 2, y + 10);
      }
    });
  },

  // 楼层信息
  renderFloorInfo(ctx, game) {
    const x = game.canvas.width / 2;
    const y = 30;

    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x - 80, y - 15, 160, 30);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 16px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(`第 ${game.currentFloor} 层`, x, y);

    if (game.dungeon?.isBossFloor) {
      ctx.fillStyle = '#e74c3c';
      ctx.font = 'bold 12px Arial';
      ctx.fillText('⚠ BOSS层 ⚠', x, y + 22);
    }
  },

  // 金币
  renderGold(ctx, player, game) {
    const x = game.canvas.width - 180;
    const y = game.canvas.height - 40;

    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(x, y, 160, 30);

    ctx.font = '20px Arial';
    ctx.textAlign = 'left';
    ctx.textBaseline = 'middle';
    ctx.fillText('💰', x + 8, y + 15);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 16px Arial';
    ctx.fillText(player.gold, x + 35, y + 15);
  },

  // 显示成就通知
  showAchievementNotification(achievement) {
    // 由UIManager处理
  },

  // 显示任务通知
  showQuestNotification(quest, type) {
    // 由UIManager处理
  }
};

window.HUD = HUD;
