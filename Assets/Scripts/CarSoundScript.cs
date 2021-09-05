using UnityEngine;
using System.Collections;
using RacingGameKit;

public class CarSoundScript : MonoBehaviour 
{
	private vehicleHandling carHandling;
	private AudioSource audioSource;
	private float factor = 0, p0, p1;
	public float maxSpeed = 20;
	private Racer_Register racerReg;
	private bool finishCheck = true;
	private bool noSound = false;

	void Start()
	{
		racerReg = PlayerManagerScript.instance.Car.GetComponent<Racer_Register> ();
		p0 = 0; 
		p1 = 1;
		carHandling = this.GetComponent<vehicleHandling> ();
		audioSource = this.GetComponent<AudioSource> ();
		audioSource.Play ();
		audioSource.pitch = 0.3f;
	}

	void Update()
	{
		if (!noSound && !GlobalVariables.isPause) 
		{
			factor = (carHandling.vehicleSpeed - p0) / maxSpeed * (p1 - p0) + p0;
			if(factor > 0.3f)
				audioSource.pitch = factor;
			else
				audioSource.pitch = 0.3f;
		}
		if (finishCheck && racerReg.IsRacerFinished) 
		{
			noSound = true;
			finishCheck = false;
			StopCarSound ();
		}

		if (GlobalVariables.isPause) 
		{
			audioSource.pitch = 0;
		}
	}

	void StopCarSound()
	{
		LeanTween.value (this.gameObject, delegate(float val) {
			audioSource.pitch = val;
		}, 0.5f, 0, 1.2f);
	}

}
