using UnityEngine;
using System.Collections;

public class LoginUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Logo;
	public GameObject Register_Icon;
	public GameObject Login_Icon;
	public GameObject Offline_Icon;
	public GameObject Register_Panel;
	public GameObject Login_Panel;
	[Header("Positions")]
	public Transform Register_Pos;
	public Transform Login_Pos;
	public Transform Offline_Pos;

	private Vector3 Logo_Actual = new Vector3(1, 1, 1); 
	private Vector3 Logo_Init = new Vector3(0.4f, 0.4f, 0.4f); 

	private Vector3 Register_Actual = new Vector3(1, 1, 1); 
	private Vector3 Register_Init = new Vector3(0.5f, 0.5f, 0.5f); 
	private Vector3 Login_Actual = new Vector3(1, 1, 1); 
	private Vector3 Login_Init = new Vector3(0.5f, 0.5f, 0.5f); 
	private Vector3 Offline_Actual = new Vector3(1, 1, 1); 
	private Vector3 Offline_Init = new Vector3(0.5f, 0.5f, 0.5f);

	private Vector3 Register_Pos_Init;
	private Vector3 Login_Pos_Init;
	private Vector3 Offline_Pos_Init;

	[Space]
	public float AnimDuration;
	public float LogoAnimDuration;

	void Start()
	{
		Register_Pos_Init = Register_Icon.transform.position;
		Login_Pos_Init = Login_Icon.transform.position;
		Offline_Pos_Init = Offline_Icon.transform.position;
	}


	public void BeginAnimation()
	{
		iTween.ScaleTo (Logo, iTween.Hash("scale", Logo_Actual, "speed", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce,
			"oncomplete", "IconsAnimationMainMenu", "oncompletetarget", this.gameObject));
	}

	private void IconsAnimationMainMenu()
	{
		iTween.MoveTo (Register_Icon, iTween.Hash ("position", Register_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Login_Icon, iTween.Hash ("position", Login_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Offline_Icon, iTween.Hash ("position", Offline_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void RegisterAnimation()
	{
		iTween.ScaleTo (Register_Panel, iTween.Hash ("scale", Register_Actual, "speed", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void LoginAnimation()
	{
		iTween.ScaleTo (Login_Panel, iTween.Hash ("scale", Login_Actual, "speed", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
		Logo.transform.localScale = Logo_Init;
		Register_Icon.transform.position = Register_Pos_Init;
		Login_Icon.transform.position = Login_Pos_Init;
		Offline_Icon.transform.position = Offline_Pos_Init;
	}

	public void ResetRegister()
	{
		//Register_Panel.transform.localScale = Register_Init;
	}

	public void ResetLogin()
	{
		//Login_Panel.transform.localScale = Login_Init;
	}
}
