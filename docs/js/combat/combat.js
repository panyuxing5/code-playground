// ==================== 永恒地牢 - 战斗系统 ====================
// 管理伤害计算、攻击判定、技能释放、战斗状态
// 数值设计参考：百分比减伤公式、暴击期望、几何分布

const CombatSystem = {
  // ==================== 数学公式工具 ====================

  /**
   * 百分比减伤公式（防御收益递减，绝大多数游戏通用）
   * 伤害 = 攻击 × 100 / (100 + 防御)
   * 防御0→100%伤害，防御50→66.7%，防御100→50%，防御200→33.3%
   */
  calculateDamageReduction(baseDamage, armor) {
    return baseDamage * 100 / (100 + Math.max(0, armor));
  },

  /**
   * 伤害期望（带暴击）
   * E = 基础伤害 × (1 - 暴击率) + 基础伤害 × 暴击率 × 暴击倍率
   *   = 基础伤害 × (1 + 暴击率 × (暴击倍率 - 1))
   */
  calculateExpectedDamage(baseDamage, critChance, critDamage) {
    return baseDamage * (1 + critChance * (critDamage - 1));
  },

  /**
   * 几何分布：平均多少次尝试才第一次成功
   * E = 1 / p
   * 例如稀有卡概率1/120，平均需要120次
   */
  calculateExpectedAttempts(successProbability) {
    return successProbability > 0 ? 1 / successProbability : Infinity;
  },

  /**
   * 集齐全套N种物品的期望购买次数（调和级数）
   * E = n × (1 + 1/2 + 1/3 + ... + 1/n)
   * 例如10张卡，平均需要约29.3包
   */
  calculateCollectionExpected(totalTypes) {
    let harmonic = 0;
    for (let i = 1; i <= totalTypes; i++) {
      harmonic += 1 / i;
    }
    return totalTypes * harmonic;
  },

  /**
   * 已有k张，下一包出新卡概率
   * P_new = (n - k) / n
   */
  calculateNewCardProbability(totalTypes, ownedTypes) {
    return Math.max(0, (totalTypes - ownedTypes) / totalTypes);
  },

  // ==================== 伤害计算 ====================

  // 计算普通攻击伤害
  calculateAttackDamage(attacker, defender) {
    const baseDamage = attacker.damage || 10;
    const armor = defender.armor || 0;
    const critChance = attacker.crit || 0.05;
    const critDamage = attacker.critDamage || 1.5;

    // 暴击判定
    const isCrit = Math.random() < critChance;
    let damage = baseDamage;

    if (isCrit) {
      damage *= critDamage;
      AchievementSystem.updateStat('criticalHits');
    }

    // 百分比减伤（防御收益递减，不会出现0伤害）
    damage = this.calculateDamageReduction(damage, armor);

    // 伤害波动（±10%）
    damage *= 0.9 + Math.random() * 0.2;

    return {
      damage: Math.max(1, Math.floor(damage)),
      isCrit,
      isMiss: false,
      isBlock: false,
      expectedDamage: this.calculateExpectedDamage(baseDamage, critChance, critDamage)
    };
  },

  // 计算技能伤害
  calculateSkillDamage(attacker, defender, skill) {
    if (!skill || !skill.effects) return { damage: 0, isCrit: false };

    const baseDamage = attacker.damage || 10;
    const multiplier = skill.effects.damageMultiplier || 1;
    let damage = baseDamage * multiplier;

    // 技能附加伤害
    if (skill.effects.bonusDamage) {
      damage += skill.effects.bonusDamage;
    }

    // 属性加成
    if (skill.effects.statScaling) {
      for (const stat in skill.effects.statScaling) {
        damage += (attacker[stat] || 0) * skill.effects.statScaling[stat];
      }
    }

    // 暴击
    const critChance = (attacker.crit || 0.05) + (skill.effects.critBonus || 0);
    const isCrit = skill.effects.guaranteedCrit || Math.random() < critChance;
    if (isCrit) {
      damage *= (attacker.critDamage || 1.5);
    }

    // 防御方抗性
    if (skill.effects.damageType && defender) {
      const resistStat = skill.effects.damageType + 'Resist';
      const resist = defender[resistStat] || 0;
      damage *= (1 - resist / 100);
    }

    // 护甲减伤（物理伤害）- 百分比减伤，防御收益递减
    if (!skill.effects.damageType || skill.effects.damageType === 'physical') {
      if (defender && defender.armor) {
        damage = this.calculateDamageReduction(damage, defender.armor);
      }
    }

    return {
      damage: Math.max(1, Math.floor(damage)),
      isCrit,
      isMiss: false,
      expectedDamage: this.calculateExpectedDamage(baseDamage * multiplier, critChance, attacker.critDamage || 1.5)
    };
  },

  // 执行攻击
  performAttack(attacker, defender) {
    // 检查闪避
    const dodgeChance = defender.dodge || 0;
    if (Math.random() < dodgeChance) {
      ParticleSystem.damageNumber(defender.x, defender.y, 0, false, false, true);
      return { hit: false, reason: 'dodge' };
    }

    // 检查格挡
    const blockChance = defender.block || 0;
    if (Math.random() < blockChance) {
      ParticleSystem.damageNumber(defender.x, defender.y, 0, false, false, false);
      AudioSystem.playSound('block');
      return { hit: false, reason: 'block' };
    }

    // 计算伤害
    const result = this.calculateAttackDamage(attacker, defender);

    // 应用伤害
    defender.takeDamage(result.damage, attacker);

    // 显示伤害数字
    ParticleSystem.damageNumber(defender.x, defender.y, result.damage, result.isCrit);

    // 吸血
    if (attacker.lifesteal && attacker.lifesteal > 0) {
      const heal = Math.floor(result.damage * attacker.lifesteal);
      attacker.hp = Math.min(attacker.maxHp, attacker.hp + heal);
      if (heal > 0) {
        ParticleSystem.damageNumber(attacker.x, attacker.y, heal, false, true);
      }
    }

    // 反伤
    if (defender.thorns && defender.thorns > 0) {
      const thornDamage = Math.floor(result.damage * defender.thorns);
      attacker.takeDamage(thornDamage, defender);
      ParticleSystem.damageNumber(attacker.x, attacker.y, thornDamage);
    }

    // 音效
    if (result.isCrit) {
      AudioSystem.playSound('crit');
    } else {
      AudioSystem.playSound('hit');
    }

    AchievementSystem.recordDamageDealt(result.damage, result.isCrit);

    return { hit: true, ...result };
  },

  // 执行技能
  performSkill(attacker, defender, skillId, targetX, targetY) {
    const skill = SkillData.getSkill(skillId);
    if (!skill) return { success: false, message: '技能不存在' };

    // 检查魔法值
    if (attacker.mp < skill.manaCost) {
      return { success: false, message: '魔法值不足' };
    }

    // 检查冷却
    if (!attacker.skillCooldowns) attacker.skillCooldowns = {};
    if (attacker.skillCooldowns[skillId] && attacker.skillCooldowns[skillId] > 0) {
      return { success: false, message: '技能冷却中' };
    }

    // 检查沉默
    if (BuffSystem.hasBuff(attacker, 'silence')) {
      return { success: false, message: '被沉默了' };
    }

    // 消耗魔法
    attacker.mp -= skill.manaCost;
    attacker.skillCooldowns[skillId] = skill.cooldown;

    AchievementSystem.updateStat('skillsUsed');

    // 根据技能类型执行
    switch (skill.type) {
      case 'active':
        return this.executeActiveSkill(attacker, defender, skill, targetX, targetY);
      case 'buff':
        return this.executeBuffSkill(attacker, skill);
      case 'ultimate':
        return this.executeUltimateSkill(attacker, defender, skill, targetX, targetY);
      default:
        return { success: false, message: '未知技能类型' };
    }
  },

  // 执行主动技能
  executeActiveSkill(attacker, defender, skill, targetX, targetY) {
    AudioSystem.playSound('spell');

    // 投射物技能
    if (skill.projectileSpeed) {
      if (window.Game) {
        window.Game.createProjectile({
          x: attacker.x,
          y: attacker.y,
          targetX,
          targetY,
          speed: skill.projectileSpeed,
          damage: this.calculateSkillDamage(attacker, defender, skill).damage,
          skill,
          owner: attacker
        });
      }
      return { success: true };
    }

    // 范围技能
    if (skill.effects.radius) {
      const radius = skill.effects.radius;
      const targets = this.getTargetsInArea(targetX, targetY, radius, attacker);

      for (const target of targets) {
        const damageResult = this.calculateSkillDamage(attacker, target, skill);
        target.takeDamage(damageResult.damage, attacker);
        ParticleSystem.damageNumber(target.x, target.y, damageResult.damage, damageResult.isCrit);

        // 应用状态效果
        this.applySkillEffects(target, skill);
      }

      // 特效
      if (skill.animation) {
        this.playSkillAnimation(targetX, targetY, skill);
      }

      return { success: true, targets: targets.length };
    }

    // 单体技能
    if (defender) {
      const damageResult = this.calculateSkillDamage(attacker, defender, skill);
      defender.takeDamage(damageResult.damage, attacker);
      ParticleSystem.damageNumber(defender.x, defender.y, damageResult.damage, damageResult.isCrit);
      this.applySkillEffects(defender, skill);

      if (skill.animation) {
        this.playSkillAnimation(defender.x, defender.y, skill);
      }

      return { success: true, damage: damageResult.damage };
    }

    return { success: true };
  },

  // 执行Buff技能
  executeBuffSkill(attacker, skill) {
    AudioSystem.playSound('spell');

    if (skill.effects) {
      for (const effect in skill.effects) {
        switch (effect) {
          case 'damageBoost':
            BuffSystem.applyBuff(attacker, 'damageBoost', null, skill.duration);
            break;
          case 'speedBoost':
            BuffSystem.applyBuff(attacker, 'speedBoost', null, skill.duration);
            break;
          case 'attackSpeedBoost':
            BuffSystem.applyBuff(attacker, 'attackSpeedBoost', null, skill.duration);
            break;
          case 'shieldPercent':
            const shieldAmount = Math.floor(attacker.maxHp * skill.effects.shieldPercent);
            attacker.shield = (attacker.shield || 0) + shieldAmount;
            ParticleSystem.magic(attacker.x, attacker.y, '#3498db');
            break;
          case 'invincibility':
            BuffSystem.applyBuff(attacker, 'invincibility', null, skill.duration);
            break;
          case 'stealth':
            BuffSystem.applyBuff(attacker, 'stealth', null, skill.duration);
            break;
          case 'healPercent':
            const heal = Math.floor(attacker.maxHp * skill.effects.healPercent);
            attacker.hp = Math.min(attacker.maxHp, attacker.hp + heal);
            ParticleSystem.heal(attacker.x, attacker.y);
            AudioSystem.playSound('heal');
            break;
          case 'nextAttackDamage':
            attacker.nextAttackBonus = skill.effects.nextAttackDamage;
            break;
        }
      }
    }

    if (skill.animation) {
      this.playSkillAnimation(attacker.x, attacker.y, skill);
    }

    return { success: true };
  },

  // 执行终极技能
  executeUltimateSkill(attacker, defender, skill, targetX, targetY) {
    AudioSystem.playSound('boss');
    // 终极技能有更华丽的效果
    return this.executeActiveSkill(attacker, defender, skill, targetX, targetY);
  },

  // 应用技能效果
  applySkillEffects(target, skill) {
    if (!skill.effects) return;

    if (skill.effects.burn) {
      BuffSystem.applyBuff(target, 'burn', null, skill.effects.burn.duration);
    }
    if (skill.effects.poison) {
      BuffSystem.applyBuff(target, 'poison', null, skill.effects.poison.duration);
    }
    if (skill.effects.stun) {
      BuffSystem.applyBuff(target, 'stun', null, skill.effects.stun);
    }
    if (skill.effects.slow) {
      BuffSystem.applyBuff(target, 'slow', null, skill.effects.slow);
    }
    if (skill.effects.freeze) {
      BuffSystem.applyBuff(target, 'freeze', null, skill.effects.freeze);
    }
    if (skill.effects.silence) {
      BuffSystem.applyBuff(target, 'silence', null, skill.effects.silence);
    }
    if (skill.effects.confuse) {
      BuffSystem.applyBuff(target, 'confuse', null, skill.effects.confuse);
    }
    if (skill.effects.knockback) {
      // 击退效果
      const angle = Math.atan2(target.y - (target.sourceY || 0), target.x - (target.sourceX || 0));
      target.x += Math.cos(angle) * skill.effects.knockback;
      target.y += Math.sin(angle) * skill.effects.knockback;
    }
  },

  // 播放技能动画
  playSkillAnimation(x, y, skill) {
    if (!skill.animation) return;

    switch (skill.animation.type) {
      case 'projectile':
        // 已在投射物系统处理
        break;
      case 'aoe':
        ParticleSystem.explosion(x, y, skill.animation.color, skill.animation.radius || 50);
        break;
      case 'explosion':
        ParticleSystem.explosion(x, y, skill.animation.color, skill.effects?.radius || 80);
        break;
      case 'heal':
        ParticleSystem.heal(x, y, 12);
        break;
      case 'buff':
        ParticleSystem.magic(x, y, skill.animation.color);
        break;
      case 'shield':
        ParticleSystem.magic(x, y, skill.animation.color, 15);
        break;
      case 'teleport':
        ParticleSystem.teleport(x, y, skill.animation.color);
        break;
      case 'meteor':
        ParticleSystem.explosion(x, y, '#e67e22', skill.animation.radius || 120, 30);
        break;
      case 'storm':
        for (let i = 0; i < 20; i++) {
          ParticleSystem.ambient(x + (Math.random() - 0.5) * 200, y + (Math.random() - 0.5) * 200, 'dust');
        }
        break;
      case 'beam':
        ParticleSystem.lightning(x, y - 200, x, y + 50, skill.animation.color);
        break;
      case 'spin':
        ParticleSystem.explosion(x, y, skill.animation.color, 80, 15);
        break;
      case 'summon':
        ParticleSystem.magic(x, y, skill.animation.color, 20);
        break;
    }
  },

  // 获取区域内的目标
  getTargetsInArea(x, y, radius, excludeEntity = null) {
    const targets = [];
    if (!window.Game || !window.Game.monsters) return targets;

    for (const monster of window.Game.monsters) {
      if (monster === excludeEntity) continue;
      if (monster.dead) continue;
      const dist = Utils.distance(x, y, monster.x, monster.y);
      if (dist <= radius) {
        targets.push(monster);
      }
    }
    return targets;
  },

  // 更新技能冷却
  updateCooldowns(entity, dt) {
    if (!entity.skillCooldowns) return;
    for (const skillId in entity.skillCooldowns) {
      if (entity.skillCooldowns[skillId] > 0) {
        entity.skillCooldowns[skillId] -= dt;
        if (entity.skillCooldowns[skillId] < 0) {
          entity.skillCooldowns[skillId] = 0;
        }
      }
    }
  },

  // 检查是否在战斗中
  isInCombat(entity) {
    return entity.inCombat || false;
  },

  // 进入战斗
  enterCombat(entity) {
    entity.inCombat = true;
    entity.combatTimer = 5;
  },

  // 退出战斗
  exitCombat(entity) {
    entity.inCombat = false;
  },

  // 更新战斗状态
  updateCombatState(entity, dt) {
    if (entity.inCombat) {
      entity.combatTimer -= dt;
      if (entity.combatTimer <= 0) {
        this.exitCombat(entity);
      }
    }
  }
};

window.CombatSystem = CombatSystem;
