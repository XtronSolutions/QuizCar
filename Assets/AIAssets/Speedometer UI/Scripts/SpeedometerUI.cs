using UnityEngine;
using System;
using System.Collections;



/// <summary>
/// 
/// </summary>
[AddComponentMenu("Racing Game Kit/Speedometer UI/Speedometer UI"), ExecuteInEditMode]
public class SpeedometerUI : MonoBehaviour
{
    #region Locals


    /// <summary>
    /// Enables/Disables loading settings from file on start.
    /// </summary>
    public Boolean LoadSettingsFromFile = true;
    /// <summary>
    /// Gauge position enumeration of SpeedometerUI Control
    /// </summary>
    public enum GaugePositionEnum
    {
        TopRight = 0,
        TopLeft = 1,
        TopCenter = 2,
        BottomRight = 3,
        BottomLeft = 4,
        BottomCenter = 5
    }
    /// <summary>
    /// Template data file. Content is predefined.
    /// </summary>
    [HideInInspector]
    public TextAsset ConfigurationAsset;
    /// <summary>
    /// Control Name for querying for special cases like seperated usage of gauages
    /// </summary>
    public String ControlName = "";
    public int Depth = 0;
    /// <summary>
    /// Gauge size 
    /// </summary>
    public Vector2 GaugeSize = new Vector2(100f, 100f);
    /// <summary>
    /// Gauge Position of SpeedometerUI Control
    /// </summary>
    public GaugePositionEnum GaugePosition;
    /// <summary>
    /// Gauge Position offset of Speedometer Control
    /// </summary>
    public Vector2 GaugePositionOffset = new Vector2(1f, 1f);
    /// <summary>
    /// Gauge Texture of Speedometer UI
    /// </summary>
    public Texture2D GaugeDial;
    /// <summary>
    /// Cap texture of speedometer needle
    /// </summary>
    public Texture2D SpeedoCapTexture;
    /// <summary>
    /// Size of the SpeedoCap
    /// </summary>
    public Vector2 SpeedoCapSize = new Vector2(5f, 5f);
    /// <summary>
    /// Texture of speedometer needle itself
    /// </summary>
    public Texture2D SpeedoNeedleTexture;

    public Vector2 SpeedoNeedleSize = new Vector2(5f, 10f);
    /// <summary>
    /// Needle offset from gauge
    /// </summary>
    public Vector2 SpeedoNeedleOffset;
    /// <summary>
    /// Speedometer needle distance to center
    /// </summary>
    public float SpeedoNeedleCenterDistance = 90;
    /// <summary>
    /// Speedometer needle min turn angle
    /// </summary>
    public float SpeedoNeedleMinValueAngle = -210f;
    /// <summary>
    /// Speedometer needle max turn angle
    /// </summary>
    public float SpeedoNeedleMaxValueAngle = 30;
    /// <summary>
    /// Max speedometer value
    /// This value will use to turning calculations. So please be accurate.
    /// </summary>
    public float SpeedoNeedleMaxValue = 240;

    /// <summary>
    /// Enable RPM needle
    /// </summary>
    public bool EnableRPM = true;
    /// <summary>
    /// RPM needle cap texture
    /// </summary>
    public Texture2D RPMCapTexture;

    public Vector2 RPMCapSize = new Vector2(5f, 5f);

    /// <summary>
    /// RPM needle texture
    /// </summary>
    public Texture2D RPMNeedleTexture;

    public Vector2 RPMNeedleSize = new Vector2(5f, 30f);
    /// <summary>
    /// RPM needle offset
    /// </summary>
    public Vector2 RPMNeedleOffset;
    /// <summary>
    /// RPM needle distance to center
    /// </summary>
    public float RPMNeedleCenterDistance = 58;
    /// <summary>
    /// RPM needle minimum angle
    /// </summary>
    public float RPMNeedleMinValueAngle = -210;
    /// <summary>
    /// RPM needle maximum angle
    /// </summary>
    public float RPMNeedleMaxValueAngle = 30;
    /// <summary>
    /// RPM Max value
    /// </summary>
    public float RPMNeedleMaxValue = 8000;


