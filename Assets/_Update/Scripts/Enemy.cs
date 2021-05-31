using UnityEngine;
using System.Collections;


[RequireComponent(typeof(CharacterController))]

public class Enemy : MonoBehaviour
{
	
	public float GravityMult = 1;
	public float Slip = 10;
	public float run = 1;
	private float fallvelocity = 0;
	private Vector3 moveDirection;
	
	public Transform Myself;
	public float Speed = 3;
	public AudioClip[] footstepSound;
	Vector3 targetPosition;
	public int timethink = 0;
	public string WalkPose = "Walk";
	public string IdlePose = "Idle";

	public int state = 0;
	private CharacterController characterController;

	bool isDead = false;

    public bool canWalk = true;
    public Transform t;
	void Start ()
	{
		if(Myself == null)
			Myself = this.gameObject.transform;
		
		characterController = this.GetComponent<CharacterController> ();
		Myself.GetComponent<Animation>().PlayQueued (IdlePose);

		targetPosition = this.transform.position;
      //  InvokeRepeating("Update2", 4, 0.1f);
	}

	void Update ()
	{
        if (Vector3.Distance(this.transform.position, PlayerManagerScript.instance.Car.transform.position) > 100)
            return;
		
		if (timethink <= 0) {
			targetPosition = new Vector3 (targetPosition.x+Random.Range (Random.Range(-30,-20), Random.Range(30,20)), 0, targetPosition.z+Random.Range (Random.Range(-30,-20), Random.Range(30,20)));
			timethink = Random.Range (100,300);
			state = Random.Range (0, 4);
        } else {
			timethink -= 1;
		}
		
   		isGrounded = GroundChecking ();
		
		targetPosition.y = transform.position.y;

		if (Vector3.Distance (targetPosition, this.transform.position) > 2) {
			Quaternion rotationTarget = Quaternion.LookRotation ((targetPosition - this.transform.position).normalized);

			transform.rotation = Quaternion.Lerp (this.transform.rotation, rotationTarget, Time.deltaTime * 2f);
		}
        else
        {
            timethink = -1;
        }

	
		if (state >1 && canWalk) {
			Myself.GetComponent<Animation> ().CrossFade (WalkPose, 0.3f);
			Vector3 direction = (targetPosition - transform.position).normalized;
			moveDirection = Vector3.Lerp (moveDirection, direction, Time.deltaTime * Slip);

          
        }  else {
				Myself.GetComponent<Animation> ().CrossFade (IdlePose, 0.3f);
				moveDirection = Vector3.zero;
		}
		

        if(!canWalk) moveDirection = Vector3.zero;


        moveDirection.y = fallvelocity;
			characterController.Move (moveDirection * Speed * Time.deltaTime);
	
			if (!isGrounded) {
				fallvelocity -= 90 * GravityMult * Time.deltaTime;
			}
		
	}
	
	public float DistanceToGround = 0.1f;
	bool isGrounded = false;

    public void OnTriggerEnter(Collider col)
    {

        if(col.gameObject.tag != "Track")
        {
            if (t != null)
                targetPosition = t.position;
          //  transform.rotation = Quaternion.Lerp(this.transform.rotation, rotationTarget, Time.deltaTime * 1);
            state = 3;
        }
    }

    public bool GroundChecking ()
	{
		if (GetComponent<Collider>()) {
			RaycastHit hit;
			if (characterController.isGrounded)
				return true;
			if (Physics.Raycast (GetComponent<Collider>().bounds.center, -Vector3.up, out hit, DistanceToGround + 0.1f)) {
				return true;
			}
		}
		return false;
		
	}

}
