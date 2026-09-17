using SUBR.Combat;
using SUBR.Data;
using SUBR.Player;
using UnityEngine;

namespace SUBR.Weapons
{
    /// <summary>
    /// Local player / bot fire. Hitscan default. Projectile mode optional.
    /// </summary>
    public sealed class WeaponController : MonoBehaviour
    {
        [SerializeField] WeaponData data;
        [SerializeField] Transform muzzle;
        [SerializeField] Transform aimOrigin; // usually camera
        [SerializeField] LayerMask hitMask = ~0;
        [SerializeField] bool isPlayerControlled = true;

        Health _ownerHealth;
        int _ammo;
        float _nextFireTime;
        bool _reloading;
        float _reloadEnd;

        public WeaponData Data => data;
        public int Ammo => _ammo;
        public bool IsReloading => _reloading;

        void Awake()
        {
            _ownerHealth = GetComponentInParent<Health>();
            if (data != null) _ammo = data.MagazineSize;
            if (aimOrigin == null && Camera.main) aimOrigin = Camera.main.transform;
        }

        public void Equip(WeaponData weapon)
        {
            data = weapon;
            _ammo = weapon != null ? weapon.MagazineSize : 0;
            _reloading = false;
        }

        void Update()
        {
            if (data == null || (_ownerHealth != null && _ownerHealth.IsDead)) return;

            if (_reloading)
            {
                if (Time.time >= _reloadEnd)
                {
                    _reloading = false;
                    _ammo = data.MagazineSize;
                }
                return;
            }

            if (isPlayerControlled)
            {
                if (Input.GetKeyDown(KeyCode.R)) StartReload();
                bool wantFire = data.FireRate >= 8f ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");
                // Fire1 = mouse0 by default
                if (Input.GetMouseButton(0) && data.FireRate >= 6f) wantFire = true;
                if (Input.GetMouseButtonDown(0) && data.FireRate < 6f) wantFire = true;
                if (wantFire) TryFire();
            }
        }

        public void SetBotFire(bool fire)
        {
            if (!isPlayerControlled && fire) TryFire();
        }

        public void ConfigureAsBot() => isPlayerControlled = false;

        public bool TryFire()
        {
            if (data == null || _reloading) return false;
            if (Time.time < _nextFireTime) return false;
            if (_ammo <= 0)
            {
                StartReload();
                return false;
            }

            _ammo--;
            _nextFireTime = Time.time + 1f / Mathf.Max(0.01f, data.FireRate);

            if (data.Mode == FireMode.Hitscan) FireHitscan();
            else FireProjectile();

            return true;
        }

        public void StartReload()
        {
            if (data == null || _reloading) return;
            if (_ammo >= data.MagazineSize) return;
            _reloading = true;
            _reloadEnd = Time.time + data.ReloadSeconds;
        }

        void FireHitscan()
        {
            var origin = aimOrigin != null ? aimOrigin : (muzzle != null ? muzzle : transform);
            Vector3 dir = ApplySpread(origin.forward, data.SpreadDegrees);
            if (Physics.Raycast(origin.position, dir, out var hit, data.Range, hitMask, QueryTriggerInteraction.Ignore))
            {
                ApplyHit(hit);
                Debug.DrawLine(origin.position, hit.point, Color.red, 0.05f);
            }
            else
            {
                Debug.DrawRay(origin.position, dir * data.Range, Color.yellow, 0.05f);
            }
        }

        void FireProjectile()
        {
            // Minimal: abhi hitscan fallback. Tum prefab spawn yahan wire kar sakte ho.
            FireHitscan();
        }

        void ApplyHit(RaycastHit hit)
        {
            var hitbox = hit.collider.GetComponent<Hitbox>();
            IDamageable dmg = hitbox != null ? hitbox.Owner : hit.collider.GetComponentInParent<IDamageable>();
            if (dmg == null || dmg.IsDead) return;
            if (_ownerHealth != null && dmg.ActorId == _ownerHealth.ActorId) return;

            bool head = hitbox != null && hitbox.IsHead;
            float amount = data.Damage * (head ? data.HeadshotMultiplier : 1f);
            int attacker = _ownerHealth != null ? _ownerHealth.ActorId : 0;
            dmg.ApplyDamage(amount, attacker, head);
        }

        static Vector3 ApplySpread(Vector3 forward, float degrees)
        {
            if (degrees <= 0f) return forward;
            return Quaternion.Euler(
                Random.Range(-degrees, degrees),
                Random.Range(-degrees, degrees),
                0f) * forward;
        }
    }
}
