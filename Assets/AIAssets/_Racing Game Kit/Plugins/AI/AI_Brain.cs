//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// PRO AI Script
// Last Change : 22/03/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Helpers;
using RacingGameKit.Interfaces;
using SmartAssembly.Attributes;


namespace RacingGameKit.AI
{
    [AddComponentMenu("")]//menu removal
    public partial class AI_Brain : MonoBehaviour, IRGKRacer
    {

        public string AiConfigFile = "ai_setup_novice";
        [HideInInspector]
        public bool EnableDebugMessages = false;
        [HideInInspector]
        [DoNotObfuscate]
        private Transform CachedDistancePoint;
        [HideInInspector]
        [DoNotObfuscate]
        private Transform CachedReversePoint;
        [HideInInspector]
        [DoNotObfuscate]
        private Transform myTransform;
        [HideInInspector]
        public bool ShowNavDebugLines = true;
        [HideInInspector]
        public bool DrawCollisonGizmo = true;

        [HideInInspector]
        [DoNotObfuscate]
        public bool RandomizeBehaviors = false;

        [HideInInspector]
        [DoNotObfuscate]
        public eAIProLevel AILevelBehavior;

        [HideInInspector]
        [DoNotObfuscate]
        public eAIBehavior AIStateBehavior;

        [HideInInspector]
        [DoNotObfuscate]
        public eAIStress AIStressBehavior;

        [HideInInspector]
        [DoNotObfuscate]
        public LayerMask AIRacerDedectionLayer;

        [HideInInspector]
        [DoNotObfuscate]
        public LayerMask ObstacleDedectionLayer;

        [HideInInspector]
        [DoNotObfuscate]
       private bool DetectDisabledVehicles = false;

        [HideInInspector]
        [DoNotObfuscate]
        public float DedectionRadius = 4f;

        [HideInInspector]
        [DoNotObfuscate]
        public float DedectionFrequency = 0.4f;
        [HideInInspector]
        private float DedectionFrequencyOriginal = 0;
        [HideInInspector]
        private float NextDedectionCycle = 0;


        [HideInInspector]
        [DoNotObfuscate]
        public float CollisionAvoidAngle = 10;
        [HideInInspector]
        private float CollisionAvoidAngleCos = 0.707f;
        [HideInInspector]
        [DoNotObfuscate]
        public  float CollisionAvoidTime = 1f;
        [HideInInspector]
        [DoNotObfuscate]
        public float CollisionAvoidFactor = 0.06f;
        [HideInInspector]
        [DoNotObfuscate]
        private float CollisionAvoidSteer = 0.5f;
        [HideInInspector]
        protected bool WillReCalculate = true;
        [HideInInspector]
        [DoNotObfuscate]
        public eAIRoadPosition AICurrentRoadPosition;
        [HideInInspector]
        [DoNotObfuscate]
        protected eAIRoadPosition AIOldRoadPosition;

        [HideInInspector]
        [DoNotObfuscate]
        public eAIRivalPosition AIAvoidingRivalAvoidPosition;
        [HideInInspector]
        [DoNotObfuscate]
        public eAIRoadPosition AIAvoidingRivalRoadPosition;

        [HideInInspector]
        [DoNotObfuscate]
        public float AISpeedFactor = 1;

        [HideInInspector]
        [DoNotObfuscate]
        public float AISteerFactor = 1;

        [HideInInspector]
        [DoNotObfuscate]
        public float AIEscapeFactor = 1;
        [HideInInspector]
        public float AISoftBrakeFactor = -0.1f;
        [HideInInspector]
        public float AIHardBrakeFactor = -0.3f;
        [HideInInspector]
        protected bool AISoftBraking = false;
        [HideInInspector]
        protected bool AIHardBraking = false;
        [HideInInspector]
        [DoNotObfuscate]
        public float ForwardSensorDistance = 15;
        [HideInInspector]
        private float ForwardSensorDistanceOriginal = 0;
        [HideInInspector]
        [DoNotObfuscate]
        public float ForwardSensorAngle = 7;
        [HideInInspector]
        [DoNotObfuscate]
        public float ForwardSensorWide = 0;

        [HideInInspector]
        [DoNotObfuscate]
        public float SideSensorDistance = 15;

        [HideInInspector]
        [DoNotObfuscate]
        public float SideSensorAngle = 30;
        [HideInInspector]
        [DoNotObfuscate]
        public float SideSensorWide = 0;

        [HideInInspector]
        [DoNotObfuscate]
        public float WallSensorDistance = 1;
        [HideInInspector]
        [DoNotObfuscate]
        public float ReverseSensorDistance = 1;
        [HideInInspector]
        public bool AIReversing = false;
        [HideInInspector]
        public bool ReverseLockedbyObstacle = false;
        [HideInInspector]
        public bool AIWillAvoid = false;
        [HideInInspector]
        public bool AICanEscape = true;
        [HideInInspector]
        public bool AIWillEscape = false;
        
