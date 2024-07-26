using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerName, playerStatus;
        [SerializeField] private Image playerStatusImg;
        [SerializeField] private Sprite readySprite, notReadySprite;
        [SerializeField] private Button kickBtn;

        public string PlayerId { get; private set; }
        private CanvasGroup canvasGroup;

        private void Awake() => canvasGroup = GetComponent<CanvasGroup>();

        public void Init(string playerId = null, string playerName = "Player ID", bool isReady = false, bool isActive = false, bool isHost = false)
        {
            PlayerId = playerId;
            this.playerName.text = playerName;
            canvasGroup.alpha = isActive ? 1 : 0.1f;
            SetReady(isActive & isReady);
            kickBtn.gameObject.SetActive(isActive & isHost);
        }

        // NOTE: This is used to quickly set the player to active before it gets updated in the lobby for everyone
        public void SetReady(bool isReady)
        {
            playerStatus.text = isReady ? "Ready" : "Waiting";
            playerStatusImg.sprite = isReady ? readySprite : notReadySprite;
        }


        private event Action<LobbyPlayerUI> kickBtnClicked;
        public event Action<LobbyPlayerUI> onKickBtnClicked
        {
            add => kickBtnClicked = value;
            remove => throw new NotImplementedException("Only overwriting is allowed!");
        }
        public void OnClickKickPlayer() => kickBtnClicked?.Invoke(this);
    }
}
