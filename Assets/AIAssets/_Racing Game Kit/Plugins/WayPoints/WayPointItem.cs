//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// WayPoint Item Script
// Last Change : 28/08/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Waypoint/Waypoint Item"), ExecuteInEditMode()]
    public class WayPointItem : ItemBase
    {
        /// <summary>
        /// SoftBrake Speed is a lower value of breaking start speed.
        /// In example, if the car speed 100km but the corner requires to 80km, car just need a short and soft brake for slowing car. 
        /// but if the car speed 150km and the corner requires 80km, break must be harder.
        /// Leave 0 (zero) for non brakespeed required waypoints
        /// This value is as Kilometers
        /// </summary>
        public float SoftBrakeSpeed = 0f;
        /// <summary>
        /// HardBrake Speed is higher value of braking start speed. This for second scenario i mentioned above.
        /// Leave 0 (zero) for non brakespeed required waypoints
        /// This value is as Kilometers
        /// </summary>
        public float HardBrakeSpeed = 0f;
        /// <summary>
        /// When value seto true, it allows to define widers sepe
        /// </summary>
         
        [HideInInspector]
        [SerializeField]
        [DoNotObfuscate]
        public bool SeperatedWiders = false;
        /// <summary>
        /// Left wider width
        /// </summary>
        /// 
        [HideInInspector]
        public float LeftWide = 2f;
        /// <summary>
        /// Right wider width
        /// </summary>
        [HideInInspector]
        public float RightWide = 2f;

        [HideInInspector]
        [DoNotObfuscate]
        public Vector3 LeftLine;

        [DoNotObfuscate]
        [HideInInspector]
        public Vector3 RightLine;

        [HideInInspector]
        public bool ShowIconGizmo = true;

        void OnDrawGizmos()
        {
            if (ShowIconGizmo)
            {
                Gizmos.DrawIcon(transform.position, "icon_waypoint.tif");
            }
            Gizmos.color = Color.red;

            if (!SeperatedWiders)
            {
                LeftWide = RightWide;
            }

            if (SoftBrakeSpeed > 0 && HardBrakeSpeed <= (SoftBrakeSpeed + 10))
            {
                HardBrakeSpeed = SoftBrakeSpeed + 10;
            }

            Vector3 RotatedLineBase = transform.position + (transform.right * RightWide);
            RightLine = Quaternion.AngleAxis(base.transform.rotation.y, transform.up) * RotatedLineBase;
            RightLine = Quaternion.AngleAxis(base.transform.rotation.x, transform.right) * RotatedLineBase;
            RightLine = Quaternion.AngleAxis(base.transform.rotation.z, transform.right) * RotatedLineBase;

            Vector3 RotatedLine2Base = transform.position + (transform.right * LeftWide * -1);
            LeftLine = Quaternion.AngleAxis(base.transform.rotation.y, transform.up) * RotatedLine2Base;
            LeftLine = Quaternion.AngleAxis(base.transform.rotation.x, transform.right * -1) * RotatedLine2Base;
            LeftLine = Quaternion.AngleAxis(base.transform.rotation.z, transform.right * -1) * RotatedLine2Base;

            Gizmos.DrawLine(base.transform.position, RightLine);
            Gizmos.DrawLine(base.transform.position, LeftLine);
        }
    }
}