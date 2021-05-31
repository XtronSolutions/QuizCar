using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
using Photon.Pun;

public class OilSpillTriggerController : MonoBehaviour, IPunInstantiateMagicCallback {

	private vehicleHandling _vehicleHandler;
	private AICarSpillEffect _AiCar;

	public Sprite icon;
	public bool isDeployedByPlayer = false;

	public AudioSource audioSource;
	public AudioClip clip;

	int viewId;
    Racer_Register rg;
    // Use this for initialization
    void Start () {

		Invoke ("EnableAction", 1f);
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void OnTriggerEnter(Collider col){
	
		if (col.gameObject.tag.Equals ("Player")) {
			
			if (!Constants.isMultiplayerSelected && !isDeployedByPlayer) {
				_vehicleHandler = col.gameObject.GetComponentInParent<vehicleHandling> ();
				_vehicleHandler.isOilSpil = true;
				PlaySpillSound ();
				this.gameObject.SetActive (false);
				Invoke ("DisAbleSpillEffect", 4f);
			}

			if (Constants.isMultiplayerSelected && col.transform.root.GetComponent<PhotonView>().ViewID == viewId) {
				return;
			}

			if (Constants.isMultiplayerSelected) {

                rg = col.transform.root.GetComponent<Racer_Register>();
                GetComponent<PhotonView> ().RPC ("OilSpillTriggered", RpcTarget.All,  viewId,PlayfabManager.PlayerID,
					PlayerManagerScript.instance.Car.GetComponent<PhotonView>().ViewID);
			}


		}

		if (col.gameObject.tag.Equals ("AI")) {

//			this.gameObject.SetActive(false);
			_AiCar = col.gameObject.GetComponentInParent<AICarSpillEffect> ();
			_AiCar.OilSpill = true;
			if(isDeployedByPlayer)
				TrapDetailManager.Instance.ShowTrapDetail(icon, col.transform.root.GetComponent<Racer_Register>().Avatar);

			PlaySpillSound ();
			Destroy (this.gameObject);
			//Invoke ("DisAbleAICarIceEffect", 8f);


		}
	}

	void DisAbleSpillEffect(){
	
		_vehicleHandler.isOilSpil = false;
		if (Constants.isMultiplayerSelected && PhotonNetwork.IsMasterClient) {
			PhotonNetwork.Destroy (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
		//_vehicleHandler.FirstTimeSpill = true;
	}

	void EnableAction(){

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}

	void PlaySpillSound(){

		audioSource.PlayOneShot (clip);
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		viewId = (int)info.photonView.InstantiationData [0];
	}

	[PunRPC]
	void OilSpillTriggered(int deployedBy, string fbid, int attachedTo){
	
		var g = GlobalVariables.FindGameObjectByViewId (attachedTo);
		if (g != null) {
			_vehicleHandler = g.GetComponent<vehicleHandling> ();
			_vehicleHandler.isOilSpil = true;

			Invoke ("DisAbleSpillEffect", 4f);
		}

        if (PlayerManagerScript.instance.Car.GetComponent<PhotonView>().ViewID == deployedBy)
        {
            TrapDetailManager.Instance.ShowTrapDetailOnline(icon, fbid, g.GetComponent<Racer_Register>().RacerDetail.avatarholder.sprite);

        }
        this.gameObject.SetActive (false);
	}
}
