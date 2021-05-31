//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;


[AddComponentMenu("Racing Game Kit/Basic Car/RGK Basic Car")]
public class RGK_BasicCar : MonoBehaviour
{

    /// <summary>
    /// Wheel Collideders for Vehicle. Important : In order of : FronLeft, FrontRight, RearLeft,RearRight
    /// </summary>
    public WheelCollider[] WheelColliders;
    /// <summary>
    /// Is front drive enabled
    /// </summary>
    public Boolean FrontDrive = false;
    /// <summary>
    /// Is rear drive enabled
    /// </summary>
    public Boolean RearDrive = true;
    /// <summary>
    /// Maximum steering angle
    /// </summary>
    public float MaxSteering = 25f;


    //Engine
    public float EngineTorque = 100.0f;
    public float EngineRpmMax = 3000.0f;
    public float EngineRpmMin = 1000.0f;
    public float EngineRpm = 0.0f;
    public float EngineSpeed = 0f;
    public int EngineGear = 0;

    public int BrakeTorqueFront = 40;
    public int BrakeTorqueRear = 60;
    public int HandBrakeTorque = 100;

    [System.NonSerialized]
    public float EngineThrottle;
    //GearBox
    public float[] GearRatio = new float[] { 4.31f, 2, 2.3f, 1.8f, 1.1f, 0.8f };

    public Vector3 centerOfMass = new Vector3(0, -0.3f, 0);
    public Transform CenterOfMassObject;

    /// <summary>
    /// WheelCollider Properties
    /// This one allows manage whell collider data from one place. 
    /// </summary>
    [System.Serializable]
    public class WheelData : System.Object
    {
        public float Spring = 5000.0f;
        public float Damper = 50.0f;
        public float Distance = 0.2f;
        public float WheelWeight = 5;


        public float FW_ForwardFriction = 1f;
        public float FW_ForwardExtremumSlip = 0.1f;
        public float FW_ForwardExtremumValue = 5500;
        public float FW_ForwardAsymptoteSlip = 1f;
        public float FW_ForwardAsymptoteValue = 3250;


        public float FW_SidewayFriction = 0.3f;
        public float FW_SidewayExtremumSlip = 0.3f;
        public float FW_SidewayExtremumValue = 5500;
        public float FW_SidewayAsymptoteSlip = 2f;
        public float FW_SidewayAsymptoteValue = 750;



        public float RW_ForwardFriction = 1f;
        public float RW_ForwardExtremumSlip = 0.1f;
        public float RW_ForwardExtremumValue = 5500;
        public float RW_ForwardAsymptoteSlip = 1f;
        public float RW_ForwardAsymptoteValue = 3250;


        public float RW_SidewayFriction = 0.5f;
        public float RW_SidewayExtremumSlip = 0.5f;
        public float RW_SidewayExtremumValue = 5500;
        public float RW_SidewayAsymptoteSlip = 2f;
        public float RW_SidewayAsymptoteValue = 750;

    }

    /// <summary>
    /// Actual reference  of WheelData type
    /// </summary>
    public WheelData _WheelData;



    void Start()
    {
        if (CenterOfMassObject != null)
        {
            this.centerOfMass = CenterOfMassObject.localPosition;
        }
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        UpdateWheelData();
    }


    /// <summary>
    /// Update wheel data from race script. 
    /// You may want customize this values for AI or Human Racer. 
    /// </summary>
    public void UpdateWheelData()
    {
        foreach (WheelCollider _WC in WheelColliders)
        {
            WheelFrictionCurve ForwardFriction = _WC.forwardFriction;
            WheelFrictionCurve SidewaysFriction = _WC.sidewaysFriction;
            JointSpring JointSpringForCollider = _WC.suspensionSpring;

            JointSpringForCollider.spring = _WheelData.Spring;
            JointSpringForCollider.damper = _WheelData.Damper;

            _WC.suspensionSpring = JointSpringForCollider;
            _WC.suspensionDistance = _WheelData.Distance;
            _WC.mass = _WheelData.WheelWeight;



            if (_WC == WheelColliders[0] || _WC == WheelColliders[1]) // Front Wheels
            {
                ForwardFriction.stiffness = _WheelData.FW_ForwardFriction;
                ForwardFriction.extremumSlip = _WheelData.FW_ForwardExtremumSlip;
                ForwardFriction.extremumValue = _WheelData.FW_ForwardExtremumValue;
                ForwardFriction.asymptoteSlip = _WheelData.FW_ForwardAsymptoteSlip;
                ForwardFriction.asymptoteValue = _WheelData.FW_ForwardAsymptoteValue;

                SidewaysFriction.stiffness = _WheelData.FW_SidewayFriction;
                SidewaysFriction.extremumSlip = _WheelData.FW_SidewayExtremumSlip;
                SidewaysFriction.extremumValue = _WheelData.FW_SidewayExtremumValue;
                SidewaysFriction.asymptoteSlip = _WheelData.FW_SidewayAsymptoteSlip;
                SidewaysFriction.asymptoteValue = _WheelData.FW_SidewayAsymptoteValue;
            }

            else //Rear Wheels
            {
                ForwardFriction.stiffness = _WheelData.RW_ForwardFriction;
                ForwardFriction.extremumSlip = _WheelData.RW_ForwardExtremumSlip;
                ForwardFriction.extremumValue = _WheelData.RW_ForwardExtremumValue;
                ForwardFriction.asymptoteSlip = _WheelData.RW_ForwardAsymptoteSlip;
                ForwardFriction.asymptoteValue = _WheelData.RW_ForwardAsymptoteValue;

                SidewaysFriction.stiffness = _WheelData.RW_SidewayFriction;
                SidewaysFriction.extremumSlip = _WheelData.RW_SidewayExtremumSlip;
                SidewaysFriction.extremumValue = _WheelData.RW_SidewayExtremumValue;
                SidewaysFriction.asymptoteSlip = _WheelData.RW_SidewayAsymptoteSlip;
                SidewaysFriction.asymptoteValue = _WheelData.RW_SidewayAsymptoteValue;
            }

            _WC.forwardFriction = ForwardFriction;
            _WC.sidewaysFriction = SidewaysFriction;
        }
    }

