// ==================== 永恒地牢 - 炫酷登录注册系统 ====================
// 全屏动画背景 + 精致UI + 流畅交互

const AuthSystem = {
  currentUser: null,
  authToken: null,
  isOpen: false,
  currentTab: 'login',
  countdown: 0,
  countdownTimer: null,
  particles: [],

  // 初始化
  init() {
    // 从localStorage恢复会话
    const savedToken = localStorage.getItem('eternal_dungeon_token');
    const savedUser = localStorage.getItem('eternal_dungeon_user');

    if (savedToken && savedUser) {
      this.authToken = savedToken;
      try {
        this.currentUser = JSON.parse(savedUser);
      } catch (e) {
        this.currentUser = null;
      }
    }

    this.createStyles();
    this.createFloatingButton();
    this.createAuthOverlay();
    this.createParticleBackground();

    // 检查登录状态
    this.checkLoginStatus();

    console.log('[AuthSystem] 炫酷登录注册系统初始化完成');
  },

  // 创建样式
  createStyles() {
    const style = document.createElement('style');
    style.textContent = `
      /* 浮动登录按钮 */
      .auth-float-btn {
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9998;
        padding: 12px 28px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border: none;
        border-radius: 30px;
        cursor: pointer;
        font-size: 15px;
        font-weight: 600;
        box-shadow: 0 4px 20px rgba(102, 126, 234, 0.5);
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        backdrop-filter: blur(10px);
        letter-spacing: 0.5px;
      }
      .auth-float-btn:hover {
        transform: translateY(-2px) scale(1.05);
        box-shadow: 0 8px 30px rgba(102, 126, 234, 0.7);
      }
      .auth-float-btn:active {
        transform: translateY(0) scale(0.98);
      }
      .auth-float-btn.logged-in {
        background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
        box-shadow: 0 4px 20px rgba(17, 153, 142, 0.5);
      }

      /* 全屏遮罩 */
      .auth-overlay {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 9999;
        display: none;
        justify-content: center;
        align-items: center;
        overflow: hidden;
      }
      .auth-overlay.active {
        display: flex;
        animation: fadeIn 0.4s ease;
      }
      @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
      }

      /* 粒子背景画布 */
      .auth-particles-canvas {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 1;
      }

      /* 渐变背景层 */
      .auth-bg-gradient {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: linear-gradient(-45deg, #0f0c29, #302b63, #24243e, #1a1a2e);
        background-size: 400% 400%;
        animation: gradientShift 15s ease infinite;
        z-index: 0;
      }
      @keyframes gradientShift {
        0% { background-position: 0% 50%; }
        50% { background-position: 100% 50%; }
        100% { background-position: 0% 50%; }
      }

      /* 登录卡片容器 */
      .auth-card-container {
        position: relative;
        z-index: 10;
        width: 440px;
        max-width: 92%;
        animation: slideUp 0.5s cubic-bezier(0.4, 0, 0.2, 1);
      }
      @keyframes slideUp {
        from {
          opacity: 0;
          transform: translateY(40px) scale(0.95);
        }
        to {
          opacity: 1;
          transform: translateY(0) scale(1);
        }
      }

      /* 登录卡片 */
      .auth-card {
        background: rgba(255, 255, 255, 0.08);
        backdrop-filter: blur(20px);
        -webkit-backdrop-filter: blur(20px);
        border-radius: 24px;
        padding: 40px 36px;
        border: 1px solid rgba(255, 255, 255, 0.15);
        box-shadow:
          0 25px 50px -12px rgba(0, 0, 0, 0.5),
          inset 0 1px 0 rgba(255, 255, 255, 0.1);
      }

      /* Logo区域 */
      .auth-logo {
        text-align: center;
        margin-bottom: 30px;
      }
      .auth-logo-icon {
        font-size: 56px;
        display: block;
        margin-bottom: 12px;
        animation: float 3s ease-in-out infinite;
      }
      @keyframes float {
        0%, 100% { transform: translateY(0); }
        50% { transform: translateY(-8px); }
      }
      .auth-logo-title {
        font-size: 32px;
        font-weight: 800;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        margin: 0;
        letter-spacing: 2px;
      }
      .auth-logo-subtitle {
        color: rgba(255, 255, 255, 0.5);
        font-size: 13px;
        margin-top: 6px;
        letter-spacing: 1px;
      }

      /* 标签切换 */
      .auth-tabs {
        display: flex;
        background: rgba(255, 255, 255, 0.06);
        border-radius: 14px;
        padding: 5px;
        margin-bottom: 28px;
        position: relative;
      }
      .auth-tab {
        flex: 1;
        padding: 12px;
        border: none;
        background: transparent;
        color: rgba(255, 255, 255, 0.5);
        border-radius: 10px;
        cursor: pointer;
        font-size: 15px;
        font-weight: 600;
        transition: all 0.3s ease;
        position: relative;
        z-index: 2;
      }
      .auth-tab.active {
        color: white;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
      }

      /* 表单区域 */
      .auth-form {
        display: none;
      }
      .auth-form.active {
        display: block;
        animation: formFadeIn 0.3s ease;
      }
      @keyframes formFadeIn {
        from { opacity: 0; transform: translateX(10px); }
        to { opacity: 1; transform: translateX(0); }
      }

      /* 输入框组 */
      .auth-input-group {
        margin-bottom: 20px;
        position: relative;
      }
      .auth-input-label {
        display: block;
        color: rgba(255, 255, 255, 0.7);
        font-size: 13px;
        font-weight: 500;
        margin-bottom: 8px;
        letter-spacing: 0.5px;
      }
      .auth-input-wrapper {
        position: relative;
        display: flex;
        align-items: center;
      }
      .auth-input-icon {
        position: absolute;
        left: 16px;
        font-size: 18px;
        color: rgba(255, 255, 255, 0.4);
        transition: all 0.3s ease;
        pointer-events: none;
      }
      .auth-input {
        width: 100%;
        padding: 14px 16px 14px 48px;
        background: rgba(255, 255, 255, 0.06);
        border: 2px solid rgba(255, 255, 255, 0.1);
        border-radius: 12px;
        color: white;
        font-size: 15px;
        box-sizing: border-box;
        outline: none;
        transition: all 0.3s ease;
        font-family: inherit;
      }
      .auth-input::placeholder {
        color: rgba(255, 255, 255, 0.3);
      }
      .auth-input:focus {
        border-color: #667eea;
        background: rgba(255, 255, 255, 0.1);
        box-shadow: 0 0 0 4px rgba(102, 126, 234, 0.15);
      }
      .auth-input:focus + .auth-input-icon,
      .auth-input-wrapper:focus-within .auth-input-icon {
        color: #667eea;
      }
      .auth-input.error {
        border-color: #ef4444;
        animation: shake 0.4s ease;
      }
      @keyframes shake {
        0%, 100% { transform: translateX(0); }
        25% { transform: translateX(-5px); }
        75% { transform: translateX(5px); }
      }

      /* 验证码输入 */
      .auth-code-wrapper {
        display: flex;
        gap: 10px;
      }
      .auth-code-input {
        flex: 1;
      }
      .auth-code-btn {
        padding: 0 18px;
        background: rgba(102, 126, 234, 0.2);
        color: #a5b4fc;
        border: 2px solid rgba(102, 126, 234, 0.3);
        border-radius: 12px;
        cursor: pointer;
        font-size: 13px;
        font-weight: 600;
        white-space: nowrap;
        transition: all 0.3s ease;
        min-width: 110px;
      }
      .auth-code-btn:hover:not(:disabled) {
        background: rgba(102, 126, 234, 0.3);
        border-color: #667eea;
      }
      .auth-code-btn:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }

      /* 密码强度指示器 */
      .auth-password-strength {
        margin-top: 8px;
        height: 4px;
        background: rgba(255, 255, 255, 0.1);
        border-radius: 2px;
        overflow: hidden;
        display: none;
      }
      .auth-password-strength-bar {
        height: 100%;
        width: 0%;
        transition: all 0.3s ease;
        border-radius: 2px;
      }
      .auth-password-strength-text {
        font-size: 11px;
        color: rgba(255, 255, 255, 0.5);
        margin-top: 4px;
        display: none;
      }

      /* 提交按钮 */
      .auth-submit-btn {
        width: 100%;
        padding: 16px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border: none;
        border-radius: 14px;
        cursor: pointer;
        font-size: 16px;
        font-weight: 700;
        transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
        box-shadow: 0 4px 20px rgba(102, 126, 234, 0.4);
        letter-spacing: 1px;
        position: relative;
        overflow: hidden;
        margin-top: 8px;
      }
      .auth-submit-btn::before {
        content: '';
        position: absolute;
        top: 0;
        left: -100%;
        width: 100%;
        height: 100%;
        background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
        transition: left 0.5s ease;
      }
      .auth-submit-btn:hover::before {
        left: 100%;
      }
      .auth-submit-btn:hover {
        transform: translateY(-2px);
        box-shadow: 0 8px 30px rgba(102, 126, 234, 0.6);
      }
      .auth-submit-btn:active {
        transform: translateY(0);
      }
      .auth-submit-btn:disabled {
        opacity: 0.6;
        cursor: not-allowed;
        transform: none;
      }
      .auth-submit-btn.loading {
        pointer-events: none;
      }
      .auth-submit-btn.loading::after {
        content: '';
        position: absolute;
        top: 50%;
        right: 20px;
        width: 20px;
        height: 20px;
        margin-top: -10px;
        border: 2px solid rgba(255,255,255,0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }
      @keyframes spin {
        to { transform: rotate(360deg); }
      }

      /* 消息提示 */
      .auth-message {
        margin-top: 18px;
        padding: 12px 16px;
        border-radius: 10px;
        font-size: 13px;
        text-align: center;
        display: none;
        animation: messageIn 0.3s ease;
      }
      @keyframes messageIn {
        from { opacity: 0; transform: translateY(-5px); }
        to { opacity: 1; transform: translateY(0); }
      }
      .auth-message.success {
        display: block;
        background: rgba(34, 197, 94, 0.15);
        color: #4ade80;
        border: 1px solid rgba(34, 197, 94, 0.3);
      }
      .auth-message.error {
        display: block;
        background: rgba(239, 68, 68, 0.15);
        color: #f87171;
        border: 1px solid rgba(239, 68, 68, 0.3);
      }
      .auth-message.info {
        display: block;
        background: rgba(59, 130, 246, 0.15);
        color: #60a5fa;
        border: 1px solid rgba(59, 130, 246, 0.3);
      }

      /* 关闭按钮 */
      .auth-close-btn {
        position: absolute;
        top: 16px;
        right: 16px;
        width: 36px;
        height: 36px;
        background: rgba(255, 255, 255, 0.1);
        border: none;
        border-radius: 50%;
        color: rgba(255, 255, 255, 0.6);
        font-size: 20px;
        cursor: pointer;
        transition: all 0.3s ease;
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 20;
      }
      .auth-close-btn:hover {
        background: rgba(239, 68, 68, 0.3);
        color: white;
        transform: rotate(90deg);
      }

      /* 用户信息卡片 */
      .auth-user-card {
        text-align: center;
        padding: 20px 0;
      }
      .auth-user-avatar {
        width: 80px;
        height: 80px;
        border-radius: 50%;
        background: linear-gradient(135deg, #11998e 0%, #38ef7d 100%);
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 36px;
        margin: 0 auto 16px;
        box-shadow: 0 8px 25px rgba(17, 153, 142, 0.4);
        animation: pulse 2s ease infinite;
      }
      @keyframes pulse {
        0%, 100% { box-shadow: 0 8px 25px rgba(17, 153, 142, 0.4); }
        50% { box-shadow: 0 8px 35px rgba(17, 153, 142, 0.6); }
      }
      .auth-user-name {
        font-size: 24px;
        font-weight: 700;
        color: white;
        margin-bottom: 4px;
      }
      .auth-user-email {
        font-size: 13px;
        color: rgba(255, 255, 255, 0.5);
        margin-bottom: 24px;
      }
      .auth-user-stats {
        display: flex;
        justify-content: center;
        gap: 30px;
        margin-bottom: 24px;
      }
      .auth-user-stat {
        text-align: center;
      }
      .auth-user-stat-value {
        font-size: 20px;
        font-weight: 700;
        color: #667eea;
      }
      .auth-user-stat-label {
        font-size: 11px;
        color: rgba(255, 255, 255, 0.4);
        margin-top: 2px;
      }
      .auth-logout-btn {
        padding: 12px 32px;
        background: rgba(239, 68, 68, 0.2);
        color: #f87171;
        border: 2px solid rgba(239, 68, 68, 0.3);
        border-radius: 12px;
        cursor: pointer;
        font-size: 14px;
        font-weight: 600;
        transition: all 0.3s ease;
      }
      .auth-logout-btn:hover {
        background: rgba(239, 68, 68, 0.3);
        border-color: #ef4444;
        transform: translateY(-2px);
      }

      /* 底部提示 */
      .auth-footer {
        text-align: center;
        margin-top: 24px;
        font-size: 12px;
        color: rgba(255, 255, 255, 0.3);
      }
      .auth-footer a {
        color: rgba(255, 255, 255, 0.5);
        text-decoration: none;
        transition: color 0.3s;
      }
      .auth-footer a:hover {
        color: white;
      }
    `;
    document.head.appendChild(style);
  },

  // 创建浮动按钮
  createFloatingButton() {
    const btn = document.createElement('button');
    btn.id = 'auth-float-btn';
    btn.className = 'auth-float-btn';
    btn.innerHTML = '👤 登录 / 注册';
    btn.onclick = () => this.openAuth();
    document.body.appendChild(btn);

    if (this.currentUser) {
      btn.classList.add('logged-in');
      btn.innerHTML = `👤 ${this.currentUser.username}`;
    }
  },

  // 创建全屏登录遮罩
  createAuthOverlay() {
    const overlay = document.createElement('div');
    overlay.id = 'auth-overlay';
    overlay.className = 'auth-overlay';
    overlay.innerHTML = `
      <div class="auth-bg-gradient"></div>
      <canvas class="auth-particles-canvas" id="auth-particles-canvas"></canvas>
      <div class="auth-card-container">
        <div class="auth-card">
          <button class="auth-close-btn" id="auth-close-btn">×</button>

          <div class="auth-logo">
            <span class="auth-logo-icon">⚔️</span>
            <h1 class="auth-logo-title">永恒地牢</h1>
            <p class="auth-logo-subtitle">ETERNAL DUNGEON · 账户中心</p>
          </div>

          <!-- 登录/注册标签 -->
          <div class="auth-tabs" id="auth-tabs">
            <button class="auth-tab active" data-tab="login">登 录</button>
            <button class="auth-tab" data-tab="register">注 册</button>
          </div>

          <!-- 登录表单 -->
          <div class="auth-form active" id="auth-login-form">
            <div class="auth-input-group">
              <label class="auth-input-label">用户名 / 邮箱</label>
              <div class="auth-input-wrapper">
                <input type="text" class="auth-input" id="auth-login-username" placeholder="请输入用户名或邮箱" autocomplete="username">
                <span class="auth-input-icon">👤</span>
              </div>
            </div>

            <div class="auth-input-group">
              <label class="auth-input-label">密码</label>
              <div class="auth-input-wrapper">
                <input type="password" class="auth-input" id="auth-login-password" placeholder="请输入密码" autocomplete="current-password">
                <span class="auth-input-icon">🔒</span>
              </div>
            </div>

            <button class="auth-submit-btn" id="auth-login-submit">登 录</button>
          </div>

          <!-- 注册表单 -->
          <div class="auth-form" id="auth-register-form">
            <div class="auth-input-group">
              <label class="auth-input-label">用户名</label>
              <div class="auth-input-wrapper">
                <input type="text" class="auth-input" id="auth-register-username" placeholder="3-20位字母数字下划线" autocomplete="username" maxlength="20">
                <span class="auth-input-icon">🎮</span>
              </div>
            </div>

            <div class="auth-input-group">
              <label class="auth-input-label">邮箱地址</label>
              <div class="auth-input-wrapper">
                <input type="email" class="auth-input" id="auth-register-email" placeholder="your@email.com" autocomplete="email">
                <span class="auth-input-icon">📧</span>
              </div>
            </div>

            <div class="auth-input-group">
              <label class="auth-input-label">邮箱验证码</label>
              <div class="auth-code-wrapper">
                <div class="auth-input-wrapper auth-code-input">
                  <input type="text" class="auth-input" id="auth-register-code" placeholder="6位验证码" maxlength="6" style="letter-spacing: 4px;">
                  <span class="auth-input-icon">🔑</span>
                </div>
                <button class="auth-code-btn" id="auth-send-code-btn">获取验证码</button>
              </div>
            </div>

            <div class="auth-input-group">
              <label class="auth-input-label">激活密钥</label>
              <div class="auth-input-wrapper">
                <input type="text" class="auth-input" id="auth-register-key" placeholder="ED-XXXX-XXXX-XXXX-XXXX" style="letter-spacing: 1px; text-transform: uppercase;" maxlength="23">
                <span class="auth-input-icon">🎫</span>
              </div>
              <div id="auth-key-validation" style="font-size: 11px; margin-top: 6px; display: none;"></div>
            </div>

            <div class="auth-input-group">
              <label class="auth-input-label">设置密码</label>
              <div class="auth-input-wrapper">
                <input type="password" class="auth-input" id="auth-register-password" placeholder="至少6位密码" autocomplete="new-password">
                <span class="auth-input-icon">🛡️</span>
              </div>
              <div class="auth-password-strength" id="auth-password-strength">
                <div class="auth-password-strength-bar" id="auth-password-strength-bar"></div>
              </div>
              <div class="auth-password-strength-text" id="auth-password-strength-text"></div>
            </div>

            <button class="auth-submit-btn" id="auth-register-submit">创建账户</button>
          </div>

          <!-- 用户信息（登录后） -->
          <div class="auth-form" id="auth-user-info">
            <div class="auth-user-card">
              <div class="auth-user-avatar">👑</div>
              <div class="auth-user-name" id="auth-user-name"></div>
              <div class="auth-user-email" id="auth-user-email"></div>
              <div class="auth-user-stats">
                <div class="auth-user-stat">
                  <div class="auth-user-stat-value" id="auth-user-level">1</div>
                  <div class="auth-user-stat-label">等级</div>
                </div>
                <div class="auth-user-stat">
                  <div class="auth-user-stat-value" id="auth-user-gold">0</div>
                  <div class="auth-user-stat-label">金币</div>
                </div>
                <div class="auth-user-stat">
                  <div class="auth-user-stat-value" id="auth-user-playtime">0h</div>
                  <div class="auth-user-stat-label">游戏时长</div>
                </div>
              </div>
              <button class="auth-logout-btn" id="auth-logout-btn">退出登录</button>
            </div>
          </div>

          <!-- 消息提示 -->
          <div class="auth-message" id="auth-message"></div>

          <div class="auth-footer">
            登录即表示同意 <a href="#">用户协议</a> 和 <a href="#">隐私政策</a>
          </div>
        </div>
      </div>
    `;
    document.body.appendChild(overlay);

    // 绑定事件
    this.bindEvents();
  },

  // 创建粒子背景
  createParticleBackground() {
    const canvas = document.getElementById('auth-particles-canvas');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');
    let animationId = null;

    const resize = () => {
      canvas.width = canvas.offsetWidth;
      canvas.height = canvas.offsetHeight;
    };
    resize();
    window.addEventListener('resize', resize);

    // 创建粒子
    this.particles = [];
    const particleCount = 60;
    for (let i = 0; i < particleCount; i++) {
      this.particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        size: Math.random() * 3 + 1,
        speedX: (Math.random() - 0.5) * 0.5,
        speedY: (Math.random() - 0.5) * 0.5,
        opacity: Math.random() * 0.5 + 0.2,
        hue: Math.random() * 60 + 220 // 蓝紫色调
      });
    }

    const animate = () => {
      ctx.clearRect(0, 0, canvas.width, canvas.height);

      // 绘制连线
      for (let i = 0; i < this.particles.length; i++) {
        for (let j = i + 1; j < this.particles.length; j++) {
          const dx = this.particles[i].x - this.particles[j].x;
          const dy = this.particles[i].y - this.particles[j].y;
          const dist = Math.sqrt(dx * dx + dy * dy);

          if (dist < 120) {
            ctx.beginPath();
            ctx.moveTo(this.particles[i].x, this.particles[i].y);
            ctx.lineTo(this.particles[j].x, this.particles[j].y);
            ctx.strokeStyle = `rgba(102, 126, 234, ${(1 - dist / 120) * 0.15})`;
            ctx.lineWidth = 1;
            ctx.stroke();
          }
        }
      }

      // 绘制粒子
      this.particles.forEach(p => {
        p.x += p.speedX;
        p.y += p.speedY;

        if (p.x < 0 || p.x > canvas.width) p.speedX *= -1;
        if (p.y < 0 || p.y > canvas.height) p.speedY *= -1;

        ctx.beginPath();
        ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        ctx.fillStyle = `hsla(${p.hue}, 70%, 70%, ${p.opacity})`;
        ctx.fill();
      });

      animationId = requestAnimationFrame(animate);
    };

    // 只在打开时动画
    this.startParticles = () => {
      if (!animationId) animate();
    };
    this.stopParticles = () => {
      if (animationId) {
        cancelAnimationFrame(animationId);
        animationId = null;
      }
    };
  },

  // 绑定事件
  bindEvents() {
    const overlay = document.getElementById('auth-overlay');
    const closeBtn = document.getElementById('auth-close-btn');
    const tabs = document.querySelectorAll('.auth-tab');

    // 关闭
    closeBtn.onclick = () => this.closeAuth();
    overlay.onclick = (e) => {
      if (e.target === overlay) this.closeAuth();
    };

    // ESC关闭
    document.addEventListener('keydown', (e) => {
      if (e.key === 'Escape' && this.isOpen) this.closeAuth();
    });

    // 标签切换
    tabs.forEach(tab => {
      tab.onclick = () => {
        const tabName = tab.dataset.tab;
        this.switchTab(tabName);
      };
    });

    // 登录提交
    document.getElementById('auth-login-submit').onclick = () => this.handleLogin();
    document.getElementById('auth-login-password').onkeypress = (e) => {
      if (e.key === 'Enter') this.handleLogin();
    };

    // 注册提交
    document.getElementById('auth-register-submit').onclick = () => this.handleRegister();

    // 发送验证码
    document.getElementById('auth-send-code-btn').onclick = () => this.handleSendCode();

    // 退出登录
    document.getElementById('auth-logout-btn').onclick = () => this.handleLogout();

    // 密码强度检测
    const passwordInput = document.getElementById('auth-register-password');
    passwordInput.oninput = () => this.checkPasswordStrength(passwordInput.value);

    // 激活密钥实时验证
    const keyInput = document.getElementById('auth-register-key');
    let keyValidateTimeout = null;
    keyInput.oninput = () => {
      keyInput.value = keyInput.value.toUpperCase();
      clearTimeout(keyValidateTimeout);
      keyValidateTimeout = setTimeout(() => {
        this.validateActivationKey(keyInput.value.trim());
      }, 500);
    };
  },

  // 切换标签
  switchTab(tabName) {
    this.currentTab = tabName;

    document.querySelectorAll('.auth-tab').forEach(tab => {
      tab.classList.toggle('active', tab.dataset.tab === tabName);
    });

    document.getElementById('auth-login-form').classList.toggle('active', tabName === 'login');
    document.getElementById('auth-register-form').classList.toggle('active', tabName === 'register');

    this.hideMessage();
  },

  // 打开登录界面
  openAuth(tab = null) {
    const overlay = document.getElementById('auth-overlay');
    overlay.classList.add('active');
    this.isOpen = true;
    this.startParticles();

    if (tab) {
      this.switchTab(tab);
    }

    // 如果已登录，显示用户信息
    if (this.currentUser) {
      this.showUserInfo();
    } else {
      this.showLoginForm();
    }
  },

  // 关闭登录界面
  closeAuth() {
    const overlay = document.getElementById('auth-overlay');
    overlay.classList.remove('active');
    this.isOpen = false;
    this.stopParticles();
  },

  // 显示登录表单
  showLoginForm() {
    document.getElementById('auth-tabs').style.display = 'flex';
    document.getElementById('auth-login-form').classList.add('active');
    document.getElementById('auth-register-form').classList.remove('active');
    document.getElementById('auth-user-info').classList.remove('active');
  },

  // 显示用户信息
  showUserInfo() {
    document.getElementById('auth-tabs').style.display = 'none';
    document.getElementById('auth-login-form').classList.remove('active');
    document.getElementById('auth-register-form').classList.remove('active');
    document.getElementById('auth-user-info').classList.add('active');

    document.getElementById('auth-user-name').textContent = this.currentUser.username;
    document.getElementById('auth-user-email').textContent = this.currentUser.email;

    // 更新浮动按钮
    const btn = document.getElementById('auth-float-btn');
    btn.classList.add('logged-in');
    btn.innerHTML = `👤 ${this.currentUser.username}`;
  },

  // 显示消息
  showMessage(message, type = 'info') {
    const msgEl = document.getElementById('auth-message');
    msgEl.className = `auth-message ${type}`;
    msgEl.textContent = message;
  },

  // 隐藏消息
  hideMessage() {
    const msgEl = document.getElementById('auth-message');
    msgEl.className = 'auth-message';
    msgEl.textContent = '';
  },

  // 密码强度检测
  checkPasswordStrength(password) {
    const strengthBar = document.getElementById('auth-password-strength');
    const strengthBarInner = document.getElementById('auth-password-strength-bar');
    const strengthText = document.getElementById('auth-password-strength-text');

    if (!password) {
      strengthBar.style.display = 'none';
      strengthText.style.display = 'none';
      return;
    }

    strengthBar.style.display = 'block';
    strengthText.style.display = 'block';

    let strength = 0;
    if (password.length >= 6) strength++;
    if (password.length >= 10) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/[0-9]/.test(password)) strength++;
    if (/[^A-Za-z0-9]/.test(password)) strength++;

    const colors = ['#ef4444', '#f97316', '#eab308', '#22c55e', '#10b981'];
    const texts = ['非常弱', '弱', '中等', '强', '非常强'];
    const widths = ['20%', '40%', '60%', '80%', '100%'];

    const index = Math.min(strength - 1, 4);
    strengthBarInner.style.width = widths[index] || '10%';
    strengthBarInner.style.background = colors[index] || '#ef4444';
    strengthText.textContent = `密码强度：${texts[index] || '非常弱'}`;
    strengthText.style.color = colors[index] || '#ef4444';
  },

  // 设置按钮加载状态
  setButtonLoading(btnId, loading, text = null) {
    const btn = document.getElementById(btnId);
    if (loading) {
      btn.classList.add('loading');
      btn.disabled = true;
      if (text) btn.textContent = text;
    } else {
      btn.classList.remove('loading');
      btn.disabled = false;
      if (text) btn.textContent = text;
    }
  },

  // 处理登录
  async handleLogin() {
    const username = document.getElementById('auth-login-username').value.trim();
    const password = document.getElementById('auth-login-password').value;

    if (!username) {
      this.showMessage('请输入用户名或邮箱', 'error');
      document.getElementById('auth-login-username').classList.add('error');
      setTimeout(() => document.getElementById('auth-login-username').classList.remove('error'), 500);
      return;
    }
    if (!password) {
      this.showMessage('请输入密码', 'error');
      return;
    }

    this.setButtonLoading('auth-login-submit', true, '登录中...');
    this.showMessage('正在验证身份...', 'info');

    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
      });

      const result = await response.json();

      if (result.success) {
        this.authToken = result.token;
        this.currentUser = {
          id: result.user_id,
          username: result.username,
          email: result.email
        };

        localStorage.setItem('eternal_dungeon_token', this.authToken);
        localStorage.setItem('eternal_dungeon_user', JSON.stringify(this.currentUser));

        this.showMessage('登录成功！欢迎回来', 'success');

        setTimeout(() => {
          this.showUserInfo();
          this.closeAuth();
        }, 1000);
      } else {
        this.showMessage(result.message || '登录失败', 'error');
      }
    } catch (error) {
      this.showMessage('网络错误，请重试', 'error');
      console.error('登录错误:', error);
    } finally {
      this.setButtonLoading('auth-login-submit', false, '登 录');
    }
  },

  // 处理注册
  async handleRegister() {
    const username = document.getElementById('auth-register-username').value.trim();
    const email = document.getElementById('auth-register-email').value.trim();
    const code = document.getElementById('auth-register-code').value.trim();
    const activationKey = document.getElementById('auth-register-key').value.trim();
    const password = document.getElementById('auth-register-password').value;

    if (!username) {
      this.showMessage('请输入用户名', 'error');
      return;
    }
    if (!/^[a-zA-Z0-9_]{3,20}$/.test(username)) {
      this.showMessage('用户名格式不正确（3-20位字母数字下划线）', 'error');
      return;
    }
    if (!email) {
      this.showMessage('请输入邮箱', 'error');
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.showMessage('邮箱格式不正确', 'error');
      return;
    }
    if (!code) {
      this.showMessage('请输入验证码', 'error');
      return;
    }
    if (!activationKey) {
      this.showMessage('请输入激活密钥', 'error');
      document.getElementById('auth-register-key').classList.add('error');
      setTimeout(() => document.getElementById('auth-register-key').classList.remove('error'), 500);
      return;
    }
    if (!password) {
      this.showMessage('请设置密码', 'error');
      return;
    }
    if (password.length < 6) {
      this.showMessage('密码至少需要6位', 'error');
      return;
    }

    this.setButtonLoading('auth-register-submit', true, '创建中...');
    this.showMessage('正在创建账户...', 'info');

    try {
      const response = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, email, password, verification_code: code, activation_key: activationKey })
      });

      const result = await response.json();

      if (result.success) {
        this.showMessage(`注册成功！会员类型：${result.membership_type || 'normal'}`, 'success');

        // 自动登录
        setTimeout(async () => {
          try {
            const loginResponse = await fetch('/api/auth/login', {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify({ username, password })
            });
            const loginResult = await loginResponse.json();

            if (loginResult.success) {
              this.authToken = loginResult.token;
              this.currentUser = {
                id: loginResult.user_id,
                username: loginResult.username,
                email: loginResult.email
              };

              localStorage.setItem('eternal_dungeon_token', this.authToken);
              localStorage.setItem('eternal_dungeon_user', JSON.stringify(this.currentUser));

              this.showUserInfo();
              this.closeAuth();
            }
          } catch (e) {
            console.error('自动登录失败:', e);
          }
        }, 1000);
      } else {
        this.showMessage(result.message || '注册失败', 'error');
      }
    } catch (error) {
      this.showMessage('网络错误，请重试', 'error');
      console.error('注册错误:', error);
    } finally {
      this.setButtonLoading('auth-register-submit', false, '创建账户');
    }
  },

  // 验证激活密钥
  async validateActivationKey(key) {
    const validationEl = document.getElementById('auth-key-validation');
    if (!key) {
      validationEl.style.display = 'none';
      return;
    }

    try {
      const response = await fetch('/api/keys/validate', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ key })
      });
      const result = await response.json();

      validationEl.style.display = 'block';
      if (result.valid) {
        const typeNames = { trial: '试用版', normal: '普通版', premium: '高级版', lifetime: '永久版' };
        const typeName = typeNames[result.key_type] || result.key_type;
        const duration = result.duration_days > 0 ? `${result.duration_days}天` : '永久';
        validationEl.innerHTML = `<span style="color: #4ade80;">✓ 密钥有效（${typeName} · ${duration}）</span>`;
      } else {
        validationEl.innerHTML = `<span style="color: #f87171;">✗ ${result.message}</span>`;
      }
    } catch (error) {
      console.error('验证密钥错误:', error);
    }
  },

  // 处理发送验证码
  async handleSendCode() {
    const email = document.getElementById('auth-register-email').value.trim();

    if (!email) {
      this.showMessage('请先输入邮箱地址', 'error');
      return;
    }
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      this.showMessage('邮箱格式不正确', 'error');
      return;
    }

    if (this.countdown > 0) return;

    const btn = document.getElementById('auth-send-code-btn');
    btn.disabled = true;
    this.showMessage('正在发送验证码...', 'info');

    try {
      const response = await fetch('/api/auth/send-code', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, purpose: 'register' })
      });

      const result = await response.json();

      if (result.success) {
        this.showMessage(`验证码已发送至 ${email}${result.code ? `（开发模式：${result.code}）` : ''}`, 'success');
        // 开发模式：自动填充验证码并弹出提醒
        if (result.code) {
          const codeInput = document.getElementById('auth-register-code');
          if (codeInput) {
            codeInput.value = result.code;
          }
          // 延迟弹出，让用户先看到页面变化
          setTimeout(() => {
            alert(`【开发模式】你的验证码是：${result.code}\n\n已自动填入验证码输入框，直接点注册即可！`);
          }, 300);
        }
        this.startCountdown();
      } else {
        this.showMessage(result.message || '发送失败', 'error');
        btn.disabled = false;
      }
    } catch (error) {
      this.showMessage('网络错误，请重试', 'error');
      btn.disabled = false;
      console.error('发送验证码错误:', error);
    }
  },

  // 倒计时
  startCountdown() {
    this.countdown = 60;
    const btn = document.getElementById('auth-send-code-btn');

    this.countdownTimer = setInterval(() => {
      this.countdown--;
      btn.textContent = `${this.countdown}秒后重发`;

      if (this.countdown <= 0) {
        clearInterval(this.countdownTimer);
        btn.disabled = false;
        btn.textContent = '获取验证码';
      }
    }, 1000);
  },

  // 处理退出登录
  async handleLogout() {
    try {
      await fetch('/api/auth/logout', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.authToken}`
        }
      });
    } catch (error) {
      console.error('登出错误:', error);
    }

    this.authToken = null;
    this.currentUser = null;
    localStorage.removeItem('eternal_dungeon_token');
    localStorage.removeItem('eternal_dungeon_user');

    const btn = document.getElementById('auth-float-btn');
    btn.classList.remove('logged-in');
    btn.innerHTML = '👤 登录 / 注册';

    this.showMessage('已退出登录', 'info');
    setTimeout(() => {
      this.showLoginForm();
    }, 500);
  },

  // 检查登录状态
  async checkLoginStatus() {
    if (!this.authToken) return;

    try {
      const response = await fetch('/api/auth/status', {
        headers: {
          'Authorization': `Bearer ${this.authToken}`
        }
      });

      const result = await response.json();

      if (result.logged_in && result.user) {
        this.currentUser = result.user;
        const btn = document.getElementById('auth-float-btn');
        btn.classList.add('logged-in');
        btn.innerHTML = `👤 ${this.currentUser.username}`;
        console.log('[AuthSystem] 已登录:', this.currentUser.username);
      } else {
        this.authToken = null;
        this.currentUser = null;
        localStorage.removeItem('eternal_dungeon_token');
        localStorage.removeItem('eternal_dungeon_user');
      }
    } catch (error) {
      console.error('检查登录状态错误:', error);
    }
  },

  // 保存游戏数据
  async saveGameData(gameData) {
    if (!this.authToken || !this.currentUser) return false;

    try {
      const response = await fetch('/api/game/save', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${this.authToken}`
        },
        body: JSON.stringify({ game_data: gameData })
      });

      const result = await response.json();
      return result.success;
    } catch (error) {
      console.error('保存游戏数据错误:', error);
      return false;
    }
  },

  // 获取游戏数据
  async getGameData() {
    if (!this.authToken || !this.currentUser) return null;

    try {
      const response = await fetch('/api/game/data', {
        headers: {
          'Authorization': `Bearer ${this.authToken}`
        }
      });

      const result = await response.json();
      return result.success ? result.game_data : null;
    } catch (error) {
      console.error('获取游戏数据错误:', error);
      return null;
    }
  }
};

// 页面加载完成后初始化
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', () => AuthSystem.init());
} else {
  AuthSystem.init();
}