    /// <summary>
    /// Display speed text or not
    /// </summary>
    public bool ShowSpeedText = true;
    /// <summary>
    /// Current Speed
    /// </summary>
    public float Speed = 0;
    /// <summary>
    /// Speed texture offset to gauge
    /// </summary>
    public Vector2 SpeedTextOffset = new Vector2(72f, 113f);

    public bool ShowSpeedUnitText = true;
    public String SpeedUnitText = "Km/h";
    public Vector2 SpeedUnitTextOffset = new Vector2(72f, 113f);
    /// <summary>
    /// Display RPM text or not
    /// </summary>
    public bool ShowRPMText = true;
    /// <summary>
    /// Current RPM
    /// </summary>
    public float RPM = 0;
    /// <summary>
    /// RPM text offset to gauge
    /// </summary>
    public Vector2 RPMTextOffset = new Vector2(72f, 140f);
    /// <summary>
    /// Display Gear text or not
    /// </summary>
    public bool ShowGearText = true;
    /// <summary>
    /// Gear value
    /// </summary>
    public string Gear = "0";
    /// <summary>
    /// Gear text offset to gauge
    /// </summary>
    public Vector2 GearTextOffset = new Vector2(72f, 84f);
    /// <summary>
    /// Skin data of the SpeedometerUI
    /// </summary>
    public GUISkin SpeedoSkin = null;

    private int _screenHeight;
    private int _screenWidth;
    private float _gaugePositionX = 0;
    private float _gaugePositionY = 0;

    private GUIStyle myGuiStyle;

    #endregion


    void Start()
    {
        _screenHeight = Screen.height;
        _screenWidth = Screen.width;
        if (LoadSettingsFromFile)
        {
            loadJSONData();
        }
        RefreshScreenPosition();
    }

