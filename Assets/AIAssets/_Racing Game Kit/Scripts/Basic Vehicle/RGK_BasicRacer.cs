//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Human Racer Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit;
using RacingGameKit.Helpers;
using RacingGameKit.Interfaces;


[AddComponentMenu("Racing Game Kit/Basic Car/RGK Basic Racer")]
public class RGK_BasicRacer : MonoBehaviour
{

    private Race_Manager raceManager;
    private Racer_Register gameRegister;
    private RGK_BasicCar RGKVehicle;


    public float FlipResetTimer = 0f;
    public float FlipResetWait = 2f;



    void Start()
    {

        GameObject GameManagerContainerGameObject = GameObject.Find("_RaceManager");
        raceManager = GameManagerContainerGameObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
        gameRegister = (Racer_Register)transform.GetComponent(typeof(Racer_Register));
        RGKVehicle = (RGK_BasicCar)transform.GetComponent(typeof(RGK_BasicCar));
    }

    void Update()
    {

        //if (raceManager == null || gameRegister == null)
        //{
        //    Debug.LogWarning(RGKMessages.RaceManagerMissing);
        //    return;
        //}

        GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 250;

        float throttle = 0f;

        //if (RGKVehicle.Gear == 0)
        //{
        //    throttle = Input.GetAxis("Vertical");
        //}
        //else
        //{

        //}
        throttle = Input.GetAxis("Vertical");
        if (raceManager !=null && gameRegister!=null)
        {
            if (raceManager.IsRaceStarted && !gameRegister.IsRacerFinished)
            {
                RGKVehicle.ShiftGears();

                RGKVehicle.ApplySteer(RGKVehicle.MaxSteer * Input.GetAxis("Horizontal"));
                if (throttle >= 0)
                {
                    RGKVehicle.ApplyDrive(throttle, 0, Input.GetKey(KeyCode.Space));
                }
                else if (throttle < 0)
                {
                    RGKVehicle.ApplyDrive(0, throttle, Input.GetKey(KeyCode.Space));
                }
            }
            else
            {
                RGKVehicle.ApplyDrive(0, 0, true);
            }
        }
        else //freedrive
        {
            RGKVehicle.ShiftGears();

            RGKVehicle.ApplySteer(RGKVehicle.MaxSteer * Input.GetAxis("Horizontal"));
            if (throttle >= 0)
            {
                RGKVehicle.ApplyDrive(throttle, 0, Input.GetKey(KeyCode.Space));
            }
            else if (throttle < 0)
            {
                RGKVehicle.ApplyDrive(0, throttle, Input.GetKey(KeyCode.Space));
            }
        }

        //CheckIsCarFlipped();
    }

    protected void CheckIsCarFlipped()
    {
        if (transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280)
            FlipResetTimer += Time.deltaTime;
        else
            FlipResetTimer = 0;

        if (FlipResetTimer > FlipResetWait)
            FlipCar();
    }

    void FlipCar()
    {
        transform.rotation = Quaternion.LookRotation(transform.forward);
        transform.position += Vector3.up * 0.3f;
        transform.position += Vector3.right * 0.5f;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        FlipResetTimer = 0;
    }



}
