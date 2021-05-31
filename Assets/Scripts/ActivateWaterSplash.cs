using UnityEngine;
using System.Collections;

public class ActivateWaterSplash : MonoBehaviour {
    public GameObject Splash;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ActivateSplash()
    {
        Splash.SetActive(true);
        StartCoroutine(DeactivateSplash());
    }
    IEnumerator DeactivateSplash()
    {
        yield return new WaitForSeconds(2f);
        Splash.SetActive(false);
    }
}