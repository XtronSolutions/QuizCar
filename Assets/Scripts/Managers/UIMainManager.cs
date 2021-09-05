using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MainScreen
{
    public Button PlayAsGuest;
    public Button SignInButton;
    public Button SignUpButton;
    public Button FacebookLoginButton;
    public Button GoogleLoginButton;
}


    [System.Serializable]
public class LoginScreen
{
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public Button SignInButton;
    public Button SignUpButton;
    public Button FacebookLoginButton;
    public Button GoogleLoginButton;
    public Button BackButton;
}

[System.Serializable]
public class RegisterScreen
{
    public TMP_InputField Name;
    public TMP_InputField EmailInput;
    public TMP_InputField PasswordInput;
    public TMP_InputField ConfirmInput;
    public TMP_InputField PhoneInput;
    public Button BackButton;
    public Button SignUpButton;
}

[System.Serializable]
public class TempScreen
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Email;
    public TextMeshProUGUI UserID;
    public TextMeshProUGUI Phone;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI QuestionAnswered;
    public Button SignOutButton;
}

[System.Serializable]
public class MessageScreen
{
    public GameObject ScreenPanel;
    public TextMeshProUGUI WrittenText;
}

[System.Serializable]
public class MultiplayerSelectionScreen
{
    public Button QuickMatch;
    public Button FriendMatch;
    public Button BackButton;
    public Button CreateMatch;
    public Button JoinMatch;
    public TextMeshProUGUI Name;
}


public class UIMainManager : MonoBehaviour
{
    public static UIMainManager Instance;

    [Tooltip("Reference of all screens in UI")]
    public GameObject[] GameScreens;

    [Space]
    [Tooltip("Reference of all objects related to login screen")]
    public LoginScreen LoginScreenData;

    [Space]
    [Tooltip("Reference of all objects related to Main screen")]
    public MainScreen MainScreenData;

    [Space]
    [Tooltip("Reference of all objects related to Register screen")]
    public RegisterScreen RegisterScreenData;

    [Space]
    [Tooltip("Reference of all objects related to message Screen")]
    public MessageScreen messageScreen;

    [Space]
    [Tooltip("Reference of all objects related to multiplayer selection screen")]
    public MultiplayerSelectionScreen SelectionScreen;

    [Space]
    [Tooltip("Reference of all objects related to temp Screen")]
    public TempScreen tempScreen;

    [Space]
    public GameObject LoadingScreen;

    [Space]
    [Tooltip("Reference of object with script FirebaseManager")]
    public FirebaseManager firebaseManager;

    //private variables
    int ScreenCounter = 0;
    string UserName = "";
    string Email;
    string Password;
    string ConfirmPassword;
    string Phone;
    private const string MatchEmailPattern =
       "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`{}|~\\w])*)(?<=[0-9a-z])@))(?([)([(\\d{1,3}.){3}\\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$";

    public void CheckFirebaseRef()
    {
        firebaseManager = FirebaseManager.Instance;
    }
    private void OnEnable()
    {
        Instance = this;
        //AssignScreenValues();

        CheckFirebaseRef();
        SelectionScreenListeners();
        MainScreenListeners();
        LoginResetVariables();
        LoginListeners();
        RegisterResetVariables();
        RegisterListeners();

        if (GameData.signedIn ==true)
        {
            OnSignInSuccess();
        }
    }
    void Start()
    {
        //MainScreenListeners();
        //LoginResetVariables();
        //LoginListeners();
        //RegisterResetVariables();
        //RegisterListeners();
    }

    #region tempScreen
    public void AssignScreenValues()
    {
        CheckFirebaseRef();
        UserProfile _data = firebaseManager.userProfile;

        //Debug.Log(_data.Name);
        //Debug.Log(_data.EmailAddress);
        //Debug.Log(_data.UID);
        //Debug.Log(_data.PhoneNumber);
        //Debug.Log(_data.Data.Score.ToString());
        //Debug.Log(_data.Data.QuestionAnswered.ToString());

        if (_data != null)
        {
            tempScreen.Name.text = _data.Name;
            tempScreen.Email.text = "Email : " + _data.EmailAddress;
            tempScreen.UserID.text = "UserID : " + _data.UID;
            tempScreen.Phone.text = "Phone : " + _data.PhoneNumber;
            tempScreen.Score.text = "Score : " + _data.Data.Score.ToString();
            tempScreen.QuestionAnswered.text = "Question Answered : " + _data.Data.QuestionAnswered.ToString();

            tempScreen.SignOutButton.onClick.AddListener(OnSignOutClicked);
            SelectionScreen.Name.text=_data.Name.ToString();
        }
    }

