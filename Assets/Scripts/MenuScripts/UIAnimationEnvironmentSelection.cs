using UnityEngine;
using System.Collections;

public class UIAnimationEnvironmentSelection : MonoBehaviour 
{
	[Header("Environment Selection")]
	public GameObject Label;
	public GameObject ForestEnvironment;
	public GameObject DesertEnvironment;
	public GameObject CoinsButton;
	public GameObject BackButton;
	public GameObject NextButton;
	public GameObject EnvironmentIcon;
	public GameObject BuggiesIcon;
	public GameObject CharactersIcon;
	public GameObject WeaponsIcon;

	[Space]
	public Transform CoinsPosition;
	public Transform BackPos;
	public Transform NextPos;

	private Vector3 coinInitPos;
	private Vector3 backInitPos;
	private Vector3 nextInitPos;

	public float animDuration;
	public float LogoAnimDuration;

	private Vector3 initScale = new Vector3 (0, 0, 0);
	private Vector3 finalScale = new Vector3 (1, 1, 1);
	private Vector3 ClickedScale = new Vector3 (1.15f, 1.15f, 1.15f);

	void Start()
	{
		coinInitPos = CoinsButton.transform.position;
		backInitPos = BackButton.transform.position;
		nextInitPos = NextButton.transform.position;
	}

	public void BeginAnimation()
	{
//		GlobalVariables.isAnimating = true;
		iTween.ScaleTo(Label, iTween.Hash("scale", finalScale, "time", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo(ForestEnvironment, iTween.Hash("scale", finalScale, "speed", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo (DesertEnvironment, iTween.Hash ("scale", finalScale, "speed", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
		ButtonsAnimation();
	}

	private void ButtonsAnimation()
	{
		iTween.MoveTo(CoinsButton, iTween.Hash("position", CoinsPosition.position, "time", animDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo(BackButton, iTween.Hash("position", BackPos.position, "time", animDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo(NextButton, iTween.Hash("position", NextPos.position, "time", animDuration, "easetype", iTween.EaseType.easeOutBounce, 
			"oncomplete", "IconsAnimation", "oncompletetarget", this.gameObject));
	}

	private void IconsAnimation()
	{
		iTween.ScaleTo(EnvironmentIcon, iTween.Hash("scale", finalScale, "time", 0.3f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo(BuggiesIcon, iTween.Hash("scale", finalScale, "time", 0.3f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo(CharactersIcon, iTween.Hash("scale", finalScale, "time", 0.3f, "easetype", iTween.EaseType.easeOutBounce));
		iTween.ScaleTo(WeaponsIcon, iTween.Hash("scale", finalScale, "time", 0.3f, "easetype", iTween.EaseType.easeOutBounce,
			"oncompletetarget", this.gameObject, "oncomplete", "AnimationComplete"));
	}

	private void AnimationComplete()
	{
		GlobalVariables.isAnimating = false;
	}

	public void ResetAnimationObjects()
	{
		Label.transform.localScale = initScale;
		ForestEnvironment.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		DesertEnvironment.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		EnvironmentIcon.transform.localScale = initScale;
		BuggiesIcon.transform.localScale = initScale;
		WeaponsIcon.transform.localScale = initScale;
		CharactersIcon.transform.localScale = initScale;

		CoinsButton.transform.position = coinInitPos;
		BackButton.transform.position = backInitPos;
		NextButton.transform.position = nextInitPos;
	}

	public void OnClickedForest(){
		iTween.ScaleTo(ForestEnvironment, iTween.Hash("scale", ClickedScale, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}
	public void OnReleasedForest(){
		iTween.ScaleTo(ForestEnvironment, iTween.Hash("scale", finalScale, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void OnClickedDesert(){
		iTween.ScaleTo(DesertEnvironment, iTween.Hash("scale", ClickedScale, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}
	public void OnReleasedDesert(){
		iTween.ScaleTo(DesertEnvironment, iTween.Hash("scale", finalScale, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

}
