using UnityEngine;
using System.Collections;

public class MainMenu_BirdController : MonoBehaviour 
{
	public GameObject birdLR;
	public GameObject birdRL;

	public Transform leftPos;
	public Transform rightPos;
	public float timeToReach;

	private float timer = -1;

	void Start()
	{
		Resources.UnloadUnusedAssets ();
	}

	void Update()
	{
		timer -= Time.deltaTime;

		if (timer < 0) 
		{
			Spawn ();
		}
	}

	void Spawn()
	{
		GameObject birdOne = (GameObject)Instantiate (birdLR, leftPos.position, birdLR.transform.rotation);
		GameObject birdTwo = (GameObject)Instantiate (birdRL, rightPos.position, Quaternion.identity);

		birdOne.GetComponent<BirdsMovement> ().finalPos = rightPos.position;
		birdTwo.GetComponent<BirdsMovement> ().finalPos = leftPos.position;

		birdOne.GetComponent<BirdsMovement> ().timeToAnimate = timeToReach;
		birdTwo.GetComponent<BirdsMovement> ().timeToAnimate = timeToReach;

		timer = Random.Range (10, 20);
	}


}
