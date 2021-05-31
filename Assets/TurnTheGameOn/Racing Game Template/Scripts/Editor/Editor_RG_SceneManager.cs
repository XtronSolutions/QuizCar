using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RG_SceneManager))]
public class Editor_RG_SceneManager : Editor {

	bool waypoints;
	bool settings;
	bool info;

	public override void OnInspectorGUI(){
		serializedObject.Update();
		EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
		EditorStyles.label.wordWrap = true;


		RG_SceneManager rg_SceneManager = (RG_SceneManager)target;
		rg_SceneManager.transform.hideFlags = HideFlags.HideInInspector;

		rg_SceneManager.managerReference.waypointCircut.hideFlags = HideFlags.HideInInspector;


		Texture racingAssetTexture = Resources.Load("RacingAssetTexture") as Texture;
		GUIStyle inspectorStyle = new GUIStyle(GUI.skin.label);		
		inspectorStyle.fixedWidth = 264;
		inspectorStyle.fixedHeight = 64;
		inspectorStyle.margin = new RectOffset( (int)(( (Screen.width * 0.98f)  - 264) / 2), 0, 0, 0);
		GUILayout.Label(racingAssetTexture,inspectorStyle, GUILayout.MaxWidth(Screen.width * 0.9f));

		EditorGUILayout.BeginVertical("Box");

		EditorGUILayout.BeginHorizontal();

		GUISkin editorSkin = Resources.Load("EditorSkin") as GUISkin;
		GUI.skin = editorSkin;
		editorSkin.button.active.textColor = Color.green;
		if (waypoints) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}

		if (GUILayout.Button ("Wayponts", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			waypoints = true;
			settings = false;
			info = false;
		}

		if (settings) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}
		if (GUILayout.Button ("Settings", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			settings = true;
			waypoints = false;
			info = false;
		}

		if (info) {
			editorSkin.button.normal.textColor = Color.green;
			editorSkin.button.hover.textColor = Color.green;
		}
		else {
			editorSkin.button.normal.textColor = Color.white;
			editorSkin.button.hover.textColor = Color.white;
		}
		if (GUILayout.Button ("Info", GUILayout.MaxWidth(Screen.width * 0.33f), GUILayout.MaxHeight(40) )) {
			info = true;
			settings = false;
			waypoints = false;
		}

		EditorGUILayout.EndHorizontal ();


		///
		///		Waypoint Options
		///
		if (waypoints) {

			SerializedObject obj = new SerializedObject (rg_SceneManager.managerReference.waypointCircut);

			SerializedProperty configureMode = serializedObject.FindProperty ("configureMode");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (configureMode, true, GUILayout.MaxWidth (Screen.width * 0.7f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
		
			SerializedProperty TotalWaypoints = serializedObject.FindProperty ("TotalWaypoints");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (TotalWaypoints, true, GUILayout.MaxWidth (Screen.width * 0.9f));
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button ("Cleanup Old Wayponts", GUILayout.MaxWidth (Screen.width * 0.9f))) {
				rg_SceneManager.CleanUpWaypoints ();
			}
			EditorGUILayout.HelpBox("You may ned to press this button multiple times to remove unused waypoints from your scene after adjusting waypoints."
			                        , MessageType.None);
			SerializedProperty smoothRoute = obj.FindProperty ("smoothRoute");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (smoothRoute, true, GUILayout.MaxWidth (Screen.width * 0.95f));
			if (EditorGUI.EndChangeCheck ())
				obj.ApplyModifiedProperties ();
			
			SerializedProperty editorVisualisationSubsteps = obj.FindProperty ("editorVisualisationSubsteps");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (editorVisualisationSubsteps, true, GUILayout.MaxWidth (Screen.width * 0.95f));
			if (EditorGUI.EndChangeCheck ())
				obj.ApplyModifiedProperties ();

			SerializedProperty waypointList = obj.FindProperty ("waypointList");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (waypointList, true, GUILayout.MaxWidth (Screen.width * 0.95f));
			if (EditorGUI.EndChangeCheck ())
				obj.ApplyModifiedProperties ();

		}
		if(settings){
			SerializedProperty readyTime = serializedObject.FindProperty ("readyTime");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(readyTime, true, GUILayout.MaxWidth(Screen.width * 0.95f));
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();

			SerializedProperty spawnPoints = serializedObject.FindProperty ("spawnPoints");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(spawnPoints, true, GUILayout.MaxWidth(Screen.width * 0.95f));
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();

		}
		if(info){
			EditorGUILayout.HelpBox("Vehicle Number: " + rg_SceneManager.raceData.vehicleNumber.ToString()
									+ "\nRace Number: " + rg_SceneManager.raceData.raceNumber.ToString()
			                        //+ "\nRace Name: " + rg_SceneManager.raceName.ToString()
									+ "\nRace Laps: " + rg_SceneManager.raceData.raceLaps.ToString()
									+ "\nPlayer Currency: " + rg_SceneManager.raceData.currency.ToString()
									+ "\nRace Reward 1st: " + rg_SceneManager.raceData.raceRewards[0].ToString()
									+ "\nRace Reward 2nd: " + rg_SceneManager.raceData.raceRewards[1].ToString()
									+ "\nRace Reward 3rd: " + rg_SceneManager.raceData.raceRewards[2].ToString()
									+ "\nBest Finish Time: " + rg_SceneManager.raceData.bestTime.ToString()
			                        , MessageType.None);

			SerializedProperty managerReference = serializedObject.FindProperty ("managerReference");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(managerReference, true, GUILayout.MaxWidth(Screen.width * 0.95f));
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();

		}
		
		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndVertical();

	}
	
}

namespace UnityStandardAssets.Utility.Inspector{

