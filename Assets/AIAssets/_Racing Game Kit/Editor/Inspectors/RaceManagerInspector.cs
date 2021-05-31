//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// RaceManager Inspector
// Last Change : 3/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using UnityEditor;
using RacingGameKit;
using RacingGameKit.Editors.Helpers;
using System.Reflection;
using System.Diagnostics;

namespace RacingGameKit.Editors
{


    [CustomEditor(typeof(Race_Manager))]
    public class RaceManagerInspector : Editor
    {
        [HideInInspector]
        public SerializedObject seriObject;

        void OnEnable()
        {
            seriObject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();

            Race_Manager GManager = target as Race_Manager;
            GManager.name = GManager.name;

            CoreFunctions.CreateRGKRaceManagerInspector(GManager, seriObject);

            seriObject.ApplyModifiedProperties();
        }
    }

}