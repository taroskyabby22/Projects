using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NertzPile : MonoBehaviour, Pile {

    private List PileList;
    public int amount = 0;

    void Start()
    {
        PileList = new List(transform);
    }

    public void InitiazeAddToPile(Transform card)
    {
        amount++;
        if (PileList.currentNode.lastNode != null && PileList.currentNode.lastNode.currentCard != transform)
        {
            PileList.currentNode.currentCard.Rotate(new Vector3(0, 0, 180));
            PileList.SetPositionSmall(PileList.currentNode);
        }
        PileList.AddToList(card);
        PileList.SetPositionSmall(PileList.currentNode);

        
        card.GetComponent<DragTransform>().CardPile = this;
    }

    public bool AddToPile(Transform card)
    {
        return false;
    }

    public void RemoveFromPile()
    {
        amount--;
        Transform card = PileList.RemoveFromList();
        PileList.currentNode.currentCard.Rotate(new Vector3(0, 0, 180));
    }


    public List GetPileList()
    {
        return PileList;
    }
}
