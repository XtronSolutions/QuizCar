using UnityEngine;
using System.Collections;

public class ToggleHeadlights : MonoBehaviour {
    public LensFlare Sun;
	// Use this for initialization
	void Start () {
	       
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
       

        if (col.gameObject.GetComponent<HeadLightsScript>() != null)
        {
            col.gameObject.GetComponent<HeadLightsScript>().ToggleLights(true);
            //Sun.gameObject.SetActive(!Sun.gameObject.activeSelf);
        }
    }
    void OnTriggerExit(Collider col)
    {

        if (col.gameObject.GetComponent<HeadLightsScript>() != null)
        {
            col.gameObject.GetComponent<HeadLightsScript>().ToggleLights(false);
            //Sun.gameObject.SetActive(!Sun.gameObject.activeSelf);
        }
    }
}