using UnityEngine;
using System.Collections;

public class ManualWrongWayUI : MonoBehaviour 
{
	GameObject WrongwaySprite;

	public void ShowWrongWayUI()
	{
		WrongwaySprite.SetActive (true);
	}

	public void HideWrongwayUI()
	{
		WrongwaySprite.SetActive (false);
	}
}
