//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Manager Inspector Helper Script
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

internal static class TouchDriveManagerHelper 
{
    private static Texture2D texTouchDriveLogoForInspectors = null;

    internal static void CreateTouchDriveInspector(TouchDriveManager TDManager)
    {
        int iAlignmentSpace = (Screen.width - 310) / 2;
       
        GUILayout.Space(10);
        GUILayoutOption[] options = new GUILayoutOption[] { };

        //Logo
        GUILayout.BeginHorizontal(options);
        GUILayout.Space(iAlignmentSpace);
        GUILayout.Box(TouchDriveLogo, GUIStyle.none, GUILayout.Width(310), GUILayout.Height(60));
        GUILayout.EndHorizontal();
        //logo end
        EditorGUILayout.Space();


        CreateVersionHeader("Touch Drive Pro v1.0");
        GUI.changed = false;
        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Control Templates", EditorStyles.boldLabel);
        TDManager.ControlTemplate = (TouchDriveManager.eControlTemplate)EditorGUILayout.EnumPopup("Current Template", TDManager.ControlTemplate);
        if (TDManager.ControlTemplate != TDManager.oldTemplate)
        {
            TDManager.oldTemplate = TDManager.ControlTemplate;
            TDManager.SwitchTemplate(TDManager.ControlTemplate);
        }
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Vehicle Controls", EditorStyles.boldLabel);
        UnityEngine.Object throttleObject = EditorGUILayout.ObjectField("Throttle", (UnityEngine.Object)TDManager.Throttle, typeof(TouchItemBase), true);
        TDManager.Throttle = throttleObject as TouchItemBase;

        UnityEngine.Object brakeObject = (TouchItemBase)EditorGUILayout.ObjectField("Brake", (UnityEngine.Object)TDManager.Brake, typeof(TouchItemBase), true);
        TDManager.Brake = brakeObject as TouchItemBase;
        EditorGUILayout.Space();
        TDManager.SteerLeft = (TouchButton)EditorGUILayout.ObjectField("Left Steer", TDManager.SteerLeft, typeof(TouchButton), true);
        TDManager.SteerRight = (TouchButton)EditorGUILayout.ObjectField("Right Steer", TDManager.SteerRight, typeof(TouchButton), true);
        EditorGUILayout.Space();
        TDManager.ShiftUp = (TouchButton)EditorGUILayout.ObjectField("Shift Up", TDManager.ShiftUp, typeof(TouchButton), true);
        TDManager.ShiftDown = (TouchButton)EditorGUILayout.ObjectField("Shift Down", TDManager.ShiftDown, typeof(TouchButton), true);
        EditorGUILayout.Space();
        TDManager.Wheel = (TouchWheel)EditorGUILayout.ObjectField("Touch Wheel", TDManager.Wheel, typeof(TouchWheel), true);
        EditorGUILayout.Space();
        GUILayout.EndVertical();

        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Misc Controls", EditorStyles.boldLabel);
        TDManager.CameraButton = (TouchButton)EditorGUILayout.ObjectField("Change Camera", TDManager.CameraButton, typeof(TouchButton), true);
        TDManager.MirrorButton = (TouchButton)EditorGUILayout.ObjectField("Mirror/Show Back Camera", TDManager.MirrorButton, typeof(TouchButton), true);
        TDManager.ResetButton = (TouchButton)EditorGUILayout.ObjectField("Reset Vehicle", TDManager.ResetButton, typeof(TouchButton), true);
        TDManager.PauseButton = (TouchButton)EditorGUILayout.ObjectField("Pause", TDManager.PauseButton, typeof(TouchButton), true);
        TDManager.Misc1Button = (TouchItemBase)EditorGUILayout.ObjectField("Misc Button 1", TDManager.Misc1Button, typeof(TouchItemBase), true);
        TDManager.Misc2Button = (TouchItemBase)EditorGUILayout.ObjectField("Misc Button 2", TDManager.Misc2Button, typeof(TouchItemBase), true);
        GUILayout.EndVertical();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Control Positions", EditorStyles.boldLabel);
        TDManager.EnableEditControlPositions = EditorGUILayout.Toggle("Edit Control Positions", TDManager.EnableEditControlPositions);
        if (TDManager.EnableEditControlPositions)
        {
            EditorGUILayout.HelpBox("In this table you can edit final positions of the controls. Template changes uses this control points to move buttons to given positions.", MessageType.Warning, true);
            EditorGUILayout.BeginHorizontal();
            TDManager.showPA = EditorGUILayout.Toggle("", TDManager.showPA, GUILayout.Width(30));
            TDManager.PositionA = (Vector4)EditorGUILayout.Vector4Field("Throttle (Position A)", TDManager.PositionA);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPB1 = EditorGUILayout.Toggle("", TDManager.showPB1, GUILayout.Width(30));
            TDManager.PositionB1 = (Vector4)EditorGUILayout.Vector4Field("Brake (Position B 1)", TDManager.PositionB1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPB2 = EditorGUILayout.Toggle("", TDManager.showPB2, GUILayout.Width(30));
            TDManager.PositionB2 = (Vector4)EditorGUILayout.Vector4Field("Brake (Position B 2)", TDManager.PositionB2);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPC = EditorGUILayout.Toggle("", TDManager.showPC, GUILayout.Width(30));
            TDManager.PositionC = (Vector4)EditorGUILayout.Vector4Field("Throttle or Brake Alone (Position C)", TDManager.PositionC);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPD = EditorGUILayout.Toggle("", TDManager.showPD, GUILayout.Width(30));
            TDManager.PositionD = (Vector4)EditorGUILayout.Vector4Field("Steer Left (Position D)", TDManager.PositionD);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPE = EditorGUILayout.Toggle("", TDManager.showPE, GUILayout.Width(30));
            TDManager.PositionE = (Vector4)EditorGUILayout.Vector4Field("Steer Right (Position E)", TDManager.PositionE);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPF = EditorGUILayout.Toggle("", TDManager.showPF, GUILayout.Width(30));
            TDManager.PositionF = (Vector4)EditorGUILayout.Vector4Field("Touch Wheel (Position F)", TDManager.PositionF);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPG = EditorGUILayout.Toggle("", TDManager.showPG, GUILayout.Width(30));
            TDManager.PositionG = (Vector4)EditorGUILayout.Vector4Field("Shift Up (Position G)", TDManager.PositionG);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPH = EditorGUILayout.Toggle("", TDManager.showPH, GUILayout.Width(30));
            TDManager.PositionH = (Vector4)EditorGUILayout.Vector4Field("Shift Down (Position H)", TDManager.PositionH);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPI = EditorGUILayout.Toggle("", TDManager.showPI, GUILayout.Width(30));
            TDManager.PositionI = (Vector4)EditorGUILayout.Vector4Field("Misc Button 1 (Position I)", TDManager.PositionI);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            TDManager.showPJ = EditorGUILayout.Toggle("", TDManager.showPJ, GUILayout.Width(30));
            TDManager.PositionJ = (Vector4)EditorGUILayout.Vector4Field("Misc Button 2 (Position J)", TDManager.PositionJ);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

        }
        TDManager.FlipPositions = EditorGUILayout.Toggle("Flip Positions", TDManager.FlipPositions);
        if (TDManager.FlipPositions)
        {
            EditorGUILayout.HelpBox("WARNING : Control positions swapped! This means your configured controls will display opposite side of screen. As example, if you configured Throttle at right, with this settings it will display at left!", MessageType.Warning, true);
        }
        if (TDManager.oldFlipPosition != TDManager.FlipPositions)
        {
            TDManager.oldFlipPosition = TDManager.FlipPositions;
            TDManager.SwitchTemplate(TDManager.ControlTemplate);
        }
        GUILayout.EndVertical();
        EditorGUILayout.Space();


        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        TDManager.EnableTouch = EditorGUILayout.Toggle("Enable Touch Inputs", TDManager.EnableTouch);
        if (!TDManager.EnableTouch)
        {
            EditorGUILayout.HelpBox("WARNING : Touch Inputs are not enabled. This means TouchDriveManager will not handled control buttons. This happens moslty when game paused.", MessageType.Warning, true);
        }
        
        TDManager.EnableMouseEmulation = EditorGUILayout.Toggle("Enable Mouse Emulation", TDManager.EnableMouseEmulation);
        EditorGUILayout.Space();

        GUILayout.EndVertical();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Driving Features", EditorStyles.boldLabel);
        TDManager._EnableDrivingOptions = EditorGUILayout.Toggle("Enable Driving Features", TDManager._EnableDrivingOptions);
        EditorGUILayout.HelpBox("This values are set by templates.", MessageType.Info);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Auto Throttle", TDManager._EnableAutoThrottle);
        EditorGUILayout.Toggle("Auto Brake", TDManager._EnableAutoBrake);
        EditorGUILayout.Toggle("Auto Gear Shifting", TDManager._EnableAutoGear);
        EditorGUILayout.Toggle("Tilt Steer", TDManager._EnableTiltSteer);
        EditorGUILayout.Toggle("Button Steer", TDManager._EnableButtonSteer);
        EditorGUILayout.Toggle("Touch Wheel Steer", TDManager._EnableTouchWheelSteer);
        EditorGUILayout.Space();
        EditorGUI.EndDisabledGroup();
        TDManager._UseXAxis=EditorGUILayout.Toggle("Use X Axis", TDManager._UseXAxis);
        TDManager._InvertAxis=EditorGUILayout.Toggle("Invert Axis", TDManager._InvertAxis);
        EditorGUILayout.Space();
        TDManager._SteerSensitivity= EditorGUILayout.Slider("Steer Sensitivity",TDManager._SteerSensitivity,1,10);
        
        GUILayout.EndVertical();
        EditorGUILayout.Space();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(TDManager);
        }

    }

