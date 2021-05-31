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
using System.Collections.Generic;

namespace RacingGameKit.RGKCar.CarControllers
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - GamePad Race")]
    public class RGKCar_C2_GamePadRace : RGKCar_C2_ControllerControllerBase
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


        private GameObject waypointContainer;
        private List<Transform> waypoints;

        protected Race_Manager RaceManager;
        protected Racer_Register RaceRegister;

        private bool IsFirstGearShifted = false;


        //Uncomment this method if you want to override here..
        void Start()
        {

            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
            waypointContainer = RaceManager.Waypoints;
            GetWaypoints();

            base.Start();
        }

        void Update()
        {
            smoothThrottle = SmoothThrottle;
            smoothSteering = SmoothSteering;

            if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished)
            {
                CarSetup.EngineData.Automatic = true;

                if (!IsFirstGearShifted)
                {
                    CarEngine.CurrentGear = 2;
                    IsFirstGearShifted = true; 
                }

                if (Input.GetButtonDown(ControlsShiftUpBinding))
                {
                    CarEngine.ShiftUp();
                }
                if (Input.GetButtonDown(ControlsShiftDownBinding))
                {
                    CarEngine.ShiftDown();
                }
            }
            else if (RaceRegister.IsRacerFinished)
            {
                steer = 0;
                throttle = 0f;
                handbrake = 0.5f;
                CarSetup.EngineData.Automatic = true;
                CarEngine.CurrentGear = 1;
            }
            else
            {
                steer = 0;
                CarSetup.EngineData.Automatic = false;
                CarEngine.CurrentGear = 1;
                handbrake = 1f;
            }

            if (CarEngine.CurrentGear > 0)
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
            else if (CarEngine.CurrentGear == 0)
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


        #region STUCKREMOVAL

        /// <summary>
        /// Receives waypoint info from waypoint container
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
            if (RaceRegister.IsRacerStarted && !RaceRegister.IsRacerFinished && !RaceRegister.IsRacerDestroyed && Input.GetButtonDown(ControlsResetBinding))
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

        private void RecoverCar()
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
            Vector3 nearestPoint;
            if (nextWPIndex > 0)
            {
                nearestPoint = NearestPoint(ClosestWP.position, WaypointArray[nextWPIndex].position, transform.position);
            }
            else
            {
                nearestPoint = WaypointArray[0].position;
            }


            //Debug.DrawLine(transform.position, nearestPoint);


            transform.rotation = Quaternion.LookRotation(ClosestWP.forward);
            transform.position = nearestPoint;
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