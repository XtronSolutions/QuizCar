using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Chat;

public class MenuManager : MonoBehaviour 
{
	private CameraFlying camFlying;
	private UIAnimationMainMenu mainMenuAnim;
	private UIAnimationEnvironmentSelection enviMenuAnim;
	private PlayerModeUIAnimation playerModeAnim;
	private MultiPlayerModeUIAnimation racingModeAnim;
	private MultiPlayerModeUIAnimation raceWithFirendModeAnim;
	private MultiPlayerModeUIAnimation globalraceModeAnim;
	private MultiPlayerModeUIAnimation multiplayerAnim;
	private MainSelectionUIAnimation mainSelectionAnim;
	private TrackSelUIAnimation trackSelectionAnim;
	public TrackSelUIAnimation trackSelectionDesertAnim;
	private BuggySelUIAnimation buggySelectionAnim;
	private CharacterSelUIAnimation characterSelectionAnim;
	private WeaponSelUIAnimation weaponSelectionAnim;
	private LoginUIAnimation loginAnim;
	private SettingUIAnimation settingAnim;

	private bool isFirstTime = true;
	private AsyncOperation async = null;
	[HideInInspector]
	public int _buggyPos;
	private int _characterPos = 0;
	private bool _charFlag = false;

	[Header("Camera Positions")]
	public Transform MainMenu_Pos;
	public Transform Player_Main_Mode_Pos;
	public Transform Player_Multi_Mode_Pos;
	public Transform Player_MultiPlayer_Mode_Pos;
	public Transform Environment_Selection_Pos;
	public Transform Main_Selection_Pos;
	public Transform Buggy_Selection_One_Pos;
	public Transform Buggy_Selection_Two_Pos;
	public Transform Buggy_Selection_Three_Pos;
	public Transform Buggy_Selection_Four_Pos;
	public Transform Character_Selection_One_Pos;
	public Transform Character_Selection_Two_Pos;
	public Transform Weapon_Selection_Pos;
	public Transform Track_Selection_Pos;
	public Transform Settings_Pos;

	[Header("Screens")]
	public AudioClip ClickSound;
	public AudioClip BackSound;
	public AudioClip BgSound;

	[Header("Screens")]
	public GameObject Main_Menu;
	public GameObject Player_Mode_Main;
	public GameObject Racing_Mode_Main;
	public GameObject Global_Racing_Mode_Main;
	public GameObject RaceWithScreen_Mode_Main;
	public GameObject Player_Mode_Multiplayer;
	public GameObject Environment_Selection;
	public GameObject Main_Selection;
	public GameObject Track_Selection;
	public GameObject Track_SelectionDesert;
	public GameObject Buggy_Selection;
	public GameObject Character_Selection;
	public GameObject Weapon_Selection;

	public GameObject Settings_Panel;
	public GameObject Main_Login_Panel;
	public GameObject Login_Panel;
	public GameObject Register_Panel;
	public GameObject StoreSreen;

	public GameObject LoadingPanel;
	public Image LoadingBar;


	[Header("Scipts")]

	public UpdateBarValues Bar_Values;

	private bool isMultiplayerSelected = false;

	void Start()
	{
      //  PlayerPrefs.SetInt(GameData.TUTORIALKEY, 0);
//		PlayerPrefs.DeleteAll ();
		Constants.isMultiplayerSelected = false;
		GameManager.Instance.playBgMusic (true, false, SoundManager.MAIN_MENU_BG);		// Starts Playing Main Menu BG Music
		camFlying = this.GetComponent<CameraFlying> ();
		mainMenuAnim = this.GetComponent<UIAnimationMainMenu> ();
		enviMenuAnim = this.GetComponent<UIAnimationEnvironmentSelection> ();
		loginAnim = this.GetComponent<LoginUIAnimation> ();
		playerModeAnim = this.GetComponent<PlayerModeUIAnimation> ();
		racingModeAnim = this.GetComponent<MultiPlayerModeUIAnimation> ();
		raceWithFirendModeAnim = this.GetComponent<MultiPlayerModeUIAnimation> ();
		globalraceModeAnim = this.GetComponent<MultiPlayerModeUIAnimation> ();
		multiplayerAnim = this.GetComponent<MultiPlayerModeUIAnimation> ();
		mainSelectionAnim = this.GetComponent<MainSelectionUIAnimation> ();
		trackSelectionAnim = this.GetComponent<TrackSelUIAnimation> ();
		buggySelectionAnim = this.GetComponent<BuggySelUIAnimation> ();
		weaponSelectionAnim = this.GetComponent<WeaponSelUIAnimation> ();
		characterSelectionAnim = this.GetComponent<CharacterSelUIAnimation> ();
		settingAnim = this.GetComponent<SettingUIAnimation> ();


		GlobalVariables.isAnimating = false;

		//StartCoroutine (Delay ());

		if (GameManager.Instance.chatClient != null) {
			GameManager.Instance.chatClient.SetOnlineStatus (ChatUserStatus.Online);
		}
        if (Constants.showLogin)
        {
            Constants.showLogin = false;
           
           
        }
        else
        {
            OpenMainMenuScreen();
        }
    }

