using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable][ExecuteInEditMode]
public class RG_AIHelper : MonoBehaviour {

	public float forwardHitLength;
	public float sideHitLength;
	public float leftAngleDegree;
	public float rightAngleDegree;
	[HideInInspector]public string avoidTagName;
	public float rotationSpeed;
	[Range(0.0f, 120.0f)]public float resetAfter;
	public float respawnYOffset;

	[Header("Read Only")]
	public int opponentNumber;
	public bool forwardHit;
	public bool rightHit;
	public bool leftHit;
	public bool rightSide;
	public bool leftSide;

	public bool forwardOtherHit;
	public bool rightOtherHit;
	public bool leftOtherHit;
	public bool rightOtherSide;
	public bool leftOtherSide;

	public float timer;
	public float stuckTimer;
	public float stuckReset;
	
	private RG_SceneManager sceneManager;
	private Vector3 tempPosition;
	private UnityStandardAssets.Vehicles.Car.CarController controller;
	private UnityStandardAssets.Vehicles.Car.CarAIControl aiController;
	private UnityStandardAssets.Utility.WaypointProgressTracker progressTracker;
	
	void Start(){
		controller = GetComponent<UnityStandardAssets.Vehicles.Car.CarController> ();
		aiController = GetComponent<UnityStandardAssets.Vehicles.Car.CarAIControl> ();
		progressTracker = GetComponent<UnityStandardAssets.Utility.WaypointProgressTracker> ();
		sceneManager = GameObject.Find ("Race Scene Manager").GetComponent<RG_SceneManager>();
	}	

