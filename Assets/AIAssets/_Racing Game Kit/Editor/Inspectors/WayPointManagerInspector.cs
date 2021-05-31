//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// WayPoint Manager Inspector
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor;
using RacingGameKit;
using RacingGameKit.Editors.Helpers;
using System.Collections.Generic;

namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(WayPointManager))]
    public class WayPointManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            WayPointManager WPManager = target as WayPointManager;
            WPManager.name = WPManager.name;

            CoreFunctions.CreateRGKWayPointManagerInspector(WPManager);
            
        }

    }

}