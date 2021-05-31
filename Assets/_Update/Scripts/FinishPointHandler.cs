using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FinishPointHandler : MonoBehaviour {

    public Text PlayerPosText;
    public GameObject FinishPanel;
    public Text[] posText = new Text[5];

    public Text rewardText;
    public Image[] stars;
    public GameObject hud;
    public GameObject restartBtn;

    public Text scoreText;


    public GameObject black, msg;
    // Use this for initialization
    void Start () {
       
            restartBtn.SetActive(!Constants.isMultiplayerSelected);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowTutorial()
    {
        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            black.SetActive(true);
            msg.SetActive(true);
        }
    }
}