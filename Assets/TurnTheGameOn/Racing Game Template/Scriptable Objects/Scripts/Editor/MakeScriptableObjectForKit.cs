using UnityEngine;
using System.Collections;
using UnityEditor;

public class MakeScriptableObjectForKit : MonoBehaviour {

	[MenuItem("Assets/Create/PlayabeVehicles")]
	public static void CreatePlayableVehicles(){
		PlayableVehicles asset = ScriptableObject.CreateInstance<PlayableVehicles>();
		AssetDatabase.CreateAsset (asset, "Assets/PlayableVehicles.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/OpponentVehicles")]
	public static void CreateOpponentVehicles(){
		OpponentVehicles asset = ScriptableObject.CreateInstance<OpponentVehicles>();
		AssetDatabase.CreateAsset (asset, "Assets/OpponentVehicles.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/RaceData")]
	public static void CreateRaceData(){
		RaceData asset = ScriptableObject.CreateInstance<RaceData>();
		AssetDatabase.CreateAsset (asset, "Assets/RaceData.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/PlayerPrefsData")]
	public static void CreatePlayerPrefsData(){
		PlayerPrefsData asset = ScriptableObject.CreateInstance<PlayerPrefsData>();
		AssetDatabase.CreateAsset (asset, "Assets/PlayerPrefsData.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/AudioData")]
	public static void CreateAudioData(){
		AudioData asset = ScriptableObject.CreateInstance<AudioData>();
		AssetDatabase.CreateAsset (asset, "Assets/AudioData.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

	[MenuItem("Assets/Create/InputData")]
	public static void CreateInputData(){
		InputData asset = ScriptableObject.CreateInstance<InputData>();
		AssetDatabase.CreateAsset (asset, "Assets/InputData.asset");
		AssetDatabase.SaveAssets ();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = asset;
	}

}