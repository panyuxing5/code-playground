using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.UI
{
    public class ChatSystem
    {
        private readonly List<ChatMessage> messages;
        private readonly Queue<string> commandHistory;
        private string currentInput;
        private bool isChatOpen;
        private int scrollOffset;
        private int maxMessages;
        private int commandHistoryIndex;

        // 事件
        public event Action<string> OnCommandExecuted;
        public event Action<string> OnMessageSent;

        public bool IsChatOpen => isChatOpen;
        public string CurrentInput => currentInput;
        public int MessageCount => messages.Count;

        public ChatSystem()
        {
            messages = new List<ChatMessage>();
            commandHistory = new Queue<string>();
            currentInput = "";
            isChatOpen = false;
            scrollOffset = 0;
            maxMessages = 100;
            commandHistoryIndex = -1;
        }

        public void Initialize()
        {
            Console.WriteLine("[ChatSystem] 聊天系统初始化完成");
            AddSystemMessage("欢迎来到 VoxelCraft!");
            AddSystemMessage("输入 /help 查看可用命令");
        }

        public void Update(float deltaTime)
        {
            // 聊天系统更新逻辑
        }

        public void OpenChat()
        {
            isChatOpen = true;
            currentInput = "";
            scrollOffset = 0;
        }

        public void OpenCommand()
        {
            isChatOpen = true;
            currentInput = "/";
            scrollOffset = 0;
        }

        public void CloseChat()
        {
            isChatOpen = false;
            currentInput = "";
            commandHistoryIndex = -1;
        }

        public void ToggleChat()
        {
            if (isChatOpen)
            {
                CloseChat();
            }
            else
            {
                OpenChat();
            }
        }

        public void AddMessage(string sender, string message, ChatMessageType type = ChatMessageType.Normal)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                Sender = sender,
                Message = message,
                Type = type,
                Timestamp = DateTime.Now,
                Id = Guid.NewGuid()
            };

            messages.Add(chatMessage);

            // 限制消息数量
            while (messages.Count > maxMessages)
            {
                messages.RemoveAt(0);
            }

            // 如果聊天未打开，自动滚动到底部
            if (!isChatOpen)
            {
                scrollOffset = 0;
            }
        }

        public void AddSystemMessage(string message)
        {
            AddMessage("系统", message, ChatMessageType.System);
        }

        public void AddErrorMessage(string message)
        {
            AddMessage("错误", message, ChatMessageType.Error);
        }

        public void AddSuccessMessage(string message)
        {
            AddMessage("成功", message, ChatMessageType.Success);
        }

        public void AddPlayerMessage(string playerName, string message)
        {
            AddMessage(playerName, message, ChatMessageType.Player);
        }

        public void AddPrivateMessage(string sender, string message)
        {
            AddMessage($"[私聊] {sender}", message, ChatMessageType.Private);
        }

        public void AddTeamMessage(string sender, string message)
        {
            AddMessage($"[队伍] {sender}", message, ChatMessageType.Team);
        }

        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(currentInput))
            {
                CloseChat();
                return;
            }

            // 检查是否是命令
            if (currentInput.StartsWith("/"))
            {
                ExecuteCommand(currentInput);
                commandHistory.Enqueue(currentInput);
                if (commandHistory.Count > 50)
                {
                    commandHistory.Dequeue();
                }
            }
            else
            {
                // 普通消息
                AddPlayerMessage(GameSettings.Instance.PlayerName, currentInput);
                OnMessageSent?.Invoke(currentInput);
            }

            currentInput = "";
            commandHistoryIndex = -1;
            CloseChat();
        }

        private void ExecuteCommand(string command)
        {
            OnCommandExecuted?.Invoke(command);

            // 解析命令
            string[] parts = command.Substring(1).Split(' ');
            if (parts.Length == 0) return;

            string commandName = parts[0].ToLower();

            switch (commandName)
            {
                case "help":
                    ShowHelp();
                    break;

                case "clear":
                case "cls":
                    messages.Clear();
                    AddSystemMessage("聊天已清空");
                    break;

                case "say":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        AddPlayerMessage(GameSettings.Instance.PlayerName, message);
                    }
                    break;

                case "me":
                    if (parts.Length > 1)
                    {
                        string action = string.Join(" ", parts, 1, parts.Length - 1);
                        AddMessage("*", $"{GameSettings.Instance.PlayerName} {action}", ChatMessageType.Action);
                    }
                    break;

                case "tell":
                case "msg":
                case "w":
                    if (parts.Length > 2)
                    {
                        string target = parts[1];
                        string message = string.Join(" ", parts, 2, parts.Length - 2);
                        AddPrivateMessage(target, message);
                    }
                    else
                    {
                        AddErrorMessage("用法: /tell <玩家> <消息>");
                    }
                    break;

                case "team":
                case "t":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        AddTeamMessage(GameSettings.Instance.PlayerName, message);
                    }
                    break;

                case "list":
                case "who":
                    AddSystemMessage("在线玩家: " + GameSettings.Instance.PlayerName);
                    break;

                case "time":
                    AddSystemMessage($"当前时间: {DateTime.Now:HH:mm:ss}");
                    break;

                case "coords":
                    AddSystemMessage("使用 F3 查看坐标");
                    break;

                case "seed":
                    AddSystemMessage("世界种子: " + GameSettings.Instance.WorldSeed);
                    break;

                case "version":
                    AddSystemMessage("VoxelCraft v1.0.0 (C# + OpenTK)");
                    break;

                case "gamemode":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"游戏模式已设置为: {parts[1]}");
                    }
                    break;

                case "gamerule":
                    if (parts.Length > 2)
                    {
                        AddSystemMessage($"游戏规则 {parts[1]} 已设置为 {parts[2]}");
                    }
                    break;

                case "give":
                    if (parts.Length > 2)
                    {
                        AddSuccessMessage($"已给予 {parts[1]} {parts[2]}");
                    }
                    break;

                case "tp":
                    if (parts.Length > 3)
                    {
                        AddSuccessMessage($"已传送到 {parts[1]}, {parts[2]}, {parts[3]}");
                    }
                    break;

                case "kill":
                    AddSystemMessage("你死了");
                    break;

                case "weather":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"天气已设置为: {parts[1]}");
                    }
                    break;

                case "time set":
                    if (parts.Length > 2)
                    {
                        AddSystemMessage($"时间已设置为: {parts[2]}");
                    }
                    break;

                case "difficulty":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"难度已设置为: {parts[1]}");
                    }
                    break;

                case "spawnpoint":
                    AddSuccessMessage("重生点已设置");
                    break;

                case "home":
                    AddSystemMessage("正在传送到家...");
                    break;

                case "sethome":
                    AddSuccessMessage("家已设置");
                    break;

                case "back":
                    AddSystemMessage("正在返回上一个位置...");
                    break;

                case "tpa":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"已向 {parts[1]} 发送传送请求");
                    }
                    break;

                case "tpaccept":
                    AddSuccessMessage("传送请求已接受");
                    break;

                case "tpdeny":
                    AddSystemMessage("传送请求已拒绝");
                    break;

                case "pay":
                    if (parts.Length > 2)
                    {
                        AddSuccessMessage($"已向 {parts[1]} 支付 {parts[2]} 金币");
                    }
                    break;

                case "balance":
                case "bal":
                    AddSystemMessage("你的余额: 0 金币");
                    break;

                case "shop":
                    AddSystemMessage("商店功能开发中...");
                    break;

                case "kit":
                    if (parts.Length > 1)
                    {
                        AddSuccessMessage($"已领取工具包: {parts[1]}");
                    }
                    break;

                case "warp":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"正在传送到 {parts[1]}...");
                    }
                    break;

                case "warps":
                    AddSystemMessage("可用传送点: 主城, 资源区, 地狱, 末地");
                    break;

                case "setwarp":
                    if (parts.Length > 1)
                    {
                        AddSuccessMessage($"传送点 {parts[1]} 已设置");
                    }
                    break;

                case "delwarp":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"传送点 {parts[1]} 已删除");
                    }
                    break;

                case "afk":
                    AddSystemMessage($"{GameSettings.Instance.PlayerName} 现在是 AFK 状态");
                    break;

                case "reply":
                case "r":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        AddPrivateMessage("回复", message);
                    }
                    break;

                case "ignore":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"已忽略 {parts[1]}");
                    }
                    break;

                case "unignore":
                    if (parts.Length > 1)
                    {
                        AddSystemMessage($"已取消忽略 {parts[1]}");
                    }
                    break;

                case "ignored":
                    AddSystemMessage("忽略列表: (空)");
                    break;

                case "friend":
                    if (parts.Length > 2)
                    {
                        if (parts[1] == "add")
                        {
                            AddSuccessMessage($"已添加好友: {parts[2]}");
                        }
                        else if (parts[1] == "remove")
                        {
                            AddSystemMessage($"已删除好友: {parts[2]}");
                        }
                        else if (parts[1] == "list")
                        {
                            AddSystemMessage("好友列表: (空)");
                        }
                    }
                    break;

                case "party":
                case "p":
                    if (parts.Length > 1)
                    {
                        if (parts[1] == "invite")
                        {
                            AddSystemMessage($"已邀请 {parts[2]} 加入队伍");
                        }
                        else if (parts[1] == "leave")
                        {
                            AddSystemMessage("已离开队伍");
                        }
                        else if (parts[1] == "list")
                        {
                            AddSystemMessage("队伍成员: (空)");
                        }
                    }
                    break;

                case "report":
                    if (parts.Length > 2)
                    {
                        AddSuccessMessage("举报已提交，感谢你的反馈");
                    }
                    break;

                case "bug":
                    AddSystemMessage("请在 GitHub 上提交 bug 报告");
                    break;

                case "suggest":
                    AddSystemMessage("建议功能开发中...");
                    break;

                case "discord":
                    AddSystemMessage("加入我们的 Discord: discord.gg/voxelcraft");
                    break;

                case "website":
                    AddSystemMessage("访问我们的网站: www.voxelcraft.com");
                    break;

                case "vote":
                    AddSystemMessage("投票链接: vote.voxelcraft.com");
                    break;

                case "reload":
                    AddSystemMessage("正在重新加载配置...");
                    break;

                case "stop":
                    AddSystemMessage("服务器正在关闭...");
                    break;

                case "restart":
                    AddSystemMessage("服务器正在重启...");
                    break;

                default:
                    AddErrorMessage($"未知命令: {commandName}，输入 /help 查看可用命令");
                    break;
            }
        }

        private void ShowHelp()
        {
            AddSystemMessage("=== 可用命令 ===");
            AddSystemMessage("/help - 显示帮助信息");
            AddSystemMessage("/clear - 清空聊天");
            AddSystemMessage("/say <消息> - 发送消息");
            AddSystemMessage("/me <动作> - 执行动作");
            AddSystemMessage("/tell <玩家> <消息> - 发送私聊");
            AddSystemMessage("/team <消息> - 发送队伍消息");
            AddSystemMessage("/list - 列出在线玩家");
            AddSystemMessage("/time - 显示当前时间");
            AddSystemMessage("/seed - 显示世界种子");
            AddSystemMessage("/version - 显示版本信息");
            AddSystemMessage("/gamemode <模式> - 设置游戏模式");
            AddSystemMessage("/gamerule <规则> <值> - 设置游戏规则");
            AddSystemMessage("/give <玩家> <物品> [数量] - 给予物品");
            AddSystemMessage("/tp <x> <y> <z> - 传送到指定位置");
            AddSystemMessage("/kill - 自杀");
            AddSystemMessage("/weather <类型> - 设置天气");
            AddSystemMessage("/difficulty <难度> - 设置难度");
            AddSystemMessage("/spawnpoint - 设置重生点");
            AddSystemMessage("/home - 传送到家");
            AddSystemMessage("/sethome - 设置家");
            AddSystemMessage("/back - 返回上一个位置");
            AddSystemMessage("/tpa <玩家> - 请求传送到玩家");
            AddSystemMessage("/tpaccept - 接受传送请求");
            AddSystemMessage("/tpdeny - 拒绝传送请求");
            AddSystemMessage("/pay <玩家> <金额> - 支付金币");
            AddSystemMessage("/balance - 查看余额");
            AddSystemMessage("/kit <工具包> - 领取工具包");
            AddSystemMessage("/warp <传送点> - 传送到传送点");
            AddSystemMessage("/warps - 列出传送点");
            AddSystemMessage("/afk - 切换 AFK 状态");
            AddSystemMessage("/reply <消息> - 回复私聊");
            AddSystemMessage("/ignore <玩家> - 忽略玩家");
            AddSystemMessage("/friend add/remove/list <玩家> - 好友管理");
            AddSystemMessage("/party invite/leave/list - 队伍管理");
            AddSystemMessage("/report <玩家> <原因> - 举报玩家");
        }

        public void HandleKeyPress(string key)
        {
            if (!isChatOpen) return;

            switch (key)
            {
                case "Escape":
                    CloseChat();
                    break;

                case "Enter":
                    SendMessage();
                    break;

                case "Backspace":
                    if (currentInput.Length > 0)
                    {
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    }
                    break;

                case "Up":
                    // 上一个命令历史
                    if (commandHistory.Count > 0)
                    {
                        string[] historyArray = commandHistory.ToArray();
                        if (commandHistoryIndex < historyArray.Length - 1)
                        {
                            commandHistoryIndex++;
                            currentInput = historyArray[historyArray.Length - 1 - commandHistoryIndex];
                        }
                    }
                    break;

                case "Down":
                    // 下一个命令历史
                    if (commandHistoryIndex > 0)
                    {
                        commandHistoryIndex--;
                        string[] historyArray = commandHistory.ToArray();
                        currentInput = historyArray[historyArray.Length - 1 - commandHistoryIndex];
                    }
                    else
                    {
                        commandHistoryIndex = -1;
                        currentInput = "";
                    }
                    break;

                case "Tab":
                    // 自动补全
                    AutoComplete();
                    break;

                default:
                    if (key.Length == 1)
                    {
                        currentInput += key;
                    }
                    break;
            }
        }

        private void AutoComplete()
        {
            if (string.IsNullOrEmpty(currentInput)) return;

            // 命令自动补全
            if (currentInput.StartsWith("/"))
            {
                string[] commands = {
                    "help", "clear", "say", "me", "tell", "team", "list", "time",
                    "seed", "version", "gamemode", "gamerule", "give", "tp", "kill",
                    "weather", "difficulty", "spawnpoint", "home", "sethome", "back",
                    "tpa", "tpaccept", "tpdeny", "pay", "balance", "kit", "warp",
                    "warps", "afk", "reply", "ignore", "friend", "party", "report"
                };

                string partial = currentInput.Substring(1).ToLower();
                foreach (string command in commands)
                {
                    if (command.StartsWith(partial))
                    {
                        currentInput = "/" + command;
                        break;
                    }
                }
            }
        }

        public void Render(UIManager uiManager)
        {
            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            if (isChatOpen)
            {
                // 聊天输入框
                int inputBoxHeight = 20;
                int inputBoxY = screenHeight - inputBoxHeight - 10;

                uiManager.DrawPanel(2, inputBoxY, screenWidth - 4, inputBoxHeight,
                    new Color4(0.0f, 0.0f, 0.0f, 0.7f));

                string inputText = currentInput + (DateTime.Now.Millisecond % 1000 < 500 ? "_" : "");
                uiManager.DrawText(inputText, 4, inputBoxY + 4, 12, Color4.White);

                // 显示更多消息
                RenderMessages(uiManager, 20, screenHeight - inputBoxHeight - 20, 100);
            }
            else
            {
                // 只显示最近的消息
                RenderMessages(uiManager, 10, screenHeight - 50, 10);
            }
        }

        private void RenderMessages(UIManager uiManager, int maxLines, int bottomY, int visibleCount)
        {
            int startIndex = Math.Max(0, messages.Count - visibleCount - scrollOffset);
            int y = bottomY;

            for (int i = messages.Count - 1 - scrollOffset; i >= startIndex && i >= 0; i--)
            {
                ChatMessage message = messages[i];
                string text = $"[{message.Timestamp:HH:mm}] {message.Sender}: {message.Message}";

                Color4 color = GetMessageColor(message.Type);

                // 消息背景
                int textWidth = uiManager.MeasureText(text, 12);
                uiManager.DrawPanel(2, y - 1, textWidth + 4, 12,
                    new Color4(0.0f, 0.0f, 0.0f, 0.5f));

                uiManager.DrawText(text, 4, y, 12, color);
                y -= 12;

                if (y < 10) break;
            }
        }

        private Color4 GetMessageColor(ChatMessageType type)
        {
            switch (type)
            {
                case ChatMessageType.System:
                    return new Color4(0.7f, 0.7f, 1.0f, 1f);

                case ChatMessageType.Error:
                    return new Color4(1.0f, 0.3f, 0.3f, 1f);

                case ChatMessageType.Success:
                    return new Color4(0.3f, 1.0f, 0.3f, 1f);

                case ChatMessageType.Player:
                    return Color4.White;

                case ChatMessageType.Private:
                    return new Color4(1.0f, 0.7f, 1.0f, 1f);

                case ChatMessageType.Team:
                    return new Color4(0.7f, 1.0f, 0.7f, 1f);

                case ChatMessageType.Action:
                    return new Color4(1.0f, 0.9f, 0.5f, 1f);

                case ChatMessageType.Announcement:
                    return new Color4(1.0f, 0.8f, 0.3f, 1f);

                default:
                    return Color4.White;
            }
        }

        public List<ChatMessage> GetMessages()
        {
            return new List<ChatMessage>(messages);
        }

        public void Clear()
        {
            messages.Clear();
        }

        public void ScrollUp()
        {
            if (scrollOffset < messages.Count - 1)
            {
                scrollOffset++;
            }
        }

        public void ScrollDown()
        {
            if (scrollOffset > 0)
            {
                scrollOffset--;
            }
        }
    }

    public class ChatMessage
    {
        public string Sender;
        public string Message;
        public ChatMessageType Type;
        public DateTime Timestamp;
        public Guid Id;

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm}] {Sender}: {Message}";
        }
    }

    public enum ChatMessageType
    {
        Normal,
        System,
        Error,
        Success,
        Player,
        Private,
        Team,
        Action,
        Announcement
    }
}
