using UnityEngine;
using System.Collections;

/// <summary>
/// Test script for SpeedometerUI. Needs to be attached same object which SpeedometerUI Script attached.
/// </summary>
public class SpeedometerUITest : MonoBehaviour
{
    SpeedometerUI oSpeedometerIU = null;
    float maxSpeed =240;
    float maxRPM = 8000;
    float curSpeed = 0;
    float curRPM = 0;
    bool speedUp = true;
    bool rpmUp = true;
   
    void Start()
    {
        oSpeedometerIU = base.GetComponent<SpeedometerUI>() as SpeedometerUI;
    }

    void Update()
    {

        if (speedUp) { curSpeed += Time.deltaTime * 40; } else { curSpeed -= Time.deltaTime * 40; }
        if (rpmUp) { curRPM += Time.deltaTime * 2000; } else { curRPM -= Time.deltaTime * 2000; }
      
        if (curSpeed >= maxSpeed) { speedUp = false; }
        if (curSpeed <= 0) { speedUp = true; }
        if (curSpeed > 0 && curSpeed < 50) oSpeedometerIU.Gear = "1";
        if (curSpeed > 50 && curSpeed < 100) oSpeedometerIU.Gear = "2";
        if (curSpeed > 100 && curSpeed < 150) oSpeedometerIU.Gear = "3";
        if (curSpeed > 150 && curSpeed < 250) oSpeedometerIU.Gear = "4";
        if (curRPM >= maxRPM) { rpmUp = false ; }
        if (curRPM <= 0) { rpmUp = true; }


        oSpeedometerIU.Speed = curSpeed;
        oSpeedometerIU.RPM = curRPM;
    }
   

}

