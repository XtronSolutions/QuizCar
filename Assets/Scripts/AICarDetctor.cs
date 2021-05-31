using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICarDetctor : MonoBehaviour
{
    public List<GameObject> EnemyCars;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("AI"))
        {
            other.GetComponentInParent<BC_AI_Helper>().reachedPlayerCar = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            other.GetComponentInParent<BC_AI_Helper>().reachedPlayerCar = false;
        }
    }
}