using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exploder.Utils;

public class TreeExplosion : MonoBehaviour {

	bool isCameraShake = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.CompareTag ("Player") || other.gameObject.CompareTag ("AI")) {
//			ExploderSingleton.Instance.ExplodeObject (this.gameObject);

			if (other.gameObject.CompareTag ("Player")){
				if(!isCameraShake){
					isCameraShake = true;
					PlayerManagerScript.instance._RaceManager.GameCamereComponent.IsShakeCameraOnHitObstacle = isCameraShake;
				}
				other.gameObject.GetComponent<vehicleHandling> ().vehicleSpeed = 0;
			}
		}
	}
}
