using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNumber : NetworkBehaviour
{

    [SyncVar(hook = "SetPlayerNumber")]
    public int _PlayerNumber;

    private bool setup = false;

    void Start()
    {
        StartCoroutine("Shrink");
        if (!isServer)
            return;
        _PlayerNumber = GameObject.FindGameObjectsWithTag("Player").Length;
        StartCoroutine("SetUp");
    }

    public bool GetIsLocal()
    {
        return isLocalPlayer;
    }


    private void SetPlayerNumber(int value)
    {
        _PlayerNumber = value;
        StartCoroutine("SetUp");
    }

    public int getPlayerNumber()
    {
        return _PlayerNumber;
    }

    public IEnumerator Shrink()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        foreach (GameObject temp in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (temp.GetComponent<PlayerNumber>().GetIsLocal())
                continue;
            temp.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            temp.GetComponent<Dealer>().SetCardSize(new Vector3(0.6f, 0.6f, 0.6f));
        }
    }

    public IEnumerator SetUp()
    {
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitForEndOfFrame();
        if (!setup)
        {
            Vector3 rot= Vector3.zero;
            setup = true;
            if (_PlayerNumber == 1)
            {
                transform.position = new Vector3(0, -28, 0);
            }
            if (_PlayerNumber == 2)
            {
                if (isLocalPlayer)
                {
                    Camera.main.transform.Rotate(new Vector3(0, 0, 180f));
                    Camera.main.transform.position = new Vector3(0, -Camera.main.transform.position.y, Camera.main.transform.position.z);
                }
                transform.position = new Vector3(0, 28, 0);
                transform.Rotate(new Vector3(0, 0, 180f));
                rot = new Vector3(0, 0, 180f);
            }
            if (_PlayerNumber == 3)
            {
                if (isLocalPlayer)
                {
                    Camera.main.transform.Rotate(new Vector3(0, 0, -90f));
                    Camera.main.transform.position = new Vector3(Camera.main.transform.position.y, 0, Camera.main.transform.position.z);
                }
                transform.position = new Vector3(-28, 0, 0);
                transform.Rotate(new Vector3(0, 0, -90));
                rot = new Vector3(0, 0, -90);
            }
            if (_PlayerNumber == 4)
            {
                if (isLocalPlayer)
                {
                    Camera.main.transform.Rotate(new Vector3(0, 0, 90f));
                    Camera.main.transform.position = new Vector3(-Camera.main.transform.position.y, 0, Camera.main.transform.position.z);
                }
                transform.position = new Vector3(28, 0, 0);
                transform.Rotate(new Vector3(0, 0, 90));
                rot = new Vector3(0, 0, 90);
            }

            float z = rot.z;
            rot.z = rot.y;
            rot.y = z;

            yield return new WaitForEndOfFrame();
            gameObject.GetComponent<Dealer>().CmdDeal(rot);
            yield return new WaitForEndOfFrame();
            if(isLocalPlayer)
                gameObject.GetComponent<Dealer>().SetCanDrag();
        }
    }
}
