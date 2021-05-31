using UnityEngine;
using System.Collections;

public class BoundScript : MonoBehaviour 
{
	public PlayerRespawn respawnManager;

	void Start()
	{
		respawnManager = GameObject.Find ("_RaceManager").GetComponent<PlayerRespawn> ();
	
	}

	void OnTriggerEnter(Collider _hit)
	{
		if (_hit.CompareTag ("Player")) 
		{
            if (PlayerManagerScript.instance.isMine(_hit.transform.root.gameObject))
            {
                //wrongWayText.SetActive (true);
                respawnManager.racerRegister.RacerDetail.RacerOutOfBound = true;
            }
			//StartCoroutine (Respawn ());
		}
	}

//	IEnumerator Respawn()
//	{
//		yield return new WaitForSeconds (0.5f);
//		respawnManager.RespawnPlayer ();
//		Debug.Log ("Respawning: Out Of Bound");
//	}
}
