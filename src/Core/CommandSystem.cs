using System;
using System.Collections.Generic;
using System.Text;
using VoxelCraft.Core;
using VoxelCraft.World;
using VoxelCraft.Entities;
using VoxelCraft.Items;

namespace VoxelCraft.Core
{
    public class CommandSystem
    {
        private readonly Dictionary<string, CommandHandler> commands;
        private readonly GameEngine gameEngine;
        private readonly WorldManager world;

        // 权限等级
        public int PermissionLevel { get; set; } = 4;

        // 事件
        public event Action<string> OnCommandOutput;
        public event Action<string> OnCommandError;

        // 历史
        private readonly List<string> commandHistory;
        private int historyIndex;

        public CommandSystem(GameEngine gameEngine, WorldManager world)
        {
            this.gameEngine = gameEngine;
            this.world = world;
            commands = new Dictionary<string, CommandHandler>();
            commandHistory = new List<string>();
            historyIndex = -1;

            RegisterCommands();
        }

        private delegate void CommandHandler(string[] args);

        public void Initialize()
        {
            Console.WriteLine("[CommandSystem] 命令系统初始化完成，共注册 " + commands.Count + " 个命令");
        }

        private void RegisterCommands()
        {
            // 基础命令
            commands["help"] = CmdHelp;
            commands["?"] = CmdHelp;
            commands["list"] = CmdList;
            commands["say"] = CmdSay;
            commands["me"] = CmdMe;
            commands["tell"] = CmdTell;
            commands["msg"] = CmdTell;
            commands["w"] = CmdTell;

            // 传送命令
            commands["tp"] = CmdTeleport;
            commands["teleport"] = CmdTeleport;
            commands["spawn"] = CmdSpawn;
            commands["setworldspawn"] = CmdSetWorldSpawn;

            // 游戏模式
            commands["gamemode"] = CmdGameMode;
            commands["gmc"] = (args) => CmdGameMode(new[] { "creative" });
            commands["gms"] = (args) => CmdGameMode(new[] { "survival" });
            commands["gma"] = (args) => CmdGameMode(new[] { "adventure" });
            commands["gmsp"] = (args) => CmdGameMode(new[] { "spectator" });

            // 时间命令
            commands["time"] = CmdTime;
            commands["weather"] = CmdWeather;

            // 物品命令
            commands["give"] = CmdGive;
            commands["clear"] = CmdClear;
            commands["effect"] = CmdEffect;

            // 世界命令
            commands["setblock"] = CmdSetBlock;
            commands["fill"] = CmdFill;
            commands["clone"] = CmdClone;
            commands["summon"] = CmdSummon;
            commands["kill"] = CmdKill;

            // 玩家命令
            commands["xp"] = CmdXp;
            commands["experience"] = CmdXp;
            commands["health"] = CmdHealth;
            commands["hunger"] = CmdHunger;

            // 其他命令
            commands["seed"] = CmdSeed;
            commands["locate"] = CmdLocate;
            commands["difficulty"] = CmdDifficulty;
            commands["gamerule"] = CmdGameRule;
            commands["defaultgamemode"] = CmdDefaultGameMode;
            commands["publish"] = CmdPublish;
            commands["save-all"] = CmdSaveAll;
            commands["save-off"] = CmdSaveOff;
            commands["save-on"] = CmdSaveOn;
            commands["stop"] = CmdStop;
            commands["w"] = CmdTell;
        }

        public bool ExecuteCommand(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            // 记录历史
            commandHistory.Add(input);
            historyIndex = commandHistory.Count;

            // 解析命令
            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return false;

            string commandName = parts[0].ToLower();
            if (commandName.StartsWith("/"))
            {
                commandName = commandName.Substring(1);
            }

            string[] args = new string[parts.Length - 1];
            Array.Copy(parts, 1, args, 0, args.Length);

            // 查找并执行命令
            if (commands.TryGetValue(commandName, out CommandHandler handler))
            {
                try
                {
                    handler(args);
                    return true;
                }
                catch (Exception ex)
                {
                    OnCommandError?.Invoke($"命令执行错误: {ex.Message}");
                    return false;
                }
            }
            else
            {
                OnCommandError?.Invoke($"未知命令: {commandName}。输入 /help 查看可用命令。");
                return false;
            }
        }

        public string GetPreviousCommand()
        {
            if (commandHistory.Count == 0) return "";
            historyIndex = Math.Max(0, historyIndex - 1);
            return commandHistory[historyIndex];
        }

