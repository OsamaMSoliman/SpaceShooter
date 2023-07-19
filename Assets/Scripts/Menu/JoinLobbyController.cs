using System.Threading.Tasks;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyController : MonoBehaviour
    {
        [SerializeField] private JoinLobbyBtn joinLobbyBtnPrefab;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private int refreshTime = 15;

        [Header("Dependencies")]
        [SerializeField] private LobbyManagerSO lobbyManagerSO;
        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;

        private Task periodicFetch;
        private void OnEnable() => InvokeRepeating(nameof(FetchLobbiesPeriodically), 2, refreshTime);

        private void OnDisable() => CancelInvoke(nameof(FetchLobbiesPeriodically));

        private async void FetchLobbiesPeriodically()
        {
            // TODO: ObjectPooling
            foreach (Transform item in scrollContent)
            {
                Destroy(item.gameObject);
            }

            var lobbies = await lobbyManagerSO.GetLobbies();

            foreach (var lobby in lobbies)
            {
                Debug.Log($"{lobby.Name}, {lobby.Id}, {lobby.LobbyCode}, {lobby.Players.Count}, {lobby.MaxPlayers} ");
                JoinLobbyBtn joinLobbyBtn = Instantiate(joinLobbyBtnPrefab, scrollContent);
                joinLobbyBtn.Init(
                    lobby.Name,
                    lobby.GetHostName(),
                    lobby.Players.Count,
                    lobby.MaxPlayers,
                    async () =>
                    {
                        await lobbyManagerSO.JoinLobbyById(lobby.Id);
                        canvasStateNotifier.OnClickChangeCanvas();
                    }
                );
            }
        }

    }
}
