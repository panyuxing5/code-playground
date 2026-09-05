// ==================== 永恒地牢 - 游戏主循环 ====================
// 游戏核心：初始化、主循环、状态管理、场景切换

class Game {
  constructor() {
    this.canvas = document.getElementById('gameCanvas');
    this.ctx = this.canvas.getContext('2d');
    this.canvas.width = 1280;
    this.canvas.height = 800;

    // 游戏状态
    this.state = 'menu'; // menu, playing, paused, gameover, victory, shop, inventory, dialogue
    this.previousState = null;

    // 游戏数据
    this.player = null;
    this.monsters = [];
    this.projectiles = [];
    this.items = [];
    this.particles = [];
    this.npcs = [];
    this.chests = [];
    this.traps = [];
    this.torches = [];
    this.dungeon = null;
    this.currentFloor = 1;
    this.maxFloor = 30;

    // 相机
    this.camera = { x: 0, y: 0 };

    // 地图尺寸
    this.mapWidth = 0;
    this.mapHeight = 0;

    // 消息
    this.messages = [];
    this.maxMessages = 50;

    // 时间
    this.gameTime = 0;
    this.lastTime = 0;
    this.deltaTime = 0;
    this.fps = 0;
    this.fpsCounter = 0;
    this.fpsTimer = 0;

    // 设置
    this.settings = {
      volume: 0.5,
      musicVolume: 0.3,
      sfxVolume: 0.7,
      difficulty: 'normal',
      showDamageNumbers: true,
      screenShake: true
    };

    // UI
    this.showInventory = false;
    this.showCharacter = false;
    this.showTalents = false;
    this.showQuests = false;
    this.showMap = false;
    this.showCrafting = false;
    this.showShop = false;
    this.showEnchant = false;
    this.showArena = false;
    this.showTitles = false;
    this.showGacha = false;
    this.showPetCollection = false;
    this.dialogueData = null;
    this.shopData = null;

    // 输入
    this.mouseX = 0;
    this.mouseY = 0;

    // 初始化
    this.init();
  }

  // 初始化
  init() {
    console.log('[Game] 永恒地牢初始化中...');

    // 初始化系统（带错误保护，单个系统失败不影响整体）
    const systems = [
      ['ParticleSystem', ParticleSystem],
      ['AchievementSystem', AchievementSystem],
      ['QuestSystem', QuestSystem],
      ['AudioSystem', AudioSystem],
      ['PetManager', PetManager],
      ['CraftingManager', CraftingManager],
      ['ShopManager', ShopManager],
      ['EnchantManager', EnchantManager],
      ['ArenaManager', ArenaManager],
      ['TitleManager', TitleManager]
    ];

    for (const [name, sys] of systems) {
      try {
        if (sys && typeof sys.init === 'function') {
          sys.init();
        }
      } catch (e) {
        console.error(`[Game] ${name} 初始化失败:`, e);
      }
    }

    // 设置事件监听
    try {
      this.setupEventListeners();
    } catch (e) {
      console.error('[Game] 事件监听设置失败:', e);
    }

    // 显示主菜单
    try {
      this.showMainMenu();
    } catch (e) {
      console.error('[Game] 显示主菜单失败:', e);
      this.state = 'menu';
    }

    console.log('[Game] 初始化完成！');
  }

  // 设置事件监听
  setupEventListeners() {
    // 键盘
    window.addEventListener('keydown', (e) => {
      InputManager.onKeyDown(e);
      this.handleKeyPress(e);
    });
    window.addEventListener('keyup', (e) => {
      InputManager.onKeyUp(e);
    });

    // 鼠标
    this.canvas.addEventListener('mousemove', (e) => {
      const rect = this.canvas.getBoundingClientRect();
      const scaleX = this.canvas.width / rect.width;
      const scaleY = this.canvas.height / rect.height;
      this.mouseX = (e.clientX - rect.left) * scaleX;
      this.mouseY = (e.clientY - rect.top) * scaleY;
    });

    this.canvas.addEventListener('mousedown', (e) => {
      if (e.button === 0) {
        // 重新计算鼠标坐标（考虑Canvas缩放）
        const rect = this.canvas.getBoundingClientRect();
        const scaleX = this.canvas.width / rect.width;
        const scaleY = this.canvas.height / rect.height;
        this.mouseX = (e.clientX - rect.left) * scaleX;
        this.mouseY = (e.clientY - rect.top) * scaleY;

        InputManager.setMouseDown(true);
        if (this.state === 'playing') {
          this.player?.performAttack(this);
        } else if (this.state === 'menu') {
          this.handleMenuClick();
        } else if (this.state === 'classSelect') {
          this.handleClassSelectClick();
        } else if (this.state === 'gameover' || this.state === 'victory') {
          this.showMainMenu();
        } else if (this.state === 'dialogue') {
          this.handleDialogueClick();
        } else if (this.state === 'shop') {
          ShopManager.handleClick(this.mouseX, this.mouseY, this);
        } else if (this.state === 'arena') {
          ArenaManager.handleClick(this.mouseX, this.mouseY, this);
        } else if (this.state === 'titles') {
          TitleManager.handleClick(this.mouseX, this.mouseY, this);
        } else if (this.state === 'gacha') {
          PetManager.handleGachaClick(this.mouseX, this.mouseY, this);
        } else if (this.state === 'story') {
          if (StorySystem) {
            StorySystem.handleClick(this);
          }
        }
      }
    });

    this.canvas.addEventListener('mouseup', (e) => {
      if (e.button === 0) {
        InputManager.setMouseDown(false);
      }
    });

    // 防止右键菜单
    this.canvas.addEventListener('contextmenu', (e) => e.preventDefault());

    // 窗口大小
    window.addEventListener('resize', () => {
      // 保持固定分辨率
    });
  }

  // 处理菜单点击
  handleMenuClick() {
    const options = [
      { text: '开始游戏', action: 'start', y: 300 },
      { text: '选择职业', action: 'class', y: 360 },
      { text: '继续游戏', action: 'continue', y: 420 },
      { text: '设置', action: 'settings', y: 480 },
      { text: '退出', action: 'exit', y: 540 }
    ];

    for (const opt of options) {
      if (this.mouseY > opt.y - 25 && this.mouseY < opt.y + 25 &&
          this.mouseX > this.canvas.width / 2 - 150 && this.mouseX < this.canvas.width / 2 + 150) {
        AudioSystem.playSound('click');
        try {
          if (opt.action === 'start') {
            console.log('[Menu] 开始游戏');
            this.startNewGame('warrior');
          } else if (opt.action === 'class') {
            console.log('[Menu] 选择职业');
            this.showClassSelect();
          } else if (opt.action === 'continue') {
            if (SaveSystem.hasSave(0)) {
              SaveSystem.loadGame(0);
              this.state = 'playing';
            } else {
              this.showMessage('没有存档！请先开始新游戏');
            }
          } else if (opt.action === 'settings') {
            this.showMessage('设置功能开发中...');
          } else if (opt.action === 'exit') {
            if (confirm('确定要退出游戏吗？')) {
              window.close();
            }
          }
        } catch (e) {
          console.error('[Menu] 操作出错:', e);
          this.showMessage('出错了: ' + e.message);
        }
        return;
      }
    }
  }

