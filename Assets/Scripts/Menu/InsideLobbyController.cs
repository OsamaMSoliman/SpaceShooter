using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InsideLobbyController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName, roomCode;
        [SerializeField] private LobbyPlayerUI[] lobbyPlayerUIs;
        [SerializeField] private GameObject loadingSpinner;

        private void OnEnable()
        {
            // if (LobbyManager.Instance.CurrentLobby != null)
            RefreshLobbyRoomFirstTime(LobbyManager.Instance.CurrentLobby);
            LobbyManager.Instance.OnLobbyUpdated += RefreshLobbyRoom;
        }
        private void OnDisable() => LobbyManager.Instance.OnLobbyUpdated -= RefreshLobbyRoom;

        private void RefreshLobbyRoom(Lobby lobby)
        {
            if (ShowLoadingSpinner(lobby == null)) return;

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

        private bool ShowLoadingSpinner(bool isLoading)
        {
            loadingSpinner.SetActive(isLoading);
            lobbyPlayerUIs[0].transform.parent.gameObject.SetActive(!isLoading);
            return isLoading;
        }

        private void RefreshLobbyRoomFirstTime(Lobby lobby)
        {
            RefreshLobbyRoom(lobby);
            AbilityToKickPlayers(lobby);
        }

        private void AbilityToKickPlayers(Lobby lobby)
        {
            if (lobby?.HostId == AuthenticationManager.PlayerId)
                foreach (var playerUI in lobbyPlayerUIs)
                    playerUI.onKickBtnClicked += (playerId) => LobbyManager.Instance.KickPlayer(lobby.Id, playerId);
        }

        public void OnClickCancel() => LobbyManager.Instance.LeaveLobby();

        public void OnClickReady(bool isReady)
        {
            lobbyPlayerUIs.FirstOrDefault(p => p.PlayerId == AuthenticationManager.PlayerId)?.SetReady(true);
            LobbyManager.Instance.SendReady(isReady);
        }
    }
}
