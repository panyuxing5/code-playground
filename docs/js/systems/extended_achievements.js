/**
 * 永恒地牢 - 扩展成就与称号系统
 * 新增成就、称号、成就奖励、成就分类
 */

class ExtendedAchievementSystem {
    constructor(game) {
        this.game = game;
        this.data = this.loadData();
        this.uiVisible = false;
        this.init();
    }

    init() {
        this.loadExtendedAchievements();
        console.log('[扩展成就系统] 初始化完成');
    }

    // 加载数据
    loadData() {
        const saved = localStorage.getItem('eternal_dungeon_extended_achievements');
        if (saved) {
            try {
                return JSON.parse(saved);
            } catch (e) {
                console.error('[扩展成就系统] 加载数据失败:', e);
            }
        }
        return {
            unlockedAchievements: {},
            unlockedTitles: {},
            currentTitle: null,
            achievementPoints: 0,
            totalAchievementPoints: 0,
            stats: {
                checkInDays: 0,
                enhanceSuccess: 0,
                enhanceFail: 0,
                maxEnhanceLevel: 0,
                skillsLearned: 0,
                dailyQuestsCompleted: 0,
                totalDailyQuests: 0,
                consecutiveLoginDays: 0,
                totalPlaySessions: 0,
                monstersKilledByType: {},
                itemsCrafted: 0,
                itemsEnchanted: 0,
                runesEquipped: 0,
                petsCaptured: 0,
                mountsUnlocked: 0,
                arenasWon: 0,
                bossesDefeated: 0,
                dungeonsCleared: 0,
                goldEarnedTotal: 0,
                damageDealtTotal: 0,
                criticalHitsTotal: 0,
                deathsTotal: 0,
                potionsUsedTotal: 0,
                skillsUsedTotal: 0,
                chestsOpenedTotal: 0,
                distanceTraveled: 0,
                playTimeSeconds: 0
            }
        };
    }

    // 保存数据
    saveData() {
        localStorage.setItem('eternal_dungeon_extended_achievements', JSON.stringify(this.data));
    }

