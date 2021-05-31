//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// RGK EarthFX 
// Last Change : 3/10/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/EarthFX/RGK EarthFX")]
    public class EarthFX : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public EarthFXData GlobalFX;

        [HideInInspector]
        [SerializeField]
        public List<EarthFXData> SurfaceFX;


        [System.Serializable]
        public class EarthFXData : System.Object 
        {
            public bool Visible = true;
            public string FxName="";
            public Texture FxTexture;

            public GameObject Splatter;
            public float SplatterStartVelocity = 10;

            public GameObject BrakeSkid;
            public GameObject BrakeSmoke;

            public float BrakeSkidStartSlip = 0.20f;

            public GameObject TrailSkid;
            public GameObject TrailSmoke;
            public float TrailSmokeStartVelocity=15f;

            public AudioClip SurfaceDriveSound;
            public AudioClip BrakeSound;

            public bool EnableSpeedDeceleration = false;
            public float ForwardDrag=0;
            public float AngularDrag=0;

            public bool EnableGripDecrease = false;
            public float ForwardGripLosePercent=0;
            public float SidewaysGripLosePercent=0;

            public object Clone() { return this.MemberwiseClone(); } 
        }

        //[System.Serializable]
        //public class CastFXData : System.Object
        //{
        //    public float ForwardGripLosePercent = 0f;
        //    public float SidewaysGripLosePercent = 0f;
        //    public float SurfaceAngularDrag = 0f;
        //    public float SurfaceForwardDrag = 0f;
        //    public AudioClip BrakeSound;
        //    public AudioClip SurfaceSound;
        //}
 


        //public CastFXData CastFX(EarthFX.EarthFXData FX, RaycastHit SurfaceHitPoint)
        //{
        //    CastFXData ReturnFX = new CastFXData();

        //    /// BRAKE SKIDS 
        //    if (FX.BrakeSkid != null && Mathf.Abs(this.slipRatio) > FX.BrakeSkidStartSlip)
        //    {
        //        Skids = FX.BrakeSkid.GetComponent(typeof(RGKCar_Skidmarks)) as RGKCar_Skidmarks;

        //        lastSkid = Skids.AddSkidMark(SurfaceHitPoint.point, SurfaceHitPoint.normal, Mathf.Abs(slipRatio), lastSkid);

        //        if (FX.BrakeSmoke != null)
        //        {
        //            SkidSmoke = FX.BrakeSmoke.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;

        //            SkidSmoke.Emit(
        //                SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
        //                    UnityEngine.Random.Range(-0.1f, 0.1f),
        //                    UnityEngine.Random.Range(-0.1f, 0.1f)),//pos
        //                    new Vector3(this.slipVelo * 0.05f, 0), //vel
        //                    UnityEngine.Random.Range(SkidSmoke.minSize, SkidSmoke.maxSize) * Mathf.Clamp(0.5f, 1f, 1.5f), //size
        //                    UnityEngine.Random.Range(SkidSmoke.minEnergy, SkidSmoke.maxEnergy), //energy
        //                    Color.white);
        //        }
        //    }
        //    else
        //    {
        //        lastSkid = -1; 
        //    }
        //    ///TRAIL SMOKE
        //    if (FX.TrailSmoke != null && Mathf.Abs(angularVelocity) > FX.TrailSmokeStartVelocity)
        //    {
        //        TrailSmoke = FX.TrailSmoke.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;

        //        TrailSmoke.Emit(
        //            SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
        //                UnityEngine.Random.Range(-0.1f, 0.1f),
        //                UnityEngine.Random.Range(-0.1f, 0.1f)),
        //                new Vector3(this.slipVelo * 0.05f, 1),
        //                UnityEngine.Random.Range(TrailSmoke.minSize, TrailSmoke.maxSize) * Mathf.Clamp(0.5f, 1f, 1.5f),
        //                UnityEngine.Random.Range(TrailSmoke.minEnergy, TrailSmoke.maxEnergy), Color.white);
        //    }
        //    ///TRAIL SKIDS 
        //    if (FX.TrailSkid != null && Math.Abs(angularVelocity) > 5)
        //    {
        //        Trails = FX.TrailSkid.GetComponent(typeof(RGKCar_Skidmarks)) as RGKCar_Skidmarks;
        //        lastTrail = Trails.AddSkidMark(SurfaceHitPoint.point, SurfaceHitPoint.normal, Mathf.Abs(1) - 0.2f, lastTrail);
        //    }
        //    else
        //    {
        //        lastTrail = -1;
        //    }
        //    ///SPLATTERS
        //    if (FX.Splatter != null && Math.Abs(angularVelocity) > 5 && !surfaceChanged)
        //    {
        //        Splatter = FX.Splatter.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;
        //        Splatter.Emit(
        //            SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f),
        //                UnityEngine.Random.Range(-0.1f, 0.2f),
        //                UnityEngine.Random.Range(-0.1f, 0.1f)),
        //                new Vector3(this.slipVelo * 0.05f, 0),
        //                UnityEngine.Random.Range(Splatter.minSize, Splatter.maxSize),
        //                UnityEngine.Random.Range(Splatter.minEnergy, Splatter.maxEnergy), Color.white);

        //    }
        //    ///DRIVE SOUND
        //    if (FX.SurfaceDriveSound != null)
        //    { SurfaceSound = FX.SurfaceDriveSound; }
        //    else
        //    { SurfaceSound = null; }

        //    ///BRAKE SOUND
        //    if (FX.BrakeSound != null)
        //    { BrakeSound = FX.BrakeSound; }
        //    else
        //    {
        //        BrakeSound = null;
        //    }

        //    ///FORWARDGRIP LOSE PERCENT
        //    if (FX.ForwardGripLosePercent > 0)
        //    {
        //        wheelGrip = definedGrip - (definedGrip * (FX.ForwardGripLosePercent / 100f));
        //    }
        //    else
        //    {
        //        wheelGrip = definedGrip;
        //    }

        //    ///SIDEGRIP LOSE PERCENT
        //    if (FX.SidewaysGripLosePercent > 0)
        //    {
        //        sideGrip= definedSideGrip- (definedSideGrip * (FX.SidewaysGripLosePercent / 100f));
        //    }
        //    else
        //    {
        //        sideGrip = definedSideGrip;
        //    }

        //    ///DRAG
        //    SurfaceAngularDrag = FX.AngularDrag;
        //    SurfaceForwardDrag = FX.ForwardDrag;

        //}



    }
    /// <summary>
    /// Retrieve terrain texture currently standing on. 
    /// </summary>
    public class SurfaceTextureDedector
    {
        public static float[] GetTextureMix(Vector3 worldPos)
        {

            Terrain terrain = Terrain.activeTerrain;
            TerrainData terrainData = terrain.terrainData;
            Vector3 terrainPos = terrain.transform.position;


            int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);


            float[, ,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);


            float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
            for (int n = 0; n < cellMix.Length; ++n)
            {
                cellMix[n] = splatmapData[0, 0, n];
            }

            return cellMix;

        }
        
        public static int GetMainTexture(Vector3 worldPos)
        {
            float[] mix = GetTextureMix(worldPos);

            float maxMix = 0;
            int maxIndex = 0;

            for (int n = 0; n < mix.Length; ++n)
            {
                if (mix[n] > maxMix)
                {
                    maxIndex = n;
                    maxMix = mix[n];
                }
            }
            return maxIndex;
        }

    }
}