    public void OnSignOutClicked()
    {
        GameData.signedIn = false;
        PlayerPrefs.DeleteAll();
        NextScreen(0);
        ToggleLoadingScreen(false);
        LoginResetVariables();
        RegisterResetVariables();

        CheckFirebaseRef();
        firebaseManager.SignOutFirebase();

    }
    #endregion

    #region MainMenu Screen
    public void MainScreenListeners()
    {
        MainScreenData.SignUpButton.onClick.AddListener(OnSignUpClicked_MainScreen);
        MainScreenData.SignInButton.onClick.AddListener(OnSignInClicked_MainScreen);
        MainScreenData.GoogleLoginButton.onClick.AddListener(OnGoogleSignInClicked_MainScreen);
        MainScreenData.FacebookLoginButton.onClick.AddListener(OnFacebookSignInClicked_MainScreen);
        MainScreenData.PlayAsGuest.onClick.AddListener(OnGuestSignInClicked_MainScreen);
    }

    public void OnSignInClicked_MainScreen()
    {
        LoginResetVariables();
        ToggleLoadingScreen(false);
        NextScreen(1);
    }

    public void OnGuestSignInClicked_MainScreen()
    {
        LoginResetVariables();
        ToggleLoadingScreen(true);
        CheckFirebaseRef();
        firebaseManager.GuestSignIn();
    }
    public void OnFacebookSignInClicked_MainScreen()
    {
        LoginResetVariables();

        //GameData.SetSavePlayerData(Email, Password);
        ToggleLoadingScreen(true);
        CheckFirebaseRef();
        FBManager.Instance.Login();
        //firebaseManager.SignInWithGoogle(false);
    }

    public void OnGoogleSignInClicked_MainScreen()
    {
        LoginResetVariables();

        //GameData.SetSavePlayerData(Email, Password);
        ToggleLoadingScreen(true);
        CheckFirebaseRef();
        firebaseManager.SignInWithGoogle(false);
    }

    public void OnSignUpClicked_MainScreen()
    {
        ToggleLoadingScreen(false);
        RegisterResetVariables();
        NextScreen(2);
    }

    #endregion

    #region Login UI
    public void LoginResetVariables()
    {
        Email = "";
        Password = "";
        LoginScreenData.EmailInput.text = "";
        LoginScreenData.PasswordInput.text = "";
    }

    public void LoginListeners()
    {
        LoginScreenData.EmailInput.onValueChanged.AddListener(OnEmailTextChanged);
        LoginScreenData.PasswordInput.onValueChanged.AddListener(OnPassTextChanged);
        LoginScreenData.SignInButton.onClick.AddListener(OnSignInClicked);
        LoginScreenData.BackButton.onClick.AddListener(OnBackClicked);
        //LoginScreenData.SignUpButton.onClick.AddListener(OnSignUpClicked);
    }

    public void OnEmailTextChanged(string _email)
    {
        Email = _email;
    }

    public void OnPassTextChanged(string _pass)
    {
        Password = _pass;
    }

    public void OnSignInClicked()
    {
        if(CheckStringInput(Email)==false)
        {
            ShowToast("Please enter Email Address.", 2f);
            return;
        }
        else if(Regex.IsMatch(Email, MatchEmailPattern)==false)
        {
            ShowToast("Please enter valid Email Address.", 2f);
            return;
        }
        else if(CheckStringInput(Password) == false)
        {
            ShowToast("Please enter Password.", 2f);
            return;
        }

        GameData.SetSavePlayerData(Email, Password);
        ToggleLoadingScreen(true);
        CheckFirebaseRef();
        firebaseManager.SignInWithEmail(Email, Password);
    }

    public void OnSignInSuccess()
    {
        ToggleLoadingScreen(false);
        NextScreen(3);
        AssignScreenValues();
        GameData.signedIn = true;
    }

    public void OnSignInFailed(int _code)
    {
        ToggleLoadingScreen(false);

        if (_code == 0)
            ShowToast("Process cancelled by user.", 2f);
        else if (_code == 1)
            ShowToast("Invalid email or password, please try again.", 2f);
        else if (_code == 2)
            ShowToast("Google sign in was cancelled.", 2f);
    }

    public void OnSignUpClicked()
    {
        NextScreen();
        RegisterResetVariables();
    }

    public void OnBackClicked()
    {
        ToggleLoadingScreen(false);
        NextScreen(0);
    }
    #endregion

    #region Register UI
    public void RegisterResetVariables()
    {
        Email = "";
        Password = "";
        ConfirmPassword = "";
        Phone = "";
        UserName = "";
        RegisterScreenData.Name.text = "";
        RegisterScreenData.EmailInput.text = "";
        RegisterScreenData.PasswordInput.text = "";
        RegisterScreenData.ConfirmInput.text = "";
        RegisterScreenData.PhoneInput.text = "";
    }

