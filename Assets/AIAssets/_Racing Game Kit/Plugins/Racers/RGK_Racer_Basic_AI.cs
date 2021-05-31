//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// AI Racer Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections.Generic;
using RacingGameKit.Interfaces;
using RacingGameKit.AI;
using RacingGameKit.Helpers;

namespace RacingGameKit.Racers
{
    [AddComponentMenu("Racing Game Kit/Racers/RGK Racer - Basic AI")]
    public class RGK_Racer_Basic_AI : AI_Basic
    {
        private Race_Manager RaceManager;
        private Racer_Register RaceRegister;
        private IRGKCarController RGKCarController;

        private GameObject waypointContainer;
        private List<Transform> waypoints;

        private int currentWaypoint = 0;

        public bool UseSteerSmoothing = true;
        
        public float SteerSmoothingLow = 10f;
        public float SteerSmoothingHigh = 15f;
        public bool UseLerp = true;
        public float SteerSmoothingReleaseCoef = 0.25f;

        public float StuckResetTimer = 0f;
        public float StuckResetWait = 2f;

        public bool UseThrottleBonus = true;
        public bool RandomizeBonusUsage = true;
        public float ThrottleBonus = 1f;

        public bool IsBraking = false;

        private float CarSpeedKm = 0;
        private float SoftBrakeSpeed = 0;
        private float HardBrakeSpeed = 0;


        public float RGKSteer = 0.0f;
        public float RGKThrottle = 0.0f;
        private float RGKHandbrake = 0f;

        public Vector3 ProbableWaypointPosition;
        public Vector3 RelativeWaypointPosition;
        public Vector3 ProbableWaypointPositionNew;
        public Vector3 RelativeWaypointPositionNew;
        public float RGKSteerFirst = 0;
        private float RGKSteerCorrectionFactor = 0;
        private eAIRoadPosition ReverseTimeRoadPosition;
        public bool IsReversing = false;
        public bool isSteeringLocked = false;
        public float nextWPCoef = 10;


        void Start()
        {
            Initialize_AI();

            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");

            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            waypointContainer = RaceManager.Waypoints;

            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
		//	Debug.Log ("race reg========" + RaceRegister);

            RGKCarController = (IRGKCarController)transform.GetComponent(typeof(IRGKCarController));

            GetWaypoints();
        }


        void FixedUpdate()
        {
            try
            {
                CarSpeedKm = Mathf.Round(GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
                Speed = CarSpeedKm;

                if (RaceManager == null || RaceRegister == null)
                {
                    Debug.LogWarning(RGKMessages.RaceManagerMissing);
                    return;
                }


                RGKCarController.IsReversing = AIReversing;
                RGKCarController.IsBraking = IsBraking;
                CheckIsCarStuck();
                ExecuteRadar();
                CalculateRoute();
            }
            catch (Exception _ex)
            {
                Debug.Log(_ex.Message + " " + _ex.StackTrace);
            }
        }

        void Update()
        {
            if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished)
            {
                RGKCarController.ShiftGears();
            }

            if (RGKThrottle >= 0)
            {
                RGKCarController.ApplyDrive(RGKThrottle, 0f, System.Convert.ToBoolean(RGKHandbrake));
            }
            else
            {
                RGKCarController.ApplyDrive(0, Math.Abs(RGKThrottle), System.Convert.ToBoolean(RGKHandbrake));
            }

            RGKCarController.ApplySteer(RGKSteer);

        }
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

        public float RGKSteerLast;
        /// <summary>
        /// Determinates which waypoint is next 
        /// </summary>
        private void CalculateRoute()
        {
            if (AIWillAvoid) WillReCalculate = true;
            WayPointItem oWPItem;

            if (WillReCalculate)
            {
                oWPItem = waypoints[currentWaypoint].GetComponent<WayPointItem>() as WayPointItem;
                SoftBrakeSpeed = oWPItem.SoftBrakeSpeed;
                HardBrakeSpeed = oWPItem.HardBrakeSpeed;
                float LeftWideOfWP = oWPItem.LeftWide;
                float RightWideOfWP = oWPItem.RightWide;

                IsBraking = false;

                ProbableWaypointPosition = new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z);
                RelativeWaypointPosition = transform.InverseTransformPoint(ProbableWaypointPosition);


                ////Calculate Escape Points

                //NextWPDis = Vector3.Distance(transform.position, ProbableWaypointPosition);
                //EscapeDistance = Mathf.Sin(RelativeWaypointPosition.x * Mathf.Deg2Rad) * NextWPDis;

                //EscapeDistance = ThinkNewEscapePoint(LeftWideOfWP, RightWideOfWP, currentWaypoint, EscapeDistance, RelativeWaypointPosition.x, AIOldRoadPosition);

                WillReCalculate = false;
            }

            ProbableWaypointPositionNew = new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z + UnityEngine.Random.Range(0.3f, 2));
            RelativeWaypointPositionNew = transform.InverseTransformPoint(ProbableWaypointPositionNew);


