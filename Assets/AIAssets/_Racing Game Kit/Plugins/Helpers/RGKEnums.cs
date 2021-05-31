using System;
using System.Collections.Generic;

namespace RacingGameKit
{
         /// <summary>
        /// Racetye Enum
        /// </summary>
        public enum RaceTypeEnum
        {
            Sprint = 0, // point to point race 
            Circuit=10, // laps
            //Checkpoint = 20, // checkpoint race 
            TimeAttack=30, // checkpoint time countdown no rival
            LapKnockout=40, // knockout lap based
            //TimeKnockout=50, // knockout time based 
            //CheckpointKnockout=55, // Knockout Checkpoint based
            Speedtrap=60, // speed sum
            //Rally=70
        }

        public enum DriveEnum
        {
            FWD = 0,
            RWD = 1,
            AWD = 2,
            Trailer=3,
        }

        public enum WheelLocationEnum
        { 
            Front =0,
            Rear=1,
            Trailer=2,
        }

        public enum eAIRoadPosition
        {
            UnKnown = 0,
            Right = 1,
            Left = 2,
        }
        public enum eAIRivalPosition
        {
            NoRival = 0,
            Right = 1,
            Left = 2,
        }

        public enum eAIProLevel
        {
            Novice = 0,
            Intermediate = 1,
            Pro = 2,
            Custom = 3,
        }

        public enum eAIBehavior
        {
            Normal = 0,
            UnConfident = 1,
            Aggresive = 2,
        }

        public enum eAIStress
        {
            Normal = 0,
            Relaxed = 1,
            Awared = 2,
            Stressed = 3,
        }

        public enum eAISpawnOrder
        {
            Random = 1,
            Order = 2,
        }

        public enum eAISpawnMode
        {
            Random = 1,
            OneTimeEach = 2,
        }
        public enum eAINamingMode
        {
            Random = 1,
            Order = 2,
            FromRacerRegister = 3,
        }

        public enum eCheckpointItemType
        { 
            Checkpoint=1,
            Sector=2,
            SpeedTrap=3,
        }

        public enum eSpeedTrapMode
        { 
            HighestTotalSpeed=1,
            HighestSpeed=2,
        }
        public enum eSteerSmoothingMode
        { 
            Basic=0,
            FPSBased=1,
        }
} 
