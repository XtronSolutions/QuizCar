using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBarValues : MonoBehaviour {

	private const int NoOfBuggies = 4;

	public float[] DefaultAccBarValue;
	public float[] DefaultStrengthBarValue;
	public float[] DefaultHandleBarValue;
	//greenbar
	public float[] DefaultAccGreenBarValue;
	public float[] DefaultStrengthGreenBarValue;
	public float[] DefaultHandleGreenBarValue;

	public Image AccBar;
	public Image StrengthBar;
	public Image HandleBar;

	//Greenbar
	public Image AccGreenBar;
	public Image StrengthGreenBar;
	public Image HandleGreenBar;

	public Text CoinsText;

	public string[] UpgradeCost;
	public Text UpgradeText;

	public Image Stage;
	public Sprite[] StageSprites;
	public GameObject UpgradedBtn;

	public GameObject BuyButtonLock;
	public GameObject BuyButtonUnlock;
	public GameObject UpgradeBtn;

	public int[] BuggiesCost;
	public Text CostText;

	public Text PercentTextAcc;
	public Text PercentTextStrngth;
	public Text PercentTextHandle;
	public int[] PercentageValue;

	private float[] AccBarValue = new float[NoOfBuggies] ;
	private float[] StrengthBarValue = new float[NoOfBuggies];
	private float[] HandleBarValue = new float[NoOfBuggies];

	// greenBar

	private float[] AccGreenBarValue = new float[NoOfBuggies] ;
	private float[] StrengthGreenBarValue = new float[NoOfBuggies];
	private float[] HandleGreenBarValue = new float[NoOfBuggies];

	private float AccIncrement = 0.181f;
	private float StrengthIncrement = 0.12175f;
	private float HandleIncrement = 0.092f;
	private int BuggyPos = 0;

	public VehiclePartsUpgradeMenu[] vehicleParts;

	public Text selectedText;

	public BuggyRotation[] buggyRotation;

	public MenuManager menuManager;

	// Use this for initialization
	void OnEnable () {
//		PlayerPrefs.DeleteAll ();
		menuManager._buggyPos = 0;
		//if (PlayerPrefs.GetInt ("FirstTime", 0) == 0) 
        if(GameData.data.vehicles[0].isUnlocked == 0)
        {

            Debug.Log("frsy");
			PlayerPrefs.SetInt ("FirstTime", 1);

			for (int i = 0; i < DefaultAccBarValue.Length; i++) {

				RewardProperties.Instance.SetBuggyAcc(i,DefaultAccBarValue[i]);
				RewardProperties.Instance.SetBuggyStrength(i,DefaultStrengthBarValue[i]);
				RewardProperties.Instance.SetBuggyHandling(i,DefaultHandleBarValue[i]);
				RewardProperties.Instance.SetBuggyUpgrade (i, 0);
				RewardProperties.Instance.SetBuggyUnlock (i, 0);
			}
			// for Greenbar
			for (int i = 0; i < DefaultAccGreenBarValue.Length; i++) {

				RewardProperties.Instance.SetBuggyGreenAcc(i,DefaultAccGreenBarValue[i]);
				RewardProperties.Instance.SetBuggyGreenStrength(i,DefaultStrengthGreenBarValue[i]);
				RewardProperties.Instance.SetBuggyGreenHandling(i,DefaultHandleGreenBarValue[i]);
			}
			//RewardProperties.Instance.SetBuggyUnlock (0, 1);
            GameData.data.Save();

            for (int i = 0; i < vehicleParts.Length; i++)
            {
                vehicleParts[i].UpgradeParts(0);
            }
        }
        else
        {
            for(int i=0;i<vehicleParts.Length;i++)
            {
                vehicleParts[i].init();
            }
        }

		ResetValues (0);
		CoinsText.text = RewardProperties.Instance.Coin.ToString ();
        menuManager.SelectforwardBuggy(RewardProperties.Instance.GetBuggySelected());
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void SetBuggyPosition(int value){
	
		BuggyPos = value;
	}

	public void OnUpgradeAcceleration(){

		AccBar.fillAmount += AccIncrement;
		AccBarValue [BuggyPos] = AccBar.fillAmount;
		RewardProperties.Instance.SetBuggyAcc(BuggyPos,AccBar.fillAmount);
		// green bar
		AccGreenBar.fillAmount += AccIncrement;
		AccGreenBarValue [BuggyPos] = AccGreenBar.fillAmount;
		RewardProperties.Instance.SetBuggyGreenAcc(BuggyPos,AccGreenBar.fillAmount);

	}

	public void OnUpgradeStrength(){

		StrengthBar.fillAmount += StrengthIncrement;
		StrengthBarValue [BuggyPos] = StrengthBar.fillAmount;
		RewardProperties.Instance.SetBuggyStrength (BuggyPos, StrengthBar.fillAmount);
		// green bar
		StrengthGreenBar.fillAmount += StrengthIncrement;
		StrengthGreenBarValue [BuggyPos] = StrengthGreenBar.fillAmount;
		RewardProperties.Instance.SetBuggyGreenStrength (BuggyPos, StrengthGreenBar.fillAmount);
	}

	public void OnUpgradeHandling(){

		HandleBar.fillAmount += HandleIncrement;
		HandleBarValue [BuggyPos] = HandleBar.fillAmount;
		RewardProperties.Instance.SetBuggyHandling (BuggyPos, HandleBar.fillAmount);

		//greenbar
		HandleGreenBar.fillAmount += HandleIncrement;
		HandleGreenBarValue [BuggyPos] = HandleGreenBar.fillAmount;
		RewardProperties.Instance.SetBuggyGreenHandling (BuggyPos, HandleGreenBar.fillAmount);
	}

	public void ResetValues(int Pos){

		if (RewardProperties.Instance.GetBuggyUnlock(Pos) == 1) {
			AccBar.fillAmount = RewardProperties.Instance.GetBuggyAcc(Pos);
			StrengthBar.fillAmount = RewardProperties.Instance.GetBuggyStrength(Pos);
			HandleBar.fillAmount =RewardProperties.Instance.GetBuggyHandling(Pos);

			//green bar
			AccGreenBar.fillAmount = RewardProperties.Instance.GetBuggyGreenAcc(Pos);
			StrengthGreenBar.fillAmount = RewardProperties.Instance.GetBuggyGreenStrength(Pos);
			HandleGreenBar.fillAmount =RewardProperties.Instance.GetBuggyGreenHandling(Pos);

		} else {
		
			// yellow bar
			AccBar.fillAmount = DefaultAccBarValue[Pos];
			StrengthBar.fillAmount = DefaultStrengthBarValue[Pos];
			HandleBar.fillAmount = DefaultHandleBarValue[Pos];

			//Greenbar
			AccGreenBar.fillAmount = DefaultAccGreenBarValue[Pos];
			StrengthGreenBar.fillAmount = DefaultStrengthGreenBarValue[Pos];
			HandleGreenBar.fillAmount = DefaultHandleGreenBarValue[Pos];
		}

		UpdateBuyButton (Pos);
		UpdateBuggyCost (Pos);
		UpdateUpgradeTextOnBuggyChange (Pos);
		UpdateStageImageOnBuggyChange (Pos);
		UpdtaeUpgradeButtonOnBuggyChange (Pos);
		UpdateBuggyRotation (Pos);
	}


	void UpdateBuyButton(int Pos){
		if (RewardProperties.Instance.GetBuggyUnlock (Pos) == 0) {
			
			BuyButtonLock.SetActive (true);
			BuyButtonUnlock.SetActive (false);
			UpgradeBtn.SetActive (false);

		} else {
		
			BuyButtonLock.SetActive (false);
			BuyButtonUnlock.SetActive (true);
			UpgradeBtn.SetActive (true);
			if (RewardProperties.Instance.GetBuggySelected () == Pos) {
				selectedText.text = "SELECTED";
			} else {
				selectedText.text = "SELECT";
			}
		}
	}

	void UpdateBuggyCost(int Pos){
	
		CostText.text = BuggiesCost [Pos].ToString();
	}

	public void OnUpgradeCar(Text txt){
	
		string str = txt.text.ToString ();
		int coins = int.Parse (str);

		if (RewardProperties.Instance.Coin > coins) {

			OnUpgradeAcceleration ();
			OnUpgradeStrength ();
			OnUpgradeHandling ();
			UpdateStageImage ();
			UpdateCoins (coins);

			if (AccBar.fillAmount < 1) {
				UpdateUpgradeText ();

			} else {
				UpdateUpgradeText ();
				UpdateUpgradeButton ();
			}
		}
	}

	void UpdateCoins(int coins){
	
		int coinValue = RewardProperties.Instance.Coin;
		coinValue -= coins;
		RewardProperties.Instance.Coin = coinValue;
		CoinsText.text = RewardProperties.Instance.Coin.ToString ();
	}

	void UpdateUpgradeText(){
	
		int value = RewardProperties.Instance.GetBuggyUpgrade(BuggyPos) + 1;
		RewardProperties.Instance.SetBuggyUpgrade (BuggyPos, value);
		if(BuggyPos < vehicleParts.Length)
			vehicleParts [BuggyPos].UpgradeParts (value);
		UpgradeText.text = UpgradeCost [RewardProperties.Instance.GetBuggyUpgrade (BuggyPos)];

	}

	void UpdateUpgradeTextOnBuggyChange(int Pos){
	
		UpgradeText.text = UpgradeCost [RewardProperties.Instance.GetBuggyUpgrade (Pos)];
	}

	void UpdateStageImage(){
	
		Stage.sprite = StageSprites [RewardProperties.Instance.GetBuggyUpgrade (BuggyPos)+1];
		// percentage text
		PercentTextAcc.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (BuggyPos) + 1].ToString();
		PercentTextStrngth.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (BuggyPos) + 1].ToString();
		PercentTextHandle.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (BuggyPos) + 1].ToString();

	}

	void UpdateStageImageOnBuggyChange(int Pos){

		Stage.sprite = StageSprites [RewardProperties.Instance.GetBuggyUpgrade (Pos)];
		// percentage text
		PercentTextAcc.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (Pos)].ToString();
		PercentTextStrngth.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (Pos)].ToString();
		PercentTextHandle.text = "+" + PercentageValue [RewardProperties.Instance.GetBuggyUpgrade (Pos)].ToString();

	}

	void UpdateUpgradeButton(){

		UpgradedBtn.SetActive (true);
		PlayerPrefs.SetInt ("BuggyFullUpgraded" + BuggyPos, 1);
	}

	void UpdtaeUpgradeButtonOnBuggyChange(int Pos){
	
        if(GameData.data.vehicles[Pos].upgraded <4 )
		//if (PlayerPrefs.GetInt ("BuggyFullUpgraded" + Pos, 0) == 0)
        {
		
			UpgradedBtn.SetActive (false);
		} else {
		
			UpgradedBtn.SetActive (true);
		}
	}

	public void OnBuyBuggy(Text txt){
	
		string str = txt.text.ToString ();
		int coins = int.Parse (str);

		if (RewardProperties.Instance.Coin > coins) {
		
			RewardProperties.Instance.SetBuggyUnlock (BuggyPos, 1);
			RewardProperties.Instance.SetBuggySelected (BuggyPos);
			UpdateCoins (coins);
			UpdateBuyButton (BuggyPos);
		}
	}

	public void OnSelectBuggy(){
		RewardProperties.Instance.SetBuggySelected (BuggyPos);
		UpdateBuyButton (BuggyPos);
	}

	void UpdateBuggyRotation(int pos){
		for (int i = 0; i < buggyRotation.Length; i++) {
			if (i == pos) {
				buggyRotation [i].enabled = true;
			} else {
				buggyRotation [i].enabled = false;
			}
		}
	}
}
