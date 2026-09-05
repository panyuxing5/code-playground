using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;

namespace VoxelCraft.UI
{
    public class DeathScreen
    {
        private float deathTime;
        private bool isActive;
        private string deathMessage;
        private string deathCause;

        // 统计
        public int TotalDeaths { get; private set; }
        public Dictionary<string, int> DeathCauses { get; private set; }

        // 事件
        public event Action OnRespawn;
        public event Action OnTitleScreen;
        public event Action OnSpectator;

        // 动画
        private float fadeInTime = 2.0f;
        private float buttonDelay = 3.0f;

        public DeathScreen()
        {
            DeathCauses = new Dictionary<string, int>();
            deathMessage = "你死了！";
        }

        public void Show(string cause = "unknown")
        {
            isActive = true;
            deathTime = 0;
            deathCause = cause;
            TotalDeaths++;

            if (!DeathCauses.ContainsKey(cause))
            {
                DeathCauses[cause] = 0;
            }
            DeathCauses[cause]++;

            deathMessage = GetDeathMessage(cause);
            Console.WriteLine($"[DeathScreen] 玩家死亡: {cause} - {deathMessage}");
        }

        public void Hide()
        {
            isActive = false;
        }

        public void Update(float deltaTime)
        {
            if (!isActive) return;

            deathTime += deltaTime;
        }

        public void Render(UIManager uiManager)
        {
            if (!isActive) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 渐入效果
            float alpha = Math.Min(1.0f, deathTime / fadeInTime);

            // 红色背景覆盖
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight,
                new Color4(0.5f, 0f, 0f, 0.75f * alpha));

            // 死亡标题
            string title = "你死了！";
            int titleWidth = uiManager.MeasureText(title, 48);
            uiManager.DrawText(title,
                (screenWidth - titleWidth) / 2,
                screenHeight / 2 - 80,
                48,
                new Color4(1f, 1f, 1f, alpha));

            // 死亡消息
            int messageWidth = uiManager.MeasureText(deathMessage, 20);
            uiManager.DrawText(deathMessage,
                (screenWidth - messageWidth) / 2,
                screenHeight / 2 - 20,
                20,
                new Color4(1f, 1f, 1f, alpha * 0.9f));

            // 死亡统计
            string stats = $"总死亡次数: {TotalDeaths}";
            int statsWidth = uiManager.MeasureText(stats, 16);
            uiManager.DrawText(stats,
                (screenWidth - statsWidth) / 2,
                screenHeight / 2 + 10,
                16,
                new Color4(0.9f, 0.9f, 0.9f, alpha * 0.8f));

            // 按钮（延迟显示）
            if (deathTime >= buttonDelay)
            {
                float buttonAlpha = Math.Min(1.0f, (deathTime - buttonDelay) / 1.0f);

                // 重生按钮
                int respawnBtnWidth = 200;
                int respawnBtnHeight = 40;
                int respawnBtnX = (screenWidth - respawnBtnWidth) / 2;
                int respawnBtnY = screenHeight / 2 + 50;

                uiManager.DrawPanel(respawnBtnX, respawnBtnY, respawnBtnWidth, respawnBtnHeight,
                    new Color4(0.3f, 0.3f, 0.3f, 0.9f * buttonAlpha));
                uiManager.DrawText("重生",
                    respawnBtnX + respawnBtnWidth / 2 - uiManager.MeasureText("重生", 20) / 2,
                    respawnBtnY + 10,
                    20,
                    new Color4(1f, 1f, 1f, buttonAlpha));

                // 标题画面按钮
                int titleBtnY = respawnBtnY + 50;
                uiManager.DrawPanel(respawnBtnX, titleBtnY, respawnBtnWidth, respawnBtnHeight,
                    new Color4(0.3f, 0.3f, 0.3f, 0.9f * buttonAlpha));
                uiManager.DrawText("返回标题画面",
                    respawnBtnX + respawnBtnWidth / 2 - uiManager.MeasureText("返回标题画面", 16) / 2,
                    titleBtnY + 12,
                    16,
                    new Color4(1f, 1f, 1f, buttonAlpha));

                // 旁观者模式按钮（仅极限模式）
                if (GameSettings.Instance.Hardcore)
                {
                    int spectatorBtnY = titleBtnY + 50;
                    uiManager.DrawPanel(respawnBtnX, spectatorBtnY, respawnBtnWidth, respawnBtnHeight,
                        new Color4(0.3f, 0.3f, 0.3f, 0.9f * buttonAlpha));
                    uiManager.DrawText("旁观者模式",
                        respawnBtnX + respawnBtnWidth / 2 - uiManager.MeasureText("旁观者模式", 16) / 2,
                        spectatorBtnY + 12,
                        16,
                        new Color4(1f, 1f, 1f, buttonAlpha));
                }
            }
        }

