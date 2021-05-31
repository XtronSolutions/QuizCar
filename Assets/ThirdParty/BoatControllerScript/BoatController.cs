using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour 
{
	public Transform[] navPoints;
	public float speed;

	private int pointToMove;
	private float dist;

	void Start()
	{
		InstantiateMovement ();
		this.transform.LookAt (navPoints [pointToMove]);	
	}

	void FixedUpdate()
	{
		MoveForward ();
	}

	private void Turn(int index)
	{
//		this.transform.LookAt (navPoints [index]);
		Vector3 direction = navPoints [index].transform.position - this.transform.position;
		Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector (transform.forward);
		Quaternion toRotation = Quaternion.FromToRotation (localForward, direction);

		StartCoroutine (TurnTowards (toRotation));

	}

	IEnumerator TurnTowards(Quaternion toRotation)
	{
		float tempSpeed = speed;
//		Debug.Log ("THIS -> " + this.transform.localRotation.y + " To -> " + toRotation.y);
		if (Mathf.Abs (this.transform.localRotation.y) < Mathf.Abs (toRotation.y)) 
		{
			while (Mathf.Abs (this.transform.localRotation.y) < Mathf.Abs (toRotation.y)) {
//				Debug.Log ("**********");
				speed = 0.12f;
				this.transform.Rotate (new Vector3 (0, 0.2f, 0), Space.Self);
				yield return new WaitForSeconds (0.007f);
			}
		} 
		else 
		{
			while (Mathf.Abs (this.transform.localRotation.y) > Mathf.Abs (toRotation.y)) {
//				Debug.Log ("**********");
				speed = 0.12f;
				this.transform.Rotate (new Vector3 (0, 0.2f, 0), Space.Self);
				yield return new WaitForSeconds (0.007f);
			}
		}

		this.transform.LookAt (navPoints[pointToMove]);
		speed = tempSpeed;
	}

	private void MoveForward()
	{
		dist = Vector3.Distance (this.transform.position, navPoints [pointToMove].position);
		if (dist < 50) 
		{
			pointToMove += 1;
			if (pointToMove == navPoints.Length) {
				pointToMove = 0;
			}
			Turn (pointToMove);
		}
		this.gameObject.transform.Translate (0, 0, speed, Space.Self);
	}


	private void InstantiateMovement()
	{
		dist = Vector3.Distance(this.transform.position, navPoints [0].transform.position);
//		Debug.Log ("0---> " + dist);
		pointToMove = 0;

		for (int i = 1; i < navPoints.Length; i++) 
		{
			float newDist = Vector3.Distance(this.transform.position, navPoints [i].transform.position);
//			Debug.Log (i + "---> " + newDist);

			if (newDist <= dist) 
			{
				dist = newDist;
				pointToMove = i;
			}
		}
		if (dist < 20) 
		{
			if (pointToMove != navPoints.Length - 1) 
			{
				pointToMove++;
			} 
			else 
			{
				pointToMove = 0;	
			}
		} 
	}
}
