﻿//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Pro AI Inspector
// Last Change : 3/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using RacingGameKit.Editors.Helpers;


namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(RacingGameKit.Racers.RGK_Racer_Pro_AI))]
    public class AIProInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();

            CoreFunctions.CreateAIProInspector(target as RacingGameKit.Racers.RGK_Racer_Pro_AI);
            serializedObject.ApplyModifiedProperties();
        }
    }

}