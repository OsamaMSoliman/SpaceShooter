using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Manager/CanvasManagerSO")]
    public class CanvasManagerSO : BaseGameEvent<CanvasState>
    {
        public CanvasState currentState { get; set; }
    }
}