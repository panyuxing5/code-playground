// ==================== 永恒地牢 - 渲染引擎 ====================
// 负责地图、实体、特效、UI的Canvas渲染

const Renderer = {
  canvas: null,
  ctx: null,
  width: 1280,
  height: 800,
  camera: { x: 0, y: 0 },
  // 光照系统
  lights: [],
  ambientLight: 0.3,
  // 屏幕震动
  shake: { intensity: 0, duration: 0, time: 0 },
  // 屏幕特效
  flash: { color: '#fff', alpha: 0, duration: 0 },
  vignette: true,
  // 初始化
  init(canvas) {
    this.canvas = canvas;
    this.ctx = canvas.getContext('2d');
    this.width = canvas.width;
    this.height = canvas.height;
    console.log('[Renderer] 渲染引擎初始化完成');
  },
  // 清屏
  clear(color = '#08080f') {
    this.ctx.fillStyle = color;
    this.ctx.fillRect(0, 0, this.width, this.height);
  },
  // 开始帧（应用屏幕震动）
  beginFrame() {
    this.ctx.save();
    if (this.shake.time > 0) {
      const intensity = this.shake.intensity * (this.shake.time / this.shake.duration);
      this.ctx.translate(
        (Math.random() - 0.5) * intensity * 2,
        (Math.random() - 0.5) * intensity * 2
      );
    }
  },
  // 结束帧
  endFrame() {
    this.ctx.restore();
    // 屏幕闪光
    if (this.flash.alpha > 0) {
      this.ctx.fillStyle = this.flash.color;
      this.ctx.globalAlpha = this.flash.alpha;
      this.ctx.fillRect(0, 0, this.width, this.height);
      this.ctx.globalAlpha = 1;
    }
    // 暗角效果
    if (this.vignette) {
      this.drawVignette();
    }
  },
  // 更新（每帧调用）
  update(dt) {
    if (this.shake.time > 0) {
      this.shake.time -= dt;
      if (this.shake.time <= 0) this.shake.time = 0;
    }
    if (this.flash.alpha > 0) {
      this.flash.alpha -= dt / this.flash.duration;
      if (this.flash.alpha < 0) this.flash.alpha = 0;
    }
  },
  // 屏幕震动
  shakeScreen(intensity, duration) {
    this.shake.intensity = intensity;
    this.shake.duration = duration;
    this.shake.time = duration;
  },
  // 屏幕闪光
  flashScreen(color = '#fff', alpha = 0.5, duration = 0.1) {
    this.flash.color = color;
    this.flash.alpha = alpha;
    this.flash.duration = duration;
  },
  // 暗角效果
  drawVignette() {
    const gradient = this.ctx.createRadialGradient(
      this.width / 2, this.height / 2, this.height * 0.3,
      this.width / 2, this.height / 2, this.height * 0.8
    );
    gradient.addColorStop(0, 'rgba(0,0,0,0)');
    gradient.addColorStop(1, 'rgba(0,0,0,0.6)');
    this.ctx.fillStyle = gradient;
    this.ctx.fillRect(0, 0, this.width, this.height);
  },
  // 设置相机位置
  setCamera(x, y) {
    this.camera.x = x - this.width / 2;
    this.camera.y = y - this.height / 2;
  },
  // 世界坐标转屏幕坐标
  worldToScreen(x, y) {
    return {
      x: x - this.camera.x,
      y: y - this.camera.y
    };
  },
  // 屏幕坐标转世界坐标
  screenToWorld(x, y) {
    return {
      x: x + this.camera.x,
      y: y + this.camera.y
    };
  },
  // ==================== 地图渲染 ====================
  // 渲染地图
  renderMap(map, exploredTiles, playerX, playerY, viewRadius = 350) {
    const ctx = this.ctx;
    const tileSize = map.tileSize || 32;
    const startX = Math.max(0, Math.floor(this.camera.x / tileSize));
    const startY = Math.max(0, Math.floor(this.camera.y / tileSize));
    const endX = Math.min(map.width, Math.ceil((this.camera.x + this.width) / tileSize) + 1);
    const endY = Math.min(map.height, Math.ceil((this.camera.y + this.height) / tileSize) + 1);

    for (let y = startY; y < endY; y++) {
      for (let x = startX; x < endX; x++) {
        const tile = map.tiles[y][x];
        const screenX = x * tileSize - this.camera.x;
        const screenY = y * tileSize - this.camera.y;
        const tileCenterX = x * tileSize + tileSize / 2;
        const tileCenterY = y * tileSize + tileSize / 2;
        const dist = Utils.distance(tileCenterX, tileCenterY, playerX, playerY);

        // 战争迷雾
        if (!exploredTiles[y][x]) continue;
        const inView = dist < viewRadius;
        const alpha = inView ? 1 : 0.4;

        ctx.globalAlpha = alpha;

        // 根据地块类型渲染
        this.renderTile(tile, screenX, screenY, tileSize, x, y);

        ctx.globalAlpha = 1;

        // 视野外变暗
        if (!inView && exploredTiles[y][x]) {
          ctx.fillStyle = 'rgba(0, 0, 0, 0.5)';
          ctx.fillRect(screenX, screenY, tileSize, tileSize);
        }
      }
    }
  },
  // 渲染单个地块
  renderTile(tile, x, y, size, tileX, tileY) {
    const ctx = this.ctx;
    switch (tile.type) {
      case 'wall':
        // 墙壁
        ctx.fillStyle = '#2a2538';
        ctx.fillRect(x, y, size, size);
        // 墙壁纹理
        ctx.fillStyle = '#1f1a2e';
        ctx.fillRect(x, y, size, 4);
        ctx.fillRect(x, y + size - 4, size, 4);
        // 砖块纹理
        if ((tileX + tileY) % 2 === 0) {
          ctx.fillStyle = 'rgba(255,255,255,0.02)';
          ctx.fillRect(x + 2, y + 6, size - 4, size - 12);
        }
        break;
      case 'floor':
        // 地板
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        // 地板纹理
        ctx.strokeStyle = 'rgba(120, 80, 200, 0.08)';
        ctx.strokeRect(x, y, size, size);
        // 随机装饰
        const seed = (tileX * 7 + tileY * 13) % 100;
        if (seed < 5) {
          ctx.fillStyle = 'rgba(120, 80, 200, 0.1)';
          ctx.beginPath();
          ctx.arc(x + size / 2, y + size / 2, 3, 0, Math.PI * 2);
          ctx.fill();
        }
        break;
      case 'door':
        // 门
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.fillStyle = '#5d4e37';
        ctx.fillRect(x + 4, y + 2, size - 8, size - 4);
        ctx.fillStyle = '#8b7355';
        ctx.fillRect(x + 6, y + 4, size - 12, size - 8);
        // 门把手
        ctx.fillStyle = '#f1c40f';
        ctx.beginPath();
        ctx.arc(x + size - 10, y + size / 2, 2, 0, Math.PI * 2);
        ctx.fill();
        break;
      case 'stairs':
        // 楼梯
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.fillStyle = '#4a3f6b';
        for (let i = 0; i < 4; i++) {
          ctx.fillRect(x + 4, y + 4 + i * 6, size - 8 - i * 4, 4);
        }
        // 发光效果
        ctx.shadowColor = '#9b59b6';
        ctx.shadowBlur = 10;
        ctx.fillStyle = 'rgba(155, 89, 182, 0.3)';
        ctx.fillRect(x + 8, y + 8, size - 16, size - 16);
        ctx.shadowBlur = 0;
        break;
      case 'shop':
        // 商店
        ctx.fillStyle = '#1a1a2e';
        ctx.fillRect(x, y, size, size);
        ctx.font = `${size * 0.7}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText('🏪', x + size / 2, y + size / 2);
        break;
      case 'chest':
        // 宝箱
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.font = `${size * 0.7}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText('📦', x + size / 2, y + size / 2);
        break;
      case 'trap':
        // 陷阱（可见时）
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.fillStyle = 'rgba(231, 76, 60, 0.2)';
        ctx.fillRect(x + 4, y + 4, size - 8, size - 8);
        break;
      case 'altar':
        // 祭坛
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.font = `${size * 0.7}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText('⛩️', x + size / 2, y + size / 2);
        break;
      case 'fountain':
        // 喷泉
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
        ctx.font = `${size * 0.7}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText('⛲', x + size / 2, y + size / 2);
        break;
      default:
        ctx.fillStyle = '#15121f';
        ctx.fillRect(x, y, size, size);
    }
  },
  // ==================== 实体渲染 ====================
  // 渲染玩家
  renderPlayer(player) {
    const ctx = this.ctx;
    const pos = this.worldToScreen(player.x, player.y);

    // 隐身效果
    if (player.stealth) ctx.globalAlpha = 0.4;
    // 无敌闪烁
    if (player.invincible > 0 && Math.floor(player.invincible * 10) % 2 === 0) {
      ctx.globalAlpha = 0.5;
    }

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(pos.x, pos.y + 15, 15, 6, 0, 0, Math.PI * 2);
    ctx.fill();

    // 角色身体
    ctx.font = '32px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(player.icon, pos.x, pos.y);

    // 护盾效果
    if (player.shield > 0) {
      ctx.strokeStyle = 'rgba(52, 152, 219, 0.6)';
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.arc(pos.x, pos.y, 25, 0, Math.PI * 2);
      ctx.stroke();
      // 护盾旋转
      ctx.strokeStyle = 'rgba(52, 152, 219, 0.3)';
      ctx.lineWidth = 1;
      const rot = Date.now() / 500;
      ctx.beginPath();
      ctx.arc(pos.x, pos.y, 28, rot, rot + Math.PI);
      ctx.stroke();
    }

    // 狂暴效果
    if (player.berserk) {
      ctx.strokeStyle = 'rgba(231, 76, 60, 0.5)';
      ctx.lineWidth = 2;
      ctx.beginPath();
      ctx.arc(pos.x, pos.y, 22 + Math.sin(Date.now() / 100) * 3, 0, Math.PI * 2);
      ctx.stroke();
    }

    ctx.globalAlpha = 1;

    // 名字和等级
    ctx.fillStyle = '#fff';
    ctx.font = 'bold 12px Arial';
    ctx.fillText(`${player.name} Lv.${player.level}`, pos.x, pos.y - 28);

    // 血条（小）
    const barWidth = 40;
    const barHeight = 4;
    ctx.fillStyle = '#333';
    ctx.fillRect(pos.x - barWidth / 2, pos.y - 22, barWidth, barHeight);
    ctx.fillStyle = '#e74c3c';
    ctx.fillRect(pos.x - barWidth / 2, pos.y - 22, barWidth * (player.hp / player.maxHp), barHeight);
  },
  // 渲染怪物
  renderMonster(monster) {
    if (monster.isDead) return;
    const ctx = this.ctx;
    const pos = this.worldToScreen(monster.x, monster.y);

    // 视野检查
    if (pos.x < -50 || pos.x > this.width + 50 || pos.y < -50 || pos.y > this.height + 50) return;

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(pos.x, pos.y + monster.size * 0.6, monster.size * 0.5, monster.size * 0.2, 0, 0, Math.PI * 2);
    ctx.fill();

    // 眩晕效果
    if (monster.stunned > 0) ctx.globalAlpha = 0.6;

    // 怪物身体
    ctx.font = `${monster.size}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(monster.icon, pos.x, pos.y);

    ctx.globalAlpha = 1;

    // BOSS 光环
    if (monster.isBoss) {
      const pulse = Math.sin(Date.now() / 300) * 0.3 + 0.7;
      ctx.strokeStyle = `rgba(231, 76, 60, ${pulse * 0.5})`;
      ctx.lineWidth = 3;
      ctx.beginPath();
      ctx.arc(pos.x, pos.y, monster.size * 0.8, 0, Math.PI * 2);
      ctx.stroke();
    }

    // 血条
    if (monster.hp < monster.maxHp) {
      const barWidth = monster.size * 1.5;
      const barHeight = monster.isBoss ? 8 : 5;
      const barY = pos.y - monster.size - (monster.isBoss ? 15 : 10);

      ctx.fillStyle = 'rgba(0,0,0,0.5)';
      ctx.fillRect(pos.x - barWidth / 2, barY, barWidth, barHeight);

      const hpPercent = monster.hp / monster.maxHp;
      const hpColor = monster.isBoss ? '#e74c3c' : (hpPercent > 0.5 ? '#2ecc71' : hpPercent > 0.25 ? '#f39c12' : '#e74c3c');
      ctx.fillStyle = hpColor;
      ctx.fillRect(pos.x - barWidth / 2, barY, barWidth * hpPercent, barHeight);

      // BOSS 名字
      if (monster.isBoss) {
        ctx.fillStyle = '#e74c3c';
        ctx.font = 'bold 14px Arial';
        ctx.fillText(`👑 ${monster.name}`, pos.x, barY - 8);
      }
    }

    // 状态图标
    let statusX = pos.x - 15;
    if (monster.poisoned > 0) {
      ctx.font = '14px Arial';
      ctx.fillText('☠️', statusX, pos.y - monster.size - 5);
      statusX += 15;
    }
    if (monster.burning > 0) {
      ctx.font = '14px Arial';
      ctx.fillText('🔥', statusX, pos.y - monster.size - 5);
      statusX += 15;
    }
    if (monster.stunned > 0) {
      ctx.font = '14px Arial';
      ctx.fillText('💫', pos.x, pos.y - monster.size - 20);
    }
    if (monster.confused > 0) {
      ctx.font = '14px Arial';
      ctx.fillText('❓', pos.x + 15, pos.y - monster.size - 5);
    }
    if (monster.slowed > 0) {
      ctx.font = '14px Arial';
      ctx.fillText('🐌', pos.x - 15, pos.y - monster.size - 20);
    }
  },
  // 渲染NPC
  renderNPC(npc) {
    const ctx = this.ctx;
    const pos = this.worldToScreen(npc.x, npc.y);

    if (pos.x < -50 || pos.x > this.width + 50 || pos.y < -50 || pos.y > this.height + 50) return;

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.beginPath();
    ctx.ellipse(pos.x, pos.y + 15, 15, 6, 0, 0, Math.PI * 2);
    ctx.fill();

    // NPC身体
    ctx.font = '30px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(npc.icon, pos.x, pos.y);

    // 名字
    ctx.fillStyle = '#3498db';
    ctx.font = 'bold 12px Arial';
    ctx.fillText(npc.name, pos.x, pos.y - 28);

    // 任务标记
    if (npc.hasQuest) {
      ctx.fillStyle = '#f1c40f';
      ctx.font = '16px Arial';
      ctx.fillText('!', pos.x + 18, pos.y - 25);
    }
    if (npc.questComplete) {
      ctx.fillStyle = '#2ecc71';
      ctx.font = '16px Arial';
      ctx.fillText('?', pos.x + 18, pos.y - 25);
    }

    // 交互提示
    const dist = Utils.distance(npc.x, npc.y, Game.player?.x || 0, Game.player?.y || 0);
    if (dist < 60) {
      ctx.fillStyle = 'rgba(255,255,255,0.8)';
      ctx.font = '10px Arial';
      ctx.fillText('按 E 对话', pos.x, pos.y + 30);
    }
  },
  // 渲染宠物
  renderPet(pet) {
    if (!pet.active) return;
    const ctx = this.ctx;
    const pos = this.worldToScreen(pet.x, pet.y);

    // 阴影
    ctx.fillStyle = 'rgba(0,0,0,0.2)';
    ctx.beginPath();
    ctx.ellipse(pos.x, pos.y + 10, 10, 4, 0, 0, Math.PI * 2);
    ctx.fill();

    // 宠物身体
    ctx.font = '24px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(pet.icon, pos.x, pos.y);

    // 宠物名字
    ctx.fillStyle = '#9b59b6';
    ctx.font = '10px Arial';
    ctx.fillText(pet.name, pos.x, pos.y - 20);
  },
  // ==================== 投射物渲染 ====================
  renderProjectile(proj) {
    const ctx = this.ctx;
    const pos = this.worldToScreen(proj.x, proj.y);

    if (pos.x < -20 || pos.x > this.width + 20 || pos.y < -20 || pos.y > this.height + 20) return;

    ctx.save();
    ctx.translate(pos.x, pos.y);
    ctx.rotate(Math.atan2(proj.vy, proj.vx));

    switch (proj.type) {
      case 'arrow':
        // 箭矢
        ctx.fillStyle = proj.owner === 'player' ? '#3498db' : '#e74c3c';
        ctx.beginPath();
        ctx.moveTo(10, 0);
        ctx.lineTo(-8, -3);
        ctx.lineTo(-8, 3);
        ctx.closePath();
        ctx.fill();
        // 箭羽
        ctx.fillStyle = '#fff';
        ctx.fillRect(-10, -2, 4, 4);
        break;
      case 'fireball':
        // 火球
        const fireGrad = ctx.createRadialGradient(0, 0, 0, 0, 0, 12);
        fireGrad.addColorStop(0, '#fff');
        fireGrad.addColorStop(0.3, '#f1c40f');
        fireGrad.addColorStop(0.6, '#e67e22');
        fireGrad.addColorStop(1, 'rgba(231, 76, 60, 0)');
        ctx.fillStyle = fireGrad;
        ctx.beginPath();
        ctx.arc(0, 0, 12, 0, Math.PI * 2);
        ctx.fill();
        break;
      case 'icebolt':
        // 冰弹
        const iceGrad = ctx.createRadialGradient(0, 0, 0, 0, 0, 10);
        iceGrad.addColorStop(0, '#fff');
        iceGrad.addColorStop(0.5, '#3498db');
        iceGrad.addColorStop(1, 'rgba(52, 152, 219, 0)');
        ctx.fillStyle = iceGrad;
        ctx.beginPath();
        ctx.arc(0, 0, 10, 0, Math.PI * 2);
        ctx.fill();
        break;
      case 'shadow':
        // 暗影弹
        ctx.fillStyle = '#9b59b6';
        ctx.shadowColor = '#9b59b6';
        ctx.shadowBlur = 10;
        ctx.beginPath();
        ctx.arc(0, 0, 8, 0, Math.PI * 2);
        ctx.fill();
        ctx.shadowBlur = 0;
        break;
      case 'holy':
        // 圣光弹
        ctx.fillStyle = '#f1c40f';
        ctx.shadowColor = '#f1c40f';
        ctx.shadowBlur = 15;
        ctx.beginPath();
        ctx.arc(0, 0, 8, 0, Math.PI * 2);
        ctx.fill();
        ctx.shadowBlur = 0;
        break;
      default:
        // 默认弹丸
        ctx.fillStyle = proj.owner === 'player' ? '#3498db' : '#e74c3c';
        ctx.beginPath();
        ctx.arc(0, 0, proj.size || 6, 0, Math.PI * 2);
        ctx.fill();
    }

    ctx.restore();
  },
  // ==================== 物品渲染 ====================
  renderItem(item) {
    const ctx = this.ctx;
    const pos = this.worldToScreen(item.x, item.y);

    if (pos.x < -30 || pos.x > this.width + 30 || pos.y < -30 || pos.y > this.height + 30) return;

    // 浮动动画
    const bobY = Math.sin(Date.now() / 300 + item.x) * 3;

    // 发光效果（稀有物品）
    if (item.rarity && item.rarity !== 'common') {
      const color = Utils.rarityColor(item.rarity);
      ctx.shadowColor = color;
      ctx.shadowBlur = 10;
    }

    ctx.font = '22px Arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(item.icon || ITEMS[item.id]?.icon || '❓', pos.x, pos.y + bobY);

    ctx.shadowBlur = 0;

    // 金币显示数量
    if (item.id === 'gold' && item.amount > 1) {
      ctx.fillStyle = '#f1c40f';
      ctx.font = '10px Arial';
      ctx.fillText(item.amount, pos.x + 12, pos.y + 10);
    }
  },
  // ==================== 粒子渲染 ====================
  renderParticles(particles) {
    const ctx = this.ctx;
    for (const p of particles) {
      const pos = this.worldToScreen(p.x, p.y);
      const alpha = p.life / p.maxLife;

      ctx.globalAlpha = alpha;

      if (p.text) {
        // 文字粒子（伤害数字等）
        ctx.fillStyle = p.color;
        ctx.font = `bold ${p.size}px Arial`;
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(p.text, pos.x, pos.y);
      } else {
        // 普通粒子
        ctx.fillStyle = p.color;
        if (p.shape === 'circle') {
          ctx.beginPath();
          ctx.arc(pos.x, pos.y, p.size * alpha, 0, Math.PI * 2);
          ctx.fill();
        } else if (p.shape === 'square') {
          ctx.fillRect(pos.x - p.size / 2, pos.y - p.size / 2, p.size * alpha, p.size * alpha);
        } else if (p.shape === 'star') {
          this.drawStar(pos.x, pos.y, p.size * alpha, 5);
        } else {
          ctx.beginPath();
          ctx.arc(pos.x, pos.y, p.size * alpha, 0, Math.PI * 2);
          ctx.fill();
        }
      }

      ctx.globalAlpha = 1;
    }
  },
  // 画星形
  drawStar(x, y, size, points) {
    const ctx = this.ctx;
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
  // ==================== 光照系统 ====================
  addLight(x, y, radius, color = '#fff', intensity = 1) {
    this.lights.push({ x, y, radius, color, intensity });
  },
  clearLights() {
    this.lights = [];
  },
  renderLights() {
    if (this.lights.length === 0) return;
    const ctx = this.ctx;
    ctx.globalCompositeOperation = 'screen';
    for (const light of this.lights) {
      const pos = this.worldToScreen(light.x, light.y);
      const gradient = ctx.createRadialGradient(pos.x, pos.y, 0, pos.x, pos.y, light.radius);
      gradient.addColorStop(0, light.color);
      gradient.addColorStop(1, 'rgba(0,0,0,0)');
      ctx.globalAlpha = light.intensity * 0.3;
      ctx.fillStyle = gradient;
      ctx.fillRect(pos.x - light.radius, pos.y - light.radius, light.radius * 2, light.radius * 2);
    }
    ctx.globalAlpha = 1;
    ctx.globalCompositeOperation = 'source-over';
  },
  // ==================== 文字渲染 ====================
  drawText(text, x, y, options = {}) {
    const ctx = this.ctx;
    const {
      size = 14,
      color = '#fff',
      align = 'left',
      baseline = 'top',
      bold = false,
      shadow = false,
      shadowColor = '#000'
    } = options;

    ctx.font = `${bold ? 'bold ' : ''}${size}px Arial`;
    ctx.textAlign = align;
    ctx.textBaseline = baseline;

    if (shadow) {
      ctx.fillStyle = shadowColor;
      ctx.fillText(text, x + 1, y + 1);
    }

    ctx.fillStyle = color;
    ctx.fillText(text, x, y);
  },
  // 绘制带描边的文字
  drawTextWithStroke(text, x, y, size = 16, color = '#fff', strokeColor = '#000', strokeWidth = 3) {
    const ctx = this.ctx;
    ctx.font = `bold ${size}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.lineWidth = strokeWidth;
    ctx.strokeStyle = strokeColor;
    ctx.strokeText(text, x, y);
    ctx.fillStyle = color;
    ctx.fillText(text, x, y);
  },
  // ==================== UI 渲染 ====================
  // 渲染血条
  drawBar(x, y, width, height, percent, color = '#e74c3c', bgColor = 'rgba(0,0,0,0.5)') {
    const ctx = this.ctx;
    ctx.fillStyle = bgColor;
    ctx.fillRect(x, y, width, height);
    ctx.fillStyle = color;
    ctx.fillRect(x, y, width * Utils.clamp(percent, 0, 1), height);
    ctx.strokeStyle = 'rgba(255,255,255,0.2)';
    ctx.lineWidth = 1;
    ctx.strokeRect(x, y, width, height);
  },
  // 渲染图标
  drawIcon(icon, x, y, size = 24) {
    const ctx = this.ctx;
    ctx.font = `${size}px Arial`;
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(icon, x, y);
  },
  // 渲染矩形边框
  drawRect(x, y, width, height, color = '#fff', lineWidth = 1, fill = false) {
    const ctx = this.ctx;
    ctx.strokeStyle = color;
    ctx.lineWidth = lineWidth;
    if (fill) {
      ctx.fillStyle = color;
      ctx.fillRect(x, y, width, height);
    } else {
      ctx.strokeRect(x, y, width, height);
    }
  },
  // 渲染圆角矩形
  drawRoundedRect(x, y, width, height, radius, color = '#fff', fill = false) {
    const ctx = this.ctx;
    ctx.beginPath();
    ctx.moveTo(x + radius, y);
    ctx.lineTo(x + width - radius, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + radius);
    ctx.lineTo(x + width, y + height - radius);
    ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
    ctx.lineTo(x + radius, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - radius);
    ctx.lineTo(x, y + radius);
    ctx.quadraticCurveTo(x, y, x + radius, y);
    ctx.closePath();
    if (fill) {
      ctx.fillStyle = color;
      ctx.fill();
    } else {
      ctx.strokeStyle = color;
      ctx.stroke();
    }
  }
};
window.Renderer = Renderer;
