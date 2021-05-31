using UnityEngine;
using System.Collections;

public class SceneSwitcher : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("SwitchScene", 1f);
	}

    void SwitchScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
	// Update is called once per frame
	void Update () {
	
	}
}