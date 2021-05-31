using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuTutorial : MonoBehaviour {

    public GameObject bg1, bg2;
    public GameObject btn1, btn2;
    public GameObject hand1, hand2;

    public GameObject a1, a2, a3;
    int i = 0;
    // Use this for initialization
    void OnEnable()
    {
        a1.SetActive(false);
      
        a3.SetActive(false);

        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            if (PlayerPrefs.GetInt("mainkey", 1) == 1)
            {
                PlayerPrefs.SetInt("mainkey", 0);
                bg1.SetActive(true);
                btn1.SetActive(true);
                hand1.SetActive(true);

                bg1.transform.SetSiblingIndex(30);
                btn1.transform.SetSiblingIndex(31);
                hand1.transform.SetSiblingIndex(32);

            }
            else
            {
                if (i == 0)
                {
                    bg2.SetActive(true);
                    btn2.SetActive(true);
                    hand2.SetActive(true);

                    bg2.transform.SetSiblingIndex(30);
                    btn2.transform.SetSiblingIndex(31);
                    hand2.transform.SetSiblingIndex(32);
                    i++;
                }
                else
                {

                    bg2.SetActive(false);

                    hand2.SetActive(false);

                    a1.SetActive(true);
                    a2.SetActive(true);
                    a3.SetActive(true);

                    a1.transform.SetSiblingIndex(30);
                    a2.transform.SetSiblingIndex(31);
                    a3.transform.SetSiblingIndex(32);

                    PlayerPrefs.SetInt(GameData.TUTORIALKEY, 0);
                }

            }
        }
        else
        {

        }
    }
}