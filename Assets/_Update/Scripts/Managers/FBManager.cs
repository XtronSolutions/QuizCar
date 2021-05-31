using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System;
using UnityEngine.UI;
public class FBManager : SingeltonBase<FBManager> {
    public delegate void callback();
    public callback OnFacebookLoginSuccessful;
    public callback OnFacebookInitialized;
    public callback OnFacebookLoginFail;
    public string playerName;
    public string facebookID;
    public Texture2D profileImage;
    public static bool IsLoggedIn;
 
    public List<Friend> friendsList = new List<Friend>();

  //  public GameObject startingLoadingText;
    // Start is called before the first frame update
    void Start()
    {
       // profileImage = new Texture2D(200, 200);

        FB.Init(this.OnInitComplete, this.OnHideUnity);
    }

    private void OnInitComplete()
    {
        string logMessage = string.Format(
                                "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'",
                                FB.IsLoggedIn,
                                FB.IsInitialized);
        Debug.Log(logMessage);
        if (AccessToken.CurrentAccessToken != null)
        {
            Debug.Log(AccessToken.CurrentAccessToken.ToString());
        }

        FB.ActivateApp();

        if (FB.IsLoggedIn)
        {

            Debug.Log("Login");
            if (OnFacebookInitialized != null)
            {
                OnFacebookInitialized();
            }
            if (OnFacebookLoginSuccessful != null)
                OnFacebookLoginSuccessful();

            GetUserFacebookDetail();

            FetchFriendsList();
            GameObject.FindObjectOfType<MenuManager>().OpenMainMenuScreen();
          //  startingLoadingText.SetActive(false);
        }
        else
        {
            PlayfabManager.Instance.CheckLogin(CheckLogin);
        }
        
    }
    void CheckLogin(bool status, string msg)
    {
      //  startingLoadingText.SetActive(false);

        //if(status)
        //    GameObject.FindObjectOfType<MenuManager>().OpenMainMenuScreen();
        //else
        //    GameObject.FindObjectOfType<MenuManager>().OpenLoginScreen(); 
    }
    private void OnHideUnity(bool isGameShown)
    {
        // Debug.Log("Is game shown: " + isGameShown);
    }
    // Update is called once per frame
    void Update()
    {
        IsLoggedIn = FB.IsLoggedIn;
    }

