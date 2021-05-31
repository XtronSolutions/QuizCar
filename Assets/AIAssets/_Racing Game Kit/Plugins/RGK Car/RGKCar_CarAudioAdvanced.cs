//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using System.Collections;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;
using System.Collections.Generic;
using SmartAssembly.Attributes;

namespace RacingGameKit.RGKCar
{
    [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Advanced Audio")]
    public class RGKCar_CarAudioAdvanced : MonoBehaviour
    {
        private RGKCar_Engine CarEngine;
        private RGKCar_Setup CarSetup;

        public AudioClip TurboPopHigh;
        public AudioClip TurboWaste;
        public AudioClip Skid;
        public AudioClip SpeedHiss;
       private AudioClip BrakeDisk;
        public AudioClip SurfaceSound;
        public AudioClip BackfireShort;
        public AudioClip BackfireLong;
        public AudioClip NOSBoost;
 
        
        public AudioClip GearChange;

        private AudioSource AudioChannel1;
        private AudioSource AudioChannel2;
        private AudioSource AudioChannelTurbo;
        private AudioSource AudioChannelHiss;
        private AudioSource AudioChannelSkid;
        private AudioSource AudioChannelBackfire;
        private AudioSource AudioChannelSurface;
        private AudioSource AudioChannelNos;
        private AudioSource AudioChannelCrash;
        private AudioSource AudioChannelGearChange;
        private AudioSource AudioChannelBrakeDisk;

        private float EngineThrootle = 0;

        private RPMLevelStage[] rpmLevelStages = new RPMLevelStage[2];

        private float EngineRPM = 0f;

        [SerializeField]
        [DoNotObfuscate]
        private List<RPMLevel> RPMLevels = new List<RPMLevel>();

        [SerializeField]
        [DoNotObfuscate]
        private AnimationCurve RevUpVolume;
        [SerializeField]
        [DoNotObfuscate]
        private AnimationCurve RevDownVolume;



        public void Start()
        {
            CarEngine = GetComponent(typeof(RGKCar_Engine)) as RGKCar_Engine;
            CarSetup = GetComponent(typeof(RGKCar_Setup)) as RGKCar_Setup;
            SetupAudioSources();



            for (int i = 0; i < this.RPMLevels.Count; i++)
            {
                this.RPMLevels[i].Source = ((i % 2) != 0) ? this.AudioChannel2 : this.AudioChannel1;
            }
            this.rpmLevelStages[0] = new RPMLevelStage();
            this.rpmLevelStages[1] = new RPMLevelStage();

            if (this.AudioChannel1 != null)
            {
                this.AudioChannel1.loop = true;
                this.AudioChannel1.volume = 0f;
            }
            if (this.AudioChannel2 != null)
            {
                this.AudioChannel2.loop = true;
                this.AudioChannel2.volume = 0f;
            }
        }
        private void SetupAudioSources()
        {
            GameObject aChannel1 = new GameObject("audio_channel1");
            aChannel1.transform.parent = transform;
            aChannel1.transform.localPosition = Vector3.zero;
            aChannel1.transform.localRotation = Quaternion.identity;
            aChannel1.AddComponent(typeof(AudioSource));
            aChannel1.GetComponent<AudioSource>().loop = true;
            aChannel1.GetComponent<AudioSource>().volume = 1;
            AudioChannel1 = aChannel1.GetComponent(typeof(AudioSource)) as AudioSource;
            AudioChannel1.playOnAwake = false;

            GameObject aChannel2 = new GameObject("audio_channel2");
            aChannel2.transform.parent = transform;
            aChannel2.transform.localPosition = Vector3.zero;
            aChannel2.transform.localRotation = Quaternion.identity;
            aChannel2.AddComponent(typeof(AudioSource));
            aChannel2.GetComponent<AudioSource>().loop = true;
            aChannel2.GetComponent<AudioSource>().volume = 1;
            AudioChannel2 = aChannel2.GetComponent(typeof(AudioSource)) as AudioSource;
            AudioChannel2.playOnAwake = false;

            if (TurboPopHigh != null)
            {
                GameObject aChannelTurbo = new GameObject("audio_turbo");
                aChannelTurbo.transform.parent = transform;
                aChannelTurbo.transform.localPosition = Vector3.zero;
                aChannelTurbo.transform.localRotation = Quaternion.identity;
                aChannelTurbo.AddComponent(typeof(AudioSource));
                aChannelTurbo.GetComponent<AudioSource>().loop = false;
                aChannelTurbo.GetComponent<AudioSource>().volume = 1;
                AudioChannelTurbo = aChannelTurbo.GetComponent(typeof(AudioSource)) as AudioSource;
                AudioChannelTurbo.playOnAwake = false;
            }

            if (SpeedHiss != null)
            {
                GameObject aChannelHiss = new GameObject("audio_hiss");
                aChannelHiss.transform.parent = transform;
                aChannelHiss.transform.localPosition = Vector3.zero;
                aChannelHiss.transform.localRotation = Quaternion.identity;
                aChannelHiss.AddComponent(typeof(AudioSource));
                aChannelHiss.GetComponent<AudioSource>().loop = true;
                aChannelHiss.GetComponent<AudioSource>().volume = 0;
                aChannelHiss.GetComponent<AudioSource>().pitch = 0;
                aChannelHiss.GetComponent<AudioSource>().clip = SpeedHiss;
                AudioChannelHiss = aChannelHiss.GetComponent(typeof(AudioSource)) as AudioSource;
                AudioChannelHiss.playOnAwake = true;
                AudioChannelHiss.Play();
            }

            GameObject aChannelSkid = new GameObject("audio_skid");
            aChannelSkid.transform.parent = transform;
            aChannelSkid.transform.localPosition = Vector3.zero;
            aChannelSkid.transform.localRotation = Quaternion.identity;
            aChannelSkid.AddComponent(typeof(AudioSource));
            aChannelSkid.GetComponent<AudioSource>().loop = true;
            aChannelSkid.GetComponent<AudioSource>().volume = 0;
            aChannelSkid.GetComponent<AudioSource>().clip = Skid;
            AudioChannelSkid = aChannelSkid.GetComponent(typeof(AudioSource)) as AudioSource;
            AudioChannelSkid.playOnAwake = true;
            AudioChannelSkid.Play();


            if (BackfireShort != null || BackfireLong != null)
            {
                GameObject aChannelBackfire = new GameObject("audio_backfire");
                aChannelBackfire.transform.parent = transform;
                aChannelBackfire.transform.localPosition = Vector3.zero;
                aChannelBackfire.transform.localRotation = Quaternion.identity;
                aChannelBackfire.AddComponent(typeof(AudioSource));
                aChannelBackfire.GetComponent<AudioSource>().loop = false;
                aChannelBackfire.GetComponent<AudioSource>().volume = 1;
                aChannelBackfire.GetComponent<AudioSource>().clip = BackfireShort;
                AudioChannelBackfire = aChannelBackfire.GetComponent(typeof(AudioSource)) as AudioSource;
                AudioChannelBackfire.playOnAwake = false;
            }


            GameObject aChannelSurface = new GameObject("audio_surface");
            aChannelSurface.transform.parent = transform;
            aChannelSurface.transform.localPosition = Vector3.zero;
            aChannelSurface.transform.localRotation = Quaternion.identity;
            aChannelSurface.AddComponent(typeof(AudioSource));
            aChannelSurface.GetComponent<AudioSource>().loop = true;
            aChannelSurface.GetComponent<AudioSource>().volume = 0;
            aChannelSurface.GetComponent<AudioSource>().pitch = 1;
            aChannelSurface.GetComponent<AudioSource>().clip = SurfaceSound;
            AudioChannelSurface = aChannelSurface.GetComponent(typeof(AudioSource)) as AudioSource;
            AudioChannelSurface.playOnAwake = false;


            if (GearChange != null)
            {
                GameObject aChannelGearchange = new GameObject("audio_gearchange");
                aChannelGearchange.transform.parent = transform;
                aChannelGearchange.transform.localPosition = Vector3.zero;
                aChannelGearchange.transform.localRotation = Quaternion.identity;
                aChannelGearchange.AddComponent(typeof(AudioSource));
                aChannelGearchange.GetComponent<AudioSource>().loop = false;
                aChannelGearchange.GetComponent<AudioSource>().volume = 1;
                AudioChannelGearChange = aChannelGearchange.GetComponent(typeof(AudioSource)) as AudioSource;
                AudioChannelGearChange.playOnAwake = false;
            }

            if (BrakeDisk != null)
            {
                GameObject aChannelBrakeDisk = new GameObject("audio_brakedisk");
                aChannelBrakeDisk.transform.parent = transform;
                aChannelBrakeDisk.transform.localPosition = Vector3.zero;
                aChannelBrakeDisk.transform.localRotation = Quaternion.identity;
                aChannelBrakeDisk.AddComponent(typeof(AudioSource));
                aChannelBrakeDisk.GetComponent<AudioSource>().loop = true;
                aChannelBrakeDisk.GetComponent<AudioSource>().volume = 0;
                aChannelBrakeDisk.GetComponent<AudioSource>().clip = BrakeDisk;
                AudioChannelBrakeDisk = aChannelBrakeDisk.GetComponent(typeof(AudioSource)) as AudioSource;
                AudioChannelBrakeDisk.playOnAwake = true;
            }

        }
        public void LateUpdate()
        {
            EngineThrootle = CarEngine.throttle;
            this.EngineRPM = CarEngine.RPM;

            for (int i = 0; i < this.RPMLevels.Count; i++)
            {

                if ((this.EngineRPM >= this.RPMLevels[i].RpmLow) && (this.EngineRPM < this.RPMLevels[i].RpmHigh))
                {
                    if ((i < (this.RPMLevels.Count - 1)) && (this.EngineRPM >= this.RPMLevels[i + 1].RpmLow))
                    {
                        this.rpmLevelStages[0].RPMLevel = this.RPMLevels[i];
                        this.rpmLevelStages[0].RPMLevel.Source.mute = false;
                        this.rpmLevelStages[1].RPMLevel = this.RPMLevels[i + 1];
                        this.rpmLevelStages[1].RPMLevel.Source.mute = false;

                    }
                    else
                    {
                        this.rpmLevelStages[0].RPMLevel = this.RPMLevels[i];
                        this.rpmLevelStages[1].RPMLevel = null;
                        if ((this.rpmLevelStages[0].RPMLevel.Source == this.AudioChannel1) && this.AudioChannel2.isPlaying)
                        {
                            this.AudioChannel1.mute = false;
                            this.AudioChannel2.mute = true;
                        }
                        else if ((this.rpmLevelStages[0].RPMLevel.Source == this.AudioChannel2) && this.AudioChannel1.isPlaying)
                        {
                            this.AudioChannel1.mute = true;
                            this.AudioChannel2.mute = false;
                        }
                    }
                    break;
                }
            }

            for (int j = 0; j < this.rpmLevelStages.Length; j++)
            {
                RPMLevelStage rpmLevelStage = this.rpmLevelStages[j];
                RPMLevel rpmLevel = rpmLevelStage.RPMLevel;
                if (rpmLevel != null)
                {

                    float currentRPMDiff = Mathf.Clamp(this.EngineRPM, rpmLevel.RpmLow, rpmLevel.RpmHigh);
                    float levelRPMDiff = rpmLevel.RpmHigh - rpmLevel.RpmLow;
                    float rpmTime = (currentRPMDiff - rpmLevel.RpmLow) / levelRPMDiff;
                    float pitchDiff = rpmLevel.PitchMax - rpmLevel.PitchMin;
                    rpmLevelStage.Pitch = rpmLevel.PitchMin + (pitchDiff * rpmLevel.PitchCurve.Evaluate(rpmTime));

                    if (EngineThrootle > 0)
                    { rpmLevel.Source.clip = rpmLevel.OnClip; }
                    else
                    { rpmLevel.Source.clip = rpmLevel.OffClip; }
                    rpmLevelStage.Volume = 1f;

                    if (this.EngineThrootle > 0f)
                    {
                        rpmLevelStage.Volume = (rpmLevelStage.Volume * this.RevUpVolume.Evaluate(this.EngineRPM / CarSetup.EngineData.EngineMaxRPM)) * 1;
                    }
                    else
                    {
                        rpmLevelStage.Volume = (rpmLevelStage.Volume * this.RevDownVolume.Evaluate(this.EngineRPM / CarSetup.EngineData.EngineMaxRPM)) * 1;
                    }
                    if (!rpmLevel.Source.isPlaying)
                    {
                        rpmLevel.Source.Play();
                    }
                }
            }

            if ((this.rpmLevelStages[0].RPMLevel != null) && (this.rpmLevelStages[1].RPMLevel != null))
            {
                float levelRPMDiff = this.rpmLevelStages[0].RPMLevel.RpmHigh - this.rpmLevelStages[1].RPMLevel.RpmLow;
                float stageRPMCoef = (this.EngineRPM - this.rpmLevelStages[1].RPMLevel.RpmLow) / levelRPMDiff;
                this.rpmLevelStages[0].RPMLevel.CurrentFade = this.rpmLevelStages[0].RPMLevel.FadeCurve.Evaluate(1f - stageRPMCoef);
                this.rpmLevelStages[1].RPMLevel.CurrentFade = this.rpmLevelStages[0].RPMLevel.FadeCurve.Evaluate(stageRPMCoef);
                RPMLevelStage levelStage1 = this.rpmLevelStages[0];
                levelStage1.Volume *= this.rpmLevelStages[0].RPMLevel.CurrentFade;
                RPMLevelStage levelStage2 = this.rpmLevelStages[1];
                levelStage2.Volume *= this.rpmLevelStages[1].RPMLevel.CurrentFade;
            }

            this.rpmLevelStages[0].Update();
            this.rpmLevelStages[1].Update();
            ApplyHiss();
            ApplySurface();
        }

        private void ApplyHiss()
        {
            if (SpeedHiss != null)
            {
                if (CarEngine.SpeedAsKM > 70)
                {
                    AudioChannelHiss.mute = false;
                    AudioChannelHiss.volume = CarEngine.SpeedAsKM / 150;
                    if (AudioChannelHiss.volume < 0.4) AudioChannelHiss.volume = 0.4f;
                    AudioChannelHiss.pitch = CarEngine.SpeedAsKM / 150;
                    if (AudioChannelHiss.pitch < 0.5f) AudioChannelHiss.pitch = 0.5f;
                    if (AudioChannelHiss.pitch > 1.5f) AudioChannelHiss.pitch = 1.5f;

                }
                else
                {
                    AudioChannelHiss.mute = true;
                }
            }
        }

        private void ApplySurface()
        {
            if (CarEngine.SpeedAsKM > 3 && SurfaceSound != null)
            {
                AudioChannelSurface.GetComponent<AudioSource>().clip = SurfaceSound;
                AudioChannelSurface.mute = false;
                AudioChannelSurface.volume = CarEngine.SpeedAsKM / 100;
                if (AudioChannelSurface.volume < 0.3)
                    AudioChannelSurface.volume = 0.3f;

                
              
                if (!AudioChannelSurface.isPlaying)
                {
                    AudioChannelSurface.Play();
                }
            }
            else
            {
                AudioChannelSurface.mute = true;
            }
        }

        public void PopTurbo(float TurboValue)
        {
            if (TurboPopHigh != null && TurboWaste != null)
            {
                bool blnIsTurboInRange = true;
                if (TurboValue > 0.9f)
                {
                    AudioChannelTurbo.GetComponent<AudioSource>().clip = TurboPopHigh;
                }
                else if (TurboValue >= 0.5f && TurboValue < 0.9f)
                {
                    AudioChannelTurbo.GetComponent<AudioSource>().clip = TurboWaste;
                }
                else
                {
                    blnIsTurboInRange = false;
                }

                if (!AudioChannelTurbo.isPlaying && blnIsTurboInRange)
                { AudioChannelTurbo.Play(); }
            }
        }

        public void PopGear()
        {
            if (GearChange != null && AudioChannelGearChange!=null)
            {
                
                 AudioChannelGearChange.GetComponent<AudioSource>().clip = GearChange; 

                AudioChannelGearChange.Play();
            }

        }

        //Postponed to next version
        public void ApplyBrakeDisk(float Brake)
        {
            //if (BrakeDisk != null)
            //{
            //    Debug.Log("apply " + Brake.ToString());
            //    AudioChannelBrakeDisk.volume = Mathf.Clamp01(Mathf.Abs(Brake));
            //}

        }

        public bool PopBackfire(Boolean IsLong)
        {
            if (AudioChannelBackfire != null)
            {
                if (IsLong)
                    AudioChannelBackfire.clip = BackfireLong;
                else
                    AudioChannelBackfire.clip = BackfireShort;

                if (!AudioChannelBackfire.isPlaying) { AudioChannelBackfire.Play(); }
                return AudioChannelBackfire.isPlaying;
            }
            else
            {
                return false;
            }
            
        }

        void Update()
        {
            AudioChannelSkid.volume = Mathf.Clamp01(Mathf.Abs(CarEngine.TotalSlip) * 0.2f - 0.3f);
        }

        public void UpdateSkidSound()
        {
            AudioChannelSkid.GetComponent<AudioSource>().clip = Skid;
            if (!AudioChannelSkid.isPlaying) AudioChannelSkid.Play();
        }

        public class RPMLevelStage
        {
            public void Update()
            {
                if (this.RPMLevel != null)
                {
                    this.RPMLevel.Source.volume = this.Volume;
                    this.RPMLevel.Source.pitch = this.Pitch;
                }
            }

            public float Pitch { get; set; }

            public float Volume { get; set; }

            public RGKCar_CarAudioAdvanced.RPMLevel RPMLevel { get; set; }
        }

        [Serializable]
        public class RPMLevel
        {
            [SerializeField]
            private AudioClip offSound;
            [SerializeField]
            private AudioClip onSound;
            [SerializeField]
            private float rpmHigh;
            [SerializeField]
            private float rpmLow;
            [SerializeField]
            private AnimationCurve fadeCurve;
            [SerializeField]
            private float currentFade;

            [SerializeField]
            private AnimationCurve pitchCurve;
            [SerializeField]
            private float pitchMax;
            [SerializeField]
            private float pitchMin;

            public AnimationCurve FadeCurve
            {
                get
                {
                    return this.fadeCurve;
                }
            }

            public float CurrentFade
            {
                get
                {
                    return this.currentFade;
                }
                set
                {
                    this.currentFade = value;
                }
            }

            public AudioClip OffClip
            {
                get
                {
                    return this.offSound;
                }
            }

            public AudioClip OnClip
            {
                get
                {
                    return this.onSound;
                }
            }

            public AnimationCurve PitchCurve
            {
                get
                {
                    return this.pitchCurve;
                }
            }

            public float PitchMax
            {
                get
                {
                    return this.pitchMax;
                }
            }

            public float PitchMin
            {
                get
                {
                    return this.pitchMin;
                }
            }

            public float RpmHigh
            {
                get
                {
                    return this.rpmHigh;
                }
            }

            public float RpmLow
            {
                get
                {
                    return this.rpmLow;
                }
            }

            public AudioSource Source { get; set; }
        }


        //public struct WobbleEffect
        //{
        //    public float PitchEffect;
        //    public float VolumeEffect;
        //}
    }
}