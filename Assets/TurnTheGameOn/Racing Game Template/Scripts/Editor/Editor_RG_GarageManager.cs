using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(RG_GarageManager))]
public class Editor_RG_GarageManager : Editor {

	bool races;
	bool cars;
	bool info;
	int editorRaceView;
	int editorCarView;

	public override void OnInspectorGUI(){
		//RG_GarageManager rg_GarageManager = (RG_GarageManager)target;
		RG_GarageManager rg_gameManager = (RG_GarageManager)target;
		rg_gameManager.transform.hideFlags = HideFlags.HideInInspector;

		Texture racingAssetTexture = Resources.Load("RacingAssetTexture") as Texture;

		GUIStyle inspectorStyle = new GUIStyle(GUI.skin.label);
		inspectorStyle.fixedWidth = 256;
		inspectorStyle.fixedHeight = 64;
		inspectorStyle.margin = new RectOffset( (int)(( (Screen.width * 0.98f)  - 264) / 2), 0, 0, 0);
		GUILayout.Label(racingAssetTexture,inspectorStyle);


		EditorGUILayout.BeginVertical("Box");
		
		EditorGUILayout.BeginHorizontal();

		GUISkin editorSkin = Resources.Load("EditorSkin") as GUISkin;
		GUI.skin = editorSkin;
		editorSkin.button.active.textColor = Color.green;
		if (races) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}
		
