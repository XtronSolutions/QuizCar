using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCameraDepthInForwardCameraWater : MonoBehaviour {
#if UNITY_EDITOR
	void OnDrawGizmos(){
		Set();
	}
#endif
    void Start()
    {
        Set();
    }
    void Set()
    {
        if (GetComponent<Camera>().depthTextureMode == DepthTextureMode.None)
            GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }
}
