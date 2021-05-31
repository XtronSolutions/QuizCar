///This scripts help to create a exhaust pop animation on vehicles

using UnityEngine;
using System.Collections;

public class Pop : MonoBehaviour {

    public bool StartAnimation = false;
    public float AnimationSpeed = 1f;
    public bool ToLeft = false;
    public float offset = 0;

    public  float intTimer = 1;

    void Update()
    {
        if (StartAnimation)
        {
            intTimer -= Time.deltaTime;
            if (intTimer <= 0)
            {
                intTimer = AnimationSpeed;
                offset += 0.125f;
                GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, GetComponent<Renderer>().material.mainTextureOffset.y);
            }
        }

    }
}
