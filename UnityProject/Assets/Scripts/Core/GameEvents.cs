using System;
using UnityEngine;

namespace SUBR.Core
{
    /// <summary>
    /// Lightweight static event bus. Network layer baad mein iske through bhi raise kar sakti hai.
    /// </summary>
    public static class GameEvents
    {
        public static event Action OnMatchStarting;
        public static event Action OnMatchStarted;
        public static event Action<MatchResult> OnMatchEnded;
        public static event Action<int> OnAliveCountChanged;
        public static event Action<float> OnZoneRadiusChanged;
        public static event Action<int, int> OnPlayerKilled; // killerId, victimId
        public static event Action<int, float, float> OnHealthChanged; // actorId, current, max
        public static event Action<string> OnToast;

        public static void RaiseMatchStarting() => OnMatchStarting?.Invoke();
        public static void RaiseMatchStarted() => OnMatchStarted?.Invoke();
        public static void RaiseMatchEnded(MatchResult r) => OnMatchEnded?.Invoke(r);
        public static void RaiseAliveCount(int n) => OnAliveCountChanged?.Invoke(n);
        public static void RaiseZoneRadius(float r) => OnZoneRadiusChanged?.Invoke(r);
        public static void RaiseKill(int killer, int victim) => OnPlayerKilled?.Invoke(killer, victim);
        public static void RaiseHealth(int id, float cur, float max) => OnHealthChanged?.Invoke(id, cur, max);
        public static void Toast(string msg) => OnToast?.Invoke(msg);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            OnMatchStarting = null;
            OnMatchStarted = null;
            OnMatchEnded = null;
            OnAliveCountChanged = null;
            OnZoneRadiusChanged = null;
            OnPlayerKilled = null;
            OnHealthChanged = null;
            OnToast = null;
        }
    }

    [Serializable]
    public struct MatchResult
    {
        public bool Victory;
        public int Placement;
        public int Kills;
        public float SurvivalSeconds;
        public string Reason;
    }
}
