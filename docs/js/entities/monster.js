// ==================== 永恒地牢 - 怪物实体 ====================
// 怪物的AI、移动、攻击、掉落

class Monster {
  constructor(x, y, monsterId, floor = 1) {
    this.x = x;
    this.y = y;
    this.monsterId = monsterId;
    this.floor = floor;

    const data = MonsterData.getMonster(monsterId);
    if (!data) {
      console.error('Unknown monster:', monsterId);
      return;
    }

    // 基础属性
    this.name = data.name;
    this.icon = data.icon;
    this.type = data.type;
    this.tags = data.tags || [];
    this.radius = data.radius || 14;
    this.color = data.color || '#c0392b';

    // 属性缩放（根据楼层）
    const scale = 1 + 0.12 * (floor - 1);
    this.maxHp = Math.floor(data.hp * scale);
    this.hp = this.maxHp;
    this.damage = Math.floor(data.damage * scale);
    this.armor = Math.floor((data.armor || 0) * scale);
    this.speed = (data.speed || 8) * 60; // 旧单位像素/帧 → 新单位像素/秒
    this.attackRange = data.attackRange || 40;
    this.sightRange = data.sightRange || 200;
    this.attackSpeed = data.attackSpeed || 1.0;
    this.expReward = Math.floor(data.exp * scale);
    this.goldReward = Math.floor(data.gold * scale);

    // 特殊属性
    this.crit = data.crit || 0.05;
    this.dodge = data.dodge || 0;
    this.lifesteal = data.lifesteal || 0;
    this.fireResist = data.fireResist || 0;
    this.iceResist = data.iceResist || 0;
    this.lightningResist = data.lightningResist || 0;
    this.holyResist = data.holyResist || 0;
    this.shadowResist = data.shadowResist || 0;
    this.poisonResist = data.poisonResist || 0;

    // AI
    this.aiType = data.aiType || 'melee';
    this.skills = data.skills || [];
    this.abilities = data.abilities || [];

    // 状态
    this.buffs = [];
    this.buffModifiers = {};
    this.dead = false;
    this.inCombat = false;
    this.combatTimer = 0;

    // 移动
    this.velocityX = 0;
    this.velocityY = 0;
    this.targetX = x;
    this.targetY = y;
    this.wanderTimer = 0;
    this.attackCooldown = 0;
    this.skillCooldowns = {};

    // 动画
    this.animFrame = 0;
    this.animTimer = 0;
    this.hitFlash = 0;

    // 其他
    this.isBoss = data.isBoss || false;
    this.isElite = data.isElite || false;
    this.isSummon = data.isSummon || false;
    this.owner = null;
    this.summonDuration = data.summonDuration || 0;
    this.summonTimer = 0;

    // 掉落
    this.lootTable = data.loot || [];
  }

  // 更新
  update(dt, game) {
    if (this.dead) return;

    // 更新Buff
    BuffSystem.updateBuffs(this, dt);

    // 更新技能冷却
    CombatSystem.updateCooldowns(this, dt);

    // 更新战斗状态
    CombatSystem.updateCombatState(this, dt);

    // 受击闪烁
    if (this.hitFlash > 0) {
      this.hitFlash -= dt;
    }

    // 召唤物计时
    if (this.isSummon) {
      this.summonTimer += dt;
      if (this.summonTimer >= this.summonDuration) {
        this.die();
        return;
      }
    }

    // AI行为
    this.updateAI(dt, game);

    // 移动
    this.updateMovement(dt, game);

    // 攻击
    this.updateAttack(dt, game);

    // 动画
    this.animTimer += dt;
    if (this.animTimer > 0.2) {
      this.animTimer = 0;
      this.animFrame = (this.animFrame + 1) % 4;
    }
  }

