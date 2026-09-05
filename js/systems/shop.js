// ==================== 永恒地牢 - 商店系统 ====================
// 商店物品、购买、出售、刷新、折扣

const ShopData = {
  // ==================== 商店类型 ====================
  shopTypes: {
    weapon: {
      name: '武器店',
      icon: '⚔️',
      items: ['dagger', 'shortSword', 'ironSword', 'steelSword', 'silverSword', 'shortBow', 'longBow', 'apprenticeStaff', 'wizardStaff', 'greatAxe'],
      refreshTime: 300
    },
    armor: {
      name: '护甲店',
      icon: '🛡️',
      items: ['leatherArmor', 'chainmail', 'scaleArmor', 'plateArmor', 'darkRobe', 'leatherHelm', 'ironHelm', 'leatherBoots', 'ironBoots', 'swiftBoots'],
      refreshTime: 300
    },
    potion: {
      name: '药水店',
      icon: '🧪',
      items: ['healthPotion', 'manaPotion', 'staminaPotion', 'superHealthPotion', 'superManaPotion', 'strengthPotion', 'agilityPotion', 'defensePotion', 'antidote', 'cleanse', 'bomb', 'key'],
      refreshTime: 120
    },
    accessory: {
      name: '饰品店',
      icon: '💍',
      items: ['ringOfPower', 'ringOfAgility', 'ringOfWisdom', 'ringOfVitality', 'amuletOfWisdom', 'amuletOfPower', 'luckyCharm'],
      refreshTime: 600
    },
    general: {
      name: '杂货铺',
      icon: '📦',
      items: ['healthPotion', 'manaPotion', 'key', 'bomb', 'scrollOfTeleport', 'scrollOfIdentify', 'antidote'],
      refreshTime: 180
    },
    mystery: {
      name: '神秘商人',
      icon: '🎭',
      items: ['invisibilityPotion', 'fullPotion', 'fireBomb', 'flameBlade', 'shadowDagger', 'dragonScale', 'demonHeart', 'heartOfDracula', 'phylactery'],
      refreshTime: 900,
      priceMultiplier: 1.5
    },
    blackmarket: {
      name: '黑市',
      icon: '🌑',
      items: ['excalibur', 'frostmourne', 'abyssBlade', 'divineArmor', 'abyssArmor', 'abyssRing', 'dragonHeart', 'demonHeart'],
      refreshTime: 1200,
      priceMultiplier: 2.0
    }
  },

  // 获取商店物品列表
  getShopItems(shopType, playerLevel = 1) {
    const shop = this.shopTypes[shopType];
    if (!shop) return [];

    const items = [];
    for (const itemId of shop.items) {
      const item = ItemData.getItem(itemId);
      if (!item) continue;

      // 根据玩家等级过滤
      if (item.requiredLevel && item.requiredLevel > playerLevel + 5) continue;

      const price = Math.floor(item.price * (shop.priceMultiplier || 1));
      const stock = item.stackable ? 5 + Math.floor(Math.random() * 10) : 1;

      items.push({
        itemId,
        ...item,
        price,
        stock,
        maxStock: stock
      });
    }
    return items;
  },

  // 计算出售价格
  getSellPrice(item) {
    if (!item) return 0;
    return Math.floor((item.price || 0) * 0.4);
  }
};

