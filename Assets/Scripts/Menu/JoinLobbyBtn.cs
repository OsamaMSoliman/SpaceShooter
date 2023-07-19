using TMPro;
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

        public string LobbyCode { get; private set; }

        public void Init(string lobbyName, string hostName, int playerCount, int maxPlayerCount, UnityAction onClick)
        {
            this.lobbyName.text = lobbyName;
            this.lobbyHost.text = hostName;

            this.playerSize = new Vector2(this.playersCount.rect.width / maxPlayerCount, this.playersCount.rect.height);

            UpdatePlayersInside(playerCount);

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
