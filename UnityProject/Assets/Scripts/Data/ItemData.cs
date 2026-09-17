using UnityEngine;

namespace SUBR.Data
{
    public enum ItemKind
    {
        Weapon,
        Ammo,
        Heal,
        Armor,
        Utility
    }

    [CreateAssetMenu(fileName = "Item_", menuName = "SUBR/Item Data", order = 1)]
    public sealed class ItemData : ScriptableObject
    {
        public int Id;
        public string DisplayName = "Item";
        public ItemKind Kind = ItemKind.Heal;
        public int MaxStack = 5;
        public float HealAmount = 40f;
        public float ArmorAmount;
        public WeaponData GrantsWeapon;
        public Sprite Icon;
    }
}
