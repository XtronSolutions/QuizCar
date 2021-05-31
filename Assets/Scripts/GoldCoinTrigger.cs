using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GoldCoinTrigger : MonoBehaviour {

	public MoveHurdle collectAnimation;
    public GameObject[] objs;

    DesertUIManager desertUIManager;
    HUD_Script uD_Script;
    // Use this for initialization
    void Start () {
		Constants.coinsCollected = 0;
        desertUIManager =  GameObject.FindObjectOfType<DesertUIManager>();
        uD_Script = GameObject.FindObjectOfType<HUD_Script>();
        //  collectAnimation = GameObject.FindObjectOfType<DesertUIManager>().coinImage.GetComponent<MoveHurdle>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider _hit)
	{
		if (_hit.CompareTag ("Player")) {
            if (PlayerManagerScript.instance.isMine(_hit.transform.root.gameObject) )
            {
                //  Debug.Log(_hit.name);
                // iTween.Stop(desertUIManager.coinImage);
               
                desertUIManager.coinImage.transform.localPosition = Vector3.zero;

                // collectAnimation = desertUIManager.coinImage.GetComponent<MoveHurdle>();
                // collectAnimation.gameObject.SetActive(true);
                // collectAnimation.Restart();
                desertUIManager.coinImage.GetComponent<coinhud>().StartAnim();
                // collectAnimation.startAnimDisableOnComplete();
                RewardProperties.Instance.Coin += 1;
                Constants.coinsCollected+=1;
                _hit.transform.root.GetComponent<PlayerAudioManager>().PlayCoinPickUp();
              //  uD_Script.coinText.text = "+" + Constants.coinsCollected;
                //Destroy(this.gameObject);
            }

            this.GetComponent<BoxCollider>().enabled = false;
            foreach (GameObject g in objs)
                g.SetActive(false);

            Invoke("Respawn", 4);

        }
        else if(_hit.transform.root.CompareTag("AI"))
        {
            this.GetComponent<BoxCollider>().enabled = false;
            foreach (GameObject g in objs)
                g.SetActive(false);

            Invoke("Respawn", 4);
        }
	}

    void Respawn()
    {
        this.GetComponent<BoxCollider>().enabled = true;
        foreach (GameObject g in objs)
            g.SetActive(true);
    }
}
