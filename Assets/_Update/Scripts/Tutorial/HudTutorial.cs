using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class HudTutorial : MonoBehaviour {
    bool isTutorial;
 
  //  public UIParts[] uiElements;
    public GameObject[] msgs;
    public GameObject nextBtn;
    public GameObject blank;
    public GameObject black;
    int i = 0;
    // Use this for initialization
    void OnEnable() {
        i = 0;
        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            isTutorial = true;
            blank.SetActive(true);
            nextBtn.SetActive(true);
            black.SetActive(true);
            //SetAllGray();
          //  for (int k = 0; k < uiElements[i].images.Length; k++)
         ///       uiElements[i].images[k].color = Color.white;
            msgs[i].SetActive(true);
            //  Time.timeScale = 0;

            GameObject.Find("GeneralMessage").GetComponent<Text>().text = "";
        }
        else
            isTutorial = false;


    }

    // Update is called once per frame
    public void Next() {

        msgs[i].SetActive(false);
       // SetAllGray();

        i++;


        if (i < msgs.Length)
        {
           // for(int k=0;k<uiElements[i].images.Length;k++)
           // uiElements[i].images[k].color = Color.white;
            msgs[i].SetActive(true);
        }
        else
        {

            TutorialFinish();
        }

    }

    void SetAllGray()
    {

        //foreach (UIParts img in uiElements)
        //{
        //    foreach (Image im in img.images)
        //    {
        //        im.color = Color.gray;
        //    }
        //}
    }
    void TutorialFinish()
    {

        //foreach (UIParts img in uiElements)
        //{
        //    foreach (Image im in img.images)
        //    {
        //        im.color = Color.white;
        //    }
        //}
        blank.SetActive(false);
        nextBtn.SetActive(false);
        black.SetActive(false);
        Time.timeScale = 1;
    }

    [Serializable]
    public class UIParts
    {
        public Image []images;
    }
}