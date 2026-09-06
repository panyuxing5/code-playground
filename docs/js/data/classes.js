// ==================== Eternal Dungeon - Class Data Configuration ====================
// 6

const ClassData = {
  // ====================  ====================
  warrior: {
    name: 'Warrior',
    icon: 'class',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 120,
      mp: 30,
      stamina: 100,
      str: 18,
      dex: 10,
      int: 6,
      vit: 16,
      luck: 8
    },
    growthStats: {
      hp: 12,
      mp: 3,
      stamina: 5,
      str: 3,
      dex: 1,
      int: 0.5,
      vit: 2.5,
      luck: 0.5
    },
    derivedStats: {
      damage: 15,
      attackSpeed: 1.0,
      attackRange: 50,
      critChance: 0.1,
      critDamage: 1.5,
      dodge: 0.05,
      armor: 8,
      magicResist: 5,
      moveSpeed: 357,
      hpRegen: 2,
      mpRegen: 1
    },
    skills: [
      {
        id: 'warrior_charge',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 5,
        manaCost: 10,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.5,
          range: 150,
          knockback: 50
        }
      },
      {
        id: 'warrior_shield_bash',
        name: 'Skill',
        icon: 'class',
        key: 'W',
        cooldown: 8,
        manaCost: 15,
        description: 'A powerful adventurer',
        effects: {
          damage: 0.8,
          radius: 100,
          stun: 2
        }
      },
      {
        id: 'warrior_battle_cry',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 15,
        manaCost: 20,
        description: 'A powerful adventurer',
        effects: {
          damageBoost: 1.5,
          speedBoost: 1.2,
          duration: 10
        }
      },
      {
        id: 'warrior_whirlwind',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 12,
        manaCost: 25,
        description: 'A powerful adventurer',
        effects: {
          damage: 2.0,
          radius: 80
        }
      },
      {
        id: 'warrior_berserker',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 60,
        manaCost: 40,
        description: 'A powerful adventurer',
        effects: {
          damageBoost: 2.0,
          attackSpeedBoost: 1.5,
          damageTaken: 1.3,
          duration: 15
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  },

  // ====================  ====================
  mage: {
    name: 'Skill',
    icon: 'class',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 70,
      mp: 100,
      stamina: 80,
      str: 6,
      dex: 10,
      int: 20,
      vit: 8,
      luck: 12
    },
    growthStats: {
      hp: 6,
      mp: 10,
      stamina: 3,
      str: 0.5,
      dex: 1,
      int: 4,
      vit: 1,
      luck: 1
    },
    derivedStats: {
      damage: 12,
      attackSpeed: 0.8,
      attackRange: 300,
      critChance: 0.15,
      critDamage: 2.0,
      dodge: 0.08,
      armor: 3,
      magicResist: 15,
      moveSpeed: 326,
      hpRegen: 1,
      mpRegen: 4
    },
    skills: [
      {
        id: 'mage_fireball',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 2,
        manaCost: 8,
        description: 'A powerful adventurer',
        effects: {
          damage: 2.0,
          radius: 60,
          burn: 3
        }
      },
      {
        id: 'mage_ice_shield',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 10,
        manaCost: 20,
        description: 'A powerful adventurer',
        effects: {
          shieldPercent: 0.5,
          duration: 8
        }
      },
      {
        id: 'mage_blink',
        name: 'Skill',
        icon: 'class',
        key: 'E',
        cooldown: 6,
        manaCost: 15,
        description: 'A powerful adventurer',
        effects: {
          range: 200
        }
      },
      {
        id: 'mage_meteor',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 20,
        manaCost: 40,
        description: 'A powerful adventurer',
        effects: {
          damage: 3.0,
          radius: 120,
          delay: 0.5
        }
      },
      {
        id: 'mage_arcane_storm',
        name: 'Skill',
        icon: 'class',
        key: 'F',
        cooldown: 60,
        manaCost: 60,
        description: 'A powerful adventurer',
        effects: {
          damagePerSec: 1.0,
          radius: 150,
          duration: 5
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  },

  // ====================  ====================
  ranger: {
    name: 'Skill',
    icon: 'class',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 90,
      mp: 60,
      stamina: 120,
      str: 12,
      dex: 20,
      int: 8,
      vit: 12,
      luck: 15
    },
    growthStats: {
      hp: 8,
      mp: 5,
      stamina: 8,
      str: 1.5,
      dex: 3.5,
      int: 1,
      vit: 1.5,
      luck: 1.5
    },
    derivedStats: {
      damage: 14,
      attackSpeed: 1.2,
      attackRange: 350,
      critChance: 0.25,
      critDamage: 1.8,
      dodge: 0.15,
      armor: 5,
      magicResist: 8,
      moveSpeed: 408,
      hpRegen: 1.5,
      mpRegen: 2
    },
    skills: [
      {
        id: 'ranger_multishot',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 4,
        manaCost: 12,
        description: 'A powerful adventurer',
        effects: {
          damage: 0.8,
          arrows: 5,
          spread: 0.6
        }
      },
      {
        id: 'ranger_dodge',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 8,
        manaCost: 10,
        description: 'A powerful adventurer',
        effects: {
          dodgeBoost: 1,
          duration: 3
        }
      },
      {
        id: 'ranger_poison_arrow',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 6,
        manaCost: 15,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.2,
          poison: 5,
          poisonDamage: 5
        }
      },
      {
        id: 'ranger_arrow_rain',
        name: 'Skill',
        icon: 'class',
        key: 'R',
        cooldown: 18,
        manaCost: 30,
        description: 'A powerful adventurer',
        effects: {
          damagePerSec: 0.8,
          radius: 100,
          duration: 3
        }
      },
      {
        id: 'ranger_wolf_form',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 60,
        manaCost: 40,
        description: 'A powerful adventurer',
        effects: {
          speedBoost: 1.5,
          attackSpeedBoost: 2.0,
          duration: 20
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  },

  // ====================  ====================
  rogue: {
    name: 'Skill',
    icon: 'class',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 80,
      mp: 70,
      stamina: 110,
      str: 10,
      dex: 22,
      int: 10,
      vit: 10,
      luck: 18
    },
    growthStats: {
      hp: 7,
      mp: 6,
      stamina: 7,
      str: 1.5,
      dex: 4,
      int: 1,
      vit: 1.2,
      luck: 2
    },
    derivedStats: {
      damage: 16,
      attackSpeed: 1.5,
      attackRange: 45,
      critChance: 0.35,
      critDamage: 2.5,
      dodge: 0.20,
      armor: 4,
      magicResist: 6,
      moveSpeed: 428,
      hpRegen: 1.5,
      mpRegen: 2.5
    },
    skills: [
      {
        id: 'rogue_backstab',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 4,
        manaCost: 10,
        description: 'A powerful adventurer',
        effects: {
          damage: 2.5,
          range: 200,
          guaranteedCrit: true
        }
      },
      {
        id: 'rogue_stealth',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 12,
        manaCost: 20,
        description: 'A powerful adventurer',
        effects: {
          duration: 5
        }
      },
      {
        id: 'rogue_smoke_bomb',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 10,
        manaCost: 15,
        description: 'A powerful adventurer',
        effects: {
          radius: 120,
          confuse: 3
        }
      },
      {
        id: 'rogue_deadly_strike',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 15,
        manaCost: 30,
        description: 'A powerful adventurer',
        effects: {
          nextAttackDamage: 3.0
        }
      },
      {
        id: 'rogue_shadow_dance',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 60,
        manaCost: 50,
        description: 'A powerful adventurer',
        effects: {
          duration: 10,
          freeSkills: ['rogue_backstab']
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  },

  // ==================== ?====================
  paladin: {
    name: 'Skill',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 100,
      mp: 60,
      stamina: 100,
      str: 15,
      dex: 8,
      int: 12,
      vit: 14,
      luck: 10
    },
    growthStats: {
      hp: 10,
      mp: 6,
      stamina: 6,
      str: 2.5,
      dex: 0.8,
      int: 2,
      vit: 2,
      luck: 0.8
    },
    derivedStats: {
      damage: 15,
      attackSpeed: 0.9,
      attackRange: 55,
      critChance: 0.12,
      critDamage: 1.6,
      dodge: 0.08,
      armor: 10,
      magicResist: 12,
      moveSpeed: 336,
      hpRegen: 2.5,
      mpRegen: 2
    },
    skills: [
      {
        id: 'paladin_holy_strike',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 4,
        manaCost: 12,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.5,
          healPercent: 0.1
        }
      },
      {
        id: 'paladin_holy_shield',
        name: 'Skill',
        icon: 'class',
        key: 'W',
        cooldown: 12,
        manaCost: 20,
        description: 'A powerful adventurer',
        effects: {
          invincibility: 3
        }
      },
      {
        id: 'paladin_heal',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 6,
        manaCost: 18,
        description: 'A powerful adventurer',
        effects: {
          healPercent: 0.3
        }
      },
      {
        id: 'paladin_consecration',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 15,
        manaCost: 25,
        description: 'A powerful adventurer',
        effects: {
          damagePerSec: 0.8,
          radius: 100,
          duration: 5
        }
      },
      {
        id: 'paladin_divine_judgment',
        name: 'Skill',
        icon: 'class',
        key: 'F',
        cooldown: 60,
        manaCost: 50,
        description: 'A powerful adventurer',
        effects: {
          damage: 5.0,
          radius: 150,
          stun: 3
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  },

  // ====================  ====================
  necromancer: {
    name: 'Necromancer',
    icon: 'class',
    description: 'A powerful adventurer',
    baseStats: {
      hp: 75,
      mp: 90,
      stamina: 85,
      str: 8,
      dex: 8,
      int: 22,
      vit: 9,
      luck: 10
    },
    growthStats: {
      hp: 7,
      mp: 9,
      stamina: 4,
      str: 0.8,
      dex: 0.8,
      int: 4.5,
      vit: 1.2,
      luck: 0.8
    },
    derivedStats: {
      damage: 13,
      attackSpeed: 0.85,
      attackRange: 280,
      critChance: 0.12,
      critDamage: 1.7,
      dodge: 0.07,
      armor: 4,
      magicResist: 18,
      moveSpeed: 316,
      hpRegen: 1.2,
      mpRegen: 3.5
    },
    skills: [
      {
        id: 'necro_shadow_bolt',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 2,
        manaCost: 6,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.2
        }
      },
      {
        id: 'necro_summon_skeleton',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 10,
        manaCost: 25,
        description: 'A powerful adventurer',
        effects: {
          summon: 'skeleton',
          duration: 30,
          maxSummons: 3
        }
      },
      {
        id: 'necro_life_drain',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 8,
        manaCost: 15,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.0,
          lifesteal: 1.0,
          range: 200
        }
      },
      {
        id: 'necro_corpse_explosion',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 12,
        manaCost: 20,
        description: 'A powerful adventurer',
        effects: {
          damage: 1.5,
          radius: 80,
          useCorpses: true
        }
      },
      {
        id: 'necro_death_knight',
        name: 'Skill',
        icon: 'class',
        key: 'Q',
        cooldown: 90,
        manaCost: 60,
        description: 'A powerful adventurer',
        effects: {
          summon: 'death_knight',
          duration: 60,
          statMultiplier: 0.8
        }
      }
    ],
    talentTree: {
      name: "Talent Tree",
      branches: []
    },
  }
};

window.ClassData = ClassData;

