using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawImgScroller : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    private RawImage rawImg;
    private void Awake() => rawImg = GetComponent<RawImage>();
    private void Update() => rawImg.uvRect = new Rect(rawImg.uvRect.position + direction * Time.deltaTime, rawImg.uvRect.size);
}