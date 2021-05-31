//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// GameUI Script
// Last Change : 12/10/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.Interfaces;


/// <summary>
/// This class craeted for managing all game options like racetype, racers,players, and statuses.
/// </summary>
[AddComponentMenu("Racing Game Kit/UI/Race UI Mobile Sample"), RequireComponent(typeof(Race_Manager))]
public class RaceUIMobileSample : MonoBehaviour, IRGKUI
   {

        public String TrackName = "";
        public Texture2D StartRedTexture;
        public Texture2D StartGreenTexture;
        public Texture2D StartYellowTexture;
        public Texture2D StartBlankTexture;
        public Texture2D WrongWayTexture;

        public Texture2D UI_Rival;
        public Texture2D UI_Lap;
        public Texture2D UI_Speed;
        public Texture2D UI_Time;

        public Texture2D _1stPlaceImage;
        public Texture2D _2ndPlaceImage;
        public Texture2D _3rdPlaceImage;
   

        public Texture2D _1pxSeperator;


        public enum enumCoundownStyle
        {
            Character = 1,
            Images = 2,
        }
        public enumCoundownStyle CountdownWindowStyle = enumCoundownStyle.Character;

        private float iScreenWidth = 0;
        private float iScreenHeigt = 0;


        //Placement Info Window
        private Rect windowRight;
        public bool _showRightWindow = true;

        //Lap Info Window
        private Rect windowLeft;
        public bool _showLeftWindow = true;

        //Countdown Info WindowTime
        private Rect windowCountdownCharacter;
        public Boolean _ShowCharacterCountdown = false;

        //New Countdown window
        private Rect windowCountdownImage;
        public Boolean _ShowImageCountdown = false;
        public int ImageCountdownDelay = 1;
        private bool StartHideTimerForImageCoutdown = false;


        //Results info Window
        private Rect windowResultsPlacement;
        public Boolean _ShowResultPlacement = false;

        //Results info Window
        private Rect windowResultsGrid;
        public Boolean _showResultsGrid = false;

        //Results info Window
        private Rect windowSpeedResultsGrid;
        public Boolean _showSpeedResultsGrid = false;


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
        private Racer_Detail PlayerDetails;
        private bool PlayerChecked=false;
        private float currentCount;

        public GUISkin UISkin;

        public float PlayerSpeed = 0;

        private bool blnUITakeOver = false;

        private void Start()
        {
            iScreenWidth = Screen.width;
            iScreenHeigt = Screen.height;

            _RaceManager = base.GetComponent(typeof(Race_Manager)) as Race_Manager;
            RacerList = _RaceManager.RegisteredRacers;

            PlayerChecked = false;
        }
        private void Update()
        {
            if (!PlayerChecked)
            {
                for (int i = 0; i < RacerList.Count; i++)
                {

                    if (RacerList[i].IsPlayer)
                    {
                        PlayerDetails = RacerList[i];
                        PlayerChecked = true;
                    }
                }
                 
               
            }

            iScreenWidth = Screen.width;
            iScreenHeigt = Screen.height;
            windowRight = new Rect(iScreenWidth - 350, 50, 340, 215);
            windowLeft = new Rect(10, 50, 300, 215);
            windowCountdownCharacter = new Rect(iScreenWidth / 2 - 300, iScreenHeigt / 2 - 100, 600, 200);
            windowCountdownImage = new Rect(iScreenWidth / 2 - 150, iScreenHeigt / 2 - 100, 300, 150);
            windowGo = new Rect(iScreenWidth / 2 - 300, iScreenHeigt / 2 - 100, 600, 200);
            windowResultsPlacement = new Rect(1, 50, iScreenWidth, iScreenHeigt - iScreenHeigt /6);
            windowResultsGrid = new Rect(1, 50, iScreenWidth, iScreenHeigt - iScreenHeigt / 6);
            windowSpeedResultsGrid = new Rect(1, 50, iScreenWidth, iScreenHeigt - iScreenHeigt / 6);
            windowCheckPointTimer = new Rect(iScreenWidth / 2 - 150, 50, 300, 215);
            windowWrong = new Rect(iScreenWidth / 2 - 50, 100, 100, 100);
            processHideTimers();
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
            
            if (isGoShowing)
            {
                CountDownForGoWindow();
            }
            if (UISkin != null) GUI.skin = UISkin;

            if (_showRightWindow)
            {
                windowRight = GUI.Window(0, windowRight, RenderRightWindow, "", "PlacementWindowStyle");
            }
            if (_showLeftWindow)
            {
                windowLeft = GUI.Window(1, windowLeft, RenderLeftWindow, "", "LapWindowStyle");
            }

            if (_ShowCharacterCountdown)
            {
                windowCountdownCharacter = GUI.Window(2, windowCountdownCharacter, RenderCountdownWindow, "", "CountDownWindowStyle");
                
            }
            if (_ShowImageCountdown)
            {
                windowCountdownImage = GUI.Window(2, windowCountdownImage, RenderCountDownWindowImages, "", "TransparentWindow");
            }

            if (_ShowGoWindow)
            {
                windowGo = GUI.Window(3, windowGo, RenderGoWindow, "", "CountDownWindowStyle");
            }
            if (_ShowResultPlacement)
            {
                windowResultsPlacement = GUI.Window(4, windowResultsPlacement, RenderResultPlacementWindow, "", "ResultsWindowStyle");
                GUI.BringWindowToFront(4);
            }
            if (_showResultsGrid)
            {
                windowResultsGrid = GUI.Window(4, windowResultsGrid, RenderResultsGridWindow, "Race Results", "ResultsWindowStyle");
                GUI.BringWindowToFront(4);
            }
            if (_showSpeedResultsGrid)
            {
                windowSpeedResultsGrid = GUI.Window(4, windowSpeedResultsGrid, RenderSpeedResultsGridWindow, "Race Results", "ResultsWindowStyle");
                GUI.BringWindowToFront(4);
            }
            if (_ShowCheckPointTimer)
            {
                windowCheckPointTimer = GUI.Window(5, windowCheckPointTimer, RenderCheckPointTimer, "", "LapWindowStyle");
            }
            if (_ShowWrongWay)
            {
                windowWrong = GUI.Window(6, windowWrong, RenderWrongWayWindow, "", "TransparentWindow");
            }

        }
        #region Windows
        public float CurrentCount
        {
            get { return currentCount; }
            set { currentCount = value; }

        }
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

        public bool ShowPlacementWIndow
        {
            get { return _showRightWindow; }
            set { _showRightWindow = value; }
        }
        public bool ShowTimeWindow
        {
            get { return _showLeftWindow; }
            set { _showLeftWindow = value; }
        }

        public bool ShowCheckPointWindow
        {
            get { return _ShowCheckPointTimer; }
            set { _ShowCheckPointTimer = value; }
        }
        public bool ShowWrongWayWindow
        {
            get { return _ShowWrongWay; }
            set { _ShowWrongWay = value; }
        }

        public void ShowResultsWindow()
        {
            if (!blnUITakeOver)
            {
                _showRightWindow = false;
                _showLeftWindow = false;
                _showRightWindow = false;
                _ShowResultPlacement = true;                
                _ShowCheckPointTimer = false;
                if (_RaceManager.RaceType == RaceTypeEnum.Speedtrap)
                {
                    _showSpeedResultsGrid = false;
                }
                else 
                {
                    _showResultsGrid = false;
                }
            }
        }
        private void RenderCountdownWindow(int windowID)
        {
            String strCountdown = currentCount.ToString();
            if (currentCount == _RaceManager.TimerCountdownFrom)
            {
                strCountdown = "Ready!";
            }
            else if (strCountdown == "0")
            {
                _ShowCharacterCountdown = false;
                isGoShowing = true;
            }
            GUI.Label(new Rect(280, 70, 35, 20), strCountdown, "CountDownText");

        }

        private void RenderCountDownWindowImages(int WindowID)
        {
            if (StartBlankTexture != null && StartRedTexture != null && StartYellowTexture != null && StartGreenTexture != null)
            {
                GUI.DrawTexture(new Rect(0, 0, 100, 100), StartRedTexture);

                if (currentCount <= 1)
                {
                    GUI.DrawTexture(new Rect(100, 0, 100, 100), StartYellowTexture);
                }
                else
                {
                    GUI.DrawTexture(new Rect(100, 0, 100, 100), StartBlankTexture);
                }

                if (currentCount == 0)
                {
                    GUI.DrawTexture(new Rect(200, 0, 100, 100), StartGreenTexture);
                }
                else
                {
                    GUI.DrawTexture(new Rect(200, 0, 100, 100), StartBlankTexture);
                }
            }
            else
            {
                GUI.Label(new Rect(280, 70, 35, 20), "MISSING TEXTURE!", "CountDownText");
            }

        }


        private void RenderGoWindow(int windowID)
        {
            GUI.Label(new Rect(280, 70, 35, 20), "GO!", "CountDownText");
        }


        private bool isGoShowing = false;

        private void CountDownForGoWindow()
        {
            if (isGoShowing)
            {
                countdown_internal_timer -= Time.deltaTime;

                if (GoHideWindowDelay > 0)
                {
                    if (countdown_internal_timer < 0.1f)
                    {
                        GoHideWindowDelay -= 1;
                        countdown_internal_timer = 1;
                    }
                    _ShowGoWindow = true;
                }
                else
                {
                    _ShowGoWindow = false;
                    isGoShowing = false;
                }

            }
        }



        private void RenderLeftWindow(int windowID)
        {
           

            string strPlayerStanding = "-";
            if (PlayerDetails != null)
            {
                strPlayerStanding = PlayerDetails.RacerStanding.ToString();
            }
            ///RIVAL
            GUI.Box(new Rect(65, 25, 180, 40), "", "TransparentBox");
            if (UI_Rival != null) { GUI.DrawTexture(new Rect(65, 25, 39, 39), UI_Rival, ScaleMode.ScaleToFit); }
            GUI.Label(new Rect(110, 33, 120, 30), strPlayerStanding + "/" + RacerList.Count.ToString(), "PL_LapLabelBig_White");

            ///LAP
            GUI.Box(new Rect(65, 75, 180, 40), "", "TransparentBox");
            if (UI_Lap != null) { GUI.DrawTexture(new Rect(65, 75, 39, 39), UI_Lap, ScaleMode.ScaleToFit); }
            GUI.Label(new Rect(110, 82, 120, 30), _RaceManager.RaceActiveLap.ToString() + "/" + _RaceManager.RaceLaps.ToString(), "PL_LapLabelBig_White");

            
        }

        private void RenderRightWindow(int windowID)
        {

            ///SPEED
            GUI.Box(new Rect(65, 25, 200, 40), "", "TransparentBox");
            if (UI_Speed != null) { GUI.DrawTexture(new Rect(65, 25, 39, 39), UI_Speed, ScaleMode.ScaleToFit); }
            GUI.Label(new Rect(110, 35, 105, 20), String.Format("{0:0}",PlayerSpeed), "Speed_Value");
            GUI.Label(new Rect(220, 47, 40, 20), "KMH", "Speed_Text");

            ///TIME
            GUI.Box(new Rect(65, 75, 200, 40), "", "TransparentBox");
            if (UI_Time != null) { GUI.DrawTexture(new Rect(65, 75, 39, 39), UI_Time, ScaleMode.ScaleToFit); }
            GUI.Label(new Rect(100, 87, 180, 20), FormatTime(_RaceManager.TimeTotal, false, 2), "PL_LapLabelBig_White");

        }

        private void RenderResultPlacementWindow(int windowID)
        {

            if (PlayerDetails == null)
            {
                _ShowResultPlacement = false;
                _showSpeedResultsGrid = true;
                return;
            }

            switch (PlayerDetails.RacerStanding.ToString())
            {
                case "1":
                    if (_1stPlaceImage != null) { GUI.DrawTexture(new Rect(windowResultsPlacement.width / 2 - 336 / 2, 50, 336, 142), _1stPlaceImage, ScaleMode.ScaleToFit); }        
                    break;
                case "2":
                    if (_2ndPlaceImage!= null) { GUI.DrawTexture(new Rect(windowResultsPlacement.width/2 - 336/2, 50, 336,142), _2ndPlaceImage, ScaleMode.ScaleToFit); }
                    break;
                case "3":
                    if (_3rdPlaceImage != null) { GUI.DrawTexture(new Rect(windowResultsPlacement.width/2 - 336/2, 50, 336,142), _3rdPlaceImage, ScaleMode.ScaleToFit); }
                    break;    
                default:
                    if (_RaceManager.RaceType != RaceTypeEnum.Speedtrap)
                    {
                        _ShowResultPlacement = false;
                        _showResultsGrid = true;
                    }
                    else
                    {
                        _ShowResultPlacement = false;
                        _showSpeedResultsGrid = true;
                    }
                break;
            }

            if (_1pxSeperator != null) { GUI.DrawTexture(new Rect(200, 275, windowResultsPlacement.width-400, 2), _1pxSeperator, ScaleMode.StretchToFill); }

            if (_RaceManager.RaceType != RaceTypeEnum.Speedtrap)
            {
                GUI.Label(new Rect(200, 300, 180, 20), "Time", "PL_LapLabelBig_White");
                GUI.Label(new Rect(windowResultsPlacement.width - 400, 300, 180, 20), FormatTime(PlayerDetails.RacerTotalTime, true, 2), "PL_LapLabelBig_White");
            }
            else
            {
                if (_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
                {
                    GUI.Label(new Rect(200, 300, 200, 20), "Top Speed", "PL_LapLabelBig_White");
                    GUI.Label(new Rect(windowResultsPlacement.width - 400, 300, 180, 20), String.Format("{0:0} KMH",PlayerDetails.RacerHighestSpeed), "PL_LapLabelBig_White");
                }
                else
                {
                    GUI.Label(new Rect(200, 300, 200, 20), "Total Speed", "PL_LapLabelBig_White");
                    GUI.Label(new Rect(windowResultsPlacement.width - 400, 300, 180, 20), String.Format("{0:0} KMH", PlayerDetails.RacerSumOfSpeeds), "PL_LapLabelBig_White");
                }
            }
            
            if (_1pxSeperator != null) { GUI.DrawTexture(new Rect(200, 340, windowResultsPlacement.width - 400, 2), _1pxSeperator, ScaleMode.StretchToFill); }

            if (GUI.Button(new Rect(100, windowResultsPlacement.height - 50, 200, 50), "Retry", "BigButton"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }
            
            if (GUI.Button(new Rect(windowResultsPlacement.width -300, windowResultsPlacement.height - 50, 200, 50), "Continue", "BigButton"))
            {
                blnUITakeOver = true;
                if (_RaceManager.RaceType != RaceTypeEnum.Speedtrap)
                {
                    _ShowResultPlacement = false;
                    _showResultsGrid = true;
                }
                else
                {
                    _ShowResultPlacement = false;
                    _showSpeedResultsGrid = true;
                }
            }


        }

        private void RenderResultsGridWindow(int windowID)
        {
            GUI.Label(new Rect(windowResultsGrid.width-410,10 , 400, 20), TrackName, "RaceResultsTrackTitle");
            
            float _fStartPoint = 60f;

            GUI.Label(new Rect(30, _fStartPoint, 100, 20), "POS", "RaceResultsGridHeaders");
            GUI.Label(new Rect(130, _fStartPoint, 250, 20), "Driver", "RaceResultsGridHeaders");
            GUI.Label(new Rect(windowResultsPlacement.width - 450, _fStartPoint, 200, 20), "Fastest ", "RaceResultsGridHeaders");
            GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), "Total Time", "RaceResultsGridHeaders");

            _fStartPoint += 40;
            for (int i = 0; i < RacerList.Count; i++)
            {

                GUI.Label(new Rect(50, _fStartPoint, 100, 20), (i+1).ToString(), "RaceResultsGridItems");
                GUI.Label(new Rect(130, _fStartPoint, 200, 20), RacerList[i].RacerName.ToString(), "RaceResultsGridItems");
                GUI.Label(new Rect(windowResultsPlacement.width - 450, _fStartPoint, 200, 20), FormatTime(RacerList[i].RacerBestTime, true, 2), "RaceResultsGridItems");
                
                float _racerDistance = RacerList[i].RacerDistance; if (_racerDistance < 0) _racerDistance = 0;

                if (RacerList[i].RacerFinished)
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), FormatTime(RacerList[i].RacerTotalTime, true, 2), "RaceResultsGridItems");
                }
                else if (RacerList[i].RacerDestroyed)
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), "DNF", "RaceResultsGridItems");
                }
                else
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), String.Format("{0:0}m", _racerDistance), "RaceResultsGridItems");
                }
                _fStartPoint += 35f;
                if (_1pxSeperator != null) { GUI.DrawTexture(new Rect(30, _fStartPoint, windowResultsPlacement.width - 100, 2), _1pxSeperator, ScaleMode.StretchToFill); }
                _fStartPoint += 5f;
            }

            if (GUI.Button(new Rect(100, windowResultsPlacement.height - 50, 200, 50), "Retry", "BigButton"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }

            if (GUI.Button(new Rect(windowResultsPlacement.width - 300, windowResultsPlacement.height - 50, 200, 50), "Continue", "BigButton"))
            {
                Application.LoadLevel(0);
            }
        }


        private void RenderSpeedResultsGridWindow(int windowID)
        {
            GUI.Label(new Rect(windowResultsGrid.width - 410, 10, 400, 20), TrackName, "RaceResultsTrackTitle");

            float _fStartPoint = 60f;

            GUI.Label(new Rect(30, _fStartPoint, 100, 20), "POS", "RaceResultsGridHeaders");
            GUI.Label(new Rect(130, _fStartPoint, 250, 20), "Driver", "RaceResultsGridHeaders");
            GUI.Label(new Rect(windowResultsPlacement.width - 450, _fStartPoint, 200, 20), "Top Speed", "RaceResultsGridHeaders");
            GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), "Total Speed", "RaceResultsGridHeaders");

            if (_RaceManager.SpeedTrapMode == eSpeedTrapMode.HighestSpeed)
            {
                RacerList.Sort(delegate(Racer_Detail r1, Racer_Detail r2) { return r1.CompareToHighSpeed(r2); });
            }
            else
            {
                RacerList.Sort(delegate(Racer_Detail r1, Racer_Detail r2) { return r1.CompareToSpeedSum(r2); });
            }


            _fStartPoint += 40;
            int m = 1;
            for (int i = RacerList.Count-1; i>=0; i--)
            {

                GUI.Label(new Rect(50, _fStartPoint, 100, 20), (m).ToString(), "RaceResultsGridItems");
                GUI.Label(new Rect(130, _fStartPoint, 200, 20), RacerList[i].RacerName.ToString(), "RaceResultsGridItems");
                GUI.Label(new Rect(windowResultsPlacement.width - 450, _fStartPoint, 200, 20), String.Format("{0:0} KMH",RacerList[i].RacerHighestSpeed), "RaceResultsGridItems");

                float _racerDistance = RacerList[i].RacerDistance; if (_racerDistance < 0) _racerDistance = 0;

                if (RacerList[i].RacerFinished)
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), String.Format("{0:0} KMH",RacerList[i].RacerSumOfSpeeds), "RaceResultsGridItems");
                }
                else if (RacerList[i].RacerDestroyed)
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20), "DNF", "RaceResultsGridItems");
                }
                else
                {
                    GUI.Label(new Rect(windowResultsPlacement.width - 250, _fStartPoint, 200, 20),String.Format("{0:0} KMH",RacerList[i].RacerSumOfSpeeds), "RaceResultsGridItems");
                }
                _fStartPoint += 35f;
                if (_1pxSeperator != null) { GUI.DrawTexture(new Rect(30, _fStartPoint, windowResultsPlacement.width - 100, 2), _1pxSeperator, ScaleMode.StretchToFill); }
                _fStartPoint += 5f;
                m++;
            }

            if (GUI.Button(new Rect(100, windowResultsPlacement.height - 50, 200, 50), "Retry", "BigButton"))
            {
                Application.LoadLevel(Application.loadedLevel);
            }

            if (GUI.Button(new Rect(windowResultsPlacement.width - 300, windowResultsPlacement.height - 50, 200, 50), "Continue", "BigButton"))
            {
                Application.LoadLevel(0);
            }
           
        }



        void RenderCheckPointTimer(int windowID)
        {
            GUI.Box(new Rect(0, 30, 300, 70), "", "TransparentBox");
            GUI.Label(new Rect(0, 30, 300, 30), "Next Checkpoint", "CheckpointTimerText");
            GUI.Label(new Rect(0, 65, 300, 30), FormatTime(_RaceManager.TimeNextCheckPoint, true, 2), "CheckpointTimerValue");
        }

        void RenderWrongWayWindow(int windowID)
        {
            if (WrongWayTexture != null)
            {
                GUI.DrawTexture(new Rect(windowWrong.width / 2 - 37.5f, 0, 75, 75), WrongWayTexture);
            }
            else
            {
                GUI.Label(new Rect(0, 0, 75, 75), "Wrong Way!", "PL_Label_White");
            }
        }

        public void RaceFinished(String RaceType)
        {
            ShowResultsWindow();
        }

        public void PlayerCheckPointPassed(RacingGameKit.CheckPointItem PassedCheckPointItem)
        {
            //Do whatever you want 
        }
        #endregion

        #region Helpers
        public string FormatTime(float TimeValue, bool ShowFraction, float FractionDecimals)
        {
            String strReturn = "00:00:00";
            if (!ShowFraction)
            { strReturn = "00:00"; }

            if (TimeValue > 0)
            {
                TimeSpan tTime = TimeSpan.FromSeconds(TimeValue);

                float minutes = tTime.Minutes;
                float seconds = tTime.Seconds;

                float fractions = (TimeValue * 100) % 100;
                if (fractions >= 99) fractions = 0;

                if (ShowFraction)
                {
                    if (FractionDecimals == 2)
                    {
                        strReturn = String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fractions);
                    }
                    else
                    { 
                        strReturn = String.Format("{0:00}:{1:00}.{2:0}", minutes, seconds, fractions);
                    }
                }
                else
                {
                    strReturn = String.Format("{0:00}.{1:00}", minutes, seconds);
                }
            }
            return strReturn;
        }


        #endregion
    }
 