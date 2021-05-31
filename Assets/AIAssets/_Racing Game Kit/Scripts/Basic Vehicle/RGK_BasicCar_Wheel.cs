//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Wheel Alignment Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================



using System;
using UnityEngine;
using System.Collections;


    [AddComponentMenu("Racing Game Kit/Basic Car/RGK Basic Car Wheel")]
    public class RGK_BasicCar_Wheel : MonoBehaviour
    {
        public WheelCollider CorrespondingCollider;
        private GameObject SlipPrefab;
        private float RotationValue = 0.0f;

        void Start()
        {
            SlipPrefab = GameObject.Find("_Skidmarks");
        }
         
        void Update()
        { 
            RaycastHit hit;

            Vector3 ColliderCenterPoint = CorrespondingCollider.transform.TransformPoint(CorrespondingCollider.center);

            if (Physics.Raycast(ColliderCenterPoint, -CorrespondingCollider.transform.up, out hit, CorrespondingCollider.suspensionDistance + CorrespondingCollider.radius))
            {
                transform.position = hit.point + (CorrespondingCollider.transform.up * CorrespondingCollider.radius);
            }
            else
            {
                transform.position = ColliderCenterPoint - (CorrespondingCollider.transform.up * CorrespondingCollider.suspensionDistance);
            }

            transform.rotation = CorrespondingCollider.transform.rotation * Quaternion.Euler(RotationValue, CorrespondingCollider.steerAngle, 0);
            RotationValue += CorrespondingCollider.rpm * (360 / 60) * Time.deltaTime;

            WheelHit CorrespondingGroundHit;
            CorrespondingCollider.GetGroundHit(out CorrespondingGroundHit);

         
            //if (Mathf.Abs(CorrespondingGroundHit.sidewaysSlip) > 2.0f)
            //{
            //    if (SlipPrefab)
            //    {
            //        Instantiate(SlipPrefab, CorrespondingGroundHit.point, Quaternion.identity);
            //    }
            //}
        }
    }
