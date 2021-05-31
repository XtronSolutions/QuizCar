using UnityEngine;
using System.Collections;

public class HeadLightsScript : MonoBehaviour {

    public GameObject[] Lights;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ToggleLights(bool val)
    {
        foreach (GameObject light in Lights)
        {
            light.SetActive(val);
        }
    }
}