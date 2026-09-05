// ==================== 永恒地牢 - 合成系统 ====================
// 物品合成、配方、材料消耗、制作成功率

const CraftingData = {
  // ==================== 合成配方 ====================
  recipes: [
    // ===== 药水类 =====
    {
      id: 'health_potion',
      name: '生命药水',
      icon: '❤️',
      category: 'potion',
      result: { itemId: 'healthPotion', quantity: 1 },
      materials: [
        { itemId: 'slimeGel', quantity: 2 },
        { itemId: 'herb', quantity: 1 }
      ],
      level: 1,
      successRate: 1.0,
      description: '基础治疗药水'
    },
    {
      id: 'mana_potion',
      name: '魔法药水',
      icon: '💙',
      category: 'potion',
      result: { itemId: 'manaPotion', quantity: 1 },
      materials: [
        { itemId: 'ectoplasm', quantity: 1 },
        { itemId: 'spiderSilk', quantity: 1 }
      ],
      level: 1,
      successRate: 1.0,
      description: '恢复魔法值'
    },
    {
      id: 'super_health_potion',
      name: '高级生命药水',
      icon: '💖',
      category: 'potion',
      result: { itemId: 'superHealthPotion', quantity: 1 },
      materials: [
        { itemId: 'healthPotion', quantity: 3 },
        { itemId: 'vampireFang', quantity: 1 }
      ],
      level: 5,
      successRate: 0.9,
      description: '更强效的治疗药水'
    },
    {
      id: 'strength_potion',
      name: '力量药水',
      icon: '💪',
      category: 'potion',
      result: { itemId: 'strengthPotion', quantity: 1 },
      materials: [
        { itemId: 'orcTusk', quantity: 2 },
        { itemId: 'healthPotion', quantity: 1 }
      ],
      level: 8,
      successRate: 0.85,
      description: '临时提升力量'
    },
    {
      id: 'invisibility_potion',
      name: '隐身药水',
      icon: '👻',
      category: 'potion',
      result: { itemId: 'invisibilityPotion', quantity: 1 },
      materials: [
        { itemId: 'ectoplasm', quantity: 3 },
        { itemId: 'soulShard', quantity: 1 },
        { itemId: 'manaPotion', quantity: 2 }
      ],
      level: 15,
      successRate: 0.7,
      description: '隐身30秒'
    },
    {
      id: 'full_potion',
      name: '全满药水',
      icon: '🌈',
      category: 'potion',
      result: { itemId: 'fullPotion', quantity: 1 },
      materials: [
        { itemId: 'superHealthPotion', quantity: 2 },
        { itemId: 'superManaPotion', quantity: 2 },
        { itemId: 'soulShard', quantity: 2 }
      ],
      level: 20,
      successRate: 0.6,
      description: '完全恢复生命和魔法'
    },

    // ===== 炸弹类 =====
    {
      id: 'bomb',
      name: '炸弹',
      icon: '💣',
      category: 'bomb',
      result: { itemId: 'bomb', quantity: 2 },
      materials: [
        { itemId: 'stoneFragment', quantity: 3 },
        { itemId: 'slimeGel', quantity: 2 }
      ],
      level: 3,
      successRate: 0.95,
      description: '范围伤害炸弹'
    },
    {
      id: 'fire_bomb',
      name: '火焰炸弹',
      icon: '🔥',
      category: 'bomb',
      result: { itemId: 'fireBomb', quantity: 1 },
      materials: [
        { itemId: 'bomb', quantity: 2 },
        { itemId: 'fireGem', quantity: 1 }
      ],
      level: 10,
      successRate: 0.8,
      description: '造成火焰伤害并点燃'
    },

    // ===== 武器类 =====
    {
      id: 'steel_sword',
      name: '钢剑',
      icon: '⚔️',
      category: 'weapon',
      result: { itemId: 'steelSword', quantity: 1 },
      materials: [
        { itemId: 'ironSword', quantity: 1 },
        { itemId: 'stoneFragment', quantity: 5 }
      ],
      level: 5,
      successRate: 0.9,
      description: '精钢打造的长剑'
    },
    {
      id: 'flame_blade',
      name: '烈焰之刃',
      icon: '🔥',
      category: 'weapon',
      result: { itemId: 'flameBlade', quantity: 1 },
      materials: [
        { itemId: 'steelSword', quantity: 1 },
        { itemId: 'fireGem', quantity: 3 },
        { itemId: 'demonHorn', quantity: 1 }
      ],
      level: 12,
      successRate: 0.7,
      description: '燃烧着永恒火焰的魔剑'
    },
    {
      id: 'dragon_slayer',
      name: '屠龙剑',
      icon: '🐲',
      category: 'weapon',
      result: { itemId: 'dragonSlayer', quantity: 1 },
      materials: [
        { itemId: 'flameBlade', quantity: 1 },
        { itemId: 'dragonScale', quantity: 5 },
        { itemId: 'dragonHeart', quantity: 1 }
      ],
      level: 20,
      successRate: 0.5,
      description: '传说中用于屠龙的圣剑'
    },

    // ===== 护甲类 =====
    {
      id: 'leather_armor',
      name: '皮甲',
      icon: '🦺',
      category: 'armor',
      result: { itemId: 'leatherArmor', quantity: 1 },
      materials: [
        { itemId: 'wolfPelt', quantity: 3 },
        { itemId: 'spiderSilk', quantity: 2 }
      ],
      level: 2,
      successRate: 0.95,
      description: '轻便的皮革护甲'
    },
    {
      id: 'chainmail',
      name: '锁子甲',
      icon: '🛡️',
      category: 'armor',
      result: { itemId: 'chainmail', quantity: 1 },
      materials: [
        { itemId: 'bone', quantity: 10 },
        { itemId: 'stoneFragment', quantity: 5 }
      ],
      level: 6,
      successRate: 0.85,
      description: '铁环编织的护甲'
    },
    {
      id: 'dragon_scale_armor',
      name: '龙鳞甲',
      icon: '🐉',
      category: 'armor',
      result: { itemId: 'dragonScale', quantity: 1 },
      materials: [
        { itemId: 'plateArmor', quantity: 1 },
        { itemId: 'dragonScaleMat', quantity: 8 },
        { itemId: 'dragonHeart', quantity: 1 }
      ],
      level: 20,
      successRate: 0.5,
      description: '用龙鳞打造的护甲'
    },

    // ===== 饰品类 =====
    {
      id: 'ring_of_power',
      name: '力量之戒',
      icon: '💍',
      category: 'accessory',
      result: { itemId: 'ringOfPower', quantity: 1 },
      materials: [
        { itemId: 'orcTusk', quantity: 3 },
        { itemId: 'bone', quantity: 5 }
      ],
      level: 8,
      successRate: 0.75,
      description: '蕴含力量的戒指'
    },
    {
      id: 'lucky_charm',
      name: '幸运符',
      icon: '🍀',
      category: 'accessory',
      result: { itemId: 'luckyCharm', quantity: 1 },
      materials: [
        { itemId: 'soulShard', quantity: 3 },
        { itemId: 'ectoplasm', quantity: 5 },
        { itemId: 'gold', quantity: 500 }
      ],
      level: 15,
      successRate: 0.6,
      description: '带来好运的护符'
    },

    // ===== 材料类 =====
    {
      id: 'iron_ingot',
      name: '铁锭',
      icon: '🔩',
      category: 'material',
      result: { itemId: 'ironIngot', quantity: 1 },
      materials: [
        { itemId: 'stoneFragment', quantity: 3 }
      ],
      level: 1,
      successRate: 1.0,
      description: '基础锻造材料'
    },
    {
      id: 'magic_dust',
      name: '魔法粉尘',
      icon: '✨',
      category: 'material',
      result: { itemId: 'magicDust', quantity: 2 },
      materials: [
        { itemId: 'ectoplasm', quantity: 2 },
        { itemId: 'spiderSilk', quantity: 1 }
      ],
      level: 5,
      successRate: 0.9,
      description: '附魔用的魔法材料'
    },

    // ===== 特殊物品 =====
    {
      id: 'scroll_of_teleport',
      name: '传送卷轴',
      icon: '📜',
      category: 'scroll',
      result: { itemId: 'scrollOfTeleport', quantity: 1 },
      materials: [
        { itemId: 'magicScroll', quantity: 1 },
        { itemId: 'soulShard', quantity: 2 }
      ],
      level: 10,
      successRate: 0.8,
      description: '传送回城镇'
    },
    {
      id: 'key',
      name: '钥匙',
      icon: '🔑',
      category: 'tool',
      result: { itemId: 'key', quantity: 1 },
      materials: [
        { itemId: 'bone', quantity: 2 },
        { itemId: 'stoneFragment', quantity: 1 }
      ],
      level: 3,
      successRate: 0.95,
      description: '可以打开上锁的宝箱'
    }
  ],

  // 获取配方
  getRecipe(recipeId) {
    return this.recipes.find(r => r.id === recipeId);
  },

  // 获取分类配方
  getRecipesByCategory(category) {
    return this.recipes.filter(r => r.category === category);
  },

  // 获取所有分类
  getCategories() {
    return [...new Set(this.recipes.map(r => r.category))];
  },

  // 获取可用配方（根据等级）
  getAvailableRecipes(level) {
    return this.recipes.filter(r => r.level <= level);
  },

  // 检查是否可以合成
  canCraft(recipe, inventory) {
    for (const mat of recipe.materials) {
      const count = InventorySystem.getItemCount(inventory, mat.itemId);
      if (count < mat.quantity) {
        return { canCraft: false, missing: mat.itemId, have: count, need: mat.quantity };
      }
    }
    return { canCraft: true };
  }
};

