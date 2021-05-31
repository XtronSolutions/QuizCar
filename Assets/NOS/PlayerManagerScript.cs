using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using RacingGameKit;
public class PlayerManagerScript : MonoBehaviour 
{
	public static PlayerManagerScript instance { get; private set;}
	public vehicleHandling _vehicleHandling{ get { return Car.GetComponent<vehicleHandling> (); } }
	public RacingGameKit.Race_Manager _RaceManager;
	public CharacterAnimationController _animController;
	public GameObject Car, NitroButton;
	private Rigidbody IceRb;
	public AudioSource NitroSource;
	public AudioSource source;
	public AudioClip oilClip;
	public AudioClip iceClip;
	public AudioClip rocketClip;
	public AudioClip trapClip;

	
	//[HideInInspector]
	//public UnityStandardAssets.ImageEffects.MotionBlur blurEffect;
	
	[HideInInspector]
	public bool BoostON = false;
	void Awake()
	{
//		if (instance == null)
			instance = this;
//		else
//			Destroy (gameObject);
	}
	
	private float _Nitro = 0;
	Vector3 buttonScale;
	void OnEnable()
	{
		if(NitroButton!=null)
			buttonScale = NitroButton.transform.localScale;
//		GameObject BlurEffectGO = GameObject.Find("_GameCamera").transform.FindChild("Camera").gameObject;
//		if (BlurEffectGO != null)
//		{
//			blurEffect = BlurEffectGO.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>();
//		}
		//Car = GameObject.Find("CustomCar(Clone)"); // commented on 17-04-2019
		//Car = GameObject.Find("CustomCarDummyNewBuggy2(Clone)");
		//Car = GameObject.Find("CustomCarDummyNew(Clone)");
		//		_animController = Car.transform.FindChild("Character").GetComponent<CharacterAnimationController> ();

		//Debug.LogError(RewardProperties.Instance.GetBuggyUpgrade(0));

//		if (Car != null) {
//
//			if (RewardProperties.Instance.GetBuggyUpgrade (0) == 1) {
//			
//				float speed1 = _vehicleHandling.MAXvehicleSpeed;
//				float increment1 = (speed1 * 10) / 100;
//				_vehicleHandling.MAXvehicleSpeed += increment1;
//
//				float acc = _vehicleHandling.maximumAcceleration;
//				float incrementAcc1 = (acc * 10) / 100;
//				_vehicleHandling.maximumAcceleration += incrementAcc1;
//
//				float Steering = _vehicleHandling.standardSteeringFactor;
//				float decrement = (Steering * 10) / 100;
//				_vehicleHandling.standardSteeringFactor -= decrement;
//
//			} 
//			else if (RewardProperties.Instance.GetBuggyUpgrade (0) == 2) {
//			
//				float speed2 = _vehicleHandling.MAXvehicleSpeed;
//				float increment2 = (speed2 * 20) / 100;
//				_vehicleHandling.MAXvehicleSpeed += increment2;
//
//				float acc = _vehicleHandling.maximumAcceleration;
//				float incrementAcc1 = (acc * 20) / 100;
//				_vehicleHandling.maximumAcceleration += incrementAcc1;
//
//				float Steering = _vehicleHandling.standardSteeringFactor;
//				float decrement = (Steering * 20) / 100;
//				_vehicleHandling.standardSteeringFactor -= decrement;
//			}
//			else if (RewardProperties.Instance.GetBuggyUpgrade (0) == 3) {
//
//				float speed3 = _vehicleHandling.MAXvehicleSpeed;
//				float increment3 = (speed3 * 30) / 100;
//				_vehicleHandling.MAXvehicleSpeed += increment3;
//
//				float acc = _vehicleHandling.maximumAcceleration;
//				float incrementAcc1 = (acc * 30) / 100;
//				_vehicleHandling.maximumAcceleration += incrementAcc1;
//
//				float Steering = _vehicleHandling.standardSteeringFactor;
//				float decrement = (Steering * 30) / 100;
//				_vehicleHandling.standardSteeringFactor -= decrement;
//			}
//			else if (RewardProperties.Instance.GetBuggyUpgrade (0) == 4) {
//
//				float speed4 = _vehicleHandling.MAXvehicleSpeed;
//				float increment4 = (speed4 * 40) / 100;
//				_vehicleHandling.MAXvehicleSpeed += increment4;
//
//				float acc = _vehicleHandling.maximumAcceleration;
//				float incrementAcc1 = (acc * 40) / 100;
//				_vehicleHandling.maximumAcceleration += incrementAcc1;
//
//				float Steering = _vehicleHandling.standardSteeringFactor;
//				float decrement = (Steering * 40) / 100;
//				_vehicleHandling.standardSteeringFactor -= decrement;
//			}
//		}
	}
	
