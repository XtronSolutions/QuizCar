// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectAndJoinRandom.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities, 
// </copyright>
// <summary>
//  Simple component to call ConnectUsingSettings and to get into a PUN room easily.
// </summary>
// <remarks>
//  A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.
//  </remarks>                                                                                               
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using RacingGameKit;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Chat;
using ExitGames.Client.Photon;

namespace Photon.Pun.UtilityScripts
{
	/// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
	/// <remarks>A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.</remarks>
	public class ConnectAndJoinPrivateRoom : MonoBehaviourPunCallbacks
	{
		/// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
		public bool AutoConnect = true;

		/// <summary>Used as PhotonNetwork.GameVersion.</summary>
		public byte Version = 1;

		public GameObject playerCar;
		public Image[] addPlayerIcons;
		public Image[] connectedPlayerIcons;
		public GameObject loadingScreen;
		public Sprite connectedImage;
		public GameObject startRaceButton;
		public Text shareCodeText;
		public GameObject connectingText;
		public GameObject inviteCodeText;
		public GameObject shareButton;
		public GameObject invalidCode;
		public GameObject enterCodePanel;
		public GameObject enterCodeButton;
		public GameObject eneterCodeInvalidText;
		public GameObject spinner;
		public GameObject InvitePanel;
		public AudioSource alertOnConnect;
		public GameObject somethingWentWrongPanel;
		public Text[] PlayerCount;

		private int connectedPlayers = 0;
		private bool isConnecting = false;
		private int width = 100;
		private int height = 100;
		private int emptyIndex = -1;
		private bool isRaceStarted = false;

		public void Start()
		{
			if (this.AutoConnect)
			{
				this.ConnectNow();
				if (GameManager.Instance.chatClient != null) {
					GameManager.Instance.chatClient.SetOnlineStatus (ChatUserStatus.Playing);
				}
			}
		}

		public void Update(){
			if(PlayerCount[0]!=null)
			{
				if(PhotonNetwork.CurrentRoom!=null)
					PlayerCount[0].text=PhotonNetwork.CurrentRoom.PlayerCount.ToString();
				else
					PlayerCount[0].text="0";
			}
			
			if(PlayerCount[1]!=null)
			{
				if(PhotonNetwork.CurrentRoom!=null)
					PlayerCount[1].text=PhotonNetwork.CurrentRoom.PlayerCount.ToString();
				else
					PlayerCount[1].text="0";
			}
		}

		public void ConnectNow()
		{
			if(PhotonNetwork.IsConnected)
			{
				ConnectionMaster();
			}else
			{
				Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
			}
		}


		// below, we implement some callbacks of the Photon Realtime API.
		// Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.

		public void ConnectionMaster()
		{
			Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
			PhotonNetwork.AutomaticallySyncScene = true;
			
			string name="player";
			if(FirebaseManager.Instance!=null)
				name=FirebaseManager.Instance.userProfile.Name;


			PhotonNetwork.LocalPlayer.NickName = name;
			if (Constants.joinCode == "") {
				var roomCode = Random.Range (100000, 999999);

                var customProperties = new Hashtable();
                customProperties.Add("m", GameData.selectedEnvironment);
                customProperties.Add("t", GameData.trackNo);

                PhotonNetwork.CreateRoom (roomCode.ToString (), new RoomOptions () { MaxPlayers = 6, PublishUserId = true, CustomRoomProperties = customProperties }, null);
				shareCodeText.text = roomCode.ToString ();
				Debug.Log(roomCode.ToString ());
				isConnecting = true;
			} else {
				Debug.LogError("Join room with code: "+Constants.joinCode);
				PhotonNetwork.JoinRoom (Constants.joinCode);
				shareCodeText.text = Constants.joinCode.ToString ();
				isConnecting = false;
			}
		}

		public override void OnConnectedToMaster()
		{
			ConnectionMaster();
		}

		public override void OnJoinedLobby()
		{
			Debug.Log("OnJoinedLobby(). This client is connected. This script now calls: PhotonNetwork.JoinRandomRoom();");
			PhotonNetwork.JoinRandomRoom();
		}

		public override void OnJoinRoomFailed(short returnCode, string message)
		{
			Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
//			PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 7 }, null);
			if (isConnecting) {
				var roomCode = Random.Range (100000, 999999);
                var customProperties = new Hashtable();
                customProperties.Add("m", GameData.selectedEnvironment);
                customProperties.Add("t", GameData.trackNo);
                PhotonNetwork.CreateRoom (roomCode.ToString (), new RoomOptions () { MaxPlayers = 6, PublishUserId = true, CustomRoomProperties = customProperties }, null);
				shareCodeText.text = roomCode.ToString ();
			} else {
				//connectingText.SetActive (false);
				//loadingScreen.SetActive (false);
				invalidCode.SetActive (true);
				//enterCodeButton.SetActive (true);
				ConnectionManager.Instance.LoadingScreen.SetActive(false);
				ConnectionManager.Instance.ResetJoinData();
			}
		}

