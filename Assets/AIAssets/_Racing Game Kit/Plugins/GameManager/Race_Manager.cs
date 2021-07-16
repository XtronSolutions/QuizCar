//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Race Manager Script
// Last Change : 12/18/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Helpers;
using RacingGameKit.Interfaces;
using SmartAssembly.Attributes;
using Photon.Pun;
using Photon.Realtime;

namespace RacingGameKit
{
    /// <summary>
    /// This class created for managing all game mechanics like wp, sp, cp management and also user registartion, race type and their options.
    /// </summary>
    [AddComponentMenu("Racing Game Kit/Game Mechanics/Race Manager"), RequireComponent(typeof(SplineInterpolator))]
    public class Race_Manager : MonoBehaviour
    {

        #region Members

		public bool isPlayerWrongway = false;

		public GameObject racer;

        /// <summary>
        /// Waypoints Container object
        /// </summary>
        [HideInInspector]
        [DoNotObfuscate]
        public GameObject Waypoints;
        /// <summary>
        /// SpawnPoints Container object
        /// </summary>
        [HideInInspector]
        [DoNotObfuscate]
        public GameObject SpawnPoints;
        /// <summary>
        /// Checkpoints Conateiner object. If empty checkpoint sytsem will be disabled
        /// </summary>
        [HideInInspector]
        [DoNotObfuscate]
        public GameObject CheckPoints;
       
        /// <summary>
        /// Start point of the race. Mandatory. 
        /// </summary>
        /// 
        [HideInInspector]
        [DoNotObfuscate]
        public GameObject StartPoint;
        /// <summary>
        /// Finish point of the race. If race type LAP or KNOCKOUT, must be same point as StartPoint. This is essential!
        /// </summary>
        [HideInInspector]
        [DoNotObfuscate]
        public GameObject FinishPoint;

        /// <summary>
        /// What Kind of game will run in this instance
        /// </summary>
        [DoNotObfuscate]
        [HideInInspector]
        public RaceTypeEnum RaceType; // Game Mode as Lap, Spirint or KnockOut
        /// <summary>
        /// Total Race Lalps - It should not lower then 1
        /// </summary>
        [HideInInspector]
        public int RaceLaps = 1;                //Race lap count 
        /// <summary>
        /// Total Racers will spawn to track (inlude player)
        /// </summary>
        [HideInInspector]
        public int RacePlayers = 10;          //Total racers will in the race
        /// <summary>
        /// Race Lenght from start to finish 
        /// </summary>
        [HideInInspector]
        public float RaceLength;            //Total Race Distance as Meters | This will calculated by waypoint distances
        /// <summary>
        /// Human Player prefab
        /// </summary>
        [HideInInspector]
        public GameObject HumanRacerPrefab;

        /// <summary>
        /// Player Spawn Position
        /// </summary>
        [HideInInspector]
        public int PlayerSpawnPosition = 3;
        /// <summary>
        /// AI Player prefabs
        /// </summary>
        [DoNotObfuscate]
        [HideInInspector]
        public GameObject[] AIRacerPrefab;


        /// <summary>
        /// Holds spawned AI data for blocking respawn
        /// </summary>
		private List<int> SpawnedAIs = new List<int>();

        private int LastSpawnedAIIndex = 0;
        /// <summary>
        /// What order AI will spawn on start 
        /// Random : Randomize AI positions on start
        /// Order : AI spawn from top to bottom
        /// </summary>
        public eAISpawnOrder AiSpawnOrder = eAISpawnOrder.Random;
        /// <summary>
        /// How AI will spawn on start
        /// </summary>
        public eAISpawnMode AiSpawnMode = eAISpawnMode.Random;
        /// <summary>
        /// How AI will named on start
        /// </summary>
        public eAINamingMode AiNamingMode = eAINamingMode.Random;
        /// <summary>
        /// AI names will assign randomly from json list or will assign in given order
        /// </summary>
        [Obsolete("This method is obsolate. Please use AINamingMode property instead!")]
        [HideInInspector]
        public bool RandomAiNames = false;
        /// <summary>
        /// If checked ai dont stop after race finished
        /// </summary>
        public bool AiContinuesAfterFinish = false;
        /// <summary>
        /// Overtake player controls with AI and continue racing on track. This is good for after race camaras
        /// </summary>
        public bool PlayerContinuesAfterFinish = false;
        /// <summary>
        /// Race finishes when player finish
        /// </summary>
        public bool StopRaceOnPlayerFinish = false;
        /// <summary>
        /// Kicks last racer after second last passed finish point. Useful for knockout
        /// </summary>
        public bool KickLastRacerAfter2ndLast = false;

        public bool MoveKickedRacerToIgnoreLayer = true;
        public bool MoveRespawnToIgnoreLayer = true;

        public bool IsRaceReady = false;

        public bool RaceStartsOnStartup = true;

        

        [NonSerialized]
        public int RaceActiveLap = 1;       //Current Lap player in it

        //Timings
        /// <summary>
        /// Time data for Total Race 
        /// </summary>
        public float TimeTotal = 0f;
        /// <summary>
        /// Time data for Active Lap 
        /// </summary>
        public float TimeCurrent = 0f;
        /// <summary>
        /// Time data for players' Last
        /// </summary>
        [HideInInspector]
        public float CurrentTimeFixForPlayer = 0;

