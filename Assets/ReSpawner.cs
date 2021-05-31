using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReSpawner : MonoBehaviour {

	bool RespawnCheck;
	bool isInWater;
void OnCollisionEnter(Collision coll){
		Debug.Log ("OnCollisionEnter"+coll.collider.tag);
		if (coll.collider.tag.Equals ("RaceCar")) {
			isInWater =true;

		}
	}

	void OnCollisionStay(Collision coll){
		Debug.Log ("OnCollisionStay"+coll.collider.tag);
		if (coll.collider.tag.Equals ("RaceCar")) {
			isInWater =true;
			if(!RespawnCheck)
			StartCoroutine(WaitForRespawn(3f));
		}
	}

	void OnCollisionExit(Collision coll){
		Debug.Log ("OnCollisionExit"+coll.collider.tag);
		if (coll.collider.tag.Equals ("RaceCar")) {
			isInWater =false;
			StopCoroutine("WaitForRespawn");
		}
	}

	IEnumerator WaitForRespawn(float t){
		RespawnCheck = true;
		yield return new WaitForSeconds (t);
		RespawnCheck = false;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}


}
