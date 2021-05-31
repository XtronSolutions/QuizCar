using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class coinhud : MonoBehaviour {

    public Text coinText;
    int i = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update() {
        this.transform.Translate(Vector3.up * Time.deltaTime * 250);

        if (this.transform.localPosition.y > 350)
        {
            i = 0;
            this.gameObject.SetActive(false);
        }
	}
    public void StartAnim()
    {
        i++;
        this.transform.localPosition = Vector3.zero;
        this.gameObject.SetActive(true);
        coinText.text = "+" + i;

    }
}
