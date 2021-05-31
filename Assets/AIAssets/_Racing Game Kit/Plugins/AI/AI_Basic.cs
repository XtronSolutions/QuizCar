//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Basic AI Script
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
    public partial class AI_Basic : MonoBehaviour, IRGKRacer
    {
        [DoNotObfuscate]
        private Transform CachedReversePoint;
        [DoNotObfuscate]
        private Transform myTransform;
        [DoNotObfuscate]
        private List<Collider> DedectedObjects;
        [DoNotObfuscate]
        private List<IRGKRacer> ThreatRivals;
        private bool EnableDebugMessages = false;
        public bool ShowNavDebugLines = true;
        public bool DrawCollisonGizmo = true;

        public float AISoftBrakeFactor = -0.1f;
        public float AIHardBrakeFactor = -0.3f;
        [HideInInspector]
        public bool AICanAvoid = false;
        [HideInInspector]
        public bool AIWillAvoid = false;
        [DoNotObfuscate]
        public float DedectionRadius = 4f;
        
        [DoNotObfuscate]
        public float DedectionFrequency = 0.4f;
        private float DedectionFrequencyOriginal = 0;
        private float NextDedectCycle = 0;
        
        [DoNotObfuscate]
        public LayerMask AIRacerDedectionLayer;
        [DoNotObfuscate]
        public LayerMask ObstacleDedectionLayer;
        [DoNotObfuscate]
        private bool DetectDisabledVehicles = false;
        
        [DoNotObfuscate]
        public float CollisionAvoidAngle = 10;
        [DoNotObfuscate]
        float CollisionAvoidAngleCos = 0.707f;
        
        [DoNotObfuscate]
        public float CollisionAvoidTime = 1f;
        
        [DoNotObfuscate]
        public float CollisionAvoidFactor = 0.02f;

        internal bool WillReCalculate = true;
        [HideInInspector]
        public eAIRoadPosition AICurrentRoadPosition;
        [HideInInspector]
        public float OverlappedCount = 0;
        [HideInInspector]
        public float DedectedRacerCount = 0;
        [HideInInspector]
        public float Steer = 0;
        [HideInInspector]
        public float SteerFactor;

        private float VehicleSpeed = 0;
        [DoNotObfuscate]
        private float ReverseSensorDistance = 1;
        public bool AIReversing = false;
        public bool ReverseLockedbyObstacle = false;

        protected void Initialize_AI()
        {
            CachedReversePoint = transform.Find("_ReversePoint");

            if (CachedReversePoint == null)
            {
                Debug.Log("Reverse Point Missing!");
            }

            if (myTransform == null)
            {
                myTransform = transform.GetComponent<Transform>();
                if (myTransform == null)
                {
                    myTransform = transform.parent.GetComponent<Transform>();
                }
            }

            AICurrentRoadPosition = eAIRoadPosition.UnKnown;

            DedectionFrequencyOriginal = DedectionFrequency;

        }


        public void ExecuteRadar()
        {
            CollisionAvoidAngleCos = Mathf.Cos(CollisionAvoidAngle * Mathf.Deg2Rad);

            NextDedectCycle -= Time.deltaTime;

            if (NextDedectCycle < 0.1f)
            {
                NextDedectCycle = DedectionFrequency;

                //Dedect Rivals around the car
                DedectedObjects = Detect();
                //Filter them if they also AI or player
                FilterDetected();

                //Calculate Steer Factor
                SteerFactor = CalculateSteerFactor();
                //Calculate Dedection Frequency based rivals
                CalculateEscaping();

                ReCalculateDedection();
            }
        }

        private void CalculateEscaping()
        {
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
        [DoNotObfuscate]
        List<Collider> Detect()
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

                //AI_Brain ApproacingAI = ApproachingVehicle.GetComponent<AI_Brain>();
                IRGKRacer ApproacingAI = (IRGKRacer)ApproachingVehicle.GetComponent(typeof(IRGKRacer));


                if (ApproacingAI != null && ApproachingVehicle != this.gameObject)
                {
                    ThreatRivals.Add(ApproacingAI);

                    if (ShowNavDebugLines)
                    {
                       // Debug.DrawLine(this.Position, ApproachingObject.transform.position, Color.magenta);
                    }
                }
            }

            DedectedRacerCount = ThreatRivals.Count;
        }

        private GameObject FindParent(Transform ObjectForParentSearch)
        {
            if (ObjectForParentSearch.parent == null)
            {
                return ObjectForParentSearch.gameObject;
            }
            else
            {
                return FindParent(ObjectForParentSearch.parent);
            }

        }


        protected float CalculateSteerFactor()
        {
            /*
            // first priority is to prevent immediate interpenetration
            Vector3 separation = steerToAvoidCloseNeighbors (0, others);
            if (separation != Vector3.zero) 
            {
                return separation;
            }
            */

            // otherwise, go on to consider potential future collisions
            Steer = 0;
            IRGKRacer threat = null;

            // Time (in seconds) until the most immediate collision threat found
            // so far.	Initial value is a threshold: don't look more than this
            // many frames into the future.
            float minTime = CollisionAvoidTime;

            Vector3 threatPositionAtNearestApproach = Vector3.zero;

            Vector3 ourPositionAtNearestApproach = Vector3.zero;

            // for each of the other vehicles, determine which (if any)
            // pose the most immediate threat of collision.
            foreach (var other in ThreatRivals)
            {
                if (other != this) //&& !other.AIWillAvoid && AICanAvoid
                {
                    // avoid when future positions are this close (or less)
                    float collisionDangerThreshold = this.DedectionRadius;

                    // predicted time until nearest approach of "this" and "other"
                    float time = PredictNearestApproachTime(other);

                    // If the time is in the future, sooner than any other
                    // threatened collision...
                    if ((time >= 0) && (time < minTime))
                    {
                        // if the two will be close enough to collide,
                        // make a note of it
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


            // if a potential collision was found, compute steering to avoid
            if (threat != null)
            {
                // parallel: +1, perpendicular: 0, anti-parallel: -1
                float parallelness = Vector3.Dot(transform.forward, threat.CachedTransform.forward);
                //Debug.Log("Parallel " + parallelness + " " + _avoidAngleCos + " " + threatPositionAtNearestApproach);

                if (parallelness < -CollisionAvoidAngleCos)
                {
                    // anti-parallel "head on" paths:
                    // steer away from future threat position
                    Vector3 offset = threatPositionAtNearestApproach - this.Position;
                    float sideDot = Vector3.Dot(offset, transform.right);
                    Steer = (sideDot > 0) ? -CollisionAvoidFactor : CollisionAvoidFactor;
                }
                else if (parallelness > CollisionAvoidAngleCos)
                {
                    // parallel paths: steer away from threat
                    Vector3 offset = threat.Position - this.Position;
                    float sideDot = Vector3.Dot(offset, transform.right);

                    Steer = (sideDot > 0) ? -CollisionAvoidFactor : CollisionAvoidFactor;
                    //AIAvoidingRivalAvoidPosition = (sideDot > 0) ? eAIRivalPosition.Right : eAIRivalPosition.Left;
                    //AIAvoidingRivalRoadPosition = threat.AICurrentRoadPosition;
                }
                else
                {
                    /* 
                        Perpendicular paths: steer behind threat

                        Only the slower vehicle attempts this, unless that 
                        slower vehicle is static.  If both have the same
                        speed, then the one with the lowest index falls
                        behind.					
					
                        Something to test is making a slower vehicle fall
                        behind, while a faster vehicle cuts ahead.
                     */
                    if (this.Speed < threat.Speed
                             || threat.Speed == 0
                             || gameObject.GetInstanceID() < threat.CachedGameObject.GetInstanceID())
                    {
                        float sideDot = Vector3.Dot(transform.right, threat.Velocity);
                        Steer = (sideDot > 0) ? -1.0f : 1.0f;
                    }
                }

                /* Steer will end up being applied as a multiplier to the
                   vehicle's side vector. If we simply apply te -1/+1 being
                   assigned above, then we'll end up with a unit displacement
                   from the other object's position. We should account for
                   both its radius and our own.
                 */
                //steer *= this.DedectionRadius+ threat.DedectionRadius;


            }
            else
            {
                //AIAvoidingRivalAvoidPosition = eAIRivalPosition.NoRival;
            }

            //if
            //   (DedectedCount > 1)
            //{ return 0; }
            //else
            //{
            //    return Steer;
            //}
            return Steer;
        }

        public float PredictNearestApproachTime(IRGKRacer other)
        {
            // imagine we are at the origin with no velocity,
            // compute the relative velocity of the other vehicle
            Vector3 myVelocity = Velocity;
            Vector3 otherVelocity = other.Velocity;
            Vector3 relVelocity = otherVelocity - myVelocity;
            float relSpeed = relVelocity.magnitude;

            // for parallel paths, the vehicles will always be at the same distance,
            // so return 0 (aka "now") since "there is no time like the present"
            if (relSpeed == 0) return 0;

            // Now consider the path of the other vehicle in this relative
            // space, a line defined by the relative position and velocity.
            // The distance from the origin (our vehicle) to that line is
            // the nearest approach.

            // Take the unit tangent along the other vehicle's path
            Vector3 relTangent = relVelocity / relSpeed;

            // find distance from its path to origin (compute offset from
            // other to us, find length of projection onto path)
            Vector3 relPosition = Position - other.Position;
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
            set
            {
                VehicleSpeed = value;
            }
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
                return AICurrentRoadPosition;
            }

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
                if (myTransform == null)
                {
                    myTransform = this.transform;
                }
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Position, DedectionRadius);
            }
        }

        private void WriteToConsole(string Message)
        {
            if (EnableDebugMessages) Debug.Log(Message);
        }

        #endregion

    }
}