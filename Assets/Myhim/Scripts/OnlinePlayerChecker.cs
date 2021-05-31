using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
using Photon.Pun;

public class OnlinePlayerChecker : MonoBehaviour {

	float time;
	public Race_Manager raceManager;
	public PhotonView photonView;
	public Race_UI raceUI;
	public GameObject quitDailogue;
	// Use this for initialization
	void Start () {

        if (Constants.isMultiplayerSelected)
        {
            time = Time.time;
            raceUI.SetMultiplayerReadyText();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Constants.isMultiplayerSelected)
        {
            if (Time.time > time + 1)
            {
                time = Time.time;

                if (Race_Manager.totalRacers > 1 && PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                    quitDailogue.SetActive(true);
                }
                else if (FindObjectsOfType<PhotonView>().Length >= PhotonNetwork.CurrentRoom.PlayerCount + 2)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        photonView.RPC("StartRace", RpcTarget.AllViaServer);
                    }
                }
            }
        }
	}

	[PunRPC]
	public void StartRace(){
		raceUI.SetMulitplayerCounterText ();
		raceManager.StartRace ();
		Destroy (this.gameObject);
	}
}
