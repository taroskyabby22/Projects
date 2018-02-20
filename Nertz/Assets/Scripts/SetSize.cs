using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSize : MonoBehaviour {

    public bool MatchXY;

    [Range(0, 1)]
    public float ScaleX;
    [Range(0, 1)]
    public float ScaleY;

    [Range(-100, 100)]
    public float OffsetX;
    [Range(-100, 100)]
    public float OffsetY;

    public float ZPos = 0;

    private static bool SetUp = false;

    void Start()
    {
        if(!SetUp)
        {
            Camera.main.orthographicSize = Screen.height * 5;
            SetUp = true;
        }

        float height = Screen.height;
        float width = Screen.width;

        transform.localScale = new Vector3(width * ScaleX, 1, MatchXY ? width * ScaleX : height * ScaleY);
        transform.localPosition = new Vector3(width * 5 * OffsetX / 100, ZPos, (MatchXY) ? width * 5 * OffsetY / 100 : height * 5 * OffsetY / 100);
        
    }
}
