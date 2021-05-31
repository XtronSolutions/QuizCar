using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlphaFade : MonoBehaviour {
	
	public Image alphaImage;
	private Color tempColor;
	public int fillRate;
	public bool fadeIn;
	public bool skipFirst;
	
	void OnEnable () {
		if (!skipFirst) {
			StartFade ();
		} else {
			skipFirst = false;
		}

	}

	void StartFade(){
		tempColor = alphaImage.color;
		tempColor.a = 0.001f;
		alphaImage.color = tempColor;
	}

	void Update () {
		if (!fadeIn) {
			tempColor = alphaImage.color;
			tempColor.a += 0.001f * fillRate;
			alphaImage.color = tempColor;
		} else {
			tempColor = alphaImage.color;
			tempColor.a += 0.001f * fillRate;
			alphaImage.color = tempColor;
		}
	}


}