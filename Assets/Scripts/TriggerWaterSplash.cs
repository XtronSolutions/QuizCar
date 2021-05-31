using UnityEngine;
using System.Collections;

public class TriggerWaterSplash : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<ActivateWaterSplash>()!=null)
        {
            col.gameObject.GetComponent<ActivateWaterSplash>().ActivateSplash();
        }
    }
}