        public float TimePlayerLast = 0f;
        /// <summary>
        /// Time data for players' best
        /// </summary>
        public float TimePlayerBest = 0f;
        /// <summary>
        /// Time data for next checkpoint
        /// </summary>
        public float TimeNextCheckPoint = 0f;
        /// <summary>
        /// Time data for total checkpoint
        /// </summary>
        public float TimeTotalCheckPoint = 0f;
        /// <summary>
        /// Timer count down from. 
        /// </summary>
        public int TimerCountdownFrom = 3;
        /// <summary>
        /// Indicator for is race started. All AI and player depends this.
        /// </summary>
        public bool IsRaceStarted = false;
        /// <summary>
        /// Indicator for is race end. All AI and player depends this.
        /// </summary>
        public bool IsRaceFinished = false;

        /// <summary>
        /// Indicator for checkpoint system. If checkpoint not implemented on track, this will be false.
        /// </summary>
        private bool EnableCheckPointSystem = false;

        public bool EnableStartupCamera = true;

        private bool tmpFinished = false;

        private bool isCounting = false;
        private float TimeStart = 0f;
        private float TimeTotalFix = 0f;
        [HideInInspector]
        public int CurrentCount;

        [HideInInspector]
        public IRGKUI GameUIComponent;

        [HideInInspector]
        public Race_Audio GameAudioComponent;
        [DoNotObfuscate]
        public Boolean StartMusicAfterCountdown = true;

        //Distance Objects
        public float DistancePointDensity = 90f;

        [HideInInspector]
        public GameObject DistanceTransformContainer;

        [DoNotObfuscate]
        private Transform[] DistanceMeasurementObjects;
        /// <summary>
        /// There's a another array that created for determining racer distances on track. If checked, This invisible array can be viewable.
        /// </summary>
        public bool ShowDistanceGizmos = false;
        /// <summary>
        /// Gamers
        /// </summary>
        [DoNotObfuscate]
        private List<Racer_Detail> ListRacers = new List<Racer_Detail>();

        /// <summary>
        /// AI racer names from JSon.
        /// </summary>
        [DoNotObfuscate]
        private String[] AIRacerNames;
        /// <summary>
        /// Racer names
        /// </summary>
        [DoNotObfuscate]
        private List<String> ListRacerNames;
        /// <summary>
        /// Contains All racer details data for reaching them later as in UI 
        /// </summary>
        [HideInInspector]
        public List<Racer_Detail> RegisteredRacers
        {
            get { return ListRacers; }
            set { ListRacers = value; }
        }
        /// <summary>
        /// Returns last racer ID for knockout type game. 
        /// </summary>
        [HideInInspector]
        public String LastRacerID;
        /// <summary>
        /// Returns last racer name
        /// </summary>
        [HideInInspector]
        public string LastRacerName;
        /// <summary>
        /// Indicator for is there any human player in race. (it's true when HumanPlayerPrefab assigned)
        /// </summary>
        private bool HumanDeployed = false;
        /// <summary>
        /// Camera Instance
        /// </summary>
        public RacingGameKit.Interfaces.IRGKCamera GameCamereComponent = null;

        private int PlayerListIndex = -1;
        /// <summary>
        /// Prevents multiple RaceFinished Message pushed
        /// </summary>
        private bool isRaceFinishPushed = false;

        
        [HideInInspector]
        public eSpeedTrapMode SpeedTrapMode = eSpeedTrapMode.HighestTotalSpeed;

        [HideInInspector]
        public bool EnableCheckpointArrow = true;

        [HideInInspector]
        public float WorkingFPS;

		public GameObject[] playerBuggies;

        #endregion

        #region PublicMethods
        /// <summary>
        /// Register Racer to game manager and return racer array index for querying racer stuff later
        /// </summary>
        /// <param name="GamerDetail"></param>
        /// <returns></returns>
        public int RegisterToGame(Racer_Detail GamerDetail)
        {
            int rIndex = -1;
            if (GamerDetail != null)
            {
                ListRacers.Add(GamerDetail);
                rIndex = ListRacers.FindIndex(_racerNew => _racerNew.ID == GamerDetail.ID);
            }

			this.GetComponent<HUD_Script> ().GetRaceDetail (ListRacers);
            return rIndex;
        }
        /// <summary>
        /// Get a name from AIRacerNameArray to AI racers.
        /// </summary>
        /// <returns></returns>
        public String GetNameForRacer(String NameOnRegisterComponent)
        {
            float Index = 0;
            String AIName = "";
            switch (AiNamingMode)
            {
                case eAINamingMode.Random:
                    Index = UnityEngine.Random.Range(0, ListRacerNames.Count);
                    if (Index > ListRacerNames.Count) Index = ListRacerNames.Count - 1;
                    AIName = ListRacerNames[Convert.ToInt16(Index)].ToString();
                    ListRacerNames.RemoveAt(Convert.ToInt16(Index));
                    break;
                case eAINamingMode.Order:
                    if (Index > ListRacerNames.Count) Index = ListRacerNames.Count - 1;
                    AIName = ListRacerNames[Convert.ToInt16(Index)].ToString();
                    ListRacerNames.RemoveAt(Convert.ToInt16(Index));
                    break;
                case eAINamingMode.FromRacerRegister:
                    AIName = NameOnRegisterComponent;
                    break;
            }
            return AIName;
        }

