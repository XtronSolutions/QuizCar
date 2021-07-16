using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarController : MonoBehaviour
{
    public List<MeshRenderer> MeshParts;
    MainCamera mainCamera;
    // Use this for initialization
    void Start()
    {
        mainCamera = GameObject.FindObjectOfType<MainCamera>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Water" && this.gameObject.GetInstanceID() == PlayerManagerScript.instance.Car.GetInstanceID())
        {
            mainCamera.waterTrails.SetActive(true);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Water")
        {
          //  mainCamera.waterTrails.SetActive(false);
        }
    }

}