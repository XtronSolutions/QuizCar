using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RacingGameKit;
using Photon.Pun;

public class GameFinishManualOnline : MonoBehaviour 
{
	private Race_Manager raceManager;
	private HUD_Script racerDetail;
	private Racer_Register racerReg;
	private GameObject racer;
	private int playerPos;
	private PlayerRespawn respawner;

	public Text PlayerPosText;
	public GameObject FinishPanel;
	public Text[] posText = new Text[5];

	public Text rewardText;
	public Image[] stars;
	public GameObject hud;

	void Start()
	{
		GlobalVariables.isGameEnd = false;
		GlobalVariables.isPause = false;
		GameObject RaceManagerObject = GameObject.Find("_RaceManager");
		raceManager = RaceManagerObject.GetComponent (typeof(Race_Manager)) as Race_Manager;
		racerDetail = raceManager.GetComponent<HUD_Script> ();
		respawner = RaceManagerObject.GetComponent<PlayerRespawn> ();

		Invoke ("GetRaceRegister", 1.5f);
	}

	void GetRaceRegister(){
		racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
		for (int i = 0; i < posText.Length; i++) {
			posText [i].text = string.Format ("{0}. waiting...", i + 1); 
		}
	}

	void OnTriggerEnter(Collider hit)
	{
		if (hit.tag.Equals ("Player")) {
			GlobalVariables.isGameEnd = true;
			Invoke("ShowUI",0.2f);
		}
	}

	void ShowUI(){
		if(racerReg.IsRacerFinished)
		{
			StartCoroutine (EndAfterDelay ());

			CalculatePlayerPosition ();
			ShowPlayerPosition ();
		}

	}

	void CalculatePlayerPosition(){
		var pos = racerReg.RacerStanding;
		posText [pos - 1].color = Color.yellow;
		posText[pos - 1].fontSize = 30;
		posText [pos - 1].text = string.Format("{0}. {1}        {2}", pos, racerReg.RacerName, 
			TimeFormatter(racerReg.RacerDetail.RacerTotalTime));

		PlayerManagerScript.instance.Car.GetComponent<PhotonView> ().RPC ("RaceEnd", RpcTarget.Others, pos, racerReg.RacerDetail.RacerTotalTime, racerReg.RacerName);
	}

	public void SetOpponentsPosition(int pos, float racerEndTime, string racerName){
		posText [pos - 1].text = string.Format("{0}. {1}        {2}", pos, racerName, TimeFormatter(racerEndTime));
	}
		

	void ShowPlayerPosition()
	{
		int reward = 0;
		int position = racerReg.RacerStanding;
		if (position == 1 || position == 2 || position == 3) {

			int opt = GlobalVariables.ENVIRONMENT;
			if (opt == 1) {
				int selectedTrack = RewardProperties.Instance.GetTrackSelected ();
				if (selectedTrack != 3) {
					RewardProperties.Instance.SetUnlockTrack (selectedTrack + 1, 1);
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
			PlayerPosText.gameObject.SetActive (true);
			PlayerPosText.text = "7th";
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
		for (int i = 0; i < posText.Length; i++) {
			if (i >= raceManager.RegisteredRacers.Count) {
				posText [i].gameObject.SetActive (false);
			}
		}
		hud.SetActive(false);
		PlayerPosText.gameObject.SetActive (false);
		iTween.ScaleTo (FinishPanel, iTween.Hash ("scale", Vector3.one, "time", 0.7f, "easetype", iTween.EaseType.easeOutBounce, "oncompletetarget", this.gameObject,
			"oncomplete", "StopGamePlay"));
	}

	void StopGamePlay()
	{
		
//		Time.timeScale = 0;
		Invoke("StopCar", 0.3f);
	}

	void StopCar(){
		PlayerManagerScript.instance.Car.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		PlayerManagerScript.instance.Car.GetComponent<VehilceSyncing> ().DisableParts ();
		respawner.stopRespawning = true;
	}

	IEnumerator EndAfterDelay()
	{
		Debug.Log ("EndAfterDelay");
		yield return new WaitForSeconds (1f);
		StartEndScreenAnimation ();
	}
}
