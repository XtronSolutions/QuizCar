using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AudioData : ScriptableObject {
	public enum MusicSelection { ListOrder, Random }

	public MusicSelection musicSelection;
	public AudioClip[] music;
	public AudioClip menuSelect;
	public AudioClip menuBack;
	public int currentAudioTrack;
}