	IEnumerator Delay()
	{
		yield return new WaitForSeconds (0.5f);

		if (RewardProperties.Instance.IsFbLoggedIn && Constants.isAutoLoginFirstTime) {
			Constants.isAutoLoginFirstTime = false;
			Constants.showLogin = false;
			OpenLoginScreen ();
			
		} else {
			if (Constants.showLogin) {
				Constants.showLogin = false;
				OpenLoginScreen ();
			} else {
				OpenMainMenuScreen ();
			}
		}
	}


	/// <summary>
	/// Start Race.
	/// </summary>
	public void Race()
	{
		int opt = GlobalVariables.ENVIRONMENT;
		Track_Selection.SetActive (false);
		Track_SelectionDesert.SetActive (false);
		LoadingPanel.SetActive (true);
		StartCoroutine (Loading (opt));

		GameManager.Instance.playBgMusic (false, false, SoundManager.MAIN_MENU_BG);
	}

	public void RaceMultiplayer(){
		GlobalVariables.FORREST_SCENE_SELECTED = false;
		Constants.isMultiplayerSelected = true;
		Constants.isPrivateModeSelected = false;
		Constants.joinCode = "";
		OpenEnvironmentSelectionScreen ();
//		LoadingPanel.SetActive (true);
//		StartCoroutine (Loading (1));
	}

	public void RaceMultiplayerPrivate(){
		GlobalVariables.FORREST_SCENE_SELECTED = false;
		Constants.isMultiplayerSelected = true;
		Constants.isPrivateModeSelected = true;
		Constants.joinCode = "";
		OpenEnvironmentSelectionScreen ();
//		LoadingPanel.SetActive (true);
//		StartCoroutine (Loading (1));
	}

	public GameObject eneterCodeInvalidText;

	public void RaceMultiplayerPrivateJoin(Text code){
		if (code.text.Length != 6) {
			eneterCodeInvalidText.SetActive (true);
			return;
		}

		eneterCodeInvalidText.SetActive (false);
		GlobalVariables.FORREST_SCENE_SELECTED = false;
		Constants.joinCode = code.text;
		Constants.isMultiplayerSelected = true;
		Constants.isPrivateModeSelected = true;
		LoadingPanel.SetActive (true);
		StartCoroutine (Loading (1));
	}

