using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PlayFab.ClientModels;
//using PlayFab;
//using PlayFab.ServerModels;
using Facebook.Unity;

public class PlayFabError
{

}

public class PlayfabManager : SingeltonBase<PlayfabManager> {

    public static string PlayFabID;
    public static bool isLoggedIn;
   // public static string PlayerName;

    const string LEADERBOARDNAME = "score";

    // Use this for initialization
    public delegate void Callback( bool status, string message);
    Callback FBLoginCallback;

    const string LOGINPREFSKEY = "login";
    const string LOGINTYPEKEY = "logintype"; //0 for custom, 1 for facebook
    const string EMAILPREFSKEY = "email";
    const string PASSWORDPREFSKEY = "pwd";
    public static string PlayerName;
    public static string PlayerID;
    public static string PlayerGameName;
    void Start () {
        PlayerID = Random.Range(01, 999).ToString();
        PlayerGameName = "Player" + PlayFabID;
        FBManager.Instance.OnFacebookLoginSuccessful += OnFacebookLogin;
        FBManager.Instance.OnFacebookInitialized += OnFacebookLogin;
        FBManager.Instance.OnFacebookLoginFail += OnFacebookLoginFail;

        //if(PlayerPrefs.GetInt(LOGINPREFSKEY,0)==1)
        //{
        //    Login()
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CheckLogin(Callback callback)
    {
      //  Debug.Log(PlayerPrefs.GetInt(LOGINTYPEKEY, 0));
        if (PlayerPrefs.GetInt(LOGINTYPEKEY, 0) == 1)
        {
            Login(GetEmail(), GetPassword(), callback);
        }
        else
        {
            callback(false, "");
        }
    }
    void SetLoginPrefs(int type, string email = "", string password = "")
    {
        PlayerPrefs.SetInt(LOGINTYPEKEY, type);
        PlayerPrefs.SetString(EMAILPREFSKEY, email);
        PlayerPrefs.SetString(PASSWORDPREFSKEY, password);
    }
    string GetEmail()
    {
        return PlayerPrefs.GetString(EMAILPREFSKEY, "");
    }
    string GetPassword()
    {
        return PlayerPrefs.GetString(PASSWORDPREFSKEY, "");
    }

    public void UpdateFriends()
    {
        for (int i = 0; i < FBManager.Instance.friendsList.Count; i++)
        {
           // AddFriend(FriendIdType.Username, FBManager.Instance.friendsList[i].facebookID);
            AddFriend( FBManager.Instance.friendsList[i].facebookID);
        }
    }

   // public void AddFriend(FriendIdType idType, string friendId)
    public void AddFriend( string friendId)
    {
       // var request = new PlayFab.ClientModels.AddFriendRequest();
       // request.FriendUsername = friendId;
        //switch (idType)
        //{
        //    case FriendIdType.PlayFabId:
        //        request.FriendPlayFabId = friendId;
        //        break;
        //    case FriendIdType.Username:
        //        request.FriendUsername = friendId;
        //        break;
        //    case FriendIdType.Email:
        //        request.FriendEmail = friendId;
        //        break;
        //    case FriendIdType.DisplayName:
        //        request.FriendTitleDisplayName = friendId;
        //        break;
        //}
        // Execute request and update friends when we are done
      //  PlayFabClientAPI.AddFriend(request, result =>
      //  {
       //     Debug.Log("Friend added successfully!");
       // }, DisplayPlayFabError);
    }


    public void RegisterNewAccount(string name, string email, string password, Callback callback)
    {
        //PlayFabClientAPI.RegisterPlayFabUser(
        //    new RegisterPlayFabUserRequest() { Email = email, Password = password, DisplayName = name, RequireBothUsernameAndEmail = false },
        //    success => {
        //        Debug.Log("Success: RegisterPlayFabUser");
        //        PlayFabClientAPI.AddOrUpdateContactEmail(
        //            new AddOrUpdateContactEmailRequest() { EmailAddress = email },
        //            result => {
        //                Debug.Log("Success: AddOrUpdateContactEmail");
        //                callback(true, "Account created successfully");
        //            }, fault => {
        //                Debug.Log("Something went wrong with your API call AddOrUpdateContactEmail. Here's some debug information:");
        //                Debug.LogError(fault.GenerateErrorReport());
        //                callback(false, "Something wrong with email address");
        //            });
        //    },
        //    error => {
        //        Debug.Log("Something went wrong with your API call LoginWithCustomID. Here's some debug information:");
        //        Debug.LogError(error.GenerateErrorReport());
        //        callback(false, "Error while creating account");
        //    });

    }


    public void ForgetPassword(string email, Callback callback)
    { 

        //PlayFabServerAPI.SendCustomAccountRecoveryEmail(new SendCustomAccountRecoveryEmailRequest
        //{
        //    Email = email,
        //    EmailTemplateId = "39C4F1E4C97F88DA"
        //}, 
        //res =>
        //{
        //    //Debug.Log("An account recovery email has been sent to the player's email address.");
        //    callback(true, "An account recovery email has been sent to email address");
        //}, 
        //err=> {
        //    callback(false, "Error while sending account recovery email.");
        //    Debug.Log(err.GenerateErrorReport());
        //});
    }

    public void Login(string email, string password, Callback callback)
    {
        //isLoggedIn = false;
        //PlayFabClientAPI.LoginWithEmailAddress(new LoginWithEmailAddressRequest()
        //{ Email = email, Password = password },
        //success => {

        //    PlayFabClientAPI.GetPlayerProfile(new PlayFab.ClientModels.GetPlayerProfileRequest()
        //    {
        //        PlayFabId = success.PlayFabId,
        //        ProfileConstraints = new PlayFab.ClientModels.PlayerProfileViewConstraints() { ShowContactEmailAddresses = true, ShowDisplayName = true }
        //    }, res => { 
        //        if(res.PlayerProfile.ContactEmailAddresses[0].VerificationStatus == PlayFab.ClientModels.EmailVerificationStatus.Confirmed)
        //        {
        //            callback(true, "Login Success");
        //            PlayFabID = success.PlayFabId;
                  

        //            isLoggedIn = true;
        //            PlayerName = res.PlayerProfile.DisplayName;

        //            PlayerID = PlayFabID;
        //            PlayerGameName = res.PlayerProfile.DisplayName;
        //            GetData();
        //            SetLoginPrefs(1, email, password);
        //            LoadWorldLeaderboard();
        //            LoadFriendsLeaderboard();
        //        }
        //        else
        //        {

        //            callback(false, "Email not verified");
        //        }

        //    }, err => {
        //        callback(false, "Error while login");
        //        Debug.Log(err.GenerateErrorReport());
        //    });
          
        //}
        //, error => {
        //    callback(false, "Please check email or password");
        //});
    }

    public void Logout()
    {
        PlayerPrefs.DeleteKey(LOGINTYPEKEY);
        PlayerPrefs.DeleteKey(PASSWORDPREFSKEY);
        PlayerPrefs.DeleteKey(EMAILPREFSKEY);
        PlayerPrefs.DeleteKey("playfabid");

        
        FBManager.Instance.Logout();
        //PlayFabClientAPI.ForgetAllCredentials();
        isLoggedIn = false;
        GameData.data.Logout();
        //PlayerPrefs.DeleteAll();
    }



    ///Login With Facebook
      public void LoginWithFacebook(Callback callback)
    {
        FBLoginCallback = callback;
        FBManager.Instance.Login();
    }
void OnFacebookLogin()
    {
        //var request = new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString };
       // PlayFabClientAPI.LoginWithFacebook(request, OnLoginSuccess, OnLoginFailure);

    }
    void OnFacebookLoginFail()
    {
        if (FBLoginCallback != null)
            FBLoginCallback(false, "fail");
    }

    //private void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
    private void OnLoginSuccess(string result)
    {
       
        //PlayFabID = result.PlayFabId;
        string savedId = GetSavedPlayfabID();
        Debug.Log("PlayFab Login Successful!");
        if (PlayFabID != savedId && savedId != "newuser")
        {
            GameData.DeletePrefData();
        }
        SevePlayfabID();
        isLoggedIn = true;
        UpdatePlayerName();
        if (FBLoginCallback != null)
            FBLoginCallback(true,"success");

        SetLoginPrefs(2, "", "");

        GetData();

        UpdateFriends();


        LoadWorldLeaderboard();
        LoadFriendsLeaderboard();
    }
    void UpdatePlayerName()
    {
        PlayerGameName = FBManager.Instance.playerName;
        //PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() { DisplayName = PlayerGameName }, null, null);
    }

    void SevePlayfabID()
    {
        PlayerPrefs.SetString("playfabid", PlayFabID);
    }
    string GetSavedPlayfabID()
    {
        return PlayerPrefs.GetString("playfabid", "newuser");
    }
    private void OnLoginFailure(PlayFabError error)
    {
        OnFacebookLoginFail();
        isLoggedIn = false;
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        //Debug.LogError(error.GenerateErrorReport());
    }


    public void LoadWorldLeaderboard()
    {
        //PlayFabClientAPI.GetLeaderboard(new PlayFab.ClientModels.GetLeaderboardRequest()
        //{
        //    StatisticName = LEADERBOARDNAME,
        //    StartPosition = 0,
        //    MaxResultsCount = 50
        //}
        //, GetResult =>
        //{
        //  //  Debug.Log(GetResult.Leaderboard.Count);
        //    GameObject.FindObjectOfType<MenusUI>().leaderBoard.GetComponent<LeaderboardMenuListner>().worldScores.UpdateBoard(GetResult);
        //}
        //, DisplayPlayFabError);
    }
    public void LoadFriendsLeaderboard()
    {
        //PlayFabClientAPI.GetFriendLeaderboard(new PlayFab.ClientModels.GetFriendLeaderboardRequest()
        //{
        //    StatisticName = LEADERBOARDNAME,
        //    IncludeFacebookFriends = true,
        //    StartPosition = 0,
        //    MaxResultsCount = 50
        //}
        //, GetResult =>
        //{
        //    Debug.Log("Friends Leaderboard: " + GetResult.Leaderboard.Count);

            
        //    GameObject.FindObjectOfType<MenusUI>().leaderBoard.GetComponent<LeaderboardMenuListner>().friendsScores.UpdateBoard(GetResult);

        //}
        //, DisplayPlayFabError);
    }

    void DisplayPlayFabError(PlayFabError error)
    {
       // Debug.Log(error.GenerateErrorReport());
    }

    public void PushData()
    {
        //if (!isLoggedIn)
        //    return;
        //PlayFabClientAPI.UpdateUserData(new PlayFab.ClientModels.UpdateUserDataRequest()
        //{
        //    Data = new Dictionary<string, string>()
        //     {
        //         {"data",GameData.data.ToJson() }
        //     }
        //}, res=> {

        //    if (GameObject.FindObjectOfType<MenusUI>() != null)
        //    {
        //        LoadFriendsLeaderboard();
        //        LoadWorldLeaderboard();
        //    }
        
        //},err=> { });
    }

    public void GetData()
    {
        //PlayFabClientAPI.GetUserData(new PlayFab.ClientModels.GetUserDataRequest()
        //{
        //    PlayFabId = PlayFabID,
        //    Keys = new List<string>() { "data" }
        //}, GetResult =>
        //{
        //    Debug.Log("Got user data: " + GetResult.Data.Count);

        //    if (GetResult.Data != null)
        //    {
        //        if (GetResult.Data.Count <= 0)
        //        {
        //            Debug.LogError("User Data Not Found, init UserData.");
        //            return;
        //        }
        //        else
        //        {
        //            if (GetResult.Data.ContainsKey("data"))
        //            {
        //                GameData.data.Load(GetResult.Data["data"].Value);
        //            }

        //        }
        //    }
        //}, (error) =>
        //{
        //    Debug.Log(error.GenerateErrorReport());
        //    Debug.Log("Got error retrieving user data:");
        //});

    }

    public void SetPlayerStatistics()
    {
        //if (!isLoggedIn)
        //    return;
        //PlayFabClientAPI.UpdatePlayerStatistics(new PlayFab.ClientModels.UpdatePlayerStatisticsRequest
        //{
        //    // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
        //    Statistics = new List<PlayFab.ClientModels.StatisticUpdate> {
        //    new PlayFab.ClientModels.StatisticUpdate { StatisticName = LEADERBOARDNAME, Value = GameData.data.score },
        //    }
        //},
        //result =>
        //{
        //},
        //error => { Debug.LogError(error.GenerateErrorReport()); });
    }

}