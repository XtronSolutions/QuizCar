using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable 0414
public class NativeShare
{
#if !UNITY_EDITOR && UNITY_ANDROID
	private static AndroidJavaClass m_ajc = null;
	private static AndroidJavaClass AJC
	{
		get
		{
			if( m_ajc == null )
				m_ajc = new AndroidJavaClass( "com.yasirkula.unity.NativeShare" );

			return m_ajc;
		}
	}

	private static AndroidJavaObject m_context = null;
	private static AndroidJavaObject Context
	{
		get
		{
			if( m_context == null )
			{
				using( AndroidJavaObject unityClass = new AndroidJavaClass( "com.unity3d.player.UnityPlayer" ) )
				{
					m_context = unityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
				}
			}

			return m_context;
		}
	}
#elif !UNITY_EDITOR && UNITY_IOS
	[System.Runtime.InteropServices.DllImport( "__Internal" )]
	private static extern void _NativeShare_Share( string[] files, int filesCount, string subject, string text );
#endif

	private string subject;
	private string text;
	private string title;

	private string targetPackage;
	private string targetClass;

	private List<string> files;
	private List<string> mimes;

	public NativeShare()
	{
		subject = string.Empty;
		text = string.Empty;
		title = string.Empty;

		targetPackage = string.Empty;
		targetClass = string.Empty;

		files = new List<string>( 0 );
		mimes = new List<string>( 0 );
	}

	public NativeShare SetSubject( string subject )
	{
		if( subject != null )
			this.subject = subject;

		return this;
	}

	public NativeShare SetText( string text )
	{
		if( text != null )
			this.text = text;

		return this;
	}

	public NativeShare SetTitle( string title )
	{
		if( title != null )
			this.title = title;

		return this;
	}

	public NativeShare SetTarget( string androidPackageName, string androidClassName = null )
	{
		if( !string.IsNullOrEmpty( androidPackageName ) )
		{
			targetPackage = androidPackageName;

			if( androidClassName != null )
				targetClass = androidClassName;
		}

		return this;
	}

	public NativeShare AddFile( string filePath, string mime = null )
	{
		if( !string.IsNullOrEmpty( filePath ) && File.Exists( filePath ) )
		{
			files.Add( filePath );
			mimes.Add( mime ?? string.Empty );
		}
		else
			Debug.LogError( "File does not exist at path or permission denied: " + filePath );

		return this;
	}

//	public void Share()
//	{
//		if( files.Count == 0 && subject.Length == 0 && text.Length == 0 )
//		{
//			Debug.LogWarning( "Share Error: attempting to share nothing!" );
//			return;
//		}
//
//#if UNITY_EDITOR
//		Debug.Log( "Shared!" );
//#elif UNITY_ANDROID
//		AJC.CallStatic( "Share", Context, targetPackage, targetClass, files.ToArray(), mimes.ToArray(), subject, text, title );
//#elif UNITY_IOS
//		_NativeShare_Share( files.ToArray(), files.Count, subject, text );
//#else
//		Debug.Log( "No sharing set up for this platform." );
//#endif
//	}

	public void Share(string shareText, string imagePath, string url, string subject = "")
	{
#if UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		if (imagePath != null)
		{			
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			intentObject.Call<AndroidJavaObject>("setType", "image/png");
		}
		else
		{
			intentObject.Call<AndroidJavaObject>("setType", "text/plain");
		}

		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);

		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
		currentActivity.Call("startActivity", jChooser);
#elif UNITY_IOS
		CallSocialShareAdvanced(shareText, subject, url, imagePath);
#else
		Debug.Log("No sharing set up for this platform.");
#endif
		}

#if UNITY_IOS
		public struct ConfigStruct
		{
		public string title;
		public string message;
		}

		[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

		public struct SocialSharingStruct
		{
		public string text;
		public string url;
		public string image;
		public string subject;
		}

		[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);

		public static void CallSocialShare(string title, string message)
		{
		ConfigStruct conf = new ConfigStruct();
		conf.title  = title;
		conf.message = message;
		showAlertMessage(ref conf);
		}


		public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
		{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = defaultTxt;
		if(url != null) {
		conf.url = url;
		conf.image = img;
		}
		conf.subject = subject;

		showSocialSharing(ref conf);
		}
#endif

	#region Utility Functions
	public static bool TargetExists( string androidPackageName, string androidClassName = null )
	{
#if !UNITY_EDITOR && UNITY_ANDROID
		if( string.IsNullOrEmpty( androidPackageName ) )
			return false;

		if( androidClassName == null )
			androidClassName = string.Empty;

		return AJC.CallStatic<bool>( "TargetExists", Context, androidPackageName, androidClassName );
#else
		return true;
#endif
	}

	public static bool FindTarget( out string androidPackageName, out string androidClassName, string packageNameRegex, string classNameRegex = null )
	{
		androidPackageName = null;
		androidClassName = null;

#if !UNITY_EDITOR && UNITY_ANDROID
		if( string.IsNullOrEmpty( packageNameRegex ) )
			return false;

		if( classNameRegex == null )
			classNameRegex = string.Empty;

		string result = AJC.CallStatic<string>( "FindMatchingTarget", Context, packageNameRegex, classNameRegex );
		if( string.IsNullOrEmpty( result ) )
			return false;

		int splitIndex = result.IndexOf( '>' );
		if( splitIndex <= 0 || splitIndex >= result.Length - 1 )
			return false;

		androidPackageName = result.Substring( 0, splitIndex );
		androidClassName = result.Substring( splitIndex + 1 );

		return true;
#else
		return false;
#endif
	}
	#endregion
}
#pragma warning restore 0414
