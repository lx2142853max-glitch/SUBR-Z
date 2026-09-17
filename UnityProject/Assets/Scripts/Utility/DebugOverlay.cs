using SUBR.Core;
using SUBR.GameMode;
using UnityEngine;

namespace SUBR.Utility
{
    /// <summary>F1 se toggle. Build mein optional.</summary>
    public sealed class DebugOverlay : MonoBehaviour
    {
        [SerializeField] bool show = true;
        MatchDirector _match;
        string _lastToast;

        void OnEnable()
        {
            GameEvents.OnToast += m => _lastToast = m;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) show = !show;
            if (_match == null) _match = FindObjectOfType<MatchDirector>();
        }

        void OnGUI()
        {
            if (!show) return;
            const int w = 320;
            GUILayout.BeginArea(new Rect(10, 10, w, 160), GUI.skin.box);
            GUILayout.Label("SUBR Debug (F1)");
            if (_match != null)
            {
                GUILayout.Label($"State: {_match.State}");
                GUILayout.Label($"Alive: {_match.AliveCount}");
            }
            GUILayout.Label($"Session kills: {GameSession.Instance.LastKills}");
            if (!string.IsNullOrEmpty(_lastToast)) GUILayout.Label($"Toast: {_lastToast}");
            GUILayout.EndArea();
        }
    }
}
