using UnityEngine;
using UnityEngine.Events;

namespace Nsr.MultiSpaceShooter
{
    public class BaseGameEvent<T> : ScriptableObject
    {
        [field: SerializeField] public T CurrentValue { get; private set; }
        private event UnityAction<T> unityEvent;
        public void Raise(T value) => unityEvent.Invoke(CurrentValue = value);
        public void AddListener(UnityAction<T> action) => unityEvent += action;
        public void RemoveListener(UnityAction<T> action) => unityEvent -= action;

    }
}
