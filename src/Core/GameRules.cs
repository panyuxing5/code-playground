using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Core
{
    public class GameRules
    {
        private readonly Dictionary<string, GameRule> rules;

        public GameRules()
        {
            rules = new Dictionary<string, GameRule>();
            InitializeDefaultRules();
        }

        private void InitializeDefaultRules()
        {
            // 玩家相关
            RegisterRule("doDaylightCycle", GameRuleType.Boolean, true, "是否进行昼夜循环");
            RegisterRule("doWeatherCycle", GameRuleType.Boolean, true, "是否进行天气循环");
            RegisterRule("doMobSpawning", GameRuleType.Boolean, true, "是否生成生物");
            RegisterRule("doMobLoot", GameRuleType.Boolean, true, "生物是否掉落物品");
            RegisterRule("doTileDrops", GameRuleType.Boolean, true, "方块是否掉落物品");
            RegisterRule("doEntityDrops", GameRuleType.Boolean, true, "实体是否掉落物品");
            RegisterRule("keepInventory", GameRuleType.Boolean, false, "死亡后是否保留物品栏");
            RegisterRule("naturalRegeneration", GameRuleType.Boolean, true, "是否自然恢复生命值");
            RegisterRule("fallDamage", GameRuleType.Boolean, true, "是否受到摔落伤害");
            RegisterRule("fireDamage", GameRuleType.Boolean, true, "是否受到火焰伤害");
            RegisterRule("drowningDamage", GameRuleType.Boolean, true, "是否受到溺水伤害");
            RegisterRule("freezeDamage", GameRuleType.Boolean, true, "是否受到冰冻伤害");
            RegisterRule("doImmediateRespawn", GameRuleType.Boolean, false, "是否立即重生");
            RegisterRule("showDeathMessages", GameRuleType.Boolean, true, "是否显示死亡消息");
            RegisterRule("doInsomnia", GameRuleType.Boolean, true, "是否失眠（幻翼生成）");

            // 世界相关
            RegisterRule("gamerule", GameRuleType.Boolean, true, "是否启用游戏规则");
            RegisterRule("commandBlockOutput", GameRuleType.Boolean, true, "命令方块是否输出");
            RegisterRule("sendCommandFeedback", GameRuleType.Boolean, true, "是否发送命令反馈");
            RegisterRule("logAdminCommands", GameRuleType.Boolean, true, "是否记录管理员命令");
            RegisterRule("showTags", GameRuleType.Boolean, true, "是否显示标签");
            RegisterRule("reducedDebugInfo", GameRuleType.Boolean, false, "是否减少调试信息");
            RegisterRule("spectatorsGenerateChunks", GameRuleType.Boolean, true, "旁观者是否生成区块");
            RegisterRule("spawnRadius", GameRuleType.Integer, 10, "重生点半径");
            RegisterRule("maxEntityCramming", GameRuleType.Integer, 24, "最大实体挤压数");
            RegisterRule("randomTickSpeed", GameRuleType.Integer, 3, "随机刻速度");
            RegisterRule("doLimitedCrafting", GameRuleType.Boolean, false, "是否限制合成");
            RegisterRule("maxCommandChainLength", GameRuleType.Integer, 65536, "最大命令链长度");
            RegisterRule("announceAdvancements", GameRuleType.Boolean, true, "是否宣布进度");
            RegisterRule("disableElytraMovementCheck", GameRuleType.Boolean, false, "是否禁用鞘翅移动检查");
            RegisterRule("doTraderSpawning", GameRuleType.Boolean, true, "是否生成流浪商人");
            RegisterRule("doWardenSpawning", GameRuleType.Boolean, true, "是否生成监守者");
            RegisterRule("doVinesSpread", GameRuleType.Boolean, true, "藤蔓是否蔓延");
            RegisterRule("snowAccumulationHeight", GameRuleType.Integer, 1, "积雪堆积高度");
            RegisterRule("waterSourceConversion", GameRuleType.Boolean, true, "水是否转化为水源");
            RegisterRule("lavaSourceConversion", GameRuleType.Boolean, false, "熔岩是否转化为熔岩源");
            RegisterRule("globalSoundEvents", GameRuleType.Boolean, true, "是否播放全局声音事件");
            RegisterRule("playersSleepingPercentage", GameRuleType.Integer, 100, "玩家睡觉百分比");
            RegisterRule("forgiveDeadPlayers", GameRuleType.Boolean, true, "是否原谅死亡玩家");
            RegisterRule("universalAnger", GameRuleType.Boolean, false, "是否通用愤怒");
            RegisterRule("allowServerFlight", GameRuleType.Boolean, false, "是否允许服务器飞行");
            RegisterRule("disableRaids", GameRuleType.Boolean, false, "是否禁用袭击");
            RegisterRule("doPatrolSpawning", GameRuleType.Boolean, true, "是否生成巡逻队");
            RegisterRule("tntExplodes", GameRuleType.Boolean, true, "TNT是否爆炸");
            RegisterRule("tntExplosionDropDecay", GameRuleType.Boolean, true, "TNT爆炸掉落衰减");
            RegisterRule("creeperExplosionDropDecay", GameRuleType.Boolean, true, "苦力怕爆炸掉落衰减");
            RegisterRule("witherExplosionDropDecay", GameRuleType.Boolean, true, "凋灵爆炸掉落衰减");
            RegisterRule("otherExplosionDropDecay", GameRuleType.Boolean, true, "其他爆炸掉落衰减");
            RegisterRule("mobExplosionDropDecay", GameRuleType.Boolean, true, "生物爆炸掉落衰减");
            RegisterRule("blockExplosionDropDecay", GameRuleType.Boolean, true, "方块爆炸掉落衰减");
            RegisterRule("doFireTick", GameRuleType.Boolean, true, "火是否蔓延");
            RegisterRule("mobGriefing", GameRuleType.Boolean, true, "生物是否破坏方块");
            RegisterRule("doWardenSpawning", GameRuleType.Boolean, true, "是否生成监守者");
            RegisterRule("doVinesSpread", GameRuleType.Boolean, true, "藤蔓是否蔓延");
            RegisterRule("snowAccumulationHeight", GameRuleType.Integer, 1, "积雪堆积高度");
            RegisterRule("waterSourceConversion", GameRuleType.Boolean, true, "水是否转化为水源");
            RegisterRule("lavaSourceConversion", GameRuleType.Boolean, false, "熔岩是否转化为熔岩源");
            RegisterRule("globalSoundEvents", GameRuleType.Boolean, true, "是否播放全局声音事件");
            RegisterRule("playersSleepingPercentage", GameRuleType.Integer, 100, "玩家睡觉百分比");
            RegisterRule("forgiveDeadPlayers", GameRuleType.Boolean, true, "是否原谅死亡玩家");
            RegisterRule("universalAnger", GameRuleType.Boolean, false, "是否通用愤怒");
            RegisterRule("allowServerFlight", GameRuleType.Boolean, false, "是否允许服务器飞行");
            RegisterRule("disableRaids", GameRuleType.Boolean, false, "是否禁用袭击");
            RegisterRule("doPatrolSpawning", GameRuleType.Boolean, true, "是否生成巡逻队");
            RegisterRule("tntExplodes", GameRuleType.Boolean, true, "TNT是否爆炸");
            RegisterRule("tntExplosionDropDecay", GameRuleType.Boolean, true, "TNT爆炸掉落衰减");
            RegisterRule("creeperExplosionDropDecay", GameRuleType.Boolean, true, "苦力怕爆炸掉落衰减");
            RegisterRule("witherExplosionDropDecay", GameRuleType.Boolean, true, "凋灵爆炸掉落衰减");
            RegisterRule("otherExplosionDropDecay", GameRuleType.Boolean, true, "其他爆炸掉落衰减");
            RegisterRule("mobExplosionDropDecay", GameRuleType.Boolean, true, "生物爆炸掉落衰减");
            RegisterRule("blockExplosionDropDecay", GameRuleType.Boolean, true, "方块爆炸掉落衰减");
            RegisterRule("doFireTick", GameRuleType.Boolean, true, "火是否蔓延");
            RegisterRule("mobGriefing", GameRuleType.Boolean, true, "生物是否破坏方块");
        }

        private void RegisterRule(string name, GameRuleType type, object defaultValue, string description)
        {
            if (!rules.ContainsKey(name))
            {
                rules[name] = new GameRule
                {
                    Name = name,
                    Type = type,
                    Value = defaultValue,
                    DefaultValue = defaultValue,
                    Description = description
                };
            }
        }

        public bool GetBool(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Boolean)
            {
                return (bool)rule.Value;
            }
            return false;
        }

        public int GetInt(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Integer)
            {
                return (int)rule.Value;
            }
            return 0;
        }

        public float GetFloat(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Float)
            {
                return (float)rule.Value;
            }
            return 0f;
        }

        public string GetString(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule))
            {
                return rule.Value.ToString();
            }
            return "";
        }

        public bool SetBool(string name, bool value)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Boolean)
            {
                rule.Value = value;
                return true;
            }
            return false;
        }

        public bool SetInt(string name, int value)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Integer)
            {
                rule.Value = value;
                return true;
            }
            return false;
        }

        public bool SetFloat(string name, float value)
        {
            if (rules.TryGetValue(name, out GameRule rule) && rule.Type == GameRuleType.Float)
            {
                rule.Value = value;
                return true;
            }
            return false;
        }

        public bool SetValue(string name, string value)
        {
            if (!rules.TryGetValue(name, out GameRule rule))
            {
                return false;
            }

            switch (rule.Type)
            {
                case GameRuleType.Boolean:
                    if (bool.TryParse(value, out bool boolValue))
                    {
                        rule.Value = boolValue;
                        return true;
                    }
                    break;

                case GameRuleType.Integer:
                    if (int.TryParse(value, out int intValue))
                    {
                        rule.Value = intValue;
                        return true;
                    }
                    break;

                case GameRuleType.Float:
                    if (float.TryParse(value, out float floatValue))
                    {
                        rule.Value = floatValue;
                        return true;
                    }
                    break;

                case GameRuleType.String:
                    rule.Value = value;
                    return true;
            }

            return false;
        }

        public void ResetRule(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule))
            {
                rule.Value = rule.DefaultValue;
            }
        }

        public void ResetAllRules()
        {
            foreach (GameRule rule in rules.Values)
            {
                rule.Value = rule.DefaultValue;
            }
        }

        public List<string> GetAllRuleNames()
        {
            return new List<string>(rules.Keys);
        }

        public GameRule GetRule(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule))
            {
                return rule;
            }
            return null;
        }

        public bool RuleExists(string name)
        {
            return rules.ContainsKey(name);
        }

        public Dictionary<string, GameRule> GetAllRules()
        {
            return new Dictionary<string, GameRule>(rules);
        }

        public string GetRuleInfo(string name)
        {
            if (rules.TryGetValue(name, out GameRule rule))
            {
                return $"{rule.Name} = {rule.Value} (类型: {rule.Type}, 默认: {rule.DefaultValue}, 描述: {rule.Description})";
            }
            return $"规则 {name} 不存在";
        }

        public void LoadFromDictionary(Dictionary<string, string> savedRules)
        {
            foreach (KeyValuePair<string, string> kvp in savedRules)
            {
                SetValue(kvp.Key, kvp.Value);
            }
        }

        public Dictionary<string, string> SaveToDictionary()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (GameRule rule in rules.Values)
            {
                result[rule.Name] = rule.Value.ToString();
            }
            return result;
        }
    }

    public class GameRule
    {
        public string Name;
        public GameRuleType Type;
        public object Value;
        public object DefaultValue;
        public string Description;

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }

    public enum GameRuleType
    {
        Boolean,
        Integer,
        Float,
        String
    }
}
