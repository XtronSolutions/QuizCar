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

namespace RacingGameKit.RGKCar.CarControllers
{
     
    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/Vehicle Controllers/RGKCar C2 - AI Controller")]
    public class RGKCar_C2_AI : RGKCar_C2_ControllerControllerBase, IRGKCarController
    {

        protected Race_Manager RaceManager;
        protected Racer_Register RaceRegister;

        public Boolean isReversing;
        public Boolean isBraking;
        public Boolean isPreviouslyReversed = false;
        private bool IsFirstGearShifted = false;

        // Initialize
        void Start()
        {
            GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
            RaceManager = (Race_Manager)GameManagerContainerGameObject.GetComponent(typeof(Race_Manager));
            RaceRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
            base.Start();

            InvokeRepeating("CheckIfStuck", 5, 5);
        }

        void Update()
        {
            /// do nothing here...
        }
        void CheckIfStuck()
        {
            isPreviouslyReversed = true;
            isReversing = false;
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
                gear= value;
            }
        }

        public float MaxSteer
        {
            get { return 1; }
        }

        public void ApplyDrive(float Throttle, float Brake, bool HandBrake)
        {
            if (RaceManager == null) return; 
            throttle = Mathf.Clamp(Throttle, -1f, 1.5f);
            //throttleInput = Mathf.Clamp(throttle, -1f, 1.5f);
            brake = Brake;

            if (RaceManager.IsRaceStarted && !RaceRegister.IsRacerFinished && !RaceRegister.IsRacerDestroyed)
            {
                handbrake = 0;
                CarSetup.EngineData.Automatic = true;
                
                if (!IsFirstGearShifted)
                {
                    handbrake = 0;
                    CarEngine.CurrentGear = 2;
                    IsFirstGearShifted = true;
                }
            }
            else if (RaceRegister.IsRacerFinished && !RaceManager.AiContinuesAfterFinish && !RaceRegister.IsRacerDestroyed && !RaceManager.PlayerContinuesAfterFinish)
            {
                steer = 0;
                throttle = 0f;
                brake = 0.5f;
                handbrake = 0.5f;
                CarSetup.EngineData.Automatic = true;
                gear = 1;
                CarEngine.CurrentGear = 1;
            }
            else if (RaceManager.IsRaceStarted && RaceRegister.IsRacerDestroyed)
            {
                steer = UnityEngine.Random.Range(-1f, 1f);
                throttle = 0;
                handbrake = 1f;
                CarSetup.EngineData.Automatic = false;
                gear = 1;
                CarEngine.CurrentGear = 1;
            }
            else if (!RaceManager.IsRaceStarted)
            {
                steer = 0;
                throttle = 0.8f;
                CarSetup.EngineData.Automatic = false;
                gear = 1;
                CarEngine.CurrentGear = 1;
                handbrake = 1f;
            }

            //CarEngine.throttle = throttle;
            //CarEngine.throttleInput = throttleInput;

            if(RaceRegister.IsRacerFinished)
            {
                brake = 1;
                steer = 1;
                handbrake = 1;
                IsBraking = true;
            }

            if (isReversing)
            {
                    isPreviouslyReversed = true;
                    gear = 0;
                    throttle = Brake;
                    //throttleInput = Brake;
                    brake = 0;
                    CarEngine.CurrentGear = 0;
            }

            if (isPreviouslyReversed && !isReversing)
            {
                gear = 2;
                CarEngine.CurrentGear = 2;
                isPreviouslyReversed = false;
            }

            if (isBraking)
            {
                CarEngine.brake = brake;
            }      
            FeedEngine();
            ProcessLights();
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