using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
//using PlayFab.Json;
using Photon.Pun;

public static class GlobalVariables 
{
	public static bool isAnimating = false;
	public static bool isPause = false;
	public static bool isGameEnd = false;

	public static int PLAYER_MODE = 0;
	public static int ENVIRONMENT = 1;
	public static int BUGGY = 0;
	public static int CHARACTER = 0;
	public static int WEAPON = 0;

	public static bool CAME_FROM_DEEPLINKING = false;
	public static bool FORREST_SCENE_SELECTED = false;
	public static bool DESERT_SCENE_SELECTED = false;
	public static string MODE_TYPE_DEEPLINKING = "";
	public static int PAYOUTCOINS_DEEPLINKING = int.MaxValue;
	public static string INVITOR_NAME_DEEPLINKING = "";
	public static string ROOMID_DEEPLINKING = "";
	public static int FacebookRequestIndex = -1;

	public static GameObject FindGameObjectByViewId(int viewId){
		foreach (var g in GameObject.FindObjectsOfType<PhotonView>()) {
			if (g.ViewID == viewId)
				return g.gameObject;
		}
		return null;
	}

	public static void Reset_DeeplinkingVariables ()
	{
		CAME_FROM_DEEPLINKING = false;
		MODE_TYPE_DEEPLINKING = "";
		PAYOUTCOINS_DEEPLINKING = int.MaxValue;
		INVITOR_NAME_DEEPLINKING = "";
		ROOMID_DEEPLINKING = "";
	}

	public static List<string> FacebookAppRequests;

	public static void GetFacebookAppRequests ()
	{
		string savedRequests = PlayerPrefs.GetString ("FacebookAppRequests", "");

		Debug.Log ("DEEPLINKING: SavedRequests: " + savedRequests);
		List<string> temp = null;
        if (!savedRequests.Equals(""))
        {
            //temp = PlayFabSimpleJson.DeserializeObject<List<string>> (savedRequests);
        }
		if (temp == null) {
			Debug.Log ("DEEPLINKING:Null List: ");
			temp = new List<string> ();
		}
		FacebookAppRequests = temp;

	}

	public static void SaveFacebookAppRequests ()
	{
		Debug.Log ("DEEPLINKING: request was now saved");
        string savedRequests = "";
        //string savedRequests = PlayFabSimpleJson.SerializeObject (FacebookAppRequests);
		Debug.Log ("DEEPLINKING: request was now saved: " + savedRequests);
		PlayerPrefs.SetString ("FacebookAppRequests", savedRequests);
	}
	//BY UZAIR DEEPLINKING....

	//BY UZAIR FINAL FEEDBACK...

	public static void SetIfNotificationCanBeSent (bool isFreeTurnAvailable)
	{
		if (!isFreeTurnAvailable)
			PlayerPrefs.SetInt ("CanNotificationBeSent", 0);
		else
			PlayerPrefs.SetInt ("CanNotificationBeSent", 1);
	}

	public static bool CanNotificationBeSent ()
	{
		if (PlayerPrefs.GetInt ("CanNotificationBeSent", 0) == 0)
			return true;
		else
			return false;
	}
	//BY UZAIR FINAL FEEDBACK...


	//BY UZAIR FINAL TO PREVENT UNDO CHEAT...
	public static void SetRemainingDicesCountForRejoin (int _count)
	{
		if (!GlobalVariables.isOffline) {
			PlayerPrefs.SetInt ("RemmainingDicesCount", _count);
			setTurnComplete (_count);//BY UZAIR...16 July.....
		}
	}

	//BY UZAIR...16 July.....
	public static void setTurnComplete (int _count)
	{
		if (_count == 0)
			PlayerPrefs.SetInt ("isTurnComplete", 0);
		else
			PlayerPrefs.SetInt ("isTurnComplete", 1);
	}

	public static void setTurnComplete (bool _count)
	{
		if (_count)
			PlayerPrefs.SetInt ("isTurnComplete", 0);
		else
			PlayerPrefs.SetInt ("isTurnComplete", 1);
	}

	public static bool isTurnComplete ()
	{
		if (!GlobalVariables.isOffline) {
			if (PlayerPrefs.GetInt ("isTurnComplete", 0) == 0)
				return true;
			else
				return false;
		} else {
			return false;
		}
	}
	//BY UZAIR...16 July.....

