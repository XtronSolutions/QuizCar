using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour {

    public GameObject[] tracks;
    public GameObject powerUpBoxesPrefab;
    public GameObject coinPrefab;

    public GameObject dummycamera;

    public GameObject scenePrefab;

    GameObject track;
    // Use this for initialization
    void Awake () {

        QualitySettings.vSyncCount = 2;
        Application.targetFrameRate =30;

        dummycamera.SetActive(false);

        if(GameData.trackNo>=3)
            track = Instantiate(tracks[GameData.trackNo-3]);
        else
            track = Instantiate(tracks[GameData.trackNo]);

        track.GetComponent<Track>().InitPowerUpsAndCoins(scenePrefab,powerUpBoxesPrefab, coinPrefab);
     
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}