        /// <summary>
        /// Draw gizmos 
        /// </summary>
        [DoNotObfuscate]
        void OnDrawGizmos()
        {
            if (ShowDistanceGizmos)
            {
                if (DistanceMeasurementObjects != null)
                {
                    foreach (Transform DistancePoint in DistanceMeasurementObjects)
                    {
                        Gizmos.DrawIcon(DistancePoint.position, "icon_distancepoint.tif");
                    }
                }
            }
        }

        #endregion

        #region PrivateMethods

        /// <summary>
        /// Component Awake
        /// </summary>
        [DoNotObfuscate]
        private void Awake()
        {
            
			if (playerBuggies != null && playerBuggies.Length > 1 && !Constants.isMultiplayerSelected) {
				var index = RewardProperties.Instance.GetBuggySelected ();
			//	index = index > 1 ? 1 : index;
            //   index =3;
				HumanRacerPrefab = playerBuggies [index];
			}
			AIRacerNames = JsonUtils.GetRacerNames();

			if (Constants.isMultiplayerSelected) {
//				HumanRacerPrefab = player;
				this.RacePlayers = totalRacers;
				this.PlayerSpawnPosition = playerSpawnPosition;
                TimerCountdownFrom = 0;
//
//				if (enemies != null && enemies.Count > 0) {
//					this.AIRacerPrefab = new GameObject[enemies.Count];
//					this.RacePlayers = enemies.Count + 1;
//
//					for (int i = 0; i < enemies.Count; i++) {
//						this.AIRacerPrefab [i] = enemies [i];
//					}
//				}

			}

        }
        /// <summary>
        /// Component Start
        /// </summary>
        [DoNotObfuscate]
        private void Start()
        {
            if (MoveRespawnToIgnoreLayer)
            {
                if (LayerMask.NameToLayer("IGNORE") > 0 && LayerMask.NameToLayer("AI") > 0)
                {
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AI"), LayerMask.NameToLayer("IGNORE"), true);
                    Physics.IgnoreLayerCollision(LayerMask.NameToLayer("IGNORE"), LayerMask.NameToLayer("IGNORE"), true);
                }
                else
                {
                    Debug.LogWarning("RACING GAME KIT WARNING\r\nOne of more required layers (AI,IGNORE or OBSTACLE) not created. Please check documentation for required layers!");
                }
            }

            InitRace();
        }

