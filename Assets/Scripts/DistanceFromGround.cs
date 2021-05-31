using UnityEngine;

using System.Collections;

public class DistanceFromGround : MonoBehaviour {
    
    public float GroundDistanceBefore, GroundDistanceAfter;
    public float targetGroundDistance;
    GameObject ground;
	// Use this for initialization
	void Start () {
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit))
        {
            
            GroundDistanceBefore = hit.distance;
            if(hit.transform.CompareTag("Track"))
                transform.position = hit.point;
            
        }
        downRay = new Ray(transform.position, -Vector3.up);
        if (Physics.Raycast(downRay, out hit))
        {
            GroundDistanceAfter = hit.distance;
         
        }

    }
	
	// Update is called once per frame
	void Update () {
        

    }
}