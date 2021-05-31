using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerCustomization : MonoBehaviour {

	public Material[] BoySkins;
	public Material[] GirlSkins;

	public Image[] SkinsImage;

	public Sprite[] BoyImages;
	public Sprite[] GirlImages;

	public int[] BoySkinCost;
	public int[] GirlSkinCost;

	public SkinnedMeshRenderer BoyMeshRendrer;
	public SkinnedMeshRenderer GirlMeshRendrer;

	public GameObject BuyButtonLock;
	public GameObject BuyButtonUnlock;

	public Text LockText;

	public Text coinsText;

	private int CharacterPos = 0; // 0 for boy 1 for girl
	private int BoySelectedSkin = 0;
	private int GirlSelectedSkin = 0;

	public Transform highlightedSprite;
	public Transform[] skinButtons;

	public Text selectedText;

	// Use this for initialization
	void Start () {
        //		PlayerPrefs.DeleteAll ();
        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) != 1)
        {
            //  RewardProperties.Instance.SetBoySkin(0, 1);
            //  CharacterPos = RewardProperties.Instance.GetCharacterSelected();
            // UpdateBuyButton(RewardProperties.Instance.sk)
            BuyButtonLock.SetActive(false);
            BuyButtonUnlock.SetActive(true);

        }
        coinsText.text = RewardProperties.Instance.Coin.ToString ();

        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickSkin1(int SkinNumber){
	
		if (CharacterPos == 0) { // for boy
		
			BoyMeshRendrer.material = BoySkins [SkinNumber];
			BoySelectedSkin = SkinNumber;
		} else if (CharacterPos == 1) { // for girl
		
			GirlMeshRendrer.material = GirlSkins [SkinNumber];
			GirlSelectedSkin = SkinNumber;
		}

		UpdateBuyButton (SkinNumber);
		UpdateBuyButtonText (SkinNumber);
		highlightedSprite.SetParent (skinButtons [SkinNumber]);
		highlightedSprite.localPosition = Vector3.zero;
		highlightedSprite.SetAsFirstSibling ();
	}

	void UpdateBuyButton(int skin_num){
	
		if (CharacterPos == 0) {

			if (RewardProperties.Instance.GetBoySkin (skin_num) == 0) {

				BuyButtonLock.SetActive (true);
				BuyButtonUnlock.SetActive (false);
			} 
			else if (RewardProperties.Instance.GetBoySkin (skin_num) == 1) {
			
				BuyButtonLock.SetActive (false);
				BuyButtonUnlock.SetActive (true);
			}
		} else if (CharacterPos == 1) { 

			if (RewardProperties.Instance.GetGirlSkin(skin_num) == 0) {

				BuyButtonLock.SetActive (true);
				BuyButtonUnlock.SetActive (false);
			}
			else if (RewardProperties.Instance.GetGirlSkin (skin_num) == 1) {

				BuyButtonLock.SetActive (false);
				BuyButtonUnlock.SetActive (true);
			}
		}

		if (RewardProperties.Instance.GetCharacterSelected () == CharacterPos) {
			selectedText.text = "SELECTED";
		} else {
			selectedText.text = "SELECT";
		}
	}

	void UpdateBuyButtonText(int skin_num){
	
		if (CharacterPos == 0) { 

			LockText.text = BoySkinCost [skin_num].ToString();
				
		} 
		else if (CharacterPos == 1) { 

			LockText.text = GirlSkinCost [skin_num].ToString();
		}	
	}

	public void BuySkin(){

	
		if (CharacterPos == 0) {
			
			if (RewardProperties.Instance.Coin > BoySkinCost [BoySelectedSkin]) {
				
				RewardProperties.Instance.SetBoySkin (BoySelectedSkin, 1);
				RewardProperties.Instance.SetCharacterSelected (0);
				UpdateBuyButton (BoySelectedSkin);
				RewardProperties.Instance.Coin -= BoySkinCost [BoySelectedSkin];
				coinsText.text = RewardProperties.Instance.Coin.ToString ();
			}
		} else if (CharacterPos == 1) { 

			if (RewardProperties.Instance.Coin > GirlSkinCost [GirlSelectedSkin]) {
				
				RewardProperties.Instance.SetGirlSkin (GirlSelectedSkin, 1);
				RewardProperties.Instance.SetCharacterSelected (1);
				UpdateBuyButton (GirlSelectedSkin);
				RewardProperties.Instance.Coin -= GirlSkinCost [GirlSelectedSkin];
				coinsText.text = RewardProperties.Instance.Coin.ToString ();
			}
		}
	}

	public void OnClickArrowButton(){
	
		if (CharacterPos == 0) {
			
			CharacterPos = 1;
			OnClickSkin1 (GirlSelectedSkin);
			for (int i = 0; i < SkinsImage.Length; i++) {

				SkinsImage [i].sprite = GirlImages [i]; 
			}
		} 
		else {
			
			CharacterPos = 0;
			OnClickSkin1 (BoySelectedSkin);

			for (int i = 0; i < SkinsImage.Length; i++) {

				SkinsImage [i].sprite = BoyImages [i]; 
			}
		}
	}

	public void SelectCharacter(){
		RewardProperties.Instance.SetCharacterSelected (CharacterPos);
		UpdateBuyButton (0);
	}

}
