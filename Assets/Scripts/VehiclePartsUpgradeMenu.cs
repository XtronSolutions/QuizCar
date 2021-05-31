using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehiclePartsUpgradeMenu : MonoBehaviour {

	public int Id;
	public VehicleUpdate.UpgradeParts[] vehiclePartsToUpgrade;

	// Use this for initialization
	void Start () {
       // Invoke("init", 1f);
        UpgradeParts(RewardProperties.Instance.GetBuggyUpgrade(Id));
    }
	public void init()
    {
        UpgradeParts(RewardProperties.Instance.GetBuggyUpgrade(Id));
    }
	// Update is called once per frame
	void Update () {
		
	}

	public void UpgradeParts(int vehicleUpgradeLevel){

       // Debug.Log(vehicleUpgradeLevel);
        if (vehicleUpgradeLevel == 0)
        {
            for (int i = 0; i < vehiclePartsToUpgrade[3].partsToDisable.Length; i++)
            {
                vehiclePartsToUpgrade[3].partsToDisable[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < vehiclePartsToUpgrade[3].partsToEnable.Length; i++)
            {
                vehiclePartsToUpgrade[3].partsToEnable[i].gameObject.SetActive(false);
            }
            return;
        }


        var vehiclePartToUpgrade = (from v in vehiclePartsToUpgrade
			where v.levelRequired == vehicleUpgradeLevel
			select v).FirstOrDefault ();

		if (vehiclePartToUpgrade == null) {
			return;
		}

		foreach (var pe in vehiclePartToUpgrade.partsToEnable) {
			pe.SetActive (true);
		}

		foreach (var pd in vehiclePartToUpgrade.partsToDisable) {
			pd.SetActive (false);
		}

       
	}
}
