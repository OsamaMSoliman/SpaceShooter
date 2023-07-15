using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class InsideLobbyController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI roomName, roomID;
        [SerializeField] private Transform playersContainer;
        [SerializeField] private LobbyPlayerUI firstLobbyPlayer;

        [Header("Dependencies")] // use ? when accessing them
        [SerializeField] private LobbyManagerSO lobbyManagerSO;

        public void Init(string lobbyName, string lobbyId)
        {
            this.roomName.text = lobbyName;
            this.roomID.text = lobbyId;

            // TODO: set firstLobbyPlayer
            // TODO: hide players over MaxPlayers
        }


        public void OnClickCancel()
        {
            // TODO: trigger LeftLobby
        }

        public void OnClickReady()
        {
            // TODO: trigger ReadyToPlay
        }

        public void OnLobbyRefreshed(Dictionary<ulong, bool> playersInLobby = default)
        {
            // TODO: MAYBE have 2 different methods? for updating the players and for refershing the connection?
            // TODO: update the LobbyPlayers statuses
            // TODO: animations ex: waiting text alpha pingpong, Lobbyplayer bounce like a beat
        }
    }
}
