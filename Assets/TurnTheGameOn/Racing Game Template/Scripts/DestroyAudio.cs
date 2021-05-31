using UnityEngine;
using System.Collections;

public class DestroyAudio : MonoBehaviour {

	AudioSource audioRef;

	void Awake(){
		audioRef = gameObject.GetComponent<AudioSource> ();
	}

	void Update () {		
		if (audioRef.isPlaying) {			
		} else {
			Destroy(gameObject);
		}
	}
}