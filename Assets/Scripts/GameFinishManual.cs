using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RacingGameKit;
using Photon.Pun;
public class GameFinishManual : MonoBehaviour 
{
	private Race_Manager raceManager;
	private HUD_Script racerDetail;
	private Racer_Register racerReg;
	private GameObject racer;
	private int playerPos;

	public Text PlayerPosText;
	public GameObject FinishPanel;

	public Text[] posText = new Text[5];

	public Text rewardText, scoreText;
	public Image[] stars;
	public GameObject hud;
    FinishPointHandler finishPointHandler;

    int coins;
	void Start()
	{
        //		racer = PlayerManagerScript.instance.Car;

        finishPointHandler = GameObject.FindObjectOfType<FinishPointHandler>();
        PlayerPosText = finishPointHandler.PlayerPosText;
        FinishPanel = finishPointHandler.FinishPanel;
        posText = finishPointHandler.posText;
        rewardText = finishPointHandler.rewardText;
        stars = finishPointHandler.stars;
        hud = finishPointHandler.hud;
        scoreText = finishPointHandler.scoreText;

		GlobalVariables.isGameEnd = false;
		GlobalVariables.isPause = false;
		GameObject RaceManagerObject = GameObject.Find("_RaceManager");
		raceManager = RaceManagerObject.GetComponent (typeof(Race_Manager)) as Race_Manager;
		racerDetail = raceManager.GetComponent<HUD_Script> ();

		Invoke ("GetRaceRegister", 1.5f);
	}
	void GetRaceRegister(){
		racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
	}
	void OnTriggerEnter(Collider hit)
	{/*
		Debug.Log ("TAGG:"+hit.tag + " isRacerFinished  "+racerReg.IsRacerFinished+ "  isEnd "+ GlobalVariables.isGameEnd  );

		if (hit.tag.Equals ("Player") && hit.transform.root.gameObject.GetInstanceID() == PlayerManagerScript.instance.Car.GetInstanceID()) {
            if (racerDetail.progressBar2.value > 0.9f)
            {
                GlobalVariables.isGameEnd = true;
                Invoke("ShowUI", 0.2f);
            }
		}
        */



//			for (int i = 0; i < raceManager.RegisteredRacers.Count; i++) 
//			{
//				Debug.Log (raceManager.RegisteredRacers [i].RacerTotalTime + " -- " + raceManager.RegisteredRacers [i].RacerName);
//			}

	}
    void ShowTutorial()
    {
        finishPointHandler.ShowTutorial();
    }
void ShowUI(){
	if(racerReg.IsRacerFinished)
	{
		StartCoroutine (EndAfterDelay ());
		//			FinishPanel.SetActive(true);
		
		CalculatePosition ();
		ShowPlayerPosition ();
        CalculateScore();
        //Invoke("ShowTutorial", 2);
    }
			
}
    void Update()
    {
        if(!GlobalVariables.isGameEnd)
        {
            if (racerReg != null)
            {

                if (racerReg.IsRacerFinished)
                {
                    GlobalVariables.isGameEnd = true;
                    if(Constants.isMultiplayerSelected)
                    PlayerManagerScript.instance.Car.GetComponent<PhotonView>().RPC("RaceEnd", RpcTarget.Others, racerReg.RacerStanding, racerReg.RacerDetail.RacerTotalTime, racerReg.RacerName);
                    Invoke("ShowUI", 0.2f);
                }
            }
        }

        if(GlobalVariables.isGameEnd)
        CalculatePosition();
    }
	void CalculatePosition()
	{
        //		raceManager.RegisteredRacers.Sort (delegate(Racer_Detail x, Racer_Detail y) {
        //			return x.RacerStanding < y.RacerStanding;
        //		});
       // Debug.Log("count " + raceManager.RegisteredRacers.Count);
		float tempTime = 0;
		for (int i = 0; i < posText.Length; i++) 
		{
            if (i < raceManager.RegisteredRacers.Count)
            {
                if (!raceManager.RegisteredRacers[i].RacerDestroyed || raceManager.RegisteredRacers[i].RacerFinished)
                { 
                    // Debug.Log("id " + raceManager.RegisteredRacers[i].ID);
                    if (raceManager.RegisteredRacers[i].IsPlayer && raceManager.RegisteredRacers[i].ID == PlayerManagerScript.instance.Car.GetComponent<Racer_Register>().RacerDetail.ID)
                    {
                        posText[i].color = Color.yellow;
                        posText[i].fontSize = 30;
                        posText[i].text = (i + 1) + ". " + raceManager.RegisteredRacers[i].RacerName + "        " + TimeFormatter(raceManager.RegisteredRacers[i].RacerTotalTime);
                        playerPos = i + 1;
                        tempTime = raceManager.RegisteredRacers[i].RacerTotalTime;
                        posText[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        if (raceManager.RegisteredRacers[i].RacerTotalTime == 0)
                        {
                            tempTime = Random.Range(tempTime + 1, tempTime + 5);
                            posText[i].text = (i + 1) + ". " + raceManager.RegisteredRacers[i].RacerName + "        " + TimeFormatter(tempTime);
                        }
                        else
                        {
                            posText[i].text = (i + 1) + ". " + raceManager.RegisteredRacers[i].RacerName + "        " + TimeFormatter(raceManager.RegisteredRacers[i].RacerTotalTime);
                        }
                        posText[i].color = Color.white;
                        posText[i].fontSize = 26;
                        posText[i].gameObject.SetActive(true);
                    }
                }
                else //if(!raceManager.RegisteredRacers[i].RacerFinished)
                {
                    raceManager.RegisteredRacers.RemoveAt(i);
                    posText[i].gameObject.SetActive(false);
                }
			}
			else 
			{
				posText [i].gameObject.SetActive (false);
			}
		}
	}

	void ShowPlayerPosition()
	{
		int reward = 0;
		int position = racerReg.RacerStanding;
		if (position == 1 || position == 2 || position == 3) {
		
			int opt = GlobalVariables.ENVIRONMENT;
			if (GameData.selectedEnvironment == 0) {
				int selectedTrack = RewardProperties.Instance.GetTrackSelected ();
				if (selectedTrack != 6) {
					RewardProperties.Instance.SetUnlockTrack (selectedTrack + 1, 1);
				}
			}
            else if (GameData.selectedEnvironment == 1)
            {
                int selectedTrack = GameData.trackNo + 1;//RewardProperties.Instance.GetDesertTrackSelected();
                if (selectedTrack != 6)
                {
                    RewardProperties.Instance.SetUnlockDesertTrack(selectedTrack + 1, 1);
                }
            }
        }
        switch (racerReg.RacerStanding) 
		{
		case 1:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "1st";
			reward = 10 + Constants.coinsCollected;
			rewardText.text = string.Format ("10 + {0}", Constants.coinsCollected);
			RewardProperties.Instance.Coin += reward;
			foreach (var star in stars) {
				star.enabled = true;
			}
			break;
		case 2:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "2nd";
			reward = 6 + Constants.coinsCollected;
			rewardText.text = string.Format ("6 + {0}", Constants.coinsCollected);
			RewardProperties.Instance.Coin += reward;
			for (int i = 0; i < 2; i++) {
				stars [i].enabled = true;
			}
			break;
		case 3:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "3rd";
			reward = 2 + Constants.coinsCollected;
			rewardText.text = string.Format ("2 + {0}", Constants.coinsCollected);
			RewardProperties.Instance.Coin += reward;
			stars [0].enabled = true;
			break;
		case 4:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "4th";
			reward = Constants.coinsCollected;
			rewardText.text = reward.ToString ();
			RewardProperties.Instance.Coin += reward;
			break;
		case 5:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "5th";
			reward = Constants.coinsCollected;
			rewardText.text = reward.ToString ();
			RewardProperties.Instance.Coin += reward;
			break;
		case 6:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "6th";
			reward = Constants.coinsCollected;
			rewardText.text = reward.ToString ();
			RewardProperties.Instance.Coin += reward;
			break;
		case 7:
                PlayerPosText.gameObject.SetActive(true);
                PlayerPosText.text = "7th";
                reward = Constants.coinsCollected;
                rewardText.text = reward.ToString();
                RewardProperties.Instance.Coin += reward;
                break;
            case 8:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "8th";
			break;
		case 9:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "9th";
			break;
		case 10:
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "10th";
			break;

		}
        coins = reward;
	}

	string TimeFormatter ( float totalSeconds )
	{

		int seconds = (int)totalSeconds % 60;
		int minutes = (int)totalSeconds / 60;

		string time = ( ( minutes < 10 ) ? "0" + minutes : minutes + "" ) + ":" + ( ( seconds < 10 ) ? "0" + seconds : seconds + "" );

		return time;
	}


	void StartEndScreenAnimation()
	{
//		Debug.Log ("StartEndScreenAnimation");
		hud.SetActive(false);
		PlayerPosText.gameObject.SetActive (false);
		iTween.ScaleTo (FinishPanel, iTween.Hash ("scale", Vector3.one, "time", 0.7f, "easetype", iTween.EaseType.easeOutBounce, "oncompletetarget", this.gameObject,
			"oncomplete", "StopGamePlay"));
	}

	void StopGamePlay()
	{
		//Time.timeScale = 0;
	}

	IEnumerator EndAfterDelay()
	{
		Debug.Log ("EndAfterDelay");
		yield return new WaitForSeconds (1f);
		StartEndScreenAnimation ();
	}

    void CalculateScore()
    {
        int sc = 10;
        Racer_Register reg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register>();
        sc += 25 - Mathf.Clamp((reg.RacerStanding * 5),5,20) + (GameData.trackNo * 5);
        sc += coins * 5;
        GameData.data.score += sc;
        scoreText.text = "Score: " + sc;
        GameData.data.Save();
    }
}
