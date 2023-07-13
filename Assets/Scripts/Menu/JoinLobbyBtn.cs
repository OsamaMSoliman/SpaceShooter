
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyBtn : MonoBehaviour
    {
        [SerializeField] private TMP_Text LobbyName;
        [SerializeField] private TMP_Text LobbyCode;
        [SerializeField] private RectTransform PlayersCount;
        private Button btn;

        private void Awake() => btn = GetComponent<Button>();

        public void Init(string lobbyName, string lobbyCode, int playerCount, UnityAction onClick)
        {
            LobbyName.text = lobbyName;
            LobbyCode.text = lobbyCode;

            var playerSize = new Vector2(PlayersCount.rect.width / playerCount, PlayersCount.rect.height);
            for (int i = 0; i < PlayersCount.childCount; i++)
            {
                var child = PlayersCount.GetChild(i) as RectTransform;
                child.gameObject.SetActive(i < playerCount);
                child.sizeDelta = playerSize;
            }

            btn.onClick.AddListener(onClick);
        }
    }
}