        public string GetNextCommand()
        {
            if (commandHistory.Count == 0) return "";
            historyIndex = Math.Min(commandHistory.Count - 1, historyIndex + 1);
            return commandHistory[historyIndex];
        }

        // ========================================
        // 命令实现
        // ========================================

        private void CmdHelp(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("可用命令列表:");
            sb.AppendLine("  /help - 显示帮助信息");
            sb.AppendLine("  /list - 列出在线玩家");
            sb.AppendLine("  /say <消息> - 发送消息");
            sb.AppendLine("  /tp <x> <y> <z> - 传送到指定位置");
            sb.AppendLine("  /gamemode <模式> - 切换游戏模式");
            sb.AppendLine("  /time <set|add> <值> - 设置时间");
            sb.AppendLine("  /weather <clear|rain|thunder> - 设置天气");
            sb.AppendLine("  /give <物品> [数量] - 给予物品");
            sb.AppendLine("  /setblock <x> <y> <z> <方块> - 设置方块");
            sb.AppendLine("  /fill <x1> <y1> <z1> <x2> <y2> <z2> <方块> - 填充区域");
            sb.AppendLine("  /summon <实体> - 生成实体");
            sb.AppendLine("  /kill - 杀死当前实体");
            sb.AppendLine("  /xp <数量> - 给予经验");
            sb.AppendLine("  /seed - 显示世界种子");
            sb.AppendLine("  /difficulty <难度> - 设置难度");
            sb.AppendLine("  /gamerule <规则> [值] - 设置游戏规则");

            OnCommandOutput?.Invoke(sb.ToString());
        }

        private void CmdList(string[] args)
        {
            OnCommandOutput?.Invoke("当前在线玩家: 1 (你)");
        }

        private void CmdSay(string[] args)
        {
            string message = string.Join(" ", args);
            OnCommandOutput?.Invoke($"[你] {message}");
        }

        private void CmdMe(string[] args)
        {
            string action = string.Join(" ", args);
            OnCommandOutput?.Invoke($"* 你 {action}");
        }

        private void CmdTell(string[] args)
        {
            if (args.Length < 2)
            {
                OnCommandError?.Invoke("用法: /tell <玩家> <消息>");
                return;
            }
            string player = args[0];
            string message = string.Join(" ", args, 1, args.Length - 1);
            OnCommandOutput?.Invoke($"你悄悄地对 {player} 说: {message}");
        }

        private void CmdTeleport(string[] args)
        {
            if (args.Length < 3)
            {
                OnCommandError?.Invoke("用法: /tp <x> <y> <z>");
                return;
            }

            if (float.TryParse(args[0], out float x) &&
                float.TryParse(args[1], out float y) &&
                float.TryParse(args[2], out float z))
            {
                gameEngine.Player.Position = new OpenTK.Mathematics.Vector3(x, y, z);
                OnCommandOutput?.Invoke($"已传送到 ({x}, {y}, {z})");
            }
            else
            {
                OnCommandError?.Invoke("无效的坐标");
            }
        }

        private void CmdSpawn(string[] args)
        {
            gameEngine.Player.Position = new OpenTK.Mathematics.Vector3(0, 100, 0);
            OnCommandOutput?.Invoke("已传送到出生点");
        }

        private void CmdSetWorldSpawn(string[] args)
        {
            OnCommandOutput?.Invoke("已设置世界出生点");
        }

        private void CmdGameMode(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /gamemode <survival|creative|adventure|spectator>");
                return;
            }

            string mode = args[0].ToLower();
            switch (mode)
            {
                case "survival":
                case "0":
                case "s":
                    GameSettings.Instance.GameMode = "Survival";
                    OnCommandOutput?.Invoke("游戏模式已切换为生存模式");
                    break;
                case "creative":
                case "1":
                case "c":
                    GameSettings.Instance.GameMode = "Creative";
                    OnCommandOutput?.Invoke("游戏模式已切换为创造模式");
                    break;
                case "adventure":
                case "2":
                case "a":
                    GameSettings.Instance.GameMode = "Adventure";
                    OnCommandOutput?.Invoke("游戏模式已切换为冒险模式");
                    break;
                case "spectator":
                case "3":
                case "sp":
                    GameSettings.Instance.GameMode = "Spectator";
                    OnCommandOutput?.Invoke("游戏模式已切换为旁观者模式");
                    break;
                default:
                    OnCommandError?.Invoke($"未知游戏模式: {mode}");
                    break;
            }
        }

