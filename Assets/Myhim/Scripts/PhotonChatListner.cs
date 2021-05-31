using UnityEngine;
using System.Collections;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using Photon;
using UnityEngine.SceneManagement;

public class PhotonChatListner : MonoBehaviour {

	private Animator animator;
	public Text text;
	private string senderID;
	private string roomName;
	// "invited"
	// "accepted"
	public string type;
	public GameObject okButton;
	public GameObject rejectButton;
	public GameObject acceptButton;
	public GameObject matchPlayersCanvas;
	public GameObject friendsCanvas;
	public GameObject menuCanvas;
	public GameObject gameTitle;
	public GameObject payoutCoinsText;
	bool leftRoom = false;
	bool Joined = false;
	bool teamMatch = false;
	bool isChamps2v2Invite = false;

	private AsyncOperation async = null;
	public GameObject LoadingPanel;
	public Image LoadingBar;
	// BY UZAIR AFTER UI TEAM UP...
	// Use this for initialization

	static PhotonChatListner instance;

	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			Destroy (this.gameObject);
		}
	}

	void Start ()
	{
		GameManager.Instance.invitationDialog = this.gameObject;
		//animator = GetComponent<Animator> ();
	}



	public void showInvitationDialog (string name, string id, string room)
	{
		Debug.Log ("Chat: dialogue shown");

		string[] messageSplit = name.Split (';');
		string newName = messageSplit [0];
		string code = messageSplit [1];
		Constants.joinCode = code;
		senderID = id;
		roomName = room;
		text.text = newName + " invited you to play Private Match.";
	}
	public void DestroyThis(){
		instance = null;
		Destroy (this.gameObject);
	}

	public void OnAccept(){

		Constants.isMultiplayerSelected = true;
		Constants.isPrivateModeSelected = true;
		LoadingPanel.SetActive (true);
		StartCoroutine (Loading (GameData.selectedEnvironment));
	}
	IEnumerator Loading(int env)
	{
        Debug.Log("Private loading");
        
		yield return new WaitForSeconds (0.5f);
		if (env ==0) 
		{
			
			//async = SceneManager.LoadSceneAsync("ForestBeachEnvLoader");

            if (GameData.trackNo < 3)
            {
                GameData.isDay = true;
                async = SceneManager.LoadSceneAsync("ForestBeachEnvLoader");
            }
            else
            {
                GameData.isDay = false;
                async = SceneManager.LoadSceneAsync("ForestBeachEnvNightNew");
            }

        }
		else 
		{
			//async = Application.LoadLevelAsync ("DesertScene");

            if (GameData.trackNo < 3)
            {
                GameData.isDay = true;
                async = Application.LoadLevelAsync("desert_01");
            }
            else
            {
                GameData.isDay = false;
                async = Application.LoadLevelAsync("desert_02night");
            }
        }

		while (!async.isDone) 
		{
			LoadingBar.fillAmount = async.progress;
			yield return null;
		}
		if (async.isDone) {
		
			DestroyThis ();
		}
	}

//	public void showInvitationDialog (int type, string name, string id, string room, int tableNumber)
//	{
//		if (PlayerPrefs.GetInt (Constants.PrivateRoomKey, 0) == 0) {
//			leftRoom = false;
//			Joined = false;
//			teamMatch = false;// BY UZAIR AFTER UI TEAM UP...
//			//			rejectButton.SetActive (true);
//			//			acceptButton.SetActive (true);
//			//			okButton.SetActive (false);
//
//			this.type = "invited";
//			senderID = id;
//			roomName = room;
//
//			//BY UZAIR AFTER R5...
//
//			GlobalVariables.isOffline = false;
//			GlobalVariables.isGameManual = false;
//			GlobalVariables.TeamUpMatchMode = false;
//			GlobalVariables.TeamMemberId = "";
//			GlobalVariables.IsInTeamUpMeetUpRoom = false;
//			GlobalVariables.isOfflineModeFromLoginScreen = false;
//			if (type == 0) {
//				GlobalVariables.TeamUpMatchMode = false;
//				text.text = name + " invited you to play Private Match.";
//				this.gameObject.SetActive (true);
//				//				animator.Play ("InvitationDialogShow");
//			} else {
//				Debug.Log ("Invitations OFF");
//			}
//
//
//
//		}
//	}

//
//	public void hideDialog (string a)
//	{
//		Debug.Log ("hideDialog: " + a);
//		animator.Play ("InvitationDialogHide");
//		GameManager.Instance.JoinedByID = true;
//		if (a.Equals ("accepted")) {
//			if (PhotonNetwork.inRoom) {
//				leftRoom = true;
//				ResetValues ();//BY UZAIR AFTER R5...
//				PhotonNetwork.LeaveRoom ();
//				ChampsManager.instance.ResetChampionshipData ();
//				GameManager.Instance.champsMatchMaking.ResetMatchMakingScreen ();
//			} else {
//				JoinRoom (a);
//			}
//		} else if (a.Equals ("rejected")) {
//			GameManager.Instance.JoinedByID = true;
//			if (GlobalVariables.FacebookAppRequests != null) {
//				if (GlobalVariables.FacebookAppRequests.Count > GlobalVariables.FacebookRequestIndex) {
//					if (GlobalVariables.FacebookRequestIndex > -1) {
//						GlobalVariables.FacebookAppRequests.RemoveAt (GlobalVariables.FacebookRequestIndex);
//						GlobalVariables.CAME_FROM_DEEPLINKING = false;
//						GlobalVariables.SaveFacebookAppRequests ();
//					}
//				}
//			}
//		}
//	}
//
//	IEnumerator JoinRoomAfterLeaving ()
//	{
//		yield return new WaitForSeconds (2f);
//		JoinRoom ("accepted");
//		Joined = true;
//	}
//
//	//BY UZAIR AFTER R5...
//	public void ResetValues ()
//	{
//		GlobalVariables.IsInTeamUpMeetUpRoom = false;//BY UZAIR AFTER UI TEAM UP...
//		GlobalVariables.TeamMemberId = "";//BY UZAIR AFTER UI TEAM UP...
//		GlobalVariables.isPlayerInivitor = false;// BY UZAIR AFTER UI TEAM UP...
//		GlobalVariables.TeamUpMatchMode = false;// BY UZAIR AFTER UI TEAM UP...
//		GameManager.Instance.privateRoomID = "";
//		GlobalVariables.AutoRoomChallengedFriendID = "";
//		GlobalVariables.IsAutoRoomToChallengeFriend = false;
//		GlobalVariables.isConnectingToGame = false;
//		GlobalVariables.isUndoDiceLimitReached = false;
//		GlobalVariables.SetCurrentUndos (0);
//	}

}