        /// <summary>
        /// Setup game manager, links the components, enumarates racers and helps everything goes well on race
        /// </summary>
        private void InitRace()
        {
            if (RaceLaps < 1) RaceLaps = 1;
            if (RacePlayers < 1) RacePlayers = 1;

            //Reach to UI, we'll send countdown and show/hide countdown window
            GameUIComponent = (IRGKUI)transform.GetComponent(typeof(IRGKUI));

            //Reach GameAudio for playing stuff
            GameAudioComponent = (Race_Audio)transform.GetComponent(typeof(Race_Audio));
            if (GameAudioComponent != null)
            {
                GameAudioComponent.InitAudio();
            }
            else
            {
                Debug.LogWarning("RACING GAME KIT WARNING\r\nRace Audio component not found or disabled! Race Audio will not managed by RaceManager");
            }


            //Determinate GameCamera, this will attached to player car.
			GameObject oRGKCamera = GameObject.Find("_GameCamera");

            if (oRGKCamera != null)
            {
                GameCamereComponent = oRGKCamera.GetComponent(typeof(RacingGameKit.Interfaces.IRGKCamera)) as RacingGameKit.Interfaces.IRGKCamera;
                GameCamereComponent.IsStartupAnimationEnabled = EnableStartupCamera;
            }
            else
            {
                Debug.LogWarning(RGKMessages.GameCameraMissing);
                Debug.DebugBreak();
                return;
            }

            //Create racer names from name array. This is for AI racers
            ListRacerNames = new List<String>(AIRacerNames);

            ///Checkpoint System Enable
            if (CheckPoints == null || CheckPoints.transform.childCount == 0) // If Checkoints Implemented
            {
//                Debug.Log(RGKMessages.CheckpointSystemDisabled);
                EnableCheckPointSystem = false;
            }
            else
            {
                EnableCheckPointSystem = true;
                GetFirstCheckPoint();
            }

            //Determinate spawn points. 
            if (SpawnPoints != null)
            {
                Transform[] SPItems = GetChildTransforms(SpawnPoints.transform);

//				Debug.Log ("sp items===" + AIRacerPrefab.Length);

                if (GameCamereComponent.TargetObjects == null) GameCamereComponent.TargetObjects = new List<Transform>();

                if (PlayerSpawnPosition > RacePlayers) PlayerSpawnPosition = RacePlayers;

                bool DeployHumanNow = false;
                bool DeploymentComplete = false;

                for (int i = 0; i < SPItems.GetLength(0); i++)
                {
					
                    UnityEngine.GameObject iRacer = null;

                    if (HumanRacerPrefab != null && !HumanDeployed && (i + 1 == PlayerSpawnPosition))
                    {
                        DeployHumanNow = true;
                    }
                    else if (AIRacerPrefab.Length > 0 && !Constants.isMultiplayerSelected)
                    {
                        if (AIRacerPrefab[0] != null)
                        {
                            int AiIndexGoingToSpawn = 0;

                            if (AiSpawnOrder == eAISpawnOrder.Random)
                            {
                                if (AIRacerPrefab.Length > 1)
                                {
                                    if (AiSpawnMode == eAISpawnMode.Random)
                                    {
                                        AiIndexGoingToSpawn = UnityEngine.Random.Range(0, AIRacerPrefab.Length);
                                    }
                                    else if (AiSpawnMode == eAISpawnMode.OneTimeEach)
                                    {
//										Debug.Log ("one time each");
										AiIndexGoingToSpawn = FindNextNotSpawnedAiIndex();
                                        if (AiIndexGoingToSpawn != -1)
                                        {
                                            SpawnedAIs.Add(AiIndexGoingToSpawn);
//                                            Debug.Log("Spawn No " + AiIndexGoingToSpawn);
                                        }
                                        else
                                        {
//											Debug.Log ("else part");
											if (!HumanDeployed) DeployHumanNow = true;
                                            DeploymentComplete = true;
                                        }
                                    }

                                }
                            }
                            else if (AiSpawnOrder == eAISpawnOrder.Order)
                            {

                                AiIndexGoingToSpawn = LastSpawnedAIIndex;
                                LastSpawnedAIIndex += 1;
                                if (LastSpawnedAIIndex > AIRacerPrefab.Length)
                                {
                                    AiIndexGoingToSpawn = -1;
                                    DeploymentComplete = true;
                                }
                            }


                            if (AiIndexGoingToSpawn != -1)
                            {
								iRacer = (GameObject)Instantiate(AIRacerPrefab[AiIndexGoingToSpawn], SPItems[i].transform.position,SPItems[i].transform.rotation);
//								Debug.Log ("i racer===" + SPItems[i].transform.rotation);
                                iRacer.GetType();//Debug Warning Removal;

                                Transform CameraTargetTransform = ((GameObject)iRacer).transform.Find("_CameraTarget");
                                if (CameraTargetTransform == null)
                                {
									CameraTargetTransform = ((GameObject)iRacer).transform;
                                }

                                GameCamereComponent.TargetObjects.Add(CameraTargetTransform);
                                ///Target the camera to latest AI
                                if (HumanRacerPrefab == null)
                                { GameCamereComponent.TargetVehicle = CameraTargetTransform; }
                            }

                        }
                    }

                    if (DeployHumanNow)
                    {
						if (Constants.isMultiplayerSelected) {
                            var name = FirebaseManager.Instance.userProfile.Name; // PlayfabManager.PlayerGameName;// FacebookAndPlayFabManager.Instance.FacebookUserName;
                            var id = FirebaseManager.Instance.userProfile.UID;//PlayfabManager.PlayerID;// FacebookAndPlayFabManager.Instance.FacebookUserId;

                            int IndexValue = 0;

                            for (int k = 0; k < PhotonNetwork.PlayerList.Length; k++)
                            {
                                if (PhotonNetwork.LocalPlayer.ActorNumber == PhotonNetwork.PlayerList[k].ActorNumber)
                                {
                                    IndexValue = k;
                                    break;
                                }
                            }

                           // var index = RewardProperties.Instance.GetBuggySelected ();
                            //index = index > 1 ? 1 : index;

                            var onlineBuggyToSpawn = playerBuggies[IndexValue].name;
                            //var onlineBuggyToSpawn = index == 0 ? "Buggy1" : "CustomDummyNewOnline2";

                            if (name == null) name = "Player";
                            if (id == null) id = UnityEngine.Random.Range(100000, 99999).ToString();

                            Debug.LogError("player instantiated: "+ IndexValue);

                            iRacer = PhotonNetwork.Instantiate (onlineBuggyToSpawn, SPItems [IndexValue].transform.position, SPItems [IndexValue].transform.rotation, 0, 
								new object[] { RewardProperties.Instance.GetBuggyUpgrade (IndexValue),

									name.Length == 0 ? "Player" : name, id
								}) as GameObject;

                            //PlayerCarController carInstane = iRacer.GetComponent<PlayerCarController>();
                            //UpdateCarMaterial MatInstance = this.gameObject.GetComponent<UpdateCarMaterial>();
                            //Material[] _materials;

                            //for (int j = 0; j < carInstane.MeshParts.Count; j++)
                            //{
                            //    _materials = carInstane.MeshParts[j].materials;
                            //    _materials[0] = MatInstance.GetRelatedMaterial(IndexValue);
                            //    carInstane.MeshParts[j].materials = _materials;
                            //}


                        } else {
							iRacer = (GameObject)Instantiate (HumanRacerPrefab, SPItems [i].transform.position, SPItems [i].transform.rotation);
							iRacer.name = "CustomCarDummyNew(Clone)";

                            //PlayerCarController carInstane = iRacer.GetComponent<PlayerCarController>();
                            //UpdateCarMaterial MatInstance = this.gameObject.GetComponent<UpdateCarMaterial>();
                            //Material[] _materials;

                            //for (int j = 0; j < carInstane.MeshParts.Count; j++)
                            //{
                            //    _materials = carInstane.MeshParts[j].materials;
                            //    _materials[0]= MatInstance.GetRelatedMaterial(1);
                            //    carInstane.MeshParts[j].materials = _materials;
                            //}
                        }
                        Transform CameraTargetTransform = ((GameObject)iRacer).transform.Find("_CameraTarget");
                        if (CameraTargetTransform == null)
                        {
                            CameraTargetTransform = ((GameObject)iRacer).transform;
                        }

                        GameCamereComponent.TargetObjects.Add(CameraTargetTransform);
                        GameCamereComponent.TargetVehicle = CameraTargetTransform;
                        HumanDeployed = true;
                        DeployHumanNow = false;
						racer = iRacer;
                    }

                    //if (i + 1 == RacePlayers || DeploymentComplete) break;
                }
            }
            else
            {
                Debug.LogWarning(RGKMessages.SpawnPointsObjectMissing);
                Debug.DebugBreak();
                return;
            }

            CreateDistanceMeasurementTransforms();
            CurrentCount = TimerCountdownFrom;

            IsRaceReady = true;

            if (RaceStartsOnStartup && !Constants.isMultiplayerSelected)
            {
                StartRace();
            }

            if (!StartMusicAfterCountdown)
            {
                if (GameAudioComponent != null) GameAudioComponent.PlayBackgroundMusic(true);
            }
        }
        // For Randomizing AI from AI list
        [HideInInspector]
        private List<int> Indexes;

