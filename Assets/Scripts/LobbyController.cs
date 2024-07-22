using System;
using System.Collections.Generic;
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
    public static class LobbyController
    {
        public static event Action<Lobby> OnLobbyUpdated;
        /// <summary>
        /// limit is 5 requests per 30 seconds	( https://docs.unity.com/lobby/en-us/manual/rate-limits)
        /// </summary>
        public static int HeartbeatInterval = 15;
        /// <summary>
        /// limit is 1 request per second	( https://docs.unity.com/lobby/en-us/manual/rate-limits)
        /// </summary>
        public static int PollingInterval = 5;

        public static async Task<Lobby> CreateLobby(int MaxPlayersCount, string RoomName, Player player, CancellationToken ct)
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayersCount, "europe-west4");
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var options = new CreateLobbyOptions
            {
                Player = player,
                Data = new Dictionary<string, DataObject> {
                    { Constants.ALLOCATION_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            Debug.Log($"Create:ALLOCATION_JOIN_CODE:{options.Data[Constants.ALLOCATION_JOIN_CODE].Value},{options.Data[Constants.ALLOCATION_JOIN_CODE].Value.Length}");

            var createdLobby = await LobbyService.Instance.CreateLobbyAsync(RoomName, MaxPlayersCount, options);
            Debug.Log($"Lobby created with Code: {createdLobby.LobbyCode}");

            NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
            // NOTE: Must start the host directly to bind to the relay allocation!!!
            NetworkManager.Singleton?.StartHost();

            PeriodicHeartBeat(createdLobby, ct);
            PeriodicPolling(createdLobby, ct);

            return createdLobby;
        }

        #region Joining a Lobby
        public static async Task<Lobby> JoinLobbyById(string lobbyId, Player player, CancellationToken ct)
        {
            var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = player });
            await JoinRelayAllocation(joinedLobby, ct);
            return joinedLobby;
        }

        [ObsoleteAttribute("NOTE: Code is only available to lobby members (so far players can't join a lobby through typing its Code)", true)]
        public static async Task<Lobby> JoinLobbyByCode(string lobbyCode, Player player, CancellationToken ct)
        {
            var joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions { Player = player });
            await JoinRelayAllocation(joinedLobby, ct);
            return joinedLobby;
        }

        private static async Task JoinRelayAllocation(Lobby joinedLobby, CancellationToken ct)
        {
            try
            {
                Debug.Log($"Lobby joined with Code: {joinedLobby.LobbyCode}");
                Debug.Log($"Join:ALLOCATION_JOIN_CODE:{joinedLobby.Data[Constants.ALLOCATION_JOIN_CODE].Value},{joinedLobby.Data[Constants.ALLOCATION_JOIN_CODE].Value.Length}");

                var allocation = await RelayService.Instance.JoinAllocationAsync(joinedLobby.Data[Constants.ALLOCATION_JOIN_CODE].Value);
                Debug.Log($"Lobby:{joinedLobby.Id}:{joinedLobby.LobbyCode}\n Allocation:{allocation.AllocationId}\n Players count: {joinedLobby.Players.Count}");

                NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
                NetworkManager.Singleton?.StartClient();

                PeriodicPolling(joinedLobby, ct);
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
        public static async void LeaveLobby(Lobby lobby, CancellationTokenSource cts)
        {
            cts?.Cancel();
            if (lobby == null) return;
            try
            {
                var playerId = AuthenticationManager.PlayerId;
                if (playerId == lobby.HostId)
                    await LobbyService.Instance.DeleteLobbyAsync(lobby.Id);
                else
                    KickPlayer(lobby.Id, playerId);
                lobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Leaving Lobby {lobby.LobbyCode} throws: {e}");
            }
        }

        /// <summary>
        /// Get public lobbies filtered with (AvailableSlots > 0 && !IsLocked)
        /// </summary>
        public static async Task<List<Lobby>> GetLobbies(int numOfLobbbiesToFetch)
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


        /// <summary>
        /// Updates the lobby IsLocked = true, so no other players can join.
        /// </summary>
        public static async Task LockLobby(Lobby lobby)
        {
            try
            {
                await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions { IsLocked = true });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Failed locking the lobby due to: {e}");
            }
        }

        #region Lock Periodic(HeartBeat & Poling)

        /// <summary>
        /// Periodic Heartbeat to keep the lobby alive.
        /// </summary>
        private static async void PeriodicHeartBeat(Lobby lobby, CancellationToken ct)
        {
            if (lobby.HostId == AuthenticationManager.PlayerId)
            {
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(HeartbeatInterval * 1000, ct);
                    Debug.Log("Heartbeat");
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                }
            }
        }

        /// <summary>
        /// Periodic Polling as a way of refreshing the current lobby reference.
        /// </summary>
        private static async void PeriodicPolling(Lobby lobby, CancellationToken ct)
        {
            OnLobbyUpdated?.Invoke(lobby);
            while (lobby != null && !ct.IsCancellationRequested)
            {
                await Task.Delay(PollingInterval * 1000, ct);
                Debug.Log("Polling");
                lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                OnLobbyUpdated?.Invoke(lobby);
            }
        }

        #endregion

        public static async void SendReady(string lobbyId, bool isReady) => await LobbyService.Instance.UpdatePlayerAsync(lobbyId, AuthenticationManager.PlayerId,
            new UpdatePlayerOptions
            {
                Data = new(){
                        { Constants.IS_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                    }
            });

        public static async void KickPlayer(string lobbyId, string playerId) => await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);

        public static Player GetPlayerData(string PlayerName) => new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                    { Constants.PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerName) }
                }
        };

    }
}