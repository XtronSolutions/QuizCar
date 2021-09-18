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
    private readonly float TimeDelay = 1.5f;
    private readonly float CoroutineDelay = 2.0f;
    bool IsCorrect = false;
    bool IsPlaying = false;
    bool StartPowerAction = false;
    AudioClip myClip;

    [HideInInspector]
    public QuestionData MainData;

    [HideInInspector]
    public PowerUpManager PowerInstance;

    private void OnEnable()
    {
        iTween.ScaleTo(this.gameObject, iTween.Hash("scale", Vector3.one, "time", 0.7f, "easetype", iTween.EaseType.easeOutBounce, "oncompletetarget", this.gameObject,
            "oncomplete", "OnScaleUp"));

        Source = this.gameObject.GetComponent<AudioSource>();
    }

    public void OnScaleUp()
    {

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

    public IEnumerator PerformPowerUpAction(float sec, bool _isCorrect)
    {
        Debug.Log(_isCorrect+" "+ sec);
        yield return new WaitUntil(()=> StartPowerAction==true);
        Debug.Log("here");
        if (_isCorrect)
        {
            PowerInstance.NitroPressed();
        }else
        {
            PowerInstance.BoulderActive();
        }

        iTween.ScaleTo(this.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.3f, "easetype", iTween.EaseType.easeInBounce, "oncompletetarget", this.gameObject,
            "oncomplete", "OnScaleDown"));

    }

    public void OnScaleDown()
    {
        Destroy(this.gameObject);
    }

    public void CorrectAnswer()
    {
        TextFeedback.text = GameData.GetLocalizaedText("Text_Correct"); //correct
        Color col = Color.green;
        TextFeedback.color = col;
        PlaySound(1);
    }

    public void WrongAsnwer()
    {
        TextFeedback.text = GameData.GetLocalizaedText("Text_Wrong"); //wrong
        Color col = Color.red;
        TextFeedback.color = col;
        PlaySound(0);
    }

    public void OnAnswerButtonPressed_True()
    {
        if(IsCorrect)//win
            CorrectAnswer();
        else
            WrongAsnwer(); //lose

        StartPowerAction = false;
        Invoke("DestoryAndContinue", TimeDelay);
        StartCoroutine(PerformPowerUpAction(CoroutineDelay, IsCorrect));
        
    }

    public void OnAnswerButtonPressed_False()
    {
        if(IsCorrect)
            WrongAsnwer();//lose
        else
            CorrectAnswer();//win

        StartPowerAction = false;
        Invoke("DestoryAndContinue", TimeDelay);
        StartCoroutine(PerformPowerUpAction(CoroutineDelay, !IsCorrect));
    }

    public void DestoryAndContinue()
    {
        GlobalVariables.isPause = false;
        Controls.IsHandBrake = false;
        Controls.StartEngine = true;
        PowerInstance.QuestionSelected = false;
        Time.timeScale = 1;
        RemoveEvents();
        StartPowerAction = true;
    }

    public void ResetObjects()
    {
        QuestionTitle.text = GameData.GetLocalizaedText("Text_Question"); //Question
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
