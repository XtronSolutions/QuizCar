//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// GameUI Script
// Last Change : 12/09/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Interfaces;
using SmartAssembly.Attributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace RacingGameKit
{
    /// <summary>
    /// This class craeted for managing all game options like racetype, racers,players, and statuses.
    /// </summary>
    [AddComponentMenu("Racing Game Kit/UI/Race UI"), RequireComponent(typeof(Race_Manager))]
    [DoNotObfuscate()]
    public class Race_UI : MonoBehaviour, IRGKUI
    {
		public Button RaceFinishedButton;
        public Texture2D StartRedTexture;
        public Texture2D StartGreenTexture;
        public Texture2D StartYellowTexture;
        public Texture2D StartBlankTexture;
        public Texture2D WrongWayTexture;

        public enum enumCoundownStyle
        {
            Character = 1,
            Images = 2,
        }
        public enumCoundownStyle CountdownWindowStyle = enumCoundownStyle.Character;

        private float iScreenWidth = 0;
        private float iScreenHeigt = 0;


        //Placement Info Window
        private Rect windowPlacement;
        public bool _ShowPlacementWindow = true;

        //Placement Info Window based Speed
        private Rect windowPlacementSpeed;
        public bool _ShowPlacementSpeedWindow = false;


        //Lap Info Window
        private Rect windowTime;
        public bool _ShowTimeWindow = true;

        //Countdown Info WindowTime
        private Rect windowCountdownCharacter;
        public Boolean _ShowCharacterCountdown = false;

        //New Countdown window
        private Rect windowCountdownImage;
        public Boolean _ShowImageCountdown = false;
        public int ImageCountdownDelay = 1;
        private bool StartHideTimerForImageCoutdown = false;

        

        //Results info Window
        private Rect windowResults;
        public Boolean _ShowResults = false;

        //CheckPoint info window
        private Rect windowCheckPointTimer;
        public Boolean _ShowCheckPointTimer = false;

        //Wrong Way window
        private Rect windowWrong;
        public Boolean _ShowWrongWay = false;

        //GoWindow
        private Rect windowGo;
        public Boolean _ShowGoWindow = false;
        public int GoHideWindowDelay = 1;




        private Race_Manager _RaceManager = null;
        private List<Racer_Detail> RacerList;

        private float currentCount;

        public GUISkin UISkin;



		#region UGUI

		public Text countdownText;
        public GameObject powerups;
		private void ShowCountDown()
		{
			String strCountdown = currentCount.ToString();
			if (currentCount >= _RaceManager.TimerCountdownFrom-1)
            {
                if (PlayerPrefs.GetInt(GameData.TUTORIALKEY, 1) == 1)
                {
                    strCountdown = "";
                }
                else
                    strCountdown = "Ready!";
			}
			else if (strCountdown == "0")
			{
				// _ShowCharacterCountdown = false;
				strCountdown = "GO!";
				StartCoroutine (DisableObject ());
				//isGoShowing = true;
			}
//			Debug.Log ("strCountdown"+strCountdown);
			countdownText.text = strCountdown;

			//   GUI.Label(new Rect(280, 70, 35, 20), strCountdown, "CountDownText");
			
		}

		public void SetMultiplayerReadyText(){
			countdownText.text = "Ready!";
		}

		public void SetMulitplayerCounterText(){
			countdownText.text = "GO!!";
			StartCoroutine (DisableObject ());
		}

		IEnumerator DisableObject(){
				yield return new WaitForSeconds(2f);
				countdownText.enabled = false;
            yield return new WaitForSeconds(1f);
            powerups.SetActive(true);
		}
		#endregion
        private void Start()
        {

            iScreenWidth = Screen.width;
            iScreenHeigt = Screen.height;

            _RaceManager = base.GetComponent(typeof(Race_Manager)) as Race_Manager;
            RacerList = _RaceManager. RegisteredRacers;

        }
        private void Update()
        {
//            iScreenWidth = Screen.width;
//            iScreenHeigt = Screen.height;
//
//            windowPlacement = new Rect(iScreenWidth - 200, 20, 300, 300);
//            windowPlacementSpeed = new Rect(iScreenWidth - 200, 20, 300, 300);
//
//            windowTime = new Rect(10, 20, 300, 215);
//            windowCountdownCharacter = new Rect(iScreenWidth / 2 - 300, iScreenHeigt / 2 - 100, 600, 200);
//            windowCountdownImage = new Rect(iScreenWidth / 2 - 150, iScreenHeigt / 2 - 100, 300, 150);
//            windowGo = new Rect(iScreenWidth / 2 - 300, iScreenHeigt / 2 - 100, 600, 200);
//            windowResults = new Rect(iScreenWidth / 2 - 200, iScreenHeigt / 2 - 125, 400, 250);
//            windowCheckPointTimer = new Rect(iScreenWidth / 2 - 150, 20, 300, 215);
            windowWrong = new Rect(iScreenWidth / 2 - 50, 100, 100, 100);

         //   processHideTimers();
			if (!Constants.isMultiplayerSelected) {
				ShowCountDown ();
			}

        }

        private float countdown_internal_timer = 1;


        private void processHideTimers()
        {

            if (StartHideTimerForImageCoutdown)
            {
                countdown_internal_timer -= Time.deltaTime;

                if (ImageCountdownDelay > 0)
                {
                    if (countdown_internal_timer < 0.1f)
                    {
                        ImageCountdownDelay -= 1;
                        countdown_internal_timer = 1;
                    }
                    _ShowImageCountdown = true;
                }
                else
                {
                    _ShowImageCountdown = false;
                    StartHideTimerForImageCoutdown = false;
                }

            }

        }


        void OnGUI()
        {
//            if (isGoShowing)
//            {
//                CountDownForGoWindow();
//            }
//            if (UISkin != null) GUI.skin = UISkin;
//
//            if (_ShowPlacementWindow)
//            {
//                windowPlacement = GUI.Window(50, windowPlacement, RenderPlacementWindow, "", "PlacementWindowStyle");
//            }
//
//            if (_ShowPlacementSpeedWindow)
//            {
//                windowPlacementSpeed = GUI.Window(60, windowPlacement, RenderPlacemenSpeedtWindow, "", "PlacementWindowStyle");
//            }
//
//
//            if (_ShowTimeWindow)
//            {
//                windowTime = GUI.Window(1, windowTime, RenderLapWindow, "", "LapWindowStyle");
//            }
//
//            if (_ShowCharacterCountdown)
//            {
//                windowCountdownCharacter = GUI.Window(70, windowCountdownCharacter, RenderCountdownWindow, "", "CountDownWindowStyle");
//            }
//            if (_ShowImageCountdown)
//            {
//                windowCountdownImage = GUI.Window(70, windowCountdownImage, RenderCountDownWindowImages, "", "TransparentWindow");
//            }
//
//            if (_ShowGoWindow)
//            {
//                windowGo = GUI.Window(80, windowGo, RenderGoWindow, "", "CountDownWindowStyle");
//            }
//
//            if (_ShowResults)
//            {
//                windowResults = GUI.Window(90, windowResults, RenderResultsWindow, "RACE RESULTS", "ResultsWindowStyle");
//            }
//            if (_ShowCheckPointTimer)
//            {
//                windowCheckPointTimer = GUI.Window(100, windowCheckPointTimer, RenderCheckPointTimer, "", "LapWindowStyle");
//            }
//            if (_ShowWrongWay)
//            {
//				windowWrong = GUI.Window(110, new Rect(Screen.width - 200, Screen.height - 200, 200, 200), RenderWrongWayWindow, "", "TransparentWindow");
//            }
//
        }
   
        public float CurrentCount
        {
            get { return currentCount; }
            set { currentCount = value; }

        }
//		#region Windows
        public bool ShowCountdownWindow
        {
            get { return _ShowCharacterCountdown; }
            set
            {
                if (CountdownWindowStyle == enumCoundownStyle.Character)
                {
                    _ShowCharacterCountdown = value;
                }
                else
                {
                    if (value)
                    {
                        _ShowImageCountdown = value;
                    }
                    else
                    {
                        StartHideTimerForImageCoutdown = true;
                    }
                }
            }
        }

//        public bool ShowPlacementWIndow
//        {
//            get { return _ShowPlacementWindow; }
//            set { _ShowPlacementWindow = value; }
//        }
//        public bool ShowPlacementSpeedWIndow
//        {
//            get { return _ShowPlacementSpeedWindow; }
//            set { _ShowPlacementSpeedWindow = value; }
//        }
//        public bool ShowTimeWindow
//        {
//            get { return _ShowTimeWindow; }
//            set { _ShowTimeWindow = value; }
//        }
//
//        public bool ShowCheckPointWindow
//        {
//            get { return _ShowCheckPointTimer; }
//            set { _ShowCheckPointTimer = value; }
//        }
        public bool ShowWrongWayWindow
        {
            get { return _ShowWrongWay; }
            set { _ShowWrongWay = value; }
        }
//
        public void ShowResultsWindow()
        {
            _ShowPlacementWindow = false;
            _ShowTimeWindow = false;
            _ShowPlacementWindow = false;
            _ShowResults = true;
            _ShowCheckPointTimer = false;
        }
       

//        private void RenderCountDownWindowImages(int WindowID)
//        {
//            if (StartBlankTexture != null && StartRedTexture != null && StartYellowTexture != null && StartGreenTexture != null)
//            {
//                GUI.DrawTexture(new Rect(0, 0, 100, 100), StartRedTexture);
//
//                if (currentCount <= 1)
//                {
//                    GUI.DrawTexture(new Rect(100, 0, 100, 100), StartYellowTexture);
//                }
//                else
//                {
//                    GUI.DrawTexture(new Rect(100, 0, 100, 100), StartBlankTexture);
//                }
//
//                if (currentCount == 0)
//                {
//                    GUI.DrawTexture(new Rect(200, 0, 100, 100), StartGreenTexture);
//                }
//                else
//                {
//                    GUI.DrawTexture(new Rect(200, 0, 100, 100), StartBlankTexture);
//                }
//            }
//            else
//            {
//                GUI.Label(new Rect(280, 70, 35, 20), "MISSING TEXTURE!", "CountDownText");
//            }
//
//        }
//
//
//        private void RenderGoWindow(int windowID)
//        {
//            GUI.Label(new Rect(280, 70, 35, 20), "GO!", "CountDownText");
//        }
//
//
////        private bool isGoShowing = false;
//
//        private void CountDownForGoWindow()
//        {
//            if (isGoShowing)
//            {
//                countdown_internal_timer -= Time.deltaTime;
//
//                if (GoHideWindowDelay > 0)
//                {
//                    if (countdown_internal_timer < 0.1f)
//                    {
//                        GoHideWindowDelay -= 1;
//                        countdown_internal_timer = 1;
//                    }
//                    _ShowGoWindow = true;
//                }
//                else
//                {
//                    _ShowGoWindow = false;
//                    isGoShowing = false;
//                }
//
//            }
//        }
//
//
//
//        private void RenderLapWindow(int windowID)
//        {
//            GUI.Label(new Rect(5, 15, 90, 20), "LAP", "PL_LapLabel_Gray");
//            GUI.Label(new Rect(100, 10, 70, 20), _RaceManager.RaceActiveLap.ToString() + "/" + _RaceManager.RaceLaps.ToString(), "PL_LapLabelBig_White");
//
//            GUI.Label(new Rect(5, 55, 90, 20), "CURRENT", "PL_LapLabel_Gray");
//            GUI.Label(new Rect(100, 50, 70, 20), FormatTime(_RaceManager.TimeCurrent, true, 2), "PL_LapLabelBig_White");
//            GUI.Label(new Rect(5, 95, 90, 20), "LAST", "PL_LapLabel_Gray");
//            GUI.Label(new Rect(100, 90, 70, 20), FormatTime(_RaceManager.TimePlayerLast, true, 2), "PL_LapLabelBig_White");
//            GUI.Label(new Rect(5, 135, 90, 20), "BEST", "PL_LapLabel_Gray");
//            GUI.Label(new Rect(100, 130, 70, 20), FormatTime(_RaceManager.TimePlayerBest, true, 2), "PL_LapLabelBig_White");
//            GUI.Label(new Rect(5, 175, 90, 20), "TOTAL", "PL_LapLabel_Gray");
//            GUI.Label(new Rect(100, 170, 70, 20), FormatTime(_RaceManager.TimeTotal, true, 2), "PL_LapLabelBig_White");
//        }
//
//        private void RenderPlacementWindow(int windowID)
//        {
//            float _fStartPoint = 20f;
//
//            for (int i = 0; i < RacerList.Count; i++)
//            {
//                float _racerDistance = RacerList[i].RacerDistance; if (_racerDistance < 0) _racerDistance = 0;
//
//                if (RacerList[i].RacerDestroyed)
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 60, 20), "DNF", "PL_Time_White");
//                }
//                else
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 60, 20), String.Format("{0:0}m", _racerDistance), "PL_Time_White");
//                }
//
//                GUI.Label(new Rect(65, _fStartPoint, 20, 20), (i + 1) + ".", "PL_Time_White");
//                GUI.Label(new Rect(90, _fStartPoint, 150, 20), RacerList[i].RacerName.ToString(), "PL_Label_White");
//                _fStartPoint += 20f;
//            }
//        }
//
//
//        private void RenderPlacemenSpeedtWindow(int windowID)
//        {
//            float _fStartPoint = 20f;
//
//            for (int i = 0; i < RacerList.Count; i++)
//            {
//                float _racerSpeed = 0;
//                if (_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
//                {
//                    _racerSpeed = RacerList[i].RacerHighestSpeed; if (_racerSpeed < 0) _racerSpeed = 0;
//                }
//                else if (_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestTotalSpeed)
//                {
//                    _racerSpeed = RacerList[i].RacerSumOfSpeeds; if (_racerSpeed < 0) _racerSpeed = 0;
//                }
//                if (RacerList[i].RacerDestroyed)
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 60, 20), "DNF", "PL_Time_White");
//                }
//                else
//                {
//                    if (_racerSpeed == 0)
//                        GUI.Label(new Rect(10, _fStartPoint, 60, 20), String.Format("-",_racerSpeed), "PL_Time_White");
//                    else
//                        GUI.Label(new Rect(10, _fStartPoint, 60, 20), String.Format("{0:0} Km/h", _racerSpeed), "PL_Time_White");
//                }
//
//                GUI.Label(new Rect(65, _fStartPoint, 20, 20), (i + 1) + ".", "PL_Time_White");
//                GUI.Label(new Rect(90, _fStartPoint, 150, 20), RacerList[i].RacerName.ToString(), "PL_Label_White");
//                _fStartPoint += 20f;
//            }
//        }
//
//        private void RenderResultsWindow(int windowID)
//        {
//            float _fStartPoint = 60f;
//
//            for (int i = 0; i < RacerList.Count; i++)
//            {
//                float _racerDistance = RacerList[i].RacerDistance; if (_racerDistance < 0) _racerDistance = 0;
//
//                if (RacerList[i].RacerFinished)
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 55, 20), FormatTime(RacerList[i].RacerTotalTime, true, 2), "PL_Time_White");
//                }
//                else if (RacerList[i].RacerDestroyed)
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 55, 20), "DNF", "PL_Time_White");
//                }
//                else
//                {
//                    GUI.Label(new Rect(10, _fStartPoint, 55, 20), String.Format("{0:0}m", _racerDistance), "PL_Time_White");
//                }
//                GUI.Label(new Rect(65, _fStartPoint, 20, 20), (i + 1) + ".", "PL_Time_White");
//                GUI.Label(new Rect(90, _fStartPoint, 150, 20), RacerList[i].RacerName.ToString(), "PL_Label_White");
//                String LapTimes = "";
//                foreach (float RacerLapTime in RacerList[i].RacerLapTimes)
//                {
//                    LapTimes += FormatTime(RacerLapTime, true, 1).ToString() + " - ";
//                }
//                _fStartPoint += 20f;
//                GUI.Label(new Rect(90, _fStartPoint, 450, 20), "[" + LapTimes + "]", "PL_Label_White");
//                _fStartPoint += 20f;
//
//
//            }
//        }
//        void RenderCheckPointTimer(int windowID)
//        {
//            GUI.Label(new Rect(85, 30, 335, 20), "Next Checkpoint", "PL_Label_White");
//            GUI.Label(new Rect(0, 60, 335, 20), FormatTime(_RaceManager.TimeNextCheckPoint, true, 1), "CountDownText");
//        }
//
//        void RenderWrongWayWindow(int windowID)
//        {
//            if (WrongWayTexture != null)
//            {
//                GUI.DrawTexture(new Rect(windowWrong.width / 2 - 37.5f, 0, 75, 75), WrongWayTexture);
//            }
//            else
//            {
//                GUI.Label(new Rect(0, 0, 75, 75), "Wrong Way!", "PL_Label_White");
//            }
//        }
//
        public void RaceFinished(String RaceType)
        {
            ShowResultsWindow();
        }
//
        public void PlayerCheckPointPassed(RacingGameKit.CheckPointItem PassedCheckPointItem)
        {
            //Do whatever you want 
        }
//        #endregion
//
//        #region Helpers
//        public string FormatTime(float TimeValue, bool ShowFraction, float FractionDecimals)
//        {
//            String strReturn = "00:00:00";
//
//            if (TimeValue > 0)
//            {
//                TimeSpan tTime = TimeSpan.FromSeconds(TimeValue);
//
//                float minutes = tTime.Minutes;
//                float seconds = tTime.Seconds;
//
//                float fractions = (TimeValue * 100) % 100;
//
//                if (ShowFraction)
//                {
//                    if (FractionDecimals == 2)
//                    {
//                        strReturn = String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fractions);
//                    }
//                    else
//                    { strReturn = String.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, fractions); }
//                }
//                else
//                {
//                    strReturn = String.Format("{0:00}:{1:00}", minutes, seconds);
//                }
//            }
//            return strReturn;
//        }
		public void LoadMainMenu()
		{
			Time.timeScale = 1;
			SceneManager.LoadScene ("MainMenu");
		}
//
//        #endregion
    }


}