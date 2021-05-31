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
using RacingGameKit.Editors.Helpers;


namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(EarthFX))]
    public class EarthFXIspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            CoreFunctions.CreateEarthFXInspector(target as EarthFX);
            serializedObject.ApplyModifiedProperties();
         
        }



    }

}