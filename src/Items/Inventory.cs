using System;
using System.Collections.Generic;
using VoxelCraft.Core;

namespace VoxelCraft.Items
{
    public class ItemStack
    {
        public int ItemId;
        public int Count;
        public int Durability;
        public Dictionary<string, object> NBT;

        public ItemStack(int itemId, int count = 1)
        {
            ItemId = itemId;
            Count = count;
            Durability = 0;
            NBT = new Dictionary<string, object>();

            ItemInfo info = ItemRegistry.GetItem(itemId);
            if (info != null && info.IsDamageable)
            {
                Durability = info.Durability;
            }
        }

        public ItemStack() : this(0, 0)
        {
        }

        public ItemInfo GetItemInfo()
        {
            return ItemRegistry.GetItem(ItemId);
        }

        public bool IsEmpty()
        {
            return Count <= 0 || ItemId <= 0;
        }

        public int GetMaxStackSize()
        {
            ItemInfo info = GetItemInfo();
            return info != null ? info.MaxStackSize : 64;
        }

        public bool CanStackWith(ItemStack other)
        {
            if (other == null) return false;
            if (ItemId != other.ItemId) return false;
            if (IsEmpty() || other.IsEmpty()) return false;

            ItemInfo info = GetItemInfo();
            if (info != null && info.IsDamageable) return false;

            return true;
        }

        public ItemStack Copy()
        {
            ItemStack copy = new ItemStack(ItemId, Count);
            copy.Durability = Durability;
            foreach (var kvp in NBT)
            {
                copy.NBT[kvp.Key] = kvp.Value;
            }
            return copy;
        }

        public void Damage(int amount)
        {
            ItemInfo info = GetItemInfo();
            if (info != null && info.IsDamageable)
            {
                Durability -= amount;
                if (Durability <= 0)
                {
                    Count = 0;
                    ItemId = 0;
                }
            }
        }
    }

    public class Inventory
    {
        public readonly ItemStack[] Slots;
        public int Size { get; }
        public string Name { get; set; }

        // 扩展属性（玩家状态相关）
        public int Experience { get; set; }
        public int ExperienceLevel { get; set; }
        public int Health { get; set; } = 20;
        public int Hunger { get; set; } = 20;
        public int MaxHealth { get; set; } = 20;
        public int MaxHunger { get; set; } = 20;

        // 快捷栏索引范围
        public const int HotbarSize = 9;
        public int HotbarStart { get; protected set; }
        public int SelectedHotbarSlot { get; set; }

        public Inventory(int size = 36)
        {
            Size = size;
            Slots = new ItemStack[size];
            for (int i = 0; i < size; i++)
            {
                Slots[i] = new ItemStack(0, 0);
            }
            HotbarStart = size - HotbarSize;
            SelectedHotbarSlot = 0;
            Name = "物品栏";
        }

        public ItemStack GetSlot(int index)
        {
            if (index < 0 || index >= Size) return null;
            return Slots[index];
        }

        public ItemStack GetItem(int index)
        {
            return GetSlot(index);
        }

        public void SetSlot(int index, ItemStack stack)
        {
            if (index < 0 || index >= Size) return;
            Slots[index] = stack ?? new ItemStack(0, 0);
        }

        public ItemStack GetSelectedItem()
        {
            return GetSlot(HotbarStart + SelectedHotbarSlot);
        }

        public void SetSelectedItem(ItemStack stack)
        {
            SetSlot(HotbarStart + SelectedHotbarSlot, stack);
        }

        public bool AddItem(ItemStack stack)
        {
            if (stack == null || stack.IsEmpty()) return true;

            // 先尝试堆叠到已有物品
            for (int i = 0; i < Size; i++)
            {
                if (Slots[i].CanStackWith(stack))
                {
                    int space = Slots[i].GetMaxStackSize() - Slots[i].Count;
                    if (space > 0)
                    {
                        int toAdd = Math.Min(space, stack.Count);
                        Slots[i].Count += toAdd;
                        stack.Count -= toAdd;
                        if (stack.Count <= 0) return true;
                    }
                }
            }

            // 再尝试放入空槽
            for (int i = 0; i < Size; i++)
            {
                if (Slots[i].IsEmpty())
                {
                    Slots[i] = stack.Copy();
                    return true;
                }
            }

            return false;
        }

        public bool RemoveItem(int itemId, int count)
        {
            int remaining = count;

            for (int i = 0; i < Size && remaining > 0; i++)
            {
                if (Slots[i].ItemId == itemId && !Slots[i].IsEmpty())
                {
                    int toRemove = Math.Min(remaining, Slots[i].Count);
                    Slots[i].Count -= toRemove;
                    remaining -= toRemove;

                    if (Slots[i].Count <= 0)
                    {
                        Slots[i] = new ItemStack(0, 0);
                    }
                }
            }

            return remaining <= 0;
        }

