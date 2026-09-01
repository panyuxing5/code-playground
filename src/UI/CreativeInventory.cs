using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using VoxelCraft.Core;
using VoxelCraft.Items;

namespace VoxelCraft.UI
{
    public class CreativeInventory
    {
        private readonly ItemRegistry itemRegistry;
        private readonly List<ItemStack> allItems;
        private int currentPage;
        private int itemsPerPage = 45;
        private int totalPages;

        // 分类
        private readonly Dictionary<string, List<int>> categories;
        private string currentCategory = "all";

        // 搜索
        private string searchText = "";
        private List<ItemStack> searchResults;

        // 选中的物品
        public int SelectedSlot { get; set; }
        public ItemStack HoveredItem { get; private set; }

        // 事件
        public event Action<int, ItemStack> OnItemSelected;
        public event Action OnInventoryClosed;

        public bool IsOpen { get; set; }

        public CreativeInventory(ItemRegistry itemRegistry)
        {
            this.itemRegistry = itemRegistry;
            allItems = new List<ItemStack>();
            categories = new Dictionary<string, List<int>>();
            searchResults = new List<ItemStack>();

            InitializeCategories();
            LoadAllItems();
        }

        private void InitializeCategories()
        {
            categories["all"] = new List<int>();
            categories["building_blocks"] = new List<int>();
            categories["decorations"] = new List<int>();
            categories["redstone"] = new List<int>();
            categories["transportation"] = new List<int>();
            categories["misc"] = new List<int>();
            categories["food"] = new List<int>();
            categories["tools"] = new List<int>();
            categories["combat"] = new List<int>();
            categories["brewing"] = new List<int>();
        }

        private void LoadAllItems()
        {
            allItems.Clear();

            // 获取所有物品
            Dictionary<int, ItemInfo> items = ItemRegistry.GetAllItems();
            foreach (KeyValuePair<int, ItemInfo> kvp in items)
            {
                ItemInfo info = kvp.Value;
                if (info == null) continue;

                ItemStack stack = new ItemStack(kvp.Key, 1);
                allItems.Add(stack);

                // 分类
                categories["all"].Add(kvp.Key);

                switch (info.Type)
                {
                    case ItemType.Block:
                        categories["building_blocks"].Add(kvp.Key);
                        break;
                    case ItemType.Tool:
                        categories["tools"].Add(kvp.Key);
                        break;
                    case ItemType.Weapon:
                        categories["combat"].Add(kvp.Key);
                        break;
                    case ItemType.Armor:
                        categories["combat"].Add(kvp.Key);
                        break;
                    case ItemType.Food:
                        categories["food"].Add(kvp.Key);
                        break;
                    case ItemType.Potion:
                        categories["brewing"].Add(kvp.Key);
                        break;
                    case ItemType.Material:
                        categories["misc"].Add(kvp.Key);
                        break;
                    default:
                        categories["misc"].Add(kvp.Key);
                        break;
                }
            }

            // 红石分类
            for (int i = 100; i < 150; i++)
            {
                if (items.ContainsKey(i))
                {
                    categories["redstone"].Add(i);
                }
            }

            // 装饰分类
            for (int i = 150; i < 200; i++)
            {
                if (items.ContainsKey(i))
                {
                    categories["decorations"].Add(i);
                }
            }

            // 交通分类
            for (int i = 200; i < 250; i++)
            {
                if (items.ContainsKey(i))
                {
                    categories["transportation"].Add(i);
                }
            }

            totalPages = (int)Math.Ceiling((double)allItems.Count / itemsPerPage);
            Console.WriteLine($"[CreativeInventory] 加载了 {allItems.Count} 个物品，共 {totalPages} 页");
        }

        public void Update()
        {
            if (!IsOpen) return;

            // 处理输入
            // 实际游戏中处理鼠标点击和键盘输入
        }

