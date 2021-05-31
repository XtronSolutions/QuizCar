using UnityEngine;
using System.Collections;
using System;

public class SoundManager : SingeltonBase<SoundManager>{

	public AudioSource gamePlayEffectsSource;
	float _vol = 0;
	// Use this for initialization

	[Space]
	[Header("BG Sounds")]
	public AudioClip MainMenuBGSound;
	public AudioClip GameBGSound;

	[Space]
	[Header("Ambience Sounds")]
	public AudioClip OceanSound;
	public AudioClip BirdsSound;
	public AudioClip WindSound;

	[Space]
	[Header("SFX")]
	public AudioClip ClickSound;
	public AudioClip BackSound;

	public AudioClip ObstacleCollideSound;
	public AudioClip PowerUpsCollectSound;
	public AudioClip CarCollideSound;
	public AudioClip WaterSplashSound;

    // **************************************

    public enum GAME_BG_SOUND
    {
        STARCATCHER,
        VORTEX,
        HIDENSEEK
        
    }	
	public const int MAIN_MENU_BG = 1;
	public const int GAMEPLAY_BG = 2;

	public enum Fade {In, Out};

	public enum VoiceState {
		VOICE_AMAZING,
		VOICE_ASTOUNDING,
		VOICE_AWESOME,
		VOICE_COOL,
		VOICE_GOOD,
		VOICE_GREAT,
		VOICE_NICE,
		VOICE_STRIKING
	};

	public delegate void AudioCallback();


	/// <summary>
	/// Music Fader.
	/// </summary>
	/// <param name="fadeTime">Fade time.</param>
	/// <param name="pFade">P fade.</param>
	public void musicFader (float fadeTime, Fade pFade ) {
		StartCoroutine(FadeAudio(fadeTime, pFade));
	}

    /// <summary>
    /// Fades the audio.
    /// </summary>
    /// <returns>The audio.</returns>
    /// <param name="timer">Timer.</param>
    /// <param name="fadeType">Fade type.</param>
    /// 
    private IEnumerator FadeAudio (float timer, Fade fadeType) {
		float start = fadeType == Fade.In? 0.0F : 1.0F;
		float end = fadeType == Fade.In? 1.0f : 0.0f;
		float vol = 0.0f;
		float step = 1.0f/timer;

		while (vol <= 1.0f) {
			vol += step * Time.deltaTime;
//			this.GetComponent<AudioSource>().volume = Mathf.Lerp(start, end, vol);
			_vol = Mathf.Lerp(start, end, vol);

			yield return new WaitForSeconds(step * Time.deltaTime);
		}
	}

    /// <summary>
    /// Play the background music.
    /// </summary>
    public void PlayBgMusic(bool pPlay, bool isPause, int pMusicType) {
//		CentralVariables.isSound = true;
		if(CentralVariables.isSound) {
			if(this.GetComponent<AudioSource>()!=null) 
			{
				if(pPlay) {
					switch(pMusicType) 
					{
					case MAIN_MENU_BG:
						this.GetComponent<AudioSource>().clip = MainMenuBGSound;
						this.GetComponent<AudioSource>().volume = 0.3f;
						break;
					case GAMEPLAY_BG:
						this.GetComponent<AudioSource>().clip = GameBGSound;
						this.GetComponent<AudioSource>().volume = 0.3f;
						break;
					}
					this.GetComponent<AudioSource>().Play ();
				} 
				else 
				{
					if(isPause) 
					{
						this.GetComponent<AudioSource>().Pause();
					} else 
					{
						this.GetComponent<AudioSource>().Stop ();
					}
				}
			}
		} else 
		{
			if(this.GetComponent<AudioSource>()!=null)
					this.GetComponent<AudioSource>().Stop ();
		}

	}


	/// <summary>
	/// Plays Sound Effects throughout the game
	/// </summary>
	public void PlaySound()
	{
		if (CentralVariables.isSound) {

			if(gamePlayEffectsSource != null) {
				switch (GameManager.Instance.GetSoundState ()) {
					
				case GameManager.SoundState.CLICK:
					gamePlayEffectsSource.PlayOneShot(ClickSound);
					break;
				case GameManager.SoundState.BACK:
					gamePlayEffectsSource.PlayOneShot(BackSound);
					break;
				case GameManager.SoundState.CAR_COLLIDE:
					gamePlayEffectsSource.PlayOneShot(CarCollideSound);
					break;
				case GameManager.SoundState.OBSTACLE_COLLIDE:
					gamePlayEffectsSource.PlayOneShot(ObstacleCollideSound);
					break;
				case GameManager.SoundState.POWERUPS_COLLECT:
					gamePlayEffectsSource.PlayOneShot(PowerUpsCollectSound);
					break;
				case GameManager.SoundState.WATER_SPLASH:
					gamePlayEffectsSource.PlayOneShot(WaterSplashSound);
					break;
				}
			}
		}
 	 }

	/// <summary>
	/// Random voices.
	/// </summary>
	/// <returns>The voice.</returns>
	public VoiceState randomVoice() {
		
		Array values = Enum.GetValues (typeof(VoiceState));
		System.Random random = new System.Random ();
		return (VoiceState)values.GetValue (random.Next (values.Length));
	}

  
	/// <summary>
	/// Play the sound with callback.
	/// </summary>
	/// <param name="clip">Clip.</param>
	/// <param name="callback">Callback.</param>
	public void PlaySoundWithCallback(AudioClip clip, float pExtraDelay, AudioCallback callback){
		GetComponent<AudioSource>().PlayOneShot(clip);
//		if(GameManager.Instance.GetCurrentGameState() == GameManager.GameState.MAIN_MENU){
			StartCoroutine(DelayedCallback(clip.length+pExtraDelay, callback));
//		}
	}

	/// <summary>
	/// Delayed callback.
	/// </summary>
	/// <returns>The callback.</returns>
	/// <param name="time">Time.</param>
	/// <param name="callback">Callback.</param>
	private IEnumerator DelayedCallback(float time, AudioCallback callback)	{
		yield return new WaitForSeconds(time);
		callback();
	}

	/// <summary>
	/// Audio finished.
	/// </summary>
	void AudioFinished() {
//		GameManager.Instance.SetSoundState (GameManager.SoundState.VOICE_ROTATIO);
//		PlaySound ();
	}

}
