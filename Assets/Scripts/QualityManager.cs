using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Diagnostics;
public class QualityManager : MonoBehaviour {
    //public Text SystemInfoText;
    // Use this for initialization
	public Text Info;
    public GameObject BlobShadow;
    public GameObject[] Objects;
    public GameObject HD_Items, SD_Items;
    //PerformanceCounter cpuCounter;
    //PerformanceCounter ramCounter;
    

//    System.Diagnostics.PerformanceCounter cpuCounter;
//    System.Diagnostics.PerformanceCounter ramCounter;
    void Start () 
	{
		Resources.UnloadUnusedAssets ();
//        Application.targetFrameRate = 60;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //SystemInfoText.text = "\nSystemInfo.processorFrequency " + SystemInfo.processorFrequency.ToString();
        
        //SystemInfoText.text = SystemInfoText.text + "\nSystemInfo.graphicsMemorySize " + SystemInfo.graphicsMemorySize;
        //SystemInfoText.text = SystemInfoText.text + "\n SystemInfo.processorCount " + SystemInfo.processorCount;
        //SystemInfoText.text = SystemInfoText.text + "\n SystemInfo.systemMemorySize " + SystemInfo.systemMemorySize;
        int GPUMemory = SystemInfo.graphicsMemorySize;

        int Cpus = SystemInfo.processorFrequency;

        int RamThreshold = 1600;
        if (Cpus <= 1400)
        {

            if (GPUMemory <= 256)
            {
                //SystemInfoText.text = SystemInfoText.text + "LOW";
                //Low Res
                SetLowQuality();
            }

            if (GPUMemory > 256)
            {

                //SystemInfoText.text = SystemInfoText.text + "MED";
                //Med Res
                SetLowQuality();

            }
        }

        if (Cpus >= 1400)
        {

            if (GPUMemory <= 512)
            {

                //SystemInfoText.text = SystemInfoText.text + "MED";
                //Med Res
                SetLowQuality();
            }

            if (GPUMemory > 512)
            {

                //High Res
                //SystemInfoText.text = SystemInfoText.text + "HIGH";
                SetHighQuality();
            }
        }
        
    }

    public void SetHighQuality()
    {
        
		UnityEngine.Debug.Log ("High Quality");
//		Info.text = "high quality";
		QualitySettings.SetQualityLevel(1, true);
      //  BlobShadow.SetActive(false);
        foreach (GameObject GO in Objects)
            GO.SetActive(true);
        //if (Camera.main != null)
        //{
        //    if (RenderSettings.fogMode == FogMode.Linear)
        //    {
        //        Camera.main.far = RenderSettings.fogEndDistance;
        //    }
        //    else if (RenderSettings.fogMode == FogMode.Exponential)
        //    {
        //        Camera.main.far = Mathf.Log(1f / 0.0019f) / RenderSettings.fogDensity;
        //    }
        //    else if (RenderSettings.fogMode == FogMode.ExponentialSquared)
        //    {
        //        Camera.main.far = Mathf.Sqrt(Mathf.Log(1f / 0.0019f)) / RenderSettings.fogDensity;
        //    }
        //}
            //Camera.main.far = 750;
        HD_Items.SetActive(true);
        SD_Items.SetActive(false);
        //GlobalProjectorManager.Get().ShadowsOn = false;

        
        //UnityStandardAssets.Vehicles.Car.WheelEffects[] WheelEffects = GameObject.FindObjectsOfType<UnityStandardAssets.Vehicles.Car.WheelEffects>();
        //foreach (UnityStandardAssets.Vehicles.Car.WheelEffects Dust in WheelEffects)
        //    Dust.enabled = true; 
        //GameObject[] tyreDust = GameObject.FindGameObjectsWithTag("TyreDust");
        //foreach (GameObject Dust in tyreDust)
        //    Dust.SetActive(true);
    }
    public void SetLowQuality()
    {
//		Info.text = "low quality";
		UnityEngine.Debug.Log ("low Quality");
		QualitySettings.SetQualityLevel(0, true);
        BlobShadow.SetActive(true);
        foreach (GameObject GO in Objects)
            GO.SetActive(false);

        //if (Camera.main != null)
        //{
        //    if (RenderSettings.fogMode == FogMode.Linear)
        //    {
        //        Camera.main.far = RenderSettings.fogEndDistance;
        //    }
        //    else if (RenderSettings.fogMode == FogMode.Exponential)
        //    {
        //        Camera.main.far = Mathf.Log(1f / 0.0019f) / RenderSettings.fogDensity;
        //    }
        //    else if (RenderSettings.fogMode == FogMode.ExponentialSquared)
        //    {
        //        Camera.main.far = Mathf.Sqrt(Mathf.Log(1f / 0.0019f)) / RenderSettings.fogDensity;
        //    }
        //}
        //if (Camera.main != null)
        //    Camera.main.far = 250;
        HD_Items.SetActive(false);
        SD_Items.SetActive(true);
        //GlobalProjectorManager.Get().ShadowsOn = true;

        
        //UnityStandardAssets.Vehicles.Car.WheelEffects[] WheelEffects = GameObject.FindObjectsOfType<UnityStandardAssets.Vehicles.Car.WheelEffects>();
        //foreach (UnityStandardAssets.Vehicles.Car.WheelEffects Dust in WheelEffects)
        //    Dust.enabled = false;
        //GameObject[] tyreDust = GameObject.FindGameObjectsWithTag("TyreDust");
        //foreach (GameObject Dust in tyreDust)
        //    Dust.SetActive(false);\


    }
  
	// Update is called once per frame
	void Update () {
        //UnityEngine.Debug.Log("> cpu: " + getCurrentCpuUsage() + "; >ram: " + getAvailableRAM());

    }

   
    public string getCurrentCpuUsage()
    {
//        return cpuCounter.NextValue() + "%";
		return "";
    }

    public string getAvailableRAM()
    {
//        return ramCounter.NextValue() + "MB";
		return "";
    }
    public void ReloadScene()
    {
        Application.LoadLevel(Application.loadedLevelName);
    }
}
