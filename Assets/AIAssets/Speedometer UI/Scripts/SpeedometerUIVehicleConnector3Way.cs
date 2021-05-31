using UnityEngine;
using System.Collections;
using RacingGameKit;
using RacingGameKit.RGKCar;

[AddComponentMenu("Racing Game Kit/Speedometer UI/Speedometer UI - Vehicle Connector - 3Way"), ExecuteInEditMode]
public class SpeedometerUIVehicleConnector3Way : MonoBehaviour
{
    SpeedometerUI[] SpeedometerIUs;
    RGKCar_Engine oEngine=null;

    SpeedometerUI oSpeedoForSpeed;
    SpeedometerUI oSpeedoForTurbo;
    SpeedometerUI oSpeedoForRPM;
    SpeedometerUI oSpeedoForAll;
    void Start()
    {
        SpeedometerIUs = base.GetComponents<SpeedometerUI>();
        oEngine = base.GetComponent<RGKCar_Engine>() as RGKCar_Engine;

        foreach (SpeedometerUI oSUI in SpeedometerIUs)
        {
            if (oSUI.ControlName == "Speed")
            {
                oSpeedoForSpeed = oSUI;
            }
            else if (oSUI.ControlName == "Turbo" )
            { 
                oSpeedoForTurbo=oSUI;
            }
            else if (oSUI.ControlName == "RPM")
            {
                oSpeedoForRPM = oSUI;
            }
            else if (oSUI.ControlName == "" || oSUI.ControlName == "ALL")
            {
                oSpeedoForAll = oSUI;
            }
        }
    }

    void Update()
    {
        if (oEngine!=null){
            if (oSpeedoForSpeed != null)
            {
                oSpeedoForSpeed.Speed = oEngine.SpeedAsKM;
                string gear = oEngine.Gear.ToString();
                if (gear == "-1") gear = "R";
                if (gear == "0") gear = "N";
                oSpeedoForSpeed.Gear = gear;
            }
            if (oSpeedoForRPM!=null) oSpeedoForRPM.Speed = oEngine.RPM;
            if (oSpeedoForTurbo!=null) oSpeedoForTurbo.Speed = oEngine.TurboFill*100f;

            if (oSpeedoForAll!=null)
            {
                oSpeedoForAll.Speed = oEngine.SpeedAsKM;
                string gear = oEngine.Gear.ToString();
                if (gear == "-1") gear = "R";
                if (gear == "0") gear = "N";
                oSpeedoForAll.Gear = gear;
                oSpeedoForAll.RPM = oEngine.RPM;
            }
        }
    }
    

}

