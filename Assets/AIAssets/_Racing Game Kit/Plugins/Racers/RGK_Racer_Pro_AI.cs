//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// PRO AI Racer Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Interfaces;
using RacingGameKit.AI;

namespace RacingGameKit.Racers
{
    [AddComponentMenu("Racing Game Kit/Racers/RGK Racer - Pro AI")]
    public class RGK_Racer_Pro_AI : AI_Brain
    {
        [HideInInspector]
        private GameObject waypointContainer;
        [HideInInspector]
        private List<Transform> waypoints;
        [HideInInspector]
        private int currentWaypoint = 0;
        [HideInInspector]
        public float RGKSteer = 0.0f;
        [HideInInspector]
        public float RGKThrottle = 0.0f;
        [HideInInspector]
        public float RGKHandbrake = 0f;
        [HideInInspector]
        public bool UseSteerSmoothing = true;
        [HideInInspector]
        public eSteerSmoothingMode SteerSmoothingMode = eSteerSmoothingMode.Basic;
        [HideInInspector]
        public SmootingData FPSBasedSmoothingdata;
        [HideInInspector]
        public float SteerSmoothingLow = 10f;
        [HideInInspector]
        public float SteerSmoothingHigh = 20f;
        public bool UseLerp = false;
        public float SteerSmoothingReleaseCoef = 0.25f;
        //[HideInInspector]
        public float CarSpeedKm = 0;
        [HideInInspector]
        private float SoftBrakeSpeed = 0;
        [HideInInspector]
        private float HardBrakeSpeed = 0;
        [HideInInspector]
        public bool IsBraking = false;


        [HideInInspector]
        private Race_Manager RaceManager;
        [HideInInspector]
        private Racer_Register RaceRegister;
        [HideInInspector]
        public float StuckResetTimer = 0f;
        [HideInInspector]
        public float StuckResetWait = 1f;

        [HideInInspector]
        protected Vector3 ProbableWaypointPosition;
        [HideInInspector]
        private Vector3 RelativeWaypointPosition;
        [HideInInspector]
        private Vector3 ProbableWaypointPositionNew;
        [HideInInspector]
        private Vector3 RelativeWaypointPositionNew;

        [HideInInspector]
        private float NextWPDis;
        [HideInInspector]
        private float EscapeDistance;
        [HideInInspector]
        private float RGKSteerFirst = 0;
        [HideInInspector]
        private float RGKSteerCorrectionFactor = 0;
        [HideInInspector]
        public bool IsReversing = false;
        [HideInInspector]
        private bool isSteeringLocked = false;
        [HideInInspector]
        private IRGKCarController RGKCarController;
        [HideInInspector]
        private eAIRoadPosition ReverseTimeRoadPosition;
        [HideInInspector]
        public float nextWPCoef = 10;
		Rigidbody myRigid ;

        void Start()
        {
            Initialize_AI();

            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
			if (RaceManager == null) {}//Debug.Log("Racemanager not found!");
            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
			if (RaceRegister == null) {}//Debug.Log("Race Register not found!");
            RGKCarController = (IRGKCarController)transform.GetComponent(typeof(IRGKCarController));
			if (RGKCarController == null){} //Debug.Log("Car Controller not found!");

            waypointContainer = RaceManager.Waypoints;
            GetWaypoints();
			myRigid = GetComponent<Rigidbody> ();
        }



