/**
 * 永恒地牢 - 装备强化系统
 * 装备强化、升级、属性提升、星级系统、保护符
 */

class EnhanceSystem {
    constructor(game) {
        this.game = game;
        this.enhanceData = this.loadEnhanceData();
        this.uiVisible = false;
        this.selectedItem = null;
        this.init();
    }

    init() {
        console.log('[强化系统] 初始化完成');
    }

    // 加载强化数据
    loadEnhanceData() {
        const saved = localStorage.getItem('eternal_dungeon_enhance');
        if (saved) {
            try {
                return JSON.parse(saved);
            } catch (e) {
                console.error('[强化系统] 加载数据失败:', e);
            }
        }
        return this.getDefaultData();
    }

    // 默认数据
    getDefaultData() {
        return {
            totalEnhanceCount: 0,           // 总强化次数
            successCount: 0,                 // 成功次数
            failCount: 0,                    // 失败次数
            maxEnhanceLevel: 0,              // 达到的最高强化等级
            totalGoldCost: 0,                // 总金币消耗
            totalMaterialCost: 0,            // 总材料消耗
            protectScrollUsed: 0,            // 使用的保护符数量
            enhanceHistory: [],               // 强化历史
            itemEnhanceLevels: {}             // 物品强化等级 {itemId: level}
        };
    }

    // 保存数据
    saveEnhanceData() {
        localStorage.setItem('eternal_dungeon_enhance', JSON.stringify(this.enhanceData));
    }

    // 获取强化配置
    getEnhanceConfig() {
        return {
            maxLevel: 15,                     // 最高强化等级
            baseSuccessRate: 0.9,             // 基础成功率
            successRateDecay: 0.06,           // 每级成功率衰减
            minSuccessRate: 0.1,               // 最低成功率
            baseGoldCost: 100,                 // 基础金币消耗
            goldCostMultiplier: 1.5,           // 金币消耗倍率
            baseStatBonus: 0.05,               // 基础属性加成（每级5%）
            statBonusMultiplier: 1.2,           // 属性加成倍率
            failDowngradeChance: 0.3,           // 失败降级概率
            protectScrollPreventDowngrade: true  // 保护符防止降级
        };
    }

    // 获取强化成功率
    getSuccessRate(currentLevel) {
        const config = this.getEnhanceConfig();
        if (currentLevel >= config.maxLevel) return 0;
        
        let rate = config.baseSuccessRate - (currentLevel * config.successRateDecay);
        return Math.max(rate, config.minSuccessRate);
    }

    // 获取强化金币消耗
    getGoldCost(currentLevel) {
        const config = this.getEnhanceConfig();
        return Math.floor(config.baseGoldCost * Math.pow(config.goldCostMultiplier, currentLevel));
    }

    // 获取强化材料消耗
    getMaterialCost(currentLevel) {
        // 不同等级需要不同材料
        const materials = [
            { level: 1, name: '强化石', count: 1 },
            { level: 3, name: '高级强化石', count: 1 },
            { level: 6, name: '稀有强化石', count: 1 },
            { level: 9, name: '史诗强化石', count: 1 },
            { level: 12, name: '传说强化石', count: 1 }
        ];
        
        const result = [];
        materials.forEach(mat => {
            if (currentLevel >= mat.level - 1) {
                result.push({ ...mat, count: mat.count + Math.floor(currentLevel / 3) });
            }
        });
        
        return result;
    }

    // 获取属性加成
    getStatBonus(enhanceLevel) {
        const config = this.getEnhanceConfig();
        let bonus = 0;
        for (let i = 1; i <= enhanceLevel; i++) {
            bonus += config.baseStatBonus * Math.pow(config.statBonusMultiplier, i - 1);
        }
        return bonus;
    }

    // 获取物品强化等级
    getItemEnhanceLevel(itemId) {
        return this.enhanceData.itemEnhanceLevels[itemId] || 0;
    }

    // 设置物品强化等级
    setItemEnhanceLevel(itemId, level) {
        this.enhanceData.itemEnhanceLevels[itemId] = level;
        this.saveEnhanceData();
    }

