//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// RGKCar Sway Bar Script
// Last Change : 13/03/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar
{
    // This class simulates an anti-roll bar.
    // Anti roll bars transfer suspension compressions forces from one wheel to another.
    // This is used to minimize body roll in corners, and improve grip by balancing the wheel loads.
    // Typical modern cars have one anti-roll bar per axle.
     [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Helpers/Sway Bar")]
    public class RGKCar_SwayBar : MonoBehaviour
    {

        // The two wheels connected by the anti-roll bar. These should be on the same axle.
        public RGKCar_Wheel wheel1;
        public RGKCar_Wheel wheel2;

        // Coeefficient determining how much force is transfered by the bar.
        public float coefficient = 5000;

        void FixedUpdate()
        {
            return;
            if (wheel1 != null && wheel2 != null && this.enabled)
            {
                float force = (wheel1.compression - wheel2.compression) * coefficient;
                wheel1.suspensionForceInput = +force;
                wheel2.suspensionForceInput = -force;
            }
            
        }
    }
}