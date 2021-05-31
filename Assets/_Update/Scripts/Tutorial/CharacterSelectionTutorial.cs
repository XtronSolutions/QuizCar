using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectionTutorial : MonoBehaviour {

    bool isTutorial;

    public GameObject  unlockBG, unlockHand, backBG, backHand;
    public GameObject  unlocbtn, backBtn;

    public Text text;
	// Use this for initialization
	void OnEnable () {



        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
        {
            isTutorial = true;
        }
        else
        isTutorial = false;

        if(isTutorial)
        {
            unlockBG.gameObject.SetActive(true);
            unlockHand.gameObject.SetActive(true);
            backBG.gameObject.SetActive(false);
            backHand.gameObject.SetActive(false);


            backBtn.transform.SetSiblingIndex(18);
            unlockBG.transform.SetSiblingIndex(19);
            unlocbtn.transform.SetSiblingIndex(20);
            unlockHand.transform.SetSiblingIndex(21);

            text.text = "100";
        }
        else
        {
            unlockBG.gameObject.SetActive(false);
            unlockHand.gameObject.SetActive(false);
            backBG.gameObject.SetActive(false);
            backHand.gameObject.SetActive(false);

        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnUnlockPress()
    {
        if (isTutorial)
        {
            unlockBG.gameObject.SetActive(false);
            unlockHand.gameObject.SetActive(false);
            backBG.gameObject.SetActive(true);
            backHand.gameObject.SetActive(true);


            backBG.transform.SetSiblingIndex(19);
            backBtn.transform.SetSiblingIndex(20);
            backHand.transform.SetSiblingIndex(21);
        }
    }
    public void OnBackPress()
    {
        if (isTutorial)
        {
            unlockBG.gameObject.SetActive(false);
            unlockHand.gameObject.SetActive(false);
            backBG.gameObject.SetActive(false);
            backHand.gameObject.SetActive(false);

        }
    }
}