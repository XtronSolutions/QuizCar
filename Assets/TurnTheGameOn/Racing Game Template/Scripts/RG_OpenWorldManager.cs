using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class RG_OpenWorldManager : MonoBehaviour {

	public InputData inputData;
	public AudioData audioData;
	public PlayableVehicles playableVehicles;
	public GameObject pauseWindow;
	public GameObject spawnPoint;
	private bool gamePaused;
	public GameObject pauseButton;
	public GameObject cameraSwitchButton;
	public GameObject disableSpawnPoint;
	private string gameMode;
	public GameObject pauseText;
	public GameObject restartButton;
	public GameObject[] spawnPointMesh;
	GameObject newVehicle;
	private GameObject audioContainer;
	private GameObject emptyObject;
	private AudioSource raceMusicAudioSource;

	void Start () {
		if (Application.isPlaying) {
			audioContainer = new GameObject ();
			audioContainer.name = "Audio Container";
			AudioMusic ();
		}
		gameMode = PlayerPrefs.GetString ("Game Mode");
		if (gameMode == "SINGLE PLAYER") {
			Time.timeScale = 1.0f;
			SpawnVehicles (playableVehicles.currentVehicleNumber);
			Invoke ("EnableUserControl", 0.25f);
		} else {
			pauseText.SetActive (false);
			restartButton.SetActive (false);
		}
		for(int i = 0; i < spawnPointMesh.Length;i++){
			spawnPointMesh [i].SetActive (false);
		}

	}

	void EnableUserControl(){
		newVehicle.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>().enabled = true;
	}

	void Update(){
		if (Input.GetKeyDown (inputData.pauseKey)) {
			PauseButton ();
		}
		if (Application.isPlaying) {
			if (audioData.music.Length > 0) {
				if (!raceMusicAudioSource.isPlaying)
					PlayNextAudioTrack ();
			}
		}
	}

	public void LoadGarageButton(){
		if (Application.isPlaying) {
			if (audioData.music.Length > 0) {
				if (!raceMusicAudioSource.isPlaying)
					PlayNextAudioTrack ();
			}
		}
		if (gameMode == "MULTIPLAYER") {
			//RG_NetworkManagerHUD networkManagerHUD = GameObject.Find ("LobbyManager").GetComponent<RG_NetworkManagerHUD> ();
			//networkManagerHUD.Button_BackLobby ();



	//		if (lobbyHUDReference.startGameButton.activeInHierarchy) {
	//			networkLobbyManager.matchMaker.DestroyMatch ((NetworkID)networkLobbyManager.matchInfo.networkId, OnDestroyMatch); 
	//			networkLobbyManager.StopHost();
	//			networkLobbyManager.StopMatchMaker();
	//			Debug.Log ("Destroy Match");
	//		} else {
	//			networkLobbyManager.StopClient();
	//			networkLobbyManager.StopMatchMaker();
	//			Debug.Log ("Leave Match");
	//		}
		}else{
			SceneManager.LoadScene("Garage");    
		}
		//DestroyImmediate (GameObject.Find ("LobbyManager"));
		    
    }

	public void RestartButton(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

	public void PauseButton(){	
		gameMode = PlayerPrefs.GetString ("Game Mode");
		if (gameMode == "SINGLE PLAYER") {
			if (gamePaused) {					
				pauseWindow.SetActive (false);
				gamePaused = false;
				Time.timeScale = 1.0f;
				AudioSource[] allAudioSources = FindObjectsOfType (typeof(AudioSource)) as AudioSource[];
				for (int i = 0; i < allAudioSources.Length; i++) {
					if(allAudioSources[i].enabled)      allAudioSources [i].Play ();
				}			
			} else if (!gamePaused) {			
				gamePaused = true;
				Time.timeScale = 0.0f;
				AudioSource[] allAudioSources = FindObjectsOfType (typeof(AudioSource)) as AudioSource[];
				for (int i = 0; i < allAudioSources.Length; i++) {
					allAudioSources [i].Pause ();
				}
				pauseWindow.SetActive (true);				
			}
		} else {
			if (gamePaused) {					
				pauseWindow.SetActive (false);
				gamePaused = false;		
			} else if (!gamePaused) {			
				gamePaused = true;
				pauseWindow.SetActive (true);				
			}
		}



	}

	public void SpawnVehicles(int vehicle){
		
		spawnPoint.SetActive (false);
		newVehicle = Instantiate(playableVehicles.vehicles[playableVehicles.currentVehicleNumber], spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject; //,GlobalControl.worldUpDir)) as GameObject;
		newVehicle.transform.rotation = spawnPoint.transform.rotation;
		newVehicle.name = "Player";
		disableSpawnPoint.SetActive (false);
		//miniMapCamera.SetActive (true);
    }

	void AudioMusic(){
		if (audioData.music.Length > 0) {
			emptyObject = new GameObject ("Audio Clip: Music");
			emptyObject.transform.parent = audioContainer.transform;
			emptyObject.AddComponent<AudioSource> ();
			raceMusicAudioSource = emptyObject.GetComponent<AudioSource> ();
			audioData.currentAudioTrack = 0;
			raceMusicAudioSource.clip = audioData.music [audioData.currentAudioTrack];
			raceMusicAudioSource.loop = false;
			raceMusicAudioSource.Play ();
		}
	}

	void PlayNextAudioTrack(){
		if (audioData.musicSelection == AudioData.MusicSelection.ListOrder) {
			if (audioData.currentAudioTrack < audioData.music.Length - 1) {
				audioData.currentAudioTrack += 1;
			} else {
				audioData.currentAudioTrack = 0;
			}
		}else if(audioData.musicSelection == AudioData.MusicSelection.Random){
			audioData.currentAudioTrack = UnityEngine.Random.Range ( 0, audioData.music.Length );
		}
		raceMusicAudioSource.clip = audioData.music [audioData.currentAudioTrack];
		raceMusicAudioSource.Play ();
	}

}