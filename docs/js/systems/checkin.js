/**
 * 永恒地牢 - 签到系统
 * 每日签到、连续签到奖励、签到日历、补签功能
 */

class CheckInSystem {
    constructor(game) {
        this.game = game;
        this.checkInData = this.loadCheckInData();
        this.uiVisible = false;
        this.uiElement = null;
        this.init();
    }

    init() {
        // 检查是否是新的一天，重置每日状态
        this.checkNewDay();
        console.log('[签到系统] 初始化完成');
    }

    // 加载签到数据
    loadCheckInData() {
        const saved = localStorage.getItem('eternal_dungeon_checkin');
        if (saved) {
            try {
                return JSON.parse(saved);
            } catch (e) {
                console.error('[签到系统] 加载数据失败:', e);
            }
        }
        return this.getDefaultData();
    }

    // 默认数据
    getDefaultData() {
        return {
            totalCheckInDays: 0,           // 总签到天数
            continuousCheckInDays: 0,       // 连续签到天数
            lastCheckInDate: null,           // 上次签到日期
            checkInHistory: [],              // 签到历史
            monthlyCheckIn: {},              // 本月签到记录 {YYYY-MM: [1,2,3...]}
            missedDays: [],                  // 本月漏签日期
            totalRewards: {                  // 累计获得奖励
                gold: 0,
                exp: 0,
                items: []
            },
            makeUpCount: 0,                  // 补签次数
            lastMakeUpDate: null             // 上次补签日期
        };
    }

    // 保存签到数据
    saveCheckInData() {
        localStorage.setItem('eternal_dungeon_checkin', JSON.stringify(this.checkInData));
    }

    // 检查是否是新的一天
    checkNewDay() {
        const today = this.getTodayString();
        const lastDate = this.checkInData.lastCheckInDate;
        
        if (lastDate && lastDate !== today) {
            // 检查是否连续签到
            const yesterday = this.getYesterdayString();
            if (lastDate !== yesterday) {
                // 断签了，重置连续签到天数
                this.checkInData.continuousCheckInDays = 0;
                console.log('[签到系统] 检测到断签，连续签到天数已重置');
            }
        }
        this.saveCheckInData();
    }

    // 获取今天日期字符串
    getTodayString() {
        const now = new Date();
        return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
    }

