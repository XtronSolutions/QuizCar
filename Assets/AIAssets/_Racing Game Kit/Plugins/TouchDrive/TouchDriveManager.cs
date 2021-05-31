using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace RacingGameKit.TouchDrive
{
    [AddComponentMenu("Racing Game Kit/Touch Drive/TouchDrive Manager")]
    public class TouchDriveManager : TouchDriveManagerBase
    {
        internal List<TouchItemData> TouchBank;
        private int BankSize = 5;

        public TouchItemBase Throttle;
        /// <summary>
        /// Brake button input
        /// </summary>
        public TouchItemBase Brake;
        /// <summary>
        /// Steer Left Button input
        /// </summary>
        public TouchItemBase SteerLeft;
        /// <summary>
        /// Steer Right Button Input
        /// </summary>
        public TouchItemBase SteerRight;
        /// <summary>
        /// Shifter Up button input
        /// </summary>
        public TouchItemBase ShiftUp;
        /// <summary>
        /// Shifter Down Button input
        /// </summary>
        public TouchItemBase ShiftDown;
        /// <summary>
        /// TouchWheel Control
        /// </summary>
        public TouchItemBase Wheel;
        /// <summary>
        /// Camera Change Button Input
        /// </summary>
        public TouchItemBase CameraButton;
        /// <summary>
        /// Reset Vehicle/Scene Button Input
        /// </summary>
        public TouchItemBase ResetButton;
        /// <summary>
        /// Pause Button Input
        /// </summary>
        public TouchItemBase PauseButton;
        /// <summary>
        /// Mirror/Back View Button Input
        /// </summary>
        public TouchItemBase MirrorButton;
        /// <summary>
        /// Miscellaneous Buttin Input. This is additional inputs that designer may want to add
        /// </summary>
        public TouchItemBase Misc1Button;
        /// <summary>
        /// Miscellaneous Buttin Input. This is additional inputs that designer may want to add
        /// </summary>
        public TouchItemBase Misc2Button;


        public bool EnableMouseEmulation = false;

        void Awake()
        {
            TouchBank = new List<TouchItemData>();

            for (int i = 0; i < BankSize; i++)
            {
                TouchBank.Add(new TouchItemData());
            }

            Input.multiTouchEnabled = true;

            //Add Items to TouchItems List
            TouchItems.Clear();
            TouchItems.Add(Wheel);
            TouchItems.Add(Throttle);
            TouchItems.Add(Brake);
            TouchItems.Add(SteerLeft);
            TouchItems.Add(SteerRight);
            TouchItems.Add(ShiftUp);
            TouchItems.Add(ShiftDown);
            TouchItems.Add(CameraButton);
            TouchItems.Add(MirrorButton);
            TouchItems.Add(ResetButton);
            TouchItems.Add(PauseButton);
            TouchItems.Add(Misc1Button);
            TouchItems.Add(Misc2Button);
        }

        
        TouchItemData GetTouchItem(int FID)
        {
            TouchItemData rTouch = null;
            foreach (TouchItemData iTouch in TouchBank)
            {
                if (iTouch.ID == FID)
                {
                    rTouch = iTouch;
                    break;
                }
            }
            return rTouch;
        }

        void Update()
        {
            foreach (Touch inputTouchItem in Input.touches)
            {
                TouchItemData touchItem = GetTouchItem(inputTouchItem.fingerId);
                if (touchItem == null)
                {
                    TouchItemData newTouchItem = GetTouchItem(-1);
                    if (newTouchItem != null) newTouchItem.Set(inputTouchItem);
                }
                else
                {
                    touchItem.Checked = true;
                    touchItem.PositionDelta = inputTouchItem.position - touchItem.Position;
                    touchItem.Position = inputTouchItem.position;
                    touchItem.Phase = inputTouchItem.phase;
                }
            }

            if (EnableMouseEmulation)
            {
                if (Input.GetMouseButton(0))
                {
                    TouchBank[BankSize - 1].Phase = TouchPhase.Stationary;
                    TouchBank[BankSize - 1].PositionDelta.x = Input.mousePosition.x - TouchBank[BankSize - 1].Position.x;
                    TouchBank[BankSize - 1].PositionDelta.y = Input.mousePosition.y - TouchBank[BankSize - 1].Position.y;
                    TouchBank[BankSize - 1].Position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    TouchBank[BankSize - 1].Checked = true;
                }
            }

            foreach (TouchItemData touchItem in TouchBank)
            {
                if (!touchItem.Checked)
                {
                    touchItem.ResetState();
                }
                else
                {
                    touchItem.Checked = false;
                }
            }
        }
    }

    #region NestedClasses

    [System.Serializable]
    public class TouchItemData
    {
        public int ID;

        public TouchPhase Phase;

        public Vector2 Position;

        public Vector2 PositionDelta;

        public bool Checked;

        public TouchItemData()
        {
            ID = -1;
            Checked = false;
            Position = new Vector2();
            PositionDelta = new Vector2();
            Phase = TouchPhase.Canceled;
        }

        public void ResetState()
        {
            ID = -1;
            Checked = false;
            Position = Vector2.zero;
            PositionDelta = Vector2.zero;
            Phase = TouchPhase.Ended;
        }

        public void Set(Touch touch)
        {
            Checked = true;
            Position = touch.position;
            PositionDelta = touch.deltaPosition;
            Phase = touch.phase;
            ID = touch.fingerId;
        }

    }


#endregion

}