        private int FindNextNotSpawnedAiIndex()
        {
            int AiIndex = -1;
            Indexes = new List<int>();
            for (int i = 0; i < AIRacerPrefab.Length; i++)
            {
                Indexes.Add(i);
            }
            ShuffleIndexes(Indexes);


            foreach (int Index in Indexes)
            {
//				Debug.Log ("index "+Index);
				if (!SpawnedAIs.Contains(Index))
                {
                    AiIndex = Index;
                    break;
                }
            }

//			for (int i = 0; i < Indexes.Count; i++)
//			{
//				//Debug.Log ("index "+i+"SpawnedAIs  "+SpawnedAIs.Count);
//				//if (SpawnedAIs != null) {
//					if (!SpawnedAIs.Contains (i)) {
//						AiIndex = i;
//						break;
//					}
////				}
////				else
////					AiIndex = i;
//					
//			}
//			Debug.Log ("index "+AiIndex);
            return AiIndex;
        }

        private void ShuffleIndexes<T>(IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void StartRace()
        {
            if (IsRaceReady)
            {
                GameUIComponent.ShowCountdownWindow = true;
                GameUIComponent.CurrentCount = this.CurrentCount;
                GameCamereComponent.CurrentCount = this.CurrentCount;
                isCounting = true;
            }
        }


        [DoNotObfuscate]
        private void OnDestroy()
        {
            DestroyObject(DistanceTransformContainer);
        }

        /// <summary>
        /// Starts the game, shows countdown and starts playing music.
        /// </summary>
        private void StartGame()
        {
            GameUIComponent.ShowCountdownWindow = false;
            IsRaceStarted = true;
            if (StartMusicAfterCountdown)
            {
                if (GameAudioComponent != null) GameAudioComponent.PlayBackgroundMusic(true);
            }
        }

        [DoNotObfuscate]
        private void Update()
        {
            DoUpdate();
        }


        private void DoUpdate()
        {
            float delta = Time.smoothDeltaTime;
            if (delta != 0.0)
            {
                this.WorkingFPS = 1 / delta;
            }

            if (isCounting)
            {
                CountDownForGameStart();
            }

            switch (RaceType)
            {
                case RaceTypeEnum.Sprint:
                    CalculateStandingsByDistance();
                    StartTimer();
                    break;

                case RaceTypeEnum.Circuit:
                    CalculateStandingsByDistance();
                    StartTimer();
                    break;

                case RaceTypeEnum.LapKnockout:
                    CalculateStandingsByDistance();
                    StartTimer();
                    GetLastRacer();
                    break;
                case RaceTypeEnum.TimeAttack:
                    StartTimer();
                    CalculateCheckPointTime();
                    break;
                case RaceTypeEnum.Speedtrap:
                    if (SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                    {
                        CalculateStandingsByHighSpeed();
                    }
                    else if (SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
                    {
                        CalculateStandingsByTotalHighSpeed();
                    }
                    StartTimer();

                    break;
                //case RaceTypeEnum.KnockOutTime:
                //    CalculateStandings();
                //    StartTimer();
                //    GetLastRacer();
                //    break;
               
            }

            CheckIsRaceFinished();
        }

        public void ShowWrongWayForPlayer(bool isWrongWay)
        {
			/*
			 * added by HADI
			 * added IsRaceFinished Condition
			 * */

			if (isWrongWay && !IsRaceFinished)
            {
                GameUIComponent.ShowWrongWayWindow = true;
            }
            else
            {
                GameUIComponent.ShowWrongWayWindow = false;
            }


        }
        /// <summary>
        /// Enumarates all racers on track and querys if all racers finished 
        /// </summary>
        private void CheckIsRaceFinished()
        {
            if (IsRaceStarted && !IsRaceFinished)
            {
                /*
				 * added by HADI
				 * Commented for loop
				 * added the finish condition only for player
				 * */

                if (StopRaceOnPlayerFinish)
                {
                    tmpFinished = true;
                }

                for (int i = 0; i < ListRacers.Count; i++)
                {
                    if (StopRaceOnPlayerFinish && ListRacers[i].IsPlayer && ListRacers[i].RacerFinished)
                    {
                        Debug.Log(ListRacers[i].RacerFinished);
                        tmpFinished = true;
                        break;
                    }
                }

                if (!tmpFinished)
                {
                    for (int i = 0; i < ListRacers.Count; i++)
                    {

						if (ListRacers[i].IsPlayer && ListRacers[i].RacerFinished)
                        {
                            tmpFinished = true;
                        }
                        else
                        {
                            tmpFinished = false;
                            break;
                        }
                    }
                }

                if (IsRaceStarted && tmpFinished)
                {
                    IsRaceFinished = true;
                }
            }

            if (IsRaceFinished)
            {
                GameUIComponent.RaceFinished(this.RaceType.ToString());
                if (!isRaceFinishPushed)
                {
//                    Debug.Log("Race Finished!");

                    isRaceFinishPushed = true;
//					GetComponent<Race_UI> ().RaceFinishedButton.gameObject.SetActive (true);
                }
            }
        }
         private int _lastI=-1;
        /// <summary>
        /// Determinates last racer based the standings. 
        /// This applies only KnockOut type games.
        /// </summary>
        private void GetLastRacer()
        {

            if (KickLastRacerAfter2ndLast && _lastI > 0 && !ListRacers[_lastI - 1].RacerDestroyed)
            {
                if (ListRacers[_lastI - 1].RacerLap > ListRacers[_lastI].RacerLap)
                {
                    LastRacerID = ListRacers[_lastI].ID.ToString();
                    LastRacerName = ListRacers[_lastI].RacerName.ToString();
                    ListRacers[_lastI].RacerFinished = true;
                    ListRacers[_lastI].RacerDestroyed = true;
                    ListRacers[_lastI].RacerTotalTime = this.TimeTotal;
                    _lastI = -1;
                    return;
                }
            }

            for (int i = ListRacers.Count - 1; i >= 1; i--)
            {
                if (KickLastRacerAfter2ndLast)
                {
                    
                    if (!ListRacers[i].RacerDestroyed && _lastI == -1)
                    {
                        _lastI = i;
                    }
                }
                else
                {
                    if (!ListRacers[i].RacerDestroyed && _lastI == -1)
                    {
                     LastRacerID = ListRacers[i].ID.ToString();
                     LastRacerName = ListRacers[i].RacerName.ToString();
                     break;
                     
                    }
                }

            }
        }

        /// <summary>
        /// Standing calculation based racers distances.
        /// </summary>
        private void CalculateStandingsByDistance()
        {
            int iPlace = 0;
            foreach (Racer_Detail _RD in ListRacers)
            {
                if (_RD.RacerFinished && !_RD.RacerDestroyed)
                {
                    iPlace++;
                }
            }

            if ((iPlace) < (ListRacers.Count))
            {
                RacerComparer RacerCompare = new RacerComparer();
                ListRacers.Sort(iPlace, ListRacers.Count - iPlace, RacerCompare);

                for (int i = iPlace; i < ListRacers.Count; i++)
                {
                    if (ListRacers[i].RacerFinished || ListRacers[i].RacerDestroyed)
                    { }
                    else
                    {
                        ListRacers[i].RacerStanding = i + 1;
                    }
                }
                ListRacers.Sort(delegate(Racer_Detail r1, Racer_Detail r2) { return r1.RacerStanding.CompareTo(r2.RacerStanding); });
                //ListRacers.Sort(iPlace, ListRacers.Count - iPlace, RacerCompare);
            }
        }
        /// <summary>
        /// calculate standings based highest captured speed
        /// </summary>
        private void CalculateStandingsByHighSpeed()
        {
            int iPlace = 0;

            bool startSort = false ;

            foreach (Racer_Detail _RD in ListRacers)
            {
                if (_RD.RacerFinished && !_RD.RacerDestroyed)
                {
                    iPlace++;
                }
                if (_RD.RacerHighestSpeed > 0) startSort = true;
            }

            if ((iPlace) < (ListRacers.Count) && startSort)
            {
                RacerComparerByHighSpeed RacerCompare = new RacerComparerByHighSpeed();
                ListRacers.Sort(iPlace, ListRacers.Count - iPlace, RacerCompare);

                for (int i = iPlace; i < ListRacers.Count; i++)
                {
                    if (!ListRacers[i].RacerFinished && !ListRacers[i].RacerDestroyed)
                    {
                        ListRacers[i].RacerStanding = i + 1;
                    }
                }

                ListRacers.Sort(delegate(Racer_Detail r1, Racer_Detail r2) { return r1.RacerStanding.CompareTo(r2.RacerStanding); });
            }
        }

        /// <summary>
        /// Calculate Standings based sum of captured speeds
        /// </summary>
        private void CalculateStandingsByTotalHighSpeed()
        {
            int iPlace = 0;

            bool startSort = false;

            foreach (Racer_Detail _RD in ListRacers)
            {
                if (_RD.RacerFinished && !_RD.RacerDestroyed)
                {
                    iPlace++;
                }
                if (_RD.RacerHighestSpeed > 0) startSort = true;
            }

            if ((iPlace) < (ListRacers.Count) && startSort)
            {
                RacerComparerByTotalHighSpeed RacerCompare = new RacerComparerByTotalHighSpeed();
                ListRacers.Sort(iPlace, ListRacers.Count - iPlace, RacerCompare);
                //ListRacers.Sort(delegate(RacerDetail r1, RacerDetail r2) { return r1.RacerDistance.CompareTo(r2.RacerDistance); });

                for (int i = iPlace; i < ListRacers.Count; i++)
                {
                    if (!ListRacers[i].RacerFinished && !ListRacers[i].RacerDestroyed)
                    {
                        ListRacers[i].RacerStanding = i + 1;
                    }
                }

                ListRacers.Sort(delegate(Racer_Detail r1, Racer_Detail r2) { return r1.RacerStanding.CompareTo(r2.RacerStanding); });
            }
        }


        #region Timer

        private void CountDownForGameStart()
        {
            if (isCounting)
            {
                countdown_internal_timer -= Time.deltaTime;
//				Debug.Log ("CurrentCount " +CurrentCount);
                if (CurrentCount > 0)
                {
                    if (countdown_internal_timer < 0.1f)
                    {
                        if (GameAudioComponent != null) GameAudioComponent.PlayCountDownAudio();
                        CurrentCount -= 1;
                        GameUIComponent.CurrentCount = CurrentCount;
                        GameCamereComponent.CurrentCount = CurrentCount;
//                        Debug.Log("Race Starting in " + CurrentCount);
                        countdown_internal_timer = 1;
                    }
                }
                else
                {
                    if (GameAudioComponent != null) GameAudioComponent.PlayStartAudio();
                    StartGame();
//					Debug.Log("Race Start");
                    GameCamereComponent.IsStartupAnimationEnabled = false;
                    isCounting = false;
                }

           }
        }
        private float countdown_internal_timer = 1;


        private void StartTimer()
        {
            if (IsRaceStarted)
            {
                //  PhotonNetwork.Time;
                if(Constants.isMultiplayerSelected)
                TimeTotal = (float)PhotonNetwork.Time - TimeTotalFix;
                else
                TimeTotal = Time.time - TimeTotalFix; // Calculates all race timeStart

                TimeCurrent = TimeTotal - CurrentTimeFixForPlayer;
            }
            else
            {
               

                if (Constants.isMultiplayerSelected)
                {
                    TimeStart = (float)PhotonNetwork.Time;      // This helps time timer start after countdown, but this value will reset each lap
                    TimeTotalFix = (float)PhotonNetwork.Time;  // This helps time timer start after countdown, but this value will NOT reset.
                }
                else
                {
                     TimeStart = Time.time;      // This helps time timer start after countdown, but this value will reset each lap
                     TimeTotalFix = Time.time;  // This helps time timer start after countdown, but this value will NOT reset.
                }
            }
        }
        #endregion

        #region TimeTrial-Checkpoint
        /// <summary>
        /// Retrieve the first checkpoint for checkpint time calculations
        /// </summary>
        private void GetFirstCheckPoint()
        {
            Transform[] CheckPointItems = GetChildTransforms(CheckPoints.transform);
            CheckPointItem FirstCheckPoint = CheckPointItems[0].GetComponent(typeof(CheckPointItem)) as CheckPointItem;
            TimeTotalCheckPoint = FirstCheckPoint.CheckpointTime;
        }
        /// <summary>
        /// Calculate checkpoint time and define is race still active or not
        /// </summary>
        private void CalculateCheckPointTime()
        {
            if (IsRaceStarted && !IsRaceFinished)
            {
                TimeNextCheckPoint = TimeTotalCheckPoint - Time.timeSinceLevelLoad + TimerCountdownFrom;
                if (TimeNextCheckPoint <= 0)
                {
                    ListRacers[0].RacerFinished = true;
                    IsRaceFinished = true;
                }
            }
        }
        /// <summary>
        /// Retrieve the checkpoint item for current checkpoint bonus time
        /// </summary>
        /// <param name="PassedCheckPoint"></param>
        public void PlayerPassedCheckPoint(CheckPointItem PassedCheckPoint)
        {
            TimeTotalCheckPoint += PassedCheckPoint.CheckpointBonus;
        }

        #endregion
        #endregion

        #region PrivateHelpers
		private Vector3 Temp;
        private void CreateDistanceMeasurementTransforms()
        {
            if (Waypoints == null) return;

            DistanceTransformContainer = new GameObject();
            DistanceTransformContainer.name = "_DistanceContainer";
//			Temp = DistanceTransformContainer.transform.position;
//			Temp.y += 0.7f;
//			DistanceTransformContainer.transform.position = Temp;


            Transform[] trans = GetChildTransforms(Waypoints.transform);
            if (trans.Length < 1)
                return;


            if (DistancePointDensity < trans.Length) DistancePointDensity = trans.Length;

            SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
            SetupSplineInterpolator(interp, trans);
            interp.StartInterpolation(null, false, eWrapMode.ONCE);

            for (int c = 0; c <= DistancePointDensity - 1; c++)
            {
                float currTime = c * 5 / DistancePointDensity;
                Vector3 currPos = interp.GetHermiteAtTime(currTime);
                // float mag = (currPos - prevPos).magnitude * 2;

                GameObject _tempTrans = new GameObject();
                if (c < 10) { _tempTrans.name = "0" + c.ToString(); } else { _tempTrans.name = c.ToString(); }

                _tempTrans.transform.parent = DistanceTransformContainer.transform;
				currPos.y += 0.7f;
                _tempTrans.transform.localPosition = currPos;

              //  BoxCollider boxCollider = _tempTrans.AddComponent<BoxCollider>();
              //  boxCollider.size = new Vector3(30, 5, 1);
              //  boxCollider.isTrigger = true;
             //   _tempTrans.tag = "dpp";
            }

            if (FinishPoint != null)
            {
                GameObject _finalPoint = new GameObject();
                _finalPoint.name = DistancePointDensity.ToString();
                _finalPoint.transform.parent = DistanceTransformContainer.transform;
                _finalPoint.transform.localPosition = FinishPoint.transform.position;
                _finalPoint.transform.localRotation= FinishPoint.transform.rotation;
            }
            else
            {
                Debug.LogWarning(RGKMessages.FinishPointMissing);
                return;
            }
            //Calculate total race distance between using this created transforms.
            DistanceMeasurementObjects = GetChildTransforms(DistanceTransformContainer.transform);
            for (int i = 0; i < DistanceMeasurementObjects.GetUpperBound(0); i++)
            {
                if (i < DistanceMeasurementObjects.GetUpperBound(0))
                {
                    if (i != DistanceMeasurementObjects.GetUpperBound(0))
                    {
                        RaceLength += Vector3.Distance(DistanceMeasurementObjects[i].transform.position, DistanceMeasurementObjects[i + 1].transform.position);
                    }
                    //Debug.Log(i.ToString() + "=" + Vector3.Distance(wayPoints[i].transform.position, wayPoints[i + 1].transform.position));
                }
            }

            FixDistancePointRotations(DistanceTransformContainer);
        }

        public static void FixDistancePointRotations(GameObject DistancePointContainer)
        {
            List<Component> components = new List<Component>(DistancePointContainer.gameObject.GetComponentsInChildren(typeof(Transform)));
            List<Transform> transforms = components.ConvertAll(c => (Transform)c);

            transforms.Remove(DistancePointContainer.gameObject.transform);
            transforms.Sort(delegate(Transform a, Transform b)
            {
                return a.name.CompareTo(b.name);
            });
            Transform[] WPItems = transforms.ToArray();
            for (int i = 0; i < WPItems.Length - 1; i++)
            {
                if (WPItems[i + 1] != null)
                {
                    WPItems[i].transform.LookAt(WPItems[i + 1].transform);
                }

                if (i==(WPItems.Length - 1))
                {
                    WPItems[i].transform.LookAt(WPItems[1].transform);
                }
            }
        }

        public enum eOrientationMode { NODE = 0, TANGENT }

        /// <summary>
        /// Interpolates Spline
        /// </summary>
        /// <param name="interp"></param>
        /// <param name="trans"></param>
        private void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
        {
            bool AutoClose = false;
            float Duration = 5f;
            eOrientationMode OrientationMode = eOrientationMode.NODE;

            interp.Reset();

            float step = (AutoClose) ? Duration / trans.Length :
                Duration / (trans.Length - 1);

            int c;
            for (c = 0; c < trans.Length; c++)
            {
                if (OrientationMode == eOrientationMode.NODE)
                {
                    interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
                }
                else if (OrientationMode == eOrientationMode.TANGENT)
                {
                    Quaternion rot;
                    if (c != trans.Length - 1)
                        rot = Quaternion.LookRotation(trans[c + 1].position - trans[c].position, trans[c].up);
                    else if (AutoClose)
                        rot = Quaternion.LookRotation(trans[0].position - trans[c].position, trans[c].up);
                    else
                        rot = trans[c].rotation;

                    interp.AddPoint(trans[c].position, rot, step * c, new Vector2(0, 1));
                }
            }

            if (AutoClose)
                interp.SetAutoCloseMode(step * c);
        }

        /// <summary>
        /// Returns all children transforms as array but removes actualy transform from this array. 
        /// </summary>
        /// <param name="RootTransform"></param>
        /// <returns></returns>
        /// 

        [DoNotObfuscate]
        private Transform[] GetChildTransforms(Transform RootTransform)
        {
            if (RootTransform == null)
                return new Transform[] { };

            List<Component> components = new List<Component>(RootTransform.GetComponentsInChildren(typeof(Transform)));
            List<Transform> transforms = components.ConvertAll(c => (Transform)c);

            transforms.Remove(RootTransform.transform);

            transforms.Sort(delegate(Transform a, Transform b) { return Convert.ToInt32(a.name).CompareTo(Convert.ToInt32(b.name)); });

            return transforms.ToArray();
        }


        #endregion

		#region multiplayer static properties
		public static GameObject player;
		public static List<GameObject> enemies;
		public static int playerSpawnPosition;
		public static int totalRacers;
		#endregion

    }
}