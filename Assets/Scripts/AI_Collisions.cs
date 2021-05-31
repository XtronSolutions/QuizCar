using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Collisions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.CompareTag ("Player")) {
			//collision.contacts[0].
			var rb = collision.gameObject.GetComponent<Rigidbody> ();
			rb.AddForce (this.transform.forward.normalized * 10 , ForceMode.VelocityChange);
		}
	}
}
