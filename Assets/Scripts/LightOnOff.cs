using UnityEngine;
using System.Collections;

public class LightOnOff : MonoBehaviour {

    public GameObject[] Lights;
	// Use this for initialization
	void Start () {
        TurnLightOff();
        //while (true)
        //{
        //    foreach (GameObject light in Lights)
        //        light.SetActive(!light.activeSelf);

        //    yield return new WaitForSeconds(0.5f);
        //}
    }

    // Update is called once per frame
    public void TurnLightOn()
    {
        foreach (GameObject light in Lights)
            light.SetActive(true);
    }
    public void TurnLightOff()
    {
        foreach (GameObject light in Lights)
            light.SetActive(false);
    }
}