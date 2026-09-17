using System;
using UnityEngine;

namespace SUBR.Network
{
    public sealed class OfflineNetworkSession : INetworkSession
    {
        public bool IsOnline => false;
        public bool IsHost => true;
        public int LocalPlayerId { get; private set; } = 1;
        public event Action OnConnected;
        public event Action OnDisconnected;

        public void ConnectOffline(string displayName)
        {
            LocalPlayerId = 1;
            Debug.Log($"[SUBR] Offline session as '{displayName}'");
            OnConnected?.Invoke();
        }

        public void Disconnect()
        {
            OnDisconnected?.Invoke();
        }
    }
}
