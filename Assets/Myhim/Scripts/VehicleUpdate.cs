using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehicleUpdate : MonoBehaviour {

	[System.Serializable]
	public class UpgradeParts{
		public string name;
		public int levelRequired;
       
		public GameObject[] partsToEnable;
		public GameObject[] partsToDisable;
	}
    public AudioClip []sounds;

    public vehicleHandling _vehicleHandling;
	public int Id =0;
	public UpgradeParts[] vehiclePartsToUpgrade;
    int vehicleUpgradeLevel;

    // Use this for initialization
    void Start () {

        if (!Constants.isMultiplayerSelected)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = sounds[RewardProperties.Instance.GetBuggyUpgrade(Id)];
            GetComponent<AudioSource>().Play();
            Invoke("setData", 0.5f);
        }
	}
	void setData()
    {
        var vehicleUpgradeLevel = RewardProperties.Instance.GetBuggyUpgrade(Id);

        float speed1 = _vehicleHandling.MAXvehicleSpeed;
        float increment1 = (speed1 * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.MAXvehicleSpeed += increment1;

        float acc = _vehicleHandling.maximumAcceleration;
        float incrementAcc1 = (acc * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.maximumAcceleration += incrementAcc1;

        float Steering = _vehicleHandling.standardSteeringFactor;
        float decrement = (Steering * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.standardSteeringFactor -= decrement;

     //   Debug.Log(Id + "  " + vehicleUpgradeLevel);

        //  var vehiclePartToUpgrade = (from v in vehiclePartsToUpgrade
        //                            where v.levelRequired == vehicleUpgradeLevel
        //                            select v).First();

        if (vehicleUpgradeLevel > 0)
        {
            var vehiclePartToUpgrade = vehiclePartsToUpgrade[vehicleUpgradeLevel - 1];



            foreach (var pe in vehiclePartToUpgrade.partsToEnable)
            {
                pe.SetActive(true);
            }

            foreach (var pd in vehiclePartToUpgrade.partsToDisable)
            {
                pd.SetActive(false);
            }
        }
       
    }
	// Update is called once per frame
	void Update () {
		
	}

    public void SetupVehicle(int upgradeLevel)
    {

        vehicleUpgradeLevel = upgradeLevel;

        float speed1 = _vehicleHandling.MAXvehicleSpeed;
        float increment1 = (speed1 * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.MAXvehicleSpeed += increment1;

        float acc = _vehicleHandling.maximumAcceleration;
        float incrementAcc1 = (acc * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.maximumAcceleration += incrementAcc1;

        float Steering = _vehicleHandling.standardSteeringFactor;
        float decrement = (Steering * 10 * vehicleUpgradeLevel) / 100;
        _vehicleHandling.standardSteeringFactor -= decrement;

        var vehiclePartToUpgrade = (from v in vehiclePartsToUpgrade
                                    where v.levelRequired == vehicleUpgradeLevel
                                    select v).First();

        foreach (var pe in vehiclePartToUpgrade.partsToEnable)
        {
            pe.SetActive(true);
        }

        foreach (var pd in vehiclePartToUpgrade.partsToDisable)
        {
            pd.SetActive(false);
        }
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = sounds[vehicleUpgradeLevel];
        GetComponent<AudioSource>().Play();
    }
}
