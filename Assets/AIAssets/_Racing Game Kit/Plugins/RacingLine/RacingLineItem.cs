//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Line Item Script
// Last Change : 08/02/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using System.Collections;
using SmartAssembly.Attributes;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Racing Line/Racing Line Item"), ExecuteInEditMode()]
    public class RacingLineItem : ItemBase
    {
         
        [HideInInspector]
        public bool ShowIconGizmo = true;

        [HideInInspector]
        public Color GizmoColor= Color.green;

        void OnDrawGizmos()
        {
            if (ShowIconGizmo)
            {
                Gizmos.DrawIcon(transform.position, "icon_racingline.tif");
            }
        }
    }
}