	public void BackButtonClickSound()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.BACK);
	}

	/// <summary>
	/// Opens the main menu screen.
	/// </summary>
	public void OpenMainMenuScreen()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		if (!GlobalVariables.isAnimating) 
		{
			if (isFirstTime) {
				isFirstTime = false;
			} else {
				camFlying.FlyCamera (MainMenu_Pos);
			}

			Player_Mode_Main.SetActive (false);
			Main_Login_Panel.SetActive (false);
			Settings_Panel.SetActive (false);
			Racing_Mode_Main.SetActive (false);
			//Global_Racing_Mode_Main.SetActive (false);

			loginAnim.ResetPositions ();
			playerModeAnim.ResetPositions ();
			racingModeAnim.ResetPositions ();
			settingAnim.ResetPositions ();

			Main_Menu.SetActive (true);
			mainMenuAnim.BeginAnimation ();
		}
	}


	public void OpenSettingsScreen()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		mainMenuAnim.ResetAnimationObjects ();

		Settings_Panel.SetActive (true);
		settingAnim.BeginAnimation ();
	}


	/// <summary>
	/// Opens the login screen.
	/// </summary>
	public void OpenLoginScreen()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		Login_Panel.SetActive (false);
		Register_Panel.SetActive (false);

		mainMenuAnim.ResetAnimationObjects ();
		loginAnim.ResetLogin ();
		loginAnim.ResetRegister ();

		Main_Login_Panel.SetActive (true);
		loginAnim.BeginAnimation ();
	}

	public void OpenStoreScreen()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);

		mainMenuAnim.ResetAnimationObjects ();

		StoreSreen.SetActive (true);
		//loginAnim.BeginAnimation ();
	}

	public void OpenLeaderboard()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);

		mainMenuAnim.ResetAnimationObjects ();

		//StoreSreen.SetActive (true);
		//loginAnim.BeginAnimation ();
	}


	public void Login()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		loginAnim.ResetPositions ();
		Main_Login_Panel.SetActive (false);

		Login_Panel.SetActive (true);
		loginAnim.LoginAnimation ();
	}


	public void Register()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		loginAnim.ResetPositions ();
		Main_Login_Panel.SetActive (false);

		Register_Panel.SetActive (true);
		loginAnim.RegisterAnimation ();
	}


	/// <summary>
	/// Opens the player mode.
	/// </summary>
	public void OpenPlayerMode()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		Environment_Selection.SetActive (false);
		Player_Mode_Multiplayer.SetActive (false);
		Main_Selection.SetActive (false);

		mainSelectionAnim.ResetPositions ();	

		multiplayerAnim.ResetPositions ();
		enviMenuAnim.ResetAnimationObjects ();
		mainMenuAnim.ResetAnimationObjects ();

		camFlying.FlyCamera (Player_Main_Mode_Pos);
		playerModeAnim.BeginAnimation ();
		Player_Mode_Main.SetActive (true);
	}
	public void OpenRacingMode()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		Environment_Selection.SetActive (false);
		Player_Mode_Multiplayer.SetActive (false);
		Main_Selection.SetActive (false);
		RaceWithScreen_Mode_Main.SetActive (false);
		Global_Racing_Mode_Main.SetActive (false);

		mainSelectionAnim.ResetPositions ();	
		raceWithFirendModeAnim.ResetPositions ();
		globalraceModeAnim.ResetPositions ();
		multiplayerAnim.ResetPositions ();
		enviMenuAnim.ResetAnimationObjects ();
		mainMenuAnim.ResetAnimationObjects ();

		camFlying.FlyCamera (Player_Main_Mode_Pos);
		racingModeAnim.BeginAnimation ();

		playerModeAnim.ResetPositions ();

		Racing_Mode_Main.SetActive (true);
	}

	public void OpenRaceWithFriendMode()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		Environment_Selection.SetActive (false);
		Player_Mode_Multiplayer.SetActive (false);
		Main_Selection.SetActive (false);
		Racing_Mode_Main.SetActive (false);

		mainSelectionAnim.ResetPositions ();	
		racingModeAnim.ResetPositions ();
		multiplayerAnim.ResetPositions ();
		enviMenuAnim.ResetAnimationObjects ();
		mainMenuAnim.ResetAnimationObjects ();

		camFlying.FlyCamera (Player_MultiPlayer_Mode_Pos);
		raceWithFirendModeAnim.BeginAnimation ();
		RaceWithScreen_Mode_Main.SetActive (true);
	}

	public void OpenGlobalRaceMode()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Main_Menu.SetActive (false);
		Environment_Selection.SetActive (false);
		Player_Mode_Multiplayer.SetActive (false);
		Main_Selection.SetActive (false);
		Racing_Mode_Main.SetActive (false);

		mainSelectionAnim.ResetPositions ();	
		racingModeAnim.ResetPositions ();
		multiplayerAnim.ResetPositions ();
		enviMenuAnim.ResetAnimationObjects ();
		mainMenuAnim.ResetAnimationObjects ();

		camFlying.FlyCamera (Player_MultiPlayer_Mode_Pos);
		globalraceModeAnim.BeginAnimation ();
		Global_Racing_Mode_Main.SetActive (true);
	}

	public void OpenMultiPlayerMode()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		Player_Mode_Main.SetActive (false);
		playerModeAnim.ResetPositions ();

		Player_Mode_Multiplayer.SetActive (true);
		multiplayerAnim.BeginAnimation ();
	}


	/// <summary>
	/// Opens the environment selection screen.
	/// </summary>
	public void OpenEnvironmentSelectionScreen()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Environment_Selection_Pos);

		Player_Mode_Main.SetActive (false);
		Main_Selection.SetActive (false);
		Track_Selection.SetActive (false);
		Track_SelectionDesert.SetActive (false);
		Racing_Mode_Main.SetActive (false);
		multiplayerAnim.ResetPositions ();

		trackSelectionAnim.ResetPositions ();
		trackSelectionDesertAnim.ResetPositions ();
		mainSelectionAnim.ResetPositions ();	
		playerModeAnim.ResetPositions ();

		Environment_Selection.SetActive (true);
		enviMenuAnim.BeginAnimation ();
	}

	public void SelectForestEnvironment(int option)
	{
        GameData.selectedEnvironment = 0;


        GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		GlobalVariables.ENVIRONMENT = option;
		GlobalVariables.FORREST_SCENE_SELECTED = true;
		GlobalVariables.DESERT_SCENE_SELECTED = false;
		OpenTrackSelection ();
	}

	public void SelectDesertEnvironment(int option)
	{
        GameData.selectedEnvironment = 1;

        GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		GlobalVariables.ENVIRONMENT = option;
		GlobalVariables.FORREST_SCENE_SELECTED = false;
		GlobalVariables.DESERT_SCENE_SELECTED = true;
		OpenTrackSelectionDesert ();
	}

	/// <summary>
	/// Opens the main selection screen.
	/// </summary>
	public void OpenMainSelectionScreen(bool isMultiplayer)
	{
        
		isMultiplayerSelected = isMultiplayer;
		Constants.isMultiplayerSelected = isMultiplayer;

		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Main_Selection_Pos);

		Environment_Selection.SetActive (false);
		Track_Selection.SetActive (false);
		Track_SelectionDesert.SetActive (false);
		Buggy_Selection.SetActive (false);
		Character_Selection.SetActive (false);
		Weapon_Selection.SetActive (false);
		Player_Mode_Main.SetActive (false);
		Main_Selection.SetActive (false);

		mainSelectionAnim.ResetPositions ();	
		playerModeAnim.ResetPositions ();

		enviMenuAnim.ResetAnimationObjects ();
		trackSelectionAnim.ResetPositions ();
		trackSelectionDesertAnim.ResetPositions ();
		buggySelectionAnim.ResetPositions ();
		characterSelectionAnim.ResetPositions ();
		weaponSelectionAnim.ResetPositions ();

		Main_Selection.SetActive (true);
		mainSelectionAnim.BeginAnimation ();
	}

	/// <summary>
	/// Opens the track selection.
	/// </summary>
	public void OpenTrackSelection()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Track_Selection_Pos);

		Main_Selection.SetActive (false);
		Environment_Selection.SetActive (false);
		enviMenuAnim.ResetAnimationObjects ();
		mainSelectionAnim.ResetPositions ();
		Track_Selection.SetActive (true);
		trackSelectionAnim.BeginAnimation ();
	}

	public void OnNextFromMainSelectionPanel(){
		if (isMultiplayerSelected) {
			OpenRacingMode ();
		} else {
			OpenEnvironmentSelectionScreen ();
		}
	}

	public void OnBackFromEnvAndMultiModeSelection(){
        Debug.Log(isMultiplayerSelected);
		OpenMainSelectionScreen (isMultiplayerSelected);
        GameData.data.Save();
    }

	public void OpenTrackSelectionDesert()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Track_Selection_Pos);

		Main_Selection.SetActive (false);
		Environment_Selection.SetActive (false);
		enviMenuAnim.ResetAnimationObjects ();
		mainSelectionAnim.ResetPositions ();
		Track_SelectionDesert.SetActive (true);
		trackSelectionDesertAnim.BeginAnimation ();
	}

	/// <summary>
	/// Opens the buggy selection panel.
	/// </summary>
	public void OpenBuggySelectionPanel()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Buggy_Selection_One_Pos);

		Main_Selection.SetActive (false);

		mainSelectionAnim.ResetPositions ();
		Buggy_Selection.SetActive (true);
		buggySelectionAnim.BeginAnimation ();
	}


	public void SelectforwardBuggy(int val)
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		_buggyPos += val;
		if (_buggyPos < 0) {
			_buggyPos = 3;
		} else if (_buggyPos > 3) {
			_buggyPos = 0;
		}

		if (_buggyPos == 0) 
		{
			camFlying.FlyCamera (Buggy_Selection_One_Pos);
		} 
		else if (_buggyPos == 1) 
		{
			camFlying.FlyCamera (Buggy_Selection_Two_Pos);
		} 
		else if (_buggyPos == 2) 
		{
			camFlying.FlyCamera (Buggy_Selection_Three_Pos);
		}
		else if (_buggyPos == 3) 
		{
			camFlying.FlyCamera (Buggy_Selection_Four_Pos);
		}

		Bar_Values.ResetValues (_buggyPos);
		Bar_Values.SetBuggyPosition (_buggyPos);
	}

	public void SelectbackwardBuggy(int val)
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		_buggyPos += val;
		if (_buggyPos < 0) {
			_buggyPos = 3;
		} else if (_buggyPos > 3) {
			_buggyPos = 0;
		}
		if (_buggyPos == 0) 
		{
			camFlying.FlyCamera (Buggy_Selection_One_Pos);
		} 
		else if (_buggyPos == 1) 
		{
			camFlying.FlyCamera (Buggy_Selection_Two_Pos);
		} 
		else if (_buggyPos == 2) 
		{
			camFlying.FlyCamera (Buggy_Selection_Three_Pos);
		}
		else if (_buggyPos == 3) 
		{
			camFlying.FlyCamera (Buggy_Selection_Four_Pos);
		}

		Bar_Values.ResetValues (_buggyPos);
		Bar_Values.SetBuggyPosition (_buggyPos);
	}

	/// <summary>
	/// Opens the character selection panel.
	/// </summary>
	public void OpenCharacterSelectionPanel()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		//camFlying.FlyCamera (Character_Selection_One_Pos);
		camFlying.FlyCamera (Character_Selection_Two_Pos);
		Main_Selection.SetActive (false);

		mainSelectionAnim.ResetPositions ();
		Character_Selection.SetActive (true);
		characterSelectionAnim.BeginAnimation ();
	}

	public void SelectNextCharacter(int val)
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		if (_charFlag) {
			camFlying.FlyCamera (Character_Selection_Two_Pos);
			_charFlag = false;

		} else {
			camFlying.FlyCamera (Character_Selection_One_Pos);
			_charFlag = true;
		}