	public static int GetRemainingDicesCountForRejoin ()
	{
		if (!GlobalVariables.isOffline) {				
			return PlayerPrefs.GetInt ("RemmainingDicesCount");
		} else {
			return 0;
		}
	}

	public static void SetRemainingDices (List<int> _dices)
	{
		if (!GlobalVariables.isOffline) {	
			for (int i = 0; i < _dices.Count; i++) {
				PlayerPrefs.SetInt ("RemmainingDicesCount" + i, _dices [i]);
			}
			SetRemainingDicesCountForRejoin (_dices.Count);
		}
	}

	public static void SetNumberOfRemainingSixes (int _count)
	{
		if (!GlobalVariables.isOffline) {	
			PlayerPrefs.SetInt ("RemmainingDicesSixCount", _count);	
		}
	}

	public static int GetNumberOfRemainingSixes ()
	{
		if (!GlobalVariables.isOffline) {	
			return PlayerPrefs.GetInt ("RemmainingDicesSixCount");
		} else {
			return 0;
		}
	}

	public static void RemoveUnusedRemainingDices (List<int> _dices)
	{
		if (!GlobalVariables.isOffline) {	
			for (int i = 0; i < GetRemainingDicesCountForRejoin (); i++) {
				PlayerPrefs.SetInt ("RemmainingDicesCount" + i, 0);
			}
			SetRemainingDices (_dices);
		}
	}

	public static void ClearAllRemainingDices ()
	{
		if (!GlobalVariables.isOffline) {	
			for (int i = 0; i < 10; i++) {
				if (PlayerPrefs.HasKey ("RemmainingDicesCount" + i)) {
					PlayerPrefs.DeleteKey ("RemmainingDicesCount" + i);
				}
			}
			SetRemainingDicesCountForRejoin (0);
			setTurnComplete (false);//BY UZAIR...16 July...
			SetIfLastRolledDiceWasSix (0);
			SetNumberOfRemainingSixes (0);
			SetIfLastTurnKilledAnotherToken (false);//BY UZAIR FINAL TO PREVENT UNDO CHEAT...
			SetDiceBeforeUndo (0);
		}
	}

	public static void SetIfLastRolledDiceWasSix (int _dice)
	{
		if (!GlobalVariables.isOffline) {	
			if (_dice == 6) {
				PlayerPrefs.SetInt ("LastRolledDiceWasSix", 1);
			} else {
				PlayerPrefs.SetInt ("LastRolledDiceWasSix", 0);
			}
		}
	}

