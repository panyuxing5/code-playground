// 永恒地牢 - 游戏统计与成就面板
class StatsPanel {
    constructor(game) {
        this.game = game;
        this.isVisible = false;
        this.stats = {
            totalKills: 0,
            totalDamage: 0,
            totalHealing: 0,
            totalGold: 0,
            totalItems: 0,
            totalLevels: 0,
            totalDeaths: 0,
            totalPlayTime: 0,
            floorsCleared: 0,
            bossesKilled: 0,
            questsCompleted: 0,
            achievementsUnlocked: 0,
            petsCollected: 0,
            mountsCollected: 0,
            itemsCrafted: 0,
            itemsEnchanted: 0,
            potionsUsed: 0,
            skillsUsed: 0,
            criticalHits: 0,
            maxCombo: 0,
            currentCombo: 0,
            comboTimer: 0
        };
        
        this.achievements = [
            { id: 'first_blood', name: '初次击杀', desc: '击杀第一个怪物', icon: '⚔️', condition: () => this.stats.totalKills >= 1, unlocked: false },
            { id: 'killer_10', name: '小试牛刀', desc: '击杀10个怪物', icon: '🗡️', condition: () => this.stats.totalKills >= 10, unlocked: false },
            { id: 'killer_50', name: '屠戮者', desc: '击杀50个怪物', icon: '💀', condition: () => this.stats.totalKills >= 50, unlocked: false },
            { id: 'killer_100', name: '百人斩', desc: '击杀100个怪物', icon: '☠️', condition: () => this.stats.totalKills >= 100, unlocked: false },
            { id: 'damage_1000', name: '伤害新手', desc: '造成1000点伤害', icon: '💥', condition: () => this.stats.totalDamage >= 1000, unlocked: false },
            { id: 'damage_10000', name: '伤害大师', desc: '造成10000点伤害', icon: '🔥', condition: () => this.stats.totalDamage >= 10000, unlocked: false },
            { id: 'gold_100', name: '小富翁', desc: '累计获得100金币', icon: '💰', condition: () => this.stats.totalGold >= 100, unlocked: false },
            { id: 'gold_1000', name: '大富翁', desc: '累计获得1000金币', icon: '💎', condition: () => this.stats.totalGold >= 1000, unlocked: false },
            { id: 'level_5', name: '初出茅庐', desc: '达到5级', icon: '📈', condition: () => this.stats.totalLevels >= 5, unlocked: false },
            { id: 'level_10', name: '身经百战', desc: '达到10级', icon: '🏆', condition: () => this.stats.totalLevels >= 10, unlocked: false },
            { id: 'floor_5', name: '深入地牢', desc: '到达第5层', icon: '🏰', condition: () => this.stats.floorsCleared >= 5, unlocked: false },
            { id: 'floor_10', name: '地牢探索者', desc: '到达第10层', icon: '🗺️', condition: () => this.stats.floorsCleared >= 10, unlocked: false },
            { id: 'boss_first', name: '屠龙勇士', desc: '击杀第一个Boss', icon: '🐉', condition: () => this.stats.bossesKilled >= 1, unlocked: false },
            { id: 'quest_1', name: '任务新手', desc: '完成第一个任务', icon: '📜', condition: () => this.stats.questsCompleted >= 1, unlocked: false },
            { id: 'craft_first', name: '工匠学徒', desc: '制作第一件物品', icon: '🔨', condition: () => this.stats.itemsCrafted >= 1, unlocked: false },
            { id: 'enchant_first', name: '附魔新手', desc: '附魔第一件物品', icon: '✨', condition: () => this.stats.itemsEnchanted >= 1, unlocked: false },
            { id: 'pet_first', name: '宠物主人', desc: '获得第一只宠物', icon: '🐾', condition: () => this.stats.petsCollected >= 1, unlocked: false },
            { id: 'mount_first', name: '骑士', desc: '获得第一只坐骑', icon: '🐴', condition: () => this.stats.mountsCollected >= 1, unlocked: false },
            { id: 'death_first', name: '不屈不挠', desc: '第一次死亡', icon: '💀', condition: () => this.stats.totalDeaths >= 1, unlocked: false },
            { id: 'combo_5', name: '连击新手', desc: '达成5连击', icon: '⚡', condition: () => this.stats.maxCombo >= 5, unlocked: false },
            { id: 'combo_10', name: '连击大师', desc: '达成10连击', icon: '🌟', condition: () => this.stats.maxCombo >= 10, unlocked: false },
            { id: 'crit_50', name: '暴击高手', desc: '造成50次暴击', icon: '💢', condition: () => this.stats.criticalHits >= 50, unlocked: false },
            { id: 'playtime_10min', name: '地牢常客', desc: '游戏时长10分钟', icon: '⏰', condition: () => this.stats.totalPlayTime >= 600, unlocked: false },
            { id: 'playtime_1hour', name: '地牢狂人', desc: '游戏时长1小时', icon: '⌛', condition: () => this.stats.totalPlayTime >= 3600, unlocked: false },
            { id: 'potion_10', name: '药剂大师', desc: '使用10瓶药水', icon: '🧪', condition: () => this.stats.potionsUsed >= 10, unlocked: false },
            { id: 'skill_100', name: '技能达人', desc: '使用100次技能', icon: '🎯', condition: () => this.stats.skillsUsed >= 100, unlocked: false }
        ];
        
        console.log('[StatsPanel] 游戏统计与成就面板初始化完成，共' + this.achievements.length + '个成就');
    }
    
