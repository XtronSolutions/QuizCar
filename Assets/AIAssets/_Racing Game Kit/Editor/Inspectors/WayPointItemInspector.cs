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
    [CustomEditor(typeof(WayPointItem)),CanEditMultipleObjects]
    public class WayPointItemInspector : Editor
    {
        public SerializedProperty LeftWide;
        public SerializedProperty RightWide;
        Vector3 transformPosition;
        WayPointItem WPItem;
        Color GizmoColor = Color.red;
        public  void OnEnable()
        {
            LeftWide = serializedObject.FindProperty("LeftWide");
            RightWide = serializedObject.FindProperty("RightWide");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUIUtility.LookLikeInspector();

             WPItem = target as WayPointItem;
            WPItem.name = WPItem.name;//Debug warning removal
            transformPosition = WPItem.transform.position;

            CoreFunctions.CreateRGKWayPointItemInspector(WPItem);
             
            serializedObject.ApplyModifiedProperties();
        }

        public void OnSceneGUI()
        {
            if (WPItem != null)
            {
                GUIStyle ogui = new GUIStyle();
                ogui.normal.textColor = GizmoColor;
                Handles.Label(WPItem.transform.position + new Vector3(0.5f, 0.5f), "Waypoint :" + WPItem.name +
                    "\r\nSoft Brake:" + WPItem.SoftBrakeSpeed + " Km" +
                    "\r\nHard Brake:" + WPItem.HardBrakeSpeed + " Km", ogui);

                CoreFunctions.CreateRGKWayPointItemScreenInspector(WPItem);

                //left wider text
                if (WPItem.RightWide > 2)
                {
                    Vector3 RightLineLabelLocation;
                    Vector3 RotatedLineBase = WPItem.transform.position + (WPItem.transform.right * WPItem.RightWide / 2);
                    RightLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.y, WPItem.transform.up) * RotatedLineBase;
                    RightLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.x, WPItem.transform.right) * RotatedLineBase;
                    RightLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.z, WPItem.transform.right) * RotatedLineBase;
                    Handles.Label(RightLineLabelLocation, WPItem.RightWide.ToString() + "m", ogui);
                }

                if (WPItem.LeftWide > 2)
                {
                    Vector3 LeftLineLabelLocation;
                    Vector3 RotatedLine2Base = WPItem.transform.position + (WPItem.transform.right * WPItem.LeftWide / 2 * -1);
                    LeftLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.y, WPItem.transform.up) * RotatedLine2Base;
                    LeftLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.x, WPItem.transform.right * -1) * RotatedLine2Base;
                    LeftLineLabelLocation = Quaternion.AngleAxis(WPItem.transform.rotation.z, WPItem.transform.right * -1) * RotatedLine2Base;
                    Handles.Label(LeftLineLabelLocation, WPItem.LeftWide.ToString() + "m", ogui);
                }
            }

        }
        
    }
}