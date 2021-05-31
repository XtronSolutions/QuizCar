using UnityEngine;
using System.Collections;

public class MainSelectionUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Logo;
	public GameObject Buggy_Icon;
	public GameObject Character_Icon;
	public GameObject Weapon_Icon;
	public GameObject Back_Button;
	public GameObject Next_Button;
	[Header("Positions")]
	public Transform Buggies_Pos;
	public Transform Character_Pos;
	public Transform Weapon_Pos;
	public Transform Back_Pos;
	public Transform Next_Pos;

	private Vector3 Logo_Actual = new Vector3(0.7f, 0.7f, 0.7f); 
	private Vector3 Logo_Init = new Vector3(0, 0, 0); 

	private Vector3 Buggies_Pos_Init;
	private Vector3 Character_Pos_Init;
	private Vector3 Weapon_Pos_Init;
	private Vector3 Back_Pos_Init;
	private Vector3 Next_Pos_Init; 

	[Space]
	public float AnimDuration;
	public float LogoAnimDuration;

	void Start()
	{
		Buggies_Pos_Init = Buggy_Icon.transform.position;
		Character_Pos_Init = Character_Icon.transform.position;
		Weapon_Pos_Init = Weapon_Icon.transform.position;
		Back_Pos_Init = Back_Button.transform.position;
		Next_Pos_Init = Next_Button.transform.position;
	}
    void OnEnable()
    {
        GameObject.FindObjectOfType<MenuManager>().Racing_Mode_Main.SetActive(false);
    }

	public void BeginAnimation()
	{
		iTween.ScaleTo (Logo, iTween.Hash("scale", Logo_Actual, "speed", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce,
			"oncomplete", "IconsAnimationMainMenu", "oncompletetarget", this.gameObject));
	}

	private void IconsAnimationMainMenu()
	{
		iTween.MoveTo (Buggy_Icon, iTween.Hash ("position", Buggies_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Character_Icon, iTween.Hash ("position", Character_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Weapon_Icon, iTween.Hash ("position", Weapon_Pos.position, "time", AnimDuration + 0.2f, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "ButtonsAnimationMainMenu", "oncompletetarget", this.gameObject));
		ButtonsAnimationMainMenu();
	}

	private void ButtonsAnimationMainMenu()
	{
		iTween.MoveTo (Back_Button, iTween.Hash ("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Next_Button, iTween.Hash ("position", Next_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
		Logo.transform.localScale = Logo_Init;
		Buggy_Icon.transform.position = Buggies_Pos_Init;
		Character_Icon.transform.position = Character_Pos_Init;
		Weapon_Icon.transform.position = Weapon_Pos_Init;
		Back_Button.transform.position = Back_Pos_Init;
		Next_Button.transform.position = Next_Pos_Init;
	}

}
