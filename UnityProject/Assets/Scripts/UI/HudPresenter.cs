using SUBR.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SUBR.UI
{
    /// <summary>
    /// Bind Text/Slider references Inspector se. Null-safe — bina UI bhi chalega.
    /// </summary>
    public sealed class HudPresenter : MonoBehaviour
    {
        [SerializeField] Text aliveText;
        [SerializeField] Text healthText;
        [SerializeField] Slider healthBar;
        [SerializeField] Text zoneText;
        [SerializeField] Text toastText;
        [SerializeField] float toastSeconds = 2f;

        float _toastUntil;

        void OnEnable()
        {
            GameEvents.OnAliveCountChanged += SetAlive;
            GameEvents.OnHealthChanged += SetHealth;
            GameEvents.OnZoneRadiusChanged += SetZone;
            GameEvents.OnToast += ShowToast;
            GameEvents.OnMatchEnded += OnEnded;
        }

        void OnDisable()
        {
            GameEvents.OnAliveCountChanged -= SetAlive;
            GameEvents.OnHealthChanged -= SetHealth;
            GameEvents.OnZoneRadiusChanged -= SetZone;
            GameEvents.OnToast -= ShowToast;
            GameEvents.OnMatchEnded -= OnEnded;
        }

        void Update()
        {
            if (toastText && Time.time > _toastUntil && toastText.text.Length > 0)
                toastText.text = string.Empty;
        }

        void SetAlive(int n)
        {
            if (aliveText) aliveText.text = $"Alive: {n}";
        }

        void SetHealth(int id, float cur, float max)
        {
            // simple: show any; later filter local id
            if (healthText) healthText.text = $"{Mathf.CeilToInt(cur)}/{Mathf.CeilToInt(max)}";
            if (healthBar)
            {
                healthBar.maxValue = max;
                healthBar.value = cur;
            }
        }

        void SetZone(float r)
        {
            if (zoneText) zoneText.text = $"Zone: {r:0}m";
        }

        void ShowToast(string msg)
        {
            if (!toastText) return;
            toastText.text = msg;
            _toastUntil = Time.time + toastSeconds;
        }

        void OnEnded(MatchResult r)
        {
            ShowToast(r.Victory ? $"WIN #{r.Placement}" : $"#{r.Placement} — {r.Reason}");
        }
    }
}
