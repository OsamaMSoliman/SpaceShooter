using System;
using System.Threading;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Nsr.MultiSpaceShooter
{
    public class LobbyManager : MonoBehaviour
    {
        #region Singleton
        private static LobbyManager _instance;
        public static LobbyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LobbyManager>() ?? new GameObject("LobbyManager").AddComponent<LobbyManager>();
                    LobbyController.OnLobbyUpdated += _instance.bindForward;
                }
                return _instance;
            }
        }
        #endregion

        #region Fields
        [field: SerializeField] private string playerName;
        public string PlayerName { private get => playerName; set => playerName = value.Trim(); }
        public Lobby CurrentLobby { get; private set; }
        private CancellationTokenSource cts = new();
        public event Action<Lobby> OnLobbyUpdated;
        private void bindForward(Lobby lobby) => OnLobbyUpdated?.Invoke(lobby);
        #endregion

        void OnDestroy() => cts?.Cancel();
        public async void CreateLobby(int maxPlayersCount, string roomName)
        {
            CurrentLobby = await LobbyController.CreateLobby(maxPlayersCount, roomName, LobbyController.GetPlayerData(PlayerName), cts.Token);
        }

        public async void JoinLobbyById(string lobbyId)
        {
            CurrentLobby = await LobbyController.JoinLobbyById(lobbyId, LobbyController.GetPlayerData(PlayerName));
        }

        internal Task<List<Lobby>> GetLobbies(int numOfLobbiesToFetch = 10) => LobbyController.GetLobbies(numOfLobbiesToFetch);

        public void KickPlayer(string lobbyId, string playerId) => LobbyController.KickPlayer(lobbyId, playerId); // TODO: should wait until player is kick in order to update the UI (free the slot)

        internal void LeaveLobby() => LobbyController.LeaveLobby(CurrentLobby, cts);

        internal void SendReady(bool isReady) => LobbyController.SendReady(CurrentLobby.Id, isReady);
    }
}