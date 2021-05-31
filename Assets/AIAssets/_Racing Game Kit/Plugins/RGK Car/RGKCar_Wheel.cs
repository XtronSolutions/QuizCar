//============================================================================================
// Racing Game Kit v1.0
// http://www.unityracingkit.com
// by Yusuf AKDAG - http://www.yusufakdag.com
// Vehicle Script
// Last Change : 10/01/2012
// You can use freely on commercial or other projects. You cant modify and resell.
//============================================================================================


using System;
using UnityEngine;
using RacingGameKit.Interfaces;
using RacingGameKit.RGKCar;

namespace RacingGameKit.RGKCar
{

    // This class simulates a single car's wheel with tire, brake and simple
    // suspension (basically just a single, independant spring and damper).
    [AddComponentMenu("Racing Game Kit/RGKCar/RGKCar Wheel")]
    public class RGKCar_Wheel : MonoBehaviour
    {
        public GameObject WheelModel;
        public GameObject WheelBlurModel;
        public GameObject DiskModel;
        public Vector3 DiskModelOffset;
        public GameObject CaliperModel;
        public Vector3 CaliperModelOffset;
		float heightOffset = 0.23f;
        public WheelLocationEnum WheelLocation;
        public bool isPowered = false;
        //[HideInInspector]
        public float suspensionHeight = 0.2f; // Wheel suspension travel in meters
        [HideInInspector]
        public float suspensionStiffness = 5000;// Damper strength in kg/s
        [HideInInspector]
        public float suspensionRelease = 50f; // damper release time
        [HideInInspector]
        public float brakeTorque = 4000; // Maximal braking torque (in Nm)
        [HideInInspector]
        public float handbrakeTorque = 0;// Maximal handbrake torque (in Nm)
        public float definedGrip = 1.0f;
        public float wheelGrip = 1.0f;
        public float definedSideGrip = 1.0f;
        public float sideGrip = 1.0f;
        public float maxSteeringAngle = 0f;

        [HideInInspector]
        public float slipVeloLimiter = 20f;
        [HideInInspector]
        public float radius = 0.34f;
        [HideInInspector]
        public float blurSwitchVelocity = 20f;
        [HideInInspector]
        public bool ShowForces = false;
        [HideInInspector]
        public bool BurnoutStart = true;
        [HideInInspector]
        public float BurnoutStartDuration = 1f;

        // Wheel angular inertia in kg * m^2
        public float inertia = 2.2f;

        // Base friction torque (in Nm)
        public float frictionTorque = 10;

        // Fraction of the car's mass carried by this wheel
        public float massFraction = 0.25f;

        // Pacejka coefficients
        //private float[] a = { 1.0f, -60f, 1688f, 4140f, 6.026f, 0f, -0.3589f, 1f, 0f, -6.111f / 1000f, -3.244f / 100f, 0f, 0f, 0f, 0f };
        //private float[] b = { 1.0f, -60f, 1588f, 0f, 229f, 0f, 0f, 0f, -10f, 0f, 0f };
        float[] a = { 1.5f, -40f, 1600f, 2600f, 8.7f, 0.014f, -0.24f, 1.0f, -0.03f, -0.0013f, -0.06f, -8.5f, -0.29f, 17.8f, -2.4f };
        float[] b = { 1.5f, -80f, 1950f, 23.3f, 390f, 0.05f, 0f, 0.055f, -0.024f, 0.014f, 0.26f };
        // inputs
        // engine torque applied to this wheel
        [HideInInspector]
        public float driveTorque = 0;
        // engine braking and other drivetrain friction torques applied to this wheel
        [HideInInspector]
        public float driveFrictionTorque = 0;
        // brake input
        [HideInInspector]
        public float brake = 0;
        // handbrake input
        [HideInInspector]
        public float handbrake = 0;
        // steering input
        [HideInInspector]
        public float steering = 0;
        // drivetrain inertia as currently connected to this wheel
        [HideInInspector]
        public float drivetrainInertia = 0;
        // suspension force externally applied (by anti-roll bars)
        [HideInInspector]
        public float suspensionForceInput = 0;

