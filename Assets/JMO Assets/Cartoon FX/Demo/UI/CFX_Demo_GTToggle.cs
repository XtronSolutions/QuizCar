using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Cartoon FX - (c) 2014 - Jean Moreno
//
// Script for the Demo scene

public class CFX_Demo_GTToggle : MonoBehaviour
{
	public Texture2D Normal, Hover;
	public Color NormalColor = new Color32(128,128,128,128), DisabledColor = new Color32(128,128,128,48);
	public bool State = true;
	
	public string Callback;
	public GameObject Receiver;
	
	private Rect CollisionRect;
	private bool Over;
	private Text Label;
	
	//-------------------------------------------------------------
	
	void Awake()
	{
		//CollisionRect = this.GetComponent<Image>().GetScreenRect(Camera.main);
		Label = this.GetComponentInChildren<Text>();
		
		UpdateTexture();
	}
	
	void Update ()
	{
        //if(CollisionRect.Contains(Input.mousePosition))
        if(true)
        {
			Over = true;
			if(Input.GetMouseButtonDown(0))
			{
				OnClick();
			}
		}
		else
		{
			Over = false;
			this.GetComponent<Image>().color = NormalColor;
		}
		
		UpdateTexture();
	}
	
	//-------------------------------------------------------------
	
	private void OnClick()
	{
		State = !State;
		
		Receiver.SendMessage(Callback);
	}
	
	private void UpdateTexture()
	{
		Color col = State ? NormalColor : DisabledColor;
        Sprite mainhover = Sprite.Create(Hover, new Rect(0.0f, 0.0f, Hover.width, Hover.height), new Vector2(0.5f, 0.5f), 100.0f);
        Sprite mainNormal = Sprite.Create(Normal, new Rect(0.0f, 0.0f, Normal.width, Normal.height), new Vector2(0.5f, 0.5f), 100.0f);
        if (Over)
        {
            this.GetComponent<Image>().sprite = mainhover;
        }
        else
            this.GetComponent<Image>().sprite = mainNormal;

        this.GetComponent<Image>().color = col;
		
		if(Label != null)
			Label.color = col * 1.75f;
	}
}
