using UnityEngine;
using System.Collections;

public class FlipUV : MonoBehaviour {

	public Material _material;
	public Camera _camera;

	void OnPreCull(){
		_camera.ResetWorldToCameraMatrix ();
		_camera.ResetProjectionMatrix ();
		Matrix4x4 mat = _camera.projectionMatrix;
		mat *= Matrix4x4.Scale (new Vector3(-1, 1, 1));
		_camera.projectionMatrix = mat;
	}

	void OnPreRender(){
		GL.invertCulling = true;
	}

	void OnPostRender(){
		GL.invertCulling = false;
	}

}