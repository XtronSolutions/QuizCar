using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RegisterUI : MonoBehaviour {

    public InputField nameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public InputField passwordConfirmInput;

    public GameObject accountPanel;
    public Text messageText;

    public GameObject loginBtn;
    public Button proceedBtn, FbBtn;
    // Use this for initialization
    void OnEnable () {

        accountPanel.SetActive(true);
        loginBtn.SetActive(false);

        nameInput.text = emailInput.text = passwordInput.text = passwordConfirmInput.text = "";

        accountPanel.SetActive(true);
        messageText.gameObject.SetActive(false);

        ChangeInputStatus(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ChangeInputStatus(bool status)
    {
        nameInput.interactable = status;
        emailInput.interactable = status;
        passwordInput.interactable = status;
        passwordConfirmInput.interactable = status;
        proceedBtn.interactable = status;
        FbBtn.interactable = status;
    }

    public void OnLoginPress()
    {
        this.gameObject.SetActive(false);
        GameObject.FindObjectOfType<MenusUI>().LoginMenu.SetActive(true);

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
    public void OnProceed()
    {
        string validate = CheckInput();
        if (validate == null)
        {
            messageText.text = "Please wait";
            messageText.gameObject.SetActive(true);
            ChangeInputStatus(false);
            PlayfabManager.Instance.RegisterNewAccount(nameInput.text, emailInput.text, passwordInput.text, OnRegister);
        }
        else
        {
            messageText.text = validate;
            messageText.gameObject.SetActive(true);
        }


    }

    void OnRegister(bool status, string message)
    {
        ChangeInputStatus(true);
        if(status)
        {
            accountPanel.SetActive(false);
            loginBtn.SetActive(true);

        }
        else
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);

        }
    }

    string CheckInput()
    {
        if (nameInput.text.Length < 1)
            return "Enter name";
        else if (nameInput.text.Length < 3)
            return "Error: Very short name";
        else if (nameInput.text.Length > 30)
            return "Error: Very long name";
        else if (emailInput.text.Length < 1)
            return "Enter email";
        else if (!IsValidEmail(emailInput.text))
            return "Error: Invalid email";
        else if (passwordInput.text.Length < 1)
            return "Enter Password";
        else if (passwordInput.text.Length < 8)
            return "Password should be 8 or more characters";
        else if (passwordConfirmInput.text != passwordInput.text)
            return "Password doesn't match";



        return null;
    }


    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}