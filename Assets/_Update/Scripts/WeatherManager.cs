using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour {


    public enum WeatherType { Rain,Snow,DustStorm,Sunny};
    public Weather[] weather;
    public bool isDay;
    GameObject cam;
    int i;
    // Use this for initialization
    void Start () {
         i = Random.Range(0, weather.Length);
         // i = 3;
       // Debug.Log(weather[i].weatherType);
        RenderSettings.fog = isDay;
        if(weather[i].weatherType== WeatherType.Sunny)
        {
            RenderSettings.fogDensity = 0.008f;
        }
        else
        {
            RenderSettings.fogDensity = 0.008f;
        }
        RenderSettings.skybox = weather[i].skybox;
        RenderSettings.fogColor = weather[i].fogColor;
        cam = null;
	}
	
	// Update is called once per frame
	void Update () {
        if (cam == null)
        {
           cam = GameObject.Find("_GameCamera");
            if (cam != null && weather[i].weatherType != WeatherType.Sunny)
            {
                GameObject eff = Instantiate(weather[i].effectPrefab);

                eff.transform.parent = cam.transform;
                if (weather[i].weatherType == WeatherType.Snow)
                {
                    eff.transform.localPosition = new Vector3(0, 4, 10);
                }
                else
                {
                    eff.transform.localPosition = new Vector3(0, 10, 10);
                }
                eff.transform.localRotation = Quaternion.identity;

                if(weather[i].weatherType == WeatherType.Rain)
                {
                    cam.GetComponent<MainCamera>().waterDrops.SetActive(true);
                }
            }
        }
    }
    [System.Serializable]
    public class Weather
    {
        public WeatherType weatherType;
        public Color fogColor;
        public Material skybox;
        public GameObject effectPrefab;
    }
}