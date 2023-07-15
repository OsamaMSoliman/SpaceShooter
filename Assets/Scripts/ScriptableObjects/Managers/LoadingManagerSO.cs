using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Manager/LoadingManager")]
    public class LoadingManagerSO : ScriptableObject
    {
        [field: SerializeField] public float ProgressValue { get; private set; }
        public void LoadSceneWithLoadingScreen(MonoBehaviour caller, int sceneIndex) => caller.StartCoroutine(LoadLevel(sceneIndex));

        private IEnumerator LoadLevel(int sceneIndex)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            while (!asyncOperation.isDone)
            {
                yield return null;
                ProgressValue = asyncOperation.progress;
            }
        }
    }
}