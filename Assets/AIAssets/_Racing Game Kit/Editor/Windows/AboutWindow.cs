
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using System.Text;
using RacingGameKit.Editors.Helpers;


namespace RacingGameKit.Editors.Windows
{
    public class AboutWindow : EditorWindow
    {
        AboutWindow window;

        static void CallCreateWindow()
        {
            AboutWindow window = (AboutWindow)EditorWindow.GetWindow(typeof(AboutWindow));
            window.autoRepaintOnSceneChange = true;
            window.title = "About RGK";
            window.minSize = new Vector2(700f, 350f);
            window.maxSize = new Vector2(700f, 350f);
            window.window = window;
            window.Show();

           
        }


        private bool iChecked = false;


        
        Vector2 scroll = Vector2.zero;
        JSONObjectForKit oJ = null;

        void OnGUI()
        {
            DrawGUI();
        }

        internal void DrawGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Box(GUIHelper.AboutWindowBGWithLogo, GUIStyle.none, GUILayout.Width(700), GUILayout.Height(350));
            GUI.Label(new Rect(24, 240, 400, 25), "Copyright © 2011-2014  Yusuf AKDAG.  All Rights Reserved.", GUIHelper.AboutWindowLabel11);
            GUI.Label(new Rect(24, 265, 400, 25), "Used under license by Unity Technologies, all other ", GUIHelper.AboutWindowLabel11);
            GUI.Label(new Rect(24, 280, 400, 25), "trademarks are the property of their respective owners.", GUIHelper.AboutWindowLabel11);
            GUI.Label(new Rect(24, 295, 400, 25), "Yusuf AKDAG is not affiliated to Unity Technologies.", GUIHelper.AboutWindowLabel11);


            GUI.Label(new Rect(330, 20, 400, 30), "Racing Game Kit", GUIHelper.AboutWindowLabel24);
            GUI.Label(new Rect(330, 55, 400, 25), "Super Cool Racing Games, in Just a Few Minutes", GUIHelper.AboutWindowLabel12Bold);
            GUI.Label(new Rect(550, 33, 400, 25), "v"+ Version.VERSIONNO, GUIHelper.AboutWindowLabel12);

            GUI.Label(new Rect(330, 100, 400, 35), "This Product is Licensed under RGK Software \r\nLicense Terms To :", GUIHelper.AboutWindowLabel12Bold);



          
            GUI.Label(new Rect(430, 200, 400, 20), "Source Version", GUIHelper.AboutWindowLabel12Bold);
          

            GUILayout.EndVertical();
        }
    

        private void WindowClosed()
        {
            iChecked = false;
            DrawGUI();
        }

        private string Clean(String What)
        {
            return What.Replace("\"", "");
        }

    }
}