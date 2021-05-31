using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorDetails : MonoBehaviour 
{

	// Use this for initialization
	public List<Material> carMat;
	//public int totalCars;

	public Material GetMaterial()
	{
		Material mat = carMat[0];
		if (carMat.Count > 0) 
		{
			int index = Random.Range (0,carMat.Count);
			mat = carMat [index];
			carMat.RemoveAt (index);


		}
			
		return mat;


	}

}
