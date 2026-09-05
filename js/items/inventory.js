// ==================== 永恒地牢 - 背包系统 ====================
// 管理物品的存储、使用、装备、丢弃

const InventorySystem = {
  MAX_SLOTS: 30,
  MAX_GOLD_STACK: 99999,

  // 创建背包
  createInventory() {
    return {
      items: new Array(this.MAX_SLOTS).fill(null),
      gold: 0,
      selectedSlot: -1
    };
  },

  // 添加物品
  addItem(inventory, itemId, quantity = 1) {
    const itemData = ItemData.getItem(itemId);
    if (!itemData) return { success: false, message: '物品不存在' };

    // 金币特殊处理
    if (itemId === 'gold') {
      inventory.gold = Math.min(this.MAX_GOLD_STACK, inventory.gold + quantity);
      return { success: true, added: quantity };
    }

    // 可堆叠物品
    if (itemData.stackable) {
      for (let i = 0; i < this.MAX_SLOTS; i++) {
        const slot = inventory.items[i];
        if (slot && slot.id === itemId && slot.quantity < (itemData.maxStack || 99)) {
          const canAdd = Math.min(quantity, (itemData.maxStack || 99) - slot.quantity);
          slot.quantity += canAdd;
          quantity -= canAdd;
          if (quantity <= 0) return { success: true, added: quantity };
        }
      }
    }

    // 找空槽位
    while (quantity > 0) {
      const emptySlot = inventory.items.findIndex(s => s === null);
      if (emptySlot === -1) {
        return { success: false, message: '背包已满', remaining: quantity };
      }
      const addQuantity = itemData.stackable ? Math.min(quantity, itemData.maxStack || 99) : 1;
      inventory.items[emptySlot] = {
        id: itemId,
        ...itemData,
        quantity: addQuantity
      };
      quantity -= addQuantity;
    }

    return { success: true };
  },

  // 移除物品
  removeItem(inventory, slotIndex, quantity = 1) {
    const slot = inventory.items[slotIndex];
    if (!slot) return { success: false, message: '槽位为空' };

    if (slot.quantity > quantity) {
      slot.quantity -= quantity;
    } else {
      inventory.items[slotIndex] = null;
    }
    return { success: true };
  },

  // 移除指定ID的物品
  removeItemById(inventory, itemId, quantity = 1) {
    let remaining = quantity;
    for (let i = 0; i < this.MAX_SLOTS && remaining > 0; i++) {
      const slot = inventory.items[i];
      if (slot && slot.id === itemId) {
        const remove = Math.min(remaining, slot.quantity);
        slot.quantity -= remove;
        remaining -= remove;
        if (slot.quantity <= 0) {
          inventory.items[i] = null;
        }
      }
    }
    return remaining === 0 ? { success: true } : { success: false, remaining };
  },

  // 使用物品
  useItem(inventory, slotIndex, player) {
    const slot = inventory.items[slotIndex];
    if (!slot) return { success: false, message: '槽位为空' };

    if (slot.type === 'consumable') {
      return this.useConsumable(slot, player, inventory, slotIndex);
    } else if (slot.type === 'weapon' || slot.type === 'armor' || slot.type === 'accessory') {
      return this.equipItem(slot, player, inventory, slotIndex);
    } else if (slot.type === 'key') {
      return { success: false, message: '钥匙需要在宝箱旁使用' };
    }

    return { success: false, message: '该物品无法使用' };
  },

  // 使用消耗品
  useConsumable(item, player, inventory, slotIndex) {
    if (!item.effect) return { success: false, message: '物品无效果' };

    const effect = item.effect;

    switch (effect.type) {
      case 'heal':
        player.hp = Math.min(player.maxHp, player.hp + effect.value);
        ParticleSystem.heal(player.x, player.y);
        AudioSystem.playSound('potion');
        break;
      case 'mana':
        player.mp = Math.min(player.maxMp, player.mp + effect.value);
        ParticleSystem.magic(player.x, player.y, '#3498db');
        AudioSystem.playSound('potion');
        break;
      case 'stamina':
        player.stamina = Math.min(player.maxStamina, player.stamina + effect.value);
        break;
      case 'full':
        player.hp = player.maxHp;
        player.mp = player.maxMp;
        player.stamina = player.maxStamina;
        ParticleSystem.heal(player.x, player.y, 15);
        AudioSystem.playSound('heal');
        break;
      case 'buff':
        BuffSystem.applyBuff(player, effect.stat + '_buff', null, effect.duration);
        AudioSystem.playSound('spell');
        break;
      case 'stealth':
        BuffSystem.applyBuff(player, 'stealth', null, effect.duration);
        AudioSystem.playSound('stealth');
        break;
      case 'damage':
        // 对周围敌人造成伤害
        if (window.Game && window.Game.monsters) {
          for (const monster of window.Game.monsters) {
            const dist = Utils.distance(player.x, player.y, monster.x, monster.y);
            if (dist < effect.radius) {
              monster.takeDamage(effect.value, player);
            }
          }
        }
        ParticleSystem.explosion(player.x, player.y, '#e67e22', effect.radius);
        AudioSystem.playSound('explosion');
        break;
      case 'cure':
        BuffSystem.cleanseDebuffs(player);
        AudioSystem.playSound('cleanse');
        break;
      case 'cleanse':
        BuffSystem.cleanseDebuffs(player);
        AudioSystem.playSound('cleanse');
        break;
      case 'teleport':
        if (window.Game) {
          window.Game.teleportToTown();
        }
        AudioSystem.playSound('teleport');
        break;
      case 'identify':
        // 鉴定物品
        break;
    }

    // 消耗物品
    this.removeItem(inventory, slotIndex, 1);
    AchievementSystem.updateStat('potionsUsed');

    return { success: true, message: `使用了 ${item.name}` };
  },

  // 装备物品
  equipItem(item, player, inventory, slotIndex) {
    if (!player.equipment) player.equipment = {};

    const slot = item.slot;
    const oldItem = player.equipment[slot];

    // 卸下旧装备
    if (oldItem) {
      this.addItem(inventory, oldItem.id, 1);
    }

    // 装备新物品
    player.equipment[slot] = { ...item };
    inventory.items[slotIndex] = null;

    // 重新计算属性
    player.calculateStats();
    AudioSystem.playSound('click');

    return { success: true, message: `装备了 ${item.name}` };
  },

  // 卸下装备
  unequipItem(player, slot, inventory) {
    if (!player.equipment || !player.equipment[slot]) {
      return { success: false, message: '该槽位没有装备' };
    }

    const item = player.equipment[slot];
    const result = this.addItem(inventory, item.id, 1);
    if (result.success) {
      player.equipment[slot] = null;
      player.calculateStats();
      return { success: true };
    }
    return result;
  },

  // 检查是否有物品
  hasItem(inventory, itemId, quantity = 1) {
    let count = 0;
    for (const slot of inventory.items) {
      if (slot && slot.id === itemId) {
        count += slot.quantity;
        if (count >= quantity) return true;
      }
    }
    return false;
  },

  // 获取物品数量
  getItemCount(inventory, itemId) {
    let count = 0;
    for (const slot of inventory.items) {
      if (slot && slot.id === itemId) {
        count += slot.quantity;
      }
    }
    return count;
  },

  // 整理背包
  sortInventory(inventory) {
    const items = inventory.items.filter(s => s !== null);
    items.sort((a, b) => {
      const typeOrder = { weapon: 0, armor: 1, accessory: 2, consumable: 3, material: 4, key: 5 };
      const typeDiff = (typeOrder[a.type] || 9) - (typeOrder[b.type] || 9);
      if (typeDiff !== 0) return typeDiff;
      const rarityOrder = { mythic: 0, legendary: 1, epic: 2, rare: 3, uncommon: 4, common: 5 };
      return (rarityOrder[a.rarity] || 9) - (rarityOrder[b.rarity] || 9);
    });
    inventory.items = [...items, ...new Array(this.MAX_SLOTS - items.length).fill(null)];
  },

  // 丢弃物品
  dropItem(inventory, slotIndex, quantity = 1) {
    const slot = inventory.items[slotIndex];
    if (!slot) return { success: false };

    // 在游戏中生成掉落物
    if (window.Game && window.Game.player) {
      window.Game.dropItem(slot.id, window.Game.player.x, window.Game.player.y, Math.min(quantity, slot.quantity));
    }

    return this.removeItem(inventory, slotIndex, quantity);
  },

  // 获取背包中所有物品
  getAllItems(inventory) {
    return inventory.items.filter(s => s !== null);
  },

  // 获取空槽位数量
  getEmptySlots(inventory) {
    return inventory.items.filter(s => s === null).length;
  },

  // 检查背包是否已满
  isFull(inventory) {
    return this.getEmptySlots(inventory) === 0;
  }
};

window.InventorySystem = InventorySystem;
