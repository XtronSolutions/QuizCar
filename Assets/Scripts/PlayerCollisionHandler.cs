using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour {

	public ParticleSystem contactSparkles;
	public ParticleSystem waterWheelEffects;
	public WheelCollider wheelCollider;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		contactSparkles.transform.position = col.contacts [0].point;
		contactSparkles.Play ();
	}

	public void ShowWaterEffects(bool show){
		if (show) {
			if(wheelCollider.rpm > 500)
				waterWheelEffects.Play (true);
		} else {
			waterWheelEffects.Stop (true);
		}
	}
}