    void OnGUI()
    {

        if (SpeedoSkin == null) return;
        if (GaugeDial == null) return;

        GUI.skin = SpeedoSkin;
        GUI.depth = Depth;
        //gauge itself
        GUI.DrawTexture(new Rect(_gaugePositionX, _gaugePositionY, GaugeSize.x, GaugeSize.y), GaugeDial);

        if (ShowGearText)
        {
            GUI.Label(new Rect(_gaugePositionX + GearTextOffset.x, _gaugePositionY + GearTextOffset.y, 100, 20), System.String.Format("{0:0}", Gear), "GearText");
        }

        //Vector2 _vNeedleCapCenter = new Vector2((_gaugePositionX + GaugeSize.x / 4), (_gaugePositionY + GaugeSize.y / 4));
        //GUI.DrawTexture(new Rect(_vNeedleCapCenter.x, _vNeedleCapCenter.y, GagueNeedle.width, GagueNeedle.height), GagueNeedle);
        if (ShowSpeedText)
        {
            GUI.Label(new Rect(_gaugePositionX + SpeedTextOffset.x, _gaugePositionY + SpeedTextOffset.y, 100, 20), System.String.Format("{0:0}", Speed), "SpeedoText");

        }

        if (ShowSpeedUnitText)
        {
            GUI.Label(new Rect(_gaugePositionX + SpeedUnitTextOffset.x, _gaugePositionY + SpeedUnitTextOffset.y, 100, 20), SpeedUnitText, "UnitText");
        }


        //speedo
        if (SpeedoNeedleTexture != null)
        {
            Matrix4x4 SPeedoMatrix = GUI.matrix;
            Vector2 _vSpeedoNeedleCenter = new Vector2(_gaugePositionX + GaugeSize.x / 2 + SpeedoNeedleOffset.x, _gaugePositionY + GaugeSize.y / 2 + SpeedoNeedleOffset.y);
            if (SpeedoNeedleMaxValue == 0) SpeedoNeedleMaxValue = 1;
            float SpeedoFraction = Speed / SpeedoNeedleMaxValue;
            float SpeedoNeedleAngle = Mathf.Lerp(SpeedoNeedleMinValueAngle, SpeedoNeedleMaxValueAngle, SpeedoFraction);
            GUIUtility.RotateAroundPivot(SpeedoNeedleAngle, _vSpeedoNeedleCenter);
            GUI.DrawTexture(new Rect(_vSpeedoNeedleCenter.x + SpeedoNeedleCenterDistance, _vSpeedoNeedleCenter.y - SpeedoNeedleSize.y / 2, SpeedoNeedleSize.x, SpeedoNeedleSize.y), SpeedoNeedleTexture);
            GUI.matrix = SPeedoMatrix;

            if (SpeedoCapTexture != null)
            {
                Vector2 vSpeedoCapCenter = new Vector2(_gaugePositionX + GaugeSize.x / 2 + SpeedoNeedleOffset.x - SpeedoCapSize.x / 2,
                    _gaugePositionY + GaugeSize.y / 2 + SpeedoNeedleOffset.y - SpeedoCapSize.y / 2);
                GUI.DrawTexture(new Rect(vSpeedoCapCenter.x, vSpeedoCapCenter.y, SpeedoCapSize.x, SpeedoCapSize.y), SpeedoCapTexture);
            }
        }

        //RPM
        if (EnableRPM)
        {
            if (ShowRPMText)
            {
                GUI.Label(new Rect(_gaugePositionX + RPMTextOffset.x, _gaugePositionY + RPMTextOffset.y, 100, 20), System.String.Format("{0:0}", RPM), "RPMText");
            }

            if (RPMNeedleMaxValue == 0) RPMNeedleMaxValue = 1;
            
            Matrix4x4 RPMMatrix = GUI.matrix;
            Vector2 _vRPMNeedleCenter = new Vector2(_gaugePositionX + GaugeSize.x / 2 + RPMNeedleOffset.x, _gaugePositionY + GaugeSize.y / 2 + RPMNeedleOffset.y);
            
            float RPMFraction = RPM / RPMNeedleMaxValue;
            float RPMNeedleAngle = Mathf.Lerp(RPMNeedleMinValueAngle, RPMNeedleMaxValueAngle, RPMFraction);
            
            GUIUtility.RotateAroundPivot(RPMNeedleAngle, _vRPMNeedleCenter);
            if (RPMNeedleTexture != null)
            {
                GUI.DrawTexture(new Rect(_vRPMNeedleCenter.x + RPMNeedleCenterDistance, _vRPMNeedleCenter.y - RPMNeedleSize.y / 2, RPMNeedleSize.x, RPMNeedleSize.y), RPMNeedleTexture);
            }
            GUI.matrix = RPMMatrix;

            if (RPMCapTexture != null)
            {
                Vector2 vRPMCapCenter = new Vector2(_gaugePositionX + GaugeSize.x / 2 + RPMNeedleOffset.x - RPMCapTexture.width / 2,
                    _gaugePositionY + GaugeSize.y / 2 + RPMNeedleOffset.y - RPMCapTexture.height / 2);
                GUI.DrawTexture(new Rect(vRPMCapCenter.x, vRPMCapCenter.y, RPMCapSize.x, RPMCapSize.y), RPMCapTexture);
            }

        }
    }

