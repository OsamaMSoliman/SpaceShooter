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

        [Header("Dependencies")] // use ? when accessing them
        [SerializeField] private LobbyManagerSO lobbyManagerSO;

        private void OnEnable()
        {
            RefreshLobbyRoomFirstTime(lobbyManagerSO.CurrentLobby);
            lobbyManagerSO.OnLobbyUpdated += RefreshLobbyRoom;
        }
        private void OnDisable() => lobbyManagerSO.OnLobbyUpdated -= RefreshLobbyRoom;

        private void RefreshLobbyRoom(Lobby lobby)
        {
            this.roomName.text = lobby.Name;
            this.roomCode.text = lobby.LobbyCode;

            var maxPlayers = lobby.MaxPlayers;
            var playersList = lobby.Players;
            var isHost = lobby.HostId == AuthenticationManagerSO.PlayerId;

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
            RefreshLobbyRoom(lobby);
            if (lobby.HostId == AuthenticationManagerSO.PlayerId)
                foreach (var playerUI in lobbyPlayerUIs)
                    playerUI.onKickBtnClicked += lobbyManagerSO.KickPlayer;
        }

        public void OnClickCancel() => lobbyManagerSO.LeaveLobby();

        public void OnClickReady(bool isReady)
        {
            lobbyPlayerUIs.FirstOrDefault(p => p.PlayerId == AuthenticationManagerSO.PlayerId)?.SetReady(true);
            lobbyManagerSO.SendReady(isReady);
        }
    }
}
