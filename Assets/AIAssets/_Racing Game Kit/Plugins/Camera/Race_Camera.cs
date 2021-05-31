//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Game Camera Script
// Last Change : 03/09/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Interfaces;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Camera/RGK Rame Camera")]
    public class Race_Camera : MonoBehaviour, IRGKCamera
    {
        public Transform target;
        private Transform target_cache;
        private Camera CameraComponent;

        private List<Transform> targetObjects;
        public bool DisableTargetChange = false;
//        public int LockFPSTo = 60;

        private bool isStartupAnimationEnabled = false;
        private int currentCountDown = 0;
        private int currentTarget = 0;

        private List<RGK_CameraPositionHelper> CameraPositions;
        private RGK_CameraPositionHelper BackCameraPosition;
        public int CurrentCameraPosition = -1;
        private bool PositionChanging = false;

        public OutsideCameraSettings GameCameraSettings;
        public ShakeSettingsData ShakeSettings;
        public DynamicFoVSettings DynamicFOVSettings;
        public StartUpAnimation CamAnimations;

        private float NewFov = 0;
        private float SpeedAs;
        private float FovDif = 0;
        private float ActualFov;
		private float PrevFov;
        private float currentHighSpeedShake;

        private bool showBackView = false;
        private bool showBackViewFromMethod = false;
        private bool showLeftView = false;
        private bool showRightView = false;


        private float tempShake = 0;
		private float obstacleShake = 0;
        bool camWillChange = false;
		private Transform Dummy;
		bool value = true;

        /// <summary>
        /// Start
        /// </summary>
        [DoNotObfuscate()]
        void Start()
        {
//            if (LockFPSTo > 0)
//            {
//                Application.targetFrameRate = LockFPSTo;
//            }
            CameraComponent = (Camera)transform.GetComponent(typeof(Camera));
            if (target != null)
            {
                target_cache = target;
				Dummy = target;
                SetCameraPoints();
            }
            else
            {
                Debug.LogWarning("Race Camera Target is not assigned!");
            
            }

            
        }
        /// <summary>
        /// LateUpdate
        /// </summary>
        [DoNotObfuscate()]
        void LateUpdate()
        {
            if (target_cache != target && target != null)
            {
                target_cache = target;
                SetCameraPoints();

            }

            if (target_cache != null)
            {
                ProcessCameraAction();
            }

        }
        void Update()
        {
            if (target_cache != null) ProcessFoV();

			if (Constants.isBombEffectEnable) {
			
				target = null;
				target_cache = null;
			} else {
			
				target = Dummy;
				target_cache = Dummy;
				ProcessCameraAction ();
			}
        }

        private void ProcessFoV()
        {
            try
            {
                if (DynamicFOVSettings.Enabled)
                {
                    if (DynamicFOVSettings.StartSpeed < 1) DynamicFOVSettings.StartSpeed = 1;

                    SpeedAs = target_cache.parent.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
                    FovDif = DynamicFOVSettings.MaxFov - DynamicFOVSettings.MinFov;

                    if (CurrentCameraPosition == -1 && SpeedAs >= DynamicFOVSettings.StartSpeed)
                    {
                        NewFov = DynamicFOVSettings.ActionFovEffectCurve.Evaluate((SpeedAs - DynamicFOVSettings.StartSpeed) / DynamicFOVSettings.MaxSpeed);
                        ActualFov = DynamicFOVSettings.MinFov + FovDif * NewFov;
                        CameraComponent.fieldOfView = ActualFov;
                    }

					if(CameraComponent.fieldOfView >= (DynamicFOVSettings.MinFov) && SpeedAs< DynamicFOVSettings.StartSpeed ){
						PrevFov = CameraComponent.fieldOfView;
						ActualFov = Mathf.Lerp(PrevFov,DynamicFOVSettings.MinFov,Time.deltaTime *10f);

						CameraComponent.fieldOfView = ActualFov;

//						NewFov = DynamicFOVSettings.ActionFovEffectCurve.Evaluate((SpeedAs - DynamicFOVSettings.StartSpeed) / DynamicFOVSettings.MaxSpeed);
//						ActualFov = DynamicFOVSettings.MaxFov - FovDif * NewFov;
//						CameraComponent.fov = ActualFov;
					}
                }
            }
            catch
            {
                // Removed uncessery warning
                // Debug.Log("DynamicFOVSettings Disabled!");
                DynamicFOVSettings.Enabled = false;
            }
        }

        /// <summary>
        /// Receives camera locations from target prefab
        /// </summary>
        private void SetCameraPoints()
        {

            if (target_cache != null)
            {
                Transform oCameraPointObject = target_cache.parent.Find("_CameraLocations");

                if (oCameraPointObject != null)
                {
                    CameraPositions = new List<RGK_CameraPositionHelper>();

                    Component[] oCamPoints = oCameraPointObject.GetComponentsInChildren<RGK_CameraPositionHelper>();


                    foreach (Component oCamPoint in oCamPoints)
                    {
                        RGK_CameraPositionHelper oPosition = (RGK_CameraPositionHelper)oCamPoint;

                        if (!oPosition.BackCamera)
                        {
                            CameraPositions.Add(oPosition);
                        }
                        else
                        {
                            BackCameraPosition = oPosition;
                        }
                    }
                }
            }
            else
            {
                CurrentCameraPosition = -1;
            }
        }

        
        /// <summary>
        /// This function provides options for camera and target switch
        /// </summary>
        private void ProcessCameraAction()
        {
            
             //if (Input.GetButtonDown("CameraBack"))showBackView = true; 
            //if (Input.GetButtonUp("CameraBack")) showBackView = false;
           //if (Input.GetButtonDown("CameraLeft")) showLeftView=true;
            //if (Input.GetButtonUp("CameraLeft")) showLeftView=false;
           // if (Input.GetButtonDown("CameraRight")) showRightView=true;
           // if (Input.GetButtonUp("CameraRight")) showRightView=false;

            if (target != null)
            {
				
				float currentCamAngle = target.eulerAngles.y + GameCameraSettings.CameraAngle;
                float currentCamPosition = target.position.y + GameCameraSettings.Height;
                float currentCamAngleFrom = base.transform.eulerAngles.y;
                float positionFrom = base.transform.position.y;
                currentCamAngleFrom = Mathf.LerpAngle(currentCamAngleFrom, currentCamAngle, GameCameraSettings.RotationDamping * Time.deltaTime);
                positionFrom = Mathf.Lerp(positionFrom, currentCamPosition, GameCameraSettings.HeightDamping * Time.deltaTime);
                Quaternion RotateTo = Quaternion.Euler(0f, currentCamAngleFrom, 0f);
                Vector3 MoveTo = target.position - ((Vector3)((RotateTo * Vector3.forward) * GameCameraSettings.Distance));
                MoveTo.y = positionFrom;
                base.transform.position = MoveTo;
                
               // base.transform.LookAt(target);
                base.transform.eulerAngles = LookAt.targetRotation;

             //   base.transform.eulerAngles = new Vector3()
				//ShakeSettings.ShakeOnObstacleCollision = shake
                if (ShakeSettings.ShakeOnNitro)
                {
                    Vector3 rigidBodyVelocity = this.target_cache.parent.GetComponent<Rigidbody>().velocity;
                    float SpeedAsKM = this.target_cache.parent.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;

                    if (SpeedAsKM > ShakeSettings.ShakeStartSpeed)
                    {
                       
                        float zpe = Mathf.Clamp01(this.target_cache.parent.GetComponent<Rigidbody>().velocity.magnitude / (ShakeSettings.ShakeStartSpeed / 1.5f));
                        this.currentHighSpeedShake -= (this.currentHighSpeedShake - zpe) * 0.005f;
                        if (currentHighSpeedShake > (ShakeSettings.MaxShake / 10f)) currentHighSpeedShake = (ShakeSettings.MaxShake / 10f);
                        this.transform.Rotate((Vector3)((Random.onUnitSphere * this.currentHighSpeedShake) * 0.5f));
                    }
                    else
                    {
                        currentHighSpeedShake = 0;
						ShakeSettings.ShakeOnNitro = false;

                        
                    }
//					Debug.Log("Shaking Camera:"+SpeedAsKM);
				} 
//				else
//				{
//					ShakeSettings.ShakeOnNitro = false;	
//				}
				
				if (ShakeSettings.ShakeOnStart)
				{
					Vector3 rigidBodyVelocity = this.target_cache.parent.GetComponent<Rigidbody>().velocity;
                    float SpeedAsKM = this.target_cache.parent.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
//					Debug.Log("Speed"+SpeedAsKM);
                    if (SpeedAsKM > 3)
                    {
                        if (tempShake >= 0)
                        {
                            if (ShakeSettings.ShakeFadoutRes==0) ShakeSettings.ShakeFadoutRes = 1;
                            tempShake -= Time.deltaTime/ShakeSettings.ShakeFadoutRes;
                            if (tempShake > ShakeSettings.ShakeFrom / 10) tempShake = ShakeSettings.ShakeFrom/10;
                            this.transform.Rotate((Vector3)((Random.onUnitSphere * this.tempShake) * 0.5f));
                        }
                    }
                    else
                    {
                        tempShake = ShakeSettings.ShakeFrom/10;
                    }
//					Debug.Log("Shaking Camera:"+tempShake);

                }

				if (ShakeSettings.ShakeOnObstacleCollision)
				{
					Vector3 rigidBodyVelocity = this.target_cache.parent.GetComponent<Rigidbody>().velocity;
					float SpeedAsKM = this.target_cache.parent.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
//					Debug.Log("Speed"+SpeedAsKM);

					if (obstacleShake >= 0)
						{
						if (ShakeSettings.ObstacleShakeFadeOutRes==0) ShakeSettings.ObstacleShakeFadeOutRes = 1;
						obstacleShake -= Time.deltaTime/ShakeSettings.ObstacleShakeFadeOutRes;
						if (obstacleShake > ShakeSettings.ObstacleShakeFrom / 10) obstacleShake = ShakeSettings.ObstacleShakeFrom/10;
						this.transform.Rotate((Vector3)((Random.onUnitSphere * this.obstacleShake) * 0.5f));

					}else
					{
						obstacleShake = ShakeSettings.ObstacleShakeFrom/10;
						ShakeSettings.ShakeOnObstacleCollision = false;
					}

//					Debug.Log("Shaking Camera:"+obstacleShake);
					
				}

            }
            else { }
				//Debug.Log ("target " + target.parent.parent.gameObject.name);


             
		
//			if (!DisableTargetChange && Input.GetButtonDown("CameraTargetChange"))
//            {  
//				
//				#if UNITY_EDITOR
//
//				#elif !UNITY_EDITOR
//				if (targetObjects.Count > 0)
//                {
//                    target = targetObjects[currentTarget];
//                    target_cache = target;
//                    currentTarget++;
//                    if (currentTarget == targetObjects.Count) currentTarget = 0;
//                    CurrentCameraPosition = -1;
//                    SetCameraPoints();
//                }
//				#endif
//
//            }


            if (CurrentCameraPosition == -1)
            {
                base.transform.parent = null;
                if (target == null && target_cache != null)
                {
                   // target = target_cache;
                }

                if (!isStartupAnimationEnabled)
                {
                    if (showLeftView)
                    {
                        GameCameraSettings.CameraAngle = -90;
                    }
                    if (showRightView)
                    {
                        GameCameraSettings.CameraAngle = 90;
                    }
                    if (showBackView)
                    {
                        GameCameraSettings.CameraAngle = 180;
                    }
                    if (showBackViewFromMethod)
                    {
                        GameCameraSettings.CameraAngle = 180;
                    }
                    if (!showBackView && !showRightView && !showLeftView && !showBackViewFromMethod)
                    {
                        GameCameraSettings.CameraAngle = 0;
                    }
                }
                else
                {
                    switch (currentCountDown)
                    {
                        case 3:
                            GameCameraSettings.CameraAngle = CamAnimations.Second3.CameraAngle;
                            GameCameraSettings.Height = CamAnimations.Second3.CameraHeight;
                            GameCameraSettings.Distance = CamAnimations.Second3.CameraDistance;
                            break;
                        case 2:
                            GameCameraSettings.CameraAngle = CamAnimations.Second2.CameraAngle;
                            GameCameraSettings.Height = CamAnimations.Second2.CameraHeight;
                            GameCameraSettings.Distance = CamAnimations.Second2.CameraDistance;
                            break;
                        case 1:
                            GameCameraSettings.CameraAngle = CamAnimations.Second1.CameraAngle;
                            GameCameraSettings.Height = CamAnimations.Second1.CameraHeight;
                            GameCameraSettings.Distance = CamAnimations.Second1.CameraDistance;
                            break;
                        case 0:
                            GameCameraSettings.CameraAngle = CamAnimations.Second0.CameraAngle;
                            GameCameraSettings.Height = CamAnimations.Second0.CameraHeight;
                            GameCameraSettings.Distance = CamAnimations.Second0.CameraDistance;
                            break;
                        default:
                            GameCameraSettings.CameraAngle = CamAnimations.Second3.CameraAngle;
                            GameCameraSettings.Height = CamAnimations.Second3.CameraHeight;
                            GameCameraSettings.Distance = CamAnimations.Second3.CameraDistance;
                            break;
                    }
                }
            }
            else
            {
                if (showBackView)
                {
                    PositionChanging = true;

                    if (PositionChanging)
                    {
                        base.transform.parent = BackCameraPosition.transform.parent;
                        base.transform.position = BackCameraPosition.transform.position;
                        base.transform.rotation = BackCameraPosition.transform.rotation;
                        CameraComponent.fieldOfView = BackCameraPosition.FieldOfValue;
                        PositionChanging = false;
                    }
                }
                if (!showBackView)
                {
                    PositionChanging = true;

                    if (PositionChanging)
                    {
                        base.transform.parent = CameraPositions[CurrentCameraPosition].transform.parent;
                        base.transform.position = CameraPositions[CurrentCameraPosition].transform.position;
                        base.transform.rotation = CameraPositions[CurrentCameraPosition].transform.rotation;
                        CameraComponent.fieldOfView = CameraPositions[CurrentCameraPosition].FieldOfValue;
                        PositionChanging = false;
                    }
                }
            }

//            if (Input.GetButtonDown("CameraChange"))
//            {
//                camWillChange = true;
//            }

            if (camWillChange)
            {
                camWillChange = false;
                if (CurrentCameraPosition == (CameraPositions.Count - 1))
                {
                    CurrentCameraPosition = -1;
                }
                else
                {

                    CurrentCameraPosition++;
                    PositionChanging = true;

                    if (PositionChanging)
                    {
                        base.transform.parent = CameraPositions[CurrentCameraPosition].transform.parent;
                        base.transform.position = CameraPositions[CurrentCameraPosition].transform.position;
                        base.transform.rotation = CameraPositions[CurrentCameraPosition].transform.rotation;
                        CameraComponent.fieldOfView = CameraPositions[CurrentCameraPosition].FieldOfValue;
                        target = null;
                        PositionChanging = false;
                    }
                }
            }
            showBackViewFromMethod = false;
        }

        public void ChangeCamera()
        {
            camWillChange = true;
            ProcessCameraAction();
        }

        public void ShowBackView()
        {
            showBackViewFromMethod = true;
        }
        /// <summary>
        /// Public setter of target Transform for RaceManagerConnection that implemented from iRGKCamera
        /// </summary>
        public Transform TargetVehicle
        {
            set { target = value; }
        }
        /// <summary>
        /// public setter of targetObjects for RaceManagerConnection that implemented from iRGKCamera
        /// </summary>
        public List<Transform> TargetObjects
        {
            set { targetObjects = value; }
            get { return targetObjects; }
        }
        /// <summary>
        /// public setter of  IsStartupAnimationEnabled for RaceManagerConnection that implemented from iRGKCamera
        /// </summary>
        public bool IsStartupAnimationEnabled
        {
            set { isStartupAnimationEnabled = value; }
        }

		/// <summary>
		/// public setter of  IsShakeCameraOnHitObstcle for Collision with Obstacles
		/// </summary>
		public bool IsShakeCameraOnHitObstacle
		{
			set { 
				//IsShakeCameraOnHitObstacle = value;
				ShakeSettings.ShakeOnObstacleCollision = value;
			
			}
		}

		public bool IsShakeCameraOnNitro
		{
			set { 
				//IsShakeCameraOnHitObstacle = value;
				ShakeSettings.ShakeOnNitro = value;
				
			}
		}
        /// <summary>
        /// public setter of CurrentCount for RaceManagerConnection that implemented from iRGKCamera
        /// </summary>
        public int CurrentCount
        {
            set { currentCountDown = value; }
        }

        #region NestedClasses
        [System.Serializable]
        public class StarUpCameraAnimationValues : System.Object
        {
            public float CameraAngle = 0;
            public float CameraHeight = 1.24f;
			public float CameraDistance = 4.68f;
		}
		
        [System.Serializable]
        public class StartUpAnimation : System.Object
        {
            public StarUpCameraAnimationValues Second3;
            public StarUpCameraAnimationValues Second2;
            public StarUpCameraAnimationValues Second1;
            public StarUpCameraAnimationValues Second0;
        }
        [System.Serializable]
        public class OutsideCameraSettings : System.Object
        {
            public float Distance = 9;
            public float Height = 1.5f;
            public float HeightDamping = 3.5f;
            public float RotationDamping = 2;
            public float CameraAngle = 0;
        }

        [DoNotObfuscate()]
        [System.Serializable]
        public class DynamicFoVSettings : System.Object
        {
            public bool Enabled = true;
            public float StartSpeed = 20;
            public float MaxSpeed = 250;
            public float MinFov = 60;
            public float MaxFov = 80;
            public AnimationCurve ActionFovEffectCurve;
        }

        [System.Serializable]
        public class ShakeSettingsData : System.Object
        {

            public bool ShakeOnStart = false;
            public float ShakeFrom = 2.5f;
            public float ShakeFadoutRes = 10f;

            public bool ShakeOnNitro = false;
            public float MaxShake = 2.5f;
            public float ShakeStartSpeed = 170;

			public bool ShakeOnObstacleCollision = false;
			public float ObstacleShakeFrom = 2.5f;
			public float ObstacleShakeFadeOutRes = 10f;

        }


        #endregion


    }
}