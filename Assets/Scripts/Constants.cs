using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Constants
{
   
    public static string TAG_PLAYER_CAR = "PlayerCar";
    public static string TAG_AI_CAR = "AI";
    public static string AI_CAR_OBJECT_PREFIX = "AI_Car";
    public static string TAG_BULLET = "Bullet";
    public static string TAG_MISSILE = "Missile";
    public static string TAG_BOMB = "Bomb";
	public static string facebookInviteMessage = "Come play this great game!";

    public static int BulletHealthHit = 10;
    public static int MissileHealthHit = 20;
    public static int BombHealthHit = 30;
    public static int AI_PLAYERS_IN_BATTLE = 0;
	public static string PrivateRoomKey = "EnablePrivateRoomRequestes";
	public static bool isBombEffectEnable = false;

	public static bool inWater = false;
	public static string AndroidPackageName = "";
	public static string ITunesAppID = "";

	public static bool isMultiplayerSelected = false;
	public static bool isPrivateModeSelected = false;
	public static string joinCode = "";
	public static int coinsCollected = 0;
	// For Photon

	public static float photonDisconnectTimeoutLong = 300.0f;
	public static List<string> friendsIDForStatus = new List<string> ();
	public static string PhotonChatID = "201c4d06-1e5d-4aaf-ae92-46a6c7f62a8e";
	public static string PhotonAppID = "b11d6031-b67f-4d2d-896a-64bca2b0ac2d";
	public static string SharePrivateLinkMessage = "Join me in Race. Room# ";
	public static string SharePrivateLinkMessage2 = "Download Game from:";

	public static bool showLogin = true;
	public static bool isAutoLoginFirstTime = true;

    public static float getAngle(Vector2 destVect, Vector2 startVect)
    {
        float distX = destVect.x - startVect.x;
        float distY = destVect.y - startVect.y;
        float angle = (Mathf.Atan2(distY, distX));
        return angle;
    }



    public static class SceneName
    {
        
        public const string MainMenu = "MainMenuScene";
        public const string DessertScene = "DesertScene";
		public const string ForestScene = "ForestBeachScene";

    }
      
    public static void AnimateText(MonoBehaviour mono, TextMesh txt, float from, float to, float time)
    {
        mono.StartCoroutine(animateTextCoroutine(txt, from, to, time));
    }

    static IEnumerator animateTextCoroutine(TextMesh txt, float from, float to, float time)
    {
        float speed = (to - from) / time;

        if (speed > 0)
        {
            while (from < to)
            {
                from += speed * Time.deltaTime;
                yield return null;
                //Debug.LogWarning(from);
                txt.text = (from).ToString();
            }
        }
        else
        {
            while (from > to)
            {
                from += speed * Time.deltaTime;
                yield return null;
                //Debug.LogWarning(from);
                txt.text = ((int)from).ToString();
            }
        }

        txt.text = ((int)to).ToString();
    }

    public static string ResolveTextSize(string input, int lineLength)
    {

        // Split string by char " "         
        string[] words = input.Split(" "[0]);

        // Prepare result
        string result = "";

        // Temp line string
        string line = "";

        // for each all words        
        foreach (string s in words)
        {
            // Append current word into line
            string temp = line + " " + s;

            // If line length is bigger than lineLength
            if (temp.Length > lineLength)
            {

                // Append current line into result
                result += line + "\n";
                // Remain word append into new line
                line = s;
            }
            // Append current word into current line
            else
            {
                line = temp;
            }
        }

        // Append last line into result        
        result += line;

        // Remove first " " char
        return result.Substring(1, result.Length - 1);
    }
    public static string TruncateText(string input)
    {

        string[] str = input.Split('\n');
        if (str.Length > 2)
        {
            return str[0] + "\n" + str[1] + "...";
        }
        else
        {
            return input;
        }
    }


    //




}