// ==================== 商店管理器 ====================
const ShopManager = {
  currentShop: null,
  shopItems: [],
  shopType: null,
  lastRefresh: {},
  discount: 0, // 0-1 折扣比例

  init() {
    this.currentShop = null;
    this.shopItems = [];
    this.lastRefresh = {};
    console.log('[ShopManager] 商店系统初始化完成');
  },

  // 打开商店
  openShop(shopType, playerLevel = 1) {
    this.shopType = shopType;
    this.shopItems = ShopData.getShopItems(shopType, playerLevel);
    this.currentShop = ShopData.shopTypes[shopType];
    this.lastRefresh[shopType] = Date.now();
    console.log(`[Shop] 打开商店: ${this.currentShop?.name}`);
  },

  // 关闭商店
  closeShop() {
    this.currentShop = null;
    this.shopItems = [];
    this.shopType = null;
  },

  // 刷新商店
  refreshShop(playerLevel = 1) {
    if (!this.shopType) return;
    this.shopItems = ShopData.getShopItems(this.shopType, playerLevel);
    this.lastRefresh[this.shopType] = Date.now();
    AudioSystem.playSound('click');
  },

  // 购买物品
  buyItem(itemIndex, player, quantity = 1) {
    const shopItem = this.shopItems[itemIndex];
    if (!shopItem) {
      return { success: false, message: '物品不存在' };
    }

    if (shopItem.stock < quantity) {
      return { success: false, message: '库存不足' };
    }

    const totalPrice = Math.floor(shopItem.price * quantity * (1 - this.discount));
    if (player.gold < totalPrice) {
      return { success: false, message: '金币不足' };
    }

    // 检查背包空间
    if (!shopItem.stackable && InventorySystem.getEmptySlots(player.inventory) < 1) {
      return { success: false, message: '背包已满' };
    }

    // 扣除金币
    player.gold -= totalPrice;

    // 添加物品
    const result = InventorySystem.addItem(player.inventory, shopItem.itemId, quantity);
    if (!result.success) {
      player.gold += totalPrice; // 退款
      return { success: false, message: result.message || '添加物品失败' };
    }

    // 减少库存
    shopItem.stock -= quantity;

    AudioSystem.playSound('coin');
    ParticleSystem.itemPickup(player.x, player.y, '#f1c40f');

    return {
      success: true,
      message: `购买了 ${shopItem.name} x${quantity}`,
      spent: totalPrice
    };
  },

  // 出售物品
  sellItem(slotIndex, player, quantity = 1) {
    const item = player.inventory.items[slotIndex];
    if (!item) {
      return { success: false, message: '该槽位没有物品' };
    }

    const sellPrice = ShopData.getSellPrice(item);
    if (sellPrice <= 0) {
      return { success: false, message: '该物品无法出售' };
    }

    const sellQuantity = Math.min(quantity, item.quantity || 1);
    const totalPrice = sellPrice * sellQuantity;

    // 移除物品
    InventorySystem.removeItem(player.inventory, slotIndex, sellQuantity);

    // 获得金币
    player.gold += totalPrice;
    AchievementSystem.recordGoldEarned(totalPrice);

    AudioSystem.playSound('coin');

    return {
      success: true,
      message: `出售了 ${item.name} x${sellQuantity}，获得 ${totalPrice} 金币`,
      earned: totalPrice
    };
  },

  // 设置折扣
  setDiscount(discount) {
    this.discount = Utils.clamp(discount, 0, 0.9);
  },

  // 检查是否需要刷新
  needsRefresh(shopType) {
    const shop = ShopData.shopTypes[shopType];
    if (!shop) return false;
    const last = this.lastRefresh[shopType] || 0;
    return (Date.now() - last) / 1000 > shop.refreshTime;
  },

  // 渲染商店界面
  renderUI(ctx, game) {
    if (!this.currentShop) return;

    const x = 80;
    const y = 60;
    const width = game.canvas.width - 160;
    const height = game.canvas.height - 120;

    // 背景
    ctx.fillStyle = 'rgba(0,0,0,0.92)';
    ctx.fillRect(x, y, width, height);
    ctx.strokeStyle = '#f1c40f';
    ctx.lineWidth = 2;
    ctx.strokeRect(x, y, width, height);

    // 标题
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 28px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(`${this.currentShop.icon} ${this.currentShop.name}`, x + width / 2, y + 40);

    // 玩家金币
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 18px Arial';
    ctx.textAlign = 'right';
    ctx.fillText(`💰 ${game.player.gold}`, x + width - 30, y + 40);

    // 折扣提示
    if (this.discount > 0) {
      ctx.fillStyle = '#2ecc71';
      ctx.font = '14px Arial';
      ctx.textAlign = 'right';
      ctx.fillText(`折扣: ${Math.floor(this.discount * 100)}% OFF`, x + width - 30, y + 65);
    }

    // 物品列表
    const listX = x + 30;
    const listY = y + 80;
    const itemHeight = 55;
    const columns = 2;
    const itemWidth = (width - 80) / columns;

    this.shopItems.forEach((item, i) => {
      const col = i % columns;
      const row = Math.floor(i / columns);
      const ix = listX + col * itemWidth;
      const iy = listY + row * itemHeight;

      if (iy > y + height - 80) return;

      const canAfford = game.player.gold >= item.price;
      const inStock = item.stock > 0;

      // 背景
      ctx.fillStyle = (canAfford && inStock) ? 'rgba(46, 204, 113, 0.1)' : 'rgba(0,0,0,0.3)';
      ctx.fillRect(ix, iy, itemWidth - 20, itemHeight - 5);

      // 物品图标
      ctx.font = '28px Arial';
      ctx.textAlign = 'left';
      ctx.fillText(item.icon, ix + 10, iy + 30);

      // 物品名称
      const rarityColors = {
        common: '#fff', uncommon: '#2ecc71', rare: '#3498db',
        epic: '#9b59b6', legendary: '#f1c40f', mythic: '#e74c3c'
      };
      ctx.fillStyle = rarityColors[item.rarity] || '#fff';
      ctx.font = 'bold 15px Arial';
      ctx.fillText(item.name, ix + 45, iy + 18);

      // 物品描述
      ctx.fillStyle = '#aaa';
      ctx.font = '11px Arial';
      ctx.fillText(item.description?.substring(0, 25) || '', ix + 45, iy + 35);

      // 价格
      ctx.fillStyle = canAfford ? '#f1c40f' : '#e74c3c';
      ctx.font = 'bold 14px Arial';
      ctx.textAlign = 'right';
      ctx.fillText(`💰 ${item.price}`, ix + itemWidth - 35, iy + 18);

      // 库存
      ctx.fillStyle = inStock ? '#2ecc71' : '#e74c3c';
      ctx.font = '12px Arial';
      ctx.fillText(`库存: ${item.stock}`, ix + itemWidth - 35, iy + 38);

      // 购买按钮
      ctx.fillStyle = (canAfford && inStock) ? '#2ecc71' : '#555';
      ctx.fillRect(ix + itemWidth - 100, iy + 8, 70, 25);
      ctx.fillStyle = '#fff';
      ctx.font = 'bold 12px Arial';
      ctx.textAlign = 'center';
      ctx.fillText('购买', ix + itemWidth - 65, iy + 25);
    });

    // 刷新按钮
    ctx.fillStyle = '#3498db';
    ctx.fillRect(x + width - 180, y + height - 50, 150, 35);
    ctx.fillStyle = '#fff';
    ctx.font = 'bold 14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🔄 刷新商品', x + width - 105, y + height - 27);

    // 关闭提示
    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.fillText('按 ESC 关闭 | 点击物品购买', x + width / 2, y + height - 25);
  },

  // 处理商店点击
  handleClick(mouseX, mouseY, game) {
    if (!this.currentShop) return null;

    const x = 80;
    const y = 60;
    const width = game.canvas.width - 160;
    const height = game.canvas.height - 120;

    // 刷新按钮
    if (mouseX > x + width - 180 && mouseX < x + width - 30 &&
        mouseY > y + height - 50 && mouseY < y + height - 15) {
      this.refreshShop(game.player.level);
      return { type: 'refresh' };
    }

    // 物品点击
    const listX = x + 30;
    const listY = y + 80;
    const itemHeight = 55;
    const columns = 2;
    const itemWidth = (width - 80) / columns;

    for (let i = 0; i < this.shopItems.length; i++) {
      const col = i % columns;
      const row = Math.floor(i / columns);
      const ix = listX + col * itemWidth;
      const iy = listY + row * itemHeight;

      // 购买按钮区域
      if (mouseX > ix + itemWidth - 100 && mouseX < ix + itemWidth - 30 &&
          mouseY > iy + 8 && mouseY < iy + 33) {
        return this.buyItem(i, game.player);
      }
    }
    return null;
  },

  // 保存
  save() {
    return {
      lastRefresh: this.lastRefresh,
      discount: this.discount
    };
  },

  // 加载
  load(data) {
    if (!data) return;
    this.lastRefresh = data.lastRefresh || {};
    this.discount = data.discount || 0;
  }
};

window.ShopData = ShopData;
window.ShopManager = ShopManager;
