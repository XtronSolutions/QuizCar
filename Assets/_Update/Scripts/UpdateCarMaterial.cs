using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCarMaterial : MonoBehaviour
{
    public List<Material> carMaterials;

    public Material GetRelatedMaterial(int _index)
    {
        return carMaterials[_index];
    }

}