	public static bool GetIfLastRolledDiceWasSix ()
	{
		if (!GlobalVariables.isOffline) {	
			if (PlayerPrefs.GetInt ("LastRolledDiceWasSix") == 1) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

//	public static void AddRemainingDicesToStepManager ()
//	{
//		if (!GlobalVariables.isOffline) {	
//			for (int i = 0; i < GetRemainingDicesCountForRejoin (); i++) {
//				if (PlayerPrefs.GetInt ("RemmainingDicesCount" + i) != 0)
//					StepsManageScript.steps.Add (PlayerPrefs.GetInt ("RemmainingDicesCount" + i));
//			}
//			StepsManageScript.sixCount = GetNumberOfRemainingSixes ();
//		}
//	}

	public static void SetIfLastTurnKilledAnotherToken (bool _hasKilled)
	{
		if (!GlobalVariables.isOffline) {	
			if (!_hasKilled)
				PlayerPrefs.SetInt ("LastTurnKilledAnotherToken", 0);
			else
				PlayerPrefs.SetInt ("LastTurnKilledAnotherToken", 1);
		}
	}

	public static bool GetIfLastTurnKilledAnotherToken ()
	{
		if (!GlobalVariables.isOffline) {	
			if (PlayerPrefs.GetInt ("LastTurnKilledAnotherToken") == 1)
				return true;
			else
				return false;
		} else {
			return false;
		}
	}

//	public static bool GetSkipInActivePlayersTurn ()
//	{
//		if (!GlobalVariables.isOffline) {	
//			if (PhotonNetwork.player.CustomProperties.ContainsKey ("Skipped")) {
//				int skipped = 0;
//				int.TryParse (PhotonNetwork.player.CustomProperties ["Skipped"].ToString (), out skipped);
//				if (skipped == 1) {
//					return true;
//				} else {
//					return false;
//				}
//			} else {
//				return false;
//			}
//		} else {
//			return false;
//		}
//
//	}
//
//	public static bool GetSkipInActivePlayersTurn (int _playerIndex)
//	{
//		if (!GlobalVariables.isOffline) {
//			if (PhotonNetwork.inRoom) {	
//				if (PhotonNetwork.room.CustomProperties.ContainsKey ("PTS" + _playerIndex)) {
//					int skipped = 0;
//					int.TryParse (PhotonNetwork.room.CustomProperties ["PTS" + _playerIndex].ToString (), out skipped);
//					if (skipped == 1) {
//						return true;
//					} else {
//						return false;
//					}
//				} else {
//					return false;
//				}
//			} else {
//				return false;
//			}
//		} else {
//			return false;
//		}
//
//	}

	//BY UZAIR FINAL TO PREVENT UNDO CHEAT...AFTER TESTING...
	public static void SetDiceBeforeUndo (int _dice)
	{
		if (!GlobalVariables.isOffline) {	
			PlayerPrefs.SetInt ("DiceBeforeUndo", _dice);
		}
	}

	public static int GetDiceBeforeUndo ()
	{
		if (!GlobalVariables.isOffline) {	
			return PlayerPrefs.GetInt ("DiceBeforeUndo");
		} else {
			return 0;
		}
	}

//	public static void AddDiceBeforeUndoToStepsManager ()
//	{
//		if (!GlobalVariables.isOffline) {	
//			if (GetDiceBeforeUndo () != 0) {
//				StepsManageScript.steps.Add (GetDiceBeforeUndo ());
//				if (GetDiceBeforeUndo () == 6)
//					StepsManageScript.sixCount = StepsManageScript.sixCount + 1;
//			}
//		}
//	}
//
	public static void AddDiceBeforeUndoToRemainingDices ()
	{
		if (!GlobalVariables.isOffline) {	
			if (GetDiceBeforeUndo () != 0) {
				PlayerPrefs.SetInt ("RemmainingDicesCount" + GetRemainingDicesCountForRejoin (), GetDiceBeforeUndo ());
				SetRemainingDicesCountForRejoin (GetRemainingDicesCountForRejoin () + 1);
				if (GetDiceBeforeUndo () == 6)
					SetNumberOfRemainingSixes (GetNumberOfRemainingSixes () + 1);
			}
			SetDiceBeforeUndo (0);
		}
	}
	//BY UZAIR FINAL TO PREVENT UNDO CHEAT...AFTER TESTING...

	//BY UZAIR FINAL TO PREVENT UNDO CHEAT...

	public static bool IsLeaderBoardFbButton = false;
	//BY UZAIR AFTER R6...



	//BY UZAIR AFTER REJOIN...

	public static int TotalPlayersOnRejoining = 0;

	public static void SetTeamMemberIDAndInvitor (string _id, bool _isInvitor)
	{
		PlayerPrefs.SetString ("TeamMemberID", _id);
		if (!_isInvitor)
			PlayerPrefs.SetInt ("IsInvitorToGame", 0);
		else
			PlayerPrefs.SetInt ("IsInvitorToGame", 1);
	}

	public static string GetTeamMemberID ()
	{
		return PlayerPrefs.GetString ("TeamMemberID");
	}

	public static void AssignTeamMemberOnRejoin ()
	{
		GlobalVariables.TeamMemberId = PlayerPrefs.GetString ("TeamMemberID");
		if (PlayerPrefs.GetInt ("IsInvitorToGame") == 0)
			GlobalVariables.isPlayerInivitor = false;
		else
			GlobalVariables.isPlayerInivitor = true;			
	}

	public static bool IsPlayerInvitor ()
	{
		if (PlayerPrefs.GetInt ("IsInvitorToGame") == 0)
			return false;
		else
			return true;
	}
	//BY UZAIR AFTER REJOIN...

	// BY UZAIR AFTER UI TEAM UP...
	public static bool IsInTeamUpMeetUpRoom = false;
	public static string TeamMemberId = "";
	public static bool TeamUpMatchMode = false;
	public static bool isPlayerInivitor = false;

	// BY UZAIR AFTER UI...
	public static string[] OfflinePlayersName = new string[3];

	// BY UZAIR AFTER UI FOR LOGIN OFFLINE...
	public static string playerNameOfflineMode = "Player 1";
	public static bool isOfflineModeFromLoginScreen = false;
	// BY UZAIR AFTER UI FOR LOGIN OFFLINE...

	// BY UZAIR AFTER UI MANUAL PlAY...
	public static bool isGameManual = false;

	public static bool isConnectingToGame = false;
	// BY UZAIR AFTER UI...

	// BY UZAIR AFTER R5...

	public static bool IsAutoRoomToChallengeFriend = false;
	public static string AutoRoomChallengedFriendID = "";

	public static bool isUndoDiceLimitReached = false;
	public static bool isUndoEnabled = true;
	public static int MAX_UNDO = 7;

	//BY UZAIR FINAL....
	public static void SetUNDO (bool _enabled)
	{
		if (_enabled) {
			PlayerPrefs.SetInt ("isUNDOEnabled", 1);
		} else {
			PlayerPrefs.SetInt ("isUNDOEnabled", 0);
		}
	}

	public static bool GetUndo ()
	{
		int x = PlayerPrefs.GetInt ("isUNDOEnabled");
		if (x == 1) {
			return true;
		} else {
			return false;
		}
	}
	//BY UZAIR FINAL....

	public static int GetCurrentUndos ()
	{
		return PlayerPrefs.GetInt ("PlayerUndoTimes");
	}

	public static void SetCurrentUndos (int times)
	{
		if (times >= MAX_UNDO) {
			isUndoDiceLimitReached = true;
		} else {
			isUndoDiceLimitReached = false;
		}
		PlayerPrefs.SetInt ("PlayerUndoTimes", times);

	}

	public static void IncrementUndos ()
	{
		int times = PlayerPrefs.GetInt ("PlayerUndoTimes");
		times = times + 1;
		SetCurrentUndos (times);
	}

	// BY UZAIR AFTER R5...
	public static bool isBuildForQATesting = false;
	//BY UZAIR FOR QA...
	public static bool isBuildForQATestingInApp = false;
	//BY UZAIR FOR QA...


	// BY UZAIR ===============================================================================================================================================
	public static bool isOffline = false;

	public static bool stepsRemaining = false;

	public static string PhotonRoomNameKey = "photonRoomName";
	public static string PhotonRoomTypeKey = "photonRoomType";
	public static bool RejoiningRoom = false;

	public static int botPlayersInGame = 0;
	public static int botIndex = -1;
	public static int botCounter = 0;

	public static int playersLeft = 0;

	public static void UpdateCurrentBotIndex ()
	{
		if (botPlayersInGame > 0) {
			botIndex++;
			botIndex = botIndex % botPlayersInGame;
		}
	}

	public static bool isLastTimeOfflineMode ()
	{
		int x = PlayerPrefs.GetInt ("isLastTimeOfflineMode");
		if (x == 1) {
			return true;
		} else {
			return false;
		}
	}

	public static void setLastPlayedMode (bool _isOffline)
	{
		if (_isOffline) {
			PlayerPrefs.SetInt ("isLastTimeOfflineMode", 1);
		} else {
			PlayerPrefs.SetInt ("isLastTimeOfflineMode", 0);
		}
	}

	public static void  SavePlayerPref (Dictionary<string, string> data)
	{ 
		foreach (var item in data) {
			PlayerPrefs.SetString (item.Key, item.Value);
		}
		PlayerPrefs.Save ();
	}

//	public static void SetPlayerName (string name)
//	{
//		PlayerPrefs.SetString (MyPlayerData.PlayerName, name);		
//	}
//
//	public static string GetPlayerName ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.PlayerName);
//	}
//
//	public static string GetPlayerRank ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.RankKey);
//	}
//
//	public static float GetPlayerXP ()
//	{
//		float temp = 0;
//		float.TryParse (PlayerPrefs.GetString (MyPlayerData.ExperienceKey), out temp);
//		return temp;
//	}
//
//	public static string GetPlayerID ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.PlayerIDKey);
//	}
//
//	public static int GetDices ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.DicesKey), out temp);
//		return temp;
//	}
//	// BY UZAIR AFTER UI...
//
//	public static int GetCoins ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.CoinsKey), out temp);
//		return temp;
//	}
//
//	public static int GetChampBought ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.ChampBoughtKey), out temp);
//		return temp;
//	}
//
//	public static int GetChamp1v1WinClaim ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ1v1WinClaimKey), out temp);
//		return temp;
//	}
//
//	public static int GetChamp2v2WinClaim ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ2v2WinClaimKey), out temp);
//		return temp;
//	}
//
//	public static int GetChamp1v1Coins ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ1v1Key), out temp);
//		return temp;
//	}
//
//	public static int GetChamp2v2Coins ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ2v2Key), out temp);
//		return temp;
//	}
//
//	public static int GetChamp1v1Wins ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ1v1WinsKey), out temp);
//		return temp;
//	}
//
//	public static int GetChamp2v2Wins ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.Champ2v2WinsKey), out temp);
//		return temp;
//	}
//
//
//	public static int GetLevel ()
//	{
//		int temp = 0;
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.LevelKey), out temp);
//		return temp;
//	}
//
//	public static string GetFlag ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.FlagKey);
//	}
//
//	public static string GetTeamMemberId ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.ChampteamMemberId);
//	}
//
//	public static string GetTempTeamMemberId ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.TempChampteamMemberId);
//	}
//
//	public static string GetTeamMemberName ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.ChampteamMemberName);
//	}
//
//	public static string GetTeamMemberRoomId ()
//	{
//		return PlayerPrefs.GetString (MyPlayerData.ChampPartnerRoomId);
//	}

