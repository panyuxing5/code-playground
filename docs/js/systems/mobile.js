// ==================== 永恒地牢 - 移动端控制 ====================
// 虚拟摇杆、触摸按钮、手势识别

const MobileControls = {
  isMobile: false,
  joystickActive: false,
  joystickStartX: 0,
  joystickStartY: 0,
  joystickCurrentX: 0,
  joystickCurrentY: 0,
  joystickMaxRadius: 50,
  moveDirection: { x: 0, y: 0 },

  // 初始化
  init: function() {
    this.isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)
      || window.innerWidth <= 768;

    if (!this.isMobile) return;

    console.log('[Mobile] 移动端控制已启用');
    this.setupJoystick();
    this.setupActionButtons();
    this.setupCanvasTouch();
  },

  // 设置虚拟摇杆
  setupJoystick: function() {
    const joystick = document.getElementById('joystick');
    const knob = document.getElementById('joystickKnob');
    if (!joystick || !knob) return;

    const handleStart = (e) => {
      e.preventDefault();
      this.joystickActive = true;
      const touch = e.touches ? e.touches[0] : e;
      const rect = joystick.getBoundingClientRect();
      this.joystickStartX = rect.left + rect.width / 2;
      this.joystickStartY = rect.top + rect.height / 2;
      this.updateJoystick(touch.clientX, touch.clientY, knob);
    };

    const handleMove = (e) => {
      if (!this.joystickActive) return;
      e.preventDefault();
      const touch = e.touches ? e.touches[0] : e;
      this.updateJoystick(touch.clientX, touch.clientY, knob);
    };

    const handleEnd = (e) => {
      e.preventDefault();
      this.joystickActive = false;
      this.moveDirection = { x: 0, y: 0 };
      knob.style.transform = 'translate(-50%, -50%)';
      // 重置键盘输入
      if (window.InputManager) {
        InputManager.keys['w'] = false;
        InputManager.keys['a'] = false;
        InputManager.keys['s'] = false;
        InputManager.keys['d'] = false;
      }
    };

    joystick.addEventListener('touchstart', handleStart);
    joystick.addEventListener('touchmove', handleMove);
    joystick.addEventListener('touchend', handleEnd);
    joystick.addEventListener('mousedown', handleStart);
    document.addEventListener('mousemove', handleMove);
    document.addEventListener('mouseup', handleEnd);
  },

  // 更新摇杆位置
  updateJoystick: function(clientX, clientY, knob) {
    let dx = clientX - this.joystickStartX;
    let dy = clientY - this.joystickStartY;
    const distance = Math.sqrt(dx * dx + dy * dy);

    if (distance > this.joystickMaxRadius) {
      dx = (dx / distance) * this.joystickMaxRadius;
      dy = (dy / distance) * this.joystickMaxRadius;
    }

    knob.style.transform = `translate(calc(-50% + ${dx}px), calc(-50% + ${dy}px))`;

    // 归一化方向
    this.moveDirection = {
      x: dx / this.joystickMaxRadius,
      y: dy / this.joystickMaxRadius
    };

    // 映射到键盘输入
    if (window.InputManager) {
      InputManager.keys['w'] = this.moveDirection.y < -0.3;
      InputManager.keys['s'] = this.moveDirection.y > 0.3;
      InputManager.keys['a'] = this.moveDirection.x < -0.3;
      InputManager.keys['d'] = this.moveDirection.x > 0.3;
    }
  },

  // 设置动作按钮
  setupActionButtons: function() {
    const btnAttack = document.getElementById('btnAttack');
    const btnSkill1 = document.getElementById('btnSkill1');
    const btnSkill2 = document.getElementById('btnSkill2');
    const btnDash = document.getElementById('btnDash');

    if (btnAttack) {
      btnAttack.addEventListener('touchstart', (e) => {
        e.preventDefault();
        if (window.Game && window.Game.player) {
          window.Game.player.performAttack(window.Game);
        }
      });
    }

    if (btnSkill1) {
      btnSkill1.addEventListener('touchstart', (e) => {
        e.preventDefault();
        if (window.InputManager) {
          InputManager.keys['1'] = true;
          setTimeout(() => { InputManager.keys['1'] = false; }, 100);
        }
      });
    }

    if (btnSkill2) {
      btnSkill2.addEventListener('touchstart', (e) => {
        e.preventDefault();
        if (window.InputManager) {
          InputManager.keys['2'] = true;
          setTimeout(() => { InputManager.keys['2'] = false; }, 100);
        }
      });
    }

    if (btnDash) {
      btnDash.addEventListener('touchstart', (e) => {
        e.preventDefault();
        if (window.InputManager) {
          InputManager.keys[' '] = true;
          setTimeout(() => { InputManager.keys[' '] = false; }, 100);
        }
      });
    }
  },

  // 设置Canvas触摸
  setupCanvasTouch: function() {
    const canvas = document.getElementById('gameCanvas');
    if (!canvas) return;

    canvas.addEventListener('touchstart', (e) => {
      e.preventDefault();
      const touch = e.touches[0];
      const rect = canvas.getBoundingClientRect();
      const scaleX = canvas.width / rect.width;
      const scaleY = canvas.height / rect.height;

      if (window.Game) {
        window.Game.mouseX = (touch.clientX - rect.left) * scaleX;
        window.Game.mouseY = (touch.clientY - rect.top) * scaleY;

        // 模拟鼠标点击
        if (window.Game.state === 'playing') {
          window.Game.player?.performAttack(window.Game);
        } else if (window.Game.state === 'menu') {
          window.Game.handleMenuClick();
        } else if (window.Game.state === 'classSelect') {
          window.Game.handleClassSelectClick();
        } else if (window.Game.state === 'story' && window.StorySystem) {
          StorySystem.handleClick(window.Game);
        }
      }
    });

    canvas.addEventListener('touchmove', (e) => {
      e.preventDefault();
      const touch = e.touches[0];
      const rect = canvas.getBoundingClientRect();
      const scaleX = canvas.width / rect.width;
      const scaleY = canvas.height / rect.height;

      if (window.Game) {
        window.Game.mouseX = (touch.clientX - rect.left) * scaleX;
        window.Game.mouseY = (touch.clientY - rect.top) * scaleY;
      }
    });
  },

  // 显示/隐藏控制
  setVisible: function(visible) {
    const controls = document.getElementById('mobileControls');
    if (controls) {
      controls.style.display = visible ? 'block' : 'none';
    }
  },

  // 获取移动方向（供游戏使用）
  getMoveDirection: function() {
    return this.moveDirection;
  }
};

// 页面加载完成后初始化
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => MobileControls.init());
} else {
  MobileControls.init();
}
