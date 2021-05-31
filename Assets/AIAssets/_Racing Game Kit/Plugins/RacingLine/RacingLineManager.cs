//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Line Manager
// Last Change : 08/02/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================



using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit.Helpers;

namespace RacingGameKit
{

    [AddComponentMenu("Racing Game Kit/Racing Line/Racing Line Manager"), ExecuteInEditMode()]
    public class RacingLineManager : MonoBehaviour
    {

        // Maximum number of marks total handled by one instance of the script.


        public bool ShowHelperIcons = true;


        public bool ShowHelperSpline = true;

        public Color HelperSplineColor = Color.green;

        public bool ClosedRacingLine = false;


        public float RacingLineWidth = 1.6f;
        public bool StickToGround = false;
        public float GroundOffset = 0.02f;
        public int MeshResolution = 1024;
        private float textureOffset = 0f;
        public int TextureTile = 10; // How long must be the texture?

        public GameObject WaypointContainer;

        private int LineSoftnessFactor = 10;
        private int lastindex = -1;
        private float ColliRadius = 0.2f;
        private int numMarks = 0;
        private MarkSection[] RacingLineParts;
        private float Duration = 1f;
        private bool updateMesh = false;
        [HideInInspector]
        public bool enableCopyMode = false;


        public void ShowHideChildIcons(bool ShowIcons)
        {
            Transform[] trans = GetTransforms();
            foreach (Transform Child in trans)
            {
                RacingLineItem RLItem = Child.GetComponent(typeof(RacingLineItem)) as RacingLineItem;
                if (RLItem != null) RLItem.ShowIconGizmo = ShowIcons;
            }

        }

        public int AddLineNode(Vector3 pos, Vector3 normal, float intensity, int lastIndex)
        {
            if (intensity > 1) intensity = 1.0f;
            if (intensity < 0) return -1;

            MarkSection curr = RacingLineParts[numMarks % MeshResolution];
            curr.pos = pos + normal * GroundOffset;
            curr.normal = normal;
            curr.intensity = intensity;
            curr.lastIndex = lastIndex;

            if (lastIndex != -1)
            {
                MarkSection last = RacingLineParts[lastIndex % MeshResolution];
                Vector3 dir = (curr.pos - last.pos);
                Vector3 xDir = Vector3.Cross(dir, normal).normalized;

                curr.posl = curr.pos + xDir * RacingLineWidth * 0.5f;
                curr.posr = curr.pos - xDir * RacingLineWidth * 0.5f;
                curr.tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

                if (last.lastIndex == -1)
                {
                    last.tangent = curr.tangent;
                    last.posl = curr.pos + xDir * RacingLineWidth * 0.5f;
                    last.posr = curr.pos - xDir * RacingLineWidth * 0.5f;
                }
            }
            numMarks++;
            updateMesh = true;
            return numMarks - 1;
        }