  // 显示职业选择
  showClassSelect() {
    this.state = 'classSelect';
    this.selectedClass = 'warrior';
  }

  // 处理职业选择点击
  handleClassSelectClick() {
    const classes = ['warrior', 'mage', 'ranger', 'rogue', 'paladin', 'necromancer'];

    for (let i = 0; i < 6; i++) {
      const x = 150 + (i % 3) * 330;
      const y = 150 + Math.floor(i / 3) * 250;
      if (this.mouseX > x && this.mouseX < x + 280 && this.mouseY > y && this.mouseY < y + 200) {
        this.selectedClass = classes[i];
        AudioSystem.playSound('click');
        return;
      }
    }

    // 开始按钮
    if (this.mouseX > this.canvas.width / 2 - 100 && this.mouseX < this.canvas.width / 2 + 100 &&
        this.mouseY > this.canvas.height - 80 && this.mouseY < this.canvas.height - 30) {
      if (this.selectedClass) {
        AudioSystem.playSound('click');
        this.startNewGame(this.selectedClass);
      }
    }
  }

  // 处理对话点击
  handleDialogueClick() {
    if (!this.dialogueData) return;
    const currentLine = this.dialogueData.currentDialogue[this.dialogueData.currentIndex];
    if (!currentLine || !currentLine.options) {
      this.state = 'playing';
      this.dialogueData = null;
      return;
    }

    // 检查点击的选项
    for (let i = 0; i < currentLine.options.length; i++) {
      const optY = this.canvas.height - 90 + i * 25;
      if (this.mouseY > optY - 12 && this.mouseY < optY + 12) {
        const opt = currentLine.options[i];
        if (opt.action) {
          this.handleDialogueAction(opt.action);
        }
        if (opt.next) {
          const nextDialogue = DialogueData.getDialogue(this.dialogueData.npcId, opt.next);
          if (nextDialogue) {
            this.dialogueData.currentDialogue = nextDialogue;
            this.dialogueData.currentIndex = 0;
          } else {
            this.state = 'playing';
            this.dialogueData = null;
          }
        } else {
          this.state = 'playing';
          this.dialogueData = null;
        }
        AudioSystem.playSound('click');
        return;
      }
    }
  }

  // 处理对话动作
  handleDialogueAction(action) {
    const parts = action.split(':');
    const type = parts[0];
    const value = parts[1];

    switch (type) {
      case 'start_quest':
        QuestSystem.acceptQuest(value);
        break;
      case 'open_shop':
        this.showMessage('商店功能开发中...');
        break;
      case 'full_heal':
        if (this.player) {
          this.player.hp = this.player.maxHp;
          this.player.mp = this.player.maxMp;
          this.showMessage('已完全恢复！');
        }
        break;
      case 'buff':
        if (this.player) {
          BuffSystem.applyBuff(this.player, 'bless', null, 300);
        }
        break;
    }
  }

  // 处理按键
  handleKeyPress(e) {
    if (this.state === 'playing') {
      switch (e.key.toLowerCase()) {
        case 'i':
          this.showInventory = !this.showInventory;
          this.state = this.showInventory ? 'inventory' : 'playing';
          break;
        case 'c':
          this.showCharacter = !this.showCharacter;
          this.state = this.showCharacter ? 'character' : 'playing';
          break;
        case 'q':
          this.showQuests = !this.showQuests;
          this.state = this.showQuests ? 'quests' : 'playing';
          break;
        case 'm':
          this.showMap = !this.showMap;
          this.state = this.showMap ? 'map' : 'playing';
          break;
        case 'f':
          this.showCrafting = !this.showCrafting;
          this.state = this.showCrafting ? 'crafting' : 'playing';
          break;
        case 'g':
          this.showShop = !this.showShop;
          if (this.showShop) {
            ShopManager.openShop('general', this.player.level);
            this.state = 'shop';
          } else {
            ShopManager.closeShop();
            this.state = 'playing';
          }
          break;
        case 'h':
          this.showEnchant = !this.showEnchant;
          this.state = this.showEnchant ? 'enchant' : 'playing';
          break;
        case 'j':
          this.showArena = !this.showArena;
          this.state = this.showArena ? 'arena' : 'playing';
          break;
        case 'k':
          this.showTitles = !this.showTitles;
          this.state = this.showTitles ? 'titles' : 'playing';
          break;
        case 'l':
          this.showGacha = !this.showGacha;
          this.state = this.showGacha ? 'gacha' : 'playing';
          break;
        case 'p':
          this.showPetCollection = !this.showPetCollection;
          this.state = this.showPetCollection ? 'petCollection' : 'playing';
          break;
        case 'v':
          // 切换坐骑
          PetManager.toggleMount();
          this.showMessage(PetManager.isMounted ? '骑乘坐骑！' : '下坐骑');
          break;
        case 'escape':
          this.state = 'paused';
          break;
        case 'e':
          this.interact(this.player);
          break;
      }
    } else if (this.state === 'paused') {
      if (e.key === 'Escape') {
        this.state = 'playing';
      }
    } else if (this.state === 'inventory' || this.state === 'character' || this.state === 'quests' || this.state === 'map' || this.state === 'crafting' || this.state === 'shop' || this.state === 'enchant' || this.state === 'arena' || this.state === 'titles' || this.state === 'gacha' || this.state === 'petCollection') {
      if (e.key === 'Escape' || e.key.toLowerCase() === 'i' || e.key.toLowerCase() === 'c' || e.key.toLowerCase() === 'q' || e.key.toLowerCase() === 'm' || e.key.toLowerCase() === 'f' || e.key.toLowerCase() === 'g' || e.key.toLowerCase() === 'h' || e.key.toLowerCase() === 'j' || e.key.toLowerCase() === 'k' || e.key.toLowerCase() === 'l' || e.key.toLowerCase() === 'p') {
        this.showInventory = false;
        this.showCharacter = false;
        this.showQuests = false;
        this.showMap = false;
        this.showCrafting = false;
        this.showShop = false;
        this.showEnchant = false;
        this.showArena = false;
        this.showTitles = false;
        this.showGacha = false;
        this.showPetCollection = false;
        ShopManager.closeShop();
        this.state = 'playing';
      }
    }
  }

  // 显示主菜单
  showMainMenu() {
    this.state = 'menu';
    try {
      AudioSystem.playMusic('title');
    } catch (e) {
      console.error('[Game] 播放菜单音乐失败:', e);
    }
  }

