using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RG_CarCamera : MonoBehaviour {

	public enum CameraType{ CarCamera, HelmetCamera}

	public KeyCode cameraSwitchKey;
	public bool mobile;
	public bool findCameraButton;
	public EventTrigger.Entry switchCameraEvent;
	public CameraType cameraType;
	public Camera carCamera;
	//public Camera helmetCamera;
    public Transform car;
	//public GameObject[] interiorCameraComponents;
    public float distance;
    public float height;
    public float rotationDamping;
    public float heightDamping;
    public float zoomRatio;
    public float DefaultFOV;
    private Vector3 rotationVector;
    private Vector3 position;
    private Rigidbody carBody;
	[Header("Camera Pivot")]
	public float max;
	public float value;
	public float minPivotMoveSpeed;
	public float maxPivotMoveSpeed;
	public float pivotMoveSpeed;
	public Vector3 targetPivotPosition;
	private string gameMode;
	public Transform lobbyPlayer;

	void Start(){
		if(GameObject.Find("Car Input") != null){
			mobile = true;
			findCameraButton = true;
		}
		//if(interiorCameraComponents.Length > 0)
		//	interiorCameraComponents [2] = GameObject.Find ("Player Arrow");
		if(findCameraButton){
			//EventTrigger findEvent = GameObject.Find ("Camera Switch Button").GetComponent<EventTrigger>();
			//if (findEvent != null) {
			//	findEvent.triggers.Add (switchCameraEvent);
			//	findEvent = null;
			//}
		}
		if (!car) {
			car = transform.parent.transform;
			carBody = car.GetComponent<Rigidbody> ();
		}
	}

	void Update(){
		if (car != null) {
			if (cameraType == CameraType.CarCamera) {
				//helmetCamera.gameObject.SetActive (false);
				transform.parent = lobbyPlayer;
				carCamera.gameObject.SetActive (true);
				//for (int i = 0; i < interiorCameraComponents.Length; i++) {
				//	if(interiorCameraComponents [i] != null)
				//		interiorCameraComponents [i].SetActive (false);
				//}
				//if (interiorCameraComponents.Length > 0 && interiorCameraComponents [2] != null)
				//	interiorCameraComponents [2].SetActive (true);
			} else if (cameraType == CameraType.HelmetCamera) {
				carCamera.gameObject.SetActive (false);
				if (car != null) {
					transform.parent = car;
				}
				//helmetCamera.gameObject.SetActive (true);
				//for (int i = 0; i < interiorCameraComponents.Length; i++) {
				//	if(interiorCameraComponents [i] != null)
				//		interiorCameraComponents [i].SetActive (true);
				//}
				//if (interiorCameraComponents.Length > 0 && interiorCameraComponents [2] != null)
				//	interiorCameraComponents [2].SetActive (false);
			}
			if (Input.GetKeyDown (cameraSwitchKey)) {
				CycleCamera ();
			}
		}
	}

    void LateUpdate() {
		if (car != null) {
			if (cameraType == CameraType.CarCamera) {				
				var wantedAngle = rotationVector.y;
				var wantedHeight = car.position.y + height;
				var myAngle = transform.eulerAngles.y;
				var myHeight = transform.position.y;
				myAngle = Mathf.LerpAngle (myAngle, wantedAngle, rotationDamping * Time.deltaTime);
				myHeight = Mathf.Lerp (myHeight, wantedHeight, heightDamping * Time.deltaTime);
				var currentRotation = Quaternion.Euler (0, myAngle, 0);
				transform.position = car.position;
				transform.position -= currentRotation * Vector3.forward * distance;
				position = transform.position;
				position.y = myHeight;
				transform.position = position;
				transform.LookAt (car);
				//carCamera.transform.LookAt (car);
				//Vector3.Lerp (cameraComponent.transform.position, targetPivotPosition, pivotMoveSpeed * Time.deltaTime);
				if (Input.GetAxis ("Horizontal") > 0) {
					value = -max;
					pivotMoveSpeed = minPivotMoveSpeed;
				} else if (Input.GetAxis ("Horizontal") < 0) {
					value = max;
					pivotMoveSpeed = minPivotMoveSpeed;
				} else {
					value = 0;
					pivotMoveSpeed = Mathf.Lerp (pivotMoveSpeed, maxPivotMoveSpeed, 1 * Time.deltaTime);
				}
				targetPivotPosition.x = Mathf.Lerp (targetPivotPosition.x, value, pivotMoveSpeed * Time.deltaTime);
				carCamera.transform.localPosition = targetPivotPosition;
			} else if (cameraType == CameraType.HelmetCamera) {
			
			}
		}
    }

	public void CycleCamera(){
		if (cameraType == CameraType.CarCamera) {
			cameraType = CameraType.HelmetCamera;
		}else if (cameraType == CameraType.HelmetCamera){
			cameraType = CameraType.CarCamera;
		}
	}
		
    void FixedUpdate(){
		
		if (cameraType == CameraType.CarCamera) {
			if (car && !mobile) {
				var localVelocity = car.InverseTransformDirection (carBody.velocity);
				if (localVelocity.z < -0.5f && Input.GetAxis ("Vertical") == -1) {
					rotationVector.y = car.eulerAngles.y + 180f;
				} else {
					rotationVector.y = car.eulerAngles.y;
				}
				var acc = carBody.velocity.magnitude;
				carCamera.fieldOfView = DefaultFOV + acc * zoomRatio * Time.deltaTime;
				//cameraComponent.transform.rotation = pivotRotation;
			}else if (car && mobile){
				
				var localVelocity = car.InverseTransformDirection (carBody.velocity);
				if (localVelocity.z < -0.5f && UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis ("Vertical") == -1) {
					rotationVector.y = car.eulerAngles.y + 180f;
				} else {
					rotationVector.y = car.eulerAngles.y;
				}
				var acc = carBody.velocity.magnitude;
				carCamera.fieldOfView = DefaultFOV + acc * zoomRatio * Time.deltaTime;
			}
		}
		else if (cameraType == CameraType.HelmetCamera){
		
		}

    }

}