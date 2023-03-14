using GooglePlayGames;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames.BasicApi;
using DG.Tweening;
using UnityEngine.Playables;
//using UnityEngine.Analytics;

public class LoginManager : MonoBehaviour
{
    public GameObject ConnectionError;
    public UsernamePanel UserNamePanel;

    public LanguagePanel LanguagePanel;

    [SerializeField]
    Popup noConnection;
    [SerializeField]
    Image fillBar;
    PlayfabManager playfab;
    int loginValue;

    //bool newUser;

    [SerializeField]
    bool forceDeviceLogin;
    //public SplashLoading splashLoading;
    public Text Debug;

    void Start()
    {
        playfab = PlayfabManager.instance;

        playfab.OnPlayerLoggedIn += Playfab_OnPlayerLoggedIn;
        playfab.OnUserDataReturned += Playfab_OnUserDataReturned;
        playfab.OnUserStatsReturned += Playfab_OnUserStatsReturned;
        playfab.OnInventoryReturned += Playfab_OnInventoryReturned;
        playfab.OnConnectionError += Playfab_OnConnectionError;
        playfab.OnCatalogReturned += Playfab_OnCatalogReturned;
        playfab.OnTitleDataReturned += Playfab_OnTitleDataReturned;

        CheckConnection();
    }

    private void CheckConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            noConnection.ShowWithCode("noconnection", () =>
            {
                CheckConnection();
            });
        }
        else
        {
            LogIn();
        }
    }

    private void OnDisable()
    {
        playfab.OnPlayerLoggedIn -= Playfab_OnPlayerLoggedIn;
        playfab.OnUserDataReturned -= Playfab_OnUserDataReturned;
        playfab.OnUserStatsReturned -= Playfab_OnUserStatsReturned;
        playfab.OnInventoryReturned -= Playfab_OnInventoryReturned;
        playfab.OnConnectionError -= Playfab_OnConnectionError;
        playfab.OnCatalogReturned -= Playfab_OnCatalogReturned;
        playfab.OnTitleDataReturned -= Playfab_OnTitleDataReturned;
    }

    public void LogIn()
    {

        loginValue = PlayerPrefs.GetInt("login", 0);

        if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) && !forceDeviceLogin)// || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            print("device login");
            InitPlayGames();
            AccountLogin();
        }
        else
        {
            print("account login");
            playfab.DeviceLogin();
        }


    }

    void InitPlayGames()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
