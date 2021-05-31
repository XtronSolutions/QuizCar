//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Controller Script
// Last Change : 04/11/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar.CarControllers
{

    [AddComponentMenu("")]
    public class RGKCar_C2_ControllerControllerBase : MonoBehaviour
    {
        [HideInInspector]
        public RGKCar_Setup CarSetup;
        [HideInInspector]
        public RGKCar_Wheel[] CarWheels;
        [HideInInspector]
        public RGKCar_Engine CarEngine;



        public float throttleInput;
        public float steerInput;

        public float brake;
        public float throttle;
        public float steer;
        public int gear;

        public float handbrake;
        public bool ThrottlePressed;
        public bool BrakePressed;


        // How long it takes to fully engage the throttle
        public float throttleSpeed = 0.5f;
        // How long it takes to fully release the throttle
        public float throttleReleaseSpeed = 0.5f;
        // How long it takes to fully engage the throttle 
        // when the wheels are spinning (and traction control is disabled)
        public float throttleSpeedOnVel = 5f;
        // How long it takes to fully release the throttle 
        // when the wheels are spinning.
        public float throttleReleaseSpeedOnVel = 0.1f;



        // These values determine how fast steering value is changed when the steering keys are pressed or released.
        // Getting these right is important to make the car controllable, as keyboard input does not allow analogue input.

        // How long it takes to fully turn the steering wheel from center to full lock
        public float steerSpeed = 1.2f;
        // This is added to steerTime per m/s of velocity, so steering is slower when the car is moving faster.
        // How long it takes to fully turn the steering wheel from full lock to center
        public float steerReleaseSpeed = 0.6f;

        public float steerSpeedOnVel = 0.1f;
        // This is added to steerReleaseTime per m/s of velocity, so steering is slower when the car is moving faster.
        public float steerReleaseSpeedOnVel = 0f;
        // When detecting a situation where the player tries to counter steer to correct an oversteer situation,
        // steering speed will be multiplied by the difference between optimal and current steering times this 
        // factor, to make the correction easier.
        public float steerCorrectionFactor = 4.0f;
        // Turn traction control on or off
        public bool tractionControl = true;
        // Used by SoundController to get average slip velo of all wheels for skid sounds.


        private Transform[] brakeLightsTextures;
        private Transform[] headLightTextures;
        private Transform[] reverseLightTextures;

        private Light[] headLightsLightObjects;
        private Light[] brakeLightsLightObject;
        private Light[] reverseLightsLightObject;

        private float brakeLightIntensity = 0f;
        public bool isHeadlightsOn = false;
        private bool isBrakeLightsOn = false;
        private bool isReverseLightsOn = false;

        private bool headLightOldState = false;
        private bool brakeLightOldstate = false;
        private bool reverseLightOldState = false;
        protected bool smoothThrottle = true;
        protected bool smoothSteering = true;

        // Initialize
        protected void Start()
        {
            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;

            CarWheels = CarSetup.Wheels;



            this.brakeLightsTextures = CarSetup.LightsData.BrakelightsLightObjects;
            this.headLightTextures = CarSetup.LightsData.HeadlightsLightObjects;
            this.reverseLightTextures = CarSetup.LightsData.ReverseLightLightObjects;

            this.headLightsLightObjects = CarSetup.LightsData.HeadlightsLights;
            this.brakeLightsLightObject = CarSetup.LightsData.BrakelightsLights;
            this.reverseLightsLightObject = CarSetup.LightsData.ReverselightLights;

            isHeadlightsOn = CarSetup.LightsData.LightsOn;
            headLightOldState = isHeadlightsOn;


        }

        protected void Update()
        {
            DoUpdate();
        }

        protected void DoUpdate()
        {

            // Steering
			//Vector3 carDir = transform.forward;
			Vector3 carDir = transform.right;

            float fVelo = GetComponent<Rigidbody>().velocity.magnitude;
            Vector3 veloDir = GetComponent<Rigidbody>().velocity * (1 / fVelo);
            float angle = -Mathf.Asin(Mathf.Clamp(Vector3.Cross(veloDir, carDir).y, -1, 1));
            float optimalSteering = angle / (CarWheels[0].maxSteeringAngle * Mathf.Deg2Rad);
            if (fVelo < 1)
                optimalSteering = 0;


            if (smoothSteering)
            {
                if (steerInput < 0) steerInput = -1;
                if (steerInput > 0) steerInput = 1;

                if (steerInput < steer)
                {
                    float steeringSpeed = (steer > 0) ? (1 / (steerReleaseSpeed + steerReleaseSpeedOnVel * fVelo)) : (1 / (steerSpeed + steerSpeedOnVel * fVelo));
                    if (steer > optimalSteering)
                        steeringSpeed *= 1 + (steer - optimalSteering) * steerCorrectionFactor;
                    steer -= steeringSpeed * Time.deltaTime;
                    if (steerInput > steer)
                        steer = steerInput;
                }
                else if (steerInput > steer)
                {
                    float steeringSpeed = (steer < 0) ? (1 / (steerReleaseSpeed + steerReleaseSpeedOnVel * fVelo)) : (1 / (steerSpeed + steerSpeedOnVel * fVelo));
                    if (steer < optimalSteering)
                        steeringSpeed *= 1 + (optimalSteering - steer) * steerCorrectionFactor;
                    steer += steeringSpeed * Time.deltaTime;
                    if (steerInput < steer)
                        steer = steerInput;
                }
            }
            else
            {
                steer = steerInput;
            }

            if (smoothThrottle)
            {
                if (ThrottlePressed)
                {
                    if (tractionControl)
                    { throttle += Time.deltaTime / throttleSpeed; }
                    else
                    { throttle = 1; }
                    brake = 0;
                }
                else
                {
                    if (CarEngine.slipRatio < 0.2f)
                    {
                        throttle -= Time.deltaTime / throttleReleaseSpeed;
                    }
                    else
                    {
                        throttle -= Time.deltaTime / throttleReleaseSpeedOnVel;
                    }

                }

                throttle = Mathf.Clamp01(throttle);

                if (BrakePressed)
                {
                    if (CarEngine.slipRatio < 0.2f)
                        brake += Time.deltaTime / throttleSpeed;
                    else
                        brake += Time.deltaTime / throttleSpeedOnVel;
                    //throttleInput -= Time.deltaTime / throttleApplySpeed;
                    throttle = 0;
                }
                else
                {
                    if (CarEngine.slipRatio < 0.2f)
                        brake -= Time.deltaTime / throttleReleaseSpeed;
                    else
                        brake -= Time.deltaTime / throttleReleaseSpeedOnVel;

                }

                brake = Mathf.Clamp01(brake);
            }
            else
            {
                if (throttleInput < 0) { brake = Mathf.Abs(throttleInput); throttle = 0; }
                if (throttleInput > 0) { throttle = throttleInput; brake = 0; }
                if (throttleInput == 0) { throttle = 0; brake = 0; }
               
            }


            FeedEngine();
            ProcessLights();
        }

        public void FeedEngine()
        {
            CarEngine.throttle = throttle;
            //CarEngine.CurrentGear = gear;
            CarEngine.brake = brake;
            CarEngine.steer = steer;
            CarEngine.handbrake = handbrake;
        }

        public void ProcessLights()
        {
            //BRAKELIGHTS=============================================
            if (brake > 0)
            {
                isBrakeLightsOn = true;
            }
            else
            {
                isBrakeLightsOn = false;
            }

            if (isBrakeLightsOn != brakeLightOldstate)
            {
                if (isBrakeLightsOn)
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
                brakeLightOldstate = isBrakeLightsOn;
            }

            //HEADLIGHS=============================================


            CarSetup.LightsData.LightsOn = isHeadlightsOn;

            if (isHeadlightsOn != headLightOldState)
            {
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
                headLightOldState = isHeadlightsOn;
            }
            //REVERSE LIGHTS=============================================

            if (CarEngine.Gear == -1)
            {
                isReverseLightsOn = true;
            }
            else
            {
                isReverseLightsOn = false;
            }

            if (isReverseLightsOn != reverseLightOldState)
            {
                if (this.isReverseLightsOn)
                {
                    foreach (Light rLight in reverseLightsLightObject)
                    {
                        rLight.enabled = true;
                    }
                    foreach (Transform rLight in reverseLightTextures)
                    {
                        if (rLight != null)
                        {
                            foreach (Material oMat in rLight.GetComponent<Renderer>().materials)
                            {
                                oMat.SetFloat("_Intensity", 3f);
                            }
                        }
                    }

                }
                else
                {
                    foreach (Light rLight in reverseLightsLightObject)
                    {

                        rLight.enabled = false;
                    }
                    foreach (Transform rLight in reverseLightTextures)
                    {
                        if (rLight != null)
                        {
                            foreach (Material oMat in rLight.GetComponent<Renderer>().materials)
                            {
                                oMat.SetFloat("_Intensity", 0f);
                            }
                        }
                    }


                }
                reverseLightOldState = isReverseLightsOn;
            }
        }

    }
}