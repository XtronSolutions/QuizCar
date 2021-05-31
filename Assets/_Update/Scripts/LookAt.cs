using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
public class LookAt : MonoBehaviour {

    public Race_Camera race_Camera;
    public static Vector3 targetRotation;
    float val;
    float speed = 0.75f;
    float maxVal = 15;
	// Use this for initialization
	void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		if(race_Camera!=null)
        {
            if(race_Camera.target!=null)
            {
                this.transform.position = race_Camera.transform.position;
                this.transform.LookAt(race_Camera.target);


                if(Controls.steerVal>0.4f)
                {
                    val = Mathf.Clamp(val + speed, -maxVal, maxVal);
                }
                else if(Controls.steerVal<-0.4f)
                {
                    val = Mathf.Clamp(val - speed, -maxVal, maxVal);
                }
                else
                {
                    if(val>0)
                    {
                        val = Mathf.Clamp(val - speed, -maxVal, maxVal);
                    }
                    else if(val<0)
                    {
                        val = Mathf.Clamp(val + speed, -maxVal, maxVal);
                    }

                }

                targetRotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, val) ;
            }
        }
	}
}