//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Manager Base
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RacingGameKit.TouchDrive
{
    [AddComponentMenu("")]
    public class TouchDriveManagerBase : MonoBehaviour, iRGKTDM
    {
        public enum TouchItemName
        {
            Wheel = 0,
            Throttle = 1,
            Brake = 2,
            SteerLeft = 3,
            SteerRight = 4,
            ShiftUp = 5,
            ShiftDown = 6,
            CameraButton = 7,
            MirrorButton = 8,
            ResetButton = 9,
            PauseMenu = 10,
            Misc1Button = 11,
            Misc2Button = 13,
        }
        private List<iRGKTouchItem> _TouchItems = new List<iRGKTouchItem>(13);

        public List<iRGKTouchItem> TouchItems
        {
            get { return _TouchItems; }
            set { _TouchItems = value; }
        }

        public TouchItemBase GetTouchItem(TouchItemName TouchItem)
        {
            iRGKTouchItem item = _TouchItems[0];
            switch (TouchItem)
            {
                case TouchItemName.Wheel:
                    item = _TouchItems[0];
                    break;
                case TouchItemName.Throttle:
                    item = _TouchItems[1];
                    break;
                case TouchItemName.Brake:
                    item = _TouchItems[2];
                    break;
                case TouchItemName.SteerLeft:
                    item = _TouchItems[3];
                    break;
                case TouchItemName.SteerRight:
                    item = _TouchItems[4];
                    break;
                case TouchItemName.ShiftUp:
                    item = _TouchItems[5];
                    break;
                case TouchItemName.ShiftDown:
                    item = _TouchItems[6];
                    break;
                case TouchItemName.CameraButton:
                    item = _TouchItems[7];
                    break;
                case TouchItemName.MirrorButton:
                    item = _TouchItems[8];
                    break;
                case TouchItemName.ResetButton:
                    item = _TouchItems[9];
                    break;
                case TouchItemName.PauseMenu:
                    item = _TouchItems[10];
                    break;
                case TouchItemName.Misc1Button:
                    item = _TouchItems[11];
                    break;
                case TouchItemName.Misc2Button:
                    item = _TouchItems[12];
                    break;
            }
            return (TouchItemBase)item;
        }
        /// <summary>
        /// Enable-Disable driving options managed by TouchDrive 
        /// </summary>
        public bool _EnableDrivingOptions = false;
        /// <summary>
        /// Enable-Disable AutoThrottle
        /// </summary>
        public bool _EnableAutoThrottle = false;
        /// <summary>
        /// Enable-Disable AutoBrake
        /// </summary>
        public bool _EnableAutoBrake = false;
        /// <summary>
        /// Enable-Disable Auto Shifting
        /// </summary>
        public bool _EnableAutoGear = false;
        /// <summary>
        /// Enable-Disable Tilt Steer
        /// </summary>
        public bool _EnableTiltSteer = false;
        /// <summary>
        /// Enable-Dsiable TouchWheel Steer
        /// </summary>
        public bool _EnableTouchWheelSteer = false;
        /// <summary>
        /// Enable-Disable Button Steer 
        /// </summary>
        public bool _EnableButtonSteer = false;
        /// <summary>
        /// Enable Axis Change for Different Devices
        /// </summary>
        public bool _UseXAxis = false;
        /// <summary>
        /// Invert axix directions (-y  to +y)
        /// </summary>
        public bool _InvertAxis = false;
        /// <summary>
        /// Steer sensitiviti for your steering calculations
        /// </summary>
        public float _SteerSensitivity = 1;


        public bool EnableAutoThrottle
        {
            get { return _EnableAutoThrottle; }
        }

        public bool EnableAutoBrake
        {
            get { return _EnableAutoBrake; }
        }

        public bool EnableAutoGear
        {
            get { return _EnableAutoGear; }
        }

        public bool EnableTiltSteer
        {
            get { return _EnableTiltSteer; }
        }

        public bool EnableButtonSteer
        {
            get { return _EnableButtonSteer; }
        }

        public bool EnableTouchWheelSteer
        {
            get { return _EnableTouchWheelSteer; }
        }
        public bool UseXAxis
        {
            get { return _UseXAxis; ;}
            set { _UseXAxis = value; }
        }

        public bool InvertAxis
        {
            get { return _InvertAxis; }
            set { _InvertAxis = value; }
        }

        public float SteeringSensitivity
        {
            get { return _SteerSensitivity; }
            set { _SteerSensitivity = value; }
        }


        public bool EnableDrivingOptions
        {
            get { return _EnableDrivingOptions; }
        }
    }

    public interface iRGKTDM
    {
        List<iRGKTouchItem> TouchItems { get; set; }
        bool EnableDrivingOptions { get; }
        bool EnableAutoThrottle { get; }
        bool EnableAutoBrake { get; }
        bool EnableAutoGear { get; }
        bool EnableTiltSteer { get; }
        bool EnableButtonSteer { get; }
        bool EnableTouchWheelSteer { get; }
        bool UseXAxis { get; set; }
        bool InvertAxis { get; set; }
        float SteeringSensitivity { get; set; }
    }
}