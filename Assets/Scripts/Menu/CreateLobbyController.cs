using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class CreateLobbyController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomNameField;
        private string _roomName => roomNameField.text.Trim();
        [field: SerializeField] public int maxPlayersCount { private get; set; } = 2;
        [SerializeField] private GameObject createBtn, loadingSpinner;

        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;


        private void OnEnable()
        {
            // reset the UI
            roomNameField.text = "";
            roomNameField.GetComponent<Outline>().enabled = false;
            createBtn.SetActive(true);
            loadingSpinner.SetActive(false);
        }

        public async void OnClickCreateNewLobby()
        {
            if (string.IsNullOrEmpty(_roomName))
            {
                // highlight the inputfield in red to indecated an error
                roomNameField.GetComponent<Outline>().enabled = true;
            }
            else
            {
                await LobbyManager.Instance.CreateLobby(maxPlayersCount, _roomName); // TODO: add the spinner here
                canvasStateNotifier.OnClickChangeCanvas();
            }
        }

    }
}
