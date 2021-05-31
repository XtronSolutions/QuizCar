using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TrackSelUIAnimation : MonoBehaviour 
{
	[Header("TrackSelectionLocks")]
	public TrackSelectionLockController Track_Two_LockController;
	public TrackSelectionLockController Track_Three_LockController;

    public TrackSelectionLockController[] tracksLockController;

    [Header("Panels")]
	public GameObject Logo;
	public GameObject Track_Icon_One;
	public GameObject Track_Icon_Two;
	public GameObject Track_Icon_Three;
	public GameObject Back_Button;
	public GameObject Next_Button;
	[Header("Positions")]
	public Transform Logo_Pos;
	public Transform Back_Pos;
	public Transform Next_Pos;

	private Vector3 Track_One_Actual = new Vector3(0.7f, 0.7f, 0.7f); 
	private Vector3 Track_One_Init = new Vector3(1, 1, 1); 
	private Vector3 Track_Two_Actual = new Vector3(1, 1, 1); 
	private Vector3 Track_Two_Init = new Vector3(1, 1, 1); 
	private Vector3 Track_Three_Actual = new Vector3(1, 1, 1); 
	private Vector3 Track_Three_Init = new Vector3(1, 1, 1); 

	private Vector3 Track_Selected = new Vector3(0.8f, 0.8f, 0.8f); 

	private Vector3 Logo_Pos_Init;
	private Vector3 Back_Pos_Init;
	private Vector3 Next_Pos_Init; 

	[Space]
	public float AnimDuration;
	public float LogoAnimDuration;

	void Start()
	{
		Logo_Pos_Init = Logo.transform.position;
		Back_Pos_Init = Back_Button.transform.position;
		Next_Pos_Init = Next_Button.transform.position;

        Next_Button.SetActive(false);
//		BeginAnimation ();
	}

	public void BeginAnimation()
	{
        iTween.MoveTo(Back_Button, iTween.Hash("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));

        for(int i=0;i<tracksLockController.Length;i++)
        {
            tracksLockController[i].transform.localScale = Vector3.zero;
        }
        for (int i = 0; i < tracksLockController.Length; i++)
        {
            iTween.ScaleTo(tracksLockController[i].gameObject, iTween.Hash("scale", Track_One_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce, "delay",0.1*i));
        }
      
        return;
		iTween.MoveTo (Logo, iTween.Hash ("position", Logo_Pos.position, "time", LogoAnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "IconOneAnimation", "oncompletetarget", this.gameObject));
		IconOneAnimation();
	}

	private void IconOneAnimation()
	{
		iTween.ScaleTo (Track_Icon_One, iTween.Hash ("scale", Track_One_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
//			"oncomplete", "IconTwoAnimation", "oncompletetarget", this.gameObject));
		Invoke("IconTwoAnimation", 0.15f);
	}

	private void IconTwoAnimation()
	{
		iTween.ScaleTo (Track_Icon_Two, iTween.Hash ("scale", Track_One_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
		Invoke("IconThreeAnimation", 0.15f);
//			"oncomplete", "IconThreeAnimation", "oncompletetarget", this.gameObject));
	}
	private void IconThreeAnimation()
	{
		iTween.ScaleTo (Track_Icon_Three, iTween.Hash ("scale", Track_One_Actual, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
        //			"oncomplete", "ButtonsAnimation", "oncompletetarget", this.gameObject));
        iTween.MoveTo(Back_Button, iTween.Hash("position", Back_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
        //  Invoke("ButtonsAnimation", 0.3f);
    }

	private void ButtonsAnimation()
	{
        Next_Button.SetActive(true);
		
		iTween.MoveTo (Next_Button, iTween.Hash ("position", Next_Pos.position, "time", AnimDuration, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void ResetPositions()
	{
		Track_Icon_One.transform.localScale = Track_One_Actual;
		Track_Icon_Two.transform.localScale = Track_One_Actual;
		Track_Icon_Three.transform.localScale = Track_One_Actual;

		Logo.transform.position = Logo_Pos_Init;
		Back_Button.transform.position = Back_Pos_Init;
		Next_Button.transform.position = Next_Pos_Init;
		Track_Icon_One.GetComponent<Outline> ().enabled = false;
        Track_Icon_Two.GetComponent<Outline>().enabled = false;
        Track_Icon_Three.GetComponent<Outline>().enabled = false;

        for (int j = 0; j < tracksLockController.Length; j++)
        {
           
            {
                tracksLockController[j].transform.localScale = Track_One_Actual;
                tracksLockController[j].GetComponent<Outline>().enabled = false;
            }
        }
        Next_Button.SetActive(false);
	}


	public void SelectedTrackOne()
	{
		iTween.ScaleTo (Track_Icon_One, iTween.Hash ("scale", Track_Selected, "time", 0.15f, "easetype", iTween.EaseType.linear));
		Track_Icon_One.GetComponent<Outline> ().enabled = true;

		Track_Icon_Two.transform.localScale = Track_One_Actual;
		Track_Icon_Three.transform.localScale = Track_One_Actual;

		Track_Icon_Two.GetComponent<Outline> ().enabled = false;
		Track_Icon_Three.GetComponent<Outline> ().enabled = false;

        ButtonsAnimation();

    }

	public void SelectedTrackTwo()
	{
		if (!Track_Two_LockController.IsLocked) {
			
			iTween.ScaleTo (Track_Icon_Two, iTween.Hash ("scale", Track_Selected, "time", 0.15f, "easetype", iTween.EaseType.linear));
			Track_Icon_Two.GetComponent<Outline> ().enabled = true;

			Track_Icon_One.transform.localScale = Track_One_Actual;
			Track_Icon_Three.transform.localScale = Track_One_Actual;

			Track_Icon_One.GetComponent<Outline> ().enabled = false;
			Track_Icon_Three.GetComponent<Outline> ().enabled = false;

            ButtonsAnimation();

        }
	}

	public void SelectedTrackThree()
	{
		if (!Track_Three_LockController.IsLocked) {
			
			iTween.ScaleTo (Track_Icon_Three, iTween.Hash ("scale", Track_Selected, "time", 0.15f, "easetype", iTween.EaseType.linear));
			Track_Icon_Three.GetComponent<Outline> ().enabled = true;

			Track_Icon_One.transform.localScale = Track_One_Actual;
			Track_Icon_Two.transform.localScale = Track_One_Actual;

			Track_Icon_One.GetComponent<Outline> ().enabled = false;
			Track_Icon_Two.GetComponent<Outline> ().enabled = false;

            ButtonsAnimation();

        }
	}

    public void SelectedTrack(int i)
    {
        i = i - 1;
        if(!tracksLockController[i].IsLocked)
        {
            iTween.ScaleTo(tracksLockController[i].gameObject, iTween.Hash("scale", Track_Selected, "time", 0.15f, "easetype", iTween.EaseType.linear));
            tracksLockController[i].GetComponent<Outline>().enabled = true;

            for(int j=0;j<tracksLockController.Length;j++)
            {
                if(j!=i)
                {
                    tracksLockController[j].transform.localScale = Track_One_Actual;
                    tracksLockController[j].GetComponent<Outline>().enabled = false;
                }
            }


         

            ButtonsAnimation();
        }
    }
}
