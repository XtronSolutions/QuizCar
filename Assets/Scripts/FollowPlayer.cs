using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour
{
    GameObject Player;
    // Use this for initialization
    IEnumerator Start()
    {
        while (Player == null)
        {
            yield return null;
            Player = GameObject.FindGameObjectWithTag(Constants.TAG_PLAYER_CAR);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
            gameObject.transform.position = new Vector3(Player.transform.position.x, gameObject.transform.position.y, Player.transform.position.z);
    }
}