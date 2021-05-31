using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSignUp()
    {
        MenusUI.Intance.WelcomeMenu.SetActive(false);
        MenusUI.Intance.SignUpMenu.SetActive(true);

    }
    public void OnLogin()
    {
        MenusUI.Intance.WelcomeMenu.SetActive(false);
        MenusUI.Intance.LoginMenu.SetActive(true);
    }
    public void OnLoginWithFacebook()
    {
        MenusUI.Intance.progressScreen.SetActive(true);
        PlayfabManager.Instance.LoginWithFacebook(OnLogin);
    }
    void OnLogin(bool status, string message)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        if (status)
        {
            GameObject.FindObjectOfType<MenusUI>().MainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }


}