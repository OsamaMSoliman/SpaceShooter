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
            // TODO: FetchLobbiesPeriodically ??
        }

        public void OnClickRefreshLobbies()
        {
            // TODO: FetchLobbies(); ??
        }


        private async void FetchLobbies()
        {
            // TODO: clear the lobbies that are already there

            var lobbies = await lobbyManagerSO?.GetPublicLobbies();
            foreach (var lobby in lobbies)
            {
                JoinLobbyBtn joinLobbyBtn = Instantiate(joinLobbyBtnPrefab, parent: scrollContent);
                joinLobbyBtn.Init(
                    lobby.Name,
                    lobby.Id,
                    lobby.Players.Count,
                    lobby.MaxPlayers,
                    () => lobbyManagerSO?.JoinLobbyById(lobby.Id)
                );
            }
        }

    }
}
