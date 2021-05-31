using UnityEngine;
using System.Collections;

public class destruction : MonoBehaviour {
	public GameObject[] Chunks;
	public GameObject[] HidingObjs; //list of the objects that will be hidden after the crush.
	[Range(1,100)]
	public int Health = 100;
	public float ExplosionForce = 200; //force added to every chunk of the broken object.
	public float ChunksRotation = 20; //rotation force added to every chunk when it explodes.
	public float strength = 5; //How easily the object brokes.
	public bool BreakByClick = false;
	public bool DestroyAftertime = true; //if true, then chunks will be destroyed after time.
	public float time = 15; //time before chunks will be destroyed from the scene.
	public GameObject FX;
	public bool AutoDestroy = true; //if true, then object will be automatically break after after "AutoDestTime" since game start.
	public float AutoDestTime = 2; //Auto destruction time (counts from game start).
	bool isCameraShake = false;
	public float cameraShakeValue= 2.5f;
	
	void Start () {
		
		if(AutoDestroy){
			Invoke("Crushing", AutoDestTime);
		}
		
		if(GetComponent<AudioSource>()){
			GetComponent<AudioSource>().pitch = Random.Range (0.7f, 1.1f);
		}
		if(HidingObjs.Length !=0){
			foreach(GameObject hidingObj in HidingObjs){
				hidingObj.SetActive(true);
			}
		}
		gameObject.layer = LayerMask.NameToLayer ("Destructable");
	}
	
	void OnCollisionEnter(Collision other){
		//		Debug.Log ("tag:"+other.collider.tag+"name:"+other.collider.name);
		//		Debug.Log ("Velocity:"+other.relativeVelocity.magnitude);
		if(other.relativeVelocity.magnitude > strength){

			//			Debug.Log ("Velocity:"+other.relativeVelocity.magnitude);
//			Debug.Log(other.gameObject.tag);
			Crushing(other);
		}
		
	}

	//	void OnMouseDown(){
	//		if(BreakByClick){
	//			Crushing();
	//			BreakByClick = false;
	//		}
	//		}
	
	void Crushing(Collision other){
		if(HidingObjs.Length !=0){
			foreach(GameObject hidingObj in HidingObjs){
				hidingObj.SetActive(false);
			}
		}
		if(FX){
			FX.SetActive(true);
		}
		if(GetComponent<AudioSource>()){
			GetComponent<AudioSource>().Play ();
		}
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
		GetComponent<Rigidbody>().isKinematic = true;
		foreach(GameObject chunk in Chunks){
			chunk.SetActive(true);
			chunk.GetComponent<Rigidbody>().AddRelativeForce((other.transform.forward) * (other.relativeVelocity.magnitude/2) * ExplosionForce);
			chunk.GetComponent<Rigidbody>().AddRelativeTorque(other.transform.forward * -ChunksRotation*Random.Range(-5f, 5f));
			chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -ChunksRotation*Random.Range(-5f, 5f));
			
			StartCoroutine (DisableCollider (chunk));
			FadeOut (chunk);
			//			StartCoroutine (FadeOutShader (chunk));
		}
		
		if (other.gameObject.tag == "Player") 
		{
			if(!isCameraShake){
				isCameraShake =true;
				PlayerManagerScript.instance._RaceManager.GameCamereComponent.IsShakeCameraOnHitObstacle =isCameraShake;
			}
			other.gameObject.GetComponent<vehicleHandling> ().vehicleSpeed = 0;
		}
		
		if(DestroyAftertime){
			Invoke("DestructObject", time);
		}
		
	}
	float a = 1;
	
	private void FadeOut(GameObject _obj)
	{
		Color col = _obj.GetComponent<MeshRenderer>().material.color;
		LeanTween.value (gameObject, delegate(float val){
			_obj.GetComponent<MeshRenderer>().material.color = new Color(col.r, col.g, col.b, val);
		}, 1, 0, 1.5f);
	}
	IEnumerator FadeOutShader(GameObject _obj)
	{
		_obj.GetComponent<MeshRenderer> ().material.SetFloat ("_Opacity", a -= 0.01f);
		//		Color col = _obj.GetComponent<MeshRenderer>().material.color;
		//		_obj.GetComponent<MeshRenderer>().material.color = new Color(col.r, col.g, col.b, a-= 0.01f);
		yield return new WaitForSeconds (2f);
		
	}
	
	IEnumerator DisableCollider(GameObject _obj)
	{
		yield return new WaitForSeconds (0.5f);
		if(_obj.GetComponent<BoxCollider> ()!=null)
		_obj.GetComponent<BoxCollider> ().enabled = false;

		if(_obj.GetComponent<SphereCollider> ()!=null)
			_obj.GetComponent<SphereCollider> ().enabled = false;
	}
	
	void DestructObject(){
//		isCameraShake = false;
//		PlayerManagerScript.instance._RaceManager.GameCamereComponent.IsShakeCameraOnHitObstacle = isCameraShake;
		Destroy(gameObject);
	}
	
}
