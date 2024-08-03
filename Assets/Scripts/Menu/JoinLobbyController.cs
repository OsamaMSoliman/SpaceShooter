using System.Threading.Tasks;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class JoinLobbyController : MonoBehaviour
    {
        [SerializeField] private JoinLobbyBtn templateBtn;
        [SerializeField] private Transform scrollContent;
        [SerializeField] private GameObject loadingSpinner;
        [SerializeField] private int refreshTime = 15;
        [SerializeField] private int _numOfLobbiesToFetch = 6;

        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;

        // private void OnEnable() => InvokeRepeating(nameof(FetchLobbiesPeriodically), 2, refreshTime);

        // private void OnDisable() => CancelInvoke(nameof(FetchLobbiesPeriodically));

        private void Awake() => templateBtn.gameObject.SetActive(false);
        public async void OnClick_RefreshBtn_FetchLobbies()
        // private async void FetchLobbiesPeriodically()
        {
            // TODO: ObjectPooling
            foreach (Transform item in scrollContent)
            {
                if (item == templateBtn.transform) continue;
                Destroy(item.gameObject);
            }

            var lobbies = await LobbyManager.Instance.GetLobbies(_numOfLobbiesToFetch);

            loadingSpinner.SetActive(lobbies.Count == 0);

            foreach (var lobby in lobbies)
            {
                Debug.Log($"{lobby.Name}, {lobby.Id}, {lobby.LobbyCode}, {lobby.Players.Count}, {lobby.MaxPlayers}");
                JoinLobbyBtn joinLobbyBtn = Instantiate(templateBtn, scrollContent);
                joinLobbyBtn.gameObject.SetActive(true);
                joinLobbyBtn.Init(lobby,
                    async () =>
                    {
                        await LobbyManager.Instance.JoinLobbyById(lobby.Id);
                        canvasStateNotifier.OnClickChangeCanvas();
                    }
                );
            }
        }
    }
}
