using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginScreenController : MonoBehaviour {

    public Material[] materials;

    public RectTransform PlayerListContent;
    public Image PlayerNamePrefab;

    public GameObject PlayerPrefab;

    public static Dictionary<string, GameObject> AllPlayers = new Dictionary<string, GameObject>();

    private DatabaseReference users;
    private DatabaseReference begin;

    void Start()
    {
        users = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users");
        begin = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("State");
        users.ChildAdded += HandleChildAdded;
        users.ChildRemoved += HandleChildRemoved;
        users.ChildChanged += HandleChildChanged;
        begin.ValueChanged += HandleChildChangedState;
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

    private void HandleChildChangedState(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        if(int.Parse(args.Snapshot.Value.ToString()) == 1 || int.Parse(args.Snapshot.Value.ToString()) == 2)
        {
            begin.ValueChanged -= HandleChildChangedState;
            SceneManager.LoadScene("Game_Rules");
        }
    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        foreach (Image b in PlayerListContent.GetComponentsInChildren<Image>())
        {
            if (b.GetComponentInChildren<Text>().text.Equals(args.Snapshot.Key))
            {
                Destroy(b.gameObject);
            }
        }
        Destroy(GameObject.Find(args.Snapshot.Key));
    }

    private void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Image name = Instantiate(PlayerNamePrefab, PlayerListContent);
        GameObject player = Instantiate(PlayerPrefab, new Vector3(Random.Range(-4f, 4f), 1f, Random.Range(-4f, 4f)), PlayerPrefab.transform.rotation);
        name.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;
        player.AddComponent<RandomMovementPlayer>();
        player.transform.name = args.Snapshot.Key;
        users.Child(args.Snapshot.Key).Child("Looks").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string info = snapshot.Value.ToString();
                char[] arr = info.ToCharArray();

                player.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[1].ToString())];
                for(int i = 0; i < player.transform.childCount; i++)
                {
                    Transform child = player.transform.GetChild(i);
                    GameObject c = child.gameObject;
                    if (int.Parse(arr[2].ToString()) == 1 && c.tag.Equals("Hat"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[3].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[3].ToString())];
                        }
                    }
                    if (int.Parse(arr[4].ToString()) == 1 && c.tag.Equals("Ears"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[5].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[5].ToString())];
                        }
                    }
                    if (int.Parse(arr[6].ToString()) == 1 && c.tag.Equals("Fro"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[7].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[7].ToString())];
                        }
                    }
                    if (int.Parse(arr[8].ToString()) == 1 && c.tag.Equals("Eye"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[9].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[9].ToString())];
                        }
                    }
                }
                
            }
        });
    }

    private void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Image name = Instantiate(PlayerNamePrefab, PlayerListContent);
        GameObject player = GameObject.Find(args.Snapshot.Key);
        users.Child(args.Snapshot.Key).Child("Looks").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string info = snapshot.Value.ToString();
                char[] arr = info.ToCharArray();

                player.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[1].ToString())];
                for (int i = 0; i < player.transform.childCount; i++)
                {
                    Transform child = player.transform.GetChild(i);
                    GameObject c = child.gameObject;
                    if (int.Parse(arr[2].ToString()) == 1 && c.tag.Equals("Hat"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[3].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[3].ToString())];
                        }
                    }
                    if (int.Parse(arr[4].ToString()) == 1 && c.tag.Equals("Ears"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[5].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[5].ToString())];
                        }
                    }
                    if (int.Parse(arr[6].ToString()) == 1 && c.tag.Equals("Fro"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[7].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[7].ToString())];
                        }
                    }
                    if (int.Parse(arr[8].ToString()) == 1 && c.tag.Equals("Eye"))
                    {
                        c.SetActive(true);
                        c.GetComponent<MeshRenderer>().material = materials[int.Parse(arr[9].ToString())];
                        for (int j = 0; j < child.childCount; j++)
                        {
                            child.GetChild(j).GetComponent<MeshRenderer>().material = materials[int.Parse(arr[9].ToString())];
                        }
                    }
                }

            }
        });
    }

    public void Go()
    {
        begin.ValueChanged -= HandleChildChangedState;
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("State").SetValueAsync(1);
        SceneManager.LoadScene("Game_Rules");
    }
}
