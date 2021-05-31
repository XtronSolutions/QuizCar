using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

[ExecuteInEditMode]
public class RG_GarageManager : MonoBehaviour {
	public enum Switch { On, Off }
	public enum MusicSelection { ListOrder, Random }
	[System.Serializable]
	public class GarageSceneReference{
        //public string developerAddress;
        //public string reviewAddress;

		[Header("Text")]
		public Text currencyText;
		public Text raceNameText;
		public Text carNameText;
        public Text carSpeedText;
		public Text rewardText;
		public Text lapText;
		public Text numberOfRacersText;
		public Text selectRaceText;
		//public Text selectCarText;
		//public Text selectPaintText;
		//public Text selectGlowText;
		//public Text selectBrakeColorText;
		//public Text selectRimColorText;
		//public Text selectGlassTintText;
		//public Text topSpeedPriceText;
		//public Text accelerationPriceText;
		//public Text brakePowerPriceText;
		//public Text tireTractionPriceText;
		//public Text steerSensetivityPriceText;
		//public Text buyCarText;
		//public Text buyPaintText;
		//public Text buyGlowText;
		//public Text buyGlassTintText;
		//public Text buyBrakeColorText;
		//public Text buyRimColorText;
		public Text unlockLevelButtonText;
		//public Text unlockLevelText;
  //      public Text upgradeConfirmText;
		//[Header("Sliders")]
		//public Slider redSlider;
		//public Slider blueSlider;
		//public Slider greenSlider;
		//public Slider redGlowSlider;
		//public Slider blueGlowSlider;
		//public Slider greenGlowSlider;
		//public Slider redBrakeSlider;
		//public Slider blueBrakeSlider;
		//public Slider greenBrakeSlider;
		//public Slider redGlassSlider;
		//public Slider blueGlassSlider;
		//public Slider greenGlassSlider;
		//public Slider alphaGlassSlider;
		//public Slider redRimSlider;
		//public Slider blueRimSlider;
		//public Slider greenRimSlider;
  //      public Slider topSpeedSlider;
  //      public Slider accelerationSlider;
  //      public Slider brakePowerSlider;
  //      public Slider tireTractionSlider;
  //      public Slider steerSensitivitySlidetr;
        [Header("Garage Menu Windows")]
        //public GameObject mainMenuWindow;
        //public GameObject menuWindow;
        public GameObject garageWindow;
        //public GameObject settingsMenuWindow;
        public GameObject garageUI;
        //public GameObject quitConfirmWindow;
        //      public GameObject buyCarConfirmWindow;
        //      public GameObject priceUI;
        //      public GameObject buyCarButton;
        //      public GameObject unlockedVehicleOptions;
        //public GameObject carConfirmWindow;
        //public GameObject paintWindow;
        //public GameObject rimColorWindow;
        //public GameObject glassColorWindow;
        //public GameObject brakeColorWindow;
        //public GameObject paintConfirmWindow;
        //public GameObject rimColorConfirmWindow;
        //public GameObject glassColorConfirmWindow;
        //public GameObject brakeColorConfirmWindow;
        //public GameObject glowWindow;
        //public GameObject glowConfirmWindow;
        //      public GameObject upgradesWindow;
        //      public GameObject upgradesConfirmWindow;
        //      public GameObject raceDetailsWindow;
        //public GameObject unlockRaceConfirmWindow;
        //public GameObject loadingWindow;
        public GameObject raceDetails;
        public GameObject racesWindow;
		//public GameObject multiplayerWindow;
		[Header("Other")]
		public GameObject raceLockedIcon;
		public GameObject unlockRaceButton;
        public GameObject upgradeTopSpeedButton;
        public GameObject upgradeAccelerationButton;
        public GameObject upgradeBrakePowerButton;
        public GameObject upgradeTireTractionButton;
        public GameObject upgradeSteerSensitivityButton;
		public GameObject previousCarButton;
		public GameObject nextCarButton;
    }
		
	public RaceData raceData;
	public PlayableVehicles playableVehicles;
	public PlayerPrefsData playerPrefsData;
	public AudioData audioData;
	public GameObject lobbyManager;
	public Switch configureRaceSize;
	public Switch configureCarSize;
	public Switch purchaseLevelUnlock;
	public Switch autoUnlockNextRace;
	[Tooltip("Enable an option to increase or decrease the amount of races available.")]
	public string lockButtonText;
	public String openWorldName;
	public GarageSceneReference uI;
	//public ParticleSystem[] sceneCarGlowLight;
	public GameObject[] sceneCarModel;
	public GameObject[] raceImage;
	private GameObject audioContainer;
	private GameObject emptyObject;
	private AudioSource garageAudioSource;
	private bool colorChange;
	private bool brakeColorChange;
	private bool glassColorChange;
	private bool rimColorChange;
	private bool glowChange;
    private bool upgradeChange;
	private bool quitConfirmIsOpen;
	private Color carColor;
	private int vehicleNumber;
	private int raceNumber;
	private int currency;
	[Range(0,1f)] private float carAlpha;
	private int totalRaces;
	private string raceNameToLoad;

