using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShareCodeController : MonoBehaviour {

	public GameObject RoomIDText;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShareCode ()
	{
		NativeShare share = new NativeShare ();
		string shareText = Constants.SharePrivateLinkMessage + " " + RoomIDText.GetComponent<Text> ().text + "\n\n" + Constants.SharePrivateLinkMessage2 + " ";
		#if UNITY_ANDROID
		shareText += "\n" + "Android : https://play.google.com/store/apps/details?id=" + Constants.AndroidPackageName + "\n" + "IOS: https://itunes.apple.com/us/app/apple-store/id" + Constants.ITunesAppID;

		#elif UNITY_IOS
		shareText += "\n" + "Android : https://play.google.com/store/apps/details?id=" + Constants.AndroidPackageName+"\n"+"IOS: https://itunes.apple.com/us/app/apple-store/id" + Constants.ITunesAppID;

		#endif
		share.Share (shareText, null, null, "Share via");
	}
}
