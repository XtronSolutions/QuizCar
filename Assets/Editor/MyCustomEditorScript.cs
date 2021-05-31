using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;


public class MyCustomEditorScript
{
    [MenuItem("Tools/ReverseArrangement")]
    static void ReverseArrangement()
    {
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            Vector3 v = Selection.gameObjects[0].transform.rotation.eulerAngles;
            Selection.gameObjects[0].transform.rotation.SetEulerAngles(new Vector3(v.x,v.y + 180f,v.z));
        }
    }
    [MenuItem("Tools/Create object with icon")]
    static void CreateSphereWithIcon()
    {
        //Texture2D icon = (Texture2D)Resources.Load("CustomIcon");
        ////var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        var editorGUIUtilityType = typeof(EditorGUIUtility);
        var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;

        foreach (GameObject go in Selection.gameObjects)
        {
            var args = new object[] { go, null };
            editorGUIUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
        

       // var editorUtilityType = typeof(EditorUtility);
       //editorUtilityType.InvokeMember("ForceReloadInspectors", bindingFlags, null, null, null);
    }

    static GameObject[] objects;
    static GameObject[] Prefabtrees;
    [MenuItem("Tools/HoldSelectedTrees")]
    private static void HoldSelectedTrees()
    {
        objects = Selection.gameObjects;
    }
    [MenuItem("Tools/HoldSelectedPrefabTrees")]
    private static void HoldSelectedPrefabTrees()
    {
        Prefabtrees = Selection.gameObjects;
    }
    [MenuItem("Tools/generatePrefabsforObjects")]
    private static void generatePrefabsforObjects()
    {
        foreach (GameObject tree in objects)
        {
            Object prefabRoot = PrefabUtility.GetPrefabParent(Selection.activeGameObject);

            if (prefabRoot != null)
                (PrefabUtility.InstantiatePrefab(prefabRoot) as GameObject).transform.parent = GameObject.Find("unity").transform;
        }

    }
    [MenuItem("Tools/RepositionPrefabs")]
    public static void RepositionPrefabs()
    {
        for(int i = 0; i < Prefabtrees.Length;i++)
        {
           Prefabtrees[i].transform.localPosition = objects[i].transform.localPosition;
           Prefabtrees[i].transform.localRotation = objects[i].transform.localRotation;

        }
    }
    [MenuItem("Tools/ClearHolded")]
    public static void ClearHolded()
    {
        Prefabtrees = null;
        objects = null;
    }
    [MenuItem("Tools/SetDistanceFromGround")]
    public static void SetDistanceFromGround()
    {

        RaycastHit hit;
        foreach (GameObject GO in Selection.gameObjects)
        {
            Ray downRay = new Ray(GO.transform.position, -Vector3.up);
            if (Physics.Raycast(downRay, out hit))
            {
                //Debug.DrawLine(GO.transform.position, hit.point, Color.cyan);
                if (hit.transform.CompareTag("Track"))
                    GO.transform.position = hit.point;

                GO.transform.position = new Vector3(GO.transform.position.x, hit.point.y +1.5f, GO.transform.position.z);

            }
        }
        
       
    }
}


 
public class AtlasSize_512 : EditorWindow {[MenuItem("LightmapSize/AtlasSize_512")] static void Init() { LightmapEditorSettings.maxAtlasHeight = 512; LightmapEditorSettings.maxAtlasSize = 512; } }


public class AtlasSize_1K : EditorWindow {[MenuItem("LightmapSize/AtlasSize_1K")] static void Init() { LightmapEditorSettings.maxAtlasHeight = 1024; LightmapEditorSettings.maxAtlasSize = 1024; } }


public class AtlasSize_2K : EditorWindow {[MenuItem("LightmapSize/AtlasSize_2K")] static void Init() { LightmapEditorSettings.maxAtlasHeight = 2048; LightmapEditorSettings.maxAtlasSize = 2048; } }


public class AtlasSize_4K : EditorWindow {[MenuItem("LightmapSize/AtlasSize_4K")] static void Init() { LightmapEditorSettings.maxAtlasHeight = 4096; LightmapEditorSettings.maxAtlasSize = 4096; } }