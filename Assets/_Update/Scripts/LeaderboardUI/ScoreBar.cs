using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScoreBar : MonoBehaviour
{
    public Image badgeImage;
    public Text positionText, nameText, scoreText;
    public Image image;
    public Sprite normalSprite;
    public Sprite playerSprite;
    public RawImage avater;

    public Sprite []badges;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetData(int pos, string name, int score , ScoreBoard.Leaderboardtype type, string id)
    {
        //if (type == ScoreBoard.Leaderboardtype.world)
        //{
        //    if (pos == PlayfabManager.Instance.PlayerRank)
        //    {
        //        image.sprite = playerSprite;
        //    }
        //    else
        //    {
        //        image.sprite = normalSprite;
        //    }
        //}
        //else if (type == ScoreBoard.Leaderboardtype.Friends)
        //{
        //    if (pos == PlayfabManager.Instance.PlayerInFriendsRank)
        //    {
        //        image.sprite = playerSprite;
        //    }
        //    else
        //    {
        //        image.sprite = normalSprite;
        //    }
        //}
    
        if(pos<3)
        {
            badgeImage.gameObject.SetActive(true);
            badgeImage.sprite = badges[pos];
            positionText.gameObject.SetActive(false);
        }
        if(id== PlayfabManager.PlayFabID)
        {
            image.sprite = playerSprite;
        }
        else
        {
            image.sprite = normalSprite;
        }
        positionText.text = (pos+1)+"";
        nameText.text = name;
        scoreText.text = score+"";
    }
}