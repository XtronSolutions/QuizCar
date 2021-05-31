using UnityEngine;
using System.Collections;

public class GameFinishOrder : MonoBehaviour 
{
	void OnTriggerEnter(Collider hit)
	{
		if (hit.tag.Equals ("Player")) 
		{
			if(CentralVariables.order == 1)
			{
				Debug.Log("------Game Finished------");
			}
		}
	}
}
