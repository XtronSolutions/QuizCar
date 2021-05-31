using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrakeButton : MonoBehaviour {

	private Vector3 defaultPosition;

	// Use this for initialization
	void Start () {
		defaultPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
		if(Input.GetMouseButton(0) && Input.mousePosition.y / Screen.height < 0.65f){
			this.transform.position = Input.mousePosition;
		}else{
			this.transform.position = defaultPosition;
		}
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).position.y / Screen.height < 0.65f){
			this.transform.position = Input.GetTouch(0).position;
		}else{
			this.transform.position = defaultPosition;
		}
		#endif
	}
}
