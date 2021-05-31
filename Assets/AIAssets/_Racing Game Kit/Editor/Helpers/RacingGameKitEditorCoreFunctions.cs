//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Racing Game Kit Editor Functions
// Last Change : 3/10/201#
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using RacingGameKit;
using RacingGameKit.RGKCar.CarControllers;
using RacingGameKit.Helpers;
using RacingGameKit.Editors.Helpers;
using System.Reflection;


namespace RacingGameKit.Editors.Helpers
{
    public static class CoreFunctions
    {

        #region Waypoint

        /// <summary>
        /// Creates WayPointContaner for waypoint objects. 
        /// </summary>
        public static void CreateWayPointContainer()
        {
            GameObject WPContainer = GameObject.Find("_WayPoints");
            if (WPContainer != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.WPObjectExists);
                return;
            }

            WPContainer = new GameObject("_WayPoints");
            WPContainer.transform.position = new Vector3(0, 0, 0);
            SplineInterpolator SIComponent = WPContainer.AddComponent<SplineInterpolator>();
            SIComponent.name = SIComponent.name;//Debug warning removal
            WayPointManager WPManager = WPContainer.AddComponent<WayPointManager>();
            WPManager.name = WPManager.name;//Debug warning removal
            Selection.activeGameObject = WPContainer;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.Waypoints = WPContainer;
            }
        }


        /// <summary>
        /// Creates New WayPoint Item
        /// </summary>
        public static void CreateWayPointItem()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent(typeof(WayPointManager)) != null)
                {
                    CreateWaypointItemFromManager(Selection.activeGameObject.GetComponent(typeof(WayPointManager)) as WayPointManager);
                }
                else
                    if (Selection.activeGameObject.GetComponent(typeof(WayPointItem)) != null)
                    {
                        CreateWaypointItemFromItem(Selection.activeGameObject.GetComponent(typeof(WayPointItem)) as WayPointItem);
                    }
            }
        }

        public static void InsertWaypointItem(WayPointItem SelectedWaypoint)
        {
            if (SelectedWaypoint != null)
            {
                InsertWaypointAfterSelectedWaypoint(SelectedWaypoint);
            }
        }

        public static void DeleteWaypointItem(WayPointItem SelectedWaypoint)
        {
            if (SelectedWaypoint != null)
            {
                DeleteSelectedWaypoint(SelectedWaypoint);
            }
        }


        public static void FixWayPointRotations(WayPointManager WPManager)
        {
            List<Component> components = new List<Component>(WPManager.gameObject.GetComponentsInChildren(typeof(Transform)));
            List<Transform> transforms = components.ConvertAll(c => (Transform)c);

            transforms.Remove(WPManager.gameObject.transform);
            transforms.Sort(delegate(Transform a, Transform b)
            {
                return a.name.CompareTo(b.name);
            });
            Transform[] WPItems = transforms.ToArray();
            for (int i = 0; i < WPItems.Length - 1; i++)
            {
                if (WPItems[i + 1] != null)
                {
                    WPItems[i].transform.LookAt(WPItems[i + 1].transform);
                }

                if (i == WPItems.Length - 1)
                {
                    WPItems[i].transform.LookAt(WPItems[0].transform);
                }
            }
        }
        private static void CreateWaypointItemFromManager(WayPointManager WPManager)
        {
            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetWaypointName(WPManager.gameObject).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (WPManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                }
            }

            GameObject NewWPItemObject = new GameObject(NewItemName);
            WayPointItem NewWPItemComponent = NewWPItemObject.AddComponent<WayPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewWPItemComponent.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewWPItemObject.transform.parent = WPManager.gameObject.transform;
            NewWPItemComponent.AlignToTerrain();
            Selection.activeGameObject = NewWPItemObject;
        }

        private static void CreateWaypointItemFromItem(WayPointItem WPItem)
        {
            GameObject WPManagerObject = WPItem.gameObject.transform.parent.gameObject;

            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetWaypointName(WPManagerObject).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (WPManagerObject.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                }
            }

            GameObject NewWPObject = new GameObject(NewItemName);
            WayPointItem NewWPItem = NewWPObject.AddComponent<WayPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewWPItem.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewWPObject.transform.parent = WPManagerObject.gameObject.transform;
            NewWPItem.AlignToTerrain();
            Selection.activeGameObject = NewWPObject;
        }

        private static void DeleteSelectedWaypoint(WayPointItem SelectedWaypoint)
        {
            GameObject WPManagerObject = SelectedWaypoint.gameObject.transform.parent.gameObject;
            string oldWPItemname = SelectedWaypoint.name;

            Transform[] WPItems = GetChildTransforms(WPManagerObject.transform);

            int CurrentWPIndex = System.Array.FindIndex(WPItems, tr => tr.name == SelectedWaypoint.name);


            SelectedWaypoint.name = "DELETED_WP";


            for (int i = WPItems.Length; i > CurrentWPIndex + 1; i--)
            {
                string NextItemName = (i - 1).ToString();

                if (NextItemName.Length < 2)
                    NextItemName = "0" + NextItemName;

                WPItems[i - 1].name = NextItemName;
            }
            Selection.activeGameObject = WPItems[CurrentWPIndex - 1].gameObject;


            GameObject DeleteParent = GameObject.Find("___SAFEDELETE");
            bool showDeletedMessage = true;

            if (DeleteParent == null)
            {
                DeleteParent = new GameObject("___SAFEDELETE");
                SelectedWaypoint.transform.parent = DeleteParent.transform;
                GameObject.DestroyImmediate(SelectedWaypoint, true);
                showDeletedMessage = true;
            }
            else
            {
                SelectedWaypoint.transform.parent = DeleteParent.transform;
                GameObject.DestroyImmediate(SelectedWaypoint, true);
                showDeletedMessage = true;
            }

            if (showDeletedMessage)
            {
                EditorUtility.DisplayDialog("Item moved to safe deletion!",
                "Waypoint Item " + oldWPItemname + " moved to under ___SAFEDELETE gameobject in project hierarchy. \r\nYou can remove this item safely now!", "OK");
            }


        }



        private static void InsertWaypointAfterSelectedWaypoint(WayPointItem SelectedWaypoint)
        {
            GameObject WPManagerObject = SelectedWaypoint.gameObject.transform.parent.gameObject;

            Transform[] WPItems = GetChildTransforms(WPManagerObject.transform);

            int CurrentWPIndex = System.Array.FindIndex(WPItems, tr => tr.name == SelectedWaypoint.name);

            for (int i = WPItems.Length; i > CurrentWPIndex + 1; i--)
            {
                string NextItemName = (i + 1).ToString();
                if (NextItemName.Length < 2)
                    NextItemName = "0" + NextItemName;

                WPItems[i - 1].name = NextItemName;
            }


            string NewItemName = (CurrentWPIndex + 2).ToString();
            if (NewItemName.Length < 2)
                NewItemName = "0" + NewItemName;


            GameObject NewItem = new GameObject(NewItemName);
            WayPointItem NewWPItemComponent = NewItem.AddComponent<WayPointItem>();

            NewItem.transform.parent = WPManagerObject.gameObject.transform;
            NewItem.transform.position = SelectedWaypoint.transform.position;

            if (WPItems[CurrentWPIndex + 1] != null)
            {
                NewItem.transform.LookAt(WPItems[CurrentWPIndex + 1].transform);
            }

            Selection.activeGameObject = NewItem;
        }

        private static int GetWaypointName(GameObject WPManager)
        {
            return new List<Component>(WPManager.GetComponentsInChildren(typeof(WayPointItem))).Count + 1;
        }
        #endregion


        #region RacingLine
        public static void CreateRacingLineItem()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent(typeof(RacingLineManager)) != null)
                {
                    CreateRacingLineItemFromManager(Selection.activeGameObject.GetComponent(typeof(RacingLineManager)) as RacingLineManager);
                }
                else
                    if (Selection.activeGameObject.GetComponent(typeof(RacingLineItem)) != null)
                    {
                        CreateRacingLineItemFromItem(Selection.activeGameObject.GetComponent(typeof(RacingLineItem)) as RacingLineItem);
                    }
            }
        }

        private static void CreateRacingLineItemFromManager(RacingLineManager RLManager)
        {
            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetRacingLineNodeName(RLManager.gameObject).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (RLManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                }
            }

            GameObject NewRLItemObject = new GameObject(NewItemName);
            RacingLineItem NewRLItemComponent = NewRLItemObject.AddComponent<RacingLineItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewRLItemComponent.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewRLItemObject.transform.parent = RLManager.gameObject.transform;
            NewRLItemComponent.AlignToTerrain();
            Selection.activeGameObject = NewRLItemObject;
        }

        private static void CreateRacingLineItemFromItem(RacingLineItem RLItem)
        {
            GameObject RacingLineManager = RLItem.gameObject.transform.parent.gameObject;

            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetRacingLineNodeName(RacingLineManager).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (RacingLineManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                }
            }

            GameObject NewRLItemObject = new GameObject(NewItemName);
            RacingLineItem NewRLItemComponent = NewRLItemObject.AddComponent<RacingLineItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewRLItemComponent.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewRLItemObject.transform.parent = RacingLineManager.gameObject.transform;
            NewRLItemComponent.AlignToTerrain();
            Selection.activeGameObject = NewRLItemObject;
        }
        public static void InsertRacingLineItem(RacingLineItem SelectedRacingLine)
        {
            if (SelectedRacingLine != null)
            {
                InsertRLItemtAfterSelectedRacingLineNode(SelectedRacingLine);
            }
        }



        private static void InsertRLItemtAfterSelectedRacingLineNode(RacingLineItem SelectedRacingLine)
        {
            GameObject RLManagerObject = SelectedRacingLine.gameObject.transform.parent.gameObject;

            Transform[] RLNodes = GetChildTransforms(RLManagerObject.transform);

            int CurrentRLNodeIndex = System.Array.FindIndex(RLNodes, tr => tr.name == SelectedRacingLine.name);

            for (int i = RLNodes.Length; i > CurrentRLNodeIndex + 1; i--)
            {
                string NextItemName = (i + 1).ToString();
                if (NextItemName.Length < 2)
                    NextItemName = "0" + NextItemName;

                RLNodes[i - 1].name = NextItemName;
            }


            string NewItemName = (CurrentRLNodeIndex + 2).ToString();
            if (NewItemName.Length < 2)
                NewItemName = "0" + NewItemName;


            GameObject NewItem = new GameObject(NewItemName);
            RacingLineItem NewWPItemComponent = NewItem.AddComponent<RacingLineItem>();

            NewItem.transform.parent = RLManagerObject.gameObject.transform;
            NewItem.transform.position = SelectedRacingLine.transform.position;

            if (RLNodes[CurrentRLNodeIndex + 1] != null)
            {
                NewItem.transform.LookAt(RLNodes[CurrentRLNodeIndex + 1].transform);
            }

            Selection.activeGameObject = NewItem;
        }



        private static void DeleteSelectedRacingLineNode(RacingLineItem SelectedRacingLineNode)
        {
            GameObject RacingLineManager = SelectedRacingLineNode.gameObject.transform.parent.gameObject;
            string oldRLItemname = SelectedRacingLineNode.name;

            Transform[] RacingLineNodes = GetChildTransforms(RacingLineManager.transform);

            int CurrentRLNIndex = System.Array.FindIndex(RacingLineNodes, tr => tr.name == SelectedRacingLineNode.name);


            SelectedRacingLineNode.name = "DELETED_RLN";


            for (int i = RacingLineNodes.Length; i > CurrentRLNIndex + 1; i--)
            {
                string NextItemName = (i - 1).ToString();

                if (NextItemName.Length < 2)
                    NextItemName = "0" + NextItemName;

                RacingLineNodes[i - 1].name = NextItemName;
            }
            Selection.activeGameObject = RacingLineNodes[CurrentRLNIndex - 1].gameObject;


            GameObject DeleteParent = GameObject.Find("___SAFEDELETE");
            bool showDeletedMessage = true;

            if (DeleteParent == null)
            {
                DeleteParent = new GameObject("___SAFEDELETE");
                SelectedRacingLineNode.transform.parent = DeleteParent.transform;
                GameObject.DestroyImmediate(SelectedRacingLineNode, true);
                showDeletedMessage = true;
            }
            else
            {
                SelectedRacingLineNode.transform.parent = DeleteParent.transform;
                GameObject.DestroyImmediate(SelectedRacingLineNode, true);
                showDeletedMessage = true;
            }

            if (showDeletedMessage)
            {
                EditorUtility.DisplayDialog("Item moved to safe deletion!",
                "Racin Line Node" + oldRLItemname + " moved to under ___SAFEDELETE gameobject in project hierarchy. \r\nYou can remove this item safely now!", "OK");
            }


        }


        private static int GetRacingLineNodeName(GameObject RacingLineManager)
        {
            return new List<Component>(RacingLineManager.GetComponentsInChildren(typeof(RacingLineItem))).Count + 1;
        }
        #endregion
        /// <summary>
        /// Creates Racing Line component with additionally SplineInterpolator and MeshRenderer
        /// </summary>
        //public static void CreateRacingLineManager()
        //{
        //    GameObject GManagerObject = GameObject.Find("_RacingLineManager");
        //    if (GManagerObject != null)
        //    {
        //        Debug.LogWarning(Helpers.EditorMessages.RCLObjectExists);
        //        return;
        //    }

        //    GManagerObject = new GameObject("_RacingLineManager");
        //    GManagerObject.transform.position = new Vector3(0, 0, 0);
        //    SplineInterpolator SIComponent = GManagerObject.AddComponent<SplineInterpolator>();
        //    SIComponent.name = SIComponent.name;//Debug warning removal

        //    MeshFilter mFilter = GManagerObject.AddComponent<MeshFilter>();

        //    MeshRenderer mRenderer = GManagerObject.AddComponent<MeshRenderer>();
        //    
        //    RacingLineManager GManager = GManagerObject.AddComponent<RacingLineManager>();


        //    Selection.activeGameObject = GManagerObject;
        //}

        /// <summary>
        /// Creates GameManager compoonent with additionally SplineInterpolator, GameUI and GameAudio
        /// </summary>
        public static void CreateRaceManager()
        {
            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.GMObjectExists);
                return;
            }

            GManagerObject = new GameObject("_RaceManager");
            GManagerObject.transform.position = new Vector3(0, 0, 0);
            SplineInterpolator SIComponent = GManagerObject.AddComponent<SplineInterpolator>();
            SIComponent.name = SIComponent.name;//Debug warning removal
            Race_Manager GManager = GManagerObject.AddComponent<Race_Manager>();
            GManager.name = GManager.name;//Debug warning removal
            Race_UI GManagerUI = GManagerObject.AddComponent<Race_UI>();
            GManagerUI.name = GManagerUI.name;//Debug warning removal;
            GUISkin DefaultSkin = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Skins/RSK_Default.guiskin", typeof(GUISkin)) as GUISkin;
            GManagerUI.UISkin = DefaultSkin;
            Race_Audio GAudio = GManagerObject.AddComponent<Race_Audio>();
            GAudio.name = GAudio.name;//Debug warning removal;
            Selection.activeGameObject = GManagerObject;
        }


        /// <summary>
        /// Creates new SpawnPoint Container Object
        /// </summary>
        public static void CreateSpawnPointContainer()
        {
            GameObject SPContainer = GameObject.Find("_SpawnPoints");
            if (SPContainer != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.SPObjectExists);
                return;
            }
            SPContainer = new GameObject("_SpawnPoints");
            SPContainer.transform.position = new Vector3(0, 0, 0);
            SpawnPointManager SPManager = SPContainer.AddComponent<SpawnPointManager>();
            SPManager.name = SPManager.name;//Debug warning removal
            Selection.activeGameObject = SPContainer;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.SpawnPoints = SPContainer;
            }
        }

        /// <summary>
        /// Creates new SpawnPoint Item
        /// </summary>
        public static void CreateSpawnPointItem()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent(typeof(SpawnPointManager)) != null)
                {
                    CreateSpawnPointItemFromManager(Selection.activeGameObject.GetComponent(typeof(SpawnPointManager)) as SpawnPointManager);
                }
                else
                    if (Selection.activeGameObject.GetComponent(typeof(SpawnPointItem)) != null)
                    {
                        CreateSpawnPointItemFromItem(Selection.activeGameObject.GetComponent(typeof(SpawnPointItem)) as SpawnPointItem);
                    }
            }
        }

        /// <summary>
        /// Creates New CheckPoint Container
        /// </summary>
        public static void CreateCheckPointContainer()
        {
            GameObject CPContainer = GameObject.Find("_CheckPoints");
            if (CPContainer != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.SPObjectExists);
                return;
            }
            CPContainer = new GameObject("_CheckPoints");
            CPContainer.transform.position = new Vector3(0, 0, 0);
            CheckPointManager CPManager = CPContainer.AddComponent<CheckPointManager>();
            CPManager.name = CPManager.name;//Debug warning removal
            Selection.activeGameObject = CPContainer;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.CheckPoints = CPContainer;
            }
        }

        /// <summary>
        /// Creates New Checkpoint Item
        /// </summary>
        public static void CreateCheckPointItem()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent(typeof(CheckPointManager)) != null)
                {
                    CreateCheckPointItemFromManager(Selection.activeGameObject.GetComponent(typeof(CheckPointManager)) as CheckPointManager);
                }
                else
                    if (Selection.activeGameObject.GetComponent(typeof(CheckPointItem)) != null)
                    {
                        CreateCheckPointItemFromItem(Selection.activeGameObject.GetComponent(typeof(CheckPointItem)) as CheckPointItem);
                    }
            }
        }

        /// <summary>
        /// Creates New StartPoint
        /// </summary>
        public static void CreateStartPoint()
        {
            GameObject SPoint = GameObject.Find("_StartPoint");
            if (SPoint != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.SPointExists);
                return;
            }
            GameObject WPObject = GameObject.Find("_WayPoints");
            if (WPObject == null || WPObject.transform.childCount == 0)
            {
                bool blnAnswer = EditorUtility.DisplayDialog("Hold on!", Helpers.EditorMessages.WPObjectNotCreatedForStart, "Cancel", "Yes I'am!");
                if (!blnAnswer)
                    CreateStartPointOnScreenPoint();
            }
            else
            {
                CreateStartPointOnFirstWayPoint(WPObject);
            }
        }

        /// <summary>
        /// Creates New FinishPoint
        /// </summary>
        public static void CreateFinishPoint()
        {
            GameObject SPoint = GameObject.Find("_FinishPoint");
            if (SPoint != null)
            {
                Debug.LogWarning(Helpers.EditorMessages.FPointExists);
                return;
            }
            GameObject WPObject = GameObject.Find("_WayPoints");
            if (WPObject == null || WPObject.transform.childCount == 0)
            {
                bool blnAnswer = EditorUtility.DisplayDialog("Hold on!", Helpers.EditorMessages.WPObjectNotCreatedForFinish, "Cancel", "Yes I'am!");
                if (!blnAnswer)
                    CreateFinishPointOnScreenPoint();
            }
            else
            {
                CreateFinishPointOnLastWayPoint(WPObject);
            }
        }




        public static void AlignToSurface()
        {
            foreach (Object IBase in Selection.GetFiltered(typeof(ItemBase), SelectionMode.Deep))
            {
                ((ItemBase)IBase).AlignToTerrain();
            }
        }

        public static void AddSoundManager()
        {
            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject == null)
            {
                Debug.LogWarning(Helpers.EditorMessages.GMObjectNotCreated);
                return;
            }
            Race_Audio GAudio = GManagerObject.AddComponent<Race_Audio>();
            GAudio.name = GAudio.name;//Debug warning removal;

        }

        public static void AddVerticalProgress()
        {
            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject == null)
            {
                Debug.LogWarning(Helpers.EditorMessages.GMObjectNotCreated);
                return;
            }

            Race_Progress GProgressHorizontal = GManagerObject.AddComponent<Race_Progress>();
            GProgressHorizontal.name = GProgressHorizontal.name;//Debug warning removal;
            GProgressHorizontal.ControlRotation = Race_Progress.eProgressRotation.Vertical;
            GProgressHorizontal.ControlPlacement = Race_Progress.eProgressPlacement.LeftBottom;
            GProgressHorizontal.ControlWidth = 50f;
            GProgressHorizontal.ControlHeight = 500f;
            GProgressHorizontal.ControlTopOffset = 10;
            GProgressHorizontal.ControlBottomOffset = 15f;
            GProgressHorizontal.ControlLeftOffset = 30f;
            GProgressHorizontal.ControlRightOffset = 30f;
            GProgressHorizontal.ProgressBarSize = 472f;
            //PROGRESSBG
            Texture2D imgProgressBG = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/progress_bg.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.ProgressBarBackground = imgProgressBG;
            GProgressHorizontal.ProgressOffset = new Vector2(0f, 0f);
            //COMPLETEBACKGROUND HERE
            Texture2D imgCompletedBG = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/progress_complete.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.PlayerCompleteForeground = imgCompletedBG;
            GProgressHorizontal.PlayerCompleteOffset = new Vector2(0f, 0f);
            //STARTPOINT IMAGE
            Texture2D imgStartIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/progress_start_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.StartPointIcon = imgStartIcon;
            GProgressHorizontal.StartPointOffset = new Vector2(4.650143f, 0f);
            //ENDPOINT IMAGE
            Texture2D imgFinishtIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/progress_finish_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.EndPointIcon = imgFinishtIcon;
            GProgressHorizontal.EndPointOffset = new Vector2(7.138184f, 3.063957f);
            //SEPERATOR IMAGE
            Texture2D imgSeperator = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/progress_seperator_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.LapSeperatorIcon = imgSeperator;
            GProgressHorizontal.LapSeperatorOffset = new Vector2(10f, 0f);
            //RACERICON
            Texture2D imgPlayerIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/arrow_player_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.RacerIcon = imgPlayerIcon;
            //RIVALICON
            Texture2D imgRivalIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/Arrow_rival_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.RivalIcon = imgRivalIcon;
            //RIVALDESTROYED
            Texture2D imgDestroyedIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_VerticalIcons/arrow_destroyed_vert.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.DestroyedIcon = imgDestroyedIcon;
            GProgressHorizontal.PlayerIconsOffset = new Vector2(31.61736f, 0f);
        }

        public static void AddHorizontalProgress()
        {
            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject == null)
            {
                Debug.LogWarning(Helpers.EditorMessages.GMObjectNotCreated);
                return;
            }

            Race_Progress GProgressHorizontal = GManagerObject.AddComponent<Race_Progress>();
            GProgressHorizontal.name = GProgressHorizontal.name;//Debug warning removal;
            GProgressHorizontal.ControlRotation = Race_Progress.eProgressRotation.Horizontal;
            GProgressHorizontal.ControlPlacement = Race_Progress.eProgressPlacement.BottomCenter;
            GProgressHorizontal.ControlWidth = 650f;
            GProgressHorizontal.ControlHeight = 50f;
            GProgressHorizontal.ControlTopOffset = 0;
            GProgressHorizontal.ControlBottomOffset = 55f;
            GProgressHorizontal.ControlLeftOffset = 0f;
            GProgressHorizontal.ControlRightOffset = 0f;
            GProgressHorizontal.ProgressBarSize = 625f;
            //PROGRESSBG
            Texture2D imgProgressBG = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/progress_bg.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.ProgressBarBackground = imgProgressBG;
            GProgressHorizontal.ProgressOffset = new Vector2(5.6f, 35.4f);
            //COMPLETEBACKGROUND HERE
            Texture2D imgCompletedBG = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/progress_complete.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.PlayerCompleteForeground = imgCompletedBG;
            GProgressHorizontal.PlayerCompleteOffset = new Vector2(5.6f, 35.4f);
            //STARTPOINT IMAGE
            Texture2D imgStartIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/progress_start.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.StartPointIcon = imgStartIcon;
            GProgressHorizontal.StartPointOffset = new Vector2(0f, 27.6f);
            //ENDPOINT IMAGE
            Texture2D imgFinishtIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/progress_finish.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.EndPointIcon = imgFinishtIcon;
            GProgressHorizontal.EndPointOffset = new Vector2(0f, 27.6f);
            //SEPERATOR IMAGE
            Texture2D imgSeperator = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/progress_seperator.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.LapSeperatorIcon = imgSeperator;
            GProgressHorizontal.LapSeperatorOffset = new Vector2(0f, 31.3f);
            //RACERICON
            Texture2D imgPlayerIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/arrow_player.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.RacerIcon = imgPlayerIcon;
            //RIVALICON
            Texture2D imgRivalIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/Arrow_rival.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.RivalIcon = imgRivalIcon;
            //RIVALDESTROYED
            Texture2D imgDestroyedIcon = AssetDatabase.LoadAssetAtPath("Assets/_Racing Game Kit/UI/Progress/_HorizontalIcons/arrow_destroyed.png", typeof(Texture2D)) as Texture2D;
            GProgressHorizontal.DestroyedIcon = imgDestroyedIcon;
            GProgressHorizontal.PlayerIconsOffset = new Vector2(0f, 9.7f);
        }



        #region Private Methods
        private static void CreateFinishPointOnScreenPoint()
        {
            GameObject FinishPointObject = new GameObject("_FinishPoint");
            BoxCollider NewItemCollider = FinishPointObject.AddComponent<BoxCollider>();
            NewItemCollider.size = new Vector3(1, 1, 1);
            NewItemCollider.isTrigger = true;
            FinishPointItem NewItemBase = FinishPointObject.AddComponent<FinishPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewItemBase.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewItemBase.AlignToTerrain();
            Selection.activeGameObject = FinishPointObject;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.FinishPoint = FinishPointObject;
            }
        }

        private static void CreateFinishPointOnLastWayPoint(GameObject WPManager)
        {
            Transform[] Childs = GetChildTransforms(WPManager.transform);
            GameObject LastWPItem = Childs[Childs.Length - 1].gameObject;
            GameObject FinishPointObject = new GameObject("_FinishPoint");
            BoxCollider NewItemCollider = FinishPointObject.AddComponent<BoxCollider>();
            NewItemCollider.size = new Vector3(1, 1, 1);
            NewItemCollider.isTrigger = true;
            FinishPointItem NewItemBase = FinishPointObject.AddComponent<FinishPointItem>();
            NewItemBase.name = NewItemBase.name; //Debug warning removal;
            FinishPointObject.transform.position = LastWPItem.transform.position;
            Selection.activeGameObject = FinishPointObject;


            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.FinishPoint = FinishPointObject;
            }
        }

        private static void CreateStartPointOnScreenPoint()
        {
            GameObject StartPointObject = new GameObject("_StartPoint");
            BoxCollider NewItemCollider = StartPointObject.AddComponent<BoxCollider>();
            NewItemCollider.size = new Vector3(1, 1, 1);
            NewItemCollider.isTrigger = true;
            StartPointItem NewItemBase = StartPointObject.AddComponent<StartPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                //NewItemBase.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            //NewItemBase.AlignToTerrain();
            Selection.activeGameObject = StartPointObject;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.StartPoint = StartPointObject;
            }
        }

        private static void CreateStartPointOnFirstWayPoint(GameObject WPManager)
        {
            GameObject FirstWPItem = GetChildTransforms(WPManager.transform)[0].gameObject;

            GameObject StartPointObject = new GameObject("_StartPoint");
            BoxCollider NewItemCollider = StartPointObject.AddComponent<BoxCollider>();
            NewItemCollider.size = new Vector3(1, 1, 1);
            NewItemCollider.isTrigger = true;
            StartPointItem NewItemBase = StartPointObject.AddComponent<StartPointItem>();
            NewItemBase.name = NewItemBase.name; //Debug warning removal;
            StartPointObject.transform.position = FirstWPItem.transform.position;

            GameObject GManagerObject = GameObject.Find("_RaceManager");
            if (GManagerObject != null)
            {
                Race_Manager GManager = GManagerObject.GetComponent(typeof(Race_Manager)) as Race_Manager;
                GManager.StartPoint = StartPointObject;
            }

            Selection.activeGameObject = StartPointObject;
        }

        private static void CreateSpawnPointItemFromManager(SpawnPointManager SPManager)
        {
            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetSpawnPointName(SPManager.gameObject).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (SPManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                    return;
                }
            }

            GameObject NewItem = new GameObject(NewItemName);
            SpawnPointItem gizmo2 = NewItem.AddComponent<SpawnPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                gizmo2.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewItem.transform.parent = SPManager.gameObject.transform;
            Selection.activeGameObject = NewItem;
        }

        private static void CreateSpawnPointItemFromItem(SpawnPointItem SPItem)
        {
            GameObject SPManager = SPItem.gameObject.transform.parent.gameObject;

            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetSpawnPointName(SPManager).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (SPManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                    return;
                }
            }

            GameObject NewItem = new GameObject(NewItemName);
            SpawnPointItem gizmo2 = NewItem.AddComponent<SpawnPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                gizmo2.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewItem.transform.parent = SPManager.gameObject.transform;
            Selection.activeGameObject = NewItem;
        }

        private static int GetSpawnPointName(GameObject SpawnManager)
        {
            return new List<Component>(SpawnManager.GetComponentsInChildren(typeof(SpawnPointItem))).Count + 1;
        }

        private static void CreateCheckPointItemFromManager(CheckPointManager CPManager)
        {
            string NewItemName = null;
            while (NewItemName == null)
            {
                NewItemName = GetCheckPointName(CPManager.gameObject).ToString();
                if (NewItemName.Length < 2)
                    NewItemName = "0" + NewItemName;

                if (CPManager.gameObject.transform.Find(NewItemName) != null)
                {
                    NewItemName = null;
                    return;
                }
            }
            GameObject NewCPItemObject = new GameObject(NewItemName);
            NewCPItemObject.tag = "CheckPoint";
            BoxCollider NewCPCollider = NewCPItemObject.AddComponent<BoxCollider>();
            NewCPCollider.isTrigger = true;
            NewCPCollider.size = new Vector3(3, 3, 1);

            CheckPointItem NewCPItemComponet = NewCPItemObject.AddComponent<CheckPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewCPItemComponet.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewCPItemObject.transform.parent = CPManager.gameObject.transform;
            Selection.activeGameObject = NewCPItemObject;
        }

        private static void CreateCheckPointItemFromItem(CheckPointItem CPItem)
        {
            GameObject CPManager = CPItem.gameObject.transform.parent.gameObject;

            string name = null;
            while (name == null)
            {
                name = GetCheckPointName(CPManager.gameObject).ToString();
                if (name.Length < 2)
                    name = "0" + name;

                if (CPManager.gameObject.transform.Find(name) != null)
                {
                    name = null;
                    return;
                }
            }
            GameObject NewCPItemObject = new GameObject(name);
            NewCPItemObject.tag = "CheckPoint";
            BoxCollider NewCPCollider = NewCPItemObject.AddComponent<BoxCollider>();
            NewCPCollider.isTrigger = true;
            NewCPCollider.size = new Vector3(3, 3, 1);

            CheckPointItem NewCPItemComponet = NewCPItemObject.AddComponent<CheckPointItem>();
            Camera camera = SceneView.lastActiveSceneView.camera;
            if (camera != null)
            {
                NewCPItemComponet.CastToCollider(camera.transform.position, camera.transform.forward, 5f, 20f);
            }
            NewCPItemObject.transform.parent = CPManager.gameObject.transform;
            Selection.activeGameObject = NewCPItemObject;
        }

        private static int GetCheckPointName(GameObject CheckPointManager)
        {
            return new List<Component>(CheckPointManager.GetComponentsInChildren(typeof(CheckPointItem))).Count + 1;
        }


        private static Transform[] GetChildTransforms(Transform RootTransform)
        {
            if (RootTransform == null)
                return new Transform[] { };

            List<Component> components = new List<Component>(RootTransform.GetComponentsInChildren(typeof(Transform)));
            List<Transform> transforms = components.ConvertAll(c => (Transform)c);

            transforms.Remove(RootTransform.transform);
            transforms.Sort(delegate(Transform a, Transform b)
            {
                return System.Convert.ToInt32(a.name).CompareTo(System.Convert.ToInt32(b.name));
            });

            return transforms.ToArray();
        }
        #endregion

        #region InspectorFunctions

        private static bool IsStartPointAssigned = false;
        private static bool IsFinishPointAssigned = false;
        private static bool IsWayPointContainerAssigned = false;
        private static bool IsSpawnPointContainerAssigned = false;
        private static bool IsCheckPointContainerAssigned = false;


        private static bool IsStartPointCreated = false;
        private static bool IsFinishPointCreated = false;
        private static bool IsWayPointContainerCreated = false;
        private static bool IsSpawnPointContainerCreated = false;
        private static bool IsCheckPointContainerCreated = false;



        internal static string CreateRGKRaceManagerInspector(Race_Manager GManager, SerializedObject Sobject)
        {

            bool showWarning = CheckRequirements(GManager);

            int iAlignmentSpace = (Screen.width - 390) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);
            GUILayout.Space((Screen.width - 300) / 2);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



           
                //buttons start
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(iAlignmentSpace - 10);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (IsWayPointContainerAssigned) GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Create\r\nWaypoint\r\nContainer", "Create new WayPoint Container"), GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(85), GUILayout.Height(50)))
                {
                    CoreFunctions.CreateWayPointContainer();
                }
                GUI.enabled = true;

                if (IsSpawnPointContainerAssigned) GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Create\r\nSpawnpoint\r\nContainer", "Create new SpawnPoint Container"), GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(90), GUILayout.Height(50)))
                {
                    CoreFunctions.CreateSpawnPointContainer();
                }
                GUI.enabled = true;
                if (IsCheckPointContainerAssigned) GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Create\r\nCheckpoint\r\nContainer", "Create new Checkpoint Container"), GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(90), GUILayout.Height(50)))
                {
                    CoreFunctions.CreateCheckPointContainer();
                }

                GUI.enabled = true;
                if (IsStartPointAssigned) GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Create\r\nStart\r\nPoint", "Create new StartPoint"), GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(65), GUILayout.Height(50)))
                {
                    CoreFunctions.CreateStartPoint();
                }
                GUI.enabled = true;
                if (IsFinishPointAssigned) GUI.enabled = false;
                if (GUILayout.Button(new GUIContent("Create\r\nFinish\r\nPoint", "Create new Finish Container"), GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(65), GUILayout.Height(50)))
                {
                    CoreFunctions.CreateFinishPoint();
                }
                GUI.enabled = true;
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical();
                GUILayout.Space(10);
                GUILayout.EndVertical();
                //buttons end
                CreateVersionHeader("Race Manager");

                if (showWarning)
                {
                    GUILayout.BeginVertical("Box");
                    Color def = GUI.color;
                    GUI.color = Color.red;
                    EditorGUILayout.LabelField("Warning! ", EditorStyles.boldLabel);
                    GUI.color = def;
                    if (!IsWayPointContainerCreated)
                    { GUILayout.Label("WayPoint Container object not found!"); }
                    else if (!IsWayPointContainerAssigned)
                    { GUILayout.Label("WayPoint Container created but not assigned!"); }
                    else if (GManager.Waypoints.transform.childCount == 0)
                    { GUILayout.Label("WayPoint Container created and assigned but waypoints not created!", Helpers.GUIHelper.WrappableLabel); }

                    if (!IsSpawnPointContainerCreated)
                    { GUILayout.Label("SpawnPoint Container object not found!"); }
                    else if (!IsSpawnPointContainerAssigned)
                    { GUILayout.Label("SpawnPoint Container created but not assigned!"); }
                    else if (GManager.SpawnPoints.transform.childCount == 0)
                    { GUILayout.Label("SpawnPoint Container created and assigned but spawnpoints not created!", Helpers.GUIHelper.WrappableLabel); }

                    if (!IsCheckPointContainerCreated)
                    { GUILayout.Label("CheckPoint Container object not found!"); }
                    else if (!IsCheckPointContainerAssigned)
                    { GUILayout.Label("CheckPoint Container crated but not assigned"); }
                    else if (GManager.CheckPoints.transform.childCount == 0)
                    { GUILayout.Label("CheckPoint Container created and assigned but checkpoints not created!", Helpers.GUIHelper.WrappableLabel); }

                    if (!IsFinishPointAssigned)
                    { GUILayout.Label("FinishPoint object created but not assigned!"); }
                    else if (GManager.FinishPoint != GManager.StartPoint && (GManager.FinishPoint.GetComponent(typeof(FinishPointItem)) == null))
                    { GUILayout.Label("Assigned Finishpoint object doesn't have \"Finish Point Item\" component!"); }
                    else if (!IsFinishPointCreated && !IsFinishPointAssigned)
                    { GUILayout.Label("FinishPoint object not found!"); }

                    if (!IsStartPointCreated)
                    { GUILayout.Label("StartPoint object not found!"); }
                    else if (!IsStartPointAssigned)
                    { GUILayout.Label("StartPoint object created but not assigned!"); }

                    GUILayout.EndVertical();
                    EditorGUILayout.Space();
                }

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Containers", EditorStyles.boldLabel);
                GManager.Waypoints = (GameObject)EditorGUILayout.ObjectField("Waypoint Container", GManager.Waypoints, typeof(GameObject), true);
                GManager.SpawnPoints = (GameObject)EditorGUILayout.ObjectField("Spawn Point Container", GManager.SpawnPoints, typeof(GameObject), true);
                GManager.CheckPoints = (GameObject)EditorGUILayout.ObjectField("Checkpoint Container", GManager.CheckPoints, typeof(GameObject), true);
                GManager.StartPoint = (GameObject)EditorGUILayout.ObjectField("Start Point", GManager.StartPoint, typeof(GameObject), true);
                GManager.FinishPoint = (GameObject)EditorGUILayout.ObjectField("Finish Point", GManager.FinishPoint, typeof(GameObject), true);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Race Settings", EditorStyles.boldLabel);

                GManager.RaceType = (RaceTypeEnum)EditorGUILayout.EnumPopup("Race Type", GManager.RaceType);
                if (GManager.RaceType == RaceTypeEnum.Sprint || GManager.RaceType == RaceTypeEnum.TimeAttack)
                {

                    GManager.RaceLaps = 1;
                }
                else
                {

                    GManager.RaceLaps = EditorGUILayout.IntField("Laps", GManager.RaceLaps);
                }


                if (GManager.RaceType == RaceTypeEnum.LapKnockout)
                {

                    GManager.KickLastRacerAfter2ndLast = EditorGUILayout.Toggle("Kick Last Racer After 2nd Last Passed ", GManager.KickLastRacerAfter2ndLast);
                    GManager.MoveKickedRacerToIgnoreLayer = EditorGUILayout.Toggle("Move Kicked Racer to IGNORE Layer", GManager.MoveKickedRacerToIgnoreLayer);
                }
                else
                {
                    GManager.KickLastRacerAfter2ndLast = false;

                }
                if (GManager.RaceType == RaceTypeEnum.Speedtrap)
                {
                    GManager.SpeedTrapMode = (eSpeedTrapMode)EditorGUILayout.EnumPopup("Speed Trap Mode", GManager.SpeedTrapMode);
                }

                GManager.RacePlayers = EditorGUILayout.IntField("Total Players", GManager.RacePlayers);
                GManager.TimerCountdownFrom = EditorGUILayout.IntField("Start Countdown From", GManager.TimerCountdownFrom);
                GManager.RaceStartsOnStartup = EditorGUILayout.Toggle("Race Starts on Level Load", GManager.RaceStartsOnStartup);
                GManager.StopRaceOnPlayerFinish = EditorGUILayout.Toggle("Stop Race After Player Finish", GManager.StopRaceOnPlayerFinish);
                GManager.AiContinuesAfterFinish = EditorGUILayout.Toggle("AI Cars Continues After Finish", GManager.AiContinuesAfterFinish);
                GManager.PlayerContinuesAfterFinish = EditorGUILayout.Toggle("Player Car Continues After Finish", GManager.PlayerContinuesAfterFinish);
                GManager.EnableCheckpointArrow = EditorGUILayout.Toggle("Show Checkpoint Arrow", GManager.EnableCheckpointArrow);
                GManager.MoveRespawnToIgnoreLayer = EditorGUILayout.Toggle("Move Respawned Racer to IGNORE Layer", GManager.MoveRespawnToIgnoreLayer);
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Player Settings", EditorStyles.boldLabel);
                GManager.HumanRacerPrefab = (GameObject)EditorGUILayout.ObjectField("Player Prefab", GManager.HumanRacerPrefab, typeof(GameObject), false);
                GManager.PlayerSpawnPosition = EditorGUILayout.IntField("Player Spawn Position", GManager.PlayerSpawnPosition);

                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("AI Settings", EditorStyles.boldLabel);
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Add AI To Race", GUILayout.Width(105f)))
                {
                    GameObject[] _temp = null;
                    if (GManager.AIRacerPrefab != null)
                    {
                        _temp = new GameObject[GManager.AIRacerPrefab.Length + 1];
                    }
                    else
                    {
                        _temp = new GameObject[1];
                    }

                    if (GManager.AIRacerPrefab != null)
                    {
                        for (int i = 0; i < GManager.AIRacerPrefab.Length; i++)
                        {
                            _temp[i] = GManager.AIRacerPrefab[i];
                        }
                    }
                    GManager.AIRacerPrefab = _temp;
                }

                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //ArrayField(Sobject.FindProperty("AIRacerPrefab"));

                if (GManager.AIRacerPrefab != null)
                {
                    for (int i = 0; i < GManager.AIRacerPrefab.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        //indicate each array slot with index number in front of it
                        GUILayout.Label((i + 1) + ".", GUILayout.Width(20));
                        //create an object field for every waypoint
                        GManager.AIRacerPrefab[i] = EditorGUILayout.ObjectField(GManager.AIRacerPrefab[i], typeof(GameObject), true) as GameObject;


                        GUI.backgroundColor = Color.red;
                        if (i == 0)
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button("▲", GUILayout.Width(25f)))
                        {
                            GManager.AIRacerPrefab = MoveItem(GManager.AIRacerPrefab, i, true);
                        }
                        GUI.enabled = true;

                        if (i == (GManager.AIRacerPrefab.Length - 1) || GManager.AIRacerPrefab.Length == 1)
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button("▼", GUILayout.Width(25f)))
                        {
                            GManager.AIRacerPrefab = MoveItem(GManager.AIRacerPrefab, i, false);
                        }
                        GUI.enabled = true;

                        if (GUILayout.Button("-", GUILayout.Width(25f)))
                        {
                            GManager.AIRacerPrefab = RemoveItemFromArray(GManager.AIRacerPrefab, i);
                        }
                        GUI.backgroundColor = defaultColor;
                        GUILayout.EndHorizontal();
                    }
                }


                EditorGUILayout.Space();

                GManager.AiSpawnOrder = (eAISpawnOrder)EditorGUILayout.EnumPopup("AI Spawn Order", GManager.AiSpawnOrder);
                GManager.AiSpawnMode = (eAISpawnMode)EditorGUILayout.EnumPopup("AI Spawn Mode", GManager.AiSpawnMode);
                GManager.AiNamingMode = (eAINamingMode)EditorGUILayout.EnumPopup("AI Naming Mode", GManager.AiNamingMode);

                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Other Settings", EditorStyles.boldLabel);
                GManager.EnableStartupCamera = EditorGUILayout.Toggle("Enable Startup Camera", GManager.EnableStartupCamera);
                GManager.StartMusicAfterCountdown = EditorGUILayout.Toggle("Start Music After Countdown", GManager.StartMusicAfterCountdown);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Race Information", EditorStyles.boldLabel);
                GUI.enabled = false;
                GManager.IsRaceReady = EditorGUILayout.Toggle("Race Ready to Start?", GManager.IsRaceReady);
                GManager.IsRaceStarted = EditorGUILayout.Toggle("Race Started?", GManager.IsRaceStarted);
                GManager.IsRaceFinished = EditorGUILayout.Toggle("Race Finished?", GManager.IsRaceFinished);
                GUI.enabled = true;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Race Length (Km)", GManager.RaceLength.ToString());
                EditorGUILayout.LabelField("Total Race Time", FormatTime(GManager.TimeTotal, true, 2));
                EditorGUILayout.LabelField("Current Lap Time", FormatTime(GManager.TimeCurrent, true, 2));
                EditorGUILayout.LabelField("Player Last Time", FormatTime(GManager.TimePlayerLast, true, 2));
                EditorGUILayout.LabelField("Player Best Time", FormatTime(GManager.TimePlayerBest, true, 2));
                EditorGUILayout.LabelField("Next Checkpoint Time", FormatTime(GManager.TimeNextCheckPoint, true, 2));
                EditorGUILayout.LabelField("Total Checkpoint Time", FormatTime(GManager.TimeTotalCheckPoint, true, 2));

                GUILayout.EndVertical();
                EditorGUILayout.Space();


                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                GManager.ShowDistanceGizmos = EditorGUILayout.Toggle("Show Distance Gizmos", GManager.ShowDistanceGizmos);
                GManager.DistancePointDensity = EditorGUILayout.FloatField("Distance Point Density", GManager.DistancePointDensity);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Working FPS", GManager.WorkingFPS.ToString());
				GUILayout.EndVertical();
                EditorGUILayout.Space();


			GUILayout.BeginVertical("Box");

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Player Cars", EditorStyles.boldLabel);
			GUI.backgroundColor = Color.red;

			if (GUILayout.Button("Add Cars To Race", GUILayout.Width(105f)))
			{
				GameObject[] _temp = null;
				if (GManager.playerBuggies != null)
				{
					_temp = new GameObject[GManager.playerBuggies.Length + 1];
				}
				else
				{
					_temp = new GameObject[1];
				}

				if (GManager.playerBuggies != null)
				{
					for (int i = 0; i < GManager.playerBuggies.Length; i++)
					{
						_temp[i] = GManager.playerBuggies[i];
					}
				}
				GManager.playerBuggies = _temp;
			}

			GUI.backgroundColor = defaultColor;
			GUILayout.EndHorizontal();
			//ArrayField(Sobject.FindProperty("AIRacerPrefab"));

			if (GManager.playerBuggies != null)
			{
				for (int i = 0; i < GManager.playerBuggies.Length; i++)
				{
					GUILayout.BeginHorizontal();
					//indicate each array slot with index number in front of it
					GUILayout.Label((i + 1) + ".", GUILayout.Width(20));
					//create an object field for every waypoint
					GManager.playerBuggies[i] = EditorGUILayout.ObjectField(GManager.playerBuggies[i], typeof(GameObject), true) as GameObject;


					GUI.backgroundColor = Color.red;
					if (i == 0)
					{
						GUI.enabled = false;
					}
					if (GUILayout.Button("▲", GUILayout.Width(25f)))
					{
						GManager.playerBuggies = MoveItem(GManager.playerBuggies, i, true);
					}
					GUI.enabled = true;

					if (i == (GManager.playerBuggies.Length - 1) || GManager.playerBuggies.Length == 1)
					{
						GUI.enabled = false;
					}
					if (GUILayout.Button("▼", GUILayout.Width(25f)))
					{
						GManager.playerBuggies = MoveItem(GManager.playerBuggies, i, false);
					}
					GUI.enabled = true;

					if (GUILayout.Button("-", GUILayout.Width(25f)))
					{
						GManager.playerBuggies = RemoveItemFromArray(GManager.playerBuggies, i);
					}
					GUI.backgroundColor = defaultColor;
					GUILayout.EndHorizontal();
				}
			}


			GUILayout.EndVertical();
			EditorGUILayout.Space();

                if (GUI.changed)
                {
                    //    RLManager.ShowHideChildIcons(RLManager.ShowHelperIcons);
                }
                return "";
           


        }

        private static GameObject[] RemoveItemFromArray(GameObject[] ActualArray, int ItemIndex)
        {
            GameObject[] _temp = new GameObject[ActualArray.Length - 1];
            int iSafe = 0;
            for (int i = 0; i < ActualArray.Length; i++)
            {
                if (i != ItemIndex)
                {
                    _temp[iSafe] = ActualArray[i];
                    iSafe++;
                }
            }
            return _temp;
        }

        private static GameObject[] MoveItem(GameObject[] ActualArray, int ItemIndex, bool IsUp)
        {
            GameObject[] _temp = ActualArray;
            GameObject TargetObject = ActualArray[ItemIndex];
            GameObject ReplaceObject;
            if (IsUp)
            {
                ReplaceObject = ActualArray[ItemIndex - 1];
                _temp[ItemIndex - 1] = TargetObject;
                _temp[ItemIndex] = ReplaceObject;

            }
            else
            {
                ReplaceObject = ActualArray[ItemIndex + 1];
                _temp[ItemIndex + 1] = TargetObject;
                _temp[ItemIndex] = ReplaceObject;
            }

            return _temp;
        }

        public static void ArrayField(SerializedProperty property)
        {
            EditorGUIUtility.LookLikeInspector();
            bool wasEnabled = GUI.enabled;
            int prevIdentLevel = EditorGUI.indentLevel;

            // Iterate over all child properties of array
            bool childrenAreExpanded = true;
            int propertyStartingDepth = property.depth;

            while (property.NextVisible(childrenAreExpanded) && propertyStartingDepth < property.depth)
            {
                childrenAreExpanded = EditorGUILayout.PropertyField(property);
            }

            EditorGUI.indentLevel = prevIdentLevel;
            GUI.enabled = wasEnabled;
        }

        static string FormatTime(float TimeValue, bool ShowFraction, float FractionDecimals)
        {
            System.String strReturn = "00:00:00";

            if (TimeValue > 0)
            {
                System.TimeSpan tTime = System.TimeSpan.FromSeconds(TimeValue);

                float minutes = tTime.Minutes;
                float seconds = tTime.Seconds;

                float fractions = (TimeValue * 100) % 100;

                if (ShowFraction)
                {
                    if (FractionDecimals == 2)
                    {
                        strReturn = System.String.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fractions);
                    }
                    else
                    { strReturn = System.String.Format("{0:00}:{1:00}:{2:0}", minutes, seconds, fractions); }
                }
                else
                {
                    strReturn = System.String.Format("{0:00}:{1:00}", minutes, seconds);
                }
            }
            return strReturn;
        }

        internal static bool CheckRequirements(Race_Manager target)
        {
            Race_Manager GManager = target;

            IsStartPointAssigned = (GManager.StartPoint != null) ? true : false;
            IsFinishPointAssigned = (GManager.FinishPoint != null) ? true : false;
            IsWayPointContainerAssigned = (GManager.Waypoints != null) ? true : false;
            IsCheckPointContainerAssigned = (GManager.CheckPoints != null) ? true : false;
            IsSpawnPointContainerAssigned = (GManager.SpawnPoints != null) ? true : false;


            IsWayPointContainerCreated = (GameObject.Find("_WayPoints") != null) ? true : false;
            IsCheckPointContainerCreated = (GameObject.Find("_CheckPoints") != null) ? true : false;
            IsSpawnPointContainerCreated = (GameObject.Find("_SpawnPoints") != null) ? true : false;
            IsStartPointCreated = (GameObject.Find("_StartPoint") != null) ? true : false;
            IsFinishPointCreated = (GameObject.Find("_FinishPoint") != null) ? true : false;

            if (!IsStartPointAssigned || !IsFinishPointAssigned || !IsWayPointContainerAssigned || !IsCheckPointContainerAssigned ||
                !IsSpawnPointContainerAssigned || !IsWayPointContainerCreated || !IsCheckPointContainerCreated || !IsSpawnPointContainerCreated
                || !IsStartPointCreated || GManager.Waypoints.transform.childCount == 0 || GManager.SpawnPoints.transform.childCount == 0
                 || GManager.CheckPoints.transform.childCount == 0)
            {
                return true;
            }
            else
            { return false; }
        }

        internal static string CreateRGKWayPointManagerInspector(WayPointManager WPManager)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end


                iAlignmentSpace = (Screen.width - 320) / 2;
                //buttons start
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New Waypoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(105), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateWayPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                if (GUILayout.Button("Fix Rotations", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(95), GUILayout.Height(30)))
                {
                    CoreFunctions.FixWayPointRotations(WPManager);
                }




                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Waypoint Manager");

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Helper Options", EditorStyles.boldLabel);
                WPManager.ShowHelperIcons = EditorGUILayout.Toggle("Show Helper Icons", WPManager.ShowHelperIcons, options);
                WPManager.ShowHelperSpline = EditorGUILayout.Toggle("Show Helper Spline", WPManager.ShowHelperSpline, options);
                WPManager.HelperSplineColor = EditorGUILayout.ColorField("Helper Spline Color", WPManager.HelperSplineColor);
                WPManager.CloseSpline = EditorGUILayout.Toggle("Closed Gizmo", WPManager.CloseSpline, options);

                if (GUI.changed)
                {
                    WPManager.ShowHideChildIcons(WPManager.ShowHelperIcons);
                }
                GUILayout.EndVertical();

                return "";
           

        }

        internal static string CreateRGKRacingLineManagerInspector(RacingLineManager RLManager)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);

            GUILayoutOption[] options = new GUILayoutOption[] { };
            //Logo
            GUILayout.BeginHorizontal(options);
            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end




             
                iAlignmentSpace = (Screen.width - 340) / 2;
                //buttons start
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Create RL Mesh", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(125), GUILayout.Height(30)))
                {
                    RLManager.CreateRacingLine();
                }
                if (GUILayout.Button("New RL Node", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(105), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateRacingLineItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }

                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Racing Line Manager");


                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Helper Settings", EditorStyles.boldLabel);
                RLManager.ShowHelperIcons = EditorGUILayout.Toggle("Show Helper Icons", RLManager.ShowHelperIcons, options);
                RLManager.ShowHelperSpline = EditorGUILayout.Toggle("Show Helper Spline", RLManager.ShowHelperSpline, options);
                RLManager.HelperSplineColor = EditorGUILayout.ColorField("Helper Spline Color", RLManager.HelperSplineColor);
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Racing Line Settings", EditorStyles.boldLabel);
                RLManager.ClosedRacingLine = EditorGUILayout.Toggle("Closed Racing Line", RLManager.ClosedRacingLine, options);
                RLManager.RacingLineWidth = EditorGUILayout.FloatField("Racing Line Width", RLManager.RacingLineWidth, options);
                RLManager.StickToGround = EditorGUILayout.Toggle("Stick To Ground", RLManager.StickToGround, options);
                RLManager.GroundOffset = EditorGUILayout.FloatField("Ground Offset", RLManager.GroundOffset, options);
                RLManager.MeshResolution = EditorGUILayout.IntField("Mesh Resolution", RLManager.MeshResolution, options);
                RLManager.TextureTile = EditorGUILayout.IntField("Texture Tile Factor", RLManager.TextureTile, options);
                EditorGUILayout.Space();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Update Racing Line Mesh", GUI.skin.GetStyle("Button"), GUILayout.Height(25)))
                {
                    RLManager.UpdateRacingLine();
                }

                GUI.backgroundColor = defaultColor;
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Waypoint Copy Options", EditorStyles.boldLabel);
                RLManager.enableCopyMode = EditorGUILayout.Toggle("Enable Copy Mode", RLManager.enableCopyMode, options);

                if (RLManager.enableCopyMode)
                {
                    GUI.enabled = true;
                }
                else
                {
                    GUI.enabled = false;
                }

                RLManager.WaypointContainer = (GameObject)EditorGUILayout.ObjectField("Waypoint Container", RLManager.WaypointContainer, typeof(GameObject), true);
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Copy Waypoints as Racing Line Nodes", GUI.skin.GetStyle("Button"), GUILayout.Height(25)))
                {
                    RLManager.CopyWaypointItemsAsNode();
                }
                GUI.enabled = true;
                GUI.backgroundColor = defaultColor;
                GUILayout.EndVertical();

                return "";
            

        }


        internal static string CreateRGKWayPointItemInspector(WayPointItem WPItem)
        {

            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };


            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



            
                iAlignmentSpace = (Screen.width - 320) / 2;
                //buttons start
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New WP", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateWayPointItem();
                }
                if (GUILayout.Button("Insert WP", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.InsertWaypointItem(WPItem);
                }
                if (GUILayout.Button("Delete WP", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {

                }
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();

                //buttons end

                CreateVersionHeader("Waypoint Item");

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Speed Brake Options", EditorStyles.boldLabel);
                WPItem.SoftBrakeSpeed = EditorGUILayout.FloatField("Soft Brake Speed", WPItem.SoftBrakeSpeed);
                WPItem.HardBrakeSpeed = EditorGUILayout.FloatField("Hard Brake Speed", WPItem.HardBrakeSpeed);
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("AI Escape Points (Wider) Settings", EditorStyles.boldLabel);
                WPItem.SeperatedWiders = EditorGUILayout.Toggle("Edit Widers Individually", WPItem.SeperatedWiders);

                if (WPItem.SeperatedWiders)
                {
                    WPItem.LeftWide = EditorGUILayout.FloatField("Left Wider Width", WPItem.LeftWide);
                    WPItem.RightWide = EditorGUILayout.FloatField("Right Wider Width", WPItem.RightWide);
                }
                else
                {
                    WPItem.RightWide = EditorGUILayout.FloatField("Wider Width", WPItem.RightWide);
                }
                GUILayout.EndVertical();


                return "";
            


        }

        internal static string CreateRGKWayPointItemScreenInspector(WayPointItem WPItem)
        {

             
                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(Screen.width - 350, Screen.height - 80, 400, 50));
                GUILayout.BeginHorizontal();
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New WP", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateWayPointItem();
                }
                if (GUILayout.Button("Insert WP", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.InsertWaypointItem(WPItem);
                }
                if (GUILayout.Button("Delete WP", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {

                    if (EditorUtility.DisplayDialog("Please confirm object deletion?",
            "Are you sure you want to delete Waypioint " + WPItem.name + " from scene?", "YES", "NO"))
                    {
                        CoreFunctions.DeleteSelectedWaypoint(WPItem);
                    }
                }
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                Handles.EndGUI();
                return "";
            


        }

        internal static string CreateRGKRacingLinetItemInspector(RacingLineItem RLItem)
        {

            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };


            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



            
                iAlignmentSpace = (Screen.width - 320) / 2;
                //buttons start
                GUILayout.BeginHorizontal(options);
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New RLN", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateRacingLineItem();
                }
                if (GUILayout.Button("Insert RLN", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {
                    CoreFunctions.InsertRacingLineItem(RLItem);
                }
                if (GUILayout.Button("Delete RLN", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {

                    if (EditorUtility.DisplayDialog("Please confirm object deletion?",
                            "Are you sure you want to delete Racing Line Node " + RLItem.name + " from scene?", "YES", "NO"))
                    {
                        CoreFunctions.DeleteSelectedRacingLineNode(RLItem);
                    }
                }
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //buttons end

                CreateVersionHeader("Racing Line Node");

                return "";
           


        }

        internal static string CreateRGKRacingLineItemScreenInspector(RacingLineItem RLItem)
        {

            
                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(Screen.width - 350, Screen.height - 80, 400, 50));
                GUILayout.BeginHorizontal();
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New RLN", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(70), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateRacingLineItem();
                }
                if (GUILayout.Button("Insert RLN", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {
                    CoreFunctions.InsertRacingLineItem(RLItem);
                }
                if (GUILayout.Button("Delete RLN", GUI.skin.GetStyle("ButtonMid"), GUILayout.Width(75), GUILayout.Height(30)))
                {

                    if (EditorUtility.DisplayDialog("Please confirm object deletion?",
                            "Are you sure you want to delete Racing Line Node " + RLItem.name + " from scene?", "YES", "NO"))
                    {
                        CoreFunctions.DeleteSelectedRacingLineNode(RLItem);
                    }
                }
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(110), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                Handles.EndGUI();
                return "";
            


        }



        internal static string CreateRGKSpawnPointManagerInspector(SpawnPointManager SPManager)
        {

            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };
            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end


             
                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 230) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New SpawnPoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateSpawnPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //buttons end
                CreateVersionHeader("Spawnpoint Manager");
                return "";
            

        }

        internal static string CreateRGKSpawnPointInspector(SpawnPointItem SPItem)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end


            
                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 230) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New SpawnPoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateSpawnPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //buttons end

                CreateVersionHeader("Spawnpoint Item");

                return "";
             
        }

        internal static string CreateRGKSpawnPointScreenInspector(SpawnPointItem SPItem)
        {

            
                Handles.BeginGUI();
                GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 80, 400, 50));
                GUILayout.BeginHorizontal();
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New SpawnPoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateSpawnPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                Handles.EndGUI();
                return "";
             
        }

        internal static string CreateStartPointInspector(StartPointItem SPItem)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end  

            

                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 125) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("Button"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Start Point");

                return "";
             
        }

        internal static string CreateFinishPointInspector(FinishPointItem FPItem)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



            
                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 125) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("Button"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Finish Point");

                return "";
            
        }

        internal static string CreateCheckPointItemInspector(CheckPointItem CPItem)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



            
                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 230) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("New CheckPoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateCheckPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }

                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Checkpoint Item");

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Checkpoint Options", EditorStyles.boldLabel);

                CPItem.ItemType = (eCheckpointItemType)EditorGUILayout.EnumPopup("Item Type", CPItem.ItemType);

                if (CPItem.ItemType == eCheckpointItemType.Checkpoint)
                {
                    CPItem.CheckpointTime = EditorGUILayout.FloatField("Checkpoint Time to Reach (Seconds)", CPItem.CheckpointTime, options);
                    CPItem.CheckpointBonus = EditorGUILayout.FloatField("Checkpoint Bonus Time (Seconds)", CPItem.CheckpointBonus, options);
                }
                GUILayout.EndVertical();

                return "";
             
        }

        internal static string CreateCheckPointManagerInspector(CheckPointManager CPManager)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end



            
                //buttons start
                GUILayout.BeginHorizontal(options);
                iAlignmentSpace = (Screen.width - 230) / 2;
                GUILayout.Space(iAlignmentSpace);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("New Checkpoint", GUI.skin.GetStyle("ButtonLeft"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.CreateCheckPointItem();
                }

                if (GUILayout.Button("Align To Surface", GUI.skin.GetStyle("ButtonRight"), GUILayout.Width(115), GUILayout.Height(30)))
                {
                    CoreFunctions.AlignToSurface();
                }

                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();
                //buttons end

                CreateVersionHeader("Checkpoint  Manager");
                return "";
             
        }


        internal static string CreateTouchDriveInspector(RacingGameKit.TouchDrive.TouchDriveManager TDManager)
        {
            int iAlignmentSpace = (Screen.width - 310) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);
            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.TouchDriveLogo, GUIStyle.none, GUILayout.Width(310), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end
            EditorGUILayout.Space();

            
                CreateVersionHeader("TouchDrive Manager");
                GUI.changed = false;


                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Vehicle Controls", EditorStyles.boldLabel);
                TDManager.Throttle = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Throttle", TDManager.Throttle, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.Brake = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Brake", TDManager.Brake, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                EditorGUILayout.Space();
                TDManager.SteerLeft = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Left Steer", TDManager.SteerLeft, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.SteerRight = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Right Steer", TDManager.SteerRight, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                EditorGUILayout.Space();
                TDManager.ShiftUp = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Shift Up", TDManager.ShiftUp, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.ShiftDown = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Shift Down", TDManager.ShiftDown, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                EditorGUILayout.Space();
                TDManager.Wheel = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Touch Wheel", TDManager.Wheel, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                EditorGUILayout.Space();
                GUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Misc Controls", EditorStyles.boldLabel);
                TDManager.CameraButton = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Change Camera", TDManager.CameraButton, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.MirrorButton = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Mirror/Show Back Camera", TDManager.MirrorButton, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.ResetButton = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Reset Vehicle", TDManager.ResetButton, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.PauseButton = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Pause", TDManager.PauseButton, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.Misc1Button = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Misc Button 1", TDManager.Misc1Button, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                TDManager.Misc2Button = (RacingGameKit.TouchDrive.TouchItemBase)EditorGUILayout.ObjectField("Misc Button 2", TDManager.Misc2Button, typeof(RacingGameKit.TouchDrive.TouchItemBase), true);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
                TDManager.EnableMouseEmulation = EditorGUILayout.Toggle("Enable Mouse Emulation", TDManager.EnableMouseEmulation);
                EditorGUILayout.Space();

                GUILayout.EndVertical();
                EditorGUILayout.Space();


                if (GUI.changed)
                {
                    EditorUtility.SetDirty(TDManager);
                }
                return "";
            
        }






        internal static string CreateRGKCameraInspector(Race_Camera DFXManager)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();
            //logo end

            CreateVersionHeader("RGK Race Camera");

            return "";
            
        }


        internal static string CreateEarthFXInspector(EarthFX EarthFXControl)
        {
            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.EarthFXLogo, GUIStyle.none, GUILayout.Width(310), GUILayout.Height(60));
            GUILayout.EndHorizontal();



            
                CreateVersionHeader("EarthFX Manager");

                //globalfx
                GUILayout.BeginVertical("Box");

                EditorGUILayout.LabelField("GlobalFX", EditorStyles.boldLabel);
                //GUILayout.Label("This settings will apply to every surface that not configured below.");
                EditorGUILayout.HelpBox("This settings will apply to every surface that not configured on SurfaceFX section.", MessageType.Info, true);
                EditorGUILayout.Space();

                EarthFXControl.GlobalFX.BrakeSkid = (GameObject)EditorGUILayout.ObjectField("Brake Skidmarks", EarthFXControl.GlobalFX.BrakeSkid, typeof(GameObject), true);
                EarthFXControl.GlobalFX.BrakeSmoke = (GameObject)EditorGUILayout.ObjectField("Brake Smoke", EarthFXControl.GlobalFX.BrakeSmoke, typeof(GameObject), true);
                EarthFXControl.GlobalFX.BrakeSkidStartSlip = EditorGUILayout.FloatField("Brake Skids Start Slip", EarthFXControl.GlobalFX.BrakeSkidStartSlip);
                EditorGUILayout.Space();

                EarthFXControl.GlobalFX.Splatter = (GameObject)EditorGUILayout.ObjectField("Splatter Particles", EarthFXControl.GlobalFX.Splatter, typeof(GameObject), true);
                EarthFXControl.GlobalFX.SplatterStartVelocity = EditorGUILayout.FloatField("Spatter Start Velocity", EarthFXControl.GlobalFX.SplatterStartVelocity);
                EditorGUILayout.Space();

                EarthFXControl.GlobalFX.TrailSkid = (GameObject)EditorGUILayout.ObjectField("Trail Marks", EarthFXControl.GlobalFX.TrailSkid, typeof(GameObject), true);
                EarthFXControl.GlobalFX.TrailSmoke = (GameObject)EditorGUILayout.ObjectField("Trail Smoke", EarthFXControl.GlobalFX.TrailSmoke, typeof(GameObject), true);
                EarthFXControl.GlobalFX.TrailSmokeStartVelocity = EditorGUILayout.FloatField("Trail Smoke Start Velocity", EarthFXControl.GlobalFX.TrailSmokeStartVelocity);
                EditorGUILayout.Space();

                EarthFXControl.GlobalFX.SurfaceDriveSound = (AudioClip)EditorGUILayout.ObjectField("Surface Drive AudioClip", EarthFXControl.GlobalFX.SurfaceDriveSound, typeof(AudioClip), true);
                EarthFXControl.GlobalFX.BrakeSound = (AudioClip)EditorGUILayout.ObjectField("Brake AudioClip", EarthFXControl.GlobalFX.BrakeSound, typeof(AudioClip), true);
                EditorGUILayout.Space();

                GUILayout.EndVertical();

                GUILayout.BeginVertical("Box");
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("SurfaceFX", EditorStyles.boldLabel);
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("New SurfaceFX", GUILayout.Width(105f)))
                {
                    EarthFXControl.SurfaceFX.Add(new EarthFX.EarthFXData());
                }
                GUI.backgroundColor = defaultColor;
                GUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("This settings will apply when vehicle on defined textures only.", MessageType.Info, true);



                for (int i = 0; i < EarthFXControl.SurfaceFX.Count; i++)
                {
                    GUILayout.BeginVertical("Box");
                    GUILayout.BeginHorizontal();
                    EarthFXControl.SurfaceFX[i].Visible = EditorGUILayout.Foldout(EarthFXControl.SurfaceFX[i].Visible, EarthFXControl.SurfaceFX[i].FxName);
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Clone", GUILayout.Width(65f)))
                    {
                        EarthFX.EarthFXData eNew = EarthFXControl.SurfaceFX[i].Clone() as EarthFX.EarthFXData;
                        eNew.FxName = EarthFXControl.SurfaceFX[i].FxName + " Clone";
                        EarthFXControl.SurfaceFX.Add(eNew);

                    }
                    if (GUILayout.Button("X", GUILayout.Width(35f)))
                    {
                        if (EditorUtility.DisplayDialog("Confirm FX Remove", "Are you sure to remove this FX?", "Yes", "No"))
                        {
                            EarthFXControl.SurfaceFX.RemoveAt(i);
                        }
                    }
                    GUI.backgroundColor = defaultColor;
                    GUILayout.EndHorizontal();
                    if (EarthFXControl.SurfaceFX[i] != null)
                    {
                        if (EarthFXControl.SurfaceFX[i].Visible)
                        {
                            EditorGUILayout.Space();
                            EarthFXControl.SurfaceFX[i].FxName = EditorGUILayout.TextField("FX Name", EarthFXControl.SurfaceFX[i].FxName);
                            EarthFXControl.SurfaceFX[i].FxTexture = (Texture2D)EditorGUILayout.ObjectField("FX Texture", EarthFXControl.SurfaceFX[i].FxTexture, typeof(Texture2D), false);
                            EditorGUILayout.Space();
                            EarthFXControl.SurfaceFX[i].BrakeSkid = (GameObject)EditorGUILayout.ObjectField("Brake Skidmarks", EarthFXControl.SurfaceFX[i].BrakeSkid, typeof(GameObject), true);
                            EarthFXControl.SurfaceFX[i].BrakeSmoke = (GameObject)EditorGUILayout.ObjectField("Brake Smoke", EarthFXControl.SurfaceFX[i].BrakeSmoke, typeof(GameObject), true);
                            EarthFXControl.SurfaceFX[i].BrakeSkidStartSlip = EditorGUILayout.FloatField("Brake Skids Start Slip", EarthFXControl.SurfaceFX[i].BrakeSkidStartSlip);
                            EditorGUILayout.Space();

                            EarthFXControl.SurfaceFX[i].Splatter = (GameObject)EditorGUILayout.ObjectField("Splatter Particles", EarthFXControl.SurfaceFX[i].Splatter, typeof(GameObject), true);
                            EarthFXControl.SurfaceFX[i].SplatterStartVelocity = EditorGUILayout.FloatField("Spatter Start Velocity", EarthFXControl.SurfaceFX[i].SplatterStartVelocity);
                            EditorGUILayout.Space();

                            EarthFXControl.SurfaceFX[i].TrailSkid = (GameObject)EditorGUILayout.ObjectField("Trail Marks", EarthFXControl.SurfaceFX[i].TrailSkid, typeof(GameObject), true);
                            EarthFXControl.SurfaceFX[i].TrailSmoke = (GameObject)EditorGUILayout.ObjectField("Trail Smoke", EarthFXControl.SurfaceFX[i].TrailSmoke, typeof(GameObject), true);
                            EarthFXControl.SurfaceFX[i].TrailSmokeStartVelocity = EditorGUILayout.FloatField("Trail Smoke Start Velocity", EarthFXControl.SurfaceFX[i].TrailSmokeStartVelocity);
                            EditorGUILayout.Space();

                            EarthFXControl.SurfaceFX[i].SurfaceDriveSound = (AudioClip)EditorGUILayout.ObjectField("Surface Drive AudioClip", EarthFXControl.SurfaceFX[i].SurfaceDriveSound, typeof(AudioClip), true);
                            EarthFXControl.SurfaceFX[i].BrakeSound = (AudioClip)EditorGUILayout.ObjectField("Brake AudioClip", EarthFXControl.SurfaceFX[i].BrakeSound, typeof(AudioClip), true);
                            EditorGUILayout.Space();

                            EarthFXControl.SurfaceFX[i].EnableSpeedDeceleration = EditorGUILayout.Toggle("Enable Speed Deceleration", EarthFXControl.SurfaceFX[i].EnableSpeedDeceleration);
                            if (EarthFXControl.SurfaceFX[i].EnableSpeedDeceleration)
                            {
                                EditorGUILayout.HelpBox("This values will use for decelerating the vehicle on this surface.", MessageType.Info, true);
                                EarthFXControl.SurfaceFX[i].ForwardDrag = EditorGUILayout.FloatField("Forward Drag", EarthFXControl.SurfaceFX[i].ForwardDrag);
                                EarthFXControl.SurfaceFX[i].AngularDrag = EditorGUILayout.FloatField("Angular Drag", EarthFXControl.SurfaceFX[i].AngularDrag);
                                EditorGUILayout.Space();
                            }

                            EarthFXControl.SurfaceFX[i].EnableGripDecrease = EditorGUILayout.Toggle("Enable Grip Decrease", EarthFXControl.SurfaceFX[i].EnableGripDecrease);

                            if (EarthFXControl.SurfaceFX[i].EnableGripDecrease)
                            {
                                EditorGUILayout.HelpBox("This values will use for decreasing grip settings for enviroment.", MessageType.Info, true);
                                EarthFXControl.SurfaceFX[i].ForwardGripLosePercent = EditorGUILayout.FloatField("Forward Grip Lose %", EarthFXControl.SurfaceFX[i].ForwardGripLosePercent);
                                EarthFXControl.SurfaceFX[i].SidewaysGripLosePercent = EditorGUILayout.FloatField("Sideways Grip Lose %", EarthFXControl.SurfaceFX[i].SidewaysGripLosePercent);
                                EditorGUILayout.Space();
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(EarthFXControl);
                }

                return "";
             
        }

        internal static string CreateAIBasicInspector(RacingGameKit.Racers.RGK_Racer_Basic_AI AI)
        {

            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();



            
                CreateVersionHeader("RGK Basic AI");

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Rival Detection", EditorStyles.boldLabel);
                AI.AIRacerDedectionLayer = LayerMaskField("AI Detection Layer", AI.AIRacerDedectionLayer);
                AI.ObstacleDedectionLayer = LayerMaskField("Obstacle Detection Layer", AI.ObstacleDedectionLayer);
                AI.DedectionRadius = EditorGUILayout.FloatField("Detction Radius", AI.DedectionRadius);
                AI.DedectionFrequency = EditorGUILayout.FloatField("Detection Frequency (seconds)", AI.DedectionFrequency);
                AI.CollisionAvoidAngle = EditorGUILayout.FloatField("Collision Avoid Angle", AI.CollisionAvoidAngle);
                AI.CollisionAvoidTime = EditorGUILayout.FloatField("Collision Avoid Time", AI.CollisionAvoidTime);
                AI.CollisionAvoidFactor = EditorGUILayout.FloatField("Seperation Factor", AI.CollisionAvoidFactor);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Driving Settings", EditorStyles.boldLabel);
                AI.AISoftBrakeFactor = EditorGUILayout.FloatField("Soft Brake ", AI.AISoftBrakeFactor);
                AI.AIHardBrakeFactor = EditorGUILayout.FloatField("Hard Brake", AI.AIHardBrakeFactor);
                EditorGUILayout.Space();

                AI.UseThrottleBonus = EditorGUILayout.Toggle("Enable Throttle Bonus", AI.UseThrottleBonus);

                if (AI.UseThrottleBonus)
                {
                    AI.RandomizeBonusUsage = EditorGUILayout.Toggle("Random Throttle Bonus", AI.RandomizeBonusUsage);
                    AI.ThrottleBonus = EditorGUILayout.FloatField("Throttle Bonus (Max)", AI.ThrottleBonus);
                }
                EditorGUILayout.Space();
                AI.UseSteerSmoothing = EditorGUILayout.Toggle("Enable Steer Smoothing", AI.UseSteerSmoothing);

                if (AI.UseSteerSmoothing)
                {

                    EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing Range"), ref AI.SteerSmoothingLow, ref  AI.SteerSmoothingHigh, 1f, 40f);
                    AI.SteerSmoothingLow = EditorGUILayout.FloatField("Min", AI.SteerSmoothingLow);
                    if (AI.SteerSmoothingLow <= 0) AI.SteerSmoothingLow = 1;
                    if (AI.SteerSmoothingLow > AI.SteerSmoothingHigh) AI.SteerSmoothingLow = AI.SteerSmoothingHigh;
                    AI.SteerSmoothingHigh = EditorGUILayout.FloatField("Max", AI.SteerSmoothingHigh);
                    if (AI.SteerSmoothingHigh <= 0) AI.SteerSmoothingHigh = 1;
                    if (AI.SteerSmoothingHigh < AI.SteerSmoothingLow) AI.SteerSmoothingHigh = AI.SteerSmoothingLow;
                    AI.UseLerp = EditorGUILayout.Toggle("Use Lerp Method", AI.UseLerp);
                    AI.SteerSmoothingReleaseCoef = EditorGUILayout.Slider("Smoothing Release Coef", AI.SteerSmoothingReleaseCoef, 0.1f, 0.5f);
                }
                EditorGUILayout.Space();
                AI.nextWPCoef = EditorGUILayout.FloatField("Waypoint Change Speed", AI.nextWPCoef);
                EditorGUILayout.Space();
                AI.StuckResetWait = EditorGUILayout.FloatField("Seconds Before Reset on Stuck", AI.StuckResetWait);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Debug Helpers", EditorStyles.boldLabel);
                AI.ShowNavDebugLines = EditorGUILayout.Toggle("Draw Debug Lines", AI.ShowNavDebugLines);
                AI.DrawCollisonGizmo = EditorGUILayout.Toggle("Draw Collision", AI.DrawCollisonGizmo);
                EditorGUILayout.Space();
                GUI.enabled = false;
                AI.IsReversing = EditorGUILayout.Toggle("Is Reversing", AI.IsReversing);
                AI.IsBraking = EditorGUILayout.Toggle("Is Breaking", AI.IsBraking);
                AI.isSteeringLocked = EditorGUILayout.Toggle("Is Steering Locked", AI.isSteeringLocked);
                AI.ReverseLockedbyObstacle = EditorGUILayout.Toggle("Is Reverse Blocked", AI.ReverseLockedbyObstacle);
                EditorGUILayout.Space();
                AI.RGKThrottle = EditorGUILayout.FloatField("Throttle Input", AI.RGKThrottle);
                AI.RGKSteer = EditorGUILayout.FloatField("Steer Input", AI.RGKSteer);
                GUI.enabled = true;
                GUILayout.EndVertical();


                if (GUI.changed)
                {
                    EditorUtility.SetDirty(AI);
                }

                return "";
            


        }

        internal static string CreateAIProInspector(RacingGameKit.Racers.RGK_Racer_Pro_AI AI)
        {

            int iAlignmentSpace = (Screen.width - 300) / 2;

            GUILayout.Space(10);
            GUILayoutOption[] options = new GUILayoutOption[] { };

            //Logo
            GUILayout.BeginHorizontal(options);

            GUILayout.Space(iAlignmentSpace);
            GUILayout.Box(Helpers.GUIHelper.RGKLogoForInspectors, GUIStyle.none, GUILayout.Width(300), GUILayout.Height(60));
            GUILayout.EndHorizontal();



            
                CreateVersionHeader("RGK Pro AI");

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Configuration File", EditorStyles.boldLabel);
                AI.AiConfigFile = EditorGUILayout.TextField("AI Configuration File", AI.AiConfigFile);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("AI Skills", EditorStyles.boldLabel);
                AI.RandomizeBehaviors = EditorGUILayout.Toggle("Randomize on Start", AI.RandomizeBehaviors);
                EditorGUILayout.Space();
                AI.AILevelBehavior = (eAIProLevel)EditorGUILayout.EnumPopup("Driving Skill Level", AI.AILevelBehavior);
                AI.AIStateBehavior = (eAIBehavior)EditorGUILayout.EnumPopup("Driving Type", AI.AIStateBehavior);
                AI.AIStressBehavior = (eAIStress)EditorGUILayout.EnumPopup("Stress Level", AI.AIStressBehavior);
                EditorGUILayout.Space();
                AI.AISpeedFactor = EditorGUILayout.FloatField("Throttle Factor", AI.AISpeedFactor);

                AI.AISteerFactor = EditorGUILayout.FloatField("Steer Response", AI.AISteerFactor);
                AI.AIEscapeFactor = EditorGUILayout.FloatField("Escape Response", AI.AIEscapeFactor);

                EditorGUILayout.Space();

                AI.AISoftBrakeFactor = EditorGUILayout.FloatField("Soft Brake ", AI.AISoftBrakeFactor);
                AI.AIHardBrakeFactor = EditorGUILayout.FloatField("Hard Brake", AI.AIHardBrakeFactor);
                EditorGUILayout.Space();
                AI.UseSteerSmoothing = EditorGUILayout.Toggle("Enable Steer Smoothing", AI.UseSteerSmoothing);

                if (AI.UseSteerSmoothing)
                {
                    AI.SteerSmoothingMode = (eSteerSmoothingMode)EditorGUILayout.EnumPopup("Smoothing Mode", AI.SteerSmoothingMode);

                    if (AI.SteerSmoothingMode == eSteerSmoothingMode.Basic)
                    {
                        EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing Range"), ref AI.SteerSmoothingLow, ref  AI.SteerSmoothingHigh, 1f, 40f);

                        AI.SteerSmoothingLow = EditorGUILayout.FloatField("Min", AI.SteerSmoothingLow);
                        if (AI.SteerSmoothingLow <= 0) AI.SteerSmoothingLow = 1;
                        if (AI.SteerSmoothingLow > AI.SteerSmoothingHigh) AI.SteerSmoothingLow = AI.SteerSmoothingHigh;
                        AI.SteerSmoothingHigh = EditorGUILayout.FloatField("Max", AI.SteerSmoothingHigh);
                        if (AI.SteerSmoothingHigh <= 0) AI.SteerSmoothingHigh = 1;
                        if (AI.SteerSmoothingHigh < AI.SteerSmoothingLow) AI.SteerSmoothingHigh = AI.SteerSmoothingLow;
                    }
                    else
                    {
                        EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing for 0 to 30 FPS"), ref AI.FPSBasedSmoothingdata.m_0_30FPS.Min, ref  AI.FPSBasedSmoothingdata.m_0_30FPS.Max, 1f, 40f);
                        AI.FPSBasedSmoothingdata.m_0_30FPS.Min = EditorGUILayout.FloatField("Min", AI.FPSBasedSmoothingdata.m_0_30FPS.Min);
                        if (AI.FPSBasedSmoothingdata.m_0_30FPS.Min <= 0) AI.FPSBasedSmoothingdata.m_0_30FPS.Min = 1;
                        if (AI.FPSBasedSmoothingdata.m_0_30FPS.Min > AI.FPSBasedSmoothingdata.m_0_30FPS.Max) AI.FPSBasedSmoothingdata.m_0_30FPS.Min = AI.FPSBasedSmoothingdata.m_0_30FPS.Max;
                        AI.FPSBasedSmoothingdata.m_0_30FPS.Max = EditorGUILayout.FloatField("Max", AI.FPSBasedSmoothingdata.m_0_30FPS.Max);
                        if (AI.FPSBasedSmoothingdata.m_0_30FPS.Max <= 0) AI.FPSBasedSmoothingdata.m_0_30FPS.Max = 1;
                        if (AI.FPSBasedSmoothingdata.m_0_30FPS.Max < AI.FPSBasedSmoothingdata.m_0_30FPS.Min) AI.FPSBasedSmoothingdata.m_0_30FPS.Max = AI.FPSBasedSmoothingdata.m_0_30FPS.Min;

                        EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing for 30 to 45 FPS"), ref AI.FPSBasedSmoothingdata.m_30_45FPS.Min, ref  AI.FPSBasedSmoothingdata.m_30_45FPS.Max, 1f, 40f);
                        AI.FPSBasedSmoothingdata.m_30_45FPS.Min = EditorGUILayout.FloatField("Min", AI.FPSBasedSmoothingdata.m_30_45FPS.Min);
                        if (AI.FPSBasedSmoothingdata.m_30_45FPS.Min <= 0) AI.FPSBasedSmoothingdata.m_30_45FPS.Min = 1;
                        if (AI.FPSBasedSmoothingdata.m_30_45FPS.Min > AI.FPSBasedSmoothingdata.m_30_45FPS.Max) AI.FPSBasedSmoothingdata.m_30_45FPS.Min = AI.FPSBasedSmoothingdata.m_30_45FPS.Max;
                        AI.FPSBasedSmoothingdata.m_30_45FPS.Max = EditorGUILayout.FloatField("Max", AI.FPSBasedSmoothingdata.m_30_45FPS.Max);
                        if (AI.FPSBasedSmoothingdata.m_30_45FPS.Max <= 0) AI.FPSBasedSmoothingdata.m_30_45FPS.Max = 1;
                        if (AI.FPSBasedSmoothingdata.m_30_45FPS.Max < AI.FPSBasedSmoothingdata.m_30_45FPS.Min) AI.FPSBasedSmoothingdata.m_30_45FPS.Max = AI.FPSBasedSmoothingdata.m_30_45FPS.Min;

                        EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing for 45 to 60 FPS"), ref AI.FPSBasedSmoothingdata.m_45_60FPS.Min, ref  AI.FPSBasedSmoothingdata.m_45_60FPS.Max, 1f, 40f);
                        AI.FPSBasedSmoothingdata.m_45_60FPS.Min = EditorGUILayout.FloatField("Min", AI.FPSBasedSmoothingdata.m_45_60FPS.Min);
                        if (AI.FPSBasedSmoothingdata.m_45_60FPS.Min <= 0) AI.FPSBasedSmoothingdata.m_45_60FPS.Min = 1;
                        if (AI.FPSBasedSmoothingdata.m_45_60FPS.Min > AI.FPSBasedSmoothingdata.m_45_60FPS.Max) AI.FPSBasedSmoothingdata.m_45_60FPS.Min = AI.FPSBasedSmoothingdata.m_45_60FPS.Max;
                        AI.FPSBasedSmoothingdata.m_45_60FPS.Max = EditorGUILayout.FloatField("Max", AI.FPSBasedSmoothingdata.m_45_60FPS.Max);
                        if (AI.FPSBasedSmoothingdata.m_45_60FPS.Max <= 0) AI.FPSBasedSmoothingdata.m_45_60FPS.Max = 1;
                        if (AI.FPSBasedSmoothingdata.m_45_60FPS.Max < AI.FPSBasedSmoothingdata.m_45_60FPS.Min) AI.FPSBasedSmoothingdata.m_45_60FPS.Max = AI.FPSBasedSmoothingdata.m_45_60FPS.Min;

                        EditorGUILayout.MinMaxSlider(new GUIContent("Smoothing for 60 and more FPS"), ref AI.FPSBasedSmoothingdata.m_60_NFPS.Min, ref  AI.FPSBasedSmoothingdata.m_60_NFPS.Max, 1f, 40f);
                        AI.FPSBasedSmoothingdata.m_60_NFPS.Min = EditorGUILayout.FloatField("Min", AI.FPSBasedSmoothingdata.m_60_NFPS.Min);
                        if (AI.FPSBasedSmoothingdata.m_60_NFPS.Min <= 0) AI.FPSBasedSmoothingdata.m_60_NFPS.Min = 1;
                        if (AI.FPSBasedSmoothingdata.m_60_NFPS.Min > AI.FPSBasedSmoothingdata.m_60_NFPS.Max) AI.FPSBasedSmoothingdata.m_60_NFPS.Min = AI.FPSBasedSmoothingdata.m_60_NFPS.Max;
                        AI.FPSBasedSmoothingdata.m_60_NFPS.Max = EditorGUILayout.FloatField("Max", AI.FPSBasedSmoothingdata.m_60_NFPS.Max);
                        if (AI.FPSBasedSmoothingdata.m_60_NFPS.Max <= 0) AI.FPSBasedSmoothingdata.m_60_NFPS.Max = 1;
                        if (AI.FPSBasedSmoothingdata.m_60_NFPS.Max < AI.FPSBasedSmoothingdata.m_60_NFPS.Min) AI.FPSBasedSmoothingdata.m_60_NFPS.Max = AI.FPSBasedSmoothingdata.m_60_NFPS.Min;

                    }
                    AI.UseLerp = EditorGUILayout.Toggle("Use Lerp Method", AI.UseLerp);
                    AI.SteerSmoothingReleaseCoef = EditorGUILayout.Slider("Smoothing Release Coef", AI.SteerSmoothingReleaseCoef, 0.1f, 0.5f);
                    EditorGUILayout.Space();
                }

                AI.nextWPCoef = EditorGUILayout.FloatField("Waypoint Change Speed", AI.nextWPCoef);
                EditorGUILayout.Space();
                AI.StuckResetWait = EditorGUILayout.FloatField("Seconds Before Reset on Stuck", AI.StuckResetWait);
                GUILayout.EndVertical();
                EditorGUILayout.Space();


                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Rival Detection", EditorStyles.boldLabel);
                AI.AIRacerDedectionLayer = LayerMaskField("AI Detection Layer", AI.AIRacerDedectionLayer);
                AI.ObstacleDedectionLayer = LayerMaskField("Obstacle Detection Layer", AI.ObstacleDedectionLayer);
                AI.DedectionRadius = EditorGUILayout.FloatField("Detection Radius", AI.DedectionRadius);
                AI.DedectionFrequency = EditorGUILayout.FloatField("Detection Frequency (seconds)", AI.DedectionFrequency);
                AI.CollisionAvoidAngle = EditorGUILayout.FloatField("Collision Avoid Angle", AI.CollisionAvoidAngle);
                AI.CollisionAvoidTime = EditorGUILayout.FloatField("Collision Avoid Time", AI.CollisionAvoidTime);
                AI.CollisionAvoidFactor = EditorGUILayout.FloatField("Seperation Factor", AI.CollisionAvoidFactor);
                GUILayout.EndVertical();
                EditorGUILayout.Space();


                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Detection Sensors", EditorStyles.boldLabel);
                AI.ForwardSensorDistance = EditorGUILayout.FloatField("Forward Sensor Distance", AI.ForwardSensorDistance);
                AI.ForwardSensorAngle = EditorGUILayout.FloatField("Forward Sensor Angle", AI.ForwardSensorAngle);
                //AI.ForwardSensorWide = EditorGUILayout.FloatField("Forward Sensor Width", AI.ForwardSensorWide);
                EditorGUILayout.Space();
                AI.SideSensorDistance = EditorGUILayout.FloatField("Side Sensor Distance", AI.SideSensorDistance);
                AI.SideSensorAngle = EditorGUILayout.FloatField("Side Sensor Angle", AI.SideSensorAngle);
                //AI.SideSensorWide = EditorGUILayout.FloatField("Side Sensor Width", AI.SideSensorWide);
                EditorGUILayout.Space();
                AI.WallSensorDistance = EditorGUILayout.FloatField("Wall Sensor Distance", AI.WallSensorDistance);
                AI.ReverseSensorDistance = EditorGUILayout.FloatField("Reverse Sensor Distance", AI.ReverseSensorDistance);
                EditorGUILayout.Space();
                AI.UseSideCorrection = EditorGUILayout.Toggle("Side Correction Enabled", AI.UseSideCorrection);
                GUILayout.EndVertical();
                EditorGUILayout.Space();

                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Obstacle Avoidance", EditorStyles.boldLabel);
                AI.ObstacleAvoidingEnabled = EditorGUILayout.Toggle("Enable Obstacle Avoidance", AI.ObstacleAvoidingEnabled);
                if (AI.ObstacleAvoidingEnabled)
                {
                    AI.ObstacleDetectionDistance = EditorGUILayout.FloatField("Detection Distance", AI.ObstacleDetectionDistance);
                    AI.ObstacleAvoidFactor = EditorGUILayout.FloatField("Avoid Factor", AI.ObstacleAvoidFactor);
                }
                GUILayout.EndVertical();
                EditorGUILayout.Space();





                GUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Debug Helpers", EditorStyles.boldLabel);
                AI.ShowNavDebugLines = EditorGUILayout.Toggle("Draw Debug Lines", AI.ShowNavDebugLines);
                AI.DrawCollisonGizmo = EditorGUILayout.Toggle("Draw Collision", AI.DrawCollisonGizmo);
                AI.EnableDebugMessages = EditorGUILayout.Toggle("Enable Debug Messages", AI.EnableDebugMessages);
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Debug States", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This controls disabled because they are only for viewing current states", MessageType.Info);
                GUI.enabled = false;
                AI.CurrentRoadPosition = (eAIRoadPosition)EditorGUILayout.EnumPopup("Road Position", AI.CurrentRoadPosition);
                AI.AIAvoidingRivalAvoidPosition = (eAIRivalPosition)EditorGUILayout.EnumPopup("Avoiding Rival Position", AI.AIAvoidingRivalAvoidPosition);
                AI.AIAvoidingRivalRoadPosition = (eAIRoadPosition)EditorGUILayout.EnumPopup("Avoiding Rival Road Position", AI.AIAvoidingRivalRoadPosition);


                AI.OverlappedCount = EditorGUILayout.FloatField("Overlapped Count", AI.OverlappedCount);
                AI.DedectedRacerCount = EditorGUILayout.FloatField("Detected Racer Count", AI.DedectedRacerCount);

                AI.IsReversing = EditorGUILayout.Toggle("Is Reversing", AI.IsReversing);
                AI.IsBraking = EditorGUILayout.Toggle("Is Breaking", AI.IsBraking);
                AI.ReverseLockedbyObstacle = EditorGUILayout.Toggle("Is Reverse Blocked", AI.ReverseLockedbyObstacle);
                EditorGUILayout.Space();
                AI.AIWillAvoid = EditorGUILayout.Toggle("Will Avoid", AI.AIWillAvoid);
                AI.AICanEscape = EditorGUILayout.Toggle("Can Escape", AI.AICanEscape);
                AI.AIWillEscape = EditorGUILayout.Toggle("Will Escape", AI.AIWillEscape);

                AI.ObstacleAvoidToLeft = EditorGUILayout.Toggle("Avoid Obstacle To Left", AI.ObstacleAvoidToLeft);
                AI.ObstacleAvoidToRight = EditorGUILayout.Toggle("Avoid Obstacle To Right", AI.ObstacleAvoidToRight);
                AI.ObstacleIsAvoiding = EditorGUILayout.Toggle("Is Obstacle Avoiding", AI.ObstacleIsAvoiding);


                EditorGUILayout.Space();
                AI.RGKThrottle = EditorGUILayout.FloatField("Throttle Data", AI.RGKThrottle);
                AI.RGKSteer = EditorGUILayout.FloatField("Steer Data", AI.RGKSteer);
                GUI.enabled = true;
                GUILayout.EndVertical();


                if (GUI.changed)
                {
                    EditorUtility.SetDirty(AI);
                }

                return "";
            


        }

        private static void CreateVersionHeader(string InspectorName)
        {
            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("Box");
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField(InspectorName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Racing Game Kit Source v" + GetAssemblyVersion());
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            GUI.backgroundColor = defaultColor;
        }

        internal static string GetAssemblyVersion()
        {
          
            return RacingGameKit.Editors.Version.VERSIONNO;
        }


        public static LayerMask LayerMaskField(string label, LayerMask selected)
        {
            return LayerMaskField(label, selected, true);
        }

        public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
        {

            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            string selectedLayers = "";

            for (int i = 0; i < 32; i++)
            {

                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    if (selected == (selected | (1 << i)))
                    {

                        if (selectedLayers == "")
                        {
                            selectedLayers = layerName;
                        }
                        else
                        {
                            selectedLayers = "Mixed";
                        }
                    }
                }
            }

            EventType lastEvent = Event.current.type;

            if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
            {
                if (selected.value == 0)
                {
                    layers.Add("Nothing");
                }
                else if (selected.value == -1)
                {
                    layers.Add("Everything");
                }
                else
                {
                    layers.Add(selectedLayers);
                }
                layerNumbers.Add(-1);
            }

            if (showSpecial)
            {
                layers.Add((selected.value == 0 ? "> " : " ") + "Nothing");
                layerNumbers.Add(-2);

                layers.Add((selected.value == -1 ? "> " : " ") + "Everything");
                layerNumbers.Add(-3);
            }

            for (int i = 0; i < 32; i++)
            {

                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    if (selected == (selected | (1 << i)))
                    {
                        layers.Add("> " + layerName);
                    }
                    else
                    {
                        layers.Add(" " + layerName);
                    }
                    layerNumbers.Add(i);
                }
            }

            bool preChange = GUI.changed;

            GUI.changed = false;

            int newSelected = 0;

            if (Event.current.type == EventType.MouseDown)
            {
                newSelected = -1;
            }

            newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

            if (GUI.changed && newSelected >= 0)
            {
                //newSelected -= 1;

                //Debug.Log(lastEvent + " " + newSelected + " " + layerNumbers[newSelected]);

                if (showSpecial && newSelected == 0)
                {
                    selected = 0;
                }
                else if (showSpecial && newSelected == 1)
                {
                    selected = -1;
                }
                else
                {

                    if (selected == (selected | (1 << layerNumbers[newSelected])))
                    {
                        selected &= ~(1 << layerNumbers[newSelected]);
                        //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
                    }
                    else
                    {
                        //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
                        selected = selected | (1 << layerNumbers[newSelected]);
                    }
                }
            }
            else
            {
                GUI.changed = preChange;
            }

            return selected;
        }



        #endregion


    }
}