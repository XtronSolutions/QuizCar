using UnityEngine;
using System.Collections;

public class UnfreezRigidBody : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == 15)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}