	public static void SetPlayFabID (string id)
	{
		PlayerPrefs.SetString ("PlayFabID", id);
	}

	public static void SetNewFreshAccount (int _isNew)
	{
		PlayerPrefs.SetInt ("NewFreshAccount", _isNew);
	}

	public static bool CheckIsAccountNew ()
	{
		int x = PlayerPrefs.GetInt ("NewFreshAccount");
		if (x == 1) {
			return true;
		} else {
			return false;
		}
	}

	public static string GetPlayFabID ()
	{
		return PlayerPrefs.GetString ("PlayFabID");
	}

//	public static Dictionary<string, string> LoadPlayerPref ()
//	{
//		GameManager.Instance.nameMy = PlayerPrefs.GetString (MyPlayerData.PlayerName);
//		int tempIndex = 0;// BY UZAIR AFTER UI...
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.AvatarIndexKey), out tempIndex);// BY UZAIR AFTER UI...
//		GameManager.Instance.avatarMy = GameObject.Find ("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController> ().avatars [tempIndex];// BY UZAIR AFTER UI...
//		Dictionary<string, string> data = new Dictionary<string, string> ();
//		data.Add (MyPlayerData.PlayerName, PlayerPrefs.GetString (MyPlayerData.PlayerName));
//		data.Add (MyPlayerData.TotalEarningsKey, PlayerPrefs.GetString (MyPlayerData.TotalEarningsKey));
//		data.Add (MyPlayerData.ChatsKey, PlayerPrefs.GetString (MyPlayerData.ChatsKey));
//		data.Add (MyPlayerData.EmojiKey, PlayerPrefs.GetString (MyPlayerData.EmojiKey));
//		data.Add (MyPlayerData.CoinsKey, PlayerPrefs.GetString (MyPlayerData.CoinsKey));
//		data.Add (MyPlayerData.DicesKey, PlayerPrefs.GetString (MyPlayerData.DicesKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.LevelKey, PlayerPrefs.GetString (MyPlayerData.LevelKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.RankKey, PlayerPrefs.GetString (MyPlayerData.RankKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.ExperienceKey, PlayerPrefs.GetString (MyPlayerData.ExperienceKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.PlayerIDKey, PlayerPrefs.GetString (MyPlayerData.PlayerIDKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.FlagKey, PlayerPrefs.GetString (MyPlayerData.FlagKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.AvatarIndexKey, PlayerPrefs.GetString (MyPlayerData.AvatarIndexKey));
//		data.Add (MyPlayerData.GamesPlayedKey, PlayerPrefs.GetString (MyPlayerData.GamesPlayedKey));
//		data.Add (MyPlayerData.TournamentsWonKey, PlayerPrefs.GetString (MyPlayerData.TournamentsWonKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TournamentsPlayedKey, PlayerPrefs.GetString (MyPlayerData.TournamentsPlayedKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TeamWinsKey, PlayerPrefs.GetString (MyPlayerData.TeamWinsKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.WinStrikeKey, PlayerPrefs.GetString (MyPlayerData.WinStrikeKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TwoPlayerWinsKey, PlayerPrefs.GetString (MyPlayerData.TwoPlayerWinsKey));
//		data.Add (MyPlayerData.FourPlayerWinsKey, PlayerPrefs.GetString (MyPlayerData.FourPlayerWinsKey));
//		data.Add (MyPlayerData.TitleFirstLoginKey, PlayerPrefs.GetString (MyPlayerData.TitleFirstLoginKey));
//		data.Add (MyPlayerData.FortuneWheelLastFreeKey, PlayerPrefs.GetString (MyPlayerData.FortuneWheelLastFreeKey));
//		return data;
//	}
//
//	public static Dictionary<string, string> LoadPlayerPrefSync ()
//	{
//		GameManager.Instance.nameMy = PlayerPrefs.GetString (MyPlayerData.PlayerName);
//		int tempIndex = 0;// BY UZAIR AFTER UI...
//		int.TryParse (PlayerPrefs.GetString (MyPlayerData.AvatarIndexKey), out tempIndex);// BY UZAIR AFTER UI...
//		GameManager.Instance.avatarMy = GameObject.Find ("StaticGameVariablesContainer").GetComponent<StaticGameVariablesController> ().avatars [tempIndex];// BY UZAIR AFTER UI...
//		Dictionary<string, string> data = new Dictionary<string, string> ();
//		data.Add (MyPlayerData.PlayerName, PlayerPrefs.GetString (MyPlayerData.PlayerName));
//		data.Add (MyPlayerData.TotalEarningsKey, PlayerPrefs.GetString (MyPlayerData.TotalEarningsKey));
//		data.Add (MyPlayerData.ChatsKey, PlayerPrefs.GetString (MyPlayerData.ChatsKey));
//		data.Add (MyPlayerData.EmojiKey, PlayerPrefs.GetString (MyPlayerData.EmojiKey));
//		data.Add (MyPlayerData.CoinsKey, PlayerPrefs.GetString (MyPlayerData.CoinsKey));
//		data.Add (MyPlayerData.DicesKey, PlayerPrefs.GetString (MyPlayerData.DicesKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.RankKey, PlayerPrefs.GetString (MyPlayerData.RankKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.ExperienceKey, PlayerPrefs.GetString (MyPlayerData.ExperienceKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.PlayerIDKey, PlayerPrefs.GetString (MyPlayerData.PlayerIDKey));//BY UZAIR AFTER R5...
//		data.Add (MyPlayerData.LevelKey, PlayerPrefs.GetString (MyPlayerData.LevelKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.FlagKey, PlayerPrefs.GetString (MyPlayerData.FlagKey));// BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TournamentsWonKey, PlayerPrefs.GetString (MyPlayerData.TournamentsWonKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TournamentsPlayedKey, PlayerPrefs.GetString (MyPlayerData.TournamentsPlayedKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.TeamWinsKey, PlayerPrefs.GetString (MyPlayerData.TeamWinsKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.WinStrikeKey, PlayerPrefs.GetString (MyPlayerData.WinStrikeKey));//BY UZAIR AFTER UI...
//		data.Add (MyPlayerData.AvatarIndexKey, PlayerPrefs.GetString (MyPlayerData.AvatarIndexKey));
//		data.Add (MyPlayerData.GamesPlayedKey, PlayerPrefs.GetString (MyPlayerData.GamesPlayedKey));
//		data.Add (MyPlayerData.TwoPlayerWinsKey, PlayerPrefs.GetString (MyPlayerData.TwoPlayerWinsKey));
//		data.Add (MyPlayerData.FourPlayerWinsKey, PlayerPrefs.GetString (MyPlayerData.FourPlayerWinsKey));
//		data.Add (MyPlayerData.TitleFirstLoginKey, PlayerPrefs.GetString (MyPlayerData.TitleFirstLoginKey));
//		return data;
//	}

	// BY UZAIR AFTER UI...
	public static string API_KEY_FOR_LOCATIONS = "7fea7c8425234fc9b84a2db10a9fb36e0a262ed7075f2430deab6ff3";

