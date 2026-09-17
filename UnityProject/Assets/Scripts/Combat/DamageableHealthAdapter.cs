using SUBR.Player;
using UnityEngine;

namespace SUBR.Combat
{
    /// <summary>Health component ko IDamageable banata hai (weapons / zone ke liye).</summary>
    [RequireComponent(typeof(Health))]
    public sealed class DamageableHealthAdapter : MonoBehaviour, IDamageable
    {
        Health _health;
        void Awake() => _health = GetComponent<Health>();
        public int ActorId => _health.ActorId;
        public bool IsDead => _health.IsDead;
        public float ApplyDamage(float amount, int attackerId, bool headshot)
            => _health.ApplyDamage(amount * (headshot ? 1f : 1f), attackerId, headshot);
        // headshot multiplier weapons side se raw damage mein apply hota hai
    }
}
