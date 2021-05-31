using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StoreUI : MonoBehaviour {

    public Text coinsText;
    public GameObject coinsActiveBtn, coinsDisableBtn, powerupActiveBtn, powerupDisableBtn, otherActiveBtn, otherDisableBtn;
    public GameObject powerupsPanel, coinsPanel, otherPanel;
    public Button adBtn;

    public GameObject unlockAllPkg, alreadyUnlockedPanel;


    public Text trapButtonText, trapOilText, trapBombText, trapIceText;  // 3,4,0,1
                                                                         // Use this for initialization
    public Text adText;

    public GameObject []powerUpBuyButtons;
    void OnEnable () {
        ShowCoins();

        bool unlk = GameData.data.isUnlockAll();
       if(unlk)
        {
            alreadyUnlockedPanel.SetActive(true);
            unlockAllPkg.SetActive(false);
        }
       else
        {
            alreadyUnlockedPanel.SetActive(false);
            unlockAllPkg.SetActive(true);
        }

        CheckTrapStatus();
	}
	
	// Update is called once per frame
	void Update () {
        adBtn.interactable = AdsManager.Instance.isRewardedAdLoaded;
        if(adBtn.interactable)
        {
            adText.text = "Watch AD";
        }
        else
        {
            adText.text = "Loading AD";
        }
        coinsText.text = GameData.data.coins + "";


	}
    public void Close()
    {
        GameData.data.Save();
    }
    public void ShowCoins()
    {

        coinsActiveBtn.SetActive(true);
        coinsDisableBtn.SetActive(false);
        powerupActiveBtn.SetActive(false);
        powerupDisableBtn.SetActive(true);
        otherActiveBtn.SetActive(false);
        otherDisableBtn.SetActive(true);

        powerupsPanel.SetActive(false);
        coinsPanel.SetActive(true);
        otherPanel.SetActive(false);
    }

    public void ShowPowerups()
    {
        coinsActiveBtn.SetActive(false);
        coinsDisableBtn.SetActive(true);
        powerupActiveBtn.SetActive(true);
        powerupDisableBtn.SetActive(false);
        otherActiveBtn.SetActive(false);
        otherDisableBtn.SetActive(true);

        powerupsPanel.SetActive(true);
        coinsPanel.SetActive(false);
        otherPanel.SetActive(false);
    }

    public void ShowOther()
    {
        coinsActiveBtn.SetActive(false);
        coinsDisableBtn.SetActive(true);
        powerupActiveBtn.SetActive(false);
        powerupDisableBtn.SetActive(true);
        otherActiveBtn.SetActive(true);
        otherDisableBtn.SetActive(false);

        powerupsPanel.SetActive(false);
        coinsPanel.SetActive(false);
        otherPanel.SetActive(true);


    }
    public void WatchAd()
    {
        AdsManager.Instance.ShowRewardedVideoAd(OnAdFinished);
       
    }

    void OnAdFinished()
    {
        GameData.data.coins += 50;
    }
    int poweruptobuy = -1;
    public void BuyPowerUp(int index)
    {
        if (GameData.data.coins >= 50)
        {
            poweruptobuy = index;
            MenusUI.Intance.popup.ShowPopup("Confirm your purchase!", OnBuyPowerups);
        }
        else
        {
            MenusUI.Intance.popup.ShowPopup("You don't have enough coins!", null);
        }
    }
    void OnBuyPowerups(bool status)
    {
        if (!status) return;
        if (GameData.data.Buy(50))
        {
            if (poweruptobuy == 0)
                GameData.data.trapButton = 1;
            else if (poweruptobuy == 1)
                GameData.data.trapOil = 1;
            else if (poweruptobuy == 2)
                GameData.data.trapBomb = 1;
            else if (poweruptobuy == 3)
                GameData.data.trapIce = 1;

            CheckTrapStatus();
        }
    }
    public void BuyCoins(string pkg)
    {
        ShopManager.Instance.PurchasePackage(pkg, OnPurchesed);
    }
    public void BuyAll()
    {
        ShopManager.Instance.PurchasePackage("4", OnPurchesed);
    }

    void OnPurchesed(string id)
    {
        if(id == "0")
        {
            GameData.data.coins += 100;
        }
        else if (id == "1")
        {
            GameData.data.coins += 500;
        }
        else if (id == "2")
        {
            GameData.data.coins += 1000;
        }
        else if (id == "3")
        {
            GameData.data.coins += 5000;
        }
        else if (id == "4")
        {
            GameData.data.UnlockAll();

            alreadyUnlockedPanel.SetActive(true);
            unlockAllPkg.SetActive(false);
        }
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


        powerUpBuyButtons[0].SetActive(!trapButtonText.transform.parent.gameObject.activeSelf);
        powerUpBuyButtons[1].SetActive(!trapOilText.transform.parent.gameObject.activeSelf);
        powerUpBuyButtons[2].SetActive(!trapBombText.transform.parent.gameObject.activeSelf);
        powerUpBuyButtons[3].SetActive(!trapIceText.transform.parent.gameObject.activeSelf);
    }
}