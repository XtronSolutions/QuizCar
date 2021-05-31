using UnityEngine;
using System.Collections;
[System.Serializable]
public struct ArrowGroup
{
   public  GameObject[] Arrows;
};

public class ArrowsOnOff : MonoBehaviour {

    public ArrowGroup[] arrowGroups;
    
	// Use this for initialization
	void Start () {
        foreach (ArrowGroup AG in arrowGroups)
        {
            StartCoroutine(BlinkArrows(AG));
        }
	}
	
    IEnumerator BlinkArrows(ArrowGroup AG)
    {

        int arrows = AG.Arrows.Length;
        int OnArrow = 0;
        while (true)
        {

            for (int i = 0; i < AG.Arrows.Length; i++)
            {
                if (i == OnArrow)
                    AG.Arrows[i].SendMessage("TurnLightOn");
                else
                    AG.Arrows[i].SendMessage("TurnLightOff"); ;
               

            }
            yield return new WaitForSeconds(0.5f);
            OnArrow++;
            if (OnArrow == arrows)
                OnArrow = 0;
        }
        yield return null;
        
    }
	// Update is called once per frame
	void Update () {
	
	}
}