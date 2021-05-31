//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Game Kit Menu
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Editors;
using RacingGameKit.Editors.Helpers;
using RacingGameKit.Editors.Windows;

namespace RacingGameKit.Editors
{
    public class RacingGameKitMenu : Editor
    {
        //[MenuItem("Racing Game Kit/New Racing Game Wizard")]
        //private static void RacingGameWizard()
        //{
        // Disabled for next version! 
        //} 

        [MenuItem("Racing Game Kit/Mechanics/Create Race Manager")]
        private static void CreateGameManager()
        {
            CoreFunctions.CreateRaceManager();
        }
        [MenuItem("Racing Game Kit/Mechanics/Create Waypoint Container")]
        private static void CreateWaypointContainer()
        {
            CoreFunctions.CreateWayPointContainer();
        }
        [MenuItem("Racing Game Kit/Mechanics/Create SpawnPoint Container")]
        private static void CreateSpawnPointContainer()
        {
            CoreFunctions.CreateSpawnPointContainer();
        }
        [MenuItem("Racing Game Kit/Mechanics/Create CheckPoint Container")]
        private static void CreateCheckPointContainer()
        {
            CoreFunctions.CreateCheckPointContainer();
        }
        [MenuItem("Racing Game Kit/Mechanics/Create Start Point")]
        private static void CreateStartPoint()
        {
            CoreFunctions.CreateStartPoint();
        }
        [MenuItem("Racing Game Kit/Mechanics/Create Finish Point")]
        private static void CreateFinishPoint()
        {
            CoreFunctions.CreateFinishPoint();
        }
        [MenuItem("Racing Game Kit/Components/Add Audio Manager")]
        private static void AddSoundManager()
        {
            CoreFunctions.AddSoundManager();
        }
        [MenuItem("Racing Game Kit/Components/Add Vertical Race Progress")]
        private static void AddVerticalProgress()
        {
            CoreFunctions.AddVerticalProgress();
        }
        [MenuItem("Racing Game Kit/Components/Add Horizontal Race Progress")]
        private static void AddHorizontalProgress()
        {
            CoreFunctions.AddHorizontalProgress();
        }
        //[MenuItem("Racing Game Kit/Components/Add 2D MiniMap")]
        //private static void Add2DMinimap()
        //{ }
        //[MenuItem("Racing Game Kit/Components/Add 3D Minimap")]
        //private static void Add3DMinimap()
        //{ }
        [MenuItem("Racing Game Kit/Waypoint Editor/New Waypoint %w")]
        private static void CreateNewWaypointItem()
        {
            CoreFunctions.CreateWayPointItem();
        }
        [MenuItem("Racing Game Kit/Waypoint Editor/Align Waypoint to Surface %e")]
        private static void AlignWaypointToSurface()
        {
            CoreFunctions.AlignToSurface();
        }

        [MenuItem("Racing Game Kit/About")]
        private static void ShowAbout()
        {
            AboutWindow window = (AboutWindow)EditorWindow.GetWindow(typeof(AboutWindow), false, "About RGK");
            window.minSize = new Vector2(700f, 350f);
            window.maxSize = new Vector2(700f, 350f);
            
            window.Show();
            
            
        }

    }
}