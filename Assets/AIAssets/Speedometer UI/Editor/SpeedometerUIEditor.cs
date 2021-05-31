using UnityEngine;
using UnityEditor;

using System.IO;
using System.Collections.Generic;
using System;


[CustomEditor(typeof(SpeedometerUI))]
public class SpeedometerUIEditor : Editor
{
    private static Texture2D texSpeedoLogoForInspectors = null;

    SpeedometerUI SpeedometerUIComponent;
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
         SpeedometerUIComponent= target as SpeedometerUI;
        SpeedometerUIComponent.name = SpeedometerUIComponent.name; // Debug Warning Removal



        GUILayout.Space(10);
        GUILayoutOption[] options = new GUILayoutOption[] { };

        //Logo
        GUILayout.BeginHorizontal(options);
        GUILayout.Space((Screen.width - 310) / 2);
        GUILayout.Box(LogoForInspector, GUIStyle.none, GUILayout.Width(310), GUILayout.Height(60));
        GUILayout.EndHorizontal();
        //logo end


        Color defaultColor = GUI.backgroundColor;

        GUILayout.BeginVertical("Box");
        EditorGUI.indentLevel = -1;
        EditorGUILayout.LabelField("Speedometer UI v1.2 ", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        SpeedometerUIComponent.ConfigurationAsset = EditorGUILayout.ObjectField("Configuration File :", SpeedometerUIComponent.ConfigurationAsset, typeof(TextAsset),false) as TextAsset;
        EditorGUILayout.Space();
        GUI.backgroundColor = Color.red;
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (SpeedometerUIComponent.ConfigurationAsset == null) GUI.enabled = false;
        if (GUILayout.Button("Load Settings", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(100), GUILayout.Height(20)))
        {
            SpeedometerUIComponent.loadJSONData();
        }

        if (GUILayout.Button("Save Settings", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(100), GUILayout.Height(20)))
        {
            if (EditorUtility.DisplayDialog("Confirm Overwrite",
                    "Are you sure you want to overwrite to " + SpeedometerUIComponent.ConfigurationAsset.name + " file?", "YES", "NO"))
            {
                JsonObjectForKit oJ = SpeedometerUIComponent.buildJSONData();
                if (oJ != null)
                {
                    bool blnRes = WriteJSONFile(SpeedometerUIComponent.ConfigurationAsset, oJ);

                    if (blnRes)
                    {
                        Debug.Log("SPEEDOMETER UI INFORMATION\r\nConfiguration file saved succesfuly!");
                    }
                }

            }

        }

        GUI.enabled = true;

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        SpeedometerUIComponent.LoadSettingsFromFile = EditorGUILayout.Toggle("Load Settings From File On Start", SpeedometerUIComponent.LoadSettingsFromFile);

        GUILayout.EndVertical();
        EditorGUILayout.Space();
        GUI.backgroundColor = defaultColor;

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Control Settings", EditorStyles.boldLabel);
        SpeedometerUIComponent.ControlName = EditorGUILayout.TextField("Control Name", SpeedometerUIComponent.ControlName);
        SpeedometerUIComponent.Depth = EditorGUILayout.IntField("Control Depth", SpeedometerUIComponent.Depth);
        GUILayout.EndVertical();
        EditorGUILayout.Space();


        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Gauge Settings", EditorStyles.boldLabel);
        SpeedometerUIComponent.GaugeDial = (Texture2D)(EditorGUILayout.ObjectField("Gauge Dial Texture", SpeedometerUIComponent.GaugeDial, typeof(Texture2D), false));
        SpeedometerUIComponent.GaugeSize = EditorGUILayout.Vector2Field("Gauge Size", SpeedometerUIComponent.GaugeSize);
        SpeedometerUIComponent.GaugePositionOffset = EditorGUILayout.Vector2Field("Gauge Position Offset", SpeedometerUIComponent.GaugePositionOffset);
        SpeedometerUIComponent.GaugePosition = (SpeedometerUI.GaugePositionEnum)EditorGUILayout.EnumPopup("Gauge Position", SpeedometerUIComponent.GaugePosition);
        SpeedometerUIComponent.SpeedoSkin = (GUISkin)(EditorGUILayout.ObjectField("Skin", SpeedometerUIComponent.SpeedoSkin, typeof(GUISkin), false));
        GUILayout.EndVertical();
        EditorGUILayout.Space();


        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Speedo Settings", EditorStyles.boldLabel);
        SpeedometerUIComponent.SpeedoNeedleTexture = (Texture2D)(EditorGUILayout.ObjectField("Needle Texture", SpeedometerUIComponent.SpeedoNeedleTexture, typeof(Texture2D), false));
        SpeedometerUIComponent.SpeedoNeedleSize = EditorGUILayout.Vector2Field("Needle Size", SpeedometerUIComponent.SpeedoNeedleSize);
        SpeedometerUIComponent.SpeedoNeedleOffset = EditorGUILayout.Vector2Field("Needle Offset", SpeedometerUIComponent.SpeedoNeedleOffset);
        SpeedometerUIComponent.SpeedoNeedleCenterDistance = EditorGUILayout.FloatField("Needle Distance to Center", SpeedometerUIComponent.SpeedoNeedleCenterDistance);
        SpeedometerUIComponent.SpeedoNeedleMinValueAngle = EditorGUILayout.FloatField("Min Value Angle", SpeedometerUIComponent.SpeedoNeedleMinValueAngle);
        SpeedometerUIComponent.SpeedoNeedleMaxValueAngle = EditorGUILayout.FloatField("Max Value Angle", SpeedometerUIComponent.SpeedoNeedleMaxValueAngle);
        SpeedometerUIComponent.SpeedoNeedleMaxValue = EditorGUILayout.FloatField("Max Value", SpeedometerUIComponent.SpeedoNeedleMaxValue);

        SpeedometerUIComponent.SpeedoCapTexture = (Texture2D)(EditorGUILayout.ObjectField("Needle Cap Texture", SpeedometerUIComponent.SpeedoCapTexture, typeof(Texture2D), false));
        SpeedometerUIComponent.SpeedoCapSize = EditorGUILayout.Vector2Field("Needle Cap Size", SpeedometerUIComponent.SpeedoCapSize);
        GUILayout.EndVertical();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("RPM Settings", EditorStyles.boldLabel);
        SpeedometerUIComponent.EnableRPM = EditorGUILayout.Toggle("Enable RPM", SpeedometerUIComponent.EnableRPM);
        if (SpeedometerUIComponent.EnableRPM)
        {
            SpeedometerUIComponent.RPMNeedleTexture = (Texture2D)(EditorGUILayout.ObjectField("Needle Texture", SpeedometerUIComponent.RPMNeedleTexture, typeof(Texture2D), false));
            SpeedometerUIComponent.RPMNeedleSize = EditorGUILayout.Vector2Field("Needle Size", SpeedometerUIComponent.RPMNeedleSize);
            SpeedometerUIComponent.RPMNeedleOffset = EditorGUILayout.Vector2Field("Needle Offset", SpeedometerUIComponent.RPMNeedleOffset);
            SpeedometerUIComponent.RPMNeedleCenterDistance = EditorGUILayout.FloatField("Needle Distance to Center", SpeedometerUIComponent.RPMNeedleCenterDistance);
            SpeedometerUIComponent.RPMNeedleMinValueAngle = EditorGUILayout.FloatField("Min Value Angle", SpeedometerUIComponent.RPMNeedleMinValueAngle);
            SpeedometerUIComponent.RPMNeedleMaxValueAngle = EditorGUILayout.FloatField("Max Value Angle", SpeedometerUIComponent.RPMNeedleMaxValueAngle);
            SpeedometerUIComponent.RPMNeedleMaxValue = EditorGUILayout.FloatField("Max Value", SpeedometerUIComponent.RPMNeedleMaxValue);

            SpeedometerUIComponent.RPMCapTexture = (Texture2D)(EditorGUILayout.ObjectField("Needle Cap Texture", SpeedometerUIComponent.RPMCapTexture, typeof(Texture2D), false));
            SpeedometerUIComponent.RPMCapSize = EditorGUILayout.Vector2Field("Needle Cap Size", SpeedometerUIComponent.RPMCapSize);
        }
        GUILayout.EndVertical();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Text Settings", EditorStyles.boldLabel);

        SpeedometerUIComponent.ShowSpeedText = EditorGUILayout.Toggle("Show Speed Text", SpeedometerUIComponent.ShowSpeedText);
        SpeedometerUIComponent.SpeedTextOffset = EditorGUILayout.Vector2Field("Speed Text Offset", SpeedometerUIComponent.SpeedTextOffset);
        SpeedometerUIComponent.ShowSpeedUnitText = EditorGUILayout.Toggle("Show Speed Unit Text", SpeedometerUIComponent.ShowSpeedUnitText);
        SpeedometerUIComponent.SpeedUnitTextOffset = EditorGUILayout.Vector2Field("Speed Unit Text Offset", SpeedometerUIComponent.SpeedUnitTextOffset);
        EditorGUILayout.Space();
        SpeedometerUIComponent.ShowRPMText = EditorGUILayout.Toggle("Show RPM Text", SpeedometerUIComponent.ShowRPMText);
        SpeedometerUIComponent.RPMTextOffset = EditorGUILayout.Vector2Field("RPM Text Offset", SpeedometerUIComponent.RPMTextOffset);
        EditorGUILayout.Space();
        SpeedometerUIComponent.ShowGearText = EditorGUILayout.Toggle("Show Gear Text", SpeedometerUIComponent.ShowGearText);
        SpeedometerUIComponent.GearTextOffset = EditorGUILayout.Vector2Field("Gear Text Offset", SpeedometerUIComponent.GearTextOffset);

        GUILayout.EndVertical();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);
        SpeedometerUIComponent.Speed = EditorGUILayout.FloatField("Speed", SpeedometerUIComponent.Speed);
        SpeedometerUIComponent.RPM = EditorGUILayout.FloatField("RPM", SpeedometerUIComponent.RPM);
        SpeedometerUIComponent.Gear = EditorGUILayout.TextField("Gear", SpeedometerUIComponent.Gear);
        GUILayout.EndVertical();
        EditorGUILayout.Space();

       

