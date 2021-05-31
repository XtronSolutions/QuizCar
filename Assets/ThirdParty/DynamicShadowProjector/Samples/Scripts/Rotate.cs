using UnityEngine;

namespace DynamicShadowProjector.Sample {
	public class Rotate : MonoBehaviour {
		public float m_rotateSpeed = 90.0f;
		public Vector3 angle = Vector3.up;
		void Update()
		{
			transform.rotation = Quaternion.AngleAxis(m_rotateSpeed*Time.deltaTime, angle) * transform.rotation;
		}
	}
}