    // 执行强化
    enhance(itemId, useProtectScroll = false) {
        const currentLevel = this.getItemEnhanceLevel(itemId);
        const config = this.getEnhanceConfig();
        
        // 检查是否已满级
        if (currentLevel >= config.maxLevel) {
            this.showMessage('装备已达到最高强化等级！', 'warning');
            return { success: false, reason: 'max_level' };
        }

        // 检查金币
        const goldCost = this.getGoldCost(currentLevel);
        if (this.game.player && this.game.player.gold < goldCost) {
            this.showMessage(`金币不足！需要 ${goldCost} 金币`, 'error');
            return { success: false, reason: 'gold_insufficient' };
        }

        // 检查材料（简化处理，实际应该检查背包）
        const materials = this.getMaterialCost(currentLevel);
        // 这里简化为不检查材料，只消耗金币

        // 检查保护符
        if (useProtectScroll) {
            // 检查是否有保护符（简化处理）
            this.enhanceData.protectScrollUsed++;
        }

        // 消耗金币
        if (this.game.player) {
            this.game.player.gold -= goldCost;
        }
        this.enhanceData.totalGoldCost += goldCost;
        this.enhanceData.totalEnhanceCount++;

        // 计算成功率
        const successRate = this.getSuccessRate(currentLevel);
        const isSuccess = Math.random() < successRate;

        if (isSuccess) {
            // 强化成功
            const newLevel = currentLevel + 1;
            this.setItemEnhanceLevel(itemId, newLevel);
            this.enhanceData.successCount++;
            
            if (newLevel > this.enhanceData.maxEnhanceLevel) {
                this.enhanceData.maxEnhanceLevel = newLevel;
            }

            // 记录历史
            this.enhanceData.enhanceHistory.push({
                itemId,
                fromLevel: currentLevel,
                toLevel: newLevel,
                success: true,
                goldCost,
                useProtectScroll,
                timestamp: Date.now()
            });

            this.saveEnhanceData();
            this.showMessage(`🎉 强化成功！装备从 +${currentLevel} 提升到 +${newLevel}`, 'success');
            
            // 更新扩展成就系统统计
            if (this.game.extendedAchievementSystem) {
                this.game.extendedAchievementSystem.updateStat('enhanceSuccess', 1);
                if (newLevel > this.game.extendedAchievementSystem.data.stats.maxEnhanceLevel) {
                    this.game.extendedAchievementSystem.setStat('maxEnhanceLevel', newLevel);
                }
            }
            
            return { 
                success: true, 
                newLevel, 
                oldLevel: currentLevel,
                statBonus: this.getStatBonus(newLevel)
            };
        } else {
            // 强化失败
            this.enhanceData.failCount++;
            let newLevel = currentLevel;
            let downgraded = false;

            // 检查是否降级
            if (!useProtectScroll || !config.protectScrollPreventDowngrade) {
                if (Math.random() < config.failDowngradeChance && currentLevel > 0) {
                    newLevel = currentLevel - 1;
                    downgraded = true;
                    this.setItemEnhanceLevel(itemId, newLevel);
                }
            }

            // 记录历史
            this.enhanceData.enhanceHistory.push({
                itemId,
                fromLevel: currentLevel,
                toLevel: newLevel,
                success: false,
                downgraded,
                goldCost,
                useProtectScroll,
                timestamp: Date.now()
            });

            this.saveEnhanceData();
            
            if (downgraded) {
                this.showMessage(`💔 强化失败！装备从 +${currentLevel} 降级到 +${newLevel}`, 'error');
            } else {
                this.showMessage(`💔 强化失败！装备等级保持 +${currentLevel}`, 'warning');
            }
            
            return { 
                success: false, 
                newLevel, 
                oldLevel: currentLevel,
                downgraded
            };
        }
    }

    // 批量强化（自动强化到指定等级）
    autoEnhance(itemId, targetLevel, useProtectScroll = false, maxGold = Infinity) {
        let currentLevel = this.getItemEnhanceLevel(itemId);
        let totalGoldSpent = 0;
        let results = [];

        while (currentLevel < targetLevel && totalGoldSpent < maxGold) {
            const result = this.enhance(itemId, useProtectScroll);
            results.push(result);
            
            if (result.success) {
                currentLevel = result.newLevel;
            } else if (result.downgraded) {
                currentLevel = result.newLevel;
            }
            
            totalGoldSpent += this.getGoldCost(currentLevel > 0 ? currentLevel - 1 : 0);
            
            // 防止无限循环
            if (results.length > 100) break;
        }

        return {
            results,
            finalLevel: currentLevel,
            totalGoldSpent,
            successCount: results.filter(r => r.success).length,
            failCount: results.filter(r => !r.success).length
        };
    }

    // 获取强化统计
    getStats() {
        const total = this.enhanceData.totalEnhanceCount;
        const success = this.enhanceData.successCount;
        const successRate = total > 0 ? ((success / total) * 100).toFixed(1) : 0;

        return {
            totalEnhanceCount: total,
            successCount: success,
            failCount: this.enhanceData.failCount,
            successRate: parseFloat(successRate),
            maxEnhanceLevel: this.enhanceData.maxEnhanceLevel,
            totalGoldCost: this.enhanceData.totalGoldCost,
            protectScrollUsed: this.enhanceData.protectScrollUsed,
            enhancedItems: Object.keys(this.enhanceData.itemEnhanceLevels).length
        };
    }

    // 显示消息
    showMessage(text, type = 'info') {
        if (this.game.showMessage) {
            this.game.showMessage(text, type);
        } else {
            console.log(`[强化系统] ${type}: ${text}`);
        }
    }

