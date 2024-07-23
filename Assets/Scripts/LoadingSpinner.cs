using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class LoadingSpinner : MonoBehaviour
    {
        [SerializeField] private float rotation = 1;
        private void OnEnable() => StartCoroutine(Rotate());
        private void OnDisable() => StopAllCoroutines();
        private IEnumerator Rotate()
        {
            while (true)
            {
                yield return null;
                transform.Rotate(Vector3.forward, rotation, Space.Self);
            }
        }
    }
}
