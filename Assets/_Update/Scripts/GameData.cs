﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MiniJSON;
using Newtonsoft.Json;

public class PlayerDataSave
{
    public string email;
    public string pass;
}

    public class GameData : MonoBehaviour
{
    public static int selectedEnvironment;  //0 for forest, 1 for desert
    public static int trackNo;
    public static int selectedCar;
    public static bool signedIn = false;

    public static int controlsType = 2; //0  for tilt, 1 for arrows and brakes on both corner, 2 for arrow on left  corner and braks on right corner
    public static float steeringSensitivity = 2;
    // Use this for initialization

    public static bool isDay = true;
    public static Data data = new Data();

    public static bool isSound = true;
    public static bool isMusic = true;
    public static bool isTutorial = false;
    public static bool isQuickMatch=false;
    public static bool isCreateRoom=false;
    
    const string CONTROLSKEY = "controls";
    const string SOUNDKEY = "sound";
    const string MUSICKEY = "music";
    public const string TUTORIALKEY = "tutorial";
    public static string PlayerDataKey = "PlayerData";
    public static string PlayerGoogleDataKey = "GoogleData";
    public static string PlayerFBDataKey = "fBData";
    public static string LanguageSelection = "LanguageSelection";

    public static void SetSavePlayerData(string email, string pass)
    {
        PlayerDataSave _dataSave = new PlayerDataSave();
        _dataSave.email = email;
        _dataSave.pass = pass;

        var JsonString = JsonConvert.SerializeObject(_dataSave);
        PlayerPrefs.SetString(GameData.PlayerDataKey, JsonString);
    }

    public static string GetSavePlayerData()
    {
        return PlayerPrefs.GetString(GameData.PlayerDataKey,"");
    }

    public static void SetGoogleData()
    {
        PlayerPrefs.SetString(GameData.PlayerGoogleDataKey, "stored");
    }

    public static string GetGoogleData()
    {
        return PlayerPrefs.GetString(GameData.PlayerGoogleDataKey, "");
    }

    public static void SetLanguageData(int _ind)
    {
        PlayerPrefs.SetInt(GameData.LanguageSelection, _ind);
    }

    public static int GetLanguageData()
    {
        return PlayerPrefs.GetInt(GameData.LanguageSelection);
    }

    public static void SetFBData()
    {
        PlayerPrefs.SetString(GameData.PlayerFBDataKey, "stored");
    }

    public static string GetFBData()
    {
        return PlayerPrefs.GetString(GameData.PlayerFBDataKey, "");
    }

    public static string GetLocalizaedText(string _id)
    {
        int LangIndex = GameData.GetLanguageData();
        string _msg = CVSParser.GetTextFromId(_id, LangIndex);
        return _msg;
    }

    public static void DeletePrefData()
    {
        PlayerPrefs.DeleteKey(GameData.CONTROLSKEY);
        PlayerPrefs.DeleteKey(GameData.SOUNDKEY);
        PlayerPrefs.DeleteKey(GameData.MUSICKEY);
        PlayerPrefs.DeleteKey(GameData.TUTORIALKEY);
        PlayerPrefs.DeleteKey(GameData.PlayerDataKey);
        PlayerPrefs.DeleteKey(GameData.PlayerGoogleDataKey);
        PlayerPrefs.DeleteKey(GameData.PlayerFBDataKey);
    }

    public static void PushData()
    {
        PlayfabManager.Instance.SetPlayerStatistics();
        PlayfabManager.Instance.PushData();
    }
    public static void SaveSettings()
    {
        PlayerPrefs.SetInt(CONTROLSKEY, controlsType);
        if (isSound)
            PlayerPrefs.SetInt(SOUNDKEY, 1);
        else
            PlayerPrefs.SetInt(SOUNDKEY, 0);

        if (isMusic)
            PlayerPrefs.SetInt(MUSICKEY, 1);
        else
            PlayerPrefs.SetInt(MUSICKEY, 0);
    }
    public static void LoadSettings()
    {
        isSound = false;
        isMusic = false;
        isTutorial = false;
        controlsType = PlayerPrefs.GetInt(CONTROLSKEY, 2);
        if (PlayerPrefs.GetInt(SOUNDKEY, 1) == 1)
            isSound = true;
        if (PlayerPrefs.GetInt(MUSICKEY, 1) == 1)
            isMusic = true;

        if (PlayerPrefs.GetInt(TUTORIALKEY, 1) == 1)
            isTutorial = true;

    }

    [Serializable]
    public class Data
    {
        public int score;
        public int coins;
        public Vehicles[] vehicles = new Vehicles[4];
        public Character[] characters = new Character[2];
        public Environment[] environments = new Environment[2];
        public int trapButton;
        public int trapOil;
        public int trapBomb;
        public int trapIce;
        public Data()
        {
          //  PlayerPrefs.DeleteAll();
            LoadSettings();
            //

            for (int i = 0; i < vehicles.Length; i++)
            {
                vehicles[i] = new Vehicles();
            }

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = new Character();
                for (int k = 0; k < characters[i].skinsLocked.Length; k++)
                {
                    characters[i].skinsLocked[k] = new int();
                }
            }

            for (int i = 0; i < environments.Length; i++)
                environments[i] = new Environment();


            Load(PlayerPrefs.GetString("data", ""));

