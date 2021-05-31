using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DessertEnvToggler : MonoBehaviour {

    public GameObject[] Trees;
    public GameObject Mountains;
    public GameObject[] houses;
    public GameObject[] stones;
    public GameObject[] bushes;
    public GameObject[] Walls;
    public GameObject[] Mummies;
    public GameObject[] Markets;
    public GameObject[] Pyramids;
    public GameObject[] Fences;
    public GameObject[] Torches;
    public GameObject[] UIButtns;
    public GameObject Dust;
    public Button[] QualityButtons;
    public Text SelectedQuality;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ToggleMummies(Toggle val)
    {
        foreach (GameObject bush in Mummies)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void ToggleMarkets(Toggle val)
    {
        foreach (GameObject bush in Markets)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void TogglePyramids(Toggle val)
    {
        foreach (GameObject bush in Pyramids)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void ToggleFences(Toggle val)
    {
        foreach (GameObject bush in Fences)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void ToggleBushes(Toggle val)
    {
        foreach (GameObject bush in bushes)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void ToggleTorches(Toggle val)
    {
        foreach (GameObject bush in Torches)
        {
            bush.SetActive(val.isOn);
        }
    }
    public void ToggleTrees(Toggle val)
    {
        foreach (GameObject Tree in Trees)
        {
            Tree.SetActive(val.isOn);
        }
    }
    public void ToggleHouses(Toggle val)
    {
        foreach (GameObject House in houses)
        {
            House.SetActive(val.isOn);
        }
    }
    public void Togglestones(Toggle val)
    {
        foreach (GameObject stone in stones)
        {
            stone.SetActive(val.isOn);
        }
    }
    public void ToggleWalls(Toggle val)
    {
        foreach (GameObject Wall in Walls)
        {
            Wall.SetActive(val.isOn);
        }
    }
    public void ToggleButtons(Toggle val)
    {
        foreach (GameObject Btn in UIButtns)
        {
            Btn.SetActive(val.isOn);
        }
    }
    GameObject Storm;
    public void ToggleStorm()
    {
        if (Storm == null)
            Storm = GameObject.FindGameObjectWithTag("Storm");
        Storm.SetActive(!Storm.activeSelf);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        SelectedQuality.text = QualitySettings.names[qualityIndex];
    }
    //void OnGUI()
    //{
    //    string[] names = QualitySettings.names;
    //    GUILayout.BeginVertical();
    //    int i = 0;
    //    while (i < names.Length)
    //    {
    //        if (GUILayout.Button(names[i]))
    //            QualitySettings.SetQualityLevel(i, true);

    //        i++;
    //    }
    //    GUILayout.EndVertical();
    //}
    public void HighQuality(Toggle val)
    {
        
    }
    public void ToggleMountain(Toggle val)
    {
            Mountains.SetActive(val.isOn);
    }
    public void ToggleDust(Toggle val)
    {
        Dust.SetActive(val.isOn);
    }


    public void ToggleBushesCollider(Toggle val)
    {
        foreach (GameObject bush in bushes)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;

        }
    }
    public void ToggleTreesCollider(Toggle val)
    {
        foreach (GameObject Tree in Trees)
        {
            Tree.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void ToggleHousesCollider(Toggle val)
    {
        foreach (GameObject House in houses)
        {
            House.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void TogglestonesCollider(Toggle val)
    {
        foreach (GameObject stone in stones)
        {
            stone.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void ToggleMountainCollider(Toggle val)
    {
        Mountains.GetComponent<MeshCollider>().enabled = val.isOn;
    }
    public void ToggleWallsCollider(Toggle val)
    {
        foreach (GameObject Wall in Walls)
        {
            Wall.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void ToggleMummiesCollider(Toggle val)
    {
        foreach (GameObject bush in Mummies)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void ToggleMarketsCollider(Toggle val)
    {
        foreach (GameObject bush in Markets)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void TogglePyramidsCollider(Toggle val)
    {
        foreach (GameObject bush in Pyramids)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
    public void ToggleFencesCollider(Toggle val)
    {
        foreach (GameObject bush in Fences)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;
        }
    }
  
    public void ToggleTorchesCollider(Toggle val)
    {
        foreach (GameObject bush in Torches)
        {
            bush.GetComponent<MeshCollider>().enabled = val.isOn;
        }
        
    }
}