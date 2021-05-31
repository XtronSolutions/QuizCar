using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelUpgradeEffect : MonoBehaviour {

	public WheelCollider wheel;
	public ParticleSystem effect;

	// Use this for initialization
	void Start () {
		effect.Play (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (wheel.rpm >= 900) {
			if (Random.Range (0, 100) % 50 == 0) {
				effect.Play (true);	
			}
		}
	}
}
