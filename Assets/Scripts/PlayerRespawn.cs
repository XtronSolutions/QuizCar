using UnityEngine;
using System.Collections;
using RacingGameKit;
using Photon.Pun;
public class PlayerRespawn : MonoBehaviour 
{
	private GameObject Car;
	private vehicleHandling carHandling;
	public Racer_Register racerRegister;
	public  float respawnTime =1f;
	private float wrongWayRespawnTimer;

	public bool stopRespawning = false;

	void Start()
	{
		wrongWayRespawnTimer = respawnTime;
		Car = PlayerManagerScript.instance.Car;
		carHandling = Car.GetComponent<vehicleHandling>();
		racerRegister = Car.GetComponent<Racer_Register> ();
	}

	void Update()
	{
		if (stopRespawning)
			return;

		if (carHandling.carStuck) 
		{

            RespawnPlayer();

            racerRegister.SetCurrentIndexOnRespawn();

          //  Debug.Log ("Car Stuck");

			carHandling.carStuck = false;

            if(Constants.isMultiplayerSelected)
            carHandling.gameObject.GetComponent<PhotonView>().RPC("Respawned", RpcTarget.Others, null);

        }

		if ((racerRegister.RacerDetail.RacerWrongWay || racerRegister.RacerDetail.RacerOutOfBound ) &&  !racerRegister.IsRacerFinished)
		{
			wrongWayRespawnTimer -= Time.deltaTime;
			if (wrongWayRespawnTimer < 0) 
			{
				wrongWayRespawnTimer = respawnTime;
				RespawnPlayer ();
				racerRegister.RacerDetail.RacerWrongWay = false;
				racerRegister.RacerDetail.RacerOutOfBound = false;
				racerRegister.SetCurrentIndexOnRespawn ();
     
			}
		}
		if(!racerRegister.RacerDetail.RacerWrongWay && !racerRegister.RacerDetail.RacerOutOfBound)
		{
			wrongWayRespawnTimer = respawnTime;
		}
	}

	public void RespawnPlayer()
	{
		//Debug.Log ("RespawnPlayer");
		Car.GetComponent<Rigidbody> ().isKinematic = true;
		Car.GetComponent<Rigidbody> ().isKinematic = false;
		carHandling.vehicleSpeed = 0;

        if(racerRegister.ClosestDP!=null)
        {
            Vector3 v = new Vector3(racerRegister.ClosestDP.position.x, racerRegister.ClosestDP.position.y + 2, racerRegister.ClosestDP.position.z);
            Car.transform.position = v;
            Car.transform.rotation = racerRegister.ClosestDP.rotation;
        }

		StartCoroutine (BlinkEffect());
	}

	IEnumerator BlinkEffect(){
		var meshRenderers = Car.GetComponentsInChildren<MeshRenderer> ();
		var skinRenderers = Car.GetComponentsInChildren<SkinnedMeshRenderer> ();

		ShowRenderers (meshRenderers, skinRenderers, false);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, true);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, false);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, true);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, false);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, true);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, false);
		yield return new WaitForSeconds (0.2f);
		ShowRenderers (meshRenderers, skinRenderers, true);
	}

	void ShowRenderers(MeshRenderer[] meshes, SkinnedMeshRenderer[] skins, bool enable){
		foreach (var r in meshes) {
			r.enabled = enable;
		}

		foreach (var s in skins) {
			s.enabled = enable;
		}
	}
}
