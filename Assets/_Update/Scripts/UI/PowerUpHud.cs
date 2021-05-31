using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PowerUpHud : MonoBehaviour {

    // Use this for initialization

    public Text trapButtonText, trapOilText, trapBombText, trapIceText;  // 3,4,0,1

    public GameObject trapBtnTimer, trapOilTimer, trapBombTimer, trapIceTimer;
	void OnEnable () {


        CheckTrapStatus();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TrapPress(int i)
    {
        if (isTimerActive()) return;



        if(i==3)
        {
            if (!trapBtnTimer.activeSelf)
            {
                GameData.data.trapButton--;
                trapBtnTimer.SetActive(true);
            }
            else
            {
                return;
            }
        }
        else if(i==4)
        {
            if (!trapOilTimer.activeSelf)
            {
                GameData.data.trapOil--;
                trapOilTimer.SetActive(true);
            }
            else
            {
                return;
            }
        }
        else if(i==0)
        {
            if (!trapBombTimer.activeSelf)
            {
                GameData.data.trapBomb--;
                trapBombTimer.SetActive(true);
            }
            else
            {
                return;
            }
        }
        else if(i==1)
        {
            if (!trapIceTimer.activeSelf)
            {
                GameData.data.trapIce--;
                trapIceTimer.SetActive(true);
            }
            else
            {
                return;
            }
        }
        trapIceTimer.SetActive(true);
        trapBombTimer.SetActive(true);
        trapOilTimer.SetActive(true);
        trapBtnTimer.SetActive(true);
        CheckTrapStatus();

        PlayerManagerScript.instance.DeployTrapFromHud(i);

        GameData.data.Save();


    }

    void CheckTrapStatus()
    {
        trapButtonText.text = GameData.data.trapButton + "";
        trapOilText.text = GameData.data.trapOil + "";
        trapBombText.text = GameData.data.trapBomb + "";
        trapIceText.text = GameData.data.trapIce + "";
        if (GameData.data.trapButton <= 0)
            trapButtonText.transform.parent.gameObject.SetActive(false);
        else
            trapButtonText.transform.parent.gameObject.SetActive(true);

        if (GameData.data.trapOil <= 0)
            trapOilText.transform.parent.gameObject.SetActive(false);
        else
            trapOilText.transform.parent.gameObject.SetActive(true);

        if (GameData.data.trapBomb <= 0)
            trapBombText.transform.parent.gameObject.SetActive(false);
        else
            trapBombText.transform.parent.gameObject.SetActive(true);

        if (GameData.data.trapIce <= 0)
            trapIceText.transform.parent.gameObject.SetActive(false);
        else
            trapIceText.transform.parent.gameObject.SetActive(true);
    }
    public void TimerOver(int index)
    {

    }

    bool isTimerActive()
    {
        if (GameData.data.trapButton > 0 && trapBtnTimer.activeSelf)
            return true;

        if (GameData.data.trapBomb > 0 && trapBombTimer.activeSelf)
            return true;

        if (GameData.data.trapIce > 0 && trapIceTimer.activeSelf)
            return true;

        if (GameData.data.trapOil > 0 && trapOilTimer.activeSelf)
            return true;

        //if (trapBtnTimer.activeSelf || trapIceTimer.activeSelf || trapOilTimer.activeSelf || trapBombTimer.activeSelf)
        //    return true;
        return false;
    }
}