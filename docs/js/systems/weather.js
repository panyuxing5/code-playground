// 永恒地牢 - 天气系统
class WeatherSystem {
    constructor(game) {
        this.game = game;
        this.currentWeather = 'clear';
        this.weatherDuration = 0;
        this.weatherTimer = 0;
        this.particles = [];
        this.maxParticles = 150;
        this.windX = 0;
        this.windY = 0;
        this.lightningTimer = 0;
        this.lightningFlash = 0;
        this.fogOpacity = 0;
        this.ambientLight = 1.0;
        
        this.weatherTypes = {
            clear: { name: '晴朗', color: '#1a1a2e', particleCount: 0, wind: 0, fog: 0, light: 1.0 },
            rain: { name: '小雨', color: '#0f0f1a', particleCount: 100, wind: 1.5, fog: 0.1, light: 0.7 },
            heavy_rain: { name: '暴雨', color: '#0a0a12', particleCount: 150, wind: 3, fog: 0.2, light: 0.5 },
            snow: { name: '小雪', color: '#1a1a2e', particleCount: 80, wind: 0.5, fog: 0.15, light: 0.8 },
            heavy_snow: { name: '大雪', color: '#151520', particleCount: 120, wind: 1, fog: 0.3, light: 0.6 },
            fog: { name: '浓雾', color: '#1a1a2e', particleCount: 0, wind: 0, fog: 0.6, light: 0.7 },
            thunderstorm: { name: '雷暴', color: '#08080f', particleCount: 150, wind: 4, fog: 0.25, light: 0.4 }
        };
        
        this.zoneWeather = {
            dungeon_entrance: ['clear', 'rain', 'fog'],
            crystal_cave: ['clear', 'fog'],
            fire_temple: ['clear', 'heavy_rain'],
            ice_cavern: ['snow', 'heavy_snow', 'fog'],
            shadow_realm: ['fog', 'thunderstorm', 'heavy_rain'],
            ancient_ruins: ['clear', 'rain', 'fog'],
            dragon_lair: ['thunderstorm', 'heavy_rain'],
            secret_garden: ['clear', 'rain', 'snow']
        };
        
        this.init();
    }
    
    init() {
        for (let i = 0; i < this.maxParticles; i++) {
            this.particles.push({
                x: Math.random() * 2000 - 500,
                y: Math.random() * 1200 - 200,
                vx: 0,
                vy: 0,
                size: 2,
                opacity: 0.6,
                active: false
            });
        }
        console.log('[WeatherSystem] 天气系统初始化完成');
    }
    
    setWeather(weatherType) {
        if (!this.weatherTypes[weatherType]) return;
        this.currentWeather = weatherType;
        const config = this.weatherTypes[weatherType];
        this.weatherDuration = 60 + Math.random() * 120; // 60-180秒（deltaTime单位是秒）
        this.weatherTimer = 0;
        this.windX = config.wind * (Math.random() > 0.5 ? 1 : -1);
        this.windY = 0;
        this.lightningTimer = 0;
        this.lightningFlash = 0;
        
        let activeCount = 0;
        for (const p of this.particles) {
            if (activeCount < config.particleCount) {
                p.active = true;
                p.x = Math.random() * 2000 - 500;
                p.y = Math.random() * 1200 - 200;
                p.size = weatherType.includes('snow') ? 3 + Math.random() * 3 : 1 + Math.random() * 2;
                p.opacity = 0.4 + Math.random() * 0.4;
                activeCount++;
            } else {
                p.active = false;
            }
        }
        
        if (this.game && this.game.showNotification) {
            this.game.showNotification(`天气变化：${config.name}`);
        }
        console.log(`[WeatherSystem] 天气变为：${config.name}`);
    }
    
    setWeatherByZone(zoneId) {
        const weathers = this.zoneWeather[zoneId] || ['clear'];
        const weather = weathers[Math.floor(Math.random() * weathers.length)];
        this.setWeather(weather);
    }
    
