using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPile : MonoBehaviour, Pile {

    public FlipToPile flipToPile;

    private List PileList = null;

    void Start()
    {
        PileList = new List(transform);
    }
    

    public void InitiazeAddToPile(Transform card)
    {
        PileList.AddToList(card);
        card.transform.Rotate(new Vector3(0, 0, 180));
        PileList.SetPositionSmall();

        card.GetComponent<DragTransform>().CanDrag = false;
        card.GetComponent<DragTransform>().CardPile = this;

    }

    public bool AddToPile(Transform card)
    {
        return false;
    }

    public List GetPileList()
    {
        return PileList;
    }

    public void RemoveFromPile()
    {
        // if there is no cards, grab all the cards
        if(PileList.currentNode.currentCard == transform)
        {
            Transform c = flipToPile.getCurrentCard();
            while(c.GetComponent<DragTransform>() != null )
            {
                InitiazeAddToPile(c);
                flipToPile.RemoveFromPile();
                c = flipToPile.getCurrentCard();
            }
        }
        else
        {
            for(int i = 0; i < 3; i ++)
            {
                Transform card = PileList.RemoveFromList();
                if (card == null)
                    return;
                card.GetComponent<DragTransform>().CanDrag = true;
                card.Rotate(0, 0, 180);
                flipToPile.AddToPile(card);
            }
        }
    }
}