    void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (PlayerPrefs.GetString("CURRENTVERSION") != "1.04")  {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("CURRENTVERSION", "1.04");
        }
		if (Application.isPlaying) {
			audioContainer = new GameObject ();
			audioContainer.name = "Audio Container";
			audioContainer.transform.SetParent (gameObject.transform);
			Time.timeScale = 1.0f;
			AudioMusic ();
			GetPlayerData ();
			//uI.currencyText.text = currency.ToString ("N0");
			for (int i = 0; i < playableVehicles.numberOfCars; i++) {
				sceneCarModel[i].SetActive (false);
                if (i > 0 && PlayerPrefs.GetInt ("Car Unlocked" + i.ToString (), 0) == 1) {
					playableVehicles.carUnlocked[i] = true;
				}
				if (i == vehicleNumber) {
					sceneCarModel[i].SetActive (true);
                    //uI.topSpeedSlider.value = playableVehicles.topSpeedLevel[i];
                    //uI.accelerationSlider.value = playableVehicles.torqueLevel[i];
                    //uI.brakePowerSlider.value = playableVehicles.brakeTorqueLevel[i];
                    //uI.tireTractionSlider.value = playableVehicles.tireTractionLevel[i];
                    //uI.steerSensitivitySlidetr.value = playableVehicles.steerSensitivityLevel[i];
                }
			}
			for (int i = 0; i < raceData.numberOfRaces; i++) {
				raceImage [i].SetActive (false);
				if (i == raceNumber) {
					raceImage [i].SetActive (true);
				}
			}
			//uI.topSpeedPriceText.text = playableVehicles.upgradeSpeedPrice.ToString ("C0");
			//uI.accelerationPriceText.text = playableVehicles.upgradeAccelerationPrice.ToString ("C0");
			//uI.brakePowerPriceText.text = playableVehicles.upgradeBrakesPrice.ToString ("C0");
			//uI.tireTractionPriceText.text = playableVehicles.upgradeTiresPrice.ToString ("C0");
			//uI.steerSensetivityPriceText.text = playableVehicles.upgradeSteeringPrice.ToString ("C0");
   //         uI.buyGlowText.text = "Change Neon Light\nfor\n$" + playableVehicles.glowPrice.ToString ("N0");
			//uI.buyPaintText.text = "Paint this car\nfor\n$" + playableVehicles.paintPrice.ToString ("N0");
			//uI.buyBrakeColorText.text = "Change Brake Color\nfor\n$" + playableVehicles.brakeColorPrice.ToString ("N0");
			//uI.buyRimColorText.text = "Change Rim Color\nfor\n$" + playableVehicles.rimColorPrice.ToString ("N0");
			//uI.buyGlassTintText.text = "Change Glass Tint\nfor\n$" + playableVehicles.glassColorPrice.ToString ("N0");
			//uI.selectGlowText.text = "$" + playableVehicles.glowPrice.ToString ("N0");
			//uI.selectPaintText.text = "$" + playableVehicles.paintPrice.ToString ("N0");
			//uI.selectBrakeColorText.text = "$" + playableVehicles.brakeColorPrice.ToString ("N0");
			//uI.selectRimColorText.text = "$" + playableVehicles.rimColorPrice.ToString ("N0");
			//uI.selectGlassTintText.text = "$" + playableVehicles.glassColorPrice.ToString ("N0");
			uI.carNameText.text = playableVehicles.vehicleNames[vehicleNumber];
			uI.raceNameText.text = raceData.raceNames [raceNumber];
			CalculateRewardText ();
			uI.lapText.text = "Laps\n" + raceData.raceLaps [raceNumber].ToString ();
			uI.numberOfRacersText.text = "Racers\n" + raceData.numberOfRacers [raceNumber].ToString();
			uI.raceLockedIcon.SetActive (false);
			if (purchaseLevelUnlock == Switch.Off) {
				uI.unlockRaceButton.SetActive (false);
			}
			if (autoUnlockNextRace == Switch.On) {
				PlayerPrefs.SetString ("AutoUnlockNextRace", "TRUE");
			} else {
				PlayerPrefs.SetString ("AutoUnlockNextRace", "FALSE");
			}
		}
		UpdateCar ();
	}

	// Called on Scene start to load data from PlayerPrefs and Scriptable Objects
	public void GetPlayerData(){
		for(int i = 0; i < playableVehicles.numberOfCars; i++){
			playableVehicles.topSpeedLevel[i] = PlayerPrefs.GetInt("CarTopSpeedLevel" + i.ToString());
			playableVehicles.torqueLevel[i] = PlayerPrefs.GetInt("CarTorqueLevel" + i.ToString());
			playableVehicles.brakeTorqueLevel[i] = PlayerPrefs.GetInt("CarBrakeLevel" + i.ToString());
			playableVehicles.tireTractionLevel[i] = PlayerPrefs.GetInt("CarTireTractionLevel" + i.ToString());
			playableVehicles.steerSensitivityLevel[i] = PlayerPrefs.GetInt("CarSteerSensitivityLevel" + i.ToString());
			//playerPrefsData.redValues[i] = PlayerPrefs.GetFloat("Red" + i.ToString(), 1);
			//playerPrefsData.blueValues[i] = PlayerPrefs.GetFloat("Blue" + i.ToString(), 1);
			//playerPrefsData.greenValues[i] = PlayerPrefs.GetFloat("Green" + i.ToString(), 1);
			//playerPrefsData.redGlowValues[i] = PlayerPrefs.GetFloat("RedGlow" + i.ToString(), 0);
			//playerPrefsData.blueGlowValues[i] = PlayerPrefs.GetFloat("BlueGlow" + i.ToString(), 0);
			//playerPrefsData.greenGlowValues[i] = PlayerPrefs.GetFloat("GreenGlow" + i.ToString(), 0);
			//playerPrefsData.redBrakeValues[i] = PlayerPrefs.GetFloat("RedBrake" + i.ToString(), 1);
			//playerPrefsData.blueBrakeValues[i] = PlayerPrefs.GetFloat("BlueBrake" + i.ToString(), 1);
			//playerPrefsData.greenBrakeValues[i] = PlayerPrefs.GetFloat("GreenBrake" + i.ToString(), 1);
			//playerPrefsData.redRimValues[i] = PlayerPrefs.GetFloat("RedRim" + i.ToString(), 1);
			//playerPrefsData.blueRimValues[i] = PlayerPrefs.GetFloat("BlueRim" + i.ToString(), 1);
			//playerPrefsData.greenRimValues[i] = PlayerPrefs.GetFloat("GreenRim" + i.ToString(), 1);
			//playerPrefsData.redGlassValues[i] = PlayerPrefs.GetFloat("RedGlass" + i.ToString(), 1);
			//playerPrefsData.blueGlassValues[i] = PlayerPrefs.GetFloat("BlueGlass" + i.ToString(), 1);
			//playerPrefsData.greenGlassValues[i] = PlayerPrefs.GetFloat("GreenGlass" + i.ToString(), 1);
			//playerPrefsData.alphaGlassValues[i] = PlayerPrefs.GetFloat("AlphaGlass" + i.ToString(), 0.5f);
			//carColor.a = carAlpha;
			//carColor.r = playerPrefsData.redValues[i];
			//carColor.b = playerPrefsData.blueValues[i];
			//carColor.g = playerPrefsData.greenValues[i];
			//playableVehicles.carMaterial[i].color = carColor;
			//carColor.a = 0.1f;
			//carColor.r = playerPrefsData.redGlowValues[i];
			//carColor.b = playerPrefsData.blueGlowValues[i];
			//carColor.g = playerPrefsData.greenGlowValues[i];
			//playableVehicles.carGlowLight[i].startColor = carColor;
			//sceneCarGlowLight[i].startColor = carColor;
			//carColor.a = carAlpha;
			//carColor.r = playerPrefsData.redBrakeValues[i];
			//carColor.b = playerPrefsData.blueBrakeValues[i];
			//carColor.g = playerPrefsData.greenBrakeValues[i];
			//playableVehicles.brakeMaterial[i].color = carColor;
			//carColor.a = carAlpha;
			//carColor.r = playerPrefsData.redRimValues[i];
			//carColor.b = playerPrefsData.blueRimValues[i];
			//carColor.g = playerPrefsData.greenRimValues[i];
			//playableVehicles.rimMaterial[i].color = carColor;
			//carColor.a = playerPrefsData.alphaGlassValues[i];
			//carColor.r = playerPrefsData.redGlassValues[i];
			//carColor.b = playerPrefsData.blueGlassValues[i];
			//carColor.g = playerPrefsData.greenGlassValues[i];
			//playableVehicles.glassMaterial[i].color = carColor;

			//playableVehicles.carGlowLight[i].startColor = carColor;
			///
			///
			///and these too
			//sceneCarGlowLight[i].gameObject.SetActive(false);
			//sceneCarGlowLight[i].gameObject.SetActive(true);

		}
		raceNumber = PlayerPrefs.GetInt ("Race Number", 0);
		for(int i = 0; i < raceData.numberOfRaces; i++){
			PlayerPrefs.GetInt("Race " + i.ToString() + " Laps", raceData.raceLaps[raceNumber]);
		}
		//raceLaps = PlayerPrefs.GetInt ("Race Laps", 1);
		vehicleNumber = PlayerPrefs.GetInt ("Vehicle Number", 0);
		currency = PlayerPrefs.GetInt ("Currency", playableVehicles.startingCurrency);
		//PlayerPrefs.SetInt ("Currency", currency);
		PlayerPrefs.SetInt ("NumberOfRaces", raceData.numberOfRaces);
		for(int i = 1; i < raceData.numberOfRaces; i++){
			PlayerPrefs.SetString( "RaceName" + i.ToString(), raceData.raceNames[i] );
            Debug.Log(PlayerPrefs.GetString("RaceLock" + raceData.raceNames[i]));
            //if(PlayerPrefs.GetString("RaceLock" + raceData.raceNames[i]) == "UNLOCKED" ){
            raceData.raceLocked[i] = false;
            //}else{
            //raceData.raceLocked[i] = true;
			//}
		}
		//raceDetails[raceNumber].bestPosition = PlayerPrefs.GetInt ("Best Position" + raceNumber.ToString(), 8);
		//raceDetails[raceNumber].bestTime = PlayerPrefs.GetFloat ("Best Time" + raceNumber.ToString(), 9999.99f);
		//raceDetails[raceNumber].bestLapTime = PlayerPrefs.GetFloat ("Best Lap Time" + raceNumber.ToString(), 9999.99f);
	}

	// Here we check to see if the player is in a car part color modification menu and update the cloor changes in real time as they adjust sliders
	public void Update(){
		if(Application.isPlaying){
			if(audioData.music.Length > 0){
				if(!garageAudioSource.isPlaying)		PlayNextAudioTrack ();
			}
			//if(colorChange)	CarColor ();
			//if(brakeColorChange) BrakeColor ();
			//if(rimColorChange) RimColor ();
			//if(glassColorChange) GlassColor ();
			//if(glowChange) GlowColor();
		}
	}

	// These methods are used for the main menu
	#region MainMenu Methods
	// Loads garage menu
	//public void Button_Play() {
	//	if (uI.mainMenuWindow.activeInHierarchy) {
	//		uI.mainMenuWindow.SetActive(false);
	//		uI.garageUI.SetActive(true);
	//		AudioMenuSelect();
	//	}
	//	else {
	//		if (PlayerPrefs.GetInt("Car Unlocked" + vehicleNumber.ToString(), 0) == 0){
	//			sceneCarModel[vehicleNumber].SetActive(false);
	//			vehicleNumber = 0;
	//			playableVehicles.currentVehicleNumber = vehicleNumber;
	//			uI.carNameText.text = playableVehicles.vehicleNames[vehicleNumber];
	//			sceneCarModel[vehicleNumber].SetActive(true);
	//		}
	//		uI.buyCarButton.SetActive(false);
	//		uI.buyCarConfirmWindow.SetActive(false);
	//		uI.priceUI.SetActive(false);
	//		uI.unlockedVehicleOptions.SetActive(true);
	//		for (int i = 0; i < playableVehicles.numberOfCars; i++){
	//			sceneCarModel[i].SetActive(false);
	//			if(i == vehicleNumber){
	//				sceneCarModel[i].SetActive(true);
	//			}
	//		}
	//		uI.garageUI.SetActive(false);
	//		//uI.mainMenuWindow.SetActive(true);
	//		//uI.topSpeedSlider.value = playableVehicles.topSpeedLevel[vehicleNumber];
	//		//uI.accelerationSlider.value = playableVehicles.torqueLevel[vehicleNumber];
	//		//uI.brakePowerSlider.value = playableVehicles.brakeTorqueLevel[vehicleNumber];
	//		//uI.tireTractionSlider.value = playableVehicles.tireTractionLevel[vehicleNumber];
	//		//uI.steerSensitivitySlidetr.value = playableVehicles.steerSensitivityLevel[vehicleNumber];
	//		AudioMenuBack();
	//	}
	//	UpdateCar();
	//}

	// Loads the options menu
	//public void Button_Settings() {
	//	if (uI.settingsMenuWindow.activeInHierarchy) {
	//		uI.menuWindow.SetActive(true);
	//		uI.settingsMenuWindow.SetActive(false);
	//		AudioMenuBack();
	//	}
	//	else {
	//		uI.settingsMenuWindow.SetActive(true);
	//		uI.menuWindow.SetActive(false);
	//		AudioMenuSelect();
	//	}
	//}

	// Prompts user with a quit confirm window
  //  public void Button_QuitGame() {
  //      if (quitConfirmIsOpen) {
		//	quitConfirmIsOpen = false;
		//	uI.quitConfirmWindow.SetActive(false);
		//} else {
		//	quitConfirmIsOpen = true;
  //          uI.quitConfirmWindow.SetActive(true);
		//}
  //  }

	//Closes the quit confirm window
    public void Button_QuitConfirm(){
		Application.Quit ();
	}

	//public void Button_Review() {
	//	Application.OpenURL(uI.reviewAddress);
	//}

	//public void Button_More() {
	//	Application.OpenURL(uI.developerAddress);
	//}
	#endregion

	// These methods open or close the selected garage sub-menu
	#region Garage Sub-Menus
	// Loads the Change Neon Glow menu for the selected car
	//public void Button_Glow() {
	//	if (glowChange) {
	//		glowChange = false;
	//		uI.glowWindow.SetActive(false);
	//		uI.glowConfirmWindow.SetActive(false);
	//		uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
	//		//uI.redGlowSlider.value = playerPrefsData.redGlowValues[vehicleNumber];
	//		//uI.blueGlowSlider.value = playerPrefsData.blueGlowValues[vehicleNumber];
	//		//uI.greenGlowSlider.value = playerPrefsData.greenGlowValues[vehicleNumber];
	//		GlowColor();
	//	} else {
	//		uI.glowWindow.SetActive(true);
	//		uI.garageWindow.SetActive(false);
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
	//		AudioMenuSelect();
	//		glowChange = true;
	//		//uI.redGlowSlider.value = playerPrefsData.redGlowValues[vehicleNumber];
	//		//uI.blueGlowSlider.value = playerPrefsData.blueGlowValues[vehicleNumber];
	//		//uI.greenGlowSlider.value = playerPrefsData.greenGlowValues[vehicleNumber];
	//		GlowColor();
	//	}
	//}

	//// Loads the Change Paint Color menu for the selected car
	//public void Button_Paint() {
	//	if (colorChange) {
	//		colorChange = false;
	//		uI.paintWindow.SetActive(false);
	//		uI.paintConfirmWindow.SetActive(false);
	//		uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
	//		//uI.redSlider.value = playerPrefsData.redValues[vehicleNumber];
	//		//uI.blueSlider.value = playerPrefsData.blueValues[vehicleNumber];
	//		//uI.greenSlider.value = playerPrefsData.greenValues[vehicleNumber];
	//		CarColor();
	//	}
	//	else {
	//		uI.paintWindow.SetActive(true);
	//		uI.garageWindow.SetActive(false);
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
	//		AudioMenuSelect();
	//		colorChange = true;
	//		//uI.redSlider.value = playerPrefsData.redValues[vehicleNumber];
	//		//uI.blueSlider.value = playerPrefsData.blueValues[vehicleNumber];
	//		//uI.greenSlider.value = playerPrefsData.greenValues[vehicleNumber];
	//		CarColor();
	//	}
	//}

	// Loads the Change Glass Color menu for the selected car
	//public void Button_GlassColor() {
	//	if (glassColorChange) {
	//		glassColorChange = false;
	//		uI.glassColorWindow.SetActive(false);
	//		uI.glassColorConfirmWindow.SetActive(false);
	//		uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
	//		//uI.redGlassSlider.value = playerPrefsData.redGlassValues[vehicleNumber];
	//		//uI.blueGlassSlider.value = playerPrefsData.blueGlassValues[vehicleNumber];
	//		//uI.greenGlassSlider.value = playerPrefsData.greenGlassValues[vehicleNumber];
	//		//uI.alphaGlassSlider.value = playerPrefsData.alphaGlassValues[vehicleNumber];
	//		BrakeColor();
	//	}
	//	else {
	//		uI.glassColorWindow.SetActive(true);
	//		uI.garageWindow.SetActive(false);
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
	//		AudioMenuSelect();
	//		glassColorChange = true;
	//		//uI.redGlassSlider.value = playerPrefsData.redGlassValues[vehicleNumber];
	//		//uI.blueGlassSlider.value = playerPrefsData.blueGlassValues[vehicleNumber];
	//		//uI.greenGlassSlider.value = playerPrefsData.greenGlassValues[vehicleNumber];
	//		//uI.alphaGlassSlider.value = playerPrefsData.alphaGlassValues[vehicleNumber];
	//		GlassColor();
	//	}
	//}

	// Loads the Change Brake Color menu for the selected car
	//public void Button_BrakeColor() {
	//	if (brakeColorChange) {
	//		brakeColorChange = false;
	//		uI.brakeColorWindow.SetActive(false);
	//		uI.brakeColorConfirmWindow.SetActive(false);
	//		uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
	//		//uI.redBrakeSlider.value = playerPrefsData.redBrakeValues[vehicleNumber];
	//		//uI.blueBrakeSlider.value = playerPrefsData.blueBrakeValues[vehicleNumber];
	//		//uI.greenBrakeSlider.value = playerPrefsData.greenBrakeValues[vehicleNumber];
	//		BrakeColor();
	//	}
	//	else {
	//		uI.brakeColorWindow.SetActive(true);
	//		uI.garageWindow.SetActive(false);
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
	//		AudioMenuSelect();
	//		brakeColorChange = true;
	//		//uI.redBrakeSlider.value = playerPrefsData.redBrakeValues[vehicleNumber];
	//		//uI.blueBrakeSlider.value = playerPrefsData.blueBrakeValues[vehicleNumber];
	//		//uI.greenBrakeSlider.value = playerPrefsData.greenBrakeValues[vehicleNumber];
	//		BrakeColor();
	//	}
	//}

	// Loads the Change Rim Color menu for the selected car
	//public void Button_RimColor() {
	//	if (rimColorChange) {
	//		rimColorChange = false;
	//		uI.rimColorWindow.SetActive(false);
	//		uI.rimColorConfirmWindow.SetActive(false);
	//		uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
	//		//uI.redRimSlider.value = playerPrefsData.redRimValues[vehicleNumber];
	//		//uI.blueRimSlider.value = playerPrefsData.blueRimValues[vehicleNumber];
	//		//uI.greenRimSlider.value = playerPrefsData.greenRimValues[vehicleNumber];
	//		RimColor();
	//	}
	//	else {
	//		uI.rimColorWindow.SetActive(true);
	//		uI.garageWindow.SetActive(false);
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
	//		AudioMenuSelect();
	//		rimColorChange = true;
	//		//uI.redRimSlider.value = playerPrefsData.redRimValues[vehicleNumber];
	//		//uI.blueRimSlider.value = playerPrefsData.blueRimValues[vehicleNumber];
	//		//uI.greenRimSlider.value = playerPrefsData.greenRimValues[vehicleNumber];
	//		RimColor();
	//	}
	//}
	#endregion

	// These methods check player currency and prompt player with a purchase confirmation window if the player has enough currency
	#region SelectItemToPurchase Methods

	//public void Button_BuyCar() {
 //       //uI.buyCarText.text = "Buy " + playableVehicles.vehicleNames[vehicleNumber].ToString() + "\nfor\n$" + playableVehicles.price[vehicleNumber].ToString("N0");
 //       if (currency >= playableVehicles.price[vehicleNumber])
 //           uI.carConfirmWindow.SetActive(true);
 //       AudioMenuSelect();
 //   }

 //   public void SelectGlow(){
	//	AudioMenuSelect ();
	//	if(currency >= playableVehicles.glowPrice)	uI.glowConfirmWindow.SetActive(true);
	//}    

 //   public void SelectPaint(){
	//	AudioMenuSelect ();
	//	if(currency >= playableVehicles.paintPrice)	uI.paintConfirmWindow.SetActive(true);
	//}

	//public void SelectBrakeColor(){
	//	AudioMenuSelect ();
	//	if(currency >= playableVehicles.brakeColorPrice)	uI.brakeColorConfirmWindow.SetActive(true);
	//}

	//public void SelectRimColor(){
	//	AudioMenuSelect ();
	//	if(currency >= playableVehicles.rimColorPrice)	uI.rimColorConfirmWindow.SetActive(true);
	//}

	//public void SelectGlassColor(){
	//	AudioMenuSelect ();
	//	if(currency >= playableVehicles.glassColorPrice)	uI.glassColorConfirmWindow.SetActive(true);
	//}
	#endregion

	// These methods are used to confirm a purchase and update PlayerPrefs data and other components with the changes
	#region AcceptPurchases Methods

	//public void AcceptPurchasePaint(){
	//	AudioMenuSelect ();
	//	currency -= playableVehicles.paintPrice;
	//	PlayerPrefs.SetInt("Currency", currency);
	//	//colorChange = false;
	//	//PlayerPrefs.SetFloat("Red" + vehicleNumber.ToString(), uI.redSlider.value);
	//	//PlayerPrefs.SetFloat("Blue" + vehicleNumber.ToString(), uI.blueSlider.value);
	//	//PlayerPrefs.SetFloat("Green" + vehicleNumber.ToString(), uI.greenSlider.value);
	//	//playerPrefsData.redValues[vehicleNumber] = uI.redSlider.value;
	//	//playerPrefsData.blueValues[vehicleNumber] = uI.blueSlider.value;
	//	//playerPrefsData.greenValues[vehicleNumber] = uI.greenSlider.value;
	//	uI.paintConfirmWindow.SetActive (false);
	//	uI.paintWindow.SetActive (false);
	//	//uI.customizationWindow.SetActive (true);
	//	uI.currencyText.text = currency.ToString("N0");
	//	uI.garageWindow.SetActive(true);
	//	uI.previousCarButton.SetActive (true);
	//	uI.nextCarButton.SetActive (true);
	//}

	//public void AcceptPurchaseGlow(){
	//	AudioMenuSelect ();
	//	currency -= playableVehicles.glowPrice;
	//	glowChange = false;
	//	//PlayerPrefs.SetInt("Currency", currency);
	//	//PlayerPrefs.SetFloat("RedGlow" + vehicleNumber.ToString(), uI.redGlowSlider.value);
	//	//PlayerPrefs.SetFloat("BlueGlow" + vehicleNumber.ToString(), uI.blueGlowSlider.value);
	//	//PlayerPrefs.SetFloat("GreenGlow" + vehicleNumber.ToString(), uI.greenGlowSlider.value);
	//	//playerPrefsData.redGlowValues[vehicleNumber] = uI.redGlowSlider.value;
	//	//playerPrefsData.blueGlowValues[vehicleNumber] = uI.blueGlowSlider.value;
	//	//playerPrefsData.greenGlowValues[vehicleNumber] = uI.greenGlowSlider.value;
	//	uI.glowConfirmWindow.SetActive (false);
	//	uI.glowWindow.SetActive (false);
	//	//uI.customizationWindow.SetActive (true);
	//	//playableVehicles.carGlowLight[vehicleNumber].startColor = carColor;
	//	uI.currencyText.text = currency.ToString("N0");
	//	uI.garageWindow.SetActive(true);
	//	uI.previousCarButton.SetActive (true);
	//	uI.nextCarButton.SetActive (true);
	//}

	//public void AcceptPurchaseBrakeColor(){
	//	AudioMenuSelect ();
	//	currency -= playableVehicles.brakeColorPrice;
	//	brakeColorChange = false;
	//	//PlayerPrefs.SetInt("Currency", currency);
	//	//PlayerPrefs.SetFloat("RedBrake" + vehicleNumber.ToString(), uI.redBrakeSlider.value);
	//	//PlayerPrefs.SetFloat("BlueBrake" + vehicleNumber.ToString(), uI.blueBrakeSlider.value);
	//	//PlayerPrefs.SetFloat("GreenBrake" + vehicleNumber.ToString(), uI.greenBrakeSlider.value);
	//	//playerPrefsData.redBrakeValues[vehicleNumber] = uI.redBrakeSlider.value;
	//	//playerPrefsData.blueBrakeValues[vehicleNumber] = uI.blueBrakeSlider.value;
	//	//playerPrefsData.greenBrakeValues[vehicleNumber] = uI.greenBrakeSlider.value;
	//	uI.brakeColorConfirmWindow.SetActive (false);
	//	uI.brakeColorWindow.SetActive (false);
	//	//uI.customizationWindow.SetActive (true);
	//	//playableVehicles.carGlowLight[vehicleNumber].startColor = carColor;
	//	uI.currencyText.text = currency.ToString("N0");
	//	uI.garageWindow.SetActive(true);
	//	uI.previousCarButton.SetActive (true);
	//	uI.nextCarButton.SetActive (true);
	//}

	//public void AcceptPurchaseRimColor(){
	//	AudioMenuSelect ();
	//	currency -= playableVehicles.rimColorPrice;
	//	rimColorChange = false;
	//	//PlayerPrefs.SetInt("Currency", currency);
	//	//PlayerPrefs.SetFloat("RedRim" + vehicleNumber.ToString(), uI.redRimSlider.value);
	//	//PlayerPrefs.SetFloat("BlueRim" + vehicleNumber.ToString(), uI.blueRimSlider.value);
	//	//PlayerPrefs.SetFloat("GreenRim" + vehicleNumber.ToString(), uI.greenRimSlider.value);
	//	//playerPrefsData.redRimValues[vehicleNumber] = uI.redRimSlider.value;
	//	//playerPrefsData.blueRimValues[vehicleNumber] = uI.blueRimSlider.value;
	//	//playerPrefsData.greenRimValues[vehicleNumber] = uI.greenRimSlider.value;
	//	uI.rimColorConfirmWindow.SetActive (false);
	//	uI.rimColorWindow.SetActive (false);
	//	uI.currencyText.text = currency.ToString("N0");
	//	uI.garageWindow.SetActive(true);
	//	uI.previousCarButton.SetActive (true);
	//	uI.nextCarButton.SetActive (true);
	//}

	//public void AcceptPurchaseGlassColor(){
	//	AudioMenuSelect ();
	//	currency -= playableVehicles.glassColorPrice;
	//	glassColorChange = false;
	//	//PlayerPrefs.SetInt("Currency", currency);  k
	//	//PlayerPrefs.SetFloat("RedGlass" + vehicleNumber.ToString(), uI.redGlassSlider.value);
	//	//PlayerPrefs.SetFloat("BlueGlass" + vehicleNumber.ToString(), uI.blueGlassSlider.value);
	//	//PlayerPrefs.SetFloat("GreenGlass" + vehicleNumber.ToString(), uI.greenGlassSlider.value);
	//	//PlayerPrefs.SetFloat("AlphaGlass" + vehicleNumber.ToString(), uI.alphaGlassSlider.value);
	//	    //playerPrefsData.redGlassValues[vehicleNumber] = uI.redGlassSlider.value;
	//	    //playerPrefsData.blueGlassValues[vehicleNumber] = uI.blueGlassSlider.value;
	//	    //playerPrefsData.greenGlassValues[vehicleNumber] = uI.greenGlassSlider.value;
	//	    //playerPrefsData.alphaGlassValues[vehicleNumber] = uI.alphaGlassSlider.value;
	//	uI.glassColorConfirmWindow.SetActive (false);
	//	uI.glassColorWindow.SetActive (false);
	//	uI.currencyText.text = currency.ToString("N0");
	//	uI.garageWindow.SetActive(true);
	//	uI.previousCarButton.SetActive (true);
	//	uI.nextCarButton.SetActive (true);
	//}

	//public void Button_AcceptUpgrade() {
	//	////if (uI.upgradeConfirmText.text == "Upgrade " + "Top Speed" + "\nfor\n$" + playableVehicles.upgradeSpeedPrice.ToString("N0")) {
	//	//	currency -= playableVehicles.upgradeSpeedPrice;
	//	//	playableVehicles.topSpeedLevel[vehicleNumber] += 1;
	//	//	PlayerPrefs.SetInt("CarTopSpeedLevel" + vehicleNumber.ToString(), playableVehicles.topSpeedLevel[vehicleNumber]);
	//	//}
	//	////if (uI.upgradeConfirmText.text == "Upgrade " + "Acceleration" + "\nfor\n$" + playableVehicles.upgradeAccelerationPrice.ToString("N0")) {
	//	//	currency -= playableVehicles.upgradeAccelerationPrice;
	//	//	playableVehicles.torqueLevel[vehicleNumber] += 1;
	//	//	PlayerPrefs.SetInt("CarTorqueLevel" + vehicleNumber.ToString(), playableVehicles.torqueLevel[vehicleNumber]);
	//	//}
	//	//if (uI.upgradeConfirmText.text == "Upgrade " + "Brake Power" + "\nfor\n$" + playableVehicles.upgradeBrakesPrice.ToString("N0")) {
	//	//	currency -= playableVehicles.upgradeBrakesPrice;
	//	//	playableVehicles.brakeTorqueLevel[vehicleNumber] += 1;
	//	//	PlayerPrefs.SetInt("CarBrakeLevel" + vehicleNumber.ToString(), playableVehicles.brakeTorqueLevel[vehicleNumber]);
	//	//}
	//	//if (uI.upgradeConfirmText.text == "Upgrade " + "Tire Traction" + "\nfor\n$" + playableVehicles.upgradeTiresPrice.ToString("N0")) {
	//	//	currency -= playableVehicles.upgradeTiresPrice;
	//	//	playableVehicles.tireTractionLevel[vehicleNumber] += 1;
	//	//	PlayerPrefs.SetInt("CarTireTractionLevel" + vehicleNumber.ToString(), playableVehicles.tireTractionLevel[vehicleNumber]);
	//	//}
	//	//if (uI.upgradeConfirmText.text == "Upgrade " + "Steer Sensitivity" + "\nfor\n$" + playableVehicles.upgradeSteeringPrice.ToString("N0")) {
	//	//	currency -= playableVehicles.upgradeSteeringPrice;
	//	//	playableVehicles.steerSensitivityLevel[vehicleNumber] += 1;
	//	//	PlayerPrefs.SetInt("CarSteerSensitivityLevel" + vehicleNumber.ToString(), playableVehicles.steerSensitivityLevel[vehicleNumber]);
	//	//}
	//	//uI.upgradesConfirmWindow.SetActive(false);
	//	//PlayerPrefs.SetInt("Currency", currency);
	//	//UpdateCar();

	//}
	#endregion
   
	// These methods are used to change the color of a car part
	#region ColorChange Methods
	//public void GlassColor(){
	//	carColor.a = uI.alphaGlassSlider.value;
	//	carColor.r = uI.redGlassSlider.value;
	//	carColor.b = uI.blueGlassSlider.value;
	//	carColor.g = uI.greenGlassSlider.value;
	//	//playableVehicles.glassMaterial[vehicleNumber].color = carColor;
	//}

	//public void BrakeColor(){
	//	carColor.a = carAlpha;
	//	carColor.r = uI.redBrakeSlider.value;
	//	carColor.b = uI.blueBrakeSlider.value;
	//	carColor.g = uI.greenBrakeSlider.value;
	//	//playableVehicles.brakeMaterial[vehicleNumber].color = carColor;
	//}

	//public void RimColor(){
	//	carColor.a = carAlpha;
	//	carColor.r = uI.redRimSlider.value;
	//	carColor.b = uI.blueRimSlider.value;
	//	carColor.g = uI.greenRimSlider.value;
	//	//playableVehicles.rimMaterial[vehicleNumber].color = carColor;
	//}

	//public void CarColor(){
	//	carColor.a = carAlpha;
	//	carColor.r = uI.redSlider.value;
	//	carColor.b = uI.blueSlider.value;
	//	carColor.g = uI.greenSlider.value;
	//	//playableVehicles.carMaterial[vehicleNumber].color = carColor;
	//}

	//public void GlowColor(){
	//	if (carColor.g != uI.greenGlowSlider.value  || carColor.r != uI.redGlowSlider.value || carColor.b != uI.blueGlowSlider.value) {
	//		carColor.a = 0.1f;
	//		carColor.r = uI.redGlowSlider.value;
	//		carColor.b = uI.blueGlowSlider.value;
	//		carColor.g = uI.greenGlowSlider.value;
	//		//sceneCarGlowLight[vehicleNumber].startColor = carColor;

	//		///Reset the particle effect
	//		//sceneCarGlowLight[vehicleNumber].gameObject.SetActive(false);
	//		//sceneCarGlowLight[vehicleNumber].gameObject.SetActive(true);
	//	}
	//}
	#endregion

	// These methods can be called to play an audio sound
	#region PlayAudio Methods
	void AudioMusic(){
		if (audioData.music.Length > 0) {
			emptyObject = new GameObject ("Audio Clip: Music");
			emptyObject.transform.parent = audioContainer.transform;
			emptyObject.AddComponent<AudioSource> ();
			garageAudioSource = emptyObject.GetComponent<AudioSource> ();
			audioData.currentAudioTrack = 0;
			garageAudioSource.clip = audioData.music [audioData.currentAudioTrack];
			garageAudioSource.loop = false;
			garageAudioSource.Play ();
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
		garageAudioSource.clip = audioData.music [audioData.currentAudioTrack];
		garageAudioSource.Play ();
	}

	public void AudioMenuSelect(){
		emptyObject = new GameObject ("Audio Clip: Menu Select");
		emptyObject.transform.parent = audioContainer.transform;
		emptyObject.AddComponent<AudioSource>();
		emptyObject.GetComponent<AudioSource>().clip = audioData.menuSelect;
		emptyObject.GetComponent<AudioSource>().loop = false;
		emptyObject.GetComponent<AudioSource>().Play ();
		emptyObject.AddComponent<DestroyAudio>();
		emptyObject = null;
	}

	public void AudioMenuBack(){
		emptyObject = new GameObject ("Audio Clip: Menu Back");
		emptyObject.transform.parent = audioContainer.transform;
		emptyObject.AddComponent<AudioSource>();
		emptyObject.GetComponent<AudioSource>().clip = audioData.menuBack;
		emptyObject.GetComponent<AudioSource>().loop = false;
		emptyObject.GetComponent<AudioSource>().Play ();
		emptyObject.AddComponent<DestroyAudio>();
		emptyObject = null;
	}
	#endregion

	//public void Button_Upgrades() {
 //       if (upgradeChange) {
 //           upgradeChange = false;
 //           uI.upgradesWindow.SetActive(false);
 //           uI.upgradesConfirmWindow.SetActive(false);
 //           uI.garageWindow.SetActive(true);
	//		uI.previousCarButton.SetActive (true);
	//		uI.nextCarButton.SetActive (true);
 //       }
 //       else {
	//		uI.previousCarButton.SetActive (false);
	//		uI.nextCarButton.SetActive (false);
 //           uI.upgradesWindow.SetActive(true);
 //           uI.garageWindow.SetActive(false);
 //           AudioMenuSelect();
 //           upgradeChange = true;
 //       }
 //   }

  //  public void Button_TopSpeed() {
  //      AudioMenuSelect();
		//uI.upgradeConfirmText.text = "Upgrade " + "Top Speed" + "\nfor\n$" + playableVehicles.upgradeSpeedPrice.ToString("N0");
		//if (currency >= playableVehicles.upgradeSpeedPrice) uI.upgradesConfirmWindow.SetActive(true);
  //  }

  //  public void Button_Acceleration() {
  //      AudioMenuSelect();
		//uI.upgradeConfirmText.text = "Upgrade " + "Acceleration" + "\nfor\n$" + playableVehicles.upgradeAccelerationPrice.ToString("N0");
		//if (currency >= playableVehicles.upgradeAccelerationPrice) uI.upgradesConfirmWindow.SetActive(true);
  //  }

  //  public void Button_BrakePower() {
  //      AudioMenuSelect();
		//uI.upgradeConfirmText.text = "Upgrade " + "Brake Power" + "\nfor\n$" + playableVehicles.upgradeBrakesPrice.ToString("N0");
		//if (currency >= playableVehicles.upgradeBrakesPrice) uI.upgradesConfirmWindow.SetActive(true);
  //  }

  //  public void Button_TireTraction() {
  //      AudioMenuSelect();
		//uI.upgradeConfirmText.text = "Upgrade " + "Tire Traction" + "\nfor\n$" + playableVehicles.upgradeTiresPrice.ToString("N0");
		//if (currency >= playableVehicles.upgradeTiresPrice) uI.upgradesConfirmWindow.SetActive(true);
  //  }

  //  public void Button_SteerSensitivity() {
  //      AudioMenuSelect();
		//uI.upgradeConfirmText.text = "Upgrade " + "Steer Sensitivity" + "\nfor\n$" + playableVehicles.upgradeSteeringPrice.ToString("N0");
		//if (currency >= playableVehicles.upgradeSteeringPrice) uI.upgradesConfirmWindow.SetActive(true);
  //  }
		
	// Call this to cancel a purchase confirmation
 //   public void DeclinePurchase(){
	//	uI.carConfirmWindow.SetActive(false);
	//	uI.paintConfirmWindow.SetActive (false);
	//	uI.glowConfirmWindow.SetActive (false);
	//	uI.glassColorConfirmWindow.SetActive(false);
	//	uI.brakeColorConfirmWindow.SetActive(false);
	//	uI.rimColorConfirmWindow.SetActive(false);
	//	uI.unlockRaceConfirmWindow.SetActive (false);
 //       uI.upgradesConfirmWindow.SetActive(false);
 //       AudioMenuBack();
	//}

	//public void AcceptPurchase(){
	//	uI.carConfirmWindow.SetActive(false);
	//	PlayerPrefs.SetInt("Car Unlocked" + vehicleNumber.ToString(), 1);
	//	playableVehicles.carUnlocked[vehicleNumber] = true;
	//	currency -= playableVehicles.price[vehicleNumber];
	//	PlayerPrefs.SetInt("Currency", currency);
	//	PlayerPrefs.SetInt ("Vehicle Number", vehicleNumber);
	//	uI.currencyText.text = currency.ToString("N0");
 //       uI.priceUI.SetActive(false);
 //       uI.buyCarButton.SetActive(false);
 //       uI.unlockedVehicleOptions.SetActive(true);
 //   }

	public void NextCar(){
        //uI.buyCarConfirmWindow.SetActive(false);
        AudioMenuSelect ();
		sceneCarModel[vehicleNumber].SetActive(false);
		if (vehicleNumber < playableVehicles.numberOfCars - 1) {
			vehicleNumber += 1;
			sceneCarModel[vehicleNumber].SetActive (true);
		} else {
			vehicleNumber = 0;
			sceneCarModel[vehicleNumber].SetActive (true);
		}
		uI.carNameText.text = playableVehicles.vehicleNames[vehicleNumber];
		if (playableVehicles.carUnlocked[vehicleNumber]) {
            //uI.buyCarButton.SetActive(false);
            PlayerPrefs.SetInt("Vehicle Number", vehicleNumber);
            //uI.priceUI.SetActive(false);
            //uI.unlockedVehicleOptions.SetActive(true);            
        } else {
            //uI.priceUI.SetActive(true);
            //uI.unlockedVehicleOptions.SetActive(false);
            //uI.selectCarText.text = "$" + playableVehicles.price[vehicleNumber].ToString("N0");
            //uI.buyCarButton.SetActive(true);
		}
		playableVehicles.currentVehicleNumber = vehicleNumber;
        UpdateCar();
    }

	public void PreviousCar(){
        //uI.buyCarConfirmWindow.SetActive(false);
        AudioMenuSelect ();
		sceneCarModel[vehicleNumber].SetActive(false);
		if (vehicleNumber > 0) {
			vehicleNumber -= 1;
			sceneCarModel[vehicleNumber].SetActive (true);
		} else {
			vehicleNumber = playableVehicles.numberOfCars - 1;
			sceneCarModel[vehicleNumber].SetActive (true);
		}
		uI.carNameText.text = playableVehicles.vehicleNames[vehicleNumber];
		if(playableVehicles.carUnlocked[vehicleNumber]){
            //uI.buyCarButton.SetActive(false);
            PlayerPrefs.SetInt("Vehicle Number", vehicleNumber);
            //uI.priceUI.SetActive(false);
            //uI.unlockedVehicleOptions.SetActive(true);
        }
        else {
            //uI.priceUI.SetActive(true);
            //uI.unlockedVehicleOptions.SetActive(false);
            //uI.selectCarText.text = "$" + playableVehicles.price[vehicleNumber].ToString("N0");
            //uI.buyCarButton.SetActive(true);
        }
		playableVehicles.currentVehicleNumber = vehicleNumber;
        UpdateCar();
    }

    public void UpdateCar() {
        //uI.currencyText.text = currency.ToString("N0");

        //uI.carSpeedText.text = (playableVehicles.vehicles[vehicleNumber].GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().m_Topspeed +
        //    (playableVehicles.topSpeedLevel[vehicleNumber] * playableVehicles.vehicles[vehicleNumber].GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().levelBonusTopSpeed)
        //).ToString() + " MPH";
       
        //uI.topSpeedSlider.value = playableVehicles.topSpeedLevel[vehicleNumber];
        //uI.accelerationSlider.value = playableVehicles.torqueLevel[vehicleNumber];
        //uI.brakePowerSlider.value = playableVehicles.brakeTorqueLevel[vehicleNumber];
        //uI.tireTractionSlider.value = playableVehicles.tireTractionLevel[vehicleNumber];
        //uI.steerSensitivitySlidetr.value = playableVehicles.steerSensitivityLevel[vehicleNumber];
        //if (playableVehicles.topSpeedLevel[vehicleNumber] >= 9) { 
        //    uI.upgradeTopSpeedButton.SetActive(false);
        //}
        //else {
        //    uI.upgradeTopSpeedButton.SetActive(true);
        //}
        //if (playableVehicles.torqueLevel[vehicleNumber] >= 9) { 
        //    uI.upgradeAccelerationButton.SetActive(false);
        //}else {
        //    uI.upgradeAccelerationButton.SetActive(true);
        //}
        //if (playableVehicles.brakeTorqueLevel[vehicleNumber] >= 9){
        //    uI.upgradeBrakePowerButton.SetActive(false);
        //}else {
        //    uI.upgradeBrakePowerButton.SetActive(true);
        //}
        //if (playableVehicles.tireTractionLevel[vehicleNumber] >= 9) { 
        //    uI.upgradeTireTractionButton.SetActive(false);
        //}else {
        //    uI.upgradeTireTractionButton.SetActive(true);
        //}
        //if (playableVehicles.steerSensitivityLevel[vehicleNumber] >= 9) { 
        //    uI.upgradeSteerSensitivityButton.SetActive(false);
        //}else {
        //    uI.upgradeSteerSensitivityButton.SetActive(true);
        //}
    }

    public GameObject Vehicles;
	#region RaceSelection methods
	public void Button_RaceSelection(){
		if (uI.garageWindow.activeInHierarchy) {
			uI.garageWindow.SetActive (false);
			uI.racesWindow.SetActive (true);
			uI.previousCarButton.SetActive (false);
			uI.nextCarButton.SetActive (false);
			AudioMenuSelect ();
            Vehicles.SetActive(false);
        } else {
            Vehicles.SetActive(true);
			uI.garageWindow.SetActive (true);
			uI.racesWindow.SetActive (false);
			uI.previousCarButton.SetActive (true);
			uI.nextCarButton.SetActive (true);
			AudioMenuBack ();
		}
        Debug.Log("adsdas");
	}

	public void SelectRace(){
		AudioMenuSelect ();
		if (!raceData.raceLocked[raceNumber]) {
			PlayerPrefs.SetString ("Game Mode", "SINGLE PLAYER");
			//uI.loadingWindow.SetActive(true);
			PlayerPrefs.SetInt ("Race Number", raceNumber);
			PlayerPrefs.SetInt ("Race Reward1", raceData.firstPrize[raceNumber] * raceData.raceLaps [raceNumber] * raceData.numberOfRacers[raceNumber]);
			PlayerPrefs.SetInt ("Race Reward2", raceData.secondPrize[raceNumber] * raceData.raceLaps [raceNumber] * raceData.numberOfRacers[raceNumber]);
			PlayerPrefs.SetInt ("Race Reward3", raceData.thirdPrize[raceNumber] * raceData.raceLaps [raceNumber] * raceData.numberOfRacers[raceNumber]);
			raceNameToLoad = raceNumber.ToString () + raceData.raceNames[raceNumber];
			SceneManager.LoadScene(raceNameToLoad); 
		} else {

		}
	}

	public void NextRace(){
		AudioMenuSelect ();
		raceImage[raceNumber].SetActive(false);
		if (raceNumber < raceData.numberOfRaces - 1) {
			raceNumber += 1;
			raceImage[raceNumber].SetActive (true);
		} else {
			raceNumber = 0;
			raceImage[raceNumber].SetActive (true);
		}
		uI.raceNameText.text = raceData.raceNames[raceNumber];
		uI.lapText.text = "Laps\n" + raceData.raceLaps [raceNumber].ToString ();
		uI.numberOfRacersText.text = "Racers\n" + raceData.numberOfRacers [raceNumber].ToString();

		if (!raceData.raceLocked[raceNumber]) {
			//uI.raceDetailsWindow.SetActive(true);
			uI.raceDetails.SetActive (true);
			uI.selectRaceText.text = "RACE";
			uI.raceLockedIcon.SetActive(false);
			CalculateRewardText ();
		}else {
			uI.raceDetails.SetActive (false);
			uI.unlockLevelButtonText.text = "Unlock\n" + raceData.unlockAmount[raceNumber].ToString("C0");
			uI.selectRaceText.text = lockButtonText;
			uI.raceLockedIcon.SetActive(true);
			//uI.unlockLevelText.text = "Unlock " + raceData.raceNames[raceNumber] + "\nfor\n" + raceData.unlockAmount[raceNumber].ToString("C0");
		}
	}

	public void PreviousRace(){
		AudioMenuSelect ();
		raceImage[raceNumber].SetActive(false);
		if (raceNumber > 0) {
			raceNumber -= 1;
			raceImage[raceNumber].SetActive (true);
		} else {
			raceNumber = raceData.numberOfRaces - 1;
			raceImage[raceNumber].SetActive (true);
		}
		uI.raceNameText.text = raceData.raceNames[raceNumber];
		uI.lapText.text = "Laps\n" + raceData.raceLaps [raceNumber].ToString ();
		uI.numberOfRacersText.text = "Racers\n" + raceData.numberOfRacers [raceNumber].ToString();
		if(!raceData.raceLocked[raceNumber]){
			//uI.raceDetailsWindow.SetActive(true);
			CalculateRewardText ();
			uI.selectRaceText.text = "RACE";
			uI.raceLockedIcon.SetActive(false);
		}else {
			uI.unlockLevelButtonText.text = "Unlock\n" + raceData.unlockAmount[raceNumber].ToString("C0");
			uI.selectRaceText.text = lockButtonText;
			uI.raceLockedIcon.SetActive(true);
			//uI.unlockLevelText.text = "Unlock " + raceData.raceNames[raceNumber] + "\nfor\n" + raceData.unlockAmount[raceNumber].ToString("C0");
		}
	}

	//public void UnlockRaceButton(){
	//	if (currency >= raceData.unlockAmount[raceNumber]) {
	//		uI.unlockRaceConfirmWindow.SetActive (true);
	//	}
	//}

	public void AcceptPurchaseUnlockRace(){
		AudioMenuSelect ();
		currency -= raceData.unlockAmount[raceNumber];
		PlayerPrefs.SetInt("Currency", currency);
		PlayerPrefs.SetString ("RaceLock" + raceData.raceNames[raceNumber], "UNLOCKED");
		raceData.raceLocked[raceNumber] = false;
		uI.raceLockedIcon.SetActive(false);
		//uI.raceDetailsWindow.SetActive(true);
		uI.selectRaceText.text = "Select Race";
		//uI.unlockRaceConfirmWindow.SetActive (false);
		uI.currencyText.text = currency.ToString("N0");
	}

	public void LapIncrease(){
		if (raceData.raceLaps [raceNumber] < raceData.lapLimit [raceNumber]) {
			raceData.raceLaps [raceNumber] += 1;
		} else {
			raceData.raceLaps [raceNumber] = 1;
		}
		uI.lapText.text = "Laps\n" + raceData.raceLaps [raceNumber].ToString ();
		CalculateRewardText ();
		PlayerPrefs.SetInt("Race " + raceNumber.ToString() + " Laps", raceData.raceLaps[raceNumber]);
	}

	public void LapDecrease(){
		if (raceData.raceLaps [raceNumber] > 1) {
			raceData.raceLaps [raceNumber] -= 1;
		} else {
			raceData.raceLaps [raceNumber] = raceData.lapLimit [raceNumber];
		}
		uI.lapText.text = "Laps\n" + raceData.raceLaps [raceNumber].ToString ();
		CalculateRewardText ();
		PlayerPrefs.SetInt("Race " + raceNumber.ToString() + " Laps", raceData.raceLaps[raceNumber]);
	}

	public void NumberOfRacersIncrease(){
		if (raceData.numberOfRacers[raceNumber] < raceData.racerLimit[raceNumber]) {
			raceData.numberOfRacers [raceNumber] += 1;
		} else {
			raceData.numberOfRacers [raceNumber] = 1;
		}
		uI.numberOfRacersText.text = "Racers\n" + raceData.numberOfRacers [raceNumber].ToString();
		CalculateRewardText ();
	}

	public void NumberOfRacersDecrease(){
		if (raceData.numberOfRacers [raceNumber] > 1) {
			raceData.numberOfRacers [raceNumber] -= 1;
		} else {
			raceData.numberOfRacers [raceNumber] = raceData.racerLimit [raceNumber];
		}
		uI.numberOfRacersText.text = "Racers\n" + raceData.numberOfRacers [raceNumber].ToString();
		CalculateRewardText ();
	}

	//public void LoadOpenWorldButton(){
	//	uI.loadingWindow.SetActive(true);
	//	PlayerPrefs.SetString ("Game Mode", "SINGLE PLAYER");
	//	SceneManager.LoadScene(openWorldName);
	//}
	#endregion

//	public void Button_MultiplayerGame(){
//		if (uI.multiplayerWindow.activeInHierarchy) {
//			uI.multiplayerWindow.SetActive (false);
//			uI.garageWindow.SetActive (true);
//			uI.previousCarButton.SetActive (true);
//			uI.nextCarButton.SetActive (true);
////			uI.multiplayerImage.SetActive (false);
//			AudioMenuSelect ();
//		} else {	
//			PlayerPrefs.SetString ("Game Mode", "MULTIPLAYER");
//			uI.garageWindow.SetActive (false);
//			uI.multiplayerWindow.SetActive (true);
//			uI.previousCarButton.SetActive (false);
//			uI.nextCarButton.SetActive (false);
////			uI.multiplayerImage.SetActive (true);
//			AudioMenuBack ();
//		}
//	}

	public void Button_BackLobby(){
		//uI.multiplayerWindow.SetActive (false);
		uI.garageWindow.SetActive (true);
		uI.previousCarButton.SetActive (true);
		uI.nextCarButton.SetActive (true);
//		uI.multiplayerImage.SetActive (false);
	}

	public void ReloadGarageScene(){
		PlayerPrefs.SetString ("HOSTDROP", "SCENERELOADED");
		Destroy (lobbyManager);
		SceneManager.LoadScene ("Garage");
	}

	void CalculateRewardText(){
		int firstPrize = (raceData.raceLaps [raceNumber] * raceData.firstPrize [raceNumber] * raceData.numberOfRacers [raceNumber]);
		int secondPrize;
		int thirdPrize;
		if (raceData.numberOfRacers [raceNumber] > 1) {
			secondPrize = (raceData.raceLaps [raceNumber] * raceData.secondPrize [raceNumber] * raceData.numberOfRacers [raceNumber]);
		} else {
			secondPrize = 0;
		}
		if (raceData.numberOfRacers [raceNumber] > 2) {
			thirdPrize = (raceData.raceLaps [raceNumber] * raceData.thirdPrize [raceNumber] * raceData.numberOfRacers [raceNumber]);
		} else {
			thirdPrize = 0;
		}

		uI.rewardText.text = "\n" + firstPrize.ToString("C0") + "\n" + secondPrize.ToString("C0") + "\n" + thirdPrize.ToString("C0");
	}

}