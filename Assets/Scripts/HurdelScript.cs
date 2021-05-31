using UnityEngine;
using System.Collections;

public class HurdelScript : MonoBehaviour {
    public GameObject camera;
    public bool StandAloneHurdle;
	// Use this for initialization
	void Start () {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.collider.gameObject.CompareTag("RaceCar") &&  collision.relativeVelocity.magnitude > 1)
        {
            if (!StandAloneHurdle)
            {
                transform.GetComponent<BoxCollider>().enabled = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<BoxCollider>().enabled = true;
                    transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;

                }
            }
            
            if (camera == null)
            {
                camera = GameObject.FindGameObjectWithTag("MainCamera");
            }
           if(camera.GetComponent<CameraShake>()!=null)camera.GetComponent<CameraShake>().ShakeCamera();
            GetComponent<AudioSource>().Play() ;
            Destroy(gameObject, 2f);
        }

    }
}