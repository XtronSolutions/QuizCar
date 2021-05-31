using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCC_AutoInput : MonoBehaviour {

	private float accelerationInput;
	public float _AccelerationInput { get { return accelerationInput; } set { accelerationInput = value; } }
	private float steeringInput;
	public float _SteeringInput { get { return steeringInput; } set { steeringInput = value; } }
	// Use this for initialization
	void Start () {
		
	}

	void Update(){
	

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (PlayerManagerScript.instance._RaceManager.CurrentCount < 1) {

			#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
			//if (!isManagedByAI) {
			if (Input.GetMouseButton (0)) {
				accelerationInput = -1;
			} else {
				accelerationInput = 1;//Input.GetAxis ("Acceleration");
			}
			steeringInput = Input.GetAxis ("Steering");
			//} 
			//		else {
			//			//				exhaust.emissionRate = vehicleSpeed;
			//		}


			#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
//			if(!isManagedByAI)
//			{
//			steeringInput = Mathf.Clamp(Input.acceleration.x * steeringSensitivity, -1, 1);
//
//			//set braking on screen touch, otherwise accelerate
//			if (Input.touchCount > 0) accelerationInput = -1;
//			else accelerationInput = 1;
//			}

			#endif

			if (RCC_SceneManager.Instance.activePlayerVehicle.canControl && !RCC_SceneManager.Instance.activePlayerVehicle.externalController) {

				if (accelerationInput > 0) {

					RCC_SceneManager.Instance.activePlayerVehicle.brakeInput = 0f;
					RCC_SceneManager.Instance.activePlayerVehicle.gasInput = accelerationInput;

				} else if (accelerationInput < 0) {

					RCC_SceneManager.Instance.activePlayerVehicle.brakeInput = -accelerationInput;
				}

				RCC_SceneManager.Instance.activePlayerVehicle.steerInput = steeringInput;

			}
		}
	}
}
