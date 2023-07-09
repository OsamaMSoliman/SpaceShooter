using UnityEngine;
using UnityEngine.UI;

public class LoadingUIController : MonoBehaviour
{
    [SerializeField] private Text loadingText;
    [SerializeField] private Color pingColor;
    [SerializeField] private Color pongColor;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;

    [Header("External")]
    [SerializeField] private LoadingManager loadingManager;


    private void Update()
    {
        loadingText.color = Color.Lerp(pingColor, pongColor, Mathf.PingPong(Time.time, 1));
        progressBar.value = loadingManager.ProgressValue;
        progressText.text = loadingManager.ProgressValue.ToString() + " %";
    }

}
