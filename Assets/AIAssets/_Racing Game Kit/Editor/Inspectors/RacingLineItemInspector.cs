//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// WayPoint Item Inspector
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
    [CustomEditor(typeof(RacingLineItem)), CanEditMultipleObjects]
    public class RacingLineItemInspector : Editor
    {
        Vector3 transformPosition;
        RacingLineItem RLItem;
        Color GizmoColor = Color.green;

        public  void OnEnable()
        {

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUIUtility.LookLikeInspector();

            RLItem = target as RacingLineItem;
            RLItem.name = RLItem.name;//Debug warning removal
            GizmoColor = RLItem.GizmoColor;
            transformPosition = RLItem.transform.position;

            CoreFunctions.CreateRGKRacingLinetItemInspector(RLItem);
            
            
        }

        public void OnSceneGUI()
        {
            if (RLItem != null)
            {
                GUIStyle ogui = new GUIStyle();
                ogui.normal.textColor = GizmoColor;
               
                Handles.Label(RLItem.transform.position + new Vector3(0.5f, 0.5f), "Racing Line Node :" + RLItem.name  ,ogui);

                CoreFunctions.CreateRGKRacingLineItemScreenInspector(RLItem);

            }

        }
        
    }
}