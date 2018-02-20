using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

class DragTransform : NetworkBehaviour
{
    public int PlayerNumber;

    public Pile CardPile;

    [SyncVar (hook = "SetSnapToPosition")]
    public Vector3 SnapToPosition = Vector3.zero;

    private bool dragging = false;
    private float distance;

    private float z = -1;
    private float y;

    private Transform pile;

    public int LocalPlayerNumber;
    public bool CanDrag = false;

    public void SetSnapToPosition(Vector3 pos)
    {
        SnapToPosition = pos;
        transform.position = SnapToPosition;
    }

    public void ShrinkCard(Vector3 size)
    {
        Debug.Log("SHRINK");
        transform.localScale = size;
    }

    void Start()
    {
    }
   
    /// <summary>
    /// Move the card up so its in the front
    /// Set dragging to true
    /// </summary>
    void OnMouseDown()
    {
        if (CanDrag)
        {
            z -= 1;
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
        }
    }

    /// <summary>
    /// Move the card back to its original position
    /// Set dragging to false 
    /// </summary>
    void OnMouseUp()
    {
        if (CanDrag)
        {
            z += 1;
            dragging = false;
            CmdCheckForStacks();
        }
    }

    void Update()
    {
        //allow the user to drag the item around
        if (dragging && CanDrag)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            rayPoint.z = z;
            transform.position = rayPoint;
        }
    }

    [Command]
    public void CmdCheckForStacks()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, new Vector3(0, 0, 1), 100.0F);
        bool _AddedToPile = false;
        foreach (RaycastHit hit in hits)
        {
            GameObject _ThingHit = hit.transform.gameObject;
            Pile pile = _ThingHit.GetComponent<Pile>();
            if (pile != null)
            {
                _AddedToPile = pile.AddToPile(transform);
            }
        }

        if (!_AddedToPile)
        {
            transform.position = SnapToPosition;
        }
    }

    public void RemoveFromPrevPile()
    {
        if(CardPile != null)
        {
            CardPile.RemoveFromPile();
        }
    }
}