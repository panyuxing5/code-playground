// ==================== 永恒地牢 - 音频系统（增强版） ====================
// 多音符合成、ADSR包络、音效池、淡入淡出、背景音乐编排

const AudioSystem = {
  audioContext: null,
  musicGain: null,
  sfxGain: null,
  masterGain: null,
  currentMusic: null,
  musicPlaying: false,
  muted: false,
  volume: 0.5,
  musicVolume: 0.25,
  sfxVolume: 0.6,

  // 音效池（避免重复创建）
  soundPool: {},
  maxPoolSize: 20,

  // 背景音乐调度
  musicScheduler: null,
  musicNoteIndex: 0,
  musicStartTime: 0,

  // ==================== 初始化 ====================
  init: function() {
    if (this.audioContext) return;

    try {
      this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
      this.masterGain = this.audioContext.createGain();
      this.masterGain.gain.value = this.volume;
      this.masterGain.connect(this.audioContext.destination);

      this.musicGain = this.audioContext.createGain();
      this.musicGain.gain.value = this.musicVolume;
      this.musicGain.connect(this.masterGain);

      this.sfxGain = this.audioContext.createGain();
      this.sfxGain.gain.value = this.sfxVolume;
      this.sfxGain.connect(this.masterGain);

      console.log('[Audio] 音频系统初始化成功');
    } catch (e) {
      console.error('[Audio] 初始化失败:', e);
    }
  },

  // 确保AudioContext已启动（浏览器自动播放策略）
  ensureStarted: function() {
    if (this.audioContext && this.audioContext.state === 'suspended') {
      this.audioContext.resume();
    }
  },

  // ==================== 音效定义（增强版） ====================
  soundEffects: {
    attack: {
      type: 'square',
      notes: [{ freq: 200, time: 0 }, { freq: 150, time: 0.05 }],
      duration: 0.12,
      volume: 0.3,
      attack: 0.005, decay: 0.05, sustain: 0.3, release: 0.05
    },
    hit: {
      type: 'sawtooth',
      notes: [{ freq: 150, time: 0 }, { freq: 100, time: 0.03 }],
      duration: 0.15,
      volume: 0.4,
      attack: 0.003, decay: 0.05, sustain: 0.2, release: 0.08
    },
    crit: {
      type: 'square',
      notes: [{ freq: 400, time: 0 }, { freq: 600, time: 0.05 }, { freq: 800, time: 0.1 }],
      duration: 0.25,
      volume: 0.5,
      attack: 0.005, decay: 0.05, sustain: 0.4, release: 0.1
    },
    hurt: {
      type: 'sawtooth',
      notes: [{ freq: 200, time: 0 }, { freq: 100, time: 0.05 }, { freq: 50, time: 0.1 }],
      duration: 0.25,
      volume: 0.4,
      attack: 0.005, decay: 0.1, sustain: 0.2, release: 0.1
    },
    heal: {
      type: 'sine',
      notes: [{ freq: 523, time: 0 }, { freq: 659, time: 0.08 }, { freq: 784, time: 0.16 }],
      duration: 0.4,
      volume: 0.3,
      attack: 0.02, decay: 0.1, sustain: 0.5, release: 0.15
    },
    levelup: {
      type: 'sine',
      notes: [
        { freq: 523, time: 0 }, { freq: 659, time: 0.1 }, { freq: 784, time: 0.2 },
        { freq: 1047, time: 0.3 }
      ],
      duration: 0.6,
      volume: 0.4,
      attack: 0.02, decay: 0.1, sustain: 0.6, release: 0.2
    },
    pickup: {
      type: 'sine',
      notes: [{ freq: 880, time: 0 }, { freq: 1100, time: 0.05 }],
      duration: 0.12,
      volume: 0.3,
      attack: 0.005, decay: 0.03, sustain: 0.2, release: 0.05
    },
    coin: {
      type: 'square',
      notes: [{ freq: 988, time: 0 }, { freq: 1319, time: 0.05 }],
      duration: 0.12,
      volume: 0.25,
      attack: 0.003, decay: 0.03, sustain: 0.2, release: 0.05
    },
    potion: {
      type: 'sine',
      notes: [{ freq: 440, time: 0 }, { freq: 554, time: 0.08 }, { freq: 659, time: 0.16 }],
      duration: 0.3,
      volume: 0.3,
      attack: 0.02, decay: 0.08, sustain: 0.4, release: 0.1
    },
    spell: {
      type: 'sawtooth',
      notes: [{ freq: 300, time: 0 }, { freq: 400, time: 0.05 }, { freq: 500, time: 0.1 }],
      duration: 0.35,
      volume: 0.3,
      attack: 0.01, decay: 0.1, sustain: 0.4, release: 0.15
    },
    fireball: {
      type: 'sawtooth',
      notes: [{ freq: 200, time: 0 }, { freq: 150, time: 0.1 }, { freq: 100, time: 0.2 }],
      duration: 0.5,
      volume: 0.4,
      attack: 0.01, decay: 0.15, sustain: 0.3, release: 0.2,
      filter: { type: 'lowpass', freq: 800 }
    },
    ice: {
      type: 'sine',
      notes: [{ freq: 800, time: 0 }, { freq: 1000, time: 0.05 }, { freq: 1200, time: 0.1 }],
      duration: 0.35,
      volume: 0.3,
      attack: 0.01, decay: 0.1, sustain: 0.3, release: 0.15,
      filter: { type: 'highpass', freq: 500 }
    },
    lightning: {
      type: 'square',
      notes: [{ freq: 1500, time: 0 }, { freq: 2000, time: 0.02 }, { freq: 1800, time: 0.04 }],
      duration: 0.2,
      volume: 0.4,
      attack: 0.001, decay: 0.05, sustain: 0.2, release: 0.1
    },
    explosion: {
      type: 'sawtooth',
      notes: [{ freq: 100, time: 0 }, { freq: 80, time: 0.05 }, { freq: 50, time: 0.1 }],
      duration: 0.6,
      volume: 0.5,
      attack: 0.005, decay: 0.2, sustain: 0.3, release: 0.3,
      noise: true
    },
    door: {
      type: 'square',
      notes: [{ freq: 100, time: 0 }, { freq: 120, time: 0.1 }],
      duration: 0.35,
      volume: 0.3,
      attack: 0.01, decay: 0.1, sustain: 0.5, release: 0.15
    },
    chest: {
      type: 'sine',
      notes: [{ freq: 523, time: 0 }, { freq: 659, time: 0.08 }, { freq: 784, time: 0.16 }, { freq: 1047, time: 0.24 }],
      duration: 0.4,
      volume: 0.35,
      attack: 0.01, decay: 0.08, sustain: 0.5, release: 0.15
    },
    death: {
      type: 'sawtooth',
      notes: [{ freq: 200, time: 0 }, { freq: 150, time: 0.1 }, { freq: 100, time: 0.2 }, { freq: 50, time: 0.4 }],
      duration: 0.8,
      volume: 0.45,
      attack: 0.01, decay: 0.2, sustain: 0.4, release: 0.3
    },
    victory: {
      type: 'sine',
      notes: [
        { freq: 523, time: 0 }, { freq: 659, time: 0.15 }, { freq: 784, time: 0.3 },
        { freq: 1047, time: 0.45 }, { freq: 784, time: 0.6 }, { freq: 1047, time: 0.75 }
      ],
      duration: 1.0,
      volume: 0.4,
      attack: 0.02, decay: 0.1, sustain: 0.6, release: 0.3
    },
    click: {
      type: 'square',
      notes: [{ freq: 800, time: 0 }],
      duration: 0.06,
      volume: 0.2,
      attack: 0.002, decay: 0.02, sustain: 0.1, release: 0.02
    },
    hover: {
      type: 'sine',
      notes: [{ freq: 600, time: 0 }],
      duration: 0.05,
      volume: 0.1,
      attack: 0.002, decay: 0.02, sustain: 0.1, release: 0.02
    },
    error: {
      type: 'square',
      notes: [{ freq: 200, time: 0 }, { freq: 180, time: 0.1 }],
      duration: 0.25,
      volume: 0.3,
      attack: 0.005, decay: 0.05, sustain: 0.3, release: 0.1
    },
    success: {
      type: 'sine',
      notes: [{ freq: 784, time: 0 }, { freq: 988, time: 0.1 }],
      duration: 0.3,
      volume: 0.3,
      attack: 0.01, decay: 0.08, sustain: 0.4, release: 0.12
    },
    boss: {
      type: 'sawtooth',
      notes: [{ freq: 60, time: 0 }, { freq: 50, time: 0.2 }, { freq: 40, time: 0.4 }],
      duration: 1.2,
      volume: 0.5,
      attack: 0.02, decay: 0.3, sustain: 0.5, release: 0.4,
      noise: true
    },
    dash: {
      type: 'sine',
      notes: [{ freq: 400, time: 0 }, { freq: 600, time: 0.05 }],
      duration: 0.18,
      volume: 0.25,
      attack: 0.005, decay: 0.05, sustain: 0.2, release: 0.08
    },
    block: {
      type: 'square',
      notes: [{ freq: 300, time: 0 }, { freq: 350, time: 0.03 }],
      duration: 0.12,
      volume: 0.3,
      attack: 0.003, decay: 0.03, sustain: 0.2, release: 0.05
    },
    dodge: {
      type: 'sine',
      notes: [{ freq: 500, time: 0 }, { freq: 700, time: 0.05 }],
      duration: 0.12,
      volume: 0.2,
      attack: 0.005, decay: 0.03, sustain: 0.1, release: 0.05
    },
    summon: {
      type: 'sawtooth',
      notes: [{ freq: 150, time: 0 }, { freq: 200, time: 0.1 }, { freq: 250, time: 0.2 }, { freq: 300, time: 0.3 }],
      duration: 0.6,
      volume: 0.35,
      attack: 0.02, decay: 0.15, sustain: 0.4, release: 0.2
    },
    // 新增音效
    sword: {
      type: 'sawtooth',
      notes: [{ freq: 800, time: 0 }, { freq: 400, time: 0.05 }],
      duration: 0.15,
      volume: 0.3,
      attack: 0.002, decay: 0.05, sustain: 0.1, release: 0.08,
      filter: { type: 'bandpass', freq: 1000, q: 5 }
    },
    bow: {
      type: 'sine',
      notes: [{ freq: 600, time: 0 }, { freq: 200, time: 0.08 }],
      duration: 0.15,
      volume: 0.25,
      attack: 0.005, decay: 0.05, sustain: 0.1, release: 0.08
    },
    magic: {
      type: 'sine',
      notes: [{ freq: 400, time: 0 }, { freq: 600, time: 0.05 }, { freq: 800, time: 0.1 }, { freq: 1000, time: 0.15 }],
      duration: 0.3,
      volume: 0.3,
      attack: 0.01, decay: 0.08, sustain: 0.3, release: 0.12
    },
    buff: {
      type: 'sine',
      notes: [{ freq: 440, time: 0 }, { freq: 554, time: 0.08 }, { freq: 659, time: 0.16 }],
      duration: 0.35,
      volume: 0.3,
      attack: 0.02, decay: 0.1, sustain: 0.4, release: 0.15
    },
    debuff: {
      type: 'sawtooth',
      notes: [{ freq: 300, time: 0 }, { freq: 250, time: 0.08 }, { freq: 200, time: 0.16 }],
      duration: 0.35,
      volume: 0.3,
      attack: 0.01, decay: 0.1, sustain: 0.3, release: 0.15
    },
    footsteps: {
      type: 'sine',
      notes: [{ freq: 80, time: 0 }],
      duration: 0.08,
      volume: 0.15,
      attack: 0.005, decay: 0.03, sustain: 0.1, release: 0.04
    },
    trap: {
      type: 'square',
      notes: [{ freq: 1000, time: 0 }, { freq: 500, time: 0.05 }, { freq: 250, time: 0.1 }],
      duration: 0.3,
      volume: 0.4,
      attack: 0.002, decay: 0.1, sustain: 0.2, release: 0.15
    },
    portal: {
      type: 'sine',
      notes: [{ freq: 200, time: 0 }, { freq: 400, time: 0.1 }, { freq: 600, time: 0.2 }, { freq: 800, time: 0.3 }],
      duration: 0.5,
      volume: 0.3,
      attack: 0.02, decay: 0.15, sustain: 0.4, release: 0.2
    }
  },

  // ==================== 背景音乐（增强版，含和弦和低音） ====================
  musicTracks: {
    town: {
      name: '永恒村',
      tempo: 90,
      melody: [
        { note: 'C4', duration: 0.5 }, { note: 'E4', duration: 0.5 }, { note: 'G4', duration: 0.5 }, { note: 'A4', duration: 0.5 },
        { note: 'G4', duration: 0.5 }, { note: 'E4', duration: 0.5 }, { note: 'D4', duration: 0.5 }, { note: 'C4', duration: 0.5 },
        { note: 'F4', duration: 0.5 }, { note: 'A4', duration: 0.5 }, { note: 'C5', duration: 0.5 }, { note: 'B4', duration: 0.5 },
        { note: 'A4', duration: 0.5 }, { note: 'G4', duration: 0.5 }, { note: 'E4', duration: 0.5 }, { note: 'C4', duration: 0.5 }
      ],
      bass: [
        { note: 'C3', duration: 1 }, { note: 'G3', duration: 1 }, { note: 'A3', duration: 1 }, { note: 'F3', duration: 1 },
        { note: 'C3', duration: 1 }, { note: 'G3', duration: 1 }, { note: 'F3', duration: 1 }, { note: 'G3', duration: 1 }
      ],
      chords: [
        { notes: ['C4', 'E4', 'G4'], duration: 1 }, { notes: ['G3', 'B3', 'D4'], duration: 1 },
        { notes: ['A3', 'C4', 'E4'], duration: 1 }, { notes: ['F3', 'A3', 'C4'], duration: 1 },
        { notes: ['C4', 'E4', 'G4'], duration: 1 }, { notes: ['G3', 'B3', 'D4'], duration: 1 },
        { notes: ['F3', 'A3', 'C4'], duration: 1 }, { notes: ['G3', 'B3', 'D4'], duration: 1 }
      ],
      loop: true,
      mood: 'peaceful'
    },
    dungeon: {
      name: '幽暗地牢',
      tempo: 100,
      melody: [
        { note: 'D4', duration: 0.4 }, { note: 'F4', duration: 0.4 }, { note: 'A4', duration: 0.4 }, { note: 'G4', duration: 0.4 },
        { note: 'F4', duration: 0.4 }, { note: 'D4', duration: 0.4 }, { note: 'E4', duration: 0.4 }, { note: 'D4', duration: 0.4 },
        { note: 'C4', duration: 0.4 }, { note: 'E4', duration: 0.4 }, { note: 'G4', duration: 0.4 }, { note: 'A4', duration: 0.4 },
        { note: 'B4', duration: 0.4 }, { note: 'A4', duration: 0.4 }, { note: 'G4', duration: 0.4 }, { note: 'D4', duration: 0.4 }
      ],
      bass: [
        { note: 'D2', duration: 1 }, { note: 'A2', duration: 1 }, { note: 'B2', duration: 1 }, { note: 'G2', duration: 1 },
        { note: 'D2', duration: 1 }, { note: 'A2', duration: 1 }, { note: 'G2', duration: 1 }, { note: 'A2', duration: 1 }
      ],
      chords: [
        { notes: ['D3', 'F3', 'A3'], duration: 1 }, { notes: ['A2', 'C3', 'E3'], duration: 1 },
        { notes: ['B2', 'D3', 'F3'], duration: 1 }, { notes: ['G2', 'B2', 'D3'], duration: 1 },
        { notes: ['D3', 'F3', 'A3'], duration: 1 }, { notes: ['A2', 'C3', 'E3'], duration: 1 },
        { notes: ['G2', 'B2', 'D3'], duration: 1 }, { notes: ['A2', 'C3', 'E3'], duration: 1 }
      ],
      loop: true,
      mood: 'mysterious'
    },
    battle: {
      name: '激烈战斗',
      tempo: 140,
      melody: [
        { note: 'E4', duration: 0.25 }, { note: 'G4', duration: 0.25 }, { note: 'A4', duration: 0.25 }, { note: 'B4', duration: 0.25 },
        { note: 'A4', duration: 0.25 }, { note: 'G4', duration: 0.25 }, { note: 'E4', duration: 0.25 }, { note: 'D4', duration: 0.25 },
        { note: 'E4', duration: 0.25 }, { note: 'F4', duration: 0.25 }, { note: 'G4', duration: 0.25 }, { note: 'A4', duration: 0.25 },
        { note: 'G4', duration: 0.25 }, { note: 'F4', duration: 0.25 }, { note: 'E4', duration: 0.25 }, { note: 'D4', duration: 0.25 }
      ],
      bass: [
        { note: 'E2', duration: 0.5 }, { note: 'E2', duration: 0.5 }, { note: 'E2', duration: 0.5 }, { note: 'E2', duration: 0.5 },
        { note: 'D2', duration: 0.5 }, { note: 'D2', duration: 0.5 }, { note: 'D2', duration: 0.5 }, { note: 'D2', duration: 0.5 }
      ],
      chords: [
        { notes: ['E3', 'G3', 'B3'], duration: 0.5 }, { notes: ['E3', 'G3', 'B3'], duration: 0.5 },
        { notes: ['D3', 'F3', 'A3'], duration: 0.5 }, { notes: ['D3', 'F3', 'A3'], duration: 0.5 }
      ],
      loop: true,
      mood: 'intense'
    },
    boss: {
      name: 'BOSS战',
      tempo: 160,
      melody: [
        { note: 'D4', duration: 0.2 }, { note: 'E4', duration: 0.2 }, { note: 'F4', duration: 0.2 }, { note: 'E4', duration: 0.2 },
        { note: 'D4', duration: 0.2 }, { note: 'C4', duration: 0.2 }, { note: 'D4', duration: 0.2 }, { note: 'E4', duration: 0.2 },
        { note: 'F4', duration: 0.2 }, { note: 'G4', duration: 0.2 }, { note: 'A4', duration: 0.2 }, { note: 'G4', duration: 0.2 },
        { note: 'F4', duration: 0.2 }, { note: 'E4', duration: 0.2 }, { note: 'D4', duration: 0.2 }, { note: 'C4', duration: 0.2 }
      ],
      bass: [
        { note: 'D2', duration: 0.4 }, { note: 'D2', duration: 0.4 }, { note: 'C2', duration: 0.4 }, { note: 'C2', duration: 0.4 },
        { note: 'D2', duration: 0.4 }, { note: 'D2', duration: 0.4 }, { note: 'E2', duration: 0.4 }, { note: 'E2', duration: 0.4 }
      ],
      chords: [
        { notes: ['D3', 'F3', 'A3'], duration: 0.4 }, { notes: ['C3', 'E3', 'G3'], duration: 0.4 },
        { notes: ['D3', 'F3', 'A3'], duration: 0.4 }, { notes: ['E3', 'G3', 'B3'], duration: 0.4 }
      ],
      loop: true,
      mood: 'epic'
    },
    victory: {
      name: '胜利',
      tempo: 100,
      melody: [
        { note: 'C4', duration: 0.5 }, { note: 'E4', duration: 0.5 }, { note: 'G4', duration: 0.5 }, { note: 'C5', duration: 0.5 },
        { note: 'B4', duration: 0.5 }, { note: 'G4', duration: 0.5 }, { note: 'E4', duration: 0.5 }, { note: 'C4', duration: 0.5 }
      ],
      bass: [
        { note: 'C3', duration: 1 }, { note: 'G3', duration: 1 }, { note: 'F3', duration: 1 }, { note: 'G3', duration: 1 }
      ],
      chords: [
        { notes: ['C4', 'E4', 'G4'], duration: 1 }, { notes: ['G3', 'B3', 'D4'], duration: 1 },
        { notes: ['F3', 'A3', 'C4'], duration: 1 }, { notes: ['G3', 'B3', 'D4'], duration: 1 }
      ],
      loop: false,
      mood: 'triumphant'
    },
    title: {
      name: '标题画面',
      tempo: 70,
      melody: [
        { note: 'A3', duration: 1 }, { note: 'C4', duration: 1 }, { note: 'E4', duration: 1 }, { note: 'A4', duration: 1 },
        { note: 'G4', duration: 1 }, { note: 'E4', duration: 1 }, { note: 'C4', duration: 1 }, { note: 'A3', duration: 1 }
      ],
      bass: [
        { note: 'A2', duration: 2 }, { note: 'F2', duration: 2 }, { note: 'D2', duration: 2 }, { note: 'E2', duration: 2 }
      ],
      chords: [
        { notes: ['A3', 'C4', 'E4'], duration: 2 }, { notes: ['F3', 'A3', 'C4'], duration: 2 },
        { notes: ['D3', 'F3', 'A3'], duration: 2 }, { notes: ['E3', 'G3', 'B3'], duration: 2 }
      ],
      loop: true,
      mood: 'epic'
    }
  },

  // 音符频率映射
  noteFrequencies: {
    'C2': 65.41, 'D2': 73.42, 'E2': 82.41, 'F2': 87.31, 'G2': 98.00, 'A2': 110.00, 'B2': 123.47,
    'C3': 130.81, 'D3': 146.83, 'E3': 164.81, 'F3': 174.61, 'G3': 196.00, 'A3': 220.00, 'B3': 246.94,
    'C4': 261.63, 'D4': 293.66, 'E4': 329.63, 'F4': 349.23, 'G4': 392.00, 'A4': 440.00, 'B4': 493.88,
    'C5': 523.25, 'D5': 587.33, 'E5': 659.25, 'F5': 698.46, 'G5': 783.99, 'A5': 880.00, 'B5': 987.77
  },

  // ==================== 播放音效（增强版，支持ADSR和多音符） ====================
  playSound: function(soundName) {
    if (!this.audioContext || this.muted) return;
    this.ensureStarted();

    const sound = this.soundEffects[soundName];
    if (!sound) {
      console.warn('[Audio] 未找到音效:', soundName);
      return;
    }

    const now = this.audioContext.currentTime;

    // 播放每个音符
    for (const noteData of sound.notes) {
      const osc = this.audioContext.createOscillator();
      const gainNode = this.audioContext.createGain();

      osc.type = sound.type;
      osc.frequency.value = noteData.freq;

      // ADSR包络
      const noteTime = now + noteData.time;
      const attack = sound.attack || 0.01;
      const decay = sound.decay || 0.05;
      const sustain = sound.sustain || 0.3;
      const release = sound.release || 0.05;
      const peakVolume = sound.volume * this.sfxVolume;

      gainNode.gain.setValueAtTime(0, noteTime);
      gainNode.gain.linearRampToValueAtTime(peakVolume, noteTime + attack);
      gainNode.gain.linearRampToValueAtTime(peakVolume * sustain, noteTime + attack + decay);
      gainNode.gain.setValueAtTime(peakVolume * sustain, noteTime + sound.duration - release);
      gainNode.gain.linearRampToValueAtTime(0, noteTime + sound.duration);

      // 滤波器
      if (sound.filter) {
        const filter = this.audioContext.createBiquadFilter();
        filter.type = sound.filter.type;
        filter.frequency.value = sound.filter.freq;
        if (sound.filter.q) filter.Q.value = sound.filter.q;
        osc.connect(filter);
        filter.connect(gainNode);
      } else {
        osc.connect(gainNode);
      }

      gainNode.connect(this.sfxGain);

      osc.start(noteTime);
      osc.stop(noteTime + sound.duration + 0.05);
    }

    // 噪声层（用于爆炸等效果）
    if (sound.noise) {
      this.playNoise(now, sound.duration, sound.volume * 0.5);
    }
  },

  // 播放噪声
  playNoise: function(startTime, duration, volume) {
    const bufferSize = this.audioContext.sampleRate * duration;
    const buffer = this.audioContext.createBuffer(1, bufferSize, this.audioContext.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < bufferSize; i++) {
      data[i] = Math.random() * 2 - 1;
    }

    const noise = this.audioContext.createBufferSource();
    noise.buffer = buffer;

    const filter = this.audioContext.createBiquadFilter();
    filter.type = 'lowpass';
    filter.frequency.value = 500;

    const gainNode = this.audioContext.createGain();
    gainNode.gain.setValueAtTime(volume * this.sfxVolume, startTime);
    gainNode.gain.exponentialRampToValueAtTime(0.001, startTime + duration);

    noise.connect(filter);
    filter.connect(gainNode);
    gainNode.connect(this.sfxGain);

    noise.start(startTime);
    noise.stop(startTime + duration);
  },

  // ==================== 背景音乐（增强版，含旋律、低音、和弦） ====================
  playMusic: function(trackName) {
    if (!this.audioContext || this.muted) return;
    this.ensureStarted();

    if (this.currentMusic === trackName && this.musicPlaying) return;

    this.stopMusic();

    const track = this.musicTracks[trackName];
    if (!track) {
      console.warn('[Audio] 未找到音乐:', trackName);
      return;
    }

    this.currentMusic = trackName;
    this.musicPlaying = true;
    this.musicNoteIndex = 0;

    const beatDuration = 60 / track.tempo;
    this.musicStartTime = this.audioContext.currentTime + 0.1;

    // 淡入
    this.musicGain.gain.cancelScheduledValues(this.audioContext.currentTime);
    this.musicGain.gain.setValueAtTime(0, this.audioContext.currentTime);
    this.musicGain.gain.linearRampToValueAtTime(this.musicVolume, this.audioContext.currentTime + 1);

    // 调度音乐
    this.scheduleMusic(track, beatDuration);
  },

  scheduleMusic: function(track, beatDuration) {
    if (!this.musicPlaying || this.currentMusic !== track.name) return;

    const now = this.audioContext.currentTime;
    const scheduleAheadTime = 0.5;

    while (this.musicStartTime < now + scheduleAheadTime) {
      const melodyIndex = this.musicNoteIndex % track.melody.length;
      const bassIndex = this.musicNoteIndex % track.bass.length;
      const chordIndex = Math.floor(this.musicNoteIndex / 2) % track.chords.length;

      const noteTime = this.musicStartTime;

      // 旋律
      const melodyNote = track.melody[melodyIndex];
      this.playMusicNote(melodyNote.note, noteTime, melodyNote.duration * beatDuration, 'triangle', 0.15);

      // 低音
      const bassNote = track.bass[bassIndex];
      this.playMusicNote(bassNote.note, noteTime, bassNote.duration * beatDuration, 'sine', 0.2);

      // 和弦
      if (chordIndex < track.chords.length) {
        const chord = track.chords[chordIndex];
        for (const chordNote of chord.notes) {
          this.playMusicNote(chordNote, noteTime, chord.duration * beatDuration, 'sine', 0.06);
        }
      }

      this.musicStartTime += beatDuration * 0.5; // 八分音符步进
      this.musicNoteIndex++;
    }

    // 继续调度
    this.musicScheduler = setTimeout(() => {
      this.scheduleMusic(track, beatDuration);
    }, 250);
  },

  playMusicNote: function(noteName, startTime, duration, type, volume) {
    const freq = this.noteFrequencies[noteName];
    if (!freq) return;

    const osc = this.audioContext.createOscillator();
    const gainNode = this.audioContext.createGain();

    osc.type = type;
    osc.frequency.value = freq;

    // 简单包络
    gainNode.gain.setValueAtTime(0, startTime);
    gainNode.gain.linearRampToValueAtTime(volume, startTime + 0.02);
    gainNode.gain.setValueAtTime(volume, startTime + duration - 0.05);
    gainNode.gain.linearRampToValueAtTime(0, startTime + duration);

    osc.connect(gainNode);
    gainNode.connect(this.musicGain);

    osc.start(startTime);
    osc.stop(startTime + duration + 0.05);
  },

  stopMusic: function() {
    if (this.musicScheduler) {
      clearTimeout(this.musicScheduler);
      this.musicScheduler = null;
    }

    // 淡出
    if (this.musicGain && this.audioContext) {
      this.musicGain.gain.cancelScheduledValues(this.audioContext.currentTime);
      this.musicGain.gain.setValueAtTime(this.musicGain.gain.value, this.audioContext.currentTime);
      this.musicGain.gain.linearRampToValueAtTime(0, this.audioContext.currentTime + 0.5);
    }

    this.musicPlaying = false;
    this.currentMusic = null;
  },

  // ==================== 音量控制 ====================
  setVolume: function(volume) {
    this.volume = Math.max(0, Math.min(1, volume));
    if (this.masterGain) {
      this.masterGain.gain.value = this.volume;
    }
  },

  setMusicVolume: function(volume) {
    this.musicVolume = Math.max(0, Math.min(1, volume));
    if (this.musicGain && this.musicPlaying) {
      this.musicGain.gain.value = this.musicVolume;
    }
  },

  setSfxVolume: function(volume) {
    this.sfxVolume = Math.max(0, Math.min(1, volume));
  },

  toggleMute: function() {
    this.muted = !this.muted;
    if (this.masterGain) {
      this.masterGain.gain.value = this.muted ? 0 : this.volume;
    }
    return this.muted;
  },

  // ==================== 保存/加载设置 ====================
  saveSettings: function() {
    return {
      volume: this.volume,
      musicVolume: this.musicVolume,
      sfxVolume: this.sfxVolume,
      muted: this.muted
    };
  },

  loadSettings: function(data) {
    if (!data) return;
    this.volume = data.volume || this.volume;
    this.musicVolume = data.musicVolume || this.musicVolume;
    this.sfxVolume = data.sfxVolume || this.sfxVolume;
    this.muted = data.muted || false;
  }
};
