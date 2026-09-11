// ==================== 永恒地牢 - 强化/附魔系统 ====================
// 装备强化、附魔、属性提升、成功率

const EnchantData = {
  // ==================== 附魔类型 ====================
  enchants: {
    sharpness: {
      id: 'sharpness',
      name: '锋利',
      icon: '⚔️',
      type: 'weapon',
      maxLevel: 5,
      effect: { stat: 'damage', perLevel: 5 },
      description: '每级增加5点攻击力',
      materials: [{ itemId: 'ironIngot', quantity: 2 }, { itemId: 'magicDust', quantity: 1 }],
      baseCost: 100,
      baseSuccessRate: 0.9
    },
    fire_enchant: {
      id: 'fire_enchant',
      name: '火焰',
      icon: '🔥',
      type: 'weapon',
      maxLevel: 3,
      effect: { stat: 'burnChance', perLevel: 0.1 },
      description: '每级增加10%点燃几率',
      materials: [{ itemId: 'fireGem', quantity: 2 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.8
    },
    ice_enchant: {
      id: 'ice_enchant',
      name: '冰霜',
      icon: '❄️',
      type: 'weapon',
      maxLevel: 3,
      effect: { stat: 'slowChance', perLevel: 0.1 },
      description: '每级增加10%减速几率',
      materials: [{ itemId: 'spiderSilk', quantity: 3 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.8
    },
    lifesteal_enchant: {
      id: 'lifesteal_enchant',
      name: '吸血',
      icon: '🩸',
      type: 'weapon',
      maxLevel: 3,
      effect: { stat: 'lifesteal', perLevel: 0.05 },
      description: '每级增加5%吸血',
      materials: [{ itemId: 'vampireFang', quantity: 2 }, { itemId: 'bloodVial', quantity: 2 }],
      baseCost: 300,
      baseSuccessRate: 0.7
    },
    critical_enchant: {
      id: 'critical_enchant',
      name: '暴击',
      icon: '💥',
      type: 'weapon',
      maxLevel: 5,
      effect: { stat: 'crit', perLevel: 0.03 },
      description: '每级增加3%暴击率',
      materials: [{ itemId: 'soulShard', quantity: 2 }, { itemId: 'magicDust', quantity: 3 }],
      baseCost: 250,
      baseSuccessRate: 0.75
    },

    protection: {
      id: 'protection',
      name: '保护',
      icon: '🛡️',
      type: 'armor',
      maxLevel: 5,
      effect: { stat: 'armor', perLevel: 5 },
      description: '每级增加5点护甲',
      materials: [{ itemId: 'ironIngot', quantity: 3 }, { itemId: 'magicDust', quantity: 1 }],
      baseCost: 100,
      baseSuccessRate: 0.9
    },
    health_enchant: {
      id: 'health_enchant',
      name: '生命',
      icon: '❤️',
      type: 'armor',
      maxLevel: 5,
      effect: { stat: 'maxHp', perLevel: 20 },
      description: '每级增加20点最大生命',
      materials: [{ itemId: 'vampireFang', quantity: 1 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.85
    },
    mana_enchant: {
      id: 'mana_enchant',
      name: '魔力',
      icon: '💙',
      type: 'armor',
      maxLevel: 5,
      effect: { stat: 'maxMp', perLevel: 15 },
      description: '每级增加15点最大魔法',
      materials: [{ itemId: 'ectoplasm', quantity: 2 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.85
    },
    fire_resist: {
      id: 'fire_resist',
      name: '火焰抗性',
      icon: '🔥',
      type: 'armor',
      maxLevel: 3,
      effect: { stat: 'fireResist', perLevel: 10 },
      description: '每级增加10%火焰抗性',
      materials: [{ itemId: 'fireGem', quantity: 1 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 150,
      baseSuccessRate: 0.85
    },
    thorns_enchant: {
      id: 'thorns_enchant',
      name: '荆棘',
      icon: '🌵',
      type: 'armor',
      maxLevel: 3,
      effect: { stat: 'thorns', perLevel: 0.05 },
      description: '每级增加5%反伤',
      materials: [{ itemId: 'stoneFragment', quantity: 5 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 250,
      baseSuccessRate: 0.75
    },

    wisdom_enchant: {
      id: 'wisdom_enchant',
      name: '智慧',
      icon: '📖',
      type: 'accessory',
      maxLevel: 5,
      effect: { stat: 'int', perLevel: 3 },
      description: '每级增加3点智力',
      materials: [{ itemId: 'ectoplasm', quantity: 3 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.8
    },
    strength_enchant: {
      id: 'strength_enchant',
      name: '力量',
      icon: '💪',
      type: 'accessory',
      maxLevel: 5,
      effect: { stat: 'str', perLevel: 3 },
      description: '每级增加3点力量',
      materials: [{ itemId: 'orcTusk', quantity: 2 }, { itemId: 'magicDust', quantity: 2 }],
      baseCost: 200,
      baseSuccessRate: 0.8
    },
    luck_enchant: {
      id: 'luck_enchant',
      name: '幸运',
      icon: '🍀',
      type: 'accessory',
      maxLevel: 3,
      effect: { stat: 'luck', perLevel: 5 },
      description: '每级增加5点幸运',
      materials: [{ itemId: 'soulShard', quantity: 3 }, { itemId: 'gold', quantity: 500 }],
      baseCost: 500,
      baseSuccessRate: 0.6
    }
  },

  // 获取附魔
  getEnchant(enchantId) {
    return this.enchants[enchantId] || null;
  },

  // 获取可用附魔（根据物品类型）
  getAvailableEnchants(itemType) {
    return Object.values(this.enchants).filter(e => e.type === itemType);
  },

  // 计算强化费用
  getEnhanceCost(item, targetLevel) {
    const baseCost = (item.price || 100) * 0.5;
    return Math.floor(baseCost * Math.pow(1.5, targetLevel));
  },

  // 计算强化成功率
  getEnhanceSuccessRate(item, targetLevel) {
    const baseRate = 0.9;
    return Math.max(0.1, baseRate - targetLevel * 0.08);
  }
};

// ==================== 强化管理器 ====================
const EnchantManager = {
  selectedItem: null,
  selectedEnchant: null,
  enhanceLevel: 0,

  init() {
    this.selectedItem = null;
    this.selectedEnchant = null;
    console.log('[EnchantManager] 强化系统初始化完成');
  },

  // 强化装备
  enhanceItem(item, player) {
    if (!item) return { success: false, message: '请选择装备' };
    if (item.type !== 'weapon' && item.type !== 'armor' && item.type !== 'accessory') {
      return { success: false, message: '该物品无法强化' };
    }

    const currentLevel = item.enhanceLevel || 0;
    if (currentLevel >= 10) {
      return { success: false, message: '已达到最高强化等级' };
    }

    const targetLevel = currentLevel + 1;
    const cost = EnchantData.getEnhanceCost(item, targetLevel);
    const successRate = EnchantData.getEnhanceSuccessRate(item, targetLevel);

    if (player.gold < cost) {
      return { success: false, message: `金币不足，需要 ${cost} 金币` };
    }

    player.gold -= cost;

    if (Math.random() < successRate) {
      // 强化成功
      item.enhanceLevel = targetLevel;
      if (!item.enhanceStats) item.enhanceStats = {};
      item.enhanceStats.damage = (item.enhanceStats.damage || 0) + 3;
      item.enhanceStats.armor = (item.enhanceStats.armor || 0) + 2;

      // 重新计算玩家属性
      player.calculateStats();

      AudioSystem.playSound('success');
      ParticleSystem.magic(player.x, player.y, '#f1c40f', 15);

      return {
        success: true,
        message: `强化成功！+${targetLevel}`,
        newLevel: targetLevel
      };
    } else {
      // 强化失败
      AudioSystem.playSound('error');
      ParticleSystem.magic(player.x, player.y, '#e74c3c', 10);

      // 有几率降级
      if (Math.random() < 0.3 && currentLevel > 0) {
        item.enhanceLevel = currentLevel - 1;
        player.calculateStats();
        return { success: false, message: `强化失败！等级降至 +${currentLevel - 1}` };
      }

      return { success: false, message: '强化失败！' };
    }
  },

  // 附魔装备
  enchantItem(item, enchantId, player) {
    if (!item) return { success: false, message: '请选择装备' };

    const enchant = EnchantData.getEnchant(enchantId);
    if (!enchant) return { success: false, message: '附魔不存在' };

    // 检查物品类型
    if (enchant.type !== item.type && enchant.type !== item.slot) {
      return { success: false, message: '该附魔不适用于此物品' };
    }

    // 检查当前附魔等级
    const currentLevel = item.enchants?.[enchantId] || 0;
    if (currentLevel >= enchant.maxLevel) {
      return { success: false, message: '已达到最高附魔等级' };
    }

    const targetLevel = currentLevel + 1;
    const cost = Math.floor(enchant.baseCost * targetLevel);
    const successRate = Math.max(0.3, enchant.baseSuccessRate - targetLevel * 0.1);

    // 检查金币
    if (player.gold < cost) {
      return { success: false, message: `金币不足，需要 ${cost} 金币` };
    }

    // 检查材料
    for (const mat of enchant.materials) {
      const have = InventorySystem.getItemCount(player.inventory, mat.itemId);
      if (have < mat.quantity * targetLevel) {
        const itemData = ItemData.getItem(mat.itemId);
        return { success: false, message: `材料不足: ${itemData?.name} (${have}/${mat.quantity * targetLevel})` };
      }
    }

    // 扣除金币和材料
    player.gold -= cost;
    for (const mat of enchant.materials) {
      InventorySystem.removeItemById(player.inventory, mat.itemId, mat.quantity * targetLevel);
    }

    if (Math.random() < successRate) {
      // 附魔成功
      if (!item.enchants) item.enchants = {};
      item.enchants[enchantId] = targetLevel;

      // 应用附魔效果
      if (!item.enchantStats) item.enchantStats = {};
      item.enchantStats[enchant.effect.stat] = (item.enchantStats[enchant.effect.stat] || 0) + enchant.effect.perLevel;

      player.calculateStats();

      AudioSystem.playSound('success');
      ParticleSystem.magic(player.x, player.y, '#9b59b6', 20);

      return {
        success: true,
        message: `附魔成功！${enchant.name} Lv.${targetLevel}`,
        enchant: enchantId,
        level: targetLevel
      };
    } else {
      AudioSystem.playSound('error');
      ParticleSystem.magic(player.x, player.y, '#e74c3c', 10);
      return { success: false, message: '附魔失败！材料已消耗' };
    }
  },

  // 移除附魔
  removeEnchant(item, enchantId, player) {
    if (!item || !item.enchants || !item.enchants[enchantId]) {
      return { success: false, message: '该物品没有此附魔' };
    }

    const enchant = EnchantData.getEnchant(enchantId);
    const level = item.enchants[enchantId];
    const cost = Math.floor(enchant.baseCost * level * 0.5);

    if (player.gold < cost) {
      return { success: false, message: `金币不足，需要 ${cost} 金币` };
    }

    player.gold -= cost;
    delete item.enchants[enchantId];

    // 移除附魔属性
    if (item.enchantStats && item.enchantStats[enchant.effect.stat]) {
      item.enchantStats[enchant.effect.stat] -= enchant.effect.perLevel * level;
      if (item.enchantStats[enchant.effect.stat] <= 0) {
        delete item.enchantStats[enchant.effect.stat];
      }
    }

    player.calculateStats();
    AudioSystem.playSound('click');

    return { success: true, message: '附魔已移除' };
  },

  // 获取物品的附魔描述
  getItemEnchantDesc(item) {
    if (!item || !item.enchants) return [];
    const descs = [];
    for (const enchantId in item.enchants) {
      const enchant = EnchantData.getEnchant(enchantId);
      if (enchant) {
        descs.push(`${enchant.icon} ${enchant.name} Lv.${item.enchants[enchantId]}`);
      }
    }
    return descs;
  },

  // 渲染强化界面
  renderUI(ctx, game) {
    const x = 100;
    const y = 60;
    const width = game.canvas.width - 200;
    const height = game.canvas.height - 120;

    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#9b59b6';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    ctx.fillStyle = '#9b59b6';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('✨ 强化与附魔', x + width / 2, y + 40);

    // 装备选择区
    ctx.fillStyle = '#fff';
    ctx.font = '16px Arial';
    ctx.textAlign = 'left';
    ctx.fillText('选择装备:', x + 30, y + 80);

    // 显示玩家装备
    const slots = ['weapon', 'armor', 'helmet', 'boots', 'ring', 'amulet'];
    const slotNames = ['武器', '护甲', '头盔', '靴子', '戒指', '护符'];
    slots.forEach((slot, i) => {
      const ix = x + 30 + (i % 3) * 180;
      const iy = y + 100 + Math.floor(i / 3) * 70;
      const item = game.player.equipment[slot];

      ctx.fillStyle = item ? 'rgba(155, 89, 182, 0.2)' : 'rgba(0,0,0,0.3)';
      ctx.fillRect(ix, iy, 160, 55);
      ctx.strokeStyle = '#555';
      ctx.strokeRect(ix, iy, 160, 55);

      ctx.fillStyle = '#888';
      ctx.font = '12px Arial';
      ctx.fillText(slotNames[i], ix + 10, iy + 18);

      if (item) {
        ctx.font = '20px Arial';
        ctx.fillText(item.icon, ix + 10, iy + 42);
        ctx.fillStyle = '#fff';
        ctx.font = '13px Arial';
        ctx.fillText(item.name, ix + 40, iy + 35);
        if (item.enhanceLevel) {
          ctx.fillStyle = '#f1c40f';
          ctx.fillText(`+${item.enhanceLevel}`, ix + 40, iy + 50);
        }
      } else {
        ctx.fillStyle = '#555';
        ctx.font = '14px Arial';
        ctx.fillText('空', ix + 70, iy + 38);
      }
    });

    // 操作说明
    ctx.fillStyle = '#aaa';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('点击装备进行强化 | 强化需要金币，有失败几率', x + width / 2, y + height - 50);
    ctx.fillText('按 ESC 关闭', x + width / 2, y + height - 25);
  },

  // 保存
  save() {
    return {};
  },

  // 加载
  load(data) {
  }
};

window.EnchantData = EnchantData;
window.EnchantManager = EnchantManager;