        void UpdateMeshFilter()
        {
            if (!updateMesh)
            {
                return;
            }
            updateMesh = false;

            Mesh mesh = ((MeshFilter)GetComponent(typeof(MeshFilter))).mesh;
            mesh.Clear();
            int segmentCount = 0;

            for (int j = 0; j < numMarks && j < MeshResolution; j++)
                if (RacingLineParts[j].lastIndex != -1 && RacingLineParts[j].lastIndex > numMarks - MeshResolution)
                    segmentCount++;

            Vector3[] vertices = new Vector3[segmentCount * 4];
            Vector3[] normals = new Vector3[segmentCount * 4];
            Vector4[] tangents = new Vector4[segmentCount * 4];
            Color[] colors = new Color[segmentCount * 4];
            Vector2[] uvs = new Vector2[segmentCount * 4];
            int[] triangles = new int[segmentCount * 6];
            segmentCount = 0;

            for (int i = 0; i < numMarks && i < MeshResolution; i++)
                if (RacingLineParts[i].lastIndex != -1 && RacingLineParts[i].lastIndex > numMarks - MeshResolution)
                {
                    MarkSection curr = RacingLineParts[i];
                    MarkSection last = RacingLineParts[curr.lastIndex % MeshResolution];
                    float magnitude = (last.posr - curr.posr).magnitude;
                    float tiling = (magnitude / TextureTile) + textureOffset;

                    vertices[segmentCount * 4 + 0] = last.posl;
                    vertices[segmentCount * 4 + 1] = last.posr;
                    vertices[segmentCount * 4 + 2] = curr.posl;
                    vertices[segmentCount * 4 + 3] = curr.posr;

                    normals[segmentCount * 4 + 0] = last.normal;
                    normals[segmentCount * 4 + 1] = last.normal;
                    normals[segmentCount * 4 + 2] = curr.normal;
                    normals[segmentCount * 4 + 3] = curr.normal;

                    tangents[segmentCount * 4 + 0] = last.tangent;
                    tangents[segmentCount * 4 + 1] = last.tangent;
                    tangents[segmentCount * 4 + 2] = curr.tangent;
                    tangents[segmentCount * 4 + 3] = curr.tangent;

                    colors[segmentCount * 4 + 0] = new Color(0, 0, 0, last.intensity);
                    colors[segmentCount * 4 + 1] = new Color(0, 0, 0, last.intensity);
                    colors[segmentCount * 4 + 2] = new Color(0, 0, 0, curr.intensity);
                    colors[segmentCount * 4 + 3] = new Color(0, 0, 0, curr.intensity);

                    uvs[segmentCount * 4 + 0] = new Vector2(0, textureOffset);
                    uvs[segmentCount * 4 + 1] = new Vector2(1, textureOffset);
                    uvs[segmentCount * 4 + 2] = new Vector2(0, tiling);
                    uvs[segmentCount * 4 + 3] = new Vector2(1, tiling);

                    textureOffset = (magnitude / TextureTile) % 1 + textureOffset;

                    triangles[segmentCount * 6 + 0] = segmentCount * 4 + 0;
                    triangles[segmentCount * 6 + 2] = segmentCount * 4 + 1;
                    triangles[segmentCount * 6 + 1] = segmentCount * 4 + 2;

                    triangles[segmentCount * 6 + 3] = segmentCount * 4 + 2;
                    triangles[segmentCount * 6 + 5] = segmentCount * 4 + 1;
                    triangles[segmentCount * 6 + 4] = segmentCount * 4 + 3;
                    segmentCount++;
                }
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.tangents = tangents;
            mesh.triangles = triangles;
            mesh.colors = colors;
            mesh.uv = uvs;
            textureOffset = 0;
        }

        Transform[] GetTransforms()
        {
            List<Component> components;
            List<Transform> transforms;

            if (WaypointContainer != null)
            {
                components = new List<Component>(WaypointContainer.GetComponentsInChildren(typeof(Transform)));
                transforms = components.ConvertAll(c => (Transform)c);
                transforms.Remove(WaypointContainer.gameObject.transform);
            }
            else
            {
                components = new List<Component>(transform.GetComponentsInChildren(typeof(Transform)));
                transforms = components.ConvertAll(c => (Transform)c);
                transforms.Remove(transform.gameObject.transform);
            }

            transforms.Sort(delegate(Transform a, Transform b) { return System.Convert.ToInt32(a.name).CompareTo(System.Convert.ToInt32(b.name)); });
            return transforms.ToArray();
        }

        void OnDrawGizmos()
        {
            if (ShowHelperSpline)
            {
                Transform[] trans = GetTransforms();
                if (trans.Length < 2)
                    return;
                SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
                SetupSplineInterpolator(interp, trans);
                interp.StartInterpolation(null, false, eWrapMode.ONCE);

                Vector3 prevPos = trans[0].position;

                for (int c = 0; c <= LineSoftnessFactor; c++)
                {
                    float currTime = c * Duration / LineSoftnessFactor;
                    Vector3 currPos = interp.GetHermiteAtTime(currTime);
                    if (c == 0) currPos = trans[0].position;

                    Gizmos.color = HelperSplineColor;//new Color(mag, 0, 0, 1);
                    Gizmos.DrawLine(prevPos, currPos);
                    prevPos = currPos;
                }
            }
        }

        void CheckIfMeshAlreadyCreated()
        {
            lastindex = 1;
            RacingLineParts = new MarkSection[MeshResolution];

            for (int i = 0; i < MeshResolution; i++)
                RacingLineParts[i] = new MarkSection();

            MeshFilter RacingLineMesh = (MeshFilter)GetComponent(typeof(MeshFilter));

            if (RacingLineMesh.mesh == null)
            {
                RacingLineMesh.mesh = new Mesh();
            }


        }