        // output

        public float angularVelocity;
        public float oAngularVelocity;
        public float slipRatio;
        public float slipVelo;
        public float compression;

        // state
        float fullCompressionSpringForce;
        public Vector3 wheelVelo;
        public Vector3 localVelo;
        Vector3 groundNormal;

        float rotation;
        float normalForce;
        Vector3 suspensionForce;
        public Vector3 roadForce;
        Vector3 up, right, forwardNormal, rightNormal;//,forward;
        Quaternion localRotation = Quaternion.identity;
        Quaternion inverseLocalRotation = Quaternion.identity;
        public float slipAngle;
        private int lastSkid = -1;
        private int lastTrail = -1;

        // cached values
        Rigidbody body;
        private float maxSlip;
        private float maxAngle;
        private float oldAngle;

        public float totalFrictionTorque;
        public int slipRes;

        private float longitunalSlipVelo;
        private float lateralSlipVelo;

        float groundCalc;
        private Vector3 force;

        public bool onGround;


        private string previousSurface = "";
        private bool surfaceChanged = false;

        LineRenderer suspensionLineRenderer;




        //------------- EARTHFX --------------
        [HideInInspector]
        public bool UseEarthFX = false;

        public GameObject SkidmarkObject;
        private RGKCar_Skidmarks Skids;
        private RGKCar_Skidmarks Trails;
        private ParticleSystem SkidSmoke;
        private ParticleSystem TrailSmoke;
        private ParticleSystem Splatter;

        [HideInInspector]
        public EarthFX EarthFX;
        [HideInInspector]
        public AudioClip SurfaceSound;
        [HideInInspector]
        public AudioClip BrakeSound;
        [HideInInspector]
        public float SurfaceForwardDrag;
        [HideInInspector]
        public float SurfaceAngularDrag;

        //------------- EARTHFX --------------


        void Awake()
        {
            suspensionLineRenderer = gameObject.AddComponent<LineRenderer>();
            suspensionLineRenderer.material = new Material(Shader.Find("Diffuse"));
            suspensionLineRenderer.material.color = Color.red;
            suspensionLineRenderer.SetWidth(0.01f, 0.1f);
            suspensionLineRenderer.useWorldSpace = false;
            suspensionLineRenderer.castShadows = false;
        }

        void Start()
        {


            Transform transformForRigidBody = transform;
            while (transformForRigidBody != null && transformForRigidBody.GetComponent<Rigidbody>() == null)
                transformForRigidBody = transformForRigidBody.parent;
            if (transformForRigidBody != null)
                body = transformForRigidBody.GetComponent<Rigidbody>();

            InitSlipMaxima();

            if (!UseEarthFX && SkidmarkObject != null)
            {
                Skids = SkidmarkObject.GetComponentInChildren(typeof(RGKCar_Skidmarks)) as RGKCar_Skidmarks;
                SkidSmoke = SkidmarkObject.GetComponentInChildren(typeof(ParticleSystem)) as ParticleSystem;
            }

            //			if (WheelLocation == WheelLocationEnum.Rear)
            //				heightOffset = 0.1f;

          //  InvokeRepeating("FixedUpdate2", 0.1f, 0.1f);
        }

