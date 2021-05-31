using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

    public GameObject LoginPanel, LoggedInPannel;
    public Text coinText;
    public RawImage profileImage;
    public Text playerNameText;

    public Transform playerinfo;
	// Use this for initialization
	void OnEnable () {

        LoginPanel.SetActive(!PlayfabManager.isLoggedIn);
        LoggedInPannel.SetActive(PlayfabManager.isLoggedIn);

        if(PlayfabManager.isLoggedIn)
        {
            profileImage.texture = FBManager.Instance.profileImage;
            playerNameText.text = PlayfabManager.PlayerName;
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (PlayfabManager.isLoggedIn)
        {
           
            profileImage.texture = FBManager.Instance.profileImage;
            playerNameText.text = PlayfabManager.PlayerName;
        }
        profileImage.gameObject.SetActive(FBManager.IsLoggedIn);

        if(FBManager.IsLoggedIn)
        {
            playerinfo.localPosition = new Vector3(170, playerinfo.localPosition.y, 0);
        }
        else
        {
            playerinfo.localPosition = new Vector3(125, playerinfo.localPosition.y, 0);
        }

        LoginPanel.SetActive(!PlayfabManager.isLoggedIn);
        LoggedInPannel.SetActive(PlayfabManager.isLoggedIn);
        coinText.text = GameData.data.coins + "";
    }

    public void OnLoginPress()
    {
        GameObject.FindObjectOfType<MenusUI>().LoginMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void OnLogoutPress()
    {
        PlayfabManager.Instance.Logout();
    }

    public void LeaderboardPress()
    {
        this.gameObject.SetActive(false);
        GameObject.FindObjectOfType<MenusUI>().leaderBoard.SetActive(true);
    }
}