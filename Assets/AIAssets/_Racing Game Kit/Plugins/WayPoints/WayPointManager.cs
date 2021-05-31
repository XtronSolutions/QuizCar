//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// WayPoint Manager
// Last Change : 10/08/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Helpers;
namespace RacingGameKit
{
    [AddComponentMenu("Racing Game Kit/Waypoint/WayPoint Manager"), RequireComponent(typeof(SplineInterpolator)), ExecuteInEditMode()]
    public class WayPointManager : MonoBehaviour
    {
        


        [HideInInspector]
        public bool ShowHelperIcons = true;

        [HideInInspector]
        public bool ShowHelperSpline = true;
        /// <summary>
        /// Close spline from start to end.
        /// </summary>
        [HideInInspector]
        public bool CloseSpline = false;
        /// <summary>
        /// Color for the waypoint visualiation spline
        /// </summary>
        [HideInInspector]
        public Color HelperSplineColor = Color.red;

        /// <summary>
        /// How soft will be spline curves. This value dummy and actual value is waypoint count * 3.
        /// Also this value must not lower then waypoint count.
        /// </summary>
        private int SplineSoftnessFactor = 10;

        //Initializator, ignore
        SplineInterpolator mSplineInterp = null;
        float Duration = 10f;

        ////v1.1 Feature
        //public bool RebuildAreas = false;
        //private Mesh SpeedAreaMesh;
        //private MeshFilter SpeedAreaMeshFilter;
        //private MeshRenderer SpeedAreaRenderer;
        //private Material SpeedAreaMaterial;


        void Start()
        {
            mSplineInterp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
        }

        /// <summary>
        /// Returns children transforms, sorted by name.
        /// </summary>
        Transform[] GetTransforms()
        {
            List<Component> components = new List<Component>(this.gameObject.GetComponentsInChildren(typeof(Transform)));
            List<Transform> transforms = components.ConvertAll(c => (Transform)c);

            transforms.Remove(this.gameObject.transform);
            transforms.Sort(delegate(Transform a, Transform b) { return System.Convert.ToInt32(a.name).CompareTo(System.Convert.ToInt32(b.name)); });
            SplineSoftnessFactor = transforms.Count * 3;
            return transforms.ToArray();
        }

        /// <summary>
        /// Disables the spline objects, we don't need them outside design-time.
        /// </summary>


        void OnDrawGizmos()
        {
            if (ShowHelperSpline)
            {
                Transform[] trans = GetTransforms();
                if (trans.Length < 2)
                    return;

                if (SplineSoftnessFactor < trans.Length) SplineSoftnessFactor = trans.Length;

                SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
                SetupSplineInterpolator(interp, trans);
                interp.StartInterpolation(null, false, eWrapMode.ONCE);

                Vector3 prevPos = trans[0].position;
                for (int c = 1; c <= SplineSoftnessFactor; c++)
                {
                    float currTime = c * Duration / SplineSoftnessFactor;
                    Vector3 currPos = interp.GetHermiteAtTime(currTime);

                    Gizmos.color = HelperSplineColor;//new Color(mag, 0, 0, 1);
                    Gizmos.DrawLine(prevPos, currPos);

                    prevPos = currPos;

                }
            }

        }

        public void ShowHideChildIcons(bool ShowIcons)
        {
            Transform[] trans = GetTransforms();
            foreach (Transform Child in trans)
            {
                WayPointItem WPItem = Child.GetComponent(typeof(WayPointItem)) as WayPointItem;
                if (WPItem!=null)WPItem.ShowIconGizmo = ShowIcons;
            }
        
        }
         

        void FixedUpdate()
        {
            
            Vector3 Zero = new Vector3(0,0,0);
            if (transform.position != Zero)
            {
                transform.position = Zero;
            }
        }

        #region Spline 


