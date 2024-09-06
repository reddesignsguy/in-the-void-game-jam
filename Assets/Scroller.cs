using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x, _y;

    void Update()
    {
        Vector2 stretch = new Vector2(0,0);
        stretch.x = 1 * Mathf.Sin(Time.unscaledTime / 10) + 1;
        stretch.y = 1 * Mathf.Sin(Time.unscaledTime / 10) + 1;
        _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, stretch);
    }
}
