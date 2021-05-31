//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Touch Button
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RacingGameKit.TouchDrive
{
    [ExecuteInEditMode]
    [Serializable]
    [AddComponentMenu("Racing Game Kit/Touch Drive/Touch Button")]

    public class TouchButton : TouchItemBase
    {
   
        private TouchDriveManager touchDriveManager;
        private Image ButtonTexture;

        public bool OneShot = false;
        private bool canTouchAgain = true;
        [HideInInspector]
        public ButtonTouchState ButtonState = ButtonTouchState.Released;
        public TouchItemData myTouch;

        void Start()
        {
            touchDriveManager = transform.parent.GetComponent(typeof(TouchDriveManager)) as TouchDriveManager;
            ButtonTexture = transform.GetComponent(typeof(Image)) as Image;
        }
        void Update()
        {
            if (touchDriveManager == null) return;
            if (ButtonTexture == null) return;

            if (touchDriveManager.TouchBank != null)
            {
                foreach (TouchItemData touch in touchDriveManager.TouchBank)
                {
                    //if (ButtonTexture.HitTest(touch.Position))
                    //{
                    //    myTouch = touch;
                    //    break;
                    //}
                }
            }

            if (OneShot && !canTouchAgain)
            {
                ButtonState = ButtonTouchState.Released;
                IsPressed = false;
            }

            if (myTouch != null)
            {
                if (myTouch.Phase == TouchPhase.Stationary  && canTouchAgain)//&& ButtonTexture.HitTest(myTouch.Position)
                {
                    ButtonState = ButtonTouchState.Pressed;
                    IsPressed = true;
                   // ChangeAlpha(ButtonTexture, true);

                    if (OneShot)
                    {
                        canTouchAgain = false;
                    }
                }
                else if (myTouch.Phase == TouchPhase.Ended)
                {
                    ButtonState = ButtonTouchState.Released;
                    IsPressed = false;
                   // ChangeAlpha(ButtonTexture, false);
                    myTouch = null;
                }
            }
            else
            { canTouchAgain = true; }

        }

         
    }
}