//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Race Progress Script
// This script registers racer to Race Manager.
// Last Change : 12/09/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Helpers;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
	[AddComponentMenu("Racing Game Kit/Game Mechanics/Racer Register")]
	public class Racer_Register : MonoBehaviour
	{
		#region Locals

		/// <summary>
		/// RacerName from RaceManager for AI or from Start Screen for Human Player
		/// </summary>
		public String RacerName = "";
		/// <summary>
		///Racer standing in races
		/// </summary>
		public int RacerStanding = 0;
		/// <summary>
		/// Racer finished game or not
		/// </summary>
		public Boolean IsRacerFinished = false;
		/// <summary>
		/// Racer started to game or not
		/// </summary>
		public Boolean IsRacerStarted = false;
		/// <summary>
		/// Racer destroyed/disabled/knocket out or disconnected from game or not
		/// </summary>
		public Boolean IsRacerDestroyed = false;
		/// <summary>
		/// Is current racer is Human Player? 
		/// </summary>
		private Boolean IsPlayer = false;
		/// <summary>
		/// RaceManager object instance
		/// </summary>
		private Race_Manager RaceManager;
		/// <summary>
		/// Racer Detail object instance that keeps racers data 
		/// </summary>
		public Racer_Detail RacerDetail = null;
		/// <summary>
		/// Distance object array that helps to calculation based waypoint distances.
		/// </summary>
		[DoNotObfuscate]
		private List<Transform> DistanceList = new List<Transform> { };
		/// <summary>
		/// Index of RaceManager racer list that reaching to gamer properties in RaceManager Racer collection.
		/// </summary>
		private int RaceManagerRacerIndex = 0;
		/// <summary>
		/// Current lap of registered racer
		/// </summary>
		private int GamerCurrentLap = 1;
		/// <summary>
		/// The textmesh for dispaying RacerName in RacerTag prefab
		/// </summary>
		private TextMesh RacerTagName;
		/// <summary>
		/// the textmesh for displaying racer position in RacerTag prefab
		/// </summary>
		private TextMesh RacerTagPlace;
		/// <summary>
		/// If not racertag found some of calculations will be disabled.
		/// </summary>
		private Boolean RacerTagNotFound = false;
		/// <summary>
		/// DistanceChecker object that placed front of racer vehicle. Distance calculations will be calculated between this object and DistanceList objects
		/// </summary>
		[DoNotObfuscate]
		private Transform GamerDistanceChecker;
		/// <summary>
		/// Distance array cache 
		/// </summary>
		[DoNotObfuscate]
		private Transform[] DistanceArray;
		/// <summary>
		/// Checkpoint array cache 
		/// </summary>
		[DoNotObfuscate]
		private Transform[] CheckpointArray;
		/// <summary>
		/// Checkpoint Arrow Object cache
		/// </summary>
		[HideInInspector]
		public GameObject CheckpointArrow;
		/// <summary>
		/// Current checkpoint objects index in checkpoint array
		/// </summary>
		[HideInInspector]
		private int CurrentCheckpointIndexInContainer = 0;
		/// <summary>
		/// Is checkpoint system available? If checkpoints not set up, this will disabled automaticaly
		/// </summary>
		private bool IsCheckPointSystemEnabled = false;
		/// <summary>
		/// Internal timer interval for multiple collider locking
		/// </summary>
		private float triggerLockTime = 1;
		/// <summary>
		/// If it car have multiple collider, this will be true for multiple collision trigger 
		/// </summary>
		private bool isTriggerLocked = false;
		/// <summary>
		/// Current distance point index in distance array
		/// </summary>
		[SerializeField]
		[DoNotObfuscate]
		private int CurrentDistancePointIndex = 0;
		/// <summary>
		/// Previous distance point index in distance array
		/// </summary>
		[DoNotObfuscate]
		private int LastDistancePointIndex;
		/// <summary>
		/// Locks distance calculator swiwtch to distance pointer for preventing wrong calculations on wrong way
		/// </summary>
		private bool DistanceCheckingStuffLocked = false;
		/// <summary>
		/// Relative distance point position. If z lower then -1 means car passed that DP.
		/// </summary>
		private Vector3 RelativeDPPosition;
		/// <summary>
		/// For calculating vehicle position for wrong way system
		/// </summary>
		private float AngleOfDP = 0;
		/// <summary>
		/// For calculating vehicle position for wrong way system
		/// </summary>
		private float AngleOfVH = 0;
		/// <summary>
		/// For calculating vehicle position for wrong way system
		/// </summary>
		private float AngleDif = 0;
		/// <summary>
		/// Is the vehicle rotated back on track
		/// </summary>
		private bool RotatedBack = false;
		/// <summary>
		/// For calculating vehicle position for wrong way system
		/// </summary>
		private bool From4 = false;
		/// <summary>
		/// Is the vehicle driving wrong way?
		/// </summary>
		public bool IsWrongWay = false;
		/// <summary>
		/// Distance array lenght cache
		/// </summary>
		private int DistanceArrayLenght = 0;
		/// <summary>
		/// Did the racer hit last DP - for lap change calculations
		/// </summary>
		private bool HitLastDP = false;
		/// <summary>
		/// This player's timer fix for lap correction
		/// </summary>
		[HideInInspector]
		public float MyTimeFix = 0;

		[HideInInspector]
		public bool PlayerContinueAfterFinish = false;

		public Sprite Avatar;

		private float totalDistance;
        public Transform LastDP;
        public Transform body;
		public float TotalDistance{
			get { return totalDistance; }
		}
		#endregion

		#region StartUp

		private void Awake()
		{
			if (!this.enabled) return;
			try
			{
				GameObject RaceManagerObject = GameObject.Find("_RaceManager");
				if (RaceManagerObject != null)
				{
					RaceManager = RaceManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
				}

			}
			catch { }
		}
		public String RacerID;
		private void Start()
		{
			if (!this.enabled) return;
			if (gameObject.tag == "Player") IsPlayer = true;
			GamerDistanceChecker = transform.Find("_DistancePoint");
			if (GamerDistanceChecker == null)
			{
				Debug.LogError(RGKMessages.DistanceCheckerMissing);
				Debug.DebugBreak();
			}

			if (RaceManager.CheckPoints != null) // If Checkoints speedtraps or sectors Implemented
			{
				CheckpointArray = GetChildTransforms(RaceManager.CheckPoints.transform);

				if (CheckpointArray.Length > 0)
				{
					IsCheckPointSystemEnabled = true;

					CheckpointArrow = GameObject.Find("CheckpointArrow");
					if (CheckpointArrow != null && RaceManager.EnableCheckpointArrow) CheckpointArrow.SetActiveRecursively(true);
				}
			}


			GameObject DistanceTransformContainer = RaceManager.DistanceTransformContainer;
			DistanceArray = GetChildTransforms(DistanceTransformContainer.transform);
			DistanceList.AddRange(DistanceArray);
			SortDPByDistance();
			DistanceArrayLenght = DistanceArray.Length;

			if (base.gameObject.transform.Find("_RacerTag") != null)
			{
				RacerTagName = base.gameObject.transform.Find("_RacerTag/Name").GetComponent(typeof(TextMesh)) as TextMesh;
				RacerTagPlace = base.gameObject.transform.Find("_RacerTag/Place").GetComponent(typeof(TextMesh)) as TextMesh;
			}
			else
			{
				RacerTagNotFound = true;
				if (!IsPlayer)
				{
					Debug.LogWarning(RGKMessages.RacerTagObjectsMissing);
				}
			}

			if (RaceManager != null)
			{
				RacerDetail = new Racer_Detail();
                RacerDetail.me = body;
				RacerDetail.avatar = Avatar;
				RacerDetail.ID = UnityEngine.Random.Range(1000,50000).ToString();
				RacerID = RacerDetail.ID;

				if (IsPlayer)
				{
					RacerDetail.RacerName = RacerName;
					RacerDetail.IsPlayer = true;
				}else if(Constants.isMultiplayerSelected){
					RacerDetail.RacerName = RacerName;					
				}
				else
				{
					RacerDetail.RacerName = RaceManager.GetNameForRacer(RacerName);
					RacerName = RacerDetail.RacerName;
				}

				RacerDetail.RacerLap = GamerCurrentLap;
				RaceManagerRacerIndex = RaceManager.RegisterToGame(RacerDetail);

				if (!RacerTagNotFound)
				{
					if (RacerTagPlace != null) RacerTagPlace.text = RaceManager.RegisteredRacers[RaceManagerRacerIndex].RacerStanding.ToString();
					if (RacerTagPlace != null) RacerTagName.text = RaceManager.RegisteredRacers[RaceManagerRacerIndex].RacerName.ToString();
				}

			}

			totalDistance = CalculateRemainingDistance ();
            InvokeRepeating("FixedUpdate2", 0.1f, 0.2f);
		}
		#endregion

		#region Process
		[DoNotObfuscate]
		void OnTriggerEnter(Collider Trigger)
		{
            if( Trigger.name == "_StartPoint")
            {
                justCrossedFinishLine = false;
            }
            /*
            if(Trigger.tag == "dpp")
            {
                if (int.Parse(Trigger.name) < CurrentDistancePointIndex + 2)
                     CurrentDistancePointIndex = int.Parse(Trigger.name);
                if (int.Parse(Trigger.name) < CurrentDistancePointIndex - 2)
                {
                    IsWrongWay = true;
                    RacerDetail.RacerWrongWay = true;
                }
                else
                {
                    IsWrongWay = false;
                    RacerDetail.RacerWrongWay = false;
                }


            }*/

			CheckTriggeredThings(Trigger);

		}

		private void CheckTriggeredThings(Collider Trigger)
		{
			if (!this.enabled) return;

			if (IsRacerStarted && !IsRacerFinished)
			{

				if (isTriggerLocked) return;
                if (this.gameObject.tag == "Player" && justCrossedFinishLine && RacerDetail.ID == PlayerManagerScript.instance.Car.GetComponent<Racer_Register>().RacerDetail.ID)
                    return;

				if (Trigger.name == RaceManager.FinishPoint.transform.name && HitLastDP)
				{
                    justCrossedFinishLine = true;
                    ClosestDP = GetClosestDP(DistanceArray);
					if (HitLastDP)
					{
                        if(IsPlayer)
                        Debug.Log("HitLastDP " + CurrentDistancePointIndex);
						DistanceCheckingStuffLocked = false;
						CurrentDistancePointIndex = 0;
						HitLastDP = false;

					}

					///Increase the lap
					GamerCurrentLap++;
					RacerDetail.RacerLap = GamerCurrentLap;


					float MyCurrentTime = RaceManager.TimeTotal; // capture the time while calculations made we dont want any delay.
					MyCurrentTime -= MyTimeFix;

					//float FixedCurrentTime = CurrentTime - CurrentTimeFix;

					///Add current time to racer's lap list
					if (!IsRacerFinished)
					{
						RacerDetail.RacerLapTimes.Add(MyCurrentTime);
					}
					//If racers active lap > total laps that means racer is finished the race. 
					if (GamerCurrentLap > RaceManager.RaceLaps)
					{
						IsRacerFinished = true;
						RacerDetail.RacerTotalTime = RaceManager.TimeTotal;
						RacerDetail.RacerFinished = true;

						if (IsPlayer && RaceManager.PlayerContinuesAfterFinish)
						{
							SwitchToAIMode();
						}
					}

					//apply new fix the active time with current time spent on lap
					MyTimeFix += MyCurrentTime;

					//Get last racer name from game manager and if its equals to current racer, kick out the racer.
					//this applies only knockout game 
					if (RacerDetail.ID.ToString().Equals(RaceManager.LastRacerID))
					{
						Debug.Log(RacerDetail.RacerName + " destroyed");
						RacerDetail.RacerTotalTime = RaceManager.TimeTotal;
						RacerDetail.RacerFinished = true;
						IsRacerFinished = true;
						RacerDetail.RacerDestroyed = true;
						IsRacerDestroyed = true;

					}

					if (gameObject.tag == "Player")
					{
						RacerDetail.RacerLastTime = MyCurrentTime;
						RaceManager.CurrentTimeFixForPlayer = MyTimeFix;
						RaceManager.TimePlayerLast = RacerDetail.RacerLastTime;

						//if racer no best time yet most likely in first lap
						if (RacerDetail.RacerBestTime == 0f)
						{
							RacerDetail.RacerBestTime = RacerDetail.RacerLastTime;
						}
						//if racer besttime lower then current time, update the best time then..
						else if (RacerDetail.RacerLastTime < RacerDetail.RacerBestTime)
						{
							RacerDetail.RacerBestTime = RacerDetail.RacerLastTime;
						}

						RaceManager.TimePlayerBest = RacerDetail.RacerBestTime;

						if (!IsRacerFinished)
						{
							RaceManager.RaceActiveLap++;
						}
					}
					else //for ai dudes
					{
						RacerDetail.RacerLastTime = MyCurrentTime;
						//if racer no best time yet most likely in first lap
						if (RacerDetail.RacerBestTime == 0f)
						{
							RacerDetail.RacerBestTime = RacerDetail.RacerLastTime;
						}
						else if (RacerDetail.RacerLastTime < RacerDetail.RacerBestTime)
						{
							RacerDetail.RacerBestTime = RacerDetail.RacerLastTime;
						}
					}
				}

				if (IsCheckPointSystemEnabled)
				{

					if (Trigger.tag == "CheckPoint" || !RacerDetail.RacerFinished)
					{
						if (RacerDetail.RacerSectorSpeedAndTime == null) RacerDetail.RacerSectorSpeedAndTime = new List<Racer_Detail.SectorSpeedAndTime>();
						Racer_Detail.SectorSpeedAndTime sectorSpeedAndTime = new Racer_Detail.SectorSpeedAndTime();
						sectorSpeedAndTime.Lap = GamerCurrentLap;
						sectorSpeedAndTime.SectorTime = 0;
						sectorSpeedAndTime.SectorSpeed = base.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
						RacerDetail.RacerSectorSpeedAndTime.Add(sectorSpeedAndTime);

						if (IsPlayer && Trigger.name == CheckpointArray[CurrentCheckpointIndexInContainer].name)
						{
							SetNextCheckPoint(CurrentCheckpointIndexInContainer);
						}

					}
				}
			}

			//Be sure gamer started race. This created for avoiding hitting start point before first distance point.
			if (Trigger.name == RaceManager.StartPoint.transform.name)
			{
				IsRacerStarted = true;
				isTriggerLocked = true;
			}

		}


		private void SwitchToAIMode()
		{
			RacingGameKit.Racers.RGK_Racer_Basic_AI oAI = base.GetComponent(typeof(RacingGameKit.Racers.RGK_Racer_Basic_AI)) as RacingGameKit.Racers.RGK_Racer_Basic_AI;
			RGKCar.RGKCar_AIController oCont = base.GetComponent(typeof(RGKCar.RGKCar_AIController)) as RGKCar.RGKCar_AIController;
			RGKCar.RGKCar_HumanController oHCont = base.GetComponent(typeof(RGKCar.RGKCar_HumanController)) as RGKCar.RGKCar_HumanController;

			if (oAI != null && oCont != null)
			{
				oAI.enabled = true;
				oCont.enabled = true;
				oHCont.enabled = false;
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

		private void FixedUpdate2()
		{
			if (!this.enabled) return;
			if (RacerDetail.RacerDestroyed != IsRacerDestroyed)
			{
				IsRacerDestroyed = RacerDetail.RacerDestroyed;
				DisableAI();
			}
			if (RacerDetail.RacerFinished != IsRacerFinished) IsRacerFinished = RacerDetail.RacerFinished;

			if (IsRacerDestroyed && RaceManager.MoveKickedRacerToIgnoreLayer)
			{
				if (base.gameObject.layer != LayerMask.NameToLayer("IGNORE"))
				{
//					ChangeLayersRecursively(base.transform, "IGNORE");
				}
			}
			// SortDPByDistance();

			float _distanceToRaceEnd = CalculateRemainingDistance();
			_distanceToRaceEnd += (RaceManager.RaceLaps - GamerCurrentLap) * RaceManager.RaceLength;
			RacerDetail.RacerDistance = _distanceToRaceEnd;

			if (RacerTagPlace != null)
			{
				RacerTagPlace.text = RacerDetail.RacerStanding.ToString();
			}
			RacerDetail.RacerPostionOnMap = transform.position;
			RacerDetail.RacerRotationOnMap = transform.rotation;
			RacerDetail.RacerDestroyed = IsRacerDestroyed;
			RacerStanding = Convert.ToInt32(RacerDetail.RacerStanding);

			if (IsPlayer)
			{
				if (RaceManager.EnableCheckpointArrow) UpdateCheckPointArrow();

				RaceManager.ShowWrongWayForPlayer(IsWrongWay);
			}
			///Feedback Wrong Way
//			if (Mathf.Abs(AngleDif) <= 230f && Mathf.Abs(AngleDif) >= 120f && (GetComponent<Rigidbody>().velocity.magnitude * 3.6f) > 10)
//			{
//				if (CurrentDistancePointIndex < DistanceArray.Length - 1) {
//					IsWrongWay = true;
//					RacerDetail.RacerWrongWay = true;
//				}
//			}
//
//			if (!RotatedBack)
//			{
//				IsWrongWay = false;
//				RacerDetail.RacerWrongWay = false;
//			}

			//Multiple Collider avoiding
			if (isTriggerLocked)
			{
				triggerLockTime = triggerLockTime - Time.deltaTime;

				if (triggerLockTime <= 0)
				{
					isTriggerLocked = false;
					triggerLockTime = 1;
				}
			}
			///Feedback Checkopoint Finished
			if (IsPlayer && RaceManager.RaceType.Equals(RaceTypeEnum.TimeAttack))
			{
				if (RaceManager.IsRaceFinished) this.IsRacerFinished = true;
			}
            if (!IsRacerFinished)
            {
                RacerDetail.RacerTotalTime = RaceManager.TimeTotal;
            }
        }

		#endregion

		#region DistanceControl
		public Transform ClosestDP;

        /// <summary>
        /// Calculates remaining distance based distance object that created by Racemanager 
        /// </summary>
        /// <returns></returns>
        /// 
        public int GetCurrentDistancePointIndex()
        {
            return CurrentDistancePointIndex;
        }
        public int GetLastDistancePointIndex()
        {
            return LastDistancePointIndex;
        }
        public int GetLap()
        {
            return GamerCurrentLap;
        }
        public void SynRacer(int currentPoint,int lastPoint, int lap, bool isFinished, bool isDestroyed)
        {
            if (RacerDetail != null)
            {
                IsRacerFinished = isFinished;
                RacerDetail.RacerFinished = isFinished;
                IsRacerDestroyed = isDestroyed;
                RacerDetail.RacerDestroyed = isDestroyed;
                CurrentDistancePointIndex = currentPoint;
                LastDistancePointIndex = lastPoint;
                GamerCurrentLap = lap;
            }
        }
		private float CalculateRemainingDistance()
		{

			//We dont have to calculate distance since racer finished or destroyed..
			if (IsRacerFinished && !IsRacerDestroyed)
			{
				return 0;
			}
			///If racers not started that means they're at first DP 
			if (!IsRacerStarted)
			{
				CurrentDistancePointIndex = 0;
				LastDistancePointIndex = 0;
			}

			if ((CurrentDistancePointIndex).Equals(DistanceArrayLenght - 1))
			{
				DistanceCheckingStuffLocked = true;
			}



			RelativeDPPosition = transform.InverseTransformPoint(new Vector3(DistanceArray[CurrentDistancePointIndex].transform.position.x, transform.position.y, DistanceArray[CurrentDistancePointIndex].transform.position.z));

			AngleOfDP = DistanceArray[CurrentDistancePointIndex].transform.eulerAngles.y;
			AngleOfVH = base.gameObject.transform.eulerAngles.y;
			AngleDif = 0;//AngleOfDP - AngleOfVH;

			if (Mathf.Abs(AngleDif) <= 230f && Mathf.Abs(AngleDif) >= 120)
			{ RotatedBack = true; }
			else
			{ RotatedBack = false; }

        
			if (RelativeDPPosition.z <= 1f && !RotatedBack && !HitLastDP)
			{
				ClosestDP = GetClosestDP(DistanceArray);
              
				if ((CurrentDistancePointIndex + 1).Equals(DistanceArrayLenght))
				{
					if (DistanceCheckingStuffLocked)
					{
						LastDistancePointIndex = 0;
						CurrentDistancePointIndex = 0;
						HitLastDP = false;
					}
				}
				else
                {
					if (CurrentDistancePointIndex >= LastDistancePointIndex)
					{
						bool leNext = false;
						if (Convert.ToInt16(ClosestDP.name) + 1 == DistanceArrayLenght)
						{
							leNext = true;
						}
						else if (DistanceArray[CurrentDistancePointIndex + 1].name == DistanceArray[Array.FindIndex(DistanceArray, tr => tr.name == ClosestDP.name) + 1].name)
						{
							leNext = true;
						}
						//&& (DistanceArray[CurrentDistancePointIndex + 1].name == DistanceArray[Array.FindIndex(DistanceArray, tr => tr.name == ClosestDP.name) + 1].name)


						LastDistancePointIndex = CurrentDistancePointIndex;

						if (!From4 && leNext && (!ClosestDP.name.Equals(DistanceArrayLenght - 1)))
						{
                            if (this.gameObject.tag == "Player")
                            {
                                if (Vector3.Distance(DistanceArray[CurrentDistancePointIndex].position, this.transform.position) < 20)
                                    CurrentDistancePointIndex++;
                            }
                            else
                                CurrentDistancePointIndex++;
                          //  if (IsPlayer)
                           // Debug.Log("CurrentDistancePointIndex " + CurrentDistancePointIndex);
							if (CurrentDistancePointIndex == (DistanceArrayLenght - 1)) // last dp approval
							{
								HitLastDP = true;
							}
						}
						else
						{

							CurrentDistancePointIndex = Array.FindIndex(DistanceArray, tr => tr.name == ClosestDP.name);
							From4 = false;
						}

					}
					else if (CurrentDistancePointIndex != 0)
					{

						CurrentDistancePointIndex = LastDistancePointIndex;
						From4 = true;
					}
					else if (CurrentDistancePointIndex == 0 && LastDistancePointIndex == (DistanceArrayLenght - 2)) // switching laps
					{
						LastDistancePointIndex = 0;
						HitLastDP = false;
					}
					else if (CurrentDistancePointIndex == (DistanceArrayLenght - 1)) // closer to last dp then first dp
					{
						CurrentDistancePointIndex = LastDistancePointIndex;
					} 
                }
               
			}

			float _distanceToRaceEnd = 0f;
			Vector3.Distance(GamerDistanceChecker.position, DistanceArray[CurrentDistancePointIndex].transform.position);
			for (int i = CurrentDistancePointIndex; i < DistanceArray.GetUpperBound(0); i++)
			{
				_distanceToRaceEnd += Vector3.Distance(DistanceArray[i].transform.position, DistanceArray[i + 1].transform.position);
			}
			_distanceToRaceEnd += Vector3.Distance(GamerDistanceChecker.position, DistanceArray[CurrentDistancePointIndex].transform.position);

			Debug.DrawLine(GamerDistanceChecker.position, DistanceArray[CurrentDistancePointIndex].transform.position, Color.red);

			return _distanceToRaceEnd;
		}

		public void SetCurrentIndexOnRespawn(){
            if (this.gameObject.tag == "Player")
            {
                if (justCrossedFinishLine)
                {
                    CurrentDistancePointIndex = 0;
                    LastDistancePointIndex = 0;
                }
                else
                {
                   // Debug.Log("SetCurrentIndexOnRespawn" + CurrentDistancePointIndex + "   " + ClosestDP.name);
                    int dpi = Convert.ToInt16(ClosestDP.name);
                    if(dpi<DistanceArray.Length-10)
                    dpi = Mathf.Clamp(Convert.ToInt16(ClosestDP.name) + 3, 0, DistanceArray.Length - 1);
                    CurrentDistancePointIndex =dpi;
                   
                }
            }
            else
                CurrentDistancePointIndex = Convert.ToInt16(ClosestDP.name);
        }

		/// <summary>
		/// Sorts Distance Objects by name
		/// </summary>
		[DoNotObfuscate]
		private void SortDPByDistance()
		{
			DistanceList.Sort(delegate(Transform t1, Transform t2)
				{
					return Vector3.Distance(t1.position, GamerDistanceChecker.position).CompareTo(Vector3.Distance(t2.position, GamerDistanceChecker.position));
				});
		}
        bool justCrossedFinishLine = false;

		Transform GetClosestDP(Transform[] DPs)
		{
            if (this.gameObject.tag == "Player")
            {
                if (justCrossedFinishLine)
                {
                    CurrentDistancePointIndex = 0;
                    LastDistancePointIndex = 0;
                    return DistanceArray[0];
                }
            }

            //  return DistanceArray[CurrentDistancePointIndex];

            Transform tMin = null;
			float minDist = Mathf.Infinity;
			foreach (Transform DP in DPs)
			{
				float dist = Vector3.Distance(DP.position, GamerDistanceChecker.position);
				if (dist < minDist)
				{
					tMin = DP;
					minDist = dist;
				}
			}

			if (ClosestDP != null) {
				int currentClosetDP = int.Parse (ClosestDP.name) ;
				int newClosetDp = int.Parse (tMin.name) ;

				int diff = newClosetDp - currentClosetDP;

				if (diff < 0 || diff> 50) {
					IsWrongWay = true;
					RacerDetail.RacerWrongWay = true;
				} else if (diff == 0) {
				
				} else {
					IsWrongWay = false;
					RacerDetail.RacerWrongWay = false;
				}
			}

            if (this.gameObject.tag == "Player")
            {
                if (int.Parse(tMin.name) > CurrentDistancePointIndex + 50)
                {
                    CurrentDistancePointIndex = 0;
                  //  if (IsPlayer)
                    //    Debug.Log(tMin.name + "  " + CurrentDistancePointIndex);
                    LastDistancePointIndex = CurrentDistancePointIndex;
                    return DistanceArray[CurrentDistancePointIndex];
                }
            }

			return tMin;
		}


		#endregion

		#region CheckPointControl

		/// <summary>
		/// Finds the next checkpooint in checkpoint container and set is active
		/// </summary>
		private void SetNextCheckPoint(int PassedCheckpointIndex)
		{
			//tell game manager about player passed the checpoint, so it can make next calculations
			RaceManager.PlayerPassedCheckPoint(CheckpointArray[CurrentCheckpointIndexInContainer].
				GetComponent(typeof(CheckPointItem)) as CheckPointItem);

			if (RaceManager.GameAudioComponent != null)
			{
				RaceManager.GameAudioComponent.PlayCheckPointAudio();
			}

			CurrentCheckpointIndexInContainer++;
			if (CurrentCheckpointIndexInContainer == CheckpointArray.Length)
			{
				CurrentCheckpointIndexInContainer = 0;
			}
		}

		/// <summary>
		/// Updates Checkpoint Arrow to next checkpoint location for player
		/// </summary>
		private void UpdateCheckPointArrow()
		{

			if (IsRacerFinished) return;
			if (!RaceManager.EnableCheckpointArrow) return;
			//_currentCheckpointIndexInContainer = Array.FindIndex(CheckpointArray, tr => tr.name == CheckpointArray[0].name);

			if (IsCheckPointSystemEnabled && CheckpointArray.Length > 0)
			{
				if (!IsRacerStarted) CurrentCheckpointIndexInContainer = 0;

				Vector3 RelativeCheckpointPosition = transform.InverseTransformPoint(new Vector3(CheckpointArray[CurrentCheckpointIndexInContainer].transform.position.x,
					transform.position.y, CheckpointArray[CurrentCheckpointIndexInContainer].transform.position.z));

				Debug.DrawLine(GamerDistanceChecker.position, CheckpointArray[CurrentCheckpointIndexInContainer].transform.position, Color.white);
				if (CheckpointArrow != null)
				{
					CheckpointArrow.transform.LookAt(CheckpointArray[CurrentCheckpointIndexInContainer].transform);
				}
			}
		}
		#endregion

		#region Helpers
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

		private void DisableAI()
		{
			RacingGameKit.AI.AI_Basic oBasicAI = transform.GetComponent(typeof(AI.AI_Basic)) as AI.AI_Basic;
			if (oBasicAI != null) oBasicAI.enabled = false;

			RacingGameKit.AI.AI_Brain oProAI = transform.GetComponent(typeof(AI.AI_Brain)) as AI.AI_Brain;
			if (oProAI != null) oProAI.enabled = false;
		}
		#endregion



	}

}