        [DoNotObfuscate]
        private List<Collider> DedectedObjects;
        [HideInInspector]
        [DoNotObfuscate]
        private List<IRGKRacer> ThreatRivals;
        [HideInInspector]
        public float SteerFactor;
        [HideInInspector]
        private float VehicleSpeed = 0;
        [HideInInspector]
        private float PredictedCollisionTime = 0;
        [HideInInspector]
        private bool LeftSensorOK = true;
        [HideInInspector]
        private bool RightSensorOK = true;
        [HideInInspector]
        public bool ObstacleAvoidingEnabled = true;
        [HideInInspector]
        public float ObstacleDetectionDistance = 15;
        [HideInInspector]
        public float ObstacleAvoidFactor = 0.2f;
        [HideInInspector]
        public bool ObstacleAvoidToLeft = false;
        [HideInInspector]
        public bool ObstacleAvoidToRight = false;
        [HideInInspector]
        public bool ObstacleIsAvoiding = false;

        [HideInInspector]
        public float OverlappedCount = 0;
        [HideInInspector]
        public float DedectedRacerCount = 0;
        [HideInInspector]
        private float Steer = 0;
        [HideInInspector]
        public bool UseSideCorrection = false;

        protected void Initialize_AI()
        {
            if (myTransform == null)
            {
                myTransform = transform.GetComponent<Transform>();
                if (myTransform == null)
                {
                    myTransform = transform.parent.GetComponent<Transform>();
                }
            }

            AICurrentRoadPosition = eAIRoadPosition.UnKnown;
            AIOldRoadPosition = eAIRoadPosition.UnKnown;

            DedectionFrequencyOriginal = DedectionFrequency;
            ForwardSensorDistanceOriginal = ForwardSensorDistance;

            if (RandomizeBehaviors)
            {
                AILevelBehavior = (eAIProLevel)UnityEngine.Random.Range(0, 2);
                AIStateBehavior = (eAIBehavior)UnityEngine.Random.Range(0, 2);
            }
            BuildAICoefficiencies();

            CachedDistancePoint = transform.Find("_DistancePoint");
            CachedReversePoint = transform.Find("_ReversePoint");

            if (CachedDistancePoint == null)
            {
                Debug.Log(RGKMessages.DistanceCheckerMissing);
            }

            if (CachedReversePoint == null)
            {
                Debug.Log("Reverse Point Missing!");
            }


        }

        private void BuildAICoefficiencies()
        {
            switch (AILevelBehavior)
            {
                case eAIProLevel.Novice:
                    this.AiConfigFile = "ai_setup_novice";
                    LoadAiConfiguration("ai_setup_novice");
                    break;

                case eAIProLevel.Intermediate:
                    this.AiConfigFile = "ai_setup_intermediate";
                    LoadAiConfiguration("ai_setup_intermediate");
                    break;

                case eAIProLevel.Pro:
                    this.AiConfigFile = "ai_setup_pro";
                    LoadAiConfiguration("ai_setup_pro");
                    break;

                case eAIProLevel.Custom:
                    if (this.AiConfigFile != "")
                    {
                        LoadAiConfiguration(this.AiConfigFile);
                    }
                    break;
            }

        }

        private void LoadAiConfiguration(string ConfigurationFile)
        {
            try
            {
                string jSonData = JsonUtils.ReadJsonFile(ConfigurationFile).Replace("\r\n", ""); ;
                
                if (jSonData != "")
                {
                    JSONObjectForKit j = new JSONObjectForKit(jSonData);

                    if (j.HasField("seperation_factor"))
                    {
                        CollisionAvoidFactor = float.Parse(j.GetField("collision_avoid_factor").str);
                        AISpeedFactor = float.Parse(j.GetField("speed_factor").str);
                        AISteerFactor = float.Parse(j.GetField("steer_factor").str);
                        AIEscapeFactor = float.Parse(j.GetField("escape_factor").str);
                        AISoftBrakeFactor = float.Parse(j.GetField("softbrake_factor").str);
                        AIHardBrakeFactor = float.Parse(j.GetField("hardbrake_factor").str);
                        DedectionRadius = float.Parse(j.GetField("dedection_radius").str);
                        DedectionFrequency = float.Parse(j.GetField("dedection_frequency").str);
                        CollisionAvoidAngle = float.Parse(j.GetField("collision_avoid_angle").str);
                        CollisionAvoidTime = float.Parse(j.GetField("collision_avoid_time").str);
                        ForwardSensorDistance = float.Parse(j.GetField("forward_sensor_distance").str);
                        ForwardSensorAngle = float.Parse(j.GetField("forward_sensor_angle").str);
                        SideSensorDistance = float.Parse(j.GetField("side_sensor_distance").str);
                        SideSensorAngle = float.Parse(j.GetField("side_sensor_angle").str);
                        WallSensorDistance = float.Parse(j.GetField("wall_sensor_distance").str);
                        ObstacleAvoidingEnabled = j.GetField("enable_obstacle_avoid").b;
                        ObstacleDetectionDistance = float.Parse(j.GetField("obstacle_dedection_distance").str);
                        ObstacleAvoidFactor = float.Parse(j.GetField("obstacle_avoid_factor").str);
                    }
                }
            }
            catch
            {
                Debug.LogError("INVALID AI CONFIG FILE!");
                Debug.Break();
            }
        }

