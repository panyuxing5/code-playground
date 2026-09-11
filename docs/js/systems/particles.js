// ==================== 永恒地牢 - 粒子系统 ====================
// 管理所有粒子效果：伤害数字、爆炸、技能特效、环境粒子

const ParticleSystem = {
  particles: [],
  maxParticles: 500,

  // 初始化
  init() {
    this.particles = [];
    console.log('[ParticleSystem] 粒子系统初始化完成');
  },

  // 更新所有粒子
  update(dt) {
    for (let i = this.particles.length - 1; i >= 0; i--) {
      const p = this.particles[i];
      p.life -= dt;
      if (p.life <= 0) {
        this.particles.splice(i, 1);
        continue;
      }
      // 物理更新
      p.x += p.vx * dt * 60;
      p.y += p.vy * dt * 60;
      if (p.gravity) {
        p.vy += p.gravity * dt * 60;
      }
      if (p.friction) {
        p.vx *= p.friction;
        p.vy *= p.friction;
      }
      // 大小变化
      if (p.sizeChange) {
        p.size += p.sizeChange * dt;
      }
      // 颜色变化
      if (p.colorChange) {
        p.color = Utils.lerpColor(p.color, p.targetColor, dt * 2);
      }
    }
  },

  // 渲染所有粒子
  render(ctx, camera) {
    for (const p of this.particles) {
      const screenX = p.x - camera.x;
      const screenY = p.y - camera.y;
      const alpha = p.life / p.maxLife;

      ctx.globalAlpha = alpha * (p.alpha || 1);

      if (p.text) {
        // 文字粒子
        ctx.fillStyle = p.color;
        ctx.font = `bold ${p.size}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(p.text, screenX, screenY);
      } else if (p.shape === 'circle') {
        ctx.fillStyle = p.color;
        ctx.beginPath();
        ctx.arc(screenX, screenY, Math.max(0.1, p.size * alpha), 0, Math.PI * 2);
        ctx.fill();
      } else if (p.shape === 'square') {
        ctx.fillStyle = p.color;
        ctx.fillRect(screenX - p.size / 2, screenY - p.size / 2, p.size * alpha, p.size * alpha);
      } else if (p.shape === 'star') {
        this.drawStar(ctx, screenX, screenY, p.size * alpha, p.points || 5);
      } else if (p.shape === 'ring') {
        ctx.strokeStyle = p.color;
        ctx.lineWidth = p.lineWidth || 2;
        ctx.beginPath();
        ctx.arc(screenX, screenY, p.size * (1 - alpha) + p.size * 0.5, 0, Math.PI * 2);
        ctx.stroke();
      } else if (p.shape === 'line') {
        ctx.strokeStyle = p.color;
        ctx.lineWidth = p.lineWidth || 2;
        ctx.beginPath();
        ctx.moveTo(screenX, screenY);
        ctx.lineTo(screenX + p.vx * 3, screenY + p.vy * 3);
        ctx.stroke();
      } else {
        // 默认圆形
        ctx.fillStyle = p.color;
        ctx.beginPath();
        ctx.arc(screenX, screenY, Math.max(0.1, p.size * alpha), 0, Math.PI * 2);
        ctx.fill();
      }
    }
    ctx.globalAlpha = 1;
  },

  // 画星形
  drawStar(ctx, x, y, size, points) {
    ctx.beginPath();
    for (let i = 0; i < points * 2; i++) {
      const radius = i % 2 === 0 ? size : size / 2;
      const angle = (i * Math.PI) / points - Math.PI / 2;
      const px = x + Math.cos(angle) * radius;
      const py = y + Math.sin(angle) * radius;
      if (i === 0) ctx.moveTo(px, py);
      else ctx.lineTo(px, py);
    }
    ctx.closePath();
    ctx.fill();
  },

  // 添加粒子
  add(particle) {
    if (this.particles.length >= this.maxParticles) {
      this.particles.shift();
    }
    particle.maxLife = particle.life;
    this.particles.push(particle);
  },

  // ==================== 特效方法 ====================

  // 伤害数字
  damageNumber(x, y, damage, isCrit = false, isHeal = false, isMiss = false) {
    let color = '#fff';
    let size = 16;
    let text = damage;

    if (isMiss) {
      text = '闪避';
      color = '#888';
      size = 14;
    } else if (isHeal) {
      text = '+' + damage;
      color = '#2ecc71';
      size = 18;
    } else if (isCrit) {
      text = damage + '!';
      color = '#f1c40f';
      size = 24;
    }

    this.add({
      x: x + (Math.random() - 0.5) * 20,
      y: y - 20,
      vx: (Math.random() - 0.5) * 2,
      vy: -3,
      gravity: 0.1,
      color,
      size,
      life: 1.0,
      text,
      shape: 'text'
    });
  },

  // 爆炸效果
  explosion(x, y, color = '#e67e22', radius = 50, count = 20) {
    for (let i = 0; i < count; i++) {
      const angle = (i / count) * Math.PI * 2;
      const speed = 2 + Math.random() * 4;
      this.add({
        x, y,
        vx: Math.cos(angle) * speed,
        vy: Math.sin(angle) * speed,
        friction: 0.95,
        color,
        size: 4 + Math.random() * 6,
        life: 0.5 + Math.random() * 0.5,
        shape: 'circle'
      });
    }
    // 冲击波
    this.add({
      x, y,
      vx: 0, vy: 0,
      color,
      size: radius,
      life: 0.3,
      shape: 'ring',
      lineWidth: 4
    });
  },

  // 血液效果
  blood(x, y, count = 10) {
    for (let i = 0; i < count; i++) {
      this.add({
        x, y,
        vx: (Math.random() - 0.5) * 6,
        vy: -Math.random() * 4 - 1,
        gravity: 0.3,
        color: '#c0392b',
        size: 3 + Math.random() * 4,
        life: 0.5 + Math.random() * 0.5,
        shape: 'circle'
      });
    }
  },

  // 治疗效果
  heal(x, y, count = 8) {
    for (let i = 0; i < count; i++) {
      this.add({
        x: x + (Math.random() - 0.5) * 30,
        y: y + 20,
        vx: (Math.random() - 0.5) * 1,
        vy: -2 - Math.random() * 2,
        color: '#2ecc71',
        size: 4 + Math.random() * 4,
        life: 0.8,
        shape: 'star',
        points: 4
      });
    }
  },

  // 魔法效果
  magic(x, y, color = '#9b59b6', count = 12) {
    for (let i = 0; i < count; i++) {
      const angle = Math.random() * Math.PI * 2;
      const radius = Math.random() * 30;
      this.add({
        x: x + Math.cos(angle) * radius,
        y: y + Math.sin(angle) * radius,
        vx: Math.cos(angle) * 1,
        vy: Math.sin(angle) * 1,
        color,
        size: 3 + Math.random() * 3,
        life: 0.6 + Math.random() * 0.4,
        shape: 'circle'
      });
    }
  },

  // 烟雾效果
  smoke(x, y, count = 15) {
    for (let i = 0; i < count; i++) {
      this.add({
        x: x + (Math.random() - 0.5) * 20,
        y: y + (Math.random() - 0.5) * 20,
        vx: (Math.random() - 0.5) * 1,
        vy: -1 - Math.random(),
        color: '#7f8c8d',
        size: 8 + Math.random() * 12,
        sizeChange: 5,
        life: 1.5 + Math.random(),
        shape: 'circle',
        alpha: 0.5
      });
    }
  },

  // 火焰效果
  fire(x, y, count = 10) {
    for (let i = 0; i < count; i++) {
      this.add({
        x: x + (Math.random() - 0.5) * 15,
        y: y + (Math.random() - 0.5) * 15,
        vx: (Math.random() - 0.5) * 1,
        vy: -2 - Math.random() * 2,
        color: Utils.randomChoice(['#e67e22', '#f1c40f', '#e74c3c']),
        size: 4 + Math.random() * 6,
        sizeChange: -3,
        life: 0.4 + Math.random() * 0.3,
        shape: 'circle'
      });
    }
  },

  // 冰霜效果
  ice(x, y, count = 10) {
    for (let i = 0; i < count; i++) {
      this.add({
        x: x + (Math.random() - 0.5) * 30,
        y: y + (Math.random() - 0.5) * 30,
        vx: (Math.random() - 0.5) * 2,
        vy: (Math.random() - 0.5) * 2,
        color: '#3498db',
        size: 3 + Math.random() * 4,
        life: 0.6 + Math.random() * 0.4,
        shape: 'star',
        points: 6
      });
    }
  },

  // 闪电效果
  lightning(x1, y1, x2, y2, color = '#f1c40f') {
    const segments = 8;
    for (let i = 0; i < segments; i++) {
      const t = i / segments;
      const x = Utils.lerp(x1, x2, t) + (Math.random() - 0.5) * 20;
      const y = Utils.lerp(y1, y2, t) + (Math.random() - 0.5) * 20;
      this.add({
        x, y,
        vx: 0, vy: 0,
        color,
        size: 4,
        life: 0.2,
        shape: 'circle'
      });
    }
  },

  // 传送效果
  teleport(x, y, color = '#9b59b6') {
    for (let i = 0; i < 15; i++) {
      const angle = (i / 15) * Math.PI * 2;
      this.add({
        x: x + Math.cos(angle) * 30,
        y: y + Math.sin(angle) * 30,
        vx: -Math.cos(angle) * 3,
        vy: -Math.sin(angle) * 3,
        color,
        size: 5,
        life: 0.5,
        shape: 'circle'
      });
    }
  },

  // 等级提升效果
  levelUp(x, y) {
    for (let i = 0; i < 20; i++) {
      const angle = (i / 20) * Math.PI * 2;
      this.add({
        x, y,
        vx: Math.cos(angle) * 4,
        vy: Math.sin(angle) * 4 - 2,
        gravity: 0.1,
        color: Utils.randomChoice(['#f1c40f', '#fff', '#9b59b6']),
        size: 6,
        life: 1.0,
        shape: 'star',
        points: 5
      });
    }
    this.add({
      x, y,
      vx: 0, vy: 0,
      color: '#f1c40f',
      size: 80,
      life: 0.5,
      shape: 'ring',
      lineWidth: 5
    });
  },

  // 物品拾取效果
  itemPickup(x, y, color = '#f1c40f') {
    for (let i = 0; i < 8; i++) {
      this.add({
        x, y,
        vx: (Math.random() - 0.5) * 3,
        vy: -2 - Math.random() * 2,
        gravity: 0.15,
        color,
        size: 4,
        life: 0.6,
        shape: 'star',
        points: 4
      });
    }
  },

  // 足迹效果
  footstep(x, y, direction = 0) {
    this.add({
      x: x + (Math.random() - 0.5) * 5,
      y: y + 10,
      vx: 0, vy: 0,
      color: 'rgba(100, 80, 60, 0.3)',
      size: 6,
      life: 1.0,
      shape: 'circle',
      alpha: 0.3
    });
  },

  // 环境粒子（灰尘、火把等）
  ambient(x, y, type = 'dust') {
    if (type === 'dust') {
      this.add({
        x: x + (Math.random() - 0.5) * 100,
        y: y + (Math.random() - 0.5) * 100,
        vx: (Math.random() - 0.5) * 0.5,
        vy: -0.3 - Math.random() * 0.3,
        color: 'rgba(200, 180, 150, 0.2)',
        size: 2 + Math.random() * 2,
        life: 3 + Math.random() * 2,
        shape: 'circle',
        alpha: 0.3
      });
    } else if (type === 'torch') {
      this.add({
        x: x + (Math.random() - 0.5) * 8,
        y: y - 10,
        vx: (Math.random() - 0.5) * 0.5,
        vy: -1 - Math.random(),
        color: Utils.randomChoice(['#e67e22', '#f1c40f']),
        size: 3 + Math.random() * 3,
        sizeChange: -2,
        life: 0.5 + Math.random() * 0.3,
        shape: 'circle'
      });
    } else if (type === 'water') {
      this.add({
        x: x + (Math.random() - 0.5) * 50,
        y: y,
        vx: (Math.random() - 0.5) * 1,
        vy: -1 - Math.random(),
        gravity: 0.2,
        color: '#3498db',
        size: 2 + Math.random() * 2,
        life: 1.0,
        shape: 'circle'
      });
    }
  },

  // 清除所有粒子
  clear() {
    this.particles = [];
  },

  // 获取粒子数量
  get count() {
    return this.particles.length;
  }
};

window.ParticleSystem = ParticleSystem;
