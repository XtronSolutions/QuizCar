using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RacingGameKit;
public class Track : MonoBehaviour {

    public int Laps = 1;

    public GameObject wayPoints, spawnPoints, startPoint, finishPoint;


    public Transform powerUpContainer, coinsContainer;


    GameObject scene;
    Race_Manager race_Manager;
	// Use this for initialization
	void Start () {

     
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitPowerUpsAndCoins(GameObject scenePrefab, GameObject powerUps, GameObject coinPrefab)
    {

        scene = Instantiate(scenePrefab);

         // race_Manager = scene.GetComponent<MultiplayerSetup>().race_Manager;
        race_Manager = scene.GetComponentInChildren<Race_Manager>();
        race_Manager.Waypoints = wayPoints;
        race_Manager.SpawnPoints = spawnPoints;
        race_Manager.StartPoint = startPoint;
        race_Manager.FinishPoint = finishPoint;
        race_Manager.RaceLaps = Laps;

        Transform[] transforms = powerUpContainer.GetComponentsInChildren<Transform>();
        for (int i = 1; i < transforms.Length; i++)
        {
            Instantiate(powerUps, transforms[i].position, transforms[i].rotation);
        }



      //  Transform[] transforms2 = coinsContainer.GetComponentsInChildren<Transform>();
       // for (int i = 0; i < transforms2.Length; i++)
       // {
       //     Instantiate(coinPrefab, transforms2[i].position, transforms2[i].rotation);
       // }

        //commented for couins
        //Transform[] wps = wayPoints.GetComponentsInChildren<Transform>();
        //int coinDistance = 9;
        //for (int i = coinDistance; i < wps.Length; i += coinDistance)
        //{

        //    float x = wps[i].position.x;
        //    float y = wps[i].position.y + 2.05f;
        //    float z = wps[i].position.z;
        //    //  x = y = z = 0;

        //    Vector3 v = new Vector3(x, y, z);
        //    GameObject coin = Instantiate(coinPrefab, v, wps[i].rotation);

        //    v = new Vector3(x - 4, y, z);
        //    GameObject t = Instantiate(coinPrefab, v, wps[i].rotation, coin.transform);
        //    t.transform.localPosition = new Vector3(-4, 0, 0);
        //    t.transform.parent = null;

        //    t = Instantiate(coinPrefab, v, wps[i].rotation, coin.transform);
        //    t.transform.localPosition = new Vector3(-2, 0, 0);
        //    t.transform.parent = null;

        //    t = Instantiate(coinPrefab, v, wps[i].rotation, coin.transform);
        //    t.transform.localPosition = new Vector3(4, 0, 0);
        //    t.transform.parent = null;

        //    t = Instantiate(coinPrefab, v, wps[i].rotation, coin.transform);
        //    t.transform.localPosition = new Vector3(2, 0, 0);
        //    t.transform.parent = null;
        //}
            //commented for couins

            //v = new Vector3(x - 2, y, z);
            //Instantiate(coinPrefab, v, Quaternion.identity);



            //v = new Vector3(x + 2, y, z);
            //Instantiate(coinPrefab, v, Quaternion.identity);

            //v = new Vector3(x + 4, y, z);
      

    }
  
}