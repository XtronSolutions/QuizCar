using UnityEngine;
using System.Collections;

public class RG_FadeTextureOut : MonoBehaviour {

	//The texture that should be faded out
	public Texture2D fadeOutTexture;
	//The higher the value the faster the texture will be faded out
	[Range(0.001f,1f)]public float fadeSpeed = 0.05f;
	//lifeTimer is used to determine how long to wait before the texture is destroyed
	//Adjust this value for layering textures via GUI.depth
	[Range(1,30)]public int lifeTimer = 4;
	//Private variables to make the script work, should not need to be adjusted
	private bool alphaWait = true;
	private float alpha = 1.0f;
	private float timePosX;
	private float timePosY;
	private float timeBoxWidth;
	private float timeBoxHeight;
	[Range(0,1)] public float positionX;
	[Range(0,1)] public float positionY;
	[Range(0,1)] public float width;
	[Range(0,1)] public float height;
	public int drawDepth = -1000;

	// Use this for initialization
	//This script executes at Start
	//The prefab it's attached to should be included in the scene at startup or instantiated
	//Use the lifeTimer variable to determine when to destroy the prefab
	void Start () {
		//Will destroy the GameObject when its lifeTimer is reached
		CalculateScale ();
		Destroy(gameObject, lifeTimer);
		fadeTextureOut();
	}
	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		CalculateScale ();
		#endif
	}

	void fadeTextureOut(){
		alphaWait = false;
	}

	void OnGUI(){
		if(alphaWait == false) {
			alpha += -1 * fadeSpeed * Time.deltaTime;
		}
		alpha = Mathf.Clamp01(alpha); 
		Color thisColor = GUI.color;
		thisColor.a = alpha;
		GUI.color = thisColor;
		GUI.depth = drawDepth;
		//The Rect values can be adjusted to customize the size and shape of your texture
		//By default the texture has a full screen presentation
		GUI.DrawTexture( new Rect(timePosX, timePosY, timeBoxWidth, timeBoxHeight), fadeOutTexture);
	}

	void CalculateScale(){
		timeBoxWidth = Screen.width * width;
		timeBoxHeight = Screen.height * height;
		timePosX = Screen.width * positionX;
		timePosY = Screen.height * positionY;
	}

}
