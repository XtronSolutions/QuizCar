using System;
using UnityEngine;

[AddComponentMenu("Yufu/Effects/Car Nitro")]
public class RGKCar_Nitro : MonoBehaviour
{

    public bool NitroEnable = false;
    public float NitroLevel = 5f;
    public float NitroLeft = 0;
    public float NitroBalance = 1f;
    public float NitroCoiffeciency= 25f;
   // private CarSoundManager oSoundmanager;
    private void Start()
    {
        //oSoundmanager = base.GetComponent(typeof(CarSoundManager)) as CarSoundManager;
        
        NitroLeft = NitroLevel;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.N))
        {
            if (NitroLeft > 0)
            {
                base.GetComponent<Rigidbody>().AddForce(base.GetComponent<Rigidbody>().transform.forward * NitroCoiffeciency*Mathf.Clamp01(NitroBalance), ForceMode.Acceleration);
                NitroLeft -= Time.deltaTime * Mathf.Clamp01(NitroBalance) * 2;
                //if (oSoundmanager != null)
                //{
                   // oSoundmanager.PlayNitroSound();
                //}
            }
        }
        else
        {
            if (NitroLeft < NitroLevel)
            {
                NitroLeft += Time.deltaTime/2;
            }
        }
    }



}

