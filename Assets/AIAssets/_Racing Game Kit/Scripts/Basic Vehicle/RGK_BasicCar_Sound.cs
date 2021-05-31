//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Sound Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;

[AddComponentMenu("Racing Game Kit/Basic Car/RGK Basic Car Sound")]
public class RGK_BasicCar_Sound : MonoBehaviour
{

    public AudioClip EngineSound;

    public float audioVolume = 1f;
    public float EngineVolume;
    private AudioSource EngineSource;
    private RGK_BasicCar vehicle;

    private AudioSource CreateAudioSource(AudioClip clip, Boolean Loop, Boolean PlayAtStartup)
    {
        GameObject oAudioSource = new GameObject("audiosource_" + clip.name.ToString().ToLower());
        oAudioSource.transform.parent = base.transform;
        oAudioSource.transform.localPosition = Vector3.zero;
        oAudioSource.transform.localRotation = Quaternion.identity;
        oAudioSource.AddComponent(typeof(AudioSource));
        oAudioSource.GetComponent<AudioSource>().clip = clip;
        oAudioSource.GetComponent<AudioSource>().loop = Loop;
        oAudioSource.GetComponent<AudioSource>().Play();
        return oAudioSource.GetComponent<AudioSource>();
    }

    private void Start()
    {
        this.vehicle = base.GetComponent(typeof(RGK_BasicCar)) as RGK_BasicCar;
        this.EngineSource = this.CreateAudioSource(this.EngineSound, true, true);

    }

    private void Update()
    {
        this.EngineSource.pitch = 0.7f + float.Parse(Math.Round((0.5 * (Mathf.Abs(this.vehicle.Rpm) / this.vehicle.EngineRpmMax)), 2).ToString());
    }

}
