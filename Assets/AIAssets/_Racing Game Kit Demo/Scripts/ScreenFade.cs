///this script help you to start faded animation on level load

using UnityEngine;
using System.Collections;
using RacingGameKit;
using System.Collections.Generic;

public class ScreenFade : MonoBehaviour
{

    // store a small, repeatable texture (a white square)
    public Texture2D theTexture;
    public float FadeTime = 3f;
    private float StartTime=0;
    private bool IsFadeDone = false;
    void OnLevelWasLoaded()
    {
        // Store the current time
        StartTime = Time.time;
    }

    void Update()
    {
        if (Time.time - StartTime >= FadeTime)
        {
            theTexture = null;
            IsFadeDone = true;
        }
    }

    void OnGUI()
    {
        if (!IsFadeDone)
        {
            GUI.depth = -1;
            Color color = Color.black;
            color.a = Mathf.Lerp(1.0f, 0.0f, (Time.time - StartTime));
            GUI.color = color;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), theTexture);
        }
    }


}
