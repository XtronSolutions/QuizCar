using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public class ManagerReference{
	[Header("Game Object Reference")]
	public GameObject pauseWindow;
	public GameObject raceCompleteWindow;
	public GameObject racePositionWindow;
	public GameObject nextLevelButton;
	//public GameObject cameraSwitchButton;
	public GameObject pauseButton;
	//public GameObject wrongWayWarning;
	//public GameObject miniMap;
	//public GameObject miniMapBorder;
	[Header("Waypoint References")]
	public Transform WaypointArrow;
	public UnityStandardAssets.Utility.WaypointCircuit waypointCircut;
	[Header("UI Text Reference")]
	public Text[] positionText;
	public RectTransform[] positionObject;
	public Vector2[] displayPositions;
	public Text playerPositionText;
	public Text playerLapText;
	public Text timerText;
	public Text completeTime;
	public Text finalStanding;
	public Text rewardText;
}

[System.Serializable]
public class RacerInfo{
	public string racerName;
	public GameObject racer;
	public int position = 1;
	public int lap = 1;
	public bool finishedRace;
	public int finalStanding;
	public float finishTime;
	public Transform currentWaypoint;
	public int nextWP;
	public float checkpointDistance;
	public float positionScore;
	public float distanceScore;
	public float totalScore;
	public RG_AIHelper aIHelper;
	public UnityStandardAssets.Vehicles.Car.CarAIControl aIController;
	public UnityStandardAssets.Utility.WaypointProgressTracker waypointController;
}

[ExecuteInEditMode][RequireComponent (typeof (UnityStandardAssets.Utility.WaypointCircuit))]
public class RG_SceneManager : MonoBehaviour {
	public enum Switch { Off, On }

	public RaceData raceData;
	public PlayableVehicles playableVehicles;
	public OpponentVehicles opponentVehicles;
	public AudioData audioData;
	public InputData inputData;
	[Range(0f,60f), Tooltip("Set the amount of time to be counted down before the race starts.")]
	public float readyTime;
	public bool canCalculatePosition;
	public RacerInfo[] racerInfo;
	public GameObject[] spawnPoints;
	public ManagerReference managerReference;
	[Tooltip("Configure mode should only be turned on when you're ready to update your waypoint settings.")]
	public Switch configureMode;
	[Range(1,600), 
		Tooltip("Set the number of waypoints for this scene.")]
	public int TotalWaypoints;
	public Transform[] Waypoints;
	private RG_Waypoint[] waypointScripts;
	private GameObject audioContainer;
	private GameObject emptyObject;
	private AudioSource raceMusicAudioSource;
    //UnityStandardAssets.Vehicles.Car.CarUserControl playerController;
    vehicleHandling playerController;

    private float gameTime;
	private string timeString;
	private bool wrongWay;
	private bool gamePaused;
	private bool gameStarted;
	private bool readyTimer = true;
	public bool countUp;
	private bool countDown = true;
	private bool playedRaceTimerAudio;
	int[] toSort;
	float[] toSort2;

	void Awake(){
		if (Application.isPlaying) {
			Array.Resize (ref racerInfo, raceData.numberOfRacers[raceData.raceNumber]);
			Array.Resize (ref toSort, raceData.numberOfRacers[raceData.raceNumber]);
			Array.Resize (ref toSort2, raceData.numberOfRacers[raceData.raceNumber]);
		}
	}