    public void RegisterListeners()
    {
        RegisterScreenData.Name.onValueChanged.AddListener(OnNameTextChanged_Register);
        RegisterScreenData.EmailInput.onValueChanged.AddListener(OnEmailTextChanged_Register);
        RegisterScreenData.PasswordInput.onValueChanged.AddListener(OnPassTextChanged_Register);
        RegisterScreenData.ConfirmInput.onValueChanged.AddListener(OnConfirmPassTextChanged_Register);
        RegisterScreenData.PhoneInput.onValueChanged.AddListener(OnPhoneTextChanged_Register);

        RegisterScreenData.SignUpButton.onClick.AddListener(OnSignUpClicked_Register);
        RegisterScreenData.BackButton.onClick.AddListener(OnSignUpBackClicked_Register);
    }

    public void OnNameTextChanged_Register(string _name)
    {
        UserName = _name;
    }

    public void OnEmailTextChanged_Register(string _email)
    {
        Email = _email;
    }

    public void OnPassTextChanged_Register(string _pass)
    {
        Password = _pass;
    }

    public void OnConfirmPassTextChanged_Register(string _confirmpass)
    {
        ConfirmPassword = _confirmpass;
    }

    public void OnPhoneTextChanged_Register(string _phone)
    {
        Phone = _phone;
    }

    public void OnSignUpClicked_Register()
    {
        if (CheckStringInput(UserName) == false)
        {
            ShowToast("Please enter Name.", 2f);
            return;
        }
        else if (CheckStringInput(Email) == false)
        {
            ShowToast("Please enter Email Address.", 2f);
            return;
        }
        else if (Regex.IsMatch(Email, MatchEmailPattern) == false)
        {
            ShowToast("Please enter valid Email Address.", 2f);
            return;
        }
        else if (CheckStringInput(Password) == false)
        {
            ShowToast("Please enter Password.", 2f);
            return;
        }
        else if (CheckStringInput(ConfirmPassword) == false)
        {
            ShowToast("Please enter Pasword again to confirm.", 2f);
            return;
        }
        else if (ConfirmPassword != Password)
        {
            ShowToast("Passwords do not match, please try again.", 2f);
            return;
        }
        else if (CheckStringInput(Phone) == false)
        {
            ShowToast("Please enter Phone Number.", 2f);
            return;
        }

        GameData.SetSavePlayerData(Email, Password);
        ToggleLoadingScreen(true);
        CheckFirebaseRef();
        firebaseManager.SignUpWithEmail(Email, Password, Phone,UserName);
    }

    public void OnSignUpSuccess()
    {
        ToggleLoadingScreen(false);
        NextScreen(3);
        AssignScreenValues();
        GameData.signedIn = true;
    }

    public void OnSignUpFailed(int _code)
    {
        ToggleLoadingScreen(false);

        if (_code == 0)
            ShowToast("Process cancelled by user.", 2f);
        else if (_code == 1)
            ShowToast("Something went wrong, please try again.", 2f);
        else if (_code==2)
            ShowToast("Google sign in was cancelled.", 2f);
    }

    public void OnSignUpBackClicked_Register()
    {
        LoginResetVariables();
        ToggleLoadingScreen(false);
        NextScreen(0);
    }

    #endregion

    #region Generic Screen

    public bool CheckStringInput(string _data)
    {
        if (_data == "" || _data == null)
        {
            return false;
        }
        return true;
    }

    public void NextScreen(int forceIndex = -1)
    {
        if (forceIndex == -1)
        {
            if (ScreenCounter < GameScreens.Length)
                ScreenCounter++;
            else
                ScreenCounter = 0;
        }
        else
            ScreenCounter = forceIndex;

        ChangeScreen(ScreenCounter);
    }

    public void PreviousScreen(int forceIndex = -1)
    {
        if (forceIndex == -1)
        {
            if (ScreenCounter >0)
                ScreenCounter--;
            else
                ScreenCounter = 0;
        }
        else
            ScreenCounter = forceIndex;

        Debug.Log("Changing screen to : "+ScreenCounter);
        ChangeScreen(ScreenCounter);
    }

    public void ChangeScreen(int index)
    {
        for (int i = 0; i < GameScreens.Length; i++)
        {
            if (i == index)
                GameScreens[i].SetActive(true);
            else
                GameScreens[i].SetActive(false);
        }
    }

    public void ToggleLoadingScreen(bool _state)
    {
        LoadingScreen.SetActive(_state);
    }

