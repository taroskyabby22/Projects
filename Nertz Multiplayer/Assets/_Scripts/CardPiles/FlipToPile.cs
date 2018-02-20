using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipToPile : MonoBehaviour, Pile {

    private List PileList;

    void Start()
    {
        PileList = new List(transform);
    }

    public Transform getCurrentCard()
    {
        return PileList.currentNode.currentCard;
    }
    
    public void InitiazeAddToPile(Transform card)
    {
    }

    public bool AddToPile(Transform card)
    {
        if (!(card.GetComponent<DragTransform>().CardPile is FlipPile))
        {
            return false;
        }

        card.GetComponent<DragTransform>().CardPile = this;
        PileList.AddToList(card);

        PileList.RearrangeCards();

        return true;
    }
   

    public void RemoveFromPile()
    {
        PileList.RemoveFromList();
        PileList.RearrangeCards();

    }

    public List GetPileList()
    {
        return PileList;
    }
}