	void Update () {
		timer += Time.deltaTime * 1;

		if (timer >= resetAfter) {
			timer = 0;
			transform.position = progressTracker.target.position;
			transform.rotation = progressTracker.target.rotation;
		}
		if(stuckTimer >= stuckReset){
			stuckTimer = 0;
			transform.position = progressTracker.target.position;
			transform.rotation = progressTracker.target.rotation;
		}
		//if(controller.speed <= 5){
		RaycastHit hit;
		Vector3 fwd = transform.TransformDirection ( new Vector3(0, 0, 1));
		Vector3 pivotPos = new Vector3(transform.localPosition.x, transform.position.y + 1, transform.localPosition.z);

///Forward Hit	
		if (Physics.Raycast (pivotPos, fwd, out hit, forwardHitLength)) {
			if (hit.collider.tag != "Waypoint") {
				if (hit.collider.tag == avoidTagName) {
//				Debug.DrawLine (pivotPos, hit.point, Color.red);
					forwardHit = true;
				} else {
					forwardOtherHit = true;
				}
			}
		} else {
			Debug.DrawRay (pivotPos, fwd * forwardHitLength, Color.white);
			forwardHit = false;
			forwardOtherHit = false;
		}
///Forward Left Hit
		if (Physics.Raycast (pivotPos, Quaternion.AngleAxis (leftAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, out hit, sideHitLength * 1.5f)) {
			if (hit.collider.tag != "Waypoint") {
				if (hit.collider.tag == avoidTagName) {
					Debug.DrawRay (pivotPos, Quaternion.AngleAxis (leftAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, Color.red);
					leftHit = true;
				} else {
					leftOtherHit = true;
				}
			}
		} else {
			Debug.DrawRay (pivotPos, Quaternion.AngleAxis (leftAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, Color.white);
			leftHit = false;
			leftOtherHit = false;
		}

///Forward Right Hit
		if (Physics.Raycast (pivotPos,  Quaternion.AngleAxis (rightAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, out hit, sideHitLength * 1.5f)) {
			if (hit.collider.tag != "Waypoint") {
				if (hit.collider.tag == avoidTagName) {
					Debug.DrawRay (pivotPos, Quaternion.AngleAxis (rightAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, Color.red);
					rightHit = true;
				} else {
					rightOtherHit = true;
				}
			}
		} else {
			Debug.DrawRay (pivotPos, Quaternion.AngleAxis (rightAngleDegree, transform.up) * fwd * sideHitLength * 1.5f, Color.white);
			rightHit = false;
			rightOtherHit = false;
		}
///Right Side Hit 
		if (Physics.Raycast (pivotPos, Quaternion.AngleAxis(90, transform.up) * fwd * sideHitLength, out hit, sideHitLength)) {
			if (hit.collider.tag != "Waypoint") {
				if (hit.collider.tag == avoidTagName) {
					Debug.DrawLine (pivotPos, hit.point, Color.red);
					rightSide = true;
				} else {
					rightOtherSide = true;
				}
			}
		} else {
			Debug.DrawRay (pivotPos, Quaternion.AngleAxis(90, transform.up) * fwd * sideHitLength, Color.white);
			rightSide = false;
			rightOtherSide = false;
		}
///Left Side Hit
		if (Physics.Raycast (pivotPos, Quaternion.AngleAxis(-90, transform.up) * fwd * sideHitLength, out hit, sideHitLength)) {
			if (hit.collider.tag != "Waypoint") {
				if (hit.collider.tag == avoidTagName) {
					Debug.DrawRay (pivotPos, Quaternion.AngleAxis (-90, transform.up) * fwd * sideHitLength, Color.red);
					leftSide = true;
				} else {
					leftOtherSide = true;
				}
			}
		} else {
			Debug.DrawRay (pivotPos, Quaternion.AngleAxis(-90, transform.up) * fwd * sideHitLength, Color.white);
			leftSide = false;
			leftOtherSide = false;
		}
		
		//if (timer >= 10.0f) {			transform.Rotate (0, Time.deltaTime * (rotationSpeed * 10.0f), 0, Space.World);		}



		if (forwardHit || rightHit || leftHit) {
			
			if (!leftOtherSide && !leftSide && !leftOtherHit && !leftHit && sceneManager.countUp && sceneManager.racerInfo [opponentNumber].nextWP != 0) {
				aiController.m_CautiousSpeedFactor = 1f;
				transform.Rotate (0, Time.deltaTime * (-rotationSpeed), 0, Space.World);
			} else if (!rightOtherSide && !rightSide && !rightHit && !rightOtherHit && sceneManager.countUp && sceneManager.racerInfo [opponentNumber].nextWP != 0) {
				aiController.m_CautiousSpeedFactor = 1f;
				transform.Rotate (0, Time.deltaTime * (rotationSpeed), 0, Space.World);
			} else {
				aiController.m_CautiousSpeedFactor = 0.3f;
			}
		} else if(!forwardHit && !forwardOtherHit){
			aiController.m_CautiousSpeedFactor = 0.5f;
		}else{
			aiController.m_CautiousSpeedFactor = 0.35f;
		}



		if(rightOtherHit && !leftOtherHit && controller.speed < 0.7f){
			//transform.Rotate (0, Time.deltaTime * (-rotationSpeed * 10), 0, Space.World);
		}

		if(leftOtherHit && !rightOtherHit && controller.speed < 0.7f){
			//transform.Rotate (0, Time.deltaTime * rotationSpeed * 10, 0, Space.World);
		}

		if (controller.speed < 1.0f && sceneManager.countUp && sceneManager.racerInfo[opponentNumber].nextWP != 0) {
			if (forwardOtherHit || rightOtherHit || leftOtherHit) {
				if(!leftSide && !leftOtherSide){
					transform.Rotate (0, Time.deltaTime * (-rotationSpeed * 10), 0, Space.World);
				}
				if(!rightSide && !rightOtherSide){
					transform.Rotate (0, Time.deltaTime * (rotationSpeed * 10), 0, Space.World);
				}
				stuckTimer += Time.deltaTime * 1;
			} else {
				stuckTimer = 0;
			}
		}else {
			stuckTimer = 0;
		}
		//if (controller.speed) {
		//}

	}

	public void ResetTimer(){
		timer = 0;
	}
	
}