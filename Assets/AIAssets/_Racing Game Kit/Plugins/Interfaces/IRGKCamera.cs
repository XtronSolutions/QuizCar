//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Game Camera Interface
// Last Change : 27/08/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections.Generic;
namespace RacingGameKit.Interfaces
{
    public interface IRGKCamera
    {

        /// <summary>
        /// Current target vehicle. If any Human Racer added to scene, this racer will default assinged to this value.
        /// </summary>
        Transform TargetVehicle { set; }
        /// <summary>
        /// Target ai vehicle transforms that used by switching AI racers.
        /// </summary>
        List<Transform> TargetObjects{get;set;}
        /// <summary>
        /// Enable or disable camera animations on startup. This value provided from RaceManager
        /// </summary>
        Boolean IsStartupAnimationEnabled {set;}

		Boolean IsShakeCameraOnHitObstacle { set;}

		Boolean IsShakeCameraOnNitro { set;}
        /// <summary>
        /// Current countdown for camera animations before start. This value provided from RaceManager
        /// </summary>
        int CurrentCount {set;}
    }

}