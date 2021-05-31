using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using RacingGameKit;
public class DebugModeUI : MonoBehaviour {

	public Text accelerationText;
	public Text acclerationDropOffText;
	bool changeDistance = false;

	int cameraAngle = 1;
	int maxAngles = 2;

	//public Scrollbar accelDropOffScrollBar;
	Race_Camera oRaceCam ;

	void Start(){
		oRaceCam = GameObject.FindObjectOfType(typeof(Race_Camera)) as Race_Camera;

	}

	void Update(){
		//acclerationDropOffText.text = accelDropOffScrollBar.value.ToString ();
	}

	public void ApplyAcceleration(){
		PlayerManagerScript.instance._vehicleHandling.maximumAcceleration = (float)Convert.ToDouble (accelerationText.text);
	}
	public void ApplyDropOff(){
		PlayerManagerScript.instance._vehicleHandling.accelerationDropOff = (float)Convert.ToDouble (acclerationDropOffText.text);
	}

	public void ChangeCameraDistance(){
		cameraAngle++;
		cameraAngle = cameraAngle > maxAngles ? 1 : cameraAngle;
		if (cameraAngle == 1) {
			oRaceCam.GameCameraSettings.Distance = 4.9f;
			oRaceCam.GameCameraSettings.Height = 1.5f;
		} else if (cameraAngle == 2) {
			oRaceCam.GameCameraSettings.Distance = 10.4f;
			oRaceCam.GameCameraSettings.Height = 7.5f; 
		} 
	}
}