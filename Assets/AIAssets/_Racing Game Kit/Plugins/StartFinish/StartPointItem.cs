//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// StartPoint Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using System.Collections;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Start Finish/Start Point"), ExecuteInEditMode]
    public class StartPointItem : MonoBehaviour //RacingGameKit.ItemBase
    {
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "icon_startpoint.tif", false);
        }
    }
}
