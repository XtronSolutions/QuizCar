using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPanel : MonoBehaviour {

    public GameObject[] objects;
	// Use this for initialization
	void OnEnable () {
        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            for (int i = 0; i < objects.Length; i++)
                objects[i].SetActive(true);
        }
        else
        {
            for (int i = 0; i < objects.Length; i++)
                objects[i].SetActive(false);
        }
    }
	
	
}