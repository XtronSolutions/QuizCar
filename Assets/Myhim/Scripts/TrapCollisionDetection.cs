using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Exploder.Utils;
using RacingGameKit;

public class TrapCollisionDetection : MonoBehaviour {

	public Sprite icon;
	public bool isDeployedByPlayer = false;
	public TrapTriggerController TrapTrigger;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.CompareTag ("Player") || collision.gameObject.CompareTag ("Enemy")) {
		
			collision.transform.root.GetComponent<PlayerAudioManager>().PlayTrapSound ();
		}

		if (collision.gameObject.CompareTag ("Player") || collision.gameObject.CompareTag ("AI") 
			|| collision.gameObject.CompareTag ("Enemy")) {
			ExploderSingleton.Instance.ExplodeObject (this.gameObject);

			if ((collision.gameObject.CompareTag ("AI")) 
				&& isDeployedByPlayer) {
				TrapDetailManager.Instance.ShowTrapDetail(icon, 
					collision.transform.root.GetComponent<Racer_Register>().Avatar);
			}

			if (Constants.isMultiplayerSelected && collision.gameObject.CompareTag ("Player")) {
				this.transform.root.GetComponent<TrapSpawnerOnline> ().CollisionDetected (PlayfabManager.PlayerID);
			}
		}
	}
}
