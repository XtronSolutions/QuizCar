using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public TextMeshProUGUI QuestionTitle;
    public TextMeshProUGUI QuestionText;
    public Image QuestionImage;
    public Button PlayAudioBtn;
    public Button TrueBtn;
    public Button FalseBtn;
    public TextMeshProUGUI TextFeedback;
    bool IsCorrect = false;

    [HideInInspector]
    public QuestionData MainData;

    [HideInInspector]
    public PowerUpManager PowerInstance;

    public void ButtonEvents()
    {
        TrueBtn.onClick.AddListener(OnAnswerButtonPressed_True);
        FalseBtn.onClick.AddListener(OnAnswerButtonPressed_False);
    }

    public void RemoveEvents()
    {
        TrueBtn.onClick.RemoveAllListeners();
        FalseBtn.onClick.RemoveAllListeners();
    }

    public void OnAnswerButtonPressed_True()
    {
        if(IsCorrect)
        {
            //win
            TextFeedback.text = "CORRECT";
        }
        else
        {
            //lose
            TextFeedback.text = "WRONG";
        }

        Invoke("DestoryAndContinue", 1.5f);
    }

    public void OnAnswerButtonPressed_False()
    {
        if(IsCorrect)
        {
            //lose
            TextFeedback.text = "WRONG";
        }
        else
        {
            //win
            TextFeedback.text = "CORRECT";
        }

        Invoke("DestoryAndContinue", 1.5f);
    }

    public void DestoryAndContinue()
    {
        Controls.IsHandBrake = false;
        Controls.StartEngine = true;
        PowerInstance.QuestionSelected = false;
        Time.timeScale = 1;
        RemoveEvents();
        Destroy(this.gameObject);
    }

    public void ResetObjects()
    {
        QuestionTitle.text = "QUESTION";
        ToggleQuestionText(false);
        ToggleQuestionImage(false);
        ToggleQuestionAudio(false);
        ToggleButtons(false);
        TextFeedback.text = "";
    }

    public void ToggleButtons(bool _state)
    {
        TrueBtn.gameObject.SetActive(_state);
        FalseBtn.gameObject.SetActive(_state);
    }

    public void ToggleQuestionText(bool _state)
    {
        QuestionText.gameObject.SetActive(_state);
    }

    public void ToggleQuestionImage(bool _state)
    {
        QuestionImage.gameObject.SetActive(_state);
    }

    public void ToggleQuestionAudio(bool _state)
    {
        PlayAudioBtn.gameObject.SetActive(_state);
    }

    public void SetInformation()
    {
        ResetObjects();

        if (MainData.containTextQuestion)
        {
            ToggleQuestionText(true);
            QuestionText.text = MainData.questionText;
        }

        if (MainData.containAutioQuestion)
        {
            ToggleQuestionAudio(true);
        }

        if (MainData.containBoolTrueFalseAnswer)
        {
            ToggleButtons(true);
            ButtonEvents();
            Debug.Log(MainData.select);

            if (MainData.select.ToLower() == "true")
                IsCorrect = true;
            else
                IsCorrect = false;
        }

        if (MainData.containTextAnswer)
        {
            ToggleButtons(true);
            Debug.Log(MainData.textAnswer);
        }

        if (MainData.containImageQuestion)
        {
            ToggleQuestionImage(true);
            StartCoroutine(GetTexture(MainData.questionImageUrl));
        }

    }

    IEnumerator GetTexture(string _url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
        Sprite _sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);

        QuestionImage.sprite = _sprite;
    }

    void Start()
    {
        
    }
}
