//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// SpawnPoint Item Inspector
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using UnityEditor;

using RacingGameKit.Editors.Helpers;

namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(RacingGameKit.StartPointItem))]
    public class StartPointItemInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            RacingGameKit.StartPointItem SPItem = target as RacingGameKit.StartPointItem;
            SPItem.name = SPItem.name;


            CoreFunctions.CreateStartPointInspector(SPItem);
               
        }
    }
}
