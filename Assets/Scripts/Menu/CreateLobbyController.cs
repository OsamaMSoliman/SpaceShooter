using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class CreateLobbyController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomName;

        [Header("Dependencies ?")] // use ? when accessing them
        [SerializeField] private LobbyManagerSO lobbyManagerSO;
        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;

        public async void OnClickCreateNewLobby()
        {
            if (string.IsNullOrEmpty(roomName.text))
            {
                // highlight the inputfield in red to indecated an error
                roomName.GetComponent<Outline>().enabled = true;
            }
            else
            {
                await lobbyManagerSO.CreateLobby();
                canvasStateNotifier.OnClickChangeCanvas();
            }
        }

    }
}
