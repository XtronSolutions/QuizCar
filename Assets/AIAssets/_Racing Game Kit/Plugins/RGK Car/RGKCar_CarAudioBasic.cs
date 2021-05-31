//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar
{
    // Simple class to controll sounds of the car, based on engine throttle and RPM, and skid velocity.
    [RequireComponent(typeof(RGKCar_Engine))]
    [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Basic Audio")]
    public class RGKCar_CarAudioBasic : MonoBehaviour
    {
        private RGKCar_Setup CarSetup;

        public AudioClip Engine;
        public AudioClip Skid;

        public float PitchLow = 0.5f;
        public float PitchHigh = 1.7f;
        AudioSource engineSource;
        AudioSource skidSource;

     
        RGKCar_Engine CarEngine;

        AudioSource CreateAudioSource(AudioClip clip)
        {
            GameObject go = new GameObject("audio");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.AddComponent(typeof(AudioSource));
            go.GetComponent<AudioSource>().clip = clip;
            go.GetComponent<AudioSource>().loop = true;
            go.GetComponent<AudioSource>().volume = 0;
            go.GetComponent<AudioSource>().Play();
            return go.GetComponent<AudioSource>();
        }

        void Start()
        {
            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;

            engineSource = CreateAudioSource(Engine);
            skidSource = CreateAudioSource(Skid);

        }

        void Update()
        {
            float pitchValue=(1.3f * CarEngine.RPM / CarSetup.EngineData.EngineMaxRPM);
            engineSource.pitch = Mathf.Clamp(pitchValue, PitchLow, PitchHigh);
            engineSource.volume = 0.4f + 0.6f * CarEngine.throttle;
            skidSource.volume = Mathf.Clamp01(Mathf.Abs(CarEngine.TotalSlip) * 0.2f - 0.5f);
        }

    
    }
}