    update(deltaTime, cameraX, cameraY) {
        const config = this.weatherTypes[this.currentWeather];
        
        this.weatherTimer += deltaTime;
        if (this.weatherTimer >= this.weatherDuration) {
            const weathers = Object.keys(this.weatherTypes);
            const newWeather = weathers[Math.floor(Math.random() * weathers.length)];
            this.setWeather(newWeather);
        }
        
        this.fogOpacity += (config.fog - this.fogOpacity) * 0.01;
        this.ambientLight += (config.light - this.ambientLight) * 0.01;
        
        if (this.currentWeather === 'thunderstorm') {
            this.lightningTimer += deltaTime;
            if (this.lightningTimer > 3 + Math.random() * 5) { // 3-8秒（秒单位）
                this.lightningFlash = 1.0;
                this.lightningTimer = 0;
                if (this.game && this.game.audioSystem) {
                    this.game.audioSystem.playSound('thunder');
                }
            }
            this.lightningFlash *= 0.9;
        }
        
        for (const p of this.particles) {
            if (!p.active) continue;
            
            if (this.currentWeather.includes('rain') || this.currentWeather === 'thunderstorm') {
                p.vy = 15 + Math.random() * 5;
                p.vx = this.windX * 3;
            } else if (this.currentWeather.includes('snow')) {
                p.vy = 2 + Math.random() * 2;
                p.vx = this.windX + Math.sin(p.y * 0.01) * 0.5;
            }
            
            p.x += p.vx * deltaTime * 0.06;
            p.y += p.vy * deltaTime * 0.06;
            
            if (p.y > 1000) {
                p.y = -50;
                p.x = cameraX + Math.random() * 1600 - 200;
            }
            if (p.x > cameraX + 1400) p.x = cameraX - 100;
            if (p.x < cameraX - 200) p.x = cameraX + 1400;
        }
    }
    
    render(ctx, cameraX, cameraY, canvasWidth, canvasHeight) {
        const config = this.weatherTypes[this.currentWeather];
        
        if (this.ambientLight < 1.0) {
            ctx.fillStyle = `rgba(0, 0, 0, ${1 - this.ambientLight})`;
            ctx.fillRect(0, 0, canvasWidth, canvasHeight);
        }
        
        for (const p of this.particles) {
            if (!p.active) continue;
            
            const screenX = p.x - cameraX + canvasWidth / 2;
            const screenY = p.y - cameraY + canvasHeight / 2;
            
            if (screenX < -50 || screenX > canvasWidth + 50 || screenY < -50 || screenY > canvasHeight + 50) continue;
            
            ctx.globalAlpha = p.opacity;
            
            if (this.currentWeather.includes('rain') || this.currentWeather === 'thunderstorm') {
                ctx.strokeStyle = '#88aaff';
                ctx.lineWidth = p.size;
                ctx.beginPath();
                ctx.moveTo(screenX, screenY);
                ctx.lineTo(screenX + this.windX * 2, screenY + 15);
                ctx.stroke();
            } else if (this.currentWeather.includes('snow')) {
                ctx.fillStyle = '#ffffff';
                ctx.beginPath();
                ctx.arc(screenX, screenY, p.size, 0, Math.PI * 2);
                ctx.fill();
            }
        }
        ctx.globalAlpha = 1;
        
        if (this.fogOpacity > 0) {
            const gradient = ctx.createRadialGradient(
                canvasWidth / 2, canvasHeight / 2, 100,
                canvasWidth / 2, canvasHeight / 2, canvasWidth * 0.7
            );
            gradient.addColorStop(0, `rgba(150, 150, 170, 0)`);
            gradient.addColorStop(1, `rgba(150, 150, 170, ${this.fogOpacity})`);
            ctx.fillStyle = gradient;
            ctx.fillRect(0, 0, canvasWidth, canvasHeight);
        }
        
        if (this.lightningFlash > 0.1) {
            ctx.fillStyle = `rgba(255, 255, 255, ${this.lightningFlash * 0.5})`;
            ctx.fillRect(0, 0, canvasWidth, canvasHeight);
        }
    }
    
    getWeatherName() {
        return this.weatherTypes[this.currentWeather]?.name || '晴朗';
    }
    
    getAmbientLight() {
        return this.ambientLight;
    }
}
