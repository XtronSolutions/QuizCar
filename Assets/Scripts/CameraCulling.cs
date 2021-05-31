using UnityEngine;
using System.Collections;


public class CameraCulling : MonoBehaviour {

    enum Layers {
       Stones = 20,
        Bushes = 22,
        Trees = 25,
        Houses = 23,
        LightHouse = 24,
        Environment = 17,
        RaceCar = 15,
		Destructatble = 16,
		BigRocks = 31

    }
    public int StonesDistance,BushesDistance,TreesDistance,HousesDistance,LightHouseDistance,EnvironmentDistance,RaceCarDistance,DestructableDiatance,BigRocksDistance;
   
    // Use this for initialization
    void OnEnable () {
        ApplyCulling();
		Resources.UnloadUnusedAssets ();

        //if (QualitySettings.GetQualityLevel() == 1 )
        //{
            //GlobalProjecKCtorManager.Get().ShadowsOn = true;
        //}
        //else {
        //    GlobalProjectorManager.Get().ShadowsOn = false;
        //}

    }
    void ApplyCulling()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[LayerMask.NameToLayer("Stones")] = StonesDistance;
        distances[LayerMask.NameToLayer("Bushes")] = BushesDistance;
        distances[LayerMask.NameToLayer("Trees")] = TreesDistance;
        distances[LayerMask.NameToLayer("Houses")] = HousesDistance;
        distances[LayerMask.NameToLayer("LightHouse")] = LightHouseDistance;
        distances[LayerMask.NameToLayer("Environment")] = EnvironmentDistance;
		distances[LayerMask.NameToLayer("Destructable")] = DestructableDiatance;
		distances[LayerMask.NameToLayer("BigRocks")] = BigRocksDistance;
		camera.layerCullDistances = distances;
    }

}
