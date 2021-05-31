//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Controller Script
// Last Change : 12/09/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;
using System.Collections.Generic;

namespace RacingGameKit.RGKCar
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/Deprecated/RGKCar Human Controller")]
    public class RGKCar_HumanController : MonoBehaviour
    {
        private GameObject waypointContainer;
        private List<Transform> waypoints;

        private RGKCar_Setup CarSetup;
        private RGKCar_Engine CarEngine;
        protected Race_Manager RaceManager;
        protected Racer_Register RaceRegister;

        private RGKCar_Wheel[] CarWheels;
        public Transform centerOfMass;

        public string ControlsThrottleBinding = "Throttle";
        public string ControlsBrakeBinding = "Brake";
        public string ControlsSteeringBinding = "Horizontal";
        public string ControlsHandbrakeBinding = "Handbrake";
        public string ControlsShiftUpBinding = "ShiftUp";
        public string ControlsShiftDownBinding = "ShiftDown";
        public string ControlsHeadlightsBinding = "Lights";
        public string ControlsResetBinding = "ResetVehicle";



        float brake;
        float throttle;
        float throttleInput;
        float steering;
        float lastShiftTime = -1;
        float handbrake;




        public float shiftSpeed = 0.8f;

        // How long it takes to fully engage the throttle
        public float throttleSpeed = 1.0f;
        // How long it takes to fully engage the throttle 
        // when the wheels are spinning (and traction control is disabled)
        public float throttleTimeTraction = 10.0f;
        // How long it takes to fully release the throttle
        public float throttleReleaseTime = 0.5f;
        // How long it takes to fully release the throttle 
        // when the wheels are spinning.
        public float throttleReleaseTimeTraction = 0.1f;

        // Turn traction control on or off
        public bool tractionControl = true;


        // These values determine how fast steering value is changed when the steering keys are pressed or released.
        // Getting these right is important to make the car controllable, as keyboard input does not allow analogue input.

        // How long it takes to fully turn the steering wheel from center to full lock
        public float steerTime = 1.2f;
        // This is added to steerTime per m/s of velocity, so steering is slower when the car is moving faster.
        public float veloSteerTime = 0.1f;

        // How long it takes to fully turn the steering wheel from full lock to center
        public float steerReleaseTime = 0.6f;
        // This is added to steerReleaseTime per m/s of velocity, so steering is slower when the car is moving faster.
        public float veloSteerReleaseTime = 0f;
        // When detecting a situation where the player tries to counter steer to correct an oversteer situation,
        // steering speed will be multiplied by the difference between optimal and current steering times this 
        // factor, to make the correction easier.
        public float steerCorrectionFactor = 4.0f;

        // Used by SoundController to get average slip velo of all wheels for skid sounds.
     
        private Transform[] brakeLightsTextures;
        private Transform[] headLightTextures;
        private Light[] headLightsLightObjects;
        private Light[] brakeLightsLightObject;
        private float brakeLightIntensity = 0f;
        private bool isHeadlightsOn = false;
        private float throttleBonus = 1f;
        private bool IsFirstGearShifted = false;
        // Initialize
        void Start()
        {
            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));

            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;

            CarWheels = CarSetup.Wheels;

            if (centerOfMass != null)
            {
                GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            }
            GetComponent<Rigidbody>().inertiaTensor *= CarSetup.CarInteria;

            this.brakeLightsTextures = CarSetup.LightsData.BrakelightsLightObjects;
            this.headLightTextures = CarSetup.LightsData.HeadlightsLightObjects;
            this.headLightsLightObjects = CarSetup.LightsData.HeadlightsLights;
            this.brakeLightsLightObject = CarSetup.LightsData.BrakelightsLights;
            isHeadlightsOn = CarSetup.LightsData.LightsOn;

            waypointContainer = RaceManager.Waypoints;
            GetWaypoints();

        }


        void Update()
        {
            DoUpdate();
        }


        void DoUpdate()
        {

            // Steering
            Vector3 carDir = transform.forward;
            float fVelo = GetComponent<Rigidbody>().velocity.magnitude;
            Vector3 veloDir = GetComponent<Rigidbody>().velocity * (1 / fVelo);
            float angle = -Mathf.Asin(Mathf.Clamp(Vector3.Cross(veloDir, carDir).y, -1, 1));
            float optimalSteering = angle / (CarWheels[0].maxSteeringAngle * Mathf.Deg2Rad);
            if (fVelo < 1)
                optimalSteering = 0;

            float steerInput = 0;

            steerInput = Input.GetAxisRaw(ControlsSteeringBinding);

            if (steerInput < 0) steerInput = -1;
            if (steerInput > 0) steerInput = 1;

            if (steerInput < steering)
            {
                float steerSpeed = (steering > 0) ? (1 / (steerReleaseTime + veloSteerReleaseTime * fVelo)) : (1 / (steerTime + veloSteerTime * fVelo));
                if (steering > optimalSteering)
                    steerSpeed *= 1 + (steering - optimalSteering) * steerCorrectionFactor;
                steering -= steerSpeed * Time.deltaTime;
                if (steerInput > steering)
                    steering = steerInput;
            }
            else if (steerInput > steering)
            {
                float steerSpeed = (steering < 0) ? (1 / (steerReleaseTime + veloSteerReleaseTime * fVelo)) : (1 / (steerTime + veloSteerTime * fVelo));
                if (steering < optimalSteering)
                    steerSpeed *= 1 + (optimalSteering - steering) * steerCorrectionFactor;
                steering += steerSpeed * Time.deltaTime;
                if (steerInput < steering)
                    steering = steerInput;
            }

            // Throttle/Brake



            bool ThrottlePressed = (Input.GetAxisRaw(ControlsThrottleBinding) > 0) ? true : false;
            bool BrakePressed = (Input.GetAxisRaw(ControlsBrakeBinding) > 0) ? true : false;

            if (CarSetup.EngineData.Automatic && CarEngine.CurrentGear == 0)
            {
                ThrottlePressed = (Input.GetAxisRaw(ControlsBrakeBinding) > 0) ? true : false;
                BrakePressed = (Input.GetAxisRaw(ControlsThrottleBinding) > 0) ? true : false;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                throttle = 1;
                throttleInput = 1;
            }
            else if (ThrottlePressed)
            {
                if (throttleInput > 0)
                    throttle += Time.deltaTime / throttleSpeed;
                else if (!tractionControl)
                    throttle = 1;
                else
                    throttle -= Time.deltaTime / throttleReleaseTime;

                if (throttleInput < 0) throttleInput = 0;
                throttleInput += Time.deltaTime / throttleSpeed;
                throttle = 1;

            }
            else
            {
                if (CarEngine.slipRatio < 0.2f)
                {
                    throttle -= Time.deltaTime / throttleReleaseTime;
                    throttleInput -= Time.deltaTime / throttleReleaseTimeTraction;
                }
                else
                {
                    throttle -= Time.deltaTime / throttleReleaseTimeTraction;
                    throttleInput -= Time.deltaTime / throttleReleaseTimeTraction;
                }
            }

            throttle = Mathf.Clamp01(throttle);

            if (BrakePressed)
            {
                if (CarEngine.slipRatio < 0.2f)
                    brake += Time.deltaTime / throttleSpeed;
                else
                    brake += Time.deltaTime / throttleTimeTraction;
                throttleInput -= Time.deltaTime / throttleSpeed;
            }
            else
            {
                if (CarEngine.slipRatio < 0.2f)
                    brake -= Time.deltaTime / throttleReleaseTime;
                else
                    brake -= Time.deltaTime / throttleReleaseTimeTraction;
                brake = 0;
            }



            brake = Mathf.Clamp01(brake);
            throttleInput = Mathf.Clamp(throttleInput, -1, 1);

            // Handbrake
            if (Input.GetAxisRaw(ControlsHandbrakeBinding) > 0)
            {
                handbrake = 1;
            }
            else
            {
                handbrake = 0;
            }

            // Gear shifting
            float shiftThrottleFactor = Mathf.Clamp01((Time.time - lastShiftTime) / shiftSpeed);


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
                    lastShiftTime = Time.time;
                    CarEngine.ShiftUp();
                }
                if (Input.GetButtonDown(ControlsShiftDownBinding))
                {
                    lastShiftTime = Time.time;
                    CarEngine.ShiftDown();
                }
            }
            else if (RaceRegister.IsRacerFinished)
            {
                steering = 0;
                throttle = 0f;
                handbrake = 0.5f;
                CarSetup.EngineData.Automatic = true;
                CarEngine.CurrentGear = 1;
            }
            else
            {
                steering = 0;
                CarSetup.EngineData.Automatic = false;
                CarEngine.CurrentGear = 1;
                handbrake = 1f;
            }

            CarEngine.throttle = throttle * throttleBonus;
            //CarEngine.throttleInput = throttleInput * throttleBonus;
            CarEngine.brake = brake;



            

            // Apply inputs

            CarEngine.brake = brake;
            CarEngine.handbrake = handbrake;
            CarEngine.steer = steering;

            CheckIsCarStuck();
            ProcessLights();
        }

        void ProcessLights()
        {
            //BRAKELIGHTS=============================================
            if (brake > 0)
            {
                foreach (Transform lBrake in brakeLightsTextures)
                {
                    if (lBrake != null)
                    {
                        foreach (Material oMat in lBrake.GetComponent<Renderer>().materials)
                        {
                            oMat.SetFloat("_Intensity", Mathf.Lerp(0, 3, 2));
                        }
                    }
                }

                foreach (Light bLight in brakeLightsLightObject)
                {
                    bLight.enabled = true;
                }

            }
            else
            {

                foreach (Transform lBrake in brakeLightsTextures)
                {
                    if (lBrake != null)
                    {
                        foreach (Material oMat in lBrake.GetComponent<Renderer>().materials)
                        {
                            oMat.SetFloat("_Intensity", brakeLightIntensity);
                        }
                    }
                }

                foreach (Light bLight in brakeLightsLightObject)
                {
                    bLight.enabled = false;
                }
            }

            //HEADLIGHS=============================================
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
                CarSetup.LightsData.LightsOn = isHeadlightsOn;
            }

            if (this.isHeadlightsOn)
            {
                foreach (Light hLight in headLightsLightObjects)
                {
                    hLight.enabled = true;
                }
                foreach (Transform hLight in headLightTextures)
                {
                    if (hLight != null)
                    {
                        foreach (Material oMat in hLight.GetComponent<Renderer>().materials)
                        {
                            oMat.SetFloat("_Intensity", 3f);
                        }
                    }
                }
                brakeLightIntensity = CarSetup.LightsData.DefaultBrakeLightIntensity;

            }
            else
            {
                foreach (Light hLight in headLightsLightObjects)
                {

                    hLight.enabled = false;
                }
                foreach (Transform hLight in headLightTextures)
                {
                    if (hLight != null)
                    {
                        foreach (Material oMat in hLight.GetComponent<Renderer>().materials)
                        {
                            oMat.SetFloat("_Intensity", 0f);
                        }
                    }
                }
                brakeLightIntensity = 0f;

            }
            //HEADLIGHS=============================================
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

       
           // Debug.DrawLine(transform.position, nearestPoint);

            
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

        Transform GetClosestWP(Transform[] DPs, Vector3 myPosition)
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