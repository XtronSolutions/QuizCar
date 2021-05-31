//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// SpawnPoint Manager Inspector
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using RacingGameKit.Editors.Helpers;


namespace RacingGameKit.Editors
{
    [CustomEditor(typeof(SpawnPointManager))]
    public class SpawnPointManagerInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            //get the selected object the inspector is revealing
            SpawnPointManager SPManager = target as SpawnPointManager;
            SPManager.name = SPManager.name;

            CoreFunctions.CreateRGKSpawnPointManagerInspector(SPManager);

        }
    }
     
}