		int frameCounter = 0;

       
        void FixedUpdate()
        {
           
            if (!UseEarthFX || EarthFX == null)
            {
                wheelGrip = definedGrip;
                sideGrip = definedSideGrip;
            }

            if (suspensionHeight < 0.05f) suspensionHeight = 0.05f;

            suspensionLineRenderer.enabled = ShowForces;

            Vector3 pos = transform.position;
            up = transform.up;

  
            RaycastHit hit;
  
                onGround = Physics.Raycast(pos, -up, out hit, suspensionHeight + radius);

                if (onGround && hit.collider.isTrigger)
                {
                    onGround = false; float dist = suspensionHeight + radius;
                    RaycastHit[] hits = Physics.RaycastAll(pos, -up, suspensionHeight + radius);
                    foreach (RaycastHit test in hits)
                    {
                        if (!test.collider.isTrigger && test.distance <= dist)
                        {
                            hit = test;
                            onGround = true;
                            dist = test.distance;
                        }
                    }
                }
  
           


            fullCompressionSpringForce = body.mass * massFraction * 2.0f * -Physics.gravity.y;
            float staticFrictionForce = wheelGrip * fullCompressionSpringForce;
            float latGravityForce = fullCompressionSpringForce * Mathf.Cos(Vector3.Angle(right, Vector3.up) * Mathf.Deg2Rad);

            if (latGravityForce < staticFrictionForce)
            {
                body.AddForceAtPosition(latGravityForce * -right, WheelModel.transform.position);

            }

            if (onGround)
            {
                groundNormal = transform.InverseTransformDirection(inverseLocalRotation * hit.normal);
                compression = 1.0f - ((hit.distance - radius) / suspensionHeight);

                suspensionForce = SuspensionForce();
                suspensionLineRenderer.SetPosition(1, new Vector3(0, 0.0005f * suspensionForce.y, 0));

                if (slipVelo < 30)
                {
                    wheelVelo = body.GetPointVelocity(pos);
                    localVelo = transform.InverseTransformDirection(inverseLocalRotation * wheelVelo);
                   

//					if(frameCounter == 2)
//					{
						roadForce = RoadForce();
//						frameCounter = 0;
//					}
//					else 
//					{
//						frameCounter++;
//					}


                    body.AddForceAtPosition(suspensionForce + roadForce, pos);
                }
                else
                {
                    angularVelocity = 0;
                    slipVelo = 0;
                }
            }
            else
            {
                if (slipVelo < 10)
                {
                    float totalInertia = inertia + drivetrainInertia;
                    float angularDelta = Time.deltaTime * invSlipRes / totalInertia;
                    float driveAngularDelta = driveTorque * angularDelta;
                    float totalFrictionTorque = brakeTorque * brake + handbrakeTorque * handbrake + frictionTorque + driveFrictionTorque;
                    float frictionAngularDelta = totalFrictionTorque * angularDelta;
                    angularVelocity += driveAngularDelta;

                    if (Mathf.Abs(angularVelocity) > frictionAngularDelta)
                    {
                        angularVelocity -= frictionAngularDelta * Mathf.Sign(angularVelocity);
                    }
                    else
                    { angularVelocity = 0; }
                }
                else
                {
                    //compression = 0.0f;
                    //suspensionForce = Vector3.zero;
                    //roadForce = Vector3.zero;
                    //angularVelocity = 0;
                    //slipRatio = 0;
                    //slipVelo = 0;
                }
            }
            /*
            if (onGround && UseEarthFX && EarthFX != null)
            {

                Texture2D hitTexture = null;
                string strTerrainName = "";
                if (Terrain.activeTerrain != null)
                {
                    strTerrainName = Terrain.activeTerrain.name;
                }

                if (hit.collider.gameObject.name == strTerrainName) // means hititng terrain
                {
                    int TextureIndex = SurfaceTextureDedector.GetMainTexture(WheelModel.transform.position);
                    hitTexture = Terrain.activeTerrain.terrainData.splatPrototypes[TextureIndex].texture;
                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                    {
                        if (hit.collider.gameObject.GetComponent<Renderer>().material.mainTexture != null)
                        {
                            hitTexture = (hit.collider.gameObject.GetComponent<Renderer>().material.mainTexture) as Texture2D;
                        }
                    }
                }
                if (hitTexture != null)
                {
                    if (previousSurface != hitTexture.name)
                    {
                        previousSurface = hitTexture.name;
                        surfaceChanged = true;
                        lastSkid = -1;
                    }
                }


                bool FXFound = false;
                foreach (EarthFX.EarthFXData FX in EarthFX.SurfaceFX)
                {
                    if (FX.FxTexture == hitTexture)
                    {
                        FXFound = true;
                        CastFX(FX, hit);
                    }

                    if (!FXFound)
                    {
                        CastFX(EarthFX.GlobalFX, hit);
                    }
                }

                if (EarthFX.SurfaceFX.Count == 0)
                {
                    CastFX(EarthFX.GlobalFX, hit);
                }

                if (surfaceChanged)
                {
                    surfaceChanged = false;
                }

            }
            else if (onGround && this.Skids != null && Mathf.Abs(this.slipRatio) > 0.15)
            {
                lastSkid = Skids.AddSkidMark(hit.point, hit.normal, Mathf.Abs(slipRatio) - 0f, lastSkid);

                if (this.SkidSmoke != null)
                {
//                    this.SkidSmoke.Emit(
//                        hit.point + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
//                            UnityEngine.Random.Range(-0.1f, 0.1f),
//                            UnityEngine.Random.Range(-0.1f, 0.1f)),
//                            new Vector3(this.slipVelo * 0.05f, 0),
//                            UnityEngine.Random.Range(SkidSmoke.minSize, SkidSmoke.maxSize) * Mathf.Clamp(0.5f, 1f, 1.5f),
//                            UnityEngine.Random.Range(SkidSmoke.minEnergy, SkidSmoke.maxEnergy), Color.white);
                }
            }
            else
            {
                lastSkid = -1;
                lastTrail = -1;
            }
            */
            compression = Mathf.Clamp01(compression);
            rotation += angularVelocity * Time.deltaTime;

            UpdateModels();

        }