    update(deltaTime) {
        this.stats.totalPlayTime += deltaTime;
        
        if (this.stats.comboTimer > 0) {
            this.stats.comboTimer -= deltaTime;
            if (this.stats.comboTimer <= 0) {
                this.stats.currentCombo = 0;
            }
        }
        
        this.checkAchievements();
    }
    
    addKill(isBoss = false) {
        this.stats.totalKills++;
        if (isBoss) this.stats.bossesKilled++;
        this.addCombo();
    }
    
    addDamage(amount, isCrit = false) {
        this.stats.totalDamage += amount;
        if (isCrit) this.stats.criticalHits++;
    }
    
    addHealing(amount) {
        this.stats.totalHealing += amount;
    }
    
    addGold(amount) {
        this.stats.totalGold += amount;
    }
    
    addLevel() {
        this.stats.totalLevels++;
    }
    
    addDeath() {
        this.stats.totalDeaths++;
        this.stats.currentCombo = 0;
    }
    
    addFloor() {
        this.stats.floorsCleared++;
    }
    
    addQuest() {
        this.stats.questsCompleted++;
    }
    
    addCraft() {
        this.stats.itemsCrafted++;
    }
    
    addEnchant() {
        this.stats.itemsEnchanted++;
    }
    
    addPotion() {
        this.stats.potionsUsed++;
    }
    
    addSkill() {
        this.stats.skillsUsed++;
    }
    
    addPet() {
        this.stats.petsCollected++;
    }
    
    addMount() {
        this.stats.mountsCollected++;
    }
    
    addCombo() {
        this.stats.currentCombo++;
        this.stats.comboTimer = 3;
        if (this.stats.currentCombo > this.stats.maxCombo) {
            this.stats.maxCombo = this.stats.currentCombo;
        }
    }
    
    checkAchievements() {
        for (const ach of this.achievements) {
            if (!ach.unlocked && ach.condition()) {
                ach.unlocked = true;
                this.stats.achievementsUnlocked++;
                if (this.game && this.game.showMessage) {
                    this.game.showMessage(`🏆 成就解锁：${ach.name}！`);
                }
                if (this.game && this.game.audioSystem) {
                    this.game.audioSystem.playSound('achievement');
                }
                console.log(`[Achievement] 解锁成就：${ach.name} - ${ach.desc}`);
            }
        }
    }
    
    toggle() {
        this.isVisible = !this.isVisible;
        return this.isVisible;
    }
    
