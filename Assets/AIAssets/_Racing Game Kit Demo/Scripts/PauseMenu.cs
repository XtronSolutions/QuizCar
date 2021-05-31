using UnityEngine;
using System.Collections;
using RacingGameKit.RGKCar.CarControllers;
using RacingGameKit.TouchDrive;
public class PauseMenu : MonoBehaviour
{
    public GameObject touchDriveManager;
    public iRGKTDM touchDriveManagerComponent;

    public GUISkin skin;

    private float gldepth = -0.5f;
    private float startTime = 0.1f;

    public Material mat;

    private long tris = 0;
    private long verts = 0;
    private float savedTimeScale;
    //private SepiaToneEffect pauseFilter;

    private bool showfps = true;
    private bool showtris;
    private bool showvtx;
    private bool showfpsgraph;

    public Color lowFPSColor = Color.red;
    public Color highFPSColor = Color.green;

    public int lowFPS = 20;
    public int highFPS = 30;

    public GameObject start;

    public string url = "http://www.unityracingkit.com";

    public Color statColor = Color.yellow;

    public string[] credits = {
	    "Racing Game Kit for Unity3D",
	    "",
	    "",
	    "Copyright (c) 2012 Yusuf AKDAG"};
    public Texture[] crediticons;

    public enum Page
    {
        None, Main, Options, Credits
    }

    private Page currentPage;

    private float[] fpsarray;
    private float fps;

    private int toolbarInt = 0;
    private string[] toolbarstrings = { "Audio", "Graphics", "Stats", "System" };


    void Start()
    {
        if (touchDriveManager != null)
        {
            touchDriveManagerComponent = touchDriveManager.GetComponent(typeof(iRGKTDM)) as iRGKTDM;
        }


        iScreenWidth = Screen.width;
        iScreenHeigt = Screen.height;

        fpsarray = new float[Screen.width];
        Time.timeScale = 1;
        //pauseFilter = Camera.main.GetComponent<SepiaToneEffect>();
        //PauseGame();
    }

