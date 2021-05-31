using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Poweruptimer : MonoBehaviour {

    Image img;
    float timer;
    public PowerUpHud powerUpHud;
    public int index;
	// Use this for initialization
	void OnEnable () {
        img = this.GetComponent<Image>();
        img.fillAmount = 1;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if(img.fillAmount>0)
        {
            img.fillAmount -= 0.005f;
        }
        else
        {
            powerUpHud.TimerOver(index);
            this.gameObject.SetActive(false);
            
        }
	}
}