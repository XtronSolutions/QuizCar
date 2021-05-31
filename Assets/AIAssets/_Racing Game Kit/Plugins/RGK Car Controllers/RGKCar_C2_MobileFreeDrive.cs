//===========================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// RGKCar Race Controller Script 
// Last Change : 22/12/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;
using RacingGameKit.TouchDrive;
namespace RacingGameKit.RGKCar.CarControllers
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - Mobile Freedrive")]
    public class RGKCar_C2_MobileFreeDrive : RGKCar_C2_ControllerControllerBase
    {
        public GameObject RGKMobileControls;
        public iRGKTDM touchDriveManager;

        public bool EnableAlphaOnPress = true;
        public bool SmoothThrottle = true;
        public bool SmoothSteering = false;
        public bool AutoThrootle = false;
        public bool UseAccSteer = false;
        public bool InvertAxis = false;
        public bool UseXAxis = false;
        public bool UseTouchSteer = true;
        public float SteeringSensitivity = 1;
        public bool UseCurve = false;
        public AnimationCurve SteeringCurve;
        private bool AutoGear = true;
        private bool useProTouchDrive = false;
        private Race_Camera oRaceCam;

        //Uncomment this method if you want to override here..
        void Start()
        {
            RGKMobileControls = GameObject.Find("_TouchDriveProManager");
            if (RGKMobileControls == null)
            {
                RGKMobileControls = GameObject.Find("_RGKTouchDriveManager");
            }
            else { useProTouchDrive = true; }

            if (RGKMobileControls == null)
            {
                Debug.LogWarning("TOUCHDRIVE WARNING : TouchDrive Manager Not Found!");
            }
            touchDriveManager = RGKMobileControls.GetComponent(typeof(iRGKTDM)) as iRGKTDM;

            if (touchDriveManager == null && useProTouchDrive)
            {
                Debug.LogError("TOUCHDRIVE ERROR : TouchDrive Pro Manager found but it doesn't implement iRGKTDM interface. Please check documentation.");
            }

            oRaceCam = GameObject.FindObjectOfType(typeof(Race_Camera)) as Race_Camera;

            base.Start();
            Input.multiTouchEnabled = true;
        }

        void Update()
        {
            if (touchDriveManager == null) return;

            if (touchDriveManager.EnableDrivingOptions)
            {
                AutoThrootle = touchDriveManager.EnableAutoThrottle;
                UseXAxis = touchDriveManager.UseXAxis;
                InvertAxis = touchDriveManager.InvertAxis;
                SteeringSensitivity = touchDriveManager.SteeringSensitivity;
                AutoGear = touchDriveManager.EnableAutoGear;
            }


            bool EnableRaceStuff = true;
            smoothThrottle = SmoothThrottle;
            smoothSteering = SmoothSteering;

            if (!AutoThrootle)
            {
                if (CarEngine.CurrentGear > 0)
                {
                    if (touchDriveManager.TouchItems[1] != null && touchDriveManager.TouchItems[1].IsPressed)
                    {
                        ThrottlePressed = true;
                        BrakePressed = false;

                    }
                    else
                    {
                        ThrottlePressed = false;

                    }

                    if (touchDriveManager.TouchItems[2] != null && touchDriveManager.TouchItems[2].IsPressed)
                    {
                        BrakePressed = true;
                        ThrottlePressed = false;


                    }
                    else
                    {
                        BrakePressed = false;

                    }
                }
                else if (CarEngine.CurrentGear == 0)
                {
                    if (touchDriveManager.TouchItems[1] != null && touchDriveManager.TouchItems[1].IsPressed)
                    {
                        BrakePressed = true;
                        ThrottlePressed = false;

                    }
                    else
                    {
                        BrakePressed = false;

                    }

                    if (touchDriveManager.TouchItems[2] != null && touchDriveManager.TouchItems[2].IsPressed)
                    {
                        ThrottlePressed = true;
                        BrakePressed = false;

                    }
                    else
                    {
                        ThrottlePressed = false;

                    }
                }
            }
            else
            {
                if (CarEngine.CurrentGear > 0)
                {
                    if (!touchDriveManager.TouchItems[2].IsPressed)
                    {
                        ThrottlePressed = true;
                        BrakePressed = false;
                    }
                    else
                    {
                        ThrottlePressed = false;
                        BrakePressed = true;
                    }
                }
                else
                {
                    if (!touchDriveManager.TouchItems[2].IsPressed)
                    {
                        ThrottlePressed = false;
                        BrakePressed = true;
                    }
                    else
                    {
                        ThrottlePressed = true;
                        BrakePressed = false;
                    }
                }
            }


            if (touchDriveManager.TouchItems[0] != null && UseTouchSteer)
            {
                steerInput = touchDriveManager.TouchItems[0].CurrentFloat;
            }
            else if (UseAccSteer)
            {
                if (!UseCurve)
                {
                    if (UseXAxis)
                    {
                        if (!InvertAxis)
                        {
                            steerInput = Mathf.Clamp(Input.acceleration.x * SteeringSensitivity / 2, -1, 1);
                        }
                        else
                        {
                            steerInput = Mathf.Clamp(-Input.acceleration.x * SteeringSensitivity / 2, -1, 1);
                        }
                    }
                    else
                    {
                        if (!InvertAxis)
                        {
                            steerInput = Mathf.Clamp(Input.acceleration.y * SteeringSensitivity / 2, -1, 1);
                        }
                        else
                        {
                            steerInput = Mathf.Clamp(-Input.acceleration.y * SteeringSensitivity / 2, -1, 1);
                        }
                    }
                }
                else
                {
                    if (UseXAxis)
                    {
                        if (!InvertAxis)
                        {
                            if (Input.acceleration.x < 0)
                            {
                                steerInput = -Mathf.Clamp(SteeringCurve.Evaluate(Math.Abs(Input.acceleration.x)), -1, 1);
                            }
                            else
                            { steerInput = Mathf.Clamp(SteeringCurve.Evaluate(Math.Abs(Input.acceleration.x)), -1, 1); }

                        }
                        else
                        {
                            steerInput = Mathf.Clamp(-SteeringCurve.Evaluate(Input.acceleration.x), -1, 1);
                        }
                    }
                    else
                    {
                        if (!InvertAxis)
                        {
                            steerInput = Mathf.Clamp(SteeringCurve.Evaluate(Input.acceleration.y), -1, 1);
                        }
                        else
                        {
                            steerInput = Mathf.Clamp(-SteeringCurve.Evaluate(Input.acceleration.y), -1, 1);
                        }
                    }
                }
            }
            else
            {
                if (touchDriveManager.TouchItems[3] != null && touchDriveManager.TouchItems[3].IsPressed)
                {
                    steerInput = -1;
                }
                else if (touchDriveManager.TouchItems[4] != null && touchDriveManager.TouchItems[4].IsPressed)
                {
                    steerInput = 1;
                }
                else
                {
                    steerInput = 0;
                }
            }

            if (EnableRaceStuff)
            {
                if (touchDriveManager.TouchItems[5] != null && touchDriveManager.TouchItems[5].IsPressed)
                {
                    CarEngine.ShiftUp();

                }
                if (touchDriveManager.TouchItems[6] != null && touchDriveManager.TouchItems[6].IsPressed)
                {
                    CarEngine.ShiftDown();
                }

                if (touchDriveManager.TouchItems[9] != null && touchDriveManager.TouchItems[9].IsPressed)
                {
                    // CheckIsCarStuck();
                }
            }

            if (touchDriveManager.TouchItems[7] != null && touchDriveManager.TouchItems[7].IsPressed)
            {
                oRaceCam.ChangeCamera();
            }

            if (touchDriveManager.TouchItems[8] != null && touchDriveManager.TouchItems[8].IsPressed)
            {
                oRaceCam.ShowBackView();
            }

            base.Update();

        }
    }
}