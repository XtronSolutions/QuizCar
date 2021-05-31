//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// CheckPoint Item Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;

namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/CheckPoints/CheckPoint Item"), ExecuteInEditMode()]
    public class CheckPointItem : ItemBase
    {
        [HideInInspector]
        public float CheckpointTime;

        [HideInInspector]
        public float CheckpointBonus;

        [HideInInspector]
        public bool ShowIcons = true;

        [HideInInspector]
        public eCheckpointItemType ItemType = eCheckpointItemType.Checkpoint;


        void OnDrawGizmos()
        {
            if (ShowIcons)
            {
                if (ItemType == eCheckpointItemType.Checkpoint) Gizmos.DrawIcon(transform.position, "icon_checkpoint.tif");
                if (ItemType == eCheckpointItemType.Sector) Gizmos.DrawIcon(transform.position, "icon_sector.tif");
                if (ItemType == eCheckpointItemType.SpeedTrap) Gizmos.DrawIcon(transform.position, "icon_camera.tif");
            }
        }
    }
}