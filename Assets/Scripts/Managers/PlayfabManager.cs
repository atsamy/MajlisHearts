using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;
using PlayFab.Json;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager instance;
    //GameManager gameManager;


    public delegate void PlayerLogedIn(UserTitleInfo userInfo);
    public event PlayerLogedIn OnPlayerLoggedIn;

    public delegate void InventoryReturned(Dictionary<string,int> currency, List<ItemInstance> userInfo);
    public event InventoryReturned OnInventoryReturned;

    public delegate void UserDataReturned(Dictionary<string, UserDataRecord> userData);
    public event UserDataReturned OnUserDataReturned;

    public delegate void CatalogReturned(List<CatalogItem> catalog);
    public event CatalogReturned OnCatalogReturned;

    public delegate void TitleDataReturned(Dictionary<string, string> titleData);
    public event TitleDataReturned OnTitleDataReturned;

    public delegate void ConnectionError(string message);
    public event ConnectionError OnConnectionError;

    //public delegate void LeaderboardDelegate(bool success, List<LeaderBoardData> leaderboardData, bool isWorld);

    bool emptyLoadOut;
    void Awake()
    {
        if (instance == null) // check to see if the instance has a reference
        {
            instance = this; // if not, give it a reference to this class...
            DontDestroyOnLoad(this.gameObject); // and make this object persistent as we load new scenes
        }
        else // if we already have a reference then remove the extra manager from the scene
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {

    }

    public void DeviceLogin()
    {
#if UNITY_ANDROID
        Debug.Log("Login Android");
        var request = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };

        PlayFabClientAPI.LoginWithAndroidDeviceID(request,
            (result) => SetupSessionData((titleData) =>
            {
                OnPlayerLoggedIn?.Invoke(titleData);
            }),
            (error) =>
            {
                Debug.Log("Login Error: " + error.ErrorMessage);
                OnConnectionError?.Invoke(error.ErrorMessage);
            });
#endif

#if UNITY_IOS
        Debug.Log("Login IOS");
        var request = new LoginWithIOSDeviceIDRequest { DeviceId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };

        PlayFabClientAPI.LoginWithIOSDeviceID(request,
            (result) => SetupSessionData(result),
            (error) => Debug.Log("Login Error: " + error.ErrorMessage));
#endif
    }

    internal void RemoveQuest(string id, Action<bool> p)
    {
        //throw new NotImplementedException();
    }

    internal void LoginWithApple()
    {
        //PlayFabClientAPI.LoginWithIOSDeviceID
        PlayFabClientAPI.LoginWithGameCenter(new LoginWithGameCenterRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            //EncryptedRequest = "",
            PlayerId = Social.localUser.id
        }, (successLoginResult) =>
        {
            Debug.LogFormat("Login With Game Center Success: ", successLoginResult.PlayFabId);
            SetupSessionData((titleData) =>
            {
                OnPlayerLoggedIn?.Invoke(titleData);
            });
        }, (errorResult) =>
        {
            Debug.Log(errorResult.GenerateErrorReport());
            //});
        });
    }

    //internal void UpdateQuests(Quest[] quests)
    //{
    //    Dictionary<string, string> data = new Dictionary<string, string>();
    //    data.Add("Quests", JsonUtility.ToJson(quests));

    //    UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest()
    //    {
    //        Data = data
    //    };

    //    PlayFabClientAPI.UpdateUserData(updateUserDataRequest,(result)=> 
    //    {

    //    },(error)=> {

    //    });
    //}


    internal void LoginWithGoogle(string authCode)
    {
        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            ServerAuthCode = authCode,
            CreateAccount = true,
        }, (successLoginResult) =>
        {
            Debug.LogFormat("Login With Google Success: ", successLoginResult.PlayFabId);
            //SetupSessionData();
        }, (errorResult) =>
        {
            Debug.Log(errorResult.GenerateErrorReport());
            //});
        });
    }


    private void SetupSessionData(Action<UserTitleInfo> onData)
    {
        //Get Game Data
        Debug.Log("Get Player Details");
        UserAccountInfo accountInfo = null;
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { },
            (res) =>
            {
                accountInfo = res.AccountInfo;

                if (accountInfo != null)
                {
                    onData?.Invoke(res.AccountInfo.TitleInfo);
                }
                else
                    Debug.Log("Account Info not found");
            },
            (error) => Debug.Log("GetInfo Error: " + error.ErrorMessage)
        );
    }

    public void GetTitleData()
    {
        PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
            (res) =>
            {
                OnTitleDataReturned?.Invoke(res.Data);
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("Got error getting titleData: " + error.ErrorMessage);
            }
        );
    }

    public void SetPlayerData(Dictionary<string, string> data)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = data
        }, (result) =>
        {
            Debug.Log("Update user data sucessfully");
        }, (error) =>
        {
            OnConnectionError?.Invoke(error.ErrorMessage);
            Debug.Log("GetInfo Error: " + error.ErrorMessage);
        });
    }

    public void GetCatalog()
    {
        PlayFabClientAPI.GetCatalogItems(
            new GetCatalogItemsRequest { },
            (res) =>
            {
                OnCatalogReturned?.Invoke(res.Catalog);
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("GetInfo Error: " + error.ErrorMessage);
            }
        );
    }

    public void GetPlayerInfo()
    {
        //PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { Username = "theuser" },())
        //PlayFabClientAPI.Matchmake
    }

    public void GetUserInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            (response) =>
            {
                OnInventoryReturned?.Invoke(response.VirtualCurrency,response.Inventory);
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("GetInventory Error: " + error.ErrorMessage);
            }
        );
    }

    //private CatalogItem GetCatalogueItem(ItemInstance InventoryItem)
    //{
    //    for (int i = 0; i < MainCatalogue.Count; i++)
    //    {
    //        if (MainCatalogue[i].ItemId == InventoryItem.ItemId)
    //            return MainCatalogue[i];
    //    }
    //    return null;
    //}

    //Equipped items should be saved to loadout separated from inventory
    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (response) =>
            {
                if (response.Data != null)
                {
                    OnUserDataReturned?.Invoke(response.Data);
                }
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("GetUserData Error: " + error.ErrorMessage);
            }
        );
    }

    internal void UpdateUserLoadout(string[] data)
    {
        if (data != null)
        {
            PlayFabClientAPI.UpdateUserData(
                new UpdateUserDataRequest()
                {
                    Data = new Dictionary<string, string>()
                    {
                        {"Loadout", JsonUtility.ToJson(data)}
                    }
                },
                (res) =>
                {
                    Debug.Log("Update Loadout Success");
                },
                (error) =>
                {
                    Debug.Log("Update Loadout Error: " + error.ErrorMessage);
                }
            );
        }
    }

    public void SetDisplayName(string UserName, Action<bool> result)
    {
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest() { DisplayName = UserName },
            (res) =>
            {
                //gameManager.Me.Name = UserName;
                result.Invoke(true);
            },
            (error) =>
            {
                Debug.Log("SetDisplayName Error: " + error.ErrorMessage);
                result.Invoke(false);
            }
        );
    }

    public void AddItemToInventory(CatalogueItem CatalogueItem)
    {
        PurchaseItemRequest request = new PurchaseItemRequest();

        request.Price = CatalogueItem.Price;
        request.ItemId = CatalogueItem.ID;
        request.VirtualCurrency = "SC";

        PlayFabClientAPI.PurchaseItem(request, (result) =>
        {
            Debug.Log("Add Item success");
        },
        (error) =>
        {
            Debug.Log("Add Item Error: " + error.ErrorMessage);
        });
    }

    public void AddCurrency(string code, int value, Action<bool> result)
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "addCurrency",
                FunctionParameter = new { CurrencyType = code, Amount = value }
            },
            (res) =>
            {
                Debug.Log("Currency Added Successfully");
                result.Invoke(true);
            },
            (error) =>
            {
                Debug.Log("AddCurrency Error: " + error.ErrorMessage);
                result.Invoke(false);
            }
        );
    }

    public void DeductCurrency(string code, int value, Action<bool> result)
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "deductCurrency",
                FunctionParameter = new { CurrencyType = code, Amount = value }
            },
            (res) =>
            {
                Debug.Log("Currency Deducted Successfully");
                result.Invoke(true);
            },
            (error) =>
            {
                Debug.Log("DeductCurrency Error: " + error.ErrorMessage);
                result.Invoke(false);
            }
        );
    }

    internal void GetPlayerStats(Action<List<StatisticValue>> onData)
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest()
            {
            },
            (response) =>
            {
                onData?.Invoke(response.Statistics);
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("GetPlayerStats Error: " + error.ErrorMessage);
            }
        );
    }

    //internal void GetMyWorldLeaderBoard(LeaderboardDelegate LeaderboardCallback)
    //{
    //    List<LeaderBoardData> leaderboardData = new List<LeaderBoardData>();

    //    PlayFabClientAPI.GetLeaderboardAroundPlayer(
    //        new GetLeaderboardAroundPlayerRequest()
    //        {
    //            StatisticName = "Points",
    //            MaxResultsCount = 10
    //        },
    //        (response) =>
    //        {
    //            foreach (var entry in response.Leaderboard)
    //            {
    //                LeaderBoardData newData = new LeaderBoardData()
    //                {
    //                    PlayerID = entry.PlayFabId,
    //                    Rank = (int)entry.Position,
    //                    playerName = entry.DisplayName,
    //                    Points = entry.StatValue
    //                };

    //                leaderboardData.Add(newData);
    //            }

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(true, leaderboardData, false);
    //        },
    //        (error) =>
    //        {
    //            Debug.Log("GetWorldLeaderBoard Error: " + error.ErrorMessage);

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(false, leaderboardData, true);
    //        }
    //    );
    //}

    //internal void GetDefaultWorldLeaderBoard(LeaderboardDelegate LeaderboardCallback)
    //{
    //    List<LeaderBoardData> leaderboardData = new List<LeaderBoardData>();

    //    PlayFabClientAPI.GetLeaderboard(
    //        new GetLeaderboardRequest()
    //        {
    //            StartPosition = 0,
    //            StatisticName = "Points",
    //            MaxResultsCount = 10
    //        },
    //        (response) =>
    //        {
    //            foreach (var entry in response.Leaderboard)
    //            {
    //                LeaderBoardData newData = new LeaderBoardData()
    //                {
    //                    PlayerID = entry.PlayFabId,
    //                    Rank = (int)entry.Position,
    //                    playerName = entry.DisplayName,
    //                    Points = entry.StatValue
    //                };

    //                leaderboardData.Add(newData);
    //            }

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(true, leaderboardData, true);
    //        },
    //        (error) =>
    //        {
    //            Debug.Log("GetDefaultWorldLeaderBoard Error: " + error.ErrorMessage);

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(false, leaderboardData, true);
    //        }
    //    );
    //}

    //internal void GetFriendLeaderBoard(LeaderboardDelegate LeaderboardCallback)
    //{
    //    List<LeaderBoardData> leaderboardData = new List<LeaderBoardData>();

    //    PlayFabClientAPI.GetFriendLeaderboard(
    //        new GetFriendLeaderboardRequest()
    //        {
    //            StartPosition = 0,
    //            StatisticName = "Points",
    //            MaxResultsCount = 10
    //        },
    //        (response) =>
    //        {
    //            foreach (var entry in response.Leaderboard)
    //            {
    //                LeaderBoardData newData = new LeaderBoardData()
    //                {
    //                    PlayerID = entry.PlayFabId,
    //                    Rank = (int)entry.Position,
    //                    playerName = entry.DisplayName,
    //                    Points = entry.StatValue
    //                };

    //                leaderboardData.Add(newData);
    //            }

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(true, leaderboardData, false);
    //        },
    //        (error) =>
    //        {
    //            Debug.Log("GetFriendLeaderBoard Error: " + error.ErrorMessage);

    //            if (LeaderboardCallback != null)
    //                LeaderboardCallback(false, leaderboardData, false);
    //        }
    //    );
    //}

}


[Serializable]
public class PF_InventoryItem
{
    public ItemInstance InventoryInstance;
    public CatalogItem CatalogueItem;

    public PF_InventoryItem(ItemInstance _InventoryInstance, CatalogItem _CatalogueItem)
    {
        InventoryInstance = _InventoryInstance;
        CatalogueItem = _CatalogueItem;
    }
}



[Serializable]
public class Wrapper<T>
{
    public T[] array;
}