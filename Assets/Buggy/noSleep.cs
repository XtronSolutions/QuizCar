using UnityEngine;
using System.Collections;

public class noSleep : MonoBehaviour {

	//prevent mobile device from going to sleep
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
#endif

}