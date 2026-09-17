using System;
using System.Collections.Generic;
using SUBR.Data;
using SUBR.Weapons;
using UnityEngine;

namespace SUBR.Inventory
{
    public sealed class InventorySystem : MonoBehaviour
    {
        [SerializeField] int maxSlots = 12;
        [SerializeField] WeaponController weaponController;

        readonly List<ItemStack> _slots = new List<ItemStack>();

        public IReadOnlyList<ItemStack> Slots => _slots;
        public event Action OnChanged;

        void Awake()
        {
            if (weaponController == null)
                weaponController = GetComponentInChildren<WeaponController>();
        }

        public bool TryAdd(ItemData item, int count = 1)
        {
            if (item == null || count <= 0) return false;

            if (item.Kind == ItemKind.Weapon && item.GrantsWeapon != null && weaponController != null)
            {
                weaponController.Equip(item.GrantsWeapon);
                OnChanged?.Invoke();
                return true;
            }

            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i].Data == item && _slots[i].Count < item.MaxStack)
                {
                    int can = item.MaxStack - _slots[i].Count;
                    int add = Mathf.Min(can, count);
                    _slots[i] = new ItemStack(item, _slots[i].Count + add);
                    count -= add;
                    if (count <= 0)
                    {
                        OnChanged?.Invoke();
                        return true;
                    }
                }
            }

            while (count > 0 && _slots.Count < maxSlots)
            {
                int add = Mathf.Min(item.MaxStack, count);
                _slots.Add(new ItemStack(item, add));
                count -= add;
            }

            OnChanged?.Invoke();
            return count <= 0;
        }

        public bool TryUseHeal(ItemData item, Player.Health health)
        {
            if (item == null || item.Kind != ItemKind.Heal || health == null) return false;
            int idx = _slots.FindIndex(s => s.Data == item);
            if (idx < 0) return false;

            health.Heal(item.HealAmount);
            var stack = _slots[idx];
            stack = new ItemStack(stack.Data, stack.Count - 1);
            if (stack.Count <= 0) _slots.RemoveAt(idx);
            else _slots[idx] = stack;
            OnChanged?.Invoke();
            return true;
        }
    }

    [Serializable]
    public struct ItemStack
    {
        public ItemData Data;
        public int Count;
        public ItemStack(ItemData d, int c) { Data = d; Count = c; }
    }
}
