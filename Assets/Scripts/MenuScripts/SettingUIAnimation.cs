using UnityEngine;
using System.Collections;

public class SettingUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Logo;
	public GameObject Music_Button;
	public GameObject SFX_Button;
	public GameObject Signout_Button;
	public GameObject Control_Button;
	public GameObject Music_Icon;
	public GameObject SFX_Icon;
	public GameObject Signout_Icon;
	public GameObject Control_Icon;

	[Header("Positions")]
	public Transform Music_Button_Pos;
	public Transform SFX_Button_Pos;
	public Transform Signout_Button_Pos;
	public Transform Control_Button_Pos;
	public Transform Music_Icon_Pos;
	public Transform SFX_Icon_Pos;
	public Transform Signout_Icon_Pos;
	public Transform Control_Icon_Pos;

	private Vector3 Music_Button_Pos_Init;
	private Vector3 SFX_Button_Pos_Init;
	private Vector3 Signout_Button_Pos_Init;
	private Vector3 Control_Button_Pos_Init;
	private Vector3 Music_Icon_Pos_Init;
	private Vector3 SFX_Icon_Pos_Init;
	private Vector3 Signout_Icon_Pos_Init;
	private Vector3 Control_Icon_Pos_Init;

	private Vector3 Logo_Actual = new Vector3(1, 1, 1);
	private Vector3 Logo_Init = new Vector3(0, 0, 0);

	[Space]
	public float AnimDuration;

	void Start()
	{
        return;
		Logo.transform.localScale = Logo_Init;

		Music_Button_Pos_Init = Music_Button.transform.position;
		SFX_Button_Pos_Init = SFX_Button.transform.position;
		Signout_Button_Pos_Init = Signout_Button.transform.position;
		Control_Button_Pos_Init = Control_Button.transform.position;
		Music_Icon_Pos_Init = Music_Icon.transform.position;
		SFX_Icon_Pos_Init = SFX_Icon.transform.position;
		Signout_Icon_Pos_Init = Signout_Icon.transform.position;
		Control_Icon_Pos_Init = Control_Icon.transform.position;
	}


	public void BeginAnimation()
	{
        return;
		iTween.ScaleTo (Logo, iTween.Hash ("scale", Logo_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce,
			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
	}


	private void ButtonsAnimation()
	{
        return;
		iTween.MoveTo (Music_Button, iTween.Hash ("position", Music_Button_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (SFX_Button, iTween.Hash ("position", SFX_Button_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Signout_Button, iTween.Hash ("position", Signout_Button_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Control_Button, iTween.Hash ("position", Control_Button_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));

		iTween.MoveTo (Music_Icon, iTween.Hash ("position", Music_Icon_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (SFX_Icon, iTween.Hash ("position", SFX_Icon_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Signout_Icon, iTween.Hash ("position", Signout_Icon_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Control_Icon, iTween.Hash ("position", Control_Icon_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}


	public void ResetPositions()
	{
        return;
		Logo.transform.localScale = Logo_Init;

		Music_Button.transform.position = Music_Button_Pos_Init;
		SFX_Button.transform.position = SFX_Button_Pos_Init;
		Signout_Button.transform.position = Signout_Button_Pos_Init;
		Control_Button.transform.position = Control_Button_Pos_Init;
		Music_Icon.transform.position = Music_Icon_Pos_Init;
		SFX_Icon.transform.position = SFX_Icon_Pos_Init;
		Signout_Icon.transform.position = Signout_Icon_Pos_Init;
		Control_Icon.transform.position = Control_Icon_Pos_Init;
	}
}
