using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AvatarCheck : MonoBehaviour {

    // Use this for initialization
    bool isSet = false;
    Image img;
	void Start () {
        img = GetComponent<Image>();

    }
	// Update is called once per frame
	void Update () {


        if (img.sprite == null)
        {
            GetComponent<Image>().sprite = GameObject.FindObjectOfType<HUD_Script>().dummyAvatars[Random.Range(0, 6)];
           
        }
    }
}