using UnityEngine;
using System.Collections;

public class RG_LevelManager : MonoBehaviour {

	private string gameMode;
	public GameObject openWorldManager;
	public GameObject pauseText;
	public GameObject restartButton;
	public GameObject[] spawnPointMesh;

	void Start () {
		gameMode = PlayerPrefs.GetString ("Game Mode");
		if (gameMode == "SINGLE PLAYER") {
			openWorldManager.SetActive (true);
		} else {
			pauseText.SetActive (false);
			restartButton.SetActive (false);
		}
		for(int i = 0; i < spawnPointMesh.Length;i++){
			spawnPointMesh [i].SetActive (false);
		}

	}

	void DisableLoadingImage(){
		
	}

	// Update is called once per frame
	void Update () {
	
	}
}
