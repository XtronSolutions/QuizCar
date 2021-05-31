using UnityEngine;
using System.Collections;

public class CameraFlying : MonoBehaviour 
{
	public Transform[] CameraPositions;

	public Transform MainMenu_Pos;
	public Transform Player_Main_Mode_Pos;
	public Transform Player_Multi_Mode_Pos;
	public Transform Player_MultiPlayer_Mode_Pos;
	public Transform Environment_Selection_Pos;
	public Transform Main_Selection_Pos;
	public Transform Buggy_Selection_One_Pos;
	public Transform Buggy_Selection_Two_Pos;
	public Transform Buggy_Selection_Three_Pos;
	public Transform Buggy_Selection_Four_Pos;
	public Transform Character_Selection_One_Pos;
	public Transform Weapon_Selection_Two_Pos;
	public Transform Track_Selection_Pos;

	public GameObject camera;
	public float flyingSpeed = 0;

	private int currentPos;

	void Start()
	{
       // PlayerPrefs.DeleteAll();
		currentPos = 0;
	}

	public void FlyCamera(Transform Pos)
	{
		LeanTween.move (camera, Pos.position, flyingSpeed);
		LeanTween.rotate (camera, Pos.eulerAngles, flyingSpeed);
	}

	public void FlyCameraRight()
	{
		currentPos++;
//		Debug.Log (currentPos);
		if (currentPos > CameraPositions.Length - 1) 
		{
			currentPos = 0;
		}

//		mainPanel.SetActive (false);
//		envPanel.SetActive (true);

		LeanTween.move (camera, CameraPositions [currentPos], flyingSpeed);
		LeanTween.rotate (camera, CameraPositions [currentPos].eulerAngles, flyingSpeed);
	}

	public void FlyCameraLeft()
	{
		currentPos--;

		if (currentPos < 0) 
		{
			currentPos = CameraPositions.Length - 1;
		}

		LeanTween.move (camera, CameraPositions [currentPos], flyingSpeed);
		LeanTween.rotate (camera, CameraPositions [currentPos].eulerAngles, flyingSpeed);
	}
}
