using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpillEffect : MonoBehaviour {

	public Rigidbody Rigid;
	public bool OilSpill = false;
	private bool SecondSpill = false;
	// Use this for initialization


	void Start () 
	{

	}

	// Update is called once per frame
	void Update () 
	{
		if (OilSpill) {


			//StartCoroutine ("SpillEffect");
			Rigid.AddTorque (transform.up * 30000);
			Rigid.AddForce (transform.forward * 10000);
			Invoke ("Disable", 2f);
		}
		if (SecondSpill) {
		
			Rigid.AddTorque (-transform.right * 20000);
			Invoke ("DisableSecondSpill", 0.5f);

		}

	}

	void Disable(){

		OilSpill = false;
		SecondSpill = true;
	}

	void DisableSecondSpill(){
	
		SecondSpill = false;
	}
}
