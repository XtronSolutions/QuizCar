using UnityEngine;
using System.Collections;

public class GetPlayerCar : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
        while (GameObject.FindObjectOfType<vehicleHandling>()==null) {
            yield return null;
        }
        GetComponent<SmoothFollow>().lookTarget = GameObject.FindObjectOfType<vehicleHandling>().transform;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