	void Start()
	{
        //		GameObject BlurEffectGO = GameObject.Find("_GameCamera").transform.FindChild("Camera").gameObject;
        //		if (BlurEffectGO != null)
        //		{
        //			blurEffect = BlurEffectGO.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>();
        //		}

        // commented on 17-04-2019
        //		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("MainMenuScene")) {
        //			if (GameObject.Find ("CustomCar(Clone)") != null)
        //				Car = GameObject.Find ("CustomCar(Clone)");
        //
        //			if (Car.transform.Find ("Character").GetComponent<CharacterAnimationController> () != null)
        //				_animController = Car.transform.Find ("Character").GetComponent<CharacterAnimationController> ();
        //		}
        //Actual
        /*
		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("MainMenuScene")) {
			if (GameObject.Find ("CustomCarDummyNew(Clone)") != null)
				Car = GameObject.Find ("CustomCarDummyNew(Clone)");
			if (Constants.isMultiplayerSelected) {
				if (GameObject.Find ("CustomCarDummyNewOnline(Clone)") != null)
					Car = GameObject.Find ("CustomCarDummyNewOnline(Clone)");
			}
           
//		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("MainMenuScene")) {
//			if (GameObject.Find ("Model_Misc_Buggy_Dummy(Clone)") != null)
//				Car = GameObject.Find ("Model_Misc_Buggy_Dummy(Clone)");
			



			if (Car.transform.Find ("Character").GetComponent<CharacterAnimationController> () != null)
				_animController = Car.transform.Find ("Character").GetComponent<CharacterAnimationController> ();
		//} */
        //actual end

        //		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("MainMenuScene")) {
        //			if (GameObject.Find ("CustomCarDummyNewBuggy2(Clone)") != null)
        //				Car = GameObject.Find ("CustomCarDummyNewBuggy2(Clone)");
        //			if (Constants.isMultiplayerSelected) {
        //				if (GameObject.Find ("CustomCarDummyNewOnline(Clone)") != null)
        //					Car = GameObject.Find ("CustomCarDummyNewOnline(Clone)");
        //			}
        //
        //			//		if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name.Equals ("MainMenuScene")) {
        //			//			if (GameObject.Find ("Model_Misc_Buggy_Dummy(Clone)") != null)
        //			//				Car = GameObject.Find ("Model_Misc_Buggy_Dummy(Clone)");
        //
        //
        //			if (Car.transform.Find ("Character").GetComponent<CharacterAnimationController> () != null)
        //				_animController = Car.transform.Find ("Character").GetComponent<CharacterAnimationController> ();
        //		}
        SetCar();
    }

