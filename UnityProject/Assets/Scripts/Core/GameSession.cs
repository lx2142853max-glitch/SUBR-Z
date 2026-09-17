using UnityEngine;

namespace SUBR.Core
{
    /// <summary>Run-wide session data (loadout, stats between scenes).</summary>
    public sealed class GameSession
    {
        static GameSession _instance;
        public static GameSession Instance => _instance ?? (_instance = new GameSession());

        public string PlayerDisplayName = "Hunter";
        public int SelectedWeaponId = 1;
        public int LastKills;
        public int LastPlacement;
        public bool OfflineBots = true;
        public int TargetBotCount = 10;

        public void ResetMatchStats()
        {
            LastKills = 0;
            LastPlacement = 0;
        }
    }
}
