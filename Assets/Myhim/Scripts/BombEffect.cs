using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour {

	public bool isAiCar = false;
	public bool isPlayerCar = false;
	public bool Value = false;
	//public float delay = 0;
	private Rigidbody rigid;
	private vehicleHandling player;

	public GameObject DummyCollider;
	public GameObject BodyCollider;
	public GameObject BombTrigger;
	private GameObject EnemyRotationObject;

	public AudioSource audioSource;
	public AudioClip clip;

	public float Rotation_Smoothness;

	private float Resulting_Value_from_Input;
	private Quaternion Quaternion_Rotate_From;
	private Quaternion Quaternion_Rotate_To;

	private Quaternion targetRotation;
	private Vector3 targetAngles;

	// Use this for initialization
	void Start () {
		player = this.GetComponent<vehicleHandling> ();
		rigid = this.GetComponent<Rigidbody> ();

//		if (isPlayerCar) {
//			DummyCollider = this.gameObject.transform.Find ("bodycolliderForEffect").gameObject;
//			BodyCollider = this.gameObject.transform.Find ("bodycollider").gameObject;
//		}

		targetRotation = transform.localRotation;
	}
	bool value1 = false;
	// Update is called once per frame
	void Update () {

		if (isPlayerCar) {
			if (Value) {
		
//				rigid.AddForce (transform.up * 10000, ForceMode.Impulse);
////
//				rigid.AddForce (transform.forward * 1500, ForceMode.Impulse);
//
//				Quaternion_Rotate_From = transform.localRotation;
//				Quaternion_Rotate_To = Quaternion.Euler (0, 0, 180);
//				
//				transform.localRotation = Quaternion.Lerp (Quaternion_Rotate_From, Quaternion_Rotate_To, Time.deltaTime * 7f);

//				targetAngles = transform.eulerAngles + 180f * Vector3.right;
//
//				transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetAngles, 2f * Time.deltaTime); // lerp to new angles

//				Quaternion_Rotate_From = transform.localRotation;
//				Quaternion_Rotate_To = Quaternion.Euler (Quaternion_Rotate_From.x, Quaternion_Rotate_From.y, -180);
//
//				transform.localRotation = Quaternion.Slerp (Quaternion_Rotate_From, Quaternion_Rotate_To, Time.deltaTime * Rotation_Smoothness);

				//Value = false;

//				if (player) {
//					Invoke ("DisableForce", 1f);
//				}



				// Correct 

				rigid.AddForce (transform.up * 300, ForceMode.Impulse);//2000

				Quaternion_Rotate_From = transform.localRotation;
				Quaternion_Rotate_To = Quaternion.Euler (0, 0, 180);

				transform.localRotation = Quaternion.Lerp (Quaternion_Rotate_From, Quaternion_Rotate_To, Time.deltaTime * Rotation_Smoothness);
				if (player) {
					//player.enabled = false;
					Invoke ("DisableForce", 1.4f);
				}

			}
		}

		if (isAiCar) {
		
			if (Value) {

				//rigid.velocity = Vector3.zero;
				rigid.AddForce (transform.up * 1000, ForceMode.Impulse);//5000
				//rigid.AddForce (transform.forward * 5000 * -1, ForceMode.Impulse);//1000

				Quaternion_Rotate_From = transform.rotation;
				Quaternion_Rotate_To = Quaternion.Euler (transform.rotation.x, transform.rotation.y, 90);

				transform.rotation = Quaternion.Lerp (Quaternion_Rotate_From, Quaternion_Rotate_To, Time.deltaTime * Rotation_Smoothness );


				//rigid.AddForce (transform.forward * 50000 * -1, ForceMode.Impulse);//1000
				//Invoke ("StartRotate", 0.2f);
				Invoke ("DisableEnemyCarForce", 1.8f);
			}
				
		}
	}


	void DisableForceOnAiCAr(){

		value1 = false;
	}
	void DisableEnemyCarForce(){

		rigid.velocity = Vector3.zero;
		Value = false;
		//value1 = false;

	}

	void DisableForce(){
	

		Value = false;
		rigid.velocity = Vector3.zero;
		Constants.isBombEffectEnable = false;
		Invoke ("EnableHandle", 0.5f);

	}

	void EnableHandle(){
	
		//PlayerManagerScript.instance._RaceManager.gameObject.GetComponent<PlayerRespawn> ().RespawnPlayer ();
		player.enabled = true;
		BodyCollider.SetActive (true);
		DummyCollider.SetActive (false);
	}

	public void EnableBombParticle(){
	
		Instantiate (Resources.Load ("Traps/FX_Explosion_Chunks"), this.transform.position, Quaternion.identity);
		PlayBlastSound ();
	}

	void PlayBlastSound(){

		audioSource.PlayOneShot (clip);
	}

}