//	public static string GetUserCountryByIp (string ip)
//	{
//		//ip = "2.24.0.0";//BY UZAIR FOR TESTING...
//		IpInfo ipInfo = new IpInfo ();
//		try {
//			string info = new WebClient ().DownloadString ("https://api.ipdata.co/" + ip + "/country_code?api-key=" + API_KEY_FOR_LOCATIONS);
//			ipInfo = PlayFabSimpleJson.DeserializeObject<IpInfo> (info);
//		} catch (Exception ex) {
//			Debug.Log ("Exception :: " + ex.Message + " || " + ex.StackTrace);
//		}
//		//Debug.LogError ("Country Code :: " + ipInfo.country_code + " On IP Adress :: " + ip);
//		return ipInfo.country_code;
//	}

//	public static string GetUserCountry ()
//	{
//		string info = "PK";
//		try {
//			info = new WebClient ().DownloadString ("https://api.ipdata.co/country_code?api-key=7fea7c8425234fc9b84a2db10a9fb36e0a262ed7075f2430deab6ff3");
//		} catch (Exception ex) {
//			//Debug.LogError ("Exception :: " + ex.Message + " || " + ex.StackTrace);
//			if (ex.Message.Equals ("The request timed out")) {//BY UZAIR FINAL FEEDBACK...AFTER QA
//				//Debug.LogError ("No net while fectching country...");
//				GameManager.Instance.connectionLost.showDialog ();
//			}//BY UZAIR FINAL FEEDBACK...AFTER QA
//		}
//		//Debug.LogError ("Country Code :: " + info);
//		return info;
//	}

	public static void SetRoomName (string _roomName, int _roomType)
	{
		PlayerPrefs.SetString (PhotonRoomNameKey, _roomName);
		PlayerPrefs.SetInt (PhotonRoomTypeKey, _roomType);
		GlobalVariables.TotalPlayersOnRejoining = 0;
	}

