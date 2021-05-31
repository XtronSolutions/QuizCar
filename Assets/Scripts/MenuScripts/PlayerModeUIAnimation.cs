using UnityEngine;
using System.Collections;

public class PlayerModeUIAnimation : MonoBehaviour 
{
	[Header("Panels")]
	public GameObject Mode_Panel;
	public GameObject Mode_Single;
	public GameObject Mode_Multi;
	public GameObject Back_Button;
	public GameObject Next_Button;

	[Header("Positions")]
	public Transform Back_Pos;
	public Transform Next_Pos;

	private Vector3 Mode_Actual = new Vector3(1, 1, 1); 
	private Vector3 Mode_Init = new Vector3(0.4f, 0.4f, 0.4f); 
	private Vector3 Mode_Clicked = new Vector3(1.15f, 1.15f, 1.15f); 

	private Vector3 Back_Pos_Init;
	private Vector3 Next_Pos_Init;

	[Space]
	public float AnimDuration;

	void Start()
	{
        return;
		Back_Pos_Init = Back_Button.transform.position;
		Next_Pos_Init = Next_Button.transform.position;
				BeginAnimation ();
	}


	public void BeginAnimation()
	{
        return;
		iTween.ScaleTo (Mode_Panel, iTween.Hash ("scale", Mode_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce,
			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
	}

	private void ButtonsAnimation()
	{
		iTween.MoveTo (Back_Button, iTween.Hash ("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		iTween.MoveTo (Next_Button, iTween.Hash ("position", Next_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
        return;
		Mode_Panel.transform.localScale = Mode_Init;

		Back_Button.transform.position = Back_Pos_Init;
		Next_Button.transform.position = Next_Pos_Init;
	}

	public void SingleClickedAnimation()
	{
		iTween.ScaleTo (Mode_Single, iTween.Hash ("scale", Mode_Clicked, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void MultiClickedAnimation()
	{
		iTween.ScaleTo (Mode_Multi, iTween.Hash ("scale", Mode_Clicked, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void SingleReleasedAnimation()
	{
		iTween.ScaleTo (Mode_Single, iTween.Hash ("scale", Mode_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

	public void MultiReleaseAnimation()
	{
		iTween.ScaleTo (Mode_Multi, iTween.Hash ("scale", Mode_Actual, "time", 0.15f, "easetype", iTween.EaseType.linear));
	}

}
