//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racer Detail Script
// This scripts hold every information related registered racers.
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RacingGameKit
{
    /// <summary>
    /// This class craeted for managing all game options like racetype, racers,players, and statuses.
    /// </summary>
    [AddComponentMenu(""), HideInInspector]
    public class Racer_Detail : IComparable<Racer_Detail>
    {
        private String _ID; //ObjectID
        private float a; //race time
        private float b = 0; //standings
        private string c = ""; //name
        private float d = 0f;//waypoint distance
        private float e = 0f; //best time
        private double f = 0; //racer active lap
        private Boolean g = false; // isfinished
        private float h = 0f; //Total Time
        private Boolean i = false;//Is Player
        private Vector3 j; // Racer location;
        private Quaternion k;//Racer rotation
        private Boolean l;// Racer Destroyed or knocked out
        private List<float> n; // Racer LapTimes
        private Boolean m=false; //Racer Is Wrong Way
		private Boolean o=false; //Racer Is Out Of Bounds
        private List<SectorSpeedAndTime> p; // Sector Speeds and Time
        public Transform me;

		public Sprite avatar;
		public UnityEngine.UI.Image avatarholder;
        /// <summary>
        /// Unique Object ID
        /// </summary>
        public String ID
        { get { return this._ID; } set { this._ID = value; } }
        /// <summary>
        /// Racer Name
        /// </summary>
        public string RacerName
        { get { return this.c; } set { this.c = value; } }
        /// <summary>
        /// Last Lap Time 
        /// </summary>
        public float RacerLastTime
        { get { return this.a; } set { this.a = value; } }
        /// <summary>
        /// Best Lap Time
        /// </summary>
        public float RacerBestTime
        { get { return this.e; } set { this.e = value; } }
        /// <summary>
        /// Total Race Complete Time
        /// </summary>
        public float RacerTotalTime
        { get { return this.h; } set { this.h = value; } }
        /// <summary>
        /// Racers Current Position on Standings
        /// </summary>
        public float RacerStanding
        { get { return this.b; } set { this.b = value; } }
        /// <summary>
        /// Remaining Distance To Finish Point
        /// </summary>
        public float RacerDistance
        { get { return this.d; } set { this.d = value; } }
        /// <summary>
        /// Current lap of the Race
        /// </summary>
        public double RacerLap
        { get { return this.f; } set { this.f = value; } }
        /// <summary>
        /// Is Racer Finished
        /// </summary>
        public Boolean RacerFinished
        { get { return this.g; } set { this.g = value; } }
        /// <summary>
        /// Is This the Player
        /// </summary>
        public Boolean IsPlayer
        { get { return this.i; } set { this.i = value; } }
        /// <summary>
        /// Vector3 position on map
        /// </summary>
        public Vector3 RacerPostionOnMap
        { get { return this.j; } set { this.j = value; } }
        /// <summary>
        /// Rotation 
        /// </summary>
        public Quaternion RacerRotationOnMap
        { get { return this.k; } set { this.k = value; } }
        /// <summary>
        /// Is Racer Destroyed (like knockout)
        /// </summary>
        public Boolean RacerDestroyed
        { get { return this.l; } set { this.l = value; } }
        /// <summary>
        /// Lap times
        /// </summary>
        public List<float> RacerLapTimes
        {
            get { return n; }
            set { n = value; }
        }
        /// <summary>
        /// Is Racer going wrong way?
        /// </summary>
        public Boolean RacerWrongWay
        { get { return this.m; } set { this.m = value; } }

		public Boolean RacerOutOfBound
		{ get { return this.o; } set { this.o = value; } }
        /// <summary>
        /// Initiator
        /// </summary>
        /// <summary>
        /// Lap times
        /// </summary>
        public List<SectorSpeedAndTime> RacerSectorSpeedAndTime
        {
            get { return p; }
            set { p = value; }
        }

        public float RacerSumOfSpeeds
        {
            get
            {
                if (p != null)
                {
                    float ret = 0;
                    foreach (SectorSpeedAndTime sp in p)
                    {
                        ret += sp.SectorSpeed;
                    }

                    return ret;
                }
                else
                {
                    return 0;
                }
            }
        
        }

        public float RacerHighestSpeed
        {
            get
            {
                if (p != null)
                {
                    float ret = 0;
                    foreach (SectorSpeedAndTime sp in p)
                    {
                        if (sp.SectorSpeed > ret)
                        {
                            ret = sp.SectorSpeed;
                        }
                    }
                    return ret;
                }
                else
                {
                    return 0;
                }
            }
        }
        public Racer_Detail()
        {
            n = new List<float>();
        }
        /// <summary>
        /// Comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Racer_Detail other)
        {
            return this.RacerDistance.CompareTo(other.RacerDistance);
        }
        public int CompareToHighSpeed(Racer_Detail other)
        {
            return this.RacerHighestSpeed.CompareTo(other.RacerHighestSpeed);
        }
        public int CompareToSpeedSum(Racer_Detail other)
        {
            return this.RacerSumOfSpeeds.CompareTo(other.RacerSumOfSpeeds);
        }

        public class SectorSpeedAndTime
        {
            public int Lap {get;set;}
            public float SectorTime {get;set;}
            public float SectorSpeed { get; set; }
        }
    }

}