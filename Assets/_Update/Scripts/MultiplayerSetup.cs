using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
public class MultiplayerSetup : MonoBehaviour {

    public GameObject multiplayerLoader;
    public Race_Manager race_Manager;

    public GameObject playerCar;
    public GameObject playerOnlineCar;
    public GameObject[] AICars;

    public GameObject GlobalCarManager;
	// Use this for initialization
	void Start () {
      //  Constants.isMultiplayerSelected = true;
        if (Constants.isMultiplayerSelected)
        {
            multiplayerLoader.SetActive(true);
            race_Manager.RaceStartsOnStartup = false;

            race_Manager.RaceLaps = 1;
            race_Manager.RacePlayers = 8;
            race_Manager.TimerCountdownFrom = 0;

            race_Manager.AIRacerPrefab = null;
            race_Manager.HumanRacerPrefab = playerOnlineCar;


        }
        else
        {
            multiplayerLoader.SetActive(false);
            race_Manager.RaceStartsOnStartup = true;
            race_Manager.RaceLaps = 3;
            race_Manager.RacePlayers = 5;
            race_Manager.TimerCountdownFrom = 5;
            race_Manager.HumanRacerPrefab = playerCar;
        }
        race_Manager.gameObject.SetActive(true);
        GlobalCarManager.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    
}