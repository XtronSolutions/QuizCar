using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IceCubeController : MonoBehaviour {

	// Use this for initialization
	private Rigidbody _vehicleHandler;
	private Rigidbody _AIRigidBody;
	private GameObject IceCube;
	private float _OMass;
	private Rigidbody _mRigidBody;

	public AudioSource audioSource;
	public AudioClip clip;

	// Use this for initialization
	void Start () {

		Invoke ("EnableAction", 0.5f);
		_mRigidBody = this.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 10);
	}

	// Update is called once per frame
	void LateUpdate () {
		_mRigidBody.AddRelativeForce (Vector3.forward * 1000);
	}

	public void OnTriggerEnter(Collider col){


	}

	public void OnCollisionEnter(Collision Col){
	
		Instantiate (Resources.Load ("Traps/FX_IceExplosion"), this.transform.position, Quaternion.identity);

		if (Col.gameObject.tag.Equals ("Player"))
        {
				if (!Constants.isMultiplayerSelected) {
					PlayIceHitSound (Col.transform.root.GetComponent<PlayerAudioManager> ());
					this.gameObject.SetActive (false);
					_vehicleHandler = Col.gameObject.GetComponentInParent<Rigidbody> ();
					IceCube = _vehicleHandler.GetComponent<vehicleHandling> ().IceCube;//.gameObject.transform.Find ("IceCube").gameObject;
					IceCube.SetActive (true);
					_vehicleHandler.mass = 20000;

              //  Debug.Log("invoke DisAbleIceEffect");
					Invoke ("DisAbleIceEffect", 8f);
				}

			if (Constants.isMultiplayerSelected) {
               
				GetComponent<PhotonView> ().RPC ("IceCollided", RpcTarget.All, Col.transform.root.GetComponent<PhotonView>().ViewID);
			}

		} else if (Col.gameObject.tag.Equals ("AI")) {

			//PlayIceHitSound ();
			this.gameObject.SetActive(false);
			_AIRigidBody = Col.gameObject.GetComponentInParent<Rigidbody> ();
			IceCube = _AIRigidBody.GetComponent<GetColor> ().iceCube;
			IceCube.SetActive (true);
			_AIRigidBody.mass = 20000;

			Invoke ("DisAbleAICarIceEffect", 8f);
		}else if(Col.gameObject.tag.Equals ("Enemy")){
		
		}
		else {
			if(!Constants.isMultiplayerSelected) {
				Destroy (this.gameObject);
			}
		}
	}

	void DisAbleIceEffect(){
       // Debug.Log(" DisAbleIceEffect");
        _vehicleHandler.mass = 1000;
		IceCube.SetActive (false);
		if (Constants.isMultiplayerSelected && PhotonNetwork.IsMasterClient) {
			PhotonNetwork.Destroy (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
        if(this.gameObject!=null)
        {
            if(Constants.isMultiplayerSelected)
                PhotonNetwork.Destroy(this.gameObject);
            if (this.gameObject != null)
                Destroy(this.gameObject);
        }
      //  Debug.Log(" DisAbleIceEffect" + this.gameObject);
    }

	void DisAbleAICarIceEffect(){
	
		_AIRigidBody.mass = 1000;
		IceCube.SetActive (false);
		Destroy (this.gameObject);
	}

	void EnableAction(){
	
		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}

	void PlayIceHitSound(PlayerAudioManager pam){

		pam.PlayIceExplosionSound ();
	}

	[PunRPC]
	void IceCollided(int viewId){
        Debug.Log(viewId);
		var g = GlobalVariables.FindGameObjectByViewId (viewId);
		if (g != null) {
			PlayIceHitSound (g.GetComponent<PlayerAudioManager>());
			this.gameObject.SetActive(false);
			_vehicleHandler = g.GetComponent<Rigidbody> ();
			IceCube = _vehicleHandler.GetComponent<vehicleHandling> ().IceCube;
			IceCube.SetActive (true);
			_vehicleHandler.mass = 20000;
         //   Debug.Log("invoke DisAbleIceEffect");
            Invoke ("DisAbleIceEffect", 8f);
		}
	}
}
