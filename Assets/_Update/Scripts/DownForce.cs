using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownForce : MonoBehaviour {

    // Use this for initialization
    public Rigidbody rb;
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 11;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.5f, layerMask))
        {
           Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
              Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.5f, Color.white);
             Debug.Log("Did not Hit");
            rb.AddForce(Vector3.down * 10000);
        }
    }
}