        public void HandleClick(int mouseX, int mouseY)
        {
            if (!isActive || deathTime < buttonDelay) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            int btnWidth = 200;
            int btnHeight = 40;
            int btnX = (screenWidth - btnWidth) / 2;
            int respawnBtnY = screenHeight / 2 + 50;
            int titleBtnY = respawnBtnY + 50;
            int spectatorBtnY = titleBtnY + 50;

            // 重生按钮
            if (mouseX >= btnX && mouseX <= btnX + btnWidth &&
                mouseY >= respawnBtnY && mouseY <= respawnBtnY + btnHeight)
            {
                Respawn();
                return;
            }

            // 标题画面按钮
            if (mouseX >= btnX && mouseX <= btnX + btnWidth &&
                mouseY >= titleBtnY && mouseY <= titleBtnY + btnHeight)
            {
                GoToTitleScreen();
                return;
            }

            // 旁观者模式按钮
            if (GameSettings.Instance.Hardcore &&
                mouseX >= btnX && mouseX <= btnX + btnWidth &&
                mouseY >= spectatorBtnY && mouseY <= spectatorBtnY + btnHeight)
            {
                GoToSpectator();
                return;
            }
        }

        public void Respawn()
        {
            Hide();
            OnRespawn?.Invoke();
        }

        public void GoToTitleScreen()
        {
            Hide();
            OnTitleScreen?.Invoke();
        }

        public void GoToSpectator()
        {
            Hide();
            OnSpectator?.Invoke();
        }

        private string GetDeathMessage(string cause)
        {
            return cause switch
            {
                "fall" => "你摔死了",
                "drown" => "你淹死了",
                "lava" => "你试图在岩浆里游泳",
                "fire" => "你被烧死了",
                "void" => "你掉入了虚空",
                "starve" => "你饿死了",
                "zombie" => "你被僵尸杀死了",
                "skeleton" => "你被骷髅射杀了",
                "creeper" => "你被爬行者炸死了",
                "spider" => "你被蜘蛛杀死了",
                "player" => "你被另一个玩家杀死了",
                "explosion" => "你被炸死了",
                "magic" => "你被魔法杀死了",
                "wither" => "你枯萎了",
                "poison" => "你被毒死了",
                "sweet_berry_bush" => "你被甜浆果丛刺死了",
                "anvil" => "你被铁砧砸死了",
                "cactus" => "你被仙人掌扎死了",
                "cramming" => "你被挤死了",
                "dragon_breath" => "你被龙息杀死了",
                "dryout" => "你在水中待太久了",
                "electrocution" => "你被电死了",
                "falling_block" => "你被下落的方块砸死了",
                "fireworks" => "你被烟花炸死了",
                "fly_into_wall" => "你撞上了墙",
                "freeze" => "你冻死了",
                "hot_floor" => "你站在了太热的方块上",
                "lightning" => "你被闪电击中了",
                "magma" => "你发现了岩浆块的秘密",
                "no_water" => "你离开了水",
                "out_of_world" => "你发现了世界的边界",
                "out_of_respawn" => "你没有可用的重生点",
                "stalactite" => "你被钟乳石刺死了",
                "stalagmite" => "你被石笋刺死了",
                "sting" => "你被蜇死了",
                "thorns" => "你被荆棘反伤致死",
                "trident" => "你被三叉戟刺死了",
                "wither_rose" => "你触碰了凋零玫瑰",
                _ => "你死了"
            };
        }

        public bool IsActive => isActive;
        public string DeathMessage => deathMessage;
        public string DeathCause => deathCause;
        public float DeathTime => deathTime;

        public void ResetStats()
        {
            TotalDeaths = 0;
            DeathCauses.Clear();
        }

        public Dictionary<string, int> GetDeathStats()
        {
            return new Dictionary<string, int>(DeathCauses);
        }
    }
}
