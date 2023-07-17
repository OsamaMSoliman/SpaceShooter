using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyController : MonoBehaviour
    {
        [SerializeField] private JoinLobbyBtn joinLobbyBtnPrefab;
        [SerializeField] private Transform scrollContent;

        [Header("Dependencies ?")] // use ? when accessing them
        [SerializeField] private LobbyManagerSO lobbyManagerSO;

        private void OnEnable()
        {
            // TODO:
            // lobbyManagerSO.FetchLobbiesPeriodically(true);
            // lobbyManagerSO.FetchedLobbies += FetchLobbies;
        }

        private void OnDisable()
        {
            // TODO:
            // lobbyManagerSO.FetchLobbiesPeriodically(false);
            // lobbyManagerSO.FetchedLobbies -= FetchLobbies;
        }

        // public async void OnClickRefreshLobbies()
        // {
        //     var lobbies = await lobbyManagerSO?.GetPublicLobbies();
        //     FetchLobbies(lobbies);
        // }

        private void FetchLobbies(List<Lobby> lobbies)
        {
            // TODO: ObjectPooling
            foreach (Transform item in scrollContent)
            {
                Destroy(item);
            }

            foreach (var lobby in lobbies)
            {
                JoinLobbyBtn joinLobbyBtn = Instantiate(joinLobbyBtnPrefab, parent: scrollContent);
                joinLobbyBtn.Init(
                    lobby.Name,
                    lobby.LobbyCode,
                    lobby.Players.Count,
                    lobby.MaxPlayers,
                    () => lobbyManagerSO?.JoinLobbyById(lobby.Id)
                );
            }
        }

    }
}
