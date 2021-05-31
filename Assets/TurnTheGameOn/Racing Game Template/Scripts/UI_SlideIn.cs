using UnityEngine;
using System.Collections;

public class UI_SlideIn : MonoBehaviour {

	//public enum SlideType{ FromBottom, FromTop, FromLeft, FromRight }
	//public SlideType slideType;
	//RectTransform rectObject;
	//Vector2 anchoredPosition;
	//Vector2 startingPosition;
	//public float smooth;
	//Vector2 tempPosition;

	//void Awake(){
	//	rectObject = GetComponent<RectTransform> ();
	//	startingPosition.x = rectObject.anchoredPosition.x;
	//	startingPosition.y = rectObject.anchoredPosition.y;
	//	anchoredPosition = rectObject.anchoredPosition;
	//}

	//// Use this for initialization
	//void OnEnable () {		
	//	OpenWindow ();
	//}

	//public void OpenWindow(){		
	//	if (slideType == SlideType.FromBottom) {
	//		startingPosition.y = -Screen.height + (-startingPosition.y);

	//	}
	//	else if (slideType == SlideType.FromTop) {
	//		startingPosition.y = Screen.height + startingPosition.y;

	//	}
	//	else if (slideType == SlideType.FromLeft) {
	//		startingPosition.x = -Screen.width + (-startingPosition.x);

	//	}
	//	else if (slideType == SlideType.FromRight) {
		
	//	}
	//	rectObject.anchoredPosition = startingPosition;
	//}

	//public void CloseWindow(){
	//	anchoredPosition.y = Screen.height * 1.25f;
	//}
	
	//// Update is called once per frame
	//void Update () {
	//	if (slideType == SlideType.FromBottom || slideType == SlideType.FromTop) {
	//		startingPosition.y = Mathf.SmoothStep (startingPosition.y, anchoredPosition.y, smooth);
	//	}
	//	else if(slideType == SlideType.FromLeft || slideType == SlideType.FromRight){
	//		startingPosition.x = Mathf.SmoothStep (startingPosition.x, anchoredPosition.x, smooth);
	//	}
	//	rectObject.anchoredPosition = startingPosition;
	//}

}
