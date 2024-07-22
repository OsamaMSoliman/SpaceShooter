namespace Nsr.MultiSpaceShooter
{
    public class CanvasStateListener : CanvasStatePersistentListener
    {
        protected override void InvokedMethod(CanvasState canvasState)
        {
            gameObject.SetActive(myCanvasState == canvasState);
        }
    }
}