        public void ExecuteRadar()
        {
            CollisionAvoidAngleCos = Mathf.Cos(CollisionAvoidAngle * Mathf.Deg2Rad);

            NextDedectionCycle -= Time.deltaTime;

            if (NextDedectionCycle <= 0f)
            {
                NextDedectionCycle = DedectionFrequency;

                //Dedect Rivals around the car
                DedectedObjects = Detect();
                //Filter them if they also AI or player
                FilterDetected();
                //Calculate Stress Level of AI
                CalculateStress();
                //Calculate Steer Factor
                SteerFactor = CalculateSteerFactor();

                if (SteerFactor != 0)
                { AIWillAvoid = true; }
                else
                { AIWillAvoid = false; }

                //Calculate Escape Positiong 
                CalculateEscaping();

                //Calculate Dedection Frequency based rivals
                ReCalculateDedection();
            }
        }

        void ReCalculateDedection()
        {
            if (DedectionFrequency > 0)
            {
                if (ThreatRivals.Count == 1)
                {
                    DedectionFrequency = DedectionFrequencyOriginal / 2;
                }
                else if (ThreatRivals.Count > 2)
                {
                    DedectionFrequency = DedectionFrequencyOriginal / 3;
                }
                else
                {
                    DedectionFrequency = DedectionFrequencyOriginal;
                }
            }
        }

