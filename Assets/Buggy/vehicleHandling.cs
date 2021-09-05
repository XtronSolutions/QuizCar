using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;
public class vehicleHandling : MonoBehaviour {

    public Transform CoM;

    [Header("Respawn Parameters")]
    [HideInInspector]
    public bool turboLocked = false;
    // For Booster
    float _turbo = 1;
    float _lastTurbo = 0;
    float _turboCooldown = 0;
    public float turbo { get { return _turbo; } }

    public bool isOilSpil = false;
    public bool isIceCube = false;
    public float turboDuration = 0.5f;
    public float turboBurning = 2;
    public float turboStrength = 2.25f;
    public float totalNitroTime = 0.09f;

    [Header("Respawn Parameters")]
    [Tooltip("Position to respawn vehicle")]
    public Vector3 spawnPosition = new Vector3(5, 10, 0);
    [Tooltip("Orientation to respawn vehicle")]
    public Vector3 spawnRotation = new Vector3(0, 0, 0);
    [Tooltip("Height which triggers respawn")]
    public float respawnHeight = -10f;


    [Header("Handling Parameters")]
    public float airborneDownforce = 0;
    [Tooltip("Curve for specifying steering input to steering angle")]
    public AnimationCurve steeringCurve;
    [Tooltip("Angle multiplier for Android tilt steering")]
    public float steeringSensitivity = 3;
    [Tooltip("Acceleration force provided while stationary")]
    public float maximumAcceleration = 100;
    [Tooltip("Rate that acceleration decreases with respect to speed")]
    public float accelerationDropOff = 0.1f;
    [Tooltip("Maximum reverse acceleration")]
    public float reverseAcceleration = 100;
    [Tooltip("Percentage of brake bias to front wheels")]
    public float frontBrakeBias = 0.6f;


    [Tooltip("Maximum possible steering angle")]
    public float maxSteeringAngle = 40f;
    [Tooltip("Steering torque which is applied directly to the vehicle - this allows it to turn in the air")]
    public float standardSteeringFactor = 4000f;
    [Tooltip("Rate that acceleration decreases with respect to speed")]
    public float steeringDropOff = 0.02f;

    [Tooltip("Check if vehicle operates as four wheel drive")]
    public bool fourWheelDrive = false;
    [Tooltip("If four wheel drive then this is the percentage of power to the rear")]
    public float rearAccelerationBias = 0.5f;

    [Tooltip("How much slowing should be applied on collision")]
    public float collisionDampFactor = 1000;

    [Tooltip("Front wheel grip when drifting")]
    public float frontDriftStiffness = 1.1f;
    [Tooltip("Rear wheel grip when drifting")]
    public float rearDriftStiffness = 0.67f;


    [Header("Vehicle Componenets")]
    [Tooltip("Front right wheel object")]
    public Transform wheelFR;
    [Tooltip("Front left wheel object")]
    public Transform wheelFL;
    [Tooltip("Rear right wheel object")]
    public Transform wheelRR;
    [Tooltip("Rear left wheel object")]
    public Transform wheelRL;

    [Tooltip("Front right wheel collider")]
    public WheelCollider wheelColliderFR;
    [Tooltip("Front left wheel collider")]
    public WheelCollider wheelColliderFL;
    [Tooltip("Rear right wheel collider")]
    public WheelCollider wheelColliderRR;
    [Tooltip("Rear left wheel collider")]
    public WheelCollider wheelColliderRL;
    UnityStandardAssets.Vehicles.Car.WheelEffects WheelEffectsFL, WheelEffectsFR, WheelEffectsRL, WheelEffectsRR;
    //inputs
    private float accelerationInput;
    public float _AccelerationInput { get { return accelerationInput; } set { accelerationInput = value; } }
    private float steeringInput;
    public float _SteeringInput { get { return steeringInput; } set { steeringInput = value; } }
    //vehicle conditions
    private bool accelerating = false;
    private bool drifting = true;
    public float vehicleSpeed;
    public float _VehicleSpeed { get { return vehicleSpeed; } }
    public float MAXvehicleSpeed;

