using UnityEngine;
using UnityEngine.UI;

namespace Nsr.MultiSpaceShooter
{
    public class RawImgScroller : MonoBehaviour
    {
        [SerializeField] private Vector2 direction;
        private RawImage rawImg;
        private void Awake() => rawImg = GetComponent<RawImage>();
        private void Update() => rawImg.uvRect = new Rect(rawImg.uvRect.position + direction * Time.deltaTime, rawImg.uvRect.size);
    }
}