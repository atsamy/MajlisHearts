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


    public delegate void PlayerLogedIn(UserTitleInfo userInfo, bool newUser);
    public event PlayerLogedIn OnPlayerLoggedIn;

    public delegate void InventoryReturned(Dictionary<string, int> currency, List<ItemInstance> userInfo);
    public event InventoryReturned OnInventoryReturned;

    public delegate void UserDataReturned(Dictionary<string, UserDataRecord> userData);
    public event UserDataReturned OnUserDataReturned;

    public delegate void UserStatsReturned(List<StatisticValue> statisticValues);
    public event UserStatsReturned OnUserStatsReturned;

    public delegate void CatalogReturned(List<CatalogItem> catalog);
    public event CatalogReturned OnCatalogReturned;

    public delegate void TitleDataReturned(Dictionary<string, string> titleData);
    public event TitleDataReturned OnTitleDataReturned;


    public delegate void ConnectionError(string message);
    public event ConnectionError OnConnectionError;

    bool emptyLoadOut;
    string currentPlayerId;
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
        Debug.Log("Login Android"); //Application.identifier };
        var request = new LoginWithAndroidDeviceIDRequest { CreateAccount = true, AndroidDeviceId = SystemInfo.deviceUniqueIdentifier };

        PlayFabClientAPI.LoginWithAndroidDeviceID(request,
            (result) => SetupSessionData((titleData) =>
            {
                OnPlayerLoggedIn?.Invoke(titleData, result.NewlyCreated);
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
            (result) => SetupSessionData((titleData) =>
            {
                OnPlayerLoggedIn?.Invoke(titleData, result.NewlyCreated);
            }),
            (error) =>
            {
                Debug.Log("Login Error: " + error.ErrorMessage);
                OnConnectionError?.Invoke(error.ErrorMessage);
            });
#endif
    }

    //internal void RemoveQuest(string id, Action<bool> p)
    //{

    //}

    internal void LoginWithApple()
    {
        //PlayFabClientAPI.LoginWithIOSDeviceID
        PlayFabClientAPI.LoginWithGameCenter(new LoginWithGameCenterRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            PlayerId = Social.localUser.id
        }, (successLoginResult) =>
        {
            Debug.LogFormat("Login With Game Center Success: ", successLoginResult.PlayFabId);
            SetupSessionData((titleData) =>
            {
                OnPlayerLoggedIn?.Invoke(titleData, successLoginResult.NewlyCreated);
            });
        }, (errorResult) =>
        {
            Debug.Log(errorResult.GenerateErrorReport());
            DeviceLogin();
        });
    }


    internal void SetAvatar(string avatar)
    {
        UpdateAvatarUrlRequest request = new UpdateAvatarUrlRequest()
        {
            ImageUrl = avatar
        };

        PlayFabClientAPI.UpdateAvatarUrl(request, (result) =>
         {
             Debug.Log("avatar update success");
         }, (error) =>
         {
             Debug.Log(error.ErrorMessage);
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

    //public void AddFriend(string name, Action<bool> success)
    //{
    //    PlayFabClientAPI.AddFriend(new AddFriendRequest() { FriendTitleDisplayName = name },
    //        (result) =>
    //        {
    //            if (result.Created)
    //            {
    //                success?.Invoke(true);
    //            }
    //            else
    //            {
    //                success?.Invoke(false);
    //            }
    //        }, (error) =>
    //        {
    //            success?.Invoke(false);
    //        });
    //}

    public void AddFriend(string name, Action<bool, string> success)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest() { TitleDisplayName = name }, (result) =>
        {
            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest()
                {
                    FunctionName = "SendFriendRequest",
                    FunctionParameter = new { PlayFabId = currentPlayerId, FriendPlayFabId = result.AccountInfo.PlayFabId }
                },
                (res) =>
                {
                    Debug.Log("add friend Success" + PlayFabSimpleJson.SerializeObject(res.FunctionResult));
                    success?.Invoke(true, currentPlayerId);
                },
                (error) =>
                {
                    Debug.Log("add friend Error: " + error.ErrorMessage);
                    success?.Invoke(false, null);
                });

        }, (error) =>
        {
            Debug.Log("add friend Error: " + error.ErrorMessage);
        });
    }

    internal void AcceptFriendRequest(string friendPlayFabID, string friendName, Action<bool, string> success)
    {
        Debug.Log(string.Format("friends {0} {1}", friendPlayFabID, friendName));

        PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest()
                {
                    FunctionName = "AcceptFriendRequest",
                    FunctionParameter = new { PlayFabId = currentPlayerId, FriendPlayFabId = friendPlayFabID }
                },
                (res) =>
                {
                    Debug.Log("Accept Friend Success" + PlayFabSimpleJson.SerializeObject(res.FunctionResult));
                    success?.Invoke(true, friendName);
                },
                (error) =>
                {
                    Debug.Log("Accept Friend Error: " + error.ErrorMessage);
                    success?.Invoke(false, friendName);
                });
    }

    internal void DenyFriendRequest(string friendPlayFabID, Action<bool> result = null)
    {
        PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest()
                {
                    FunctionName = "DenyFriendRequest",
                    FunctionParameter = new { PlayFabId = currentPlayerId, FriendPlayFabId = friendPlayFabID }
                },
                (res) =>
                {
                    Debug.Log("UpgradeCard Success" + PlayFabSimpleJson.SerializeObject(res.FunctionResult));
                    result?.Invoke(true);

                },
                (error) =>
                {
                    Debug.Log("UpgradeCard Error: " + error.ErrorMessage);
                    result?.Invoke(false);
                });
    }

    public void GetFriends(Action<List<FriendInfo>> onFriendsReturned)
    {
        PlayerProfileViewConstraints Profile = new PlayerProfileViewConstraints()
        {
            ShowAvatarUrl = true,
            ShowDisplayName = true
        };


        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest() { ProfileConstraints = Profile }, (result) =>
          {
              onFriendsReturned?.Invoke(result.Friends);
          },
        (error) =>
        {
            onFriendsReturned?.Invoke(null);
        });
    }

    internal void LoginWithGoogle(string authCode)
    {
        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            ServerAuthCode = authCode,
            CreateAccount = true,
        }, (result) => SetupSessionData((titleData) =>
        {
            OnPlayerLoggedIn?.Invoke(titleData, result.NewlyCreated);
        }), (errorResult) =>
        {
            Debug.LogError(errorResult.GenerateErrorReport());
        });
    }

    internal void GetHeartsLeaderboard(Action<List<PlayerLeaderboardEntry>> leaderBoardResult)
    {
        GetLeaderboardRequest getLeaderboardRequest = new GetLeaderboardRequest()
        {
            StatisticName = "HeartsPoints",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(getLeaderboardRequest, (result) =>
        {
            leaderBoardResult?.Invoke(result.Leaderboard);
        }, (error) =>
        {
            Debug.LogError(error.ErrorMessage);
        });
    }

    internal void UpdateStats(string statName, int value, Action<bool> requestResult)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate> { new StatisticUpdate() { Value = value, StatisticName = statName } }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) =>
        {
            requestResult?.Invoke(true);
        }, (error) =>
        {
            Debug.LogError($"UpdatePlayerStatistics: {error.ErrorMessage}");
            requestResult?.Invoke(false);
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
                    currentPlayerId = res.AccountInfo.PlayFabId;
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
                OnInventoryReturned?.Invoke(response.VirtualCurrency, response.Inventory);
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
        PlayFabClientAPI.AddUserVirtualCurrency(
            new AddUserVirtualCurrencyRequest()
            {
                VirtualCurrency = code,
                Amount = value
            },
            (res) =>
            {
                Debug.Log("Currency Added Successfully");
                result?.Invoke(true);
            },
            (error) =>
            {
                Debug.Log("AddCurrency Error: " + error.ErrorMessage);
                result?.Invoke(false);
            }
        );
    }

    public void DeductCurrency(string code, int value, Action<bool> result)
    {

        PlayFabClientAPI.SubtractUserVirtualCurrency(
            new SubtractUserVirtualCurrencyRequest()
            {
                VirtualCurrency = code,
                Amount = value
            },
            (res) =>
            {
                Debug.Log("Currency Subtracted Successfully");
                result?.Invoke(true);
            },
            (error) =>
            {
                Debug.Log("Subtract Currency Error: " + error.ErrorMessage);
                result?.Invoke(false);
            }
        );
    }

    internal void GetPlayerStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest()
            {
                StatisticNames = new List<string>() { "HeartsPoints" }
            },
            (response) =>
            {
                OnUserStatsReturned?.Invoke(response.Statistics);
            },
            (error) =>
            {
                OnConnectionError?.Invoke(error.ErrorMessage);
                Debug.Log("GetPlayerStats Error: " + error.ErrorMessage);
            }
        );
    }

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