    public void Login()
    {

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("No internet.");
            return;
        }
        Debug.Log("Connecting...");
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "user_friends" }, this.HandleResult);
    }
    public void Logout()
    {
        FB.LogOut();
    }

    void HandleResult(IResult result)
    {

        var Status = "";
        var LastResponse = "";

        if (result == null)
        {
            LastResponse = "Null Response\n";
            Debug.Log(LastResponse);
            if (OnFacebookLoginFail != null)
                OnFacebookLoginFail();
            return;
        }

        // Some platforms return the empty string instead of null.
        if (!string.IsNullOrEmpty(result.Error))
        {
            Status = "Error - Check log for details";
            LastResponse = "Error Response:\n" + result.Error;
            if (OnFacebookLoginFail != null)
                OnFacebookLoginFail();
        }
        else if (result.Cancelled)
        {
            Status = "Cancelled - Check log for details";
            LastResponse = "Cancelled Response:\n" + result.RawResult;
            if (OnFacebookLoginFail != null)
                OnFacebookLoginFail();
        }
        else if (!string.IsNullOrEmpty(result.RawResult))
        {
            Status = "Success - Check log for details";
            LastResponse = "Success Response:\n" + result.RawResult;

            if (OnFacebookLoginSuccessful != null)
                OnFacebookLoginSuccessful();

            GetUserFacebookDetail();

            FetchFriendsList();


            PlayerPrefs.SetString("login", "true");
            PlayerPrefs.Save();
        }
        else
        {
            LastResponse = "Empty Response\n";
            if (OnFacebookLoginFail != null)
                OnFacebookLoginFail();
        }

        Debug.Log(result.ToString());
        Debug.Log(Status);
        Debug.Log(LastResponse);
    }

    void GetUserFacebookDetail()
    {
        FB.API("/me?fields=id,name", HttpMethod.GET, delegate (IGraphResult httpResult)
        {
            if (httpResult.Error == null)
            {
                facebookID = "" + httpResult.ResultDictionary["id"];
                playerName = "" + httpResult.ResultDictionary["name"];
                PlayfabManager.PlayerID = facebookID;
                PlayfabManager.PlayerGameName = playerName;
                PlayfabManager.PlayerName = playerName;
                GetFacebookUserPictureFromUrl("me", profileImage);
            }
            else
            {
                Debug.LogError(httpResult.Error);
            }
        });
       // Debug.Log(facebookID);
       
    }


    void FetchFriendsList()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            JSONObject data = JSONObject.Create(result.RawResult).GetField("data");
            for (int i = 0; i < data.Count; i++)
            {
                string id = data[i].GetField("id").ToString().Replace("\"", "");
                Friend f = new Friend(id);
                friendsList.Add(f);
            }
        });
    }

    public void InviteFriends()
    {
        if (IsLoggedIn)
        {
            // FB.AppRequest("Hey! Come and play this awesome game!", null, null, null, null, null, "Lets play together, come join me!", InviteCallback);
            FB.AppRequest("Message", null, null, null, null, null, "Message", InviteCallback);

        }
        else
        {
            Login();
        }
    }
    void InviteCallback(IAppRequestResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Invite cancelled.");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Invite error:" + result.Error);
        }
        else
        {
            Debug.Log("Invite was successful:" + result.RawResult);
           // MenuManager.Instance.LeaderboardMenu.GetComponent<LeaderboardMenuListner>().InvitedSuccessfully();
        }
    }
    public void Share()
    {
        if (IsLoggedIn)
        {
            // FB.AppRequest("Hey! Come and play this awesome game!", null, null, null, null, null, "Lets play together, come join me!", InviteCallback);
            FB.ShareLink(new System.Uri("https://play.google.com/store/apps/details?id=com.creativeartz.llamarunner"), "This is title", "This is description", null, ShareCallback);

        }
        else
        {
            Login();
        }
    }
    void ShareCallback(IShareResult result)
    {
        if (result.Cancelled)
        {
            Debug.Log("Share cancelled.");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Share error:" + result.Error);
        }
        else
        {
            Debug.Log("Share was successful:" + result.RawResult);
           // MenuManager.Instance.LeaderboardMenu.GetComponent<LeaderboardMenuListner>().SharedSuccessfully();
        }
    }



    public void GetFacebookUserPictureFromUrl(string id, Texture2D texture)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}", id, 100, 100);
        Debug.Log(query);
        FB.API(query, HttpMethod.GET,
            (res =>
            {
                Debug.Log("res");
                if (ValidateResult(res))
                {
                    Debug.Log("res2");
                    profileImage = res.Texture;
                    //StartCoroutine(GetTextureFromGraphResult(res, texture));
                }
            
            }));
    }
    public void GetFacebookUserPicture(string id, int width, int height, Action<IGraphResult> successCallback = null, Action<IGraphResult> errorCallback = null)
    {
        string query = string.Format("/{0}/picture?type=square&height={1}&width={2}", id, height, width);

        FB.API(query, HttpMethod.GET,
            (res =>
            {
                if (!ValidateResult(res) || res.Texture == null)
                {
                    if (errorCallback != null)
                        errorCallback(res);

                    return;
                }

                if (successCallback != null)
                    successCallback(res);

                Debug.Log("FBManager.GetFacebookUserPicture => Success!");
            }));
    }

    public IEnumerator GetTextureFromGraphResult(IGraphResult result, Texture2D texture)
    {
        profileImage = result.Texture;
     //   texture = result.Texture;

        Debug.Log(result.RawResult);
        var data = result.ResultDictionary["data"] as IDictionary<string, object>;
        var url = data["url"].ToString();
        Debug.Log("res3");
        WWW request = new WWW(url);

        yield return new WaitUntil(() => request.isDone);
        if (request.texture != null)
        {
            texture = request.texture;
            Debug.Log("res4");
        }
        Debug.Log("res5");
    }
    private bool ValidateResult(IResult result)
    {
        if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
            return true;

        Debug.LogError(string.Format("{0} is invalid (Cancelled={1}, Error={2}, JSON={3})",
            result.GetType(), result.Cancelled, result.Error, Facebook.MiniJSON.Json.Serialize(result.ResultDictionary)));

        return false;
    }
    public class Friend
    {
        public string Name;
        public string facebookID;
        public string playfabID;
        public string score;
        public string rank;

        public Friend(string id)
        {
            FB.API("/" + id + "?fields=name", HttpMethod.GET, delegate (IGraphResult httpResult)
            {
                if (httpResult.Error == null)
                {
                    facebookID = "" + httpResult.ResultDictionary["id"];
                    Name = "" + httpResult.ResultDictionary["name"];
                }
                else
                {
                    Debug.LogError(httpResult.Error);
                }
            });
        }
    }
}