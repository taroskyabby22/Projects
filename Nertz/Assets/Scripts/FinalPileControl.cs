using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPileControl : MonoBehaviour, Pile {

    public GameObject[] smallCards;

    public char suit;

    private int CurrentCardNum = 0;

    private GameObject oldC = null;

    public bool AddToPile(Transform cardT)
    {
        //get the card
        string card = cardT.name.Substring(4);

        //Check the suit
        if (suit != (card.ToCharArray())[card.Length - 1])
            return false;

        //Get the number
        int cardNum = int.Parse(card.Substring(0, 1));
        if(card.Length == 3)
            cardNum = int.Parse(card.Substring(0, 2));

        //check the number
        if (cardNum != CurrentCardNum + 1)
            return false;

        if (cardT.GetComponent<Pile>() != null)
            cardT.GetComponent<Pile>().RemoveFromPile();
        else
            Debug.Log("Error: FinalPileControl: Card has no previous pile");

        //TODO Send Signal to other players

        Destroy(cardT.gameObject);
        CurrentCardNum++;
        Debug.Log("" + CurrentCardNum);

        //TODO rest of the small
        GameObject temp = (Instantiate(smallCards[CurrentCardNum - 1], transform));
        temp.transform.localScale = new Vector3(6, 3, 1);
        temp.transform.position += new Vector3(0, 0, 0.1f);

        if (oldC != null)
            Destroy(oldC);
        oldC = temp;

        return true;
    }

    /// <summary>
    /// You cannot remove from piles that are done
    /// </summary>
    /// <returns>Null</returns>
    public Transform RemoveFromPile()
    {
        return null;
    }

}
