using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using RacingGameKit;
public class TrapTriggerController : MonoBehaviour {

	public GameObject TrapToMove;
	bool MoveTrap = false;
	public GameObject Trap;
	public BoxCollider TrapToMoveCollider;

	public AudioSource audioSource;
	public AudioClip clip;

	public int viewId;

    [HideInInspector]
    public Racer_Register rg;
	// Use this for initialization
	void Start () {

		Invoke ("EnableAction",0.2f);
	}
	
	// Update is called once per frame
	void Update () {

		if (MoveTrap) {
		
			//iTween.RotateBy (TrapToMove, Rotation, 1f);
			//MoveTrap = false;
		}
	}

	public void OnTriggerEnter(Collider col){

		if (col.gameObject.tag.Equals ("Player") || col.gameObject.tag.Equals ("AI") || col.gameObject.tag.Equals ("Enemy")) {

			if (Constants.isMultiplayerSelected && col.transform.root.GetComponent<PhotonView>().ViewID == viewId) {
				return;
			}

			if (!Constants.isMultiplayerSelected && TrapToMoveCollider.GetComponent<TrapCollisionDetection> ().isDeployedByPlayer) {
				return;
			}
            rg = col.transform.root.GetComponent<Racer_Register>();

            this.gameObject.SetActive(false);
			TrapToMoveCollider.enabled = true;
			iTween.RotateTo(TrapToMove, iTween.Hash("x", 0, "y", 0,"z", 0, "time", 0.2f, "isLocal", true));
			Invoke ("DisableTrap", 1.3f);

		}
	}

	void DisableTrap(){

		Trap.SetActive (false);
		if (Constants.isMultiplayerSelected && PhotonNetwork.IsMasterClient) {
			PhotonNetwork.Destroy (Trap);
		} else {
			Destroy (Trap);
		}
	}

	void EnableAction(){

		this.gameObject.GetComponent<BoxCollider> ().enabled = true;
	}

	public void PlayHitSound(){
		if (!audioSource.isPlaying) {
			audioSource.PlayOneShot (clip);
		}
	}
}
