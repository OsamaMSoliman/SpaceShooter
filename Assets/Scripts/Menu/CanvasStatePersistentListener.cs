using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public abstract class CanvasStatePersistentListener : MonoBehaviour
    {
        [SerializeField] private CanvasManagerSO canvasManagerSO;
        [SerializeField] protected CanvasState myCanvasState;

        private void Awake()
        {
            gameObject.SetActive(myCanvasState == canvasManagerSO.currentState);
            canvasManagerSO.AddListener(InvokedMethod);
            Debug.Log(gameObject.name);
        }
        // private void Awake() => canvasStateChangedEvent.AddListener(InvokedMethod);
        private void OnDestroy() => canvasManagerSO.RemoveListener(InvokedMethod);

        protected abstract void InvokedMethod(CanvasState canvasState);
    }
}