    private Vector3 vehicleTrajectory;
    internal bool isSleeping = false;
    //extra variables which are filled in on Start
    private Rigidbody rigid;
    private WheelFrictionCurve frontGrip;
    private WheelFrictionCurve frontDrift;
    private WheelFrictionCurve rearGrip;
    private WheelFrictionCurve rearDrift;

    //braking force is set to private as it probably shouldn't be modified, instead adjust the braking characteristics with the wheel colliders forward friction curve
    private float brakingForce = 6000;
    public List<PigeonCoopToolkit.Effects.Trails.Trail> TankTrackTrails;
    bool isManagedByAI;
    public RandomTargetSwitcher randomargetSwitcher;

    private float respawnTimer = 10f;
    public bool carStuck = false;
    public GameObject IceCube;
    private bool FirstTimeSpill = true;
    private bool SecondTimeSpill = false;
    //	public ParticleSystem exhaust;

    public GameObject HeadLights, BrakeLights;
    public GameObject nitroObj;
    Racer_Register race_register;
    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.mute = !GameData.isSound;

        race_register = GetComponent<Racer_Register>();

        isManagedByAI = GetComponent<BC_AI_Helper>() != null;
        //get the rigidbody for future reference
        rigid = this.GetComponent<Rigidbody>();
        if(CoM!=null)
        rigid.centerOfMass = CoM.localPosition;
        WheelEffectsFL = wheelColliderFL.GetComponent<UnityStandardAssets.Vehicles.Car.WheelEffects>();
        WheelEffectsFR = wheelColliderFR.GetComponent<UnityStandardAssets.Vehicles.Car.WheelEffects>();
        WheelEffectsRL = wheelColliderRL.GetComponent<UnityStandardAssets.Vehicles.Car.WheelEffects>();
        WheelEffectsRR = wheelColliderRR.GetComponent<UnityStandardAssets.Vehicles.Car.WheelEffects>();

        //store grip curves from the left wheels (under assumption left and right should be the same)
        frontGrip = wheelColliderFL.sidewaysFriction;
        frontDrift = wheelColliderFL.sidewaysFriction;
        rearGrip = wheelColliderRL.sidewaysFriction;
        rearDrift = wheelColliderRL.sidewaysFriction;

        //set drift stiffness values from user input
        frontDrift.stiffness = frontDriftStiffness;
        rearDrift.stiffness = rearDriftStiffness;

        //start in grip mode (change right wheels)
        wheelColliderFR.sidewaysFriction = frontGrip;
        wheelColliderRR.sidewaysFriction = rearGrip;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Resources.UnloadUnusedAssets();


