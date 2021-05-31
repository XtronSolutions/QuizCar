using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RG_AIHelper))]
public class Editor_RG_AIHelper : Editor {
	public override void OnInspectorGUI(){
		serializedObject.Update();
		RG_AIHelper myTarget = (RG_AIHelper)target;
		myTarget.avoidTagName = EditorGUILayout.TagField("Avoid Tag", myTarget.avoidTagName);
		DrawDefaultInspector ();
	}
}