    // 加载扩展成就定义
    loadExtendedAchievements() {
        this.extendedAchievements = [
            // ===== 签到成就 =====
            {
                id: 'checkin_1',
                name: '初次签到',
                icon: '📅',
                description: '完成第一次每日签到',
                category: '签到',
                points: 10,
                condition: { type: 'checkInDays', target: 1 },
                rewards: { gold: 100, title: '初来乍到' }
            },
            {
                id: 'checkin_7',
                name: '一周坚持',
                icon: '📆',
                description: '累计签到7天',
                category: '签到',
                points: 30,
                condition: { type: 'checkInDays', target: 7 },
                rewards: { gold: 500, title: '坚持不懈' }
            },
            {
                id: 'checkin_30',
                name: '月度达人',
                icon: '🗓️',
                description: '累计签到30天',
                category: '签到',
                points: 100,
                condition: { type: 'checkInDays', target: 30 },
                rewards: { gold: 2000, title: '月度达人' }
            },
            {
                id: 'checkin_100',
                name: '百日筑基',
                icon: '🏆',
                description: '累计签到100天',
                category: '签到',
                points: 500,
                condition: { type: 'checkInDays', target: 100 },
                rewards: { gold: 10000, title: '百日传说' }
            },

            // ===== 强化成就 =====
            {
                id: 'enhance_first',
                name: '初次强化',
                icon: '⚒️',
                description: '第一次成功强化装备',
                category: '强化',
                points: 10,
                condition: { type: 'enhanceSuccess', target: 1 },
                rewards: { gold: 100, title: '强化新手' }
            },
            {
                id: 'enhance_10',
                name: '强化达人',
                icon: '🔨',
                description: '累计成功强化10次',
                category: '强化',
                points: 50,
                condition: { type: 'enhanceSuccess', target: 10 },
                rewards: { gold: 500, title: '强化达人' }
            },
            {
                id: 'enhance_50',
                name: '强化大师',
                icon: '⚔️',
                description: '累计成功强化50次',
                category: '强化',
                points: 200,
                condition: { type: 'enhanceSuccess', target: 50 },
                rewards: { gold: 2000, title: '强化大师' }
            },
            {
                id: 'enhance_plus5',
                name: '小有所成',
                icon: '✨',
                description: '将装备强化到+5',
                category: '强化',
                points: 30,
                condition: { type: 'maxEnhanceLevel', target: 5 },
                rewards: { gold: 300, title: '精炼师' }
            },
            {
                id: 'enhance_plus10',
                name: '精益求精',
                icon: '💎',
                description: '将装备强化到+10',
                category: '强化',
                points: 100,
                condition: { type: 'maxEnhanceLevel', target: 10 },
                rewards: { gold: 1000, title: '神匠' }
            },
            {
                id: 'enhance_plus15',
                name: '极致追求',
                icon: '👑',
                description: '将装备强化到+15（满级）',
                category: '强化',
                points: 500,
                condition: { type: 'maxEnhanceLevel', target: 15 },
                rewards: { gold: 5000, title: '至尊神匠' }
            },

            // ===== 技能树成就 =====
            {
                id: 'skill_first',
                name: '初学乍练',
                icon: '📖',
                description: '学习第一个技能',
                category: '技能',
                points: 10,
                condition: { type: 'skillsLearned', target: 1 },
                rewards: { gold: 100, title: '学徒' }
            },
            {
                id: 'skill_5',
                name: '博采众长',
                icon: '📚',
                description: '学习5个技能',
                category: '技能',
                points: 50,
                condition: { type: 'skillsLearned', target: 5 },
                rewards: { gold: 500, title: '博学者' }
            },
            {
                id: 'skill_10',
                name: '融会贯通',
                icon: '🎓',
                description: '学习10个技能',
                category: '技能',
                points: 150,
                condition: { type: 'skillsLearned', target: 10 },
                rewards: { gold: 1500, title: '武学宗师' }
            },
            {
                id: 'skill_all_branch',
                name: '三系精通',
                icon: '🌟',
                description: '在三个技能分支中都学习过技能',
                category: '技能',
                points: 300,
                condition: { type: 'allBranchesLearned', target: 1 },
                rewards: { gold: 3000, title: '全能战士' }
            },

            // ===== 每日任务成就 =====
            {
                id: 'daily_first',
                name: '任务新手',
                icon: '📋',
                description: '完成第一个每日任务',
                category: '任务',
                points: 10,
                condition: { type: 'dailyQuestsCompleted', target: 1 },
                rewards: { gold: 100, title: '任务新人' }
            },
            {
                id: 'daily_10',
                name: '勤劳蜜蜂',
                icon: '🐝',
                description: '累计完成10个每日任务',
                category: '任务',
                points: 50,
                condition: { type: 'dailyQuestsCompleted', target: 10 },
                rewards: { gold: 500, title: '勤劳者' }
            },
            {
                id: 'daily_50',
                name: '任务达人',
                icon: '✅',
                description: '累计完成50个每日任务',
                category: '任务',
                points: 200,
                condition: { type: 'dailyQuestsCompleted', target: 50 },
                rewards: { gold: 2000, title: '任务大师' }
            },
            {
                id: 'daily_all_in_day',
                name: '今日事今日毕',
                icon: '🌅',
                description: '一天内完成所有每日任务',
                category: '任务',
                points: 100,
                condition: { type: 'allDailyInOneDay', target: 1 },
                rewards: { gold: 1000, title: '高效达人' }
            },

            // ===== 综合成就 =====
            {
                id: 'achievement_points_100',
                name: '成就新手',
                icon: '🏅',
                description: '获得100成就点',
                category: '综合',
                points: 50,
                condition: { type: 'achievementPoints', target: 100 },
                rewards: { gold: 500, title: '成就爱好者' }
            },
            {
                id: 'achievement_points_500',
                name: '成就达人',
                icon: '🥇',
                description: '获得500成就点',
                category: '综合',
                points: 200,
                condition: { type: 'achievementPoints', target: 500 },
                rewards: { gold: 2000, title: '成就猎人' }
            },
            {
                id: 'achievement_points_1000',
                name: '成就大师',
                icon: '🏆',
                description: '获得1000成就点',
                category: '综合',
                points: 500,
                condition: { type: 'achievementPoints', target: 1000 },
                rewards: { gold: 5000, title: '成就传说' }
            },
            {
                id: 'all_systems_unlocked',
                name: '全能玩家',
                icon: '🎮',
                description: '解锁所有游戏系统',
                category: '综合',
                points: 1000,
                condition: { type: 'allSystemsUnlocked', target: 1 },
                rewards: { gold: 10000, title: '永恒王者' }
            }
        ];

        // 计算总成就点
        this.data.totalAchievementPoints = this.extendedAchievements.reduce((sum, a) => sum + a.points, 0);
    }