    public void MainScreen()
    {
        LoadingScreen.SetActive(false);
        NextScreen(0);
    }

    #endregion

    #region Message UI Setup
    public void ToggleMessageScreen(bool _state)
    {
        messageScreen.ScreenPanel.SetActive(_state);
    }

    public void ShowToast(string _msg,float _time)
    {
        ToggleMessageScreen(true);
        messageScreen.WrittenText.text = _msg;
        StartCoroutine(DisableToast(_time));
    }

    IEnumerator DisableToast(float sec)
    {
        yield return new WaitForSeconds(sec);
        ToggleMessageScreen(false);
    }

    #endregion

    #region LoadGameScene (single/multiplayer)
    public void LoadEvironment()
    {
        CheckFirebaseRef();
        ToggleLoadingScreen(true);
        GameData.isDay = true;
        Constants.isMultiplayerSelected = false;
        Constants.isPrivateModeSelected = false;
        Constants.joinCode = "";
        FirebaseManager.Instance.CopyList();
        SceneManager.LoadScene("Env2",LoadSceneMode.Single);
    }

    public void RaceMultiplayer()
    {
        CheckFirebaseRef();
        GameData.selectedEnvironment = 0;
        ToggleLoadingScreen(true);
        GameData.isDay = true;
        Constants.isMultiplayerSelected = true;
        Constants.isPrivateModeSelected = false;
        Constants.joinCode = "";
        FirebaseManager.Instance.CopyList();
        SceneManager.LoadScene("ConnectionScene", LoadSceneMode.Single);
    }


    #endregion

    #region Multiplayer Selection Screen
    public void SelectionScreenListeners()
    {
        SelectionScreen.BackButton.onClick.AddListener(OnBackClicked_SelectionScreen);
        SelectionScreen.QuickMatch.onClick.AddListener(OnQuickMatchClicked_SelectionScreen);
        //SelectionScreen.FriendMatch.onClick.AddListener(OnFriendMatchClicked_SelectionScreen);

        SelectionScreen.CreateMatch.onClick.AddListener(OnFriendMatchCreateClicked_SelectionScreen);
        SelectionScreen.JoinMatch.onClick.AddListener(OnFriendMatchJoinClicked_SelectionScreen);
    }

    public void OnBackClicked_SelectionScreen()
    {
       // Debug.Log("Back clicked");
        PreviousScreen(-1);
    }

    public void OnQuickMatchClicked_SelectionScreen()
    {
        GameData.isQuickMatch=true;
        RaceMultiplayer();
    }

    public void OnFriendMatchCreateClicked_SelectionScreen()
    {
        GameData.isQuickMatch=false;
        GameData.isCreateRoom=true;
        RaceMultiplayer();
    }

    public void OnFriendMatchJoinClicked_SelectionScreen()
    {
        GameData.isQuickMatch=false;
        GameData.isCreateRoom=false;
        RaceMultiplayer();
    }


    #endregion
    
    private void OnDisable()
    {
        MainScreenData.SignUpButton.onClick.RemoveAllListeners();
        MainScreenData.SignInButton.onClick.RemoveAllListeners();

        LoginScreenData.EmailInput.onValueChanged.RemoveAllListeners();
        LoginScreenData.PasswordInput.onValueChanged.RemoveAllListeners();
        LoginScreenData.SignInButton.onClick.RemoveAllListeners();
        LoginScreenData.BackButton.onClick.RemoveAllListeners();

        RegisterScreenData.Name.onValueChanged.RemoveAllListeners();
        RegisterScreenData.EmailInput.onValueChanged.RemoveAllListeners();
        RegisterScreenData.PasswordInput.onValueChanged.RemoveAllListeners();
        RegisterScreenData.ConfirmInput.onValueChanged.RemoveAllListeners();
        RegisterScreenData.PhoneInput.onValueChanged.RemoveAllListeners();

        RegisterScreenData.SignUpButton.onClick.RemoveAllListeners();
        RegisterScreenData.BackButton.onClick.RemoveAllListeners();

        tempScreen.SignOutButton.onClick.RemoveAllListeners();
        MainScreenData.GoogleLoginButton.onClick.RemoveAllListeners();
        MainScreenData.FacebookLoginButton.onClick.RemoveAllListeners();
        MainScreenData.PlayAsGuest.onClick.RemoveAllListeners();

        SelectionScreen.BackButton.onClick.RemoveAllListeners();
        SelectionScreen.QuickMatch.onClick.RemoveAllListeners();

        SelectionScreen.CreateMatch.onClick.RemoveAllListeners();
        SelectionScreen.JoinMatch.onClick.RemoveAllListeners();
    }
}
