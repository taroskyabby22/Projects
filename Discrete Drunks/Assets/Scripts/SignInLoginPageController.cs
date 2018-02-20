using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInLoginPageController : MonoBehaviour {


    public Image JoinImage;
    public Image CreateImage;

    public Button JoinButton;
    public Button CreateButton;

    private ColorBlock Clicked;
    private ColorBlock Unclicked;

    public Text CreateUsername;
    public Text CreateGameName;
    public Text CreateError;

    public Text JoinUsername;

    public Button GameNamesPrefab;
    private RectTransform Content;

    
    public ColorBlock GameNameColorUnclick;
    public ColorBlock GameNameColorClick;
    public static ColorBlock GameColorUnclick;
    public static ColorBlock GameColorClick;

    private List<string> names = new List<string>();

    public static string CurrentGameName = "";
    private static string tempGameName = "";
    public static string CurrentUserName = "";
    public static DatabaseReference database;

    public static string PlayerSetup = "1000000000";

    void Start()
    {
        GameColorUnclick = GameNameColorUnclick;
        GameColorClick = GameNameColorClick;
        Content = GameObject.FindGameObjectWithTag("Content").GetComponent<RectTransform>();
        Clicked = JoinButton.colors;
        Unclicked = CreateButton.colors;

        Enable(CreateImage.GetComponent<RectTransform>(), false);
        CreateButton.colors = Unclicked;

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://discrete-6eb08.firebaseio.com/");
        database = FirebaseDatabase.DefaultInstance.RootReference;

        database.ChildAdded += HandleChildAdded;
        database.ChildRemoved += HandleChildRemoved;

    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if (Content == null)
            return;
        foreach (Button b in Content.GetComponentsInChildren<Button>())
        {
            if(b.GetComponentInChildren<Text>().text.Equals(args.Snapshot.Key))
            {
                Destroy(b.gameObject);
            }
        }
        names.Remove(args.Snapshot.Key);
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        database.Child(args.Snapshot.Key).Child("TimeStamp").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Button ndame = Instantiate(GameNamesPrefab, Content);
                ndame.colors = GameNameColorUnclick;
                ndame.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;
                names.Add(args.Snapshot.Key);
                return;
            }
            try
            {
                if (System.DateTime.Now.ToUniversalTime() - DateTime.Parse(task.Result.Value.ToString()) >= new TimeSpan(0, 5, 0))
                {
                    database.Child(args.Snapshot.Key).SetValueAsync(null);
                    return;
                }
            }
            catch (System.Exception) { }
            Button name = Instantiate(GameNamesPrefab, Content);
            name.colors = GameNameColorUnclick;
            name.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;
            name.onClick.AddListener(() => GameNameClickUnsetAll());
            names.Add(args.Snapshot.Key);
        });
    }
    

    public void GameNameClickUnsetAll()
    {
        foreach(GameObject c in GameObject.FindGameObjectsWithTag("Content"))
        {
            foreach (Button b in c.GetComponentsInChildren<Button>())
            {
                b.colors = GameNameColorUnclick;
            }
            GameObject clicked = EventSystem.current.currentSelectedGameObject;
            try
            {
                clicked.GetComponent<Button>().colors = GameNameColorClick;
                tempGameName = clicked.GetComponentInChildren<Text>().text;
            }
            catch (Exception) { }
        }
       
    }
    

    public void OnCreateClick()
    {
        Enable(JoinImage.GetComponent<RectTransform>(), false);
        Enable(CreateImage.GetComponent<RectTransform>(), true);
        CreateButton.colors = Clicked;
        JoinButton.colors = Unclicked;

        if (Content == null)
            Content = GameObject.FindGameObjectWithTag("Content").GetComponent<RectTransform>();
        foreach (Button b in Content.GetComponentsInChildren<Button>())
        {
            b.colors = GameNameColorUnclick;
        }
    }

    public void OnJoinClick()
    {
        Enable(JoinImage.GetComponent<RectTransform>(), true);
        Enable(CreateImage.GetComponent<RectTransform>(), false);
        CreateButton.colors = Unclicked;
        JoinButton.colors = Clicked;

        if (Content == null)
            Content = GameObject.FindGameObjectWithTag("Content").GetComponent<RectTransform>();
        foreach (Button b in Content.GetComponentsInChildren<Button>())
        {
            b.colors = GameNameColorUnclick;
        }
    }
    
    private void Enable(RectTransform c, bool value)
    {
        for(int i = 0; i< c.childCount; i++)
        {
            Enable(c.GetChild(i).GetComponent<RectTransform>(), value);
        }
        try
        {
            c.GetComponent<Image>().enabled = value;
            return;
        }
        catch (Exception) { }

        try
        {
            c.GetComponent<Text>().enabled = value;
            return;
        }
        catch (Exception) { }

        try
        {
            c.GetComponent<Button>().enabled = value;
            return;
        }
        catch (Exception) { }
    }

    public void CreateGame()
    {
        if (CreateUsername.text.Equals(""))
        {
            CreateUsername.GetComponentInParent<Image>().color = Color.yellow;
            return;
        }
        else if (CreateGameName.text.Equals(""))
        {
            CreateUsername.GetComponentInParent<Image>().color = Color.white;
            CreateGameName.GetComponentInParent<Image>().color = Color.yellow;
            return;
        }
        if(names.Contains(CreateGameName.text))
        {
            CreateGameName.GetComponentInParent<Image>().color = Color.yellow;
            return;
        }
        CurrentGameName = CreateGameName.text;
        CurrentUserName = CreateUsername.text;
        database.Child(CreateGameName.text).Child("Users").Child(CreateUsername.text).Child("Looks").SetValueAsync("1000000000");
        database.Child(CurrentGameName).Child("State").SetValueAsync(0);
        database.Child(CurrentGameName).Child("TimeStamp").SetValueAsync(DateTime.Now.ToUniversalTime().ToString());
        database.Child(CurrentGameName).Child("Users").Child(CreateUsername.text).Child("Position").SetValueAsync(0);
        database.Child(CurrentGameName).Child("MiniGame").Child(CreateUsername.text).SetValueAsync(0);
        Go();
    }

    public void JoinGame()
    {
        CurrentGameName = tempGameName;
        database.Child(CurrentGameName).Child("Users").Child(JoinUsername.text).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                CurrentGameName = tempGameName;
                if (JoinUsername.text.Equals(""))
                {
                    JoinUsername.GetComponentInParent<Image>().color = Color.yellow;
                    return;
                }
                if (CurrentGameName.Equals(""))
                {
                    return;
                }
                if (snapshot.Value == null || snapshot.Value.ToString().Equals("Null"))
                {
                    //body hat headphones eyes fro earphones
                    database.Child(CurrentGameName).Child("Users").Child(JoinUsername.text).Child("Looks").SetValueAsync("1000000000");
                    database.Child(CurrentGameName).Child("Users").Child(JoinUsername.text).Child("Position").SetValueAsync(0);
                    database.Child(CurrentGameName).Child("MiniGame").Child(JoinUsername.text).SetValueAsync(0);
                    database.Child(CurrentGameName).Child("TimeStamp").SetValueAsync(DateTime.Now.ToUniversalTime().ToString());
                    CurrentUserName = JoinUsername.text;
                    Go();
                }
                else
                {
                    JoinUsername.GetComponentInParent<Image>().color = Color.blue;
                    return;
                }
               
            }
        });
       
    }

    private void Go()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("PlayerSetup");
    }

   
    }
