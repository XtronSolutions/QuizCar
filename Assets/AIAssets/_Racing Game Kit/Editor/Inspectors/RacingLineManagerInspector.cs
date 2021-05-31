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
 
[CustomEditor(typeof(RacingLineManager))]
public class RacingLineManagerInspector : Editor
{
    RacingLineManager RLItemManager;
	public override void OnInspectorGUI() {
		//get the selected object the inspector is revealing
        RLItemManager = target as RacingLineManager;
        EditorGUIUtility.LookLikeInspector();

        CoreFunctions.CreateRGKRacingLineManagerInspector(RLItemManager);
	}

    public void OnSceneGUI()
    {
        if (RLItemManager != null)
        {
            
        }

    }
}
 