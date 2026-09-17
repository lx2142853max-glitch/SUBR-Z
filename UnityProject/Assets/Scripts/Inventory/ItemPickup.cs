using SUBR.Core;
using SUBR.Data;
using UnityEngine;

namespace SUBR.Inventory
{
    [RequireComponent(typeof(Collider))]
    public sealed class ItemPickup : MonoBehaviour
    {
        [SerializeField] ItemData item;
        [SerializeField] int count = 1;
        [SerializeField] bool destroyOnPickup = true;

        void Reset()
        {
            var c = GetComponent<Collider>();
            c.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            var inv = other.GetComponentInParent<InventorySystem>();
            if (inv == null) return;
            if (inv.TryAdd(item, count))
            {
                GameEvents.Toast($"Picked {item.DisplayName}");
                if (destroyOnPickup) Destroy(gameObject);
            }
        }
    }
}
