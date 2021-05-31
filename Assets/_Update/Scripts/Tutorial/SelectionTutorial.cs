using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTutorial : MonoBehaviour {

    public GameObject buggyBG, characterBG , nextBG;
    public GameObject buggyHand, characterHand, nextHand;
    public GameObject buggyBtn, characterBtn , nextBtn;
    int i = 0;
    // Use this for initialization
    void OnEnable()
    {
        buggyBG.SetActive(false);
        buggyHand.SetActive(false);
        characterBG.SetActive(false);
        characterHand.SetActive(false);

        nextBG.SetActive(false);
        nextHand.SetActive(false);

        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            if(i==0)
            {
                buggyBG.SetActive(true);
                buggyHand.SetActive(true);
                buggyBtn.transform.SetAsLastSibling();
                buggyBG.transform.SetSiblingIndex(buggyBtn.transform.GetSiblingIndex() - 1);
                i = 1;
            }
            else if(i==1)
            {
                characterBG.SetActive(true);
                characterHand.SetActive(true);
                characterBtn.transform.SetAsLastSibling();
                characterBG.transform.SetSiblingIndex(characterBtn.transform.GetSiblingIndex() - 1);
                i = 2;

            }
            else if (i == 2)
            {
                nextBG.SetActive(true);
                nextHand.SetActive(true);
                nextBtn.transform.SetAsLastSibling();
                nextBG.transform.SetSiblingIndex(nextBtn.transform.GetSiblingIndex() - 1);
                i = 3;

            }
        }
    }

}