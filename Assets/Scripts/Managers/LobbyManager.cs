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
                }
                return _instance;
            }
        }
        #endregion

        #region Fields
        [SerializeField] private PlayerData playerData;
        [SerializeField] private LobbyData lobbyData;
        public Lobby CurrentLobby { get; private set; }
        public event Action<Lobby> OnLobbyUpdated;
        private CancellationTokenSource cts = new();
        private void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        #endregion

        # region public Methods
        // NOTE: this waits for the lobby to be created before returning
        public async Task CreateLobby() =>
            CurrentLobby = await LobbyController.CreateLobby(lobbyData.LobbyName, lobbyData.MaxPlayers, LobbyController.GetPlayer(playerData), cts.Token, OnLobbyUpdated);

        // NOTE: this waits for the lobby to be joined before returning
        public async Task JoinLobbyById(string lobbyId) =>
            CurrentLobby = await LobbyController.JoinLobbyById(lobbyId, LobbyController.GetPlayer(playerData), ct: cts.Token, OnLobbyUpdated);

        public Task<List<Lobby>> GetLobbies(int numOfLobbiesToFetch = 10) => LobbyController.GetLobbies(numOfLobbiesToFetch);

        public Task KickPlayer(string lobbyId, string playerId) => LobbyController.KickPlayer(lobbyId, playerId);

        public void LeaveLobby() => LobbyController.LeaveLobby(CurrentLobby, cts);

        public async void SendReady(bool isReady) => await LobbyController.SendReady(CurrentLobby.Id, isReady);

        public async void StartGame()
        {
            if (CurrentLobby.HostId != AuthenticationManager.PlayerId) return;
            string relayCode = await LobbyController.CreateRelayAllocation(CurrentLobby.MaxPlayers);
            await LobbyController.LockLobby(CurrentLobby.Id, relayCode);
            // NOTE: the lobby shouldn't be destroyed here directly, it should be destroyed later, after the rest of the players enter the game
        }
        #endregion

    }
}