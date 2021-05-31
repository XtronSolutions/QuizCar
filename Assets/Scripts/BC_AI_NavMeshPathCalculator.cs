using UnityEngine;
using System.Collections;
public class BC_AI_NavMeshPathCalculator : MonoBehaviour
{
    public Color Path_Color;
	public Transform target;
    public UnityEngine.AI.NavMeshPath path;
    private float elapsed = 0.0f;
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag("PlayerCar").transform;
        path = new UnityEngine.AI.NavMeshPath();
        elapsed = 0.0f;
        //NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
    }
    
    void Update()
    {
        // Update the way to the goal every second.
        if (target != null) {
//            elapsed += Time.deltaTime;
//            if (elapsed > 0.0f)
//            {
                elapsed -= 1.0f;
			UnityEngine.AI.NavMesh.CalculatePath(transform.position, target.position, UnityEngine.AI.NavMesh.AllAreas, path);
//			NavMesh.CalculatePath(target1.position, target.position, NavMesh.AllAreas, path);

//            }
            for (int i = 0; i < path.corners.Length - 1; i++)
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Path_Color);
        }
        
    }
}