///this script help you to create random animations on crowd characters

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class RandomCrowdAnimation : MonoBehaviour
{

    public int RandLow = 5;
    public int RandHigh = 10;
    private float intTimer;
    private List<string> AnimationNames;
    private bool StartRandomizing = false;
    string NextAnimName = "idle";

    void Start()
    {
        AnimationNames = new List<string>();
        if (base.GetComponent<Animation>().GetClipCount() > 0)
        {
            foreach (AnimationState AClip in GetComponent<Animation>())
            {
                AnimationNames.Add(AClip.name);
            }
            StartRandomizing = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (StartRandomizing)
        {
            intTimer -= Time.deltaTime;
            if (intTimer <= 0)
            {
                intTimer = Random.Range(RandLow, RandHigh);
                NextAnimName = AnimationNames[Random.Range(0, GetComponent<Animation>().GetClipCount())];
                base.GetComponent<Animation>().CrossFade(NextAnimName, 0.5f);
            }
        }

        

    }




}
