using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class CreateLobbyController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField roomNameField;
        [SerializeField] private LobbyData lobbyData;
        [SerializeField] private GameObject createBtn, loadingSpinner;

        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasStateNotifier canvasStateNotifier;


        private void OnEnable()
        {
            // reset the UI
            roomNameField.text = "";
            roomNameField.GetComponent<Outline>().enabled = false;
            ResetVisibility(true);
        }

        private void ResetVisibility(bool isVisible)
        {
            createBtn.SetActive(isVisible);
            loadingSpinner.SetActive(!isVisible);
        }

        public async void OnClickCreateNewLobby()
        {
            if (string.IsNullOrEmpty(lobbyData.LobbyName))
            {
                // highlight the inputfield in red to indecated an error
                roomNameField.GetComponent<Outline>().enabled = true;
            }
            else
            {
                ResetVisibility(false);
                await LobbyManager.Instance.CreateLobby();
                canvasStateNotifier.OnClickChangeCanvas();
            }
        }

    }
}
