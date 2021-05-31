using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTutorial : MonoBehaviour {

    // Use this for initialization
   
    void OnEnable()
    {
        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {

        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}