// ==================== 合成管理器 ====================
const CraftingManager = {
  craftingLevel: 1,
  craftingExp: 0,
  craftingExpToNext: 100,
  knownRecipes: [],
  isCrafting: false,
  craftProgress: 0,

  init() {
    this.craftingLevel = 1;
    this.craftingExp = 0;
    this.knownRecipes = CraftingData.recipes.filter(r => r.level <= 1).map(r => r.id);
    console.log('[CraftingManager] 合成系统初始化完成');
  },

  // 合成物品
  craft(recipeId, inventory) {
    const recipe = CraftingData.getRecipe(recipeId);
    if (!recipe) {
      return { success: false, message: '配方不存在' };
    }

    // 检查等级
    if (recipe.level > this.craftingLevel) {
      return { success: false, message: `需要合成等级 ${recipe.level}` };
    }

    // 检查材料
    const check = CraftingData.canCraft(recipe, inventory);
    if (!check.canCraft) {
      const item = ItemData.getItem(check.missing);
      return { success: false, message: `材料不足: ${item?.name || check.missing} (${check.have}/${check.need})` };
    }

    // 消耗材料
    for (const mat of recipe.materials) {
      InventorySystem.removeItemById(inventory, mat.itemId, mat.quantity);
    }

    // 成功率判定
    const success = Math.random() < recipe.successRate;

    if (success) {
      // 获得产物
      InventorySystem.addItem(inventory, recipe.result.itemId, recipe.result.quantity);

      // 获得合成经验
      this.gainCraftingExp(recipe.level * 10);

      AudioSystem.playSound('success');
      ParticleSystem.magic(window.Game.player.x, window.Game.player.y, '#2ecc71', 15);

      const item = ItemData.getItem(recipe.result.itemId);
      return {
        success: true,
        message: `合成成功: ${item?.name || recipe.result.itemId} x${recipe.result.quantity}`,
        item: recipe.result.itemId,
        quantity: recipe.result.quantity
      };
    } else {
      // 失败，返还部分材料
      AudioSystem.playSound('error');
      ParticleSystem.magic(window.Game.player.x, window.Game.player.y, '#e74c3c', 10);

      // 返还50%材料
      for (const mat of recipe.materials) {
        const refund = Math.floor(mat.quantity * 0.5);
        if (refund > 0) {
          InventorySystem.addItem(inventory, mat.itemId, refund);
        }
      }

      return { success: false, message: '合成失败！返还了部分材料' };
    }
  },

  // 获得合成经验
  gainCraftingExp(amount) {
    this.craftingExp += amount;
    while (this.craftingExp >= this.craftingExpToNext) {
      this.craftingExp -= this.craftingExpToNext;
      this.craftingLevel++;
      this.craftingExpToNext = Math.floor(this.craftingExpToNext * 1.3);

      // 解锁新配方
      const newRecipes = CraftingData.recipes.filter(r => r.level === this.craftingLevel);
      for (const r of newRecipes) {
        if (!this.knownRecipes.includes(r.id)) {
          this.knownRecipes.push(r.id);
        }
      }

      if (window.Game) {
        window.Game.showMessage(`合成等级提升到 ${this.craftingLevel}！`);
      }
      ParticleSystem.levelUp(window.Game.player.x, window.Game.player.y);
      AudioSystem.playSound('levelup');
    }
  },

  // 学习配方
  learnRecipe(recipeId) {
    if (!this.knownRecipes.includes(recipeId)) {
      this.knownRecipes.push(recipeId);
      return true;
    }
    return false;
  },

  // 获取已知配方
  getKnownRecipes() {
    return this.knownRecipes.map(id => CraftingData.getRecipe(id)).filter(r => r);
  },

  // 渲染合成界面
  renderUI(ctx, game) {
    const x = 100;
    const y = 80;
    const width = game.canvas.width - 200;
    const height = game.canvas.height - 160;

    // 背景
    ctx.fillStyle = 'rgba(0,0,0,0.9)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#f1c40f';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    // 标题
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🔨 合成工坊', x + width / 2, y + 40);

    // 合成等级
    ctx.fillStyle = '#fff';
    ctx.font = '16px Arial';
    ctx.textAlign = 'left';
    ctx.fillText(`合成等级: ${this.craftingLevel}`, x + 20, y + 70);
    ctx.fillText(`经验: ${this.craftingExp}/${this.craftingExpToNext}`, x + 20, y + 95);

    // 配方列表
    const recipes = this.getKnownRecipes();
    const listY = y + 120;
    const itemHeight = 50;

    ctx.fillStyle = '#aaa';
    ctx.font = '14px Arial';
    ctx.fillText('已知配方:', x + 20, listY - 10);

    recipes.forEach((recipe, i) => {
      const ry = listY + i * itemHeight;
      if (ry > y + height - 50) return;

      const canCraft = CraftingData.canCraft(recipe, game.player.inventory).canCraft;

      ctx.fillStyle = canCraft ? 'rgba(46, 204, 113, 0.2)' : 'rgba(0,0,0,0.3)';
      ctx.fillRect(x + 20, ry, width - 40, itemHeight - 5);

      ctx.fillStyle = canCraft ? '#2ecc71' : '#888';
      ctx.font = '18px Arial';
      ctx.textAlign = 'left';
      ctx.fillText(`${recipe.icon} ${recipe.name}`, x + 35, ry + 20);

      // 材料
      ctx.fillStyle = '#aaa';
      ctx.font = '12px Arial';
      let matText = '材料: ';
      recipe.materials.forEach((mat, mi) => {
        const item = ItemData.getItem(mat.itemId);
        const have = InventorySystem.getItemCount(game.player.inventory, mat.itemId);
        matText += `${item?.icon || ''}${have}/${mat.quantity}`;
        if (mi < recipe.materials.length - 1) matText += ' ';
      });
      ctx.fillText(matText, x + 35, ry + 38);

      // 成功率
      ctx.fillStyle = recipe.successRate >= 0.8 ? '#2ecc71' : recipe.successRate >= 0.5 ? '#f1c40f' : '#e74c3c';
      ctx.textAlign = 'right';
      ctx.fillText(`成功率: ${Math.floor(recipe.successRate * 100)}%`, x + width - 120, ry + 20);

      // 合成按钮
      ctx.fillStyle = canCraft ? '#2ecc71' : '#555';
      ctx.fillRect(x + width - 100, ry + 8, 80, 30);
      ctx.fillStyle = '#fff';
      ctx.font = 'bold 14px Arial';
      ctx.textAlign = 'center';
      ctx.fillText('合成', x + width - 60, ry + 28);
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 ESC 关闭', x + width / 2, y + height - 20);
  },

  // 保存
  save() {
    return {
      craftingLevel: this.craftingLevel,
      craftingExp: this.craftingExp,
      knownRecipes: this.knownRecipes
    };
  },

  // 加载
  load(data) {
    if (!data) return;
    this.craftingLevel = data.craftingLevel || 1;
    this.craftingExp = data.craftingExp || 0;
    this.knownRecipes = data.knownRecipes || CraftingData.recipes.filter(r => r.level <= 1).map(r => r.id);
  }
};

window.CraftingData = CraftingData;
window.CraftingManager = CraftingManager;
