using System;
using UnityEngine;
using System.Collections;


namespace RacingGameKit.TouchDrive
{
    [ExecuteInEditMode]
    [Serializable]
    [AddComponentMenu("Racing Game Kit/Touch Drive/Touch Wheel")]
    public class TouchWheel : TouchItemBase
    {
        private TouchDriveManager touchDriveManager;

        public float MaxAngle = 105f;
        public bool UseSensitivityCurve = false;
        public AnimationCurve SensitivityCurve;
  
        public float CenterSpeed = 5f;
        public Texture2D WheelTexture;
        public Vector2 WheelSize = new Vector2((float)200, (float)200);
        public Color TextureColor = Color.white;

        public Texture2D DbgTouchMask;
        private Color TextureColorWithAlpha = Color.white;
        private string TurnRotation = string.Empty;
        private Vector2 rotatePivot;
        private Vector2 posWheel = new Vector2((float)0, (float)0);
        private Rect rectWheel;
        private Rect rectPivot;
        private Vector2 WheelPivot = new Vector2((float)200, (float)200);

        private float AlignLeft = 0;
        private float AlignTop = 0;
        private Vector2 mtPos;
        private Vector2 ntPos;
        private float sH;
        private float sW;
        [HideInInspector]

        public TouchItemData TouchData;

        void Start()
        {
            sH = Screen.height;
            sW = Screen.width;

            touchDriveManager = transform.parent.GetComponent(typeof(TouchDriveManager)) as TouchDriveManager;
        }

        void OnGUI()
        {
            this.TurnRotation = null;


            this.UpdatePos();


            if (DbgTouchMask != null) GUI.DrawTexture(rectPivot, this.DbgTouchMask);
            Matrix4x4 matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(this.CurrentAngle, this.rotatePivot);
            GUI.color = this.TextureColorWithAlpha;
            GUI.DrawTexture(this.rectWheel, this.WheelTexture);
            GUI.matrix = matrix;
        }
        /// <summary>
        /// Update 
        /// </summary>
        void Update()
        {

            sH = Screen.height;
            sW = Screen.width;


            this.AlignLeft = Mathf.Clamp(transform.position.x, 0, 1) * (sW - WheelSize.x / 2);
            this.AlignTop = (sH - WheelSize.y) - (Mathf.Clamp(transform.position.y, -1, 1) * (sH - WheelSize.y / 2));

            if (touchDriveManager == null) return;

            if (CenterSpeed <= 1) CenterSpeed = 1;
            if (AlphaValue < 0) AlphaValue = 0;

            if (string.IsNullOrEmpty(TurnRotation))
            {
                this.CurrentAngle = Mathf.Lerp(CurrentAngle, 0, CenterSpeed * Time.deltaTime);
            }


            if (Mathf.Abs(CurrentAngle) < 0.05) CurrentAngle = 0;

            float oX = WheelSize.x;
            float oY = WheelSize.y;
            rectPivot = new Rect(AlignLeft, rectWheel.y, oX, oY);

            WheelPivot = new Vector2(rectPivot.width / 2, rectPivot.height / 2);


            if (touchDriveManager.TouchBank != null)
            {
                foreach (TouchItemData touch in touchDriveManager.TouchBank)
                {
                    ntPos = new Vector2(touch.Position.x, touch.Position.y);

                    if (((ntPos.x > 0) && (ntPos.x < oX + AlignLeft)) && ((ntPos.y > 0) && (ntPos.y < oY + (sH - AlignTop))))
                    {
                        TouchData = touch;
                        break;
                    }
                }
            }

            if (TouchData != null)
            {
                mtPos = new Vector2(TouchData.Position.x - AlignLeft, sH - TouchData.Position.y - AlignTop);

                //if (((myTouch.Position.x > 0) && (myTouch.Position.x < oX)) && ((Screen.height - myTouch.Position.y > 0) && (Screen.height - myTouch.Position.y < oY)))
                if (((mtPos.x > 0) && (mtPos.x < oX)) && ((mtPos.y > 0) && (mtPos.y < oY)))
                {

                    mtPos = new Vector2(mtPos.x, WheelSize.y - mtPos.y);

                    if (mtPos.x <= WheelPivot.x)
                    {
                        this.CurrentAngle = -Vector2.Angle(new Vector2((float)0, (float)1), mtPos - this.WheelPivot);
                    }
                    else
                    {
                        this.CurrentAngle = Vector2.Angle(new Vector2((float)0, (float)1), mtPos - this.WheelPivot);
                    }
                    ChangeAlpha(true);
                }
                else
                { ChangeAlpha(false); }
            }
            else
            { ChangeAlpha(false); }


            if (CurrentAngle >= MaxAngle) CurrentAngle = MaxAngle;
            if (CurrentAngle <= -MaxAngle) CurrentAngle = -MaxAngle;

            if (!UseSensitivityCurve)
            {
                CurrentFloat = Mathf.Clamp((1 / MaxAngle) * CurrentAngle, -1, 1);
            }
            else
            {
                CurrentFloat = SensitivityCurve.Evaluate(Mathf.Abs(CurrentAngle) / MaxAngle);
                if (CurrentAngle < 0) CurrentFloat *= -1;


            }
            if (CurrentFloat > 0) { CurrentInt = 1; }
            else if (CurrentFloat < 0) { CurrentInt = -1; }
            else { CurrentInt = 0; }

        }
        /// <summary>
        /// Update wheel position and rect for next calculation. This will not called in game anyway. Only for editor.
        /// </summary>
        void UpdatePos()
        {
            this.posWheel = new Vector2(AlignLeft, AlignTop);
            this.rectWheel = new Rect(this.posWheel.x, this.posWheel.y, this.WheelSize.x, this.WheelSize.y);
            this.rotatePivot = new Vector2(this.rectWheel.xMin + (this.rectWheel.width * 0.5f), this.rectWheel.yMin + (this.rectWheel.height * 0.5f));
        }

        /// Change wheel alpha when it pressed
        /// </summary>
        /// <param name="IsPressed"></param>
        void ChangeAlpha(bool IsPressed)
        {
            if (!EnableAlphaOnPress)
            {
                TextureColorWithAlpha = new Color(TextureColor.r, TextureColor.g, TextureColor.b, 1f);
            }
            else
            {
                if (IsPressed)
                {
                    TextureColorWithAlpha = new Color(TextureColor.r, TextureColor.g, TextureColor.b, 1f);
                }
                else
                {
                    TextureColorWithAlpha = new Color(TextureColor.r, TextureColor.g, TextureColor.b, AlphaValue);
                }
            }

        }

    }

}



