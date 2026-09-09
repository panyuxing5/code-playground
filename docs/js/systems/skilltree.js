/**
 * 永恒地牢 - 技能树系统
 * 技能树、技能加点、技能升级、被动加成、主动技能
 */

class SkillTreeSystem {
    constructor(game) {
        this.game = game;
        this.skillTreeData = this.loadSkillTreeData();
        this.uiVisible = false;
        this.selectedBranch = 'warrior';
        this.init();
    }

    init() {
        console.log('[技能树系统] 初始化完成');
    }

    // 加载技能树数据
    loadSkillTreeData() {
        const saved = localStorage.getItem('eternal_dungeon_skilltree');
        if (saved) {
            try {
                return JSON.parse(saved);
            } catch (e) {
                console.error('[技能树系统] 加载数据失败:', e);
            }
        }
        return this.getDefaultData();
    }

    // 默认数据
    getDefaultData() {
        return {
            skillPoints: 0,                  // 当前技能点
            totalSkillPoints: 0,              // 总获得技能点
            usedSkillPoints: 0,               // 已使用技能点
            learnedSkills: {},                 // 已学习技能 {skillId: level}
            skillTreeVersion: 1,
            lastResetLevel: 1
        };
    }

    // 保存数据
    saveSkillTreeData() {
        localStorage.setItem('eternal_dungeon_skilltree', JSON.stringify(this.skillTreeData));
    }

