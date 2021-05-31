using UnityEngine;
using System.Collections;

public class dest : MonoBehaviour {
	
	void Start () 
	{
		this.GetComponent<SphereCollider> ().enabled = false;
	}

	void Update () 
	{
		if (Input.GetKeyUp (KeyCode.A)) 
		{
			this.GetComponent<SphereCollider> ().enabled = true;
			this.transform.position += new Vector3 (this.transform.position.x, this.transform.position.y, this.transform.position.z + 0.5f);
		}
	}
}
