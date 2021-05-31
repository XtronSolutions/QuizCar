using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Popup : MonoBehaviour {

    public delegate void Callback(bool status);
    Callback caller;

    public GameObject cancelbtn;
    public Text msgText;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ShowPopup(string message, Callback callback)
    {
        caller = null;
        if(callback==null)
        {
            cancelbtn.SetActive(false);
        }
        else
        {
            cancelbtn.SetActive(true);
        }
        caller = callback;
        msgText.text = message;
        this.gameObject.SetActive(true);
    }
    public void Ok()
    {
        if(caller!=null)
        {
            caller(true);
        }
        this.gameObject.SetActive(false);

    }
    public void Cancel()
    {
        if (caller != null)
        {
            caller(false);
        }
        this.gameObject.SetActive(false);

    }
}