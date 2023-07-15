using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    [CreateAssetMenu(menuName = "SOs/Events/CanvasChangedEvent")]
    public class CanvasChangedEvent : BaseGameEvent<CanvasState>
    {
        [field: SerializeField] private CanvasState defaultValue;
        private void OnEnable() => CurrentValue = defaultValue;
    }
}