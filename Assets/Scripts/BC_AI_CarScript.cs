using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BC_AI_CarScript : MonoBehaviour
{
    public Vector3 centerOfMass;
    public BC_AI_NavMeshPathCalculator pathCalculator;
    //public ArrayList path;
    //public Transform pathGroup;
    public float maxSteer = 15.0f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public Transform wheelFL_mesh;
    public Transform wheelFR_mesh;
    public Transform wheelRL_mesh;
    public Transform wheelRR_mesh;
    public int currentPathObj;
    public float distFromPath = 20;
    public float maxTorque = 50;
    public float currentSpeed;
    public float topSpeed = 150;
    public float decellarationSpeed = 10;
    //public Renderer breakingMesh;
    //public Material idleBreakLight;
    //public Material activeBreakLight;
    public bool isBreaking;
    public bool inSector;
    public float sensorLength = 5;
    public float frontSensorStartPoint = 5;
    public float frontSensorSideDist = 5;
    public float frontSensorsAngle = 30;
    public float sidewaySensorLength = 5;
    public float avoidSpeed = 10;
    private int flag = 0;
    public bool reversing = false;
    public float reverCounter = 0.0f;
    public float waitToReverse = 2.0f;
    public float reverFor = 1.5f;
    public float respawnWait = 5f;
    public float respawnCounter = 0.0f;
    Rigidbody CarRigidBody;
    private Vector3 vehicleTrajectory;
    private float vehicleSpeed;

    private WheelFrictionCurve frontGrip;
    private WheelFrictionCurve frontDrift;
    private WheelFrictionCurve rearGrip;
    private WheelFrictionCurve rearDrift;
    [Tooltip("Front wheel grip when drifting")]
    public float frontDriftStiffness = 1.1f;
    [Tooltip("Rear wheel grip when drifting")]
    public float rearDriftStiffness = 0.67f;
    public float _VehicleSpeed { get { return vehicleSpeed; } }
    private bool drifting = true;
    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        GetPath();
        CarRigidBody = GetComponent<Rigidbody>();
        //store grip curves from the left wheels (under assumption left and right should be the same)
        frontGrip = wheelFL.sidewaysFriction;
        frontDrift = wheelFL.sidewaysFriction;
        rearGrip = wheelRL.sidewaysFriction;
        rearDrift = wheelRL.sidewaysFriction;

        //set drift stiffness values from user input
        frontDrift.stiffness = frontDriftStiffness;
        rearDrift.stiffness = rearDriftStiffness;

        //start in grip mode (change right wheels)
        wheelFR.sidewaysFriction = frontGrip;
        wheelRR.sidewaysFriction = rearGrip;
    }

    void GetPath()
    {
        //Transform[] path_objs = pathGroup.GetComponentsInChildren<Transform>();
        //path = new ArrayList();

        //foreach (Transform path_obj in path_objs)
        //{
        //    if (path_obj != pathGroup)
        //        path.Add(path_obj);

        //}
    }


    void Update()
    {
        if (flag == 0)
            GetSteer();
        Move();
        moveWheels();
        //BreakingEffect();
        Sensors();
        Respawn();
    }

    public void FixedUpdate()
    {
        vehicleTrajectory = Quaternion.Inverse(transform.rotation) * CarRigidBody.velocity;
        vehicleSpeed = vehicleTrajectory.z;
        if (drifting) maintainDrift();
        detectDrift();
    }

    void GetSteer()
    {
        Vector3 steerVector = transform.InverseTransformPoint(pathCalculator.path.corners[1]);
        //Vector3 steerVector = transform.InverseTransformPoint(new Vector3(((Transform)path[currentPathObj]).position.x, transform.position.y, ((Transform)path[currentPathObj]).position.z));
        float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;

        //if (steerVector.magnitude <= distFromPath)
        //{
        //    currentPathObj++;
        //    if (currentPathObj >= pathCalculator.path.corners.Length)
        //        currentPathObj = 0;
        //    //if (currentPathObj >= path.Count)
        //    //    currentPathObj = 0;
        //}

    }
    void moveWheels()
    {
        setWheelPosition(wheelFL, wheelFL_mesh);
        setWheelPosition(wheelFR, wheelFR_mesh);
        setWheelPosition(wheelRL, wheelRL_mesh);
        setWheelPosition(wheelRR, wheelRR_mesh);
    }

    void setWheelPosition(WheelCollider col, Transform obj)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);
        obj.position = position;
        obj.rotation = rotation;
    }
    void Move()
    {
        //Debug.Log(Vector3.Distance(transform.position, pathCalculator.path.corners[pathCalculator.path.corners.Length - 1]));
        //inSector = (Vector3.Distance(transform.position, pathCalculator.path.corners[pathCalculator.path.corners.Length - 1]) <= 5f);
        //CarRigidBody.isKinematic = inSector;
        currentSpeed = 2 * (22 / 7) * wheelRL.radius * wheelRL.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
        if (currentSpeed <= topSpeed && !inSector)
        {
            if (!reversing)
            {
                wheelRL.motorTorque = maxTorque;
                wheelRR.motorTorque = maxTorque;
            }
            else
            {
                wheelRL.motorTorque = -maxTorque;
                wheelRR.motorTorque = -maxTorque;
            }
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
        else if (!inSector)
        {
            
            wheelRL.motorTorque = 0;
            wheelRR.motorTorque = 0;
            wheelRL.brakeTorque = decellarationSpeed;
            wheelRR.brakeTorque = decellarationSpeed;
        }
        
        
    }

    //void BreakingEffect()
    //{
    //    if (isBreaking)
    //    {
    //        breakingMesh.material = activeBreakLight;
    //    }
    //    else
    //    {
    //        breakingMesh.material = idleBreakLight;
    //    }

    //}

    void Sensors()
    {
        flag = 0;
        float avoidSenstivity = 0;
        Vector3 pos;
        RaycastHit hit;
        var rightAngle = Quaternion.AngleAxis(frontSensorsAngle, transform.up) * transform.forward;
        var leftAngle = Quaternion.AngleAxis(-frontSensorsAngle, transform.up) * transform.forward;



        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;

        //BRAKING SENSOR

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                wheelRL.brakeTorque = decellarationSpeed;
                wheelRR.brakeTorque = decellarationSpeed;
                Debug.DrawLine(pos, hit.point, Color.red);
            }
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }


        //Front Straight Right Sensor
        pos += transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                avoidSenstivity -= 1;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                avoidSenstivity -= 0.5f;
                flag++;
                Debug.DrawLine(pos, hit.point, Color.green);
            }
        }


        //Front Straight left Sensor
        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;
        pos -= transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                avoidSenstivity += 1;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.black);
            }
        }
        else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                avoidSenstivity += 0.5f;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }

        //Right SideWay Sensor
        if (Physics.Raycast(transform.position + (transform.right * frontSensorSideDist), transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                avoidSenstivity -= 0.5f;
                Debug.DrawLine(transform.position + (transform.right * frontSensorSideDist), hit.point, Color.cyan);
            }
        }


        //Left SideWay Sensor
        if (Physics.Raycast(transform.position - (transform.right * frontSensorSideDist), -transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "PlayerCar")
            {
                flag++;
                avoidSenstivity += 0.5f;
                Debug.DrawLine(transform.position - (transform.right * frontSensorSideDist), hit.point, Color.grey);
            }
        }

        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;
        //Front Mid Sensro                                 
        if (avoidSenstivity == 0)
        {

            if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
            {
                if (hit.transform.tag != "PlayerCar")
                {
                    if (hit.normal.x < 0)
                        avoidSenstivity = -1;
                    else
                        avoidSenstivity = 1;
                    Debug.DrawLine(pos, hit.point, Color.white);
                }
            }
        }


        if (CarRigidBody.velocity.magnitude < 2 && !reversing)
        {
            reverCounter += Time.deltaTime;
            if (reverCounter >= waitToReverse)
            {
                reverCounter = 0;
                reversing = true;
            }
        }
        else if (!reversing)
        {
            reverCounter = 0;
        }


        if (reversing)
        {
            avoidSenstivity *= -1;
            reverCounter += Time.deltaTime;
            if (reverCounter >= reverFor)
            {
                reverCounter = 0;
                reversing = false;
            }
        }


        if (flag != 0)
            AvoidSteer(avoidSenstivity);


    }


    void AvoidSteer(float senstivity)
    {
        wheelFL.steerAngle = avoidSpeed * senstivity;
        wheelFR.steerAngle = avoidSpeed * senstivity;

    }


    void Respawn()
    {
        if (CarRigidBody.velocity.magnitude < 2 && !inSector)
        {
            respawnCounter += Time.deltaTime;
            if (respawnCounter >= respawnWait)
            {
                //if (currentPathObj == 0)
                //{
                //    transform.position = pathCalculator.path.corners[pathCalculator.path.corners.Length - 1]; ;
                //    //transform.position = ((Transform)path[path.Count - 1]).position;
                //}
                //else
                //{
                //    transform.position = pathCalculator.path.corners[currentPathObj - 1];
                //    //transform.position = ((Transform)path[currentPathObj - 1]).position;
                //}
                Vector3 difference = pathCalculator.path.corners[0] - pathCalculator.path.corners[1];
                Vector3 quarterPoint = pathCalculator.path.corners[0] + difference * 0.25f;
                Vector3 midPoint = pathCalculator.path.corners[0] + difference * 0.5f;

                if ((Vector3.Distance(midPoint, pathCalculator.path.corners[pathCalculator.path.corners.Length - 1])) > 15f)
                {
                    transform.position = midPoint;

                    respawnCounter = 0;
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);

                    transform.LookAt(pathCalculator.path.corners[1]);
                    CarRigidBody.velocity = Vector3.zero;
                    CarRigidBody.angularVelocity = Vector3.zero;
                }
                
            }
        }
    }
    void detectDrift()
    {
        //if slip angle is high enough then initiate drift by reducing rear sideways grip and increase front
        if (Mathf.Abs(vehicleTrajectory.x / vehicleSpeed) > 0.2)
        {
            drifting = true;

            //make grip changes
            wheelFL.sidewaysFriction = frontDrift;
            wheelFR.sidewaysFriction = frontDrift;
            wheelRL.sidewaysFriction = rearDrift;
            wheelRR.sidewaysFriction = rearDrift;
        }
    }

    void maintainDrift()
    {
        if (Mathf.Abs(vehicleTrajectory.x / vehicleSpeed) < 0.2)
        {
            drifting = false;
            //make grip changes

            wheelFL.sidewaysFriction = frontGrip;
            wheelFR.sidewaysFriction = frontGrip;
            wheelRL.sidewaysFriction = rearGrip;
            wheelRR.sidewaysFriction = rearGrip;
        }
    }
    void selfRight()
    {
        //get eulerAngles of vehicle
        Vector3 vehicleAngle = transform.eulerAngles;

        if (vehicleAngle.x > 30 && vehicleAngle.x < 320)
        {
            if (vehicleAngle.x < 180) CarRigidBody.AddTorque(transform.right * (vehicleAngle.x - 30) * -100);
            else CarRigidBody.AddTorque(transform.right * (330 - vehicleAngle.x) * 100);

        }

        if (vehicleAngle.z > 30 && vehicleAngle.z < 320)
        {
            if (vehicleAngle.z < 180) CarRigidBody.AddTorque(transform.forward * (vehicleAngle.z - 30) * -100);
            else CarRigidBody.AddTorque(transform.forward * (330 - vehicleAngle.z) * 100);
        }
    }
}