            if (UseSteerSmoothing && UseLerp)
            {
                if (Mathf.Abs(RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) < SteerSmoothingReleaseCoef)
                {
                    RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude); 
                }
                else
                {
                    float RGKSteerLast = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);
                    if ((RGKSteerLast < 0 && RGKSteerFirst > 0) || (RGKSteerFirst < 0 && RGKSteerLast > 0))
                    {
                        RGKSteerFirst = 0;
                    }
                    RGKSteerFirst = Mathf.Lerp(RGKSteerFirst, RGKSteerLast, Time.deltaTime * UnityEngine.Random.Range(SteerSmoothingLow / 10, SteerSmoothingHigh / 10));
                }
            }
            else if (UseSteerSmoothing && !UseLerp)
            {
                if (Mathf.Abs(RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) < SteerSmoothingReleaseCoef)
                {
                    RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(SteerSmoothingLow, SteerSmoothingHigh);
                }
                else
                { RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude); }
            }
            else
            {
                RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);
            }

            if (RelativeWaypointPositionNew.z < 0 && !ReverseLockedbyObstacle)
            {
                AIReversing = true;
                IsReversing = AIReversing;
            }

            if (AIReversing)
            {
                if (!isSteeringLocked)
                {
                    if (RGKSteer > 0)
                    { RGKSteerCorrectionFactor = 1; }
                    else
                    { RGKSteerCorrectionFactor = -1; }

                    if (ReverseTimeRoadPosition == eAIRoadPosition.Right) RGKSteerCorrectionFactor *= -1;

                    isSteeringLocked = true;
                }

                RGKSteer = RGKSteerCorrectionFactor;

                if (RelativeWaypointPositionNew.z > 25)
                {
                    RGKThrottle = 0;
                    isSteeringLocked = false;
                    AIReversing = false;
                    IsReversing = AIReversing;
                }

            }
            else
            {
                ReverseTimeRoadPosition = AICurrentRoadPosition;
                RGKSteer = (RGKSteerFirst + SteerFactor);
                AIReversing = false;
            }

            if (RelativeWaypointPositionNew.z > 25) ReverseLockedbyObstacle = false; //recover from reverselock

            if (Mathf.Abs(RGKSteer) < 1.5f)
            {
                if (!AIReversing)
                {
                    if (UseThrottleBonus)
                    {

                        float thrBonus = ThrottleBonus;
                        if (RandomizeBonusUsage) thrBonus = UnityEngine.Random.Range(1f, ThrottleBonus);

                        RGKThrottle = (RelativeWaypointPositionNew.z / RelativeWaypointPositionNew.magnitude - Mathf.Abs(RGKSteer) / 2) * thrBonus;
                    }
                    else
                    {
                        RGKThrottle = (RelativeWaypointPositionNew.z / RelativeWaypointPositionNew.magnitude - Mathf.Abs(RGKSteer) / 2);
                    }
                }
                else
                {
                    RGKThrottle = -1;
                }

                if (SoftBrakeSpeed > 0)
                {
                    if (CarSpeedKm > SoftBrakeSpeed && CarSpeedKm < HardBrakeSpeed)
                    {
                        RGKThrottle = AISoftBrakeFactor;
                        IsBraking = true;
                    }
                    else if (CarSpeedKm > HardBrakeSpeed)
                    {
                        RGKThrottle = AIHardBrakeFactor;
                        IsBraking = true;
                    }
                }

            }
            else
            {
                RGKThrottle = 0.0f;
            }

            if (ReverseLockedbyObstacle)
            {
                if (RGKSteer > 0)
                { RGKSteer = 1; }
                else
                { RGKSteer = -1; }

                isSteeringLocked = true;
                RGKThrottle = 1;
            }

            float innerNextWPCoef = UnityEngine.Random.Range(nextWPCoef, nextWPCoef + 3);
            if (RelativeWaypointPositionNew.magnitude <= innerNextWPCoef)
            {
                currentWaypoint++;

                if (currentWaypoint >= waypoints.Count)
                {
                    currentWaypoint = 0;
                }
                WillReCalculate = true;
            }

            if (ShowNavDebugLines)
            {
               // Debug.DrawLine(base.transform.position, ProbableWaypointPosition, Color.cyan);
                //Debug.DrawLine(base.transform.position, ProbableWaypointPositionNew, Color.yellow);
            }
        }
        #region STUCKREMOVAL

        protected void CheckIsCarStuck()
        {
            if (RaceRegister.IsRacerStarted && !RaceRegister.IsRacerFinished)
            {
                if (!RaceRegister.IsRacerDestroyed)
                {
                    if (CarSpeedKm < 1)
                        StuckResetTimer += Time.deltaTime;
                    else
                        StuckResetTimer = 0;

                    if (StuckResetTimer > StuckResetWait)
                        RecoverCar();
                }
            }

            if (layerChangeStarted)
            {
                layerChangeForIgnoreProcessor();
            }
        }

        private void RecoverCar()
        {
            Transform targetWp;
			Debug.Log ("recover car");
            if (currentWaypoint > 0)
            {
                targetWp = waypoints[currentWaypoint - 1].transform;
            }
            else
            {
                targetWp = waypoints[0].transform;
            }


            RGKThrottle = 0;
            RGKSteer = 0;
            AIReversing = false;
            RGKCarController.Gear = 2;
            RGKCarController.ApplyDrive(0, 0, false);

            transform.rotation = Quaternion.LookRotation(targetWp.forward);

            transform.position = targetWp.position;
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

        private void RecoverCar(Transform TargetPosition)
        {
            //Debug.DrawLine(transform.position, TargetPosition.position);
			Debug.Log ("Collide at : "+transform.position+"\n target position :"+TargetPosition.position);
            RGKThrottle = 0;
            RGKSteer = 0;
            IsReversing = false;

            RGKCarController.Gear = 1;
            RGKCarController.ApplyDrive(0, 0, true);
            RGKCarController.IsPreviouslyReversed = true;

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

        void ChangeLayersRecursively(Transform trans, String LayerName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerName);
                ChangeLayersRecursively(trans.GetChild(i), LayerName);
            }

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