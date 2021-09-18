using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShareCodeController : MonoBehaviour {

	public GameObject RoomIDText;

	public void ShareCode ()
	{
		NativeShare share = new NativeShare ();

		string msg1= GameData.GetLocalizaedText("Text_SharePrivateLinkMessage");//Join me in Race. Room#
		string msg2 = GameData.GetLocalizaedText("Text_SharePrivateLinkMessage2");//Download Game from:

		string shareText = msg1 + " " + RoomIDText.GetComponent<Text> ().text + "\n\n" + msg2 + " ";
		
		#if UNITY_ANDROID
		shareText += "\n" + "Android : https://play.google.com/store/apps/details?id=" + Constants.AndroidPackageName;

		#elif UNITY_IOS
		shareText += "\n"+"IOS: https://itunes.apple.com/us/app/apple-store/id" + Constants.ITunesAppID;

		#endif
		share.Share (shareText, null, null, "Share via");



		//#if UNITY_ANDROID
		//shareText += "\n" + "Android : https://play.google.com/store/apps/details?id=" + Constants.AndroidPackageName + "\n" + "IOS: https://itunes.apple.com/us/app/apple-store/id" + Constants.ITunesAppID;

		//#elif UNITY_IOS
		//shareText += "\n" + "Android : https://play.google.com/store/apps/details?id=" + Constants.AndroidPackageName+"\n"+"IOS: https://itunes.apple.com/us/app/apple-store/id" + Constants.ITunesAppID;

	}
}
