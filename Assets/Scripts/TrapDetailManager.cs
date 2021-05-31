using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapDetailManager : MonoBehaviour {

	public static TrapDetailManager Instance;

	void Awake(){
		Instance = this;
	}

	public GameObject content;
	public Image trap;
	public Image avatar;

	public Sprite defaultAvatar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowTrapDetail(Sprite trapSprite, Sprite avatarSprite){
		trap.sprite = trapSprite;
		avatar.sprite = avatarSprite;
		content.SetActive (true);

		Invoke ("DisableContent", 3.3f);
	}

	public void ShowTrapDetailOnline(Sprite trapSprite, string fbid, Sprite avatarSprite)
    {
        
        trap.sprite = trapSprite;
        avatar.sprite = avatarSprite;
        content.SetActive(true);

        Invoke("DisableContent", 3.3f);

        return;

        if (fbid.Length > 0) {
			FBManager.Instance.GetFacebookUserPicture (fbid, 100, 100, res => {
				trap.sprite = trapSprite;
				avatar.sprite = ImageUtils.CreateSprite (res.Texture, new Rect (0, 0, 100, 100), Vector2.zero);
				content.SetActive (true);
				Invoke ("DisableContent", 3.3f);
			});
		} else {

			trap.sprite = trapSprite;
			avatar.sprite = defaultAvatar;
			content.SetActive (true);
			Invoke ("DisableContent", 3.3f);
		}


	}

	void DisableContent(){
		content.SetActive (false);
	}
}
