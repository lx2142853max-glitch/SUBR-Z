using System;

namespace SUBR.Network
{
    /// <summary>
    /// Abstraction — pehle Offline, baad mein Photon/Mirror implement karenge.
    /// </summary>
    public interface INetworkSession
    {
        bool IsOnline { get; }
        bool IsHost { get; }
        int LocalPlayerId { get; }
        void ConnectOffline(string displayName);
        void Disconnect();
        event Action OnConnected;
        event Action OnDisconnected;
    }
}
