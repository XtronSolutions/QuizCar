using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;

public class FacebookFriendEntry : MonoBehaviour {

	public Image userPictureImage;
	public Image userStatusDotImage;
	public Text userNameText;
	public Text statusText;
	public GameObject InviteBtn;
	public string Id = "";

	public Sprite GreenDotImage;
	public Sprite RedDotImage;

	public void SetUserPictureSprite(string userPictureSprite)
	{
		StartCoroutine (loadImage (userPictureSprite, userPictureImage));
	}

	public void SetUserName(string userName)
	{
		userNameText.text = userName;
	}

	public void SetUserStatusBoxColor(string status)
	{
		
	}

	public void SetUserStatusText(string status)
	{
		statusText.text = status;
	}

	public void SetUserID(string id)
	{
		Id = id;
	}

	public IEnumerator loadImage (string url, Image image)
	{
		// Load avatar image

		// Start a download of the given URL
		WWW www = new WWW (url);

		// Wait for download to complete
		yield return www;

		if (www.error == null) {//BY UZAIR AFTER R6...
			image.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0.5f, 0.5f), 32);
		} else {
			//			Debug.LogError (www.error.ToString ());
			//image.sprite = playersAvatars [0];
		}//BY UZAIR AFTER R6...

	}

	public void updateFriendStatus (int status, string id)
	{
		if (id.Equals (Id)) {
			if (status == ChatUserStatus.Playing) {
				this.transform.SetAsFirstSibling ();
				statusText.text = "Playing";
				userStatusDotImage.sprite = RedDotImage;
				InviteBtn.SetActive (false);
			} else if (status == ChatUserStatus.Offline) {
				this.transform.SetAsFirstSibling ();
				statusText.text = "Offline";
				userStatusDotImage.sprite = RedDotImage;
				InviteBtn.SetActive (false);
			} else if (status == ChatUserStatus.Online) {
				this.transform.SetAsFirstSibling ();
				statusText.text = "Online";
				userStatusDotImage.sprite = GreenDotImage;
				InviteBtn.SetActive (true);
			}
		}
	}

}
