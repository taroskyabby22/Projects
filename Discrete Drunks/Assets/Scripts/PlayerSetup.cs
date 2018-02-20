using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetup : MonoBehaviour {
    
    public GameObject hat;
    public GameObject fro;
    public GameObject earphones;
    public GameObject eyes;


    public void SetNumber(char c, int location)
    {
        char[] arr = SignInLoginPageController.PlayerSetup.ToCharArray();
        arr[9 - location] = c;
        SignInLoginPageController.PlayerSetup = new string(arr);
    }

    public void Hat(bool value)
    {
        hat.SetActive(value);
        SetNumber(value ? '1' : '0', 7);
    }

    public void Fro(bool value)
    {
        fro.SetActive(value);
        SetNumber(value ? '1' : '0', 3);
    }

    public void Earphones(bool value)
    {
        earphones.SetActive(value);
        SetNumber(value ? '1' : '0', 5);
    }

    public void Eyes(bool value)
    {
        eyes.SetActive(value);
        SetNumber(value ? '1' : '0', 1);
    }

    float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time > 30)
        {
            SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("TimeStamp").SetValueAsync(System.DateTime.Now.ToUniversalTime().ToString());
            time = 0;
        }
    }

    public void Go()
    {
        if(SignInLoginPageController.database == null)
        {
            return;
        }
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Looks").SetValueAsync(SignInLoginPageController.PlayerSetup);

        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("State").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                if (task.Result.Value.ToString().Equals("0"))
                    SceneManager.LoadScene("PlayersList");
                else
                    SceneManager.LoadScene("Game_Rules");
            }
        });
    }

}
