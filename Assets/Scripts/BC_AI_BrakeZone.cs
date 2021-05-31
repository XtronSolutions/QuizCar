using UnityEngine;
using System.Collections;

public class BC_AI_BrakeZone : MonoBehaviour
{
    float maxBreakTorque;
    float minCarSpeed;

    // Use this for initialization
    void Start()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "AI")
        {
            float controlCurrentSpeed = other.transform.root.GetComponent<BC_AI_CarScript>().currentSpeed;
            if (controlCurrentSpeed >= minCarSpeed)
            {
                other.transform.root.GetComponent<BC_AI_CarScript>().inSector = true;
                other.transform.root.GetComponent<BC_AI_CarScript>().wheelRR.brakeTorque = maxBreakTorque;
                other.transform.root.GetComponent<BC_AI_CarScript>().wheelRL.brakeTorque = maxBreakTorque;
            }
            else
            {
                other.transform.root.GetComponent<BC_AI_CarScript>().inSector = false;
                other.transform.root.GetComponent<BC_AI_CarScript>().wheelRR.brakeTorque = 0;
                other.transform.root.GetComponent<BC_AI_CarScript>().wheelRL.brakeTorque = 0;
            }
            other.transform.root.GetComponent<BC_AI_CarScript>().isBreaking = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "AI")
        {
            other.transform.root.GetComponent<BC_AI_CarScript>().inSector = false;
            other.transform.root.GetComponent<BC_AI_CarScript>().wheelRR.brakeTorque = 0;
            other.transform.root.GetComponent<BC_AI_CarScript>().wheelRL.brakeTorque = 0;
            other.transform.root.GetComponent<BC_AI_CarScript>().isBreaking = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}