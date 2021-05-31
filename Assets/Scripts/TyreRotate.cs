using UnityEngine;

	public class TyreRotate : MonoBehaviour {
		public float m_rotateSpeed = 90.0f;
		void Update()
		{
		transform.RotateAround(transform.position, Vector3.forward,m_rotateSpeed*Time.deltaTime);
		}

}