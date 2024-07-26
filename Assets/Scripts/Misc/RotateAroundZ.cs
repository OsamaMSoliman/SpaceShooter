using System.Collections;
using UnityEngine;

namespace Nsr.MultiSpaceShooter
{
    public class RotateAroundZ : MonoBehaviour
    {
        [SerializeField] private bool rotateOnStart;
        private void Awake()
        {
            if (rotateOnStart) StartRotation(true);
        }

        public void StartRotation(bool forever) => StartCoroutine(Rotate(forever));

        private IEnumerator Rotate(bool forever)
        {
            float rotation = 0;
            while (rotation++ < 360 || forever)
            {
                yield return null;
                transform.Rotate(Vector3.forward, 10, Space.Self);
            }
        }
    }
}