		// the following methods are implemented to give you some context. re-implement them as needed.
		public override void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("OnDisconnected("+cause+")");
			if (cause != DisconnectCause.DisconnectByClientLogic) {
				somethingWentWrongPanel.SetActive (true);
			}
		}

		public override void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running.");

            GameData.trackNo =int.Parse( PhotonNetwork.CurrentRoom.CustomProperties["t"].ToString());
            Debug.Log("OnJoinedRoom " + GameData.trackNo);
            GameData.selectedEnvironment = int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["m"].ToString());
            Debug.Log("OnJoinedRoom " + GameData.selectedEnvironment);

            //connectingText.SetActive (false);
			//inviteCodeText.SetActive (true);
			
			if(GameData.isCreateRoom)
			{
				shareCodeText.gameObject.SetActive (true);
				shareButton.SetActive (true);
			}
			else
			{
				ConnectionManager.Instance.JoinData.CodeObject.SetActive(false);
				ConnectionManager.Instance.JoinData.ConnectingText.gameObject.SetActive(true);
			}

			connectedPlayers++;
			Race_Manager.player = playerCar;
			Race_Manager.playerSpawnPosition = PhotonNetwork.CurrentRoom.PlayerCount;
			PhotonNetwork.LocalPlayer.SetScore (Race_Manager.playerSpawnPosition);
			Race_Manager.totalRacers = connectedPlayers;

			loadingScreen.SetActive (false);

			if(ConnectionManager.Instance)
				ConnectionManager.Instance.LoadingScreen.SetActive(false);

			alertOnConnect.Play ();
		}

		public override void OnPlayerEnteredRoom (Player newPlayer)
		{
			connectedPlayers++;
			Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + connectedPlayers);
			if (connectedPlayers >= 2) {
				if (PhotonNetwork.IsMasterClient) {
					startRaceButton.SetActive (true);
					startRaceButton.GetComponent<Button>().interactable=true;
				}
				Race_Manager.totalRacers = connectedPlayers;

				int indexToSet = connectedPlayers - 2;
				if (emptyIndex != -1) {
					indexToSet = emptyIndex;
					emptyIndex = -1;
				}

				addPlayerIcons [indexToSet].enabled = false;
				connectedPlayerIcons [indexToSet].sprite = connectedImage;
				connectedPlayerIcons [indexToSet].enabled = true;
				if (newPlayer.NickName.Length > 0) {
					addPlayerIcons [indexToSet].name = newPlayer.NickName;
					connectedPlayerIcons [indexToSet].name = newPlayer.NickName;
					/*FBManager.Instance.GetFacebookUserPicture (newPlayer.NickName, width, height, res => {
						connectedPlayerIcons [indexToSet].transform.localScale = new Vector3 (0.9f, 0.9f, 1);
						connectedPlayerIcons [indexToSet].sprite = ImageUtils.CreateSprite (res.Texture, new Rect (0, 0, width, height), Vector2.zero);
					});*/
				} else {
					addPlayerIcons [indexToSet].name = "-1";
					connectedPlayerIcons [indexToSet].name = "-1";
				}
				alertOnConnect.Play ();
				photonView.RPC ("OnJoin", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, Race_Manager.playerSpawnPosition);
			}
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (isRaceStarted)
				return;

			if (PhotonNetwork.IsMasterClient && connectedPlayers >= 2) {
				startRaceButton.SetActive (true);
				startRaceButton.GetComponent<Button>().interactable=true;
			}
		}

		public override void OnPlayerLeftRoom(Player otherPlayer){

			if (isRaceStarted)
				return;

			connectedPlayers--;

			if (connectedPlayers < 2 && PhotonNetwork.IsMasterClient) {
				//startRaceButton.SetActive (false);
				startRaceButton.GetComponent<Button>().interactable=false;
			}

			Debug.Log("OnPlayerLeftRoom() called by PUN. Connected players " + connectedPlayers + " Spawn Point " + otherPlayer.GetScore());
			Race_Manager.totalRacers = connectedPlayers;
			var queryName = otherPlayer.NickName.Length > 0 ? otherPlayer.NickName : "-1";

			var indexToFind = 0;

			for (int i = 0; i < addPlayerIcons.Length; i++) {
				if (connectedPlayerIcons [i].name.Equals (queryName)) {
					indexToFind = i;
					break;
				}
			}
			connectedPlayerIcons [indexToFind].name = "image";
			connectedPlayerIcons [indexToFind].enabled = false;
			connectedPlayerIcons [indexToFind].transform.localScale = Vector3.one;
			addPlayerIcons [indexToFind].enabled = true;
			emptyIndex = indexToFind;

			if (Race_Manager.playerSpawnPosition > PhotonNetwork.CurrentRoom.PlayerCount) {
				Race_Manager.playerSpawnPosition = otherPlayer.GetScore ();
			}

		}

		[PunRPC]
		void OnJoin(string id, int pos){
			Debug.Log ("OnJoin() called by PUNRPC > " + id);
			if (connectedPlayers < PhotonNetwork.CurrentRoom.PlayerCount) {
				connectedPlayers++;
				Race_Manager.totalRacers = connectedPlayers;

				int indexToSet = pos - 1;
				if (emptyIndex != -1) {
					indexToSet = emptyIndex;
					emptyIndex = -1;
				}

				addPlayerIcons [indexToSet].enabled = false;
				connectedPlayerIcons [indexToSet].sprite = connectedImage;
				connectedPlayerIcons [indexToSet].enabled = true;
                if (id != null)
                {
                    if (id.Length > 0)
                    {
                        addPlayerIcons[indexToSet].name = id;
                        connectedPlayerIcons[indexToSet].name = id;
                      /*  FBManager.Instance.GetFacebookUserPicture(id, width, height, res =>
                        {
                            connectedPlayerIcons[indexToSet].transform.localScale = new Vector3(0.9f, 0.9f, 1);
                            connectedPlayerIcons[indexToSet].sprite = ImageUtils.CreateSprite(res.Texture, new Rect(0, 0, width, height), Vector2.zero);
                        });*/
                    }
                    else
                    {
                        addPlayerIcons[indexToSet].name = "-1";
                        connectedPlayerIcons[indexToSet].name = "-1";
                    }
                }
                else
                {
                    addPlayerIcons[indexToSet].name = "-1";
                    connectedPlayerIcons[indexToSet].name = "-1";
                }
                alertOnConnect.Play ();
			}
		}

		[PunRPC]
		void OnRaceStart(){
			Debug.Log ("OnRaceStart() called by PUNRPC");
			isRaceStarted = true;
			loadingScreen.SetActive (true);
		}

		public void OnBack(){
			if (loadingScreen != null) {
				loadingScreen.SetActive (true);
			} else {
				var script = GameObject.FindObjectOfType<DesertUIManager> ();
				if (script != null) {
					script.loading_screen.SetActive (true);
				}
			}
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.Disconnect ();
			}
            DontDestroy dd = FindObjectOfType<DontDestroy>();
            if(dd!=null)
            Destroy(dd.gameObject);
			Destroy (this.gameObject);
			SceneManager.LoadScene ("MainMenuScene");
		}

		public void StartRace(){
			if (PhotonNetwork.IsMasterClient) {
				loadingScreen.SetActive (true);
				isRaceStarted = true;
				photonView.RPC ("OnRaceStart", RpcTarget.Others);
				PhotonNetwork.CurrentRoom.IsOpen = false;
				int trackNo = GameData.trackNo;
                if (GameData.selectedEnvironment == 0)
                {

                    //PhotonNetwork.LoadLevel("ForestBeachEnvLoader");
                    if (GameData.trackNo < 3)
                    {
                        GameData.isDay = true;
                        PhotonNetwork.LoadLevel("Env2");//ForestBeachEnvLoader
                    }
                    else
                    {
                        GameData.isDay = false;
                        PhotonNetwork.LoadLevel("Env2");//ForestBeachEnvLoader
                    }
                }
                else
                {
                   // PhotonNetwork.LoadLevel("desert_01");

                    if (GameData.trackNo < 3)
                    {
                        GameData.isDay = true;
                        PhotonNetwork.LoadLevel("Env2");//desert_01
                    }
                    else
                    {
                        GameData.isDay = false;
                        PhotonNetwork.LoadLevel("Env2");//desert_02night
                    }
                }
            }
		}

		public void OnEnterCode(){
			invalidCode.SetActive (false);
			enterCodeButton.SetActive (false);
			enterCodePanel.SetActive (true);
			eneterCodeInvalidText.SetActive (false);
		}

		public void OnJoin(Text code){
			if (code.text.Length != 6) {
				eneterCodeInvalidText.SetActive (true);
				return;
			}

			eneterCodeInvalidText.SetActive (false);
			Constants.joinCode = code.text;
			PhotonNetwork.JoinRoom (code.text);
			shareCodeText.text = code.text;
			enterCodePanel.SetActive (false);
			connectingText.SetActive (true);
			inviteCodeText.SetActive (false);
			shareCodeText.gameObject.SetActive (false);
			shareButton.SetActive (false);
		}

		public void OnInviteFriends(Transform container){
			spinner.SetActive (true);
			//FaceBookFriends.Instance.getFacebookInvitableFriends (container, spinner, InvitePanel);
		//	FaceBookFriends.Instance.code = shareCodeText.text;
		}
	}
		
}
