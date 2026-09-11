// ==================== 永恒地牢 - 输入管理系统 ====================
// 处理键盘、鼠标、游戏手柄输入，支持按键映射和组合键

const Input = {
  // 按键状态
  keys: {},
  keysPressed: {},  // 本帧刚按下的键
  keysReleased: {}, // 本帧刚释放的键
  mouse: {
    x: 0, y: 0,
    worldX: 0, worldY: 0,
    down: false,
    rightDown: false,
    middleDown: false,
    pressed: false,
    rightPressed: false,
    wheel: 0,
    lastClickTime: 0,
    clickCount: 0
  },
  // 按键映射（可自定义）
  keyBindings: {
    up: ['KeyW', 'ArrowUp'],
    down: ['KeyS', 'ArrowDown'],
    left: ['KeyA', 'ArrowLeft'],
    right: ['KeyD', 'ArrowRight'],
    skill1: ['KeyQ'],
    skill2: ['KeyW'], // 注意：W同时是移动和技能，需要特殊处理
    skill3: ['KeyE'],
    skill4: ['KeyR'],
    ultimate: ['KeyF'],
    interact: ['KeyE', 'Space'],
    inventory: ['KeyI', 'Tab'],
    map: ['KeyM'],
    quest: ['KeyJ'],
    talents: ['KeyK'],
    craft: ['KeyC'],
    rune: ['KeyN'],
    pause: ['Escape'],
    potion1: ['Digit1'],
    potion2: ['Digit2'],
    potion3: ['Digit3'],
    potion4: ['Digit4'],
    potion5: ['Digit5'],
    sprint: ['ShiftLeft', 'ShiftRight'],
    attack: ['Space']
  },
  // 初始化
  init(canvas) {
    this.canvas = canvas;
    this.bindEvents();
    console.log('[Input] 输入系统初始化完成');
  },
  // 兼容game.js的InputManager接口
  setMouseDown(down) {
    this.mouse.down = down;
    if (down) {
      this.mouse.pressed = true;
    }
  },
  onKeyDown(e) {
    // 空格键兼容性处理：有些浏览器 key code 不一样
    let keyCode = e.code;
    if (e.key === ' ' || e.keyCode === 32) {
      keyCode = 'Space';
    }
    if (!this.keys[keyCode]) {
      this.keysPressed[keyCode] = true;
    }
    this.keys[keyCode] = true;
    if (['Space', 'ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'Tab'].includes(keyCode)) {
      e.preventDefault();
    }
  },
  onKeyUp(e) {
    // 空格键兼容性处理
    let keyCode = e.code;
    if (e.key === ' ' || e.keyCode === 32) {
      keyCode = 'Space';
    }
    this.keys[keyCode] = false;
    this.keysReleased[keyCode] = true;
  },
  // 绑定事件
  bindEvents() {
    // 键盘按下 - 调用 onKeyDown 方法（含空格键兼容性处理）
    window.addEventListener('keydown', (e) => this.onKeyDown(e));
    // 键盘释放 - 调用 onKeyUp 方法
    window.addEventListener('keyup', (e) => this.onKeyUp(e));
    // 鼠标移动
    this.canvas.addEventListener('mousemove', (e) => {
      const rect = this.canvas.getBoundingClientRect();
      this.mouse.x = e.clientX - rect.left;
      this.mouse.y = e.clientY - rect.top;
    });
    // 鼠标按下
    this.canvas.addEventListener('mousedown', (e) => {
      if (e.button === 0) {
        this.mouse.down = true;
        this.mouse.pressed = true;
        // 双击检测
        const now = Date.now();
        if (now - this.mouse.lastClickTime < 300) {
          this.mouse.clickCount++;
        } else {
          this.mouse.clickCount = 1;
        }
        this.mouse.lastClickTime = now;
      } else if (e.button === 2) {
        this.mouse.rightDown = true;
        this.mouse.rightPressed = true;
      } else if (e.button === 1) {
        this.mouse.middleDown = true;
      }
    });
    // 鼠标释放
    window.addEventListener('mouseup', (e) => {
      if (e.button === 0) this.mouse.down = false;
      else if (e.button === 2) this.mouse.rightDown = false;
      else if (e.button === 1) this.mouse.middleDown = false;
    });
    // 鼠标滚轮
    this.canvas.addEventListener('wheel', (e) => {
      this.mouse.wheel = e.deltaY > 0 ? 1 : -1;
      e.preventDefault();
    }, { passive: false });
    // 右键菜单
    this.canvas.addEventListener('contextmenu', (e) => {
      e.preventDefault();
    });
    // 窗口失焦时清除按键状态
    window.addEventListener('blur', () => {
      this.resetAll();
    });
    // 页面隐藏时清除按键状态（切换标签页等）
    document.addEventListener('visibilitychange', () => {
      if (document.hidden) {
        this.resetAll();
      }
    });
    // 触摸支持（移动端）
    this.setupTouch();
  },
  // 重置所有输入状态（防止按键卡住）
  resetAll() {
    this.keys = {};
    this.keysPressed = {};
    this.keysReleased = {};
    this.mouse.down = false;
    this.mouse.rightDown = false;
    this.mouse.middleDown = false;
    this.mouse.pressed = false;
    this.mouse.rightPressed = false;
    this.mouse.wheel = 0;
  },
  // 触摸支持
  setupTouch() {
    let touchStartX = 0, touchStartY = 0;
    this.canvas.addEventListener('touchstart', (e) => {
      e.preventDefault();
      const touch = e.touches[0];
      const rect = this.canvas.getBoundingClientRect();
      this.mouse.x = touch.clientX - rect.left;
      this.mouse.y = touch.clientY - rect.top;
      this.mouse.down = true;
      this.mouse.pressed = true;
      touchStartX = this.mouse.x;
      touchStartY = this.mouse.y;
    }, { passive: false });
    this.canvas.addEventListener('touchmove', (e) => {
      e.preventDefault();
      const touch = e.touches[0];
      const rect = this.canvas.getBoundingClientRect();
      this.mouse.x = touch.clientX - rect.left;
      this.mouse.y = touch.clientY - rect.top;
    }, { passive: false });
    this.canvas.addEventListener('touchend', (e) => {
      e.preventDefault();
      this.mouse.down = false;
    }, { passive: false });
  },
  // 每帧结束时调用，清除单帧状态
  endFrame() {
    this.keysPressed = {};
    this.keysReleased = {};
    this.mouse.pressed = false;
    this.mouse.rightPressed = false;
    this.mouse.wheel = 0;
  },
  // 检查动作是否按下（持续）
  isActionDown(action) {
    const keys = this.keyBindings[action];
    if (!keys) return false;
    return keys.some(key => this.keys[key]);
  },
  // 检查动作是否刚按下（单帧）
  isActionPressed(action) {
    const keys = this.keyBindings[action];
    if (!keys) return false;
    return keys.some(key => this.keysPressed[key]);
  },
  // 检查动作是否刚释放（单帧）
  isActionReleased(action) {
    const keys = this.keyBindings[action];
    if (!keys) return false;
    return keys.some(key => this.keysReleased[key]);
  },
  // 获取移动方向
  getMovementDirection() {
    let x = 0, y = 0;
    if (this.isActionDown('left')) x -= 1;
    if (this.isActionDown('right')) x += 1;
    if (this.isActionDown('up')) y -= 1;
    if (this.isActionDown('down')) y += 1;
    // 归一化
    if (x !== 0 && y !== 0) {
      const len = Math.sqrt(x * x + y * y);
      x /= len;
      y /= len;
    }
    return { x, y };
  },
  // 获取鼠标朝向角度
  getMouseAngle(centerX, centerY) {
    return Math.atan2(this.mouse.y - centerY, this.mouse.x - centerX);
  },
  // 检查是否双击
  isDoubleClick() {
    return this.mouse.clickCount >= 2 && Date.now() - this.mouse.lastClickTime < 100;
  },
  // 重新绑定按键
  rebindKey(action, newKey) {
    if (this.keyBindings[action]) {
      this.keyBindings[action] = [newKey];
    }
  },
  // 重置为默认按键
  resetBindings() {
    this.keyBindings = {
      up: ['KeyW', 'ArrowUp'],
      down: ['KeyS', 'ArrowDown'],
      left: ['KeyA', 'ArrowLeft'],
      right: ['KeyD', 'ArrowRight'],
      skill1: ['KeyQ'],
      skill2: ['KeyW'],
      skill3: ['KeyE'],
      skill4: ['KeyR'],
      ultimate: ['KeyF'],
      interact: ['KeyE', 'Space'],
      inventory: ['KeyI', 'Tab'],
      map: ['KeyM'],
      quest: ['KeyJ'],
      talents: ['KeyK'],
      craft: ['KeyC'],
      pause: ['Escape'],
      potion1: ['Digit1'],
      potion2: ['Digit2'],
      potion3: ['Digit3'],
      potion4: ['Digit4'],
      potion5: ['Digit5'],
      sprint: ['ShiftLeft', 'ShiftRight'],
      attack: ['Space']
    };
  },
  // 导出/导入按键配置
  exportBindings() {
    return JSON.stringify(this.keyBindings);
  },
  importBindings(json) {
    try {
      this.keyBindings = JSON.parse(json);
      return true;
    } catch (e) {
      return false;
    }
  }
};
window.Input = Input;
window.InputManager = Input;  // 兼容game.js中的InputManager引用
