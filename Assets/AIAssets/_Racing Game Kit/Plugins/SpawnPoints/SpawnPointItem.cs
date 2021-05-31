//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// SpawnPoint Item Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;
namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/SpawnPoints/SpawnPoint"), ExecuteInEditMode()]
    public class SpawnPointItem : ItemBase
    {
        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "icon_spawnpoint.tif");
        }
    }
}