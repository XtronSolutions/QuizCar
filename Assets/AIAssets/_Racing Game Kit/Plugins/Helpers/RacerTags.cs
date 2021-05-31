//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racer Tag Scripts
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Helpers/Racer Tags")]
    public class Racer_Tags : MonoBehaviour
    {
        private GameObject _GameCamera;
        void Start()
        {
            _GameCamera = GameObject.Find("_GameCamera");
            if (_GameCamera == null)
            { 
               
            }
        }
        void Update()
        {
            if (_GameCamera != null)
            {
                transform.LookAt(transform.position + _GameCamera.transform.rotation * Vector3.forward);
            }
        }

    }
}