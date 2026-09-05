// ==================== 永恒地牢 - 玩家实体 ====================
// 玩家角色的属性、移动、攻击、技能、升级

class Player {
  constructor(x, y, classType = 'warrior') {
    this.x = x;
    this.y = y;
    this.width = 32;
    this.height = 32;
    this.radius = 16;
    this.classType = classType;
    this.name = '冒险者';
    this.level = 1;
    this.exp = 0;
    this.expToNext = 100;

    // 基础属性
    this.baseStats = { str: 10, dex: 10, int: 10, vit: 10 };
    this.attributePoints = 0;
    this.skillPoints = 0;

    // 资源
    this.maxHp = 100;
    this.hp = 100;
    this.maxMp = 50;
    this.mp = 50;
    this.maxStamina = 100;
    this.stamina = 100;
    this.gold = 0;
    this.shield = 0;

    // 战斗属性（计算后）
    this.damage = 10;
    this.armor = 0;
    this.magicResist = 0;
    this.crit = 0.05;
    this.critDamage = 1.5;
    this.dodge = 0;
    this.block = 0;
    this.lifesteal = 0;
    this.attackSpeed = 1.0;
    this.moveSpeed = 3.0;
    this.hpRegen = 1;
    this.mpRegen = 1;

    // 装备
    this.equipment = {
      weapon: null,
      armor: null,
      helmet: null,
      boots: null,
      ring: null,
      amulet: null
    };

    // 背包
    this.inventory = InventorySystem.createInventory();

    // 技能
    this.skills = [];
    this.skillCooldowns = {};
    this.skillBar = [null, null, null, null, null];

    // 状态
    this.buffs = [];
    this.buffModifiers = {};
    this.talentModifiers = {};
    this.skillModifiers = {};
    this.passiveAbilities = [];

    // 移动
    this.velocityX = 0;
    this.velocityY = 0;
    this.facing = 'down';
    this.isMoving = false;
    this.isAttacking = false;
    this.attackTimer = 0;
    this.attackCooldown = 0;

    // 动画
    this.animFrame = 0;
    this.animTimer = 0;
    this.invincibleTimer = 0;

    // 其他
    this.dead = false;
    this.inCombat = false;
    this.combatTimer = 0;
    this.nextAttackBonus = 0;
    this.footstepTimer = 0;

    // 初始化
    this.initClass();
    this.calculateStats();
  }

  // 初始化职业
  initClass() {
    const classData = ClassData[this.classType];
    if (!classData) return;

    this.name = classData.name;
    this.baseStats = { ...classData.baseStats };
    this.maxHp = classData.baseStats.hp || 100;
    this.hp = this.maxHp;
    this.maxMp = classData.baseStats.mp || 50;
    this.mp = this.maxMp;
    this.maxStamina = classData.baseStats.stamina || 100;
    this.stamina = this.maxStamina;
    this.moveSpeed = (classData.derivedStats && classData.derivedStats.moveSpeed) || 3.0;
    this.damage = (classData.derivedStats && classData.derivedStats.damage) || 10;
    this.attackSpeed = (classData.derivedStats && classData.derivedStats.attackSpeed) || 1.0;
    this.armor = (classData.derivedStats && classData.derivedStats.armor) || 0;
    this.magicResist = (classData.derivedStats && classData.derivedStats.magicResist) || 0;
    this.crit = (classData.derivedStats && classData.derivedStats.critChance) || 0.05;
    this.dodge = (classData.derivedStats && classData.derivedStats.dodge) || 0;

    // 初始技能
    this.skills = classData.skills.map(s => ({ ...s, level: 1 }));
    this.skillBar = this.skills.slice(0, 5).map(s => s.id);

    // 初始装备
    if (classData.startingWeapon) {
      const weapon = ItemData.getItem(classData.startingWeapon);
      if (weapon) {
        this.equipment.weapon = { ...weapon };
      }
    }
    if (classData.startingArmor) {
      const armor = ItemData.getItem(classData.startingArmor);
      if (armor) {
        this.equipment.armor = { ...armor };
      }
    }
  }

