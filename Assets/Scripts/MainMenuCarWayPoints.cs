using UnityEngine;
using System.Collections;

public class MainMenuCarWayPoints : MonoBehaviour 
{
	[Header("Variables")]
	public Transform nextTarget;
	public float startDelay;
	public int dir;
	[Space]
	public BC_AI_NavMeshPathCalculator pathFinder;

	public mBuggies[] Cars;
	public Transform PosOne;
	public Transform PosTwo;
	public GameObject Main_Menu;

	Vector3 carPos;
	Quaternion carRot;

	void Start()
	{
		
	}

	void OnTriggerEnter(Collider _hit)
	{
		if (_hit.tag.Equals ("Player")) 
		{
//			Debug.LogError ("Player Reached");
			_hit.transform.parent.Rotate (0, -180, 0);
			if (this.name == "WayPoint") {
				carPos = PosOne.position;
				carRot = PosOne.rotation;
				
			} else {
				carPos = PosTwo.position;
				carRot = PosTwo.rotation;
			}

			Destroy (_hit.transform.parent.gameObject);
			StartCoroutine (InstantiateAfterDelay (startDelay));
		}
	}

	IEnumerator InstantiateAfterDelay(float time)
	{
		yield return new WaitForSeconds (time);
		mBuggies selectedCar = Cars [Random.Range (0, Cars.Length)];
		GameObject car = (GameObject)Instantiate (selectedCar.BuggyModel, carPos, carRot);
		car.GetComponent<BC_AI_NavMeshPathCalculator> ().target = nextTarget;
		car.transform.Find("buggy meshes").Find("body").GetComponent<Renderer>().material.mainTexture =selectedCar.BuggyTextures[Random.Range(0,selectedCar.BuggyTextures.Length)];
//		if (!Main_Menu.activeSelf)
//			car.GetComponent<AudioSource> ().enabled = false;
	}
}