//		int temp = _characterPos;
//		_characterPos += val;
//		if (_characterPos <= 1 && _characterPos >= 0) 
//		{
//			if (_characterPos == 0) 
//			{
//				camFlying.FlyCamera (Character_Selection_Two_Pos);
//			} 
//			else if (_characterPos == 1) 
//			{
//				camFlying.FlyCamera (Character_Selection_One_Pos);
//			} 
//		} 
//		else 
//		{
//			_characterPos = temp;
//		}
	}


	/// <summary>
	/// Opens the weapon selection panel.
	/// </summary>
	public void OpenWeaponSelectionPanel()
	{
		GameManager.Instance.ChangeSoundState (GameManager.SoundState.CLICK);
		camFlying.FlyCamera (Weapon_Selection_Pos);
		Main_Selection.SetActive (false);
		mainSelectionAnim.ResetPositions ();
		Weapon_Selection.SetActive (true);
		weaponSelectionAnim.BeginAnimation ();
	}

    
	IEnumerator Loading(int env)
	{
		yield return new WaitForSeconds (0.5f);
		if (GameData.selectedEnvironment == 0) 
		{
            //async = Application.LoadLevelAsync ("ForestBeachEnv");

            Debug.Log(Constants.isMultiplayerSelected);
            Debug.Log(Constants.isPrivateModeSelected);
            if (Constants.isMultiplayerSelected)
            {
                if (Constants.isPrivateModeSelected)
                {
                    SceneManager.LoadScene("PrivateMultiplayerConnectionScene");
                }
                else
                {
                    SceneManager.LoadScene("MultiplayerConnectionScene");
                }
            }
            else
            {
                if (GameData.trackNo < 3)
                {
                    Debug.Log("in here1");
                    GameData.isDay = true;
                    async = SceneManager.LoadSceneAsync("ForestBeachEnvLoader");
                   
                }
                else
                {
                    Debug.Log("in here2");
                    GameData.isDay = false;
                    async = SceneManager.LoadSceneAsync("ForestBeachEnvNightNew");
                    
                }

              
            }
		}
		else if (GameData.selectedEnvironment == 1)
        {
            if (Constants.isMultiplayerSelected)
            {
                if (Constants.isPrivateModeSelected)
                {
                    SceneManager.LoadScene("PrivateMultiplayerConnectionScene");
                }
                else
                {
                    SceneManager.LoadScene("MultiplayerConnectionScene");
                }
            }
            else
            {

                if (GameData.trackNo < 3)
                {
                    GameData.isDay = true;
                    async = Application.LoadLevelAsync("desert_01");
                }
                else
                {
                    GameData.isDay = false;
                    async = Application.LoadLevelAsync("desert_02night");
                }


            }
        }

        if (async != null)
        {
            while (!async.isDone)
            {
                LoadingBar.fillAmount = async.progress;
                yield return null;
            }
        }
	}
}
