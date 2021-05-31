//============================================================================================
// Touch Drive Pro v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// TouchDrive Manager 
// Last Change : 10/10/2013
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Racing Game Kit/Touch Drive Pro/TouchDrive Manager")]
[ExecuteInEditMode]
public class TouchDriveManager : TouchDriveManagerBase
{
    /// <summary>
    /// Template Definitions
    /// </summary>
    public enum eControlTemplate
    {
        ThrottleTilt_Steer = 0,
        ThrottleButton_Steer = 1,
        ThrottleWheel_Steer = 2,
        BrakeTilt_Steer = 10,
        BrakeButton_Steer = 11,
        BrakeWheel_Steer = 12,
        ThrottleBrake1Tilt_Steer = 20,
        ThrottleBrake2Tilt_Steer = 21,
        ThrottleBrakeButton_Steer = 22,
        ThrottleBrakeWheel_Steer = 23,
        ShifterOnly = 30,
        HideAll=100,
    }
    /// <summary>
    /// Enable-Disable Touch Input. Disable required like states.
    /// </summary>
    public bool EnableTouch = true;
    /// <summary>
    /// Contais current touch data
    /// </summary>
    internal List<TouchItemData> TouchBank;
    /// <summary>
    /// Maximum touch size allowed by device
    /// </summary>
    private int BankSize = 5;
    /// <summary>
    /// Enable-Disable mouse emulation on development time
    /// </summary>
    public bool EnableMouseEmulation = false;
    /// <summary>
    /// Current control template state
    /// </summary>
    public eControlTemplate ControlTemplate = eControlTemplate.ThrottleBrake1Tilt_Steer;
    /// <summary>
    /// Previous template state
    /// </summary>
    public eControlTemplate oldTemplate;
    /// <summary>
    /// Enable-Disable control position flips
    /// </summary>
    public bool FlipPositions = false;
    /// <summary>
    /// Previous Flip Position
    /// </summary>
    [HideInInspector]
    public bool oldFlipPosition = false;
    /// <summary>
    /// Enable-Disable throttle and brake buttons movement on screen
    /// </summary>
    //private bool MoveToTouchPosition = false;
    /// <summary>
    /// Screen move speed of throttle and brake buttons
    /// </summary>
    internal float MoveSpeed = 0.3f;
    /// <summary>
    /// Throttle Button Input
    /// </summary>
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


    /// <summary>
    /// Configure the positions when template switch
    /// </summary>
    public bool EnableEditControlPositions = false;
    public bool showPA = true;
    public Vector4 PositionA = new Vector4(0.84f, 0.08f, 100, 200); //Throttle Position  | Throttle Brake Together at Right 
    public bool showPB1 = true;
    public Vector4 PositionB1 = new Vector4(0.616f, 0.08f, 150, 100); //Break Position 1 | Throttle Brake Together at Right
    public bool showPB2 = true;
    public Vector4 PositionB2 = new Vector4(0.15f, 0.08f, 150, 100); //Break Position 1 | Throttle Brake Together at Right
    public bool showPC = true;
    public Vector4 PositionC = new Vector4(0.75f, 0.05f, 100, 200); //Throttle or Brake position when button alone at Right
    public bool showPD = true;
    public Vector4 PositionD = new Vector4(0.07f, 0.08f, 100, 200); //SteerLeft position | Steerbuttons together at Left
    public bool showPE = true;
    public Vector4 PositionE = new Vector4(0.163f, 0.08f, 100, 200); //SteerRight position | Steerbutins together at Left
    public bool showPF = true;
    public Vector4 PositionF = new Vector4(0.078f, 0.08f, 200, 200); //SteeringWheel position at Left
    public bool showPG = true;
    public Vector4 PositionG = new Vector4(0.09f, 0.68f, 200, 100); //Shift up position at Left
    public bool showPH = true;
    public Vector4 PositionH = new Vector4(0.09f, 0.517f, 200, 100); //Shift down position at left
    public bool showPI = true;
    public Vector4 PositionI = new Vector4(0.09f, 0.517f, 200, 100); //Misc Button 1
    public bool showPJ = true;
    public Vector4 PositionJ = new Vector4(0.09f, 0.517f, 200, 100); //Misc Button 2

