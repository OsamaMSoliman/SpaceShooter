using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class CanvasStateListener : CanvasStatePersistentListener
    {
        
        protected override void InvokedMethod(CanvasState canvasState)
        {
            Debug.Log($"{gameObject.name} {myCanvasState == canvasState}");
            gameObject.SetActive(myCanvasState == canvasState);
        }
    }
}