    // 获取技能树配置
    getSkillTreeConfig() {
        return {
            // 战士分支
            warrior: {
                name: '战士',
                icon: '⚔️',
                color: '#e74c3c',
                description: '近战物理输出，高血量高防御',
                skills: [
                    {
                        id: 'w1',
                        name: '力量强化',
                        icon: '💪',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 1 },
                        prerequisites: [],
                        description: '永久增加攻击力',
                        effect: { attack: 10 },
                        effectPerLevel: { attack: 10 }
                    },
                    {
                        id: 'w2',
                        name: '坚韧体魄',
                        icon: '🛡️',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 2, y: 1 },
                        prerequisites: [],
                        description: '永久增加生命值和防御力',
                        effect: { hp: 50, defense: 5 },
                        effectPerLevel: { hp: 50, defense: 5 }
                    },
                    {
                        id: 'w3',
                        name: '狂暴打击',
                        icon: '🔥',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1, y: 2 },
                        prerequisites: ['w1'],
                        description: '普通攻击有几率造成双倍伤害',
                        effect: { critChance: 5 },
                        effectPerLevel: { critChance: 5 }
                    },
                    {
                        id: 'w4',
                        name: '钢铁意志',
                        icon: '🏰',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 2, y: 2 },
                        prerequisites: ['w2'],
                        description: '受到伤害时有几率减免50%伤害',
                        effect: { damageReduction: 10 },
                        effectPerLevel: { damageReduction: 10 }
                    },
                    {
                        id: 'w5',
                        name: '战吼',
                        icon: '📢',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1.5, y: 3 },
                        prerequisites: ['w3', 'w4'],
                        description: '主动技能：提升全队攻击力和防御力',
                        isActive: true,
                        cooldown: 30,
                        effect: { teamAttack: 20, teamDefense: 20, duration: 10 },
                        effectPerLevel: { teamAttack: 10, teamDefense: 10 }
                    },
                    {
                        id: 'w6',
                        name: '嗜血',
                        icon: '🩸',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 4 },
                        prerequisites: ['w5'],
                        description: '击杀敌人时恢复生命值',
                        effect: { lifesteal: 5 },
                        effectPerLevel: { lifesteal: 5 }
                    },
                    {
                        id: 'w7',
                        name: '不屈',
                        icon: '💎',
                        maxLevel: 1,
                        currentLevel: 0,
                        position: { x: 2, y: 4 },
                        prerequisites: ['w5'],
                        description: '生命值低于20%时，伤害提升100%',
                        effect: { berserkDamage: 100 },
                        effectPerLevel: { berserkDamage: 100 }
                    }
                ]
            },
            // 法师分支
            mage: {
                name: '法师',
                icon: '🔮',
                color: '#3498db',
                description: '远程魔法输出，高爆发高控制',
                skills: [
                    {
                        id: 'm1',
                        name: '奥术精通',
                        icon: '✨',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 1 },
                        prerequisites: [],
                        description: '永久增加魔法攻击力',
                        effect: { magicAttack: 15 },
                        effectPerLevel: { magicAttack: 15 }
                    },
                    {
                        id: 'm2',
                        name: '法力涌动',
                        icon: '💧',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 2, y: 1 },
                        prerequisites: [],
                        description: '永久增加魔法值和魔法恢复',
                        effect: { mp: 40, mpRegen: 2 },
                        effectPerLevel: { mp: 40, mpRegen: 2 }
                    },
                    {
                        id: 'm3',
                        name: '元素暴击',
                        icon: '⚡',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1, y: 2 },
                        prerequisites: ['m1'],
                        description: '魔法攻击有几率造成暴击',
                        effect: { magicCritChance: 8 },
                        effectPerLevel: { magicCritChance: 8 }
                    },
                    {
                        id: 'm4',
                        name: '冰霜护盾',
                        icon: '❄️',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 2, y: 2 },
                        prerequisites: ['m2'],
                        description: '受到攻击时有几率冻结敌人',
                        effect: { freezeChance: 10 },
                        effectPerLevel: { freezeChance: 10 }
                    },
                    {
                        id: 'm5',
                        name: '流星火雨',
                        icon: '☄️',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1.5, y: 3 },
                        prerequisites: ['m3', 'm4'],
                        description: '主动技能：召唤流星火雨造成范围伤害',
                        isActive: true,
                        cooldown: 45,
                        effect: { aoeDamage: 200, duration: 5 },
                        effectPerLevel: { aoeDamage: 100 }
                    },
                    {
                        id: 'm6',
                        name: '时空扭曲',
                        icon: '🌀',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 4 },
                        prerequisites: ['m5'],
                        description: '减少所有技能冷却时间',
                        effect: { cooldownReduction: 5 },
                        effectPerLevel: { cooldownReduction: 5 }
                    },
                    {
                        id: 'm7',
                        name: '奥术爆发',
                        icon: '💥',
                        maxLevel: 1,
                        currentLevel: 0,
                        position: { x: 2, y: 4 },
                        prerequisites: ['m5'],
                        description: '魔法值满时，下次攻击造成3倍伤害',
                        effect: { maxMpDamage: 200 },
                        effectPerLevel: { maxMpDamage: 200 }
                    }
                ]
            },
            // 游侠分支
            ranger: {
                name: '游侠',
                icon: '🏹',
                color: '#2ecc71',
                description: '远程物理输出，高敏捷高暴击',
                skills: [
                    {
                        id: 'r1',
                        name: '敏捷强化',
                        icon: '👟',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 1 },
                        prerequisites: [],
                        description: '永久增加攻击速度和移动速度',
                        effect: { attackSpeed: 5, moveSpeed: 3 },
                        effectPerLevel: { attackSpeed: 5, moveSpeed: 3 }
                    },
                    {
                        id: 'r2',
                        name: '精准射击',
                        icon: '🎯',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 2, y: 1 },
                        prerequisites: [],
                        description: '永久增加暴击率和暴击伤害',
                        effect: { critChance: 5, critDamage: 10 },
                        effectPerLevel: { critChance: 5, critDamage: 10 }
                    },
                    {
                        id: 'r3',
                        name: '闪避大师',
                        icon: '💨',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1, y: 2 },
                        prerequisites: ['r1'],
                        description: '有几率闪避敌人攻击',
                        effect: { dodgeChance: 8 },
                        effectPerLevel: { dodgeChance: 8 }
                    },
                    {
                        id: 'r4',
                        name: '穿透射击',
                        icon: '🏹',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 2, y: 2 },
                        prerequisites: ['r2'],
                        description: '攻击有几率穿透敌人',
                        effect: { pierceChance: 15 },
                        effectPerLevel: { pierceChance: 15 }
                    },
                    {
                        id: 'r5',
                        name: '箭雨',
                        icon: '🌧️',
                        maxLevel: 3,
                        currentLevel: 0,
                        position: { x: 1.5, y: 3 },
                        prerequisites: ['r3', 'r4'],
                        description: '主动技能：射出箭雨造成范围伤害',
                        isActive: true,
                        cooldown: 25,
                        effect: { aoeDamage: 150, arrowCount: 10 },
                        effectPerLevel: { aoeDamage: 75, arrowCount: 5 }
                    },
                    {
                        id: 'r6',
                        name: '致命一击',
                        icon: '☠️',
                        maxLevel: 5,
                        currentLevel: 0,
                        position: { x: 1, y: 4 },
                        prerequisites: ['r5'],
                        description: '暴击时有几率造成即死效果',
                        effect: { instantKillChance: 3 },
                        effectPerLevel: { instantKillChance: 3 }
                    },
                    {
                        id: 'r7',
                        name: '风之化身',
                        icon: '🌪️',
                        maxLevel: 1,
                        currentLevel: 0,
                        position: { x: 2, y: 4 },
                        prerequisites: ['r5'],
                        description: '生命值低于30%时，移动速度和攻击速度翻倍',
                        effect: { lowHpSpeed: 100 },
                        effectPerLevel: { lowHpSpeed: 100 }
                    }
                ]
            }
        };
    }

    // 获取技能点
    getSkillPoints() {
        return this.skillTreeData.skillPoints;
    }

    // 添加技能点（升级时调用）
    addSkillPoints(amount = 1) {
        this.skillTreeData.skillPoints += amount;
        this.skillTreeData.totalSkillPoints += amount;
        this.saveSkillTreeData();
        console.log(`[技能树系统] 获得 ${amount} 技能点，当前: ${this.skillTreeData.skillPoints}`);
    }

    // 学习技能
    learnSkill(branch, skillId) {
        const config = this.getSkillTreeConfig();
        const branchConfig = config[branch];
        if (!branchConfig) return { success: false, reason: 'invalid_branch' };

        const skill = branchConfig.skills.find(s => s.id === skillId);
        if (!skill) return { success: false, reason: 'skill_not_found' };

        // 检查是否已学满
        const currentLevel = this.skillTreeData.learnedSkills[skillId] || 0;
        if (currentLevel >= skill.maxLevel) {
            return { success: false, reason: 'max_level' };
        }

        // 检查前置技能
        for (const prereqId of skill.prerequisites) {
            const prereqLevel = this.skillTreeData.learnedSkills[prereqId] || 0;
            if (prereqLevel === 0) {
                return { success: false, reason: 'prerequisite_not_met', prerequisite: prereqId };
            }
        }

        // 检查技能点
        if (this.skillTreeData.skillPoints < 1) {
            return { success: false, reason: 'no_skill_points' };
        }

        // 学习技能
        this.skillTreeData.skillPoints--;
        this.skillTreeData.usedSkillPoints++;
        this.skillTreeData.learnedSkills[skillId] = currentLevel + 1;
        this.saveSkillTreeData();

        // 应用技能效果
        this.applySkillEffect(skill, currentLevel + 1);

        // 更新扩展成就系统统计
        if (this.game.extendedAchievementSystem) {
            this.game.extendedAchievementSystem.updateStat('skillsLearned', 1);
        }

        console.log(`[技能树系统] 学习技能 ${skill.name} 到等级 ${currentLevel + 1}`);
        return { 
            success: true, 
            skill, 
            newLevel: currentLevel + 1,
            remainingPoints: this.skillTreeData.skillPoints
        };
    }

    // 应用技能效果
    applySkillEffect(skill, level) {
        if (!this.game.player) return;

        const effect = skill.effectPerLevel || skill.effect;
        const totalEffect = {};
        
        for (const key in effect) {
            totalEffect[key] = effect[key] * level;
        }

        // 应用到玩家属性（简化处理）
        if (totalEffect.attack && this.game.player.attack !== undefined) {
            // 攻击力加成在伤害计算时处理
        }
        if (totalEffect.hp && this.game.player.maxHp !== undefined) {
            // 生命值加成在初始化时处理
        }
    }

    // 获取技能等级
    getSkillLevel(skillId) {
        return this.skillTreeData.learnedSkills[skillId] || 0;
    }

    // 检查技能是否可学习
    canLearnSkill(branch, skillId) {
        const config = this.getSkillTreeConfig();
        const branchConfig = config[branch];
        if (!branchConfig) return false;

        const skill = branchConfig.skills.find(s => s.id === skillId);
        if (!skill) return false;

        const currentLevel = this.getSkillLevel(skillId);
        if (currentLevel >= skill.maxLevel) return false;
        if (this.skillTreeData.skillPoints < 1) return false;

        for (const prereqId of skill.prerequisites) {
            if (this.getSkillLevel(prereqId) === 0) return false;
        }

        return true;
    }

    // 重置技能树
    resetSkillTree() {
        const refundPoints = this.skillTreeData.usedSkillPoints;
        this.skillTreeData.skillPoints += refundPoints;
        this.skillTreeData.usedSkillPoints = 0;
        this.skillTreeData.learnedSkills = {};
        this.saveSkillTreeData();
        
        this.showMessage(`技能树已重置，返还 ${refundPoints} 技能点`, 'info');
        return { success: true, refundedPoints: refundPoints };
    }

    // 获取技能树统计
    getStats() {
        const learnedCount = Object.keys(this.skillTreeData.learnedSkills).length;
        const totalSkills = Object.values(this.getSkillTreeConfig()).reduce((sum, branch) => sum + branch.skills.length, 0);
        
        return {
            skillPoints: this.skillTreeData.skillPoints,
            totalSkillPoints: this.skillTreeData.totalSkillPoints,
            usedSkillPoints: this.skillTreeData.usedSkillPoints,
            learnedSkills: learnedCount,
            totalSkills: totalSkills,
            completionRate: Math.round((learnedCount / totalSkills) * 100)
        };
    }

    // 显示消息
    showMessage(text, type = 'info') {
        if (this.game.showMessage) {
            this.game.showMessage(text, type);
        } else {
            console.log(`[技能树系统] ${type}: ${text}`);
        }
    }

    // 显示技能树面板
    showSkillTreePanel() {
        let panel = document.getElementById('skilltree-panel');
        if (panel) {
            panel.style.display = 'flex';
            this.updateSkillTreePanel();
            return;
        }

        panel = document.createElement('div');
        panel.id = 'skilltree-panel';
        panel.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            width: 800px;
            max-width: 95vw;
            max-height: 90vh;
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            border: 2px solid #6c5ce7;
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
        const config = this.getSkillTreeConfig();

        return `
            <div style="text-align:center;margin-bottom:20px;">
                <h2 style="color:#6c5ce7;margin:0;font-size:28px;">🌟 技能树</h2>
                <p style="color:rgba(255,255,255,0.6);margin:5px 0 0 0;">学习技能，提升实力，打造最强角色</p>
            </div>
            
            <div style="display:grid;grid-template-columns:repeat(4,1fr);gap:10px;margin-bottom:20px;">
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#6c5ce7;">${stats.skillPoints}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">可用技能点</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#00b894;">${stats.learnedSkills}/${stats.totalSkills}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">已学技能</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#fdcb6e;">${stats.completionRate}%</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">完成度</div>
                </div>
                <div style="background:rgba(255,255,255,0.05);border-radius:10px;padding:15px;text-align:center;">
                    <div style="font-size:28px;font-weight:bold;color:#e17055;">${stats.totalSkillPoints}</div>
                    <div style="font-size:12px;color:rgba(255,255,255,0.6);">总获得点数</div>
                </div>
            </div>
            
            <div style="display:flex;gap:10px;margin-bottom:20px;">
                ${Object.entries(config).map(([key, branch]) => `
                    <button onclick="window.skillTreeSystem.selectBranch('${key}')" 
                        id="branch-${key}"
                        style="
                        flex:1;
                        padding:15px;
                        border:2px solid ${this.selectedBranch === key ? branch.color : 'rgba(255,255,255,0.2)'};
                        border-radius:10px;
                        background:${this.selectedBranch === key ? branch.color + '33' : 'rgba(255,255,255,0.05)'};
                        color:white;
                        cursor:pointer;
                        font-size:16px;
                        font-weight:bold;
                        transition:all 0.3s;
                    ">
                        ${branch.icon} ${branch.name}
                    </button>
                `).join('')}
            </div>
            
            <div id="skill-tree-content" style="margin-bottom:20px;">
                ${this.getBranchSkillsHTML(this.selectedBranch)}
            </div>
            
            <div style="display:flex;gap:10px;">
                <button onclick="window.skillTreeSystem.resetSkillTree()" style="
                    flex:1;
                    padding:15px;
                    border:1px solid #e17055;
                    border-radius:10px;
                    background:rgba(225,112,85,0.2);
                    color:#e17055;
                    cursor:pointer;
                    font-size:16px;
                    font-weight:bold;
                ">
                    🔄 重置技能树
                </button>
                <button onclick="window.skillTreeSystem.hideSkillTreePanel()" style="
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

    // 获取分支技能HTML
    getBranchSkillsHTML(branchKey) {
        const config = this.getSkillTreeConfig();
        const branch = config[branchKey];
        if (!branch) return '';

        return `
            <div style="background:${branch.color}11;border:1px solid ${branch.color}33;border-radius:15px;padding:20px;margin-bottom:15px;">
                <div style="display:flex;align-items:center;gap:10px;margin-bottom:15px;">
                    <span style="font-size:32px;">${branch.icon}</span>
                    <div>
                        <div style="font-size:20px;font-weight:bold;color:${branch.color};">${branch.name}</div>
                        <div style="font-size:13px;color:rgba(255,255,255,0.6);">${branch.description}</div>
                    </div>
                </div>
                
                <div style="display:grid;grid-template-columns:repeat(3,1fr);gap:15px;">
                    ${branch.skills.map(skill => this.getSkillCardHTML(skill, branchKey)).join('')}
                </div>
            </div>
        `;
    }

    // 获取技能卡片HTML
    getSkillCardHTML(skill, branchKey) {
        const currentLevel = this.getSkillLevel(skill.id);
        const isMaxed = currentLevel >= skill.maxLevel;
        const canLearn = this.canLearnSkill(branchKey, skill.id);
        const hasPrereq = skill.prerequisites.length === 0 || 
                          skill.prerequisites.every(p => this.getSkillLevel(p) > 0);

        let bgColor = 'rgba(255,255,255,0.05)';
        let borderColor = 'rgba(255,255,255,0.1)';
        let opacity = '1';

        if (currentLevel > 0) {
            bgColor = 'rgba(0,184,148,0.1)';
            borderColor = '#00b894';
        } else if (!hasPrereq) {
            opacity = '0.5';
        } else if (canLearn) {
            borderColor = '#fdcb6e';
        }

        return `
            <div style="
                background:${bgColor};
                border:2px solid ${borderColor};
                border-radius:12px;
                padding:15px;
                opacity:${opacity};
                transition:all 0.3s;
                cursor:${canLearn ? 'pointer' : 'default'};
            " ${canLearn ? `onclick="window.skillTreeSystem.learnSkill('${branchKey}', '${skill.id}'); window.skillTreeSystem.updateSkillTreePanel();"` : ''}>
                <div style="text-align:center;margin-bottom:10px;">
                    <span style="font-size:36px;">${skill.icon}</span>
                </div>
                <div style="text-align:center;font-weight:bold;margin-bottom:5px;">${skill.name}</div>
                <div style="text-align:center;font-size:12px;color:rgba(255,255,255,0.6);margin-bottom:10px;">
                    等级: ${currentLevel}/${skill.maxLevel}
                </div>
                <div style="font-size:11px;color:rgba(255,255,255,0.7);line-height:1.4;margin-bottom:10px;">
                    ${skill.description}
                </div>
                ${skill.isActive ? `<div style="font-size:10px;color:#fdcb6e;text-align:center;">主动技能 · 冷却${skill.cooldown}秒</div>` : ''}
                ${isMaxed ? '<div style="text-align:center;color:#00b894;font-weight:bold;font-size:12px;">已满级</div>' : 
                  canLearn ? '<div style="text-align:center;color:#fdcb6e;font-weight:bold;font-size:12px;">点击学习</div>' :
                  !hasPrereq ? '<div style="text-align:center;color:#e17055;font-size:11px;">需要前置技能</div>' :
                  '<div style="text-align:center;color:rgba(255,255,255,0.4);font-size:11px;">技能点不足</div>'}
            </div>
        `;
    }

    // 选择分支
    selectBranch(branchKey) {
        this.selectedBranch = branchKey;
        this.updateSkillTreePanel();
    }

    // 更新面板
    updateSkillTreePanel() {
        const panel = document.getElementById('skilltree-panel');
        if (panel) {
            panel.innerHTML = this.getPanelHTML();
        }
    }

    // 隐藏面板
    hideSkillTreePanel() {
        const panel = document.getElementById('skilltree-panel');
        if (panel) {
            panel.style.display = 'none';
        }
        this.uiVisible = false;
    }

    // 切换UI
    toggleUI() {
        this.uiVisible = !this.uiVisible;
        if (this.uiVisible) {
            this.showSkillTreePanel();
        } else {
            this.hideSkillTreePanel();
        }
    }

    // 更新
    update(deltaTime) {}

    // 重置数据
    resetData() {
        if (confirm('确定要重置所有技能树数据吗？此操作不可恢复！')) {
            this.skillTreeData = this.getDefaultData();
            this.saveSkillTreeData();
            this.showMessage('技能树数据已重置', 'info');
        }
    }
}

// 导出到全局
if (typeof window !== 'undefined') {
    window.SkillTreeSystem = SkillTreeSystem;
}
