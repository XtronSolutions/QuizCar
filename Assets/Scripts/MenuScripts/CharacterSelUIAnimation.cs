using UnityEngine;
using System.Collections;

public class CharacterSelUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Logo;
	public GameObject Texture_Panel;
	public GameObject Specs_Panel;
	public GameObject Buy_Button;
	public GameObject Back_Button;
	public GameObject Next_Button;
	public GameObject CustomizePanel;

	[Header("Positions")]
	public Transform Logo_Pos;
	public Transform Specs_Pos;
	public Transform Texture_Pos;
	public Transform Buy_Pos;
	public Transform Back_Pos;
	public Transform Next_Pos;
	public Transform CustomizePanel_Pos;

	private Vector3 Logo_Pos_Init;
	private Vector3 Specs_Pos_Init;
	private Vector3 Texture_Pos_Init;
	private Vector3 Buy_Pos_Init;
	private Vector3 Back_Pos_Init;
	private Vector3 Next_Pos_Init;
	private Vector3 Customize_Pos_Init;

	[Space]
	public float AnimDuration;
	public float LogoAnimDuration;

	void Start()
	{
		Logo_Pos_Init = Logo.transform.position;
		Specs_Pos_Init = Specs_Panel.transform.position;
		Texture_Pos_Init = Texture_Panel.transform.position;
		Buy_Pos_Init = Buy_Button.transform.position;
		Back_Pos_Init = Back_Button.transform.position;
		Next_Pos_Init = Next_Button.transform.position;
		Customize_Pos_Init = CustomizePanel.transform.position;
		BeginAnimation ();
	}


	public void BeginAnimation()
	{
		iTween.MoveTo (Logo, iTween.Hash ("position", Logo_Pos.position, "time", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
		Invoke("ButtonsAnimation", 0.2f);
	}

	private void ButtonsAnimation()
	{
		iTween.MoveTo (Buy_Button, iTween.Hash ("position", Buy_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Back_Button, iTween.Hash ("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Next_Button, iTween.Hash ("position", Next_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (CustomizePanel, iTween.Hash ("position", CustomizePanel_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "PanelsAnimation", "oncompletetarget", this.gameObject));
		Invoke("PanelsAnimation", 0.2f);
	}

	private void PanelsAnimation()
	{
		iTween.MoveTo (Texture_Panel, iTween.Hash ("position", Texture_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Specs_Panel, iTween.Hash ("position", Specs_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
		Logo.transform.position = Logo_Pos_Init;
		Specs_Panel.transform.position = Specs_Pos_Init;
		Texture_Panel.transform.position = Texture_Pos_Init;
		Buy_Button.transform.position = Buy_Pos_Init;
		Back_Button.transform.position = Back_Pos_Init;
		Next_Button.transform.position = Next_Pos_Init;
		CustomizePanel.transform.position = Customize_Pos_Init;
	}


}
