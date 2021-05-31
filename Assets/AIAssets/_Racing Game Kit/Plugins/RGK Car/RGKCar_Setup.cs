//============================================================================================
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

namespace RacingGameKit.RGKCar
{
    [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Setup")]
    public class RGKCar_Setup : MonoBehaviour
    {
        private Rigidbody rigidBody;
        public bool SetupMode = false;
        public bool RigidbodySleepOnAwake = true;
        
        public float CarMass = 1500f;
        public float CarInteria = 1.5f;
        public Transform CenterOfMass;

        public DriveEnum Drive = DriveEnum.RWD;
        public RGKCar_Wheel[] Wheels;
        public WheelDataBase WheelData;
        public EngineDataBase EngineData;
        public LightsDataBase LightsData;
        public AeroDynamicsBase AeroDynamicsData;
        public FXDataBase MiscFX;
        private EarthFX EarthFX;
        public bool UseEarthFX;
        public string SkidmarkObjectName;
        private GameObject SkidMarks;

        [System.Serializable]
        public class EngineDataBase : System.Object
        {
            public float EngineMinRPM = 4000;
            public float EngineMaxRPM = 8000;
            public float GearDownRPM = 4000;
            public float GearUpRPM = 7000;
            public float ClutchTime = 0.15f;
            public float EngineMaxTorque = 700;
            public float EngineTorqueRPM = 4000;
            public float EngineMaxPowerKw = 220;
            public float EnginePowerRPM = 6500;
            public float EngineInteria = 0.5f;
            public float EngineFriction = 25f;
            public float EngineRPMFriction = 0.04f;
            public bool  LimitEngineSpeed = false;
            public float LimitSpeedTo = 30f;

            public float[] Gears = { -4f, 0, 3.89f, 2.25f, 1.76f, 1.33f, 1f,0.75f };
            public float GearFinalRatio = 5.74f;
			public bool Automatic = true;
            public float DifferentialLock = 10f;
        }

        [System.Serializable]
        public class WheelDataBase : System.Object
        {
            public float FrontBrakeTorque = 2000;
            public float RearBrakeTorque = 3500;
            public float HandBrakeTorque = 10000;

            public float MaxSteeringAngle = 30;
            public float FrontWheelGrip = 1.0f;
            public float FrontWheelSideGrip = 1.0f;
            public float RearWheelGrip = 1.2f;
            public float RearWheelSideGrip = 1.0f;

            
            public float FrontWheelRadius = 0.35f;
            public float RearWheelRadius = 0.35f;
            public float WheelInteria = 1f;
            public float FrictionTorque = 10f;
            public float MassFriction = 0.25f;
            

            public float SuspensionHeight = 0.1f;
            public float SuspensionStiffness = 5000;
            public float SuspensionReleaseCoef = 50;
            public Transform WheelBase;
            public float WheelBaseAlignment = 0f;

            public float BlurSwtichVelocity = 20f;
            /// <summary>
            /// nexyt version
            /// </summary>
//            private bool BurnoutStart = false;
//            private float BurnoutStartDuration = 1f;

            public bool ShowForces = false;
        }

        [System.Serializable]
        public class AeroDynamicsBase : System.Object
        {
            public bool EnableWing = true;
            public float WingDownforceCoef = -1f;
            public bool EnableAirFriction = true;
            public Vector3 AirFrictionCoef = new Vector3(1.0f, 1.0f, 0.05f);
        }

        [System.Serializable]
        public class LightsDataBase : System.Object
        {
            public Transform[] HeadlightsLightObjects;
            public Light[] HeadlightsLights;
            public Transform[] BrakelightsLightObjects;
            public Light[] BrakelightsLights;
            public Transform[] ReverseLightLightObjects;
            public Light[] ReverselightLights;

            public float DefaultBrakeLightIntensity = 0.7f;
            public bool LightsOn = false;
        }

        [System.Serializable]
        public class FXDataBase : System.Object
        {
            public GameObject ExhaustBackfire;
            public float BackfireSeconds = 0.5f;
            public float BackfireBlockingSeconds = 5;
            public GameObject NosFire;

        }

        void Awake()
        {
            this.rigidBody = transform.GetComponent<Rigidbody>();
            if (RigidbodySleepOnAwake)
            {
                GetComponent<Rigidbody>().Sleep();
            }
        }

        void Start()
        {

            if (CenterOfMass != null)
            { 
                GetComponent<Rigidbody>().centerOfMass = CenterOfMass.localPosition; 
            }
          
            if (WheelData.WheelBase != null)
            {
                WheelData.WheelBase.localPosition = new Vector3(0, WheelData.WheelBaseAlignment);
            }

            if (EarthFX == null && UseEarthFX)
            {
                //EarthFX
                GameObject oEFX = GameObject.Find("_EarthFX");

                if (oEFX != null)
                {
                    EarthFX = oEFX.GetComponent(typeof(EarthFX)) as EarthFX;
                }
            }
            else
            {
                if (SkidmarkObjectName != "")
                {
                    SkidMarks = GameObject.Find(SkidmarkObjectName);
                }
            }

            //Update Vehicle Config with default values;
            updateVehicleConfig();
        }

        void FixedUpdate()
        {
            if (SetupMode)
            {
//                updateVehicleConfig();
            }
        }

        void updateVehicleConfig()
        {
            

            if (CarMass<0.5f) CarMass=0.5f;
            GetComponent<Rigidbody>().mass = CarMass;

            if (CarInteria < 0.01f) CarInteria = 0.01f;
            GetComponent<Rigidbody>().inertiaTensor *= CarInteria;


            foreach (RGKCar_Wheel RGKWheel in Wheels)
            {
                if (RGKWheel.WheelLocation == WheelLocationEnum.Front)
                {
                    if (Drive == DriveEnum.FWD || Drive == DriveEnum.AWD) RGKWheel.isPowered = true;
                    RGKWheel.maxSteeringAngle = WheelData.MaxSteeringAngle;
                    RGKWheel.suspensionHeight = WheelData.SuspensionHeight;
                    RGKWheel.suspensionStiffness = WheelData.SuspensionStiffness;
                    RGKWheel.brakeTorque = WheelData.FrontBrakeTorque;
                    RGKWheel.handbrakeTorque = WheelData.HandBrakeTorque / 10;
                    RGKWheel.definedGrip = WheelData.FrontWheelGrip;
                    RGKWheel.definedSideGrip= WheelData.FrontWheelSideGrip;
                    RGKWheel.radius = WheelData.FrontWheelRadius;
                    
                }

                if (RGKWheel.WheelLocation == WheelLocationEnum.Rear)
                {
                    if (Drive == DriveEnum.RWD || Drive == DriveEnum.AWD) RGKWheel.isPowered = true;
                    RGKWheel.maxSteeringAngle = 0;
                    RGKWheel.suspensionHeight = WheelData.SuspensionHeight;
                    RGKWheel.suspensionStiffness = WheelData.SuspensionStiffness;
                    RGKWheel.brakeTorque = WheelData.RearBrakeTorque;
                    RGKWheel.handbrakeTorque = WheelData.HandBrakeTorque;
                    RGKWheel.definedGrip = WheelData.RearWheelGrip;
                    RGKWheel.radius = WheelData.RearWheelRadius;
                    RGKWheel.definedSideGrip = WheelData.RearWheelSideGrip;
                }

                RGKWheel.suspensionRelease = WheelData.SuspensionReleaseCoef;
                RGKWheel.ShowForces = WheelData.ShowForces;
                RGKWheel.inertia = WheelData.WheelInteria;
                RGKWheel.frictionTorque = WheelData.FrictionTorque;
                RGKWheel.massFraction = WheelData.MassFriction;
                RGKWheel.blurSwitchVelocity = WheelData.BlurSwtichVelocity;
                //RGKWheel.BurnoutStart = WheelData.BurnoutStart;
                //RGKWheel.BurnoutStartDuration = WheelData.BurnoutStartDuration;
                RGKWheel.EarthFX = this.EarthFX;
                RGKWheel.UseEarthFX = this.UseEarthFX;
                RGKWheel.SkidmarkObject = SkidMarks;
            }



            if (rigidBody != null && AeroDynamicsData.EnableWing)
            {
                float wingDownEff = AeroDynamicsData.WingDownforceCoef * rigidBody.velocity.sqrMagnitude;
                rigidBody.AddForceAtPosition(wingDownEff * transform.up, transform.position);
            }

            if (rigidBody != null && AeroDynamicsData.EnableAirFriction)
            {
                Vector3 localVelo = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);

                // Strip signs.
                Vector3 absLocalVelo = new Vector3(Mathf.Abs(localVelo.x), Mathf.Abs(localVelo.y), Mathf.Abs(localVelo.z));

                // Calculate and apply aerodynamic force.
                Vector3 airResistance = Vector3.Scale(Vector3.Scale(localVelo, absLocalVelo), -2 * AeroDynamicsData.AirFrictionCoef);
                GetComponent<Rigidbody>().AddForce(transform.TransformDirection(airResistance));
            }
        }


        void OnDrawGizmosSelected()
        {
            if (Wheels != null)
            {
                Gizmos.color = Color.red;
                foreach (RGKCar_Wheel RGKWheel in Wheels)
                {
                    if (RGKWheel.WheelLocation == WheelLocationEnum.Front)
                    {
                        Gizmos.DrawWireSphere(RGKWheel.transform.position, WheelData.FrontWheelRadius);
                    }
                    else if (RGKWheel.WheelLocation == WheelLocationEnum.Rear)
                    {
                        
                        Gizmos.DrawWireSphere(RGKWheel.transform.position, WheelData.RearWheelRadius);
                    }
                }
                
            }
        }

    }
}