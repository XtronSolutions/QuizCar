//This script help you to pass data from main screen to loaded screens

using UnityEngine;
using System.Collections;
using RacingGameKit;
public class RaceManagerStaticDataManager : MonoBehaviour
{

    Race_Manager oRaceManager;
    GameObject RaceManagerObject;
    private bool LetsRaceStarted = false;

    void Awake()
    {
        RaceManagerObject = GameObject.Find("_RaceManager");
        if (RaceManagerObject == null)
        { Debug.Log("Cant find RaceManager"); }
        else
        {
            oRaceManager = RaceManagerObject.GetComponent<Race_Manager>();
            if (RaceManagerStaticData.SelectedCar != null)
            {
                oRaceManager.HumanRacerPrefab = RaceManagerStaticData.SelectedCar;
            }
        }
    }

    void Start()
    {
        if (oRaceManager == null)
        { Debug.Log("Cant find RaceManager"); }
        else
        {
            LetsRaceStarted = true;
        }

    }

    void Update()
    {
        if (oRaceManager.IsRaceReady &&  !oRaceManager.IsRaceStarted && LetsRaceStarted)
        {
            oRaceManager.StartRace();
            LetsRaceStarted = false;
        }
    }

}