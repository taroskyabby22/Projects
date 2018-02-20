using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScale : MonoBehaviour {

    [Range(0, 100)]
    public float ScaleX;
    [Range(0, 100)]
    public float ScaleY;

    [Range(-50, 50)]
    public float OffsetX;
    [Range(-50, 50)]
    public float OffsetY;

    void Start () {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Camera.main.pixelWidth * ScaleX / 100.0f, ScaleY != 0 ? Camera.main.pixelHeight * ScaleY / 100.0f : Camera.main.pixelWidth * ScaleX / 100.0f);
        gameObject.GetComponent<RectTransform>().localPosition = new Vector2(Camera.main.pixelWidth * OffsetX / 100.0f, Camera.main.pixelHeight * OffsetY / 100.0f);
    }
}
