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

        public ulong PlayerId { get; private set; }

        public void Init(ulong playerId)
        {
            PlayerId = playerId;
            this.playerId.text = $"Player {playerId}";
        }

        public void SetReady(bool isReady)
        {
            playerStatus.text = isReady ? "Ready" : "Waiting";
            playerStatusImg.sprite = isReady ? readySprite : notReadySprite;
        }
    }
}