  // 计算所有属性
  calculateStats() {
    // 基础属性
    let str = this.baseStats.str;
    let dex = this.baseStats.dex;
    let int = this.baseStats.int;
    let vit = this.baseStats.vit;

    // 装备加成
    for (const slot in this.equipment) {
      const item = this.equipment[slot];
      if (item && item.stats) {
        if (item.stats.str) str += item.stats.str;
        if (item.stats.dex) dex += item.stats.dex;
        if (item.stats.int) int += item.stats.int;
        if (item.stats.vit) vit += item.stats.vit;
      }
    }

    // Buff加成
    if (this.buffModifiers) {
      str += this.buffModifiers.str || 0;
      dex += this.buffModifiers.dex || 0;
      int += this.buffModifiers.int || 0;
      vit += this.buffModifiers.vit || 0;
    }

    // 计算派生属性
    this.maxHp = 100 + vit * 10;
    this.maxMp = 50 + int * 5;
    this.damage = 5 + str * 2;
    this.armor = Math.floor(vit * 0.5);
    this.magicResist = Math.floor(int * 0.5);
    this.crit = 0.05 + dex * 0.002;
    this.dodge = dex * 0.001;
    this.hpRegen = 1 + vit * 0.1;
    this.mpRegen = 1 + int * 0.1;

    // 装备附加属性
    for (const slot in this.equipment) {
      const item = this.equipment[slot];
      if (!item || !item.stats) continue;
      if (item.stats.damage) this.damage += item.stats.damage;
      if (item.stats.armor) this.armor += item.stats.armor;
      if (item.stats.magicResist) this.magicResist += item.stats.magicResist;
      if (item.stats.crit) this.crit += item.stats.crit;
      if (item.stats.critDamage) this.critDamage += item.stats.critDamage;
      if (item.stats.dodge) this.dodge += item.stats.dodge;
      if (item.stats.lifesteal) this.lifesteal += item.stats.lifesteal;
      if (item.stats.attackSpeed) this.attackSpeed += item.stats.attackSpeed;
      if (item.stats.moveSpeed) this.moveSpeed += item.stats.moveSpeed;
      if (item.stats.hpRegen) this.hpRegen += item.stats.hpRegen;
      if (item.stats.mpRegen) this.mpRegen += item.stats.mpRegen;
      if (item.stats.magicDamage) this.damage += item.stats.magicDamage * 0.5;
    }

    // 天赋修饰
    if (this.talentModifiers) {
      for (const stat in this.talentModifiers) {
        if (this[stat] !== undefined) {
          this[stat] *= this.talentModifiers[stat];
        }
      }
    }

    // 确保当前值不超过最大值
    this.hp = Math.min(this.hp, this.maxHp);
    this.mp = Math.min(this.mp, this.maxMp);
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

    // 无敌时间
    if (this.invincibleTimer > 0) {
      this.invincibleTimer -= dt;
    }

    // 资源恢复
    if (!this.inCombat) {
      this.hp = Math.min(this.maxHp, this.hp + this.hpRegen * dt);
      this.mp = Math.min(this.maxMp, this.mp + this.mpRegen * dt);
    }
    this.stamina = Math.min(this.maxStamina, this.stamina + 5 * dt);

    // 处理输入
    this.handleInput(dt, game);

    // 移动
    this.updateMovement(dt, game);

    // 攻击
    this.updateAttack(dt, game);

    // 动画
    this.animTimer += dt;
    if (this.animTimer > 0.15) {
      this.animTimer = 0;
      this.animFrame = (this.animFrame + 1) % 4;
    }

    // 足迹
    if (this.isMoving) {
      this.footstepTimer += dt;
      if (this.footstepTimer > 0.3) {
        this.footstepTimer = 0;
        ParticleSystem.footstep(this.x, this.y);
      }
    }
  }

  // 处理输入
  handleInput(dt, game) {
    if (BuffSystem.isControlled(this)) return;

    const input = InputManager;
    let dx = 0, dy = 0;

    if (input.isActionDown('up')) dy -= 1;
    if (input.isActionDown('down')) dy += 1;
    if (input.isActionDown('left')) dx -= 1;
    if (input.isActionDown('right')) dx += 1;

    // 混乱状态
    if (BuffSystem.hasBuff(this, 'confuse')) {
      dx = -dx;
      dy = -dy;
    }

    // 归一化
    if (dx !== 0 || dy !== 0) {
      const len = Math.sqrt(dx * dx + dy * dy);
      dx /= len;
      dy /= len;
      this.isMoving = true;

      // 确定朝向
      if (Math.abs(dx) > Math.abs(dy)) {
        this.facing = dx > 0 ? 'right' : 'left';
      } else {
        this.facing = dy > 0 ? 'down' : 'up';
      }
    } else {
      this.isMoving = false;
    }

    this.velocityX = dx * this.moveSpeed;
    this.velocityY = dy * this.moveSpeed;

    // 冲刺
    if (input.isActionPressed('sprint') && this.stamina >= 20) {
      this.stamina -= 20;
      this.velocityX *= 3;
      this.velocityY *= 3;
      ParticleSystem.smoke(this.x, this.y, 5);
      AudioSystem.playSound('dash');
    }

    // 普通攻击
    if (input.isActionPressed('attack') && this.attackCooldown <= 0) {
      this.performAttack(game);
    }

    // 技能
    const skillActions = ['skill1', 'skill2', 'skill3', 'skill4', 'ultimate'];
    for (let i = 0; i < 5; i++) {
      if (input.isActionPressed(skillActions[i])) {
        const skillId = this.skillBar[i];
        if (skillId) {
          this.useSkill(skillId, game);
        }
      }
    }

    // 交互
    if (input.isActionPressed('interact')) {
      game.interact(this);
    }

    // 打开背包
    if (input.isActionPressed('inventory')) {
      game.toggleInventory();
    }
  }

