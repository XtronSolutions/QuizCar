//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Game Camera Script
// Last Change : 20/06/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Interfaces;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Camera/RGK Camera Position")]
    public class RGK_CameraPositionHelper : MonoBehaviour
    {
        public float FieldOfValue = 60f;
        public bool BackCamera = false;
        public bool AllowLookSides = false;
        //private bool Shake = false;
         
    }
}