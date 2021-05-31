using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
//using Facebook.Unity;
using MiniJSON;

public class FBManager2 : MonoBehaviour {
//	public GameObject FBResponse;
	private Text username;
	private Image img;
	public GameObject LogInButton;
	public GameObject FBLoggedInInfoButton;
	public static bool IsPushNoti = true;
	private List<string> permissions = new List<string> ();
	private bool isFBlogged = false;
	private string imageURL = "";

	// invite

	void Awake()
	{
//		isFBlogged = false;
//		IsPushNoti = false;
//		permissions.Add ("public_profile");
//		permissions.Add ("email");
//		if (!FB.IsInitialized) {
//			// Initialize the Facebook SDK
//			FB.Init (SetInit, OnHideUnity);
//		} else {
//			// Already initialized, signal an app activation App Event
//			FB.ActivateApp();
//		}
	}
	void Start(){
//		username = FBLoggedInInfoButton.GetComponentInChildren<Text> ();
//		img = FBLoggedInInfoButton.transform.FindChild("DP").GetComponent<Image> ();
	}


	private void loadUserProfile () {
			ProfileInfo();
	
	}

	void SetInit()
	{

	}
	
	void OnHideUnity( bool isGameShown)
	{
	}
	
	public void LogInFacebook()
	{
//		if (FB.IsLoggedIn) 
//		{
////			permissions.Add ("public_profile");
////			permissions.Add ("email");
//			isFBlogged = true;
////			CentralVariables.isFacebooklogged = true;
////			CentralVariables.SaveToFile();
//
//			this.GetComponent<UIManager>().OnFacebookLoginSuccessfull();
//			LogInButton.SetActive(false);
//			ProfileInfo();
//
//
//		} else 
//		{
////			CentralVariables.isFacebooklogged = false;
////			CentralVariables.SaveToFile();
//			//LogInButton.SetActive(true);
//			FB.LogInWithReadPermissions (permissions, AuthCallBack);
//		}


	}

	public void SignOut(){
//		FB.LogOut();
	}
	public static IEnumerator RealLoadImage (Image img, string url) {
		
		//A URL where the image is stored
		string urlimg = url;  
		//Call the WWW class constructor
		
		WWW imageURLWWW = new WWW(urlimg );  
		
		//Wait for the download
		yield return imageURLWWW;     
		
		if (imageURLWWW.texture != null) {
			
			if(imageURLWWW.error != null){
				img.sprite = Resources.Load<Sprite>("PictureNA") as Sprite; 
				
			}
			else{
//				Sprite sprite = new Sprite ();  
				imageURLWWW.texture.filterMode = FilterMode.Bilinear;
				imageURLWWW.texture.mipMapBias = 0;
				Sprite sprite = Sprite.Create (imageURLWWW.texture, new Rect (0, 0, imageURLWWW.texture.width, imageURLWWW.texture.height), Vector2.zero);  
				img.GetComponent<Image> ().sprite = sprite;  
			}
		} 
		
	}
	public void LogInFacebookRegister()
	{
//		FB.LogInWithReadPermissions (permissions, AuthCallBack);
	}
//	void AuthCallBack(ILoginResult result){
//		if (FB.IsLoggedIn) {
//			this.GetComponent<UIManager>().OnFacebookLoginSuccessfull();
//			ProfileInfo ();
////			CentralVariables.isFacebooklogged = true;
////			CentralVariables.SaveToFile();
//			LogInButton.SetActive(false);
//		
//		} else 
//		{
////			CentralVariables.isFacebooklogged = false;
////			CentralVariables.SaveToFile();
//			/// Active FB Response Dialogue and set values for conecting with facebook
//			///
//
//		}

//	}
	