    private static void CreateVersionHeader(string InspectorName)
    {
        int iIndent = EditorGUI.indentLevel;
        Color defaultColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        GUILayout.BeginVertical("Box");
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField(InspectorName, EditorStyles.boldLabel);
        GUILayout.EndVertical();
        EditorGUILayout.Space();
        GUI.backgroundColor = defaultColor;
        EditorGUI.indentLevel = iIndent;
    }

    public static Texture2D TouchDriveLogo
    {
        get
        {
            if (texTouchDriveLogoForInspectors == null)
            {
                texTouchDriveLogoForInspectors = new Texture2D(310, 60, TextureFormat.ARGB32, false);
                texTouchDriveLogoForInspectors.anisoLevel = 0;
                texTouchDriveLogoForInspectors.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAATYAAAA8CAYAAADsUJZ7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDowMkFBMjQyQ0ZGQjdFMjExQUY1QkJBNzU2RjE4REU3RiIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDpBODJBOUQ2MEJBRkQxMUUyQUUwNEU2NzhBMTNDMkRDNiIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpBODJBOUQ1RkJBRkQxMUUyQUUwNEU2NzhBMTNDMkRDNiIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOjI3QkJBMUMwRkJCQUUyMTE5OThCREMzQkE2RjRCOTIwIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjAyQUEyNDJDRkZCN0UyMTFBRjVCQkE3NTZGMThERTdGIi8+IDwvcmRmOkRlc2NyaXB0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"));
            }
            return texTouchDriveLogoForInspectors;
        }
    }
}
