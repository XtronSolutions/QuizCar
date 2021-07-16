using UnityEngine;
using System.Collections;

public class PowerUps : MonoBehaviour 
{
	private PowerUpManager powerUp;
    public GameObject BookObject;
	public bool isNitro, isShield, isWeapon;

	void Start()
	{
        //powerUp = transform.parent.parent.GetComponent<PowerUpManager> ();
        powerUp = GameObject.FindObjectOfType<PowerUpManager>();
    }

	void OnTriggerEnter(Collider _hit)
	{
		if (_hit.CompareTag ("Player")) 
		{
            //			if (isNitro) 
            //			{
            //				powerUp.ActivateSpecificPowerUp (PowerUpManager.powerUpType.NITRO);
            //			} 
            //			else if (isShield) 
            //			{
            //				powerUp.ActivateSpecificPowerUp (PowerUpManager.powerUpType.SHIELD);
            //			} 
            //			else if (isWeapon) 
            //			{
            //				powerUp.ActivateSpecificPowerUp (PowerUpManager.powerUpType.WEAPON);
            //			} 
            //			else 
            //			{
            //				powerUp.ActivatePowerUp ();
            //			}
            DisablePowerUp();
            StartCoroutine(RespawnPowerUps());
            if (Constants.isMultiplayerSelected)
            {
                if (!PlayerManagerScript.instance.isMine(_hit.transform.root.gameObject))
                    return;
            }

            //powerUp.SelectRandomPowerUp();
            GlobalVariables.isPause = true;
            Controls.IsHandBrake = true;
             powerUp.SelectRandomQuestion();
            _hit.transform.root.GetComponent<PlayerAudioManager> ().PlayPowerUpPickUp ();
            


        }

		if (_hit.CompareTag ("AI")) {
			DisablePowerUp ();
			StartCoroutine (RespawnPowerUps ());
			_hit.gameObject.GetComponentInParent<EnemyTrapsController> ().DeployTrap ();
		}

		if (_hit.CompareTag ("Enemy")) {
			DisablePowerUp ();
			StartCoroutine (RespawnPowerUps ());
		}
	}

	void DisablePowerUp(){
		this.GetComponent<MeshRenderer> ().enabled = false;
		this.GetComponent<BoxCollider> ().enabled = false;
        BookObject.SetActive(false);

        for (int i = 0; i < this.transform.childCount; i++) 
		{
			this.transform.GetChild (i).gameObject.SetActive (false);
		}
	}

	IEnumerator RespawnPowerUps()
	{
		yield return new WaitForSeconds (30);
        BookObject.SetActive(true);
        this.GetComponent<MeshRenderer> ().enabled = false;
		this.GetComponent<BoxCollider> ().enabled = true;
		for (int i = 0; i < this.transform.childCount; i++) 
		{
			this.transform.GetChild (i).gameObject.SetActive (true);
		}
	}
}
