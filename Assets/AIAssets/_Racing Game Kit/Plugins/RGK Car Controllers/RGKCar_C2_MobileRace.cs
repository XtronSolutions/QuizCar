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
using RacingGameKit;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;
using RacingGameKit.TouchDrive;
using System.Collections.Generic;

namespace RacingGameKit.RGKCar.CarControllers
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - Mobile Race")]
    public class RGKCar_C2_MobileRace : RGKCar_C2_ControllerControllerBase
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



        private GameObject waypointContainer;
        private List<Transform> waypoints;
        protected Race_Manager RaceManager;
        protected Racer_Register RaceRegister;
        private bool IsFirstGearShifted = false;


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

            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            if (RaceManager != null)
            {
                RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
                waypointContainer = RaceManager.Waypoints;
                GetWaypoints();

                oRaceCam = GameObject.FindObjectOfType(typeof(Race_Camera)) as Race_Camera;

                base.Start();
            }
            else
            {
            }

            Input.multiTouchEnabled = true;
        }

        void Update()
        {
            if (RaceManager == null) return;
            if (touchDriveManager == null) return;
            if (touchDriveManager.EnableDrivingOptions)
            {
                AutoThrootle = touchDriveManager.EnableAutoThrottle;
                UseXAxis = touchDriveManager.UseXAxis;
                InvertAxis = touchDriveManager.InvertAxis;
                SteeringSensitivity = touchDriveManager.SteeringSensitivity;
                AutoGear = touchDriveManager.EnableAutoGear;
                UseTouchSteer = touchDriveManager.EnableTouchWheelSteer;
                UseAccSteer = touchDriveManager.EnableTiltSteer;
            }


            bool EnableRaceStuff = true;
            smoothThrottle = SmoothThrottle;
            smoothSteering = SmoothSteering;


            if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished)
            {
                CarSetup.EngineData.Automatic = AutoGear;

                if (!IsFirstGearShifted)
                {
                    handbrake = 0;
                    CarEngine.CurrentGear = 2;
                    IsFirstGearShifted = true;
                }

                EnableRaceStuff = true;

            }
            else if (RaceRegister.IsRacerFinished)
            {
                steer = 0;
                throttle = 0f;
                handbrake = 0.5f;
                CarSetup.EngineData.Automatic = AutoGear;
                CarEngine.CurrentGear = 1;
                EnableRaceStuff = false;
            }
            else
            {
                steer = 0;
                CarSetup.EngineData.Automatic = false;
                CarEngine.CurrentGear = 1;
                handbrake = 1f;
                EnableRaceStuff = false;
            }




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
                    CheckIsCarStuck();
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

        #region STUCKREMOVAL

        /// <summary>
        /// Receives waypint info from waypoint container
        /// </summary>
        private void GetWaypoints()
        {
            Transform[] potentialWaypoints = waypointContainer.GetComponentsInChildren<Transform>();
            waypoints = new List<Transform> { };

            foreach (Transform potentialWaypoint in potentialWaypoints)
            {
                if (potentialWaypoint != waypointContainer.transform)
                {
                    waypoints.Add(potentialWaypoint);
                }
            }

        }
        protected void CheckIsCarStuck()
        {
            if (RaceRegister.IsRacerStarted && !RaceRegister.IsRacerFinished && !RaceRegister.IsRacerDestroyed)
            {
                RecoverCar();
                Debug.Log("Resetting Player Car");
            }

            if (layerChangeStarted)
            {
                layerChangeForIgnoreProcessor();
            }
        }
        public float StuckResetTimer = 0f;

        public void RecoverCar()
        {
			Debug.Log ("recover car");
			Transform[] WaypointArray = waypoints.ToArray();
            Transform ClosestWP = GetClosestWP(WaypointArray, transform.position);
            Vector3 RelativeWPPosition = transform.InverseTransformPoint(ClosestWP.transform.position);

            int nextWPIndex = 0;

            if (RelativeWPPosition.z > 0) // close to forward
            {
                nextWPIndex = Array.FindIndex(WaypointArray, tr => tr.name == ClosestWP.name) - 1;
            }
            else // close to back
            {
                nextWPIndex = Array.FindIndex(WaypointArray, tr => tr.name == ClosestWP.name) + 1;
            }

            transform.rotation = Quaternion.LookRotation(ClosestWP.forward);
            transform.position = ClosestWP.position;
            transform.position += Vector3.up * 0.1f;
            transform.position += Vector3.right * 0.1f;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            StuckResetTimer = 0;

            layerChangeTimer = layerChangeTimerValue;
            currentLayerCache = LayerMask.LayerToName(gameObject.layer);
            ChangeLayersRecursively(gameObject.transform, "IGNORE");

            layerChangeStarted = true;

        }

        public void RecoverCar(Transform TargetPosition)
        {
			Debug.Log ("recover car");
			transform.rotation = Quaternion.LookRotation(TargetPosition.forward);
            transform.position = TargetPosition.position;
            transform.position += Vector3.up * 0.1f;
            transform.position += Vector3.right * 0.1f;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            StuckResetTimer = 0;

            layerChangeTimer = layerChangeTimerValue;
            currentLayerCache = LayerMask.LayerToName(gameObject.layer);
            ChangeLayersRecursively(gameObject.transform, "IGNORE");

            layerChangeStarted = true;

        }

        //Ignore Collision stuff
        string currentLayerCache;
        float layerChangeTimerValue = 7;
        float layerChangeTimer = 0;
        bool layerChangeStarted = false;

        private void layerChangeForIgnoreProcessor()
        {
            layerChangeTimer -= Time.deltaTime;

            if (layerChangeTimer <= 0)
            {
                ChangeLayersRecursively(gameObject.transform, currentLayerCache);
                layerChangeTimer = layerChangeTimerValue;
            }
        }

        private void ChangeLayersRecursively(Transform trans, String LayerName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerName);
                ChangeLayersRecursively(trans.GetChild(i), LayerName);
            }

        }

        private Transform GetClosestWP(Transform[] DPs, Vector3 myPosition)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            foreach (Transform DP in DPs)
            {
                float dist = Vector3.Distance(DP.position, myPosition);
                if (dist < minDist)
                {
                    tMin = DP;
                    minDist = dist;
                }
            }
            return tMin;
        }


        public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 lineDirection = Vector3.Normalize(lineEnd - lineStart);
            float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
            return lineStart + (closestPoint * lineDirection);
        }
        public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
        {
            Vector3 fullDirection = lineEnd - lineStart;
            Vector3 lineDirection = Vector3.Normalize(fullDirection);
            float closestPoint = Vector3.Dot((point - lineStart), lineDirection) / Vector3.Dot(lineDirection, lineDirection);
            return lineStart + (Mathf.Clamp(closestPoint, 0.0f, Vector3.Magnitude(fullDirection)) * lineDirection);
        }

        #endregion


    }
}