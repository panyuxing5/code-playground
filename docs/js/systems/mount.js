// 永恒地牢 - 坐骑系统
class MountSystem {
    constructor(player) {
        this.player = player;
        this.mounts = [];
        this.activeMount = null;
        this.isMounted = false;
        this.mountSpeedBonus = 0;
        this.summonTimer = 0;
        this.summonDuration = 2; // 2秒（deltaTime单位是秒）
        this.isSummoning = false;
        
        this.mountData = {
            warhorse: { id: 'warhorse', name: '战马', icon: '🐴', speedBonus: 0.6, rarity: 'common', description: '可靠的战马，能在战斗中保护骑手' },
            shadow_steed: { id: 'shadow_steed', name: '暗影战马', icon: '🐎', speedBonus: 0.8, rarity: 'rare', description: '来自暗影位面的神秘战马' },
            flaming_mount: { id: 'flaming_mount', name: '烈焰坐骑', icon: '🔥', speedBonus: 1.0, rarity: 'epic', description: '被烈焰包裹的传奇坐骑' },
            ice_wolf: { id: 'ice_wolf', name: '冰原巨狼', icon: '🐺', speedBonus: 0.9, rarity: 'rare', description: '来自北境冰原的巨狼' },
            griffin: { id: 'griffin', name: '狮鹫', icon: '🦅', speedBonus: 1.2, rarity: 'epic', description: '狮头鹰身的神兽，可以短距离飞行' },
            dragon_whelp: { id: 'dragon_whelp', name: '幼龙', icon: '🐉', speedBonus: 1.5, rarity: 'legendary', description: '年幼的龙族，拥有强大的力量' },
            skeleton_horse: { id: 'skeleton_horse', name: '骷髅马', icon: '💀', speedBonus: 0.7, rarity: 'uncommon', description: '亡灵骑士的忠实伙伴' },
            unicorn: { id: 'unicorn', name: '独角兽', icon: '🦄', speedBonus: 1.1, rarity: 'epic', description: '纯洁的神兽，能治愈骑手的伤口' },
            tiger: { id: 'tiger', name: '猛虎', icon: '🐯', speedBonus: 0.85, rarity: 'rare', description: '森林之王，敏捷而凶猛' },
            boar: { id: 'boar', name: '野猪', icon: '🐗', speedBonus: 0.5, rarity: 'common', description: '皮糙肉厚的野猪，适合新手' },
            raptor: { id: 'raptor', name: '迅猛龙', icon: '🦖', speedBonus: 1.3, rarity: 'epic', description: '远古时代的掠食者，速度极快' },
            nightmare: { id: 'nightmare', name: '梦魇', icon: '🌙', speedBonus: 1.4, rarity: 'legendary', description: '来自梦境的恐怖生物' },
            holy_charger: { id: 'holy_charger', name: '圣光战马', icon: '✨', speedBonus: 1.2, rarity: 'epic', description: '被圣光祝福的战马，能驱散黑暗' },
            void_walker: { id: 'void_walker', name: '虚空行者', icon: '🌀', speedBonus: 1.6, rarity: 'legendary', description: '来自虚空的存在，能穿越空间' },
            phoenix_chick: { id: 'phoenix_chick', name: '凤凰幼崽', icon: '🐦', speedBonus: 1.8, rarity: 'legendary', description: '传说中的不死鸟，浴火重生' },
            golem: { id: 'golem', name: '石魔像', icon: '🗿', speedBonus: 0.3, rarity: 'uncommon', description: '缓慢但极其坚固的石魔像' },
            spider: { id: 'spider', name: '巨型蜘蛛', icon: '🕷️', speedBonus: 0.75, rarity: 'uncommon', description: '能在墙壁上行走的巨型蜘蛛' },
            bat: { id: 'bat', name: '吸血蝙蝠', icon: '🦇', speedBonus: 0.9, rarity: 'rare', description: '能短暂飞行的吸血蝙蝠' },
            slime_mount: { id: 'slime_mount', name: '史莱姆坐骑', icon: '🟢', speedBonus: 0.4, rarity: 'common', description: 'Q弹的史莱姆，坐上去很舒服' },
            crystal_deer: { id: 'crystal_deer', name: '水晶鹿', icon: '🦌', speedBonus: 1.0, rarity: 'epic', description: '身体由水晶构成的神秘鹿' }
        };
        
        this.rarityColors = {
            common: '#9d9d9d',
            uncommon: '#1eff00',
            rare: '#0070dd',
            epic: '#a335ee',
            legendary: '#ff8000'
        };
        
        console.log('[MountSystem] 坐骑系统初始化完成，共' + Object.keys(this.mountData).length + '种坐骑');
    }
    
