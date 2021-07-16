using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;


public class PhotonLobby : MonoBehaviourPunCallbacks {

	public static PhotonLobby lobby;
	public GameObject ConnectButton;
	public GameObject CancelButton;
	public GameObject ShareCodeButton;
	public Text RoomNo;

	void Awake(){
	
		lobby = this;
	}
	// Use this for initialization
	void Start () {


//		string userId = FacebookAndPlayFabManager.Instance.FacebookUserId;
//		PhotonNetwork.AuthValues = new AuthenticationValues (userId);
//		PhotonNetwork.ConnectUsingSettings ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnConnectedToMaster ()
	{
		//base.OnConnectedToMaster ();
		Debug.Log("Player has connected to master server");
		//CreateRoom ();
		//ConnectButton.SetActive (true);
	}

	public void CreateRoom(){
	
		int randRoomNum = Random.Range (0, 100000);
		RoomOptions roomOps = new RoomOptions{IsVisible = true, IsOpen = true, MaxPlayers = 6 };
		PhotonNetwork.CreateRoom ("Room " + randRoomNum, roomOps);
		//ConnectButton.SetActive (false);
		//CancelButton.SetActive (true);
		RoomNo.text = randRoomNum.ToString ();
		Debug.Log ("Creating new room");
		Debug.Log ("Room " + randRoomNum);
		ShareCodeButton.SetActive (true);
	}

	public override void OnCreateRoomFailed (short returnCode, string message)
	{
		//base.OnCreateRoomFailed (returnCode, message);
		Debug.Log("Room Failed, Already exists");
		CreateRoom ();
	}

	public override void OnJoinedRoom ()
	{
//		base.OnJoinedRoom ();
		Debug.Log("Join Room successfull");

	}

	public void CancelButtonClicked(){
	
		//ConnectButton.SetActive (true);
		//CancelButton.SetActive (false);
		ShareCodeButton.SetActive (false);

		PhotonNetwork.LeaveRoom ();
	}
}
