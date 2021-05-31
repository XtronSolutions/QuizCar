//===========================================================================================
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
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - GamePad Freedrive")]
    public class RGKCar_C2_GamePadFreeDrive : RGKCar_C2_ControllerControllerBase
    {
        public bool SmoothThrottle = false;
        public bool SmoothSteering = false;

        public string ControlsThrottleAxisBinding = "Throttle";
        public string ControlsBrakeAxisBinding = "Brake";
        public string ControlsSteeringAxisBinding = "Horizontal";
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

            smoothThrottle = SmoothThrottle;
            smoothSteering = SmoothSteering;

            if (CarSetup.EngineData.Automatic && CarEngine.CurrentGear > 0)
            {
                if (Input.GetAxisRaw(ControlsThrottleAxisBinding) > 0)
                {
                    ThrottlePressed = true;
                    BrakePressed = false;
                }
                else if (Input.GetAxisRaw(ControlsBrakeAxisBinding) < 0)
                {
                    ThrottlePressed = false;
                    BrakePressed = true;
                }
                else
                {
                    ThrottlePressed = false;
                    BrakePressed = false;
                }
            }
            else if (CarSetup.EngineData.Automatic && CarEngine.CurrentGear == 0)
            {
                if (Input.GetAxisRaw(ControlsThrottleAxisBinding) > 0)
                {
                    ThrottlePressed = false;
                    BrakePressed = true;
                }
                else if (Input.GetAxisRaw(ControlsBrakeAxisBinding) < 0)
                {
                    ThrottlePressed = true;
                    BrakePressed = false;
                }
                else
                {
                    ThrottlePressed = false;
                    BrakePressed = false;
                }
            }

            throttleInput = Input.GetAxisRaw(ControlsThrottleAxisBinding);

            steerInput = 0;
            steerInput = Input.GetAxisRaw(ControlsSteeringAxisBinding);

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