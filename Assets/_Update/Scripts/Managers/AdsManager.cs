using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : SingeltonBase<AdsManager>, IUnityAdsListener
{
    ////iOS appID="ca-app-pub-3940256099942544~1458002511";

    const string RewardedVideoAdID_Android = "3493308";
    const string RewardedVideoAdID_iOS = "3493309";
    const string myPlacementId = "rewardedVideo";

    public bool isRewardedAdLoaded 
    { 
        get { return Advertisement.IsReady(myPlacementId); } 
    }
    bool testMode = false;
    public delegate void AdEvents();
    AdEvents caller;
    //
 
    // Start is called before the first frame update
    void Start()
    {
        Advertisement.AddListener(this);


        if(Application.platform == RuntimePlatform.Android)
        {
            Advertisement.Initialize(RewardedVideoAdID_Android, testMode);
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Advertisement.Initialize(RewardedVideoAdID_iOS, testMode);
        }
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void LoeadRewardedAd()
    {
        Advertisement.Load(myPlacementId);
        
    }
    public void ShowRewardedVideoAd(AdEvents CallbackFunction)
    {
        caller = null;
        //caller = CallbackFunction;
        if (Advertisement.IsReady(myPlacementId))
        {
            caller = CallbackFunction;
         
            Advertisement.Show(myPlacementId);
        }
        else
        {
            LoeadRewardedAd();
        }
    }



    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            caller();
            caller = null;
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
        LoeadRewardedAd();
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        Debug.Log("Unity Ads OnUnityAdsReady: " + placementId);
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
        Debug.Log("Unity Ads OnUnityAdsDidError: " + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
        Debug.Log("Unity Ads OnUnityAdsDidStart: " + placementId);
    }
}