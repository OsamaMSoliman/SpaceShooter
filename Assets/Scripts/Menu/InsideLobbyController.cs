using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class InsideLobbyController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName, roomCode;
        [SerializeField] private LobbyPlayerUI[] lobbyPlayerUIs;
        [SerializeField] private Button readyBtn, notReadyBtn;
        private CanvasStateNotifier canvasStateNotifier;
        private void Awake() => canvasStateNotifier = GetComponent<CanvasStateNotifier>();

        private void OnEnable()
        {
            RefreshLobbyRoomFirstTime(LobbyManager.Instance.CurrentLobby);
            LobbyManager.Instance.OnLobbyUpdated += RefreshLobbyRoom;
            // reset the UI
            readyBtn.gameObject.SetActive(true);
            notReadyBtn.gameObject.SetActive(false);
        }
        private void OnDisable() => LobbyManager.Instance.OnLobbyUpdated -= RefreshLobbyRoom;

        private void RefreshLobbyRoom(Lobby lobby)
        {
            if (lobby.Players.FirstOrDefault(p => p.Id == AuthenticationManager.PlayerId) == null)
            {
                canvasStateNotifier.OnClickChangeCanvas();
                return;
            }

            this.roomName.text = lobby.Name;
            this.roomCode.text = lobby.LobbyCode;

            var maxPlayers = lobby.MaxPlayers;
            var playersList = lobby.Players;
            var isHost = lobby.HostId == AuthenticationManager.PlayerId;

            for (int i = 0; i < lobbyPlayerUIs.Length; i++)
            {
                lobbyPlayerUIs[i].gameObject.SetActive(i < maxPlayers);
                var isActive = i < playersList.Count;
                if (isActive && playersList[i].Data != null)
                {
                    playersList[i].Data.TryGetValue(Constants.PLAYER_NAME, out var playerName);
                    playersList[i].Data.TryGetValue(Constants.IS_READY, out var isPlayerReady);
                    bool.TryParse(isPlayerReady?.Value, out bool isReady);
                    lobbyPlayerUIs[i].Init(
                        playersList[i].Id,
                        playerName?.Value,
                        isReady,
                        isActive,
                        isHost & playersList[i].Id != lobby.HostId);
                }
                else
                {
                    // NOTE: This is necessary in case a re-join after a disconnect!
                    lobbyPlayerUIs[i].Init();
                }
            }
        }

        private void RefreshLobbyRoomFirstTime(Lobby lobby)
        {
            if (lobby == null) throw new System.Exception("Lobby is null, this should not happen in the init phase!");
            RefreshLobbyRoom(lobby);
            AbilityToKickPlayers(lobby);
        }

        private void AbilityToKickPlayers(Lobby lobby)
        {
            if (lobby.HostId == AuthenticationManager.PlayerId)
                foreach (var playerUI in lobbyPlayerUIs)
                    playerUI.onKickBtnClicked += (playerUI) => OnPlayerKicked(playerUI, lobby.Id);
        }

        private async void OnPlayerKicked(LobbyPlayerUI playerUI, string lobbyId)
        {
            await LobbyManager.Instance.KickPlayer(lobbyId, playerUI.PlayerId);
            playerUI.Init();
        }

        public void OnClickCancel() => LobbyManager.Instance.LeaveLobby();

        public void OnClickReady(bool isReady)
        {
            // NOTE: we don't know which player slot is ours, so we search first then set the first one we found to ready
            lobbyPlayerUIs.FirstOrDefault(p => p.PlayerId == AuthenticationManager.PlayerId)?.SetReady(true);
            LobbyManager.Instance.SendReady(isReady);
        }
    }
}