            if (coins == 0)
                coins = 1200;

        }
        public void Logout()
        {
            PlayerPrefs.SetString("data", "");
            data = null;
            data = new Data();
            data.vehicles[0].isUnlocked = 1;
        }
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public void Load(string json)
        {
            string str = json;
            if (str.Length < 2)
                return;

            JSONObject jb = JSONObject.Create(str);

            score = int.Parse(GetVal("score", jb));
            coins = int.Parse(GetVal("coins", jb));

            trapButton = int.Parse(GetVal("trapButton", jb));
            trapOil = int.Parse(GetVal("trapOil", jb));
            trapBomb = int.Parse(GetVal("trapBomb", jb));
            trapIce = int.Parse(GetVal("trapIce", jb));

            JSONObject v = JSONObject.Create(jb.GetField("vehicles").ToString());
            for (int i = 0; i < vehicles.Length; i++)
            {
                vehicles[i].isUnlocked = int.Parse(GetVal("isUnlocked", v[i]));
                vehicles[i].upgraded = int.Parse(GetVal("upgraded", v[i]));
                JSONObject acc = JSONObject.Create(v[i].GetField("acceleration").ToString());
                vehicles[i].acceleration.val = float.Parse(GetVal("val", acc));
                vehicles[i].acceleration.nextVal = float.Parse(GetVal("nextVal", acc));

                JSONObject speed = JSONObject.Create(v[i].GetField("speed").ToString());
                vehicles[i].speed.val = float.Parse(GetVal("val", speed));
                vehicles[i].speed.nextVal = float.Parse(GetVal("nextVal", speed));

                JSONObject handling = JSONObject.Create(v[i].GetField("handling").ToString());
                vehicles[i].handling.val = float.Parse(GetVal("val", handling));
                vehicles[i].handling.nextVal = float.Parse(GetVal("nextVal", handling));
            }
            JSONObject ch = JSONObject.Create(jb.GetField("characters").ToString());
            for (int i = 0; i < characters.Length; i++)
            {
                JSONObject sk = JSONObject.Create(ch[i].GetField("skinsLocked").ToString());
                for (int j = 0; j < characters[i].skinsLocked.Length; j++)
                {
                    characters[i].skinsLocked[j] = int.Parse(sk[j].ToString());
                }
            }

            JSONObject en = JSONObject.Create(jb.GetField("environments").ToString());
            for (int i = 0; i < environments.Length; i++)
            {
                environments[i].isUnlocked = int.Parse(GetVal("isUnlocked", en[i]));
                JSONObject tr = JSONObject.Create(en[i].GetField("tracks").ToString());
                for (int j = 0; j < environments[i].tracks.Length; j++)
                {
                    environments[i].tracks[j].isUnlocked = int.Parse(GetVal("isUnlocked", tr[j]));
                }
            }

        }
        string GetVal(string name, JSONObject json)
        {

            return json.GetField(name).ToString().Replace("\"", "");

        }
        public void Save()
        {
            //  Debug.Log("Save");
            Debug.Log(data.ToJson());
            PlayerPrefs.SetString("data", data.ToJson());
            PlayerPrefs.Save();
            PushData();
        }
        public void SaveLocal()
        {
            PlayerPrefs.SetString("data", data.ToJson());
            PlayerPrefs.Save();
        }
        public void UnlockAll()
        {
            for (int i = 0; i < vehicles.Length; i++)
                vehicles[i].isUnlocked = 1;

            for (int i = 0; i < environments.Length; i++)
            {
                environments[i].isUnlocked = 1;
                for (int j = 0; j < environments[i].tracks.Length; j++)
                    environments[i].tracks[j].isUnlocked = 1;
            }

            for (int i = 0; i < characters.Length; i++)
            {

                for (int j = 0; j < characters[i].skinsLocked.Length; j++)
                    characters[i].skinsLocked[j] = 1;
            }


        }

        public bool isUnlockAll()
        {
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i].isUnlocked != 1)
                    return false;
            }

            for (int i = 0; i < environments.Length; i++)
            {
                // environments[i].isUnlocked = 1;
                for (int j = 0; j < environments[i].tracks.Length; j++)
                    if (environments[i].tracks[j].isUnlocked != 1)
                        return false;
            }

            for (int i = 0; i < characters.Length; i++)
            {

                for (int j = 0; j < characters[i].skinsLocked.Length; j++)
                    if (characters[i].skinsLocked[j] != 1)
                        return false;
            }

            return true;


        }

        public bool Buy(int price)
        {
            if (coins >= price)
            {
                coins -= price;
                return true;
            }
            return false;
        }
        [Serializable]
        public class Vehicles
        {
            public int isUnlocked;
            public int upgraded;
            public Upgrade acceleration = new Upgrade();
            public Upgrade speed = new Upgrade();
            public Upgrade handling = new Upgrade();

        }
        [Serializable]
        public class Upgrade
        {
            public float val;
            public float nextVal;
            public void RoundValues()
            {
                val = Truncate(val, 2);
                nextVal = Truncate(nextVal, 2);
            }

        }
        public static float Truncate(float value, int digits)
        {
            double mult = Math.Pow(10.0, digits);
            double result = Math.Truncate(mult * value) / mult;
            return (float)result;
        }
        [Serializable]
        public class Character
        {
            public int[] skinsLocked = new int[4];
        }
        [Serializable]
        public class Environment
        {
            public int isUnlocked;
            public Track[] tracks = new Track[6];

            public Environment()
            {
                for (int i = 0; i < tracks.Length; i++)
                    tracks[i] = new Track();
            }
        }
        [Serializable]
        public class Track
        {
            public int isUnlocked;
        }

    }
}