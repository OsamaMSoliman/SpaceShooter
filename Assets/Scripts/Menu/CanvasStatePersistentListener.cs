using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public abstract class CanvasStatePersistentListener : MonoBehaviour
    {
        [SerializeField] private CanvasChangedEvent canvasChangedEvent;
        [SerializeField] protected CanvasState myCanvasState;

        private void Awake()
        {
            gameObject.SetActive(myCanvasState == canvasChangedEvent.CurrentValue);
            canvasChangedEvent.AddListener(InvokedMethod);
        }
        private void OnDestroy() => canvasChangedEvent.RemoveListener(InvokedMethod);

        protected abstract void InvokedMethod(CanvasState canvasState);
    }
}