    /// <summary>
    /// Refresh screen position after xml values reload
    /// </summary>
    public void RefreshScreenPosition()
    {
        if (GaugeDial == null) return;
        switch (GaugePosition)
        {
            case GaugePositionEnum.TopRight:
                _gaugePositionX = _screenWidth - GaugeSize.x - GaugePositionOffset.x;
                _gaugePositionY = 0;//_screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
            case GaugePositionEnum.TopLeft:
                _gaugePositionX = 0;//_screenWidth - GaugeSize.x - GaugePositionOffset.x;
                _gaugePositionY = 0;//_screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
            case GaugePositionEnum.TopCenter:
                _gaugePositionX = _screenWidth / 2 - GaugeSize.x / 2 - GaugePositionOffset.x;
                _gaugePositionY = 0;//_screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
            case GaugePositionEnum.BottomRight:
                _gaugePositionX = _screenWidth - GaugeSize.x - GaugePositionOffset.x;
                _gaugePositionY = _screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
            case GaugePositionEnum.BottomLeft:
                _gaugePositionX = 0;//_screenWidth - GaugeSize.x - GaugePositionOffset.x;
                _gaugePositionY = _screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
            case GaugePositionEnum.BottomCenter:
                _gaugePositionX = _screenWidth / 2 - GaugeSize.x / 2 - GaugePositionOffset.x;
                _gaugePositionY = _screenHeight - GaugeSize.y - GaugePositionOffset.y;
                break;
        }
    }
    /// <summary>
    /// Initiate loading while game running
    /// </summary>
    /// <param name="TemplateDataFile"></param>
    public void LoadUITeemplate(String TemplateDataFile)
    {
        loadJSONData();
        RefreshScreenPosition();
    }
    /// <summary>
    /// Loads the values from given json documents.
    /// </summary>
    public void loadJSONData()
    {
        if (ConfigurationAsset == null) return;
        string jsonContent = SpeedometerUIHelper.ReadJSONFile(ConfigurationAsset.name);

        JsonObjectForKit oJson = new JsonObjectForKit(jsonContent);
        if (oJson == null) return;

        if (oJson.GetField("SpeedometerUIData").GetField("GaugePosition").str == "")
        {
            Debug.LogWarning("SPEEDOMETER UI WARNING \r\nJSON File valid but its empty. Please save your settings before loading template.");
            return;
        }

        this.ControlName = oJson.GetField("SpeedometerUIData").GetField("ControlName").str;
        string[] sizeGauge = oJson.GetField("SpeedometerUIData").GetField("GaugeSize").str.Split(new char[] { ',' });
        this.GaugeSize = new Vector2(float.Parse(sizeGauge[0]), float.Parse(sizeGauge[1]));
        this.GaugePosition = (GaugePositionEnum)float.Parse(oJson.GetField("SpeedometerUIData").GetField("GaugePosition").str);
        string[] offGauge = oJson.GetField("SpeedometerUIData").GetField("GaugePositonOffset").str.Split(new char[] { ',' });
        this.GaugePositionOffset = new Vector2(float.Parse(offGauge[0]), float.Parse(offGauge[1]));
        string[] offSpeedoNeedle = oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleOffset").str.Split(new char[] { ',' });
        this.SpeedoNeedleOffset = new Vector2(float.Parse(offSpeedoNeedle[0]), float.Parse(offSpeedoNeedle[1]));
        string[] sizeSpeedoNeedle = oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleSize").str.Split(new char[] { ',' });
        this.SpeedoNeedleSize = new Vector2(float.Parse(sizeSpeedoNeedle[0]), float.Parse(sizeSpeedoNeedle[1]));
        this.SpeedoNeedleCenterDistance = float.Parse(oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleCenterDistance").str);

        string[] sizeSpeedoCap = oJson.GetField("SpeedometerUIData").GetField("SpeedoCapSize").str.Split(new char[] { ',' });
        this.SpeedoCapSize = new Vector2(float.Parse(sizeSpeedoCap[0]), float.Parse(sizeSpeedoCap[1]));

        this.SpeedoNeedleMinValueAngle = float.Parse(oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMinValueAngle").str);
        this.SpeedoNeedleMaxValueAngle = float.Parse(oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMaxValueAngle").str);
        this.SpeedoNeedleMaxValue = Convert.ToInt16(oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMaxValue").str);
        this.GaugePositionOffset = new Vector2(float.Parse(offGauge[0]), float.Parse(offGauge[1]));
        string[] offSpeedTextOffset = oJson.GetField("SpeedometerUIData").GetField("SpeedTextOffset").str.Split(new char[] { ',' });
        this.SpeedTextOffset = new Vector2(float.Parse(offSpeedTextOffset[0]), float.Parse(offSpeedTextOffset[1]));
        this.EnableRPM = Convert.ToBoolean(oJson.GetField("SpeedometerUIData").GetField("EnableRpm").str);
        string[] offRPMNeedle = oJson.GetField("SpeedometerUIData").GetField("RpmNeedleOffset").str.Split(new char[] { ',' });

        string[] sizeRPMNeedle = oJson.GetField("SpeedometerUIData").GetField("RPMNeedleSize").str.Split(new char[] { ',' });
        this.RPMNeedleSize = new Vector2(float.Parse(sizeRPMNeedle[0]), float.Parse(sizeRPMNeedle[1]));

        string[] sizeRPMCap = oJson.GetField("SpeedometerUIData").GetField("RpmCapSize").str.Split(new char[] { ',' });
        this.RPMCapSize = new Vector2(float.Parse(sizeRPMCap[0]), float.Parse(sizeRPMCap[1]));

        this.RPMNeedleOffset = new Vector2(float.Parse(offRPMNeedle[0]), float.Parse(offRPMNeedle[1]));
        this.RPMNeedleCenterDistance = float.Parse(oJson.GetField("SpeedometerUIData").GetField("RpmNeedleCenterDistance").str);
        this.RPMNeedleMinValueAngle = float.Parse(oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMinValueAngle").str);
        this.RPMNeedleMaxValueAngle = float.Parse(oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMaxValueAngle").str);
        this.RPMNeedleMaxValue = float.Parse(oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMaxValue").str);
        string[] offRPMText = oJson.GetField("SpeedometerUIData").GetField("RpmTextOffset").str.Split(new char[] { ',' });
        this.RPMTextOffset = new Vector2(float.Parse(offRPMText[0]), float.Parse(offRPMText[1]));
        string[] offGearText = oJson.GetField("SpeedometerUIData").GetField("GearTextOffset").str.Split(new char[] { ',' });
        this.GearTextOffset = new Vector2(float.Parse(offGearText[0]), float.Parse(offGearText[1]));
        this.ShowSpeedText = Convert.ToBoolean(oJson.GetField("SpeedometerUIData").GetField("ShowSpeedText").str);
        
        this.ShowSpeedUnitText= Convert.ToBoolean(oJson.GetField("SpeedometerUIData").GetField("ShowSpeedUnitText").str);
        this.SpeedUnitText = oJson.GetField("SpeedometerUIData").GetField("SpeedUnitText").str;
        string[] offSpeedUnitText = oJson.GetField("SpeedometerUIData").GetField("SpeedUnitTextOffset").str.Split(new char[] { ',' });
        this.SpeedUnitTextOffset = new Vector2(float.Parse(offSpeedUnitText[0]), float.Parse(offSpeedUnitText[1]));
        

        this.ShowRPMText = Convert.ToBoolean(oJson.GetField("SpeedometerUIData").GetField("ShowRpmText").str);
        this.ShowGearText = Convert.ToBoolean(oJson.GetField("SpeedometerUIData").GetField("ShowGearText").str);
        this.SpeedoSkin = (GUISkin)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("SpeedoSkin").str);

        this.GaugeDial = (Texture2D)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("GaugeDialTexture").str);
        if (oJson.GetField("SpeedometerUIData").GetField("SpeedoCapTexture").str != "") { this.SpeedoCapTexture = (Texture2D)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("SpeedoCapTexture").str); } else { this.SpeedoCapTexture = null; };
        if (oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleTexture").str != "") { this.SpeedoNeedleTexture = (Texture2D)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleTexture").str); } else { this.SpeedoNeedleTexture = null; };
        if (oJson.GetField("SpeedometerUIData").GetField("RpmCapTexture").str != "") { this.RPMCapTexture = (Texture2D)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("RpmCapTexture").str); } else { this.RPMCapTexture = null; };
        if (oJson.GetField("SpeedometerUIData").GetField("RpmNeedleTexture").str != "") { this.RPMNeedleTexture = (Texture2D)Resources.Load(oJson.GetField("SpeedometerUIData").GetField("RpmNeedleTexture").str); } else { this.RPMNeedleTexture = null; };

        RefreshScreenPosition();

    }
    /// <summary>
    /// Create a JSON Data for template save.
    /// </summary>
    /// <returns></returns>
    public JsonObjectForKit buildJSONData()
    {
        if (ConfigurationAsset == null) return null;

        string jsonContent = SpeedometerUIHelper.ReadJSONFile(ConfigurationAsset.name);

        JsonObjectForKit oJson = new JsonObjectForKit(jsonContent);
        if (oJson == null)
        {
            Debug.LogWarning("SPEEDOMETER UI WARNING \r\nImproper JSon File! Please use speedometerui-template.txt for creating your own configuration");
            return null;
        }


        oJson.GetField("SpeedometerUIData").GetField("ControlName").str = this.ControlName.ToString();
        oJson.GetField("SpeedometerUIData").GetField("GaugeSize").str = GaugeSize.x.ToString() + "," + GaugeSize.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("GaugePosition").str = Convert.ToInt16(this.GaugePosition).ToString();
        oJson.GetField("SpeedometerUIData").GetField("GaugePositonOffset").str = GaugePositionOffset.x.ToString() + "," + GaugePositionOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleOffset").str = SpeedoNeedleOffset.x.ToString() + "," + SpeedoNeedleOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleSize").str = SpeedoNeedleSize.x.ToString() + "," + SpeedoNeedleSize.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoCapSize").str = SpeedoCapSize.x.ToString() + "," + SpeedoCapSize.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleCenterDistance").str = SpeedoNeedleCenterDistance.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMinValueAngle").str = SpeedoNeedleMinValueAngle.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMaxValueAngle").str = SpeedoNeedleMaxValueAngle.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleMaxValue").str = SpeedoNeedleMaxValue.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedTextOffset").str = SpeedTextOffset.x.ToString() + "," + SpeedTextOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("EnableRpm").str = EnableRPM.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmNeedleOffset").str = RPMNeedleOffset.x.ToString() + "," + RPMNeedleOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmNeedleCenterDistance").str = RPMNeedleCenterDistance.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMinValueAngle").str = RPMNeedleMinValueAngle.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMaxValueAngle").str = RPMNeedleMaxValueAngle.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmNeedleMaxValue").str = RPMNeedleMaxValue.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmCapSize").str = RPMCapSize.x.ToString() + "," + RPMCapSize.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RPMNeedleSize").str = RPMNeedleSize.x.ToString() + "," + RPMNeedleSize.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("RpmTextOffset").str = RPMTextOffset.x.ToString() + "," + RPMTextOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("GearTextOffset").str = GearTextOffset.x.ToString() + "," + GearTextOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("ShowSpeedText").str = ShowSpeedText.ToString();
        oJson.GetField("SpeedometerUIData").GetField("ShowSpeedUnitText").str = ShowSpeedUnitText.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedUnitText").str = SpeedUnitText.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedUnitTextOffset").str = SpeedUnitTextOffset.x.ToString() + "," + SpeedUnitTextOffset.y.ToString();
        oJson.GetField("SpeedometerUIData").GetField("ShowRpmText").str = ShowRPMText.ToString();
        oJson.GetField("SpeedometerUIData").GetField("ShowGearText").str = ShowGearText.ToString();
        oJson.GetField("SpeedometerUIData").GetField("SpeedoSkin").str = SpeedoSkin.name.ToString();

        oJson.GetField("SpeedometerUIData").GetField("GaugeDialTexture").str = GaugeDial.name.ToString();
        
        if (SpeedoCapTexture != null) { oJson.GetField("SpeedometerUIData").GetField("SpeedoCapTexture").str = SpeedoCapTexture.name.ToString(); }
        else { oJson.GetField("SpeedometerUIData").GetField("SpeedoCapTexture").str = ""; }

        if (SpeedoNeedleTexture != null) { oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleTexture").str = SpeedoNeedleTexture.name.ToString(); }
        else { oJson.GetField("SpeedometerUIData").GetField("SpeedoNeedleTexture").str = ""; }

        if (RPMCapTexture != null) { oJson.GetField("SpeedometerUIData").GetField("RpmCapTexture").str = RPMCapTexture.name.ToString(); }
        else { oJson.GetField("SpeedometerUIData").GetField("RpmCapTexture").str = ""; }

        if (RPMNeedleTexture != null) { oJson.GetField("SpeedometerUIData").GetField("RpmNeedleTexture").str = RPMNeedleTexture.name.ToString(); }
        else { oJson.GetField("SpeedometerUIData").GetField("RpmNeedleTexture").str = "";}


        return oJson;
    }
}