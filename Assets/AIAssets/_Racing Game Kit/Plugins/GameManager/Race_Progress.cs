//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Race Progress Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using System.Collections.Generic;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/UI/Race Progress"), RequireComponent(typeof(Race_Manager)), ExecuteInEditMode]
    [DoNotObfuscate]
    public class Race_Progress : MonoBehaviour
    {
        public enum eProgressRotation
        {
            Vertical = 0,
            Horizontal = 1
        }
        public enum eProgressPlacement
        {
            TopCenter = 0,
            BottomCenter = 1,
            LeftBottom = 2,
            RightBottom = 3,
        }

        public eProgressRotation ControlRotation = eProgressRotation.Horizontal;
        public eProgressPlacement ControlPlacement = eProgressPlacement.BottomCenter;
        public float ControlWidth;
        public float ControlHeight;
        public float ControlTopOffset;
        public float ControlBottomOffset;
        public float ControlLeftOffset;
        public float ControlRightOffset;

        public float ProgressBarSize;
        public Texture2D ProgressBarBackground;
        public Vector2 ProgressOffset;
        public Texture2D PlayerCompleteForeground;
        public Vector2 PlayerCompleteOffset;
        public Texture2D StartPointIcon;
        public Vector2 StartPointOffset;
        public Texture2D EndPointIcon;
        public Vector2 EndPointOffset;
        public Texture2D LapSeperatorIcon;
        public Vector2 LapSeperatorOffset;
        public Texture2D RacerIcon;
        public Texture2D RivalIcon;
        public Texture2D DestroyedIcon;
        public Vector2 PlayerIconsOffset;



        public GUISkin UISkin;

        private Race_Manager RaceManagerComponent;

        [DoNotObfuscate]
        void Start()
        {
            RaceManagerComponent = base.GetComponent(typeof(Race_Manager)) as Race_Manager;
        }

        private void CreateVerticalStructure()
        {
            if (ControlHeight < ProgressBarSize)
            {
                Debug.LogWarning("RST KIT WARNING : Control Height value cannot lower then progress size!! Automaticaly adjusting!");
                ProgressBarSize = ControlHeight - 20;
            }
            float ControlLeft = 0;
            float ControlTop = 0;

            switch (ControlPlacement)
            {
                case eProgressPlacement.LeftBottom:
                    ControlLeft = ControlLeftOffset;
                    ControlTop = Screen.height - ControlHeight - ControlBottomOffset;
                    break;
                case eProgressPlacement.RightBottom:
                    ControlLeft = Screen.width - ControlWidth - ControlRightOffset;
                    ControlTop = Screen.height - ControlHeight - ControlBottomOffset;
                    break;
            }

            GUI.BeginGroup(new Rect(ControlLeft, ControlTop, ControlHeight, ControlHeight));
            //This added to showing progress area. Actual group width must be same size the progress size because we've to size the progress image first
            //then have to rotate around its pivot. But if progress size lower then control size, it acts weird. Dirty trick but works. 
            //This mod applies only vertical progress.
            //If you dont want to use background for progress area, comment the line below this 
            if (UISkin != null)
            {
                GUI.Box(new Rect(0, 0, ControlWidth, ControlHeight), "", "ProgressBG");
            }
            //Progress bar itself
            if (ProgressBarBackground != null)
            {
                Matrix4x4 MatrixProgressBG = GUI.matrix;
                GUIUtility.RotateAroundPivot(-90, new Vector2(ProgressBarBackground.height, ProgressBarSize));
                GUI.DrawTexture(new Rect(ProgressOffset.x, ProgressOffset.y + ProgressBarSize, ProgressBarSize, ProgressBarBackground.height), ProgressBarBackground, ScaleMode.StretchToFill, true, 1f);
                GUI.matrix = MatrixProgressBG;

            }
            //Player Start Point Icon
            if (StartPointIcon != null)
            {
                GUI.DrawTexture(new Rect(StartPointOffset.x, StartPointOffset.y + ProgressBarSize, StartPointIcon.width, StartPointIcon.height), StartPointIcon, ScaleMode.StretchToFill, true, 10.0F);
            }
            //Race End Point Icon
            if (EndPointIcon != null)
            {
                GUI.DrawTexture(new Rect(EndPointOffset.x, EndPointOffset.y, EndPointIcon.width, EndPointIcon.height), EndPointIcon, ScaleMode.StretchToFill, true, 10.0F);
            }
            //Lap Seperator Icon. This will use only if circuit racemode enabled
            if (LapSeperatorIcon != null)
            {
                float fSeperatorLocations = ProgressBarSize / RaceManagerComponent.RaceLaps;
                for (int i = 1; i < RaceManagerComponent.RaceLaps; i++)
                {
                    GUI.DrawTexture(new Rect(LapSeperatorOffset.x, (fSeperatorLocations * i) + LapSeperatorOffset.y - ProgressOffset.x + (RacerIcon.height / 2), LapSeperatorIcon.width, LapSeperatorIcon.height), LapSeperatorIcon, ScaleMode.StretchToFill, true, 10.0F);
                }
            }

            if (RaceManagerComponent.RaceLength > 0)
            {
                //Race distance calculations from total racelenght multiply bu racelaps.
                float fActualLenght = RaceManagerComponent.RaceLength * RaceManagerComponent.RaceLaps;
                //Calculate pointpixel(as pixel per meter) will use by progress calculations
                float fDistancePixel = ProgressBarSize / fActualLenght;

                //Game not started, we've to move icons to zero point. 
                if (!RaceManagerComponent.IsRaceStarted)
                {
                    fDistancePixel = 0;
                }
                foreach (Racer_Detail Gamer in RaceManagerComponent.RegisteredRacers)
                {
                                      
                    if (Gamer.RacerFinished && !Gamer.RacerDestroyed) 
                    {

                        if (!Gamer.IsPlayer)
                        {
                            GUI.DrawTexture(new Rect(PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y - ProgressOffset.x, RivalIcon.width, RivalIcon.height), RivalIcon, ScaleMode.StretchToFill, true, 10.0F);
                        }
                        else
                        {
                            GUI.DrawTexture(new Rect(PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y - ProgressOffset.x, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                        }
                    }
                    else 
                    {
                        if (!Gamer.IsPlayer) // if racer is not player itself
                        {
                            float fRivalPosition = ProgressBarSize - (fActualLenght - Gamer.RacerDistance) * fDistancePixel; ;
                            if (!Gamer.RacerDestroyed) // If Racer still in race 
                            {
                                GUI.DrawTexture(new Rect(PlayerIconsOffset.x - RivalIcon.width / 2, PlayerIconsOffset.y + fRivalPosition - ProgressOffset.x, RivalIcon.width, RivalIcon.height), RivalIcon, ScaleMode.StretchToFill, true, 10.0F);
                            }
                            else // If Racer knocked out or destroyed by accident or whatever
                            {
                                GUI.DrawTexture(new Rect(PlayerIconsOffset.x - DestroyedIcon.width / 2, PlayerIconsOffset.y + fRivalPosition - ProgressOffset.x, DestroyedIcon.width, DestroyedIcon.height), DestroyedIcon, ScaleMode.StretchToFill, true, 10.0F);
                            }
                        }
                        else //if racer is player 
                        {
                            float fRacerPosition = (fActualLenght - Gamer.RacerDistance) * fDistancePixel;

                            //This will show how much race distance taken by user.
                            if (PlayerCompleteForeground != null)
                            {
                                Matrix4x4 MatrixForeGroundBG = GUI.matrix;
                                GUIUtility.RotateAroundPivot(-90, new Vector2(PlayerCompleteForeground.height, ProgressBarSize));
                                GUI.DrawTexture(new Rect(ProgressOffset.x, ProgressOffset.y + ProgressBarSize, fRacerPosition, ProgressBarBackground.height), PlayerCompleteForeground, ScaleMode.StretchToFill, true, 1f);
                                GUI.matrix = MatrixForeGroundBG;
                            }

                            GUI.DrawTexture(new Rect(PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y + ProgressBarSize - fRacerPosition - ProgressOffset.x, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                        }
                    }
                }
            }
            else
            {//Designtime racer  Offset settings.
                if (RacerIcon != null)
                {
                    GUI.DrawTexture(new Rect(PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y - ProgressOffset.x + ProgressBarSize, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                }
            }
            GUI.EndGroup();

        }

        private void CreateHorizontalStructure()
        {
            float ControlLeft = 0;
            float ControlTop = 0;

            switch (ControlPlacement)
            {
                case eProgressPlacement.BottomCenter:
                    ControlLeft = (Screen.width - ControlWidth) / 2;
                    ControlTop = Screen.height - ControlBottomOffset;
                    break;
                case eProgressPlacement.TopCenter:
                    ControlLeft = (Screen.width - ControlWidth) / 2;
                    ControlTop = ControlTopOffset;
                    break;
            }

            if (UISkin != null)
            {
                GUI.BeginGroup(new Rect(ControlLeft, ControlTop, ControlWidth, ControlHeight), "", "ProgressBG");
            }
            else
            { GUI.BeginGroup(new Rect(ControlLeft, ControlTop, ControlWidth, ControlHeight), ""); }
            //Progress bar itself
            if (ProgressBarBackground != null)
            {
                GUI.DrawTexture(new Rect(ProgressOffset.x, ProgressOffset.y, ProgressBarSize, ProgressBarBackground.height), ProgressBarBackground, ScaleMode.StretchToFill, true, 10.0F);
            }
            //Player Start Point Icon
            if (StartPointIcon != null)
            {
                GUI.DrawTexture(new Rect(StartPointOffset.x, StartPointOffset.y, StartPointIcon.width, StartPointIcon.height), StartPointIcon, ScaleMode.StretchToFill, true, 10.0F);
            }
            //Race End Point Icon
            if (EndPointIcon != null)
            {
                GUI.DrawTexture(new Rect(ProgressBarSize + EndPointOffset.x, EndPointOffset.y, EndPointIcon.width, EndPointIcon.height), EndPointIcon, ScaleMode.StretchToFill, true, 10.0F);
            }
            //Lap Seperator Icon. This will use only if circuit racemode enabled
            if (LapSeperatorIcon != null)
            {
                float fSeperatorLocations = ProgressBarSize / RaceManagerComponent.RaceLaps;
                for (int i = 1; i < RaceManagerComponent.RaceLaps; i++)
                {
                    GUI.DrawTexture(new Rect((fSeperatorLocations * i) + ProgressOffset.x + LapSeperatorOffset.x, LapSeperatorOffset.y, LapSeperatorIcon.width, LapSeperatorIcon.height), LapSeperatorIcon, ScaleMode.StretchToFill, true, 10.0F);
                }
            }

            if (RaceManagerComponent.RaceLength > 0)
            {
                //Race distance calculations from total racelenght multiply bu racelaps.
                float fActualLenght = RaceManagerComponent.RaceLength * RaceManagerComponent.RaceLaps;
                //Calculate pointpixel(as pixel per meter) will use by progress calculations
                float fDistancePixel = ProgressBarSize / fActualLenght;

                //Game not started, we've to move icons to zero point. 
                if (!RaceManagerComponent.IsRaceStarted)
                {
                    fDistancePixel = 0;
                }
                foreach (Racer_Detail Gamer in RaceManagerComponent.RegisteredRacers)
                {
                    if (Gamer.RacerFinished && !Gamer.RacerDestroyed) // Racer still in race, so we'll calculate position
                    {
                        if (!Gamer.IsPlayer)
                        {
                            GUI.DrawTexture(new Rect(ProgressOffset.x + ProgressBarSize + PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y, RivalIcon.width, RivalIcon.height), RivalIcon, ScaleMode.StretchToFill, true, 10.0F);
                        }
                        else
                        {
                            GUI.DrawTexture(new Rect(ProgressOffset.x + ProgressBarSize + PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                        }
                    }
                    else // Racer  finished, so we'll move the icon to end
                    {
                        if (!Gamer.IsPlayer) // if racer is not player itself
                        {
                            float fRivalPosition = (fActualLenght - Gamer.RacerDistance) * fDistancePixel; ;
                            if (!Gamer.RacerDestroyed) // If Racer still in race 
                            {
                                GUI.DrawTexture(new Rect(ProgressOffset.x + fRivalPosition + PlayerIconsOffset.x - RivalIcon.width / 2, PlayerIconsOffset.y, RivalIcon.width, RivalIcon.height), RivalIcon, ScaleMode.StretchToFill, true, 10.0F);
                            }
                            else // If Racer knocked out or destroyed by accident or whatever
                            {
                                GUI.DrawTexture(new Rect(ProgressOffset.x + fRivalPosition + PlayerIconsOffset.x - DestroyedIcon.width / 2, PlayerIconsOffset.y, DestroyedIcon.width, DestroyedIcon.height), DestroyedIcon, ScaleMode.StretchToFill, true, 10.0F);
                            }
                        }
                        else //if racer is player 
                        {
                            float fRacerPosition = (fActualLenght - Gamer.RacerDistance) * fDistancePixel;

                            //This will show how much race distance taken by user.
                            if (PlayerCompleteForeground != null)
                            {
                                GUI.DrawTexture(new Rect(PlayerCompleteOffset.x, PlayerCompleteOffset.y, fRacerPosition, PlayerCompleteForeground.height), PlayerCompleteForeground, ScaleMode.StretchToFill, true, 10.0F);
                            }

                            GUI.DrawTexture(new Rect(ProgressOffset.x + fRacerPosition + PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                            
                        }
                    }
                }
            }
            else
            {//Designtime racer  Offset settings.
                if (RacerIcon != null)
                {
                    GUI.DrawTexture(new Rect(ProgressOffset.x + PlayerIconsOffset.x - RacerIcon.width / 2, PlayerIconsOffset.y, RacerIcon.width, RacerIcon.height), RacerIcon, ScaleMode.StretchToFill, true, 10.0F);
                }
            }
            GUI.EndGroup();

        }
      
        [DoNotObfuscate]
        void OnGUI()
        {
            if (UISkin != null) GUI.skin = UISkin;
            if (ControlRotation == eProgressRotation.Horizontal)
            { CreateHorizontalStructure(); }
            else
            { CreateVerticalStructure(); }
        }


    }

}