using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class DesertUIManager : MonoBehaviour 
{
	public GameObject Pause_Screen;
	public GameObject loading_screen;

    public GameObject coinImage;

    public GameObject multiplayerPause, singlePlayerPause;
    void Awake()
    {
        loading_screen.SetActive(true);
        Invoke("DisableLoading", 1f);
    }
    void DisableLoading()
    {
        loading_screen.SetActive(false);

        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            Time.timeScale = 0;
        }
    }
	void Start()
	{
		GameManager.Instance.playBgMusic (true, false, SoundManager.GAMEPLAY_BG);
	}

	/// <summary>
	/// Restarts the game.
	/// </summary>
	public void RestartGame()
	{
		Time.timeScale = 1;
		GlobalVariables.isPause = false;
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
	}

	/// <summary>
	/// Gos to main menu.
	/// </summary>
	public void GoToMainMenu()
	{
        
        Time.timeScale = 1;
		SceneManager.LoadScene ("MainMenu");
	}

	public void GoToMainMenuScene()
	{
		Time.timeScale = 1;
		loading_screen.SetActive (true);
        DontDestroy dd = FindObjectOfType<DontDestroy>();
        if(dd!=null)
        Destroy(dd.gameObject);

		if (Constants.isMultiplayerSelected) {
			PhotonNetwork.Disconnect ();
		}

		Resources.UnloadUnusedAssets ();
        //SceneManager.LoadScene ("MainMenuScene");
        SceneManager.LoadScene("MainMenu");
    }

	public void GoToMainMenuSceneDesert(){
		Time.timeScale = 1;
		loading_screen.SetActive (true);
        //SceneManager.LoadScene ("MainMenuScene");
        SceneManager.LoadScene("MainMenu");
    }

	/// <summary>
	/// Pauses the game.
	/// Setting global variable isPause to TRUE
	/// Settng Timescale to 0.
	/// </summary>
	public void PauseGame()
	{
		if (!GlobalVariables.isPause && !GlobalVariables.isGameEnd) 
		{
			Pause_Screen.SetActive (true);
			Time.timeScale = 0;
			GlobalVariables.isPause = true;
      
                multiplayerPause.SetActive(Constants.isMultiplayerSelected);
                singlePlayerPause.SetActive(!Constants.isMultiplayerSelected);
           
		}
	}

	public void PauseGameOnline()
	{
		Pause_Screen.SetActive (true);
	}

	/// <summary>
	/// Pauses the game.
	/// Setting global variable isPause to FALSE
	/// Settng Timescale to 1.
	/// </summary>
	public void ResumeGame()
	{
		Pause_Screen.SetActive (false);
		Time.timeScale = 1;
		GlobalVariables.isPause = false;
	}

	/// <summary>
	/// Quits the game.
	/// </summary>
	public void QuitGame()
	{
		Application.Quit ();
	}
}
