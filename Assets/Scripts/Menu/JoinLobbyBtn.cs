
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyBtn : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lobbyName, lobbyCode;
        [SerializeField] private RectTransform playersCount;
        private Vector2 playerSize;

        public string LobbyCode { get; private set; }

        public void Init(string lobbyName, string lobbyCode, int playerCount, int maxPlayerCount, UnityAction onClick)
        {
            this.lobbyName.text = lobbyName;
            this.lobbyCode.text = lobbyCode;

            this.playerSize = new Vector2(this.playersCount.rect.width / maxPlayerCount, this.playersCount.rect.height);

            UpdatePlayersInside(playerCount);

            GetComponent<Button>().onClick.AddListener(onClick);
        }

        public void UpdatePlayersInside(int playerCount)
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
