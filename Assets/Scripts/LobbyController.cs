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
        /// <summary>
        /// limit is 5 requests per 30 seconds	( https://docs.unity.com/lobby/en-us/manual/rate-limits)
        /// </summary>
        public static int HeartbeatInterval = 15;
        /// <summary>
        /// limit is 1 request per second	( https://docs.unity.com/lobby/en-us/manual/rate-limits)
        /// </summary>
        public static int PollingInterval = 5;

        #region Relay
        // https://docs.unity.com/ugs/en-us/manual/relay/manual/relay-and-ngo
        public static async Task<string> CreateRelayAllocation(int MaxPlayersCount)
        {
            // NOTE: The host is included in the MaxPlayersCount but maxConnections is without the host
            var allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayersCount - 1, "europe-west4");
            NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
            // NOTE: Must start the host directly to bind to the relay allocation!!!
            return NetworkManager.Singleton?.StartHost() == true ? await RelayService.Instance.GetJoinCodeAsync(allocationId: allocation.AllocationId) : null;
        }

        private static async Task<bool> JoinRelayAllocation(string joinCode)
        {
            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                NetworkManager.Singleton?.GetComponent<UnityTransport>().SetRelayServerData(new(allocation, "dtls"));
                return NetworkManager.Singleton?.StartClient() == true;
            }
            catch (RelayServiceException e)
            {
                Debug.Log($"Failed to Join allocation: {nameof(e.Reason)} : {e.Message}");
                return false;
            }
        }
        #endregion

        #region Lobby

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
        public static async Task<Lobby> CreateLobby(string lobbyName, int maxPlayers, Player player, CancellationToken ct, Action<Lobby> periodicUpdateCb)
        {
            var createdLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, new() { Player = player });
            Debug.Log($"{createdLobby.Name} is created with lobby code: {createdLobby.LobbyCode}");

            PeriodicHeartBeat(createdLobby, ct);
            PeriodicPolling(createdLobby, ct, periodicUpdateCb);

            return createdLobby;
        }

        public static async Task<Lobby> JoinLobbyById(string lobbyId, Player player, CancellationToken ct, Action<Lobby> periodicUpdateCb)
        {
            var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, new JoinLobbyByIdOptions { Player = player });
            PeriodicPolling(joinedLobby, ct, periodicUpdateCb);
            return joinedLobby;
        }

        [ObsoleteAttribute("NOTE: Code is only available to lobby members (so far players can't join a lobby through typing its Code)", true)]
        public static async Task<Lobby> JoinLobbyByCode(string lobbyCode, Player player, CancellationToken ct, Action<Lobby> periodicUpdateCb)
        {
            var joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions { Player = player });
            PeriodicPolling(joinedLobby, ct, periodicUpdateCb);
            return joinedLobby;
        }

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
                    await KickPlayer(lobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Leaving Lobby {lobby.LobbyCode} throws: {e}");
            }
        }

        /// <summary>
        /// Kicking a player from the lobby and return when the task is done.
        /// </summary>
        public static async Task KickPlayer(string lobbyId, string playerId) => await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);

        /// <summary>
        /// Sends the Relay Code to the lobby members and locks it so that no other players can join.
        /// </summary>
        public static async Task<Lobby> LockLobby(string lobbyId, string relayCode)
        {
            try
            {
                var lockedLobby = await Lobbies.Instance.UpdateLobbyAsync(lobbyId, new UpdateLobbyOptions
                {
                    IsLocked = true,
                    Data = new Dictionary<string, DataObject> {
                        { Constants.RELAY_ALLOCATION_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });
                return lockedLobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log($"Failed locking the lobby due to: {e}");
                return null;
            }
        }

        public static async Task<Lobby> SendReady(string lobbyId, bool isReady) => await LobbyService.Instance.UpdatePlayerAsync(lobbyId, AuthenticationManager.PlayerId,
                    new UpdatePlayerOptions
                    {
                        Data = new(){
                        { Constants.IS_READY, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, isReady.ToString()) }
                            }
                    });
        #endregion


        #region Periodic calls

        /// <summary>
        /// Periodic Heartbeat to keep the lobby alive.
        /// </summary>
        private static async void PeriodicHeartBeat(Lobby lobby, CancellationToken ct)
        {
            if (lobby.HostId == AuthenticationManager.PlayerId)
            {
                try
                {
                    while (!ct.IsCancellationRequested)
                    {
                        await Task.Delay(HeartbeatInterval * 1000, ct);
                        Debug.Log("Heartbeat");
                        await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                    }
                }
                catch (TaskCanceledException)
                {
                    Debug.Log("Heartbeat stopped!");
                }
            }
        }

        /// <summary>
        /// Periodic Polling as a way of refreshing the current lobby reference.
        /// </summary>
        private static async void PeriodicPolling(Lobby lobby, CancellationToken ct, Action<Lobby> updateCallback)
        {
            updateCallback?.Invoke(lobby);
            try
            {
                while (lobby != null && !ct.IsCancellationRequested)
                {
                    await Task.Delay(PollingInterval * 1000, ct);
                    Debug.Log("Polling");
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                    updateCallback?.Invoke(lobby);

                    // NOTE: when the player is kicked and the lobby is not deleted, the player can still see the lobby's public data (list of players is not public)
                    // handle the case where the player is not in the lobby (kicked or left)
                    if (lobby.Players == null || !lobby.Players.Exists(p => p.Id == AuthenticationManager.PlayerId))
                    {
                        Debug.Log("You are not in the lobby anymore!");
                        updateCallback?.Invoke(null); // notify the UI
                        lobby = null; // stop polling
                    }
                    // handle the case when the allocation code is sent and if player is not host then join relay
                    if (lobby.Data.TryGetValue(Constants.RELAY_ALLOCATION_JOIN_CODE, out var relayCode))
                    {
                        if (lobby.HostId != AuthenticationManager.PlayerId)
                        {
                            Debug.Log($"Joining Relay with Code: {relayCode.Value}");
                            await JoinRelayAllocation(relayCode.Value); // join the relay and start the game for the client
                            lobby = null; // stop polling
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                Debug.Log("Polling stopped!");
            }
        }

        #endregion


        /// <summary>
        /// Get a player object with the given name.
        /// NOTE: The player object can hold data like the player's name, ready state, etc.
        /// </summary>
        public static Player GetPlayer(PlayerData playerData) => new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                    { Constants.PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerData.PlayerName) }
                }
        };

    }
}