	[CustomPropertyDrawer(typeof ( WaypointCircuit.WaypointList))]
	public class WaypointListDrawer : PropertyDrawer{

		private float lineHeight = 17.7f;
		private float spacing = 0;
				
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

			EditorGUI.BeginProperty(position, label, property);
			float x = position.x;
			float y = position.y;
			float inspectorWidth = position.width;
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;
			
			var items = property.FindPropertyRelative("items");
			var props = new string[] {"transform", "^", "v", "-"};
			var widths = new float[] {0.7f, .1f, .1f, .1f};
			float lineHeight = 18;
			if (items.arraySize > 0) {
				//for (int i = -1; i < items.arraySize; ++i){
				for (int i = 0; i < items.arraySize; ++i){
					var item = items.GetArrayElementAtIndex(i);
					float rowX = x;
					for (int n = 0; n < props.Length; ++n){
						float w = widths[n] * inspectorWidth;
						// Calculate rects
						Rect rect = new Rect(rowX, y, w, lineHeight);
						rowX += w;
						if (n == 0) {
							EditorGUI.ObjectField(rect, item.objectReferenceValue, typeof (Transform), true);
						}
					}
					y += lineHeight + spacing;
				}
			}
			else{
				// add button
				var addButtonRect = new Rect((x + position.width) - widths[widths.Length - 1]*inspectorWidth, y,
				                             widths[widths.Length - 1]*inspectorWidth, lineHeight);
				if (GUI.Button(addButtonRect, "+")){
					items.InsertArrayElementAtIndex(items.arraySize);
				}
				y += lineHeight + spacing;
			}
			y += lineHeight + spacing;
			// Set indent back to what it was
			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
			SerializedProperty items = property.FindPropertyRelative("items");
			float lineAndSpace = lineHeight + spacing;
			return 40 + (items.arraySize*lineAndSpace) + lineAndSpace;
		}
		
		// comparer for check distances in ray cast hits
		public class TransformNameComparer : IComparer{
			public int Compare(object x, object y){
				return ((Transform) x).name.CompareTo(((Transform) y).name);
			}
		}
	}

}