using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RacingGameKit;

public class gameFinishDesert : MonoBehaviour 
{
	public GameObject EndPanel;
	public Text[] posText = new Text[5];
	public Text PlayerPosText;

	private int playerPos;
	private Race_Manager raceManager;
	private Racer_Register racerReg;

	public Text rewardText;
	public Image[] stars;

	public GameObject hud;

	void Start()
	{
		GameObject RaceManagerObject = GameObject.Find("_RaceManager");
		raceManager = RaceManagerObject.GetComponent (typeof(Race_Manager)) as Race_Manager;
		GlobalVariables.isGameEnd = false;
		GlobalVariables.isPause = false;
		Invoke ("GetRaceRegister", 1.5f);
	}
	void GetRaceRegister(){
		racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
	}

	void OnTriggerEnter(Collider hit)
	{
		Debug.Log ("TAGG:"+hit.tag);
		
		if (hit.tag.Equals ("Player")) {
			GlobalVariables.isGameEnd = true;
			Invoke("ShowUI",0.2f);
		}
		
	}
	
	void ShowUI(){
		if(racerReg.IsRacerFinished)
		{
			StartCoroutine (EndAfterDelay ());
			//			FinishPanel.SetActive(true);
			
			CalculatePosition ();
			ShowPlayerPosition ();
		}
		
	}

	void CalculatePosition()
	{
		float tempTime = 0;
		//		raceManager.RegisteredRacers.Sort (delegate(Racer_Detail x, Racer_Detail y) {
		//			return x.RacerStanding < y.RacerStanding;
		//		});

		for (int i = 0; i < posText.Length; i++) 
		{
			if (i < raceManager.RegisteredRacers.Count) 
			{
				if (raceManager.RegisteredRacers [i].IsPlayer) 
				{
					posText [i].color = Color.yellow;
					posText[i].fontSize = 30;
					posText [i].text = (i + 1) + ". " + raceManager.RegisteredRacers [i].RacerName + "        " + TimeFormatter(raceManager.RegisteredRacers [i].RacerTotalTime);
					playerPos = i + 1;
					tempTime = raceManager.RegisteredRacers [i].RacerTotalTime;
				}
				else 
				{
					if (raceManager.RegisteredRacers [i].RacerTotalTime == 0) 
					{
						tempTime = Random.Range (tempTime + 1, tempTime + 5);
						posText [i].text = (i + 1) + ". " + raceManager.RegisteredRacers [i].RacerName + "        " + TimeFormatter(tempTime);
					}
					else 
					{
						posText [i].text = (i + 1) + ". " + raceManager.RegisteredRacers [i].RacerName + "        " + TimeFormatter(raceManager.RegisteredRacers [i].RacerTotalTime);
					}
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
		int position = playerPos;
		if (position == 1 || position == 2 || position == 3) {

			int selectedTrack = RewardProperties.Instance.GetDesertTrackSelected ();
			if (selectedTrack != 3) {
				RewardProperties.Instance.SetUnlockDesertTrack (selectedTrack + 1, 1);
			}
		}
		switch (playerPos) 
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
		}
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
		PlayerPosText.gameObject.SetActive (false);
		hud.SetActive (false);
		iTween.ScaleTo (EndPanel, iTween.Hash ("scale", Vector3.one, "time", 0.7f, "easetype", iTween.EaseType.easeOutBounce, "oncompletetarget", this.gameObject,
			"oncomplete", "StopGamePlay"));
	}

	void StopGamePlay()
	{
		Time.timeScale = 0;
	}


	IEnumerator EndAfterDelay()
	{
		yield return new WaitForSeconds (1f);
		StartEndScreenAnimation ();
	}
}
