using UnityEngine;
using System.Collections;

public class RG_DisableObjectAfter : MonoBehaviour {

	public float waitTime;

	void OnEnable(){
		Invoke ("DisableObject", waitTime);
	}

	void DisableObject(){
		gameObject.SetActive (false);
	}

}
