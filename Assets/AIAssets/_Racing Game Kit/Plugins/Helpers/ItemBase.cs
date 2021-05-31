//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Item Base Script
// This script used visualise and alingments for CheckPoints,WayPoints and SpawnPoints. 
// Last Change : 22/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using System;
using UnityEngine;
using System.Collections;

namespace RacingGameKit
{
    [AddComponentMenu("")]
    public partial class ItemBase : MonoBehaviour
    {
        private float ColliRadius = 0.2f;

    

        public void AlignToTerrain()
        {
            Vector3 forward = new Vector3(0f, -1f, 0f);
            this.CastToCollider(base.transform.position, forward, 0f, 0f);
        }

        /// <summary>
        /// Find the nearest position of colliding object and move the object to this location
        /// </summary>
        public void CastToCollider(Vector3 fromPos, Vector3 forward, float minDistance, float maxDistance)
        {
            RaycastHit hit;
            Ray ray = new Ray(fromPos, forward);
            bool flag = false;
            if (maxDistance > 0f)
            {
                flag = Physics.SphereCast(ray, this.ColliRadius, out hit, maxDistance);
            }
            else
            {
                flag = Physics.SphereCast(ray, this.ColliRadius, out hit);
            }
            if (flag)
            {
                base.transform.position = hit.point;
                base.transform.position +=  ((Vector3)(Vector3.up.normalized * 0.5f));
            }
            else if (minDistance > 0f)
            {
                base.transform.position = fromPos + ((Vector3)(forward.normalized * minDistance));
                base.transform.position += ((Vector3)(Vector3.up.normalized * 0.5f));
            }
        }
    }
}