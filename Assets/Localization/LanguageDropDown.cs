using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LanguageDropDown : MonoBehaviour
{
    static public Action<int> ChangeLanguage;
    public Dropdown dropDown;
    public Text Label;
    
    public void LanguageChanged()
    {
        if (ChangeLanguage != null)
            ChangeLanguage(dropDown.value);

        dropDown.captionText.text = CVSParser.GetAvailableLanguages()[dropDown.value];

        GameData.SetLanguageData(dropDown.value);
        //Debug.LogError("Setting lang: " + dropDown.value);
        Label.text = dropDown.captionText.text;
    }

    public void OnEnable()
    {
        int LangIndex = GameData.GetLanguageData();

        dropDown.ClearOptions();
        dropDown.AddOptions(CVSParser.GetAvailableLanguages());

        dropDown.value = LangIndex;
 
       // Debug.LogError(LangIndex);
        LanguageChanged();
    }
}
