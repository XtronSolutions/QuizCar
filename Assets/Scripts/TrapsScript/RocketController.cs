using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent (typeof(Rigidbody))]
public class RocketController : MonoBehaviour {

	public GameObject explosionParticle;

	private Rigidbody _mRigidBody;
	public AudioSource audioSource;
	public AudioClip clip;
	public MeshRenderer _meshRenderer;

	// Use this for initialization
	void Start () {
		_mRigidBody = this.GetComponent<Rigidbody> ();
		Destroy (this.gameObject, 10);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		_mRigidBody.AddRelativeForce (Vector3.forward * 350);
	}

	public void OnCollisionEnter(Collision Col){
		
		Instantiate (explosionParticle, this.transform.position, Quaternion.identity);
		_meshRenderer.enabled = false;
        Debug.Log("Missile hit" + Col.gameObject.name);
		var transformToCheck = Col.transform.root;
		//if (transformToCheck.CompareTag ("Player")) 
        {

			//PlayBalstSound (transformToCheck.GetComponent<PlayerAudioManager>());

		}
        
		if (transformToCheck.CompareTag ("Player") || transformToCheck.CompareTag ("AI") ) {

			if (!Constants.isMultiplayerSelected) {

				var blastEffect = transformToCheck.GetComponentInParent<BombEffect> ();
				blastEffect.Value = true;

				if (blastEffect.isPlayerCar) {
					blastEffect.BodyCollider.SetActive (false);
					blastEffect.DummyCollider.SetActive (true);
					Constants.isBombEffectEnable = true;
				}
			}

			if (Constants.isMultiplayerSelected) {
				GetComponent<PhotonView> ().RPC ("RocketCollided", RpcTarget.All, transformToCheck.GetComponent<PhotonView>().ViewID);
			}


        }
        //if (!Constants.isMultiplayerSelected)
        {
            Destroy(this.gameObject);
        }
        if (GameData.isSound)
        {
            PlayerManagerScript.instance.Car.GetComponent<PlayerAudioManager>().PlayMissileExplosionSound();
            //Debug.Log("Missile hit" + Col.gameObject.name);
            //  audioSource.PlayOneShot(clip);
        }

    }

	void PlayBalstSound(PlayerAudioManager pam){

		pam.PlayMissileExplosionSound ();
	}

	[PunRPC]
	void RocketCollided(int viewId){
		var g = GlobalVariables.FindGameObjectByViewId (viewId);
		if (g != null) {
			var blastEffect = g.GetComponent<BombEffect> ();
			blastEffect.Value = true;

			if (PlayerManagerScript.instance.Car.GetComponent<PhotonView> ().ViewID == viewId)  {
				blastEffect.BodyCollider.SetActive (false);
				blastEffect.DummyCollider.SetActive (true);
				Constants.isBombEffectEnable = true;
			}
		}
		if (PhotonNetwork.IsMasterClient) {
			PhotonNetwork.Destroy (this.gameObject);
		}
	}
}
