//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Minimap Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RacingGameKit
{
    //[AddComponentMenu("Racing Game Kit/UI/Game MiniMap"), RequireComponent(typeof(RaceManager)), ExecuteInEditMode]
    [AddComponentMenu(""), RequireComponent(typeof(Race_Manager)), ExecuteInEditMode]
    public class Race_MiniMap : MonoBehaviour
    {
        public Texture2D MapBase;
        public Texture2D MapOutline;
        public float MapSize = 512f;



        public Texture2D PlayerIcon;
        public Texture2D[] RivalIcons;

        private float iScreenWidth = 0;
        private float iScreenHeigt = 0;
        public GUISkin UISkin;

        private Rect windowMap;
        public bool _showMapWindow = true;

        private Race_Manager _RaceManager = null;
        private List<Racer_Detail> RacerList;


        //Minimap Offset Stuff - this will help to determinate map resulation and object distance between offset points.
        //This part developed because map size and shape must editable by an artists easily.
        public bool DebugMode = true;
        public Vector2 MapOffset1;
        public Vector2 MapOffset2;
        public Texture2D MapOffsetTexture;

        private Transform OffsetPoint1;
        private Transform OffsetPoint2;
        public float Resulation = 0f;
        public float Resulation2 = 0;
        void Start()
        {
            iScreenWidth = Screen.width;
            iScreenHeigt = Screen.height;

            _RaceManager = base.GetComponent(typeof(Race_Manager)) as Race_Manager;
            RacerList = _RaceManager.RegisteredRacers;

            GameObject MinimapHelperObject = GameObject.Find("_MiniMapHelper");
            if (MinimapHelperObject == null)
            {
                Debug.LogWarning("RST KIT WARNING : Minimap Helper Object not dropped into hierarchy or its disabled! Please consult documentation! ");
            }
            else
            {
                OffsetPoint1 = MinimapHelperObject.transform.Find("_miniMap_OffsetPoint1");
                OffsetPoint2 = MinimapHelperObject.transform.Find("_miniMap_OffsetPoint2");
            }
        }

        void Update()
        {

            if (OffsetPoint1 != null || OffsetPoint2 != null)
            {
                windowMap = new Rect(150, 50, MapSize, MapSize);
                Resulation = Vector3.Distance(OffsetPoint1.position, OffsetPoint2.position); //offset objeleri arasindaki mesafe
                Resulation2 = Vector2.Distance(MapOffset1, MapOffset2) / MapSize; //birimpixel = iki obje arasindaki mesafenin pixel cinsinden degeri
            }
        }

        void OnGUI()
        {
            if (UISkin != null) GUI.skin = UISkin;

            if (_showMapWindow)
            {
                windowMap = GUI.Window(5, windowMap, RenderMapWindow, "", "MapWindow");
            }
        }


        void RenderMapWindow(int windowID)
        {
            // GUI.DrawTexture(new Rect(0, 0, MapOutline.width, MapOutline.height), MapOutline);

            GUI.DrawTexture(new Rect(0, 0, MapSize, MapSize), MapBase);
            if (DebugMode)
            {
                GUI.DrawTexture(new Rect(MapOffset1.x, MapOffset1.y, 3, 3), MapOffsetTexture);
                GUI.DrawTexture(new Rect(MapOffset2.x, MapOffset2.y, 3, 3), MapOffsetTexture);
            }

            if (RacerList != null)
            {
                foreach (Racer_Detail Racer in RacerList)
                {
                    Vector3 RacerPosition = Racer.RacerPostionOnMap;
                    float RacerTop = MapSize - RacerPosition.z;
                    float RacerLeft = RacerPosition.x;
                    float RacerMiniTop = RacerTop * Resulation2 + MapOffset1.y;
                    float RacerMiniLeft = RacerLeft * Resulation2 + MapOffset2.x;

                    if (Racer.IsPlayer)
                    {
                        Matrix4x4 IconMatrix = GUI.matrix;
                        Vector2 IconCenter = new Vector2(RacerMiniLeft + PlayerIcon.width / 2, RacerMiniTop + PlayerIcon.height / 2);

                        float IconAngle = Racer.RacerRotationOnMap.eulerAngles.y;
                        GUIUtility.RotateAroundPivot(IconAngle, IconCenter);
                        GUI.DrawTexture(new Rect(RacerMiniLeft, RacerMiniTop, PlayerIcon.width, PlayerIcon.height), PlayerIcon);
                        GUI.matrix = IconMatrix;
                    }
                    else
                    {
                        Matrix4x4 IconMatrix = GUI.matrix;
                        Vector2 IconCenter = new Vector2(RacerMiniLeft + RivalIcons[0].width / 2, RacerMiniTop + RivalIcons[0].height / 2);

                        float IconAngle = Racer.RacerRotationOnMap.eulerAngles.y;
                        GUIUtility.RotateAroundPivot(IconAngle, IconCenter);
                        GUI.DrawTexture(new Rect(RacerMiniLeft, RacerMiniTop, RivalIcons[0].width, RivalIcons[0].height), RivalIcons[0]);
                        GUI.matrix = IconMatrix;
                    }
                }

            }
        }
    }
}