    // 更新统计数据
    updateStat(statType, amount = 1) {
        if (this.data.stats[statType] !== undefined) {
            this.data.stats[statType] += amount;
            this.checkAchievements();
            this.saveData();
        }
    }

    // 设置统计数据
    setStat(statType, value) {
        if (this.data.stats[statType] !== undefined) {
            this.data.stats[statType] = value;
            this.checkAchievements();
            this.saveData();
        }
    }

    // 检查成就解锁
    checkAchievements() {
        for (const achievement of this.extendedAchievements) {
            if (this.data.unlockedAchievements[achievement.id]) continue;

            const condition = achievement.condition;
            let unlocked = false;

            switch (condition.type) {
                case 'checkInDays':
                    unlocked = this.data.stats.checkInDays >= condition.target;
                    break;
                case 'enhanceSuccess':
                    unlocked = this.data.stats.enhanceSuccess >= condition.target;
                    break;
                case 'maxEnhanceLevel':
                    unlocked = this.data.stats.maxEnhanceLevel >= condition.target;
                    break;
                case 'skillsLearned':
                    unlocked = this.data.stats.skillsLearned >= condition.target;
                    break;
                case 'dailyQuestsCompleted':
                    unlocked = this.data.stats.dailyQuestsCompleted >= condition.target;
                    break;
                case 'achievementPoints':
                    unlocked = this.data.achievementPoints >= condition.target;
                    break;
                case 'allBranchesLearned':
                    // 简化处理
                    unlocked = this.data.stats.skillsLearned >= 3;
                    break;
                case 'allDailyInOneDay':
                    // 简化处理
                    unlocked = this.data.stats.dailyQuestsCompleted >= 3;
                    break;
                case 'allSystemsUnlocked':
                    // 简化处理
                    unlocked = this.data.achievementPoints >= 500;
                    break;
            }

            if (unlocked) {
                this.unlockAchievement(achievement);
            }
        }
    }

    // 解锁成就
    unlockAchievement(achievement) {
        this.data.unlockedAchievements[achievement.id] = {
            unlockedAt: Date.now(),
            achievement: achievement
        };
        this.data.achievementPoints += achievement.points;

        // 发放奖励
        if (achievement.rewards) {
            if (achievement.rewards.gold && this.game.player) {
                this.game.player.gold = (this.game.player.gold || 0) + achievement.rewards.gold;
            }
            if (achievement.rewards.title) {
                this.unlockTitle(achievement.rewards.title);
            }
        }

        this.saveData();
        this.showMessage(`🏆 成就解锁：${achievement.name}！获得 ${achievement.points} 成就点`, 'success');
        console.log('[扩展成就系统] 解锁成就:', achievement.name);
    }

