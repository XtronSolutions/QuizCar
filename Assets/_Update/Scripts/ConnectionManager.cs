using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;

[System.Serializable]
public class QuickMatchData
{
    public GameObject MainScreen;
    public GameObject PhotonScriptInstance;
}

[System.Serializable]
public class CreateMatchData
{
    public GameObject MainScreen;
    public GameObject PhotonScriptInstance;
}

[System.Serializable]
public class JoinMatchData
{
    public GameObject MainScreen;
    public GameObject PhotonScriptInstance;
    public InputField CodeInput;
    public Text ConnectingText;
    public GameObject CodeObject;
    public Text PlayerCount;

}

public class ConnectionManager : MonoBehaviour
{
    private string Code;
    public static ConnectionManager Instance;
    public GameObject LoadingScreen;
    public QuickMatchData QuickData;
    public CreateMatchData CreateData;
    public JoinMatchData JoinData;
    public bool isTest=false;
    // Start is called before the first frame update
    void OnEnable()
    {
        Instance=this;
        // if(isTest)
        // {
        //    CreateRoomWithCode();
        //    return;
        // }

        if(GameData.isQuickMatch)
        {
            QuickData.MainScreen.SetActive(true);
            QuickData.PhotonScriptInstance.SetActive(true);
            LoadingScreen.SetActive(false);
        }
        else
        {
            if(GameData.isCreateRoom)
                CreateRoomWithCode();
            else
                JoinRoomViaCode(); 
        }
    }

    public void CreateRoomWithCode()
    {
        CreateData.MainScreen.SetActive(true);
        CreateData.PhotonScriptInstance.SetActive(true);
        Constants.joinCode="";
        CreateData.PhotonScriptInstance.GetComponent<ConnectAndJoinPrivateRoom>().ConnectNow();
    }

    public void ResetJoinData()
    {
        JoinData.PlayerCount.text="0";
        JoinData.ConnectingText.gameObject.SetActive(false);
        JoinData.CodeObject.SetActive(true);
        JoinData.CodeInput.text="";
        Constants.joinCode="";
    }
    
    public void JoinRoomViaCode()
    {
        JoinData.MainScreen.SetActive(true);
        ResetJoinData();
        JoinData.PhotonScriptInstance.SetActive(true);
        LoadingScreen.SetActive(false);
    }

    public void OnJoinRoom_Clicked()
    {
        Code=JoinData.CodeInput.text;
        if(Code!="")
        {
            Constants.joinCode=Code;
            JoinData.PhotonScriptInstance.GetComponent<ConnectAndJoinPrivateRoom>().ConnectNow();
            LoadingScreen.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
