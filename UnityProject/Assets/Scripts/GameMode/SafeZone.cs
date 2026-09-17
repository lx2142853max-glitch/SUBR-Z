using SUBR.Combat;
using SUBR.Core;
using SUBR.Data;
using UnityEngine;

namespace SUBR.GameMode
{
    /// <summary>
    /// Shrinking circle. Visual ke liye child pe scaled mesh / projector tum laga dena.
    /// </summary>
    public sealed class SafeZone : MonoBehaviour
    {
        [SerializeField] MatchConfig config;
        [SerializeField] Transform visual; // optional cylinder
        [SerializeField] float currentRadius = 200f;
        [SerializeField] Vector3 center;

        public float Radius => currentRadius;
        public Vector3 Center => center;

        float _phaseTimer;
        int _phaseIndex;
        bool _shrinking;
        float _shrinkFrom;
        float _shrinkTo;
        float _shrinkDuration;
        float _shrinkElapsed;
        bool _running;

        public void Begin(MatchConfig cfg)
        {
            config = cfg;
            center = transform.position;
            currentRadius = cfg.StartRadius;
            _phaseIndex = 0;
            _phaseTimer = cfg.WaitBetweenPhases;
            _shrinking = false;
            _running = true;
            ApplyVisual();
            GameEvents.RaiseZoneRadius(currentRadius);
        }

        public void StopZone() => _running = false;

        void Update()
        {
            if (!_running || config == null) return;

            if (_shrinking)
            {
                _shrinkElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(_shrinkElapsed / Mathf.Max(0.01f, _shrinkDuration));
                currentRadius = Mathf.Lerp(_shrinkFrom, _shrinkTo, t);
                ApplyVisual();
                GameEvents.RaiseZoneRadius(currentRadius);
                if (t >= 1f)
                {
                    _shrinking = false;
                    _phaseTimer = config.WaitBetweenPhases;
                }
            }
            else
            {
                _phaseTimer -= Time.deltaTime;
                if (_phaseTimer <= 0f && _phaseIndex < config.PhaseCount)
                    StartNextShrink();
            }

            TickOutsideDamage();
        }

        void StartNextShrink()
        {
            _phaseIndex++;
            _shrinkFrom = currentRadius;
            float p = _phaseIndex / (float)config.PhaseCount;
            _shrinkTo = Mathf.Lerp(config.StartRadius, config.EndRadius, p);
            _shrinkDuration = config.PhaseDuration;
            _shrinkElapsed = 0f;
            _shrinking = true;
            // slight center drift
            center += Random.insideUnitSphere * (currentRadius * 0.05f);
            center.y = transform.position.y;
            GameEvents.Toast($"Zone phase {_phaseIndex}");
        }

        void TickOutsideDamage()
        {
            float dps = config.OutsideDamagePerSecond * Time.deltaTime;
            var healths = FindObjectsOfType<Player.Health>();
            for (int i = 0; i < healths.Length; i++)
            {
                var h = healths[i];
                if (h.IsDead) continue;
                Vector3 p = h.transform.position;
                p.y = center.y;
                if (Vector3.Distance(p, center) > currentRadius)
                {
                    var dmg = h.GetComponent<IDamageable>();
                    dmg?.ApplyDamage(dps, 0, false);
                }
            }
        }

        void ApplyVisual()
        {
            if (visual == null) return;
            visual.position = center;
            float d = currentRadius * 2f;
            visual.localScale = new Vector3(d, visual.localScale.y, d);
        }

        public bool IsInside(Vector3 worldPos)
        {
            worldPos.y = center.y;
            return Vector3.Distance(worldPos, center) <= currentRadius;
        }
    }
}
