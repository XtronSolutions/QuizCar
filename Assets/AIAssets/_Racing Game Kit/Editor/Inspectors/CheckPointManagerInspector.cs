//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// CheckPoint Manager Inspector
// Last Change : 08/25/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using UnityEditor;
using RacingGameKit;
using RacingGameKit.Editors.Helpers;
using System.Collections.Generic;

namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(CheckPointManager))]
    public class CheckPointManagerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            CheckPointManager CPManager = target as CheckPointManager;
            CPManager.name = CPManager.name;

            CoreFunctions.CreateCheckPointManagerInspector(CPManager);
             
        }


    }

}