using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizationText : MonoBehaviour
{
    public enum LocaleID
    {
        Default,
        Title =49,
        Button_Guest=1,
        Button_SignIn=2,
        Button_SignUp=3,
        Button_SignOut=4,
        Button_LoginWith=5,
        Placeholder_Email=6,
        Placeholder_Password=7,
        Button_Back=8,
        Placeholder_Name=9,
        Placeholder_ConfirmPassword=10,
        Placeholder_Number=11,
        Text_Welcome=12,
        Button_Singleplayer=13,
        Button_Multiplayer=14,
        Button_QuickMatch=15,
        Text_PlayFirends=16,
        Button_CreateMatch=17,
        Button_JoinMatch=18,
        Text_PlayerSearching=19,
        Text_NoPlayerAvailable=20,
        Button_Home=21,
        Text_RaceStart=22,
        Text_Seconds=23,
        Text_PlayerWaiting=24,
        Text_PrivateCode=25,
        Button_ShareCode=26,
        Button_StartGame=27,
        Text_PlayersConnected=28,
        Placeholder_EnterCode=29,
        Button_Join=30,
        Text_Ready=31,
        Text_Go=32,
        Text_Place=33,
        Text_Lap=34,
        Text_WrongWay=35,
        Text_Pause=36,
        Text_Question=37,
        Text_True=38,
        Text_False=39,
        Text_Correct=40,
        Text_Wrong=41,
        Text_RaceCompleted=42,
        Text_1st=43,
        Text_2nd=44,
        Text_3rd=45,
        Text_4th=46,
        Text_5th=47,
        Text_6th=48,
        Toast_EnterEmail=50,
        Toast_ValidEmail=51,
        Toast_EnterPass=52,
        Toast_ProcessCancelled=53,
        Toast_ValidEmailPass=54,
        Toast_GoogleCancelled=55,
        Toast_EnterName=56,
        Toast_ConfirmPass=57,
        Toast_PassMismatch=58,
        Toast_EnterNumber=59,
        Toast_WentWrong=60,
        Toast_OK=61,
        Toast_Oops=62,
        Text_SharePrivateLinkMessage=63,
        Text_SharePrivateLinkMessage2=64,
        Toast_InvalidCode=65,
        Text_PlayAudio=66,
        Text_Bounds=67
    };

    public LocaleID KeyID = LocaleID.Default;
    public bool IsTextMeshPro = false;

    public void ChangeLanguage(int index)
    {
        if(IsTextMeshPro)
            gameObject.GetComponent<TextMeshProUGUI>().text = CVSParser.GetTextFromId(KeyID.ToString(), index);
        else
            gameObject.GetComponent<Text>().text = CVSParser.GetTextFromId(KeyID.ToString(), index);
    }

    public void OnEnable()
    {
        int LangIndex = GameData.GetLanguageData();
        ChangeLanguage(LangIndex);

        LanguageDropDown.ChangeLanguage += ChangeLanguage;
    }

    public void OnDisable()
    {
        LanguageDropDown.ChangeLanguage -= ChangeLanguage;
    }
}