    // 获取昨天日期字符串
    getYesterdayString() {
        const yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);
        return `${yesterday.getFullYear()}-${String(yesterday.getMonth() + 1).padStart(2, '0')}-${String(yesterday.getDate()).padStart(2, '0')}`;
    }

    // 获取本月月份字符串
    getMonthString(date = new Date()) {
        return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    }

    // 检查今天是否已签到
    isTodayCheckedIn() {
        return this.checkInData.lastCheckInDate === this.getTodayString();
    }

    // 执行签到
    doCheckIn() {
        if (this.isTodayCheckedIn()) {
            this.showMessage('今天已经签到过了！', 'warning');
            return null;
        }

        const today = this.getTodayString();
        const month = this.getMonthString();
        
        // 更新签到数据
        this.checkInData.totalCheckInDays++;
        this.checkInData.continuousCheckInDays++;
        this.checkInData.lastCheckInDate = today;
        this.checkInData.checkInHistory.push({
            date: today,
            timestamp: Date.now(),
            rewards: null
        });

        // 记录本月签到
        if (!this.checkInData.monthlyCheckIn[month]) {
            this.checkInData.monthlyCheckIn[month] = [];
        }
        const day = new Date().getDate();
        if (!this.checkInData.monthlyCheckIn[month].includes(day)) {
            this.checkInData.monthlyCheckIn[month].push(day);
        }

        // 计算签到奖励
        const rewards = this.calculateRewards();
        this.checkInData.checkInHistory[this.checkInData.checkInHistory.length - 1].rewards = rewards;
        
        // 发放奖励
        this.grantRewards(rewards);

        // 保存数据
        this.saveCheckInData();

        // 更新扩展成就系统统计
        if (this.game.extendedAchievementSystem) {
            this.game.extendedAchievementSystem.updateStat('checkInDays', 1);
        }

        // 显示签到成功提示
        this.showCheckInSuccess(rewards);

        console.log('[签到系统] 签到成功:', rewards);
        return rewards;
    }

    // 计算签到奖励
    calculateRewards() {
        const continuous = this.checkInData.continuousCheckInDays;
        const total = this.checkInData.totalCheckInDays;
        
        // 基础奖励
        let gold = 50 + continuous * 10;
        let exp = 20 + continuous * 5;
        let items = [];

        // 连续签到奖励
        if (continuous >= 7) {
            gold += 200;
            exp += 100;
            items.push({ id: 'hp_potion_large', name: '大型生命药水', count: 2 });
        }
        if (continuous >= 14) {
            gold += 500;
            exp += 300;
            items.push({ id: 'mp_potion_large', name: '大型魔法药水', count: 2 });
        }
        if (continuous >= 30) {
            gold += 1000;
            exp += 500;
            items.push({ id: 'rare_chest', name: '稀有宝箱', count: 1 });
        }
        if (continuous >= 100) {
            gold += 5000;
            exp += 2000;
            items.push({ id: 'legendary_chest', name: '传说宝箱', count: 1 });
        }

        // 累计签到奖励
        if (total % 10 === 0) {
            gold += 100;
            exp += 50;
        }
        if (total % 50 === 0) {
            gold += 500;
            exp += 200;
            items.push({ id: 'special_chest', name: '特殊宝箱', count: 1 });
        }
        if (total % 100 === 0) {
            gold += 2000;
            exp += 1000;
            items.push({ id: 'epic_chest', name: '史诗宝箱', count: 1 });
        }

        // 随机奖励（10%概率获得额外物品）
        if (Math.random() < 0.1) {
            const randomItems = [
                { id: 'hp_potion', name: '生命药水', count: 3 },
                { id: 'mp_potion', name: '魔法药水', count: 3 },
                { id: 'scroll', name: '神秘卷轴', count: 1 },
                { id: 'gem', name: '宝石', count: 2 }
            ];
            const randomItem = randomItems[Math.floor(Math.random() * randomItems.length)];
            items.push(randomItem);
        }

        return { gold, exp, items, continuous, total };
    }

    // 发放奖励
    grantRewards(rewards) {
        // 发放金币
        if (this.game.player && this.game.player.gold !== undefined) {
            this.game.player.gold += rewards.gold;
        }
        
        // 发放经验
        if (this.game.player && this.game.player.exp !== undefined) {
            this.game.player.exp += rewards.exp;
        }

        // 发放物品（简化处理，记录到累计奖励）
        this.checkInData.totalRewards.gold += rewards.gold;
        this.checkInData.totalRewards.exp += rewards.exp;
        rewards.items.forEach(item => {
            this.checkInData.totalRewards.items.push(item);
        });
    }

    // 补签
    makeUpCheckIn(date) {
        const today = this.getTodayString();
        const targetDate = new Date(date);
        const todayDate = new Date(today);
        
        // 只能补签本月的日期
        if (targetDate.getMonth() !== todayDate.getMonth() || 
            targetDate.getFullYear() !== todayDate.getFullYear()) {
            this.showMessage('只能补签本月的日期！', 'error');
            return false;
        }

        // 不能补签未来的日期
        if (targetDate > todayDate) {
            this.showMessage('不能补签未来的日期！', 'error');
            return false;
        }

        // 检查是否已经签到
        const month = this.getMonthString();
        const day = targetDate.getDate();
        if (this.checkInData.monthlyCheckIn[month] && 
            this.checkInData.monthlyCheckIn[month].includes(day)) {
            this.showMessage('该日期已经签到过了！', 'warning');
            return false;
        }

        // 检查补签次数（每天最多补签1次，每月最多补签5次）
        if (this.checkInData.lastMakeUpDate === today) {
            this.showMessage('今天已经补签过了！', 'warning');
            return false;
        }

        // 消耗补签次数（可以用金币购买补签次数）
        const makeUpCost = 100; // 每次补签消耗100金币
        if (this.game.player && this.game.player.gold >= makeUpCost) {
            this.game.player.gold -= makeUpCost;
        } else {
            this.showMessage(`补签需要 ${makeUpCost} 金币！`, 'error');
            return false;
        }

        // 执行补签
        if (!this.checkInData.monthlyCheckIn[month]) {
            this.checkInData.monthlyCheckIn[month] = [];
        }
        this.checkInData.monthlyCheckIn[month].push(day);
        this.checkInData.makeUpCount++;
        this.checkInData.lastMakeUpDate = today;

        // 补签奖励（减半）
        const rewards = { gold: 25, exp: 10, items: [], isMakeUp: true };
        this.grantRewards(rewards);

        this.saveCheckInData();
        this.showMessage(`补签成功！获得 ${rewards.gold} 金币，${rewards.exp} 经验`, 'success');
        
        return true;
    }

    // 显示签到成功提示
    showCheckInSuccess(rewards) {
        let message = `🎉 签到成功！\n\n`;
        message += `连续签到：${rewards.continuous} 天\n`;
        message += `累计签到：${rewards.total} 天\n\n`;
        message += `💰 金币：+${rewards.gold}\n`;
        message += `⭐ 经验：+${rewards.exp}\n`;
        if (rewards.items.length > 0) {
            message += `🎁 物品：\n`;
            rewards.items.forEach(item => {
                message += `   - ${item.name} x${item.count}\n`;
            });
        }
        
        this.showMessage(message, 'success');
    }

    // 显示消息
    showMessage(text, type = 'info') {
        // 使用游戏的消息系统
        if (this.game.showMessage) {
            this.game.showMessage(text, type);
        } else {
            console.log(`[签到系统] ${type}: ${text}`);
        }
    }

    // 获取本月签到日历数据
    getMonthlyCalendar(year, month) {
        const monthKey = `${year}-${String(month + 1).padStart(2, '0')}`;
        const checkedDays = this.checkInData.monthlyCheckIn[monthKey] || [];
        const daysInMonth = new Date(year, month + 1, 0).getDate();
        const firstDay = new Date(year, month, 1).getDay();
        
        const calendar = [];
        for (let i = 0; i < firstDay; i++) {
            calendar.push({ day: null, checked: false, isToday: false, isFuture: false });
        }
        
        const today = new Date();
        for (let day = 1; day <= daysInMonth; day++) {
            const isToday = today.getFullYear() === year && 
                           today.getMonth() === month && 
                           today.getDate() === day;
            const isFuture = new Date(year, month, day) > today;
            calendar.push({
                day,
                checked: checkedDays.includes(day),
                isToday,
                isFuture,
                canMakeUp: !isFuture && !checkedDays.includes(day)
            });
        }
        
        return calendar;
    }

    // 获取签到统计
    getStats() {
        const today = new Date();
        const month = this.getMonthString();
        const checkedDays = this.checkInData.monthlyCheckIn[month] || [];
        const daysInMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate();
        
        return {
            totalDays: this.checkInData.totalCheckInDays,
            continuousDays: this.checkInData.continuousCheckInDays,
            monthCheckedDays: checkedDays.length,
            monthTotalDays: daysInMonth,
            monthProgress: Math.round((checkedDays.length / daysInMonth) * 100),
            isTodayChecked: this.isTodayCheckedIn(),
            makeUpCount: this.checkInData.makeUpCount,
            totalGold: this.checkInData.totalRewards.gold,
            totalExp: this.checkInData.totalRewards.exp,
            totalItems: this.checkInData.totalRewards.items.length
        };
    }

    // 渲染签到UI
    renderUI(ctx, canvasWidth, canvasHeight) {
        if (!this.uiVisible) return;

        // UI由HTML层处理，这里只做数据准备
    }

    // 切换UI显示
    toggleUI() {
        this.uiVisible = !this.uiVisible;
        if (this.uiVisible) {
            this.showCheckInPanel();
        } else {
            this.hideCheckInPanel();
        }
    }

    // 显示签到面板
    showCheckInPanel() {
        // 检查是否已存在面板
        let panel = document.getElementById('checkin-panel');
        if (panel) {
            panel.style.display = 'flex';
            this.updateCheckInPanel();
            return;
        }

        // 创建面板
        panel = document.createElement('div');
        panel.id = 'checkin-panel';
        panel.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 500px;
            max-height: 80vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            border: 2px solid #e94560;
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

        // 绑定事件
        this.bindPanelEvents(panel);
    }

    // 获取面板HTML
    getPanelHTML() {
        const stats = this.getStats();
        const calendar = this.getMonthlyCalendar(new Date().getFullYear(), new Date().getMonth());
        
        let calendarHTML = '';
        const weekDays = ['日', '一', '二', '三', '四', '五', '六'];
        weekDays.forEach(day => {
            calendarHTML += `<div style="text-align:center;font-weight:bold;color:#e94560;padding:8px;">${day}</div>`;
        });
        
        calendar.forEach(day => {
            if (day.day === null) {
                calendarHTML += `<div></div>`;
            } else {
                let bgColor = 'rgba(255,255,255,0.05)';
                let textColor = 'white';
                let border = '';
                
                if (day.checked) {
                    bgColor = 'linear-gradient(135deg, #e94560, #ff6b6b)';
                }
                if (day.isToday) {
                    border = '2px solid #ffd700';
                }
                if (day.isFuture) {
                    textColor = 'rgba(255,255,255,0.3)';
                }
                
                const clickable = day.canMakeUp ? `onclick="window.checkInSystem.makeUpCheckIn('${new Date().getFullYear()}-${String(new Date().getMonth()+1).padStart(2,'0')}-${String(day.day).padStart(2,'0')}')"` : '';
                const cursor = day.canMakeUp ? 'cursor:pointer;' : '';
                
                calendarHTML += `
                    <div ${clickable} style="
                        ${cursor}
                        background:${bgColor};
                        color:${textColor};
                        ${border ? `border:${border};` : ''}
                        border-radius:8px;
                        padding:12px;
                        text-align:center;
                        font-weight:${day.isToday ? 'bold' : 'normal'};
                        transition:all 0.3s;
                    " onmouseover="this.style.transform='scale(1.1)'" onmouseout="this.style.transform='scale(1)'">
                        ${day.checked ? '✓' : day.day}
                    </div>
                `;
            }
        });

        return `
            <div style="text-align:center;margin-bottom:20px;">
                <h2 style="color:#e94560;margin:0;font-size:28px;">📅 每日签到</h2>
                <p style="color:rgba(255,255,255,0.6);margin:5px 0 0 0;">每日签到领取丰厚奖励</p>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:10px;margin-bottom:20px;">
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#e94560;">${stats.continuousDays}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">连续签到</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#ffd700;">${stats.totalDays}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">累计签到</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#00b894;">${stats.monthCheckedDays}/${stats.monthTotalDays}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">本月签到</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:24px;font-weight:bold;color:#0984e3;">${stats.makeUpCount}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">补签次数</div>
                </div>
            </div>
            
            <div style="margin-bottom:20px;">
                <div style="display:flex;justify-content:space-between;margin-bottom:10px;">
                    <span style="font-weight:bold;">${new Date().getFullYear()}年${new Date().getMonth()+1}月</span>
                    <span style="color:rgba(255,255,255,0.6);">本月进度: ${stats.monthProgress}%</span>
                </div>
                <div style="background:rgba(255,255,255,0.1);height:8px;border-radius:4px;overflow:hidden;">
                    <div style="background:linear-gradient(90deg,#e94560,#ff6b6b);height:100%;width:${stats.monthProgress}%;border-radius:4px;transition:width 0.5s;"></div>
                </div>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(7,1fr);gap:5px;margin-bottom:20px;">
                ${calendarHTML}
            </div>
            
            <div style="display:flex;gap:10px;margin-bottom:20px;">
                <button id="checkin-btn" onclick="window.checkInSystem.doCheckIn()" style="
                    flex:1;
                    padding:15px;
                    border:none;
                    border-radius:10px;
                    font-size:16px;
                    font-weight:bold;
                    cursor:pointer;
                    background:${stats.isTodayChecked ? 'rgba(255,255,255,0.1)' : 'linear-gradient(135deg,#e94560,#ff6b6b)'};
                    color:white;
                    transition:all 0.3s;
                " ${stats.isTodayChecked ? 'disabled' : ''}>
                    ${stats.isTodayChecked ? '✓ 今日已签到' : '🎁 立即签到'}
                </button>
                <button onclick="window.checkInSystem.hideCheckInPanel()" style="
                    padding:15px 25px;
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
            
            <div style="background:rgba(255,255,255,0.03);border-radius:10px;padding:15px;font-size:13px;color:rgba(255,255,255,0.7);">
                <div style="font-weight:bold;margin-bottom:8px;color:#e94560;">📋 签到奖励说明</div>
                <div>• 基础奖励：50金币 + 20经验（每连续签到1天+10金币+5经验）</div>
                <div>• 连续7天：额外200金币+100经验+大型生命药水x2</div>
                <div>• 连续14天：额外500金币+300经验+大型魔法药水x2</div>
                <div>• 连续30天：额外1000金币+500经验+稀有宝箱x1</div>
                <div>• 连续100天：额外5000金币+2000经验+传说宝箱x1</div>
                <div>• 补签：消耗100金币，奖励减半</div>
            </div>
        `;
    }

    // 绑定面板事件
    bindPanelEvents(panel) {
        // 点击面板外部关闭
        panel.addEventListener('click', (e) => {
            if (e.target === panel) {
                this.hideCheckInPanel();
            }
        });
    }

    // 更新签到面板
    updateCheckInPanel() {
        const panel = document.getElementById('checkin-panel');
        if (panel) {
            panel.innerHTML = this.getPanelHTML();
            this.bindPanelEvents(panel);
        }
    }

    // 隐藏签到面板
    hideCheckInPanel() {
        const panel = document.getElementById('checkin-panel');
        if (panel) {
            panel.style.display = 'none';
        }
        this.uiVisible = false;
    }

    // 更新（每帧调用）
    update(deltaTime) {
        // 签到系统不需要每帧更新
    }

    // 重置数据（调试用）
    resetData() {
        if (confirm('确定要重置所有签到数据吗？此操作不可恢复！')) {
            this.checkInData = this.getDefaultData();
            this.saveCheckInData();
            this.showMessage('签到数据已重置', 'info');
        }
    }
}

// 导出到全局
if (typeof window !== 'undefined') {
    window.CheckInSystem = CheckInSystem;
}
