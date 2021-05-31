using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeArrowAnimation : MonoBehaviour {

	public GameObject firstArrow;
	public GameObject secondArrow;
	public GameObject thirdArrow;

	public Vector3 startPosition;
	public Vector3 firstPosition;
	public Vector3 secondPosition;
	public Vector3 thirdPosition;

	// Use this for initialization
	void Start () {
		AnimateOne ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AnimateOne(){
		secondArrow.SetActive (false);
		thirdArrow.SetActive (false);
		firstArrow.transform.localPosition = startPosition;
		iTween.MoveTo (firstArrow, iTween.Hash ("position", firstPosition, "time", 0.2f, "islocal", true,
			"oncomplete", "AnimateTwo", "oncompletetarget", this.gameObject));
	}

	void AnimateTwo(){
		secondArrow.SetActive (true);
		iTween.MoveTo (firstArrow, iTween.Hash ("position", secondPosition, "time", 0.2f, "islocal", true,
			"oncomplete", "AnimateThree", "oncompletetarget", this.gameObject));
	}

	void AnimateThree(){
		thirdArrow.SetActive (true);
		iTween.MoveTo (firstArrow, iTween.Hash ("position", thirdPosition, "time", 1.6f, "islocal", true,
			"oncomplete", "AnimateOne", "oncompletetarget", this.gameObject));
	}
}