    private float sH;
    private float sW;
    // 1.1 feature stuff
    private float fRatW;
    private float fRatY;
    private Vector2 mXT = new Vector2(0.51f, 1f);
    private Vector2 mXB = new Vector2(0, 0.49f);

   


    ///// <summary>
    ///// All touch items in a container for reaching from other scripts easily.
    ///// </summary>
    //private List<TouchItemBase> _TouchItems = new List<TouchItemBase>(13);

    /// <summary>
    /// Define Touch bank items on awake. 
    /// </summary>
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


    void Start()
    {
        sH = Screen.height;
        sW = Screen.width;

        //start with initial template
        SwitchTemplate(ControlTemplate);
        mXT = new Vector2(0.51f, 1f);
        mXB = new Vector2(0, 0.49f);
    }

    /// <summary>
    /// Get touch item by id
    /// </summary>
    /// <param name="FID">TouchItem ID</param>
    /// <returns></returns>
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
        sH = Screen.height;
        sW = Screen.width;

        if (EnableTouch)
        {
            //Scan the touch inputs and update the bank
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
        }
        else
        {
            return;
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

     /// <summary>
    /// Switch Template to given. 
    /// </summary>
    public void SwitchTemplate(eControlTemplate TemplateTo)
    {
        ControlTemplate = TemplateTo;

        switch (TemplateTo)
        {
            case eControlTemplate.ThrottleTilt_Steer:
                EnableDisableButtons(true, false, false, false, false, false, false);
                MoveButtonsToPositions(PositionC, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, true, true, true,false,false);
                mXT = new Vector2(-0.1f, 1);
                mXB = new Vector2(-0.1f, 1);
                break;

            case eControlTemplate.ThrottleButton_Steer:
                EnableDisableButtons(true, false, true, true, false, false, false);
                MoveButtonsToPositions(PositionC, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, true, false, true,false,true);
                if (!FlipPositions)
                { mXT = new Vector2(0.51f, 1f); }
                else
                { mXT = new Vector2(-0.1f, 0.49f); }
                mXB = new Vector2(-0.1f, 0.49f);
                break;

            case eControlTemplate.ThrottleWheel_Steer:
                EnableDisableButtons(true, false, false, false, true, false, false);
                MoveButtonsToPositions(PositionC, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, true, false, true,true,false);
                if (!FlipPositions)
                { mXT = new Vector2(0.51f, 1f); }
                else
                { mXT = new Vector2(-0.1f, 0.49f); }
                mXB = new Vector2(-0.1f, 0.49f);
                break;

            case eControlTemplate.BrakeTilt_Steer:
                EnableDisableButtons(false, true, false, false, false, false, false);
                MoveButtonsToPositions(PositionC, PositionC, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(true, false, true, true,false,false);
                mXT = new Vector2(-0.1f, 1);
                mXB = new Vector2(-0.1f, 1);
                break;
            case eControlTemplate.BrakeButton_Steer:
                EnableDisableButtons(false, true, true, true, false, false, false);
                MoveButtonsToPositions(PositionC, PositionC, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(true, false, false, true,false,true);
                mXT = new Vector2(-0.1f, 1f);
                if (!FlipPositions)
                {
                    mXB = new Vector2(0.51f, 1f);
                }
                else
                { mXB = new Vector2(-0.1f, 0.49f); }
                break;

            case eControlTemplate.BrakeWheel_Steer:
                EnableDisableButtons(false, true, false, false, true, false, false);
                MoveButtonsToPositions(PositionC, PositionC, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(true, false, false, true,true,false);
                mXT = new Vector2(-0.1f, 1f);
                if (!FlipPositions)
                {
                    mXB = new Vector2(0.51f, 1f);
                }
                else
                { mXB = new Vector2(-0.1f, 0.49f); }
                break;

            case eControlTemplate.ThrottleBrake1Tilt_Steer:
                EnableDisableButtons(true, true, false, false, false, false, false);
                MoveButtonsToPositions(PositionA, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, false, true, true,false,false);
                mXT = new Vector2(0f, 0f);
                mXB = new Vector2(0f, 0f);
                break;
            case eControlTemplate.ThrottleBrake2Tilt_Steer:
                EnableDisableButtons(true, true, false, false, false, false, false);
                MoveButtonsToPositions(PositionC, PositionB2, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, false, true, true,false,false);
                if (!FlipPositions)
                {
                    mXT = new Vector2(0.51f, 1f);
                    mXB = new Vector2(-0.1f, 0.49f);
                }
                else
                {
                    mXT = new Vector2(-0.1f, 0.49f);
                    mXB = new Vector2(0.51f, 1f);
                }
                break;

            case eControlTemplate.ThrottleBrakeButton_Steer:
                EnableDisableButtons(true, true, true, true, false, false, false);
                MoveButtonsToPositions(PositionA, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, false, false, true,false,true);
                mXT = new Vector2(0f, 0f);
                mXB = new Vector2(0f, 0f);
                break;

            case eControlTemplate.ThrottleBrakeWheel_Steer:
                EnableDisableButtons(true, true, false, false, true, false, false);
                MoveButtonsToPositions(PositionA, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(false, false, false, true,true,false);
                mXT = new Vector2(0f, 0f);
                mXB = new Vector2(0f, 0f);
                break;

            case eControlTemplate.ShifterOnly:
                EnableDisableButtons(false, false, false, false, false, true, true);
                MoveButtonsToPositions(PositionA, PositionB1, PositionD, PositionE, PositionF, PositionG, PositionH, PositionI, PositionJ);
                ControlFeatures(true, false, false, false,false,false);
                break;
            case eControlTemplate.HideAll:
                EnableDisableButtons(false, false, false, false, false, false, false);
                break;

        }


    }
    /// <summary>
    /// Enable or disable buttons based the template config
    /// </summary>
    void EnableDisableButtons(bool bThrottle, bool bBreak, bool bSteerLeft, bool bSteerRight, bool bSteerWheel, bool bShiftUp, bool bShiftDown)
    {
        //if (Throttle != null) Throttle.gameObject.SetActiveRecursively(bThrottle);
        if (Throttle != null)
        {
            TouchButton tbThrottle = Throttle as TouchButton;
            tbThrottle.gameObject.SetActiveRecursively(bThrottle);
        }
        if (Brake != null)
        {
            TouchButton tbBrake = Brake as TouchButton;
            tbBrake.gameObject.SetActiveRecursively(bBreak);
        }
        if (SteerLeft != null)
        {
            TouchButton tbSteerLeft = SteerLeft as TouchButton;
            tbSteerLeft.gameObject.SetActiveRecursively(bSteerLeft);
        }
        if (SteerRight != null)
        {
            TouchButton tbSteerRight = SteerRight as TouchButton;
            tbSteerRight.gameObject.SetActiveRecursively(bSteerRight);
        }
        if (ShiftUp != null)
        {
            TouchButton tbShiftUp = ShiftUp as TouchButton;
            tbShiftUp.gameObject.SetActiveRecursively(bShiftUp);
        }
        if (ShiftDown != null)
        {
            TouchButton tbShiftDown = ShiftDown as TouchButton;
            tbShiftDown.gameObject.SetActiveRecursively(bShiftDown);
        }
        if (Wheel != null)
        {
            TouchWheel twWheel = Wheel as TouchWheel;
            twWheel.gameObject.SetActiveRecursively(bSteerWheel);
        }

        //if (Brake != null) Brake.gameObject.SetActiveRecursively(bBreak);
        //if (SteerLeft != null) SteerLeft.gameObject.SetActiveRecursively(bSteerLeft);
        //if (SteerRight != null) SteerRight.gameObject.SetActiveRecursively(bSteerRight);
        //if (Wheel != null) Wheel.gameObject.SetActiveRecursively(bSteerWheel);
        //if (ShiftUp != null) ShiftUp.gameObject.SetActiveRecursively(bShiftUp);
        //if (ShiftDown != null) ShiftDown.gameObject.SetActiveRecursively(bShiftDown);
    }
    /// <summary>
    /// Move positions to preconfigured positions 
    /// </summary>
    void MoveButtonsToPositions(Vector4 vThrottle, Vector4 vBrake, Vector4 vSteerLeft, Vector4 vSteerRight, Vector4 vSteerWheel, Vector4 vShiftUp, Vector4 vShiftDown, Vector4 vMisc1, Vector4 vMisc2)
    {
        if (Throttle != null)
        {
            TouchButton tbThrottle = Throttle as TouchButton;
            tbThrottle.transform.position = FlipPosition(vThrottle);
        }
        if (Brake != null) Brake.transform.position = FlipPosition(vBrake);
        if (SteerLeft != null) SteerLeft.transform.position = FlipPosition(vSteerLeft);
        if (FlipPositions && SteerLeft != null) SteerLeft.transform.position = FlipPosition(vSteerRight);//fix swap position 
        if (SteerRight != null) SteerRight.transform.position = FlipPosition(vSteerRight);
        if (FlipPositions && SteerRight != null) SteerRight.transform.position = FlipPosition(vSteerLeft); //fix swap position
        if (Wheel != null) Wheel.transform.position = FlipPosition(vSteerWheel);
        if (ShiftUp != null) ShiftUp.transform.position = FlipPosition(vShiftUp);
        if (FlipPositions && ShiftUp != null) ShiftUp.transform.position = FlipPosition(vShiftDown);
        if (ShiftDown != null) ShiftDown.transform.position = FlipPosition(vShiftDown);
        if (FlipPositions && ShiftDown != null) ShiftDown.transform.position = FlipPosition(vShiftUp);
        if (Misc1Button != null) Misc1Button.transform.position = FlipPosition(vMisc1);
        if (Misc2Button != null) Misc2Button.transform.position = FlipPosition(vMisc2);

    }
    /// <summary>
    /// Set the vehicle control features based to template.
    /// </summary>
    void ControlFeatures(bool bAutoThrottle, bool bAutoBrake, bool bTiltSteer, bool vAutoGear,bool vTouchWheel,bool vButtonSteer)
    {
        _EnableAutoThrottle = bAutoThrottle;
        _EnableAutoBrake = bAutoBrake;
        _EnableTiltSteer = bTiltSteer;
        _EnableTouchWheelSteer = vTouchWheel;
        _EnableButtonSteer = vButtonSteer;
        _EnableAutoGear = vAutoGear;
    }
    /// <summary>
    /// Swap the item positions based their configured positions on screen. Its basic calculation
    /// </summary>
    Vector4 FlipPosition(Vector4 Source)
    {
        if (!FlipPositions)
        {
            return Source;
        }
        else
        {
            float swappedX = 1 - Source.x;
            swappedX = swappedX - (1 / sW) * Source.z;
            Vector4 vSwapped = new Vector4(swappedX, Source.y, Source.z, Source.w);
            return vSwapped;
        }
    }

    void OnGUI()
    {
        if (EnableEditControlPositions)
        {
            sH = Screen.height;
            sW = Screen.width;

            Color areaBG = Color.red;
            areaBG.a = 0.25f;
            if (showPA) DrawRect(PositionA, areaBG, "Position A\r\nThrottle");
            if (showPB1) DrawRect(PositionB1, areaBG, "Position B1\r\nBrake");
            if (showPB2) DrawRect(PositionB2, areaBG, "Position B2\r\nBrake");
            if (showPC) DrawRect(PositionC, areaBG, "Position C\r\nThrottle or Brake");
            if (showPD) DrawRect(PositionD, areaBG, "Position D\r\nSteer Left");
            if (showPE) DrawRect(PositionE, areaBG, "Position E\r\nSteer Right");
            if (showPF) DrawRect(PositionF, areaBG, "Position F\r\n(Wheel)");
            if (showPG) DrawRect(PositionG, areaBG, "Position F\r\n(Shift Up)");
            if (showPH) DrawRect(PositionH, areaBG, "Position F\r\n(Shift Down)");
            if (showPI) DrawRect(PositionI, areaBG, "Position I\r\n(Misc Button 1)");
            if (showPJ) DrawRect(PositionJ, areaBG, "Position J\r\n(Misc Button 2)");
        }
    }

    /// <summary>
    /// Draw rectangles for Position Edit mode
    /// </summary>
    void DrawRect(Vector4 position, Color colorm, string positionName)
    {
        if (position.x <= -1) position.x = -1f;
        if (position.y <= -1) position.y = -1f;
        if (position.x >= 1) position.x = 1f;
        if (position.y >= 1) position.y = 1f;
        GUI.Box(new Rect((position.x * sW), (sH - position.w - position.y * sH), position.z, position.w), positionName + "\r\n" + position.x.ToString() + "-" + position.y.ToString());
    }

  
}


#region NestedClasses

/// <summary>
/// Touch Items, touch state and phase data
/// </summary>
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