  // 开始新游戏
  startNewGame(classType = 'warrior') {
    console.log(`[Game] 开始新游戏，职业: ${classType}`);

    try {
      // 初始化玩家
      this.player = new Player(0, 0, classType);
      TalentSystem.init(classType);

      // 重置数据
      this.monsters = [];
      this.projectiles = [];
      this.items = [];
      this.currentFloor = 1;
      this.gameTime = 0;
      this.messages = [];

      // 重置系统（带错误保护）
      try { PetManager.init(); } catch(e) { console.error('PetManager init error:', e); }
      try { CraftingManager.init(); } catch(e) { console.error('CraftingManager init error:', e); }
      try { ShopManager.init(); } catch(e) { console.error('ShopManager init error:', e); }
      try { EnchantManager.init(); } catch(e) { console.error('EnchantManager init error:', e); }
      try { ArenaManager.init(); } catch(e) { console.error('ArenaManager init error:', e); }
      try { TitleManager.init(); } catch(e) { console.error('TitleManager init error:', e); }
      try { StorySystem.init(); } catch(e) { console.error('StorySystem init error:', e); }

      // 初始化击败BOSS记录
      this.defeatedBosses = [];

      // 启动自动保存
      if (SaveSystem) {
        SaveSystem.startAutoSave();
      }

      // 接受初始任务
      QuestSystem.acceptQuest('main_enter_dungeon');

      // 生成第一层
      this.loadFloor(1);

      // 切换状态
      this.state = 'playing';
      AudioSystem.playMusic('dungeon');

      this.showMessage('欢迎来到永恒地牢！');
      this.showMessage('使用 WASD 移动，鼠标左键攻击，1-5 释放技能。');
      this.showMessage('F合成 G商店 H强化 J竞技场 K称号 L抽卡 P图鉴 V坐骑');
      console.log('[Game] 新游戏启动成功');
    } catch (e) {
      console.error('[Game] 启动游戏出错:', e);
      this.showMessage('启动失败: ' + e.message);
      this.state = 'menu';
    }
  }

  // 加载楼层
  loadFloor(floor) {
    console.log(`[Game] 加载第 ${floor} 层...`);

    this.currentFloor = floor;
    this.dungeon = DungeonGenerator.generate(floor);
    this.mapWidth = this.dungeon.width * DungeonGenerator.TILE_SIZE;
    this.mapHeight = this.dungeon.height * DungeonGenerator.TILE_SIZE;

    // 重置实体
    this.monsters = [];
    this.projectiles = [];
    this.items = [];
    this.chests = [...this.dungeon.chests];
    this.traps = [...this.dungeon.traps];
    this.torches = [...this.dungeon.torches];
    this.npcs = [...this.dungeon.npcs];

    // 生成怪物
    for (const m of this.dungeon.monsters) {
      const monster = new Monster(m.x, m.y, m.monsterId, floor);
      if (m.isBoss) {
        monster.isBoss = true;
        if (BossSystem) {
          BossSystem.initBoss(monster);
        }
      }
      this.monsters.push(monster);
    }

    // 生成物品
    for (const item of this.dungeon.items) {
      this.items.push({
        x: item.x,
        y: item.y,
        itemId: item.itemId,
        quantity: item.quantity,
        bobOffset: Math.random() * Math.PI * 2
      });
    }

    // 设置玩家位置
    if (this.player) {
      this.player.x = this.dungeon.spawnPoint.x;
      this.player.y = this.dungeon.spawnPoint.y;
    }

    // 任务进度
    QuestSystem.updateProgress('enter_floor', null, floor);
    QuestSystem.updateProgress('reach_floor', null, floor);
    AchievementSystem.recordFloor(floor);

    // BOSS音乐
    if (this.dungeon.isBossFloor) {
      AudioSystem.playMusic('boss');
      this.showMessage('⚠ 你感受到了强大的气息...BOSS就在附近！');
    } else {
      AudioSystem.playMusic('dungeon');
    }

    this.showMessage(`进入第 ${floor} 层`);
  }

  // 下一层
  nextFloor() {
    if (this.currentFloor >= this.maxFloor) {
      this.victory();
      return;
    }
    this.loadFloor(this.currentFloor + 1);
    SaveSystem.autoSave();
  }

  // 上一层
  prevFloor() {
    if (this.currentFloor <= 1) {
      this.showMessage('已经是第一层了！');
      return;
    }
    this.loadFloor(this.currentFloor - 1);
  }

  // 游戏主循环
  gameLoop(timestamp) {
    try {
      // 计算deltaTime
      if (this.lastTime === 0) this.lastTime = timestamp;
      this.deltaTime = Math.min((timestamp - this.lastTime) / 1000, 0.1);
      this.lastTime = timestamp;

      // FPS计算
      this.fpsCounter++;
      this.fpsTimer += this.deltaTime;
      if (this.fpsTimer >= 1) {
        this.fps = this.fpsCounter;
        this.fpsCounter = 0;
        this.fpsTimer = 0;
      }

      // 剧情触发检查
      if (this.state === 'playing' && StorySystem) {
        try { StorySystem.checkTriggers(this); } catch(e) {}
      }

      // 更新
      this.update(this.deltaTime);

      // 渲染
      this.render();

      // 清除单帧输入状态（必须在帧末调用）
      if (window.InputManager) {
        InputManager.endFrame();
      }
    } catch (e) {
      console.error('[Game] 游戏循环出错:', e);
      // 出错时尝试显示错误信息
      try {
        this.showMessage('游戏出错: ' + e.message);
      } catch(e2) {}
    }

    // 继续循环（即使出错也继续，避免卡死）
    requestAnimationFrame((t) => this.gameLoop(t));
  }

  // 更新
  update(dt) {
    if (this.state !== 'playing') return;

    this.gameTime += dt;
    AchievementSystem.stats.totalPlayTime += dt;

    // 更新玩家
    if (this.player && !this.player.dead) {
      this.player.update(dt, this);
    }

    // 更新怪物
    for (const monster of this.monsters) {
      if (!monster.dead) {
        monster.update(dt, this);
        // BOSS系统更新
        if (monster.isBoss && BossSystem) {
          BossSystem.updateBoss(monster, this, dt);
        }
      }
    }
    // 清理死亡怪物
    this.monsters = this.monsters.filter(m => !m.dead || m.deathTimer > 0);

    // 更新投射物
    this.updateProjectiles(dt);

    // 更新粒子
    ParticleSystem.update(dt);

    // 更新相机
    this.updateCamera();

    // 检查楼梯
    this.checkStairs();

    // 检查陷阱
    this.checkTraps();

    // 检查物品拾取
    this.checkItemPickup();

    // 更新宠物
    PetManager.update(dt, this);

    // 更新竞技场
    ArenaManager.update(dt, this);

    // 检查称号解锁
    if (Math.floor(this.gameTime) % 5 === 0) {
      TitleManager.checkTitles(this);
    }

    // 检查宝箱
    // 交互时处理

    // 环境粒子
    if (Math.random() < 0.02) {
      for (const torch of this.torches) {
        ParticleSystem.ambient(torch.x, torch.y, 'torch');
      }
    }

    // 自动保存（每30秒）
    if (Math.floor(this.gameTime) % 30 === 0 && Math.floor(this.gameTime) !== this.lastAutoSave) {
      this.lastAutoSave = Math.floor(this.gameTime);
      // SaveSystem.autoSave();
    }
  }