        public void Render(UIManager uiManager)
        {
            if (!IsOpen) return;

            // 渲染背景
            int screenWidth = GameSettings.Instance.WindowWidth;
            int screenHeight = GameSettings.Instance.WindowHeight;

            int invWidth = 380;
            int invHeight = 220;
            int invX = (screenWidth - invWidth) / 2;
            int invY = (screenHeight - invHeight) / 2;

            // 背景面板
            uiManager.DrawPanel(invX, invY, invWidth, invHeight, new Color4(0.8f, 0.8f, 0.8f, 0.95f));

            // 标题
            uiManager.DrawText("物品栏", invX + 10, invY + 10, 16, Color4.Black);

            // 分类标签
            string[] categoryNames = { "全部", "建筑", "装饰", "红石", "交通", "杂项", "食物", "工具", "战斗", "酿造" };
            string[] categoryKeys = { "all", "building_blocks", "decorations", "redstone", "transportation", "misc", "food", "tools", "combat", "brewing" };

            for (int i = 0; i < categoryNames.Length; i++)
            {
                int tabX = invX + 10 + i * 36;
                int tabY = invY + 30;
                bool isSelected = currentCategory == categoryKeys[i];

                uiManager.DrawPanel(tabX, tabY, 34, 20,
                    isSelected ? new Color4(0.7f, 0.7f, 0.7f, 1f) : new Color4(0.6f, 0.6f, 0.6f, 0.8f));
                uiManager.DrawText(categoryNames[i], tabX + 2, tabY + 4, 10, Color4.Black);
            }

            // 搜索框
            uiManager.DrawPanel(invX + 10, invY + 55, 200, 20, new Color4(1f, 1f, 1f, 0.9f));
            uiManager.DrawText(searchText, invX + 14, invY + 58, 12, Color4.Black);

            // 物品网格
            List<ItemStack> displayItems = GetDisplayItems();
            int startIndex = currentPage * itemsPerPage;

            for (int i = 0; i < itemsPerPage && startIndex + i < displayItems.Count; i++)
            {
                int slotX = invX + 10 + (i % 9) * 40;
                int slotY = invY + 80 + (i / 9) * 40;

                ItemStack item = displayItems[startIndex + i];

                // 槽位背景
                uiManager.DrawPanel(slotX, slotY, 36, 36, new Color4(0.5f, 0.5f, 0.5f, 0.8f));

                // 物品图标
                uiManager.DrawItemIcon(item.ItemId, slotX + 2, slotY + 2, 32);

                // 悬停检测
                // 实际游戏中检测鼠标位置
            }

            // 页码
            uiManager.DrawText($"第 {currentPage + 1}/{totalPages} 页", invX + invWidth - 80, invY + invHeight - 25, 12, Color4.Black);

            // 翻页按钮
            if (currentPage > 0)
            {
                uiManager.DrawPanel(invX + 10, invY + invHeight - 30, 50, 20, new Color4(0.6f, 0.6f, 0.6f, 0.9f));
                uiManager.DrawText("上一页", invX + 14, invY + invHeight - 26, 10, Color4.Black);
            }

            if (currentPage < totalPages - 1)
            {
                uiManager.DrawPanel(invX + 70, invY + invHeight - 30, 50, 20, new Color4(0.6f, 0.6f, 0.6f, 0.9f));
                uiManager.DrawText("下一页", invX + 74, invY + invHeight - 26, 10, Color4.Black);
            }
        }

        private List<ItemStack> GetDisplayItems()
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                return searchResults;
            }

            if (categories.TryGetValue(currentCategory, out List<int> categoryItems))
            {
                List<ItemStack> result = new List<ItemStack>();
                foreach (int itemId in categoryItems)
                {
                    result.Add(new ItemStack(itemId, 1));
                }
                return result;
            }

            return allItems;
        }

        public void SetCategory(string category)
        {
            currentCategory = category;
            currentPage = 0;
            totalPages = (int)Math.Ceiling((double)GetDisplayItems().Count / itemsPerPage);
        }

        public void SetSearchText(string text)
        {
            searchText = text;
            currentPage = 0;

            // 搜索物品
            searchResults.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                foreach (ItemStack item in allItems)
                {
                    ItemInfo info = ItemRegistry.GetItem(item.ItemId);
                    if (info != null && (info.Name.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                        info.TranslationKey.Contains(text, StringComparison.OrdinalIgnoreCase)))
                    {
                        searchResults.Add(item);
                    }
                }
            }

            totalPages = (int)Math.Ceiling((double)GetDisplayItems().Count / itemsPerPage);
        }

        public void NextPage()
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
            }
        }

        public void SelectItem(int slotIndex)
        {
            List<ItemStack> displayItems = GetDisplayItems();
            int index = currentPage * itemsPerPage + slotIndex;

            if (index >= 0 && index < displayItems.Count)
            {
                ItemStack selected = displayItems[index];
                OnItemSelected?.Invoke(slotIndex, selected);
            }
        }

        public void Open()
        {
            IsOpen = true;
            currentPage = 0;
        }

        public void Close()
        {
            IsOpen = false;
            OnInventoryClosed?.Invoke();
        }

        public void Toggle()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }
}
