using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayController : MonoBehaviour {

    private bool CanMouseScroll = false;

    public Text RideTheBusText;
    public Text RideTheBusEnterText;
    private bool RideTheBusReady = true;
    public RectTransform ridePlayerContent;

    public Image WINNER_SHOW;
    public Text WINNER_NAME;

    public Text debug;

    public Vector3 CAMERA_OFFSET;

    private int numGames = 5;
    private bool completed = false;

    public Button PlayerNamesPrefab;

    public string winnerChug="";

    public RectTransform chugPlayerContent;
    public RectTransform flipPlayerContent;
    public RectTransform pencilPlayerContent;
    public RectTransform stackPlayerContent;

    public Image chugmini;
    public Image flipmini;
    public Image pencilmini;
    public Image ridemini;
    public Image stackmini;

    public Image showStandings;
    public Image PlayerNamePrefab;

    public Image Winner;

    public RectTransform standingsPlayerContent;

    public Image showDrinkDone;
    public Image showSick;
    public Image showSpill;

    public GameObject PlayerPrefab;
    public Material[] materials;

    public int PathSize;
    public GameObject blockStraight;
    public GameObject blockCorner;

   
    private List<Vector3> takenPos = new List<Vector3>();
    private DatabaseReference users;
    private DatabaseReference minigame;
    private DatabaseReference game;
    private BlockList blocklist;
    private bool startedNewGame = false;
    private int OldGame = 0;

    public void Exit()
    {
        showStandings.gameObject.SetActive(false);
        showDrinkDone.gameObject.SetActive(false);
        showSick.gameObject.SetActive(false);
        showSpill.gameObject.SetActive(false);
    }

    public void GetSick()
    {
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsFaulted)
            {

            }
            else
            {
                int current = int.Parse(task2.Result.Value.ToString());
                SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").SetValueAsync((current - 10 < 0 ? 0 : current - 10));
                showSick.gameObject.SetActive(false);
            }
        });
    }

    public void SpillDrink()
    {
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsFaulted)
            {

            }
            else
            {
                int current = int.Parse(task2.Result.Value.ToString());
                SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").SetValueAsync(((current - 5) < 0 ? 0 : current - 5));
                showSpill.gameObject.SetActive(false);
            }
        });
    }

    public void FinishDrink()
    {
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsFaulted)
            {

            }
            else
            {
                int current = int.Parse(task2.Result.Value.ToString());
                SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").SetValueAsync(current + 5);
                showDrinkDone.gameObject.SetActive(false);
            }
        });
    }

    public void SickClick()
    {
        showSick.gameObject.SetActive(true);
    }

    public void SpillClick()
    {
        showSpill.gameObject.SetActive(true);
    }

    public void DrinkDoneClick()
    {
        showDrinkDone.gameObject.SetActive(true);
    }

    public void StandingsClick()
    {
        showStandings.gameObject.SetActive(true);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Finish"))
        {
            Destroy(g);
        }
        users.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            foreach (DataSnapshot d in task.Result.Children)
            {
                users.Child(d.Key).Child("Position").GetValueAsync().ContinueWith(task2 =>
                {
                    if(task2.IsFaulted)
                    {
                        return;
                    }
                    {
                        Image name = Instantiate(PlayerNamePrefab, standingsPlayerContent);
                        name.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = d.Key + " -> " + task2.Result.Value;
                    }
                });
            }
        });
    }

    public class Block
    {
        public GameObject currentGameobject;
        public Block nextBlock;
        public Block lastBlock;

        public Block(GameObject block)
        {
            currentGameobject = block;
        }
    }

    public class BlockList
    {

        public Block StarterBlock;
        public Block CurrentBlock;
        public GameObject blockCorner;
        public GameObject blockStraight;
        public Dictionary<string, Block> playerpositions = new Dictionary<string, Block>();

        public BlockList(GameObject starter, GameObject corner, GameObject straight)
        {
            StarterBlock = new Block(starter);
            StarterBlock.nextBlock = null;
            CurrentBlock = StarterBlock;
            blockCorner = corner;
            blockStraight = straight;
        }

        public void AddBlock(GameObject block)
        {
            Block b = new Block(block);
            CurrentBlock.nextBlock = b;
            GameObject oldb = CurrentBlock.currentGameobject;
            if (CurrentBlock.lastBlock != null && !((oldb.transform.position.z == CurrentBlock.lastBlock.currentGameobject.transform.position.z && oldb.transform.position.z == CurrentBlock.nextBlock.currentGameobject.transform.position.z) ||
                (oldb.transform.position.x == CurrentBlock.lastBlock.currentGameobject.transform.position.x && oldb.transform.position.x == CurrentBlock.nextBlock.currentGameobject.transform.position.x)))
            {
                CurrentBlock.currentGameobject = Instantiate(blockCorner, oldb.transform.position, oldb.transform.rotation);
                CurrentBlock.currentGameobject.name = oldb.name;
                Destroy(oldb);

                if(CurrentBlock.currentGameobject.transform.position.z > CurrentBlock.lastBlock.currentGameobject.transform.position.z
                    && CurrentBlock.currentGameobject.transform.position.x > CurrentBlock.nextBlock.currentGameobject.transform.position.x)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.z > CurrentBlock.lastBlock.currentGameobject.transform.position.z
                   && CurrentBlock.currentGameobject.transform.position.x < CurrentBlock.nextBlock.currentGameobject.transform.position.x)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.z < CurrentBlock.lastBlock.currentGameobject.transform.position.z
                    && CurrentBlock.currentGameobject.transform.position.x > CurrentBlock.nextBlock.currentGameobject.transform.position.x)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.z < CurrentBlock.lastBlock.currentGameobject.transform.position.z
                    && CurrentBlock.currentGameobject.transform.position.x < CurrentBlock.nextBlock.currentGameobject.transform.position.x)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                }


                if (CurrentBlock.currentGameobject.transform.position.x < CurrentBlock.lastBlock.currentGameobject.transform.position.x
                    && CurrentBlock.currentGameobject.transform.position.z > CurrentBlock.nextBlock.currentGameobject.transform.position.z)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.x > CurrentBlock.lastBlock.currentGameobject.transform.position.x
                    && CurrentBlock.currentGameobject.transform.position.z < CurrentBlock.nextBlock.currentGameobject.transform.position.z)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.x > CurrentBlock.lastBlock.currentGameobject.transform.position.x
                    && CurrentBlock.currentGameobject.transform.position.z > CurrentBlock.nextBlock.currentGameobject.transform.position.z)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                }
                if (CurrentBlock.currentGameobject.transform.position.x < CurrentBlock.lastBlock.currentGameobject.transform.position.x
                    && CurrentBlock.currentGameobject.transform.position.z < CurrentBlock.nextBlock.currentGameobject.transform.position.z)
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                }
            }
            else
            {
                CurrentBlock.currentGameobject = Instantiate(blockStraight, oldb.transform.position, oldb.transform.rotation);
                CurrentBlock.currentGameobject.name = oldb.name;
                Destroy(oldb);
                if (CurrentBlock.lastBlock != null && (CurrentBlock.lastBlock.currentGameobject.transform.position.x != CurrentBlock.nextBlock.currentGameobject.transform.position.x || CurrentBlock.lastBlock.currentGameobject.transform.position.x != CurrentBlock.nextBlock.currentGameobject.transform.position.x))
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
                }
                else
                {
                    CurrentBlock.currentGameobject.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
                }
            }
            b.lastBlock = CurrentBlock;
            CurrentBlock = b;
        }

        public GameObject RemoveBlock()
        {
            GameObject b = CurrentBlock.currentGameobject;
            CurrentBlock = CurrentBlock.lastBlock;
            CurrentBlock.nextBlock = null;
            return b;
        }

        public Vector3 LookTowardNext(Block block)
        {
            if (block.nextBlock == null)
                return Vector3.one;
            Vector3 t = block.nextBlock.nextBlock.currentGameobject.transform.position;
            t.y = 1;
            t.x *= -1;
            return t;
        }

        
    }

    IEnumerator Move(int newBlock, string name)
    {
        Block Current;
        GameObject player = GameObject.Find(name);

        blocklist.playerpositions.TryGetValue(name, out Current);
        if (Current == null)
        {
            Current = blocklist.StarterBlock;
        }
        int oldBlock = int.Parse(Current.currentGameobject.name);

        while (!CanMouseScroll)
        { 
            blocklist.playerpositions.TryGetValue(name, out Current);
            if(Current == null)
            {
               Current = blocklist.StarterBlock;
            }
            oldBlock = int.Parse(Current.currentGameobject.name);
            yield return new WaitForEndOfFrame();
        }
        CanMouseScroll = false;
        Camera.main.transform.position = CAMERA_OFFSET + player.transform.position;
        Camera.main.transform.parent = player.transform;
        while (newBlock > oldBlock)
        {
            if (Current.nextBlock == null)
                break;

            Vector3 move = (Current.nextBlock.currentGameobject.transform.position);
            move.y = player.transform.position.y;
            while (player.transform.position != move)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, move, .1f);
                yield return new WaitForEndOfFrame();
            }
            //player.transform.LookAt(blocklist.LookTowardNext(Current));

            oldBlock += 1;
            Current = Current.nextBlock;
            blocklist.playerpositions[name] = Current;
        }
        while (newBlock < oldBlock)
        {
            if (Current == null)
                break;
            if (Current.lastBlock == null)
                break;
            Vector3 move = (Current.lastBlock.currentGameobject.transform.position);
            blocklist.playerpositions[name] = Current;
            move.y = player.transform.position.y;
            while (player.transform.position != move)
            {
                player.transform.position = Vector3.MoveTowards(player.transform.position, move, 0.1f);
                yield return new WaitForEndOfFrame();
            }

            //player.transform.LookAt(blocklist.LookTowardNext(Current));
            oldBlock -= 1;
            Current = Current.lastBlock;
            blocklist.playerpositions[name] = Current;
        }
        Camera.main.transform.parent = null;
        CanMouseScroll = true;
        if(Current.nextBlock == null)
        {
            WINNER_SHOW.gameObject.SetActive(true);
            WINNER_NAME.text = player.name + "\nhas won!";
        }
    }

    float speed = 1f;
    float time = 0;
    void Update()
    {
        time += Time.deltaTime;
        if (time > 30)
        {
            SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("TimeStamp").SetValueAsync(System.DateTime.Now.ToUniversalTime().ToString());
            time = 0;
        }


        if (CanMouseScroll)
        {
            if (Input.touchCount > 0)// && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                debug.text = "MOUSE DOWN";
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                Camera.main.transform.Translate(-touchDeltaPosition.x * speed * Time.deltaTime, -touchDeltaPosition.y * speed * Time.deltaTime, 0);
            }
        }
    }

    void Start()
    {
        if (SignInLoginPageController.CurrentGameName.Equals(""))
        {
            SignInLoginPageController.CurrentGameName = "lit";
            SignInLoginPageController.CurrentUserName = "abbyt";
        }
        if(SignInLoginPageController.database == null)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://discrete-6eb08.firebaseio.com/");
            SignInLoginPageController.database = FirebaseDatabase.DefaultInstance.RootReference;
        }
        users = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users");
        minigame = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("MiniGame");
        game = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Game");
        users.ChildAdded += HandleChildAdded;
        users.ChildRemoved += HandleChildRemoved;
        minigame.ChildChanged += HandleChildChanged;

        game.ValueChanged += HandleChildChangedGame;
        Destroy(Camera.main.gameObject.GetComponent<Animator>(), 1.2f);

        MakeGamePath();
        StartCoroutine(RemoveCameraAnimator());
    }

    public void GameNameClickUnsetAllChug()
    {
        foreach (Button b in chugPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        GameObject clicked = EventSystem.current.currentSelectedGameObject;
        try
        {
            clicked.GetComponent<Button>().colors = SignInLoginPageController.GameColorClick;
            winnerChug = clicked.GetComponentInChildren<Text>().text;
        }
        catch (System.Exception) { }
    }

    public void GameNameClickUnsetAllPencil()
    {
        foreach (Button b in pencilPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        GameObject clicked = EventSystem.current.currentSelectedGameObject;
        try
        {
            clicked.GetComponent<Button>().colors = SignInLoginPageController.GameColorClick;
            winnerChug = clicked.GetComponentInChildren<Text>().text;
        }
        catch (System.Exception) { }
    }

    public void GameNameClickUnsetAllStack()
    {
        foreach (Button b in stackPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        GameObject clicked = EventSystem.current.currentSelectedGameObject;
        try
        {
            clicked.GetComponent<Button>().colors = SignInLoginPageController.GameColorClick;
            winnerChug = clicked.GetComponentInChildren<Text>().text;
        }
        catch (System.Exception) { }
    }

    public void GameNameClickUnsetAllFlip()
    {
        foreach (Button b in flipPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        GameObject clicked = EventSystem.current.currentSelectedGameObject;
        try
        {
            clicked.GetComponent<Button>().colors = SignInLoginPageController.GameColorClick;
            winnerChug = clicked.GetComponentInChildren<Text>().text;
        }
        catch (System.Exception) { }
    }

    private void HandleChildChangedGame(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        foreach (Button b in chugPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        foreach (Button b in flipPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        foreach (Button b in pencilPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        foreach (Button b in stackPlayerContent.GetComponentsInChildren<Button>())
        {
            b.colors = SignInLoginPageController.GameColorUnclick;
        }
        startedNewGame = true;
        OldGame = int.Parse(args.Snapshot.Value.ToString());
        switch (int.Parse(args.Snapshot.Value.ToString()))
        {
            case 0:
                winnerChug = "";
                chugmini.gameObject.SetActive(true);
                break;
            case 1:
                winnerChug = "";
                flipmini.gameObject.SetActive(true);
                break;
            case 2:
                winnerChug = "";
                pencilmini.gameObject.SetActive(true);
                break;
            case 3:
                winnerChug = "";
                stackmini.gameObject.SetActive(true);
                break;
            case 4:
                ridemini.gameObject.SetActive(true);
                RideTheBus();
                break;
        }
        
    }

    public void Chug()
    {
        chugmini.gameObject.SetActive(true);
    }
    
    IEnumerator RemoveCameraAnimator()
    {
        yield return new WaitForSeconds(1.2f);
        while(Camera.main.orthographicSize > 5)
        {
            Camera.main.orthographicSize -= 0.5f;
            yield return new WaitForSeconds(0.01f);
        }
        while (GameObject.Find(SignInLoginPageController.CurrentUserName) == null)
        {
            yield return new WaitForEndOfFrame();
        }
        
        blocklist.CurrentBlock = blocklist.StarterBlock;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            g.transform.LookAt(blocklist.LookTowardNext(blocklist.CurrentBlock));
            blocklist.playerpositions.Add(g.name, blocklist.CurrentBlock);
            DatabaseReference position = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(g.name).Child("Position");
            position.ValueChanged += HandleValueChangedPosition;
        }
        completed = true;
        Camera.main.transform.parent = GameObject.Find(SignInLoginPageController.CurrentUserName).transform;
        CAMERA_OFFSET = Camera.main.transform.position - GameObject.Find(SignInLoginPageController.CurrentUserName).transform.position;
        Camera.main.transform.parent = null;
        CanMouseScroll = true;
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("State").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return;
            }
            if (int.Parse(task.Result.Value.ToString()) == 1)
            {
                Chug();
            }
        });
    }

    private void MakeGamePath()
    {
        Vector3 currentPos = Vector3.zero;
        currentPos.y = -0.25f;
        GameObject t = Instantiate(blockStraight, currentPos, blockStraight.transform.rotation);
        takenPos.Add(currentPos);
        t.name = "0";
        blocklist = new BlockList(t, blockCorner, blockStraight);
        

        int[] shit = {2,2,2,2,2,2,3,3,3,3,3,0,0,0,0,0,3,3,3,3,3,2,2,2,2,2,2,2,
            2,1,1,1,1,1,1,1,1,1,1,2,3,3,3,3,3,3,2,2,2,2,2,1,1,1,1,1,1,2,2,
            2,3,3,3,3,3,3,2,1,1,1,1,1,1,2,2,2,
            2,3,3,3,3,3,3,3,0,3,2,3,3,2,1,1,2,1,0,1,1,1,1,1,1,1};

        for(int i = 1; i < shit.Length; i++)
        {
            Vector3 temp;
            List<int> used = new List<int>();
            temp = currentPos;
            float space = 3.05f;

            switch (shit[i])
            {
                case 0:
                    temp.x += space;
                    used.Add(0);
                    break;
                case 1:
                    temp.z += space;
                    used.Add(1);
                    break;
                case 2:
                    temp.x -= space;
                    used.Add(2);
                    break;
                case 3:
                    temp.z -= space;
                    used.Add(3);
                    break;
            }
            
            currentPos = temp;

            t = Instantiate(blockStraight, currentPos, blockStraight.transform.rotation);
            t.name = i + "";
            takenPos.Add(currentPos);
            blocklist.AddBlock(t);
        }
    }

    private void HandleValueChangedPosition(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        //Get the players name
        string name = args.Snapshot.Reference.ToString();
        name = name.Substring(name.IndexOf("Users/") + "Users/".Length);
        name = name.Substring(0, name.Length -"Positions".Length);

        debug.text = name;

        //Get the players current position
        int newBlock = int.Parse(args.Snapshot.Value.ToString());
        StartCoroutine(Move(newBlock, name));
        
    }

    private void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        Dictionary<string, int> scores = new Dictionary<string, int>();
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("MiniGame").GetValueAsync().ContinueWith(task =>
        {
            int total = 0;
            if (task.IsFaulted)
            {

            }
            else
            {
                foreach (DataSnapshot t in task.Result.Children)
                {
                    scores.Add(t.Key.ToString(), int.Parse(t.Value.ToString()));
                    total += int.Parse(t.Value.ToString());
                }

                if (total == GameObject.FindGameObjectsWithTag("Player").Length)
                {
                    startedNewGame = false;
                    debug.text = "Good total";
                    //someone won
                    string winner = "";
                    int high = 0;
                    foreach (string c in scores.Keys)
                    {
                        int temp = 0;
                        scores.TryGetValue(c, out temp);
                        if (temp > high)
                        {
                            high = temp;
                            winner = c;
                        }
                    }
                    if (winner == SignInLoginPageController.CurrentUserName)
                    {
                        foreach (string c in scores.Keys)
                        {
                            SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("MiniGame").Child(c).SetValueAsync(0);
                        }
                        //I WON!!!
                        debug.text = "WINNER";
                        int move1 = Random.Range(1, 6);
                        int move2 = Random.Range(1, 6);
                        int movetotal = move1 + move2;
                        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(winner).Child("Position").GetValueAsync().ContinueWith(task2 =>
                        {
                            if (task2.IsFaulted)
                            {

                            }
                            else
                            {
                                int current = int.Parse(task2.Result.Value.ToString());
                                SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(winner).Child("Position").SetValueAsync(current + movetotal);
                            }
                        });
                        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Game").SetValueAsync(-1);
                        StartCoroutine(WaitThenSetGame());

                    }
                }
            }

        });
    }
    
    IEnumerator WaitThenSetGame()
    {
        while (!CanMouseScroll)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        while (!CanMouseScroll)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3);
        int t = (Random.Range(0, numGames * 5) % numGames);
        while (t == OldGame)
            t = (Random.Range(0, numGames * 5) % numGames);
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Game").SetValueAsync(t);

    }

    private void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
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
        debug.text = args.Snapshot.Key;
        GameObject old = GameObject.Find(args.Snapshot.Key.ToString());
        if (old != null)
        {
            Destroy(old);
        }
        debug.text = args.Snapshot.Key + " AFTER OLD";


        Button chugname = Instantiate(PlayerNamesPrefab, chugPlayerContent);
        chugname.onClick.AddListener(() => UserClick(args.Snapshot.Key));
        chugname.onClick.AddListener(() => GameNameClickUnsetAllChug());
        chugname.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;

        Button flipname = Instantiate(PlayerNamesPrefab, flipPlayerContent);
        flipname.onClick.AddListener(() => GameNameClickUnsetAllFlip());
        flipname.onClick.AddListener(() => UserClick(args.Snapshot.Key));
        flipname.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;

        Button pencilname = Instantiate(PlayerNamesPrefab, pencilPlayerContent);
        pencilname.onClick.AddListener(() => UserClick(args.Snapshot.Key));
        pencilname.onClick.AddListener(() => GameNameClickUnsetAllPencil());
        pencilname.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;

        Button stackname = Instantiate(PlayerNamesPrefab, stackPlayerContent);
        stackname.onClick.AddListener(() => UserClick(args.Snapshot.Key));
        stackname.onClick.AddListener(() => GameNameClickUnsetAllStack());
        stackname.GetComponent<RectTransform>().GetChild(0).GetComponent<Text>().text = args.Snapshot.Key;

        GameObject player = Instantiate(PlayerPrefab, new Vector3(Random.Range(-1f, 1f), 1f, 0), PlayerPrefab.transform.rotation);
        player.transform.name = args.Snapshot.Key;
        users.Child(args.Snapshot.Key).Child("Looks").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("FAULT");
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

        if(completed)
        {
            //need the new one to move
            blocklist.CurrentBlock = blocklist.StarterBlock;
            player.transform.LookAt(blocklist.LookTowardNext(blocklist.CurrentBlock));
            blocklist.playerpositions.Add(player.name, blocklist.CurrentBlock);
            DatabaseReference position = SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(player.name).Child("Position");
            position.ValueChanged += HandleValueChangedPosition;
        }

    }

    private void UserClick(string user)
    {
        winnerChug = user;
    }

    public void GoChug()
    {
        if (winnerChug.Equals(""))
            return;
        int t = 0;
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("MiniGame").Child(winnerChug).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {

            }
            else
            {
                DataSnapshot snapshot = task.Result;
                try {
                    t = int.Parse(snapshot.Value.ToString());
                }
                catch (System.Exception) { }
            }
            SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("MiniGame").Child(winnerChug).SetValueAsync(t + 1);
            chugmini.gameObject.SetActive(false);
            flipmini.gameObject.SetActive(false);
            pencilmini.gameObject.SetActive(false);
            stackmini.gameObject.SetActive(false);
            ridemini.gameObject.SetActive(false);
        });
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("State").SetValueAsync(2);
    }


    /*

        Ride the bus    

    */

    public class Card
    {
        private int value;
        private string suit;
        private int color;
        public int id;
        private bool flipped;


        public string ToString()
        {
            string val;
            switch (value)
            {
                case (11):
                    val = "Jack";
                    break;
                case (12):
                    val = "Queen";
                    break;
                case (13):
                    val = "King";
                    break;
                case (0):
                    val = "Ace";
                    break;
                default:
                    val = value + "";
                    break;
            }
            return (val + " of " + suit);
        }
        public void setValue(int val)
        {
            value = val;
        }
        public void setSuit(string s)
        {
            suit = s;
        }
        public int getValue()
        {
            return value;
        }
        public string getSuit()
        {
            return suit;
        }

        public int getColor()
        {
            return color;
        }

        public void setColor(int color)
        {
            this.color = color;
        }

        public void setFlipped(bool flipped)
        {
            this.flipped = flipped;
        }
    }

    public class Players
    {
        private int id;
        public Players(int i)
        {
            id = i;
        }
        private Card card1 = new Card();
        private Card card2 = new Card();
        private Card card3 = new Card();
        private Card card4 = new Card();
        
        public string ToString()
        {
            return (card1 + "\n" + card2 + "\n" + card3 + "\n" + card4);
        }
        public void setCard1(Card card)
        {
            card1 = card;
        }
        public void setCard2(Card card)
        {
            card2 = card;
        }
        public void setCard3(Card card)
        {
            card3 = card;
        }
        public void setCard4(Card card)
        {
            card4 = card;
        }
    }

    private static Card[] deck = new Card[52];
    private static Players[] player = new Players[100];
    private static Card[] pyramid = new Card[15];
    private int MoveTotal;

    public void RideTheBus()
    {
        startedNewGame = false;
        int numOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        initCards();
        initPlayers(numOfPlayers);
        StartCoroutine(stageOne(numOfPlayers, false));
    }

    bool isCorrectTwo;
    private IEnumerator stageTwo(int numOfPlayers, bool rideTheBus)
    {
        isCorrectTwo = true;
        //for (int i = 1; i <= numOfPlayers; i++)
        //{
        /*    GameObject playerC = (GameObject)GameObject.FindGameObjectsWithTag("Player").GetValue(i - 1);
            if (rideTheBus)
            {
                i = numOfPlayers;
            }*/
            System.Random rand = new System.Random();
            int card1 = rand.Next(51);
            int card2 = rand.Next(51);
            Text t = Instantiate(RideTheBusText, ridePlayerContent);
            t.text += SignInLoginPageController.CurrentUserName + ":";

            Text s = Instantiate(RideTheBusText, ridePlayerContent);
            s.text += deck[card1].ToString();
            string userinput = "x";
            while (userinput.Equals("x"))
            {

                Text w = Instantiate(RideTheBusText, ridePlayerContent);
                w.text +=  "Guess higher or lower (h/l): ";

                RideTheBusReady = false;
                while(!RideTheBusReady)
                {
                    yield return new WaitForEndOfFrame();
                }

                userinput = RideTheBusEnterText.text;
                Debug.Log(userinput);
                Debug.Log(deck[card1].getValue().ToString() + " and " + deck[card2].getValue());

                if (deck[card1].getValue() < deck[card2].getValue() && userinput.Equals("h"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Correct! The card was " + deck[card2].ToString());
                Instantiate(RideTheBusText, ridePlayerContent).text = "Move 3 Spaces!";
                MoveTotal += 3;
                    isCorrectTwo = true;
                }
                else if (deck[card1].getValue() < deck[card2].getValue() && userinput.Equals("l"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Drink! The card was " + deck[card2].ToString());
                    isCorrectTwo = false;
                }
                else if (deck[card1].getValue() > deck[card2].getValue() && userinput.Equals("h"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Drink! The card was " + deck[card2].ToString());
                    isCorrectTwo = false;
                }
                else if (deck[card1].getValue() > deck[card2].getValue() && userinput.Equals("l"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Correct! The card was " + deck[card2].ToString());
                Instantiate(RideTheBusText, ridePlayerContent).text = "Move 3 Spaces!";
                MoveTotal += 4;
                isCorrectTwo = true;
                }
                else if (deck[card1].getValue() == deck[card2].getValue() && userinput.Equals("l"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Correct! The card was " + deck[card2].ToString());
                Instantiate(RideTheBusText, ridePlayerContent).text = "Move 3 Spaces!";
                MoveTotal += 4;
                isCorrectTwo = true;
                }
                else if (deck[card1].getValue() == deck[card2].getValue() && userinput.Equals("h"))
                {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Correct! The card was " + deck[card2].ToString());
                Instantiate(RideTheBusText, ridePlayerContent).text = "Move 3 Spaces!";
                MoveTotal += 4;
                isCorrectTwo = true;
                }
                else {
                    Text ws = Instantiate(RideTheBusText, ridePlayerContent);
                    ws.text += ("Invalid Guess");
                    userinput = "x";
                }
            }
            player[0].setCard2(deck[card2]);

            Text sts = Instantiate(RideTheBusText, ridePlayerContent);
            sts.text += ("Press Enter To Continue");
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            //CLEAR SCREEN
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("RideTheBusT"))
            {
                Destroy(g);
            }
       // }

        StartCoroutine(stageThree(numOfPlayers, false));
    }

    public void RideTheBusEnter()
    {
        RideTheBusReady = true;
    }

     private static void initCards()
       {
           for (int i = 0; i < 52; i++)
           {
               deck[i] = new Card();
               deck[i].setFlipped(true);
           }
           int j = 0;
           for (int x = 0; x < 4; x++)
           {
               for (int i = 1; i < 14; i++)
               {
                   deck[j].setValue(i);
                   switch (x)
                   {
                       case (0):
                           deck[j].setSuit("Diamonds");
                           deck[j].setColor(1);
                           break;
                       case (1):
                           deck[j].setSuit("Hearts");
                           deck[j].setColor(1);
                           break;
                       case (2):
                           deck[j].setSuit("Clubs");
                           deck[j].setColor(0);
                           break;
                       case (3):
                           deck[j].setSuit("Spades");
                           deck[j].setColor(0);
                           break;
                   }
                   j++;
               }
           }
           for (int i = 0; i < 15; i++)
           {
               bool pickAgain = true;
               System.Random rand = new System.Random();
               int card = rand.Next(51);
               pyramid[i] = new Card();
               while (pickAgain)
               {
                   card = rand.Next(51);
                   pickAgain = false;
                   for (int x = 0; x < 15; x++)
                   {
                       if (pyramid[x] == deck[card])
                       {
                           pickAgain = true;
                       }
                   }
               }
               pyramid[i] = deck[card];
           }
       }

    private static void initPlayers(int numOfPlayers)
    {
        //for (int i = 1; i <= numOfPlayers; i++)
       // {
            player[0] = new Players(0);
        //}
    }

    bool isCorrectOne = true;
    private IEnumerator stageOne(int numOfPlayers, bool rideTheBus)
    {
        isCorrectOne = true;
        //for (int i = 1; i <= numOfPlayers; i++)
        //{
        /*
            if (rideTheBus)
            {
                i = numOfPlayers;
            }*/

            //GameObject playerC = (GameObject)GameObject.FindGameObjectsWithTag("Player").GetValue(i - 1);
            System.Random rand = new System.Random();
            int card = rand.Next(51);
            int guess = 3;
            while (guess == 3)
            {
                Instantiate(RideTheBusText, ridePlayerContent).text = SignInLoginPageController.CurrentUserName + ":";
                Instantiate(RideTheBusText, ridePlayerContent).text = "Guess red or black (r/b):";
                RideTheBusReady = false;
                while (!RideTheBusReady)
                {
                    yield return new WaitForEndOfFrame();
                }
                string rOrB = RideTheBusEnterText.text;
                switch (rOrB.ToLower())
                {
                    case ("r"):
                        guess = 1;
                        break;
                    case ("b"):
                        guess = 0;
                        break;
                    default:
                        Text st = Instantiate(RideTheBusText, ridePlayerContent);
                        st.text += "Invalid guess";
                        break;
                }
                if (guess == deck[card].getColor())
                {
                    Text st = Instantiate(RideTheBusText, ridePlayerContent);
                    st.text += "You guessed correctly";
                Instantiate(RideTheBusText, ridePlayerContent).text = "Move 2 Spaces!";

                MoveTotal += 2;
                isCorrectOne = true;
                }
                else
                {
                    Text st = Instantiate(RideTheBusText, ridePlayerContent);
                    st.text += ("DRINK!");
                    isCorrectOne = false;
                }
            }
            player[0].setCard1(deck[card]);

            Text sts = Instantiate(RideTheBusText, ridePlayerContent);
            sts.text += ("Press Enter To Continue");
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            //CLEAR SCREEN
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("RideTheBusT"))
            {
                Destroy(g);
            }

       // }

        StartCoroutine(stageTwo(numOfPlayers, false));

    }

    bool isCorrectThree;
    private IEnumerator stageThree(int numOfPlayers, bool rideTheBus)
    {
        isCorrectThree = true;
        //for (int i = 1; i <= numOfPlayers; i++)
        //{
            /*GameObject playerC = (GameObject)GameObject.FindGameObjectsWithTag("Player").GetValue(i - 1);
            if (rideTheBus)
            {
                i = numOfPlayers;
            }*/
            System.Random rand = new System.Random();
            int card1 = rand.Next(51);
            int card2 = rand.Next(51);
            int card3 = rand.Next(51);
            bool inside;
            Text t = Instantiate(RideTheBusText, ridePlayerContent);
            t.text += SignInLoginPageController.CurrentUserName + ":";
            Text ts = Instantiate(RideTheBusText, ridePlayerContent);
            ts.text += deck[card1].ToString();
            Text td = Instantiate(RideTheBusText, ridePlayerContent);
            td.text += deck[card2].ToString();
            Text ta = Instantiate(RideTheBusText, ridePlayerContent);
            ta.text +="Will the next card be inside or outside? (i/o)";
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            string userinput = RideTheBusEnterText.text;
            if ((deck[card1].getValue() < deck[card3].getValue() && deck[card3].getValue() < deck[card2].getValue()) ||
                (deck[card1].getValue() > deck[card3].getValue() && deck[card3].getValue() > deck[card2].getValue()))
            {
                inside = true;
            }
            else {
                inside = false;
            }
            //TODO no check on input
            if (inside && userinput.Equals("i"))
            {
                Text tw = Instantiate(RideTheBusText, ridePlayerContent);
                tw.text += ("Correct! The card was " + deck[card3].ToString());
            Instantiate(RideTheBusText, ridePlayerContent).text = "Move 4 Spaces!";
            MoveTotal += 4;
            isCorrectThree = true;
            }
            else if (!inside && userinput.Equals("o"))
            {
                Text tw = Instantiate(RideTheBusText, ridePlayerContent);
                tw.text += ("Correct! The card was " + deck[card3].ToString());
            Instantiate(RideTheBusText, ridePlayerContent).text = "Move 4 Spaces!";
            MoveTotal += 4;
            isCorrectThree = true;
            }
            else {
                Text tw = Instantiate(RideTheBusText, ridePlayerContent);
                tw.text += ("DRINK! The card was " + deck[card3].ToString());
                isCorrectThree = false;
            }
            player[0].setCard3(deck[card3]);
            Text sts = Instantiate(RideTheBusText, ridePlayerContent);
            sts.text += ("Press Enter To Continue");
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("RideTheBusT"))
            {
                Destroy(g);
            }
        //}
        StartCoroutine(stageFour(numOfPlayers, false));
    }

    bool isCorrectFour;
    private IEnumerator stageFour(int numOfPlayers, bool rideTheBus)
    {
        isCorrectFour = true;
        //for (int i = 1; i <= numOfPlayers; i++)
        //{
            /*
            GameObject playerC = (GameObject)GameObject.FindGameObjectsWithTag("Player").GetValue(i - 1);
            if (rideTheBus)
            {
                i = numOfPlayers;
            }*/
            System.Random rand = new System.Random();
            int card1 = rand.Next(51);
            Text ta = Instantiate(RideTheBusText, ridePlayerContent);
            ta.text += SignInLoginPageController.CurrentUserName + ": ";
            Text t = Instantiate(RideTheBusText, ridePlayerContent);
            t.text += "Guess a suit (Diamonds/Hearts/Spades/Clubs)";
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            string userinput = RideTheBusEnterText.text;
            if (userinput.ToLower().Equals(deck[card1].getSuit().ToLower()))
            {
                Text td = Instantiate(RideTheBusText, ridePlayerContent);
                td.text += ("Correct! You get to distribute 5 drinks");
            Instantiate(RideTheBusText, ridePlayerContent).text = "Move 5 Spaces!";
            MoveTotal += 5;
            isCorrectFour = true;
            }
            else {
                Text td = Instantiate(RideTheBusText, ridePlayerContent);
                td.text += ("DRINK! The card was " + deck[card1].ToString());
                isCorrectFour = false;
            }
            player[0].setCard4(deck[card1]);
            Text sts = Instantiate(RideTheBusText, ridePlayerContent);
            sts.text += ("Press Enter To Continue");
            RideTheBusReady = false;
            while (!RideTheBusReady)
            {
                yield return new WaitForEndOfFrame();
            }
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("RideTheBusT"))
            {
                Destroy(g);
            }
        //}
        RideTheBusReady = false;
        SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").GetValueAsync().ContinueWith(task2 =>
        {
            if (task2.IsFaulted)
            {

            }
            else
            {
                int current = int.Parse(task2.Result.Value.ToString());
                SignInLoginPageController.database.Child(SignInLoginPageController.CurrentGameName).Child("Users").Child(SignInLoginPageController.CurrentUserName).Child("Position").SetValueAsync(current + MoveTotal);
                Debug.Log("Start Game? " + startedNewGame);
                if (!startedNewGame) 
                    StartCoroutine(WaitThenSetGame());
            }
        });
        ridemini.gameObject.SetActive(false);
        
        /*
        if (rideTheBus)
        {
            //return isCorrectFour;
        }
        for (int i = 1; i <= 5; i++)
        {
            Text td = Instantiate(RideTheBusText, ridePlayerContent);
            td.text += ("M A T C H  A  C A R D  W I T H  T H E  F L I P P E D  C A R D S");
            displayPyramid(i);
            for (int j = 1; j <= numOfPlayers; j++)
            {
                Instantiate(RideTheBusText, ridePlayerContent).text = ("\nPlayer " + i + ":");
                Instantiate(RideTheBusText, ridePlayerContent).text = "Press (i) to see your cards or (x) to continue...";
                RideTheBusReady = false;
                string userinput2 = "i";
                while (!userinput2.Equals("x"))
                {
                    while (!RideTheBusReady)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    userinput2 = RideTheBusEnterText.text;
                    if (userinput2.ToLower().Equals("i"))
                    {
                        Instantiate(RideTheBusText, ridePlayerContent).text = (player[j].ToString());
                    }
                }
            }
            //CLEAR SCREEN
        }
        /*
        Scanner in = new Scanner(System.in);
        System.out.print("Please enter the player with the most cards left: ");
        int mostCards = in.nextInt();
        System.out.print("Please enter the player with the least cards left: ");
        int leastCards = in.nextInt();
        System.out.println("\n\nP L A Y E R  " + leastCards + "  W I N S ! ! ! !\n\nP L A Y E R  " + mostCards + "  H A S  T O  R I D E  T H E  B U S ! ! ! !");
        rideTheBus(mostCards);*/
    }

    private void displayPyramid(int i)
    {
        switch (i)
        {
            case (1):
                Instantiate(RideTheBusText, ridePlayerContent).text = ("Each matched card is worth two drinks");
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   ||   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[0].ToString() + "||" + pyramid[1].ToString() + "||" + pyramid[2].ToString() + "||" + pyramid[3].ToString() + "||" + pyramid[4].ToString() + "|";
                break;
            case (2):
                Instantiate(RideTheBusText, ridePlayerContent).text = ("Each matched card is worth four drinks");
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[5].ToString() + "||" + pyramid[6].ToString() + "||" + pyramid[7].ToString() + "||" + pyramid[8].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[0].ToString() + "||" + pyramid[1].ToString() + "||" + pyramid[2].ToString() + "||" + pyramid[3].ToString() + "||" + pyramid[4].ToString() + "|";
                break;
            case (3):
                Instantiate(RideTheBusText, ridePlayerContent).text = ("Each matched card is worth eight drinks");
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|   X   ||   X   |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[9].ToString() + "||" + pyramid[10].ToString() + "||" + pyramid[11].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[5].ToString() + "||" + pyramid[6].ToString() + "||" + pyramid[7].ToString() + "||" + pyramid[8].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[0].ToString() + "||" + pyramid[1].ToString() + "||" + pyramid[2].ToString() + "||" + pyramid[3].ToString() + "||" + pyramid[4].ToString() + "|";
                break;
            case (4):
                Instantiate(RideTheBusText, ridePlayerContent).text = ("Each matched card is worth sixteen drinks");
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[12].ToString() + "||" + pyramid[13].ToString() + " |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[9].ToString() + "||" + pyramid[10].ToString() + "||" + pyramid[11].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[5].ToString() + "||" + pyramid[6].ToString() + "||" + pyramid[7].ToString() + "||" + pyramid[8].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[0].ToString() + "||" + pyramid[1].ToString() + "||" + pyramid[2].ToString() + "||" + pyramid[3].ToString() + "||" + pyramid[4].ToString() + "|";
                break;
            case (5):
                Instantiate(RideTheBusText, ridePlayerContent).text = ("Each matched card is worth one shot (can be amoung many people) drinks");
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[14].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[12].ToString() + "||" + pyramid[13].ToString() + " |";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[9].ToString() + "||" + pyramid[10].ToString() + "||" + pyramid[11].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[5].ToString() + "||" + pyramid[6].ToString() + "||" + pyramid[7].ToString() + "||" + pyramid[8].ToString() + "|";
                Instantiate(RideTheBusText, ridePlayerContent).text = "|" + pyramid[0].ToString() + "||" + pyramid[1].ToString() + "||" + pyramid[2].ToString() + "||" + pyramid[3].ToString() + "||" + pyramid[4].ToString() + "|";
                break;

        }
    }

        /*
       public class RideTheBus
       {

           public static void main(String[] args)
           {
               System.out.println("\n\nE N D  O F  G A M E");
           }




           private static void rideTheBus(int mostCards)
           {
               System.out.println("\nGet ready player " + mostCards + ". Hope you're feeling lucky...");
               int numOfCards = 0;
               bool done = false;
               while (!done)
               {
                   stageOne(mostCards, true) 
                   if (isCorrectOne&& stageTwo(mostCards, true) && stageThree(mostCards, true) && stageFour(mostCards, true))
                   {
                       done = true;
                   }
                   else {
                       done = false;
                   }
                   numOfCards++;
                   if (numOfCards == 52)
                   {
                       done = true;
                   }
               }


           }

          

           }







          */
    }

