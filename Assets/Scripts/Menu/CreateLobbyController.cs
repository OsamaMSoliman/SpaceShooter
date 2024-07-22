using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class CreateLobbyController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomNameField;
        private string _roomName => roomNameField.text.Trim();

        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;

        [field: SerializeField] public int maxPlayersCount { private get; set; } = 2;

        public void OnClickCreateNewLobby()
        {
            if (string.IsNullOrEmpty(_roomName))
            {
                // highlight the inputfield in red to indecated an error
                roomNameField.GetComponent<Outline>().enabled = true;
            }
            else
            {
                Debug.Log($"Creating new lobby with room name: {_roomName} and max players count: {maxPlayersCount}");
                LobbyManager.Instance.CreateLobby(maxPlayersCount, _roomName);
                canvasStateNotifier.OnClickChangeCanvas();
            }
        }

    }
}
