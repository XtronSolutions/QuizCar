using UnityEngine;
using System.Collections;
using RacingGameKit.Interfaces ;
public class RGKCarUISpeedPush : MonoBehaviour {

    private RaceUIMobileSample RGKActiveUI;
	void Start () {

        GameObject RaceManagerObject = GameObject.Find("_RaceManager");
        if (RaceManagerObject != null)
        {
            RGKActiveUI = RaceManagerObject.GetComponent(typeof(RaceUIMobileSample)) as RaceUIMobileSample;
 
        }
	}
	
	
	void Update () {
        if (RGKActiveUI != null)
        {
            RGKActiveUI.PlayerSpeed = (GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
        }
	}
}