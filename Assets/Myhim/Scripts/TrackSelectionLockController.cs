using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackSelectionLockController : MonoBehaviour {

	public bool IsLocked;
	public int TrackNo;
	public GameObject UnlockMessgaePanel;
	public GameObject LockImage;
	public bool IsDesert = false;
	public bool IsForest = false;

	// Use this for initialization
	void OnEnable () {
		if (IsForest) {
			if (RewardProperties.Instance.GetUnlockTrack (TrackNo) == 1) {
		
				IsLocked = false;
                if(LockImage!=null)
				LockImage.SetActive (false);
			}
		}
		if (IsDesert) {
		
			if (RewardProperties.Instance.GetUnlockDesertTrack (TrackNo) == 1) {

				IsLocked = false;
                if (LockImage != null)
                    LockImage.SetActive (false);
			}
		}
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}

	public void OnClickTrack(){
	
		if (IsLocked) {

            UnlockMessgaePanel.transform.SetAsLastSibling();
			UnlockMessgaePanel.SetActive (true);
		} else {

            GameData.trackNo = TrackNo - 1;
			if (IsForest) {
				RewardProperties.Instance.SetTrackSelected (TrackNo);
			}
			if (IsDesert) {
			
				RewardProperties.Instance.SetDesertTrackSelected (TrackNo);
			}
		}
	}
}
