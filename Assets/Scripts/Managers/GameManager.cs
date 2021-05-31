using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Photon.Chat;

public class GameManager : SingeltonBase<GameManager> {

	private GameState previousGameState;
	private GameState currentGameState;
	private int RateUsCount = 0;
	public GameObject invitationDialog;
	private SoundState currentSoundState;

	public string invitationID = "";
	public bool timerStart = false;
	public bool isFreeOfTime = false;
	public bool logged = false;
	public string facebookIDMy;
	public string nameMy;
	public bool LinkFbAccount = false;
	public string avatarMyUrl;
	public List<string> friendsIDForStatus = new List<string> ();
	public ChatClient chatClient;
	public List<string[]> friendsStatuses = new List<string[]> ();
	public  List<GameObject> friendsObjects = new List<GameObject>();
	public enum GameState { 
		SPLASH,
		HOME
	};

	public enum SoundState { 
		CLICK,
		BACK,
		OBSTACLE_COLLIDE,
		CAR_COLLIDE,
		POWERUPS_COLLECT,
		WATER_SPLASH
	};

	public delegate void onToggleBrakes();
	public static event onToggleBrakes onToggleBraked; 

	public delegate void onSafetyTripTimerUp();
	public static event onSafetyTripTimerUp onSafetyTripTimerUped;

	public delegate void onSafetyTripBusStarts();
	public static event onSafetyTripBusStarts onSafetyTripBusStarted;

