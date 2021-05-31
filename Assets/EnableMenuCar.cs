using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableMenuCar : MonoBehaviour {

	public GameObject Buggy;
	// Use this for initialization
	void Start () {

		Invoke ("EnableCar", 2f);
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	void EnableCar(){
	
		Buggy.SetActive (true);
	}
}
