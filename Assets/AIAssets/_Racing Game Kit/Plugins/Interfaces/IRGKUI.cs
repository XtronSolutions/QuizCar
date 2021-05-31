using System;
using UnityEngine;

namespace RacingGameKit.Interfaces
{
    
    public interface IRGKUI
    {
        bool ShowCountdownWindow { get; set; }
        //bool ShowPlacementWIndow { get; set; }
        //bool ShowCheckPointWindow { get; set; }
        bool ShowWrongWayWindow { get; set; }
        float CurrentCount { get; set; }

        void ShowResultsWindow();
        void RaceFinished(String RaceType);
        void PlayerCheckPointPassed(CheckPointItem PassedCheckpoint);
        
    }

}