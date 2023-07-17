using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Manager/LobbyManagerSO")]
    public class LobbyManagerSO : ScriptableObject
    {
        #region Room
        [field: SerializeField] public string RoomName { private get; set; }
        [field: SerializeField] public int MaxPlayersCount { private get; set; }
        #endregion

        public Lobby CurrentLobby { get; private set; }
        private CancellationTokenSource cts;
        private const int HeartbeatInterval = 15;

        private void OnEnable()
        {
            MaxPlayersCount = 2;
            RoomName = "";
            // currentLobby = null;
        }

        public async void CreateLobby()
        {
            CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(RoomName, MaxPlayersCount);
            Debug.Log("Lobby.Id: " + CurrentLobby.Id);

            cts = new CancellationTokenSource();
            PeriodicHeartBeat(cts.Token);
            EnterLobbyRoom();
        }

        public async Task<List<Lobby>> GetPublicLobbies()
        {
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            Debug.Log("Available Lobbies: " + queryResponse.Results.Count);
            return queryResponse.Results;
        }

        private async void PeriodicHeartBeat(CancellationToken ct)
        {
            if (CurrentLobby.HostId == AuthenticationManagerSO.PlayerId)
            {
                while (CurrentLobby != null && !ct.IsCancellationRequested)
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
                    await Task.Delay(HeartbeatInterval * 1000);
                }
            }
        }

        public void EnterLobbyRoom()
        {
            // TODO: 
        }

        public async void JoinLobbyById(string lobbyId)
        {
            CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log($"Number of players: {CurrentLobby.Players.Count} in Lobby {CurrentLobby.Id}");
            EnterLobbyRoom();
        }
    }
}