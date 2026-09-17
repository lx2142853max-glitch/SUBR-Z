using System;
using SUBR.Core;
using UnityEngine;

namespace SUBR.Player
{
    /// <summary>HP + armor. Damage pipeline yahin se guzarti hai.</summary>
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float maxArmor = 50f;
        [SerializeField] int actorId;

        public float Current { get; private set; }
        public float Armor { get; private set; }
        public float MaxHealth => maxHealth;
        public int ActorId => actorId;
        public bool IsDead { get; private set; }

        public event Action<Health> OnDied;
        public event Action<Health, float> OnDamaged;

        void Awake()
        {
            if (actorId == 0) actorId = GetInstanceID();
            Current = maxHealth;
            Armor = 0f;
        }

        public void ConfigureId(int id) => actorId = id;

        public void FullHeal()
        {
            IsDead = false;
            Current = maxHealth;
            PushEvent();
        }

        public void AddArmor(float amount)
        {
            Armor = Mathf.Clamp(Armor + amount, 0f, maxArmor);
            PushEvent();
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            Current = Mathf.Clamp(Current + amount, 0f, maxHealth);
            PushEvent();
        }

        /// <returns>Actual health damage applied after armor.</returns>
        public float ApplyDamage(float raw, int attackerId, bool headshot)
        {
            if (IsDead || raw <= 0f) return 0f;

            float remaining = raw;
            if (Armor > 0f)
            {
                float absorbed = Mathf.Min(Armor, remaining);
                Armor -= absorbed;
                remaining -= absorbed;
            }

            if (remaining > 0f)
            {
                Current = Mathf.Max(0f, Current - remaining);
                OnDamaged?.Invoke(this, remaining);
            }

            PushEvent();

            if (Current <= 0f && !IsDead)
            {
                IsDead = true;
                OnDied?.Invoke(this);
                GameEvents.RaiseKill(attackerId, actorId);
            }

            return remaining;
        }

        void PushEvent() => GameEvents.RaiseHealth(actorId, Current, maxHealth);
    }
}
