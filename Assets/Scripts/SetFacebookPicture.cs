using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFacebookPicture : MonoBehaviour {

	public Image Sprite;
	// Use this for initialization
	void Start () {
		if (FBManager.Instance.profileImage != null) {
          //  Sprite.sprite =   UnityEngine.Sprite.Create(FBManager.Instance.profileImage, new Rect(0, 0, FBManager.Instance.profileImage.width, FBManager.Instance.profileImage.height), new Vector2(0.5f, 0.5f)); 

        }
	}
		

	public void setPicture(){
	
		if (FBManager.Instance.profileImage != null) {
			//Sprite.sprite = UnityEngine.Sprite.Create(FBManager.Instance.profileImage, new Rect(0, 0, FBManager.Instance.profileImage.width, FBManager.Instance.profileImage.height), new Vector2(0.5f, 0.5f));
        }
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void EmptyFriendList(){
	
		GameManager.Instance.friendsObjects.Clear();

	}
}
