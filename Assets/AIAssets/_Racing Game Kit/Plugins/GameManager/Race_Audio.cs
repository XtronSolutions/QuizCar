//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Game Audio Script
// Last Change : 20/06/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using System.Collections;
using RacingGameKit.Helpers;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Game Mechanics/Race Audio Manager"), RequireComponent(typeof(Race_Manager))]
    public class Race_Audio : MonoBehaviour
    {
        [HideInInspector]
        public GameObject GameCamera;
        public AudioClip RaceCountDown;
        public AudioClip RaceCheckpoint;
        public AudioClip RaceStart;
        public AudioClip RaceWon;
        public AudioClip RaceLost;
        public AudioClip[] BackgroundMusic;
        public bool RandomBackground = true;
        public bool LoopBackground = true;
        public float BackgroundMusicVolume = 0.7f;
        public float EffectsSoundVolume = 0.7f;
        public bool MuteAllSounds = false;

        private AudioSource BackgroundMusicPlayer;
        private AudioSource RaceSoundsPlayer;
        private bool IsBackgroundMusicStarted = false;
        private int LastPlayedIndex = 0;
        private bool IsAudioEnabled = true;

        public void InitAudio()
        {
            if (GameCamera == null) //if game camera not attached
            {
                GameCamera = GameObject.Find("_GameCamera"); //try find _GameCamera Object
                
            }
            if (GameCamera == null)
            {
                Debug.LogWarning(RGKMessages.GameCameraNotAttached);
                IsAudioEnabled = false;
            }
            else
            {
                BackgroundMusicPlayer = CreateAudioSource(GameCamera.transform, "background");
                RaceSoundsPlayer = CreateAudioSource(GameCamera.transform, "effects");
            }
            
        }

        public void FixedUpdate()
        {
            try
            {
                if (Input.GetButtonDown("Mute"))
                {
                    if (MuteAllSounds)
                    {
                        MuteAllSounds = false;
                    }
                    else
                    { MuteAllSounds = true; }
                }

                if (IsAudioEnabled)
                {
                    if (!BackgroundMusicPlayer.isPlaying && IsBackgroundMusicStarted && BackgroundMusic != null && BackgroundMusic.GetLength(0) > 0)
                    {

                        if (RandomBackground)
                        {
                            BackgroundMusicPlayer.clip = BackgroundMusic[UnityEngine.Random.Range(0, BackgroundMusic.Length)];
                            BackgroundMusicPlayer.Play();
                        }
                        else
                        {
                            BackgroundMusicPlayer.clip = BackgroundMusic[LastPlayedIndex];
                            BackgroundMusicPlayer.Play();

                            LastPlayedIndex++;

                            if (LoopBackground && LastPlayedIndex == BackgroundMusic.Length)
                            {
                                LastPlayedIndex = 0;
                            }
                        }
                    }

                    if (BackgroundMusic != null)
                    {
                        BackgroundMusicPlayer.volume = Mathf.Clamp01(BackgroundMusicVolume);
                        BackgroundMusicPlayer.mute = MuteAllSounds;
                    }
                    if (RaceSoundsPlayer != null)
                    {
                        RaceSoundsPlayer.volume = Mathf.Clamp01(EffectsSoundVolume);

                        RaceSoundsPlayer.mute = MuteAllSounds;
                    }
                }
            }
            catch { }
        }

        public void PlayCountDownAudio()
        {
            if (RaceCountDown != null && RaceSoundsPlayer != null)
            {
                RaceSoundsPlayer.clip = RaceCountDown;
                RaceSoundsPlayer.Play();
            }
        }

        public void PlayStartAudio()
        {
            if (RaceStart != null && RaceSoundsPlayer != null)
            {
                RaceSoundsPlayer.clip = RaceStart;
                RaceSoundsPlayer.Play();
            }
        }
        public void PlayCheckPointAudio()
        {
            if (RaceCheckpoint != null && RaceSoundsPlayer != null)
            {
                RaceSoundsPlayer.clip = RaceCheckpoint;
                RaceSoundsPlayer.Play();
            }
        }

        public void PlayRaceWon()
        {
            if (RaceWon != null && RaceSoundsPlayer != null)
            {
                RaceSoundsPlayer.clip = RaceWon;
                RaceSoundsPlayer.Play();
            }
        }
        public void PlayRaceLost()
        {
            if (RaceLost != null && RaceSoundsPlayer != null)
            {
                RaceSoundsPlayer.clip = RaceLost;
                RaceSoundsPlayer.Play();
            }
        }

        public void PlayBackgroundMusic(bool Play)
        {
            IsBackgroundMusicStarted = Play;
        }

        private AudioSource CreateAudioSource(Transform Parent, string AudioSourceName)
        {
            GameObject oAudioSource = new GameObject("audiosource_" + AudioSourceName);
            oAudioSource.transform.parent = Parent;
            oAudioSource.transform.localPosition = Vector3.zero;
            oAudioSource.transform.localRotation = Quaternion.identity;
            oAudioSource.AddComponent(typeof(AudioSource));
            oAudioSource.GetComponent<AudioSource>().loop = false;
            oAudioSource.GetComponent<AudioSource>().Play();
            return oAudioSource.GetComponent<AudioSource>();
        }

    }
}