  // 更新投射物
  updateProjectiles(dt) {
    for (let i = this.projectiles.length - 1; i >= 0; i--) {
      const proj = this.projectiles[i];
      proj.x += proj.vx * dt * 60;
      proj.y += proj.vy * dt * 60;
      proj.life -= dt;

      // 检查碰撞
      if (proj.owner === this.player) {
        for (const monster of this.monsters) {
          if (monster.dead) continue;
          const dist = Utils.distance(proj.x, proj.y, monster.x, monster.y);
          if (dist < monster.radius + 5) {
            monster.takeDamage(proj.damage, proj.owner);
            ParticleSystem.damageNumber(monster.x, monster.y, proj.damage);
            ParticleSystem.explosion(proj.x, proj.y, proj.color || '#e67e22', 30, 8);
            this.projectiles.splice(i, 1);
            break;
          }
        }
      } else {
        // 敌人投射物打玩家
        if (this.player && !this.player.dead) {
          const dist = Utils.distance(proj.x, proj.y, this.player.x, this.player.y);
          if (dist < this.player.radius + 5) {
            this.player.takeDamage(proj.damage, proj.owner);
            this.projectiles.splice(i, 1);
            continue;
          }
        }
      }

      // 墙壁碰撞
      const tile = DungeonGenerator.worldToTile(proj.x, proj.y);
      if (DungeonGenerator.isWall(this.dungeon, tile.x, tile.y)) {
        ParticleSystem.explosion(proj.x, proj.y, proj.color || '#888', 20, 5);
        this.projectiles.splice(i, 1);
        continue;
      }

      // 寿命结束
      if (proj.life <= 0) {
        this.projectiles.splice(i, 1);
      }
    }
  }

  // 创建投射物
  createProjectile(data) {
    const dx = data.targetX - data.x;
    const dy = data.targetY - data.y;
    const dist = Math.sqrt(dx * dx + dy * dy);
    this.projectiles.push({
      x: data.x,
      y: data.y,
      vx: (dx / dist) * data.speed,
      vy: (dy / dist) * data.speed,
      damage: data.damage,
      owner: data.owner,
      skill: data.skill,
      color: data.skill?.animation?.color || '#fff',
      life: 3
    });
  }

  // 更新相机
  updateCamera() {
    if (!this.player) return;

    const targetX = this.player.x - this.canvas.width / 2;
    const targetY = this.player.y - this.canvas.height / 2;

    // 平滑跟随
    this.camera.x += (targetX - this.camera.x) * 0.1;
    this.camera.y += (targetY - this.camera.y) * 0.1;

    // 边界限制
    this.camera.x = Utils.clamp(this.camera.x, 0, this.mapWidth - this.canvas.width);
    this.camera.y = Utils.clamp(this.camera.y, 0, this.mapHeight - this.canvas.height);
  }

  // 检查楼梯
  checkStairs() {
    if (!this.player || !this.dungeon) return;

    const tile = DungeonGenerator.worldToTile(this.player.x, this.player.y);
    const tileType = DungeonGenerator.getTile(this.dungeon, tile.x, tile.y);

    if (tileType === DungeonGenerator.TILE_TYPES.STAIRS_DOWN) {
      if (InputManager.isActionPressed('interact')) {
        this.nextFloor();
      }
    } else if (tileType === DungeonGenerator.TILE_TYPES.STAIRS_UP) {
      if (InputManager.isActionPressed('interact')) {
        this.prevFloor();
      }
    }
  }

  // 检查陷阱
  checkTraps() {
    if (!this.player) return;

    for (const trap of this.traps) {
      if (trap.triggered) continue;
      const dist = Utils.distance(this.player.x, this.player.y, trap.x, trap.y);
      if (dist < 25) {
        trap.triggered = true;
        this.player.takeDamage(trap.damage, null);
        this.showMessage(`触发了${trap.type}陷阱！`);

        if (trap.type === 'poison') {
          BuffSystem.applyBuff(this.player, 'poison', null, 5);
        } else if (trap.type === 'fire') {
          BuffSystem.applyBuff(this.player, 'burn', null, 3);
        } else if (trap.type === 'ice') {
          BuffSystem.applyBuff(this.player, 'freeze', null, 3);
        }

        ParticleSystem.explosion(trap.x, trap.y, '#e74c3c', 40, 10);
      }
    }
  }

  // 检查物品拾取
  checkItemPickup() {
    if (!this.player) return;

    for (let i = this.items.length - 1; i >= 0; i--) {
      const item = this.items[i];
      const dist = Utils.distance(this.player.x, this.player.y, item.x, item.y);
      if (dist < 30) {
        const result = this.player.addToInventory(item.itemId, item.quantity);
        if (result.success) {
          const itemData = ItemData.getItem(item.itemId);
          this.showMessage(`拾取了 ${itemData.name} x${item.quantity}`);
          ParticleSystem.itemPickup(item.x, item.y);
          AudioSystem.playSound('pickup');
          this.items.splice(i, 1);
          QuestSystem.updateProgress('collect', item.itemId, item.quantity);
        }
      }
    }
  }

  // 交互
  interact(player) {
    if (!player) return;

    // 检查NPC
    for (const npc of this.npcs) {
      const dist = Utils.distance(player.x, player.y, npc.x, npc.y);
      if (dist < 50) {
        this.startDialogue(npc.type);
        return;
      }
    }

    // 检查宝箱
    for (const chest of this.chests) {
      if (chest.opened) continue;
      const dist = Utils.distance(player.x, player.y, chest.x, chest.y);
      if (dist < 40) {
        this.openChest(chest);
        return;
      }
    }

    // 检查楼梯提示
    const tile = DungeonGenerator.worldToTile(player.x, player.y);
    const tileType = DungeonGenerator.getTile(this.dungeon, tile.x, tile.y);
    if (tileType === DungeonGenerator.TILE_TYPES.STAIRS_DOWN) {
      this.showMessage('按 E 下楼');
    } else if (tileType === DungeonGenerator.TILE_TYPES.STAIRS_UP) {
      this.showMessage('按 E 上楼');
    }
  }

  // 打开宝箱
  openChest(chest) {
    chest.opened = true;
    AudioSystem.playSound('chest');
    ParticleSystem.itemPickup(chest.x, chest.y, '#f1c40f');

    // 生成战利品
    const goldAmount = 20 + chest.tier * 10 + Math.floor(Math.random() * 30);
    this.player.gold += goldAmount;
    this.showMessage(`打开宝箱！获得 ${goldAmount} 金币`);

    // 随机物品
    const itemCount = 1 + Math.floor(Math.random() * 2);
    for (let i = 0; i < itemCount; i++) {
      const itemId = ItemData.getRandomEquipment(chest.tier);
      this.items.push({
        x: chest.x + (Math.random() - 0.5) * 40,
        y: chest.y + (Math.random() - 0.5) * 40,
        itemId,
        quantity: 1,
        bobOffset: Math.random() * Math.PI * 2
      });
    }

    QuestSystem.updateProgress('open_chest');
  }

