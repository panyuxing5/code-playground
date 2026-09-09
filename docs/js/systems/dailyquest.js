/**
 * 永恒地牢 - 每日任务系统
 * 每日任务、任务进度、任务奖励、每日刷新
 */

class DailyQuestSystem {
    constructor(game) {
        this.game = game;
        this.dailyQuestData = this.loadDailyQuestData();
        this.uiVisible = false;
        this.init();
    }

    init() {
        this.checkDailyReset();
        console.log('[每日任务系统] 初始化完成');
    }

    // 加载数据
    loadDailyQuestData() {
        const saved = localStorage.getItem('eternal_dungeon_dailyquest');
        if (saved) {
            try {
                return JSON.parse(saved);
            } catch (e) {
                console.error('[每日任务系统] 加载数据失败:', e);
            }
        }
        return this.getDefaultData();
    }

    // 默认数据
    getDefaultData() {
        return {
            lastResetDate: null,
            currentQuests: [],
            completedQuests: [],
            totalCompleted: 0,
            totalRewards: {
                gold: 0,
                exp: 0,
                items: []
            },
            streakDays: 0,
            lastCompleteDate: null
        };
    }

    // 保存数据
    saveDailyQuestData() {
        localStorage.setItem('eternal_dungeon_dailyquest', JSON.stringify(this.dailyQuestData));
    }

    // 检查每日重置
    checkDailyReset() {
        const today = this.getTodayString();
        if (this.dailyQuestData.lastResetDate !== today) {
            this.generateDailyQuests();
            this.dailyQuestData.lastResetDate = today;
            this.saveDailyQuestData();
            console.log('[每日任务系统] 每日任务已刷新');
        }
    }

    // 获取今天日期
    getTodayString() {
        const now = new Date();
        return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
    }

    // 生成每日任务
    generateDailyQuests() {
        const questTemplates = this.getQuestTemplates();
        const selectedQuests = [];
        
        // 随机选择3-5个任务
        const questCount = 3 + Math.floor(Math.random() * 3);
        const shuffled = [...questTemplates].sort(() => Math.random() - 0.5);
        
        for (let i = 0; i < Math.min(questCount, shuffled.length); i++) {
            const template = shuffled[i];
            const quest = {
                ...template,
                id: `daily_${Date.now()}_${i}`,
                progress: 0,
                target: this.generateTarget(template.type),
                completed: false,
                claimed: false,
                createdAt: Date.now()
            };
            quest.rewards = this.generateRewards(template.difficulty, quest.target);
            selectedQuests.push(quest);
        }
        
        this.dailyQuestData.currentQuests = selectedQuests;
        this.dailyQuestData.completedQuests = [];
    }

    // 生成任务目标
    generateTarget(type) {
        const targets = {
            kill_monsters: [10, 20, 30, 50],
            kill_boss: [1, 2, 3],
            collect_gold: [100, 200, 500, 1000],
            collect_items: [5, 10, 20],
            complete_dungeon: [1, 2, 3],
            enhance_equipment: [1, 2, 3],
            use_skills: [20, 50, 100],
            deal_damage: [500, 1000, 2000, 5000],
            survive_time: [60, 120, 300],
            open_chests: [3, 5, 10]
        };
        
        const options = targets[type] || [10];
        return options[Math.floor(Math.random() * options.length)];
    }

    // 生成奖励
    generateRewards(difficulty, target) {
        const baseGold = 50 * difficulty * (target / 10);
        const baseExp = 30 * difficulty * (target / 10);
        
        const rewards = {
            gold: Math.floor(baseGold),
            exp: Math.floor(baseExp),
            items: []
        };
        
        // 高难度任务有几率给物品
        if (difficulty >= 3 && Math.random() < 0.5) {
            const itemPool = [
                { id: 'hp_potion', name: '生命药水', count: 3 },
                { id: 'mp_potion', name: '魔法药水', count: 3 },
                { id: 'enhance_stone', name: '强化石', count: 2 },
                { id: 'scroll', name: '神秘卷轴', count: 1 }
            ];
            rewards.items.push(itemPool[Math.floor(Math.random() * itemPool.length)]);
        }
        
        return rewards;
    }

