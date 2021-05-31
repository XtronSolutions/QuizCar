using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenusUI : MonoBehaviour {


    public GameObject WelcomeMenu,SignUpMenu,LoginMenu, MainMenu, progressScreen, leaderBoard;
    public Popup popup;

    public static MenusUI Intance;
	// Use this for initialization

	void Start () {
       
        Intance = this;

        if (PlayfabManager.isLoggedIn)
        {
            PlayfabManager.Instance.LoadFriendsLeaderboard();
            PlayfabManager.Instance.LoadWorldLeaderboard();
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}