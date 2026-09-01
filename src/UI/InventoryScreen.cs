using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.UI
{
    public class InventoryScreen
    {
        private bool isOpen;
        private int selectedSlot;
        private int selectedTab;
        private ItemStack draggedItem;
        private bool isDragging;

        private readonly string[] tabs = { "物品栏", "合成", "附魔", "熔炉", "铁砧", "石切机" };

        // 事件
        public event Action OnClosed;
        public event Action<int, int> OnSlotClicked;
        public event Action<int> OnItemUsed;

        public bool IsOpen => isOpen;
        public int SelectedSlot => selectedSlot;

        public InventoryScreen()
        {
            selectedSlot = 0;
            selectedTab = 0;
            isDragging = false;
        }

        public void Initialize()
        {
            Console.WriteLine("[InventoryScreen] 物品栏界面初始化完成");
        }

        public void Open()
        {
            isOpen = true;
            selectedSlot = 0;
            selectedTab = 0;
        }

        public void Close()
        {
            isOpen = false;
            isDragging = false;
            draggedItem = null;
            OnClosed?.Invoke();
        }

        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Update()
        {
            if (!isOpen) return;
        }

        public void Render(UIManager uiManager, Inventory inventory)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            // 半透明背景
            uiManager.DrawPanel(0, 0, screenWidth, screenHeight, new Color4(0.0f, 0.0f, 0.0f, 0.6f));

            // 物品栏面板
            int panelWidth = 350;
            int panelHeight = 300;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            uiManager.DrawPanel(panelX, panelY, panelWidth, panelHeight,
                new Color4(0.15f, 0.15f, 0.15f, 0.95f));

            // 标题
            string title = "物品栏";
            int titleWidth = uiManager.MeasureText(title, 20);
            uiManager.DrawText(title, panelX + (panelWidth - titleWidth) / 2, panelY + 10, 20, Color4.White);

            // 标签页
            int tabY = panelY + 40;
            int tabWidth = 50;
            int tabHeight = 20;
            int tabSpacing = 3;

            for (int i = 0; i < tabs.Length; i++)
            {
                int tabX = panelX + 10 + i * (tabWidth + tabSpacing);
                bool isSelected = i == selectedTab;

                uiManager.DrawPanel(tabX, tabY, tabWidth, tabHeight,
                    isSelected ? new Color4(0.4f, 0.4f, 0.4f, 0.9f) : new Color4(0.25f, 0.25f, 0.25f, 0.9f));

                int textWidth = uiManager.MeasureText(tabs[i], 10);
                uiManager.DrawText(tabs[i], tabX + (tabWidth - textWidth) / 2, tabY + 5, 10,
                    isSelected ? Color4.White : new Color4(0.8f, 0.8f, 0.8f, 1f));
            }

            // 玩家信息
            int infoY = panelY + 70;
            uiManager.DrawText($"生命值: {inventory.Health}/{inventory.MaxHealth}", panelX + 15, infoY, 12,
                new Color4(1.0f, 0.3f, 0.3f, 1f));
            uiManager.DrawText($"饥饿值: {inventory.Hunger}/{inventory.MaxHunger}", panelX + 15, infoY + 15, 12,
                new Color4(0.8f, 0.6f, 0.2f, 1f));
            uiManager.DrawText($"经验: {inventory.Experience} (等级 {inventory.ExperienceLevel})", panelX + 15, infoY + 30, 12,
                new Color4(0.3f, 0.8f, 0.3f, 1f));

            // 物品栏格子
            int slotSize = 36;
            int slotSpacing = 2;
            int slotsPerRow = 9;
            int startX = panelX + 15;
            int startY = panelY + 110;

            // 主物品栏（3行）
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < slotsPerRow; col++)
                {
                    int slotIndex = row * slotsPerRow + col;
                    int slotX = startX + col * (slotSize + slotSpacing);
                    int slotY = startY + row * (slotSize + slotSpacing);

                    RenderSlot(uiManager, inventory, slotIndex, slotX, slotY, slotSize);
                }
            }

            // 快捷栏（1行）
            int hotbarY = startY + 3 * (slotSize + slotSpacing) + 10;
            for (int col = 0; col < slotsPerRow; col++)
            {
                int slotIndex = 27 + col;
                int slotX = startX + col * (slotSize + slotSpacing);

                RenderSlot(uiManager, inventory, slotIndex, slotX, hotbarY, slotSize);
            }

            // 盔甲槽
            int armorX = panelX + panelWidth - 60;
            int armorY = panelY + 70;
            string[] armorSlots = { "头盔", "胸甲", "护腿", "靴子" };
            for (int i = 0; i < 4; i++)
            {
                int slotY = armorY + i * (slotSize + slotSpacing);
                uiManager.DrawPanel(armorX, slotY, slotSize, slotSize,
                    new Color4(0.2f, 0.2f, 0.2f, 0.9f));
                uiManager.DrawText(armorSlots[i], armorX + 2, slotY + slotSize - 10, 8,
                    new Color4(0.6f, 0.6f, 0.6f, 1f));
            }

            // 副手槽
            int offhandX = panelX + panelWidth - 60;
            int offhandY = hotbarY;
            uiManager.DrawPanel(offhandX, offhandY, slotSize, slotSize,
                new Color4(0.2f, 0.2f, 0.2f, 0.9f));
            uiManager.DrawText("副手", offhandX + 5, offhandY + slotSize - 10, 8,
                new Color4(0.6f, 0.6f, 0.6f, 1f));

            // 拖拽中的物品
            if (isDragging && draggedItem != null)
            {
                // 跟随鼠标（简化显示在中心）
                int dragX = screenWidth / 2 - slotSize / 2;
                int dragY = screenHeight / 2 - slotSize / 2;
                uiManager.DrawPanel(dragX, dragY, slotSize, slotSize,
                    new Color4(0.5f, 0.5f, 0.3f, 0.8f));
                uiManager.DrawText(draggedItem.ItemId.ToString(), dragX + 2, dragY + 10, 8, Color4.White);
                if (draggedItem.Count > 1)
                {
                    uiManager.DrawText(draggedItem.Count.ToString(),
                        dragX + slotSize - 15, dragY + slotSize - 12, 10, Color4.White);
                }
            }

            // 关闭提示
            uiManager.DrawText("按 E 或 ESC 关闭", panelX + 15, panelY + panelHeight - 20, 10,
                new Color4(0.6f, 0.6f, 0.6f, 1f));
        }

        private void RenderSlot(UIManager uiManager, Inventory inventory, int slotIndex, int x, int y, int size)
        {
            bool isSelected = slotIndex == selectedSlot;
            ItemStack item = inventory.GetItem(slotIndex);

            // 格子背景
            uiManager.DrawPanel(x, y, size, size,
                isSelected ? new Color4(0.5f, 0.5f, 0.3f, 0.9f) : new Color4(0.25f, 0.25f, 0.25f, 0.9f));

            // 格子边框
            uiManager.DrawPanel(x, y, size, 1, new Color4(0.4f, 0.4f, 0.4f, 1f));
            uiManager.DrawPanel(x, y + size - 1, size, 1, new Color4(0.1f, 0.1f, 0.1f, 1f));
            uiManager.DrawPanel(x, y, 1, size, new Color4(0.4f, 0.4f, 0.4f, 1f));
            uiManager.DrawPanel(x + size - 1, y, 1, size, new Color4(0.1f, 0.1f, 0.1f, 1f));

            // 物品
            if (item != null && item.ItemId != 0)
            {
                // 物品图标背景（简化）
                uiManager.DrawPanel(x + 4, y + 4, size - 8, size - 8,
                    GetItemColor(item.ItemId));

                // 物品数量
                if (item.Count > 1)
                {
                    string countText = item.Count.ToString();
                    int countWidth = uiManager.MeasureText(countText, 10);
                    uiManager.DrawText(countText, x + size - countWidth - 3, y + size - 12, 10, Color4.White);
                }
            }
        }

        private Color4 GetItemColor(int itemId)
        {
            // 简化的物品颜色
            if (itemId < 256)
            {
                // 方块
                return new Color4(0.4f, 0.6f, 0.4f, 1f);
            }
            else if (itemId < 500)
            {
                // 工具/武器
                return new Color4(0.6f, 0.6f, 0.6f, 1f);
            }
            else if (itemId < 700)
            {
                // 食物
                return new Color4(0.8f, 0.5f, 0.3f, 1f);
            }
            else
            {
                // 其他
                return new Color4(0.5f, 0.4f, 0.6f, 1f);
            }
        }

        public void HandleClick(int mouseX, int mouseY, bool isRightClick)
        {
            if (!isOpen) return;

            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            int panelWidth = 350;
            int panelHeight = 300;
            int panelX = (screenWidth - panelWidth) / 2;
            int panelY = (screenHeight - panelHeight) / 2;

            // 检查标签页点击
            int tabY = panelY + 40;
            int tabWidth = 50;
            int tabHeight = 20;
            int tabSpacing = 3;

            for (int i = 0; i < tabs.Length; i++)
            {
                int tabX = panelX + 10 + i * (tabWidth + tabSpacing);

                if (mouseX >= tabX && mouseX <= tabX + tabWidth &&
                    mouseY >= tabY && mouseY <= tabY + tabHeight)
                {
                    selectedTab = i;
                    return;
                }
            }

            // 检查物品栏格子点击
            int slotSize = 36;
            int slotSpacing = 2;
            int slotsPerRow = 9;
            int startX = panelX + 15;
            int startY = panelY + 110;

            // 主物品栏
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < slotsPerRow; col++)
                {
                    int slotIndex = row * slotsPerRow + col;
                    int slotX = startX + col * (slotSize + slotSpacing);
                    int slotY = startY + row * (slotSize + slotSpacing);

                    if (mouseX >= slotX && mouseX <= slotX + slotSize &&
                        mouseY >= slotY && mouseY <= slotY + slotSize)
                    {
                        HandleSlotClick(slotIndex, isRightClick);
                        return;
                    }
                }
            }

            // 快捷栏
            int hotbarY = startY + 3 * (slotSize + slotSpacing) + 10;
            for (int col = 0; col < slotsPerRow; col++)
            {
                int slotIndex = 27 + col;
                int slotX = startX + col * (slotSize + slotSpacing);

                if (mouseX >= slotX && mouseX <= slotX + slotSize &&
                    mouseY >= hotbarY && mouseY <= hotbarY + slotSize)
                {
                    HandleSlotClick(slotIndex, isRightClick);
                    return;
                }
            }
        }

        private void HandleSlotClick(int slotIndex, bool isRightClick)
        {
            selectedSlot = slotIndex;
            OnSlotClicked?.Invoke(slotIndex, isRightClick ? 1 : 0);
        }

        public void HandleKeyPress(string key)
        {
            if (!isOpen) return;

            switch (key)
            {
                case "Escape":
                case "E":
                    Close();
                    break;

                case "Left":
                    if (selectedSlot > 0)
                    {
                        selectedSlot--;
                    }
                    break;

                case "Right":
                    if (selectedSlot < 35)
                    {
                        selectedSlot++;
                    }
                    break;

                case "Up":
                    if (selectedSlot >= 9)
                    {
                        selectedSlot -= 9;
                    }
                    break;

                case "Down":
                    if (selectedSlot < 27)
                    {
                        selectedSlot += 9;
                    }
                    break;

                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    int hotbarIndex = int.Parse(key) - 1 + 27;
                    selectedSlot = hotbarIndex;
                    break;
            }
        }

        public void StartDragging(ItemStack item)
        {
            draggedItem = item;
            isDragging = true;
        }

        public void StopDragging()
        {
            isDragging = false;
            draggedItem = null;
        }

        public ItemStack GetDraggedItem()
        {
            return draggedItem;
        }
    }
}