    // 解锁称号
    unlockTitle(titleName) {
        if (!this.data.unlockedTitles[titleName]) {
            this.data.unlockedTitles[titleName] = {
                unlockedAt: Date.now(),
                name: titleName
            };
            this.showMessage(`🎖️ 获得新称号：${titleName}`, 'success');
        }
    }

    // 设置当前称号
    setCurrentTitle(titleName) {
        if (this.data.unlockedTitles[titleName]) {
            this.data.currentTitle = titleName;
            this.saveData();
            this.showMessage(`已装备称号：${titleName}`, 'info');
        }
    }

    // 获取成就统计
    getStats() {
        const unlockedCount = Object.keys(this.data.unlockedAchievements).length;
        const totalCount = this.extendedAchievements.length;
        const titleCount = Object.keys(this.data.unlockedTitles).length;

        return {
            unlockedAchievements: unlockedCount,
            totalAchievements: totalCount,
            completionRate: totalCount > 0 ? Math.round((unlockedCount / totalCount) * 100) : 0,
            achievementPoints: this.data.achievementPoints,
            totalAchievementPoints: this.data.totalAchievementPoints,
            unlockedTitles: titleCount,
            currentTitle: this.data.currentTitle
        };
    }

    // 按分类获取成就
    getAchievementsByCategory(category) {
        return this.extendedAchievements.filter(a => a.category === category);
    }

    // 获取所有分类
    getCategories() {
        const categories = new Set();
        this.extendedAchievements.forEach(a => categories.add(a.category));
        return Array.from(categories);
    }

    // 显示消息
    showMessage(text, type = 'info') {
        if (this.game.showMessage) {
            this.game.showMessage(text, type);
        } else {
            console.log(`[扩展成就系统] ${type}: ${text}`);
        }
    }

