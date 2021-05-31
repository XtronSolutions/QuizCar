using UnityEngine;
using System.Collections;

public class Dolphins : MonoBehaviour {

   public  Transform StartPosition;
    public Rigidbody Rb;
    float timer = 0;
    float JumpTime = 4f;
   public  bool dolphinCanJump = false;
    bool DolphinJumped = false;
	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (dolphinCanJump)
        {
            if(!DolphinJumped)
                Rb.AddForce(new Vector3(0f, 20f, 0f), ForceMode.Impulse);
            DolphinJumped = true;
            Rb.isKinematic = false;
            Rb.useGravity = true;
            Rb.AddRelativeForce(Vector3.forward * Time.deltaTime * 500f);
            timer += Time.deltaTime;
            if (timer > JumpTime)
            {
                timer = 0;
                dolphinCanJump = false;
                DolphinJumped = false;
                
                Rb.useGravity = false;
                Rb.isKinematic = true;
                transform.localPosition = StartPosition.localPosition;
            }
        }
        
        
        

    }
}