        float CalcLongitudinalForce(float Fz, float slip)
        {
            Fz *= 0.001f;//convert to kN
            float FzSquared = Fz * Fz;
            slip *= 100f; //covert to %
            float FzFz = Fz * Fz;
            float uP = b[1] * Fz + b[2];
            //float D = uP * Fz;
            float D = (b[1] * FzFz + b[2] * Fz);
            //float B = ((b[3] * Fz + b[4]) * Mathf.Exp(-b[5] * Fz)) / (b[0] * uP);
            float B = ((b[3] * FzSquared + b[4] * Fz) * Mathf.Exp(-b[5] * Fz)) / (b[0] * D);

            float E = b[6] * FzSquared + b[7] * Fz + b[8];
            float Sh = b[9] * Fz + b[10];
            float Sv = 0;
            //float S = slip + b[9] * Fz + b[10];

            //float Fx = D * Mathf.Sin(b[0] * Mathf.Atan(S * B + E * (Mathf.Atan(S * B) - S * B)));
            float Fx = D * Mathf.Sin(b[0] * Mathf.Atan(B * (1.0f - E) * (slip + Sh) + E * Mathf.Atan(B * (slip + Sh)))) + Sv;
            return Fx;
        }
        public float camberAngle;

        float CalcLateralForce(float Fz, float slipAngle)
        {


            Fz *= 0.001f;//convert to kN
            float FzSquare = Fz * Fz;


            Vector3 wheelUp = transform.TransformDirection(Vector3.up);
            Vector3 carUp = transform.root.transform.TransformDirection(Vector3.up);
            camberAngle = Vector3.Angle(wheelUp, carUp);



            slipAngle *= (360f / (2 * Mathf.PI)); //convert angle to deg
            float uP = a[1] * Fz + a[2];
            //float D = uP * Fz;
            float D = (a[1] * FzSquare + a[2] * Fz) * sideGrip; /// SIDEWAYSGRIP

            //float B = (a[3] * Mathf.Sin(2 * Mathf.Atan(Fz / a[4]))) / (a[0] * peakFrictionCoeff * Fz);
            //      B=(a3*sinf(2*atanf(Fz/a4))*(1-a5*fabs(camber)))/(C*D);                                                        

            float B = (a[3] * Mathf.Sin(2 * Mathf.Atan(Fz / a[4])) * (1 - a[5] * Mathf.Abs(camberAngle))) / (a[0] * D);
            float S = slipAngle + a[9] * Fz + a[10];

            float E = a[6] * Fz + a[7];
            //float Sv = a[12] * Fz + a[13];
            //float Sv = a[12] * Fz + a[13] * -a[11] * Fz;
            float Sv = (a[11] * Fz + a[12]) * camberAngle * Fz + a[12] * Fz + a[13];
            //Sv = (a111 * Fz + a112) * camber * Fz + a12 * Fz + a13;
            float Fy = D * Mathf.Sin(a[0] * Mathf.Atan(S * B + E * (Mathf.Atan(S * B) - S * B))) + Sv;
            return Fy;

            //float slipAngle = CalcSlipAngle(steerangle);

            // Camber angle, this probably could be 0
            //float camberAngle = 0f;
            //Vector3 wheelUp = transform.TransformDirection(Vector3.up);
            //Vector3 carUp = transform.root.transform.TransformDirection(Vector3.up);
            //camberAngle = Vector3.Angle(wheelUp, carUp);

            //float Fzx = ((Fz) * 0.01f); // Load as KiloNewtons (multiply by .25 because we assume each tire supports 1/4 of the car)
            //float peakFrictionCoeff = a[1] * Fzx + a[2];

            //float D = peakFrictionCoeff * Fzx; // Peak value of curve in Newtons
            //float S = slipAngle + a[8] * camberAngle + a[9] * Fzx + a[10];
            //float B = (a[3] * Mathf.Sin(2 * Mathf.Atan(Fzx / a[4])) * (1 - a[5] * Mathf.Abs(camberAngle))) / (a[0] * peakFrictionCoeff * Fzx);
            //float E = a[6] * Fzx + a[7];
            //float Sv = ((a[11] * Fzx) * camberAngle + a[12]) * Fzx + a[13];

            //float lateralForce = D * Mathf.Sin(a[0] * Mathf.Atan(S * B + E * Mathf.Atan(S * B) - S * B)) + Sv;

            //return lateralForce;



        }

