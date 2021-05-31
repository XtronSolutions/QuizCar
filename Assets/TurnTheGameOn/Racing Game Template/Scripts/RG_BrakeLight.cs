using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RG_BrakeLight
{//NetworkBehaviour
    public Color _colorReverse;
	public Color _colorBrakeOn;
	public Color _colorBrakeOff;
	public UnityStandardAssets.Vehicles.Car.CarController car; // reference to the car controller, must be dragged in inspector

	public bool localPlayer;
	public Material brakeMaterial;
	//public RG_SyncData syncData;

	private string gameMode;

    private void Start(){
		gameMode = PlayerPrefs.GetString ("Game Mode");
		if (gameMode == "MULTIPLAYER") {
			//localPlayer = transform.root.GetComponent<NetworkIdentity> ().isLocalPlayer;
		} else {
			localPlayer = true;
		}
		//brakeMaterial = GetComponent<MeshRenderer>().materials[9];
		brakeMaterial.EnableKeyword ("_EMISSION");
    }

    private void Update(){		
		if (localPlayer) {	
			if (car.BrakeInput > 0f) {
				if (car.reversing) {
					brakeMaterial.SetColor ("_EmissionColor", _colorReverse);
				} else {
					brakeMaterial.SetColor ("_EmissionColor", _colorBrakeOn);
				}
			} else {
				//Invoke("TurnOff", 0.5f);
			}
		} else {
			//if (syncData.verticalInput < 0f) {
			//	if (syncData.gearString == "R") {
			//		brakeMaterial.SetColor ("_EmissionColor", _colorReverse);
			//	} else {
			//		brakeMaterial.SetColor ("_EmissionColor", _colorBrakeOn);
			//	}
			//} else {
			//	//Invoke("TurnOff", 0.5f);
			//}
		}
    }

	void TurnOff(){
		brakeMaterial.SetColor ("_EmissionColor", _colorBrakeOff);
	}

}