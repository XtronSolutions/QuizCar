using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

    public int[] pos;
    public RectTransform arrow;
    public Transform musicImage, soundImage;
	// Use this for initialization
	void OnEnable () {
        UpdatePos();

        if(GameData.isSound)
        {
            soundImage.localPosition = new Vector3(35, soundImage.localPosition.y, 0);
        }
        else
        {
            soundImage.localPosition = new Vector3(-45, soundImage.localPosition.y, 0);
        }


        if (GameData.isMusic)
        {
            musicImage.localPosition = new Vector3(35, musicImage.localPosition.y, 0);
        }
        else
        {
            musicImage.localPosition = new Vector3(-45, musicImage.localPosition.y, 0);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    void UpdatePos()
    {
        arrow.localPosition = new Vector3(arrow.localPosition.x, pos[GameData.controlsType], 0);
    }
    public void ChangeContol(int control)
    {
        GameData.controlsType = control;
        UpdatePos();
        GameData.SaveSettings();
    }

    public void Toggle(Transform t)
    {
        if(t.localPosition.x==-45)
        {
            t.localPosition = new Vector3(35, t.localPosition.y, 0);
        }
        else
        {
            t.localPosition = new Vector3(-45, t.localPosition.y, 0);
        }
    }
    public void SoundToggle()
    {
        GameData.isSound = !GameData.isSound;
        SFX[] sfx = GameObject.FindObjectsOfType<SFX>();
        foreach (SFX s in sfx)
            s.UpdateState();

        GameData.SaveSettings();
    }
    public void MusicToggle()
    {
        GameData.isMusic = !GameData.isMusic;
        Music[] sfx = GameObject.FindObjectsOfType<Music>();
        foreach (Music s in sfx)
            s.UpdateState();

        GameData.SaveSettings();
    }
}