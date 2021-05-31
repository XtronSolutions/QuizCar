using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LeaderboardMenuListner : MonoBehaviour
{
    public ScoreBoard worldScores, friendsScores;
    public GameObject LoginBtn;

    public GameObject friendBtn, friendsBtnPress;
    public GameObject worldBtn, worldBtnPress;
    // Start is called before the first frame update
    void OnEnable()
    {
        FriendBtnPress();
        if (!PlayfabManager.isLoggedIn)
        {
            friendsScores.RemoveAllRecords();
            worldScores.RemoveAllRecords();

        }

    }

    // Update is called once per frame
    void Update()
    {

        LoginBtn.SetActive(!PlayfabManager.isLoggedIn);

    }

    public void Close()
    {
        //  SoundsManager.Instance.PlayButtonSound();
        GameObject.FindObjectOfType<MenusUI>().MainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void OnLoginPress()
    {
        GameObject.FindObjectOfType<MenusUI>().LoginMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void FriendBtnPress()
    {
        worldScores.gameObject.SetActive(false);
        friendsScores.gameObject.SetActive(true);
        friendsBtnPress.SetActive(true);
        friendBtn.gameObject.SetActive(false);

        worldBtnPress.SetActive(false);
        worldBtn.SetActive(true);
    }

    public void GlobalBtnPress()
    {
        worldScores.gameObject.SetActive(true);
        friendsScores.gameObject.SetActive(false);
        friendsBtnPress.SetActive(false);
        friendBtn.gameObject.SetActive(true);

        worldBtnPress.SetActive(true);
        worldBtn.SetActive(false);
    }
  
  
}