#endif
    }

    private void Playfab_OnTitleDataReturned(Dictionary<string, string> titleData)
    {
        fillBar.DOFillAmount(0.8f, 0.2f);
        foreach (var item in titleData)
        {
            if (item.Key == "Tasks")
            {
                TasksManager.Instance.Tasks = JsonUtility.FromJson<Wrapper<TaskData>>(item.Value).array;
            }
            else if (item.Key == "GameData")
            {
                GameManager.Instance.GameData = JsonUtility.FromJson<GameData>(item.Value);
            }
            else if (item.Key == "Gems")
            {
                Purchaser.Instance.GemsPrices = JsonUtility.FromJson<Wrapper<int>>(item.Value).array;
            }
        }
        playfab.GetCatalog();
    }

    private void Playfab_OnCatalogReturned(List<CatalogItem> catalogItems)
    {
        fillBar.DOFillAmount(1f, 0.2f);
        Dictionary<string, List<CatalogueItem>> AllItems = new Dictionary<string, List<CatalogueItem>>();

        foreach (var item in catalogItems)
        {
            if (AllItems.ContainsKey(item.ItemClass))
            {
                AllItems[item.ItemClass].Add(new CatalogueItem(item));
            }
            else
            {
                AllItems.Add(item.ItemClass, new List<CatalogueItem>());
                AllItems[item.ItemClass].Add(new CatalogueItem(item));
            }
        }

        if (GameManager.Instance.EquippedItem.Count == 0)
            SetDefaultItems(AllItems);

        GameManager.Instance.Catalog = AllItems;
        StartCoroutine(LoadYourAsyncScene());
    }

    public void AccountLogin()
    {
        Social.localUser.Authenticate((success) =>
        {
            print("social " + success);
            //Debug.text = "Social: " + success;

            if (success)
            {
#if UNITY_ANDROID
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                print("Server Auth Code: " + serverAuthCode);

                //PlayGamesPlatform.Instance.RequestServerSideAccess(false, (code) =>
                //{
                //    print("code:"+code);
                PlayfabManager.instance.LoginWithGoogle(serverAuthCode);
                //});

#elif UNITY_IOS
                //playfab.DeviceLogin();
                playfab.LoginWithApple();
#endif
            }
            else
            {
                playfab.DeviceLogin();
            }
        });
    }

    private void Playfab_OnPlayerLoggedIn(UserTitleInfo userInfo, bool newUser)
    {
        fillBar.DOFillAmount(0.2f, 0.2f);
        if (newUser)
        {
            //this.newUser = true;
            SetLanguageAndUserName();
        }
        else if (string.IsNullOrEmpty(userInfo.DisplayName))
        {
            print("lang:" + LanguageManager.Instance.CurrentLanguage);
            ShowUserNamePanel();
        }
        else
        {
            GameManager.Instance.MyPlayer.Avatar = userInfo.AvatarUrl;
            GameManager.Instance.EquippedItem.Add("Avatar", userInfo.AvatarUrl);
            GameManager.Instance.MyPlayer.Name = userInfo.DisplayName;
            playfab.GetUserInventory();
        }
    }

    private void Playfab_OnInventoryReturned(Dictionary<string, int> currency, List<ItemInstance> inventory)
    {
        fillBar.DOFillAmount(0.4f, 0.2f);
        GameManager.Instance.Coins = currency["SC"];
        GameManager.Instance.Gems = currency["HC"];
        GameManager.Instance.Inventory = new List<InventoryItem>();

        foreach (var item in inventory)
        {
            GameManager.Instance.Inventory.Add(new InventoryItem(item.ItemClass, item.ItemId));
        }

        playfab.GetUserData();
    }

    private void Playfab_OnUserDataReturned(Dictionary<string, UserDataRecord> userData)
    {
        fillBar.DOFillAmount(0.6f, 0.2f);
        //GameManager.Instance.Customization = new List<InventoryItem>();
        TasksManager.Instance.FinishedTasks = new List<FinishedTask>();

        foreach (var item in userData)
        {
            switch (item.Key)
            {
                case "Customization":
                    TasksManager.Instance.FinishedTasks = JsonUtility.FromJson<Wrapper<FinishedTask>>(item.Value.Value).array.ToList();
                    break;
                //case "Avatar":
                //    GameManager.Instance.MyPlayer.Avatar = item.Value.Value;
                //    break;
                case "CardBack":
                    GameManager.Instance.EquippedItem.Add("CardBack", item.Value.Value);
                    break;
                case "TableTop":
                    GameManager.Instance.EquippedItem.Add("TableTop", item.Value.Value);
                    break;
                case "majlisName":
                    GameManager.Instance.MyPlayer.MajlisName = item.Value.Value;
                    break;
                //case "TaskIndex":
                //    TasksManager.Instance.SetIndex(int.Parse(item.Value.Value));
                //    break;
                case "Points":
                    GameManager.Instance.MyPlayer.Points = int.Parse(item.Value.Value);
                    break;
                case "tutorial":
                    GameManager.Instance.IsTutorialDone = true;
                    break;
            }
        }
        playfab.GetPlayerStats();
    }

    private void Playfab_OnUserStatsReturned(List<StatisticValue> statisticValues)
    {
        foreach (var item in statisticValues)
        {
            GameManager.Instance.MyPlayer.GamePoints.Add(item.StatisticName, item.Value);
        }

        playfab.GetTitleData();
    }

    public void SetDefaultItems(Dictionary<string, List<CatalogueItem>> AllItems)
    {
        List<InventoryItem> customization = new List<InventoryItem>();

        foreach (var classItem in AllItems)
        {
            foreach (var item in classItem.Value)
            {
                if (item.IsDefault)
                {
                    if (item.IsCustomization)
                    {
                        customization.Add(new InventoryItem(item.ItemClass, item.ID));
                    }
                    else
                    {
                        GameManager.Instance.EquippedItem.Add(item.ItemClass, item.ID);
                    }

                    playfab.AddItemToInventory(item);
                }
            }
        }

        playfab.SetPlayerData(GameManager.Instance.EquippedItem);

        //Wrapper<InventoryItem> wrappedCustomization = new Wrapper<InventoryItem>();
        //wrappedCustomization.array = customization.ToArray();

        //PlayfabManager.instance.SetPlayerData(new Dictionary<string, string>()
        //{
        //    { "Customization", JsonUtility.ToJson(wrappedCustomization) }
        //});
    }

    private void Playfab_OnConnectionError(string message)
    {
        ConnectionError.SetActive(true);
    }

    private void SetLanguageAndUserName()
    {
        LanguagePanel.Open(() =>
        {
            ShowUserNamePanel();
        });
    }

    void ShowUserNamePanel()
    {
        UserNamePanel.Show((name) =>
        {
            playfab.SetDisplayName(name, (result) =>
            {
                if (result)
                {
                    GameManager.Instance.MyPlayer.Name = name;
                    //playfab.GetCatalog();
                    //playfab.GetTitleData();
                    playfab.GetUserInventory();
                    //StartCoroutine(LoadYourAsyncScene());
                }
                else
                {
                    ShowUserNamePanel();
                }
            });
        });
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        // splashLoading.StartLoading();

        int sceneIndex = 1;

        //PlayerPrefs.SetInt("Tutorial", 2);

        //if (GameManager.Instance.IsInGameTutorial)
        //{
        //    sceneIndex = 2;
        //    GameManager.Instance.RouteIndex = 0;
        //    GameManager.Instance.SetEquippedItems();
        //    GameManager.Instance.GameMode = GameModes.Training;
        //}

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);


        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void SetLanguage(string language)
    {
        //LanguageManager.Instance.setLanguage(language);
    }
}

//[Serializable]
//public class Wrapper<T>
//{
//    public T[] array;
//}
