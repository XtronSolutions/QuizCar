using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UpdateState();
        Invoke("UpdateState", 0.2f);
        Invoke("UpdateState", 0.4f);

        Invoke("UpdateState", 1f);
    }

	// Update is called once per frame
	public void UpdateState () {
        GetComponent<AudioSource>().mute = !GameData.isSound;
	}

}