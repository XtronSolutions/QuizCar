using UnityEngine;
using System.Collections;

public class NitroShacking : MonoBehaviour {
	Vector3 scale;
	// Use this for initialization
	void Start () {
		scale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.localRotation = Quaternion.Euler(new Vector3(2*(Random.value-0.5f),0,0));
		transform.localScale = (1+Mathf.Sin(Time.time*60)/10)*scale;
	}
}