        float CalcLongitudinalForceUnit(float Fz, float slip)
        {
            return CalcLongitudinalForce(Fz, slip * maxSlip);
        }

        float CalcLateralForceUnit(float Fz, float slipAngle)
        {
            return CalcLateralForce(Fz, slipAngle * maxAngle);
        }

        Vector3 CombinedForce(float Fz, float slip, float slipAngle)
        {
            float unitSlip = slip / maxSlip;
            float unitAngle = slipAngle / maxAngle;
            float p = Mathf.Sqrt(unitSlip * unitSlip + unitAngle * unitAngle);
            if (p <= Mathf.Epsilon)
            {
                return Vector3.zero;
            }
            else
            {
                if (slip < -0.8f)
                    return -localVelo.normalized * (Mathf.Abs(unitAngle / p * CalcLateralForceUnit(Fz, p)) + Mathf.Abs(unitSlip / p * CalcLongitudinalForceUnit(Fz, p)));
            }

            Vector3 forward = new Vector3(0, -groundNormal.z, groundNormal.y);
            return Vector3.right * unitAngle / p * CalcLateralForceUnit(Fz, p) + forward * unitSlip / p * CalcLongitudinalForceUnit(Fz, p);
        }

        void InitSlipMaxima()
        {
            const float stepSize = 0.001f;
            const float testNormalForce = 4000f;
            float force = 0;
            for (float slip = stepSize; ; slip += stepSize)
            {
                float newForce = CalcLongitudinalForce(testNormalForce, slip);
                if (force < newForce)
                    force = newForce;
                else
                {
                    maxSlip = slip - stepSize;
                    break;
                }
            }
            force = 0;
            for (float slipAngle = stepSize; ; slipAngle += stepSize)
            {
                float newForce = CalcLateralForce(testNormalForce, slipAngle);
                if (force < newForce)
                    force = newForce;
                else
                {
                    maxAngle = slipAngle - stepSize;
                    break;
                }
            }
        }

        Vector3 SuspensionForce()
        {
            float springForce;
            if (suspensionRelease > 0)
            {
                springForce = compression * fullCompressionSpringForce * Time.deltaTime * suspensionRelease;
            }
            else
            {
                springForce = compression * fullCompressionSpringForce;
            }
            normalForce = springForce;

            float damperForce = Vector3.Dot(localVelo, groundNormal) * suspensionStiffness;

            return (springForce - damperForce + suspensionForceInput) * up;
        }



