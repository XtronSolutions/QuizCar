//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// EarthFX Inspector
// Last Change : 3/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor; 
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.RGKCar.CarControllers;
using RacingGameKit.Editors.Helpers;


using RacingGameKit.TouchDrive;

namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(RacingGameKit.TouchDrive.TouchDriveManager))]
    public class TouchDriveInspector : Editor
    {
        RacingGameKit.TouchDrive.TouchDriveManager touchDriveManager;

        public override void OnInspectorGUI()
        {
            touchDriveManager = target as RacingGameKit.TouchDrive.TouchDriveManager;

            EditorGUIUtility.LookLikeInspector();
            CoreFunctions.CreateTouchDriveInspector(touchDriveManager);
        }
    }

}