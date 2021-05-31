//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// AI Awareness for Human Racers Script.
// Last Change : 22/03/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Interfaces;
using RacingGameKit.Helpers;
using SmartAssembly.Attributes;
using System;

namespace RacingGameKit.Racers
{
    [AddComponentMenu("Racing Game Kit/Racers/RGK Racer - AI Awareness for Human Racer")]
    [DoNotObfuscate()]
    public class RGK_Racer_AI_Awareness : MonoBehaviour, IRGKRacer
    {
        private float CarSpeedKm = 0;
        private Transform myTransform;
        private float VehicleSpeed = 0;
        [DoNotObfuscate]
        public bool DrawCollisonGizmo = true;
        [DoNotObfuscate]
        public float DedectionRadius = 2f;

        void Awake()
        {

            if (myTransform == null)
            {
                myTransform = transform.GetComponent<Transform>();
                if (myTransform == null)
                {
                    myTransform = transform.parent.GetComponent<Transform>();
                }
            }
        }
        void Update()
        {
            CarSpeedKm = Mathf.Round(GetComponent<Rigidbody>().velocity.magnitude * 3.6f);
        }

        public float Speed
        {
            get
            {
                return CarSpeedKm;
            }
            set
            { }
        }

        public Transform CachedTransform
        {
            get { return this.transform; }
        }

        public GameObject CachedGameObject
        {
            get { return this.gameObject; }
        }

        public eAIRoadPosition CurrentRoadPosition
        {
            get
            {
                return eAIRoadPosition.UnKnown;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return myTransform.forward * VehicleSpeed;
            }
        }

        public Vector3 Position
        {
            get
            {
                return myTransform.position;
            }
        }

        void OnDrawGizmos()
        {
            if (DrawCollisonGizmo)
            {
                if (myTransform == null)
                {
                    myTransform = this.transform;
                }
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Position, DedectionRadius);
            }
        }


    }
}
