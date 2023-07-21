using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class CanvasStateNotifier : MonoBehaviour
    {
        [Header("Event Raiser when successful")]
        [SerializeField] private CanvasChangedEvent canvasChangedEvent;
        [SerializeField] private CanvasState nextCanvasState;

        public void OnClickChangeCanvas() => canvasChangedEvent.Raise(nextCanvasState);
    }
}
