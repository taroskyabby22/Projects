using UnityEngine;
using System.Collections;

public class List
{
    public Node currentNode;
    public Transform parentPile;

    #region Node
    public class Node
    {
        public Node lastNode;
        public Transform currentCard;

        public Node(Transform card)
        {
            currentCard = card;
        }
    }
    #endregion

    public List(Transform t)
    {
        parentPile = t;
        currentNode = new Node(t);
    }

    #region Add and Remove
    public void AddToList(Transform currentCard)
    {
        Node n = new Node(currentCard);
        n.lastNode = currentNode;
        currentNode = n;
    }

    public Transform RemoveFromList()
    {
        if (currentNode.currentCard == parentPile)
            return null;
        Transform r = currentNode.currentCard;
        currentNode = currentNode.lastNode;
        return r;
    }

    #endregion

    #region Actions

    public void LastCannotDrag()
    {
        if (currentNode.lastNode.currentCard != parentPile && currentNode.currentCard != parentPile)
            currentNode.lastNode.currentCard.GetComponent<DragTransform>().CanDrag = false;
    }
    public void LastCanDrag()
    {
        if (currentNode.lastNode.currentCard != parentPile && currentNode.currentCard != parentPile)
            currentNode.lastNode.currentCard.GetComponent<DragTransform>().CanDrag = true;
    }

    #endregion


    #region Position

    public void SetPosition(Node card, float scaleX)
    {
        Vector3 pos = new Vector3(card.lastNode.currentCard.position.x, card.lastNode.currentCard.position.y - card.currentCard.localScale.x * scaleX, card.lastNode.currentCard.position.z - 0.01f);
        if (card.lastNode.currentCard == parentPile)
        {
            pos.y = card.lastNode.currentCard.position.y;
        }
        card.currentCard.position = pos;
        card.currentCard.GetComponent<DragTransform>().SnapToPosition = pos;
    }

    public void SetPositionSmall()
    {
        SetPositionSmall(currentNode);
    }

    public void SetPositionSmall(Node card)
    {
        SetPosition(card, 1/40.0f);
    }

    public void SetPositionLarge()
    {
        SetPosition(currentNode, 3);
    }

    public void RearrangeCards()
    {
        Node card = currentNode;

        //card 3
        if(card.currentCard != parentPile)
        {
            card = card.lastNode;
            if(card.currentCard != parentPile)
            {
                card = card.lastNode;
                if (card.currentCard != parentPile) 
                {
                    SetPositionSmall(card);
                }
            }
        }

        card = currentNode;
        //card 2
        if (card.currentCard != parentPile)
        {
            card = card.lastNode;
            if (card.currentCard != parentPile)
            {
                SetPosition(card, 2.5f);
                card.currentCard.GetComponent<DragTransform>().CanDrag = false;
            }
        }

        card = currentNode;
        //card 1
        if (card.currentCard == parentPile)
            return;
        SetPosition(card, 2.5f);
        card.currentCard.GetComponent<DragTransform>().CanDrag = true;

    }

    #endregion

}