    // 获取任务模板
    getQuestTemplates() {
        return [
            {
                type: 'kill_monsters',
                name: '猎杀怪物',
                icon: '⚔️',
                description: '击杀指定数量的怪物',
                difficulty: 1,
                category: 'combat'
            },
            {
                type: 'kill_boss',
                name: '挑战Boss',
                icon: '👹',
                description: '击败指定数量的Boss',
                difficulty: 3,
                category: 'combat'
            },
            {
                type: 'collect_gold',
                name: '收集金币',
                icon: '💰',
                description: '收集指定数量的金币',
                difficulty: 1,
                category: 'collection'
            },
            {
                type: 'collect_items',
                name: '收集物品',
                icon: '📦',
                description: '收集指定数量的物品',
                difficulty: 2,
                category: 'collection'
            },
            {
                type: 'complete_dungeon',
                name: '通关地牢',
                icon: '🏰',
                description: '通关指定次数的地牢',
                difficulty: 2,
                category: 'exploration'
            },
            {
                type: 'enhance_equipment',
                name: '强化装备',
                icon: '⚒️',
                description: '成功强化指定次数的装备',
                difficulty: 2,
                category: 'progression'
            },
            {
                type: 'use_skills',
                name: '使用技能',
                icon: '✨',
                description: '使用指定次数的技能',
                difficulty: 1,
                category: 'combat'
            },
            {
                type: 'deal_damage',
                name: '造成伤害',
                icon: '💥',
                description: '累计造成指定数量的伤害',
                difficulty: 2,
                category: 'combat'
            },
            {
                type: 'survive_time',
                name: '生存挑战',
                icon: '🛡️',
                description: '在地牢中生存指定时间（秒）',
                difficulty: 2,
                category: 'exploration'
            },
            {
                type: 'open_chests',
                name: '开启宝箱',
                icon: '🎁',
                description: '开启指定数量的宝箱',
                difficulty: 1,
                category: 'exploration'
            }
        ];
    }

    // 更新任务进度
    updateQuestProgress(type, amount = 1) {
        let updated = false;
        
        for (const quest of this.dailyQuestData.currentQuests) {
            if (quest.type === type && !quest.completed) {
                quest.progress = Math.min(quest.progress + amount, quest.target);
                
                if (quest.progress >= quest.target) {
                    quest.completed = true;
                    this.dailyQuestData.completedQuests.push(quest.id);
                    this.showMessage(`🎉 任务完成：${quest.name}！点击领取奖励`, 'success');
                }
                
                updated = true;
            }
        }
        
        if (updated) {
            this.saveDailyQuestData();
        }
        
        return updated;
    }

    // 领取任务奖励
    claimQuestReward(questId) {
        const quest = this.dailyQuestData.currentQuests.find(q => q.id === questId);
        if (!quest) return { success: false, reason: 'quest_not_found' };
        if (!quest.completed) return { success: false, reason: 'not_completed' };
        if (quest.claimed) return { success: false, reason: 'already_claimed' };

        // 发放奖励
        if (this.game.player) {
            if (this.game.player.gold !== undefined) {
                this.game.player.gold += quest.rewards.gold;
            }
            if (this.game.player.exp !== undefined) {
                this.game.player.exp += quest.rewards.exp;
            }
        }

        quest.claimed = true;
        this.dailyQuestData.totalCompleted++;
        this.dailyQuestData.totalRewards.gold += quest.rewards.gold;
        this.dailyQuestData.totalRewards.exp += quest.rewards.exp;
        quest.rewards.items.forEach(item => {
            this.dailyQuestData.totalRewards.items.push(item);
        });

        // 更新连续完成天数
        const today = this.getTodayString();
        if (this.dailyQuestData.lastCompleteDate !== today) {
            const yesterday = this.getYesterdayString();
            if (this.dailyQuestData.lastCompleteDate === yesterday) {
                this.dailyQuestData.streakDays++;
            } else {
                this.dailyQuestData.streakDays = 1;
            }
            this.dailyQuestData.lastCompleteDate = today;
        }

        this.saveDailyQuestData();
        this.showMessage(`🎁 领取奖励：${quest.rewards.gold}金币 + ${quest.rewards.exp}经验`, 'success');
        
        // 更新扩展成就系统统计
        if (this.game.extendedAchievementSystem) {
            this.game.extendedAchievementSystem.updateStat('dailyQuestsCompleted', 1);
        }
        
        return { success: true, rewards: quest.rewards };
    }

