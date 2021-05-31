//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar.CarControllers
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - Free Drive")]
    public class RGKCar_C2_FreeDrive : RGKCar_C2_ControllerControllerBase
    {

        public string ControlsThrottleBinding = "Throttle";
        public string ControlsBrakeBinding = "Brake";
        public string ControlsSteeringBinding = "Horizontal";
        public string ControlsHandbrakeBinding = "Handbrake";
        public string ControlsShiftUpBinding = "ShiftUp";
        public string ControlsShiftDownBinding = "ShiftDown";
        public string ControlsHeadlightsBinding = "Lights";
        public string ControlsResetBinding = "ResetVehicle";

        //Uncomment this method if you want to override here..
        void Start()
        {
            base.Start();
        }

        void Update()
        {

            ThrottlePressed = (Input.GetAxisRaw(ControlsThrottleBinding) > 0) ? true : false;
            BrakePressed = (Input.GetAxisRaw(ControlsBrakeBinding) > 0) ? true : false;
            
            if (CarSetup.EngineData.Automatic && CarEngine.CurrentGear == 0)
            {
                ThrottlePressed = (Input.GetAxisRaw(ControlsBrakeBinding) > 0) ? true : false;
                BrakePressed = (Input.GetAxisRaw(ControlsThrottleBinding) > 0) ? true : false;
            }

            throttleInput = Input.GetAxisRaw(ControlsThrottleBinding);

            steerInput = 0;
            steerInput = Input.GetAxisRaw(ControlsSteeringBinding);

            if (Input.GetButtonDown(ControlsShiftUpBinding))
            {
              
                CarEngine.ShiftUp();
            }
            if (Input.GetButtonDown(ControlsShiftDownBinding))
            {
            
                CarEngine.ShiftDown();
            }

            // Handbrake
            if (Input.GetAxisRaw(ControlsHandbrakeBinding) > 0)
            {
                handbrake = 1;
            }
            else
            {
                handbrake = 0;
            }

            if (Input.GetButtonDown(ControlsHeadlightsBinding))
            {
                if (isHeadlightsOn)
                {
                    isHeadlightsOn = false;
                }
                else
                {
                    isHeadlightsOn = true;
                }
            }
            base.Update();

        }

        

    }
}