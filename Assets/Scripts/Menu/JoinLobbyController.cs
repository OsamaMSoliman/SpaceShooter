using System.Threading.Tasks;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyController : MonoBehaviour
    {
        [SerializeField] private JoinLobbyBtn joinLobbyBtnPrefab;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private int refreshTime = 15;
        [SerializeField] private int _numOfLobbiesToFetch = 6;

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

            var lobbies = await LobbyManager.Instance.GetLobbies(_numOfLobbiesToFetch);

            foreach (var lobby in lobbies)
            {
                Debug.Log($"{lobby.Name}, {lobby.Id}, {lobby.LobbyCode}, {lobby.Players.Count}, {lobby.MaxPlayers}");
                JoinLobbyBtn joinLobbyBtn = Instantiate(joinLobbyBtnPrefab, scrollContent);
                joinLobbyBtn.Init(
                    lobby.Name,
                    lobby.GetHostName(),
                    lobby.Players.Count,
                    lobby.MaxPlayers,
                    () =>
                    {
                        LobbyManager.Instance.JoinLobbyById(lobby.Id);
                        canvasStateNotifier.OnClickChangeCanvas();
                    }
                );
            }
        }

    }
}
