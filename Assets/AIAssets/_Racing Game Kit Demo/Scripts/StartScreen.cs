

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

 
public class StartScreen : MonoBehaviour
{
    public Texture2D SprintRace;
    public Texture2D CircuitRace;
    public Texture2D TimeTrialRace;
    public Texture2D KnockOutRace;
    public Texture2D SpeedTrapRace;
    public Texture2D LoadingScreen;
    public Texture2D RGKLogo;

    private float iScreenWidth = 0;
    private float iScreenHeigt = 0;
    
    
    void Start()
    {
        iScreenWidth = Screen.width;
        iScreenHeigt = Screen.height;
    

   }

    private void Update()
    {
        iScreenWidth = Screen.width;
        iScreenHeigt = Screen.height;
    
    }

    bool isLoading = false;
    void OnGUI()
    {
        float ime = (iScreenWidth / 5) - 100;
        GUI.DrawTexture(new Rect(iScreenWidth / 2 - 150, 50, 300, 196), RGKLogo, ScaleMode.StretchToFill, true, 1f);

        GUI.BeginGroup(new Rect(ime, 300, 800, 160));
        int ibo = 0;
        if (GUI.Button(new Rect(ibo, 0, 150, 150), SprintRace))
        {
            isLoading = true;
            StartCoroutine(LoadLevel("Demo-Sprint"));
        }
        ibo = ibo + 160;
        if(GUI.Button(new Rect(ibo, 0, 150, 150), CircuitRace))
        {
            isLoading = true;
            StartCoroutine(LoadLevel("Demo-Circuit"));
        }
        ibo = ibo + 160;
        if (GUI.Button(new Rect(ibo, 0, 150, 150), TimeTrialRace))
        {
            isLoading = true;
            StartCoroutine(LoadLevel("Demo-TimeAttack"));
        }
        ibo = ibo + 160;
        if (GUI.Button(new Rect(ibo, 0, 150, 150), KnockOutRace))
        {
            isLoading = true;
            StartCoroutine(LoadLevel("Demo-KNockout"));
        }
        ibo = ibo + 160;
        if (GUI.Button(new Rect(ibo, 0, 150, 150), SpeedTrapRace))
        {
            isLoading = true;
            StartCoroutine(LoadLevel("Demo-SpeedTrap"));
        }
        GUI.EndGroup();

        if (isLoading)
        {
            GUI.DrawTexture(new Rect(0, 0, iScreenWidth, iScreenHeigt), LoadingScreen, ScaleMode.StretchToFill, true, 1);
        }
    }

    IEnumerator LoadLevel(String Trackname)
    {
        
        AsyncOperation async = Application.LoadLevelAsync(Trackname);
        yield return async;
        Debug.Log("Loading complete");
    }
     


}
