using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RG_SettingsManager : MonoBehaviour {
	
	public Toggle audioToggle;
	public Text qualityText;
	public string qualityTextPrefix;

	void Start(){
		//if(PlayerPrefs.GetString("Audio", "ON") == "OFF") audioToggle.isOn = false;
		//if(PlayerPrefs.GetString("Audio", "ON") == "ON") audioToggle.isOn = true;
		//int qualityNumber = PlayerPrefs.GetInt("Quality");
		//QualitySettings.SetQualityLevel(qualityNumber, true);
		//qualityText.text = qualityTextPrefix + QualitySettings.names[QualitySettings.GetQualityLevel()];
		//UpdateAudio();
	}

	public void UpdateAudio(){
		if(audioToggle.isOn){
			AudioListener.pause = false;
			PlayerPrefs.SetString("Audio", "ON");
		}else {
			AudioListener.pause = true;
			PlayerPrefs.SetString("Audio", "OFF");
		}
	}

	public void QualityUp(){
		QualitySettings.IncreaseLevel();
		qualityText.text = qualityTextPrefix + QualitySettings.names[QualitySettings.GetQualityLevel()];
		PlayerPrefs.SetInt("Quality", QualitySettings.GetQualityLevel());
	}
	
	public void QualityDown(){
		QualitySettings.DecreaseLevel();
		qualityText.text = qualityTextPrefix + QualitySettings.names[QualitySettings.GetQualityLevel()];
		PlayerPrefs.SetInt("Quality", QualitySettings.GetQualityLevel());
	}

}