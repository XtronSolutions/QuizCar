using UnityEngine;
using System.Collections;

public class RG_RotateAround : MonoBehaviour {
	
	public Transform target;
	public float distance= 10.0f;
	public float xSpeed= 250.0f;
	public float ySpeed= 120.0f;
	public float yMinLimit= -20;
	public float yMaxLimit= 80;
	public float rotationSpeed;
	private float x= 0.0f;
	private float y= 0.0f;
	private float xx;
	private float yy;
	private Vector3 position = new Vector3();
	Quaternion rotation;

	public static bool isDrag;
	public static bool isMousePressed;
	private float counter;
	private Vector3 mouseUpPosition;
	private Vector3 mouseDownPosition;
	private float xMouseMoved;
	private float yMouseMoved;



	void  Start (){
		isDrag = false;
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;

		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = true;
		rotation = Quaternion.Euler(y, x, 0);
		position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

		transform.rotation = rotation;
		transform.position = position;
	}

	void Update(){
		if(Application.isMobilePlatform){
			if(Input.touchCount > 1){
				isDrag = false;
			}
		}
	}

	void  LateUpdate (){
		if(Application.isMobilePlatform){
			if (target && isDrag) {
				x += Input.touches[0].deltaPosition.x * xSpeed * 0.008f;
				y -= Input.touches[0].deltaPosition.y * ySpeed * 0.008f;
				y = ClampAngle(y, yMinLimit, yMaxLimit);
				rotation = Quaternion.Euler(y, x, 0);
				position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				transform.rotation = rotation;
				transform.position = position;
			}
			else if(target){
				x += Time.deltaTime * xSpeed * 0.02f * rotationSpeed;
				y = ClampAngle(y, yMinLimit, yMaxLimit);
				rotation = Quaternion.Euler(y, x, 0);
				position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				transform.rotation = rotation;
				transform.position = position;		
			}
		}
		else{
			if (target && isDrag){
				x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
				y = ClampAngle(y, yMinLimit, yMaxLimit);
				rotation = Quaternion.Euler(y, x, 0);
				position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				transform.rotation = rotation;
				transform.position = position;
			}
			else if(target)	{
				x += Time.deltaTime * xSpeed * 0.02f * rotationSpeed;
				y = ClampAngle(y, yMinLimit, yMaxLimit);
				rotation = Quaternion.Euler(y, x, 0);
				position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
				transform.rotation = rotation;
				transform.position = position;		
			}
		}
	}

	static float  ClampAngle ( float angle ,   float min ,   float max  ){
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}

	// Update is called once per frame
	void OnGUI () {
		if(Application.isMobilePlatform){
			if(Input.touchCount > 1){
				isDrag = false;
			}
		}
		//Debug.Log (isDrag);
		if (Event.current.type == EventType.MouseDown) {
			if(Application.isMobilePlatform){			
				mouseDownPosition = Input.touches[0].position;
			}
			else{
				mouseDownPosition = Input.mousePosition;
			}
			isMousePressed = true;
		}
		else if(Event.current.type == EventType.MouseDrag){
			CancelInvoke("StartCounting");
			counter = 0f;
			isDrag=true;
		}
		else if(Event.current.type == EventType.MouseUp){
			isMousePressed = false;
			if(Application.isMobilePlatform){				
				mouseUpPosition = Input.touches[0].position;
			}
			else{
				mouseUpPosition = Input.mousePosition;
			}
			xMouseMoved = Mathf.Abs(mouseUpPosition.x - mouseDownPosition.x);
			yMouseMoved = Mathf.Abs(mouseUpPosition.y - mouseDownPosition.y);
			//if the makes a small drag, consider it click
			if((xMouseMoved < 5f) || (yMouseMoved < 5f)){
				isDrag = false;
			}
			else{
				InvokeRepeating("StartCounting", 0.02f, 0.02f);
			}
		}
	}

	//we need this method so that Raycast won't be activated if we release the mouse button or the swipe, only on touches, not on drag
	void StartCounting(){
		counter += Time.deltaTime;
		if(counter > 0.01f){
			isDrag = false;
		}
	}

}