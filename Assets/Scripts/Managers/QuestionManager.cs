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
    public AudioClip[] Responses; //0 is wrong, 1 is corect
    AudioSource Source;
    bool IsCorrect = false;
    bool IsPlaying = false;
    AudioClip myClip;

    [HideInInspector]
    public QuestionData MainData;

    [HideInInspector]
    public PowerUpManager PowerInstance;

    private void OnEnable()
    {
        Source = this.gameObject.GetComponent<AudioSource>();
    }

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

    public void PlaySound(int index)
    {
        Source.Stop();
        Source.clip = Responses[index];
        Source.Play();
    }

    public void CorrectAnswer()
    {
        TextFeedback.text = "CORRECT";
        Color col = Color.green;
        TextFeedback.color = col;
        PlaySound(1);
    }

    public void WrongAsnwer()
    {
        TextFeedback.text = "WRONG";
        Color col = Color.red;
        TextFeedback.color = col;
        PlaySound(0);
    }

    public void OnAnswerButtonPressed_True()
    {
        if(IsCorrect)
        {
            //win
            CorrectAnswer();
        }
        else
        {
            //lose
            WrongAsnwer();
        }

        Invoke("DestoryAndContinue", 1.5f);
    }

    public void OnAnswerButtonPressed_False()
    {
        if(IsCorrect)
        {
            //lose
            WrongAsnwer();
        }
        else
        {
            //win
            CorrectAnswer();
        }

        Invoke("DestoryAndContinue", 1.5f);
    }

    public void DestoryAndContinue()
    {
        GlobalVariables.isPause = false;
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

            if (PlayAudioBtn.transform.childCount > 0)
                PlayAudioBtn.transform.GetChild(0).gameObject.SetActive(true);

            StartCoroutine(GetAudio(MainData.audioQuestionUrl));
        }

        if (MainData.containBoolTrueFalseAnswer)
        {
            ToggleButtons(true);
            ButtonEvents();
           // Debug.Log(MainData.select);

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

            if (QuestionImage.transform.childCount > 0)
                QuestionImage.transform.GetChild(0).gameObject.SetActive(true);

            StartCoroutine(GetTexture(MainData.questionImageUrl));
        }

    }

    IEnumerator GetAudio(string _url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result== UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                PlayAudioBtn.GetComponent<AudioSource>().clip = myClip;

                if (PlayAudioBtn.transform.childCount > 0)
                    PlayAudioBtn.transform.GetChild(0).gameObject.SetActive(false);

                //audioSource.clip = myClip;
                //audioSource.Play();
                //Debug.Log("Audio is playing.");
            }
        }
    }

    IEnumerator GetTexture(string _url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url);
        yield return www.SendWebRequest();

        Texture2D myTexture = DownloadHandlerTexture.GetContent(www);
        Sprite _sprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero);

        QuestionImage.sprite = _sprite;

        if (QuestionImage.transform.childCount > 0)
            QuestionImage.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PlayAudioToggle()
    {
        if (!IsPlaying)
            PlayAudio();
        else
            StopAudio();

        IsPlaying = !IsPlaying;
    }

    public void PlayAudio()
    {
        PlayAudioBtn.GetComponent<AudioSource>().Play();
    }

    public void StopAudio()
    {
        PlayAudioBtn.GetComponent<AudioSource>().Stop();
    }

    private void OnDisable()
    {
        StopAudio();
        IsPlaying = false;
    }

    private void OnDestroy()
    {
        StopAudio();
        IsPlaying = false;
    }


}
