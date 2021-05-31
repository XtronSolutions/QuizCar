using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
namespace UnityStandardAssets.Vehicles.Car
{
   
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
#if UNITY_EDITOR
            float h =  CrossPlatformInputManager.GetAxis("Horizontal");
            float v =  CrossPlatformInputManager.GetAxis("Vertical");
#else
           float h =  Input.acceleration.x;
            float v =  1f;
#endif
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