  // 开始对话
  startDialogue(npcId) {
    const dialogue = DialogueData.getDialogue(npcId);
    const npcInfo = DialogueData.getNPCInfo(npcId);
    if (dialogue && npcInfo) {
      this.dialogueData = {
        npcId,
        npcName: npcInfo.name,
        npcIcon: npcInfo.icon,
        currentDialogue: dialogue,
        currentIndex: 0
      };
      this.state = 'dialogue';
      AudioSystem.playSound('click');
    }
  }

  // 掉落物品
  dropItem(itemId, x, y, quantity = 1) {
    this.items.push({
      x, y,
      itemId,
      quantity,
      bobOffset: Math.random() * Math.PI * 2
    });
  }

  // 碰撞检测
  isColliding(x, y, radius) {
    if (!this.dungeon) return false;

    // 检查四个角
    const corners = [
      { x: x - radius, y: y - radius },
      { x: x + radius, y: y - radius },
      { x: x - radius, y: y + radius },
      { x: x + radius, y: y + radius }
    ];

    for (const corner of corners) {
      const tile = DungeonGenerator.worldToTile(corner.x, corner.y);
      if (DungeonGenerator.isWall(this.dungeon, tile.x, tile.y)) {
        return true;
      }
    }
    return false;
  }

  // 玩家死亡
  onPlayerDeath() {
    this.state = 'gameover';
    AudioSystem.playMusic('menu');
    this.showMessage('你死了...');
  }

  // 胜利
  victory() {
    this.state = 'victory';
    AchievementSystem.unlockAchievement('achievement_legend');
    AudioSystem.playSound('victory');
  }

  // 显示消息
  showMessage(text, color = '#fff') {
    this.messages.push({ text, color, time: Date.now() });
    if (this.messages.length > this.maxMessages) {
      this.messages.shift();
    }
  }

  // 传送回城镇
  teleportToTown() {
    this.showMessage('传送回城镇...');
    // 简化处理：回到第一层
    this.loadFloor(1);
  }

  // 渲染
  render() {
    const ctx = this.ctx;

    // 清屏
    ctx.fillStyle = '#0a0a1a';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    switch (this.state) {
      case 'menu':
        this.renderMenu(ctx);
        break;
      case 'classSelect':
        this.renderClassSelect(ctx);
        break;
      case 'playing':
      case 'paused':
      case 'inventory':
      case 'character':
      case 'quests':
      case 'map':
      case 'dialogue':
      case 'shop':
      case 'crafting':
      case 'enchant':
      case 'arena':
      case 'titles':
      case 'gacha':
      case 'petCollection':
        this.renderGame(ctx);
        break;
      case 'gameover':
        this.renderGameOver(ctx);
        break;
      case 'victory':
        this.renderVictory(ctx);
        break;
      case 'story':
        // 先渲染游戏背景
        this.renderGame(ctx);
        // 再渲染剧情
        if (StorySystem) {
          StorySystem.render(ctx, this.canvas, this);
        }
        break;
    }
  }

  // 渲染游戏画面
  renderGame(ctx) {
    if (!this.dungeon) return;

    // 渲染地图
    this.renderMap(ctx);

    // 渲染物品
    this.renderItems(ctx);

    // 渲染宝箱
    this.renderChests(ctx);

    // 渲染NPC
    this.renderNPCs(ctx);

    // 渲染怪物
    for (const monster of this.monsters) {
      if (!monster.dead) {
        monster.render(ctx, this.camera);
      }
    }

    // 渲染玩家
    if (this.player && !this.player.dead) {
      this.player.render(ctx, this.camera);
    }

    // 渲染宠物
    PetManager.render(ctx, this.camera);

    // 渲染投射物
    this.renderProjectiles(ctx);

    // 渲染粒子
    ParticleSystem.render(ctx, this.camera);

    // 渲染陷阱（已触发的显示）
    this.renderTraps(ctx);

    // HUD
    HUD.render(ctx, this);

    // 暂停菜单
    if (this.state === 'paused') {
      this.renderPauseMenu(ctx);
    }

    // 背包
    if (this.state === 'inventory') {
      this.renderInventory(ctx);
    }

    // 角色面板
    if (this.state === 'character') {
      this.renderCharacter(ctx);
    }

    // 任务面板
    if (this.state === 'quests') {
      this.renderQuests(ctx);
    }

    // 地图
    if (this.state === 'map') {
      this.renderFullMap(ctx);
    }

    // 对话
    if (this.state === 'dialogue') {
      this.renderDialogue(ctx);
    }

    // 合成
    if (this.state === 'crafting') {
      CraftingManager.renderUI(ctx, this);
    }

    // 商店
    if (this.state === 'shop') {
      ShopManager.renderUI(ctx, this);
    }

    // 强化
    if (this.state === 'enchant') {
      EnchantManager.renderUI(ctx, this);
    }

    // 竞技场
    if (this.state === 'arena') {
      ArenaManager.renderMenu(ctx, this);
    }

    // 称号
    if (this.state === 'titles') {
      TitleManager.renderUI(ctx, this);
    }

    // 抽卡
    if (this.state === 'gacha') {
      PetManager.renderGachaUI(ctx, this);
    }

    // 宠物图鉴
    if (this.state === 'petCollection') {
      PetManager.renderPetCollection(ctx, this);
    }

    // 竞技场HUD
    if (ArenaManager.isInArena) {
      ArenaManager.renderHUD(ctx, this);
    }

    // FPS
    ctx.fillStyle = 'rgba(255,255,255,0.5)';
    ctx.font = '11px Arial';
    ctx.textAlign = 'right';
    ctx.fillText(`FPS: ${this.fps}`, this.canvas.width - 10, this.canvas.height - 10);
  }