        WeatherManager wm = GameObject.FindObjectOfType<WeatherManager>();
        if (wm != null && !isManagedByAI)
            HeadLights.SetActive(!GameObject.FindObjectOfType<WeatherManager>().isDay);

    }

    // There is only one visual function which needs to be placed in 'Update'
    void Update()
    {
        //move physical wheels to match colliders positions
        moveWheels();
        //			if (Mathf.Abs(vehicleSpeed) > 0)
        //			{
        //				TankTrackTrails.ForEach(trail => { trail.Emit = true; });
        //			}
        //			else
        //			{
        //				TankTrackTrails.ForEach(trail => { trail.Emit = false; });
        //			}

        if (race_register != null)
        {
            if (race_register.IsRacerFinished)
                return;
        }
        if (vehicleSpeed < 5 && vehicleSpeed > -5)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer < 0)
            {
                carStuck = true;
                respawnTimer = 5;
            }
        }
        else
        {
            carStuck = false;
            respawnTimer = 5;
        }

    }

    bool isFirstTime = true;
    //all the physics needs to be in 'FixedUpdate'
    void FixedUpdate()
    {
        //Debug.Log(this.gameObject.name + PlayerManagerScript.instance._RaceManager.IsRaceStarted + "   " + GetComponent<Rigidbody>().isKinematic );

        if ((!Constants.isMultiplayerSelected && PlayerManagerScript.instance._RaceManager.CurrentCount < 1) ||
            (Constants.isMultiplayerSelected && PlayerManagerScript.instance._RaceManager.IsRaceStarted))
        {

            if (!isManagedByAI)
            {
                if(Controls.StartEngine==true)
                {
                    Controls.acceleration = 1;
                    Controls.StartEngine = false;
                }

                if(Controls.IsHandBrake==true)
                {
                    //Debug.Log("Applied handbrake");
                    applyBrakeForce(999999);
                    Controls.acceleration = 0;
                    accelerationInput = 0;
                    return;
                }

                accelerationInput = Controls.acceleration;
                steeringInput = Controls.steerVal;

                if(race_register.IsRacerFinished)
                {
                    applyBrakeForce(999999);
                    accelerationInput = 0;
                    steeringInput = 1;
                }

                if (accelerationInput <= 0)
                    BrakeLights.SetActive(true);
                else
                    BrakeLights.SetActive(false);

            }

            /*
            //if desktop, get axes as set in Input settings, if mobile get phone angle for steering, auto accelerate and get braking as screen touch
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
            if (!isManagedByAI) {

               

                
				if(Input.GetMouseButton(0) && Input.mousePosition.y / Screen.height < 0.65f){
					accelerationInput = -1;
				}else{
				accelerationInput = 1;//Input.GetAxis ("Acceleration");
				}
				steeringInput = Input.GetAxis ("Steering");

			    } else {
				//				exhaust.emissionRate = vehicleSpeed;
                
            }
			
			
			#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			if(!isManagedByAI)
			{
				steeringInput = Mathf.Clamp((Input.acceleration.x) * steeringSensitivity, -1, 1);
				
				//set braking on screen touch, otherwise accelerate
				if (Input.touchCount > 0 && Input.GetTouch(0).position.y / Screen.height < 0.65f) 
					accelerationInput = -1;
				else 
					accelerationInput = 1;
			}
			
			#endif
            */


            //get speed based on vehicle direction

            if (Constants.isMultiplayerSelected && isFirstTime)
            {
                isFirstTime = false;
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }

            vehicleTrajectory = Quaternion.Inverse(transform.rotation) * rigid.velocity;
            vehicleSpeed = vehicleTrajectory.z;

            if (vehicleSpeed < 20f || Mathf.Abs(rigid.angularVelocity.magnitude) > 1.5f)
            {
                isSleeping = false;
            }
            else
                isSleeping = true;

            //add acceleration force
            if (drifting)
                maintainDrift();


            accelerate(accelerationInput);

            //set steering angles and apply steering force
            if (isOilSpil)
            {
                steerOnOil(steeringInput);
            }
            else
                steer(steeringInput);

            // make sure the vehicle doesn't flip over
            selfRight();

            // add downforce when airborne
            if (airborneDownforce != 0 && !wheelColliderFR.isGrounded && !wheelColliderFL.isGrounded && !wheelColliderRR.isGrounded && !wheelColliderRL.isGrounded)
                rigid.AddForce(Vector3.down * airborneDownforce);

            //respawn vehicle if it falls too low
            //if (transform.position.y < respawnHeight)
            //{
            //	transform.position = spawnPosition;
            //	transform.rotation = Quaternion.Euler(spawnRotation);
            //	rigid.velocity = Vector3.zero;
            //	rigid.angularVelocity = Vector3.zero;
            //}
        }
        else
        {
            applyBrakeForce(brakingForce);
        }
    }

    public void accelerate(float input)
    {
        //if accelerating
        if (input >= 0)
        {
            //if accelerating while moving forwards then force is max* input divided by speed factor
            if (vehicleSpeed >= 0)
            {
                if (PlayerManagerScript.instance != null && !PlayerManagerScript.instance.BoostON)
                {
                    //					if(Camera.main.gameObject.GetComponent<RacingGameKit.Race_Camera>()!=null)
                    //Camera.main.gameObject.GetComponent<RacingGameKit.Race_Camera>().GameCameraSettings.Distance = 5.5f;

                    applyAccelerationForce(input * maximumAcceleration / (accelerationDropOff * vehicleSpeed + 1));

                }
                else if (PlayerManagerScript.instance.BoostON)
                {
                    //					Camera.main.gameObject.GetComponent<RacingGameKit.Race_Camera>().GameCameraSettings.Distance = 5f;
                    applyAccelerationForce(input * maximumAcceleration);
                    //					Debug.Log ("Accelerate with Boost");
                }
            }

            //if accelerating while moving backwards then force is max* input(no decrease based on speed)
            else if (PlayerManagerScript.instance != null) //  && !PlayerManagerScript.instance.BoostON
                applyAccelerationForce(input * maximumAcceleration);
        }
        //if braking / reverse
        else //if(true){//!PlayerManagerScript.instance.BoostON) 
        {
            //if moving forwards then use brakes
            if (vehicleSpeed > 0.1)
            {
                applyBrakeForce(-input * brakingForce);
            }
            //if not moving forwards then accelerate backwards
            else
            {
                applyAccelerationForce(input * reverseAcceleration / (accelerationDropOff * -vehicleSpeed + 1));
            }
        }
    }

    //apply acceleration force to each applicable wheel
    public void applyAccelerationForce(float totalForce)
    {
        float wheelForce;
        if (!fourWheelDrive)
        {
            //two wheel drive acceleraion (half to each rear wheel)
            wheelForce = totalForce / 2;

            wheelColliderRR.motorTorque = wheelForce;
            wheelColliderRL.motorTorque = wheelForce;
        }
        else
        {
            //four wheel drive acceleration (front rear bias applied then halved between sides)
            wheelForce = totalForce * rearAccelerationBias / 2;

            wheelColliderRR.motorTorque = wheelForce;
            wheelColliderRL.motorTorque = wheelForce;

            wheelForce = totalForce * (1 - rearAccelerationBias) / 2;

            wheelColliderFR.motorTorque = wheelForce;
            wheelColliderFL.motorTorque = wheelForce;
        }

        if (!accelerating)
        {
            accelerating = true;
            zeroBrake();
            detectDrift();
        }
        if (WheelEffectsFL != null) WheelEffectsFL.EmitTyreSmoke();
        if (WheelEffectsFR != null) WheelEffectsFR.EmitTyreSmoke();
        if (WheelEffectsRL != null) WheelEffectsRL.EmitTyreSmoke();
        if (WheelEffectsRR != null) WheelEffectsRR.EmitTyreSmoke();

    }

    //reset acceleration to zero (when applying brake force)
    void zeroAcceleration()
    {
        if (!fourWheelDrive)
        {
            wheelColliderRR.motorTorque = 0;
            wheelColliderRL.motorTorque = 0;
        }
        else
        {
            wheelColliderRR.motorTorque = 0;
            wheelColliderRL.motorTorque = 0;
            wheelColliderFR.motorTorque = 0;
            wheelColliderFL.motorTorque = 0;
        }
    }

    //apply appropriate brake force to each wheel
    public void applyBrakeForce(float totalForce)
    {
        float wheelForce;
        //brake force is divided based on front rear bias then halved for each side
        wheelForce = totalForce * (1 - frontBrakeBias) / 2;
        wheelColliderRR.brakeTorque = wheelForce;
        wheelColliderRL.brakeTorque = wheelForce;

        wheelForce = totalForce * frontBrakeBias / 2;
        wheelColliderFR.brakeTorque = wheelForce;
        wheelColliderFL.brakeTorque = wheelForce;

        if (accelerating)
        {
            accelerating = false;
            zeroAcceleration();
        }
    }

    //apply zero brake force when going from braking to accelerating
    void zeroBrake()
    {
        wheelColliderRR.brakeTorque = 0;
        wheelColliderRL.brakeTorque = 0;
        wheelColliderFR.brakeTorque = 0;
        wheelColliderFL.brakeTorque = 0;
    }


    public void steer(float input)
    {
        //apply steering curve to get steeringValue
        var steeringValue = steeringCurve.Evaluate(Mathf.Abs(input));
        if (input < 0)
            steeringValue = -steeringValue;

        //calcualte actual steering angle (and apply speed drop off)
        float steeringAngle = steeringValue * maxSteeringAngle / (steeringDropOff * Mathf.Abs(vehicleSpeed) + 1);
        //if (vehicleSpeed<0) steeringAngle = -steeringAngle;

        //apply steering angle to wheel colliders
        wheelColliderFL.steerAngle = steeringAngle;
        wheelColliderFR.steerAngle = steeringAngle;

        //add standard steering force
        rigid.AddTorque(transform.up * steeringValue * standardSteeringFactor);
        //Debug.LogError("Val: "+transform.up * steeringValue * 20000);

    }


    public void steerOnOil(float input)
    {
        //apply steering curve to get steeringValue

        var steeringValue = steeringCurve.Evaluate(Mathf.Abs(input));
        if (input < 0)
            steeringValue = -steeringValue;

        //calcualte actual steering angle (and apply speed drop off)
        float steeringAngle = steeringValue * maxSteeringAngle / (steeringDropOff * Mathf.Abs(vehicleSpeed) + 1);
        //if (vehicleSpeed<0) steeringAngle = -steeringAngle;

        //apply steering angle to wheel colliders
        if (FirstTimeSpill)
        {
            FirstTimeSpill = false;
            wheelColliderFL.steerAngle = steeringAngle + 25f;
            wheelColliderFR.steerAngle = steeringAngle + 25f;
            Invoke("EnableSecondTime", 1f);
        }
        if (SecondTimeSpill)
        {

            //			wheelColliderFL.steerAngle = steeringAngle - 20f;
            //			wheelColliderFR.steerAngle = steeringAngle - 20f;
            wheelColliderFL.steerAngle = steeringAngle;
            wheelColliderFR.steerAngle = steeringAngle;
            //SecondTimeSpill = false;
            //Invoke ("EnableFirstTime", 1f);
        }
        //add standard steering force
        rigid.AddTorque(transform.up * steeringValue * 20000);

    }

    void EnableSecondTime()
    {

        SecondTimeSpill = true;
    }

    void EnableFirstTime()
    {

        FirstTimeSpill = true;
    }

    void moveWheels()
    {
        setWheelPosition(wheelColliderFL, wheelFL);
        setWheelPosition(wheelColliderFR, wheelFR);
        setWheelPosition(wheelColliderRL, wheelRL);
        setWheelPosition(wheelColliderRR, wheelRR);
    }

    void setWheelPosition(WheelCollider col, Transform obj)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);
        obj.position = position;
        obj.rotation = rotation;
    }



    void OnCollisionEnter(Collision coll)
    {
        //		if (PlayerManagerScript.instance._animController != null) {
        //			if (Random.Range (0, 2) == 0)
        //				PlayerManagerScript.instance._animController.SetAnimationTrigger (CharacterAnimationController.Triggers.isStrokeRight);
        //			else
        //				PlayerManagerScript.instance._animController.SetAnimationTrigger (CharacterAnimationController.Triggers.isStrokeLeft);
        //		}
   //     float mag = 1 + coll.impulse.magnitude * collisionDampFactor; // scale collision magnitude

     //   if (rigid != null)
      //      rigid.velocity = rigid.velocity / mag; //divide velocity by scaled magnitude
    }

    public void selfRight()
    {
        //get eulerAngles of vehicle
        Vector3 vehicleAngle = transform.eulerAngles;

        if (vehicleAngle.x > 30 && vehicleAngle.x < 320)
        {
            if (vehicleAngle.x < 180) rigid.AddTorque(transform.right * (vehicleAngle.x - 30) * -100);
            else rigid.AddTorque(transform.right * (330 - vehicleAngle.x) * 100);

        }

        if (vehicleAngle.z > 30 && vehicleAngle.z < 320)
        {
            if (vehicleAngle.z < 180) rigid.AddTorque(transform.forward * (vehicleAngle.z - 30) * -100);
            else rigid.AddTorque(transform.forward * (330 - vehicleAngle.z) * 100);
        }
    }

    void detectDrift()
    {
        //		Debug.Log ("Drift Value: " + Mathf.Abs(vehicleTrajectory.x / vehicleSpeed));
        //if slip angle is high enough then initiate drift by reducing rear sideways grip and increase front
        if (Mathf.Abs(vehicleTrajectory.x / vehicleSpeed) > 0.1)
        {
            drifting = true;
            //			TankTrackTrails.ForEach(trail => { trail.Emit = true; });
            //make grip changes
            //isSleeping = true;
            wheelColliderFR.sidewaysFriction = frontDrift;
            wheelColliderFL.sidewaysFriction = frontDrift;
            wheelColliderRR.sidewaysFriction = rearDrift;
            wheelColliderRL.sidewaysFriction = rearDrift;
        }
    }

    void maintainDrift()
    {
        if (Mathf.Abs(vehicleTrajectory.x / vehicleSpeed) < 0.2)
        {
            drifting = false;
            //isSleeping = false;
            //make grip changes

            wheelColliderFR.sidewaysFriction = frontGrip;
            wheelColliderFL.sidewaysFriction = frontGrip;
            wheelColliderRR.sidewaysFriction = rearGrip;
            wheelColliderRL.sidewaysFriction = rearGrip;
            //			TankTrackTrails.ForEach(trail => { trail.Emit = false; });
        }
    }
    //ADNAN code
    public void Move()
    {

    }

    public void DestroyCar()
    {
        Destroy(gameObject, 2f);
        if (randomargetSwitcher != null)
            randomargetSwitcher.RemoveDestroyedItemsFromList();
    }
    /// <summary>
    /// NOS booster functionality for Car
    /// </summary>
    IEnumerator Turbo()
    {
        turboLocked = true;
        if (_turboCooldown < (Time.time - _lastTurbo) && _turbo == 1)
        {
            _lastTurbo = Time.time;
            _turboCooldown = turboDuration;
            _turbo = turboStrength;

            while (!PlayerManagerScript.instance.isEmpty)
            {
                yield return new WaitForSeconds(0.1f);
                PlayerManagerScript.instance.burn(totalNitroTime);//0.17f
            }

            _turbo = 1;
        }
        else
        {
            yield return new WaitForFixedUpdate();
        }
        turboLocked = false;
        nitroObj.SetActive(false);
    }

    /// <summary>
    /// Start Boost for car
    /// </summary>
    public void StartTurbo()
    {
        Debug.Log("Instance empty:" + PlayerManagerScript.instance.isEmpty + " " + "Turbo locked: "+ turboLocked);

        if (PlayerManagerScript.instance.isEmpty || turboLocked)
            return;
        // transform.Find("nitro").gameObject.SetActive(true);
        nitroObj.SetActive(true);
        //PlayerManagerScript.instance.nitroImage.SetActive (true);
        //PlayerManagerScript.instance.blurEffect.enabled = true;
        PlayerManagerScript.instance._RaceManager.GameCamereComponent.IsShakeCameraOnNitro = true;
        PlayerManagerScript.instance._animController.PlayBoostAniamtions();
        StartCoroutine("Turbo");

    }

    /// <summary>
    /// End Boost for Car
    /// </summary>
    public void EndTurbo()
    {
        if (_turbo != 1)
        {
            PlayerAudioManager.instance.NitroSource.Stop();
            StopCoroutine("Turbo");
            turboLocked = false;
            PlayerManagerScript.instance.BoostON = false;
            _turbo = 1;
            _turboCooldown = Time.time - _lastTurbo;
            nitroObj.SetActive(false);
          //  transform.Find("nitro").gameObject.SetActive(false);
            //			PlayerManagerScript.instance.blurEffect.enabled = false;

        }
    }
}
