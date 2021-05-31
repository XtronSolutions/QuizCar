using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RacingGameKit;

public class TrackSelectionSceneLoader : MonoBehaviour {

	public string m_Scene;
	public GameObject m_MyGameObject;
	public GameObject m_LoadingScreen;
	public Race_Manager m_raceManager;


	void Start()
	{
		if (GlobalVariables.FORREST_SCENE_SELECTED && !Constants.isMultiplayerSelected) {
			
			int trackNo = RewardProperties.Instance.GetTrackSelected ();
			if (trackNo == 2) {
			
				m_Scene = "ForestBeachEnvTrack" + trackNo;
				SceneManager.LoadScene (m_Scene);
			}
			else if (trackNo == 3) {
			
				m_Scene = "ForestBeachEnvTrack" + trackNo;
				SceneManager.LoadScene (m_Scene);
			}
			else if (trackNo == 1) {

                m_Scene = "ForestBeachEnv";// "ForestBeachTemp";
                SceneManager.LoadScene (m_Scene);
			}


		}
        
        else if(Constants.isMultiplayerSelected){
			if (Constants.isPrivateModeSelected) {
				SceneManager.LoadScene ("PrivateMultiplayerConnectionScene");
			} else {
				SceneManager.LoadScene ("MultiplayerConnectionScene");
			}
		}
	}

	void Update()
	{

	}


}
