using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceCubeTrap : MonoBehaviour {

    
	// Use this for initialization
	void OnEnable () {
        Invoke("DisableIt", 8);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void DisableIt()
    {
        this.transform.root.GetComponent<Rigidbody>().mass = 1000;
        this.gameObject.SetActive(false);
    }
}