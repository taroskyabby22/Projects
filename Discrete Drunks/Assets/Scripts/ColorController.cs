using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour {

    public GameObject[] items;
    public int location;
    
    public Material white;
    public Material yellow;
    public Material orange;
    public Material red;
    public Material green;
    public Material blue;
    public Material purple;
    public Material black;

    public void SetNumber(char c)
    {
        char[] arr = SignInLoginPageController.PlayerSetup.ToCharArray();
        arr[9 - location] = c;
        SignInLoginPageController.PlayerSetup = new string(arr);
    }


    public void Blue()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = blue;
        }
        SetNumber('5');
    }

    public void White()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = white;
        }
        SetNumber('0');
    }

    public void Yellow()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = yellow;
        }
        SetNumber('1');
    }
    public void Orange()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = orange;
        }
        SetNumber('2');
    }
    public void Red()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = red;
        }
        SetNumber('3');
    }
    public void Green()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = green;
        }
        SetNumber('4');
    }
    public void Purple()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = purple;
        }
        SetNumber('6');
    }
    public void Black()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<MeshRenderer>().material = black;
        }
        SetNumber('7');
    }
}
