//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// FinishPoint Item Inspector
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
    [CustomEditor(typeof(FinishPointItem))]
    public class FinishPointItemInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            //get the selected object the inspector is revealing
            FinishPointItem FPItem = target as FinishPointItem;
            FPItem.name = FPItem.name;

            CoreFunctions.CreateFinishPointInspector(FPItem);

        }

    }

}