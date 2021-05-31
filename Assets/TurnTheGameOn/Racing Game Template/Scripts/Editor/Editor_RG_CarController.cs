using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UnityStandardAssets.Vehicles.Car {

    [CustomEditor(typeof(UnityStandardAssets.Vehicles.Car.CarController))]
    public class Editor_RG_CarController : Editor
    {

        JointSpring frjs;
        JointSpring fljs;
        JointSpring rrjs;
        JointSpring rljs;
        WheelFrictionCurve frwfc;
        WheelFrictionCurve flwfc;
        WheelFrictionCurve rrwfc;
        WheelFrictionCurve rlwfc;
        WheelFrictionCurve frswfc;
        WheelFrictionCurve flswfc;
        WheelFrictionCurve rrswfc;
        WheelFrictionCurve rlswfc;

        public override void OnInspectorGUI()
        {
            
              

            //RG_CarController rg_carController = (RG_CarController)target;
            UnityStandardAssets.Vehicles.Car.CarController carController = (CarController)target;

            //     Texture racingAssetTexture = Resources.Load("RacingAssetTexture") as Texture;
            //
            //     GUIStyle inspectorStyle = new GUIStyle(GUI.skin.label);
            //     inspectorStyle.fixedWidth = 256;
            //     inspectorStyle.fixedHeight = 64;
            //     inspectorStyle.margin = new RectOffset((int)(((Screen.width * 0.98f) - 264) / 2), 0, 0, 0);
            //GUILayout.Label(racingAssetTexture, inspectorStyle);

            EditorGUILayout.BeginVertical("Box");

            ///Car Max Steer Angle
            SerializedProperty maxSteerAngle = serializedObject.FindProperty("m_MaximumSteerAngle");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(maxSteerAngle, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Max Steer Angle Top Speed
            SerializedProperty maxSteerAngleTopSpeed = serializedObject.FindProperty("m_SteerAngleAtMaxSpeed");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(maxSteerAngleTopSpeed, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Steer Helper
            SerializedProperty steerHelper = serializedObject.FindProperty("m_SteerHelper");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(steerHelper, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginVertical("Box");
            ///Car Traction Control
            SerializedProperty tractionControl = serializedObject.FindProperty("m_TractionControl");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tractionControl, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level " + carController.levelTireTraction.ToString(), GUILayout.MaxWidth(Screen.width * 0.14f));
            SerializedProperty levelBonusTireTraction = serializedObject.FindProperty("levelBonusTireTraction");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(levelBonusTireTraction, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("Box");

            ///Car Steer Sensetivity
            SerializedProperty steerSensitivity = serializedObject.FindProperty("m_SteerSensitivity");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(steerSensitivity, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Steer Sensetivity
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level " + carController.levelSteerSensitivity.ToString(), GUILayout.MaxWidth(Screen.width * 0.14f));
            SerializedProperty levelBonusSteerSensitivity = serializedObject.FindProperty("levelBonusSteerSensitivity");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(levelBonusSteerSensitivity, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            ///
            ///         TOP SPEED
            ///
            EditorGUILayout.BeginVertical("Box");
           
                ///Car Top Speed
                SerializedProperty topSpeed = serializedObject.FindProperty("m_Topspeed");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(topSpeed, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                ///Car Level Bonus Top Speed
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level " + carController.levelTopSpeed.ToString(), GUILayout.MaxWidth(Screen.width * 0.14f));
                SerializedProperty levelBonusTopSpeed = serializedObject.FindProperty("levelBonusTopSpeed");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(levelBonusTopSpeed, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            ///
            ///         TORQUE / ACCELERATION
            ///
            EditorGUILayout.BeginVertical("Box");

                ///Car Torque
                SerializedProperty torque = serializedObject.FindProperty("m_FullTorqueOverAllWheels");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(torque, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                ///Car Level Bonus Acceleration - Torque is used as a variable for increasing acceleration rate.
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level " + carController.levelAcceleration.ToString(), GUILayout.MaxWidth(Screen.width * 0.14f));
                SerializedProperty levelBonusAcceleration = serializedObject.FindProperty("levelBonusAcceleration");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(levelBonusAcceleration, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            ///
            ///         BRAKE TORQUE / BRAKE POWER
            ///
            EditorGUILayout.BeginVertical("Box");

                SerializedProperty brakeTorque = serializedObject.FindProperty("m_BrakeTorque");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(brakeTorque, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level " + carController.levelBrakePower.ToString(), GUILayout.MaxWidth(Screen.width * 0.14f));
                SerializedProperty levelBonusBrakePower = serializedObject.FindProperty("levelBonusBrakePower");
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(levelBonusBrakePower, true);
                if (EditorGUI.EndChangeCheck())
                    serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            ///
            ///         Car Reverse Torque
            ///
            SerializedProperty reverseTorque = serializedObject.FindProperty("m_ReverseTorque");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(reverseTorque, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            ///
            ///         Car Downforce
            /// 
            SerializedProperty downforce = serializedObject.FindProperty("m_Downforce");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(downforce, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Rev Range Boundary
            SerializedProperty revRangeBoundary = serializedObject.FindProperty("m_RevRangeBoundary");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(revRangeBoundary, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Slip Limit
            SerializedProperty slipLimit = serializedObject.FindProperty("m_SlipLimit");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(slipLimit, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Centre of Mass Offset
            SerializedProperty comOffset = serializedObject.FindProperty("m_CentreOfMassOffset");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(comOffset, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Drive Type
            SerializedProperty carDriveType = serializedObject.FindProperty("m_CarDriveType");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(carDriveType, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Speed Type
            SerializedProperty speedType = serializedObject.FindProperty("m_SpeedType");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(speedType, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical("Box");
            ///Car Nitro Top Speed Limit Increase
            SerializedProperty nitroTopSpeed = serializedObject.FindProperty("nitroTopSpeed");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroTopSpeed, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro Full Torque Limit Increase
            SerializedProperty nitroFullTorque = serializedObject.FindProperty("nitroFullTorque");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroFullTorque, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro Duration
            SerializedProperty nitroDuration = serializedObject.FindProperty("nitroDuration");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroDuration, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro Spend Rate
            SerializedProperty nitroSpendRate = serializedObject.FindProperty("nitroSpendRate");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroSpendRate, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro Refill Rate
            SerializedProperty nitroRefillRate = serializedObject.FindProperty("nitroRefillRate");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroRefillRate, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro ON Event Trigger
            SerializedProperty nitroON = serializedObject.FindProperty("nitroON");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroON, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro OFF Event Trigger
            SerializedProperty nitroOFF = serializedObject.FindProperty("nitroOFF");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroOFF, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            ///Car Nitro OFF Event Trigger
            SerializedProperty nitroFX = serializedObject.FindProperty("nitroFX");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(nitroFX, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();



            



            EditorGUILayout.BeginVertical("Box");
            SerializedProperty wheelColliders = serializedObject.FindProperty("m_WheelColliders");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wheelColliders, true, GUILayout.MaxWidth(Screen.width * 0.9f));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.MaxWidth(Screen.width * 0.29f));
            EditorGUILayout.LabelField("FR", GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.LabelField("FL", GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.LabelField("RR", GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.LabelField("RL", GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mass", GUILayout.MaxWidth(Screen.width * 0.25f));
            carController.m_WheelColliders[0].mass = EditorGUILayout.FloatField(carController.m_WheelColliders[0].mass, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[1].mass = EditorGUILayout.FloatField(carController.m_WheelColliders[1].mass, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[2].mass = EditorGUILayout.FloatField(carController.m_WheelColliders[2].mass, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[3].mass = EditorGUILayout.FloatField(carController.m_WheelColliders[3].mass, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Radius", GUILayout.MaxWidth(Screen.width * 0.25f));
            carController.m_WheelColliders[0].radius = EditorGUILayout.FloatField(carController.m_WheelColliders[0].radius, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[1].radius = EditorGUILayout.FloatField(carController.m_WheelColliders[1].radius, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[2].radius = EditorGUILayout.FloatField(carController.m_WheelColliders[2].radius, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[3].radius = EditorGUILayout.FloatField(carController.m_WheelColliders[3].radius, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Wheel Damping Rate", GUILayout.MaxWidth(Screen.width * 0.25f));
            carController.m_WheelColliders[0].wheelDampingRate = EditorGUILayout.FloatField(carController.m_WheelColliders[0].wheelDampingRate, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[1].wheelDampingRate = EditorGUILayout.FloatField(carController.m_WheelColliders[1].wheelDampingRate, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[2].wheelDampingRate = EditorGUILayout.FloatField(carController.m_WheelColliders[2].wheelDampingRate, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[3].wheelDampingRate = EditorGUILayout.FloatField(carController.m_WheelColliders[3].wheelDampingRate, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Suspension Distance", GUILayout.MaxWidth(Screen.width * 0.25f));
            carController.m_WheelColliders[0].suspensionDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[0].suspensionDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[1].suspensionDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[1].suspensionDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[2].suspensionDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[2].suspensionDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[3].suspensionDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[3].suspensionDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Force App Point Distance", GUILayout.MaxWidth(Screen.width * 0.25f));
            carController.m_WheelColliders[0].forceAppPointDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[0].forceAppPointDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[1].forceAppPointDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[1].forceAppPointDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[2].forceAppPointDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[2].forceAppPointDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            carController.m_WheelColliders[3].forceAppPointDistance = EditorGUILayout.FloatField(carController.m_WheelColliders[3].forceAppPointDistance, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Suspension Spring");
            frjs = carController.m_WheelColliders[0].suspensionSpring;
            fljs = carController.m_WheelColliders[1].suspensionSpring;
            rrjs = carController.m_WheelColliders[2].suspensionSpring;
            rljs = carController.m_WheelColliders[3].suspensionSpring;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spring", GUILayout.MaxWidth(Screen.width * 0.25f));            
            frjs.spring = EditorGUILayout.FloatField(frjs.spring, GUILayout.MaxWidth(Screen.width * 0.14f));            
            fljs.spring = EditorGUILayout.FloatField(fljs.spring, GUILayout.MaxWidth(Screen.width * 0.14f));            
            rrjs.spring = EditorGUILayout.FloatField(rrjs.spring, GUILayout.MaxWidth(Screen.width * 0.14f));            
            rljs.spring = EditorGUILayout.FloatField(rljs.spring, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damper", GUILayout.MaxWidth(Screen.width * 0.25f));
            frjs.damper = EditorGUILayout.FloatField(frjs.damper, GUILayout.MaxWidth(Screen.width * 0.14f));
            fljs.damper = EditorGUILayout.FloatField(fljs.damper, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrjs.damper = EditorGUILayout.FloatField(rrjs.damper, GUILayout.MaxWidth(Screen.width * 0.14f));
            rljs.damper = EditorGUILayout.FloatField(rljs.damper, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Target Position", GUILayout.MaxWidth(Screen.width * 0.25f));
            frjs.targetPosition = EditorGUILayout.FloatField(frjs.targetPosition, GUILayout.MaxWidth(Screen.width * 0.14f));
            fljs.targetPosition = EditorGUILayout.FloatField(fljs.targetPosition, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrjs.targetPosition = EditorGUILayout.FloatField(rrjs.targetPosition, GUILayout.MaxWidth(Screen.width * 0.14f));
            rljs.targetPosition = EditorGUILayout.FloatField(rljs.targetPosition, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();            
            carController.m_WheelColliders[0].suspensionSpring = frjs;
            carController.m_WheelColliders[1].suspensionSpring = fljs;
            carController.m_WheelColliders[2].suspensionSpring = rrjs;
            carController.m_WheelColliders[3].suspensionSpring = rljs;
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Forward Friction");
            frwfc = carController.m_WheelColliders[0].forwardFriction;
            flwfc = carController.m_WheelColliders[1].forwardFriction;
            rrwfc = carController.m_WheelColliders[2].forwardFriction;
            rlwfc = carController.m_WheelColliders[3].forwardFriction;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extremum Slip", GUILayout.MaxWidth(Screen.width * 0.25f));            
            frwfc.extremumSlip = EditorGUILayout.FloatField(frwfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));            
            flwfc.extremumSlip = EditorGUILayout.FloatField(flwfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));            
            rrwfc.extremumSlip = EditorGUILayout.FloatField(rrwfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));            
            rlwfc.extremumSlip = EditorGUILayout.FloatField(rlwfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extremum Value", GUILayout.MaxWidth(Screen.width * 0.25f));
            frwfc.extremumValue = EditorGUILayout.FloatField(frwfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            flwfc.extremumValue = EditorGUILayout.FloatField(flwfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrwfc.extremumValue = EditorGUILayout.FloatField(rrwfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlwfc.extremumValue = EditorGUILayout.FloatField(rlwfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asmptote Slip", GUILayout.MaxWidth(Screen.width * 0.25f));
            frwfc.asymptoteSlip = EditorGUILayout.FloatField(frwfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            flwfc.asymptoteSlip = EditorGUILayout.FloatField(flwfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrwfc.asymptoteSlip = EditorGUILayout.FloatField(rrwfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlwfc.asymptoteSlip = EditorGUILayout.FloatField(rlwfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asymptote Value", GUILayout.MaxWidth(Screen.width * 0.25f));
            frwfc.asymptoteValue = EditorGUILayout.FloatField(frwfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            flwfc.asymptoteValue = EditorGUILayout.FloatField(flwfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrwfc.asymptoteValue = EditorGUILayout.FloatField(rrwfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlwfc.asymptoteValue = EditorGUILayout.FloatField(rlwfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stiffness", GUILayout.MaxWidth(Screen.width * 0.25f));
            frwfc.stiffness = EditorGUILayout.FloatField(frwfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            flwfc.stiffness = EditorGUILayout.FloatField(flwfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrwfc.stiffness = EditorGUILayout.FloatField(rrwfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlwfc.stiffness = EditorGUILayout.FloatField(rlwfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            carController.m_WheelColliders[0].forwardFriction = frwfc;
            carController.m_WheelColliders[1].forwardFriction = flwfc;
            carController.m_WheelColliders[2].forwardFriction = rrwfc;
            carController.m_WheelColliders[3].forwardFriction = rlwfc;
            EditorGUILayout.EndVertical();



            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField("Sidways Friction");
            frswfc = carController.m_WheelColliders[0].sidewaysFriction;
            flswfc = carController.m_WheelColliders[1].sidewaysFriction;
            rrswfc = carController.m_WheelColliders[2].sidewaysFriction;
            rlswfc = carController.m_WheelColliders[3].sidewaysFriction;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extremum Slip", GUILayout.MaxWidth(Screen.width * 0.25f));
            frswfc.extremumSlip = EditorGUILayout.FloatField(frswfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            flswfc.extremumSlip = EditorGUILayout.FloatField(flswfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrswfc.extremumSlip = EditorGUILayout.FloatField(rrswfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlswfc.extremumSlip = EditorGUILayout.FloatField(rlswfc.extremumSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Extremum Value", GUILayout.MaxWidth(Screen.width * 0.25f));
            frswfc.extremumValue = EditorGUILayout.FloatField(frswfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            flswfc.extremumValue = EditorGUILayout.FloatField(flswfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrswfc.extremumValue = EditorGUILayout.FloatField(rrswfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlswfc.extremumValue = EditorGUILayout.FloatField(rlswfc.extremumValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asmptote Slip", GUILayout.MaxWidth(Screen.width * 0.25f));
            frswfc.asymptoteSlip = EditorGUILayout.FloatField(frswfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            flswfc.asymptoteSlip = EditorGUILayout.FloatField(flswfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrswfc.asymptoteSlip = EditorGUILayout.FloatField(rrswfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlswfc.asymptoteSlip = EditorGUILayout.FloatField(rlswfc.asymptoteSlip, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asymptote Value", GUILayout.MaxWidth(Screen.width * 0.25f));
            frswfc.asymptoteValue = EditorGUILayout.FloatField(frswfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            flswfc.asymptoteValue = EditorGUILayout.FloatField(flswfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrswfc.asymptoteValue = EditorGUILayout.FloatField(rrswfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlswfc.asymptoteValue = EditorGUILayout.FloatField(rlswfc.asymptoteValue, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Stiffness", GUILayout.MaxWidth(Screen.width * 0.25f));
            frswfc.stiffness = EditorGUILayout.FloatField(frswfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            flswfc.stiffness = EditorGUILayout.FloatField(flswfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            rrswfc.stiffness = EditorGUILayout.FloatField(rrswfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            rlswfc.stiffness = EditorGUILayout.FloatField(rlswfc.stiffness, GUILayout.MaxWidth(Screen.width * 0.14f));
            EditorGUILayout.EndHorizontal();
            carController.m_WheelColliders[0].sidewaysFriction = frswfc;
            carController.m_WheelColliders[1].sidewaysFriction = flswfc;
            carController.m_WheelColliders[2].sidewaysFriction = rrswfc;
            carController.m_WheelColliders[3].sidewaysFriction = rlswfc;
            EditorGUILayout.EndVertical();
            

            SerializedProperty wheelMeshes = serializedObject.FindProperty("m_WheelMeshes");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wheelMeshes, true, GUILayout.MaxWidth(Screen.width * 0.9f));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            SerializedProperty wheelEffects = serializedObject.FindProperty("m_WheelEffects");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(wheelEffects, true, GUILayout.MaxWidth(Screen.width * 0.9f));
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();
            
        }


    }

}