        void CalculateStress()
        {
            if (ThreatRivals.Count == 0)
            {
                AIStressBehavior = eAIStress.Normal;
            }
            else if (ThreatRivals.Count >= 1 && ThreatRivals.Count < 2)
            {
                AIStressBehavior = eAIStress.Awared;
            }
            else if (ThreatRivals.Count >= 2)
            {
                AIStressBehavior = eAIStress.Stressed;
            }

            switch (AIStressBehavior)
            {
                case eAIStress.Relaxed:

                    break;
                case eAIStress.Normal:

                    break;
                case eAIStress.Awared:

                    break;
                case eAIStress.Stressed:

                    break;
            }
        }
        private float NewEscape = 0;
        private float CalcEscape = 0;
        protected float ThinkNewEscapePoint(float WideLeft, float WideRight, int CurrentWaypointIndex, float CalculatedEscapeDistance, float CurrentRelativePisiton, eAIRoadPosition AIRoadPosition)
        {
            NewEscape = 0;
            float Wide = 0;
            CalcEscape = CalculatedEscapeDistance;

            //Determinate where we are to current waypoint

            if (CalculatedEscapeDistance > 0) //we're at left
            {
                AICurrentRoadPosition = eAIRoadPosition.Left;
                Wide = WideLeft;
            }
            else if (CalculatedEscapeDistance < 0) //we're at Right
            {
                AICurrentRoadPosition = eAIRoadPosition.Right;
                Wide = WideRight;
            }


            //Cases 
            if (AIWillAvoid && !AIWillEscape) //Avoiding side and front collisions based steering
            {

                if (AIRoadPosition == eAIRoadPosition.Left && AIAvoidingRivalRoadPosition == eAIRoadPosition.Left)
                {
                    if (AIAvoidingRivalAvoidPosition == eAIRivalPosition.Left)
                    {
                        NewEscape = UnityEngine.Random.Range(0.1f, Wide / 2);
                    }
                    else
                    {
                        NewEscape = UnityEngine.Random.Range(Wide / 2, Wide);
                    }
                }
                else if (AIRoadPosition == eAIRoadPosition.Right && AIAvoidingRivalRoadPosition == eAIRoadPosition.Right)
                {
                    if (AIAvoidingRivalAvoidPosition == eAIRivalPosition.Left)
                    {
                        NewEscape = UnityEngine.Random.Range(Wide / 2, Wide);
                    }
                    else
                    {
                        NewEscape = UnityEngine.Random.Range(0.1f, Wide / 2);
                    }
                }
                else
                {
                    if (AIAvoidingRivalAvoidPosition == eAIRivalPosition.Left)
                    {
                        NewEscape = UnityEngine.Random.Range(Wide / 2, Wide);
                    }
                    else
                    {
                        NewEscape = UnityEngine.Random.Range(0.1f, Wide / 2);
                    }
                }

                WriteToConsole("AVOIDING!");
            }
            else if (AIWillAvoid && AIWillEscape) //Faster car and avoiding collision into front car
            {
                WriteToConsole("I NEED TO ESCAPE!");
                UseSideCorrection = false;
                if (AIRoadPosition == eAIRoadPosition.Left)
                {
                    if (RightSensorOK)
                    {
                        NewEscape = UnityEngine.Random.Range(0.2f, WideRight);
                        WriteToConsole("LEFT - ESCAPING RIGHT");
                    }
                    else if (LeftSensorOK)
                    {
                        NewEscape = UnityEngine.Random.Range(WideLeft - 1, WideLeft);
                        WriteToConsole("LEFT - ESCAPING LEFT");
                    }

                }
                else
                {
                    if (RightSensorOK)
                    {
                        NewEscape = UnityEngine.Random.Range(0.2f, WideLeft);
                        WriteToConsole("RIGHT - ESCAPING LEFT");
                    }
                    else if (LeftSensorOK)
                    {
                        NewEscape = UnityEngine.Random.Range(WideRight - 1, WideRight);
                        WriteToConsole("RIGHT - ESCAPING RIGHT");
                    }
                }

                AIWillEscape = false;
                AIWillAvoid = false;

            }
            else //No avoid or no drafting, so continue to best course
            {
                UseSideCorrection = true;
                if (Wide <= Mathf.Abs(AIEscapeFactor))
                {
                    NewEscape = UnityEngine.Random.Range(0.2f, Wide);
                    WriteToConsole("NO ESCAPE - WIDE CALC");
                }
                else
                {
                    NewEscape = UnityEngine.Random.Range(0.5f, AIEscapeFactor);
                    WriteToConsole("NO ESCAPE - ESCAPE FACTOR");
                }

                //if (AICurrentRoadPosition == eAIRoadPosition.Right && NewEscape < 0)
                //{
                //    NewEscape *= -1;
                //    WriteToConsole("RIGHht - NO ESCAPE - NEW ESC<0");
                //}
                //else if (AICurrentRoadPosition == eAIRoadPosition.Left && NewEscape > 0)
                //{
                //    NewEscape *= -1;
                //    WriteToConsole("RIGHht - NO ESCAPE - NEW ESC>0");
                //}


            }



            if (CurrentWaypointIndex == 0) //Game Start so we've to find where're we 
            {
                NewEscape = CalculatedEscapeDistance;

                if (Mathf.Abs(CalculatedEscapeDistance) > Wide)
                {
                    NewEscape = UnityEngine.Random.Range(0.2f, Wide);
                }
                AIOldRoadPosition = AICurrentRoadPosition;
            }
            else
            {
                if (Mathf.Abs(CurrentRelativePisiton) < 35 || !AIWillAvoid && !AIWillEscape)
                {
                    if (AICurrentRoadPosition != AIOldRoadPosition)
                    {
                        //Stay at right as possible
                        if (AIOldRoadPosition == eAIRoadPosition.Right && AICurrentRoadPosition == eAIRoadPosition.Left)
                        {
                            AICurrentRoadPosition = eAIRoadPosition.Right;
                            WriteToConsole("STAYING RIGHT");
                        }
                        //Stay at left as possible
                        if (AIOldRoadPosition == eAIRoadPosition.Left && AICurrentRoadPosition == eAIRoadPosition.Right)
                        {
                            AICurrentRoadPosition = eAIRoadPosition.Left;
                            WriteToConsole("STAYING LEFT");
                        }

                        if (AICurrentRoadPosition == eAIRoadPosition.Left) AIOldRoadPosition = eAIRoadPosition.Left;
                        if (AICurrentRoadPosition == eAIRoadPosition.Right) AIOldRoadPosition = eAIRoadPosition.Right;
                    }

                }
                else
                {
                    AIOldRoadPosition = AICurrentRoadPosition;
                }
            }

            if (UseSideCorrection)
            {
                if (AICurrentRoadPosition == eAIRoadPosition.Right && NewEscape > 0) NewEscape *= -1;
                if (AICurrentRoadPosition == eAIRoadPosition.Left && NewEscape < 0) NewEscape *= -1;
            }

            return NewEscape;
        }