    public bool isMine(GameObject g)
    {
        if (Constants.isMultiplayerSelected)
        {
            if (g.GetInstanceID() == Car.GetInstanceID())
                return true;
            else
                return false;
                //return Car.GetComponent<PhotonView>().IsMine;
        }
        else
           return true;
    }
    void SetCar()
{
        Car = GameObject.FindObjectOfType<Race_Manager>().racer;
        if (Car != null) { 
        if (Car.transform.Find("Character").GetComponent<CharacterAnimationController>() != null)
            _animController = Car.transform.Find("Character").GetComponent<CharacterAnimationController>();
        }
    }
	Vector3 NormalVector = new Vector3 (0,0,0);
	public void DeployTrap(){
	
		Vector3 Pos = Car.transform.position;
		Quaternion Rot = Car.transform.rotation;
        // Pos.y -= 0.3f;
        Pos.y = GetY();
        Rot = trapRot;

        int selectedPowerUp = PowerUpManager.Instance.selectedPowerUp;
		switch (selectedPowerUp) {
		case 0:
			GameObject bomb;
			if (Constants.isMultiplayerSelected) {
				bomb = PhotonNetwork.Instantiate ("Traps/BombTrigger", Pos, Rot, 0,
					new object[]{ Car.GetComponent<PhotonView> ().ViewID }) as GameObject;
			} else {
				bomb = Instantiate (Resources.Load ("Traps/BombTrigger"), Pos, Rot) as GameObject;
			}
			bomb.GetComponent<BombTrigger> ().isDeployedByPlayer = true;
			break;

		case 1:
			var iceSpawnTransform = Car.transform.Find ("IcePosition");
			GameObject Trap;
                Quaternion rot2 = iceSpawnTransform.rotation;
                rot2.eulerAngles = new Vector3(0, rot2.eulerAngles.y, rot2.eulerAngles.z);
                if (Constants.isMultiplayerSelected) {
				Trap = PhotonNetwork.Instantiate ("Traps/IceShoot", 
					iceSpawnTransform.position, rot2) as GameObject;
			} else {
				Trap = Instantiate (Resources.Load ("Traps/IceShoot"), 
					iceSpawnTransform.position, rot2) as GameObject;
			}
			Trap.GetComponent<Rigidbody> ().velocity = Car.GetComponent<Rigidbody> ().velocity;
			source.PlayOneShot (iceClip);
			break;

		case 2:
			PowerUpManager.Instance.NitroPressed ();
			PlayerAudioManager.instance.PlayNitroSound ();
			break;

		case 3:
			GameObject trap;
			if (Constants.isMultiplayerSelected) {
				trap = PhotonNetwork.Instantiate ("Traps/Trap_Anim", Pos, Rot, 0,
					new object[]{ Car.GetComponent<PhotonView> ().ViewID }) as GameObject;
			} else {
				trap = Instantiate (Resources.Load ("Traps/Trap_Anim"), Pos, Rot) as GameObject;
			}
			trap.GetComponentInChildren<TrapCollisionDetection> ().isDeployedByPlayer = true;
			source.PlayOneShot (trapClip);
			break;

		case 4:
			GameObject oil;
			if (Constants.isMultiplayerSelected) {
				oil = PhotonNetwork.Instantiate ("Traps/OilSpillTrigger", Pos, Rot, 0,
					new object[]{ Car.GetComponent<PhotonView> ().ViewID }) as GameObject;
			} else {
				oil = Instantiate (Resources.Load ("Traps/OilSpillTrigger"), Pos, Rot) as GameObject;
			}
			oil.GetComponent<OilSpillTriggerController> ().isDeployedByPlayer = true;
			source.PlayOneShot (oilClip);
			break;

		case 5:
			var rocketSpawnTransform = Car.transform.Find ("IcePosition");
			GameObject rocket;
                Quaternion rot = rocketSpawnTransform.rotation;
                rot.eulerAngles = new Vector3(0, rot.eulerAngles.y, rot.eulerAngles.z);

            if (Constants.isMultiplayerSelected) {
				rocket = PhotonNetwork.Instantiate ("Traps/RocketShoot", 
					rocketSpawnTransform.position, rot) as GameObject;
			} else {
				rocket = Instantiate (Resources.Load ("Traps/RocketShoot"), 
					rocketSpawnTransform.position, rot) as GameObject;
			}
			rocket.GetComponent<Rigidbody> ().velocity = Car.GetComponent<Rigidbody> ().velocity;
			source.PlayOneShot (rocketClip);
			break;
		}

		PowerUpManager.Instance.deployButton.gameObject.SetActive (false);

	}
	private float speed = 0f;
	void ShootIceCube(GameObject Ice){
	
		speed = _vehicleHandling._VehicleSpeed;
		speed += 500f;
		value = true;

//		IceRb.AddForce (transform.forward * 5000 * -1);
	}

	public bool value = false;
	void Update(){
	
		if (value) {
			
			if (IceRb) {
		

				IceRb.AddForce (Vector3.left * speed);
			}
		}

	}

	
	public void addNitro (int _val)
	{
		_Nitro += _val;
		_Nitro = _Nitro > 1 ? 1 : _Nitro;
	}
	
	public float getNitro ()
	{
		return _Nitro;
	}
	
	public void burn (float delta)
	{   
		if (!BoostON)
			BoostON = true;
		_Nitro -= _Nitro*delta;
		//		NitroButton.transform.localScale = new Vector3 (buttonScale.x + 0.05f, buttonScale.y+ 0.05f, buttonScale.z + 0.05f);
		//		StartCoroutine (Buttonanimation());
		if (isEmpty) 
		{
			_Nitro = 0;
			BoostON = false;
			_vehicleHandling.EndTurbo ();
			NitroButton.gameObject.GetComponent<BoosterScript> ().OnEmptyNitro ();
		}
	}
	