    void OnPostRender()
    {
        if (showfpsgraph && mat != null)
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            for (var i = 0; i < mat.passCount; ++i)
            {
                mat.SetPass(i);
                GL.Begin(GL.LINES);
                for (int x = 0; x < fpsarray.Length; ++x)
                {
                    GL.Vertex3(x, fpsarray[x], gldepth);
                }
                GL.End();
            }
            GL.PopMatrix();
            ScrollFPS();
        }
    }

    void ScrollFPS()
    {
        for (int x = 1; x < fpsarray.Length; ++x)
        {
            fpsarray[x - 1] = fpsarray[x];
        }
        if (fps < 1000)
        {
            fpsarray[fpsarray.Length - 1] = fps;
        }
    }

    static bool IsDashboard()
    {
//        return Application.platform == RuntimePlatform.OSXDashboardPlayer;
		return false;
    }

    static bool IsBrowser()
    {
//        return (Application.platform == RuntimePlatform.WindowsWebPlayer ||
//            Application.platform == RuntimePlatform.OSXWebPlayer);
		return false;
    }

    void LateUpdate()
    {
        windowPause= new Rect(0, 0, iScreenWidth, iScreenHeigt);

        if (showfps || showfpsgraph)
        {
            FPSUpdate();
        }
        bool PauseButtonPressed=false;

        if (touchDriveManagerComponent != null)
        {
            if (touchDriveManagerComponent.TouchItems[10] != null)
            {
                PauseButtonPressed = touchDriveManagerComponent.TouchItems[10].IsPressed;
            }
        }

        if (Input.GetKeyDown("escape") || PauseButtonPressed) 
        {
            switch (currentPage)
            {
                case Page.None:
                    PauseGame();
                    break;

                case Page.Main:
                    if (!IsBeginning())
                        UnPauseGame();
                    break;

                default:
                    currentPage = Page.Main;
                    break;
            }
        }
    }
    private Rect windowPause;
    private float iScreenWidth = 0;
    private float iScreenHeigt = 0;

    void OnGUI()
    {
        
        if (skin != null)
        {
            GUI.skin = skin;
        }
        ShowStatNums();
        ShowLegal();
        if (IsGamePaused())
        {
            windowPause = GUI.Window(999, windowPause, RenderMenu, "", "PauseMenuWindow");
            GUI.BringWindowToFront(999);
           
        }
    }

    void RenderMenu(int windowID)
    {
        
        GUI.color = statColor;
        switch (currentPage)
        {
            case Page.Main: MainPauseMenu(); break;
            case Page.Options: ShowToolbar(); break;
            case Page.Credits: ShowCredits(); break;
        }
        GUI.BringWindowToFront(999);
    }

    void ShowLegal()
    {
        if (!IsLegal())
        {
            GUI.Label(new Rect(Screen.width - 100, Screen.height - 20, 90, 20),
            "unityracingkit.com");
        }
    }

    bool IsLegal()
    {
        return !IsBrowser() ||
        Application.absoluteURL.StartsWith("http://www.unityracingkit.com") ||
        Application.absoluteURL.StartsWith("http://www.unityracingkit.com");

    }

    void ShowToolbar()
    {
        BeginPage(800, 300);
        toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarstrings, "BigButton", GUILayout.Width(800), GUILayout.Height(50));
        switch (toolbarInt)
        {
            case 0: VolumeControl(); break;
            case 3: ShowDevice(); break;
            case 1: Qualities(); QualityControl(); break;
            case 2: StatControl(); break;
        }
        EndPage();
    }

    void ShowCredits()
    {
        BeginPage(300, 300);
        foreach (string credit in credits)
        {
            GUILayout.Label(credit);
        }
        foreach (Texture credit in crediticons)
        {
            GUILayout.Label(credit);
        }
        EndPage();
    }

    void ShowBackButton()
    {
        if (GUI.Button(new Rect(20, Screen.height - 50, 100, 50), "Back"))
        {
            currentPage = Page.Main;
        }
    }

    void ShowDevice()
    {
        GUILayout.Label("Unity player version " + Application.unityVersion);
        GUILayout.Label("Graphics: " + SystemInfo.graphicsDeviceName + " " +
        SystemInfo.graphicsMemorySize + "MB\n" +
        SystemInfo.graphicsDeviceVersion + "\n" +
        SystemInfo.graphicsDeviceVendor);
        GUILayout.Label("Shadows: " + SystemInfo.supportsShadows);
        GUILayout.Label("Image Effects: " + SystemInfo.supportsImageEffects);
        GUILayout.Label("Render Textures: " + SystemInfo.supportsRenderTextures);
    }

    void Qualities()
    {
        switch (QualitySettings.GetQualityLevel())
        {
            case 1:
                GUILayout.Label("Fastest");
                break;
            case 2:
                GUILayout.Label("Fast");
                break;
            case 3:
                GUILayout.Label("Simple");
                break;
            case 4:
                GUILayout.Label("Good");
                break;
            case 5:
                GUILayout.Label("Beautiful");
                break;
            case 6:
                GUILayout.Label("Fantastic");
                break;
        }
    }

    void QualityControl()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Decrease", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            QualitySettings.DecreaseLevel();
        }
        if (GUILayout.Button("Increase", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            QualitySettings.IncreaseLevel();
        }
        GUILayout.EndHorizontal();
    }

    void VolumeControl()
    {
        GUILayout.Label("Volume");
        AudioListener.volume = GUILayout.HorizontalSlider(AudioListener.volume, 0, 1);
    }

    void StatControl()
    {
        GUILayout.BeginHorizontal();
        showfps = GUILayout.Toggle(showfps, "FPS");
        showtris = GUILayout.Toggle(showtris, "Triangles");
        showvtx = GUILayout.Toggle(showvtx, "Vertices");
        showfpsgraph = GUILayout.Toggle(showfpsgraph, "FPS Graph");
        GUILayout.EndHorizontal();
    }

    void FPSUpdate()
    {
        float delta = Time.smoothDeltaTime;
        if (!IsGamePaused() && delta != 0.0)
        {
            fps = 1 / delta;
        }
    }

    void ShowStatNums()
    {
        Color guiDefault = GUI.color;
        GUILayout.BeginArea(new Rect(Screen.width - 100, 10, 100, 200));
        if (showfps)
        {
            string fpsstring = fps.ToString("#,##0 fps");
            GUI.color = Color.Lerp(lowFPSColor, highFPSColor, (fps - lowFPS) / (highFPS - lowFPS));
            GUILayout.Label(fpsstring);
        }
        if (showtris || showvtx)
        {
            GetObjectStats();
            GUI.color = statColor;
        }
        if (showtris)
        {
            GUILayout.Label(tris + "tri");
        }
        if (showvtx)
        {
            GUILayout.Label(verts + "vtx");
        }
        GUILayout.EndArea();
        GUI.color = guiDefault;
    }

    void BeginPage(int width, int height)
    {
        GUILayout.BeginArea(new Rect((Screen.width - width) / 2, (Screen.height - height) / 2, width, height));
    }

    void EndPage()
    {
        GUILayout.EndArea();
        if (currentPage != Page.Main)
        {
            ShowBackButton();
        }
    }

    bool IsBeginning()
    {
        return (Time.time < startTime);
    }


    void MainPauseMenu()
    {
        BeginPage(200, 250);

        if (GUILayout.Button(IsBeginning() ? "Play" : "Continue", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            UnPauseGame();
        }

        if (GUILayout.Button("Home","BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            UnPauseGame();
            Application.LoadLevel(0);
        }

        if (GUILayout.Button("Restart", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (GUILayout.Button("Options", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            currentPage = Page.Options;
        }
        if (GUILayout.Button("Credits", "BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            currentPage = Page.Credits;
        }
        if (IsBrowser() && !IsBeginning() && GUILayout.Button("Restart","BigButton", GUILayout.Width(200), GUILayout.Height(50)))
        {
            Application.OpenURL(url);
        }
        EndPage();
    }

    void GetObjectStats()
    {
        verts = 0;
        tris = 0;
        GameObject[] ob = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in ob)
        {
            GetObjectStats(obj);
        }
    }

    void GetObjectStats(GameObject obj)
    {
        Component[] filters;
        filters = obj.GetComponentsInChildren<MeshFilter>();
        foreach (MeshFilter f in filters)
        {
            tris += f.sharedMesh.triangles.Length / 3;
            verts += f.sharedMesh.vertexCount;
        }
    }

    public void PauseGame()
    {
        savedTimeScale = Time.timeScale;
        Time.timeScale = 0;
        AudioListener.pause = true;
        //if (pauseFilter)
        //    pauseFilter.enabled = true;
        currentPage = Page.Main;
    }

    void UnPauseGame()
    {
        Time.timeScale = savedTimeScale;
        AudioListener.pause = false;
        //if (pauseFilter)
        //    pauseFilter.enabled = false;

        currentPage = Page.None;

        if (IsBeginning() && start != null)
        {
            start.active = true;
        }
    }

    bool IsGamePaused()
    {
        return (Time.timeScale == 0);
    }

    void OnApplicationPause(bool pause)
    {
        if (IsGamePaused())
        {
            AudioListener.pause = true;
        }
    }
}