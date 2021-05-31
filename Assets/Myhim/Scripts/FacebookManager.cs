using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine.UI;
//using AssemblyCSharp;
using UnityEngine.SceneManagement;
using System;
using System.Text.RegularExpressions;
using Facebook.Unity;
//using PlayFab;
//using PlayFab.ClientModels;




public class FacebookManager : MonoBehaviour {

	public static FacebookManager instance = null;
	private bool LoggedIn = false;
	private bool callDeepLinkOnce = true;
	public GameObject splashCanvas;
	public GameObject loginCanvas;

	// Use this for initialization
	void Start () {
		
	}
	void Awake()
	{
		
		Debug.Log ("FBManager awake");

		if (instance == null) {
			instance = this;
		} else {
			DestroyImmediate (instance.gameObject);
			instance = this;
		}

		DontDestroyOnLoad (transform.gameObject);

		if (!GameManager.Instance.logged) {

			#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
			} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
			initSession();
			}
			#elif (UNITY_EDITOR) // BY UZAIR AFTER R5...
			if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback, OnHideUnity);
			} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
			initSession ();
			}
			#else// BY UZAIR AFTER R5...
			initSession();
			#endif

			GameManager.Instance.logged = true;
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp ();
			// Continue with Facebook SDK
			// ...

			//			if (PlayerPrefs.GetString ("LoggedType").Equals ("Facebook")) {
			//				Debug.Log ("Already logged to facebook!!!!");
			initSession ();
			//			}

		} else {
			Debug.Log ("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	private void initSession ()
	{
		Debug.Log ("FbManager init session");
		InitCallbackForApplink ();//BY UZAIR DEEPLINKING...
		string logType = PlayerPrefs.GetString ("LoggedType");
		int autoLogin = PlayerPrefs.GetInt ("autoLogin");// BY UZAIR AFTER UI FOR LOGIN OFFLINE MODE...
		if (logType.Equals ("Facebook")) {
			//showLoadingCanvas ();// BY UZAIR AFTER R5... // commented on 27 may
			if (Facebook.Unity.AccessToken.CurrentAccessToken != null) {
				GameManager.Instance.facebookIDMy = Facebook.Unity.AccessToken.CurrentAccessToken.UserId;
				callApiToGetName ();
				//loginPlayFab ();
				//getMyProfilePicture (GameManager.Instance.facebookIDMy);
				LoggedIn = true;
			} else {
				//Debug.LogError ("FacebookErro Access Token is Null");
				FBLogin ();
			}
		} /*else if (logType.Equals ("Guest") && autoLogin == 1) {
			playFabManager.Login ();// BY UZAIR AFTER UI FOR LOGIN OFFLINE MODE...
			//Debug.LogError ("Auto Logging In...");
			PlayerPrefs.SetInt ("autoLogin", 0);
		} else if (logType.Equals ("EmailAccount")) {
			playFabManager.LoginWithEmailAccount ();
		}*/ // commented on 27 may

	}
	void loginPlayFab(){
	
		//FacebookAndPlayFabManager.Instance.playFabLogin();

	}
	public void FBLoginWithoutLink ()
	{
		//BY UZAIR FINAL...
		string logType = PlayerPrefs.GetString ("LoggedType");
		Debug.Log ("FB: " + logType);
		if (logType.Equals ("Guest")) {
			Debug.Log ("FB: " + logType);
			//Debug.LogError ("Guest Account Exists...Opening PopUp For Confirmation...");
			//GameManager.Instance.connectionLost.facebookConfirmationPanel.SetActive (true); // commented on 27 may
		} else {//BY UZAIR FINAL...
			//Debug.LogError ("Logging in to facebook");
			Debug.Log ("FB: Logging in to facebook");
			GameManager.Instance.LinkFbAccount = false;
			FBLogin ();
		}
	}

	#region Deep Linking

	//BY UZAIR DEEPLINKING....

//	public void getMyProfilePicture (string userID)
//	{
//
//		//FB.API("/" + userID + "/picture?type=square&height=92&width=92", Facebook.Unity.HttpMethod.GET, delegate(IGraphResult result) {
//		FB.API ("/me?fields=picture.width(200).height(200)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result) {
//			if (result.Error == null) {
//
//				// use texture
//
//				Dictionary<string, object> reqResult = Json.Deserialize (result.RawResult) as Dictionary<string, object>;
//
//				if (reqResult == null)
//					Debug.Log ("JEST NULL");
//				else
//					Debug.Log ("nie null");
//
//
//				GameManager.Instance.avatarMyUrl = ((reqResult ["picture"] as Dictionary<string, object>) ["data"] as Dictionary<string, object>) ["url"] as string;
//				Debug.Log ("My avatar " + GameManager.Instance.avatarMyUrl);
//
//				//StartCoroutine (loadImageMy (GameManager.Instance.avatarMyUrl)); // commented on 27 may
//
//				//GameManager.Instance.avatarMy = Sprite.Create(result.Texture, new Rect(0, 0, result.Texture.width, result.Texture.height), new Vector2(0.5f, 0.5f), 32);
//				//sHOW POP HERE TO CONFIRM WHEATEHR TO USE OLD ACCOUNT OR NEW...CHANGEHER
//
//
//
//				if (GameManager.Instance.LinkFbAccount) {
//					//playFabManager.LinkFacebookAccount ();
//				} else {
//					//playFabManager.LoginWithFacebook ();
//					//FacebookAndPlayFabManager.Instance.playFabLogin();
//				}
//			} else {
//				//Debug.LogError ("Facebook Error retreiving image: " + result.Error);
//				//GameManager.Instance.connectionLost.showDialog ();//BY UZAIR FINAL...  // commented on 27 may
//			}
//		});
//	}



	public void InitCallbackForApplink ()
	{
		if (FB.IsInitialized) {
			FB.GetAppLink (DeepLinkCallback);
		}
	}

	void CallDeepLinkOnceReset ()
	{
		callDeepLinkOnce = true;
	}

	void DeepLinkCallback (IAppLinkResult result)
	{
		if (callDeepLinkOnce) {
			callDeepLinkOnce = false;
			Invoke ("CallDeepLinkOnceReset", 10f);
			if (SceneManager.GetActiveScene ().name.Equals ("GameScene")) {

			} else {
				#if UNITY_ANDROID
				if (!string.IsNullOrEmpty (result.Url)) {
					Uri newURI = (new Uri (result.Url));

					var index = newURI.Query.IndexOf ("request_ids");
					if (index != -1) {
						Dictionary<string, string> nvc = new Dictionary<string, string> ();
						string url = result.Url;
						if (url.Contains ("?")) {
							url = url.Substring (url.IndexOf ('?') + 1);
						}
						foreach (string vp in Regex.Split(url, "&")) {
							string[] singlePair = Regex.Split (vp, "=");
							if (singlePair.Length == 2) {
								nvc.Add (singlePair [0], singlePair [1]);
							} else {
								// only one key with no value specified in query string
								nvc.Add (singlePair [0], string.Empty);
							}
						}
						string[] requesrIds = Regex.Split ((string)nvc ["request_ids"], "%2C");
						GlobalVariables.GetFacebookAppRequests ();
						if (!GlobalVariables.FacebookAppRequests.Contains (requesrIds [requesrIds.Length - 1])) {
							FB.API ("/" + requesrIds [requesrIds.Length - 1], HttpMethod.GET, JoinRoomAndUpdateUI);
							GlobalVariables.FacebookAppRequests.Add (requesrIds [requesrIds.Length - 1]);
							GlobalVariables.FacebookRequestIndex = GlobalVariables.FacebookAppRequests.Count - 1;
							GlobalVariables.SaveFacebookAppRequests ();
						} else {
							//GameManager.Instance.connectionLost.facebookRequestPopUp.SetActive (true); // commented on 27 may
						}

					}

				}
				#endif
				#if UNITY_IOS
				//if (!string.IsNullOrEmpty (result.Url)) {
				//if (result.Url.Contains ("request_ids")) {
				//string playerID = "";
				//string[] singlePair = Regex.Split (result.Url, "request_ids");
				//if (singlePair.Length == 2) {
				//string[] singlePair1 = Regex.Split (singlePair [1], "%");
				//string[] singlePair2 = Regex.Split (singlePair1 [1], "D");
				//playerID = singlePair2 [1];
				//}
				//GlobalVariables.GetFacebookAppRequests ();
				//if (!GlobalVariables.FacebookAppRequests.Contains (playerID)) {
				//FB.API ("/" + playerID, HttpMethod.GET, JoinRoomAndUpdateUI);
				//GlobalVariables.FacebookAppRequests.Add (playerID);
				//GlobalVariables.FacebookRequestIndex = GlobalVariables.FacebookAppRequests.Count - 1;
				//GlobalVariables.SaveFacebookAppRequests ();
				//} else {
				//GameManager.Instance.connectionLost.facebookRequestPopUp.SetActive (true);
				//}
				//} else {
				//Debug.Log ("003: No request index found");
				//}

				//}
				#endif
			}
		}
	}

	private void JoinRoomAndUpdateUI (IResult result)
	{
		if (result.Error == null) {
			if (SceneManager.GetActiveScene ().name.Equals ("LoginSplash") || (SceneManager.GetActiveScene ().name.Equals ("MenuScene"))) {
				string[] messageSplit = result.ResultDictionary ["data"].ToString ().Split (';');
				string invitor = messageSplit [0];
				string payout = messageSplit [1];
				string roomID = messageSplit [2];
				string type = messageSplit [3];
				GlobalVariables.INVITOR_NAME_DEEPLINKING = invitor;
				GlobalVariables.PAYOUTCOINS_DEEPLINKING = int.Parse (payout);
				GlobalVariables.ROOMID_DEEPLINKING = roomID;
				GlobalVariables.MODE_TYPE_DEEPLINKING = type;
				GlobalVariables.CAME_FROM_DEEPLINKING = true;
//				if ((SceneManager.GetActiveScene ().name.Equals ("MenuScene"))) {
//					GameManager.Instance.playfabManager.SendInGameFBInvition ();
//				} // commented on 27 may
			} else {
				GlobalVariables.CAME_FROM_DEEPLINKING = false;
				GlobalVariables.Reset_DeeplinkingVariables ();
			}
		} else {
			GlobalVariables.CAME_FROM_DEEPLINKING = false;
			GlobalVariables.Reset_DeeplinkingVariables ();
		}
	}


	void OnApplicationPause (bool pauseStatus)
	{
		if (!pauseStatus) {
			if ((SceneManager.GetActiveScene ().name.Equals ("MenuScene"))) {				
				if (FB.IsInitialized) {			
					FB.GetAppLink (DeepLinkCallback);
				}

			}

		}

	}

	//BY UZAIR DEEPLINKING....

	#endregion

	public void FBLogin ()
	{
		if (!LoggedIn) {
			var perms = new List<string> () { "public_profile", "email", "user_friends" };
			FB.LogInWithReadPermissions (perms, AuthCallback);
		} else {//BY UZAIR FINAL
			initSession ();
		}//BY UZAIR FINAL

	}

	private void AuthCallback (ILoginResult result)
	{
		//BY UZAIR FINAL...
		if (result.Error != null) {
			//Debug.LogError ("FacebookError : " + result.Error);
			//GameManager.Instance.connectionLost.showDialog (); // commented on 27 may
		} else {
			if (FB.IsLoggedIn) {
				// AccessToken class will have session details
				var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
				// Print current access token's User ID

				GameManager.Instance.facebookIDMy = aToken.UserId;
				// Print current access token's granted permissions
				Debug.Log (aToken.ToJson ());
				foreach (string perm in aToken.Permissions) {
					Debug.Log (perm);
				}

				//			getFacebookInvitableFriends ();
				//			getFacebookFriends ();
				//			callApiToGetName ();
				//			getMyProfilePicture (GameManager.Instance.facebookIDMy);
				//
				//
				//			LoggedIn = true;
				//
				//			GameObject.Find ("FbLoginButtonText").GetComponent <Text>().text = "Play";

				PlayerPrefs.SetString ("LoggedType", "Facebook");
				PlayerPrefs.Save ();

//				if (!GameManager.Instance.LinkFbAccount) {
//					loginCanvas.SetActive (false);
//					splashCanvas.SetActive (true);
//
//				} // commented on 27 may

				initSession ();
			} /*else {
				if (GameManager.Instance.Loader != null)//BY UZAIR AFTER R5 ClientFeedback...
					GameManager.Instance.Loader.SetActive (false);//BY UZAIR AFTER R5 ClientFeedback...
				if (facebookLoginButton != null) {// BY UZAIR AFTER R5...
					facebookLoginButton.GetComponent<Button> ().interactable = true;
					guestLoginButton.GetComponent<Button> ().interactable = true;
					offlineLoginButton.GetComponent<Button> ().interactable = true;
					Debug.Log ("User cancelled login");
				}
			}*/ // commented on 27 may
		}//BY UZAIR FINAL...
	}

	private void callApiToGetName ()
	{
		FB.API ("me?fields=name", Facebook.Unity.HttpMethod.GET, APICallbackName);
	}

	void APICallbackName (IResult response)
	{
		try {
			GameManager.Instance.nameMy = response.ResultDictionary ["name"].ToString ();
			Debug.Log ("My name " + GameManager.Instance.nameMy);
		} catch (Exception ex) {
			//Debug.LogError ("ISSUE IN APICallbackName : " + ex.Message + " || " + ex.StackTrace);
			//GameManager.Instance.connectionLost.showDialog (); // Commented on 27 may
		}
		SceneManager.LoadScene ("02_LeaderboardMenu");
	}

	public void showLoadingCanvas ()
	{
		//BY UZAIR AFTER REJOIN QA...
		if (loginCanvas == null) {
			loginCanvas = GameObject.FindGameObjectWithTag ("LoginCanvas");
		}

		if (splashCanvas == null) {
			splashCanvas = GameObject.FindGameObjectWithTag ("SplashCanvas");
		}

		if (loginCanvas != null) {// BY UZAIR AFTER R5...
			loginCanvas.SetActive (false);
			if (splashCanvas != null)
				splashCanvas.SetActive (true);
		}
		//BY UZAIR AFTER REJOIN QA...
	}

	public void hideLoadingCanvas ()
	{
		//BY UZAIR AFTER REJOIN QA...
		if (loginCanvas == null) {
			loginCanvas = GameObject.FindGameObjectWithTag ("LoginCanvas");
		}

		if (splashCanvas == null) {
			splashCanvas = GameObject.FindGameObjectWithTag ("SplashCanvas");
		}


		if (loginCanvas != null) {// BY UZAIR AFTER R5...
			loginCanvas.SetActive (true);
			if (splashCanvas != null)
				splashCanvas.SetActive (false);
		}
		//BY UZAIR AFTER REJOIN QA...
	}


	// Update is called once per frame
	void Update () {
		
	}
}
