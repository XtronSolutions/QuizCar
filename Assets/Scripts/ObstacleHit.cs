using UnityEngine;
using System.Collections;

public class ObstacleHit : MonoBehaviour 
{
	public GameObject obstacleMesh;
	public Transform camTransform;
	public GameObject breakEffect;
	[Space]
	[Header("Parameters")]
	public float shakeDuration;
	public float shakeAmount;
	public float decreaseFactor;

	private Vector3 orignalPos;
	private float duration;
	private bool isHit = false;

	void OnTriggerEnter(Collider hit)
	{
		if (!isHit) 
		{
			if (hit.tag.Equals ("Player")) 
			{
				isHit = true;
				duration = shakeDuration;
				orignalPos = camTransform.localPosition;
//				camTransform.GetComponent<CameraShake> ().ShakeCamera ();
				StartCoroutine (ShakeCamera ());
				//			GameObject.Instantiate (breakEffect, this.transform.position, this.transform.rotation);
				breakEffect.SetActive (true);
				obstacleMesh.SetActive (false);
			}
		}
	}

	IEnumerator ShakeCamera()
	{
		while (duration > 0) 
		{
			camTransform.localPosition = camTransform.localPosition + new Vector3(Random.insideUnitSphere.x, 0, Random.insideUnitSphere.z) * shakeAmount;
			duration -= Time.deltaTime * decreaseFactor;
			yield return new WaitForSeconds(0.001f);
		}
	}
}
