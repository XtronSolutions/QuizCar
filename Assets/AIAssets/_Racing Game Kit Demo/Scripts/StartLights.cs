/// This script help you manage a startlights for track

using UnityEngine;
using System.Collections;
using RacingGameKit;
public class StartLights : MonoBehaviour
{


    private Race_Manager RaceManager;
    public Light Light3;
    public Light Light2;
    public Light Light1;
    public Light Light0;

    void Start()
    {

        GameObject RaceManagerObject = GameObject.Find("_RaceManager");
        RaceManager = RaceManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;

        Light0.enabled = false;
        Light1.enabled = false;
        Light2.enabled = false;
        Light3.enabled = false;

    }


    void Update()
    {
        if (this.enabled && RaceManager!=null)
        {
            if (RaceManager.CurrentCount >= 0 && RaceManager.CurrentCount < 4)
            {
                switch (RaceManager.CurrentCount)
                {
                    case 3:
                        Light3.enabled = true;
                        break;
                    case 2:
                        Light2.enabled = true;
                        break;
                    case 1:
                        Light1.enabled = true;
                        break;
                    case 0:
                        Light1.enabled = false;
                        Light2.enabled = false;
                        Light3.enabled = false;
                        Light0.enabled = true;
                        break;
                }
            }
        }
    }
}