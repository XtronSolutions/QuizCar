//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Controller Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar
{

    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/Deprecated/RGKCar Free Drive")]
    public class RGKCar_ControllerFreeDrive : MonoBehaviour
    {

        private RGKCar_Setup CarSetup;
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

        RGKCar_Engine CarEngine;


        public float shiftSpeed = 0.8f;

        // How long it takes to fully engage the throttle
        public float throttleTime = 1.0f;
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

        // Initialize
        void Start()
        {
            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;

            CarWheels = CarSetup.Wheels;

            if (centerOfMass != null)
            { GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition; }

            GetComponent<Rigidbody>().inertiaTensor *= CarSetup.CarInteria;

            this.brakeLightsTextures = CarSetup.LightsData.BrakelightsLightObjects;
            this.headLightTextures = CarSetup.LightsData.HeadlightsLightObjects;
            this.headLightsLightObjects = CarSetup.LightsData.HeadlightsLights;
            this.brakeLightsLightObject = CarSetup.LightsData.BrakelightsLights;
            isHeadlightsOn = CarSetup.LightsData.LightsOn;
        }

        void Update()
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
            if (Input.GetKey(KeyCode.LeftArrow))
                steerInput = -1;
            if (Input.GetKey(KeyCode.RightArrow))
                steerInput = 1;

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

            bool accelKey = Input.GetKey(KeyCode.UpArrow);
            bool brakeKey = Input.GetKey(KeyCode.DownArrow);

            if (CarSetup.EngineData.Automatic && CarEngine.CurrentGear == 0)
            {
                accelKey = Input.GetKey(KeyCode.DownArrow);
                brakeKey = Input.GetKey(KeyCode.UpArrow);
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                throttle += Time.deltaTime / throttleTime;
                throttleInput += Time.deltaTime / throttleTime;
            }
            else if (accelKey)
            {

                if (CarEngine.slipRatio < 0.10f)
                    throttle += Time.deltaTime / throttleTime;
                else if (!tractionControl)
                    throttle += Time.deltaTime / throttleTimeTraction;
                else
                    //throttle -= Time.deltaTime / throttleReleaseTime;
                    throttle = 0;

                if (throttleInput < 0)
                    throttleInput = 0;
                throttleInput += Time.deltaTime / throttleTime;
                brake = 0;

            }
            else
            {
                if (CarEngine.slipRatio < 0.2f)
                    throttle -= Time.deltaTime / throttleReleaseTime;
                else
                    throttle -= Time.deltaTime / throttleReleaseTimeTraction;
            }

            throttle = Mathf.Clamp01(throttle);

            if (brakeKey)
            {
                if (CarEngine.slipRatio < 0.2f)
                    brake += Time.deltaTime / throttleTime;
                else
                    brake += Time.deltaTime / throttleTimeTraction;
                throttle = 0;
                throttleInput -= Time.deltaTime / throttleTime;
            }
            else
            {
                if (CarEngine.slipRatio < 0.2f)
                    brake -= Time.deltaTime / throttleReleaseTime;
                else
                    brake -= Time.deltaTime / throttleReleaseTimeTraction;
            }



            brake = Mathf.Clamp01(brake);
            throttleInput = Mathf.Clamp(throttleInput, -1, 1);

            // Handbrake
            if (Input.GetKey(KeyCode.Space))
            {
                handbrake = 1;
            }
            else
            {
                handbrake = 0;
            }

            // Gear shifting
            //float shiftThrottleFactor = Mathf.Clamp01((Time.time - lastShiftTime) / shiftSpeed);


            CarEngine.throttle = throttle;// *shiftThrottleFactor;
           // CarEngine.throttleInput = throttleInput;
            CarEngine.brake = brake;

            if (Input.GetKeyDown(KeyCode.A))
            {
                lastShiftTime = Time.time;
                CarEngine.ShiftUp();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                lastShiftTime = Time.time;
                CarEngine.ShiftDown();
            }

             

            // Apply inputs
            //foreach (RGKCar_Wheel w in CarWheels)
            //{
            //    w.brake = brake;
            //    w.handbrake = handbrake;
            //    w.steering = steering;
            //}
            CarEngine.brake = brake;
            CarEngine.steer = steering;
            CarEngine.handbrake = handbrake;

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
        // Debug GUI. Disable when not needed.
        //void OnGUI()
        //{
        //    GUI.Label(new Rect(0, 60, 100, 200), "km/h: " + rigidbody.velocity.magnitude * 3.6f);
        //    tractionControl = GUI.Toggle(new Rect(0, 80, 300, 20), tractionControl, "Traction Control (bypassed by shift key)");
        //}
    }
}