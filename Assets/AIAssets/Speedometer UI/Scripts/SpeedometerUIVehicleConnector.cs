using UnityEngine;
using System.Collections;

//UNCOMMENT FOR RGKCAR
using RacingGameKit;
using RacingGameKit.RGKCar;

[AddComponentMenu("Racing Game Kit/Speedometer UI/Speedometer UI - Vehicle Connector"), ExecuteInEditMode]
public class SpeedometerUIVehicleConnector : MonoBehaviour
{
    SpeedometerUI SpeedometerUIComponent;
    
    //UNCOMMENT FOR RGKCAR
    RGKCar_Engine RGKEngine = null;


    //UNCOMMENT FOR UNITYCAR
    //Drivetrain UnityCarDriveTrain = null;


    void Start()
    {
        //FIND THE SPEEDOMETER COMPONENT 
        SpeedometerUIComponent = base.GetComponent<SpeedometerUI>();


        #region "RGKCAR SPEEDO CONNECTION"
        ////UNCOMMENT FOR RGKCAR
        RGKEngine = base.GetComponent<RGKCar_Engine>() as RGKCar_Engine;
        #endregion

        #region "UNITYCAR SPEEDO CONNECTION"
        ////UNCOMMENT FOR UNITYCAR
        //UnityCarDriveTrain = base.GetComponent<Drivetrain>() as Drivetrain;
        #endregion
    }

    void Update()
    {
        #region "RGKCAR SPEEDO CONNECTION"
        //// UNCOMMENT FOR RGKCAR
        if (RGKEngine!=null){
            if (SpeedometerUIComponent != null)
            {
                SpeedometerUIComponent.Speed = RGKEngine.SpeedAsKM;
                SpeedometerUIComponent.RPM = RGKEngine.RPM;
                string Gear = RGKEngine.Gear.ToString();
                if (Gear == "-1") Gear = "R";
                if (Gear == "0") Gear = "N";
                SpeedometerUIComponent.Gear = Gear;
            }
        }
        ////RGKCAR SPEEDO END
        #endregion

        #region "UNITYCAR SPEEDO CONNECTION"
        //UNCOMMENT FOR UNITYCAR
        //if (UnityCarDriveTrain)
        //{
        //    if (SpeedometerUIComponent != null)
        //    {
        //        SpeedometerUIComponent.Speed = float.Parse((UnityCarDriveTrain.velo*3.6).ToString()); // This conversation required because UnityCar scripts lack of vehicle speed property..
        //        SpeedometerUIComponent.RPM = UnityCarDriveTrain.rpm;
        //        string Gear = UnityCarDriveTrain.gear.ToString();
        //        if (Gear == "-1") Gear = "R";
        //        if (Gear == "0") Gear = "N";
        //        SpeedometerUIComponent.Gear = Gear;                
        //    }
        //}
        #endregion

    }

}