	void ProfileInfo()
	{
//		FB.API ("/me?fields=id,first_name,last_name,email,picture", HttpMethod.GET, GetUserData);
//		FB.API("/me/picture?redirect=false", HttpMethod.GET, UserImageURL);
	//	FB.API ("/me?fields=last_name", HttpMethod.GET, GetUserData);
	//	FB.API ("/me?fields=picture{url}",HttpMethod.GET,UserImageURL);
	//	FB.API("/me?fields=email", HttpMethod.GET, GetUserData); 
	}

//	void GetUserData(IResult result)
//	{
//		if (result.Error == null) {
//
////			NetworkConstants.u_socialID = "" + result.ResultDictionary ["id"];
////			NetworkConstants.u_firstName = "" + result.ResultDictionary ["first_name"];
////			NetworkConstants.u_lastName = "" + result.ResultDictionary ["last_name"];
//			username.text = "" + result.ResultDictionary ["first_name"];
////			NetworkConstants.u_email = "" + result.ResultDictionary ["email"];
//			//FacebookAPIResponse response = ServerConstants.GetValueFromJSON<FacebookAPIResponse> (result.RawResult);
//			IDictionary fbResponseDict = MiniJSON.Json.Deserialize(result.RawResult) as IDictionary;
//			IDictionary fbPictureDict = fbResponseDict["picture"] as IDictionary;
//			IDictionary fbPicDataDict = fbPictureDict["data"] as IDictionary;
//			imageURL = fbPicDataDict["url"].ToString();
//			StartCoroutine(RealLoadImage(img,imageURL));
//			//img.transform.parent.gameObject.SetActive(true);
//			FBLoggedInInfoButton.SetActive(true);
//           // imageURL = response.picture.data.url;
//			if(!PlayerPrefs.HasKey("facebookRegister")){
//				PlayerPrefs.SetInt("facebookRegister",1);
//				//SendFacebookNetworkcall () ;
//			}else{
//				//FaceBookUserLogin();
//			}
//
//
//		} else 
//		{
////			NetworkConstants.u_firstName = "Error";
////			Debug.Log("SBW >>  GetUserData: Error");
//		}
//
//	}

//	void UserImageURL(IResult result){
//	
//		if (string.IsNullOrEmpty(result.Error) && !result.Cancelled) {
//			IDictionary data = result.ResultDictionary["data"] as IDictionary;
//			string photoURL = data["url"] as string;
//
////			NetworkConstants.u_imageURL = photoURL;
//		//	FBResponse.SetActive(false);
//
//		}
//		else{
//
//			LogInButton.SetActive(true);
//
//		}
//
//	
//
//	}

	public void RetryFBConnection(){
		//FBResponse.SetActive(false);
		//FB.Init (SetInit, OnHideUnity);
	}
	public void OkButton(){
		//FBResponse.SetActive(false);
	}







	void invite(){
//		FB.Mobile.AppInvite(
//			new Uri("https://fb.me/279377669125278"),
//			new Uri("http://imgur.com/gallery/xMFIc"),
//			OnSuccess
//			);
	}
//	void OnSuccess(IAppInviteResult result)
//	{
//	
//	}



	/// <summery >
	/// post to facebook
	/// </summery>
	
//	public void PostToFacebook(){
//		FB.ShareLink(
//			new Uri(CentralVariables.appLink()),
//			CentralVariables.LINK_NAME,
//			getSharingText(),
//			new Uri("http://imgur.com/gallery/xMFIc"),
//			this.HandleResult);
//	}
//	private void HandleResult(IResult result)
//	{
//		if (result == null) {
//			Debug.Log("Null Response\n");
//			return;
//		}
//		// Some platforms return the empty string instead of null.
//		if (!string.IsNullOrEmpty(result.Error)) {
//			Debug.Log("Error Response:\n" + result.Error);
//			
//		} else if (result.Cancelled) {
//			Debug.Log("Cancelled Response:\n" + result.RawResult);
//			
//		} else if (!string.IsNullOrEmpty(result.RawResult)) {
//			Debug.Log("Success Response:\n" + result.RawResult);
//			
//		} else {
//			Debug.Log("Empty Response\n");
//		}
//	}

}
