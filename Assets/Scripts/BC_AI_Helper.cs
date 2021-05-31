using UnityEngine;
using System.Collections;

public class BC_AI_Helper : MonoBehaviour
{
    public int CarID;
    vehicleHandling VC;
    public BC_AI_NavMeshPathCalculator pathCalculator;
    public float maxSteer = 15.0f;
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
    public bool reachedPlayerCar = false;
    // Use this for initialization
    void Start()
    {
        CarRigidBody = GetComponent<Rigidbody>();
        VC = GetComponent<vehicleHandling>();
    }
    void GetSteer()
    {
        if (pathCalculator != null && pathCalculator.path != null && pathCalculator.path.corners !=null && pathCalculator.path.corners.Length > 0 && pathCalculator.path.corners[1]!=null)
        {
            Vector3 steerVector = transform.InverseTransformPoint(pathCalculator.path.corners[1]);
            float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
            VC._SteeringInput = newSteer;
        }


    }
    // Update is called once per frame
    void Update()
    {
        Move();
        GetSteer();
        VC.selfRight();
        Sensors();
        Respawn();
    }
    void Move()
    {
        //Debug.Log(Vector3.Distance(transform.position, pathCalculator.path.corners[pathCalculator.path.corners.Length - 1]));
        //inSector = (Vector3.Distance(transform.position, pathCalculator.path.corners[pathCalculator.path.corners.Length - 1]) <= 5f);
        //CarRigidBody.isKinematic = inSector;


        if (VC._VehicleSpeed <= VC.MAXvehicleSpeed)
        {
            if (!reversing)
            {
                VC._AccelerationInput = 1f;

            }
            else
            {
                VC._AccelerationInput = -1f;

            }

        }
    }
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
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
            {
                flag++;
                reversing = true;
                Debug.DrawLine(pos, hit.point, Color.red);
            }
        }
        else
        {
            reversing = false;
        }


        //Front Straight Right Sensor
        pos += transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
            {
                flag++;
                avoidSenstivity -= 1;
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
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
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
            {
                flag++;
                avoidSenstivity += 1;
                Debug.DrawLine(pos, hit.point, Color.black);
            }
        }
        else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
            {
                flag++;
                avoidSenstivity += 0.5f;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }

        //Right SideWay Sensor
        if (Physics.Raycast(transform.position + (transform.right * frontSensorSideDist), transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
            {
                flag++;
                avoidSenstivity -= 0.5f;
                Debug.DrawLine(transform.position + (transform.right * frontSensorSideDist), hit.point, Color.cyan);
            }
        }


        //Left SideWay Sensor
        if (Physics.Raycast(transform.position - (transform.right * frontSensorSideDist), -transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
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
                if (hit.transform.tag != Constants.TAG_PLAYER_CAR)
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
        VC._SteeringInput = avoidSpeed * senstivity;


    }
    void Respawn()
    {
//        if (reachedPlayerCar)
//        {
//            return;
//        }
//        if (CarRigidBody.velocity.magnitude < 2)
//        {
//            respawnCounter += Time.deltaTime;
//
//            if (respawnCounter >= respawnWait)
//            {
//                FindObjectOfType<SpawnManager>().RespawnCar(gameObject);
//                respawnCounter = 0;
//
//            }
//        }
    }
}