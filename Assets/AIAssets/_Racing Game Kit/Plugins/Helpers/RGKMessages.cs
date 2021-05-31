//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Game Kit Messages 
// Last Change : 22/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

namespace RacingGameKit.Helpers
{
    using System;

    public static class RGKMessages
    {
        public const string RaceManagerMissing          = "RACING GAME KIT WARNING\r\nRaceManager object is missing!";
        public const string DistanceCheckerMissing      = "RACING GAME KIT WARNING\r\nDistance checker object not found! Be sure _DistancePoint named object placed under child!";
        public const string RacerTagObjectsMissing      = "RACING GAME KIT WARNING\r\nRacerTag objects is missing! Racer name and standing will not shown above vehicles ";
        public const string SpawnPointsObjectMissing    = "RACING GAME KIT WARNING\r\nSpawnpints object is missing! ";
        public const string GameCameraMissing           = "RACING GAME KIT WARNING\r\nGameCamera is missing! Racing Game Kit requires a camera script that implement iRGKCamera";
        public const string FinishPointMissing          = "RACING GAME KIT WARNING\r\nFinishPoint is missing";
        public const string CheckpointSystemDisabled    = "RACING GAME KIT INFORMATION\r\nCheckpoint container not found or empty. Checkpoint System disabled";
        public const string GameCameraNotAttached       = "RACING GAME KIT WARNING\r\nGameCamera not attached to Game Audio script. Please be sure _GameCamera object created or attached. Game Audio Disabled.";
    }
}
