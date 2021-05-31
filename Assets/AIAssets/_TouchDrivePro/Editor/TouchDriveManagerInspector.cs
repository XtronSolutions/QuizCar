//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Manager Inspector 
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TouchDriveManager))]
public class TouchDriveManagerInspector : Editor
{
    TouchDriveManager TouchDriveManager;

    public override void OnInspectorGUI()
    {
        TouchDriveManager = target as TouchDriveManager;
        EditorGUIUtility.LookLikeInspector();
        TouchDriveManagerHelper.CreateTouchDriveInspector(TouchDriveManager);
    }
}