        //GUILayout.BeginVertical("Box");
        //EditorGUILayout.LabelField("Speedometer UI Settings", EditorStyles.boldLabel);
        //EditorGUILayout.Space();
        //EditorGUI.indentLevel = 0;
        //DrawDefaultInspector();
        //EditorGUI.indentLevel = 1;
        //GUILayout.EndVertical();

        if (GUI.changed)
        {
            SpeedometerUIComponent.RefreshScreenPosition();
            EditorUtility.SetDirty(SpeedometerUIComponent);
        }

        
    }
     
     
    private bool WriteJSONFile(TextAsset ConfigurationFile, JsonObjectForKit Data)
    {
        try
        {

            FileInfo oFile = new FileInfo(ConfigurationFile.name);
            string strPath = oFile.Directory + "\\" + AssetDatabase.GetAssetPath(ConfigurationFile);
            if (IsOnMac())
            {
                strPath = strPath.Replace("\\", "/");
            }

            using (StreamWriter writer = new StreamWriter(strPath, false))
            {
                writer.Write(Data.ToString());
            }

            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(ConfigurationFile), typeof(TextAsset));
            return true;
        }
        catch (Exception _ex) { Debug.Log(_ex.Message); return false; }

    }

    private bool IsOnMac()
    {
        string strDataPath = Application.dataPath;
        if (strDataPath.Contains("\\"))
        { return false; }
        else
        { return true; }
    }

    public static Texture2D LogoForInspector
    {
        get
        {
            if (texSpeedoLogoForInspectors == null)
            {
                texSpeedoLogoForInspectors = new Texture2D(310, 60, TextureFormat.ARGB32, false);
                texSpeedoLogoForInspectors.anisoLevel = 0;
                texSpeedoLogoForInspectors.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAATYAAAA8CAYAAADsUJZ7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA2ZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDowMkFBMjQyQ0ZGQjdFMjExQUY1QkJBNzU2RjE4REU3RiIgeG1wTU06RG9jdW1lbnRJRD0ieG1wLmRpZDpDQkVGREJDQ0JCMDUxMUUyQkU4M0VBNTdGNEM2M0Y5NCIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpDQkVGREJDQkJCMDUxMUUyQkU4M0VBNTdGNEM2M0Y5NCIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgQ1M2IChXaW5kb3dzKSI+IDx4bXBNTTpEZXJpdmVkRnJvbSBzdFJlZjppbnN0YW5jZUlEPSJ4bXAuaWlkOkI1NkZDNzIxNEVCQUUyMTE4RDIwQ0IwNThGMTAxRUUwIiBzdFJlZjpkb2N1bWVudElEPSJ4bXAuZGlkOjAyQUEyNDJDRkZCN0UyMTFBRjVCQkE3NTZGMThERTdGIi8+IDwvcmRmOkRlc2NyaXB0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"));
            }
            return texSpeedoLogoForInspectors;
        }
    }

}




