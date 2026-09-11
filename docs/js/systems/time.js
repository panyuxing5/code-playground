// 永恒地牢 - 时间系统（昼夜循环）
class TimeSystem {
    constructor(game) {
        this.game = game;
        this.gameTime = 8 * 60 * 60;
        this.dayLength = 20 * 60; // 20分钟 = 1200秒（deltaTime单位是秒）
        this.timeScale = (24 * 60 * 60) / this.dayLength;
        this.sunX = 0;
        this.sunY = 0;
        this.moonX = 0;
        this.moonY = 0;
        this.stars = [];
        this.maxStars = 80;
        this.ambientColor = { r: 255, g: 255, b: 255 };
        this.skyColor = { r: 26, g: 26, b: 46 };
        this.currentPhase = 'day';
        this.season = 0;
        this.dayOfYear = 1;
        this.gameDay = 1;
        
        this.phases = {
            dawn: { name: '黎明', start: 5, end: 7, light: 0.7, sky: { r: 255, g: 150, b: 100 } },
            day: { name: '白天', start: 7, end: 17, light: 1.0, sky: { r: 100, g: 150, b: 220 } },
            dusk: { name: '黄昏', start: 17, end: 20, light: 0.6, sky: { r: 255, g: 100, b: 50 } },
            night: { name: '夜晚', start: 20, end: 5, light: 0.3, sky: { r: 10, g: 10, b: 30 } }
        };
        
        this.seasons = ['春季', '夏季', '秋季', '冬季'];
        
        this.init();
    }
    
    init() {
        for (let i = 0; i < this.maxStars; i++) {
            this.stars.push({
                x: Math.random() * 2000,
                y: Math.random() * 600,
                size: 0.5 + Math.random() * 1.5,
                twinkleSpeed: 0.5 + Math.random() * 2,
                twinkleOffset: Math.random() * Math.PI * 2
            });
        }
        console.log('[TimeSystem] 时间系统初始化完成');
    }
    
    update(deltaTime) {
        this.gameTime += deltaTime * this.timeScale;
        
        if (this.gameTime >= 24 * 60 * 60) {
            this.gameTime -= 24 * 60 * 60;
            this.gameDay++;
            this.dayOfYear++;
            if (this.dayOfYear > 90) {
                this.dayOfYear = 1;
                this.season = (this.season + 1) % 4;
                if (this.game && this.game.showNotification) {
                    this.game.showNotification(`季节变化：${this.seasons[this.season]}`);
                }
            }
        }
        
        const hours = this.getHours();
        let newPhase = 'night';
        for (const [phase, config] of Object.entries(this.phases)) {
            if (phase === 'night') {
                if (hours >= 20 || hours < 5) newPhase = 'night';
            } else if (hours >= config.start && hours < config.end) {
                newPhase = phase;
            }
        }
        
        if (newPhase !== this.currentPhase) {
            this.currentPhase = newPhase;
            if (this.game && this.game.showNotification) {
                this.game.showNotification(`进入${this.phases[newPhase].name}`);
            }
        }
        
        this.updateCelestialPositions();
        this.updateAmbientColor();
    }
    
    updateCelestialPositions() {
        const hours = this.getHours();
        const dayProgress = ((hours - 6) / 12) * Math.PI;
        const nightProgress = ((hours - 18 + 24) % 24 / 12) * Math.PI;
        
        this.sunX = Math.cos(dayProgress) * 500;
        this.sunY = -Math.sin(dayProgress) * 300;
        
        this.moonX = Math.cos(nightProgress) * 500;
        this.moonY = -Math.sin(nightProgress) * 300;
    }
    
    updateAmbientColor() {
        const hours = this.getHours();
        let targetLight, targetSky;
        
        if (hours >= 5 && hours < 7) {
            const t = (hours - 5) / 2;
            targetLight = this.lerp(this.phases.night.light, this.phases.dawn.light, t);
            targetSky = this.lerpColor(this.phases.night.sky, this.phases.dawn.sky, t);
        } else if (hours >= 7 && hours < 17) {
            targetLight = this.phases.day.light;
            targetSky = this.phases.day.sky;
        } else if (hours >= 17 && hours < 20) {
            const t = (hours - 17) / 3;
            targetLight = this.lerp(this.phases.day.light, this.phases.dusk.light, t);
            targetSky = this.lerpColor(this.phases.day.sky, this.phases.dusk.sky, t);
        } else {
            targetLight = this.phases.night.light;
            targetSky = this.phases.night.sky;
        }
        
        this.ambientColor.r += (255 * targetLight - this.ambientColor.r) * 0.01;
        this.ambientColor.g += (255 * targetLight - this.ambientColor.g) * 0.01;
        this.ambientColor.b += (255 * targetLight - this.ambientColor.b) * 0.01;
        
        this.skyColor.r += (targetSky.r - this.skyColor.r) * 0.01;
        this.skyColor.g += (targetSky.g - this.skyColor.g) * 0.01;
        this.skyColor.b += (targetSky.b - this.skyColor.b) * 0.01;
    }
    
