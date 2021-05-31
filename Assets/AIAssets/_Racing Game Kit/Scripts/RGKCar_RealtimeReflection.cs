using UnityEngine;
using System.Collections;

[AddComponentMenu("Racing Game Kit/Helpers/Car Realtime Reflection"), ExecuteInEditMode]
public class RGKCar_RealtimeReflection : MonoBehaviour
{
    // Attach this script to an object that uses a Reflective shader.
    // Realtime reflective cubemaps!


    public int cubemapSize = 128;
    public bool oneFacePerFrame = false;
    public Camera CubeCamera;
    private RenderTexture Renter2Texture;
	public GameObject cubeCamLocation;
	public LayerMask CullingMask;
    public float FarClipPlane = 250;
    public Material  Skybox;

    void Start()
    {
        // render all six faces at startup
        UpdateCubemap(63);
    }

    void LateUpdate()
    {
        if (!this.enabled) return;
        if (oneFacePerFrame)
        {
            int faceToRender = Time.frameCount % 6;
            int faceMask = 1 << faceToRender;
            UpdateCubemap(faceMask);
        }
        else
        {
            UpdateCubemap(63); // all six faces
        }
        
    }

    void UpdateCubemap(int faceMask)
    {
        if (CubeCamera==null)
        {
            GameObject go = new GameObject("CubemapCamera", typeof(Camera));
            go.hideFlags = HideFlags.HideAndDontSave;
            //go.transform.position = transform.position;
            //go.transform.rotation = Quaternion.identity;
            CubeCamera = go.GetComponent<Camera>();
        }

        if (CubeCamera!=null)
        {
            CubeCamera.farClipPlane = FarClipPlane; // don't render very far into cubemap
            CubeCamera.enabled = false;
            CubeCamera.cullingMask = CullingMask;

            if (cubeCamLocation != null)
            {
                CubeCamera.transform.parent = cubeCamLocation.transform.parent;
            }
            if (Skybox != null)
            {
                CubeCamera.gameObject.AddComponent(typeof(Skybox));
                Skybox oSky = CubeCamera.GetComponent(typeof(Skybox)) as Skybox;
                oSky.material = Skybox;
            }
          
        }

		if (cubeCamLocation!=null)
	    {
	    	CubeCamera.transform.localPosition=cubeCamLocation.transform.localPosition; 
	    }

        if (Renter2Texture==null)
        {
            Renter2Texture = new RenderTexture(cubemapSize, cubemapSize, 16);
            Renter2Texture.isPowerOfTwo = true;
            Renter2Texture.isCubemap = true;
            Renter2Texture.hideFlags = HideFlags.HideAndDontSave;
            //renderer.sharedMaterial.SetTexture("_Cube", rtex);

            Material[] materials = GetComponent<Renderer>().sharedMaterials;
            foreach (Material mat in materials)
            {
                if (mat.HasProperty("_Cube"))
                    mat.SetTexture("_Cube", Renter2Texture);
            }

        }

        //cam.transform.position = transform.position;
        CubeCamera.RenderToCubemap(Renter2Texture, faceMask);
    }

    void OnDisable()
    {
        DestroyImmediate(Renter2Texture);
    }
}