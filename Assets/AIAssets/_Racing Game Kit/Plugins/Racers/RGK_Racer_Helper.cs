//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// External Racer Controller Helper Functions
// Last Change : 22/03/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Interfaces;
using RacingGameKit.Helpers;
using SmartAssembly.Attributes;
using System;

namespace RacingGameKit.Racers
{
    [AddComponentMenu("")]
    [DoNotObfuscate()]
    public class  RGK_Racer_Helper:MonoBehaviour
    {
        protected static Race_Manager RaceManager;
        protected static Racer_Register RaceRegister;
        private static GameObject waypointContainer;
        private static List<Transform> waypoints;
        public static string ControlsResetBinding = "ResetVehicle";

        private static float CarSpeedKm = 0;
        private static Transform CachedTransform; 
     

        public static void HelperInit(Transform TransformObject)
        {
            CachedTransform=TransformObject;
            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            RaceRegister = (Racer_Register)CachedTransform.GetComponent(typeof(Racer_Register));
            waypointContainer = RaceManager.Waypoints;
            GetWaypoints();

        }

        void Update()
        {
            CarSpeedKm = Mathf.Round(CachedTransform.GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
        }


        #region STUCKREMOVAL

        /// <summary>
        /// Receives waypint info from waypoint container
        /// </summary>
        private static void GetWaypoints()
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
        public static void RecoverCar()
        {
			Debug.Log ("recover car");
			if (RaceRegister.IsRacerStarted && !RaceRegister.IsRacerFinished && !RaceRegister.IsRacerDestroyed )
            {
                Resetcar();
                Debug.Log("Resetting Player Car");
            }

            if (layerChangeStarted)
            {
                layerChangeForIgnoreProcessor();
            }
        }
      
        private static void Resetcar()
        {
            Transform[] WaypointArray = waypoints.ToArray();
            Transform ClosestWP = GetClosestWP(WaypointArray, CachedTransform.position);
            Vector3 RelativeWPPosition = CachedTransform.InverseTransformPoint(ClosestWP.transform.position);

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
                nearestPoint = NearestPoint(ClosestWP.position, WaypointArray[nextWPIndex].position, CachedTransform.position);
            }
            else
            {
                nearestPoint = WaypointArray[0].position;
            }


            //Debug.DrawLine(CachedTransform.position, nearestPoint);


            CachedTransform.rotation = Quaternion.LookRotation(ClosestWP.forward);
            CachedTransform.position = nearestPoint;
            CachedTransform.position += Vector3.up * 0.1f;
            CachedTransform.position += Vector3.right * 0.1f;
            CachedTransform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            CachedTransform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
         

            layerChangeTimer = layerChangeTimerValue;
            currentLayerCache = LayerMask.LayerToName(CachedTransform.gameObject.layer);
            ChangeLayersRecursively(CachedTransform.transform, "IGNORE");

            layerChangeStarted = true;

        }
        //Ignore Collision stuff
        static string  currentLayerCache;
        static float layerChangeTimerValue = 7;
        static float  layerChangeTimer = 0;
        static bool layerChangeStarted = false;

        private static void layerChangeForIgnoreProcessor()
        {
            layerChangeTimer -= Time.deltaTime;

            if (layerChangeTimer <= 0)
            {
                ChangeLayersRecursively(CachedTransform.transform, currentLayerCache);
                layerChangeTimer = layerChangeTimerValue;
            }
        }

        static void ChangeLayersRecursively(Transform trans, String LayerName)
        {
            for (int i = 0; i < trans.childCount; i++)
            {
                trans.GetChild(i).gameObject.layer = LayerMask.NameToLayer(LayerName);
                ChangeLayersRecursively(trans.GetChild(i), LayerName);
            }

        }

        static Transform GetClosestWP(Transform[] DPs, Vector3 myPosition)
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