		if (GUILayout.Button ("Races", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			races = true;
			cars = false;
			info = false;
		}
		
		if (cars) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}
		if (GUILayout.Button ("Cars", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			cars = true;
			races = false;
			info = false;
		}
		
		if (info) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}
		if (GUILayout.Button ("Info", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			info = true;
			cars = false;
			races = false;
		}
		editorSkin.button.normal.textColor = Color.white;

		EditorGUILayout.EndHorizontal ();

		if(races){
			SerializedProperty configureRaceSize = serializedObject.FindProperty ("configureRaceSize");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (configureRaceSize, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			
			if(rg_gameManager.configureRaceSize == RG_GarageManager.Switch.On){
				EditorGUILayout.HelpBox("When you reduce this number the values of the affected arrays are deleted. Only reduce this number if you want fewer races."
				                        , MessageType.Warning);
				EditorGUI.BeginChangeCheck ();
				rg_gameManager.raceData.numberOfRaces = EditorGUILayout.IntField("Number Of Races", rg_gameManager.raceData.numberOfRaces);
				if (EditorGUI.EndChangeCheck ()){
					serializedObject.ApplyModifiedProperties ();
					serializedObject.ApplyModifiedProperties ();
				}
				if(editorRaceView >= rg_gameManager.raceData.numberOfRaces){
					editorRaceView = 0;
				}
				System.Array.Resize (ref rg_gameManager.raceData.raceNames, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.numberOfRacers, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.raceLaps, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.lapLimit, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.racerLimit, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.raceLocked, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.unlockAmount, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.firstPrize, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.secondPrize, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceData.thirdPrize, rg_gameManager.raceData.numberOfRaces);
				System.Array.Resize (ref rg_gameManager.raceImage, rg_gameManager.raceData.numberOfRaces);
			}

			EditorGUILayout.BeginVertical("Box");

			EditorGUILayout.BeginHorizontal("Box");
			
			if (GUILayout.Button ("<", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
				if(editorRaceView > 0)
					editorRaceView -= 1;
			}

			GUILayout.Box("Race Number\n" + editorRaceView.ToString(), GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) );

			if (GUILayout.Button (">", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
				if(editorRaceView < (rg_gameManager.raceData.numberOfRaces - 1))
				editorRaceView += 1;
			}
			EditorGUILayout.EndHorizontal();

			rg_gameManager.raceData.raceNames[editorRaceView] = EditorGUILayout.TextField("Race Name", rg_gameManager.raceData.raceNames[editorRaceView]);
			rg_gameManager.raceData.raceLocked[editorRaceView] = EditorGUILayout.Toggle("Race Locked", rg_gameManager.raceData.raceLocked[editorRaceView]);
			if(rg_gameManager.purchaseLevelUnlock == RG_GarageManager.Switch.On)
				rg_gameManager.raceData.unlockAmount[editorRaceView] = EditorGUILayout.IntField("Unlock Amount", rg_gameManager.raceData.unlockAmount[editorRaceView]);
			rg_gameManager.raceData.firstPrize[editorRaceView] = EditorGUILayout.IntField("First Prize", rg_gameManager.raceData.firstPrize[editorRaceView]);
			rg_gameManager.raceData.secondPrize[editorRaceView] = EditorGUILayout.IntField("Second Prize", rg_gameManager.raceData.secondPrize[editorRaceView]);
			rg_gameManager.raceData.thirdPrize[editorRaceView] = EditorGUILayout.IntField("Third Prize", rg_gameManager.raceData.thirdPrize[editorRaceView]);

			rg_gameManager.raceData.numberOfRacers[editorRaceView] = EditorGUILayout.IntField("Number of Racers", rg_gameManager.raceData.numberOfRacers[editorRaceView], GUILayout.MaxWidth (Screen.width * 0.7f));
			rg_gameManager.raceData.racerLimit[editorRaceView] = EditorGUILayout.IntField("Racer Limit", rg_gameManager.raceData.racerLimit[editorRaceView], GUILayout.MaxWidth (Screen.width * 0.7f));

			rg_gameManager.raceData.raceLaps[editorRaceView] = EditorGUILayout.IntField("Number of Laps", rg_gameManager.raceData.raceLaps[editorRaceView], GUILayout.MaxWidth (Screen.width * 0.7f));
			rg_gameManager.raceData.lapLimit[editorRaceView] = EditorGUILayout.IntField("Lap Limit", rg_gameManager.raceData.lapLimit[editorRaceView], GUILayout.MaxWidth (Screen.width * 0.7f));
			rg_gameManager.raceImage[editorRaceView] = (GameObject) EditorGUILayout.ObjectField("UI Race Image", rg_gameManager.raceImage[editorRaceView], typeof (GameObject), true ) as GameObject;

			//rg_gameManager.raceData.raceImage[editorRaceView] = (GameObject) EditorGUILayout.ObjectField("Race Image", rg_gameManager.raceData.raceImage[editorRaceView], typeof (GameObject), false );
			//SerializedObject raceData = new SerializedObject (rg_gameManager.raceData);

			EditorGUILayout.EndVertical();

			SerializedProperty purchaseLevelUnlock = serializedObject.FindProperty ("purchaseLevelUnlock");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (purchaseLevelUnlock, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			
			SerializedProperty autoUnlockNextRace = serializedObject.FindProperty ("autoUnlockNextRace");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (autoUnlockNextRace, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

			SerializedProperty lockButtonText = serializedObject.FindProperty ("lockButtonText");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (lockButtonText, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

		}

		if (cars) {

			SerializedProperty configureCarSize = serializedObject.FindProperty ("configureCarSize");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (configureCarSize, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			
			if(rg_gameManager.configureCarSize == RG_GarageManager.Switch.On){
				EditorGUILayout.HelpBox("When you reduce this number the values of the affected arrays are deleted. Only reduce this number if you want fewer cars."
				                        , MessageType.Warning);
				EditorGUI.BeginChangeCheck ();
				rg_gameManager.playableVehicles.numberOfCars = EditorGUILayout.IntField("Number Of Cars", rg_gameManager.playableVehicles.numberOfCars);
				if (EditorGUI.EndChangeCheck ()){
					//serializedObject.Update ();
					serializedObject.ApplyModifiedProperties ();
					serializedObject.ApplyModifiedProperties ();
				}
				if(editorCarView >= rg_gameManager.playableVehicles.numberOfCars){
					editorCarView = 0;
				}
				System.Array.Resize (ref rg_gameManager.playableVehicles.vehicles, rg_gameManager.playableVehicles.numberOfCars);
				System.Array.Resize (ref rg_gameManager.playableVehicles.vehicleNames, rg_gameManager.playableVehicles.numberOfCars);
				System.Array.Resize (ref rg_gameManager.playableVehicles.price, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize (ref rg_gameManager.playableVehicles.carMaterial, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize (ref rg_gameManager.playableVehicles.brakeMaterial, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize (ref rg_gameManager.playableVehicles.glassMaterial, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize (ref rg_gameManager.playableVehicles.rimMaterial, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize (ref rg_gameManager.playableVehicles.carGlowLight, rg_gameManager.playableVehicles.numberOfCars);
				System.Array.Resize (ref rg_gameManager.playableVehicles.carUnlocked, rg_gameManager.playableVehicles.numberOfCars);
                System.Array.Resize(ref rg_gameManager.playableVehicles.topSpeedLevel, rg_gameManager.playableVehicles.numberOfCars);
                System.Array.Resize(ref rg_gameManager.playableVehicles.torqueLevel, rg_gameManager.playableVehicles.numberOfCars);
                System.Array.Resize(ref rg_gameManager.playableVehicles.brakeTorqueLevel, rg_gameManager.playableVehicles.numberOfCars);
                System.Array.Resize(ref rg_gameManager.playableVehicles.tireTractionLevel, rg_gameManager.playableVehicles.numberOfCars);
                System.Array.Resize(ref rg_gameManager.playableVehicles.steerSensitivityLevel, rg_gameManager.playableVehicles.numberOfCars);

				//System.Array.Resize(ref rg_gameManager.playerPrefsData.redValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.blueValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.greenValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.redGlowValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.blueGlowValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.greenGlowValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.redGlassValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.blueGlassValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.greenGlassValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.alphaGlassValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.redBrakeValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.blueBrakeValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.greenBrakeValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.redRimValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.blueRimValues, rg_gameManager.playableVehicles.numberOfCars);
				//System.Array.Resize(ref rg_gameManager.playerPrefsData.greenRimValues, rg_gameManager.playableVehicles.numberOfCars);

				EditorUtility.SetDirty (rg_gameManager.playerPrefsData);
            }


			EditorGUILayout.BeginVertical("Box");
			
			EditorGUILayout.BeginHorizontal("Box");
			
			if (GUILayout.Button ("<", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
				if(editorCarView > 0)
					editorCarView -= 1;
			}
			
			GUILayout.Box("Car Number\n" + editorCarView.ToString(), GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) );
			
			if (GUILayout.Button (">", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
				if(editorCarView < (rg_gameManager.playableVehicles.numberOfCars - 1))
					editorCarView += 1;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

            rg_gameManager.playableVehicles.vehicleNames[editorCarView] = EditorGUILayout.TextField("Vehicle Name", rg_gameManager.playableVehicles.vehicleNames[editorCarView]);
			rg_gameManager.playableVehicles.price[editorCarView] = EditorGUILayout.IntField("Vehicle Price", rg_gameManager.playableVehicles.price[editorCarView]);
			rg_gameManager.playableVehicles.carUnlocked[editorCarView] = EditorGUILayout.Toggle("Car Unlocked", rg_gameManager.playableVehicles.carUnlocked[editorCarView]);


	//		EditorGUILayout.LabelField("Level Top Speed               " + rg_gameManager.playableVehicles.topSpeedLevel[editorCarView].ToString());
	//		EditorGUILayout.LabelField("Level Torque                 ", rg_gameManager.playableVehicles.torqueLevel[editorCarView].ToString());
	//		EditorGUILayout.LabelField("Level Brake Torque           ", rg_gameManager.playableVehicles.brakeTorqueLevel[editorCarView].ToString());
	//		EditorGUILayout.LabelField("Level Tire Traction          ", rg_gameManager.playableVehicles.tireTractionLevel[editorCarView].ToString());
	//		EditorGUILayout.LabelField("Level Steer Sensitivit       ", rg_gameManager.playableVehicles.steerSensitivityLevel[editorCarView].ToString());


            rg_gameManager.playableVehicles.vehicles[editorCarView] = (GameObject) EditorGUILayout.ObjectField("Vehicle Prefab", rg_gameManager.playableVehicles.vehicles[editorCarView], typeof (GameObject), false );
			//rg_gameManager.playableVehicles.carMaterial[editorCarView] = (Material) EditorGUILayout.ObjectField("Car Body Material", rg_gameManager.playableVehicles.carMaterial[editorCarView], typeof (Material), false );
			//rg_gameManager.playableVehicles.brakeMaterial[editorCarView] = (Material) EditorGUILayout.ObjectField("Brake Material", rg_gameManager.playableVehicles.brakeMaterial[editorCarView], typeof (Material), false );
			//rg_gameManager.playableVehicles.glassMaterial[editorCarView] = (Material) EditorGUILayout.ObjectField("Glass Material", rg_gameManager.playableVehicles.glassMaterial[editorCarView], typeof (Material), false );
			//rg_gameManager.playableVehicles.rimMaterial[editorCarView] = (Material) EditorGUILayout.ObjectField("Rim Material", rg_gameManager.playableVehicles.rimMaterial[editorCarView], typeof (Material), false );
			//rg_gameManager.playableVehicles.carGlowLight[editorCarView] = (ParticleSystem) EditorGUILayout.ObjectField("Car Glow Light", rg_gameManager.playableVehicles.carGlowLight[editorCarView], typeof (ParticleSystem), false );

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Scene Objects");
			//rg_gameManager.sceneCarGlowLight[editorCarView] = (ParticleSystem) EditorGUILayout.ObjectField("Scene Car Glow Light", rg_gameManager.sceneCarGlowLight[editorCarView], typeof (ParticleSystem), true );
			rg_gameManager.sceneCarModel[editorCarView] = (GameObject) EditorGUILayout.ObjectField("Scene Car Model", rg_gameManager.sceneCarModel[editorCarView], typeof (GameObject), true );

			EditorGUILayout.EndVertical();

			rg_gameManager.playableVehicles.startingCurrency = EditorGUILayout.IntField("Starting Currency", rg_gameManager.playableVehicles.startingCurrency);
			rg_gameManager.playableVehicles.paintPrice = EditorGUILayout.IntField("Body Paint Price", rg_gameManager.playableVehicles.paintPrice);
			rg_gameManager.playableVehicles.brakeColorPrice = EditorGUILayout.IntField("Brake Color Price", rg_gameManager.playableVehicles.brakeColorPrice);
			rg_gameManager.playableVehicles.rimColorPrice = EditorGUILayout.IntField("Rim Color Price", rg_gameManager.playableVehicles.rimColorPrice);
			rg_gameManager.playableVehicles.glassColorPrice = EditorGUILayout.IntField("Glass Tint Price", rg_gameManager.playableVehicles.glassColorPrice);
			rg_gameManager.playableVehicles.glowPrice = EditorGUILayout.IntField("Neon Light Price", rg_gameManager.playableVehicles.glowPrice);
			rg_gameManager.playableVehicles.upgradeSpeedPrice = EditorGUILayout.IntField("Upgrade Speed Price", rg_gameManager.playableVehicles.upgradeSpeedPrice);
			rg_gameManager.playableVehicles.upgradeAccelerationPrice = EditorGUILayout.IntField("Upgrade Acceleration Price", rg_gameManager.playableVehicles.upgradeAccelerationPrice);
			rg_gameManager.playableVehicles.upgradeBrakesPrice = EditorGUILayout.IntField("Upgrade Brakes Price", rg_gameManager.playableVehicles.upgradeBrakesPrice);
			rg_gameManager.playableVehicles.upgradeTiresPrice = EditorGUILayout.IntField("Upgrade Tires Price", rg_gameManager.playableVehicles.upgradeTiresPrice);
			rg_gameManager.playableVehicles.upgradeSteeringPrice = EditorGUILayout.IntField("Upgrade Steering Price", rg_gameManager.playableVehicles.upgradeSteeringPrice);

        }

		if (info) {
			SerializedProperty uI = serializedObject.FindProperty ("uI");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (uI, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
		}

		EditorGUILayout.EndVertical ();
		
		//EditorGUILayout.HelpBox (">", MessageType.Info);
		if (GUILayout.Button ("Delete PlayerPrefs Data")) {
			PlayerPrefs.DeleteAll();
			Debug.Log ("Deleted PlayerPrefs Data");
		}
	}

}