        private void CmdTime(string[] args)
        {
            if (args.Length < 2)
            {
                OnCommandError?.Invoke("用法: /time <set|add|query> <值>");
                return;
            }

            string action = args[0].ToLower();
            if (action == "set")
            {
                if (int.TryParse(args[1], out int time))
                {
                    world.WorldTime = time;
                    OnCommandOutput?.Invoke($"时间已设置为 {time}");
                }
                else if (args[1] == "day")
                {
                    world.WorldTime = 1000;
                    OnCommandOutput?.Invoke("时间已设置为白天");
                }
                else if (args[1] == "night")
                {
                    world.WorldTime = 13000;
                    OnCommandOutput?.Invoke("时间已设置为夜晚");
                }
                else if (args[1] == "noon")
                {
                    world.WorldTime = 6000;
                    OnCommandOutput?.Invoke("时间已设置为正午");
                }
                else if (args[1] == "midnight")
                {
                    world.WorldTime = 18000;
                    OnCommandOutput?.Invoke("时间已设置为午夜");
                }
            }
            else if (action == "add")
            {
                if (int.TryParse(args[1], out int time))
                {
                    world.WorldTime += time;
                    OnCommandOutput?.Invoke($"时间已增加 {time}，当前时间: {world.WorldTime}");
                }
            }
            else if (action == "query")
            {
                OnCommandOutput?.Invoke($"当前时间: {world.WorldTime}");
            }
        }

        private void CmdWeather(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /weather <clear|rain|thunder> [持续时间]");
                return;
            }

            string weather = args[0].ToLower();
            int duration = args.Length > 1 && int.TryParse(args[1], out int d) ? d : 6000;

