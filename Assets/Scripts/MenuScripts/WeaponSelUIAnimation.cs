using UnityEngine;
using System.Collections;

public class WeaponSelUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Logo;
	public GameObject Weapon_Icon_One;
	public GameObject Weapon_Icon_Two;
	public GameObject Weapon_Icon_Three;
	public GameObject Back_Button;
	public GameObject Next_Button;
	[Header("Positions")]
	public Transform Logo_Pos;
	public Transform Back_Pos;
	public Transform Next_Pos;

	private Vector3 Weapon_One_Actual = new Vector3(1, 1, 1); 
	private Vector3 Weapon_One_Init = new Vector3(0, 0, 0); 
	private Vector3 Weapon_Two_Actual = new Vector3(1, 1, 1); 
	private Vector3 Weapon_Two_Init = new Vector3(0, 0, 0); 
	private Vector3 Weapon_Three_Actual = new Vector3(1, 1, 1); 
	private Vector3 Weapon_Three_Init = new Vector3(0, 0, 0); 

	private Vector3 Weapon_Clicked = new Vector3(1.15f, 1.15f, 1.15f); 

	private Vector3 Logo_Pos_Init;
	private Vector3 Back_Pos_Init;
	private Vector3 Next_Pos_Init; 

	[Space]
	public float AnimDuration;

	void Start()
	{
		Logo_Pos_Init = Logo.transform.position;
		Back_Pos_Init = Back_Button.transform.position;
		Next_Pos_Init = Next_Button.transform.position;
		BeginAnimation ();
	}

	public void BeginAnimation()
	{
		iTween.MoveTo (Logo, iTween.Hash ("position", Logo_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "IconOneAnimation", "oncompletetarget", this.gameObject));
		Invoke("IconOneAnimation", 0.4f);
	}

	private void IconOneAnimation()
	{
		iTween.ScaleTo (Weapon_Icon_One, iTween.Hash ("scale", Weapon_One_Actual, "time", AnimDuration - 0.1f, "easetype", iTween.EaseType.easeOutBounce,
			"oncomplete", "IconTwoAnimation", "oncompletetarget", this.gameObject));
	}

	private void IconTwoAnimation()
	{
		iTween.ScaleTo (Weapon_Icon_Two, iTween.Hash ("scale", Weapon_One_Actual, "time", AnimDuration - 0.1f, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "IconThreeAnimation", "oncompletetarget", this.gameObject));
		Invoke ("IconThreeAnimation", 0.15f);
	}
	private void IconThreeAnimation()
	{
		iTween.ScaleTo (Weapon_Icon_Three, iTween.Hash ("scale", Weapon_One_Actual, "time", AnimDuration - 0.1f, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
		Invoke ("ButtonsAnimation", 0.2f);
	}

	private void ButtonsAnimation()
	{
		iTween.MoveTo (Back_Button, iTween.Hash ("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Next_Button, iTween.Hash ("position", Next_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
		Weapon_Icon_One.transform.localScale = Weapon_One_Init;
		Weapon_Icon_Two.transform.localScale = Weapon_One_Init;
		Weapon_Icon_Three.transform.localScale = Weapon_One_Init;

		Logo.transform.position = Logo_Pos_Init;
		Back_Button.transform.position = Back_Pos_Init;
		Next_Button.transform.position = Next_Pos_Init;
	}

	public void WeaponOneClicked()
	{
		iTween.ScaleTo (Weapon_Icon_One, iTween.Hash ("scale", Weapon_Clicked, "time", 0.15f, "easetype", iTween.EaseType.linear));

		iTween.ScaleTo (Weapon_Icon_Two, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo (Weapon_Icon_Three, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void WeaponTwoClicked()
	{
		iTween.ScaleTo (Weapon_Icon_Two, iTween.Hash ("scale", Weapon_Clicked, "time", 0.15f, "easetype", iTween.EaseType.linear));

		iTween.ScaleTo (Weapon_Icon_One, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo (Weapon_Icon_Three, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void WeaponThreeClicked()
	{
		iTween.ScaleTo (Weapon_Icon_Three, iTween.Hash ("scale", Weapon_Clicked, "time", 0.15f, "easetype", iTween.EaseType.linear));

		iTween.ScaleTo (Weapon_Icon_Two, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
		iTween.ScaleTo (Weapon_Icon_One, iTween.Hash ("scale", Weapon_One_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}
}
