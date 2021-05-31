using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using PlayFab.ClientModels;
public class ScoreBoard : MonoBehaviour
{
   
    public ScoreBar scoreBarPrefab;
    public Transform grid;

    List<GameObject> scoresbars = new List<GameObject>();
    public enum Leaderboardtype { Friends, world };
    public Leaderboardtype type;
    // Start is called before the first frame update
    void OnEnable()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetScoreBar(int pos, string name, int score, string id)
    {
        GameObject bar = Instantiate(scoreBarPrefab.gameObject, grid);
        bar.transform.localScale = Vector3.one;
        bar.GetComponent<ScoreBar>().SetData(pos, name, score,type,id);
        bar.SetActive(true);
        scoresbars.Add(bar);
    }

    public void UpdateBoard(GetLeaderboardResult result)
    {
        foreach (GameObject go in scoresbars.ToArray())
        {
            scoresbars.Remove(go); // else there will be pointer to null
            GameObject.Destroy(go);
        }
        scoresbars.Clear();

       
        //for (int i=0;i<result.Leaderboard.Count;i++)
        //{
            
        //    SetScoreBar(result.Leaderboard[i].Position, result.Leaderboard[i].DisplayName, result.Leaderboard[i].StatValue, result.Leaderboard[i].PlayFabId);
        //}
    }
    public void RemoveAllRecords()
    {
        foreach (GameObject go in scoresbars.ToArray())
        {
            scoresbars.Remove(go); // else there will be pointer to null
            GameObject.Destroy(go);
        }
        scoresbars.Clear();
    }
}