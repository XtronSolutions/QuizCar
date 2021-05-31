using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
using Photon.Pun;

public class BombTrigger : MonoBehaviour, IPunInstantiateMagicCallback {

	private BombEffect PlayerBombEffect;
	private BombEffect EnemyBombEffect;
	public AudioSource audioSource;
	public AudioClip clip;
//	private GameObject DummyCollider;
//	private GameObject BodyCollider;
//	private GameObject Bomb;

	public Sprite icon;
	public bool isDeployedByPlayer = false;

	int viewId;

    Racer_Register rg;
	// Use this for initialization
	void Start () {

		Invoke ("EnableAction", 0.2f);


	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void OnTriggerEnter(Collider col){


        if (col.gameObject.tag.Equals ("Player")) {

			if (Constants.isMultiplayerSelected && col.transform.root.GetComponent<PhotonView>().ViewID == viewId) {
				return;
			}

			if (!Constants.isMultiplayerSelected && !isDeployedByPlayer) {
				this.gameObject.SetActive (false);
				PlayerBombEffect = col.gameObject.GetComponentInParent<BombEffect> ();
				PlayerBombEffect.BombTrigger.SetActive (true);

				Invoke ("BlastPlayerCar", 3f);
			}

			if (Constants.isMultiplayerSelected) {

                rg = col.transform.root.GetComponent<Racer_Register>();


                GetComponent<PhotonView> ().RPC ("BombTriggered", RpcTarget.All,  viewId, PlayfabManager.PlayerID,
					col.gameObject.GetComponentInParent<PhotonView>().ViewID);
			}

		}

		if (col.gameObject.tag.Equals ("AI")) {

			this.gameObject.SetActive(false);
			EnemyBombEffect = col.gameObject.GetComponentInParent<BombEffect> ();
			EnemyBombEffect.BombTrigger.SetActive (true);

			if(isDeployedByPlayer)
				TrapDetailManager.Instance.ShowTrapDetail (icon, col.transform.root.GetComponent<Racer_Register>().Avatar);

			Invoke ("BlastEnemyCar", 3f);
		}
	}


	void BlastPlayerCar(){

		PlayerBombEffect.BodyCollider.SetActive (false);
		PlayerBombEffect.DummyCollider.SetActive (true);
		PlayerBombEffect.Value = true;
		Constants.isBombEffectEnable = true;
		PlayerBombEffect.BombTrigger.SetActive (false);
		PlayerBombEffect.EnableBombParticle ();

		if (Constants.isMultiplayerSelected && PhotonNetwork.IsMasterClient) {
			PhotonNetwork.Destroy (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
	}

	void BlastPlayerCarOnline(){
		PlayerBombEffect.BombTrigger.SetActive (false);
		PlayerBombEffect.EnableBombParticle ();
	}

	void BlastEnemyCar(){

		EnemyBombEffect.BombTrigger.SetActive (false);
		EnemyBombEffect.Value = true;
		EnemyBombEffect.EnableBombParticle ();


		Destroy (this.gameObject);
	}

	void EnableAction(){

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}

	void PlayBlastSound(){

	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		viewId = (int)info.photonView.InstantiationData [0];
	}

	[PunRPC]
	void BombTriggered (int deployedBy, string fbid, int attachedTo){
	

		var g = GlobalVariables.FindGameObjectByViewId (attachedTo);
		if (g != null) {
			PlayerBombEffect = g.GetComponent<BombEffect> ();
			PlayerBombEffect.BombTrigger.SetActive (true);
			if (PlayerManagerScript.instance.Car.GetComponent<PhotonView> ().ViewID == attachedTo) {
				Invoke ("BlastPlayerCar", 3f);
			} else {
				Invoke ("BlastPlayerCarOnline", 3f);
			}
		}
        if (PlayerManagerScript.instance.Car.GetComponent<PhotonView>().ViewID == deployedBy)
        {
            TrapDetailManager.Instance.ShowTrapDetailOnline(icon, fbid, g.GetComponent<Racer_Register>().RacerDetail.avatarholder.sprite);
        }
        this.gameObject.SetActive(false);
	}
}