  // 更新移动
  updateMovement(dt, game) {
    const newX = this.x + this.velocityX * dt * 60;
    const newY = this.y + this.velocityY * dt * 60;

    // 碰撞检测
    if (!game.isColliding(newX, this.y, this.radius)) {
      this.x = newX;
    }
    if (!game.isColliding(this.x, newY, this.radius)) {
      this.y = newY;
    }

    // 边界限制
    this.x = Utils.clamp(this.x, this.radius, game.mapWidth - this.radius);
    this.y = Utils.clamp(this.y, this.radius, game.mapHeight - this.radius);
  }

  // 更新攻击
  updateAttack(dt, game) {
    if (this.attackCooldown > 0) {
      this.attackCooldown -= dt;
    }
    if (this.isAttacking) {
      this.attackTimer -= dt;
      if (this.attackTimer <= 0) {
        this.isAttacking = false;
      }
    }
  }

  // 执行普通攻击
  performAttack(game) {
    this.isAttacking = true;
    this.attackTimer = 0.3 / this.attackSpeed;
    this.attackCooldown = 0.5 / this.attackSpeed;

    AudioSystem.playSound('attack');

    // 查找攻击范围内的敌人
    const attackRange = 50;
    const attackX = this.x + (this.facing === 'right' ? 30 : this.facing === 'left' ? -30 : 0);
    const attackY = this.y + (this.facing === 'down' ? 30 : this.facing === 'up' ? -30 : 0);

    for (const monster of game.monsters) {
      if (monster.dead) continue;
      const dist = Utils.distance(attackX, attackY, monster.x, monster.y);
      if (dist < attackRange) {
        // 下一次攻击加成
        if (this.nextAttackBonus > 0) {
          const oldDamage = this.damage;
          this.damage *= this.nextAttackBonus;
          CombatSystem.performAttack(this, monster);
          this.damage = oldDamage;
          this.nextAttackBonus = 0;
        } else {
          CombatSystem.performAttack(this, monster);
        }
      }
    }

    // 攻击特效
    ParticleSystem.magic(attackX, attackY, '#fff', 5);
  }

  // 使用技能
  useSkill(skillId, game) {
    const skill = SkillData.getSkill(skillId);
    if (!skill) return;

    // 查找目标
    let target = null;
    let minDist = Infinity;
    for (const monster of game.monsters) {
      if (monster.dead) continue;
      const dist = Utils.distance(this.x, this.y, monster.x, monster.y);
      if (dist < minDist && dist < (skill.range || 300)) {
        minDist = dist;
        target = monster;
      }
    }

    // 目标位置
    const targetX = target ? target.x : this.x + (this.facing === 'right' ? 100 : this.facing === 'left' ? -100 : 0);
    const targetY = target ? target.y : this.y + (this.facing === 'down' ? 100 : this.facing === 'up' ? -100 : 0);

    const result = CombatSystem.performSkill(this, target, skillId, targetX, targetY);

    if (!result.success) {
      game.showMessage(result.message || '无法使用技能');
    }
  }

  // 受到伤害
  takeDamage(damage, source) {
    if (this.invincibleTimer > 0) return;
    if (BuffSystem.hasBuff(this, 'invincibility')) return;

    // 护盾吸收
    if (this.shield > 0) {
      const absorbed = Math.min(this.shield, damage);
      this.shield -= absorbed;
      damage -= absorbed;
      if (damage <= 0) return;
    }

    this.hp -= damage;
    this.invincibleTimer = 0.5;
    CombatSystem.enterCombat(this);

    // 屏幕震动
    if (window.Game && window.Game.renderer) {
      window.Game.renderer.shake(5, 0.2);
    }

    AudioSystem.playSound('hurt');
    ParticleSystem.blood(this.x, this.y, 5);

    if (this.hp <= 0) {
      this.die();
    }
  }

