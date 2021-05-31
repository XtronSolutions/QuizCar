using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public GameObject waterSplash;
//	public FBManager _fbManager;
	bool iswaterSplashRunning;
	private AudioSource audioSource;
	private MenuManager _mManager;

	void Start()
	{
		Constants.isMultiplayerSelected = false;
		if(this.GetComponent<AudioSource> ()!=null)
		audioSource = this.GetComponent<AudioSource> ();

//		_fbManager = GetComponent<FBManager> ();
		_mManager = GameObject.Find ("MenuManager").GetComponent<MenuManager> ();

	}

	public void LoginWithFacebook(){
//		_fbManager.LogInFacebook ();
	}

	public void SignOutFromFaceBook(){
//		_fbManager.SignOut ();
	}

	public void OnFacebookLoginSuccessfull(){
		_mManager.Login_Panel.SetActive (false);
		_mManager.Register_Panel.SetActive (false);
		_mManager.OpenMainMenuScreen();

	}
	public void TurnOnWaterSplash(){

		waterSplash.SetActive (true);
	}


	public void RestartGame(){
		GlobalVariables.isPause = false;
		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void OnTriggerEnter(Collider col){

		//Debug.Log ("col.tag="+col.tag);
		if (col.tag.Equals ("Player")) {
			TurnOnWaterSplash();
			audioSource.Play ();
			if(!iswaterSplashRunning)
			StartCoroutine(TurnOffSplash());
		}
	}

	void OnTriggerStay(Collider col){
		
		//Debug.Log ("col.tag="+col.tag);
		if (col.tag.Equals ("Player")) {
			TurnOnWaterSplash();
			if(!iswaterSplashRunning)
			StartCoroutine(TurnOffSplash());
		}
	}


	IEnumerator TurnOffSplash(){
		iswaterSplashRunning = true;
		yield return new WaitForSeconds (1.5f);
		waterSplash.SetActive (false);
		iswaterSplashRunning = false;
		yield return null;
	}

	public void LoadMultiplayerConnectionScene(){
		Constants.isMultiplayerSelected = true;
		SceneManager.LoadSceneAsync ("MultiplayerConnectionScene");
	}
}
