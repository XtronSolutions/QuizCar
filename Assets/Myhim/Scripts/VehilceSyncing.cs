using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using RacingGameKit;

public class VehilceSyncing : MonoBehaviour, IPunInstantiateMagicCallback {

	 PhotonView photonView;
	public GameObject[] changeTagObjects;
	 vehicleHandling controller;
	 VehicleUpdate vehicleUpdate;
	 Racer_Register racer;

	public GameObject[] partsToDisable;

	// Use this for initialization
	void Awake () {
        photonView = GetComponent<PhotonView>();
        controller = GetComponent<vehicleHandling>();
        vehicleUpdate = GetComponent<VehicleUpdate>();
        racer = GetComponent<Racer_Register>();
        if (Constants.isMultiplayerSelected)
        {


            if (photonView.IsMine)
            {
                this.gameObject.name = "CustomCarDummyNewOnline(Clone)";
                this.controller.enabled = true;
                foreach (var g in changeTagObjects)
                {
                    g.tag = "Player";
                }

                InvokeRepeating("SyncPlayer", 15, 2);
            }
            else
            {
                this.controller.enabled = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		var vehicleUpgradeLevel = (int)info.photonView.InstantiationData [0];
		if (vehicleUpgradeLevel > 0) {
			vehicleUpdate.SetupVehicle (vehicleUpgradeLevel);
		}
		racer.RacerName = (string)info.photonView.InstantiationData [1];
		var id = (string)info.photonView.InstantiationData [2];
		if (id.Length > 0) {
			//FBManager.Instance.GetFacebookUserPicture(id, 60, 60, res =>
			//{
			//		var sprite = ImageUtils.CreateSprite(res.Texture, new Rect(0,0,60,60), Vector2.zero);
			//		racer.RacerDetail.avatarholder.sprite = sprite;
			//});
		}
	}

    void SyncPlayer()
    {
        if(!racer.IsRacerFinished)
        this.GetComponent<PhotonView>().RPC("Sync", RpcTarget.Others, racer.GetCurrentDistancePointIndex(), racer.GetLastDistancePointIndex(), racer.GetLap(),racer.IsRacerFinished,racer.IsRacerDestroyed);
    }


	[PunRPC]
	public void RaceEnd(int pos, float racerEndTime, string racerName){
		Debug.Log ("RacerEnd() called by RPC");
        //GameObject.FindObjectOfType<GameFinishManualOnline> ().SetOpponentsPosition (pos, racerEndTime, racerName);
        Racer_Register rg = this.GetComponent<Racer_Register>();
        rg.IsRacerFinished = true;
        rg.RacerDetail.RacerFinished = true;
        rg.RacerName = racerName;
        rg.RacerDetail.RacerName = racerName;
        rg.RacerStanding = pos;
        rg.RacerDetail.RacerStanding = pos;
        rg.RacerDetail.RacerTotalTime = racerEndTime;
        

    }
    [PunRPC]
    public void Sync(int currentPoint, int lastPoint, int lap, bool isFinished, bool isDestroyed)
    {
        this.GetComponent<Racer_Register>().SynRacer(currentPoint,lastPoint,lap,isFinished,isDestroyed);
    
    }

    [PunRPC]
    public void Respawned()
    {
       // Debug.Log("Respawned" + this.gameObject.name);

        this.GetComponent<Racer_Register>().SetCurrentIndexOnRespawn();
    }
 






    public void DisableParts(){
		foreach (var p in partsToDisable) {
			p.SetActive (false);
		}
	}

    void OnDestroy()
    {
        Debug.Log(racer.RacerName + "  Destroyed");
        racer.IsRacerDestroyed = true;
        racer.RacerDetail.RacerDestroyed = true;

        if (!racer.IsRacerFinished)
        {
            Race_Manager rm = GameObject.FindObjectOfType<Race_Manager>();
            rm.RegisteredRacers.Remove(racer.RacerDetail);
            Race_Manager.totalRacers--;
            rm.RacePlayers--;

            if (racer != null)
                Destroy(racer);
        }


    }
}
