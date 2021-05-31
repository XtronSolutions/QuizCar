using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour {

    public InputField emailInput;
    public InputField passwordInput;
    public Button forgetPasswordBtn, FbBtn,proceedBtn;
    public Text messageText;

    public Text forgetPasswordMessageText;
    public InputField forgetPasswordEmailInput;
    public Button forgetPasswordProceedBtn;

    public GameObject loginPanel;
    public GameObject forgetPasswordPanel;
    public GameObject recoverEmailSentPanel;
	// Use this for initialization
	void OnEnable () {
        ChangeInputStatus(true);
        messageText.gameObject.SetActive(false);
        emailInput.text = passwordInput.text = "";

        loginPanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
        recoverEmailSentPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void OnCreateNewAccount()
    {
        GameObject.FindObjectOfType<MenusUI>().SignUpMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    void ChangeInputStatus(bool status)
    {
        emailInput.interactable = status;
        passwordInput.interactable = status;
        proceedBtn.interactable = status;
        FbBtn.interactable = status;
        forgetPasswordBtn.interactable = status;
    }
    public void OnForgetPassword()
    {
        loginPanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
        forgetPasswordEmailInput.interactable = true;
        forgetPasswordMessageText.text = "";
        forgetPasswordProceedBtn.interactable = true;
    }
    public void OnForgetPasswrodProceed()
    {
        if(forgetPasswordEmailInput.text.Length>0)
        {
            forgetPasswordEmailInput.interactable = false;
            forgetPasswordMessageText.text = "Please wait!";
            forgetPasswordProceedBtn.interactable = false;
            PlayfabManager.Instance.ForgetPassword(forgetPasswordEmailInput.text, OnRecoveryEmailSent);
        }
        else
        {
            forgetPasswordMessageText.text = "Enter email address";
        }
    }

    void OnRecoveryEmailSent(bool status, string message)
    {
        if(status)
        {
            recoverEmailSentPanel.SetActive(true);
            forgetPasswordPanel.SetActive(false);
        }
        else
        {
            forgetPasswordMessageText.text = message;
        }
        forgetPasswordEmailInput.interactable = true;
        forgetPasswordProceedBtn.interactable = true;
    }
    public void OnShowLogin()
    {
        ChangeInputStatus(true);
        messageText.gameObject.SetActive(false);
        emailInput.text = passwordInput.text = "";

        loginPanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
        recoverEmailSentPanel.SetActive(false);

    }
    public void OnProceed()
    {
       if(emailInput.text.Length>0 && passwordInput.text.Length>0)
        {
            PlayfabManager.Instance.Login(emailInput.text, passwordInput.text, OnLoginCall);
            messageText.text = "Please wait...";
            messageText.gameObject.SetActive(true);
            ChangeInputStatus(false);
        }
    }

    void OnLoginCall(bool status, string message)
    {
        ChangeInputStatus(true);
        if (status)
        {
            GameObject.FindObjectOfType<MenuManager>().OpenMainMenuScreen();
            this.gameObject.SetActive(false);
        }
        else
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }
    }
    public void OnLoginWithFacebook()
    {
        MenusUI.Intance.progressScreen.SetActive(true);
        ChangeInputStatus(false);
        PlayfabManager.Instance.LoginWithFacebook(OnFBLogin);
    }
    void OnFBLogin(bool status, string message)
    {
        MenusUI.Intance.progressScreen.SetActive(false);
        if (status)
        {
            GameObject.FindObjectOfType<MenusUI>().MainMenu.SetActive(true);
            this.gameObject.SetActive(false);
        }
        ChangeInputStatus(true);
    }
}