        float SlipRatio()
        {
            //const float fullSlipVelo = 4f;
            //float slipRatio = 0;
            //float wheelRoadVelo = Vector3.Dot(wheelVelo, forwardNormal);
            //if (wheelRoadVelo == 0)
            //    return 0;

            //float absRoadVelo = Mathf.Abs(wheelRoadVelo);
            //float damping = Mathf.Clamp01(absRoadVelo / fullSlipVelo);

            //float wheelTireVelo = angularVelocity * radius;
            //slipRatio = ((wheelTireVelo - wheelRoadVelo) / absRoadVelo) * damping;

            //return slipRatio;

            const float fullSlipVelo = 4f;
            //if (transform.name.Contains("FL"))
            //{
            //    Debug.Log(transform.name + "" + wheelVelo);
            //}
            float wheelRoadVelo = Vector3.Dot(wheelVelo, forwardNormal);
            if (wheelRoadVelo == 0)
                return 0;

            float absRoadVelo = Mathf.Abs(wheelRoadVelo);
            float damping = Mathf.Clamp01(absRoadVelo / fullSlipVelo);

            float wheelTireVelo = angularVelocity * radius;
            slipRatio = ((wheelTireVelo - wheelRoadVelo) / absRoadVelo) * damping;
            //Debug.Log(transform.name + " " + wheelRoadVelo + " " + damping + " " + wheelTireVelo);
            return slipRatio;


        }

        float SlipAngle()
        {
            Vector3 localVelo = this.localVelo;
            localVelo.y = 0f;
            if (localVelo.sqrMagnitude < float.Epsilon)
            {
                return 0f;
            }
            float x = localVelo.normalized.x;
            Mathf.Clamp(x, -1f, 1f);
            float t = Mathf.Clamp01(this.localVelo.magnitude / 2f);
            return ((-Mathf.Asin(x) * t) * t);
        }

        public float invSlipRes;
        private float slipFactor = 10f;

        Vector3 RoadForce()
        {
            //slipRes = (int)((100.0f - Mathf.Abs(angularVelocity)) / (5f));
            slipRes = (int)((100.0f - Mathf.Abs(angularVelocity)) / (slipFactor));
            if (slipRes < 1)
                slipRes = 1;

            invSlipRes = (1f / (float)slipRes);

            float totalInertia = inertia + drivetrainInertia;
            float driveAngularDelta = driveTorque * Time.deltaTime * invSlipRes / totalInertia;
            totalFrictionTorque = brakeTorque * brake + handbrakeTorque * handbrake + frictionTorque + driveFrictionTorque / 2;
            float frictionAngularDelta = totalFrictionTorque * Time.deltaTime * invSlipRes / totalInertia;

            Vector3 totalForce = Vector3.zero;
            float newAngle = maxSteeringAngle * steering;

            for (int i = 0; i < slipRes; i++)
            {
                float f = i * 1.0f / (float)slipRes;
                localRotation = Quaternion.Euler(0, oldAngle + (newAngle - oldAngle) * f, 0);
                inverseLocalRotation = Quaternion.Inverse(localRotation);
                forwardNormal = transform.TransformDirection(localRotation * Vector3.forward);
                right = transform.TransformDirection(localRotation * Vector3.right);

                groundCalc = Vector3.Dot(right, groundNormal);
                rightNormal = right - groundNormal * groundCalc;
                forwardNormal = Vector3.Cross(rightNormal, groundNormal);

                slipRatio = SlipRatio();
                slipAngle = SlipAngle();

                if (brake > 0)
                {
                    force = invSlipRes * wheelGrip * 1.5f * CombinedForce(normalForce, slipRatio / 2, slipAngle / 2);
                }
                else
                {
                    force = invSlipRes * wheelGrip * CombinedForce(normalForce, slipRatio, slipAngle);
                }

                Vector3 worldForce = transform.TransformDirection(localRotation * force);

                angularVelocity -= (force.z * radius * Time.deltaTime) / totalInertia;
                angularVelocity += driveAngularDelta;

                if (Mathf.Abs(angularVelocity) > frictionAngularDelta)
                    angularVelocity -= frictionAngularDelta * Mathf.Sign(angularVelocity);
                else
                    angularVelocity = 0;

                //wheelVelo += worldForce * (1 / body.mass) * Time.deltaTime * invSlipRes; // 1.2 Implementation
                totalForce += worldForce;
            }

            longitunalSlipVelo = Mathf.Abs(angularVelocity * radius - Vector3.Dot(wheelVelo, forwardNormal));

            lateralSlipVelo = Vector3.Dot(wheelVelo, right);

            slipVelo = Mathf.Sqrt(longitunalSlipVelo * longitunalSlipVelo + lateralSlipVelo * lateralSlipVelo);

            oldAngle = newAngle;
			//Debug.Log ("total force "+totalForce);
            return totalForce;
        }

