using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCameraEffects : MonoBehaviour {

	public string tagToDetect;
	public ParticleSystem[] effectsToEmit;
	public AudioSource sfx;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider hit) {
		if(hit.CompareTag(tagToDetect)){
			sfx.Play ();
			foreach(var e in effectsToEmit){
				e.Play ();
			}

			hit.transform.root.GetComponent<PlayerCollisionHandler> ().ShowWaterEffects (true);

			Constants.inWater = true;
		}
	}

	void OnTriggerExit(Collider hit) {
		if(hit.CompareTag(tagToDetect)){
			hit.transform.root.GetComponent<PlayerCollisionHandler> ().ShowWaterEffects (false);
			Constants.inWater = false;
		}
	}
}
