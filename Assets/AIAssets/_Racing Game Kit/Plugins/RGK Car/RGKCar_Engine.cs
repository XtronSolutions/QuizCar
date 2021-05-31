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

    // This class simulates a car's engine and drivetrain, generating
    // torque, and applying the torque to the wheels.
    [RequireComponent(typeof(RGKCar_Setup))]
    [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Engine")]
    public class RGKCar_Engine : MonoBehaviour
    {
        private RGKCar_Setup CarSetup;
        private RGKCar_CarAudioAdvanced CarAudio;


        private Vector3 engineOrientation = Vector3.right;

        // inputs
        // engine throttle
        public float throttle = 0;
        // engine throttle without traction control (used for automatic gear shifting)
        //public float throttleInput = 0;

        public float brake = 0;
        public float handbrake = 0;
        public float steer;
        // state
        [HideInInspector]
        public int CurrentGear = 1;
        public int Gear = 0;
        public String GearAuto = "D";

        public Boolean isClutch = false;
        private float clutchInteral = 0;
        private int gearToChange = 0;

        public float RPM;
        public float SpeedAsKM;
		public float _SpeedAsKM { get { return SpeedAsKM; } }
        public float SpeedAsMile;
        public float TurboFill = 0;
        private float TurboDiff = 0;

        public float slipRatio = 0.0f;
        public float engineAngularVelo;

        public float totalSlip = 0.0f;

        private bool isShifting = false;
        private float ShiftInternalCounter = 1;
        private bool isBackfire = false;
        private bool isBackfireBlocked = false;
        private float BackfireBlockingInternal = 0;
        private float BackfireInternalCounter;
        private bool isBackfireAudioPlaying = false;
        public float limitedTimeEngineVelo = 0;

        float Sqr(float x) { return x * x; }

		Rigidbody myRigid;
        void Start()
        {
            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarAudio = GetComponent(typeof(RGKCar_CarAudioAdvanced)) as RGKCar_CarAudioAdvanced;
			myRigid = transform.GetComponent<Rigidbody> ();
			DefinedAngularDrag = myRigid.angularDrag;
			DefinedForwardDrag = myRigid.drag;
            //PushHelpOrig = PushHelp;
        }

        // Calculate engine torque for current rpm and throttle values.
        float CalcEngineTorque()
        {
            float result;
			if (RPM < CarSetup.EngineData.EngineTorqueRPM) 
			{
				result = CarSetup.EngineData.EngineMaxTorque * (-Sqr (RPM / CarSetup.EngineData.EngineTorqueRPM - 1) + 1);
			}
            else
            {
                float maxPowerTorque = (CarSetup.EngineData.EngineMaxPowerKw * 1000) / (CarSetup.EngineData.EnginePowerRPM * 2 * Mathf.PI / 60);
                float aproxFactor = (CarSetup.EngineData.EngineMaxTorque - maxPowerTorque) /
                    (2 * CarSetup.EngineData.EngineTorqueRPM * CarSetup.EngineData.EnginePowerRPM - Sqr(CarSetup.EngineData.EnginePowerRPM) - Sqr(CarSetup.EngineData.EngineTorqueRPM));
                float torque = aproxFactor * Sqr(RPM - CarSetup.EngineData.EngineTorqueRPM) + CarSetup.EngineData.EngineMaxTorque;
                result = torque > 0 ? torque : 0;
            }
            if (RPM > CarSetup.EngineData.EngineMaxRPM)
            {
                result *= 1 - ((RPM - CarSetup.EngineData.EngineMaxRPM) * 0.006f);
                if (result < 0)
                    result = 0;
            }
            if (RPM < 0)
                result = 0;
            return result;
        }
        //public float PushHelp = 0;
        //private float PushHelpOrig = 0;
        //public float PushAmount = 0;

        public float WaitForReverse = 2f;
        float lockingTorque;

        void FixedUpdate()
        {
            float intThrottle = 0;
            if (isClutch)
            {
                CurrentGear = 1;
                intThrottle = 0;
                //throttleInput = 0;
            }
            else
            {
                intThrottle = throttle;
            }


            float ratio = CarSetup.EngineData.Gears[CurrentGear] * CarSetup.EngineData.GearFinalRatio;
            float inertia = CarSetup.EngineData.EngineInteria * Sqr(ratio);
            float engineFrictionTorque = CarSetup.EngineData.EngineFriction + RPM * CarSetup.EngineData.EngineRPMFriction;

            float engineTorque = 0;
            Boolean PoweredWheelsOnGround = true;

            if (!isClutch && PoweredWheelsOnGround)
            {  
				
//					Debug.Log ("engineTorque       " + engineTorque);

				engineTorque =(CalcEngineTorque() + Mathf.Abs(engineFrictionTorque)) * intThrottle;
				
            }

            slipRatio = 0.0f;

            if (ratio == 0)
            {
                // Neutral gear - just rev up engine
                float engineAngularAcceleration = (engineTorque - engineFrictionTorque) / CarSetup.EngineData.EngineInteria;
                engineAngularVelo += engineAngularAcceleration * Time.deltaTime * 2;

                // Apply torque to car body
				myRigid.AddTorque(-engineOrientation * engineTorque);
            }
            else
            {
                int iPoweredCount = 0;

                foreach (RGKCar_Wheel RGKWheel in CarSetup.Wheels)
                {
                    if (RGKWheel.isPowered) iPoweredCount++;
                }
                float drivetrainFraction = 0.8f / iPoweredCount;
                float averageAngularVelo = 0;

                foreach (RGKCar_Wheel RGKWheel in CarSetup.Wheels)
                {
                    if (RGKWheel.isPowered) averageAngularVelo += RGKWheel.angularVelocity * drivetrainFraction;
                }


                float PoweredWheelsOnGroundCount = 0;

                foreach (RGKCar_Wheel RGKWheel in CarSetup.Wheels)
                {
                    if (RGKWheel.isPowered)
                    {
                        if (!isClutch)
                        {
                            if (RGKWheel.angularVelocity * CarSetup.EngineData.GearFinalRatio *
                                CarSetup.EngineData.Gears[CurrentGear] * (25 / Mathf.PI) > CarSetup.EngineData.EngineMaxRPM)
                            {
                                RGKWheel.angularVelocity = CarSetup.EngineData.EngineMaxRPM /
                                    (CarSetup.EngineData.GearFinalRatio * CarSetup.EngineData.Gears[CurrentGear] * (25 / Mathf.PI));
                            }

                            if (CarSetup.EngineData.LimitEngineSpeed && SpeedAsKM >= CarSetup.EngineData.LimitSpeedTo)
                            {
                                if (RGKWheel.angularVelocity * CarSetup.EngineData.GearFinalRatio *
                               CarSetup.EngineData.Gears[CurrentGear] * (25 / Mathf.PI) > limitedTimeEngineVelo)
                                {
                                    RGKWheel.angularVelocity = limitedTimeEngineVelo /
                                        (CarSetup.EngineData.GearFinalRatio * CarSetup.EngineData.Gears[CurrentGear] * (25 / Mathf.PI));
                                }
                            }

                            lockingTorque = (averageAngularVelo - RGKWheel.angularVelocity) * CarSetup.EngineData.DifferentialLock;
                            RGKWheel.drivetrainInertia = inertia * drivetrainFraction;
                            RGKWheel.driveFrictionTorque = engineFrictionTorque * Mathf.Abs(ratio) * drivetrainFraction;
                            RGKWheel.driveTorque = engineTorque * ratio * drivetrainFraction + lockingTorque;
                        }

                        slipRatio += RGKWheel.slipRatio * drivetrainFraction;


                        if (RGKWheel.onGround)
                        {
                            PoweredWheelsOnGroundCount++;
                        }
                    }
                }

                PoweredWheelsOnGround = (PoweredWheelsOnGroundCount == iPoweredCount) ? true : false;

                // update engine angular velo
                engineAngularVelo = averageAngularVelo * ratio;
            }

            // update state
            //slipRatio *= Mathf.Sign(ratio); // 1.2 Implementation

            //Limit engine speed based engine velocity
            if (CarSetup.EngineData.LimitEngineSpeed && SpeedAsKM >= CarSetup.EngineData.LimitSpeedTo)
            {
                RPM = limitedTimeEngineVelo;
            }
            else
            {
                RPM = engineAngularVelo * (60.0f / (2 * Mathf.PI));
                limitedTimeEngineVelo=RPM;
            }

            // very simple simulation of clutch - just pretend we are at a higher rpm.
            float minClutchRPM = CarSetup.EngineData.EngineMinRPM;

            if (CurrentGear == 2 || CurrentGear == 0)
            {
                minClutchRPM += intThrottle * UnityEngine.Random.Range(3000, 3250);


                //if (CurrentGear == 2 && PushHelp > 0 && throttle > 0)
                //{
                //    base.rigidbody.AddForce(base.rigidbody.transform.forward * PushAmount * Mathf.Clamp01(5), ForceMode.Acceleration);
                //    PushHelp -= Time.deltaTime * Mathf.Clamp01(5) * 2;
                //}
                //else if (CurrentGear == 0 && PushHelp > 0 && throttle > 0)
                //{
                //    base.rigidbody.AddForce(base.rigidbody.transform.forward * -1 * PushAmount * Mathf.Clamp01(5), ForceMode.Acceleration);
                //    PushHelp -= Time.deltaTime * Mathf.Clamp01(5) * 2;
                //}
                //else
                //{
                //    PushHelp = PushHelpOrig;
                //}

            }

            if (RPM < minClutchRPM)
            {
                RPM = minClutchRPM;
            }

            if (RPM > CarSetup.EngineData.EngineMaxRPM)
            {
                RPM = CarSetup.EngineData.EngineMaxRPM;
            }
            TurboDiff = CarSetup.EngineData.EnginePowerRPM - CarSetup.EngineData.EngineTorqueRPM;

            if (brake > 0) TurboFill = 0;
            if (intThrottle > 0) FillTurbo(RPM);

            float rpmWiggle = (CarSetup.EngineData.EngineMaxRPM * (0.5f + 0.5f * intThrottle));

            if (RPM >= rpmWiggle - 2000 && CurrentGear > 0 && intThrottle > 0)
            {
                RPM = RPM + UnityEngine.Random.Range(20, 100);
            }
            else if (RPM >= rpmWiggle - 1500 && CurrentGear > 0 && intThrottle > 0)
            {
                RPM = RPM + UnityEngine.Random.Range(100, 250);
            }
            else if ((RPM >= rpmWiggle - 1000 && CurrentGear > 0 && intThrottle > 0))
            {
                RPM = RPM + UnityEngine.Random.Range(50, 350);
            }


            // Automatic gear shifting. Bases shift points on throttle input and rpm.
            if (CarSetup.EngineData.Automatic)
            {
                if (!isClutch)
                {
                    if (CurrentGear == 2 && brake > 0 && SpeedAsKM < 10)
                    {
                        CurrentGear = 1;
                        ShiftDown();

                    }
                    else if (CurrentGear == 0 && brake > 0 && SpeedAsKM < 10)
                    {
                        CurrentGear = 1;
                        ShiftUp();
                    }


                    if (RPM >= (CarSetup.EngineData.GearUpRPM) && CurrentGear > 0 && intThrottle > 0 && CurrentGear < CarSetup.EngineData.Gears.Length - 1)
                    {

                        ShiftUp();

                        if (CarAudio != null && CurrentGear > 2)
                        {
                            CarAudio.PopTurbo(TurboFill);
                        }
                        TurboFill = 0;
                        isClutch = true;


                    }
                    //CarSetup.EngineData.EngineMaxRPM * (0.25f + 0.25f * throttle)
                    else if (RPM <= CarSetup.EngineData.GearDownRPM && CurrentGear > 2)
                    {
                        ShiftDown();

                        isClutch = true;
                    }

                    if (intThrottle < 0 && RPM <= CarSetup.EngineData.EngineMinRPM)
                    {
                        CurrentGear = (CurrentGear == 0 ? 2 : 0);
                    }

                }

                // ShiftDone();

            }



            //Correct the gear for GUI usage
            int iGear = CurrentGear - 1;
            Gear = iGear; // gear conversation to normal
            if (iGear == 0)
            {
                GearAuto = "N";
            }
            if (iGear > 0)
            {
                GearAuto = "D";
            }
            if (iGear < 0)
            {
                GearAuto = "R";
            }
			this.SpeedAsKM = myRigid.velocity.magnitude * 2f;
            this.SpeedAsMile = this.SpeedAsKM * 0.3214f;

            if (CarSetup.MiscFX.ExhaustBackfire != null)
            {
                if (brake > 0 && !isBackfire && !isBackfireBlocked && CurrentGear > 2)
                {
                    isBackfire = true;
                    BackfireInternalCounter = UnityEngine.Random.Range(0.1f, 0.6f);
                    if (!isBackfireBlocked) BackfireBlockingInternal = 0;
                }

                setBeckfire();
            }


            if (CarAudio != null)
            {
                if (brake > 0 && CurrentGear > 1)
                {
                    CarAudio.ApplyBrakeDisk(brake);
                }
                else
                {
                    CarAudio.ApplyBrakeDisk(0);
                }
            }

            foreach (RGKCar_Wheel CarWheel in CarSetup.Wheels)
            {
                CarWheel.brake = brake;
                CarWheel.steering = steer;
                CarWheel.handbrake = handbrake;

                if (CarWheel.isPowered && CarSetup.UseEarthFX)
                {
                    if (CarWheel.SurfaceSound != null)
                    {
                        if (surfacAaudioClipName != CarWheel.SurfaceSound.name)
                        {
                            surfacAaudioClipName = CarWheel.SurfaceSound.name;
                        }
                        else
                        {
                            if (CarAudio != null)
                            {
                                CarAudio.SurfaceSound = CarWheel.SurfaceSound;

                            }
                        }
                    }
                    else
                    {
                        surfacAaudioClipName = "";
                        if (CarAudio != null)
                        {
                            CarAudio.SurfaceSound = null;

                        }
                    }


                    if (CarWheel.BrakeSound != null)
                    {
                        if (brakeAudioClipName != CarWheel.BrakeSound.name)
                        {
                            brakeAudioClipName = CarWheel.BrakeSound.name;
                        }
                        else
                        {
                            if (CarAudio != null)
                            {
                                CarAudio.Skid = CarWheel.BrakeSound;
                                CarAudio.UpdateSkidSound();
                            }
                        }
                    }
                    else
                    {
                        brakeAudioClipName = "";

                        if (CarAudio != null)
                        {
                            CarAudio.Skid = null;
                            CarAudio.UpdateSkidSound();
                        }
                    }

                    if (CarWheel.SurfaceForwardDrag != 0)
                    {
                        if (SurfaceForwardDrag != CarWheel.SurfaceForwardDrag)
                        {
                            SurfaceForwardDrag = CarWheel.SurfaceForwardDrag;

                        }
                    }
                    else
                    {
                        SurfaceForwardDrag = 0;
                    }

                    if (CarWheel.SurfaceAngularDrag != 0)
                    {
                        if (SurfaceAngularDrag != CarWheel.SurfaceAngularDrag)
                        {
                            SurfaceAngularDrag = CarWheel.SurfaceAngularDrag;

                        }
                    }
                    else
                    {
                        SurfaceAngularDrag = 0;

                    }

                    ApplySurfaceDrag(SurfaceForwardDrag, SurfaceAngularDrag);
                }
            }

            if (isClutch) Clutch();
        }

        void ApplySurfaceDrag(float ForwardDrag, float AngularDrag)
        {
            if (ForwardDrag == 0) ForwardDrag = DefinedForwardDrag;
            if (AngularDrag == 0) AngularDrag = DefinedAngularDrag;

			myRigid.drag = ForwardDrag;
			myRigid.angularDrag = AngularDrag;
        }


        private string surfacAaudioClipName = "";
        private string brakeAudioClipName = "";
        private float DefinedForwardDrag = 0;
        private float DefinedAngularDrag = 0;
        private float SurfaceForwardDrag = 0;
        private float SurfaceAngularDrag = 0;


        private void Clutch()
        {
            if (isClutch)
            {
                clutchInteral -= Time.deltaTime;

                CurrentGear = 1;
                if (clutchInteral <= 0f)
                {
                    isClutch = false;
                    clutchInteral = CarSetup.EngineData.ClutchTime;

                    CurrentGear = gearToChange;

                }
            }
        }

        public void ShiftUp()
        {
            if (CurrentGear < CarSetup.EngineData.Gears.Length - 1)
            {
                if (!isClutch)
                {
                    clutchInteral = CarSetup.EngineData.ClutchTime;
                    gearToChange = CurrentGear + 1;
                    isClutch = true;
                    if (CarAudio != null)
                    { CarAudio.PopGear(); }
                }
            }
        }

        public void ShiftDown()
        {
            if (CurrentGear > 0)
            {
                if (!isClutch)
                {
                    clutchInteral = CarSetup.EngineData.ClutchTime;
                    gearToChange = CurrentGear - 1;
                    isClutch = true;
                    if (CarAudio != null)
                    { CarAudio.PopGear(); }
                }
            }
        }


        //void Update()
        //{
        //    foreach (RGKCar_Wheel CarWheel in CarSetup.Wheels)
        //    {
        //        CarWheel.brake = brake;
        //        CarWheel.steering = steer;
        //        CarWheel.handbrake = handbrake;
        //    }
        //}

        private void setBeckfire()
        {
            if (isBackfireBlocked)
            {
                BackfireBlockingInternal += Time.deltaTime;
                if (BackfireBlockingInternal >= CarSetup.MiscFX.BackfireBlockingSeconds) { BackfireBlockingInternal = 0; isBackfireBlocked = false; }
            }

            if (isBackfire)
            {
                BackfireInternalCounter -= Time.deltaTime;
                if (BackfireInternalCounter >= 0f)
                {
                    CarSetup.MiscFX.ExhaustBackfire.SetActiveRecursively(true);

                    if (CarAudio != null && !isBackfireAudioPlaying)
                    {
                        if (BackfireInternalCounter > CarSetup.MiscFX.BackfireSeconds)
                            isBackfireAudioPlaying = CarAudio.PopBackfire(true);
                        else
                            isBackfireAudioPlaying = CarAudio.PopBackfire(false);
                    }
                    isBackfireBlocked = true;

                }
                else
                {
                    isBackfire = false;
                    isBackfireAudioPlaying = false;
                    resetBackFire();
                }
            }
            else
            {
                resetBackFire();
            }
        }

        private void resetBackFire()
        {

            if (CarSetup.MiscFX.ExhaustBackfire.active) CarSetup.MiscFX.ExhaustBackfire.SetActiveRecursively(false);
        }

        private void FillTurbo(float eRPM)
        {

            if (eRPM >= CarSetup.EngineData.EngineTorqueRPM && eRPM <= CarSetup.EngineData.EnginePowerRPM)
            {
                TurboFill += (((RPM * TurboDiff) / CarSetup.EngineData.EnginePowerRPM) / TurboDiff) * Time.deltaTime;
                if (TurboFill > 1) TurboFill = 1;
            }
        }

        public void ShiftDone()
        {
            if (isShifting)
            {
                ShiftInternalCounter -= Time.deltaTime;
                if (ShiftInternalCounter < 0f)
                {
                    isShifting = false;
                    ShiftInternalCounter = 1;
                }
            }
        }
        public float TotalSlip
        {
            get
            {
                totalSlip = 0;
                if (CarSetup != null)
                {
                    foreach (RGKCar_Wheel CarWheels in CarSetup.Wheels)
                        totalSlip += CarWheels.slipVelo / CarSetup.Wheels.Length;
                }
                return totalSlip;
            }
        }
    }
}