    addMount(mountId) {
        if (!this.mountData[mountId]) return false;
        if (this.mounts.includes(mountId)) return false;
        this.mounts.push(mountId);
        if (this.player && this.player.game && this.player.game.showNotification) {
            this.player.game.showNotification(`获得新坐骑：${this.mountData[mountId].name}`);
        }
        return true;
    }
    
    summonMount(mountId) {
        if (!this.mounts.includes(mountId)) return false;
        if (this.isSummoning) return false;
        
        this.isSummoning = true;
        this.summonTimer = 0;
        this.activeMount = mountId;
        
        if (this.player && this.player.game && this.player.game.audioSystem) {
            this.player.game.audioSystem.playSound('mount_summon');
        }
        
        return true;
    }
    
    dismissMount() {
        if (!this.isMounted) return false;
        this.isMounted = false;
        this.mountSpeedBonus = 0;
        this.activeMount = null;
        if (this.player) {
            this.player.speed = this.player.baseSpeed || this.player.speed;
        }
        return true;
    }
    
    toggleMount() {
        if (this.isMounted) {
            this.dismissMount();
        } else if (this.mounts.length > 0) {
            this.summonMount(this.mounts[0]);
        }
    }
    
    update(deltaTime) {
        if (this.isSummoning) {
            this.summonTimer += deltaTime;
            if (this.summonTimer >= this.summonDuration) {
                this.isSummoning = false;
                this.isMounted = true;
                const mount = this.mountData[this.activeMount];
                this.mountSpeedBonus = mount.speedBonus;
                if (this.player) {
                    this.player.baseSpeed = this.player.baseSpeed || this.player.speed;
                    this.player.speed = this.player.baseSpeed * (1 + this.mountSpeedBonus);
                }
                if (this.player && this.player.game && this.player.game.showNotification) {
                    this.player.game.showNotification(`召唤坐骑：${mount.name}（速度+${Math.round(mount.speedBonus * 100)}%）`);
                }
            }
        }
    }
    
    render(ctx, playerX, playerY, cameraX, cameraY, canvasWidth, canvasHeight) {
        if (!this.isMounted && !this.isSummoning) return;
        if (!this.activeMount) return;
        
        const mount = this.mountData[this.activeMount];
        const screenX = playerX - cameraX + canvasWidth / 2;
        const screenY = playerY - cameraY + canvasHeight / 2;
        
        let scale = 1;
        let opacity = 1;
        
        if (this.isSummoning) {
            const progress = this.summonTimer / this.summonDuration;
            scale = 0.5 + progress * 0.5;
            opacity = progress;
            
            ctx.globalAlpha = opacity * 0.5;
            ctx.fillStyle = '#ffffff';
            ctx.beginPath();
            ctx.arc(screenX, screenY, 40 * (1 - progress), 0, Math.PI * 2);
            ctx.fill();
            ctx.globalAlpha = 1;
        }
        
        ctx.save();
        ctx.translate(screenX, screenY + 20);
        ctx.scale(scale, scale);
        ctx.globalAlpha = opacity;
        
        ctx.font = '40px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(mount.icon, 0, 0);
        
        ctx.restore();
        ctx.globalAlpha = 1;
        
        if (this.isMounted) {
            ctx.font = '12px Arial';
            ctx.fillStyle = this.rarityColors[mount.rarity];
            ctx.textAlign = 'center';
            ctx.fillText(mount.name, screenX, screenY - 50);
        }
    }
    
    getMountCount() {
        return this.mounts.length;
    }
    
    getAvailableMounts() {
        return this.mounts.map(id => this.mountData[id]);
    }
    
    getActiveMount() {
        return this.activeMount ? this.mountData[this.activeMount] : null;
    }
    
    getSpeedBonus() {
        return this.isMounted ? this.mountSpeedBonus : 0;
    }
}
