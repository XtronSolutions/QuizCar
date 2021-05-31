using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrapSpawnerOnline : MonoBehaviour, IPunInstantiateMagicCallback {

	public TrapTriggerController triggerController;

	public Sprite icon;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		triggerController.viewId = (int)info.photonView.InstantiationData [0];
	}

	public void CollisionDetected(string fbid){
		this.GetComponent<PhotonView> ().RPC ("CollisionDetectedOnline", RpcTarget.All, triggerController.viewId, fbid);
	}

	[PunRPC]
	void CollisionDetectedOnline(int viewId, string fbid){
		if (PlayerManagerScript.instance.Car.GetComponent<PhotonView> ().ViewID == viewId) {
			TrapDetailManager.Instance.ShowTrapDetailOnline (icon, fbid,triggerController.rg.RacerDetail.avatarholder.sprite);
		}
	}
}
