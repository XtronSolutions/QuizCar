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
using Photon.Chat;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>Simple component to call ConnectUsingSettings and to get into a PUN room easily.</summary>
    /// <remarks>A custom inspector provides a button to connect in PlayMode, should AutoConnect be false.</remarks>
    public class ConnectAndJoinRandom : MonoBehaviourPunCallbacks
    {
        /// <summary>Connect automatically? If false you can set this to true later on or call ConnectUsingSettings in your own scripts.</summary>
        public bool AutoConnect = true;

        /// <summary>Used as PhotonNetwork.GameVersion.</summary>
        public byte Version = 1;

		public GameObject playerCar;
		public GameObject enemyCar;
		public UnityEngine.UI.Text timerText;
		public GameObject[] searchingIcons;
		public GameObject loadingScreen;
		public UnityEngine.UI.Image[] dps;
		public GameObject errorDailogue;
		public AudioSource alertOnConnect;
		public GameObject errorDailogue2;
		public GameObject somethingWentWrongPanel;
		public Sprite defaultImage;

		private bool startTimer = false;
		private float timer = 30;
		private int connectedPlayers = 0;
		private int width = 100;
		private int height = 100;
		private float errorTimer = 5;
		private bool startErrorTimer = false;
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
			if (startTimer) {
				timer -= Time.deltaTime;

                if(timerText!=null)
				timerText.text = ((int)timer).ToString();

				if (timer <= 0) {
					startTimer = false;
					if (PhotonNetwork.IsMasterClient) {
						if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
							errorDailogue2.SetActive (true);
						} else {
							loadingScreen.SetActive (true);
							PhotonNetwork.CurrentRoom.IsOpen = false;

                            int trackNo = GameData.trackNo;
                            if (GameData.selectedEnvironment == 0)
                            {

                                //PhotonNetwork.LoadLevel("ForestBeachEnvLoader");
                                if (GameData.trackNo < 3)
                                {
                                    GameData.isDay = true;
                                    PhotonNetwork.LoadLevel("Env2"); //ForestBeachEnvLoader
                                }
                                else
                                {
                                    GameData.isDay = false;
                                    PhotonNetwork.LoadLevel("Env2");//ForestBeachEnvNightNew
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
                            isRaceStarted = true;
							photonView.RPC ("OnRaceStart", RpcTarget.Others);
						}
					} else {
						startErrorTimer = true;
					}
				}
			} else if (startErrorTimer) {
				errorTimer -= Time.deltaTime;

				if (errorTimer <= 0) {
					startErrorTimer = false;
                    if(errorDailogue!=null)
					errorDailogue.SetActive (true);
				}
			}
		}

        public void ConnectNow()
        {
            Debug.Log("ConnectAndJoinRandom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = this.Version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
        }


        // below, we implement some callbacks of the Photon Realtime API.
        // Being a MonoBehaviourPunCallbacks means, we can override the few methods which are needed here.


        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");

			PhotonNetwork.AutomaticallySyncScene = true;
			PhotonNetwork.LocalPlayer.NickName = PlayfabManager.PlayerID;

			var customProperties = new Hashtable ();
			customProperties.Add("m", GameData.selectedEnvironment);
			customProperties.Add ("t", GameData.trackNo);

			PhotonNetwork.JoinRandomRoom (customProperties, 0);
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby(). This client is connected. This script now calls: PhotonNetwork.JoinRandomRoom();");
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
       		
			var customProperties = new Hashtable ();
            customProperties.Add("m", GameData.selectedEnvironment);
            customProperties.Add("t", GameData.trackNo);

            var roomOptions = new RoomOptions ();
			roomOptions.MaxPlayers = 7;
			roomOptions.CustomRoomProperties = customProperties;
			roomOptions.CustomRoomPropertiesForLobby = new string[]{ "m", "t" };

			PhotonNetwork.CreateRoom(null, roomOptions, null);
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
			connectedPlayers++;
			Race_Manager.player = playerCar;
			Race_Manager.playerSpawnPosition = PhotonNetwork.CurrentRoom.PlayerCount;
			PhotonNetwork.LocalPlayer.SetScore (Race_Manager.playerSpawnPosition);
			Race_Manager.totalRacers = connectedPlayers;
       
            startTimer = true;
			loadingScreen.SetActive (false);
			alertOnConnect.Play ();
		}

		public override void OnPlayerLeftRoom(Player otherPlayer){
			if (isRaceStarted) {
             //   connectedPlayers--;

                //Race_Manager.totalRacers = connectedPlayers;

                return;
			}

			connectedPlayers--;
			Debug.Log("OnPlayerLeftRoom() called by PUN. Connected players " + connectedPlayers + " Spawn Point " + otherPlayer.GetScore());
			Race_Manager.totalRacers = connectedPlayers;
			var queryName = otherPlayer.NickName.Length > 0 ? otherPlayer.NickName : "-1";

			var indexToFind = 0;

			for (int i = 0; i < searchingIcons.Length; i++) {
				if (dps [i].name.Equals (queryName)) {
					indexToFind = i;
					break;
				}
			}
			dps [indexToFind].name = "image";
			dps [indexToFind].sprite = defaultImage;
			dps [indexToFind].transform.localScale = Vector3.one;
			searchingIcons [indexToFind].SetActive (true);
			emptyIndex = indexToFind;

			if (Race_Manager.playerSpawnPosition > PhotonNetwork.CurrentRoom.PlayerCount) {
				Race_Manager.playerSpawnPosition = otherPlayer.GetScore ();
			}

		}

		public override void OnPlayerEnteredRoom (Player newPlayer)
		{
			connectedPlayers++;
			Debug.Log("OnPlayerEnteredRoom() called by PUN. Connected players " + connectedPlayers);
			if (connectedPlayers >= 2) {
				Race_Manager.totalRacers = connectedPlayers;
				int indexToSet = connectedPlayers - 2;
				if (emptyIndex != -1) {
					indexToSet = emptyIndex;
					emptyIndex = -1;
				}
				searchingIcons [indexToSet].SetActive (false);
				if (newPlayer.NickName.Length > 0) {
					dps [indexToSet].name = newPlayer.NickName;
					/*FBManager.Instance.GetFacebookUserPicture (newPlayer.NickName, width, height, res => {
						dps [indexToSet].transform.localScale = new Vector3 (0.9f, 0.9f, 1);
						dps [indexToSet].sprite = ImageUtils.CreateSprite (res.Texture, new Rect (0, 0, width, height), Vector2.zero);
					});*/
				} else {
					dps [indexToSet].name = "-1";
				}
				alertOnConnect.Play ();
				//photonView.RPC ("OnJoin", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName, Race_Manager.playerSpawnPosition);
			}
		}

		[PunRPC]
		void OnJoin(string id, int pos){
			Debug.Log ("OnJoin() called by PUNRPC");
			if (connectedPlayers < PhotonNetwork.CurrentRoom.PlayerCount) {
				connectedPlayers++;
				Race_Manager.totalRacers = connectedPlayers;
				int indexToSet = pos - 1;
				if (emptyIndex != -1) {
					indexToSet = emptyIndex;
					emptyIndex = -1;
				}
				searchingIcons [indexToSet].SetActive (false);
				if (id.Length > 0) {
					dps [indexToSet].name = id;
					/*FBManager.Instance.GetFacebookUserPicture (id, width, height, res => {
						dps [indexToSet].transform.localScale = new Vector3 (0.9f, 0.9f, 1);
						dps [indexToSet].sprite = ImageUtils.CreateSprite (res.Texture, new Rect (0, 0, width, height), Vector2.zero);
					});*/
				} else {
					dps [indexToSet].name = "-1";
				}
				alertOnConnect.Play ();
			}
		}

		[PunRPC]
		void OnRaceStart(){
			Debug.Log ("OnRaceStart() called by PUNRPC");
			isRaceStarted = true;
            if(loadingScreen!=null)
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
            //DontDestroy dd = FindObjectOfType<DontDestroy>();
            //if (dd != null)
            //{
            //    Destroy(dd);
            //}
			Destroy (this.gameObject);
			SceneManager.LoadScene ("MainMenu",LoadSceneMode.Single);
		}

		public void OnTryAgain(){
			loadingScreen.SetActive (true);
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.Disconnect ();
			}
			Destroy (this.gameObject);
			SceneManager.LoadScene ("ConnectionScene", LoadSceneMode.Single);
		}
    }


#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(ConnectAndJoinRandom), true)]
public class ConnectAndJoinRandomInspector : Editor
{
	void OnEnable() { EditorApplication.update += Update; }
	void OnDisable() { EditorApplication.update -= Update; }

	bool IsConnectedCache = false;

	void Update()
	{
		if (IsConnectedCache != PhotonNetwork.IsConnected)
		{
			Repaint ();
		}
	}

    public override void OnInspectorGUI()
    {
        this.DrawDefaultInspector(); // Draw the normal inspector


        if (Application.isPlaying && !PhotonNetwork.IsConnected)
        {
            if (GUILayout.Button("Connect"))
            {
				((ConnectAndJoinRandom)this.target).ConnectNow ();
            }
        }
    }
}
#endif
}
