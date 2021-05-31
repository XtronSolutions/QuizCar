using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SpawnManager : MonoBehaviour
{
    
    public HudScript HUD;
    public GameObject PlayerCar;
    public GameObject[] AI_Cars;
    public GameObject[] spawnPoints;
    List<GameObject> TakenSpawnPoints;
    public SmoothFollow CameraFollow;
    public int MaxAICarsToSpawn = 3;
    // Use this for initialization
    IEnumerator Start()
    {
        //Time.timeScale = 0.2f;
        TakenSpawnPoints = new List<GameObject>();

        int SpawnPointIndex = Random.Range(0, spawnPoints.Length - 1);
        GameObject randomSpawnPoint = spawnPoints[SpawnPointIndex];
        TakenSpawnPoints.Add(randomSpawnPoint);
        GameObject car = Instantiate(PlayerCar, randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation) as GameObject;

        Car_Weapon weapon = car.GetComponentInChildren<Car_Weapon>();
        vehicleHandling PlayerCar_Handler = car.GetComponent<vehicleHandling>();
        PlayerCar_Handler._AccelerationInput = 1;
        HUD.weaponScript = weapon;
        HUD.PlayerCar = PlayerCar_Handler;
        

        CameraFollow.lookTarget = car.transform;
        for (int i = 0; i < Constants.AI_PLAYERS_IN_BATTLE; i++)
        {
            SpawnPointIndex = Random.Range(0, spawnPoints.Length - 1);
            randomSpawnPoint = spawnPoints[SpawnPointIndex];
            while (TakenSpawnPoints.Contains(randomSpawnPoint))
            {
                SpawnPointIndex = Random.Range(0, spawnPoints.Length - 1);
                randomSpawnPoint = spawnPoints[SpawnPointIndex];
                yield return null;
            }
            TakenSpawnPoints.Add(randomSpawnPoint);
            int AI_Car_index = Random.Range(0, AI_Cars.Length - 1);
            car = Instantiate(AI_Cars[AI_Car_index], randomSpawnPoint.transform.position, randomSpawnPoint.transform.rotation) as GameObject;
        }
        yield return null;
         
    }
    private void AddEventTrigger(UnityAction action, EventTriggerType triggerType,EventTrigger eventTrigger)
    {

       
        // Create a nee TriggerEvent and add a listener
        EventTrigger.TriggerEvent trigger = new EventTrigger.TriggerEvent();
        trigger.AddListener((eventData) => action()); // ignore event data
         
        // Create and initialise EventTrigger.Entry using the created TriggerEvent
        EventTrigger.Entry entry = new EventTrigger.Entry() { callback = trigger, eventID = triggerType };

        // Add the EventTrigger.Entry to delegates list on the EventTrigger
        eventTrigger.triggers.Add(entry);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void RespawnCar(GameObject car)
    {
        int SpawnPointIndex = Random.Range(0, spawnPoints.Length - 1);
        GameObject randomSpawnPoint = spawnPoints[SpawnPointIndex];
        car.transform.position = randomSpawnPoint.transform.position;
        car.transform.rotation = randomSpawnPoint.transform.rotation;

        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }
}
