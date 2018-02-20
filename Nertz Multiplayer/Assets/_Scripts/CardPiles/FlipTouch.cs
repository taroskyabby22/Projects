using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTouch : MonoBehaviour {

    public Transform flip;

	void OnMouseDown()
    {
        flip.GetComponent<Pile>().RemoveFromPile();
    }
}
