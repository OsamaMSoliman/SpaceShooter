using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InsideLobbyController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName, roomID;
        [SerializeField] private LobbyPlayerUI[] lobbyPlayerUIs;

        [Header("Dependencies")] // use ? when accessing them
        [SerializeField] private LobbyManagerSO lobbyManagerSO;

        private void OnEnable()
        {
            // TODO: maybe OnEnable is caller too early before the Lobby gets to refresh?!

            var lobby = lobbyManagerSO.CurrentLobby;
            this.roomName.text = lobby?.Name;
            this.roomID.text = lobby?.LobbyCode;

            var maxPlayers = lobby?.MaxPlayers ?? 0;
            var playersList = lobby?.Players;

            for (int i = 0; i < lobbyPlayerUIs.Length; i++)
            {
                lobbyPlayerUIs[i].gameObject.SetActive(i < maxPlayers);
                if (playersList != null && i < playersList.Count)
                {
                    bool.TryParse(playersList[i].Data["status"]?.Value, out bool playerStatus);
                    lobbyPlayerUIs[i].Init(playersList[i].Id, playerStatus);
                }
            }
        }


        public void OnClickCancel()
        {
            // TODO: trigger LeftLobby
            foreach (var playerUI in lobbyPlayerUIs)
            {
                if (playerUI.PlayerId == AuthenticationManagerSO.PlayerId)
                {
                    playerUI.terminate();
                }
            }
        }

        public void OnClickReady()
        {
            // TODO: trigger ReadyToPlay, (set self to ready and notify the others)
            foreach (var playerUI in lobbyPlayerUIs)
            {
                if (playerUI.PlayerId == AuthenticationManagerSO.PlayerId)
                {
                    playerUI.SetReady(true);
                }
            }
        }

        public void OnLobbyRefreshed(Dictionary<ulong, bool> playersInLobby = default)
        {
            // TODO: MAYBE have 2 different methods? for updating the players and for refershing the connection?
            // TODO: update the LobbyPlayers statuses
            // TODO: animations ex: waiting text alpha pingpong, Lobbyplayer bounce like a beat
        }
    }
}