  // AI更新
  updateAI(dt, game) {
    if (BuffSystem.isControlled(this)) {
      this.velocityX = 0;
      this.velocityY = 0;
      return;
    }

    const player = game.player;
    if (!player || player.dead) return;

    const distToPlayer = Utils.distance(this.x, this.y, player.x, player.y);

    // 恐惧状态 - 逃跑
    if (BuffSystem.hasBuff(this, 'fear')) {
      const angle = Math.atan2(this.y - player.y, this.x - player.x);
      this.velocityX = Math.cos(angle) * this.speed;
      this.velocityY = Math.sin(angle) * this.speed;
      return;
    }

    // 嘲讽状态 - 强制攻击施法者
    if (BuffSystem.hasBuff(this, 'taunt') && this.tauntSource) {
      this.moveTowards(this.tauntSource.x, this.tauntSource.y);
      return;
    }

    switch (this.aiType) {
      case 'melee':
        this.meleeAI(dt, game, distToPlayer);
        break;
      case 'ranged':
        this.rangedAI(dt, game, distToPlayer);
        break;
      case 'caster':
        this.casterAI(dt, game, distToPlayer);
        break;
      case 'tank':
        this.tankAI(dt, game, distToPlayer);
        break;
      case 'charger':
        this.chargerAI(dt, game, distToPlayer);
        break;
      case 'summoner':
        this.summonerAI(dt, game, distToPlayer);
        break;
      case 'boss':
        this.bossAI(dt, game, distToPlayer);
        break;
      case 'passive':
        this.passiveAI(dt, game, distToPlayer);
        break;
      default:
        this.meleeAI(dt, game, distToPlayer);
    }
  }

