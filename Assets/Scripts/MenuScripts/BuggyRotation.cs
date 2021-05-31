using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuggyRotation : MonoBehaviour {

	public Transform buggyTransform;
	private TKOneFingerRotationRecognizer gestureRecognizer;

	public GameObject tutorial;

	void OnEnable(){
		gestureRecognizer = new TKOneFingerRotationRecognizer ();
//		gestureRecognizer.targetPosition = Camera.main.WorldToScreenPoint( buggyTransform.position );
		gestureRecognizer.gestureRecognizedEvent += OnSwipe;
		TouchKit.addGestureRecognizer (gestureRecognizer);
	}

	void OnDisable(){
		gestureRecognizer.gestureRecognizedEvent -= OnSwipe;
		TouchKit.removeGestureRecognizer (gestureRecognizer);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnSwipe(TKOneFingerRotationRecognizer r){
		tutorial.SetActive (false);
		buggyTransform.Rotate (Vector3.up, gestureRecognizer.deltaRotation * 5);
	}

}
