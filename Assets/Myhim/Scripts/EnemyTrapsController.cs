using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit.RGKCar;

public class EnemyTrapsController : MonoBehaviour {


	public GameObject Car;
	private Rigidbody IceRb;
	bool value = false;
	private RGKCar_Engine CarEngine;
	private float speed = 0f;

	public AudioSource source;
	public AudioClip oilClip;
	public AudioClip iceClip;
	public AudioClip rocketClip;
	public AudioClip trapClip;
	// Use this for initialization
	void Start () {

		CarEngine = GetComponent<RGKCar_Engine> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (value) {

			if (IceRb) {

				IceRb.AddForce (transform.forward * speed);
			}
		}
		
	}

	public void DeployTrap(){
		Invoke ("DeployTrapWithDelay", Random.Range (0.5f, 3.5f));
	}

	void DeployTrapWithDelay(){
		Vector3 Pos = Car.transform.position;
		Quaternion Rot = Car.transform.rotation;
        Rot.eulerAngles = new Vector3(0, Rot.eulerAngles.y, 0);
		Pos.y += 1.2f;



		int num = Random.Range (0, 5);

		if (num == 0) {
			GameObject Trap = Instantiate (Resources.Load ("Traps/Trap_Anim"), Pos, Rot) as GameObject;
			source.PlayOneShot (trapClip);

		} else if (num == 1) {

			GameObject Trap = Instantiate (Resources.Load ("Traps/OilSpillTrigger"), Pos, Rot) as GameObject;
			source.PlayOneShot (oilClip);

		} else if (num == 2) {

			var iceSpawnTransform = Car.transform.Find ("IcePosition");
			GameObject Trap = Instantiate (Resources.Load ("Traps/IceShoot"), 
				                  iceSpawnTransform.position, iceSpawnTransform.rotation) as GameObject;
			Trap.GetComponent<Rigidbody> ().velocity = Car.GetComponent<Rigidbody> ().velocity;
			source.PlayOneShot (iceClip);
		} else if (num == 3) {

			Pos.y += 0.5f;
			GameObject Trap = Instantiate (Resources.Load ("Traps/BombTrigger"), Pos, Rot) as GameObject;

		} else if (num == 4) {
			var rocketSpawnTransform = Car.transform.Find ("IcePosition");
			GameObject rocket = Instantiate (Resources.Load ("Traps/RocketShoot"), 
				rocketSpawnTransform.position, rocketSpawnTransform.rotation) as GameObject;
			rocket.GetComponent<Rigidbody> ().velocity = Car.GetComponent<Rigidbody> ().velocity;
			source.PlayOneShot (rocketClip);
		}

	}

//	void ShootWithDelay(){
//	
//		Vector3 IcePos = Car.transform.Find("IcePosition").position;
//		Quaternion IceRot = Car.transform.Find("IcePosition").rotation;
//		GameObject Trap = Instantiate(Resources.Load("Traps/IceShoot"), IcePos, IceRot) as GameObject;
//		IceRb = Trap.GetComponent<Rigidbody> ();
//		ShootIceCube (Trap);
//	}

	void ShootIceCube(GameObject Ice){

		speed = CarEngine._SpeedAsKM;
		speed += 500f;
		value = true;
	}
}
