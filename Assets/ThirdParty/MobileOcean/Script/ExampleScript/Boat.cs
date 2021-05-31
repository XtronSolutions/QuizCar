using UnityEngine;
using System.Collections;

public class Boat : MonoBehaviour {
	float orgY = 0;
	public float waveSpeed = 1f;
	public float moveDis = 0.01f;
	public float startOffset;
	// Use this for initialization
	void Start () {
		orgY = transform.position.y;
		StartCoroutine (AnimateBoat (startOffset));
	}
	
	// Update is called once per frame
	IEnumerator AnimateBoat (float startOffset) {
		yield return new WaitForSeconds (startOffset);
		while (true) {
			transform.position = new Vector3 (transform.position.x, orgY + moveDis * Mathf.Sin (Time.time * waveSpeed), transform.position.z);
			transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, 3 * Mathf.Sin (Time.time * (waveSpeed) + 1.5f));
			yield return new WaitForEndOfFrame ();
		}
	}
}