        void UpdateModels()
        {
            ///WHEEL MODEL
            if (WheelModel != null)
            {
				

				Vector3 pos = Vector3.up * (compression - 1.0f) * suspensionHeight;
				WheelModel.transform.localPosition = new Vector3 ( pos.x,pos.y+heightOffset,pos.z);
                WheelModel.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * rotation, maxSteeringAngle * steering, 0);
/*
                if (WheelBlurModel != null)
                {
					Vector3 BlurPos = Vector3.up * (compression - 1.0f) * suspensionHeight;
					WheelBlurModel.transform.localPosition = new Vector3 ( BlurPos.x,BlurPos.y+heightOffset,BlurPos.z);
                    WheelBlurModel.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * rotation, maxSteeringAngle * steering, 0);
                }*/

//				if(WheelLocation == WheelLocationEnum.Rear)
//					Debug.Log ("tyre POs - "+WheelModel.transform.localPosition);
            }
            /*
            if (WheelBlurModel != null)
            {
                if (angularVelocity > blurSwitchVelocity)
                {
                    WheelBlurModel.active = true;
                    WheelModel.active = false;

                }
                else
                {
                    WheelModel.active = true;
                    WheelBlurModel.active = false;
                }
            }

            ///DISK MODEL
            if (DiskModel != null)
            {
                DiskModel.transform.localPosition = WheelModel.transform.localPosition + DiskModelOffset;
                DiskModel.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * rotation, maxSteeringAngle * steering, 0);
            }

            ///CALIPER MODEL
            if (CaliperModel != null)
            {
                CaliperModel.transform.localPosition = WheelModel.transform.localPosition + CaliperModelOffset;
                CaliperModel.transform.localRotation = Quaternion.Euler(0, maxSteeringAngle * steering, 0);
            }
            */



        }

