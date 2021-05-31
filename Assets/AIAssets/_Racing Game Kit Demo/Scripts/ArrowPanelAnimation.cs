using UnityEngine;
using System.Collections;
using RacingGameKit;
using System.Collections.Generic;

public class ArrowPanelAnimation : MonoBehaviour
{

    public bool StartAnimation = false;
    public float AnimationSpeed=1f;
    public bool ToLeft = false;
    public float offset = 0;
    void FixedUpdate()
    {
        if (StartAnimation)
        {
            offset = Time.time * AnimationSpeed;
            if (ToLeft) offset *= -1;
            GetComponent<Renderer>().material.mainTextureOffset = new Vector2(offset, GetComponent<Renderer>().material.mainTextureOffset.y);
            if (offset >= 10 || offset <= -10) offset = 0;
        }

    }
}