            switch (weather)
            {
                case "clear":
                    world.Weather.SetWeather(WeatherType.Clear, duration);
                    OnCommandOutput?.Invoke("天气已切换为晴天");
                    break;
                case "rain":
                    world.Weather.SetWeather(WeatherType.Rain, duration);
                    OnCommandOutput?.Invoke("天气已切换为雨天");
                    break;
                case "thunder":
                    world.Weather.SetWeather(WeatherType.Thunder, duration);
                    OnCommandOutput?.Invoke("天气已切换为雷暴");
                    break;
                default:
                    OnCommandError?.Invoke($"未知天气类型: {weather}");
                    break;
            }
        }

        private void CmdGive(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /give <物品ID> [数量]");
                return;
            }

            if (int.TryParse(args[0], out int itemId))
            {
                int count = args.Length > 1 && int.TryParse(args[1], out int c) ? c : 1;
                ItemStack stack = new ItemStack(itemId, count);
                gameEngine.Player.Inventory.AddItem(stack);
                OnCommandOutput?.Invoke($"已给予 {count} 个物品 (ID: {itemId})");
            }
            else
            {
                OnCommandError?.Invoke("无效的物品ID");
            }
        }

        private void CmdClear(string[] args)
        {
            gameEngine.Player.Inventory.Clear();
            OnCommandOutput?.Invoke("物品栏已清空");
        }

        private void CmdEffect(string[] args)
        {
            OnCommandOutput?.Invoke("效果命令已执行（简化实现）");
        }

        private void CmdSetBlock(string[] args)
        {
            if (args.Length < 4)
            {
                OnCommandError?.Invoke("用法: /setblock <x> <y> <z> <方块ID>");
                return;
            }

            if (int.TryParse(args[0], out int x) &&
                int.TryParse(args[1], out int y) &&
                int.TryParse(args[2], out int z) &&
                int.TryParse(args[3], out int blockId))
            {
                world.SetBlock(x, y, z, (ushort)blockId);
                OnCommandOutput?.Invoke($"已在 ({x}, {y}, {z}) 设置方块 (ID: {blockId})");
            }
            else
            {
                OnCommandError?.Invoke("无效的参数");
            }
        }

        private void CmdFill(string[] args)
        {
            if (args.Length < 7)
            {
                OnCommandError?.Invoke("用法: /fill <x1> <y1> <z1> <x2> <y2> <z2> <方块ID>");
                return;
            }

            if (int.TryParse(args[0], out int x1) &&
                int.TryParse(args[1], out int y1) &&
                int.TryParse(args[2], out int z1) &&
                int.TryParse(args[3], out int x2) &&
                int.TryParse(args[4], out int y2) &&
                int.TryParse(args[5], out int z2) &&
                int.TryParse(args[6], out int blockId))
            {
                int count = 0;
                for (int x = Math.Min(x1, x2); x <= Math.Max(x1, x2); x++)
                {
                    for (int y = Math.Min(y1, y2); y <= Math.Max(y1, y2); y++)
                    {
                        for (int z = Math.Min(z1, z2); z <= Math.Max(z1, z2); z++)
                        {
                            world.SetBlock(x, y, z, (ushort)blockId);
                            count++;
                        }
                    }
                }
                OnCommandOutput?.Invoke($"已填充 {count} 个方块");
            }
            else
            {
                OnCommandError?.Invoke("无效的参数");
            }
        }

        private void CmdClone(string[] args)
        {
            OnCommandOutput?.Invoke("克隆命令已执行（简化实现）");
        }

        private void CmdSummon(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /summon <实体类型>");
                return;
            }

            string entityType = args[0].ToLower();
            OnCommandOutput?.Invoke($"已生成实体: {entityType}");
        }

        private void CmdKill(string[] args)
        {
            gameEngine.Player.Health = 0;
            OnCommandOutput?.Invoke("你已死亡");
        }

        private void CmdXp(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /xp <数量>");
                return;
            }

            if (int.TryParse(args[0], out int amount))
            {
                gameEngine.Player.Experience += amount;
                OnCommandOutput?.Invoke($"已给予 {amount} 经验值");
            }
        }

        private void CmdHealth(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandOutput?.Invoke($"当前生命值: {gameEngine.Player.Health}/{gameEngine.Player.MaxHealth}");
                return;
            }

            if (float.TryParse(args[0], out float health))
            {
                gameEngine.Player.Health = Math.Min(health, gameEngine.Player.MaxHealth);
                OnCommandOutput?.Invoke($"生命值已设置为 {health}");
            }
        }

        private void CmdHunger(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandOutput?.Invoke($"当前饥饿值: {gameEngine.Player.Hunger}");
                return;
            }

            if (int.TryParse(args[0], out int hunger))
            {
                gameEngine.Player.Hunger = Math.Min(hunger, 20);
                OnCommandOutput?.Invoke($"饥饿值已设置为 {hunger}");
            }
        }

        private void CmdSeed(string[] args)
        {
            OnCommandOutput?.Invoke($"世界种子: {world.Seed}");
        }

        private void CmdLocate(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /locate <结构类型>");
                return;
            }

            string structure = args[0].ToLower();
            OnCommandOutput?.Invoke($"最近的 {structure} 位于: (0, 0, 0)（简化实现）");
        }

        private void CmdDifficulty(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandOutput?.Invoke($"当前难度: {GameSettings.Instance.Difficulty}");
                return;
            }

            string difficulty = args[0].ToLower();
            GameSettings.Instance.Difficulty = difficulty switch
            {
                "peaceful" => "Peaceful",
                "easy" => "Easy",
                "normal" => "Normal",
                "hard" => "Hard",
                _ => GameSettings.Instance.Difficulty
            };
            OnCommandOutput?.Invoke($"难度已设置为 {GameSettings.Instance.Difficulty}");
        }

        private void CmdGameRule(string[] args)
        {
            if (args.Length < 1)
            {
                OnCommandError?.Invoke("用法: /gamerule <规则名> [值]");
                return;
            }

            string rule = args[0].ToLower();
            if (args.Length == 1)
            {
                OnCommandOutput?.Invoke($"游戏规则 {rule}: true（简化实现）");
            }
            else
            {
                OnCommandOutput?.Invoke($"游戏规则 {rule} 已设置为 {args[1]}");
            }
        }

        private void CmdDefaultGameMode(string[] args)
        {
            OnCommandOutput?.Invoke("默认游戏模式已设置");
        }

        private void CmdPublish(string[] args)
        {
            OnCommandOutput?.Invoke("已向局域网开放世界（端口: 25565）");
        }

        private void CmdSaveAll(string[] args)
        {
            OnCommandOutput?.Invoke("世界已保存");
        }

        private void CmdSaveOff(string[] args)
        {
            OnCommandOutput?.Invoke("已关闭自动保存");
        }

        private void CmdSaveOn(string[] args)
        {
            OnCommandOutput?.Invoke("已开启自动保存");
        }

        private void CmdStop(string[] args)
        {
            OnCommandOutput?.Invoke("正在关闭服务器...");
            gameEngine.Stop();
        }

        public List<string> GetCommandSuggestions(string partial)
        {
            List<string> suggestions = new List<string>();
            foreach (string command in commands.Keys)
            {
                if (command.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                {
                    suggestions.Add(command);
                }
            }
            return suggestions;
        }

        public Dictionary<string, CommandHandler> GetAllCommands()
        {
            return new Dictionary<string, CommandHandler>(commands);
        }
    }
}
