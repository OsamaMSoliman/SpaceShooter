using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class LoadingUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Color pingColor;
        [SerializeField] private Color pongColor;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;

        [Header("Dependencies")] // use ? when accessing them
        [SerializeField] private LoadingManagerSO loadingManagerSO;


        private void Update()
        {
            loadingText.color = Color.Lerp(pingColor, pongColor, Mathf.PingPong(Time.time, 1));
            progressBar.value = loadingManagerSO?.ProgressValue ?? default;
            progressText.text = loadingManagerSO?.ProgressValue.ToString() + " %";
        }

    }
}