        bool DecideBreaking()
        {
            bool shouldIBrake = false;
            if (this.AIStateBehavior == eAIBehavior.UnConfident)
            {
                shouldIBrake = true;
            }
            else if (this.AIStateBehavior == eAIBehavior.Normal && AIStressBehavior == eAIStress.Stressed)
            {
                shouldIBrake = true;
            }

            if (!AICanEscape) shouldIBrake = true;

            return shouldIBrake;
        }

        private float PredictedCollisionTimeOld = 0f;


        void CalculateEscaping()
        {
            SideSensorWide = Mathf.Tan(SideSensorAngle * Mathf.Deg2Rad) * SideSensorDistance;
            ForwardSensorWide = Mathf.Tan(ForwardSensorAngle * Mathf.Deg2Rad) * SideSensorDistance;
            //AIWallSensorWide = Mathf.Tan(AISideSensorAngle * Mathf.Deg2Rad) * AIWallSensorDistance;



            Transform SensorsCenterPoint = CachedDistancePoint;

            Vector3 ForwardSensorLeftPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x - 0.1f)));
            Vector3 ForwaradSensorRightPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x + 0.1f)));
            Vector3 ForwardSensorHorizon = SensorsCenterPoint.TransformDirection(Vector3.forward * ForwardSensorDistance);
            Vector3 ForwardSensorLeftHorizon = SensorsCenterPoint.TransformDirection((Vector3.left * ForwardSensorWide) + (Vector3.forward * ForwardSensorDistance));
            Vector3 ForwardSensorRightHorizon = SensorsCenterPoint.TransformDirection((Vector3.right * ForwardSensorWide) + (Vector3.forward * ForwardSensorDistance));

            Vector3 SideSensorLeftPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x - 0.5f)));
            Vector3 SideSensorRightPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x + 0.5f)));
            Vector3 SideSensorLeftHorizon = SensorsCenterPoint.TransformDirection((Vector3.left * SideSensorWide) + (Vector3.forward * SideSensorDistance));
            Vector3 SideSensorRightHorizon = SensorsCenterPoint.TransformDirection((Vector3.right * SideSensorWide) + (Vector3.forward * SideSensorDistance));

            Vector3 WallSensorLeftPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x - 0.5f)));
            Vector3 WallSensorRightPosition = SensorsCenterPoint.position + SensorsCenterPoint.TransformDirection((Vector3.right * (SensorsCenterPoint.localPosition.x + 0.5f)));
            Vector3 WallSensorLeftHorizon = SensorsCenterPoint.TransformDirection(-WallSensorDistance, 0, 0);
            Vector3 WallSensorRightHorizon = SensorsCenterPoint.TransformDirection(WallSensorDistance, 0, 0);

            
            if (ShowNavDebugLines)
            {
//                Debug.DrawRay(SensorsCenterPoint.position, ForwardSensorHorizon, Color.blue);
//                Debug.DrawRay(ForwardSensorLeftPosition, ForwardSensorLeftHorizon, Color.blue);
//                Debug.DrawRay(ForwaradSensorRightPosition, ForwardSensorRightHorizon, Color.blue);
//                Debug.DrawRay(WallSensorLeftPosition, WallSensorLeftHorizon, Color.magenta);
//                Debug.DrawRay(WallSensorRightPosition, WallSensorRightHorizon, Color.magenta);

            }


            RaycastHit HitAIForwardSensor;
            RaycastHit HitAILeftSensor;
            RaycastHit HitAIRightSensor;
            
            //RaycastHit HitObjectLeftWallSensor;
            //RaycastHit HitObjectRightWallSensor;

            #region AIDedection

            //RaycastHit HitRight45 for AI;
            if (Physics.Raycast(SensorsCenterPoint.position, ForwardSensorHorizon, out HitAIForwardSensor, ForwardSensorDistance, AIRacerDedectionLayer)
                || Physics.Raycast(ForwardSensorLeftPosition, ForwardSensorLeftHorizon, out HitAIForwardSensor, ForwardSensorDistance, AIRacerDedectionLayer)
                || Physics.Raycast(ForwaradSensorRightPosition, ForwardSensorRightHorizon, out HitAIForwardSensor, ForwardSensorDistance, AIRacerDedectionLayer)
                )
            {
                ForwardSensorDistance = HitAIForwardSensor.distance;

                GameObject ApproachingObject = null;

                if (HitAIForwardSensor.transform.parent != null)
                {
                    ApproachingObject = HitAIForwardSensor.transform.parent.gameObject;
                }
                else
                {
                    ApproachingObject = HitAIForwardSensor.transform.gameObject;
                }
                var ApproachingAI = ApproachingObject.GetComponent<AI_Brain>();

                if (ApproachingAI != null)
                {
                    if ((this.Speed - ApproachingAI.Speed) > 0)
                    {
                        PredictedCollisionTime = PredictNearestApproachTime(ApproachingAI);

                        if (DecideBreaking())
                        {
                            WriteToConsole("I'm faster - breaking!");
                            AISoftBraking = true;
                            if (ForwardSensorDistance <= DedectionRadius)
                            {
                                AIHardBraking = true;
                                WriteToConsole("I'm faster - HARD  breaking!");
                            }
                        }
                        else
                        {
                            if (ShowNavDebugLines)
                            {
//                                Debug.DrawRay(SideSensorLeftPosition, SideSensorLeftHorizon, Color.blue);
//                                Debug.DrawRay(SideSensorRightPosition, SideSensorRightHorizon, Color.blue);
                            }
                            LeftSensorOK = true;
                            RightSensorOK = true;
                            //Left Sensor Dedection
                            if (Physics.Raycast(SideSensorLeftPosition, SideSensorLeftHorizon, out HitAILeftSensor, SideSensorDistance, AIRacerDedectionLayer))
                            {
                                LeftSensorOK = false;
                            }
                            //Right Sensor Dedection
                            if (Physics.Raycast(SideSensorRightPosition, SideSensorRightHorizon, out HitAIRightSensor, SideSensorDistance, AIRacerDedectionLayer))
                            {
                                RightSensorOK = false;
                            }

                            if (LeftSensorOK || RightSensorOK)
                            {
                                WriteToConsole("I'm faster - looking for escape! ");
                                AIWillAvoid = true;
                                AICanEscape = true;
                                AIWillEscape = true;
                            }
                            else
                            {
                                WriteToConsole("I'm faster - Sensors sux");
                                AICanEscape = false;
                                AIWillEscape = false;
                            }
                        }
                    }
                }
            }
            else
            {
                ForwardSensorDistance = ForwardSensorDistanceOriginal;
                if (AISoftBraking) AISoftBraking = false;
                if (AIHardBraking) AIHardBraking = false;


            }
            #endregion

            #region Obstacle Dedection

            if (ObstacleAvoidingEnabled && !AIReversing)
            {
                RaycastHit HitObjectForwardSensor;
                RaycastHit HitObjectLeftSensor;
                RaycastHit HitObjectRightSensor;



                //Obstacle Dedection
                if (Physics.Raycast(SensorsCenterPoint.position, ForwardSensorHorizon, out HitObjectForwardSensor, ObstacleDetectionDistance, ObstacleDedectionLayer)
                   || Physics.Raycast(ForwardSensorLeftPosition, ForwardSensorLeftHorizon, out HitObjectForwardSensor, ObstacleDetectionDistance, ObstacleDedectionLayer)
                   || Physics.Raycast(ForwaradSensorRightPosition, ForwardSensorRightHorizon, out HitObjectForwardSensor, ObstacleDetectionDistance, ObstacleDedectionLayer)
                   || Physics.Raycast(WallSensorLeftPosition, WallSensorLeftHorizon, out HitObjectForwardSensor, WallSensorDistance, ObstacleDedectionLayer)
                   || Physics.Raycast(WallSensorRightPosition, WallSensorRightHorizon, out HitObjectForwardSensor, WallSensorDistance, ObstacleDedectionLayer))
                {

                    float predictedCollitionTime = PredictCollisionTime(HitObjectForwardSensor.transform.gameObject);
                    if (PredictedCollisionTimeOld == 0)
                    {
                        PredictedCollisionTimeOld = predictedCollitionTime;


                        ObstacleIsAvoiding = true;

                        WriteToConsole("Going to hit something in " + predictedCollitionTime);

                        if (ShowNavDebugLines)
                        {
//                            Debug.DrawRay(SideSensorLeftPosition, SideSensorLeftHorizon, Color.magenta);
//                            Debug.DrawRay(SideSensorRightPosition, SideSensorRightHorizon, Color.magenta);
                        }
                        ObstacleAvoidToLeft = true;
                        ObstacleAvoidToRight = true;

                        //Left Sensor Dedection
                        if (Physics.Raycast(SideSensorLeftPosition, SideSensorLeftHorizon, out HitObjectLeftSensor, SideSensorDistance, ObstacleDedectionLayer))
                        {
                            ObstacleAvoidToLeft = false;
                        }
                        //Right Sensor Dedection
                        if (Physics.Raycast(SideSensorRightPosition, SideSensorRightHorizon, out HitObjectRightSensor, SideSensorDistance, ObstacleDedectionLayer))
                        {
                            ObstacleAvoidToRight = false;
                        }

                    }
                    else
                    {
                        PredictedCollisionTimeOld = 0f;
                    }


                }
                else
                {

                    ObstacleIsAvoiding = false;
                }

                if (ObstacleIsAvoiding)
                {
                    DecideObsticleAvoid(ObstacleAvoidToLeft, ObstacleAvoidToRight, AIAvoidingRivalAvoidPosition);

                }

            }

            if (CachedReversePoint != null)
            {
                Transform SensorReversePoint = CachedReversePoint;
                Vector3 ReverseSensorHorizon = SensorReversePoint.TransformDirection(Vector3.back * ReverseSensorDistance);
                RaycastHit HitReverseSensor;

                if (AIReversing)
                {
                    if (ShowNavDebugLines)
                    {
                       // Debug.DrawRay(SensorReversePoint.position, ReverseSensorHorizon, Color.magenta);
                    }

                    if (Physics.Raycast(SensorReversePoint.position, ReverseSensorHorizon, out HitReverseSensor, ReverseSensorDistance, ObstacleDedectionLayer))
                    {
                        ReverseLockedbyObstacle = true;
                        AIReversing = false;
                    }
                }
            }
            #endregion


        }

        private void DecideObsticleAvoid(bool CanAvoidToLeft, bool CaAvoidToRight, eAIRivalPosition AvoidingRivalPosition)
        {

            bool ShouldIBrake = false;
            bool ShouldISteerLeft = false;
            bool ShouldISteerRight = false;

            if (ObstacleAvoidToLeft)
            {
                if (AvoidingRivalPosition == eAIRivalPosition.Left)
                {
                    ShouldISteerLeft = false;
                }
                else
                {
                    ShouldISteerLeft = true;
                }

            }

            if (ObstacleAvoidToRight)
            {
                if (AvoidingRivalPosition == eAIRivalPosition.Right)
                {
                    ShouldISteerRight = false;
                }
                else
                {
                    ShouldISteerRight = true;
                }
            }

            if (ShouldISteerRight && ShouldISteerLeft)
            {
                if (AICurrentRoadPosition == eAIRoadPosition.Left)
                {
                    SteerFactor = ObstacleAvoidFactor;
                }
                else if (AICurrentRoadPosition == eAIRoadPosition.Right)
                {
                    SteerFactor -= ObstacleAvoidFactor;
                }
            }
            else if (ShouldISteerLeft && !ShouldISteerRight)
            {
                SteerFactor -= ObstacleAvoidFactor;
            }
            else if (ShouldISteerRight && !ShouldISteerLeft)
            {
                SteerFactor += ObstacleAvoidFactor;
            }
            else
            {
                ShouldIBrake = true;
            }

            if (ShouldIBrake) AIHardBraking = true;

        }

        private List<Collider> Detect()
        {
            var detected = Physics.OverlapSphere(this.Position, DedectionRadius, AIRacerDedectionLayer);
            var list = new List<Collider>();
            foreach (Collider oCol in detected)
            {
                list.Add(oCol);
            }
            OverlappedCount = list.Count;
            return list;

        }

        private void FilterDetected()
        {
            if (ThreatRivals == null)
            {
                ThreatRivals = new List<IRGKRacer>();
            }

            ThreatRivals.Clear();


            foreach (var ApproachingObject in DedectedObjects)
            {
                GameObject ApproachingVehicle = null;

                ApproachingVehicle = GetParent(ApproachingObject.transform);

                //IRGKRacer ApproacingAI = ApproachingVehicle.GetComponent<IRGKRacer>();
                IRGKRacer ApproacingAI = (IRGKRacer)ApproachingVehicle.GetComponent(typeof(IRGKRacer));

                if (ApproacingAI != null && ApproachingVehicle != this.gameObject)
                {
                    ThreatRivals.Add(ApproacingAI);

                    if (ShowNavDebugLines)
                    {
                        //Debug.DrawLine(this.Position, ApproachingObject.transform.position, Color.magenta);
                    }
                }
            }

            DedectedRacerCount = ThreatRivals.Count;
        }

        #region Steering

        protected float CalculateSteerFactor()
        {

            Steer = 0;
            IRGKRacer threat = null;

            float minTime = CollisionAvoidTime;

            Vector3 threatPositionAtNearestApproach = Vector3.zero;

            Vector3 ourPositionAtNearestApproach = Vector3.zero;


            foreach (var other in ThreatRivals)
            {
                if (other != this)
                {

                    float collisionDangerThreshold = this.DedectionRadius;

                    float time = PredictNearestApproachTime(other);
                    if ((time >= 0) && (time < minTime))
                    {
                        Vector3 ourPos = Vector3.zero;
                        Vector3 hisPos = Vector3.zero;
                        float dist = ComputeNearestApproachPositions(other, time, ref ourPos, ref hisPos);

                        if (dist < collisionDangerThreshold)
                        {
                            minTime = time;
                            threat = other;
                            threatPositionAtNearestApproach = hisPos;

                            ourPositionAtNearestApproach = ourPos;

                        }
                    }
                }
            }

            if (threat != null)
            {
                float parallelness = Vector3.Dot(transform.forward, threat.CachedTransform.forward);

                if (parallelness < -CollisionAvoidAngleCos)
                {
                    Vector3 offset = threatPositionAtNearestApproach - this.Position;
                    float sideDot = Vector3.Dot(offset, transform.right);
                    Steer = (sideDot > 0) ? -CollisionAvoidFactor : CollisionAvoidFactor;
                }
                else if (parallelness > CollisionAvoidAngleCos)
                {
                    Vector3 offset = threat.Position - this.Position;
                    float sideDot = Vector3.Dot(offset, transform.right);

                    Steer = (sideDot > 0) ? -CollisionAvoidFactor : CollisionAvoidFactor;
                    AIAvoidingRivalAvoidPosition = (sideDot > 0) ? eAIRivalPosition.Right : eAIRivalPosition.Left;
                    AIAvoidingRivalRoadPosition = threat.CurrentRoadPosition;
                }
                else
                {
                    if (this.Speed < threat.Speed
                             || threat.Speed == 0
                             || gameObject.GetInstanceID() < threat.CachedGameObject.GetInstanceID())
                    {
                        float sideDot = Vector3.Dot(transform.right, threat.Velocity);
                        Steer = (sideDot > 0) ? (-1 * 1) : 1;
                    }
                }

            }
            else
            {
                AIAvoidingRivalAvoidPosition = eAIRivalPosition.NoRival;
            }
            return Steer;
        }

        public float PredictNearestApproachTime(IRGKRacer other)
        {

            Vector3 myVelocity = Velocity;
            Vector3 otherVelocity = other.Velocity;
            Vector3 relVelocity = otherVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;
            if (relSpeed == 0) return 0;
            Vector3 relTangent = relVelocity / relSpeed;
            Vector3 relPosition = Position - other.Position;
            float projection = Vector3.Dot(relTangent, relPosition);

            return projection / relSpeed;
        }

        //Experimental
        public float PredictCollisionTime(GameObject other)
        {
            Vector3 myVelocity = Velocity;
            Vector3 otherVelocity = other.transform.forward;
            Vector3 relVelocity = otherVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;

            if (relSpeed == 0) return 0;

            Vector3 relTangent = relVelocity / relSpeed;

            Vector3 relPosition = Position - other.transform.position;
            float projection = Vector3.Dot(relTangent, relPosition);

            return projection / relSpeed;
        }

        protected float ComputeNearestApproachPositions(IRGKRacer other, float time, ref Vector3 ourPosition, ref Vector3 hisPosition)
        {
            return ComputeNearestApproachPositions(other, time, ref ourPosition, ref hisPosition, Speed, myTransform.forward);
        }

        protected float ComputeNearestApproachPositions(IRGKRacer other, float time, ref Vector3 ourPosition, ref Vector3 hisPosition, float ourSpeed, Vector3 ourForward)
        {
            Vector3 myTravel = ourForward * ourSpeed * time;
            Vector3 otherTravel = other.CachedTransform.forward * other.Speed * time;

            ourPosition = Position + myTravel;
            hisPosition = other.Position + otherTravel;
            return Vector3.Distance(ourPosition, hisPosition);
        }


        #endregion
        #region Properties


        public Vector3 Velocity
        {
            get
            {
                if (myTransform == null)
                {
                    myTransform = transform.GetComponent<Transform>();
                    if (myTransform == null)
                    {
                        myTransform = transform.parent.GetComponent<Transform>();
                    }
                }

                return myTransform.forward * VehicleSpeed;
            }
        }

        public float Speed
        {
            get
            {
                return VehicleSpeed;
            }
            set { VehicleSpeed = value; }

        }

        public Transform CachedTransform
        {
            get
            {
                return transform;
            }
        }
        public GameObject CachedGameObject
        {
            get
            {
                return this.gameObject;
            }
        }

        public eAIRoadPosition CurrentRoadPosition
        {
            get
            {
                return this.AICurrentRoadPosition;
            }
            set { AICurrentRoadPosition = value; }
        }
        public Vector3 Position
        {
            get
            {

                return myTransform.position;
            }
        }
        #endregion
        #region Helpers

        private GameObject GetParent(Transform ParentSeekingObject)
        {
            if (ParentSeekingObject.transform.parent != null)
            {
                return GetParent(ParentSeekingObject.transform.parent.transform);
            }
            else
            {
                return ParentSeekingObject.gameObject;
            }
        }

        void OnDrawGizmos()
        {
            if (DrawCollisonGizmo)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, DedectionRadius);
            }
        }

        private void WriteToConsole(string Message)
        {
//            if (EnableDebugMessages) Debug.Log(Message);
        }

        #endregion

    }
}