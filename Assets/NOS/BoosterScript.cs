using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class BoosterScript : MonoBehaviour 
{  
	public GameObject FillNitroButton, Wind;
	public Image NitroButtonFiller;
	bool WindOn = true, IsNitroEmpty= true;

	void Start()
	{
	FillNitroButton.SetActive (false);
	PlayerManagerScript.instance.addNitro (1); 
	NitroButtonFiller.fillAmount = 0.01f;
	}

	public void NOSButtonDown()
	{
		if (!GlobalVariables.isPause) 
		{
			Nitro ();
			IsNitroEmpty = false;
			gameObject.GetComponent<Button> ().interactable = false;
			NitroButtonFiller.fillAmount = 0.0f;
		}

	}
		
	/// <summary>
	/// Turbo Button Is Pressed
	/// </summary>
	void Nitro()
	{  
		if (PlayerManagerScript.instance._vehicleHandling.turboLocked)
			PlayerManagerScript.instance._vehicleHandling.EndTurbo ();

		else {
			if (PlayerManagerScript.instance.isEmpty)
				OnEmptyNitro ();
			else {
				PlayerManagerScript.instance._vehicleHandling.StartTurbo ();
			}
		}
	}

	// If nitro is empty then open popUp for no nitro
public	void OnEmptyNitro()
	{
//		Debug.Log ("Plaese fill the Nitro to boost your speed .... (:  ");
//		FillNitroButton.SetActive (true);
		if (!GlobalVariables.isPause) 
		{
			PlayerManagerScript.instance.addNitro (1); 
			IsNitroEmpty = true;
//			Debug.Log (IsNitroEmpty+" Plaese fill the Nitro to boost your speed .... (:  ");
		}
	}
		
	public	void fillNitro()
	{
		if (!GlobalVariables.isPause) 
		{
			PlayerManagerScript.instance.addNitro (1); 

			FillNitroButton.SetActive (false);
		}
	}

	void FixedUpdate()
	{  
		if(gameObject.GetComponent<Button> ().interactable== false && IsNitroEmpty==true)
		NitroButtonFiller.fillAmount += 0.1f*Time.deltaTime;
		if (NitroButtonFiller.fillAmount >= 1f && gameObject.GetComponent<Button> ().interactable == false) 
		{
			gameObject.GetComponent<Button> ().interactable = true;
		}
	}
		
	public void windOn_Off()
	{
		if (!GlobalVariables.isPause) 
		{
			if (WindOn) 
			{
				WindOn = false;
				Wind.SetActive (false);
			}
			else 
			{
				WindOn = true;
				Wind.SetActive (true);
			}
		}
	}
}