    // 领取所有已完成任务的奖励
    claimAllRewards() {
        let totalGold = 0;
        let totalExp = 0;
        let claimedCount = 0;

        for (const quest of this.dailyQuestData.currentQuests) {
            if (quest.completed && !quest.claimed) {
                const result = this.claimQuestReward(quest.id);
                if (result.success) {
                    totalGold += result.rewards.gold;
                    totalExp += result.rewards.exp;
                    claimedCount++;
                }
            }
        }

        if (claimedCount > 0) {
            this.showMessage(`🎉 一键领取 ${claimedCount} 个任务奖励：${totalGold}金币 + ${totalExp}经验`, 'success');
        } else {
            this.showMessage('没有可领取的奖励', 'warning');
        }

        return { claimedCount, totalGold, totalExp };
    }

    // 获取昨天日期
    getYesterdayString() {
        const yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);
        return `${yesterday.getFullYear()}-${String(yesterday.getMonth() + 1).padStart(2, '0')}-${String(yesterday.getDate()).padStart(2, '0')}`;
    }

    // 获取任务统计
    getStats() {
        const total = this.dailyQuestData.currentQuests.length;
        const completed = this.dailyQuestData.currentQuests.filter(q => q.completed).length;
        const claimed = this.dailyQuestData.currentQuests.filter(q => q.claimed).length;

        return {
            totalQuests: total,
            completedQuests: completed,
            claimedQuests: claimed,
            unclaimedRewards: completed - claimed,
            completionRate: total > 0 ? Math.round((completed / total) * 100) : 0,
            totalCompleted: this.dailyQuestData.totalCompleted,
            streakDays: this.dailyQuestData.streakDays,
            totalGoldEarned: this.dailyQuestData.totalRewards.gold,
            totalExpEarned: this.dailyQuestData.totalRewards.exp
        };
    }

    // 显示消息
    showMessage(text, type = 'info') {
        if (this.game.showMessage) {
            this.game.showMessage(text, type);
        } else {
            console.log(`[每日任务系统] ${type}: ${text}`);
        }
    }

    // 显示任务面板
    showQuestPanel() {
        let panel = document.getElementById('dailyquest-panel');
        if (panel) {
            panel.style.display = 'flex';
            this.updateQuestPanel();
            return;
        }

        panel = document.createElement('div');
        panel.id = 'dailyquest-panel';
        panel.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 600px;
            max-height: 85vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            border: 2px solid #00b894;
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
        const quests = this.dailyQuestData.currentQuests;

        return `
            <div style="text-align:center;margin-bottom:20px;">
                <h2 style="color:#00b894;margin:0;font-size:28px;">📋 每日任务</h2>
                <p style="color:rgba(255,255,255,0.6);margin:5px 0 0 0;">完成任务获取丰厚奖励，每日0点刷新</p>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:10px;margin-bottom:20px;">
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#00b894;">${stats.completedQuests}/${stats.totalQuests}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">今日完成</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#fdcb6e;">${stats.unclaimedRewards}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">待领取</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#e17055;">${stats.streakDays}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">连续天数</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#0984e3;">${stats.totalCompleted}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">累计完成</div>
                </div>
            </div>
            
            <div style="margin-bottom:20px;">
                <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
                    <span style="font-weight:bold;">今日进度</span>
                    <span style="color:rgba(255,255,255,0.6);">${stats.completionRate}%</span>
                </div>
                <div style="background:rgba(255,255,255,0.1);height:10px;border-radius:5px;overflow:hidden;">
                    <div style="background:linear-gradient(90deg,#00b894,#55efc4);height:100%;width:${stats.completionRate}%;border-radius:5px;transition:width 0.5s;"></div>
                </div>
            </div>
            
            <div style="margin-bottom:20px;">
                ${quests.length === 0 ? `
                    <div style="text-align:center;padding:40px;color:rgba(255,255,255,0.5);">
                        <div style="font-size:48px;margin-bottom:10px;">📭</div>
                        <div>暂无任务，明天再来看看吧</div>
                    </div>
                ` : quests.map(quest => this.getQuestCardHTML(quest)).join('')}
            </div>
            
            <div style="display:flex;gap:10px;">
                <button onclick="window.dailyQuestSystem.claimAllRewards(); window.dailyQuestSystem.updateQuestPanel();" style="
                    flex:1;
                    padding:15px;
                    border:none;
                    border-radius:10px;
                    background:linear-gradient(135deg,#fdcb6e,#e17055);
                    color:white;
                    cursor:pointer;
                    font-size:16px;
                    font-weight:bold;
                ">
                    🎁 一键领取奖励
                </button>
                <button onclick="window.dailyQuestSystem.hideQuestPanel()" style="
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

    // 获取任务卡片HTML
    getQuestCardHTML(quest) {
        const progress = Math.min((quest.progress / quest.target) * 100, 100);
        const isCompleted = quest.completed;
        const isClaimed = quest.claimed;

        let bgColor = 'rgba(255,255,255,0.03)';
        let borderColor = 'rgba(255,255,255,0.1)';
        
        if (isCompleted && !isClaimed) {
            bgColor = 'rgba(253,203,110,0.1)';
            borderColor = '#fdcb6e';
        } else if (isClaimed) {
            bgColor = 'rgba(0,184,148,0.05)';
            borderColor = '#00b894';
        }

        return `
            <div style="
                background:${bgColor};
                border:2px solid ${borderColor};
                border-radius:12px;
                padding:15px;
                margin-bottom:10px;
                transition:all 0.3s;
            ">
                <div style="display:flex;align-items:center;gap:15px;">
                    <div style="font-size:36px;">${quest.icon}</div>
                    <div style="flex:1;">
                        <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:5px;">
                            <span style="font-weight:bold;font-size:16px;">${quest.name}</span>
                            ${isClaimed ? '<span style="color:#00b894;font-size:12px;">✓ 已领取</span>' : 
                              isCompleted ? '<span style="color:#fdcb6e;font-size:12px;">🎉 可领取</span>' :
                              `<span style="color:rgba(255,255,255,0.6);font-size:12px;">${quest.progress}/${quest.target}</span>`}
                        </div>
                        <div style="font-size:13px;color:rgba(255,255,255,0.6);margin-bottom:8px;">${quest.description}</div>
                        <div style="background:rgba(255,255,255,0.1);height:6px;border-radius:3px;overflow:hidden;">
                            <div style="background:${isCompleted ? '#00b894' : '#fdcb6e'};height:100%;width:${progress}%;border-radius:3px;transition:width 0.3s;"></div>
                        </div>
                    </div>
                    <div style="text-align:right;min-width:100px;">
                        <div style="font-size:12px;color:rgba(255,255,255,0.6);margin-bottom:3px;">奖励</div>
                        <div style="font-size:13px;color:#fdcb6e;">💰 ${quest.rewards.gold}</div>
                        <div style="font-size:13px;color:#74b9ff;">⭐ ${quest.rewards.exp}</div>
                        ${quest.rewards.items.length > 0 ? `<div style="font-size:11px;color:#55efc4;">🎁 ${quest.rewards.items.length}件物品</div>` : ''}
                    </div>
                </div>
                ${isCompleted && !isClaimed ? `
                    <button onclick="window.dailyQuestSystem.claimQuestReward('${quest.id}'); window.dailyQuestSystem.updateQuestPanel();" style="
                        width:100%;
                        margin-top:10px;
                        padding:10px;
                        border:none;
                        border-radius:8px;
                        background:linear-gradient(135deg,#fdcb6e,#e17055);
                        color:white;
                        cursor:pointer;
                        font-weight:bold;
                        font-size:14px;
                    ">
                        🎁 领取奖励
                    </button>
                ` : ''}
            </div>
        `;
    }

    // 更新面板
    updateQuestPanel() {
        const panel = document.getElementById('dailyquest-panel');
        if (panel) {
            panel.innerHTML = this.getPanelHTML();
        }
    }

    // 隐藏面板
    hideQuestPanel() {
        const panel = document.getElementById('dailyquest-panel');
        if (panel) {
            panel.style.display = 'none';
        }
        this.uiVisible = false;
    }

    // 切换UI
    toggleUI() {
        this.uiVisible = !this.uiVisible;
        if (this.uiVisible) {
            this.showQuestPanel();
        } else {
            this.hideQuestPanel();
        }
    }

    // 更新（每帧调用）
    update(deltaTime) {
        // 可以在这里添加实时进度更新
    }

    // 重置数据
    resetData() {
        if (confirm('确定要重置所有每日任务数据吗？此操作不可恢复！')) {
            this.dailyQuestData = this.getDefaultData();
            this.saveDailyQuestData();
            this.showMessage('每日任务数据已重置', 'info');
        }
    }
}

// 导出到全局
if (typeof window !== 'undefined') {
    window.DailyQuestSystem = DailyQuestSystem;
}
