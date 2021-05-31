/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Photon.Chat;
using Photon;
using Photon.Pun;
using System.Linq;

public class FaceBookFriends : MonoBehaviour, IChatClientListener {

	public ChatClient chatClient;
	private List<GameObject> friendsObjects = new List<GameObject>();
	public string authToken;

	[SerializeField] private FacebookFriendEntry _entryPrefab;
	[SerializeField] private Transform _facebookFriendEntryParent;
	[SerializeField] private GameObject InvitePanel;
	[SerializeField] private GameObject InvitationDialogue;
	// Use this for initialization
	public string code;
	void Awake(){

		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	void OnDestroy ()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public static FaceBookFriends Instance;

	void Start () {

		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {

		if (chatClient != null) {
			chatClient.Service ();
		}
		if (chatClient != null) {
			if (chatClient.State == ChatState.Disconnected) {
				Debug.Log ("chat is not connected ");
			}
		}
	}

	private void OnSceneLoaded (Scene _scene, LoadSceneMode _mode)
	{
		Debug.Log ("Scene Loaded :: " + _scene.name);

		if (_scene.name.Equals ("MainMenuScene")) {
			
			if (chatClient != null) {		

				chatClient.SetOnlineStatus (ChatUserStatus.Online);
			}
				
		} 
	}



	public void getPlayerDataRequest ()
	{
		Debug.Log ("Get player data request!!");
		GetUserDataRequest getdatarequest = new GetUserDataRequest () {
			PlayFabId = FacebookAndPlayFabManager.Instance.PlayFabTitleId,
		};

		PlayFabClientAPI.GetUserData (getdatarequest, (result) => {

			Dictionary<string, UserDataRecord> data = result.Data;
			//GameManager.Instance.myPlayerData = new MyPlayerData (data, true);//commnted on 30 may

			// BY UZAIR==================================================================================================================
			if (GlobalVariables.isLastTimeOfflineMode ()) {
				Debug.Log ("Syncing Offline Data");
				//Dictionary<string,string> _data = GlobalVariables.LoadPlayerPrefSync (); //commnted on 30 may
				//GameManager.Instance.myPlayerData.SyncOfflineUserData (_data);//commnted on 30 may
			}
			// BY UZAIR==================================================================================================================
			GlobalVariables.setLastPlayedMode (false);//BY UZAIR


			Debug.Log ("Get player data request finish!!");
			//			StartCoroutine (loadSceneMenu ()); //commnted on 30 may
		}, (error) => {
			Debug.Log ("Data updated error " + error.ErrorMessage);
		}, null);
	}

	public void connectToChat ()
	{
		Debug.Log ("Connecting to chat");
		chatClient = new ChatClient (this);
		chatClient.DebugOut = DebugLevel.ALL;
		GameManager.Instance.chatClient = chatClient;
		Photon.Chat.AuthenticationValues authValues = new Photon.Chat.AuthenticationValues ();
		authValues.UserId = FacebookAndPlayFabManager.Instance.FacebookUserId;
		Debug.Log ("FacebookAndPlayFabManager.Instance.PlayFabUserId " + FacebookAndPlayFabManager.Instance.FacebookUserId);
		chatClient.Connect (Constants.PhotonChatID, "1.0", new Photon.Chat.AuthenticationValues (FacebookAndPlayFabManager.Instance.FacebookUserId));
		//getFacebookFriends ();

	}
		
	void OnPlayFabError (PlayFabError error)
	{
		Debug.Log ("Playfab Error: " + error.ErrorMessage);
	}

	List<string> friendNames = new List<string> ();
	List<string> friendIDs = new List<string> ();
	List<string> friendAvatars = new List<string> ();
	public void getFacebookInvitableFriends (Transform parentTransform, GameObject spinner, GameObject Panel)
	{
		_facebookFriendEntryParent = parentTransform;
		ClearPreviousFriendList ();
		//&fields=picture.width(100).height(100)
		FB.API ("/me/friends?limit=5000&fields=id,name,picture.width(117).height(112)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result) {

			if (result.Error == null) {
				InvitePanel = Panel;
				List<string> friendsNames = new List<string> ();
				List<string> friendsIDs = new List<string> ();
				List<string> friendsAvatars = new List<string> ();
				//Grab all requests in the form of a dictionary. 
				chatClient.RemoveFriends(GameManager.Instance.friendsIDForStatus.ToArray ());
				Dictionary<string, object> reqResult = Json.Deserialize (result.RawResult) as Dictionary<string, object>;
				//Grab 'data' and put it in a list of objects. 


				List<object> newObj = reqResult ["data"] as List<object>;
				//For every item in newObj is a separate request, so iterate on each of them separately. 


				Debug.Log ("Invitable Friends Count: " + newObj.Count);
				for (int xx = 0; xx < newObj.Count; xx++) {
					Dictionary<string, object> reqConvert = newObj [xx] as Dictionary<string, object>;

					string name = reqConvert ["name"] as string;
					string id = reqConvert ["id"] as string;

					friendsIDs.Add(id);

					Dictionary<string, object> avatarDict = reqConvert ["picture"] as Dictionary<string, object>;
					avatarDict = avatarDict ["data"] as Dictionary<string, object>;

					string avatarUrl = avatarDict ["url"] as string;

					AddFacebookFriend (name, id, avatarUrl);

				}

				GameManager.Instance.friendsIDForStatus = friendsIDs;
				chatClient.AddFriends (friendsIDs.ToArray ());

			} else {
				Debug.Log ("Something went wrong. " + result.Error + "  " + result.ToString ());
			}
			spinner.SetActive(false);

		});
	}
		
	public void ClearPreviousFriendList()
	{
		for (int i = 0; i < _facebookFriendEntryParent.childCount; i++)
		{
			Destroy(_facebookFriendEntryParent.GetChild(i).gameObject);

		}
	}

	public void DisableInvitePanel(GameObject Panel){
	
		Panel.SetActive (false);
	}


	public void InviteFriend (string i , string name)
	{
		Debug.Log ("Chat: Send Message");
		if (chatClient.State == ChatState.Disconnected) {
			connectToChat ();
			Debug.Log ("chat is disconnected and trying to connect");
		} else {
			Debug.Log ("chat is connected");
			chatClient.SendPrivateMessage (i, name+ ";" + code);
		}
	}



	void AddFacebookFriend(string friendsNames, string friendsIDs, string friendsAvatars){

		FacebookFriendEntry entry = Instantiate(_entryPrefab.gameObject, _facebookFriendEntryParent).GetComponent<FacebookFriendEntry>();

		int width = 117;
		int height = 112;

		entry.SetUserName(friendsNames);
		entry.SetUserPictureSprite(friendsAvatars);
		entry.SetUserID (friendsIDs);


		string friendID = friendsIDs;
		Debug.Log ("friendID: " + friendID);
		Debug.Log ("friendName: " + friendsNames);
		entry.transform.Find ("InviteButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		entry.transform.Find ("InviteButton").GetComponent<Button> ().onClick.AddListener (() => InviteFriend (friendID, FacebookAndPlayFabManager.Instance.FacebookUserName));
		entry.transform.Find ("InviteButton").GetComponent<Button> ().onClick.AddListener (() => DisableInvitePanel (InvitePanel));

		GameManager.Instance.friendsObjects.Add (entry.gameObject);


		for (int j = 0; j < GameManager.Instance.friendsStatuses.Count; j++) {

			string[] friend1 = GameManager.Instance.friendsStatuses [j];
			Debug.Log (friendID + "  " + friend1 [0]);
			if (friend1 [0].Equals (friendID)) {
				Debug.Log ("Found FRIEND");
				if (friend1 [1].Equals ("" + ChatUserStatus.Online))
					entry.updateFriendStatus (ChatUserStatus.Online, friendID);
				break;
			}
		}
	}
		


	// override mwthods

	public void OnChatStateChange (ChatState state)
	{
		
	}

	public void OnGetMessages (string channelName, string[] senders, object[] messages)
	{

	}

	public void OnUnsubscribed (string[] channels)
	{

	}
	public void OnSubscribed (string[] channels, bool[] results)
	{
		//Debug.LogError ("Subscribed to CHAT - set online status!");
		chatClient.SetOnlineStatus (ChatUserStatus.Online);

	}

	void IChatClientListener.OnStatusUpdate (string user, int status, bool gotMessage, object message)
	{
		Debug.Log ("OnStatusupdate called");
		Debug.Log (GameManager.Instance.friendsStatuses.Count);
		if (status == ChatUserStatus.Online) {
			bool foundFriend = false;


			for (int i = 0; i < GameManager.Instance.friendsStatuses.Count; i++) {
				string[] friend = GameManager.Instance.friendsStatuses [i];
				if (friend [0].Equals (user)) {
					GameManager.Instance.friendsStatuses [i] [1] = "" + status;
					foundFriend = true;
					break;
				}
			}

			if (!foundFriend) {
				GameManager.Instance.friendsStatuses.Add (new string[] { user, "" + status });
			}


			Debug.Log ("GameManager.Instance.friendsObjects.ToArray ().Length " + GameManager.Instance.friendsObjects.ToArray ().Length);

			if (GameManager.Instance.friendsObjects != null) {

				foreach (var friend in GameManager.Instance.friendsObjects) {

					friend.GetComponent<FacebookFriendEntry> ().updateFriendStatus (status, user);
				}
			}
		} else {
			if (GameManager.Instance.friendsObjects != null) {

				foreach (var friend in GameManager.Instance.friendsObjects) {

					friend.GetComponent<FacebookFriendEntry> ().updateFriendStatus (status, user);
				}
			}
		}

	}

	public void OnUserUnsubscribed(string channel, string user)
	{


	}

 void IChatClientListener.OnDisconnected ()
	{
		Debug.Log ("Chat disconnected - Reconnect");
		connectToChat ();
	}

 void IChatClientListener.OnConnected ()
	{
		Debug.LogError ("Photon Chat connected!!!");
		chatClient.Subscribe (new string[] { "invitationsChannel" });
	}

	 void IChatClientListener.OnPrivateMessage (string sender, object message, string channelName)
	{
		Debug.Log ("Sender " + sender);
		Debug.Log ("FacebookUserId " + FacebookAndPlayFabManager.Instance.FacebookUserId);
		if (!sender.Equals (FacebookAndPlayFabManager.Instance.FacebookUserId)){
			
			GameManager.Instance.invitationID = sender;
			GameObject Invitation = Instantiate (InvitationDialogue.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			Invitation.GetComponent<PhotonChatListner> ().showInvitationDialog (message.ToString(),sender,"");
		}

		if ((GameManager.Instance.invitationID.Length == 0 || !GameManager.Instance.invitationID.Equals (sender))) {

		} else {
			GameManager.Instance.invitationID = "";
		}
	}

	public void OnUserSubscribed(string channel, string user)
	{

	}

	public void DebugReturn (DebugLevel level, string message)
	{
		switch (level) {
		case DebugLevel.ERROR:
			//Debug.LogError (message);
			break;
		case DebugLevel.WARNING:
			Debug.LogWarning (message);
			break;
		case DebugLevel.INFO:
			Debug.Log (message);
			break;
		}

	}

	//Unused


	//	void AddFriend(){
	//
	//		GameObject entry = Instantiate(_entryPrefab.gameObject, null);
	//		GameManager.Instance.friendsObjects.Add (entry.gameObject);
	//
	//	}


	//	public void getFacebookInvitableFriends (Transform parentTransform, GameObject spinner)
	//	{
	//		_facebookFriendEntryParent = parentTransform;
	//		ClearPreviousFriendList ();
	//
	//
	//		string[] frndNam = friendNames.ToArray();
	//		string[] frndId = friendIDs.ToArray();
	//		string[] frndPic = friendAvatars.ToArray();
	//
	//		for (int i = 0; i < friendNames.ToArray ().Length; i++) {
	//
	//			AddFacebookFriend (frndNam[i], frndId[i], frndPic[i]);
	//		}
	//		spinner.SetActive (false);
	//	}

	public void InviteFriend (string i)
	{
		Debug.Log ("" + i);
		List<string> to = new List<string> ();
		to.Add (i);
		FB.AppRequest (
			Constants.facebookInviteMessage, to, null, null, null, null, null,
			delegate (IAppRequestResult result) {
				Debug.Log ("RESULT: " + "Cancelled - " + result.Cancelled);
				Debug.Log ("REQUEST RESULT: " + result.RawResult);
			}
		);

	}


	public void showFriends (List<string> friendsNames, List<string> friendsIDs, List<string> friendsAvatars)
	{

		//		loaderCanvas.SetActive (false);//BY UZAIR AFTER R5...
		//		friendsMenu.gameObject.SetActive (true);
		//mainMenu.gameObject.SetActive(false);

		//Debug.LogError ("Here Friends");

		if (friendsNames != null) {
			for (int i = 0; i < friendsNames.Count; i++) {


				GameObject friend = Instantiate (_entryPrefab.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				FacebookFriendEntry entry = friend.GetComponent<FacebookFriendEntry> ();

				string name = friendsNames [i];
				if (friendsNames [i].Length > 13) {
					name = friendsNames [i].Substring (0, 12) + "...";
				}

				friend.transform.Find ("FriendName").GetComponent<Text> ().text = name;

				string friendID = friendsIDs [i];

				friend.transform.Find ("InviteButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
				friend.transform.Find ("InviteButton").GetComponent<Button> ().onClick.AddListener (() => InviteFriend (friendID));


				entry.SetUserPictureSprite(friendsAvatars[i]);


				friend.transform.parent = _facebookFriendEntryParent;
				friend.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);

				friendsObjects.Add (friend);
				Debug.Log ("KUPA");
				//				for (int j = 0; j < GameManager.Instance.friendsStatuses.Count; j++) {
				//
				//					string[] friend1 = GameManager.Instance.friendsStatuses [j];
				//					Debug.Log (friendID + "  " + friend1 [0]);
				//					if (friend1 [0].Equals (friendID)) {
				//						Debug.Log ("Found FRIEND");
				//						if (friend1 [1].Equals ("" + ChatUserStatus.Online))
				//							GameManager.Instance.facebookFriendsMenu.updateFriendStatus (ChatUserStatus.Online, friendID);
				//						break;
				//					}
				//				}

			}
		}


		// GameObject.Find("FriendsMask").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
		// GameObject.Find("FriendsMask").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1);

	}

	public void addPlayFabFriends (List<string> playfabIDs, List<string> playfabFBName, List<string> playfabFBID)
	{
		//		playersAvatars = GameObject.Find ("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController> ().avatars;

		//		friendsObjects = new List<GameObject> ();
		//
		//		loaderCanvas.SetActive (false);//BY UZAIR AFTER R5...
		//		friendsMenu.gameObject.SetActive (true);
		//		mainMenu.gameObject.SetActive(false);
		//
		//
		//		for (int i = 0; i < playfabIDs.Count; i++) {
		//
		//
		//			GameObject friend = Instantiate (friendPrefab2, Vector3.zero, Quaternion.identity) as GameObject;
		//			string name = playfabFBName [i];
		//			if (playfabFBName [i].Length > 13) {
		//				name = playfabFBName [i].Substring (0, 12) + "...";
		//			}
		//			friend.transform.Find ("FriendName").GetComponent<Text> ().text = name;
		//			//friend.transform.Find("FriendName").GetComponent<Text>().text = playfabFBName[i];
		//
		//			string friendName = playfabFBName [i];
		//
		//			string friendID = playfabIDs [i];
		//
		//			string friendFBID = playfabFBID [i];//BY UZAIR DEEPLINKING...
		//
		//			friend.GetComponent<PlayFabFriendScript> ().playfabID = friendID;
		//			Debug.Log ("ADD LISTENER");
		//			friend.transform.Find ("InviteFriendButton").GetComponent<Button> ().onClick.RemoveAllListeners ();
		//			friend.transform.Find ("DeleteFriend").GetComponent<Button> ().onClick.RemoveAllListeners ();
		//			friend.transform.Find ("InviteFriendFBButton").GetComponent<Button> ().onClick.RemoveAllListeners ();//BY UZAIR DEEPLINKING...
		//			friend.transform.Find ("InviteFriendButton").GetComponent<Button> ().onClick.AddListener (() => ChallengeFriend (friendID));
		//			friend.transform.Find ("DeleteFriend").GetComponent<Button> ().onClick.AddListener (() => RemoveFriend (friendID, friendName, friend));
		//			// Debug.LogError("Yeah Sucker");
		//			friend.transform.Find ("InviteFriendFBButton").GetComponent<Button> ().onClick.AddListener (() => ChallengeFriendFB (friendFBID));
		//			friend.transform.Find ("InviteFriendFBButton").gameObject.SetActive (false);
		//			//
		//			//			friend.GetComponent<MonoBehaviour> ().StartCoroutine (
		//			//			if(!playfabFBID[i].Equals ("NoID"))
		//			//				loadImageFBID (playfabFBID [i], friend.transform.Find ("FriendAvatar").GetComponent <Image> ());
		//			//			);
		//
		//			getFriendImageUrl (friendID, friend.transform.Find ("Avatar/FriendAvatar").GetComponent<Image> (), friend.transform.Find ("Avatar/FriendAvatar").gameObject);
		//
		//			//Debug.LogError ("Facebook ID : "+playfabFBID[i]);
		//
		//
		//
		//			friend.transform.parent = listBuddies.transform;
		//			friend.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		//
		//			friendsObjects.Add (friend);
		//
		//			if (playfabFBName [i].Length < 1) {
		//				friendsObjects [i].SetActive (false);
		//			}
		//
		//			if (!playfabFBID [i].Equals ("Null")) {
		//				AddFacebookFriendInGame (friendName, friendID, playfabFBID [i]);//BY UZAIR DEEPLINKING....
		//			}
		//
		//		}
	}

	public void GetPlayfabFriends ()
	{
		Debug.Log ("IND");
		GetFriendsListRequest request = new GetFriendsListRequest ();
		request.IncludeFacebookFriends = true;
		PlayFabClientAPI.GetFriendsList (request, (result) => {

			Debug.Log ("Friends list Playfab: " + result.Friends.Count);
			var friends = result.Friends;

			List<string> playfabFriends = new List<string> ();
			List<string> playfabFriendsName = new List<string> ();
			List<string> playfabFriendsFacebookId = new List<string> ();


			chatClient.RemoveFriends (GameManager.Instance.friendsIDForStatus.ToArray ());

			List<string> friendsToStatus = new List<string> ();


			int index = 0;
			int indfb = 0;
			foreach (var friend in friends) {

				playfabFriends.Add (friend.FriendPlayFabId);
				Debug.Log("friend.FriendPlayFabId: " + friend.FriendPlayFabId);
				//Debug.LogError ("Title: " + friend.TitleDisplayName);
				GetUserDataRequest getdatarequest = new GetUserDataRequest () {
					PlayFabId = friend.FriendPlayFabId,//friend.TitleDisplayName,
				};


				int ind2 = index;
				int intfb = indfb;

				//				PlayFabClientAPI.GetUserData (getdatarequest, (result2) => {
				//
				//					Dictionary<string, UserDataRecord> data2 = result2.Data;
				//					playfabFriendsName [ind2] = data2 ["PlayerName"].Value;
				//					//Debug.LogError ("Added " + data2 ["PlayerName"].Value);
				//					updateName (intfb, data2 ["PlayerName"].Value, friend.FriendPlayFabId);
				//					// BY UZAIR AFTER R5...
				//					if (friend.FacebookInfo != null && friend.FacebookInfo.FacebookId != null) {
				//						if (!friend.FacebookInfo.FacebookId.Equals ("")) {
				//							updateName (intfb + 1, data2 ["PlayerName"].Value, friend.FriendPlayFabId);
				//						}
				//					}
				//					// BY UZAIR AFTER R5...
				//				}, (error) => {
				//
				//					Debug.Log ("Data updated error " + error.ErrorMessage);
				//				}, null);

				playfabFriendsName.Add ("");

				//Debug.LogError ("Facebook :: \"" + friend.FacebookInfo.FacebookId + "\"");
				if (friend.FacebookInfo != null && friend.FacebookInfo.FacebookId != null) {
					if (!friend.FacebookInfo.FacebookId.Equals ("")) {
						playfabFriendsFacebookId.Add (friend.FacebookInfo.FacebookId);
						indfb++;
					} else
						playfabFriendsFacebookId.Add ("Null");
				} else
					playfabFriendsFacebookId.Add ("Null");

				friendsToStatus.Add (friend.FriendPlayFabId);

				indfb++;
				index++;
			}

			GameManager.Instance.friendsIDForStatus = friendsToStatus;

			chatClient.AddFriends (friendsToStatus.ToArray ());

			addPlayFabFriends(playfabFriends, playfabFriendsName, playfabFriendsFacebookId);

			if (PlayerPrefs.GetString ("LoggedType").Equals ("Facebook")) {
				//fbManager.getFacebookFriends();//BY UZAIR AFTER R5...for facebook friends...
				//				getFacebookInvitableFriends ();
			} else {
				showFriends (null, null, null);
			}
		}, OnPlayFabError);
	}

	public void updateName (int i, string text, string id)
	{
		//		Debug.Log (i + " -- " + friendsObjects.Count);
		if (friendsObjects != null && friendsObjects.Count > 0 && i <= friendsObjects.Count - 1 && friendsObjects [i] != null) {
			friendsObjects [i].SetActive (true);
			friendsObjects [i].transform.Find ("FriendName").GetComponent<Text> ().text = text;
		}

	}

	//	public void GetPhotonToken ()
	//	{
	//		GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest ();
	//		request.PhotonApplicationId = Constants.PhotonAppID.Trim ();
	//		PlayFabClientAPI.GetPhotonAuthenticationToken (request, OnPhotonAuthenticationSuccess, OnPlayFabError);
	//		//Debug.LogError ("Getting Token");
	//	}
	//
	//	void OnPhotonAuthenticationSuccess (GetPhotonAuthenticationTokenResult result)
	//	{
	//		string photonToken = result.PhotonCustomAuthenticationToken;
	//		Debug.Log (string.Format ("Yay, logged in session token: {0}", photonToken));
	//		PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues ();
	//		PhotonNetwork.AuthValues.AuthType = Photon.Realtime.CustomAuthenticationType.Custom;
	//		PhotonNetwork.AuthValues.AddAuthParameter ("username", FacebookAndPlayFabManager.Instance.PlayFabUserId);
	//		PhotonNetwork.AuthValues.AddAuthParameter ("Token", result.PhotonCustomAuthenticationToken);
	//		PhotonNetwork.AuthValues.UserId = FacebookAndPlayFabManager.Instance.PlayFabUserId;
	//		//PhotonNetwork.ConnectUsingSettings ("1.0");
	//		//Debug.LogError ("Trying to connect to photon");
	//		//PhotonNetwork.playerName = this.PlayFabUserId;
	//		authToken = result.PhotonCustomAuthenticationToken;
	//		//getPlayerDataRequest ();
	//		connectToChat ();
	//	}

	//backup

//	public void getFacebookFriends ()
//	{
//		ClearPreviousFriendList ();
//
//		FB.API ("/me/friends?limit=5000&fields=id,name,picture.width(117).height(112)", Facebook.Unity.HttpMethod.GET, delegate (IGraphResult result) {
//
//			if (result.Error == null) {
//
//				List<string> friendsNames = new List<string> ();
//				List<string> friendsIDs = new List<string> ();
//				List<string> friendsAvatars = new List<string> ();
//				//Grab all requests in the form of a dictionary. 
//				chatClient.RemoveFriends(GameManager.Instance.friendsIDForStatus.ToArray ());
//				Dictionary<string, object> reqResult = Json.Deserialize (result.RawResult) as Dictionary<string, object>;
//				//Grab 'data' and put it in a list of objects. 
//
//
//				List<object> newObj = reqResult ["data"] as List<object>;
//				//For every item in newObj is a separate request, so iterate on each of them separately. 
//
//				for (int xx = 0; xx < newObj.Count; xx++) {
//					Dictionary<string, object> reqConvert = newObj [xx] as Dictionary<string, object>;
//
//					string name = reqConvert ["name"] as string;
//					string id = reqConvert ["id"] as string;
//
//					friendsIDs.Add(id);
//
//					Dictionary<string, object> avatarDict = reqConvert ["picture"] as Dictionary<string, object>;
//					avatarDict = avatarDict ["data"] as Dictionary<string, object>;
//
//					string avatarUrl = avatarDict ["url"] as string;
//
//					friendNames.Add(name);
//					friendAvatars.Add(avatarUrl);
//					friendIDs.Add(id);
//				}
//
//				GameManager.Instance.friendsIDForStatus = friendsIDs;
//				chatClient.AddFriends (friendsIDs.ToArray ());
//
//
//			} else {
//				Debug.Log ("Something went wrong. " + result.Error + "  " + result.ToString ());
//			}
//		});
//	}

}
*/