        //Drawn Spline for WayPoints
        void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
        {
            interp.Reset();

            float step = (CloseSpline) ? (Duration / trans.Length) : Duration / (trans.Length - 1);

            int c;
            for (c = 0; c < trans.Length; c++)
            {
                interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
            }

            if (CloseSpline)
                interp.SetAutoCloseMode(step * c);
        }
        #endregion
        #region SpeedAreaVisualisation v.1.1



        //private void buildMesh()
        //{
        //    SpeedAreaMesh = new Mesh();
        //    SpeedAreaMesh.name = "SpeedAreaMesh";
        //    SpeedAreaMesh.Clear();

        //    Transform[] ChildWPs = GetTransforms();
        //    Vector3[] Vertices = new Vector3[(ChildWPs.Length) * 12];
        //    int[] Tris = new int[(ChildWPs.Length) * 12];
        //    WayPointItem WP1;
        //    WayPointItem WP2;
        //    int y = 0;
        //    for (int i = 0; i < ChildWPs.Length; i++)
        //    {
        //        if (i + 1 < ChildWPs.Length)
        //        {
        //            WP1 = ChildWPs[i].GetComponent(typeof(WayPointItem)) as WayPointItem;
        //            WP2 = ChildWPs[i + 1].GetComponent(typeof(WayPointItem)) as WayPointItem;

        //            //Vertices[0] = WP2.LeftLine;
        //            //Vertices[1] = WP1.transform.position;
        //            //Vertices[2] = WP1.LeftLine;

        //            //Vertices[3] = WP2.transform.position;
        //            //Vertices[4] = WP1.transform.position;
        //            //Vertices[5] = WP1.LeftLine;

        //            Vertices[y] = WP1.LeftLine;
        //            Vertices[y + 1] = WP2.LeftLine;
        //            Vertices[y + 2] = WP2.transform.position;

        //            Vertices[y + 3] = WP2.transform.position;
        //            Vertices[y + 4] = WP1.transform.position;
        //            Vertices[y + 5] = WP1.LeftLine;

        //            Vertices[y + 6] = WP2.transform.position;
        //            Vertices[y + 7] = WP1.RightLine;
        //            Vertices[y + 8] = WP1.transform.position;

        //            Vertices[y + 9] = WP1.RightLine;
        //            Vertices[y + 10] = WP2.transform.position;
        //            Vertices[y + 11] = WP2.RightLine;


        //            Tris[y + 0] = y;
        //            Tris[y + 1] = y + 1;
        //            Tris[y + 2] = y + 2;

        //            Tris[y + 3] = y;
        //            Tris[y + 4] = y + 3;
        //            Tris[y + 5] = y + 4;

        //            Tris[y + 6] = y + 6;
        //            Tris[y + 7] = y + 7;
        //            Tris[y + 8] = y + 8;

        //            Tris[y + 9] = y + 9;
        //            Tris[y + 10] = y + 10;
        //            Tris[y + 11] = y + 11;
        //            Debug.Log("Created");

        //            y += 12;
        //        }
        //    }


        //    Vector2[] myUVs = new Vector2[Vertices.Length]; // Create array with the same element count
        //    for (int i = 0; i < Vertices.Length; i++)
        //    {
        //        myUVs[i] = new Vector2(Vertices[i].x, Vertices[i].y);
        //    }

        //    SpeedAreaMesh.vertices = Vertices;
        //    SpeedAreaMesh.uv1 = myUVs;

        //    SpeedAreaMesh.triangles = Tris;
        //    SpeedAreaMesh.RecalculateNormals();
        //    SpeedAreaMesh.RecalculateBounds();

        //    if (gameObject.GetComponent(typeof(MeshFilter)) == null)
        //    {
        //        SpeedAreaMeshFilter = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        //        SpeedAreaRenderer = (MeshRenderer)gameObject.AddComponent(typeof(MeshRenderer));
        //    }
        //    else
        //    {
        //        SpeedAreaMeshFilter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
        //        SpeedAreaRenderer = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
        //    }
        //    SpeedAreaMeshFilter.mesh = SpeedAreaMesh;
        //    SpeedAreaRenderer.renderer.material.color = Color.yellow;
        //}

        #endregion
    }
}