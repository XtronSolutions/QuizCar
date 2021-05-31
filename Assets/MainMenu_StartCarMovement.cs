using UnityEngine;
using System.Collections;

public class MainMenu_StartCarMovement : MonoBehaviour 
{
	public Vector3 carInitPos;
	public Quaternion carInitRot;
	public float startDelay;

	public GameObject car;

	void Start()
	{
		StartCoroutine (StartAfterDelay ());
	}

	IEnumerator StartAfterDelay()
	{
		yield return new WaitForSeconds (startDelay);

		GameObject cloneCar = (GameObject)Instantiate (car, carInitPos, carInitRot);
	}
}
