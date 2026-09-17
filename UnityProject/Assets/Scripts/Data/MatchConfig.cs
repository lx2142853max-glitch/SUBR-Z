using UnityEngine;

namespace SUBR.Data
{
    [CreateAssetMenu(fileName = "MatchConfig", menuName = "SUBR/Match Config", order = 2)]
    public sealed class MatchConfig : ScriptableObject
    {
        [Header("Population")]
        public int MaxPlayers = 30;
        public int BotFillCount = 12;

        [Header("Zone")]
        public float StartRadius = 200f;
        public float EndRadius = 15f;
        public int PhaseCount = 5;
        public float PhaseDuration = 45f;
        public float WaitBetweenPhases = 20f;
        public float OutsideDamagePerSecond = 4f;

        [Header("Match")]
        public float WarmupSeconds = 5f;
        public bool FriendlyFire;
    }
}
