using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAvatar : MonoBehaviour {

    public Transform target;
    public Transform cam;

    public string ID;
	// Use this for initialization
	void Start () {
    //    InvokeRepeating("Update2", 1, 3);
	}
	public void SetTexture(Sprite sprite)
    {
        GetComponent<MeshRenderer>().material.mainTexture = textureFromSprite(sprite);
    }
	// Update is called once per frame
	void Update () {
		if(cam==null)
        {
            cam = GameObject.Find("_GameCamera").transform;
        }
        else if(target!=null)
        {
            this.transform.LookAt(cam);
            this.transform.position = new Vector3(target.position.x, target.position.y + 1.5f, target.position.z);
        }
        else if(target==null)
        {
            Destroy(this.gameObject);
        }
	}

    public static Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}