using UnityEngine;

namespace SUBR.Data
{
    public enum FireMode
    {
        Hitscan,
        Projectile
    }

    [CreateAssetMenu(fileName = "Weapon_", menuName = "SUBR/Weapon Data", order = 0)]
    public sealed class WeaponData : ScriptableObject
    {
        public int Id = 1;
        public string DisplayName = "Assault Rifle";
        public FireMode Mode = FireMode.Hitscan;

        [Header("Ballistics")]
        public float Damage = 22f;
        public float HeadshotMultiplier = 2f;
        public float Range = 120f;
        public float FireRate = 10f; // rounds per second
        public float SpreadDegrees = 1.2f;
        public int MagazineSize = 30;
        public float ReloadSeconds = 2.2f;
        public float ProjectileSpeed = 80f;

        [Header("Presentation (assign in Editor)")]
        public GameObject WorldPrefab;
        public GameObject MuzzleFlashPrefab;
        public AudioClip FireSfx;
        public AudioClip ReloadSfx;
    }
}
