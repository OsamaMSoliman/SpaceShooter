using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyBtn : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyName, lobbyHost;
        [SerializeField] private RectTransform playersCount;
        private Vector2 playerSize;

        #region TODO: will be removed when using pooling
        private Button btn;
        private void Awake() => btn = GetComponent<Button>();
        private void OnDestroy() => btn.onClick.RemoveAllListeners();
        #endregion

        // public Lobby Lobby { get; private set; }

        public void Init(Lobby lobby, UnityAction onClick)
        {
            // this.Lobby = lobby;
            this.lobbyName.text = lobby.Name;
            this.lobbyHost.text = lobby.GetHostName();

            this.playerSize = new Vector2(this.playersCount.rect.width / lobby.MaxPlayers, this.playersCount.rect.height);

            UpdatePlayersInside(lobby.Players.Count);

            btn.onClick.AddListener(onClick);
        }

        private void UpdatePlayersInside(int playerCount)
        {
            for (int i = 0; i < this.playersCount.childCount; i++)
            {
                var child = this.playersCount.GetChild(i) as RectTransform;
                child.gameObject.SetActive(i < playerCount);
                child.sizeDelta = this.playerSize;
            }
        }
    }
}
