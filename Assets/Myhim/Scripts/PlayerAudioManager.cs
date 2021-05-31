using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour {

	public static PlayerAudioManager instance { get; private set;}
	public AudioSource MissileSource;
	public AudioSource IceSource;
	public AudioSource TrapSource;
	public AudioSource NitroSource;
	public AudioClip coinPickUp;
	public AudioClip powerUpPickup;
	// Use this for initialization

	void Awake()
	{
		if (instance == null)
			instance = this;
//		else
//			Destroy (gameObject);
	}

	void Start () {
		
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void PlayMissileExplosionSound(){
	
		if (!MissileSource.isPlaying) {
		
			MissileSource.Play ();
		}
	}

	public void PlayIceExplosionSound(){

		if (!IceSource.isPlaying) {

			IceSource.Play ();
		}
	}

	public void PlayTrapSound(){

		if (!TrapSource.isPlaying) {

			TrapSource.Play ();
		}
	}

	public void PlayNitroSound(){

		if (!NitroSource.isPlaying) {

			NitroSource.Play ();
		}
	}

	public void PlayCoinPickUp(){
		MissileSource.PlayOneShot (coinPickUp);
	}

	public void PlayPowerUpPickUp(){
		MissileSource.PlayOneShot (powerUpPickup);
	}
}
