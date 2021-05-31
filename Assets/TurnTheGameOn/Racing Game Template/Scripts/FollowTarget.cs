using System;
using UnityEngine;

public class FollowTarget : MonoBehaviour {

	public bool autoTargetPlayer;
    public Transform target;
    public Vector3 offset = new Vector3(0f, 7.5f, 0f);

	void Start(){
		transform.parent = null;
		if (autoTargetPlayer) {
			target = GameObject.Find ("Player").transform;
		}
	}

    private void LateUpdate(){
		if (target) {
			transform.position = target.position + offset;
			transform.LookAt (target);
		}
    }
}