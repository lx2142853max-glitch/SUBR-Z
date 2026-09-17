using SUBR.Core;
using SUBR.Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SUBR.UI
{
    public sealed class LobbyUI : MonoBehaviour
    {
        [SerializeField] InputField nameField;
        [SerializeField] Toggle botsToggle;
        [SerializeField] Button playButton;
        [SerializeField] string matchScene = GameIds.SceneMatch;

        void Start()
        {
            if (nameField) nameField.text = GameSession.Instance.PlayerDisplayName;
            if (botsToggle) botsToggle.isOn = GameSession.Instance.OfflineBots;
            if (playButton) playButton.onClick.AddListener(OnPlay);
        }

        public void OnPlay()
        {
            if (nameField) GameSession.Instance.PlayerDisplayName = nameField.text;
            if (botsToggle) GameSession.Instance.OfflineBots = botsToggle.isOn;

            if (ServiceLocator.TryResolve<INetworkSession>(out var net))
                net.ConnectOffline(GameSession.Instance.PlayerDisplayName);

            SceneManager.LoadScene(matchScene);
        }
    }
}