    render(ctx, canvasWidth, canvasHeight) {
        if (this.currentPhase === 'night' || this.currentPhase === 'dusk' || this.currentPhase === 'dawn') {
            const time = Date.now() * 0.001;
            for (const star of this.stars) {
                const twinkle = 0.5 + 0.5 * Math.sin(time * star.twinkleSpeed + star.twinkleOffset);
                const nightFactor = this.currentPhase === 'night' ? 1 : 0.5;
                ctx.globalAlpha = twinkle * nightFactor;
                ctx.fillStyle = '#ffffff';
                ctx.beginPath();
                ctx.arc(star.x % canvasWidth, star.y, star.size, 0, Math.PI * 2);
                ctx.fill();
            }
            ctx.globalAlpha = 1;
        }
        
        if (this.currentPhase !== 'night') {
            const sunScreenX = canvasWidth / 2 + this.sunX;
            const sunScreenY = canvasHeight * 0.3 + this.sunY;
            if (sunScreenY < canvasHeight) {
                const gradient = ctx.createRadialGradient(sunScreenX, sunScreenY, 0, sunScreenX, sunScreenY, 80);
                gradient.addColorStop(0, 'rgba(255, 255, 200, 0.9)');
                gradient.addColorStop(0.3, 'rgba(255, 200, 100, 0.5)');
                gradient.addColorStop(1, 'rgba(255, 150, 50, 0)');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(sunScreenX, sunScreenY, 80, 0, Math.PI * 2);
                ctx.fill();
                
                ctx.fillStyle = '#ffffcc';
                ctx.beginPath();
                ctx.arc(sunScreenX, sunScreenY, 25, 0, Math.PI * 2);
                ctx.fill();
            }
        }
        
        if (this.currentPhase === 'night' || this.currentPhase === 'dusk') {
            const moonScreenX = canvasWidth / 2 + this.moonX;
            const moonScreenY = canvasHeight * 0.25 + this.moonY;
            if (moonScreenY < canvasHeight && moonScreenY > -100) {
                const gradient = ctx.createRadialGradient(moonScreenX, moonScreenY, 0, moonScreenX, moonScreenY, 60);
                gradient.addColorStop(0, 'rgba(200, 200, 255, 0.6)');
                gradient.addColorStop(1, 'rgba(150, 150, 200, 0)');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(moonScreenX, moonScreenY, 60, 0, Math.PI * 2);
                ctx.fill();
                
                ctx.fillStyle = '#e8e8f0';
                ctx.beginPath();
                ctx.arc(moonScreenX, moonScreenY, 20, 0, Math.PI * 2);
                ctx.fill();
                
                ctx.fillStyle = '#c8c8d8';
                ctx.beginPath();
                ctx.arc(moonScreenX - 5, moonScreenY - 3, 4, 0, Math.PI * 2);
                ctx.arc(moonScreenX + 6, moonScreenY + 5, 3, 0, Math.PI * 2);
                ctx.arc(moonScreenX + 2, moonScreenY - 8, 2, 0, Math.PI * 2);
                ctx.fill();
            }
        }
        
        if (this.ambientColor.r < 250 || this.ambientColor.g < 250 || this.ambientColor.b < 250) {
            ctx.fillStyle = `rgba(0, 0, 30, ${1 - this.ambientColor.r / 255 * 0.7})`;
            ctx.fillRect(0, 0, canvasWidth, canvasHeight);
        }
    }
    
    getHours() {
        return Math.floor(this.gameTime / 3600);
    }
    
    getMinutes() {
        return Math.floor((this.gameTime % 3600) / 60);
    }
    
    getTimeString() {
        const h = this.getHours().toString().padStart(2, '0');
        const m = this.getMinutes().toString().padStart(2, '0');
        return `${h}:${m}`;
    }
    
    getPhaseName() {
        return this.phases[this.currentPhase]?.name || '白天';
    }
    
    getSeasonName() {
        return this.seasons[this.season];
    }
    
    getAmbientLight() {
        return this.ambientColor.r / 255;
    }
    
    isNight() {
        return this.currentPhase === 'night';
    }
    
    lerp(a, b, t) {
        return a + (b - a) * t;
    }
    
    lerpColor(a, b, t) {
        return {
            r: this.lerp(a.r, b.r, t),
            g: this.lerp(a.g, b.g, t),
            b: this.lerp(a.b, b.b, t)
        };
    }
}
