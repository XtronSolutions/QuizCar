using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_WheelCollider : MonoBehaviour {

	// Use this for initialization
	private vehicleHandling carController;
	private RCC_Skidmarks skidmarks;	
	private float wheelSlipAmountForward = 0f;		// Forward slip.
	private float wheelSlipAmountSideways = 0f;	// Sideways slip.
	internal float totalSlip = 0f;
	private float startSlipValue = .25f;		// Draw skidmarks when forward or sideways slip is bigger than this value.
	private Rigidbody rigid;
	private int lastSkidmark = -1;
	private WheelCollider _wheelCollider;

	public ParticleSystem dustEffect;

	void Start () {

		_wheelCollider = GetComponent<WheelCollider>();
		carController = GetComponentInParent<vehicleHandling>();
		rigid = carController.GetComponent<Rigidbody> ();

		if (GameObject.FindObjectOfType (typeof(RCC_Skidmarks)))
			skidmarks = GameObject.FindObjectOfType (typeof(RCC_Skidmarks)) as RCC_Skidmarks;
		else
			skidmarks = Instantiate (Resources.Load ("SkidMarks/SkidmarksManager"), Vector3.zero, Quaternion.identity) as RCC_Skidmarks;


        InvokeRepeating("FixedUpdate2", 0.1f, 0.2f);
    }

//	public WheelCollider wheelCollider{
//		get{
//			if(_wheelCollider == null)
//				_wheelCollider = GetComponent<WheelCollider>();
//			return _wheelCollider;
//		}
//	}

	
	// Update is called once per frame
	void Update () {

		if (carController.isSleeping)
			return;
		
	}

	void FixedUpdate2(){

		if (carController.isSleeping)
			return;

		if (!carController.isSleeping){

			SkidMarks ();
		}
	}

	void SkidMarks(){

		// First, we are getting groundhit data.
		WheelHit GroundHit;
		_wheelCollider.GetGroundHit(out GroundHit);

		if (dustEffect != null) {
			if (_wheelCollider.rpm > 500 && !dustEffect.isPlaying && _wheelCollider.isGrounded && !Constants.inWater) {
				dustEffect.Play ();
			} else if (_wheelCollider.rpm <= 500 && dustEffect.isPlaying) {
				dustEffect.Stop ();
			} else if (!_wheelCollider.isGrounded && dustEffect.isPlaying) {
				dustEffect.Stop ();
			}else if (Constants.inWater && dustEffect.isPlaying) {
				dustEffect.Stop ();
			}
		}

		// Forward, sideways, and total slips.
		wheelSlipAmountForward = Mathf.Abs(GroundHit.forwardSlip);
		wheelSlipAmountSideways = Mathf.Abs(GroundHit.sidewaysSlip);

		totalSlip = Mathf.Lerp(totalSlip, (wheelSlipAmountSideways + wheelSlipAmountForward), Time.fixedDeltaTime * 3f) / 1f;

		// If scene has skidmarks manager...
		if(skidmarks){

			// If slips are bigger than target value...
			if (wheelSlipAmountSideways + wheelSlipAmountForward > startSlipValue){

				Vector3 skidPoint = GroundHit.point + 2f * (rigid.velocity) * Time.deltaTime;

				if(rigid.velocity.magnitude > 1f)
					lastSkidmark = skidmarks.AddSkidMark(skidPoint, GroundHit.normal, totalSlip, lastSkidmark);
				else
					lastSkidmark = -1;

			}else{

				lastSkidmark = -1;

			}

		}

	}
}
