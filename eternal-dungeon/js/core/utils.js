// ==================== 永恒地牢 - 工具函数库 ====================
// 提供数学、随机、字符串、时间、碰撞检测等通用工具

const Utils = {
  // ==================== 数学工具 ====================

  // 限制数值范围
  clamp(value, min, max) {
    return Math.max(min, Math.min(max, value));
  },

  // 线性插值
  lerp(a, b, t) {
    return a + (b - a) * t;
  },

  // 平滑插值
  smoothLerp(a, b, t) {
    t = t * t * (3 - 2 * t);
    return a + (b - a) * t;
  },

  // 角度转弧度
  degToRad(deg) {
    return deg * Math.PI / 180;
  },

  // 弧度转角度
  radToDeg(rad) {
    return rad * 180 / Math.PI;
  },

  // 计算两点距离
  distance(x1, y1, x2, y2) {
    return Math.sqrt((x2 - x1) ** 2 + (y2 - y1) ** 2);
  },

  // 计算两点距离的平方（性能优化，避免开方）
  distanceSq(x1, y1, x2, y2) {
    return (x2 - x1) ** 2 + (y2 - y1) ** 2;
  },

  // 计算两点间角度
  angle(x1, y1, x2, y2) {
    return Math.atan2(y2 - y1, x2 - x1);
  },

  // 角度差（-PI 到 PI）
  angleDiff(a, b) {
    let diff = b - a;
    while (diff > Math.PI) diff -= Math.PI * 2;
    while (diff < -Math.PI) diff += Math.PI * 2;
    return diff;
  },

  // 向量归一化
  normalize(x, y) {
    const len = Math.sqrt(x * x + y * y);
    if (len === 0) return { x: 0, y: 0 };
    return { x: x / len, y: y / len };
  },

  // 向量长度
  magnitude(x, y) {
    return Math.sqrt(x * x + y * y);
  },

  // 点积
  dot(x1, y1, x2, y2) {
    return x1 * x2 + y1 * y2;
  },

  // 叉积
  cross(x1, y1, x2, y2) {
    return x1 * y2 - y1 * x2;
  },

  // 随机整数 [min, max]
  randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  },

  // 随机浮点数 [min, max)
  randomFloat(min, max) {
    return Math.random() * (max - min) + min;
  },

  // 随机选择数组元素
  randomChoice(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
  },

  // 加权随机选择
  weightedChoice(items, weights) {
    const total = weights.reduce((a, b) => a + b, 0);
    let rand = Math.random() * total;
    for (let i = 0; i < items.length; i++) {
      rand -= weights[i];
      if (rand <= 0) return items[i];
    }
    return items[items.length - 1];
  },

  // 正态分布随机（Box-Muller变换）
  randomNormal(mean = 0, stdDev = 1) {
    const u1 = Math.random();
    const u2 = Math.random();
    const z = Math.sqrt(-2 * Math.log(u1)) * Math.cos(2 * Math.PI * u2);
    return mean + z * stdDev;
  },

  // 概率事件
  chance(probability) {
    return Math.random() < probability;
  },

  // ==================== 字符串工具 ====================

  // 首字母大写
  capitalize(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
  },

  // 驼峰转下划线
  camelToSnake(str) {
    return str.replace(/([A-Z])/g, '_$1').toLowerCase();
  },

  // 下划线转驼峰
  snakeToCamel(str) {
    return str.replace(/_([a-z])/g, (_, c) => c.toUpperCase());
  },

  // 填充字符串
  padStart(str, length, char = ' ') {
    return String(str).padStart(length, char);
  },

  // 截断字符串
  truncate(str, maxLength, suffix = '...') {
    if (str.length <= maxLength) return str;
    return str.substring(0, maxLength - suffix.length) + suffix;
  },

  // 移除HTML标签
  stripHtml(html) {
    return html.replace(/<[^>]*>/g, '');
  },

  // 转义HTML
  escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  },

  // 生成唯一ID
  generateId(prefix = 'id') {
    return `${prefix}_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  },

  // 简单哈希
  simpleHash(str) {
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
      const char = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + char;
      hash = hash & hash;
    }
    return Math.abs(hash);
  },

  // ==================== 时间工具 ====================

  // 格式化时间（秒 -> mm:ss）
  formatTime(seconds) {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${this.padStart(mins, 2, '0')}:${this.padStart(secs, 2, '0')}`;
  },

  // 格式化时间（秒 -> hh:mm:ss）
  formatTimeLong(seconds) {
    const hours = Math.floor(seconds / 3600);
    const mins = Math.floor((seconds % 3600) / 60);
    const secs = Math.floor(seconds % 60);
    return `${this.padStart(hours, 2, '0')}:${this.padStart(mins, 2, '0')}:${this.padStart(secs, 2, '0')}`;
  },

  // 格式化日期
  formatDate(timestamp) {
    const date = new Date(timestamp);
    return `${date.getFullYear()}-${this.padStart(date.getMonth() + 1, 2, '0')}-${this.padStart(date.getDate(), 2, '0')} ${this.padStart(date.getHours(), 2, '0')}:${this.padStart(date.getMinutes(), 2, '0')}`;
  },

  // 相对时间
  relativeTime(timestamp) {
    const diff = Date.now() - timestamp;
    const seconds = Math.floor(diff / 1000);
    if (seconds < 60) return '刚刚';
    if (seconds < 3600) return `${Math.floor(seconds / 60)}分钟前`;
    if (seconds < 86400) return `${Math.floor(seconds / 3600)}小时前`;
    return `${Math.floor(seconds / 86400)}天前`;
  },

  // ==================== 碰撞检测 ====================

  // 矩形碰撞检测
  rectCollision(x1, y1, w1, h1, x2, y2, w2, h2) {
    return x1 < x2 + w2 && x1 + w1 > x2 && y1 < y2 + h2 && y1 + h1 > y2;
  },

  // 圆形碰撞检测
  circleCollision(x1, y1, r1, x2, y2, r2) {
    return this.distance(x1, y1, x2, y2) < r1 + r2;
  },

  // 点在矩形内
  pointInRect(px, py, rx, ry, rw, rh) {
    return px >= rx && px <= rx + rw && py >= ry && py <= ry + rh;
  },

  // 点在圆内
  pointInCircle(px, py, cx, cy, r) {
    return this.distance(px, py, cx, cy) <= r;
  },

  // 线段与圆相交
  lineCircleIntersect(x1, y1, x2, y2, cx, cy, r) {
    const dx = x2 - x1;
    const dy = y2 - y1;
    const fx = x1 - cx;
    const fy = y1 - cy;
    const a = dx * dx + dy * dy;
    const b = 2 * (fx * dx + fy * dy);
    const c = fx * fx + fy * fy - r * r;
    let discriminant = b * b - 4 * a * c;
    if (discriminant < 0) return false;
    discriminant = Math.sqrt(discriminant);
    const t1 = (-b - discriminant) / (2 * a);
    const t2 = (-b + discriminant) / (2 * a);
    return (t1 >= 0 && t1 <= 1) || (t2 >= 0 && t2 <= 1);
  },

  // ==================== 颜色工具 ====================

  // RGB转十六进制
  rgbToHex(r, g, b) {
    return '#' + [r, g, b].map(x => {
      const hex = Math.round(this.clamp(x, 0, 255)).toString(16);
      return hex.length === 1 ? '0' + hex : hex;
    }).join('');
  },

  // 十六进制转RGB
  hexToRgb(hex) {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
      r: parseInt(result[1], 16),
      g: parseInt(result[2], 16),
      b: parseInt(result[3], 16)
    } : null;
  },

  // 颜色插值
  lerpColor(color1, color2, t) {
    const c1 = this.hexToRgb(color1);
    const c2 = this.hexToRgb(color2);
    if (!c1 || !c2) return color1;
    return this.rgbToHex(
      this.lerp(c1.r, c2.r, t),
      this.lerp(c1.g, c2.g, t),
      this.lerp(c1.b, c2.b, t)
    );
  },

  // 颜色变暗
  darkenColor(hex, amount) {
    const rgb = this.hexToRgb(hex);
    if (!rgb) return hex;
    return this.rgbToHex(rgb.r * (1 - amount), rgb.g * (1 - amount), rgb.b * (1 - amount));
  },

  // 颜色变亮
  lightenColor(hex, amount) {
    const rgb = this.hexToRgb(hex);
    if (!rgb) return hex;
    return this.rgbToHex(
      rgb.r + (255 - rgb.r) * amount,
      rgb.g + (255 - rgb.g) * amount,
      rgb.b + (255 - rgb.b) * amount
    );
  },

  // ==================== 数组工具 ====================

  // 打乱数组
  shuffle(arr) {
    const result = [...arr];
    for (let i = result.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [result[i], result[j]] = [result[j], result[i]];
    }
    return result;
  },

  // 数组去重
  unique(arr) {
    return [...new Set(arr)];
  },

  // 数组分组
  groupBy(arr, key) {
    return arr.reduce((groups, item) => {
      const group = typeof key === 'function' ? key(item) : item[key];
      if (!groups[group]) groups[group] = [];
      groups[group].push(item);
      return groups;
    }, {});
  },

  // 求平均值
  average(arr) {
    if (arr.length === 0) return 0;
    return arr.reduce((sum, val) => sum + val, 0) / arr.length;
  },

  // 求中位数
  median(arr) {
    const sorted = [...arr].sort((a, b) => a - b);
    const mid = Math.floor(sorted.length / 2);
    return sorted.length % 2 !== 0 ? sorted[mid] : (sorted[mid - 1] + sorted[mid]) / 2;
  },

  // ==================== 对象工具 ====================

  // 深拷贝
  deepClone(obj) {
    if (obj === null || typeof obj !== 'object') return obj;
    if (obj instanceof Date) return new Date(obj.getTime());
    if (obj instanceof Array) return obj.map(item => this.deepClone(item));
    const result = {};
    for (const key in obj) {
      if (obj.hasOwnProperty(key)) {
        result[key] = this.deepClone(obj[key]);
      }
    }
    return result;
  },

  // 合并对象
  merge(target, ...sources) {
    for (const source of sources) {
      for (const key in source) {
        if (source.hasOwnProperty(key)) {
          if (typeof source[key] === 'object' && source[key] !== null && !Array.isArray(source[key])) {
            if (typeof target[key] !== 'object' || target[key] === null) {
              target[key] = {};
            }
            this.merge(target[key], source[key]);
          } else {
            target[key] = source[key];
          }
        }
      }
    }
    return target;
  },

  // 对象转查询字符串
  toQueryString(obj) {
    return Object.entries(obj)
      .map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');
  },

  // ==================== 存储工具 ====================

  // 本地存储
  storage: {
    get(key, defaultValue = null) {
      try {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : defaultValue;
      } catch (e) {
        return defaultValue;
      }
    },
    set(key, value) {
      try {
        localStorage.setItem(key, JSON.stringify(value));
        return true;
      } catch (e) {
        return false;
      }
    },
    remove(key) {
      try {
        localStorage.removeItem(key);
        return true;
      } catch (e) {
        return false;
      }
    },
    clear() {
      try {
        localStorage.clear();
        return true;
      } catch (e) {
        return false;
      }
    }
  },

  // ==================== 性能工具 ====================

  // 节流
  throttle(fn, delay) {
    let lastCall = 0;
    return function(...args) {
      const now = Date.now();
      if (now - lastCall >= delay) {
        lastCall = now;
        fn.apply(this, args);
      }
    };
  },

  // 防抖
  debounce(fn, delay) {
    let timer = null;
    return function(...args) {
      clearTimeout(timer);
      timer = setTimeout(() => fn.apply(this, args), delay);
    };
  },

  // 简单的性能计时
  createTimer() {
    const start = performance.now();
    return {
      elapsed() {
        return performance.now() - start;
      },
      reset() {
        return performance.now() - start;
      }
    };
  },

  // ==================== 游戏专用工具 ====================

  // 计算伤害（含暴击、闪避）
  calculateDamage(baseDamage, critChance, critMultiplier, targetArmor, targetDodge) {
    // 闪避判定
    if (Math.random() < targetDodge) {
      return { damage: 0, isCrit: false, isDodge: true };
    }
    // 暴击判定
    const isCrit = Math.random() < critChance;
    let damage = baseDamage;
    if (isCrit) damage *= critMultiplier;
    // 护甲减伤
    damage = Math.max(1, damage - targetArmor * 0.5);
    // 随机浮动 ±10%
    damage *= 0.9 + Math.random() * 0.2;
    return {
      damage: Math.floor(damage),
      isCrit,
      isDodge: false
    };
  },

  // 经验值计算
  expForLevel(level) {
    return Math.floor(100 * Math.pow(1.3, level - 1));
  },

  // 怪物属性缩放（根据层数）
  scaleMonsterStats(baseStats, floor) {
    const multiplier = 1 + (floor - 1) * 0.15;
    return {
      hp: Math.floor(baseStats.hp * multiplier),
      damage: Math.floor(baseStats.damage * multiplier),
      exp: Math.floor(baseStats.exp * multiplier),
      gold: Math.floor(baseStats.gold * multiplier),
      armor: Math.floor((baseStats.armor || 0) * multiplier)
    };
  },

  // 稀有度颜色
  rarityColor(rarity) {
    const colors = {
      common: '#95a5a6',
      uncommon: '#2ecc71',
      rare: '#3498db',
      epic: '#9b59b6',
      legendary: '#f39c12',
      mythic: '#e74c3c'
    };
    return colors[rarity] || '#95a5a6';
  },

  // 稀有度名称
  rarityName(rarity) {
    const names = {
      common: '普通',
      uncommon: '优秀',
      rare: '稀有',
      epic: '史诗',
      legendary: '传说',
      mythic: '神话'
    };
    return names[rarity] || '普通';
  },

  // 生成随机名称
  generateName(type = 'monster') {
    const prefixes = ['暗影', '烈焰', '寒冰', '雷霆', '剧毒', '狂暴', '远古', '深渊', '血腥', '幽灵'];
    const suffixes = {
      monster: ['者', '兽', '魔', '怪', '灵', '王', '领主', '使者'],
      item: ['之剑', '之杖', '之弓', '之甲', '之戒', '之护符', '之刃', '之锤'],
      location: ['之地', '之渊', '之塔', '之城', '之穴', '之域', '之境', '之海']
    };
    const prefix = this.randomChoice(prefixes);
    const suffix = this.randomChoice(suffixes[type] || suffixes.monster);
    return prefix + suffix;
  },

  // 检查是否在视野内
  isInView(x, y, cameraX, cameraY, viewWidth, viewHeight, margin = 50) {
    return x > cameraX - margin && x < cameraX + viewWidth + margin &&
           y > cameraY - margin && y < cameraY + viewHeight + margin;
  }
};

// 导出到全局
window.Utils = Utils;