    /// <summary>
    /// Gear shifting code
    /// </summary>
    public void ShiftGears()
    {
        int AppropriateGear = Gear;

        if (EngineRpm >= EngineRpmMax)
        {

            for (int i = 0; i < GearRatio.Length; i++)
            {
                if (WheelColliders[0].rpm * GearRatio[i] < EngineRpmMax)
                {
                    AppropriateGear = i;
                    break;
                }
            }

            EngineGear = AppropriateGear;
        }

        if (Rpm <= EngineRpmMin)
        {
            AppropriateGear = Gear;

            for (int j = GearRatio.Length - 1; j >= 0; j--)
            {
                if (WheelColliders[0].rpm * GearRatio[j] > EngineRpmMin)
                {
                    AppropriateGear = j;
                    break;
                }
            }

            EngineGear = AppropriateGear;
        }
    }
    public bool IsReversing = false;

    public void ApplyDrive(float Throttle, float Brake, Boolean HandBrake)
    {
        //Debug.Log(Throttle + "-" + Brake + "-" + HandBrake.ToString());
        this.EngineThrottle = Throttle;
        if (Throttle > 0 && !HandBrake)
        {
            ResetBrakeTorques();

            if (IsReversing) IsReversing = false;
            if (FrontDrive)
            {
                WheelColliders[0].motorTorque = EngineTorque / GearRatio[EngineGear] * Throttle;
                WheelColliders[1].motorTorque = EngineTorque / GearRatio[EngineGear] * Throttle;
            }
            if (RearDrive)
            {
                WheelColliders[2].motorTorque = EngineTorque / GearRatio[EngineGear] * Throttle;
                WheelColliders[3].motorTorque = EngineTorque / GearRatio[EngineGear] * Throttle;
            }

        }
        else if (HandBrake)
        {
            WheelColliders[2].brakeTorque = HandBrakeTorque;
            WheelColliders[3].brakeTorque = HandBrakeTorque;
        }
        if (Mathf.Abs(Brake) > 0)
        {
            EngineGear = 0;
            if (EngineSpeed > 0.2 && !IsReversing)
            {
                WheelColliders[0].motorTorque = 0;
                WheelColliders[1].motorTorque = 0;

                WheelColliders[0].brakeTorque = BrakeTorqueFront * Brake ;
                WheelColliders[1].brakeTorque = BrakeTorqueFront * Brake ;
                WheelColliders[2].brakeTorque = BrakeTorqueRear * Brake ;
                WheelColliders[3].brakeTorque = BrakeTorqueRear * Brake ;
            }
            else
            {
                IsReversing = true;
            }
            if (IsReversing)
            {
                ResetBrakeTorques();
                if (FrontDrive)
                {
                    WheelColliders[0].motorTorque = 50 * Brake * -1;
                    WheelColliders[1].motorTorque = 50 * Brake * -1;
                }
                if (RearDrive)
                {
                    WheelColliders[2].motorTorque = 50 * Brake * -1;
                    WheelColliders[3].motorTorque = 50 * Brake * -1;
                }
            }
        }


    }

    private void ResetBrakeTorques()
    {
        WheelColliders[0].brakeTorque = 0;
        WheelColliders[1].brakeTorque = 0;
        WheelColliders[2].brakeTorque = 0;
        WheelColliders[3].brakeTorque = 0;
    }
    public void ApplySteer(float Steer)
    {
        WheelColliders[0].steerAngle = Steer;
        WheelColliders[1].steerAngle = Steer;
    }


    void LateUpdate()
    {
        EngineRpm = Mathf.Abs(WheelColliders[0].rpm + WheelColliders[1].rpm) / 2 * GearRatio[EngineGear];
        EngineSpeed = base.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;

        UpdateWheelData();

    }

    public float Rpm
    {
        get { return EngineRpm; }
    }
    public float Speed
    {
        get { return EngineSpeed; }
    }
    public int Gear
    {
        get { return EngineGear; }
        set { EngineGear = value; }
    }

    public float MaxSteer
    {
        get { return MaxSteering; }
        set { MaxSteering = value; }
    }

}