        public void CopyWaypointItemsAsNode()
        {
            if (WaypointContainer == null) return;

            Transform[] Transforms = GetTransforms();

            foreach (Transform childTrans in Transforms)
            {
                GameObject NewRLItemObject = new GameObject(childTrans.name);
                //WayPointItem NewWPItemComponent = NewWPItemObject.AddComponent<WayPointItem>();
                NewRLItemObject.transform.position = childTrans.position;
                NewRLItemObject.transform.rotation = childTrans.rotation;
                NewRLItemObject.transform.parent = this.transform;
                RacingLineItem NewRLItemComponent = NewRLItemObject.AddComponent<RacingLineItem>();
            }
            enableCopyMode = false;
            WaypointContainer = null;
        }

        public void CreateRacingLine()
        {
            CheckIfMeshAlreadyCreated();

            Transform[] trans = GetTransforms();
            if (trans.Length < 2)
                return;

            if (MeshResolution < 16)
            {
                MeshResolution = 16;
                LineSoftnessFactor = MeshResolution - 2;
                return;
            }
            LineSoftnessFactor = MeshResolution - 2;

            SplineInterpolator interp = GetComponent(typeof(SplineInterpolator)) as SplineInterpolator;
            SetupSplineInterpolator(interp, trans);

            interp.StartInterpolation(null, false, eWrapMode.ONCE);

            Vector3 prevPos = trans[0].position;


            for (int c = 0; c <= LineSoftnessFactor; c++)
            {
                float currTime = c * Duration / LineSoftnessFactor;
                Vector3 currPos = interp.GetHermiteAtTime(currTime);

                SurfaceProperties oSurf = CastToCollider(GroundOffset, currPos, new Vector3(0f, -1f, 0f), 0, 1);

                lastindex = AddLineNode(oSurf.Position, oSurf.Normal, 1f, lastindex);
                prevPos = currPos;

            }

            UpdateMeshFilter();
            lastindex = -1;
            numMarks = 0;
        }

        public void UpdateRacingLine()
        {
            CreateRacingLine();
        }

        private SurfaceProperties CastToCollider(float GroundOffset, Vector3 fromPos, Vector3 forward, float minDistance, float maxDistance)
        {
            SurfaceProperties oSurf;
            if (!StickToGround)
            {
                oSurf = new SurfaceProperties();
                oSurf.Position = fromPos;
                oSurf.Normal = new Vector3(0, 1f, 0);
                return oSurf;
            }

            Vector3 NodePosition = fromPos;
            Vector3 NodeNormal = new Vector3(0, 1f, 0);

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
                NodePosition = hit.point;
                NodeNormal = hit.normal;
                //NodePosition += ((Vector3)(Vector3.up.normalized * GroundOffset));
            }
            else if (minDistance > 0f)
            {
                NodePosition = fromPos + ((Vector3)(forward.normalized * minDistance));
                NodePosition += ((Vector3)(Vector3.up.normalized * GroundOffset));
            }

            oSurf = new SurfaceProperties();
            oSurf.Position = NodePosition;
            oSurf.Normal = NodeNormal;
            return oSurf;
        }


        #region Spline


        //Drawn Spline for WayPoints
        void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
        {
            interp.Reset();

            float step = (ClosedRacingLine) ? (Duration / trans.Length) : Duration / (trans.Length - 1);

            int c;
            for (c = 0; c < trans.Length; c++)
            {
                interp.AddPoint(trans[c].position, trans[c].rotation, step * c, new Vector2(0, 1));
            }

            if (ClosedRacingLine)
                interp.SetAutoCloseMode(step * c);
        }

        #endregion
        [System.Serializable]
        class SurfaceProperties
        {
            public Vector3 Position = Vector3.zero;
            public Vector3 Normal = Vector3.zero;
        }

        [System.Serializable]
        public class MarkSection : System.Object
        {
            public Vector3 pos = Vector3.zero;
            public Vector3 normal = Vector3.zero;
            public Vector4 tangent = Vector4.zero;
            public Vector3 posl = Vector3.zero;
            public Vector3 posr = Vector3.zero;
            public float intensity = 0.0f;
            public int lastIndex = 0;
        };

    }
}