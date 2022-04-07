//using GameSparks.Core;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//using UnityEngine.Analytics;

public class LoginManager : MonoBehaviour
{
    public GameObject ConnectionError;
    public UsernamePanel UserNamePanel;

    PlayfabManager playfab;
    int loginValue;

    //public SplashLoading splashLoading;
    public Text Debug;

    void Start()
    {
        playfab = PlayfabManager.instance;

        playfab.OnPlayerLoggedIn += Playfab_OnPlayerLoggedIn;
        playfab.OnUserDataReturned += Playfab_OnUserDataReturned;
        playfab.OnInventoryReturned += Playfab_OnInventoryReturned;
        playfab.OnConnectionError += Playfab_OnConnectionError;
        playfab.OnCatalogReturned += Playfab_OnCatalogReturned;

        LogIn();
    }

    private void OnDisable()
    {
        playfab.OnPlayerLoggedIn -= Playfab_OnPlayerLoggedIn;
        playfab.OnUserDataReturned -= Playfab_OnUserDataReturned;
        playfab.OnInventoryReturned -= Playfab_OnInventoryReturned;
        playfab.OnConnectionError -= Playfab_OnConnectionError;
        playfab.OnCatalogReturned -= Playfab_OnCatalogReturned;
    }

    public void LogIn()
    {
        InitPlayGames();
        loginValue = PlayerPrefs.GetInt("login", 0);

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AccountLogin();
        }
        else
        {
            playfab.DeviceLogin();
        }
    }

    void InitPlayGames()
    {
#if UNITY_ANDROID
        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //.RequestServerAuthCode(false).AddOauthScope("profile").Build();

        //PlayGamesPlatform.DebugLogEnabled = true;
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

        //print("initiated");
#endif
    }

    private void Playfab_OnCatalogReturned(List<CatalogItem> catalogItems)
    {
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
                //print("auth code " + PlayGamesPlatform.Instance.GetServerAuthCode());
                //PlayfabManager.instance.LoginWithGoogle(PlayGamesPlatform.Instance.GetServerAuthCode());
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

    private void Playfab_OnPlayerLoggedIn(UserTitleInfo userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.DisplayName))
        {
            ShowUserNamePanel();
        }
        else
        {
            playfab.GetUserInventory();
        }
    }

    private void Playfab_OnInventoryReturned(Dictionary<string, int> currency, List<ItemInstance> inventory)
    {
        GameManager.Instance.Currency = currency["SC"];
        GameManager.Instance.Inventory = new List<InventoryItem>();

        foreach (var item in inventory)
        {
            GameManager.Instance.Inventory.Add(new InventoryItem(item.ItemClass,item.ItemId));
        }

        playfab.GetUserData();
    }

    private void Playfab_OnUserDataReturned(Dictionary<string, UserDataRecord> userData)
    {
        GameManager.Instance.Customization = new List<InventoryItem>();

        foreach (var item in userData)
        {
            switch (item.Key)
            {
                case "Customization":
                    GameManager.Instance.Customization = JsonUtility.FromJson<Wrapper<InventoryItem>>(item.Value.Value).array.ToList();
                    break;
                default:
                    break;
            }
        }

        playfab.GetCatalog();
    }

    private void Playfab_OnConnectionError(string message)
    {
        ConnectionError.SetActive(true);
    }

    private void ShowUserNamePanel()
    {
        UserNamePanel.Show((name) =>
        {
            playfab.SetDisplayName(name, (result) =>
            {
                if (result)
                {
                    StartCoroutine(LoadYourAsyncScene());
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
