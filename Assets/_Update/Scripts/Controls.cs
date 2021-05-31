using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {


    public GameObject[] controls;

    public static float steerVal = 0;
    public static float acceleration;
    bool isLeftPress, isRightPress;
    float steerSpeed = 0.1f;

    public static bool IsHandBrake=false;
    public static bool StartEngine=false;

    // Use this for initialization
    void OnEnable () {

        foreach (GameObject g in controls)
            g.SetActive(false);

        controls[GameData.controlsType].SetActive(true);
        steerVal = 0;
        acceleration = 1;
	}
	
	// Update is called once per frame
	void Update () {

        if (GameData.controlsType == 0) //is Tilt
        {
           steerVal=  Mathf.Clamp((Input.acceleration.x) * GameData.steeringSensitivity, -1, 1);
        }
        else
        {
            if (isLeftPress)
            {
                steerVal = Mathf.Clamp(steerVal - steerSpeed, -1, 1);
            }
            else if (isRightPress)
            {
                steerVal = Mathf.Clamp(steerVal + steerSpeed, -1, 1);
            }
            else
            {
                steerVal = 0;
            }
        }
        KeyboardInput();
	}

    public void OnBrake(bool isPress)
    {
        if (isPress)
            acceleration = -1;
        else
            acceleration = 1;
    }

    public void HandBrake()
    {
        IsHandBrake = true;
    }

    public void OnLeftArrow(bool isPress)
    {
        steerVal = 0;
        isLeftPress = isPress;
    }
    public void OnRightArrow(bool isPress)
    {
        steerVal = 0;
        isRightPress = isPress;
    }


    void KeyboardInput()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) OnLeftArrow(true);
            if (Input.GetKeyUp(KeyCode.LeftArrow)) OnLeftArrow(false);
            if (Input.GetKeyDown(KeyCode.RightArrow)) OnRightArrow(true);
            if (Input.GetKeyUp(KeyCode.RightArrow)) OnRightArrow(false);
            if (Input.GetKeyDown(KeyCode.DownArrow)) OnBrake(true);
            if (Input.GetKeyUp(KeyCode.DownArrow)) OnBrake(false);
        }
    }
   
}