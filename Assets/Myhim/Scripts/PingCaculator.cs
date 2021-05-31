using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PingCaculator : MonoBehaviour {

	public Text pingText;

	private float time;

	// Use this for initialization
	void Start () {
		time = Time.time - 3;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > time + 3) {
			time = Time.time;
			pingText.text = string.Format ("ping {0}", PhotonNetwork.GetPing ());
		}
	}
}
