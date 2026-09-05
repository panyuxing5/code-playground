// ==================== 永恒地牢 - BOSS战系统 ====================
// 实现阶段变化、特殊技能、机制应对

const BossSystem = {
  // BOSS阶段配置
  bossPhases: {
    boss_slime_king: {
      name: '史莱姆之王',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：王者之威',
          skills: ['slam', 'bounce'],
          behavior: 'chase',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.6,
          name: '第二阶段：分裂之怒',
          skills: ['slam', 'bounce', 'split', 'acidSpray'],
          behavior: 'erratic',
          speedMultiplier: 1.3,
          damageMultiplier: 1.2,
          onEnter: function(boss, game) {
            game.showMessage('史莱姆之王愤怒了！它开始分裂！');
            // 召唤2个小史莱姆
            for (let i = 0; i < 2; i++) {
              const angle = (Math.PI * 2 / 2) * i;
              const mx = boss.x + Math.cos(angle) * 80;
              const my = boss.y + Math.sin(angle) * 80;
              game.monsters.push(new Monster(mx, my, 'slime', game.currentFloor));
            }
          }
        },
        {
          threshold: 0.3,
          name: '第三阶段：酸液暴走',
          skills: ['slam', 'bounce', 'split', 'acidSpray', 'acidRain'],
          behavior: 'erratic',
          speedMultiplier: 1.5,
          damageMultiplier: 1.5,
          onEnter: function(boss, game) {
            game.showMessage('史莱姆之王进入暴走状态！小心酸液雨！');
          }
        }
      ],
      // 特殊技能定义
      skills: {
        slam: {
          name: '猛击',
          cooldown: 3000,
          damage: 15,
          range: 60,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 80) {
              game.player.takeDamage(15 * boss.damageMultiplier, game);
              // 震地粒子效果
              for (let i = 0; i < 15; i++) {
                ParticleSystem.spawn(boss.x + (Math.random() - 0.5) * 100, boss.y + 30, {
                  vx: (Math.random() - 0.5) * 4,
                  vy: -Math.random() * 3,
                  life: 500,
                  color: '#8B4513',
                  size: 6
                });
              }
            }
          }
        },
        bounce: {
          name: '弹跳',
          cooldown: 2000,
          execute: function(boss, game) {
            // 向玩家方向跳跃
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              boss.x += (dx / dist) * 100;
              boss.y += (dy / dist) * 100;
            }
            // 落地伤害
            const newDist = Math.sqrt((game.player.x - boss.x) ** 2 + (game.player.y - boss.y) ** 2);
            if (newDist < 50) {
              game.player.takeDamage(10 * boss.damageMultiplier, game);
            }
          }
        },
        split: {
          name: '分裂',
          cooldown: 8000,
          execute: function(boss, game) {
            // 召唤小史莱姆
            for (let i = 0; i < 3; i++) {
              const angle = Math.random() * Math.PI * 2;
              const mx = boss.x + Math.cos(angle) * 60;
              const my = boss.y + Math.sin(angle) * 60;
              game.monsters.push(new Monster(mx, my, 'slime', game.currentFloor));
            }
            game.showMessage('史莱姆之王分裂出了小史莱姆！');
          }
        },
        acidSpray: {
          name: '酸液喷射',
          cooldown: 4000,
          execute: function(boss, game) {
            // 向玩家发射酸液弹
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x,
                y: boss.y,
                vx: (dx / dist) * 5,
                vy: (dy / dist) * 5,
                damage: 12 * boss.damageMultiplier,
                color: '#7FFF00',
                size: 10,
                fromMonster: true,
                life: 2000
              });
            }
          }
        },
        acidRain: {
          name: '酸液雨',
          cooldown: 10000,
          execute: function(boss, game) {
            // 在玩家周围降下酸液雨
            game.showMessage('酸液雨来袭！快躲开！');
            for (let i = 0; i < 20; i++) {
              setTimeout(() => {
                const rx = game.player.x + (Math.random() - 0.5) * 300;
                const ry = game.player.y + (Math.random() - 0.5) * 300;
                game.projectiles.push({
                  x: rx,
                  y: ry - 200,
                  vx: 0,
                  vy: 8,
                  damage: 8 * boss.damageMultiplier,
                  color: '#7FFF00',
                  size: 8,
                  fromMonster: true,
                  life: 3000
                });
              }, i * 100);
            }
          }
        }
      }
    },

    boss_skeleton_lord: {
      name: '骷髅领主',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：亡灵统帅',
          skills: ['slash', 'summonSkeletons'],
          behavior: 'chase',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.6,
          name: '第二阶段：骨刃风暴',
          skills: ['slash', 'summonSkeletons', 'boneStorm', 'boneShield'],
          behavior: 'chase',
          speedMultiplier: 1.2,
          damageMultiplier: 1.3,
          onEnter: function(boss, game) {
            game.showMessage('骷髅领主召唤了骨刃风暴！');
          }
        },
        {
          threshold: 0.3,
          name: '第三阶段：亡者之怒',
          skills: ['slash', 'summonSkeletons', 'boneStorm', 'deathNova'],
          behavior: 'erratic',
          speedMultiplier: 1.4,
          damageMultiplier: 1.6,
          onEnter: function(boss, game) {
            game.showMessage('骷髅领主释放了亡者之怒！');
            boss.hp = Math.min(boss.maxHp * 0.3, boss.hp + boss.maxHp * 0.1); // 回复一点血
          }
        }
      ],
      skills: {
        slash: {
          name: '斩击',
          cooldown: 2500,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 70) {
              game.player.takeDamage(20 * boss.damageMultiplier, game);
            }
          }
        },
        summonSkeletons: {
          name: '召唤骷髅',
          cooldown: 7000,
          execute: function(boss, game) {
            for (let i = 0; i < 3; i++) {
              const angle = Math.random() * Math.PI * 2;
              const mx = boss.x + Math.cos(angle) * 100;
              const my = boss.y + Math.sin(angle) * 100;
              game.monsters.push(new Monster(mx, my, 'skeleton', game.currentFloor));
            }
            game.showMessage('骷髅领主召唤了骷髅士兵！');
          }
        },
        boneStorm: {
          name: '骨刃风暴',
          cooldown: 5000,
          execute: function(boss, game) {
            // 向8个方向发射骨刃
            for (let i = 0; i < 8; i++) {
              const angle = (Math.PI * 2 / 8) * i;
              game.projectiles.push({
                x: boss.x,
                y: boss.y,
                vx: Math.cos(angle) * 4,
                vy: Math.sin(angle) * 4,
                damage: 10 * boss.damageMultiplier,
                color: '#F5F5DC',
                size: 8,
                fromMonster: true,
                life: 2500
              });
            }
          }
        },
        boneShield: {
          name: '骨盾',
          cooldown: 12000,
          execute: function(boss, game) {
            boss.shield = 30;
            game.showMessage('骷髅领主召唤了骨盾！攻击被吸收！');
          }
        },
        deathNova: {
          name: '死亡新星',
          cooldown: 8000,
          execute: function(boss, game) {
            // 范围AOE
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 150) {
              game.player.takeDamage(25 * boss.damageMultiplier, game);
            }
            // 视觉效果
            for (let i = 0; i < 30; i++) {
              const angle = (Math.PI * 2 / 30) * i;
              ParticleSystem.spawn(boss.x + Math.cos(angle) * 50, boss.y + Math.sin(angle) * 50, {
                vx: Math.cos(angle) * 6,
                vy: Math.sin(angle) * 6,
                life: 600,
                color: '#8B008B',
                size: 10
              });
            }
          }
        }
      }
    },

    boss_dragon: {
      name: '远古巨龙',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：龙之威仪',
          skills: ['claw', 'fireball', 'tailSwipe'],
          behavior: 'chase',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.6,
          name: '第二阶段：烈焰吐息',
          skills: ['claw', 'fireball', 'tailSwipe', 'fireBreath', 'fly'],
          behavior: 'kite',
          speedMultiplier: 1.3,
          damageMultiplier: 1.3,
          onEnter: function(boss, game) {
            game.showMessage('巨龙展开翅膀，烈焰吐息准备！');
          }
        },
        {
          threshold: 0.3,
          name: '第三阶段：龙之怒',
          skills: ['claw', 'fireball', 'tailSwipe', 'fireBreath', 'meteor', 'dragonRage'],
          behavior: 'erratic',
          speedMultiplier: 1.5,
          damageMultiplier: 1.7,
          onEnter: function(boss, game) {
            game.showMessage('巨龙进入暴怒状态！流星火雨来袭！');
          }
        }
      ],
      skills: {
        claw: {
          name: '龙爪',
          cooldown: 2000,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 80) {
              game.player.takeDamage(25 * boss.damageMultiplier, game);
            }
          }
        },
        fireball: {
          name: '火球术',
          cooldown: 3000,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x,
                y: boss.y,
                vx: (dx / dist) * 6,
                vy: (dy / dist) * 6,
                damage: 18 * boss.damageMultiplier,
                color: '#FF4500',
                size: 14,
                fromMonster: true,
                life: 3000,
                isFireball: true
              });
            }
          }
        },
        tailSwipe: {
          name: '尾击',
          cooldown: 4000,
          execute: function(boss, game) {
            // 身后范围攻击
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 100 && dist > 40) {
              game.player.takeDamage(20 * boss.damageMultiplier, game);
              // 击退
              game.player.x += (dx / dist) * 50;
              game.player.y += (dy / dist) * 50;
            }
          }
        },
        fireBreath: {
          name: '烈焰吐息',
          cooldown: 6000,
          execute: function(boss, game) {
            game.showMessage('巨龙喷出烈焰！');
            // 锥形火焰区域
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 200) {
              game.player.takeDamage(30 * boss.damageMultiplier, game);
              game.player.applyBuff('burn', 5, 3000); // 燃烧debuff
            }
            // 火焰粒子
            for (let i = 0; i < 25; i++) {
              ParticleSystem.spawn(boss.x + (Math.random() - 0.5) * 100, boss.y + (Math.random() - 0.5) * 50, {
                vx: (Math.random() - 0.5) * 8,
                vy: (Math.random() - 0.5) * 4,
                life: 800,
                color: Math.random() > 0.5 ? '#FF4500' : '#FFD700',
                size: 12
              });
            }
          }
        },
        fly: {
          name: '飞行',
          cooldown: 5000,
          execute: function(boss, game) {
            // 飞到随机位置
            boss.x = 200 + Math.random() * (game.mapWidth - 400);
            boss.y = 200 + Math.random() * (game.mapHeight - 400);
            game.showMessage('巨龙飞到了别处！');
          }
        },
        meteor: {
          name: '流星火雨',
          cooldown: 10000,
          execute: function(boss, game) {
            game.showMessage('流星火雨！快找掩护！');
            for (let i = 0; i < 15; i++) {
              setTimeout(() => {
                const rx = game.player.x + (Math.random() - 0.5) * 400;
                const ry = game.player.y + (Math.random() - 0.5) * 400;
                game.projectiles.push({
                  x: rx,
                  y: ry - 300,
                  vx: (Math.random() - 0.5) * 2,
                  vy: 10,
                  damage: 20 * boss.damageMultiplier,
                  color: '#FF6347',
                  size: 16,
                  fromMonster: true,
                  life: 4000,
                  isMeteor: true
                });
              }, i * 150);
            }
          }
        },
        dragonRage: {
          name: '龙之怒',
          cooldown: 15000,
          execute: function(boss, game) {
            // 全屏AOE，需要躲到掩体后
            game.showMessage('龙之怒！快躲到石头后面！');
            setTimeout(() => {
              const dx = game.player.x - boss.x;
              const dy = game.player.y - boss.y;
              const dist = Math.sqrt(dx * dx + dy * dy);
              // 检查玩家附近是否有掩体（简化：距离BOSS远就减伤）
              const damage = dist > 200 ? 15 : 40;
              game.player.takeDamage(damage * boss.damageMultiplier, game);
            }, 1500);
          }
        }
      }
    },

    boss_demon_lord: {
      name: '恶魔领主',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：地狱使者',
          skills: ['demonSlash', 'hellfire', 'summonDemons'],
          behavior: 'chase',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.6,
          name: '第二阶段：恶魔变身',
          skills: ['demonSlash', 'hellfire', 'summonDemons', 'darkPulse', 'lifeDrain'],
          behavior: 'chase',
          speedMultiplier: 1.3,
          damageMultiplier: 1.4,
          onEnter: function(boss, game) {
            game.showMessage('恶魔领主变身了！它开始吸取生命！');
          }
        },
        {
          threshold: 0.3,
          name: '第三阶段：地狱之门',
          skills: ['demonSlash', 'hellfire', 'summonDemons', 'darkPulse', 'lifeDrain', 'hellgate', 'apocalypse'],
          behavior: 'erratic',
          speedMultiplier: 1.5,
          damageMultiplier: 1.8,
          onEnter: function(boss, game) {
            game.showMessage('恶魔领主打开了地狱之门！');
          }
        }
      ],
      skills: {
        demonSlash: {
          name: '恶魔斩',
          cooldown: 2000,
          execute: function(boss, game) {
            const dist = Math.sqrt((game.player.x - boss.x) ** 2 + (game.player.y - boss.y) ** 2);
            if (dist < 75) {
              game.player.takeDamage(28 * boss.damageMultiplier, game);
            }
          }
        },
        hellfire: {
          name: '地狱火',
          cooldown: 3500,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x, y: boss.y,
                vx: (dx / dist) * 5, vy: (dy / dist) * 5,
                damage: 22 * boss.damageMultiplier,
                color: '#8B0000', size: 12,
                fromMonster: true, life: 2500
              });
            }
          }
        },
        summonDemons: {
          name: '召唤恶魔',
          cooldown: 8000,
          execute: function(boss, game) {
            for (let i = 0; i < 2; i++) {
              const angle = Math.random() * Math.PI * 2;
              const mx = boss.x + Math.cos(angle) * 100;
              const my = boss.y + Math.sin(angle) * 100;
              game.monsters.push(new Monster(mx, my, 'imp', game.currentFloor));
            }
            game.showMessage('恶魔领主召唤了小恶魔！');
          }
        },
        darkPulse: {
          name: '黑暗脉冲',
          cooldown: 5000,
          execute: function(boss, game) {
            // 环形扩散
            for (let i = 0; i < 12; i++) {
              const angle = (Math.PI * 2 / 12) * i;
              game.projectiles.push({
                x: boss.x, y: boss.y,
                vx: Math.cos(angle) * 5, vy: Math.sin(angle) * 5,
                damage: 15 * boss.damageMultiplier,
                color: '#4B0082', size: 10,
                fromMonster: true, life: 2000
              });
            }
          }
        },
        lifeDrain: {
          name: '生命吸取',
          cooldown: 6000,
          execute: function(boss, game) {
            const dist = Math.sqrt((game.player.x - boss.x) ** 2 + (game.player.y - boss.y) ** 2);
            if (dist < 150) {
              const drain = 15 * boss.damageMultiplier;
              game.player.takeDamage(drain, game);
              boss.hp = Math.min(boss.maxHp, boss.hp + drain * 0.5);
              game.showMessage('恶魔领主吸取了你的生命！');
            }
          }
        },
        hellgate: {
          name: '地狱之门',
          cooldown: 12000,
          execute: function(boss, game) {
            // 持续召唤恶魔
            game.showMessage('地狱之门打开了，恶魔源源不断地涌出！');
            for (let i = 0; i < 5; i++) {
              setTimeout(() => {
                const angle = Math.random() * Math.PI * 2;
                const mx = boss.x + Math.cos(angle) * 120;
                const my = boss.y + Math.sin(angle) * 120;
                game.monsters.push(new Monster(mx, my, 'imp', game.currentFloor));
              }, i * 800);
            }
          }
        },
        apocalypse: {
          name: '末日审判',
          cooldown: 20000,
          execute: function(boss, game) {
            game.showMessage('末日审判！全屏伤害，快用无敌技能！');
            setTimeout(() => {
              game.player.takeDamage(50 * boss.damageMultiplier, game);
            }, 2000);
          }
        }
      }
    },

    boss_lich_king: {
      name: '巫妖王',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：冰霜之王',
          skills: ['frostbolt', 'frostNova', 'summonGhouls'],
          behavior: 'kite',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.6,
          name: '第二阶段：亡灵大军',
          skills: ['frostbolt', 'frostNova', 'summonGhouls', 'deathCoil', 'mindControl'],
          behavior: 'kite',
          speedMultiplier: 1.2,
          damageMultiplier: 1.4,
          onEnter: function(boss, game) {
            game.showMessage('巫妖王召唤了亡灵大军！');
          }
        },
        {
          threshold: 0.3,
          name: '第三阶段：霜之哀伤',
          skills: ['frostbolt', 'frostNova', 'summonGhouls', 'deathCoil', 'mindControl', 'froststorm', 'soulHarvest'],
          behavior: 'erratic',
          speedMultiplier: 1.4,
          damageMultiplier: 1.8,
          onEnter: function(boss, game) {
            game.showMessage('巫妖王拔出了霜之哀伤！灵魂收割开始！');
          }
        }
      ],
      skills: {
        frostbolt: {
          name: '寒冰箭',
          cooldown: 2500,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x, y: boss.y,
                vx: (dx / dist) * 5, vy: (dy / dist) * 5,
                damage: 20 * boss.damageMultiplier,
                color: '#00CED1', size: 12,
                fromMonster: true, life: 2500,
                onHit: function(player) {
                  player.applyBuff('slow', 0.5, 2000);
                }
              });
            }
          }
        },
        frostNova: {
          name: '冰霜新星',
          cooldown: 5000,
          execute: function(boss, game) {
            const dist = Math.sqrt((game.player.x - boss.x) ** 2 + (game.player.y - boss.y) ** 2);
            if (dist < 120) {
              game.player.takeDamage(25 * boss.damageMultiplier, game);
              game.player.applyBuff('freeze', 0, 2000); // 冻结
            }
            // 冰霜粒子
            for (let i = 0; i < 20; i++) {
              const angle = (Math.PI * 2 / 20) * i;
              ParticleSystem.spawn(boss.x + Math.cos(angle) * 30, boss.y + Math.sin(angle) * 30, {
                vx: Math.cos(angle) * 5, vy: Math.sin(angle) * 5,
                life: 500, color: '#00CED1', size: 8
              });
            }
          }
        },
        summonGhouls: {
          name: '召唤食尸鬼',
          cooldown: 7000,
          execute: function(boss, game) {
            for (let i = 0; i < 4; i++) {
              const angle = Math.random() * Math.PI * 2;
              const mx = boss.x + Math.cos(angle) * 100;
              const my = boss.y + Math.sin(angle) * 100;
              game.monsters.push(new Monster(mx, my, 'zombie', game.currentFloor));
            }
            game.showMessage('巫妖王召唤了食尸鬼！');
          }
        },
        deathCoil: {
          name: '死亡缠绕',
          cooldown: 4000,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x, y: boss.y,
                vx: (dx / dist) * 6, vy: (dy / dist) * 6,
                damage: 25 * boss.damageMultiplier,
                color: '#00FF00', size: 14,
                fromMonster: true, life: 2500,
                onHit: function(player, game) {
                  // 恐惧效果
                  game.showMessage('你被恐惧了！');
                }
              });
            }
          }
        },
        mindControl: {
          name: '精神控制',
          cooldown: 10000,
          execute: function(boss, game) {
            game.showMessage('巫妖王试图控制你的精神！快速点击挣脱！');
            // 简化：造成伤害并混乱
            game.player.takeDamage(15 * boss.damageMultiplier, game);
            game.player.applyBuff('confuse', 0, 3000);
          }
        },
        froststorm: {
          name: '冰霜风暴',
          cooldown: 8000,
          execute: function(boss, game) {
            game.showMessage('冰霜风暴来袭！移动速度降低！');
            game.player.applyBuff('slow', 0.3, 5000);
            // 持续伤害
            for (let i = 0; i < 10; i++) {
              setTimeout(() => {
                game.player.takeDamage(5 * boss.damageMultiplier, game);
              }, i * 500);
            }
          }
        },
        soulHarvest: {
          name: '灵魂收割',
          cooldown: 15000,
          execute: function(boss, game) {
            game.showMessage('灵魂收割！你的灵魂被吸取了！');
            const currentHp = game.player.hp;
            game.player.takeDamage(currentHp * 0.3, game); // 掉30%当前血
            boss.hp = Math.min(boss.maxHp, boss.hp + currentHp * 0.2);
          }
        }
      }
    },

    boss_final: {
      name: '混沌之主',
      phases: [
        {
          threshold: 1.0,
          name: '第一阶段：混沌初现',
          skills: ['chaosBolt', 'realitySlice', 'summonChaos'],
          behavior: 'chase',
          speedMultiplier: 1.0,
          damageMultiplier: 1.0
        },
        {
          threshold: 0.7,
          name: '第二阶段：扭曲现实',
          skills: ['chaosBolt', 'realitySlice', 'summonChaos', 'timeWarp', 'voidZone'],
          behavior: 'erratic',
          speedMultiplier: 1.3,
          damageMultiplier: 1.5,
          onEnter: function(boss, game) {
            game.showMessage('混沌之主开始扭曲现实！空间变得不稳定！');
          }
        },
        {
          threshold: 0.4,
          name: '第三阶段：虚空吞噬',
          skills: ['chaosBolt', 'realitySlice', 'summonChaos', 'timeWarp', 'voidZone', 'blackHole', 'entropy'],
          behavior: 'erratic',
          speedMultiplier: 1.5,
          damageMultiplier: 2.0,
          onEnter: function(boss, game) {
            game.showMessage('混沌之主释放了虚空之力！这是最终决战！');
          }
        }
      ],
      skills: {
        chaosBolt: {
          name: '混沌箭',
          cooldown: 2000,
          execute: function(boss, game) {
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.projectiles.push({
                x: boss.x, y: boss.y,
                vx: (dx / dist) * 6, vy: (dy / dist) * 6,
                damage: 30 * boss.damageMultiplier,
                color: '#FF00FF', size: 14,
                fromMonster: true, life: 2500
              });
            }
          }
        },
        realitySlice: {
          name: '现实切割',
          cooldown: 3500,
          execute: function(boss, game) {
            // 直线切割
            const dx = game.player.x - boss.x;
            const dy = game.player.y - boss.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist < 200) {
              game.player.takeDamage(35 * boss.damageMultiplier, game);
            }
            // 视觉效果
            for (let i = 0; i < 30; i++) {
              ParticleSystem.spawn(boss.x + (dx / dist) * i * 8, boss.y + (dy / dist) * i * 8, {
                vx: (Math.random() - 0.5) * 2, vy: (Math.random() - 0.5) * 2,
                life: 400, color: '#FF00FF', size: 6
              });
            }
          }
        },
        summonChaos: {
          name: '召唤混沌',
          cooldown: 6000,
          execute: function(boss, game) {
            const types = ['imp', 'skeleton', 'zombie'];
            for (let i = 0; i < 3; i++) {
              const angle = Math.random() * Math.PI * 2;
              const mx = boss.x + Math.cos(angle) * 120;
              const my = boss.y + Math.sin(angle) * 120;
              const type = types[Math.floor(Math.random() * types.length)];
              game.monsters.push(new Monster(mx, my, type, game.currentFloor));
            }
            game.showMessage('混沌之主召唤了混沌生物！');
          }
        },
        timeWarp: {
          name: '时间扭曲',
          cooldown: 8000,
          execute: function(boss, game) {
            game.showMessage('时间被扭曲了！你的动作变慢了！');
            game.player.applyBuff('slow', 0.4, 4000);
            boss.speedMultiplier *= 1.5;
            setTimeout(() => { boss.speedMultiplier /= 1.5; }, 4000);
          }
        },
        voidZone: {
          name: '虚空领域',
          cooldown: 7000,
          execute: function(boss, game) {
            // 在玩家位置创建虚空区域
            game.showMessage('虚空领域在你脚下展开！快离开！');
            const voidX = game.player.x;
            const voidY = game.player.y;
            // 简化：持续伤害
            for (let i = 0; i < 8; i++) {
              setTimeout(() => {
                const dist = Math.sqrt((game.player.x - voidX) ** 2 + (game.player.y - voidY) ** 2);
                if (dist < 100) {
                  game.player.takeDamage(8 * boss.damageMultiplier, game);
                }
              }, i * 400);
            }
          }
        },
        blackHole: {
          name: '黑洞',
          cooldown: 12000,
          execute: function(boss, game) {
            game.showMessage('黑洞出现了！你被吸向BOSS！');
            // 吸引玩家
            const dx = boss.x - game.player.x;
            const dy = boss.y - game.player.y;
            const dist = Math.sqrt(dx * dx + dy * dy);
            if (dist > 0) {
              game.player.x += (dx / dist) * 100;
              game.player.y += (dy / dist) * 100;
            }
            game.player.takeDamage(20 * boss.damageMultiplier, game);
          }
        },
        entropy: {
          name: '熵增',
          cooldown: 20000,
          execute: function(boss, game) {
            game.showMessage('熵增！万物归于混沌！这是最终一击！');
            setTimeout(() => {
              // 随机效果
              const effects = [
                () => { game.player.takeDamage(60 * boss.damageMultiplier, game); },
                () => { game.player.applyBuff('slow', 0.3, 5000); game.player.takeDamage(30, game); },
                () => { for (let i = 0; i < 5; i++) {
                  const angle = Math.random() * Math.PI * 2;
                  game.monsters.push(new Monster(boss.x + Math.cos(angle) * 80, boss.y + Math.sin(angle) * 80, 'imp', game.currentFloor));
                }}
              ];
              effects[Math.floor(Math.random() * effects.length)]();
            }, 2000);
          }
        }
      }
    }
  },

  // 初始化BOSS
  initBoss: function(boss) {
    const config = this.bossPhases[boss.monsterId];
    if (!config) return;

    boss.currentPhase = 0;
    boss.skillCooldowns = {};
    boss.damageMultiplier = 1.0;
    boss.speedMultiplier = 1.0;
    boss.shield = 0;
    boss.phaseEntered = [false, false, false];

    // 初始化技能冷却
    for (const skillName in config.skills) {
      boss.skillCooldowns[skillName] = 0;
    }
  },

  // 更新BOSS（每帧调用）
  updateBoss: function(boss, game, deltaTime) {
    const config = this.bossPhases[boss.monsterId];
    if (!config) return;

    // 检查阶段变化
    const hpPercent = boss.hp / boss.maxHp;
    for (let i = config.phases.length - 1; i >= 0; i--) {
      if (hpPercent <= config.phases[i].threshold && !boss.phaseEntered[i]) {
        boss.currentPhase = i;
        boss.phaseEntered[i] = true;
        const phase = config.phases[i];
        boss.damageMultiplier = phase.damageMultiplier;
        boss.speedMultiplier = phase.speedMultiplier;
        if (phase.onEnter) {
          phase.onEnter(boss, game);
        }
        game.showMessage(`【${phase.name}】`);
        break;
      }
    }

    // 更新技能冷却
    for (const skillName in boss.skillCooldowns) {
      if (boss.skillCooldowns[skillName] > 0) {
        boss.skillCooldowns[skillName] -= deltaTime;
      }
    }

    // 尝试使用技能
    const currentPhase = config.phases[boss.currentPhase];
    if (currentPhase) {
      for (const skillName of currentPhase.skills) {
        const skill = config.skills[skillName];
        if (skill && boss.skillCooldowns[skillName] <= 0) {
          // 有一定概率使用技能
          if (Math.random() < 0.02) {
            skill.execute(boss, game);
            boss.skillCooldowns[skillName] = skill.cooldown;
          }
        }
      }
    }

    // 护盾处理
    if (boss.shield > 0) {
      // 护盾在渲染时显示
    }
  },

  // BOSS受到伤害（考虑护盾）
  bossTakeDamage: function(boss, damage) {
    if (boss.shield > 0) {
      const absorbed = Math.min(boss.shield, damage);
      boss.shield -= absorbed;
      damage -= absorbed;
    }
    return damage;
  },

  // 获取BOSS当前阶段信息
  getBossPhaseInfo: function(boss) {
    const config = this.bossPhases[boss.monsterId];
    if (!config) return null;
    return config.phases[boss.currentPhase];
  }
};
