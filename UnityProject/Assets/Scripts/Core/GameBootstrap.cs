using SUBR.Network;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SUBR.Core
{
    /// <summary>
    /// Boot scene pe rakho. DontDestroyOnLoad. Services register + pehli scene load.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] string firstScene = GameIds.SceneLobby;
        [SerializeField] bool dontDestroy = true;

        void Awake()
        {
            if (dontDestroy) DontDestroyOnLoad(gameObject);

            ServiceLocator.Clear();
            ServiceLocator.Register<INetworkSession>(new OfflineNetworkSession());
            ServiceLocator.Register(GameSession.Instance);

            Debug.Log("[SUBR] Bootstrap ready (Offline network stub).");
        }

        void Start()
        {
            if (!string.IsNullOrEmpty(firstScene) &&
                SceneManager.GetActiveScene().name != firstScene)
            {
                SceneManager.LoadScene(firstScene);
            }
        }
    }
}
