//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Button Control
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

    [ExecuteInEditMode]
    [Serializable]
    [AddComponentMenu("Racing Game Kit/Touch Drive Pro/Touch Button")]
public class TouchButton : TouchItemBase
    {
        private TouchDriveManager touchDriveManager;
        private Image ButtonTexture;
        public bool OneShot = false;
        private bool canTouchAgain = true;
        [HideInInspector]
        public ButtonTouchState ButtonState = ButtonTouchState.Released;
        public TouchItemData TouchData;

        void Start()
        {
            touchDriveManager = transform.parent.GetComponent(typeof(TouchDriveManager)) as TouchDriveManager;
            ButtonTexture = transform.GetComponent(typeof(Image)) as Image;
        }

        void Update()
        {
            if (touchDriveManager == null) return;
            if (ButtonTexture == null) return;
            if (AlphaValue < 0) AlphaValue = 0;

            if (touchDriveManager.TouchBank != null)
            {
                foreach (TouchItemData touch in touchDriveManager.TouchBank)
                {
                    //if (ButtonTexture.HitTest(touch.Position))
                    //{
                    //    TouchData = touch;
                    //    break;
                    //}
                }
            }

            if (OneShot && !canTouchAgain)
            {
                ButtonState = ButtonTouchState.Released;
                IsPressed = false;
            }

            if (TouchData != null)
            {
                //if (TouchData.Phase == TouchPhase.Stationary && ButtonTexture.HitTest(TouchData.Position) && canTouchAgain)
                //{
                //    ButtonState = ButtonTouchState.Pressed;
                //    IsPressed = true;
                //    ChangeAlpha(ButtonTexture, true);

                //    if (OneShot)
                //    {
                //        canTouchAgain = false;
                //    }
                //}
                if (TouchData.Phase == TouchPhase.Ended)
                {
                    ButtonState = ButtonTouchState.Released;
                    IsPressed = false;
                    ChangeAlpha(ButtonTexture, false);
                    TouchData = null;
                }
            }
            else
            { canTouchAgain = true; }
        }

         
    }