    // 显示成就面板
    showPanel() {
        let panel = document.getElementById('extended-achievement-panel');
        if (panel) {
            panel.style.display = 'flex';
            this.updatePanel();
            return;
        }

        panel = document.createElement('div');
        panel.id = 'extended-achievement-panel';
        panel.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 700px;
            max-height: 90vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            border: 2px solid #fdcb6e;
            border-radius: 20px;
            padding: 30px;
            color: white;
            z-index: 10000;
            box-shadow: 0 20px 60px rgba(0,0,0,0.5);
            overflow-y: auto;
            font-family: 'Microsoft YaHei', sans-serif;
        `;
        panel.innerHTML = this.getPanelHTML();
        document.body.appendChild(panel);
    }

    // 获取面板HTML
    getPanelHTML() {
        const stats = this.getStats();
        const categories = this.getCategories();

        return `
            <div style="text-align:center;margin-bottom:20px;">
                <h2 style="color:#fdcb6e;margin:0;font-size:28px;">🏆 成就与称号</h2>
                <p style="color:rgba(255,255,255,0.6);margin:5px 0 0 0;">解锁成就，获得称号，证明你的实力</p>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:10px;margin-bottom:20px;">
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#fdcb6e;">${stats.achievementPoints}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">成就点</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#00b894;">${stats.unlockedAchievements}/${stats.totalAchievements}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">已解锁成就</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#e17055;">${stats.completionRate}%</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">完成度</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#6c5ce7;">${stats.unlockedTitles}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">称号数量</div>
                </div>
            </div>
            
            <div style="margin-bottom:20px;padding:15px;background:rgba(255,255,255,0.03);border-radius:10px;">
                <div style="font-weight:bold;margin-bottom:10px;color:#fdcb6e;">🎖️ 当前称号</div>
                <div style="display:flex;align-items:center;gap:10px;">
                    <span style="font-size:24px;">👑</span>
                    <span style="font-size:18px;font-weight:bold;color:#fdcb6e;">${stats.currentTitle || '暂无称号'}</span>
                </div>
            </div>
            
            ${categories.map(category => `
                <div style="margin-bottom:20px;">
                    <div style="font-weight:bold;font-size:18px;margin-bottom:10px;color:#fdcb6e;">
                        ${this.getCategoryIcon(category)} ${category}
                    </div>
                    <div style="display:grid;grid-template-columns:repeat(2,1fr);gap:10px;">
                        ${this.getAchievementsByCategory(category).map(a => this.getAchievementCardHTML(a)).join('')}
                    </div>
                </div>
            `).join('')}
            
            <div style="display:flex;gap:10px;">
                <button onclick="window.extendedAchievementSystem.hidePanel()" style="
                    flex:1;
                    padding:15px;
                    border:1px solid rgba(255,255,255,0.2);
                    border-radius:10px;
                    background:rgba(255,255,255,0.05);
                    color:white;
                    cursor:pointer;
                    font-size:16px;
                ">
                    关闭
                </button>
            </div>
        `;
    }

    // 获取分类图标
    getCategoryIcon(category) {
        const icons = {
            '签到': '📅',
            '强化': '⚒️',
            '技能': '📖',
            '任务': '📋',
            '综合': '🏆'
        };
        return icons[category] || '🎯';
    }

    // 获取成就卡片HTML
    getAchievementCardHTML(achievement) {
        const isUnlocked = !!this.data.unlockedAchievements[achievement.id];
        const progress = this.getAchievementProgress(achievement);

        return `
            <div style="
                background:${isUnlocked ? 'rgba(253,203,110,0.1)' : 'rgba(255,255,255,0.03)'};
                border:2px solid ${isUnlocked ? '#fdcb6e' : 'rgba(255,255,255,0.1)'};
                border-radius:10px;
                padding:12px;
                opacity:${isUnlocked ? '1' : '0.7'};
            ">
                <div style="display:flex;align-items:center;gap:10px;margin-bottom:8px;">
                    <span style="font-size:24px;">${achievement.icon}</span>
                    <div style="flex:1;">
                        <div style="font-weight:bold;font-size:14px;">${achievement.name}</div>
                        <div style="font-size:11px;color:rgba(255,255,255,0.6);">${achievement.description}</div>
                    </div>
                    <div style="text-align:right;">
                        <div style="font-size:14px;font-weight:bold;color:#fdcb6e;">+${achievement.points}</div>
                        <div style="font-size:10px;color:rgba(255,255,255,0.5);">成就点</div>
                    </div>
                </div>
                ${!isUnlocked ? `
                    <div style="background:rgba(255,255,255,0.1);height:4px;border-radius:2px;overflow:hidden;">
                        <div style="background:#fdcb6e;height:100%;width:${progress}%;"></div>
                    </div>
                ` : `<div style="text-align:center;color:#00b894;font-size:12px;font-weight:bold;">✓ 已解锁</div>`}
            </div>
        `;
    }

    // 获取成就进度
    getAchievementProgress(achievement) {
        const condition = achievement.condition;
        const current = this.data.stats[condition.type] || 0;
        return Math.min((current / condition.target) * 100, 100);
    }

    // 更新面板
    updatePanel() {
        const panel = document.getElementById('extended-achievement-panel');
        if (panel) {
            panel.innerHTML = this.getPanelHTML();
        }
    }

    // 隐藏面板
    hidePanel() {
        const panel = document.getElementById('extended-achievement-panel');
        if (panel) {
            panel.style.display = 'none';
        }
        this.uiVisible = false;
    }

    // 切换UI
    toggleUI() {
        this.uiVisible = !this.uiVisible;
        if (this.uiVisible) {
            this.showPanel();
        } else {
            this.hidePanel();
        }
    }

    // 更新
    update(deltaTime) {}

    // 重置数据
    resetData() {
        if (confirm('确定要重置所有扩展成就数据吗？此操作不可恢复！')) {
            this.data = this.loadData();
            this.loadExtendedAchievements();
            this.saveData();
            this.showMessage('扩展成就数据已重置', 'info');
        }
    }
}

// 导出到全局
if (typeof window !== 'undefined') {
    window.ExtendedAchievementSystem = ExtendedAchievementSystem;
}
