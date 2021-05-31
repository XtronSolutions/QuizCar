//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// CheckPoint Item Inspector
// Last Change : 08/25/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor; 
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Editors.Helpers;


namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(CheckPointItem))]
    public class CheckPointItemInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            CheckPointItem CPItem = target as CheckPointItem;
            CPItem.name = CPItem.name;

            CoreFunctions.CreateCheckPointItemInspector(CPItem);
          
        }



    }

}