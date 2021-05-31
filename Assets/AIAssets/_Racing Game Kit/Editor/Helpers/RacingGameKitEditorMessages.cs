//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Game Kit Messages
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;

namespace RacingGameKit.Editors.Helpers
{
    internal class EditorMessages
    {
        public const string WPObjectExists                  = "RACING GAME KIT WARNING\r\n_Waypoints object already exits!";
        public const string WPObjectNotCreatedForStart      = "RACING GAME KIT WARNING\r\n_Waypoints object not created or empty!\r\nIts better if you create _StartPoint object on same position with first waypoint object.\r\nAre you sure want to create _StartPoint anyway?";
        public const string WPObjectNotCreatedForFinish     = "RACING GAME KIT WARNING\r\n_Waypoints object not created or empty!\r\nIts better if you create _FinishPoint object on same position with last waypoint object.\r\nAre you sure want to create _FinishPoint anyway?";
        public const string FirstWPItemNotFound             = "RACING GAME KIT WARNING\r\n_Waypoints object not created or its empty!\r\n";
        public const string SPObjectExists                  = "RACING GAME KIT WARNING\r\n_Spawnpoints object already exits!";
        public const string GMObjectExists                  = "RACING GAME KIT WARNING\r\n_RaceManager object already exits!";
        public const string RCLObjectExists                 = "RACING GAME KIT WARNING\r\n_RacingLineManager object already exits!";
        public const string GMObjectNotCreated              = "RACING GAME KIT WARNING\r\n_RaceManager object not crated yet!";
        public const string SPointExists                    = "RACING GAME KIT WARNING\r\n_StartPoint object already exits!";
        public const string FPointExists                    = "RACING GAME KIT WARNING\r\n_FinishPoint object already exits!";
        public const string ActivationScreenHelp            = "You can authorize or deauthorize your computer via this screen. \r\nFor authorization, please enter your registration email and password used on unityracingkit.com. You have to deauthorize this computer before install another computer. \n\r \n\rConsult product documentation for detailed instructions.";
    }
}

