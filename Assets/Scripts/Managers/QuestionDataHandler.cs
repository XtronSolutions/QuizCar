using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionDataHandler : MonoBehaviour
{
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI AnswerText;
    public Image QuestionImage;
    public Button PlayButton;
    public GameObject QuestionPanel;

    public int Counter = 0;

    public static QuestionDataHandler Instance;
    public 
    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;
        ResetVariables();
    }

    public void ResetVariables()
    {
        QuestionText.gameObject.SetActive(false);
        AnswerText.gameObject.SetActive(false);
        QuestionImage.gameObject.SetActive(false);
        PlayButton.gameObject.SetActive(false);
    }

    public void ToggleQuestionPanel(bool state)
    {
        QuestionPanel.SetActive(state);
    }

    public void NextQuestion()
    {
        if(Counter< FirebaseManager.Instance.DataQuestions.Count-1)
        {
            Counter++;
            SetInformation(Counter);
        }
        else
        {

        }
    }

    public void PreviousQuestion()
    {
        if (Counter > 0)
        {
            Counter--;
            SetInformation(Counter);
        }
        else
        {

        }
    }

    public void CloseWindow()
    {
        ToggleQuestionPanel(false);
    }

    public void OpenQuestions()
    {
        ToggleQuestionPanel(true);
        Counter = 0;
        SetInformation(Counter);

    }

    public void SetInformation(int _index)
    {
        ResetVariables();

        QuestionData _data =FirebaseManager.Instance.DataQuestions[_index];

        if (_data.containTextQuestion)
        {
            QuestionText.gameObject.SetActive(true);
            QuestionText.text = _data.questionText;
        }

        if(_data.containAutioQuestion)
        {
            PlayButton.gameObject.SetActive(true);
        }

        if (_data.containBoolTrueFalseAnswer)
        {
            AnswerText.gameObject.SetActive(true);
            Debug.Log(_data.select);
            AnswerText.text = _data.select;
        }

        if (_data.containTextAnswer)
        {
            AnswerText.gameObject.SetActive(true);
            AnswerText.text = _data.textAnswer;
        }

        if (_data.containImageQuestion)
        {
            QuestionImage.gameObject.SetActive(true);
            StartCoroutine(GetTexture(_data.questionImageUrl));
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
