//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Touch Item Base
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// ===== UNCOMMENT THE LINE BELOW IF YOU WANT TO USE TOUCHDRIVEPRO WITH RGK!
using RacingGameKit.TouchDrive;


[ExecuteInEditMode]
[Serializable]
[AddComponentMenu("")]
public class TouchItemBase : MonoBehaviour, iRGKTouchItem //<<======== for RGK Integration replace this with iRGKTouchItem
{
    public enum ButtonTouchState
    {
        Pressed = 1,
        Released = 2,
        Disabled = 3,
    }

    public bool _IsPressed = false;
    public bool _IsToggled = false;
    public float _CurrentAngle;
    public float _CurrentFloat;
    public int _CurrentInt;
    public bool EnableAlphaOnPress = true;
    public float AlphaValue = 0.25f;

    internal void ChangeAlpha(Image PressedButton, bool IsPressed)
    {
        Color buttonBG = PressedButton.color;

        if (!EnableAlphaOnPress)
        {
            buttonBG.a = 1f;
        }
        else
        {
            if (IsPressed) { buttonBG.a = 1f; } else { buttonBG.a = AlphaValue; }
        }

        PressedButton.color = buttonBG;
    }

    #region properties

    public bool IsPressed
    {
        get
        {
            return _IsPressed;
        }
        set
        {
            _IsPressed = value;
        }
    }

    public bool IsToggled
    {
        get
        {
            return _IsToggled;
        }
        set
        {
            _IsToggled = value;
        }
    }

    public float CurrentAngle
    {
        get
        {
            return _CurrentAngle;
        }
        set
        {
            _CurrentAngle = value;
        }
    }

    public float CurrentFloat
    {
        get
        {
            return _CurrentFloat;
        }
        set
        {
            _CurrentFloat = value;
        }
    }

    public int CurrentInt
    {
        get
        {
            return _CurrentInt;
        }
        set
        {
            _CurrentInt = value;
        }
    }

    #endregion

}


public interface iTouchItem
{
    bool IsPressed { get; set; }
    bool IsToggled { get; set; }
    float CurrentAngle { get; set; }
    float CurrentFloat { get; set; }
    int CurrentInt { get; set; }
}