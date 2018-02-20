using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonePile : MonoBehaviour, Pile {

    public GameObject[] hearts;
    public GameObject[] clubs;
    public GameObject[] spades;
    public GameObject[] diamonds;

    private int cardNum = 0;
    private string suit = "";
    

    public void InitiazeAddToPile(Transform card)
    {
    }

    public bool AddToPile(Transform card)
    {
        //Cannot add a card with a child to the done stack 
        if (card.childCount != 1)
            return false;

        string cSuit = card.name.Substring(2, 1);
        int cNum = int.Parse(card.name.Substring(0, 2));
        
        //if there is nothing in the done pile, an ace can be added 
        if(suit.Equals("") && cNum == 1)
        {
            suit = cSuit;
            cardNum = cNum;
            card.GetComponent<DragTransform>().RemoveFromPrevPile();
            Destroy(card.gameObject);
            Instantiate(hearts[cNum - 1], transform.position, transform.rotation);
            return true;
        }
        //if the suit matches and the card number is the next greatest, add it 
        else if (suit.Equals(cSuit) && cardNum + 1 == cardNum)
        {
            cardNum++;
            Instantiate(hearts[cNum - 1], transform.position, transform.rotation);
            card.GetComponent<DragTransform>().RemoveFromPrevPile();
            Destroy(card.gameObject);
            return true;
        }

        return false;
    }
    

    public void RemoveFromPile()
    {

    }

}
