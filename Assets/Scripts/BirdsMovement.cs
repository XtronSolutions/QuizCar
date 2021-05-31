using UnityEngine;
using System.Collections;

public class BirdsMovement : MonoBehaviour 
{
	private Vector3 initPos;
	public Vector3 finalPos;

	public float timeToAnimate;

	void Awake()
	{
		initPos = this.transform.position;
//		finalPos = new Vector3 (1, 1, 1);
	}

	void Start()
	{
		timeToAnimate = Random.Range (20, 30);
		Fly ();
	}

	void Fly()
	{
		iTween.MoveTo (this.gameObject, iTween.Hash ("position", finalPos, "time", timeToAnimate, "easetype", iTween.EaseType.linear, 
			"oncomplete", "DestroyBird", "oncompletetarget", this.gameObject));
	}

	void DestroyBird()
	{
		Destroy (this.gameObject);
	}

}
