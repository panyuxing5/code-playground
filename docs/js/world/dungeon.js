// ==================== 永恒地牢 - 地牢生成系统 ====================
// 程序化生成地牢地图、房间、走廊、道具

const DungeonGenerator = {
  TILE_SIZE: 32,
  TILE_TYPES: {
    FLOOR: 0,
    WALL: 1,
    DOOR: 2,
    STAIRS_DOWN: 3,
    STAIRS_UP: 4,
    CHEST: 5,
    TRAP: 6,
    WATER: 7,
    LAVA: 8,
    TORCH: 9,
    ALTAR: 10,
    SHOP: 11,
    NPC: 12
  },

  // 生成地牢
  generate(floor, width = 60, height = 45) {
    console.log(`[Dungeon] 生成第 ${floor} 层地牢...`);

    const dungeon = {
      floor,
      width,
      height,
      tiles: [],
      rooms: [],
      corridors: [],
      spawnPoint: { x: 0, y: 0 },
      stairsDown: { x: 0, y: 0 },
      stairsUp: { x: 0, y: 0 },
      chests: [],
      traps: [],
      npcs: [],
      monsters: [],
      items: [],
      torches: [],
      isBossFloor: floor % 5 === 0,
      isShopFloor: floor % 3 === 0 && floor % 5 !== 0
    };

    // 初始化地图为墙壁
    for (let y = 0; y < height; y++) {
      dungeon.tiles[y] = [];
      for (let x = 0; x < width; x++) {
        dungeon.tiles[y][x] = this.TILE_TYPES.WALL;
      }
    }

    // 生成房间
    this.generateRooms(dungeon);

    // 连接房间
    this.generateCorridors(dungeon);

    // 放置特殊元素
    this.placeStairs(dungeon);
    this.placeChests(dungeon);
    this.placeTraps(dungeon);
    this.placeTorches(dungeon);

    if (dungeon.isShopFloor) {
      this.placeShop(dungeon);
    }

    if (dungeon.isBossFloor) {
      this.placeBossRoom(dungeon);
    }

    // 放置NPC
    this.placeNPCs(dungeon);

    // 生成怪物
    this.spawnMonsters(dungeon);

    // 生成掉落物
    this.spawnItems(dungeon);

    console.log(`[Dungeon] 第 ${floor} 层生成完成，房间数: ${dungeon.rooms.length}`);
    return dungeon;
  },

  // 生成房间
  generateRooms(dungeon) {
    const maxRooms = dungeon.isBossFloor ? 6 : 12;
    const minRoomSize = 5;
    const maxRoomSize = 12;
    let attempts = 0;

    while (dungeon.rooms.length < maxRooms && attempts < 100) {
      attempts++;

      const roomWidth = minRoomSize + Math.floor(Math.random() * (maxRoomSize - minRoomSize));
      const roomHeight = minRoomSize + Math.floor(Math.random() * (maxRoomSize - minRoomSize));
      const roomX = 1 + Math.floor(Math.random() * (dungeon.width - roomWidth - 2));
      const roomY = 1 + Math.floor(Math.random() * (dungeon.height - roomHeight - 2));

      const room = {
        x: roomX,
        y: roomY,
        width: roomWidth,
        height: roomHeight,
        centerX: Math.floor(roomX + roomWidth / 2),
        centerY: Math.floor(roomY + roomHeight / 2)
      };

      // 检查重叠
      let overlaps = false;
      for (const other of dungeon.rooms) {
        if (this.roomsOverlap(room, other, 2)) {
          overlaps = true;
          break;
        }
      }

      if (!overlaps) {
        this.carveRoom(dungeon, room);
        dungeon.rooms.push(room);
      }
    }

    // 按中心X排序
    dungeon.rooms.sort((a, b) => a.centerX - b.centerX);

    // 设置出生点
    if (dungeon.rooms.length > 0) {
      dungeon.spawnPoint = {
        x: dungeon.rooms[0].centerX * this.TILE_SIZE,
        y: dungeon.rooms[0].centerY * this.TILE_SIZE
      };
    }
  },

  // 检查房间重叠
  roomsOverlap(room1, room2, padding = 0) {
    return !(
      room1.x + room1.width + padding < room2.x ||
      room2.x + room2.width + padding < room1.x ||
      room1.y + room1.height + padding < room2.y ||
      room2.y + room2.height + padding < room1.y
    );
  },

  // 挖掘房间
  carveRoom(dungeon, room) {
    for (let y = room.y; y < room.y + room.height; y++) {
      for (let x = room.x; x < room.x + room.width; x++) {
        dungeon.tiles[y][x] = this.TILE_TYPES.FLOOR;
      }
    }
  },

  // 生成走廊
  generateCorridors(dungeon) {
    for (let i = 0; i < dungeon.rooms.length - 1; i++) {
      const room1 = dungeon.rooms[i];
      const room2 = dungeon.rooms[i + 1];

      // 随机选择先横后竖或先竖后横
      if (Math.random() < 0.5) {
        this.carveHorizontalCorridor(dungeon, room1.centerX, room2.centerX, room1.centerY);
        this.carveVerticalCorridor(dungeon, room1.centerY, room2.centerY, room2.centerX);
      } else {
        this.carveVerticalCorridor(dungeon, room1.centerY, room2.centerY, room1.centerX);
        this.carveHorizontalCorridor(dungeon, room1.centerX, room2.centerX, room2.centerY);
      }
    }
  },

  // 水平走廊
  carveHorizontalCorridor(dungeon, x1, x2, y) {
    const start = Math.min(x1, x2);
    const end = Math.max(x1, x2);
    for (let x = start; x <= end; x++) {
      if (dungeon.tiles[y] && dungeon.tiles[y][x] !== undefined) {
        dungeon.tiles[y][x] = this.TILE_TYPES.FLOOR;
        // 走廊宽度2格
        if (dungeon.tiles[y + 1] && dungeon.tiles[y + 1][x] !== undefined) {
          dungeon.tiles[y + 1][x] = this.TILE_TYPES.FLOOR;
        }
      }
    }
  },

  // 垂直走廊
  carveVerticalCorridor(dungeon, y1, y2, x) {
    const start = Math.min(y1, y2);
    const end = Math.max(y1, y2);
    for (let y = start; y <= end; y++) {
      if (dungeon.tiles[y] && dungeon.tiles[y][x] !== undefined) {
        dungeon.tiles[y][x] = this.TILE_TYPES.FLOOR;
        if (dungeon.tiles[y][x + 1] !== undefined) {
          dungeon.tiles[y][x + 1] = this.TILE_TYPES.FLOOR;
        }
      }
    }
  },

  // 放置楼梯
  placeStairs(dungeon) {
    if (dungeon.rooms.length < 2) return;

    // 下楼楼梯在最后一个房间
    const lastRoom = dungeon.rooms[dungeon.rooms.length - 1];
    dungeon.stairsDown = {
      x: lastRoom.centerX,
      y: lastRoom.centerY
    };
    dungeon.tiles[lastRoom.centerY][lastRoom.centerX] = this.TILE_TYPES.STAIRS_DOWN;

    // 上楼楼梯在第一个房间
    const firstRoom = dungeon.rooms[0];
    dungeon.stairsUp = {
      x: firstRoom.centerX + 2,
      y: firstRoom.centerY
    };
    if (dungeon.floor > 1) {
      dungeon.tiles[firstRoom.centerY][firstRoom.centerX + 2] = this.TILE_TYPES.STAIRS_UP;
    }
  },

  // 放置宝箱
  placeChests(dungeon) {
    const chestCount = dungeon.isBossFloor ? 2 : 3 + Math.floor(Math.random() * 3);

    for (let i = 0; i < chestCount; i++) {
      const room = dungeon.rooms[Math.floor(Math.random() * dungeon.rooms.length)];
      if (!room) continue;

      const chestX = room.x + 1 + Math.floor(Math.random() * (room.width - 2));
      const chestY = room.y + 1 + Math.floor(Math.random() * (room.height - 2));

      if (dungeon.tiles[chestY][chestX] === this.TILE_TYPES.FLOOR) {
        dungeon.chests.push({
          x: chestX * this.TILE_SIZE,
          y: chestY * this.TILE_SIZE,
          opened: false,
          locked: Math.random() < 0.3,
          tier: dungeon.floor
        });
      }
    }
  },

  // 放置陷阱
  placeTraps(dungeon) {
    if (dungeon.floor < 2) return;

    const trapCount = Math.min(dungeon.floor, 5);
    const trapTypes = ['spike', 'poison', 'fire', 'ice'];

    for (let i = 0; i < trapCount; i++) {
      // 陷阱放在走廊上
      let attempts = 0;
      while (attempts < 20) {
        attempts++;
        const x = Math.floor(Math.random() * dungeon.width);
        const y = Math.floor(Math.random() * dungeon.height);

        if (dungeon.tiles[y][x] === this.TILE_TYPES.FLOOR) {
          // 不在房间中心
          let inRoomCenter = false;
          for (const room of dungeon.rooms) {
            if (Math.abs(x - room.centerX) < 2 && Math.abs(y - room.centerY) < 2) {
              inRoomCenter = true;
              break;
            }
          }
          if (!inRoomCenter) {
            dungeon.traps.push({
              x: x * this.TILE_SIZE,
              y: y * this.TILE_SIZE,
              type: trapTypes[Math.floor(Math.random() * trapTypes.length)],
              triggered: false,
              damage: 10 + dungeon.floor * 2
            });
            break;
          }
        }
      }
    }
  },

  // 放置火把
  placeTorches(dungeon) {
    for (const room of dungeon.rooms) {
      // 房间四角放火把
      const torchPositions = [
        { x: room.x + 1, y: room.y + 1 },
        { x: room.x + room.width - 2, y: room.y + 1 },
        { x: room.x + 1, y: room.y + room.height - 2 },
        { x: room.x + room.width - 2, y: room.y + room.height - 2 }
      ];

      for (const pos of torchPositions) {
        if (Math.random() < 0.6) {
          dungeon.torches.push({
            x: pos.x * this.TILE_SIZE,
            y: pos.y * this.TILE_SIZE,
            flickerOffset: Math.random() * Math.PI * 2
          });
        }
      }
    }
  },

  // 放置商店
  placeShop(dungeon) {
    if (dungeon.rooms.length < 3) return;

    const shopRoom = dungeon.rooms[Math.floor(dungeon.rooms.length / 2)];
    dungeon.npcs.push({
      x: shopRoom.centerX * this.TILE_SIZE,
      y: shopRoom.centerY * this.TILE_SIZE,
      type: 'merchant',
      name: '地牢商人'
    });
  },

  // 放置BOSS房间
  placeBossRoom(dungeon) {
    // BOSS在最后一个房间
    const bossRoom = dungeon.rooms[dungeon.rooms.length - 1];
    // 放大BOSS房间
    this.carveRoom(dungeon, {
      x: bossRoom.x - 2,
      y: bossRoom.y - 2,
      width: bossRoom.width + 4,
      height: bossRoom.height + 4
    });
  },

  // 放置NPC
  placeNPCs(dungeon) {
    // 随机放置一些NPC
    const npcTypes = ['mysterious_merchant', 'ghost_adventurer'];
    if (Math.random() < 0.3 && dungeon.rooms.length > 2) {
      const room = dungeon.rooms[1 + Math.floor(Math.random() * (dungeon.rooms.length - 2))];
      dungeon.npcs.push({
        x: room.centerX * this.TILE_SIZE,
        y: room.centerY * this.TILE_SIZE,
        type: npcTypes[Math.floor(Math.random() * npcTypes.length)]
      });
    }
  },

  // 生成怪物
  spawnMonsters(dungeon) {
    const monsterCount = dungeon.isBossFloor ? 3 : 5 + Math.floor(Math.random() * 5) + Math.floor(dungeon.floor / 2);

    for (let i = 0; i < monsterCount; i++) {
      const room = dungeon.rooms[1 + Math.floor(Math.random() * (dungeon.rooms.length - 1))];
      if (!room) continue;

      const monsterX = (room.x + 1 + Math.floor(Math.random() * (room.width - 2))) * this.TILE_SIZE;
      const monsterY = (room.y + 1 + Math.floor(Math.random() * (room.height - 2))) * this.TILE_SIZE;

      // 根据楼层选择怪物
      const monsterId = MonsterData.getRandomMonsterForFloor(dungeon.floor);
      dungeon.monsters.push({
        x: monsterX,
        y: monsterY,
        monsterId,
        floor: dungeon.floor
      });
    }

    // BOSS
    if (dungeon.isBossFloor) {
      const bossRoom = dungeon.rooms[dungeon.rooms.length - 1];
      const bossId = MonsterData.getBossForFloor(dungeon.floor);
      if (bossId) {
        dungeon.monsters.push({
          x: bossRoom.centerX * this.TILE_SIZE,
          y: bossRoom.centerY * this.TILE_SIZE,
          monsterId: bossId,
          floor: dungeon.floor,
          isBoss: true
        });
      }
    }
  },

  // 生成掉落物
  spawnItems(dungeon) {
    const itemCount = 2 + Math.floor(Math.random() * 3);
    const consumables = ['healthPotion', 'manaPotion', 'staminaPotion', 'bomb', 'antidote'];

    for (let i = 0; i < itemCount; i++) {
      const room = dungeon.rooms[Math.floor(Math.random() * dungeon.rooms.length)];
      if (!room) continue;

      const itemX = (room.x + 1 + Math.floor(Math.random() * (room.width - 2))) * this.TILE_SIZE;
      const itemY = (room.y + 1 + Math.floor(Math.random() * (room.height - 2))) * this.TILE_SIZE;

      dungeon.items.push({
        x: itemX,
        y: itemY,
        itemId: consumables[Math.floor(Math.random() * consumables.length)],
        quantity: 1
      });
    }
  },

  // 检查是否是墙壁
  isWall(dungeon, tileX, tileY) {
    if (tileX < 0 || tileX >= dungeon.width || tileY < 0 || tileY >= dungeon.height) {
      return true;
    }
    return dungeon.tiles[tileY][tileX] === this.TILE_TYPES.WALL;
  },

  // 获取瓦片
  getTile(dungeon, tileX, tileY) {
    if (tileX < 0 || tileX >= dungeon.width || tileY < 0 || tileY >= dungeon.height) {
      return this.TILE_TYPES.WALL;
    }
    return dungeon.tiles[tileY][tileX];
  },

  // 世界坐标转瓦片坐标
  worldToTile(worldX, worldY) {
    return {
      x: Math.floor(worldX / this.TILE_SIZE),
      y: Math.floor(worldY / this.TILE_SIZE)
    };
  },

  // 瓦片坐标转世界坐标
  tileToWorld(tileX, tileY) {
    return {
      x: tileX * this.TILE_SIZE + this.TILE_SIZE / 2,
      y: tileY * this.TILE_SIZE + this.TILE_SIZE / 2
    };
  }
};

window.DungeonGenerator = DungeonGenerator;