	IEnumerator Buttonanimation()
	{
		yield return new WaitForEndOfFrame ();
		NitroButton.transform.localScale = new Vector3 (buttonScale.x , buttonScale.y , buttonScale.z );
	}
	public bool isEmpty {
		get { return _Nitro < 0.01f; }
	}



    float GetY()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 <<9;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Car.transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
        {
           // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
            Debug.Log("Ray Angle " + hit.normal);
            //  trapRot  = Quaternion.LookRotation(hit.normal);

            trapRot =Car.transform.rotation;
            trapRot.eulerAngles = new Vector3(0, trapRot.eulerAngles.y, 0);
            return Car.transform.position.y - hit.distance+0.15f;
        }
        else
            return Car.transform.position.y - 0.3f;

    }
    Quaternion trapRot;
    public void DeployTrapFromHud(int i)
    {
       // i = 5;
        Vector3 Pos = Car.transform.position;
        Quaternion Rot = Car.transform.rotation;
     
        // Pos.y -= 0.3f;
        Pos.y = GetY();
        Rot = trapRot;
        Debug.Log("Car Angle " + Car.transform.eulerAngles);
        int selectedPowerUp = i;
        switch (selectedPowerUp)
        {
            case 0:
                GameObject bomb;
                if (Constants.isMultiplayerSelected)
                {
                    bomb = PhotonNetwork.Instantiate("Traps/BombTrigger", Pos, Rot, 0,
                        new object[] { Car.GetComponent<PhotonView>().ViewID }) as GameObject;
                }
                else
                {
                    bomb = Instantiate(Resources.Load("Traps/BombTrigger"), Pos, Rot) as GameObject;
                }
                bomb.GetComponent<BombTrigger>().isDeployedByPlayer = true;
                break;

            case 1:
                var iceSpawnTransform = Car.transform.Find("IcePosition");
                GameObject Trap;
                if (Constants.isMultiplayerSelected)
                {
                    Trap = PhotonNetwork.Instantiate("Traps/IceShoot",
                        iceSpawnTransform.position, iceSpawnTransform.rotation) as GameObject;
                }
                else
                {
                    Trap = Instantiate(Resources.Load("Traps/IceShoot"),
                        iceSpawnTransform.position, iceSpawnTransform.rotation) as GameObject;
                }
                Trap.GetComponent<Rigidbody>().velocity = Car.GetComponent<Rigidbody>().velocity;
                source.PlayOneShot(iceClip);
                break;

            case 2:
                PowerUpManager.Instance.NitroPressed();
                PlayerAudioManager.instance.PlayNitroSound();
                break;

            case 3:
                GameObject trap;
                if (Constants.isMultiplayerSelected)
                {
                    trap = PhotonNetwork.Instantiate("Traps/Trap_Anim", Pos, Rot, 0,
                        new object[] { Car.GetComponent<PhotonView>().ViewID }) as GameObject;
                }
                else
                {
                    trap = Instantiate(Resources.Load("Traps/Trap_Anim"), Pos, Rot) as GameObject;
                }
                trap.GetComponentInChildren<TrapCollisionDetection>().isDeployedByPlayer = true;
                source.PlayOneShot(trapClip);
                break;

            case 4:
                GameObject oil;
                if (Constants.isMultiplayerSelected)
                {
                    oil = PhotonNetwork.Instantiate("Traps/OilSpillTrigger", Pos, Rot, 0,
                        new object[] { Car.GetComponent<PhotonView>().ViewID }) as GameObject;
                }
                else
                {
                    oil = Instantiate(Resources.Load("Traps/OilSpillTrigger"), Pos, Rot) as GameObject;
                }
                oil.GetComponent<OilSpillTriggerController>().isDeployedByPlayer = true;
                source.PlayOneShot(oilClip);
                break;

            case 5:
                var rocketSpawnTransform = Car.transform.Find("IcePosition");
                GameObject rocket;
                if (Constants.isMultiplayerSelected)
                {
                    rocket = PhotonNetwork.Instantiate("Traps/RocketShoot",
                        rocketSpawnTransform.position, rocketSpawnTransform.rotation) as GameObject;
                }
                else
                {
                    rocket = Instantiate(Resources.Load("Traps/RocketShoot"),
                        rocketSpawnTransform.position, rocketSpawnTransform.rotation) as GameObject;
                }
                rocket.GetComponent<Rigidbody>().velocity = Car.GetComponent<Rigidbody>().velocity;
                source.PlayOneShot(rocketClip);
                break;
        }

      

    }
}
