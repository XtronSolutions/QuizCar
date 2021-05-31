using UnityEngine;
using System.Collections;

public class ShadowSwitch : MonoBehaviour {

	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width - 140, 50, 130, 40), "Toggle Shadows")) {
			GlobalProjectorManager.Get().ShadowsOn = !GlobalProjectorManager.Get().ShadowsOn;
		}
	}
}
