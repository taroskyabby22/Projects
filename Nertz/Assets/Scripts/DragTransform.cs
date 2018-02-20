using System.Collections;
using UnityEngine;

class DragTransform : MonoBehaviour
{
    private bool dragging = false;
    private float distance;

    private Renderer rend;
    private float y = 1;

    private Transform pile;
    private Vector3 position;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    /// <summary>
    /// Set the card to this pile
    /// </summary>
    /// <param name="currentPile">Which pile the card should be set to</param>
    public void SetPile(Transform currentPile)
    {
        pile = currentPile;

    }

    /// <summary>
    /// Get the pile that the card is currently snapped to
    /// This is useful when removing a card from an old pile
    /// </summary>
    /// <returns>Pile that is currently attached to the card</returns>
    public Transform GetPile()
    {
        return pile;
    }

    void OnMouseDown()
    {
        //move up (so its what you see)
        y += 10;
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    void OnMouseUp()
    {
        //move back down on drop
        y -= 10;
        dragging = false;
        
        RaycastHit[] hits = Physics.RaycastAll(transform.position, new Vector3(0, -1, 0), 100.0F);

        bool found = false;
        //See if we hit a pile
        foreach (RaycastHit hit in hits)
        {
            Pile p = hit.collider.gameObject.GetComponent<Pile>();
            if (p != null)
            {
                //Try to add it to the pile
                if (p.AddToPile(transform))
                {
                    SetPile(hit.collider.transform);
                    position = transform.position;
                    found = true;
                }
            }
        }
        //If there was no pile to be added to snap back to the original position
        if(!found)
            transform.position = position;

    }

    void Update()
    {
        //allow the user to drag the item around
        if (dragging)
        {
            //TODO Send signal... Maybe? 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            rayPoint.y = y;
            transform.position = rayPoint;
        }
    }
}