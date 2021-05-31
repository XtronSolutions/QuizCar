//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle AntiRoll Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;


    [AddComponentMenu("Racing Game Kit/Basic Car/RGK Basic Car Anti Roll")]
public class RGK_BasicCar_AntiRoll : MonoBehaviour
    {

        public float AntiRoll = 5000f;
        public WheelCollider WheelLeft;
        public WheelCollider WheelRight;


        void FixedUpdate()
        {
            if (this.enabled)
            {
                WheelHit hit = new WheelHit();
                float num = 1f;
                float num2 = 1f;
                bool groundHit = this.WheelLeft.GetGroundHit(out hit);
                if (groundHit)
                {
                    num = (-this.WheelLeft.transform.InverseTransformPoint(hit.point).y - this.WheelLeft.radius) / this.WheelLeft.suspensionDistance;
                }
                bool flag2 = this.WheelRight.GetGroundHit(out hit);
                if (flag2)
                {
                    num2 = (-this.WheelRight.transform.InverseTransformPoint(hit.point).y - this.WheelRight.radius) / this.WheelRight.suspensionDistance;
                }
                float num3 = (num - num2) * this.AntiRoll;
                if (groundHit)
                {
                    this.GetComponent<Rigidbody>().AddForceAtPosition((Vector3)(this.WheelLeft.transform.up * -num3), this.WheelLeft.transform.position);
                }
                if (flag2)
                {
                    this.GetComponent<Rigidbody>().AddForceAtPosition((Vector3)(this.WheelRight.transform.up * num3), this.WheelRight.transform.position);
                }
            }
        }

    }
