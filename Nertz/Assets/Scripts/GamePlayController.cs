using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour {

    public GameObject[] cards;

    //TODO Set
    private int PlayerNumber = 4;
    private GameObject Player;

	void Start () {
        StartCoroutine(WaitForStart());
	}

    /// <summary>
    /// Set Up the Screen so that the game can be played
    /// Start the game
    /// </summary>
    /// <returns>Time</returns>
    IEnumerator WaitForStart()
    {
        //TODO wait for all players to be present
        yield return new WaitForSeconds(2);

        //Reset Camera size to be closer
        Camera.main.orthographicSize = Camera.main.orthographicSize * 2 / 3;

        //Rotate Camera and final piles if necessary
        GameObject finalPile = GameObject.FindGameObjectWithTag("FinalPile");
        float moveUp = 1.3f;
        float moveDown = 0.6f;
        switch(PlayerNumber)
        {
            case 1:
                Camera.main.transform.localPosition -= new Vector3(0, 0, Screen.height * moveUp);
                Player = GameObject.Find("PlayerSpace (1)");
                Player.transform.localScale = new Vector3(Screen.width * 5 / 8, 1, Screen.width * 5 / 10);
                Player.transform.position -= new Vector3(0, 0, Screen.height * moveDown);
                break;
            case 2:
                Camera.main.transform.Rotate(Vector3.forward, 180);
                Camera.main.transform.localPosition += new Vector3(0, 0, Screen.height * moveUp);
                finalPile.transform.Rotate(Vector3.up, -180);
                Player = GameObject.Find("PlayerSpace (2)");
                Player.transform.localScale = new Vector3(Screen.width * 5 / 8, 1, Screen.width * 5 / 10);
                Player.transform.position += new Vector3(0, 0, Screen.height * moveDown);
                break;
            case 3:
                Camera.main.transform.localPosition += new Vector3(Screen.height * moveUp, 0, 0);
                Camera.main.transform.Rotate(Vector3.forward, 90);
                finalPile.transform.Rotate(Vector3.up, -90);
                Player = GameObject.Find("PlayerSpace (3)");
                Player.transform.localScale = new Vector3(Screen.width * 5 / 8, 1, Screen.width * 5 / 10);
                Player.transform.position += new Vector3(Screen.height * moveDown,0,0);
                break;
            case 4:
                Camera.main.transform.Rotate(Vector3.forward, -90);
                Camera.main.transform.localPosition -= new Vector3(Screen.height * moveUp, 0, 0);
                finalPile.transform.Rotate(Vector3.up, 90);
                Player = GameObject.Find("PlayerSpace (4)");
                Player.transform.localScale = new Vector3(Screen.width * 5 / 8, 1, Screen.width * 5 / 10);
                Player.transform.position -= new Vector3(Screen.height * moveDown, 0, 0);
                break;
        }

        StartCoroutine(Deal());
    }

    IEnumerator Deal()
    {
        yield return new WaitForEndOfFrame();

        //Deal out all of the current players cards
    }
}
