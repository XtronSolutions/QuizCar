using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehicleUpdateOnline : MonoBehaviour {

	[System.Serializable]
	public class UpgradeParts{
		public string name;
		public int levelRequired;
		public GameObject[] partsToEnable;
		public GameObject[] partsToDisable;
	}


	public vehicleHandling _vehicleHandling;
	public int Id =0;
	public UpgradeParts[] vehiclePartsToUpgrade;
	int vehicleUpgradeLevel;

	// Use this for initialization
	void Start () {


	}

	// Update is called once per frame
	void Update () {

	}

	public void SetupVehicle(int upgradeLevel){

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
			select v).First ();

		foreach (var pe in vehiclePartToUpgrade.partsToEnable) {
			pe.SetActive (true);
		}

		foreach (var pd in vehiclePartToUpgrade.partsToDisable) {
			pd.SetActive (false);
		}
	}
}
