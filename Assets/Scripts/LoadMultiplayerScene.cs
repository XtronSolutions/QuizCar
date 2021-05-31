using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RacingGameKit;

public class LoadMultiplayerScene : MonoBehaviour {

	public string m_Scene;
	public GameObject m_MyGameObject;
	public GameObject m_LoadingScreen;
	public Race_Manager m_raceManager;


	void Start()
	{
		if (Constants.isMultiplayerSelected) {
			StartCoroutine (LoadYourAsyncScene ());
		} else {
			m_LoadingScreen.SetActive (false);
			m_raceManager.StartRace ();
		}
	}

	void Update()
	{

	}

	IEnumerator LoadYourAsyncScene()
	{
		// Set the current Scene to be able to unload it later
		Scene currentScene = SceneManager.GetActiveScene();

		// The Application loads the Scene in the background at the same time as the current Scene.
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_Scene, LoadSceneMode.Additive);

		// Wait until the last operation fully loads to return anything
		while (!asyncLoad.isDone)
		{
			yield return null;
		}

		// Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
		SceneManager.MoveGameObjectToScene(m_MyGameObject, SceneManager.GetSceneByName(m_Scene));
		// Unload the previous Scene
		SceneManager.UnloadSceneAsync(currentScene);
		m_LoadingScreen.SetActive (false);
	}
}
