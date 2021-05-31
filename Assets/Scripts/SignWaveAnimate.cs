using UnityEngine;
using System.Collections;

public class SignWaveAnimate : MonoBehaviour {
	public float waterLevel;
	public float floatHeight;
	public Vector3 buoyancyCentreOffset;
	public float bounceDamp;
	Vector3 actionPoint;
	float forceFactor;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		actionPoint = transform.position + transform.TransformDirection(buoyancyCentreOffset);
		forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);

		if (forceFactor > 0f) {
			Vector3 uplift = -Physics.gravity * (forceFactor - GetComponent<Rigidbody>().velocity.y * bounceDamp);
			GetComponent<Rigidbody>().AddForceAtPosition(uplift, actionPoint);
		}
	}


	
	
	
}
