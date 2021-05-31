using UnityEngine;
using System.Collections;

public class UIAnimationMainMenu : MonoBehaviour 
{
	[Header("Main Menu")]
	public GameObject Logo;
	public GameObject LoginButton;
	public GameObject CoinsButton;
	public GameObject BattleModeButton;
	public GameObject RacingModeButton;
	public GameObject SettingsIcon;
	public GameObject StoreIcon;
	public GameObject LeaderboardIcon;
	public GameObject HelpIcon;
	[Space]
	public float animDuration = 0.5f;
	public float logoAnimDuration = 0;
	public Transform posLoginButton;
	public Transform posCoinsButton;
	public Transform posBattleMode;
	public Transform posRacingMode;


	private Vector3 initScale = new Vector3(0, 0, 0);
	private Vector3 finalScale = new Vector3(1, 1, 1);
	private Vector3 loginInitPos;
	private Vector3 coinsInitPos;
	private Vector3 battleInitPos;
	private Vector3 racingInitPos;

	void Start()
	{
		loginInitPos = LoginButton.transform.position;
		coinsInitPos = CoinsButton.transform.position;
		battleInitPos = BattleModeButton.transform.position;
		racingInitPos = RacingModeButton.transform.position;
	}

	public void BeginAnimation()
	{
//		GlobalVariables.isAnimating = true;
		iTween.ScaleTo (Logo, iTween.Hash ("scale", finalScale, "speed", logoAnimDuration, "easetype", iTween.EaseType.easeOutBounce)); 
//			"oncomplete", "ButtonsAnimationMainMenu", "oncompletetarget", this.gameObject));
		ButtonsAnimationMainMenu ();
	}

	private void ButtonsAnimationMainMenu()
	{
//		Debug.Log ("Button Animation Starts");
		iTween.MoveTo (LoginButton, iTween.Hash ("position", posLoginButton.position, "time", animDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (CoinsButton, iTween.Hash ("position", posCoinsButton.position, "time", animDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (BattleModeButton, iTween.Hash ("position", posBattleMode.position, "time", animDuration + 0.2f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (RacingModeButton, iTween.Hash ("position", posRacingMode.position, "time", animDuration + 0.2f, "easetype", iTween.EaseType.easeOutBounce, 
			"oncomplete", "IconsAnimationMainMenu", "oncompletetarget", this.gameObject));

	}

	private void IconsAnimationMainMenu()
	{
		iTween.ScaleTo (SettingsIcon, iTween.Hash ("scale", finalScale, "time", animDuration - 0.4f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo (StoreIcon, iTween.Hash ("scale", finalScale, "time", animDuration - 0.4f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo (LeaderboardIcon, iTween.Hash ("scale", finalScale, "time", animDuration - 0.4f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo (HelpIcon, iTween.Hash ("scale", finalScale, "time", animDuration - 0.4f, "easetype", iTween.EaseType.easeOutBounce,
			"oncompletetarget", this.gameObject, "oncomplete", "AnimationComplete"));
	}

	private void AnimationComplete()
	{
		GlobalVariables.isAnimating = false;
	}

	public void ResetAnimationObjects()
	{
		Logo.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		SettingsIcon.transform.localScale = initScale;
		StoreIcon.transform.localScale = initScale;
		LeaderboardIcon.transform.localScale = initScale;
		HelpIcon.transform.localScale = initScale;

		LoginButton.transform.position = loginInitPos;
		CoinsButton.transform.position = coinsInitPos;
		BattleModeButton.transform.position = battleInitPos;
		RacingModeButton.transform.position = racingInitPos;
	}
}
