using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

class Dealer : NetworkBehaviour {
    
    public GameObject Flip;
    public GameObject Nertz;
    public GameObject[] SlotPiles;
    
    private Transform c;

    private int number;
    private List<Transform> cards = new List<Transform>();
    private List<Transform> cards2 = new List<Transform>();
    private GameObject CardsHolder;

    private bool delt = false;

    public void SetCanDrag()
    {
        foreach(Transform c in cards2)
        {
            c.GetComponent<DragTransform>().CanDrag = true;
        }
    }

    public void SetCardSize(Vector3 size)
    {
        CardsHolder = GameObject.FindGameObjectWithTag("Card" + gameObject.GetComponent<PlayerNumber>()._PlayerNumber);
        cards2.Clear();
        for (int i = 0; i < CardsHolder.transform.childCount; i++)
        {
            cards2.Add(CardsHolder.transform.GetChild(i).transform);
        }

        foreach (Transform c in cards2)
        {
            c.GetComponent<DragTransform>().ShrinkCard(size);
        }
    }

    void Start()
    {
        if (!isServer)
            return;
    }

    [Command]
    public void CmdDeal(Vector3 rot)
    {
        if (!isServer || delt)
            return;

        CardsHolder = GameObject.FindGameObjectWithTag("Card" + gameObject.GetComponent<PlayerNumber>()._PlayerNumber);
        cards.Clear();
        cards2.Clear();
        for (int i = 0; i < CardsHolder.transform.childCount; i++)
        {
            CardsHolder.transform.GetChild(i).transform.Rotate(rot);
            cards.Add(CardsHolder.transform.GetChild(i).transform);
            cards2.Add(CardsHolder.transform.GetChild(i).transform);
        }

        //put one on each slot
        for (int i = 0; i < SlotPiles.Length; i++)
        {
            CreateAndRotate();
            SlotPiles[i].GetComponent<Pile>().InitiazeAddToPile(c);
        }

        //put 13 in the nertz pile
        for (int i = 0; i < 13; i++)
        {
            CreateAndRotate();
            Nertz.GetComponent<Pile>().InitiazeAddToPile(c);
        }

        //Put the rest in the flip pile
        while (cards.Count != 0)
        {
            CreateAndRotate();
            Flip.GetComponent<Pile>().InitiazeAddToPile(c);
        }
        delt = true;
    }
    
    private void CreateAndRotate()
    {
        number = Random.Range(0, 52 * 20);
        number %= cards.Count;
        c = cards[number];
        cards.RemoveAt(number);
    }

}
