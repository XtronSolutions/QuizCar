//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// SpawnPoint Item Inspector
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
    [CustomEditor(typeof(SpawnPointItem))]
    public class SpawnPointItemInspector : Editor
    {
        SpawnPointItem SPItem;
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.LookLikeInspector();
            //get the selected object the inspector is revealing
            SPItem = target as SpawnPointItem;
            SPItem.name = SPItem.name;

            CoreFunctions.CreateRGKSpawnPointInspector(SPItem);
        }

        public void OnSceneGUI()
        {
            if (SPItem != null)
            {
                Handles.color = Color.white;
                Handles.Label(SPItem.transform.position + new Vector3(0.5f, 0.5f), "Spawnpoint :" + SPItem.name);

                CoreFunctions.CreateRGKSpawnPointScreenInspector(SPItem);
            }

        }
    }

}