using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    // interface ILobbyManager
    // {
    //     Task<List<Lobby>> GetLobbies();
    //     Task CreateLobby(); //string name, int maxCapacity
    //     Task JoinLobby(); //string lobbyCode or lobbyId
    //     Task LeaveLobby();

    //     void PeriodicHeartbeat();
    //     Task LockLobby();
    //     void PeriodicRefresh();
    // }

    [CreateAssetMenu(menuName = "SOs/Manager/LobbyManagerSO")]
    public class LobbyManagerSO : ScriptableObject
    {
        #region Room
        [field: SerializeField] private string playerName;
        public string PlayerName { private get => playerName; set => playerName = value.Trim(); }
        [field: SerializeField] private string roomName;
        public string RoomName { private get => roomName; set => roomName = value.Trim(); }
        [field: SerializeField] public int MaxPlayersCount { private get; set; }
        [field: SerializeField] private int numOfLobbbiesToFetch = 8;
        private void OnEnable()
        {
            MaxPlayersCount = 2;
            RoomName = "";
            LeaveLobby();
        }
        private void OnDisable() => cts?.Cancel(); // TODO: heartbeat and polling doesn't stop when stopping the game?!
        #endregion


        #region Constants
        // NOTE: https://docs.unity.com/lobby/en-us/manual/rate-limits
        private const int HeartbeatInterval = 15; // 5 requests per 30 seconds	
        private const int PollingInterval = 5; // 1 request per second
        private const string ALLOCATION_JOIN_CODE = "J";
        #endregion

        public Lobby CurrentLobby { get; private set; }
        public event Action<Lobby> OnLobbyUpdated;
        private CancellationTokenSource cts;

        #region Create Join Leave GetLobbies

        public async Task CreateLobby()
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayersCount, "europe-west4");
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var options = new CreateLobbyOptions
            {
                Player = GetPlayerData(),
                Data = new Dictionary<string, DataObject> {
                    { ALLOCATION_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            Debug.Log($"Create:ALLOCATION_JOIN_CODE:{options.Data[ALLOCATION_JOIN_CODE].Value},{options.Data[ALLOCATION_JOIN_CODE].Value.Length}");

            CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(RoomName, MaxPlayersCount, options);
            Debug.Log($"Lobby created with Code: {CurrentLobby.LobbyCode}");

            NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
            // NOTE: Must start the host directly to bind to the relay allocation!!!
            NetworkManager.Singleton?.StartHost();

            cts = new CancellationTokenSource();
            PeriodicHeartBeat(cts.Token);
            PeriodicPolling(cts.Token);
        }

        #region Joining a Lobby
        public async Task JoinLobbyById(string lobbyId)
        {
            CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = GetPlayerData() });
            await JoinAllocation();
        }

        [ObsoleteAttribute("NOTE: Code is only available to lobby members (so far players can't join a lobby through typing its Code)", true)]
        public async Task JoinLobbyByCode(string lobbyCode)
        {
            CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions { Player = GetPlayerData() });
            await JoinAllocation();
        }

        private async Task JoinAllocation()
        {
            try
            {
                Debug.Log($"Lobby joined with Code: {CurrentLobby.LobbyCode}");
                Debug.Log($"Join:ALLOCATION_JOIN_CODE:{CurrentLobby.Data[ALLOCATION_JOIN_CODE].Value},{CurrentLobby.Data[ALLOCATION_JOIN_CODE].Value.Length}");

                var allocation = await RelayService.Instance.JoinAllocationAsync(CurrentLobby.Data[ALLOCATION_JOIN_CODE].Value);
                Debug.Log($"Lobby:{CurrentLobby.Id}:{CurrentLobby.LobbyCode}\n Allocation:{allocation.AllocationId}\n Players count: {CurrentLobby.Players.Count}");

                NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
                NetworkManager.Singleton?.StartClient();

                cts = new CancellationTokenSource();
                PeriodicPolling(cts.Token);
            }
            catch (RelayServiceException e)
            {
                Debug.Log($"Failed to Join allocation: {nameof(e.Reason)} : {e.Message}");
            }
        }
        #endregion

        /// <summary>
        /// Leaving the lobby if a member and deleting it if the Host
        /// </summary>
        public async void LeaveLobby()
        {
            cts?.Cancel();
            if (CurrentLobby == null) return;
            try
            {
                var playerId = AuthenticationManagerSO.PlayerId;
                if (playerId == CurrentLobby.HostId)
                    await LobbyService.Instance.DeleteLobbyAsync(CurrentLobby.Id);
                else
                    await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, playerId);
                CurrentLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Leaving Lobby {CurrentLobby.LobbyCode} throws: {e}");
            }
        }

        /// <summary>
        /// Get public lobbies filtered with (AvailableSlots > 0 && !IsLocked)
        /// </summary>
        public async Task<List<Lobby>> GetLobbies()
        {
            var filterOptions = new QueryLobbiesOptions
            {
                Count = numOfLobbbiesToFetch,
                Filters = new List<QueryFilter> {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                        new(QueryFilter.FieldOptions.IsLocked, "0", QueryFilter.OpOptions.EQ)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(filterOptions);
            Debug.Log($"{queryResponse.Results.Count} available lobbies!");
            return queryResponse.Results;
        }

        #endregion

        #region Lock Periodic(HeartBeat & Poling)

        /// <summary>
        /// Updates the lobby IsLocked = true, so no other players can join.
        /// </summary>
        public async Task LockLobby()
        {
            try
            {
                await Lobbies.Instance.UpdateLobbyAsync(CurrentLobby.Id, new UpdateLobbyOptions { IsLocked = true });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Failed locking the lobby due to: {e}");
            }
        }

        /// <summary>
        /// Periodic Heartbeat to keep the lobby alive.
        /// </summary>
        private async void PeriodicHeartBeat(CancellationToken ct)
        {
            if (CurrentLobby.HostId == AuthenticationManagerSO.PlayerId)
            {
                await Task.Delay(HeartbeatInterval * 1000);
                while (CurrentLobby != null && !ct.IsCancellationRequested)
                {
                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(CurrentLobby.Id);
                    // NOTE: token cancellation dictates that the wait must be at the end of the loop
                    await Task.Delay(HeartbeatInterval * 1000);
                }
            }
        }

        /// <summary>
        /// Periodic Polling as a way of refreshing the current lobby reference.
        /// </summary>
        private async void PeriodicPolling(CancellationToken ct)
        {
            await Task.Delay(PollingInterval * 1000);
            while (CurrentLobby != null && !ct.IsCancellationRequested)
            {
                Debug.Log(message: "Polling");
                CurrentLobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);
                OnLobbyUpdated?.Invoke(CurrentLobby);
                // NOTE: token cancellation dictates that the wait must be at the end of the loop
                await Task.Delay(PollingInterval * 1000);
            }
        }

        #endregion

        #region additional methods

        public async void SendReady(bool isReady)
        {
            var playerUpdate = new UpdatePlayerOptions
            {
                Data = new(){
                    { Constants.IS_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                }
            };

            CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, AuthenticationManagerSO.PlayerId, playerUpdate);
        }

        public async void KickPlayer(string playerId)
        {
            await LobbyService.Instance.RemovePlayerAsync(CurrentLobby.Id, playerId);
        }

        private Player GetPlayerData()
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject> {
                    { Constants.PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName) }
                }
            };
        }

        public void UseCancelToken() => cts?.Cancel();
        #endregion
    }
}