	void Start () {
		if (Application.isPlaying) {
			Invoke ("StartGame", 0.3f);
		}
		Time.timeScale = 1.0f;
	}
	void StartGame(){
		for (int i = 0; i < raceData.numberOfRacers[raceData.raceNumber]; i++) {
			managerReference.positionText [i].gameObject.SetActive (true);
			racerInfo [i].position = i + 1;
		}
		GetPlayerData ();
//		SpawnVehicles (raceData.vehicleNumber);
//		audioContainer = new GameObject ();
//		audioContainer.name = "Audio Container";
//		AudioMusic ();
		gameStarted = true;
		canCalculatePosition = true;
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

	public void GetPlayerData(){
		if(PlayerPrefs.GetString("Audio", "ON") == "ON") AudioListener.pause = false;
		if(PlayerPrefs.GetString("Audio", "ON") == "OFF") AudioListener.pause = true;
		racerInfo [0].racerName = playableVehicles.playerName;
		raceData.raceNumber = PlayerPrefs.GetInt ("Race Number", 0);


		raceData.raceLaps[raceData.raceNumber] = PlayerPrefs.GetInt ("Race " + raceData.raceNumber.ToString() + " Laps", raceData.raceLaps[raceData.raceNumber]);


		raceData.vehicleNumber = PlayerPrefs.GetInt ("Vehicle Number", 0);
		raceData.currency = PlayerPrefs.GetInt ("Currency");
		//raceData.raceRewards[0] = PlayerPrefs.GetInt ("Race Reward1", raceData.raceRewards[0]);
		//raceData.raceRewards[1] = PlayerPrefs.GetInt ("Race Reward2", raceData.raceRewards[1]);
		//raceData.raceRewards[2] = PlayerPrefs.GetInt ("Race Reward3", raceData.raceRewards[2]);
		raceData.bestTime = PlayerPrefs.GetFloat ("Best Time" + raceData.raceNumber.ToString(), 9999.99f);
		//bestLapTime = PlayerPrefs.GetFloat ("Best Lap Time" + raceDetails.raceNumber.ToString(), 9999.99f);
	}
		
	public void LoadNextLevelButton(){
		int temp = raceData.raceNumber + 1;
		SceneManager.LoadScene(temp.ToString() + raceData.raceNames[raceData.raceNumber + 1]);
	}
	public void LoadGarageButton(){
		SceneManager.LoadScene ("Garage");
	}
	public void RestartButton(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
	public void PauseButton(){	
		if(gamePaused){					
			managerReference.pauseWindow.SetActive (false);
			gamePaused = false;                                                                                                           
			Time.timeScale = 1.0f;
			AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
			for(int i=0; i < allAudioSources.Length; i++){
				if(allAudioSources[i].enabled){
					allAudioSources[i].Play();
				}
			}			
		}
		else if(!gamePaused){			
			gamePaused = true;
			Time.timeScale = 0.0f;
			AudioSource[] allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
			for(int i=0; i < allAudioSources.Length; i++){			allAudioSources[i].Pause();			}
			managerReference.pauseWindow.SetActive (true);				
		}
	}

	public void SpawnVehicles(int vehicle){
		GameObject newVehicle;
		gameTime = readyTime;
		readyTimer = true;
		countDown = true;
		spawnPoints[0].SetActive (false);
		newVehicle = Instantiate(playableVehicles.vehicles[raceData.vehicleNumber], spawnPoints[0].transform.position, spawnPoints[0].transform.rotation) as GameObject;
		newVehicle.transform.rotation = spawnPoints[0].transform.rotation;
        //GameObject.FindObjectOfType<DynamicShadowProjector.DrawTargetObject>().target = newVehicle.transform;
		//newVehicle.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>().enabled = false;
		racerInfo [0].racer = newVehicle;
		racerInfo[0].racer.name = racerInfo[0].racerName;
		racerInfo [0].lap = 1;
		playerController = newVehicle.GetComponent<vehicleHandling>();
        racerInfo[0].aIHelper = racerInfo[0].racer.GetComponent<RG_AIHelper>();
        racerInfo[0].aIController = racerInfo[0].racer.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
        racerInfo[0].waypointController = racerInfo[0].racer.GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker>();
        racerInfo [0].currentWaypoint = Waypoints [0];
		Debug.Log (raceData.numberOfRacers[raceData.raceNumber]);
		for(int i = 1; i < raceData.numberOfRacers[raceData.raceNumber]; i ++){
			spawnPoints[i].SetActive (false);
            int opponentVehicleIndex = UnityEngine.Random.Range(0, raceData.numberOfRacers[raceData.raceNumber]);

            newVehicle = Instantiate(opponentVehicles.vehicles[opponentVehicleIndex], spawnPoints[i].transform.position, spawnPoints[i].transform.rotation) as GameObject;
			newVehicle.transform.rotation = spawnPoints[i].transform.rotation;
			racerInfo[i].racer = newVehicle;
			racerInfo[i].racerName = opponentVehicles.opponentNames[i - 1];
			racerInfo[i].racer.name = racerInfo[i].racerName;
			racerInfo [0].lap = 1;
			if(racerInfo[i].racer.GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker>()){
				racerInfo[i].aIHelper = racerInfo[i].racer.GetComponent<RG_AIHelper>();
				racerInfo[i].aIHelper.opponentNumber = i;
				racerInfo[i].aIController = racerInfo[i].racer.GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl>();
				racerInfo [i].waypointController = racerInfo [i].racer.GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker> ();
				racerInfo [i].positionScore = 0;
				racerInfo [i].currentWaypoint = Waypoints [0];
			}
		}
		newVehicle = null;
		for (int i = 0; i < spawnPoints.Length; i++) {
			spawnPoints [i].SetActive (false);
		}
	}

    float waitTime = 0;
	void Update () {
		#if UNITY_EDITOR
		if(!Application.isPlaying){			CalculateWaypoints ();		}
		#endif

		if (gameStarted) {
			if (Application.isPlaying) {
				if (audioData.music.Length > 0) {
					if (!raceMusicAudioSource.isPlaying)
						PlayNextAudioTrack ();
				}
			}
			if (Input.GetKeyDown (inputData.pauseKey)) {
				PauseButton ();
			}
			managerReference.playerPositionText.text = racerInfo [0].position.ToString() + "/" + raceData.numberOfRacers[raceData.raceNumber].ToString();
			managerReference.playerLapText.text = "Lap     " + racerInfo[0].lap.ToString() + " / " + raceData.raceLaps[raceData.raceNumber].ToString();
			if(readyTimer && gameTime <= 0)	{
				readyTimer = false; countDown = false; gameTime = 0; countUp = true;
				for (int i = 1; i < racerInfo.Length; i++){
					racerInfo[i].aIController.enabled = true;
				}
                playerController.enabled = true;
            }
			if(countUp)			gameTime += 1 * Time.deltaTime;
			if(countDown){
				gameTime -= 1 * Time.deltaTime;
				if(gameTime <= 2 && !playedRaceTimerAudio){
					playedRaceTimerAudio = true;
					GameObject tempObject = Instantiate(Resources.Load ("Audio Clip - Race Timer")) as GameObject;
					tempObject.name = "Audio Clip - Race Timer";
					tempObject = null;
				}
			}
			GenTimeSpanFromSeconds( (double) gameTime );
			for (int i = 0; i < racerInfo.Length; i ++) {
				if(!racerInfo[i].finishedRace){
					if(racerInfo[i].lap >= raceData.raceLaps[raceData.raceNumber] + 1){
						racerInfo[i].finishedRace = true;
						racerInfo[i].finishTime = gameTime;
						racerInfo[i].finalStanding = racerInfo[i].position;
                      
                        Debug.Log("Racer " + i + " finished the race.");
					}
				}
			}
			//if (managerReference.WaypointArrow == null) {
			//	GameObject waypointArrow = GameObject.Find ("Waypoint Arrow");
			//	if (waypointArrow != null) {
			//		managerReference.WaypointArrow = waypointArrow.transform;
			//	}
			//} else {
				//managerReference.WaypointArrow.LookAt (racerInfo [0].currentWaypoint);
				Vector3 forward = racerInfo [0].racer.transform.TransformDirection (Vector3.forward);
				Vector3 toOther = racerInfo [0].currentWaypoint.localPosition - racerInfo [0].racer.transform.localPosition;
                //if(!racerInfo[0].racer.GetComponent<vehicleHandling>().wheelColliderFL.isGrounded && !racerInfo[0].racer.GetComponent<vehicleHandling>().wheelColliderFR.isGrounded
                //    && !racerInfo[0].racer.GetComponent<vehicleHandling>().wheelColliderFR.isGrounded && !racerInfo[0].racer.GetComponent<vehicleHandling>().wheelColliderRR.isGrounded)
    //            if (Vector3.Dot(forward, toOther) < 0)
    //            {
				//	wrongWay = true;
				//	//managerReference.wrongWayWarning.SetActive (true);
				//} else {
				//	wrongWay = false;
    //                //managerReference.wrongWayWarning.SetActive (false);
				//}
			//}
            //if (wrongWay)
            //{
            //    waitTime += Time.deltaTime;
            //}
            //else
            //{
            //    waitTime = 0;
            //}
            //if (waitTime > 1.5f)
            //{
            //    //RespawnCar();
            //    waitTime = 0;
            //}
            

            if (canCalculatePosition)
                CalculateRacerPositions ();
		}
	}
    public void RespawnCar()
    {
        racerInfo[0].racer.transform.position += Vector3.up;
        racerInfo[0].racer.transform.position = racerInfo[0].currentWaypoint.position;
        racerInfo[0].racer.transform.rotation = racerInfo[0].currentWaypoint.localRotation;
        racerInfo[0].racer.transform.Rotate(0f, 180f, 0f);
        racerInfo[0].racer.GetComponent<Rigidbody>().velocity = Vector3.zero;

    }
	public void ChangeTarget(int racerNumber, int waypointNumber){
        //Debug.Log(racerInfo[racerNumber].nextWP);
		if (Application.isPlaying && waypointNumber == racerInfo[racerNumber].nextWP + 1) {
			if(racerNumber > 0){	racerInfo[racerNumber].aIHelper.ResetTimer();		}
			float check = racerInfo[racerNumber].nextWP;
			if (check < TotalWaypoints) {
				racerInfo[racerNumber].nextWP += 1;
				racerInfo[racerNumber].positionScore += 2;
				if(racerNumber == 0)
					//racerInfo[racerNumber].currentWaypoint.gameObject.GetComponent<MeshRenderer>().enabled = false;
				if (racerInfo [racerNumber].nextWP != TotalWaypoints) {
					racerInfo [racerNumber].currentWaypoint = Waypoints [racerInfo [racerNumber].nextWP];
				} else {
					racerInfo[racerNumber].nextWP = 0;
					if (raceData.raceLaps[raceData.raceNumber] > racerInfo [racerNumber].lap) {
						racerInfo[racerNumber].nextWP = 0;
						racerInfo [racerNumber].currentWaypoint = Waypoints [racerInfo [racerNumber].nextWP];
						racerInfo [racerNumber].lap += 1;
						racerInfo[racerNumber].positionScore += 100;
					}else if(!racerInfo[racerNumber].finishedRace){
						racerInfo[racerNumber].finishedRace = true;
						racerInfo[racerNumber].finalStanding = racerInfo[racerNumber].position;
						if(racerNumber == 0){
                                racerInfo[0].racer.GetComponent<Rigidbody>().isKinematic = true;
                                //	racerInfo[0].aIHelper.enabled = true;
                                //managerReference.miniMapBorder.SetActive(false);
                                //managerReference.miniMap.SetActive(false);
                                //racerInfo[0].aIController.enabled = true;
                                //racerInfo [0].waypointController.enabled = true;
                                playerController.enabled = false;
                                int temp = raceData.raceNumber + 1;
							managerReference.finalStanding.text = "Final Standing: " + racerInfo[0].position.ToString();
							racerInfo[0].finishTime = gameTime;
							TimeSpan    interval = TimeSpan.FromSeconds( racerInfo[0].finishTime );			
							string      timeInterval = string.Format("{0:D2}:{1:D2}",  interval.Minutes, interval.Seconds);
							timeString = timeInterval + "." + interval.Ticks.ToString ().Substring (1,3);
							managerReference.completeTime.text = "Finish Time: " + timeString;
							if(racerInfo[0].finishTime < raceData.bestTime){
								PlayerPrefs.SetFloat ("Best Time" + raceData.raceNumber.ToString(), racerInfo[0].finishTime);
							}
							if(racerInfo[0].finalStanding == 1){
								raceData.currency += raceData.raceRewards[0];
								PlayerPrefs.SetInt ("Currency", raceData.currency);
								managerReference.rewardText.text = "Reward: $" + raceData.raceRewards[0].ToString();
								if (PlayerPrefs.GetString ("AutoUnlockNextRace") == "TRUE" && raceData.raceNumber != raceData.raceNames.Length - 1) {
									PlayerPrefs.SetInt ("Race Number", temp);
									managerReference.nextLevelButton.SetActive(true);
									PlayerPrefs.SetString ("RaceLock" + raceData.raceNames[raceData.raceNumber +1], "UNLOCKED");
								}
								else {	managerReference.nextLevelButton.SetActive(false);	}
							}
							if(racerInfo[0].finalStanding == 2){
								raceData.currency += raceData.raceRewards[1];
								PlayerPrefs.SetInt ("Currency", raceData.currency);
								managerReference.rewardText.text = "Reward: $" + raceData.raceRewards[1].ToString();
								if (PlayerPrefs.GetString ("AutoUnlockNextRace") == "TRUE" && raceData.raceNumber != raceData.raceNames.Length - 1) {
									PlayerPrefs.SetInt ("Race Number", temp);
									managerReference.nextLevelButton.SetActive(true);
									PlayerPrefs.SetString ("RaceLock" + raceData.raceNames[raceData.raceNumber +1], "UNLOCKED");
								}
								else {	managerReference.nextLevelButton.SetActive(false);	}
							}
							if(racerInfo[0].finalStanding == 3){
								raceData.currency += raceData.raceRewards[2];
								PlayerPrefs.SetInt ("Currency", raceData.currency);
								managerReference.rewardText.text = "Reward: $" + raceData.raceRewards[2].ToString();
								if (PlayerPrefs.GetString ("AutoUnlockNextRace") == "TRUE" && raceData.raceNumber != raceData.raceNames.Length - 1) {
									PlayerPrefs.SetInt ("Race Number", temp);
									managerReference.nextLevelButton.SetActive(true);
									PlayerPrefs.SetString ("RaceLock" + raceData.raceNames[raceData.raceNumber +1], "UNLOCKED");
								}
								else {	managerReference.nextLevelButton.SetActive(false);	}
							}
							managerReference.raceCompleteWindow.SetActive(true);
							managerReference.racePositionWindow.SetActive(false);
							managerReference.WaypointArrow.gameObject.SetActive(false);
						}
					}
				}
				if(racerNumber == 0)
					//racerInfo[racerNumber].currentWaypoint.gameObject.GetComponent<MeshRenderer>().enabled = true;
				if (racerInfo[racerNumber].nextWP == TotalWaypoints) {
			//		racerInfo[racerNumber].nextWP = 0;
			//		ChangeTarget (racerNumber, 0);
			//		racerInfo[racerNumber].lap += 1;
			//		racerInfo[racerNumber].positionScore += 100;

				}
			}
		}
	}

	void CalculateRacerPositions() {
		canCalculatePosition = false;

		if (Application.isPlaying && gameStarted) {
			float distance;
			for (int i = 0; i < racerInfo.Length; i ++) {
				racerInfo [i].checkpointDistance = Vector3.Distance (racerInfo [i].racer.transform.position, racerInfo [i].currentWaypoint.position);
				distance = (racerInfo [i].checkpointDistance / 500);
				racerInfo [i].distanceScore = - (distance * distance);
				racerInfo [i].totalScore = racerInfo [i].positionScore + racerInfo [i].distanceScore;
				for(int i2 = 0; i2 < raceData.numberOfRacers[raceData.raceNumber]; i2++){
					toSort[i2] = racerInfo [i2].position;
				}
				foreach (int sort in toSort.OrderBy(sorted=>sorted)) {
					if (racerInfo [i].position == sort) {
						managerReference.positionText [sort - 1].text = sort + "   " + racerInfo [i].racerName;
					}
				}
			}
			for(int i2 = 0; i2 < raceData.numberOfRacers[raceData.raceNumber]; i2++){
				toSort2[i2] = racerInfo [i2].totalScore;
			}
			for (int i = 0; i < racerInfo.Length; i ++) {
				var sort2 = toSort2.OrderByDescending(sorted=>sorted).ToArray();
				float scoreCheck = racerInfo[i].totalScore;
				for (int i2 = 0; i2 < toSort2.Length; i2 ++) {
					if (scoreCheck == sort2 [i2]) {
						racerInfo [i].position = i2 + 1;
					}
				}
			}
		}
		//asdad
		if(raceData.numberOfRacers[raceData.raceNumber] > 8){
			if (racerInfo [0].position > 8) {
				for (int i = 6; i < raceData.numberOfRacers[raceData.raceNumber] + 1; i++) {
					if (racerInfo [0].position != i) {
						managerReference.positionObject [i - 1].gameObject.SetActive (false);

					} else {
						Vector3 tPos;
						tPos = managerReference.positionObject [i - 1].localPosition;
						tPos.y = -230;
						managerReference.positionObject [i - 1].localPosition = tPos;
						managerReference.positionObject [i - 1].gameObject.SetActive (true);

						tPos = managerReference.positionObject [i - 2].localPosition;
						tPos.y = -160;
						managerReference.positionObject [i - 2].localPosition = tPos;
						//managerReference.positionObject [i - 2].transform.localPosition.y = -160;
						managerReference.positionObject [i - 2].gameObject.SetActive (true);

						tPos = managerReference.positionObject [i - 3].localPosition;
						tPos.y = -90;
						managerReference.positionObject [i - 3].localPosition = tPos;
						//managerReference.positionObject [i - 2].transform.localPosition.y = -90;
						managerReference.positionObject [i - 3].gameObject.SetActive (true);
					}
				}
			} else {
				for(int i = 0; i < raceData.numberOfRacers[raceData.raceNumber]; i++){
					if (i <= 7) {
						Vector3 tPos;
						if (i == 6) {
							tPos = managerReference.positionObject [i].localPosition;
							tPos.y = -160;
							managerReference.positionObject [i].localPosition = tPos;
						}
						if (i == 7) {							
							tPos = managerReference.positionObject [i].localPosition;
							tPos.y = -230;
							managerReference.positionObject [i].localPosition = tPos;
						}
						managerReference.positionObject [i].gameObject.SetActive (true);
					} else {
						managerReference.positionObject [i].gameObject.SetActive (false);
					}
				}
			}
		}
		canCalculatePosition = true;
	}

	public void CalculateWaypoints(){
		if (configureMode == Switch.On) {
			GameObject newWaypoint;
			string newWaypointName;
			System.Array.Resize (ref Waypoints, TotalWaypoints);
			System.Array.Resize (ref waypointScripts, TotalWaypoints);
			for (var i = 0; i < TotalWaypoints; i++) {
				newWaypointName = "Waypoint " + (i + 1);
				if (Waypoints [i] == null) {
					foreach (Transform child in transform) {
						if (child.name == newWaypointName) {		Waypoints [i] = child;			}
					}
					if (Waypoints [i] == null) {
						newWaypoint = Instantiate (Resources.Load ("Waypoint")) as GameObject;
						newWaypoint.name = newWaypointName;
						newWaypoint.transform.parent = gameObject.transform;
						Waypoints [i] = newWaypoint.transform;
						waypointScripts [i] = newWaypoint.GetComponent<RG_Waypoint>();
						waypointScripts [i].waypointNumber = i + 1;
						Debug.Log ("Waypoint Controller created a new Waypoint: " + newWaypointName);
					}
				}				
			}
			newWaypoint = null;
			newWaypointName = null;
			CleanUpWaypoints ();
			managerReference.waypointCircut.AssignWaypoints();
		}
	}

	public void ResetLostPlayer(int playerNumber){
		int temp = racerInfo [playerNumber].nextWP - 1;
		if(temp < 0)	temp = Waypoints.Length - 1;
		racerInfo [playerNumber].racer.transform.position = Waypoints [ temp].position;
	}

	public void CleanUpWaypoints(){
		if (configureMode == Switch.On) {
			if (transform.childCount > Waypoints.Length) {
				foreach (Transform oldChild in transform) {
					if (oldChild.GetComponent<RG_Waypoint> ().waypointNumber > Waypoints.Length) {
						DestroyImmediate (oldChild.gameObject);
					}
				}
			}
		}
	}

	public void GenTimeSpanFromSeconds( double seconds ){
		TimeSpan    interval = TimeSpan.FromSeconds( seconds );			
		string      timeInterval = string.Format("{0:D2}:{1:D2}",  interval.Minutes, interval.Seconds);
		timeString = timeInterval + "." + interval.Ticks.ToString ().Substring (1,3);
		managerReference.timerText.text = timeString;
	} 

}