    render(ctx, canvasWidth, canvasHeight) {
        if (!this.isVisible) return;
        
        const panelWidth = 600;
        const panelHeight = 500;
        const x = (canvasWidth - panelWidth) / 2;
        const y = (canvasHeight - panelHeight) / 2;
        
        ctx.fillStyle = 'rgba(10, 10, 30, 0.95)';
        ctx.fillRect(x, y, panelWidth, panelHeight);
        
        ctx.strokeStyle = '#8b7355';
        ctx.lineWidth = 3;
        ctx.strokeRect(x, y, panelWidth, panelHeight);
        
        ctx.fillStyle = '#ffd700';
        ctx.font = 'bold 24px Arial';
        ctx.textAlign = 'center';
        ctx.fillText('📊 游戏统计 & 🏆 成就', x + panelWidth / 2, y + 40);
        
        const stats = [
            ['⚔️ 总击杀', this.stats.totalKills],
            ['💥 总伤害', Math.floor(this.stats.totalDamage)],
            ['💰 总金币', Math.floor(this.stats.totalGold)],
            ['📈 等级', this.stats.totalLevels],
            ['🏰 层数', this.stats.floorsCleared],
            ['🐉 Boss击杀', this.stats.bossesKilled],
            ['📜 任务完成', this.stats.questsCompleted],
            ['💀 死亡次数', this.stats.totalDeaths],
            ['⏰ 游戏时长', this.formatTime(this.stats.totalPlayTime)],
            ['⚡ 最高连击', this.stats.maxCombo],
            ['💢 暴击次数', this.stats.criticalHits],
            ['🏆 成就解锁', `${this.stats.achievementsUnlocked}/${this.achievements.length}`]
        ];
        
        ctx.font = '14px Arial';
        ctx.textAlign = 'left';
        const startX = x + 30;
        const startY = y + 70;
        const colWidth = 180;
        const rowHeight = 28;
        
        for (let i = 0; i < stats.length; i++) {
            const col = i % 3;
            const row = Math.floor(i / 3);
            const statX = startX + col * colWidth;
            const statY = startY + row * rowHeight;
            
            ctx.fillStyle = '#aaa';
            ctx.fillText(stats[i][0], statX, statY);
            ctx.fillStyle = '#fff';
            ctx.fillText(stats[i][1], statX + 110, statY);
        }
        
        const achievementStartY = startY + Math.ceil(stats.length / 3) * rowHeight + 20;
        ctx.fillStyle = '#ffd700';
        ctx.font = 'bold 16px Arial';
        ctx.fillText('成就列表：', startX, achievementStartY);
        
        ctx.font = '12px Arial';
        const unlockedAchievements = this.achievements.filter(a => a.unlocked);
        for (let i = 0; i < Math.min(unlockedAchievements.length, 6); i++) {
            const ach = unlockedAchievements[i];
            const achY = achievementStartY + 25 + i * 22;
            ctx.fillStyle = '#ffd700';
            ctx.fillText(`${ach.icon} ${ach.name}`, startX, achY);
            ctx.fillStyle = '#888';
            ctx.fillText(ach.desc, startX + 150, achY);
        }
        
        if (unlockedAchievements.length === 0) {
            ctx.fillStyle = '#666';
            ctx.fillText('还没有解锁任何成就，继续努力！', startX, achievementStartY + 25);
        }
        
        ctx.fillStyle = '#888';
        ctx.font = '12px Arial';
        ctx.textAlign = 'center';
        ctx.fillText('按 Tab 键关闭', x + panelWidth / 2, y + panelHeight - 20);
    }
    
    formatTime(seconds) {
        const h = Math.floor(seconds / 3600);
        const m = Math.floor((seconds % 3600) / 60);
        const s = Math.floor(seconds % 60);
        if (h > 0) return `${h}时${m}分`;
        if (m > 0) return `${m}分${s}秒`;
        return `${s}秒`;
    }
    
    getStats() {
        return { ...this.stats };
    }
    
    getUnlockedAchievements() {
        return this.achievements.filter(a => a.unlocked);
    }
}