        public int CountItem(int itemId)
        {
            int count = 0;
            for (int i = 0; i < Size; i++)
            {
                if (Slots[i].ItemId == itemId && !Slots[i].IsEmpty())
                {
                    count += Slots[i].Count;
                }
            }
            return count;
        }

        public bool HasItem(int itemId, int count = 1)
        {
            return CountItem(itemId) >= count;
        }

        public void Clear()
        {
            for (int i = 0; i < Size; i++)
            {
                Slots[i] = new ItemStack(0, 0);
            }
        }

        public void Sort()
        {
            // 简单排序：非空物品前移，按ID排序
            List<ItemStack> items = new List<ItemStack>();
            for (int i = 0; i < Size; i++)
            {
                if (!Slots[i].IsEmpty())
                {
                    items.Add(Slots[i]);
                }
            }

            items.Sort((a, b) => a.ItemId.CompareTo(b.ItemId));

            for (int i = 0; i < Size; i++)
            {
                Slots[i] = i < items.Count ? items[i] : new ItemStack(0, 0);
            }
        }

        public void SwapSlots(int index1, int index2)
        {
            if (index1 < 0 || index1 >= Size || index2 < 0 || index2 >= Size) return;
            (Slots[index1], Slots[index2]) = (Slots[index2], Slots[index1]);
        }

        public void DropSlot(int index)
        {
            if (index < 0 || index >= Size) return;
            Slots[index] = new ItemStack(0, 0);
        }

        public int GetEmptySlotCount()
        {
            int count = 0;
            for (int i = 0; i < Size; i++)
            {
                if (Slots[i].IsEmpty()) count++;
            }
            return count;
        }

        public bool IsFull()
        {
            return GetEmptySlotCount() == 0;
        }

        public List<ItemStack> GetAllItems()
        {
            List<ItemStack> items = new List<ItemStack>();
            for (int i = 0; i < Size; i++)
            {
                if (!Slots[i].IsEmpty())
                {
                    items.Add(Slots[i]);
                }
            }
            return items;
        }

        public virtual void OnSlotChanged(int index)
        {
            // 可以在子类中重写
        }
    }

    // ========================================
    // 玩家背包（包含护甲槽和副手）
    // ========================================
    public class PlayerInventory : Inventory
    {
        public ItemStack[] ArmorSlots { get; }
        public ItemStack OffhandSlot { get; set; }
        public ItemStack CursorSlot { get; set; }

        public PlayerInventory() : base(36)
        {
            ArmorSlots = new ItemStack[4];
            for (int i = 0; i < 4; i++)
            {
                ArmorSlots[i] = new ItemStack(0, 0);
            }
            OffhandSlot = new ItemStack(0, 0);
            CursorSlot = new ItemStack(0, 0);
            Name = "玩家物品栏";
        }

        public int GetTotalArmor()
        {
            int total = 0;
            for (int i = 0; i < 4; i++)
            {
                ItemInfo info = ArmorSlots[i].GetItemInfo();
                if (info != null)
                {
                    total += info.Armor;
                }
            }
            return total;
        }

        public float GetTotalToughness()
        {
            float total = 0;
            for (int i = 0; i < 4; i++)
            {
                ItemInfo info = ArmorSlots[i].GetItemInfo();
                if (info != null)
                {
                    total += info.Toughness;
                }
            }
            return total;
        }

        public void DamageArmor(int amount)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!ArmorSlots[i].IsEmpty())
                {
                    ArmorSlots[i].Damage(amount);
                }
            }
        }
    }

    // ========================================
    // 箱子物品栏
    // ========================================
    public class ChestInventory : Inventory
    {
        public ChestInventory(int rows = 3) : base(rows * 9)
        {
            Name = "箱子";
        }
    }

    // ========================================
    // 工作台物品栏
    // ========================================
    public class CraftingInventory : Inventory
    {
        public ItemStack[] CraftingGrid { get; }
        public ItemStack ResultSlot { get; set; }

        public CraftingInventory() : base(36)
        {
            CraftingGrid = new ItemStack[9];
            for (int i = 0; i < 9; i++)
            {
                CraftingGrid[i] = new ItemStack(0, 0);
            }
            ResultSlot = new ItemStack(0, 0);
            Name = "工作台";
        }
    }

    // ========================================
    // 熔炉物品栏
    // ========================================
    public class FurnaceInventory : Inventory
    {
        public ItemStack InputSlot { get; set; }
        public ItemStack FuelSlot { get; set; }
        public ItemStack OutputSlot { get; set; }
        public int BurnTime { get; set; }
        public int CookTime { get; set; }

        public FurnaceInventory() : base(36)
        {
            InputSlot = new ItemStack(0, 0);
            FuelSlot = new ItemStack(0, 0);
            OutputSlot = new ItemStack(0, 0);
            Name = "熔炉";
        }
    }
}
