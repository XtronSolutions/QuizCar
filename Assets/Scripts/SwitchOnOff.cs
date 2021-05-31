using UnityEngine;
using System.Collections;

public class SwitchOnOff : MonoBehaviour {

	public bool SwitchOn;
	public GameObject[] objects;

	// Use this for initialization
	void Awake () {
	
		foreach (GameObject obj in objects) {
			if(SwitchOn)
				obj.SetActive (true);
			else
				obj.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
