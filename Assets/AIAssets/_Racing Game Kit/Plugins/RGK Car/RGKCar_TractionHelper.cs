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

    // This script is meant to help set up controllable cars.
    // What it does is to detect situations where the car is fishtailing, and in that case, remove grip 
    // from the front wheels. This will cause the car to slip more in the front then in the rear,
    // thus recovering from the oversteer.

    // This is not quite physically realitic, and may cause the gameplay to feel somewhat more acrade
    // like, but is similar to how ESP systems work in real life
    // (those will just apply the brakes to remove grip from wheels).
            [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Helpers/Traction Helper")]
    public class RGKCar_TractionHelper: MonoBehaviour
    {

        // assign car's front wheels here.
        public RGKCar_Wheel[] front;

        // how strong oversteer is compensated for
        public float compensationFactor = 0.1f;

        // state
        float oldGrip;
        float angle;
        float angularVelo;

        void Start()
        {
            oldGrip = front[0].wheelGrip;
        }

        void Update()
        {
            Vector3 driveDir = transform.forward;
            Vector3 veloDir = GetComponent<Rigidbody>().velocity;
            veloDir -= transform.up * Vector3.Dot(veloDir, transform.up);
            veloDir.Normalize();

            angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(driveDir, veloDir), transform.up));

            angularVelo = GetComponent<Rigidbody>().angularVelocity.y;

            foreach (RGKCar_Wheel w in front)
            {
                if (angle * w.steering < 0)
                    w.wheelGrip = oldGrip * (1.0f - Mathf.Clamp01(compensationFactor * Mathf.Abs(angularVelo)));
                else
                    w.wheelGrip = oldGrip;
            }

        }
    }
}