//	public static string GetRoomName ()
//	{
//		SetRoomType ();
//		return PlayerPrefs.GetString (PhotonRoomNameKey);
//	}

//	public static void SetRoomType ()
//	{
//		int type = PlayerPrefs.GetInt (PhotonRoomTypeKey);
//		switch (type) {
//		case 0:
//			GameManager.Instance.type = MyGameType.TwoPlayer;
//			break;
//		case 1:
//			GameManager.Instance.type = MyGameType.FourPlayer;
//			break;
//		case 2:
//			GameManager.Instance.type = MyGameType.Private;
//			break;
//		case 3:
//			GameManager.Instance.type = MyGameType.TeamUp;
//			break;
//		}
//	}

//	public static bool doesRoomExist ()
//	{
//		//			if (ChampsManager.instance.champsData.currentChampionshipPhase == EnumChampionshipPhase.NotHappening) {
//		if (!GetRoomName ().Equals ("")) {
//			return true;
//		} else {
//			return false;
//		}
//		//			} 
//		//			else {
//		//				return false;
//		//			}
//	}

//	public static void InitializePhotonCustomProperties ()
//	{
//		ExitGames.Client.Photon.Hashtable setValues = new ExitGames.Client.Photon.Hashtable ();
//		for (int index = 0; index < 4; index++)
//			setValues.Add ("currentPosition" + index.ToString (), -1);
//		setValues.Add ("canGoHome", false);
//		setValues.Add ("finishedPawns", 0);
//		setValues.Add ("BoardIndex", 0);
//		setValues.Add ("Skipped", 0);//BY UZAIR FINAL TO PREVENT UNDO CHEAT...AFTER TESTING...
//		PhotonNetwork.player.SetCustomProperties (setValues, null, false);
//		ClearAllRemainingDices ();//BY UZAIR FINAL TO PREVENT UNDO CHEAT...
//	}
//
//	public static void CanPhotonPlayerGoHomeIntialize ()
//	{
//		ExitGames.Client.Photon.Hashtable setValues = new ExitGames.Client.Photon.Hashtable ();
//		setValues.Add ("canGoHome", !GameManager.Instance.needToKillOpponentToEnterHome);
//		PhotonNetwork.player.SetCustomProperties (setValues, null, false);
//
//	}
	// BY UZAIR ===============================================================================================================================================
}