  // 渲染地图
  renderMap(ctx) {
    const startTileX = Math.floor(this.camera.x / DungeonGenerator.TILE_SIZE);
    const startTileY = Math.floor(this.camera.y / DungeonGenerator.TILE_SIZE);
    const endTileX = Math.ceil((this.camera.x + this.canvas.width) / DungeonGenerator.TILE_SIZE);
    const endTileY = Math.ceil((this.camera.y + this.canvas.height) / DungeonGenerator.TILE_SIZE);

    for (let ty = startTileY; ty <= endTileY; ty++) {
      for (let tx = startTileX; tx <= endTileX; tx++) {
        if (ty < 0 || ty >= this.dungeon.height || tx < 0 || tx >= this.dungeon.width) continue;

        const tile = this.dungeon.tiles[ty][tx];
        const screenX = tx * DungeonGenerator.TILE_SIZE - this.camera.x;
        const screenY = ty * DungeonGenerator.TILE_SIZE - this.camera.y;

        if (tile === DungeonGenerator.TILE_TYPES.FLOOR) {
          // 地板
          ctx.fillStyle = '#2a2a3e';
          ctx.fillRect(screenX, screenY, DungeonGenerator.TILE_SIZE, DungeonGenerator.TILE_SIZE);
          // 地板纹理
          ctx.fillStyle = '#252538';
          ctx.fillRect(screenX + 1, screenY + 1, DungeonGenerator.TILE_SIZE - 2, DungeonGenerator.TILE_SIZE - 2);
        } else if (tile === DungeonGenerator.TILE_TYPES.WALL) {
          // 墙壁
          ctx.fillStyle = '#1a1a2e';
          ctx.fillRect(screenX, screenY, DungeonGenerator.TILE_SIZE, DungeonGenerator.TILE_SIZE);
          ctx.fillStyle = '#16162a';
          ctx.fillRect(screenX + 2, screenY + 2, DungeonGenerator.TILE_SIZE - 4, DungeonGenerator.TILE_SIZE - 4);
        } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_DOWN) {
          ctx.fillStyle = '#2a2a3e';
          ctx.fillRect(screenX, screenY, DungeonGenerator.TILE_SIZE, DungeonGenerator.TILE_SIZE);
          ctx.fillStyle = '#e74c3c';
          ctx.font = '20px Arial';
          ctx.textAlign = 'center';
          ctx.textBaseline = 'middle';
          ctx.fillText('⬇', screenX + 16, screenY + 16);
        } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_UP) {
          ctx.fillStyle = '#2a2a3e';
          ctx.fillRect(screenX, screenY, DungeonGenerator.TILE_SIZE, DungeonGenerator.TILE_SIZE);
          ctx.fillStyle = '#3498db';
          ctx.font = '20px Arial';
          ctx.textAlign = 'center';
          ctx.textBaseline = 'middle';
          ctx.fillText('⬆', screenX + 16, screenY + 16);
        }
      }
    }

    // 火把光照
    for (const torch of this.torches) {
      const screenX = torch.x - this.camera.x;
      const screenY = torch.y - this.camera.y;
      const flicker = Math.sin(Date.now() / 100 + torch.flickerOffset) * 0.2 + 0.8;

      const gradient = ctx.createRadialGradient(screenX, screenY, 0, screenX, screenY, 80 * flicker);
      gradient.addColorStop(0, 'rgba(255, 150, 50, 0.3)');
      gradient.addColorStop(1, 'rgba(255, 150, 50, 0)');
      ctx.fillStyle = gradient;
      ctx.fillRect(screenX - 80, screenY - 80, 160, 160);

      ctx.font = '16px Arial';
      ctx.textAlign = 'center';
      ctx.fillText('🔥', screenX, screenY);
    }
  }

  // 渲染物品
  renderItems(ctx) {
    for (const item of this.items) {
      const screenX = item.x - this.camera.x;
      const screenY = item.y - this.camera.y + Math.sin(Date.now() / 300 + item.bobOffset) * 3;

      const itemData = ItemData.getItem(item.itemId);
      if (itemData) {
        // 光晕
        const gradient = ctx.createRadialGradient(screenX, screenY, 0, screenX, screenY, 20);
        gradient.addColorStop(0, 'rgba(241, 196, 15, 0.3)');
        gradient.addColorStop(1, 'rgba(241, 196, 15, 0)');
        ctx.fillStyle = gradient;
        ctx.fillRect(screenX - 20, screenY - 20, 40, 40);

        ctx.font = '20px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(itemData.icon, screenX, screenY);
      }
    }
  }

  // 渲染宝箱
  renderChests(ctx) {
    for (const chest of this.chests) {
      const screenX = chest.x - this.camera.x;
      const screenY = chest.y - this.camera.y;

      ctx.font = '24px Arial';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      ctx.fillText(chest.opened ? '📭' : '📦', screenX, screenY);

      if (!chest.opened && chest.locked) {
        ctx.fillStyle = '#f1c40f';
        ctx.font = '12px Arial';
        ctx.fillText('🔒', screenX + 12, screenY - 10);
      }
    }
  }

  // 渲染NPC
  renderNPCs(ctx) {
    for (const npc of this.npcs) {
      const screenX = npc.x - this.camera.x;
      const screenY = npc.y - this.camera.y;

      // 光环
      const gradient = ctx.createRadialGradient(screenX, screenY, 0, screenX, screenY, 30);
      gradient.addColorStop(0, 'rgba(52, 152, 219, 0.3)');
      gradient.addColorStop(1, 'rgba(52, 152, 219, 0)');
      ctx.fillStyle = gradient;
      ctx.fillRect(screenX - 30, screenY - 30, 60, 60);

      ctx.font = '28px Arial';
      ctx.textAlign = 'center';
      ctx.textBaseline = 'middle';
      const npcInfo = DialogueData.getNPCInfo(npc.type);
      ctx.fillText(npcInfo?.icon || '🧙', screenX, screenY);

      // 交互提示
      const dist = Utils.distance(this.player.x, this.player.y, npc.x, npc.y);
      if (dist < 60) {
        ctx.fillStyle = '#f1c40f';
        ctx.font = 'bold 11px Arial';
        ctx.fillText('按 E 对话', screenX, screenY - 25);
      }
    }
  }

  // 渲染投射物
  renderProjectiles(ctx) {
    for (const proj of this.projectiles) {
      const screenX = proj.x - this.camera.x;
      const screenY = proj.y - this.camera.y;

      ctx.fillStyle = proj.color || '#fff';
      ctx.beginPath();
      ctx.arc(screenX, screenY, 6, 0, Math.PI * 2);
      ctx.fill();

      // 拖尾
      ctx.globalAlpha = 0.5;
      ctx.beginPath();
      ctx.arc(screenX - proj.vx, screenY - proj.vy, 4, 0, Math.PI * 2);
      ctx.fill();
      ctx.globalAlpha = 1;
    }
  }

  // 渲染陷阱
  renderTraps(ctx) {
    for (const trap of this.traps) {
      if (trap.triggered) continue;
      const screenX = trap.x - this.camera.x;
      const screenY = trap.y - this.camera.y;

      ctx.globalAlpha = 0.3;
      ctx.fillStyle = trap.type === 'spike' ? '#95a5a6' :
                      trap.type === 'poison' ? '#27ae60' :
                      trap.type === 'fire' ? '#e67e22' : '#3498db';
      ctx.beginPath();
      ctx.arc(screenX, screenY, 15, 0, Math.PI * 2);
      ctx.fill();
      ctx.globalAlpha = 1;
    }
  }

  // 渲染主菜单
  renderMenu(ctx) {
    // 背景
    const gradient = ctx.createLinearGradient(0, 0, 0, this.canvas.height);
    gradient.addColorStop(0, '#1a1a2e');
    gradient.addColorStop(1, '#0a0a1a');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    // 标题
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 64px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText('永恒地牢', this.canvas.width / 2, 150);

    ctx.fillStyle = '#9b59b6';
    ctx.font = '24px Arial';
    ctx.fillText('Eternal Dungeon', this.canvas.width / 2, 200);

    // 菜单选项
    const options = [
      { text: '开始游戏', action: 'start' },
      { text: '选择职业', action: 'class' },
      { text: '继续游戏', action: 'continue' },
      { text: '设置', action: 'settings' },
      { text: '退出', action: 'exit' }
    ];

    options.forEach((opt, i) => {
      const y = 300 + i * 60;
      const isHovered = this.mouseY > y - 20 && this.mouseY < y + 20 &&
                        this.mouseX > this.canvas.width / 2 - 100 && this.mouseX < this.canvas.width / 2 + 100;

      ctx.fillStyle = isHovered ? '#f1c40f' : '#fff';
      ctx.font = isHovered ? 'bold 28px Arial' : '24px Arial';
      ctx.fillText(opt.text, this.canvas.width / 2, y);
    });

    // 提示
    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.fillText('点击选项开始游戏 | WASD移动 | 鼠标攻击 | 1-5技能', this.canvas.width / 2, this.canvas.height - 50);
  }

  // 渲染职业选择
  renderClassSelect(ctx) {
    const gradient = ctx.createLinearGradient(0, 0, 0, this.canvas.height);
    gradient.addColorStop(0, '#1a1a2e');
    gradient.addColorStop(1, '#0a0a1a');
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 48px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('选择你的职业', this.canvas.width / 2, 80);

    const classes = [
      { id: 'warrior', name: '战士', icon: '⚔️', desc: '高生命高防御，近战输出', color: '#e74c3c' },
      { id: 'mage', name: '法师', icon: '🔮', desc: '强大的魔法伤害，远程输出', color: '#9b59b6' },
      { id: 'ranger', name: '游侠', icon: '🏹', desc: '敏捷的远程射手，高暴击', color: '#27ae60' },
      { id: 'rogue', name: '盗贼', icon: '🗡️', desc: '高爆发高闪避，刺客型', color: '#34495e' },
      { id: 'paladin', name: '圣骑士', icon: '✨', desc: '攻防兼备，可治疗', color: '#f1c40f' },
      { id: 'necromancer', name: '死灵法师', icon: '💀', desc: '召唤亡灵大军，持续伤害', color: '#2c3e50' }
    ];

    classes.forEach((cls, i) => {
      const x = 150 + (i % 3) * 330;
      const y = 150 + Math.floor(i / 3) * 250;
      const isSelected = this.selectedClass === cls.id;
      const isHovered = this.mouseX > x && this.mouseX < x + 280 && this.mouseY > y && this.mouseY < y + 200;

      ctx.fillStyle = isSelected ? 'rgba(241, 196, 15, 0.2)' : isHovered ? 'rgba(255,255,255,0.1)' : 'rgba(0,0,0,0.5)';
      ctx.fillRect(x, y, 280, 200);
      ctx.strokeStyle = isSelected ? '#f1c40f' : cls.color;
      ctx.lineWidth = isSelected ? 3 : 2;
      ctx.strokeRect(x, y, 280, 200);

      ctx.fillStyle = cls.color;
      ctx.font = '48px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(cls.icon, x + 140, y + 60);

      ctx.fillStyle = '#fff';
      ctx.font = 'bold 24px Arial';
      ctx.fillText(cls.name, x + 140, y + 110);

      ctx.fillStyle = '#aaa';
      ctx.font = '14px Arial';
      ctx.fillText(cls.desc, x + 140, y + 145);

      if (isSelected) {
        ctx.fillStyle = '#f1c40f';
        ctx.font = 'bold 16px Arial';
        ctx.fillText('✓ 已选择', x + 140, y + 180);
      }
    });

    // 开始按钮
    ctx.fillStyle = this.selectedClass ? '#2ecc71' : '#555';
    ctx.fillRect(this.canvas.width / 2 - 100, this.canvas.height - 80, 200, 50);
    ctx.fillStyle = '#fff';
    ctx.font = 'bold 20px Arial';
    ctx.fillText('开始冒险', this.canvas.width / 2, this.canvas.height - 50);

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.fillText('点击选择职业，再点击开始冒险', this.canvas.width / 2, this.canvas.height - 20);
  }

  // 渲染暂停菜单
  renderPauseMenu(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.7)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#fff';
    ctx.font = 'bold 48px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('游戏暂停', this.canvas.width / 2, 200);

    const options = ['继续游戏', '保存游戏', '设置', '返回主菜单'];
    options.forEach((opt, i) => {
      ctx.fillStyle = '#fff';
      ctx.font = '24px Arial';
      ctx.fillText(opt, this.canvas.width / 2, 300 + i * 50);
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '16px Arial';
    ctx.fillText('按 ESC 继续游戏', this.canvas.width / 2, this.canvas.height - 50);
  }

  // 渲染背包
  renderInventory(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 32px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('背包', this.canvas.width / 2, 60);

    // 装备栏
    const equipSlots = ['weapon', 'armor', 'helmet', 'boots', 'ring', 'amulet'];
    const equipNames = ['武器', '护甲', '头盔', '靴子', '戒指', '护符'];
    equipSlots.forEach((slot, i) => {
      const x = 100 + (i % 3) * 80;
      const y = 120 + Math.floor(i / 3) * 80;
      ctx.fillStyle = '#333';
      ctx.fillRect(x, y, 70, 70);
      ctx.strokeStyle = '#555';
      ctx.strokeRect(x, y, 70, 70);
      ctx.fillStyle = '#888';
      ctx.font = '12px Arial';
      ctx.textAlign = 'center';
      ctx.fillText(equipNames[i], x + 35, y + 35);

      const item = this.player.equipment[slot];
      if (item) {
        ctx.font = '28px Arial';
        ctx.fillText(item.icon, x + 35, y + 40);
      }
    });

    // 背包格子
    ctx.fillStyle = '#fff';
    ctx.font = '20px Arial';
    ctx.fillText('物品栏', 450, 110);

    for (let i = 0; i < 30; i++) {
      const x = 350 + (i % 6) * 70;
      const y = 130 + Math.floor(i / 6) * 70;
      ctx.fillStyle = '#222';
      ctx.fillRect(x, y, 60, 60);
      ctx.strokeStyle = '#444';
      ctx.strokeRect(x, y, 60, 60);

      const item = this.player.inventory.items[i];
      if (item) {
        ctx.font = '24px Arial';
        ctx.textAlign = 'center';
        ctx.fillText(item.icon, x + 30, y + 35);
        if (item.quantity > 1) {
          ctx.fillStyle = '#fff';
          ctx.font = 'bold 12px Arial';
          ctx.textAlign = 'right';
          ctx.fillText(item.quantity, x + 55, y + 55);
        }
      }
    }

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 I 或 ESC 关闭', this.canvas.width / 2, this.canvas.height - 30);
  }

  // 渲染角色面板
  renderCharacter(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    const p = this.player;
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 32px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('角色信息', this.canvas.width / 2, 60);

    ctx.fillStyle = '#fff';
    ctx.font = '18px Arial';
    ctx.textAlign = 'left';
    const stats = [
      `名字: ${p.name}`,
      `职业: ${ClassData[p.classType]?.name || p.classType}`,
      `等级: ${p.level}`,
      `经验: ${p.exp} / ${p.expToNext}`,
      ``,
      `生命: ${Math.floor(p.hp)} / ${p.maxHp}`,
      `魔法: ${Math.floor(p.mp)} / ${p.maxMp}`,
      `体力: ${Math.floor(p.stamina)} / ${p.maxStamina}`,
      ``,
      `力量: ${p.baseStats.str}`,
      `敏捷: ${p.baseStats.dex}`,
      `智力: ${p.baseStats.int}`,
      `体质: ${p.baseStats.vit}`,
      ``,
      `攻击力: ${Math.floor(p.damage)}`,
      `护甲: ${p.armor}`,
      `暴击率: ${(p.crit * 100).toFixed(1)}%`,
      `闪避率: ${(p.dodge * 100).toFixed(1)}%`,
      ``,
      `可用属性点: ${p.attributePoints}`,
      `可用技能点: ${p.skillPoints}`
    ];

    stats.forEach((line, i) => {
      ctx.fillText(line, 100, 120 + i * 28);
    });

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 C 或 ESC 关闭', this.canvas.width / 2, this.canvas.height - 30);
  }

  // 渲染任务面板
  renderQuests(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 32px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('任务日志', this.canvas.width / 2, 60);

    const activeQuests = QuestSystem.getActiveQuests();
    if (activeQuests.length === 0) {
      ctx.fillStyle = '#888';
      ctx.font = '18px Arial';
      ctx.fillText('暂无进行中的任务', this.canvas.width / 2, 200);
    } else {
      activeQuests.forEach((quest, i) => {
        const y = 120 + i * 100;
        ctx.fillStyle = quest.completed ? '#2ecc71' : '#fff';
        ctx.font = 'bold 18px Arial';
        ctx.textAlign = 'left';
        ctx.fillText(`[${quest.type === 'main' ? '主线' : quest.type === 'daily' ? '每日' : '支线'}] ${quest.name}`, 100, y);

        ctx.fillStyle = '#aaa';
        ctx.font = '14px Arial';
        ctx.fillText(quest.description, 100, y + 25);

        quest.progress.forEach((obj, j) => {
          ctx.fillStyle = obj.completed ? '#2ecc71' : '#fff';
          ctx.fillText(`  ${obj.completed ? '✓' : '○'} ${obj.description} (${obj.current}/${obj.count})`, 120, y + 50 + j * 20);
        });
      });
    }

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 Q 或 ESC 关闭', this.canvas.width / 2, this.canvas.height - 30);
  }

  // 渲染完整地图
  renderFullMap(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.9)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 32px Arial';
    ctx.textAlign = 'center';
    ctx.fillText(`第 ${this.currentFloor} 层地图`, this.canvas.width / 2, 50);

    if (this.dungeon) {
      const scale = Math.min(
        (this.canvas.width - 100) / this.dungeon.width,
        (this.canvas.height - 150) / this.dungeon.height
      );
      const offsetX = (this.canvas.width - this.dungeon.width * scale) / 2;
      const offsetY = 100;

      for (let ty = 0; ty < this.dungeon.height; ty++) {
        for (let tx = 0; tx < this.dungeon.width; tx++) {
          const tile = this.dungeon.tiles[ty][tx];
          if (tile === DungeonGenerator.TILE_TYPES.FLOOR) {
            ctx.fillStyle = '#4a4a6a';
          } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_DOWN) {
            ctx.fillStyle = '#e74c3c';
          } else if (tile === DungeonGenerator.TILE_TYPES.STAIRS_UP) {
            ctx.fillStyle = '#3498db';
          } else {
            continue;
          }
          ctx.fillRect(offsetX + tx * scale, offsetY + ty * scale, scale, scale);
        }
      }

      // 玩家位置
      const px = offsetX + (this.player.x / DungeonGenerator.TILE_SIZE) * scale;
      const py = offsetY + (this.player.y / DungeonGenerator.TILE_SIZE) * scale;
      ctx.fillStyle = '#2ecc71';
      ctx.beginPath();
      ctx.arc(px, py, 6, 0, Math.PI * 2);
      ctx.fill();
    }

    ctx.fillStyle = '#7f8c8d';
    ctx.font = '14px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('按 M 或 ESC 关闭', this.canvas.width / 2, this.canvas.height - 30);
  }

  // 渲染对话
  renderDialogue(ctx) {
    if (!this.dialogueData) return;

    const d = this.dialogueData;
    const currentLine = d.currentDialogue[d.currentIndex];
    if (!currentLine) return;

    // 对话框背景
    ctx.fillStyle = 'rgba(0,0,0,0.85)';
    ctx.fillRect(50, this.canvas.height - 200, this.canvas.width - 100, 180);
    ctx.strokeStyle = '#f1c40f';
    ctx.lineWidth = 2;
    ctx.strokeRect(50, this.canvas.height - 200, this.canvas.width - 100, 180);

    // NPC名字和头像
    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 20px Arial';
    ctx.textAlign = 'left';
    ctx.fillText(`${d.npcIcon} ${d.npcName}`, 80, this.canvas.height - 170);

    // 对话文本
    ctx.fillStyle = '#fff';
    ctx.font = '16px Arial';
    ctx.fillText(currentLine.text, 80, this.canvas.height - 130);

    // 选项
    if (currentLine.options) {
      currentLine.options.forEach((opt, i) => {
        const y = this.canvas.height - 90 + i * 25;
        ctx.fillStyle = '#3498db';
        ctx.font = '14px Arial';
        ctx.fillText(`${i + 1}. ${opt.text}`, 100, y);
      });
    }
  }

  // 渲染游戏结束
  renderGameOver(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#e74c3c';
    ctx.font = 'bold 64px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('你死了', this.canvas.width / 2, 250);

    ctx.fillStyle = '#fff';
    ctx.font = '24px Arial';
    ctx.fillText(`到达层数: ${this.currentFloor}`, this.canvas.width / 2, 330);
    ctx.fillText(`等级: ${this.player?.level || 1}`, this.canvas.width / 2, 370);
    ctx.fillText(`游戏时间: ${Math.floor(this.gameTime / 60)}分${Math.floor(this.gameTime % 60)}秒`, this.canvas.width / 2, 410);

    ctx.fillStyle = '#f1c40f';
    ctx.font = '20px Arial';
    ctx.fillText('点击重新开始', this.canvas.width / 2, 500);
  }

  // 渲染胜利
  renderVictory(ctx) {
    ctx.fillStyle = 'rgba(0,0,0,0.8)';
    ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);

    ctx.fillStyle = '#f1c40f';
    ctx.font = 'bold 64px Arial';
    ctx.textAlign = 'center';
    ctx.fillText('🏆 胜利！🏆', this.canvas.width / 2, 200);

    ctx.fillStyle = '#fff';
    ctx.font = '24px Arial';
    ctx.fillText('你征服了永恒地牢！', this.canvas.width / 2, 280);
    ctx.fillText('你成为了真正的传奇！', this.canvas.width / 2, 320);

    ctx.fillStyle = '#f1c40f';
    ctx.font = '20px Arial';
    ctx.fillText('点击返回主菜单', this.canvas.width / 2, 450);
  }

  // 切换背包
  toggleInventory() {
    this.showInventory = !this.showInventory;
    this.state = this.showInventory ? 'inventory' : 'playing';
  }
}

// 启动游戏
window.addEventListener('load', () => {
  window.Game = new Game();
  requestAnimationFrame((t) => window.Game.gameLoop(t));
});

window.Game = Game;
