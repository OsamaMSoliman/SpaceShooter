using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerId, playerStatus;
        [SerializeField] private Image playerStatusImg;
        [SerializeField] private Sprite readySprite, notReadySprite;

        public string PlayerId { get; private set; }
        private CanvasGroup canvasGroup;

        private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

        public void Init(string playerId, bool isReady)
        {
            PlayerId = playerId;
            this.playerId.text = $"Player {playerId}";
            canvasGroup.alpha = 1;
            SetReady(isReady);
        }

        public void SetReady(bool isReady)
        {
            playerStatus.text = isReady ? "Ready" : "Waiting";
            playerStatusImg.sprite = isReady ? readySprite : notReadySprite;
        }

        public void terminate()
        {
            this.playerId.text = "Player Id";
            SetReady(false);
            canvasGroup.alpha = 0.1f;
        }
    }
}