  // 近战AI
  meleeAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange) {
      // 发现玩家，进入战斗
      CombatSystem.enterCombat(this);
      this.moveTowards(player.x, player.y);
    } else if (this.inCombat) {
      // 失去目标，返回巡逻
      this.wander(dt);
    } else {
      // 巡逻
      this.wander(dt);
    }
  }

  // 远程AI
  rangedAI(dt, game, distToPlayer) {
    const player = game.player;
    const preferredRange = 150;

    if (distToPlayer < this.sightRange) {
      CombatSystem.enterCombat(this);

      if (distToPlayer < preferredRange - 30) {
        // 太近，后退
        this.moveAway(player.x, player.y);
      } else if (distToPlayer > preferredRange + 30) {
        // 太远，靠近
        this.moveTowards(player.x, player.y);
      } else {
        // 保持距离
        this.velocityX = 0;
        this.velocityY = 0;
      }
    } else {
      this.wander(dt);
    }
  }

  // 法师AI
  casterAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange) {
      CombatSystem.enterCombat(this);

      // 保持距离
      if (distToPlayer < 120) {
        this.moveAway(player.x, player.y);
      } else {
        this.velocityX = 0;
        this.velocityY = 0;
      }

      // 使用技能
      this.tryUseSkill(game, player);
    } else {
      this.wander(dt);
    }
  }

  // 坦克AI
  tankAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange) {
      CombatSystem.enterCombat(this);
      this.moveTowards(player.x, player.y);
    } else {
      this.wander(dt);
    }
  }

  // 冲锋AI
  chargerAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange) {
      CombatSystem.enterCombat(this);

      if (!this.charging && distToPlayer < 150 && this.attackCooldown <= 0) {
        // 开始冲锋
        this.charging = true;
        this.chargeTime = 0.5;
        const angle = Math.atan2(player.y - this.y, player.x - this.x);
        this.velocityX = Math.cos(angle) * this.speed * 4;
        this.velocityY = Math.sin(angle) * this.speed * 4;
        this.attackCooldown = 3;
        AudioSystem.playSound('dash');
      }

      if (!this.charging) {
        this.moveTowards(player.x, player.y);
      }
    } else {
      this.wander(dt);
    }

    if (this.charging) {
      this.chargeTime -= dt;
      if (this.chargeTime <= 0) {
        this.charging = false;
      }
    }
  }

  // 召唤者AI
  summonerAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange) {
      CombatSystem.enterCombat(this);

      // 保持距离
      if (distToPlayer < 150) {
        this.moveAway(player.x, player.y);
      } else {
        this.velocityX = 0;
        this.velocityY = 0;
      }

      // 召唤怪物
      this.trySummon(game);
    } else {
      this.wander(dt);
    }
  }

  // BOSS AI
  bossAI(dt, game, distToPlayer) {
    const player = game.player;

    if (distToPlayer < this.sightRange * 1.5) {
      CombatSystem.enterCombat(this);

      // 根据血量阶段改变行为
      const hpPercent = this.hp / this.maxHp;

      if (hpPercent > 0.5) {
        // 第一阶段：普通攻击
        this.moveTowards(player.x, player.y);
      } else if (hpPercent > 0.25) {
        // 第二阶段：更激进，使用技能
        this.moveTowards(player.x, player.y);
        this.tryUseSkill(game, player);
      } else {
        // 第三阶段：狂暴
        if (!this.enraged) {
          this.enraged = true;
          this.damage *= 1.5;
          this.speed *= 1.3;
          game.showMessage(`${this.name} 进入狂暴状态！`);
          ParticleSystem.explosion(this.x, this.y, '#e74c3c', 80, 20);
        }
        this.moveTowards(player.x, player.y);
        this.tryUseSkill(game, player);
      }
    }
  }

  // 被动AI（不主动攻击）
  passiveAI(dt, game, distToPlayer) {
    if (this.inCombat) {
      const player = game.player;
      this.moveTowards(player.x, player.y);
    } else {
      this.wander(dt);
    }
  }

  // 尝试使用技能
  tryUseSkill(game, target) {
    for (const skill of this.skills) {
      if (!this.skillCooldowns[skill.id] || this.skillCooldowns[skill.id] <= 0) {
        const result = CombatSystem.performSkill(this, target, skill.id, target.x, target.y);
        if (result.success) {
          this.skillCooldowns[skill.id] = skill.cooldown || 5;
          break;
        }
      }
    }
  }

  // 尝试召唤
  trySummon(game) {
    if (!this.summonCooldown || this.summonCooldown <= 0) {
      const summonId = this.abilities.find(a => a.type === 'summon')?.summonId;
      if (summonId && game.monsters.filter(m => m.owner === this).length < 3) {
        const angle = Math.random() * Math.PI * 2;
        const dist = 50;
        const summon = new Monster(
          this.x + Math.cos(angle) * dist,
          this.y + Math.sin(angle) * dist,
          summonId,
          this.floor
        );
        summon.owner = this;
        summon.isSummon = true;
        game.monsters.push(summon);
        ParticleSystem.magic(summon.x, summon.y, '#9b59b6', 15);
        AudioSystem.playSound('summon');
      }
      this.summonCooldown = 8;
    } else {
      this.summonCooldown -= 0.016;
    }
  }

  // 移动向目标
  moveTowards(targetX, targetY) {
    const dx = targetX - this.x;
    const dy = targetY - this.y;
    const dist = Math.sqrt(dx * dx + dy * dy);
    if (dist > 0) {
      this.velocityX = (dx / dist) * this.speed;
      this.velocityY = (dy / dist) * this.speed;
    }
  }

  // 远离目标
  moveAway(targetX, targetY) {
    const dx = this.x - targetX;
    const dy = this.y - targetY;
    const dist = Math.sqrt(dx * dx + dy * dy);
    if (dist > 0) {
      this.velocityX = (dx / dist) * this.speed;
      this.velocityY = (dy / dist) * this.speed;
    }
  }

  // 巡逻
  wander(dt) {
    this.wanderTimer -= dt;
    if (this.wanderTimer <= 0) {
      this.wanderTimer = 2 + Math.random() * 3;
      this.targetX = this.x + (Math.random() - 0.5) * 100;
      this.targetY = this.y + (Math.random() - 0.5) * 100;
    }

    const dx = this.targetX - this.x;
    const dy = this.targetY - this.y;
    const dist = Math.sqrt(dx * dx + dy * dy);
    if (dist > 5) {
      this.velocityX = (dx / dist) * this.speed;
      this.velocityY = (dy / dist) * this.speed;
    } else {
      this.velocityX = 0;
      this.velocityY = 0;
    }
  }

  // 更新移动
  updateMovement(dt, game) {
    // 速度单位：像素/秒
    const newX = this.x + this.velocityX * dt;
    const newY = this.y + this.velocityY * dt;

    if (!game.isColliding(newX, this.y, this.radius)) {
      this.x = newX;
    }
    if (!game.isColliding(this.x, newY, this.radius)) {
      this.y = newY;
    }

    this.x = Utils.clamp(this.x, this.radius, game.mapWidth - this.radius);
    this.y = Utils.clamp(this.y, this.radius, game.mapHeight - this.radius);
  }

  // 更新攻击
  updateAttack(dt, game) {
    if (this.attackCooldown > 0) {
      this.attackCooldown -= dt;
      return;
    }

    const player = game.player;
    if (!player || player.dead) return;

    const dist = Utils.distance(this.x, this.y, player.x, player.y);

    if (dist < this.attackRange && !BuffSystem.isControlled(this)) {
      this.performAttack(player);
      this.attackCooldown = 1.0 / this.attackSpeed;
    }
  }

  // 执行攻击
  performAttack(target) {
    AudioSystem.playSound('attack');
    CombatSystem.performAttack(this, target);
  }

  // 受到伤害
  takeDamage(damage, source) {
    // BOSS护盾处理
    if (this.isBoss && this.shield && this.shield > 0) {
      const absorbed = Math.min(this.shield, damage);
      this.shield -= absorbed;
      damage -= absorbed;
      if (damage <= 0) {
        this.hitFlash = 0.1;
        return;
      }
    }
    this.hp -= damage;
    this.hitFlash = 0.1;
    CombatSystem.enterCombat(this);

    // 被动怪物被攻击后进入战斗
    if (this.aiType === 'passive') {
      this.aiType = 'melee';
    }

    // 吸血
    if (this.lifesteal > 0) {
      this.hp = Math.min(this.maxHp, this.hp + damage * this.lifesteal);
    }

    if (this.hp <= 0) {
      this.die(source);
    }
  }

  // 死亡
  die(killer = null) {
    if (this.dead) return;
    this.dead = true;

    AudioSystem.playSound('death');
    ParticleSystem.blood(this.x, this.y, 15);

    // 掉落
    if (window.Game) {
      this.dropLoot();

      // 经验和金币
      if (killer === window.Game.player) {
        killer.gainExp(this.expReward);
        killer.gold += this.goldReward;
        ParticleSystem.itemPickup(this.x, this.y, '#f1c40f');
        AchievementSystem.recordGoldEarned(this.goldReward);
      }

      // 记录击杀
      AchievementSystem.recordKill(this.monsterId, this.isBoss);

      // 记录击败的BOSS
      if (this.isBoss && window.Game) {
        if (!window.Game.defeatedBosses) {
          window.Game.defeatedBosses = [];
        }
        if (!window.Game.defeatedBosses.includes(this.monsterId)) {
          window.Game.defeatedBosses.push(this.monsterId);
        }
      }

      // 任务进度
      QuestSystem.updateProgress('kill', this.monsterId);
      QuestSystem.updateProgress('kill_any');
      if (this.tags.includes('undead')) {
        QuestSystem.updateProgress('kill_type', 'undead');
      }
    }
  }

  // 掉落物品
  dropLoot() {
    if (!window.Game) return;

    for (const loot of this.lootTable) {
      if (Math.random() < loot.chance) {
        const count = loot.min + Math.floor(Math.random() * (loot.max - loot.min + 1));
        window.Game.dropItem(loot.itemId, this.x + (Math.random() - 0.5) * 30, this.y + (Math.random() - 0.5) * 30, count);
      }
    }

    // 符文掉落（符文系统）
    if (window.RuneSystem) {
      const monsterType = this.isBoss ? (this.isWorldBoss ? 'worldBoss' : 'boss') : (this.isElite ? 'elite' : 'normal');
      const rune = RuneSystem.tryDropRune({ type: monsterType }, window.Game.currentFloor || 1);
      if (rune) {
        // 直接添加到地面物品
        const dropX = this.x + (Math.random() - 0.5) * 30;
        const dropY = this.y + (Math.random() - 0.5) * 30;
        if (!window.Game.groundItems) window.Game.groundItems = [];
        window.Game.groundItems.push({
          ...rune,
          x: dropX,
          y: dropY,
          isRune: true
        });
        // 显示符文掉落提示
        if (window.Game.showMessage) {
          window.Game.showMessage(`✨ 掉落了 ${rune.name}！`, rune.color);
        }
      }
    }
  }

  // 渲染
  render(ctx, camera) {
    const screenX = this.x - camera.x;
    const screenY = this.y - camera.y;

    // 受击闪烁
    if (this.hitFlash > 0) {
      ctx.globalAlpha = 0.7;
    }

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(screenX, screenY + this.radius - 2, this.radius * 0.8, this.radius * 0.3, 0, 0, Math.PI * 2);
    ctx.fill();

    // 身体
    ctx.fillStyle = this.hitFlash > 0 ? '#fff' : this.color;
    ctx.beginPath();
    ctx.arc(screenX, screenY, this.radius, 0, Math.PI * 2);
    ctx.fill();

    // 精英/BOSS光环
    if (this.isElite) {
      ctx.strokeStyle = '#f1c40f';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(screenX, screenY, this.radius + 3, 0, Math.PI * 2);
      ctx.stroke();
    }
    if (this.isBoss) {
      ctx.strokeStyle = '#e74c3c';
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.arc(screenX, screenY, this.radius + 5, 0, Math.PI * 2);
      ctx.stroke();
    }

    // 眼睛
    ctx.fillStyle = '#fff';
    ctx.beginPath();
    ctx.arc(screenX - 4, screenY - 3, 3, 0, Math.PI * 2);
    ctx.arc(screenX + 4, screenY - 3, 3, 0, Math.PI * 2);
    ctx.fill();
    ctx.fillStyle = '#000';
    ctx.beginPath();
    ctx.arc(screenX - 4, screenY - 3, 1.5, 0, Math.PI * 2);
    ctx.arc(screenX + 4, screenY - 3, 1.5, 0, Math.PI * 2);
    ctx.fill();

    ctx.globalAlpha = 1;

    // 血条
    const barWidth = this.isBoss ? 60 : 30;
    const barHeight = this.isBoss ? 6 : 4;
    const barX = screenX - barWidth / 2;
    const barY = screenY - this.radius - 10;

    ctx.fillStyle = '#333';
    ctx.fillRect(barX, barY, barWidth, barHeight);
    ctx.fillStyle = this.isBoss ? '#e74c3c' : '#27ae60';
    ctx.fillRect(barX, barY, barWidth * (this.hp / this.maxHp), barHeight);

    // 名字（BOSS和精英）
    if (this.isBoss || this.isElite) {
      ctx.fillStyle = this.isBoss ? '#e74c3c' : '#f1c40f';
      ctx.font = 'bold 12px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(this.name, screenX, barY - 5);
    }

    // Buff图标
    const buffs = BuffSystem.getBuffIcons(this);
    buffs.forEach((buff, i) => {
      ctx.font = '10px Arial';
      ctx.fillText(buff.icon, screenX - 15 + i * 12, screenY + this.radius + 12);
    });
  }
}

window.Monster = Monster;