    // 显示强化面板
    showEnhancePanel() {
        let panel = document.getElementById('enhance-panel');
        if (panel) {
            panel.style.display = 'flex';
            this.updateEnhancePanel();
            return;
        }

        panel = document.createElement('div');
        panel.id = 'enhance-panel';
        panel.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 600px;
            max-height: 85vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            border: 2px solid #ffd700;
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
        const config = this.getEnhanceConfig();

        return `
            <div style="text-align:center;margin-bottom:20px;">
                <h2 style="color:#ffd700;margin:0;font-size:28px;">⚔️ 装备强化</h2>
                <p style="color:rgba(255,255,255,0.6);margin:5px 0 0 0;">强化装备，提升属性，打造神装</p>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:10px;margin-bottom:20px;">
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#ffd700;">${stats.totalEnhanceCount}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">总强化次数</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#00b894;">${stats.successRate}%</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">成功率</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#e94560;">+${stats.maxEnhanceLevel}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">最高强化</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#0984e3;">${stats.enhancedItems}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">已强化装备</div>
                </div>
            </div>
            
            <div style="background:rgba(255,255,255,0.03);border-radius:10px;padding:20px;margin-bottom:20px;">
                <div style="font-weight:bold;margin-bottom:15px;color:#ffd700;font-size:18px;">📊 强化等级说明</div>
                <div style="display:grid;grid-template-columns:repeat(5,1fr);gap:8px;">
                    ${this.getEnhanceLevelBadges()}
                </div>
            </div>
            
            <div style="background:rgba(255,255,255,0.03);border-radius:10px;padding:20px;margin-bottom:20px;">
                <div style="font-weight:bold;margin-bottom:15px;color:#ffd700;font-size:18px;">📈 成功率与消耗</div>
                <table style="width:100%;border-collapse:collapse;font-size:13px;">
                    <thead>
                        <tr style="border-bottom:1px solid rgba(255,255,255,0.1);">
                            <th style="padding:8px;text-align:left;">等级</th>
                            <th style="padding:8px;text-align:center;">成功率</th>
                            <th style="padding:8px;text-align:center;">金币消耗</th>
                            <th style="padding:8px;text-align:right;">属性加成</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${this.getEnhanceTableRows()}
                    </tbody>
                </table>
            </div>
            
            <div style="display:flex;gap:10px;">
                <button onclick="window.enhanceSystem.hideEnhancePanel()" style="
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

    // 获取强化等级徽章
    getEnhanceLevelBadges() {
        const levels = [
            { level: 1, color: '#ffffff', name: '普通' },
            { level: 3, color: '#00b894', name: '优秀' },
            { level: 5, color: '#0984e3', name: '精良' },
            { level: 7, color: '#6c5ce7', name: '稀有' },
            { level: 9, color: '#e17055', name: '史诗' },
            { level: 11, color: '#fdcb6e', name: '传说' },
            { level: 13, color: '#e84393', name: '神话' },
            { level: 15, color: '#ff7675', name: '至尊' }
        ];

        return levels.map(l => `
            <div style="
                background:${l.color}22;
                border:1px solid ${l.color};
                border-radius:8px;
                padding:10px;
                text-align:center;
            ">
                <div style="font-size:20px;font-weight:bold;color:${l.color};">+${l.level}</div>
                <div style="font-size:11px;color:rgba(255,255,255,0.7);">${l.name}</div>
            </div>
        `).join('');
    }

    // 获取强化表格行
    getEnhanceTableRows() {
        let rows = '';
        for (let i = 0; i <= 10; i++) {
            const rate = (this.getSuccessRate(i) * 100).toFixed(0);
            const cost = this.getGoldCost(i);
            const bonus = (this.getStatBonus(i) * 100).toFixed(1);
            
            let rateColor = '#00b894';
            if (rate < 50) rateColor = '#e17055';
            else if (rate < 70) rateColor = '#fdcb6e';
            
            rows += `
                <tr style="border-bottom:1px solid rgba(255,255,255,0.05);">
                    <td style="padding:8px;font-weight:bold;">+${i}</td>
                    <td style="padding:8px;text-align:center;color:${rateColor};">${rate}%</td>
                    <td style="padding:8px;text-align:center;">${cost.toLocaleString()}</td>
                    <td style="padding:8px;text-align:right;color:#00b894;">+${bonus}%</td>
                </tr>
            `;
        }
        return rows;
    }

    // 更新面板
    updateEnhancePanel() {
        const panel = document.getElementById('enhance-panel');
        if (panel) {
            panel.innerHTML = this.getPanelHTML();
        }
    }

    // 隐藏面板
    hideEnhancePanel() {
        const panel = document.getElementById('enhance-panel');
        if (panel) {
            panel.style.display = 'none';
        }
        this.uiVisible = false;
    }

    // 切换UI
    toggleUI() {
        this.uiVisible = !this.uiVisible;
        if (this.uiVisible) {
            this.showEnhancePanel();
        } else {
            this.hideEnhancePanel();
        }
    }

    // 更新
    update(deltaTime) {}

    // 重置数据
    resetData() {
        if (confirm('确定要重置所有强化数据吗？此操作不可恢复！')) {
            this.enhanceData = this.getDefaultData();
            this.saveEnhanceData();
            this.showMessage('强化数据已重置', 'info');
        }
    }
}

// 导出到全局
if (typeof window !== 'undefined') {
    window.EnhanceSystem = EnhanceSystem;
}