        void FixedUpdate()
        {
            try
            {
				CarSpeedKm = Mathf.Round(myRigid.velocity.magnitude * 3.6f);
                Speed = CarSpeedKm;

//                if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished)
//                {
//
//                    RGKCarController.ShiftGears();
//                }

                if (RGKThrottle >= 0)
                {
                    RGKCarController.ApplyDrive(RGKThrottle, 0f, System.Convert.ToBoolean(RGKHandbrake));
                }
                else
                {
                    RGKCarController.ApplyDrive(0, Math.Abs(RGKThrottle), System.Convert.ToBoolean(RGKHandbrake));
                }


                RGKCarController.ApplySteer(RGKSteer);
                RGKCarController.IsReversing = AIReversing;
                RGKCarController.IsBraking = IsBraking;
//                CheckIsCarStuck();   // this checks speed of car then respawn it 
//                ExecuteRadar();
				if (!RaceRegister.IsRacerFinished && RaceRegister.IsRacerStarted)
				{
					if (!RaceRegister.IsRacerDestroyed)
					{  
						//Debug.Log("CarSpeedKm:"+CarSpeedKm);
						if (CarSpeedKm < 4f) 
						StartCoroutine(RecoverMyCar());
					}

				}
                CalculateRoute();
            }
            catch (Exception _ex)
            {
//                Debug.Log(_ex.Message + " " + _ex.StackTrace);
            }
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


        /// <summary>
        /// Determinates which waypoint is next 
        /// </summary>
        private void CalculateRoute()
        {
            //if (AIWillAvoid) WillReCalculate = true;
            WayPointItem oWPItem;
            IsBraking = false;
            if (WillReCalculate)
            {
                oWPItem = waypoints[currentWaypoint].GetComponent<WayPointItem>() as WayPointItem;
                SoftBrakeSpeed = oWPItem.SoftBrakeSpeed;
                HardBrakeSpeed = oWPItem.HardBrakeSpeed;
                float LeftWideOfWP = oWPItem.LeftWide;
                float RightWideOfWP = oWPItem.RightWide;



                ProbableWaypointPosition = new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z);
                RelativeWaypointPosition = transform.InverseTransformPoint(ProbableWaypointPosition);


                //Calculate Escape Points

                NextWPDis = Vector3.Distance(transform.position, ProbableWaypointPosition);
                EscapeDistance = Mathf.Sin(RelativeWaypointPosition.x * Mathf.Deg2Rad) * NextWPDis;

                EscapeDistance = ThinkNewEscapePoint(LeftWideOfWP, RightWideOfWP, currentWaypoint, EscapeDistance, RelativeWaypointPosition.x, AIOldRoadPosition);

                WillReCalculate = false;
            }

            ProbableWaypointPositionNew = new Vector3(waypoints[currentWaypoint].position.x, transform.position.y, waypoints[currentWaypoint].position.z + EscapeDistance);
            RelativeWaypointPositionNew = transform.InverseTransformPoint(ProbableWaypointPositionNew);

            if (!AIWillEscape)
            {
                if (UseSteerSmoothing && UseLerp)
                {

                    if (Mathf.Abs(RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) < SteerSmoothingReleaseCoef)
                    {
                        RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);
                    }
                    else
                    {

                        if (SteerSmoothingMode == eSteerSmoothingMode.Basic)
                        {

                            float RGKSteerLast = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);// *Time.deltaTime * UnityEngine.Random.Range(SteerSmoothingLow, SteerSmoothingHigh); //*2
                            if ((RGKSteerLast < 0 && RGKSteerFirst > 0) || (RGKSteerFirst < 0 && RGKSteerLast > 0))
                            {
                                RGKSteerFirst = 0;
                            }
                            RGKSteerFirst = Mathf.Lerp(RGKSteerFirst, RGKSteerLast, Time.deltaTime * UnityEngine.Random.Range(SteerSmoothingLow / 10, SteerSmoothingHigh / 10));
                        }
                        else
                        {
                            float currentFPS = RaceManager.WorkingFPS;
                            if (currentFPS <= 30)
                            {
                                RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(FPSBasedSmoothingdata.m_0_30FPS.Min, FPSBasedSmoothingdata.m_0_30FPS.Max);
                            }
                            else if (currentFPS > 30 && currentFPS <= 45)
                            {
                                RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(FPSBasedSmoothingdata.m_30_45FPS.Min, FPSBasedSmoothingdata.m_30_45FPS.Max);
                            }
                            else if (currentFPS > 45 && currentFPS <= 60)
                            {
                                RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(FPSBasedSmoothingdata.m_45_60FPS.Min, FPSBasedSmoothingdata.m_45_60FPS.Max);
                            }
                            else if (currentFPS > 60)
                            {
                                RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(FPSBasedSmoothingdata.m_60_NFPS.Min, FPSBasedSmoothingdata.m_60_NFPS.Max);
                            }
                        }
                    }
                }
                else if (UseSteerSmoothing && !UseLerp)
                {
                    if (Mathf.Abs(RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) < SteerSmoothingReleaseCoef)
                    {
                        RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude) * Time.deltaTime * UnityEngine.Random.Range(SteerSmoothingLow, SteerSmoothingHigh);
                    }
                    else
                    {
                        RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);
                    }
                }
                else
                {
                    RGKSteerFirst = (RelativeWaypointPositionNew.x / RelativeWaypointPositionNew.magnitude);
                }
            }
            else
            {  //Mor sharp turns when tries escape
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
                    RGKThrottle = RelativeWaypointPositionNew.z / RelativeWaypointPositionNew.magnitude - Mathf.Abs(RGKSteer) / 2 * AISpeedFactor;
                }
                else
                {
                    RGKThrottle = -1;
                }

                if (SoftBrakeSpeed > 0)
                {

                    if (CarSpeedKm > SoftBrakeSpeed && CarSpeedKm < HardBrakeSpeed)
                    {
                        RGKThrottle *= AISoftBrakeFactor;
                        IsBraking = true;

                    }
                    else if (CarSpeedKm > HardBrakeSpeed)
                    {
                        RGKThrottle *= AIHardBrakeFactor;
                        IsBraking = true;

                    }
                }

                if (AISoftBraking)
                {
                    RGKThrottle *= AISoftBrakeFactor;
                    IsBraking = true;
                }
                else if (AIHardBraking)
                {
                    RGKThrottle *= AIHardBrakeFactor;
                    IsBraking = true;
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
            if (RelativeWaypointPositionNew.magnitude < innerNextWPCoef)
            {
                currentWaypoint++;

                if (currentWaypoint >= waypoints.Count)
                {
                    currentWaypoint = 0;
                }
                WillReCalculate = true;
            }

//            if (ShowNavDebugLines)
//            {
//				Debug.DrawLine(base.transform.position, ProbableWaypointPosition, Color.green);
//                Debug.DrawLine(base.transform.position, ProbableWaypointPositionNew, Color.yellow);
//            }
        }

        #region STUCKREMOVAL

        protected void CheckIsCarStuck()
        {  
			Debug.Log ("car speed  "+RaceRegister.IsRacerStarted+"set timer **  "+RaceRegister.IsRacerFinished);
            if (RaceRegister.IsRacerStarted && !RaceRegister.IsRacerFinished)
            {
                if (!RaceRegister.IsRacerDestroyed)
                {
					
					if (CarSpeedKm < 5) 
					{ 
						StuckResetTimer += Time.deltaTime;
						Debug.Log ("car name " + gameObject.name + " speed " + CarSpeedKm + " time " + StuckResetTimer +"   StuckResetWait "+StuckResetWait);
					} 
					else 
					{
						Debug.Log ("car speed "+CarSpeedKm+"set timer ** "+StuckResetTimer);
						StuckResetTimer = 0;
					}

					//if (StuckResetTimer > StuckResetWait) 
					if (StuckResetTimer > 1.5f)
					{

						RecoverCar ();

					}
                }
            }

            if (layerChangeStarted)
            {
                layerChangeForIgnoreProcessor();
            }
        }

		IEnumerator RecoverMyCar()
		{
			yield return new WaitForSeconds (5f);
			if (CarSpeedKm < 4) 
			{
				RecoverCar ();
			}
		}
			

        private void RecoverCar()
        {
			StopAllCoroutines ();
//			Debug.Log ("recover car");
			Transform targetWp;
            if (currentWaypoint > 0)
            {
                targetWp = waypoints[currentWaypoint-1].transform; 
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
			transform.position = new Vector3 (targetWp.position.x+ UnityEngine.Random.Range (-5, 5), targetWp.position.y+3, targetWp.position.z); //targetWp.position;
            transform.position += Vector3.up * 0.1f;
            transform.position += Vector3.right * 0.1f;
			myRigid.velocity = Vector3.zero;
			myRigid.angularVelocity = Vector3.zero;
            StuckResetTimer = 0;
		//	Debug.Log (" car recovered");
            layerChangeTimer = layerChangeTimerValue;
            currentLayerCache = LayerMask.LayerToName(gameObject.layer);
//            ChangeLayersRecursively(gameObject.transform, "IGNORE");



            layerChangeStarted = true;
        }
        private void RecoverCar(Transform TargetPosition)
        {
			//Debug.Log ("recover car transform");
			//Debug.DrawLine(transform.position, TargetPosition.position);

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
			myRigid.velocity = Vector3.zero;
			myRigid.angularVelocity = Vector3.zero;
            StuckResetTimer = 0;

            layerChangeTimer = layerChangeTimerValue;
            currentLayerCache = LayerMask.LayerToName(gameObject.layer);
//            ChangeLayersRecursively(gameObject.transform, "IGNORE");

            layerChangeStarted = true;

        }
        //Ignore Collision stuff
        [HideInInspector]
        string currentLayerCache;
        [HideInInspector]
        float layerChangeTimerValue = 7;
        [HideInInspector]
        float layerChangeTimer = 0;
        [HideInInspector]
        bool layerChangeStarted = false;

        private void layerChangeForIgnoreProcessor()
        {
//			Debug.Log ("layer changes for ignore processor");
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

        [Serializable]
        public class SmootingData : System.Object
        {
            public SmoothingBase m_0_30FPS;
            public SmoothingBase m_30_45FPS;
            public SmoothingBase m_45_60FPS;
            public SmoothingBase m_60_NFPS;
        }
        [Serializable]
        public class SmoothingBase : System.Object
        {
            public float Min;
            public float Max;
        }

    }
}