using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RG_Waypoint : MonoBehaviour {


	private RG_SceneManager sceneManager;
	public int waypointNumber;

	void Start () {
		sceneManager = this.transform.parent.GetComponent<RG_SceneManager>();
	}

	void OnTriggerEnter (Collider col) {
		for(int i = 0; i < sceneManager.raceData.numberOfRacers[sceneManager.raceData.raceNumber]; i++){
			if (i == 0) {
				if (col.transform.parent.name == sceneManager.playableVehicles.playerName) {
					sceneManager.ChangeTarget (0, waypointNumber);
					//Debug.Log ("Player waypoint");

				}
				//break;
			} else {
				//Debug.Log (i);
				if (col.transform.parent.name == sceneManager.opponentVehicles.opponentNames[i - 1]) {
					sceneManager.ChangeTarget (i, waypointNumber);
					//Debug.Log ("Opponent " + i.ToString() + " waypoint");

				}
				//break;
			}
		}


	//	if(col.gameObject.tag == "Player"){
			
			//gameObject.SetActive(false);
	//	}
	//	else if(col.gameObject.tag == "AI 1"){
	//		sceneManager.ChangeTarget(1, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 2"){
	//		sceneManager.ChangeTarget(2, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 3"){
	//		sceneManager.ChangeTarget(3, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 4"){
	//		sceneManager.ChangeTarget(4, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 5"){
	//		sceneManager.ChangeTarget(5, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 6"){
	//		sceneManager.ChangeTarget(6, waypointNumber);
	//	}
	//	else if(col.gameObject.tag == "AI 7"){
	//		sceneManager.ChangeTarget(7, waypointNumber);
	//	}
	}
	
	//void OnDrawGizmosSelected(){
	//	gameManager.OnDrawGizmosSelected();
	//}

}