	// Use this for initialization
	void Start () {

//		Application.targetFrameRate = 60;
//		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	void OnApplicationPause(bool status) {
//		if(status && GameManager.Instance.GetCurrentGameState() == GameManager.GameState.GAME_PLAY_BEGIN) {
//			GamePlayScript.Instance.togglePauseGame ();
//
//		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable(){

		#if UNITY_IPHONE
		#endif
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	void OnDisable(){

		#if UNITY_IPHONE
		#endif

	}

	/// <summary>
	/// Use for Timer Countdown in the Meditation Scene
	/// in order to maintain State and to keep running 
	/// on other scenes
	/// </summary> 
	void Update()
	{
	

	}


	/// <summary>
	/// Gets the state of the sound.
	/// </summary>
	/// <returns>The sound state.</returns>
	public SoundState GetSoundState() {
		return currentSoundState;
	}

	/// <summary>
	/// Sets the state of the sound.
	/// </summary>
	/// <param name="state">State.</param>
	public void SetSoundState(SoundState state) 
	{
		currentSoundState = state;
	}

	/// <summary>
	/// Changes the state of the sound.
	/// </summary>
	/// <param name="soundState">Sound state.</param>
	public void ChangeSoundState (SoundState soundState) 
	{
		SetSoundState(soundState);
		SoundManager.Instance.PlaySound();
	}

	public void ChangeSoundState (SoundState soundState,float pExtraDelay, SoundManager.AudioCallback callback ) 
	{
		SetSoundState(soundState);
		//SoundManager.Instance.PlaySoundWithCallback(pExtraDelay, callback);
	}



	/// <summary>
	/// Plaies the background music.
	/// </summary>
	/// <param name="pPlay">If set to <c>true</c> p play.</param>
//	public void playBgMusic(bool pPlay, int pMusicType) {
//		SoundManager.Instance.PlayBgMusic (pPlay, pMusicType);
//	}

	/// <summary>
	/// Plaies the background music.
	/// </summary>
	/// <param name="pPlay">If set to <c>true</c> p play.</param>
	/// <param name="pPause">If set to <c>true</c> p pause.</param>
	public void playBgMusic(bool pPlay, bool pPause, int pMusicType) 
	{
		SoundManager.Instance.PlayBgMusic (pPlay, pPause, pMusicType);
	}
	
	#region StateHandler

	/// <summary>
	/// Gets the state of the current game.
	/// </summary>
	/// <returns>The current game state.</returns>
	public GameState GetCurrentGameState() 
	{
		return currentGameState;
	}

	/// <summary>
	/// Gets the state of the previous game.
	/// </summary>
	/// <returns>The previous game state.</returns>
	public GameState GetPreviousGameState() {
		return previousGameState;
	}

	/// <summary>
	/// Sets the state of the previous game.
	/// </summary>
	/// <param name="state">State.</param>
	public void SetPreviousGameState(GameState state) {
		previousGameState=state;
	}

	/// <summary>
	/// Sets the state of the game.
	/// </summary>
	/// <param name="state">State.</param>
	private void SetGameState(GameState state) {
		previousGameState = currentGameState;
		currentGameState = state;
	}

	/// <summary>
	/// Changes the state.
	/// </summary>
	/// <param name="state">State.</param>
	public void ChangeState (GameState state) {
		
		SetGameState(state);

		switch(currentGameState){
		case GameState.SPLASH:
			break;
		case GameState.HOME:
			break;
		}

	}

	/// <summary>
	/// Changes the sound.
	/// </summary>
	/// <param name="isMute">If set to <c>true</c> is mute.</param>
	private void changeSound(bool isMute){
		//Implementation Changed @@Deprecated
		if(isMute) {

		} else {

		}
	}
		
	#endregion

	/// <summary>
	/// Purchases the product result.
	/// </summary>
	/// <param name="package">Package.</param>
	/// <param name="result">If set to <c>true</c> result.</param>
	public void PurchaseProductResult(string package, bool result) {
		Debug.Log("purchase prodcut result " + result );

		if(result) {

//			if(package == .PACKAGE_REMOVEADS)
//			{
//				
//			}
//
//		} else{
//
//			}
		}

	}

	#region Achievements
	
	/// <summary>
	/// Locked Achievement.
	/// </summary>
	/// <param name="achieveID">Achieve ID.</param>
	public void LockAchievement(string achieveID){

//		GCManager.Instance.UnlockAchievement (achieveID);
	}
	
	#endregion
	
	/// <summary>
	/// Posts the on facebook.
	/// </summary>
	public void postOnFacebook(){
		#if UNITY_IPHONE
//
//		if (FacebookBinding.canUserUseFacebookComposer()){
//			string text = this.getSharingText();
//			FacebookBinding.showFacebookComposer(text,null,RotatioConstants.APP_LINK);
//		} else {
//			Debug.Log("Facebook not supported");
//		}
//		
		#endif
		
		#if UNITY_ANDROID

//		FacebookManager.Instance.Login("public_profile,publish_actions",
//		delegate()
//		{
//			if (FacebookManager.Instance.LoggedIn)
//			{
//				string text = this.getSharingText();
//				FacebookManager.Instance.PostToWall(text);
//			}
//			
//		},
//		delegate(string reason)
//		{
//			
//		});
		
		#endif
	}


	#region TwitterEvents

	/// <summary>
	/// Posts the on twitter.
	/// </summary>
	public void postOnTwitter() {

		#if UNITY_IPHONE

//			if (TwitterBinding.isTweetSheetSupported()){
//				string text = this.getSharingText();
//			TwitterBinding.showTweetComposer(text,null,RotatioConstants.APP_LINK);
//			} else {
//				Debug.Log("TweetSheet not supported");
//			}

		#endif

		#if UNITY_ANDROID
//			if(TwitterAndroid.isLoggedIn()) {
//				createTweet ();
//			} else {
//				TwitterAndroid.showLoginDialog();
//			}

		#endif
	}

	/// <summary>
	/// Gets the sharing text.
	/// </summary>
	/// <returns>The sharing text.</returns>
	public string getSharingText() {
		return "";//RotatioConstants.getShareText ();
	}

	/// <summary>
	/// Creates the tweet.
	/// </summary>
	private void createTweet() {

		#if UNITY_ANDROID

//			string text = this.getSharingText();
//			TwitterAndroid.postStatusUpdate(text + " " + RotatioConstants.appLink());

		#endif
	}

	/// <summary>
	/// Logins the succeeded.
	/// </summary>
	/// <param name="username">Username.</param>
	void loginSucceeded( string username ) {
		Debug.Log( "Local Methods: Successfully logged in to Twitter: " + username );
		createTweet ();
	}
	
	/// <summary>
	/// Logins the failed.
	/// </summary>
	/// <param name="error">Error.</param>
	void loginFailed( string error ) {
		Debug.Log( "Local Methods: Twitter login failed: " + error );
	}
	
	/// <summary>
	/// Requests the did fail event.
	/// </summary>
	/// <param name="error">Error.</param>
	void requestDidFailEvent( string error ) {
		Debug.Log( "Local Methods: requestDidFailEvent: " + error );
	}
	
	/// <summary>
	/// Requests the did finish event.
	/// </summary>
	/// <param name="result">Result.</param>
	void requestDidFinishEvent( object result ) {
		if( result != null )
		{
			Debug.Log( "Local Methods: requestDidFinishEvent" );
		}
	}
	
	/// <summary>
	/// Tweets the sheet completed event.
	/// </summary>
	/// <param name="didSucceed">If set to <c>true</c> did succeed.</param>
	void tweetSheetCompletedEvent( bool didSucceed ) {
		Debug.Log( "Local Methods: tweetSheetCompletedEvent didSucceed: " + didSucceed );
	}

	#endregion

	#if UNITY_IPHONE

	// facebook events
	void facebookComposeCompletedEvent( bool status) {
		Debug.Log( "Local Methods: facebookComposeCompletedEvent didSucceed: " + status );
	}
	
	#endif
	public void onSafetyTripTimer() {
		if(onSafetyTripTimerUped != null) {
			onSafetyTripTimerUped ();
			
		}
	}

	/// <summary>
	/// Ons the bus started.
	/// </summary>
	public void onBusStarted () {
		if (onSafetyTripBusStarted != null) {
			onSafetyTripBusStarted ();
		}
	}
}
