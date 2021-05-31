using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpManager : MonoBehaviour 
{
	public enum powerUpType
	{
		NITRO,
		SHIELD,
		WEAPON
	};

//	public GameObject[] powerUps = new GameObject[3];

	public GameObject randomizorCube;
	public Image deployButton;
	public Sprite nitroSprite;
	public Sprite bombSprite;
	public Sprite iceCubeSprite;
	public Sprite trapSprite;
	public Sprite oilSpilSprite;
	public Sprite rocketSprite;
	public GameObject aimIcon;

    public GameObject QuestionPrefab;
    public GameObject UIParent;
	private GameObject selectedButton;
	private int powerGot;

	[HideInInspector]
	public int selectedPowerUp;

	public static PowerUpManager Instance;

    [HideInInspector]
    public bool QuestionSelected = false;

    public GameObject black, blank, tutorialmsg;
	void OnEnable(){
		Instance = this;
	}

	void Start()
	{
//		powerUps [0].GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);		// Temp only for Nitro
//		powerUps [0].GetComponent<Button>().interactable = false;

		randomizorCube.SetActive (false);
		deployButton.gameObject.SetActive (false);

	}

	public void ActivatePowerUp()
	{
//		powerGot = Random.Range (0, powerUps.Length);
//		powerUps [powerGot].SetActive (true);
	}

	public void ActivateSpecificPowerUp(powerUpType type)
	{
		switch(type)
		{
		case powerUpType.NITRO:
			Debug.Log ("NITRO");
			PlayerManagerScript.instance.addNitro (1);
//			selectedButton = powerUps [0];
			AnimateButton (selectedButton);
//			powerUps [0].GetComponent<Image> ().color = new Color (1, 1, 1, 1f);
//			powerUps [0].GetComponent<Button>().interactable = true;
			break;

		case powerUpType.SHIELD:
			Debug.Log ("SHIELD");
			break;

		case powerUpType.WEAPON:
			Debug.Log ("WEAPON");
			break;
		}
	}


	public void NitroPressed()
	{
		PlayerManagerScript.instance.addNitro (1);
		Nitro ();
//		powerUps [0].GetComponent<Button>().interactable = false;
//		powerUps [0].GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
	}

	private void Nitro()
	{  
		if (PlayerManagerScript.instance._vehicleHandling.turboLocked) 
		{
		}
//			PlayerManagerScript.instance._vehicleHandling.EndTurbo ();

		else {
//			powerUps [0].GetComponent<Button>().interactable = false;
//			powerUps [0].GetComponent<Image> ().color = new Color (1, 1, 1, 0.5f);
			if (PlayerManagerScript.instance.isEmpty)
				OnEmptyNitro ();
			else {
				PlayerManagerScript.instance._vehicleHandling.StartTurbo ();
			}
		}
	}

	// If nitro is empty then open popUp for no nitro
	public	void OnEmptyNitro()
	{
		Debug.Log ("Plaese fill the Nitro to boost your speed .... (:  ");
		if (!GlobalVariables.isPause) 
		{
			PlayerManagerScript.instance.addNitro (1); 
		}
	}


	private void AnimateButton(GameObject _btn)
	{
		iTween.ScaleTo (_btn, iTween.Hash("scale", new Vector3(1.3f, 1.3f, 1.3f), "time", 0.1f, "easetype", iTween.EaseType.linear,
			"oncomplete", "AnimateButtonCallback", "oncompletetarget", this.gameObject));
	}
	private void AnimateButtonCallback()
	{
		iTween.ScaleTo (selectedButton, iTween.Hash ("scale", new Vector3 (1f, 1f, 1f), "time", 0.5f, "easetype", iTween.EaseType.easeOutBounce));
	}

	public void SelectRandomPowerUp(){
		deployButton.gameObject.SetActive (false);
		randomizorCube.SetActive (true);
		Invoke ("RandomPowerUpSelected", 2.3f);
	}

 
    private void RandomQuestionSelected()
    {
        if (!QuestionSelected)
        {
            QuestionSelected = true;
            if (FirebaseManager.Instance.DataQuestionsLocal.Count > 0)
            {
                int _index = Random.Range(0, FirebaseManager.Instance.DataQuestionsLocal.Count);
                QuestionData _data = FirebaseManager.Instance.DataQuestionsLocal[_index];
                FirebaseManager.Instance.DataQuestionsLocal.RemoveAt(_index);

                GameObject QuestionObj = Instantiate(QuestionPrefab, UIParent.transform);
                QuestionObj.GetComponent<QuestionManager>().MainData = _data;
                QuestionObj.GetComponent<QuestionManager>().PowerInstance = this;
                QuestionObj.GetComponent<QuestionManager>().SetInformation(); 
            }
            else
            {
                Time.timeScale = 1;
                Debug.Log("no question remains");
            }
        }
    }
    public void SelectRandomQuestion()
    {
        Invoke("RandomQuestionSelected", 0.8f);
    }

	void RandomPowerUpSelected(){
		selectedPowerUp = Random.Range (0, 6);
     //  selectedPowerUp = 1;
		switch (selectedPowerUp) {
		case 0:
			deployButton.sprite = bombSprite;
			break;

		case 1:
			deployButton.sprite = iceCubeSprite;
			aimIcon.SetActive (true);
			break;

		case 2:
			deployButton.sprite = nitroSprite;
			break;

		case 3:
			deployButton.sprite = trapSprite;
			break;

		case 4:
			deployButton.sprite = oilSpilSprite;
			break;

		case 5:
			deployButton.sprite = rocketSprite;
			aimIcon.SetActive (true);
			break;
		}

		deployButton.gameObject.SetActive (true);
		randomizorCube.SetActive (false);

        if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1 && !istutorialshown)
        {
            istutorialshown = true;
            Time.timeScale = 0;
            black.SetActive(true);
            blank.SetActive(true);
            tutorialmsg.SetActive(true);
        }
    }
    bool istutorialshown = false;
    public void DisableTutoril()
    {
        Time.timeScale = 1;
        black.SetActive(false);
        blank.SetActive(false);
        tutorialmsg.SetActive(false);
    }
}
