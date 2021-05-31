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
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/Deprecated/RGKCar AI Controller")]
    public class RGKCar_AIController : MonoBehaviour, IRGKCarController
    {

        private RGKCar_Setup CarSetup;
        private RGKCar_Engine CarEngine;
        protected Race_Manager RaceManager;
        protected Racer_Register RaceRegister;

        private RGKCar_Wheel[] CarWheels;

        private Transform[] brakeLightsObjects;
        private Transform[] headLightObjects;
        private Light[] headLightsLightComponent;
        private Light[] brakeLightsLightComponent;
        private float brakeLightIntensity = 0f;
        private bool isHeadlightsOn = false;

        public Transform centerOfMass;

        public float throttle;
        public float throttleInput;
        public float brake;
        public float steer;
        public float handbrake;

        public Boolean isReversing;
        public Boolean isBraking;
        public Boolean isPreviouslyReversed = false;

        // Initialize
        void Start()
        {
            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));

            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;
            CarWheels = CarSetup.Wheels;


//            this.brakeLightsObjects = CarSetup.LightsData.BrakelightsLightObjects;
//            this.headLightObjects = CarSetup.LightsData.HeadlightsLightObjects;
//            this.headLightsLightComponent = CarSetup.LightsData.HeadlightsLights;
//            this.brakeLightsLightComponent = CarSetup.LightsData.BrakelightsLights;


            foreach (Transform brakeLightObject in brakeLightsObjects)
            {

                Material[] brakeMaterial = new Material[1]; brakeMaterial[0] = new Material(brakeLightObject.GetComponent<Renderer>().sharedMaterial);
                brakeLightObject.GetComponent<Renderer>().materials = brakeMaterial;
            }
            foreach (Transform headLightObject in headLightObjects)
            {
                Material[] headlightMaterial = new Material[1]; headlightMaterial[0] = new Material(headLightObject.GetComponent<Renderer>().sharedMaterial);
                headLightObject.GetComponent<Renderer>().materials = headlightMaterial;
            }



            if (centerOfMass != null)
            {
                GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
            }

            GetComponent<Rigidbody>().inertiaTensor *= CarSetup.CarInteria;

            // RGK_Start();
        }

        void Update()
        {
            isHeadlightsOn = CarSetup.LightsData.LightsOn;

            brake = Mathf.Clamp01(brake);



            //BRAKELIGHTS=============================================
            if (brake > 0)
            {
                foreach (Transform lBrake in brakeLightsObjects)
                {
                    foreach (Material oMat in lBrake.GetComponent<Renderer>().materials)
                    {
                        oMat.SetFloat("_Intensity", Mathf.Lerp(0, 3, 2));
                    }
                }

                foreach (Light bLight in brakeLightsLightComponent)
                {
                    bLight.enabled = true;
                }

            }
            else
            {

                foreach (Transform lBrake in brakeLightsObjects)
                {
                    foreach (Material oMat in lBrake.GetComponent<Renderer>().materials)
                    {
                        oMat.SetFloat("_Intensity", brakeLightIntensity);
                    }
                }
                foreach (Light bLight in brakeLightsLightComponent)
                {
                    bLight.enabled = false;
                }
            }

            //HEADLIGHS============================================


            if (this.isHeadlightsOn)
            {
                foreach (Light hLight in headLightsLightComponent)
                {
                    hLight.enabled = true;
                }
                foreach (Transform hLight in headLightObjects)
                {
                    foreach (Material oMat in hLight.GetComponent<Renderer>().materials)
                    {
                        oMat.SetFloat("_Intensity", 3f);
                    }
                }
                brakeLightIntensity = CarSetup.LightsData.DefaultBrakeLightIntensity;

            }
            else
            {
                foreach (Light hLight in headLightsLightComponent)
                {

                    hLight.enabled = false;
                }
                foreach (Transform hLight in headLightObjects)
                {
                    foreach (Material oMat in hLight.GetComponent<Renderer>().materials)
                    {
                        oMat.SetFloat("_Intensity", 0f);
                    }
                }
                brakeLightIntensity = 0f;

            }
            //HEADLIGHS=============================================


            // Apply inputs
            CarEngine.steer = steer;
            CarEngine.brake = brake;
            CarEngine.handbrake = handbrake;

        }

        // Debug GUI. Disable when not needed.
        void OnGUI()
        {
            //GUI.Label(new Rect(0, 20, 100, 200), "T " + throttle);
            //GUI.Label(new Rect(0, 30, 100, 200), "B " + brake);
            //GUI.Label(new Rect(0, 60, 100, 200), "S " + steering);
            //tractionControl = GUI.Toggle(new Rect(0, 80, 300, 20), tractionControl, "Traction Control (bypassed by shift key)");
        }

        public float Speed
        {
            get { return CarEngine.SpeedAsKM; }
        }

        public float Rpm
        {
            get { return CarEngine.RPM; }
        }

        public Boolean IsReversing
        {
            set { isReversing = value; }
        }
        public Boolean IsPreviouslyReversed
        {
            set { isPreviouslyReversed = value; }
        }
        public Boolean IsBraking
        {
            set { isBraking = value; }
        }


        public int Gear
        {
            get
            {
                return CarEngine.CurrentGear;
            }
            set
            {
                CarEngine.CurrentGear = value;
            }
        }

        public float MaxSteer
        {
            get { return 1; }
        }

        public void ApplyDrive(float Throttle, float Brake, bool HandBrake)
        {
            throttle = Mathf.Clamp(Throttle, -1f, 1.5f);
            throttleInput = Mathf.Clamp(throttle, -1f, 1.5f);
            brake = Brake;

            if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished && !RaceRegister.IsRacerDestroyed)
            {
                handbrake = 0;
                CarSetup.EngineData.Automatic = true;
            }
            else if (RaceRegister.IsRacerFinished && !RaceManager.AiContinuesAfterFinish && !RaceRegister.IsRacerDestroyed && !RaceManager.PlayerContinuesAfterFinish)
            {
                steer = 0;
                throttle = 0f;
                handbrake = 0.5f;
                CarSetup.EngineData.Automatic = true;
                CarEngine.CurrentGear = 1;
				//Debug.Log ();
            }
            else if (RaceManager.IsRaceStarted && RaceRegister.IsRacerDestroyed)
            {
                steer = UnityEngine.Random.Range(-1f, 1f);
                throttle = 0;
                handbrake = 1f;
                CarSetup.EngineData.Automatic = false;
                CarEngine.CurrentGear = 1;
            }
            else if (!RaceManager.IsRaceStarted)
            {
                steer = 0;
                throttle = 0.8f;
                CarSetup.EngineData.Automatic = false;
                CarEngine.CurrentGear = 1;
                handbrake = 1f;
            }

            CarEngine.throttle = throttle;
            //CarEngine.throttleInput = throttleInput;


            if (isReversing)
            {
                isPreviouslyReversed = true;
                CarEngine.CurrentGear = 0;
                CarEngine.throttle = Brake;
                // CarEngine.throttleInput = Brake;
                brake = 0;
            }

            if (isPreviouslyReversed && !isReversing)
            {
                CarEngine.CurrentGear = 2;
                isPreviouslyReversed = false;
            }

            if (isBraking)
            {
                CarEngine.brake = brake;
            }
        }

        public void ApplySteer(float Steer)
        {
            steer = Steer;
        }

        public void ShiftGears()
        {
            //throw new NotImplementedException();
        }
    }
}