  // 死亡
  die() {
    this.dead = true;
    AchievementSystem.updateStat('totalDeaths');
    AudioSystem.playSound('death');
    ParticleSystem.explosion(this.x, this.y, '#c0392b', 50, 20);

    if (window.Game) {
      window.Game.onPlayerDeath();
    }
  }

  // 复活
  revive() {
    this.dead = false;
    this.hp = this.maxHp;
    this.mp = this.maxMp;
    this.stamina = this.maxStamina;
    this.invincibleTimer = 3;
    BuffSystem.clearAllBuffs(this);
  }

  // 获得经验
  gainExp(amount) {
    this.exp += amount;
    while (this.exp >= this.expToNext) {
      this.exp -= this.expToNext;
      this.levelUp();
    }
  }

  // 升级
  levelUp() {
    this.level++;
    this.expToNext = Math.floor(this.expToNext * 1.2);
    this.attributePoints += 3;
    this.skillPoints += 1;
    TalentSystem.addTalentPoints(1);

    // 恢复全部
    this.hp = this.maxHp;
    this.mp = this.maxMp;
    this.stamina = this.maxStamina;

    // 重新计算属性
    this.calculateStats();

    ParticleSystem.levelUp(this.x, this.y);
    AudioSystem.playSound('levelup');

    if (window.Game) {
      window.Game.showMessage(`升级！当前等级 ${this.level}`);
    }

    AchievementSystem.recordLevel(this.level);
  }

  // 添加到背包
  addToInventory(itemId, quantity = 1) {
    return InventorySystem.addItem(this.inventory, itemId, quantity);
  }

  // 渲染
  render(ctx, camera) {
    const screenX = this.x - camera.x;
    const screenY = this.y - camera.y;

    // 隐身效果
    const alpha = BuffSystem.hasBuff(this, 'stealth') ? 0.3 : 1;
    ctx.globalAlpha = alpha;

    // 无敌闪烁
    if (this.invincibleTimer > 0 && Math.floor(this.invincibleTimer * 10) % 2 === 0) {
      ctx.globalAlpha = alpha * 0.5;
    }

    // 身体
    const classColors = {
      warrior: '#e74c3c',
      mage: '#9b59b6',
      ranger: '#27ae60',
      rogue: '#34495e',
      paladin: '#f1c40f',
      necromancer: '#2c3e50'
    };
    const bodyColor = classColors[this.classType] || '#3498db';

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(screenX, screenY + 14, 12, 5, 0, 0, Math.PI * 2);
    ctx.fill();

    // 身体
    ctx.fillStyle = bodyColor;
    ctx.fillRect(screenX - 10, screenY - 8, 20, 20);

    // 头
    ctx.fillStyle = '#f5d0a9';
    ctx.beginPath();
    ctx.arc(screenX, screenY - 14, 8, 0, Math.PI * 2);
    ctx.fill();

    // 眼睛
    ctx.fillStyle = '#000';
    const eyeOffset = this.facing === 'left' ? -2 : this.facing === 'right' ? 2 : 0;
    ctx.fillRect(screenX - 3 + eyeOffset, screenY - 16, 2, 2);
    ctx.fillRect(screenX + 1 + eyeOffset, screenY - 16, 2, 2);

    // 武器
    if (this.equipment.weapon) {
      ctx.fillStyle = '#bdc3c7';
      const weaponAngle = this.isAttacking ? this.animFrame * 0.5 : 0;
      ctx.save();
      ctx.translate(screenX + 12, screenY);
      ctx.rotate(weaponAngle);
      ctx.fillRect(0, -2, 16, 4);
      ctx.restore();
    }

    // 护盾
    if (this.shield > 0) {
      ctx.strokeStyle = 'rgba(52, 152, 219, 0.6)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(screenX, screenY, 22, 0, Math.PI * 2);
      ctx.stroke();
    }

    ctx.globalAlpha = 1;

    // 血条
    const barWidth = 30;
    const barHeight = 4;
    const barX = screenX - barWidth / 2;
    const barY = screenY - 28;

    ctx.fillStyle = '#333';
    ctx.fillRect(barX, barY, barWidth, barHeight);
    ctx.fillStyle = '#e74c3c';
    ctx.fillRect(barX, barY, barWidth * (this.hp / this.maxHp), barHeight);

    // 蓝条
    ctx.fillStyle = '#333';
    ctx.fillRect(barX, barY + 5, barWidth, 3);
    ctx.fillStyle = '#3498db';
    ctx.fillRect(barX, barY + 5, barWidth * (this.mp / this.maxMp), 3);
  }
}

window.Player = Player;