        void CastFX(EarthFX.EarthFXData FX, RaycastHit SurfaceHitPoint)
        {

            /// BRAKE SKIDS 
            if (FX.BrakeSkid != null && Mathf.Abs(this.slipRatio) > FX.BrakeSkidStartSlip)
            {
                Skids = FX.BrakeSkid.GetComponent(typeof(RGKCar_Skidmarks)) as RGKCar_Skidmarks;

                lastSkid = Skids.AddSkidMark(SurfaceHitPoint.point, SurfaceHitPoint.normal, Mathf.Abs(slipRatio), lastSkid);

                if (FX.BrakeSmoke != null)
                {
                    SkidSmoke = FX.BrakeSmoke.GetComponent(typeof(ParticleSystem)) as ParticleSystem;

//                    SkidSmoke.Emit(
//                        SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
//                            UnityEngine.Random.Range(-0.1f, 0.1f),
//                            UnityEngine.Random.Range(-0.1f, 0.1f)),//pos
//                            new Vector3(this.slipVelo * 0.05f, 0), //vel
//                            UnityEngine.Random.Range(SkidSmoke.minSize, SkidSmoke.maxSize) * Mathf.Clamp(0.5f, 1f, 1.5f), //size
//                            UnityEngine.Random.Range(SkidSmoke.minEnergy, SkidSmoke.maxEnergy), //energy
//                            Color.white);
                }
            }
            else
            {
                lastSkid = -1;
            }
            ///TRAIL SMOKE
            if (FX.TrailSmoke != null && Mathf.Abs(angularVelocity) > FX.TrailSmokeStartVelocity)
            {
                TrailSmoke = FX.TrailSmoke.GetComponent(typeof(ParticleSystem)) as ParticleSystem;

//                TrailSmoke.Emit(
//                    SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f),
//                        UnityEngine.Random.Range(-0.1f, 0.1f),
//                        UnityEngine.Random.Range(-0.1f, 0.1f)),
//                        new Vector3(this.slipVelo * 0.05f, 1),
//                        UnityEngine.Random.Range(TrailSmoke.minSize, TrailSmoke.maxSize) * Mathf.Clamp(0.5f, 1f, 1.5f),
//                        UnityEngine.Random.Range(TrailSmoke.minEnergy, TrailSmoke.maxEnergy), Color.white);
            }
            ///TRAIL SKIDS 
            if (FX.TrailSkid != null && Math.Abs(angularVelocity) > 5)
            {
                Trails = FX.TrailSkid.GetComponent(typeof(RGKCar_Skidmarks)) as RGKCar_Skidmarks;
                lastTrail = Trails.AddSkidMark(SurfaceHitPoint.point, SurfaceHitPoint.normal, Mathf.Abs(1) - 0.2f, lastTrail);
            }
            else
            {
                lastTrail = -1;
            }
            ///SPLATTERS
            if (FX.Splatter != null && Math.Abs(angularVelocity) > 5 && !surfaceChanged)
            {
                Splatter = FX.Splatter.GetComponent(typeof(ParticleSystem)) as ParticleSystem;
//                Splatter.Emit(
//                    SurfaceHitPoint.point + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f),
//                        UnityEngine.Random.Range(-0.1f, 0.2f),
//                        UnityEngine.Random.Range(-0.1f, 0.1f)),
//                        new Vector3(this.slipVelo * 0.05f, 0),
//                        UnityEngine.Random.Range(Splatter.minSize, Splatter.maxSize),
//                        UnityEngine.Random.Range(Splatter.minEnergy, Splatter.maxEnergy), Color.white);

            }
            ///DRIVE SOUND
            if (FX.SurfaceDriveSound != null)
            { SurfaceSound = FX.SurfaceDriveSound; }
            else
            { SurfaceSound = null; }

            ///BRAKE SOUND
            if (FX.BrakeSound != null)
            { BrakeSound = FX.BrakeSound; }
            else
            {
                BrakeSound = null;
            }

            ///FORWARDGRIP LOSE PERCENT
            if (FX.ForwardGripLosePercent > 0)
            {
                wheelGrip = definedGrip - (definedGrip * (FX.ForwardGripLosePercent / 100f));
            }
            else
            {
                wheelGrip = definedGrip;
            }

            ///SIDEGRIP LOSE PERCENT
            if (FX.SidewaysGripLosePercent > 0)
            {
                sideGrip = definedSideGrip - (definedSideGrip * (FX.SidewaysGripLosePercent / 100f));
            }
            else
            {
                sideGrip = definedSideGrip;
            }

            ///DRAG
            SurfaceAngularDrag = FX.AngularDrag;
            SurfaceForwardDrag = FX.ForwardDrag;

        }

        //void BurnUpdate()
        //{
        //    if (this.BurnoutStart && driveTorque>0)
        //    {
        //        if (this.isPowered)
        //        {
        //            if (this.angularVelocity > -10 && this.angularVelocity < 30)
        //            {
        //                this.wheelGrip = Mathf.Clamp(0.5f * Time.deltaTime * 5, 0.2f, definedGrip);
        //            }
        //            else
        //            {
        //                this.wheelGrip = definedGrip;
        //            }
        //        }
        //        else
        //        {
        //            this.wheelGrip = definedGrip;
        //        }
        //    }
        //    else
        //    {
        //        this.wheelGrip = definedGrip;
        //    }

        //}


    }


}