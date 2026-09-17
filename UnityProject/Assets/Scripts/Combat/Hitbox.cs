using UnityEngine;

namespace SUBR.Combat
{
    /// <summary>Collider pe lagao. Head hitbox pe isHead = true.</summary>
    public sealed class Hitbox : MonoBehaviour
    {
        public bool IsHead;
        public IDamageable Owner { get; private set; }

        void Awake()
        {
            Owner = GetComponentInParent<IDamageable>();
            if (Owner == null)
                Debug.LogWarning($"[SUBR] Hitbox on {name} has no IDamageable parent.", this);
        }
    }
}
