using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
	[Tooltip("The target object to follow and lookat")]
	public Transform lookTarget;
	[Tooltip("The relative offset for the camera to aim for")]
	public Vector3 offset;
	[Tooltip("How quickly the camera will move to it's target position")]
	public float smoothSpeed;
	[Tooltip("How quickly the camera will rotate to match steering input")]
	public float rotationSmoothSpeed;

	private Vector3 oldPosition;
	private Vector3 targetPosition;

	private float oldRotation = 0;
	private float targetRotation = 0;

	private float fixedTime = 0;

	// set inital target position and set oldPosition as the same for the start
	void Start () {
		targetPosition = lookTarget.position + offset;
		oldPosition = targetPosition;
		
	}

	// Interpolate between old and target positions (this method of camera movement matches the way the rigidbody interpolation works)
	//i.e. calculate target position in fixed update and interpolate towards that in update.
	void Update()
	{
		float interpolateAmount = (Time.time - fixedTime) / Time.fixedDeltaTime;
		this.transform.position = Vector3.Lerp(oldPosition, targetPosition, interpolateAmount);

		transform.LookAt(lookTarget);



#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		//if on mobile, rotate the camera to match mobile rotation 
		transform.Rotate(new Vector3(0, 0, Mathf.Lerp(oldRotation, targetRotation, interpolateAmount)));
#endif
	}

	//update new target position on physics update
	void FixedUpdate()
	{
		//store latest fixed time step for interpolating in update
		fixedTime = Time.time;

		//store old parameters
		oldPosition = targetPosition;
		oldRotation = targetRotation;

		//lerp new targets
		targetPosition = Vector3.Lerp(targetPosition, lookTarget.position + lookTarget.rotation * offset, Time.fixedDeltaTime * smoothSpeed);
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		targetRotation = Mathf.Lerp(targetRotation, Input.acceleration.x * -40f, Time.fixedDeltaTime * rotationSmoothSpeed);
#endif
	}
}
