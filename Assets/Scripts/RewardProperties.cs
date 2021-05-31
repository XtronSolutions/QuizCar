using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardProperties : SingeltonBase<RewardProperties> {

	private int coins;
	private int nitro;
	private int tuningKit;
	private int sound;
	private int music; 
	private const string BuggyAcc = "BuggyAcceleration";
	private const string BuggyGreenAcc = "BuggyGreenAcceleration";
	private const string BuggyStrength = "BuggyStrength";
	private const string BuggyGreenStrength = "BuggyGreenStrength";
	private const string BuggyHandling = "BuggyHandling";
	private const string BuggyGreenHandling = "BuggyGreenHandling";
	private const string NoOfCarUpgrades = "BuggyUpgrades";
	private const string BuggyUpgradeText = "BuggyUpgradeText";
	private const string Buggies = "Buggies";
	private const string BoySkin = "BoySkin";
	private const string GirlSkin = "GirlSkin";
	private const string BuggiesSelected = "BuggiesSeleted";
	private const string CharacterSelected = "CharacterSelected";
	private const string TrackNumber = "TrackNumber";
	private const string TrackNumberDesert = "TrackNumberDesert";
	private const string SelectedTrack = "SelectedTrack";
	private const string SelectedTrackDesert = "SelectedTrackDesert";
	//private const bool[] IsBuggyLocked = { true, false, false, false };

	public int SelectedBuggy = 0;
	private bool isFbLoggedIn = false;

	public int Coin{
		get{
            //if(!PlayerPrefs.HasKey("Coin")){
            //	PlayerPrefs.SetInt("Coin",100000);
            //}
            //coins = PlayerPrefs.GetInt("Coin");
            //return coins;
            return GameData.data.coins;
		}

		set{
            GameData.data.coins = value;
             
			//coins = value;
			//PlayerPrefs.SetInt("Coin", coins);
		}
	}

	public int Nitro{
		get{
			if(!PlayerPrefs.HasKey("Nitro")){
				PlayerPrefs.SetInt("Nitro",1);
			}
			nitro = PlayerPrefs.GetInt("Nitro");
			return nitro;
		}

		set{
			nitro = value;
			PlayerPrefs.SetInt("Nitro", nitro);
		}
	}

	public int TuningKit{
		get{
			if(!PlayerPrefs.HasKey("TuningKit")){
				PlayerPrefs.SetInt("TuningKit",1);
			}
			tuningKit = PlayerPrefs.GetInt("TuningKit");
			return tuningKit;
		}

		set{
			tuningKit = value;
			PlayerPrefs.SetInt("TuningKit", tuningKit);
		}
	}
		
	public int Sounds{
		get{
			if(!PlayerPrefs.HasKey("Sounds")){
				PlayerPrefs.SetInt("Sounds",1);
			}
			sound = PlayerPrefs.GetInt("Sounds");
			return sound;
		}

		set{
			sound = value;
			PlayerPrefs.SetInt("Sounds", sound);
		}
	}

	public int Music{
		get{
			if(!PlayerPrefs.HasKey("Music")){
				PlayerPrefs.SetInt("Music",1);
			}
			music = PlayerPrefs.GetInt("Music");
			return music;
		}

		set{
			music = value;
			PlayerPrefs.SetInt("Music", music);
		}
	}

	public bool IsFbLoggedIn{
		get{ 
			if (!PlayerPrefs.HasKey ("is_fb_logged_in")) {
				PlayerPrefs.SetInt ("is_fb_logged_in", 0);
			}
			isFbLoggedIn = PlayerPrefs.GetInt ("is_fb_logged_in") == 1;
			return isFbLoggedIn;
		}

		set{ 
			isFbLoggedIn = value;
			PlayerPrefs.SetInt ("is_fb_logged_in", value ? 1 : 0);
		}
	}

	public void SetBuggyAcc(int num, float val){

        GameData.data.vehicles[num].acceleration.val = val;
         
        //PlayerPrefs.SetFloat (BuggyAcc + num.ToString(), val);

    }

	public float GetBuggyAcc(int num){
     //   Debug.Log("GetBuggyAcc " + num + "   " + GameData.data.vehicles[num].acceleration.val);
        return GameData.data.vehicles[num].acceleration.val;
        //return PlayerPrefs.GetFloat (BuggyAcc + num.ToString());

    }
		

	public void SetBuggyGreenAcc(int num, float val){

        GameData.data.vehicles[num].acceleration.nextVal = val;
        //  PlayerPrefs.SetFloat (BuggyGreenAcc + num.ToString(), val);
         
    }

	public float GetBuggyGreenAcc(int num){

        return GameData.data.vehicles[num].acceleration.nextVal;
       // return PlayerPrefs.GetFloat (BuggyGreenAcc + num.ToString());

	}

	public void SetBuggyStrength(int num, float val){

        GameData.data.vehicles[num].speed.val = val;
        // PlayerPrefs.SetFloat (BuggyStrength + num.ToString(), val);
         
    }

	public float GetBuggyStrength(int num){

        return GameData.data.vehicles[num].speed.val;

       // return PlayerPrefs.GetFloat (BuggyStrength + num.ToString());
	}

	public void SetBuggyGreenStrength(int num, float val){

        GameData.data.vehicles[num].speed.nextVal = val;
        //  PlayerPrefs.SetFloat (BuggyGreenStrength + num.ToString(), val);
         
    }

	public float GetBuggyGreenStrength(int num){

        return GameData.data.vehicles[num].speed.nextVal;
      //  return PlayerPrefs.GetFloat (BuggyGreenStrength + num.ToString());
	}

	public void SetBuggyHandling(int num, float val){

         GameData.data.vehicles[num].handling.val = val;
        // PlayerPrefs.SetFloat (BuggyHandling + num.ToString(), val);
         
    }

	public float GetBuggyHandling(int num){

        return GameData.data.vehicles[num].handling.val;
       // return PlayerPrefs.GetFloat (BuggyHandling + num.ToString());
	}

	public void SetBuggyGreenHandling(int num, float val){

         GameData.data.vehicles[num].handling.nextVal = val;
        // PlayerPrefs.SetFloat (BuggyGreenHandling + num.ToString(), val);
         
    }

	public float GetBuggyGreenHandling(int num){

        return GameData.data.vehicles[num].handling.nextVal;
        //return PlayerPrefs.GetFloat (BuggyGreenHandling + num.ToString());
	}

	public void SetBuggyUpgrade(int num, int val){

        GameData.data.vehicles[num].upgraded = val;
        // PlayerPrefs.SetInt (NoOfCarUpgrades + num.ToString(), val);
         
    }

	public int GetBuggyUpgrade(int num){
       
        return GameData.data.vehicles[num].upgraded;

       // return PlayerPrefs.GetInt (NoOfCarUpgrades + num.ToString(),0);
	}

	public void SetBuggyUpgradeText(int num, int val){


		PlayerPrefs.SetInt (BuggyUpgradeText + num.ToString(), val);
         
    }

	public int GetBuggyUpgradeText(int num){


		return PlayerPrefs.GetInt (BuggyUpgradeText + num.ToString(),500);
	}


	public void SetBuggyUnlock(int num, int val){
        GameData.data.vehicles[num].isUnlocked = val;

        //  PlayerPrefs.SetInt (Buggies + num.ToString(), val);
         
    }

	public int GetBuggyUnlock(int num){
     //   Debug.Log("GetBuggyUnlock " + num + "   " + GameData.data.vehicles[num].isUnlocked);
        return GameData.data.vehicles[num].isUnlocked;


      //  return PlayerPrefs.GetInt (Buggies + num.ToString(),0);
	}

	public void SetBuggySelected(int val){


		PlayerPrefs.SetInt (BuggiesSelected, val);
	}

	public int GetBuggySelected(){


		return PlayerPrefs.GetInt (BuggiesSelected,0);
	}

	public void SetBoySkin(int num, int val){

        GameData.data.characters[0].skinsLocked[num] = val;
         
        //PlayerPrefs.SetInt (BoySkin + num.ToString(), val);
    }

	public int GetBoySkin(int num){

        return GameData.data.characters[0].skinsLocked[num];
		//return PlayerPrefs.GetInt (BoySkin + num.ToString(),0);
	}

	public void SetGirlSkin(int num, int val){

        GameData.data.characters[1].skinsLocked[num] = val;
         
        // PlayerPrefs.SetInt (GirlSkin + num.ToString(), val);
    }

	public int GetGirlSkin(int num){

        return GameData.data.characters[1].skinsLocked[num];
       // return PlayerPrefs.GetInt (GirlSkin + num.ToString(),0);
	}

	public void SetCharacterSelected(int val){


		PlayerPrefs.SetInt (CharacterSelected, val);
	}

	public int GetCharacterSelected(){


		return PlayerPrefs.GetInt (CharacterSelected,0);
	}

	public void SetUnlockTrack(int num, int val){

        GameData.data.environments[0].tracks[num-1].isUnlocked = val;
     
        //PlayerPrefs.SetInt (TrackNumber + num.ToString(), val);
    }

	public int GetUnlockTrack(int num){
       // Debug.Log(num);
        return GameData.data.environments[0].tracks[num-1].isUnlocked;

       // return PlayerPrefs.GetInt (TrackNumber + num.ToString(),0);
	}

	public void SetTrackSelected(int val){


		PlayerPrefs.SetInt (SelectedTrack, val);
	}

	public int GetTrackSelected(){


		return PlayerPrefs.GetInt (SelectedTrack,1);
	}


	public void SetUnlockDesertTrack(int num, int val){

        GameData.data.environments[1].tracks[num-1].isUnlocked = val;
      
        // PlayerPrefs.SetInt (TrackNumberDesert + num.ToString(), val);
    }

	public int GetUnlockDesertTrack(int num){
      //  Debug.Log(num);
        return GameData.data.environments[1].tracks[num-1].isUnlocked;


       // return PlayerPrefs.GetInt (TrackNumberDesert + num.ToString(),0);
	}

	public void SetDesertTrackSelected(int val){


		PlayerPrefs.SetInt (SelectedTrackDesert, val);
	}

	public int GetDesertTrackSelected(){

		return PlayerPrefs.GetInt (SelectedTrackDesert,1);
	}


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
//	void Update () {
//
//	}
}
