using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
using UnityEngine.UI;
public class DebugTime : MonoBehaviour {

    public Race_Manager race_Manager;
    Text text;
	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        text.text = "TimeTotal : " + race_Manager.TimeTotal + "\nTimeCurrent : " + race_Manager.TimeCurrent;
	}
}