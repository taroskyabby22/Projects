using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SlotPile : NetworkBehaviour, Pile {

    public int cardNum = 0;
    public string color = "";

    private List PileList;

    void Start()
    {
        PileList = new List(transform);
    }

    public void InitiazeAddToPile(Transform card)
    {
        AddToPile(card);
    }
    
    public bool AddToPile(Transform card)
    {
        string cSuit = card.name.Substring(2, 1);
        int cNum = int.Parse(card.name.Substring(0, 2));
        bool placed = false;
        //if there is nothing in the done pile, an ace can be added 
        if (PileList.currentNode.currentCard == transform)
        {
            if (cSuit.Equals("S") || cSuit.Equals("C"))
                color = "B";
            else
                color = "R";
            cardNum = cNum;
            placed = true;
        }
        //if the suit matches and the card number is the next greatest, add it 
        else if ((color.Equals("R") && (cSuit.Equals("S") || cSuit.Equals("C"))) && cardNum - 1 == cNum)
        {
            color = "B";
            cardNum--;
            placed = true;
        }
        else if ((color.Equals("B") && (cSuit.Equals("H") || cSuit.Equals("D"))) && cardNum - 1 == cNum)
        {
            cardNum--;
            color = "R";
            placed = true;
        }

        if(placed)
        {
            PileList.AddToList(card);
            PileList.SetPositionLarge();

            card.GetComponent<DragTransform>().RemoveFromPrevPile();
            card.GetComponent<DragTransform>().CardPile = this;

            for (int i = 0; i < card.childCount; i++)
            {
                if(!card.GetChild(i).transform.name.Equals("back"))
                {
                    return AddToPile(card.GetChild(i).transform);
                }
            }
        }

        return false;
    }


    public void RemoveFromPile()
    {
        if (color.Equals("R"))
            color = "B";
        else
            color = "R";

        cardNum++;
        PileList.RemoveFromList();
    }
}
